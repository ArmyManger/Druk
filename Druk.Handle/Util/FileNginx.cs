using Druk.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Druk.Handle.Util
{
    public class FileNginx
    {
        static FileConfig config = new FileConfig();

        #region //方法 上传处理

        /// <summary>
        /// 上传方法
        /// </summary>
        /// <param name="httpContext">请求对象</param>
        /// <param name="fileSaveDir">文件上传的存放路径</param>
        /// <param name="fileexeLimit">对应上传的文件后缀名限制</param>
        /// <param name="fileFrom">文件来源(前端:front 后端:back)</param>
        /// <returns></returns>
        public object Upload(Microsoft.AspNetCore.Http.HttpRequest Request, string fileSaveDir, string[] fileTypeLimit, string FileType = "image")
        {
            FileType = FileType.ToLower();
            if (Request.HasFormContentType)
            {
                var fileList = Request.Form.Files;
                if (fileList != null)
                {
                    if (fileList.Count > 0)
                    {
                        Stream stream = null;
                        try
                        {
                            var upFile = fileList.First();
                            var upFileInfo = new FileInfo(upFile.FileName);

                            //上传准入
                            var Admittance = this.Admittance(upFile, fileTypeLimit, FileType);
                            if (!Admittance.status) return new { Admittance.message };

                            if (fileTypeLimit.Contains(upFileInfo.Extension.ToLower()))
                            {
                                //混淆出的一个文件名，文件后缀还是保留
                                var fileName = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0).ToString().Substring(0, 15) + upFileInfo.Extension;
                                //拼接出一个按天存放的文件目录
                                var filePath = fileSaveDir + DateTime.Now.ToString("yyyyMMdd") + "/";
                                //形成本地的保存目录
                                var localPath = Directory.GetCurrentDirectory() + filePath + fileName;


                                #region //保存到本地,准备Nginx推送
                                //获取流对象，先保存到本地
                                stream = upFile.OpenReadStream();
                                switch (FileType)
                                {
                                    case "image": DoImage.SaveImage(localPath, stream); break;
                                    case "excel": DoExcel.SaveExcel(localPath, stream); break;
                                }
                                if (!DoIOFile.ExistsFile(localPath))
                                {
                                    //Druk.Log.Info("文件本地保存失败 [" + upFileInfo.FullName + "]", ComEnum.RabbitQueue.日志_常规);
                                    return new { message = "上传失败" };
                                }
                                #endregion


                                #region //分流处理是到移动到别的文件夹,还是移动到文件服务器

                                var assignType = FileType == "excel" ? config.UploadType_Excel : config.UploadType_Image;
                                var result = new FileNginx().FileMoveToNginx(localPath, filePath, fileName, assignType, stream);

                                #endregion

                                return result;
                            }
                            else
                            { //Druk.Log.Info("试图上传文件[" + upFile.FileName + "],文件格式不合法", ComEnum.RabbitQueue.日志_常规); return new { message = "格式不合法" };
                            }
                        }
                        catch (Exception ex)
                        {
                            //Druk.Log.Error(ex.ToSimple(), ComEnum.RabbitQueue.日志_常规);
                            return new { message = ex.Message };
                        }
                        finally
                        {
                            if (stream != null)
                            {
                                stream.Close();
                                stream.Dispose();
                            }
                        }
                    }
                }
            }
            return null;
        }



        #endregion

        #region //文件移动到最终目标位置(Local/FTPS/OSS)




        /// <summary>
        /// 文件从节点目录移动到图片站点文件夹下或者上传ftp
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="assignType">指定上传下载类型(针对后台Excel上传下载)</param>
        /// <returns></returns>
        public object FileMoveToNginx(string localFile, string filePath, string fileName, string assignType = "", Stream stream = null)
        {
            //if (!File.Exists((AppDomain.CurrentDomain.BaseDirectory + localFile).Replace("//", "/"))) return null;

            try
            {
                //进行分流(存储在同服务器文件夹/文件服务器)
                var result = false;
                var uploadType = !string.IsNullOrWhiteSpace(assignType) ? assignType : config.UploadType_Image;

                switch (uploadType.ToUpper())
                {
                    case "LOCAL":
                        //当配置是本地路径时(单节点运行), 文件移到
                        result = DoIOFile.Move(localFile, config.FileSite_Upload_LocalPath + filePath + fileName);
                        break;
                    case "FTP":
                        var clientFTP = new DoFtps(
                            config.Ftps_AddressIp,
                            config.Ftps_User,
                            config.Ftps_Password
                        );
                        result = clientFTP.Upload(localFile, filePath + fileName);
                        break;
                    case "FTPS":
                        var clientFTPS = new DoFtps(
                            config.Ftps_AddressIp,
                            config.Ftps_User,
                            config.Ftps_Password,
                            config.Ftps_Port.ToInt()
                        );
                        //配置了ftp之后,要上传到指定的文件服务器节点上
                        result = clientFTPS.Upload(filePath + fileName, localFile);

                        //如果前边报传失败了，就再验证一下是否有文件，因为发现有些报无法连接，但ftp里却传成功了。
                        //报错信息为： 由于连接方在一段时间后没有正确答复或连接的主机没有反应，连接尝试失败。 106.14.150.184:6000
                        if (!result)
                        {
                            result = new FileInfo(localFile).Length == clientFTPS.GetLength(filePath + fileName);
                        }
                        break;
                    case "OSS":
                        //var clientOSS = new DoOss(
                        //    config.OSS_AppID,
                        //    config.OSS_AppSecret,
                        //    config.OSS_Bucket,
                        //    config.OSS_EndPoint);
                        //result = clientOSS.PushFile(localFile, filePath + fileName);

                        //Druk.Log.Info("oss推送文件结果" + result);
                        break;
                }

                if (result) { return new { files = new List<object> { new { name = fileName, originalName = fileName, url = filePath + fileName } } }; }//size = upFile.Length.ToInt(),
            }
            catch (Exception ex)
            {
                Console.WriteLine("file upload：" + ex.ToJson());
                //Druk.Log.Error(ex.ToSimple(), ComEnum.RabbitQueue.日志_常规);
            }
            finally
            {
                //删除当前节点下的文件及文件夹 
                DoIOFile.Delete(localFile, true);
            }

            return null;
        }
        #endregion

        #region//文件从Nginx移动到本地

        /// <summary>
        /// 文件从Nginx移动到本地
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string FileMoveFromNginx(string localPath, string filePath, string fileName)
        {

            try
            {
                //进行分流(存储在同服务器文件夹/文件服务器)
                var result = false;
                var localFullPath = Directory.GetCurrentDirectory() + localPath + fileName;

                var uploadConfig = new FileConfig();
                var ftpSite_LocalPath = uploadConfig.FileSite_Upload_LocalPath;
                if (!string.IsNullOrEmpty(ftpSite_LocalPath))
                {
                    //当配置是本地路径时(单节点运行), 文件移到
                    result = DoIOFile.Move(ftpSite_LocalPath + filePath + fileName, localFullPath);
                }
                else
                {
                    var client = new DoFtps(uploadConfig.Ftps_AddressIp, uploadConfig.Ftps_User, uploadConfig.Ftps_Password, uploadConfig.Ftps_Port.ToInt());

                    //配置了ftp之后,从文件服务器节点上下载文件到本地
                    //client.DownLoad("远程路径例如: /upload/image/2019-03-02/lsjflsdjfl.png", Directory.GetCurrentDirectory() + "/upload/toFZ/lsjflsdjfl.png");
                    result = client.DownLoad(filePath + fileName, localFullPath);

                }

                if (result) { return localFullPath; }
            }
            catch (Exception ex)
            {
                //Druk.Log.Error(ex.ToSimple("FileMoveFromNginx"), ComEnum.RabbitQueue.日志_常规);
            }

            return null;
        }


        #endregion

        #region//文件上传准入
        /// <summary>
        /// 文件上传准入
        /// 目前只有文件大小验证,以后如果有文件尺寸等其他验证可以修改此方法
        /// </summary>
        /// <param name="IFormFile">IFormFile</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="isVerify">是否验证</param>
        /// <returns></returns>
        public (bool status, string message) Admittance(IFormFile fileInfo, string[] fileexeLimit, string fileType = "image")
        {
            #region //文件大小限制

            var limitSize = 0;

            switch (fileType.ToLower())
            {
                case "image": limitSize = new FileConfig().Size_Limit_Image; break;
                case "excel": limitSize = new FileConfig().Size_Limit_Excel; break;
                default: break;
            }

            if (fileInfo.Length > limitSize * 1024) return (false, $"文件大小超过({limitSize}KB)限制");

            #endregion


            return (true, "Success");
        }
        #endregion
    }
}

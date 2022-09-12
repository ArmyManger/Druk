using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// Ftps操作类 使用 FluentFTP.dll 
    /// 官方GitHub https://github.com/robinrodricks/FluentFTP
    /// </summary>
    public class DoFtps
    {
        #region //属性
        string ipAddress { get; set; }
        string UserName { get; set; }
        string UserPwd { get; set; }
        int Port { get; set; }
        FtpEncryptionMode EncryMode { get; set; }
        SslProtocols SslMode { get; set; }
        FtpDataConnectionType ConnectMode { get; set; }



        //客户端对象
        FtpClient client { get; set; }
        #endregion

        #region //构造
        public DoFtps(string Url, string LoginName, string LoginPwd, int FtpPort = 990, FtpEncryptionMode Encry = FtpEncryptionMode.Implicit, SslProtocols Ssl = SslProtocols.Tls, FtpDataConnectionType Connect = FtpDataConnectionType.PASV)
        {
            ipAddress = Url;
            UserName = LoginName;
            UserPwd = LoginPwd;
            Port = FtpPort;
            EncryMode = Encry;
            SslMode = Ssl;
            ConnectMode = Connect;
        }
        #endregion

        #region //连接

        #region //打开连接
        void OpenClient()
        {

            try
            {
                if (client == null || client.IsDisposed) //初始化
                {
                    NetworkCredential netCredential = new NetworkCredential(UserName, UserPwd);  //用户名密码
                    client = new FtpClient(ipAddress, Port, netCredential);


                    client.SslProtocols = SslMode; //要使用的加密方式
                    client.DataConnectionEncryption = true; //是否要加密
                    client.EncryptionMode = EncryMode; //加密的形式  Implicit: 绝对加密    或者是相对加密
                    client.DataConnectionType = ConnectMode; //请求的模式  PASV:被动模式  PORT:主动模式

                    //Uri 形式的链接方式 作为参考
                    //client = FtpClient.Connect(new Uri("ftps://sunyanjie:KgsL1i38@139.224.118.153:990"), false);


                    client.ValidateCertificate += new FtpSslValidation(delegate (FtpClient control, FtpSslValidationEventArgs e) { e.Accept = true; }); //默认不检查证书 直接接受 Accept=true
                }
                if (!client.IsConnected) //连接服务
                {
                    client.Connect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region //关闭连接

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close()
        {
            if (client != null && client.IsConnected)
            {
                client.Disconnect();
                client.Dispose();
            }
        }

        #endregion

        #endregion

        #region //上传

        #region //上传单个文件

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="remotePath">带文件名的服务器路径</param>
        /// <param name="localPath">本地文件</param>
        /// <returns></returns>
        public bool Upload(string remotePath, string localPath)
        {
            try
            {
                remotePath = remotePath.TrimStart('/').TrimStart('\\');
                var file = new FileInfo(localPath);
                if (file.Exists)
                {
                    OpenClient(); //初始化连接
                    //检查文件夹是否存在,不存在则创建
                    client.CreateDirectory(Path.GetDirectoryName(remotePath), false);

                    #region //文件上传
                    using (Stream istream = new FileStream(localPath, FileMode.Open, FileAccess.Read), ostream = client.OpenWrite(remotePath, FtpDataType.ASCII, false))
                    {
                        byte[] buf = new byte[8192]; //每次读取8K数据
                        int read = 0;
                        while ((read = istream.Read(buf, 0, buf.Length)) > 0)
                        {
                            ostream.Write(buf, 0, read);
                        }
                    }

                    client.GetReply(); //获取一下服务器应答..
                    #endregion

                    #region //验证服务器上的文件大小,与当前文件对应比较
                    if (client.GetFileSize(remotePath) == file.Length)
                    {
                        Console.WriteLine("Info:本地文件[" + localPath + "]上传至[" + remotePath + "]成功!");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Info:本地文件[" + localPath + "]上传至[" + remotePath + "]失败!上传文件大小比较失败!!");
                        if (client.FileExists(remotePath)) { client.DeleteFile(remotePath); } //这时候ftp上是有文件的,但大小不一致,删除之
                        return false;
                    }
                    #endregion
                }
                Console.WriteLine("Error:本地文件[" + localPath + "]上传至[" + remotePath + "]失败;原因本地文件不存在!");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:本地文件[" + localPath + "]上传至[" + remotePath + "]失败;原因" + ex.Message);
                return false;
            }
            finally
            {
                Close();
            }
        }

        #endregion

        #region //上传文件夹

        /// <summary>
        /// 上传文件夹 
        /// </summary>
        /// <param name="remoteDir">服务器路径</param>
        /// <param name="localDir">本地文件夹</param>
        /// <returns></returns>
        public bool Upload_Folder(string remoteDir, string localDir, bool IsUploadChild = true)
        {
            try
            {
                remoteDir = remoteDir.TrimStart('/').TrimStart('\\');
                var list = DoIOFile.GetFiles(localDir, IsUploadChild) ?? new List<string>();
                if (list.Count > 0)
                {
                    int index = 0;
                    OpenClient(); //初始化连接
                    list.ForEach(localFilePath =>
                    {
                        var file = new FileInfo(localFilePath);
                        if (file.Exists)
                        {
                            var remoteFilePath = (remoteDir + localFilePath.Replace(localDir, "").Replace('\\', '/')).Replace("//", "/");

                            if (client.FileExists(remoteFilePath)) { client.DeleteFile(remoteFilePath); } //文件如果存在,则删除
                            client.CreateDirectory(Path.GetDirectoryName(remoteFilePath), false); //创建文件夹

                            #region //文件上传
                            using (Stream istream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read), ostream = client.OpenWrite(remoteFilePath, FtpDataType.ASCII, false))
                            {
                                byte[] buf = new byte[8192]; //每次读取8K数据
                                int read = 0;
                                while ((read = istream.Read(buf, 0, buf.Length)) > 0)
                                {
                                    ostream.Write(buf, 0, read);
                                }
                            }

                            client.GetReply(); //获取一下服务器应答..

                            #endregion

                            #region //验证服务器上的文件大小,与当前文件对应比较
                            var size = client.GetFileSize(remoteFilePath);

                            if (size == file.Length)
                            {
                                Console.WriteLine("Info:本地文件[" + localFilePath + "]上传至[" + remoteFilePath + "]成功!");
                                index++;
                            }
                            else
                            {
                                Console.WriteLine("Info:本地文件[" + localFilePath + "]上传至[" + remoteFilePath + "]失败!上传文件大小比较失败!!");
                                if (client.FileExists(remoteFilePath)) { client.DeleteFile(remoteFilePath); } //这时候ftp上是有文件的,但大小不一致,删除之
                                return;
                            }
                            #endregion
                        }
                    });

                    return list.Count == index;
                }
                Console.WriteLine("Error:本地文件夹[" + localDir + "]上传至[" + remoteDir + "]失败!原因:本地文件不存在!");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:本地文件夹[" + localDir + "]上传至[" + remoteDir + "]失败!原因:" + ex.Message);
                return false;
            }

        }

        #endregion

        #endregion

        #region //下载

        #region //下载单个文件
        /// <summary>
        /// 下载单个文件
        /// </summary>
        /// <param name="RemotePath">服务器路径</param>
        /// <param name="LocalPath">本地绝对路径</param>
        /// <returns></returns>
        public bool DownLoad(string RemotePath, string LocalPath)
        {
            try
            {
                OpenClient(); //初始化连接
                RemotePath = RemotePath.TrimStart('/').TrimStart('\\');
                if (client.FileExists(RemotePath))
                {
                    DoIOFile.Delete(LocalPath); //先删除文件
                    var result = client.DownloadFile(LocalPath, RemotePath);
                    if (result)
                    {
                        if (client.GetFileSize(RemotePath) == new FileInfo(LocalPath).Length)
                        {
                            Console.WriteLine("Error:下载文件成功;[" + RemotePath + "]至[" + LocalPath + "]");
                            return true;
                        }
                    }
                    DoIOFile.Delete(LocalPath); //删除文件
                    Console.WriteLine("Error:下载文件失败;[" + RemotePath + "]至[" + LocalPath + "]");
                    return false;
                }
                Console.WriteLine("Error:下载文件失败;远程文件不存在!![" + RemotePath + "]至[" + LocalPath + "]");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:下载文件异常;[" + RemotePath + "]至[" + LocalPath + "],原因:" + ex.Message);
                return false;
            }
            finally
            {
                Close();
            }
        }
        #endregion

        #region //下载文件夹

        /// <summary>
        /// 下载文件夹  不成功
        /// </summary>
        /// <param name="RemotePath">服务器路径</param>
        /// <param name="LocalPath">本地路径</param>
        /// <param name="IsDownLoadChild">是否下载子文件夹</param>
        /// <returns></returns>
        public bool DownLoad_Folder(string RemotePath, string LocalPath, bool IsDownLoadChild = true)
        {
            try
            {
                OpenClient(); //初始化连接
                RemotePath = RemotePath.TrimStart('/').TrimStart('\\');
                if (client.DirectoryExists(RemotePath))
                {
                    var FileList = ScanPath_File(RemotePath, IsDownLoadChild);
                    int index = 0;
                    FileList.ForEach(remoteFilePath =>
                    {
                        var local = DoPath.Combine(LocalPath, remoteFilePath).Replace(RemotePath.Replace('/', '\\'), ""); //本地路径
                        DoIOFile.Delete(local); //删除本地文件
                        if (client.DownloadFile(local, remoteFilePath) && client.GetFileSize(remoteFilePath) == new FileInfo(local).Length)
                        {
                            Console.WriteLine("Info:下载文件成功![" + remoteFilePath + "]至[" + local + "]");
                            index++;
                        }
                        else
                        {
                            Console.WriteLine("Error:下载文件失败;[" + remoteFilePath + "]至[" + local + "]");
                            DoIOFile.Delete(local); //删除本地文件
                            return;
                        }
                    });
                    Console.WriteLine("Info:下载文件夹成功![" + RemotePath + "]至[" + LocalPath + "]");
                    return FileList.Count == index;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:下载文件夹异常;[" + RemotePath + "]至[" + LocalPath + "],原因:" + ex.Message);
                return false;
            }
            finally
            {
                Close();
            }
        }
        #endregion     

        #endregion

        #region //删除

        #region //删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="RemotePath">带文件名的文件路劲</param>
        /// <returns></returns>
        public bool Delete(string RemotePath)
        {
            try
            {
                OpenClient();
                RemotePath = RemotePath.TrimStart('/').TrimStart('\\');
                if (client.FileExists(RemotePath))
                {
                    client.DeleteFile(RemotePath);
                    Console.WriteLine("Info:删除文件成功![" + RemotePath + "]");
                    return !client.FileExists(RemotePath);
                }
                Console.WriteLine("Info:删除文件![" + RemotePath + "]原因:文件不存在!返回True");
                return true; //删除的目的便是让文件不存在,所以返回True
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:删除文件失败![" + RemotePath + "],原因:" + ex.Message);
                return false;
            }
            finally
            {
                Close();  //关闭连接
            }
        }
        #endregion

        #region //删除文件夹
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="RemotePath">文件夹路径</param>
        /// <param name="IsDelWhenNotEmtry">如果有内容,是否依然删除,是则会递归删除其中所有内容,包括文件与文件夹</param>
        /// <returns></returns>
        public bool Delete_Folder(string RemotePath, bool IsDelWhenNotEmtry = false)
        {
            try
            {
                OpenClient(); //初始化连接
                RemotePath = RemotePath.TrimStart('/').TrimStart('\\');
                if (client.DirectoryExists(RemotePath))
                {
                    var List = client.GetListing(RemotePath); //获取全部列表
                    if (List.Length > 0)
                    {
                        if (IsDelWhenNotEmtry)
                        {
                            var fileList = ScanPath_File(RemotePath); //递归获取所有文件路径

                            //删除全部文件
                            fileList.ForEach(o =>
                            {
                                client.DeleteFile(o);
                                Console.WriteLine("Info:删除文件[" + Path.GetDirectoryName(o) + "]成功!");
                            });

                            var dirList = ScanPath_Floder(RemotePath); //递归获取所有文件夹路径
                            dirList = dirList.OrderByDescending(o => o.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).Length).ToList(); //已文件夹层数倒序排列 先删除最深的
                            dirList.Add(RemotePath); //添加本身文件夹

                            //删除全部文件夹
                            dirList.ForEach(o =>
                            {
                                client.DeleteDirectory(o);
                                Console.WriteLine("Info:删除文件夹[" + o + "]成功!");
                            });
                        }
                        else
                        {
                            Console.WriteLine("Error:删除文件夹[" + RemotePath + "]失败!原因:该文件夹内有文件!");
                            return false;  //如果有文件不删除.. 则失败
                        }
                    }
                    else
                    {
                        client.DeleteDirectory(RemotePath); //没有内容则直接删除当前文件夹
                        Console.WriteLine("Info:删除文件夹[" + RemotePath + "]成功!");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error::删除文件夹[" + RemotePath + "]失败!原因:" + ex.Message);
                return false;
            }
            finally
            {
                Close();  //关闭连接
            }
        }
        #endregion

        #endregion

        #region //获取文件长度

        /// <summary>
        /// 获取文件的长度  不存在则返回 -1
        /// </summary>
        /// <param name="RemotePath">文件路径</param>
        /// <returns></returns>
        public long GetLength(string RemotePath)
        {
            try
            {
                OpenClient(); //初始化连接
                RemotePath = RemotePath.TrimStart('/').TrimStart('\\');
                if (client.FileExists(RemotePath))
                {
                    var length = client.GetFileSize(RemotePath);  //只要能够获取长度
                    Console.WriteLine("Info:获取文件长度成功,文件长度[" + length + "]");
                    return length;
                }
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:获取文件长度失败;原因" + ex.Message);
                return -1;
            }
            finally
            {
                Close();
            }
        }
        #endregion

        #region //扫描所有文件路径

        /// <summary>
        /// 获取指定路径下的所有文件
        /// </summary>
        /// <param name="RemotePath">为空或者为 /  时获取根目录</param>
        /// <returns></returns>
        public List<string> Scan_File(string RemotePath, bool IsScanChild = true)
        {
            try
            {
                OpenClient(); //初始化连接                
                RemotePath = RemotePath.TrimStart('/').TrimStart('\\');
                if (client.DirectoryExists(RemotePath))
                {
                    //扫描文件夹
                    var result = ScanPath_File(RemotePath, IsScanChild);
                    Console.WriteLine("Info:获取文件夹[" + RemotePath + "]下所有文件成功!共计" + result.Count + "个文件!");
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:获取文件夹[" + RemotePath + "]下所有文件失败;原因:" + ex.Message);
                return null;
            }
            finally
            {
                Close();  //关闭连接
            }
        }
        #endregion

        #region //扫描所有文件夹列表

        /// <summary>
        /// 获取指定路径下的文件夹列表
        /// </summary>
        /// <param name="RemotePath">为空或者为 /  时获取根目录</param>
        /// <returns></returns>
        public List<string> Scan_Folder(string RemotePath, bool IsScanChild = true)
        {
            try
            {
                OpenClient(); //初始化连接                
                RemotePath = RemotePath.TrimStart('/').TrimStart('\\');
                if (client.DirectoryExists(RemotePath))
                {
                    //扫描文件夹
                    var result = ScanPath_Floder(RemotePath, IsScanChild);
                    Console.WriteLine("Info:获取所有文件夹成功!共计" + result.Count + "个![" + RemotePath + "]");
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:获取所有文件夹失败;[" + RemotePath + "],原因:" + ex.Message);
                return null;
            }
            finally
            {
                Close();  //关闭连接
            }
        }
        #endregion



        #region //工具

        #region //工具 扫描指定文件夹下的所有文件夹,子文件夹

        /// <summary>
        /// 获取指定文件夹下的所有文件夹路径
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns></returns>
        List<string> ScanPath_Floder(string folderPath, bool IsScanChild = true)
        {
            var list = new List<string>();

            var dirList = client.GetListing(folderPath).Where(o => o.Type == FtpFileSystemObjectType.Directory).ToList();
            list.AddRange(dirList.Select(o => DoPath.Combine(folderPath, o.Name)));

            //获取子文件夹路径
            if (IsScanChild)
            {
                foreach (var dir in dirList)
                {
                    list.AddRange(ScanPath_Floder(DoPath.Combine(folderPath, dir.Name)));
                }
            }
            return list;
        }

        #endregion

        #region //工具 扫描指定文件夹下的所有文件,子文件

        /// <summary>
        /// 获取指定文件夹下的所有文件路径
        /// </summary>
        /// <param name="folderPath">文件路径</param>
        /// <returns></returns>
        List<string> ScanPath_File(string folderPath, bool IsScanChild = true)
        {
            var list = new List<string>();


            var allList = client.GetListing(folderPath);
            list.AddRange(allList.Where(o => o.Type != FtpFileSystemObjectType.Directory).ToList().Select(o => DoPath.Combine(folderPath, o.Name)));

            if (IsScanChild)
            {
                //文件夹列表
                var dirList = allList.Where(o => o.Type == FtpFileSystemObjectType.Directory).ToList();

                foreach (var dir in dirList)
                {
                    list.AddRange(ScanPath_File(DoPath.Combine(folderPath, dir.Name), IsScanChild));
                }
            }
            return list;
        }
        #endregion

        #endregion

    }
}

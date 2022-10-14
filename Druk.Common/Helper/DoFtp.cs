using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Druk.Common
{
    public class DoFtp
    {
        #region //连接FTP
        string ftpServerIP;
        string ftpRemotePath;
        string ftpUserID;
        string ftpPassword;
        string ftpURI;
        int sftpPort;


        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="ftpServerIp">FTP连接地址</param>
        /// <param name="ftpPort">端口号</param>
        /// <param name="ftpUserId">用户名</param>
        /// <param name="ftpPassword">密码</param>
        public DoFtp(string ftpServerIp, int ftpPort, string ftpUserId, string ftpPassword)
        {
            ftpServerIP = ftpServerIp;
            sftpPort = ftpPort;
            ftpUserID = ftpUserId;
            this.ftpPassword = ftpPassword;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="localPath">本地地址</param>
        /// <param name="remotePath">服务器地址</param>
        public bool Download(string localPath, string remotePath)
        {
            try
            {
                using var client = new SftpClient(ftpServerIP, sftpPort, ftpUserID, ftpPassword);
                client.Connect(); //连接
                var buffer = client.ReadAllBytes(remotePath);
                var file = new FileInfo(localPath);
                if (file.Directory != null && !file.Directory.Exists) { file.Directory.Create(); }
                File.WriteAllBytes(localPath, buffer);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:下载文件异常;[" + remotePath + "]至[" + localPath + "],IP:" + ftpServerIP + ",sftpPort:" + sftpPort + ",sftpUserID:" + ftpUserID + ",sftpPassword:" + ftpPassword + ",原因:" + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="ftpServerIp">FTP连接地址</param>
        /// <param name="ftpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="ftpUserId">用户名</param>
        /// <param name="ftpPassword">密码</param>
        public DoFtp(string ftpServerIp, string ftpRemotePath, string ftpUserId, string ftpPassword)
        {
            ftpServerIP = ftpServerIp;
            this.ftpRemotePath = ftpRemotePath;
            ftpUserID = ftpUserId;
            this.ftpPassword = ftpPassword;
            ftpURI = "ftp://" + ftpServerIP + "/" + this.ftpRemotePath + "/";
        }
        #endregion

        #region //上传
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="filename">文件名称</param>
        public void Upload(string filename)
        {
            var fileInf = new FileInfo(filename);
            var uri = ftpURI + fileInf.Name;
            var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(uri));
            reqFtp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFtp.KeepAlive = false;
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
            reqFtp.UseBinary = true;
            reqFtp.ContentLength = fileInf.Length;
            const int buffLength = 2048;
            var buff = new byte[buffLength];
            var fs = fileInf.OpenRead();
            try
            {
                var strm = reqFtp.GetRequestStream();
                var contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "Upload Error --> " + ex.Message);
            }

        }
        #endregion

        #region //上传
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="filename">要上传的文件全路径：@"D:\123\123.txt"</param>
        /// <param name="path">基于要上传的文件夹下的自路径： @"234\345\" 如果该文件夹不存在则创建</param>
        public bool Upload(string filename, string path)
        {
            if (path == "//" || path == "\\")
            {
                path = "";
            }
            else if (!path.TrimStart('/').Contains("/"))
            {
            }
            else
            {
                FtpCheckDirectoryExist(path);
            }
            var fileInf = new FileInfo(filename);
            // string uri = ftpURI + path + fileInf.Name;
            var uri = ftpURI + path;
            var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(uri));
            reqFtp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFtp.KeepAlive = false;
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
            reqFtp.UseBinary = true;
            reqFtp.ContentLength = fileInf.Length;
            var buffLength = 2048;
            var buff = new byte[buffLength];
            var fs = fileInf.OpenRead();
            try
            {
                var strm = reqFtp.GetRequestStream();
                var contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                //Insert_Standard_ErrorLog.Insert("FtpWeb", "Upload Error --> " + ex.Message);
                Console.WriteLine("Upload Error --> " + ex.Message);
                return false;
            }

        }
        #endregion


        #region //下载
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public bool DownLoad(string filePath, string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                var fileP = Path.GetDirectoryName(filePath + fileName);
                if (!Directory.Exists(fileP))//如果不存在就创建 dir 文件夹  
                    Directory.CreateDirectory(fileP);

                FileStream outputStream = new FileStream(filePath + fileName, FileMode.Create);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();

                return true;//
            }

            catch (Exception ex)
            {
                //Insert_Standard_ErrorLog.Insert("FtpWeb", "Download Error --> " + ex.Message);
                Console.WriteLine("Download Error --> " + ex.Message);
                return false;
            }

        }
        #endregion




        #region //删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        public void Delete(string fileName)
        {
            try
            {
                string uri = ftpURI + fileName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "Delete Error --> " + ex.Message + "  文件名:" + fileName);
            }
        }
        #endregion


        #region //删除文件夹
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderName"></param>
        public void RemoveDirectory(string folderName)
        {
            try
            {
                string uri = ftpURI + folderName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "Delete Error --> " + ex.Message + "  文件名:" + folderName);
            }
        }
        #endregion



        #region //获取当前目录下明细(包含文件和文件夹) 
        /// <summary>
        /// 获取当前目录下明细(包含文件和文件夹) 
        /// </summary>
        /// <returns></returns>
        public string[] GetFilesDetailList()
        {
            string[] downloadFiles;
            try
            {
                StringBuilder result = new StringBuilder();
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                //while (reader.Read() > 0)

                //{



                //}

                string line = reader.ReadLine();
                //line = reader.ReadLine();
                //line = reader.ReadLine();

                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                downloadFiles = null;
                Insert_Standard_ErrorLog.Insert("FtpWeb", "GetFilesDetailList Error --> " + ex.Message);
                return downloadFiles;
            }

        }
        #endregion



        #region //获取当前目录下文件列表(仅文件)
        /// <summary>
        /// 获取当前目录下文件列表(仅文件)
        /// </summary>
        /// <returns></returns>
        public string[] GetFileList(string mask)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (mask.Trim() != string.Empty && mask.Trim() != "*.*")
                    {
                        string mask_ = mask.Substring(0, mask.IndexOf("*"));
                        if (line.Substring(0, mask_.Length) == mask_)
                        {
                            result.Append(line);
                            result.Append("\n");
                        }
                    }
                    else
                    {
                        result.Append(line);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                downloadFiles = null;
                if (ex.Message.Trim() != "远程服务器返回错误: (550) 文件不可用(例如，未找到文件，无法访问文件)。")
                {
                    Insert_Standard_ErrorLog.Insert("FtpWeb", "GetFileList Error --> " + ex.Message.ToString());
                }
                return downloadFiles;
            }
        }
        #endregion



        #region //判断文件的目录是否存,不存则创建
        /// <summary>
        /// 判断文件的目录是否存,不存则创建
        /// </summary>
        /// <param name="destFilePath">要上传的文件 ftpRemotePath 这个文件夹下的 文件夹路径 如 234/123/</param>
        public void FtpCheckDirectoryExist(string destFilePath)
        {
            string fullDir = FtpParseDirectory(destFilePath);
            string[] dirs = fullDir.Split('/');
            string curDir = "/";
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空  
                if (dir != null && dir.Length > 0)
                {
                    try
                    {
                        curDir += dir + "/";
                        FtpMakeDir(curDir);
                    }
                    catch (Exception)
                    { }
                }
            }
        }
        #endregion

        public string FtpParseDirectory(string destFilePath)
        {
            return destFilePath.Substring(0, destFilePath.LastIndexOf("/"));
        }

        #region //创建目录
        //创建目录
        public Boolean FtpMakeDir(string localFile)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpURI + localFile);
            req.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            req.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception)
            {
                req.Abort();
                return false;
            }
            req.Abort();
            return true;
        }
        #endregion


        #region //判断当前目录下指定的文件是否存在
        /// <summary>
        /// 判断当前目录下指定的文件是否存在
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        public bool FileExist(string RemoteFileName)
        {
            string[] fileList = GetFileList("*.*");
            foreach (string str in fileList)
            {
                if (str.Trim() == RemoteFileName.Trim())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region //创建文件夹

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirName"></param>
        public void MakeDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                // dirName = name of the directory to create.
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + dirName));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "MakeDir Error --> " + ex.Message);
            }
        }
        #endregion


        #region //获取指定文件大小

        /// <summary>
        /// 获取指定文件大小
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize = 0;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + filename));
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                ftpStream.Close();
                response.Close();

            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "GetFileSize Error --> " + ex.Message);
            }
            return fileSize;

        }
        #endregion



        #region //改名
        /// <summary>
        /// 改名
        /// </summary>
        /// <param name="currentFilename"></param>
        /// <param name="newFilename"></param>
        public void ReName(string currentFilename, string newFilename)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + currentFilename));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Insert_Standard_ErrorLog.Insert("FtpWeb", "ReName Error --> " + ex.Message);
            }

        }
        #endregion



        #region //移动文件
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="currentFilename"></param>
        /// <param name="newDirectory"></param>
        public void MovieFile(string currentFilename, string newDirectory)
        {
            ReName(currentFilename, newDirectory);
        }
        #endregion



        #region //切换当前目录
        /// <summary>
        /// 切换当前目录
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <param name="IsRoot">true 绝对路径   false 相对路径</param>
        public void GotoDirectory(string DirectoryName, bool IsRoot)
        {
            if (IsRoot)
            {
                ftpRemotePath = DirectoryName;
            }
            else
            {
                ftpRemotePath += DirectoryName + "/";
            }
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
        }
        #endregion 
    }
     
    public class Insert_Standard_ErrorLog
    {
        public static void Insert(string x, string y)
        {

        }
    }
}

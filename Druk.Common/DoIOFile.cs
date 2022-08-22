using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// IO文件操作
    /// </summary>
    public static class DoIOFile
    {
        #region //读取文件

        #region //读取全部文件

        /// <summary>
        /// 将指定路径中的文件内容读出来 以字符串形式返回
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>对象文件的内容</returns>
        public static string Read(string filePath, string encoding = "UTF-8")
        {
            encoding = string.IsNullOrEmpty(encoding) ? Config.DefaultEncoding : encoding;
            filePath = DoPath.GetFullPath(filePath);
            if (File.Exists(filePath))
            {
                //读取文本
                StreamReader sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding(encoding));
                string str = sr.ReadToEnd();
                sr.Close();
                return str;
            }
            else
                return "";
        }
        #endregion

        #region 分段读取文件(对于大文件读取适用)

        /// <summary>
        /// 分段读取文件
        /// 默认每次读取 1M 数据
        /// </summary>
        /// <param name="sourceFile">文件全路径</param>
        /// <param name="readStart">读取的开始位置</param>
        /// <param name="contentLeave">是否还有剩下部分</param>
        /// <param name="splitFileSize">每次读取的片段长度</param>
        /// <param name="separate">分隔符号的ASCII码值</param>
        /// <returns></returns>
        public static string Read_BigFile_1M(string sourceFile, ref long readStart, ref bool contentLeave, long splitFileSize = 1024 * 1024 * 1, char separate = '\n')
        {
            string resultContent = string.Empty;

            try
            {
                using (FileStream stream = new FileStream(sourceFile, FileMode.Open))
                {
                    long FileTotalLength = stream.Length;
                    if (readStart < FileTotalLength)
                    {
                        //创建二进制读取
                        using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                        {
                            //直接将开始读取的位置设定到基础大小的字节上
                            //下面要做的是往后找到这一行的结束
                            reader.BaseStream.Position = splitFileSize + readStart - 1;
                            //判断当前位置不超过文件总大小
                            if (reader.BaseStream.Position <= FileTotalLength)
                            {
                                //往后挨个儿字符找换行
                                //这里要说明的是  reader.ReadByte() 方法执行时会自动将 reader.BaseStream.Position 的值向后+1
                                //网上有些例子执行了 ReadByte 另外还做 Position++  明显是有字符隔掉的
                                while (reader.BaseStream.Position < FileTotalLength && reader.ReadByte() != separate) { }

                                //这里获得现在找到换行的那个字节上的位置到这次遍历开始的位置中间的字节数量
                                //+1 是为了把找到的那个换行符也带上
                                int readWrodCountNow = (int)(reader.BaseStream.Position - readStart);
                                //把读取的起始位置重置到这次查询的开始位置
                                reader.BaseStream.Position = readStart;
                                //把这次读取的内容写入到新文件
                                resultContent = reader.ReadBytes(readWrodCountNow).ToStr();
                            }
                            else
                            {
                                reader.BaseStream.Position = readStart;
                                resultContent = reader.ReadBytes((FileTotalLength - readStart + 1).ToInt()).ToStr();
                            }
                            //将这次读取到的位置作为下次的起始位置
                            readStart = reader.BaseStream.Position;
                            contentLeave = readStart < FileTotalLength;
                            //去除隐藏字符 byte值为65279  unicode的文件第一个字符会出现
                            resultContent = resultContent.Replace(((char)65279).ToString(), "");
                            return resultContent;
                        }
                    }
                    else
                    {
                        contentLeave = false;
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                contentLeave = false;
                return "";
            }
        }
        #endregion

        #region //读取最后一行
        /// <summary>
        ///
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="splitFileSize"></param>
        /// <param name="separate"></param>
        /// <returns></returns>
        public static string Read_LastRow(string sourceFile, long splitFileSize = 1024 * 1024 * 1, char separate = '\n')
        {
            //读取步骤的最后位置
            var historyFile = new System.IO.FileInfo(sourceFile);
            if (historyFile.Exists)
            {
                //1M  如果文件小于1M  就全部读取,否则读取最后1M
                var tempLength = 1024 * 1024;
                var length = historyFile.Length;
                var beginPosition = length < tempLength ? 0 : length - tempLength;
                var isLeave = false;
                var contentHistory = Druk.Common.DoIOFile.Read_BigFile_1M(historyFile.FullName, ref beginPosition, ref isLeave, tempLength * 10);
                if (!string.IsNullOrEmpty(contentHistory)) //得到内容
                {
                    var historyList = contentHistory.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (historyList.Count() > 0)
                    {
                        return historyList.LastOrDefault(); //取得最后一条
                    }
                    return string.Empty;
                }
                return null;
            }
            return string.Empty;
        }
        #endregion

        #endregion

        #region //写入文件


        #region //保存图片  流对象



        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">全路径（根目录+文件夹），磁盘目录</param>
        /// <param name="fileName"> 文件名</param>
        /// <param name="ms">数据流</param>
        /// <param name="uploadType">上传类型</param>
        /// <param name="uploadPath">访问的路劲（针对本地服务方式）</param>
        /// <returns>result=成功或失败 path=图片的访问路劲，不包含url</returns>
        public static (bool result, string path) SaveFile(string filePath, string fileName, Stream ms, ComEnum.UploadMode uploadType = ComEnum.UploadMode.本地文件服务器, string uploadPath = "")
        {
            var path = "";
            var result = false;
            if (uploadType == ComEnum.UploadMode.本地文件服务器)
            {
                if (ms != null)
                {
                    #region 本地文件服务器
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    FileInfo file = new FileInfo(DoPath.GetFullPath(filePath + fileName));
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    using (FileStream FileStream = new FileStream(file.FullName, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        byte[] bytes = new byte[ms.Length];
                        ms.Read(bytes, 0, (int)ms.Length);
                        FileStream.Write(bytes, 0, bytes.Length);
                        FileStream.Close();
                        ms.Close();
                    }
                    file.Refresh();
                    result = true;
                    path = $"{uploadPath}{fileName}";
                    #endregion
                }
            }
            else if (uploadType == ComEnum.UploadMode.阿里OSS)//浅蓝使用oss图档系统
            {
                DoOss doOss = new DoOss();
                result = doOss.PushFile(ms, filePath + fileName);
                path = $"{filePath}{fileName}";
            }
            return (result, path);

        }
        #endregion


        #region //将字符串写入文件

        /// <summary>
        /// 创建文件或者附加内容
        /// </summary>
        /// <param name="Data">要写入的文本</param>
        /// <param name="Encoding">编码方式</param>
        /// <param name="filePath">文件路径(支持相对路径或绝对路径)</param>
        /// <param name="IsAppend">附加还是全新写入</param>
        public static bool Write(string filePath, string Data, bool IsAppend = true, string Encode = "UTF-8")
        {
            Encode = string.IsNullOrEmpty(Encode) ? Config.DefaultEncoding : Encode;
            //文件的绝对路径 web程序和winform程序的获取方法是不一样的
            filePath = DoPath.GetFullPath(filePath);
            try
            {
                //获取文件对象
                FileInfo file = new FileInfo(filePath);
                //判断文件夹是否创建
                if (!file.Directory.Exists) { file.Directory.Create(); }
                //判断文件是否存在
                //或者判断文件属性是否只读,是只读的话将只读属性去掉
                if (!file.Exists) { using (file.Create()) { } }
                else if (file.Attributes.ToString().IndexOf("ReadOnly") != -1) { file.IsReadOnly = false; }
                //将内容覆盖或者追加到文件的最后
                using (StreamWriter writer = new StreamWriter(file.FullName, IsAppend, Encoding.GetEncoding(Encode)))
                {
                    writer.Write(Data);
                    writer.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }
        #endregion
        #region //将二进制字符写入文件

        /// <summary>
        /// 创建文件或者附加内容
        /// </summary>
        /// <param name="filePath">文件路径(支持相对路径或绝对路径)</param>
        /// <param name="data">要写入的Bype[]数组</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="IsAppend">是否续写文件,false的话会覆盖文件原内容</param>
        public static bool Write(string filePath, byte[] data, bool IsAppend = false, string Encode = "UTF-8")
        {
            Encode = string.IsNullOrEmpty(Encode) ? Config.DefaultEncoding : Encode;
            return Write(filePath, System.Text.Encoding.GetEncoding(Encode).GetString(data), IsAppend, Encode);
        }
        #endregion
        #region //将流对象保存至文件

        /// <summary>
        /// 将流对象保存至文件
        /// </summary>
        /// <param name="filePath">文件路径(支持相对路径或绝对路径)</param>
        /// <param name="data">要写入的Bype[]数组</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="IsAppend">是否续写文件,false的话会覆盖文件原内容</param>
        public static bool Write(string filePath, Stream stream, bool IsAppend = true, string Encode = "UTF-8")
        {
            if (stream != null)
            {
                try
                {
                    #region //从Stream中读取字符串
                    string Str = string.Empty;
                    int count = -1, offset = 0, StepLength = 10 * 1024 * 1024;
                    byte[] bytes = new byte[StepLength];
                    do
                    {
                        count = stream.Read(bytes, offset, StepLength); //从设置的起始点开始读取 10M 长度
                        Str += System.Text.Encoding.GetEncoding(Encode).GetString(bytes);
                        offset += count;  //更新读取起始点
                    } while (count != 0);
                    #endregion

                    return Write(filePath, Str, IsAppend, Encode);

                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region //保存网络图片
        /// <summary>
        /// 保存网络图片
        /// </summary>
        /// <param name="WebImageURL">网络图片绝对路径</param>
        /// <param name="LocalPath">本地路径(支持相对路径或绝对路径) </param>
        /// <returns></returns>
        public static bool SaveWebImage(string WebImageURL, string LocalPath)
        {
            try
            {
                LocalPath = DoPath.GetFullPath(LocalPath); //从当前路劲获取到绝对路径
                var File = new FileInfo(LocalPath);
                if (!File.Directory.Exists) { File.Directory.Create(); } //如果没有文件夹 自动创建
                WebRequest wreq = WebRequest.Create(WebImageURL);
                HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();
                using (Stream s = wresp.GetResponseStream())
                {
                    System.DrawingCore.Image img;
                    img = System.DrawingCore.Image.FromStream(s);
                    img.Save(LocalPath, System.DrawingCore.Imaging.ImageFormat.Gif);   //保存
                    img.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }
        #endregion
        #endregion

        #region //文件改名

        public static bool ReName(string filePath, string newName)
        {
            filePath = DoPath.GetFullPath(filePath);
            var file = new FileInfo(filePath);
            try
            {
                if (file.Exists)
                {
                    file.MoveTo(Path.Combine(new string[] { file.DirectoryName, newName + file.Extension }));
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        #endregion

        #region //创建文件

        public static bool Create(string filePath, bool IsReCreate = true)
        {
            filePath = DoPath.GetFullPath(filePath);

            var file = new FileInfo(filePath);
            try
            {
                if (!IsReCreate && file.Exists) { return true; }

                if (file.Exists)
                {
                    if (!IsReCreate) { return true; }//如果存在不重建且文件存在  则直接返回True
                    else
                    {
                        file.Delete(); //需要重建则删除文件
                    }
                }
                if (!file.Directory.Exists) { file.Directory.Create(); } //验证文件夹

                //创建文件
                using (var stream = file.Create()) { }
                file.Refresh();

                return file.Exists;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region //获取文件内容有多少行
        /// <summary>
        /// 获取文件内容有多少行
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static int Get_FileLineCount(string filePath)
        {
            int result = 0;
            filePath = DoPath.GetFullPath(filePath);

            long readStart = 0;
            bool contentLeave = true;
            while (contentLeave)
            {
                //每次1M文件
                string content = Druk.Common.DoIOFile.Read_BigFile_1M(filePath, ref readStart, ref contentLeave, 1024 * 1024 * 1);
                result += content.Length > 0 ? content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length : 0;
            }


            return result;
        }
        #endregion

        #region //扩展方法
        /// <summary>
        /// 获取文件对象的文件名, 不带后缀名
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string FileInfoName(this FileInfo file)
        {
            return file.Name.Remove(file.Name.Length - file.Extension.Length);
        }
        #endregion

        #region //工具方法

        /// <summary>
        /// 获得字符流
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static MemoryStream GetMemoryStream(byte[] data)
        {
            return new MemoryStream(data);
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ExistsFile(string filePath)
        {
            filePath = DoPath.GetFullPath(filePath);
            return System.IO.File.Exists(filePath);
        }

        /// <summary>
        /// 验证文件是否可以被操作,,验证是否存在,是否被占用
        /// </summary>
        /// <returns></returns>
        public static bool FileCanOperate(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists) return false;
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open);
                return true;
            }
            catch { return false; }
            finally { if (fs != null) { fs.Close(); fs.Dispose(); } }
        }


        #endregion

        #region //文件大小

        public static string GetFileSizeStr(long size)
        {
            long uu = 1024;
            var converDouble = size.ToDouble();
            if (converDouble < uu) return converDouble + "B";
            if (converDouble < uu * uu) return (converDouble / uu).ToString("F2") + "KB";
            if (converDouble < uu * uu * uu) return (converDouble / uu / uu).ToString("F2") + "M";
            else return (converDouble / uu / uu / uu).ToString("F2") + "G";
        }
        #endregion

        #region //删除文件
        /// <summary>
        /// 删除文件, 并同时删除空文件夹
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="Del_Empty_Directory"></param>
        /// <returns></returns>
        public static bool Delete(string filePath, bool Del_Empty_Directory = false)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath.Trim())) { return false; }

                FileInfo File = new FileInfo(filePath);
                if (File.Exists)
                {
                    //Logger.Info("删除文件: " + File.FullName);
                    File.Delete();
                }

                if (Del_Empty_Directory && File.Directory.Exists)
                {
                    if (File.Directory.GetDirectories().Length + File.Directory.GetFiles().Length == 0)
                    {
                        //Logger.Info("删除文件夹: " + File.Directory.FullName);
                        File.Directory.Delete();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //Logger.Error("删除文件时异常!! [" + (filePath + "," + Del_Empty_Directory.ToString()) + "]");
                //Logger.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="DirPath">目标文件夹</param>
        /// <param name="IsDeleteDir">是否删除本文件夹</param>
        /// <returns></returns>
        public static bool Delete_ClearDir(string DirPath, bool IsDelete_Self_Dir = true)
        {
            try
            {
                if (!Directory.Exists(DirPath)) { return true; }
                Array.ForEach(Directory.GetFiles(DirPath), file => { File.Delete(file); });
                Array.ForEach(Directory.GetDirectories(DirPath), dir => { Delete_ClearDir(dir, true); });
                if (IsDelete_Self_Dir) { Directory.Delete(DirPath); }
                return true;

            }
            catch (Exception ex)
            {
                //Logger.Error("删除文件夹时失败!! [" + DirPath + "," + IsDelete_Self_Dir + "]");
                //Logger.Error(ex);
                return false;
            }
        }
        #endregion

        #region //文件移动
        /// <summary>
        /// 文件移动
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="TargetPath"></param>
        /// <returns></returns>
        public static bool Move(string SourcePath, string TargetPath, bool IsOverWrite = true)
        {
            var file = new FileInfo(SourcePath);
            if (file.Exists)
            {
                try
                {
                    var fileTarget = new FileInfo(TargetPath);
                    if (fileTarget.Exists) //检查目标文件是否存在
                    {
                        if (IsOverWrite) { fileTarget.Delete(); } else { return false; }  //如果要覆盖的话.就删除,如果不覆盖.则直接返回false
                    }
                    if (!fileTarget.Directory.Exists) { fileTarget.Directory.Create(); } //检查并创建目标文件的文件夹
                    file.CopyTo(fileTarget.FullName, IsOverWrite); //文件移动
                    fileTarget.Refresh(); //刷新目标文件对象 如果目标文件不存在,或者与原文件大小不一致,则删除文件返回false
                    if (!fileTarget.Exists || fileTarget.Length != file.Length) { Delete(fileTarget.FullName, true); return false; }
                    file.Refresh(); //刷新原文件对象,如果文件存在就删除
                    if (file.Exists) { Delete(file.FullName, true); }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return false;
        }
        #endregion

        #region //获取文件夹下所有的文件路径 包括子文件夹中的文件
        /// <summary>
        /// 获取文件夹下所有的文件路径 包括子文件夹中的文件
        /// </summary>
        /// <param name="DirectoryPath"></param>
        /// <param name="IsScanChild"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string DirectoryPath, bool IsScanChild = true)
        {
            var result = new List<string>();
            var dir = new DirectoryInfo(DirectoryPath);
            if (dir.Exists)
            {
                var fileList = dir.GetFiles();
                result.AddRange(fileList.Select(o => o.FullName));

                #region //获取子文件夹 并递归
                if (IsScanChild)
                {
                    var dirList = dir.GetDirectories();
                    foreach (var itemDir in dirList)
                    {
                        result.AddRange(GetFiles(itemDir.FullName, IsScanChild));
                    }
                }
                #endregion
                return result;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Stream FileToStream(string fileName)
        {
            // 打开文件   
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[]   
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream   
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

    }
}

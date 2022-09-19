using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Druk.Handle
{
    /// <summary>
    /// 配置信息(用来读取相应配置文件)，客户端配置类
    /// </summary>
    public class ConfigInfo<T> where T : class, new()
    {
        /// <summary>
        /// 配置地址
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 当前配置
        /// </summary>
        public T CurrentConfig { get; set; }
        /// <summary>
        /// 读写锁
        /// </summary>
        private ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigInfo(string FileName)
        {
            this.FileName = FileName;
            this.FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
            CurrentConfig = Read();
        }
        /// <summary>
        /// 读取配置信息
        /// </summary>
        /// <returns></returns>
        public T Read()
        {
            T t = new T();
            if (File.Exists(FilePath))
            {
                try
                {
                    rwl.EnterReadLock();
                    var Str = File.ReadAllText(FilePath);
                    if (!string.IsNullOrEmpty(Str))
                    {
                        try
                        {
                            t = JsonConvert.DeserializeObject<T>(Str);
                            CurrentConfig = t;
                            return t;
                        }
                        catch (Exception)
                        { }
                    }
                }
                finally
                {
                    rwl.ExitReadLock();
                }
            }
            else
            {
                Save(t);
            }
            return t;
        }
        /// <summary>
        /// 写配置
        /// </summary>
        /// <param name="t"></param>
        public void Save(T t)
        {
            try
            {
                rwl.EnterWriteLock();
                if (!File.Exists(FilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(new FileInfo(FilePath).DirectoryName);
                        CurrentConfig = t;
                    }
                    catch (Exception)
                    { }
                }
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(t));
            }
            catch (Exception)
            {
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }
        /// <summary>
        /// 写配置
        /// </summary>
        /// <param name="t"></param>
        public void Save()
        {
            try
            {
                rwl.EnterWriteLock();
                if (!File.Exists(FilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(new FileInfo(FilePath).DirectoryName);
                    }
                    catch (Exception)
                    { }
                }
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(CurrentConfig));
            }
            catch (Exception)
            {
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// 客户端配置
    /// </summary>
    public class ClientConfig
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public string Server { get; set; }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// 文件路径操作
    /// </summary>
    public static class DoPath
    {
        #region //获取文件绝对路径
        //目录分隔符，因为是跨平台的应用，我们要判断目录分隔符，windows 下是 "\"， Mac OS and Linux 下是 "/"
        private static string DirectorySeparatorChar = Path.DirectorySeparatorChar.ToString();
        //包含应用程序的目录的绝对路径
        private static string _ContentRootPath = DI.ServiceProvider.GetRequiredService<IHostingEnvironment>().ContentRootPath;

        /// <summary>
        /// 获取文件绝对路径
        /// </summary>
        public static string GetFullPath(string path)
        {
            path = path ?? "";
            //return IsAbsolute(path) ? path : Path.Combine(_ContentRootPath, path.TrimStart('~', '/').Replace("/", DirectorySeparatorChar));
            return path;

            //try
            //{
            //    return IsAbsolute(path) ? path : Path.Combine(_ContentRootPath, path.TrimStart('~', '/').Replace("/", DirectorySeparatorChar));
            //}
            //catch { }
            //return "";

        }
        #endregion

        #region //是否是绝对路径
        /// <summary>
        /// 是否是绝对路径
        /// </summary>
        public static bool IsAbsolute(string path)
        {
            return Path.VolumeSeparatorChar == ':'
                ? path.Contains(Path.VolumeSeparatorChar.ToString())  // windows系统    路径是否包含 ":"
                : path.StartsWith('\\');   // Mac OS、Linux下判断 路径是否以 "\" 开头
        }
        #endregion

        #region //合并拼接多个路径
        /// <summary>
        /// 合并多个路径为同一路径,并处理中间的间隔符号
        /// </summary>
        /// <param name="path">多地址多参数的合集</param>
        /// <returns></returns>
        public static string Combine(params string[] path)
        {
            return string.Join("/", string.Join("/", path).Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries));
        }
        #endregion
    }
}

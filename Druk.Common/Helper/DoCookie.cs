using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common
{
    public class DoCookie
    {
        #region //写Cookie
        /// <summary>
        /// 写入Cookie
        /// </summary>
        /// <param name="CookieName"></param>
        /// <param name="Value"></param>
        public static void WriteCookie(string CookieName, string Value)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies != null)
                {
                    HttpContext.Current.Response.Cookies.Append(CookieName, DoWebRequest.UrlEncode(Value));
                }
            }
            catch (Exception)
            {
            }
            //  WriteCookie(CookieName, Value, null);
        }

        /// <summary>
        /// 写入Cookie 和失效时间
        /// </summary>
        /// <param name="CookieName"></param>
        /// <param name="Value"></param>
        /// <param name="Expires">失效时间</param>
        public static void WriteCookie(string CookieName, string Value, DateTime Expires)
        {
            WriteCookie(CookieName, Value, new CookieOptions() { Expires = Expires });
        }



        /// <summary>
        /// 写入Cookie并支持深入配置
        /// </summary>
        /// <param name="CookieName"></param>
        /// <param name="Value"></param>
        /// <param name="Options"></param>
        public static void WriteCookie(string CookieName, string Value, CookieOptions Options)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies != null)
                {
                    var cookie = HttpContext.Current.Request.Cookies[CookieName];
                    HttpContext.Current.Response.Cookies.Append(CookieName, DoWebRequest.UrlEncode(Value), Options);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region //读Cookie
        /// <summary>
        /// 读取Cookie内容 没有返回Null
        /// </summary>
        /// <param name="CookieName"></param>
        /// <returns></returns>
        public static string GetCookie(string CookieName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                return DoWebRequest.UrlDecode(HttpContext.Current.Request.Cookies[CookieName]);
            }
            return null;
        }
        #endregion

        #region //删Cookie
        /// <summary>
        /// 删除Cookie
        /// </summary>
        /// <param name="CookieName"></param>
        public static void DeleteCookie(string CookieName)
        {
            if (HttpContext.Current.Response.Cookies != null)
            {
                HttpContext.Current.Response.Cookies.Delete(CookieName);
            }
        }
        #endregion
    }
}

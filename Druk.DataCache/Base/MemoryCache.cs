using Druk.Common;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.DataCache.Base
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    public static class MemCache
    {
        //内存实例
        static readonly MemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());

        #region //获取缓存内容

        /// <summary>
        /// 获取当前内存缓存内容
        /// </summary>
        /// <param name="keyName">存储key</param>
        /// <returns></returns>
        public static dynamic GetCacheValue(string keyName)
        {
            try
            {
                //获取存储值
                var value = MemoryCache.Get(keyName);
                return value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当前内存缓存内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">存储key</param>
        /// <returns></returns>
        public static T GetCacheValue<T>(string keyName) where T : class
        {
            try
            {
                //获取内存存储值
                var cacheValue = MemoryCache.Get(keyName);
                //序列化得到的存储值
                if (cacheValue != null)
                {
                    var value = cacheValue.ToString().ToObjectFromJson<T>();
                    return value;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region //存储值到缓存
        /// <summary>
        /// 存储值内容到内存缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">存储key</param>
        /// <param name="value">值内容</param>
        /// <param name="seconds">单位秒</param>
        /// <returns></returns>
        public static bool SetCacheValue<T>(string keyName, T value, int seconds) where T : class
        {
            try
            {
                //存储值到内存缓存
                MemoryCache.Set(keyName, value.ToJson(), DateTime.Now.AddSeconds(seconds));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 存储值内容到内存缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">存储key</param>
        /// <param name="value">值内容</param>
        /// <param name="seconds">单位秒</param>
        /// <returns></returns>
        public static bool SetCacheValue(string keyName, string value, int seconds)
        {
            try
            {
                //存储值到内存缓存
                MemoryCache.Set(keyName, value, DateTime.Now.AddSeconds(seconds));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}

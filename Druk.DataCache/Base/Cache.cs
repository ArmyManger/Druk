using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Druk.DataCache.Base
{
    /// <summary>
    /// 缓存管理公用类
    /// </summary>
    public static class Cache
    {    

        #region //获取
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey">缓存名称</param>
        /// <param name="dbNo">缓存库号</param>
        /// <returns></returns>
        public static object Get(string cacheKey, int dbNo)
        {
            switch (new Config().CacheModule.ToLower())
            {
                case "enyim": return MemberCached.Get(cacheKey);
                case "redis":
                default: return Redis.Get(cacheKey, dbNo);
            }
        }
        #endregion

        #region //插入

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="objObject">缓存数据</param>
        /// <param name="dbNo">缓存库号</param>
        public static void Insert(string cacheKey, object objObject, int dbNo, int Second = 60 * 60 * 12)
        {
            switch (new Config().CacheModule.ToLower())
            {
                case "enyim": MemberCached.Set(cacheKey, objObject, Second); break;
                case "redis":
                default: Redis.Set(cacheKey, objObject, Second, dbNo); break;
            }
        }
        #endregion

        #region //是否存在

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="dbNo">缓存库号</param>
        /// <returns></returns>
        public static bool Exists(string cacheKey, int dbNo)
        {
            switch (new Config().CacheModule.ToLower())
            {
                case "enyim": return MemberCached.Exists(cacheKey);
                case "redis":
                default: return Redis.Exists(cacheKey, dbNo);
            }
        }
        #endregion

        #region //移除

        /// <summary>
        /// 移除缓存项
        /// </summary>
        /// <param name="cacheKey">缓存key</param>
        /// <param name="dbNo">缓存库号</param>
        /// <returns></returns>
        public static object Remove(string cacheKey, int dbNo)
        {
            switch (new Config().CacheModule.ToLower())
            {
                case "enyim": return MemberCached.Remove(cacheKey);
                case "redis":
                default: return Redis.Remove(cacheKey, dbNo);
            }
        }
        #endregion

        #region //遍历缓存项
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="Search">缓存key</param>
        /// <param name="dbNo">库号</param>
        /// <returns></returns>

        public static IEnumerable<string> ScanKeys(string Search, int dbNo)
        {
            var result = new List<string>();
            switch (new Config().CacheModule.ToLower())
            {
                case "enyim": result = MemberCached.ScanKeys(Search); break;
                case "redis":
                default: result = Redis.ScanKeys(Search, dbNo); break;
            }
            return (result ?? new List<string>()).Select(o => o);

        }
        #endregion
    }
}

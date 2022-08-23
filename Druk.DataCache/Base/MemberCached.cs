using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.DataCache.Base
{
    public class MemberCached
    {
        #region //获取客户端
        /// <summary>
        /// 获取客户端连接
        /// </summary>
        /// <returns></returns>
        public static MemcachedClient GetClient()
        {
            MemcachedClientConfiguration config = new MemcachedClientConfiguration();//创建配置参数

            var ip = "";
            var Port = 123;

            config.Servers.Add(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), Port));//增加服务节点  可以增加多个


            config.Protocol = MemcachedProtocol.Text;
            config.Authentication.Type = typeof(PlainTextAuthenticator).Name;//设置验证模式
            config.Authentication.Parameters["userName"] = "uid";//用户名参数
            config.Authentication.Parameters["password"] = "pwd";//密码参数
            MemcachedClient mac = new MemcachedClient(null, config);//创建客户端
            return mac;
        }
        #endregion

        #region //写入
        /// <summary>
        /// 写入值信息
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="value">缓存内容</param>
        /// <param name="second">默认超时时间是12小时</param>
        /// <returns></returns>
        public static bool Set(string key, object value, int second)
        {
            try
            {
                using (var client = GetClient())
                {
                    client.Store(StoreMode.Set, key, value, new TimeSpan(0, 0, second));
                }
            }
            catch (Exception ex)
            {
                Druk.Log.Error(ex);
                return false;
            }
            return true;
        }
        #endregion

        #region //读取
        /// <summary>
        /// 读取缓存信息
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            try
            {
                using (var client = GetClient())
                {
                    return client.Get(key);
                }
            }
            catch (Exception ex)
            {
                Druk.Log.Error(ex);
            }
            return null;
        }
        #endregion

        #region //删除
        /// <summary>
        /// 根据缓存key删除缓存值
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            try
            {
                using (var client = GetClient())
                {
                    return client.Remove(key);
                }
            }
            catch (Exception ex)
            {
                Druk.Log.Error(ex);
                return false;
            }
        }
        #endregion

        #region //清空
        /// <summary>
        /// 清空所有缓存内容
        /// </summary>
        /// <returns></returns>
        public static void FlushAll()
        {
            try
            {
                using (var client = GetClient())
                {
                    client.FlushAll();
                }
            }
            catch (Exception ex)
            {
                Druk.Log.Error(ex);
            }
        }
        #endregion

        #region //是否存在
        /// <summary>
        /// 验证Key是否存在
        /// </summary>
        /// <param name="cacheKey">缓存key</param>
        /// <returns></returns>
        internal static bool Exists(string cacheKey)
        {
            try
            {
                using (var client = GetClient())
                {
                    return client.Exists(cacheKey);
                }
            }
            catch (Exception ex)
            {
                Druk.Log.Error(ex);
                return false;
            }
        }

        #endregion

        #region //遍历Keys
        /// <summary>
        /// 根据缓存key查询存储值
        /// </summary>
        /// <param name="Search">缓存key</param>
        /// <returns></returns>
        public static List<string> ScanKeys(string Search = "")
        {
            throw new Exception();  //暂时用不到.. 后期使用时再完善
        }
        #endregion
    }
}

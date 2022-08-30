using Druk.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Druk.DataCache.Base
{
    /// <summary>
    /// Redis
    /// </summary>
    public static class Redis
    {

        #region //获取客户端
        /// <summary>
        /// Redis连接态
        /// </summary>
        static ConnectionMultiplexer Connection = null;
        /// <summary>
        /// Redis 缓存库信息 
        /// </summary>
        static Dictionary<string, IDatabase> Dic_DB = new Dictionary<string, IDatabase>();


        /// <summary>
        /// 初始化缓存库连接
        /// </summary>
        /// <param name="dbNo"></param>
        /// <returns></returns>
        internal static IDatabase InitClient(int dbNo)
        {
            try
            {
                var Config = new Config();

                #region //监测连接状态,若无效则重新连接,并清理现有库连接对象
                if (Connection == null || !Connection.IsConnected)
                {
                    string connStr = Config.Redis_ServerPath; // 读取redis连接 
                    Console.WriteLine("Redis-Conn-Str: " + connStr); 
                    Connection = ConnectionMultiplexer.Connect(connStr);
                    Dic_DB.Clear(); //清除现有的库连接
                }
                #endregion

                //能到这里应该连接是有效的
                var ConnName = "DB_" + dbNo;
                if (!Dic_DB.ContainsKey(ConnName)) { Dic_DB[ConnName] = Connection.GetDatabase(dbNo); }

                return Dic_DB[ConnName];
            }
            catch (Exception ex)
            {
                Connection = null;
                Dic_DB.Clear();
                Log.Error(ex.ToSimple("Redis开启连接报错!!"));
                return null;
            }
        }

        #endregion

        #region //读取
        /// <summary>
        /// 读取缓存值信息
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="dbNo">缓存库号</param>
        /// <returns></returns>
        public static object Get(string key, int dbNo)
        {
            try
            {
                var result = InitClient(dbNo).StringGet(key);
                //序列化二进制对象信息返回
                return Druk.Common.DoJson.SerializableGet(result);
            }
            catch (RedisTimeoutException ex)
            {
                Log.Error(ex.ToSimple("redis.Get方法异常 - " + key));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToSimple("redis.Get方法异常 - " + key));
            }
            return null;
        }
        #endregion

        #region //写入
        /// <summary>
        /// 写入缓存信息
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="value">缓存值内容</param>
        /// <param name="second">缓存时间 单位秒</param>
        /// <param name="dbNo">缓存库号</param>
        public static void Set(string key, Object value, int second, int dbNo)
        {
            try
            {
                var vv = Druk.Common.DoJson.SerializableSet(value);
                InitClient(dbNo).StringSet(key, vv, new TimeSpan(0, 0, second));
            }
            catch (RedisTimeoutException)
            {

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToSimple("redis.Set方法异常 - " + key));
            }
        }
        #endregion

        #region //删除
        /// <summary>
        /// 删除缓存信息
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="dbNo">缓存库号</param>
        /// <returns></returns>
        public static bool Remove(string key, int dbNo)
        {
            try
            {
                return InitClient(dbNo).KeyDelete(key);
            }
            catch (RedisTimeoutException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToSimple("redis.Remove方法异常 - " + key));
                return false;
            }
        }
        #endregion

        #region //是否存在

        /// <summary>
        /// 检查Key是否存在
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="dbNo">缓存库号</param>
        /// <returns></returns>
        internal static bool Exists(string key, int dbNo)
        {
            try
            {
                return InitClient(dbNo).KeyExists(key);
            }
            catch (RedisTimeoutException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToSimple("redis.Exists方法异常 - " + key));
                return false;
            }
        }

        #endregion


        #region //执行Redis命令并返回结果

        /// <summary>
        /// 执行Redis命令并返回结果
        /// </summary>
        /// <param name="command">命令中的第一节</param>
        /// <param name="dbNo">缓存库号</param>
        /// <param name="args">后续参数</param>
        /// <returns>返回执行结果 dynamic类型 需要强制转换 </returns>
        public static RedisResult Execute(string command, int dbNo, params object[] args)
        {
            try
            {
                var result = InitClient(dbNo).Execute(command, args);
                return result.IsNull ? null : result;
            }
            catch (RedisTimeoutException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToSimple("redis.Execute方法异常 - " + command + " " + string.Join(" ", args)));
                return null;
            }
        }
        #endregion

        #region //根据Key名条件来搜索所有的Key名称

        /// <summary>
        /// 根据Key名条件来搜索所有的Key名称
        /// </summary>
        /// <param name="search">Key名搜索条件</param>
        /// <param name="dbNo">缓存库号</param>
        /// <returns></returns>
        public static List<string> ScanKeys(string search, int dbNo)
        {
            search = string.IsNullOrEmpty(search) ? "*" : "*" + search + "*";
            var result = Execute("keys", dbNo, search);
            if (result != null)
            {
                var keys = (RedisKey[])result;
                return keys.Select(o => o.ToString()).ToList();
            }
            return new List<string>();
        }
        #endregion



    }
}

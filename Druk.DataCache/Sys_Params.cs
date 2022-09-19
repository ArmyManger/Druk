using Druk.Common;
using Druk.DataCache.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Druk.DataCache
{
    /// <summary>
    /// Sys_Params=系统参数配置表数据缓存类
    /// </summary>
    public class Sys_Params
    {
        //当前缓存对象配置信息
        private const string CacheName = "Sys_Params.";
        private static int CacheNo = new Base.Config().RedisNoSysParams;
        private const int DB_Second = 43200;

        #region //InitCache
        /// <summary>
        /// 初始化缓存
        /// </summary>
        /// <param name="name">缓存名称</param>
        /// <param name="value">缓存值</param>
        /// <returns></returns>
        public static string InitCache(string name = "", string value = "")
        {
            #region //没有特定值，更新全部缓存到redis

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(value))
            {
                //var dicParam = (Druk.DB.Config.Conn_Wathet.ExecuteDT("SELECT [pName],[pValue] FROM [t_Sys_Params]") ?? new DataTable()).ToDictionary("pName", "pValue", true);
                var dicParam = new DataTable().ToDictionary("pName", "pValue", true);
                if (dicParam != null && dicParam.Count > 0)
                {

                    #region //处理Cache-Redis参数

                    var cache = dicParam.Where(o => o.Key.StartsWith("Cache.", System.StringComparison.OrdinalIgnoreCase)).ToList();
                    if (cache != null && cache.Count > 0)
                    {
                        var co = new Dictionary<string, string>();
                        cache.ForEach(o =>
                        {
                            co[o.Key.ToLower()] = o.Value;
                        });
                        Druk.DataCache.Base.Config.CacheConfig = co;
                    }

                    #endregion

                    CacheNo = new Base.Config().RedisNoSysParams;

                    foreach (var key in dicParam.Keys)
                    {
                        Cache.Insert(CacheName + key.ToLower(), dicParam[key], CacheNo, DB_Second);
                    }
                }
                return string.Empty;
            }
            #endregion

            #region //给了特定更新值

            if (!string.IsNullOrEmpty(name))
            {
                if (string.IsNullOrEmpty(value)) //没给value，表示redis查不出这个值,就从数据库中找出更新redis，并返回
                {
                    //value = (Druk.DB.Config.Conn_Wathet.ExecuteFirstCell("SELECT TOP 1 [pValue] FROM [t_Sys_Params] WHERE [pName]=@pName", CommandType.Text, new SqlParameter[] { new SqlParameter("@pName", name) }) ?? "").ToString();
                }
                else  //给了Name 和Value， 表示外边这个值发生改变了，就更新数据库和Redis
                {
                    //Druk.DB.Config.Conn_Wathet.ExecuteRowCount("UPDATE [t_Sys_Params] SET [pValue]=@pValue WHERE [pName]=@pName", CommandType.Text, new SqlParameter[] { new SqlParameter("@pName", name), new SqlParameter("@pValue", value) });
                }
                Cache.Insert(CacheName + name.ToLower(), value, CacheNo, DB_Second);
                return value;
            }
            #endregion

            return null;
        }
        #endregion

        #region //根据配置名称获取配置值
        /// <summary>
        /// 根据配置名称获取配置值
        /// </summary>
        /// <param name="name">缓存值名称</param>
        /// <returns></returns>
        public static string GetValue(string name)
        {
            if (string.IsNullOrEmpty(name)) { return null; }
            var entity = Cache.Get(CacheName + name.ToLower(), CacheNo);
            if (entity == null)
            {
                entity = InitCache(name);
            }
            return entity.ToString();
        }
        #endregion
    }
}

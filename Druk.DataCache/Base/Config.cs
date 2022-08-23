using Druk.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.DataCache.Base
{
    /// <summary>
    /// 配置
    /// </summary>
    public  class Config
    {

        #region //获取系统配置--Redis
        /// <summary>
        /// 缓存值参数
        /// </summary>
        static Dictionary<string, string> CacheConfig_pri;
        /// <summary>
        /// 系统配置项
        /// </summary>
        public static Dictionary<string, string> CacheConfig
        {
            get { return CacheConfig_pri; }
            set { CacheConfig_pri = value; }
        }
        #endregion

        #region //Redis设置

        /// <summary>
        /// 缓存设置模式
        /// </summary>
        public string CacheModule = ("Redis").ToLower();
        /// <summary>
        /// Redis缓存地址
        /// </summary>
        public string Redis_ServerPath = CacheConfig.GetValue("Cache.Redis.ServerPath") ?? "127.0.0.1";
        ///// <summary>
        ///// Redis缓存端口
        ///// </summary>
        //public int Redis_ServerPort = (CacheConfig.GetValue("Cache.Redis.ServerPort") ?? "6379").ToInt();
        ///// <summary>
        ///// Redis缓存登录密码
        ///// </summary>
        //public string Redis_ServerPwd = CacheConfig.GetValue("Cache.Redis.ServerPwd") ?? "noPassword";

        #endregion


        #region //Redis 缓存库号

        /// <summary>
        /// 系统基础信息
        /// </summary>
        public int RedisNoBasic = (CacheConfig.GetValue("Cache.Redis.DBNumber.Basic") ?? "0").ToInt();
        /// <summary>
        /// token缓存信息
        /// </summary>
        public int RedisNoToken = (CacheConfig.GetValue("Cache.Redis.DBNumber.Token") ?? "0").ToInt();
        /// <summary>
        /// 系统参数缓存信息
        /// </summary>
        public int RedisNoSysParams = (CacheConfig.GetValue("Cache.Redis.DBNumber.SysParams") ?? "0").ToInt();
        /// <summary>
        /// 当日交易汇总信息
        /// </summary>
        public int RedisNoTransSummaryByDay = (CacheConfig.GetValue("Cache.Redis.DBNumber.TransSummaryByDay") ?? "0").ToInt();
        /// <summary>
        /// 用户oken相关信息
        /// </summary>
        public int RedisNoUser = (CacheConfig.GetValue("Cache.Redis.DBNumber.User") ?? "0").ToInt();

        #endregion
    }
}

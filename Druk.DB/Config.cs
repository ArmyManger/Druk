using Druk.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Druk.DB
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    public class Config
    {
        #region 配置 
        /// <summary>
        /// 数据库日志清理标准 单位:M
        /// </summary>
        public static int SqlClearLogSize = 1024 * 5;

        /// <summary>
        ///加密值。生成器： https://suijimimashengcheng.51240.com/
        /// </summary>
        public static string key = "M1q!7WKkbW#Uz!tpdOdN3k*rdaAK2K0L";    //32位
        public static string iv = "Tg#2qhm6QE69LspB";   //16位
        #endregion


        /// <summary>
        /// 系统基础数据库
        /// </summary>
        public static string Conn_Wathet
        {
            get
            {
                if (Setting_SqlConn.ContainsKey("SQL.Wathet".ToLower()))
                {
                    return GetSqlConn("SQL.Wathet");
                }

                try
                {
                    //读配置文件中的数据库连接
                    var connStr = Druk.Common.ConfigJson.AppSettings["DBConnStr:SQLConnection:Druk"] ?? "";
                    //解密数据库连接字符串
                    connStr = Common.DoEncrypt.AesDecrypt(connStr, key, iv);
                    return connStr;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }


        #region //工具 静态变量获取数据库连接字符串
        /// <summary>
        /// 静态变量获取数据库连接字符串
        /// </summary>
        public static Dictionary<string, string> Setting_SqlConn = new Dictionary<string, string>();
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="sqlConnName"></param>
        /// <returns></returns>
        static string GetSqlConn(string sqlConnName)
        {
            if (Setting_SqlConn == null || Setting_SqlConn.Count == 0)
            {
                //读取配置表中的数据库配置信息 
                var list = Conn_Wathet.GetList<Entity.Sys_Params>("PNAME,PVALUE", "PNAME LIKE 'SQL.%'");

                #region //解密数据库连接 
                Setting_SqlConn = new Dictionary<string, string>();
                foreach (var item in list)
                {
                    //解密数据连接字符串s
                    Setting_SqlConn.Add(item.pName, Druk.Common.DoEncrypt.AesDecrypt(item.pValue, key, iv));
                }
                #endregion
            }
            //获取特定的数据库连接字符串
            if (!Setting_SqlConn.ContainsKey(sqlConnName.ToLower())) return string.Empty;
            var result = Setting_SqlConn[sqlConnName.ToLower()];
            return result;
        }
        #endregion

        #region 检查只读
        /// <summary>
        /// 检查是否是只读连接并且标记为只读的， 休眠一秒
        /// </summary>
        /// <param name="connectionString"></param>
        public static void CheckIsOnlyReadDB(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return;
            try
            {
                var http = Druk.Common.HttpContext.Current;  //httpContent对象httpContent对象  
                if (http != null && connectionString.Contains("ReadOnly"))
                {
                    var isOnlyReadDBObj = http.Items["isOnlyReadDB"];
                    if (isOnlyReadDBObj != null)
                    {
                        var isOnlyReadDB = isOnlyReadDBObj.ToBool(false);
                        if (isOnlyReadDB)
                        {
                            //List action用的是只读库，休眠一秒后在取数据
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}

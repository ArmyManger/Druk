using Druk.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.WeChat.Util
{
    public class WeChatTools
    {
        #region //公众号

        /// <summary>
        /// 公众号AppId
        /// </summary>
        public string AppId { get { return DataCache.Sys_Params.GetValue("WX:AppId"); } }
        /// <summary>
        /// 公众号AppSecret
        /// </summary>
        public string AppSecret { get { return DataCache.Sys_Params.GetValue("WX:AppSecret"); } }
        /// <summary>
        /// 公众号EncodingAESKey
        /// </summary>
        public string EncodingAESKey { get { return DataCache.Sys_Params.GetValue("WX:EncodingAESKey"); } }
        /// <summary>
        /// 公众号Token
        /// </summary>
        public string Token { get { return DataCache.Sys_Params.GetValue("WX:Token"); } }
        #endregion

        #region //小程序 
        /// <summary>
        /// 公众号AppId
        /// </summary>
        public string MiniApp_AppId { get { return DataCache.Sys_Params.GetValue("MiniApp_AppId"); } }

        /// <summary>
        /// 公众号AppSecret
        /// </summary>
        public string MiniApp_AppSecret { get { return DataCache.Sys_Params.GetValue("MiniApp_AppSecret"); } }


        #endregion

        #region //第三方平台

        /// <summary>
        /// 第三方平台AppID
        /// </summary>
        public string Component_AppId { get { return DataCache.Sys_Params.GetValue("WX:Component_AppId"); } }
        /// <summary>
        /// 第三方平台AppSecret
        /// </summary>
        public string Component_AppSecret { get { return DataCache.Sys_Params.GetValue("WX:Component_AppSecret"); } }
        /// <summary>
        /// 第三方平台EncodingAESKey
        /// </summary>
        public string Component_EncodingAESKey { get { return DataCache.Sys_Params.GetValue("WX:Component_EncodingAESKey"); } }
        /// <summary>
        /// 第三方平台Token
        /// </summary>
        public string Component_Token { get { return DataCache.Sys_Params.GetValue("WX:Component_Token"); } }


        #endregion


        #region //微信授权过程中间参数
        /// <summary>
        /// 根据请求获取第三方票证  component_verify_ticket
        /// </summary>
        public string Component_Verify_Ticket
        {
            set { Druk.DataCache.Base.Cache.Insert("WX.Config.component_verify_ticket", value, 14 * 60); }
            get { return (DataCache.Sys_Params.GetValue("WX.Config.component_verify_ticket") ?? string.Empty).ToString(); }
        }
        /// <summary>
        /// 获取第三方平台component_access_token
        /// </summary> DBConfigurationManager.InitializeCache
        public string Component_Access_Token
        {
            set { Druk.DataCache.Base.Cache.Insert("WX.Config.component_access_token", value, 100 * 60); }
            get { return (DataCache.Sys_Params.GetValue("WX.Config.component_access_token") ?? string.Empty).ToString(); }
        }
        /// <summary>
        /// 由公众号给第三方平台授权的链接中,回调的第三方链接,会带上授权码Auth_Code
        /// </summary>
        public string Component_AuthCodeUrl_CallBack
        {
            get { return DataCache.Sys_Params.GetValue("WX:Component_AuthCodeUrl_CallBack"); }
        }
        /// <summary>
        /// 公众号授权第三方后,回调的第三方链接中带的Auth_Code
        /// </summary>
        public string Component_Auth_Code
        {
            set { UpdateSysParam("WX.Config.Auth_Code", value); }
            get { return DataCache.Sys_Params.GetValue("WX.Config.Auth_Code"); }
        }


        #endregion


        #region //更新配置
        static bool UpdateSysParam(string PKey, string PValue)
        {
            var sql = @"UPDATE [t_SysParams] SET PVALUE=@PVALUE,UPDATETIME=getdate() WHERE PKEY=@PKEY";
            var sqlParams = new List<System.Data.SqlClient.SqlParameter>();
            sqlParams.Add(new System.Data.SqlClient.SqlParameter("@PKEY", PKey));
            sqlParams.Add(new System.Data.SqlClient.SqlParameter("@PVALUE", PValue));

            var result = Druk.DB.Config.SqlConn_Druk.ExecuteRowCount(sql, System.Data.CommandType.Text, sqlParams.ToArray());   
            if (result == 1)
            { 
                return true;
            }
            return false;
        }

        #endregion
    }
}

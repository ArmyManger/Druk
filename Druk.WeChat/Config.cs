using Druk.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.WeChat
{
    #region 开放平台
    /// <summary>
    /// 开放平台
    /// </summary>
    public static class OpenService
    {
        /// <summary>对外接口地址</summary>
        public static string ApiUrl => DataCache.Sys_Params.GetValue("OpenService.ApiUrl");
        /// <summary>对外接口用户名</summary>
        public static string UserId => DataCache.Sys_Params.GetValue("OpenService.UserId");
        /// <summary>对外接口用户秘钥</summary>
        public static string UserSecret => DataCache.Sys_Params.GetValue("OpenService.UserSecret");

        /// <summary>
        /// 是否启动第三方平台
        /// </summary>
        public static bool IsStart => DataCache.Sys_Params.GetValue("OpenService.IsStart").ToInt() == 1;
    }
    #endregion
}

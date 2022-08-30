using Druk.API.CMS.Util;
using Druk.Common.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Controllers
{
    /// <summary>
    /// 测试控制器
    /// </summary>
    public class TestController : BaseController
    {
        public static string key = "VZXp2Y8N3IszhwTpsz4BKQrv6HrNljNK";    //32位
        public static string iv = "4ORYvHbBr6Ux1XCj";   //16位

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResponse SqlAesEncrypt(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return ErrorPara();
            }
            var connStr = Common.DoEncrypt.AesEncrypt(sql, key, iv);
            return SuccessResult(connStr);
        }
    }
}

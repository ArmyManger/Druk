using Druk.API.CMS.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Controllers
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class AdminController : BaseController
    {
        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult login()
        {
            return Ok("请求成功");
        }
    }
}

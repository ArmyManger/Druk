using Druk.API.CMS.Model;
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
    ///用户
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add(UserModel usermodel)
        {
            return Ok("请求成功");
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult List()
        {
            return Ok("请求成功");
        }
    }
}

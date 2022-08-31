using Druk.API.CMS.Util;
using Druk.Common.Entity;
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
        /// 获取用户列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResponse List(string keyWord = "", int page = 1, int pageSize = Common.Config.DefaultPageSize)
        {

            return SuccessResult();
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResponse Detail(int id = 0)
        {
            if (id <= 0)
            {
                ErrorResult();
            }

            return SuccessResult();
        }
    }
}

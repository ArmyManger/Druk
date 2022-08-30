using Druk.API.CMS.Operation;
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
    /// 管理员
    /// </summary>
    public class AdminController : BaseController
    {
        /// <summary>
        /// 增加管理员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse Add(AdminPJson adminPjson)
        {
            if (adminPjson == null)
            {
                return ErrorPara();
            }
            if (!OperationAdmin.Add(adminPjson))
            {
                return ErrorResult();
            }
            return SuccessResult();
        }
    }
}

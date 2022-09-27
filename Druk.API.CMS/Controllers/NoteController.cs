using Druk.API.CMS.Util;
using Druk.Common.Entity;
using Druk.Handle;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Controllers
{
    public class NoteController : BaseController
    { 
        /// <summary>
        /// 操作配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse Add(AdminPJson adminPjson)
        {
            ConfigInfo<ClientConfig> configInfo = new ConfigInfo<ClientConfig>("ClientConfig.cfg");
            configInfo.CurrentConfig.Server = "10";
            configInfo.Save();
            configInfo.Read();
            configInfo.CurrentConfig.Server = "100";
            configInfo.Save();
            return SuccessResult();
        } 
    }
}

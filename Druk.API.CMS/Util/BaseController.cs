using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Util
{ 
    [Produces("application/json")] //返回数据的格式 直接约定为Json
    [EnableCors("UseCore")]   //跨域方案名称 可以在StartUp 内查看设置内容
    [Route("api/[controller]/[action]")]  //路由
    [ApiController]
    public class BaseController : ControllerBase
    {
    }
}

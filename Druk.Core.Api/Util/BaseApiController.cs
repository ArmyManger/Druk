using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.Core.Api.Util
{
    [Route("api/[controller]/[action]")]  //路由
    [ApiController]
    [Authorize]
    public class BaseApiController : Controller
    {
        
    }
}

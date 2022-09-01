using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.Core.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PayController : ControllerBase
    {

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}

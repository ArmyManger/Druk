using Druk.Core.Api.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Druk.Core.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginController : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string username, string password)
        {

            if (username != null)
            {
                var claims = new[]
               {
                   new Claim(ClaimTypes.Name,username),
                   new Claim(JwtRegisteredClaimNames.Jti, "1"), //唯一标识 
                   new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()), //jwt的签发时间
                   new Claim(JwtRegisteredClaimNames.Sub, "subName"), //主题
                   new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),    //NotBefore  token生效时间
                   new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddMilliseconds(1)).ToUnixTimeSeconds()}") //Expiration  到期时间，按秒数计算
                };

                var Issuer = Common.Helper.Appsettings.app(new string[] { "Audience", "Issuer" });
                var Audience = Common.Helper.Appsettings.app(new string[] { "Audience", "Audience" });
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123456888jdijxhelloworldprefect"));   //key的长度要超过16个字符，不然回抛出异常
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                   issuer: Issuer,
                   audience: Audience,
                   claims: claims,
                   expires: DateTime.Now.AddMinutes(30),
                   signingCredentials: creds);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    success = true,
                    message = "登录成功"
                });
            }
            else
            {
                return BadRequest(new { success = false, message = "登录失败，请重试" });
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet] 
        public IActionResult List()
        {
            return Ok();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet] 
        public IActionResult Add()
        {
            return Ok();
        }
    }
}

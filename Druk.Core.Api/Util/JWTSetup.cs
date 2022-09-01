using Druk.Common.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Druk.Core.Api.Util
{
    public static class JWTSetup
    {
        public static void AddJwt(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var Issuer = Appsettings.app(new string[] { "Audience", "Issuer" });
            var Audience = Appsettings.app(new string[] { "Audience", "Audience" });
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123456888jdijxhelloworldprefect"));   //key的长度要超过16个字符，不然回抛出异常
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //使用jwt进行认证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,  //是否验证超时  当设置exp和nbf时有效 
                        ValidateIssuerSigningKey = true,  ////是否验证密钥
                        ValidAudience = Audience,
                        ValidIssuer = Issuer,
                        IssuerSigningKey = key,
                        //缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟                                                                                                            //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                        ClockSkew = TimeSpan.FromMinutes(30),  //设置过期时间
                        //RequireExpirationTime = true,
                    };
                });
            // 开启Bearer认证
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }
    }
}

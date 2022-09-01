using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.Core.Api.Util
{
    public static class SwaggerSetUp
    {
        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwagger(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    // {ApiName} 定义成全局变量，方便修改
                    Version = "v1",
                    Title = "DrukCoreApi——Netcore 3.1",
                    Description = "自定义接口文档",
                });
                c.OrderActionsBy(o => o.RelativePath);

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Druk.Core.Api.xml");
                c.IncludeXmlComments(xmlPath, true);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                { 
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey

                });

                //加载授权方案
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"  //授权方案名称
                            }
                        },
                        new string[] { }
                    }
                }); 
            });
        }
    }
}

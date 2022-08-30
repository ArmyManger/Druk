using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Util
{
    /// <summary>
    /// swagger启动
    /// </summary>
    public static class SwaggerSetUp
    {
        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var ApiName = "Druk.API.CMS";

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    // {ApiName} 定义成全局变量，方便修改
                    Version = "V1",
                    Title = $"{ApiName} 接口文档——Netcore 3.1",
                    Description = $"{ApiName} HTTP API V1",

                });
                c.OrderActionsBy(o => o.RelativePath);

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Druk.API.CMS.xml");
                c.IncludeXmlComments(xmlPath, true);
                var xmlPathModel = Path.Combine(AppContext.BaseDirectory, "Druk.API.CMS.Model.xml");
                c.IncludeXmlComments(xmlPathModel, true);
                //添加httpHeader参数
                c.OperationFilter<Util.SwaggerOperation>();
                c.DocumentFilter<Util.HiddenApiFilter>();
            });
        }
    }
}

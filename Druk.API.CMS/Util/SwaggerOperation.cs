using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Util
{
    /// <summary>
    /// swagger
    /// </summary>
    public class SwaggerOperation : IOperationFilter
    {
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters = operation.Parameters ?? new List<OpenApiParameter>();
            //添加Authorization头部参数
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "token",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema { Type = "string" },
                Required = false,
                Description = "Token参数",

            });
        }
    }


    /// <summary> 
    /// 隐藏接口，不生成到swagger文档展示 
    /// </summary> 
    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Class)]
    public partial class HiddenApiAttribute : System.Attribute
    {

        public HiddenApiAttribute() { }

        /// <summary>
        /// 需要在哪个运行模式隐藏
        /// 0：忽略模式，始终隐藏
        /// </summary>
        /// <param name="_runMode"></param>
        public HiddenApiAttribute(int _runMode)
        {
            runMode = _runMode;
        }
        /// <summary>
        /// 运行模式
        /// </summary>
        public int runMode { get; set; } = 0;

    }
    /// <summary>
    /// 隐藏api过滤器
    /// </summary>
    public class HiddenApiFilter : IDocumentFilter
    {
        /// <summary>
        /// 隐藏api过滤器
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //var HiddenApi = Tools.HiddenApi;配置项
            var HiddenApi = false;
            if (HiddenApi)
            {
                //隐藏所有api
                foreach (var ignoreApi in context.ApiDescriptions)
                {
                    swaggerDoc.Paths.Remove("/" + ignoreApi.RelativePath);
                }
            }
            else
            {
                //隐藏部分api
                var ignoreApis = context.ApiDescriptions.Where(x => x.CustomAttributes().Any(any => any is HiddenApiAttribute));
                if (ignoreApis != null)
                {
                    foreach (var ignoreApi in ignoreApis)
                    {
                        swaggerDoc.Paths.Remove("/" + ignoreApi.RelativePath);
                    }
                }
            }
        }
    }
}

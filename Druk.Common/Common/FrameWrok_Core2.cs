using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common
{

    #region HttpContext
    /// <summary>
    /// 上下文对象,程序里都可以使用
    /// </summary>
    public static class HttpContext
    {
        private static IHttpContextAccessor _accessor;

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                if (_accessor == null)
                    return null;
                return _accessor.HttpContext;
            }
        }

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }

    /// <summary>
    /// IServiceCollection 扩展方法, 用于注册HttpContext
    /// </summary>
    public static class StaticHttpContextExtensions
    {
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        //注册使用HttpContext
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            HttpContext.Configure(httpContextAccessor);
            IocManager.SetService(app.ApplicationServices);
            return app;
        }
    }

    public static class IocManager
    {
        private static IServiceProvider Service;
        public static void SetService(IServiceProvider serviceProvider)
        {
            Service = serviceProvider;
        }
        public static TService GetService<TService>() where TService : class
        {
            if (Service == null) return null;
            using var scope = Service.CreateScope();
            return scope.ServiceProvider.GetService<TService>();
        }
    } 
    #endregion

    #region Path 获取项目的绝对路径 使用DoPath下的方法
    public static class Extensions
    {
        public static IServiceCollection AddWkMvcDI(this IServiceCollection services)
        {
            return services;
        }

        public static IApplicationBuilder UseWkMvcDI(this IApplicationBuilder builder)
        {
            DI.ServiceProvider = builder.ApplicationServices;
            return builder;
        }
    }

    public static class DI
    {
        public static IServiceProvider ServiceProvider
        {
            get; set;
        }
    }

    #endregion

    #region ConfigJson  读取项目配置文件 Config.json

    public static class ConfigJson
    {
        public static ConfigJson_Temp AppSettings
        {
            get
            {
                return new ConfigJson_Temp();
            }
        }

        public class ConfigJson_Temp
        {
            public string this[string name]
            {
                get
                {
                    try
                    {
                        var appVersion = Common.Config.appVersion;
                        var configFile = string.Empty;
                        switch (appVersion)
                        {
                            case "dev":
                            case "uat":
                            case "prod":
                            default:
                                configFile = $"Config.{appVersion}.json";
                                break;
                        }
                        var configuration = new ConfigurationBuilder();
                        configuration.AddJsonFile(configFile);
                        return configuration.Build()[name];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToSimple("Config文件读取失败，目标: " + name));
                        return null;
                    }
                }
            }
        }
    }

    #endregion


}

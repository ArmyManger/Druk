using Druk.API.CMS.Util;
using Druk.Common;
using Druk.Common.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS
{
    /// <summary>
    /// 启动
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region //中间件 注册使用HttpContext
            //注入HttpContextAccessor
            //配置后， 可以在Controller中获取HttpContextAccessor， 并用它来访问HttpContext
            Druk.Common.StaticHttpContextExtensions.AddHttpContextAccessor(services);
            #endregion

            #region //中间件 注册使用跨域
            //services.AddCors(options => options.AddPolicy("UseCore", builder => builder
            //  .WithOrigins(new Tools().CorsDomain)    //从Config.json 中获取的允许跨域的站点配置
            //  .SetIsOriginAllowedToAllowWildcardSubdomains() //允许使用通配符来描述允许的站点列表
            //  .WithMethods("get", "post", "put", "delete")   //允许的请求方法 Get Post 
            //  .AllowAnyHeader()  //允许任何Header  不对头信息进行任何验证
            //  .AllowCredentials() //允许Cookie跨域传递 在Ajax中 也需要设置 xhrFields: { withCredentials: true },
            //    )); 
            services.AddCors(options => options.AddPolicy("CorsPolicy",
             builder =>
             {
                 builder.AllowAnyMethod()
                     .SetIsOriginAllowed(_ => true)
                     .AllowAnyHeader()
                     .AllowCredentials();
             }));
            #endregion

            #region //中间件 NewTonsoft
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                //取消返回Josn数据默认首字母小写
                options.UseMemberCasing();
                //设置返回json数据中datetime类型字段的格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            #endregion

            #region //中间件 指定权限验证的方式 
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => { });
            services.AddControllersWithViews(option =>
            {
                option.Filters.Add(typeof(Util.MyAuthorizeFilter)); //并添加自定义过滤器
                option.RespectBrowserAcceptHeader = true;
            });
            #endregion

            #region //IIS反向代理设置
            services.Configure<IISOptions>(options =>
            {
                //当使用Https请求时,反向代理是否会获取客户端的证书,默认为false,不请求
                //详情 https://docs.azure.cn/zh-cn/service-fabric/service-fabric-reverseproxy-configure-secure-communication
                options.ForwardClientCertificate = false;
            });
            #endregion

            #region //HttpClient 服务设置
            services.AddHttpClient();//添加协议

            services.AddHttpClientServiceList(new List<Client> {
                new Client{ clientName="CMSAPI" },
             });
            #endregion
             
            services.AddSwaggerSetup();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
             
            app.UseRouting();
            //注册使用跨域
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                //特性路由(WebApi)
                endpoints.MapControllers().RequireCors("CorsPolicy");
                //控制器路由(MVC)
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
             
            #region 注册中间件
            //注册使用权限验证过滤器
            app.UseAuthentication();
            //注册使用HttpContext
            app.UseStaticHttpContext();
            //能够获取项目绝对路径的中间件
            app.UseWkMvcDI();

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();
            //注册使用swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "Druk.API.CMS V1");
                c.RoutePrefix = "swagger";
            });
            app.Use(next => new RequestDelegate(
             async context =>
             {
                 context.Request.EnableBuffering();
                 await next(context);
             }
          ));
            #endregion
        }
    }
}

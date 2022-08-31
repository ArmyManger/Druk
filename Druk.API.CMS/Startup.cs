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
    /// ����
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
            #region //�м�� ע��ʹ��HttpContext
            //ע��HttpContextAccessor
            //���ú� ������Controller�л�ȡHttpContextAccessor�� ������������HttpContext
            Druk.Common.StaticHttpContextExtensions.AddHttpContextAccessor(services);
            #endregion

            #region //�м�� ע��ʹ�ÿ���
            //services.AddCors(options => options.AddPolicy("UseCore", builder => builder
            //  .WithOrigins(new Tools().CorsDomain)    //��Config.json �л�ȡ����������վ������
            //  .SetIsOriginAllowedToAllowWildcardSubdomains() //����ʹ��ͨ��������������վ���б�
            //  .WithMethods("get", "post", "put", "delete")   //��������󷽷� Get Post 
            //  .AllowAnyHeader()  //�����κ�Header  ����ͷ��Ϣ�����κ���֤
            //  .AllowCredentials() //����Cookie���򴫵� ��Ajax�� Ҳ��Ҫ���� xhrFields: { withCredentials: true },
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

            #region //�м�� NewTonsoft
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                //ȡ������Josn����Ĭ������ĸСд
                options.UseMemberCasing();
                //���÷���json������datetime�����ֶεĸ�ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            #endregion

            #region //�м�� ָ��Ȩ����֤�ķ�ʽ 
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => { });
            services.AddControllersWithViews(option =>
            {
                option.Filters.Add(typeof(Util.MyAuthorizeFilter)); //������Զ��������
                option.RespectBrowserAcceptHeader = true;
            });
            #endregion

            #region //IIS�����������
            services.Configure<IISOptions>(options =>
            {
                //��ʹ��Https����ʱ,��������Ƿ���ȡ�ͻ��˵�֤��,Ĭ��Ϊfalse,������
                //���� https://docs.azure.cn/zh-cn/service-fabric/service-fabric-reverseproxy-configure-secure-communication
                options.ForwardClientCertificate = false;
            });
            #endregion

            #region //HttpClient ��������
            services.AddHttpClient();//���Э��

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
            //ע��ʹ�ÿ���
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                //����·��(WebApi)
                endpoints.MapControllers().RequireCors("CorsPolicy");
                //������·��(MVC)
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
             
            #region ע���м��
            //ע��ʹ��Ȩ����֤������
            app.UseAuthentication();
            //ע��ʹ��HttpContext
            app.UseStaticHttpContext();
            //�ܹ���ȡ��Ŀ����·�����м��
            app.UseWkMvcDI();

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();
            //ע��ʹ��swagger
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

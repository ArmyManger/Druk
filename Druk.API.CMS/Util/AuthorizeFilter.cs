using Druk.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Util
{
    #region 过滤器 自定义特性 在可以加在控制器或者视图上 [NoLogin] 的方式 在权限验证方法中检查是否有该过滤器
    public class NoLoginAttribute : ActionFilterAttribute { }
    #endregion


    #region 过滤器 验证是否用了只读查询
    /// <summary>
    ///  只读查询
    /// </summary>
    public class OnlyReadDBAttribute : ActionFilterAttribute { }
    #endregion

    #region //拦截器 验证权限
    /// <summary>
    /// 自定义拦截器 过滤权限
    /// </summary>
    public class MyAuthorizeFilter : AuthorizeFilter
    {
        public readonly Stopwatch Timmer = new Stopwatch();


        private static AuthorizationPolicy _policy_ = new AuthorizationPolicy(new[] { new DenyAnonymousAuthorizationRequirement() }, new string[] { });
        /// <summary>
        /// 拦截器 过滤权限
        /// </summary>
        public MyAuthorizeFilter() : base(_policy_) { }
        /// <summary>
        /// 业务访问前执行 过滤权限
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        { 
            try
            {
                Timmer.Start(); //启动计时器
                //验证当前请求是不是包含 允许匿名 的筛选器  [Util.NoLogin](自定义特性) 不必登录
                //如果当前请求是一个不登录也可以访问的地址
                var IsAllowAnonymous = context.Filters.Any(item => item is NoLoginAttribute);
                if (IsAllowAnonymous)
                {
                    //没有登录或者token失效时
                    if (context.HttpContext.GetHeaders("token") == null)
                    {

                        context.Result = new ObjectResult(ComEnum.Code.未登录.JsonR());
                    } 
                    //直接获取权限
                    //var authList = Operation.User.Login.GetCurrentUserAuth();
                    //if (authList == null)
                    //{
                    //    context.Result = new ObjectResult(ComEnum.Code.Token失效.JsonR());
                    //}
                    //if (context.RouteData.Values.Values.Count == 2)
                    //{
                    //    var Controller = context.RouteData.Values["controller"].ToString().ToLower();
                    //    var Action = context.RouteData.Values["Action"].ToString().ToLower();
                    //    if (authList.ContainsKey(Controller))
                    //    {
                    //        if (authList[Controller].Contains(Action))
                    //        {
                    //            //List action用的是只读库，休眠一秒后在取数据
                    //            //if (Action.ToUpper().EndsWith("LIST"))
                    //            //{
                    //            //    var IsOnlyReadDB = context.Filters.Any(item => item is OnlyReadDBAttribute);
                    //            //    if (IsOnlyReadDB)
                    //            //        System.Threading.Thread.Sleep(1000);   
                    //            //}

                    //            //不做操作, 用户进入路径
                    //        }
                    //        else { context.Result = new ObjectResult(ComEnum.Code.无权限.JsonR()); }
                    //    }
                    //    else { context.Result = new ObjectResult(ComEnum.Code.无权限.JsonR()); }
                    //}
                    //else { context.Result = new ObjectResult(ComEnum.Code.未找到对应权限.JsonR()); }

                }

                //如果上方直接给了Context.Rssult内容,则不会执行后方的任何方法,所以要在此直接存储PV
                if (context.Result != null)
                {

                    ////记录pv访问日志
                    //var User = Operation.User.Login.GetCurrentloginUser(); 
                    //Utility.Tools.SavePV(
                    //        context,
                    //        GetFilterOfTimmer.GetTime(context.Filters), //获取
                    //        User != null ? User.id : 0,
                    //        User != null ? User.codeNo : string.Empty,
                    //        User != null ? User.loginName : string.Empty,
                    //        Tools.ThreadTagVisit
                    //    );
                }
            }
            catch (Exception ex)
            {
                Log.Error("权限验证出错;" + ex.Message, ComEnum.RabbitQueue.日志_常规);
            }
        }
    }
    #endregion


    #region //拦截器 业务程序处理完成之后  存储访问日志 以及处理程序中的异常

    /// <summary>
    /// 业务程序处理完成之后
    /// </summary>
    public class ExecutedFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 业务程序处理完成之后
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext context)
        {

        }
        /// <summary>
        /// 获取上下文信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetContextInfo(ActionExecutedContext context, int userId)
        {
            if (context == null) return "";
            var http = context.HttpContext;//协议内容
            if (http == null) return "";
            var json = new
            {
                userId = userId,
                IP = http.GetIP(),
                Method = http.GetMethod(),
                PageURL = http.GetRequestURL(),
                ParamsGet = http.GetQueryString(),
                ParamsPost = http.GetBodyForm(),
                Exception = context.Exception,
                Token = http.GetHeaders("token"),
                ConnID = http.Connection.Id
            };
            return json.ToJson();//转类型 
        }
    }
    #endregion

    #region //获取当前请求中的计时器
    /// <summary>
    /// 获取当前请求中的计时器
    /// </summary>
    static class GetFilterOfTimmer
    {
        public static long GetTime(IList<IFilterMetadata> filters)
        {
            var fff = filters.FirstOrDefault(o => o is MyAuthorizeFilter);
            if (fff != null)
            {
                var fiel = fff as MyAuthorizeFilter;//自定义拦截
                fiel.Timmer.Stop();
                return fiel.Timmer.ElapsedMilliseconds;
            }
            return 50000;
        }
    }
    #endregion
}

using Druk.Common.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Util
{ 
    [Produces("application/json")] //返回数据的格式 直接约定为Json
    [EnableCors("UseCore")]   //跨域方案名称 可以在StartUp 内查看设置内容
    [Route("api/[controller]/[action]")]  //路由
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 返回失败
        /// </summary>
        /// <param name="code"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected JsonResponse ErrorResult(Common.ComEnum.Code code = Common.ComEnum.Code.请求失败, string error = "", object body = null)
        {
            return new JsonResponse { body = body, code = code.GetHashCode(), message = string.IsNullOrEmpty(error) ? code.ToString() : error };
        }
        /// <summary>
        /// 返回失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="error"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        protected JsonResponse<T> Error<T>(Common.ComEnum.Code code = Common.ComEnum.Code.请求失败, string error = "", T body = default)
        {
            return new JsonResponse<T> { body = body, code = code.GetHashCode(), message = string.IsNullOrEmpty(error) ? code.ToString() : error };
        }
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected JsonResponse SuccessResult(object body = null, Common.ComEnum.Code code = Common.ComEnum.Code.请求成功, string message = "请求成功")
        {
            return new JsonResponse { body = body, code = code.GetHashCode(), message = message };
        }
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected JsonResponse<T> Success<T>(T body = default, Common.ComEnum.Code code = Common.ComEnum.Code.请求成功, string message = "请求成功")
        {
            return new JsonResponse<T> { body = body, code = code.GetHashCode(), message = message };
        }
    }
}

using Druk.Common.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common
{

    public static class Tools
    {
        #region //根据枚举值返回对应的JsonR

        /// <summary>
        /// 根据枚举值返回对应的JsonR
        /// </summary>
        /// <param name="codeNum"></param>
        /// <param name="body"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonResponse GetJsonR(int codeNum, object body = null, string message = "")
        {
            var Code = (ComEnum.Code)codeNum;
            return Code.JsonR(body, message);
        }
        #endregion

        #region //根据枚举返回对应的状态码
        /// <summary>
        /// 根据枚举返回对应的状态码
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonResponse JsonR(this Enum Code, object obj = null, string message = "")
        {

            //如果是必传字段为空,带上字段名称
            if (Code.GetHashCode() == ComEnum.Code.必填字段为空.GetHashCode()) { message = DoEnum.GetDesc(Code) + message; }

            return new JsonResponse()
            {
                code = Code.GetHashCode(),
                message = string.IsNullOrEmpty(message) ? DoEnum.GetDesc(Code) : message,
                body = obj
            };
        }

        /// <summary>
        /// 根据枚举返回对应的状态码, 预先指定了body的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Code"></param>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static JsonResponse<T> JsonRT<T>(this Enum Code, T obj, string message = "")
        {
            //如果是必传字段为空,带上字段名称
            if (Code.GetHashCode() == ComEnum.Code.必填字段为空.GetHashCode()) { message = DoEnum.GetDesc(Code) + message; }

            return new JsonResponse<T>()
            {
                code = Code.GetHashCode(),
                message = string.IsNullOrEmpty(message) ? DoEnum.GetDesc(Code) : message,
                body = obj
            };
        }

        #endregion


    }

    #region 分页处理工具
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class PagedResult<TResult>
    {
        /// <summary>
        /// 分页结果
        /// </summary>
        /// <param name="items">结果集</param>
        /// <param name="totalCount">数据总条数</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页尺寸</param>
        public PagedResult(IEnumerable<TResult> items, int totalCount, int page, int pageSize)
        {
            List = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
        /// <summary>
        /// 分页集合
        /// </summary>
        public virtual IEnumerable<TResult> List { get; private set; }
        /// <summary>
        /// 总数据量
        /// </summary>
        public virtual int TotalCount { get; private set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public virtual int Page { get; private set; }
        /// <summary>
        /// 页码大小
        /// </summary>
        public virtual int PageSize { get; private set; }
    }
    /// <summary>
    /// 分页结果拓展类
    /// </summary>
    public static class PagedResultExtension
    {
        /// <summary>
        /// 分页结果
        /// </summary>
        /// <typeparam name="T">分页数据项类型</typeparam>
        /// <param name="items">分页数据项</param>
        /// <param name="totalCount">总数居量</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页码大小</param>
        /// <returns></returns>
        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            return new PagedResult<T>(items, totalCount, page, pageSize);
        }
    } 
    #endregion
}

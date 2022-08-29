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
}

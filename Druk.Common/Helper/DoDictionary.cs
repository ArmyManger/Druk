using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Druk.Common
{
    public static class DoDictionary
    {
        #region //将键值对的数据转换为SQL的Case When 
        /// <summary>
        /// 将键值对的数据转换为SQL的Case When 
        /// 输出的值需要替换{0}为列名 {1}为默认值
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string ToCaseWhenSQL(this Dictionary<int, string> dic)
        {
            if (dic.Count > 0)
            {
                string result = " Case ";

                foreach (var item in dic)
                {
                    result += " When {0}='" + item.Key.ToString() + "' Then '" + item.Value.ToString() + "'";
                }
                result += " Else '{1}' End";
                return result;
            }
            else
            {
                return "''";
            }
        }
        #endregion

        #region //将字符串键值对转换为数字字符串键值对
        /// <summary>
        /// 将字符串键值对转换为数字字符串键值对
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<int, string> ToInt(this Dictionary<string, string> dic)
        {
            var result = new Dictionary<int, string>();
            foreach (var item in dic)
            {
                result[item.Key.ToInt()] = item.Value;
            }
            return result;
        }
        #endregion

        #region //从键值对中获取值
        /// <summary>
        /// 从键值对中获取值
        /// </summary>
        /// <typeparam name="valueT"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="isToLower"></param>
        /// <returns></returns>
        public static valueT GetValue<valueT>(this Dictionary<string, valueT> dic, string key, bool isToLower = true)
        {
            if (dic != null && dic.Count > 0 && !string.IsNullOrEmpty(key))
            {
                key = isToLower ? key.ToLower() : key;
                return dic.ContainsKey(key) ? dic[key] : default(valueT);
            }
            return default(valueT);
        }
        #endregion

        #region //将键值对对象添加到上下文对象中

        /// <summary>
        /// 将键值对对象添加到上下文对象中
        /// 主要用于后期的接口记录日志，
        /// 这样能够不但访问日志能够记录，逻辑中的运行日志也可以一起记录
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="ItemsName"></param>
        /// <returns></returns>
        public static bool InsertHttp(this Dictionary<string, object> dic, string ItemsName = "RunLog")
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    var http = HttpContext.Current;
                    if (!http.Items.ContainsKey(ItemsName))
                    {
                        http.Items[ItemsName] = dic;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToSimple("键值对添加到上下文时出错"));
                return false;
            }
        }
        #endregion

        #region Dictionary key忽略大小写获取第一位值
        /// <summary>
        /// ictionary key忽略大小写获取第一位值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T TryGetValueIgnoreCase<T>(this IDictionary<string, T> dic, string key) => TryGetValueIgnoreCase(dic, key, default(T));

        /// <summary>
        /// Dictionary key忽略大小写获取第一位值
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryGetValueIgnoreCase<T>(this IDictionary<string, T> dic, string key, T defaultValue)
        {
            if (dic == null || string.IsNullOrEmpty(key))
            {
                return defaultValue;
            }
            var exKey = dic.Keys.FirstOrDefault(c => string.Compare(c, key, StringComparison.OrdinalIgnoreCase) == 0);
            return string.IsNullOrEmpty(exKey) ? defaultValue : dic[exKey];
        }
        #endregion

        #region Dictionary key忽略大小写移除项
        /// <summary>
        /// Dictionary key忽略大小写移除项
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveIgnoreCase<T>(this IDictionary<string, T> dic, string key)
        {
            if (dic == null || string.IsNullOrEmpty(key))
            {
                return false;
            }
            var exKey = dic.Keys.FirstOrDefault(c => string.Compare(c, key, StringComparison.OrdinalIgnoreCase) == 0);
            if (string.IsNullOrEmpty(exKey))
            {
                return false;
            }
            return dic.Remove(exKey);
        }
        #endregion
    }
}

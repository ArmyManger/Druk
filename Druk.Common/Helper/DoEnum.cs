using Druk.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Druk.Common
{
    public static class DoEnum
    {
        /// <summary>
        /// 根据字符串获取枚举描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string GetDescByName<T>(string Name)
        {
            var obj = Enum.Parse(typeof(T), Name) as Enum;
            return obj != null && obj.GetType().IsEnum ? obj.GetDesc() : string.Empty;
        }


        /// <summary>
        /// 根据字符串获取枚举描述
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetDescByNumber<T>(int number)
        {
            var obj = Enum.Parse(typeof(T), number.ToString()) as Enum;
            var dic = obj.GetDescAll();
            return obj != null && obj.GetType().IsEnum && dic != null && dic.ContainsKey(number) ? dic[number] : string.Empty;
        }


        #region //扩展函数

        /// <summary>
        /// 查询枚举name
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnumName(this Enum obj)
        {
            return obj.ToString();
        }
        /// <summary>
        /// 查询枚举value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetEnumValue(this Enum obj)
        {
            return obj.GetHashCode();
        }
        /// <summary>
        /// 查询枚举描述
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDesc(this Enum obj)
        {
            var Type = obj.GetType();
            FieldInfo[] fieldinfos = Type.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum && Enum.Parse(Type, field.Name).GetHashCode() == obj.GetHashCode())
                {
                    Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    return ((DescriptionAttribute)objs[0]).Description;
                }
            }
            return "";
        }
        /// <summary>
        /// 获取枚举的所有描述
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetDescAll(this Enum obj)
        {
            var Type = obj.GetType();
            if (Type.IsEnum)
            {
                var dic = new Dictionary<int, string>();
                FieldInfo[] fieldinfos = Type.GetFields();
                foreach (FieldInfo field in fieldinfos)
                {
                    if (field.FieldType.IsEnum)
                    {
                        Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        dic.Add(Enum.Parse(Type, field.Name).GetHashCode(), ((DescriptionAttribute)objs[0]).Description);
                    }
                }
                return dic;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取枚举的所有描述
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<JsonR_SimpleCode> GetDescAll_Dynamic(this Enum obj)
        {
            var Type = obj.GetType();
            if (Type.IsEnum)
            {
                var dic = new List<JsonR_SimpleCode>();
                FieldInfo[] fieldinfos = Type.GetFields();
                foreach (FieldInfo field in fieldinfos)
                {
                    if (field.FieldType.IsEnum)
                    {
                        Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        dic.Add(new JsonR_SimpleCode
                        {
                            id = Enum.Parse(Type, field.Name).GetHashCode(),
                            name = ((DescriptionAttribute)objs[0]).Description,
                            codeNo = field.Name,
                        });
                    }
                }
                return dic;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取枚举所有的描述值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<string> GetDescAllName(this Enum obj)
        {
            var dic = obj.GetDescAll();
            return dic != null ? dic.Select(o => o.Value).ToList() : null;
        }

        /// <summary>
        /// 根据枚举值输出特定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="number"></param>
        /// <returns></returns>
        public static JsonR_Simple GetJsonrSimple<T>(int number)
        {
            var obj = Enum.Parse(typeof(T), number.ToString()) as Enum;
            var dic = obj.GetDescAll();
            if (obj != null && obj.GetType().IsEnum && dic != null && dic.ContainsKey(number))
            {
                return new JsonR_Simple(number, dic[number]);
            }
            return new JsonR_Simple();
        } 
        #endregion 
    }
}

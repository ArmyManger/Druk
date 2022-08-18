using Ganss.XSS;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Druk.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class DoObjectExtensions
    {
        #region //this Byte[]


        /// <summary>
        /// Hashes the specified data.使用默认算法Hash
        /// </summary>
        public static byte[] Hash(this byte[] data) { return Hash(data, null); }
        /// <summary>
        /// Hashes the specified data.使用指定算法Hash
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="hashName">Name of the hash.</param>
        /// <returns></returns>
        public static byte[] Hash(this byte[] data, string hashName) { HashAlgorithm algorithm = string.IsNullOrEmpty(hashName) ? HashAlgorithm.Create() : HashAlgorithm.Create(hashName); return algorithm.ComputeHash(data); }
        public static byte[] ToBytes(this string data) { return ToBytes(data, Encoding.UTF8); }
        public static byte[] ToBytes(this string data, Encoding encoding) { data = data ?? ""; return encoding.GetBytes(data); }
        public static string ToStr(this byte[] data) { return ToStr(data, Encoding.UTF8); }
        public static string ToStr(this byte[] data, Encoding encoding) { data = data ?? new byte[] { }; return encoding.GetString(data); }



        #endregion

        #region // string[] List<string>
        /// <summary>
        /// string[]内所有元素转换为小写
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static string[] ToLower(this string[] Obj)
        {
            return Obj.ToList().ToLower().ToArray();
        }
        /// <summary>
        /// List
        /// </summary>内所有元素转换为小写
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static List<string> ToLower(this List<string> Obj)
        {
            for (int i = 0; i < Obj.Count; i++)
            {
                Obj[i] = Obj[i].ToLower();
            }

            return Obj;
        }

        /// <summary>
        /// string[]内所有元素转换为大写
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static string[] ToUpper(this string[] Obj)
        {
            return Obj.ToList().ToUpper().ToArray();
        }
        /// <summary>
        /// List
        /// </summary>内所有元素转换为小写
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static List<string> ToUpper(this List<string> Obj)
        {
            for (int i = 0; i < Obj.Count; i++)
            {
                Obj[i] = Obj[i].ToUpper();
            }
            return Obj;
        }
        #endregion

        #region //this Byte

        /// <summary>
        /// Gets the bit.获取取第index是否为1,index从0开始
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static bool GetBit(this byte b, int index)
        {
            return (b & (1 << index)) > 0;
        }
        /// <summary>
        /// Sets the bit.将第index位设为1
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static byte SetBit(this byte b, int index)
        {
            b |= (byte)(1 << index);
            return b;
        }

        /// <summary>
        /// 将第index位设为0
        /// Clears the bit.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static byte ClearBit(this byte b, int index)
        {
            b &= (byte)((1 << 8) - 1 - (1 << index));
            return b;
        }

        /// <summary>
        /// Reverses the bit.
        /// 将第index位取反
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static byte ReverseBit(this byte b, int index)
        {
            b ^= (byte)(1 << index);
            return b;
        }


        #endregion

        #region //this Object


        #region //基础类型 转换

        public static int ToInt(this object obj) { return obj.ToInt(0); }
        public static int ToInt(this object obj, int defValue)
        {
            obj = obj ?? defValue; int def; if (int.TryParse(obj.ToString(), out def)) { return def; }
            try { var dou = obj.ToDouble(); return Convert.ToInt32(dou >= 0 ? Math.Floor(dou) : Math.Ceiling(dou)); } catch { return defValue; }
        }
        public static byte ToByte(this object obj) { return obj.ToByte(0); }
        public static byte ToByte(this object obj, byte defValue) { obj = obj ?? defValue; byte def; byte.TryParse(obj.ToString(), out def); return def == 0 ? defValue : def; }
        public static short ToShort(this object obj) { return obj.ToShort(0); }
        public static short ToShort(this object obj, short defValue) { obj = obj ?? defValue; short def; short.TryParse(obj.ToString(), out def); return def == 0 ? defValue : def; }
        public static decimal ToDecimal(this object obj) { return obj.ToDecimal(0); }
        public static decimal ToDecimal(this object obj, decimal defValue) { obj = obj ?? defValue; decimal def; decimal.TryParse(obj.ToString(), out def); return def == 0 ? defValue : def; }
        public static long ToLong(this object obj) { return obj.ToLong(0); }
        public static long ToLong(this object obj, long defValue) { obj = obj ?? defValue; long def; long.TryParse(obj.ToString(), out def); return def == 0 ? defValue : def; }
        public static Int64 ToInt64(this object obj) { return obj.ToInt64(0); }
        public static Int64 ToInt64(this object obj, Int64 defValue) { obj = obj ?? defValue; Int64 def; Int64.TryParse(obj.ToString(), out def); return def == 0 ? defValue : def; }
        public static float ToFloat(this object obj) { return obj.ToFloat(0); }
        public static float ToFloat(this object obj, float defValue) { obj = obj ?? defValue; float def; float.TryParse(obj.ToString(), out def); return def == 0 ? defValue : def; }
        public static double ToDouble(this object obj) { return obj.ToDouble(0); }
        public static double ToDouble(this object obj, double defValue) { obj = obj ?? defValue; double def; double.TryParse(obj.ToString(), out def); return def == 0 ? defValue : def; }
        public static bool ToBool(this object obj) { return obj.ToBool(false); }
        public static bool ToBool(this object obj, bool defValue) { obj = obj ?? defValue; bool def; bool.TryParse(obj.ToString(), out def); return !def ? defValue : def; }
        public static DateTime ToDateTime(this object obj) { return obj.ToDateTime(Config.DefaultDateTime); }
        public static DateTime ToDateTime(this object obj, DateTime defValue) { obj = obj ?? defValue; DateTime def; DateTime.TryParse(obj.ToString(), out def); return def == Convert.ToDateTime(null) ? defValue : def; }

        #endregion

        #region //实体类型

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object value) { return value.ConvertTo(default(T)); }
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object value, T defaultValue)
        {
            if (value != null)
            {
                var targetType = typeof(T);

                var converter = TypeDescriptor.GetConverter(value);
                if (converter != null)
                {
                    if (converter.CanConvertTo(targetType)) return (T)converter.ConvertTo(value, targetType);
                }

                converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null)
                {
                    try { if (converter.CanConvertFrom(value.GetType())) return (T)converter.ConvertFrom(value); }
                    catch { }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="ignoreException">if set to <c>true</c> [ignore exception].</param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object value, T defaultValue, bool ignoreException)
        {
            if (ignoreException)
            {
                try
                {
                    return value.ConvertTo<T>();
                }
                catch
                {
                    return defaultValue;
                }
            }
            return value.ConvertTo<T>();
        }
        #endregion

        #region //判断对象类型

        /// <summary>
        /// 判断对象是否为指定类型
        /// </summary>
        public static bool IsType(this object obj, Type type) { return obj.GetType().Equals(type); }
        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsInt(this object obj) { return obj.ToInt().ToString() == obj.ToString(); }
        /// <summary>
        /// 验证是否为Double
        /// </summary>
        /// <returns></returns>
        public static bool IsDouble(this object obj) { return obj.ToDouble().ToString() == obj.ToString(); }
        /// <summary>
        /// 验证是否为时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDateTime(this object obj) { DateTime defValue; return obj != null && DateTime.TryParse(obj.ToString(), out defValue); }
        /// <summary>
        /// 判断对象是否为数组
        /// </summary>
        public static bool IsArrayType(this object obj) { return obj.IsType(typeof(Array)); }
        /// <summary>
        /// 判断给定的数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        public static bool IsIntArray(this object[] strNumber)
        {
            if (strNumber == null) { return false; }
            if (strNumber.Length == 0) { return false; }
            foreach (string id in strNumber) { if (!id.IsInt()) { return false; } }
            return true;
        }
        /// <summary>
        /// 验证对象是否为DBNull
        /// DBNull主要应用于数据库
        /// </summary>
        public static bool IsDBNullType(this object obj) { return obj.IsType(typeof(DBNull)); }

        /// <summary>
        /// 正则表达式匹配是否数值类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNumericType(this string obj) { Regex r = new Regex(@"([1-9]\d*\.?\d*)|(0\.\d*[1-9])"); return r.IsMatch(obj); }

        /// <summary>
        /// 判断类型是否是数值类型 
        /// 包括了int,double,long,short,float,Int16,Int32,Int64,uint,UInt16,UInt32,UInt64,sbyte,Single,Decimal
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type o)
        {
            return !o.IsClass && !o.IsInterface && o.GetInterfaces().Any(q => q == typeof(IFormattable));
        }
        #endregion

        #endregion


        #region //  <T>

        #region //数据填充到对象

        /// <summary>
        /// 实体数据填充
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T FillFrom<T>(this T obj, object source) where T : class, new()
        {
            PropertyInfo[] pis = typeof(T).GetProperties();
            PropertyInfo[] pisSource = source.GetType().GetProperties();

            foreach (PropertyInfo pi in pis)
            {
                try
                {
                    foreach (var propertyInfo in pisSource)
                    {
                        if (propertyInfo.Name == pi.Name)
                        {
                            object value = propertyInfo.GetValue(source, null);
                            pi.SetValue(obj, value, null);

                            break;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return obj;
        }

        /// <summary>
        /// 从FormCollection表单填充
        /// 要求表单中的对象名与实体的属性名一样
        /// </summary>
        /// <param name="formCollection">页面表单</param>
        public static T FillFromCollection<T>(this T obj, FormCollection formCollection) where T : class, new()
        {
            PropertyInfo[] pis = typeof(T).GetProperties();

            foreach (PropertyInfo pi in pis)
            {
                try
                {
                    //if (formCollection[pi.Name] != null)
                    //{
                    //    object value = formCollection[pi.Name] ?? "";
                    //    if (pi.PropertyType == typeof(int))
                    //        value = value.ToInt();
                    //    if (pi.PropertyType == typeof(decimal))
                    //        value = value.ToDecimal();
                    //    if (pi.PropertyType == typeof(int?))
                    //        value = value.ToInt();
                    //    if (pi.PropertyType == typeof(bool))
                    //        value = value.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length > 0 && value.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0] == "true";
                    //    if (pi.PropertyType == typeof(DateTime))
                    //        value = value.ToDateTime();

                    //    pi.SetValue(obj, value, null);
                    //}
                }
                catch (Exception) { }
            }
            return obj;
        }

        #endregion

        #region //Invoke 使用当前参数调取当前实体的methodName方法

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object InvokeMethod(this object obj, string methodName, params object[] parameters)
        {
            return InvokeMethod<object>(obj, methodName, parameters);
        }
        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public static T InvokeMethod<T>(this object obj, string methodName)
        {
            return InvokeMethod<T>(obj, methodName, null);
        }
        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static T InvokeMethod<T>(this object obj, string methodName, params object[] parameters)
        {
            if (obj != null)
            {
                var type = obj.GetType();
                var method = type.GetMethod(methodName);

                if (method == null) throw new ArgumentException(string.Format("Method '{0}' not found.", methodName), methodName);

                var value = method.Invoke(obj, parameters);
                return (value is T ? (T)value : default(T));
            }
            return default(T);
        }
        #endregion

        #region // Property 操作T的Public属性 (常用)

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return GetPropertyValue<object>(obj, propertyName, null);
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return GetPropertyValue(obj, propertyName, default(T));
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this object obj, string propertyName, T defaultValue)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);

            if (property == null) throw new ArgumentException(string.Format("Property '{0}' not found.", propertyName), propertyName);

            var value = property.GetValue(obj, null);
            return (value is T ? (T)value : defaultValue);
        }
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);

            if (property == null) throw new ArgumentException(string.Format("Property '{0}' not found.", propertyName), propertyName);

            property.SetValue(obj, value, null);
        }



        #endregion

        #region // Attribute 操作T的Private属性

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this object obj) where T : Attribute
        {
            return GetAttribute<T>(obj, true);
        }
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="includeInherited">if set to <c>true</c> [include inherited].</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this object obj, bool includeInherited) where T : Attribute
        {
            var type = (obj as Type ?? obj.GetType());
            var attributes = type.GetCustomAttributes(typeof(T), includeInherited);
            if ((attributes.Length > 0))
            {
                return (attributes[0] as T);
            }
            return null;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this object obj) where T : Attribute
        {
            return GetAttributes<T>(obj);
        }
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="includeInherited">if set to <c>true</c> [include inherited].</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this object obj, bool includeInherited) where T : Attribute
        {
            var type = (obj as Type ?? obj.GetType());
            return type.GetCustomAttributes(typeof(T), includeInherited).OfType<T>().Select(attribute => attribute);
        }

        #endregion

        #endregion

        #region //Json <=> T

        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            }
            catch (TargetInvocationException)
            {
                return string.Empty;
            }
            catch (PlatformNotSupportedException)
            {
                return string.Empty;
            }
            catch (Exception ex)
            {
                Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
                setting.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj, setting);
            }
        }

        /// <summary>
        /// json转对象
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static object ToObjectFromJson(this string str)
        {
            try
            {
                if (str.StartsWith('{') || str.StartsWith('['))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(str);
                }
                return str;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// json转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static T ToObjectFromJson<T>(this string str) where T : class
        {
            try
            {
                if (str.StartsWith('{') || str.StartsWith('['))
                {
                    var aaa = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);

                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        #endregion

        #region //XML <=> T

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static byte[] Serialize(this object value)
        {
            var ms = new MemoryStream();
            var bf1 = new BinaryFormatter();
            bf1.Serialize(ms, value);
            return ms.ToArray();
        }

        /// <summary>
        /// Serializes the XML file.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void SerializeXmlFile(this object o, string fileName)
        {
            var serializer = new XmlSerializer(o.GetType());
            if (!File.Exists(fileName)) return;
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) serializer.Serialize(stream, o);
        }
        /// <summary>
        /// Deserializes the XML file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static T DeserializeXmlFile<T>(string fileName)
        {
            T o;
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) o = (T)serializer.Deserialize(stream);
            return o;
        }

        /// <summary>
        /// Serializes the XML.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static string SerializeXml(this object o)
        {
            var serializer = new XmlSerializer(o.GetType());
            var stringBuilder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(stringBuilder)) serializer.Serialize(textWriter, o);
            return stringBuilder.ToString();
        }
        /// <summary>
        /// Deserializes the XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static T DeserializeXml<T>(this string xml)
        {
            return (T)Deserialize(xml, typeof(T));
        }
        /// <summary>
        /// Deserializes the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object Deserialize(string xml, Type type)
        {
            object o;
            var serializer = new XmlSerializer(type);
            using (TextReader textReader = new StringReader(xml)) o = serializer.Deserialize(textReader);
            return o;
        }

        #endregion


        #region //this String

        #region //this string[]

        public static byte[][] ToBytesArray(this string[] array)
        {
            return ToBytesArray(array, Encoding.UTF8);
        }

        public static byte[][] ToBytesArray(this string[] array, Encoding encoding)
        {
            List<byte[]> result = new List<byte[]>();
            foreach (string str in array)
            {
                result.Add(encoding.GetBytes(str));
            }
            return result.ToArray();
        }
        #endregion

        #region //Char[]

        public static List<string> ToStringArray(this char[] chars)
        {
            var strArray = new List<string>();
            chars.ToList().ForEach(o => strArray.Add(o.ToString()));
            return strArray;

        }
        #endregion

        #region //全角半角

        /// <summary>
        /// 转全角(SBC case)
        /// </summary>
        public static string ToSBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 转半角(DBC case)
        /// </summary>
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        #endregion

        #region //正则匹配

        /// <summary>
        /// 是否匹配规则
        /// </summary>
        public static bool IsMatch(this string str, string pattern) { if (str == null) return false; return Regex.IsMatch(str, pattern); }
        /// <summary>
        /// 筛选出字符串中匹配的字符,
        /// </summary>
        public static string Match(this string str, string pattern) { if (str == null) return ""; return Regex.Match(str, pattern).Value; }

        #endregion

        #endregion

        #region //this Random 随机数

        /// <summary>
        /// 随机一个Bool值
        /// </summary>
        public static bool NextBool(this Random random) { return random.NextDouble() > 0.5; }

        /// <summary>
        /// 随机枚举
        /// </summary>
        public static T NextEnum<T>(this Random random) where T : struct
        {
            Type type = typeof(T);
            if (type.IsEnum == false) throw new InvalidOperationException(); //如果不是枚举..抛异常

            //获取该枚举的所有值
            var array = Enum.GetValues(type);
            //最小和最大索引
            var index = random.Next(array.GetLowerBound(0), array.GetUpperBound(0) + 1);
            //通过随机数返得枚举并返回
            return (T)array.GetValue(index);
        }
        /// <summary>
        /// 随机Byte[]
        /// </summary>
        public static byte[] NextBytes(this Random random, int length) { var data = new byte[length]; random.NextBytes(data); return data; }
        /// <summary>
        /// 随机Int16 大于0的
        /// </summary>
        public static UInt16 NextUInt16(this Random random) { return BitConverter.ToUInt16(random.NextBytes(2), 0); }
        /// <summary>
        /// 随机Int16 可能小于0
        /// </summary>
        public static Int16 NextInt16(this Random random) { return BitConverter.ToInt16(random.NextBytes(2), 0); }
        /// <summary>
        /// 随机Float
        /// </summary>
        public static float NextFloat(this Random random) { return BitConverter.ToSingle(random.NextBytes(4), 0); }
        /// <summary>
        /// 随机任何时间
        /// </summary>
        public static DateTime NextDateTime(this Random random) { return NextDateTime(random, DateTime.MinValue, DateTime.MaxValue); }
        /// <summary>
        /// 区间内随机时间
        /// </summary>
        public static DateTime NextDateTime(this Random random, DateTime minValue, DateTime maxValue) { var ticks = minValue.Ticks + (long)((maxValue.Ticks - minValue.Ticks) * random.NextDouble()); return new DateTime(ticks); }


        #endregion

        #region //this Dictionary<TKey, TValue>  键值对
        /// <summary>
        /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// </summary>
        public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false) dict.Add(key, value);
            return dict;
        }
        /// <summary>
        /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
        /// </summary>
        public static Dictionary<TKey, TValue> AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
            return dict;
        }

        /// <summary>
        /// 获取与指定的键相关联的值，如果没有则返回输入的默认值
        /// </summary>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            return dict.ContainsKey(key) ? dict[key] : defaultValue;
        }

        /// <summary>
        /// 向字典中批量添加键值对
        /// </summary>
        /// <param name="replaceExisted">如果已存在，是否替换,</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> values, bool replaceExisted)
        {
            foreach (var item in values)
            {
                if (!dict.ContainsKey(item.Key))
                {
                    dict.Add(item.Key, item.Value);
                }
                else if (replaceExisted)
                {
                    dict[item.Key] = item.Value;
                }
            }
            return dict;
        }
        #endregion

        #region //this Expression 扩展  Expression<Func<T,bool>>

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            //var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, expr2.Body), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());

            return Expression.Lambda<Func<T, bool>>(Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression AndAlso(this Expression left, Expression right)
        {
            return Expression.AndAlso(left, right);
        }
        public static Expression Call(this Expression instance, string methodName, params Expression[] arguments)
        {
            return Expression.Call(instance, instance.Type.GetMethod(methodName), arguments);
        }
        public static Expression Property(this Expression expression, string propertyName)
        {
            return Expression.Property(expression, propertyName);
        }
        public static Expression GreaterThan(this Expression left, Expression right)
        {
            return Expression.GreaterThan(left, right);
        }
        public static Expression<TDelegate> ToLambda<TDelegate>(this Expression body, params ParameterExpression[] parameters)
        {
            return Expression.Lambda<TDelegate>(body, parameters);
        }
        #endregion

        #region //Distinct List<T> 去重操作

        /// <summary>
        /// Distincts the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TV>(this IEnumerable<T> source, Func<T, TV> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, TV>(keySelector));
        }

        /// <summary>
        /// Distincts the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV">The type of the V.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TV>(this IEnumerable<T> source, Func<T, TV> keySelector, IEqualityComparer<TV> comparer)
        {
            return source.Distinct(new CommonEqualityComparer<T, TV>(keySelector, comparer));
        }
        #endregion

        #region //IsBetween
        /// <summary>
        /// Determines whether the specified t is between.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="includeLowerBound">if set to <c>true</c> [include lower bound].</param>
        /// <param name="includeUpperBound">if set to <c>true</c> [include upper bound].</param>
        /// <returns>
        ///   <c>true</c> if the specified t is between; otherwise, <c>false</c>.
        /// </summary>
        public static bool IsBetween<T>(this T t, T lowerBound, T upperBound, bool includeLowerBound = false, bool includeUpperBound = false) where T : class, IComparable<T>
        {
            if (t == null) throw new ArgumentNullException("t");

            var lowerCompareResult = t.CompareTo(lowerBound);
            var upperCompareResult = t.CompareTo(upperBound);

            return (includeLowerBound && lowerCompareResult == 0) || (includeUpperBound && upperCompareResult == 0) || (lowerCompareResult > 0 && upperCompareResult < 0);
        }

        /// <summary>
        /// Determines whether the specified t is between.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="comparer">The comparer.</param>
        /// <param name="includeLowerBound">if set to <c>true</c> [include lower bound].</param>
        /// <param name="includeUpperBound">if set to <c>true</c> [include upper bound].</param>
        /// <returns>
        ///   <c>true</c> if the specified t is between; otherwise, <c>false</c>.
        /// </summary>
        public static bool IsBetween<T>(this T t, T lowerBound, T upperBound, IComparer<T> comparer, bool includeLowerBound = false, bool includeUpperBound = false)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");

            var lowerCompareResult = comparer.Compare(t, lowerBound);
            var upperCompareResult = comparer.Compare(t, upperBound);

            return (includeLowerBound && lowerCompareResult == 0) ||
                (includeUpperBound && upperCompareResult == 0) ||
                (lowerCompareResult > 0 && upperCompareResult < 0);
        }
        #endregion


        #region //对象过滤
        /// <summary>
        /// 返回安全的entity对象
        /// </summary>
        /// <param name="model">entity对象</param>
        /// <returns>安全的entity对象</returns>
        public static object ReturnSecurityObject(this object model)
        {
            var sanitizer = new HtmlSanitizer();
            Type t = model.GetType();//获取类型
            foreach (PropertyInfo propertyInfo in t.GetProperties())//遍历该类型下所有属性
            {
                if (propertyInfo.PropertyType == "".GetType())//如果属性为string类型
                {
                    var inputString = (propertyInfo.GetValue(model) ?? "").ToString();
                    propertyInfo.SetValue(model, sanitizer.Sanitize(inputString));//将过滤后的值设置给传入的对象
                }
            }
            return model;//返回安全对象
        }
        #endregion

    }

    #region //IEqualityComparer<T>

    /// <summary>
    /// 自定义的List<T>要实现 Distinct方法..必须实现IEqualityComparer<T>接口才能生效
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TV">The type of the V.</typeparam>
    public class CommonEqualityComparer<T, TV> : IEqualityComparer<T>
    {
        private Func<T, TV> keySelector;
        private IEqualityComparer<TV> comparer;

        public CommonEqualityComparer(Func<T, TV> keySelector, IEqualityComparer<TV> comparer)
        {
            this.keySelector = keySelector;
            this.comparer = comparer;
        }

        public CommonEqualityComparer(Func<T, TV> keySelector) : this(keySelector, EqualityComparer<TV>.Default)
        {

        }

        public bool Equals(T x, T y)
        {
            return comparer.Equals(keySelector(x), keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return comparer.GetHashCode(keySelector(obj));
        }

    }
    #endregion
}

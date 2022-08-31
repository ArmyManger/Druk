using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Druk.Common
{
    public class DoJson
    {
        #region DataTable <=> Json

        /// <summary>
        /// 将DataRow转换为Json格式
        /// </summary>
        /// <param name="dt">要转换的DataTable,只提供结构</param>
        /// <param name="dr">要转换的dr</param>
        /// <returns>转换后的json格式字符串</returns>
        public static string DataRowToJson(DataTable dt, DataRow dr)
        {
            try
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(result);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return json.TrimStart('[').TrimEnd(']');
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }

        /// <summary>
        /// 将DataRow转换为Json格式
        /// </summary>
        /// <param name="dt">要转换的DataTable,只提供结构</param>
        /// <param name="dr">要转换的dr</param>
        /// <param name="dic">继续附加的dic数据</param>
        /// <returns>转换后的json格式字符串</returns>
        public static string DataRowAddListToJson(DataTable dt, DataRow dr, Dictionary<string, string> dic)
        {
            try
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }

                if (dic != null)
                {
                    foreach (KeyValuePair<string, string> d in dic)
                    {
                        result.Add(d.Key, d.Value);
                    }
                }
                list.Add(result);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return json.TrimStart('[').TrimEnd(']');
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }


        /// <summary>
        /// 将DataRow转换为Json格式
        /// </summary>
        /// <param name="dt">要转换的DataTable,只提供结构</param>
        /// <param name="dr">要转换的dr</param>
        /// <param name="unClumns">所有列中要去除的列</param>
        /// <returns>转换后的json格式字符串</returns>
        public static string DataRowUnClumnToJson(DataTable dt, DataRow dr, string[] unClumns)
        {
            try
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (!unClumns.Contains(dc.ColumnName))
                    {
                        result.Add(dc.ColumnName, dr[dc].ToString());
                    }
                }
                list.Add(result);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return json.TrimStart('[').TrimEnd(']');
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }

        /// <summary>
        /// 将DataTable转换为Json格式
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <returns>转换后的json格式字符串</returns>
        public static string DataTableToJson(DataTable dt)
        {
            try
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        result.Add(dc.ColumnName, dr[dc].ToString());
                    }
                    list.Add(result);
                }
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return json;
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }

        /// <summary>
        /// 将DataTable转换为Json格式
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <param name="dic">继续添加的dic</param>
        /// <returns>转换后的json格式字符串</returns>
        public static string DataTableApenListToJson(DataTable dt, Dictionary<string, string> dic)
        {
            try
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        result.Add(dc.ColumnName, dr[dc].ToString());
                    }
                    list.Add(result);
                }
                if (list != null)
                {
                    list.Add(dic);
                }
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return json;
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }


        /// <summary>
        /// 将DataTable转换为Json格式
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <param name="unClounm">不包含的列</param>
        /// <returns>转换后的json格式字符串</returns>
        public static string DataTableToJsonUnClounm(DataTable dt, string[] unClounm)
        {
            try
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (!unClounm.Contains(dc.ColumnName))
                        {
                            result.Add(dc.ColumnName, dr[dc].ToString());
                        }
                    }
                    list.Add(result);
                }
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return json;
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }
        /// <summary>
        /// 将DataTable转换为Json格式
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <param name="unClounm">去除表中的某些列</param>
        /// <param name="addic"></param>
        /// <returns>转换后的json格式字符串</returns>
        public static string DataTableUnClounmAppenListToJson(DataTable dt, string[] unClounm, Dictionary<string, string> addic)
        {
            try
            {
                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (!unClounm.Contains(dc.ColumnName))
                        {
                            result.Add(dc.ColumnName, dr[dc].ToString());
                        }
                    }
                    list.Add(result);
                }
                if (list != null)
                {
                    list.Add(addic);
                }
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return json;
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }
        }

        #endregion


        /// <summary>
        /// 将json格式的字符串转换为list格式的数据
        /// </summary>
        /// <param name="jsonStr">要转换的json格式字符串</param>
        /// <returns>转换后list格式的数据</returns>
        public static List<T> JsonToList<T>(string jsonStr)
        {
            try
            {
                if (jsonStr.StartsWith("[") && jsonStr.EndsWith("]"))
                {
                    List<T> t = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(jsonStr);

                    return t;
                }
                else
                {
                    List<T> t = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>("[" + jsonStr + "]");
                    return t;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// 将json格式的字符串转换为Entity格式的数据 20160917
        /// </summary>
        /// <param name="jsonStr">要转换的json格式字符串</param>
        /// <returns>转换后Model格式的数据</returns>
        public static T JsonToEntity<T>(string jsonStr)
        {
            try
            {
                jsonStr = jsonStr.TrimStart('[').TrimEnd(']');
                T model = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonStr);
                return model;
            }
            catch (Exception ex)
            {
                return default(T);
                throw ex;
            }
        }

        #region //byte[] <=> Json

        /// <summary>
        /// 将对象序例化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializableSet(object obj)
        {
            if (obj == null) return null;
            using (MemoryStream fs = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
                formatter = null;
                return fs.ToArray();
            }
        }
        /// <summary>
        /// 将二进制数据反序例化为对象
        /// </summary>
        /// <param name="tmp"></param>
        public static object SerializableGet(byte[] tmp)
        {
            if (tmp == null) return null;
            using (MemoryStream fs = new MemoryStream(tmp))
            {
                fs.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(fs);
            }
        }
        #endregion


        #region //将未知object转为Json并展平
        /// <summary>
        /// 将未知object转为Json并展平
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fixedStr"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Json_ZhanPing(object obj, string fixedStr = "", string symbol = ".")
        {
            var result = new Dictionary<string, string>();
            if (obj != null)
            {
                var json = obj.ToJson().ToObjectFromJson<Dictionary<string, object>>();
                if (json != null)
                {
                    foreach (var key in json.Keys)
                    {
                        var fixedTop = string.Join(symbol, new List<string>() { fixedStr, key }.Where(o => !string.IsNullOrEmpty(o)));
                        var value = json[key];
                        value = value != null ? value : "null";
                        if (value is string || value is int || value is double) //如果是基础类型,则存储字符串
                        {
                            result.Add(fixedTop, value.ToString());
                        }
                        else if (value is Array) //如果是数组,则序列化存储
                        {
                            result.Add(fixedTop, value.ToJson());
                        }
                        else //如果是复杂类型,则进入递归处理
                        {
                            result.AddRange(Json_ZhanPing(value, fixedTop, symbol), false);
                        }
                    }
                }
                return result;
            }
            return null;
        }
        #endregion

    }
}

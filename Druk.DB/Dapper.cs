using Dapper;
using Druk.Common;
using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Druk.DB
{
    public static class Dapper
    {
        #region sql验证
        /// <summary>
        /// 获取防JS攻击的sql
        /// </summary>
        /// <param name="inputString">输入的sql</param>
        /// <returns></returns>
        public static string GetAntiXssSql(string inputString)
        {
            //检测下SQL语句是否有危险语句
            if (CheckSqlinJect(inputString))
                throw new Exception("sql脚本存在安全隐患，请使用参数化");

            return inputString;

            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(inputString); //过滤js的标签
        }

        /// <summary>
        /// 验证sql注入  
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool CheckSqlinJect(string sql)
        {
            var regx = new Regex(@"\-\-\s*\'");  //检测sql脚本是否 包含--'
            if (regx.IsMatch(sql))
                return true;

            return false;
        }

        #endregion

        #region 构建sql语句
        /// <summary>
        /// 构建sql语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="selectField"></param>
        /// <param name="whereParam"></param>
        /// <param name="orderBy"></param>
        /// <param name="isNoLock"></param>
        /// <param name="pageOrderBy">是否是分页的排序（分页排序不需要加 order by）</param>
        /// <returns></returns>
        private static string CreateQueryTSql(string tableName, object selectField, object whereParam = null, object orderBy = null, bool isNoLock = true, bool pageOrderBy = false)
        {
            var sql = string.Empty;

            #region 查询字段
            if (selectField != null)
                sql = $"select {CreateQueryFiedlTsql(selectField)} from  { tableName}{(isNoLock ? "(nolock)" : "")}";
            else
                sql = $"select * from  {tableName}{(isNoLock ? "(nolock)" : "")}   ";
            #endregion
            #region where 条件
            sql += CreateWhereTsql(whereParam);

            #endregion
            #region 排序字段
            sql += CreateOrderByTsql(orderBy, pageOrderBy);
            #endregion
            return sql;

        }


        /// <summary>
        /// 获取insert子句 insert into table （xxx,yyy,zzz） values (@xxx,@yyy,@zzz)
        /// </summary>
        /// <param name="type">需要转sql的对象类型</param>
        /// <param name="primaryKey">自增主键的字段名称</param>
        /// <param name="isSelectPrimaryId">是否需要查询插入成功后的自增id</param>
        /// <returns></returns>
        private static string GetInsertParamSql(Type type, string primaryKey, bool isSelectPrimaryId = true, string asName = "")
        {
            var properties = type.GetProperties();
            var fields = type.GetFields();

            var tabName = GetTableName(type);
            if (!string.IsNullOrEmpty(asName))
                tabName = asName;
            var paramSql = $"insert into {tabName} ";
            var paramString = string.Empty;
            var fieldString = string.Empty;
            var allFieldsName = new List<string>();
            primaryKey = (primaryKey ?? "id").ToLower();
            if (properties != null && properties.Length > 0)
                for (int i = 0; i < properties.Length; i++)
                    if (primaryKey != (properties[i].Name.ToLower()))
                        allFieldsName.Add(properties[i].Name);

            allFieldsName.ForEach(x => { fieldString += $"[{x}],"; paramString += $"@{x},"; });
            paramSql += string.Format("({0}) values ({1});", fieldString.TrimEnd(','), paramString.TrimEnd(','));
            if (isSelectPrimaryId)
                paramSql += "select @@identity;";
            return GetAntiXssSql(paramSql);
        }

        /// <summary>
        /// where条件过滤
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string WhereFilterSql(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            input = input.Replace("insert ", "", true, CultureInfo.DefaultThreadCurrentUICulture);
            input = input.Replace("delete ", "", true, CultureInfo.DefaultThreadCurrentUICulture);
            input = input.Replace("declare ", "", true, CultureInfo.DefaultThreadCurrentUICulture);
            input = input.Replace("exec ", "", true, CultureInfo.DefaultThreadCurrentUICulture);
            input = input.Replace("drop ", "", true, CultureInfo.DefaultThreadCurrentUICulture);
            input = input.Replace("create ", "", true, CultureInfo.DefaultThreadCurrentUICulture);
            input = input.Replace("update ", "", true, CultureInfo.DefaultThreadCurrentUICulture);

            return input;
        }

        private static string CreateWhereTsql(object whereParam)
        {
            var sql = "";
            if (whereParam != null)
            {
                if (whereParam is string)
                {
                    //需要将whereParam 替换敏感词
                    sql += $" where { WhereFilterSql(whereParam.ToString())}";
                }
                else
                {
                    sql += " where ";
                    var t = whereParam.GetType();
                    var properties = t.GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (i > 0)
                            sql += $" and  ";
                        sql += $" {properties[i].Name}=@{properties[i].Name}";
                    }
                }

            }
            return sql;
        }

        private static string CreateQueryFiedlTsql(object field)
        {
            var sql = "";
            if (field != null)
            {
                if (field is string)
                {
                    sql = field.ToString();
                }
                else
                {
                    var fieldObj = field.GetType().GetProperties();
                    for (int i = 0; i < fieldObj.Length; i++)
                    {
                        if (i > 0)
                            sql += ",";
                        sql += $" {fieldObj[i].Name} ";
                    }
                }
            }
            return sql;
        }

        private static string CreateOrderByTsql(object orderBy, bool pageOrderBy = false)
        {
            var sql = "";
            if (orderBy != null)
            {
                if (orderBy is string)
                {
                    sql += $"{(pageOrderBy ? "" : " order by  ")}{orderBy}";
                }
                else
                {
                    sql += $" {(pageOrderBy ? "" : " order by  ")}";
                    var t = orderBy.GetType();
                    var properties = t.GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (i > 0)
                            sql += $" , ";
                        sql += $" {properties[i].Name} {(properties[i].GetValue(orderBy).ToBool() ? "asc" : "desc")}";
                    }
                }

            }
            return sql;
        }

        #endregion

        #region 工具
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTableName(Type type)
        {
            var tableName = "t_" + type.Name;
            return tableName;
        }

        /// <summary>
        /// 私有方法
        /// </summary>
        /// <param name="whereParam"></param>
        /// <returns></returns>
        private static object CheckWhereParam(this object whereParam)
        {
            var param = whereParam;
            if (param != null)
            {
                if (whereParam is string)
                    param = null;

            }
            return param;
        }

        /// <summary>
        /// 返回安全的entity对象
        /// </summary>
        /// <typeparam name="T">entity对象的类型</typeparam>
        /// <param name="model">entity对象</param>
        /// <returns>安全的entity对象</returns>
        private static object ReturnSecurityObject(object model)
        {
            Type t = model.GetType();//获取类型
            if (t.IsGenericType)
            {
                #region 泛型反射
                var count = Convert.ToInt32(t.GetProperty("Count").GetValue(model, null));
                for (int i = 0; i < count; i++)
                {
                    object listItem = t.GetProperty("Item").GetValue(model, new object[] { i });

                    Type item = listItem.GetType();
                    foreach (PropertyInfo propertyInfo in item.GetProperties())//遍历该类型下所有属性
                    {
                        if (propertyInfo.PropertyType == "".GetType())//如果属性为string类型
                        {
                            var inputString = (propertyInfo.GetValue(listItem) ?? "").ToString();
                            var sx = GetAntiXssSql(inputString);//进行字符串过滤

                            propertyInfo.SetValue(listItem, sx);//将过滤后的值设置给传入的对象
                        }
                        else if (propertyInfo.PropertyType == DateTime.Now.GetType() && propertyInfo.Name.ToLower() == "updatetime")
                        {
                            propertyInfo.SetValue(listItem, DateTime.Now);//以防万一给updatetime赋值
                        }
                    }
                }
                #endregion 
            }
            else
            {
                #region class 反射
                foreach (PropertyInfo propertyInfo in t.GetProperties())//遍历该类型下所有属性
                {
                    if (propertyInfo.PropertyType == "".GetType())//如果属性为string类型
                    {
                        var inputString = (propertyInfo.GetValue(model) ?? "").ToString();
                        var sx = GetAntiXssSql(inputString);//进行字符串过滤

                        propertyInfo.SetValue(model, sx);//将过滤后的值设置给传入的对象
                    }
                    else if (propertyInfo.PropertyType == DateTime.Now.GetType() && propertyInfo.Name.ToLower() == "updatetime")
                    {
                        propertyInfo.SetValue(model, DateTime.Now);//以防万一给updatetime赋值
                    }

                }
                #endregion
            }
            return model;//返回安全对象
        }


        /// <summary>
        /// 位置主键的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="primaryKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T SetIdentify<T>(T model, string primaryKey, int? value)
        where T : class
        {
            primaryKey = primaryKey.ToLower();
            var t = typeof(T);
            var properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name.ToLower() == primaryKey)
                {
                    properties[i].SetValue(model, value);
                    break;
                }
            }
            var fields = t.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name.ToLower() == primaryKey)
                {
                    fields[i].SetValue(model, value);
                    break;
                }
            }
            return model;
        }
        #endregion

        #region select
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conStr"></param>
        /// <param name="whereParam">查询条件，支持new{id=1,status=1}对象和字段名称=value</param>
        /// <param name="selectField">要返回的字段，null为返回所有字段，返回某个字段则为：new{id=0,title="",status=0}或字段名称+逗号的形式</param>
        /// <returns></returns>
        public static T GetModel<T>(this string conStr, object selectField, object whereParam)
        {
            var querySql = GetAntiXssSql(CreateQueryTSql(GetTableName(typeof(T)), selectField, whereParam));

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var reuslt = con.Query<T>(querySql, whereParam.CheckWhereParam());
                    if (reuslt != null)
                        return reuslt.FirstOrDefault();
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = querySql,
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return default(T); ;
            }
        }


        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conStr"></param>
        /// <param name="whereParam">查询条件，支持new{id=1,status=1}对象和字段名称=value</param>
        /// <param name="selectField">要返回的字段，null为返回所有字段，返回某个字段则为：new{id=0,title="",status=0}或字段名称+逗号的形式</param>
        /// <returns></returns>
        public static async Task<T> GetModelAsync<T>(this string conStr, object selectField, object whereParam)
        {
            var querySql = GetAntiXssSql(CreateQueryTSql(GetTableName(typeof(T)), selectField, whereParam));
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var reuslt = await con.QueryAsync<T>(querySql, whereParam.CheckWhereParam());
                    if (reuslt != null)
                        return reuslt.FirstOrDefault();
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = querySql,
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return default(T); ;
            }
        }

        /// <summary>
        /// 执行sql并返回单个对象
        /// </summary>
        /// <typeparam name="T">model类型</typeparam>
        /// <param name="conStr">直接调用DapperConnection类</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns>查询到的对象，可能为空</returns>
        public static T GetModelBySql<T>(this string conStr, string sql, object param = null)
        {
            var querySql = GetAntiXssSql(sql);
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var reuslt = con.Query<T>(querySql, param.CheckWhereParam());
                    if (reuslt != null)
                        return reuslt.FirstOrDefault();
                    return default(T);


                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = querySql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return default(T);
            }
        }

        /// <summary>
        /// 执行sql并返回单个对象
        /// </summary>
        /// <typeparam name="T">model类型</typeparam>
        /// <param name="conStr">直接调用DapperConnection类</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns>查询到的对象，可能为空</returns>
        public static async Task<T> GetModelBySqlAsync<T>(this string conStr, string sql, object param = null)
        {
            var querySql = GetAntiXssSql(sql);
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var reuslt = await con.QueryFirstOrDefaultAsync<T>(querySql, param.CheckWhereParam());
                    if (reuslt != null)
                        return reuslt;
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = querySql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return default(T);
            }
        }

        #endregion

        #region insert 
        /// <summary>
        /// insert 一个实体，返回一个实体（实体包含插入成功的自增id）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">要插入的对象</param>
        /// <param name="primaryKey">不需要插入的字段，主要是针对自增主键</param>
        /// <param name="isFilter">sql语句安全过滤</param>
        /// <returns></returns>
        public static T AddModel<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true)
        where T : class
        {
            var insertParameterSql = string.Empty;
            try
            {
                insertParameterSql = GetInsertParamSql(typeof(T), primaryKey, (primaryKey == "" ? false : true));
                if (isFilter)
                    model = ReturnSecurityObject(model) as T;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var identify = con.Query<int>(insertParameterSql, model).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(primaryKey))
                        model = SetIdentify(model, primaryKey, identify);
                    return model;
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = insertParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return default(T); ;
            }
        }
         
        /// <summary>
        /// insert 一个实体，返回一个实体（实体包含插入成功的自增id）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">要插入的对象</param>
        /// <param name="primaryKey">不需要插入的字段，主要是针对自增主键</param>
        /// <param name="isFilter">sql语句安全过滤</param>
        /// <returns></returns>
        public static async Task<T> AddModelAsync<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true)
        where T : class
        {
            var insertParameterSql = string.Empty;
            try
            {
                insertParameterSql = GetInsertParamSql(typeof(T), primaryKey, (primaryKey == "" ? false : true));
                if (isFilter)
                    model = ReturnSecurityObject(model) as T;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var result = await con.QueryAsync<int>(insertParameterSql, model);
                    var identify = result.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(primaryKey))
                        model = SetIdentify(model, primaryKey, identify);
                    return model;
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = insertParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return default(T); ;
            }
        }
         
        /// <summary>
        /// insert 一个实体，返回受影响行数
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">Entity对象</param>
        /// <param name="primaryKey">不需要插入的字段，主要是针对自增主键</param>
        /// <param name="isFilter">sql语句安全过滤</param>
        /// <returns></returns>
        public static int AddModelRowCount<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true, string asName = "")
        where T : class
        {
            var insertParameterSql = string.Empty;
            try
            {
                insertParameterSql = GetInsertParamSql(typeof(T), primaryKey, false, asName);
                if (isFilter)
                    model = ReturnSecurityObject(model) as T;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var identify = con.Execute(insertParameterSql, model);
                    return identify;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = insertParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// insert 一个实体，返回受影响行数
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">Entity对象</param>
        /// <param name="primaryKey">不需要插入的字段，主要是针对自增主键</param>
        /// <param name="isFilter">sql语句安全过滤</param>
        /// <returns></returns>
        public static async Task<int> AddModelRowCountAsync<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true, string asName = "")
        where T : class
        {
            var insertParameterSql = string.Empty;
            try
            {
                insertParameterSql = GetInsertParamSql(typeof(T), primaryKey, false, asName);
                if (isFilter)
                    model = ReturnSecurityObject(model) as T;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var identify = await con.ExecuteAsync(insertParameterSql, model);
                    return identify;
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = insertParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// insert 一个实体，返回插入后的Id
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">Entity对象</param>
        /// <param name="primaryKey">不需要插入的字段，主要是针对自增主键</param>
        /// <param name="isFilter">sql语句安全过滤</param>
        /// <returns></returns>
        public static int AddModelIdentity<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true)
        where T : class
        {
            var insertParameterSql = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    insertParameterSql = GetInsertParamSql(typeof(T), primaryKey);
                    if (isFilter)
                        model = ReturnSecurityObject(model) as T;

                    var identify = 0;
                    var reuslt = con.Query<int>(insertParameterSql, model);
                    if (reuslt != null)
                        identify = reuslt.FirstOrDefault();
                    return identify;
                }


            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = insertParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// insert 一个实体，返回插入后的Id
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">Entity对象</param>
        /// <param name="primaryKey">不需要插入的字段，主要是针对自增主键</param>
        /// <param name="isFilter">sql语句安全过滤</param>
        /// <returns></returns>
        public static async Task<int> AddModelIdentityAsync<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true)
        where T : class
        {
            var insertParameterSql = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    insertParameterSql = GetInsertParamSql(typeof(T), primaryKey);
                    if (isFilter)
                        model = ReturnSecurityObject(model) as T;
                    var reuslt = await con.ExecuteScalarAsync<int>(insertParameterSql, model);
                    return reuslt;
                }


            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = insertParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// 批量插入多个model对象，注：如数据量千条以上，建议使用数据库的自定义表类型来批量插入
        /// </summary>
        /// <param name="conStr">数据库连接字符串</param>
        /// <param name="primaryKey">生成sql的对象的对象</param>
        /// <param name="listModel">待插入数据库的对象</param>
        /// <param name="isFilter">是否安全过滤sql语句</param>
        /// <param name="asName">表名</param>
        /// <returns></returns>
        public static int AddBatchModelRowCount<T>(this string conStr, List<T> listModel, string primaryKey = "id", bool isFilter = true, string asName = "")
        where T : class, new()
        {
            var insertParameterSql = string.Empty;
            try
            {
                insertParameterSql = GetInsertParamSql(typeof(T), primaryKey, false, asName);
                if (isFilter)
                    listModel = ReturnSecurityObject(listModel) as List<T>;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var identify = con.Execute(insertParameterSql, listModel);
                    return identify;
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = insertParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// 批量插入多个model对象，注：如数据量千条以上，建议使用数据库的自定义表类型来批量插入
        /// </summary>
        /// <param name="conStr">数据库连接字符串</param>
        /// <param name="primaryKey">生成sql的对象的对象</param>
        /// <param name="listModel">待插入数据库的对象</param>
        /// <param name="isFilter">是否安全过滤sql语句</param>
        /// <param name="asName">表名</param>
        /// <returns></returns>
        public static async Task<int> AddBatchModelRowCountAsync<T>(this string conStr, List<T> listModel, string primaryKey = "id", bool isFilter = true, string asName = "")
       where T : class, new()
        {
            var insertParameterSql = string.Empty;
            try
            {
                insertParameterSql = GetInsertParamSql(typeof(T), primaryKey, false, asName);
                if (isFilter)
                    listModel = ReturnSecurityObject(listModel) as List<T>;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var identify = await con.ExecuteAsync(insertParameterSql, listModel);
                    return identify;
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = insertParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>  
        /// 使用SqlBulkCopy将DataTable中的数据批量插入数据库中  
        /// </summary>  
        /// <param name="conStr">数据库链接</param>  
        /// <param name="strTableName">数据库中对应的表名</param>  
        /// <param name="dtData">数据集</param>
        /// <param name="transaction"></param>  
        public static int AddBatchCopy(this string conStr, string strTableName, DataTable dtData, SqlTransaction transaction = null)
        {
            SqlBulkCopy sqlRevdBulkCopy = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    sqlRevdBulkCopy = transaction == null ?
                new SqlBulkCopy(conn) :
                new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, (SqlTransaction)transaction);
                    sqlRevdBulkCopy.BulkCopyTimeout = 8000;

                    if (conn.State == ConnectionState.Closed) { conn.Open(); }
                    sqlRevdBulkCopy.DestinationTableName = strTableName;//数据库中对应的表名
                    sqlRevdBulkCopy.NotifyAfter = dtData.Rows.Count;//有几行数据
                    sqlRevdBulkCopy.WriteToServer(dtData);//数据导入数据库
                    return dtData.Rows.Count;
                }
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = strTableName,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return -1;
            }
            finally
            {
                if (sqlRevdBulkCopy != null)
                    sqlRevdBulkCopy.Close();

            }

        }
        #endregion
    }
}

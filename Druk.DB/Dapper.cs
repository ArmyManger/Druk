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

        #region 构建查询sql语句
        /// <summary>
        /// 获取记录总数SQL语句
        /// </summary>
        /// <param name="_safeSql">SQL查询语句</param>
        /// <returns>记录总数SQL语句</returns>
        private static string CreateCountingSql(string _safeSql)
        {
            return string.Format(" SELECT COUNT(1) AS RecordCount FROM ({0}) AS T ", _safeSql);
        }
        private static string CreatePagingSql(int _pageSize, int _pageIndex, string _safeSql, string _orderField)
        {
            //拼接SQL字符串，加上ROW_NUMBER函数进行分页
            StringBuilder newSafeSql = new StringBuilder();
            newSafeSql.AppendFormat("SELECT ROW_NUMBER() OVER(ORDER BY {0}) as row_number,", _orderField);
            newSafeSql.Append(_safeSql.Substring(_safeSql.ToUpper().IndexOf("SELECT") + 6));

            //拼接成最终的SQL语句
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("SELECT * FROM (");
            sbSql.Append(newSafeSql.ToString());
            sbSql.Append(") AS T");
            sbSql.AppendFormat(" WHERE row_number between {0} and {1}", ((_pageIndex - 1) * _pageSize) + 1, _pageIndex * _pageSize);

            return sbSql.ToString() + " order by  row_number asc";
        }

        /// <summary>
        /// 获取分页SQL语句，默认row_number为关健字，所有表不允许使用该字段名
        /// </summary>
        /// <param name="_recordCount">记录总数</param>
        /// <param name="_pageSize">每页记录数</param>
        /// <param name="_pageIndex">当前页数</param>
        /// <param name="_safeSql">SQL查询语句</param>
        /// <param name="_orderField">排序字段，多个则用“,”隔开</param>
        /// <returns>分页SQL语句</returns>
        private static string CreatePagingSql(int _recordCount, int _pageSize, int _pageIndex, string _safeSql, string _orderField)
        {
            //计算总页数
            _pageSize = _pageSize == 0 ? _recordCount : _pageSize;
            int pageCount = (_recordCount + _pageSize - 1) / _pageSize;

            //检查当前页数
            if (_pageIndex < 1)
            {
                _pageIndex = 1;
            }
            else if (_pageIndex > pageCount)
            {
                _pageIndex = pageCount;
            }
            //拼接SQL字符串，加上ROW_NUMBER函数进行分页
            StringBuilder newSafeSql = new StringBuilder();
            newSafeSql.AppendFormat("SELECT ROW_NUMBER() OVER(ORDER BY {0}) as row_number,", _orderField);
            newSafeSql.Append(_safeSql.Substring(_safeSql.ToUpper().IndexOf("SELECT") + 6));

            //拼接成最终的SQL语句
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("SELECT * FROM (");
            sbSql.Append(newSafeSql.ToString());
            sbSql.Append(") AS T");
            sbSql.AppendFormat(" WHERE row_number between {0} and {1}", ((_pageIndex - 1) * _pageSize) + 1, _pageIndex * _pageSize);

            return sbSql.ToString() + " order by  row_number asc";
        }

        #endregion

        #region 构建更新sql语句
        /// <summary>
        /// 获取更新sql
        /// </summary>
        /// <param name="type">需要更新的类型</param>
        /// <param name="primaryKey">需要更新的主键名</param>
        /// <returns></returns>
        public static string GetUpdateParamSql(Type type, string primaryKey, string asName = "")
        {
            var tableName = GetTableName(type);
            if (!string.IsNullOrEmpty(asName))
                tableName = asName;
            var properties = type.GetProperties();
            var fields = type.GetFields();
            var paramSql = $"update {tableName} set ";
            primaryKey = (primaryKey ?? "").ToLower();
            if (properties != null && properties.Length > 0)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    var name = properties[i].Name;
                    if (primaryKey != (name.ToLower()))
                        paramSql += "[" + name + "]=@" + name + ",";
                }
            }
            return paramSql.TrimEnd(',') + string.Format(" where {0}=@{0}", primaryKey);
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

        #region select list

        /// <summary>
        /// 异步获取分页结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conStr"></param>
        /// <param name="sql"></param>
        /// <param name="orderby"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <param name="param"></param>
        /// <param name="countSql"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<PagedResult<T>> GetListPagedAsync<T>(this string conStr, string sql, string orderby, int pagesize, int pageindex, object param = null, string countSql = null, CommandType commandType = CommandType.Text)
        {
            Config.CheckIsOnlyReadDB(conStr);
            var pagingSql = string.Empty;
            var whereParam = param.CheckWhereParam();
            try
            {
                var safeSql = GetAntiXssSql(sql);
                countSql ??= CreateCountingSql(safeSql);
                pagingSql = CreatePagingSql(pagesize, pageindex, safeSql, orderby);
                using SqlConnection con = new SqlConnection(conStr);
                var reader = await con.QueryMultipleAsync($"{countSql};{pagingSql}", whereParam);
                var totalCount = await reader.ReadSingleOrDefaultAsync<int>();
                var items = await reader.ReadAsync<T>();
                return items.ToPagedResult(totalCount, pageindex, pagesize);
            }
            catch (Exception ex)
            {
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = pagingSql,
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return null;
            }
        }


        /// <summary>
        /// 查询分页数据列表
        /// </summary>
        /// <typeparam name="T">查询对象</typeparam>
        /// <param name="con">数据库链接</param>
        /// <param name="fieldParam">查询字段,查询全部为null,查询某个字段，如：new {name="",id=0,status=0}</param>
        /// <param name="whereField">查询条件, sql语句,不包含where</param>
        /// <param name="whereParam">查询条件参数值，如：new {name="danny",status=1}</param>
        /// <param name="orderBy">排序字段 ,正序为true，降序为false，如：new {sort_id=false,id=true}</param>
        /// <param name="pagesize">每页条数</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="totalCount">返回总条数</param>
        /// <returns></returns>
        public static List<T> GetListPaged<T>(this string conStr, object fieldParam, string whereField, object whereParam, object orderBy, int pagesize, int pageindex, out int totalCount)
        {
            Config.CheckIsOnlyReadDB(conStr);
            var safeSql = string.Empty;
            var pagingSql = string.Empty;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(CreateQueryTSql(GetTableName(typeof(T)), fieldParam));

                #region 查询条件
                if (!string.IsNullOrEmpty(whereField.Trim()))
                    sql.Append($" where {whereField.ToString()}");
                #endregion

                var order = CreateOrderByTsql(orderBy, true);

                safeSql = GetAntiXssSql(sql.ToString());
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    totalCount = con.Query<int>(CreateCountingSql(safeSql), whereParam.CheckWhereParam()).First();
                    pagingSql = CreatePagingSql(totalCount, pagesize, pageindex, safeSql, order);
                    return con.Query<T>(pagingSql, whereParam.CheckWhereParam()).ToList();
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = pagingSql,
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                totalCount = 0;
                return default(List<T>);// new List<T>();
            }
        }



        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T">查询对象</typeparam>
        /// <param name="conStr">数据库链接</param>
        /// <param name="fieldParam">查询字段,查询全部为null,查询某个字段，如：new {name="",id=0,status=0}</param>
        /// <param name="whereParam">查询条件，如：new {name="danny",status=1}</param>
        /// <param name="orderBy">排序字段 ,正序为true，降序为false，如：new {sort_id=false,id=true}</param>
        /// <returns></returns>
        public static List<T> GetList<T>(this string conStr, object fieldParam, object whereParam = null, object orderBy = null)
        {
            Config.CheckIsOnlyReadDB(conStr);
            var safeSql = string.Empty;
            try
            {
                safeSql = GetAntiXssSql(CreateQueryTSql(GetTableName(typeof(T)), fieldParam, whereParam, orderBy));
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    return con.Query<T>(safeSql, whereParam.CheckWhereParam()).ToList();
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = safeSql,
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return null;
            }

        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T">查询对象</typeparam>
        /// <param name="con">数据库链接</param>
        /// <param name="fieldParam">查询字段,查询全部为null,查询某个字段，如：new {name="",id=0,status=0}</param>
        /// <param name="whereParam">查询条件，如：new {name="danny",status=1}</param>
        /// <param name="orderBy">排序字段 ,正序为true，降序为false，如：new {sort_id=false,id=true}</param>
        /// <returns></returns>
        public static async Task<List<T>> GetListAsync<T>(this string conStr, object fieldParam, object whereParam = null, object orderBy = null)
        {
            Config.CheckIsOnlyReadDB(conStr);
            var safeSql = string.Empty;
            try
            {
                safeSql = GetAntiXssSql(CreateQueryTSql(GetTableName(typeof(T)), fieldParam, whereParam, orderBy));
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var result = await con.QueryAsync<T>(safeSql, whereParam.CheckWhereParam());
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = safeSql,
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return null;
            }

        }



        /// <summary>
        ///获取数据列表
        /// </summary>
        /// <typeparam name="T">获取的列表类型</typeparam>
        /// <param name="conStr"></param>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">sql语句含orderby</param>
        /// <param name="param">参数</param>
        /// <param name="isFilter"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static List<T> GetListBySql<T>(this string conStr, string sql, object param = null, bool isFilter = true, CommandType commandType = CommandType.Text)
        {
            Config.CheckIsOnlyReadDB(conStr);
            var safeSql = sql;
            try
            {
                if (isFilter)
                    safeSql = GetAntiXssSql(sql);

                using (SqlConnection con = new SqlConnection(conStr))
                {
                    return con.Query<T>(safeSql, param.CheckWhereParam(), commandType: commandType).ToList();
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = safeSql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return null;  //不要返回new list
            }
        }

        /// <summary>
        ///获取数据列表
        /// </summary>
        /// <typeparam name="T">获取的列表类型</typeparam>
        /// <param name="conStr"></param>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">sql语句含orderby</param>
        /// <param name="param">参数</param>
        /// <param name="isFilter"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async Task<List<T>> GetListBySqlAsync<T>(this string conStr, string sql, object param = null, bool isFilter = true, CommandType commandType = CommandType.Text)
        {
            Config.CheckIsOnlyReadDB(conStr);
            var safeSql = sql;
            try
            {
                if (isFilter)
                    safeSql = GetAntiXssSql(sql);

                using (SqlConnection con = new SqlConnection(conStr))
                {
                    return (await con.QueryAsync<T>(safeSql, param.CheckWhereParam(), commandType: commandType)).ToList();
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = safeSql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return null;
            }
        }

        /// <summary>
        /// Dapper获取分页列表
        /// </summary>
        /// <typeparam name="T">获取的列表类型</typeparam>
        /// <param name="conStr"></param>
        /// <param name="sql">sql语句（不包含orderby以外的部分）</param>
        /// <param name="orderby">orderby的字段，如果多个可用,分隔，逆序可用desc</param>
        /// <param name="pagesize">页大小</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="param"></param>
        /// <param name="sqlCount"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static List<T> GetListBySql<T>(this string conStr, string sql, string orderby, int pagesize, int pageindex, out int totalCount, object param = null, string sqlCount = null,
            CommandType commandType = CommandType.Text)
        {
            Config.CheckIsOnlyReadDB(conStr);
            var safeSql = string.Empty;
            try
            {
                safeSql = GetAntiXssSql(sql);
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    if (sqlCount == null)
                    {
                        var tmpStr = CreateCountingSql(safeSql);
                        totalCount = con.Query<int>(tmpStr, param.CheckWhereParam()).First();
                    }
                    else
                        totalCount = con.Query<int>(sqlCount, param.CheckWhereParam()).First();
                    var pagingSql = CreatePagingSql(totalCount, pagesize, pageindex, safeSql, orderby);
                    return con.Query<T>(pagingSql, param.CheckWhereParam(), commandType: commandType).ToList();
                }
            }
            catch (Exception ex)
            {
                totalCount = 0;
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = safeSql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return null;
            }
        }


        #endregion

        #region select count
        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="whereParam">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static int GetCount<T>(this string conStr, object whereParam) where T : class, new()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                var properties = typeof(T).GetProperties();
                sql.Append($"select count(*) from {GetTableName(typeof(T))}");
                if (whereParam != null)
                {
                    if (whereParam is string)
                    {
                        sql.Append($" where { WhereFilterSql(whereParam.ToString())}");
                    }
                    else
                    {
                        sql.Append(" where ");
                        var whereField = whereParam.GetType().GetProperties();
                        for (int i = 0; i < whereField.Length; i++)
                        {
                            if (i > 0)
                                sql.Append(" and  ");
                            sql.Append($" {whereField[i].Name}=@{whereField[i].Name}");
                        }
                    }
                }
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int count = con.Query<int>(sql.ToString(), whereParam.CheckWhereParam()).FirstOrDefault();
                    return count;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql.ToString(),
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="whereParam">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static async Task<int> GetCountAsync<T>(this string conStr, object whereParam) where T : class, new()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                var properties = typeof(T).GetProperties();
                sql.Append($"select count(*) from {GetTableName(typeof(T))}");
                if (whereParam != null)
                {
                    if (whereParam is string)
                    {
                        sql.Append($" where { WhereFilterSql(whereParam.ToString())}");
                    }
                    else
                    {
                        sql.Append(" where ");
                        var whereField = whereParam.GetType().GetProperties();
                        for (int i = 0; i < whereField.Length; i++)
                        {
                            if (i > 0)
                                sql.Append(" and  ");
                            sql.Append($" {whereField[i].Name}=@{whereField[i].Name}");
                        }
                    }
                }
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var count = await con.QueryAsync<int>(sql.ToString(), whereParam.CheckWhereParam());
                    return count.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql.ToString(),
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return 0;
            }
        }


        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="sql"></param>
        /// <param name="whereParam">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>

        /// <returns></returns>
        public static int GetCount(this string conStr, string sql, object whereParam = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int count = con.Query<int>(sql.ToString(), whereParam).FirstOrDefault();
                    return count;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql.ToString(),
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="sql"></param>
        /// <param name="whereParam">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static async Task<int> GetCountAsync(this string conStr, string sql, object whereParam = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var count = await con.QueryAsync<int>(GetAntiXssSql(sql), whereParam);
                    return count.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql.ToString(),
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return 0;
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

        #region update
        /// <summary>
        /// update一个实体,实体是先查询出来的
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">需要更新的Entity对象</param>
        /// <param name="primaryKey">不需要更新的字段，主要是针对自增主键</param>
        /// <param name="isFilter"></param>
        /// <param name="asName"></param>
        /// <returns></returns>
        public static bool UpdateModel<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true, string asName = "") where T : class
        {
            var updateParameterSql = GetUpdateParamSql(typeof(T), primaryKey, asName);
            try
            {
                if (isFilter)
                    model = ReturnSecurityObject(model) as T;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    return con.Execute(updateParameterSql, model) > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = updateParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return false;
            }
        }

        /// <summary>
        /// update一个实体,实体是先查询出来的
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="model">需要更新的Entity对象</param>
        /// <param name="primaryKey">不需要更新的字段，主要是针对自增主键</param>
        /// <param name="isFilter"></param>
        /// <param name="asName"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateModelAsync<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true, string asName = "") where T : class
        {
            var updateParameterSql = GetUpdateParamSql(typeof(T), primaryKey, asName);
            try
            {
                if (isFilter)
                    model = ReturnSecurityObject(model) as T;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    return (await con.ExecuteAsync(updateParameterSql, model)) > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = updateParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return false;
            }
        }


        /// <summary>
        /// 修改一个实体，返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conStr"></param>
        /// <param name="model"></param>
        /// <param name="primaryKey"></param>
        /// <param name="isFilter"></param>
        /// <param name="asName"></param>
        /// <returns></returns>
        public static T UpdateModelEntity<T>(this string conStr, T model, string primaryKey = "id", bool isFilter = true, string asName = "") where T : class
        {
            var updateParameterSql = string.Empty;
            try
            {
                updateParameterSql = GetUpdateParamSql(typeof(T), primaryKey, asName);
                if (isFilter)
                    model = ReturnSecurityObject(model) as T;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var identify = con.Query<int>(updateParameterSql, model).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(primaryKey))
                        model = SetIdentify(model, primaryKey, identify);
                    return model;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = updateParameterSql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return default(T); ;
            }
        }

        /// <summary>
        /// 批量更新多个model对象，注：如数据量千条以上，建议使用数据库的自定义表类型来批量更新
        /// </summary>
        /// <typeparam name="T">更新对象的类型</typeparam>
        /// <param name="conStr">链接字符串</param>
        /// <param name="listModel">更新的list</param>
        /// <param name="primaryKey">主键id</param>
        /// <param name="isFilter"></param>
        /// <returns></returns>
        public static int UpdateBatchModelRowCount<T>(this string conStr, List<T> listModel, string primaryKey = "id", bool isFilter = true, string asName = "")
        where T : class, new()
        {
            var sql = string.Empty;
            try
            {
                sql = GetUpdateParamSql(typeof(T), primaryKey, asName);
                if (isFilter)
                    listModel = ReturnSecurityObject(listModel) as List<T>;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var identify = con.Execute(sql, listModel);
                    return identify;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return 0;
            }
        }


        /// <summary>
        /// 批量更新多个model对象，注：如数据量千条以上，建议使用数据库的自定义表类型来批量更新
        /// </summary>
        /// <typeparam name="T">更新对象的类型</typeparam>
        /// <param name="conStr">链接字符串</param>
        /// <param name="listModel">更新的list</param>
        /// <param name="primaryKey">主键id</param>
        /// <param name="isFilter"></param>
        /// <returns></returns>
        public static async Task<int> UpdateBatchModelRowCountAsync<T>(this string conStr, List<T> listModel, string primaryKey = "id", bool isFilter = true, string asName = "")
        where T : class, new()
        {
            var sql = string.Empty;
            try
            {
                sql = GetUpdateParamSql(typeof(T), primaryKey, asName);
                if (isFilter)
                    listModel = ReturnSecurityObject(listModel) as List<T>;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    var identify = await con.ExecuteAsync(sql, listModel);
                    return identify;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = null,
                    Error = ex
                });
                return 0;
            }
        }



        /// <summary>
        /// 更新对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="conStr"></param>
        /// <param name="updateParam">set字段</param>
        /// <param name="whereParam">where字段</param>
        /// <param name="isFilter"></param>
        /// <returns></returns>
        public static bool UpdateModel<T>(this string conStr, object updateParam, object whereParam, bool isFilter = true) where T : class, new()
        {
            T model = new T();
            StringBuilder sql = new StringBuilder();
            var t = typeof(T);
            var properties = t.GetProperties();
            sql.Append($"update {GetTableName(typeof(T))} set ");

            #region 更新字段
            if (updateParam != null)
            {
                if (updateParam is string)
                {
                    sql.Append(updateParam.ToString());
                }
                else
                {
                    var fieldObj = updateParam.GetType().GetProperties();
                    int i = 0;
                    foreach (var pi in fieldObj)
                    {
                        if (i > 0)
                            sql.Append(",");
                        sql.Append($" {pi.Name}=@{pi.Name} ");
                        #region 处理参数
                        var info = properties.Where(x => x.Name.ToLower() == pi.Name.ToLower()).FirstOrDefault();
                        if (info != null)
                        {
                            if (info.PropertyType == "".GetType() && isFilter)//如果属性为string类型
                            {
                                var inputString = (pi.GetValue(updateParam) ?? "").ToString();
                                info.SetValue(model, GetAntiXssSql(inputString));//将过滤后的值设置给传入的对象
                            }
                            else
                            {
                                info.SetValue(model, pi.GetValue(updateParam));
                            }
                        }
                        #endregion
                        i++;
                    }
                }
            }
            #endregion

            #region where条件
            if (whereParam != null)
            {
                sql.Append($" where ");
                if (whereParam is string)
                {
                    sql.Append($"   {whereParam}");
                }
                else
                {
                    var whereField = whereParam.GetType().GetProperties();
                    int i = 0;
                    foreach (var pi in whereField)
                    {
                        if (i > 0)
                            sql.Append(" and ");
                        sql.Append($" {pi.Name}=@{pi.Name} ");

                        var info = properties.Where(x => x.Name.ToLower() == pi.Name.ToLower()).FirstOrDefault();
                        if (info != null)
                        {
                            if (info.PropertyType == "".GetType() && isFilter)//如果属性为string类型
                            {
                                var inputString = (pi.GetValue(whereParam) ?? "").ToString();
                                info.SetValue(model, GetAntiXssSql(inputString));//将过滤后的值设置给传入的对象
                            }
                            else
                            {
                                info.SetValue(model, pi.GetValue(whereParam));
                            }
                        }
                        i++;
                    }
                }
            }

            #endregion
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = con.Execute(sql.ToString(), model);
                    return num > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql.ToString(),
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return false;
            }
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="conStr"></param>
        /// <param name="updateParam">set字段</param>
        /// <param name="whereParam">where字段</param>
        /// <param name="isFilter"></param>
        public static async Task<bool> UpdateModelAsync<T>(this string conStr, object updateParam, object whereParam, bool isFilter = true) where T : class, new()
        {
            T model = new T();
            StringBuilder sql = new StringBuilder();
            var t = typeof(T);
            var properties = t.GetProperties();
            sql.Append($"update {GetTableName(typeof(T))} set ");

            #region 更新字段
            if (updateParam != null)
            {
                if (updateParam is string)
                {
                    sql.Append(updateParam.ToString());
                }
                else
                {
                    var fieldObj = updateParam.GetType().GetProperties();
                    int i = 0;
                    foreach (var pi in fieldObj)
                    {
                        if (i > 0)
                            sql.Append(",");
                        sql.Append($" {pi.Name}=@{pi.Name} ");
                        #region 处理参数
                        var info = properties.Where(x => x.Name.ToLower() == pi.Name.ToLower()).FirstOrDefault();
                        if (info != null)
                        {
                            if (info.PropertyType == "".GetType() && isFilter)//如果属性为string类型
                            {
                                var inputString = (pi.GetValue(updateParam) ?? "").ToString();
                                info.SetValue(model, GetAntiXssSql(inputString));//将过滤后的值设置给传入的对象
                            }
                            else
                            {
                                info.SetValue(model, pi.GetValue(updateParam));
                            }
                        }
                        #endregion
                        i++;
                    }
                }
            }
            #endregion

            #region where条件
            if (whereParam != null)
            {
                sql.Append($" where ");
                if (whereParam is string)
                {
                    sql.Append($"   {whereParam}");
                }
                else
                {
                    var whereField = whereParam.GetType().GetProperties();
                    int i = 0;
                    foreach (var pi in whereField)
                    {
                        if (i > 0)
                            sql.Append(" and ");
                        sql.Append($" {pi.Name}=@{pi.Name} ");

                        var info = properties.Where(x => x.Name.ToLower() == pi.Name.ToLower()).FirstOrDefault();
                        if (info != null)
                        {
                            if (info.PropertyType == "".GetType() && isFilter)//如果属性为string类型
                            {
                                var inputString = (pi.GetValue(whereParam) ?? "").ToString();
                                info.SetValue(model, GetAntiXssSql(inputString));//将过滤后的值设置给传入的对象
                            }
                            else
                            {
                                info.SetValue(model, pi.GetValue(whereParam));
                            }
                        }
                        i++;
                    }
                }
            }

            #endregion
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = await con.ExecuteAsync(sql.ToString(), model);
                    return num > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql.ToString(),
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return false;
            }
        }
        #endregion

        #region delete 
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="T">要删除的对象</typeparam>
        /// <param name="con"></param>
        /// <param name="whereParam"> 参数， 假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static bool DeleteModel<T>(this string conStr, object whereParam)
        {
            var sql = $"delete {GetTableName(typeof(T))}  {CreateWhereTsql(whereParam)} ";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = con.Execute(sql, whereParam);
                    return num > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return false;
            }
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="T">要删除的对象</typeparam>
        /// <param name="con"></param>
        /// <param name="whereParam"> 参数， 假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <returns></returns>
        public static async Task<bool> DeleteModelAsync<T>(this string conStr, object whereParam)
        {
            var sql = $"delete {GetTableName(typeof(T))}  {CreateWhereTsql(whereParam)} ";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = await con.ExecuteAsync(sql, whereParam);
                    return num > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = whereParam,
                    Error = ex
                });
                return false;
            }
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="T">要删除的对象</typeparam>
        /// <param name="conStr"></param>
        /// <param name="id"> 要删除的id</param>
        /// <returns></returns>
        public static bool DeleteModel<T>(this string conStr, int id)
        {

            var sql = $"delete {GetTableName(typeof(T))}  where id={id} ";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = con.Execute(sql, null);
                    return num > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = id,
                    Error = ex
                });
                return false;
            }
        }


        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="T">要删除的对象</typeparam>
        /// <param name="conStr"></param>
        /// <param name="id"> 要删除的id</param>
        /// <returns></returns>
        public static async Task<bool> DeleteModelAsync<T>(this string conStr, int id)
        {

            var sql = $"delete {GetTableName(typeof(T))}  where id={id} ";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = await con.ExecuteAsync(sql, null);
                    return num > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = id,
                    Error = ex
                });
                return false;
            }
        }
        #endregion

        #region 执行一条sql语句(增删改)
        /// <summary>
        /// 执行一条sql语句(增删改)
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">TSQL语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <param name="saveLog"></param>
        /// <param name="commandType"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool ExecuteSql(this string conStr, string sql, object param = null, bool saveLog = true, CommandType commandType = CommandType.Text)
        {
            string filterSql = string.Empty;
            try
            {
                filterSql = GetAntiXssSql(sql);
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = con.Execute(filterSql, param.CheckWhereParam(), commandType: commandType);

                    return num > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return false;
            }
        }

        /// <summary>
        /// 执行一条sql语句(增删改)
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="con">直接调用DapperConnection类</param>
        /// <param name="sql">TSQL语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <param name="saveLog"></param>
        /// <param name="commandType"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static async ValueTask<bool> ExecuteSqlAsync(this string conStr, string sql, object param = null, bool saveLog = true, CommandType commandType = CommandType.Text)
        {
            string filterSql = string.Empty;
            try
            {
                filterSql = GetAntiXssSql(sql);
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = await con.ExecuteAsync(filterSql, param.CheckWhereParam(), commandType: commandType);
                    return num > 0;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return false;
            }
        }

        /// <summary>
        /// 执行一条sql语句(增删改),返回受影响的行数
        /// </summary>
        /// <param name="conStr">直接调用DapperConnection类</param>
        /// <param name="sql">TSQL语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <param name="saveLog"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static int ExecuteSqlReturnRowCount(this string conStr, string sql, object param = null, bool saveLog = true, CommandType commandType = CommandType.Text)
        {
            string filterSql = string.Empty;
            try
            {
                filterSql = GetAntiXssSql(sql);
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = con.Execute(filterSql, param.CheckWhereParam(), commandType: commandType);
                    return num;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// 执行一条sql语句(增删改),返回受影响的行数
        /// </summary>
        /// <param name="conStr">直接调用DapperConnection类</param>
        /// <param name="sql">TSQL语句</param>
        /// <param name="param">sql里面用的参数，假设用到@a，则传入new {a='xxx'},传入对象即可</param>
        /// <param name="saveLog"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static async ValueTask<int> ExecuteSqlReturnRowCountAsync(this string conStr, string sql, object param = null, bool saveLog = true, CommandType commandType = CommandType.Text)
        {
            string filterSql = string.Empty;
            try
            {
                filterSql = GetAntiXssSql(sql);
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    int num = await con.ExecuteAsync(filterSql, param.CheckWhereParam(), commandType: commandType);
                    return num;
                }
            }
            catch (Exception ex)
            {
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = null,
                    Sql = sql,
                    IsTrans = false,
                    Params = param,
                    Error = ex
                });
                return 0;
            }
        }
        #endregion
    }
}

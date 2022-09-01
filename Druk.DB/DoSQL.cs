using Druk.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Druk.DB
{
    /// <summary>
    ///访问SqlServer
    /// </summary>
    public static class DoSQL
    {
        #region ExecuteDT

        /// <summary>
        /// 返回查询结果表 (支持事务)
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="sql">查询语句</param>
        /// <param name="commandType">查询类型</param>
        /// <param name="paramList">参数列表</param>
        /// <param name="transaction">事务,不使用事务则不传</param>
        /// <param name="commandTimeout">查询超时时间,默认8000毫秒</param>
        /// <returns></returns>
        public static DataTable ExecuteDT(this SqlConnection conn, string sql, CommandType commandType = CommandType.Text, IDbDataParameter[] paramList = null, DbTransaction transaction = null, int commandTimeout = 8000)
        {
            var dt = new DataTable("table");
            try
            {
                //打开连接
                var dbc =
                    transaction == null
                        ? new SqlCommand(sql, conn) { CommandType = commandType }
                        : new SqlCommand(sql, conn, (SqlTransaction)transaction) { CommandType = commandType };

                //去除参数列表中无效参数
                if (paramList != null)
                {
                    Array.ForEach(paramList, par => { if (par.Value == null) { par.Value = string.Empty; } });
                    dbc.Parameters.Clear();
                    dbc.Parameters.AddRange(paramList);
                }

                dbc.CommandTimeout = commandTimeout;
                if (conn.State == ConnectionState.Closed) { conn.Open(); }

                var adapter = new SqlDataAdapter(dbc);

                // SqlDataReader sdr = dbc.ExecuteReader();
                adapter.Fill(dt);
                adapter.Dispose();

                //dt.Load(sdr);

                //sdr.Close();

                dbc.Parameters.Clear();
                dbc.Dispose();
            }
            catch (Exception ex)
            {
                //记录异常SQL日志
                dt = null;
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = conn,
                    Sql = sql,
                    IsTrans = transaction != null,
                    Params = paramList?.Select(o => new { key = o.ParameterName, value = o.Value }).ToList(),
                    Error = ex
                });
                Console.WriteLine(sql);
                Console.WriteLine(ex.ToSimple("DoSql.ExecuteDT"));
            }
            finally
            {
                if (transaction == null)
                {
                    //关闭连接
                    conn.Close();
                }
            }
            return dt;
        }

        #endregion

        #region Execute_FirstCell

        /// <summary>
        /// 返回第一行第一列 (支持事务)
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="sql"> 查询语句</param>
        /// <param name="commandType">查询类型</param>
        /// <param name="paramList">参数集合</param>
        /// <param name="transaction">事务,不使用事务则不传</param>
        /// <param name="commandTimeout">查询超时时间,默认8000毫秒</param>
        /// <returns></returns>
        public static object ExecuteFirstCell(this SqlConnection conn, string sql, CommandType commandType = CommandType.Text, IDbDataParameter[] paramList = null, DbTransaction transaction = null, int commandTimeout = 8000)
        {
            var start = DateTime.Now;
            object result;
            try
            {
                var dbc =
                    transaction == null
                        ? new SqlCommand(sql, conn) { CommandType = commandType }
                        : new SqlCommand(sql, conn, (SqlTransaction)transaction) { CommandType = commandType };


                if (paramList != null)
                {
                    //查询类型是存储过程,拼接参数
                    if (commandType == CommandType.StoredProcedure)
                    {
                        var pa = paramList.ToList();
                        pa.Add(new SqlParameter("@Result_" + DateTime.Now.Ticks, ""));
                        pa.Last().Direction = ParameterDirection.ReturnValue;
                        paramList = pa.ToArray();
                    }
                    //去除参数中无效的参数
                    Array.ForEach(paramList, par => { if (par.Value == null) { par.Value = string.Empty; } });
                    dbc.Parameters.Clear();
                    dbc.Parameters.AddRange(paramList);
                }


                dbc.CommandTimeout = commandTimeout;

                //如果数据库连接关闭则打开连接
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                result = dbc.ExecuteScalar();
                if (commandType == CommandType.StoredProcedure)
                {
                    result = paramList.Last().Value;
                }
                dbc.Parameters.Clear();
                //释放数据库资源
                dbc.Dispose();
            }
            catch (Exception ex)
            {
                result = null;
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = conn,
                    Sql = sql,
                    IsTrans = transaction != null,
                    Params = paramList?.Select(o => new { key = o.ParameterName, value = o.Value }).ToList(),
                    Error = ex
                });
                Console.WriteLine(sql);
                Console.WriteLine(ex.ToSimple("DoSql.ExecuteFirstCell"));
            }
            finally
            {
                if (transaction == null)
                {
                    //关闭数据库连接
                    conn.Close();

                }
            }
            return result;
        }

        #endregion

        #region Excute_RowCount

        /// <summary>
        /// 返回影响的行数   执行Sql语句，支持事务  
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">SQL执行类型</param>
        /// <param name="paramList">参数集合</param>
        /// <param name="transaction">事务,不使用事务则不传</param>
        /// <param name="commandTimeout">执行超时时间,默认8000毫秒</param>
        /// <returns></returns>
        public static int ExecuteRowCount(this SqlConnection conn, string sql, CommandType commandType = CommandType.Text, SqlParameter[] paramList = null, DbTransaction transaction = null, int commandTimeout = 8000)
        {
            var start = DateTime.Now;
            var result = 0;
            try
            {
                //打开数据连接
                var dbc =
                    transaction == null
                        ? new SqlCommand(sql, conn) { CommandType = commandType, CommandTimeout = 8000 }
                        : new SqlCommand(sql, conn, (SqlTransaction)transaction) { CommandType = commandType, CommandTimeout = 500 };

                //去除参数中的无效参数
                if (paramList != null)
                {
                    Array.ForEach(paramList, par => { if (par.Value == null) { par.Value = string.Empty; } });
                    dbc.Parameters.Clear();
                    dbc.Parameters.AddRange(paramList);
                }
                dbc.CommandTimeout = commandTimeout;

                //判断数据连接是否已打开
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                result = dbc.ExecuteNonQuery();
                dbc.Parameters.Clear();
                dbc.Dispose();
            }
            catch (Exception ex)
            {
                result = -1;
                //记录异常日志
                DBError_Log.SQL_Error(new Entity.DBError()
                {
                    Conn = conn,
                    Sql = sql,
                    IsTrans = transaction != null,
                    Params = paramList?.Select(o => new { key = o.ParameterName, value = o.Value }).ToList(),
                    Error = ex
                });
                Console.WriteLine(ex.ToSimple("DoSql.ExecuteRowCount"));
                Console.WriteLine(sql);
            }
            finally
            {
                if (transaction == null)
                {
                    //关闭数据连接
                    conn.Close();

                }
            }
            return result;
        }

        #endregion 
    }
}

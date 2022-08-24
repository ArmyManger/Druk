using Druk.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Druk.DB
{
    /// <summary>
    /// 记录错误SQL日志
    /// </summary>
    public static class DBError_Log
    {
        #region //Log_Error

        /// <summary>
        /// 记录SQL异常日志
        /// </summary>
        /// <param name="dbError"></param>
        public static void SQL_Error(Entity.DBError dbError)
        {
            try
            {
                if (dbError == null) { return; }
                if (dbError.Sql.Contains("Log_SQLError")) { return; }
                #region //判断表存不存在，不存在则创建,插入日志的语句参数化
                var sql = @"
if OBJECT_ID('[Log_SQLError]') Is Null 
Begin
	CREATE TABLE [dbo].[Log_SQLError](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[DataKey] [datetime] NULL,
		[DBName] [nvarchar](200) NULL,
		[ConnStr] [nvarchar](500) NULL,
		[SqlStr] [nvarchar](2000) NULL,
		[IsTrans] [bit] NULL,
		[ParamList] [text] NULL,
		[ErrMessage] [nvarchar](500) NULL,
		[ErrBody] [text] NULL,
	 CONSTRAINT [PK_Log_SQLError] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
End 
Insert Into [Log_SQLError] (DataKey, DBName, ConnStr, SqlStr, IsTrans, ParamList, ErrMessage, ErrBody) 
Values (@DataKey, @DBName, @ConnStr, @SqlStr, @IsTrans, @ParamList, @ErrMessage, @ErrBody);
";
                #endregion
                //格式化参数
                var paramList = new List<SqlParameter>() {
                    new SqlParameter ("@DataKey",DateTime.Now),
                    new SqlParameter ("@DBName",dbError.Conn?.Database??""),
                    new SqlParameter ("@ConnStr",dbError.Conn?.ConnectionString??""),
                    new SqlParameter ("@SqlStr",dbError.Sql??""),
                    new SqlParameter ("@IsTrans",dbError.IsTrans?1:0),
                    new SqlParameter ("@ParamList",(dbError.Params??"").ToJson()),
                    new SqlParameter ("@ErrMessage",dbError.Error?.Message??""),
                    new SqlParameter ("@ErrBody",dbError.Error?.ToSimple()??"")
                };

                //执行插入
                //Config.Conn_Wathet_Log.ExecuteRowCount(sql, CommandType.Text, paramList.ToArray());

                Console.WriteLine(dbError.Sql ?? "");
                Console.WriteLine(dbError.Error?.Message ?? "");
                Console.WriteLine(dbError.Error?.ToSimple() ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToSimple("插入SQL异常日志失败"));
            }
        }
        #endregion
    }
}

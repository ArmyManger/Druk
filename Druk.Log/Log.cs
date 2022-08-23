using Druk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; 
using static Druk.Common.ComEnum;

namespace Druk
{
    public partial class Log
    {
        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="obj">内容</param>
        /// <param name="queue">队列名称</param>`   
        /// <param name="suffix">过期时间</param>
        /// <param name="ttl"></param>
        public static void Info(object obj, RabbitQueue queue = RabbitQueue.日志_常规, string suffix = "", int ttl = 0)
        {
            Set(obj, queue, LogLevel.Info, suffix, ttl);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="obj">内容</param>
        /// <param name="Queue">队列名称</param>
        /// <param name="suffix">过期时间</param>
        public static void Error(object obj, RabbitQueue Queue = RabbitQueue.日志_常规, string suffix = "")
        {
            Set(obj, Queue, LogLevel.Error, suffix);
        }

        #region //分流写日志

        /// <summary>
        /// 线程槽
        /// </summary>
        static DoThread DoThread = new DoThread();

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="info">内容信息</param>
        /// <param name="queue">队列名称</param>
        /// <param name="logLevel">日志等级</param>
        /// <param name="suffix">后缀名</param>
        /// <param name="ttl">过期时间</param>
        static void Set(object info, RabbitQueue queue = RabbitQueue.日志_常规, LogLevel logLevel = LogLevel.Info, string suffix = "", int ttl = 0)
        {

            //首先输出到控制台
            if (info is string || info is int) { Console.WriteLine("[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "]  " + info); }

            string threadName = Thread.CurrentThread.Name ?? "";
            if (queue == RabbitQueue.日志_常规) { queue = (RabbitQueue)(DoThread.GetDataSlot_From_Thread("Thread_Tag") ?? "0").ToInt(); }

            //检查要怎么发送日志
            var queueConfig = (queue.GetDesc() ?? "").Split('.', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (queueConfig.Count != 3)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "]  " + info.ToJson());
                return;
            }

            switch (queueConfig.First().ToLower())
            {
                case "sql":
                    Log_Write_SqlServer(queue, logLevel, threadName, queueConfig[2] + "_" + DateTime.Now.Format_yyyyMM(false), queueConfig[1], info);
                    break;

                case "rabbitmq":
                    Log_Write_RabbitMQ(queue, suffix, threadName, logLevel, info, ttl);
                    break;

                case "log4":
                    Log_Write_Log4Net(queue, logLevel, threadName, info);
                    break; 
                default:
                    Console.WriteLine("[" + DateTime.Now.ToString("MM-dd HH:mm:ss") + "]  " + info.ToJson());
                    break;
            }

        }
        #endregion



        #region //日志写 SqlServer
        /// <summary>
        /// 日志分月表检查
        /// </summary>
        /// <param name="queue">日志类型</param>
        /// <param name="logLevel">日志等级</param>
        /// <param name="threadName">线程名称</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableType">表类型</param>
        /// <param name="body">日志表内容</param>
        static void Log_Write_SqlServer(RabbitQueue queue, LogLevel logLevel, string threadName, string tableName, string tableType, object body)
        {
            try
            {
                //数据库连接
                var conn = ""; 
                #region //如果当前没有这个表，则创建

                string sql_Insert = string.Empty;
                var paramsList = new List<System.Data.SqlClient.SqlParameter>();
                switch (tableType.ToLower())
                {
                    #region //normal 日志
                    case "normal":

                        paramsList = new List<System.Data.SqlClient.SqlParameter>()
                        {
                            new System.Data.SqlClient.SqlParameter("@DateKey",DateTime.Now.Format_yyyyMMddHHmmssfff()),
                            new System.Data.SqlClient.SqlParameter("@Program", queue.GetDesc()),
                            new System.Data.SqlClient.SqlParameter("@ThreadName", threadName),
                            new System.Data.SqlClient.SqlParameter("@LevelID", logLevel.GetHashCode()),
                            new System.Data.SqlClient.SqlParameter("@LevelName", logLevel.GetEnumName()),
                            new System.Data.SqlClient.SqlParameter("@Content", body.ToJson()),
                        };

                        sql_Insert = string.Format(@"
EXEC Pro_CreateLog_Normal {0};
INSERT INTO [dbo].[{0}]([DateKey],[Program],[ThreadName],[LevelID],[LevelName],[Content])
VALUES(@DateKey,@Program,@ThreadName,@LevelID,@LevelName,@Content)", tableName);

                        break;
                    #endregion

                    #region //PV 日志

                    case "pv":
                        var pvEntity = (Druk.Common.Entity.LogVisitPV)body;
                        if (pvEntity != null)
                        {
                            paramsList = new List<System.Data.SqlClient.SqlParameter>()
                            {
                                new System.Data.SqlClient.SqlParameter("@DateKey",DateTime.Now.Format_yyyyMMddHHmmssfff()),

                                new System.Data.SqlClient.SqlParameter("@UserID", pvEntity.UserID),
                                new System.Data.SqlClient.SqlParameter("@UserCodeNo", pvEntity.UserCodeNo),
                                new System.Data.SqlClient.SqlParameter("@UserName", pvEntity.UserName),
                                new System.Data.SqlClient.SqlParameter("@Token", pvEntity.Token),
                                new System.Data.SqlClient.SqlParameter("@ProfileToken", pvEntity.ProfileToken),
                                new System.Data.SqlClient.SqlParameter("@BeginTime", (pvEntity.BeginTime <= new DateTime(1900, 1, 1) ? new DateTime(1900, 1, 1) : pvEntity.BeginTime).Format_yyyyMMddHHmmssfff()),
                                new System.Data.SqlClient.SqlParameter("@EndTime", pvEntity.EndTime.Format_yyyyMMddHHmmssfff()),
                                new System.Data.SqlClient.SqlParameter("@PageURL", pvEntity.PageURL),
                                new System.Data.SqlClient.SqlParameter("@ParamsGet", pvEntity.ParamsGet),
                                new System.Data.SqlClient.SqlParameter("@ParamsPost", pvEntity.ParamsPost),
                                new System.Data.SqlClient.SqlParameter("@Method", pvEntity.Method),
                                new System.Data.SqlClient.SqlParameter("@IP", pvEntity.IP),
                                new System.Data.SqlClient.SqlParameter("@ResultCode", pvEntity.ResultCode),
                                new System.Data.SqlClient.SqlParameter("@ResultMessage", pvEntity.ResultMessage),
                                new System.Data.SqlClient.SqlParameter("@ResultBody", pvEntity.ResultBody),
                                new System.Data.SqlClient.SqlParameter("@HttpCodeStatus", pvEntity.HttpCodeStatus),
                                new System.Data.SqlClient.SqlParameter("@Exception", pvEntity.Exception),
                            };
                            sql_Insert = string.Format(@"
EXEC Pro_CreateLog_PV {0};
INSERT INTO [dbo].[{0}]([DateKey],[UserID],[UserCodeNo],[UserName],[Token],[ProfileToken],[BeginTime],[EndTime],[PageURL],[ParamsGet],[ParamsPost],[Method],[IP],[ResultCode],[ResultMessage],[ResultBody],[HttpCodeStatus],[Exception])
VALUES(@DateKey,@UserID,@UserCodeNo,@UserName,@Token,@ProfileToken,@BeginTime,@EndTime,@PageURL,@ParamsGet,@ParamsPost,@Method,@IP,@ResultCode,@ResultMessage,@ResultBody,@HttpCodeStatus,@Exception)", tableName);
                        }
                        break;
                    #endregion

//                    #region //Operate

//                    case "operate":

//                        var modifiyEntity = (Wathet.Common.Entity.LogEntityModifiy)body;
//                        if (modifiyEntity != null)
//                        {

//                            paramsList = new List<System.Data.SqlClient.SqlParameter>()
//                            {
//                                new System.Data.SqlClient.SqlParameter("@DateKey",DateTime.Now.Format_yyyyMMddHHmmssfff()),
//                                new System.Data.SqlClient.SqlParameter("@Program", queue.GetDesc()),
//                                new System.Data.SqlClient.SqlParameter("@ThreadName", threadName),
//                                new System.Data.SqlClient.SqlParameter("@LevelID", logLevel.GetHashCode()),
//                                new System.Data.SqlClient.SqlParameter("@LevelName", logLevel.GetEnumName()),

//                                new System.Data.SqlClient.SqlParameter("@UserID", (modifiyEntity.User as Wathet.DB.Entity.User)?.id),
//                                new System.Data.SqlClient.SqlParameter("@UserName", (modifiyEntity.User as Wathet.DB.Entity.User)?.realName),

//                                new System.Data.SqlClient.SqlParameter("@ObjID", modifiyEntity.ID),
//                                new System.Data.SqlClient.SqlParameter("@EntityType", modifiyEntity.Entity.GetHashCode()),
//                                new System.Data.SqlClient.SqlParameter("@EntityTypeName", modifiyEntity.Entity.GetDesc()),
//                                new System.Data.SqlClient.SqlParameter("@OldEntity", modifiyEntity.OldEntity.ToJson()),
//                                new System.Data.SqlClient.SqlParameter("@NewEntity", modifiyEntity.NewEntity.ToJson()),
//                                new System.Data.SqlClient.SqlParameter("@Action", modifiyEntity.Method.GetHashCode()),
//                                new System.Data.SqlClient.SqlParameter("@ActionName", modifiyEntity.Method.GetDesc()),
//                                new System.Data.SqlClient.SqlParameter("@Reason", modifiyEntity.Remark),
//                            };

//                            sql_Insert = string.Format(@"
//EXEC Pro_CreateLog_Operation {0}; 
//INSERT INTO [dbo].[{0}]([DateKey],[Program],[ThreadName],[LevelID],[LevelName],[UserID],[UserName],[ObjID],[EntityType],[EntityTypeName],[OldEntity],[NewEntity],[Action],[ActionName],[Reason])
//VALUES(@DateKey,@Program,@ThreadName,@LevelID,@LevelName,@UserID,@UserName,@ObjID,@EntityType,@EntityTypeName,@OldEntity,@NewEntity,@Action,@ActionName,@Reason)", tableName);
//                        }

//                        break;
//                        #endregion
                }

                //数据库执行
                //conn.ExecuteRowCount(sql_Insert, System.Data.CommandType.Text, paramsList.ToArray());

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToSimple());
            }
        }

        #endregion


        #region //日志写 RabbitMQ
        static void Log_Write_RabbitMQ(RabbitQueue queue, string suffix, string threadName, LogLevel logLevel, object info, int TTL = 0)
        {
            //推送至Rabbit队列,队列的发送是异步的
            RabbitMQ.Delivery.Send(
                queue,
                suffix,
                new Druk.Common.Entity.LogEntity()
                {
                    Program = queue.GetDesc(),
                    ThreadName = threadName,
                    Level = logLevel,
                    Body = info,
                    Time = DateTime.Now,
                    HandleTime = DateTime.Now,
                },
                TTL
            );
        }
        #endregion

        #region //日志写 Log4net

        static void Log_Write_Log4Net(RabbitQueue queue, LogLevel logLevel, string threadName, object info)
        {
            Log4_ConfigHelper.Init();

            switch (logLevel)
            {
                case LogLevel.Info: Log4NetUtil.LogInfo(info.ToJson()); break;
                case LogLevel.Error: Log4NetUtil.LogError(info.ToJson(), null); break;
            }
        }
        #endregion

    }
}

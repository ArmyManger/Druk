using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Druk
{
    public partial class Log
    {
        internal class Log4_ConfigHelper
        {
            internal const string RepositoryName = "NETCoreRepository";
            internal static void Init()
            {
                var repository = LogManager.CreateRepository(RepositoryName);
                XmlConfigurator.Configure(repository, new FileInfo("/log4net.config"));
            }
        }

        internal class Log4NetUtil
        {
            private static readonly ILog ErrorLog = LogManager.GetLogger(Log4_ConfigHelper.RepositoryName, "Error");

            private static readonly ILog InfoLog = LogManager.GetLogger(Log4_ConfigHelper.RepositoryName, "Info");

            public static void LogError(string throwMsg, Exception ex)
            {
                ErrorLog.Error(throwMsg);
            }

            public static void LogInfo(string msg)
            {
                InfoLog.Info(msg);
            }
        }
    }
}

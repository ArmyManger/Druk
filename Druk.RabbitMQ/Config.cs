using Druk.Common;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.RabbitMQ
{
    /// <summary>
    /// rabbitmq配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 工厂
        /// </summary>
        private static ConnectionFactory factory = null;

        #region //初始化客户端和连接
        /// <summary>
        /// 初始化客户端和连接
        /// </summary>
        internal static ConnectionFactory InitFactory()
        {
            if (factory == null)
            {
                Config config = new RabbitMQ.Config();
                factory = new ConnectionFactory()
                {
                    HostName = config.Rabbit_HostName,
                    Port = config.Rabbit_Port,
                    UserName = config.Rabbit_UserName,
                    Password = config.Rabbit_Password,

                    //发送端设置断开连接后自动启动属性，默认为断开后每隔五秒钟重试连接
                    //设置端口后自动恢复连接属性
                    AutomaticRecoveryEnabled = false,
                    //ContinuationTimeout = new TimeSpan(0, 0, 30)
                }; 
                Console.WriteLine("=================连接Rabbitmq================");
                Console.WriteLine(factory.HostName);
                Console.WriteLine(factory.UserName);
                Console.WriteLine(factory.Password);
                Console.WriteLine(factory.Port);
                Console.WriteLine(factory.AutomaticRecoveryEnabled);
                Console.WriteLine("=================连接Rabbitmq================");
            }
            return factory;
        }
        #endregion


        #region //获取系统配置--Rabbit

        static Dictionary<string, string> RabbitConfig_pri;
        /// <summary>
        /// 系统配置项
        /// </summary>
        public static Dictionary<string, string> RabbitConfig
        {
            get { return RabbitConfig_pri; }
            set { RabbitConfig_pri = value; }
        }
        #endregion


        /// <summary>
        /// 消息队列 连接地址
        /// </summary>
        public string Rabbit_HostName { get { return RabbitConfig.GetValue("RabbitMQ.Rabbit_HostName") ?? "127.0.0.1"; } }
        /// <summary>
        /// 消息队列 端口
        /// </summary>
        public int Rabbit_Port { get { return (RabbitConfig.GetValue("RabbitMQ.Rabbit_Port") ?? "5672").ToInt(); } }
        /// <summary>
        /// 消息队列 连接用户名
        /// </summary>
        public string Rabbit_UserName { get { return RabbitConfig.GetValue("RabbitMQ.Rabbit_UserName") ?? "druk"; } }
        /// <summary>
        /// 消息队列 连接密码
        /// </summary>
        public string Rabbit_Password { get { return RabbitConfig.GetValue("RabbitMQ.Rabbit_Password") ?? "druk2022"; } }
        /// <summary>
        /// 消息队列 队列名前缀
        /// </summary>
        public string Rabbit_QueueNameFixed { get { return RabbitConfig.GetValue("RabbitMQ.Rabbit_QueueFixed") ?? ""; } }
    }
}

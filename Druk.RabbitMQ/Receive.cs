using Druk.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.RabbitMQ
{
    /// <summary>
    /// RabbitMQ 队列 消费端
    /// </summary>
    public static class Receive
    {
        static Config config = new RabbitMQ.Config();
        public static void ReceiveQueue(IReceive IReceive)
        {
            var QueueName = config.Rabbit_QueueNameFixed + IReceive.QueueName.GetDesc();

            IConnection connection = Config.InitFactory().CreateConnection(IReceive.QueueName.GetDesc());


            var channel = connection.CreateModel();
            connection.AutoClose = true;
            var ok = channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null); //durable=false, 不持久化


            channel.BasicQos(0, 1, false);

            //订阅模式 (有消息到达将被自动接收) 消费者  
            //创建消费者  消费器
            var consumer = new EventingBasicConsumer(channel);
            //开始执行
            channel.BasicConsume(QueueName, false, consumer);

            //定义事件, 消费方法主体 处理消息
            consumer.Received += (sender, ea) => IReceive.DoReceive_Method(sender, ea, channel, false);

            connection.ConnectionBlocked += (sender, ea) => IReceive.ConnectionBlocked(sender, ea, IReceive);

            connection.ConnectionShutdown += (sender, ea) => IReceive.ConnectionShutdown(sender, ea, IReceive);

            connection.CallbackException += (sender, ea) => IReceive.CallbackException(sender, ea, IReceive);

            connection.ConnectionRecoveryError += (sender, ea) => IReceive.ConnectionRecoveryError(sender, ea, IReceive);

            //启动一个队列之后..暂停1秒钟..太快可能会有队列消费不上
            System.Threading.Thread.Sleep(1000);
        }
    }
}

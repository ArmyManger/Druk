using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.RabbitMQ
{
    public interface IReceive
    {
        /// <summary>
        /// 要监控的队列名
        /// </summary>
        Druk.Common.ComEnum.RabbitQueue QueueName { get; set; }

        /// <summary>
        /// 指定队列的消费方法,使用该方法来对队列中的任务进行处理
        /// 消息处理完成 必须对调用 channel.BasicAck(ea.DeliveryTag, false); 来对任务进行回执, 否则任务会依然在队列中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        /// <param name="channel"></param>
        void DoReceive_Method(object sender, BasicDeliverEventArgs ea, IModel channel, bool AutoAck = true);


        #region //当连接出现异常时

        /// <summary>
        /// 当连接出现异常时，以下方法用于连接的事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        /// <param name="IReceive"></param>
        void ConnectionBlocked(object sender, ConnectionBlockedEventArgs ea, IReceive IReceive);

        void ConnectionShutdown(object sender, ShutdownEventArgs ea, IReceive IReceive);

        void CallbackException(object sender, CallbackExceptionEventArgs ea, IReceive IReceive);

        void ConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs ea, IReceive IReceive);
        #endregion
    }
}

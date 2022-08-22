using Druk.Common;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Druk.Common.ComEnum;

namespace Druk.RabbitMQ
{
    public partial class Delivery
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        private static IConnection Connection = null;
        /// <summary>
        /// 发送消息到指定队列
        /// </summary>
        /// <param name="queueName">队列名称, 如果没有则自动创建</param>
        /// <param name="obj">消息主体,建议使用Json</param>
        /// <param name="TTL">过期时间,到时会转换到不带.TTL的队列中,在那个队列中消费处理</param>
        public static void Send(RabbitQueue queueName, string Suffix, object obj, int TTL = 0)
        {
            try
            {
                Task.Run(delegate
                {
                    Config config = new RabbitMQ.Config();
                    //从队列枚举的描述中获取队列名,并附加队列子名称
                    string QueueName_Receive = config.Rabbit_QueueNameFixed + queueName.GetDesc() + (string.IsNullOrEmpty(Suffix) ? string.Empty : "." + Suffix);

                    //判断当前消息是否要延时,如果延时则附加.TTL作为暂存队列
                    string QueueName = QueueName_Receive + (TTL == 0 ? string.Empty : ".TTL");
                    try
                    {
                        //将客户端和连接对象作为全局变量,只有在关闭了之后再重新连接,能有效的提高入站效率
                        if (Connection == null || !Connection.IsOpen) { Connection = Config.InitFactory().CreateConnection(); }

                        using (var channel = Connection.CreateModel())
                        {

                            //创建一个队列  队列名称
                            bool durable = true;      //设置队列持久化 服务器重启时，该队列依然能够存活 如果使用Exchange转发器,则需要转发器也是支持本地化的
                            bool exclusive = false;   //是否为当前连接的专用队列，在连接断开后，会自动删除该队列，生产环境中应该不会用到
                            bool autodelete = false;  //当没有任何消费者使用时，自动删除该队列, 否,其实无所谓,因为一般下次添加队列或者处理队列的时候都会先创建队列

                            #region //设置消息的基本参数 持久化等
                            //设置性能
                            var properties = channel.CreateBasicProperties();
                            //设置消息持久化
                            //需要注意的是，将消息设置为持久化并不能完全保证消息不丢失。
                            //虽然他告诉RabbitMQ将消息保存到磁盘上，但是在RabbitMQ接收到消息和将其保存到磁盘上这之间仍然有一个小的时间窗口。 
                            //RabbitMQ 可能只是将消息保存到了缓存中，并没有将其写入到磁盘上。持久化是不能够一定保证的，但是对于一个简单任务队列来说已经足够。
                            //如果需要消息队列持久化的强保证，可以使用publisher confirms
                            properties.Persistent = true;
                            //1:不持久化 2：持久化 这里指的是消息的持久化，配合channel(durable=true),queue(durable)可以实现，即使服务器宕机，消息仍然保留
                            properties.DeliveryMode = 2;
                            #endregion

                            if (TTL == 0)  //如果不需要设置过期 则直接加入队列
                            {
                                channel.QueueDeclare(QueueName_Receive, durable, exclusive, autodelete, null);
                            }
                            else
                            {
                                string ExchangeName = QueueName + ".Exchange"; //转发器名称
                                string RouteKey = QueueName + ".RouteKey"; //转向路由名称

                                //如果要实现过期.. 实现原理是将消息设置过期时间之后放在队列中..而此队列一直没有消费者会去消费.. 让消息过期..变成死信(Dead Letter)
                                //消息会根据队列上配置的转发器Exchange 来转发消息.. 消息会被推送到其他绑定了此转发器的队列上.. 那个队列有实时消费者..这样就形成了消息的延时处理..延时的时长是灵活设置的


                                //需要设置过期..要创建对应的过期结构.
                                //将消息加入到一个会过期的队列中..本方法中将原队列名后缀加上.TTL.. 实际转发消息队列为枚举获取到的队列名,方便后期消费处理
                                //可以在队列上设置过期时间,参数名:x-message-ttl 单位毫秒 在下方的键值对Dic中设置
                                //也可以在消息上设置过期时间..更下方的properties 的 Expiration 属性..单位也是毫秒 在消息上设置过期时间更加灵活.. 消息会比较多个过期时间取最小值


                                //此处需要注意的是
                                Dictionary<string, object> dic = new Dictionary<string, object>();
                                dic.Add("x-dead-letter-exchange", ExchangeName);//过期消息转发器
                                dic.Add("x-dead-letter-routing-key", RouteKey);//过期消息转向路由相匹配routingkey

                                //创建ExChange转发器
                                //设置转发器也是可以本地化  不自动删除的
                                channel.ExchangeDeclare(exchange: ExchangeName, type: "direct", durable: true, autoDelete: false, arguments: null);
                                //声明虚假队列 此队列没有消费端 会按消息上设置的时间过期
                                channel.QueueDeclare(QueueName, durable, exclusive, autodelete, dic);
                                //通过当前 Channel 绑定QueryName Exchange  和 routingKey
                                channel.QueueBind(QueueName, ExchangeName, RouteKey);

                                //声明真实的队列
                                channel.QueueDeclare(QueueName_Receive, durable, exclusive, autodelete, null);
                                channel.QueueBind(QueueName_Receive, ExchangeName, RouteKey);


                                //设置消息的过期时间
                                properties.Expiration = TTL.ToString();
                            }
                            //内容
                            channel.BasicPublish("", QueueName, false, properties, obj.ToJson().ToBytes(Encoding.UTF8));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.WriteLine("{0} - {1} - {2} - {3}", DateTime.Now.Format_yyyyMMddHHmmssfff(), QueueName, ex.Message, obj.ToJson());
                        Druk.Common.DoIOFile.Write("/logs/DeliveryRabbit_Error.log", string.Format("{0} - {1} - {2}\n", QueueName, DateTime.Now.Format_yyyyMMddHHmmssfff(), obj.ToJson()), true, "UTF-8");
                    }
                });
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }

        }
    }
}

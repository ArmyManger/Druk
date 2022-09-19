using Druk.IServices.BASE;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Druk.Handle
{
    /// <summary>
    /// 任务队列
    /// </summary>
    public class TaskQueue<T> : ITaskQueue<T>, IDisposable
    {
        /// <summary>
        /// 内置队列
        /// </summary>
        private ConcurrentDictionary<int, IList<T>> taskQueues;
        /// <summary>
        /// 分区大小
        /// </summary>
        private int PartitionSize;
        /// <summary>
        /// 默认index为0
        /// </summary>
        private int Index = 0;
        /// <summary>
        /// 默认处理偏移
        /// </summary>
        private int OffSet = 0;
        /// <summary>
        /// 内置锁
        /// </summary>
        private object Lock = new object();
        /// <summary>
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="PartitionSize">分区大小，默认分区大小为10 </param>
        public TaskQueue(int PartitionSize = 10)
        {
            taskQueues = new ConcurrentDictionary<int, IList<T>>();
            this.PartitionSize = PartitionSize;
            List<T> ts = new List<T>();
            taskQueues.AddOrUpdate(Index, ts, (k, v) => ts);
        }
        /// <summary>
        /// 增加一个对象
        /// </summary>
        /// <param name="t"></param>
        public void Add(T t)
        {
            lock (Lock)
            {
                IList<T> ts;
                if (taskQueues.TryGetValue(Index, out ts))
                {
                    if (ts.Count < PartitionSize)
                    {
                        ts.Add(t);
                        taskQueues.AddOrUpdate(Index, ts, (k, v) => ts);
                    }
                    else //超出区域范围，则新建区
                    {
                        Index++;
                        IList<T> ts1 = new List<T>();
                        ts1.Add(t);
                        taskQueues.AddOrUpdate(Index, ts1, (k, v) => ts1);
                    }
                }
                else
                {
                    IList<T> ts1 = new List<T>();
                    ts1.Add(t);
                    taskQueues.AddOrUpdate(Index, ts1, (k, v) => ts1);
                }
            }
        }
        /// <summary>
        /// 获取一个分组队列
        /// </summary>
        /// <returns></returns>
        public IList<T> GetQueue()
        {
            lock (Lock)
            {
                IList<T> ts;
                if (taskQueues.TryGetValue(OffSet, out ts))
                {
                    if (OffSet == Index)//如果直接获取一个能用的，那就新建区为新区
                    {
                        Index++;
                    }
                    return ts;
                }
                return null;
            }
        }
        /// <summary>
        /// 是否阻塞增加
        /// </summary>
        /// <returns></returns>
        public bool IsWaitAdd()
        {
            lock (Lock)
            {
                if (OffSet != Index)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 当前队列完成
        /// </summary>
        public void Complete()
        {
            lock (Lock)
            {
                IList<T> ts;
                taskQueues.TryRemove(OffSet, out ts);
                if (OffSet < Index)
                {
                    OffSet++;
                }
            }
        }

        public void Dispose()
        {
            if (taskQueues != null)
            {
                taskQueues.Clear();
                taskQueues = null;
            }
        }
    }
    class Program
    {
        static TaskQueue<string> taskQueue;
        static void Main(string[] args)
        {
            taskQueue = new TaskQueue<string>();
            #region 一个一直生产指定大小，另外一个消费  先生产，再消费
            //for (int i = 0; i < 1000; i++)
            //{
            //    taskQueue.Add(i.ToString());
            //}
            //另外一个按照指定的时间，消费
            //while (true)
            //{
            //    Thread.Sleep(2000);
            //    Console.WriteLine("获取-----开始获取到数据!");
            //    var list = taskQueue.GetQueue();
            //    if (list != null)
            //    {
            //        Console.WriteLine($"获取-----对象状态：{taskQueue.IsWaitAdd()}已获取的队列列表:{string.Join(",", list)}");
            //        Console.WriteLine("获取-----处理1秒后，提交当前!");
            //        Thread.Sleep(1000);
            //        taskQueue.Complete();
            //        Console.WriteLine("获取-----已经提交!");
            //    }
            //}
            #endregion
            #region 两个任务处理，实现 一个生产，一个消费  批量  生产，并消费
            Task.Run(() =>
            {
                for (int i = 0; i < 10000000; i++)
                {
                    taskQueue.Add(i.ToString());
                    Thread.Sleep(100);//一秒插入一条
                    Console.WriteLine($"插入-----队列状态：{taskQueue.IsWaitAdd()}");
                    while (taskQueue.IsWaitAdd())//有待处理任务
                    {
                        Console.WriteLine("插入-----任务插入中开始阻塞!");
                        SpinWait.SpinUntil(() => !taskQueue.IsWaitAdd());
                    }
                }
            });

            while (true)
            {
                Thread.Sleep(2000);
                Console.WriteLine("获取-----开始获取到数据!");
                var list = taskQueue.GetQueue();
                if (list != null)
                {
                    Console.WriteLine($"获取-----对象状态：{taskQueue.IsWaitAdd()}已获取的队列列表:{string.Join(",", list)}");
                    Console.WriteLine("获取-----处理10秒后，提交当前!");
                    Thread.Sleep(1000);
                    taskQueue.Complete();
                    Console.WriteLine("获取-----已经提交!");
                }
            }
            #endregion
            Console.WriteLine("开始获取信息!");
            Console.ReadLine();
        }
    }
}

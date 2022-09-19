using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.IServices.BASE
{
    /// <summary>
    /// 任务队列接口
    /// </summary>
    public interface ITaskQueue<T>
    {
        /// <summary>
        /// 增加一个对象
        /// </summary>
        /// <param name="t"></param>
        void Add(T t);
        /// <summary>
        /// 获取一个分组队列
        /// </summary>
        /// <returns></returns>
        IList<T> GetQueue();
        /// <summary>
        /// 是否阻塞增加
        /// </summary>
        /// <returns></returns>
        bool IsWaitAdd();
        /// <summary>
        /// 当前队列完成
        /// </summary>
        void Complete();
    }
}

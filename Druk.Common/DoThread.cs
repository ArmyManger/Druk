using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Druk.Common
{
    public class DoThread
    {

        #region //线程数据槽 DataSlot

        #region //将值存入线程的数据槽

        /// <summary>
        /// 将值存入线程的数据槽,
        /// 此数据槽是对所有线程通用的..所有线程皆可访问, 如果有同名线程则将覆盖..
        /// </summary>
        /// <param name="DataSlotName">数据槽名称, 根据isUseThreadName参数来确定是否拼接线程名称 例: 线程名称.DataSlotName</param>
        /// <param name="obj">存入的值</param>
        /// <param name="isUseThreadName">是否拼接线程名称</param>
        public void SetDataSlot_To_Thread(string DataSlotName, object obj, bool isUseThreadName = true)
        {
            var name = isUseThreadName ? Thread.CurrentThread.Name + "." + DataSlotName : DataSlotName;
            Thread.FreeNamedDataSlot(name);
            LocalDataStoreSlot dataSlot = Thread.AllocateNamedDataSlot(name);
            Thread.SetData(dataSlot, obj);
        }
        #endregion

        #region //从线程数据槽中获取值
        /// <summary>
        /// 从线程数据槽中获取值
        /// </summary>
        /// <param name="DataSlotName">数据槽名称, 根据isUseThreadName参数来确定是否拼接线程名称 例: 线程名称.DataSlotName</param>
        /// <param name="isUseThreadName">是否拼接线程名称</param>
        public object GetDataSlot_From_Thread(string DataSlotName, bool isUseThreadName = true)
        {
            var name = isUseThreadName ? Thread.CurrentThread.Name + "." + DataSlotName : DataSlotName;
            var dataSlot = Thread.GetNamedDataSlot(name); //根据名字获取数据槽位
            var obj = Thread.GetData(dataSlot);
            return obj;
        }
        #endregion

        #endregion
    }
}

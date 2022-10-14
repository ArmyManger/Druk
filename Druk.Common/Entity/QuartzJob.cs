using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common.Entity
{
    public class QuartzJob
    {
        public QuartzJob()
        {
            ClassName = null;
            Status = true;
            BeginTime = DateTime.Now;
            EndTime = DateTime.Now.AddYears(2);
            Cron_Like = string.Empty;
        }
        /// <summary>
        /// 方法体 所在类 必须实现 IJob 接口  Execute 方法
        /// </summary>
        public Type ClassName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 开始时间, 程序会在下一分钟开始时开始任务
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间 
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// cron-like表达式
        /// </summary>
        public string Cron_Like { get; set; }

        #region //cron-like表达式 常用示例
        /*
            cron-like表达式
            常用示例:

            0 10 * * * ?                 每个小时第10分执行一次
            0 0/32 8,12 * * ?            每天8:32,12:32 执行一次
            0 0/2 * * * ?                每2分钟执行一次
            0 0 12 * * ?                 在每天中午12：00触发
            0 15 10 ? * *                每天上午10:15 触发
            0 15 10 * * ?                每天上午10:15 触发
            0 15 10 * * ? *              每天上午10:15 触发
            0 15 10 * * ? 2005           在2005年中的每天上午10:15 触发
            0 * 14 * * ?                 每天在下午2：00至2：59之间每分钟触发一次
            0 0/5 14 * * ?               每天在下午2：00至2：59之间每5分钟触发一次
            0 0/5 14,18 * * ?            每天在下午2：00至2：59和6：00至6：59之间的每5分钟触发一次
            0 0-5 14 * * ?               每天在下午2：00至2：05之间每分钟触发一次
            0 10,44 14 ? 3 WED           每三月份的星期三在下午2：00和2：44时触发
            0 15 10 ? * MON-FRI          从星期一至星期五的每天上午10：15触发
            0 15 10 15 * ?               在每个月的每15天的上午10：15触发
            0 15 10 L * ?                在每个月的最后一天的上午10：15触发
            0 15 10 ? * 6L               在每个月的最后一个星期五的上午10：15触发 
            0 15 10 ? * 6L 2002-2005     在2002, 2003, 2004 and 2005年的每个月的最后一个星期五的上午10：15触发
            0 15 10 ? * 6#3              在每个月的第三个星期五的上午10：15触发 
            0 0 12 1/5 * ?               从每月的第一天起每过5天的中午12：00时触发 
            0 11 11 11 11 ?              在每个11月11日的上午11：11时触发.
        */
        #endregion
    }
}

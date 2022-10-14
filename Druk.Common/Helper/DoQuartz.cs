using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Druk.Common
{
    public class DoQuartz
    {

        #region //方法 作业调度

        /// <summary>
        /// 根据任务配置开始作业调度
        /// </summary>
        /// <param name="jobList"></param>
        public static void CreateJobList(List<Entity.QuartzJob> jobList)
        {
            //1.首先创建一个作业调度池
            ISchedulerFactory schedf = new StdSchedulerFactory();
            IScheduler sched = schedf.GetScheduler().Result;
            //将配置的多任务全部放入调度池
            jobList.Where(o => o.Status).ToList().ForEach(job =>
            {
                //2.创建出来一个具体的作业
                //创建一个Job来执行特定的任务
                IJobDetail jobDetail = new JobDetailImpl(job.ClassName.Name, job.ClassName);
                //开始时间
                DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(job.BeginTime, 0);
                //3.创建并配置一个触发器
                var trigger = TriggerBuilder.Create().StartAt(startTime).WithCronSchedule(job.Cron_Like);
                //如果结束时间比当前时间晚,则加载结束时间
                if (job.EndTime > DateTime.Now) { trigger = trigger.EndAt(DateBuilder.NextGivenSecondDate(job.EndTime, 3)); }
                //4.加入作业调度池中
                sched.ScheduleJob(jobDetail, trigger.Build());
            });

            //5.开始运行
            sched.Start();
        }
        #endregion
    }
}

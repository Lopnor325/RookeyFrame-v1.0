using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.QuartzClient
{
    public class Job
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 任务组名
        /// </summary>
        public string JobGroupName { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string JobDes { get; set; }

        /// <summary>
        /// 任务计划名称
        /// </summary>
        public string[] TriggerName { get; set; }

        /// <summary>
        /// 任务计划组名
        /// </summary>
        public string[] TriggerGroupName { get; set; }

        /// <summary>
        /// 任务执行表达式
        /// </summary>
        public string[] TriggerCron { get; set; }

        /// <summary>
        /// 任务计划描述
        /// </summary>
        public string[] TriggerDes { get; set; }

        /// <summary>
        /// 任务执行开始时间
        /// </summary>
        public string[] StartTime { get; set; }

        /// <summary>
        /// 任务执行结束时间
        /// </summary>
        public string[] EndTime { get; set; }

        /// <summary>
        /// 任务是否立即执行
        /// </summary>
        public bool[] IsStartNow { get; set; }
    }
}

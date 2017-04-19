/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Timers;
using System.Web;
using System.Linq;

namespace Rookey.Frame.AutoProcess
{
    /// <summary>
    /// 自动处理任务
    /// </summary>
    public class AutoProcessTask
    {
        /// <summary>
        /// 执行Execute方法后事件
        /// </summary>
        public static event EventHandler EventAfterExecute;

        /// <summary>
        /// 是否已经开始运行
        /// </summary>
        private static bool isStart = false;

        /// <summary>
        /// 任务列表
        /// </summary>
        private static List<BackgroundTask> taskList = new List<BackgroundTask>();

        /// <summary>
        /// 是否启动
        /// </summary>
        public static bool IsStart
        {
            get { return isStart; }
        }
       
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task">任务对象</param>
        public static void AddTask(BackgroundTask task)
        {
            if (task != null && isStart)
            {
                BackgroundTask tempTask = taskList.Where(x => x.ExecuteMethod == task.ExecuteMethod).FirstOrDefault();
                if (tempTask != null) //系统已存在该任务
                {
                    return;
                }
                taskList.Add(task); //添加到任务列表
                task.Execute(); //开始执行任务
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public static void Execute()
        {
            isStart = true;
            if (EventAfterExecute != null)
            {
                EventAfterExecute(null, null);
            }
        }
    }
}

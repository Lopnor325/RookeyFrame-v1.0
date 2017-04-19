/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.AutoProcess;
using Rookey.Frame.Operate.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Controllers.AutoHandle
{
    /// <summary>
    /// 系统自动任务
    /// </summary>
    public class SysAutoHandle
    {
        /// <summary>
        /// 添加后台系统任务
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public static void SysBackgroundTaskAdd(object obj, EventArgs e)
        {
            //重建索引任务，已移至任务调度中心
            //BackgroundTask reBuildIndexTask = new BackgroundTask((args) =>
            //{
            //    if (DateTime.Now.Hour == 4 && DateTime.Now.Minute == 0)
            //        SystemOperate.RebuildAllTableIndex();
            //    return true;
            //}, null, false, 45, false);
            //AutoProcessTask.AddTask(reBuildIndexTask);
        }
    }
}

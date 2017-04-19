using Rookey.Frame.Common;
using Quartz;
using System;
using System.IO;

namespace Rookey.Frame.QuartzClient.FrameJob
{
    /// <summary>
    /// 重建数据库索引
    /// </summary>
    public class RebuildDbIndexJob : IJob
    {
        /// <summary>
        /// 执行重建索引任务
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                object instance = null;
                Globals.ExecuteReflectMethod("Rookey.Frame.Operate.Base", "SystemOperate", "RebuildAllTableIndex", null, ref instance, true);
            }
            catch { }
        }
    }
}

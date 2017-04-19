using System.ServiceProcess;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Configuration;
using System;
using System.IO;
using System.Text;
using Quartz;

namespace Rookey.Frame.QuartzService
{
    /// <summary>
    /// 调度服务
    /// </summary>
    public partial class QuartzService : ServiceBase
    {
        private IScheduler scheduler;

        public QuartzService()
        {
            InitializeComponent();
            //取配置属性
            try
            {
                NameValueCollection properties = new NameValueCollection();
                var section = (NameValueCollection)ConfigurationManager.GetSection("quartz");
                foreach (string property in section.Keys)
                {
                    properties[property] = section[property];
                }
                properties["quartz.scheduler.exporter.rejectRemoteRequests"] = "false";
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
                scheduler = schedulerFactory.GetScheduler();
                WriteLog("任务调度服务初始化成功!");
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("调度服务初始化失败，原因：{0}", ex.Message));
            }
        }

        protected override void OnStart(string[] args)
        {
            if (scheduler == null)
            {
                WriteLog("任务调度对象为空!");
                return;
            }
            scheduler.Start();
            WriteLog("任务调度服务成功启动!");
        }

        protected override void OnStop()
        {
            if (scheduler == null)
            {
                WriteLog("任务调度对象为空!");
                return;
            }
            scheduler.Shutdown();
            WriteLog("任务调度服务成功终止!");
        }

        protected override void OnPause()
        {
            if (scheduler == null)
            {
                WriteLog("任务调度对象为空!");
                return;
            }
            scheduler.PauseAll();
            WriteLog("调度服务暂停!");
        }

        protected override void OnContinue()
        {
            if (scheduler == null)
            {
                WriteLog("任务调度对象为空!");
                return;
            }
            scheduler.ResumeAll();
            WriteLog("调度服务恢复!");
        }

        /// <summary>
        /// 写日志
        /// </summary>
        private void WriteLog(string msg)
        {
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "Logs";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string file = dir + string.Format("\\log_{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
                StreamWriter sw = new StreamWriter(file, true, Encoding.UTF8);
                sw.WriteLine(msg);
                sw.Close();
            }
            catch { }
        }
    }
}

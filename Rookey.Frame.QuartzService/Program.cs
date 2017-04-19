using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.ServiceProcess;

namespace Rookey.Frame.QuartzService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            /*-------开启服务---------*/
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new QuartzService() 
            };
            ServiceBase.Run(ServicesToRun);
            /*------------------------*/
        }
    }
}

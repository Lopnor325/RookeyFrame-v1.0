using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace Rookey.Frame.QuartzClient
{
    public class ServiceControl
    {
        public static string ErrMsg = string.Empty;

        /// <summary>
        /// 获取调度服务启动信息
        /// </summary>
        /// <returns></returns>
        public static string GetQuartzServiceInfo()
        {
            string result = string.Empty;
            ErrMsg = string.Empty;
            try
            {
                ServiceController sc = ServiceInstaller.GetService();
                if (sc != null)
                {
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Running:
                            result = "服务已启动!";
                            break;
                        case ServiceControllerStatus.Stopped:
                            result = "服务已停止!";
                            break;

                    }
                }
                else
                {
                    result = "服务未安装!";

                }
            }
            catch(Exception ex) 
            {
                ErrMsg = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 安装服务
        /// </summary>
        public static bool InstallQuartzService()
        {
            ErrMsg = string.Empty;
            bool state = ServiceInstaller.Install();
            ErrMsg = ServiceInstaller.ErrMsg;
            return state;
        }

        /// <summary>
        /// 反安装服务
        /// </summary>
        public static bool UninstallQuartzService()
        {
            ErrMsg = string.Empty;
            bool state = ServiceInstaller.UnInstall();
            ErrMsg = ServiceInstaller.ErrMsg;
            return state;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public static bool StartQuartzService()
        {
            ErrMsg = string.Empty;
            try
            {
                ServiceController sc = ServiceInstaller.GetService();
                if (sc != null && sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 重启服务
        /// </summary>
        /// <returns></returns>
        public static bool RestartQuartzService()
        {
            ErrMsg = string.Empty;
            try
            {
                ServiceController sc = ServiceInstaller.GetService();
                if (sc != null)
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                        sc.Refresh();
                        if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            sc.Start();
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public static bool StopQuartzService()
        {
            ErrMsg = string.Empty;
            try
            {
                ServiceController sc = ServiceInstaller.GetService();
                if (sc != null)
                {
                    sc.Stop();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ServiceProcess;
using System.Web;

namespace Rookey.Frame.QuartzClient
{
    class ServiceInstaller
    {
        public static string ErrMsg = string.Empty;
        private static string GetDotNetVersions()
        {
            string path = Environment.SystemDirectory.Replace(@"\system32", "") + @"\Microsoft.NET\Framework\";
            DirectoryInfo[] directories = new DirectoryInfo(path).GetDirectories("v4.?.*");
            ArrayList list = new ArrayList();
            foreach (DirectoryInfo info2 in directories)
            {
                list.Add(path + info2.Name);
            }
            if (list.Count > 0)
            {
                path = list[0] + @"\InstallUtil.exe";
                if (!File.Exists(path))
                {
                    path = null;
                }
            }
            return path;
        }

        public static bool Install()
        {
            bool state = false;
            try
            {
                string installpath = GetDotNetVersions();
                if (installpath != null)
                {
                    string spath = AppDomain.CurrentDomain.BaseDirectory + "Service\\Rookey.Frame.QuartzService.exe";
                    if (!File.Exists(spath))
                    {
                        throw new FileNotFoundException();
                    }
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    //设置程序名
                    startInfo.FileName = installpath;
                    startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "Service\\";
                    startInfo.Arguments = "/c Rookey.Frame.QuartzService.exe";
                    startInfo.Verb = "RunAs";
                    // 重定向标准输入
                    startInfo.RedirectStandardInput = true;
                    // 重定向标准输出
                    startInfo.RedirectStandardOutput = true;
                    //重定向错误输出
                    startInfo.RedirectStandardError = true;
                    // 关闭Shell的使用
                    startInfo.UseShellExecute = false;
                    // 设置不显示窗口
                    startInfo.CreateNoWindow = true;
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    //启动进程
                    process.Start();

                    state = true;
                }
                else
                {
                    state = false;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            return state;
        }

        public static bool UnInstall()
        {
            bool state = false;
            try
            {
                string installpath = GetDotNetVersions();
                if (installpath != null)
                {
                    string spath = AppDomain.CurrentDomain.BaseDirectory + "Service\\Rookey.Frame.QuartzService.exe";
                    if (!File.Exists(spath))
                    {
                        throw new FileNotFoundException();
                    }
                    Process p = new Process();
                    //设置程序名
                    p.StartInfo.FileName = installpath;
                    p.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "Service\\";
                    p.StartInfo.Arguments = "/u Rookey.Frame.QuartzService.exe";
                    // 关闭Shell的使用
                    p.StartInfo.UseShellExecute = false;
                    // 重定向标准输入
                    p.StartInfo.RedirectStandardInput = true;
                    // 重定向标准输出
                    p.StartInfo.RedirectStandardOutput = true;
                    //重定向错误输出
                    p.StartInfo.RedirectStandardError = true;
                    // 设置不显示窗口
                    p.StartInfo.CreateNoWindow = true;

                    p.Start();

                    state = true;
                }
                else
                {
                    state = false;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            return state;
        }

        public static ServiceController GetService()
        {
            ServiceController currentService = null;
            ServiceController[] Service = ServiceController.GetServices();
            foreach (ServiceController sc in Service)
            {
                if (sc.ServiceName.ToLower() == "quartzservice")
                {
                    currentService = sc;
                    break;
                }
            }
            return currentService;
        }
    }
}

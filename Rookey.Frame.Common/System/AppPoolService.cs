/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using Microsoft.Web.Administration; //位于：C:\Windows\System32\inetsrv\Microsoft.Web.Administration.dll  

namespace Rookey.Frame.Common.Sys
{
    /// <summary>  
    ///     IIS应用程序池辅助类  
    /// </summary>  
    public class AppPoolService
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        protected static string Host = "localhost";

        /// <summary>  
        ///     取得所有应用程序池  
        /// </summary>  
        /// <returns><para> Dictionary<string,ApplicationPool>应用程序名，程序池对象</para> </returns>  
        public static Dictionary<string,ApplicationPool> GetAppPools()
        {
            Dictionary<string, ApplicationPool> dic = new Dictionary<string, ApplicationPool>();
            var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
            foreach (DirectoryEntry entry in appPools.Children)
            {
                var manager = new ServerManager();
                ApplicationPool app = manager.ApplicationPools[entry.Name];
                dic.Add(entry.Name, app);
            }
            return dic;
        }

        /// <summary>  
        ///     取得单个应用程序池  
        /// </summary>  
        /// <returns></returns>  
        public static ApplicationPool GetAppPool(string appPoolName)
        {
            ApplicationPool app = null;
            var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
            foreach (DirectoryEntry entry in appPools.Children)
            {
                if (entry.Name == appPoolName)
                {
                    var manager = new ServerManager();
                    app = manager.ApplicationPools[appPoolName];
                }
            }
            return app;
        }

        /// <summary>  
        ///     判断程序池是否存在  
        /// </summary>  
        /// <param name="appPoolName">程序池名称</param>  
        /// <returns>true存在 false不存在</returns>  
        public static bool IsAppPoolExsit(string appPoolName)
        {
            bool result = false;
            var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
            foreach (DirectoryEntry entry in appPools.Children)
            {
                if (entry.Name.Equals(appPoolName))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>  
        ///     删除指定程序池  
        /// </summary>  
        /// <param name="appPoolName">程序池名称</param>  
        /// <returns>true删除成功 false删除失败</returns>  
        public static bool DeleteAppPool(string appPoolName)
        {
            bool result = false;
            var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
            foreach (DirectoryEntry entry in appPools.Children)
            {
                if (entry.Name.Equals(appPoolName))
                {
                    try
                    {
                        entry.DeleteTree();
                        result = true;
                        break;
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>  
        ///     创建应用程序池  
        /// </summary>  
        /// <param name="appPool"></param>  
        /// <returns></returns>  
        public static bool CreateAppPool(string appPool)
        {
            try
            {
                if (!IsAppPoolExsit(appPool))
                {
                    var appPools = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/AppPools", Host));
                    DirectoryEntry entry = appPools.Children.Add(appPool, "IIsApplicationPool");
                    entry.CommitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>  
        ///     编辑应用程序池  
        /// </summary>  
        /// <param name="application"></param>  
        /// <returns></returns>  
        public static bool EditAppPool(ApplicationPool application)
        {
            try
            {
                if (IsAppPoolExsit(application.Name))
                {
                    var manager = new ServerManager();
                    manager.ApplicationPools[application.Name].ManagedRuntimeVersion = application.ManagedRuntimeVersion;
                    manager.ApplicationPools[application.Name].ManagedPipelineMode = application.ManagedPipelineMode;
                    //托管模式Integrated为集成 Classic为经典  
                    manager.CommitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}

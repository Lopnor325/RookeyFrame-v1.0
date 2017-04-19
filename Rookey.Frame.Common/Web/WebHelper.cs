using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// Web辅助处理类
    /// </summary>
    public static class WebHelper
    {
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static string GetClientIP(HttpRequestBase request)
        {
            if (request == null) return string.Empty;
            string clientIp = string.Empty;
            if (request.ServerVariables["HTTP_VIA"] != null) //使用代理
            {
                clientIp = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString(); // 返回真实的客户端IP 
            }
            else// 没有使用代理时获取客户端IP 
            {
                clientIp = request.ServerVariables["REMOTE_ADDR"].ToString(); //当不能获取客户端IP时,将获取客户端代理IP. 
            }
            return clientIp;
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="context">上下文请求对象</param>
        /// <returns></returns>
        public static string GetClientIP(HttpContext context)
        {
            if (context == null) return string.Empty;
            string clientIp = string.Empty;
            if (context.Request.ServerVariables["HTTP_VIA"] != null) //使用代理
            {
                clientIp = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString(); // 返回真实的客户端IP 
            }
            else// 没有使用代理时获取客户端IP 
            {
                clientIp = context.Request.ServerVariables["REMOTE_ADDR"].ToString(); //当不能获取客户端IP时,将获取客户端代理IP. 
            }
            return clientIp;
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="configName">配置文件名，带文件扩展名</param>
        /// <returns></returns>
        public static string GetConfigFilePath(string configName)
        {
            string pathFlag = "\\";
            if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                pathFlag = "/";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (!basePath.EndsWith(pathFlag))
            {
                basePath += pathFlag;
            }
            string xmlPath = basePath + string.Format("Config{0}{1}", pathFlag, configName);
            if (!System.IO.File.Exists(xmlPath)) //文件不存在
                return string.Empty;
            return xmlPath;
        }

        /// <summary>
        /// 获取webapi请求对象中的传统request对象
        /// </summary>
        /// <param name="requestMessage">webapi请求对象</param>
        /// <returns></returns>
        public static HttpRequestBase GetContextRequest(HttpRequestMessage requestMessage)
        {
            if (requestMessage != null)
            {
                HttpContextBase context = (HttpContextBase)requestMessage.Properties["MS_HttpContext"]; //获取传统context
                return context.Request; //定义传统request对象
            }
            return null;
        }

        /// <summary>
        /// 获取JS的修改时间
        /// </summary>
        /// <param name="jsPath">JS的URL路径</param>
        /// <returns></returns>
        public static string GetJsModifyTimeStr(string jsPath)
        {
            if (string.IsNullOrEmpty(jsPath))
                return string.Empty;
            string tempPath = jsPath;
            try
            {
                if (tempPath.StartsWith("/"))
                    tempPath = tempPath.Substring(1, tempPath.Length - 1);
            }
            catch 
            {
                return string.Empty;
            }
            bool isLinux = WebConfigHelper.GetAppSettingValue("IsLinux") == "true";
            string filePath = Globals.GetWebDir();
            if (isLinux)
                filePath += jsPath;
            else
                filePath += jsPath.Replace("/", "\\");
            if (!System.IO.File.Exists(filePath)) //js不存在
                return string.Empty;
            try
            {
                FileInfo fi = new FileInfo(filePath);
                string r = fi.LastWriteTime.ToString("yyMMddHHmmss");
                return r;
            }
            catch { }
            return string.Empty;
        }
    }
}

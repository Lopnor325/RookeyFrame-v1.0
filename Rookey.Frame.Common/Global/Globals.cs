/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Reflection;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 用途：用于全局使用
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// 转向ssl
        /// </summary>
        /// <param name="context"></param>
        public static void RedirectToSSL(HttpContext context)
        {
            if (!context.Request.IsSecureConnection)
            {
                Uri url = context.Request.Url;
                context.Response.Redirect("https://" + url.ToString().Substring(7));
            }
        }

        #region Encode/Decode

        /// <summary>
        /// Converts a prepared subject line back into a raw text subject line.
        /// </summary>
        /// <param name="text">The prepared subject line.</param>
        /// <returns>A raw text subject line.</returns>
        /// <remarks>This function is only needed when editing an existing message or when replying to
        /// a message - it turns the HTML escaped characters back into their pre-escaped status.</remarks>
        public static string HtmlEncode(String text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return System.Web.HttpUtility.HtmlEncode(text);
        }

        public static string UrlEncode(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            return System.Web.HttpUtility.UrlEncode(url).Replace("'", "%27");
        }

        public static string UrlDecode(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            return System.Web.HttpUtility.UrlDecode(url.Replace("\\\\", "\\"));
        }

        #endregion

        /// <summary>
        /// 获取网站架构Url
        /// </summary>
        /// <returns></returns>
        public static string GetBaseUrl()
        {
            try
            {
                return ApplicationObject.CurrentOneHttpContext.Request.Url.Scheme + "://" + ApplicationObject.CurrentOneHttpContext.Request.Url.Authority + ApplicationObject.CurrentOneHttpContext.Request.ApplicationPath.TrimEnd('/') + '/';
            }
            catch { }
            string webHost = WebConfigHelper.GetAppSettingValue("WebHost");
            return webHost;
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            try
            {
                return ApplicationObject.CurrentOneHttpContext.Request.UserHostAddress;
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 获取Web路径
        /// </summary>
        /// <returns></returns>
        public static string GetWebDir()
        {
            string pathFlag = "\\";
            if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                pathFlag = "/";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (basePath.EndsWith(pathFlag))
            {
                basePath = basePath.Substring(0, basePath.Length - 1);
            }
            if (basePath.EndsWith("bin"))
            {
                basePath = basePath.Substring(0, basePath.Length - 3);
            }
            if (!basePath.EndsWith(pathFlag))
            {
                basePath += pathFlag;
            }
            return basePath;
        }

        /// <summary>
        /// 获取Bin目录，包含反斜杠
        /// </summary>
        /// <returns></returns>
        public static string GetBinPath()
        {
            string pathFlag = "\\";
            if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                pathFlag = "/";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (!basePath.EndsWith(pathFlag))
            {
                basePath += pathFlag;
            }
            if (!basePath.Contains("bin"))
            {
                basePath += "bin";
            }
            if (!basePath.EndsWith(pathFlag))
            {
                basePath += pathFlag;
            }
            return basePath;
        }

        /// <summary>
        /// 获取当前目录
        /// </summary>
        /// <returns></returns>
        public static string GetCurretnDir()
        {
            return ApplicationObject.CurrentOneHttpContext.Request.PhysicalApplicationPath;
        }

        /// <summary>
        /// 获取当前地址
        /// </summary>
        /// <returns>当前地址</returns>
        public static string GetCurrentUrl()
        {
            return ApplicationObject.CurrentOneHttpContext.Request.Url.AbsoluteUri;
        }

        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="jsonData">Json数据</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string jsonData)
        {
            return JsonHelper.Deserialize(jsonData, type);
        }

        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="jsonData">Json数据</param>
        /// <param name="errMsg">异常信息</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string jsonData, out string errMsg)
        {
            errMsg = string.Empty;
            if (type == null || string.IsNullOrEmpty(jsonData)) return null;
            try
            {
                return JsonHelper.Deserialize(jsonData, type, out errMsg);
            }
            catch { }
            return null;
        }

        #region 反射处理

        /// <summary>
        /// 程序集缓存
        /// </summary>
        private static Dictionary<string, List<Type>> assemblyTypesCaches = new Dictionary<string, List<Type>>();

        /// <summary>
        /// 获取程序集中的所有类型集合
        /// </summary>
        /// <param name="dllName">程序集名称</param>
        /// <returns></returns>
        public static List<Type> GetAssemblyTypes(string dllName)
        {
            if (assemblyTypesCaches.ContainsKey(dllName))
            {
                List<Type> listTypes = assemblyTypesCaches[dllName];
                if (listTypes == null) listTypes = new List<Type>();
                return listTypes;
            }
            try
            {
                string dllPath = string.Format("{0}{1}.dll", Globals.GetBinPath(), dllName);
                Assembly assembly = Assembly.LoadFrom(dllPath);
                List<Type> list = assembly.GetTypes().ToList();
                assemblyTypesCaches.Add(dllName, list);
                return list;
            }
            catch { }
            return new List<Type>();
        }

        /// <summary>
        /// 获取程序集中的类型
        /// </summary>
        /// <param name="dllName">程序集名称</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static Type GetAssemblyType(string dllName, string className)
        {
            List<Type> types = GetAssemblyTypes(dllName);
            return types.Where(x => x.Name == className).FirstOrDefault();
        }

        /// <summary>
        /// 执行反射方法，针对自定义程序集
        /// </summary>
        /// <param name="dllName">dll名称</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <param name="instance">类型实例对象</param>
        /// <param name="isStaticMethod">是否静态方法</param>
        /// <param name="parmaTypes">方法参数类型集合</param>
        /// <param name="methodGenericTypes">方法泛型参数</param>
        /// <returns>返回执行结果</returns>
        public static object ExecuteReflectMethod(string dllName, string className, string methodName, object[] args, ref object instance, bool isStaticMethod = false, Type[] parmaTypes = null, Type[] methodGenericTypes = null)
        {
            Type type = GetAssemblyType(dllName, className);
            return ExecuteReflectMethod(type, methodName, args, ref instance, isStaticMethod, parmaTypes, methodGenericTypes);
        }

        /// <summary>
        /// 执行反射方法，针对系统程序集
        /// </summary>
        /// <param name="type">要反射的类型</param>
        /// <param name="genericTypes">类型的参数类型集合</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <param name="instance">类型实例对象</param>
        /// <param name="isStaticMethod">是否静态方法</param>
        /// <param name="parmaTypes">方法参数类型集合</param>
        /// <param name="methodGenericTypes">方法泛型参数</param>
        /// <returns>返回执行结果</returns>
        public static object ExecuteReflectMethod(Type type, Type[] genericTypes, string methodName, object[] args, ref object instance, bool isStaticMethod = false, Type[] parmaTypes = null, Type[] methodGenericTypes = null)
        {
            Type tempType = type.MakeGenericType(genericTypes);
            return ExecuteReflectMethod(tempType, methodName, args, ref instance, isStaticMethod, parmaTypes, methodGenericTypes);
        }

        /// <summary>
        /// 执行反射方法，针对系统程序集
        /// </summary>
        /// <param name="type">要反射的类型</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <param name="instance">类型实例对象</param>
        /// <param name="isStaticMethod">是否静态方法</param>
        /// <param name="parmaTypes">方法参数类型集合</param>
        /// <param name="methodGenericTypes">方法的泛型参数</param>
        /// <returns>返回执行结果</returns>
        public static object ExecuteReflectMethod(Type type, string methodName, object[] args, ref object instance, bool isStaticMethod = false, Type[] parmaTypes = null, Type[] methodGenericTypes = null)
        {
            BindingFlags bindFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            if (!isStaticMethod)
            {
                if (instance == null)
                    instance = Activator.CreateInstance(type);
            }
            MethodInfo method = parmaTypes != null ? type.GetMethod(methodName, bindFlags, null, parmaTypes, null) : type.GetMethod(methodName, bindFlags);
            if (method == null) return null;
            if (methodGenericTypes != null)
                method = method.MakeGenericMethod(methodGenericTypes);
            //反射执行方法
            FastInvoke.FastInvokeHandler fastInvoker = FastInvoke.GetMethodInvoker(method);
            try
            {
                object executedObj = fastInvoker(instance, args);
                return executedObj;
            }
            catch
            {
                return method.Invoke(instance, args);
            }
        }

        #endregion
    }
}

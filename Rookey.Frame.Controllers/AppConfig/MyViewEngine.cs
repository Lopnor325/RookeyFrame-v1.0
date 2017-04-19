using Rookey.Frame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rookey.Frame.Controllers.AppConfig
{
    /// <summary>
    /// 自定义视图引擎类
    /// </summary>
    public sealed class MyViewEngine : RazorViewEngine
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MyViewEngine()
        {
            List<string> viewFormats = new List<string>()
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Page/{0}.cshtml", //自定义View规则 
                "~/Views/Page/Common/{0}.cshtml", //自定义View规则
                "~/Views/Page/System/{0}.cshtml", //自定义View规则
                "~/Views/Page/Permission/{0}.cshtml", //自定义View规则
                "~/Views/Page/Desktop/{0}.cshtml", //自定义View规则
                "~/Views/Page/Email/{0}.cshtml" //自定义View规则
            };
            string otherViewConfigs = WebConfigHelper.GetAppSettingValue("ViewConfig");
            if (!string.IsNullOrEmpty(otherViewConfigs)) //其他视图配置
            {
                List<string> token = otherViewConfigs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                viewFormats.AddRange(token);
            }
            ViewLocationFormats = viewFormats.ToArray();
        }

        /// <summary>
        /// 查找视图
        /// </summary>
        /// <param name="controllerContext">控制器上下文</param>
        /// <param name="viewName">视图名</param>
        /// <param name="masterName"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }
    }
}
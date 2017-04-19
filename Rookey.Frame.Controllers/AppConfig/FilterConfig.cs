using Rookey.Frame.Controllers.Attr;
using System.Web;
using System.Web.Mvc;

namespace Rookey.Frame.Controllers.AppConfig
{
    /// <summary>
    /// 过滤配置
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// 全局过滤配置
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // ExceptionAttribute继承自HandleError，主要作用是将异常信息写入日志系统中
            filters.Add(new ExceptionAttribute());
            //默认的异常记录类
            filters.Add(new HandleErrorAttribute());
        }
    }
}
using Rookey.Frame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;

namespace Rookey.Frame.Controllers.Attr
{
    /// <summary>
    /// 跨域属性类
    /// </summary>
    public class CrossOriginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CrossOriginAttribute()
        {
            OriginHeaderdefault = Globals.GetBaseUrl();
        }
        /// <summary>
        /// OriginHeaderdefault的值可以使 URL 或 *，如果是 URL 则只会允许来自该 URL 的请求，* 则允许任何域的请求
        /// </summary>
        public string OriginHeaderdefault { get; set; }
        /// <summary>
        /// Access-Control-Allow-Origin是HTML5中定义的一种服务器端返回Response header，用来解决资源（比如字体）的跨域权限问题。
        /// </summary>
        private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        /// <summary>
        /// 该方法允许api支持跨域调用
        /// </summary>
        /// <param name="actionExecutedContext"> 初始化 System.Web.Http.Filters.HttpActionExecutedContext 类的新实例。</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.Add(AccessControlAllowOrigin, OriginHeaderdefault);
        }
    }
}

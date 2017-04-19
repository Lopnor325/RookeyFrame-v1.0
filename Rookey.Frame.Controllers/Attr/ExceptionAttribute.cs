using Rookey.Frame.Operate.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Rookey.Frame.Controllers.Attr
{
    /// <summary>
    /// 异常处理特性
    /// </summary>
    public class ExceptionAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// 触发异常时调用的方法
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            //参数
            StringBuilder strParams = new StringBuilder();
            if (filterContext.Controller != null && filterContext.Controller.ControllerContext != null
                && filterContext.Controller.ControllerContext.HttpContext != null
                && (filterContext.Controller.ControllerContext.HttpContext).Request != null
                && (filterContext.Controller.ControllerContext.HttpContext).Request.QueryString != null)
            {
                NameValueCollection paramCollection = (filterContext.Controller.ControllerContext.HttpContext).Request.QueryString;
                string[] paramArray = ((filterContext.Controller.ControllerContext.HttpContext).Request.QueryString).AllKeys;
                foreach (string oneParam in paramArray)
                {
                    try
                    {
                        strParams.AppendFormat("{0}={1},", oneParam, paramCollection[oneParam]);
                    }
                    catch (Exception ex)
                    {
                        strParams.AppendFormat("{0}=获得参数值异常,{1}", oneParam, ex.Message);
                    }
                }
            }
            LogOperate.AddExceptionLog(filterContext.Exception, strParams.ToString(), filterContext.RouteData.Values["controller"].ToString(), filterContext.RouteData.Values["action"].ToString());
            base.OnException(filterContext);
        }
    }
}

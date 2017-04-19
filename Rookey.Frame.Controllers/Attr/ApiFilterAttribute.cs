using Rookey.Frame.Common;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http;
using System.Net;

namespace Rookey.Frame.Controllers.Attr
{
    /// <summary>
    /// webapi过滤特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        /// <summary>
        /// 执行action前
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }
            HttpRequestBase request = WebHelper.GetContextRequest(actionContext.Request);
            //接下来进行权限拦截与验证
            if (!this.AuthorizeCore(actionContext))//根据验证判断进行处理
            {
                bool validOk = false;
                //是否ajax请求
                bool isAjax = request.IsAjaxRequest();
                if (isAjax)
                {
                    //未登录验证
                    if (!request.IsAuthenticated)
                        validOk = false;
                    else
                        validOk = true;
                }
                else
                {
                    validOk = false;
                }
                if (!validOk) //未通过验证
                {
                    actionContext.Response.Content = new StringContent("验证不通过，非法操作！");
                    actionContext.Response.StatusCode = HttpStatusCode.Conflict;
                    //在这里为了不继续走流程，要throw出来，才会立马返回到客户端
                    throw new HttpResponseException(actionContext.Response);
                }
            }
            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// [Anonymous标记]验证是否匿名访问
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public bool CheckAnonymous(HttpActionContext actionContext)
        {
            //验证是否是匿名访问的Action
            var attrsAnonymous = actionContext.ActionDescriptor.GetCustomAttributes<AnonymousAttribute>();
            //是否是Anonymous
            return attrsAnonymous.Count >= 1;
        }

        /// <summary>
        /// [LoginAllowView标记]验证是否登录就可以访问(如果已经登陆,那么不对于标识了LoginAllowView的方法就不需要验证了)
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public bool CheckLoginAllowView(HttpActionContext actionContext)
        {
            //在这里允许一种情况,如果已经登陆,那么不对于标识了LoginAllowView的方法就不需要验证了
            var attrs = actionContext.ActionDescriptor.GetCustomAttributes<LoginAllowViewAttribute>();
            //是否是LoginAllowView
            return attrs.Count >= 1;
        }

        /// <summary>
        /// //权限判断业务逻辑
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected virtual bool AuthorizeCore(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }
            //验证当前Action是否是匿名访问Action
            if (CheckAnonymous(actionContext))
                return true;
            //未登录验证
            HttpRequestBase request = WebHelper.GetContextRequest(actionContext.Request);
            if (!request.IsAuthenticated)
            {
                return false;
            }
            //验证当前Action是否是登录就可以访问的Action
            if (CheckLoginAllowView(actionContext))
                return true;
            //下面开始用户权限验证
            var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = actionContext.ActionDescriptor.ActionName;

            return true;
        }

        /// <summary>
        /// 执行action后
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}

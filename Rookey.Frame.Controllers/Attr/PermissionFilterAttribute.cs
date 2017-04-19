/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Base;
using Rookey.Frame.Common;
using System;
using System.Text;
using System.Web.Mvc;

namespace Rookey.Frame.Controllers.Attr
{
    /// <summary>
    /// 权限拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 权限拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            //接下来进行权限拦截与验证
            if (!this.AuthorizeCore(filterContext))//根据验证判断进行处理
            {
                string loginUrl = "/user/login.html";
                string returnUrl = filterContext.HttpContext.Request["returnUrl"].ObjToStr();
                if (!string.IsNullOrEmpty(returnUrl))
                    loginUrl += string.Format("?returnUrl={0}", returnUrl);
                string loginContent = string.Format("<script>top.location.href='{0}';</script>", loginUrl);
                //是否ajax请求
                bool isAjax = filterContext.HttpContext.Request.IsAjaxRequest();
                if (isAjax)
                {
                    //未登录验证
                    if (!filterContext.HttpContext.Request.IsAuthenticated)
                    {
                        //弹出登录页面框
                        filterContext.Result = new ContentResult() { Content = loginContent, ContentType = "text/html", ContentEncoding = Encoding.UTF8 };
                        return;
                    }
                    return;
                }
                //跳转到登录页面
                filterContext.Result = new ContentResult() { Content = loginContent, ContentType = "text/html", ContentEncoding = Encoding.UTF8 };
                return;
            }
        }

        /// <summary>
        /// [Anonymous标记]验证是否匿名访问
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        public bool CheckAnonymous(ActionExecutingContext filterContext)
        {
            //验证是否是匿名访问的Action
            object[] attrsAnonymous = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AnonymousAttribute), true);
            //是否是Anonymous
            return attrsAnonymous.Length >= 1;
        }

        /// <summary>
        /// [LoginAllowView标记]验证是否登录就可以访问(如果已经登陆,那么不对于标识了LoginAllowView的方法就不需要验证了)
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        public bool CheckLoginAllowView(ActionExecutingContext filterContext)
        {
            //在这里允许一种情况,如果已经登陆,那么不对于标识了LoginAllowView的方法就不需要验证了
            object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(LoginAllowViewAttribute), true);
            //是否是LoginAllowView
            return attrs.Length >= 1;
        }

        /// <summary>
        /// //权限判断业务逻辑
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        protected virtual bool AuthorizeCore(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            //验证当前Action是否是匿名访问Action
            if (CheckAnonymous(filterContext))
                return true;
            //未登录验证
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                return false;
            }
            //验证当前Action是否是登录就可以访问的Action
            if (CheckLoginAllowView(filterContext))
                return true;
            //下面开始用户权限验证
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();

            return true;
        }
    }
}
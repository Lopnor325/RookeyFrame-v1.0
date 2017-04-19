using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Rookey.Frame.Base;
using Rookey.Frame.Common;
using System.Web.Security;
using Rookey.Frame.AutoProcess;
using Rookey.Frame.Controllers.AutoHandle;
using FluentValidation.Mvc;
using Rookey.Frame.Controllers.Other;
using FluentValidation.Attributes;
using Rookey.Frame.Operate.Base;

namespace Rookey.Frame.Controllers.AppConfig
{
    /// <summary>
    /// MVC应用程序类
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// 应用程序启动
        /// </summary>
        public void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //启动自动处理程序
            AutoProcessTask.EventAfterExecute += new EventHandler(SysAutoHandle.SysBackgroundTaskAdd);
            AutoProcessTask.Execute();
            //用户扩展对象
            UserExtendEventHandler.BindUserExtendEvent += new UserExtendEventHandler.EventUserExtend(UserExtendHandle.GetUserExtendObject);
            //自定义应用程序启动
            SysApplicationHandle.Application_Start(this.Application);
            //验证配置
            ConfigureFluentValidation();
            //注册视图访问规则
            RegisterView();
        }

        /// <summary>
        /// 应用程序结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Application_End(object sender, EventArgs e)
        {
            try
            {
                HttpApplication app = (HttpApplication)sender;
                //自定义应用程序结束
                SysApplicationHandle.Application_End(app);
            }
            catch { }
        }

        /// <summary>
        /// 应用程序请求开始
        /// </summary>
        /// <param name="sender">发送对象</param>
        /// <param name="e">事件参数</param>
        public void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            ApplicationObject.CurrentOneHttpContext = app.Context;
            HttpCookie cookie = app.Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                cookie.Expires = DateTime.Now.AddMinutes(UserInfo.ACCOUNT_EXPIRATION_TIME);
                Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 应用程序结束请求
        /// </summary>
        public void Application_EndRequest()
        {
        }

        /// <summary>
        /// 应用程序认证请求
        /// </summary>
        /// <param name="sender">发送对象</param>
        /// <param name="e">事件参数</param>
        public void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            string username = string.Empty;
            if (app.Context.User != null && app.Context.User.Identity != null)
                username = app.Context.User.Identity.Name;
            int w = 0;
            int h = 0;
            if (app.Context.Request["nfm"].ObjToInt() == 1)
            {
                username = app.Context.Request["un"].ObjToStr(); //请求中自带的用户名
                w = app.Context.Request["w"].ObjToInt();
                h = app.Context.Request["h"].ObjToInt();
            }
            if (!string.IsNullOrEmpty(username))
            {
                UserInfo tempUserInfo = UserInfo.GetCurretnUser(app.Context);
                if (tempUserInfo == null || tempUserInfo.UserId == Guid.Empty || tempUserInfo.UserName.ToLower() != username.ToLower())
                {
                    Guid userId = UserOperate.GetUserIdByUserName(username);
                    UserInfo userInfo = UserOperate.GetUserInfo(userId);
                    if (w > 0 && h > 0)
                    {
                        userInfo.ClientBrowserWidth = w;
                        userInfo.ClientBrowserHeight = h;
                    }
                    //缓存用户扩展信息
                    UserInfo.CacheUserExtendInfo(userInfo.UserName, userInfo.ExtendUserObject);
                    //保存票据
                    FormsPrincipal.Login(userInfo.UserName, userInfo, UserInfo.ACCOUNT_EXPIRATION_TIME, app.Context);
                }
                FormsPrincipal.TrySetUserInfo(app.Context);
            }
            else
            {
                FormsPrincipal.TrySetUserInfo(app.Context);
            }
        }

        /// <summary>
        /// FluentValidation验证设置
        /// </summary>
        private void ConfigureFluentValidation()
        {
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new AttributedValidatorFactory()));
        }

        /// <summary>
        /// 注册视图访问规则
        /// </summary>
        private void RegisterView()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MyViewEngine());
        }
    }
}

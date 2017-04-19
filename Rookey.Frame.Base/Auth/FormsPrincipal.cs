/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Rookey.Frame.Base
{
    /// <summary>
    /// 表单认证
    /// </summary>
    public sealed class FormsPrincipal : IPrincipal
    {
        private readonly IIdentity _identity;
        private readonly UserInfo _userData;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="userData"></param>
        public FormsPrincipal(FormsAuthenticationTicket ticket, UserInfo userData)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");
            if (userData == null)
                throw new ArgumentNullException("userData");

            _identity = new FormsIdentity(ticket);
            _userData = userData;
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo UserData
        {
            get { return _userData; }
        }

        /// <summary>
        /// 身份标识
        /// </summary>
        public IIdentity Identity
        {
            get { return _identity; }
        }

        /// <summary>
        /// 角色判断
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            return true;
        }

        /// <summary>
        /// 执行用户登录操作
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="userData">与登录名相关的用户信息</param>
        /// <param name="expiration">登录Cookie的过期时间，单位：分钟。</param>
        /// <param name="currContext">当前context</param>
        public static void Login(string loginName, UserInfo userData, int expiration, HttpContext currContext)
        {
            if (string.IsNullOrEmpty(loginName))
                throw new ArgumentNullException("loginName");
            if (userData == null)
                throw new ArgumentNullException("userData");

            // 1. 把需要保存的用户数据转成一个字符串。
            userData.ExtendUserObject = null;
            string data = JsonHelper.Serialize(userData); //用户基本信息

            // 2. 创建一个FormsAuthenticationTicket，它包含登录名以及额外的用户数据。
            var ticket = new FormsAuthenticationTicket(2, loginName, DateTime.Now, DateTime.Now.AddDays(1), true, data);

            // 3. 加密Ticket，变成一个加密的字符串。
            var cookieValue = FormsAuthentication.Encrypt(ticket);

            // 4. 根据加密结果创建登录Cookie
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Domain = FormsAuthentication.CookieDomain,
                Path = FormsAuthentication.FormsCookiePath
            };
            if (expiration > 0)
                cookie.Expires = DateTime.Now.AddMinutes(expiration);

            HttpContext context = currContext;
            if (context == null)
                throw new InvalidOperationException();

            // 5. 写登录Cookie
            context.Response.Cookies.Remove(cookie.Name);
            context.Response.Cookies.Add(cookie);
            if (currContext != null)
            {
                context.Request.Cookies.Remove(cookie.Name);
                context.Request.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 安全退出系统
        /// </summary>
        /// <param name="response">响应对象</param>
        /// <param name="session">session对象</param>
        public static void Logout(HttpResponseBase response, HttpSessionStateBase session)
        {
            FormsAuthentication.SignOut();
            session.Abandon();
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty);
            cookie1.Expires = DateTime.Now.AddYears(-1);
            response.Cookies.Add(cookie1);
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            response.Cookies.Add(cookie2);
            FormsAuthentication.RedirectToLoginPage();
        }

        /// <summary>
        /// 根据HttpContext对象设置用户标识对象
        /// </summary>
        /// <param name="context"></param>
        public static void TrySetUserInfo(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            // 1. 读登录Cookie
            HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return;
            try
            {
                UserInfo userData = null;
                // 2. 解密Cookie值，获取FormsAuthenticationTicket对象
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket != null && !string.IsNullOrEmpty(ticket.UserData))
                {  
                    // 3. 还原用户数据
                    userData = JsonHelper.Deserialize<UserInfo>(ticket.UserData);
                }
                if (userData != null)
                {  
                    // 4. 构造我们的FormsPrincipal实例，重新给context.User赋值。
                    context.User = new FormsPrincipal(ticket, userData);
                }
            }
            catch { /* 有异常也不要抛出，防止攻击者试探。 */ }
        }
    }
}

/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Base.User;
using Rookey.Frame.Cache.Factory;
using Rookey.Frame.Cache.Factory.Provider;
using Rookey.Frame.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;

namespace Rookey.Frame.Base
{
    /// <summary>
    /// 用户类
    /// </summary>
    public sealed class UserInfo
    {
        #region 当前账户

        /// <summary>
        /// 互斥锁
        /// </summary>
        private static object locker = new object();

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <param name="context">上下文对象</param>
        /// <returns></returns>
        public static UserInfo GetCurretnUser(HttpContext context)
        {
            lock (locker)
            {
                try
                {
                    if (context == null) return null;
                    FormsIdentity identity = context.User.Identity as FormsIdentity;
                    if (identity != null)
                    {
                        //取用户基本信息
                        UserInfo userInfo = JsonHelper.Deserialize<UserInfo>(identity.Ticket.UserData);
                        UserExtendBase userExtend = GetUserExtendCache(userInfo.UserName);
                        if (userInfo.UserName != "admin" && userExtend == null)
                        {
                            //重新加载用户扩展信息
                            userExtend = UserExtendEventHandler.GetUserExtendInfo(userInfo);
                        }
                        userInfo.ExtendUserObject = userExtend;
                        return userInfo;
                    }
                }
                catch { }
                return null;
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserInfo()
        {
            ClientBrowserWidth = 0;
            ClientBrowserHeight = 0;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户别名
        /// </summary>
        public string AliasName { get; set; }

        /// <summary>
        /// 所属组织
        /// </summary>
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public Guid? EmpId { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmpName { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmpCode { get; set; }

        /// <summary>
        /// 扩展用户对象
        /// </summary>
        public UserExtendBase ExtendUserObject { get; set; }

        #region 客户端参数
        /// <summary>
        /// 客户端浏览器可见区域宽
        /// </summary>
        public int ClientBrowserWidth { get; set; }

        /// <summary>
        /// 客户端浏览器可见区域高
        /// </summary>
        public int ClientBrowserHeight { get; set; }
        #endregion

        #endregion

        #region 静态方法

        /// <summary>
        /// 获取当前用户别名
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static string GetUserAliasName(UserInfo currUser)
        {
            if (currUser != null)
            {
                if (string.IsNullOrWhiteSpace(currUser.AliasName))
                {
                    return currUser.UserName;
                }
                return currUser.AliasName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 当前用户是否为超级管理员
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static bool IsSuperAdmin(UserInfo currUser)
        {
            return currUser != null && currUser.UserName == "admin";
        }

        /// <summary>
        /// 缓存用户扩展信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="extend">用户扩展信息</param>
        public static void CacheUserExtendInfo(string username, UserExtendBase extend)
        {
            if (!string.IsNullOrEmpty(username) && extend != null)
            {
                ICacheProvider cacheFactory = CacheFactory.GetCacheInstance(CacheProviderType.LOCALMEMORYCACHE);
                if (cacheFactory != null)
                {
                    cacheFactory.Set(username, extend);
                }
            }
        }

        /// <summary>
        /// 获取用户扩展信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public static UserExtendBase GetUserExtendCache(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                ICacheProvider cacheFactory = CacheFactory.GetCacheInstance(CacheProviderType.LOCALMEMORYCACHE);
                if (cacheFactory != null)
                {
                    return cacheFactory.Get<UserExtendBase>(username);
                }
            }
            return null;
        }

        /// <summary>
        /// 移除用户扩展缓存
        /// </summary>
        /// <param name="username">用户名</param>
        public static void RemoveExtendCache(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                ICacheProvider cacheFactory = CacheFactory.GetCacheInstance(CacheProviderType.LOCALMEMORYCACHE);
                if (cacheFactory != null)
                {
                    cacheFactory.Remove(username);
                }
            }
        }

        /// <summary>
        /// 获取当前用户扩展信息
        /// </summary>
        /// <param name="currUser">用户信息</param>
        /// <param name="companyId">公司ID</param>
        /// <returns></returns>
        public static List<EmpExtendInfo> GetCurrEmpExtendInfo(UserInfo currUser, Guid? companyId = null)
        {
            if (currUser != null && currUser.ExtendUserObject != null)
            {
                UserExtendInfo extend = currUser.ExtendUserObject as UserExtendInfo;
                if (extend == null)
                    extend = GetUserExtendCache(currUser.UserName) as UserExtendInfo;
                if (extend != null && extend.EmpExtend != null && extend.EmpExtend.Count > 0)
                {
                    List<EmpExtendInfo> list = extend.EmpExtend;
                    if (list != null && list.Count > 0 && companyId.HasValue && companyId.Value != Guid.Empty)
                    {
                        list = list.Where(x => x.CompanyId == companyId).ToList();
                    }
                    return list;
                }
            }
            return new List<EmpExtendInfo>();
        }

        #endregion

        #region 常量

        /// <summary>
        /// 账号过期时间（分钟）
        /// </summary>
        public const int ACCOUNT_EXPIRATION_TIME = 720;

        #endregion
    }
}

/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Base
{
    /// <summary>
    /// 用户扩展事件处理
    /// </summary>
    public static class UserExtendEventHandler
    {
        /// <summary>
        /// 用户扩展代理
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public delegate UserExtendBase EventUserExtend(object o, EventUserArgs e); //定义有返回值的委托
        /// <summary>
        /// 绑定用户扩展事件
        /// </summary>
        public static event EventUserExtend BindUserExtendEvent; //定义事件

        /// <summary>
        /// 获取用户扩展对象
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        public static UserExtendBase GetUserExtendInfo(UserInfo userInfo)
        {
            if (BindUserExtendEvent != null)
            {
                UserExtendBase extend = BindUserExtendEvent(null, new EventUserArgs(userInfo));//有返回值了
                return extend;
            }
            return null;
        }
    }
}

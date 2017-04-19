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
    /// 用户登录事件参数
    /// </summary>
    public class EventUserArgs : EventArgs
    {
        private UserInfo _currUser;
        /// <summary>
        /// 当前用户
        /// </summary>
        public UserInfo CurrUser
        {
            get { return _currUser; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currUser"></param>
        public EventUserArgs(UserInfo currUser)
        {
            this._currUser = currUser;
        }
    }
}

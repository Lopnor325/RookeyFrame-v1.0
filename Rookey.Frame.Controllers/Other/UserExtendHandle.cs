using Rookey.Frame.Base;
using Rookey.Frame.Base.Set;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Model.OrgM;
using System.Linq;
using System.Collections.Generic;
using Rookey.Frame.Base.User;
using System;

namespace Rookey.Frame.Controllers.Other
{
    /// <summary>
    /// 用户扩展处理类
    /// </summary>
    public static class UserExtendHandle
    {
        /// <summary>
        /// 获取用户扩展信息
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static UserExtendBase GetUserExtendObject(object o, EventUserArgs e)
        {
            if (e.CurrUser != null)
            {
                return UserOperate.GetUserExtend(e.CurrUser);
            }
            return null;
        }
    }
}

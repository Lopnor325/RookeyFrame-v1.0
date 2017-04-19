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
    /// 用户扩展基类
    /// </summary>
    public abstract class UserExtendBase
    {
        /// <summary>
        /// 角色ID集合
        /// </summary>
        public List<Guid?> RoleIds { get; set; }

        /// <summary>
        /// 角色名称集合
        /// </summary>
        public List<string> RoleNames { get; set; }
    }
}

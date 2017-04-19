/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 用户快捷菜单
    /// </summary>
    [NoModule]
    public class Sys_UserQuckMenu : BaseSysEntity
    {
        /// <summary>
        /// 用户
        /// </summary>
        [NoField]
        public Guid? Sys_UserId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [NoField]
        public Guid? Sys_MenuId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [NoField]
        public int Sort { get; set; }
    }
}

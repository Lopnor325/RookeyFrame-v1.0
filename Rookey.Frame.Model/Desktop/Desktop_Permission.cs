/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase.Attr;
using System;

namespace Rookey.Frame.Model.Desktop
{
    /// <summary>
    /// 桌面权限配置
    /// </summary>
    [NoModule]
    public class Desktop_Permission : BaseDeskEntity
    {
        /// <summary>
        /// 权限设置类型，0-角色，1-职务
        /// </summary>
        [NoField]
        public int SetType { get; set; }

        /// <summary>
        /// 类型ID，当SetType=0时为角色ID，当SetType=1时为职务ID
        /// </summary>
        [NoField]
        public Guid? TypeId { get; set; }

        /// <summary>
        /// 桌面项标签
        /// </summary>
        [NoField]
        public Guid? Desktop_ItemId { get; set; }
    }
}

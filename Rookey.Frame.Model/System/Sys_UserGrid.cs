/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 用户视图
    /// </summary>
    [ModuleConfig(Name = "用户视图", PrimaryKeyFields = "Sys_UserId,Sys_GridId", IsAllowAdd = false, IsAllowEdit = false, IsAllowCopy = false, IsEnabledBatchEdit = false, Sort = 8, StandardJsFolder = "System")]
    public class Sys_UserGrid : BaseSysEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [FieldConfig(Display = "用户", IsEnableForm = false, HeadSort = 1, ForeignModuleName = "用户管理")]
        public Guid? Sys_UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Ignore]
        public string Sys_UserName { get; set; }

        /// <summary>
        /// 视图Id
        /// </summary>
        [FieldConfig(Display = "视图", IsEnableForm = false, HeadSort = 2, ForeignModuleName = "视图管理")]
        public Guid? Sys_GridId { get; set; }

        /// <summary>
        /// 视图名称
        /// </summary>
        [Ignore]
        public string Sys_GridName { get; set; }

        /// <summary>
        /// 是否为默认视图
        /// </summary>
        [FieldConfig(Display = "默认视图", IsEnableForm = false, HeadSort = 3)]
        public bool IsDefault { get; set; }
    }
}

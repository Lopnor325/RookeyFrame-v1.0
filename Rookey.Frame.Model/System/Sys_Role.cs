/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 角色
    /// </summary>
    [ModuleConfig(Name = "角色管理", Sort = 2, PrimaryKeyFields = "Name", TitleKey = "Name", StandardJsFolder = "System")]
    public class Sys_Role : BaseSysEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [FieldConfig(Display = "角色名称", IsFrozen = true, RowNum = 1, ColNum = 1, IsUnique = true, IsRequired = true, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 父角色Id
        /// </summary>
        [FieldConfig(Display = "父角色", IsFrozen = true, ControlType = (int)ControlTypeEnum.ComboTree, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "角色管理")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 父角色名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 其他父角色
        /// </summary>
        [FieldConfig(Display = "其他父角色", ControlType = (int)ControlTypeEnum.DialogGrid, IsMultiSelect = true, ControlWidth = 490, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "角色管理")]
        [StringLength(1000)]
        public string OtherParentRoles { get; set; }

        /// <summary>
        /// 其他父角色，名称字段
        /// </summary>
        [Ignore]
        public string OtherParentRolesName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldConfig(Display = "是否有效", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 3, ColNum = 1, HeadSort = 4)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 失效时间
        /// </summary>
        [FieldConfig(Display = "失效时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 3, ColNum = 2, HeadSort = 5)]
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [FieldConfig(Display = "描述", RowNum = 6, ColNum = 1, ControlWidth = 490, HeadSort = 6)]
        [StringLength(300)]
        public string Des { get; set; }
    }
}

/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.OrgM
{
    /// <summary>
    /// 岗位管理
    /// </summary>
    [ModuleConfig(Name = "岗位管理", PrimaryKeyFields = "OrgM_DeptId,OrgM_DutyId", TitleKey = "Name", StandardJsFolder = "OrgM", Sort = 72)]
    public class OrgM_DeptDuty : BaseOrgMEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [FieldConfig(Display = "编码", RowNum = 1, ColNum = 1, IsFrozen = true, IsRequired = true, IsUnique = true, HeadSort = 1)]
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        [FieldConfig(Display = "部门", ControlType = (int)ControlTypeEnum.DialogGrid, IsFrozen = true, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "部门管理")]
        public Guid? OrgM_DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Ignore]
        public string OrgM_DeptName { get; set; }

        /// <summary>
        /// 职务信息
        /// </summary>
        [FieldConfig(Display = "职务", ControlType = (int)ControlTypeEnum.ComboBox, IsFrozen = true, IsRequired = true, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "职务管理")]
        public Guid? OrgM_DutyId { get; set; }

        /// <summary>
        /// 职务名称
        /// </summary>
        [Ignore]
        public string OrgM_DutyName { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        [FieldConfig(Display = "岗位名称", RowNum = 2, ColNum = 2, IsFrozen = true, IsUnique = true, IsRequired = true, HeadSort = 4)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 上级岗位
        /// </summary>
        [FieldConfig(Display = "上级岗位", ControlType = (int)ControlTypeEnum.ComboTree, RowNum = 3, ColNum = 1, HeadSort = 5, ForeignModuleName = "岗位管理")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 上级岗位名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 岗位职责
        /// </summary>
        [FieldConfig(Display = "岗位职责", ControlType = (int)ControlTypeEnum.TextAreaBox, ControlWidth = 490, RowNum = 4, ColNum = 1, HeadSort = 6)]
        [StringLength(2000)]
        public string PositionDuty { get; set; }

        /// <summary>
        /// 岗位编制
        /// </summary>
        [FieldConfig(Display = "岗位编制", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 5, ColNum = 1, HeadSort = 7)]
        public int Establishment { get; set; }

        /// <summary>
        /// 是否部门主管岗位
        /// </summary>
        [FieldConfig(Display = "部门主管岗位", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 5, ColNum = 2, HeadSort = 8)]
        public bool IsDeptCharge { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [FieldConfig(Display = "是否启用", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 6, ColNum = 1, HeadSort = 9)]
        public bool IsValid { get; set; }
    }
}

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

namespace Rookey.Frame.Model.OrgM
{
    /// <summary>
    /// 员工岗位
    /// </summary>
    [ModuleConfig(Name = "员工岗位", PrimaryKeyFields = "OrgM_EmpId,OrgM_DeptId,OrgM_DutyId", TitleKey = "Code", StandardJsFolder = "OrgM", Sort = 74)]
    public class OrgM_EmpDeptDuty : BaseOrgMEntity
    {
        /// <summary>
        /// 员工编码
        /// </summary>
        [FieldConfig(Display = "编码", IsRequired = true, IsUnique = true, IsFrozen = true, RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 员工信息
        /// </summary>
        [FieldConfig(Display = "员工", ControlType = (int)ControlTypeEnum.DialogGrid, IsFrozen = true, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "员工管理")]
        public Guid? OrgM_EmpId { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        [Ignore]
        public string OrgM_EmpName { get; set; }

        /// <summary>
        /// 部门信息
        /// </summary>
        [FieldConfig(Display = "部门", ControlType = (int)ControlTypeEnum.DialogGrid, IsFrozen = true, IsRequired = true, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "部门管理")]
        public Guid? OrgM_DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Ignore]
        public string OrgM_DeptName { get; set; }

        /// <summary>
        /// 职务信息
        /// </summary>
        [FieldConfig(Display = "职务", ControlType = (int)ControlTypeEnum.ComboBox, IsFrozen = true, IsRequired = true, RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "职务管理")]
        public Guid? OrgM_DutyId { get; set; }

        /// <summary>
        /// 职务名称
        /// </summary>
        [Ignore]
        public string OrgM_DutyName { get; set; }

        /// <summary>
        /// 是否主职职务
        /// </summary>
        [FieldConfig(Display = "是否主职岗位", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 3, ColNum = 1, HeadSort = 5)]
        public bool IsMainDuty { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [FieldConfig(Display = "是否启用", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 3, ColNum = 2, HeadSort = 6)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        [FieldConfig(Display = "生效日期", ControlType = (int)ControlTypeEnum.DateBox, RowNum = 4, ColNum = 1, IsRequired = true, HeadSort = 7)]
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        [FieldConfig(Display = "失效日期", ControlType = (int)ControlTypeEnum.DateBox, RowNum = 4, ColNum = 2, HeadSort = 8)]
        public DateTime? InValidDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [FieldConfig(Display = "备注", ControlType = (int)ControlTypeEnum.TextAreaBox, ControlWidth = 490, RowNum = 5, ColNum = 1, HeadSort = 9)]
        [StringLength(2000)]
        public string Memo { get; set; }
    }
}
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
    /// 部门管理
    /// </summary>
    [ModuleConfig(Name = "部门管理", PrimaryKeyFields = "Code", TitleKey = "Name", StandardJsFolder = "OrgM", Sort = 70)]
    public class OrgM_Dept : BaseOrgMEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [FieldConfig(Display = "编码", RowNum = 1, ColNum = 1, IsFrozen = true, IsRequired = true, IsUnique = true, HeadSort = 1)]
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [FieldConfig(Display = "部门名称", RowNum = 1, ColNum = 2, IsFrozen = true, IsUnique = true, IsRequired = true, HeadSort = 2)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        [FieldConfig(Display = "部门简称", IsFrozen = true, RowNum = 2, ColNum = 1, HeadSort = 3)]
        [StringLength(100)]
        public string Alias { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [FieldConfig(Display = "上级部门", ControlType = (int)ControlTypeEnum.ComboTree, IsFrozen = true, RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "部门管理")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 上级部门名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 部门类型
        /// </summary>
        [FieldConfig(Display = "部门类型", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        [StringLength(100)]
        public string DeptType { get; set; }

        /// <summary>
        /// 部门级别
        /// </summary>
        [FieldConfig(Display = "部门级别", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 2, HeadSort = 6)]
        [StringLength(100)]
        public string DeptGrade { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        [FieldConfig(Display = "传真", RowNum = 4, ColNum = 1, HeadSort = 7)]
        [StringLength(100)]
        public string Fax { get; set; }

        /// <summary>
        /// 内线
        /// </summary>
        [FieldConfig(Display = "内线电话", RowNum = 4, ColNum = 2, HeadSort = 8)]
        [StringLength(100)]
        public string InnerPhone { get; set; }

        /// <summary>
        /// 外线
        /// </summary>
        [FieldConfig(Display = "外线电话", RowNum = 5, ColNum = 1, HeadSort = 9)]
        [StringLength(100)]
        public string OuterPhone { get; set; }

        /// <summary>
        /// 部门描述
        /// </summary>
        [FieldConfig(Display = "部门描述", ControlType = (int)ControlTypeEnum.TextAreaBox, RowNum = 6, ColNum = 1, ControlWidth = 490, IsGridVisible = false)]
        [StringLength(2000)]
        public string DeptDes { get; set; }

        /// <summary>
        /// 部门职责
        /// </summary>
        [FieldConfig(Display = "部门职责", ControlType = (int)ControlTypeEnum.TextAreaBox, RowNum = 7, ColNum = 1, ControlWidth = 490, IsGridVisible = false)]
        [StringLength(2000)]
        public string DeptDuty { get; set; }

        /// <summary>
        /// 部门人数
        /// </summary>
        [FieldConfig(Display = "部门人数", ControlType = (int)ControlTypeEnum.IntegerBox, IsEnableForm = false, HeadSort = 10)]
        public int EmpCount { get; set; }

        /// <summary>
        /// 定编人数
        /// </summary>
        [FieldConfig(Display = "定编人数", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 8, ColNum = 1, HeadSort = 11)]
        public int FixedPersons { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [FieldConfig(Display = "是否启用", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 9, ColNum = 1, HeadSort = 12)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        [FieldConfig(Display = "生效日期", ControlType = (int)ControlTypeEnum.DateBox, RowNum = 9, ColNum = 2, IsRequired = true, HeadSort = 13)]
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        [FieldConfig(Display = "失效日期", ControlType = (int)ControlTypeEnum.DateBox, RowNum = 10, ColNum = 1, HeadSort = 14)]
        public DateTime? InValidDate { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [FieldConfig(Display = "序号", ControlType = (int)ControlTypeEnum.IntegerBox, DefaultValue = "0", RowNum = 10, ColNum = 2, HeadSort = 15)]
        public int Sort { get; set; }

        /// <summary>
        /// 是否为公司
        /// </summary>
        [FieldConfig(Display = "是否公司", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 11, ColNum = 1, HeadSort = 16)]
        [StringLength(2000)]
        public bool IsCompany { get; set; }

        /// <summary>
        /// 部门层级
        /// </summary>
        [NoField]
        public int LevelDepth { get; set; }

        /// <summary>
        /// 自定义字段1
        /// </summary>
        [NoField]
        [StringLength(200)]
        public string F1 { get; set; }

        /// <summary>
        /// 自定义字段2
        /// </summary>
        [NoField]
        [StringLength(100)]
        public string F2 { get; set; }

        /// <summary>
        /// 自定义字段3
        /// </summary>
        [NoField]
        [StringLength(100)]
        public string F3 { get; set; }
    }
}

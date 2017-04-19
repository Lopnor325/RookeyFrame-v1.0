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
    /// 职务管理
    /// </summary>
    [ModuleConfig(Name = "职务管理", PrimaryKeyFields = "Code", TitleKey = "Name", StandardJsFolder = "OrgM", Sort = 71)]
    public class OrgM_Duty : BaseOrgMEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [FieldConfig(Display = "编码", IsFrozen = true, IsUnique = true, IsRequired = true, RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [FieldConfig(Display = "职务名称", IsFrozen = true, IsRequired = true, IsUnique = true, RowNum = 1, ColNum = 2, HeadSort = 2)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 职务等级-字典
        /// </summary>
        [FieldConfig(Display = "职务等级", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 2, ColNum = 1, HeadSort = 3)]
        [StringLength(100)]
        public string DutyLevel { get; set; }

        /// <summary>
        /// 职务类型-字典
        /// </summary>
        [FieldConfig(Display = "职务类型", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 2, ColNum = 2, HeadSort = 4)]
        [StringLength(100)]
        public string DutyType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [FieldConfig(Display = "是否启用", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 3, ColNum = 1, HeadSort = 5)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        [FieldConfig(Display = "生效日期", ControlType = (int)ControlTypeEnum.DateBox, RowNum = 3, ColNum = 2, IsRequired = true, HeadSort = 6)]
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        [FieldConfig(Display = "失效日期", ControlType = (int)ControlTypeEnum.DateBox, RowNum = 4, ColNum = 1, HeadSort = 7)]
        public DateTime? InValidDate { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [FieldConfig(Display = "序号", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 4, ColNum = 2, HeadSort = 8)]
        public int Sort { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [FieldConfig(Display = "备注", ControlType = (int)ControlTypeEnum.TextAreaBox, ControlWidth = 490, RowNum = 5, ColNum = 1, HeadSort = 9)]
        [StringLength(2000)]
        public string Memo { get; set; }
    }
}
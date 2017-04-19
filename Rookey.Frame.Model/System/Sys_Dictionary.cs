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

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 数据字典
    /// </summary>
    [ModuleConfig(Name = "数据字典", PrimaryKeyFields = "ClassName,Name", TitleKey = "Name", IsEnabledBatchEdit = true, Sort = 12, StandardJsFolder = "System")]
    public class Sys_Dictionary : BaseSysEntity
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [FieldConfig(Display = "分类名称", IsFrozen = true, ControlType = (int)ControlTypeEnum.ComboBox, Url = "/DataAsync/BindExistsFieldValueData.html?tableName=Sys_Dictionary&fieldName=ClassName", RowNum = 1, ColNum = 1, IsGroupField = true, IsRequired = true, HeadSort = 1)]
        [StringLength(200)]
        public string ClassName { get; set; }

        /// <summary>
        /// 字典文本
        /// </summary>
        [FieldConfig(Display = "字典文本", IsFrozen = true, RowNum = 1, ColNum = 2, IsRequired = true, HeadSort = 2)]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        [FieldConfig(Display = "字典值", RowNum = 2, ColNum = 1, IsRequired = true, DefaultValue = "{Name}", HeadSort = 3)]
        [StringLength(100)]
        public string Value { get; set; }

        /// <summary>
        /// 字典上级
        /// </summary>
        [FieldConfig(Display = "字典上级", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "数据字典")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 上级名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [FieldConfig(Display = "排序", ControlType = (int)ControlTypeEnum.IntegerBox, DefaultValue = "1", RowNum = 3, ColNum = 1, HeadSort = 5)]
        public int Sort { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        [FieldConfig(Display = "是否生效", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "1", RowNum = 3, ColNum = 2, HeadSort = 6)]
        public bool? IsValid { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        [FieldConfig(Display = "是否默认", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 4, ColNum = 1, HeadSort = 7)]
        public bool? IsDefault { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [FieldConfig(Display = "备注", RowNum = 4, ColNum = 2, HeadSort = 8)]
        [StringLength(200)]
        public string Memo { get; set; }
    }
}

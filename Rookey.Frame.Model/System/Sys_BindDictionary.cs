using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 字典绑定
    /// </summary>
    [ModuleConfig(Name = "字典绑定", PrimaryKeyFields = "Sys_ModuleId,FieldName,ClassName", Sort = 13, StandardJsFolder = "System")]
    public class Sys_BindDictionary : BaseSysEntity
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", IsFrozen = true, ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 1, ColNum = 1, HeadSort = 1, IsRequired = true, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        [FieldConfig(Display = "字段名称", IsFrozen = true, ControlType = (int)ControlTypeEnum.ComboBox, ValueField = "Name", TextField = "Display", RowNum = 1, ColNum = 2, IsRequired = true, HeadSort = 2)]
        public string FieldName { get; set; }

        /// <summary>
        /// 字典分类名称
        /// </summary>
        [FieldConfig(Display = "字典分类", ControlType = (int)ControlTypeEnum.ComboBox, Url = "/DataAsync/BindExistsFieldValueData.html?tableName=Sys_Dictionary&fieldName=ClassName", RowNum = 2, ColNum = 1, IsRequired = true, HeadSort = 3)]
        public string ClassName { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        [FieldConfig(Display = "是否生效", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 2, ColNum = 2, HeadSort = 4)]
        public bool? IsValid { get; set; }
    }
}

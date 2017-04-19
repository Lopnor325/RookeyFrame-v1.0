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
    /// 编码规则
    /// </summary>
    [ModuleConfig(Name = "编码规则", PrimaryKeyFields = "Name", TitleKey = "Name", IsAllowCopy = true, Sort = 24, StandardJsFolder = "System")]
    public class Sys_BillCodeRule : BaseSysEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [FieldConfig(Display = "规则名称", IsFrozen = true, RowNum = 1, ColNum = 1, IsRequired = true, IsUnique = true, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 所属模块
        /// </summary>
        [FieldConfig(Display = "模块", IsFrozen = true, ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 1, ColNum = 2, HeadSort = 2, IsRequired = true, Url = "/SystemAsync/LoadBillCodeModule.html", ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        [FieldConfig(Display = "字段名称", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 2, ColNum = 1, ValueField = "Name", TextField = "Display", IsRequired = true, HeadSort = 3)]
        [StringLength(100)]
        public string FieldName { get; set; }

        /// <summary>
        /// 前缀
        /// </summary>
        [FieldConfig(Display = "前缀", RowNum = 2, ColNum = 2, HeadSort = 4)]
        [StringLength(20)]
        public string Prefix { get; set; }

        /// <summary>
        /// 是否启用日期
        /// </summary>
        [FieldConfig(Display = "是否启用日期", RowNum = 3, ColNum = 1, ControlType = (int)ControlTypeEnum.SingleCheckBox, HeadSort = 5)]
        public bool IsEnableDate { get; set; }

        /// <summary>
        /// 日期格式
        /// </summary>
        [FieldConfig(Display = "日期格式", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 2, HeadSort = 6)]
        public int DateFormat { get; set; }

        /// <summary>
        /// 日期格式（枚举类型）
        /// </summary>
        [Ignore]
        public DateFormatEnum DateFormatOfEnum
        {
            get
            {
                return (DateFormatEnum)Enum.Parse(typeof(DateFormatEnum), DateFormat.ToString());
            }
            set { DateFormat = (int)value; }
        }

        /// <summary>
        /// 流水号
        /// </summary>
        [FieldConfig(Display = "流水号", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 4, ColNum = 1, DefaultValue = "1", HeadSort = 7)]
        public int SerialNumber { get; set; }

        /// <summary>
        /// 流水号占位符
        /// </summary>
        [FieldConfig(Display = "流水号占位符", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 4, ColNum = 2, DefaultValue = "0", HeadSort = 8)]
        public int PlaceHolder { get; set; }

        /// <summary>
        /// 流水号长度
        /// </summary>
        [FieldConfig(Display = "流水号长度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 5, ColNum = 1, DefaultValue = "6", HeadSort = 9)]
        public int SNLength { get; set; }

        /// <summary>
        /// 显示格式
        /// </summary>
        [FieldConfig(Display = "显示格式", RowNum = 5, ColNum = 2, HeadSort = 10)]
        [StringLength(50)]
        public string RuleFormat { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [FieldConfig(Display = "备注", ControlWidth = 490, RowNum = 6, ColNum = 1, HeadSort = 11)]
        public string Memo { get; set; }

        /// <summary>
        /// 当前编码
        /// </summary>
        [FieldConfig(Display = "当前编码", RowNum = 7, ColNum = 1, IsAllowAdd = false, IsAllowEdit = false, HeadSort = 12)]
        [StringLength(50)]
        public string CurrCode { get; set; }

        /// <summary>
        /// 下一编码
        /// </summary>
        [FieldConfig(Display = "下一编码", RowNum = 7, ColNum = 2, IsAllowAdd = false, IsAllowEdit = false, HeadSort = 13)]
        [StringLength(50)]
        public string NextCode { get; set; }
    }
}

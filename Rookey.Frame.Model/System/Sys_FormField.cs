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
    /// 表单字段
    /// </summary>
    [ModuleConfig(Name = "表单字段", PrimaryKeyFields = "Sys_FormId,Sys_FieldId", Sort = 11, ParentName = "表单管理", IsEnabledBatchEdit = true, StandardJsFolder = "System")]
    public class Sys_FormField : BaseSysEntity
    {
        #region 基本信息

        /// <summary>
        /// 表单Id
        /// </summary>
        [FieldConfig(Display = "模块表单", IsFrozen = true, GroupName = "基本信息", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 1, ColNum = 1, HeadSort = 1, IsRequired = true, ForeignModuleName = "表单管理")]
        public Guid? Sys_FormId { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        [Ignore]
        public string Sys_FormName { get; set; }

        /// <summary>
        /// 字段Id
        /// </summary>
        [FieldConfig(Display = "模块字段", IsFrozen = true, GroupName = "基本信息", ControlType = (int)ControlTypeEnum.ComboBox, ValueField = "Id", TextField = "Display", Url = "", RowNum = 1, ColNum = 2, HeadSort = 2, IsRequired = true, ForeignModuleName = "字段管理")]
        public Guid? Sys_FieldId { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        [Ignore]
        public string Sys_FieldName { get; set; }

        /// <summary>
        /// 显示名称（冗余字段）
        /// </summary>
        [StringLength(100)]
        [FieldConfig(Display = "显示名称", GroupName = "基本信息", RowNum = 2, ColNum = 1, IsAllowAdd = false, IsAllowEdit = false, HeadSort = 3, IsRequired = true)]
        public string Display { get; set; }

        #endregion

        #region 输入控制

        /// <summary>
        /// 是否允许新增
        /// </summary>
        [FieldConfig(Display = "允许新增", GroupName = "输入控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 1, HeadSort = 4)]
        public bool? IsAllowAdd { get; set; }

        /// <summary>
        /// 是否允许编辑
        /// </summary>
        [FieldConfig(Display = "允许编辑", GroupName = "输入控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 2, HeadSort = 5)]
        public bool? IsAllowEdit { get; set; }

        /// <summary>
        /// 是否允许批量编辑
        /// </summary>
        [FieldConfig(Display = "允许批量编辑", GroupName = "输入控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 4, ColNum = 1, HeadSort = 6)]
        public bool? IsAllowBatchEdit { get; set; }

        /// <summary>
        /// 是否允许复制
        /// </summary>
        [FieldConfig(Display = "允许复制", GroupName = "输入控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 4, ColNum = 2, HeadSort = 7)]
        public bool? IsAllowCopy { get; set; }

        #endregion

        #region 验证信息

        /// <summary>
        /// 是否必填
        /// </summary>
        [FieldConfig(Display = "是否必填", GroupName = "验证信息", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 5, ColNum = 1, HeadSort = 8)]
        public bool? IsRequired { get; set; }

        /// <summary>
        /// 是否唯一
        /// </summary>
        [FieldConfig(Display = "是否唯一", GroupName = "验证信息", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 5, ColNum = 2, HeadSort = 9)]
        public bool? IsUnique { get; set; }

        /// <summary>
        /// 最小字符长度
        /// </summary>
        [FieldConfig(Display = "最小字符长度", ControlType = (int)ControlTypeEnum.IntegerBox, GroupName = "验证信息", RowNum = 6, ColNum = 1, HeadSort = 10)]
        public int? MinCharLen { get; set; }

        /// <summary>
        /// 最大字符长度
        /// </summary>
        [FieldConfig(Display = "最大字符长度", ControlType = (int)ControlTypeEnum.IntegerBox, GroupName = "验证信息", RowNum = 6, ColNum = 2, HeadSort = 11)]
        public int? MaxCharLen { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        [FieldConfig(Display = "最小值", ControlType = (int)ControlTypeEnum.NumberBox, GroupName = "验证信息", RowNum = 7, ColNum = 1, HeadSort = 12)]
        public decimal? MinValue { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        [FieldConfig(Display = "最大值", ControlType = (int)ControlTypeEnum.NumberBox, GroupName = "验证信息", RowNum = 7, ColNum = 2, HeadSort = 13)]
        public decimal? MaxValue { get; set; }

        #endregion

        #region 分组信息

        /// <summary>
        /// 分组名称
        /// </summary>
        [FieldConfig(Display = "分组名称", GroupName = "分组信息", RowNum = 8, ColNum = 1, HeadSort = 14)]
        [StringLength(100)]
        public string GroupName { get; set; }

        /// <summary>
        /// 分组图标
        /// </summary>
        [FieldConfig(Display = "分组图标", GroupName = "分组信息", ControlType = (int)ControlTypeEnum.IconBox, RowNum = 8, ColNum = 2, HeadSort = 15)]
        [StringLength(500)]
        public string GroupIcon { get; set; }

        /// <summary>
        /// 标签页名称
        /// </summary>
        [FieldConfig(Display = "标签页名称", GroupName = "分组信息", RowNum = 9, ColNum = 1, HeadSort = 16)]
        [StringLength(100)]
        public string TabName { get; set; }

        /// <summary>
        /// 标签页图标
        /// </summary>
        [FieldConfig(Display = "标签页图标", GroupName = "分组信息", ControlType = (int)ControlTypeEnum.IconBox, RowNum = 9, ColNum = 2, HeadSort = 17)]
        [StringLength(500)]
        public string TabIcon { get; set; }
        #endregion

        #region 字段控件

        /// <summary>
        /// 控件类型
        /// </summary>
        [FieldConfig(Display = "控件类型", GroupName = "字段控件", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 10, ColNum = 1, HeadSort = 18)]
        public int ControlType { get; set; }

        /// <summary>
        /// 控件类型（枚举）
        /// </summary>
        [Ignore]
        public ControlTypeEnum ControlTypeOfEnum
        {
            get
            {
                return (ControlTypeEnum)Enum.Parse(typeof(ControlTypeEnum), ControlType.ToString());
            }
            set { ControlType = (int)value; }
        }

        /// <summary>
        /// 验证类型
        /// </summary>
        [FieldConfig(Display = "验证类型", GroupName = "字段控件", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 10, ColNum = 2, HeadSort = 19)]
        public int ValidateType { get; set; }

        /// <summary>
        /// 验证类型（枚举）
        /// </summary>
        [Ignore]
        public ValidateTypeEnum ValidateTypeOfEnum
        {
            get
            {
                return (ValidateTypeEnum)Enum.Parse(typeof(ValidateTypeEnum), ValidateType.ToString());
            }
            set { ValidateType = (int)value; }
        }

        /// <summary>
        /// 控件宽度
        /// </summary>
        [FieldConfig(Display = "控件宽度", GroupName = "字段控件", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 11, ColNum = 1, HeadSort = 20)]
        public int? Width { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [FieldConfig(Display = "默认值", GroupName = "字段控件", RowNum = 11, ColNum = 2, HeadSort = 21)]
        [StringLength(100)]
        public string DefaultValue { get; set; }

        /// <summary>
        /// 空文本时提示文字
        /// </summary>
        [FieldConfig(Display = "空文本时提示", GroupName = "字段控件", RowNum = 12, ColNum = 1, ControlWidth = 490, HeadSort = 22)]
        [StringLength(100)]
        public string NullTipText { get; set; }

        #region 弹出框/下拉框/下拉列表/下拉树

        /// <summary>
        /// 值字段名
        /// </summary>
        [FieldConfig(Display = "值字段", GroupName = "弹出框/下拉框/下拉列表/下拉树", RowNum = 13, ColNum = 1, HeadSort = 23)]
        [StringLength(100)]
        public string ValueField { get; set; }

        /// <summary>
        /// 文本字段名
        /// </summary>
        [FieldConfig(Display = "文本字段", GroupName = "弹出框/下拉框/下拉列表/下拉树", RowNum = 13, ColNum = 2, HeadSort = 24)]
        [StringLength(100)]
        public string TextField { get; set; }

        /// <summary>
        /// 数据源（URL或JSON数据）
        /// </summary>
        [FieldConfig(Display = "数据源", NullTipText = "URL或JSON数据或SQL", GroupName = "弹出框/下拉框/下拉列表/下拉树", ControlType = (int)ControlTypeEnum.TextAreaBox, RowNum = 14, ColNum = 1, ControlWidth = 490, HeadSort = 25, HeadWidth = 250)]
        public string UrlOrData { get; set; }

        /// <summary>
        /// 过滤条件
        /// </summary>
        [FieldConfig(Display = "过滤条件", NullTipText = "JSON数据或SQL", GroupName = "弹出框/下拉框/下拉列表/下拉树", ControlType = (int)ControlTypeEnum.TextAreaBox, RowNum = 15, ColNum = 1, ControlWidth = 490, HeadSort = 26)]
        public string FilterCondition { get; set; }

        /// <summary>
        /// 是否多选
        /// </summary>
        [FieldConfig(Display = "是否多选", GroupName = "弹出框/下拉框/下拉列表/下拉树", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 16, ColNum = 1, HeadSort = 27)]
        public bool? IsMultiSelect { get; set; }

        #endregion

        #endregion

        #region 显示信息

        /// <summary>
        /// 所在行
        /// </summary>
        [FieldConfig(Display = "所在行", GroupName = "显示信息", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 17, ColNum = 1, HeadSort = 28)]
        public int RowNo { get; set; }

        /// <summary>
        /// 所在列
        /// </summary>
        [FieldConfig(Display = "所在列", GroupName = "显示信息", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 17, ColNum = 2, HeadSort = 29)]
        public int ColNo { get; set; }

        /// <summary>
        /// 前置图标
        /// </summary>
        [FieldConfig(Display = "前置图标", GroupName = "显示信息", ControlType = (int)ControlTypeEnum.IconBox, RowNum = 18, ColNum = 2, HeadSort = 30)]
        [StringLength(500)]
        public string BeforeIcon { get; set; }

        /// <summary>
        /// 行编辑时行号
        /// </summary>
        [FieldConfig(Display = "行编辑时行号", GroupName = "显示信息", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 18, ColNum = 2, HeadSort = 31)]
        public int RowEditRowNo { get; set; }

        #endregion

        /// <summary>
        /// 临时字段信息
        /// </summary>
        [Ignore]
        public Sys_Field TempSysField { get; set; }

        /// <summary>
        /// 临时字典信息
        /// </summary>
        [Ignore]
        public object TempDics { get; set; }

        /// <summary>
        /// 临时枚举信息
        /// </summary>
        [Ignore]
        public object TempEnums { get; set; }

        /// <summary>
        /// 格式化函数
        /// </summary>
        [Ignore]
        public string FieldFormatter { get; set; }

        /// <summary>
        /// 编辑器函数
        /// </summary>
        [Ignore]
        public string EditorFormatter { get; set; }

    }
}

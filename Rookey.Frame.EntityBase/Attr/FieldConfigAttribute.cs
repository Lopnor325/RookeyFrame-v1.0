/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;

namespace Rookey.Frame.EntityBase.Attr
{
    /// <summary>
    /// 表单字段配置
    /// </summary>
    public class FieldConfigAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FieldConfigAttribute()
        {
            IsAllowAdd = true;
            IsAllowEdit = true;
            IsAllowBatchEdit = false;
            IsAllowCopy = true;
            IsAllowGridSearch = true;
            ControlType = 0;
            ControlWidth = 180;
            MinCharLen = 0;
            MaxCharLen = 0;
            IsRequired = false;
            IsUnique = false;
            RowNum = 0;
            ColNum = 0;

            HeadWidth = 120;
            IsFrozen = false;
            IsGroupField = false;
            IsGridVisible = true;
            HeadSort = 0;

            IsEnableForm = true;
            IsEnableGrid = true;

            FieldLen = 300;
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段类型（针对数据库）
        /// </summary>
        public string FieldType { get; set; }

        /// <summary>
        /// 字段长度（针对数据库）
        /// </summary>
        public int FieldLen { get; set; }

        /// <summary>
        /// 字段显示名称
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// 外键模块名称
        /// </summary>
        public string ForeignModuleName { get; set; }

        #region 表单配置

        /// <summary>
        /// 是否是启用表单字段配置，为否是将不会插入到表单字段表中
        /// </summary>
        public bool IsEnableForm { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        public int ControlType { get; set; }

        /// <summary>
        /// 控件宽度
        /// </summary>
        public int ControlWidth { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 最小字符长度
        /// </summary>
        public int MinCharLen { get; set; }

        /// <summary>
        /// 最大字符长度
        /// </summary>
        public int MaxCharLen { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 是否唯一
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// 是否允许新增
        /// </summary>
        public bool IsAllowAdd { get; set; }

        /// <summary>
        /// 是否允许编辑
        /// </summary>
        public bool IsAllowEdit { get; set; }

        /// <summary>
        /// 是否允许批量编辑
        /// </summary>
        public bool IsAllowBatchEdit { get; set; }

        /// <summary>
        /// 是否允许复制
        /// </summary>
        public bool IsAllowCopy { get; set; }

        /// <summary>
        /// 表单行号,从1开始
        /// </summary>
        public int RowNum { get; set; }

        /// <summary>
        /// 表单列号，从1开始
        /// </summary>
        public int ColNum { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 分组图标
        /// </summary>
        public string GroupIcon { get; set; }

        /// <summary>
        /// 标签页名称
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// 标签页图标
        /// </summary>
        public string TabIcon { get; set; }

        /// <summary>
        /// 空文本时提示文字
        /// </summary>
        public string NullTipText { get; set; }

        /// <summary>
        /// 值字段名
        /// </summary>
        public string ValueField { get; set; }

        /// <summary>
        /// 文本字段名
        /// </summary>
        public string TextField { get; set; }

        /// <summary>
        /// 数据加载Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMultiSelect { get; set; }

        #endregion

        #region 列表配置

        /// <summary>
        /// 是否是启用视图字段配置，为否是将不会插入到视图字段表中
        /// </summary>
        public bool IsEnableGrid { get; set; }

        /// <summary>
        /// 视图字段宽度
        /// </summary>
        public int HeadWidth { get; set; }

        /// <summary>
        /// 是否冻结
        /// </summary>
        public bool IsFrozen { get; set; }

        /// <summary>
        /// 是否分组字段
        /// </summary>
        public bool IsGroupField { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsGridVisible { get; set; }

        /// <summary>
        /// 列头排序编号，从0开始
        /// </summary>
        public int HeadSort { get; set; }

        /// <summary>
        /// 是否允许列表中搜索
        /// </summary>
        public bool IsAllowGridSearch { get; set; }

        #endregion
    }
}

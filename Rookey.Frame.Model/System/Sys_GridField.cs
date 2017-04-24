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
    /// 视图字段类
    /// </summary>
    [ModuleConfig(Name = "视图字段", PrimaryKeyFields = "Sys_GridId,Sys_FieldId", Sort = 7, ParentName = "视图管理", IsEnabledBatchEdit = true, StandardJsFolder = "System")]
    public class Sys_GridField : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_GridField()
        {
            MinWidth = 60;
            Width = 120;
            IsAllowSearch = true;
            IsVisible = true;
            Align = (int)AlignTypeEnum.Center;
        }

        #region 基本信息

        /// <summary>
        /// 列表Id
        /// </summary>
        [FieldConfig(Display = "模块视图", IsFrozen = true, ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 1, ColNum = 1, HeadSort = 1, IsRequired = true, ForeignModuleName = "视图管理")]
        public Guid? Sys_GridId { get; set; }

        /// <summary>
        /// 列表名称
        /// </summary>
        [Ignore]
        public string Sys_GridName { get; set; }

        /// <summary>
        /// 字段Id
        /// </summary>
        [FieldConfig(Display = "模块字段", IsFrozen = true, ControlType = (int)ControlTypeEnum.ComboBox, ValueField = "Id", TextField = "Display", Url = "", RowNum = 1, ColNum = 2, HeadSort = 2, IsRequired = true, ForeignModuleName = "字段管理")]
        public Guid? Sys_FieldId { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        [Ignore]
        public string Sys_FieldName { get; set; }

        /// <summary>
        /// 显示名称（冗余字段）
        /// </summary>
        [FieldConfig(Display = "显示名称", RowNum = 2, ColNum = 1, IsAllowAdd = false, IsAllowEdit = false, HeadSort = 3, IsFrozen = true, IsRequired = true)]
        [StringLength(100)]
        public string Display { get; set; }

        #endregion

        #region 显示控制

        /// <summary>
        /// 视图字段最小宽度
        /// </summary>
        [FieldConfig(Display = "字段最小宽度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 2, HeadSort = 4)]
        public int? MinWidth { get; set; }

        /// <summary>
        /// 视图字段宽度
        /// </summary>
        [FieldConfig(Display = "字段宽度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public int? Width { get; set; }

        /// <summary>
        /// 是否冻结
        /// </summary>
        [FieldConfig(Display = "是否冻结", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 2, HeadSort = 6)]
        public bool IsFrozen { get; set; }

        /// <summary>
        /// 是否分组字段
        /// </summary>
        [FieldConfig(Display = "分组字段", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 4, ColNum = 1, HeadSort = 7)]
        public bool IsGroupField { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        [FieldConfig(Display = "是否可见", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 4, ColNum = 2, HeadSort = 8)]
        public bool IsVisible { get; set; }

        /// <summary>
        /// 列头排序
        /// </summary>
        [FieldConfig(Display = "列头排序", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 5, ColNum = 1, HeadSort = 9)]
        public int Sort { get; set; }

        /// <summary>
        /// 是否允许搜索
        /// </summary>
        [FieldConfig(Display = "允许搜索", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 5, ColNum = 2, HeadSort = 10)]
        public bool IsAllowSearch { get; set; }

        /// <summary>
        /// 是否允许排序
        /// </summary>
        [FieldConfig(Display = "允许排序", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 6, ColNum = 1, HeadSort = 11)]
        public bool IsAllowSort { get; set; }

        /// <summary>
        /// 是否允许隐藏
        /// </summary>
        [FieldConfig(Display = "允许隐藏", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 6, ColNum = 2, HeadSort = 12)]
        public bool IsAllowHide { get; set; }

        /// <summary>
        /// 对齐方式
        /// </summary>
        [FieldConfig(Display = "对齐方式", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 7, ColNum = 1, HeadSort = 13)]
        public int Align { get; set; }

        /// <summary>
        /// 对齐方式（枚举）
        /// </summary>
        [Ignore]
        public AlignTypeEnum AlignOfEnum
        {
            get
            {
                return (AlignTypeEnum)Enum.Parse(typeof(AlignTypeEnum), Align.ToString());
            }
            set { Align = (int)value; }
        }

        /// <summary>
        /// 允许导出
        /// </summary>
        [FieldConfig(Display = "允许导出", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 7, ColNum = 2, HeadSort = 14)]
        public bool IsAllowExport { get; set; }

        /// <summary>
        /// 格式化函数
        /// </summary>
        [FieldConfig(Display = "格式化函数", RowNum = 8, ColNum = 1, HeadSort = 15)]
        [StringLength(4000)]
        public string FieldFormatter { get; set; }

        /// <summary>
        /// 编辑器函数
        /// </summary>
        [FieldConfig(Display = "编辑器函数", RowNum = 8, ColNum = 2, HeadSort = 16)]
        [StringLength(4000)]
        public string EditorFormatter { get; set; }

        #endregion

        /// <summary>
        /// 临时字段信息
        /// </summary>
        [Ignore]
        public Sys_Field TempSysField { get; set; }
    }
}

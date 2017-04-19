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
    /// 视图按钮
    /// </summary>
    [ModuleConfig(Name = "视图按钮", PrimaryKeyFields = "Sys_ModuleId,ButtonText", TitleKey = "ButtonText", Sort = 9, StandardJsFolder = "System")]
    public class Sys_GridButton : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_GridButton()
        {
            OperateButtonType = 0;
            GridButtonLocation = 0;
        }

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
        /// 按钮显示名称
        /// </summary>
        [FieldConfig(Display = "显示名称", IsFrozen = true, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2)]
        [StringLength(100)]
        public string ButtonText { get; set; }

        /// <summary>
        /// 按钮标签属性Id
        /// </summary>
        [FieldConfig(Display = "标签属性Id", RowNum = 2, ColNum = 1, HeadSort = 3)]
        [StringLength(100)]
        public string ButtonTagId { get; set; }

        /// <summary>
        /// 按钮图标
        /// </summary>
        [FieldConfig(Display = "按钮图标", ControlType = (int)ControlTypeEnum.IconBox, RowNum = 2, ColNum = 2, IsAllowGridSearch = false, HeadSort = 4)]
        [StringLength(100)]
        public string ButtonIcon { get; set; }

        /// <summary>
        /// 单击事件
        /// </summary>
        [FieldConfig(Display = "单击事件", RowNum = 3, ColNum = 1, HeadSort = 5)]
        [StringLength(100)]
        public string ClickMethod { get; set; }

        /// <summary>
        /// 是否系统按钮
        /// </summary>
        [FieldConfig(Display = "系统按钮", IsEnableForm = false, HeadSort = 6)]
        public bool? IsSystem { get; set; }

        /// <summary>
        /// 按钮类型
        /// </summary>
        [FieldConfig(Display = "按钮类型", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 2, HeadSort = 7)]
        public int OperateButtonType { get; set; }

        /// <summary>
        /// 按钮类型（枚举）
        /// </summary>
        [Ignore]
        public OperateButtonTypeEnum OperateButtonTypeOfEnum
        {
            get
            {
                return (OperateButtonTypeEnum)Enum.Parse(typeof(OperateButtonTypeEnum), OperateButtonType.ToString());
            }
            set { OperateButtonType = (int)value; }
        }

        /// <summary>
        /// 上级按钮Id
        /// </summary>
        [FieldConfig(Display = "父按钮", ControlType = (int)ControlTypeEnum.ComboBox, Url = "", RowNum = 4, ColNum = 1, IsAllowBatchEdit = false, HeadSort = 8, ForeignModuleName = "视图按钮")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 父按钮名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 视图按钮位置
        /// </summary>
        [FieldConfig(Display = "按钮位置", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 4, ColNum = 2, HeadSort = 9)]
        public int GridButtonLocation { get; set; }

        /// <summary>
        /// 视图按钮位置（枚举）
        /// </summary>
        [Ignore]
        public GridButtonLocationEnum GridButtonLocationOfEnum
        {
            get
            {
                return (GridButtonLocationEnum)Enum.Parse(typeof(GridButtonLocationEnum), GridButtonLocation.ToString());
            }
            set { GridButtonLocation = (int)value; }
        }

        /// <summary>
        /// 排序
        /// </summary>
        [FieldConfig(Display = "排序", ControlType = (int)ControlTypeEnum.IntegerBox, DefaultValue = "1", RowNum = 5, ColNum = 1, HeadSort = 10)]
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [FieldConfig(Display = "是否启用", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 5, ColNum = 2, HeadSort = 11)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 后接分隔符
        /// </summary>
        [FieldConfig(Display = "后接分隔符", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 6, ColNum = 1, HeadSort = 12)]
        public bool AfterSeparator { get; set; }
    }
}

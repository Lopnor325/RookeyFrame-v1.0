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
using System.Collections.Generic;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 表单类
    /// </summary>
    [ModuleConfig(Name = "表单管理", PrimaryKeyFields = "Name,Sys_ModuleId", Sort = 10, TitleKey = "Name", IsAllowAdd = false, IsEnabledBatchEdit = true, StandardJsFolder = "System")]
    public class Sys_Form : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_Form()
        {
            InputWidth = 180;
            LabelWidth = 100;
            SpaceWidth = 40;
            RightToken = "：";
            LabelAlign = (int)AlignTypeEnum.Left;
            InputAlign = (int)AlignTypeEnum.Left;
            ButtonLocationOfEnum = ButtonLocationEnum.Bottom;
        }

        /// <summary>
        /// 表单名称
        /// </summary>
        [FieldConfig(Display = "表单名称", RowNum = 1, ColNum = 1, IsUnique = true, IsRequired = true, HeadSort = 1, IsFrozen = true)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 1, ColNum = 2, HeadSort = 2, IsRequired = true, IsFrozen = true, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 表单宽度
        /// </summary>
        [FieldConfig(Display = "表单宽度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 1, HeadSort = 3)]
        [Default(0)]
        public int? Width { get; set; }

        /// <summary>
        /// 表单高度
        /// </summary>
        [FieldConfig(Display = "表单高度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 2, HeadSort = 4)]
        [Default(0)]
        public int? Height { get; set; }

        /// <summary>
        /// 模块编辑模式
        /// </summary>
        [FieldConfig(Display = "编辑模式", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public int? ModuleEditMode { get; set; }

        /// <summary>
        /// 模块编辑模式（枚举）
        /// </summary>
        [Ignore]
        public ModuleEditModeEnum ModuleEditModeOfEnum
        {
            get
            {
                if (!ModuleEditMode.HasValue) return ModuleEditModeEnum.None;
                return (ModuleEditModeEnum)Enum.Parse(typeof(ModuleEditModeEnum), ModuleEditMode.Value.ToString());
            }
            set { ModuleEditMode = (int)value; }
        }

        /// <summary>
        /// 表单控件宽度
        /// </summary>
        [FieldConfig(Display = "表单控件宽度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 3, ColNum = 2, HeadSort = 6)]
        public int InputWidth { get; set; }

        /// <summary>
        /// 标签宽度
        /// </summary>
        [FieldConfig(Display = "标签宽度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 4, ColNum = 1, HeadSort = 7)]
        public int LabelWidth { get; set; }

        /// <summary>
        /// 间隔宽度
        /// </summary>
        [FieldConfig(Display = "间隔宽度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 4, ColNum = 2, HeadSort = 8)]
        public int SpaceWidth { get; set; }

        /// <summary>
        /// 分隔符
        /// </summary>
        [FieldConfig(Display = "分隔符", RowNum = 5, ColNum = 1, HeadSort = 9)]
        [StringLength(10)]
        public string RightToken { get; set; }

        /// <summary>
        /// 标签对齐方式
        /// </summary>
        [FieldConfig(Display = "标签对齐方式", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 5, ColNum = 2, HeadSort = 10)]
        public int LabelAlign { get; set; }

        /// <summary>
        /// 对齐方式（枚举）
        /// </summary>
        [Ignore]
        public AlignTypeEnum LabelAlignOfEnum
        {
            get
            {
                return (AlignTypeEnum)Enum.Parse(typeof(AlignTypeEnum), LabelAlign.ToString());
            }
            set { LabelAlign = (int)value; }
        }

        /// <summary>
        /// 控件对齐方式
        /// </summary>
        [FieldConfig(Display = "控件对齐方式", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 6, ColNum = 1, HeadSort = 11)]
        public int InputAlign { get; set; }

        /// <summary>
        /// 对齐方式（枚举）
        /// </summary>
        [Ignore]
        public AlignTypeEnum InputAlignOfEnum
        {
            get
            {
                return (AlignTypeEnum)Enum.Parse(typeof(AlignTypeEnum), InputAlign.ToString());
            }
            set { InputAlign = (int)value; }
        }

        /// <summary>
        /// 按钮组位置（top:表单顶部,bottom:表单底部）
        /// </summary>
        [FieldConfig(Display = "按钮组位置", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 6, ColNum = 2, HeadSort = 12)]
        public int? ButtonLocation { get; set; }

        /// <summary>
        /// 按钮组位置（枚举）
        /// </summary>
        [Ignore]
        public ButtonLocationEnum ButtonLocationOfEnum
        {
            get
            {
                if (!ButtonLocation.HasValue) return ButtonLocationEnum.Top;
                return (ButtonLocationEnum)Enum.Parse(typeof(ButtonLocationEnum), ButtonLocation.Value.ToString());
            }
            set { ButtonLocation = (int)value; }
        }

        /// <summary>
        /// 是否默认
        /// </summary>
        [FieldConfig(Display = "是否默认", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 7, ColNum = 1, HeadSort = 13)]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 表单字段集合
        /// </summary>
        [Ignore]
        public List<Sys_FormField> FormFields { get; set; }
    }
}

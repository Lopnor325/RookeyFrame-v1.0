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

namespace Rookey.Frame.Model.Bpm
{
    /// <summary>
    /// 流程按钮
    /// </summary>
    [ModuleConfig(Name = "流程按钮", PrimaryKeyFields = "ButtonText", TitleKey = "ButtonText", Sort = 80, StandardJsFolder = "Bpm")]
    public class Bpm_FlowBtn : BaseBpmEntity
    {
        /// <summary>
        /// 按钮显示名称
        /// </summary>
        [FieldConfig(Display = "显示名称", IsFrozen = true, IsRequired = true, RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(100)]
        public string ButtonText { get; set; }

        /// <summary>
        /// 按钮图标
        /// </summary>
        [FieldConfig(Display = "按钮图标", ControlType = (int)ControlTypeEnum.IconBox, RowNum = 1, ColNum = 2, IsAllowGridSearch = false, HeadSort = 2)]
        [StringLength(100)]
        public string ButtonIcon { get; set; }

        /// <summary>
        /// 单击事件
        /// </summary>
        [FieldConfig(Display = "单击事件", RowNum = 2, ColNum = 1, HeadSort = 3)]
        [StringLength(100)]
        public string ClickMethod { get; set; }

        /// <summary>
        /// 上级按钮Id
        /// </summary>
        [FieldConfig(Display = "父按钮", ControlType = (int)ControlTypeEnum.ComboBox, Url = "", RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "流程按钮")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 按钮类型
        /// </summary>
        [FieldConfig(Display = "按钮类型", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public int ButtonType { get; set; }

        /// <summary>
        /// 按钮类型（枚举）
        /// </summary>
        [Ignore]
        public FlowButtonTypeEnum ButtonTypeOfEnum
        {
            get
            {
                return (FlowButtonTypeEnum)Enum.Parse(typeof(FlowButtonTypeEnum), ButtonType.ToString());
            }
            set { ButtonType = (int)value; }
        }

        /// <summary>
        /// 排序
        /// </summary>
        [FieldConfig(Display = "排序", ControlType = (int)ControlTypeEnum.IntegerBox, DefaultValue = "1", RowNum = 3, ColNum = 2, HeadSort = 6)]
        public int Sort { get; set; }

        /// <summary>
        /// 按钮说明
        /// </summary>
        [FieldConfig(Display = "按钮说明", RowNum = 4, ColNum = 1, HeadSort = 7)]
        [StringLength(500)]
        public string Memo { get; set; }
    }
}

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
    /// 视图管理
    /// </summary>
    [ModuleConfig(Name = "视图管理", PrimaryKeyFields = "Name,Sys_ModuleId", IsAllowAdd = false, Sort = 6, TitleKey = "Name", IsEnabledBatchEdit = true, StandardJsFolder = "System")]
    public class Sys_Grid : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_Grid()
        {
            ShowCheckBox = true;
            GridType = 0;
            ButtonLocation = 0;
            MaxSearchNum = 3;
        }

        /// <summary>
        /// 视图名称
        /// </summary>
        [FieldConfig(Display = "视图名称", IsFrozen = true, IsAllowEdit = false, RowNum = 1, ColNum = 1, IsRequired = true, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", IsFrozen = true, IsAllowEdit = false, ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 1, ColNum = 2, HeadSort = 2, IsRequired = true, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        [FieldConfig(Display = "视图类型", ControlType = (int)ControlTypeEnum.ComboBox, IsEnableForm = false, HeadSort = 3)]
        public int GridType { get; set; }

        /// <summary>
        /// 视图类型（枚举）
        /// </summary>
        [Ignore]
        public GridTypeEnum GridTypeOfEnum
        {
            get
            {
                return (GridTypeEnum)Enum.Parse(typeof(GridTypeEnum), GridType.ToString());
            }
            set { GridType = (int)value; }
        }

        /// <summary>
        /// 最大搜索字段数
        /// </summary>
        [FieldConfig(Display = "最大搜索字段数", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 1, HeadSort = 4)]
        public int MaxSearchNum { get; set; }

        /// <summary>
        /// 是否为默认视图
        /// </summary>
        [FieldConfig(Display = "默认视图", ControlType = (int)ControlTypeEnum.SingleCheckBox, IsEnableForm = false, HeadSort = 5)]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 是否添加过滤行
        /// </summary>
        [FieldConfig(Display = "添加过滤行", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 2, ColNum = 2, HeadSort = 6)]
        public bool? AddFilterRow { get; set; }

        /// <summary>
        /// 是否显示复选框
        /// </summary>
        [FieldConfig(Display = "显示复选框", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 1, HeadSort = 7)]
        public bool? ShowCheckBox { get; set; }

        /// <summary>
        /// 允许多选
        /// </summary>
        [FieldConfig(Display = "允许多选", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 2, HeadSort = 8)]
        public bool? MutiSelect { get; set; }

        /// <summary>
        /// 按钮组位置（top:列表顶部,bottom:列表底部）
        /// </summary>
        [FieldConfig(Display = "按钮组位置", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 4, ColNum = 1, HeadSort = 9)]
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
        /// 左边显示树，右边显示列表的形成树的字段，如员工中的部门
        /// 可以是枚举字段、字典字段、字符串字段、外键字段
        /// 如果是外键字段，则显示外键模块树
        /// </summary>
        [FieldConfig(Display = "树显示字段", RowNum = 4, ColNum = 2, HeadSort = 10)]
        [StringLength(100)]
        public string TreeField { get; set; }

        /// <summary>
        /// 是否为树型网格
        /// </summary>
        [FieldConfig(Display = "是否树型网格", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 5, ColNum = 1, HeadSort = 11)]
        public bool IsTreeGrid { get; set; }

        /// <summary>
        /// 视图字段集合
        /// </summary>
        [Ignore]
        public List<Sys_GridField> GridFields { get; set; }
    }
}

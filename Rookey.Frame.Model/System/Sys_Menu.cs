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
    /// 菜单管理
    /// </summary>
    [ModuleConfig(Name = "菜单管理", Sort = 4, PrimaryKeyFields = "Name", TitleKey = "Name", StandardJsFolder = "System")]
    public class Sys_Menu : BaseSysEntity
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [FieldConfig(Display = "菜单名称", RowNum = 1, ColNum = 1, IsFrozen = true, IsUnique = true, IsRequired = true, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [FieldConfig(Display = "显示名称", RowNum = 1, ColNum = 2, IsFrozen = true, IsRequired = true, DefaultValue = "{Name}", HeadSort = 2)]
        [StringLength(100)]
        public string Display { get; set; }

        /// <summary>
        /// 上级菜单Id
        /// </summary>
        [FieldConfig(Display = "上级菜单", ControlType = (int)ControlTypeEnum.ComboTree, Url = "/SystemAsync/LoadFolderMenuTree.html", RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "菜单管理")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 上级菜单名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [FieldConfig(Display = "排序", ControlType = (int)ControlTypeEnum.IntegerBox, DefaultValue = "1", RowNum = 3, ColNum = 1, HeadSort = 5)]
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [FieldConfig(Display = "是否启用", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 3, ColNum = 2, HeadSort = 6)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 是否是叶子节点
        /// </summary>
        [FieldConfig(Display = "叶子节点", RowNum = 4, ColNum = 1, ControlType = (int)ControlTypeEnum.SingleCheckBox, HeadSort = 7)]
        public bool IsLeaf { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [FieldConfig(Display = "菜单图标", ControlType = (int)ControlTypeEnum.IconBox, RowNum = 4, ColNum = 2, IsAllowGridSearch = false, HeadSort = 8)]
        [StringLength(100)]
        public string Icon { get; set; }

        /// <summary>
        /// 菜单URL
        /// </summary>
        [FieldConfig(Display = "菜单URL", RowNum = 5, ColNum = 1, HeadSort = 9, ControlWidth = 490, HeadWidth = 250)]
        [StringLength(500)]
        public string Url { get; set; }

        /// <summary>
        /// 新窗口打开
        /// </summary>
        [FieldConfig(Display = "新窗口打开", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 6, ColNum = 1, HeadSort = 10)]
        public bool IsNewWinOpen { get; set; }

        /// <summary>
        /// 菜单路径
        /// </summary>
        //public string TreeValuePath { get; set; }
    }
}

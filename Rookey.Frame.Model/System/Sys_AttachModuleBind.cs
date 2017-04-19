/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 附属模块绑定
    /// </summary>
    [ModuleConfig(Name = "附属模块绑定", PrimaryKeyFields = "Sys_UserId,Sys_ModuleId", TitleKey = "ModuleName", Sort = 15, StandardJsFolder = "System")]
    public class Sys_AttachModuleBind : BaseSysEntity
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        [FieldConfig(Display = "模块名称", ControlType = (int)ControlTypeEnum.ComboBox, TextField = "Name", ValueField = "Name", Url = "/SystemAsync/LoadNotDetailModule.html", RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(100)]
        public string ModuleName { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [FieldConfig(Display = "用户", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "用户管理")]
        public Guid? Sys_UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Ignore]
        public string Sys_UserName { get; set; }

        /// <summary>
        /// 附属模块Id
        /// </summary>
        [FieldConfig(Display = "附属模块", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldConfig(Display = "排序", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 2, HeadSort = 4)]
        public int Sort { get; set; }

        /// <summary>
        /// 网格内展开附属模块，针对网格页面
        /// </summary>
        [FieldConfig(Display = "网格内展开查看", NullTipText = "针对网格页面，是否在网格内展开查看附属模块分页数据", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public bool AttachModuleInGrid { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [FieldConfig(Display = "是否启用", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 2, HeadSort = 6)]
        public bool IsValid { get; set; }
    }
}

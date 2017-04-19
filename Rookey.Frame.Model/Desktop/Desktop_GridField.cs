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

namespace Rookey.Frame.Model.Desktop
{
    /// <summary>
    /// 桌面列表配置
    /// </summary>
    [ModuleConfig(Name = "桌面列表配置", PrimaryKeyFields = "Sys_ModuleId,FieidName", TitleKey = "FieidName", Sort = 43, StandardJsFolder = "Desktop")]
    public class Desktop_GridField : BaseDeskEntity
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 1, ColNum = 1, IsRequired = true, HeadSort = 1, IsGroupField = true, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [FieldConfig(Display = "字段名", ControlType = (int)ControlTypeEnum.ComboBox, ValueField = "Sys_FieldName", TextField = "Display", RowNum = 1, ColNum = 2, IsRequired = true, HeadSort = 2)]
        public string FieidName { get; set; }

        /// <summary>
        /// 字段宽度
        /// </summary>
        [FieldConfig(Display = "字段宽度", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 1, HeadSort = 3)]
        public int Width { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldConfig(Display = "排序编号", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 2, HeadSort = 4)]
        public int Sort { get; set; }
    }
}

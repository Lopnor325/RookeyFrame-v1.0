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
    /// 字段管理
    /// </summary>
    [ModuleConfig(Name = "字段管理", PrimaryKeyFields = "Name,Sys_ModuleId", Sort = 5, TitleKey = "Name", IsAllowAdd = false, StandardJsFolder = "System")]
    public class Sys_Field : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_Field()
        {
            Precision = 2;
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        [FieldConfig(Display = "字段名称", RowNum = 1, ColNum = 1, IsFrozen = true, IsAllowAdd = false, IsAllowEdit = false, IsAllowBatchEdit = false, IsAllowCopy = false, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 字段显示名称
        /// </summary>
        [FieldConfig(Display = "显示名称", RowNum = 1, ColNum = 2, IsUnique = true, IsRequired = true, HeadSort = 2)]
        [StringLength(100)]
        public string Display { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 2, ColNum = 1, HeadSort = 3, IsRequired = true, IsGroupField = true, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 外键模块名称
        /// </summary>
        [FieldConfig(Display = "外键模块名称", RowNum = 2, ColNum = 2, HeadSort = 4)]
        [StringLength(100)]
        public string ForeignModuleName { get; set; }

        /// <summary>
        /// 精度值
        /// </summary>
        [FieldConfig(Display = "精度值", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public int? Precision { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        [Ignore]
        [FieldConfig(Display = "Db字段类型", IsAllowAdd = false, IsAllowEdit = false, IsAllowGridSearch = false, RowNum = 3, ColNum = 2, HeadSort = 6)]
        public string DbType { get; set; }

        /// <summary>
        /// Db字段长度
        /// </summary>
        [Ignore]
        [FieldConfig(Display = "Db字段长度", IsAllowAdd = false, IsAllowEdit = false, IsAllowGridSearch = false, RowNum = 4, ColNum = 1, HeadSort = 7)]
        public int DbLen { get; set; }
    }
}

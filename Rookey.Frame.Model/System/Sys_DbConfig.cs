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
    /// 数据库配置
    /// </summary>
    [ModuleConfig(Name = "数据库配置", DataSourceType = (int)ModuleDataSourceType.Other, IsAllowAdd = false, IsAllowDelete = false, Sort = 21, StandardJsFolder = "System")]
    public class Sys_DbConfig : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_DbConfig()
        {
            this.HasLoadForeignNameValue = true;
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        [FieldConfig(Display = "模块名称", IsFrozen = true, IsAllowAdd = false, IsAllowEdit = false, IsAllowBatchEdit = false, RowNum = 1, ColNum = 1, HeadSort = 1)]
        public string ModuleName { get; set; }

        /// <summary>
        /// 是否自动重建索引
        /// </summary>
        [FieldConfig(Display = "自动重建索引", IsAllowAdd = false, ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 1, ColNum = 2, HeadSort = 2)]
        public bool AutoReCreateIndex { get; set; }

        /// <summary>
        /// 重建索引页密度
        /// </summary>
        [FieldConfig(Display = "重建百分比", NullTipText = "逻辑碎片达到多少时重建索引", IsAllowAdd = false, ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 1, HeadSort = 3)]
        public int CreateIndexPageDensity { get; set; }

        /// <summary>
        /// 当前页密度
        /// </summary>
        [FieldConfig(Display = "碎片百分比", IsAllowAdd = false, IsAllowEdit = false, IsAllowBatchEdit = false, ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 2, HeadSort = 4)]
        public int CurrPageDensity { get; set; }

        /// <summary>
        /// 是否自动分区
        /// </summary>
        [FieldConfig(Display = "自动分区", IsAllowAdd = false, ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public bool AutomaticPartition { get; set; }

        /// <summary>
        /// 分区间隔记录数
        /// </summary>
        [FieldConfig(Display = "分区间隔(百万)", IsAllowAdd = false, ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 3, ColNum = 2, HeadWidth = 140, HeadSort = 6)]
        public int PartitionInterval { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        [FieldConfig(Display = "数据库类型", IsAllowAdd = false, ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 4, ColNum = 1, HeadWidth = 150, HeadSort = 7)]
        public int DbType { get; set; }

        /// <summary>
        /// 数据库类型（枚举）
        /// </summary>
        [Ignore]
        public TempDatabaseType DbTypeOfEnum
        {
            get
            {
                return (TempDatabaseType)Enum.Parse(typeof(TempDatabaseType), DbType.ToString());
            }
            set { DbType = (int)value; }
        }

        /// <summary>
        /// 读连接字符串
        /// </summary>
        [FieldConfig(Display = "读连接字符串", IsAllowAdd = false, IsAllowGridSearch = false, ControlWidth = 490, RowNum = 5, ColNum = 1, HeadWidth = 250, HeadSort = 8)]
        public string ReadConnString { get; set; }

        /// <summary>
        /// 写连接字符串
        /// </summary>
        [FieldConfig(Display = "写连接字符串", IsAllowAdd = false, IsAllowGridSearch = false, ControlWidth = 490, RowNum = 6, ColNum = 1, HeadWidth = 250, HeadSort = 9)]
        public string WriteConnString { get; set; }
    }
}

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
    /// 缓存配置
    /// </summary>
    [ModuleConfig(Name = "缓存配置", DataSourceType = (int)ModuleDataSourceType.Other, IsAllowAdd = false, IsAllowDelete = false, Sort = 22, StandardJsFolder = "System")]
    public class Sys_CacheConfig : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_CacheConfig()
        {
            this.HasLoadForeignNameValue = true;
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        [FieldConfig(Display = "模块名称", IsFrozen = true, IsAllowAdd = false, IsAllowEdit = false, IsAllowBatchEdit = false, RowNum = 1, ColNum = 1, HeadSort = 1)]
        public string ModuleName { get; set; }

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        [FieldConfig(Display = "是否启用缓存", IsAllowAdd = false, ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 2, ColNum = 1, HeadSort = 2)]
        public bool IsEnableCache { get; set; }

        /// <summary>
        /// 缓存类型
        /// </summary>
        [FieldConfig(Display = "缓存类型", IsAllowAdd = false, ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 1, HeadWidth = 200, HeadSort = 3)]
        public int CacheType { get; set; }

        /// <summary>
        /// 缓存类型（枚举）
        /// </summary>
        [Ignore]
        public TempCacheProviderType CacheTypeOfEnum
        {
            get
            {
                return (TempCacheProviderType)Enum.Parse(typeof(TempCacheProviderType), CacheType.ToString());
            }
            set { CacheType = (int)value; }
        }
    }
}

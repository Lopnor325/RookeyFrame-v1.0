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
    /// 页面缓存，缓存页面html
    /// </summary>
    [ModuleConfig(Name = "页面缓存", PrimaryKeyFields = "Sys_ModuleId,CacheKey", TitleKey = "CacheKey", IsAllowAdd = false, IsAllowEdit = false, IsEnabledBatchEdit = false, Sort = 26, StandardJsFolder = "System")]
    public class Sys_PageCache : BaseSysEntity
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", IsGroupField = true, ControlType = (int)ControlTypeEnum.ComboBox, ControlWidth = 490, RowNum = 1, ColNum = 1, HeadSort = 1, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 缓存键值
        /// </summary>
        [FieldConfig(Display = "缓存键值", RowNum = 2, ColNum = 1, ControlWidth = 490, HeadSort = 2)]
        [StringLength(500)]
        public string CacheKey { get; set; }

        /// <summary>
        /// 缓存页面类型
        /// </summary>
        [FieldConfig(Display = "缓存页面类型", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 1, ControlWidth = 490, HeadSort = 3)]
        public int CachePageType { get; set; }

        /// <summary>
        /// 缓存页面类型（枚举）
        /// </summary>
        [Ignore]
        public CachePageTypeEnum CachePageTypeOfEnum
        {
            get
            {
                return (CachePageTypeEnum)Enum.Parse(typeof(CachePageTypeEnum), CachePageType.ToString());
            }
            set { CachePageType = (int)value; }
        }


        /// <summary>
        /// 页面html
        /// </summary>
        [FieldConfig(Display = "页面html", ControlType = (int)ControlTypeEnum.RichTextBox, RowNum = 4, ColNum = 1, ControlWidth = 490, IsEnableGrid = false)]
        [StringLength(int.MaxValue)]
        public string PageHtml { get; set; }
    }
}

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
    /// 桌面项标签
    /// </summary>
    [ModuleConfig(Name = "桌面项标签", ParentName = "桌面项管理", PrimaryKeyFields = "Desktop_ItemId,Title", TitleKey = "Title", Sort = 42, StandardJsFolder = "Desktop")]
    public class Desktop_ItemTab : BaseDeskEntity
    {
        /// <summary>
        /// 桌面项
        /// </summary>
        [FieldConfig(Display = "桌面项", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 1, ColNum = 1, HeadSort = 1, ForeignModuleName = "桌面项管理")]
        public Guid? Desktop_ItemId { get; set; }

        /// <summary>
        /// 桌面项名称
        /// </summary>
        [Ignore]
        public string Desktop_ItemName { get; set; }

        /// <summary>
        /// 标签项标题
        /// </summary>
        [FieldConfig(Display = "标签项标题", RowNum = 1, ColNum = 2, IsRequired = true, IsUnique = true, HeadSort = 2)]
        [StringLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 链接Url地址
        /// </summary>
        [FieldConfig(Display = "链接Url地址", RowNum = 2, ColNum = 1, ControlWidth = 490, DefaultValue = "/Page/DesktopGrid.html", HeadWidth = 250, HeadSort = 3)]
        [StringLength(500)]
        public string Url { get; set; }

        /// <summary>
        /// 更多Url
        /// </summary>
        [FieldConfig(Display = "更多Url", RowNum = 3, ColNum = 1, ControlWidth = 490, HeadWidth = 250, HeadSort = 4)]
        [StringLength(500)]
        public string MoreUrl { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldConfig(Display = "排序编号", IsRequired = true, ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 4, ColNum = 1, DefaultValue = "0", HeadSort = 5)]
        public int Sort { get; set; }

        /// <summary>
        /// 标签项描述
        /// </summary>
        [FieldConfig(Display = "标签项描述", RowNum = 4, ColNum = 2, HeadSort = 6)]
        [StringLength(500)]
        public string Des { get; set; }

        /// <summary>
        /// 标识字符
        /// </summary>
        [FieldConfig(Display = "标识字符", IsRequired = true, IsUnique = true, MinCharLen = 2, MaxCharLen = 20, RowNum = 5, ColNum = 1, HeadSort = 7)]
        [StringLength(20)]
        public string Flag { get; set; }
    }
}

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
using System.Collections.Generic;

namespace Rookey.Frame.Model.Desktop
{
    /// <summary>
    /// 桌面项管理
    /// </summary>
    [ModuleConfig(Name = "桌面项管理", PrimaryKeyFields = "Name", TitleKey = "Name", Sort = 41, StandardJsFolder = "Desktop")]
    public class Desktop_Item : BaseDeskEntity
    {
        /// <summary>
        /// 桌面项名称
        /// </summary>
        [FieldConfig(Display = "桌面项名称", RowNum = 1, ColNum = 1, IsRequired = true, IsUnique = true, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldConfig(Display = "排序编号", IsRequired = true, ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 2, ColNum = 1, DefaultValue = "0", HeadSort = 2)]
        public int Sort { get; set; }

        /// <summary>
        /// 是否允许关闭
        /// </summary>
        [FieldConfig(Display = "允许关闭", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 1, HeadSort = 3)]
        public bool IsCanClose { get; set; }

        /// <summary>
        /// 桌面项描述
        /// </summary>
        [FieldConfig(Display = "桌面项描述", ControlType = (int)ControlTypeEnum.TextAreaBox, RowNum = 4, ColNum = 1, HeadSort = 4)]
        [StringLength(500)]
        public string Des { get; set; }

        /// <summary>
        /// 桌面项明细
        /// </summary>
        [Ignore]
        public List<Desktop_ItemTab> Desktop_ItemTabs { get; set; }
    }
}

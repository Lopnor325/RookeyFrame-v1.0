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
    /// 图标管理
    /// </summary>
    [ModuleConfig(Name = "图标管理", PrimaryKeyFields = "StyleClassName", TitleKey = "StyleClassName", Sort = 16, StandardJsFolder = "System")]
    public class Sys_IconManage : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_IconManage()
        {
            IconClass = (int)IconClassTypeEnum.UserUploadIcon;
            IconType = (int)IconTypeEnum.Piex16;
        }

        /// <summary>
        /// 样式类名
        /// </summary>
        [FieldConfig(Display = "样式类名", IsFrozen = true, NullTipText = "不能以icon-开头的样式类名", ControlWidth = 350, RowNum = 1, ColNum = 1, IsRequired = true, IsUnique = true, IsAllowEdit = false, HeadSort = 1)]
        public string StyleClassName { get; set; }

        /// <summary>
        /// 样式图标
        /// </summary>
        [FieldConfig(Display = "样式图标", ControlType = (int)ControlTypeEnum.ImageUpload, ControlWidth = 350, RowNum = 2, ColNum = 1, IsRequired = true, IsAllowEdit = false, HeadWidth = 200, HeadSort = 2)]
        public string IconAddr { get; set; }

        /// <summary>
        /// 图标分类
        /// </summary>
        [FieldConfig(Display = "图标分类", IsEnableForm = false, HeadSort = 3)]
        public int IconClass { get; set; }

        /// <summary>
        /// 图标分类（枚举）
        /// </summary>
        [Ignore]
        public IconClassTypeEnum IconClassOfEnum
        {
            get
            {
                return (IconClassTypeEnum)Enum.Parse(typeof(IconClassTypeEnum), IconClass.ToString());
            }
            set { IconClass = (int)value; }
        }

        /// <summary>
        /// 图标类型
        /// </summary>
        [FieldConfig(Display = "图标类型", IsEnableForm = false, HeadSort = 4)]
        public int IconType { get; set; }

        /// <summary>
        /// 图标类型（枚举）
        /// </summary>
        [Ignore]
        public IconTypeEnum IconTypeOfEnum
        {
            get
            {
                return (IconTypeEnum)Enum.Parse(typeof(IconTypeEnum), IconType.ToString());
            }
            set { IconType = (int)value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        [FieldConfig(Display = "备注", ControlWidth = 350, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public string Memo { get; set; }
    }
}

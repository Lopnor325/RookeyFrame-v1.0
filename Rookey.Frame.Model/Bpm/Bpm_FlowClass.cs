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

namespace Rookey.Frame.Model.Bpm
{
    /// <summary>
    /// 流程分类
    /// </summary>
    [ModuleConfig(Name = "流程分类", PrimaryKeyFields = "Name", TitleKey = "Name", Sort = 70, StandardJsFolder = "Bpm")]
    public class Bpm_FlowClass : BaseBpmEntity
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [FieldConfig(Display = "分类名称", IsUnique = true, IsRequired = true, RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 上级分类
        /// </summary>
        [FieldConfig(Display = "上级分类", ControlType = (int)ControlTypeEnum.ComboTree, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "流程分类")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 上级分类
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 分类描述
        /// </summary>
        [FieldConfig(Display = "分类描述", RowNum = 2, ColNum = 1, ControlWidth = 490, HeadSort = 3)]
        [StringLength(2000)]
        public string Des { get; set; }
    }
}

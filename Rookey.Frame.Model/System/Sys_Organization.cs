using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 组织管理，主要用于数据权限控制
    /// </summary>
    [ModuleConfig(Name = "组织管理", Sort = 0, TitleKey = "Name", PrimaryKeyFields = "Name", StandardJsFolder = "System")]
    public class Sys_Organization : BaseSysEntity
    {
        /// <summary>
        /// 组织名称
        /// </summary>
        [FieldConfig(Display = "组织名称", IsFrozen = true, RowNum = 1, ColNum = 1, IsUnique = true, IsRequired = true, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 上级组织Id
        /// </summary>
        [FieldConfig(Display = "上级组织", IsFrozen = true, ControlType = (int)ControlTypeEnum.ComboTree, RowNum = 2, ColNum = 1, HeadSort = 2, ForeignModuleName = "组织管理")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 上级组织
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 标识，NULL或""－自定义组织，其他为GUID的为对应的部门ID
        /// </summary>
        [FieldConfig(Display = "标识", RowNum = 3, ColNum = 1, HeadSort = 3)]
        [StringLength(50)]
        public string Flag { get; set; }

        /// <summary>
        /// 组织描述
        /// </summary>
        [FieldConfig(Display = "组织描述", RowNum = 4, ColNum = 1, HeadSort = 4)]
        [StringLength(200)]
        public string Des { get; set; }
    }
}

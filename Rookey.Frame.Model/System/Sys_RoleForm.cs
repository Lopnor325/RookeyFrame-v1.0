using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 角色表单
    /// </summary>
    [ModuleConfig(Name = "角色表单", PrimaryKeyFields = "Sys_RoleId,Sys_FormId", Sort = 23, StandardJsFolder = "System")]
    public class Sys_RoleForm : BaseSysEntity
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [FieldConfig(Display = "角色", IsEnableForm = false, HeadSort = 1, ForeignModuleName = "角色管理")]
        public Guid? Sys_RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Ignore]
        public string Sys_RoleName { get; set; }

        /// <summary>
        /// 表单Id
        /// </summary>
        [FieldConfig(Display = "表单", IsEnableForm = false, HeadSort = 2, ForeignModuleName = "表单管理")]
        public Guid? Sys_FormId { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        [Ignore]
        public string Sys_FormName { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", IsEnableForm = false, HeadSort = 3, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }
    }
}

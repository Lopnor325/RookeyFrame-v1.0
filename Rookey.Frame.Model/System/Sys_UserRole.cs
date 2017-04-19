using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 用户角色
    /// </summary>
    [ModuleConfig(Name = "用户角色", Sort = 1, PrimaryKeyFields = "Sys_UserId,Sys_RoleId", StandardJsFolder = "System")]
    public class Sys_UserRole : BaseSysEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [FieldConfig(Display = "用户", RowNum = 1, ColNum = 1, ControlType = (int)ControlTypeEnum.DialogGrid, HeadSort = 1, ForeignModuleName = "用户管理")]
        public Guid? Sys_UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Ignore]
        public string Sys_UserName { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        [FieldConfig(Display = "角色", RowNum = 2, ColNum = 1, ControlType = (int)ControlTypeEnum.DialogGrid, HeadSort = 2, ForeignModuleName = "角色管理")]
        public Guid? Sys_RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Ignore]
        public string Sys_RoleName { get; set; }
    }
}

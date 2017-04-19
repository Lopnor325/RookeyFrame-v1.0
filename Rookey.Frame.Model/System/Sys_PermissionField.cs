using Rookey.Frame.EntityBase.Attr;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 角色字段权限
    /// </summary>
    [NoModule]
    public class Sys_PermissionField:BaseSysEntity
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [NoField]
        public Guid? Sys_RoleId { get; set; }

        /// <summary>
        /// 所属模块
        /// </summary>
        [NoField]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 允许查看的字段，字段以逗号分隔，如Name,Des，为空代表可查看所有字段
        /// </summary>
        [NoField]
        public string CanViewFields { get; set; }

        /// <summary>
        /// 允许新增的字段，字段以逗号分隔，如Name,Des，为空代表可新增所有字段
        /// </summary>
        [NoField]
        public string CanAddFields { get; set; }

        /// <summary>
        /// 允许修改的字段，字段以逗号分隔，如Name,Des，为空代表可修改所有字段
        /// </summary>
        [NoField]
        public string CanEditFields { get; set; }
    }
}

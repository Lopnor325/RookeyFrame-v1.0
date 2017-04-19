using Rookey.Frame.EntityBase.Attr;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 用户数据权限
    /// </summary>
    [NoModule]
    public class Sys_UserPermissionData : BaseSysEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [NoField]
        public Guid? Sys_UserId { get; set; }

        /// <summary>
        /// 所属模块
        /// </summary>
        [NoField]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 允许查看所属组织的数据，组织Id以逗号分隔，如1045，1046，为空则可查看所有
        /// </summary>
        [NoField]
        public string CanViewOrgIds { get; set; }

        /// <summary>
        /// 允许修改所属组织的数据，组织Id以逗号分隔，如1045，1046，为空则可修改所有
        /// </summary>
        [NoField]
        public string CanEditOrgIds { get; set; }

        /// <summary>
        /// 允许删除所属组织的数据，组织Id以逗号分隔，如1045，1046，为空则可删除所有
        /// </summary>
        [NoField]
        public string CanDelOrgIds { get; set; }
    }
}

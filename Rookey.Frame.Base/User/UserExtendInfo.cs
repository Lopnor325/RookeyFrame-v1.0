using Rookey.Frame.Base;
using System;
using System.Collections.Generic;

namespace Rookey.Frame.Base.User
{
    /// <summary>
    /// 员工扩展信息类
    /// </summary>
    public class EmpExtendInfo
    {
        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 主职部门ID
        /// </summary>
        public Guid? DeptId { get; set; }

        /// <summary>
        /// 主职部门
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 主职职务ID
        /// </summary>
        public Guid? DutyId { get; set; }

        /// <summary>
        /// 主职职务
        /// </summary>
        public string DutyName { get; set; }

        /// <summary>
        /// 兼职部门ID集合
        /// </summary>
        public List<Guid> PartimeDeptIds { get; set; }

        /// <summary>
        /// 兼职部门集合
        /// </summary>
        public List<string> PartimeDeptNames { get; set; }

        /// <summary>
        /// 兼职岗位ID集合
        /// </summary>
        public List<Guid> PartimePositionIds { get; set; }

        /// <summary>
        /// 兼职岗位集合
        /// </summary>
        public List<string> PartimePositionNames { get; set; }
    }

    /// <summary>
    /// 用户扩展对象
    /// </summary>
    public class UserExtendInfo : UserExtendBase
    {
        /// <summary>
        /// 员工扩展信息
        /// </summary>
        public List<EmpExtendInfo> EmpExtend { get; set; }
    }
}

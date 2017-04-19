/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Log
{
    /// <summary>
    /// 登录日志
    /// </summary>
    [ModuleConfig(Name = "登录日志", IsAllowAdd = false, IsAllowEdit = false, IsAllowExport = true, Sort = 30, StandardJsFolder = "Log")]
    public class Log_Login : BaseLogEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [FieldConfig(Display = "用户Id", IsEnableForm = false, HeadWidth = 50, HeadSort = 1)]
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// 登录名称
        /// </summary>
        [FieldConfig(Display = "登录名称", IsEnableForm = false, HeadWidth = 70, HeadSort = 2)]
        [StringLength(30)]
        public string LoginName { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        [FieldConfig(Display = "登录时间", IsEnableForm = false, HeadWidth = 150, HeadSort = 3)]
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        [FieldConfig(Display = "登录IP", IsEnableForm = false, HeadWidth = 100, HeadSort = 4)]
        [StringLength(50)]
        public string LoginIp { get; set; }

        /// <summary>
        /// 登录状态
        /// </summary>
        [FieldConfig(Display = "登录状态", IsEnableForm = false, HeadWidth = 60, HeadSort = 5)]
        [StringLength(20)]
        public string LoginStatus { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        [FieldConfig(Display = "失败原因", IsEnableForm = false, HeadWidth = 100, HeadSort = 6)]
        [StringLength(1000)]
        public string FailureReason { get; set; }

        /// <summary>
        /// 登录总次数
        /// </summary>
        [FieldConfig(Display = "登录总次数", IsEnableForm = false, HeadWidth = 60, HeadSort = 7)]
        public int LoginNum { get; set; }
    }
}

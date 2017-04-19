/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Model.Log
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [ModuleConfig(Name = "操作日志", IsAllowAdd = false, IsAllowEdit = false, IsAllowExport = true, Sort = 31, StandardJsFolder = "Log")]
    public class Log_Operate : BaseLogEntity
    {
        /// <summary>
        /// 操作用户Id
        /// </summary>
        [FieldConfig(Display = "用户Id", IsEnableForm = false, HeadWidth = 50, HeadSort = 1)]
        [StringLength(36)]
        public string UserId { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        [FieldConfig(Display = "操作用户", IsEnableForm = false, HeadWidth = 70, HeadSort = 2)]
        [StringLength(30)]
        public string UserAlias { get; set; }

        /// <summary>
        /// 操作模块
        /// </summary>
        [FieldConfig(Display = "操作模块", IsEnableForm = false, HeadWidth = 100, HeadSort = 3)]
        [StringLength(100)]
        public string ModuleName { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [FieldConfig(Display = "操作类型", IsEnableForm = false, HeadWidth = 60, HeadSort = 4)]
        [StringLength(30)]
        public string OperateType { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        [FieldConfig(Display = "操作内容", IsEnableForm = false, HeadWidth = 250, HeadSort = 5)]
        [StringLength(int.MaxValue)]
        public string OperateContent { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [FieldConfig(Display = "操作时间", IsEnableForm = false, HeadWidth = 150, HeadSort = 6)]
        public DateTime OperateTime { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [FieldConfig(Display = "客户端IP", IsEnableForm = false, HeadWidth = 100, HeadSort = 7)]
        [StringLength(50)]
        public string ClientIp { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        [FieldConfig(Display = "操作结果", IsEnableForm = false, HeadWidth = 60, HeadSort = 8)]
        [StringLength(30)]
        public string OperateResult { get; set; }

        /// <summary>
        /// 结果提示
        /// </summary>
        [FieldConfig(Display = "结果提示", IsEnableForm = false, HeadWidth = 200, HeadSort = 9)]
        [StringLength(2000)]
        public string OperateTip { get; set; }
    }
}

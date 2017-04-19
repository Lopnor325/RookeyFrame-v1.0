/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;

namespace Rookey.Frame.Model.Monitor
{
    /// <summary>
    /// 操作时间监控
    /// </summary>
    [ModuleConfig(Name = "操作时间监控", PrimaryKeyFields = "ModuleName,ControllerName,ActionName,CreateDate", Sort = 60, IsAllowAdd = false, IsAllowEdit = false, IsAllowExport = true, StandardJsFolder = "Monitor")]
    public class Monitor_OpExecuteTime : BaseMonitorEntity
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        [FieldConfig(Display = "模块名称", IsEnableForm = false, HeadSort = 1)]
        [StringLength(100)]
        public string ModuleName { get; set; }

        /// <summary>
        /// 控制器名称
        /// </summary>
        [FieldConfig(Display = "控制器名称", IsEnableForm = false, HeadSort = 2)]
        [StringLength(100)]
        public string ControllerName { get; set; }

        /// <summary>
        /// Action名称
        /// </summary>
        [FieldConfig(Display = "Action名称", IsEnableForm = false, HeadSort = 3)]
        [StringLength(100)]
        public string ActionName { get; set; }

        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        [FieldConfig(Display = "执行时间", IsEnableForm = false, HeadSort = 4)]
        public double ExecuteMiniSeconds { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        [FieldConfig(Display = "操作用户", IsEnableForm = false, HeadSort = 5)]
        [StringLength(50)]
        public string OpUserName { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [FieldConfig(Display = "客户端IP", IsEnableForm = false, HeadSort = 6)]
        [StringLength(50)]
        public string ClientIp { get; set; }
    }
}

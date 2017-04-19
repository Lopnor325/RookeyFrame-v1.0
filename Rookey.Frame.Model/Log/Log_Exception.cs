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
    /// 异常日志
    /// </summary>
    [ModuleConfig(Name = "异常日志", IsAllowAdd = false, IsAllowEdit = false, IsAllowExport = true, Sort = 32, StandardJsFolder = "Log")]
    public class Log_Exception : BaseLogEntity
    {
        /// <summary>
        /// 控制器或类名
        /// </summary>
        [FieldConfig(Display = "控制器或类名", IsEnableForm = false, HeadWidth = 120, HeadSort = 1)]
        [StringLength(100)]
        public string ControllerName { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        [FieldConfig(Display = "方法名", IsEnableForm = false, HeadWidth = 100, HeadSort = 2)]
        [StringLength(100)]
        public string ActionName { get; set; }

        /// <summary>
        /// 异常名称
        /// </summary>
        [FieldConfig(Display = "异常名称", IsEnableForm = false, HeadWidth = 100, HeadSort = 3)]
        [StringLength(100)]
        public string ExceptionName { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [FieldConfig(Display = "异常信息", IsEnableForm = false, HeadWidth = 300, HeadSort = 4)]
        [StringLength(int.MaxValue)]
        public string ExceptionMsg { get; set; }

        /// <summary>
        /// 异常源
        /// </summary>
        [FieldConfig(Display = "异常源", IsEnableForm = false, HeadWidth = 100, HeadSort = 5)]
        [StringLength(100)]
        public string ExceptionSource { get; set; }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        [FieldConfig(Display = "堆栈信息", IsEnableForm = false, HeadWidth = 120, HeadSort = 6)]
        [StringLength(2000)]
        public string StackTrace { get; set; }

        /// <summary>
        /// 异常时间
        /// </summary>
        [FieldConfig(Display = "异常时间", IsEnableForm = false, HeadWidth = 150, HeadSort = 7)]
        public DateTime ExceptionTime { get; set; }

        /// <summary>
        /// 参数对象
        /// </summary>
        [FieldConfig(Display = "参数对象", IsEnableForm = false, HeadWidth = 100, HeadSort = 8)]
        [StringLength(500)]
        public string ParamsObj { get; set; }
    }
}

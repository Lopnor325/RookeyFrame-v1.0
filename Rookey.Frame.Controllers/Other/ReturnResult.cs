
using System;
namespace Rookey.Frame.Controllers.Other
{
    /// <summary>
    /// 控制器方法返回结果
    /// </summary>
    public class ReturnResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 登录返回结果
    /// </summary>
    public class LoginReturnResult : ReturnResult
    {
        /// <summary>
        /// 转向url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 是否显示验证码
        /// </summary>
        public bool IsShowCode { get; set; }
    }

    /// <summary>
    /// 保存表单数据返回结果
    /// </summary>
    public class SaveDataReturnResult : ReturnResult
    {
        /// <summary>
        /// 记录Id
        /// </summary>
        public Guid RecordId { get; set; }
    }

    /// <summary>
    /// 更新字段值返回结果
    /// </summary>
    public class UpdateFieldReturnResult : ReturnResult
    {
        /// <summary>
        /// 字段显示值
        /// </summary>
        public string FieldDisplayValue { get; set; }
    }

    /// <summary>
    /// 导出返回结果
    /// </summary>
    public class ExportReturnResult : ReturnResult
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownUrl { get; set; }
    }
}

/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System.ComponentModel;

namespace Rookey.Frame.Model.EnumSpace
{
    /// <summary>
    /// 消息模板类型枚举
    /// </summary>
    public enum MsgTemplateTypeEnum
    {
        /// <summary>
        /// 通用
        /// </summary>
        [Description("通用")]
        Common = 0,

        /// <summary>
        /// 系统
        /// </summary>
        [Description("系统")]
        Sys = 1,

        /// <summary>
        /// 邮件
        /// </summary>
        [Description("邮件")]
        Email = 2,

        /// <summary>
        /// 短信
        /// </summary>
        [Description("短信")]
        Sms = 3
    }

    /// <summary>
    /// 事件通知类型枚举
    /// </summary>
    public enum EventNotifyTypeEnum
    {
        /// <summary>
        /// 邮件通知
        /// </summary>
        [Description("邮件")]
        Email = 1,

        /// <summary>
        /// 系统通知
        /// </summary>
        [Description("系统")]
        Sys = 2,

        /// <summary>
        /// 短信通知
        /// </summary>
        [Description("短信")]
        Sms = 3,

        /// <summary>
        /// 邮件+系统
        /// </summary>
        [Description("邮件+系统")]
        EmailSys = 4,

        /// <summary>
        /// 邮件+短信
        /// </summary>
        [Description("邮件+短信")]
        EmailSms = 5,

        /// <summary>
        /// 系统+短信
        /// </summary>
        [Description("系统+短信")]
        SysSms = 6,

        /// <summary>
        /// 系统+短信
        /// </summary>
        [Description("系统+短信+邮件")]
        SysSmsEmail = 7
    }

    /// <summary>
    /// 收件、抄送、密送范围类型
    /// </summary>
    public enum ReceiverTypeEnum
    {
        /// <summary>
        /// 部门
        /// </summary>
        [Description("部门")]
        Dept = 1,

        /// <summary>
        /// 职务
        /// </summary>
        [Description("职务")]
        Duty = 2,

        /// <summary>
        /// 岗位
        /// </summary>
        [Description("岗位")]
        Position = 3,

        /// <summary>
        /// 人员
        /// </summary>
        [Description("人员")]
        Employee = 4,

        /// <summary>
        /// 角色
        /// </summary>
        [Description("角色")]
        Role = 5
    }
}

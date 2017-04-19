/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Msg
{
    /// <summary>
    /// 消息发送日志
    /// </summary>
    [ModuleConfig(Name = "消息发送日志", IsAllowAdd = false, IsAllowEdit = false, Sort = 95, StandardJsFolder = "Msg")]
    public class Msg_SendLog : BaseMsgEntity
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        [FieldConfig(Display = "事件名称", ControlWidth = 490, RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(100)]
        public string EventName { get; set; }

        /// <summary>
        /// 发送者
        /// </summary>
        [FieldConfig(Display = "发送者", ControlWidth = 490, RowNum = 2, ColNum = 1, HeadSort = 2)]
        [StringLength(50)]
        public string Sender { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        [FieldConfig(Display = "收件人", ControlWidth = 490, ControlType = (int)ControlTypeEnum.TextAreaBox, RowNum = 3, ColNum = 1, HeadSort = 3)]
        [StringLength(2000)]
        public string Tos { get; set; }

        /// <summary>
        /// 抄送人
        /// </summary>
        [FieldConfig(Display = "抄送人", ControlWidth = 490, RowNum = 4, ColNum = 1, HeadSort = 4)]
        [StringLength(2000)]
        public string Ccs { get; set; }

        /// <summary>
        /// 密送人
        /// </summary>
        [FieldConfig(Display = "密送人", ControlWidth = 490, RowNum = 5, ColNum = 1, HeadSort = 5)]
        [StringLength(2000)]
        public string Bccs { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [FieldConfig(Display = "主题", ControlWidth = 490, RowNum = 6, ColNum = 1, HeadSort = 6)]
        [StringLength(500)]
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [FieldConfig(Display = "内容", ControlWidth = 490, ControlType = (int)ControlTypeEnum.TextAreaBox, RowNum = 7, ColNum = 1, HeadSort = 7)]
        [StringLength(int.MaxValue)]
        public string Content { get; set; }

        /// <summary>
        /// 是否发送成功
        /// </summary>
        [FieldConfig(Display = "是否发送成功", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 8, ColNum = 1, HeadSort = 8)]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [FieldConfig(Display = "异常信息", ControlWidth = 490, RowNum = 9, ColNum = 1, HeadSort = 9)]
        [StringLength(2000)]
        public string ErrMsg { get; set; }

        /// <summary>
        /// 发送标识
        /// </summary>
        [FieldConfig(Display = "发送标识", ControlWidth = 490, RowNum = 10, ColNum = 1, HeadSort = 10)]
        [StringLength(200)]
        public string SendFlag { get; set; }
    }
}

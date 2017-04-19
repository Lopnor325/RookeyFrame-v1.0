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
    /// 消息收件人
    /// </summary>
    [ModuleConfig(Name = "消息收件人", ParentName = "事件通知", PrimaryKeyFields = "Msg_EventNotifyId,ReceiverType,ReceiverRange", Sort = 92, StandardJsFolder = "Msg")]
    public class Msg_To : BaseMsgEntity
    {
        /// <summary>
        /// 事件
        /// </summary>
        [FieldConfig(Display = "事件", ControlType = (int)ControlTypeEnum.DialogGrid, IsRequired = true, RowNum = 1, ColNum = 1, HeadSort = 1, ForeignModuleName = "事件通知")]
        public Guid? Msg_EventNotifyId { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        [Ignore]
        public string Msg_EventNotifyName { get; set; }

        /// <summary>
        /// 收件人类型
        /// </summary>
        [FieldConfig(Display = "收件人类型", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2)]
        public int ReceiverType { get; set; }

        /// <summary>
        /// 收件人类型（枚举）
        /// </summary>
        [Ignore]
        public ReceiverTypeEnum ReceiverTypeOfEnum
        {
            get
            {
                return (ReceiverTypeEnum)Enum.Parse(typeof(ReceiverTypeEnum), ReceiverType.ToString());
            }
            set { ReceiverType = (int)value; }
        }

        /// <summary>
        /// 收件人范围
        /// </summary>
        [FieldConfig(Display = "收件人范围", IsRequired = true, ControlType = (int)ControlTypeEnum.TextAreaBox, ControlWidth = 490, RowNum = 2, ColNum = 1, HeadSort = 3, HeadWidth = 250)]
        [StringLength(4000)]
        public string ReceiverRange { get; set; }

        /// <summary>
        /// 其他收件人
        /// </summary>
        [FieldConfig(Display = "其他收件人", ControlType = (int)ControlTypeEnum.TextAreaBox, ControlWidth = 490, RowNum = 3, ColNum = 1, HeadSort = 4, HeadWidth = 250)]
        [StringLength(2000)]
        public string OtherReceiver { get; set; }
    }
}

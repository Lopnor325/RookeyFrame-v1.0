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
    /// 事件通知
    /// </summary>
    [ModuleConfig(Name = "事件通知", PrimaryKeyFields = "Sys_ModuleId,Msg_TemplateId", TitleKey = "Name", Sort = 91, StandardJsFolder = "Msg")]
    public class Msg_EventNotify : BaseMsgEntity
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        [FieldConfig(Display = "事件名称", GroupName = "主信息", IsUnique = true, IsRequired = true, RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", GroupName = "主信息", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 流程结点
        /// </summary>
        [FieldConfig(Display = "流程结点", GroupName = "主信息", ControlWidth = 490, ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 2, ColNum = 1, Url = "/BpmAsync/GetModuleNodes.html", HeadSort = 3, ForeignModuleName = "流程结点")]
        public Guid? Bpm_WorkNodeId { get; set; }

        /// <summary>
        /// 事件类型，新增、编辑、删除、流程发起、审批、拒绝、回退、指派、自定义
        /// </summary>
        [FieldConfig(Display = "事件类型", GroupName = "主信息", ControlWidth = 490, ControlType = (int)ControlTypeEnum.MutiCheckBox, TextField = "新增,编辑,删除,流程发起,流程审批,流程拒绝,流程回退,流程指派,自定义", ValueField = "0,0,0,0,0,0,0,0,0", RowNum = 3, ColNum = 1, HeadSort = 4)]
        [StringLength(100)]
        public string EventType { get; set; }

        /// <summary>
        /// 通知方式
        /// </summary>
        [FieldConfig(Display = "通知方式", GroupName = "主信息", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, DefaultValue = "1", RowNum = 4, ColNum = 1, HeadSort = 5)]
        public int NotifyType { get; set; }

        /// <summary>
        /// 通知方式（枚举）
        /// </summary>
        [Ignore]
        public EventNotifyTypeEnum NotifyTypeOfEnum
        {
            get
            {
                return (EventNotifyTypeEnum)Enum.Parse(typeof(EventNotifyTypeEnum), NotifyType.ToString());
            }
            set { NotifyType = (int)value; }
        }

        /// <summary>
        /// 消息模板
        /// </summary>
        [FieldConfig(Display = "模板", GroupName = "主信息", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 4, ColNum = 2, HeadSort = 6, ForeignModuleName = "消息模板")]
        public Guid? Msg_TemplateId { get; set; }

        /// <summary>
        /// 消息模板名称
        /// </summary>
        [Ignore]
        public string Msg_TemplateName { get; set; }

        /// <summary>
        /// 有效开始时间
        /// </summary>
        [FieldConfig(Display = "有效开始时间", GroupName = "主信息", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 5, ColNum = 1, HeadSort = 7)]
        public DateTime? ValidStartTime { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        [FieldConfig(Display = "有效结束时间", GroupName = "主信息", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 5, ColNum = 2, HeadSort = 8)]
        public DateTime? ValidEndTime { get; set; }

        /// <summary>
        /// 以操作者身份发送
        /// </summary>
        [FieldConfig(Display = "以操作者身份发送", GroupName = "主信息", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 6, ColNum = 1, HeadSort = 9)]
        public bool SendByOper { get; set; }

        /// <summary>
        /// 自定义事件必填标识
        /// </summary>
        [FieldConfig(Display = "标识", GroupName = "主信息", RowNum = 6, ColNum = 2, HeadSort = 10)]
        [StringLength(100)]
        public string Flag { get; set; }
    }
}

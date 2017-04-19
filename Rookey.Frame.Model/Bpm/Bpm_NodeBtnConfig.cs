/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Bpm
{
    /// <summary>
    /// 审批按钮配置
    /// </summary>
    [ModuleConfig(Name = "审批按钮配置", PrimaryKeyFields = "Bpm_WorkFlowId,Bpm_WorkNodeId,Bpm_FlowBtnId", TitleKey = "Code", Sort = 81, StandardJsFolder = "Bpm")]
    public class Bpm_NodeBtnConfig : BaseBpmEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [FieldConfig(Display = "编码", RowNum = 1, ColNum = 1, IsRequired = true, IsAllowAdd = false, IsAllowEdit = false, HeadSort = 1)]
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 流程
        /// </summary>
        [FieldConfig(Display = "流程", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "流程信息")]
        public Guid? Bpm_WorkFlowId { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkFlowName { get; set; }

        /// <summary>
        /// 结点
        /// </summary>
        [FieldConfig(Display = "结点", ControlType = (int)ControlTypeEnum.ComboBox, IsGroupField = true, IsRequired = true, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "流程结点")]
        public Guid? Bpm_WorkNodeId { get; set; }

        /// <summary>
        /// 结点名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkNodeName { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        [FieldConfig(Display = "按钮", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "流程按钮")]
        public Guid? Bpm_FlowBtnId { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        [Ignore]
        public string Bpm_FlowBtnName { get; set; }

        /// <summary>
        /// 按钮显示名称
        /// </summary>
        [FieldConfig(Display = "显示名称", RowNum = 3, ColNum = 1, HeadSort = 5)]
        [StringLength(100)]
        public string BtnDisplay { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [FieldConfig(Display = "是否启用", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 3, ColNum = 2, HeadSort = 6)]
        public bool IsEnabled { get; set; }
    }
}

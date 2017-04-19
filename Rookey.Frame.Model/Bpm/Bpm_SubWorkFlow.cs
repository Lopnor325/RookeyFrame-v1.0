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
    /// 子流程配置
    /// </summary>
    [ModuleConfig(Name = "子流程配置", PrimaryKeyFields = "Bpm_WorkFlowId,Bpm_WorkNodeId,Bpm_WorkFlowChildId", Sort = 72, StandardJsFolder = "Bpm")]
    public class Bpm_SubWorkFlow : BaseBpmEntity
    {
        /// <summary>
        /// 父流程Id
        /// </summary>
        [FieldConfig(Display = "父流程", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 1, ColNum = 1, HeadSort = 1, ForeignModuleName = "流程信息")]
        public Guid? Bpm_WorkFlowId { get; set; }

        /// <summary>
        /// 父流程名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkFlowName { get; set; }

        /// <summary>
        /// 父流程结点Id
        /// </summary>
        [FieldConfig(Display = "父流程结点", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "流程结点")]
        public Guid? Bpm_WorkNodeId { get; set; }

        /// <summary>
        /// 父流程结点名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkNodeName { get; set; }

        /// <summary>
        /// 子流程Id
        /// </summary>
        [FieldConfig(Display = "子流程", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "流程信息")]
        public Guid? Bpm_WorkFlowChildId { get; set; }

        /// <summary>
        /// 子流程名称
        /// </summary>
        [Ignore]
        [NoField]
        public string Bpm_WorkFlowChildName { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [FieldConfig(Display = "备注", RowNum = 2, ColNum = 2, HeadSort = 4)]
        [StringLength(500)]
        public string Memo { get; set; }
    }
}

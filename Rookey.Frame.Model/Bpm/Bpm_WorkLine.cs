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
    /// 流程连线配置
    /// </summary>
    [ModuleConfig(Name = "流程连线", PrimaryKeyFields = "Bpm_WorkFlowId,Bpm_WorkNodeStartId,Bpm_WorkNodeEndId", TitleKey = "Code", Sort = 74, StandardJsFolder = "Bpm")]
    public class Bpm_WorkLine : BaseBpmEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [FieldConfig(Display = "编码", RowNum = 1, ColNum = 1, GroupName = "基本信息", IsRequired = true, IsAllowAdd = false, IsAllowEdit = false, HeadSort = 1)]
        [StringLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// 流程Id
        /// </summary>
        [FieldConfig(Display = "流程", ControlType = (int)ControlTypeEnum.ComboBox, GroupName = "基本信息", IsGroupField = true, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "流程信息")]
        public Guid? Bpm_WorkFlowId { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkFlowName { get; set; }

        /// <summary>
        /// 起始节点
        /// </summary>
        [FieldConfig(Display = "起始结点", ControlType = (int)ControlTypeEnum.ComboBox, GroupName = "基本信息", IsRequired = true, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "流程结点")]
        public Guid? Bpm_WorkNodeStartId { get; set; }

        /// <summary>
        /// 起始节点名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkNodeStartName { get; set; }

        /// <summary>
        /// 终止节点
        /// </summary>
        [FieldConfig(Display = "终止结点", ControlType = (int)ControlTypeEnum.ComboBox, GroupName = "基本信息", IsRequired = true, RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "流程结点")]
        public Guid? Bpm_WorkNodeEndId { get; set; }

        /// <summary>
        /// 终止节点名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkNodeEndName { get; set; }

        /// <summary>
        /// 连线标注
        /// </summary>
        [FieldConfig(Display = "连线标注", GroupName = "基本信息", ControlWidth = 490, RowNum = 3, ColNum = 1, HeadSort = 5)]
        [StringLength(200)]
        public string Note { get; set; }

        /// <summary>
        /// 是否自定义转向条件
        /// </summary>
        [FieldConfig(Display = "自定义条件", ControlType = (int)ControlTypeEnum.SingleCheckBox, GroupName = "转向条件设置", RowNum = 4, ColNum = 1, HeadSort = 6)]
        public bool IsCustomerCondition { get; set; }

        /// <summary>
        /// 表单条件
        /// </summary>
        [FieldConfig(Display = "表单条件", GroupName = "转向条件设置", ControlWidth = 490, RowNum = 5, ColNum = 1, HeadSort = 7)]
        [StringLength(500)]
        public string FormCondition { get; set; }

        /// <summary>
        /// 职务条件
        /// </summary>
        [FieldConfig(Display = "职务条件", GroupName = "转向条件设置", ControlWidth = 490, RowNum = 6, ColNum = 1, HeadSort = 8)]
        [StringLength(4000)]
        public string DutyCondition { get; set; }

        /// <summary>
        /// 部门条件
        /// </summary>
        [FieldConfig(Display = "部门条件", GroupName = "转向条件设置", ControlWidth = 490, RowNum = 7, ColNum = 1, HeadSort = 9)]
        [StringLength(4000)]
        public string DeptCondition { get; set; }

        /// <summary>
        /// SQL条件
        /// </summary>
        [FieldConfig(Display = "SQL条件", GroupName = "转向条件设置", ControlWidth = 490, RowNum = 8, ColNum = 1, HeadSort = 10)]
        [StringLength(4000)]
        public string SqlCondition { get; set; }

        /// <summary>
        /// M
        /// </summary>
        [NoField]
        public float M { get; set; }

        /// <summary>
        /// 连接起始节点
        /// </summary>
        [NoField]
        [StringLength(50)]
        public string FromTagId { get; set; }

        /// <summary>
        /// 连接终止节点
        /// </summary>
        [NoField]
        [StringLength(50)]
        public string ToTagId { get; set; }

        /// <summary>
        /// 标签id
        /// </summary>
        [NoField]
        [StringLength(50)]
        public string TagId { get; set; }

        /// <summary>
        /// 连线类型
        /// </summary>
        [NoField]
        [StringLength(10)]
        public string LineType { get; set; }
    }
}

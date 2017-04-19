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

namespace Rookey.Frame.Model.Bpm
{
    /// <summary>
    /// 待办任务
    /// </summary>
    [ModuleConfig(Name = "待办任务", PrimaryKeyFields = "Bmp_WorkFlowInstanceId,Bmp_WorkNodeInstanceId", TitleKey = "Code", IsAllowAdd = false, IsAllowEdit = false, IsAllowDelete = false, Sort = 79, StandardJsFolder = "Bpm")]
    public class Bpm_WorkToDoList : BaseBpmEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [FieldConfig(Display = "编码", RowNum = 1, ColNum = 1, IsRequired = true, IsAllowAdd = false, IsAllowEdit = false, HeadWidth = 150, HeadSort = 1)]
        [StringLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// 流程标题
        /// </summary>
        [FieldConfig(Display = "标题", RowNum = 2, ColNum = 1, ControlWidth = 490, HeadWidth = 300, HeadSort = 2)]
        [StringLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// 流程实例Id
        /// </summary>
        [FieldConfig(Display = "流程实例", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 3, ColNum = 1, IsEnableGrid = false, ForeignModuleName = "流程实例")]
        public Guid? Bpm_WorkFlowInstanceId { get; set; }

        /// <summary>
        /// 流程实例
        /// </summary>
        [Ignore]
        public string Bpm_WorkFlowInstanceName { get; set; }

        /// <summary>
        /// 结点实例Id
        /// </summary>
        [FieldConfig(Display = "结点实例", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 3, ColNum = 2, IsEnableGrid = false, HeadSort = 4, ForeignModuleName = "结点实例")]
        public Guid? Bpm_WorkNodeInstanceId { get; set; }

        /// <summary>
        /// 结点实例
        /// </summary>
        [Ignore]
        public string Bpm_WorkNodeInstanceName { get; set; }

        /// <summary>
        /// 待办人
        /// </summary>
        [FieldConfig(Display = "待办人", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 4, ColNum = 1, HeadWidth = 80, HeadSort = 5, ForeignModuleName = "员工管理")]
        public Guid? OrgM_EmpId { get; set; }

        /// <summary>
        /// 待办人
        /// </summary>
        [Ignore]
        public string OrgM_EmpName { get; set; }

        /// <summary>
        /// 发起人
        /// </summary>
        [FieldConfig(Display = "发起人", RowNum = 4, ColNum = 2, HeadWidth = 80, HeadSort = 6)]
        [StringLength(50)]
        public string Launcher { get; set; }

        /// <summary>
        /// 发起时间
        /// </summary>
        [FieldConfig(Display = "发起时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 5, ColNum = 1, HeadWidth = 150, HeadSort = 7)]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        [FieldConfig(Display = "接收时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 5, ColNum = 2, HeadSort = 8, IsEnableGrid = false)]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [FieldConfig(Display = "完成时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 6, ColNum = 1, HeadSort = 9, IsEnableGrid = false)]
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [FieldConfig(Display = "流程状态", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 6, ColNum = 2, HeadWidth = 80, HeadSort = 10)]
        public int Status { get; set; }

        /// <summary>
        /// 状态（枚举）
        /// </summary>
        [Ignore]
        public WorkFlowStatusEnum StatusOfEnum
        {
            get
            {
                return (WorkFlowStatusEnum)Enum.Parse(typeof(WorkFlowStatusEnum), Status.ToString());
            }
            set { Status = (int)value; }
        }

        /// <summary>
        /// 处理意见
        /// </summary>
        [FieldConfig(Display = "处理意见", RowNum = 7, ColNum = 1, HeadSort = 11, IsEnableGrid = false)]
        [StringLength(2000)]
        public string ApprovalOpinions { get; set; }

        /// <summary>
        /// 操作动作
        /// </summary>
        [FieldConfig(Display = "操作动作", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 7, ColNum = 2, HeadSort = 12, IsEnableGrid = false)]
        public int WorkAction { get; set; }

        /// <summary>
        /// 操作动作（枚举）
        /// </summary>
        [Ignore]
        public WorkActionEnum WorkActionOfEnum
        {
            get
            {
                return (WorkActionEnum)Enum.Parse(typeof(WorkActionEnum), WorkAction.ToString());
            }
            set { WorkAction = (int)value; }
        }

        /// <summary>
        /// 下一处理结点Id
        /// </summary>
        [FieldConfig(Display = "下一结点", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 8, ColNum = 1, HeadSort = 13, IsEnableGrid = false, ForeignModuleName = "流程结点")]
        public Guid? Bpm_WorkNodeId { get; set; }

        /// <summary>
        /// 下一处理结点名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkNodeName { get; set; }

        /// <summary>
        /// 下一处理人
        /// </summary>
        [FieldConfig(Display = "下一处理人", RowNum = 8, ColNum = 2, HeadSort = 14, IsEnableGrid = false)]
        [StringLength(2000)]
        public string NextNodeHandler { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        [FieldConfig(Display = "模块ID", RowNum = 9, ColNum = 1, IsEnableGrid = false, HeadSort = 15)]
        public Guid ModuleId { get; set; }

        /// <summary>
        /// 记录ID
        /// </summary>
        [FieldConfig(Display = "记录ID", RowNum = 9, ColNum = 2, IsEnableGrid = false, HeadSort = 16)]
        public Guid RecordId { get; set; }

        /// <summary>
        /// 是否原始待办人，找不到待办人时或其他原因系统自动指派给其他人处理待办，这里保存当前待办处理是否原始处理人
        /// </summary>
        [FieldConfig(Display = "是否原始待办人", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 10, ColNum = 1, IsEnableGrid = false, HeadSort = 17)]
        public bool IsInitHandler { get; set; }

        /// <summary>
        /// 父待办ID，针对子流程都有一个父流程待办
        /// </summary>
        [FieldConfig(Display = "父待办ID", RowNum = 10, ColNum = 2, IsEnableGrid = false, HeadSort = 18)]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 当前是否为父待办
        /// </summary>
        [NoField]
        public bool? IsParentTodo { get; set; }

        /// <summary>
        /// 自定义表单URL
        /// </summary>
        [Ignore]
        public string FormUrl { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string ModuleName { get; set; }
    }
}

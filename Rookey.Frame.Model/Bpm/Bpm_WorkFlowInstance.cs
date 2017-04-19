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
    /// 流程实例
    /// </summary>
    [ModuleConfig(Name = "流程实例", PrimaryKeyFields = "Code", TitleKey = "Code", Sort = 76, StandardJsFolder = "Bpm")]
    public class Bpm_WorkFlowInstance : BaseBpmEntity
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        [FieldConfig(Display = "流程编号", RowNum = 1, ColNum = 1, IsRequired = true, IsAllowAdd = false, IsAllowEdit = false, HeadSort = 1)]
        [StringLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// 流程标题
        /// </summary>
        [FieldConfig(Display = "流程标题", RowNum = 1, ColNum = 2, HeadSort = 2)]
        [StringLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 父流程实例Id
        /// </summary>
        [FieldConfig(Display = "父流程实例", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "流程实例")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 父流程实例名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 流程Id
        /// </summary>
        [FieldConfig(Display = "流程信息", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "流程信息")]
        public Guid? Bpm_WorkFlowId { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkFlowName { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [FieldConfig(Display = "流程状态", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public int Status { get; set; }

        /// <summary>
        /// 流程状态（枚举）
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
        /// 发起时间
        /// </summary>
        [FieldConfig(Display = "发起时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 3, ColNum = 2, HeadSort = 6)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 发起者
        /// </summary>
        [FieldConfig(Display = "发起者", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 4, ColNum = 1, HeadSort = 7, ForeignModuleName = "员工管理")]
        public Guid? OrgM_EmpId { get; set; }

        /// <summary>
        /// 发起者
        /// </summary>
        [Ignore]
        public string OrgM_EmpName { get; set; }

        /// <summary>
        /// 记录id
        /// </summary>
        [FieldConfig(Display = "记录Id", ControlType = (int)ControlTypeEnum.TextBox, RowNum = 4, ColNum = 2, HeadSort = 8)]
        public Guid RecordId { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [FieldConfig(Display = "完成时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 5, ColNum = 1, HeadSort = 9)]
        public DateTime? EndDate { get; set; }
    }
}

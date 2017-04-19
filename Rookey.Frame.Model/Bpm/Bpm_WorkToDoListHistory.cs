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
    /// 待办任务历史数据
    /// </summary>
    [NoModule]
    public class Bpm_WorkToDoListHistory : BaseBpmEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [NoField]
        [StringLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// 流程标题
        /// </summary>
        [NoField]
        [StringLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// 流程实例Id
        /// </summary>
        [NoField]
        public Guid? Bpm_WorkFlowInstanceId { get; set; }

        /// <summary>
        /// 结点实例Id
        /// </summary>
        [NoField]
        public Guid? Bpm_WorkNodeInstanceId { get; set; }

        /// <summary>
        /// 待办人
        /// </summary>
        [NoField]
        public Guid? OrgM_EmpId { get; set; }

        /// <summary>
        /// 发起人
        /// </summary>
        [NoField]
        [StringLength(50)]
        public string Launcher { get; set; }

        /// <summary>
        /// 发起时间
        /// </summary>
        [NoField]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        [NoField]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [NoField]
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [NoField]
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
        [NoField]
        [StringLength(2000)]
        public string ApprovalOpinions { get; set; }

        /// <summary>
        /// 操作动作
        /// </summary>
        [NoField]
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
        [NoField]
        public Guid? Bpm_WorkNodeId { get; set; }

        /// <summary>
        /// 下一处理人
        /// </summary>
        [NoField]
        [StringLength(2000)]
        public string NextNodeHandler { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        [NoField]
        public Guid ModuleId { get; set; }

        /// <summary>
        /// 记录ID
        /// </summary>
        [NoField]
        public Guid RecordId { get; set; }

        /// <summary>
        /// 是否原始待办人，找不到待办人时或其他原因系统自动指派给其他人处理待办，这里保存当前待办处理是否原始处理人
        /// </summary>
        [NoField]
        public bool IsInitHandler { get; set; }

        /// <summary>
        /// 父待办ID，针对子流程都有一个父流程待办
        /// </summary>
        [NoField]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 当前是否为父待办
        /// </summary>
        [NoField]
        public bool? IsParentTodo { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string ModuleName { get; set; }
    }
}

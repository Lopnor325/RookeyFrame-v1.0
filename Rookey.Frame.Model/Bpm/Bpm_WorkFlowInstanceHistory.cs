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
    /// 流程实例历史
    /// </summary>
    [NoModule]
    public class Bpm_WorkFlowInstanceHistory : BaseBpmEntity
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        [NoField]
        [StringLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// 流程标题
        /// </summary>
        [NoField]
        [StringLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 父流程实例Id
        /// </summary>
        [NoField]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 流程Id
        /// </summary>
        [NoField]
        public Guid? Bpm_WorkFlowId { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [NoField]
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
        [NoField]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 发起者
        /// </summary>
        [NoField]
        public Guid? OrgM_EmpId { get; set; }

        /// <summary>
        /// 记录id
        /// </summary>
        [NoField]
        public Guid RecordId { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [NoField]
        public DateTime? EndDate { get; set; }
    }
}

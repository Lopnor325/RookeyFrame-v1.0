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
    /// 流程结点实例
    /// </summary>
    [NoModule]
    public class Bpm_WorkNodeInstanceHistory : BaseBpmEntity
    {
        /// <summary>
        /// 流水号
        /// </summary>
        [NoField]
        [StringLength(100)]
        public string SerialNo { get; set; }

        /// <summary>
        /// 流程实例Id
        /// </summary>
        [NoField]
        public Guid? Bpm_WorkFlowInstanceId { get; set; }

        /// <summary>
        /// 结点Id
        /// </summary>
        [NoField]
        public Guid? Bpm_WorkNodeId { get; set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        [NoField]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [NoField]
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [NoField]
        public int Status { get; set; }

        /// <summary>
        /// 状态（枚举）
        /// </summary>
        [Ignore]
        public WorkNodeStatusEnum StatusOfEnum
        {
            get
            {
                return (WorkNodeStatusEnum)Enum.Parse(typeof(WorkNodeStatusEnum), Status.ToString());
            }
            set { Status = (int)value; }
        }
    }
}

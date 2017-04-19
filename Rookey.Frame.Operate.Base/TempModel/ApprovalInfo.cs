using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 审批信息类
    /// </summary>
    public class ApprovalInfo
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public Guid NodeId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public string Handler { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string HandleResult { get; set; }

        /// <summary>
        /// 处理意见
        /// </summary>
        public string ApprovalOpinions { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public string HandleTime { get; set; }

        /// <summary>
        /// 下一节点显示名称
        /// </summary>
        public string NextNodeName { get; set; }

        /// <summary>
        /// 下一节点名称
        /// </summary>
        public string NextName { get; set; }

        /// <summary>
        /// 下一处理人
        /// </summary>
        public string NextHandler { get; set; }
    }
}

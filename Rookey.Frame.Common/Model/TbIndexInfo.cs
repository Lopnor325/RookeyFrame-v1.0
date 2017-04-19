using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Common.Model
{
    /// <summary>
    /// 数据表索引信息
    /// </summary>
    public class TbIndexInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 索引名
        /// </summary>
        public string IndexName { get; set; }
        /// <summary>
        /// 索引类型
        /// </summary>
        public string IndexTypeDes { get; set; }
        /// <summary>
        /// 分区数
        /// </summary>
        public int PartitionNum { get; set; }
        /// <summary>
        /// 逻辑碎片百分比
        /// </summary>
        public int FragmentationPercent { get; set; }
    }
}

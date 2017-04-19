using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 网格过滤规则
    /// </summary>
    class GridFilterRule
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public FilterOpEnum op { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// 过滤操作枚举
    /// </summary>
    enum FilterOpEnum
    {
        /// <summary>
        /// 包含
        /// </summary>
        contains = 12,

        /// <summary>
        /// 等于
        /// </summary>
        equal = 0,

        /// <summary>
        /// 不等于
        /// </summary>
        notequal = 9,

        /// <summary>
        /// 开始于
        /// </summary>
        beginwith = 10,

        /// <summary>
        /// 结束于
        /// </summary>
        endwith = 11,

        /// <summary>
        /// 小于
        /// </summary>
        less = 1,

        /// <summary>
        /// 小于等于
        /// </summary>
        lessorequal = 3,

        /// <summary>
        /// 大于
        /// </summary>
        greater = 2,

        /// <summary>
        /// 大于等于
        /// </summary>
        greaterorequal = 4,

        /// <summary>
        /// 等于空
        /// </summary>
        isnull = 13,

        /// <summary>
        /// 不等于空
        /// </summary>
        isnotnull = 14
    }
}

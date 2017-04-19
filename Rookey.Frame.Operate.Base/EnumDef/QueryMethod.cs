/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.EnumDef
{
    /// <summary>
    /// 表单元素的检索方式
    /// </summary>
    public enum QueryMethod
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Description("等于")]
        Equal = 0,

        /// <summary>
        /// 不等于
        /// </summary>
        [Description("不等于")]
        NotEqual = 9,

        /// <summary>
        /// 小于
        /// </summary>
        [Description("小于")]
        LessThan = 1,

        /// <summary>
        /// 大于
        /// </summary>
        [Description("大于")]
        GreaterThan = 2,

        /// <summary>
        /// 小于等于
        /// </summary>
        [Description("小于等于")]
        LessThanOrEqual = 3,

        /// <summary>
        /// 大于等于
        /// </summary>
        [Description("大于等于")]
        GreaterThanOrEqual = 4,

        /// <summary>
        /// 处理Like的问题
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Description("包含")]
        Contains = 12,

        /// <summary>
        /// 包含于
        /// </summary>
        [Description("包含于")]
        In = 7,

        /// <summary>
        /// Like
        /// </summary>
        [Description("类似于")]
        Like = 6,

        /// <summary>
        /// 开头为
        /// </summary>
        [Description("开头为")]
        StartsWith = 10,

        /// <summary>
        /// 结尾为
        /// </summary>
        [Description("结尾为")]
        EndsWith = 11,

        /// <summary>
        /// 处理In的问题
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Description("StdIn")]
        StdIn = 13,

        /// <summary>
        /// 输入一个时间获取当前天的时间块操作, ToSql未实现，仅实现了IQueryable
        /// </summary>
        [Description("DateBlock")]
        DateBlock = 8,

        /// <summary>
        /// 处理Datetime小于+23h59m59s999f的问题
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Description("时间小于或等于")]
        DateTimeLessThanOrEqual = 14
    }
}

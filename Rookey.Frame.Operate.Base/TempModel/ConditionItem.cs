/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Operate.Base.EnumDef;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 用于存储查询条件的单元
    /// </summary>
    public class ConditionItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConditionItem() { }

        /// <summary>
        /// 带参数构造函数
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="method">条件</param>
        /// <param name="val">值</param>
        public ConditionItem(string field, QueryMethod method, object val)
        {
            Field = field;
            Method = method;
            Value = val;
        }

        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 查询方式，用于标记查询方式HtmlName中使用[]进行标识
        /// </summary>
        public QueryMethod Method { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 如果使用Or组合，则此组组合为一个Or序列
        /// </summary>
        public string OrGroup { get; set; }
    }
}

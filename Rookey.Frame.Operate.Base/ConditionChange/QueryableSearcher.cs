/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Rookey.Frame.Operate.Base.EnumDef;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Operate.Base.ConditionChange.TransformProviders;
using Rookey.Frame.Common;

namespace Rookey.Frame.Operate.Base.ConditionChange
{
    /// <summary>
    /// 查询处理类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryableSearcher<T>
    {
        #region 私有成员

        private List<ITransformProvider> TransformProviders
        {
            get
            {
                return new List<ITransformProvider>
                                     {
                                         new LikeTransformProvider(),
                                         new DateBlockTransformProvider(),
                                         new InTransformProvider(),
                                         new UnixTimeTransformProvider()
                                     };
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="conditionItems">条件集合</param>
        /// <returns>返回Lamda表达式</returns>
        public Expression<Func<T, bool>> GetQueryCondition(List<ConditionItem> conditionItems)
        {
            //构建 c=>Body中的c
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            //构建c=>Body中的Body
            var body = GetExpressoinBody(param, conditionItems);
            //将二者拼为c=>Body
            var expression = Expression.Lambda<Func<T, bool>>(body, param);
            return expression;
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="dicCondition">条件集合</param>
        /// <returns></returns>
        public Expression<Func<T, bool>> GetQueryCondition(Dictionary<string, string> dicCondition)
        {
            if (dicCondition == null || dicCondition.Count == 0) return null;
            List<ConditionItem> list = new List<ConditionItem>();
            foreach (string fieldName in dicCondition.Keys)
            {
                ConditionItem item = new ConditionItem(fieldName, QueryMethod.Equal, dicCondition[fieldName]);
                list.Add(item);
            }
            return GetQueryCondition(list);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取条件表达式
        /// </summary>
        /// <param name="param">条件参数</param>
        /// <param name="items">条件集合</param>
        /// <returns></returns>
        private Expression GetExpressoinBody(ParameterExpression param, ICollection<ConditionItem> items)
        {
            var list = new List<Expression>();
            //OrGroup为空的情况下，即为And组合
            var andList = items.Where(c => string.IsNullOrEmpty(c.OrGroup)).ToArray();
            //将And的子Expression以AndAlso拼接
            if (andList.Count() != 0)
            {
                list.Add(GetGroupExpression(param, andList, Expression.AndAlso));
            }
            //其它的则为Or关系，不同Or组间以And分隔
            var orGroupByList = items.Where(c => !string.IsNullOrEmpty(c.OrGroup)).GroupBy(c => c.OrGroup);
            //拼接子Expression的Or关系
            var collection = orGroupByList
                .Where(grouping => grouping.Count() != 0)
                .Select(grouping => GetGroupExpression(param, grouping, Expression.OrElse));
            list.AddRange(collection);
            //将这些Expression再以And相连
            return list.Aggregate(Expression.AndAlso);
        }

        private Expression GetGroupExpression(ParameterExpression param, IEnumerable<ConditionItem> items, Func<Expression, Expression, Expression> func)
        {
            //获取最小的判断表达式
            var list = items.Select(item => GetExpression(param, item));
            //再以逻辑运算符相连
            return list.Aggregate(func);
        }

        private Expression GetExpression(ParameterExpression param, ConditionItem item)
        {
            //属性表达式
            LambdaExpression exp = GetPropertyLambdaExpression(item, param);
            if (item.Value != null)
            {
                //如果有特殊类型处理，则进行处理，暂时不关注
                foreach (var provider in TransformProviders)
                {
                    if (provider.Match(item, exp.Body.Type))
                    {
                        return GetGroupExpression(param, provider.Transform(item, exp.Body.Type), Expression.AndAlso);
                    }
                }
            }
            //常量表达式
            var constant = ChangeTypeToExpression(item, exp.Body.Type);
            //以判断符或方法连接

            return QueryMethodExpression.Dictionary[item.Method](exp.Body, constant);
        }

        static private LambdaExpression GetPropertyLambdaExpression(ConditionItem item, ParameterExpression param)
        {
            //获取每级属性如c.Users.Proiles.UserId
            var props = item.Field.Split('.');
            Expression propertyAccess = param;
            var typeOfProp = typeof(T);
            int i = 0;
            do
            {
                PropertyInfo property = typeOfProp.GetProperty(props[i]);
                if (property == null) return null;
                typeOfProp = TypeUtil.GetUnNullableTypeToString(property.PropertyType);
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                i++;
            } while (i < props.Length);

            return Expression.Lambda(propertyAccess, param);
        }

        #region ChangeType

        /// <summary>
        /// 类型转换，支持非空类型与可空类型之间的转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        private static object ChangeType(object value, Type conversionType)
        {
            if (value == null) return null;
            if (conversionType.ToString() == "System.Nullable`1[System.Guid]")
            {
                return new Guid(value.ToString());
            }
            return Convert.ChangeType(value, TypeUtil.GetUnNullableType(conversionType), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 转换SearchItem中的Value的类型，为表达式树
        /// </summary>
        /// <param name="item"></param>
        /// <param name="conversionType">目标类型</param>
        private static Expression ChangeTypeToExpression(ConditionItem item, Type conversionType)
        {
            if (item == null) return Expression.Constant(item.Value, conversionType);
            #region 数组
            if (item.Method == QueryMethod.StdIn)
            {
                var arr = (item.Value as Array);
                var expList = new List<Expression>();
                //确保可用
                if (arr != null)
                {
                    var collection =
                        arr.Cast<object>()
                            .Select((t, i) => ChangeType(arr.GetValue(i), conversionType))
                            .Select(c => Expression.Constant(c, conversionType));
                    expList.AddRange(collection);
                }
                //构造inType类型的数组表达式树，并为数组赋初值
                return Expression.NewArrayInit(conversionType, expList);
            }

            #endregion

            var elementType = TypeUtil.GetUnNullableType(conversionType);
            //var value = Convert.ChangeType(item.Value, elementType, CultureInfo.CurrentCulture);
            var value = TypeUtil.ChangeType(item.Value, elementType);

            return Expression.Constant(value, conversionType);
        }

        #endregion

        #endregion
    }
}

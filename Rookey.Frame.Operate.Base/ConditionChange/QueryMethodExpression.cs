/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Operate.Base.EnumDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rookey.Frame.Operate.Base.ConditionChange
{


    public static class QueryMethodExpression
    {
        public static Dictionary<QueryMethod, Func<Expression, Expression, Expression>> _dict;

        public static Dictionary<QueryMethod, Func<Expression, Expression, Expression>> Dictionary
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<QueryMethod, Func<Expression, Expression, Expression>>();
                    _dict.Clear();
                    _dict.Add(QueryMethod.LessThan, Expression.LessThan);
                    _dict.Add(QueryMethod.LessThanOrEqual, Expression.LessThanOrEqual);
                    _dict.Add(QueryMethod.Contains, (left, right) =>
                                                        {
                                                            if (left.Type != typeof (string)) return null;
                                                            return
                                                                Expression.Call(left,
                                                                                typeof(string).GetMethod("Contains"),
                                                                                new[] {right});
                                                        });
                    _dict.Add(QueryMethod.Like, (left, right) =>
                                                    {
                                                        if (left.Type != typeof (string)) return null;

                                                        return Expression.Call(left,
                                                                               typeof (string).GetMethod("Contains"),
                                                                               new[] {right});
                                                    });
                    _dict.Add(QueryMethod.StdIn, (left, right) =>
                                                     {
                                                         if (!right.Type.IsArray) return null;
                                                         //调用Enumerable.Contains扩展方法
                                                         MethodCallExpression resultExp =
                                                             Expression.Call(
                                                                 typeof (Enumerable),
                                                                 "Contains",
                                                                 new[] {left.Type},
                                                                 right,
                                                                 left);

                                                         return resultExp;
                                                     });
                    _dict.Add(QueryMethod.NotEqual, Expression.NotEqual);
                    _dict.Add(QueryMethod.StartsWith, (left, right) =>
                                                          {
                                                              if (left.Type != typeof (string)) return null;
                                                              return Expression.Call(left,
                                                                                     typeof (string).GetMethod(
                                                                                         "StartsWith",
                                                                                         new[] {typeof (string)}),
                                                                                     new[]{right});

                                                          });
                    _dict.Add(QueryMethod.EndsWith, (left, right) =>
                                                        {
                                                            if (left.Type != typeof (string)) return null;
                                                            return Expression.Call(
                                                                left,
                                                                typeof (string)
                                                                    .GetMethod("EndsWith", new[] {typeof (string)}),
                                                                new[] {right});
                                                        });
                    _dict.Add(QueryMethod.DateTimeLessThanOrEqual, Expression.LessThanOrEqual);
                    _dict.Add(QueryMethod.GreaterThanOrEqual, Expression.GreaterThanOrEqual);
                    _dict.Add(QueryMethod.GreaterThan, Expression.GreaterThan);
                    _dict.Add(QueryMethod.Equal, Expression.Equal);
                }
                return _dict;
            }
        }
    }
}
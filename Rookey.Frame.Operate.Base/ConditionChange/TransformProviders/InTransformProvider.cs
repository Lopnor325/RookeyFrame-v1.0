/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Operate.Base.EnumDef;
using Rookey.Frame.Operate.Base.TempModel;
using System;
using System.Collections.Generic;

namespace Rookey.Frame.Operate.Base.ConditionChange.TransformProviders
{
    internal class InTransformProvider : ITransformProvider
    {
        public bool Match(ConditionItem item, Type type)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            return item.Method == QueryMethod.In;
        }

        public IEnumerable<ConditionItem> Transform(ConditionItem item, Type type)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            var arr = (item.Value as Array);
            if (arr == null)
            {
                var arrStr = string.Empty;
                if (item.Value.GetType().ToString() == "System.Collections.Generic.List`1[System.Guid]")
                {
                    foreach (Guid id in item.Value as List<Guid>)
                    {
                        if (arrStr == string.Empty)
                        {
                            arrStr += id.ToString();
                        }
                        else
                        {
                            arrStr += "," + id.ToString();
                        }
                    }
                }
                else
                {
                    arrStr = item.Value.ToString();
                }
                if (!string.IsNullOrEmpty(arrStr))
                {
                    arr = arrStr.Split(',');
                }
            }
            return new[] { new ConditionItem(item.Field, QueryMethod.StdIn, arr) };
        }
    }
}
/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using Rookey.Frame.Common;

namespace Rookey.Frame.Operate.Base.Extension
{
    /// <summary>
    /// 扩展JS转换类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtendedJavaScriptConverter<T> : JavaScriptConverter where T : new()
    {
        private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new[] { typeof(T) };
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            T p = new T();
            var props = typeof(T).GetProperties();
            foreach (string key in dictionary.Keys)
            {
                var prop = props.Where(t => t.Name == key).FirstOrDefault();
                if (prop != null)
                {
                    if (prop.PropertyType == typeof(DateTime))
                    {
                        prop.SetValue2(p, DateTime.ParseExact(dictionary[key] as string, _dateFormat, DateTimeFormatInfo.InvariantInfo), null);
                    }
                    else
                    {
                        prop.SetValue2(p, dictionary[key], null);
                    }
                }
            }
            return p;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            T p = (T)obj;
            IDictionary<string, object> serialized = new Dictionary<string, object>();
            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (pi.PropertyType == typeof(DateTime))
                {
                    serialized[pi.Name] = ((DateTime)pi.GetValue2(p, null)).ToString(_dateFormat);
                }
                else
                {
                    serialized[pi.Name] = pi.GetValue2(p, null);
                }
            }
            return serialized;
        }

        public static JavaScriptSerializer GetSerializer()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new ExtendedJavaScriptConverter<T>() });

            return serializer;
        }
    }
}

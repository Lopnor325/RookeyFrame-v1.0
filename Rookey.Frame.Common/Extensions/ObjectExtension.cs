using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 对象扩展类
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 将整形转换为指定枚举类型的显示输出
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <param name="defaultText">未在枚举中找到此枚举值时显示的文本</param>
        /// <returns>枚举显示</returns>
        public static string ToDisplay<TEnum>(this object value, string defaultText = "")
        {
            if (value == null) return defaultText;
            return EnumHelper.GetDisplayName<TEnum>(value.ToString(), defaultText);
        }

        /// <summary>
        /// 将来多个对象合并成一个字典输出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="overrideProperty"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this object value, bool overrideProperty, params object[] objects)
        {
            var dict = value.ToDictionary();
            dict.Concat(overrideProperty, objects);
            return dict;
        }

        /// <summary>
        /// 将对象所有属性转换为一个字典
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns>对象属性字典</returns>
        public static Dictionary<string, object> ToDictionary(this object value)
        {
            var dict = new Dictionary<string, object>();
            if (value == null) return dict;
            var properties = TypeDescriptor.GetProperties(value);
            foreach (PropertyDescriptor p in properties)
            {
                var obj = p.GetValue(value);
                dict.Add(p.Name, obj);
            }
            return dict;
        }

        /// <summary>
        /// 检查输入对象值是否为空，为空返回默认值，否则将对象转换为输入类型
        /// </summary>
        /// <typeparam name="TFrom">源类型</typeparam>
        /// <typeparam name="TTo">目标类型</typeparam>
        /// <param name="from">待转换值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回值</returns>
        public static TTo To<TFrom, TTo>(this TFrom from, TTo defaultValue)
        {
            if (from.Equals(null)) return defaultValue;
            try
            {

                var type = typeof(TTo);

                //如目标类型为可空类型，则获取其基础类型作为转换目标类型
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                //如果目标类型为枚举，则使用枚举格式化方法
                if (type.IsEnum) return (TTo)Enum.Parse(type, from.ToString());
                return (TTo)Convert.ChangeType(from, type);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 检查输入对象值是否为空，为空返回default(TTo)，否则将对象转换为输入类型
        /// </summary>
        /// <typeparam name="TFrom">源类型</typeparam>
        /// <typeparam name="TTo">目标类型</typeparam>
        /// <param name="from">待转换值</param>
        /// <returns>返回值</returns>
        public static TTo To<TFrom, TTo>(this TFrom from)
        {
            return To(from, default(TTo));
        }

        /// <summary>
        /// 检查输入对象值是否为空，为空返回默认值，否则将对象转换为输入类型
        /// </summary>
        /// <typeparam name="TTo">类型</typeparam>
        /// <param name="from">待转换值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回值</returns>
        public static TTo To<TTo>(this object from, TTo defaultValue)
        {
            return To<object, TTo>(from, defaultValue);
        }

        /// <summary>
        /// 检查输入对象值是否为空，为空返回默认值，否则将对象转换为输入类型
        /// </summary>
        /// <typeparam name="TTo">类型</typeparam>
        /// <param name="from">待转换值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回值</returns>
        public static TTo To<TTo>(this string from, TTo defaultValue)
        {
            return To<string, TTo>(from, defaultValue);
        }

        /// <summary>
        /// 检查输入对象值是否为空，为空返回default(TTo)，否则将对象转换为输入类型
        /// </summary>
        /// <typeparam name="TTo">类型</typeparam>
        /// <param name="from">待转换值</param>
        /// <returns>返回值</returns>
        public static TTo To<TTo>(this object from)
        {
            return To(from, default(TTo));
        }

        /// <summary>
        /// 检查输入字符串是否为空，为空返回default(TTo)，否则将字符串转换为输入类型
        /// </summary>
        /// <typeparam name="TTo">类型</typeparam>
        /// <param name="from">待转换字符串</param>
        /// <returns>返回值</returns>
        public static TTo To<TTo>(this string from)
        {
            return To(from, default(TTo));
        }

        /// <summary>
        /// 获取对象的属性值，并转换成目标类型，获取或者转换失败，返回默认值
        /// </summary>
        /// <typeparam name="TFrom">对象的类型</typeparam>
        /// <typeparam name="TProperty">对象属性类型</typeparam>
        /// <typeparam name="TTo">转换类型</typeparam>
        /// <param name="from">对象</param>
        /// <param name="getProperty">属性获取委托</param>
        /// <param name="defaultValue"></param>
        /// <returns>转换结果</returns>
        public static TTo To<TFrom, TProperty, TTo>(this TFrom from, Func<TFrom, TProperty> getProperty, TTo defaultValue)
        {
            if (from.Equals(null)) return defaultValue;
            TProperty v = getProperty(from);
            return To(v, defaultValue);
        }

        /// <summary>
        /// 获取对象的属性值，并转换成目标类型，获取或者转换失败，返回类型默认值
        /// </summary>
        /// <typeparam name="TFrom">对象的类型</typeparam>
        /// <typeparam name="TProperty">对象属性类型</typeparam>
        /// <typeparam name="TTo">转换类型</typeparam>
        /// <param name="from">对象</param>
        /// <param name="getProperty">属性获取委托</param>
        /// <returns>转换结果</returns>
        public static TTo To<TFrom, TProperty, TTo>(this TFrom from, Func<TFrom, TProperty> getProperty)
        {
            if (from.Equals(null)) return default(TTo);
            TProperty v = getProperty(from);
            return To(v, default(TTo));
        }

        /// <summary>
        /// 获取对象的属性值，并转换成目标类型，获取或者转换失败，返回类型默认值
        /// </summary>
        /// <typeparam name="TFrom">对象的类型</typeparam>
        /// <typeparam name="TTo">转换类型</typeparam>
        /// <param name="from">对象</param>
        /// <param name="getProperty">属性获取委托</param>
        /// <returns>转换结果</returns>
        public static TTo To<TFrom, TTo>(this TFrom from, Func<TFrom, object> getProperty)
        {
            return To<TFrom, object, TTo>(from, getProperty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Rookey.Frame.Common
{
    public static class IEnumerableExtension
    {

        /// <summary>
        /// 将可枚举对象值转换为以指定符号分隔字符串连接的字符串
        /// </summary>
        /// <param name="val">可枚举对象</param>
        /// <param name="separator">连接符号</param>
        /// <param name="format">转换格式</param>
        /// <returns>字符串</returns>
        public static string Splice<T>(this IEnumerable<T> val, string separator, Func<T, string> format)
        {
            StringBuilder s = new StringBuilder();
            if (val == null) return string.Empty;
            foreach (T v in val)
            {
                s.AppendFormat(format(v), v.ToString()).Append(separator);
            }
            if (s.Length > 0)
                s.Remove(s.Length - separator.Length, separator.Length);
            return s.ToString();
        }


        /// <summary>
        /// 将可枚举对象值转换为以指定符号分隔字符串连接的字符串
        /// </summary>
        /// <param name="val">可枚举对象</param>
        /// <param name="separator">连接符号</param>
        /// <param name="format">转换格式</param>
        /// <returns>字符串</returns>
        public static string Splice<T>(this IEnumerable<T> val, string separator, string format)
        {
            return Splice(val, separator, o => string.Format(format, o));
        }


        /// <summary>
        /// 将可枚举对象值转换为以指定符号分隔字符串连接的字符串
        /// </summary>
        /// <param name="val">可枚举对象</param>
        /// <param name="separator">连接符号</param>
        /// <returns>字符串</returns>
        public static string Splice<T>(this IEnumerable<T> val, string separator)
        {
            return Splice(val, separator, "{0}");
        }


        /// <summary>
        /// 将可枚举对象值转换为以逗号分隔字符串连接的字符串
        /// </summary>
        /// <param name="val">可枚举对象</param>
        /// <returns>字符串</returns>
        public static string Splice<T>(this IEnumerable<T> val)
        {
            return Splice(val, ",", "{0}");
        }

        /// <summary>
        /// 可枚举类型迭代函数
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="val">枚举</param>
        /// <param name="action">迭代函数主体，第一个参数为迭代项，第二个参数为索引号</param>
        public static void ForEach<T>(this IEnumerable<T> val, Action<T, int> action)
        {
            if (val == null) throw new ArgumentNullException();
            if (action == null) throw new ArgumentNullException();
            var index = 0;
            foreach (var o in val)
            {
                action(o, index++);
            }
        }

        /// <summary>
        /// 可枚举类型迭代函数
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="val">枚举</param>
        /// <param name="action">迭代函数主体,第一个参数为迭代项</param>
        public static void ForEach<T>(this IEnumerable<T> val, Action<T> action)
        {
            ForEach(val, (o, i) => action(o));
        }



    }
}

/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 参数检查类
    /// </summary>
    public static class ArgumentCheck
    {
        /// <summary>
        /// 参数不能为空
        /// </summary>
        /// <param name="argumentValue"></param>
        /// <param name="argumentName"></param>
        public static void NotNull<T>(T argumentValue, string parameterName) where T : class
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(string.Format("参数 {0} 为空引发异常", parameterName));
            }
        }

        /// <summary>
        /// 参数不能为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(string.Format("参数 {0} 为空引发异常", parameterName));
            }
        }

        /// <summary>
        /// 参数必须大于零
        /// </summary>
        /// <param name="argumentValue"></param>
        /// <param name="argumentName"></param>
        public static void MustMoreThanZero(int? argumentValue, string argumentName)
        {
            if (argumentValue <= 0)
            {
                throw new ArgumentException(string.Format("{0} 参数必须大于零", argumentName));
            }
        }

        /// <summary>
        /// 参数不能为null和empty
        /// </summary>
        /// <param name="argumentValue"></param>
        /// <param name="argumentName"></param>
        public static void NotEmpty(string argumentValue, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentException("ArgumentMustNotBeEmpty", argumentName);
            }
        }

        private static string GetTypeName(object assignmentInstance)
        {
            try
            {
                return assignmentInstance.GetType().FullName;
            }
            catch (Exception)
            {
                return "UnknownType";
            }
        }

        public static void InstanceIsAssignable(Type assignmentTargetType, object assignmentInstance, string argumentName)
        {
            if (assignmentTargetType == null)
            {
                throw new ArgumentNullException("assignmentTargetType", argumentName);
            }
            if (assignmentInstance == null)
            {
                throw new ArgumentNullException("assignmentInstance", argumentName);
            }
            if (!assignmentTargetType.IsInstanceOfType(assignmentInstance))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "TypesAreNotAssignable", new object[] { assignmentTargetType, GetTypeName(assignmentInstance) }), argumentName);
            }
        }

        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            if (assignmentTargetType == null)
            {
                throw new ArgumentNullException("assignmentTargetType");
            }
            if (assignmentValueType == null)
            {
                throw new ArgumentNullException("assignmentValueType");
            }
            if (!assignmentTargetType.IsAssignableFrom(assignmentValueType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "TypesAreNotAssignable", new object[] { assignmentTargetType, assignmentValueType }), argumentName);
            }
        }

        public static void AllNotNull(params object[] objs)
        {
            foreach (var o in objs)
            {
                NotNull(o, o.GetType().FullName);
            }
        }
    }
}

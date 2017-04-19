/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 非空检查类
    /// </summary>
    public static class NotNullCheck
    {
        /// <summary>
        /// 对象非空检查
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="objName">对象名称</param>
        public static void NotNull(object obj, string objName)
        {
            if (obj == null)
            {
                throw new NullReferenceException(string.Format("{0} 为空引发异常", objName));
            }
        }

        /// <summary>
        /// 非空和非空字符串检查
        /// </summary>
        /// <param name="obj">字符串对象</param>
        /// <param name="objName">对象名称</param>
        public static void NotEmpty(object obj, string objName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
            {
                throw new NullReferenceException(string.Format("{0} 为空或为空字符串引发异常", objName));
            }
        }
    }
}

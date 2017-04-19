//======================================================================
//        开源日期:           2013/09/01
//            作者:           何雨泉    
//            博客：          http://www.cnblogs.com/heyuquan/
//            版本:           0.0.0.1
//======================================================================

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace Rookey.Frame.Email
{
    /// <summary>
    /// 邮件验证帮助类
    /// </summary>
    static class MailValidatorHelper
    {
        /// <summary>
        /// 数据验证类使用的正则表述式选项
        /// </summary>
        private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
        /// <summary>
        /// 检测字符串是否为有效的邮件地址捕获正则
        /// </summary>
        private static readonly Regex EmailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", Options);

        /// <summary>
        /// 检测字符串是否为有效的邮件地址
        /// </summary>
        /// <param name="input">需要检查的字符串</param>
        /// <returns>如果字符串为有效的邮件地址，则为 true；否则为 false。</returns>
        public static bool IsEmail(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            else
            {
                return EmailRegex.IsMatch(input);
            }
        }

        public const string EMAIL_ADDRESS_RANGE_ERROR = "地址类型设置错误，只接受\"收件人、抄送人、密送人\"类型";
        public const string EMAIL_ADDRESS_LIST_ERROR = "Email地址列表子项不能为空";
        public const string EMAIL_ADDRESS_DIC_ERROR = "Email地址字典中地址子项不能为空";

        public const string EMAIL_PREPARESENDCOUNT_NOTSET_ERROR = "计划异步批量发送邮件的数量未设置异常";
        public const string EMAIL_ASYNC_CALL_ERROR = "此方法只能用于异步方式的邮件发送";
        public const string EMAIL_ASYNC_SEND_PREPARE_ERROR = "计划异步发送邮件数量不能小于已经异步发送完成的邮件数量";
        public const string EMAIL_SMTP_TYPE_ERROR = "邮件类型设置错误，不能设置为NONE";

        /// <summary>
        /// 验证对象是否为null，并抛出异常
        /// </summary>
        /// <param name="argValue">参数值</param>
        /// <param name="paramName">参数名</param>
        public static void ValideArgumentNull<T>(T argValue, string paramName) where T : class
        {
            if (argValue == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// 验证对象是否为null，并抛出异常
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="argValue">参数值</param>
        /// <param name="message">提示信息</param>
        public static void ValideArgumentNull<T>(T argValue, string paramName, string message) where T : class
        {
            if (argValue == null)
                throw new ArgumentNullException(paramName, message);
        }

        /// <summary>
        /// 验证对象是否为null，并抛出异常
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="argValue">参数值</param>
        /// <param name="message">提示信息</param>
        public static void ValideStrNullOrEmpty(string argValue, string paramName)
        {
            if (String.IsNullOrEmpty(argValue))
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// 验证对象是否为null，并抛出异常
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="argValue">参数值</param>
        /// <param name="message">提示信息</param>
        public static void ValideStrNullOrEmpty(string argValue, string paramName, string message)
        {
            if (String.IsNullOrEmpty(argValue))
                throw new ArgumentNullException(paramName, message);
        }

    }
}

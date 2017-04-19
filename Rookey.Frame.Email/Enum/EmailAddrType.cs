
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Email
{
    /// <summary>
    /// 接收邮件地址类型
    /// </summary>
    enum EmailAddrType
    {
        /// <summary>
        /// 发件人
        /// </summary>
        From = 0,
        /// <summary>
        /// 收件人
        /// </summary>
        To = 2,
        /// <summary>
        /// 抄送人
        /// </summary>
        CC = 4,
        /// <summary>
        /// 密送人
        /// </summary>
        Bcc = 8,
    }
}

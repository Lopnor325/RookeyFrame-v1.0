using System;
using System.Collections.Generic;
using System.Web.Helpers;

namespace Rookey.Frame.Email
{
    /// <summary>
    /// WebMail api
    /// </summary>
    public class WebEmailHelper
    {
        #region 邮件发送

        /// <summary>
        /// 发送邮件，WebMail方式
        /// </summary>
        /// <param name="smtpServer">smtp服务器</param>
        /// <param name="smtpPort">smtp端口号</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <param name="to">收件人</param>
        /// <param name="fileAttachList">附件</param>
        /// <param name="from">发送人描述</param>
        /// <param name="cc">抄送人</param>
        /// <param name="bcc">密送人</param>
        /// <returns>返回异常信息</returns>
        public string SendByWebMail(string smtpServer, int smtpPort, string username, string password,
            string subject, string body, string to, List<string> fileAttachList = null, string from = null, string cc = null, string bcc = null)
        {
            try
            {
                WebMail.SmtpServer = smtpServer;
                WebMail.SmtpPort = smtpPort;
                WebMail.EnableSsl = false;
                WebMail.UserName = username;
                WebMail.From = string.IsNullOrEmpty(from) ? username : from;
                WebMail.Password = password;
                // 发送邮件       
                WebMail.Send(to: to, cc: cc, bcc: bcc,
                subject: subject, body: body, headerEncoding: "utf-8",
                contentEncoding: "utf-8", filesToAttach: fileAttachList);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion
    }
}

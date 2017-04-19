using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;

namespace Rookey.Frame.Email
{
    /// <summary>
    /// 委托（用于异步发送邮件）
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns>是否执行成功</returns>
    public delegate void MailAsyncHandle(bool success, string message);

    /// <summary>
    /// 邮件发送类
    /// </summary>
    public class MailSend
    {
        private int _preSendCount = 1; //计划发送邮件数
        private MailHelper _mail = null; //邮件对象
        private MailSendParams _mailSendParams = null; //邮件发送参数
        private bool _autoReleaseSmtp = true; //是否自动释放SmtpClient
        private bool _isReuse = true; //是否重用SmtpClient
        private bool _isAsync = true; //是否异步发送邮件
        private bool _isSimple = true; //是否单条发送

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sendParams">发送参数对象</param>
        /// <param name="mailAsyncMethod">异步发送回调方法</param>
        /// <param name="isSimple">是否单条发送</param>
        /// <param name="isAsync">是否异常发送</param>
        public MailSend(MailSendParams sendParams, bool isSimple = true, bool isAsync = true)
        {
            if (sendParams == null) return;
            this._mailSendParams = sendParams;
            this._isAsync = isAsync;
            this._mail = new MailHelper(isAsync);
            if (!this._isReuse || !this._mail.ExistsSmtpClient())
            {
                SmtpClient client = new SmtpHelper(this._mailSendParams.EmailType, this._mailSendParams.EnabledSsl, this._mailSendParams.Username, this._mailSendParams.Password).SmtpClient;
                this._mail.SetSmtpClient(client, this._autoReleaseSmtp);
            }

            this._mail.From = this._mailSendParams.From;
            this._mail.FromDisplayName = this._mailSendParams.FromDisplay;

            if (this._mailSendParams.To != null && this._mailSendParams.To.Count > 0)
            {
                foreach (string to in this._mailSendParams.To.Keys)
                {
                    this._mail.AddReceive(EmailAddrType.To, to, this._mailSendParams.To[to]);
                }
            }
            else
            {
                return;
            }
            if (this._mailSendParams.Cc != null && this._mailSendParams.Cc.Count > 0)
            {
                foreach (string cc in this._mailSendParams.Cc.Keys)
                {
                    this._mail.AddReceive(EmailAddrType.CC, cc, this._mailSendParams.Cc[cc]);
                }
            }
            if (this._mailSendParams.Bcc != null && this._mailSendParams.Bcc.Count > 0)
            {
                foreach (string bcc in this._mailSendParams.Bcc.Keys)
                {
                    this._mail.AddReceive(EmailAddrType.Bcc, bcc, this._mailSendParams.Bcc[bcc]);
                }
            }

            this._mail.Subject = this._mailSendParams.Subject;
            // Guid.NewGuid() 防止重复内容，被SMTP服务器拒绝接收邮件
            this._mail.IsBodyHtml = true;
            this._mail.Body = this._mailSendParams.Body;// +"   ---Email flage:" + Guid.NewGuid().ToString();//"<span style='display:none'>" + Guid.NewGuid().ToString() + "</span>";

            if (this._mailSendParams.Attachments != null && this._mailSendParams.Attachments.Count > 0)
            {
                foreach (string filePath in this._mailSendParams.Attachments)
                {
                    _mail.AddAttachment(filePath);
                }
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="mailAsyncMethod"></param>
        /// <returns></returns>
        public bool Send(out string errMsg, MailAsyncHandle mailAsyncMethod = null)
        {
            errMsg = string.Empty;
            bool rs = false;
            if (this._isAsync)
            {
                if (mailAsyncMethod != null)
                {
                    this._mail.AsycUserState = string.Empty;
                    this._mail.Client.SendCompleted += (send, args) =>
                    {
                        AsyncCompletedEventArgs arg = args;
                        if (arg.Error == null)
                        {
                            // 需要注意的事使用 MailHelper 发送异步邮件，其UserState是 MailUserState 类型
                            if (mailAsyncMethod != null)
                            {
                                mailAsyncMethod.Invoke(true, ((MailUserState)args.UserState).UserState.ToString() + "发送成功");
                            }
                        }
                        else
                        {
                            if (mailAsyncMethod != null)
                            {
                                string err = String.Format("{0} 异常：{1}", ((MailUserState)args.UserState).UserState.ToString() + "发送失败."
                                           , (arg.Error.InnerException == null ? arg.Error.Message : arg.Error.Message + arg.Error.InnerException.Message));
                                mailAsyncMethod.Invoke(true, err);
                            }
                            // 标识异常已处理，否则若有异常，会抛出异常
                            ((MailUserState)args.UserState).IsErrorHandle = true;
                        }
                    };
                }

            }
            if (this._isSimple)
            {
                rs = SendMessage(out errMsg);
            }
            else
            {
                for (long i = 1; i <= this._preSendCount; i++)
                {
                    rs = SendMessage(out errMsg);
                }
                this._mail.SetBatchMailCount(this._preSendCount);
            }
            return rs;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="errMsg">反馈信息</param>
        private bool SendMessage(out string errMsg)
        {
            errMsg = string.Empty;
            if (this._mail == null)
            {
                errMsg = "MailHelper对象为空";
                return false;
            }
            Dictionary<MailInfoType, string> dic = this._mail.CheckSendMail();
            if (dic.Count > 0 && MailInfoHelper.ExistsError(dic))
            {
                // 反馈“错误+提示”信息
                errMsg = MailInfoHelper.GetMailInfoStr(dic);
                return false;
            }
            try
            {
                if (this._isSimple)
                {
                    this._mail.SendOneMail();
                }
                else
                {
                    // 发送
                    this._mail.SendBatchMail();
                }
                return true;
            }
            catch (Exception ex)
            {
                // 反馈异常信息
                errMsg = ex.InnerException == null ? ex.Message : ex.Message + ex.InnerException.Message;
            }
            finally
            {
                _mail.Reset();
            }
            return false;
        }
    }
}

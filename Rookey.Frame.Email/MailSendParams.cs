using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rookey.Frame.Email
{
    /// <summary>
    /// 邮件发送参数类
    /// </summary>
    public class MailSendParams
    {
        private EmailType _emailType = EmailType.Customer;

        public EmailType EmailType
        {
            get { return _emailType; }
        }
        private bool _enabledSsl = false;

        public bool EnabledSsl
        {
            get { return _enabledSsl; }
        }
        private string _from;

        public string From
        {
            get { return _from; }
            set { _from = value; }
        }
        private string _fromDisplay;

        public string FromDisplay
        {
            get { return _fromDisplay; }
        }
        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        private string _password;

        public string Password
        {
            get { return _password; }
        }
        private Dictionary<string, string> _to = null;

        public Dictionary<string, string> To
        {
            get { return _to; }
        }
        private Dictionary<string, string> _cc = null;

        public Dictionary<string, string> Cc
        {
            get { return _cc; }
        }
        private Dictionary<string, string> _bcc = null;

        public Dictionary<string, string> Bcc
        {
            get { return _bcc; }
        }
        private string _subject;

        public string Subject
        {
            get { return _subject; }
        }
        private string _body;

        public string Body
        {
            get { return _body; }
        }
        private List<string> _attachments = null;

        public List<string> Attachments
        {
            get { return _attachments; }
        }

        public MailSendParams(Dictionary<string, string> to, string from, string username, string password, string subject, string body = "", List<string> attachments = null, string fromDisplay = "", Dictionary<string, string> cc = null, Dictionary<string, string> bcc = null, bool enableSSL = false)
        {
            this._from = from;
            this._fromDisplay = fromDisplay;
            this._username = username;
            this._password = password;
            this._to = to;
            this._cc = cc;
            this._bcc = cc;
            this._subject = subject;
            this._body = body;
            this._attachments = attachments;
            this._enabledSsl = enableSSL;
        }
    }
}

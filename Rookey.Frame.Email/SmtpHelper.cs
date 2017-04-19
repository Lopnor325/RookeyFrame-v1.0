
using System;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;


namespace Rookey.Frame.Email
{
    // 1、一个SmtpClient一次只能发送一个MailMessage，不管是同步还是异步发送，所以批量发送也会因为这个条件而被阻塞。
    // 2、若要异步发送大批量邮件，方案：应当多个线程、每个线程去使用一个单独的SmtpClient去发送。（但要注意不合理分配资源会更加降低性能）
    // 3、何时使用 SmtpClient.SendAsync() 异步发送呢？是在发件内容、附件、加密等因素造成一条短信发送比较耗时的情况下使用。

    /// <summary>
    /// SmtpClient构造器
    /// 使用注意事项：
    /// 1、非线程安全类
    /// 2、构造的SmtpClient 实例由外部进行Dispose()。SmtpHelper类只简单提供构造，不做释放操作。
    /// 3、SmtpClient 没有提供 Finalize() 终结器，所以GC不会进行回收，只能由外部使用完后进行显示释放，否则会发生内存泄露问题
    /// </summary>
    class SmtpHelper
    {
        /// <summary>
        /// 返回内部构造的SmtpClient实例
        /// </summary>
        public SmtpClient SmtpClient { get; private set; }

        #region  SmtpHelper 构造函数

        #region SMTP服务器 需要身份验证凭据

        /// <summary>
        /// 创建 SmtpHelper 实例
        /// </summary>
        /// <param name="host">设置 SMTP 主服务器</param>
        /// <param name="port">端口号</param>
        /// <param name="enableSsl">指定 SmtpClient 是否使用安全套接字层 (SSL) 加密连接。</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public SmtpHelper(string host, int port, bool enableSsl, string userName, string password)
        {
            MailValidatorHelper.ValideStrNullOrEmpty(host, "host");
            MailValidatorHelper.ValideStrNullOrEmpty(userName, "userName");
            MailValidatorHelper.ValideStrNullOrEmpty(password, "password");

            SmtpClient = new SmtpClient(host, port);
            SmtpClient.EnableSsl = enableSsl;
            SmtpClient.UseDefaultCredentials = false;
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpClient.Credentials = new NetworkCredential(userName, password);
            SmtpClient.Timeout = 100000;
        }

        /// <summary>
        /// 创建 SmtpHelper 实例
        /// </summary>
        /// <param name="host">设置 SMTP 主服务器</param>
        /// <param name="port">端口号</param>
        /// <param name="enableSsl">指定 SmtpClient 是否使用安全套接字层 (SSL) 加密连接。</param>
        /// <param name="credential">设置用于验证发件人身份的凭据。</param>
        public SmtpHelper(string host, int port, bool enableSsl, NetworkCredential credential)
        {
            MailValidatorHelper.ValideStrNullOrEmpty(host, "host");
            MailValidatorHelper.ValideArgumentNull<NetworkCredential>(credential, "credential");

            SmtpClient = new SmtpClient(host, port);
            SmtpClient.EnableSsl = enableSsl;
            SmtpClient.UseDefaultCredentials = false;
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpClient.Credentials = credential;
            SmtpClient.Timeout = 100000;
        }

        /// <summary>
        /// 创建 SmtpHelper 实例
        /// </summary>
        /// <param name="type">Email类型</param>
        /// <param name="enableSsl">指定 SmtpClient 是否使用安全套接字层 (SSL) 加密连接。</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public SmtpHelper(EmailType type, bool enableSsl, string userName, string password)
        {
            MailValidatorHelper.ValideStrNullOrEmpty(userName, "userName");
            MailValidatorHelper.ValideStrNullOrEmpty(password, "password");

            this.EmailTypeConfig(type, enableSsl);
            SmtpClient.UseDefaultCredentials = false;
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpClient.Credentials = new NetworkCredential(userName, password);
            SmtpClient.Timeout = 100000;
        }

        /// <summary>
        /// 创建 SmtpHelper 实例
        /// </summary>
        /// <param name="type">Email类型</param>
        /// <param name="enableSsl">指定 SmtpClient 是否使用安全套接字层 (SSL) 加密连接。</param>
        /// <param name="credential">设置用于验证发件人身份的凭据。</param>
        public SmtpHelper(EmailType type, bool enableSsl, NetworkCredential credential)
        {
            MailValidatorHelper.ValideArgumentNull<NetworkCredential>(credential, "credential");

            this.EmailTypeConfig(type, enableSsl);
            SmtpClient.UseDefaultCredentials = false;
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpClient.Credentials = credential;
            SmtpClient.Timeout = 100000;
        }

        #endregion

        #region SMTP服务器 根据 useDefaultCredentials 参数决定，SMTP服务器是否传输系统默认凭证
        // useDefaultCredentials
        // false：则连接到服务器时会将 Credentials 属性中设置的值用作凭据。
        //        如果UseDefaultCredentials属性设置为 false 并且尚未设置 Credentials 属性，则将邮件以匿名方式发送到服务器。
        //        若SMTP 服务器要求在验证客户端的身份则会抛出异常。。
        // true：System.Net.CredentialCache.DefaultCredentials （应用程序系统凭证）会随请求一起发送。

        /// <summary>
        /// 创建 SmtpHelper 实例
        /// </summary>
        /// <param name="host">设置 SMTP 主服务器</param>
        /// <param name="port">端口号</param>
        /// <param name="enableSsl">指定 SmtpClient 是否使用安全套接字层 (SSL) 加密连接。</param>
        /// <param name="useDefaultCredentials">SMTP服务器是否传输系统默认凭证。</param>
        public SmtpHelper(string host, int port, bool enableSsl, bool useDefaultCredentials)
        {
            MailValidatorHelper.ValideStrNullOrEmpty(host, "host");

            SmtpClient.Host = host;
            SmtpClient.Port = port;
            SmtpClient.EnableSsl = enableSsl;
            SmtpClient.UseDefaultCredentials = useDefaultCredentials;
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpClient.Timeout = 100000;
        }

        /// <summary>
        /// 创建 SmtpHelper 实例
        /// </summary>
        /// <param name="type">Email类型</param>
        /// <param name="enableSsl">指定 SmtpClient 是否使用安全套接字层 (SSL) 加密连接。</param>
        /// <param name="useDefaultCredentials">SMTP服务器是否传输系统默认凭证。</param>
        public SmtpHelper(EmailType type, bool enableSsl, bool useDefaultCredentials)
        {
            this.EmailTypeConfig(type, enableSsl);
            SmtpClient.UseDefaultCredentials = useDefaultCredentials;
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpClient.Timeout = 100000;
        }

        #endregion

        /// <summary>
        /// 根据Email类型创建SmtpClient
        /// </summary>
        /// <param name="type">Email类型</param>
        /// <param name="enableSsl">端口号会根据是否支持ssl而不同</param>
        private void EmailTypeConfig(EmailType type, bool enableSsl)
        {
            switch (type)
            {
                case EmailType.Customer:
                    {
                        // 自定义邮箱
                        string host = string.Empty;
                        int port = 25;
                        try
                        {
                            string emailConfig = ConfigurationManager.AppSettings["EmailServer"].ToString();
                            if (!string.IsNullOrEmpty(emailConfig))
                            {
                                string[] token = emailConfig.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                if (token.Length > 0)
                                {
                                    host = token[0];
                                    if (token.Length > 1)
                                    {
                                        int.TryParse(token[1], out port);
                                    }
                                }
                            }
                        }
                        catch { }
                        if (host == string.Empty)
                            throw new Exception(MailValidatorHelper.EMAIL_SMTP_TYPE_ERROR);
                        SmtpClient = new SmtpClient(host, port);
                        SmtpClient.EnableSsl = enableSsl;
                    }
                    break;
                case EmailType.Gmail:
                    {
                        // Gmail的SMTP要求使用安全连接（SSL）
                        SmtpClient = new SmtpClient("smtp.gmail.com", 587);
                        SmtpClient.EnableSsl = true;
                    }
                    break;
                case EmailType.HotMail:
                    {
                        // HotMail的SMTP要求使用安全连接（SSL）
                        SmtpClient = new SmtpClient("smtp.live.com", 25);
                        SmtpClient.EnableSsl = true;
                    }
                    break;
                case EmailType.QQ_FoxMail:
                    {
                        SmtpClient = new SmtpClient("smtp.qq.com", 25);
                        SmtpClient.EnableSsl = false;
                    }
                    break;
                case EmailType.Mail_126:
                    {
                        SmtpClient = new SmtpClient("smtp.126.com", 25);
                        SmtpClient.EnableSsl = false;
                    }
                    break;
                case EmailType.Mail_163:
                    {
                        SmtpClient = new SmtpClient("smtp.163.com", enableSsl ? 994 : 25);
                        SmtpClient.EnableSsl = enableSsl;
                    }
                    break;
                case EmailType.Sina:
                    {
                        SmtpClient = new SmtpClient("smtp.sina.com", 25);
                        SmtpClient.EnableSsl = false;
                    }
                    break;
                case EmailType.Tom:
                    {
                        SmtpClient = new SmtpClient("smtp.tom.com", 25);
                        SmtpClient.EnableSsl = false;
                    }
                    break;
                case EmailType.SoHu:
                    {
                        SmtpClient = new SmtpClient("smtp.sohu.com", 25);
                        SmtpClient.EnableSsl = false;
                    }
                    break;
                case EmailType.Yahoo:
                    {
                        SmtpClient = new SmtpClient("smtp.mail.yahoo.com", 25);
                        SmtpClient.EnableSsl = false;
                    }
                    break;
                case EmailType.None:
                default:
                    {
                        throw new Exception(MailValidatorHelper.EMAIL_SMTP_TYPE_ERROR);
                    }
            }
        }

        #endregion

        /// <summary>
        /// 设置SmtpClient.Send() 调用的超时时间。
        /// SmtpClient默认 Timeout =  （100秒=100*1000毫秒）。
        /// 应当根据“邮件大小、附件大小、加密耗时”等因素进行调整
        /// </summary>
        public SmtpHelper SetTimeout(int timeout)
        {
            if (timeout > 0)
            {
                SmtpClient.Timeout = timeout;
            }
            return this;
        }

        /// <summary>
        /// 设置 SmtpClient 如何处理待发的电子邮件。
        /// </summary>
        /// <param name="deliveryMethod">
        /// 0、Network（默认）：电子邮件通过网络发送到 SMTP 服务器。
        /// 1、SpecifiedPickupDirectory：将电子邮件复制到 SmtpClient.PickupDirectoryLocation 属性指定的目录，然后由外部应用程序传送。
        /// 2、PickupDirectoryFromIis：将电子邮件复制到拾取目录，然后通过本地 Internet 信息服务 (IIS) 传送。
        /// </param>
        public SmtpHelper SetDeliveryMethod(int deliveryMethod)
        {
            if (deliveryMethod < 0 || deliveryMethod > 2)
                deliveryMethod = 0;     //  Network（默认）

            SmtpClient.DeliveryMethod = (SmtpDeliveryMethod)deliveryMethod;

            return this;
        }

        /// <summary>
        /// 添加建立安全套接字层 (SSL) 连接的证书
        /// </summary>
        public SmtpHelper AddClientCertificate(X509Certificate certificate)
        {
            MailValidatorHelper.ValideArgumentNull<X509Certificate>(certificate, "certificate");

            SmtpClient.EnableSsl = true;
            SmtpClient.ClientCertificates.Add(certificate);

            return this;
        }

    }
}

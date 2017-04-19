//======================================================================
//        开源日期:           2013/09/01
//            作者:           何雨泉    
//            博客：          http://www.cnblogs.com/heyuquan/
//            版本:           0.0.0.1
//======================================================================

#define debug

using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;


namespace Rookey.Frame.Email
{
    /// <summary>
    /// 非线程安全类
    /// 使用注意事项：
    /// 1、该类无需（也不能）在外部包裹多线程，因为内部有提供“异步发送”方法，内、外都使用多线程会导致线程池对可用资源的误判，从而创建过多阻塞线程。
    /// 2、MailHelper类的 m_autoDisposeSmtp 属性的使用，具体见此字段注释。
    /// 3、启用 UTF-8 字符编码
    /// </summary>
    class MailHelper
    {

        #region 构造函数

        /// <summary>
        /// 构建 MailHelper 实例
        /// </summary>
        /// <param name="isAsync">是否启用异步邮件发送，默认为同步发送</param>
        public MailHelper(bool isAsync = false)
        {
            m_IsAsync = isAsync;
        }

        /// <summary>
        /// 构建 MailHelper 实例
        /// </summary>
        /// <param name="mSmtpClient">SmtpClient实例</param>
        /// <param name="autoReleaseSmtp">是否自动释放SmtpClient实例</param>
        /// <param name="isAsync">是否启用异步邮件发送</param>
        public MailHelper(SmtpClient mSmtpClient, bool autoReleaseSmtp, bool isAsync = false)
        {
            this.SetSmtpClient(mSmtpClient, autoReleaseSmtp);
            m_IsAsync = isAsync;
        }

        #endregion

        #region  计划邮件数量 和 已执行完成邮件数量

        // 记录和获取在大批量执行异步短信发送时已经处理了多少条记录
        // 1、根据此值手动或自动释放 SmtpClient .实际上没有需要根据此值进行手动释放，因为完全可以用自动释放替换此逻辑
        // 2、根据此值可以自己设置进度
        private long m_CompletedSendCount = 0;
        public long CompletedSendCount
        {
            get { return Interlocked.Read(ref m_CompletedSendCount); }
            private set { Interlocked.Exchange(ref m_CompletedSendCount, value); }
        }

        // 计划邮件数量
        private long m_PrepareSendCount = 0;
        public long PrepareSendCount
        {
            get { return Interlocked.Read(ref m_PrepareSendCount); }
            private set { Interlocked.Exchange(ref m_PrepareSendCount, value); }
        }

        #endregion

        #region 异步 发送邮件相关参数

        // 是否启用异步发送邮件
        private bool m_IsAsync = false;

        // 案例：因为异步发送邮件在SmtpClient处必须加锁保证一封一封的发送。
        // 这样阻塞了主线程。所以换用队列的方式以无阻塞的方式进行异步发送大批量邮件

        // 发送任务可能很长，所以使用 Thread 而不是用ThreadPool。（避免长时间暂居线程池线程），并且SmtpClient只支持一次一封邮件发送
        private Thread m_SendMailThread = null;

        private AutoResetEvent m_AutoResetEvent = null;
        private AutoResetEvent AutoResetEvent
        {
            get
            {
                if (m_AutoResetEvent == null)
                    m_AutoResetEvent = new AutoResetEvent(true);
                return m_AutoResetEvent;
            }
        }

        // 待发送队列缓存数量。单独开个计数是为了提高获取此计数的效率
        private int m_messageQueueCount = 0;
        // 因为 MessageQueue 可能在 m_SendMailThread 线程中进行出队操作,所以使用并发队列ConcurrentQueue.
        // 队列中的数据只能通过取消异步发送进行清空，或则就会每一元素都执行发送邮件
        private ConcurrentQueue<MailUserState> m_MessageQueue = null;
        private ConcurrentQueue<MailUserState> MessageQueue
        {
            get
            {
                if (m_MessageQueue == null)
                    m_MessageQueue = new ConcurrentQueue<MailUserState>();
                return m_MessageQueue;
            }
        }

        /// <summary>
        /// 在执行异步发送时传递的对象，用于传递给异步发生完成时调用的方法 OnSendCompleted 。
        /// </summary>
        public object AsycUserState { get; set; }

        #endregion

        #region 内部字段、属性

        private SmtpClient m_SmtpClient = null;

        /// <summary>
        /// 默认为false。设置在 MailHelper 类内部，发送完邮件后是否自动释放 SmtpClient 实例
        /// Smtp不管是在 MailHelper 内部还是在外部都必须进行主动释放，
        /// 因为：SmtpClient 没有提供 Finalize() 终结器，所以GC不会进行回收，只能使用完后主动进行释放，否则会发生内存泄露问题。
        /// 
        /// 何时将 autoReleaseSmtp 设置为false，就是SmtpClient需要重复使用的情况，即需要使用“相同MailHelper”向“相同Smtp服务器”发送大批量的邮件时。
        /// </summary>
        private bool m_autoDisposeSmtp = false;

        /// <summary>
        /// 设置此电子邮件的收件人的地址集合。
        /// </summary>
        Dictionary<string, string> m_DicTo = null;
        Dictionary<string, string> DicTo
        {
            get
            {
                if (m_DicTo == null)
                    m_DicTo = new Dictionary<string, string>();
                return m_DicTo;
            }
        }
        /// <summary>
        /// 设置此电子邮件的抄送 (CC) 收件人的地址集合。
        /// </summary>
        Dictionary<string, string> m_DicCC = null;
        Dictionary<string, string> DicCC
        {
            get
            {
                if (m_DicCC == null)
                    m_DicCC = new Dictionary<string, string>();
                return m_DicCC;
            }
        }
        /// <summary>
        /// 设置此电子邮件的密件抄送 (BCC) 收件人的地址集合。
        /// </summary>
        Dictionary<string, string> m_DicBcc = null;
        Dictionary<string, string> DicBcc
        {
            get
            {
                if (m_DicBcc == null)
                    m_DicBcc = new Dictionary<string, string>();
                return m_DicBcc;
            }
        }
        // 附件集合
        Collection<Attachment> m_Attachments;
        Collection<Attachment> Attachments
        {
            get
            {
                if (m_Attachments == null)
                    m_Attachments = new Collection<Attachment>();
                return m_Attachments;
            }
        }
        // 指定一个电子邮件不同格式显示的副本。
        Collection<AlternateView> m_AlternateViews;
        Collection<AlternateView> AlternateViews
        {
            get
            {
                if (m_AlternateViews == null)
                    m_AlternateViews = new Collection<AlternateView>();
                return m_AlternateViews;
            }
        }

        #endregion

        #region 公开属性

        /// <summary>
        /// SmtpClient对象
        /// </summary>
        public SmtpClient Client
        {
            get { return this.m_SmtpClient; }
        }

        /// <summary>
        /// 设置此电子邮件的发信人地址。
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 设置此电子邮件的发信人地址。
        /// </summary>
        public string FromDisplayName { get; set; }

        /// <summary>
        /// 设置此电子邮件的主题。
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 设置邮件正文。
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 设置邮件正文是否为 Html 格式的值。
        /// </summary>
        public bool IsBodyHtml { get; set; }

        private int priority = 0;
        /// <summary>
        /// 设置此电子邮件的优先级  0-Normal   1-Low   2-High
        /// 默认Normal。
        /// </summary>
        public int Priority
        {
            get { return this.priority; }
            set
            {
                if (value < 0 || value > 2)
                    priority = 0;
                else
                    priority = value;
            }
        }

        #endregion

        /// <summary>
        /// 重置 MailHelper 实例信息 
        /// 不释放 SmtpClient 实例和相关的AutoReleaseSimple字段，因为存在异步发送。。这两个字段由SetSmtpClient方法设置
        /// </summary>
        public void Reset()
        {
            From = String.Empty;
            FromDisplayName = String.Empty;
            if (m_DicTo != null)
                m_DicTo.Clear();
            if (m_DicCC != null)
                m_DicCC.Clear();
            if (m_DicBcc != null)
                m_DicBcc.Clear();
            if (m_Attachments != null)
                m_Attachments.Clear();
            if (m_AlternateViews != null)
                m_AlternateViews.Clear();

            Subject = String.Empty;
            Body = String.Empty;
            IsBodyHtml = false;
            priority = 0;

            AsycUserState = null;

            // 1、不重置SmtpClient。根据 m_autoDisposeSmtp 参数自动释放或由外部主动释放
            // 2、不重置：异步待发送队列及队列计数，AutoResetEvent实例，执行异步发送线程，是否启用异步发送标识
        }

        #region SmtpClient 相关方法

        /// <summary>
        /// 检查此 MailHelper 实例是否已经设置了 SmtpClient
        /// </summary>
        /// <returns>true代表已设置</returns>
        public bool ExistsSmtpClient()
        {
            return m_SmtpClient != null ? true : false;
        }

        /// <summary>
        /// 设置 SmtpClient 实例 和是否自动释放Smtp的唯一入口
        /// 1、将内部 计划数量 和 已完成数量 清零，重新统计以便自动释放SmtpClient
        /// 2、若要对SmtpClent设置SendCompleted事件，请在调用此方法前进行设置
        /// </summary>
        /// <param name="mSmtpClient"> SmtpClient 实例</param>
        /// <param name="autoReleaseSmtp">设置在 MailHelper 类内部，发送完邮件后是否自动释放 SmtpClient 实例</param>
        public void SetSmtpClient(SmtpClient mSmtpClient, bool autoReleaseSmtp)
        {
#if DEBUG
            Debug.WriteLine("设置SmtpClient,自动释放为" + (autoReleaseSmtp ? "TRUE" : "FALSE"));
#endif
            m_SmtpClient = mSmtpClient;
            m_autoDisposeSmtp = autoReleaseSmtp;

            // 将内部 计划数量 和 已完成数量 清零，重新统计以便自动释放SmtpClient  (MailHelper实例唯一的清零地方)
            m_PrepareSendCount = 0;
            m_CompletedSendCount = 0;

            if (m_IsAsync && autoReleaseSmtp)
            {
                // 注册内部释放回调事件.释放对象---该事件不进行取消注册，只在释放SmtpClient时，一起释放   （所以SmtpClient与MailHelper绑定后，就不要再单独使用了）
                m_SmtpClient.SendCompleted += new SendCompletedEventHandler(SendCompleted4Dispose);
            }
        }

        /// <summary>
        /// 释放 SmtpClient
        /// </summary>
        public void ManualDisposeSmtp()
        {
            this.InnerDisposeSmtp();
        }

        /// <summary>
        /// 释放SmtpClient
        /// </summary>
        private void AutoDisposeSmtp()
        {
            if (m_autoDisposeSmtp && m_SmtpClient != null)
            {
                if (PrepareSendCount == 0)
                {
                    // PrepareSendCount=0 说明还未设置计划批量邮件数，所以不自动释放SmtpClient。
                    // 不能因为小于CompletedSendCount就报错，因为可能是先发送再设置计划邮件数量
                }
                else if (PrepareSendCount < CompletedSendCount)
                {
                    throw new Exception(MailValidatorHelper.EMAIL_ADDRESS_RANGE_ERROR);
                }
                else if (PrepareSendCount == CompletedSendCount)
                {
                    this.InnerDisposeSmtp();
                }
            }
            else
            {
                // 不清空和Dispose()内部的SmtpClient字段，即用在需要重复使用时不需要再调用 SetSmtpClient() 进行设置。
            }
        }

        /// <summary>
        /// 释放SmtpClient
        /// </summary>
        private void InnerDisposeSmtp()
        {
            if (m_SmtpClient != null)
            {
#if DEBUG
                Debug.WriteLine("释放SMtpClient");
#endif
                m_SmtpClient.Dispose();
                m_SmtpClient = null;

                // 在设置 SmtpClient 入口处重新进行设置
                m_autoDisposeSmtp = false;

                PrepareSendCount = 0;
                CompletedSendCount = 0;
            }
        }

        #endregion

        #region MessageAddress、Attachment、AlternateView 相关方法

        #region 添加收件人、抄送人、密送人（每个类型中，若地址有重复，只保留第一个地址）

        /// <summary>
        /// 添加收件人、抄送人、密送人（每个类型中，若地址有重复，只保留第一个地址）
        /// </summary>
        /// <param name="type">类型：收件人、抄送人、密送人</param>
        /// <param name="addressList">Email地址列表</param>
        public void AddReceive(EmailAddrType type, IEnumerable<string> addressList)
        {
            MailValidatorHelper.ValideArgumentNull<IEnumerable<string>>(addressList, "addressList");
            if (addressList.Count() > 0)
            {
                Dictionary<string, string> dic = null;
                switch (type)
                {
                    case EmailAddrType.To:
                        dic = DicTo;
                        break;
                    case EmailAddrType.CC:
                        dic = DicCC;
                        break;
                    case EmailAddrType.Bcc:
                        dic = DicBcc;
                        break;
                    case EmailAddrType.From:
                        throw new Exception(MailValidatorHelper.EMAIL_ADDRESS_RANGE_ERROR);
                }

                foreach (string address in addressList)
                {
                    MailValidatorHelper.ValideStrNullOrEmpty(address, "addressList", MailValidatorHelper.EMAIL_ADDRESS_LIST_ERROR);
                    if (dic.Count > 0 && !dic.ContainsKey(address))
                        dic.Add(address, String.Empty);
                }
            }
        }

        /// <summary>
        /// 添加收件人、抄送人、密送人（每个类型中，若地址有重复，只保留第一个地址）
        /// </summary>
        /// <param name="type">类型：收件人、抄送人、密送人</param>
        /// <param name="address">Email地址</param>
        /// <param name="displayName">显示名称</param>
        public void AddReceive(EmailAddrType type, string address, string displayName)
        {
            MailValidatorHelper.ValideStrNullOrEmpty(address, "address");

            Dictionary<string, string> dic = null;
            switch (type)
            {
                case EmailAddrType.To:
                    dic = DicTo;
                    break;
                case EmailAddrType.CC:
                    dic = DicCC;
                    break;
                case EmailAddrType.Bcc:
                    dic = DicBcc;
                    break;
                case EmailAddrType.From:
                    throw new Exception(MailValidatorHelper.EMAIL_ADDRESS_RANGE_ERROR);
            }

            if (dic.Count == 0 || !dic.ContainsKey(address))
                dic.Add(address, displayName);

        }

        /// <summary>
        /// 添加收件人、抄送人、密送人（每个类型中，若地址有重复，只保留第一个地址）
        /// </summary>
        /// <param name="type">类型：收件人、抄送人、密送人</param>
        /// <param name="dicAddress">Email地址，显示名称</param>
        public void AddReceive(EmailAddrType type, Dictionary<string, string> dicAddress)
        {
            MailValidatorHelper.ValideArgumentNull<Dictionary<string, string>>(dicAddress, "dicAddress");
            if (dicAddress.Count > 0)
            {
                Dictionary<string, string> dic = null;
                switch (type)
                {
                    case EmailAddrType.To:
                        dic = DicTo;
                        break;
                    case EmailAddrType.CC:
                        dic = DicCC;
                        break;
                    case EmailAddrType.Bcc:
                        dic = DicBcc;
                        break;
                    case EmailAddrType.From:
                        throw new Exception(MailValidatorHelper.EMAIL_ADDRESS_RANGE_ERROR);
                }

                foreach (KeyValuePair<string, string> keyValue in dicAddress)
                {
                    MailValidatorHelper.ValideStrNullOrEmpty(keyValue.Key, "dicAddress", MailValidatorHelper.EMAIL_ADDRESS_DIC_ERROR);
                    if (dic.Count > 0 && !dic.ContainsKey(keyValue.Key))
                        dic.Add(keyValue.Key, keyValue.Value);
                }
            }
        }

        #endregion

        #region 添加附件

        /// <summary>
        /// 添加单个附件
        /// </summary>
        /// <param name="attachment">Attachment附件实例</param>
        public void AddAttachment(Attachment attachment)
        {
            MailValidatorHelper.ValideArgumentNull<Attachment>(attachment, "attachment");
            Attachments.Add(attachment);
        }

        /// <summary>
        /// 添加单个附件
        /// </summary>
        /// <param name="fieldPath">待上传文件路径</param>
        /// <param name="fileName">文件显示名称（不带后缀）</param>
        public void AddAttachment(string fieldPath, string fileName = "")
        {
            MailValidatorHelper.ValideStrNullOrEmpty(fieldPath, "fieldPath");

            this.InnerAddAttachment(fieldPath, fileName, false, String.Empty);
        }

        /// <summary>
        /// 添加内嵌资源（eg：图片，mp3等等）
        /// </summary>
        /// <param name="fieldPath">内嵌资源的文件路径</param>
        /// <param name="cidName">设置此附件的 MIME 内容 ID</param>
        public void AddInlineAttachment(string fieldPath, string cidName)
        {
            MailValidatorHelper.ValideStrNullOrEmpty(fieldPath, "fieldPath");
            MailValidatorHelper.ValideStrNullOrEmpty(cidName, "cidName");

            this.InnerAddAttachment(fieldPath, String.Empty, true, cidName);
        }

        private void InnerAddAttachment(string fieldPath, string fileName, bool isInline, string cidName)
        {
            // 因为Attachment中存储的时FilePath对应文件的Stream，所以这边在获取FileInfo信息的时候，同时转化为Stream传递给Attachment实例，
            // 避免再次根据FilePath获取文件内容

            FileInfo file = new FileInfo(fieldPath);

            Stream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            Attachment data = new Attachment(stream, String.Empty);
            data.NameEncoding = Encoding.UTF8;
            //实例邮件内容
            ContentDisposition disposition = data.ContentDisposition;
            if (isInline)
            {
                disposition.Inline = true;
                // 设置此附件的 MIME 内容 ID。
                data.ContentId = cidName;
            }

            // 设置文件附件的创建日期。
            disposition.CreationDate = file.CreationTime;
            // 设置文件附件的修改日期。
            disposition.ModificationDate = file.LastWriteTime;
            // 设置文件附件的读取日期。
            disposition.ReadDate = file.LastAccessTime;
            // 设定文件名称 (内嵌资源设置文件名后下载下来才有默认后缀)
            if (String.IsNullOrEmpty(fileName))
                disposition.FileName = file.Name.ToString();
            else
            {

                disposition.FileName = fileName + Path.GetExtension(fieldPath);
            }

            Attachments.Add(data);
        }

        #endregion

        #region 添加AlternateView
        // 指定一个电子邮件不同格式的副本。
        //（eg：发送HTML格式的邮件，可能希望同时提供邮件的纯文本格式，以防止一些收件人使用的电子邮件阅读程序无法显示html内容）

        /// <summary>
        /// 添加一个电子邮件不同格式的副本。
        /// </summary>
        /// <param name="filePath">包含电子邮件内容的文件路径</param>
        public void AddAlterViewPath(string filePath)
        {
            MailValidatorHelper.ValideStrNullOrEmpty(filePath, "filePath");
            AlternateViews.Add(new AlternateView(filePath));
        }

        /// <summary>
        /// 添加一个电子邮件不同格式的副本。
        /// </summary>
        /// <param name="mailContent">电子邮件内容</param>
        public void AddAlterViewContent(string mailContent)
        {
            MailValidatorHelper.ValideStrNullOrEmpty(mailContent, "mailContent");
            AlternateViews.Add(AlternateView.CreateAlternateViewFromString(mailContent));
        }

        /// <summary>
        /// 添加一个电子邮件不同格式的副本。
        /// </summary>
        /// <param name="contentStream">电子邮件内容流</param>
        public void AddAlterViewStream(Stream contentStream)
        {
            MailValidatorHelper.ValideArgumentNull<Stream>(contentStream, "contentStream");
            AlternateViews.Add(new AlternateView(contentStream));
        }

        /// <summary>
        /// 添加一个电子邮件不同格式的副本。
        /// </summary>
        /// <param name="alternateView">电子邮件视图</param>
        public void AddAlternateView(AlternateView alternateView)
        {
            MailValidatorHelper.ValideArgumentNull<AlternateView>(alternateView, "alternateView");
            AlternateViews.Add(alternateView);
        }

        #endregion

        #endregion

        #region 发送邮件 相关方法

        /// <summary>
        /// 计划批量发送邮件的个数，配合自动释放SmtpClient。（批量邮件发送不调用此方法就不会自动释放SmtpClient）
        /// 0、此方法可以在发送邮件方法之前或之后调用
        /// 1、只有设置后才会自动根据 m_autoDisposeSmtp 字段进行释放SmtpClient。
        /// 2、若 m_autoDisposeSmtp = false 即由自己手动进行设置的无需调用此方法设置预计邮件数
        /// </summary>
        /// <param name="preCount">计划邮件数量</param>
        public void SetBatchMailCount(long preCount)
        {
            PrepareSendCount = preCount;

            if (preCount < CompletedSendCount)
            {
                throw new ArgumentOutOfRangeException("preCount", MailValidatorHelper.EMAIL_ADDRESS_RANGE_ERROR);
            }
            else if (preCount == CompletedSendCount)
            {
                if (m_autoDisposeSmtp)
                    this.InnerDisposeSmtp();
            }
        }

        /// <summary>
        /// 同步发送一封Email
        /// </summary>
        public void SendOneMail()
        {
            m_PrepareSendCount = 1;
            this.InnerSendMessage();
        }

        /// <summary>
        /// 批量同步发送Email
        /// </summary>
        public void SendBatchMail()
        {
            this.InnerSendMessage();
        }

        /// <summary>
        /// 取消异步邮件发送
        /// </summary>
        public void SendAsyncCancel()
        {
            // 因为此类为非线程安全类，所以 SendAsyncCancel 和发送邮件方法中操作MessageQueue部分的代码肯定是串行化的。
            // 所以不存在一边入队，一边出队导致无法完全取消所有邮件发送

            // 1、清空队列。
            // 2、取消正在异步发送的mail。
            // 3、设置计划数量=完成数量
            // 4、执行 AutoDisposeSmtp() 

            if (m_IsAsync)
            {
                // 1、清空队列。
                MailUserState tempMailUserState = null;
                while (MessageQueue.TryDequeue(out tempMailUserState))
                {
                    Interlocked.Decrement(ref m_messageQueueCount);
                    MailMessage message = tempMailUserState.CurMailMessage;
                    this.InnerDisposeMessage(message);
                }
                tempMailUserState = null;
                // 2、取消正在异步发送的mail。
                m_SmtpClient.SendAsyncCancel();
                // 3、设置计划数量=完成数量
                PrepareSendCount = CompletedSendCount;
                // 4、执行 AutoDisposeSmtp() 
                this.AutoDisposeSmtp();
            }
            else
            {
                throw new Exception(MailValidatorHelper.EMAIL_ASYNC_CALL_ERROR);
            }
        }

        /// <summary>
        /// 发送Email
        /// </summary>
        private void InnerSendMessage()
        {

            bool hasError = false;
            MailMessage mMailMessage = null;

            #region 构建 MailMessage
            try
            {
                mMailMessage = new MailMessage();

                mMailMessage.From = new MailAddress(From, FromDisplayName);

                this.InnerSetAddress(EmailAddrType.To, mMailMessage);
                this.InnerSetAddress(EmailAddrType.CC, mMailMessage);
                this.InnerSetAddress(EmailAddrType.Bcc, mMailMessage);

                mMailMessage.Subject = Subject;
                mMailMessage.Body = Body;

                if (m_Attachments != null && m_Attachments.Count > 0)
                {
                    foreach (Attachment attachment in m_Attachments)
                    {
                        attachment.NameEncoding = Encoding.UTF8;
                        mMailMessage.Attachments.Add(attachment);
                    }
                }
                //Encoding chtEnc = Encoding.BigEndianUnicode;
                mMailMessage.SubjectEncoding = Encoding.UTF8;
                mMailMessage.BodyEncoding = Encoding.UTF8;
                // SmtpClient 的 Headers 中会根据 MailMessage 默认设置些值，所以应该为 UTF8 。
                mMailMessage.HeadersEncoding = Encoding.UTF8;

                mMailMessage.IsBodyHtml = IsBodyHtml;

                if (m_AlternateViews != null && m_AlternateViews.Count > 0)
                {
                    foreach (AlternateView alternateView in AlternateViews)
                    {
                        mMailMessage.AlternateViews.Add(alternateView);
                    }
                }

                mMailMessage.Priority = (MailPriority)Priority;
            }
            catch (ArgumentNullException argumentNullEx)
            {
                hasError = true;
                throw argumentNullEx;
            }
            catch (ArgumentException argumentEx)
            {
                hasError = true;
                throw argumentEx;
            }
            catch (FormatException formatEx)
            {
                hasError = true;
                throw formatEx;
            }
            finally
            {
                if (hasError)
                {
                    if (mMailMessage != null)
                    {
                        this.InnerDisposeMessage(mMailMessage);
                        mMailMessage = null;
                    }
                    this.InnerDisposeSmtp();
                }
            }

            #endregion

            if (!hasError)
            {
                if (m_IsAsync)
                {
                    #region 异步发送邮件

                    if (PrepareSendCount == 1)
                    {
                        // 情况一：不重用 SmtpClient 实例会将PrepareSendCount设置为1
                        // 情况二：计划发送只有一条

                        // PrepareSendCount 是发送单条邮件。
                        MailUserState state = new MailUserState()
                        {
                            AutoReleaseSmtp = m_autoDisposeSmtp,
                            CurMailMessage = mMailMessage,
                            CurSmtpClient = m_SmtpClient,
                            IsSmpleMail = true,
                            UserState = AsycUserState,
                        };
                        if (m_autoDisposeSmtp)
                            // 由发送完成回调函数根据 IsSmpleMail 字段进行释放
                            m_SmtpClient = null;

                        ThreadPool.QueueUserWorkItem((userState) =>
                        {
                            // 无需 catch 发送异常，因为是异步，所以这里 catch 不到。
                            MailUserState curUserState = userState as MailUserState;
                            curUserState.CurSmtpClient.SendAsync(mMailMessage, userState);
                        }, state);

                    }
                    else
                    {
                        // 情况一：重用 SmtpClient 逻辑，即我们可以直接操作全局的 m_SmtpClient 
                        // 情况二：批量发送邮件 PrepareSendCount>1
                        // 情况三：PrepareSendCount 还未设置，为0。比如场景在循环中做些判断，再决定发邮件，循环完才调用 SetBatchMailCount 设置计划邮件数量

                        MailUserState state = new MailUserState()
                        {
                            AutoReleaseSmtp = m_autoDisposeSmtp,
                            CurMailMessage = mMailMessage,
                            CurSmtpClient = m_SmtpClient,
                            UserState = AsycUserState,
                        };

                        MessageQueue.Enqueue(state);
                        Interlocked.Increment(ref m_messageQueueCount);

                        if (m_SendMailThread == null)
                        {
                            m_SendMailThread = new Thread(() =>
                            {
                                // noItemCount 次获取不到元素，就抛出线程异常
                                int noItemCount = 0;
                                while (true)
                                {
                                    if (PrepareSendCount != 0 && PrepareSendCount == CompletedSendCount)
                                    {
                                        // 已执行完毕。
                                        this.AutoDisposeSmtp();
                                        break;
                                    }
                                    else
                                    {
                                        MailUserState curUserState = null;

                                        if (!MessageQueue.IsEmpty)
                                        {
#if DEBUG
                                            Debug.WriteLine("WaitOne" + Thread.CurrentThread.ManagedThreadId);
#endif
                                            // 当执行异步取消时，会清空MessageQueue，所以 WaitOne 必须在从MessageQueue中取到元素之前
                                            AutoResetEvent.WaitOne();

                                            if (MessageQueue.TryDequeue(out curUserState))
                                            {
                                                Interlocked.Decrement(ref m_messageQueueCount);
                                                m_SmtpClient.SendAsync(curUserState.CurMailMessage, curUserState);
                                            }
                                        }
                                        else
                                        {
                                            if (noItemCount >= 10)
                                            {
                                                // 没有正确设置 PrepareSendCount 值。导致已没有邮件但此线程出现死循环
                                                this.InnerDisposeSmtp();

                                                throw new Exception(MailValidatorHelper.EMAIL_PREPARESENDCOUNT_NOTSET_ERROR);
                                            }

                                            Thread.Sleep(1000);
                                            noItemCount++;
                                        }
                                    }
                                    // SmtpClient 为null表示异步预计发送邮件数已经发送完，在 OnSendCompleted 进行了 m_SmtpClient 释放
                                    if (m_SmtpClient == null)
                                        break;
                                }

                                m_SendMailThread = null;
                            });
                            m_SendMailThread.Start();
                        }
                    }

                    #endregion
                }
                else
                {
                    #region 同步发送邮件
                    try
                    {
                        m_SmtpClient.Send(mMailMessage);
                        m_CompletedSendCount++;
                    }
                    catch (ObjectDisposedException smtpDisposedEx)
                    {
                        throw smtpDisposedEx;
                    }
                    catch (InvalidOperationException smtpOperationEx)
                    {
                        throw smtpOperationEx;
                    }
                    catch (SmtpFailedRecipientsException smtpFailedRecipientsEx)
                    {
                        throw smtpFailedRecipientsEx;
                    }
                    catch (SmtpException smtpEx)
                    {
                        throw smtpEx;
                    }
                    finally
                    {
                        if (mMailMessage != null)
                        {
                            this.InnerDisposeMessage(mMailMessage);
                            mMailMessage = null;
                        }
                        this.AutoDisposeSmtp();
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 将收件人、抄送人、密送人添加到 MailMessage 中
        /// </summary>
        /// <param name="type">收件人、抄送人、密送人</param>
        /// <param name="mMailMessage">待发送的MailMessage类</param>
        private void InnerSetAddress(EmailAddrType type, MailMessage mMailMessage)
        {
            MailAddressCollection receiveCol = null;
            Dictionary<string, string> dicReceive = null;
            bool hasAddress = false;
            switch (type)
            {
                case EmailAddrType.To:
                    {
                        if (m_DicTo != null && m_DicTo.Count > 0)
                        {
                            dicReceive = m_DicTo;
                            receiveCol = mMailMessage.To;
                            hasAddress = true;
                        }
                    }
                    break;
                case EmailAddrType.CC:
                    {
                        if (m_DicCC != null && m_DicCC.Count > 0)
                        {
                            dicReceive = m_DicCC;
                            receiveCol = mMailMessage.CC;
                            hasAddress = true;
                        }
                    }
                    break;
                case EmailAddrType.Bcc:
                    {
                        if (m_DicBcc != null && m_DicBcc.Count > 0)
                        {
                            dicReceive = m_DicBcc;
                            receiveCol = mMailMessage.Bcc;
                            hasAddress = true;
                        }
                    }
                    break;
                case EmailAddrType.From:
                    throw new Exception(MailValidatorHelper.EMAIL_ADDRESS_RANGE_ERROR);
            }
            if (hasAddress)
            {
                foreach (KeyValuePair<string, string> keyValue in dicReceive)
                {
                    receiveCol.Add(new MailAddress(keyValue.Key, keyValue.Value));
                }
            }
        }

        /// <summary>
        /// 释放 MailMessage 对象
        /// </summary>
        private void InnerDisposeMessage(MailMessage message)
        {
            if (message != null)
            {
                if (message.AlternateViews.Count > 0)
                {
                    message.AlternateViews.Dispose();
                }

                message.Dispose();
                message = null;
            }
        }

        /// <summary>
        /// 声明在 SmtpClient.SendAsync() 执行完后释放相关对象的回调方法   最后触发的委托
        /// </summary>
        protected void SendCompleted4Dispose(object sender, AsyncCompletedEventArgs e)
        {
            MailUserState state = e.UserState as MailUserState;

            if (state.CurMailMessage != null)
            {
                MailMessage message = state.CurMailMessage;
                this.InnerDisposeMessage(message);
                state.CurMailMessage = null;
            }

            if (state.IsSmpleMail)
            {
                if (state.AutoReleaseSmtp && state.CurSmtpClient != null)
                {
#if DEBUG
                    Debug.WriteLine("释放SmtpClient");
#endif
                    state.CurSmtpClient.Dispose();
                    state.CurSmtpClient = null;
                }
            }
            else
            {
                if (!e.Cancelled)   // 取消的就不计数
                    CompletedSendCount++;

                if (state.AutoReleaseSmtp)
                {
                    this.AutoDisposeSmtp();
                }

                // 若批量异步发送，需要设置信号
#if DEBUG
                Debug.WriteLine("Set" + Thread.CurrentThread.ManagedThreadId);
#endif
                AutoResetEvent.Set();
            }

            // 先释放资源，处理错误逻辑
            if (e.Error != null && !state.IsErrorHandle)
            {
                throw e.Error;
            }
        }

        #endregion

        #region 异步发送邮件，MessageQueue队列中缓冲的待发邮件数量，使用者可根据此数量来限制邮件数量，以免内存浪费

        /// <summary>
        /// 获取异步发送邮件，MessageQueue队列中缓冲的待发邮件数量
        /// （使用者可根据此数量来限制邮件数量，以免内存浪费）
        /// </summary>
        public int GetAwaitMailCountAsync()
        {
            if (m_IsAsync)
            {
                return Thread.VolatileRead(ref m_messageQueueCount);
            }
            else
            {
                throw new Exception(MailValidatorHelper.EMAIL_ASYNC_CALL_ERROR);
            }

        }

        #endregion

        #region 发送邮件前检查 相关方法

        /// <summary>
        /// 发送邮件前检查需要设置的信息是否完整，收集（提示+错误）信息
        /// </summary>
        public Dictionary<MailInfoType, string> CheckSendMail()
        {
            Dictionary<MailInfoType, string> dicMsg = new Dictionary<MailInfoType, string>();

            this.InnerCheckSendMail4Info(dicMsg);
            this.InnerCheckSendMail4Error(dicMsg);

            return dicMsg;
        }

        /// <summary>
        /// 发送邮件前检查需要设置的信息是否完整，收集 提示 信息
        /// </summary>
        public Dictionary<MailInfoType, string> CheckSendMail4Info()
        {
            Dictionary<MailInfoType, string> dicMsg = new Dictionary<MailInfoType, string>();

            this.InnerCheckSendMail4Info(dicMsg);

            return dicMsg;
        }

        /// <summary>
        /// 发送邮件前检查需要设置的信息是否完整，收集 错误 信息
        /// </summary>
        public Dictionary<MailInfoType, string> CheckSendMail4Error()
        {
            Dictionary<MailInfoType, string> dicMsg = new Dictionary<MailInfoType, string>();

            this.InnerCheckSendMail4Error(dicMsg);

            return dicMsg;
        }

        /// <summary>
        /// 发送邮件前检查需要设置的信息是否完整，收集 提示 信息
        /// </summary>
        /// <param name="dicMsg">将检查信息收集到此集合</param>
        private void InnerCheckSendMail4Info(Dictionary<MailInfoType, string> dicMsg)
        {
            // 注意每个验证使用完 infoBuilder 都要清零 infoBuilder 。
            StringBuilder infoBuilder = new StringBuilder(128);

            this.InnerCheckAddress(infoBuilder, dicMsg, EmailAddrType.CC);
            this.InnerCheckAddress(infoBuilder, dicMsg, EmailAddrType.Bcc);

            // 邮件主题
            if (Subject.Length == 0)
                dicMsg.Add(MailInfoType.SubjectEmpty, MailInfoHelper.GetMailInfoStr(MailInfoType.SubjectEmpty));

            // 邮件内容
            if (Body.Length == 0 &&
                (m_Attachments == null || (m_Attachments != null && m_Attachments.Count == 0))
                )
            {
                dicMsg.Add(MailInfoType.BodyEmpty, MailInfoHelper.GetMailInfoStr(MailInfoType.BodyEmpty));
            }
        }

        /// <summary>
        /// 发送邮件前检查需要设置的信息是否完整，收集 错误 信息
        /// </summary>
        /// <param name="dicMsg">将检查信息收集到此集合</param>
        private void InnerCheckSendMail4Error(Dictionary<MailInfoType, string> dicMsg)
        {
            // 注意每个验证使用完 infoBuilder 都要清零 infoBuilder 。
            StringBuilder infoBuilder = new StringBuilder(128);

            this.InnerCheckAddress(infoBuilder, dicMsg, EmailAddrType.From);
            this.InnerCheckAddress(infoBuilder, dicMsg, EmailAddrType.To);

            // SmtpClient 实例未设置
            if (m_SmtpClient == null)
                dicMsg.Add(MailInfoType.SmtpClientEmpty, MailInfoHelper.GetMailInfoStr(MailInfoType.SmtpClientEmpty));
            else
            {
                // SMTP 主服务器设置  （默认端口为25）
                if (m_SmtpClient.Host.Length == 0)
                    dicMsg.Add(MailInfoType.HostEmpty, MailInfoHelper.GetMailInfoStr(MailInfoType.HostEmpty));
                // SMPT 凭证
                if (m_SmtpClient.EnableSsl && m_SmtpClient.ClientCertificates.Count == 0)
                    dicMsg.Add(MailInfoType.CertificateEmpty, MailInfoHelper.GetMailInfoStr(MailInfoType.CertificateEmpty));
            }
        }

        /// <summary>
        /// 检查 发件人、收件人、抄送人、密送人 邮箱地址
        /// </summary>
        /// <param name="infoBuilder">StringBuilder实例</param>
        /// <param name="dicMsg">将检查信息收集到此集合</param>
        /// <param name="type">接收邮件地址类型</param>
        private void InnerCheckAddress(StringBuilder infoBuilder, Dictionary<MailInfoType, string> dicMsg, EmailAddrType type)
        {
            Dictionary<string, string> dic = null;
            MailInfoType addressFormat = MailInfoType.None;
            MailInfoType addressEmpty = MailInfoType.None;
            bool allowEmpty = true;
            // 只有 发件人 是单个地址，特别进行处理
            bool hasHandle = false;
            switch (type)
            {
                case EmailAddrType.From:
                    {
                        // 标识为已处理
                        hasHandle = true;

                        allowEmpty = false;
                        if (From.Length == 0)
                        {
                            dicMsg.Add(MailInfoType.FromEmpty, MailInfoHelper.GetMailInfoStr(MailInfoType.FromEmpty));
                        }
                        else if (!MailValidatorHelper.IsEmail(From))
                        {
                            string strTemp = infoBuilder.AppendFormat(MailInfoHelper.GetMailInfoStr(MailInfoType.FromFormat), FromDisplayName, From).ToString();
                            dicMsg.Add(MailInfoType.FromFormat, strTemp);
                            infoBuilder.Length = 0;
                        }
                    }
                    break;
                case EmailAddrType.To:
                    {
                        dic = m_DicTo;
                        addressEmpty = MailInfoType.ToEmpty;

                        allowEmpty = false;
                        addressFormat = MailInfoType.ToFormat;
                    }
                    break;
                case EmailAddrType.CC:
                    {
                        dic = m_DicCC;
                        addressFormat = MailInfoType.CCFormat;

                        allowEmpty = true;
                        addressEmpty = MailInfoType.None;
                    }
                    break;
                case EmailAddrType.Bcc:
                    {
                        dic = m_DicBcc;
                        addressFormat = MailInfoType.BccFormat;

                        allowEmpty = true;
                        addressEmpty = MailInfoType.None;
                    }
                    break;
            }


            #region 处理 收件人、抄送人、密送人

            if (!hasHandle)
            {
                if (dic == null)
                {
                    if (!allowEmpty)
                    {
                        // 地址为空
                        dicMsg.Add(addressEmpty, MailInfoHelper.GetMailInfoStr(addressEmpty));
                    }
                }
                else
                {
                    if (dic.Count > 0)
                    {
                        string strTemp = String.Empty;
                        // 邮件地址格式
                        foreach (KeyValuePair<string, string> keyValue in dic)
                        {
                            if (keyValue.Key.Length == 0)
                            {
                                if (!allowEmpty)
                                {
                                    // 地址为空
                                    dicMsg.Add(addressEmpty, MailInfoHelper.GetMailInfoStr(addressEmpty));
                                }
                            }
                            else if (!MailValidatorHelper.IsEmail(keyValue.Key))
                            {
                                if (strTemp.Length == 0)
                                    strTemp = MailInfoHelper.GetMailInfoStr(addressFormat);
                                if (infoBuilder.Length > 0)
                                    infoBuilder.AppendLine();
                                infoBuilder.AppendFormat(strTemp, keyValue.Value, keyValue.Key);
                            }
                        }
                        if (infoBuilder.Length > 0)
                        {
                            dicMsg.Add(addressFormat, infoBuilder.ToString());
                            infoBuilder.Length = 0;
                        }
                    }
                    else if (!allowEmpty)
                    {
                        // 地址为空
                        dicMsg.Add(addressEmpty, MailInfoHelper.GetMailInfoStr(addressEmpty));
                    }
                }
            }

            #endregion
        }

        #endregion

    }

    /// <summary>
    /// 异步发送邮件时保存的信息，用于释放和传递数据
    /// </summary>
    public class MailUserState
    {
        #region 由MailHelper内部的SendCompleted注册的事件使用
        // 用于释放 MailMessage 和 SmtpClient
        public MailMessage CurMailMessage { get; set; }
        public bool AutoReleaseSmtp { get; set; }
        public SmtpClient CurSmtpClient { get; set; }
        // 只发送单封邮件的时候使用此进行判断释放  
        public bool IsSmpleMail { get; set; }
        #endregion

        /// <summary>
        /// 用户传递的状态对象
        /// </summary>
        public object UserState { get; set; }

        /// <summary>
        /// 当异步发送报错时可通过此标识是否已经处理该异常
        /// </summary>
        public bool IsErrorHandle { get; set; }
    }

}

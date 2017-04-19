using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;  

namespace Rookey.Frame.Common.Web
{
    /// <summary>
    /// 异步监听httpHandler类
    /*--示例
    public class TestHandler : HttpAsyncHandler  
    {  
        public override void BeginProcess(System.Web.HttpContext context)  
        {  
            try  
            {  
                StreamReader sr = new StreamReader(context.Request.InputStream);  
                string reqStr = sr.ReadToEnd();  
                context.Response.Write("get your input : " + reqStr + " at " + DateTime.Now.ToString());  
            }  
            catch (Exception ex)  
            {  
                context.Response.Write("exception eccurs ex info : " + ex.Message);  
            }  
            finally  
            {  
                EndProcess();////最后别忘了end  
            }  
        }  
    }*/
    /// </summary>
    public abstract class HttpAsyncHandler : IHttpAsyncHandler, IAsyncResult  
    {  
        /// <summary>
        /// 开始处理请求
        /// </summary>
        /// <param name="context">上下文对象</param>
        /// <param name="cb"></param>
        /// <param name="extraData"></param>
        /// <returns></returns>
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)  
        {  
            _callback = cb;  
            _context = context;  
            _completed = false;  
            _state = this;  
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoProcess), this);  
            return this;  
        }  

        public void EndProcessRequest(IAsyncResult result)  
        {  
        }  
 
        public bool IsReusable  
        {  
            get { return false; }  
        }  
 
        /// <summary>
        /// 结束处理抽象方法
        /// </summary>
        /// <param name="context"></param>
        public abstract void BeginProcess(HttpContext context);  
        /// <summary>
        /// 结束处理
        /// </summary>
        public void EndProcess()  
        {  
            //防止多次进行多次EndProcess  
            if (!_completed)  
            {  
                try  
                {  
                    _completed = true;  
                    if (_callback != null)  
                    {  
                        _callback(this);  
                    }  
                }  
                catch (Exception) { }  
            }  
        }  
 
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="state"></param>
        private static void DoProcess(object state)  
        {  
            HttpAsyncHandler handler = (HttpAsyncHandler)state;  
            handler.BeginProcess(handler._context);  
        }  
 
        /// <summary>
        /// 接收请求
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)  
        {  
            throw new NotImplementedException();  
        }  
 
        private bool _completed;  
        private Object _state;  
        private AsyncCallback _callback;  
        private HttpContext _context;  
 
        public object AsyncState  
        {  
            get { return _state; }  
        }  
 
        public WaitHandle AsyncWaitHandle  
        {  
            get { throw new NotImplementedException(); }  
        }  
 
        public bool CompletedSynchronously  
        {  
            get { return false; }  
        }  
 
        public bool IsCompleted  
        {  
            get { return _completed; }  
        }  
    }  
}  


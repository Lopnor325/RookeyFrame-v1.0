using Rookey.Frame.Base;
using Rookey.Frame.Common;
using Rookey.Frame.Common.Web;
using System;
using System.Text;
using System.Web;

namespace Rookey.Frame.Controllers.Other
{
    /// <summary>
    /// 文件访问拦截
    /// </summary>
    public class FileAccessInterception : HttpAsyncHandler
    {
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        public override void BeginProcess(HttpContext context)
        {
            try
            {
                ProcessRequest(context);
            }
            catch (Exception ex)
            {
                context.Response.Write(string.Format("异常：{0}", ex.Message));
            }
            finally
            {
                EndProcess();
            }  
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void ProcessRequest(HttpContext context)
        {
            if (UserInfo.GetCurretnUser(context) != null)
            {
                string path = string.Empty;
                if (context.Request.RawUrl.Contains("?"))
                {
                    string temp = context.Request.RawUrl.Substring(0, context.Request.RawUrl.IndexOf("?"));
                    path = context.Server.MapPath(temp);
                }
                else
                {
                    path = context.Server.MapPath(context.Request.RawUrl);
                }
                try
                {
                    string ext = FileOperateHelper.GetFileExt(path);
                    context.Response.HeaderEncoding = Encoding.UTF8;
                    context.Response.ContentEncoding = Encoding.UTF8;
                    context.Response.ContentType = FileOperateHelper.GetHttpMIMEContentType(ext);
                    context.Response.WriteFile(path);
                }
                catch(Exception ex) 
                {
                    context.Response.Write(string.Format("异常：{0}", ex.Message));
                }
            }
            else
            {
                context.Response.Write("未登录前不能访问该资源，请登录后访问。<a href='/User/Login.html'>登录</a>");
            }
        }

        /// <summary>
        /// IsReusable
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

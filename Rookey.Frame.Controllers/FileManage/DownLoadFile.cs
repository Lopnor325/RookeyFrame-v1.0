using System;
using System.Collections.Generic;
using System.Web;

namespace Rookey.Frame.Controllers.FileManage
{
    /// <summary>
    /// 文件管理-下载
    /// </summary>
    public static class DownloadFile
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        /// <param name="hasfileName"></param>
        public static void ResponseFile(string path, HttpContext context, bool hasfileName)
        {
            context = HttpContext.Current;

            System.IO.Stream iStream = null;
            byte[] buffer = new Byte[10000];
            int length;
            long dataToRead;
            string filename;
            if (!hasfileName)
            {
                filename = System.IO.Path.GetFileName(path);
            }
            else
            {
                filename = "down_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
            }

            try
            {
                iStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                dataToRead = iStream.Length;
                context.Response.ContentType = "application/octet-stream";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8));

                while (dataToRead > 0)
                {
                    if (context.Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10000);
                        context.Response.OutputStream.Write(buffer, 0, length);
                        context.Response.Flush();

                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    iStream.Close();
                }
            }
        }
    }
}
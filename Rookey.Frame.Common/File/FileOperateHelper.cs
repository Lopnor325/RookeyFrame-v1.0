/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Common
{
    public static class FileOperateHelper
    {
        #region 文件操作

        /****************************************
         * 函数名称：ReadFile
         * 功能说明：读取文本内容
         * 参    数：Path:文件路径
         * 调用示列：
         *           string Path = Server.MapPath("Default2.aspx");       
         *           string s = Rookey.Common.FileOperate.ReadFile(Path);
        *****************************************/
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            string s = string.Empty;
            if (System.IO.File.Exists(path))
            {
                try
                {
                    StreamReader fr = new StreamReader(path, System.Text.Encoding.UTF8);
                    s = fr.ReadToEnd();
                    fr.Close();
                    fr.Dispose();
                }
                catch { }
            }
            else
            {
                s = string.Empty;
            }
            return s;
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileName(string file)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;
            try
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Exists) //文件存在
                {
                    return fi.Name;
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns></returns>
        public static string GetFileExtension(string file)
        {
            if (string.IsNullOrEmpty(file)) 
                return string.Empty;
            string ext = string.Empty;
            try
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Exists) //文件存在
                {
                    ext = fi.Extension;
                }
            }
            catch { }
            return ext;
        }

        /// <summary>
        /// 取文件后缀名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>.gif|.html格式</returns>
        public static string GetFileExt(string filename)
        {
            int start = filename.LastIndexOf(".");
            int length = filename.Length;
            string postfix = filename.Substring(start, length - start);
            return postfix;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns></returns>
        public static string GetFileSize(string file)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;
            try
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Exists) //文件存在
                {
                    return FileSize(fi.Length);
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 将文件大小转换输出
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static string FileSize(long Size)
        {
            string m_strSize = "";
            long FactSize = 0;
            FactSize = Size;
            if (FactSize < 1024.00)
                m_strSize = FactSize.ToString() + "Byte";
            else if (FactSize >= 1024.00 && FactSize < 1048576)
                m_strSize = (FactSize / 1024.00).ToString("F2") + "K";
            else if (FactSize >= 1048576 && FactSize < 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + "M";
            else if (FactSize >= 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + "G";

            return m_strSize;
        }

        #endregion

        #region 其他

        /// <summary>
        /// 获取响应内容类型
        /// </summary>
        /// <param name="ext">文件后缀</param>
        public static string GetHttpMIMEContentType(string ext)
        {
            switch (ext.ToLower())
            {
                case ".asa":
                    return "text/asa";
                case ".asp":
                    return "text/asp";
                case ".awf":
                    return "application/vnd.adobe.workflow";
                case ".bmp":
                    return "application/x-bmp";
                case ".doc":
                case ".docx":
                    return "application/msword";
                case ".gif":
                    return "image/gif";
                case ".htc":
                    return "text/x-component";
                case ".html":
                case ".htx":
                case ".htm":
                case ".jsp":
                case ".php":
                case ".xhtml":
                    return "text/html";
                case ".ico":
                    return "image/x-icon";
                case ".img":
                    return "application/x-img";
                case ".java":
                    return "java/*";
                case ".jpe":
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".mdb":
                    return "application/msaccess";
                case ".mhtml":
                case ".mht":
                    return "message/rfc822";
                case ".mp2v":
                    return "video/mpeg";
                case ".mp4":
                    return "video/mpeg4";
                case ".mpd":
                    return "application/vnd.ms-project";
                case ".mpeg":
                    return "video/mpg";
                case ".mpga":
                    return "audio/rn-mpeg";
                case ".mps":
                    return "video/x-mpeg";
                case ".mpv":
                    return "video/mpg";
                case ".mpw":
                    return "application/vnd.ms-project";
                case ".mtx":
                case ".biz":
                case ".xml":
                case ".dtd":
                case ".mml":
                case ".tld":
                case ".xslt":
                case ".xsd":
                    return "text/xml";
                case ".png":
                    return "image/png";
                case ".pdf":
                    return "application/pdf";
                case ".ppt":
                case ".pptx":
                    return "application/vnd.ms-powerpoint";
                case ".rm":
                    return "application/vnd.rn-realmedia";
                case ".xls":
                case ".xlsx":
                    return "application/vnd.ms-excel";
                case ".wav":
                    return "audio/wav";
                case ".wma":
                    return "audio/x-ms-wma";
                case ".wmv":
                    return "video/x-ms-wmv";
                case ".apk":
                    return "application/vnd.android.package-archive";
                case ".avi":
                    return "video/avi";
                case ".class":
                    return "java/*";
                case ".css":
                    return "text/css";
                case ".htt":
                    return "text/webviewhtml";
                case ".js":
                case ".ls":
                case ".ts":
                    return "application/x-javascript";
                case ".mac":
                    return "application/x-mac";
                case ".mns":
                    return "audio/x-musicnet-stream";
                case ".mp2":
                    return "audio/mp2";
                case ".mp3":
                    return "audio/mp3";
                case ".mpa":
                    return "video/x-mpg";
                case ".mpe":
                    return "video/x-mpeg";
                case ".mpg":
                    return "video/mpg";
                case ".mpp":
                case ".mpt":
                    return "application/vnd.ms-project";
                case ".mpv2":
                    return "video/mpeg";
                case ".mpx":
                    return "application/vnd.ms-project";
                case ".ps":
                    return "application/x-ps";
                case ".txt":
                case ".sql":
                    return "text/plain";
                case ".vsd":
                case ".vss":
                case ".vsx":
                    return "application/vnd.visio";
                case ".wmd":
                    return "application/x-ms-wmd";
                case ".wmx":
                    return "video/x-ms-wmx";
                case ".xap":
                    return "application/x-silverlight-app";
                case ".zip":
                case ".rar":
                case ".7z":
                    return "application/x-zip-compressed";
                default:
                    return "application/octet-stream";
            }
        }

        #endregion
    }
}

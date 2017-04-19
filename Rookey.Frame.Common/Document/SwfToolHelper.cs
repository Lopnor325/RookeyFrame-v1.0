/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// SWF文件转换工具类
    /// </summary>
    public class SwfToolHelper
    {
        /// <summary>
        /// 转换所有的页，图片质量80%
        /// </summary>
        /// <param name="pdfPath">PDF文件地址</param>
        /// <param name="swfPath">生成后的SWF文件地址</param>
        /// <param name="exePath">exe路径</param>
        /// <param name="binPath"></param>
        public static bool PDF2SWF(string pdfPath, string swfPath, string exePath, string binPath)
        {
            return PDF2SWF(pdfPath, swfPath, 1, GetPageCount(pdfPath), 80, exePath, binPath);
        }

        /// <summary>
        /// 转换前N页，图片质量80%
        /// </summary>
        /// <param name="pdfPath">PDF文件地址</param>
        /// <param name="swfPath">生成后的SWF文件地址</param>
        /// <param name="page">页数</param>
        /// <param name="exePath">exe路径</param>
        /// <param name="binPath"></param>
        public static bool PDF2SWF(string pdfPath, string swfPath, int page, string exePath, string binPath)
        {
            return PDF2SWF(pdfPath, swfPath, 1, page, 80, exePath, binPath);
        }

        /// <summary>
        /// PDF格式转为SWF
        /// </summary>
        /// <param name="pdfPath">PDF文件地址</param>
        /// <param name="swfPath">生成后的SWF文件地址</param>
        /// <param name="beginpage">转换开始页</param>
        /// <param name="endpage">转换结束页</param>
        /// <param name="photoQuality"></param>
        /// <param name="exePath"></param>
        /// <param name="binPath"></param>
        private static bool PDF2SWF(string pdfPath, string swfPath, int beginpage, int endpage, int photoQuality, string exePath, string binPath)
        {
            //swftool，首先先安装，然后将安装目录下的东西拷贝到tools目录下
            if (!System.IO.File.Exists(exePath) || !System.IO.File.Exists(pdfPath) || System.IO.File.Exists(swfPath))
            {
                return false;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(" \"" + pdfPath + "\"");
            sb.Append(" -o \"" + swfPath + "\"");
            sb.Append(" -s flashversion=9");
            if (endpage > GetPageCount(pdfPath)) endpage = GetPageCount(pdfPath);
            sb.Append(" -p " + "\"" + beginpage + "" + "-" + endpage + "\"");
            sb.Append(" -j " + photoQuality);
            string Command = sb.ToString();
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = Command;
            p.StartInfo.WorkingDirectory = binPath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
            p.BeginErrorReadLine();
            p.WaitForExit();
            p.Close();
            p.Dispose();
            return true;
        }

        /// <summary>
        /// 返回页数
        /// </summary>
        /// <param name="pdfPath">PDF文件地址</param>
        private static int GetPageCount(string pdfPath)
        {
            byte[] buffer = System.IO.File.ReadAllBytes(pdfPath);
            int length = buffer.Length;
            if (buffer == null)
                return -1;
            if (buffer.Length <= 0)
                return -1;
            string pdfText = Encoding.Default.GetString(buffer);
            System.Text.RegularExpressions.Regex rx1 = new System.Text.RegularExpressions.Regex(@"/Type\s*/Page[^s]");
            System.Text.RegularExpressions.MatchCollection matches = rx1.Matches(pdfText);
            return matches.Count;
        }
    }
}

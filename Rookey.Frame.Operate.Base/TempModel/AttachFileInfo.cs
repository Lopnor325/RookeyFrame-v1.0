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

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 附件信息
    /// </summary>
    public class AttachFileInfo
    {
        /// <summary>
        /// 附件Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string AttachFile { get; set; }
        /// <summary>
        /// pdf路径
        /// </summary>
        public string PdfFile { get; set; }
        /// <summary>
        /// swf路径
        /// </summary>
        public string SwfFile { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }
        /// <summary>
        /// 文件描述
        /// </summary>
        public string FileDes { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string AttachType { get; set; }
    }

}

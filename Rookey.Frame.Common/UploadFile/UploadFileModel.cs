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
    public class UploadFileModel
    {
        /// <summary>
        /// 上传文件原始名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 带路径的上传文件名称
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }

    }
}

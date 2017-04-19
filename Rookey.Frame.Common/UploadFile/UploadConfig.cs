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
    /// 上传信息配置
    /// </summary>
    public class UploadConfig
    {

        /// <summary>
        /// 允许上传的文件类型
        /// </summary>
        public List<string> AllowType { get; set; }

        /// <summary>
        /// 上传文件最大尺寸,单位KB
        /// </summary>
        public int MaxFileSize { get; set; }


        /// <summary>
        /// 最多允许上传文件个数
        /// </summary>
        public int QueueSizeLimit { get; set; }

        /// <summary>
        /// 上传文件的基目录
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// 上传文件缓存目录
        /// </summary>
        public string TempPath { get; set; }

        /// <summary>
        /// 实际保存目录
        /// </summary>
        public string Folder { get; set; }

    }
}

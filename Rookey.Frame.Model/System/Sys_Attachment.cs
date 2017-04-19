/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using ServiceStack.DataAnnotations;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.EntityBase;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 附件信息
    /// </summary>
    [ModuleConfig(Name = "附件信息", PrimaryKeyFields = "Sys_ModuleId,RecordId,FileName", Sort = 14, IsAllowAdd = false, IsAllowEdit = false, StandardJsFolder = "System")]
    public class Sys_Attachment : BaseSysEntity
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        [FieldConfig(Display = "模块", ControlType = (int)ControlTypeEnum.DialogGrid, IsFrozen = true, RowNum = 1, ColNum = 1, HeadSort = 1, IsRequired = true, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 记录Id
        /// </summary>
        [FieldConfig(Display = "记录Id", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 1, ColNum = 2, HeadSort = 2)]
        public Guid? RecordId { get; set; }

        /// <summary>
        /// 记录的TitleKey值
        /// </summary>
        [FieldConfig(Display = "记录的TitleKey值", RowNum = 2, ColNum = 1, HeadSort = 3)]
        [StringLength(300)]
        public string RecordTitleKeyValue { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [FieldConfig(Display = "文件名称", RowNum = 2, ColNum = 2, HeadSort = 4)]
        [StringLength(200)]
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [FieldConfig(Display = "文件类型", RowNum = 3, ColNum = 1, HeadSort = 5)]
        [StringLength(50)]
        public string FileType { get; set; }

        /// <summary>
        /// 文件说明
        /// </summary>
        [FieldConfig(Display = "文件说明", RowNum = 3, ColNum = 2, HeadSort = 6)]
        [StringLength(200)]
        public string FileDes { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [FieldConfig(Display = "文件大小", RowNum = 4, ColNum = 1, HeadSort = 7)]
        [StringLength(50)]
        public string FileSize { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [FieldConfig(Display = "文件路径", RowNum = 4, ColNum = 2, HeadSort = 8)]
        [StringLength(500)]
        public string FileUrl { get; set; }

        /// <summary>
        /// PDF文件路径
        /// </summary>
        [FieldConfig(Display = "PDF文件路径", RowNum = 5, ColNum = 1, HeadSort = 9)]
        [StringLength(500)]
        public string PdfUrl { get; set; }

        /// <summary>
        /// SWF文件路径
        /// </summary>
        [FieldConfig(Display = "SWF文件路径", RowNum = 5, ColNum = 2, HeadSort = 10)]
        [StringLength(500)]
        public string SwfUrl { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        [FieldConfig(Display = "缩略图", RowNum = 6, ColNum = 1, HeadSort = 11)]
        [StringLength(500)]
        public string Thumbnail { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        [FieldConfig(Display = "附件类型", RowNum = 6, ColNum = 2, HeadSort = 12)]
        [StringLength(50)]
        public string AttachType { get; set; }
    }
}

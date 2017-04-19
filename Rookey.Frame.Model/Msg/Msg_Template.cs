/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Msg
{
    /// <summary>
    /// 消息模板
    /// </summary>
    [ModuleConfig(Name = "消息模板", PrimaryKeyFields = "Name", TitleKey = "Name", Sort = 90, StandardJsFolder = "Msg")]
    public class Msg_Template : BaseMsgEntity
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        [FieldConfig(Display = "模板名称", IsUnique = true, IsRequired = true, RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        [FieldConfig(Display = "模板类型", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2)]
        public int TemplateType { get; set; }

        /// <summary>
        /// 模板类型（枚举）
        /// </summary>
        [Ignore]
        public MsgTemplateTypeEnum TemplateTypeOfEnum
        {
            get
            {
                return (MsgTemplateTypeEnum)Enum.Parse(typeof(MsgTemplateTypeEnum), TemplateType.ToString());
            }
            set { TemplateType = (int)value; }
        }

        /// <summary>
        /// 模板标题
        /// </summary>
        [FieldConfig(Display = "模板标题", IsRequired = true, ControlWidth = 490, RowNum = 2, ColNum = 1, HeadSort = 3)]
        [StringLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        [FieldConfig(Display = "模板内容", IsRequired = true, ControlType = (int)ControlTypeEnum.RichTextBox, ControlWidth = 490, RowNum = 3, ColNum = 1, IsEnableGrid = false)]
        [StringLength(int.MaxValue)]
        public string Content { get; set; }

        /// <summary>
        /// 有效开始时间
        /// </summary>
        [FieldConfig(Display = "有效开始时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 4, ColNum = 1, HeadSort = 5)]
        public DateTime? ValidStartTime { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        [FieldConfig(Display = "有效结束时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 4, ColNum = 2, HeadSort = 6)]
        public DateTime? ValidEndTime { get; set; }
    }
}

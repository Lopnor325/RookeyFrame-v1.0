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

namespace Rookey.Frame.Model.Bpm
{
    /// <summary>
    /// 流程结点实例
    /// </summary>
    [ModuleConfig(Name = "结点实例", PrimaryKeyFields = "Bpm_WorkFlowInstanceId,Bpm_WorkNodeId", TitleKey = "SerialNo", Sort = 77, StandardJsFolder = "Bpm")]
    public class Bpm_WorkNodeInstance : BaseBpmEntity
    {
        /// <summary>
        /// 流水号
        /// </summary>
        [FieldConfig(Display = "流水号", RowNum = 1, ColNum = 1, IsRequired = true, IsUnique = true, HeadSort = 1)]
        [StringLength(100)]
        public string SerialNo { get; set; }

        /// <summary>
        /// 流程实例Id
        /// </summary>
        [FieldConfig(Display = "流程实例", ControlType = (int)ControlTypeEnum.DialogGrid, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "流程实例")]
        public Guid? Bpm_WorkFlowInstanceId { get; set; }

        /// <summary>
        /// 流程实例
        /// </summary>
        [Ignore]
        public string Bpm_WorkFlowInstanceName { get; set; }

        /// <summary>
        /// 结点Id
        /// </summary>
        [FieldConfig(Display = "流程结点", ControlType = (int)ControlTypeEnum.DialogGrid, IsRequired = true, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "流程结点")]
        public Guid? Bpm_WorkNodeId { get; set; }

        /// <summary>
        /// 结点名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkNodeName { get; set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        [FieldConfig(Display = "启动时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 3, ColNum = 1, HeadSort = 4)]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [FieldConfig(Display = "完成时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 3, ColNum = 2, HeadSort = 5)]
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [FieldConfig(Display = "状态", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 4, ColNum = 1, HeadSort = 6)]
        public int Status { get; set; }

        /// <summary>
        /// 状态（枚举）
        /// </summary>
        [Ignore]
        public WorkNodeStatusEnum StatusOfEnum
        {
            get
            {
                return (WorkNodeStatusEnum)Enum.Parse(typeof(WorkNodeStatusEnum), Status.ToString());
            }
            set { Status = (int)value; }
        }
    }
}

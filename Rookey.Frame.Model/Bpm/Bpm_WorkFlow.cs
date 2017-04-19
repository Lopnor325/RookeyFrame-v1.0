/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Rookey.Frame.Model.Bpm
{
    /// <summary>
    /// 流程信息
    /// </summary>
    [ModuleConfig(Name = "流程信息", PrimaryKeyFields = "Name", TitleKey = "Name", Sort = 71, StandardJsFolder = "Bpm")]
    public class Bpm_WorkFlow : BaseBpmEntity
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        [FieldConfig(Display = "流程名称", IsUnique = true, IsRequired = true, RowNum = 1, ColNum = 1, HeadSort = 1)]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [FieldConfig(Display = "显示名称", RowNum = 1, ColNum = 2, DefaultValue = "{Name}", HeadSort = 2)]
        [StringLength(200)]
        public string DisplayName { get; set; }

        /// <summary>
        /// 所属分类Id
        /// </summary>
        [FieldConfig(Display = "所属分类", ControlType = (int)ControlTypeEnum.ComboTree, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "流程分类")]
        public Guid? Bpm_FlowClassId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Ignore]
        public string Bpm_FlowClassName { get; set; }

        /// <summary>
        /// 关联模块
        /// </summary>
        [FieldConfig(Display = "关联模块", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 2, ColNum = 2, HeadSort = 4, ForeignModuleName = "模块管理")]
        public Guid? Sys_ModuleId { get; set; }

        /// <summary>
        /// 关联模块名称
        /// </summary>
        [Ignore]
        public string Sys_ModuleName { get; set; }

        /// <summary>
        /// 有效开始时间
        /// </summary>
        [FieldConfig(Display = "有效开始时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public DateTime? ValidStartTime { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        [FieldConfig(Display = "有效结束时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 3, ColNum = 2, HeadSort = 6)]
        public DateTime? ValidEndTime { get; set; }

        /// <summary>
        /// 管理人，流程异常时跳到管理人处理
        /// </summary>
        [FieldConfig(Display = "管理人", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 4, ColNum = 1, HeadWidth = 80, HeadSort = 7, ForeignModuleName = "员工管理")]
        public Guid? OrgM_EmpId { get; set; }

        /// <summary>
        /// 管理人
        /// </summary>
        [Ignore]
        public string OrgM_EmpName { get; set; }

        /// <summary>
        /// 节点集合
        /// </summary>
        [Ignore]
        public List<Bpm_WorkNode> WorkNodes { get; set; }

        /// <summary>
        /// 连线集合
        /// </summary>
        [Ignore]
        public List<Bpm_WorkLine> WorkLines { get; set; }
    }
}

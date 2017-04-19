/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using System.Collections.Generic;

namespace Rookey.Frame.Model.Bpm
{
    /// <summary>
    /// 流程结点
    /// </summary>
    [ModuleConfig(Name = "流程结点", PrimaryKeyFields = "Bpm_WorkFlowId,Name", TitleKey = "Name", Sort = 73, StandardJsFolder = "Bpm")]
    public class Bpm_WorkNode : BaseBpmEntity
    {
        /// <summary>
        /// 节点编码
        /// </summary>
        [FieldConfig(Display = "结点编码", RowNum = 1, ColNum = 1, IsRequired = true, IsAllowAdd = false, IsAllowEdit = false, HeadSort = 1)]
        [StringLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// 流程信息Id
        /// </summary>
        [FieldConfig(Display = "流程信息", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 1, ColNum = 2, HeadSort = 2, ForeignModuleName = "流程信息")]
        public Guid? Bpm_WorkFlowId { get; set; }

        /// <summary>
        /// 流程信息
        /// </summary>
        [Ignore]
        public string Bpm_WorkFlowName { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [FieldConfig(Display = "结点名称", RowNum = 2, ColNum = 1, IsRequired = true, HeadSort = 3)]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [FieldConfig(Display = "显示名称", RowNum = 2, ColNum = 2, DefaultValue = "{Name}", HeadSort = 4)]
        [StringLength(200)]
        public string DisplayName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [FieldConfig(Display = "排序", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 3, ColNum = 1, HeadSort = 5)]
        public int Sort { get; set; }

        /// <summary>
        /// 节点表单
        /// </summary>
        [FieldConfig(Display = "结点表单", ControlType = (int)ControlTypeEnum.ComboBox, IsRequired = true, RowNum = 3, ColNum = 2, HeadSort = 6, ForeignModuleName = "表单管理")]
        public Guid? Sys_FormId { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        [Ignore]
        public string Sys_FormName { get; set; }

        /// <summary>
        /// 表单URL
        /// </summary>
        [FieldConfig(Display = "表单URL", RowNum = 4, ColNum = 1, ControlWidth = 490, HeadSort = 7)]
        [StringLength(500)]
        public string FormUrl { get; set; }

        /// <summary>
        /// 处理者类型
        /// </summary>
        [FieldConfig(Display = "处理者类型", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 5, ColNum = 1, HeadSort = 8)]
        public int HandlerType { get; set; }

        /// <summary>
        /// 处理者类型（枚举）
        /// </summary>
        [Ignore]
        public NodeHandlerTypeEnum HandlerTypeOfEnum
        {
            get
            {
                return (NodeHandlerTypeEnum)Enum.Parse(typeof(NodeHandlerTypeEnum), HandlerType.ToString());
            }
            set { HandlerType = (int)value; }
        }

        /// <summary>
        /// 处理范围
        /// </summary>
        [FieldConfig(Display = "处理范围", RowNum = 5, ColNum = 2, HeadSort = 9)]
        [StringLength(4000)]
        public string HandleRange { get; set; }

        /// <summary>
        /// 处理策略
        /// </summary> 
        [FieldConfig(Display = "处理策略", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 6, ColNum = 1, HeadSort = 10)]
        public int HandleStrategy { get; set; }

        /// <summary>
        /// 处理策略（枚举）
        /// </summary>
        [Ignore]
        public HandleStrategyTypeEnum HandleStrategyOfEnum
        {
            get
            {
                return (HandleStrategyTypeEnum)Enum.Parse(typeof(HandleStrategyTypeEnum), HandleStrategy.ToString());
            }
            set { HandleStrategy = (int)value; }
        }

        /// <summary>
        /// 表单字段
        /// </summary>
        [FieldConfig(Display = "表单字段", RowNum = 6, ColNum = 2, HeadSort = 11)]
        [StringLength(50)]
        public string FormFieldName { get; set; }

        /// <summary>
        /// 自动跳转规则
        /// </summary>
        [FieldConfig(Display = "自动跳转规则", ControlType = (int)ControlTypeEnum.MutiCheckBox, TextField = "处理者是发起者,处理者已出现过,处理者与上一步相同,找不到处理者,提示错误", ValueField = "0,0,0,0,0",DefaultValue="1,1,1,0,0", RowNum = 7, ColNum = 1, ControlWidth = 490, HeadSort = 12)]
        [StringLength(50)]
        public string AutoJumpRule { get; set; }

        /// <summary>
        /// 回退类型设置
        /// </summary>
        [FieldConfig(Display = "回退类型设置", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 8, ColNum = 1, HeadSort = 13)]
        public int BackType { get; set; }

        /// <summary>
        /// 回退类型设置（枚举）
        /// </summary>
        [Ignore]
        public NodeBackTypeEnum BackTypeOfEnum
        {
            get
            {
                return (NodeBackTypeEnum)Enum.Parse(typeof(NodeBackTypeEnum), BackType.ToString());
            }
            set { BackType = (int)value; }
        }

        /// <summary>
        /// 结点类型
        /// </summary>
        [FieldConfig(Display = "结点类型", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 8, ColNum = 2, DefaultValue = "0", HeadSort = 14)]
        public int WorkNodeType { get; set; }

        /// <summary>
        /// 结点类型（枚举）
        /// </summary>
        [Ignore]
        public WorkNodeTypeEnum WorkNodeTypeOfEnum
        {
            get
            {
                return (WorkNodeTypeEnum)Enum.Parse(typeof(WorkNodeTypeEnum), WorkNodeType.ToString());
            }
            set { WorkNodeType = (int)value; }
        }

        /// <summary>
        /// 子流程信息
        /// </summary>
        [FieldConfig(Display = "子流程", ControlType = (int)ControlTypeEnum.DialogGrid, RowNum = 9, ColNum = 1, HeadSort = 15, ForeignModuleName = "流程信息")]
        public Guid? Bpm_WorkFlowSubId { get; set; }

        /// <summary>
        /// 子流程类型
        /// </summary>
        [FieldConfig(Display = "子流程类型", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 9, ColNum = 2, HeadSort = 16)]
        public int SubFlowType { get; set; }

        /// <summary>
        /// 子流程类型（枚举）
        /// </summary>
        [Ignore]
        public SubFlowTypeEnum SubFlowTypeOfEnum
        {
            get
            {
                return (SubFlowTypeEnum)Enum.Parse(typeof(SubFlowTypeEnum), SubFlowType.ToString());
            }
            set { SubFlowType = (int)value; }
        }

        /// <summary>
        /// 子流程名称
        /// </summary>
        [Ignore]
        public string Bpm_WorkFlowSubName { get; set; }

        /// <summary>
        /// 节点宽度
        /// </summary>
        [NoField]
        public int Width { get; set; }

        /// <summary>
        /// 节点高度
        /// </summary>
        [NoField]
        public int Height { get; set; }

        /// <summary>
        /// 纵坐标
        /// </summary>
        [NoField]
        public int Top { get; set; }

        /// <summary>
        /// 横坐标
        /// </summary>
        [NoField]
        public int Left { get; set; }

        /// <summary>
        /// 标签id
        /// </summary>
        [NoField]
        [StringLength(100)]
        public string TagId { get; set; }

        /// <summary>
        /// 结点审批按钮配置
        /// </summary>
        [Ignore]
        public List<Bpm_NodeBtnConfig> BtnConfigs { get; set; }
    }
}

/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 表单数据对象
    /// </summary>
    [Serializable]
    public class FormDataObject
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 模块数据
        /// </summary>
        public List<NameValueObject> ModuleData { get; set; }

        /// <summary>
        /// 模块明细
        /// </summary>
        public List<DetailFormObject> Details { get; set; }

        /// <summary>
        /// 是否保存为草稿
        /// </summary>
        public bool IsDraft { get; set; }

        /// <summary>
        /// 是否发布草稿
        /// </summary>
        public bool IsReleaseDraft { get; set; }

        #region 流程相关参数
        /// <summary>
        /// 操作的流程按钮ID，为0时为提交流程，大于0时为审批流程
        /// </summary>
        public Guid? OpFlowBtnId { get; set; }

        /// <summary>
        /// 待办任务ID，如果当前为子流程审批，则当前为父待办ID
        /// </summary>
        public Guid? ToDoTaskId { get; set; }

        /// <summary>
        /// 子流程待办ID集合，针对子流程审批用到
        /// </summary>
        public string ChildTodoIds { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string ApprovalOpinions { get; set; }

        /// <summary>
        /// 回退结点ID
        /// </summary>
        public Guid? ReturnNodeId { get; set; }

        /// <summary>
        /// 被指派处理人ID
        /// </summary>
        public Guid? DirectHandler { get; set; }
        #endregion
    }

    /// <summary>
    /// 键值对象
    /// </summary>
    [Serializable]
    public class NameValueObject
    {
        /// <summary>
        /// 键
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// 明细表单对象
    /// </summary>
    [Serializable]
    public class DetailFormObject
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 明细行数据
        /// </summary>
        public List<string> ModuleDatas { get; set; }
    }
}

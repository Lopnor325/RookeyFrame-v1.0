/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.EnumDef
{
    /// <summary>
    /// 数据网格类型
    /// </summary>
    public enum DataGridType
    {
        /// <summary>
        /// 主网格
        /// </summary>
        [Description("主网格")]
        MainGrid = 0,

        /// <summary>
        /// 弹出选择网格
        /// </summary>
        [Description("弹出选择网格")]
        DialogGrid = 1,

        /// <summary>
        /// 编辑页面明细网格
        /// </summary>
        [Description("编辑页面明细网格")]
        EditDetailGrid = 2,

        /// <summary>
        /// 查看页面明细网格
        /// </summary>
        [Description("查看页面明细网格")]
        ViewDetailGrid = 3,

        /// <summary>
        /// 网格内明细网格
        /// </summary>
        [Description("网格内明细网格")]
        InnerDetailGrid = 4,

        /// <summary>
        /// 主网格下方网格
        /// </summary>
        [Description("主网格下方网格")]
        FlowGrid = 5,

        /// <summary>
        /// 回收站网格
        /// </summary>
        [Description("回收站网格")]
        RecycleGrid = 6,

        /// <summary>
        /// 我的草稿网格
        /// </summary>
        [Description("我的草稿网格")]
        MyDraftGrid = 7,

        /// <summary>
        /// 桌面配置网格
        /// </summary>
        [Description("桌面配置网格")]
        DesktopGrid = 8,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 10
    }

    /// <summary>
    /// 按钮图标类型
    /// </summary>
    public enum ButtonIconType
    {
        /// <summary>
        /// 保存
        /// </summary>
        [Description("保存")]
        Save = 0,

        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        Edit = 1,

        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Close = 2,

        /// <summary>
        /// 保存并新增
        /// </summary>
        [Description("保存并新增")]
        SaveAndNew = 3,

        /// <summary>
        /// 保存为草稿
        /// </summary>
        [Description("保存为草稿")]
        SaveDraft = 4,

        /// <summary>
        /// 保存并发布
        /// </summary>
        [Description("保存并发布")]
        DraftRelease = 5,

        /// <summary>
        /// 审批同意
        /// </summary>
        [Description("审批同意")]
        FlowAgree = 6,

        /// <summary>
        /// 审批拒绝
        /// </summary>
        [Description("审批拒绝")]
        FlowReject = 7,

        /// <summary>
        /// 审批回退
        /// </summary>
        [Description("审批回退")]
        FlowReturn = 8,

        /// <summary>
        /// 审批指派
        /// </summary>
        [Description("审批指派")]
        FlowDirect = 9,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 10
    }
}

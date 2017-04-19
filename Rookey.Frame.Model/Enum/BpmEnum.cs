/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System.ComponentModel;

namespace Rookey.Frame.Model.EnumSpace
{
    /// <summary>
    /// 流程状态
    /// </summary>
    public enum WorkFlowStatusEnum
    {
        /// <summary>
        /// 流程已发起
        /// </summary>
        [Description("已发起")]
        Start = 1,

        /// <summary>
        /// 流程审批中
        /// </summary>
        [Description("审批中")]
        Approving = 2,

        /// <summary>
        /// 流程被回退
        /// </summary>
        [Description("被退回")]
        Return = 3,

        /// <summary>
        /// 流程被拒绝
        /// </summary>
        [Description("被拒绝")]
        Refused = 4,

        /// <summary>
        /// 流程被冻结
        /// </summary>
        [Description("被冻结")]
        Freezed = 5,

        /// <summary>
        /// 已通过,流程结束
        /// </summary>
        [Description("已通过")]
        Over = 10,

        /// <summary>
        /// 无状态，未发起
        /// </summary>
        [Description("未提交")]
        NoStatus = 0,

        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义")]
        Customer = -1,
    }

    /// <summary>
    /// 结点状态
    /// </summary>
    public enum WorkNodeStatusEnum
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        Undo = 0,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Do = 1,

        /// <summary>
        /// 暂停
        /// </summary>
        [Description("暂停")]
        Stop = 2
    }

    /// <summary>
    /// 操作动作
    /// </summary>
    public enum WorkActionEnum
    {
        /// <summary>
        /// 未执行
        /// </summary>
        [Description("未执行")]
        NoAction = 1,

        /// <summary>
        /// 发起流程
        /// </summary>
        [Description("发起流程")]
        Starting = 2,

        /// <summary>
        /// 审批流程
        /// </summary>
        [Description("审批流程")]
        Approving = 3,

        /// <summary>
        /// 回退流程
        /// </summary>
        [Description("回退流程")]
        Returning = 4,

        /// <summary>
        /// 指派流程
        /// </summary>
        [Description("指派流程")]
        Directing = 5,

        /// <summary>
        /// 拒绝流程
        /// </summary>
        [Description("拒绝流程")]
        Refusing = 6,

        /// <summary>
        /// 沟通流程
        /// </summary>
        [Description("沟通流程")]
        Communicating = 7,

        /// <summary>
        /// 发起子流程
        /// </summary>
        [Description("发起子流程")]
        SubStarting = 8,

        /// <summary>
        /// 重新发起
        /// </summary>
        [Description("重新发起")]
        ReStarting = 9
    }

    /// <summary>
    /// 节点类型
    /// </summary>
    public enum WorkNodeTypeEnum
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        [Description("开始结点")]
        Start = 0,

        /// <summary>
        /// 结束节点
        /// </summary>
        [Description("结束结点")]
        End = 1,

        /// <summary>
        /// 普通节点
        /// </summary>
        [Description("普通结点")]
        Common = 2,

        /// <summary>
        /// 系统节点
        /// </summary>
        [Description("系统结点")]
        System = 3
    }

    /// <summary>
    /// 处理策略类型
    /// </summary>
    public enum HandleStrategyTypeEnum
    {
        /// <summary>
        /// 一人同意即可
        /// </summary>
        [Description("一人同意即可")]
        OneAgree = 0,

        /// <summary>
        /// 所有人必须同意
        /// </summary>
        [Description("所有人必须同意")]
        AllAgree = 1
    }

    /// <summary>
    /// 节点处理者类型
    /// </summary>
    public enum NodeHandlerTypeEnum
    {
        /// <summary>
        /// 所有人员
        /// </summary>
        [Description("所有人员")]
        All = 0,

        /// <summary>
        /// 部门
        /// </summary>
        [Description("部门")]
        Dept = 1,

        /// <summary>
        /// 职务
        /// </summary>
        [Description("职务")]
        Duty = 2,

        /// <summary>
        /// 岗位
        /// </summary>
        [Description("岗位")]
        Position = 3,

        /// <summary>
        /// 人员
        /// </summary>
        [Description("人员")]
        Employee = 4,

        /// <summary>
        /// 角色
        /// </summary>
        [Description("角色")]
        Role = 5,

        /// <summary>
        /// 发起者直接上级
        /// </summary>
        [Description("发起者直接上级")]
        StarterDirectLeader = 6,

        /// <summary>
        /// 发起者部门负责人，部门leader
        /// </summary>
        [Description("发起者部门负责人")]
        StarterChargeLeader = 7,

        /// <summary>
        /// 上一步处理者直接上级
        /// </summary>
        [Description("上一处理者直接上级")]
        LastHandlerDirectLeader = 8,

        /// <summary>
        /// 上一处理者部门负责人
        /// </summary>
        [Description("上一处理者部门负责人")]
        LastHandlerChargeLeader = 9,

        /// <summary>
        /// 表单字段值
        /// </summary>
        [Description("表单字段值")]
        FormFieldValue = 10,

        /// <summary>
        /// 发起者
        /// </summary>
        [Description("发起者")]
        Starter = 11,

        /// <summary>
        /// 上一步处理者
        /// </summary>
        [Description("上一处理者")]
        LastHandler = 12,

        /// <summary>
        /// 发起者上级部门负责人
        /// </summary>
        [Description("发起者上级部门负责人")]
        StarterParentDeptLeader = 13,

        /// <summary>
        /// 发起者上上级部门负责人
        /// </summary>
        [Description("发起者上上级部门负责人")]
        StarterParentParentDeptLeader = 14,

        /// <summary>
        /// 上一处理者上级部门负责人
        /// </summary>
        [Description("上一处理者上级部门负责人")]
        LastHandlerParentDeptLeader = 15,

        /// <summary>
        /// 上一处理者上上级部门负责人
        /// </summary>
        [Description("上一处理者上上级部门负责人")]
        LastHandlerParentParentDeptLeader = 16,

        /// <summary>
        /// 发起者第一层级部门负责人
        /// </summary>
        [Description("发起者第一层级部门负责人")]
        StarterLevel1DeptLeader = 17,

        /// <summary>
        /// 发起者第二层级部门负责人
        /// </summary>
        [Description("发起者第二层级部门负责人")]
        StarterLevel2DeptLeader = 18,

        /// <summary>
        /// 发起者第三层级部门负责人
        /// </summary>
        [Description("发起者第三层级部门负责人")]
        StarterLevel3DeptLeader = 19,

        /// <summary>
        /// 上一处理者第一层级部门负责人
        /// </summary>
        [Description("上一处理者第一层级部门负责人")]
        LastHandlerLevel1DeptLeader = 20,

        /// <summary>
        /// 上一处理者第二层级部门负责人
        /// </summary>
        [Description("上一处理者第二层级部门负责人")]
        LastHandlerLevel2DeptLeader = 21,

        /// <summary>
        /// 上一处理者第三层级部门负责人
        /// </summary>
        [Description("上一处理者第三层级部门负责人")]
        LastHandlerLevel3DeptLeader = 22
    }

    /// <summary>
    /// 节点回退类型
    /// </summary>
    public enum NodeBackTypeEnum
    {
        /// <summary>
        /// 回退到任意步骤
        /// </summary>
        [Description("回退到任意节点")]
        BackToAll = 0,

        /// <summary>
        /// 回退到发起者
        /// </summary>
        [Description("回退到发起者")]
        BackToFirst = 1,

        /// <summary>
        /// 回退到上一步
        /// </summary>
        [Description("回退到上一步")]
        BackToLast = 2
    }

    /// <summary>
    /// 流程按钮类型
    /// </summary>
    public enum FlowButtonTypeEnum
    {
        /// <summary>
        /// 同意按钮
        /// </summary>
        [Description("同意按钮")]
        AgreeBtn = 0,

        /// <summary>
        /// 拒绝按钮
        /// </summary>
        [Description("拒绝按钮")]
        RejectBtn = 1,

        /// <summary>
        /// 回退按钮
        /// </summary>
        [Description("回退按钮")]
        BackBtn = 2,

        /// <summary>
        /// 指派按钮
        /// </summary>
        [Description("指派按钮")]
        AssignBtn = 3,

        /// <summary>
        /// 自定义按钮
        /// </summary>
        [Description("自定义按钮")]
        CustomerBtn = 100
    }

    /// <summary>
    /// 子流程类型
    /// </summary>
    public enum SubFlowTypeEnum
    {
        /// <summary>
        /// 子流程
        /// </summary>
        [Description("子流程")]
        ChildFlow = 1,

        /// <summary>
        /// 分支流程
        /// </summary>
        [Description("分支流程")]
        BranchFlow = 2
    }
}

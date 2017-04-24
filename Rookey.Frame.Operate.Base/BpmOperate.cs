/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Base;
using Rookey.Frame.Base.User;
using Rookey.Frame.Common;
using Rookey.Frame.Common.Model;
using Rookey.Frame.Common.PubDefine;
using Rookey.Frame.Model.Bpm;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Model.OrgM;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base.EnumDef;
using Rookey.Frame.Operate.Base.OperateHandle;
using Rookey.Frame.Operate.Base.TempModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Rookey.Frame.Operate.Base
{
    /// <summary>
    /// 工作流操作类
    /// </summary>
    public static class BpmOperate
    {
        #region 流程分类

        /// <summary>
        /// 获取所流程分类
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<Bpm_FlowClass> GetAllFlowClass(Expression<Func<Bpm_FlowClass, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Bpm_FlowClass, bool>> exp = x => !x.IsDeleted && !x.IsDraft;
            if (expression != null) exp = ExpressionExtension.And<Bpm_FlowClass>(exp, expression);
            List<Bpm_FlowClass> flowClass = CommonOperate.GetEntities<Bpm_FlowClass>(out errMsg, exp, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (flowClass == null) flowClass = new List<Bpm_FlowClass>();
            return flowClass;
        }

        /// <summary>
        /// 根据ID获取流程分类
        /// </summary>
        /// <returns></returns>
        public static Bpm_FlowClass GetFlowClassById(Guid classId)
        {
            return GetAllFlowClass(x => x.Id == classId).FirstOrDefault();
        }

        /// <summary>
        /// 流程分类树，包含分类下的流程
        /// </summary
        /// <param name="classId">分类根结点ID，为空是加载整棵树</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static TreeNode LoadFlowClassTree(Guid? classId, UserInfo currUser)
        {
            TreeNode node = CommonOperate.GetTreeNode<Bpm_FlowClass>(classId, null, null, null, null, "eu-icon-folder", null, null, null, false, currUser);
            HandleFlowTreeNode(node);
            return node;
        }

        /// <summary>
        /// 处理流程分类节点，在分类叶子节点上添加流程信息节点
        /// </summary>
        /// <param name="node"></param>
        private static void HandleFlowTreeNode(TreeNode node)
        {
            if (node == null) return;
            List<Bpm_WorkFlow> workflows = GetWorkFlowOfFlowClass(node.id.ObjToGuid());
            if (workflows.Count > 0)
            {
                List<TreeNode> childs = workflows.Select(x => new TreeNode() { id = x.Id.ToString(), iconCls = "eu-icon-cog", text = string.IsNullOrEmpty(x.DisplayName) ? x.Name : x.DisplayName, attribute = new TreeAttributes() { obj = new { workflowId = x.Id } } }).ToList();
                List<TreeNode> list = new List<TreeNode>();
                if (node.children != null)
                {
                    list.AddRange(node.children.Cast<TreeNode>());
                }
                list.AddRange(childs);
                node.children = list;
            }
            else
            {
                if (node.children != null)
                {
                    foreach (TreeNode child in node.children)
                    {
                        HandleFlowTreeNode(child);
                    }
                }
            }
        }

        #endregion

        #region 流程信息

        /// <summary>
        /// 获取所有流程
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<Bpm_WorkFlow> GetAllWorkflows(Expression<Func<Bpm_WorkFlow, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Bpm_WorkFlow, bool>> exp = x => !x.IsDeleted && !x.IsDraft;
            if (expression != null) exp = ExpressionExtension.And<Bpm_WorkFlow>(exp, expression);
            List<Bpm_WorkFlow> workflows = CommonOperate.GetEntities<Bpm_WorkFlow>(out errMsg, exp, null, false);
            if (workflows == null) workflows = new List<Bpm_WorkFlow>();
            return workflows;
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <param name="id">流程ID</param>
        /// <returns></returns>
        public static Bpm_WorkFlow GetWorkflow(Guid id)
        {
            Bpm_WorkFlow workflow = GetAllWorkflows(x => x.Id == id).FirstOrDefault();
            if (workflow != null && workflow.Sys_ModuleId.HasValue)
                workflow.Sys_ModuleName = SystemOperate.GetModuleNameById(workflow.Sys_ModuleId.Value);
            return workflow;
        }

        /// <summary>
        /// 获取模块流程
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <returns></returns>
        public static Bpm_WorkFlow GetModuleWorkFlow(Guid moduleId)
        {
            List<Bpm_WorkFlow> workflows = GetAllWorkflows(x => x.Sys_ModuleId == moduleId);
            if (workflows.Count == 0) return null;
            //找所有时间上满足的流程
            workflows = workflows.Where(x => (x.ValidStartTime == null && x.ValidEndTime == null) || (x.ValidStartTime != null && x.ValidEndTime == null && x.ValidStartTime.Value < DateTime.Now) || (x.ValidEndTime != null && x.ValidStartTime == null && x.ValidEndTime.Value > DateTime.Now) || (x.ValidStartTime != null && x.ValidEndTime != null && x.ValidStartTime.Value < DateTime.Now && x.ValidEndTime.Value > DateTime.Now)).ToList();
            if (workflows.Count == 1) //只有一个版本
            {
                Bpm_WorkFlow firstWorkflow = workflows.FirstOrDefault();
                if (firstWorkflow != null && firstWorkflow.Sys_ModuleId.HasValue)
                    firstWorkflow.Sys_ModuleName = SystemOperate.GetModuleDiplay(firstWorkflow.Sys_ModuleId.Value);
                return firstWorkflow;
            }
            //找满足当前时间区间的流程
            Bpm_WorkFlow workflow = workflows.Where(x => x.ValidStartTime != null && x.ValidEndTime != null).FirstOrDefault();
            if (workflow == null) //找未设置结束时间的流程
                workflow = workflows.Where(x => x.ValidStartTime != null && x.ValidEndTime == null).FirstOrDefault();
            if (workflow == null) //找默认流程
                workflow = workflows.Where(x => x.ValidStartTime == null && x.ValidEndTime == null).FirstOrDefault();
            if (workflow != null && workflow.Sys_ModuleId.HasValue)
                workflow.Sys_ModuleName = SystemOperate.GetModuleDiplay(workflow.Sys_ModuleId.Value);
            return workflow;
        }

        /// <summary>
        /// 获取流程分类下的流程
        /// </summary>
        /// <param name="classId">分类ID</param>
        /// <returns></returns>
        public static List<Bpm_WorkFlow> GetWorkFlowOfFlowClass(Guid classId)
        {
            return GetAllWorkflows(x => x.Bpm_FlowClassId == classId);
        }

        /// <summary>
        /// 获取流程的关联模块ID
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <returns></returns>
        public static Guid GetWorkflowModuleId(Guid workflowId)
        {
            Bpm_WorkFlow workflow = GetWorkflow(workflowId);
            if (workflow != null && workflow.Sys_ModuleId.HasValue && workflow.Sys_ModuleId.Value != Guid.Empty)
                return workflow.Sys_ModuleId.Value;
            return Guid.Empty;
        }

        /// <summary>
        /// 判断是不是子流程
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <returns></returns>
        public static bool IsSubWorkFlow(Guid workflowId)
        {
            return GetAllWorkNodes(x => x.Bpm_WorkFlowSubId == workflowId).Count > 0;
        }

        /// <summary>
        /// 获取记录的流程实例
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public static Bpm_WorkFlowInstance GetWorkflowInstance(Guid moduleId, Guid recordId)
        {
            List<Guid?> workflowIds = BpmOperate.GetAllWorkflows(x => x.Sys_ModuleId == moduleId).Select(x => (Guid?)x.Id).ToList();
            string errMsg = string.Empty;
            Bpm_WorkFlowInstance workflowInst = workflowIds.Count > 1 ? CommonOperate.GetEntity<Bpm_WorkFlowInstance>(x => workflowIds.Contains(x.Bpm_WorkFlowId) && x.RecordId == recordId, null, out errMsg) :
                                               CommonOperate.GetEntity<Bpm_WorkFlowInstance>(x => x.Bpm_WorkFlowId == workflowIds.FirstOrDefault() && x.RecordId == recordId, null, out errMsg);
            return workflowInst;
        }

        /// <summary>
        /// 获取流程实例
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public static Bpm_WorkFlowInstance GetWorkflowInstanceById(Guid id)
        {
            string errMsg = string.Empty;
            Bpm_WorkFlowInstance workflowInst = CommonOperate.GetEntityById<Bpm_WorkFlowInstance>(id, out errMsg);
            return workflowInst;
        }

        #endregion

        #region 流程结点

        /// <summary>
        /// 获取所有流程结点
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<Bpm_WorkNode> GetAllWorkNodes(Expression<Func<Bpm_WorkNode, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Bpm_WorkNode, bool>> exp = x => !x.IsDeleted && !x.IsDraft;
            if (expression != null) exp = ExpressionExtension.And<Bpm_WorkNode>(exp, expression);
            List<Bpm_WorkNode> workNodes = CommonOperate.GetEntities<Bpm_WorkNode>(out errMsg, exp, null, false);
            if (workNodes == null) workNodes = new List<Bpm_WorkNode>();
            return workNodes;
        }

        /// <summary>
        /// 获取流程结点
        /// </summary>
        /// <param name="workNodeId">流程结点ID</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetWorkNode(Guid workNodeId)
        {
            return GetAllWorkNodes(x => x.Id == workNodeId).FirstOrDefault();
        }

        /// <summary>
        /// 获取流程的所有流程结点
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <returns></returns>
        public static List<Bpm_WorkNode> GetWorkNodesOfFlow(Guid workflowId)
        {
            return GetAllWorkNodes(x => x.Bpm_WorkFlowId == workflowId);
        }

        /// <summary>
        /// 获取发起节点
        /// </summary>
        /// <param name="workflowId">流程id</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetLaunchNode(Guid workflowId)
        {
            string errMsg = string.Empty;
            int workNodeType = (int)WorkNodeTypeEnum.Start;
            Bpm_WorkNode startNode = GetAllWorkNodes(x => x.Bpm_WorkFlowId == workflowId && x.WorkNodeType == workNodeType).FirstOrDefault();
            if (startNode == null) return null;
            Bpm_WorkLine startLine = GetAllWorkLines(x => x.Bpm_WorkNodeStartId == startNode.Id).FirstOrDefault();
            if (startLine == null || !startLine.Bpm_WorkNodeEndId.HasValue) return null;
            Bpm_WorkNode launchNode = GetWorkNode(startLine.Bpm_WorkNodeEndId.Value);
            return launchNode;
        }

        /// <summary>
        /// 获取结束结点
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetEndNode(Guid workflowId)
        {
            string errMsg = string.Empty;
            int workNodeType = (int)WorkNodeTypeEnum.End;
            Bpm_WorkNode endNode = GetAllWorkNodes(x => x.Bpm_WorkFlowId == workflowId && x.WorkNodeType == workNodeType).FirstOrDefault();
            return endNode;
        }

        /// <summary>
        /// 根据模块ID获取发起结点
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetLaunchNodeByModuleId(Guid moduleId)
        {
            Bpm_WorkFlow workflow = GetModuleWorkFlow(moduleId);
            if (workflow != null)
            {
                return GetLaunchNode(workflow.Id);
            }
            return null;
        }

        /// <summary>
        /// 获取当前结点的前一结点
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="workNodeId">当前结点ID</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetPrexNode(Guid workflowId, Guid workNodeId)
        {
            Bpm_WorkLine workLine = GetAllWorkLines(x => x.Bpm_WorkFlowId == workflowId && x.Bpm_WorkNodeEndId == workNodeId).FirstOrDefault();
            if (workLine != null && workLine.Bpm_WorkNodeStartId.HasValue && workLine.Bpm_WorkNodeStartId.Value != Guid.Empty)
                return GetWorkNode(workLine.Bpm_WorkNodeStartId.Value);
            return null;
        }

        /// <summary>
        /// 获取当前结点的所有前面结点集合
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="workNodeId">当前结点ID</param>
        /// <returns></returns>
        public static List<Bpm_WorkNode> GetPrexNodes(Guid workflowId, Guid workNodeId)
        {
            List<Bpm_WorkNode> preNodes = new List<Bpm_WorkNode>();
            Bpm_WorkNode preNode = GetPrexNode(workflowId, workNodeId);
            if (preNode != null && preNode.WorkNodeTypeOfEnum == WorkNodeTypeEnum.Common)
            {
                preNodes.Add(preNode);
                preNodes.AddRange(GetPrexNodes(workflowId, preNode.Id));
            }
            return preNodes;
        }

        /// <summary>
        /// 获取当前节点的下一节点集合
        /// </summary>
        /// <param name="workflowId">流程id</param>
        /// <param name="currNodeId">起始节点id</param>
        /// <returns></returns>
        public static List<Bpm_WorkNode> GetNextWorkNodes(Guid workflowId, Guid currNodeId)
        {
            string errMsg = string.Empty;
            List<Bpm_WorkLine> workLines = GetAllWorkLines(x => x.Bpm_WorkFlowId == workflowId && x.Bpm_WorkNodeStartId == currNodeId);
            if (workLines != null && workLines.Count > 0)
            {
                List<Guid> workNodeIds = workLines.Where(x => x.Bpm_WorkNodeEndId.HasValue).Select(x => x.Bpm_WorkNodeEndId.Value).ToList();
                List<Bpm_WorkNode> workNodes = GetAllWorkNodes(x => workNodeIds.Contains(x.Id));
                if (workNodes == null) workNodes = new List<Bpm_WorkNode>();
                workNodes = workNodes.Where(x => x.WorkNodeType != (int)WorkNodeTypeEnum.End).ToList();
                return workNodes;
            }
            return new List<Bpm_WorkNode>();
        }

        /// <summary>
        /// 获取下一审批节点
        /// </summary>
        /// <param name="nextNodes">下一结点集合</param>
        /// <param name="workflow">流程</param>
        /// <param name="currNode">当前结点</param>
        /// <param name="workFlowInst">流程实例</param>
        /// <param name="currNodeInst">当前实例结点</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetNextActionNode(List<Bpm_WorkNode> nextNodes, Bpm_WorkFlow workflow, Bpm_WorkNode currNode, Bpm_WorkFlowInstance workFlowInst, Bpm_WorkNodeInstance currNodeInst)
        {
            string errMsg = string.Empty;
            Guid moduleId = workflow.Sys_ModuleId.Value;
            foreach (Bpm_WorkNode nextNode in nextNodes)
            {
                Bpm_WorkLine workLine = GetAllWorkLines(x => x.Bpm_WorkFlowId == workflow.Id && x.Bpm_WorkNodeStartId == currNode.Id && x.Bpm_WorkNodeEndId == nextNode.Id && !x.IsDeleted).FirstOrDefault();
                if (workLine == null) continue;
                if (!workLine.IsCustomerCondition)
                {
                    bool formConn = true;
                    bool dutyConn = true;
                    bool deptConn = true;
                    bool sqlConn = true;
                    if (!string.IsNullOrEmpty(workLine.FormCondition))
                    {
                        try
                        {
                            List<ConditionItem> items = JsonHelper.Deserialize<List<ConditionItem>>(workLine.FormCondition);
                            object queryLamda = CommonOperate.GetQueryCondition(moduleId, items);
                            object model = CommonOperate.GetEntity(moduleId, queryLamda, string.Format("Id='{0}'", workFlowInst.RecordId), out errMsg);
                            formConn = model != null;
                        }
                        catch { }
                    }
                    if (!string.IsNullOrEmpty(workLine.DutyCondition))
                    {
                    }
                    if (!string.IsNullOrEmpty(workLine.DeptCondition))
                    {
                    }
                    if (!string.IsNullOrEmpty(workLine.SqlCondition))
                    {
                    }
                    if (formConn && dutyConn && deptConn && sqlConn)
                    {
                        return nextNode;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 根据tagId获取流程结点
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="tagId">tagid</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetWorkNodeByTagId(Guid workflowId, string tagId)
        {
            return GetAllWorkNodes(x => x.Bpm_WorkFlowId == workflowId && x.TagId == tagId).FirstOrDefault();
        }

        /// <summary>
        /// 获取下一结点的处理者
        /// </summary>
        /// <param name="nextNode">下一结点</param>
        /// <param name="currUser">当前结点处理人</param>
        /// <param name="workFlowInst">流程实例</param>
        /// <returns>返回员工ID集合</returns>
        public static List<Guid> GetNextNodeHandler(Bpm_WorkNode nextNode, UserInfo currUser, Bpm_WorkFlowInstance workFlowInst)
        {
            if (nextNode == null || currUser == null || workFlowInst == null)
                return new List<Guid>();
            if (!nextNode.Bpm_WorkFlowId.HasValue || nextNode.WorkNodeTypeOfEnum == WorkNodeTypeEnum.End)
                return new List<Guid>();
            List<Guid> empIds = new List<Guid>();
            Guid moduleId = GetWorkflowModuleId(nextNode.Bpm_WorkFlowId.Value);
            //调用自定义查找结点处理人方法
            object rsObj = CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, "GetNodeHandler", new object[] { workFlowInst.RecordId, nextNode.Name, workFlowInst.Id, currUser });
            if (rsObj != null)
            {
                try
                {
                    empIds = rsObj as List<Guid>;
                }
                catch { }
            }
            if (empIds.Count > 0) return empIds;
            //调用通用查找结点处理人
            List<Guid> filter = new List<Guid>(); //过滤范围
            if (!string.IsNullOrEmpty(nextNode.HandleRange))
            {
                string[] token = nextNode.HandleRange.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (token.Length > 0)
                    filter = token.Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
            }
            #region 找处理人
            switch (nextNode.HandlerTypeOfEnum) //处理者类型
            {
                case NodeHandlerTypeEnum.All: //所有人员
                    {
                        empIds = OrgMOperate.GetAllEmps(x => x.Id != currUser.EmpId.Value).Select(x => x.Id).ToList();
                    }
                    break;
                case NodeHandlerTypeEnum.Dept: //部门
                    foreach (Guid deptId in filter)
                    {
                        empIds.AddRange(OrgMOperate.GetDeptEmps(deptId, true).Select(x => x.Id));
                    }
                    break;
                case NodeHandlerTypeEnum.Duty: //职务
                    foreach (Guid dutyId in filter)
                    {
                        empIds.AddRange(OrgMOperate.GetDutyEmps(dutyId).Select(x => x.Id));
                    }
                    break;
                case NodeHandlerTypeEnum.Employee: //人员
                    empIds = filter;
                    break;
                case NodeHandlerTypeEnum.Position: //岗位
                    foreach (Guid positionId in filter)
                    {
                        empIds.AddRange(OrgMOperate.GetPositionEmps(positionId).Select(x => x.Id));
                    }
                    break;
                case NodeHandlerTypeEnum.Role: //角色
                    foreach (Guid roleId in filter)
                    {
                        List<string> usernames = PermissionOperate.GetRoleUsers(roleId).Select(x => x.UserName).ToList();
                        empIds.AddRange(OrgMOperate.GetEmpsByUserNames(usernames).Select(x => x.Id));
                    }
                    break;
                case NodeHandlerTypeEnum.FormFieldValue: //表单字段值
                    {
                        Guid tempEmpId = nextNode.FormFieldName.ObjToGuid();
                        if (tempEmpId != Guid.Empty)
                            empIds.Add(tempEmpId);
                    }
                    break;
                case NodeHandlerTypeEnum.LastHandlerDirectLeader: //上一步处理者直接上级
                    {
                        if (currUser.EmpId.HasValue && currUser.EmpId.Value != Guid.Empty)
                        {
                            var tempEmp = OrgMOperate.GetEmpParentEmp(currUser.EmpId.Value);
                            if (tempEmp != null) empIds.Add(tempEmp.Id);
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.LastHandlerChargeLeader: //上一步处理者分管领导
                    {
                        if (currUser.EmpId.HasValue && currUser.EmpId.Value != Guid.Empty)
                        {
                            var tempEmp = OrgMOperate.GetEmpChargeLeader(currUser.EmpId.Value);
                            if (tempEmp != null) empIds.Add(tempEmp.Id);
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.StarterDirectLeader: //发起者直接上级
                    {
                        if (workFlowInst.OrgM_EmpId.HasValue && workFlowInst.OrgM_EmpId.Value != Guid.Empty)
                        {
                            var tempEmp = OrgMOperate.GetEmpParentEmp(workFlowInst.OrgM_EmpId.Value);
                            if (tempEmp != null) empIds.Add(tempEmp.Id);
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.StarterChargeLeader: //发起者部门负责人
                    {
                        if (workFlowInst.OrgM_EmpId.HasValue && workFlowInst.OrgM_EmpId.Value != Guid.Empty)
                        {
                            var tempEmp = OrgMOperate.GetEmpChargeLeader(workFlowInst.OrgM_EmpId.Value);
                            if (tempEmp != null) empIds.Add(tempEmp.Id);
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.Starter: //发起者
                    {
                        if (workFlowInst.OrgM_EmpId.HasValue && workFlowInst.OrgM_EmpId.Value != Guid.Empty)
                            empIds.Add(workFlowInst.OrgM_EmpId.Value);
                    }
                    break;
                case NodeHandlerTypeEnum.LastHandler: //上一处理人
                    {
                        if (currUser.EmpId.HasValue && currUser.EmpId.Value != Guid.Empty)
                            empIds.Add(currUser.EmpId.Value);
                    }
                    break;
                case NodeHandlerTypeEnum.StarterParentDeptLeader: //发起者上级部门负责人
                    {
                        if (workFlowInst.OrgM_EmpId.HasValue && workFlowInst.OrgM_EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(workFlowInst.OrgM_EmpId.Value);
                            if (dept != null && dept.ParentId.HasValue && dept.ParentId.Value != Guid.Empty)
                            {
                                OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(dept.ParentId.Value);
                                if (tempEmp != null) empIds.Add(tempEmp.Id);
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.StarterParentParentDeptLeader: //发起者上上级部门负责人
                    {
                        if (workFlowInst.OrgM_EmpId.HasValue && workFlowInst.OrgM_EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(workFlowInst.OrgM_EmpId.Value);
                            if (dept != null && dept.ParentId.HasValue && dept.ParentId.Value != Guid.Empty)
                            {
                                OrgM_Dept parentDept = OrgMOperate.GetDeptById(dept.ParentId.Value);
                                if (parentDept != null && parentDept.ParentId.HasValue && parentDept.ParentId.Value != Guid.Empty)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(parentDept.ParentId.Value);
                                    if (tempEmp != null) empIds.Add(tempEmp.Id);
                                }
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.LastHandlerParentDeptLeader: //上一处理者上级部门负责人
                    {
                        if (currUser.EmpId.HasValue && currUser.EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(currUser.EmpId.Value);
                            if (dept != null && dept.ParentId.HasValue && dept.ParentId.Value != Guid.Empty)
                            {
                                OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(dept.ParentId.Value);
                                if (tempEmp != null) empIds.Add(tempEmp.Id);
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.LastHandlerParentParentDeptLeader: //上一处理者上上级部门负责人
                    {
                        if (currUser.EmpId.HasValue && currUser.EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(currUser.EmpId.Value);
                            if (dept != null && dept.ParentId.HasValue && dept.ParentId.Value != Guid.Empty)
                            {
                                OrgM_Dept parentDept = OrgMOperate.GetDeptById(dept.ParentId.Value);
                                if (parentDept != null && parentDept.ParentId.HasValue && parentDept.ParentId.Value != Guid.Empty)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(parentDept.ParentId.Value);
                                    if (tempEmp != null) empIds.Add(tempEmp.Id);
                                }
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.StarterLevel1DeptLeader: //发起者第一层级部门负责人
                    {
                        if (workFlowInst.OrgM_EmpId.HasValue && workFlowInst.OrgM_EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(workFlowInst.OrgM_EmpId.Value);
                            if (dept != null)
                            {
                                OrgM_Dept levelDept = OrgMOperate.GetLevelDepthDepts(1, dept.Id).FirstOrDefault();
                                if (levelDept != null)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(levelDept.Id);
                                    if (tempEmp != null) empIds.Add(tempEmp.Id);
                                }
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.StarterLevel2DeptLeader: //发起者第二层级部门负责人
                    {
                        if (workFlowInst.OrgM_EmpId.HasValue && workFlowInst.OrgM_EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(workFlowInst.OrgM_EmpId.Value);
                            if (dept != null)
                            {
                                OrgM_Dept levelDept = OrgMOperate.GetLevelDepthDepts(2, dept.Id).FirstOrDefault();
                                if (levelDept != null)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(levelDept.Id);
                                    if (tempEmp != null) empIds.Add(tempEmp.Id);
                                }
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.StarterLevel3DeptLeader: //发起者第三层级部门负责人
                    {
                        if (workFlowInst.OrgM_EmpId.HasValue && workFlowInst.OrgM_EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(workFlowInst.OrgM_EmpId.Value);
                            if (dept != null)
                            {
                                OrgM_Dept levelDept = OrgMOperate.GetLevelDepthDepts(3, dept.Id).FirstOrDefault();
                                if (levelDept != null)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(levelDept.Id);
                                    if (tempEmp != null) empIds.Add(tempEmp.Id);
                                }
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.LastHandlerLevel1DeptLeader: //上一处理者第一层级部门负责人
                    {
                        if (currUser.EmpId.HasValue && currUser.EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(currUser.EmpId.Value);
                            if (dept != null)
                            {
                                OrgM_Dept levelDept = OrgMOperate.GetLevelDepthDepts(1, dept.Id).FirstOrDefault();
                                if (levelDept != null)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(levelDept.Id);
                                    if (tempEmp != null) empIds.Add(tempEmp.Id);
                                }
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.LastHandlerLevel2DeptLeader: //上一处理者第二层级部门负责人
                    {
                        if (currUser.EmpId.HasValue && currUser.EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(currUser.EmpId.Value);
                            if (dept != null)
                            {
                                OrgM_Dept levelDept = OrgMOperate.GetLevelDepthDepts(2, dept.Id).FirstOrDefault();
                                if (levelDept != null)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(levelDept.Id);
                                    if (tempEmp != null) empIds.Add(tempEmp.Id);
                                }
                            }
                        }
                    }
                    break;
                case NodeHandlerTypeEnum.LastHandlerLevel3DeptLeader: //上一处理者第三层级部门负责人
                    {
                        if (currUser.EmpId.HasValue && currUser.EmpId.Value != Guid.Empty)
                        {
                            OrgM_Dept dept = OrgMOperate.GetEmpMainDept(currUser.EmpId.Value);
                            if (dept != null)
                            {
                                OrgM_Dept levelDept = OrgMOperate.GetLevelDepthDepts(3, dept.Id).FirstOrDefault();
                                if (levelDept != null)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetDeptLeader(levelDept.Id);
                                    if (tempEmp != null) empIds.Add(tempEmp.Id);
                                }
                            }
                        }
                    }
                    break;
            }
            #endregion
            return empIds;
        }

        /// <summary>
        /// 获取前一结点的处理人集合
        /// </summary>
        /// <param name="currNode">当前结点</param>
        /// <param name="workflowInst">流程实例</param>
        /// <returns></returns>
        public static List<Guid> GetPreNodeHandler(Bpm_WorkNode currNode, Bpm_WorkFlowInstance workflowInst)
        {
            if (currNode == null || !currNode.Bpm_WorkFlowId.HasValue || workflowInst == null)
                return new List<Guid>();
            List<Guid> empIds = new List<Guid>();
            List<Bpm_WorkNode> preNodes = GetPrexNodes(currNode.Bpm_WorkFlowId.Value, currNode.Id);
            string errMsg = string.Empty;
            foreach (Bpm_WorkNode node in preNodes)
            {
                Bpm_WorkNodeInstance workNodeInst = CommonOperate.GetEntity<Bpm_WorkNodeInstance>(x => x.Bpm_WorkFlowInstanceId == workflowInst.Id && x.Bpm_WorkNodeId == currNode.Id, null, out errMsg);
                if (workNodeInst == null) continue;
                int wa = (int)WorkActionEnum.NoAction;
                List<Bpm_WorkToDoList> todos = CommonOperate.GetEntities<Bpm_WorkToDoList>(out errMsg, x => x.Bpm_WorkFlowInstanceId == workflowInst.Id && x.Bpm_WorkNodeInstanceId == workNodeInst.Id && x.WorkAction != wa, null, false);
                empIds.AddRange(todos.Where(x => x.OrgM_EmpId.HasValue).Select(x => x.OrgM_EmpId.Value).ToList());
            }
            return empIds;
        }

        /// <summary>
        /// 获取发起结点处理者
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public static List<Guid> GetLaunchNodeHandler(Guid moduleId)
        {
            List<Guid> empIds = new List<Guid>();
            Bpm_WorkNode launchNode = GetLaunchNodeByModuleId(moduleId);
            if (launchNode == null) return empIds;
            List<Guid> filter = new List<Guid>(); //过滤范围
            if (!string.IsNullOrEmpty(launchNode.HandleRange))
            {
                string[] token = launchNode.HandleRange.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (token.Length > 0)
                    filter = token.Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
            }
            switch (launchNode.HandlerTypeOfEnum) //处理者类型
            {
                case NodeHandlerTypeEnum.All:
                    {
                        empIds = OrgMOperate.GetAllEmps().Select(x => x.Id).ToList();
                        if (filter.Count > 0) empIds = empIds.Where(x => filter.Contains(x)).ToList();
                    }
                    break;
                case NodeHandlerTypeEnum.Dept:
                    foreach (Guid deptId in filter)
                    {
                        empIds.AddRange(OrgMOperate.GetDeptEmps(deptId, true).Select(x => x.Id));
                    }
                    break;
                case NodeHandlerTypeEnum.Duty:
                    foreach (Guid dutyId in filter)
                    {
                        empIds.AddRange(OrgMOperate.GetDutyEmps(dutyId).Select(x => x.Id));
                    }
                    break;
                case NodeHandlerTypeEnum.Employee:
                    empIds = filter;
                    break;
                case NodeHandlerTypeEnum.Position:
                    foreach (Guid positionId in filter)
                    {
                        empIds.AddRange(OrgMOperate.GetPositionEmps(positionId).Select(x => x.Id));
                    }
                    break;
                case NodeHandlerTypeEnum.Role:
                    foreach (Guid roleId in filter)
                    {
                        List<string> usernames = PermissionOperate.GetRoleUsers(roleId).Select(x => x.UserName.ObjToStr().ToLower()).ToList();
                        empIds.AddRange(OrgMOperate.GetEmpsByUserNames(usernames).Select(x => x.Id));
                    }
                    break;
            }
            return empIds;
        }

        /// <summary>
        /// 获取子流程的父流程当前结点
        /// </summary>
        /// <param name="subWorkflowId">子流程结点</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetParentFlowNode(Guid subWorkflowId)
        {
            return GetAllWorkNodes(x => x.Bpm_WorkFlowSubId == subWorkflowId).FirstOrDefault();
        }

        /// <summary>
        /// 获取回退结点集合
        /// </summary>
        /// <param name="toDoTaskId">待办ID</param>
        /// <returns></returns>
        public static List<Bpm_WorkNode> GetBackNodes(Guid toDoTaskId)
        {
            Guid workNodeId = BpmOperate.GetWorkNodeIdByTodoId(toDoTaskId);
            Bpm_WorkNode workNode = BpmOperate.GetWorkNode(workNodeId);
            List<Bpm_WorkNode> bakcNodes = new List<Bpm_WorkNode>();
            if (workNode != null && workNode.Bpm_WorkFlowId.HasValue && workNode.Bpm_WorkFlowId.Value != Guid.Empty)
            {
                if (workNode.BackTypeOfEnum == NodeBackTypeEnum.BackToLast) //只能回退到上一结点
                {
                    string errMsg = string.Empty;
                    List<Guid> preNodeIds = GetPrexNodes(workNode.Bpm_WorkFlowId.Value, workNodeId).Select(x => x.Id).ToList();
                    string preNodeIdStr = string.Join("','", preNodeIds);
                    string whereSql = string.Format("Bpm_WorkNodeId IN('{0}') AND Id IN(SELECT Bpm_WorkNodeInstanceId FROM Bpm_WorkToDoList WHERE WorkAction IN(2,3) AND Bpm_WorkFlowInstanceId IN(SELECT Bpm_WorkFlowInstanceId FROM dbo.Bpm_WorkToDoList WHERE Id='{1}')) ORDER BY AutoIncrmId DESC", preNodeIdStr, toDoTaskId.ToString());
                    Bpm_WorkNodeInstance nodeInst = CommonOperate.GetEntity<Bpm_WorkNodeInstance>(null, whereSql, out errMsg, new List<string>() { "Bpm_WorkNodeId" });
                    if (nodeInst != null && nodeInst.Bpm_WorkNodeId.HasValue)
                        bakcNodes = GetAllWorkNodes(x => x.Id == nodeInst.Bpm_WorkNodeId.Value);
                }
                else if (workNode.BackTypeOfEnum == NodeBackTypeEnum.BackToFirst) //只能回退到发起结点
                {
                    Bpm_WorkNode launchNode = BpmOperate.GetLaunchNode(workNode.Bpm_WorkFlowId.Value);
                    if (launchNode != null)
                        bakcNodes = new List<Bpm_WorkNode>() { launchNode };
                }
                else if (workNode.BackTypeOfEnum == NodeBackTypeEnum.BackToAll) //允许回退到任意结点，并且要排除并行节点
                {
                    string errMsg = string.Empty;
                    List<Guid> preNodeIds = GetPrexNodes(workNode.Bpm_WorkFlowId.Value, workNodeId).Select(x => x.Id).ToList();
                    string preNodeIdStr = string.Join("','", preNodeIds);
                    string whereSql = string.Format("Bpm_WorkNodeId IN('{0}') AND Id IN(SELECT Bpm_WorkNodeInstanceId FROM Bpm_WorkToDoList WHERE WorkAction IN(2,3) AND Bpm_WorkFlowInstanceId IN(SELECT Bpm_WorkFlowInstanceId FROM dbo.Bpm_WorkToDoList WHERE Id='{1}')) ORDER BY AutoIncrmId DESC", preNodeIdStr, toDoTaskId.ToString());
                    List<Bpm_WorkNodeInstance> nodeInsts = CommonOperate.GetEntities<Bpm_WorkNodeInstance>(out errMsg, null, whereSql, false, null, null, null, new List<string>() { "Bpm_WorkNodeId" });
                    if (nodeInsts != null && nodeInsts.Count > 0)
                    {
                        List<Guid> nodeIds = nodeInsts.Where(x => x.Bpm_WorkNodeId.HasValue).Select(x => x.Bpm_WorkNodeId.Value).Distinct().ToList();
                        foreach (Guid nodeId in nodeIds)
                        {
                            bakcNodes.Add(GetWorkNode(nodeId));
                        }
                    }
                }
            }
            return bakcNodes;
        }

        /// <summary>
        /// 获取流程结点实例
        /// </summary>
        /// <param name="id">结点ID</param>
        /// <returns></returns>
        public static Bpm_WorkNodeInstance GetWorkNodeInstanceById(Guid id)
        {
            string errMsg = string.Empty;
            Bpm_WorkNodeInstance nodeInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstance>(id, out errMsg);
            return nodeInst;
        }

        #endregion

        #region 流程连线

        /// <summary>
        /// 获取所有流程连线
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<Bpm_WorkLine> GetAllWorkLines(Expression<Func<Bpm_WorkLine, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Bpm_WorkLine, bool>> exp = x => !x.IsDeleted && !x.IsDraft;
            if (expression != null) exp = ExpressionExtension.And<Bpm_WorkLine>(exp, expression);
            List<Bpm_WorkLine> workLines = CommonOperate.GetEntities<Bpm_WorkLine>(out errMsg, exp, null, false);
            if (workLines == null) workLines = new List<Bpm_WorkLine>();
            return workLines;
        }

        /// <summary>
        /// 获取流程中所有流程连线
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <returns></returns>
        public static List<Bpm_WorkLine> GetWorkLinesOfFlow(Guid workflowId)
        {
            return GetAllWorkLines(x => x.Bpm_WorkFlowId == workflowId);
        }

        /// <summary>
        /// 根据tagId获取流程连线
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="tagId">tagid</param>
        /// <returns></returns>
        public static Bpm_WorkLine GetWorkLineByTagId(Guid workflowId, string tagId)
        {
            return GetAllWorkLines(x => x.Bpm_WorkFlowId == workflowId && x.TagId == tagId).FirstOrDefault();
        }

        #endregion

        #region 流程按钮

        /// <summary>
        /// 获取所有流程按钮
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<Bpm_FlowBtn> GetAllWorkButtons(Expression<Func<Bpm_FlowBtn, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Bpm_FlowBtn, bool>> exp = x => !x.IsDeleted && !x.IsDraft;
            if (expression != null) exp = ExpressionExtension.And<Bpm_FlowBtn>(exp, expression);
            List<Bpm_FlowBtn> workBtns = CommonOperate.GetEntities<Bpm_FlowBtn>(out errMsg, exp, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (workBtns == null) workBtns = new List<Bpm_FlowBtn>();
            return workBtns;
        }

        /// <summary>
        /// 获取所有审批按钮配置
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<Bpm_NodeBtnConfig> GetAllApprovalBtnConfigs(Expression<Func<Bpm_NodeBtnConfig, bool>> expression = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Bpm_NodeBtnConfig, bool>> exp = x => !x.IsDeleted && !x.IsDraft;
            if (expression != null) exp = ExpressionExtension.And<Bpm_NodeBtnConfig>(exp, expression);
            List<Bpm_NodeBtnConfig> btnConfigs = CommonOperate.GetEntities<Bpm_NodeBtnConfig>(out errMsg, exp, null, false);
            if (btnConfigs == null) return new List<Bpm_NodeBtnConfig>();
            Dictionary<int, Bpm_NodeBtnConfig> dic = new Dictionary<int, Bpm_NodeBtnConfig>();
            foreach (Bpm_NodeBtnConfig btnConfig in btnConfigs)
            {
                if (btnConfig.Bpm_FlowBtnId.HasValue && btnConfig.Bpm_FlowBtnId.Value != Guid.Empty)
                {
                    Bpm_FlowBtn btn = GetAllWorkButtons(x => x.Id == btnConfig.Bpm_FlowBtnId.Value).FirstOrDefault();
                    if (btn != null)
                    {
                        int sort = btn.Sort;
                        if (dic.ContainsKey(sort))
                            sort = dic.Keys.Max() + 1;
                        dic.Add(sort, btnConfig);
                    }
                }
            }
            return dic.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }

        /// <summary>
        /// 获取流程表单按钮集合
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="workNodeId">流程结点ID</param>
        /// <returns></returns>
        public static List<FormButton> GetWorkNodeFormBtns(Guid workflowId, Guid workNodeId)
        {
            List<FormButton> btns = new List<FormButton>();
            Bpm_WorkNode workNode = GetWorkNode(workNodeId);
            if (workNode == null) return new List<FormButton>();
            List<Bpm_NodeBtnConfig> nodeConfigBtns = GetAllApprovalBtnConfigs(x => x.Bpm_WorkFlowId == workflowId && x.Bpm_WorkNodeId == workNodeId && x.IsEnabled && !x.IsDeleted);
            if (nodeConfigBtns.Count > 0)
            {
                int sort = 5;
                foreach (Bpm_NodeBtnConfig btnConfig in nodeConfigBtns)
                {
                    Bpm_FlowBtn flowBtn = GetAllWorkButtons(x => x.Id == btnConfig.Bpm_FlowBtnId).FirstOrDefault();
                    if (flowBtn == null) continue;
                    string clickMethod = flowBtn.ClickMethod;
                    ButtonIconType iconType = ButtonIconType.FlowAgree;
                    if (string.IsNullOrEmpty(clickMethod))
                    {
                        if (flowBtn.ButtonTypeOfEnum == FlowButtonTypeEnum.BackBtn)
                        {
                            clickMethod = "BackFlow(this)";
                            iconType = ButtonIconType.FlowReturn;
                        }
                        else if (flowBtn.ButtonTypeOfEnum == FlowButtonTypeEnum.AssignBtn)
                        {
                            clickMethod = "DirectFlow(this)";
                            iconType = ButtonIconType.FlowDirect;
                        }
                        else if (flowBtn.ButtonTypeOfEnum == FlowButtonTypeEnum.AgreeBtn)
                        {
                            clickMethod = "ApprovalFlow(this)";
                            iconType = ButtonIconType.FlowAgree;
                        }
                        else if (flowBtn.ButtonTypeOfEnum == FlowButtonTypeEnum.RejectBtn)
                        {
                            clickMethod = "ApprovalFlow(this)";
                            iconType = ButtonIconType.FlowReject;
                        }
                    }
                    btns.Add(new FormButton() { TagId = string.Format("flowBtn_{0}", flowBtn.Id), IconType = iconType, Icon = flowBtn.ButtonIcon, DisplayText = btnConfig.BtnDisplay, ClickMethod = clickMethod, Sort = sort });
                    sort++;
                }
            }
            else
            {
                Bpm_FlowBtn flowBtn = GetAllWorkButtons(x => x.ButtonType == 0 && x.ButtonText == "同意").FirstOrDefault();
                if (flowBtn != null)
                    btns.Add(new FormButton() { TagId = string.Format("flowBtn_{0}", flowBtn.Id), IconType = ButtonIconType.FlowAgree, Icon = flowBtn.ButtonIcon, DisplayText = flowBtn.ButtonText, ClickMethod = "ApprovalFlow(this)", Sort = 1 });
            }
            return btns;
        }

        /// <summary>
        /// 获取发起结点流程操作按钮集合
        /// </summary>
        /// <param name="todoTaskId">待办ID，针对回退到发起结点时的待办ID</param>
        /// <returns></returns>
        public static List<FormButton> GetLaunchNodeFlowBtns(Guid? todoTaskId = null)
        {
            List<FormButton> btns = new List<FormButton>();
            btns.Add(new FormButton() { TagId = string.Format("flowBtn_{0}", todoTaskId.HasValue ? todoTaskId.Value : Guid.Empty), DisplayText = "保存并提交", IconType = ButtonIconType.Save, ClickMethod = "SubmitFlow(this)", Icon = "eu-icon-tosubmit", Sort = 4 });
            return btns;
        }

        /// <summary>
        /// 根据待办任务ID获取审批表单流程操作按钮集合
        /// </summary>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static List<FormButton> GetNodeFlowBtns(Guid todoTaskId, UserInfo currUser = null)
        {
            string errMsg = string.Empty;
            bool isParentTodo = false;
            Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(todoTaskId, out errMsg);
            if (todo == null)
            {
                Bpm_WorkToDoListHistory todoHistory = CommonOperate.GetEntityById<Bpm_WorkToDoListHistory>(todoTaskId, out errMsg);
                if (todoHistory != null)
                    isParentTodo = todoHistory.IsParentTodo == true;
            }
            else
            {
                isParentTodo = todo.IsParentTodo == true;
            }
            if (!isParentTodo) //非父待办
            {
                if (todo == null) return new List<FormButton>();
                if (todo.Bpm_WorkNodeInstanceId.HasValue && todo.Bpm_WorkFlowInstanceId.HasValue)
                {
                    Bpm_WorkFlowInstance workflowInst = CommonOperate.GetEntityById<Bpm_WorkFlowInstance>(todo.Bpm_WorkFlowInstanceId.Value, out errMsg);
                    if (workflowInst != null && workflowInst.Bpm_WorkFlowId.HasValue && workflowInst.Bpm_WorkFlowId.Value != Guid.Empty)
                    {
                        Bpm_WorkNodeInstance workNodeInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstance>(todo.Bpm_WorkNodeInstanceId.Value, out errMsg);
                        if (workNodeInst != null && workNodeInst.Bpm_WorkNodeId.HasValue && workNodeInst.Bpm_WorkNodeId.Value != Guid.Empty)
                        {
                            Bpm_WorkNode launchNode = BpmOperate.GetLaunchNode(workflowInst.Bpm_WorkFlowId.Value);
                            if (launchNode != null && launchNode.Id == workNodeInst.Bpm_WorkNodeId.Value)
                                return GetLaunchNodeFlowBtns(todoTaskId);
                            return GetWorkNodeFormBtns(workflowInst.Bpm_WorkFlowId.Value, workNodeInst.Bpm_WorkNodeId.Value);
                        }
                    }
                }
            }
            else //当前为父待办，子流程
            {
                UserInfo tempCurrUser = currUser;
                if (tempCurrUser == null || !tempCurrUser.EmpId.HasValue)
                    return new List<FormButton>();
                int noAction = (int)WorkActionEnum.NoAction;
                Bpm_WorkToDoList childTodo = CommonOperate.GetEntity<Bpm_WorkToDoList>(x => x.ParentId == todoTaskId && x.OrgM_EmpId == currUser.EmpId.Value && x.WorkAction == noAction && x.IsDeleted == false, null, out errMsg);
                if (childTodo.Bpm_WorkNodeInstanceId.HasValue && childTodo.Bpm_WorkFlowInstanceId.HasValue)
                {
                    Bpm_WorkFlowInstance workflowInst = CommonOperate.GetEntityById<Bpm_WorkFlowInstance>(childTodo.Bpm_WorkFlowInstanceId.Value, out errMsg);
                    if (workflowInst != null && workflowInst.Bpm_WorkFlowId.HasValue && workflowInst.Bpm_WorkFlowId.Value != Guid.Empty)
                    {
                        Bpm_WorkNodeInstance workNodeInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstance>(childTodo.Bpm_WorkNodeInstanceId.Value, out errMsg);
                        if (workNodeInst != null && workNodeInst.Bpm_WorkNodeId.HasValue && workNodeInst.Bpm_WorkNodeId.Value != Guid.Empty)
                        {
                            List<FormButton> btns = GetWorkNodeFormBtns(workflowInst.Bpm_WorkFlowId.Value, workNodeInst.Bpm_WorkNodeId.Value);
                            btns.ForEach(x => { x.ParentToDoId = todoTaskId.ToString(); });
                            return btns;
                        }
                    }
                }
            }
            return new List<FormButton>();
        }

        #endregion

        #region 流程操作

        #region 审批操作

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="currUser">发起人</param>
        /// <param name="parentFlowInstId">父流程实例ID</param>
        /// <param name="parentWorkTodoId">父待办ID</param>
        /// <param name="detailIds">当前子流程需要发起的所有明细ID集合，判断所有子流程是否结束时用到</param>
        /// <returns></returns>
        public static string StartProcess(Guid moduleId, Guid recordId, UserInfo currUser, Guid? parentFlowInstId = null, Guid? parentWorkTodoId = null, List<Guid> detailIds = null)
        {
            string errMsg = string.Empty;
            string errPrex = GetFlowErrPrex(WorkActionEnum.Starting);
            Bpm_WorkFlow workFlow = GetModuleWorkFlow(moduleId);
            if (workFlow == null)
                return string.Format("{0}模块流程配置获取失败", errPrex);
            Bpm_WorkNode launchNode = GetLaunchNode(workFlow.Id);
            if (launchNode == null)
                return string.Format("{0}发起结点信息获取失败", errPrex);
            if (currUser == null)
                return string.Format("{0}当前用户信息获取失败", errPrex);
            object model = CommonOperate.GetEntityById(moduleId, recordId, out errMsg);
            if (model == null)
                return string.Format("{0}记录信息不存在，{1}", errPrex, errMsg);
            if (BpmOperate.IsLaunchFlowCompleted(moduleId, recordId))
                return string.Format("{0}流程已发起，请不要重复发起", errPrex);
            if (!BpmOperate.IsAllowLaunchFlow(moduleId, currUser, null))
                return string.Format("{0}您没有该流程的发起权限，如有疑问请联系管理员", errPrex);
            string userAliasName = string.IsNullOrEmpty(currUser.AliasName) ? currUser.UserName : currUser.AliasName;
            string subStr = parentFlowInstId.HasValue && parentFlowInstId.Value != Guid.Empty ? "_SUB" : string.Empty;
            Bpm_WorkFlowInstance workFlowInst = new Bpm_WorkFlowInstance()
            {
                Code = string.Format("WFI{0}{1}", subStr, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                Bpm_WorkFlowId = workFlow.Id,
                Status = (int)WorkFlowStatusEnum.Start,
                StartDate = DateTime.Now,
                OrgM_EmpId = currUser.EmpId,
                OrgM_EmpName = string.IsNullOrEmpty(currUser.EmpName) ? currUser.UserName : currUser.EmpName,
                RecordId = recordId,
                ParentId = parentFlowInstId,
                CreateUserId = currUser.UserId,
                CreateUserName = userAliasName,
                ModifyUserId = currUser.UserId,
                ModifyUserName = userAliasName,
            };
            Bpm_WorkNodeInstance launchNodeInst = new Bpm_WorkNodeInstance()
            {
                SerialNo = string.Format("WNI{0}{1}", subStr, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                StartDate = DateTime.Now,
                FinishDate = DateTime.Now,
                StatusOfEnum = WorkNodeStatusEnum.Do,
                CreateUserId = currUser.UserId,
                CreateUserName = userAliasName,
                ModifyUserId = currUser.UserId,
                ModifyUserName = userAliasName,
            };
            WorkActionEnum workAction = parentFlowInstId.HasValue && parentFlowInstId.Value != Guid.Empty ? WorkActionEnum.SubStarting : WorkActionEnum.Starting;
            string opinions = workAction == WorkActionEnum.SubStarting ? "发起子流程" : "发起流程";
            errMsg = HandleProcess(workFlow, workFlowInst, launchNode, launchNodeInst, currUser, workAction, opinions, null, null, null, parentWorkTodoId, detailIds);
            return errMsg;
        }

        /// <summary>
        /// 审批流程
        /// </summary>
        /// <param name="workTodoId">待办任务ID</param>
        /// <param name="approvalOpinions">审批意见</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="workAction">审批动作</param>
        /// <param name="returnNodeId">针对回退时回退节点ID</param>
        /// <param name="directHandler">针对指派时被指派人ID</param>
        /// <returns></returns>
        public static string ApproveProcess(Guid workTodoId, string approvalOpinions, UserInfo currUser, WorkActionEnum workAction, Guid? returnNodeId = null, Guid? directHandler = null)
        {
            WorkFlowStatusEnum flowStatus = WorkFlowStatusEnum.NoStatus;
            string errPrex = string.Empty;
            GetFlowStatusAndErrPrex(workAction, out flowStatus, out errPrex);
            if (workAction == WorkActionEnum.NoAction || workAction == WorkActionEnum.Starting)
                return string.Format("{0}没有可用的操作", errPrex);
            string errMsg = string.Empty;
            Bpm_WorkToDoList workTodo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(workTodoId, out errMsg);
            if (workTodo == null || !workTodo.Bpm_WorkFlowInstanceId.HasValue || !workTodo.Bpm_WorkNodeInstanceId.HasValue)
                return string.Format("{0}当前待办任务数据丢失", errPrex);
            Bpm_WorkFlowInstance workFlowInst = CommonOperate.GetEntityById<Bpm_WorkFlowInstance>(workTodo.Bpm_WorkFlowInstanceId.Value, out errMsg);
            if (workFlowInst == null || !workFlowInst.Bpm_WorkFlowId.HasValue)
                return string.Format("{0}当前待办任务对应的流程实例数据丢失", errPrex);
            Bpm_WorkNodeInstance workNodeInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstance>(workTodo.Bpm_WorkNodeInstanceId.Value, out errMsg);
            if (workNodeInst == null || !workNodeInst.Bpm_WorkNodeId.HasValue)
                return string.Format("{0}当前待办任务对应的流程结点实例数据丢失", errPrex);
            Bpm_WorkFlow workFlow = GetWorkflow(workFlowInst.Bpm_WorkFlowId.Value);
            if (workFlow == null)
                return string.Format("{0}当前待办任务对应的流程结点数据丢失", errPrex);
            Bpm_WorkNode workNode = GetWorkNode(workNodeInst.Bpm_WorkNodeId.Value);
            if (workNode == null)
                return string.Format("{0}当前待办任务对应的流程结点数据丢失", errPrex);
            if (currUser == null)
                return string.Format("{0}当前用户信息获取失败", errPrex);
            if (currUser.UserName != "admin")
            {
                if (!BpmOperate.IsCurrentToDoTaskHandler(workTodoId, currUser))
                    return string.Format("{0}您不是当前待办的处理者，如有疑问请联系管理员", errPrex);
            }
            else if (workAction != WorkActionEnum.Directing)
            {
                return string.Format("{0}您不是当前待办的处理者，如有疑问请联系管理员", errPrex);
            }
            Bpm_WorkNode returnNode = null;
            if (workAction == WorkActionEnum.Returning) //回退时
            {
                if (returnNodeId.HasValue && returnNodeId.Value != Guid.Empty) //有回退结点ID参数
                    returnNode = GetWorkNode(returnNodeId.Value);
                else //默认回退到前一结点
                    returnNode = GetPrexNode(workFlow.Id, workNode.Id);
                if (returnNode == null)
                    return string.Format("{0}回退节点不存在，请联系系统运维人员", errPrex);
            }
            else if (workAction == WorkActionEnum.Directing) //指派时
            {
                if (!directHandler.HasValue || directHandler.Value == Guid.Empty)
                    return string.Format("{0}指派时必须指定处理人", errPrex);
                if (directHandler == currUser.EmpId)
                    return string.Format("{0}不能将待办指派给自己", errPrex);
            }
            workFlowInst.StatusOfEnum = flowStatus;
            errMsg = HandleProcess(workFlow, workFlowInst, workNode, workNodeInst, currUser, workAction, approvalOpinions, workTodo, returnNode, directHandler);
            return errMsg;
        }

        /// <summary>
        /// 锁对象
        /// </summary>
        private static object lockObj = new object();
        /// <summary>
        /// 处理流程
        /// </summary>
        /// <param name="workflow">流程</param>
        /// <param name="workFlowInst">流程实例</param>
        /// <param name="workNode">流程结点</param>
        /// <param name="workNodeInst">流程结点实例</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="workAction">操作动作</param>
        /// <param name="approvalOpinions">审批意见</param>
        /// <param name="workTodo">审批时处理的当前待办任务</param>
        /// <param name="returnNode">回退时的回退结点</param>
        /// <param name="directHandler">指派时的人员ID</param>
        /// <param name="parentWorkTodoId">父待办ID，针对发起子流程</param>
        /// <param name="detailIds">当前子流程需要发起的所有明细ID集合，判断所有子流程是否结束时用到</param>
        /// <returns></returns>
        private static string HandleProcess(Bpm_WorkFlow workflow, Bpm_WorkFlowInstance workFlowInst, Bpm_WorkNode workNode, Bpm_WorkNodeInstance workNodeInst, UserInfo currUser, WorkActionEnum workAction, string approvalOpinions, Bpm_WorkToDoList workTodo, Bpm_WorkNode returnNode = null, Guid? directHandler = null, Guid? parentWorkTodoId = null, List<Guid> detailIds = null)
        {
            #region 准备参数
            string errMsg = string.Empty;
            string userAliasName = string.IsNullOrEmpty(currUser.AliasName) ? currUser.UserName : currUser.AliasName;
            WorkFlowStatusEnum workflowStatus = workFlowInst.StatusOfEnum; //流程状态
            string errPrex = GetFlowErrPrex(workAction);
            if (workAction == WorkActionEnum.Returning && returnNode == null)
                return string.Format("{0}回退节点不存在，请联系系统运维人员", errPrex);
            DatabaseType dbType = DatabaseType.MsSqlServer;
            string connStr = ModelConfigHelper.GetModelConnStr(typeof(Bpm_WorkFlow), out dbType, false);
            string subStr = workFlowInst.ParentId.HasValue && workFlowInst.ParentId.Value != Guid.Empty ? "_SUB" : string.Empty;
            #region 并行审批判断
            bool isParallelHasRejectOver = false; //是否并行结点存在拒绝的就终止，只要有一个人拒绝，该并行结点就视为被拒绝，流程终止
            bool isParallelApproval = false; //是否是并行审批
            //并行审批判断条件：是并行审批结点并且对应该结点的并行审批还有其他人员未处理的，即并行审批
            //最后一个人审批时按非并行审批处理，并行审批不允许回退
            if (workTodo != null && workNode.HandleStrategyOfEnum == HandleStrategyTypeEnum.AllAgree)
            {
                if (workAction == WorkActionEnum.Returning)
                    return string.Format("{0}该审批结点为并行结点不允许进行该操作", errPrex);
                List<Guid?> tempHandlers = workTodo.NextNodeHandler.ObjToStr().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.ObjToGuidNull()).Where(x => x != Guid.Empty).ToList();
                bool isParallelNode = tempHandlers.Count > 1; //当前为并行结点
                tempHandlers = tempHandlers.Where(x => x != currUser.EmpId.Value).ToList();
                if (tempHandlers.Count > 0 || isParallelNode) //除了当前人还有其他并行审批人员
                {
                    List<Bpm_WorkNodeInstance> tempNodeInsts = CommonOperate.GetEntities<Bpm_WorkNodeInstance>(out errMsg, x => x.Bpm_WorkFlowInstanceId == workFlowInst.Id && x.Bpm_WorkNodeId == workNode.Id, null, false); //当前结点所有并行结点实例集合
                    List<Guid?> nodeInstIds = tempNodeInsts.Select(x => (Guid?)x.Id).Where(x => x != workNodeInst.Id).ToList(); //除当前结点实例外的其他结点实例ID集合
                    if (tempHandlers.Count > 0)
                    {
                        int tempWorkAction = (int)WorkActionEnum.NoAction;
                        Expression<Func<Bpm_WorkToDoList, bool>> exp = x => x.Bpm_WorkFlowInstanceId == workFlowInst.Id && nodeInstIds.Contains(x.Bpm_WorkNodeInstanceId) && tempHandlers.Contains(x.OrgM_EmpId) && x.WorkAction == tempWorkAction;
                        if (!workTodo.ParentId.HasValue || workTodo.ParentId.Value == Guid.Empty) //非子流程排除冻结的待办
                            exp = ExpressionExtension.And(exp, x => !x.IsDeleted);
                        List<Bpm_WorkToDoList> noHandleTodos = CommonOperate.GetEntities<Bpm_WorkToDoList>(out errMsg, exp, null, false);
                        if (noHandleTodos != null && noHandleTodos.Count > 0) //还有其他并行审批人员未审批
                            isParallelApproval = true;
                    }
                    if (isParallelNode)
                    {
                        int rejectAction = (int)WorkActionEnum.Refusing;
                        long rejectCount = CommonOperate.Count<Bpm_WorkToDoList>(out errMsg, false, x => x.Bpm_WorkFlowInstanceId == workFlowInst.Id && nodeInstIds.Contains(x.Bpm_WorkNodeInstanceId) && x.WorkAction == rejectAction);
                        isParallelHasRejectOver = rejectCount > 0;
                    }
                }
            }
            #endregion
            bool canTransfer = false; //达到转移待办历史数据条件
            List<Bpm_WorkToDoList> nextTodos = new List<Bpm_WorkToDoList>(); //下一结点待办集合
            #region 自动跳转相关
            Bpm_WorkNode autoJumpNode = null; //自动跳转结点
            Bpm_WorkNodeInstance autoJumpNodeInst = null; //自动跳转结点实例
            bool notFindNextHandlerIsAuto = false; //找不到处理人是否自动跳转
            bool handlerIsStarterAuto = false; //处理人是发起人自动跳转
            bool handlerIsAppearedAuto = false; //处理人出现过自动跳转
            bool handlerIsLastEmpAuto = false; //处理人与上一处理人相同
            #endregion
            #region 子流程预处理
            bool isStartSubFlow = workNode.Bpm_WorkFlowSubId.HasValue && workNode.Bpm_WorkFlowSubId.Value != Guid.Empty; //是否发起子流程或分支流程
            bool isChildFlowHandle = (parentWorkTodoId.HasValue && parentWorkTodoId.Value != Guid.Empty) || (workTodo != null && workTodo.ParentId.HasValue && workTodo.ParentId.Value != Guid.Empty); //子流程处理标识
            List<Guid> tempNextHandlers = new List<Guid>(); //临时下一待办处理人
            Dictionary<Guid, Bpm_WorkToDoList> tempHandleEmpIds = new Dictionary<Guid, Bpm_WorkToDoList>(); //临时下一待办处理人及待办集合
            List<Guid?> parentNextNodeInstIds = new List<Guid?>(); //父流程下一结点实例ID集合
            bool isAllChildFlowOver = false; //是否父流程下所有子均结束
            Bpm_WorkNode parentNextNode = null; //父流程下一审批结点
            if (workFlowInst.ParentId.HasValue && workFlowInst.ParentId.Value != Guid.Empty && isChildFlowHandle)
            {
                Bpm_WorkNode parentNode = GetParentFlowNode(workflow.Id); //当前子流程的父流程结点
                if (parentNode != null && parentNode.Bpm_WorkFlowId.HasValue)
                {
                    parentNextNode = GetNextWorkNodes(parentNode.Bpm_WorkFlowId.Value, parentNode.Id).FirstOrDefault(); //父流程下一审批结点集合
                    if (parentNextNode != null)
                    {
                        int overStatus = (int)WorkFlowStatusEnum.Over;
                        int refuseStatus = (int)WorkFlowStatusEnum.Refused;
                        //未结束的子流程数量
                        long subNoOverFlowCount = CommonOperate.Count<Bpm_WorkFlowInstance>(out errMsg, false, x => x.Bpm_WorkFlowId == workflow.Id && x.Id != workFlowInst.Id && x.Status != overStatus && x.Status != refuseStatus && x.ParentId == workFlowInst.ParentId);
                        if (subNoOverFlowCount == 0)
                        {
                            //子流程发起时，还未添加子流程实例信息，判断是否有下一结点，没有则结束
                            if (workFlowInst.Id != Guid.Empty) //子流程审批中
                            {
                                isAllChildFlowOver = true;
                            }
                            else if (GetNextWorkNodes(workflow.Id, workNode.Id).Count == 0 && detailIds != null && detailIds.Count == 1) //当前子流程只有一个结点，发起即结束，并且明细子流程只有一个
                            {
                                isAllChildFlowOver = true;
                            }
                            if (isAllChildFlowOver)
                            {
                                //下一结点实例
                                List<Bpm_WorkNodeInstance> insts = CommonOperate.GetEntities<Bpm_WorkNodeInstance>(out errMsg, x => x.Bpm_WorkFlowInstanceId == workFlowInst.ParentId.Value && x.Bpm_WorkNodeId == parentNextNode.Id, null, false, new List<string>() { "Id" });
                                if (insts != null && insts.Count > 0)
                                    parentNextNodeInstIds = insts.Select(x => (Guid?)x.Id).ToList();
                            }
                        }
                    }
                }
            }
            #endregion
            #region 回退预处理
            List<Guid> returnHandles = new List<Guid>(); //回退处理人
            if (workAction == WorkActionEnum.Returning) //回退
            {
                Bpm_WorkNodeInstance nodeInst = CommonOperate.GetEntity<Bpm_WorkNodeInstance>(x => x.Bpm_WorkFlowInstanceId == workFlowInst.Id && x.Bpm_WorkNodeId == returnNode.Id, null, out errMsg, new List<string>() { "Id" });
                if (nodeInst != null)
                {
                    List<Bpm_WorkToDoList> tempTodos = CommonOperate.GetEntities<Bpm_WorkToDoList>(out errMsg, x => x.Bpm_WorkFlowInstanceId == workFlowInst.Id && x.Bpm_WorkNodeInstanceId == nodeInst.Id, null, false);
                    if (tempTodos != null && tempTodos.Count > 0)
                        returnHandles = tempTodos.Where(x => x.OrgM_EmpId.HasValue).Select(x => x.OrgM_EmpId.Value).ToList();
                }
            }
            #endregion
            #endregion
            #region 事务处理
            lock (lockObj)
            {
                //启用事务
                CommonOperate.TransactionHandle((conn) =>
                {
                    #region 针对流程发起处理流程实例及结点实例
                    //针对流程发起，初始化流程实例、流程结点实例
                    if (workFlowInst.Id == Guid.Empty || workTodo == null)
                    {
                        string subPrex = workAction == WorkActionEnum.SubStarting ? "_SUB" : string.Empty;
                        string tempCode = string.Format("WFI{0}{1}", subPrex, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                        workFlowInst.Code = tempCode;
                        Guid workFlowInstId = CommonOperate.OperateRecord<Bpm_WorkFlowInstance>(workFlowInst, ModelRecordOperateType.Add, out errMsg, null, false, false, null, null, conn);
                        if (workFlowInstId == Guid.Empty)
                            throw new Exception(string.Format("{0}流程实例初始化失败，{1}", errPrex, errMsg));
                        workFlowInst.Id = workFlowInstId;
                        workNodeInst.SerialNo = string.Format("WNI{0}{1}", subPrex, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                        workNodeInst.Bpm_WorkNodeId = workNode.Id;
                        workNodeInst.Bpm_WorkFlowInstanceId = workFlowInstId;
                        Guid workNodeInstId = CommonOperate.OperateRecord<Bpm_WorkNodeInstance>(workNodeInst, ModelRecordOperateType.Add, out errMsg, null, false, false, null, null, conn);
                        if (workNodeInstId == Guid.Empty)
                        {
                            errMsg = string.Format("{0}流程结点实例初始化失败，{1}", errPrex, errMsg);
                            throw new Exception(errMsg);
                        }
                        workNodeInst.Id = workNodeInstId;
                    }
                    #endregion
                    #region 针对非并行审批
                    if (!isParallelApproval) //非并行审批
                    {
                        if (workAction != WorkActionEnum.Directing) //非指派操作
                        {
                            #region 找下一审批结点及处理人
                            //处理当前结点实例
                            workNodeInst.FinishDate = DateTime.Now;
                            workNodeInst.StatusOfEnum = WorkNodeStatusEnum.Do;
                            Guid result = CommonOperate.OperateRecord<Bpm_WorkNodeInstance>(workNodeInst, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "Status", "FinishDate" }, false, false, null, null, conn);
                            if (result == Guid.Empty)
                                throw new Exception(string.Format("{0}流程结点实例状态更新失败，{1}", errPrex, errMsg));
                            //待办处理
                            List<Guid> handleEmpIds = new List<Guid>(); //下一结点待办人
                            Bpm_WorkNode nextApprovalNode = null; //下一审批结点
                            List<string> updateWorkflowInstFields = new List<string>() { "Status" };
                            List<Bpm_WorkNode> nextNodes = new List<Bpm_WorkNode>();
                            if (workAction == WorkActionEnum.Approving || workAction == WorkActionEnum.Starting ||
                                workAction == WorkActionEnum.SubStarting || workAction == WorkActionEnum.ReStarting)
                            {
                                if (workAction == WorkActionEnum.Approving && isParallelHasRejectOver) //并行结点审批并且存在拒绝的情况
                                    nextNodes = new List<Bpm_WorkNode>();
                                else
                                    nextNodes = GetNextWorkNodes(workflow.Id, workNode.Id);
                            }
                            else if (workAction == WorkActionEnum.Returning) //回退
                            {
                                nextNodes = new List<Bpm_WorkNode>() { returnNode };
                                workflowStatus = WorkFlowStatusEnum.Return;
                                isStartSubFlow = false;
                            }
                            else if (workAction == WorkActionEnum.Refusing) //拒绝
                            {
                                nextNodes = new List<Bpm_WorkNode>();
                                workflowStatus = WorkFlowStatusEnum.Refused;
                                isStartSubFlow = false;
                            }
                            if (nextNodes.Count > 1) //对应的下一结点有多个
                            {
                                //调用自定义取流程节点接口
                                object nextHandleNodeNameObj = CommonOperate.ExecuteCustomeOperateHandleMethod(workflow.Sys_ModuleId.Value, "GetFlowNextHandleNodeName", new object[] { workFlowInst.RecordId, workflow.Name, workNode.Name, currUser });
                                if (!string.IsNullOrEmpty(nextHandleNodeNameObj.ObjToStr()))
                                {
                                    string nodeName = nextHandleNodeNameObj.ObjToStr();
                                    nextApprovalNode = GetAllWorkNodes(x => x.Bpm_WorkFlowId == workflow.Id && x.Name == nodeName).FirstOrDefault();
                                }
                                else
                                {
                                    nextApprovalNode = GetNextActionNode(nextNodes, workflow, workNode, workFlowInst, workNodeInst);
                                }
                                if (nextApprovalNode == null)
                                {
                                    errMsg = string.Format("{0}找不到当前结点【{1}】的下一处理结点，指向所有下一结点流转条件均不满足", errPrex, workNode.Name);
                                    throw new Exception(errMsg);
                                }
                                handleEmpIds = GetNextNodeHandler(nextApprovalNode, currUser, workFlowInst);
                            }
                            else if (nextNodes.Count == 1) //只有一个
                            {
                                nextApprovalNode = nextNodes.FirstOrDefault();
                                if (workAction == WorkActionEnum.Returning) //回退
                                    handleEmpIds = returnHandles;
                                else
                                    handleEmpIds = GetNextNodeHandler(nextApprovalNode, currUser, workFlowInst);
                            }
                            else //没有下一结点，流程结束
                            {
                                if (workAction != WorkActionEnum.Refusing && !isParallelHasRejectOver)
                                    workflowStatus = WorkFlowStatusEnum.Over;
                                workFlowInst.EndDate = DateTime.Now;
                                updateWorkflowInstFields.Add("EndDate");
                            }
                            if (nextApprovalNode != null && nextApprovalNode.HandleStrategyOfEnum == HandleStrategyTypeEnum.AllAgree && handleEmpIds.Count == 0) //下一结点为并行结点并且无处理人时
                                throw new Exception(string.Format("{0}找不到当前结点【{1}】的下一结点【{2}】处理人", errPrex, workNode.Name, nextApprovalNode.Name));
                            //更新流程实例状态
                            workFlowInst.StatusOfEnum = isParallelHasRejectOver ? WorkFlowStatusEnum.Refused : workflowStatus;
                            result = CommonOperate.OperateRecord<Bpm_WorkFlowInstance>(workFlowInst, ModelRecordOperateType.Edit, out errMsg, updateWorkflowInstFields, false, false, null, null, conn);
                            if (result == Guid.Empty)
                                throw new Exception(string.Format("{0}流程实例状态更新失败，{1}", errPrex, errMsg));
                            #endregion
                            #region 处理当前待办
                            //处理当前待办
                            if (workTodo == null) //发起节点
                            {
                                #region 发起待办初始化
                                string titleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(workflow.Sys_ModuleId.Value);
                                string titleKeyValue = string.IsNullOrEmpty(titleKeyDisplay) ? string.Empty : CommonOperate.GetModelTitleKeyValue(workflow.Sys_ModuleId.Value, workFlowInst.RecordId);
                                //获取模块自定义待办标题
                                string title = CommonOperate.ExecuteCustomeOperateHandleMethod(workflow.Sys_ModuleId.Value, "GetWorkTodoTitle", new object[] { workFlowInst.RecordId, workflow.Name, currUser }).ObjToStr();
                                if (string.IsNullOrEmpty(title)) //没有自定义标题则使用通用标题
                                {
                                    string formatStr = string.IsNullOrEmpty(titleKeyDisplay) || string.IsNullOrEmpty(titleKeyValue) ? string.Empty : string.Format("－{0}：{1}", titleKeyDisplay, titleKeyValue);
                                    string deptName = currUser.UserName != "admin" && workFlowInst.OrgM_EmpId.HasValue ? OrgMOperate.GetEmpMainDeptName(workFlowInst.OrgM_EmpId.Value) : string.Empty;
                                    title = string.Format("{0}{1}发起【{2}】流程{3}", deptName, workFlowInst.OrgM_EmpName.ObjToStr(), workflow.Sys_ModuleName, formatStr);
                                }
                                workTodo = new Bpm_WorkToDoList()
                                {
                                    Code = string.Format("WT{0}{1}", subStr, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                    Title = title,
                                    Bpm_WorkFlowInstanceId = workFlowInst.Id,
                                    Bpm_WorkNodeInstanceId = workNodeInst.Id,
                                    OrgM_EmpId = currUser.UserName == "admin" && !currUser.EmpId.HasValue ? Guid.Empty : currUser.EmpId.Value,
                                    Launcher = workFlowInst.OrgM_EmpName,
                                    LaunchTime = workFlowInst.StartDate,
                                    ModuleId = workflow.Sys_ModuleId.Value,
                                    RecordId = workFlowInst.RecordId,
                                    StartDate = DateTime.Now,
                                    FinishDate = DateTime.Now,
                                    StatusOfEnum = workflowStatus,
                                    WorkActionOfEnum = WorkActionEnum.Starting,
                                    ApprovalOpinions = approvalOpinions,
                                    Bpm_WorkNodeId = nextApprovalNode != null ? nextApprovalNode.Id : (Guid?)null,
                                    NextNodeHandler = string.Join(",", handleEmpIds),
                                    CreateUserId = currUser.UserId,
                                    CreateUserName = userAliasName,
                                    ModifyUserId = currUser.UserId,
                                    ModifyUserName = userAliasName,
                                    IsInitHandler = true
                                };
                                if (parentWorkTodoId.HasValue && parentWorkTodoId.Value != Guid.Empty) //当前为子流程
                                    workTodo.ParentId = parentWorkTodoId.Value;
                                if (isStartSubFlow && workNode.SubFlowTypeOfEnum == SubFlowTypeEnum.ChildFlow)
                                    workTodo.IsParentTodo = true;
                                result = CommonOperate.OperateRecord<Bpm_WorkToDoList>(workTodo, ModelRecordOperateType.Add, out errMsg, null, false, false, null, null, conn);
                                if (result == Guid.Empty)
                                    throw new Exception(string.Format("{0}当前待办任务初始化失败,{1}", errPrex, errMsg));
                                #endregion
                            }
                            else //审批节点
                            {
                                #region 当前待办初始化
                                List<string> upFn = new List<string>() { "ApprovalOpinions", "FinishDate", "Status", "WorkAction", "IsDeleted" };
                                workTodo.ApprovalOpinions = approvalOpinions;
                                workTodo.FinishDate = DateTime.Now;
                                workTodo.StatusOfEnum = workflowStatus;
                                workTodo.WorkActionOfEnum = workAction;
                                workTodo.IsDeleted = false;
                                if (nextApprovalNode == null) //不存在下一审批节点
                                {
                                    if (workAction != WorkActionEnum.Refusing) //审批结束
                                    {
                                        Bpm_WorkNode endNode = GetEndNode(workflow.Id);
                                        if (endNode != null)
                                        {
                                            workTodo.NextNodeHandler = string.Empty;
                                            workTodo.Bpm_WorkNodeId = endNode.Id;
                                            upFn.Add("Bpm_WorkNodeId");
                                            upFn.Add("NextNodeHandler");
                                        }
                                    }
                                    else //审批拒绝
                                    {
                                        workTodo.NextNodeHandler = string.Empty;
                                        workTodo.Bpm_WorkNodeId = null;
                                        upFn.Add("Bpm_WorkNodeId");
                                        upFn.Add("NextNodeHandler");
                                    }
                                }
                                else //有下一审批节点
                                {
                                    workTodo.Bpm_WorkNodeId = nextApprovalNode.Id;
                                    workTodo.NextNodeHandler = string.Join(",", handleEmpIds);
                                    upFn.Add("Bpm_WorkNodeId");
                                    upFn.Add("NextNodeHandler");
                                }
                                result = CommonOperate.OperateRecord<Bpm_WorkToDoList>(workTodo, ModelRecordOperateType.Edit, out errMsg, upFn, false, false, null, null, conn);
                                if (result == Guid.Empty)
                                    throw new Exception(string.Format("{0}当前待办任务状态更新失败,{1}", errPrex, errMsg));
                                #endregion
                            }
                            #endregion
                            //处理下一结点
                            if (nextApprovalNode != null)
                            {
                                #region 自动跳转处理
                                if ((workAction == WorkActionEnum.Starting || workAction == WorkActionEnum.SubStarting ||
                                    workAction == WorkActionEnum.Approving) && !isStartSubFlow)
                                {
                                    //自动跳转规则
                                    string autoJumpRule = nextApprovalNode.AutoJumpRule.ObjToStr();
                                    string[] autoJumpToken = autoJumpRule.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                    if (autoJumpToken == null || autoJumpToken.Length != 5)
                                        autoJumpToken = new string[] { "1", "1", "1", "0", "0" };
                                    notFindNextHandlerIsAuto = autoJumpToken[3] == "1";
                                    handlerIsStarterAuto = autoJumpToken[0] == "1";
                                    handlerIsAppearedAuto = autoJumpToken[1] == "1";
                                    handlerIsLastEmpAuto = workAction == WorkActionEnum.Approving && autoJumpToken[2] == "1";
                                }
                                #endregion
                                #region 初始化下一审批结点实例和下一审批结点待办
                                //初始化下一结点实例
                                Dictionary<Guid, Bpm_WorkNodeInstance> nextApprovalNodeInstDic = null;
                                Bpm_WorkNodeInstance nextApprovalNodeInst = null;
                                if (nextApprovalNode.HandleStrategyOfEnum == HandleStrategyTypeEnum.AllAgree) //下一结点为并行结点
                                {
                                    nextApprovalNodeInstDic = new Dictionary<Guid, Bpm_WorkNodeInstance>();
                                    foreach (Guid tempEmpId in handleEmpIds)
                                    {
                                        Bpm_WorkNodeInstance tempNodeInst = new Bpm_WorkNodeInstance()
                                        {
                                            SerialNo = string.Format("WNI{0}{1}", subStr, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                            Bpm_WorkFlowInstanceId = workFlowInst.Id,
                                            Bpm_WorkNodeId = nextApprovalNode.Id,
                                            StartDate = DateTime.Now,
                                            StatusOfEnum = WorkNodeStatusEnum.Undo,
                                            CreateUserId = currUser.UserId,
                                            CreateUserName = userAliasName,
                                            ModifyUserId = currUser.UserId,
                                            ModifyUserName = userAliasName,
                                        };
                                        Guid tempNodeInstId = CommonOperate.OperateRecord<Bpm_WorkNodeInstance>(tempNodeInst, ModelRecordOperateType.Add, out errMsg, null, false, false, null, null, conn);
                                        if (tempNodeInstId == Guid.Empty)
                                            throw new Exception(string.Format("{0}下一审批结点实例初始化失败,{1}", errPrex, errMsg));
                                        tempNodeInst.Id = tempNodeInstId;
                                        nextApprovalNodeInstDic.Add(tempEmpId, tempNodeInst);
                                    }
                                    if (nextApprovalNodeInstDic.Count == 1)
                                        nextApprovalNodeInst = nextApprovalNodeInstDic.FirstOrDefault().Value;
                                }
                                else //下一结点为串行结点
                                {
                                    nextApprovalNodeInst = new Bpm_WorkNodeInstance()
                                    {
                                        SerialNo = string.Format("WNI{0}{1}", subStr, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                        Bpm_WorkFlowInstanceId = workFlowInst.Id,
                                        Bpm_WorkNodeId = nextApprovalNode.Id,
                                        StartDate = DateTime.Now,
                                        StatusOfEnum = WorkNodeStatusEnum.Undo,
                                        CreateUserId = currUser.UserId,
                                        CreateUserName = userAliasName,
                                        ModifyUserId = currUser.UserId,
                                        ModifyUserName = userAliasName,
                                    };
                                    Guid nextApprovalNodeInstId = CommonOperate.OperateRecord<Bpm_WorkNodeInstance>(nextApprovalNodeInst, ModelRecordOperateType.Add, out errMsg, null, false, false, null, null, conn);
                                    if (nextApprovalNodeInstId == Guid.Empty)
                                        throw new Exception(string.Format("{0}下一审批结点实例初始化失败,{1}", errPrex, errMsg));
                                    nextApprovalNodeInst.Id = nextApprovalNodeInstId;
                                }
                                bool isUpdateNextHandler = false;
                                if (handleEmpIds.Count == 0) //找不到下一结点处理人
                                {
                                    if (notFindNextHandlerIsAuto && !isStartSubFlow) //找不到下一处理者自动跳转
                                    {
                                        handleEmpIds = new List<Guid>() { Guid.Empty };
                                        isUpdateNextHandler = true;
                                    }
                                    else if (workflow.OrgM_EmpId.HasValue && workflow.OrgM_EmpId.Value != Guid.Empty) //设定了流程管理人时待办转到管理人
                                    {
                                        handleEmpIds = new List<Guid>() { workflow.OrgM_EmpId.Value };
                                        isUpdateNextHandler = true;
                                    }
                                    else //否则弹出错误提示
                                    {
                                        throw new Exception(string.Format("{0}找不到当前结点【{1}】的下一结点【{2}】处理人", errPrex, workNode.Name, nextApprovalNode.Name));
                                    }
                                }
                                //生成待办
                                List<Bpm_WorkToDoList> nextTodoList = new List<Bpm_WorkToDoList>();
                                int num = 0;
                                foreach (Guid empId in handleEmpIds)
                                {
                                    num++;
                                    string code = string.Format("WT{0}{1}{2}", subStr, DateTime.Now.ToString("yyyyMMddHHmmssff"), num);
                                    Bpm_WorkToDoList toDo = new Bpm_WorkToDoList()
                                    {
                                        Code = code,
                                        Title = workTodo.Title,
                                        Bpm_WorkFlowInstanceId = workFlowInst.Id,
                                        Bpm_WorkNodeInstanceId = nextApprovalNodeInstDic != null ? nextApprovalNodeInstDic[empId].Id : nextApprovalNodeInst.Id,
                                        OrgM_EmpId = empId,
                                        Launcher = workTodo.Launcher,
                                        LaunchTime = workTodo.LaunchTime,
                                        ModuleId = workTodo.ModuleId,
                                        RecordId = workTodo.RecordId,
                                        StartDate = DateTime.Now,
                                        StatusOfEnum = workflowStatus,
                                        WorkActionOfEnum = WorkActionEnum.NoAction,
                                        Bpm_WorkNodeId = nextApprovalNode != null ? nextApprovalNode.Id : (Guid?)null,
                                        NextNodeHandler = string.Join(",", handleEmpIds),
                                        CreateUserId = currUser.UserId,
                                        CreateUserName = userAliasName,
                                        ModifyUserId = currUser.UserId,
                                        ModifyUserName = userAliasName,
                                        IsInitHandler = !isUpdateNextHandler
                                    };
                                    if ((isStartSubFlow && workNode.SubFlowTypeOfEnum == SubFlowTypeEnum.ChildFlow) || isChildFlowHandle)
                                    {
                                        //有子流程时即当前为父流程或当前为子流程时，不显示下一待办
                                        toDo.IsDeleted = true;
                                        if (isChildFlowHandle) //当前为子流程
                                        {
                                            if (parentWorkTodoId.HasValue && parentWorkTodoId.Value != Guid.Empty)
                                                toDo.ParentId = parentWorkTodoId.Value;
                                            else
                                                toDo.ParentId = workTodo.ParentId;
                                        }
                                    }
                                    nextTodoList.Add(toDo);
                                    if (tempHandleEmpIds.Values.Count(x => x.ParentId.HasValue && x.ParentId != Guid.Empty && x.ParentId == toDo.ParentId && x.OrgM_EmpId == empId) == 0)
                                        tempHandleEmpIds.Add(empId, toDo);
                                }
                                tempNextHandlers = handleEmpIds;
                                bool rs = CommonOperate.OperateRecords<Bpm_WorkToDoList>(nextTodoList, ModelRecordOperateType.Add, out errMsg, false, false, null, null, conn);
                                if (!rs)
                                    throw new Exception(string.Format("{0}下一审批节点待办任务初始化失败，{1}", errPrex, errMsg));
                                if (isUpdateNextHandler) //当前待办更新下一处理人
                                {
                                    workTodo.NextNodeHandler = string.Join(",", handleEmpIds);
                                    result = CommonOperate.OperateRecord<Bpm_WorkToDoList>(workTodo, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "NextNodeHandler" }, false, false, null, null, conn);
                                    if (result == Guid.Empty)
                                        throw new Exception(string.Format("{0}当前待办任务下一处理人更新失败,{1}", errPrex, errMsg));
                                }
                                nextTodos = nextTodoList;
                                if (!isStartSubFlow || workNode.SubFlowTypeOfEnum != SubFlowTypeEnum.ChildFlow) //有子流程时不允许自动跳转
                                {
                                    //下一结点为并行结点并且有多个处理人时不允许自动跳转
                                    if (nextApprovalNodeInstDic == null || nextApprovalNodeInstDic.Count == 1)
                                    {
                                        autoJumpNode = nextApprovalNode;
                                        autoJumpNodeInst = nextApprovalNodeInst;
                                    }
                                }
                                #endregion
                            }
                            #region 流程结束
                            if (workflowStatus == WorkFlowStatusEnum.Over || workflowStatus == WorkFlowStatusEnum.Refused || isParallelHasRejectOver) //流程结束或拒绝
                            {
                                //如果当前流程是子流程，如果所有子流程都结束，则主流程继续流到下一结点
                                if (isChildFlowHandle) //当前为子流程
                                {
                                    if (parentNextNodeInstIds.Count > 0 && isAllChildFlowOver) //父流程下一审批节点存在并且所有子流程均结束
                                    {
                                        //显示父流程下一结点的所有待办
                                        CommonOperate.UpdateRecordsByExpression<Bpm_WorkToDoList>(new { IsDeleted = false }, x => x.Bpm_WorkFlowInstanceId == workFlowInst.ParentId.Value && parentNextNodeInstIds.Contains(x.Bpm_WorkNodeInstanceId), out errMsg, null, null, conn);
                                    }
                                }
                                canTransfer = true;
                            }
                            #endregion
                        }
                        else //流程指派
                        {
                            #region 指派处理
                            isStartSubFlow = false;
                            //指派人待办
                            Bpm_WorkToDoList directTodo = new Bpm_WorkToDoList()
                            {
                                Code = string.Format("WT{0}{1}", subStr, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                Title = workTodo.Title,
                                Bpm_WorkFlowInstanceId = workTodo.Bpm_WorkFlowInstanceId,
                                Bpm_WorkNodeInstanceId = workTodo.Bpm_WorkNodeInstanceId,
                                OrgM_EmpId = directHandler,
                                Launcher = workTodo.Launcher,
                                LaunchTime = workTodo.LaunchTime,
                                ModuleId = workTodo.ModuleId,
                                RecordId = workTodo.RecordId,
                                StartDate = DateTime.Now,
                                StatusOfEnum = workTodo.StatusOfEnum,
                                WorkActionOfEnum = WorkActionEnum.NoAction,
                                Bpm_WorkNodeId = workTodo.Bpm_WorkNodeId,
                                NextNodeHandler = workTodo.NextNodeHandler,
                                CreateUserId = currUser.UserId,
                                CreateUserName = userAliasName,
                                ModifyUserId = currUser.UserId,
                                ModifyUserName = userAliasName
                            };
                            List<string> upFn = new List<string>() { "ApprovalOpinions", "FinishDate", "WorkAction", "Bpm_WorkNodeId", "NextNodeHandler" };
                            workTodo.ApprovalOpinions = approvalOpinions;
                            workTodo.FinishDate = DateTime.Now;
                            workTodo.WorkActionOfEnum = workAction;
                            workTodo.Bpm_WorkNodeId = workNode.Id; //下一结点仍然指向当前结点
                            workTodo.NextNodeHandler = directHandler.HasValue ? directHandler.Value.ObjToStr() : string.Empty; //下一结点处理人指向指派人
                            tempNextHandlers = new List<Guid>() { directHandler.Value };
                            tempHandleEmpIds.Add(directHandler.Value, directTodo);
                            if (isChildFlowHandle) //当前为子流程
                            {
                                directTodo.ParentId = workTodo.ParentId;
                            }
                            Guid result = CommonOperate.OperateRecord<Bpm_WorkToDoList>(workTodo, ModelRecordOperateType.Edit, out errMsg, upFn, false, false, null, null, conn);
                            if (result != Guid.Empty)
                            {
                                result = CommonOperate.OperateRecord<Bpm_WorkToDoList>(directTodo, ModelRecordOperateType.Add, out errMsg, null, false, false, null, null, conn);
                                if (result == Guid.Empty)
                                    throw new Exception(string.Format("{0}指派人待办任务初始化失败,{1}", errPrex, errMsg));
                            }
                            else
                            {
                                throw new Exception(string.Format("{0}当前待办任务状态更新失败,{1}", errPrex, errMsg));
                            }
                            #endregion
                        }
                    }
                    #endregion
                    #region 并行审批
                    else //并行审批
                    {
                        //处理当前待办
                        List<string> upFn = new List<string>() { "ApprovalOpinions", "FinishDate", "WorkAction", "IsDeleted" };
                        workTodo.ApprovalOpinions = approvalOpinions;
                        workTodo.FinishDate = DateTime.Now;
                        workTodo.WorkActionOfEnum = workAction;
                        workTodo.IsDeleted = false;
                        #region 指派处理
                        Bpm_WorkToDoList directTodo = null; //指派人待办
                        if (workAction == WorkActionEnum.Directing)
                        {
                            //指派人待办
                            directTodo = new Bpm_WorkToDoList()
                            {
                                Code = string.Format("WT{0}{1}", subStr, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                Title = workTodo.Title,
                                Bpm_WorkFlowInstanceId = workTodo.Bpm_WorkFlowInstanceId,
                                Bpm_WorkNodeInstanceId = workTodo.Bpm_WorkNodeInstanceId,
                                OrgM_EmpId = directHandler,
                                Launcher = workTodo.Launcher,
                                LaunchTime = workTodo.LaunchTime,
                                ModuleId = workTodo.ModuleId,
                                RecordId = workTodo.RecordId,
                                StartDate = DateTime.Now,
                                StatusOfEnum = workTodo.StatusOfEnum,
                                WorkActionOfEnum = WorkActionEnum.NoAction,
                                Bpm_WorkNodeId = workTodo.Bpm_WorkNodeId,
                                NextNodeHandler = workTodo.NextNodeHandler,
                                CreateUserId = currUser.UserId,
                                CreateUserName = userAliasName,
                                ModifyUserId = currUser.UserId,
                                ModifyUserName = userAliasName,
                            };
                            workTodo.Bpm_WorkNodeId = workNode.Id; //下一结点仍然指向当前结点
                            workTodo.NextNodeHandler = directHandler.ObjToStr(); //下一结点处理人指向指派人
                            upFn.Add("Bpm_WorkNodeId");
                            upFn.Add("NextNodeHandler");
                        }
                        else
                        {
                            workTodo.StatusOfEnum = workflowStatus;
                            upFn.Add("Status");
                        }
                        #endregion
                        Guid result = CommonOperate.OperateRecord<Bpm_WorkToDoList>(workTodo, ModelRecordOperateType.Edit, out errMsg, upFn, false, false, null, null, conn);
                        if (result != Guid.Empty)
                        {
                            if (directTodo != null)
                            {
                                result = CommonOperate.OperateRecord<Bpm_WorkToDoList>(directTodo, ModelRecordOperateType.Add, out errMsg, null, false, false, null, null, conn);
                                if (result == Guid.Empty)
                                    throw new Exception(string.Format("{0}指派人待办任务初始化失败,{1}", errPrex, errMsg));
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("{0}当前待办任务状态更新失败,{1}", errPrex, errMsg));
                        }
                    }
                    #endregion
                }, out errMsg, connStr, dbType);
            }
            #endregion
            if (string.IsNullOrEmpty(errMsg)) //流程处理成功
            {
                #region 操作完成事件触发
                //更新单据流程状态
                AsyncUpdateRecordFlowStatus(workflow.Sys_ModuleId.Value, workFlowInst.RecordId, isParallelHasRejectOver ? WorkFlowStatusEnum.Refused : workflowStatus, currUser);
                //转移历史待办数据
                if (canTransfer)
                {
                    TransferWorkToDoHistory(workFlowInst, currUser);
                }
                //执行流程操作完成事件
                CommonOperate.ExecuteCustomeOperateHandleMethod(workflow.Sys_ModuleId.Value, "AfterFlowOperateCompleted", new object[] { workFlowInst.RecordId, workNode.Name, currUser, string.IsNullOrEmpty(errMsg), workAction, workflowStatus, approvalOpinions });
                //事件通知
                SystemOperate.TriggerEventNotify(workflow.Sys_ModuleId.Value, workFlowInst.RecordId, workNode.Id, workAction, currUser, workFlowInst.OrgM_EmpId, tempHandleEmpIds, workflowStatus, directHandler, workTodo);
                #endregion
                #region 子流程待办显示处理
                //子流程待办根据父待办ID及待办人，针对同一父待办同一流程同一待办人只显示一
                //条子流程待办，点击子流程待办直接到父待办表单页面处理
                if (isChildFlowHandle) //当前为子流程
                {
                    Guid p_todoId = parentWorkTodoId.HasValue && parentWorkTodoId.Value != Guid.Empty ? parentWorkTodoId.Value : workTodo.ParentId.Value;
                    string tempMsg = string.Empty;
                    int noAction = (int)WorkActionEnum.NoAction;
                    if (workAction != WorkActionEnum.Starting && workAction != WorkActionEnum.SubStarting && workAction != WorkActionEnum.ReStarting)
                    {
                        Guid currEmpId = currUser.EmpId.HasValue ? currUser.EmpId.Value : Guid.Empty; //当前待办人
                        tempNextHandlers.Add(currEmpId);
                    }
                    foreach (Guid empId in tempNextHandlers) //对下一处理人待办处理包含当前待办人待办
                    {
                        List<Bpm_WorkToDoList> childTodos = CommonOperate.GetEntities<Bpm_WorkToDoList>(out tempMsg, x => x.ParentId == p_todoId && x.OrgM_EmpId == empId && x.WorkAction == noAction, null, false);
                        if (childTodos.Count == 0) continue;
                        if (childTodos.Where(x => x.IsDeleted == false).Count() == 0) //当前没有已经显示的子流程待办
                        {
                            Bpm_WorkToDoList tempTodo = childTodos.FirstOrDefault();
                            bool rs = CommonOperate.UpdateRecordsByExpression<Bpm_WorkToDoList>(new { IsDeleted = false }, x => x.Id == tempTodo.Id, out tempMsg);
                            if (!rs)
                            {
                                LogOperate.AddOperateLog(currUser, "待办任务", "子流程待办显示处理", string.Format("子流程待办显示处理失败，子流程待办ID【{0}】，待办人ID【{1}】", tempTodo.Id, empId), false, errMsg);
                            }
                        }
                    }
                }
                #endregion
                #region 发起子流程或分支流程
                if (isStartSubFlow) //需要发子流程或分支流程
                {
                    List<Guid> nextTodoIds = nextTodos.Select(x => x.Id).ToList();
                    Guid mainModuleId = GetWorkflowModuleId(workflow.Id); //当前流程模块ID
                    Type mainModelType = CommonOperate.GetModelType(mainModuleId); //当前流程模块类型
                    Guid subModuleId = GetWorkflowModuleId(workNode.Bpm_WorkFlowSubId.Value); //子流程模块
                    Type subModelType = CommonOperate.GetModelType(subModuleId);
                    string tableName = ModelConfigHelper.GetModuleTableName(subModelType);
                    //获取当前记录的明细ID集合
                    DatabaseType tempDbType = DatabaseType.MsSqlServer;
                    string tempConnStr = ModelConfigHelper.GetModelConnStr(subModelType, out tempDbType, true);
                    string sql = string.Format("SELECT Id FROM {0} WHERE {1}Id='{2}' AND IsDeleted=0", tableName, mainModelType.Name, workTodo.RecordId);
                    DataTable dt = CommonOperate.ExecuteQuery(out errMsg, sql, null, tempConnStr, tempDbType);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        List<Guid> subIds = dt.Rows.Cast<DataRow>().Select(x => x[0].ObjToGuid()).ToList();
                        foreach (Guid subId in subIds)
                        {
                            Guid? tempParentTodoId = workNode.SubFlowTypeOfEnum == SubFlowTypeEnum.ChildFlow ? workTodo.Id : (Guid?)null;
                            string msg = StartProcess(subModuleId, subId, currUser, workFlowInst.Id, tempParentTodoId, subIds); //以当前人身份发起子流程或分支流程
                            if (!string.IsNullOrEmpty(msg)) //子流程或分支流程发起失败
                            {
                                LogOperate.AddOperateLog(currUser, "待办任务", string.Format("{0}发起失败", tempParentTodoId.HasValue ? "子流程" : "分支流程"), string.Format("{0}发起失败，流程ID【{1}】，父流程ID【{2}】", string.Format("{0}发起失败", tempParentTodoId.HasValue ? "子流程" : "分支流程"), subId, workTodo.Id), false, msg);
                            }
                        }
                    }
                }
                #endregion
                #region 流程自动跳转
                else if ((autoJumpNode != null && autoJumpNodeInst != null) || (isChildFlowHandle && parentNextNodeInstIds.Count > 0 && isAllChildFlowOver))
                {
                    WorkActionEnum autoWorkAction = WorkActionEnum.Approving;
                    //如果当前自动跳转或当前为子流程父流程下一审批节点存在并且所有子流程均结束
                    #region 获取自动跳转待办集合
                    Bpm_WorkFlow tempWorkflow = workflow;
                    Bpm_WorkFlowInstance tempWorkFlowInst = workFlowInst;
                    bool isParentFlowAuto = false;
                    if (isChildFlowHandle && parentNextNodeInstIds.Count > 0 && isAllChildFlowOver) //当前为子流程并且所有其他子流程均结束
                    {
                        if (workflowStatus == WorkFlowStatusEnum.Over || workflowStatus == WorkFlowStatusEnum.Refused) //当前子流程已结束
                        {
                            string tempMsg = string.Empty;
                            //父流程下一审批结点待办集合
                            nextTodos = CommonOperate.GetEntities<Bpm_WorkToDoList>(out tempMsg, x => x.Bpm_WorkFlowInstanceId == workFlowInst.ParentId.Value && parentNextNodeInstIds.Contains(x.Bpm_WorkNodeInstanceId), null, false);
                            Bpm_WorkFlowInstance parentWorkflowInst = GetWorkflowInstanceById(workFlowInst.ParentId.Value);
                            tempWorkflow = GetWorkflow(parentWorkflowInst.Bpm_WorkFlowId.Value);
                            tempWorkFlowInst = parentWorkflowInst;
                            autoJumpNode = parentNextNode;
                            List<Bpm_WorkNode> tempNextNodes = GetNextWorkNodes(tempWorkflow.Id, autoJumpNode.Id);
                            if (tempNextNodes.Count == 0 && workflowStatus == WorkFlowStatusEnum.Refused) //父流程当前自动审批结点为最后一个结点时，判断子流程只要有一个子流程审批通过则该父流程结点为同意，否则为拒绝
                            {
                                List<object> statusObjs = CommonOperate.GetColumnFieldValues<Bpm_WorkFlowInstance>(out tempMsg, "Status", x => x.ParentId == parentWorkflowInst.Id); //获取所有子流程状态
                                if (statusObjs.Count > 0)
                                {
                                    int statusOver = (int)WorkFlowStatusEnum.Over;
                                    if (statusObjs.Select(x => x.ObjToInt()).ToList().Contains(statusOver))
                                        autoWorkAction = WorkActionEnum.Approving;
                                    else
                                        autoWorkAction = WorkActionEnum.Refusing;
                                }
                            }
                            autoJumpNodeInst = GetWorkNodeInstanceById(parentNextNodeInstIds.FirstOrDefault().Value);
                            isParentFlowAuto = true;
                            //自动跳转规则
                            string autoJumpRule = autoJumpNode.AutoJumpRule.ObjToStr();
                            string[] autoJumpToken = autoJumpRule.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (autoJumpToken == null || autoJumpToken.Length != 5)
                                autoJumpToken = new string[] { "1", "1", "1", "0", "0" };
                            notFindNextHandlerIsAuto = autoJumpToken[3] == "1";
                            handlerIsStarterAuto = autoJumpToken[0] == "1";
                            handlerIsAppearedAuto = autoJumpToken[1] == "1";
                            handlerIsLastEmpAuto = workAction == WorkActionEnum.Approving && autoJumpToken[2] == "1";
                        }
                        else
                        {
                            nextTodos = new List<Bpm_WorkToDoList>();
                        }
                    }
                    List<Bpm_WorkToDoList> autoJumpTodos = new List<Bpm_WorkToDoList>(); //自动跳转待办
                    int returnBackAction = (int)WorkActionEnum.Returning;
                    foreach (Bpm_WorkToDoList nextTodo in nextTodos)
                    {
                        if (tempWorkFlowInst.OrgM_EmpId.HasValue && nextTodo.OrgM_EmpId.HasValue && nextTodo.OrgM_EmpId != Guid.Empty)
                        {
                            if (!isParentFlowAuto)
                            {
                                //下一处理人在下一处理结点有过回退处理则不能自动处理
                                string tempMsg = string.Empty;
                                List<Bpm_WorkToDoList> returnTodos = CommonOperate.GetEntities<Bpm_WorkToDoList>(out tempMsg, x => x.Bpm_WorkFlowInstanceId == tempWorkFlowInst.Id && x.WorkAction == returnBackAction && x.OrgM_EmpId == nextTodo.OrgM_EmpId.Value && !x.IsDeleted, null, false);
                                if (returnTodos != null && returnTodos.Count > 0)
                                {
                                    bool isContnue = false;
                                    foreach (Bpm_WorkToDoList tempReturnTodo in returnTodos)
                                    {
                                        Guid tempNodeId = GetWorkNodeIdByTodoId(tempReturnTodo.Id);
                                        if (tempNodeId == autoJumpNode.Id)
                                        {
                                            isContnue = true;
                                            break;
                                        }
                                    }
                                    if (isContnue)
                                        continue;
                                }
                            }
                            Bpm_WorkToDoList tempTodo = null;
                            if (handlerIsStarterAuto)
                            {
                                if (tempWorkFlowInst.OrgM_EmpId == nextTodo.OrgM_EmpId.Value)
                                    tempTodo = nextTodo;
                            }
                            if (tempTodo == null && handlerIsLastEmpAuto)
                            {
                                Bpm_WorkNode currNode = GetWorkNode(GetWorkNodeIdByTodoId(nextTodo.Id));
                                List<Guid> lastEmpIds = GetPreNodeHandler(currNode, tempWorkFlowInst);
                                if (lastEmpIds.Contains(nextTodo.OrgM_EmpId.Value))
                                    autoJumpTodos.Add(nextTodo);
                            }
                            if (tempTodo == null && handlerIsAppearedAuto)
                            {
                                int wa = (int)WorkActionEnum.NoAction;
                                long count = CommonOperate.Count<Bpm_WorkToDoList>(out errMsg, false, x => x.Bpm_WorkFlowInstanceId == tempWorkFlowInst.Id && x.OrgM_EmpId == nextTodo.OrgM_EmpId.Value && x.WorkAction != wa);
                                if (count > 0)
                                    autoJumpTodos.Add(nextTodo);
                            }
                            if (tempTodo != null)
                            {
                                autoJumpTodos.Add(tempTodo);
                            }
                        }
                        else
                        {
                            autoJumpTodos.Add(nextTodo);
                        }
                    }
                    #endregion
                    #region 自动处理待办
                    if (autoJumpTodos.Count > 0)
                    {
                        foreach (Bpm_WorkToDoList todo in autoJumpTodos)
                        {
                            UserInfo userInfo = null;
                            if (todo.OrgM_EmpId.Value != Guid.Empty)
                            {
                                Guid userId = OrgMOperate.GetUserIdByEmpId(todo.OrgM_EmpId.Value);
                                userInfo = UserOperate.GetUserInfo(userId);
                            }
                            else
                            {
                                userInfo = UserOperate.GetSuperAdmin();
                            }
                            HandleProcess(tempWorkflow, tempWorkFlowInst, autoJumpNode, autoJumpNodeInst, userInfo, autoWorkAction, "系统自动审批", todo, null, null, todo.ParentId);
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else //流程操作失败
            {
                //异步添加流程操作失败日志
                Task.Factory.StartNew(() =>
                {
                    LogOperate.AddOperateLog(currUser, workflow.Sys_ModuleId.Value, errPrex, string.Format("【{0}】失败，流程【{1}】，待办【{2}】，处理人【{3}】", errPrex, workflow.Name, workTodo != null ? workTodo.Id.ToString() : string.Empty, currUser.UserName), false, errMsg);
                });
            }
            return errMsg;
        }

        #endregion

        #region 审批操作附属

        /// <summary>
        /// 审批完成后的数据迁移到待办历史数据表中
        /// </summary>
        /// <param name="workFlowInst">流程已结束的流程实例对象</param>
        /// <param name="currUser">当前用户</param>
        private static void TransferWorkToDoHistory(Bpm_WorkFlowInstance workFlowInst, UserInfo currUser)
        {
            if (workFlowInst == null) return;
            //异步处理
            Task.Factory.StartNew(() =>
            {
                string errMsg = string.Empty;
                #region 迁移待办任务数据
                DbLinkArgs currDbLink = ModelConfigHelper.GetDbLinkArgs(typeof(Bpm_WorkToDoListHistory));
                string tableName = ModelConfigHelper.GetModuleTableName(typeof(Bpm_WorkToDoList), currDbLink);
                string sql = string.Format("SELECT * FROM {0} WHERE Bpm_WorkFlowInstanceId='{1}'", tableName, workFlowInst.Id);
                List<Bpm_WorkToDoListHistory> worktodoHistorys = CommonOperate.GetEntitiesBySql<Bpm_WorkToDoListHistory>(out errMsg, sql);
                if (worktodoHistorys != null && worktodoHistorys.Count > 0)
                {
                    bool rs = CommonOperate.OperateRecords<Bpm_WorkToDoListHistory>(worktodoHistorys, ModelRecordOperateType.Add, out errMsg, false);
                    if (rs)
                    {
                        rs = CommonOperate.DeleteRecordsByExpression<Bpm_WorkToDoList>(x => x.Bpm_WorkFlowInstanceId == workFlowInst.Id, out errMsg, false);
                        if (!rs)
                        {
                            errMsg = string.Format("流程结束后的待办迁移删除失败，流程实例ID【{0}】，异常信息：{1}", workFlowInst.Id, errMsg);
                            LogOperate.AddOperateLog(currUser, "待办任务", "流程结束后迁移待办", string.Format("流程实例ID【{0}】", workFlowInst.Id), false, errMsg);
                        }
                    }
                    else
                    {
                        errMsg = string.Format("流程结束后写入待办历史数据失败，流程实例ID【{0}】，异常信息：{1}", workFlowInst.Id, errMsg);
                        LogOperate.AddOperateLog(currUser, "待办任务", "流程结束后迁移待办", string.Format("流程实例ID【{0}】", workFlowInst.Id), false, errMsg);
                    }
                }
                #endregion
                #region 迁移结点实例数据
                currDbLink = ModelConfigHelper.GetDbLinkArgs(typeof(Bpm_WorkNodeInstanceHistory));
                string tableNameNode = ModelConfigHelper.GetModuleTableName(typeof(Bpm_WorkNodeInstance), currDbLink);
                string sqlNode = string.Format("SELECT * FROM {0} WHERE Bpm_WorkFlowInstanceId='{1}'", tableNameNode, workFlowInst.Id);
                List<Bpm_WorkNodeInstanceHistory> workNodeHistorys = CommonOperate.GetEntitiesBySql<Bpm_WorkNodeInstanceHistory>(out errMsg, sqlNode);
                if (workNodeHistorys != null && workNodeHistorys.Count > 0)
                {
                    bool rs = CommonOperate.OperateRecords<Bpm_WorkNodeInstanceHistory>(workNodeHistorys, ModelRecordOperateType.Add, out errMsg, false);
                    if (rs)
                    {
                        rs = CommonOperate.DeleteRecordsByExpression<Bpm_WorkNodeInstance>(x => x.Bpm_WorkFlowInstanceId == workFlowInst.Id, out errMsg, false);
                        if (!rs)
                        {
                            errMsg = string.Format("流程结束后的流程结点实例删除失败，流程实例ID【{0}】，异常信息：{1}", workFlowInst.Id, errMsg);
                            LogOperate.AddOperateLog(currUser, "结点实例", "流程结束后迁移流程结点实例", string.Format("流程实例ID【{0}】", workFlowInst.Id), false, errMsg);
                        }
                    }
                    else
                    {
                        errMsg = string.Format("流程结束后写入流程实例历史数据失败，流程实例ID【{0}】，异常信息：{1}", workFlowInst.Id, errMsg);
                        LogOperate.AddOperateLog(currUser, "结点实例", "流程结束后迁移流程结点实例", string.Format("流程实例ID【{0}】", workFlowInst.Id), false, errMsg);
                    }
                }
                #endregion
                #region 迁移流程实例数据
                Bpm_WorkFlowInstanceHistory workFlowInstHistory = new Bpm_WorkFlowInstanceHistory();
                ObjectHelper.CopyValue(workFlowInst, workFlowInstHistory);
                Guid result = CommonOperate.OperateRecord<Bpm_WorkFlowInstanceHistory>(workFlowInstHistory, ModelRecordOperateType.Add, out errMsg, null, false);
                if (result != Guid.Empty)
                {
                    bool rs = CommonOperate.DeleteRecordsByExpression<Bpm_WorkFlowInstance>(x => x.Id == workFlowInst.Id, out errMsg, false);
                    if (!rs)
                    {
                        errMsg = string.Format("流程结束后的流程实例删除失败，流程实例ID【{0}】，异常信息：{1}", workFlowInst.Id, errMsg);
                        LogOperate.AddOperateLog(currUser, "流程实例", "流程结束后迁移流程实例", string.Format("流程实例ID【{0}】", workFlowInst.Id), false, errMsg);
                    }
                }
                else
                {
                    errMsg = string.Format("流程结束后写入流程实例历史数据失败，流程实例ID【{0}】，异常信息：{1}", workFlowInst.Id, errMsg);
                    LogOperate.AddOperateLog(currUser, "流程实例", "流程结束后迁移流程实例", string.Format("流程实例ID【{0}】", workFlowInst.Id), false, errMsg);
                }
                #endregion
            });
        }

        /// <summary>
        /// 根据操作动作获取流程状态
        /// </summary>
        /// <param name="workAction">操作动作</param>
        /// <param name="workflowStatus">流程状态</param>
        /// <param name="errPrex">异常前缀</param>
        private static void GetFlowStatusAndErrPrex(WorkActionEnum workAction, out WorkFlowStatusEnum workflowStatus, out string errPrex)
        {
            workflowStatus = WorkFlowStatusEnum.NoStatus;
            errPrex = string.Empty;
            switch (workAction)
            {
                case WorkActionEnum.Starting:
                case WorkActionEnum.ReStarting:
                    workflowStatus = WorkFlowStatusEnum.Start;
                    errPrex = "【流程发起】";
                    break;
                case WorkActionEnum.Approving:
                case WorkActionEnum.Directing:
                    workflowStatus = WorkFlowStatusEnum.Approving;
                    errPrex = "【流程审批】";
                    break;
                case WorkActionEnum.Returning:
                    workflowStatus = WorkFlowStatusEnum.Return;
                    errPrex = "【流程回退】";
                    break;
                case WorkActionEnum.Refusing:
                    workflowStatus = WorkFlowStatusEnum.Refused;
                    errPrex = "【流程拒绝】";
                    break;
            }
        }

        /// <summary>
        /// 获取流程异常前缀
        /// </summary>
        /// <param name="workAction">操作动作</param>
        /// <returns></returns>
        private static string GetFlowErrPrex(WorkActionEnum workAction)
        {
            switch (workAction)
            {
                case WorkActionEnum.Starting:
                case WorkActionEnum.ReStarting:
                    return "【流程发起】";
                case WorkActionEnum.SubStarting:
                    return "【子流程发起】";
                case WorkActionEnum.Approving:
                    return "【流程审批】";
                case WorkActionEnum.Directing:
                    return "【流程指派】";
                case WorkActionEnum.Returning:
                    return "【流程回退】";
                case WorkActionEnum.Refusing:
                    return "【流程拒绝】";
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取流程操作类型描述
        /// </summary>
        /// <param name="workAction"></param>
        /// <returns></returns>
        public static string GetFlowOpTypeDes(WorkActionEnum workAction)
        {
            switch (workAction)
            {
                case WorkActionEnum.Starting:
                case WorkActionEnum.ReStarting:
                    return "发起流程";
                case WorkActionEnum.Approving:
                    return "审批流程";
                case WorkActionEnum.Directing:
                    return "指派流程";
                case WorkActionEnum.Returning:
                    return "回退流程";
                case WorkActionEnum.Refusing:
                    return "拒绝流程";
                case WorkActionEnum.SubStarting:
                    return "发起子流程";
                case WorkActionEnum.Communicating:
                    return "沟通流程";
            }
            return string.Empty;
        }

        /// <summary>
        /// 是否启用工作流
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public static bool IsEnabledWorkflow(Guid moduleId)
        {
            Bpm_WorkFlow workflow = GetAllWorkflows(x => x.Sys_ModuleId == moduleId).FirstOrDefault();
            return workflow != null;
        }

        #endregion

        #region 流程状态处理

        /// <summary>
        /// 异步更新单据流程状态
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <param name="status">流程状态</param>
        /// <param name="currUser">当前用户</param>
        public static void AsyncUpdateRecordFlowStatus(Guid moduleId, Guid id, WorkFlowStatusEnum status, UserInfo currUser = null)
        {
            //异步处理
            Task.Factory.StartNew(() =>
            {
                string errMsg = string.Empty;
                bool rs = CommonOperate.UpdateField(moduleId, id, "FlowStatus", ((int)status).ToString(), out errMsg, null, currUser);
                if (!rs)
                {
                    string moduleName = SystemOperate.GetModuleNameById(moduleId);
                    LogOperate.AddOperateLog(currUser, moduleId, "更新单据流程状态", string.Format("更新模块【{0}】单据流程状态失败，记录ID【{1}】，流程状态【{2}】", moduleName, id, (int)status), false, errMsg);
                }
            });
        }

        /// <summary>
        /// 获取单据的流程状态，从流程实例中取
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public static WorkFlowStatusEnum GetRecordFlowStatus(Guid moduleId, Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                Bpm_WorkFlowInstance workflowInst = GetWorkflowInstance(moduleId, id.Value);
                if (workflowInst == null)
                {
                    string errMsg = string.Empty;
                    List<Guid?> workflowIds = GetAllWorkflows(x => x.Sys_ModuleId == moduleId).Select(x => (Guid?)x.Id).ToList();
                    Bpm_WorkFlowInstanceHistory workflowInstHistory = workflowIds.Count > 0 ? CommonOperate.GetEntity<Bpm_WorkFlowInstanceHistory>(x => workflowIds.Contains(x.Bpm_WorkFlowId) && x.RecordId == id.Value, null, out errMsg) :
                                                                     CommonOperate.GetEntity<Bpm_WorkFlowInstanceHistory>(x => x.Bpm_WorkFlowId == workflowIds.FirstOrDefault().Value && x.RecordId == id.Value, null, out errMsg);
                    if (workflowInstHistory != null)
                    {
                        workflowInst = new Bpm_WorkFlowInstance();
                        ObjectHelper.CopyValue(workflowInstHistory, workflowInst);
                    }
                }
                if (workflowInst != null)
                    return workflowInst.StatusOfEnum;
            }
            return WorkFlowStatusEnum.NoStatus;
        }

        /// <summary>
        /// 获取单据流程状态，从单据上取
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public static WorkFlowStatusEnum GetRecordStatus(Guid moduleId, Guid id)
        {
            string tempMsg = string.Empty;
            object model = CommonOperate.GetEntityById(moduleId, id, out tempMsg, new List<string>() { "FlowStatus" });
            int? recordStatus = CommonOperate.GetModelFieldValueByModel(moduleId, model, "FlowStatus").ObjToIntNull();
            if (!recordStatus.HasValue)
                return WorkFlowStatusEnum.NoStatus;
            return (WorkFlowStatusEnum)Enum.Parse(typeof(WorkFlowStatusEnum), recordStatus.Value.ToString());
        }

        #endregion

        #region 获取审批信息

        /// <summary>
        /// 获取记录最新的审批信息
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public static ApprovalInfo GetRecordLastApprovalInfo(Guid moduleId, Guid id)
        {
            ApprovalInfo approvalInfo = GetModuleRecordApprovalInfos(moduleId, id, true).FirstOrDefault();
            return approvalInfo;
        }

        /// <summary>
        /// 获取记录的审批信息
        /// </summary>
        /// <param name="workFlowInst">流程实例对象</param>
        /// <param name="onlyGetLastOne">只取最新的审批信息</param>
        /// <returns></returns>
        public static List<ApprovalInfo> GetRecordApprovalInfos(Bpm_WorkFlowInstance workFlowInst, bool onlyGetLastOne = false)
        {
            List<ApprovalInfo> approvalInfos = new List<ApprovalInfo>();
            if (workFlowInst == null) return approvalInfos;
            string errMsg = string.Empty;
            //是否从历史记录中取审批信息
            bool getFromHistory = workFlowInst.StatusOfEnum == WorkFlowStatusEnum.Over || workFlowInst.StatusOfEnum == WorkFlowStatusEnum.Refused;
            List<Bpm_WorkToDoList> todoList = null;
            if (getFromHistory) //从历史表中取
            {
                todoList = CommonOperate.GetEntitiesBySql<Bpm_WorkToDoList>(out errMsg, string.Format("SELECT * FROM Bpm_WorkToDoListHistory WHERE Bpm_WorkFlowInstanceId='{0}'", workFlowInst.Id));
                if (todoList != null)
                {
                    if (onlyGetLastOne) //取一条的时候按倒序排列
                        todoList = todoList.OrderByDescending(x => x.FinishDate).ToList();
                    else
                        todoList = todoList.OrderBy(x => x.FinishDate).ToList();
                }
            }
            else //从当前待办中取
            {
                List<bool> descs = onlyGetLastOne ? new List<bool>() { true } : new List<bool>() { false };
                todoList = CommonOperate.GetEntities<Bpm_WorkToDoList>(out errMsg, x => x.Bpm_WorkFlowInstanceId == workFlowInst.Id && !x.IsDeleted, null, false, new List<string>() { "FinishDate" }, descs);
            }
            if (todoList != null && todoList.Count > 0)
            {
                foreach (Bpm_WorkToDoList todo in todoList)
                {
                    if (!todo.Bpm_WorkNodeInstanceId.HasValue)
                        continue;
                    Bpm_WorkNodeInstance nodeInst = null;
                    if (getFromHistory) //从历史数据中取
                    {
                        Bpm_WorkNodeInstanceHistory nodeHistoryInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstanceHistory>(todo.Bpm_WorkNodeInstanceId.Value, out errMsg);
                        if (nodeHistoryInst != null)
                        {
                            nodeInst = new Bpm_WorkNodeInstance();
                            ObjectHelper.CopyValue(nodeHistoryInst, nodeInst);
                        }
                    }
                    else
                    {
                        nodeInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstance>(todo.Bpm_WorkNodeInstanceId.Value, out errMsg);
                    }
                    if (nodeInst == null || !nodeInst.Bpm_WorkNodeId.HasValue)
                        continue;
                    Bpm_WorkNode currNode = GetWorkNode(nodeInst.Bpm_WorkNodeId.Value);
                    if (currNode == null || !currNode.Bpm_WorkFlowId.HasValue) continue;
                    Bpm_WorkFlow currFlow = GetWorkflow(currNode.Bpm_WorkFlowId.Value);
                    if (currFlow == null) continue;
                    if (todo.WorkActionOfEnum != WorkActionEnum.Directing) //非指派
                    {
                        if (currNode.HandleStrategyOfEnum == HandleStrategyTypeEnum.AllAgree)
                        {
                            if (todo.WorkActionOfEnum == WorkActionEnum.NoAction)
                                continue;
                        }
                        else
                        {
                            if (nodeInst.StatusOfEnum == WorkNodeStatusEnum.Undo)
                                continue;
                        }
                    }
                    string empIdStr = string.Empty; //处理人
                    if (todo.IsInitHandler)
                    {
                        if (todo.OrgM_EmpId.HasValue && todo.OrgM_EmpId.Value != Guid.Empty)
                        {
                            OrgM_Emp tempEmp = OrgMOperate.GetEmp(todo.OrgM_EmpId.Value);
                            if (tempEmp != null)
                                empIdStr = tempEmp.Name;
                        }
                        else if (todo.OrgM_EmpId == Guid.Empty)
                        {
                            empIdStr = "系统管理员";
                        }
                    }
                    else
                    {
                        if (todo.OrgM_EmpId.HasValue && todo.OrgM_EmpId.Value == Guid.Empty)
                        {
                            empIdStr = "系统管理员";
                        }
                        else if (todo.OrgM_EmpId.HasValue)
                        {
                            OrgM_Emp tempEmp = OrgMOperate.GetEmp(todo.OrgM_EmpId.Value);
                            if (tempEmp != null)
                                empIdStr = tempEmp.Name;
                        }
                    }
                    string handleTime = todo.FinishDate.HasValue ? todo.FinishDate.Value.ToString() : string.Empty;
                    string handleResult = string.Empty;
                    if (todo.WorkActionOfEnum != WorkActionEnum.Starting &&
                        todo.WorkActionOfEnum != WorkActionEnum.ReStarting &&
                        todo.WorkActionOfEnum != WorkActionEnum.SubStarting)
                    {
                        int btnType = (int)FlowButtonTypeEnum.AgreeBtn;
                        switch (todo.WorkActionOfEnum)
                        {
                            case WorkActionEnum.Returning:
                                btnType = (int)FlowButtonTypeEnum.BackBtn;
                                break;
                            case WorkActionEnum.Refusing:
                                btnType = (int)FlowButtonTypeEnum.RejectBtn;
                                break;
                            case WorkActionEnum.Directing:
                                btnType = (int)FlowButtonTypeEnum.AssignBtn;
                                break;
                        }
                        Bpm_FlowBtn flowBtn = GetAllWorkButtons(x => x.ButtonType == btnType).FirstOrDefault();
                        if (flowBtn != null)
                        {
                            Bpm_NodeBtnConfig btnConfig = GetAllApprovalBtnConfigs(x => x.Bpm_FlowBtnId == flowBtn.Id && x.Bpm_WorkFlowId == currNode.Bpm_WorkFlowId.Value && x.Bpm_WorkNodeId == currNode.Id).FirstOrDefault();
                            if (btnConfig != null)
                                handleResult = btnConfig.BtnDisplay;
                            else
                                handleResult = flowBtn.ButtonText;
                        }
                    }
                    else
                    {
                        handleResult = "发起";
                    }
                    string nextHandlerStr = string.Empty;
                    if (!string.IsNullOrEmpty(todo.NextNodeHandler))
                    {
                        List<Guid> tempEmpIds = todo.NextNodeHandler.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.ObjToGuid()).ToList();
                        if (currNode.HandleStrategyOfEnum == HandleStrategyTypeEnum.AllAgree)
                        {
                            List<Guid> hasHandler = todoList.Where(x => x.Bpm_WorkNodeInstanceId == nodeInst.Id && x.OrgM_EmpId.HasValue && x.WorkActionOfEnum != WorkActionEnum.NoAction).Select(x => x.OrgM_EmpId.Value).ToList();
                            tempEmpIds = tempEmpIds.Where(x => !hasHandler.Contains(x)).ToList();
                        }
                        if (tempEmpIds.Count > 0)
                        {
                            foreach (Guid tempEmpId in tempEmpIds)
                            {
                                string empName = string.Empty;
                                if (tempEmpId != Guid.Empty)
                                {
                                    OrgM_Emp tempEmp = OrgMOperate.GetEmp(tempEmpId);
                                    if (tempEmp == null) continue;
                                    empName = tempEmp.Name;
                                    if (currFlow.OrgM_EmpId.HasValue && currFlow.OrgM_EmpId.Value == tempEmpId)
                                    {
                                        empName += "(找不到处理人转由流程管理人处理)";
                                    }
                                }
                                else
                                {
                                    empName = "系统管理员(找不到处理人转由系统管理员处理)";
                                }
                                if (nextHandlerStr != string.Empty)
                                    nextHandlerStr += ",";
                                nextHandlerStr += empName;
                            }
                        }
                    }
                    Bpm_WorkNode nextNode = todo.Bpm_WorkNodeId.HasValue ? GetWorkNode(todo.Bpm_WorkNodeId.Value) : null;
                    ApprovalInfo item = new ApprovalInfo()
                    {
                        NodeId = currNode.Id,
                        NodeName = string.IsNullOrEmpty(currNode.DisplayName) ? currNode.Name : currNode.DisplayName,
                        Handler = empIdStr,
                        HandleTime = handleTime,
                        HandleResult = handleResult,
                        ApprovalOpinions = todo.ApprovalOpinions,
                        NextNodeName = nextNode != null ? (string.IsNullOrEmpty(nextNode.DisplayName) ? nextNode.Name : nextNode.DisplayName) : string.Empty,
                        NextName = nextNode != null ? nextNode.Name : string.Empty,
                        NextHandler = nextHandlerStr
                    };
                    approvalInfos.Add(item);
                    if (onlyGetLastOne) //只取最新一条
                        return approvalInfos;
                }
            }
            return approvalInfos;
        }

        /// <summary>
        /// 获取记录的审批信息
        /// </summary>
        /// <param name="workflowInstId">流程实例ID</param>
        /// <param name="onlyGetLastOne">只取最新的审批信息</param>
        /// <returns></returns>
        public static List<ApprovalInfo> GetRecordApprovalInfos(Guid workflowInstId, bool onlyGetLastOne = false)
        {
            string errMsg = string.Empty;
            Bpm_WorkFlowInstance workFlowInst = GetWorkflowInstanceById(workflowInstId);
            if (workFlowInst == null) //从历史表中取
            {
                Bpm_WorkFlowInstanceHistory workflowInstHistory = CommonOperate.GetEntityById<Bpm_WorkFlowInstanceHistory>(workflowInstId, out errMsg);
                if (workflowInstHistory != null)
                {
                    workFlowInst = new Bpm_WorkFlowInstance();
                    ObjectHelper.CopyValue(workflowInstHistory, workFlowInst);
                }
            }
            return GetRecordApprovalInfos(workFlowInst, onlyGetLastOne);
        }

        /// <summary>
        /// 根据待办任务ID获取审批信息
        /// </summary>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <returns></returns>
        public static List<ApprovalInfo> GetRecordApprovalInfosByTodoId(Guid todoTaskId)
        {
            string errMsg = string.Empty;
            Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(todoTaskId, out errMsg);
            if (todo != null && todo.Bpm_WorkFlowInstanceId.HasValue)
            {
                Bpm_WorkFlowInstance workFlowInst = CommonOperate.GetEntityById<Bpm_WorkFlowInstance>(todo.Bpm_WorkFlowInstanceId.Value, out errMsg);
                if (workFlowInst == null) //从历史表中取
                {
                    Bpm_WorkFlowInstanceHistory workflowInstHistory = CommonOperate.GetEntityById<Bpm_WorkFlowInstanceHistory>(todo.Bpm_WorkFlowInstanceId.Value, out errMsg);
                    if (workflowInstHistory != null)
                    {
                        workFlowInst = new Bpm_WorkFlowInstance();
                        ObjectHelper.CopyValue(workflowInstHistory, workFlowInst);
                    }
                }
                if (workFlowInst != null)
                    return GetRecordApprovalInfos(workFlowInst);
            }
            return new List<ApprovalInfo>();
        }

        /// <summary>
        /// 根据模块记录ID获取审批信息
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <param name="onlyGetLastOne">是否只取最新一条</param>
        /// <returns></returns>
        public static List<ApprovalInfo> GetModuleRecordApprovalInfos(Guid moduleId, Guid id, bool onlyGetLastOne = false)
        {
            Bpm_WorkFlowInstance workflowInst = GetWorkflowInstance(moduleId, id);
            if (workflowInst == null)
            {
                string errMsg = string.Empty;
                List<Guid?> workflowIds = GetAllWorkflows(x => x.Sys_ModuleId == moduleId).Select(x => (Guid?)x.Id).ToList();
                Bpm_WorkFlowInstanceHistory workflowInstHistory = workflowIds.Count > 0 ? CommonOperate.GetEntity<Bpm_WorkFlowInstanceHistory>(x => workflowIds.Contains(x.Bpm_WorkFlowId) && x.RecordId == id, null, out errMsg) :
                                                                 CommonOperate.GetEntity<Bpm_WorkFlowInstanceHistory>(x => x.Bpm_WorkFlowId == workflowIds.FirstOrDefault().Value && x.RecordId == id, null, out errMsg);
                if (workflowInstHistory != null)
                {
                    workflowInst = new Bpm_WorkFlowInstance();
                    ObjectHelper.CopyValue(workflowInstHistory, workflowInst);
                }
            }
            if (workflowInst != null)
                return GetRecordApprovalInfos(workflowInst, onlyGetLastOne);
            return new List<ApprovalInfo>();
        }

        #endregion

        #region 结点表单处理

        /// <summary>
        /// 获取当前审批节点
        /// </summary>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <returns></returns>
        public static Bpm_WorkNode GetCurrentApprovalNode(Guid todoTaskId)
        {
            string errMsg = string.Empty;
            Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(todoTaskId, out errMsg);
            if (todo != null && todo.Bpm_WorkNodeInstanceId.HasValue)
            {
                Bpm_WorkNodeInstance workNodeInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstance>(todo.Bpm_WorkNodeInstanceId.Value, out errMsg);
                if (workNodeInst != null && workNodeInst.Bpm_WorkNodeId.HasValue)
                    return GetWorkNode(workNodeInst.Bpm_WorkNodeId.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取流程结点表单
        /// </summary>
        /// <param name="workNodeId">流程结点ID</param>
        /// <returns></returns>
        public static Sys_Form GetWorkNodeForm(Guid workNodeId)
        {
            Bpm_WorkNode workNode = GetWorkNode(workNodeId);
            if (workNode != null && workNode.Sys_FormId.HasValue)
                return SystemOperate.GetForm(workNode.Sys_FormId.Value);
            return null;
        }

        /// <summary>
        /// 获取自定义流程表单URL
        /// </summary>
        /// <param name="workNodeId">流程结点ID</param>
        /// <returns></returns>
        public static string GetCustomerWorkNodeFormUrl(Guid workNodeId)
        {
            Bpm_WorkNode workNode = GetWorkNode(workNodeId);
            if (workNode != null && workNode.Sys_FormId.HasValue)
                return workNode.FormUrl;
            return null;
        }

        /// <summary>
        /// 获取发起结点表单
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public static Sys_Form GetLaunchNodeForm(Guid moduleId)
        {
            Bpm_WorkFlow workflow = GetModuleWorkFlow(moduleId);
            if (workflow != null)
            {
                Bpm_WorkNode launchNode = GetLaunchNode(workflow.Id);
                if (launchNode != null)
                    return GetWorkNodeForm(launchNode.Id);
            }
            return null;
        }

        /// <summary>
        /// 获取发起结点自定义表单
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public static string GetLaunchNodeCustomerFormUrl(Guid moduleId)
        {
            Bpm_WorkFlow workflow = GetModuleWorkFlow(moduleId);
            if (workflow != null)
            {
                Bpm_WorkNode launchNode = GetLaunchNode(workflow.Id);
                if (launchNode != null)
                    return GetCustomerWorkNodeFormUrl(launchNode.Id);
            }
            return null;
        }

        /// <summary>
        /// 根据流程待办任务获取流程结点ID
        /// </summary>
        /// <param name="workTodoId">待办任务ID</param>
        /// <returns></returns>
        public static Guid GetWorkNodeIdByTodoId(Guid workTodoId)
        {
            string errMsg = string.Empty;
            Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(workTodoId, out errMsg);
            if (todo != null && todo.Bpm_WorkNodeInstanceId.HasValue)
            {
                Bpm_WorkNodeInstance workNodeInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstance>(todo.Bpm_WorkNodeInstanceId.Value, out errMsg);
                if (workNodeInst != null && workNodeInst.Bpm_WorkNodeId.HasValue)
                    return workNodeInst.Bpm_WorkNodeId.Value;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 根据待办ID获取模块ID和记录ID
        /// </summary>
        /// <param name="workTodoId">待办ID</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public static void GetModuleIdAndRecordIdByTodoId(Guid workTodoId, out Guid moduleId, out Guid recordId)
        {
            moduleId = Guid.Empty;
            recordId = Guid.Empty;
            string errMsg = string.Empty;
            Bpm_WorkToDoList workTodo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(workTodoId, out errMsg);
            if (workTodo != null)
            {
                moduleId = workTodo.ModuleId;
                recordId = workTodo.RecordId;
            }
        }

        #endregion

        #region 处理人判断

        /// <summary>
        /// 是否是当前待办任务的处理者
        /// </summary>
        /// <param name="todoTaskId">当前待办任务ID</param>
        /// <param name="currUser">用户信息</param>
        /// <returns></returns>
        public static bool IsCurrentToDoTaskHandler(Guid todoTaskId, UserInfo currUser)
        {
            if (currUser == null) return false;
            //if (currUser.UserName == "admin") return true;
            if (!currUser.EmpId.HasValue) return false;
            string errMsg = string.Empty;
            bool isParentTodo = false;
            Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(todoTaskId, out errMsg);
            if (todo == null)
            {
                Bpm_WorkToDoListHistory todoHistory = CommonOperate.GetEntityById<Bpm_WorkToDoListHistory>(todoTaskId, out errMsg);
                if (todoHistory != null)
                    isParentTodo = todoHistory.IsParentTodo == true;
            }
            else
            {
                isParentTodo = todo.IsParentTodo == true;
            }
            if (!isParentTodo) //非父待办
            {
                if (todo == null) return false;
                if (todo.WorkActionOfEnum == WorkActionEnum.NoAction && (todo.StatusOfEnum == WorkFlowStatusEnum.Start || todo.StatusOfEnum == WorkFlowStatusEnum.Approving || todo.StatusOfEnum == WorkFlowStatusEnum.Return))
                {
                    if (todo.Bpm_WorkNodeInstanceId.HasValue && todo.Bpm_WorkNodeInstanceId.Value != Guid.Empty)
                    {
                        Bpm_WorkNodeInstance workNodeInst = CommonOperate.GetEntityById<Bpm_WorkNodeInstance>(todo.Bpm_WorkNodeInstanceId.Value, out errMsg);
                        if (workNodeInst != null && workNodeInst.StatusOfEnum == WorkNodeStatusEnum.Undo)
                        {
                            return currUser.EmpId.HasValue && todo.OrgM_EmpId.HasValue && todo.OrgM_EmpId.Value == currUser.EmpId.Value;
                        }
                    }
                }
                return false;
            }
            else //当前为父待办，子流程
            {
                int noAction = (int)WorkActionEnum.NoAction;
                long count = CommonOperate.Count<Bpm_WorkToDoList>(out errMsg, false, x => x.ParentId == todoTaskId && x.OrgM_EmpId == currUser.EmpId.Value && x.WorkAction == noAction && x.IsDeleted == false);
                return count > 0;
            }
        }

        /// <summary>
        /// 是否允许发起流程
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public static bool IsAllowLaunchFlow(Guid moduleId, UserInfo currUser, Guid? id)
        {
            if (currUser == null) return false;
            if (currUser.UserName == "admin") return true;
            if (!currUser.EmpId.HasValue || currUser.EmpId.Value == Guid.Empty)
                return false;
            Guid empId = currUser.EmpId.Value;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                Bpm_WorkFlowInstance workflowInst = GetWorkflowInstance(moduleId, id.Value);
                if (workflowInst != null || (workflowInst != null && workflowInst.StatusOfEnum != WorkFlowStatusEnum.NoStatus))
                    return false;
                string errMsg = string.Empty;
                List<int> statusList = new List<int>() { (int)WorkFlowStatusEnum.Start, (int)WorkFlowStatusEnum.Return, (int)WorkFlowStatusEnum.Refused, (int)WorkFlowStatusEnum.Over, (int)WorkFlowStatusEnum.Freezed, (int)WorkFlowStatusEnum.Freezed, (int)WorkFlowStatusEnum.Approving };
                Bpm_WorkToDoList todo = CommonOperate.GetEntity<Bpm_WorkToDoList>(x => x.Bpm_WorkFlowInstanceId == workflowInst.Id && x.OrgM_EmpId == empId && statusList.Contains(x.Status), null, out errMsg);
                if (todo != null) return false;
            }
            List<Guid> empIds = GetLaunchNodeHandler(moduleId);
            return empIds.Contains(currUser.EmpId.Value);
        }

        /// <summary>
        /// 流程是否已发起了
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public static bool IsLaunchFlowCompleted(Guid moduleId, Guid id)
        {
            Bpm_WorkFlowInstance workflowInst = GetWorkflowInstance(moduleId, id);
            return workflowInst != null && workflowInst.StatusOfEnum != WorkFlowStatusEnum.NoStatus;
        }

        /// <summary>
        /// 当前待办是否为子流程的父待办
        /// </summary>
        /// <param name="todoTaskId">待办ID</param>
        /// <returns></returns>
        public static bool IsChildFlowParentTodo(Guid todoTaskId)
        {
            string errMsg = string.Empty;
            Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(todoTaskId, out errMsg);
            if (todo == null)
            {
                Bpm_WorkToDoListHistory todoHistory = CommonOperate.GetEntityById<Bpm_WorkToDoListHistory>(todoTaskId, out errMsg);
                if (todoHistory != null)
                    return todoHistory.IsParentTodo == true;
                else
                    return false;
            }
            return todo.IsParentTodo == true;
        }

        #endregion

        #region 其他

        /// <summary>
        /// 获取当前子流程待办ID
        /// </summary>
        /// <param name="parentTodoId">父待办ID</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="currEmpId">当前人empId</param>
        /// <param name="isAdmin">是否超级管理员</param>
        /// <returns></returns>
        public static Guid GetChildFlowToDoId(Guid parentTodoId, Guid moduleId, Guid recordId, Guid currEmpId, bool isAdmin = false)
        {
            string errMsg = string.Empty;
            int noAction = (int)WorkActionEnum.NoAction;
            Bpm_WorkToDoList childTodo = isAdmin ? CommonOperate.GetEntity<Bpm_WorkToDoList>(x => x.ParentId == parentTodoId && x.ModuleId == moduleId && x.RecordId == recordId && x.WorkAction == noAction, null, out errMsg) :
                                       CommonOperate.GetEntity<Bpm_WorkToDoList>(x => x.ParentId == parentTodoId && x.OrgM_EmpId == currEmpId && x.ModuleId == moduleId && x.RecordId == recordId && x.WorkAction == noAction, null, out errMsg);
            if (childTodo != null)
                return childTodo.Id;
            return Guid.Empty;
        }

        /// <summary>
        /// 获取当前处理人
        /// </summary>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <returns></returns>
        public static OrgM_Emp GetCurrentToDoHandler(Guid todoTaskId)
        {
            string errMsg = string.Empty;
            Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(todoTaskId, out errMsg);
            if (todo != null && todo.OrgM_EmpId.HasValue)
            {
                return OrgMOperate.GetEmp(todo.OrgM_EmpId.Value);
            }
            return null;
        }

        /// <summary>
        /// 移除单据待办，慎用
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">单据记录ID</param>
        /// <param name="defaultFlowStatus">单据还原的状态</param>
        public static void RemoveRecordToDo(Guid moduleId, Guid recordId, int? defaultFlowStatus = null)
        {
            string errMsg = string.Empty;
            int status = defaultFlowStatus.HasValue ? defaultFlowStatus.Value : 0;
            List<object> instIds = CommonOperate.GetColumnFieldValues<Bpm_WorkFlowInstance>(out errMsg, "Id", x => x.RecordId == recordId);
            List<Guid?> flowInstIds = instIds.Select(x => x.ObjToGuidNull()).ToList();
            CommonOperate.DeleteRecordsByExpression<Bpm_WorkToDoList>(x => x.RecordId == recordId && x.ModuleId == moduleId, out errMsg);
            CommonOperate.DeleteRecordsByExpression<Bpm_WorkFlowInstance>(x => x.RecordId == recordId, out errMsg);
            CommonOperate.DeleteRecordsByExpression<Bpm_WorkNodeInstance>(x => flowInstIds.Contains(x.Bpm_WorkFlowInstanceId), out errMsg);
            CommonOperate.UpdateField(moduleId, recordId, "FlowStatus", status.ToString(), out errMsg);
        }

        /// <summary>
        /// 根据待办ID获取发起人
        /// </summary>
        /// <param name="todoTaskId">流程待办ID</param>
        /// <returns></returns>
        public static OrgM_Emp GetFlowLauncher(Guid todoTaskId)
        {
            string errMsg = string.Empty;
            Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(todoTaskId, out errMsg, new List<string>() { "Bpm_WorkFlowInstanceId" });
            if (todo == null)
            {
                string sql = string.Format("SELECT Id,Bpm_WorkFlowInstanceId FROM Bpm_WorkToDoListHistory WHERE Id='{0}'", todoTaskId);
                List<Bpm_WorkToDoList> todos = CommonOperate.GetEntitiesBySql<Bpm_WorkToDoList>(out errMsg, sql);
                if (todos != null && todos.Count > 0)
                    todo = todos.FirstOrDefault();
            }
            if (todo != null)
            {
                if (todo.Bpm_WorkFlowInstanceId.HasValue)
                {
                    Bpm_WorkFlowInstance flowInst = GetWorkflowInstanceById(todo.Bpm_WorkFlowInstanceId.Value);
                    if (flowInst != null && flowInst.OrgM_EmpId.HasValue)
                        return OrgMOperate.GetEmp(flowInst.OrgM_EmpId.Value);
                }
            }
            return null;
        }
        
        /// <summary>
        /// 获取流程发起者
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public static OrgM_Emp GetFlowStarter(Guid recordId)
        {
            string errMsg = string.Empty;
            //先在流程实例表中找
            Bpm_WorkFlowInstance flowInst = CommonOperate.GetEntity<Bpm_WorkFlowInstance>(x => x.RecordId == recordId, null, out errMsg);
            if (flowInst != null)
            {
                if (flowInst.OrgM_EmpId.HasValue)
                    return OrgMOperate.GetEmp(flowInst.OrgM_EmpId.Value);
                return null;
            }
            //在流程实例历史表中找
            Bpm_WorkFlowInstanceHistory flowInstHistory = CommonOperate.GetEntity<Bpm_WorkFlowInstanceHistory>(x => x.RecordId == recordId, null, out errMsg);
            if (flowInstHistory != null && flowInstHistory.OrgM_EmpId.HasValue)
                return OrgMOperate.GetEmp(flowInstHistory.OrgM_EmpId.Value);
            return null;
        }

        #endregion

        #endregion
    }
}

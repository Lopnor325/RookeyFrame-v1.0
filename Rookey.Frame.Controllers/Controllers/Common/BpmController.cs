/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Operate.Base.Extension;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Rookey.Frame.Model.Bpm;
using Rookey.Frame.Controllers.Other;
using System.Web;
using Rookey.Frame.Operate.Base.OperateHandle;
using Rookey.Frame.Common.Model;
using Rookey.Frame.Common.PubDefine;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Base;
using Rookey.Frame.Model.Sys;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using Rookey.Frame.Operate.Base.EnumDef;

namespace Rookey.Frame.Controllers
{
    /// <summary>
    /// 工作流处理控制器
    /// </summary>
    public class BpmController : BaseController
    {
        #region 构造函数

        private HttpRequestBase _Request = null; //请求对象

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public BpmController()
        {
            _Request = Request;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request">请求对象</param>
        public BpmController(HttpRequestBase request)
            : base(request)
        {
            _Request = request;
        }

        #endregion

        #region 页面定义

        /// <summary>
        /// 流程设计页面
        /// </summary>
        /// <returns></returns>
        public ActionResult FlowDesign()
        {
            return View();
        }

        /// <summary>
        /// 流程画布
        /// </summary>
        /// <returns></returns>
        public ActionResult FlowCanvas()
        {
            return View();
        }

        /// <summary>
        /// 流程节点参数设置
        /// </summary>
        /// <returns></returns>
        public ActionResult NodeParamSet()
        {
            return View();
        }

        /// <summary>
        /// 流程连线参数设置
        /// </summary>
        /// <returns></returns>
        public ActionResult LineParamSet()
        {
            return View();
        }

        /// <summary>
        /// 流程tips
        /// </summary>
        /// <returns></returns>
        public ActionResult FlowTips()
        {
            return View();
        }

        /// <summary>
        /// 流程操作指南
        /// </summary>
        /// <returns></returns>
        public ActionResult FlowOpGuide()
        {
            return View();
        }

        #endregion

        #region 数据处理

        #region 流程设计

        /// <summary>
        /// 获取流程分类树，包括流程分类下的流程
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFlowClassTree()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid parentId = _Request["parentId"].ObjToGuid(); //指定根节点
            bool noRoot = _Request["noRoot"].ObjToInt() == 1; //是否去掉根结点
            TreeNode node = BpmOperate.LoadFlowClassTree(parentId, GetCurrentUser(_Request));
            if (noRoot && node != null && node.id == Guid.Empty.ToString())
            {
                if (node.children != null && node.children.Count() > 0)
                    return Json(node.children.ToJson().Content, JsonRequestBehavior.AllowGet);
                else
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            return Json(node, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新流程图
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateWorkflowChart()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string workflowJson = HttpUtility.UrlDecode(MySecurity.DecodeBase64(_Request["workflowJson"].ObjToStr()), Encoding.UTF8);
            Bpm_WorkFlow workFlow = JsonHelper.Deserialize<Bpm_WorkFlow>(workflowJson);
            if (workFlow == null)
                return Json(new ReturnResult() { Success = false, Message = "流程对象反序列化失败！" });
            string errMsg = string.Empty;
            long count = CommonOperate.Count<Bpm_WorkFlowInstance>(out errMsg, false, x => x.Bpm_WorkFlowId == workFlow.Id);
            if (count > 0)
                return Json(new ReturnResult() { Success = false, Message = "当前流程已经在使用中不允许重建！" });
            Guid moduleId = BpmOperate.GetWorkflowModuleId(workFlow.Id);
            if (moduleId == Guid.Empty)
                return Json(new ReturnResult() { Success = false, Message = "流程没有设置关联模块！" });
            //保存流程节点
            //先删除节点
            CommonOperate.DeleteRecordsByExpression<Bpm_WorkNode>(x => x.Bpm_WorkFlowId == workFlow.Id, out errMsg);
            //删除节点审批按钮
            CommonOperate.DeleteRecordsByExpression<Bpm_NodeBtnConfig>(x => x.Bpm_WorkFlowId == workFlow.Id, out errMsg);
            if (workFlow.WorkNodes != null && workFlow.WorkNodes.Count > 0)
            {
                Sys_Form defaultForm = SystemOperate.GetDefaultForm(moduleId);
                //保存节点和节点审批按钮
                foreach (Bpm_WorkNode workNode in workFlow.WorkNodes)
                {
                    workNode.DisplayName = string.IsNullOrEmpty(workNode.DisplayName) ? workNode.Name : workNode.DisplayName;
                    workNode.Code = string.Format("WN{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                    workNode.Bpm_WorkFlowId = workFlow.Id;
                    if (workNode.WorkNodeTypeOfEnum != WorkNodeTypeEnum.Start && workNode.WorkNodeTypeOfEnum != WorkNodeTypeEnum.End &&
                        workNode.Sys_FormId == Guid.Empty && string.IsNullOrEmpty(workNode.FormUrl) && defaultForm != null)
                    {
                        workNode.Sys_FormId = defaultForm.Id;
                    }
                    Guid nodeId = CommonOperate.OperateRecord<Bpm_WorkNode>(workNode, ModelRecordOperateType.Add, out errMsg, null, false);
                    if (nodeId != Guid.Empty && workNode.BtnConfigs != null && workNode.BtnConfigs.Count > 0)
                    {
                        workNode.Id = nodeId;
                        workNode.BtnConfigs.ForEach(x =>
                        {
                            x.Code = string.Format("NBC{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                            x.Bpm_WorkFlowId = workFlow.Id;
                            x.Bpm_WorkNodeId = nodeId;
                        });
                        CommonOperate.OperateRecords<Bpm_NodeBtnConfig>(workNode.BtnConfigs, ModelRecordOperateType.Add, out errMsg, false);
                    }
                }
            }
            //保存流程连线
            //先删除连线
            CommonOperate.DeleteRecordsByExpression<Bpm_WorkLine>(x => x.Bpm_WorkFlowId == workFlow.Id, out errMsg);
            if (workFlow.WorkLines != null && workFlow.WorkLines.Count > 0)
            {
                foreach (Bpm_WorkLine workLine in workFlow.WorkLines)
                {
                    Bpm_WorkNode startNode = workFlow.WorkNodes.Where(x => x.TagId == workLine.FromTagId).FirstOrDefault();
                    Bpm_WorkNode endNode = workFlow.WorkNodes.Where(x => x.TagId == workLine.ToTagId).FirstOrDefault();
                    workLine.Bpm_WorkFlowId = workFlow.Id;
                    workLine.Bpm_WorkNodeStartId = startNode.Id;
                    workLine.Bpm_WorkNodeEndId = endNode.Id;
                    workLine.Code = string.Format("WL{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                    Guid lineId = CommonOperate.OperateRecord<Bpm_WorkLine>(workLine, ModelRecordOperateType.Add, out errMsg, null, false);
                }
            }
            return Json(new ReturnResult() { Success = true, Message = string.Empty });
        }

        /// <summary>
        /// 加载流程图
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadWorkflowChart()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid workflowId = _Request["workflowId"].ObjToGuid();
            Guid moduleId = _Request["moduleId"].ObjToGuid();
            Guid id = _Request["id"].ObjToGuid();
            Guid todoId = _Request["todoId"].ObjToGuid();
            string currNodeName = string.Empty;
            string currNodeTagId = string.Empty;
            string currNodeDisplay = string.Empty;
            int maxTop = 0;
            int maxLeft = 0;
            Bpm_WorkFlow workflow = null;
            string errMsg = string.Empty;
            if (workflowId == Guid.Empty) //表单页面
            {
                Bpm_WorkFlowInstance flowInst = BpmOperate.GetWorkflowInstance(moduleId, id);
                if (flowInst == null) //从历史数据中取
                {
                    Bpm_WorkFlowInstanceHistory workflowInstHistory = CommonOperate.GetEntity<Bpm_WorkFlowInstanceHistory>(x => x.RecordId == id, null, out errMsg);
                    if (workflowInstHistory != null)
                    {
                        flowInst = new Bpm_WorkFlowInstance();
                        ObjectHelper.CopyValue(workflowInstHistory, flowInst);
                    }
                }
                if (flowInst != null && flowInst.Bpm_WorkFlowId.HasValue)
                {
                    workflow = BpmOperate.GetWorkflow(flowInst.Bpm_WorkFlowId.Value);
                    ApprovalInfo approvalInfo = BpmOperate.GetRecordApprovalInfos(flowInst, true).FirstOrDefault();
                    if (approvalInfo != null)
                        currNodeName = approvalInfo.NextName;
                }
            }
            else //流程设计页面
            {
                workflow = BpmOperate.GetWorkflow(workflowId);
            }
            if (workflow != null)
            {
                Dictionary<string, object> dicNodes = null;
                Dictionary<string, object> dicLines = null;
                Dictionary<string, object> nodeParams = null;
                Dictionary<string, object> lineParams = null;
                List<Bpm_WorkNode> workNodes = BpmOperate.GetWorkNodesOfFlow(workflow.Id);
                List<Bpm_WorkLine> workLines = BpmOperate.GetWorkLinesOfFlow(workflow.Id);
                if (workNodes.Count > 0)
                {
                    if (!string.IsNullOrEmpty(currNodeName))
                    {
                        Bpm_WorkNode currNode = workNodes.Where(x => x.Name == currNodeName).FirstOrDefault();
                        if (currNode != null)
                        {
                            currNodeTagId = currNode.TagId;
                            currNodeDisplay = string.IsNullOrEmpty(currNode.DisplayName) ? currNode.Name : currNode.DisplayName;
                        }
                    }
                    dicNodes = new Dictionary<string, object>();
                    if (workflowId != Guid.Empty)
                        nodeParams = new Dictionary<string, object>();
                    foreach (Bpm_WorkNode tempNode in workNodes)
                    {
                        if (maxTop < tempNode.Top + tempNode.Height)
                            maxTop = tempNode.Top + tempNode.Height;
                        if (maxLeft <= tempNode.Left + tempNode.Width)
                            maxLeft = tempNode.Left + tempNode.Width;
                        string nodeType = "task round";
                        switch (tempNode.WorkNodeTypeOfEnum)
                        {
                            case WorkNodeTypeEnum.Start:
                                nodeType = "start round";
                                break;
                            case WorkNodeTypeEnum.End:
                                nodeType = "end round";
                                break;
                            default:
                                nodeType = "task round";
                                break;
                        }
                        dicNodes.Add(tempNode.TagId, new { type = nodeType, name = string.IsNullOrEmpty(tempNode.DisplayName) ? tempNode.Name : tempNode.DisplayName, alt = true, width = tempNode.Width, height = tempNode.Height, left = tempNode.Left, top = tempNode.Top });
                        if (workflowId != Guid.Empty)
                        {
                            if (tempNode.WorkNodeTypeOfEnum != WorkNodeTypeEnum.Start && tempNode.WorkNodeTypeOfEnum != WorkNodeTypeEnum.End)
                            {
                                tempNode.BtnConfigs = BpmOperate.GetAllApprovalBtnConfigs(x => x.Bpm_WorkFlowId == workflow.Id && x.Bpm_WorkNodeId == tempNode.Id);
                                nodeParams.Add(tempNode.TagId, new { Name = tempNode.Name, DisplayName = tempNode.DisplayName, Sys_FormId = tempNode.Sys_FormId, FormUrl = tempNode.FormUrl, HandlerType = tempNode.HandlerType, HandleRange = tempNode.HandleRange, HandleStrategy = tempNode.HandleStrategy, FormFieldName = tempNode.FormFieldName, BackType = tempNode.BackType, AutoJumpRule = tempNode.AutoJumpRule, Bpm_WorkFlowId = tempNode.Bpm_WorkFlowId, TagId = tempNode.TagId, SubFlowType = tempNode.SubFlowType, BtnConfigs = tempNode.BtnConfigs });
                            }
                        }
                    }
                }
                if (workLines.Count > 0)
                {
                    dicLines = new Dictionary<string, object>();
                    if (workflowId != Guid.Empty)
                        lineParams = new Dictionary<string, object>();
                    foreach (Bpm_WorkLine tempLine in workLines)
                    {
                        dicLines.Add(tempLine.TagId, new { type = string.IsNullOrEmpty(tempLine.LineType) ? "sl" : tempLine.LineType, name = tempLine.Note, M = tempLine.M, from = tempLine.FromTagId, to = tempLine.ToTagId });
                        if (workflowId != Guid.Empty)
                            lineParams.Add(tempLine.TagId, new { Note = tempLine.Note, FormCondition = tempLine.FormCondition, DutyCondition = tempLine.DutyCondition, DeptCondition = tempLine.DeptCondition, SqlCondition = tempLine.SqlCondition, IsCustomerCondition = tempLine.IsCustomerCondition });
                    }
                }
                int initNum = (dicNodes != null ? dicNodes.Count : 0) + (dicLines != null ? dicLines.Count : 0);
                long count = CommonOperate.Count<Bpm_WorkFlowInstance>(out errMsg, false, x => x.Bpm_WorkFlowId == workflow.Id);
                if (count == 0) //从历史数据中取
                {
                    count = CommonOperate.Count<Bpm_WorkFlowInstanceHistory>(out errMsg, false, x => x.Bpm_WorkFlowId == workflow.Id);
                }
                List<ApprovalInfo> appInfos = null;
                string titleKey = string.Empty;
                if (workflowId == Guid.Empty)
                {
                    titleKey = SystemOperate.GetModuleTitleKey(moduleId);
                    if (todoId == Guid.Empty) //查看页面
                    {
                        appInfos = BpmOperate.GetModuleRecordApprovalInfos(moduleId, id);
                    }
                    else //审批页面
                    {
                        appInfos = BpmOperate.GetRecordApprovalInfosByTodoId(todoId);
                    }
                }
                return Json(new { IsRun = count > 0, TitleKey = titleKey, AppInfos = appInfos, FlowData = new { title = workflow.Name, nodes = dicNodes, lines = dicLines, initNum = initNum }, NodeParams = nodeParams, LineParams = lineParams, CurrNodeTagObj = new { TagId = currNodeTagId, MaxTop = maxTop, MaxLeft = maxLeft, Display = currNodeDisplay } }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取节点表单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNodeForms()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid workflowId = _Request["workflowId"].ObjToGuid(); //流程ID
            string tagId = _Request["tagId"].ObjToStr(); //节点tagId
            Bpm_WorkFlow workflow = BpmOperate.GetWorkflow(workflowId);
            if (workflow != null && workflow.Sys_ModuleId.HasValue && workflow.Sys_ModuleId.Value != Guid.Empty)
            {
                List<Sys_Form> forms = SystemOperate.GetModuleForms(workflow.Sys_ModuleId.Value);
                return Json(forms);
            }
            return Json(null);
        }

        /// <summary>
        /// 加载处理者字段
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadHandlerFields()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid formId = _Request["formId"].ObjToGuid(); //表单ID
            List<Sys_FormField> formFields = SystemOperate.GetFormField(formId, false);
            if (formFields.Count > 0)
            {
                List<Guid> sysFieldIds = formFields.Where(x => x.Sys_FieldId.HasValue && x.Sys_FieldId.Value != Guid.Empty).Select(x => x.Sys_FieldId.Value).ToList();
                List<Sys_Field> fields = SystemOperate.GetFields(x => sysFieldIds.Contains(x.Id)).Where(x => x.ForeignModuleName == "员工管理").ToList();
                return Json(fields, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新结点参数，针对已在运行的流程
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateNodeParams()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid workflowId = _Request["workflowId"].ObjToGuid();
            string tagId = _Request["tagId"].ObjToStr();
            Bpm_WorkNode oldNode = BpmOperate.GetWorkNodeByTagId(workflowId, tagId);
            if (oldNode == null)
                return Json(new ReturnResult() { Success = false, Message = "原始结点不存在" });
            string nodeData = HttpUtility.UrlDecode(_Request["nodeData"].ObjToStr(), Encoding.UTF8);
            if (string.IsNullOrEmpty(nodeData))
                return Json(new ReturnResult() { Success = false, Message = "更新结点数据获取失败" });
            string errMsg = string.Empty;
            Bpm_WorkNode workNode = JsonHelper.Deserialize<Bpm_WorkNode>(nodeData, out errMsg);
            if (workNode == null)
                return Json(new ReturnResult() { Success = false, Message = string.Format("更新结点数据反序列化失败，{0}", errMsg) });
            if (string.IsNullOrWhiteSpace(workNode.Name))
                return Json(new ReturnResult() { Success = false, Message = "结点名称不能为空" });
            if (BpmOperate.GetWorkNodesOfFlow(workflowId).Where(x => x.Id != oldNode.Id && x.Name == workNode.Name.Trim()).Count() > 0)
                return Json(new ReturnResult() { Success = false, Message = "结点名称已使用，不能重复设置" });
            oldNode.Name = workNode.Name.Trim();
            oldNode.DisplayName = string.IsNullOrWhiteSpace(workNode.DisplayName) ? workNode.Name.Trim() : workNode.DisplayName.Trim();
            oldNode.Sys_FormId = workNode.Sys_FormId;
            oldNode.FormUrl = workNode.FormUrl;
            oldNode.HandlerType = workNode.HandlerType;
            oldNode.HandleRange = workNode.HandleRange;
            oldNode.HandleStrategy = workNode.HandleStrategy;
            oldNode.FormFieldName = workNode.FormFieldName;
            oldNode.BackType = workNode.BackType;
            oldNode.SubFlowType = workNode.SubFlowType;
            oldNode.Bpm_WorkFlowSubId = workNode.Bpm_WorkFlowSubId;
            oldNode.AutoJumpRule = workNode.AutoJumpRule;
            Guid nodeId = CommonOperate.OperateRecord<Bpm_WorkNode>(oldNode, ModelRecordOperateType.Edit, out errMsg, null, false);
            if (nodeId != Guid.Empty && workNode.BtnConfigs != null && workNode.BtnConfigs.Count > 0)
            {
                List<Guid> nodeBtnIds = workNode.BtnConfigs.Select(x => x.Id).ToList();
                CommonOperate.DeleteRecordsByExpression<Bpm_NodeBtnConfig>(x => x.Bpm_WorkFlowId == workflowId && x.Bpm_WorkNodeId == oldNode.Id && !nodeBtnIds.Contains(x.Id), out errMsg);
                foreach (var nodeConfig in workNode.BtnConfigs)
                {
                    if (nodeConfig.Id != Guid.Empty)
                    {
                        nodeConfig.Bpm_WorkFlowId = workflowId;
                        nodeConfig.Bpm_WorkNodeId = oldNode.Id;
                        CommonOperate.OperateRecord<Bpm_NodeBtnConfig>(nodeConfig, ModelRecordOperateType.Edit, out errMsg, null, false);
                    }
                    else
                    {
                        nodeConfig.Code = string.Format("NBC{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                        nodeConfig.Bpm_WorkFlowId = workflowId;
                        nodeConfig.Bpm_WorkNodeId = oldNode.Id;
                        CommonOperate.OperateRecord<Bpm_NodeBtnConfig>(nodeConfig, ModelRecordOperateType.Add, out errMsg, null, false);
                    }
                }
            }
            return Json(new ReturnResult() { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        #endregion

        #region 流程操作

        /// <summary>
        /// 批量发起
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="ids">记录ID，多个以逗号分隔</param>
        /// <returns></returns>
        public ActionResult MutiStartProcess(Guid moduleId, string ids)
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            if (string.IsNullOrEmpty(ids))
                return Json(new ReturnResult() { Success = false, Message = "记录ID为空" });
            StringBuilder sb = new StringBuilder();
            bool containsSuccess = false; //有发起成功
            string[] token = ids.Split(",".ToCharArray());
            List<Guid> list = token.Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
            if (list.Count == 0)
                return Json(new ReturnResult() { Success = false, Message = "没有满足条件的记录" });
            foreach (Guid id in list)
            {
                JsonResult result = StartProcess(moduleId, id) as JsonResult;
                PropertyInfo pErrMsg = result.Data.GetType().GetProperty("Message");
                string errMsg = pErrMsg.GetValue2(result.Data, null).ObjToStr();
                if (!string.IsNullOrEmpty(errMsg))
                    sb.AppendLine(errMsg);
                else
                    containsSuccess = true;
            }
            if (sb.ToString().Length > 0)
            {
                if (containsSuccess)
                    return Json(new ReturnResult() { Success = true, Message = string.Format("部分流程发起失败，{0}", sb.ToString()) });
                else
                    return Json(new ReturnResult() { Success = false, Message = string.Format("流程发起失败，{0}", sb.ToString()) });
            }
            return Json(new ReturnResult() { Success = true, Message = string.Empty });
        }

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public ActionResult StartProcess(Guid moduleId, Guid id)
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            //调用流程处理前自定义验证事件
            object checkMsgObj = CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, "BeforeFlowOperateCheck", new object[] { currUser, id, WorkActionEnum.Starting, null, null, null, null });
            string checkMsg = checkMsgObj.ObjToStr();
            if (checkMsg != string.Empty) //验证失败，返回
                return Json(new SaveDataReturnResult() { Success = false, Message = checkMsg });
            //发起流程
            string errMsg = BpmOperate.StartProcess(moduleId, id, currUser);
            Task.Factory.StartNew(() =>
            {
                LogOperate.AddOperateLog(currUser, SystemOperate.GetModuleNameById(moduleId), BpmOperate.GetFlowOpTypeDes(WorkActionEnum.Starting), JsonHelper.Serialize(new { RecordId = id }), string.IsNullOrEmpty(errMsg), errMsg);
            });
            return Json(new SaveDataReturnResult() { Success = string.IsNullOrEmpty(errMsg), Message = errMsg, RecordId = id });
        }

        /// <summary>
        /// 审批流程
        /// </summary>
        /// <param name="toDoTaskId">待办任务ID，子流程审批时为父待办ID</param>
        /// <param name="approvalOpinions">处理意见</param>
        /// <param name="workAction">动作</param>
        /// <param name="returnNodeId">退回时退回结点</param>
        /// <param name="directHandler">指派时的被指派人</param>
        /// <param name="childTodoIds">子流程待办ID集合</param>
        /// <returns></returns>
        public ActionResult ApprovalProcess(Guid toDoTaskId, string approvalOpinions, WorkActionEnum workAction, Guid? returnNodeId = null, Guid? directHandler = null, string childTodoIds = null)
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            if (string.IsNullOrEmpty(childTodoIds)) //非子流程审批
            {
                //调用流程处理前自定义验证事件
                Guid moduleId = Guid.Empty;
                Guid recordId = Guid.Empty;
                BpmOperate.GetModuleIdAndRecordIdByTodoId(toDoTaskId, out moduleId, out recordId);
                Guid currNodeId = BpmOperate.GetWorkNodeIdByTodoId(toDoTaskId);
                Bpm_WorkNode currNode = BpmOperate.GetWorkNode(currNodeId); //当前结点
                string currNodeName = currNode != null ? currNode.Name : string.Empty;
                object checkMsgObj = CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, "BeforeFlowOperateCheck", new object[] { currUser, recordId, workAction, toDoTaskId, returnNodeId, directHandler, currNodeName });
                string checkMsg = checkMsgObj.ObjToStr();
                if (checkMsg != string.Empty) //验证失败，返回
                    return Json(new SaveDataReturnResult() { Success = false, Message = checkMsg });
                //处理流程
                string errMsg = BpmOperate.ApproveProcess(toDoTaskId, approvalOpinions, currUser, workAction, returnNodeId, directHandler);
                Task.Factory.StartNew(() =>
                {
                    LogOperate.AddOperateLog(currUser, SystemOperate.GetModuleNameById(moduleId), BpmOperate.GetFlowOpTypeDes(workAction), JsonHelper.Serialize(new { RecordId = recordId }), string.IsNullOrEmpty(errMsg), errMsg);
                });
                return Json(new ReturnResult() { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
            }
            else //子流程审批
            {
                return MutiApprovalProcess(childTodoIds, approvalOpinions, workAction, returnNodeId, directHandler) as JsonResult;
            }
        }

        /// <summary>
        /// 批量审批
        /// </summary>
        /// <param name="toDoTaskIds">待办任务ID集合</param>
        /// <param name="approvalOpinions">处理意见</param>
        /// <param name="workAction">动作</param>
        /// <param name="returnNodeId">退回时退回结点</param>
        /// <param name="directHandler">指派时的被指派人</param>
        /// <returns></returns>
        public ActionResult MutiApprovalProcess(string toDoTaskIds, string approvalOpinions, WorkActionEnum workAction, Guid? returnNodeId = null, Guid? directHandler = null)
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            if (string.IsNullOrEmpty(toDoTaskIds))
                return Json(new ReturnResult() { Success = false, Message = "待办ID为空" });
            StringBuilder sb = new StringBuilder();
            bool containsSuccess = false; //有发起成功
            string[] token = toDoTaskIds.Split(",".ToCharArray());
            List<Guid> list = token.Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
            if (list.Count == 0)
                return Json(new ReturnResult() { Success = false, Message = "待办ID为空" });
            foreach (Guid todoId in list)
            {
                JsonResult result = ApprovalProcess(todoId, approvalOpinions, workAction, returnNodeId, directHandler) as JsonResult;
                PropertyInfo pErrMsg = result.Data.GetType().GetProperty("Message");
                string errMsg = pErrMsg.GetValue2(result.Data, null).ObjToStr();
                if (!string.IsNullOrEmpty(errMsg))
                    sb.AppendLine(errMsg);
                else
                    containsSuccess = true;
            }
            if (sb.ToString().Length > 0)
            {
                if (containsSuccess)
                    return Json(new ReturnResult() { Success = true, Message = string.Format("部分流程处理失败，{0}", sb.ToString()) });
                else
                    return Json(new ReturnResult() { Success = false, Message = string.Format("流程处理失败，{0}", sb.ToString()) });
            }
            return Json(new ReturnResult() { Success = true, Message = string.Empty });
        }

        /// <summary>
        /// 处理待办流程，包括同意、拒绝等操作
        /// </summary>
        /// <param name="toDoTaskId">待办任务ID，子流程审批时为父待办ID</param>
        /// <param name="approvalOpinions">处理意见</param>
        /// <param name="flowBtnId">操作按钮ID</param>
        /// <param name="returnNodeId">退回时退回结点</param>
        /// <param name="directHandler">指派时的被指派人</param>
        /// <param name="childTodoIds">子流程待办ID集合</param>
        /// <returns></returns>
        public ActionResult HandleOpProcess(Guid toDoTaskId, string approvalOpinions, Guid flowBtnId, Guid? returnNodeId = null, Guid? directHandler = null, string childTodoIds = null)
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            WorkActionEnum workAction = WorkActionEnum.NoAction;
            Bpm_FlowBtn flowBtn = BpmOperate.GetAllWorkButtons(x => x.Id == flowBtnId).FirstOrDefault();
            if (flowBtn != null)
            {
                switch (flowBtn.ButtonTypeOfEnum)
                {
                    case FlowButtonTypeEnum.AgreeBtn:
                        workAction = WorkActionEnum.Approving;
                        break;
                    case FlowButtonTypeEnum.BackBtn:
                        workAction = WorkActionEnum.Returning;
                        break;
                    case FlowButtonTypeEnum.RejectBtn:
                        workAction = WorkActionEnum.Refusing;
                        break;
                    case FlowButtonTypeEnum.AssignBtn:
                        workAction = WorkActionEnum.Directing;
                        break;
                }
            }
            else if (toDoTaskId == flowBtnId) //回退重新提交
            {
                workAction = WorkActionEnum.ReStarting;
            }
            return ApprovalProcess(toDoTaskId, approvalOpinions, workAction, returnNodeId, directHandler, childTodoIds);
        }

        /// <summary>
        /// 加载回退结点
        /// </summary>
        /// <param name="toDoTaskId">待办ID</param>
        /// <returns></returns>
        public ActionResult LoadBackNode(Guid toDoTaskId)
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            List<Bpm_WorkNode> backNodes = BpmOperate.GetBackNodes(toDoTaskId);
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"padding-top:20px;padding-left:30px\">");
            sb.Append("<select id=\"backNodes\" name=\"backNodes\" class=\"easyui-combobox\" style=\"width:200px;\" data-options=\"editable:false\">");
            foreach (Bpm_WorkNode node in backNodes)
            {
                string display = string.IsNullOrEmpty(node.DisplayName) ? node.Name : node.DisplayName;
                sb.AppendFormat("<option value=\"{0}\">{1}</option>", node.Id, display);
            }
            sb.Append("</select>");
            sb.Append("</div>");
            return Json(new { html = sb.ToString() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取我的待办、我的申请、我的审批
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMyToDoList()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            bool isAdmin = currUser.UserName == "admin";
            List<Bpm_WorkToDoList> list = new List<Bpm_WorkToDoList>();
            PageInfo pageInfo = PageInfo.GetPageInfo(_Request);
            int top = _Request["top"].ObjToInt();
            if (top > 0)
            {
                pageInfo.page = 1;
                pageInfo.pagesize = top;
            }
            string errMsg = string.Empty;
            if (currUser.EmpId.HasValue || isAdmin)
            {
                Guid userId = currUser.UserId;
                Guid empId = currUser.EmpId.HasValue ? currUser.EmpId.Value : Guid.Empty;
                int noAction = (int)WorkActionEnum.NoAction;
                int tp = _Request["tp"].ObjToInt();
                DatabaseType dbType = DatabaseType.MsSqlServer;
                string connStr = ModelConfigHelper.GetModelConnStr(typeof(Bpm_WorkToDoListHistory), out dbType, false);
                GridDataParmas gridParams = CommonOperate.GetGridDataParams(_Request);
                string whereSql = string.Empty;
                Dictionary<string, string> searchDic = gridParams != null ? JsonHelper.Deserialize<Dictionary<string, string>>(gridParams.Q) : null;
                Expression<Func<Bpm_WorkToDoList, bool>> searchExp = CommonOperate.GetGridFilterCondition<Bpm_WorkToDoList>(ref whereSql, searchDic, DataGridType.MainGrid, null, null, null, null, null, currUser);
                if (tp == 0) //我的待办
                {
                    Expression<Func<Bpm_WorkToDoList, bool>> exp = x => x.OrgM_EmpId == empId && x.WorkAction == noAction && x.IsDeleted == false;
                    if (isAdmin)
                        exp = x => x.WorkAction == noAction && x.IsDeleted == false;
                    if (searchExp != null)
                        exp = ExpressionExtension.And(exp, searchExp);
                    list = CommonOperate.GetPageEntities<Bpm_WorkToDoList>(out errMsg, pageInfo, false, exp, whereSql, null, false, null, null, currUser);
                }
                else if (tp == 1) //我的申请
                {
                    int startAction = (int)WorkActionEnum.Starting;
                    int startSubAction = (int)WorkActionEnum.SubStarting;
                    long total = 0;
                    DbLinkArgs currDbLink = ModelConfigHelper.GetDbLinkArgs(typeof(Bpm_WorkToDoList));
                    string workTodoHistoryTable = ModelConfigHelper.GetModuleTableName(typeof(Bpm_WorkToDoListHistory), currDbLink);
                    string table = string.Format("(SELECT * FROM Bpm_WorkToDoList UNION ALL SELECT * FROM {0}) A", workTodoHistoryTable);
                    string where = string.Format("IsDeleted=0 AND WorkAction IN({0},{1})", startAction.ToString(), startSubAction.ToString());
                    if (!isAdmin)
                        where += string.Format(" AND CreateUserId='{0}'", userId.ToString());
                    if (searchExp != null)
                    {
                        string tempWhere = new CommonOperate.TempOperate<Bpm_WorkToDoList>(currUser).ExpressionConditionToWhereSql(searchExp);
                        where += string.Format(" AND {0}", tempWhere);
                    }
                    DataTable dt = CommonOperate.PagingQueryByProcedure(out errMsg, out total, table, "*", where, pageInfo, connStr, dbType);
                    list = ObjectHelper.FillModel<Bpm_WorkToDoList>(dt);
                    CommonOperate.ExecuteCustomeOperateHandleMethod(gridParams.ModuleId, "PageGridDataHandle", new object[] { list, null, null });
                    pageInfo.totalCount = total;
                }
                else if (tp == 2) //我的审批
                {
                    long total = 0;
                    DbLinkArgs currDbLink = ModelConfigHelper.GetDbLinkArgs(typeof(Bpm_WorkToDoList));
                    string workTodoHistoryTable = ModelConfigHelper.GetModuleTableName(typeof(Bpm_WorkToDoListHistory), currDbLink);
                    string table = string.Format("(SELECT * FROM Bpm_WorkToDoList UNION ALL SELECT * FROM {0}) A", workTodoHistoryTable);
                    List<int> actionStatus = new List<int>() { (int)WorkActionEnum.Approving, (int)WorkActionEnum.Communicating, (int)WorkActionEnum.Directing, (int)WorkActionEnum.Refusing, (int)WorkActionEnum.Returning };
                    string actionStr = string.Join(",", actionStatus);
                    string where = string.Format("IsDeleted=0 AND WorkAction IN({0})", actionStr);
                    if (!isAdmin)
                        where += string.Format(" AND OrgM_EmpId='{0}'", empId.ToString());
                    if (searchExp != null)
                    {
                        string tempWhere = new CommonOperate.TempOperate<Bpm_WorkToDoList>(currUser).ExpressionConditionToWhereSql(searchExp);
                        where += string.Format(" AND {0}", tempWhere);
                    }
                    DataTable dt = CommonOperate.PagingQueryByProcedure(out errMsg, out total, table, "*", where, pageInfo, connStr, dbType);
                    list = ObjectHelper.FillModel<Bpm_WorkToDoList>(dt);
                    CommonOperate.ExecuteCustomeOperateHandleMethod(gridParams.ModuleId, "PageGridDataHandle", new object[] { list, null, null });
                    pageInfo.totalCount = total;
                }
            }
            if (list != null && list.Count > 0)
            {
                foreach (Bpm_WorkToDoList todo in list)
                {
                    if (!todo.Bpm_WorkFlowInstanceId.HasValue)
                        continue;
                    Bpm_WorkFlowInstance workFlowInst = BpmOperate.GetWorkflowInstanceById(todo.Bpm_WorkFlowInstanceId.Value);
                    if (workFlowInst == null)
                        continue;
                    if (todo.ParentId.HasValue && todo.ParentId.Value != Guid.Empty) //当前是子流程待办时以父待办显示
                    {
                        Bpm_WorkToDoList parentTodo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(todo.ParentId.Value, out errMsg, new List<string>() { "Id", "ModuleId", "RecordId" });
                        if (parentTodo == null) //父流程已结束，从历史待办中取
                        {
                            var tempList = CommonOperate.GetEntitiesBySql<Bpm_WorkToDoList>(out errMsg, string.Format("SELECT TOP 1 * FROM Bpm_WorkToDoListHistory WHERE Id='{0}'", todo.ParentId.Value.ToString()));
                            if (tempList != null && tempList.Count > 0)
                                parentTodo = tempList.FirstOrDefault();
                        }
                        if (parentTodo != null)
                        {
                            todo.Id = parentTodo.Id;
                            todo.ModuleId = parentTodo.ModuleId;
                            todo.ModuleName = SystemOperate.GetModuleNameById(parentTodo.ModuleId);
                            todo.RecordId = parentTodo.RecordId;
                        }
                    }
                    todo.StatusOfEnum = workFlowInst.StatusOfEnum;
                    Guid workNodeId = BpmOperate.GetWorkNodeIdByTodoId(todo.Id);
                    Bpm_WorkNode workNode = BpmOperate.GetWorkNode(workNodeId);
                    string formUrl = string.Empty;
                    if (workNode != null && !string.IsNullOrEmpty(workNode.FormUrl))
                        formUrl = workNode.FormUrl;
                    todo.FormUrl = formUrl;
                }
            }
            var result = list.Paged(pageInfo.totalCount);
            return result;
        }

        /// <summary>
        /// 获取模块结点集合
        /// </summary>
        /// <returns></returns>
        public ActionResult GetModuleNodes()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            Bpm_WorkFlow workflow = BpmOperate.GetModuleWorkFlow(module.Id);
            if (workflow == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            List<Bpm_WorkNode> nodes = BpmOperate.GetAllWorkNodes(x => x.Bpm_WorkFlowId == workflow.Id).Where(x => x.WorkNodeTypeOfEnum != WorkNodeTypeEnum.Start && x.WorkNodeTypeOfEnum != WorkNodeTypeEnum.End).OrderBy(x => x.Sort).ToList();
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// 异步控制器
    /// </summary>
    public class BpmAsyncController : AsyncBaseController
    {
        #region 构造函数

        private HttpRequestBase _Request = null; //请求对象

        /// <summary>
        /// 构造函数
        /// </summary>
        public BpmAsyncController()
        {
            _Request = Request;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request">请求对象</param>
        public BpmAsyncController(HttpRequestBase request)
        {
            _Request = request != null ? request : Request;
        }

        #endregion

        #region 流程设计

        /// <summary>
        /// 获取流程分类树，包括流程分类下的流程
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetFlowClassTreeAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new BpmController(Request).GetFlowClassTree();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 更新流程图
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> UpdateWorkflowChartAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new BpmController(Request).UpdateWorkflowChart();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 加载流程图
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadWorkflowChartAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new BpmController(Request).LoadWorkflowChart();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 获取节点表单
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetNodeFormsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new BpmController(Request).GetNodeForms();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 加载处理者字段
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadHandlerFieldsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new BpmController(Request).LoadHandlerFields();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 更新结点参数，针对已在运行的流程
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> UpdateNodeParamsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new BpmController(Request).UpdateNodeParams();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 流程操作

        /// <summary>
        /// 批量发起
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="ids">记录ID，多个以逗号分隔</param>
        /// <returns></returns>
        public Task<ActionResult> MutiStartProcessAsync(Guid moduleId, string ids)
        {
            return Task.Factory.StartNew(() =>
            {
                if (_Request == null) _Request = Request;
                return new BpmController(_Request).MutiStartProcess(moduleId, ids);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public Task<ActionResult> StartProcessAsync(Guid moduleId, Guid id)
        {
            return Task.Factory.StartNew(() =>
            {
                if (_Request == null) _Request = Request;
                return new BpmController(_Request).StartProcess(moduleId, id);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 审批流程
        /// </summary>
        /// <param name="toDoTaskId">待办任务ID</param>
        /// <param name="approvalOpinions">处理意见</param>
        /// <param name="workAction">动作</param>
        /// <param name="returnNodeId">退回时退回结点</param>
        /// <param name="directHandler">指派时的被指派人</param>
        /// <param name="childTodoIds">子流程待办ID集合</param>
        /// <returns></returns>
        public Task<ActionResult> ApprovalProcessAsync(Guid toDoTaskId, string approvalOpinions, WorkActionEnum workAction, Guid? returnNodeId = null, Guid? directHandler = null, string childTodoIds = null)
        {
            return Task.Factory.StartNew(() =>
            {
                if (_Request == null) _Request = Request;
                return new BpmController(_Request).ApprovalProcess(toDoTaskId, approvalOpinions, workAction, returnNodeId, directHandler, childTodoIds);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 批量审批流程
        /// </summary>
        /// <param name="toDoTaskIds">待办任务ID集合</param>
        /// <param name="approvalOpinions">处理意见</param>
        /// <param name="workAction">动作</param>
        /// <param name="returnNodeId">退回时退回结点</param>
        /// <param name="directHandler">指派时的被指派人</param>
        /// <returns></returns>
        public Task<ActionResult> MutiApprovalProcessAsync(string toDoTaskIds, string approvalOpinions, WorkActionEnum workAction, Guid? returnNodeId = null, Guid? directHandler = null)
        {
            return Task.Factory.StartNew(() =>
            {
                if (_Request == null) _Request = Request;
                return new BpmController(_Request).MutiApprovalProcess(toDoTaskIds, approvalOpinions, workAction, returnNodeId, directHandler);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 处理待办流程，包括同意、拒绝等操作
        /// </summary>
        /// <param name="toDoTaskId">待办任务ID</param>
        /// <param name="approvalOpinions">处理意见</param>
        /// <param name="flowBtnId">操作按钮ID</param>
        /// <param name="returnNodeId">退回时退回结点</param>
        /// <param name="directHandler">指派时的被指派人</param>
        /// <param name="childTodoIds">子流程待办ID集合</param>
        /// <returns></returns>
        public Task<ActionResult> HandleOpProcessAsync(Guid toDoTaskId, string approvalOpinions, Guid flowBtnId, Guid? returnNodeId = null, Guid? directHandler = null, string childTodoIds = null)
        {
            return Task.Factory.StartNew(() =>
            {
                if (_Request == null) _Request = Request;
                return new BpmController(_Request).HandleOpProcess(toDoTaskId, approvalOpinions, flowBtnId, returnNodeId, directHandler, childTodoIds);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 加载回退结点
        /// </summary>
        /// <param name="toDoTaskId">待办ID</param>
        /// <returns></returns>
        public Task<ActionResult> LoadBackNodeAsync(Guid toDoTaskId)
        {
            return Task.Factory.StartNew(() =>
            {
                if (_Request == null) _Request = Request;
                return new BpmController(_Request).LoadBackNode(toDoTaskId);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 获取我的待办、我的申请、我的审批
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetMyToDoListAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                if (_Request == null) _Request = Request;
                return new BpmController(_Request).GetMyToDoList();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 获取我的待办、我的申请、我的审批
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetModuleNodesAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                if (_Request == null) _Request = Request;
                return new BpmController(_Request).GetModuleNodes();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion
    }
}

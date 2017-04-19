using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Rookey.Frame.Common;
using Rookey.Frame.Base;
using Rookey.Frame.Base.User;
using Rookey.Frame.Model.Bpm;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Model.OrgM;
using Rookey.Frame.Operate.Base.EnumDef;
using Rookey.Frame.Model.Sys;
using System.Web;
using Rookey.Frame.Operate.Base.TempModel;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    /// <summary>
    /// 待办任务权限过滤
    /// </summary>
    class Bpm_WorkToDoListOperateHandle : IGridOperateHandle<Bpm_WorkToDoList>, IGridSearchHandle<Bpm_WorkToDoList>
    {
        #region 网格接口
        /// <summary>
        /// 网格参数设置
        /// </summary>
        /// <param name="gridType"></param>
        /// <param name="gridParams"></param>
        /// <param name="request"></param>
        public void GridParamsSet(DataGridType gridType, GridParams gridParams, HttpRequestBase request = null)
        {
            string tp = string.Empty;
            if (gridParams.DicPramas != null && gridParams.DicPramas.ContainsKey("p_tp"))
            {
                tp = gridParams.DicPramas["p_tp"].ObjToStr();
            }
            if (!string.IsNullOrEmpty(tp))
            {
                int tpType = tp.ObjToInt();
                gridParams.DataOrUrl = string.Format("/BpmAsync/GetMyToDoList.html?moduleName={0}&tp={1}", HttpUtility.UrlEncode("待办任务"), tpType);
                if (gridType == DataGridType.DesktopGrid)
                {
                    if (gridParams.DicPramas.ContainsKey("top"))
                        gridParams.DataOrUrl += "&top=" + gridParams.DicPramas["top"].ObjToInt();
                    if (tpType == 1) //我的申请
                    {
                        gridParams.GridFields = new List<Sys_GridField>();
                        gridParams.GridFields.Add(new Sys_GridField() { Sys_FieldName = "Title", Width = 400, Sort = 1 });
                        gridParams.GridFields.Add(new Sys_GridField() { Sys_FieldName = "Status", Width = 60, Sort = 2 });
                        gridParams.GridFields.Add(new Sys_GridField() { Sys_FieldName = "OrgM_EmpName", Width = 60, Sort = 3 });
                    }
                }
            }
            else
            {
                gridParams.DataOrUrl = string.Empty;
            }
        }

        /// <summary>
        /// 网格数据加载参数设置
        /// </summary>
        /// <param name="module"></param>
        /// <param name="gridDataParams"></param>
        /// <param name="request"></param>
        public void GridLoadDataParamsSet(Sys_Module module, GridDataParmas gridDataParams, HttpRequestBase request = null)
        {
        }

        /// <summary>
        /// 网格数据处理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="otherParams"></param>
        /// <param name="currUser"></param>
        public void PageGridDataHandle(List<Bpm_WorkToDoList> data, object[] otherParams = null, UserInfo currUser = null)
        {
            if (data != null && data.Count > 0)
            {
                string errMsg = string.Empty;
                foreach (Bpm_WorkToDoList todo in data)
                {
                    todo.ModuleName = SystemOperate.GetModuleNameById(todo.ModuleId);
                    if (!todo.Bpm_WorkFlowInstanceId.HasValue) continue;
                    Bpm_WorkFlowInstance flowInst = BpmOperate.GetWorkflowInstanceById(todo.Bpm_WorkFlowInstanceId.Value);
                    if (flowInst == null) continue;
                    todo.StatusOfEnum = flowInst.StatusOfEnum;
                    int status = (int)flowInst.StatusOfEnum;
                    Bpm_WorkToDoList tempTodo = CommonOperate.GetEntity<Bpm_WorkToDoList>(x => x.Bpm_WorkFlowInstanceId == todo.Bpm_WorkFlowInstanceId.Value && x.Status == status, null, out errMsg);
                    if (tempTodo != null && !string.IsNullOrEmpty(todo.NextNodeHandler))
                    {
                        List<Guid> tempEmpIds = tempTodo.NextNodeHandler.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
                        if (tempEmpIds.Count > 0)
                        {
                            todo.OrgM_EmpName = string.Join(",", tempEmpIds.Select(x => OrgMOperate.GetEmpName(x)).Where(x => !string.IsNullOrEmpty(x)));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取网格过滤条件
        /// </summary>
        /// <param name="where"></param>
        /// <param name="gridType"></param>
        /// <param name="condition"></param>
        /// <param name="initModule"></param>
        /// <param name="initField"></param>
        /// <param name="otherParams"></param>
        /// <param name="currUser"></param>
        /// <returns></returns>
        public Expression<Func<Bpm_WorkToDoList, bool>> GetGridFilterCondition(out string where, DataGridType gridType, Dictionary<string, string> condition = null, string initModule = null, string initField = null, Dictionary<string, string> otherParams = null, UserInfo currUser = null)
        {
            where = string.Empty;
            return null;
        }

        /// <summary>
        /// 网络按钮验证
        /// </summary>
        /// <param name="buttonText"></param>
        /// <param name="ids"></param>
        /// <param name="otherParams"></param>
        /// <param name="currUser"></param>
        /// <returns></returns>
        public string GridButtonOperateVerify(string buttonText, List<Guid> ids, object[] otherParams = null, UserInfo currUser = null)
        {
            return string.Empty;
        }
        #endregion
        #region 搜索接口
        public List<ConditionItem> GetSeachResults(Dictionary<string, string> q, out string whereSql, UserInfo currUser = null)
        {
            whereSql = string.Empty;
            return null;
        }

        public ConditionItem GetSearchResult(string fieldName, object value, out string whereSql, UserInfo currUser = null)
        {
            whereSql = string.Empty;
            if (!UserInfo.IsSuperAdmin(currUser) && !string.IsNullOrWhiteSpace(value.ObjToStr()))
            {
                if (fieldName == "OrgM_EmpId") //待办人
                {
                    whereSql = string.Format("CHARINDEX((SELECT TOP 1 NextNodeHandler FROM Bpm_WorkToDoList WHERE Bpm_WorkFlowInstanceId=Bpm_WorkFlowInstanceId AND Status=Status), (SELECT Id FROM dbo.OrgM_Emp WHERE Name LIKE '%{0}%'))>0", value.ObjToStr());
                }
            }
            return null;
        }
        #endregion
    }
}

using Rookey.Frame.Model.Bpm;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using Rookey.Frame.Base;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    class Bpm_WorkFlowOperateHandle : IModelOperateHandle<Bpm_WorkFlow>
    {
        /// <summary>
        /// 操作后处理，流程删除后删除对应的流程结点和流程连线
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandle(ModelRecordOperateType operateType, Bpm_WorkFlow t, bool result, UserInfo currUser, object[] otherParams = null)
        {
            string errMsg = string.Empty;
            if (result && operateType == ModelRecordOperateType.Del)
            {
                //删除流程结点
                CommonOperate.DeleteRecordsByExpression<Bpm_WorkNode>(x => x.Bpm_WorkFlowId == t.Id, out errMsg);
                //删除流程连线
                CommonOperate.DeleteRecordsByExpression<Bpm_WorkLine>(x => x.Bpm_WorkFlowId == t.Id, out errMsg);
            }
        }

        /// <summary>
        /// 操作前验证，如果流程已经在运行则不能删除
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, Bpm_WorkFlow t, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            if (operateType == ModelRecordOperateType.Del)
            {
                //如果该流程存在流程实例则不允许删除
                Bpm_WorkFlowInstance workflowInst = CommonOperate.GetEntity<Bpm_WorkFlowInstance>(x => x.Bpm_WorkFlowId == t.Id, null, out errMsg);
                if (workflowInst != null)
                {
                    errMsg = "运行中的流程不允许删除！";
                    return false;
                }
            }
            else if (operateType == ModelRecordOperateType.Add)
            {
                if (t.ValidEndTime != null && t.ValidEndTime.Value <= DateTime.Now)
                {
                    errMsg = "结束时间必须大于今天！";
                    return false;
                }
                if (t.ValidEndTime != null)
                {
                    if (t.ValidStartTime == null)
                    {
                        errMsg = "设置了结束时间必须也要设置起始时间！";
                        return false;
                    }
                    else if (t.ValidStartTime.Value < DateTime.Now)
                    {
                        errMsg = "起始时间必须大于等于今天！";
                        return false;
                    }
                }
                if (t.ValidStartTime == null && t.ValidEndTime == null)
                {
                    if (BpmOperate.GetAllWorkflows(x => x.Sys_ModuleId == t.Sys_ModuleId && x.ValidStartTime == null && x.ValidEndTime == null).Count > 0)
                    {
                        errMsg = "该模块已经存在了默认流程（起始时间和结束时间为空），不允许再添加默认流程！";
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 集合操作完成后事件
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandles(ModelRecordOperateType operateType, List<Bpm_WorkFlow> ts, bool result, UserInfo currUser, object[] otherParams = null)
        {
            string errMsg = string.Empty;
            if (result && operateType == ModelRecordOperateType.Del)
            {
                foreach (Bpm_WorkFlow t in ts)
                {
                    //删除流程结点
                    CommonOperate.DeleteRecordsByExpression<Bpm_WorkNode>(x => x.Bpm_WorkFlowId == t.Id, out errMsg);
                    //删除流程连线
                    CommonOperate.DeleteRecordsByExpression<Bpm_WorkLine>(x => x.Bpm_WorkFlowId == t.Id, out errMsg);
                }
            }
        }

        /// <summary>
        /// 操作前验证
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<Bpm_WorkFlow> ts, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            if (operateType == ModelRecordOperateType.Del)
            {
                foreach (Bpm_WorkFlow t in ts)
                {
                    //如果该流程存在流程实例则不允许删除
                    Bpm_WorkFlowInstance workflowInst = CommonOperate.GetEntity<Bpm_WorkFlowInstance>(x => x.Bpm_WorkFlowId == t.Id, null, out errMsg);
                    if (workflowInst != null)
                    {
                        errMsg += string.Format("【{0}】,", t.Name);
                    }
                }
                if (errMsg.Length > 0)
                {
                    errMsg = string.Format("流程{0}正在运行不允许删除！", errMsg);
                    return false;
                }
            }
            return true;
        }
    }
}

using Rookey.Frame.Base;
using Rookey.Frame.Model.OrgM;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    class OrgM_DeptOperateHandle : IModelOperateHandle<OrgM_Dept>
    {
        /// <summary>
        /// 部门操作完成
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandle(ModelRecordOperateType operateType, OrgM_Dept t, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (result)
            {
                string errMsg = string.Empty;
                Guid? parentId = null;
                if (t.ParentId.HasValue && t.ParentId.Value != Guid.Empty)
                {
                    OrgM_Dept parentDept = OrgMOperate.GetParentDept(t.ParentId.Value);
                    if (parentDept != null && !string.IsNullOrEmpty(parentDept.Name))
                    {
                        Sys_Organization parentOrg = CommonOperate.GetEntity<Sys_Organization>(x => x.Name == parentDept.Name && x.Flag == parentDept.Id.ToString() && !x.IsDeleted, null, out errMsg);
                        if (parentOrg != null) parentId = parentOrg.Id;
                    }
                }
                //部门新增后增加到系统组织中
                if (operateType == ModelRecordOperateType.Add)
                {
                    Sys_Organization org = new Sys_Organization()
                    {
                        Name = t.Name,
                        ParentId = parentId,
                        Flag = t.Id.ToString(),
                        Des = t.Alias,
                        CreateUserId = t.CreateUserId,
                        CreateUserName = t.CreateUserName,
                        ModifyUserId = t.ModifyUserId,
                        ModifyUserName = t.ModifyUserName
                    };
                    CommonOperate.OperateRecord<Sys_Organization>(org, ModelRecordOperateType.Add, out errMsg, null, false);
                }
                else if (operateType == ModelRecordOperateType.Edit)
                {
                    Sys_Organization org = CommonOperate.GetEntity<Sys_Organization>(x => x.Name == t.Name && x.Flag == t.Id.ToString() && !x.IsDeleted, null, out errMsg);
                    if (org != null)
                    {
                        org.ParentId = parentId;
                        org.ModifyDate = DateTime.Now;
                        org.ModifyUserId = t.ModifyUserId;
                        org.ModifyUserName = t.ModifyUserName;
                        List<string> fileNames = new List<string>() { "ParentId", "ModifyDate", "ModifyUserId", "ModifyUserName" };
                        CommonOperate.OperateRecord<Sys_Organization>(org, ModelRecordOperateType.Edit, out errMsg, fileNames, false);
                    }
                }
            }
        }

        /// <summary>
        /// 操作前处理
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, OrgM_Dept t, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// 操作后处理
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandles(ModelRecordOperateType operateType, List<OrgM_Dept> ts, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (ts != null && ts.Count > 0)
            {
                foreach (OrgM_Dept t in ts)
                {
                    OperateCompeletedHandle(operateType, t, result, currUser, otherParams);
                }
            }
        }

        /// <summary>
        /// 操作前处理
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<OrgM_Dept> ts, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            return true;
        }
    }
}

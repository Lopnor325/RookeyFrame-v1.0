using Rookey.Frame.Base;
using Rookey.Frame.Model.OrgM;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    /// <summary>
    /// 员工岗位操作类
    /// </summary>
    class OrgM_EmpDeptDutyOperateHandle : IModelOperateHandle<OrgM_EmpDeptDuty>
    {
        /// <summary>
        /// 操作完成后
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandle(ModelRecordOperateType operateType, OrgM_EmpDeptDuty t, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (result)
            {
                string errMsg = string.Empty;
                if (operateType == ModelRecordOperateType.Add || operateType == ModelRecordOperateType.Edit)
                {
                    if (t.OrgM_EmpId.HasValue && t.OrgM_EmpId.Value != Guid.Empty && t.OrgM_DeptId.HasValue && t.OrgM_DeptId.Value != Guid.Empty)
                    {
                        OrgM_Dept dept = OrgMOperate.GetDeptById(t.OrgM_DeptId.Value);
                        string username = OrgMOperate.GetUserNameByEmpId(t.OrgM_EmpId.Value);
                        if (!string.IsNullOrEmpty(username) && dept != null)
                        {
                            Sys_User user = UserOperate.GetUser(username);
                            Sys_Organization org = UserOperate.GetAllOrgs(x => x.Name == dept.Name && x.Flag == dept.Id.ToString()).FirstOrDefault();
                            if (user != null && org != null && user.Sys_OrganizationId != org.Id)
                            {
                                user.Sys_OrganizationId = org.Id;
                                CommonOperate.OperateRecord<Sys_User>(user, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "Sys_OrganizationId" }, false);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 操作前验证
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, OrgM_EmpDeptDuty t, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// 操作完成后
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandles(ModelRecordOperateType operateType, List<OrgM_EmpDeptDuty> ts, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (ts != null && ts.Count > 0)
            {
                foreach (OrgM_EmpDeptDuty t in ts)
                {
                    OperateCompeletedHandle(operateType, t, result, currUser, otherParams);
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
        public bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<OrgM_EmpDeptDuty> ts, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            return true;
        }
    }
}

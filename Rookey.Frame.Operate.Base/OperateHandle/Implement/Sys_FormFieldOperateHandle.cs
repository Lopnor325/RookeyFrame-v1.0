using Rookey.Frame.Common;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    class Sys_FormFieldOperateHandle : IModelOperateHandle<Sys_FormField>
    {
        /// <summary>
        /// 保存后
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandle(ModelRecordOperateType operateType, Sys_FormField t, bool result, Frame.Base.UserInfo currUser, object[] otherParams = null)
        {
            if (result)
            {
                string errMsg=string.Empty;
                Sys_FormField tempT = CommonOperate.GetEntityById<Sys_FormField>(t.Id, out errMsg);
                bool isFormEnableMemeryCache = ModelConfigHelper.IsModelEnableMemeryCache(typeof(Sys_Form)); //Sys_Form是否启动内存缓存
                if (tempT.Sys_FormId.HasValue && isFormEnableMemeryCache)
                {
                    Sys_Form form = SystemOperate.GetForm(tempT.Sys_FormId.Value);
                    if (form != null)
                        form.FormFields = null;
                }
            }
        }

        /// <summary>
        /// 保存前
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, Sys_FormField t, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            if (operateType == ModelRecordOperateType.Add || operateType == ModelRecordOperateType.Edit)
            {
                if (t.GroupName == string.Empty)
                    t.GroupName = null;
                if (t.TabName == string.Empty)
                    t.TabName = null;
                if (t.UrlOrData == string.Empty)
                    t.UrlOrData = null;
            }
            return true;
        }

        /// <summary>
        /// 保存后
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandles(ModelRecordOperateType operateType, List<Sys_FormField> ts, bool result, Frame.Base.UserInfo currUser, object[] otherParams = null)
        {
            if (ts != null)
            {
                foreach (Sys_FormField t in ts)
                {
                    OperateCompeletedHandle(operateType, t, result, currUser, otherParams);
                }
            }
        }

        /// <summary>
        /// 保存前
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<Sys_FormField> ts, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            if (operateType == ModelRecordOperateType.Add || operateType == ModelRecordOperateType.Edit)
            {
                if (ts != null && ts.Count > 0)
                {
                    foreach (Sys_FormField t in ts)
                    {
                        BeforeOperateVerifyOrHandle(operateType, t, out errMsg, otherParams);
                    }
                }
            }
            return true;
        }
    }
}

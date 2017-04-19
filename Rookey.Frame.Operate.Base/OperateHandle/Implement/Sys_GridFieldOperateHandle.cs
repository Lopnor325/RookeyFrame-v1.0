using Rookey.Frame.Base;
using Rookey.Frame.Common;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    /// <summary>
    /// 列表字段操作处理类
    /// </summary>
    class Sys_GridFieldOperateHandle : IModelOperateHandle<Sys_GridField>
    {
        /// <summary>
        /// 操作完成
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandle(ModelRecordOperateType operateType, Sys_GridField t, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (result)
            {
                Sys_GridField tempT = SystemOperate.GetAllGridFields(x => x.Id == t.Id).FirstOrDefault();
                bool isGridEnableMemeryCache = ModelConfigHelper.IsModelEnableMemeryCache(typeof(Sys_Grid)); //Sys_Grid是否启动内存缓存
                if (tempT.Sys_GridId.HasValue && isGridEnableMemeryCache)
                {
                    Sys_Grid grid = SystemOperate.GetGrid(tempT.Sys_GridId.Value);
                    if (grid != null)
                        grid.GridFields = null;
                }
                string errMsg = string.Empty;
                CommonOperate.UpdateRecordsByExpression<Sys_GridField>(new { FieldFormatter = "", EditorFormatter = "" }, x => x.Id == t.Id, out errMsg);
            }
        }

        /// <summary>
        /// 操作前
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, Sys_GridField t, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// 操作完成
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandles(ModelRecordOperateType operateType, List<Sys_GridField> ts, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (ts != null)
            {
                foreach (Sys_GridField t in ts)
                {
                    OperateCompeletedHandle(operateType, t, result, currUser, otherParams);
                }
            }
        }

        /// <summary>
        /// 操作前
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<Sys_GridField> ts, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            return true;
        }
    }
}

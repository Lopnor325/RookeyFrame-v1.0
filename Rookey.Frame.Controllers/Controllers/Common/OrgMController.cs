/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System.Collections.Generic;
using System.Web.Mvc;
using Rookey.Frame.Common;
using Rookey.Frame.Model.OrgM;
using Rookey.Frame.Operate.Base;
using System.Web;
using System.Threading.Tasks;
using Rookey.Frame.Controllers.Attr;
using System;

namespace Rookey.Frame.Controllers.OrgM
{
    /// <summary>
    /// 组织机构相关操作控制器（异步）
    /// </summary>
    public class OrgMAsyncController : AsyncBaseController
    {
        /// <summary>
        /// 异步获取部门职务
        /// </summary>
        /// <returns></returns>
        [OpTimeMonitor]
        public Task<ActionResult> GetDeptDutysAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new OrgMController(Request).GetDeptDutys();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步获取员工的层级部门信息
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetEmpLevelDepthDeptAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new OrgMController(Request).GetEmpLevelDepthDept();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }
    }

    /// <summary>
    /// 组织机构相关操作控制器
    /// </summary>
    public class OrgMController : BaseController
    {
        #region 构造函数

        private HttpRequestBase _Request = null; //请求对象

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public OrgMController()
        {
            _Request = Request;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request">请求对象</param>
        public OrgMController(HttpRequestBase request)
            : base(request)
        {
            _Request = request;
        }

        #endregion

        /// <summary>
        /// 获取部门职务
        /// </summary>
        /// <returns></returns>
        [OpTimeMonitor]
        public ActionResult GetDeptDutys()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid deptId = _Request["deptId"].ObjToGuid();
            List<OrgM_Duty> dutys = OrgMOperate.GetDeptDutys(deptId);
            dutys.Insert(0, new OrgM_Duty() { Id = Guid.Empty, Name = "请选择" });
            return Json(dutys, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取员工的层级部门信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEmpLevelDepthDept()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            int levelDepth = _Request["levelDepth"].ObjToInt(); //层级
            Guid empId = _Request["empId"].ObjToGuid(); //员工ID
            Guid? companyId = _Request["companyId"].ObjToGuidNull(); //所属公司，集团模式下用到
            Guid? deptId = _Request["deptId"].ObjToGuidNull(); //兼职部门，以兼职部门找
            if (empId == Guid.Empty || levelDepth < 0)
                return Json(null);
            //层级部门
            OrgM_Dept depthDept = OrgMOperate.GetEmpLevelDepthDept(levelDepth, empId, companyId, deptId);
            //当前部门
            OrgM_Dept currDept = deptId.HasValue && deptId.Value != Guid.Empty ? OrgMOperate.GetDeptById(deptId.Value) : OrgMOperate.GetEmpMainDept(empId, companyId);
            return Json(new { CurrDept = currDept, DepthDept = depthDept });
        }
    }
}

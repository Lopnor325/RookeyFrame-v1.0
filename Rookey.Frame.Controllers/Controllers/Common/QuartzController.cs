using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Rookey.Frame.QuartzClient;
using Rookey.Frame.Common;
using Rookey.Frame.Controllers.Other;
using Rookey.Frame.Operate.Base;
using System.Data;

namespace Rookey.Frame.Controllers
{
    /// <summary>
    /// 任务调度控制器
    /// </summary>
    public class QuartzController : BaseController
    {
        #region 页面
        /// <summary>
        /// Job管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult JobManage()
        {
            return View();
        }
        /// <summary>
        /// 添加任务页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddJob()
        {
            return View();
        }
        /// <summary>
        /// 添加任务计划页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddJobPlan()
        {
            return View();
        }
        /// <summary>
        /// 调度中心页面
        /// </summary>
        /// <returns></returns>
        public ActionResult QuartzCenter()
        {
            return View();
        }
        /// <summary>
        /// 调度日志查看页面
        /// </summary>
        /// <returns></returns>
        public ActionResult QuartzLog()
        {
            return View();
        }

        #endregion

        #region 数据获取

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJobList()
        {
            QuartzDataHandler quartzDataHandler = QuartzDataHandler.GetInstance();
            string errMsg = string.Empty;
            List<Dictionary<string, string>> jobs = quartzDataHandler.GetAllJob(out errMsg);
            object obj = new
            {
                total = jobs.Count,
                rows = jobs
            };
            return Json(obj);
        }

        /// <summary>
        /// 调度日志
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQuartzLog()
        {
            var jobName = Server.UrlDecode(Request["jobName"].ObjToStr());
            var jobGroup = Server.UrlDecode(Request["jobGroup"].ObjToStr());
            if (string.IsNullOrEmpty(jobName) || string.IsNullOrEmpty(jobGroup))
                return Json(null);
            string errMsg = string.Empty;
            string connString = WebConfigHelper.GetConnectionString("QuartzConnString");
            DateTime dtBefore = DateTime.Now.AddDays(-7);
            dtBefore = DateTime.SpecifyKind(dtBefore, DateTimeKind.Utc);
            DateTimeOffset dtOffSet = dtBefore;
            long ticks = dtOffSet.UtcTicks;
            string sql = string.Format("SELECT * FROM QRTZ_FIRED_TRIGGERS WHERE FIRED_TIME>{0} AND JOB_NAME='{1}' AND JOB_GROUP='{2}' ORDER BY FIRED_TIME DESC", ticks, jobName, jobGroup);
            DataTable dt = CommonOperate.ExecuteQuery(out errMsg, sql, null, connString);
            if (dt != null && dt.Rows.Count > 0)
            {
                var list = dt.AsEnumerable().Cast<DataRow>().Select(x => new { jobName = x["JOB_NAME"].ObjToStr(), jobGroup = x["JOB_GROUP"].ObjToStr(), planName = x["TRIGGER_NAME"].ObjToStr(), planGroupName = x["TRIGGER_GROUP"].ObjToStr(), planExecTime = new DateTimeOffset(x["FIRED_TIME"].ObjToLong(), TimeSpan.Zero).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss") });
                return Json(list);
            }
            return Json(null);
        }

        /// <summary>
        /// 获取任务计划列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJobPlanList()
        {
            string op = Request["op"];
            string jobName = Request.Params["jobName"];
            string jobGroupName = Request.Params["jobGroupName"];
            QuartzDataHandler quartzDataHandler = QuartzDataHandler.GetInstance();
            List<Dictionary<string, string>> jobs = quartzDataHandler.GetJobPlan(jobName, jobGroupName, op);
            object obj = new
            {
                total = jobs.Count,
                rows = jobs
            };
            return Json(obj);
        }

        /// <summary>
        /// 获取任务组
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJobGroupList()
        {
            QuartzDataHandler quartzDataHandler = QuartzDataHandler.GetInstance();
            List<string> list = quartzDataHandler.GetAllGroupsName();
            var obj = from o in list select new { id = o, text = o };
            return Json(obj);
        }

        /// <summary>
        /// 获取计划组
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPlanGroupList()
        {
            QuartzDataHandler quartzDataHandler = QuartzDataHandler.GetInstance();
            List<string> list = quartzDataHandler.GetAllTriggerGroupName();
            var obj = from o in list select new { id = o, text = o };
            return Json(obj);
        }

        /// <summary>
        /// 获取任务类型列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJobTypeList()
        {
            string jobTypeDll = WebConfigHelper.GetAppSettingValue("QuartzJob");
            List<Type> types = Globals.GetAssemblyTypes(jobTypeDll);
            if (types == null) types = new List<Type>();
            List<Type> jobTypes = new List<Type>();
            foreach (Type tempType in types)
            {
                if (tempType.GetInterface("IJob") != null)
                    jobTypes.Add(tempType);
            }
            List<Type> frameTypes = Globals.GetAssemblyTypes("Rookey.Frame.QuartzClient").Where(x => x.Namespace == "Rookey.Frame.QuartzClient.FrameJob" && x.GetInterface("IJob") != null).ToList();
            var frameResult = frameTypes.Select(x => new { id = string.Format("{0},Rookey.Frame.QuartzClient", x.FullName), text = x.FullName }).ToList();
            var result = jobTypes.Select(x => new { id = string.Format("{0},{1}", x.FullName, jobTypeDll), text = x.FullName }).ToList();
            result.AddRange(frameResult);
            if (result.Count == 0) return Json(null);
            return Json(result);
        }

        /// <summary>
        /// 获取任务调度信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSchedulerInfo()
        {
            try
            {
                QuartzDataHandler quartzDataHandler = QuartzDataHandler.GetInstance();
                var obj = new
                {
                    SchedulerName = quartzDataHandler.SchedulerName,
                    SchedulerInstanceID = quartzDataHandler.SchedulerInstanceID,
                    SchedulerIsRemote = quartzDataHandler.SchedulerIsRemote.ToString().ToLower(),
                    SchedulerType = quartzDataHandler.SchedulerType,
                    SchedulerStartTime = quartzDataHandler.SchedulerStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    SchedulerRunningStatus = quartzDataHandler.SchedulerRunningStatus,
                    JobTotal = quartzDataHandler.JobTotal,
                    JobExecuteTotal = quartzDataHandler.JobExecuteTotal
                };
                return Json(obj);
            }
            catch { }
            return Json(null);
        }

        /// <summary>
        /// 获取调度服务信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQuartzServiceInfo()
        {
            string quartzServiceInfo = ServiceControl.GetQuartzServiceInfo();
            string localHostName = Dns.GetHostName();
            string localIp = Globals.GetClientIp();
            string remoteHostName = Server.MachineName;
            return Json(new { Message = quartzServiceInfo, ErrMsg = ServiceControl.ErrMsg, RemoteHostName = remoteHostName, LocalHostName = localHostName, LocalIp = localIp });
        }

        #endregion

        #region 数据操作

        /// <summary>
        /// 保存任务
        /// </summary>
        /// <param name="jobInfo">任务JSON数据</param>
        /// <returns></returns>
        public ActionResult SaveJob(string jobInfo)
        {
            string errMsg = string.Empty;
            QuartzDataHandler quartzDataHandler = QuartzDataHandler.GetInstance();
            errMsg = quartzDataHandler.AddJob(jobInfo);
            return Json(new ReturnResult() { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        /// <summary>
        /// 任务操作
        /// </summary>
        /// <returns></returns>
        public ActionResult OperateJob()
        {
            QuartzDataHandler quartzDataHandler = QuartzDataHandler.GetInstance();
            string json = string.Empty;
            string op = Request["op"];
            string jobName = Request.Params["jobName"];
            string jobGroupName = Request.Params["jobGroupName"];
            string errMsg = string.Empty;
            switch (op)
            {
                case "DeleteJob": //删除任务
                    errMsg = quartzDataHandler.DeleteJob(jobName, jobGroupName);
                    break;
                case "ExecuteJob": //立即执行任务
                    errMsg = quartzDataHandler.RunJobNow(jobName, jobGroupName);
                    break;
                case "PauseJob": //暂停任务
                    errMsg = quartzDataHandler.PauseJob(jobName, jobGroupName);
                    break;
                case "ResumeJob": //恢复任务
                    errMsg = quartzDataHandler.ResumeJob(jobName, jobGroupName);
                    break;
            }
            return Json(new ReturnResult() { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        /// <summary>
        /// 服务操作
        /// </summary>
        /// <param name="op"></param>
        public ActionResult ServiceOperate(string op)
        {
            bool isSuccess = false;
            switch (op)
            {
                case "SetupQuartzService": //安装调度服务
                    isSuccess = ServiceControl.InstallQuartzService();
                    break;
                case "UnSetupQuartzService": //反安装调度服务
                    isSuccess = ServiceControl.UninstallQuartzService();
                    break;
                case "StartQuartzService": //启动调度服务
                    isSuccess = ServiceControl.StartQuartzService();
                    break;
                case "StopQuartzService": //停止调度服务
                    isSuccess = ServiceControl.StopQuartzService();
                    break;
                case "RestartQuartzService": //重启动调度服务
                    isSuccess = ServiceControl.RestartQuartzService();
                    break;
            }
            string errMsg = ServiceControl.ErrMsg;
            return Json(new ReturnResult() { Success = isSuccess, Message = errMsg });
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Quartz.Core;
using Quartz.Impl.Matchers;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using Quartz.Impl;
using System.Configuration;
using Quartz;
using Rookey.Frame.Common;

namespace Rookey.Frame.QuartzClient
{
    /// <summary>
    /// quartz任务调度处理类
    /// </summary>
    public class QuartzDataHandler
    {
        #region 成员
        private static DefaultScheduler _scheduler = null;
        private static SchedulerData _data = null;
        private static QuartzDataHandler _instance = null;
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <returns></returns>
        public static QuartzDataHandler GetInstance()
        {
            if (_instance == null)
            {
                _instance = new QuartzDataHandler();
            }
            else if (_scheduler != null)
            {
                _data = _scheduler.Data;
            }
            return _instance;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private QuartzDataHandler()
        {
            if (_scheduler == null)
            {
                string quartzServer = WebConfigHelper.GetAppSettingValue("QuartzServer");
                if (string.IsNullOrEmpty(quartzServer))
                    quartzServer = "tcp://127.0.0.1:7005";
                NameValueCollection properties = new NameValueCollection();
                properties["quartz.scheduler.instanceName"] = "RemoteQuartzClient";
                properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                properties["quartz.threadPool.threadCount"] = "5";
                properties["quartz.threadPool.threadPriority"] = "Normal";
                //远程调度配置
                properties["quartz.scheduler.proxy"] = "true";
                properties["quartz.scheduler.proxy.address"] = string.Format("{0}/QuartzScheduler", quartzServer);
                try
                {
                    ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
                    IScheduler scheduler = schedulerFactory.GetScheduler();
                    if (scheduler != null)
                    {
                        _scheduler = new DefaultScheduler(scheduler);
                        _scheduler.StartScheduler();
                        _data = _scheduler.Data;
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        #endregion

        #region 属性

        public string SchedulerName
        {
            get
            {
                if (_data == null) return string.Empty;
                return _data.Name;
            }
        }

        public string SchedulerInstanceID
        {
            get
            {
                if (_data == null) return string.Empty;
                return _data.InstanceId;
            }
        }

        public bool SchedulerIsRemote
        {
            get
            {
                if (_data == null) return false;
                return _data.IsRemote;
            }
        }

        public string SchedulerType
        {
            get
            {
                if (_data == null) return string.Empty;
                return _data.SchedulerType.ToString();
            }
        }

        public DateTime SchedulerStartTime
        {
            get
            {
                if (_data == null) return DateTime.MinValue;
                return _data.RunningSince.Value.LocalDateTime;
            }
        }

        public string SchedulerRunningStatus
        {
            get
            {
                if (_data == null) return "未初始化";
                SchedulerStatus status = _data.Status;
                switch (status)
                {
                    case SchedulerStatus.Empty:
                        return "未初始化";
                    case SchedulerStatus.Ready:
                        return "准备";
                    case SchedulerStatus.Shutdown:
                        return "关闭";
                    case SchedulerStatus.Started:
                        return "启动";
                    default:
                        return string.Empty;
                }
            }
        }

        public int JobTotal
        {
            get
            {
                if (_data == null) return 0;
                return _data.JobsTotal;
            }
        }

        public int JobExecuteTotal
        {
            get
            {
                if (_data == null) return 0;
                return _data.JobsExecuted;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 运行状态
        /// </summary>
        /// <param name="scheduler">调度对象</param>
        /// <returns></returns>
        private SchedulerStatus GetSchedulerStatus(IScheduler scheduler)
        {
            if (scheduler == null) return SchedulerStatus.Empty;
            if (scheduler.IsShutdown)
            {
                return SchedulerStatus.Shutdown;
            }

            if (scheduler.GetJobGroupNames() == null || scheduler.GetJobGroupNames().Count == 0)
            {
                return SchedulerStatus.Empty;
            }

            if (scheduler.IsStarted)
            {
                return SchedulerStatus.Started;
            }

            return SchedulerStatus.Ready;
        }

        #endregion

        #region 公共方法

        #region 任务

        /// <summary>
        /// 获取任务详细信息
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public JobDetailsData GetJobDetailsData(string jobName, string jobGroupName)
        {
            JobDetailsData jobData = _scheduler.GetJobDetailsData(jobName, jobGroupName);
            return jobData;
        }

        /// <summary>
        /// 获取所有任务组名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllGroupsName()
        {
            List<string> listName = new List<string>();
            IList<JobGroupData> list = _scheduler.Data.JobGroups;
            if (list != null && list.Count > 0)
            {
                foreach (JobGroupData group in list)
                {
                    listName.Add(group.Name);
                }
            }
            return listName;
        }

        /// <summary>
        /// 获取所有任务信息，返回JSON格式数据
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetAllJob(out string errMsg)
        {
            errMsg = string.Empty;
            if (_scheduler == null)
            {
                errMsg = "远程调度服务未开启，请检查！";
                return new List<Dictionary<string, string>>();
            }
            List<Dictionary<string, string>> listDic = new List<Dictionary<string, string>>();
            List<JobData> listJobs = new List<JobData>();
            IList<JobGroupData> list = _scheduler.Data.JobGroups;
            if (list != null && list.Count > 0)
            {
                foreach (JobGroupData group in list)
                {
                    listJobs.AddRange(group.Jobs);
                }
            }
            if (listJobs != null && listJobs.Count > 0)
            {
                foreach (JobData job in listJobs)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    JobDetailsData jobData = _scheduler.GetJobDetailsData(job.Name, job.GroupName);
                    dic["jobGroup"] = job.GroupName;
                    dic["jobName"] = job.Name;
                    string status = string.Empty;
                    switch (job.Status)
                    {
                        case ActivityStatus.Active:
                            status = "运行中";
                            break;
                        case ActivityStatus.Complete:
                            status = "运行完毕";
                            break;
                        case ActivityStatus.Mixed:
                            status = "运行中";
                            break;
                        case ActivityStatus.Paused:
                            status = "暂停中";
                            break;

                    }
                    dic["jobStatus"] = status;
                    dic["jobType"] = jobData.JobProperties["Full name"] == null ? string.Empty : jobData.JobProperties["Full name"].ToString();//任务类型
                    dic["jobDes"] = jobData.JobProperties["Description"] == null ? string.Empty : jobData.JobProperties["Description"].ToString();//任务描述
                    dic["viewLog"] = "查看日志";
                    listDic.Add(dic);
                }
            }
            return listDic;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="jobInfo">序列化后的任务数据</param>
        public string AddJob(string jobInfo)
        {
            string errMsg = string.Empty;
            if (_scheduler == null)
            {
                errMsg = "远程调度服务未开启！";
                return errMsg;
            }
            try
            {
                string json = string.Empty;
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                serialize.MaxJsonLength = Int32.MaxValue;
                Job job = serialize.Deserialize<Job>(jobInfo);
                errMsg = _scheduler.AddJob(job);
                return errMsg;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroupName">任务组名</param>
        /// <returns></returns>
        public string DeleteJob(string jobName, string jobGroupName)
        {
            return _scheduler.DeleteJob(jobName, jobGroupName);
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroupName">任务组名</param>
        /// <returns></returns>
        public string PauseJob(string jobName, string jobGroupName)
        {
            return _scheduler.PauseJob(jobName, jobGroupName);
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroupName">任务组名</param>
        /// <returns></returns>
        public string ResumeJob(string jobName, string jobGroupName)
        {
            return _scheduler.ResumeJob(jobName, jobGroupName);
        }

        /// <summary>
        /// 立即执行任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public string RunJobNow(string jobName, string jobGroupName)
        {
            return _scheduler.RunJobNow(jobName, jobGroupName);
        }

        #endregion

        #region 计划

        /// <summary>
        /// 获取所有计划组名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTriggerGroupName()
        {
            List<string> listName = new List<string>();
            IList<TriggerGroupData> list = _scheduler.Data.TriggerGroups;
            if (list != null && list.Count > 0)
            {
                foreach (TriggerGroupData group in list)
                {
                    listName.Add(group.Name);
                }
            }
            return listName;
        }

        /// <summary>
        /// 获取任务计划
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public IList<TriggerData> GetJobPlan(string jobName, string jobGroupName)
        {
            IList<TriggerData> listTrigger = _scheduler.GetTriggers(jobName, jobGroupName);
            return listTrigger;
        }

        /// <summary>
        /// 获取任务计划数据,返回JSON格式数据
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetJobPlan(string jobName, string jobGroupName, string op)
        {
            List<Dictionary<string, string>> listDic = new List<Dictionary<string, string>>();
            IList<TriggerData> listTrigger = _scheduler.GetTriggers(jobName, jobGroupName);
            if (listTrigger != null && listTrigger.Count > 0)
            {
                foreach (TriggerData tri in listTrigger)
                {
                    string statusStr = string.Empty;
                    switch (tri.Status)
                    {
                        case ActivityStatus.Active:
                            statusStr = "运行中";
                            break;
                        case ActivityStatus.Paused:
                            statusStr = "暂停中";
                            break;
                        case ActivityStatus.Complete:
                            statusStr = "已结束";
                            break;
                        case ActivityStatus.Mixed:
                            statusStr = "";
                            break;
                    }
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    if (string.IsNullOrEmpty(op) || op == "view")
                    {
                        dic["planName"] = tri.Name;
                        dic["planGroupName"] = tri.Group;
                        dic["planPriority"] = tri.Priority.ToString();
                        dic["planStartTime"] = tri.StartDate.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                        dic["planEndTime"] = tri.EndDate.HasValue ? tri.EndDate.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
                        dic["planPreFireTime"] = tri.PreviousFireDate.HasValue ? tri.PreviousFireDate.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
                        dic["planNextFireTime"] = tri.NextFireDate.HasValue ? tri.NextFireDate.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
                        dic["planDes"] = tri.Des == null ? string.Empty : tri.Des;
                        dic["planStatus"] = statusStr;
                    }
                    listDic.Add(dic);
                }
            }
            return listDic;
        }

        #endregion

        #endregion
    }
}

using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Linq;
using System.Text;
using Quartz.Collection;
using System.IO;

namespace Rookey.Frame.QuartzClient
{
    class DefaultScheduler
    {
        private readonly IScheduler _scheduler;

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="scheduler"></param>
        public DefaultScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 调度数据
        /// </summary>
        public SchedulerData Data
        {
            get
            {
                if (_scheduler == null) return null;
                var metadata = _scheduler.GetMetaData();
                var jobGroups = GetJobGroups();
                int jobCount = 0;
                foreach (var group in jobGroups)
                {
                    if (group.Jobs != null)
                        jobCount += group.Jobs.Count();
                }
                return new SchedulerData
                           {
                               Name = _scheduler.SchedulerName,
                               InstanceId = _scheduler.SchedulerInstanceId,
                               JobGroups = jobGroups,
                               TriggerGroups = GetTriggerGroups(_scheduler),
                               Status = GetSchedulerStatus(_scheduler),
                               IsRemote = true,
                               JobsExecuted = metadata.NumberOfJobsExecuted,
                               RunningSince = metadata.RunningSince,
                               SchedulerType = metadata.SchedulerType,
                               JobsTotal = jobCount
                           };
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 启动调度
        /// </summary>
        /// <returns></returns>
        public string StartScheduler()
        {
            try
            {
                if (_scheduler == null)
                    return "调度对象为空";
                _scheduler.Start();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 停止调度
        /// </summary>
        /// <returns></returns>
        public string StopScheduler()
        {
            try
            {
                if (_scheduler == null)
                    return "调度对象为空";
                _scheduler.Shutdown();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 暂停任务组
        /// </summary>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public string PauseJobGroup(string jobGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                _scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(jobGroupName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 恢复暂停的任务组
        /// </summary>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public string ResumeJobGroup(string jobGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                _scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(jobGroupName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public string PauseJob(string jobName, string jobGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                _scheduler.PauseJob(new JobKey(jobName, jobGroupName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 恢得暂停的任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public string ResumeJob(string jobName, string jobGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                _scheduler.ResumeJob(new JobKey(jobName, jobGroupName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 暂停执行计划
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="jogGroupName"></param>
        /// <returns></returns>
        public string PauseTrigger(string triggerName, string jogGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                _scheduler.PauseTrigger(new TriggerKey(triggerName, jogGroupName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 恢得暂停的执行计划
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public string ResumeTrigger(string triggerName, string jobGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                _scheduler.ResumeTrigger(new TriggerKey(triggerName, jobGroupName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 触发任务执行
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public string TriggerJobExecute(string jobName, string jobGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                _scheduler.TriggerJob(new JobKey(jobName, jobGroupName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="triggersAndJob"></param>
        /// <returns></returns>
        public string AddJob(Job job)
        {
            if (job == null) return "任务为空";
            try
            {
                System.Type jobType = Type.GetType(job.JobType);
                IJobDetail JobDetail = JobBuilder
                    .Create()
                    .OfType(jobType)
                    .WithDescription(job.JobDes)
                    .WithIdentity(new JobKey(job.JobName, job.JobGroupName))
                    .StoreDurably()
                    .Build();
                ISet<ITrigger> triggerList = new HashSet<ITrigger>();
                for (int i = 0; i < job.TriggerName.Length; i++)
                {
                    bool isStartNow = false; //是否立即触发
                    if (job.IsStartNow != null && job.IsStartNow.Length > 0)
                    {
                        isStartNow = job.IsStartNow[i];
                    }
                    DateTime startTime = DateTime.MinValue; //触发开发时间
                    DateTime endTime = DateTime.MinValue; //触发结束时间
                    if (job.StartTime != null && job.StartTime.Length > 0)
                    {
                        DateTime.TryParse(job.StartTime[i], out startTime);
                    }
                    if (job.EndTime != null && job.EndTime.Length > 0)
                    {
                        DateTime.TryParse(job.EndTime[i], out endTime);
                    }
                    var builder = TriggerBuilder
                            .Create()
                            .ForJob(JobDetail)
                            .WithDescription(job.TriggerDes[i])
                            .WithIdentity(new TriggerKey(job.TriggerName[i], job.TriggerGroupName[i]));

                    if (isStartNow)
                    {
                        builder.StartNow();
                    }
                    if (startTime != DateTime.MinValue)
                    {
                        builder.StartAt(startTime);
                    }
                    if (endTime != DateTime.MinValue)
                    {
                        builder.EndAt(endTime);
                    }
                    ITrigger Trigger = builder.WithSchedule(CronScheduleBuilder.CronSchedule(job.TriggerCron[i])).Build();
                    triggerList.Add(Trigger);
                }
                System.Collections.Generic.IDictionary<IJobDetail, ISet<ITrigger>> triggersAndJob = new System.Collections.Generic.Dictionary<IJobDetail, ISet<ITrigger>>();
                triggersAndJob[JobDetail] = triggerList;
                _scheduler.ScheduleJobs(triggersAndJob, true); //添加到作业中
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public JobDetailsData GetJobDetailsData(string name, string group)
        {
            if (_scheduler == null) return null;
            if (_scheduler.IsShutdown)
            {
                return null;
            }
            IJobDetail job = null;
            try
            {
                job = _scheduler.GetJobDetail(new JobKey(name, group));
            }
            catch { }
            if (job == null)
            {
                return null;
            }
            var detailsData = new JobDetailsData
            {
                PrimaryData = GetJobData(name, group)
            };
            foreach (var key in job.JobDataMap.Keys)
            {
                detailsData.JobDataMap.Add(key, job.JobDataMap[key]);
            }
            detailsData.JobProperties.Add("Description", job.Description);
            detailsData.JobProperties.Add("Full name", job.JobType.FullName);
            detailsData.JobProperties.Add("Job type", job.JobType);
            detailsData.JobProperties.Add("Durable", job.Durable);

            return detailsData;
        }

        public SchedulerStatus GetSchedulerStatus(IScheduler scheduler)
        {
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

        public System.Collections.Generic.IList<TriggerData> GetTriggers(string jobName, string group)
        {
            var result = new System.Collections.Generic.List<TriggerData>();

            foreach (var trigger in _scheduler.GetTriggersOfJob(new JobKey(jobName, group)))
            {
                var data = new TriggerData(trigger.Key.Name, GetTriggerStatus(trigger, _scheduler))
                {
                    StartDate = trigger.StartTimeUtc,
                    EndDate = trigger.EndTimeUtc,
                    NextFireDate = trigger.GetNextFireTimeUtc(),
                    PreviousFireDate = trigger.GetPreviousFireTimeUtc(),
                    Group = trigger.Key.Group,
                    Priority = trigger.Priority,
                    Des = trigger.Description
                };
                result.Add(data);
            }

            return result;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroupName">任务组名</param>
        /// <returns></returns>
        public string DeleteJob(string jobName, string jobGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                bool rs = _scheduler.DeleteJob(new JobKey(jobName, jobGroupName));
                return rs ? string.Empty : "任务删除失败，原因未知";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 立即执行任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public string RunJobNow(string jobName, string jobGroupName)
        {
            try
            {
                if (_scheduler == null) return "IScheduler组件为空";
                _scheduler.TriggerJob(new JobKey(jobName, jobGroupName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region 私有方法

        private static ActivityStatus GetTriggerStatus(string triggerName, string triggerGroup, IScheduler scheduler)
        {
            var state = scheduler.GetTriggerState(new TriggerKey(triggerName, triggerGroup));
            switch (state)
            {
                case TriggerState.Paused:
                    return ActivityStatus.Paused;
                case TriggerState.Complete:
                    return ActivityStatus.Complete;
                default:
                    return ActivityStatus.Active;
            }
        }

        private static ActivityStatus GetTriggerStatus(ITrigger trigger, IScheduler scheduler)
        {
            return GetTriggerStatus(trigger.Key.Name, trigger.Key.Group, scheduler);
        }

        private static System.Collections.Generic.IList<TriggerGroupData> GetTriggerGroups(IScheduler scheduler)
        {
            var result = new System.Collections.Generic.List<TriggerGroupData>();
            if (!scheduler.IsShutdown)
            {
                foreach (var groupName in scheduler.GetTriggerGroupNames())
                {
                    var data = new TriggerGroupData(groupName);
                    data.Init();
                    result.Add(data);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取job groups
        /// </summary>
        /// <returns></returns>
        private System.Collections.Generic.IList<JobGroupData> GetJobGroups()
        {
            var result = new System.Collections.Generic.List<JobGroupData>();
            try
            {
                if (!_scheduler.IsShutdown)
                {
                    foreach (var groupName in _scheduler.GetJobGroupNames())
                    {
                        var groupData = new JobGroupData(
                            groupName,
                            GetJobs(groupName));
                        groupData.Init();
                        result.Add(groupData);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
            return result;
        }

        private System.Collections.Generic.IList<JobData> GetJobs(string groupName)
        {
            var result = new System.Collections.Generic.List<JobData>();

            foreach (var jobKey in _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)))
            {
                result.Add(GetJobData(jobKey.Name, groupName));
            }

            return result;
        }

        private JobData GetJobData(string jobName, string group)
        {
            var jobData = new JobData(jobName, group, GetTriggers(jobName, group));
            jobData.Init();
            return jobData;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        private void WriteLog(string msg)
        {
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "Logs";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string file = dir + string.Format("\\quartz_log_{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
                StreamWriter sw = new StreamWriter(file, true, Encoding.UTF8);
                sw.WriteLine(msg);
                sw.Close();
            }
            catch { }
        }

        #endregion
    }
}

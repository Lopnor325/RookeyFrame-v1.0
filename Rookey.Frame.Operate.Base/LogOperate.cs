using Rookey.Frame.Base;
using Rookey.Frame.Model.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rookey.Frame.Operate.Base
{
    /// <summary>
    /// 日志操作
    /// </summary>
    public static class LogOperate
    {
        #region 操作日志

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="operateUser">操作用户</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="opType">操作类型，如新增、修改、删除等</param>
        /// <param name="opContent">操作内容</param>
        /// <param name="IsOpSuccess">是否成功</param>
        /// <param name="opErrMsg">操作异常信息</param>
        public static void AddOperateLog(UserInfo operateUser, string moduleName, string opType, string opContent, bool IsOpSuccess, string opErrMsg)
        {
            try
            {
                if (operateUser == null || string.IsNullOrEmpty(moduleName))
                    return;
                //构建操作日志对象
                UserInfo admin = UserOperate.GetSuperAdmin();
                Log_Operate operateLog = new Log_Operate();
                operateLog.Id = Guid.NewGuid();
                operateLog.ModuleName = moduleName;
                operateLog.OperateType = opType;
                operateLog.OperateContent = opContent;
                operateLog.OperateResult = IsOpSuccess ? "操作成功" : "操作失败";
                operateLog.OperateTip = opErrMsg;
                operateLog.UserId = operateUser.UserName;
                operateLog.UserAlias = operateUser.AliasName;
                operateLog.OperateTime = DateTime.Now;
                operateLog.ClientIp = operateUser.ClientIP;
                operateLog.CreateDate = DateTime.Now;
                operateLog.CreateUserId = admin.UserId;
                operateLog.CreateUserName = admin.AliasName;
                operateLog.ModifyDate = DateTime.Now;
                operateLog.ModifyUserId = admin.UserId;
                operateLog.ModifyUserName = admin.AliasName;
                string errMsg = string.Empty;
                //保存日志
                CommonOperate.OperateRecord<Log_Operate>(operateLog, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
            }
            catch { }
        }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="operateUser">操作用户</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="opType">操作类型，如新增、修改、删除等</param>
        /// <param name="opContent">操作内容</param>
        /// <param name="IsOpSuccess">是否成功</param>
        /// <param name="opErrMsg">操作异常信息</param>
        public static void AddOperateLog(UserInfo operateUser, Guid moduleId, string opType, string opContent, bool IsOpSuccess, string opErrMsg)
        {
            string moduleName = SystemOperate.GetModuleNameById(moduleId);
            AddOperateLog(operateUser, moduleName, opType, opContent, IsOpSuccess, opErrMsg);
        }

        #endregion

        #region 异常日志

        /// <summary>
        /// 添加异常日志
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="paramObj">参数</param>
        /// <param name="controlOrClass">控制器或类名</param>
        /// <param name="actionName">方法名</param>
        /// <param name="currUser">当前用户</param>
        public static void AddExceptionLog(Exception ex, string paramObj, string controlOrClass = null, string actionName = null, UserInfo currUser = null)
        {
            if (ex == null) return;
            try
            {
                //构造对象
                Log_Exception exceptionLog = new Log_Exception();
                exceptionLog.ExceptionName = ex.GetType().Name;
                exceptionLog.ExceptionSource = ex.Source;
                exceptionLog.ExceptionMsg = ex.Message;
                exceptionLog.StackTrace = ex.StackTrace;
                exceptionLog.ExceptionTime = DateTime.Now;
                exceptionLog.ControllerName = controlOrClass;
                exceptionLog.ActionName = actionName;
                exceptionLog.CreateUserId = currUser != null ? currUser.UserId : (Guid?)null;
                exceptionLog.CreateUserName = UserInfo.GetUserAliasName(currUser);
                exceptionLog.CreateDate = DateTime.Now;
                exceptionLog.ModifyDate = DateTime.Now;
                exceptionLog.ModifyUserId = currUser != null ? currUser.UserId : (Guid?)null;
                exceptionLog.ModifyUserName = UserInfo.GetUserAliasName(currUser);
                exceptionLog.Id = Guid.NewGuid();
                //保存
                string errMsg = string.Empty;
                CommonOperate.OperateRecord<Log_Exception>(exceptionLog, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
            }
            catch { }
        }

        #endregion
    }
}

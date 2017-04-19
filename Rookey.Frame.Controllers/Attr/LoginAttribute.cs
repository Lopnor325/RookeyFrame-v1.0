/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Web.Mvc;
using Rookey.Frame.Common;
using Rookey.Frame.Model.Log;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Base;
using System.Reflection;
using System.Threading.Tasks;

namespace Rookey.Frame.Controllers.Attr
{
    /// <summary>
    /// 登录属性，记录登录信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LoginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 定义对象
        /// </summary>
        Log_Login loginLog = new Log_Login();

        /// <summary>
        /// 登录前
        /// </summary>
        /// <param name="filterContext">过滤上下文</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                UserInfo admin = UserOperate.GetSuperAdmin(); //获取管理员信息
                string username = filterContext.ActionParameters["username"] as string;
                loginLog.UserId = UserOperate.GetUserIdByUserName(username).ObjToStr();
                loginLog.LoginName = username;
                loginLog.LoginTime = DateTime.Now;
                loginLog.LoginIp = WebHelper.GetClientIP(filterContext.HttpContext.Request);
                loginLog.CreateUserId = admin.UserId; //添加人默认为空
                loginLog.CreateDate = DateTime.Now;
                loginLog.CreateUserName = admin.AliasName;
                loginLog.ModifyUserId = admin.UserId; //修改人默认为空
                loginLog.ModifyDate = DateTime.Now;
                loginLog.ModifyUserName = admin.AliasName;
            }
            catch { }
        }

        /// <summary>
        /// 登录后
        /// </summary>
        /// <param name="resultContext">结果上下文</param>
        public override void OnResultExecuted(ResultExecutedContext resultContext)
        {
            //异步处理
            Task.Factory.StartNew(() =>
            {
                try
                {
                    JsonResult result = resultContext.Result as JsonResult;
                    if (result != null && result.Data != null)
                    {
                        PropertyInfo pSuccess = result.Data.GetType().GetProperty("Success");
                        PropertyInfo pErrMsg = result.Data.GetType().GetProperty("Message");
                        bool loginSuccess = pSuccess.GetValue2(result.Data, null).ObjToBool();
                        loginLog.LoginStatus = loginSuccess ? "登录成功" : "登录失败";
                        loginLog.FailureReason = pErrMsg.GetValue2(result.Data, null).ObjToStr();
                    }
                    else
                    {
                        loginLog.LoginStatus = "登录失败";
                        loginLog.FailureReason = "无返回结果";
                    }
                    string errMsg = string.Empty;
                    long loginNum = CommonOperate.Count<Log_Login>(out errMsg, false, x => x.LoginName == loginLog.LoginName);
                    loginLog.LoginNum = (int)loginNum + 1;
                    loginLog.Id = Guid.NewGuid();
                    CommonOperate.OperateRecord<Log_Login>(loginLog, Operate.Base.OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                }
                catch { }
            });
        }
    }
}

using Rookey.Frame.Base;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    /// <summary>
    /// 用户操作事件处理
    /// </summary>
    public class Sys_UserOperateHandle : IModelOperateHandle<Sys_User>
    {
        /// <summary>
        /// 单个用户操作完成后
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="t">用户对象</param>
        /// <param name="result">操作结果</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="otherParams">其他参数</param>
        public void OperateCompeletedHandle(ModelRecordOperateType operateType, Sys_User t, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (result)
            {
                if (operateType == ModelRecordOperateType.Add)
                {
                    string pwd = string.Format("{0}_123456", t.UserName);
                    if (string.IsNullOrEmpty(t.PasswordHash))
                    {
                        //新增用户后初始化用户密码为 username+'_'+123456
                        string errMsg = string.Empty;
                        bool rs = UserOperate.ModifyPassword(t.Id, pwd, out errMsg);
                        if (rs)
                        {
                            new UserOperateHandleFactory().AfterRegiterUser(t.UserName, pwd, t.AliasName);
                        }
                    }
                    else
                    {
                        new UserOperateHandleFactory().AfterRegiterUser(t.UserName, pwd, t.AliasName);
                    }
                }
                if (operateType == ModelRecordOperateType.Del)
                {
                    new UserOperateHandleFactory().AfterDeleteUser(t.UserName);
                }
            }
        }

        /// <summary>
        /// 操作前验证
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="t">用户对象</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="otherParams">其他参数</param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, Sys_User t, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// 用户集合操作完成后事件
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="ts">用户对象集合</param>
        /// <param name="result">操作结果</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="otherParams">其他参数</param>
        public void OperateCompeletedHandles(ModelRecordOperateType operateType, List<Sys_User> ts, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (operateType == ModelRecordOperateType.Add && result)
            {
                //新增用户后初始化用户密码为 username+'_'+123456
                string errMsg = string.Empty;
                foreach (Sys_User t in ts)
                {
                    if (string.IsNullOrEmpty(t.PasswordHash))
                    {
                        string pwd = string.Format("{0}_123456", t.UserName);
                        bool rs = UserOperate.ModifyPassword(t.Id, pwd, out errMsg);
                        if (rs)
                        {
                            new UserOperateHandleFactory().AfterRegiterUser(t.UserName, pwd);
                        }
                    }
                }
            }
            if (operateType == ModelRecordOperateType.Del && result)
            {
                foreach (Sys_User t in ts)
                {
                    new UserOperateHandleFactory().AfterDeleteUser(t.UserName);
                }
            }
        }

        /// <summary>
        /// 操作前验证
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="ts">用户对象集合</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="otherParams">其他参数</param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<Sys_User> ts, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            return true;
        }
    }
}

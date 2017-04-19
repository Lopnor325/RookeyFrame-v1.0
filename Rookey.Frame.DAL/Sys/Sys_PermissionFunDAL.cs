using Rookey.Frame.Base;
using Rookey.Frame.Common;
using Rookey.Frame.DAL.Base;
using Rookey.Frame.IDAL.Sys;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Rookey.Frame.DAL.Sys
{
    /// <summary>
    /// 功能权限自定义数据层
    /// </summary>
    public class Sys_PermissionFunDAL : BaseDAL<Sys_PermissionFun>, ISys_PermissionFunDAL
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currUser">当前用户</param>
        public Sys_PermissionFunDAL(UserInfo currUser)
            : base(currUser, null)
        {
        }

        /// <summary>
        /// 获取角色功能权限，子角色默认继承父角色的所有权限，当前返回字典，
        /// 字典第一个参数为功能Id（菜单或按钮Id）,第二个参数为是否该功能Id继承父角色，
        /// 非当前角色所有
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="functionType">功能类型，菜单或网格按钮,0-菜单，1-按钮</param>
        /// <param name="moduleId">模块ID，针对按钮需要</param>
        /// <returns>返回功能Id、是否来自父角色的权限字典</returns>
        public Dictionary<Guid, bool> GetRoleFunPermissions(Guid roleId, int functionType, Guid? moduleId)
        {
            string errMsg = string.Empty;
            //取当前角色的功能权限
            Expression<Func<Sys_PermissionFun, bool>> exp = x => x.Sys_RoleId == roleId && x.FunType == functionType && x.IsDeleted == false;
            if (functionType == 1 && moduleId.HasValue)
            {
                string mId = moduleId.Value.ToString().ToUpper();
                Expression<Func<Sys_PermissionFun, bool>> tempExp = x => x.Des == mId;
                exp = ExpressionExtension.And<Sys_PermissionFun>(exp, tempExp);
            }
            List<Sys_PermissionFun> permissionFuns = this.GetEntities(out errMsg, exp, null, false);
            Dictionary<Guid, bool> dic = new Dictionary<Guid, bool>();
            if (permissionFuns != null && permissionFuns.Count > 0)
            {
                List<Guid> funIds = permissionFuns.Select(x => x.FunId).Distinct().ToList();
                dic = funIds.ToDictionary(x => x, y => false);
            }
            //取所有父角色的功能权限
            List<Guid> parentsRoleIds = ObjectReferenceClass.GetParentsRoleId(roleId); //获取所有父角色
            foreach (Guid tempRoleId in parentsRoleIds)
            {
                List<Sys_PermissionFun> tempFuns = this.GetEntities(out errMsg, x => x.Sys_RoleId == tempRoleId && x.FunType == functionType && !x.IsDeleted, null, false);
                if (tempFuns != null && tempFuns.Count > 0)
                {
                    List<Guid> funIds = tempFuns.Select(x => x.FunId).ToList();
                    foreach (Guid funId in funIds)
                    {
                        if (dic.Keys.Contains(funId))
                            continue;
                        dic.Add(funId, true);
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取用户的功能（菜单或网格按钮）权限，返回字典，
        /// 字典的第一个参数为功能Id（菜单或按钮Id），第二个
        /// 参数为是否来自父角色的权限，非当前角色权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="functionType">功能类型（菜单、网格按钮）,0-菜单，1-按钮</param>
        /// <param name="moduleId">模块ID，针对按钮需要</param>
        /// <returns></returns>
        public Dictionary<Guid, bool> GetUserFunPermissions(Guid userId, int functionType, Guid? moduleId)
        {
            Dictionary<Guid, bool> funIds = new Dictionary<Guid, bool>();
            //如果存在用户自定义权限则取自定义用户权限
            string errMsg = string.Empty;
            BaseDAL<Sys_UserPermissionFun> userPermissionDal = new BaseDAL<Sys_UserPermissionFun>();
            Expression<Func<Sys_UserPermissionFun, bool>> exp = x => x.Sys_UserId == userId && x.FunType == functionType && !x.IsDeleted;
            if (functionType == 1 && moduleId.HasValue)
            {
                string mId = moduleId.Value.ToString().ToUpper();
                Expression<Func<Sys_UserPermissionFun, bool>> tempExp = x => x.Des == mId;
                exp = ExpressionExtension.And(exp, tempExp);
            }
            List<Sys_UserPermissionFun> userFuns = userPermissionDal.GetEntities(out errMsg, exp, null, false);
            if (userFuns != null && userFuns.Count > 0)
            {
                List<Guid> tempFunIds = userFuns.Select(x => x.FunId).Distinct().ToList();
                foreach (Guid funId in tempFunIds)
                {
                    if (funIds.Keys.Contains(funId))
                        continue;
                    funIds.Add(funId, false);
                }
                return funIds;
            }
            //取用户角色权限
            List<Guid> roleIds = ObjectReferenceClass.GetUserRoleIds(userId);
            foreach (Guid roleId in roleIds)
            {
                Dictionary<Guid, bool> dic = GetRoleFunPermissions(roleId, functionType, moduleId);
                foreach (Guid funId in dic.Keys)
                {
                    if (funIds.ContainsKey(funId))
                        continue;
                    funIds.Add(funId, dic[funId]);
                }
            }
            return funIds;
        }
    }
}

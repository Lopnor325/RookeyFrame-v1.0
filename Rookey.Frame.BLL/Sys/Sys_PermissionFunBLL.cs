using Rookey.Frame.Base;
using Rookey.Frame.BLL.Base;
using Rookey.Frame.Bridge;
using Rookey.Frame.IBLL.Sys;
using Rookey.Frame.IDAL.Sys;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.BLL.Sys
{
    /// <summary>
    /// 功能权限自定义业务层
    /// </summary>
    public class Sys_PermissionFunBLL : BaseBLL<Sys_PermissionFun>, ISys_PermissionFunBLL
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currUser">当前用户</param>
        public Sys_PermissionFunBLL(UserInfo currUser)
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
        /// <param name="moduleId">模块ID,按钮时需要</param>
        /// <returns>返回功能Id、是否来自父角色的权限字典</returns>
        public Dictionary<Guid, bool> GetRoleFunPermissions(Guid roleId, int functionType, Guid? moduleId)
        {
            ISys_PermissionFunDAL permissionDal = BridgeObject.Resolve<ISys_PermissionFunDAL>(this.CurrUser);
            return permissionDal.GetRoleFunPermissions(roleId, functionType, moduleId);
        }

        /// <summary>
        /// 获取用户的功能（菜单或网格按钮）权限，返回字典，
        /// 字典的第一个参数为功能Id（菜单或按钮Id），第二个
        /// 参数为是否来自父角色的权限，非当前角色权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="functionType">功能类型（菜单、网格按钮）,0-菜单，1-按钮</param>
        /// <param name="moduleId">模块ID，按钮时需要</param>
        /// <returns></returns>
        public Dictionary<Guid, bool> GetUserFunPermissions(Guid userId, int functionType, Guid? moduleId)
        {
            ISys_PermissionFunDAL permissionDal = BridgeObject.Resolve<ISys_PermissionFunDAL>(this.CurrUser);
            return permissionDal.GetUserFunPermissions(userId, functionType, moduleId);
        }
    }
}

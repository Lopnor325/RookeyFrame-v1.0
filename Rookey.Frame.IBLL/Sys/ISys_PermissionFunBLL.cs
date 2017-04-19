using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.IBLL.Sys
{
    /// <summary>
    /// 功能权限自定义业务层接口
    /// </summary>
    public interface ISys_PermissionFunBLL
    {
        /// <summary>
        /// 获取角色功能权限，子角色默认继承父角色的所有权限，当前返回字典，
        /// 字典第一个参数为功能Id（菜单或按钮Id）,第二个参数为是否该功能Id继承父角色，
        /// 非当前角色所有
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="functionType">功能类型，菜单或网格按钮,0-菜单，1-按钮</param>
        /// <param name="moduleId">模块ID，针对按钮权限需要</param>
        /// <returns>返回功能Id、是否来自父角色的权限字典</returns>
        Dictionary<Guid, bool> GetRoleFunPermissions(Guid roleId, int functionType, Guid? moduleId);

        /// <summary>
        /// 获取用户的功能（菜单或网格按钮）权限，返回字典，
        /// 字典的第一个参数为功能Id（菜单或按钮Id），第二个
        /// 参数为是否来自父角色的权限，非当前角色权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="functionType">功能类型（菜单、网格按钮）,0-菜单，1-按钮</param>
        /// <param name="moduleId">模块ID，针对按钮权限需要</param>
        /// <returns></returns>
        Dictionary<Guid, bool> GetUserFunPermissions(Guid userId, int functionType, Guid? moduleId);
    }
}

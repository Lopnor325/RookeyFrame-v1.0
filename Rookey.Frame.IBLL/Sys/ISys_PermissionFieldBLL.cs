using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.IBLL.Sys
{
    /// <summary>
    /// 字段权限自定义业务层接口
    /// </summary>
    public interface ISys_PermissionFieldBLL
    {
        /// <summary>
        /// 获取角色（查看、新增、编辑）字段权限，返回允许（查看、新增、编辑）的字段名字典，
        /// 第一个参数为字段名，第二个参数为是否来自父角色的权限，
        /// 非当前角色权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldPermissionType">字段权限类型,0-查看，1-新增，2-编辑</param>
        /// <param name="read">是否为应用读取，应用读取快速返回</param>
        /// <returns>返回允许（查看、新增、编辑）的字段名字典</returns>
        Dictionary<string, bool> GetRoleFieldPermissions(Guid roleId, Guid moduleId, int fieldPermissionType, bool read = false);

        /// <summary>
        /// 获取用户（查看、新增、编辑）字段权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldPermissionType">字段权限类型，0-查看，1-新增，2-编辑</param>
        /// <returns>返回允许（查看、新增、编辑）的字段名集合</returns>
        List<string> GetUserFieldsPermissions(Guid userId, Guid moduleId, int fieldPermissionType);
    }
}

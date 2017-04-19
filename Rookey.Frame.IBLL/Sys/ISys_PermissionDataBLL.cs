using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.IBLL.Sys
{
    /// <summary>
    /// 数据权限自定义业务层接口
    /// </summary>
    public interface ISys_PermissionDataBLL
    {
        /// <summary>
        /// 获取角色可（查看、编辑、删除）模块中哪些组织下的数据，返回可（查看、编辑、删除）
        /// 的组织Id集合字典，
        /// 第一个参数为组织Id，第二个参数为是否来自父角色的权限，非当前
        /// 角色的权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="dataPermissionType">数据权限类型,0-查看，1-编辑，2-删除</param>
        /// <param name="read">是否为应用读取</param>
        /// <returns>返回组织Id集合字典</returns>
        Dictionary<string, bool> GetRoleDataPermissions(Guid roleId, Guid moduleId, int dataPermissionType, bool read = false);

        /// <summary>
        /// 获取用户可（查看、编辑、删除）模块中哪些组织下的数据，
        /// 返回可（查看、编辑、删除）的组织Id集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="dataPermissionType">数据权限类型,0-查看，1-编辑，2-删除</param>
        /// <returns>返回组织Id集合字典</returns>
        List<string> GetUserDataPermissions(Guid userId, Guid moduleId, int dataPermissionType);
    }
}

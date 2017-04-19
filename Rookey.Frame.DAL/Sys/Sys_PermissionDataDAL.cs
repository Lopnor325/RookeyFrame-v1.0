using Rookey.Frame.DAL.Base;
using Rookey.Frame.IDAL.Sys;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Rookey.Frame.EntityBase;
using Rookey.Frame.Base;
using Rookey.Frame.Base.User;
using Rookey.Frame.Model.OrgM;

namespace Rookey.Frame.DAL.Sys
{
    /// <summary>
    /// 对象处理相关
    /// </summary>
    internal static class ObjectReferenceClass
    {
        #region 角色相关

        /// <summary>
        /// 获取父角色Id
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        private static Guid GetParentRoleId(Guid roleId)
        {
            if (roleId == Guid.Empty) return Guid.Empty;
            string errMsg = string.Empty;
            BaseDAL<Sys_Role> dal = new BaseDAL<Sys_Role>();
            Sys_Role role = dal.GetEntityById(out errMsg, roleId);
            if (role != null && role.ParentId.HasValue)
                return role.ParentId.Value;
            return Guid.Empty;
        }

        /// <summary>
        /// 获取所有祖宗角色Id，包含父角色Id，祖父角色Id，。。
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public static List<Guid> GetParentsRoleId(Guid roleId)
        {
            List<Guid> list = new List<Guid>();
            Guid parentId = GetParentRoleId(roleId);
            if (parentId != Guid.Empty) //存在父角色
            {
                list.Add(parentId); //将父角色添加到列表
                list.AddRange(GetParentsRoleId(parentId));
            }
            return list;
        }

        /// <summary>
        /// 获取用户角色Id，包含父角色
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public static List<Guid> GetUserRoleIds(Guid userId)
        {
            List<Guid> list = new List<Guid>();
            object instance = null;
            object rolesObj = Globals.ExecuteReflectMethod("Rookey.Frame.Operate.Base", "PermissionOperate", "GetUserRoles", new object[] { userId, true }, ref instance, true);
            List<Sys_Role> roles = rolesObj as List<Sys_Role>;
            if (roles != null && roles.Count > 0)
                list = roles.Select(x => x.Id).ToList();
            return list;
        }

        #endregion

        #region 菜单相关

        /// <summary>
        /// 获取父菜单Id
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        private static Guid GetParentMenuId(Guid menuId, UserInfo currUser = null)
        {
            if (menuId == Guid.Empty) return Guid.Empty;
            string errMsg = string.Empty;
            BaseDAL<Sys_Menu> dal = new BaseDAL<Sys_Menu>(currUser);
            Sys_Menu menu = dal.GetEntityById(out errMsg, menuId);
            if (menu != null && menu.ParentId.HasValue)
                return menu.ParentId.Value;
            return Guid.Empty;
        }

        /// <summary>
        /// 获取所有祖宗菜单Id，包含父菜单Id，祖父菜单Id，。。
        /// </summary>
        /// <param name="menuId">菜单Id</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static List<Guid> GetParentsMenuId(Guid menuId, UserInfo currUser = null)
        {
            List<Guid> list = new List<Guid>();
            Guid parentId = GetParentMenuId(menuId, currUser);
            if (parentId != Guid.Empty) //存在父菜单
            {
                list.Add(parentId); //将父菜单添加到列表
                list.AddRange(GetParentsMenuId(parentId, currUser));
            }
            return list;
        }

        #endregion

        #region 用户上下级

        /// <summary>
        /// 获取所有下级人员用户ID集合
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static List<Guid?> GetAllInferiorPerson(UserInfo currUser)
        {
            List<Guid?> list = new List<Guid?>();
            if (currUser == null || !currUser.EmpId.HasValue)
                return list;
            object instance = null;
            object objs = Globals.ExecuteReflectMethod("Rookey.Frame.Operate.Base", "OrgMOperate", "GetEmpChildUserIds", new object[] { currUser.EmpId.Value, false, null, null }, ref instance, true);
            if (objs == null)
                return list;
            List<Guid> tempList = objs as List<Guid>;
            list = tempList.Select(x => (Guid?)x).ToList();
            return list;
        }

        #endregion
    }

    /// <summary>
    /// 数据权限自定义数据层
    /// </summary>
    public class Sys_PermissionDataDAL : BaseDAL<Sys_PermissionData>, ISys_PermissionDataDAL
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currUser">当前用户</param>
        public Sys_PermissionDataDAL(UserInfo currUser)
            : base(currUser, null)
        {
        }

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
        public Dictionary<string, bool> GetRoleDataPermissions(Guid roleId, Guid moduleId, int dataPermissionType, bool read = false)
        {
            string errMsg = string.Empty;
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            //取当前角色的数据权限
            Sys_PermissionData permissionData = this.GetEntity(out  errMsg, x => x.Sys_RoleId == roleId && x.Sys_ModuleId == moduleId && !x.IsDeleted);
            string orgIdsStr = string.Empty;
            if (permissionData != null)
            {
                orgIdsStr = dataPermissionType == 0 ? permissionData.CanViewOrgIds :
                    (dataPermissionType == 1 ? permissionData.CanEditOrgIds : permissionData.CanDelOrgIds);
            }
            if (!string.IsNullOrWhiteSpace(orgIdsStr))
            {
                string[] token = orgIdsStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (read && token.Contains("-1"))
                {
                    dic.Add("-1", false);
                    return dic;
                }
                foreach (string orgId in token)
                {
                    dic.Add(orgId, false);
                }
            }
            //取父角色的数据权限
            List<Guid> parentsRoleIds = ObjectReferenceClass.GetParentsRoleId(roleId); //获取所有父角色
            foreach (Guid tempRoleId in parentsRoleIds)
            {
                Sys_PermissionData tempPermission = this.GetEntity(out errMsg, x => x.Sys_RoleId == tempRoleId && x.Sys_ModuleId == moduleId && !x.IsDeleted);
                string tempOrgIdsStr = string.Empty;
                if (tempPermission != null)
                {
                    tempOrgIdsStr = dataPermissionType == 0 ? tempPermission.CanViewOrgIds :
                        (dataPermissionType == 1 ? tempPermission.CanEditOrgIds : tempPermission.CanDelOrgIds);
                }
                if (!string.IsNullOrWhiteSpace(tempOrgIdsStr))
                {
                    string[] token = tempOrgIdsStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (read && token.Contains("-1"))
                    {
                        dic.Add("-1", false);
                        return dic;
                    }
                    foreach (string orgId in token)
                    {
                        if (dic.ContainsKey(orgId))
                            continue;
                        dic.Add(orgId, true);
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取用户可（查看、编辑、删除）模块中哪些组织下的数据，
        /// 返回可（查看、编辑、删除）的组织Id集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="dataPermissionType">数据权限类型,0-查看，1-编辑，2-删除</param>
        /// <returns>返回组织Id集合字典</returns>
        public List<string> GetUserDataPermissions(Guid userId, Guid moduleId, int dataPermissionType)
        {
            List<string> orgIds = new List<string>();
            //如果存在用户自定义权限则取自定义用户权限
            string errMsg = string.Empty;
            BaseDAL<Sys_UserPermissionData> userPermissionDal = new BaseDAL<Sys_UserPermissionData>(this.CurrUser);
            Sys_UserPermissionData permissionData = userPermissionDal.GetEntity(out errMsg, x => x.Sys_UserId == userId && x.Sys_ModuleId == moduleId && !x.IsDeleted);
            string orgIdsStr = string.Empty;
            if (permissionData != null)
            {
                orgIdsStr = dataPermissionType == 0 ? permissionData.CanViewOrgIds :
                    (dataPermissionType == 1 ? permissionData.CanEditOrgIds : permissionData.CanDelOrgIds);
            }
            if (!string.IsNullOrWhiteSpace(orgIdsStr))
            {
                string[] token = orgIdsStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (token.Contains("-1"))
                    return new List<string>() { "-1" };
                foreach (string orgId in token)
                {
                    if (!orgIds.Contains(orgId))
                        orgIds.Add(orgId);
                }
                return orgIds;
            }
            //取用户角色权限
            List<Guid> roleIds = ObjectReferenceClass.GetUserRoleIds(userId);
            foreach (Guid roleId in roleIds)
            {
                Dictionary<string, bool> tempOrgIds = GetRoleDataPermissions(roleId, moduleId, dataPermissionType, true);
                if (tempOrgIds.ContainsKey("-1")) return new List<string>() { "-1" };
                foreach (string orgId in tempOrgIds.Keys)
                {
                    if (!orgIds.Contains(orgId))
                        orgIds.Add(orgId);
                }
            }
            return orgIds;
        }
    }
}

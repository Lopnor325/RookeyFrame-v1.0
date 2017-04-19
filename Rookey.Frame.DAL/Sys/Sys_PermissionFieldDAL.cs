using Rookey.Frame.Base;
using Rookey.Frame.DAL.Base;
using Rookey.Frame.IDAL.Sys;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.DAL.Sys
{
    /// <summary>
    /// 字段权限自定义数据层
    /// </summary>
    public class Sys_PermissionFieldDAL : BaseDAL<Sys_PermissionField>, ISys_PermissionFieldDAL
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currUser">当前用户</param>
        public Sys_PermissionFieldDAL(UserInfo currUser)
            : base(currUser, null)
        {
        }

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
        public Dictionary<string, bool> GetRoleFieldPermissions(Guid roleId, Guid moduleId, int fieldPermissionType, bool read = false)
        {
            string errMsg = string.Empty;
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            //取当前角色的字段权限
            Sys_PermissionField permissionField = this.GetEntity(out errMsg, x => x.Sys_RoleId == roleId && x.Sys_ModuleId == moduleId && !x.IsDeleted);
            string fields = string.Empty;
            if (permissionField != null)
            {
                fields = fieldPermissionType == 0 ? permissionField.CanViewFields :
                    (fieldPermissionType == 1 ? permissionField.CanAddFields : permissionField.CanEditFields);
            }
            if (!string.IsNullOrWhiteSpace(fields))
            {
                string[] token = fields.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (read && token.Contains("-1")) //应用读取
                {
                    dic.Add("-1", false);
                    return dic;
                }
                foreach (string field in token)
                {
                    dic.Add(field, false);
                }
            }
            //取父角色的字段权限
            List<Guid> parentsRoleIds = ObjectReferenceClass.GetParentsRoleId(roleId); //获取所有父角色
            foreach (Guid tempRoleId in parentsRoleIds)
            {
                Sys_PermissionField tempPermission = this.GetEntity(out errMsg, x => x.Sys_RoleId == tempRoleId && x.Sys_ModuleId == moduleId && !x.IsDeleted);
                string tempFields = string.Empty;
                if (tempPermission != null)
                {
                    tempFields = fieldPermissionType == 0 ? tempPermission.CanViewFields :
                        (fieldPermissionType == 1 ? tempPermission.CanAddFields : tempPermission.CanEditFields);
                }
                if (!string.IsNullOrWhiteSpace(tempFields))
                {
                    string[] token = tempFields.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string field in token)
                    {
                        if (dic.ContainsKey(field))
                            continue;
                        dic.Add(field, true);
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取用户（查看、新增、编辑）字段权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldPermissionType">字段权限类型，0-查看，1-新增，2-编辑</param>
        /// <returns>返回允许（查看、新增、编辑）的字段名集合</returns>
        public List<string> GetUserFieldsPermissions(Guid userId, Guid moduleId, int fieldPermissionType)
        {
            List<string> fieldNames = new List<string>();
            //如果存在用户自定义权限则取自定义用户权限
            string errMsg = string.Empty;
            BaseDAL<Sys_UserPermissionField> userPermissionDal = new BaseDAL<Sys_UserPermissionField>();
            Sys_UserPermissionField permissionField = userPermissionDal.GetEntity(out errMsg, x => x.Sys_UserId == userId && x.Sys_ModuleId == moduleId && !x.IsDeleted);
            string fields = string.Empty;
            if (permissionField != null)
            {
                fields = fieldPermissionType == 0 ? permissionField.CanViewFields :
                    (fieldPermissionType == 1 ? permissionField.CanAddFields : permissionField.CanEditFields);
            }
            if (!string.IsNullOrWhiteSpace(fields))
            {
                string[] token = fields.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (token.Contains("-1"))
                    return new List<string>() { "-1" }; //所有字段权限
                foreach (string field in token)
                {
                    if (!fieldNames.Contains(field))
                        fieldNames.Add(field);
                }
                return fieldNames;
            }
            //取用户角色权限
            List<Guid> roleIds = ObjectReferenceClass.GetUserRoleIds(userId);
            foreach (Guid roleId in roleIds)
            {
                Dictionary<string, bool> tempFields = GetRoleFieldPermissions(roleId, moduleId, fieldPermissionType, true);
                if (tempFields.ContainsKey("-1")) return new List<string>() { "-1" }; //所有字段权限
                foreach (string field in tempFields.Keys)
                {
                    if (!fieldNames.Contains(field))
                        fieldNames.Add(field);
                }
            }
            return fieldNames;
        }
    }
}

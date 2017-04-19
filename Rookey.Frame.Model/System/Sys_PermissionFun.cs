using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 角色功能权限
    /// </summary>
    [NoModule]
    public class Sys_PermissionFun:BaseSysEntity
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [NoField]
        public Guid? Sys_RoleId { get; set; }

        /// <summary>
        /// 功能Id（如果FunType为菜单类型，则FunId为菜单Id,如果FunType为按钮类型，则FunId为按钮Id）
        /// </summary>
        [NoField]
        public Guid FunId { get; set; }

        /// <summary>
        /// 功能类型
        /// </summary>
        [NoField]
        public int FunType { get; set; }

        /// <summary>
        /// 功能类型（枚举类型）
        /// </summary>
        [Ignore]
        public FunctionTypeEnum FunTypeOfEnum
        {
            get
            {
                return (FunctionTypeEnum)Enum.Parse(typeof(FunctionTypeEnum), FunType.ToString());
            }
            set { FunType = (int)value; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        [NoField]
        public string Des { get; set; }
    }
}

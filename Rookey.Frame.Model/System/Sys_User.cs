/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [ModuleConfig(Name = "用户管理", Sort = 0, TitleKey = "UserName", PrimaryKeyFields = "UserName", StandardJsFolder = "System")]
    public class Sys_User : BaseSysEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [FieldConfig(Display = "用户名", RowNum = 1, ColNum = 1, IsUnique = true, IsRequired = true, HeadSort = 1)]
        [StringLength(30)]
        public string UserName { get; set; }

        /// <summary>
        /// 用户别名
        /// </summary>
        [FieldConfig(Display = "用户别名", RowNum = 1, ColNum = 2, HeadSort = 2)]
        [StringLength(30)]
        public string AliasName { get; set; }

        /// <summary>
        /// 组织Id
        /// </summary>
        [FieldConfig(Display = "所属组织", ControlType = (int)ControlTypeEnum.ComboTree, RowNum = 2, ColNum = 1, HeadSort = 3, ForeignModuleName = "组织管理")]
        public Guid? Sys_OrganizationId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        [Ignore]
        public string Sys_OrganizationName { get; set; }

        /// <summary>
        /// 通过PasswordSalt混淆加密后的密码散列码
        /// </summary>
        [FieldConfig(Display = "密码", IsEnableForm = false, IsAllowGridSearch = false, IsGridVisible = false)]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        /// <summary>
        /// 混淆码
        /// </summary>
        [StringLength(100)]
        [FieldConfig(Display = "混淆码", IsGridVisible = false, IsAllowGridSearch = false, IsEnableForm = false)]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// 密保问题
        /// </summary>
        [FieldConfig(Display = "密保问题", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 2, ColNum = 2, HeadSort = 4)]
        [StringLength(200)]
        public string PwdProtectQuest { get; set; }

        /// <summary>
        /// 密保答案
        /// </summary>
        [FieldConfig(Display = "密保答案", RowNum = 3, ColNum = 1, HeadSort = 5)]
        [StringLength(200)]
        public string PwdProtectAnswer { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [FieldConfig(Display = "是否激活", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 3, ColNum = 2, HeadSort = 6)]
        public bool IsActivated { get; set; }

        /// <summary>
        /// 激活时间
        /// </summary>
        [FieldConfig(Display = "激活时间", ControlType = (int)ControlTypeEnum.DateTimeBox, IsAllowAdd = false, IsAllowEdit = false, RowNum = 4, ColNum = 1, HeadSort = 7)]
        public DateTime? ActivatedDate { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldConfig(Display = "是否有效", ControlType = (int)ControlTypeEnum.SingleCheckBox, DefaultValue = "true", RowNum = 4, ColNum = 2, HeadSort = 8)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 失效时间
        /// </summary>
        [FieldConfig(Display = "失效时间", ControlType = (int)ControlTypeEnum.DateTimeBox, RowNum = 5, ColNum = 1, HeadSort = 9)]
        public DateTime? ExpiryDate { get; set; }
    }
}

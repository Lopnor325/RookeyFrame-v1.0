/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase.Attr;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.EntityBase
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseEntity()
        {
            Id = Guid.Empty;
            CreateDate = DateTime.Now;
            ModifyDate = DateTime.Now;
            IsDeleted = false;
            IsDraft = false;
        }

        /// <summary>
        /// ID
        /// </summary>
        [FieldConfig(Display = "ID", IsEnableForm = false, IsAllowGridSearch = false, HeadWidth = 60, HeadSort = 0)]
        public Guid Id { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [FieldConfig(Display = "创建人", HeadWidth = 80, IsEnableForm = false, IsAllowEdit = false, IsAllowCopy = false, IsAllowBatchEdit = false, HeadSort = 1001, ForeignModuleName = "用户管理")]
        public Guid? CreateUserId { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        [FieldConfig(Display = "修改人", HeadWidth = 80, IsEnableForm = false, HeadSort = 1002, ForeignModuleName = "用户管理")]
        public Guid? ModifyUserId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [NoField]
        [StringLength(30)]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [NoField]
        [StringLength(30)]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [FieldConfig(Display = "创建日期", ControlType = (int)ControlTypeEnum.DateTimeBox, IsEnableForm = false, IsAllowEdit = false, IsAllowCopy = false, IsAllowBatchEdit = false, HeadWidth = 150, HeadSort = 1005)]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        [FieldConfig(Display = "修改日期", ControlType = (int)ControlTypeEnum.DateTimeBox, IsEnableForm = false, HeadWidth = 150, HeadSort = 1006)]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [NoField]
        [Default(typeof(bool), "0")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [NoField]
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 是否草稿
        /// </summary>
        [NoField]
        [Default(typeof(bool), "0")]
        public bool IsDraft { get; set; }

        /// <summary>
        /// 单据所属组织
        /// </summary>
        [NoField]
        public Guid? OrgId { get; set; }

        /// <summary>
        /// 自增ID，排序使用
        /// </summary>
        [NoField]
        [AutoIncrement]
        public long AutoIncrmId { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [NoField]
        public int? FlowStatus { get; set; }

        /// <summary>
        /// 是否已经加载外键Name字段值
        /// </summary>
        [Ignore]
        public bool HasLoadForeignNameValue { get; set; }

        /// <summary>
        /// 当前记录对应的待办ID
        /// </summary>
        [Ignore]
        public Guid? TaskToDoId { get; set; }
    }
}

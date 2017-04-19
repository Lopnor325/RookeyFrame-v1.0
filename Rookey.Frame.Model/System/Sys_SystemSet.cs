/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using System;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 参数设定，系统或用户参数设置
    /// </summary>
    [ModuleConfig(Name = "参数设定", PrimaryKeyFields = "Name", TitleKey = "Name", Sort = 25, StandardJsFolder = "System")]
    public class Sys_SystemSet : BaseSysEntity
    {
        /// <summary>
        /// 设置名称
        /// </summary>
        [FieldConfig(Display = "名称", ControlWidth = 490, IsFrozen = true, IsAllowEdit = false, RowNum = 1, ColNum = 1, IsRequired = true, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 设置值
        /// </summary>
        [FieldConfig(Display = "值", ControlWidth = 490, RowNum = 2, ColNum = 1, HeadSort = 2)]
        [StringLength(4000)]
        public string Value { get; set; }

        /// <summary>
        /// 设置类型
        /// </summary>
        [FieldConfig(Display = "类型", IsRequired = true, ControlWidth = 490, ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 3, ColNum = 1, HeadSort = 3)]
        public int SetType { get; set; }

        /// <summary>
        /// 设置类型（枚举）
        /// </summary>
        [Ignore]
        public SystemSetTypeEnum SetTypeOfEnum
        {
            get
            {
                return (SystemSetTypeEnum)Enum.Parse(typeof(SystemSetTypeEnum), SetType.ToString());
            }
            set { SetType = (int)value; }
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        [FieldConfig(Display = "用户", ControlWidth = 490, RowNum = 4, ColNum = 1, ControlType = (int)ControlTypeEnum.DialogGrid, HeadSort = 4, ForeignModuleName = "用户管理")]
        public Guid? Sys_UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Ignore]
        public string Sys_UserName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [FieldConfig(Display = "描述", ControlWidth = 490, RowNum = 5, ColNum = 1, HeadSort = 5)]
        [StringLength(500)]
        public string Des { get; set; }
    }
}

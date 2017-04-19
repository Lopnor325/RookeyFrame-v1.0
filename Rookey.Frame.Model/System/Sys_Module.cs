/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using Rookey.Frame.Model.EnumSpace;
using ServiceStack.DataAnnotations;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.EntityBase;

namespace Rookey.Frame.Model.Sys
{
    /// <summary>
    /// 模块管理
    /// </summary>
    [ModuleConfig(Name = "模块管理", Sort = 3, PrimaryKeyFields = "Name", TitleKey = "Name", ModuleEditMode = (int)ModuleEditModeEnum.TabFormEdit, StandardJsFolder = "System")]
    public class Sys_Module : BaseSysEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Sys_Module()
        {
            Sort = 0;
            FormAttachDisplayStyle = 0;
            DataSourceType = 0;
            IsMutiSelect = true;
        }

        #region 基本信息

        /// <summary>
        /// 模块名称
        /// </summary>
        [FieldConfig(Display = "模块名称", GroupName = "基本信息", RowNum = 1, ColNum = 1, IsFrozen = true, IsAllowEdit = false, IsUnique = true, IsRequired = true, HeadSort = 1)]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [FieldConfig(Display = "显示名称", GroupName = "基本信息", RowNum = 1, ColNum = 2, IsFrozen = true, DefaultValue = "{Name}", HeadSort = 2)]
        [StringLength(100)]
        public string Display { get; set; }

        /// <summary>
        /// 模块对应的物理表名或视图名
        /// </summary>
        [FieldConfig(Display = "模块表名", GroupName = "基本信息", RowNum = 2, ColNum = 1, IsFrozen = true, IsAllowEdit = false, IsRequired = true, IsUnique = true, DefaultValue = "{Name}", HeadSort = 3)]
        [StringLength(100)]
        public string TableName { get; set; }

        /// <summary>
        /// 数据来源类型
        /// </summary>
        [FieldConfig(Display = "数据来源类型", IsEnableForm = false, HeadSort = 2)]
        public int DataSourceType { get; set; }

        /// <summary>
        /// 数据来源类型（枚举）
        /// </summary>
        [Ignore]
        public ModuleDataSourceType DataSourceTypeOfEnum
        {
            get
            {
                return (ModuleDataSourceType)Enum.Parse(typeof(ModuleDataSourceType), DataSourceType.ToString());
            }
            set { DataSourceType = (int)value; }
        }

        /// <summary>
        /// 主键字段，多个字段以逗号分隔
        /// </summary>
        [FieldConfig(Display = "主键字段", GroupName = "基本信息", RowNum = 2, ColNum = 2, HeadSort = 4)]
        [StringLength(100)]
        public string PrimaryKeyFields { get; set; }

        /// <summary>
        /// 模块标题列
        /// </summary>
        [FieldConfig(Display = "标记字段", GroupName = "基本信息", RowNum = 3, ColNum = 1, HeadSort = 5)]
        [StringLength(100)]
        public string TitleKey { get; set; }

        /// <summary>
        /// 父模块Id
        /// </summary>
        [FieldConfig(Display = "父模块", GroupName = "基本信息", RowNum = 3, ColNum = 2, ControlType = (int)ControlTypeEnum.DialogGrid, HeadSort = 6, ForeignModuleName = "模块管理")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 父模块名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 模块图标
        /// </summary>
        [FieldConfig(Display = "模块图标", ControlType = (int)ControlTypeEnum.IconBox, GroupName = "基本信息", RowNum = 4, ColNum = 1, IsAllowGridSearch = false, HeadSort = 7)]
        [StringLength(100)]
        public string ModuleLogo { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [FieldConfig(Display = "排序", GroupName = "基本信息", ControlType = (int)ControlTypeEnum.IntegerBox, RowNum = 4, ColNum = 2, HeadSort = 8)]
        public int Sort { get; set; }

        #endregion

        #region 模块控制

        /// <summary>
        /// 是否允许新增
        /// </summary>
        [FieldConfig(Display = "允许新增", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 5, ColNum = 1, HeadSort = 9)]
        public bool IsAllowAdd { get; set; }

        /// <summary>
        /// 是否允许编辑
        /// </summary>
        [FieldConfig(Display = "允许编辑", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 5, ColNum = 2, HeadSort = 10)]
        public bool IsAllowEdit { get; set; }

        /// <summary>
        /// 是否允许删除
        /// </summary>
        [FieldConfig(Display = "允许删除", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 6, ColNum = 1, HeadSort = 11)]
        public bool IsAllowDelete { get; set; }

        /// <summary>
        /// 是否允许复制
        /// </summary>
        [FieldConfig(Display = "允许复制", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 6, ColNum = 2, HeadSort = 12)]
        public bool IsAllowCopy { get; set; }

        /// <summary>
        /// 是否允许导入
        /// </summary>
        [FieldConfig(Display = "允许导入", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 7, ColNum = 1, HeadSort = 13)]
        public bool IsAllowImport { get; set; }

        /// <summary>
        /// 是否允许导出
        /// </summary>
        [FieldConfig(Display = "允许导出", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 7, ColNum = 2, HeadSort = 14)]
        public bool IsAllowExport { get; set; }

        /// <summary>
        /// 是否启用附件
        /// </summary>
        [FieldConfig(Display = "启用附件", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 8, ColNum = 1, HeadSort = 15)]
        public bool IsEnableAttachment { get; set; }

        /// <summary>
        /// 是否启用回收站
        /// </summary>
        [FieldConfig(Display = "启用回收站", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 8, ColNum = 2, HeadSort = 16)]
        public bool IsEnabledRecycle { get; set; }

        /// <summary>
        /// 是否启用批量编辑
        /// </summary>
        [FieldConfig(Display = "启用批量编辑", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 9, ColNum = 1, HeadSort = 17)]
        public bool IsEnabledBatchEdit { get; set; }

        /// <summary>
        /// 启用打印
        /// </summary>
        [FieldConfig(Display = "启用打印", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 9, ColNum = 2, HeadSort = 18)]
        public bool IsEnabledPrint { get; set; }

        /// <summary>
        /// 是否启用草稿
        /// </summary>
        [FieldConfig(Display = "启用草稿", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 10, ColNum = 1, HeadSort = 19)]
        public bool IsEnabledDraft { get; set; }

        /// <summary>
        /// 是否启用多选
        /// </summary>
        [FieldConfig(Display = "启用多选", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 10, ColNum = 2, HeadSort = 20)]
        public bool IsMutiSelect { get; set; }

        /// <summary>
        /// 是否启用编码规则
        /// </summary>
        [FieldConfig(Display = "启用编码规则", GroupName = "模块控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 11, ColNum = 1, HeadSort = 21)]
        public bool IsEnableCodeRule { get; set; }

        /// <summary>
        /// 是否自定义模块
        /// </summary>
        [FieldConfig(Display = "是否自定义模块", IsEnableForm = false, HeadSort = 22)]
        public bool IsCustomerModule { get; set; }

        #endregion

        #region 前端控制

        /// <summary>
        /// 标准JS所属文件夹
        /// </summary>
        [FieldConfig(Display = "模块JS文件夹", GroupName = "前端控制", RowNum = 12, ColNum = 1, HeadSort = 23)]
        [StringLength(100)]
        public string StandardJsFolder { get; set; }

        /// <summary>
        /// 明细在顶部显示，针对编辑或查看页面
        /// </summary>
        [FieldConfig(Display = "明细顶部显示", NullTipText = "针对编辑或查看页面，明细在是否在顶部与主信息平行标签页显示", GroupName = "前端控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 12, ColNum = 2, HeadSort = 24)]
        public bool DetailTopDisplay { get; set; }

        /// <summary>
        /// 网格展开明细，针对网格页面
        /// </summary>
        [FieldConfig(Display = "网格展开明细", NullTipText = "针对网格页面，是否网格内展开查看明细分页数据", GroupName = "前端控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 13, ColNum = 1, HeadSort = 25)]
        public bool DetailInGrid { get; set; }

        /// <summary>
        /// 附属模块在顶部显示，针对查看页面
        /// </summary>
        [FieldConfig(Display = "附属模块在顶部显示", NullTipText = "在记录查看页面附属模块是否在顶部显示", GroupName = "前端控制", ControlType = (int)ControlTypeEnum.SingleCheckBox, RowNum = 13, ColNum = 2, HeadSort = 26)]
        public bool AttachModuleTopDisplay { get; set; }

        /// <summary>
        /// 附件显示方式，针对编辑可查看页面
        /// </summary>
        [FieldConfig(Display = "附件显示方式", GroupName = "前端控制", ControlType = (int)ControlTypeEnum.ComboBox, RowNum = 14, ColNum = 1, HeadSort = 27)]
        public int? FormAttachDisplayStyle { get; set; }

        /// <summary>
        /// 附件显示方式（枚举）
        /// </summary>
        [Ignore]
        public FormAttachDisplayStyleEnum FormAttachDisplayStyleOfEnum
        {
            get
            {
                if (!FormAttachDisplayStyle.HasValue) return FormAttachDisplayStyleEnum.SimpleStyle;
                return (FormAttachDisplayStyleEnum)Enum.Parse(typeof(FormAttachDisplayStyleEnum), FormAttachDisplayStyle.Value.ToString());
            }
            set { FormAttachDisplayStyle = (int)value; }
        }

        #endregion

        /// <summary>
        /// TitleKey显示名称
        /// </summary>
        [Ignore]
        public string TitleKeyDisplay { get; set; }
    }
}

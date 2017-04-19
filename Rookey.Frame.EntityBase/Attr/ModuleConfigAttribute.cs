/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;

namespace Rookey.Frame.EntityBase.Attr
{
    /// <summary>
    /// 模块配置属性类
    /// </summary>
    public class ModuleConfigAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModuleConfigAttribute()
        {
            Sort = 100;
            IsAllowAdd = true;
            IsAllowEdit = true;
            IsAllowDelete = true;
            IsEnabledBatchEdit = false;
            IsEnabledRecycle = true;
            IsEnabledPrint = false;
            IsAllowCopy = true;
            IsAllowImport = false;
            IsAllowExport = false;
            ModuleEditMode = 0;
        }

        /// <summary>
        /// 数据表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据来源类型
        /// </summary>
        public int DataSourceType { get; set; }

        /// <summary>
        /// 父模块名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模块标题列
        /// </summary>
        public string TitleKey { get; set; }

        /// <summary>
        /// 主键字段，记录唯一标识，可以是多个字段
        /// </summary>
        public string PrimaryKeyFields { get; set; }

        /// <summary>
        /// 模块图标
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 是否允许新增
        /// </summary>
        public bool IsAllowAdd { get; set; }

        /// <summary>
        /// 是否允许编辑
        /// </summary>
        public bool IsAllowEdit { get; set; }

        /// <summary>
        /// 是否允许删除
        /// </summary>
        public bool IsAllowDelete { get; set; }

        /// <summary>
        /// 是否允许复制
        /// </summary>
        public bool IsAllowCopy { get; set; }

        /// <summary>
        /// 是否允许导入
        /// </summary>
        public bool IsAllowImport { get; set; }

        /// <summary>
        /// 是否允许导出
        /// </summary>
        public bool IsAllowExport { get; set; }

        /// <summary>
        /// 是否启用附件
        /// </summary>
        public bool IsEnableAttachment { get; set; }

        /// <summary>
        /// 是否启用回收站
        /// </summary>
        public bool IsEnabledRecycle { get; set; }

        /// <summary>
        /// 是否启用批量编辑
        /// </summary>
        public bool IsEnabledBatchEdit { get; set; }

        /// <summary>
        /// 是否允许打印
        /// </summary>
        public bool IsEnabledPrint { get; set; }

        /// <summary>
        /// 排序编码
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 标准JS所属文件夹
        /// </summary>
        public string StandardJsFolder { get; set; }

        /// <summary>
        /// 编辑模式
        /// </summary>
        public int ModuleEditMode { get; set; }
    }
}

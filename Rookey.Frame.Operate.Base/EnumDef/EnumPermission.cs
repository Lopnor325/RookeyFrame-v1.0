using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.EnumDef
{
    /// <summary>
    /// 字段权限类型
    /// </summary>
    public enum FieldPermissionTypeEnum
    {
        /// <summary>
        /// 字段查看
        /// </summary>
        ViewField = 0,

        /// <summary>
        /// 字段新增
        /// </summary>
        AddField = 1,

        /// <summary>
        /// 字段编辑
        /// </summary>
        EditField = 2
    }

    /// <summary>
    /// 数据权限类型
    /// </summary>
    public enum DataPermissionTypeEnum
    {
        /// <summary>
        /// 数据查看
        /// </summary>
        ViewData = 0,

        /// <summary>
        /// 数据编辑
        /// </summary>
        EditData = 1,

        /// <summary>
        /// 数据删除
        /// </summary>
        DelData = 2
    }
}

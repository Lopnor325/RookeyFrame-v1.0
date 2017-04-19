/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System.ComponentModel;

namespace Rookey.Frame.Common.PubDefine
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// 微软MsSqlServer数据库
        /// </summary>
        MsSqlServer = 0,

        /// <summary>
        /// MySql数据库
        /// </summary>
        MySql = 1,

        /// <summary>
        /// Oracle数据库
        /// </summary>
        Oracle = 2
    }

    /// <summary>
    /// 数据操作类型
    /// </summary>
    public enum DataOperateType
    {
        /// <summary>
        /// 新增
        /// </summary>
        Add = 0,

        /// <summary>
        /// 编辑
        /// </summary>
        Edit = 1,

        /// <summary>
        /// 删除
        /// </summary>
        Del = 2
    }
}

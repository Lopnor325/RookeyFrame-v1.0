using Rookey.Frame.Common.PubDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Common.Model
{
    /// <summary>
    /// 数据库连接参数
    /// </summary>
    public class DbLinkArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DbLinkArgs()
        {
            DbType = DatabaseType.MsSqlServer;
        }

        /// <summary>
        /// 服务器
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString { get; set; }
    }
}

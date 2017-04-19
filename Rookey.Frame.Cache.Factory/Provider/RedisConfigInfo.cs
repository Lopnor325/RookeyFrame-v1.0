using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Cache.Factory.Provider
{
    /// <summary>
    /// Redis配置信息类
    /// </summary>
    public class RedisConfigInfo
    {
        private string _host;//主机地址
        private int _port;//端口号
        private string _pwd;//密码
        private int _initaldb;//数据库初始化大小

        /// <summary>
        /// 主机
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = string.IsNullOrEmpty(value) ? "127.0.0.1" : value; }
        }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value > 0 ? value : 6379; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }
        /// <summary>
        /// 数据库初始化大小
        /// </summary>
        public int InitalDB
        {
            get { return _initaldb; }
            set { _initaldb = value > 0 ? value : 0; }
        }
    }
}

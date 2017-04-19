/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common.Model;
using Rookey.Frame.Common.PubDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 模块配置帮助类
    /// </summary>
    public static class ModelConfigHelper
    {
        /// <summary>
        /// 获取实体节点
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <returns></returns>
        private static string GetModelNode(Type modelType)
        {
            if (modelType == null) return null;
            string modelConfigPath = GetModelConfigXml();
            string node = string.Format("/Root/{0}", modelType.Name);
            bool nodeIsExists = XmlHelper.NodeIsExists(modelConfigPath, node);
            if (!nodeIsExists) //不存在实体节点配置信息，找对应基类的节点配置信息
            {
                //取实体基类
                Type baseType = modelType.BaseType;
                if (baseType != null) //存在基类
                {
                    node = string.Format("/Root/{0}", baseType.Name); //基类节点
                    nodeIsExists = XmlHelper.NodeIsExists(modelConfigPath, node);
                    if (!nodeIsExists) //不存在实体节点配置信息，找对应基类的节点配置信息
                    {
                        //取实体基类的基类
                        Type baseTypeTemp = baseType.BaseType;
                        if (baseTypeTemp != null) //存在基类
                        {
                            node = string.Format("/Root/{0}", baseTypeTemp.Name); //基类节点
                            nodeIsExists = XmlHelper.NodeIsExists(modelConfigPath, node);
                            if (!nodeIsExists) //不存在实体节点配置信息，找对应基类的节点配置信息
                            {
                                //取实体基类的基类的基类
                                Type baseBaseTypeTemp = baseTypeTemp.BaseType;
                                if (baseBaseTypeTemp != null) //存在基类
                                {
                                    node = string.Format("/Root/{0}", baseBaseTypeTemp.Name); //基类节点
                                    nodeIsExists = XmlHelper.NodeIsExists(modelConfigPath, node);
                                }
                            }
                        }
                    }
                }
            }
            if (!nodeIsExists) return null;
            return node;
        }

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        /// <param name="modelType">实体类型对象</param>
        /// <param name="cacheType">缓存类型</param>
        /// <returns></returns>
        public static bool IsEnableCache(Type modelType, out string cacheType)
        {
            cacheType = string.Empty;
            if (null == modelType) return false;
            string modelConfigPath = GetModelConfigXml();
            if (string.IsNullOrEmpty(modelConfigPath)) return false;
            //实体节点
            string node = GetModelNode(modelType);
            if (string.IsNullOrEmpty(node)) return false;
            bool isEnableCache = XmlHelper.Read(modelConfigPath, node, "isEnableCache") == "1";
            //获取缓存类型
            cacheType = XmlHelper.Read(modelConfigPath, node, "cacheType");
            return isEnableCache;
        }

        /// <summary>
        /// 模块是否启用缓存
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <returns></returns>
        public static bool IsModelEnableCache(Type modelType)
        {
            string cacheType = string.Empty;
            bool isEnableCache = ModelConfigHelper.IsEnableCache(modelType, out cacheType);
            return isEnableCache;
        }

        /// <summary>
        /// 是否启用内存缓存
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <returns></returns>
        public static bool IsModelEnableMemeryCache(Type modelType)
        {
            string cacheType = string.Empty;
            bool isEnableCache = ModelConfigHelper.IsEnableCache(modelType, out cacheType);
            if (string.IsNullOrEmpty(cacheType)) cacheType = "0";
            return isEnableCache && cacheType == "0";
        }

        /// <summary>
        /// 获取实体连接字符串
        /// </summary>
        /// <param name="modelType">实体类型对象</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="read">读写分离标识，是否读数据库，为否则取写数据库</param>
        /// <param name="export">是否为导出</param>
        /// <returns></returns>
        public static string GetModelConnString(Type modelType, out string dbType, bool read = true, bool export = false)
        {
            dbType = string.Empty;
            if (null == modelType)
                return string.Empty;
            string modelConfigPath = GetModelConfigXml();
            string node = GetModelNode(modelType);
            if (string.IsNullOrEmpty(node)) return string.Empty;
            string tempConnStr = string.Empty;
            if (export) //导出
            {
                tempConnStr = XmlHelper.Read(modelConfigPath, node, "exportConnString");
                if (string.IsNullOrEmpty(tempConnStr))
                {
                    tempConnStr = XmlHelper.Read(modelConfigPath, node, "readConnString");
                }
            }
            else //非导出
            {
                tempConnStr = XmlHelper.Read(modelConfigPath, node, read ? "readConnString" : "writeConnString");
                if (!read && string.IsNullOrEmpty(tempConnStr))
                {
                    tempConnStr = XmlHelper.Read(modelConfigPath, node, "readConnString");
                }
            }
            dbType = XmlHelper.Read(modelConfigPath, node, "dbType");
            if (string.IsNullOrEmpty(tempConnStr) && modelType.BaseType != null)
            {
                tempConnStr = GetModelConnString(modelType.BaseType, out dbType, read);
                if (string.IsNullOrEmpty(tempConnStr) && modelType.BaseType.BaseType != null)
                {
                    tempConnStr = GetModelConnString(modelType.BaseType.BaseType, out dbType, read);
                }
            }
            return tempConnStr;
        }

        /// <summary>
        /// 获取实体连接字符串
        /// </summary>
        /// <param name="modelType">实体类型对象</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="read">读写分离标识，是否读数据库，为否则取写数据库</param>
        /// <param name="export">是否为导出</param>
        /// <returns></returns>
        public static string GetModelConnStr(Type modelType, out DatabaseType dbType, bool read = true, bool export = false)
        {
            string dbTypeStr = string.Empty;
            string connStr = GetModelConnString(modelType, out dbTypeStr, read, export);
            dbType = DatabaseType.MsSqlServer;
            try
            {
                dbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbTypeStr);
            }
            catch { }
            return connStr;
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="dbLinkArgs">连接对象</param>
        /// <returns></returns>
        public static string GetDbLinkArgsConnStr(DbLinkArgs dbLinkArgs)
        {
            if (dbLinkArgs == null)
                return string.Empty;
            return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Pooling=true;MAX Pool Size=512;Min Pool Size=50;Connection Lifetime=30", dbLinkArgs.DataSource.ObjToStr(), dbLinkArgs.DbName.ObjToStr(), dbLinkArgs.UserId.ObjToStr(), dbLinkArgs.Pwd.ObjToStr());
        }

        /// <summary>
        /// 获取模块配置xml
        /// </summary>
        /// <returns></returns>
        public static string GetModelConfigXml()
        {
            string pathFlag = "\\";
            if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                pathFlag = "/";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (!basePath.EndsWith(pathFlag))
            {
                basePath += pathFlag;
            }
            string modelConfigPath = WebConfigHelper.GetAppSettingValue("ModelConfig");
            if (string.IsNullOrEmpty(modelConfigPath)) //没有配置实体配置
            {
                modelConfigPath = string.Format("Config{0}modelConfig.xml", pathFlag); //取默认配置
            }
            modelConfigPath = basePath + modelConfigPath;
            if (!System.IO.File.Exists(modelConfigPath)) //文件不存在
                return string.Empty;
            return modelConfigPath;
        }

        /// <summary>
        /// 模块是否为视图模式
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <returns></returns>
        public static bool ModelIsViewMode(Type modelType)
        {
            string modelConfigPath = GetModelConfigXml();
            string node = GetModelNode(modelType);
            if (string.IsNullOrEmpty(node)) return false;
            return XmlHelper.Read(modelConfigPath, node, "viewMode").ObjToInt() == 1;
        }

        /// <summary>
        /// 获取服务器数据源
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        private static string GetDataSource(string dataSource, DatabaseType dbType)
        {
            if (dbType == DatabaseType.MsSqlServer)
            {
                if (dataSource == "(local)")
                    return ".";
                else if (dataSource == "127.0.0.1")
                    return ".";
                return dataSource;
            }
            return dataSource;
        }

        /// <summary>
        /// 获取跨库模块的远程连接参数
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static DbLinkArgs GetDbLinkArgs(string connString, DatabaseType dbType = DatabaseType.MsSqlServer)
        {
            DbLinkArgs dbLinkArgs = null;
            if (!string.IsNullOrWhiteSpace(connString))
            {
                #region MsSqlServer
                if (dbType == DatabaseType.MsSqlServer) //sql server数据库
                {
                    string[] token = connString.Trim().Split(";".ToCharArray());
                    if (token != null && token.Length > 0)
                    {
                        string dataSource = string.Empty;
                        string dbName = string.Empty;
                        string uId = string.Empty;
                        string pwd = string.Empty;
                        foreach (string str in token)
                        {
                            if (str.StartsWith("Data Source"))
                            {
                                string[] tempToken = str.Split("=".ToCharArray());
                                if (tempToken != null && tempToken.Length == 2)
                                {
                                    dataSource = GetDataSource(tempToken[1].Trim(), dbType);
                                }
                            }
                            else if (str.StartsWith("Initial Catalog"))
                            {
                                string[] tempToken = str.Split("=".ToCharArray());
                                if (tempToken != null && tempToken.Length == 2)
                                {
                                    dbName = tempToken[1].Trim();
                                }
                            }
                            else if (str.StartsWith("User ID"))
                            {
                                string[] tempToken = str.Split("=".ToCharArray());
                                if (tempToken != null && tempToken.Length == 2)
                                {
                                    uId = tempToken[1].Trim();
                                }
                            }
                            else if (str.StartsWith("Password"))
                            {
                                string[] tempToken = str.Split("=".ToCharArray());
                                if (tempToken != null && tempToken.Length == 2)
                                {
                                    pwd = tempToken[1].Trim();
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(dataSource) && !string.IsNullOrEmpty(dbName) &&
                            !string.IsNullOrEmpty(uId) && !string.IsNullOrEmpty(pwd))
                        {
                            dbLinkArgs = new DbLinkArgs();
                            dbLinkArgs.DataSource = dataSource;
                            dbLinkArgs.DbName = dbName;
                            dbLinkArgs.UserId = uId;
                            dbLinkArgs.Pwd = pwd;
                            dbLinkArgs.DbType = dbType;
                            dbLinkArgs.ConnString = connString;
                        }
                    }
                }
                #endregion
                #region MySql
                else if (dbType == DatabaseType.MySql) //mysql数据库
                {
                    string[] token = connString.Trim().Split(";".ToCharArray());
                    if (token != null && token.Length > 0)
                    {
                        string dataSource = string.Empty;
                        string dbName = string.Empty;
                        string uId = string.Empty;
                        string pwd = string.Empty;
                        foreach (string str in token)
                        {
                            if (str.StartsWith("Server"))
                            {
                                string[] tempToken = str.Split("=".ToCharArray());
                                if (tempToken != null && tempToken.Length == 2)
                                {
                                    dataSource = GetDataSource(tempToken[1].Trim(), dbType);
                                }
                            }
                            else if (str.StartsWith("Database"))
                            {
                                string[] tempToken = str.Split("=".ToCharArray());
                                if (tempToken != null && tempToken.Length == 2)
                                {
                                    dbName = tempToken[1].Trim();
                                }
                            }
                            else if (str.StartsWith("User"))
                            {
                                string[] tempToken = str.Split("=".ToCharArray());
                                if (tempToken != null && tempToken.Length == 2)
                                {
                                    uId = tempToken[1].Trim();
                                }
                            }
                            else if (str.StartsWith("Password"))
                            {
                                string[] tempToken = str.Split("=".ToCharArray());
                                if (tempToken != null && tempToken.Length == 2)
                                {
                                    pwd = tempToken[1].Trim();
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(dataSource) && !string.IsNullOrEmpty(dbName) &&
                            !string.IsNullOrEmpty(uId) && !string.IsNullOrEmpty(pwd))
                        {
                            dbLinkArgs = new DbLinkArgs();
                            dbLinkArgs.DataSource = dataSource;
                            dbLinkArgs.DbName = dbName;
                            dbLinkArgs.UserId = uId;
                            dbLinkArgs.Pwd = pwd;
                            dbLinkArgs.DbType = dbType;
                            dbLinkArgs.ConnString = connString;
                        }
                    }
                }
                #endregion
            }
            return dbLinkArgs;
        }

        /// <summary>
        /// 获取模块数据库连接参数
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <returns></returns>
        public static DbLinkArgs GetDbLinkArgs(Type modelType)
        {
            DatabaseType dbType = DatabaseType.MsSqlServer;
            string connStr = GetModelConnStr(modelType, out dbType);
            return GetDbLinkArgs(connStr, dbType);
        }

        /// <summary>
        /// 模块数据库是否是跨服务器
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <returns></returns>
        public static bool IsCrossServer(Type modelType)
        {
            string dbType = string.Empty;
            //额外配置的连接字符串
            string connString = GetModelConnString(modelType, out dbType);
            if (string.IsNullOrWhiteSpace(dbType)) dbType = "0";
            DatabaseType dbTypeEnum = DatabaseType.MsSqlServer;
            try
            {
                dbTypeEnum = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbType);
            }
            catch { }
            //额外数据库连接对象
            DbLinkArgs dblinkArgs = GetDbLinkArgs(connString, dbTypeEnum);
            if (dblinkArgs != null)
            {
                //本地连接字符串
                string currConnString = WebConfigHelper.GetConnectionString("DbReadConnString");
                string currDbTypeStr = WebConfigHelper.GetAppSettingValue("DbType");
                if (string.IsNullOrEmpty(currDbTypeStr)) currDbTypeStr = "0";
                DatabaseType currDbType = DatabaseType.MsSqlServer;
                try
                {
                    currDbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), currDbTypeStr);
                }
                catch { }
                //本地连接对象
                DbLinkArgs currDblinkArgs = GetDbLinkArgs(currConnString, currDbType);
                if (dblinkArgs.DataSource != currDblinkArgs.DataSource) //服务器不相同
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取模块表名，支持跨库
        /// </summary>
        /// <param name="modelType">实体类型对象</param>
        /// <param name="currTempDblinkArgs">当前连接对象，默认为本地连接</param>
        /// <returns></returns>
        public static string GetModuleTableName(Type modelType, DbLinkArgs currTempDblinkArgs = null)
        {
            string dbType = string.Empty;
            //额外配置的连接字符串
            string connString = GetModelConnString(modelType, out dbType);
            if (string.IsNullOrWhiteSpace(dbType)) dbType = "0";
            DatabaseType dbTypeEnum = DatabaseType.MsSqlServer;
            try
            {
                dbTypeEnum = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbType);
            }
            catch { }
            //额外数据库连接对象
            DbLinkArgs dblinkArgs = GetDbLinkArgs(connString, dbTypeEnum);
            if (dblinkArgs != null)
            {
                //本地连接字符串
                string currConnString = WebConfigHelper.GetConnectionString("DbReadConnString");
                string currDbTypeStr = WebConfigHelper.GetAppSettingValue("DbType");
                if (string.IsNullOrEmpty(currDbTypeStr)) currDbTypeStr = "0";
                DatabaseType currDbType = DatabaseType.MsSqlServer;
                try
                {
                    currDbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), currDbTypeStr);
                }
                catch { }
                //本地连接对象
                DbLinkArgs currDblinkArgs = currTempDblinkArgs != null ? currTempDblinkArgs : GetDbLinkArgs(currConnString, currDbType);
                if (dblinkArgs.DataSource == currDblinkArgs.DataSource || dblinkArgs.DataSource == "." || dblinkArgs.DataSource == "(local)") //服务器相同或本地服务器
                {
                    if (dblinkArgs.DbName != currDblinkArgs.DbName) //跨库不跨服务器
                    {
                        return string.Format("[{0}].[dbo].[{1}]", dblinkArgs.DbName, modelType.Name);
                    }
                }
                else //跨服务器
                {
                    return string.Format("[{0}].[{1}].[dbo].[{2}]", dblinkArgs.DataSource, dblinkArgs.DbName, modelType.Name);
                }
            }
            return modelType.Name;
        }

        /// <summary>
        /// 获取本地数据库连接对象
        /// </summary>
        /// <returns></returns>
        public static DbLinkArgs GetLocalDbLinkArgs()
        {
            string currConnString = WebConfigHelper.GetConnectionString("DbReadConnString");
            string currDbTypeStr = WebConfigHelper.GetAppSettingValue("DbType");
            if (string.IsNullOrEmpty(currDbTypeStr)) currDbTypeStr = "0";
            DatabaseType currDbType = DatabaseType.MsSqlServer;
            try
            {
                currDbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), currDbTypeStr);
            }
            catch { }
            //本地连接对象
            DbLinkArgs currDblinkArgs = GetDbLinkArgs(currConnString, currDbType);
            return currDblinkArgs;
        }

        /// <summary>
        /// 获取跨服务器的连接参数集合
        /// </summary>
        /// <param name="isCrossServer">是否跨库服务器，不包含服务器相同数据库不同的</param>
        /// <returns></returns>
        public static List<DbLinkArgs> GetCrossServerDbLinkArgs(bool isCrossServer = true)
        {
            List<DbLinkArgs> list = new List<DbLinkArgs>();
            string modelConfigPath = GetModelConfigXml();
            string rootNode = "/Root";
            XmlNodeList nodeList = XmlHelper.ReadAllChild(modelConfigPath, rootNode);
            if (nodeList != null && nodeList.Count > 0)
            {
                //本地连接对象
                DbLinkArgs currDblinkArgs = GetLocalDbLinkArgs();
                //取跨服务器的连接对象
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode node = nodeList[i];
                    string nodeStr = string.Format("/Root/{0}", node.Name);
                    string tempReadConnStr = XmlHelper.Read(modelConfigPath, nodeStr, "readConnString");
                    string dbTypeStr = XmlHelper.Read(modelConfigPath, nodeStr, "dbType");
                    DatabaseType dbTypeEnum = DatabaseType.MsSqlServer;
                    try
                    {
                        dbTypeEnum = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbTypeStr);
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(tempReadConnStr))
                    {
                        DbLinkArgs linkArgs = GetDbLinkArgs(tempReadConnStr, dbTypeEnum);
                        if (isCrossServer) //只有服务器不相同时才添加
                        {
                            if (linkArgs.DataSource != currDblinkArgs.DataSource && list.Where(x => x.DataSource == linkArgs.DataSource).FirstOrDefault() == null)
                            {
                                list.Add(linkArgs);
                            }
                        }
                        else //只要是数据库不同就添加
                        {
                            if (linkArgs.DbName != currDblinkArgs.DbName && list.Where(x => x.DbName == linkArgs.DbName).FirstOrDefault() == null)
                            {
                                list.Add(linkArgs);
                            }
                        }
                    }
                    string tempWriteConnStr = XmlHelper.Read(modelConfigPath, nodeStr, "writeConnString");
                    if (!string.IsNullOrEmpty(tempWriteConnStr))
                    {
                        DbLinkArgs linkArgs = GetDbLinkArgs(tempWriteConnStr, dbTypeEnum);
                        if (isCrossServer)
                        {
                            if (linkArgs.DataSource != currDblinkArgs.DataSource && list.Where(x => x.DataSource == linkArgs.DataSource).FirstOrDefault() == null)
                            {
                                list.Add(linkArgs);
                            }
                        }
                        else
                        {
                            if (linkArgs.DbName != currDblinkArgs.DbName && list.Where(x => x.DbName == linkArgs.DbName).FirstOrDefault() == null)
                            {
                                list.Add(linkArgs);
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}

/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Base;
using Rookey.Frame.Cache.Factory;
using Rookey.Frame.Cache.Factory.Provider;
using Rookey.Frame.Common;
using Rookey.Frame.DAL.Base;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rookey.Frame.DAL.Sys
{
    /// <summary>
    /// 缓存配置数据层重写
    /// </summary>
    public class Sys_CacheConfigDAL : BaseDAL<Sys_CacheConfig>
    {
        #region 私有方法

        /// <summary>
        /// Table - Id对应表
        /// </summary>
        private static Dictionary<string, Guid> tableIdDic = new Dictionary<string, Guid>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currUser">当前用户</param>
        public Sys_CacheConfigDAL(UserInfo currUser)
            : base(currUser, null)
        {
        }

        /// <summary>
        /// 获取所有modelTypes
        /// </summary>
        /// <returns></returns>
        private List<Type> GetAllModelTypes()
        {
            ICacheProvider dalCacheFactory = CacheFactory.GetCacheInstance(CacheProviderType.LOCALMEMORYCACHE);
            List<Type> modelTypes = dalCacheFactory.Get<List<Type>>("cache_modelType");
            return modelTypes;
        }

        /// <summary>
        /// 从xml配置文件中获取模块缓存配置
        /// </summary>
        /// <param name="tableName">模块表名或类名</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        private Sys_CacheConfig GetModuleCacheConfig(string tableName, string moduleName = null, Guid? moduleId = null)
        {
            //将未添加到模块表中的模块也加进来
            List<Type> modelTypes = GetAllModelTypes();
            Type modelType = modelTypes.Where(x => x.Name == tableName).FirstOrDefault();
            Guid id = Guid.Empty;
            if (!moduleId.HasValue)
            {
                if (!tableIdDic.ContainsKey(tableName))
                {
                    Guid tempId = Guid.NewGuid();
                    tableIdDic.Add(tableName, tempId);
                    id = tempId;
                }
                else
                {
                    id = tableIdDic[tableName];
                }
            }
            else
            {
                id = moduleId.Value;
            }
            Sys_CacheConfig cacheConfig = new Sys_CacheConfig()
            {
                Id = id,
                ModuleName = string.IsNullOrEmpty(moduleName) ? tableName : moduleName
            };
            string cacheType = string.Empty;
            bool isEnableCache = ModelConfigHelper.IsEnableCache(modelType, out cacheType);
            if (string.IsNullOrEmpty(cacheType)) cacheType = "0";
            TempCacheProviderType cacheTypeEnum = (TempCacheProviderType)Enum.Parse(typeof(TempCacheProviderType), cacheType);
            cacheConfig.IsEnableCache = isEnableCache;
            cacheConfig.CacheTypeOfEnum = cacheTypeEnum;
            return cacheConfig;
        }

        /// <summary>
        /// 保存模块缓存配置
        /// </summary>
        /// <param name="cacheConfig">缓存配置对象</param>
        /// <returns></returns>
        private string SaveModuleCacheConfig(Sys_CacheConfig cacheConfig)
        {
            string errMsg = string.Empty;
            if (cacheConfig != null)
            {
                try
                {
                    string tableName = string.Empty;
                    BaseDAL<Sys_Module> moduleDal = new BaseDAL<Sys_Module>(this.CurrUser);
                    Sys_Module module = moduleDal.GetEntity(out errMsg, x => x.Name == cacheConfig.ModuleName);
                    if (module == null || string.IsNullOrEmpty(module.TableName))
                    {
                        tableName = tableIdDic.Where(x => x.Value == cacheConfig.Id).FirstOrDefault().Key;
                    }
                    else
                    {
                        tableName = module.TableName;
                    }
                    if (string.IsNullOrWhiteSpace(tableName))
                        return "找不到模块表！";
                    string modelConfigPath = ModelConfigHelper.GetModelConfigXml();
                    string node = string.Format("/Root/{0}", tableName);
                    bool nodeIsExists = XmlHelper.NodeIsExists(modelConfigPath, node);
                    if (!nodeIsExists) //不存在实体节点配置信息，插入节点
                    {
                        XmlHelper.Insert(modelConfigPath, "/Root", tableName, string.Empty, string.Empty);
                    }
                    XmlHelper.Update(modelConfigPath, node, "isEnableCache", cacheConfig.IsEnableCache ? "1" : "0");
                    XmlHelper.Update(modelConfigPath, node, "cacheType", cacheConfig.CacheType.ToString());
                    if (!cacheConfig.IsEnableCache) //不启用缓存时
                    {
                        this.ClearCache();
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
            }
            return errMsg;
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否新增导航数据</param>
        /// <param name="connString">连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override Guid AddEntity(Sys_CacheConfig entity, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = "该模块不支持新增";
            return Guid.Empty;
        }

        /// <summary>
        /// 新增实体集合
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存导航数据</param>
        /// <param name="connString">连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool AddEntities(List<Sys_CacheConfig> entities, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = "该模块不支持新增";
            return false;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <returns></returns>
        public override bool DeleteEntity(System.Linq.Expressions.Expression<Func<Sys_CacheConfig, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = "该模块不允许删除";
            return false;
        }

        /// <summary>
        /// 通过ID删除实体
        /// </summary>
        /// <returns></returns>
        public override bool DeleteEntityById(object id, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = "该模块不允许删除";
            return false;
        }

        /// <summary>
        /// 通过ids删除实体
        /// </summary>
        /// <returns></returns>
        public override bool DeleteEntityByIds(System.Collections.IEnumerable ids, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = "该模块不允许删除";
            return false;
        }

        /// <summary>
        /// 分页方法重写
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionFilter">是否权限过滤</param>
        /// <param name="pageIndex">页号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">是否降序</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="whereSql">条件语句</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public override List<Sys_CacheConfig> GetPageEntities(out long totalCount, out string errorMsg, bool permissionFilter = true, int pageIndex = 1, int pageSize = 10, List<string> orderFields = null, List<bool> isDescs = null, System.Linq.Expressions.Expression<Func<Sys_CacheConfig, bool>> expression = null, string whereSql = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            totalCount = 0;
            BaseDAL<Sys_Module> moduleDal = new BaseDAL<Sys_Module>(this.CurrUser);
            int dataSourceType = (int)ModuleDataSourceType.DbTable;
            List<Sys_Module> modules = moduleDal.GetEntities(out errorMsg, x => x.DataSourceType == dataSourceType, null, permissionFilter);
            List<Sys_CacheConfig> list = modules.Select(x => GetModuleCacheConfig(x.TableName, x.Name, x.Id)).ToList();
            //将未添加到模块表中的模块也加进来
            List<Type> modelTypes = GetAllModelTypes();
            List<string> tables = moduleDal.GetEntities(out errorMsg, x => x.TableName != null && x.TableName != string.Empty).Select(x => x.TableName).ToList();
            list.AddRange(modelTypes.Where(x => !tables.Contains(x.Name)).Select(x => GetModuleCacheConfig(x.Name)));
            if (expression != null)
            {
                list = list.Where(expression.Compile()).ToList();
                if (orderFields != null && orderFields.Count > 0)
                {
                    for (int i = 0; i < orderFields.Count; i++)
                    {
                        string orderField = string.IsNullOrEmpty(orderFields[i]) ? "Id" : orderFields[i];
                        bool isdesc = isDescs != null && orderFields.Count == isDescs.Count ? isDescs[i] : true;
                        SortComparer<Sys_CacheConfig> reverser = new SortComparer<Sys_CacheConfig>(typeof(Sys_CacheConfig), orderField, isdesc ? ReverserInfo.Direction.DESC : ReverserInfo.Direction.ASC);
                        list.Sort(reverser);
                    }
                }
                totalCount = list.Count;
            }
            //页序号
            int index = pageIndex < 1 ? 0 : (pageIndex - 1);
            //每页记录数
            int rows = pageSize < 1 ? 10 : (pageSize > 2000 ? 2000 : pageSize);
            list = list.Skip<Sys_CacheConfig>(rows * index).Take<Sys_CacheConfig>(rows).ToList();
            return list;
        }

        /// <summary>
        /// 获取实体集合重写
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="expression"></param>
        /// <param name="whereSql"></param>
        /// <param name="permissionFilter"></param>
        /// <param name="orderFields"></param>
        /// <param name="isDescs"></param>
        /// <param name="top"></param>
        /// <param name="references"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public override List<Sys_CacheConfig> GetEntities(out string errorMsg, System.Linq.Expressions.Expression<Func<Sys_CacheConfig, bool>> expression = null, string whereSql = null, bool permissionFilter = true, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            BaseDAL<Sys_Module> moduleDal = new BaseDAL<Sys_Module>(this.CurrUser);
            int dataSourceType = (int)ModuleDataSourceType.DbTable;
            List<Sys_Module> modules = moduleDal.GetEntities(out errorMsg, x => x.DataSourceType == dataSourceType, null, permissionFilter);
            List<Sys_CacheConfig> list = modules.Select(x => GetModuleCacheConfig(x.TableName, x.Name, x.Id)).ToList();
            //将未添加到模块表中的模块也加进来
            List<Type> modelTypes = GetAllModelTypes();
            List<string> tables = moduleDal.GetEntities(out errorMsg, x => x.TableName != null && x.TableName != string.Empty).Select(x => x.TableName).ToList();
            list.AddRange(modelTypes.Where(x => !tables.Contains(x.Name)).Select(x => GetModuleCacheConfig(x.Name)));
            if (expression != null)
            {
                list = list.Where(expression.Compile()).ToList();
                if (orderFields != null && orderFields.Count > 0)
                {
                    for (int i = 0; i < orderFields.Count; i++)
                    {
                        string orderField = string.IsNullOrEmpty(orderFields[i]) ? "Id" : orderFields[i];
                        bool isdesc = isDescs != null && orderFields.Count == isDescs.Count ? isDescs[i] : true;
                        SortComparer<Sys_CacheConfig> reverser = new SortComparer<Sys_CacheConfig>(typeof(Sys_CacheConfig), orderField, isdesc ? ReverserInfo.Direction.DESC : ReverserInfo.Direction.ASC);
                        list.Sort(reverser);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 重写获取实体对象
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="id">Id</param>
        /// <param name="references"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public override Sys_CacheConfig GetEntityById(out string errorMsg, object id, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            BaseDAL<Sys_Module> moduleDal = new BaseDAL<Sys_Module>(this.CurrUser);
            Sys_Module module = moduleDal.GetEntityById(out errorMsg, id);
            if (module != null)
                return GetModuleCacheConfig(module.TableName, module.Name, module.Id);
            string tableName = tableIdDic.Where(x => x.Value == id.ObjToGuid()).FirstOrDefault().Key;
            return GetModuleCacheConfig(tableName);
        }

        /// <summary>
        /// 重写更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <returns></returns>
        public override bool UpdateEntity(Sys_CacheConfig entity, out string errorMsg, bool permissionValidate = true, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = SaveModuleCacheConfig(entity);
            return string.IsNullOrEmpty(errorMsg);
        }

        /// <summary>
        /// 重写更新实体对象
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="fieldNames">要更新的字段</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <returns></returns>
        public override bool UpdateEntityFields(Sys_CacheConfig entity, List<string> fieldNames, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = SaveModuleCacheConfig(entity);
            return string.IsNullOrEmpty(errorMsg);
        }

        /// <summary>
        /// 重写根据表达式更新实体
        /// </summary>
        /// <param name="obj">更新字段对象</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public override bool UpdateEntityByExpression(object obj, System.Linq.Expressions.Expression<Func<Sys_CacheConfig, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            BaseDAL<Sys_Module> moduleDal = new BaseDAL<Sys_Module>(this.CurrUser);
            int dataSourceType = (int)ModuleDataSourceType.DbTable;
            List<Sys_Module> modules = moduleDal.GetEntities(out errorMsg, x => x.DataSourceType == dataSourceType);
            List<Sys_CacheConfig> list = modules.Select(x => GetModuleCacheConfig(x.TableName, x.Name, x.Id)).ToList();
            if (expression != null) list = list.Where(expression.Compile()).ToList();
            PropertyInfo[] ps = obj.GetType().GetProperties(); //要更新的属性
            StringBuilder errSb = new StringBuilder();
            if (ps.Length > 0)
            {
                List<string> updateFields = ps.Select(x => x.Name).ToList();
                PropertyInfo[] tempPs = typeof(Sys_CacheConfig).GetProperties().Where(x => updateFields.Contains(x.Name)).ToArray();
                foreach (Sys_CacheConfig cacheConfig in list)
                {
                    foreach (PropertyInfo p in tempPs)
                    {
                        object value = ps.Where(x => x.Name == p.Name).FirstOrDefault().GetValue2(obj, null);
                        p.SetValue2(cacheConfig, value, null);
                    }
                    string temMsg = string.Empty;
                    bool rs = UpdateEntity(cacheConfig, out temMsg);
                    if (!string.IsNullOrEmpty(temMsg))
                    {
                        errSb.AppendLine(temMsg);
                    }
                }
            }
            errorMsg = errSb.ToString();
            return string.IsNullOrEmpty(errorMsg);
        }

        #endregion
    }
}

/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using Rookey.Frame.Common;
using Rookey.Frame.IDAL.Base;
using Rookey.Frame.DALFactory;
using Rookey.Frame.Cache.Factory;
using Rookey.Frame.Cache.Factory.Provider;
using System.Configuration;
using System.Data;
using Rookey.Frame.Common.PubDefine;
using Rookey.Frame.Common.Model;
using Rookey.Frame.Base;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.DAL.Sys;
using Rookey.Frame.EntityBase;
using System.Threading.Tasks;

namespace Rookey.Frame.DAL.Base
{
    /// <summary>
    /// 数据层基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDAL<T> : IBaseDAL<T> where T : BaseEntity, new()
    {
        #region 私有成员

        /// <summary>
        /// dal抽象工厂对象
        /// </summary>
        private DalAbstractFactory<T> dalFactory = null;

        /// <summary>
        /// 缓存工厂
        /// </summary>
        private ICacheProvider cacheFactory = null;

        /// <summary>
        /// 缓存key
        /// </summary>
        private string cacheKey = typeof(T).Name;

        /// <summary>
        /// 缓存类型
        /// </summary>
        private CacheProviderType cacheType = CacheProviderType.LOCALMEMORYCACHE;

        /// <summary>
        /// 所有缓存key集合
        /// </summary>
        private static List<string> cacheKeyList = new List<string>();

        /// <summary>
        /// 默认排序字段
        /// </summary>
        private const string defaultSortField = "CreateDate";

        /// <summary>
        /// 当前用户
        /// </summary>
        private UserInfo currUser = null;

        /// <summary>
        /// 锁对象
        /// </summary>
        private static object lockObj = new object();

        /// <summary>
        /// 是否异步更新缓存
        /// </summary>
        private bool isAsyncUpCache = true;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseDAL()
        {
            //数据库工厂实例化
            DalFactoryInit();
            //缓存初始化
            CacheInit();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">当前用户</param>
        public BaseDAL(UserInfo userInfo)
            : this()
        {
            this.currUser = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">当前用户</param>
        /// <param name="dbType">数据库类型</param>
        public BaseDAL(UserInfo userInfo = null, DatabaseType? dbType = null)
        {
            currUser = userInfo;
            //数据库工厂实例化
            DalFactoryInit(dbType);
            //缓存初始化
            CacheInit();
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        public UserInfo CurrUser
        {
            get { return currUser; }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="whereSql">where条件</param>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        private List<T> GetEntitiesByWhere(string whereSql, string connString = null)
        {
            if (string.IsNullOrEmpty(whereSql)) return new List<T>();
            string errMsg = string.Empty;
            List<T> list = dalFactory.GetEntities(out errMsg, null, whereSql, null, null, null, null, false, connString);
            if (list == null) list = new List<T>();
            return list;
        }

        /// <summary>
        /// 数据工厂初始化
        /// </summary>
        /// <param name="_dbType">数据库类型</param>
        private void DalFactoryInit(DatabaseType? _dbType = null)
        {
            //取默认配置数据库类型
            string dbTypeStr = WebConfigHelper.GetAppSettingValue("DbType");
            if (string.IsNullOrEmpty(dbTypeStr)) dbTypeStr = "0";
            DatabaseType dbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbTypeStr);
            if (!_dbType.HasValue) //找实体配置表的数据库类型
            {
                string tempDbTypeStr = string.Empty;
                string connStr = ModelConfigHelper.GetModelConnString(typeof(T), out tempDbTypeStr);
                if (tempDbTypeStr != string.Empty) //实体配置了数据库类型
                {
                    try
                    {
                        dbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), tempDbTypeStr);
                    }
                    catch { }
                }
            }
            else //参数中传过来的数据库类型
            {
                dbType = _dbType.Value;
            }
            dalFactory = DalAbstractFactory<T>.GetInstance(dbType);
        }

        /// <summary>
        /// 缓存初始化
        /// </summary>
        private void CacheInit()
        {
            string cacheTypeStr = string.Empty;
            bool isEnableCache = ModelConfigHelper.IsEnableCache(typeof(T), out cacheTypeStr);
            if (isEnableCache) //启用缓存
            {
                if (string.IsNullOrEmpty(cacheTypeStr)) cacheTypeStr = "0";
                cacheType = (CacheProviderType)Enum.Parse(typeof(CacheProviderType), cacheTypeStr);
                //缓存工厂实例化
                cacheFactory = CacheFactory.GetCacheInstance(cacheType);
                isAsyncUpCache = cacheType != CacheProviderType.ServiceStackREDIS;
            }
        }

        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="whereSql">条件语句</param>
        /// <param name="connString">连接字符串</param>
        private void CacheData(string whereSql, string connString = null)
        {
            string tempCacheKey = !string.IsNullOrEmpty(whereSql) ? string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(whereSql.ToLower())) : cacheKey;
            List<T> cacheList = new List<T>();
            try
            {
                cacheList = string.IsNullOrEmpty(whereSql) ? dalFactory.GetAllEntities(false, connString) : this.GetEntitiesByWhere(whereSql, connString);
            }
            catch { }
            List<string> permissionModels = new List<string>() { "Sys_PermissionData", "Sys_PermissionField", "Sys_PermissionFun", "Sys_UserPermissionData", "Sys_UserPermissionField", "Sys_UserPermissionFun" };
            if (permissionModels.Contains(typeof(T).Name)) //权限相关实体
            {
                if (cacheList == null) cacheList = new List<T>();
                cacheFactory.Set<List<T>>(tempCacheKey, cacheList);
                if (!cacheKeyList.Contains(tempCacheKey))
                    cacheKeyList.Add(tempCacheKey);
            }
            else if (cacheList != null && cacheList.Count > 0)
            {
                cacheFactory.Set<List<T>>(tempCacheKey, cacheList);
                if (!cacheKeyList.Contains(tempCacheKey))
                    cacheKeyList.Add(tempCacheKey);
            }
        }

        #region 权限相关

        /// <summary>
        /// 获取数据权限过滤语句，返回NULL无权限，
        /// 返回空字符串有全部权限，否则返回where语句
        /// </summary>
        /// <returns></returns>
        private string GetPermissionFilterWhere()
        {
            UserInfo userInfo = currUser;
            if (userInfo == null) return "1=2";
            if (UserInfo.IsSuperAdmin(userInfo)) return string.Empty;
            Guid userId = userInfo.UserId;
            if (typeof(T).Name == typeof(Sys_Menu).Name) //菜单权限
            {
                Sys_PermissionFunDAL permissionFunDal = new Sys_PermissionFunDAL(currUser);
                List<Guid> menuIds = permissionFunDal.GetUserFunPermissions(userId, 0, null).Keys.ToList();
                List<Guid> tempMenuIds = new List<Guid>();
                tempMenuIds.AddRange(menuIds);
                foreach (Guid menuId in menuIds)
                {
                    List<Guid> parentMenuIds = ObjectReferenceClass.GetParentsMenuId(menuId, currUser);
                    tempMenuIds.AddRange(parentMenuIds);
                }
                string where = string.Format("Id IN('{0}')", string.Join("','", tempMenuIds));
                return where;
            }
            else if (typeof(T).Name == typeof(Sys_GridButton).Name) //按钮权限
            {
                Sys_PermissionFunDAL permissionFunDal = new Sys_PermissionFunDAL(currUser);
                List<Guid> btnIds = permissionFunDal.GetUserFunPermissions(userId, 1, null).Keys.ToList();
                string where = string.Format("Id IN('{0}')", string.Join("','", btnIds));
                return where;
            }
            else
            {
                string errMsg = string.Empty;
                string tableName = typeof(T).Name;
                BaseDAL<Sys_Module> moduleDal = new BaseDAL<Sys_Module>(currUser);
                Sys_Module module = moduleDal.GetEntity(out errMsg, x => x.TableName == tableName);
                if (module == null) return string.Empty; //没有模块对象跳过
                BaseDAL<Sys_Menu> menuDal = new BaseDAL<Sys_Menu>(currUser);
                Sys_Menu menu = menuDal.GetEntity(out errMsg, x => x.Sys_ModuleId == module.Id && !x.IsDeleted && !x.IsDraft && x.IsValid);
                if (menu == null) //模块没有配置生效菜单是默认有全部权限
                    return string.Empty;
                Sys_PermissionDataDAL permissionDataDal = new Sys_PermissionDataDAL(currUser);
                List<string> orgIds = permissionDataDal.GetUserDataPermissions(userId, module.Id, 0);
                if (orgIds.Contains("-1")) //有全部权限
                    return string.Empty;
                if (orgIds.Count > 0) //有配置数据权限
                {
                    List<string> tempOrgIds = orgIds.Where(x => x != "-1").ToList();
                    if (orgIds.Contains(Guid.Empty.ObjToStr()) && userInfo.OrganizationId.HasValue && userInfo.OrganizationId.Value != Guid.Empty &&
                        !orgIds.Contains(userInfo.OrganizationId.Value.ToString()))
                        tempOrgIds.Add(userInfo.OrganizationId.Value.ToString());
                    string where = tempOrgIds.Count > 0 ? string.Format("OrgId IN('{0}')", string.Join("','", tempOrgIds)) :
                                  string.Format("CreateUserId='{0}'", userId);
                    return where;
                }
                else if (typeof(T).BaseType != null && typeof(T).BaseType.Name == "BaseSysEntity")
                {
                    //系统模块没有配置则能看到所有
                    return string.Empty;
                }
                else //业务模块没有配置只能查看个人的数据
                {
                    List<Guid?> childIds = ObjectReferenceClass.GetAllInferiorPerson(currUser);
                    if (childIds.Count > 0)
                    {
                        childIds.Add(currUser.UserId);
                        string join = string.Join("','", childIds.Select(x => x.Value));
                        return string.Format("CreateUserId IN('{0}')", join);
                    }
                    return string.Format("CreateUserId='{0}'", userId);
                }
            }
        }

        /// <summary>
        /// 获取权限表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<T, bool>> GetPermissionFilterExpression()
        {
            UserInfo userInfo = currUser;
            if (userInfo == null) return x => x.CreateUserId == Guid.Empty;
            if (UserInfo.IsSuperAdmin(userInfo)) return null;
            Guid userId = userInfo.UserId;
            if (typeof(T).Name == typeof(Sys_Menu).Name) //菜单权限
            {
                Sys_PermissionFunDAL permissionFunDal = new Sys_PermissionFunDAL(currUser);
                List<Guid> menuIds = permissionFunDal.GetUserFunPermissions(userId, 0, null).Keys.ToList();
                List<Guid> tempMenuIds = new List<Guid>();
                tempMenuIds.AddRange(menuIds);
                foreach (Guid menuId in menuIds)
                {
                    List<Guid> parentMenuIds = ObjectReferenceClass.GetParentsMenuId(menuId, currUser);
                    tempMenuIds.AddRange(parentMenuIds);
                }
                return x => tempMenuIds.Distinct().ToList().Contains(x.Id);
            }
            else if (typeof(T).Name == typeof(Sys_GridButton).Name) //按钮权限
            {
                Sys_PermissionFunDAL permissionFunDal = new Sys_PermissionFunDAL(currUser);
                List<Guid> btnIds = permissionFunDal.GetUserFunPermissions(userId, 1, null).Keys.ToList();
                return x => btnIds.Contains(x.Id);
            }
            else
            {
                string errMsg = string.Empty;
                string tableName = typeof(T).Name;
                BaseDAL<Sys_Module> moduleDal = new BaseDAL<Sys_Module>(currUser);
                Sys_Module module = moduleDal.GetEntity(out errMsg, x => x.TableName == tableName);
                if (module == null) //没有模块对象默认全部权限
                    return null; 
                BaseDAL<Sys_Menu> menuDal = new BaseDAL<Sys_Menu>(currUser);
                Sys_Menu menu = menuDal.GetEntity(out errMsg, x => x.Sys_ModuleId == module.Id && !x.IsDeleted && !x.IsDraft && x.IsValid);
                if (menu == null) //模块没有配置生效菜单是默认有全部权限
                    return null;
                Sys_PermissionDataDAL permissionDataDal = new Sys_PermissionDataDAL(currUser);
                List<string> orgIds = permissionDataDal.GetUserDataPermissions(userId, module.Id, 0);
                if (orgIds.Contains("-1")) //有全部权限
                    return null;
                if (orgIds.Count > 0) //有配置数据权限
                {
                    List<Guid?> tempOrgIds = orgIds.Where(x => x != "-1").Select(x => x.ObjToGuidNull()).ToList();
                    if (orgIds.Contains(Guid.Empty.ObjToStr()) && userInfo.OrganizationId.HasValue && userInfo.OrganizationId.Value != Guid.Empty &&
                        !orgIds.Contains(userInfo.OrganizationId.Value.ToString()))
                        tempOrgIds.Add(userInfo.OrganizationId.Value);
                    if (tempOrgIds.Count > 0)
                        return x => tempOrgIds.Contains(x.OrgId);
                    else
                        return x => x.CreateUserId == userId;
                }
                else if (typeof(T).BaseType != null && typeof(T).BaseType.Name == "BaseSysEntity")
                {
                    //系统模块没有配置则能看到所有
                    return null;
                }
                else //业务模块没有配置只能查看个人的数据
                {
                    List<Guid?> childIds = ObjectReferenceClass.GetAllInferiorPerson(currUser);
                    if (childIds.Count > 0) //存在下级人员
                    {
                        childIds.Add(currUser.UserId);
                        return x => childIds.Contains(x.CreateUserId);
                    }
                    return x => x.CreateUserId == userId;
                }
            }
        }

        /// <summary>
        /// 是否有记录的操作（查看，更新、删除）权限
        /// </summary>
        /// <param name="t">实体对象</param>
        /// <param name="type">类型，0-查看，1-更新，2-删除</param>
        /// <returns></returns>
        private bool HasRecordOperatePermission(T t, int type)
        {
            //调用通用权限处理
            UserInfo userInfo = currUser;
            if (userInfo == null) return false; //用户不存在不允许操作
            if (UserInfo.IsSuperAdmin(userInfo)) return true; //管理员跳过
            string errMsg = string.Empty;
            Guid userId = userInfo.UserId;
            //先检查有没有自定义权限处理，有则先调用各模块的自定义权限过滤处理，没有调用通用权限处理
            try
            {
                object instance = null;
                object typesObj = Globals.ExecuteReflectMethod("Rookey.Frame.Bridge", "BridgeObject", "GetCustomerOperateHandleTypes", null, ref instance, true);
                if (typesObj != null)
                {
                    List<Type> types = typesObj as List<Type>;
                    Type operateHandleType = types.Where(x => x.Namespace == "Rookey.Frame.Operate.Base.OperateHandle" && x.Name.StartsWith("OperateHandleFactory")).FirstOrDefault();
                    if (operateHandleType != null)
                    {
                        Type[] argsType = new Type[] { typeof(T) };
                        object[] args = new object[] { userInfo, t, type };
                        object handleInstance = null;
                        object result = Globals.ExecuteReflectMethod(operateHandleType, argsType, "HasRecordOperatePermission", args, ref handleInstance);
                        if (handleInstance != null)
                        {
                            return result.ObjToBool();
                        }
                    }
                }
            }
            catch { }
            if (!t.CreateUserId.HasValue)
            {
                object crId = Scalar(x => x.CreateUserId, x => x.Id == t.Id, out errMsg);
                t.CreateUserId = crId.ObjToGuidNull();
            }
            if (!t.CreateUserId.HasValue) return false;
            string tableName = typeof(T).Name;
            BaseDAL<Sys_Module> moduleDal = new BaseDAL<Sys_Module>(currUser);
            Sys_Module module = moduleDal.GetEntity(out errMsg, x => x.TableName == tableName);
            if (module == null) return true; //没有模块对象跳过
            BaseDAL<Sys_Menu> menuDal = new BaseDAL<Sys_Menu>(currUser);
            Sys_Menu menu = menuDal.GetEntity(out errMsg, x => x.Sys_ModuleId == module.Id && !x.IsDeleted && !x.IsDraft && x.IsValid);
            if (menu == null) //模块没有配置生效菜单是默认有全部权限
                return true;
            Sys_PermissionDataDAL permissionDataDal = new Sys_PermissionDataDAL(currUser);
            List<string> orgIds = permissionDataDal.GetUserDataPermissions(userId, module.Id, type);
            if (orgIds.Contains("-1")) //有全部权限
                return true;
            if (orgIds.Count > 0) //有配置数据权限
            {
                List<Guid?> tempOrgIds = orgIds.Where(x => x != "-1").Select(x => x.ObjToGuidNull()).ToList();
                if (orgIds.Contains(Guid.Empty.ObjToStr()) && userInfo.OrganizationId.HasValue && userInfo.OrganizationId.Value != Guid.Empty &&
                        !orgIds.Contains(userInfo.OrganizationId.Value.ToString()))
                    tempOrgIds.Add(userInfo.OrganizationId.Value);
                if (tempOrgIds.Count > 0)
                    return tempOrgIds.Contains(t.OrgId);
                else
                    return t.CreateUserId == userId;
            }
            else if (typeof(T).BaseType != null && typeof(T).BaseType.Name == "BaseSysEntity")
            {
                return type == 0 ? true : t.CreateUserId == userId;
            }
            return t.CreateUserId == userId;
        }

        /// <summary>
        /// 获取权限表达式
        /// </summary>
        /// <param name="filterWhere">过虑条件SQL</param>
        /// <param name="queryCache">是否从缓存中查询</param>
        /// <returns></returns>
        private Expression<Func<T, bool>> GetPermissionExp(out string filterWhere, bool queryCache)
        {
            filterWhere = string.Empty;
            UserInfo userInfo = currUser;
            if (userInfo == null) return x => x.CreateUserId == Guid.Empty;
            if (UserInfo.IsSuperAdmin(userInfo)) return null;
            Guid userId = userInfo.UserId;
            //先检查有没有自定义权限处理，有则先调用各模块的自定义权限过滤处理，没有调用通用权限处理
            try
            {
                object instance = null;
                object typesObj = Globals.ExecuteReflectMethod("Rookey.Frame.Bridge", "BridgeObject", "GetCustomerOperateHandleTypes", null, ref instance, true);
                if (typesObj != null)
                {
                    List<Type> types = typesObj as List<Type>;
                    Type modelType = typeof(T);
                    Type modelOpType = types.Where(x => x.Name == string.Format("{0}OperateHandle", modelType.Name)).FirstOrDefault();
                    if (modelOpType != null)
                    {
                        Type[] modelOpInterfaceTypes = modelOpType.GetInterfaces();
                        if (types != null && types.Count > 0 && modelOpType != null && modelOpInterfaceTypes != null && modelOpInterfaceTypes.Where(x => x.Name.StartsWith("IPermissionHandle")).FirstOrDefault() != null)
                        {
                            Type operateHandleType = types.Where(x => x.Namespace == "Rookey.Frame.Operate.Base.OperateHandle" && x.Name.StartsWith("OperateHandleFactory")).FirstOrDefault();
                            if (operateHandleType != null)
                            {
                                Type[] argsType = new Type[] { modelType };
                                object[] args = new object[] { userInfo, filterWhere, queryCache };
                                object handleInstance = null;
                                object exp = Globals.ExecuteReflectMethod(operateHandleType, argsType, "GetPermissionExp", args, ref handleInstance);
                                if (handleInstance != null)
                                {
                                    filterWhere = args[1].ObjToStr();
                                    Expression<Func<T, bool>> temExp = exp as Expression<Func<T, bool>>;
                                    if (queryCache)
                                    {
                                        filterWhere = string.Empty;
                                    }
                                    return temExp;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            //调用通用权限处理
            if (queryCache)
                return GetPermissionFilterExpression();
            else
                filterWhere = GetPermissionFilterWhere();
            return null;
        }

        #endregion

        #endregion

        #region 实体查询

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">查询条件</param>
        /// <param name="whereSql">SQL条件语句</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public virtual T GetEntity(out string errorMsg, Expression<Func<T, bool>> expression = null, string whereSql = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                bool isNotGetFromCache = cacheFactory == null;
                if (typeof(T).Name == "Sys_User" && fieldNames != null && fieldNames.Count == 0) //针对用户时的特殊处理
                {
                    isNotGetFromCache = true; //从数据库中取
                }
                //没有启用缓存时从数据库取
                if (isNotGetFromCache)
                {
                    return dalFactory.GetEntity(out errorMsg, expression, whereSql, fieldNames, references, connString);
                }
                //启用了缓存从缓存中取
                string tempCacheKey = !string.IsNullOrEmpty(whereSql) ? string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(whereSql)) : cacheKey;
                List<T> list = cacheFactory.Get<List<T>>(tempCacheKey);
                if (list == null) //缓存中没有数据
                {
                    //从数据库中取一条
                    T tempT = dalFactory.GetEntity(out errorMsg, expression, whereSql, fieldNames, references, connString);
                    if (isAsyncUpCache)
                    {
                        //异步缓存数据
                        Task.Factory.StartNew(() =>
                        {
                            this.CacheData(whereSql, connString);
                        });
                    }
                    else
                    {
                        this.CacheData(whereSql, connString);
                    }
                    return tempT;
                }
                T t = expression != null ?
                      list.Where(expression.Compile()).FirstOrDefault() :
                      list.FirstOrDefault();
                return t;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 通过主键Id获取实体
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="id">主键Id</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public virtual T GetEntityById(out string errorMsg, object id, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (cacheFactory != null) //启用缓存
                {
                    List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                    if (list == null) //缓存中没有数据
                    {
                        T tempT = dalFactory.GetEntityById(out errorMsg, id, fieldNames, references, connString);
                        if (isAsyncUpCache)
                        {
                            //异步缓存数据
                            Task.Factory.StartNew(() =>
                            {
                                this.CacheData(null, connString);
                            });
                        }
                        else
                        {
                            this.CacheData(null, connString);
                        }
                        return tempT;
                    }
                    T t = list.Where(x => x.Id == id.ObjToGuid()).FirstOrDefault();
                    return t;
                }
                else
                {
                    return dalFactory.GetEntityById(out errorMsg, id, fieldNames, references, connString);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 根据字段获取记录
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段值</param>
        /// <param name="permissionFilter">是否进行权限过滤</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">是否降序</param>
        /// <param name="top">取前几条记录</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public virtual List<T> GetEntitiesByField(out string errorMsg, string fieldName, object fieldValue, bool permissionFilter = true, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                bool queryCache = cacheFactory != null; //是否从缓存中查
                List<T> list = new List<T>();
                if (cacheFactory != null) //启用缓存
                {
                    list = cacheFactory.Get<List<T>>(cacheKey);
                    queryCache = list != null;
                }
                Expression<Func<T, bool>> tempExp = null;
                string tempWhere = null;
                if (permissionFilter) //启用权限过滤
                {
                    tempExp = GetPermissionExp(out tempWhere, queryCache);
                }
                if (queryCache) //从缓存中查询
                {
                    if (tempExp != null)
                    {
                        list = list.Where(tempExp.Compile()).ToList();
                    }
                    PropertyInfo p = typeof(T).GetProperty(fieldName);
                    if (p == null) return new List<T>();
                    List<T> data = new List<T>();
                    foreach (T t in list)
                    {
                        object obj1 = null;
                        try
                        {
                            obj1 = TypeUtil.ChangeType(p.GetValue2(t, null), p.PropertyType);
                            if (obj1 != null)
                            {
                                object obj2 = null;
                                try
                                {
                                    obj2 = TypeUtil.ChangeType(fieldValue, p.PropertyType);
                                }
                                catch { }
                                if (obj1 != null && obj1.Equals(obj2))
                                {
                                    data.Add(t);
                                }
                            }
                            else if (fieldValue == null)
                            {
                                data.Add(t);
                            }
                        }
                        catch { }
                    }
                    if (orderFields != null && orderFields.Count > 0)
                    {
                        for (int i = 0; i < orderFields.Count; i++)
                        {
                            string orderField = string.IsNullOrEmpty(orderFields[i]) ? defaultSortField : orderFields[i];
                            bool isdesc = isDescs != null && orderFields.Count == isDescs.Count ? isDescs[i] : true;
                            SortComparer<T> reverser = new SortComparer<T>(typeof(T), orderField, isdesc ? ReverserInfo.Direction.DESC : ReverserInfo.Direction.ASC);
                            data.Sort(reverser);
                        }
                    }
                    if (top.HasValue && top.Value > 0)
                    {
                        data = data.Take(top.Value).ToList();
                    }
                    return data;
                }
                else //从数据库中查询
                {
                    if (cacheFactory != null) //启用了缓存
                    {
                        if (isAsyncUpCache)
                        {
                            //异步缓存数据
                            Task.Factory.StartNew(() =>
                            {
                                this.CacheData(tempWhere, connString);
                            });
                        }
                        else
                        {
                            this.CacheData(tempWhere, connString);
                        }
                    }
                    list = dalFactory.GetEntitiesByField(out errorMsg, fieldName, fieldValue, tempExp, tempWhere, orderFields, isDescs, top, fieldNames, references, connString);
                    return list;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return new List<T>();
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="whereSql">SQL条件语句</param>
        /// <param name="permissionFilter">是否进行权限过滤</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">是否降序排序</param>
        /// <param name="top">取前多少条</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public virtual List<T> GetEntities(out string errorMsg, Expression<Func<T, bool>> expression = null, string whereSql = null, bool permissionFilter = true, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string tempCacheKey = !string.IsNullOrEmpty(whereSql) ? string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(whereSql)) : cacheKey;
                bool queryCache = cacheFactory != null; //是否从缓存中查
                List<T> list = new List<T>();
                if (cacheFactory != null)
                {
                    list = cacheFactory.Get<List<T>>(tempCacheKey);
                    queryCache = list != null;
                }
                Expression<Func<T, bool>> tempExp = expression;
                string tempWhere = whereSql;
                if (permissionFilter) //启用权限过滤
                {
                    string filterWhere = string.Empty;
                    Expression<Func<T, bool>> exp = GetPermissionExp(out filterWhere, queryCache);
                    if (exp != null)
                    {
                        tempExp = tempExp != null ? ExpressionExtension.And(exp, tempExp) : exp;
                    }
                    if (!string.IsNullOrEmpty(filterWhere))
                    {
                        if (!string.IsNullOrEmpty(tempWhere))
                            tempWhere += " AND ";
                        tempWhere += filterWhere;
                    }
                }
                if (queryCache) //从缓存中查询
                {
                    //启用了缓存从缓存中取
                    List<T> data = tempExp != null ? list.Where(tempExp.Compile()).ToList() : list;
                    if (orderFields != null && orderFields.Count > 0)
                    {
                        for (int i = 0; i < orderFields.Count; i++)
                        {
                            string orderField = string.IsNullOrEmpty(orderFields[i]) ? defaultSortField : orderFields[i];
                            bool isdesc = isDescs != null && orderFields.Count == isDescs.Count ? isDescs[i] : true;
                            SortComparer<T> reverser = new SortComparer<T>(typeof(T), orderField, isdesc ? ReverserInfo.Direction.DESC : ReverserInfo.Direction.ASC);
                            data.Sort(reverser);
                        }
                    }
                    if (top.HasValue && top.Value > 0)
                    {
                        data = data.Take(top.Value).ToList();
                    }
                    return data;
                }
                else //从数据库中查
                {
                    if (cacheFactory != null) //启用了缓存
                    {
                        if (isAsyncUpCache)
                        {
                            //异步缓存数据
                            Task.Factory.StartNew(() =>
                            {
                                this.CacheData(tempWhere, connString);
                            });
                        }
                        else
                        {
                            this.CacheData(tempWhere, connString);
                        }
                    }
                    list = dalFactory.GetEntities(out errorMsg, tempExp, tempWhere, orderFields, isDescs, top, fieldNames, references, connString);
                    return list;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return new List<T>();
        }

        /// <summary>
        /// 获取分页实体集合
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionFilter">是否进行权限过滤</param>
        /// <param name="pageIndex">页号，最小为1，默认为1</param>
        /// <param name="pageSize">每页记录数，最大为2000，默认为10</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">降序排序</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="whereSql">SQL条件语句</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public virtual List<T> GetPageEntities(out long totalCount, out string errorMsg, bool permissionFilter = true, int pageIndex = 1, int pageSize = 10, List<string> orderFields = null, List<bool> isDescs = null, Expression<Func<T, bool>> expression = null, string whereSql = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            totalCount = 0;
            errorMsg = string.Empty;
            try
            {
                string tempCacheKey = !string.IsNullOrEmpty(whereSql) ? string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(whereSql)) : cacheKey;
                //页序号
                int index = pageIndex < 1 ? 0 : (pageIndex - 1);
                //每页记录数
                int rows = pageSize < 1 ? 10 : (pageSize > 2000 ? 2000 : pageSize);
                bool queryCache = cacheFactory != null; //是否从缓存中查
                List<T> list = new List<T>();
                if (cacheFactory != null) //启用了缓存
                {
                    list = cacheFactory.Get<List<T>>(tempCacheKey);
                    queryCache = list != null;
                }
                Expression<Func<T, bool>> tempExp = expression;
                string tempWhere = whereSql;
                if (permissionFilter) //启用权限过滤
                {
                    string filterWhere = string.Empty;
                    Expression<Func<T, bool>> exp = GetPermissionExp(out filterWhere, queryCache);
                    if (exp != null)
                    {
                        tempExp = tempExp != null ? ExpressionExtension.And(exp, tempExp) : exp;
                    }
                    if (!string.IsNullOrEmpty(filterWhere))
                    {
                        if (!string.IsNullOrEmpty(tempWhere))
                            tempWhere += " AND ";
                        tempWhere += filterWhere;
                    }
                }
                if (queryCache) //从缓存中取
                {
                    List<T> data = tempExp != null ? list.Where(tempExp.Compile()).ToList() : list;
                    //排序
                    if (orderFields != null && orderFields.Count > 0)
                    {
                        for (int i = 0; i < orderFields.Count; i++)
                        {
                            string orderField = string.IsNullOrEmpty(orderFields[i]) ? defaultSortField : orderFields[i];
                            bool isdesc = isDescs != null && orderFields.Count == isDescs.Count ? isDescs[i] : true;
                            SortComparer<T> reverser = new SortComparer<T>(typeof(T), orderField, isdesc ? ReverserInfo.Direction.DESC : ReverserInfo.Direction.ASC);
                            data.Sort(reverser);
                        }
                    }
                    totalCount = data.Count;
                    data = data.Skip<T>(rows * index).Take<T>(rows).ToList();
                    return data;
                }
                else //从数据库中取
                {
                    if (cacheFactory != null) //启用了缓存
                    {
                        if (isAsyncUpCache)
                        {
                            //异步缓存数据
                            Task.Factory.StartNew(() =>
                            {
                                this.CacheData(tempWhere, connString);
                            });
                        }
                        else
                        {
                            this.CacheData(tempWhere, connString);
                        }
                    }
                    list = dalFactory.GetPageEntities(out totalCount, out errorMsg, pageIndex, pageSize, orderFields, isDescs, tempExp, tempWhere, fieldNames, references, connString);
                    return list;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return new List<T>();
        }

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionFilter">是否启用权限过滤</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="whereSql">SQL条件语句</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public virtual long Count(out string errorMsg, bool permissionFilter = true, Expression<Func<T, bool>> expression = null, string whereSql = null, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string tempCacheKey = !string.IsNullOrEmpty(whereSql) ? string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(whereSql)) : cacheKey;
                bool queryCache = cacheFactory != null; //是否从缓存中查
                List<T> list = new List<T>();
                if (cacheFactory != null) //启用了缓存
                {
                    list = cacheFactory.Get<List<T>>(tempCacheKey);
                    queryCache = list != null;
                }
                Expression<Func<T, bool>> tempExp = expression;
                string tempWhere = whereSql;
                if (permissionFilter) //启用权限过滤
                {
                    string filterWhere = string.Empty;
                    Expression<Func<T, bool>> exp = GetPermissionExp(out filterWhere, queryCache);
                    if (exp != null)
                    {
                        tempExp = tempExp != null ? ExpressionExtension.And(exp, tempExp) : exp;
                    }
                    if (!string.IsNullOrEmpty(filterWhere))
                    {
                        if (!string.IsNullOrEmpty(tempWhere))
                            tempWhere += " AND ";
                        tempWhere += filterWhere;
                    }
                }
                if (queryCache) //从缓存中取
                {
                    List<T> data = tempExp != null ? list.Where(tempExp.Compile()).ToList() : list;
                    return data.Count;
                }
                else //从数据库中取
                {
                    if (cacheFactory != null) //启用了缓存
                    {
                        if (isAsyncUpCache)
                        {
                            //异步缓存数据
                            Task.Factory.StartNew(() =>
                            {
                                this.CacheData(tempWhere, connString);
                            });
                        }
                        else
                        {
                            this.CacheData(tempWhere, connString);
                        }
                    }
                    long count = dalFactory.Count(out errorMsg, tempExp, tempWhere, connString);
                    return count;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// 加载实体对象的关联对象（导航属性）
        /// </summary>
        /// <param name="instance">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public virtual void LoadReferences(T instance, out string errorMsg, string connString = null)
        {
            dalFactory.LoadReferences(instance, out errorMsg, connString);
        }

        /// <summary>
        /// 加载关联对象（导航属性）
        /// </summary>
        /// <param name="instances">实体对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public virtual void LoadListReferences(List<T> instances, out string errorMsg, string connString = null)
        {
            dalFactory.LoadListReferences(instances, out errorMsg, connString);
        }

        /// <summary>
        /// 获取实体某个字段值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public virtual object Scalar(Expression<Func<T, object>> field, Expression<Func<T, bool>> expression, out string errorMsg, string connString = null)
        {
            return dalFactory.Scalar(field, expression, out errorMsg, connString);
        }

        #endregion

        #region 新增实体

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回实体Id</returns>
        public virtual Guid AddEntity(T entity, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (entity.Id == null || entity.Id == Guid.Empty)
                {
                    entity.Id = Guid.NewGuid();
                }
                Guid id = dalFactory.AddEntity(entity, out errorMsg, references, connString, transConn);
                if (cacheFactory != null && id != Guid.Empty) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                            if (cacheType == CacheProviderType.LOCALMEMORYCACHE && list != null && list.Count > 0)
                            {
                                lock (lockObj)
                                {
                                    list.Add(entity);
                                }
                            }
                            else
                            {
                                this.CacheData(null, connString);
                            }
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public virtual bool AddEntities(List<T> entities, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                foreach (T t in entities)
                {
                    if (t.Id == null || t.Id == Guid.Empty)
                    {
                        t.Id = Guid.NewGuid();
                    }
                }
                bool rs = dalFactory.AddEntities(entities, out errorMsg, references, connString, transConn);
                if (cacheFactory != null && rs) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                            if (cacheType == CacheProviderType.LOCALMEMORYCACHE && list != null && list.Count > 0)
                            {
                                lock (lockObj)
                                {
                                    list.AddRange(entities);
                                }
                            }
                            else
                            {
                                this.CacheData(null, connString);
                            }
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        #endregion

        #region 更新实体

        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <param name="references">是否保存关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回更新结果</returns>
        public virtual bool UpdateEntity(T entity, out string errorMsg, bool permissionValidate = true, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (permissionValidate)
                {
                    if (!HasRecordOperatePermission(entity, 1))
                    {
                        errorMsg = "您无编辑此记录的权限，如有疑问请联系管理员！";
                        return false;
                    }
                }
                bool rs = dalFactory.UpdateEntity(entity, out errorMsg, references, connString, transConn);
                if (cacheFactory != null && rs) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                            if (list != null && list.Count > 0 && cacheType == CacheProviderType.LOCALMEMORYCACHE)
                            {
                                lock (lockObj)
                                {
                                    T tempT = list.Where(x => x.Id == entity.Id).FirstOrDefault();
                                    if (tempT != null) //存在更新属性
                                        ObjectHelper.CopyValue<T>(entity, tempT);
                                    else //不存在添加到缓存列表中
                                        list.Add(entity);
                                }
                            }
                            else
                            {
                                this.CacheData(null, connString);
                            }
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <param name="references">是否保存关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public virtual bool UpdateEntities(List<T> entities, out string errorMsg, bool permissionValidate = true, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (entities == null || entities.Count == 0) return true;
                List<T> ts = permissionValidate ? entities.Where(x => HasRecordOperatePermission(x, 1)).ToList() : entities;
                bool rs = dalFactory.UpdateEntities(ts, out errorMsg, references, connString, transConn);
                if (cacheFactory != null && rs) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                            if (list != null && list.Count > 0 && cacheType == CacheProviderType.LOCALMEMORYCACHE)
                            {
                                lock (lockObj)
                                {
                                    foreach (T t in ts)
                                    {
                                        T tempT = list.Where(x => x.Id == t.Id).FirstOrDefault();
                                        if (tempT != null) //存在更新属性
                                            ObjectHelper.CopyValue<T>(t, tempT);
                                        else //不存在添加到缓存列表中
                                            list.Add(t);
                                    }
                                }
                            }
                            else
                            {
                                this.CacheData(null, connString);
                            }
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 更新实体字段
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="fieldNames">要更新的字段</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public virtual bool UpdateEntityFields(T entity, List<string> fieldNames, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (permissionValidate)
                {
                    if (!HasRecordOperatePermission(entity, 1))
                    {
                        errorMsg = "您无编辑此记录的权限，如有疑问请联系管理员！";
                        return false;
                    }
                }
                bool rs = dalFactory.UpdateEntityFields(entity, fieldNames, out errorMsg, connString, transConn);
                if (cacheFactory != null && rs) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                            if (list != null && list.Count > 0 && cacheType == CacheProviderType.LOCALMEMORYCACHE)
                            {
                                lock (lockObj)
                                {
                                    T tempT = list.Where(x => x.Id == entity.Id).FirstOrDefault();
                                    if (tempT != null) //存在更新属性
                                        ObjectHelper.CopyValue<T>(entity, tempT, fieldNames);
                                    else //不存在添加到缓存列表中
                                        list.Add(entity);
                                }
                            }
                            else
                            {
                                this.CacheData(null, connString);
                            }
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 通过表达式更新实体
        ///  UpdateEntityByExpression(new { FirstName = "JJ" }, p => p.LastName == "Hendrix");
        ///  UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// </summary>
        /// <param name="obj">匿名对象</param>
        /// <param name="expression">表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public virtual bool UpdateEntityByExpression(object obj, Expression<Func<T, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                bool rs = dalFactory.UpdateEntityByExpression(obj, expression, out errorMsg, connString, transConn);
                if (cacheFactory != null && rs) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            this.CacheData(null, connString);
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        #endregion

        #region 删除实体

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="expression">条件</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回结果</returns>
        public virtual bool DeleteEntity(Expression<Func<T, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                bool rs = dalFactory.DeleteEntity(expression, out errorMsg, connString, transConn);
                if (cacheFactory != null && rs) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                            if (list != null && list.Count > 0 && cacheType == CacheProviderType.LOCALMEMORYCACHE)
                            {
                                lock (lockObj)
                                {
                                    List<T> tempList = list.Where(expression.Compile()).ToList();
                                    foreach (T t in tempList)
                                    {
                                        list.Remove(t);
                                    }
                                }
                            }
                            else
                            {
                                this.CacheData(null, connString);
                            }
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 通过主键Id删除实体
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public virtual bool DeleteEntityById(object id, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (permissionValidate)
                {
                    T t = GetEntityById(out errorMsg, id);
                    if (t == null)
                    {
                        errorMsg = "记录不存在，无法删除！";
                        return false;
                    }
                    if (!HasRecordOperatePermission(t, 2))
                    {
                        errorMsg = "您无删除此记录的权限，如有疑问请联系管理员！";
                        return false;
                    }
                }
                bool rs = dalFactory.DeleteEntityById(id, out errorMsg, connString, transConn);
                if (cacheFactory != null && rs) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                            if (list != null && list.Count > 0 && cacheType == CacheProviderType.LOCALMEMORYCACHE)
                            {
                                lock (lockObj)
                                {
                                    T tempT = list.Where(x => x.Id == id.ObjToGuid()).FirstOrDefault();
                                    if (tempT != null)
                                        list.Remove(tempT);
                                }
                            }
                            else
                            {
                                this.CacheData(null, connString);
                            }
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 通过主键Id集合删除实体
        /// </summary>
        /// <param name="ids">主键Id集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public virtual bool DeleteEntityByIds(IEnumerable ids, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                List<Guid> tempIds = new List<Guid>();
                if (permissionValidate)
                {
                    foreach (object id in ids)
                    {
                        T t = GetEntityById(out errorMsg, id);
                        if (t == null) continue;
                        if (!HasRecordOperatePermission(t, 2))
                            continue;
                        tempIds.Add(id.ObjToGuid());
                    }
                    if (tempIds.Count == 0)
                    {
                        errorMsg = "没有可更新的记录，或没有编辑数据的权限！";
                        return false;
                    }
                }
                else
                {
                    foreach (object id in ids)
                    {
                        tempIds.Add(id.ObjToGuid());
                    }
                }
                bool rs = dalFactory.DeleteEntityByIds(tempIds, out errorMsg, connString, transConn);
                if (cacheFactory != null && rs) //启用缓存
                {
                    if (isAsyncUpCache)
                    {
                        //异步更新缓存
                        Task.Factory.StartNew(() =>
                        {
                            List<T> list = cacheFactory.Get<List<T>>(cacheKey);
                            if (list != null && list.Count > 0 && cacheType == CacheProviderType.LOCALMEMORYCACHE)
                            {
                                lock (lockObj)
                                {
                                    List<T> dels = list.Where(x => tempIds.Contains(x.Id)).ToList();
                                    foreach (T t in dels)
                                        list.Remove(t);
                                }
                            }
                            else
                            {
                                this.CacheData(null, connString);
                            }
                        });
                    }
                    else
                    {
                        this.CacheData(null, connString);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        #endregion

        #region SQL方式

        /// <summary>
        /// 以SQL方式获取实体集合
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public List<T> GetEntitiesBySql(out string errMsg, string sql, object[] sqlParams = null, string connString = null)
        {
            errMsg = string.Empty;
            List<T> list = new List<T>();
            //启用缓存，从缓存中取
            string tempCacheKey = string.Empty;
            if (cacheFactory != null && !string.IsNullOrEmpty(sql))
            {
                if (sqlParams != null)
                {
                    try
                    {
                        string json = JsonHelper.Serialize(sqlParams);
                        json = string.Format("{0}_{1}", sql, json);
                        tempCacheKey = string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(json.ToLower()));
                        list = cacheFactory.Get<List<T>>(tempCacheKey);
                    }
                    catch
                    { }
                }
                else
                {
                    tempCacheKey = string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(sql.ToLower()));
                    list = cacheFactory.Get<List<T>>(tempCacheKey);
                }
                if (list.Count > 0)
                    return list;
            }
            //未启用缓存或缓存中不存在从数据库中取
            list = dalFactory.GetEntitiesBySql(out errMsg, sql, sqlParams, connString);
            if (!string.IsNullOrEmpty(tempCacheKey) && list != null && list.Count > 0)
            {
                cacheFactory.Set<List<T>>(tempCacheKey, list);
                if (!cacheKeyList.Contains(tempCacheKey))
                    cacheKeyList.Add(tempCacheKey);
            }
            return list;
        }

        /// <summary>
        /// 以SQL方式执行查询
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public DataTable ExecuteQuery(out string errMsg, string sql, Dictionary<string, object> sqlParams = null, string connString = null)
        {
            errMsg = string.Empty;
            DataTable dt = null;
            //启用缓存，从缓存中取
            string tempCacheKey = string.Empty;
            if (cacheFactory != null && !string.IsNullOrEmpty(sql))
            {
                if (sqlParams != null && sqlParams.Count > 0)
                {
                    try
                    {
                        string json = JsonHelper.Serialize(sqlParams);
                        json = string.Format("{0}_{1}", sql, json);
                        tempCacheKey = string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(json.ToLower()));
                        dt = cacheFactory.Get<DataTable>(tempCacheKey);
                    }
                    catch
                    { }
                }
                else
                {
                    tempCacheKey = string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(sql.ToLower()));
                    dt = cacheFactory.Get<DataTable>(tempCacheKey);
                }
                if (dt != null && dt.Rows.Count > 0)
                    return dt;
            }
            dt = dalFactory.ExecuteQuery(out errMsg, sql, sqlParams, connString);
            if (!string.IsNullOrEmpty(tempCacheKey) && dt != null && dt.Rows.Count > 0)
            {
                cacheFactory.Set<DataTable>(tempCacheKey, dt);
                if (!cacheKeyList.Contains(tempCacheKey))
                    cacheKeyList.Add(tempCacheKey);
            }
            return dt;
        }

        /// <summary>
        /// 获取查询到的第一行第一列的数据
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public object ExecuteScale(out string errMsg, string sql, object[] sqlParams = null, string connString = null)
        {
            errMsg = string.Empty;
            return dalFactory.ExecuteScale(out errMsg, sql, sqlParams, connString);
        }

        /// <summary>
        /// 执行增删改语句
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="sql">sql</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(out string errMsg, string sql, Dictionary<string, object> sqlParams = null, string connString = null, IDbConnection transConn = null)
        {
            errMsg = string.Empty;
            return dalFactory.ExecuteNonQuery(out errMsg, sql, sqlParams, connString, transConn);
        }

        /// <summary>
        /// 执行存储过程，针对非查询
        /// </summary>
        /// <param name="errMsg">异常信息</param>
        /// <param name="outParams">输出参数</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="inParams">输入参数 eg:new{Age=30}</param>
        /// <param name="connString">连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回受影响的行数</returns>
        public int RunProcedureNoQuery(out string errMsg, ref Dictionary<string, object> outParams, string procedureName, object inParams = null, string connString = null, IDbConnection transConn = null)
        {
            errMsg = string.Empty;
            return dalFactory.RunProcedureNoQuery(out errMsg, ref outParams, procedureName, inParams, connString, transConn);
        }

        /// <summary>
        /// 执行存储过程，针对查询
        /// </summary>
        /// <param name="errMsg">异常信息</param>
        /// <param name="outParams">输出参数</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="inParams">输入参数 eg:new{Age=30}</param>
        /// <param name="connString">连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回查询记录</returns>
        public DataTable RunProcedure(out string errMsg, ref Dictionary<string, object> outParams, string procedureName, object inParams = null, string connString = null, IDbConnection transConn = null)
        {
            errMsg = string.Empty;
            DataTable dt = null;
            //启用缓存，从缓存中取
            string tempCacheKey = string.Empty;
            if (cacheFactory != null && !string.IsNullOrEmpty(procedureName))
            {
                if (inParams != null)
                {
                    try
                    {
                        string json = JsonHelper.Serialize(inParams);
                        json = string.Format("{0}_{1}", procedureName, json);
                        tempCacheKey = string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(json.ToLower()));
                        dt = cacheFactory.Get<DataTable>(tempCacheKey);
                    }
                    catch
                    { }
                }
                else
                {
                    tempCacheKey = string.Format("{0}_{1}", cacheKey, MySecurity.EncodeBase64(procedureName.ToLower()));
                    dt = cacheFactory.Get<DataTable>(tempCacheKey);
                }
                if (dt != null && dt.Rows.Count > 0)
                    return dt;
            }
            dt = dalFactory.RunProcedure(out errMsg, ref outParams, procedureName, inParams, connString, transConn);
            if (!string.IsNullOrEmpty(tempCacheKey) && dt != null && dt.Rows.Count > 0)
            {
                cacheFactory.Set<DataTable>(tempCacheKey, dt);
                if (!cacheKeyList.Contains(tempCacheKey))
                    cacheKeyList.Add(tempCacheKey);
            }
            return dt;
        }

        #endregion

        #region 事务操作

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transactionObjects">事务对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public void ExecuteTransaction(List<TransactionModel<T>> transactionObjects, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            dalFactory.ExecuteTransaction(transactionObjects, out errorMsg, connString);
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transactionObjects">事务扩展对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public void ExecuteTransactionExtend(List<TransactionExtendModel> transactionObjects, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            dalFactory.ExecuteTransactionExtend(transactionObjects, out errorMsg, connString);
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transTask">事务处理函数</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public void TransactionHandle(TransactionTask transTask, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            dalFactory.TransactionHandle(transTask, out errorMsg, connString);
        }

        #endregion

        #region 数据库操作

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>创建成功返回空字符串，失败返回异常信息</returns>
        public string CreateTable(string connString = null)
        {
            return dalFactory.CreateTable(connString);
        }

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>删除成功返回空字符串，失败返回异常信息</returns>
        public string DropTable(string connString = null)
        {
            return dalFactory.DropTable(connString);
        }

        /// <summary>
        /// 修改数据表
        /// </summary>
        /// <param name="command">操作sql，如：[ALTER TABLE a] ADD Column b int</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public string AlterTable(string command, string connString = null)
        {
            return dalFactory.AlterTable(command, connString);
        }

        /// <summary>
        /// 数据列是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public bool ColumnIsExists(string fieldName, string connString = null)
        {
            return dalFactory.ColumnIsExists(fieldName, connString);
        }

        /// <summary>
        /// 增加数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public string AddColumn(string fieldName, string connString = null)
        {
            return dalFactory.AddColumn(fieldName, connString);
        }

        /// <summary>
        /// 修改数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public string AlterColumn(string fieldName, string connString = null)
        {
            return dalFactory.AlterColumn(fieldName, connString);
        }

        /// <summary>
        /// 修改列名
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="oldFieldName">旧的字段名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public string ChangeColumnName(string fieldName, string oldFieldName, string connString = null)
        {
            return dalFactory.ChangeColumnName(fieldName, oldFieldName, connString);
        }

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public string DropColumn(string columnName, string connString = null)
        {
            return dalFactory.DropColumn(columnName, connString);
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="indexName">索引名</param>
        /// <param name="unique">是否唯一索引</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public string CreateIndex(Expression<Func<T, object>> field, string indexName = null, bool unique = false, string connString = null)
        {
            return dalFactory.CreateIndex(field, indexName, unique, connString);
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public string DropIndex(string indexName, string connString = null)
        {
            return dalFactory.DropIndex(indexName, connString);
        }

        #endregion

        #region 其他

        /// <summary>
        /// 将lamda条件表达式转换成sql
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public string ExpressionConditionToWhereSql(Expression<Func<T, bool>> expression)
        {
            return dalFactory.ExpressionConditionToWhereSql(expression);
        }

        /// <summary>
        /// 清除当前模块缓存
        /// </summary>
        public void ClearCache()
        {
            if (cacheFactory != null)
            {
                cacheFactory.Remove(cacheKey);
                List<string> removeKeys = new List<string>();
                removeKeys.Add(cacheKey);
                foreach (string key in cacheKeyList)
                {
                    if (key.StartsWith(string.Format("{0}_", cacheKey)))
                    {
                        cacheFactory.Remove(key);
                        removeKeys.Add(key);
                    }
                }
                foreach (string key in removeKeys)
                {
                    cacheKeyList.Remove(key);
                }
            }
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void ClearAllCache()
        {
            if (cacheFactory != null)
            {
                foreach (string key in cacheKeyList)
                {
                    cacheFactory.Remove(key);
                }
                cacheKeyList.Clear();
            }
        }

        /// <summary>
        /// 设置自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="data">数据</param>
        public void SetCustomerCache(string key, object data)
        {
            if (cacheFactory == null)
            {
                CacheProviderType cacheTypeEnum = CacheProviderType.LOCALMEMORYCACHE;
                //缓存工厂实例化
                ICacheProvider tempCacheFactory = CacheFactory.GetCacheInstance(cacheTypeEnum);
                tempCacheFactory.Set(key, data);
            }
            else
            {
                cacheFactory.Set(key, data);
            }
        }

        /// <summary>
        /// 移除自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        public void RemoveCustomerCache(string key)
        {
            if (cacheFactory == null)
            {
                CacheProviderType cacheTypeEnum = CacheProviderType.LOCALMEMORYCACHE;
                //缓存工厂实例化
                ICacheProvider tempCacheFactory = CacheFactory.GetCacheInstance(cacheTypeEnum);
                tempCacheFactory.Remove(key);
            }
            else
            {
                cacheFactory.Remove(key);
            }
        }

        /// <summary>
        /// 获取自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        public object GetCustomerCache(string key)
        {
            if (cacheFactory == null)
            {
                CacheProviderType cacheTypeEnum = CacheProviderType.LOCALMEMORYCACHE;
                //缓存工厂实例化
                ICacheProvider tempCacheFactory = CacheFactory.GetCacheInstance(cacheTypeEnum);
                return tempCacheFactory.Get(key);
            }
            else
            {
                return cacheFactory.Get(key);
            }
        }

        #endregion
    }
}

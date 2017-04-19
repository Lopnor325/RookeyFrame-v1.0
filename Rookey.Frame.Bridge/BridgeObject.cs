/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Rookey.Frame.Common;
using Rookey.Frame.Common.PubDefine;
using System.ComponentModel;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Cache.Factory;
using Rookey.Frame.Cache.Factory.Provider;
using Rookey.Frame.Base;
using System.Collections.Concurrent;

namespace Rookey.Frame.Bridge
{
    /// <summary>
    /// 业务层桥接类
    /// </summary>
    public static class BridgeObject
    {
        #region 常量变量
        /// <summary>
        /// 默认实体层DLL
        /// </summary>
        private const string DEFAULT_MODEL_DLL = "Rookey.Frame.Model";
        /// <summary>
        /// 默认业务层接口DLL
        /// </summary>
        private const string DEFAULT_IBLL_DLL = "Rookey.Frame.IBLL";
        /// <summary>
        /// 默认业务层DLL
        /// </summary>
        private const string DEFAULT_BLL_DLL = "Rookey.Frame.BLL";
        /// <summary>
        /// 默认数据层接口DLL
        /// </summary>
        private const string DEFAULT_IDAL_DLL = "Rookey.Frame.IDAL";
        /// <summary>
        /// 默认数据层DLL
        /// </summary>
        private const string DEFAULT_DAL_DLL = "Rookey.Frame.DAL";
        //默认实体验证DLL
        private const string DEFAULT_MODEL_VALIDATOR_DLL = "Rookey.Frame.ModelValidator";
        /// <summary>
        /// 实体类型缓存键
        /// </summary>
        private const string cache_modelType = "cache_modelType";
        /// <summary>
        /// 实体验证类型缓存键
        /// </summary>
        private const string cache_modelValidateType = "cache_modelValidateType";
        /// <summary>
        /// 业务层接口类型缓存键
        /// </summary>
        private const string cache_ibllType = "cache_ibllType";
        /// <summary>
        /// 业务层类型缓存键
        /// </summary>
        private const string cache_bllType = "cache_bllType";
        /// <summary>
        /// 数据层接口类型缓存键
        /// </summary>
        private const string cache_idalType = "cache_idalType";
        /// <summary>
        /// 数据层类型缓存键
        /// </summary>
        private const string cache_dalType = "cache_dalType";
        /// <summary>
        /// 默认操作层dll
        /// </summary>
        private const string DEFAULT_OPERATE_DLL = "Rookey.Frame.Operate.Base";
        /// <summary>
        /// 自定义操作处理类型
        /// </summary>
        private const string cache_operateHandleType = "cache_operateHandleType";
        #endregion

        #region 变量定义

        /// <summary>
        /// 缓存对象
        /// </summary>
        private static ICacheProvider cacheFactory = CacheFactory.GetCacheInstance(CacheProviderType.LOCALMEMORYCACHE);

        /// <summary>
        /// 业务层、数据层实体缓存对象集合
        /// </summary>
        private static ConcurrentDictionary<string, object> interfaceObjCaches = new ConcurrentDictionary<string, object>();

        #endregion

        #region 接口实例化

        /// <summary>
        /// 实例化业务层或数据层对象
        /// </summary>
        /// <typeparam name="T">T为业务层接口类</typeparam>
        /// <param name="currUser">当前用户</param>
        /// <returns>返回业务层或数据层对象</returns>
        public static T Resolve<T>(UserInfo currUser) where T : class
        {
            Type type = typeof(T);
            return Resolve(currUser, type) as T;
        }

        /// <summary>
        /// 实例化业务层或数据层对象
        /// </summary>
        /// <typeparam name="T">T为业务层接口类</typeparam>
        /// <param name="currUser">当前用户</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns>返回业务层或数据层对象</returns>
        public static T Resolve<T>(UserInfo currUser, DatabaseType? dbType = null) where T : class
        {
            Type type = typeof(T);
            return Resolve(currUser, type, dbType) as T;
        }

        /// <summary>
        /// 实体化业务层或数据层对象
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <param name="type">业务层或数据层接口类型</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns>返回业务层或数据层对象</returns>
        public static object Resolve(UserInfo currUser, Type type, DatabaseType? dbType = null)
        {
            string key = type.FullName;
            if (currUser != null)
                key += currUser.UserId.ToString();
            object obj = null;
            if (interfaceObjCaches.ContainsKey(key))
            {
                interfaceObjCaches.TryGetValue(key, out obj);
                if (obj != null)
                    return obj;
            }
            if (type.Name.EndsWith("BLL") || (type.IsGenericType && type.Name.Contains("IBaseBLL")))
            {
                #region 实例化业务层对象
                List<Type> bllTypeList = GetAllBLLTypes();
                if (bllTypeList != null && bllTypeList.Count > 0)
                {
                    Type bllType = null;
                    if (type.IsGenericType && type.Name.Contains("IBaseBLL")) //业务层泛型类型
                    {
                        Type paramType = type.GetGenericArguments()[0]; //参数类型
                        //先判断有没有自定义业务层，有则调用自定义业务层
                        Type customerBLLType = bllTypeList.Where(x => x.Name == string.Format("{0}BLL", paramType.Name) && x.BaseType.Name.Contains("BaseBLL")).FirstOrDefault();
                        if (customerBLLType == null) //没有自定义业务层
                        {
                            Type genericBllType = bllTypeList.Where(x => x.Name.Contains("BaseBLL")).FirstOrDefault();
                            bllType = genericBllType.MakeGenericType(new Type[] { paramType });
                            obj = Activator.CreateInstance(bllType, new object[] { currUser, dbType });
                        }
                        else //有自定义业务层，实例化自定义业务层
                        {
                            obj = Activator.CreateInstance(customerBLLType, new object[] { currUser });
                        }
                    }
                    else //业务层类型
                    {
                        bllType = bllTypeList.Where(x => "I" + x.Name == type.Name).FirstOrDefault();
                        obj = Activator.CreateInstance(bllType, new object[] { currUser });
                    }
                }
                #endregion
            }
            else if (type.Name.EndsWith("DAL") || (type.IsGenericType && type.Name.Contains("IBaseDAL")))
            {
                #region 实例化数据层对象
                List<Type> dalTypeList = GetAllDALTypes();
                if (dalTypeList != null && dalTypeList.Count > 0)
                {
                    Type dalType = null;
                    if (type.IsGenericType && type.Name.Contains("IBaseDAL")) //数据层泛型类型
                    {
                        Type paramType = type.GetGenericArguments()[0]; //参数类型
                        //先判断有没有自定义数据层，有则调用自定义数据层
                        Type customerDALType = dalTypeList.Where(x => x.Name == string.Format("{0}DAL", paramType.Name) && x.BaseType.Name.Contains("BaseDAL")).FirstOrDefault();
                        if (customerDALType == null) //没有自定义业务层
                        {
                            Type genericBllType = dalTypeList.Where(x => x.Name.Contains("BaseDAL")).FirstOrDefault();
                            dalType = genericBllType.MakeGenericType(new Type[] { paramType });
                            obj = Activator.CreateInstance(dalType, new object[] { currUser, dbType });
                        }
                        else //有自定义数据层，实例化自定义数据层
                        {
                            obj = Activator.CreateInstance(customerDALType, new object[] { currUser });
                        }
                    }
                    else //数据层类型
                    {
                        dalType = dalTypeList.Where(x => "I" + x.Name == type.Name).FirstOrDefault();
                        obj = Activator.CreateInstance(dalType, new object[] { currUser });
                    }
                }
                #endregion
            }
            if (obj != null)
            {
                interfaceObjCaches.TryAdd(key, obj);
            }
            return obj;
        }

        #endregion

        #region 模块类型

        #region 获取DLL中的类型

        /// <summary>
        /// 获取DLL的所有类型
        /// </summary>
        /// <param name="dllName">DLL名称</param>
        /// <returns></returns>
        public static List<Type> GetTypesByDLL(string dllName)
        {
            List<Type> tempTypes = new List<Type>();
            if (string.IsNullOrEmpty(dllName)) return tempTypes;
            string basePath = Globals.GetBinPath();
            string dllPath = string.Format(@"{0}{1}.dll", basePath, dllName);
            if (!File.Exists(dllPath))
            {
                dllPath = string.Format(@"{0}{1}.exe", basePath, dllName);
                if (!File.Exists(dllPath))
                {
                    string tempBasePath = AppDomain.CurrentDomain.BaseDirectory;
                    if (!tempBasePath.EndsWith("\\"))
                        tempBasePath += "\\";
                    dllPath = string.Format(@"{0}{1}.dll", tempBasePath, dllName);
                    if (!File.Exists(dllPath))
                        return tempTypes;
                }
            }
            try
            {
                Assembly dllAssembly = Assembly.LoadFrom(dllPath);
                tempTypes = dllAssembly.GetTypes().ToList();
            }
            catch
            {
                try
                {
                    Assembly dllAssembly = Assembly.LoadFile(dllPath);
                    tempTypes = dllAssembly.GetTypes().ToList();
                }
                catch { }
            }
            return tempTypes;
        }

        #endregion

        #region 数据模型

        /// <summary>
        /// 获取所有的模块类型集合
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllModelTypes()
        {
            List<Type> tempTypes = new List<Type>();
            if (cacheFactory != null && cacheFactory.Exists(cache_modelType))
            {
                tempTypes = cacheFactory.Get<List<Type>>(cache_modelType);
                if (tempTypes == null) tempTypes = new List<Type>();
                return tempTypes;
            }
            string modelDll = DEFAULT_MODEL_DLL; //默认实体层dll
            //取自定义数据模型类型
            string customerModelBLL = WebConfigHelper.GetAppSettingValue("Model");
            if (!string.IsNullOrEmpty(customerModelBLL))
            {
                modelDll += string.Format(",{0}", customerModelBLL);
            }
            NotNullCheck.NotEmpty(modelDll, "Web.config－>appSettings中实体层程序集名称");
            List<Type> types = new List<Type>();
            if (!string.IsNullOrEmpty(modelDll))
            {
                string[] token = modelDll.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string dllName in token)
                {
                    types.AddRange(GetTypesByDLL(dllName));
                }
            }
            //添加临时实体类型集合
            List<Type> tempModelTypes = GetTempModelTypes();
            types.AddRange(tempModelTypes);
            //取模块类型集合
            foreach (Type type in types)
            {
                ModuleConfigAttribute moduleConfig = ((ModuleConfigAttribute)(Attribute.GetCustomAttribute(type, typeof(ModuleConfigAttribute))));
                NoModuleAttribute noModuleAttr = ((NoModuleAttribute)(Attribute.GetCustomAttribute(type, typeof(NoModuleAttribute))));
                if (moduleConfig == null && noModuleAttr == null) continue;
                tempTypes.Add(type);
            }
            if (tempTypes.Count > 0 && cacheFactory != null)
            {
                cacheFactory.Set<List<Type>>(cache_modelType, tempTypes);
            }
            return tempTypes;
        }

        /// <summary>
        /// 获取临时实体类型集合
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetTempModelTypes()
        {
            string basePath = Globals.GetBinPath();
            string dllPath = string.Format(@"{0}TempModel", basePath);
            if (!Directory.Exists(dllPath)) //临时实体dll目录不存在
            {
                return new List<Type>();
            }
            List<Type> list = new List<Type>();
            string[] files = Directory.GetFiles(dllPath);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Extension.ToLower() != ".dll")
                    continue;
                try
                {
                    Assembly dllAssembly = Assembly.LoadFrom(file);
                    List<Type> tempTypes = dllAssembly.GetTypes().Where(x => x.Namespace == "Rookey.Frame.TempModel").ToList();
                    list.AddRange(tempTypes);
                }
                catch
                { }
            }
            return list;
        }

        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="tableName">实体表名</param>
        /// <returns></returns>
        public static Type GetModelType(string tableName)
        {
            List<Type> types = GetAllModelTypes();
            return types.Where(x => x.Name == tableName).FirstOrDefault();
        }

        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public static Type GetModelTypeByModuleName(string moduleName)
        {
            List<Type> types = GetAllModelTypes();
            foreach (Type type in types)
            {
                ModuleConfigAttribute moduleConfig = ((ModuleConfigAttribute)(Attribute.GetCustomAttribute(type, typeof(ModuleConfigAttribute))));
                if (moduleConfig != null && moduleConfig.Name == moduleName)
                {
                    return type;
                }
            }
            return null;
        }

        #endregion

        #region 验证模型

        /// <summary>
        /// 获取所有的验证模块类型集合
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllFluentValidationModelTypes()
        {
            List<Type> tempTypes = new List<Type>();
            if (cacheFactory != null && cacheFactory.Exists(cache_modelValidateType))
            {
                tempTypes = cacheFactory.Get<List<Type>>(cache_modelValidateType);
                if (tempTypes == null) tempTypes = new List<Type>();
                return tempTypes;
            }
            string modelDll = DEFAULT_MODEL_VALIDATOR_DLL; //默认实体验证层dll
            //取自定义数据模型类型
            string customerModelBLL = WebConfigHelper.GetAppSettingValue("ModelValidator");
            if (!string.IsNullOrEmpty(customerModelBLL))
            {
                modelDll += string.Format(",{0}", customerModelBLL);
            }
            if (string.IsNullOrEmpty(modelDll))
            {
                return new List<Type>();
            }
            List<Type> types = new List<Type>();
            if (!string.IsNullOrEmpty(modelDll))
            {
                string[] token = modelDll.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string dllName in token)
                {
                    types.AddRange(GetTypesByDLL(dllName));
                }
            }
            tempTypes = types.Where(x => x.BaseType.Name.Contains("AbstractValidator")).ToList();
            if (tempTypes.Count > 0 && cacheFactory != null)
            {
                cacheFactory.Set<List<Type>>(cache_modelValidateType, tempTypes);
            }
            return tempTypes;
        }

        /// <summary>
        /// 获取验证实体类型
        /// </summary>
        /// <param name="tableName">实体表名</param>
        /// <returns></returns>
        public static Type GetFluentValidationModelType(string tableName)
        {
            List<Type> types = GetAllFluentValidationModelTypes();
            return types.Where(x => x.Name == string.Format("{0}Validator", tableName)).FirstOrDefault();
        }

        #endregion

        #region 业务层接口

        /// <summary>
        /// 获取当前所有业务接口类型集合
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllBLLInterfaceTypes()
        {
            List<Type> bllInterfaceTypeList = new List<Type>();
            if (cacheFactory != null && cacheFactory.Exists(cache_ibllType))
            {
                bllInterfaceTypeList = cacheFactory.Get<List<Type>>(cache_ibllType);
                if (bllInterfaceTypeList == null) bllInterfaceTypeList = new List<Type>();
                return bllInterfaceTypeList;
            }
            string ibllDll = DEFAULT_IBLL_DLL; //默认业务层接口dll
            //取自定义业务层接口类型
            string customerIBLL = WebConfigHelper.GetAppSettingValue("IBLL");
            if (!string.IsNullOrEmpty(customerIBLL))
            {
                ibllDll += string.Format(",{0}", customerIBLL);
            }
            List<Type> tempTypes = new List<Type>();
            if (!string.IsNullOrEmpty(ibllDll))
            {
                string[] token = ibllDll.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string dllName in token)
                {
                    tempTypes.AddRange(GetTypesByDLL(dllName));
                }
            }
            //过滤
            foreach (Type type in tempTypes)
            {
                if (type.IsInterface)
                {
                    bllInterfaceTypeList.Add(type);
                }
            }
            if (bllInterfaceTypeList.Count > 0 && bllInterfaceTypeList != null)
            {
                cacheFactory.Set<List<Type>>(cache_ibllType, bllInterfaceTypeList);
            }
            return bllInterfaceTypeList;
        }

        /// <summary>
        /// 获取数据模型类型对应的业务层接口类型
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static Type GetBLLInterfaceType(Type modelType)
        {
            List<Type> list = GetAllBLLInterfaceTypes();
            return list.Where(x => x.Name == string.Format("I{0}BLL", modelType.Name)).FirstOrDefault();
        }
        #endregion

        #region 业务层

        /// <summary>
        /// 获取当前所有业务类型集合
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllBLLTypes()
        {
            List<Type> bllTypeList = new List<Type>();
            if (cacheFactory != null && cacheFactory.Exists(cache_bllType))
            {
                bllTypeList = cacheFactory.Get<List<Type>>(cache_bllType);
                if (bllTypeList == null) bllTypeList = new List<Type>();
                return bllTypeList;
            }
            string bllDll = DEFAULT_BLL_DLL; //默认业务层DLL
            //取自定义业务层类型
            string customerBLL = WebConfigHelper.GetAppSettingValue("BLL");
            if (!string.IsNullOrEmpty(customerBLL))
            {
                bllDll += string.Format(",{0}", customerBLL);
            }
            List<Type> tempTypes = new List<Type>();
            if (!string.IsNullOrEmpty(bllDll))
            {
                string[] token = bllDll.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string dllName in token)
                {
                    tempTypes.AddRange(GetTypesByDLL(dllName));
                }
            }
            //过滤
            foreach (Type type in tempTypes)
            {
                if ((type.IsGenericType && type.Name.Contains("BaseBLL")) || (type.BaseType != null && type.BaseType.Name.Contains("BaseBLL")))
                {
                    bllTypeList.Add(type);
                }
            }
            if (bllTypeList.Count > 0 && cacheFactory != null)
            {
                cacheFactory.Set<List<Type>>(cache_bllType, bllTypeList);
            }
            return bllTypeList;
        }

        /// <summary>
        /// 获取数据模型类型对应的业务类型
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static Type GetBLLType(Type modelType)
        {
            List<Type> list = GetAllBLLTypes();
            return list.Where(x => x.Name == string.Format("{0}BLL", modelType.Name)).FirstOrDefault();
        }
        #endregion

        #region 数据层接口

        /// <summary>
        /// 获取所有数据层接口类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllDALInterfaceTypes()
        {
            List<Type> dalInterfaceTypeList = new List<Type>();
            if (cacheFactory != null && cacheFactory.Exists(cache_idalType))
            {
                dalInterfaceTypeList = cacheFactory.Get<List<Type>>(cache_idalType);
                if (dalInterfaceTypeList == null) dalInterfaceTypeList = new List<Type>();
                return dalInterfaceTypeList;
            }
            string idalDll = DEFAULT_IDAL_DLL; //默认数据层接口DLL
            //取自定义数据层接口类型
            string customerIDAL = WebConfigHelper.GetAppSettingValue("IDAL");
            if (!string.IsNullOrEmpty(customerIDAL))
            {
                idalDll += string.Format(",{0}", customerIDAL);
            }
            List<Type> tempTypes = new List<Type>();
            if (!string.IsNullOrEmpty(idalDll))
            {
                string[] token = idalDll.Split(",".ToCharArray());
                foreach (string dllName in token)
                {
                    tempTypes.AddRange(GetTypesByDLL(dllName));
                }
            }
            //过滤
            foreach (Type type in tempTypes)
            {
                Type[] types = type.GetInterfaces();
                if (type.Name == "IBaseDAL`1")
                {
                    dalInterfaceTypeList.Add(type);
                    continue;
                }
                if (types == null || types.Length == 0) continue;
                List<string> typeNames = types.Select(x => x.Name).ToList();
                if (typeNames.Contains("IBaseDAL"))
                {
                    dalInterfaceTypeList.Add(type);
                }
            }
            if (dalInterfaceTypeList.Count > 0 && cacheFactory != null)
            {
                cacheFactory.Set<List<Type>>(cache_idalType, dalInterfaceTypeList);
            }
            return dalInterfaceTypeList;
        }

        /// <summary>
        /// 获取数据模型类型对应的数据层接口类型
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static Type GetDALInterfaceType(Type modelType)
        {
            List<Type> list = GetAllDALInterfaceTypes();
            return list.Where(x => x.Name == string.Format("I{0}DAL", modelType.Name)).FirstOrDefault();
        }
        #endregion

        #region 数据层

        /// <summary>
        /// 获取所有数据层类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllDALTypes()
        {
            List<Type> dalTypeList = new List<Type>();
            if (cacheFactory != null && cacheFactory.Exists(cache_dalType))
            {
                dalTypeList = cacheFactory.Get<List<Type>>(cache_dalType);
                if (dalTypeList == null) dalTypeList = new List<Type>();
                return dalTypeList;
            }
            string dalDll = DEFAULT_DAL_DLL; //默认数据层DLL
            //取自定义数据层类型
            string customerDAL = WebConfigHelper.GetAppSettingValue("DAL");
            if (!string.IsNullOrEmpty(customerDAL))
            {
                dalDll += string.Format(",{0}", customerDAL);
            }
            List<Type> tempTypes = new List<Type>();
            if (!string.IsNullOrEmpty(dalDll))
            {
                string[] token = dalDll.Split(",".ToCharArray());
                foreach (string dllName in token)
                {
                    tempTypes.AddRange(GetTypesByDLL(dllName));
                }
            }
            //过滤
            foreach (Type type in tempTypes)
            {
                if ((type.IsGenericType && type.Name.Contains("BaseDAL")) || (type.BaseType != null && type.BaseType.Name.Contains("BaseDAL")))
                {
                    dalTypeList.Add(type);
                }
            }
            if (dalTypeList.Count > 0 && cacheFactory != null)
            {
                cacheFactory.Set<List<Type>>(cache_dalType, dalTypeList);
            }
            return dalTypeList;
        }

        /// <summary>
        /// 获取数据模型类型对应的数据层类型
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static Type GetDALType(Type modelType)
        {
            List<Type> list = GetAllDALTypes();
            return list.Where(x => x.Name == string.Format("{0}DAL", modelType.Name)).FirstOrDefault();
        }
        #endregion

        #region 自定义相关

        /// <summary>
        /// 获取业务层方法类型（主要有公共方法，重写方法，自定义方法）
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <param name="methodName">方法名称</param>
        /// <returns></returns>
        public static MethodCallTypeEnum GetBLLMethodType(Type modelType, string methodName)
        {
            if (modelType == null || string.IsNullOrEmpty(methodName))
                return MethodCallTypeEnum.CommonMethod;
            if (modelType.BaseType != null && modelType.Namespace.StartsWith("Rookey.Frame.Model"))
                return MethodCallTypeEnum.CommonMethod;
            List<Type> list = GetAllBLLTypes();
            Type customerBLLType = list.Where(x => x.Name == string.Format("{0}BLL", modelType.Name) && x.BaseType.Name.Contains("BaseBLL")).FirstOrDefault();
            if (customerBLLType == null) //没有自定义业务层
            {
                return MethodCallTypeEnum.CommonMethod;
            }
            MethodInfo method = customerBLLType.GetMethod(methodName);
            if (method == null) //没有该方法的定义说明是自定义方法
            {
                return MethodCallTypeEnum.CustomerMethod;
            }
            return method.DeclaringType.Name == string.Format("{0}BLL", modelType.Name) ?
                MethodCallTypeEnum.OverrideMethod : MethodCallTypeEnum.CommonMethod;
        }

        #endregion

        #region 自定义操作处理类型

        /// <summary>
        /// 获取自定义操作处理类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetCustomerOperateHandleTypes()
        {
            List<Type> list = new List<Type>();
            if (cacheFactory != null && cacheFactory.Exists(cache_operateHandleType))
            {
                list = cacheFactory.Get<List<Type>>(cache_operateHandleType);
                if (list == null) list = new List<Type>();
                return list;
            }
            string operateDll = DEFAULT_OPERATE_DLL; //默认操作层dll
            //取自定义操作类类型
            string customerOperateDll = WebConfigHelper.GetAppSettingValue("Operate");
            if (!string.IsNullOrEmpty(customerOperateDll))
            {
                operateDll += string.Format(",{0}", customerOperateDll);
            }
            string[] token = operateDll.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (token.Length > 0)
            {
                foreach (string str in token)
                {
                    list.AddRange(BridgeObject.GetTypesByDLL(str));
                }
            }
            list = list.Where(x => x.Name.EndsWith("OperateHandle") || x.Name.StartsWith("OperateHandleFactory") || (x.BaseType != null && x.BaseType.Name == "InitFactory")).ToList();
            if (list == null) list = new List<Type>();
            if (list.Count > 0 && cacheFactory != null)
            {
                cacheFactory.Set<List<Type>>(cache_operateHandleType, list);
            }
            return list;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// 方法调用类型
    /// </summary>
    public enum MethodCallTypeEnum
    {
        /// <summary>
        /// 通用方法
        /// </summary>
        [Description("通用方法")]
        CommonMethod = 0,

        /// <summary>
        /// 重写通用方法
        /// </summary>
        [Description("重写通用方法")]
        OverrideMethod = 1,

        /// <summary>
        /// 自定义方法
        /// </summary>
        [Description("自定义方法")]
        CustomerMethod = 2
    }
}

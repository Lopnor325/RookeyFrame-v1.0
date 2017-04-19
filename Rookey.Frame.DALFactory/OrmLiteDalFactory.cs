/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Rookey.Frame.Common;
using Rookey.Frame.Common.Model;
using Rookey.Frame.Common.PubDefine;
using ServiceStack.OrmLite;
using System.Collections.Concurrent;

namespace Rookey.Frame.DALFactory
{
    /// <summary>
    /// OrmLite映射工厂类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class OrmLiteDalFactory<T> : DalAbstractFactory<T> where T : class
    {
        #region 私有成员

        /// <summary>
        /// orm Provider
        /// </summary>
        private IOrmLiteDialectProvider _dialectProvider = null;
        /// <summary>
        /// 默认排序字段
        /// </summary>
        private string defaultSortField = "AutoIncrmId";

        /// <summary>
        /// 模块写连接字符串，线程安全
        /// </summary>
        private ConcurrentDictionary<Type, string> modelConnStringWriteDic = new ConcurrentDictionary<Type, string>();
        /// <summary>
        /// 模块读连接字符串，线程安全
        /// </summary>
        private ConcurrentDictionary<Type, string> modelConnStringReadDic = new ConcurrentDictionary<Type, string>();

        #endregion

        #region 构造函数

        public OrmLiteDalFactory(IOrmLiteDialectProvider DialectProvider)
        {
            _dialectProvider = DialectProvider;
            OrmLiteConfig.CommandTimeout = 30;
            OrmLiteConfig.TSCommandTimeout = 60;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取模块配置的连接字符串
        /// </summary>
        /// <param name="read">是否读</param>
        /// <param name="modelType">指定实体类型</param>
        /// <returns></returns>
        private string GetModelConfigConnString(bool read = true, Type modelType = null)
        {
            string dbType = string.Empty;
            Type type = modelType != null ? modelType : typeof(T);
            string tempConnStr = ModelConfigHelper.GetModelConnString(type, out dbType, read);
            return tempConnStr;
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="read">是否读</param>
        /// <param name="modelType">指定实体类型</param>
        /// <returns></returns>
        private string GetConnString(string connString, bool read = true, Type modelType = null)
        {
            Type type = modelType != null ? modelType : typeof(T);
            if (type.Name != "BaseEntity")
            {
                if (read) //读操作
                {
                    if (modelConnStringReadDic.ContainsKey(type)) //从读缓存中取
                    {
                        string tempStr = string.Empty;
                        modelConnStringReadDic.TryGetValue(type, out tempStr);
                        if (!string.IsNullOrEmpty(tempStr))
                            return tempStr;
                    }
                }
                else //写操作
                {
                    if (modelConnStringWriteDic.ContainsKey(type)) //从写缓存中取
                    {
                        string tempStr = string.Empty;
                        modelConnStringWriteDic.TryGetValue(type, out tempStr);
                        if (!string.IsNullOrEmpty(tempStr))
                            return tempStr;
                    }
                }
            }
            string lastConnStr = !string.IsNullOrEmpty(connString) ? connString : (read ? this.ReadConnectionString : this.WriteConnectionString);
            string modelConfigConnString = GetModelConfigConnString(read, modelType); //取模块数据库连接配置
            if (string.IsNullOrEmpty(connString) && !string.IsNullOrEmpty(modelConfigConnString))
            {
                lastConnStr = modelConfigConnString;
            }
            NotNullCheck.NotEmpty(lastConnStr, "数据库连接字符串");
            if (type.Name != "BaseEntity" && !string.IsNullOrEmpty(lastConnStr)) //将连接字符串缓存起来
            {
                if (read) //读操作
                    modelConnStringReadDic.TryAdd(type, lastConnStr);
                else //写操作
                    modelConnStringWriteDic.TryAdd(type, lastConnStr);
            }
            return lastConnStr;
        }

        #endregion

        #region 查询实体

        /// <summary>
        /// 获取所有实体集合
        /// </summary>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override List<T> GetAllEntities(bool references = false, string connString = null)
        {
            string connStr = GetConnString(connString);
            OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
            using (var conn = factory.OpenDbConnection())
            {
                try
                {
                    SqlExpression<T> exp = conn.From<T>().ThenByDescending(defaultSortField);
                    if (references)
                    {
                        exp = exp.Limit(0, 2000);
                    }
                    List<T> list = references ? conn.LoadSelect<T>(exp) : conn.Select<T>(exp);
                    if (list == null) list = new List<T>();
                    return list;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="where">条件语句</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override T GetEntity(out string errorMsg, Expression<Func<T, bool>> expression = null, string where = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    SqlExpression<T> exp = conn.From<T>();
                    if (!string.IsNullOrEmpty(where))
                    {
                        exp = exp.Where(where, null);
                    }
                    if (expression != null)
                    {
                        exp = exp.Where(expression).ThenByDescending(defaultSortField);
                    }
                    if (references)
                    {
                        exp = exp.Limit(0, 1);
                    }
                    try
                    {
                        return references ? conn.LoadSelect<T>(exp, fieldNames).FirstOrDefault() : conn.Single<T>(exp, fieldNames);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="id">记录ID</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override T GetEntityById(out string errorMsg, object id, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        return references ? conn.LoadSingleById<T>(id, fieldNames) : conn.SingleById<T>(id, fieldNames);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="where">条件语句</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">排序方式</param>
        /// <param name="top">取前几条</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override List<T> GetEntities(out string errorMsg, Expression<Func<T, bool>> expression = null, string where = null, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    SqlExpression<T> exp = conn.From<T>();
                    if (expression != null)
                        exp = exp.Where(expression);
                    if (!string.IsNullOrEmpty(where))
                        exp = exp.Where(where, null);
                    if (orderFields != null && orderFields.Count > 0)
                    {
                        for (int i = 0; i < orderFields.Count; i++)
                        {
                            string orderField = string.IsNullOrEmpty(orderFields[i]) ? defaultSortField : orderFields[i];
                            bool isdesc = isDescs != null && orderFields.Count == isDescs.Count ? isDescs[i] : true;
                            exp = isdesc ? exp.ThenByDescending(orderField) : exp.ThenBy(orderField);
                        }
                    }
                    int maxRows = 2000;
                    if (top.HasValue && top.Value > 0)
                    {
                        maxRows = top.Value;
                    }
                    exp = exp.Limit(0, maxRows);
                    try
                    {
                        List<T> list = references ? conn.LoadSelect<T>(exp, fieldNames) : conn.Select<T>(exp, fieldNames);
                        string sql = conn.GetLastSql();
                        return list;
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
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
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValue">字段值</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="where">条件语句</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">排序方式</param>
        /// <param name="top">取前几条</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override List<T> GetEntitiesByField(out string errorMsg, string fieldName, object fieldValue, Expression<Func<T, bool>> expression = null, string where = null, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    string tempWhere = fieldValue == null ? string.Format("{0} IS {1}", fieldName, "{0}") : string.Format("{0}={1}", fieldName, "{0}");
                    if (!string.IsNullOrEmpty(where)) tempWhere += " AND " + where;
                    SqlExpression<T> exp = conn.From<T>().Where(tempWhere, fieldValue);
                    if (expression != null) exp = exp.Where(expression);
                    if (orderFields != null && orderFields.Count > 0)
                    {
                        for (int i = 0; i < orderFields.Count; i++)
                        {
                            string orderField = string.IsNullOrEmpty(orderFields[i]) ? defaultSortField : orderFields[i];
                            bool isdesc = isDescs != null && orderFields.Count == isDescs.Count ? isDescs[i] : true;
                            exp = isdesc ? exp.ThenByDescending(orderField) : exp.ThenBy(orderField);
                        }
                    }
                    int maxRows = 2000;
                    if (top.HasValue && top.Value > 0)
                    {
                        maxRows = top.Value;
                    }
                    exp = exp.Limit(0, maxRows);
                    try
                    {
                        return references ? conn.LoadSelect<T>(exp, fieldNames) : conn.Select<T>(exp, fieldNames);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return new List<T>();
        }

        /// <summary>
        /// 分页获取实体集合
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">排序方式</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="where">条件语句</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override List<T> GetPageEntities(out long totalCount, out string errorMsg, int pageIndex = 1, int pageSize = 10, List<string> orderFields = null, List<bool> isDescs = null, Expression<Func<T, bool>> expression = null, string where = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            totalCount = 0;
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                //页序号
                int index = pageIndex < 1 ? 0 : (pageIndex - 1);
                //每页记录数
                int rows = pageSize < 1 ? 10 : (pageSize > 2000 ? 2000 : pageSize);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    SqlExpression<T> exp = conn.From<T>();
                    if (expression != null)
                    {
                        exp = exp.Where(expression);
                    }
                    if (!string.IsNullOrEmpty(where))
                    {
                        exp = exp.Where(where, null);
                    }
                    if (orderFields != null && orderFields.Count > 0)
                    {
                        for (int i = 0; i < orderFields.Count; i++)
                        {
                            string orderField = string.IsNullOrEmpty(orderFields[i]) ? defaultSortField : orderFields[i];
                            bool isdesc = isDescs != null && orderFields.Count == isDescs.Count ? isDescs[i] : true;
                            exp = isdesc ? exp.ThenByDescending(orderField) : exp.ThenBy(orderField);
                        }
                    }
                    else
                    {
                        exp = exp.ThenByDescending(defaultSortField);
                    }
                    exp = exp.Limit(index * rows, rows);
                    try
                    {
                        List<T> list = references ? conn.LoadSelect<T>(exp, fieldNames) : conn.Select(exp, fieldNames);
                        string sql = conn.GetLastSql();
                        totalCount = conn.Count<T>(exp);
                        return list;
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
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
        /// <param name="expression">条件表达式</param>
        /// <param name="where">条件语句</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override long Count(out string errorMsg, Expression<Func<T, bool>> expression = null, string where = null, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    SqlExpression<T> exp = conn.From<T>();
                    if (expression != null)
                    {
                        exp = exp.Where(expression);
                    }
                    if (!string.IsNullOrEmpty(where))
                    {
                        exp = exp.Where(where, null);
                    }
                    try
                    {
                        return conn.Count<T>(exp);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// 加载导航属性
        /// </summary>
        /// <param name="instance">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public override void LoadReferences(T instance, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (instance == null) return;
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        conn.LoadReferences<T>(instance);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        /// <summary>
        /// 加载导航属性
        /// </summary>
        /// <param name="instances">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public override void LoadListReferences(List<T> instances, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (instances == null || instances.Count == 0) return;
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        foreach (T t in instances)
                        {
                            conn.LoadReferences<T>(t);
                        }
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        /// <summary>
        /// 加载字段值
        /// </summary>
        /// <param name="field">字段表达式</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override object Scalar(Expression<Func<T, object>> field, Expression<Func<T, bool>> expression, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                if (field == null) return null;
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        return expression == null ? conn.Scalar<T, object>(field) : conn.Scalar<T, object>(field, expression);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        #endregion

        #region 新增实体

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override Guid AddEntity(T entity, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                Guid id = Guid.Empty;
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    bool rs = references ? conn.Save<T>(entity, true) : conn.Insert<T>(entity) > 0;
                    PropertyInfo p = typeof(T).GetProperty("Id");
                    id = p.GetValue2(entity, null).ObjToGuid();
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
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
        /// 新增实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool AddEntities(List<T> entities, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                bool rs = false;
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    if (references)
                    {
                        if (transConn == null) //外界未启用事务
                        {
                            using (var dbTrans = conn.OpenTransaction())
                            {
                                foreach (T t in entities)
                                {
                                    conn.Save<T>(t, true);
                                }
                                dbTrans.Commit();
                                rs = true;
                            }
                        }
                        else //外界已启用事务
                        {
                            foreach (T t in entities)
                            {
                                rs = conn.Save<T>(t, true);
                                if (!rs) break;
                            }
                        }
                    }
                    else
                    {
                        if (transConn == null) //外界未启用事务
                        {
                            conn.InsertAll<T>(entities);
                            rs = true;
                        }
                        else //外界已启用事务
                        {
                            foreach (T t in entities)
                            {
                                rs = conn.Insert<T>(t) > 0;
                                if (!rs) break;
                            }
                        }
                    }
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
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
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool UpdateEntity(T entity, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                int rs = 0;
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    if (references)
                    {
                        bool result = conn.Save<T>(entity, true);
                        rs = result ? 1 : 0;
                    }
                    else
                    {
                        rs = conn.Update<T>(entity);
                    }
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                return rs > 0;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 更新实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool UpdateEntities(List<T> entities, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                bool rs = false;
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    if (references)
                    {
                        if (transConn == null) //外界未启用事务
                        {
                            using (var dbTrans = conn.OpenTransaction())
                            {
                                foreach (T t in entities)
                                {
                                    conn.Save<T>(t, true);
                                }
                                dbTrans.Commit();
                                rs = true;
                            }
                        }
                        else //外界已启用事务
                        {
                            foreach (T t in entities)
                            {
                                rs = conn.Save<T>(t, true);
                                if (!rs) break;
                            }
                        }
                    }
                    else
                    {
                        if (transConn == null) //外界未启用事务
                        {
                            rs = conn.UpdateAll<T>(entities) > 0;
                        }
                        else //外界已启用事务
                        {
                            foreach (T t in entities)
                            {
                                rs = conn.Update<T>(t) > 0;
                                if (!rs) break;
                            }
                        }
                    }
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
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
        /// <param name="fieldNames">字段名集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool UpdateEntityFields(T entity, List<string> fieldNames, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                int rs = 0;
                PropertyInfo pId = typeof(T).GetProperty("Id");
                object id = pId.GetValue2(entity, null);
                if (id.ObjToGuid() == Guid.Empty)
                {
                    errorMsg = "实体Id不存在，无法更新，请检查实体对象！";
                    return false;
                }
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    SqlExpression<T> sqlExpression = conn.From<T>().Where("Id={0}", new object[] { id }).Update(fieldNames);
                    rs = conn.UpdateOnly<T>(entity, sqlExpression);
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                return rs > 0;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 更新记录值
        /// </summary>
        /// <param name="obj">要更新的匿名对象</param>
        /// <param name="expression">要更新的记录的条件表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool UpdateEntityByExpression(object obj, Expression<Func<T, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                int rs = 0;
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    rs = conn.Update<T>(obj, expression);
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                return rs > 0;
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
        /// <param name="expression">条件表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool DeleteEntity(Expression<Func<T, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                int rs = 0;
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    rs = conn.Delete<T>(expression);
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                return rs > 0;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool DeleteEntityById(object id, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                int rs = 0;
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    rs = conn.DeleteById<T>(id);
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                return rs > 0;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="ids">记录ID集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override bool DeleteEntityByIds(System.Collections.IEnumerable ids, out string errorMsg, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                int rs = 0;
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    rs = conn.DeleteByIds<T>(ids);
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                return rs > 0;
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
        /// 获取实体集合
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="sql">查询语句</param>
        /// <param name="sqlParams">查询参数</param>
        /// <param name="connString">连接字符串</param>
        /// <returns>返回实体集合</returns>
        public override List<T> GetEntitiesBySql(out string errorMsg, string sql, object[] sqlParams = null, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        return conn.SelectFmt<T>(sql, sqlParams);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 以SQL方式执行查询
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override DataTable ExecuteQuery(out string errorMsg, string sql, Dictionary<string, object> sqlParams = null, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        return conn.ExecuteQuery(sql, sqlParams);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 获取查询到的第一行第一列的数据
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override object ExecuteScale(out string errorMsg, string sql, object[] sqlParams = null, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        return conn.ScalarFmt<object>(sql, sqlParams);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 执行增删改语句
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="sql">sql</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public override int ExecuteNonQuery(out string errorMsg, string sql, Dictionary<string, object> sqlParams = null, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    return conn.ExecuteNonQuery(sql, sqlParams);
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return 0;
        }

        /// <summary>
        /// 执行存储过程，针对非查询
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="outParams">输出参数</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="inParams">输入参数 eg:new{Age=30}</param>
        /// <param name="connString">连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回受影响的行数</returns>
        public override int RunProcedureNoQuery(out string errorMsg, ref Dictionary<string, object> outParams, string procedureName, object inParams = null, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    IDbCommand cmd = conn.SqlProc(procedureName, inParams);
                    Dictionary<string, IDataParameter> dic = new Dictionary<string, IDataParameter>();
                    if (outParams != null && outParams.Count > 0)
                    {
                        foreach (string key in outParams.Keys)
                        {
                            if (outParams[key].GetType() == typeof(IDbDataParameter))
                            {
                                dic.Add(key, outParams[key] as IDbDataParameter);
                            }
                            else
                            {
                                IDataParameter v = cmd.AddParam(key, outParams[key], ParameterDirection.Output);
                                dic.Add(key, v);
                            }
                        }
                    }
                    int rs = cmd.ExecuteNonQuery();
                    foreach (string key in dic.Keys)
                    {
                        outParams[key] = dic[key].Value;
                    }
                    return rs;
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return 0;
        }

        /// <summary>
        /// 执行存储过程，针对查询
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="outParams">输出参数</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="inParams">输入参数 eg:new{Age=30}</param>
        /// <param name="connString">连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回查询记录</returns>
        public override DataTable RunProcedure(out string errorMsg, ref Dictionary<string, object> outParams, string procedureName, object inParams = null, string connString = null, IDbConnection transConn = null)
        {
            errorMsg = string.Empty;
            try
            {
                var conn = transConn == null ? new OrmLiteConnectionFactory(GetConnString(connString, false), _dialectProvider).OpenDbConnection() : transConn;
                try
                {
                    IDbCommand cmd = conn.SqlProc(procedureName, inParams);
                    Dictionary<string, IDataParameter> dic = new Dictionary<string, IDataParameter>();
                    if (outParams != null && outParams.Count > 0)
                    {
                        foreach (string key in outParams.Keys)
                        {
                            if (outParams[key].GetType() == typeof(IDbDataParameter))
                            {
                                dic.Add(key, outParams[key] as IDbDataParameter);
                            }
                            else
                            {
                                IDataParameter v = cmd.AddParam(key, outParams[key], ParameterDirection.Output);
                                dic.Add(key, v);
                            }
                        }
                    }
                    DataTable dt = cmd.ConvertToDt();
                    foreach (string key in dic.Keys)
                    {
                        outParams[key] = dic[key].Value;
                    }
                    return dt;
                }
                finally
                {
                    if (transConn == null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return null;
        }

        #endregion

        #region 事务操作

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transactionObjects">事务对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public override void ExecuteTransaction(List<TransactionModel<T>> transactionObjects, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            if (transactionObjects == null || transactionObjects.Count == 0)
                return;
            try
            {
                string connStr = GetConnString(connString, false, typeof(T));
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        using (var dbTrans = conn.OpenTransaction())
                        {
                            try
                            {
                                PropertyInfo pId = typeof(T).GetProperty("Id");
                                foreach (TransactionModel<T> obj in transactionObjects)
                                {
                                    switch (obj.OperateType)
                                    {
                                        case DataOperateType.Add:
                                            foreach (T t in obj.Models)
                                            {
                                                object id = pId.GetValue2(t, null);
                                                if (id.ObjToGuid() == Guid.Empty)
                                                    pId.SetValue2(t, Guid.NewGuid(), null);
                                                if (obj.References)
                                                    conn.Save<T>(t, true);
                                                else
                                                    conn.Insert<T>(t);
                                            }
                                            break;
                                        case DataOperateType.Edit:
                                            foreach (T t in obj.Models)
                                            {
                                                if (obj.References)
                                                    conn.Save<T>(t, true);
                                                else
                                                    conn.Update<T>(t);
                                            }
                                            break;
                                        case DataOperateType.Del:
                                            List<Guid> listId = new List<Guid>();
                                            foreach (T t in obj.Models)
                                            {
                                                Guid id = pId.GetValue2(t, null).ObjToGuid();
                                                listId.Add(id);
                                            }
                                            conn.DeleteByIds<T>(listId);
                                            break;
                                    }
                                }
                                dbTrans.Commit();
                            }
                            catch (Exception exx)
                            {
                                dbTrans.Rollback();
                                errorMsg = exx.Message;
                            }
                        }
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transactionObjects">事务扩展对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public override void ExecuteTransactionExtend(List<TransactionExtendModel> transactionObjects, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            if (transactionObjects == null || transactionObjects.Count == 0)
                return;
            try
            {
                if (transactionObjects.FirstOrDefault().Models == null || transactionObjects.FirstOrDefault().Models.Cast<T>().Count() == 0)
                    return;
                Type modelType = transactionObjects.FirstOrDefault().Models.Cast<T>().FirstOrDefault().GetType();
                string connStr = GetConnString(connString, false, modelType);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        using (var dbTrans = conn.OpenTransaction())
                        {
                            try
                            {
                                PropertyInfo pId = typeof(T).GetProperty("Id");
                                foreach (TransactionExtendModel obj in transactionObjects)
                                {
                                    IUntypedApi untypeApi = UntypedApiExtensions.CreateTypedApi(conn, obj.ModelType);
                                    switch (obj.OperateType)
                                    {
                                        case DataOperateType.Add:
                                            foreach (T t in obj.Models)
                                            {
                                                object id = pId.GetValue2(t, null);
                                                if (id.ObjToGuid() == Guid.Empty)
                                                    pId.SetValue2(t, Guid.NewGuid(), null);
                                                untypeApi.Insert(t);
                                            }
                                            break;
                                        case DataOperateType.Edit:
                                            foreach (T t in obj.Models)
                                            {
                                                untypeApi.Update(t);
                                            }
                                            break;
                                        case DataOperateType.Del:
                                            List<Guid> listId = new List<Guid>();
                                            foreach (T t in obj.Models)
                                            {
                                                Guid id = pId.GetValue2(t, null).ObjToGuid();
                                                listId.Add(id);
                                            }
                                            untypeApi.DeleteByIds(listId);
                                            break;
                                    }
                                }
                                dbTrans.Commit();
                            }
                            catch (Exception exx)
                            {
                                dbTrans.Rollback();
                                errorMsg = exx.Message;
                            }
                        }
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transTask">事务处理函数</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public override void TransactionHandle(TransactionTask transTask, out string errorMsg, string connString = null)
        {
            errorMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString, false);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        using (var dbTrans = conn.OpenTransaction())
                        {
                            try
                            {
                                transTask.Invoke(conn);
                                dbTrans.Commit();
                            }
                            catch (Exception exx)
                            {
                                dbTrans.Rollback();
                                errorMsg = exx.Message;
                            }
                        }
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }
        
        #endregion

        #region 数据库操作

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>创建成功返回空字符串，失败返回异常信息</returns>
        public override string CreateTable(string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        if (conn.TableExists<T>())
                            return string.Empty;
                        else
                            conn.CreateTable<T>();
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>删除成功返回空字符串，失败返回异常信息</returns>
        public override string DropTable(string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        if (!conn.TableExists<T>())
                            errMsg = "数据表不存在，无需删除！";
                        else
                            conn.DropTable<T>();
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        /// <summary>
        /// 修改数据表
        /// </summary>
        /// <param name="command">操作sql，如：[ALTER TABLE a] ADD Column b int</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public override string AlterTable(string command, string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        conn.AlterTable<T>(command);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        /// <summary>
        /// 数据列是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override bool ColumnIsExists(string fieldName, string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        return conn.ColumnIsExists(typeof(T), fieldName);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 增加数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public override string AddColumn(string fieldName, string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        Type modelType = typeof(T);
                        ModelDefinition modelDef = modelType.GetModelMetadata();
                        FieldDefinition fieldDef = modelDef.FieldDefinitions.Where(x => x.FieldName == fieldName).FirstOrDefault();
                        conn.AddColumn(modelType, fieldDef);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        /// <summary>
        /// 修改数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public override string AlterColumn(string fieldName, string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        Type modelType = typeof(T);
                        ModelDefinition modelDef = modelType.GetModelMetadata();
                        FieldDefinition fieldDef = modelDef.FieldDefinitions.Where(x => x.FieldName == fieldName).FirstOrDefault();
                        conn.AlterColumn(modelType, fieldDef);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        /// <summary>
        /// 修改列名
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="oldFieldName">旧的字段名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public override string ChangeColumnName(string fieldName, string oldFieldName, string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        Type modelType = typeof(T);
                        ModelDefinition modelDef = modelType.GetModelMetadata();
                        FieldDefinition fieldDef = modelDef.FieldDefinitions.Where(x => x.FieldName == fieldName).FirstOrDefault();
                        conn.ChangeColumnName(modelType, fieldDef, oldFieldName);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override string DropColumn(string columnName, string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        conn.DropColumn<T>(columnName);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="indexName">索引名</param>
        /// <param name="unique">是否唯一索引</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public override string CreateIndex(Expression<Func<T, object>> field, string indexName = null, bool unique = false, string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        conn.CreateIndex<T>(field, indexName, unique);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public override string DropIndex(string indexName, string connString = null)
        {
            string errMsg = string.Empty;
            try
            {
                string connStr = GetConnString(connString);
                OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connStr, _dialectProvider);
                using (var conn = factory.OpenDbConnection())
                {
                    try
                    {
                        conn.DropIndex<T>(indexName);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return errMsg;
        }

        #endregion

        #region 其他

        /// <summary>
        /// 将lamda条件表达式转换成sql
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public override string ExpressionConditionToWhereSql(Expression<Func<T, bool>> expression)
        {
            if (expression == null) return string.Empty;
            SqlExpression<T> sqlExp = this._dialectProvider.SqlExpression<T>();
            sqlExp = sqlExp.Where<T>(expression);
            string whereSql = sqlExp.WhereExpression.Replace("WHERE", string.Empty);
            return whereSql;
        }

        #endregion
    }
}

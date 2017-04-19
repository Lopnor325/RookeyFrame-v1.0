/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Collections;
using System.Data;
using Rookey.Frame.IBLL.Base;
using Rookey.Frame.IDAL.Base;
using Rookey.Frame.DAL.Base;
using Rookey.Frame.Common.PubDefine;
using Rookey.Frame.Common.Model;
using Rookey.Frame.EntityBase;
using Rookey.Frame.Base;

namespace Rookey.Frame.BLL.Base
{
    /// <summary>
    /// 通用业务层
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseBLL<T> : IBaseBLL<T> where T : BaseEntity, new()
    {
        #region 成员属性

        /// <summary>
        /// ORM数据库连接对象
        /// </summary>
        private IBaseDAL<T> _dal = null;

        /// <summary>
        /// 数据层对象
        /// </summary>
        public IBaseDAL<T> Dal
        {
            get { return _dal; }
        }

        private UserInfo currUser = null;
        /// <summary>
        /// 当前用户
        /// </summary>
        public UserInfo CurrUser
        {
            get { return currUser; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造DAL对象
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="dbType">数据库类型</param>
        private void BuildDal(UserInfo userInfo, DatabaseType? dbType)
        {
            //先判断有没有自定义重写数据层
            try
            {
                Type customerDalType = Bridge.BridgeObject.GetDALType(typeof(T));
                if (customerDalType != null) //有自定义重写类，取自定义数据层对象
                {
                    object obj = Activator.CreateInstance(customerDalType, new object[] { userInfo });
                    if (obj != null)
                        _dal = obj as BaseDAL<T>;
                    else
                        _dal = new BaseDAL<T>(userInfo, dbType);
                }
                else
                {
                    _dal = new BaseDAL<T>(userInfo, dbType);
                }
            }
            catch
            {
                _dal = new BaseDAL<T>(userInfo, dbType);
            }
        }

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public BaseBLL()
        {
            BuildDal(null, null);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        public BaseBLL(DatabaseType? dbType = null)
        {
            BuildDal(null, dbType);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">当前用户</param>
        /// <param name="dbType">数据库类型</param>
        public BaseBLL(UserInfo userInfo, DatabaseType? dbType = null)
        {
            currUser = userInfo;
            BuildDal(userInfo, dbType);
        }

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
            return _dal.GetEntity(out errorMsg, expression, whereSql, fieldNames, references, connString);
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
            return _dal.GetEntityById(out errorMsg, id, fieldNames, references, connString);
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="whereSql">SQL条件语句</param>
        /// <param name="permissionFilter">是否启用权限过滤</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">是否降序排序</param>
        /// <param name="top">取前几条记录</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public virtual List<T> GetEntities(out string errorMsg, Expression<Func<T, bool>> expression = null, string whereSql = null, bool permissionFilter = true, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            return _dal.GetEntities(out errorMsg, expression, whereSql, permissionFilter, orderFields, isDescs, top, fieldNames, references, connString);
        }

        /// <summary>
        /// 根据字段获取记录
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段值</param>
        /// <param name="permissionFilter">是否启用权限过滤</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">是否降序</param>
        /// <param name="top">取前几条记录</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public virtual List<T> GetEntitiesByField(out string errorMsg, string fieldName, object fieldValue, bool permissionFilter = true, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null)
        {
            return _dal.GetEntitiesByField(out errorMsg, fieldName, fieldValue, permissionFilter, orderFields, isDescs, top, fieldNames, references, connString);
        }

        /// <summary>
        /// 获取分页实体集合
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionFilter">是否启用权限过滤</param>
        /// <param name="pageIndex">页号，最小为1，默认为1</param>
        /// <param name="pageSize">每页记录数，最大为500，默认为10</param>
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
            return _dal.GetPageEntities(out totalCount, out errorMsg, permissionFilter, pageIndex, pageSize, orderFields, isDescs, expression, whereSql, fieldNames, references, connString);
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
            return _dal.Count(out errorMsg, permissionFilter, expression, whereSql, connString);
        }

        /// <summary>
        /// 加载实体对象的关联对象（导航属性）
        /// </summary>
        /// <param name="instance">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public virtual void LoadReferences(T instance, out string errorMsg, string connString = null)
        {
            _dal.LoadReferences(instance, out errorMsg, connString);
        }

        /// <summary>
        /// 加载关联对象（导航属性）
        /// </summary>
        /// <param name="instances">实体对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public virtual void LoadListReferences(List<T> instances, out string errorMsg, string connString = null)
        {
            _dal.LoadListReferences(instances, out errorMsg, connString);
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
            return _dal.Scalar(field, expression, out errorMsg, connString);
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
            return _dal.AddEntity(entity, out errorMsg, references, connString, transConn);
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
            return _dal.AddEntities(entities, out errorMsg, references, connString, transConn);
        }

        #endregion

        #region 更新实体

        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="fieldNames">要更新的字段名称集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <param name="references">是否保存关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns>返回更新结果</returns>
        public virtual bool UpdateEntity(T entity, out string errorMsg, List<string> fieldNames = null, bool permissionValidate = true, bool references = false, string connString = null, IDbConnection transConn = null)
        {
            if (fieldNames != null && fieldNames.Count > 0)
            {
                return _dal.UpdateEntityFields(entity, fieldNames, out errorMsg, permissionValidate, connString, transConn);
            }
            else
            {
                return _dal.UpdateEntity(entity, out errorMsg, permissionValidate, references, connString, transConn);
            }
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
            return _dal.UpdateEntities(entities, out errorMsg, permissionValidate, references, connString, transConn);
        }

        /// <summary>
        /// 批量更新字段
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="fieldNames">字段集合</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public virtual bool UpdateFields(List<T> entities, out string errMsg, List<string> fieldNames, string connString = null, IDbConnection transConn = null)
        {
            if (entities == null || entities.Count == 0)
            {
                errMsg = "实体集合为空";
                return false;
            }
            if (fieldNames == null || fieldNames.Count == 0)
            {
                errMsg = "没有可更新字段";
                return false;
            }
            StringBuilder msg = new StringBuilder();
            foreach (T t in entities)
            {
                string tempMsg = string.Empty;
                bool rs = UpdateEntity(t, out tempMsg, fieldNames, false, false, connString, transConn);
                if (!string.IsNullOrEmpty(tempMsg))
                {
                    msg.Append(tempMsg);
                }
            }
            errMsg = msg.ToString();
            return string.IsNullOrEmpty(errMsg);
        }

        /// <summary>
        /// 通过表达式更新实体
        ///  UpdateEntityByExpression(new { FirstName = "JJ" }, p => p.LastName == "Hendrix");
        ///  UPDATE "Person" SET "FirstName" = 'JJ' WHERE ("LastName" = 'Hendrix')
        /// </summary>
        /// <param name="obj">匿名对象</param>
        /// <param name="expression">表达式</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public virtual bool UpdateEntityByExpression(object obj, Expression<Func<T, bool>> expression, out string errMsg, string connString = null, IDbConnection transConn = null)
        {
            return _dal.UpdateEntityByExpression(obj, expression, out errMsg, connString, transConn);
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
            return _dal.DeleteEntity(expression, out errorMsg, connString, transConn);
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
            return _dal.DeleteEntityById(id, out errorMsg, permissionValidate, connString, transConn);
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
            return _dal.DeleteEntityByIds(ids, out errorMsg, permissionValidate, connString, transConn);
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
            return _dal.GetEntitiesBySql(out errMsg, sql, sqlParams, connString);
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
            return _dal.ExecuteQuery(out errMsg, sql, sqlParams, connString);
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
            return _dal.ExecuteScale(out errMsg, sql, sqlParams, connString);
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
            return _dal.ExecuteNonQuery(out errMsg, sql, sqlParams, connString, transConn);
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
            return _dal.RunProcedureNoQuery(out errMsg, ref outParams, procedureName, inParams, connString, transConn);
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
            return _dal.RunProcedure(out errMsg, ref outParams, procedureName, inParams, connString, transConn);
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
            _dal.ExecuteTransaction(transactionObjects, out errorMsg, connString);
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
            _dal.ExecuteTransactionExtend(transactionObjects, out errorMsg, connString);
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
            _dal.TransactionHandle(transTask, out errorMsg, connString);
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
            return _dal.CreateTable(connString);
        }

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>删除成功返回空字符串，失败返回异常信息</returns>
        public string DropTable(string connString = null)
        {
            return _dal.DropTable(connString);
        }

        /// <summary>
        /// 修改数据表
        /// </summary>
        /// <param name="command">操作sql，如：[ALTER TABLE a] ADD Column b int</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public string AlterTable(string command, string connString = null)
        {
            return _dal.AlterTable(command, connString);
        }

        /// <summary>
        /// 数据列是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public bool ColumnIsExists(string fieldName, string connString = null)
        {
            return _dal.ColumnIsExists(fieldName, connString);
        }

        /// <summary>
        /// 增加数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public string AddColumn(string fieldName, string connString = null)
        {
            return _dal.AddColumn(fieldName, connString);
        }

        /// <summary>
        /// 修改数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public string AlterColumn(string fieldName, string connString = null)
        {
            return _dal.AlterColumn(fieldName, connString);
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
            return _dal.ChangeColumnName(fieldName, oldFieldName, connString);
        }

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public string DropColumn(string columnName, string connString = null)
        {
            return _dal.DropColumn(columnName, connString);
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
            return _dal.CreateIndex(field, indexName, unique, connString);
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public string DropIndex(string indexName, string connString = null)
        {
            return _dal.DropIndex(indexName, connString);
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
            return _dal.ExpressionConditionToWhereSql(expression);
        }

        /// <summary>
        /// 清除当前模块缓存
        /// </summary>
        public void ClearCache()
        {
            _dal.ClearCache();
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void ClearAllCache()
        {
            _dal.ClearAllCache();
        }

        /// <summary>
        /// 设置自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="data">数据</param>
        public void SetCustomerCache(string key, object data)
        {
            _dal.SetCustomerCache(key, data);
        }

        /// <summary>
        /// 移除自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        public void RemoveCustomerCache(string key)
        {
            _dal.RemoveCustomerCache(key);
        }

        /// <summary>
        /// 获取自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        public object GetCustomerCache(string key)
        {
            return _dal.GetCustomerCache(key);
        }

        #endregion
    }
}

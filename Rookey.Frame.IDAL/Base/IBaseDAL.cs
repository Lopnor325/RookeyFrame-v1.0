/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Rookey.Frame.IDAL.Base
{
    /// <summary>
    /// 数据层基接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseDAL<T> where T : class
    {
        #region 查询实体

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">查询表达式</param>
        /// <param name="whereSql">SQL条件语句</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        T GetEntity(out string errorMsg, Expression<Func<T, bool>> expression = null, string whereSql = null, List<string> fieldNames = null, bool references = false, string connString = null);

        /// <summary>
        /// 通过主键Id获取实体
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="id">主键Id</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        T GetEntityById(out string errorMsg, object id, List<string> fieldNames = null, bool references = false, string connString = null);

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
        List<T> GetEntities(out string errorMsg, Expression<Func<T, bool>> expression = null, string whereSql = null, bool permissionFilter = true, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null);

        /// <summary>
        /// 根据字段获取记录
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段值</param>
        /// <param name="permissionFilter">是否进行权限过滤</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">是否降序</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        List<T> GetEntitiesByField(out string errorMsg, string fieldName, object fieldValue, bool permissionFilter = true, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null);

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
        List<T> GetPageEntities(out long totalCount, out string errorMsg, bool permissionFilter = true, int pageIndex = 1, int pageSize = 10, List<string> orderFields = null, List<bool> isDescs = null, Expression<Func<T, bool>> expression = null, string whereSql = null, List<string> fieldNames = null, bool references = false, string connString = null);

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionFilter">是否进行权限过滤</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="whereSql">SQL条件语句</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        long Count(out string errorMsg, bool permissionFilter = true, Expression<Func<T, bool>> expression = null, string whereSql = null, string connString = null);

        /// <summary>
        /// 加载实体对象的关联对象（导航属性）
        /// </summary>
        /// <param name="instance">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        void LoadReferences(T instance, out string errorMsg, string connString = null);

        /// <summary>
        /// 加载关联对象（导航属性）
        /// </summary>
        /// <param name="instances">实体对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        void LoadListReferences(List<T> instances, out string errorMsg, string connString = null);

        /// <summary>
        /// 获取实体某个字段值
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        object Scalar(Expression<Func<T, object>> field, Expression<Func<T, bool>> expression, out string errorMsg, string connString = null);

        #endregion

        #region 新增实体

        /// <summary>
        /// 新增实体返回实体Id
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        Guid AddEntity(T entity, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存关联对象数据（导航属性）</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        bool AddEntities(List<T> entities, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null);

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
        bool UpdateEntity(T entity, out string errorMsg, bool permissionValidate = true, bool references = false, string connString = null, IDbConnection transConn = null);

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
        bool UpdateEntities(List<T> entities, out string errorMsg, bool permissionValidate = true, bool references = false, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 更新实体字段
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="fieldNames">要更新的字段</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        bool UpdateEntityFields(T entity, List<string> fieldNames, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null);

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
        bool UpdateEntityByExpression(object obj, Expression<Func<T, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null);

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
        bool DeleteEntity(Expression<Func<T, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 通过主键Id删除实体
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        bool DeleteEntityById(object id, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 通过主键Id集合删除实体
        /// </summary>
        /// <param name="ids">主键Id集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="permissionValidate">是否进行权限验证</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        bool DeleteEntityByIds(IEnumerable ids, out string errorMsg, bool permissionValidate = true, string connString = null, IDbConnection transConn = null);

        #endregion

        #region SQL方式

        /// <summary>
        /// 以SQL方式获取实体集合
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <returns></returns>
        List<T> GetEntitiesBySql(out string errMsg, string sql, object[] sqlParams = null, string connString = null);

        /// <summary>
        /// 以SQL方式执行查询
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <returns></returns>
        DataTable ExecuteQuery(out string errMsg, string sql, Dictionary<string, object> sqlParams = null, string connString = null);

        /// <summary>
        /// 获取查询到的第一行第一列的数据
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <returns></returns>
        object ExecuteScale(out string errMsg, string sql, object[] sqlParams = null, string connString = null);

        /// <summary>
        /// 执行增删改语句
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        /// <param name="sql">sql</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接对象</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        int ExecuteNonQuery(out string errMsg, string sql, Dictionary<string, object> sqlParams = null, string connString = null, IDbConnection transConn = null);

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
        int RunProcedureNoQuery(out string errMsg, ref Dictionary<string, object> outParams, string procedureName, object inParams = null, string connString = null, IDbConnection transConn = null);

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
        DataTable RunProcedure(out string errMsg, ref Dictionary<string, object> outParams, string procedureName, object inParams = null, string connString = null, IDbConnection transConn = null);

        #endregion

        #region 事务操作

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transactionObjects">事务对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        void ExecuteTransaction(List<TransactionModel<T>> transactionObjects, out string errorMsg, string connString = null);

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transactionObjects">事务扩展对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        void ExecuteTransactionExtend(List<TransactionExtendModel> transactionObjects, out string errorMsg, string connString = null);

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transTask">事务处理函数</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        void TransactionHandle(TransactionTask transTask, out string errorMsg, string connString = null);
        #endregion

        #region 数据库操作

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>创建成功返回空字符串，失败返回异常信息</returns>
        string CreateTable(string connString = null);

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>删除成功返回空字符串，失败返回异常信息</returns>
        string DropTable(string connString = null);

        /// <summary>
        /// 修改数据表
        /// </summary>
        /// <param name="command">操作sql，如：[ALTER TABLE a] ADD Column b int</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        string AlterTable(string command, string connString = null);

        /// <summary>
        /// 数据列是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        bool ColumnIsExists(string fieldName, string connString = null);

        /// <summary>
        /// 增加数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        string AddColumn(string fieldName, string connString = null);

        /// <summary>
        /// 修改数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        string AlterColumn(string fieldName, string connString = null);

        /// <summary>
        /// 修改列名
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="oldFieldName">旧的字段名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        string ChangeColumnName(string fieldName, string oldFieldName, string connString = null);

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        string DropColumn(string columnName, string connString = null);

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="indexName">索引名</param>
        /// <param name="unique">是否唯一索引</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        string CreateIndex(Expression<Func<T, object>> field, string indexName = null, bool unique = false, string connString = null);

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        string DropIndex(string indexName, string connString = null);

        #endregion

        #region 其他

        /// <summary>
        /// 将lamda条件表达式转换成sql
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        string ExpressionConditionToWhereSql(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 清除当前模块缓存
        /// </summary>
        void ClearCache();

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        void ClearAllCache();

        /// <summary>
        /// 设置自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        /// <param name="data">数据</param>
        void SetCustomerCache(string key, object data);

        /// <summary>
        /// 移除自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        void RemoveCustomerCache(string key);

        /// <summary>
        /// 获取自定义缓存
        /// </summary>
        /// <param name="key">缓存key</param>
        object GetCustomerCache(string key);

        #endregion
    }
}

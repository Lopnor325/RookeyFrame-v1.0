/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using Rookey.Frame.Common;
using System.Data;
using Rookey.Frame.Common.PubDefine;
using Rookey.Frame.Common.Model;

namespace Rookey.Frame.DALFactory
{
    /// <summary>
    /// 数据层抽象工厂
    /// </summary>
    public abstract class DalAbstractFactory<T> where T : class
    {
        #region 成员属性
        /// <summary>
        /// 读数据库连接串
        /// </summary>
        public string ReadConnectionString
        {
            get
            {
                string connSting = WebConfigHelper.GetConnectionString("DbReadConnString");
                if (!string.IsNullOrEmpty(connSting)) return connSting;
                string writeConnSting = WebConfigHelper.GetConnectionString("DbWriteConnString");
                return writeConnSting;
            }
        }

        /// <summary>
        /// 写数据库连接串
        /// </summary>
        public string WriteConnectionString
        {
            get
            {
                string connSting = WebConfigHelper.GetConnectionString("DbWriteConnString");
                if (!string.IsNullOrEmpty(connSting)) return connSting;
                string readConnStr = WebConfigHelper.GetConnectionString("DbReadConnString");
                return readConnStr;
            }
        }
        #endregion

        #region 实例化对象

        /// <summary>
        /// 实例化工厂
        /// </summary>
        /// <param name="factoryType">数据工厂类型</param>
        /// <returns></returns>
        public static DalAbstractFactory<T> GetInstance(DatabaseType factoryType)
        {
            IOrmLiteDialectProvider dialectProvider = SqlServerDialect.Provider;
            switch (factoryType)
            {
                case DatabaseType.MySql:
                    dialectProvider = MySqlDialect.Provider;
                    break;
                case DatabaseType.Oracle:
                    dialectProvider = OracleDialect.Provider;
                    break;
                default:
                    dialectProvider = SqlServerDialect.Provider;
                    break;
            }
            dialectProvider.DefaultStringLength = 500; //设置默认字符串字段长度
            return new OrmLiteDalFactory<T>(dialectProvider);
        }

        #endregion

        #region 公共方法
        #region 实体查询
        /// <summary>
        /// 获取所有实体集合
        /// </summary>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract List<T> GetAllEntities(bool references = false, string connString = null);

        /// <summary>
        /// 获取单个实体对象
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="where">条件语句</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract T GetEntity(out string errorMsg, Expression<Func<T, bool>> expression = null, string where = null, List<string> fieldNames = null, bool references = false, string connString = null);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="id">记录ID</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract T GetEntityById(out string errorMsg, object id, List<string> fieldNames = null, bool references = false, string connString = null);

        /// <summary>
        /// 获取实体对象集合
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="where">条件语句</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isDescs">是否降序</param>
        /// <param name="top">取前几条</param>
        /// <param name="fieldNames">要查询的字段集合</param>
        /// <param name="references">是否加载导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract List<T> GetEntities(out string errorMsg, Expression<Func<T, bool>> expression = null, string where = null, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null);

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
        public abstract List<T> GetEntitiesByField(out string errorMsg, string fieldName, object fieldValue, Expression<Func<T, bool>> expression = null, string where = null, List<string> orderFields = null, List<bool> isDescs = null, int? top = null, List<string> fieldNames = null, bool references = false, string connString = null);

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
        public abstract List<T> GetPageEntities(out long totalCount, out string errorMsg, int pageIndex = 1, int pageSize = 10, List<string> orderFields = null, List<bool> isDescs = null, Expression<Func<T, bool>> expression = null, string where = null, List<string> fieldNames = null, bool references = false, string connString = null);

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="where">条件语句</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract long Count(out string errorMsg, Expression<Func<T, bool>> expression = null, string where = null, string connString = null);

        /// <summary>
        /// 加载导航属性
        /// </summary>
        /// <param name="instance">实体对象</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public abstract void LoadReferences(T instance, out string errorMsg, string connString = null);

        /// <summary>
        /// 加载导航属性
        /// </summary>
        /// <param name="instances">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public abstract void LoadListReferences(List<T> instances, out string errorMsg, string connString = null);

        /// <summary>
        /// 加载字段值
        /// </summary>
        /// <param name="field">字段表达式</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract object Scalar(Expression<Func<T, object>> field, Expression<Func<T, bool>> expression, out string errorMsg, string connString = null);

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
        public abstract Guid AddEntity(T entity, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 新增实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public abstract bool AddEntities(List<T> entities, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null);

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
        public abstract bool UpdateEntity(T entity, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 更新实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="references">是否保存导航属性</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public abstract bool UpdateEntities(List<T> entities, out string errorMsg, bool references = false, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 更新实体字段
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="fieldNames">字段名集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public abstract bool UpdateEntityFields(T entity, List<string> fieldNames, out string errorMsg, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 更新记录值
        /// </summary>
        /// <param name="obj">要更新的匿名对象</param>
        /// <param name="expression">要更新的记录的条件表达式</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public abstract bool UpdateEntityByExpression(object obj, Expression<Func<T, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null);

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
        public abstract bool DeleteEntity(System.Linq.Expressions.Expression<Func<T, bool>> expression, out string errorMsg, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public abstract bool DeleteEntityById(object id, out string errorMsg, string connString = null, IDbConnection transConn = null);

        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="ids">记录ID集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public abstract bool DeleteEntityByIds(System.Collections.IEnumerable ids, out string errorMsg, string connString = null, IDbConnection transConn = null);

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
        public abstract List<T> GetEntitiesBySql(out string errorMsg, string sql, object[] sqlParams = null, string connString = null);

        /// <summary>
        /// 以SQL方式执行查询
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract DataTable ExecuteQuery(out string errorMsg, string sql, Dictionary<string, object> sqlParams = null, string connString = null);

        /// <summary>
        /// 获取查询到的第一行第一列的数据
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract object ExecuteScale(out string errorMsg, string sql, object[] sqlParams = null, string connString = null);

        /// <summary>
        /// 执行增删改语句
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="sql">sql</param>
        /// <param name="sqlParams">sql参数</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="transConn">事务连接对象</param>
        /// <returns></returns>
        public abstract int ExecuteNonQuery(out string errorMsg, string sql, Dictionary<string, object> sqlParams = null, string connString = null, IDbConnection transConn = null);

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
        public abstract int RunProcedureNoQuery(out string errorMsg, ref Dictionary<string, object> outParams, string procedureName, object inParams = null, string connString = null, IDbConnection transConn = null);

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
        public abstract DataTable RunProcedure(out string errorMsg, ref Dictionary<string, object> outParams, string procedureName, object inParams = null, string connString = null, IDbConnection transConn = null);

        #endregion
        #region 事务操作
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transactionObjects">事务对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public abstract void ExecuteTransaction(List<TransactionModel<T>> transactionObjects, out string errorMsg, string connString = null);

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transactionObjects">事务扩展对象集合</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public abstract void ExecuteTransactionExtend(List<TransactionExtendModel> transactionObjects, out string errorMsg, string connString = null);

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="transTask">事务处理函数</param>
        /// <param name="errorMsg">异常信息</param>
        /// <param name="connString">数据库连接字符串</param>
        public abstract void TransactionHandle(TransactionTask transTask, out string errorMsg, string connString = null);
        #endregion
        #region 数据库操作

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>创建成功返回空字符串，失败返回异常信息</returns>
        public abstract string CreateTable(string connString = null);

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>删除成功返回空字符串，失败返回异常信息</returns>
        public abstract string DropTable(string connString = null);

        /// <summary>
        /// 修改数据表
        /// </summary>
        /// <param name="command">操作sql，如：[ALTER TABLE a] ADD Column b int</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public abstract string AlterTable(string command, string connString = null);

        /// <summary>
        /// 数据列是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract bool ColumnIsExists(string fieldName, string connString = null);

        /// <summary>
        /// 增加数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public abstract string AddColumn(string fieldName, string connString = null);

        /// <summary>
        /// 修改数据列
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public abstract string AlterColumn(string fieldName, string connString = null);

        /// <summary>
        /// 修改列名
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="oldFieldName">旧的字段名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public abstract string ChangeColumnName(string fieldName, string oldFieldName, string connString = null);

        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract string DropColumn(string columnName, string connString = null);

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="indexName">索引名</param>
        /// <param name="unique">是否唯一索引</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns>成功返回空字符串，失败返回异常信息</returns>
        public abstract string CreateIndex(Expression<Func<T, object>> field, string indexName = null, bool unique = false, string connString = null);

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public abstract string DropIndex(string indexName, string connString = null);

        #endregion
        #region 其他

        /// <summary>
        /// 将lamda条件表达式转换成sql
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public abstract string ExpressionConditionToWhereSql(Expression<Func<T, bool>> expression);
        #endregion
        #endregion
    }
}

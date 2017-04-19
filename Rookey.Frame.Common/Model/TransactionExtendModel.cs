/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common.PubDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Common.Model
{
    /// <summary>
    /// 委托（用于异步处理事务）
    /// </summary>
    /// <param name="conn">事务连接对象</param>
    public delegate void TransactionTask(IDbConnection conn);

    /// <summary>
    /// 数据库事务扩展类
    /// </summary>
    public class TransactionExtendModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_modelType">实体类型</param>
        /// <param name="_operateType">操作类型</param>
        /// <param name="_models">实体集合</param>
        public TransactionExtendModel(Type _modelType, DataOperateType _operateType, IEnumerable _models)
        {
            modelType = _modelType;
            operateType = _operateType;
            models = _models;
        }

        private Type modelType;

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type ModelType
        {
            get { return modelType; }
        }

        private DataOperateType operateType;

        /// <summary>
        /// 操作类型
        /// </summary>
        public DataOperateType OperateType
        {
            get { return operateType; }
        }

        private IEnumerable models;

        /// <summary>
        /// 实体数据集合List<T>
        /// </summary>
        public IEnumerable Models
        {
            get { return models; }
        }
    }
}

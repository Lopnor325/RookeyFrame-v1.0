/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common.PubDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Common.Model
{
    /// <summary>
    /// 数据库事务对象
    /// </summary>
    public class TransactionModel<T> where T : class
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_operateType">操作类型</param>
        /// <param name="_models">实体集合</param>
        /// <param name="references">是否保存实体的关联表数据</param>
        public TransactionModel(DataOperateType _operateType, List<T> _models, bool _references = false)
        {
            operateType = _operateType;
            models = _models;
            references = _references;
        }

        private DataOperateType operateType;

        /// <summary>
        /// 操作类型
        /// </summary>
        public DataOperateType OperateType
        {
            get { return operateType; }
        }

        private List<T> models;

        /// <summary>
        /// 操作实体集合
        /// </summary>
        public List<T> Models
        {
            get { return models; }
        }

        private bool references = false;

        /// <summary>
        /// 是否保存实体的关联表数据
        /// </summary>
        public bool References
        {
            get { return references; }
        }
    }
}

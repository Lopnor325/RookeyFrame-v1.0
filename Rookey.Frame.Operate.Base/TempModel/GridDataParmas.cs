/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Operate.Base.EnumDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 默认网格数据参数
    /// </summary>
    public class GridDataParmas
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        public GridDataParmas(Guid moduleId)
        {
            ModuleId = moduleId;
            PagingInfo = PageInfo.GetDefaultPageInfo();
            GridType = DataGridType.MainGrid;
            IsPermissionFilter = true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="q">搜索关键字</param>
        /// <param name="condition">过滤条件</param>
        public GridDataParmas(Guid moduleId, PageInfo pageInfo, string q, string condition)
        {
            ModuleId = moduleId;
            PagingInfo = pageInfo == null ? PageInfo.GetDefaultPageInfo() : pageInfo;
            Q = q;
            Condition = condition;
            GridType = DataGridType.MainGrid;
            IsPermissionFilter = true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="q">搜索关键字</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="items">过滤条件集合</param>
        public GridDataParmas(Guid moduleId, PageInfo pageInfo, string q, string condition, List<ConditionItem> items)
            : this(moduleId, pageInfo, q, condition)
        {
            CdItems = items;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="q">搜索关键字</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="items">过滤条件集合</param>
        /// <param name="whereCon">Where条件语句</param>
        public GridDataParmas(Guid moduleId, PageInfo pageInfo, string q, string condition, List<ConditionItem> items, string whereCon)
            : this(moduleId, pageInfo, q, condition, items)
        {
            WhereCon = whereCon;
        }

        /// <summary>
        /// 模块Id
        /// </summary>
        public Guid ModuleId { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PageInfo PagingInfo { get; set; }

        /// <summary>
        /// 搜索关键字，必须是可反序列化成Dictionary<string, string>
        /// </summary>
        public string Q { get; set; }

        /// <summary>
        /// 网格过滤规则
        /// </summary>
        public List<ConditionItem> FilterRules { get; set; }

        /// <summary>
        /// 过滤条件，必须是通过UrlDecode反编码后可反序列化成Dictionary<string, string>
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 过滤条件集合
        /// </summary>
        public List<ConditionItem> CdItems { get; set; }

        /// <summary>
        /// Where条件语句
        /// </summary>
        public string WhereCon { get; set; }

        /// <summary>
        /// 网格类型
        /// </summary>
        public DataGridType GridType { get; set; }

        /// <summary>
        /// 其他参数
        /// </summary>
        public Dictionary<string, string> OtherParams { get; set; }

        /// <summary>
        /// 综合视图Id，为综合视图时传递
        /// </summary>
        public Guid? ViewId { get; set; }

        /// <summary>
        /// 当前加载视图ID
        /// </summary>
        public Guid? GridViewId { get; set; }

        /// <summary>
        /// 是否综合明细视图，综合视图中带明细字段
        /// </summary>
        public bool IsComprehensiveDetailView { get; set; }

        /// <summary>
        /// 是否明细复制
        /// </summary>
        public bool IsDetailCopy { get; set; }

        /// <summary>
        /// 是否是树型网格
        /// </summary>
        public bool IsTreeGrid { get; set; }

        /// <summary>
        /// 是否权限过滤
        /// </summary>
        public bool IsPermissionFilter { get; set; }

        /// <summary>
        /// 是否为重新发起流程
        /// </summary>
        public bool IsRestartFlow { get; set; }

        /// <summary>
        /// 是否导出数据
        /// </summary>
        public bool IsExportData { get; set; }

        /// <summary>
        /// 加载数据时需要加载的字段
        /// </summary>
        public List<string> NeedLoadFields { get; set; }
    }

    /// <summary>
    /// 弹出框网格数据参数
    /// </summary>
    public class DialogGridDataParams : GridDataParmas
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="initModule">原始模块</param>
        /// <param name="initField">原始字段</param>
        public DialogGridDataParams(Guid moduleId, string initModule, string initField)
            : base(moduleId)
        {
            InitModule = initModule;
            InitField = initField;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="initModule">原始模块</param>
        /// <param name="initField">原始字段</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="q">搜索关键字</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="relyFieldsValue">依赖字段值</param>
        /// <param name="items">过滤条件集合</param>
        /// <param name="whereCon">where条件语句</param>
        public DialogGridDataParams(Guid moduleId, string initModule, string initField, PageInfo pageInfo, string q, string condition, string relyFieldsValue, List<ConditionItem> items, string whereCon)
            : base(moduleId, pageInfo, q, condition, items, whereCon)
        {
            InitModule = initModule;
            InitField = initField;
            RelyFieldsValue = relyFieldsValue;
        }

        /// <summary>
        /// 针对外键弹出框时选择外键模块数据的原始模块，
        /// 如在员工新增页面选择部门时，员工模块就为原始模块
        /// </summary>
        public string InitModule { get; set; }

        /// <summary>
        /// 针对外键弹出框时选择外键模块数据的原始字段，
        /// 如在员工新增页面选择部门时，员工模块的部门字段就为原始字段
        /// </summary>
        public string InitField { get; set; }

        /// <summary>
        /// 针对外键弹出框时选择外键模块数据，表单字段中设置的过滤条件和依赖字段条件
        /// </summary>
        public string RelyFieldsValue { get; set; }
    }

    /// <summary>
    /// 自动完成数据加载参数
    /// </summary>
    public class AutoCompelteDataParams : GridDataParmas
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名</param>
        public AutoCompelteDataParams(Guid moduleId, string fieldName)
            : base(moduleId)
        {
            FieldName = fieldName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="q">搜索关键字</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="items">过滤条件集合</param>
        /// <param name="whereCon">where条件语句</param>
        public AutoCompelteDataParams(Guid moduleId, string fieldName, PageInfo pageInfo, string q, string condition, List<ConditionItem> items, string whereCon)
            : base(moduleId, pageInfo, q, condition, items, whereCon)
        {
            FieldName = fieldName;
        }

        /// <summary>
        /// 实现自动补全功能的字段名
        /// </summary>
        public string FieldName { get; set; }
    }
}

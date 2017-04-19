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
using Rookey.Frame.Common;
using Rookey.Frame.Bridge;
using Rookey.Frame.Operate.Base.EnumDef;
using System.Linq.Expressions;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Model.EnumSpace;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Rookey.Frame.Base;
using Rookey.Frame.Model.Sys;
using System.Reflection;
using Rookey.Frame.Model.OrgM;

namespace Rookey.Frame.Operate.Base.OperateHandle
{
    #region 枚举定义

    /// <summary>
    /// 模块操作类型
    /// </summary>
    public enum ModelRecordOperateType
    {
        /// <summary>
        /// 新增
        /// </summary>
        Add = 0,

        /// <summary>
        /// 编辑
        /// </summary>
        Edit = 1,

        /// <summary>
        /// 删除
        /// </summary>
        Del = 2,

        /// <summary>
        /// 查看
        /// </summary>
        View = 3
    }

    /// <summary>
    /// 接口操作类型
    /// </summary>
    enum OperateInterfaceType
    {
        ModelOperate = 0,
        GridOperate = 1,
        FormOperate = 2,
        TreeOperate = 3,
        PermissionOperate = 4,
        FlowOperate = 5,
        UIDraw = 6,
        GridSearch = 7,
        FlowUI = 8,
        MsgNotify = 9,
        ImExport = 10
    }

    #endregion

    /// <summary>
    /// 操作处理接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelOperateHandle<T> where T : class
    {
        #region 实体操作接口

        /// <summary>
        /// 单个实体操作完成后的处理，针对新增保存后、删除后、修改保存后
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="t">实体对象</param>
        /// <param name="result">操作结果，成功或失败</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="otherParams">其他参数</param>
        void OperateCompeletedHandle(ModelRecordOperateType operateType, T t, bool result, UserInfo currUser, object[] otherParams = null);

        /// <summary>
        /// 单个实体操作前验证或处理，针对新增保存前、删除前、修改保存前
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="t">操作对象</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="otherParams">其他参数</param>
        /// <returns>是否通过验证</returns>
        bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, T t, out string errMsg, object[] otherParams = null);

        /// <summary>
        /// 多个实体操作完成后的处理，针对新增保存后、删除后、修改保存后
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="ts">实体记录集合</param>
        /// <param name="result">操作结果，成功或失败</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="otherParams">其他参数</param>
        void OperateCompeletedHandles(ModelRecordOperateType operateType, List<T> ts, bool result, UserInfo currUser, object[] otherParams = null);

        /// <summary>
        /// 多个实体操作前验证或处理，针对新增保存前、删除前、修改保存前
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="ts">操作对象集合</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="otherParams">其他参数</param>
        /// <returns>是否通过验证</returns>
        bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<T> ts, out string errMsg, object[] otherParams = null);

        #endregion
    }

    /// <summary>
    /// 网格相关操作处理接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGridOperateHandle<T> where T : class
    {
        #region 网格相关接口

        /// <summary>
        /// 网格参数设置
        /// </summary>
        /// <param name="gridType">网格类型</param>
        /// <param name="gridParams">网格参数对象</param>
        /// <param name="request">请求对象</param>
        void GridParamsSet(DataGridType gridType, GridParams gridParams, HttpRequestBase request = null);

        /// <summary>
        /// 网格数据加载参数设置
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="gridDataParams">数据加载参数</param>
        /// <param name="request">请求对象</param>
        void GridLoadDataParamsSet(Sys_Module module, GridDataParmas gridDataParams, HttpRequestBase request = null);

        /// <summary>
        /// 返回分页网格数据前对数据处理
        /// </summary>
        /// <param name="data">处理前的网格数据</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        void PageGridDataHandle(List<T> data, object[] otherParams = null, UserInfo currUser = null);

        /// <summary>
        /// 返回网格过滤条件
        /// </summary>
        /// <param name="where">where条件语句</param>
        /// <param name="gridType">网格类型</param>
        /// <param name="condition">条件参数</param>
        /// <param name="initModule">原始模块（弹出选择外键模块数据的初始模块），弹出列表用到</param>
        /// <param name="initField">原始字段（弹出选择外键模块数据的初始字段），弹出列表用到</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        /// <returns>返回条件表达式</returns>
        Expression<Func<T, bool>> GetGridFilterCondition(out string where, DataGridType gridType, Dictionary<string, string> condition = null, string initModule = null, string initField = null, Dictionary<string, string> otherParams = null, UserInfo currUser = null);

        /// <summary>
        /// 网格按钮操作验证
        /// </summary>
        /// <param name="buttonText">按钮显示名称</param>
        /// <param name="ids">操作的记录Id集合</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        string GridButtonOperateVerify(string buttonText, List<Guid> ids, object[] otherParams = null, UserInfo currUser = null);

        #endregion
    }

    /// <summary>
    /// 网格搜索接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGridSearchHandle<T> where T : class
    {
        #region 网格搜索接口

        /// <summary>
        /// 重写多字段搜索结果
        /// </summary>
        /// <param name="q">搜索键值对</param>
        /// <param name="whereSql">过滤条件语句</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        List<ConditionItem> GetSeachResults(Dictionary<string, string> q, out string whereSql, UserInfo currUser = null);

        /// <summary>
        /// 重写单字段搜索结果
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">字段值</param>
        /// <param name="whereSql">过滤条件语句</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        ConditionItem GetSearchResult(string fieldName, object value, out string whereSql, UserInfo currUser = null);

        #endregion
    }

    /// <summary>
    /// 表单操作处理接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFormOperateHandle<T> where T : class
    {
        #region 表单相关接口

        /// <summary>
        /// 返回表单数据前对表单数据进行处理
        /// </summary>
        /// <param name="t">实体对象</param>
        /// <param name="formType">表单类型</param>
        /// <param name="currUser">当前用户</param>
        void FormDataHandle(T t, FormTypeEnum formType, UserInfo currUser = null);

        /// <summary>
        /// 取表单页面按钮
        /// </summary>
        /// <param name="formType">表单类型</param>
        /// <param name="buttons">原有表单按钮</param>
        /// <param name="isAdd">是否新增页面</param>
        /// <param name="isDraft">是否草稿</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        List<FormButton> GetFormButtons(FormTypeEnum formType, List<FormButton> buttons, bool isAdd = false, bool isDraft = false, UserInfo currUser = null);

        /// <summary>
        /// 获取表单工具标签按钮集合
        /// </summary>
        /// <param name="formType">表单类型</param>
        /// <param name="tags">tags</param>
        /// <param name="isAdd">是否新增页面</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        List<FormToolTag> GetFormToolTags(FormTypeEnum formType, List<FormToolTag> tags, bool isAdd = false, UserInfo currUser = null);

        /// <summary>
        /// 获取智能提示下拉面板显示值，各模块通过重写此方法可以任意设置下拉显示格式
        /// </summary>
        /// <param name="t">实体对象</param>
        /// <param name="initModule">针对编辑表单时，原始模块</param>
        /// <param name="initField">针对编辑表单时，原始字段</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        string GetAutoCompleteDisplay(T t, string initModule, string initField, object[] otherParams = null, UserInfo currUser = null);

        /// <summary>
        /// 重写表单明细保存数据
        /// </summary>
        /// <param name="detailObj">明细表单数据对象</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="currUser">当前用户</param>
        /// <returns>返回是否执行了自定义明细保存方法</returns>
        bool OverSaveDetailData(DetailFormObject detailObj, out string errMsg, UserInfo currUser = null);

        #endregion
    }

    /// <summary>
    /// 权限操作处理接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPermissionHandle<T> where T : class
    {
        #region 权限相关接口
        /// <summary>
        /// 权限过滤条件表达式，如x=>x.Name=="name"
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="filterWhere">权限过滤SQL语句，如 Name='name'</param>
        /// <param name="queryCache">是否从缓存中查询</param>
        /// <returns></returns>
        Expression<Func<T, bool>> GetPermissionExp(UserInfo userInfo, out string filterWhere, bool queryCache);

        /// <summary>
        /// 是否有记录的操作（查看，更新、删除）权限
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="t">单据对象</param>
        /// <param name="type">类型，0-查看，1-更新，2-删除</param>
        /// <returns></returns>
        bool HasRecordOperatePermission(UserInfo userInfo, T t, int type);
        #endregion
    }

    /// <summary>
    /// 流程操作处理接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFlowOperateHandle<T> where T : class
    {
        #region 流程相关接口

        /// <summary>
        /// 获取流程下一处理节点名称
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="workflowName">流程名称</param>
        /// <param name="currNodeName">当前节点名称</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        string GetFlowNextHandleNodeName(Guid recordId, string workflowName, string currNodeName, UserInfo currUser);

        /// <summary>
        /// 流程操作前验证事件
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="workAction">操作动作</param>
        /// <param name="toDoTaskId">待办ID，为空时为发起</param>
        /// <param name="returnNodeId">回退ID</param>
        /// <param name="directHandler">被指派人ID</param>
        /// <param name="currNodeName">当前结点名称</param>
        /// <returns>返回验证失败信息，为空表示验证成功</returns>
        string BeforeFlowOperateCheck(UserInfo currUser, Guid recordId, WorkActionEnum workAction, Guid? toDoTaskId, Guid? returnNodeId = null, Guid? directHandler = null, string currNodeName = null);

        /// <summary>
        /// 流程操作完成后事件处理
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="currNodeName">当前流程结点名称</param>
        /// <param name="currUser">当前处理人</param>
        /// <param name="isOpSuccess">是否操作成功</param>
        /// <param name="workAction">流程操作动作，同意、拒绝、。。。</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="approvalOpinions">审批意见</param>
        void AfterFlowOperateCompleted(Guid id, string currNodeName, UserInfo currUser, bool isOpSuccess, WorkActionEnum workAction, WorkFlowStatusEnum flowStatus, string approvalOpinions);

        /// <summary>
        /// 获取结点处理人
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="workFlowInstId">流程实例ID</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        List<Guid> GetNodeHandler(Guid recordId, string nodeName, Guid workFlowInstId, UserInfo currUser);

        /// <summary>
        /// 获取待办标题
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="workflowName">流程名称</param>
        /// <param name="currUser">发起人</param>
        /// <returns></returns>
        string GetWorkTodoTitle(Guid recordId, string workflowName, UserInfo currUser);

        #endregion
    }

    /// <summary>
    /// 树操作处理接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeOperateHandle<T> where T : class
    {
        #region 树节点相关接口

        /// <summary>
        /// 树节点处理
        /// </summary>
        /// <param name="node">节点对象</param>
        /// <returns></returns>
        void TreeNodeHandle(TreeNode node);

        /// <summary>
        /// 树子节点集合处理
        /// </summary>
        /// <param name="childDatas">所有子结点数据</param>
        /// <param name="currUser">当前用户</param>
        List<T> ChildNodesDataHandler(List<T> childDatas, UserInfo currUser);

        #endregion
    }

    /// <summary>
    /// 消息通知处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMsgNotifyHandle<T> where T : class
    {
        #region 消息通知接口

        /// <summary>
        /// 流程消息通知
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="workAction">操作</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="empStarter">发起人</param>
        /// <param name="nextHandlers">下一处理人及对应跳转URL</param>
        /// <param name="notifyUrl">知会跳转URL</param>
        /// <param name="isEmailNotify">是否邮件通知，否则为短信通知</param>
        /// <param name="otherDicCcs">需要知会的其他人员，主要针对邮件通知</param>
        /// <param name="subjectContens">主题内容，List[0]为主题，List[1]为内容</param>
        /// <returns>返回null继续走通用处理，否则表示处理成功</returns>
        string WorkflowMsgNotify(Guid recordId, string nodeName, WorkActionEnum workAction, WorkFlowStatusEnum flowStatus, UserInfo currUser, OrgM_Emp empStarter, Dictionary<Guid, string> nextHandlers, string notifyUrl, bool isEmailNotify = true, Dictionary<string, string> otherDicCcs = null, List<string> subjectContens = null);

        #endregion
    }

    /// <summary>
    /// 导入导出处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IImExportHandle<T> where T : class
    {
        #region 导入导出处理接口

        /// <summary>
        /// 返回导入模板字段，导入模板中将只出现返回后的字段
        /// </summary>
        /// <param name="formFields">表单字段</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        List<string> GetImportTemplateFields(List<string> formFields, UserInfo currUser);

        /// <summary>
        /// 返回导出字段列
        /// </summary>
        /// <param name="gridFields">列表字段</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        List<string> GetExportFields(List<string> gridFields, UserInfo currUser);

        #endregion
    }

    /// <summary>
    /// UI绘制接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUIDrawHandle<T> where T : class
    {
        #region UI绘制接口

        /// <summary>
        /// 返回网格页面HTML
        /// </summary>
        /// <param name="gridType">网格类型</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="where">where过滤条件</param>
        /// <param name="viewId">视图Id</param>
        /// <param name="initModule">针对表单弹出外键选择框，表单原始模块</param>
        /// <param name="initField">针对表单外键弹出框，表单原始字段</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="detailCopy">明细复制</param>
        /// <param name="filterFields">过滤字段</param>
        /// <param name="menuId">菜单ID，从哪个菜单进来的</param>
        /// <param name="isGridLeftTree">是否网格左侧树，针对附属网格</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        string GetGridHTML(DataGridType gridType = DataGridType.MainGrid, string condition = null, string where = null, Guid? viewId = null, string initModule = null, string initField = null, Dictionary<string, object> otherParams = null, bool detailCopy = false, List<string> filterFields = null, Guid? menuId = null, bool isGridLeftTree = false, HttpRequestBase request = null);

        /// <summary>
        /// 获取简单搜索HTML
        /// </summary>
        /// <param name="searchFields">可搜索字段</param>
        /// <param name="gridType">网格类型</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="where">where过滤条件</param>
        /// <param name="viewId">视图Id</param>
        /// <param name="initModule">针对表单弹出外键选择框，表单原始模块</param>
        /// <param name="initField">针对表单外键弹出框，表单原始字段</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        string GetSimpleSearchHTML(List<Sys_GridField> searchFields, DataGridType gridType = DataGridType.MainGrid, string condition = null, string where = null, Guid? viewId = null, string initModule = null, string initField = null, HttpRequestBase request = null);

        /// <summary>
        /// 获取编辑表单HTML
        /// </summary>
        /// <param name="id">记录Id</param>
        /// <param name="gridId">为网格表单编辑模式的网格Id</param>
        /// <param name="copyId">复制时被复制的记录Id</param>
        /// <param name="showTip">是否显示表单tip按钮</param>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <param name="formId">指定表单ID</param>
        /// <param name="request">请求对象</param>
        string GetEditFormHTML(Guid? id, string gridId = null, Guid? copyId = null, bool showTip = false, Guid? todoTaskId = null, Guid? formId = null, HttpRequestBase request = null);

        /// <summary>
        /// 获取编辑表单明细编辑网格HTML
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="detailTopDisplay">明细是否顶部显示</param>
        /// <param name="copyId">复制记录ID</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        string GetEditDetailHTML(Guid? id, out bool detailTopDisplay, Guid? copyId = null, HttpRequestBase request = null);

        /// <summary>
        /// 获取查看表单HTML
        /// </summary>
        /// <param name="id">记录Id</param>
        /// <param name="gridId">为网格表单查看模式的网格Id</param>
        /// <param name="fromEditPageFlag">从编辑页面点击查看按钮标识</param>
        /// <param name="isRecycle">是否来自回收站</param>
        /// <param name="showTip">是否显示表单tip按钮</param>
        /// <param name="formId">指定表单ID</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        string GetViewFormHTML(Guid id, string gridId = null, string fromEditPageFlag = null, bool isRecycle = false, bool showTip = false, Guid? formId = null, HttpRequestBase request = null);

        /// <summary>
        /// 获取查看表单明细查看网格HTML
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="detailTopDisplay">明细是否顶部显示</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        string GetViewDetailHTML(Guid? id, out bool detailTopDisplay, HttpRequestBase request = null);

        #endregion
    }

    /// <summary>
    /// 流程UI绘制接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUIFlowHandle<T> where T : class
    {
        #region 流程UI绘制
        /// <summary>
        /// 流程UI重新绘制
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        string GetFlowTipsHTML(Guid recordId, WorkFlowStatusEnum flowStatus, UserInfo currUser);
        #endregion
    }

    /// <summary>
    /// 操作处理工厂
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class OperateHandleFactory<T> where T : class
    {
        #region 私有方法

        /// <summary>
        /// 获取操作对象实体
        /// </summary>
        /// <returns></returns>
        internal object GetOperateHandleInstance(OperateInterfaceType interfaceType)
        {
            List<Type> types = BridgeObject.GetCustomerOperateHandleTypes().Where(x => x.GetInterfaces().Length > 0).Where(x => x.Name == string.Format("{0}OperateHandle", typeof(T).Name)).ToList();
            if (types.Count == 0) return null;
            switch (interfaceType)
            {
                case OperateInterfaceType.ModelOperate:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IModelOperateHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IModelOperateHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.GridOperate:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IGridOperateHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IGridOperateHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.GridSearch:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IGridSearchHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IGridSearchHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.FormOperate:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IFormOperateHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IFormOperateHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.TreeOperate:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("ITreeOperateHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (ITreeOperateHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.PermissionOperate:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IPermissionHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IPermissionHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.FlowOperate:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IFlowOperateHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IFlowOperateHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.MsgNotify:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IMsgNotifyHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IMsgNotifyHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.ImExport:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IImExportHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IImExportHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.UIDraw:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IUIDrawHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IUIDrawHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case OperateInterfaceType.FlowUI:
                    {
                        types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IUIFlowHandle")).Count() > 0).ToList();
                        foreach (Type type in types)
                        {
                            try
                            {
                                object obj = (IUIFlowHandle<T>)Activator.CreateInstance(type);
                                if (obj != null) return obj;
                            }
                            catch
                            { }
                        }
                    }
                    break;
            }
            return null;
        }

        #endregion

        #region 实体操作

        /// <summary>
        /// 单个实体操作前验证或处理
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="t">操作对象</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="otherParams">其他参数</param>
        /// <returns>是否通过验证</returns>
        internal bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, T t, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            object instance = GetOperateHandleInstance(OperateInterfaceType.ModelOperate);
            if (instance != null)
            {
                return ((IModelOperateHandle<T>)instance).BeforeOperateVerifyOrHandle(operateType, t, out errMsg, otherParams);
            }
            return true;
        }

        /// <summary>
        /// 单个实体操作完成处理
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="t">实体对象</param>
        /// <param name="result">操作结果</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="otherParams">其他参数</param>
        internal void OperateCompeletedHandle(ModelRecordOperateType operateType, T t, bool result, UserInfo currUser, object[] otherParams = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.ModelOperate);
            if (instance != null)
            {
                //异步处理
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ((IModelOperateHandle<T>)instance).OperateCompeletedHandle(operateType, t, result, currUser, otherParams);
                    }
                    catch { }
                });
            }
        }

        /// <summary>
        /// 多个实体操作前验证或处理
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="ts">操作对象集合</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="otherParams">其他参数</param>
        /// <returns>是否通过验证</returns>
        internal bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<T> ts, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            object instance = GetOperateHandleInstance(OperateInterfaceType.ModelOperate);
            if (instance != null)
            {
                return ((IModelOperateHandle<T>)instance).BeforeOperateVerifyOrHandles(operateType, ts, out errMsg, otherParams);
            }
            return true;
        }

        /// <summary>
        /// 多个实体操作完成处理
        /// </summary>
        /// <param name="operateType">操作类型</param>
        /// <param name="ts">实体对象集合</param>
        /// <param name="result">操作结果</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="otherParams">其他参数</param>
        internal void OperateCompeletedHandles(ModelRecordOperateType operateType, List<T> ts, bool result, UserInfo currUser, object[] otherParams = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.ModelOperate);
            if (instance != null)
            {
                //异步处理
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ((IModelOperateHandle<T>)instance).OperateCompeletedHandles(operateType, ts, result, currUser, otherParams);
                    }
                    catch { }
                });
            }
            if (result && ts != null && ts.Count > 0 && operateType == ModelRecordOperateType.Del)
            {
                if (otherParams != null && !otherParams.FirstOrDefault().ObjToBool()) //硬删除
                {
                    //异步删除附件信息
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            #region 删除附件
                            //硬删除，判断是否有附件，有附件删除相应的附件信息
                            string errMsg = string.Empty;
                            Guid moduleId = SystemOperate.GetModuleIdByTableName(typeof(T).Name);
                            Type modelType = CommonOperate.GetModelType(moduleId);
                            PropertyInfo pid = modelType.GetProperty("Id");
                            if (pid == null) return;
                            List<Guid?> ids = ts.Select(x => pid.GetValue2(x, null).ObjToGuidNull()).ToList();
                            List<Sys_Attachment> tempAttachments = CommonOperate.GetEntities<Sys_Attachment>(out errMsg, x => x.Sys_ModuleId == moduleId && ids.Contains(x.RecordId), null, false);
                            SystemOperate.DeleteAttachment(tempAttachments);
                            #endregion
                        }
                        catch { }
                    });
                }
            }
        }

        #endregion

        #region 网格相关

        /// <summary>
        /// 网格参数设置
        /// </summary>
        /// <param name="gridType">网格类型</param>
        /// <param name="gridParams">网格参数对象</param>
        /// <param name="request">请求对象</param>
        internal void GridParamsSet(DataGridType gridType, GridParams gridParams, HttpRequestBase request = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.GridOperate);
            if (instance != null)
            {
                ((IGridOperateHandle<T>)instance).GridParamsSet(gridType, gridParams, request);
            }
        }

        /// <summary>
        /// 网格数据加载参数设置
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="gridDataParams">数据加载参数</param>
        /// <param name="request">请求对象</param>
        internal void GridLoadDataParamsSet(Sys_Module module, GridDataParmas gridDataParams, HttpRequestBase request = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.GridOperate);
            if (instance != null)
            {
                ((IGridOperateHandle<T>)instance).GridLoadDataParamsSet(module, gridDataParams, request);
            }
        }

        /// <summary>
        /// 返回分页网格数据前对数据处理
        /// </summary>
        /// <param name="data">处理前的网格数据</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        internal void PageGridDataHandle(List<T> data, object[] otherParams = null, UserInfo currUser = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.GridOperate);
            if (instance != null)
            {
                ((IGridOperateHandle<T>)instance).PageGridDataHandle(data, otherParams, currUser);
            }
        }

        /// <summary>
        /// 返回网格过滤条件
        /// </summary>
        /// <param name="where">where条件语句</param>
        /// <param name="gridType">网格类型</param>
        /// <param name="condition">条件参数</param>
        /// <param name="initModule">原始模块（弹出选择外键模块数据的初始模块），弹出列表用到</param>
        /// <param name="initField">原始字段（弹出选择外键模块数据的初始字段），弹出列表用到</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        /// <returns>返回条件表达式</returns>
        internal Expression<Func<T, bool>> GetGridFilterCondition(out string where, DataGridType gridType, Dictionary<string, string> condition = null, string initModule = null, string initField = null, Dictionary<string, string> otherParams = null, UserInfo currUser = null)
        {
            where = string.Empty;
            object instance = GetOperateHandleInstance(OperateInterfaceType.GridOperate);
            if (instance != null)
            {
                return ((IGridOperateHandle<T>)instance).GetGridFilterCondition(out where, gridType, condition, initModule, initField, otherParams, currUser);
            }
            return null;
        }

        /// <summary>
        /// 网格按钮操作验证
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="buttonText">按钮显示名称</param>
        /// <param name="ids">操作的记录Id集合</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        /// <returns>验证通过，返回空字符串，验证不通过，返回验证提示信息</returns>
        internal string GridButtonOperateVerify(Guid moduleId, string buttonText, List<Guid> ids, object[] otherParams = null, UserInfo currUser = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.GridOperate);
            if (instance != null)
            {
                return ((IGridOperateHandle<T>)instance).GridButtonOperateVerify(buttonText, ids, otherParams, currUser);
            }
            return string.Empty;
        }

        #endregion

        #region 网格搜索

        /// <summary>
        /// 重写多字段搜索结果
        /// </summary>
        /// <param name="q">搜索键值对</param>
        /// <param name="whereSql">过滤条件语句</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public List<ConditionItem> GetSeachResults(Dictionary<string, string> q, out string whereSql, UserInfo currUser = null)
        {
            whereSql = string.Empty;
            object instance = GetOperateHandleInstance(OperateInterfaceType.GridSearch);
            if (instance != null)
            {
                return ((IGridSearchHandle<T>)instance).GetSeachResults(q, out whereSql, currUser);
            }
            return null;
        }

        /// <summary>
        /// 重写单字段搜索结果
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">字段值</param>
        /// <param name="whereSql">过滤条件语句</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public ConditionItem GetSearchResult(string fieldName, object value, out string whereSql, UserInfo currUser = null)
        {
            whereSql = string.Empty;
            object instance = GetOperateHandleInstance(OperateInterfaceType.GridSearch);
            if (instance != null)
            {
                return ((IGridSearchHandle<T>)instance).GetSearchResult(fieldName, value, out whereSql, currUser);
            }
            return null;
        }

        #endregion

        #region 表单相关

        /// <summary>
        /// 返回表单数据前对表单数据进行处理
        /// </summary>
        /// <param name="t">实体对象</param>
        /// <param name="formType">表单类型</param>
        /// <param name="currUser">当前用户</param>
        internal void FormDataHandle(T t, FormTypeEnum formType, UserInfo currUser = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FormOperate);
            if (instance != null)
            {
                ((IFormOperateHandle<T>)instance).FormDataHandle(t, formType, currUser);
            }
        }

        /// <summary>
        /// 取表单页面按钮
        /// </summary>
        /// <param name="formType">表单类型</param>
        /// <param name="buttons">原有表单按钮</param>
        /// <param name="isAdd">是否新增页面</param>
        /// <param name="isDraft">是否草稿</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public List<FormButton> GetFormButtons(FormTypeEnum formType, List<FormButton> buttons, bool isAdd = false, bool isDraft = false, UserInfo currUser = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FormOperate);
            if (instance != null)
            {
                return ((IFormOperateHandle<T>)instance).GetFormButtons(formType, buttons, isAdd, isDraft, currUser);
            }
            return new List<FormButton>();
        }

        /// <summary>
        /// 获取表单工具标签按钮集合
        /// </summary>
        /// <param name="formType">表单类型</param>
        /// <param name="tags">tags</param>
        /// <param name="isAdd">是否新增页面</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public List<FormToolTag> GetFormToolTags(FormTypeEnum formType, List<FormToolTag> tags, bool isAdd = false, UserInfo currUser = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FormOperate);
            if (instance != null)
            {
                return ((IFormOperateHandle<T>)instance).GetFormToolTags(formType, tags, isAdd, currUser);
            }
            return new List<FormToolTag>();
        }

        /// <summary>
        /// 获取智能提示下拉面板显示值，各模块通过重写此方法可以任意设置下拉显示格式
        /// </summary>
        /// <param name="t">实体对象</param>
        /// <param name="initModule">针对编辑表单时，原始模块</param>
        /// <param name="initField">针对编辑表单时，原始字段</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public string GetAutoCompleteDisplay(T t, string initModule, string initField, object[] otherParams = null, UserInfo currUser = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FormOperate);
            if (instance != null)
            {
                return ((IFormOperateHandle<T>)instance).GetAutoCompleteDisplay(t, initModule, initField, otherParams, currUser);
            }
            return null;
        }

        /// <summary>
        /// 重写表单明细保存数据
        /// </summary>
        /// <param name="detailObj">明细表单数据对象</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="currUser">当前用户</param>
        /// <returns>返回是否执行了自定义明细保存方法</returns>
        public bool OverSaveDetailData(DetailFormObject detailObj, out string errMsg, UserInfo currUser = null)
        {
            errMsg = string.Empty;
            object instance = GetOperateHandleInstance(OperateInterfaceType.FormOperate);
            if (instance != null)
            {
                return ((IFormOperateHandle<T>)instance).OverSaveDetailData(detailObj, out errMsg, currUser);
            }
            return false;
        }

        #endregion

        #region 树节点相关

        /// <summary>
        /// 树节点处理
        /// </summary>
        /// <param name="node">节点对象</param>
        /// <returns></returns>
        internal void TreeNodeHandle(TreeNode node)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.TreeOperate);
            if (instance != null)
            {
                ((ITreeOperateHandle<T>)instance).TreeNodeHandle(node);
            }
        }

        /// <summary>
        /// 树子节点集合处理
        /// </summary>
        /// <param name="childDatas">所有子结点数据</param>
        /// <param name="currUser">当前用户</param>
        internal List<T> ChildNodesDataHandler(List<T> childDatas, UserInfo currUser)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.TreeOperate);
            if (instance != null)
            {
                return ((ITreeOperateHandle<T>)instance).ChildNodesDataHandler(childDatas, currUser);
            }
            return null;
        }

        #endregion

        #region 权限相关

        /// <summary>
        /// 权限过滤条件表达式，如x=>x.Name=="name"
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="filterWhere">权限过滤SQL语句，如 Name='name'</param>
        /// <param name="queryCache">是否从缓存中查询</param>
        /// <returns></returns>
        internal Expression<Func<T, bool>> GetPermissionExp(UserInfo userInfo, out string filterWhere, bool queryCache)
        {
            filterWhere = string.Empty;
            if (userInfo != null && userInfo.UserName == "admin") return null;
            object instance = GetOperateHandleInstance(OperateInterfaceType.PermissionOperate);
            if (instance != null)
            {
                //先检查有没用全部权限
                Sys_Module module = SystemOperate.GetModuleByTableName(typeof(T).Name);
                if (module == null) return x => false;
                List<string> orgIds = PermissionOperate.GetUserDataPermissions(userInfo.UserId, module.Id, DataPermissionTypeEnum.ViewData);
                if (orgIds.Contains("-1")) //有全部权限
                    return null;
                //调用自定义权限
                return ((IPermissionHandle<T>)instance).GetPermissionExp(userInfo, out filterWhere, queryCache);
            }
            return null;
        }

        /// <summary>
        /// 是否有记录的操作（查看，更新、删除）权限
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="t">单据对象</param>
        /// <param name="type">类型，0-查看，1-更新，2-删除</param>
        /// <returns></returns>
        internal bool HasRecordOperatePermission(UserInfo userInfo, T t, int type)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.PermissionOperate);
            if (instance != null)
            {
                return ((IPermissionHandle<T>)instance).HasRecordOperatePermission(userInfo, t, type);
            }
            return true;
        }

        #endregion

        #region 流程相关

        /// <summary>
        /// 获取流程下一处理节点名称
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="workflowName">流程名称</param>
        /// <param name="currNodeName">当前节点名称</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        internal string GetFlowNextHandleNodeName(Guid recordId, string workflowName, string currNodeName, UserInfo currUser)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FlowOperate);
            if (instance != null)
            {
                return ((IFlowOperateHandle<T>)instance).GetFlowNextHandleNodeName(recordId, workflowName, currNodeName, currUser);
            }
            return string.Empty;
        }

        /// <summary>
        /// 流程操作前验证事件
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="workAction">操作动作</param>
        /// <param name="toDoTaskId">待办ID，为空时为发起</param>
        /// <param name="returnNodeId">回退ID</param>
        /// <param name="directHandler">被指派人ID</param>
        /// <param name="currNodeName">当前节点名称</param>
        /// <returns>返回验证失败信息，为空表示验证成功</returns>
        internal string BeforeFlowOperateCheck(UserInfo currUser, Guid recordId, WorkActionEnum workAction, Guid? toDoTaskId, Guid? returnNodeId = null, Guid? directHandler = null, string currNodeName = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FlowOperate);
            if (instance != null)
            {
                return ((IFlowOperateHandle<T>)instance).BeforeFlowOperateCheck(currUser, recordId, workAction, toDoTaskId, returnNodeId, directHandler, currNodeName);
            }
            return string.Empty;
        }

        /// <summary>
        /// 流程操作完成后事件处理
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="currNodeName">当前流程结点名称</param>
        /// <param name="currUser">当前处理人</param>
        /// <param name="isOpSuccess">是否操作成功</param>
        /// <param name="workAction">流程操作动作，同意、拒绝、。。。</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="approvalOpinions">审批意见</param>
        internal void AfterFlowOperateCompleted(Guid id, string currNodeName, UserInfo currUser, bool isOpSuccess, WorkActionEnum workAction, WorkFlowStatusEnum flowStatus, string approvalOpinions)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FlowOperate);
            if (instance != null)
            {
                //异步处理
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ((IFlowOperateHandle<T>)instance).AfterFlowOperateCompleted(id, currNodeName, currUser, isOpSuccess, workAction, flowStatus, approvalOpinions);
                    }
                    catch { }
                });
            }
        }

        /// <summary>
        /// 获取结点处理人
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="workFlowInstId">流程实例ID</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        internal List<Guid> GetNodeHandler(Guid recordId, string nodeName, Guid workFlowInstId, UserInfo currUser)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FlowOperate);
            if (instance != null)
            {
                try
                {
                    return ((IFlowOperateHandle<T>)instance).GetNodeHandler(recordId, nodeName, workFlowInstId, currUser);
                }
                catch { }
            }
            return new List<Guid>();
        }

        /// <summary>
        /// 获取待办标题
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="workflowName">流程名称</param>
        /// <param name="currUser">发起人</param>
        /// <returns></returns>
        internal string GetWorkTodoTitle(Guid recordId, string workflowName, UserInfo currUser)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FlowOperate);
            if (instance != null)
            {
                try
                {
                    return ((IFlowOperateHandle<T>)instance).GetWorkTodoTitle(recordId, workflowName, currUser);
                }
                catch { }
            }
            return null;
        }

        #endregion

        #region 消息通知

        /// <summary>
        /// 流程消息通知
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="workAction">操作</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="empStarter">发起人</param>
        /// <param name="nextHandlers">下一处理人及对应跳转URL</param>
        /// <param name="notifyUrl">知会时跳转URL</param>
        /// <param name="isEmailNotify">是否邮件通知，否则为短信通知</param>
        /// <param name="otherDicCcs">其他需要知会的人员，主要针对邮件通知</param>
        /// <param name="subjectContens">主题内容，List[0]为主题，List[1]为内容</param>
        /// <returns>返回null继续走通用处理，否则表示处理成功</returns>
        internal string WorkflowMsgNotify(Guid recordId, string nodeName, WorkActionEnum workAction, WorkFlowStatusEnum flowStatus, UserInfo currUser, OrgM_Emp empStarter, Dictionary<Guid, string> nextHandlers, string notifyUrl, bool isEmailNotify = true, Dictionary<string, string> otherDicCcs = null, List<string> subjectContens = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.MsgNotify);
            if (instance != null)
            {
                try
                {
                    return ((IMsgNotifyHandle<T>)instance).WorkflowMsgNotify(recordId, nodeName, workAction, flowStatus, currUser, empStarter, nextHandlers, notifyUrl, isEmailNotify, otherDicCcs, subjectContens);
                }
                catch { }
            }
            return null;
        }

        #endregion

        #region 导入导出处理接口

        /// <summary>
        /// 返回导入模板字段，导入模板中将只出现返回后的字段
        /// </summary>
        /// <param name="formFields">表单字段</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        internal List<string> GetImportTemplateFields(List<string> formFields, UserInfo currUser)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.ImExport);
            if (instance != null)
            {
                try
                {
                    return ((IImExportHandle<T>)instance).GetImportTemplateFields(formFields, currUser);
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// 返回导出字段列
        /// </summary>
        /// <param name="gridFields">列表字段</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        internal List<string> GetExportFields(List<string> gridFields, UserInfo currUser)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.ImExport);
            if (instance != null)
            {
                try
                {
                    return ((IImExportHandle<T>)instance).GetExportFields(gridFields, currUser);
                }
                catch { }
            }
            return null;
        }

        #endregion

        #region UI绘制

        /// <summary>
        /// 返回网格页面HTML
        /// </summary>
        /// <param name="gridType">网格类型</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="where">where过滤条件</param>
        /// <param name="viewId">视图Id</param>
        /// <param name="initModule">针对表单弹出外键选择框，表单原始模块</param>
        /// <param name="initField">针对表单外键弹出框，表单原始字段</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="detailCopy">明细复制</param>
        /// <param name="filterFields">过滤字段</param>
        /// <param name="menuId">菜单ID，从哪个菜单进来的</param>
        /// <param name="isGridLeftTree">是否网格左侧树，针对附属网格</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        internal string GetGridHTML(DataGridType gridType = DataGridType.MainGrid, string condition = null, string where = null, Guid? viewId = null, string initModule = null, string initField = null, Dictionary<string, object> otherParams = null, bool detailCopy = false, List<string> filterFields = null, Guid? menuId = null, bool isGridLeftTree = false, HttpRequestBase request = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.UIDraw);
            if (instance != null)
            {
                return ((IUIDrawHandle<T>)instance).GetGridHTML(gridType, condition, where, viewId, initModule, initField, otherParams, detailCopy, filterFields, menuId, isGridLeftTree, request);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取简单搜索HTML
        /// </summary>
        /// <param name="searchFields">可搜索字段</param>
        /// <param name="gridType">网格类型</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="where">where过滤条件</param>
        /// <param name="viewId">视图Id</param>
        /// <param name="initModule">针对表单弹出外键选择框，表单原始模块</param>
        /// <param name="initField">针对表单外键弹出框，表单原始字段</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        internal string GetSimpleSearchHTML(List<Sys_GridField> searchFields, DataGridType gridType = DataGridType.MainGrid, string condition = null, string where = null, Guid? viewId = null, string initModule = null, string initField = null, HttpRequestBase request = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.UIDraw);
            if (instance != null)
            {
                return ((IUIDrawHandle<T>)instance).GetSimpleSearchHTML(searchFields, gridType, condition, where, viewId, initModule, initField, request);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取编辑表单HTML
        /// </summary>
        /// <param name="id">记录Id</param>
        /// <param name="gridId">为网格表单编辑模式的网格Id</param>
        /// <param name="copyId">复制时被复制的记录Id</param>
        /// <param name="showTip">是否显示表单tip按钮</param>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <param name="formId">指定表单ID</param>
        /// <param name="request">请求对象</param>
        internal string GetEditFormHTML(Guid? id, string gridId = null, Guid? copyId = null, bool showTip = false, Guid? todoTaskId = null, Guid? formId = null, HttpRequestBase request = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.UIDraw);
            if (instance != null)
            {
                return ((IUIDrawHandle<T>)instance).GetEditFormHTML(id, gridId, copyId, showTip, todoTaskId, formId, request);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取编辑表单明细编辑网格HTML
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="detailTopDisplay">明细是否顶部显示</param>
        /// <param name="copyId">复制记录ID</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        internal string GetEditDetailHTML(Guid? id, out bool detailTopDisplay, Guid? copyId = null, HttpRequestBase request = null)
        {
            detailTopDisplay = false;
            object instance = GetOperateHandleInstance(OperateInterfaceType.UIDraw);
            if (instance != null)
            {
                return ((IUIDrawHandle<T>)instance).GetEditDetailHTML(id, out detailTopDisplay, copyId, request);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取查看表单HTML
        /// </summary>
        /// <param name="id">记录Id</param>
        /// <param name="gridId">为网格表单查看模式的网格Id</param>
        /// <param name="fromEditPageFlag">从编辑页面点击查看按钮标识</param>
        /// <param name="isRecycle">是否来自回收站</param>
        /// <param name="showTip">是否显示表单tip按钮</param>
        /// <param name="formId">指定表单ID</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        internal string GetViewFormHTML(Guid id, string gridId = null, string fromEditPageFlag = null, bool isRecycle = false, bool showTip = false, Guid? formId = null, HttpRequestBase request = null)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.UIDraw);
            if (instance != null)
            {
                return ((IUIDrawHandle<T>)instance).GetViewFormHTML(id, gridId, fromEditPageFlag, isRecycle, showTip, formId, request);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取查看表单明细查看网格HTML
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="detailTopDisplay">明细是否顶部显示</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        internal string GetViewDetailHTML(Guid? id, out bool detailTopDisplay, HttpRequestBase request = null)
        {
            detailTopDisplay = false;
            object instance = GetOperateHandleInstance(OperateInterfaceType.UIDraw);
            if (instance != null)
            {
                return ((IUIDrawHandle<T>)instance).GetViewDetailHTML(id, out detailTopDisplay, request);
            }
            return string.Empty;
        }

        #endregion

        #region 流程UI绘制
        /// <summary>
        /// 流程UI重新绘制
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        internal string GetFlowTipsHTML(Guid recordId, WorkFlowStatusEnum flowStatus, UserInfo currUser)
        {
            object instance = GetOperateHandleInstance(OperateInterfaceType.FlowUI);
            if (instance != null)
            {
                return ((IUIFlowHandle<T>)instance).GetFlowTipsHTML(recordId, flowStatus, currUser);
            }
            return string.Empty;
        }
        #endregion
    }

    /// <summary>
    /// 用户操作处理接口
    /// </summary>
    public interface IUserOperateHandle
    {
        /// <summary>
        /// 登录成功后
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="request">request</param>
        /// <param name="response">response</param>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="expires">有效时间（分钟）</param>
        void AfterLoginSuccess(HttpSessionStateBase session, HttpRequestBase request, HttpResponseBase response, string username, string pwd, int expires);

        /// <summary>
        /// 用户注册后操作处理
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="nickName">昵称</param>
        void AfterRegiterUser(string username, string pwd, string nickName = null);

        /// <summary>
        /// 用户修改密码后操作处理
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        void AfterChangePwd(string username, string oldPwd, string newPwd);

        /// <summary>
        /// 删除用户后处理操作
        /// </summary>
        /// <param name="username"></param>
        void AfterDeleteUser(string username);
    }

    /// <summary>
    /// 用户操作处理
    /// </summary>
    internal class UserOperateHandleFactory
    {
        /// <summary>
        /// 获取通用操作接口对象
        /// </summary>
        /// <returns></returns>
        public List<IUserOperateHandle> GetUserOperateHandleInstances()
        {
            List<IUserOperateHandle> operates = new List<IUserOperateHandle>();
            List<Type> types = BridgeObject.GetCustomerOperateHandleTypes().Where(x => x.GetInterfaces().Length > 0).Where(x => x.Name.EndsWith("OperateHandle")).ToList();
            if (types.Count == 0) return operates;
            types = types.Where(x => x.GetInterfaces().Select(y => y.Name.Contains("IUserOperateHandle")).Count() > 0).ToList();
            foreach (Type type in types)
            {
                try
                {
                    IUserOperateHandle obj = (IUserOperateHandle)Activator.CreateInstance(type);
                    if (obj != null) operates.Add(obj);
                }
                catch
                { }
            }
            return operates;
        }

        /// <summary>
        /// 登录成功后操作处理
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="request">request</param>
        /// <param name="response">response</param>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="expires">有效时间（分钟）</param>
        public void AfterLoginSuccess(HttpSessionStateBase session, HttpRequestBase request, HttpResponseBase response, string username, string pwd, int expires)
        {
            List<IUserOperateHandle> instances = GetUserOperateHandleInstances();
            if (instances.Count > 0)
            {
                foreach (IUserOperateHandle instance in instances)
                {
                    instance.AfterLoginSuccess(session, request, response, username, pwd, expires);
                }
            }
        }

        /// <summary>
        /// 用户注册后操作处理
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="nickName">昵称</param>
        public void AfterRegiterUser(string username, string pwd, string nickName = null)
        {
            List<IUserOperateHandle> instances = GetUserOperateHandleInstances();
            if (instances.Count > 0)
            {
                foreach (IUserOperateHandle instance in instances)
                {
                    try
                    {
                        instance.AfterRegiterUser(username, pwd, nickName);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 用户修改密码后操作处理
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        public void AfterChangePwd(string username, string oldPwd, string newPwd)
        {
            List<IUserOperateHandle> instances = GetUserOperateHandleInstances();
            if (instances.Count > 0)
            {
                foreach (IUserOperateHandle instance in instances)
                {
                    instance.AfterChangePwd(username, oldPwd, newPwd);
                }
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="username"></param>
        public void AfterDeleteUser(string username)
        {
            List<IUserOperateHandle> instances = GetUserOperateHandleInstances();
            if (instances.Count > 0)
            {
                foreach (IUserOperateHandle instance in instances)
                {
                    instance.AfterDeleteUser(username);
                }
            }
        }
    }
}

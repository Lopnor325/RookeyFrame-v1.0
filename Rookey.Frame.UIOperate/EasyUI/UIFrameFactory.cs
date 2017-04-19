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
using System.Reflection;
using System.Text;
using System.Web;
using Rookey.Frame.Common;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Operate.Base.EnumDef;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.UIOperate.Control;
using Rookey.Frame.Base;
using Rookey.Frame.EntityBase;
using Rookey.Frame.Model.Bpm;
using Rookey.Frame.Base.User;
using Rookey.Frame.Base.Set;
using Rookey.Frame.Model.OrgM;

namespace Rookey.Frame.UIOperate
{
    /// <summary>
    /// UI框架工厂类
    /// </summary>
    public abstract class UIFrameFactory
    {
        #region 实例化工厂

        /// <summary>
        /// 实例化工厂
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static UIFrameFactory GetInstance(HttpRequestBase request)
        {
            UIFrameFactory factory = new EasyUIFrame();
            if (factory != null)
            {
                factory.CurrUser = UserInfo.GetCurretnUser(ApplicationObject.GetHttpContext(request));
                factory.Init();
            }
            return factory;
        }
        #endregion

        #region 属性
        private UserInfo currUser;
        /// <summary>
        /// 当前用户
        /// </summary>
        public UserInfo CurrUser
        {
            get { return currUser; }
            set { currUser = value; }
        }
        #endregion

        #region 抽象方法

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public abstract void Init();

        #endregion

        #region 登录页面

        /// <summary>
        /// 获取登录页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetLoginHTML();

        /// <summary>
        /// 获取弹出登录框页面HTML
        /// </summary>
        /// <returns></returns>
        public abstract string GetDialogLoginHTML();

        #endregion

        #region 主页面

        /// <summary>
        /// 获取前端框架主页面HTML
        /// </summary>
        /// <returns></returns>
        public abstract string GetMainPageHTML();

        /// <summary>
        /// 获取个人设置页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetPersonalSetHTML();

        /// <summary>
        /// 获取修改密码页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetChangePwdHTML();

        /// <summary>
        /// 获取添加快捷菜单页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetAddQuckMenuHTML();

        #endregion

        #region 列表页面

        /// <summary>
        /// 返回网格页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
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
        public abstract string GetGridHTML(Guid moduleId, DataGridType gridType = DataGridType.MainGrid, string condition = null, string where = null, Guid? viewId = null, string initModule = null, string initField = null, Dictionary<string, object> otherParams = null, bool detailCopy = false, List<string> filterFields = null, Guid? menuId = null, bool isGridLeftTree = false, HttpRequestBase request = null);

        /// <summary>
        /// 返回高级搜索页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="viewId">视图Id</param>
        /// <returns></returns>
        public abstract string GetAdvanceSearchHTML(Guid moduleId, Guid? viewId);

        /// <summary>
        /// 加载快速编辑视图页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="viewId">视图ID</param>
        /// <returns></returns>
        public abstract string GetQuickEditViewHTML(Guid moduleId, Guid? viewId);

        /// <summary>
        /// 获取列表视图设置页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public abstract string GetGridSetHTML(Guid moduleId);

        /// <summary>
        /// 获取附属模块设置页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public abstract string GetAttachModuleSetHTML(Guid moduleId);

        #endregion

        #region 表单页面

        /// <summary>
        /// 获取通用表单页面HTML
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="id">记录Id</param>
        /// <param name="gridId">为网格表单编辑模式的网格Id</param>
        /// <param name="copyId">复制时被复制的记录Id</param>
        /// <param name="showTip">是否显示表单tip按钮</param>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <param name="formId">表单ID</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public abstract string GetEditFormHTML(Guid moduleId, Guid? id, string gridId = null, Guid? copyId = null, bool showTip = false, Guid? todoTaskId = null, Guid? formId = null, HttpRequestBase request = null);

        /// <summary>
        /// 获取通用查看表单页面HTML
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="id">记录Id</param>
        /// <param name="gridId">明细网格Id</param>
        /// <param name="fromEditPageFlag">从编辑页面点击查看按钮标识</param>
        /// <param name="isRecycle">是否来自回收站</param>
        /// <param name="showTip">是否显示表单tip按钮</param>
        /// <param name="formId">表单ID</param>        
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public abstract string GetViewFormHTML(Guid moduleId, Guid id, string gridId = null, string fromEditPageFlag = null, bool isRecycle = false, bool showTip = false, Guid? formId = null, HttpRequestBase request = null);

        /// <summary>
        /// 多文件上传表单HTML
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public abstract string GetUploadMitiFileHTML(Guid? moduleId);

        /// <summary>
        /// 获取编辑字段页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="recordId">记录Id</param>
        /// <returns></returns>
        public abstract string GetEditFieldHTML(Guid moduleId, string fieldName, Guid recordId);

        /// <summary>
        /// 获取批量编辑页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="selectRecords">选择的记录数</param>
        /// <param name="pageRecords">当前页记录数</param>
        /// <returns></returns>
        public abstract string GetBatchEditHTML(Guid moduleId, int selectRecords, int pageRecords);

        /// <summary>
        /// 获取实体导入页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public abstract string GetImportModelHTML(Guid moduleId);

        /// <summary>
        /// 获取导出设置页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="cc">当前记录数</param>
        /// <returns></returns>
        public abstract string GetExportModelHTML(Guid moduleId, int cc);

        /// <summary>
        /// 获取下拉框数据源设置页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetCombDataSourceSetHTML();

        #endregion

        #region 公共页面

        /// <summary>
        /// 返回选择图标页面HTML
        /// </summary>
        /// <returns></returns>
        public abstract string GetIconSelectHTML();

        /// <summary>
        /// 获取弹出树HTML
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public abstract string GetDialogTreeHTML(Guid moduleId, HttpRequestBase request);

        #endregion

        #region 特殊页面

        /// <summary>
        /// 设置角色表单页面
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public abstract string GetSetRoleFormHTML(Guid roleId);

        /// <summary>
        /// 加载快速编辑角色表单页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="formId">表单Id</param>
        /// <returns></returns>
        public abstract string GetQuickEditFormHTML(Guid moduleId, Guid roleId, Guid? formId);

        /// <summary>
        /// 获取设置用户角色页面
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public abstract string GetSetUserRoleHTML(Guid userId);

        /// <summary>
        /// 获取系统配置页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetWebConfigHTML();

        /// <summary>
        /// 添加通用按钮
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <returns></returns>
        public abstract string GetAddCommonBtnHTML(Guid moduleId);

        #endregion

        #region 权限页面

        #region 角色权限

        /// <summary>
        /// 获取设置角色模块权限页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetSetRoleModulePermissionHTML();

        /// <summary>
        /// 获取设置角色权限页面
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public abstract string GetSetRolePermissionHTML(Guid? roleId);

        /// <summary>
        /// 获取角色数据权限设置页面
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="type">数据权限类型</param>
        /// <returns></returns>
        public abstract string GetRoleDataPermissionSetHTML(string moduleName, string roleName, int type);

        /// <summary>
        /// 获取角色字段权限设置页面
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="type">字段权限类型</param>
        /// <returns></returns>
        public abstract string GetRoleFieldPermissionSetHTML(string moduleName, string roleName, int type);

        #endregion

        #region 用户权限

        /// <summary>
        /// 获取设置用户权限页面
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public abstract string GetSetUserPermissionHTML(Guid? userId);

        /// <summary>
        /// 获取用户数据权限设置页面
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="userId">用户Id</param>
        /// <param name="type">数据权限类型</param>
        /// <returns></returns>
        public abstract string GetUserDataPermissionSetHTML(string moduleName, Guid userId, int type);

        /// <summary>
        /// 获取用户字段权限设置页面
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="userId">用户Id</param>
        /// <param name="type">字段权限类型</param>
        /// <returns></returns>
        public abstract string GetUserFieldPermissionSetHTML(string moduleName, Guid userId, int type);

        #endregion

        #endregion

        #region 桌面页面

        /// <summary>
        /// 获取我的桌面页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetDesktopIndexHTML();

        /// <summary>
        /// 获取通用桌面列表页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="top">取前多少条</param>
        /// <param name="sortName">排序字段</param>
        /// <param name="isDesc">是否降序</param>
        /// <param name="dic">其他参数</param>
        /// <returns></returns>
        public abstract string GetDesktopGridHTML(Guid moduleId, int top = 5, string sortName = "Id", bool isDesc = true, Dictionary<string, object> dic = null);

        #endregion

        #region 邮件管理

        /// <summary>
        /// 获取邮件首页
        /// </summary>
        /// <returns></returns>
        public abstract string GetEmailIndexHTML();

        #endregion

        #region 流程页面

        /// <summary>
        /// 获取流程设计页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetFlowDesignHTML();

        /// <summary>
        /// 获取流程画布页面
        /// </summary>
        /// <returns></returns>
        public abstract string GetFlowCanvasHTML();

        /// <summary>
        /// 获取流程节点参数设置页面
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="tagId">节点TagId</param>
        /// <returns></returns>
        public abstract string GetNodeParamSetHTML(Guid workflowId, string tagId);

        /// <summary>
        /// 获取流程连接线参数设置页面
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="tagId">连线TagId</param>
        /// <returns></returns>
        public abstract string GetLineParamSetHTML(Guid workflowId, string tagId);

        /// <summary>
        /// 获取选择回退结点页面
        /// </summary>
        /// <param name="workTodoId">待办ID</param>
        /// <returns></returns>
        public abstract string GetSelectBackNodeHTML(Guid workTodoId);

        /// <summary>
        /// 获取流程tips页面HTML
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public abstract string GetFlowTipsHTML(Guid moduleId, Guid recordId);

        #endregion

        #region 调度页面

        /// <summary>
        /// 获取调度中心页面HTML
        /// </summary>
        /// <returns></returns>
        public abstract string GetQuartzCenterHTML();

        /// <summary>
        /// 获取任务管理页面HTML
        /// </summary>
        /// <returns></returns>
        public abstract string GetJobManageHTML();

        /// <summary>
        /// 获取添加任务页面HTML
        /// </summary>
        /// <returns></returns>
        public abstract string GetAddJobHTML();

        /// <summary>
        /// 获取添加任务计划页面HTML
        /// </summary>
        /// <returns></returns>
        public abstract string GetAddJobPlanHTML();

        /// <summary>
        /// 获取查看执行日志页面HTML
        /// </summary>
        /// <returns></returns>
        public abstract string GetQuartzLogHTML();

        #endregion

        #endregion

        #region 静态方法

        /// <summary>
        /// 动态计算表单宽度
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="form">表单</param>
        /// <returns></returns>
        internal static int CalcFormWidth(Guid moduleId, Sys_Form form)
        {
            List<Sys_FormField> formFields = SystemOperate.GetFormField(moduleId, form.Name, false);
            int hiddenType = (int)ControlTypeEnum.HiddenBox;
            formFields = formFields.Where(x => x.ControlType != hiddenType).ToList();
            int width = 0;
            if (formFields != null && formFields.Count > 0)
            {
                var tabs = formFields.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.TabName).ToList();
                foreach (var tab in tabs)
                {
                    var groups = tab.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.GroupName).ToList();
                    foreach (var g in groups)
                    {
                        var rows = g.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.RowNo).ToList();
                        foreach (var row in rows)
                        {
                            int w = 0;
                            int space = row.ToList().Count * (form.SpaceWidth > 0 ? form.SpaceWidth : 40);
                            foreach (var field in row)
                            {
                                w += (form.LabelWidth > 0 ? form.LabelWidth : 90) + (field.Width.HasValue ? field.Width.Value : (form.InputWidth > 0 ? form.InputWidth : 180));
                            }
                            w += space;
                            if (width < w) width = w;
                        }
                    }
                }
                width += 40;
            }
            return width;
        }

        /// <summary>
        /// 动态计算表单高度
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="form">表单</param>
        /// <returns></returns>
        internal static int CalcFormHeight(Guid moduleId, Sys_Form form)
        {
            int rowRh = ConstDefine.FORM_ROW_HEIGHT;
            List<Sys_FormField> formFields = SystemOperate.GetFormField(moduleId, form.Name, false);
            int hiddenType = (int)ControlTypeEnum.HiddenBox;
            formFields = formFields.Where(x => x.ControlType != hiddenType).ToList();
            int height = 0;
            if (formFields != null && formFields.Count > 0)
            {
                var tabs = formFields.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.TabName).ToList();
                foreach (var tab in tabs)
                {
                    int maxHeight = 0;
                    var groups = tab.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.GroupName).ToList();
                    foreach (var group in groups)
                    {
                        var rows = group.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.RowNo).ToList();
                        int panelHeight = rows.Count * rowRh + ConstDefine.FORM_PANEL_PADDING * 2;
                        maxHeight += panelHeight;
                    }
                    maxHeight += groups.Count * 3;
                    if (height < maxHeight) height = maxHeight;
                }
            }
            return height;
        }

        /// <summary>
        /// 获取Panel高度
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        internal static int GetPanelHeight(IGrouping<string, Sys_FormField> group)
        {
            if (group == null || group.ToList().Count == 0) return 0;
            int rowRh = ConstDefine.FORM_ROW_HEIGHT;
            var rows = group.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.RowNo).ToList();
            int panelHeight = rows.Count * rowRh + ConstDefine.FORM_PANEL_PADDING * 2;
            return panelHeight;
        }

        /// <summary>
        /// 获取表单panel宽度
        /// </summary>
        /// <param name="group"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        internal static int GetPanelWidth(IGrouping<string, Sys_FormField> group, Sys_Form form)
        {
            if (group == null || group.ToList().Count == 0 || form == null) return 0;
            var rows = group.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.RowNo).ToList();
            int width = 0;
            foreach (var row in rows)
            {
                int w = 0;
                int space = row.ToList().Count * (form.SpaceWidth > 0 ? form.SpaceWidth : 40);
                foreach (var field in row)
                {
                    w += (form.LabelWidth > 0 ? form.LabelWidth : 90) + (field.Width.HasValue ? field.Width.Value : (form.InputWidth > 0 ? form.InputWidth : 180));
                }
                w += space;
                if (width < w) width = w;
            }
            return width;
        }

        /// <summary>
        /// 获取编辑模式
        /// </summary>
        /// <param name="module">模块对象</param>
        /// <param name="form">表单对象</param>
        /// <param name="formWidth">表单宽度</param>
        /// <param name="formHeight">表单高度</param>
        /// <param name="userId">当前用户</param>
        /// <returns></returns>
        internal static int GetEditMode(Sys_Module module, Sys_Form form, out int formWidth, out int formHeight, Guid? userId = null)
        {
            formWidth = 500;
            formHeight = 300;
            if (form == null)
                return (int)ModuleEditModeEnum.TabFormEdit;
            string errMsg = string.Empty;
            int editMode = form.ModuleEditMode.HasValue ? form.ModuleEditMode.Value : 0;
            if (form.ModuleEditModeOfEnum != ModuleEditModeEnum.None &&
                form.ModuleEditModeOfEnum != ModuleEditModeEnum.PopFormEdit)
                return (int)form.ModuleEditModeOfEnum;
            bool upW = false; //是否更新表单宽
            bool upH = false; //是否更新表单高
            object updateObj = null; //更新对象
            if (form.Width.HasValue && form.Width.Value > 0)
            {
                formWidth = form.Width.Value;
            }
            else
            {
                formWidth = CalcFormWidth(module.Id, form); //动态计算表单宽
                if (formWidth > ConstDefine.DIALOG_FORM_MAX_WIDTH)
                    formWidth = ConstDefine.DIALOG_FORM_MAX_WIDTH;
                upW = true;
            }
            if (form.Height.HasValue && form.Height.Value > 0)
            {
                formHeight = form.Height.Value;
            }
            else
            {
                formHeight = CalcFormHeight(module.Id, form); //动态计算表单高
                if (formHeight > ConstDefine.DIALOG_FORM_MAX_HEIGHT)
                    formHeight = ConstDefine.DIALOG_FORM_MAX_HEIGHT;
                upH = true;
            }
            if (upW && upH)
                updateObj = new { Width = formWidth, Height = formHeight };
            else if (upW)
                updateObj = new { Width = formWidth };
            else if (upH)
                updateObj = new { Height = formHeight };
            if (upH || upW)
            {
                try
                {
                    //将计算的表单宽度、高度更新到表单对象
                    bool rs = CommonOperate.UpdateRecordsByExpression<Sys_Form>(updateObj, x => x.Id == form.Id, out errMsg);
                    if (rs)
                    {
                        if (upW) form.Width = formWidth;
                        if (upH) form.Height = formHeight;
                    }
                }
                catch { }
            }
            if (editMode == (int)ModuleEditModeEnum.None) //自适应编辑模式
            {
                //高度超出自动转成标签页编辑模式
                if (formHeight > ConstDefine.DIALOG_FORM_MAX_HEIGHT || formWidth > ConstDefine.DIALOG_FORM_MAX_WIDTH ||
                    SystemOperate.HasDetailModule(module.Id) || (userId.HasValue && SystemOperate.HasUserAttachModule(userId.Value, module.Id, false)))
                {
                    editMode = (int)ModuleEditModeEnum.TabFormEdit;
                }
                else //表单高宽比较小时自动转成弹出框编辑模式
                {
                    editMode = (int)ModuleEditModeEnum.PopFormEdit;
                }
                //当前为明细模块时，如果是tab编辑模式或网格行内编辑模式时自动转成弹出框编辑模式
                if (module.ParentId.HasValue && module.ParentId.Value != Guid.Empty &&
                    (editMode == (int)ModuleEditModeEnum.TabFormEdit || editMode == (int)ModuleEditModeEnum.GridRowBottomFormEdit))
                {
                    editMode = (int)ModuleEditModeEnum.PopFormEdit;
                }
            }
            else if (module.ParentId.HasValue && module.ParentId.Value != Guid.Empty &&
                    (editMode == (int)ModuleEditModeEnum.TabFormEdit || editMode == (int)ModuleEditModeEnum.GridRowBottomFormEdit))
            {
                //当前为明细模块时，如果是tab编辑模式或网格行内编辑模式时自动转成弹出框编辑模式
                editMode = (int)ModuleEditModeEnum.PopFormEdit;
            }
            if (editMode == (int)ModuleEditModeEnum.PopFormEdit)
            {
                //弹出框编辑模式如果有明细或有附属模块则弹出框最大化
                if (SystemOperate.HasDetailModule(module.Id) || (userId.HasValue && SystemOperate.HasUserAttachModule(userId.Value, module.Id, false)))
                {
                    formWidth = ConstDefine.DIALOG_FORM_MAX_WIDTH;
                    formHeight = ConstDefine.DIALOG_FORM_MAX_HEIGHT;
                }
                else if (module.IsEnableAttachment) //启用附件时增加表单高度
                {
                    formHeight += module.FormAttachDisplayStyleOfEnum == FormAttachDisplayStyleEnum.GridStype ? 200 : 100;
                    if (formHeight > ConstDefine.DIALOG_FORM_MAX_HEIGHT)
                        formHeight = ConstDefine.DIALOG_FORM_MAX_HEIGHT;
                }
                if (formWidth < ConstDefine.DIALOG_FORM_MIN_WIDTH) formWidth = ConstDefine.DIALOG_FORM_MIN_WIDTH;
                if (formHeight < ConstDefine.DIALOG_FORM_MIN_HEIGHT) formHeight = ConstDefine.DIALOG_FORM_MIN_HEIGHT;
            }
            else if (editMode == (int)ModuleEditModeEnum.GridRowBottomFormEdit)
            {
                if (module.IsEnableAttachment) //启用附件时增加表单高度
                {
                    formHeight += module.FormAttachDisplayStyleOfEnum == FormAttachDisplayStyleEnum.GridStype ? 200 : 100;
                }
            }
            return editMode;
        }

        /// <summary>
        /// 取自定义页面html
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="methodName">接口方法名称</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        internal static string GetCustomerPageHTML(Guid moduleId, string methodName, object[] args)
        {
            try
            {
                object obj = CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, methodName, args);
                return obj.ObjToStr();
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取表单字段值
        /// </summary>
        /// <param name="textValue">文本字段值</param>
        /// <param name="module">模块</param>
        /// <param name="field">表单字段</param>
        /// <param name="sysField">字段</param>
        /// <param name="model">实体对象</param>
        /// <param name="copyModel">复制对象</param>
        /// <param name="canOp">是否可新增或编辑</param>
        /// <param name="isRestartFlow">是否重新发起流程</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        internal static string GetFormFieldInputValue(out string textValue, Sys_Module module, Sys_FormField field, Sys_Field sysField, object model, object copyModel = null, bool canOp = true, bool isRestartFlow = false, UserInfo currUser = null)
        {
            object value = null;
            textValue = string.Empty;
            if (model == null) //新增页面
            {
                //当前字段为编码字段
                if (!string.IsNullOrWhiteSpace(sysField.Name) && SystemOperate.GetBillCodeFieldName(module) == sysField.Name)
                {
                    if (field.IsAllowAdd == false || !canOp)
                    {
                        textValue = "系统自动生成";
                        value = string.Empty;
                    }
                    else
                    {
                        string billCode = SystemOperate.GetBillCode(module);
                        if (!string.IsNullOrEmpty(billCode))
                            value = billCode;
                    }
                }
                else //未启用编码规则
                {
                    if (field.IsAllowAdd != false) //字段允许新增
                    {
                        value = field.DefaultValue;
                        if (field.ControlType == (int)ControlTypeEnum.TextBox ||
                            field.ControlType == (int)ControlTypeEnum.TextAreaBox)
                        {
                            if (value.ObjToStr().StartsWith("{") && value.ObjToStr().EndsWith("}"))
                            {
                                value = string.Empty;
                            }
                        }
                        if (copyModel != null) //复制对象不为空
                        {
                            if ((field.IsAllowCopy.HasValue && field.IsAllowCopy.Value) || isRestartFlow) //允许复制或是重新发起流程
                                value = CommonOperate.GetModelFieldValueByModel(module.Id, copyModel, sysField.Name);
                        }
                        if (value.ObjToStr() == "currDate") //当前日期
                        {
                            value = DateTime.Now.ToString("yyyy-MM-dd");
                            textValue = value.ObjToStr();
                        }
                        else if (value.ObjToStr() == "currTime") //当前时间
                        {
                            value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            textValue = value.ObjToStr();
                        }
                        else if (currUser != null)
                        {
                            if (value.ObjToStr() == "currDept") //当前部门
                            {
                                value = string.Empty;
                                if (currUser.ExtendUserObject != null)
                                {
                                    EmpExtendInfo empExtend = UserInfo.GetCurrEmpExtendInfo(currUser).FirstOrDefault();
                                    if (empExtend != null)
                                    {
                                        if (empExtend.DeptId.HasValue)
                                        {
                                            value = empExtend.DeptId.Value.ObjToStr();
                                            textValue = empExtend.DeptName.ObjToStr();
                                        }
                                    }
                                }
                            }
                            else if (value.ObjToStr() == "currDuty") //当前职务
                            {
                                value = string.Empty;
                                if (currUser.ExtendUserObject != null)
                                {
                                    EmpExtendInfo empExtend = UserInfo.GetCurrEmpExtendInfo(currUser).FirstOrDefault();
                                    if (empExtend != null)
                                    {
                                        if (empExtend.DutyId.HasValue)
                                        {
                                            value = empExtend.DutyId.Value.ObjToStr();
                                            textValue = empExtend.DutyName.ObjToStr();
                                        }
                                    }
                                }
                            }
                            else if (value.ObjToStr() == "currEmp") //当前员工
                            {
                                value = string.Empty;
                                if (currUser.EmpId.HasValue)
                                {
                                    value = currUser.EmpId.Value.ObjToStr();
                                    textValue = currUser.EmpName.ObjToStr();
                                }
                            }
                            else if (value.ObjToStr() == "currEmpCode") //当前员工编号
                            {
                                value = currUser.EmpCode.ObjToStr();
                                textValue = currUser.EmpCode.ObjToStr();
                            }
                            else if (value.ObjToStr() == "currTel") //当前员工手机
                            {
                                value = string.Empty;
                                if (currUser.EmpId.HasValue)
                                {
                                    OrgM_Emp emp = OrgMOperate.GetEmp(currUser.EmpId.Value);
                                    if (emp != null)
                                    {
                                        value = emp.Mobile.ObjToStr();
                                        textValue = emp.Mobile.ObjToStr();
                                    }
                                }
                            }
                            else if (value.ObjToStr() == "currEmail") //当前员工邮箱
                            {
                                value = string.Empty;
                                if (currUser.EmpId.HasValue)
                                {
                                    OrgM_Emp emp = OrgMOperate.GetEmp(currUser.EmpId.Value);
                                    if (emp != null)
                                    {
                                        value = emp.Email.ObjToStr();
                                        textValue = emp.Email.ObjToStr();
                                    }
                                }
                            }
                        }
                        else if (value.ObjToStr() == "currDept" || value.ObjToStr() == "currDuty" ||
                                 value.ObjToStr() == "currEmp" || value.ObjToStr() == "currEmpCode" ||
                                 value.ObjToStr() == "currTel" || value.ObjToStr() == "currEmail")
                        {
                            value = string.Empty;
                        }
                    }
                }
            }
            else //编辑页面
            {
                value = CommonOperate.GetModelFieldValueByModel(module.Id, model, sysField.Name);
            }
            Type fieldType = SystemOperate.GetFieldType(module.Id, sysField.Name); //字段类型
            if (field.ControlTypeOfEnum == ControlTypeEnum.ComboBox)
            {
                if (SystemOperate.IsForeignField(module.Id, sysField.Name)) //外键字段
                {
                    if (field.UrlOrData == null)
                    {
                        if (value == null) value = Guid.Empty;
                    }
                    bool isMutiSelect = field.IsMultiSelect == true && SystemOperate.GetFieldType(module.Id, sysField.Name) == typeof(String);
                    string fieldName = sysField.Name; //字段名
                    string textFieldName = isMutiSelect ? fieldName : fieldName.Substring(0, fieldName.Length - 2) + "Name";
                    textValue = CommonOperate.GetModelFieldValueByModel(module.Id, model, textFieldName).ObjToStr();
                }
                else if (SystemOperate.IsEnumField(module.Id, sysField.Name)) //枚举字段
                {
                    textValue = SystemOperate.GetEnumFieldDisplayText(module.Id, sysField.Name, value.ObjToStr());
                }
                else if (SystemOperate.IsDictionaryBindField(module.Id, sysField.Name))
                {
                    List<Sys_Dictionary> dics = SystemOperate.GetDictionaryData(module.Id, sysField.Name);
                    if (dics.Count > 0) //有字典值时
                    {
                        if (model == null) //新增时
                        {
                            Sys_Dictionary defaultDic = dics.Where(x => x.IsDefault == true).FirstOrDefault();
                            if (defaultDic != null) //字典设置了默认值
                            {
                                value = defaultDic.Value;
                                textValue = defaultDic.Name;
                            }
                            else //没有默认值
                            {
                                textValue = dics.Where(x => x.Value == value.ObjToStr()).Select(x => x.Name).FirstOrDefault();
                            }
                        }
                        else //编辑
                        {
                            textValue = dics.Where(x => x.Value == value.ObjToStr()).Select(x => x.Name).FirstOrDefault();
                        }
                    }
                }
            }
            else if (field.ControlTypeOfEnum == ControlTypeEnum.DialogGrid ||
                     field.ControlTypeOfEnum == ControlTypeEnum.DialogTree)
            {
                string textField = field.TextField;
                Sys_Module foreignModule = null;
                bool isMutiSelect = field.IsMultiSelect == true && SystemOperate.GetFieldType(module.Id, sysField.Name) == typeof(String);
                if (!string.IsNullOrEmpty(sysField.ForeignModuleName)) //外键字段
                {
                    foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName);
                }
                if (foreignModule != null) //当前字段是外键字段
                {
                    if (string.IsNullOrEmpty(textField))
                        textField = SystemOperate.GetModuleTitleKey(foreignModule.Id);
                    string fieldName = sysField.Name; //字段名
                    //先取当前模块冗余字段值，当冗余字段不存在时从外键模块中取
                    string textFieldName = isMutiSelect ? fieldName : fieldName.Substring(0, fieldName.Length - 2) + "Name";
                    textValue = CommonOperate.GetModelFieldValueByModel(module.Id, model, textFieldName).ObjToStr();
                    if (!string.IsNullOrEmpty(value.ObjToStr()))
                    {
                        if (string.IsNullOrEmpty(textValue) || isMutiSelect)
                        {
                            string errMsg = string.Empty;
                            string tempTextStr = string.Empty;
                            List<Guid> ids = value.ObjToStr().Split(",".ToCharArray()).Select(x => x.ObjToGuid()).ToList();
                            foreach (Guid id in ids)
                            {
                                string tempStr = string.Empty;
                                object tempObj = CommonOperate.GetEntityById(foreignModule.Id, id, out errMsg, new List<string>() { textField });
                                if (tempObj != null)
                                    tempStr = CommonOperate.GetModelFieldValueByModel(foreignModule.Id, tempObj, textField).ObjToStr();
                                if (tempTextStr != string.Empty) tempTextStr += ",";
                                tempTextStr += tempStr;
                            }
                            textValue = tempTextStr;
                        }
                    }
                }
            }
            else if (field.ControlTypeOfEnum == ControlTypeEnum.SingleCheckBox)
            {
                if (value.ObjToBool())
                {
                    value = "1";
                    textValue = "是";
                }
                else
                {
                    value = "0";
                    textValue = "否";
                }
            }
            else if ((fieldType == typeof(Decimal) || fieldType == typeof(Double) || fieldType == typeof(float) ||
                    fieldType == typeof(Decimal?) || fieldType == typeof(Double?) || fieldType == typeof(float?)) &&
                    sysField.Precision.HasValue && sysField.Precision.Value > 0)
            {
                if (value.ObjToStr() != string.Empty && sysField.Precision.HasValue && sysField.Precision.Value > 0)
                {
                    string zeros = string.Empty;
                    for (int i = 0; i < sysField.Precision.Value; i++)
                        zeros += "0";
                    textValue = value.ObjToDecimal().ToString(string.Format("0.{0}", zeros));
                }
                else
                {
                    textValue = value.ObjToStr();
                }
            }
            else if (fieldType == typeof(Int16) || fieldType == typeof(Int32) || fieldType == typeof(Int64) ||
                fieldType == typeof(Int16?) || fieldType == typeof(Int32?) || fieldType == typeof(Int64?))
            {
                if (value.ObjToStr() != string.Empty)
                    textValue = value.ObjToStr().Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                else
                    textValue = string.Empty;
            }
            else if (field.ControlTypeOfEnum == ControlTypeEnum.TextBox ||
                field.ControlTypeOfEnum == ControlTypeEnum.TextAreaBox ||
                field.ControlTypeOfEnum == ControlTypeEnum.RichTextBox ||
                field.ControlTypeOfEnum == ControlTypeEnum.IntegerBox)
            {
                if (string.IsNullOrEmpty(textValue))
                    textValue = value.ObjToStr();
            }
            else if (field.ControlTypeOfEnum == ControlTypeEnum.DateBox ||
                    field.ControlTypeOfEnum == ControlTypeEnum.DateTimeBox)
            {
                if (value.ObjToStr() != string.Empty)
                {
                    value = value.ObjToStr().Replace("/", "-");
                    textValue = value.ObjToStr();
                }
            }
            return value.ObjToStr();
        }

        /// <summary>
        /// 获取查看表单字段值
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="form">表单对象</param>
        /// <param name="field">表单字段</param>
        /// <param name="sysField">字段</param>
        /// <param name="model">实体对象</param>
        /// <param name="id">记录ID</param>
        /// <param name="canView">是否允许查看该字段</param>
        /// <param name="canEdit">是否允许编辑该字段</param>
        /// <param name="hasRecordEditPower">是否有记录编辑权限</param>
        /// <returns></returns>
        internal static string GetFormFieldViewValue(Sys_Module module, Sys_Form form, Sys_FormField field, Sys_Field sysField, object model, Guid id, bool canView, bool canEdit, bool hasRecordEditPower)
        {
            #region 字段显示值处理
            string valueStr = string.Empty;
            string editTagA = string.Empty;
            string v = string.Empty;
            if (canView) //有字段查看权限
            {
                int labelWidth = form.LabelWidth > 0 ? form.LabelWidth : 90;
                int inputWidth = (field.Width.HasValue ? field.Width.Value : (form.InputWidth > 0 ? form.InputWidth : 180));
                int width = labelWidth + inputWidth;
                object value = model == null ? null : CommonOperate.GetModelFieldValueByModel(module.Id, model, sysField.Name);
                valueStr = model == null ? string.Empty : SystemOperate.GetFieldDisplayValue(module.Id, model, field);
                int tempFieldWidth = width + (form.SpaceWidth > 0 ? form.SpaceWidth : 40);
                //编辑图标标记，要求字段可编辑，允许批量编辑并且有当前记录编辑权限
                editTagA = field.IsAllowEdit.HasValue && field.IsAllowEdit.Value && field.IsAllowBatchEdit.HasValue && field.IsAllowBatchEdit.Value && canEdit && hasRecordEditPower ?
                    string.Format("&nbsp;&nbsp;<a id=\"btnEditField_{0}\" moduleId=\"{1}\" moduleName=\"{2}\" fieldName=\"{0}\" recordId=\"{3}\" fieldDisplay=\"{4}\" fieldWidth=\"{5}\" oldValue=\"{6}\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-docEdit',plain:true\" onclick=\"EditField(this)\"></a>",
                    sysField.Name, module.Id, module.Name, id, field.Display, tempFieldWidth, value.ObjToStr()) : string.Empty;
                v = value.ObjToStr();
                if (!string.IsNullOrEmpty(valueStr))
                {
                    Type fieldType = SystemOperate.GetFieldType(module.Id, sysField.Name);
                    if ((fieldType == typeof(Decimal) || fieldType == typeof(Double) || fieldType == typeof(float) ||
                        fieldType == typeof(Decimal?) || fieldType == typeof(Double?) || fieldType == typeof(float?)) &&
                        sysField.Precision.HasValue && sysField.Precision.Value > 0)
                    {
                        string zeros = string.Empty;
                        for (int i = 0; i < sysField.Precision.Value; i++)
                        {
                            zeros += "0";
                        }
                        valueStr = valueStr.ObjToDecimal().ToString(string.Format("0.{0}", zeros));
                    }
                    else if (fieldType == typeof(Int16) || fieldType == typeof(Int32) || fieldType == typeof(Int64) ||
                        fieldType == typeof(Int16?) || fieldType == typeof(Int32?) || fieldType == typeof(Int64?))
                    {
                        valueStr = valueStr.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    }
                }
            }
            else //没有字段查看权限
            {
                valueStr = "******";
            }
            #endregion
            string valueSpan = string.Format("<span id=\"span_{0}\" style=\"word-break:normal;width:auto;display:block;white-space:pre-wrap;word-wrap:break-word;overflow:hidden;\" value=\"{2}\">{1}</span>", sysField.Name, valueStr, HttpUtility.HtmlEncode(v));
            if (field.ControlTypeOfEnum == ControlTypeEnum.IconBox) //图标控件特殊处理
            {
                string imgUrl = SystemOperate.GetIconUrl(valueStr);
                valueSpan = string.Format("<span id=\"span_{0}\" style=\"display:none;\">{1}</span><img src=\"{2}\" />", sysField.Name, valueStr, imgUrl);
            }
            return valueSpan + editTagA;
        }

        /// <summary>
        /// 获取表单字段的编辑控件的HTML
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="field">表单字段</param>
        /// <param name="sysField">字段对象</param>
        /// <param name="model">该模块记录</param>
        /// <param name="isSearchForm">是否搜索表单</param>
        /// <param name="linkFields">受当前字段值关联的字段，多个以逗号分隔</param>
        /// <param name="copyModel">被复制的对象</param>
        /// <param name="controlWidth">指定控件宽度</param>
        /// <param name="controlHeight">指定控件高度</param>
        /// <param name="canOp">允许操作（新增、编辑）字段集合</param>
        /// <param name="isCache">是否来自缓存</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="isRestartFlow">是否为重新发起流程</param>
        /// <returns></returns>
        internal static string GetFormFieldInputHTML(Guid moduleId, Sys_FormField field, Sys_Field sysField, object model, bool isSearchForm = false, string linkFields = null, object copyModel = null, int? controlWidth = null, int? controlHeight = null, bool canOp = true, bool? isCache = false, UserInfo currUser = null, bool isRestartFlow = false)
        {
            #region 变量定义
            StringBuilder sb = new StringBuilder();
            bool isDisabled = false; //不允许编辑控件
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            string errMsg = string.Empty;
            object value = null;
            string textValue = string.Empty;
            string foreignModuleName = sysField.ForeignModuleName;
            #endregion
            #region 生成输入控件
            string inputOptions = "data-options=\"tipPosition:'right'";
            bool isAllowOp = true; //是否允许新增或编辑
            if (!isSearchForm) //非搜索表单
            {
                if (!string.IsNullOrEmpty(field.NullTipText))
                    inputOptions += string.Format(",prompt:'{0}'", field.NullTipText);
                if (field.ControlTypeOfEnum != ControlTypeEnum.DialogGrid)
                {
                    string onchangeString = "onChange:function(newValue,oldValue){if(typeof(OnFieldValueChanged)=='function'){OnFieldValueChanged({moduleId:'" + moduleId + "',moduleName:'" + module.Name + "'},'" + sysField.Name + "',newValue,oldValue);}}";
                    inputOptions += "," + onchangeString;
                }
                #region 控件值处理
                //获取表单控件值
                if (isCache.HasValue && isCache.Value) //启用缓存
                {
                    value = "{" + sysField.Name + "}";
                    textValue = "{" + sysField.Name + "_textValue}";
                }
                else
                {
                    value = GetFormFieldInputValue(out textValue, module, field, sysField, model, copyModel, canOp, isRestartFlow, currUser);
                }
                //字段权限
                if (model != null) //编辑页面
                {
                    if (field.IsAllowEdit == false || !canOp) //不允许编辑
                    {
                        isAllowOp = false;
                        isDisabled = true;
                        inputOptions += ",disabled:true";
                    }
                }
                else //新增页面
                {
                    if (field.IsAllowAdd == false || !canOp) //不允许新增
                    {
                        isAllowOp = false;
                        isDisabled = true;
                        inputOptions += ",disabled:true";
                    }
                }
                #endregion
                #region 验证处理
                //必填性验证
                if (field.IsRequired.HasValue && field.IsRequired.Value)
                {
                    inputOptions += ",required:true,missingMessage:null";
                }
                //字符长度验证
                string validTypeStr = string.Empty;
                if (field.MinCharLen.HasValue && field.MinCharLen.Value > 0 && field.MaxCharLen.HasValue && field.MaxCharLen.Value > 0)
                {
                    validTypeStr = string.Format("'length[{0},{1}]'", field.MinCharLen.Value.ToString(), field.MaxCharLen.Value.ToString());
                }
                else if (field.MinCharLen.HasValue && field.MinCharLen.Value > 0)
                {
                    validTypeStr = string.Format("'minLength[{0}]'", field.MinCharLen.Value.ToString());
                }
                else if (field.MaxCharLen.HasValue && field.MaxCharLen.Value > 0)
                {
                    validTypeStr = string.Format("'maxLength:[{0}]'", field.MaxCharLen.Value.ToString());
                }
                //其他验证类型
                switch (field.ValidateTypeOfEnum)
                {
                    case ValidateTypeEnum.email:
                        validTypeStr += ",'email'";
                        break;
                    case ValidateTypeEnum.url:
                        validTypeStr = ",'url'";
                        break;
                    case ValidateTypeEnum.intNum:
                        validTypeStr = ",'int'";
                        break;
                    case ValidateTypeEnum.floatNum:
                        validTypeStr = ",'float'";
                        break;
                }
                if (!string.IsNullOrEmpty(validTypeStr))
                {
                    inputOptions += string.Format(",validType:[{0}]", validTypeStr);
                }
                #endregion
            }
            string valueField = field.ValueField;
            string textField = field.TextField;
            string fieldUrl = HttpUtility.UrlDecode(field.UrlOrData);
            #region 外键字段、枚举字段、字典字段处理
            if (field.UrlOrData == null && field.ControlTypeOfEnum == ControlTypeEnum.ComboBox)
            {
                if (SystemOperate.IsForeignField(moduleId, sysField.Name)) //外键字段
                {
                    fieldUrl = string.Format("/{0}/BindForeignFieldComboData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), sysField.Name);
                }
                else if (SystemOperate.IsEnumField(moduleId, sysField.Name)) //枚举字段
                {
                    valueField = "Id";
                    textField = "Name";
                    fieldUrl = string.Format("/{0}/BindEnumFieldData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), sysField.Name);
                }
                else if (SystemOperate.IsDictionaryBindField(moduleId, sysField.Name)) //字典绑定字段
                {
                    valueField = "Id";
                    textField = "Name";
                    fieldUrl = string.Format("/{0}/BindDictionaryData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), sysField.Name);
                }
            }
            #endregion
            string inputWidthStr = controlWidth.HasValue && controlWidth.Value > 0 ? string.Format("{0}px", controlWidth.Value.ToString()) : "100%";
            Sys_Form form = SystemOperate.GetForm(moduleId, field.Sys_FormName);
            int inputWidth = controlWidth.HasValue && controlWidth.Value > 0 ? controlWidth.Value : (field.Width.HasValue ? field.Width.Value : (form.InputWidth > 0 ? form.InputWidth : 180));
            if (!isSearchForm) //非搜索表单
            {
                //字段帮助信息不为空或表单字段数据源右边添加一个添加数据源的按钮
                if (module.Name == "表单字段" && sysField.Name == "UrlOrData")
                {
                    inputWidth -= 26;
                    inputWidthStr = inputWidth + "px";
                }
            }
            inputWidthStr += controlHeight.HasValue && controlHeight.Value > 0 ? string.Format(";height:{0}px", controlHeight.Value.ToString()) : string.Format(";height:{0}px", !isSearchForm && field.ControlTypeOfEnum == ControlTypeEnum.TextAreaBox ? ConstDefine.FORM_TEXTAREA_CONTROL_HEIGHT.ToString() : ConstDefine.FORM_CONTROL_HEIGHT.ToString());
            #region 控件类型处理
            if (isAllowOp)
            {
                ControlTypeEnum controlTypeEnum = field.ControlTypeOfEnum;
                #region 搜索表单Label控件处理
                if (isSearchForm && controlTypeEnum == ControlTypeEnum.LabelBox) //搜索表单并且是Label控件
                {
                    Type fieldType = SystemOperate.GetFieldType(moduleId, sysField.Name);
                    if (SystemOperate.IsEnumField(moduleId, sysField.Name))
                    {
                        valueField = "Id";
                        textField = "Name";
                        fieldUrl = string.Format("/{0}/BindEnumFieldData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), sysField.Name);
                        controlTypeEnum = ControlTypeEnum.ComboBox;
                    }
                    else if (SystemOperate.IsDictionaryBindField(moduleId, sysField.Name))
                    {
                        valueField = "Id";
                        textField = "Name";
                        fieldUrl = string.Format("/{0}/BindDictionaryData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), sysField.Name);
                        controlTypeEnum = ControlTypeEnum.ComboBox;
                    }
                    else if (fieldType == typeof(Boolean) || fieldType == typeof(Boolean?))
                    {
                        controlTypeEnum = ControlTypeEnum.SingleCheckBox;
                    }
                    else
                    {
                        controlTypeEnum = ControlTypeEnum.TextBox;
                    }
                }
                #endregion
                switch (controlTypeEnum)
                {
                    case ControlTypeEnum.RichTextBox: //富文本框
                        {
                            #region 富文本框
                            if (!isSearchForm)
                            {
                                sb.Append(" <div style=\"width: 100%;\"><script id=\"" + sysField.Name + "\" name=\"" + sysField.Name + "\" type=\"text/plain\">" + value.ObjToStr() + "</script><script type=\"text/javascript\">$(function () {var ud = UE.getEditor('" + sysField.Name + "');$('#edui224_body').hide();});</script></div>");
                            }
                            else
                            {
                                sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" class=\"easyui-textbox\" value=\"{1}\" style=\"width:{2};\"/>", sysField.Name, value.ObjToStr(), inputWidthStr);
                            }
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.TextAreaBox: //文本域
                    case ControlTypeEnum.TextBox: //文本框
                        {
                            #region 文本框、文本域
                            string linkFieldsStr = string.Empty;
                            if (!isSearchForm) //非搜索表单
                            {
                                if (field.ControlTypeOfEnum == ControlTypeEnum.TextAreaBox)
                                {
                                    inputOptions += ",multiline:true";
                                }
                                if (!string.IsNullOrEmpty(field.BeforeIcon))
                                {
                                    inputOptions += string.Format(",iconAlign:'left',iconCls:'{0}'", field.BeforeIcon);
                                }
                                if (!string.IsNullOrWhiteSpace(linkFields))
                                {
                                    linkFieldsStr = string.Format("linkFields=\"{0}\"", linkFields);
                                }
                            }
                            inputOptions += "\"";
                            sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" noforein=\"1\" class=\"easyui-textbox\" value=\"{1}\" style=\"width:{2};\" {3} {4}/>",
                                sysField.Name, value.ObjToStr(), inputWidthStr, inputOptions, linkFieldsStr);
                            if (module.Name == "表单字段" && sysField.Name == "UrlOrData")
                            {
                                //添加一个添加数据源的按钮
                                int w = 0;
                                int h = 0;
                                int editMode = GetEditMode(module, form, out w, out h, currUser != null ? currUser.UserId : (Guid?)null);
                                string btn = string.Format("<a id=\"btn_addSource\" pmode=\"{0}\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-cog\" onclick=\"AddDataSource(this)\" plain=\"true\"></a>", editMode.ToString());
                                sb.Append(btn);
                            }
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.DialogGrid: //弹出列表框
                    case ControlTypeEnum.DialogTree: //弹出选择树
                        {
                            #region 弹出列表
                            string fieldName = sysField.Name; //字段名
                            Guid foreignModuleId = Guid.Empty; //外键Id
                            string foreignModuleDisplay = foreignModuleName;
                            Sys_Module foreignModule = null; //外键模块
                            bool isMutiSelect = field.IsMultiSelect == true && SystemOperate.GetFieldType(module.Id, fieldName) == typeof(String);
                            if (!string.IsNullOrEmpty(foreignModuleName)) //外键字段
                            {
                                foreignModule = SystemOperate.GetModuleByName(foreignModuleName);
                            }
                            if (foreignModule != null) //当前字段是外键字段
                            {
                                if (!string.IsNullOrEmpty(foreignModule.Display))
                                    foreignModuleDisplay = foreignModule.Display;
                                //先取当前模块冗余字段值，当冗余字段不存在时从外键模块中取
                                foreignModuleId = foreignModule.Id;
                                if (string.IsNullOrEmpty(fieldUrl))
                                {
                                    fieldUrl = field.ControlTypeOfEnum == ControlTypeEnum.DialogGrid ? string.Format("/Page/Grid.html?page=fdGrid&moduleId={0}&initModule={1}&initField={2}", foreignModuleId.ToString(), HttpUtility.UrlEncode(module.Name), fieldName) :
                                              string.Format("/Page/DialogTree.html?moduleName={0}", HttpUtility.UrlEncode(foreignModuleName));
                                    if (isMutiSelect) fieldUrl += "&ms=1";
                                }
                                if (string.IsNullOrEmpty(valueField)) valueField = "Id";
                                if (string.IsNullOrEmpty(textField)) textField = SystemOperate.GetModuleTitleKey(foreignModuleId);
                            }
                            else //非外键字段
                            {
                                if (string.IsNullOrEmpty(valueField)) valueField = "Id";
                                if (string.IsNullOrEmpty(textField)) textField = "Name";
                            }
                            string fieldAttr = string.Format("url=\"{0}\" valueField=\"{1}\" textField=\"{2}\" foreignModuleId=\"{3}\" foreignModuleName=\"{4}\" foreignModuleDisplay=\"{5}\"", fieldUrl, valueField, textField, foreignModuleId.ToString(), foreignModuleName, foreignModuleDisplay);
                            if (isMutiSelect) fieldAttr += " ms=\"1\"";
                            if (field.ControlTypeOfEnum == ControlTypeEnum.DialogTree)
                                fieldAttr += " isTree=\"1\"";
                            inputOptions += ",icons: [{iconCls:'eu-icon-search',handler: function(e){SelectDialogData($(e.data.target))}}]";
                            if (!isSearchForm && SystemOperate.IsDetailModule(moduleId) && SystemOperate.GetParentModule(moduleId).Name == foreignModuleName)
                            {
                                if (!inputOptions.Contains("disabled:true"))
                                    inputOptions += ",disabled:true\"";
                                if (field.IsAllowAdd.HasValue && field.IsAllowAdd.Value)
                                {
                                    fieldAttr += " isParentForeignField=\"1\""; //添加父模块外键字段标识
                                }
                            }
                            inputOptions += "\"";
                            sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" foreignField=\"1\" class=\"easyui-textbox\" value=\"{1}\" textValue=\"{5}\" style=\"width:{2};\" {3} {4}/>",
                               fieldName, value.ObjToStr(), inputWidthStr, fieldAttr, inputOptions, textValue);
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.DateBox: //日期
                    case ControlTypeEnum.DateTimeBox: //日期时间型
                        {
                            #region 日期、时间
                            string dateClass = "easyui-datebox";
                            if (field.ControlTypeOfEnum == ControlTypeEnum.DateTimeBox)
                                dateClass = "easyui-datetimebox";
                            inputOptions += "\"";
                            sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" class=\"{1}\" style=\"width:{2};\" value=\"{3}\" {4}/>",
                                sysField.Name, dateClass, inputWidthStr, value.ObjToStr(), inputOptions);
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.NumberBox: //数值输入框
                    case ControlTypeEnum.IntegerBox: //整型输入框
                        {
                            #region 整形、数值
                            string tempOption = string.Format("precision:{0},groupSeparator:','", field.ControlTypeOfEnum == ControlTypeEnum.IntegerBox ? "0" : (sysField.Precision.HasValue && sysField.Precision.Value > 0 ? sysField.Precision.Value.ToString() : "2"));
                            if (field.MinValue.HasValue)
                            {
                                tempOption += string.Format(",min:{0}", field.MinValue.Value.ToString());
                            }
                            if (field.MaxValue.HasValue)
                            {
                                tempOption += string.Format(",max:{0}", field.MaxValue.Value.ToString());
                            }
                            inputOptions += string.Format(",{0}\"", tempOption);
                            sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" style=\"width:{1};\" class=\"easyui-numberbox\" value=\"{2}\" {3}/>",
                                sysField.Name, inputWidthStr, value.ObjToStr(), inputOptions);
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.SingleCheckBox: //单选checkbox
                        {
                            #region 单选checkbox框
                            if (!isSearchForm) //正常表单
                            {
                                string v = value == null ? string.Empty : (value.GetType() == typeof(Boolean) ? (value.ObjToBool() ? "1" : "0") : value.ObjToStr());
                                string checkStr = v == "1" ? "checked=\"checked\"" : string.Empty;
                                if (isDisabled)
                                    checkStr += " disabled=\"disabled\"";
                                string onchangeStr = "onchange=\"function(){if(typeof(OnFieldValueChanged)=='function'){OnFieldValueChanged({moduleId:'" + moduleId.ToString() + "',moduleName:'" + module.Name + "'},'" + sysField.Name + "');}}\"";
                                sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" singleChk=\"1\" type=\"checkbox\" value=\"{1}\" {2} {3}/>",
                                    sysField.Name, v, checkStr, onchangeStr);
                            }
                            else //搜索表单
                            {
                                sb.AppendFormat("<select id=\"{0}\" class=\"easyui-combobox\" name=\"{0}\" style=\"width:{1};\" data-options=\"editable:false\">", sysField.Name, inputWidthStr);
                                sb.Append("<option value=\"\">所有</option>");
                                sb.Append("<option value=\"1\">是</option>");
                                sb.Append("<option value=\"0\">否</option>");
                                sb.Append("</select>");
                            }
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.MutiCheckBox: //多选checkbox
                        {
                            #region 多选checkbox框
                            if (!isSearchForm)
                            {
                                bool isAllowEdit = model != null ? field.IsAllowEdit.HasValue || field.IsAllowEdit.Value :
                                                  field.IsAllowAdd.HasValue && field.IsAllowAdd.Value;
                                string html = MvcExtensions.CreateMutiCheckbox(sysField.Name, field.TextField, field.ValueField, value.ObjToStr(), isAllowEdit);
                                sb.Append(html);
                            }
                            else
                            {
                                sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" noforein=\"1\" class=\"easyui-textbox\" style=\"width:{1};\" />", sysField.Name, inputWidthStr);
                            }
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.RadioList: //单选框组
                        {
                            #region 单选框组
                            if (!isSearchForm)
                            {
                                bool isAllowEdit = model != null ? field.IsAllowEdit.HasValue || field.IsAllowEdit.Value :
                                                  field.IsAllowAdd.HasValue && field.IsAllowAdd.Value;
                                string html = MvcExtensions.CreateMutiRadioButton(sysField.Name, field.TextField, field.ValueField, value.ObjToStr(), isAllowEdit);
                                sb.Append(html);
                            }
                            else
                            {
                                sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" noforein=\"1\" class=\"easyui-textbox\" style=\"width:{1};\" />", sysField.Name, inputWidthStr);
                            }
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.ComboBox: //下拉框
                    case ControlTypeEnum.ComboTree: //下拉树
                        {
                            #region 下拉框/下拉树
                            Sys_Module foreignModule = SystemOperate.GetModuleByName(foreignModuleName);
                            string className = "easyui-combobox";
                            string loadFilterElseStr = string.Empty;
                            string isTreeStr = "false";
                            if (field.ControlTypeOfEnum == ControlTypeEnum.ComboTree)
                            {
                                className = "easyui-combotree";
                                isTreeStr = "true";
                                if (string.IsNullOrEmpty(valueField)) valueField = "id";
                                if (string.IsNullOrEmpty(textField)) textField = "text";
                                if (fieldUrl == null && foreignModule != null)
                                {
                                    fieldUrl = string.Format("/{0}/GetTreeByNode.html?moduleId={1}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, foreignModule.Id.ToString());
                                }
                                loadFilterElseStr = " else {if(typeof (data)== 'string'){var tempData=eval('(' + data + ')');return tempData;} else{var arr=[];arr.push(data);return arr;}}";
                                inputOptions += ",delay:300,editable:true,keyHandler:{query:function(q){QueryComboTree(q, '" + sysField.Name + "');}}";
                                inputOptions += ",panelMinWidth:200";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(valueField)) valueField = "Id";
                                if (string.IsNullOrEmpty(textField))
                                {
                                    string foreignTitleKey = foreignModule == null ? null : SystemOperate.GetModuleTitleKey(foreignModule.Id);
                                    textField = string.IsNullOrWhiteSpace(foreignTitleKey) ? "Name" : foreignTitleKey;
                                }
                                loadFilterElseStr = " else {if(typeof (data)== 'string'){var tempData=eval('(' + data + ')');return tempData;} else{return data;}}";
                                inputOptions += ",filter:function(q, row){var opts = $(this).combobox('options');return row[opts.textField].indexOf(q)>-1;}";
                                inputOptions += ",panelMinWidth:150";
                                if (SystemOperate.IsEnumField(moduleId, sysField.Name) ||
                                    SystemOperate.IsDictionaryBindField(moduleId, sysField.Name))
                                    inputOptions += ",editable:false";
                            }
                            if (fieldUrl != null && fieldUrl.StartsWith("[{") && fieldUrl.EndsWith("}]")) //下拉数据固定data
                            {
                                inputOptions += string.Format(",valueField:'{0}',textField:'{1}',data:{2}", valueField, textField, fieldUrl);
                            }
                            else //从URL取数据
                            {
                                inputOptions += string.Format(",valueField:'{0}',textField:'{1}',url:'{2}'", valueField, textField, fieldUrl.ObjToStr());
                            }
                            inputOptions += ",onLoadSuccess:function(){if(typeof(OnFieldLoadSuccess)=='function'){ OnFieldLoadSuccess('" + sysField.Name + "','" + valueField + "','" + textField + "'," + isTreeStr + ");}}";
                            inputOptions += ",loadFilter:function(data,parent){if(typeof(OnLoadFilter)=='function'){return OnLoadFilter('" + sysField.Name + "','" + valueField + "','" + textField + "',data,parent," + isTreeStr + ");}" + loadFilterElseStr + "}";
                            if (!isSearchForm)
                            {
                                inputOptions += ",onSelect:function(record){if(typeof(OnFieldSelect)=='function'){OnFieldSelect(record,'" + sysField.Name + "','" + valueField + "','" + textField + "');}}";
                                inputOptions += ",onLoadError:function(arguments){if(typeof(OnLoadError)=='function'){OnLoadError('" + sysField.Name + "','" + valueField + "','" + textField + "',arguments);}}";
                            }
                            if (field.IsMultiSelect.HasValue && field.IsMultiSelect.Value)
                            {
                                inputOptions += ",multiple:true";
                            }
                            inputOptions += "\"";
                            sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" style=\"width:{1};\" class=\"{4}\" value=\"{2}\" {3}/>",
                                 sysField.Name, inputWidthStr, value.ObjToStr(), inputOptions, className);
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.IconBox: //图标控件
                        {
                            #region 图标控件
                            if (isSearchForm)
                            {
                                sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" class=\"easyui-textbox\" value=\"{1}\" style=\"width:{2};\" />",
                                sysField.Name, value.ObjToStr(), inputWidthStr);
                            }
                            else
                            {
                                sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" type=\"hidden\" value=\"{1}\" />", sysField.Name, value.ObjToStr());
                                string iconUrl = SystemOperate.GetIconUrl(value.ObjToStr());
                                if (string.IsNullOrWhiteSpace(iconUrl))
                                {
                                    iconUrl = "/Scripts/jquery-easyui/themes/icons/large_picture.png";
                                }
                                sb.AppendFormat("<div name=\"divImg\" style=\"width:100%;height:100%;border:1px solid #95b8e7;\"><img id=\"img_{0}\" style=\"height:{2}px;border:0px none;\" src=\"{1}\" />", sysField.Name, iconUrl, ConstDefine.FORM_CONTROL_HEIGHT.ToString());
                                sb.AppendFormat("<a style=\"float:right;\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-search\" onclick=\"SelectIcon(this)\" plain=\"true\" iconControlId=\"{0}\">选择图标</a>", sysField.Name);
                                sb.Append("</div>");
                            }
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.ImageUpload: //图片上传控件
                        {
                            #region 图片上传控件
                            if (!isSearchForm)
                            {
                                sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" type=\"hidden\" value=\"{1}\" />", sysField.Name, value.ObjToStr());
                                string iconPath = value.ObjToStr();
                                if (string.IsNullOrWhiteSpace(iconPath))
                                {
                                    iconPath = "/Scripts/jquery-easyui/themes/icons/large_picture.png";
                                }
                                sb.AppendFormat("<div name=\"divImg\" style=\"width:100%;height:100%;border:1px solid #95b8e7;\"><img id=\"img_{0}\" style=\"height:100%;border:0px none;\" src=\"{1}\" />", sysField.Name, iconPath);
                                sb.AppendFormat("<iframe id=\"iframe_{0}\" src=\"/Page/ImgUploadForm.html?\" style=\"display:none\" onload=\"ImgUploadIframeLoaded('{0}')\"></iframe>", sysField.Name);
                                sb.AppendFormat("<a id=\"a_{0}\" style=\"float:right;\" class=\"easyui-linkbutton\" data-options=\"disabled:true\" iconCls=\"eu-icon-search\" onclick=\"SelectImage(this)\" plain=\"true\" imgControlId=\"{0}\">选择图片</a>", sysField.Name);
                                sb.Append("</div>");
                            }
                            #endregion
                        }
                        break;
                    case ControlTypeEnum.LabelBox:
                        {
                            #region Label控件
                            if (isCache.HasValue && isCache.Value) //启用缓存
                            {
                                textValue = "{" + sysField.Name + "_textValue}";
                            }
                            else if (string.IsNullOrEmpty(textValue))
                            {
                                textValue = model == null ? string.Empty : SystemOperate.GetFieldDisplayValue(module.Id, model, field);
                            }
                            sb.AppendFormat("<span fieldSpan=\"0\" style=\"word-break:normal;width:auto;display:block;white-space:pre-wrap;word-wrap:break-word;overflow:hidden;\" id=\"{0}\" value=\"{1}\">{2}</span>", sysField.Name, value.ObjToStr(), textValue);
                            #endregion
                        }
                        break;
                }
            }
            else
            {
                if (isCache.HasValue && isCache.Value) //启用缓存
                {
                    textValue = "{" + sysField.Name + "_textValue}";
                }
                else if (string.IsNullOrEmpty(textValue))
                {
                    textValue = model == null ? string.Empty : SystemOperate.GetFieldDisplayValue(module.Id, model, field);
                }
                sb.AppendFormat("<span fieldSpan=\"1\" style=\"word-break:normal;width:auto;display:block;white-space:pre-wrap;word-wrap:break-word;overflow:hidden;\" id=\"{0}\" value=\"{1}\">{2}</span>", sysField.Name, value.ObjToStr(), textValue);
            }
            #endregion
            #endregion

            return sb.ToString();
        }

        /// <summary>
        /// 附件信息列表HTML
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="id">记录Id</param>
        /// <param name="formType">表单类型</param>
        /// <returns></returns>
        public static string GetAttachmentListHTML(Sys_Module module, Guid? id, FormTypeEnum formType)
        {
            if (!module.IsEnableAttachment) //未启用附件时返回空
            {
                return string.Empty;
            }
            string errMsg = string.Empty;
            //取附件数据
            List<Sys_Attachment> attachList = new List<Sys_Attachment>();
            string attachFileJson = string.Empty;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                bool isEnableCache = ModelConfigHelper.IsModelEnableMemeryCache(typeof(Sys_Attachment));
                List<Sys_Attachment> tempAttachList = CommonOperate.GetEntities<Sys_Attachment>(out errMsg, x => x.Sys_ModuleId == module.Id && x.RecordId == id.Value, string.Empty, false, new List<string>() { "Id" }, new List<bool>() { false });
                if (tempAttachList != null && tempAttachList.Count > 0)
                {
                    if (!isEnableCache)
                        attachList = tempAttachList;
                    List<AttachFileInfo> attachFileInfos = tempAttachList.Select(x => new AttachFileInfo()
                    {
                        Id = x.Id.ToString(),
                        AttachFile = string.Format("~/{0}", x.FileUrl),
                        PdfFile = string.Format("~/{0}", x.PdfUrl),
                        SwfFile = string.Format("~/{0}", x.SwfUrl),
                        FileName = x.FileName,
                        FileType = x.FileType,
                        FileSize = x.FileSize,
                        FileDes = x.FileDes
                    }).ToList();
                    attachFileJson = HttpUtility.UrlEncode(JsonHelper.Serialize(attachFileInfos).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20");
                    foreach (Sys_Attachment attach in tempAttachList)
                    {
                        if (isEnableCache)
                        {
                            Sys_Attachment tempAttach = new Sys_Attachment();
                            ObjectHelper.CopyValue<Sys_Attachment>(attach, tempAttach);
                            tempAttach.FileUrl = string.Format("{0}{1}", Globals.GetBaseUrl(), attach.FileUrl);
                            tempAttach.PdfUrl = string.IsNullOrEmpty(attach.PdfUrl) ? string.Empty : string.Format("{0}{1}", Globals.GetBaseUrl(), attach.PdfUrl);
                            tempAttach.SwfUrl = string.IsNullOrEmpty(attach.SwfUrl) ? string.Empty : string.Format("{0}{1}", Globals.GetBaseUrl(), attach.SwfUrl);
                            attachList.Add(tempAttach);
                        }
                        else
                        {
                            attach.FileUrl = string.Format("{0}{1}", Globals.GetBaseUrl(), attach.FileUrl);
                            attach.PdfUrl = string.IsNullOrEmpty(attach.PdfUrl) ? string.Empty : string.Format("{0}{1}", Globals.GetBaseUrl(), attach.PdfUrl);
                            attach.SwfUrl = string.IsNullOrEmpty(attach.SwfUrl) ? string.Empty : string.Format("{0}{1}", Globals.GetBaseUrl(), attach.SwfUrl);
                        }
                    }
                }
            }
            //组装html
            StringBuilder sb = new StringBuilder();
            string attachmentJs = UIOperate.FormatJsPath("/Scripts/common/Attachment.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", attachmentJs);
            sb.AppendFormat("<input id=\"attachFile\" type=\"hidden\" value=\"{0}\" />", attachFileJson);
            if (module.FormAttachDisplayStyleOfEnum == FormAttachDisplayStyleEnum.GridStype) //列表方式显示
            {
                Guid attachModuleId = SystemOperate.GetModuleIdByName("附件信息");
                List<Sys_GridField> attachGridFields = SystemOperate.GetDefaultGridFields(attachModuleId);
                if (attachGridFields == null) attachGridFields = new List<Sys_GridField>();
                List<string> displayFields = new List<string>() { "FileName", "FileType", "FileSize", "CreateUserName", "CreateDate" };
                sb.Append("<div id=\"attachPanel\" class=\"easyui-panel\" title=\"附件信息\" style=\"width:100%;height:200px;padding:5px;\" data-options=\"collapsible:true\">");
                string gridOptions = "pagination:false,idField:'Id',rownumbers:true,collapsible:true,fitColumns:false,singleSelect:true,toolbar:'#attach_toolbar'";
                sb.AppendFormat("<table id=\"attachGrid\" class=\"easyui-datagrid\" data-options=\"{0}\">", gridOptions);
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th data-options=\"title:'ID',field:'Id',checkbox:true\">ID</th>");
                sb.Append("<th data-options=\"title:'操作',field:'ActionField',width:80,align:'center'\">操作</th>");
                //加载网格字段
                foreach (Sys_GridField field in attachGridFields)
                {
                    if (!field.Sys_FieldId.HasValue || !displayFields.Contains(field.Sys_FieldName)) continue;
                    int fieldWidth = field.Width.HasValue ? field.Width.Value : 120;
                    sb.Append("<th data-options=\"title:'" + field.Display + "',field:'" + field.Sys_FieldName + "',width:" + fieldWidth.ToString() + ",align:'" + field.Align.ToString() + "'\">" + field.Display + "</th>");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("<tbody>");
                foreach (Sys_Attachment attchement in attachList)
                {
                    //行操作链接
                    string action = string.Format("<a href=\"javascript:DelAttachRow({0},'{1}')\">删除</a>", attchement.Id.ToString(), attchement.FileName);
                    if (formType == FormTypeEnum.ViewForm) //查看页面
                    {
                        action += string.Format("&nbsp;&nbsp;<a href=\"javascript:DownloadAttach({0})\">下载</a>", attchement.Id.ToString());
                    }
                    //构造链接文件名
                    string tempUrl = attchement.FileUrl;
                    if (!string.IsNullOrWhiteSpace(attchement.SwfUrl))
                        tempUrl = string.Format("/Page/DocView.html?fn={0}&swfUrl={1}", HttpUtility.UrlEncode(attchement.FileName), HttpUtility.UrlEncode(attchement.SwfUrl));
                    string tempFileName = string.Format("<a target='_blank' href='{0}'>{1}</a>", tempUrl, attchement.FileName);
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td>",
                                 attchement.Id.ToString(), action, tempFileName, attchement.FileType, attchement.FileSize, attchement.CreateUserName, attchement.CreateDate.HasValue ? attchement.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty);
                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
                //工具栏
                sb.Append("<div id=\"attach_toolbar\" class=\"toolbar datagrid-toolbar\" style=\"height:27px;\">");
                sb.Append("<a id=\"btnUploadAttach\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-add\" onclick=\"UploadFile(this)\" plain=\"true\" moduleId=\"" + module.Id + "\" moduleName=\"" + module.Name + "\" attachDisplayStyle=\"1\">上传</a>");
                sb.Append("<a id=\"btnDelAttach\" class=\"easyui-linkbutton\" iconCls=\"eu-p2-icon-delete2\" onclick=\"DelAttach()\" plain=\"true\" moduleId=\"" + module.Id + "\" moduleName=\"" + module.Name + "\" attachDisplayStyle=\"1\">删除</a>");
                sb.Append("</div>");
                sb.Append("</div>");
            }
            else //简单方式
            {
                string btnClass = "easyui-linkbutton";
                string panelClass = "easyui-panel";
                string btnOptions = string.Format("moduleId=\"{0}\" moduleName=\"{1}\" attachDisplayStyle=\"0\"", module.Id.ToString(), module.Name);
                string panelOptions = "data-options=\"collapsible:true\"";
                sb.AppendFormat("<a id=\"btnAddAttach\" class=\"{0}\" plain=\"true\" iconCls=\"eu-p2-icon-add_other\" onclick=\"UploadFile(this)\" {1}>上传附件</a>", btnClass, btnOptions);
                sb.AppendFormat("<div class=\"{0}\" title=\"附件信息\" style=\"width:100%;height:74px;\" {1}>", panelClass, panelOptions);
                sb.Append("<div id=\"attachPanel\">");
                if (attachList != null && attachList.Count > 0)
                {
                    foreach (Sys_Attachment attach in attachList)
                    {
                        string tempUrl = attach.FileUrl;
                        if (!string.IsNullOrEmpty(attach.SwfUrl))
                        {
                            tempUrl = string.Format("/Page/DocView.html?fn={0}&swfUrl={1}", HttpUtility.UrlEncode(attach.FileName).Replace("+", "%20"), HttpUtility.UrlEncode(attach.SwfUrl).Replace("+", "%20"));
                        }
                        sb.AppendFormat("<a id='btn_file_{0}' target='_blank' href='{1}'>{2}</a>", attach.Id.ToString(), tempUrl, attach.FileName);
                        string styleStr = formType == FormTypeEnum.EditForm ? " style=\"margin-right:10px;\"" : string.Empty;
                        string btnAttr = string.Empty;
                        btnAttr = string.Format("AttachId=\"{0}\" FileName=\"{1}\"", attach.Id.ToString(), attach.FileName);
                        sb.Append("<a id=\"btn_remove_" + attach.Id.ToString() + "\" " + styleStr + " class=\"" + btnClass + "\" plain=\"true\" iconCls=\"eu-p2-icon-delete2\" onclick=\"DelAttach(this)\" " + btnAttr + "></a>");
                        if (formType == FormTypeEnum.ViewForm)
                        {
                            sb.Append("<a id=\"btn_download_" + attach.Id.ToString() + "\" style=\"margin-right:10px;\" class=\"" + btnClass + "\" plain=\"true\" iconCls=\"eu-icon-arrow_down\" onclick=\"window.open( '/Annex/DownloadAttachment.html?attachId=" + attach.Id + "')\"></a>");
                        }
                    }
                }
                sb.Append("</div>");
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取审批信息和审批意见页面HTML
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="id">记录ID</param>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="isParentTodo">是否为子流程的父待办</param>
        /// <param name="domainPath">域路径</param>
        /// <returns></returns>
        public static string GetApprovalInfoAndOpinionsHtml(Guid moduleId, Guid? id, Guid? todoTaskId, UserInfo currUser, bool isParentTodo = false, string domainPath = null)
        {
            string path = string.IsNullOrEmpty(domainPath) ? "/" : domainPath;
            StringBuilder sb = new StringBuilder();
            bool isDisplayOpinionPanel = todoTaskId.HasValue && todoTaskId.Value != Guid.Empty && BpmOperate.IsCurrentToDoTaskHandler(todoTaskId.Value, currUser);
            List<ApprovalInfo> approvalInfos = new List<ApprovalInfo>();
            string childFlowFlag = string.Empty; //子流程标识
            if (todoTaskId.HasValue && todoTaskId.Value != Guid.Empty)
            {
                if (!BpmOperate.IsChildFlowParentTodo(todoTaskId.Value)) //当前非子流程父待办
                {
                    approvalInfos = BpmOperate.GetRecordApprovalInfosByTodoId(todoTaskId.Value);
                    if (approvalInfos.Count == 0) return sb.ToString();
                }
            }
            else if (id.HasValue && id.Value != Guid.Empty) //查看页面
            {
                if (!isParentTodo) //非子流程父待办ID
                {
                    approvalInfos = BpmOperate.GetModuleRecordApprovalInfos(moduleId, id.Value);
                    if (approvalInfos.Count == 0) return sb.ToString();
                }
                else //子流程父待办ID
                {
                    approvalInfos = new List<ApprovalInfo>();
                    childFlowFlag = " childflow=\"1\"";
                }
            }
            else
            {
                return string.Empty;
            }
            sb.Append("<div id=\"approvalDiv\">");
            sb.AppendFormat("<link href=\"{0}Scripts/Bpm/GooFlow/default.css\" rel=\"stylesheet\" type=\"text/css\" />", path);
            sb.AppendFormat("<link href=\"{0}Scripts/Bpm/GooFlow/GooFlow2.css\" rel=\"stylesheet\" type=\"text/css\" />", path);
            if (id.HasValue && id.Value != Guid.Empty)
            {
                var group = approvalInfos.GroupBy(x => x.NodeName).ToList();
                string mergeCount = string.Empty;
                string mergeIndex = string.Empty;
                foreach (var infos in group)
                {
                    int count = infos.Count();
                    if (count > 1)
                    {
                        if (mergeCount != string.Empty)
                            mergeCount += ",";
                        mergeCount += count;
                        ApprovalInfo info = infos.FirstOrDefault();
                        int tbIndex = approvalInfos.IndexOf(info);
                        if (mergeIndex != string.Empty)
                            mergeIndex += ",";
                        mergeIndex += tbIndex;
                    }
                }
                sb.Append("<div style=\"padding-bottom:2px;margin-top:2px;\">");
                sb.AppendFormat("<div id=\"div_ApprovalList\" mergeCount=\"{0}\" mergeIndex=\"{1}\" class=\"easyui-tabs\" data-options=\"fit:false,border:true,tabHeight:{2}\"{3}>", mergeCount, mergeIndex, ConstDefine.TAB_HEAD_HEIGHT, childFlowFlag);
                sb.Append("<div id=\"div_approvalListTitle\" style=\"padding:5px;\" title=\"处理记录\" data-options=\"iconCls:'eu-icon-approvalok',collapsible:true\">");
                sb.Append("<table id=\"tb_approvalList\" class=\"easyui-datagrid\" data-options=\"onSelect:function(rowIndex, rowData){$('#tb_approvalList').datagrid('unselectAll');}\">");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th data-options=\"field:'NodeName',width:120\">节点名称</th>");
                sb.Append("<th data-options=\"field:'Handler',width:80\">处理人</th>");
                sb.Append("<th data-options=\"field:'HandleResult',width:80\">处理结果</th>");
                sb.Append("<th data-options=\"field:'HandleTime',width:150\">处理时间</th>");
                sb.Append("<th data-options=\"field:'ApprovalOpinions',width:250\">处理意见</th>");
                sb.Append("<th data-options=\"field:'NextNodeName',width:120\">下一节点</th>");
                sb.Append("<th data-options=\"field:'NextHandler',width:150\">下一处理人</th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("<tbody>");
                if (!isParentTodo) //不是子流程父待办
                {
                    foreach (var info in approvalInfos)
                    {
                        sb.Append("<tr>");
                        sb.AppendFormat("<td>{0}</td>", info.NodeName);
                        sb.AppendFormat("<td>{0}</td>", info.Handler);
                        sb.AppendFormat("<td>{0}</td>", info.HandleResult);
                        sb.AppendFormat("<td>{0}</td>", info.HandleTime);
                        sb.AppendFormat("<td><span title=\"{0}\">{0}</span></td>", info.ApprovalOpinions);
                        sb.AppendFormat("<td>{0}</td>", info.NextNodeName);
                        sb.AppendFormat("<td><span title=\"{0}\">{0}</span></td>", info.NextHandler);
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
                sb.Append("</div>");
                sb.Append("<div style=\"padding:5px;\" title=\"流程轨迹\" data-options=\"iconCls:'eu-p2-icon-chart_line',collapsible:true\">");
                sb.Append("<div id=\"gooFlowDom\" style=\"width: 100%; height: 100%; border-width: 0px;\"></div>");
                sb.Append("</div>");
                sb.Append("</div>");
                sb.Append("</div>");
            }
            if (isDisplayOpinionPanel)
            {
                Bpm_WorkNode currNode = BpmOperate.GetCurrentApprovalNode(todoTaskId.Value);
                sb.Append("<div style=\"padding-bottom:2px;\">");
                sb.Append("<div class=\"easyui-panel\" title=\"处理及意见\" style=\"padding-bottom:5px;\" data-options=\"iconCls:'eu-icon-edit',collapsible:true\">");
                sb.Append("<table style=\"line-height:36px;\">");
                sb.Append("<tr>");
                sb.Append("<td style=\"width: 100px;\">");
                sb.Append("<span style=\"margin-left: 13px;\">处理节点：</span>");
                sb.Append("</td>");
                string nodeDisplay = currNode != null ? (string.IsNullOrEmpty(currNode.DisplayName) ? currNode.Name : currNode.DisplayName) : string.Empty;
                sb.AppendFormat("<td id=\"handleNode\" style=\"width: 200px;\">{0}</td>", nodeDisplay);
                sb.Append("<td style=\"width: 100px;\">");
                sb.Append("<span>处理人员：</span>");
                sb.Append("</td>");
                sb.AppendFormat("<td style=\"width: 200px;\">{0}</td>", currUser != null ? currUser.EmpName : string.Empty);
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style=\"width: 100px;\">");
                sb.Append("<span style=\"margin-left: 13px;\">处理意见：</span>");
                sb.Append("</td>");
                sb.Append("<td colspan=\"3\">");
                sb.Append("<input id=\"txt_approvalOpinions\" class=\"easyui-textbox\" data-options=\"multiline:true,prompt:'请填写处理意见！'\" style=\"width:600px;height:120px\" />");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</div>");
                sb.Append("</div>");
            }
            else
            {
                sb.Append("<div style=\"padding-bottom:2px\">&nbsp;</div>");
            }
            sb.Append("</div>");
            string approvalInfor = WebHelper.GetJsModifyTimeStr("/Scripts/Bpm/ApprovalInfo.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/Bpm/ApprovalInfo.js?r={1}\"></script>", path, approvalInfor);
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/Bpm/GooFlow/GooFunc.js\"></script>", path);
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/Bpm/GooFlow/GooFlow.js\"></script>", path);
            string flowCanvasr = WebHelper.GetJsModifyTimeStr("/Scripts/Bpm/FlowCanvas.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/Bpm/FlowCanvas.js?r={1}\"></script>", path, flowCanvasr);
            return sb.ToString();
        }

        /// <summary>
        /// 获取表单按钮HTML
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="buttons">按钮集合</param>
        /// <param name="editMode">编辑模式</param>
        /// <param name="gridId">网格domid</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="id">记录ID</param>
        /// <param name="titleKeyValue">titleKeyValue</param>
        /// <returns></returns>
        public static string GetFormButtonHTML(Sys_Module module, List<FormButton> buttons, int editMode, string gridId, UserInfo currUser, Guid? id = null, string titleKeyValue = null)
        {
            StringBuilder formBtnSb = new StringBuilder();
            if (buttons != null && buttons.Count > 0)
            {
                string btnCls = string.Empty;
                string btnWidth = "90%";
                if (editMode == (int)ModuleEditModeEnum.PopFormEdit)
                {
                    btnWidth = "100%";
                }
                formBtnSb.AppendFormat("<div id=\"divFormBtn\" class=\"noprint\" style=\"width:{0};height:30px;line-height:30px;text-align:center;margin-left:-3px;margin-top:8px;\">", btnWidth);
                int no = 0;
                foreach (FormButton btn in buttons)
                {
                    if (editMode == (int)ModuleEditModeEnum.GridRowBottomFormEdit && (btn.DisplayText == "保存并新增" || btn.DisplayText == "关闭"))
                        continue;
                    no++;
                    string tagId = string.Format("btn_{0}", new Random().Next(100).ToString());
                    if (!string.IsNullOrEmpty(btn.TagId))
                    {
                        tagId = btn.TagId;
                    }
                    string icon = btn.Icon;
                    switch (btn.IconType)
                    {
                        case ButtonIconType.Edit:
                            if (btn.DisplayText == "编辑" && id.HasValue && id.Value != Guid.Empty && currUser != null)
                            {
                                bool canEdit = PermissionOperate.UserHasOperateRecordPermission(currUser.UserId, module.Id, id.Value, DataPermissionTypeEnum.EditData);
                                if (!canEdit) continue;
                            }
                            icon = "eu-icon-edit";
                            break;
                        case ButtonIconType.Close:
                            icon = "eu-icon-close";
                            break;
                    }
                    string c = string.Empty;
                    if (GlobalSet.IsShowStyleBtn)
                        c = string.Format(" c{0}", no % 9);
                    formBtnSb.Append("<a id=\"" + tagId + "\" class=\"easyui-linkbutton" + c + "\" style=\"margin-right:5px;\" iconCls=\"" + icon + "\" onclick=\"" + (string.IsNullOrEmpty(btn.ClickMethod) ? string.Empty : btn.ClickMethod) + "\"");
                    if (btn.IconType == ButtonIconType.DraftRelease) //草稿发布按钮
                    {
                        formBtnSb.Append(" release=\"1\"");
                    }
                    else if (btn.IconType == ButtonIconType.FlowAgree) //同意
                    {
                        formBtnSb.Append(" flowflag=\"1\"");
                    }
                    else if (btn.IconType == ButtonIconType.FlowReject) //拒绝
                    {
                        formBtnSb.Append(" flowflag=\"2\" nosave=\"1\"");
                    }
                    else if (btn.IconType == ButtonIconType.FlowReturn) //回退
                    {
                        formBtnSb.Append(" flowflag=\"3\" nosave=\"1\"");
                    }
                    else if (btn.IconType == ButtonIconType.FlowDirect) //指派
                    {
                        formBtnSb.Append(" flowflag=\"4\" nosave=\"1\"");
                    }
                    formBtnSb.Append(" detail=\"" + (module.ParentId.HasValue && module.ParentId.Value != Guid.Empty ? "true" : "false") + "\" editMode=\"" + editMode.ToString() + "\" moduleId=\"" + module.Id.ToString() + "\" moduleName=\"" + module.Name + "\" gridId=\"" + (string.IsNullOrEmpty(gridId) ? string.Empty : gridId) + "\"");
                    if (!string.IsNullOrEmpty(titleKeyValue))
                        formBtnSb.Append(" titleKeyValue=\"" + titleKeyValue + "\"");
                    if (!string.IsNullOrEmpty(btn.ParentToDoId))
                        formBtnSb.Append(" parentToDoId=\"" + btn.ParentToDoId + "\"");
                    formBtnSb.Append(">" + (string.IsNullOrEmpty(btn.DisplayText) ? string.Empty : btn.DisplayText) + "</a>");
                }
                formBtnSb.Append("</div>");
            }
            return formBtnSb.ToString();
        }

        /// <summary>
        /// 获取表单tooltags的html
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="id">ID</param>
        /// <param name="toolTags">工具标签按钮集合</param>
        /// <returns></returns>
        public static string GetFormToolTagsHTML(Sys_Module module, Guid? id, List<FormToolTag> toolTags)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"div_tags\" class=\"noprint\" style=\"text-align:right;margin-right:5px;\">");
            if (toolTags != null && toolTags.Count > 0 && module != null)
            {
                foreach (FormToolTag tag in toolTags)
                {
                    sb.AppendFormat("<a id=\"{0}\" style=\"margin-right:2px;\" href=\"#\" title=\"{2}\" class=\"easyui-linkbutton easyui-tooltip\" data-options=\"plain:true,iconCls:'{3}'\" onclick=\"{4}\" moduleId=\"{5}\" moduleName=\"{6}\" recordId=\"{7}\">{1}</a>",
                        tag.TagId, tag.Text, string.IsNullOrEmpty(tag.Title) ? tag.Text : tag.Title, tag.Icon, tag.ClickMethod, module.Id.ToString(), module.Name, id.HasValue && id.Value != Guid.Empty ? id.Value.ToString() : string.Empty);
                }
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        #endregion
    }
}

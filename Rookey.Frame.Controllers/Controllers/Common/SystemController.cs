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
using System.Web.Mvc;
using Rookey.Frame.Common;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Operate.Base.OperateHandle;
using System.Text;
using Rookey.Frame.Operate.Base.EnumDef;
using System.Reflection;
using Rookey.Frame.Base;
using System.Web;
using Rookey.Frame.Operate.Base.TempModel;
using System.IO;
using System.Threading.Tasks;
using Rookey.Frame.EntityBase;
using Rookey.Frame.Common.PubDefine;
using Rookey.Frame.Common.Model;
using Rookey.Frame.Base.Set;
using Rookey.Frame.Controllers.Other;
using Rookey.Frame.Model.OrgM;

namespace Rookey.Frame.Controllers.Sys
{
    /// <summary>
    /// 系统模块相关操作控制器（异步）
    /// </summary>
    public class SystemAsyncController : AsyncBaseController
    {
        #region 模块处理

        /// <summary>
        /// 异步加载所有模块
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadModulesAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadModules();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步加载所有模块并将当前模块也添加到其中，
        /// 针对模块添加时添加下拉外键模块
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadModulesExpAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadModulesExp();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步加载非明细模块
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadNotDetailModuleAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadNotDetailModule();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步加载明细模块，参数moduleId或moduleName不为空取该模块的明细模块，否则取所有明细模块
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadDetailModuleAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadDetailModule();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步加载启用了编码规则的模块
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadBillCodeModuleAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadBillCodeModule();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 重新生成模块
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> ReCreateModuleAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).ReCreateModule();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 字段处理

        /// <summary>
        /// 异步获取表单字段信息
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetFormFieldsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).GetFormFields();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步获取单个表单字段
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetFormFieldAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).GetFormField();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步加载视图字段，参数：viewId或moduleId或moduleName,参数为viewId时加载该视图字段，
        /// 参数为moduleId或moduleName时加载该模块的默认视图字段
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadViewFieldsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadViewFields();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #region 注释掉的
        ///// <summary>
        ///// 异步加载网格的所有树显示字段
        ///// 可以是枚举字段、字典字段、字符串字段、外键字段
        ///// </summary>
        ///// <returns></returns>
        //public Task<ActionResult> LoadGridTreeFieldsAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        return new SystemController(Request).LoadGridTreeFields();
        //    }).ContinueWith<ActionResult>(task =>
        //    {
        //        return task.Result;
        //    });
        //}
        #endregion

        /// <summary>
        /// 异步加载字段信息
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadFieldsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadFields();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #region 注释掉的
        ///// <summary>
        ///// 异步加载字符串型字段
        ///// </summary>
        ///// <returns></returns>
        //public Task<ActionResult> LoadStringFieldsAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        return new SystemController(Request).LoadStringFields();
        //    }).ContinueWith<ActionResult>(task =>
        //    {
        //        return task.Result;
        //    });
        //}

        ///// <summary>
        ///// 异步加载视图中不存在的字段信息集合
        ///// </summary>
        ///// <returns></returns>
        //public Task<ActionResult> LoadFieldsNotInViewAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        return new SystemController(Request).LoadFieldsNotInView();
        //    }).ContinueWith<ActionResult>(task =>
        //    {
        //        return task.Result;
        //    });
        //}

        ///// <summary>
        ///// 异步加载视图中存在的字段信息集合
        ///// </summary>
        ///// <returns></returns>
        //public Task<ActionResult> LoadFieldsInViewAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        return new SystemController(Request).LoadFieldsInView();
        //    }).ContinueWith<ActionResult>(task =>
        //    {
        //        return task.Result;
        //    });
        //}

        ///// <summary>
        ///// 异步加载表单中不存在的字段信息集合
        ///// </summary>
        ///// <returns></returns>
        //public Task<ActionResult> LoadFieldsNotInFormAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        return new SystemController(Request).LoadFieldsNotInForm();
        //    }).ContinueWith<ActionResult>(task =>
        //    {
        //        return task.Result;
        //    });
        //}

        ///// <summary>
        ///// 异步加载表单中存在的字段信息集合
        ///// </summary>
        ///// <returns></returns>
        //public Task<ActionResult> LoadFieldsInFormAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        return new SystemController(Request).LoadFieldsInForm();
        //    }).ContinueWith<ActionResult>(task =>
        //    {
        //        return task.Result;
        //    });
        //}

        ///// <summary>
        ///// 异步加载数据源字段，下拉字段或弹出框字段
        ///// </summary>
        ///// <returns></returns>
        //public Task<ActionResult> LoadComboOrDialogGridFieldsAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        return new SystemController(Request).LoadComboOrDialogGridFields();
        //    }).ContinueWith<ActionResult>(task =>
        //    {
        //        return task.Result;
        //    });
        //}

        ///// <summary>
        ///// 异步加载可绑定字典的字段信息集合
        ///// </summary>
        ///// <returns></returns>
        //public Task<ActionResult> LoadCanBindDictionaryFieldsAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        return new SystemController(Request).LoadCanBindDictionaryFields();
        //    }).ContinueWith<ActionResult>(task =>
        //    {
        //        return task.Result;
        //    });
        //}
        #endregion

        #endregion

        #region 视图处理

        /// <summary>
        /// 异步取用户列表视图，取特定用户视图传userId或username参数，否则取当前用户视图
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadUserGridViewAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadUserGridView();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步切换用户视图，参数viewId
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> ChangeGridViewAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).ChangeGridView();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步保存用户视图
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> SaveGridViewAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).SaveGridView();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步删除视图
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> DelGridViewAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).DelGridView();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步检测视图是否可编辑，系统视图和非自己创建视图不可编辑
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> IsGridViewCanOperateAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).IsGridViewCanOperate();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步加载明细或附属模块数据视图
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadInnerDetailModuleGridAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadInnerDetailModuleGrid();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步保存用户附属模块设置
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> SaveUserAttachModuleSetAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).SaveUserAttachModuleSet();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步加载视图按钮
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadViewButtonsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadViewButtons();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 保存常用按钮设置
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> SaveCommonBtnAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).SaveCommonBtn();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 刷新字段格式化
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> RefreshFieldFormatAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).RefreshFieldFormat();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 表单处理

        /// <summary>
        /// 异步加载角色表单
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadRoleFormsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadRoleForms();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步保存表单
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> SaveFormAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).SaveForm();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步关联角色表单
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> RelateRoleFormAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).RelateRoleForm();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步删除角色表单
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> DelRoleFormAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).DelRoleForm();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 获取模块表单
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetModuleFormsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).GetModuleForms();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 加载表单按钮
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadFormBtnsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadFormBtns();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 数据字典

        /// <summary>
        /// 异步获取字典分类下字典项的最大排序号
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetDicClassMaxSortAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).GetDicClassMaxSort();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 菜单处理

        /// <summary>
        /// 异步加载非叶子节点菜单树
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadFolderMenuTreeAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadFolderMenuTree();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 保存用户快捷菜单
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> SaveUserQuckMenusAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).SaveUserQuckMenus();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 重新加载用户快捷菜单
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> ReloadUserQuckMenusAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).ReloadUserQuckMenus();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 图标处理

        /// <summary>
        /// 异步获取图标分页html
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetIconPageHtmlAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).GetIconPageHtml();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步判断图标样式类名是否已存在
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> IconStyleClassNameIsExistsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).IconStyleClassNameIsExists();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步增加自定义样式类
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> AddCustomerStyleClassAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).AddCustomerStyleClass();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 权限处理

        #region 角色权限

        /// <summary>
        /// 异步保存用户角色
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> SaveUserRoleAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).SaveUserRole();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步加载角色权限
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadRolePermissionAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadRolePermission();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步保存角色权限
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> SaveRolePermissionAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).SaveRolePermission();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 用户权限

        /// <summary>
        /// 异步加载用户权限
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadUserPermissionAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadUserPermission();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步保存用户权限
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> SaveUserPermissionAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).SaveUserPermission();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #endregion

        #region 其他处理

        /// <summary>
        /// 加载查询方法枚举列表
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> LoadQueryMethodEnumListAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).LoadQueryMethodEnumList();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> RefreshCacheAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new SystemController(Request).RefreshCache();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion
    }

    /// <summary>
    /// 系统模块相关操作控制器
    /// </summary>
    public class SystemController : BaseController
    {
        #region 构造函数

        private HttpRequestBase _Request = null; //请求对象

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public SystemController()
        {
            _Request = Request;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request">请求对象</param>
        public SystemController(HttpRequestBase request)
            : base(request)
        {
            _Request = request;
        }

        #endregion

        #region 模块处理

        /// <summary>
        /// 加载所有模块
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadModules()
        {
            List<Sys_Module> list = SystemOperate.GetModules();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 加载所有模块并将当前模块也添加到其中，针对模块添加时添加下拉外键模块
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadModulesExp()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string moduleName = HttpUtility.UrlDecode(_Request["moduleName"].ObjToStr(), Encoding.UTF8);
            string tableName = _Request["tableName"].ObjToStr();
            List<Sys_Module> list = SystemOperate.GetModules();
            list.Add(new Sys_Module() { Name = moduleName, TableName = tableName });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 加载非明细模块
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadNotDetailModule()
        {
            List<Sys_Module> list = SystemOperate.GetModules(x => x.ParentId == null || x.ParentId == Guid.Empty);
            return Json(list);
        }

        /// <summary>
        /// 加载明细模块，参数moduleId或moduleName不为空取该模块的明细模块，否则取所有明细模块
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadDetailModule()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            List<Sys_Module> list = null;
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module != null) //取该模块的明细模块
            {
                list = SystemOperate.GetDetailModules(module.Id);
            }
            else //取所有明细模块
            {
                list = SystemOperate.GetModules(x => x.ParentId != null && x.ParentId != Guid.Empty);
            }
            return Json(list);
        }

        /// <summary>
        /// 加载启用了编码规则的模块
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadBillCodeModule()
        {
            List<Sys_Module> list = SystemOperate.GetModules();
            list = list.Where(x => x.IsEnableCodeRule).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重新生成模块
        /// </summary>
        /// <returns></returns>
        public ActionResult ReCreateModule()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module == null)
                return Json(new ReturnResult { Success = false, Message = "模块不存在无法重新生成！" });
            if (module.DataSourceTypeOfEnum != ModuleDataSourceType.DbTable)
                return Json(new ReturnResult { Success = false, Message = "数据来源类型不是数据表类型的模块不允许重新生成！" });
            if (string.IsNullOrEmpty(module.TableName))
                return Json(new ReturnResult { Success = false, Message = "模块数据表为空无法重新生成！" });
            if (!module.IsCustomerModule)
                return Json(new ReturnResult { Success = false, Message = "只有自定义模块才能重新生成！" });
            string errMsg = string.Empty;
            if (CommonOperate.Count(out errMsg, module.Id, false) > 0)
                return Json(new ReturnResult { Success = false, Message = "重新生成模块前必须先清空数据！" });
            //删除模块引用
            SystemOperate.DeleteModuleReferences(module);
            //删除数据表
            string connString = string.Empty;
            DatabaseType dbTypeEnum = SystemOperate.GetModuleDbType(module, out connString);
            DbLinkArgs dbLinkArgs = ModelConfigHelper.GetDbLinkArgs(connString, dbTypeEnum);
            if (dbLinkArgs == null) dbLinkArgs = ModelConfigHelper.GetLocalDbLinkArgs();
            if (dbLinkArgs == null)
                return Json(new ReturnResult { Success = false, Message = "模块的数据库连接对象为空！" });
            Type modelType = CommonOperate.GetModelType(module.TableName);
            errMsg = CommonOperate.DropTable(modelType, connString, dbTypeEnum);
            if (string.IsNullOrEmpty(errMsg))
            {
                //删除模块信息
                bool rs = CommonOperate.DeleteRecordsByExpression<Sys_Module>(x => x.Id == module.Id, out errMsg);
                if (rs)
                {
                    //生成模块信息
                    errMsg = ToolOperate.RepairTables(new List<string>() { module.TableName });
                }
            }
            return Json(new ReturnResult { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        #endregion

        #region 字段处理

        /// <summary>
        /// 获取表单字段信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFormFields()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module == null) return Json(null);
            bool isLoadSysField = _Request["load_sysfield"].ObjToInt() == 1; //是否加载Sys_Field
            bool isLoadDic = _Request["load_dic"].ObjToInt() == 1; //是否加载字典信息
            List<Sys_FormField> list = SystemOperate.GetDefaultFormField(module.Id);
            List<Sys_FormField> tempList = new List<Sys_FormField>();
            if (list != null && list.Count > 0)
            {
                int controlHideType = (int)ControlTypeEnum.HiddenBox;
                int controlDialogType = (int)ControlTypeEnum.DialogGrid;
                foreach (Sys_FormField field in list)
                {
                    if (field.IsDeleted || field.IsDraft)
                        continue;
                    Sys_Module foreignModule = SystemOperate.GetForeignModule(module.Id, field.Sys_FieldName);
                    if (field.ControlType == controlDialogType && foreignModule != null) //外键Name字段过滤
                        continue;
                    if (field.ControlType == controlHideType && foreignModule == null) //非外键隐藏字段过滤
                        continue;
                    Sys_FormField obj = SystemOperate.FormatFormField(module, field.Sys_FieldName);
                    if (isLoadSysField && field.Sys_FieldId.HasValue)
                    {
                        Sys_Field sysField = SystemOperate.GetFieldById(field.Sys_FieldId.Value);
                        obj.TempSysField = sysField;
                    }
                    if (isLoadDic)
                    {
                        if (SystemOperate.IsEnumField(module.Id, field.Sys_FieldName))
                        {
                            obj.TempEnums = SystemOperate.EnumFieldDataJson(module.Id, field.Sys_FieldName);
                        }
                        else if (SystemOperate.IsDictionaryBindField(module.Id, field.Sys_FieldName))
                        {
                            obj.TempDics = SystemOperate.DictionaryDataJson(module.Id, field.Sys_FieldName);
                        }
                    }
                    tempList.Add(obj);
                }
            }
            return Json(tempList);
        }

        /// <summary>
        /// 获取单个表单字段
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFormField()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            string fieldName = _Request["fieldName"].ObjToStr();
            if (module == null || string.IsNullOrWhiteSpace(fieldName))
                return Json(null);
            Sys_FormField obj = SystemOperate.FormatFormField(module, fieldName);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 加载视图字段，参数：viewId或moduleId或moduleName,参数为viewId时加载该视图字段，
        /// 参数为moduleId或moduleName时加载该模块的默认视图字段
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadViewFields()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid viewId = _Request["viewId"].ObjToGuid();
            int flag = _Request["flag"].ObjToInt(); //标识
            List<Sys_GridField> list = new List<Sys_GridField>();
            Sys_Module module = null;
            if (viewId == Guid.Empty)
            {
                module = SystemOperate.GetModuleByRequest(_Request);
                list = flag > 0 ? SystemOperate.GetDefaultGridFields(module.Id) : SystemOperate.GetDefaultGridFields(module.Id, false);
            }
            else
            {
                module = SystemOperate.GetModuleByViewId(viewId);
                list = flag > 0 ? SystemOperate.GetGridFields(viewId) : SystemOperate.GetGridFields(viewId, false);
            }
            if (module == null) return Json(null);
            if (list == null) list = new List<Sys_GridField>();
            list = list.Where(x => x.Sys_FieldName != "Id").ToList();
            switch (flag)
            {
                case 1: //返回可见字段
                    list = list.Where(x => x.IsVisible).ToList();
                    break;
                case 2: //不包含外键Name字段
                    list = list.Where(x => !SystemOperate.IsForeignNameField(module.Id, x.Sys_FieldName)).ToList();
                    break;
                case 3: //加载网格的所有树显示字段，可以是枚举字段、字典字段、字符串字段、外键字段
                    {
                        #region 加载网格的所有树显示字段
                        list = list.Where(x => x.IsVisible).ToList();
                        List<object> tempList = new List<object>();
                        Type modelType = CommonOperate.GetModelType(module.Id);
                        foreach (Sys_GridField gridField in list)
                        {
                            bool isForeignNameField = SystemOperate.IsForeignNameField(module.Id, gridField.Sys_FieldName);
                            if (SystemOperate.IsEnumField(modelType, gridField.Sys_FieldName) ||
                                SystemOperate.IsDictionaryBindField(module.Id, gridField.Sys_FieldName) ||
                                SystemOperate.GetFieldType(module.Id, gridField.Sys_FieldName) == typeof(String) ||
                                isForeignNameField)
                            {
                                string fieldName = gridField.Sys_FieldName;
                                if (isForeignNameField)
                                {
                                    fieldName = fieldName.Substring(0, fieldName.Length - 4) + "Id";
                                }
                                tempList.Add(new { FieldName = fieldName, Display = gridField.Display });
                            }
                        }
                        return Json(tempList, JsonRequestBehavior.AllowGet);
                        #endregion
                    }
                default: //默认返回所有字段
                    break;
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 加载字段信息
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadFields()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            int flag = _Request["flag"].ObjToInt(); //标识
            List<Sys_Field> list = new List<Sys_Field>();
            Guid viewId = _Request["viewId"].ObjToGuid(); //视图Id
            Guid formId = _Request["formId"].ObjToGuid(); //表单Id
            Sys_Module module = null;
            if (viewId != Guid.Empty)
            {
                Sys_Grid grid = SystemOperate.GetGrid(viewId);
                if (grid == null || !grid.Sys_ModuleId.HasValue) return Json(null);
                List<Sys_GridField> gridFields = SystemOperate.GetGridFields(grid, false);
                List<string> gridFieldNames = new List<string>();
                if (gridFields != null) gridFieldNames = gridFields.Select(x => x.Sys_FieldName).ToList();
                list = SystemOperate.GetFieldInfos(grid.Sys_ModuleId.Value);
                if (flag == 4)
                    list = list.Where(x => !gridFieldNames.Contains(x.Name)).ToList();
                else if (flag == 5)
                    list = list.Where(x => gridFieldNames.Contains(x.Name)).ToList();
            }
            else if (formId != Guid.Empty)
            {
                Sys_Form form = SystemOperate.GetForm(formId);
                if (form == null || !form.Sys_ModuleId.HasValue) return Json(null);
                List<Sys_FormField> formFields = SystemOperate.GetFormField(formId);
                List<string> formFieldNames = new List<string>();
                if (formFields != null) formFieldNames = formFields.Select(x => x.Sys_FieldName).ToList();
                list = SystemOperate.GetFieldInfos(form.Sys_ModuleId.Value);
                if (flag == 6)
                    list = list.Where(x => !formFieldNames.Contains(x.Name)).ToList();
                else if (flag == 7)
                    list = list.Where(x => formFieldNames.Contains(x.Name)).ToList();
            }
            else
            {
                module = SystemOperate.GetModuleByRequest(_Request);
                if (module == null) return Json(null);
                list = SystemOperate.GetFieldInfos(module.Id);
            }
            switch (flag)
            {
                case 1: //不过滤基类字段
                case 4: //加载视图中不存在的字段信息集合
                case 5: //加载视图中存在的字段信息集合
                case 6: //加载表单中不存在的字段信息集合
                case 7: //加载表单中存在的字段信息集合
                    return Json(list, JsonRequestBehavior.AllowGet);
                case 2: //加载字符串型字段不过滤基类字段
                    list = list.Where(x => SystemOperate.GetFieldType(module.Id, x.Name) == typeof(String)).ToList();
                    break;
                case 3: //加载字符串型字段并过滤基类字段
                    list = list.Where(x => !CommonDefine.BaseEntityFieldsContainId.Contains(x.Name) && SystemOperate.GetFieldType(module.Id, x.Name) == typeof(String)).ToList();
                    break;
                case 8: //加载数据源字段，下拉字段或弹出框字段
                    {
                        List<Sys_FormField> formFields = SystemOperate.GetDefaultFormField(module.Id, false);
                        list = formFields.Where(x => x.Sys_FieldId.HasValue &&
                            (x.ControlType == (int)ControlTypeEnum.ComboBox || x.ControlType == (int)ControlTypeEnum.ComboGrid ||
                            x.ControlType == (int)ControlTypeEnum.ComboTree || x.ControlType == (int)ControlTypeEnum.DialogGrid))
                            .Select(x => SystemOperate.GetFieldById(x.Sys_FieldId.Value)).ToList();
                    }
                    break;
                case 9: //加载可绑定字典的字段信息集合
                    {
                        //实体类型
                        Type modelType = CommonOperate.GetModelType(module.Id);
                        //实体属性集合
                        PropertyInfo[] ps = modelType.GetProperties();
                        List<string> fieldNames = ps.Where(x => x.PropertyType == typeof(String)).Select(x => x.Name).ToList();
                        //过滤
                        list = list.Where(x => fieldNames.Contains(x.Name)).ToList();
                    }
                    break;
                case 10: //加载外键是员工的字段
                    {
                        list = list.Where(x => x.ForeignModuleName == "员工管理").ToList();
                    }
                    break;
                default: //默认过滤创建人，创建时间等基类字段
                    list = list.Where(x => !CommonDefine.BaseEntityFieldsContainId.Contains(x.Name)).ToList();
                    break;
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 视图处理

        /// <summary>
        /// 取用户列表视图，取特定用户视图传userId或username参数，否则取当前用户视图
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadUserGridView()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module == null) return Json(null);
            UserInfo currUser = GetCurrentUser(_Request);
            if (currUser == null) return Json(null);
            Guid userId = _Request["userId"].ObjToGuid();
            if (userId == Guid.Empty)
            {
                string username = _Request["username"].ObjToStr();
                if (!string.IsNullOrEmpty(username))
                {
                    string errMsg = string.Empty;
                    userId = UserOperate.GetUserIdByUserName(username);
                }
                if (userId == Guid.Empty) userId = currUser.UserId;
            }
            List<Sys_Grid> list = SystemOperate.GetUserGrids(userId, module.Id); //用户创建视图
            //管理员admin默认视图
            Sys_Grid adminGrid = SystemOperate.GetAdminDefaultConfigGrid(module.Id);
            if (adminGrid != null)
            {
                if (!list.Select(x => x.Id).Contains(adminGrid.Id))
                    list.Add(adminGrid);
            }
            //系统默认视图
            Sys_Grid sysGrid = SystemOperate.GetDefaultGrid(module.Id);
            if (sysGrid != null)
            {
                if (!list.Select(x => x.Id).Contains(sysGrid.Id))
                    list.Add(sysGrid);
            }
            return Json(list.Select(x => new { Id = x.Id, Name = x.Name }).ToList());
        }

        /// <summary>
        /// 切换用户视图，参数viewId
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeGridView()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid viewId = _Request["viewId"].ObjToGuid();
            if (viewId == Guid.Empty) return Json(null, JsonRequestBehavior.AllowGet);
            string gridId = _Request["gridId"].ObjToStr(); //网格domId
            Sys_Grid grid = SystemOperate.GetGrid(viewId);
            if (grid == null || !grid.Sys_ModuleId.HasValue)
                return Json(null, JsonRequestBehavior.AllowGet);
            List<Sys_GridField> gridFields = SystemOperate.GetGridFields(viewId);
            if (gridFields == null) return Json(null, JsonRequestBehavior.AllowGet);
            int rf = _Request["isRf"].ObjToInt(); //客户端启用过滤行参数标识
            foreach (Sys_GridField gridField in gridFields)
            {
                if (!string.IsNullOrEmpty(gridField.FieldFormatter)) continue;
                if (!gridField.Sys_FieldId.HasValue) continue;
                Sys_Field sysField = gridField.TempSysField != null ? gridField.TempSysField : SystemOperate.GetFieldById(gridField.Sys_FieldId.Value);
                if (sysField == null) continue;
                if (gridField.TempSysField == null) gridField.TempSysField = sysField;
                gridField.FieldFormatter = SystemOperate.GetComplexGridFormatFun(GetCurrentUser(_Request).UserId, sysField, gridField.Sys_FieldName, gridId);
            }
            string tempStr = string.Empty;
            string tempNoFilterStr = string.Empty;
            bool isEnabledRowFilter = (rf == 1) || (rf != 2 && grid.AddFilterRow.HasValue && grid.AddFilterRow.Value);
            bool isEnabledFlow = BpmOperate.IsEnabledWorkflow(grid.Sys_ModuleId.Value); //是否启用流程
            string moduleName = SystemOperate.GetModuleNameById(grid.Sys_ModuleId.Value);
            bool isAddFlowStatus = false;
            if (isEnabledRowFilter)
            {
                //行过滤规则
                List<string> noFilterFields = new List<string>();
                if (isEnabledFlow)
                {
                    string mIdStr = grid.Sys_ModuleId.Value.ToString();
                    string mNameStr = moduleName;
                    if (grid.GridTypeOfEnum == GridTypeEnum.ComprehensiveDetail)
                    {
                        Sys_Module detailModule = SystemOperate.GetDetailModules(grid.Sys_ModuleId.Value).FirstOrDefault();
                        mIdStr = detailModule.Id.ToString();
                        mNameStr = detailModule.Name;
                    }
                    gridFields.Insert(2, new Sys_GridField() { Sys_FieldName = "FlowStatus", Display = "状态", IsAllowSearch = false, Width = 86, Sort = 1, IsFrozen = true, FieldFormatter = "function(value, row, index){return FlowStatusFormatter(value, row, index,'" + mIdStr + "','" + mNameStr + "');}" });
                    isAddFlowStatus = true;
                }
                StringBuilder ruleFilters = SystemOperate.GetGridRowFilterRules(grid.Sys_ModuleId.Value, gridFields, out noFilterFields, GetCurrentUser(_Request));
                //组装过滤规则
                tempStr = ruleFilters.Length > 0 ? HttpUtility.UrlEncode(string.Format("[{0}]", ruleFilters.ToString().Substring(0, ruleFilters.Length - 1)), Encoding.UTF8) : string.Empty;
                tempNoFilterStr = noFilterFields.Count > 0 ? string.Join(",", noFilterFields) : string.Empty;
            }
            //搜索字段处理
            List<Sys_GridField> tempSearchFields = SystemOperate.GetSearchGridFields(grid.Id);
            var searchFields = tempSearchFields.Select(x => new { FieldName = x.Sys_FieldName, Display = x.Display }).ToList();
            if (isEnabledFlow)
            {
                string mIdStr = grid.Sys_ModuleId.Value.ToString();
                string mNameStr = moduleName;
                if (grid.GridTypeOfEnum == GridTypeEnum.ComprehensiveDetail)
                {
                    Sys_Module detailModule = SystemOperate.GetDetailModules(grid.Sys_ModuleId.Value).FirstOrDefault();
                    mIdStr = detailModule.Id.ToString();
                    mNameStr = detailModule.Name;
                }
                string flowColumn = string.Format("flow_{0}", mIdStr);
                tempNoFilterStr = tempNoFilterStr.Length > 0 ? string.Format("{0},{1}", tempNoFilterStr, flowColumn) : flowColumn;
                gridFields.Insert(1, new Sys_GridField() { Sys_FieldName = flowColumn, Display = string.Empty, IsAllowSearch = false, Width = 30, Sort = 0, IsFrozen = true, FieldFormatter = "function(value, row, index){return FlowIconFormatter(value, row, index,'" + mIdStr + "','" + mNameStr + "');}" });
                if (!isAddFlowStatus)
                    gridFields.Insert(2, new Sys_GridField() { Sys_FieldName = "FlowStatus", Display = "状态", IsAllowSearch = false, Width = 50, Sort = 1, IsFrozen = true, FieldFormatter = "function(value, row, index){return FlowStatusFormatter(value, row, index,'" + mIdStr + "','" + mNameStr + "');}" });
            }
            //返回数据
            if (searchFields.Count > 0)
                return Json(new { GridView = grid, ViewFields = gridFields, RuleFilters = tempStr, NoFilterFields = tempNoFilterStr, SearchFields = searchFields }, JsonRequestBehavior.AllowGet);
            return Json(new { GridView = grid, ViewFields = gridFields, RuleFilters = tempStr, NoFilterFields = tempNoFilterStr }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存用户视图
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveGridView()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string errMsg = string.Empty;
            Guid moduleId = _Request["moduleId"].ObjToGuid();
            Guid viewId = _Request["viewId"].ObjToGuid();
            string fieldNames = _Request["fieldNames"].ObjToStr().Trim(); //视图字段名称
            string gridName = _Request["gridName"].ObjToStr().Trim(); //视图名称
            string treeField = _Request["treeField"].ObjToStr().Trim(); //树显示字段
            string groupField = _Request["groupField"].ObjToStr(); //分组字段
            bool isDefault = _Request["isDefault"].ObjToBool(); //是否默认
            UserInfo currUser = GetCurrentUser(_Request);
            if (currUser == null)
                return Json(new ReturnResult { Success = false, Message = "非法操作，请先登录！" });
            if (string.IsNullOrEmpty(fieldNames))
                return Json(new ReturnResult { Success = false, Message = "视图字段未设置！" });
            if (string.IsNullOrEmpty(gridName))
                return Json(new ReturnResult { Success = false, Message = "视图名称不能为空！" });
            if (moduleId == Guid.Empty && viewId == Guid.Empty)
                return Json(new ReturnResult { Success = false, Message = "没有传递视图Id或模块Id！" });
            string[] token = fieldNames.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            List<string> fn = new List<string>();
            Dictionary<string, int> sortDic = new Dictionary<string, int>(); //排序
            sortDic.Add("Id", 0);
            int no = 0; //字段序号
            foreach (string name in token)
            {
                if (string.IsNullOrWhiteSpace(name)) continue;
                no++;
                fn.Add(name);
                int tempNo = no;
                if (CommonDefine.BaseEntityFields.Contains(name))
                    tempNo = no + 100;
                sortDic.Add(name, tempNo);
            }
            Sys_Grid defaultGrid = SystemOperate.GetDefaultGrid(moduleId); //默认视图
            List<Sys_GridField> fields = defaultGrid.GridFields != null && defaultGrid.GridFields.Count > 0 ? defaultGrid.GridFields : SystemOperate.GetDefaultGridFields(moduleId); //加载默认视图字段
            if (fields.Count > 0 && (defaultGrid.GridFields == null || defaultGrid.GridFields.Count == 0))
                defaultGrid.GridFields = fields;
            Sys_GridField groupFieldDefault = !string.IsNullOrEmpty(groupField) ? fields.Where(x => x.Sys_FieldName == groupField).FirstOrDefault() : null;
            fields = fields.Where(x => fn.Contains(x.Sys_FieldName) || x.Sys_FieldName == "Id").ToList();
            if (fields.Count == 0)
                return Json(new ReturnResult { Success = false, Message = "找不到模块对应的视图字段！" });
            Sys_Grid view = SystemOperate.GetGrid(viewId);
            ModelRecordOperateType operateType = ModelRecordOperateType.Add;
            if (view == null) //新增视图
            {
                //先保存视图信息
                view = new Sys_Grid();
                Sys_Grid grid = SystemOperate.GetDefaultGrid(moduleId);
                view.Sys_ModuleId = moduleId;
                view.GridType = (int)GridTypeEnum.Custom;
                ObjectHelper.CopyValue(grid, view);
                view.Id = Guid.NewGuid();
                view.IsDefault = false;
                view.CreateDate = DateTime.Now;
                view.CreateUserId = currUser.UserId;
                view.CreateUserName = UserInfo.GetUserAliasName(currUser);
                view.ModifyDate = DateTime.Now;
                view.ModifyUserId = currUser.UserId;
                view.ModifyUserName = UserInfo.GetUserAliasName(currUser);
            }
            else //修改视图
            {
                operateType = ModelRecordOperateType.Edit;
            }
            //验证视图名称是否重复
            var simpleViews = SystemOperate.GetUserGrids(currUser.UserId, moduleId).Where(x => x.Name == gridName && x.Id != viewId).ToList();
            if (simpleViews != null && simpleViews.Count > 0)
            {
                return Json(new ReturnResult { Success = false, Message = string.Format("名称为【{0}】的视图已存在，请更新视图名称！", gridName) });
            }
            //保存视图
            view.Name = gridName;
            view.TreeField = treeField;
            view.IsDefault = false;
            if (!string.IsNullOrEmpty(treeField))
            {
                Sys_GridField treeGridField = fields.Where(x => x.Sys_FieldName == treeField && x.IsVisible).FirstOrDefault();
                if (treeGridField != null && !SystemOperate.IsForeignField(moduleId, treeField)) //树显示字段存在并且是非外键字段
                {
                    view.IsTreeGrid = true;
                }
            }
            Guid tempViewId = CommonOperate.OperateRecord<Sys_Grid>(view, operateType, out errMsg, null, false);
            if (tempViewId == Guid.Empty)
                return Json(new ReturnResult { Success = false, Message = string.Format("视图主信息保存失败，异常信息：{0}！", errMsg) });
            //再保存视图字段
            if (operateType == ModelRecordOperateType.Edit) //编辑视图时先将所有视图字段删除
            {
                bool delRs = CommonOperate.DeleteRecordsByExpression<Sys_GridField>(x => x.Sys_GridId == viewId, out errMsg);
                if (!delRs && !string.IsNullOrEmpty(errMsg))
                    return Json(new ReturnResult { Success = false, Message = string.Format("视图主信息保存失败，异常信息：{0}！", errMsg) });
            }
            else
            {
                view.Id = tempViewId;
            }
            List<Sys_GridField> tempFields = new List<Sys_GridField>();
            bool isComprehensiveView = false; //是否是综合视图
            bool isComprehensiveDetailView = false; //是否综合明细视图
            //添加明细或外键模块字段
            List<string> mainModuleFns = fields.Select(x => x.Sys_FieldName).ToList(); //主模块字段名称集合
            List<string> otherModuleFns = fn.Where(x => !mainModuleFns.Contains(x)).ToList(); //明细或外键模块字段集合
            if (otherModuleFns.Count > 0) //存在明细或外键模块字段
            {
                //明细模块字段处理
                Sys_Module detailModule = SystemOperate.GetDetailModules(moduleId).FirstOrDefault();
                if (detailModule != null)
                {
                    List<Sys_Field> detailFields = SystemOperate.GetFieldInfos(detailModule).Where(x => otherModuleFns.Contains(x.Name) || (!string.IsNullOrEmpty(x.ForeignModuleName) && SystemOperate.GetModuleByName(x.ForeignModuleName) != null && otherModuleFns.Contains(x.Name.Substring(0, x.Name.Length - 2) + "Name"))).ToList();
                    if (detailFields.Count > 0)
                    {
                        tempFields.AddRange(detailFields.Select(x => new Sys_GridField()
                        {
                            Sys_GridId = view.Id,
                            Sys_GridName = view.Name,
                            Sys_FieldId = x.Id,
                            Sys_FieldName = x.Name,
                            Display = x.Display,
                            Sort = sortDic.ContainsKey(x.Name) ? sortDic[x.Name] : sortDic[x.Name.Substring(0, x.Name.Length - 2) + "Name"],
                            CreateDate = DateTime.Now,
                            CreateUserId = currUser.UserId,
                            CreateUserName = UserInfo.GetUserAliasName(currUser),
                            ModifyDate = DateTime.Now,
                            ModifyUserId = currUser.UserId,
                            ModifyUserName = UserInfo.GetUserAliasName(currUser)
                        }));
                        List<string> tempDetailFns = detailFields.Select(x => x.Name).ToList();
                        otherModuleFns = otherModuleFns.Where(x => !tempDetailFns.Contains(x)).ToList();
                        isComprehensiveDetailView = true;
                    }
                }
                if (otherModuleFns.Count > 0)
                {
                    //外键模块字段处理
                    List<Sys_Module> foreignModules = SystemOperate.GetNoRepeatForeignModules(moduleId);
                    foreach (Sys_Module foreignModule in foreignModules)
                    {
                        if (otherModuleFns.Count == 0) continue;
                        List<Sys_Field> foreignFields = SystemOperate.GetFieldInfos(foreignModule).Where(x => otherModuleFns.Contains(x.Name) || (!string.IsNullOrEmpty(x.ForeignModuleName) && SystemOperate.GetModuleByName(x.ForeignModuleName) != null && otherModuleFns.Contains(x.Name.Substring(0, x.Name.Length - 2) + "Name"))).ToList();
                        if (foreignFields.Count == 0) continue;
                        tempFields.AddRange(foreignFields.Select(x => new Sys_GridField()
                        {
                            Sys_GridId = view.Id,
                            Sys_GridName = view.Name,
                            Sys_FieldId = x.Id,
                            Sys_FieldName = x.Name,
                            Display = x.Display,
                            Sort = sortDic.ContainsKey(x.Name) ? sortDic[x.Name] : sortDic[x.Name.Substring(0, x.Name.Length - 2) + "Name"],
                            CreateDate = DateTime.Now,
                            CreateUserId = currUser.UserId,
                            CreateUserName = UserInfo.GetUserAliasName(currUser),
                            ModifyDate = DateTime.Now,
                            ModifyUserId = currUser.UserId,
                            ModifyUserName = UserInfo.GetUserAliasName(currUser)
                        }));
                        List<string> tempFns = foreignFields.Select(x => x.Name).ToList();
                        otherModuleFns = otherModuleFns.Where(x => !tempFns.Contains(x)).ToList();
                        isComprehensiveView = true;
                    }
                }
                if (isComprehensiveView || isComprehensiveDetailView) //将原来的视图模式修改为综合模式
                {
                    if (isComprehensiveDetailView)
                        view.GridType = (int)GridTypeEnum.ComprehensiveDetail;
                    else
                        view.GridType = (int)GridTypeEnum.Comprehensive;
                    CommonOperate.OperateRecord<Sys_Grid>(view, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "GridType" }, false);
                }
                else
                {
                    view.GridTypeOfEnum = GridTypeEnum.System;
                    CommonOperate.OperateRecord<Sys_Grid>(view, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "GridType" }, false);
                }
            }
            else
            {
                view.GridTypeOfEnum = GridTypeEnum.System;
                CommonOperate.OperateRecord<Sys_Grid>(view, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "GridType" }, false);
            }
            //添加主模块字段
            foreach (Sys_GridField f in fields)
            {
                Sys_GridField fd = new Sys_GridField();
                ObjectHelper.CopyValue(f, fd);
                fd.Id = Guid.NewGuid();
                Guid fieldId = SystemOperate.GetFieldId(view.Sys_ModuleId.Value, groupField);
                if (fieldId == f.Sys_FieldId.Value)
                {
                    fd.IsGroupField = true;
                }
                else
                {
                    fd.IsGroupField = false;
                }
                fd.Sys_GridId = view.Id;
                fd.Sys_GridName = view.Name;
                fd.Sort = sortDic[fd.Sys_FieldName];
                fd.CreateDate = DateTime.Now;
                fd.CreateUserId = currUser.UserId;
                fd.CreateUserName = UserInfo.GetUserAliasName(currUser);
                fd.ModifyDate = DateTime.Now;
                fd.ModifyUserId = currUser.UserId;
                fd.ModifyUserName = UserInfo.GetUserAliasName(currUser);
                tempFields.Add(fd);
            }
            if (groupFieldDefault != null && !tempFields.Select(x => x.Sys_FieldName).Contains(groupField))
            {
                Sys_GridField fd = new Sys_GridField();
                ObjectHelper.CopyValue(groupFieldDefault, fd);
                fd.Id = Guid.NewGuid();
                fd.IsGroupField = true;
                fd.Sys_GridId = view.Id;
                fd.Sys_GridName = view.Name;
                fd.Sort = 0;
                fd.IsVisible = false;
                fd.IsAllowSearch = false;
                fd.IsAllowSort = false;
                fd.CreateDate = DateTime.Now;
                fd.CreateUserId = currUser.UserId;
                fd.CreateUserName = UserInfo.GetUserAliasName(currUser);
                fd.ModifyDate = DateTime.Now;
                fd.ModifyUserId = currUser.UserId;
                fd.ModifyUserName = UserInfo.GetUserAliasName(currUser);
                tempFields.Add(fd);
            }
            //保存网格字段
            bool rs = CommonOperate.OperateRecords<Sys_GridField>(tempFields, ModelRecordOperateType.Add, out errMsg, false);
            if (rs)
            {
                Sys_UserGrid userGrid = CommonOperate.GetEntity<Sys_UserGrid>(x => x.Sys_UserId == currUser.UserId && x.Sys_GridId == view.Id, null, out errMsg);
                ModelRecordOperateType userGridOperateType = ModelRecordOperateType.Edit;
                if (userGrid == null)
                {
                    userGridOperateType = ModelRecordOperateType.Add;
                    userGrid = new Sys_UserGrid() { Sys_GridId = view.Id, Sys_UserId = currUser.UserId, IsDefault = isDefault };
                }
                else
                {
                    userGrid.IsDefault = isDefault;
                }
                userGrid.IsDeleted = false;
                Guid result = CommonOperate.OperateRecord<Sys_UserGrid>(userGrid, userGridOperateType, out errMsg, null, false);
                if (result != Guid.Empty && isDefault)
                {
                    //将其他用户视图更新为非默认视图
                    CommonOperate.UpdateRecordsByExpression<Sys_UserGrid>(new { IsDefault = false }, x => x.Sys_UserId == currUser.UserId && x.Sys_GridId != view.Id, out errMsg);
                }
                return Json(new { Success = result != Guid.Empty, Message = errMsg, ViewId = view.Id, TreeField = treeField });
            }
            else
            {
                return Json(new ReturnResult { Success = false, Message = string.Format("视图字段信息保存失败，异常信息：{0}！", errMsg) });
            }
        }

        /// <summary>
        /// 删除视图
        /// </summary>
        /// <returns></returns>
        public ActionResult DelGridView()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            if (currUser == null)
                return Json(new ReturnResult { Success = false, Message = "非法操作，请先登录！" });
            Guid viewId = _Request["viewId"].ObjToGuid();
            bool isCanDel = SystemOperate.IsUserGridView(viewId, currUser.UserId);
            if (!isCanDel)
                return Json(new ReturnResult { Success = false, Message = "您不是该视图的创建者，无法删除该视图！" });
            string errMsg = SystemOperate.DeleteUserGrid(currUser.UserId, viewId);
            return Json(new ReturnResult { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        /// <summary>
        /// 检测视图是否可编辑，系统视图和非自己创建视图不可编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult IsGridViewCanOperate()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid viewId = _Request["viewId"].ObjToGuid();
            bool rs = SystemOperate.IsUserGridView(viewId, GetCurrentUser(_Request).UserId);
            Sys_Grid grid = SystemOperate.GetGrid(viewId);
            string msg = rs ? string.Empty : "您不是该视图的创建者无法操作该视图！";
            return Json(new { Success = rs, Message = msg, TreeField = grid.TreeField == null ? string.Empty : grid.TreeField });
        }

        /// <summary>
        /// 加载网格内明细视图
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadInnerDetailModuleGrid()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid id = _Request["id"].ObjToGuid(); //行展开的记录Id
            int hasTree = _Request["hasTree"].ObjToInt(); //是否有左侧树
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module == null || id == Guid.Empty)
                return Content(string.Empty);
            UserInfo currUser = GetCurrentUser(_Request);
            if (currUser == null)
                return Content(string.Empty);
            StringBuilder viewStr = new StringBuilder();
            List<Sys_Module> detailOrAttachModules = new List<Sys_Module>();
            if (module.DetailInGrid)
            {
                //明细模块
                detailOrAttachModules.AddRange(SystemOperate.GetDetailModules(module.Id));
            }
            //附属模块
            detailOrAttachModules.AddRange(SystemOperate.GetUserBindAttachModules(currUser.UserId, module.Id, true));
            string ttClass = string.Empty;
            int w = currUser.ClientBrowserWidth - 180 - 120;
            if (hasTree == 1)
                w -= 180;
            string wStr = string.Format("{0}px", w);
            string hStr = "272px";
            string tag = id.ToString();
            viewStr.AppendFormat("<div id=\"ddv_{0}\">", tag);
            viewStr.AppendFormat("<div id=\"tt_{0}\" class=\"easyui-tabs\" style=\"width:{1};height:{2};overflow:hidden\" data-options=\"tools:'#tt_toolbar_{0}',tabHeight:25,onSelect:ExpandRowTabSelected\">", tag, wStr, hStr);
            string condition = "{" + module.TableName + "Id:" + id.ToString() + "}";
            foreach (Sys_Module detailModule in detailOrAttachModules)
            {
                string url = string.Format("/Page/Grid.html?moduleId={0}&page=inGrid&condition={1}", detailModule.Id.ToString(), condition);
                viewStr.AppendFormat("<div title=\"{0}\" attachModuleId=\"{1}\" border=\"false\"><iframe url=\"{2}\" scrolling=\"auto\" frameborder=\"0\" class=\"ifr\" src=\"\" style=\"width: 100%;height: 100%;\"></iframe></div>",
                    detailModule.Name, detailModule.Id.ToString(), url);
            }
            viewStr.Append("</div>");
            //tab工具栏
            viewStr.AppendFormat("<div id=\"tt_toolbar_{0}\">", tag);
            viewStr.AppendFormat("<a id=\"tt_a_refresh_{0}\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-reload',plain:true\" title=\"刷新\"></a>", tag);
            //用户是否绑定了网格内附属模块
            bool hasInnerAttach = SystemOperate.HasUserAttachModule(currUser.UserId, module.Id, true);
            //是否显示附属设置按钮
            bool showAttachBtn = module.DetailInGrid || hasInnerAttach;
            if (showAttachBtn)
            {
                viewStr.AppendFormat("<a id=\"tt_a_set_{0}\" moduleId=\"{1}\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-cog',plain:true\" title=\"附属模块显示设置\" onclick=\"AttachModuleSet(this)\"></a>", tag, module.Id.ToString());
            }
            viewStr.Append("</div>");
            //标签右键菜单
            viewStr.AppendFormat("<div id=\"tt_mm_{0}\" class=\"easyui-menu\" style=\"width: 150px;\">", tag);
            viewStr.AppendFormat("<div id=\"tt_mm_refresh_{0}\">刷新</div>", tag);
            viewStr.Append("</div>");
            viewStr.Append("</div>");
            return Content(viewStr.ToString());
        }

        /// <summary>
        /// 保存用户附属模块设置
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveUserAttachModuleSet()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            if (currUser == null)
                return Json(new ReturnResult { Success = false, Message = "非法操作，请先登录！" });
            //主模块Id
            Guid moduleId = _Request["moduleId"].ObjToGuid();
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            if (module == null)
                return Json(new ReturnResult { Success = false, Message = "模块不存在！" });
            //启用的附属模块绑定信息
            string json = HttpUtility.UrlDecode(_Request["attachModuleInfo"].ObjToStr(), Encoding.UTF8);
            try
            {
                List<Sys_AttachModuleBind> attachBinds = JsonHelper.Deserialize<List<Sys_AttachModuleBind>>(json);
                if (attachBinds == null)
                    return Json(new ReturnResult { Success = false, Message = "附属模块绑定信息不能为空！" });
                Guid userId = currUser.UserId;
                string errMsg = string.Empty;
                foreach (Sys_AttachModuleBind attachBind in attachBinds)
                {
                    attachBind.ModuleName = module.Name;
                    attachBind.Sys_UserId = userId;
                    attachBind.IsValid = true;
                    attachBind.CreateDate = DateTime.Now;
                    attachBind.CreateUserId = userId;
                    attachBind.CreateUserName = UserInfo.GetUserAliasName(currUser);
                    attachBind.ModifyDate = DateTime.Now;
                    attachBind.ModifyUserId = userId;
                    attachBind.ModifyUserName = UserInfo.GetUserAliasName(currUser);
                }
                //先删除之前的
                bool rs = CommonOperate.DeleteRecordsByExpression<Sys_AttachModuleBind>(x => x.ModuleName == module.Name && x.Sys_UserId == userId, out errMsg);
                if (!string.IsNullOrEmpty(errMsg))
                    return Json(new ReturnResult { Success = false, Message = errMsg });
                rs = CommonOperate.OperateRecords<Sys_AttachModuleBind>(attachBinds, ModelRecordOperateType.Add, out errMsg, false);
                return Json(new ReturnResult { Success = rs, Message = errMsg });
            }
            catch (Exception ex)
            {
                return Json(new ReturnResult { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 加载视图按钮
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadViewButtons()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module == null) return Json(null);
            List<Sys_GridButton> buttons = SystemOperate.GetTopButtons(module.Id);
            buttons.Insert(0, new Sys_GridButton() { Id = Guid.Empty, ButtonText = "请选择" });
            return Json(buttons);
        }

        /// <summary>
        /// 保存常用按钮设置
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveCommonBtn()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            Guid moduleId = _Request["moduleId"].ObjToGuid();
            string btnIndexs = _Request["indexs"].ObjToStr();
            string errMsg = string.Empty;
            string[] token = btnIndexs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //先删除之前的通用按钮
            bool rs = CommonOperate.DeleteRecordsByExpression<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && CommonDefine.GridCommonBtns.Contains(x.ButtonText), out errMsg);
            if (rs)
            {
                if (token != null && token.Length > 0)
                {
                    //添加通用按钮
                    List<Sys_GridButton> buttons = new List<Sys_GridButton>();
                    List<int> indexs = token.Select(x => x.ObjToInt()).Where(x => x >= 0).ToList();
                    Guid auxiliaryBtnId = Guid.Empty;
                    bool auxiliaryFlag = false; //是否添加辅助按钮标识
                    foreach (int index in indexs)
                    {
                        string text = CommonDefine.GridCommonBtns[index];
                        if (text == "复制" || text == "批量编辑" || text == "导出" || text == "打印")
                        {
                            auxiliaryFlag = true;
                            break;
                        }
                    }
                    if (auxiliaryFlag)
                    {
                        #region 辅助
                        Sys_GridButton auxiliaryBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "辅助", null, out errMsg);
                        if (auxiliaryBtn == null)
                        {
                            auxiliaryBtn = new Sys_GridButton()
                            {
                                Sys_ModuleId = moduleId,
                                ButtonTagId = "btnAuxiliary",
                                IsValid = true,
                                ButtonText = "辅助",
                                ButtonIcon = "eu-icon-cog",
                                ClickMethod = "",
                                IsSystem = true,
                                Sort = 10,
                                OperateButtonTypeOfEnum = OperateButtonTypeEnum.FileMenuButton,
                                GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar,
                                CreateDate = DateTime.Now,
                                CreateUserId = currUser.UserId,
                                CreateUserName = UserInfo.GetUserAliasName(currUser),
                                ModifyDate = DateTime.Now,
                                ModifyUserId = currUser.UserId,
                                ModifyUserName = UserInfo.GetUserAliasName(currUser)
                            };
                            auxiliaryBtnId = CommonOperate.OperateRecord<Sys_GridButton>(auxiliaryBtn, ModelRecordOperateType.Add, out errMsg);
                        }
                        else
                        {
                            auxiliaryBtnId = auxiliaryBtn.Id;
                        }
                        #endregion
                    }
                    foreach (int index in indexs)
                    {
                        string text = CommonDefine.GridCommonBtns[index];
                        string tagId = string.Empty;
                        string icon = string.Empty;
                        string click = string.Empty;
                        int sort = 0;
                        Guid parentId = Guid.Empty;
                        switch (text)
                        {
                            case "导入":
                                tagId = "btnImportModel";
                                icon = "eu-icon-export";
                                click = "ImportModel(this)";
                                sort = 0;
                                break;
                            case "新增":
                                tagId = "btnAdd";
                                icon = "eu-icon-add";
                                click = "Add(this)";
                                sort = 1;
                                break;
                            case "编辑":
                                tagId = "btnEdit";
                                icon = "eu-icon-edit";
                                click = "Edit(this)";
                                sort = 2;
                                break;
                            case "删除":
                                tagId = "btnDelete";
                                icon = "eu-p2-icon-delete2";
                                click = "Delete(this)";
                                sort = 3;
                                break;
                            case "查看":
                                tagId = "btnViewRecord";
                                icon = "eu-icon-search";
                                click = "ViewRecord(this)";
                                sort = 4;
                                break;
                            case "复制":
                                tagId = "btnCopy";
                                icon = "eu-icon-copy";
                                click = "Copy(this)";
                                sort = 11;
                                parentId = auxiliaryBtnId;
                                break;
                            case "批量编辑":
                                tagId = "btnBatchEdit";
                                icon = "eu-icon-edit";
                                click = "BatchEdit(this)";
                                sort = 12;
                                parentId = auxiliaryBtnId;
                                break;
                            case "导出":
                                tagId = "btnExportModel";
                                icon = "eu-icon-export";
                                click = "ExportModel(this)";
                                sort = 13;
                                parentId = auxiliaryBtnId;
                                break;
                            case "打印":
                                tagId = "btnPrintModel";
                                icon = "eu-icon-print";
                                click = "PrintModel(this)";
                                sort = 14;
                                parentId = auxiliaryBtnId;
                                break;
                        }
                        buttons.Add(new Sys_GridButton()
                        {
                            Sys_ModuleId = moduleId,
                            ButtonTagId = tagId,
                            IsValid = true,
                            ButtonText = text,
                            ButtonIcon = icon,
                            ClickMethod = click,
                            IsSystem = true,
                            ParentId = parentId,
                            Sort = sort,
                            OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton,
                            GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar,
                            CreateDate = DateTime.Now,
                            CreateUserId = currUser.UserId,
                            CreateUserName = UserInfo.GetUserAliasName(currUser),
                            ModifyDate = DateTime.Now,
                            ModifyUserId = currUser.UserId,
                            ModifyUserName = UserInfo.GetUserAliasName(currUser)
                        });
                    }
                    if (buttons.Count > 0)
                    {
                        CommonOperate.OperateRecords<Sys_GridButton>(buttons, ModelRecordOperateType.Add, out errMsg, false);
                    }
                }
            }
            return Json(new ReturnResult { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        /// <summary>
        /// 刷新字段格式化
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshFieldFormat()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string ids = _Request["ids"].ObjToStr();
            if (string.IsNullOrEmpty(ids))
                return Json(new ReturnResult() { Success = false, Message = "ids为空" }, JsonRequestBehavior.AllowGet);
            List<Guid> token = ids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
            if (token == null || token.Count == 0)
                return Json(new ReturnResult() { Success = false, Message = "ids为空" }, JsonRequestBehavior.AllowGet);
            string errMsg = string.Empty;
            bool isGridEnableMemeryCache = ModelConfigHelper.IsModelEnableMemeryCache(typeof(Sys_Grid)); //Sys_Grid是否启动内存缓存
            foreach (Guid id in token)
            {
                Sys_GridField gf = SystemOperate.GetAllGridFields(x => x.Id == id).FirstOrDefault();
                gf.FieldFormatter = null;
                gf.EditorFormatter = null;
                if (gf == null) continue;
                CommonOperate.OperateRecord<Sys_GridField>(gf, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "FieldFormatter", "EditorFormatter" }, false);
                if (isGridEnableMemeryCache && gf.Sys_GridId.HasValue)
                {
                    Sys_Grid grid = SystemOperate.GetGrid(gf.Sys_GridId.Value);
                    if (grid != null && grid.GridFields != null)
                        grid.GridFields = null;
                }
            }
            return Json(new ReturnResult() { Success = string.IsNullOrEmpty(errMsg), Message = errMsg }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 表单处理

        /// <summary>
        /// 加载角色表单
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadRoleForms()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module == null)
                return Json(new ReturnResult { Success = false, Message = "选择的模块在系统中不存在！" });
            Guid roleId = _Request["roleId"].ObjToGuid();
            string errMsg = string.Empty;
            Sys_RoleForm roleForm = CommonOperate.GetEntity<Sys_RoleForm>(x => x.Sys_RoleId == roleId && !x.IsDeleted, null, out errMsg);
            List<Sys_Form> forms = SystemOperate.GetModuleForms(module.Id);
            forms = forms.Where(x => !x.IsDefault).ToList();
            return Json(forms.Select(x => new { Id = x.Id, Name = x.Name, IsBind = roleForm != null && roleForm.Sys_FormId == x.Id }));
        }

        /// <summary>
        /// 保存表单
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveForm()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string errMsg = string.Empty;
            Guid moduleId = _Request["moduleId"].ObjToGuid();
            Guid formId = _Request["formId"].ObjToGuid();
            string fieldNames = _Request["fieldNames"].ObjToStr().Trim(); //表单字段名称
            string rowNums = _Request["rowNums"].ObjToStr(); //行号
            string colNums = _Request["colNums"].ObjToStr(); //列号
            string canEdits = _Request["canEdits"].ObjToStr(); //是否允许编辑
            string formName = _Request["formName"].ObjToStr().Trim(); //表单名称
            int editMode = _Request["editMode"].ObjToInt(); //编辑模式
            if (string.IsNullOrEmpty(fieldNames))
                return Json(new ReturnResult { Success = false, Message = "表单字段未设置！" });
            if (string.IsNullOrEmpty(formName))
                return Json(new ReturnResult { Success = false, Message = "表单名称不能为空！" });
            if (moduleId == Guid.Empty && formId == Guid.Empty)
                return Json(new ReturnResult { Success = false, Message = "没有传递表单Id或模块Id！" });
            UserInfo currUser = GetCurrentUser(_Request);
            string[] token = fieldNames.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            int[] tokenRowNum = string.IsNullOrEmpty(rowNums) ? null : rowNums.Split(",".ToCharArray()).Select(x => x.ObjToInt()).ToArray();
            int[] tokenColNum = string.IsNullOrEmpty(colNums) ? null : colNums.Split(",".ToCharArray()).Select(x => x.ObjToInt()).ToArray();
            bool[] tokenCanEdit = string.IsNullOrEmpty(canEdits) ? null : canEdits.Split(",".ToCharArray()).Select(x => x == "是").ToArray();
            List<string> fn = new List<string>();
            Dictionary<string, int> rowNumDic = new Dictionary<string, int>();
            Dictionary<string, int> colNumDic = new Dictionary<string, int>();
            Dictionary<string, bool> canEditDic = new Dictionary<string, bool>();
            for (int i = 0; i < token.Length; i++)
            {
                string name = token[i];
                if (string.IsNullOrWhiteSpace(name)) continue;
                fn.Add(name);
                if (tokenRowNum != null && tokenRowNum.Length == token.Length)
                    rowNumDic.Add(name, tokenRowNum[i]);
                if (tokenColNum != null && tokenColNum.Length == token.Length)
                    colNumDic.Add(name, tokenColNum[i]);
                if (tokenCanEdit != null && tokenCanEdit.Length == token.Length)
                    canEditDic.Add(name, tokenCanEdit[i]);
            }
            List<Sys_FormField> fields = SystemOperate.GetDefaultFormField(moduleId);
            fields = fields.Where(x => fn.Contains(x.Sys_FieldName) || x.Sys_FieldName == "Id").ToList();
            if (fields.Count == 0)
                return Json(new ReturnResult { Success = false, Message = "找不到模块对应的表单字段！" });
            Sys_Form form = SystemOperate.GetForm(formId);
            ModelRecordOperateType operateType = ModelRecordOperateType.Add;
            if (form == null) //新增表单
            {
                //验证表单名称是否重复
                var simpleForms = SystemOperate.GetForm(moduleId, formName);
                if (simpleForms != null)
                {
                    return Json(new ReturnResult { Success = false, Message = string.Format("名称为【{0}】的表单已存在，请重新设置表单名称！", formName) });
                }
                //复制原始表单数据
                form = new Sys_Form();
                Sys_Form tempForm = SystemOperate.GetDefaultForm(moduleId);
                form.Sys_ModuleId = moduleId;
                ObjectHelper.CopyValue(tempForm, form);
                form.Id = Guid.NewGuid();
                form.Width = null;
                form.Height = null;
            }
            else //修改表单
            {
                operateType = ModelRecordOperateType.Edit;
                //验证表单名称是否重复
                var simpleForms = CommonOperate.GetEntities<Sys_Form>(out errMsg, x => x.Sys_ModuleId == moduleId && x.Name == formName && x.Id != formId, null, false);
                if (simpleForms != null && simpleForms.Count > 0)
                {
                    return Json(new ReturnResult { Success = false, Message = string.Format("名称为【{0}】的表单已存在，请重新设置表单名称！", formName) });
                }
            }
            //保存表单
            form.Name = formName;
            form.ModuleEditMode = editMode;
            form.IsDefault = false;
            Guid tempFormId = CommonOperate.OperateRecord<Sys_Form>(form, operateType, out errMsg, null, false);
            if (tempFormId == Guid.Empty)
                return Json(new ReturnResult { Success = false, Message = string.Format("表单主信息保存失败，异常信息：{0}！", errMsg) });
            //再保存表单字段
            if (operateType == ModelRecordOperateType.Edit) //编辑表单时先将所有表单字段删除
            {
                bool delRs = CommonOperate.DeleteRecordsByExpression<Sys_FormField>(x => x.Sys_FormId == formId, out errMsg);
                if (!delRs)
                    return Json(new ReturnResult { Success = false, Message = string.Format("表单主信息保存失败，异常信息：{0}！", errMsg) });
            }
            else
            {
                form.Id = tempFormId;
            }
            List<Sys_FormField> tempFields = new List<Sys_FormField>();
            foreach (Sys_FormField f in fields)
            {
                Sys_FormField fd = new Sys_FormField();
                ObjectHelper.CopyValue(f, fd);
                fd.Id = Guid.NewGuid();
                fd.CreateDate = DateTime.Now;
                fd.CreateUserId = currUser.UserId;
                fd.CreateUserName = UserInfo.GetUserAliasName(currUser);
                fd.Sys_FormId = form.Id;
                fd.ModifyDate = DateTime.Now;
                fd.ModifyUserId = currUser.UserId;
                fd.ModifyUserName = UserInfo.GetUserAliasName(currUser);
                if (rowNumDic.ContainsKey(f.Sys_FieldName))
                {
                    fd.RowNo = rowNumDic[f.Sys_FieldName];
                }
                if (colNumDic.ContainsKey(f.Sys_FieldName))
                {
                    fd.ColNo = colNumDic[f.Sys_FieldName];
                }
                if (canEditDic.ContainsKey(f.Sys_FieldName))
                {
                    fd.IsAllowAdd = canEditDic[f.Sys_FieldName];
                    fd.IsAllowEdit = canEditDic[f.Sys_FieldName];
                }
                tempFields.Add(fd);
            }
            bool rs = CommonOperate.OperateRecords<Sys_FormField>(tempFields, ModelRecordOperateType.Add, out errMsg, false);
            return Json(new ReturnResult { Success = rs, Message = errMsg });
        }

        /// <summary>
        /// 关联角色表单
        /// </summary>
        /// <returns></returns>
        public ActionResult RelateRoleForm()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string errMsg = string.Empty;
            Guid roleId = _Request["roleId"].ObjToGuid();
            Guid formId = _Request["formId"].ObjToGuid();
            Guid moduleId = _Request["moduleId"].ObjToGuid();
            Sys_RoleForm roleForm = CommonOperate.GetEntity<Sys_RoleForm>(x => x.Sys_RoleId == roleId && x.Sys_FormId == formId, null, out errMsg);
            ModelRecordOperateType roleFormOperateType = ModelRecordOperateType.Edit;
            if (roleForm == null)
            {
                roleFormOperateType = ModelRecordOperateType.Add;
                roleForm = new Sys_RoleForm() { Sys_FormId = formId, Sys_RoleId = roleId, Sys_ModuleId = moduleId };
            }
            roleForm.IsDeleted = false;
            Guid result = CommonOperate.OperateRecord<Sys_RoleForm>(roleForm, roleFormOperateType, out errMsg, new List<string>() { "IsDeleted" }, false);
            return Json(new { Success = result != Guid.Empty, Message = errMsg, FormId = formId });
        }

        /// <summary>
        /// 删除角色表单
        /// </summary>
        /// <returns></returns>
        public ActionResult DelRoleForm()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid roleId = _Request["roleId"].ObjToGuid();
            Guid formId = _Request["formId"].ObjToGuid();
            string errMsg = string.Empty;
            //先取消角色表单关联
            bool rs = CommonOperate.DeleteRecordsByExpression<Sys_RoleForm>(x => x.Sys_FormId == formId && x.Sys_RoleId == roleId, out errMsg);
            //再删除对应的表单
            rs = CommonOperate.DeleteRecords<Sys_Form>(new List<Guid>() { formId }, out errMsg, false, false);
            return Json(new ReturnResult { Success = rs, Message = errMsg });
        }

        /// <summary>
        /// 获取模块表单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetModuleForms()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module != null)
            {
                List<Sys_Form> forms = SystemOperate.GetModuleForms(module.Id);
                return Json(forms);
            }
            return Json(null);
        }

        /// <summary>
        /// 加载表单按钮
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadFormBtns()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
            if (module == null) return Json(null, JsonRequestBehavior.AllowGet);
            FormTypeEnum formType = (FormTypeEnum)Enum.Parse(typeof(FormTypeEnum), _Request["formType"].ObjToInt().ToString());
            ModuleEditModeEnum editMode = (ModuleEditModeEnum)Enum.Parse(typeof(ModuleEditModeEnum), _Request["editMode"].ObjToInt().ToString());
            Guid? recordId = _Request["id"].ObjToGuidNull();
            bool isDraft = _Request["isDraft"].ObjToInt() == 1;
            Guid? todoTaskId = _Request["todoId"].ObjToGuidNull();
            int? recycle = _Request["recycle"].ObjToIntNull();
            string page = _Request["page"].ObjToStr(); //弹出框架加载前当前页面
            string detailStr = (module.ParentId.HasValue && module.ParentId.Value != Guid.Empty && (page == "edit" || page == "add")).ToString().ToLower();
            List<FormButton> buttons = SystemOperate.GetFormButtons(module, formType, !recordId.HasValue, isDraft, recordId, todoTaskId, GetCurrentUser(_Request));
            string methodPrex = string.Empty;
            string topStr = _Request["nfm"].ObjToInt() == 1 ? "topWin" : "top";
            if (editMode == ModuleEditModeEnum.PopFormEdit)
            {
                methodPrex = string.Format("{0}.GetCurrentDialogFrame()[0].contentWindow.", topStr);
                if (string.IsNullOrEmpty(page)) buttons = buttons.Where(x => x.IconType == ButtonIconType.Close).ToList();
            }
            if (buttons != null && buttons.Count > 0)
            {
                List<object> btnObjs = new List<object>();
                int no = 0;
                foreach (FormButton btn in buttons)
                {
                    string tagId = btn.TagId;
                    string text = btn.DisplayText.ObjToStr();
                    string icon = btn.Icon;
                    string method = btn.ClickMethod;
                    string release = string.Empty;
                    switch (btn.IconType)
                    {
                        case ButtonIconType.Save:
                            method = "Save(this,FormDataSaveCompeleted)";
                            break;
                        case ButtonIconType.SaveDraft:
                            method = "Save(this,FormDataSaveCompeleted,false,true)";
                            break;
                        case ButtonIconType.DraftRelease:
                            method = "Save(this,FormDataSaveCompeleted)";
                            release = "1";
                            break;
                        case ButtonIconType.SaveAndNew:
                            method = "Save(this,FormDataSaveCompeleted,true)";
                            break;
                        case ButtonIconType.Edit:
                            Type modelType = CommonOperate.GetModelType(module.Id);
                            if (ModelConfigHelper.ModelIsViewMode(modelType) || recycle == 1 || editMode == ModuleEditModeEnum.GridRowEdit)
                                continue;
                            method = "ToEdit(this)";
                            break;
                        case ButtonIconType.Close:
                            if (editMode == ModuleEditModeEnum.PopFormEdit ||
                                (editMode == ModuleEditModeEnum.GridRowEdit && formType == FormTypeEnum.ViewForm))
                                method = string.Format("{0}.CloseDialog()", topStr);
                            else
                                method = "CloseTab()";
                            break;
                    }
                    if (btn.IconType != ButtonIconType.Close)
                        method = methodPrex + method;
                    no++;
                    string c = string.Empty;
                    if (GlobalSet.IsShowStyleBtn)
                        c = string.Format("c{0}", (no % 9).ToString());
                    btnObjs.Add(new { id = tagId, text = text, iconCls = icon, handler = "function(){" + method + "}", detail = detailStr, release = release, iconType = (int)btn.IconType, c = c });
                }
                return Json(btnObjs, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 数据字典

        /// <summary>
        /// 获取字典分类下字典项的最大排序号
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDicClassMaxSort()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string className = _Request["className"].ObjToStr();
            if (string.IsNullOrWhiteSpace(className)) return Json(null, JsonRequestBehavior.AllowGet);
            string errMsg = string.Empty;
            List<Sys_Dictionary> list = CommonOperate.GetEntities<Sys_Dictionary>(out errMsg, x => x.ClassName == className, null, false);
            if (list != null && list.Count > 0)
            {
                int max = list.Max(x => x.Sort);
                return Json(new { MaxSort = max }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 菜单处理

        /// <summary>
        /// 加载非叶子节点菜单树
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadFolderMenuTree()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            string errMsg = string.Empty;
            Sys_Menu root = null;
            List<Sys_Menu> menus = CommonOperate.GetAllChildNodesData<Sys_Menu>(null, null, null, null, null, null, currUser);
            List<Sys_Menu> roots = CommonOperate.GetEntities<Sys_Menu>(out errMsg, x => x.ParentId == null || x.ParentId == Guid.Empty, null, true, null, null, null, null, false, null, null, currUser);
            if (roots == null) return Json(null);
            if (roots.Count > 0)
            {
                menus = menus.Where(x => !roots.Select(y => y.Name).Contains(x.Name)).ToList();
                root = new Sys_Menu() { Id = Guid.Empty, Name = "根结点", ParentId = null };
                roots.ForEach(x => { x.ParentId = Guid.Empty; });
                menus.AddRange(roots);
            }
            else
            {
                root = roots.FirstOrDefault();
            }
            menus = menus.Where(x => !x.IsLeaf && !x.IsDeleted && x.IsValid).ToList();
            TreeNode treeNode = CommonOperate.GetTree<Sys_Menu>(menus, root);
            return Json(treeNode);
        }

        /// <summary>
        /// 保存用户快捷菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveUserQuckMenus()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string menuIds = _Request["menuIds"].ObjToStr();
            if (string.IsNullOrEmpty(menuIds))
                return Json(new ReturnResult { Success = false, Message = "功能菜单Id为空！" });
            List<Guid> menuIdList = menuIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).Distinct().ToList();
            if (menuIdList == null || menuIdList.Count == 0)
                return Json(new ReturnResult { Success = false, Message = "功能菜单Id为空！" });
            UserInfo currUser = GetCurrentUser(_Request);
            string userAliasName = UserInfo.GetUserAliasName(currUser);
            List<Sys_UserQuckMenu> userQuckMenus = menuIdList.Select(x => new Sys_UserQuckMenu()
            {
                Sys_MenuId = x,
                Sys_UserId = currUser.UserId,
                Sort = menuIdList.IndexOf(x),
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                CreateUserId = currUser.UserId,
                ModifyUserId = currUser.UserId,
                CreateUserName = userAliasName,
                ModifyUserName = userAliasName
            }).ToList();
            string errMsg = string.Empty;
            //先删除用户快捷菜单设置
            CommonOperate.DeleteRecordsByExpression<Sys_UserQuckMenu>(x => x.Sys_UserId == currUser.UserId, out errMsg);
            if (string.IsNullOrEmpty(errMsg))
            {
                //再添加用户快捷菜单
                bool rs = CommonOperate.OperateRecords<Sys_UserQuckMenu>(userQuckMenus, ModelRecordOperateType.Add, out errMsg, false);
                return Json(new ReturnResult { Success = rs, Message = errMsg });
            }
            else
            {
                return Json(new ReturnResult { Success = false, Message = errMsg });
            }
        }

        /// <summary>
        /// 重新加载用户快捷菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult ReloadUserQuckMenus()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table style=\"width:400px;line-height:50px;\">");
            List<Sys_Menu> userQuckMenus = SystemOperate.GetUserQuckMenus(currUser.UserId);
            if (userQuckMenus.Count > 0)
            {
                for (int i = 0; i < userQuckMenus.Count; i++)
                {
                    Sys_Menu menu = userQuckMenus[i];
                    int r = i / 3; //当前所在行
                    int c = i % 3; //当前所在列
                    if (i == 0)
                    {
                        sb.Append("<tr>");
                    }
                    else if (c == 0)
                    {
                        sb.Append("</tr><tr>");
                    }
                    string nodeJson = "{text:'" + (string.IsNullOrEmpty(menu.Display) ? menu.Name : menu.Display) + "',attribute:{url:'" + menu.Url.ObjToStr() + "',obj:{isNewWinOpen:" + menu.IsNewWinOpen.ToString().ToLower() + "";
                    if (menu.Sys_ModuleId.HasValue && menu.Sys_ModuleId.Value != Guid.Empty)
                    {
                        nodeJson += ",moduleId:'" + menu.Sys_ModuleId.Value.ToString() + "',moduleName:'" + menu.Sys_ModuleName + "'";
                    }
                    nodeJson += "}}}";
                    sb.AppendFormat("<td style=\"width:33%\"><a href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'{0}',plain:true\"  onclick=\"TreeNodeOnClick({2})\">{1}</a></td>", string.IsNullOrEmpty(menu.Icon) ? "eu-p2-icon-table" : menu.Icon, string.IsNullOrEmpty(menu.Display) ? menu.Name : menu.Display, nodeJson);
                }
                sb.Append("</tr>");
            }
            else
            {
                sb.Append("<tr><td><div style=\"height:100px;line-height:100px;text-align:center\">还没有添加快捷菜单！</div></td></tr>");
            }
            sb.Append("<tr><td colspan=\"2\"><a href=\"#\" onclick=\"SetQuckMenu();\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-p2-icon-tag_blue_add',plain:true\">添加快捷菜单</a></td></tr>");
            sb.Append("</table>");
            return Content(sb.ToString());
        }

        #endregion

        #region 图标处理

        /// <summary>
        /// 获取图标分页html
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIconPageHtml()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            int pageIndex = _Request["pageIndex"].ObjToInt();
            int pageSize = _Request["pageSize"].ObjToInt();
            int iconType = _Request["iconType"].ObjToInt();
            IconTypeEnum iconTypeOfEnum = (IconTypeEnum)Enum.Parse(typeof(IconTypeEnum), iconType.ToString());
            long total = 0;
            string html = SystemOperate.GetPageIconsHtml(out total, out pageSize, iconTypeOfEnum, pageIndex);
            return Content(html);
        }

        /// <summary>
        /// 判断图标样式类名是否已存在
        /// </summary>
        /// <returns></returns>
        public ActionResult IconStyleClassNameIsExists()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string errMsg = string.Empty;
            Guid recordId = _Request["recordId"].ObjToGuid(); //编辑时的记录Id
            string styleClassName = _Request["styleClassName"].ObjToStr(); //样式类名
            long count = CommonOperate.Count<Sys_IconManage>(out errMsg, false, x => x.Id != recordId && x.StyleClassName == styleClassName);
            return Json(new { IsNotExists = count == 0 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 增加自定义样式类
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCustomerStyleClass()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid recordId = _Request["recordId"].ObjToGuid();
            string errMsg = string.Empty;
            Sys_IconManage icon = CommonOperate.GetEntityById<Sys_IconManage>(recordId, out errMsg);
            if (icon == null || string.IsNullOrWhiteSpace(icon.StyleClassName))
                return Json(new ReturnResult { Success = false, Message = "不存在图标样式记录或样式名称为空！" });
            if (icon.IconClassOfEnum != IconClassTypeEnum.UserUploadIcon)
                return Json(new ReturnResult { Success = false, Message = "必须是用户上传图标！" });
            if (string.IsNullOrWhiteSpace(icon.IconAddr.Trim()))
                return Json(new ReturnResult { Success = false, Message = "样式图标为空！" });
            string iconFile = Server.MapPath(icon.IconAddr);
            if (!System.IO.File.Exists(iconFile))
                return Json(new ReturnResult { Success = false, Message = "样式图标文件不存在！" });
            try
            {
                StreamWriter sw = new StreamWriter(Server.MapPath("/Css/icon.css"), true, Encoding.UTF8);
                string cssContent = "." + icon.StyleClassName + " { background:url('" + icon.IconAddr + "') no-repeat center center;}";
                sw.WriteLine();
                sw.WriteLine(cssContent);
                sw.Close();
                return Json(new ReturnResult { Success = true, Message = string.Empty });
            }
            catch (Exception ex)
            {
                return Json(new ReturnResult { Success = false, Message = ex.Message });
            }
        }

        #endregion

        #region 权限处理

        #region 角色权限

        /// <summary>
        /// 保存用户角色
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveUserRole()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid userId = _Request["userId"].ObjToGuid();
            string roleIds = _Request["roleIds"].ObjToStr();
            if (userId == Guid.Empty || string.IsNullOrEmpty(roleIds))
                return Json(new ReturnResult { Success = false, Message = "用户Id或角色Id参数有误！" });
            string[] token = roleIds.Split(",".ToCharArray());
            List<Sys_UserRole> userRoles = new List<Sys_UserRole>();
            foreach (string str in token)
            {
                Guid roleId = str.ObjToGuid();
                if (roleId == Guid.Empty) continue;
                userRoles.Add(new Sys_UserRole() { Sys_UserId = userId, Sys_RoleId = roleId });
            }
            string errMsg = string.Empty;
            bool rs = CommonOperate.OperateRecords<Sys_UserRole>(userRoles, ModelRecordOperateType.Add, out errMsg, false);
            return Json(new ReturnResult { Success = rs, Message = errMsg });
        }

        /// <summary>
        /// 加载角色权限
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadRolePermission()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            Guid topMenuId = _Request["topMenuId"].ObjToGuid(); //顶部菜单Id
            Guid roleId = _Request["roleId"].ObjToGuid(); //角色Id
            if (roleId == Guid.Empty)
                return Json(null, JsonRequestBehavior.AllowGet);
            Sys_Role role = PermissionOperate.GetRole(roleId);
            if (role == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            List<Sys_Menu> topMenus = SystemOperate.GetTopMenus(true, currUser);
            if (topMenuId != Guid.Empty)
                topMenus = topMenus.Where(x => x.Id == topMenuId).ToList();
            List<object> list = new List<object>();
            //用户拥有的菜单权限Id集合
            Dictionary<Guid, bool> permissionMenuIds = PermissionOperate.GetRoleFunPermissions(currUser, roleId, FunctionTypeEnum.Menu);
            //用户拥有的列表按钮权限Id集合
            Dictionary<Guid, bool> permissionBtnIds = PermissionOperate.GetRoleFunPermissions(currUser, roleId, FunctionTypeEnum.Button);
            foreach (Sys_Menu topMenu in topMenus)
            {
                List<Sys_Menu> childMenus = SystemOperate.GetChildMenus(topMenu.Id, false, true, true, currUser);
                foreach (Sys_Menu childMenu in childMenus)
                {
                    string opPermissionStr = string.Empty; //操作权限
                    string canViewFieldsStr = string.Empty; //可查看字段
                    string canAddFieldsStr = string.Empty; //可新增字段
                    string canEditFieldsStr = string.Empty; //可编辑字段
                    string canViewDataStr = string.Empty; //可浏览数据
                    string canEditDataStr = string.Empty; //可编辑数据
                    string canDelDataStr = string.Empty; //可删除数据
                    if (childMenu.IsLeaf) //子节点菜单
                    {
                        string checkedStr = permissionMenuIds == null || permissionMenuIds.ContainsKey(childMenu.Id) ? "checked='checked'" : string.Empty;
                        string funTypeStr = "funType=\"0\"";
                        if (permissionMenuIds != null)
                        {
                            if (permissionMenuIds.ContainsKey(childMenu.Id) && permissionMenuIds[childMenu.Id])
                            {
                                checkedStr += " disabled='disabled'";
                                funTypeStr = string.Empty;
                            }
                        }
                        Guid tempModuleId = childMenu.Sys_ModuleId.HasValue ? childMenu.Sys_ModuleId.Value : Guid.Empty;
                        Sys_Module module = SystemOperate.GetModuleById(tempModuleId);
                        opPermissionStr += string.Format("<input {0} type=\"checkbox\" menuId=\"{1}\" moduleId=\"{3}\" value=\"{1}\"{2} /><span>浏览</span><input id=\"hd_module_{1}\" type=\"hidden\" value=\"{3}\" />", funTypeStr, childMenu.Id, checkedStr, module == null ? string.Empty : module.Id.ToString());
                        if (module != null)
                        {
                            #region 操作权限
                            //操作权限斌值
                            List<Sys_GridButton> btns = SystemOperate.GetGridButtons(module.Id);
                            int n = 0;
                            foreach (Sys_GridButton btn in btns)
                            {
                                n++;
                                string br = "&nbsp;&nbsp;";
                                if (n % 3 == 0) br = "</br>";
                                checkedStr = permissionBtnIds == null || permissionBtnIds.ContainsKey(btn.Id) ? "checked='checked'" : string.Empty;
                                funTypeStr = "funType=\"1\"";
                                if (permissionBtnIds != null)
                                {
                                    if (permissionBtnIds.ContainsKey(btn.Id) && permissionBtnIds[btn.Id])
                                    {
                                        checkedStr += " disabled='disabled'";
                                        funTypeStr = string.Empty;
                                    }
                                }
                                opPermissionStr += string.Format("{0}<input {1} type=\"checkbox\" moduleId=\"{5}\" menuId=\"{6}\" value=\"{2}\"{3} /><span>{4}</span>", br, funTypeStr, btn.Id.ToString(), checkedStr, btn.ButtonText, module.Id.ToString(), childMenu.Id.ToString());
                            }
                            #endregion
                            #region 字段权限
                            //可查看字段
                            string viewFieldImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】字段查看权限\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" roleId=\"{2}\" roleName=\"{3}\" menuId=\"{4}\" onclick=\"SetFieldPermission(this,0)\" />", module.Name, module.Id.ToString(), roleId.ToString(), role.Name, childMenu.Id.ToString());
                            Dictionary<string, bool> canViewFields = PermissionOperate.GetRoleFieldPermissions(roleId, module.Id, FieldPermissionTypeEnum.ViewField);
                            if (canViewFields.Count == 0) canViewFields.Add("-1", false);
                            canViewFieldsStr = string.Join(string.Empty, canViewFields.Select(x => canViewFields[x.Key] ? string.Format("<span name=\"viewField_p\" title=\"继承自父角色\">【{0}】</span>", x.Key == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x.Key)) : string.Format("<span name=\"viewField\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Key, x.Key != "2" ? (x.Key == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x.Key)) : "无", module.Id, childMenu.Id)).ToList());
                            if (!(canViewFields.ContainsKey("-1") && canViewFields["-1"])) //包含‘全部’权限并且继承自父角色时不允许编辑权限
                            {
                                canViewFieldsStr += viewFieldImgStr;
                            }
                            //可新增字段
                            if (module.IsAllowAdd)
                            {
                                string addFieldImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】字段新增权限\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" roleId=\"{2}\" roleName=\"{3}\" menuId=\"{4}\" onclick=\"SetFieldPermission(this,1)\" />", module.Name, module.Id, roleId, role.Name, childMenu.Id);
                                Dictionary<string, bool> canAddFields = PermissionOperate.GetRoleFieldPermissions(roleId, module.Id, FieldPermissionTypeEnum.AddField);
                                if (canAddFields.Count == 0) canAddFields.Add("-1", false);
                                canAddFieldsStr = string.Join(string.Empty, canAddFields.Select(x => canAddFields[x.Key] ? string.Format("<span name=\"addField_p\" title=\"继承自父角色\">【{0}】</span>", x.Key == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x.Key)) : string.Format("<span name=\"addField\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Key, x.Key != "2" ? (x.Key == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x.Key)) : "无", module.Id, childMenu.Id)).ToList());
                                if (!(canAddFields.ContainsKey("-1") && canAddFields["-1"])) //包含‘全部’权限并且继承自父角色时不允许编辑权限
                                {
                                    canAddFieldsStr += addFieldImgStr;
                                }
                            }
                            else
                            {
                                canAddFieldsStr = "<div style=\"width:100%;height:100%;text-align:center;\">不允许新增</div>";
                            }
                            //可编辑字段
                            if (module.IsAllowEdit)
                            {
                                string editFieldImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】字段编辑权限\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" roleId=\"{2}\" roleName=\"{3}\" menuId=\"{4}\" onclick=\"SetFieldPermission(this,2)\" />", module.Name, module.Id, roleId, role.Name, childMenu.Id);
                                Dictionary<string, bool> canEditFields = PermissionOperate.GetRoleFieldPermissions(roleId, module.Id, FieldPermissionTypeEnum.EditField);
                                if (canEditFields.Count == 0) canEditFields.Add("-1", false);
                                canEditFieldsStr = string.Join(string.Empty, canEditFields.Select(x => canEditFields[x.Key] ? string.Format("<span name=\"editField_p\" title=\"继承自父角色\">【{0}】</span>", x.Key == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x.Key)) : string.Format("<span name=\"editField\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Key, x.Key != "2" ? (x.Key == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x.Key)) : "无", module.Id, childMenu.Id)).ToList());
                                if (!(canEditFields.ContainsKey("-1") && canEditFields["-1"])) //包含‘全部’权限并且继承自父角色时不允许编辑权限
                                {
                                    canEditFieldsStr += editFieldImgStr;
                                }
                            }
                            else
                            {
                                canEditFieldsStr = "<div style=\"width:100%;height:100%;text-align:center;\">不允许编辑</div>";
                            }
                            #endregion
                            #region 数据权限
                            //数据浏览范围
                            Dictionary<string, bool> viewOrgIds = PermissionOperate.GetRoleDataPermissions(roleId, module.Id, DataPermissionTypeEnum.ViewData);
                            string viewDataImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】数据浏览权限范围\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" roleId=\"{2}\" roleName=\"{3}\" menuId=\"{4}\" onclick=\"SetDataPermissionRange(this,0)\" />", module.Name, module.Id, roleId, role.Name, childMenu.Id);
                            if (viewOrgIds.Count > 0)
                            {
                                List<TempOrganization> orgs = UserOperate.GetFormatOrgs(viewOrgIds.Keys.ToList());
                                canViewDataStr = string.Join(string.Empty, orgs.Select(x => viewOrgIds[x.Id] ? string.Format("<span name=\"viewData_p\" title=\"继承自父角色\">【{0}】</span>", x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部")) : string.Format("<span name=\"viewData\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Id, x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部"), module.Id.ToString(), childMenu.Id.ToString())));
                                if (!(orgs.Select(x => x.Id).ToList().Contains("-1") && viewOrgIds["-1"]))
                                {
                                    canViewDataStr += viewDataImgStr;
                                }
                            }
                            else //只能查看本人创建的单据
                            {
                                canViewDataStr = string.Format("<div style=\"width:100%;height:100%;text-align:center;\">【本人】{0}</div>", viewDataImgStr);
                            }
                            //数据编辑范围
                            if (module.IsAllowEdit)
                            {
                                Dictionary<string, bool> editOrgIds = PermissionOperate.GetRoleDataPermissions(roleId, module.Id, DataPermissionTypeEnum.EditData);
                                string editDataImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】数据编辑权限范围\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" rowId=\"{1}\" roleId=\"{2}\" roleName=\"{3}\" menuId=\"{4}\" onclick=\"SetDataPermissionRange(this,1)\" />", module.Name, module.Id, roleId, role.Name, childMenu.Id);
                                if (editOrgIds.Count > 0)
                                {
                                    List<TempOrganization> orgs = UserOperate.GetFormatOrgs(editOrgIds.Keys.ToList());
                                    canEditDataStr = string.Join(string.Empty, orgs.Select(x => editOrgIds[x.Id] ? string.Format("<span name=\"editData_p\" title=\"继承自父角色\">【{0}】</span>", x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部")) : string.Format("<span name=\"editData\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Id, x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部"), module.Id.ToString(), childMenu.Id.ToString())));
                                    if (!(orgs.Select(x => x.Id).ToList().Contains("-1") && editOrgIds["-1"]))
                                    {
                                        canEditDataStr += editDataImgStr;
                                    }
                                }
                                else //只能编辑本人创建的单据
                                {
                                    canEditDataStr = string.Format("<div style=\"width:100%;height:100%;text-align:center;\">【本人】{0}</div>", editDataImgStr);
                                }
                            }
                            else
                            {
                                canEditDataStr = "<div style=\"width:100%;height:100%;text-align:center;\">不允许编辑</div>";
                            }
                            //数据删除范围
                            if (module.IsAllowDelete)
                            {
                                Dictionary<string, bool> delOrgIds = PermissionOperate.GetRoleDataPermissions(roleId, module.Id, DataPermissionTypeEnum.DelData);
                                string delDataImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】数据删除权限范围\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" roleId=\"{2}\" roleName=\"{3}\" menuId=\"{4}\" onclick=\"SetDataPermissionRange(this,2)\" />", module.Name, module.Id, roleId, role.Name, childMenu.Id);
                                if (delOrgIds.Count > 0)
                                {
                                    List<TempOrganization> orgs = UserOperate.GetFormatOrgs(delOrgIds.Keys.ToList());
                                    canDelDataStr = string.Join(string.Empty, orgs.Select(x => delOrgIds[x.Id] ? string.Format("<span name=\"delData_p\" title=\"继承自父角色\">【{0}】</span>", x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部")) : string.Format("<span name=\"delData\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Id, x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部"), module.Id.ToString(), childMenu.Id.ToString())));
                                    if (!(orgs.Select(x => x.Id).ToList().Contains("-1") && delOrgIds["-1"]))
                                    {
                                        canDelDataStr += delDataImgStr;
                                    }
                                }
                                else //只能删除本人创建的单据
                                {
                                    canDelDataStr = string.Format("<div style=\"width:100%;height:100%;text-align:center;\">【本人】{0}</div>", delDataImgStr);
                                }
                            }
                            else
                            {
                                canDelDataStr = "<div style=\"width:100%;height:100%;text-align:center;\">不允许删除</div>";
                            }
                            #endregion
                        }
                    }
                    list.Add(new
                    {
                        MenuId = childMenu.Id,
                        BigModule = topMenu.Display,
                        MenuDisplay = childMenu.Display,
                        OpPermisson = opPermissionStr,
                        CanViewFields = canViewFieldsStr,
                        CanAddFields = canAddFieldsStr,
                        CanEditFields = canEditFieldsStr,
                        CanViewData = canViewDataStr,
                        CanEditData = canEditDataStr,
                        CanDelData = canDelDataStr,
                        ParentId = childMenu.ParentId.Value,
                        iconCls = childMenu.Icon
                    });
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveRolePermission()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid roleId = _Request["roleId"].ObjToGuid();
            Guid? topMenuId = _Request["topMenuId"].ObjToGuidNull();
            List<PermissionModel> permissionData = JsonHelper.Deserialize<List<PermissionModel>>(HttpUtility.UrlDecode(MySecurity.DecodeBase64(_Request["powerData"].ObjToStr()), Encoding.UTF8));
            string errMsg = PermissionOperate.SaveRolePermission(roleId, permissionData, topMenuId, GetCurrentUser(_Request));
            return Json(new ReturnResult { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        #endregion

        #region 用户权限

        /// <summary>
        /// 加载用户权限
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadUserPermission()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            Guid topMenuId = _Request["topMenuId"].ObjToGuid(); //顶部菜单Id
            Guid userId = _Request["userId"].ObjToGuid(); //用户Id
            if (userId == Guid.Empty)
                return Json(null, JsonRequestBehavior.AllowGet);
            Sys_User user = UserOperate.GetUser(userId);
            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            List<Sys_Menu> topMenus = SystemOperate.GetTopMenus(true, currUser);
            if (topMenuId != Guid.Empty)
                topMenus = topMenus.Where(x => x.Id == topMenuId).ToList();
            List<object> list = new List<object>();
            //用户拥有的菜单权限Id集合
            List<Guid> permissionMenuIds = PermissionOperate.GetUserFunPermissions(userId, FunctionTypeEnum.Menu);
            //用户拥有的列表按钮权限Id集合
            List<Guid> permissionBtnIds = PermissionOperate.GetUserFunPermissions(userId, FunctionTypeEnum.Button);
            foreach (Sys_Menu topMenu in topMenus)
            {
                List<Sys_Menu> childMenus = SystemOperate.GetChildMenus(topMenu.Id, false, true, true, currUser);
                foreach (Sys_Menu childMenu in childMenus)
                {
                    string opPermissionStr = string.Empty; //操作权限
                    string canViewFieldsStr = string.Empty; //可查看字段
                    string canAddFieldsStr = string.Empty; //可新增字段
                    string canEditFieldsStr = string.Empty; //可编辑字段
                    string canViewDataStr = string.Empty; //可浏览数据
                    string canEditDataStr = string.Empty; //可编辑数据
                    string canDelDataStr = string.Empty; //可删除数据
                    if (childMenu.IsLeaf) //子节点菜单
                    {
                        string checkedStr = permissionMenuIds == null || permissionMenuIds.Contains(childMenu.Id) ? "checked='checked'" : string.Empty;
                        string funTypeStr = "funType=\"0\"";
                        Guid tempModuleId = childMenu.Sys_ModuleId.HasValue ? childMenu.Sys_ModuleId.Value : Guid.Empty;
                        Sys_Module module = SystemOperate.GetModuleById(tempModuleId);
                        opPermissionStr += string.Format("<input {0} type=\"checkbox\" menuId=\"{1}\" moduleId=\"{3}\" value=\"{1}\"{2} /><span>浏览</span><input id=\"hd_module_{1}\" type=\"hidden\" value=\"{3}\" />", funTypeStr, childMenu.Id, checkedStr, module == null ? string.Empty : module.Id.ToString());
                        if (module != null)
                        {
                            #region 操作权限
                            //操作权限斌值
                            List<Sys_GridButton> btns = SystemOperate.GetGridButtons(module.Id);
                            int n = 0;
                            foreach (Sys_GridButton btn in btns)
                            {
                                n++;
                                string br = "&nbsp;&nbsp;";
                                if (n % 3 == 0) br = "</br>";
                                checkedStr = permissionMenuIds == null || permissionBtnIds.Contains(btn.Id) ? "checked='checked'" : string.Empty;
                                funTypeStr = "funType=\"1\"";
                                opPermissionStr += string.Format("{0}<input {1} type=\"checkbox\" moduleId=\"{5}\" menuId=\"{6}\" value=\"{2}\"{3} /><span>{4}</span>", br, funTypeStr, btn.Id, checkedStr, btn.ButtonText, module.Id, childMenu.Id);
                            }
                            #endregion
                            #region 字段权限
                            //可查看字段
                            string viewFieldImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】字段查看权限\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" userId=\"{2}\" userName=\"{3}\" menuId=\"{4}\" onclick=\"SetFieldPermission(this,0)\" />", module.Name, module.Id.ToString(), userId.ToString(), user.UserName, childMenu.Id.ToString());
                            List<string> canViewFields = PermissionOperate.GetUserFieldsPermissions(userId, module.Id, FieldPermissionTypeEnum.ViewField);
                            if (canViewFields.Count == 0) canViewFields.Add("-1");
                            canViewFieldsStr = string.Join(string.Empty, canViewFields.Select(x => string.Format("<span name=\"viewField\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x, x != "-2" ? (x == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x)) : "无", module.Id, childMenu.Id)).ToList());
                            canViewFieldsStr += viewFieldImgStr;
                            //可新增字段
                            if (module.IsAllowAdd)
                            {
                                string addFieldImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】字段新增权限\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" userId=\"{2}\" userName=\"{3}\" menuId=\"{4}\" onclick=\"SetFieldPermission(this,1)\" />", module.Name, module.Id.ToString(), userId.ToString(), user.UserName, childMenu.Id.ToString());
                                List<string> canAddFields = PermissionOperate.GetUserFieldsPermissions(userId, module.Id, FieldPermissionTypeEnum.AddField);
                                if (canAddFields.Count == 0) canAddFields.Add("-1");
                                canAddFieldsStr = string.Join(string.Empty, canAddFields.Select(x => string.Format("<span name=\"addField\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x, x != "-2" ? (x == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x)) : "无", module.Id, childMenu.Id)).ToList());
                                canAddFieldsStr += addFieldImgStr;
                            }
                            else
                            {
                                canAddFieldsStr = "<div style=\"width:100%;height:100%;text-align:center;\">不允许新增</div>";
                            }
                            //可编辑字段
                            if (module.IsAllowEdit)
                            {
                                string editFieldImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】字段编辑权限\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" userId=\"{2}\" userName=\"{3}\" menuId=\"{4}\" onclick=\"SetFieldPermission(this,2)\" />", module.Name, module.Id.ToString(), userId.ToString(), user.UserName, childMenu.Id.ToString());
                                List<string> canEditFields = PermissionOperate.GetUserFieldsPermissions(userId, module.Id, FieldPermissionTypeEnum.EditField);
                                if (canEditFields.Count == 0) canEditFields.Add("-1");
                                canEditFieldsStr = string.Join(string.Empty, canEditFields.Select(x => string.Format("<span name=\"editField\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x, x != "-2" ? (x == "-1" ? "全部" : SystemOperate.GetFieldDisplay(module.Id, x)) : "无", module.Id.ToString(), childMenu.Id.ToString())).ToList());
                                canEditFieldsStr += editFieldImgStr;
                            }
                            else
                            {
                                canEditFieldsStr = "<div style=\"width:100%;height:100%;text-align:center;\">不允许编辑</div>";
                            }
                            #endregion
                            #region 数据权限
                            //数据浏览范围
                            List<string> viewOrgIds = PermissionOperate.GetUserDataPermissions(userId, module.Id, DataPermissionTypeEnum.ViewData);
                            string viewDataImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】数据浏览权限范围\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" userId=\"{2}\" userName=\"{3}\" menuId=\"{4}\" onclick=\"SetDataPermissionRange(this,0)\" />", module.Name, module.Id.ToString(), userId.ToString(), user.UserName, childMenu.Id.ToString());
                            if (viewOrgIds.Count > 0)
                            {
                                List<TempOrganization> orgs = UserOperate.GetFormatOrgs(viewOrgIds.ToList());
                                canViewDataStr = string.Join(string.Empty, orgs.Select(x => string.Format("<span name=\"viewData\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Id, x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部"), module.Id.ToString(), childMenu.Id.ToString())));
                                canViewDataStr += viewDataImgStr;
                            }
                            else //只能查看本人创建的单据
                            {
                                canViewDataStr = string.Format("<div style=\"width:100%;height:100%;text-align:center;\">【本人】{0}</div>", viewDataImgStr);
                            }
                            //数据编辑范围
                            if (module.IsAllowEdit)
                            {
                                List<string> editOrgIds = PermissionOperate.GetUserDataPermissions(userId, module.Id, DataPermissionTypeEnum.EditData);
                                string editDataImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】数据编辑权限范围\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" rowId=\"{1}\" userId=\"{2}\" userName=\"{3}\" menuId=\"{4}\" onclick=\"SetDataPermissionRange(this,1)\" />", module.Name, module.Id.ToString(), userId.ToString(), user.UserName, childMenu.Id.ToString());
                                if (editOrgIds.Count > 0)
                                {
                                    List<TempOrganization> orgs = UserOperate.GetFormatOrgs(editOrgIds.ToList());
                                    canEditDataStr = string.Join(string.Empty, orgs.Select(x => string.Format("<span name=\"editData\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Id, x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部"), module.Id.ToString(), childMenu.Id.ToString())));
                                    canEditDataStr += editDataImgStr;
                                }
                                else //只能编辑本人创建的单据
                                {
                                    canEditDataStr = string.Format("<div style=\"width:100%;height:100%;text-align:center;\">【本人】{0}</div>", editDataImgStr);
                                }
                            }
                            else
                            {
                                canEditDataStr = "<div style=\"width:100%;height:100%;text-align:center;\">不允许编辑</div>";
                            }
                            //数据删除范围
                            if (module.IsAllowDelete)
                            {
                                List<string> delOrgIds = PermissionOperate.GetUserDataPermissions(userId, module.Id, DataPermissionTypeEnum.DelData);
                                string delDataImgStr = string.Format("&nbsp;&nbsp;<img name=\"setPower\" style=\"cursor:pointer;\" title=\"设置【{0}】数据删除权限范围\" src=\"/Css/icons/docEdit.png\" moduleName=\"{0}\" moduleId=\"{1}\" userId=\"{2}\" userName=\"{3}\" menuId=\"{4}\" onclick=\"SetDataPermissionRange(this,2)\" />", module.Name, module.Id.ToString(), userId.ToString(), user.UserName, childMenu.Id.ToString());
                                if (delOrgIds.Count > 0)
                                {
                                    List<TempOrganization> orgs = UserOperate.GetFormatOrgs(delOrgIds.ToList());
                                    canDelDataStr = string.Join(string.Empty, orgs.Select(x => string.Format("<span name=\"delData\" value=\"{0}\" moduleId=\"{2}\" menuId=\"{3}\">【{1}】</span>", x.Id, x.Id != "-1" ? x.Name : (x.Id == "0" ? "本部门" : "全部"), module.Id.ToString(), childMenu.Id.ToString())));
                                    canDelDataStr += delDataImgStr;
                                }
                                else //只能删除本人创建的单据
                                {
                                    canDelDataStr = string.Format("<div style=\"width:100%;height:100%;text-align:center;\">【本人】{0}</div>", delDataImgStr);
                                }
                            }
                            else
                            {
                                canDelDataStr = "<div style=\"width:100%;height:100%;text-align:center;\">不允许删除</div>";
                            }
                            #endregion
                        }
                    }
                    list.Add(new
                    {
                        MenuId = childMenu.Id,
                        BigModule = topMenu.Display,
                        MenuDisplay = childMenu.Display,
                        OpPermisson = opPermissionStr,
                        CanViewFields = canViewFieldsStr,
                        CanAddFields = canAddFieldsStr,
                        CanEditFields = canEditFieldsStr,
                        CanViewData = canViewDataStr,
                        CanEditData = canEditDataStr,
                        CanDelData = canDelDataStr,
                        ParentId = childMenu.ParentId.Value,
                        iconCls = childMenu.Icon
                    });
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存用户权限
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveUserPermission()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid userId = _Request["userId"].ObjToGuid();
            Guid? topMenuId = _Request["topMenuId"].ObjToGuidNull();
            List<PermissionModel> permissionData = JsonHelper.Deserialize<List<PermissionModel>>(HttpUtility.UrlDecode(MySecurity.DecodeBase64(_Request["powerData"].ObjToStr()), Encoding.UTF8));
            string errMsg = PermissionOperate.SaveUserPermission(userId, permissionData, topMenuId, GetCurrentUser(_Request));
            return Json(new ReturnResult { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        /// <summary>
        /// 清除用户权限
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearUserPermission()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            Guid userId = _Request["userId"].ObjToGuid();
            string errMsg = string.Empty;
            //删除全部功能权限
            bool rs = CommonOperate.DeleteRecordsByExpression<Sys_UserPermissionFun>(x => x.Sys_UserId == userId, out errMsg);
            if (!rs && !string.IsNullOrEmpty(errMsg))
                return Json(new ReturnResult() { Success = false, Message = errMsg });
            //删除全部字段权限
            rs = CommonOperate.DeleteRecordsByExpression<Sys_UserPermissionField>(x => x.Sys_UserId == userId, out errMsg);
            if (!rs && !string.IsNullOrEmpty(errMsg))
                return Json(new ReturnResult() { Success = false, Message = errMsg });
            //删除全部数据权限
            rs = CommonOperate.DeleteRecordsByExpression<Sys_UserPermissionData>(x => x.Sys_UserId == userId, out errMsg);
            if (!rs && !string.IsNullOrEmpty(errMsg))
                return Json(new ReturnResult() { Success = false, Message = errMsg });
            return Json(new ReturnResult() { Success = true, Message = string.Empty });
        }

        #endregion

        #endregion

        #region 其他处理

        /// <summary>
        /// 加载查询方法枚举列表
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadQueryMethodEnumList()
        {
            return Json(SystemOperate.GetEnumTypeList(typeof(ConditionMethodEnum)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshCache()
        {
            if (_Request == null) _Request = Request;
            int type = _Request["type"].ObjToInt();
            if (type != 1 && type != 2)
                return Json(new ReturnResult() { Success = false, Message = "参数传递不正确" });
            string errMsg = string.Empty;
            if (type == 1) //刷新模块缓存
            {
                Sys_Module module = SystemOperate.GetModuleByRequest(_Request);
                string tableName = module != null ? module.TableName : _Request["moduleName"].ObjToStr();
                if (string.IsNullOrEmpty(tableName))
                    return Json(new ReturnResult() { Success = false, Message = "模块不存在" });
                if (module != null && tableName == typeof(Sys_GridField).Name)
                    CommonOperate.UpdateRecordsByExpression<Sys_GridField>(new { FieldFormatter = string.Empty, EditorFormatter = string.Empty }, null, out errMsg);
                CommonOperate.ClearCache(tableName);
            }
            else //刷新所有缓存
            {
                CommonOperate.UpdateRecordsByExpression<Sys_GridField>(new { FieldFormatter = string.Empty, EditorFormatter = string.Empty }, null, out errMsg);
                CommonOperate.ClearAllCache();
            }
            return Json(new ReturnResult() { Success = true, Message = string.Empty });
        }

        #endregion
    }
}

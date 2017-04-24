/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Model.Msg;
using Rookey.Frame.Model.OrgM;
using Rookey.Frame.Common;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Operate.Base.EnumDef;
using System.Text;
using Rookey.Frame.Common.Model;
using Rookey.Frame.Base;
using Rookey.Frame.Common.PubDefine;
using System.IO;
using Rookey.Frame.Model.Desktop;
using Rookey.Frame.EntityBase;
using System.Data;
using ServiceStack.DataAnnotations;
using Rookey.Frame.Operate.Base.OperateHandle;
using System.Threading.Tasks;
using Rookey.Frame.Base.Set;
using Rookey.Frame.Email;
using Rookey.Frame.Model.Bpm;
using System.Collections.Concurrent;

namespace Rookey.Frame.Operate.Base
{
    /// <summary>
    /// 系统操作类
    /// </summary>
    public static class SystemOperate
    {
        #region 模块

        #region 基本

        #region 基本信息

        /// <summary>
        /// 取模块集合
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_Module> GetModules(Expression<Func<Sys_Module, bool>> expression = null)
        {
            string errMsg = string.Empty;
            return CommonOperate.GetEntities<Sys_Module>(out errMsg, expression);
        }

        /// <summary>
        /// 通过模块Id获取模块信息
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static Sys_Module GetModuleById(Guid moduleId)
        {
            string errMsg = string.Empty;
            return CommonOperate.GetEntityById<Sys_Module>(moduleId, out errMsg);
        }

        /// <summary>
        /// 通过模块名称获取模块信息
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public static Sys_Module GetModuleByName(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName)) return null;
            string errMsg = string.Empty;
            Sys_Module module = CommonOperate.GetEntity<Sys_Module>(x => x.Name == moduleName, string.Empty, out errMsg);
            return module;
        }

        /// <summary>
        /// 根据表名获取模块
        /// </summary>
        /// <param name="tableName">实体表名</param>
        /// <returns></returns>
        public static Sys_Module GetModuleByTableName(string tableName)
        {
            string errMsg = string.Empty;
            Sys_Module module = CommonOperate.GetEntity<Sys_Module>(x => x.TableName == tableName, string.Empty, out errMsg);
            return module;
        }

        /// <summary>
        /// 根据表名获取模块Id
        /// </summary>
        /// <param name="tableName">实体表名</param>
        /// <returns></returns>
        public static Guid GetModuleIdByTableName(string tableName)
        {
            Sys_Module module = GetModuleByTableName(tableName);
            if (module != null) return module.Id;
            return Guid.Empty;
        }

        /// <summary>
        /// 根据模块名称获取模块Id
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public static Guid GetModuleIdByName(string moduleName)
        {
            string errMsg = string.Empty;
            Sys_Module module = CommonOperate.GetEntity<Sys_Module>(x => x.Name == moduleName, string.Empty, out errMsg);
            if (module != null)
            {
                return module.Id;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 根据模块Id获取模块名称
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static string GetModuleNameById(Guid moduleId)
        {
            string errMsg = string.Empty;
            Sys_Module module = CommonOperate.GetEntityById<Sys_Module>(moduleId, out errMsg);
            if (module != null)
            {
                return module.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取模块显示名称
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public static string GetModuleDiplay(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            if (module != null)
            {
                if (string.IsNullOrEmpty(module.Display))
                    return module.Name;
                return module.Display;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取模块表名
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static string GetModuleTableNameById(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            if (module != null)
            {
                return module.TableName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取模块表名
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public static string GetModuleTableNameByName(string moduleName)
        {
            Sys_Module module = GetModuleByName(moduleName);
            if (module != null)
            {
                return module.TableName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="request">request请求</param>
        /// <returns></returns>
        public static Sys_Module GetModuleByRequest(HttpRequestBase request)
        {
            Guid moduleId = request["moduleId"].ObjToGuid();
            string moduleName = HttpUtility.UrlDecode(request["moduleName"].ObjToStr());
            Sys_Module module = moduleId != Guid.Empty ? GetModuleById(moduleId) : GetModuleByName(moduleName);
            if (module == null)
            {
                string tableName = request["tableName"].ObjToStr();
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    module = GetModuleByTableName(tableName);
                }
            }
            return module;
        }

        /// <summary>
        /// 通过实体类型获取模块
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <returns></returns>
        public static Guid GetModuleIdByModelType(Type modelType)
        {
            if (modelType != null)
                return GetModuleIdByTableName(modelType.Name);
            else
                return Guid.Empty;
        }

        #endregion

        #region 模块自定义js

        /// <summary>
        /// 获取模块的Js文件路径
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="isFullPath">是否显示全路径</param>
        /// <returns></returns>
        public static string GetModuleJsFilePath(Guid moduleId, bool isFullPath = false)
        {
            Sys_Module module = GetModuleById(moduleId);
            return GetModuleJsFilePath(module);
        }

        /// <summary>
        /// 获取模块的Js文件路径
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="isFullPath">是否显示全路径</param>
        /// <returns></returns>
        public static string GetModuleJsFilePath(Sys_Module module, bool isFullPath = false)
        {
            string jsPath = string.Empty;
            if (module != null && !string.IsNullOrEmpty(module.TableName))
            {
                if (!string.IsNullOrEmpty(module.StandardJsFolder))
                {
                    jsPath = string.Format("Scripts/model/{0}/{1}.js", module.StandardJsFolder, module.TableName);
                }
                else
                {
                    jsPath = string.Format("Scripts/model/{0}.js", module.TableName);
                }
                string filePath = Globals.GetWebDir() + jsPath.Replace("/", "\\");
                if (!System.IO.File.Exists(filePath)) //js不存在
                {
                    return string.Empty;
                }
                try
                {
                    FileInfo fi = new FileInfo(filePath);
                    string r = fi.LastWriteTime.ToString("yyMMddHHmmss");
                    jsPath += string.Format("?r={0}", r);
                }
                catch { }
                string domainPath = isFullPath ? Globals.GetBaseUrl() : "/";
                jsPath = domainPath + jsPath;
            }
            return jsPath;
        }

        #endregion

        #region 外键模块

        /// <summary>
        /// 取外键字段的外键模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static Sys_Module GetForeignModule(Guid moduleId, string fieldName)
        {
            Sys_Field field = GetFieldInfo(moduleId, fieldName);
            if (field != null)
            {
                Sys_Module module = GetModuleByName(field.ForeignModuleName);
                return module;
            }
            return null;
        }

        /// <summary>
        /// 获取外键模块
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static Sys_Module GetForeignModule(Sys_Field field)
        {
            if (field != null)
            {
                Sys_Module module = GetModuleByName(field.ForeignModuleName);
                return module;
            }
            return null;
        }

        /// <summary>
        /// 获取外键模块Id
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static Guid GetForeignModuleId(Sys_Field field)
        {
            if (field != null)
            {
                Sys_Module module = GetModuleByName(field.ForeignModuleName);
                return module.Id;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 获取字段外键模块名称
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>返回该模块对应的外键模块名称</returns>
        public static string GetForeignModuleName(Guid moduleId, string fieldName)
        {
            Sys_Field field = GetFieldInfo(moduleId, fieldName);
            if (field != null)
            {
                return field.ForeignModuleName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取模块的所有外键模块（除用户管理模块）
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Module> GetForeignModules(Guid moduleId)
        {
            List<Sys_Field> fields = SystemOperate.GetFieldInfos(moduleId);
            List<string> tempModuleNames = new List<string>();
            foreach (Sys_Field field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.ForeignModuleName))
                    continue;
                if (CommonDefine.BaseEntityFields.Contains(field.Name) || tempModuleNames.Contains(field.ForeignModuleName.Trim()))
                    continue;
                tempModuleNames.Add(field.ForeignModuleName.Trim());
            }
            List<Sys_Module> modules = new List<Sys_Module>();
            foreach (string moduleName in tempModuleNames)
            {
                Sys_Module tempModule = SystemOperate.GetModuleByName(moduleName);
                if (tempModule != null)
                    modules.Add(tempModule);
            }
            return modules;
        }

        /// <summary>
        /// 获取模块的所有外键模块（除用户管理模块）
        /// </summary>
        /// <param name="module">模块对象</param>
        /// <returns></returns>
        public static List<Sys_Module> GetForeignModules(Sys_Module module)
        {
            return GetForeignModules(module.Id);
        }

        /// <summary>
        /// 获取模块的所有字段外键模块不重复的外键模块，
        /// 即模块字段中不允许有两个字段是同一个外键模块
        /// 并且不包含用户管理外键模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Module> GetNoRepeatForeignModules(Guid moduleId)
        {
            List<Sys_Field> fields = SystemOperate.GetFieldInfos(moduleId);
            List<string> tempModuleNames = new List<string>(); //可用外键模块名称集合
            List<string> removeModuleNames = new List<string>(); //需要移除的
            foreach (Sys_Field field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.ForeignModuleName))
                    continue;
                if (CommonDefine.BaseEntityFields.Contains(field.Name))
                    continue;
                if (tempModuleNames.Contains(field.ForeignModuleName.Trim())) //已经包含了该模块
                {
                    if (!removeModuleNames.Contains(field.ForeignModuleName.Trim()))
                    {
                        removeModuleNames.Add(field.ForeignModuleName.Trim()); //添加到移除列表
                    }
                }
                else
                {
                    tempModuleNames.Add(field.ForeignModuleName.Trim());
                }
            }
            //移除重复项
            foreach (string name in removeModuleNames)
            {
                tempModuleNames.Remove(name);
            }
            List<Sys_Module> modules = new List<Sys_Module>();
            foreach (string moduleName in tempModuleNames)
            {
                Sys_Module tempModule = SystemOperate.GetModuleByName(moduleName);
                if (tempModule != null)
                    modules.Add(tempModule);
            }
            return modules;
        }

        #endregion

        #region 模块标记字段

        /// <summary>
        /// 获取模块的TitleKey
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public static string GetModuleTitleKey(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName)) return string.Empty;
            Sys_Module module = GetModuleByName(moduleName);
            if (module != null)
            {
                return module.TitleKey;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取模块的TitleKey
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static string GetModuleTitleKey(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            if (module != null)
            {
                return module.TitleKey;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取模块的TitleKey的显示名称
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static string GetModuleTitleKeyDisplay(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            return GetModuleTitleKeyDisplay(module);
        }

        /// <summary>
        /// 获取模块的TitleKey的显示名称
        /// </summary>
        /// <param name="module">模块对象</param>
        /// <returns></returns>
        public static string GetModuleTitleKeyDisplay(Sys_Module module)
        {
            if (module != null && !string.IsNullOrWhiteSpace(module.TitleKey))
            {
                if (module.TitleKeyDisplay != null) return module.TitleKeyDisplay;
                Sys_Field field = GetFieldInfo(module.Id, module.TitleKey);
                if (field != null)
                {
                    string display = field.Display.ObjToStr();
                    module.TitleKeyDisplay = display;
                    return display;
                }
            }
            return string.Empty;
        }

        #endregion

        #region 模块主键字段

        /// <summary>
        /// 取模块的主键字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<string> GetModulePrimaryKeyFields(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            if (module != null && !string.IsNullOrWhiteSpace(module.PrimaryKeyFields))
            {
                return module.PrimaryKeyFields.Split(",".ToCharArray()).ToList();
            }
            return new List<string>();
        }

        #endregion

        #region 明细相关

        /// <summary>
        /// 是否为明细模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static bool IsDetailModule(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            if (module != null)
            {
                return module.ParentId.HasValue && module.ParentId.Value != Guid.Empty;
            }
            return false;
        }

        /// <summary>
        /// 获取所有明细模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Module> GetDetailModules(Guid moduleId)
        {
            string errMsg = string.Empty;
            Sys_Module module = GetModuleById(moduleId);
            if (module != null)
            {
                Guid parentId = module.Id;
                List<Sys_Module> detailModules = CommonOperate.GetEntities<Sys_Module>(out errMsg, x => x.ParentId == parentId, string.Empty, false, new List<string>() { "Sort" }, new List<bool>() { false });
                if (detailModules == null) detailModules = new List<Sys_Module>();
                return detailModules;
            }
            return new List<Sys_Module>();
        }

        /// <summary>
        /// 是否有明细模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static bool HasDetailModule(Guid moduleId)
        {
            List<Sys_Module> detailModules = GetDetailModules(moduleId);
            return detailModules != null && detailModules.Count > 0;
        }

        /// <summary>
        /// 获取父模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static Sys_Module GetParentModule(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            if (module != null && module.ParentId.HasValue && module.ParentId.Value != Guid.Empty)
            {
                return GetModuleById(module.ParentId.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取父模块
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static Guid GetParentModuleId(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            if (module != null && module.ParentId.HasValue && module.ParentId.Value != Guid.Empty)
            {
                Sys_Module parentModule = GetModuleById(module.ParentId.Value);
                if (parentModule != null)
                    return parentModule.Id;
            }
            return Guid.Empty;
        }

        #endregion

        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static Type GetModelType(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            if (module != null)
            {
                return CommonOperate.GetModelType(module.TableName);
            }
            return null;
        }

        #endregion

        #region 附属模块

        /// <summary>
        /// 获取模块的附属模块集合
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Module> GetAttachModules(Guid moduleId)
        {
            List<Sys_Module> list = new List<Sys_Module>();
            //取模块信息
            Sys_Module module = GetModuleById(moduleId);
            //取模块字段
            List<Sys_Field> fields = GetFields(x => x.Sys_ModuleId != moduleId && x.ForeignModuleName == module.Name && !x.IsDeleted);
            //取外键模块
            if (fields != null && fields.Count > 0)
            {
                fields = fields.Where(x => !CommonDefine.BaseEntityFields.Contains(x.Name)).ToList();
                List<Guid> hasAdd = new List<Guid>(); //已添加的moduleId
                foreach (Sys_Field field in fields)
                {
                    if (!field.Sys_ModuleId.HasValue) continue;
                    Sys_Module attachModule = GetModuleById(field.Sys_ModuleId.Value);
                    if (attachModule == null || hasAdd.Contains(attachModule.Id) || (attachModule.ParentId.HasValue && attachModule.ParentId.Value != Guid.Empty))
                        continue;
                    list.Add(attachModule);
                    hasAdd.Add(attachModule.Id);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取附属模块绑定集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_AttachModuleBind> GetAttachModuleBind(Guid userId, Guid moduleId)
        {
            Sys_Module currentModule = GetModuleById(moduleId);
            if (currentModule == null) return new List<Sys_AttachModuleBind>();
            string errMsg = string.Empty;
            List<Sys_AttachModuleBind> attachBinds = CommonOperate.GetEntities<Sys_AttachModuleBind>(out errMsg, x => x.Sys_UserId == userId && x.ModuleName == currentModule.Name && !x.IsDeleted, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (attachBinds == null) attachBinds = new List<Sys_AttachModuleBind>();
            return attachBinds;
        }

        /// <summary>
        /// 取用户绑定的附属模块集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Module> GetUserBindAttachModules(Guid userId, Guid moduleId)
        {
            List<Sys_AttachModuleBind> attachBinds = GetAttachModuleBind(userId, moduleId).Where(x => x.IsValid).ToList();
            if (attachBinds.Count > 0)
            {
                List<Guid> attachModuleIds = attachBinds.Select(x => x.Sys_ModuleId.Value).ToList();
                List<Sys_Module> attachModules = GetAttachModules(moduleId);
                var tempList = attachModules.Where(x => attachModuleIds.Contains(x.Id)).ToList();
                return tempList;
            }
            return new List<Sys_Module>();
        }

        /// <summary>
        /// 获取用户绑定的附属模块集合
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="isInnerGrid">是否网格内绑定的附属模块</param>
        /// <returns></returns>
        public static List<Sys_Module> GetUserBindAttachModules(Guid userId, Guid moduleId, bool isInnerGrid)
        {
            List<Sys_AttachModuleBind> attachBinds = GetAttachModuleBind(userId, moduleId).Where(x => x.IsValid).ToList();
            if (isInnerGrid)
            {
                attachBinds = attachBinds.Where(x => x.AttachModuleInGrid).ToList();
            }
            else
            {
                attachBinds = attachBinds.Where(x => !x.AttachModuleInGrid).ToList();
            }
            if (attachBinds.Count > 0)
            {
                List<Guid> attachModuleIds = attachBinds.Select(x => x.Sys_ModuleId.Value).ToList();
                List<Sys_Module> list = new List<Sys_Module>();
                foreach (Guid id in attachModuleIds)
                {
                    list.Add(GetModuleById(id));
                }
                return list;
            }
            return new List<Sys_Module>();
        }

        /// <summary>
        /// 用户是否绑定了附属模块
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="isInnerGrid">是否网格内绑定的附属模块</param>
        /// <returns></returns>
        public static bool HasUserAttachModule(Guid userId, Guid moduleId, bool isInnerGrid)
        {
            return GetUserBindAttachModules(userId, moduleId, isInnerGrid).Count > 0;
        }

        #endregion

        #region 其他

        /// <summary>
        /// 根据视图Id获取模块Id
        /// </summary>
        /// <param name="viewId">视图Id</param>
        /// <returns></returns>
        public static Sys_Module GetModuleByViewId(Guid viewId)
        {
            Sys_Grid grid = GetGrid(viewId);
            if (grid != null && grid.Sys_ModuleId.HasValue)
                return GetModuleById(grid.Sys_ModuleId.Value);
            return null;
        }

        /// <summary>
        /// 删除模块相关的数据，删除对应的字段、表单、表单字段、列表、列表字段、列表按钮、字典绑定等信息
        /// </summary>
        /// <param name="t">模块对象</param>
        public static void DeleteModuleReferences(Sys_Module t)
        {
            if (t == null || !t.IsCustomerModule) return;
            string errMsg = string.Empty;
            List<Sys_Field> sysFields = SystemOperate.GetFieldInfos(t.Id);
            if (sysFields.Count > 0)
            {
                List<Guid?> sysFieldIds = sysFields.Select(x => (Guid?)x.Id).ToList();
                //删除字段信息
                CommonOperate.DeleteRecordsByExpression<Sys_Field>(x => x.Sys_ModuleId == t.Id, out errMsg);
                //删除表单
                List<Sys_Form> forms = CommonOperate.GetEntities<Sys_Form>(out errMsg, x => x.Sys_ModuleId == t.Id, null, false);
                List<Guid?> formIds = forms == null ? new List<Guid?>() : forms.Select(x => (Guid?)x.Id).ToList();
                CommonOperate.DeleteRecordsByExpression<Sys_Form>(x => x.Sys_ModuleId == t.Id, out errMsg);
                //删除角色表单
                CommonOperate.DeleteRecordsByExpression<Sys_RoleForm>(x => formIds.Contains(x.Sys_FormId), out errMsg);
                //删除表单字段
                CommonOperate.DeleteRecordsByExpression<Sys_FormField>(x => sysFieldIds.Contains(x.Sys_FieldId), out errMsg);
                //删除视图
                List<Sys_Grid> grids = CommonOperate.GetEntities<Sys_Grid>(out errMsg, x => x.Sys_ModuleId == t.Id, null, false);
                List<Guid?> gridIds = grids == null ? new List<Guid?>() : grids.Select(x => (Guid?)x.Id).ToList();
                CommonOperate.DeleteRecordsByExpression<Sys_Grid>(x => x.Sys_ModuleId == t.Id, out errMsg);
                //删除用户视图
                CommonOperate.DeleteRecordsByExpression<Sys_UserGrid>(x => gridIds.Contains(x.Sys_GridId), out errMsg);
                //删除列表字段
                CommonOperate.DeleteRecordsByExpression<Sys_GridField>(x => sysFieldIds.Contains(x.Sys_FieldId), out errMsg);
                //删除列表按钮
                CommonOperate.DeleteRecordsByExpression<Sys_GridButton>(x => x.Sys_ModuleId == t.Id, out errMsg);
                //删除字典绑定
                CommonOperate.DeleteRecordsByExpression<Sys_BindDictionary>(x => x.Sys_ModuleId == t.Id, out errMsg);
                //删除附件
                #region 删除附件的文件
                List<Sys_Attachment> attachments = CommonOperate.GetEntities<Sys_Attachment>(out errMsg, x => x.Sys_ModuleId == t.Id, null, false);
                if (attachments != null && attachments.Count > 0)
                {
                    foreach (Sys_Attachment tempObj in attachments)
                    {
                        try
                        {
                            string tempFile = string.Format("{0}{1}", Globals.GetWebDir(), tempObj.FileUrl).Replace("/", "\\");
                            string tempPdfFile = string.Format("{0}{1}", Globals.GetWebDir(), tempObj.PdfUrl).Replace("/", "\\");
                            string tempSwfFile = string.Format("{0}{1}", Globals.GetWebDir(), tempObj.SwfUrl).Replace("/", "\\");
                            if (!string.IsNullOrEmpty(tempFile) && System.IO.File.Exists(tempFile))
                            {
                                System.IO.File.Delete(tempFile); //删除原文件
                            }
                            if (!string.IsNullOrEmpty(tempPdfFile) && System.IO.File.Exists(tempPdfFile))
                            {
                                System.IO.File.Delete(tempPdfFile); //删除ＰＤＦ文件
                            }
                            if (!string.IsNullOrEmpty(tempSwfFile) && System.IO.File.Exists(tempSwfFile))
                            {
                                System.IO.File.Delete(tempSwfFile);　//删除ＳＷＦ文件
                            }
                        }
                        catch { }
                    }
                }
                #endregion
                CommonOperate.DeleteRecordsByExpression<Sys_Attachment>(x => x.Sys_ModuleId == t.Id, out errMsg);
                //删除附属模块绑定
                CommonOperate.DeleteRecordsByExpression<Sys_AttachModuleBind>(x => x.Sys_ModuleId == t.Id || x.ModuleName == t.Name, out errMsg);
                //删除模块菜单
                CommonOperate.DeleteRecordsByExpression<Sys_Menu>(x => x.Sys_ModuleId == t.Id, out errMsg);
                //删除数据表
                try
                {
                    Type modelType = CommonOperate.GetModelType(t.TableName);
                    CommonOperate.DropTable(modelType);
                }
                catch { }
                //删除临时代码文件
                try
                {
                    string codeFile = string.Format("{0}Config\\TempModel\\{1}.code", Globals.GetWebDir(), t.TableName);
                    if (System.IO.File.Exists(codeFile))
                        System.IO.File.Delete(codeFile);
                    string dllFile = string.Format("{0}TempModel\\{1}.dll", Globals.GetBinPath(), t.TableName);
                    if (System.IO.File.Exists(dllFile))
                        System.IO.File.Delete(dllFile);
                }
                catch { }
            }
        }

        #endregion

        #endregion

        #region 菜单

        /// <summary>
        /// 用户菜单缓存
        /// </summary>
        public static Dictionary<Guid, List<Sys_Menu>> userMenusCaches = new Dictionary<Guid, List<Sys_Menu>>();

        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <param name="permissionFilter">是否过滤菜单权限</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static List<Sys_Menu> GetAllMenus(Expression<Func<Sys_Menu, bool>> exp = null, bool permissionFilter = true, UserInfo currUser = null)
        {
            if (permissionFilter && currUser != null)
            {
                if (userMenusCaches.ContainsKey(currUser.UserId))
                {
                    List<Sys_Menu> tempList = userMenusCaches[currUser.UserId];
                    if (tempList != null)
                    {
                        if (exp != null)
                            tempList = tempList.Where(exp.Compile()).ToList();
                        return tempList;
                    }
                }
            }
            string errMsg = string.Empty;
            Expression<Func<Sys_Menu, bool>> tempExp = x => x.IsValid && !x.IsDeleted && !x.IsDraft;
            List<Sys_Menu> list = CommonOperate.GetEntities<Sys_Menu>(out errMsg, tempExp, null, permissionFilter, new List<string>() { "Sort" }, new List<bool>() { false }, null, null, false, null, null, currUser);
            if (list == null) return new List<Sys_Menu>();
            if (permissionFilter && currUser != null)
            {
                userMenusCaches.Remove(currUser.UserId);
                userMenusCaches.Add(currUser.UserId, list);
            }
            if (exp != null)
                list = list.Where(exp.Compile()).ToList();
            return list;
        }

        /// <summary>
        /// 获取顶级菜单
        /// </summary>
        /// <param name="permissionFilter">是否需要权限过滤</param>
        /// <param name="currUser">当前用户，进行菜单权限过滤</param>
        /// <returns></returns>
        public static List<Sys_Menu> GetTopMenus(bool permissionFilter = true, UserInfo currUser = null)
        {
            List<Sys_Menu> list = GetAllMenus(x => x.ParentId == null || x.ParentId == Guid.Empty, permissionFilter, currUser);
            return list;
        }

        /// <summary>
        /// 取子菜单
        /// </summary>
        /// <param name="menuId">父菜单Id</param>
        /// <param name="isDirect">是否直接子菜单，否时取所有子菜单</param>
        /// <param name="detailModuleToAdd">是否将明细模块添加到菜单中，默认为否</param>
        /// /// <param name="permissionFilter">是否需要权限过滤</param>
        /// <param name="currUser">当前用户，进行菜单权限过滤</param>
        /// <returns></returns>
        public static List<Sys_Menu> GetChildMenus(Guid? menuId, bool isDirect = true, bool detailModuleToAdd = false, bool permissionFilter = true, UserInfo currUser = null)
        {
            List<Sys_Menu> listTemp = menuId.HasValue && menuId.Value != Guid.Empty ? GetAllMenus(x => x.ParentId == menuId, permissionFilter, currUser) : GetAllMenus(x => x.ParentId == null || x.ParentId == Guid.Empty, permissionFilter, currUser);
            if (detailModuleToAdd) //需要将明细模块添加到菜单中
            {
                List<Guid> moduleIds = listTemp.Where(x => x.Sys_ModuleId.HasValue && x.Sys_ModuleId.Value != Guid.Empty).Select(x => x.Sys_ModuleId.Value).ToList();
                List<Sys_Menu> addMenus = new List<Sys_Menu>();
                foreach (Sys_Menu menu in listTemp)
                {
                    if (!menu.Sys_ModuleId.HasValue) continue;
                    Sys_Module module = GetModuleById(menu.Sys_ModuleId.Value);
                    if (module == null) continue;
                    List<Sys_Module> detailModules = GetDetailModules(module.Id);
                    foreach (Sys_Module detailModule in detailModules)
                    {
                        if (moduleIds.Contains(detailModule.Id))
                            continue;
                        Sys_Menu tempMenu = new Sys_Menu() { Id = detailModule.Id, Name = detailModule.Name, Display = detailModule.Name, Sort = menu.Sort, Sys_ModuleId = detailModule.Id, Icon = menu.Icon, IsLeaf = true, IsValid = true, ParentId = menu.ParentId };
                        addMenus.Add(tempMenu);
                    }
                }
                listTemp.AddRange(addMenus);
            }
            if (isDirect)
            {
                return listTemp;
            }
            List<Sys_Menu> list = new List<Sys_Menu>();
            foreach (Sys_Menu menu in listTemp)
            {
                list.Add(menu);
                list.AddRange(GetChildMenus(menu.Id, isDirect, detailModuleToAdd, permissionFilter, currUser));
            }
            return list;
        }

        /// <summary>
        /// 获取模块对应的菜单
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static Sys_Menu GetMenuOfModule(Guid moduleId)
        {
            Sys_Menu menu = GetAllMenus(x => x.Sys_ModuleId == moduleId, false).FirstOrDefault();
            return menu;
        }

        /// <summary>
        /// 获取用户快捷操作菜单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public static List<Sys_Menu> GetUserQuckMenus(Guid userId)
        {
            string errMsg = string.Empty;
            List<Sys_Menu> list = new List<Sys_Menu>();
            List<Sys_UserQuckMenu> quckMenus = CommonOperate.GetEntities<Sys_UserQuckMenu>(out errMsg, x => x.Sys_UserId == userId && x.Sys_MenuId != null && !x.IsDeleted, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (quckMenus != null && quckMenus.Count > 0)
            {
                foreach (Sys_UserQuckMenu quckMenu in quckMenus)
                {
                    if (!quckMenu.Sys_MenuId.HasValue) continue;
                    Sys_Menu menu = CommonOperate.GetEntityById<Sys_Menu>(quckMenu.Sys_MenuId.Value, out errMsg);
                    if (menu == null) continue;
                    list.Add(menu);
                }
            }
            return list;
        }

        #endregion

        #region 字段

        /// <summary>
        /// 获取所有字段
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_Field> GetAllSysFields(Expression<Func<Sys_Field, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Sys_Field, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => !x.IsDeleted && !x.IsDraft) : x => !x.IsDeleted && !x.IsDraft;
            List<Sys_Field> fields = CommonOperate.GetEntities<Sys_Field>(out errMsg, tempExp, null, false);
            if (fields == null) fields = new List<Sys_Field>();
            return fields;
        }

        /// <summary>
        /// 获取主键字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Field> GetPrimaryKeyFields(Guid moduleId)
        {
            List<string> fieldNames = GetModulePrimaryKeyFields(moduleId);
            if (fieldNames.Count > 0)
            {
                return GetAllSysFields(x => x.Sys_ModuleId == moduleId && fieldNames.Contains(x.Name));
            }
            return new List<Sys_Field>();
        }

        /// <summary>
        /// 根据Id获取字段信息
        /// </summary>
        /// <param name="id">字段Id</param>
        /// <returns></returns>
        public static Sys_Field GetFieldById(Guid id)
        {
            return GetAllSysFields(x => x.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 获取字段信息
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_Field> GetFields(Expression<Func<Sys_Field, bool>> expression = null)
        {
            return GetAllSysFields(expression);
        }

        /// <summary>
        /// 获取模块字段信息
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Field> GetFieldInfos(Guid moduleId)
        {
            return GetAllSysFields(x => x.Sys_ModuleId == moduleId);
        }

        /// <summary>
        /// 获取模块字段信息
        /// </summary>
        /// <param name="module">模块对象</param>
        /// <returns></returns>
        public static List<Sys_Field> GetFieldInfos(Sys_Module module)
        {
            return GetFieldInfos(module.Id);
        }

        /// <summary>
        /// 获取模块的某个字段信息
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static Sys_Field GetFieldInfo(Guid moduleId, string fieldName)
        {
            return GetAllSysFields(x => x.Sys_ModuleId == moduleId && x.Name == fieldName).FirstOrDefault();
        }

        /// <summary>
        /// 获取字段id
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static Guid GetFieldId(Guid moduleId, string fieldName)
        {
            Sys_Field field = GetFieldInfo(moduleId, fieldName);
            if (field != null) return field.Id;
            return Guid.Empty;
        }

        /// <summary>
        /// 通过字段显示名称获取字段信息
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="display">显示名称</param>
        /// <returns></returns>
        public static Sys_Field GetFieldByDisplay(Guid moduleId, string display)
        {
            return GetAllSysFields(x => x.Sys_ModuleId == moduleId && x.Display == display).FirstOrDefault();
        }

        /// <summary>
        /// 判断是否为外键字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static bool IsForeignField(Guid moduleId, string fieldName)
        {
            Sys_Field field = GetFieldInfo(moduleId, fieldName);
            return field != null && !string.IsNullOrEmpty(field.ForeignModuleName);
        }

        /// <summary>
        /// 判断是否外键字段
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool IsForeignField(Sys_Field field)
        {
            if (field != null)
            {
                Sys_Module module = GetModuleByName(field.ForeignModuleName);
                return module != null;
            }
            return false;
        }

        /// <summary>
        /// 是否外键Name字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">外键Name字段名称</param>
        /// <returns></returns>
        public static bool IsForeignNameField(Guid moduleId, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName)) return false;
            Sys_Field field = GetFieldInfo(moduleId, fieldName);
            if (field != null) return false;
            Type modelType = CommonOperate.GetModelType(moduleId);
            if (fieldName.Length > 4 && fieldName.EndsWith("Name"))
            {
                PropertyInfo p = modelType.GetProperty(fieldName);
                if (p != null)
                {
                    string tempFieldName = fieldName.Substring(0, fieldName.Length - 4) + "Id";
                    field = GetFieldInfo(moduleId, tempFieldName);
                    if (field != null)
                    {
                        Sys_Module module = GetModuleByName(field.ForeignModuleName);
                        return module != null;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取外键Name字段
        /// </summary>
        /// <param name="formField">表单字段对象</param>
        /// <returns></returns>
        public static string GetForeignNameField(Sys_FormField formField)
        {
            string tempFieldName = formField.Sys_FieldName;
            if (formField == null || string.IsNullOrEmpty(tempFieldName) || !formField.Sys_FieldId.HasValue)
                return string.Empty;
            Sys_Field sysField = GetFieldById(formField.Sys_FieldId.Value);
            if (sysField == null || string.IsNullOrEmpty(sysField.ForeignModuleName))
                return string.Empty;
            if (tempFieldName.Length > 2 && tempFieldName.EndsWith("Id"))
                return tempFieldName.Substring(0, tempFieldName.Length - 2) + "Name";
            return string.Empty;
        }

        /// <summary>
        /// 是否为枚举字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static bool IsEnumField(Guid moduleId, string fieldName)
        {
            Type modelType = GetModelType(moduleId);
            if (modelType == null) return false;
            PropertyInfo pEnum = modelType.GetProperty(string.Format("{0}OfEnum", fieldName));
            return pEnum != null && pEnum.PropertyType.IsEnum;
        }

        /// <summary>
        /// 是否为枚举字段
        /// </summary>
        /// <param name="modelType">模块类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static bool IsEnumField(Type modelType, string fieldName)
        {
            if (modelType == null) return false;
            PropertyInfo pEnum = modelType.GetProperty(string.Format("{0}OfEnum", fieldName));
            return pEnum != null && pEnum.PropertyType.IsEnum;
        }

        /// <summary>
        /// 是否字典绑定字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static bool IsDictionaryBindField(Guid moduleId, string fieldName)
        {
            string className = GetBindDictonaryClass(moduleId, fieldName);
            return !string.IsNullOrEmpty(className);
        }

        /// <summary>
        /// 获取字段的显示名称
        /// </summary>
        /// <param name="fieldId">字段Id</param>
        /// <returns></returns>
        public static string GetFieldDisplay(Guid fieldId)
        {
            Sys_Field field = GetFieldById(fieldId);
            if (field != null) return field.Display;
            return string.Empty;
        }

        /// <summary>
        /// 获取字段的显示名称
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string GetFieldDisplay(Guid moduleId, string fieldName)
        {
            Sys_Field field = GetFieldInfo(moduleId, fieldName);
            if (field != null) return field.Display;
            if (IsForeignNameField(moduleId, fieldName))
            {
                string tempFieldName = fieldName.Substring(0, fieldName.Length - 4) + "Id";
                field = GetFieldInfo(moduleId, tempFieldName);
                if (field != null) return field.Display;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取字段的外键模块名称
        /// </summary>
        /// <param name="fieldId">字段Id</param>
        /// <returns></returns>
        public static string GetFieldForeignModuleName(Guid fieldId)
        {
            Sys_Field field = GetFieldById(fieldId);
            if (field != null) return field.ForeignModuleName;
            return string.Empty;
        }

        /// <summary>
        /// 获取字段的外键模块名称
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string GetFieldForeignModuleName(Guid moduleId, string fieldName)
        {
            Sys_Field field = GetFieldInfo(moduleId, fieldName);
            if (field != null) return field.ForeignModuleName;
            return string.Empty;
        }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static Type GetFieldType(Guid moduleId, string fieldName)
        {
            Type modelType = CommonOperate.GetModelType(moduleId);
            if (modelType == null) return null;
            PropertyInfo p = modelType.GetProperty(fieldName);
            if (p == null) return null;
            return p.PropertyType;
        }

        /// <summary>
        /// 获取字段类型
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static Type GetFieldType(Type modelType, string fieldName)
        {
            if (modelType == null) return null;
            PropertyInfo p = modelType.GetProperty(fieldName);
            if (p == null) return null;
            return p.PropertyType;
        }

        /// <summary>
        /// 获取字段综合信息
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetFieldCommonInfo(Guid moduleId, string fieldName)
        {
            string foreignModuleName = null;
            object dicData = null;
            object enumData = null;
            int controlType = 0;
            string foreignTitleKey = null;
            string tempFieldName = fieldName;
            if (SystemOperate.IsForeignNameField(moduleId, fieldName))
                tempFieldName = fieldName.Substring(0, fieldName.Length - 4) + "Id";
            Type fieldType = SystemOperate.GetFieldType(moduleId, tempFieldName);
            Sys_Field sysField = SystemOperate.GetFieldInfo(moduleId, tempFieldName);
            if (sysField == null) return null;
            foreignModuleName = sysField.ForeignModuleName;
            Sys_Module foreignModule = SystemOperate.GetForeignModule(sysField);
            Sys_FormField formField = SystemOperate.GetNfDefaultFormSingleField(sysField);
            if (formField != null)
            {
                controlType = formField.ControlType;
                dicData = SystemOperate.DictionaryDataJson(moduleId, tempFieldName);
                enumData = SystemOperate.EnumFieldDataJson(moduleId, tempFieldName);
            }
            else if (CommonDefine.BaseEntityFields.Contains(tempFieldName))
            {
                if (foreignModule != null)
                    controlType = (int)ControlTypeEnum.DialogGrid;
                else if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
                    controlType = (int)ControlTypeEnum.DateTimeBox;
            }
            foreignTitleKey = foreignModule == null ? null : foreignModule.TitleKey;
            return new { FieldName = fieldName, ControlType = controlType, DicData = dicData, EnumData = enumData, ForeignModule = foreignModuleName, FieldType = fieldType.ToString(), ForeignTitleKey = foreignTitleKey };
        }

        #region 字段值

        /// <summary>
        /// 获取字段显示值
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="model">实体对象</param>
        /// <param name="sysField">字段信息</param>
        /// <param name="field">表单字段</param>
        /// <returns></returns>
        public static string GetFieldDisplayValue(Guid moduleId, object model, Sys_Field sysField, Sys_FormField field)
        {
            if (field == null || model == null || sysField == null) return string.Empty;
            object value = CommonOperate.GetModelFieldValueByModel(moduleId, model, sysField.Name);
            if (value == null) return string.Empty;
            string valueStr = value.ObjToStr();
            if (SystemOperate.IsForeignField(moduleId, sysField.Name)) //外键字段
            {
                bool isMutiSelect = field.IsMultiSelect == true && SystemOperate.GetFieldType(moduleId, sysField.Name) == typeof(String);
                string textFieldName = isMutiSelect ? sysField.Name : sysField.Name.Substring(0, sysField.Name.Length - 2) + "Name";
                valueStr = CommonOperate.GetModelFieldValueByModel(moduleId, model, textFieldName).ObjToStr();
                Sys_Module foreignModule = GetForeignModule(sysField);
                if (foreignModule != null)
                {
                    if (isMutiSelect) //多选外键处理
                    {
                        string tempTextStr = string.Empty;
                        List<Guid> ids = value.ObjToStr().Split(",".ToCharArray()).Select(x => x.ObjToGuid()).ToList();
                        foreach (Guid id in ids)
                        {
                            string tempStr = CommonOperate.GetModelTitleKeyValue(foreignModule.Id, id);
                            if (tempTextStr != string.Empty) tempTextStr += ",";
                            tempTextStr += tempStr;
                        }
                        valueStr = tempTextStr;
                    }
                    else if (string.IsNullOrEmpty(valueStr)) //当前实体中不存在从数据库中取
                    {
                        valueStr = CommonOperate.GetModelTitleKeyValue(foreignModule.Id, value.ObjToGuid());
                    }
                }
            }
            else if (field.ControlTypeOfEnum == ControlTypeEnum.RadioList) //单选框组
            {
                if (!string.IsNullOrEmpty(valueStr) && !string.IsNullOrEmpty(field.ValueField) && !string.IsNullOrEmpty(field.TextField))
                {
                    string[] tokenValue = field.ValueField.Split(",".ToCharArray());
                    string[] tokenText = field.TextField.Split(",".ToCharArray());
                    if (tokenValue.Length == tokenText.Length)
                    {
                        for (int i = 0; i < tokenValue.Length; i++)
                        {
                            if (tokenValue[i] == valueStr)
                            {
                                valueStr = tokenText[i];
                                break;
                            }
                        }
                    }
                }
            }
            else if (SystemOperate.IsEnumField(moduleId, sysField.Name)) //枚举字段
            {
                valueStr = SystemOperate.GetEnumFieldDisplayText(moduleId, sysField.Name, valueStr);
            }
            else if (SystemOperate.IsDictionaryBindField(moduleId, sysField.Name)) //字典绑定字段
            {
                valueStr = SystemOperate.GetDictionaryDisplayText(moduleId, sysField.Name, valueStr);
            }
            else if (field.ControlTypeOfEnum == ControlTypeEnum.DateBox ||
                field.ControlTypeOfEnum == ControlTypeEnum.DateTimeBox)
            {
                string dateFormat = field.ControlTypeOfEnum == ControlTypeEnum.DateBox ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss";
                try
                {
                    valueStr = DateTime.Parse(valueStr).ToString(dateFormat);
                }
                catch { }
            }
            else
            {
                Type fieldType = SystemOperate.GetFieldType(moduleId, sysField.Name);
                if (fieldType == typeof(Boolean) || fieldType == typeof(Boolean?))
                {
                    if (valueStr.ToLower() == "true")
                        valueStr = "是";
                    else if (valueStr.ToLower() == "false")
                        valueStr = "否";
                }
            }
            return valueStr;
        }

        /// <summary>
        /// 获取字段显示值
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="model">实体对象</param>
        /// <param name="field">表单字段</param>
        /// <returns></returns>
        public static string GetFieldDisplayValue(Guid moduleId, object model, Sys_FormField field)
        {
            if (field == null || model == null) return string.Empty;
            Sys_Field sysField = SystemOperate.GetFieldById(field.Sys_FieldId.Value);
            return GetFieldDisplayValue(moduleId, model, sysField, field);
        }

        /// <summary>
        /// 获取字段显示值
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="recordId">记录Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string GetFieldDisplayValue(Guid moduleId, Guid recordId, string fieldName)
        {
            string errMsg = string.Empty;
            object model = CommonOperate.GetEntityById(moduleId, recordId, out errMsg);
            Sys_FormField field = GetNfDefaultFormSingleField(moduleId, fieldName);
            return GetFieldDisplayValue(moduleId, model, field);
        }

        /// <summary>
        /// 获取字段显示值
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="model">实体对象</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static string GetFieldDisplayValue(Guid moduleId, object model, string fieldName)
        {
            Sys_FormField formField = GetNfDefaultFormSingleField(moduleId, fieldName);
            if (formField == null)
            {
                Type modelType = CommonOperate.GetModelType(moduleId);
                PropertyInfo p = modelType.GetProperty(fieldName);
                if (p == null) return string.Empty;
                return p.GetValue2(model, null).ObjToStr();
            }
            return GetFieldDisplayValue(moduleId, model, formField);
        }

        #endregion

        #endregion

        #region 列表

        #region 视图字段

        /// <summary>
        /// 获取所有视图字段集合
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_GridField> GetAllGridFields(Expression<Func<Sys_GridField, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Sys_GridField, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => !x.IsDeleted && !x.IsDraft) : x => !x.IsDeleted && !x.IsDraft;
            List<Sys_GridField> list = CommonOperate.GetEntities<Sys_GridField>(out errMsg, tempExp, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (list == null) list = new List<Sys_GridField>();
            return list;
        }

        /// <summary>
        /// 加载用户视图字段
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_GridField> GetUserGridFields(Guid userId, Guid moduleId)
        {
            Sys_Grid grid = GetUserDefaultGrid(userId, moduleId);
            return GetGridFields(grid);
        }

        /// <summary>
        /// 获取默认列表视图的字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="addForeignNameField">是否添加外键显示字段</param>
        /// <returns></returns>
        public static List<Sys_GridField> GetDefaultGridFields(Guid moduleId, bool addForeignNameField = true)
        {
            string errMsg = string.Empty;
            Sys_Grid grid = GetGrids(moduleId).Where(x => x.IsDefault).FirstOrDefault();
            if (grid == null) return new List<Sys_GridField>();
            List<Sys_GridField> gridFields = GetGridFields(grid, addForeignNameField);
            return gridFields;
        }

        /// <summary>
        /// 获取视图字段
        /// </summary>
        /// <param name="viewId">视图Id</param>
        /// <param name="addForeignNameField">是否自动添加外键Name网格字段</param>
        /// <returns></returns>
        public static List<Sys_GridField> GetGridFields(Guid viewId, bool addForeignNameField = true)
        {
            Sys_Grid grid = GetGrid(viewId);
            return GetGridFields(grid, addForeignNameField);
        }

        /// <summary>
        /// 获取网格字段
        /// </summary>
        /// <param name="viewId">视图Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static Sys_GridField GetGridField(Guid viewId, string fieldName)
        {
            string errMsg = string.Empty;
            return GetGridFields(viewId, false).Where(x => x.Sys_FieldName == fieldName).FirstOrDefault();
        }

        /// <summary>
        /// 获取默认网格字段
        /// </summary>
        /// <param name="sysField">字段信息</param>
        /// <returns></returns>
        public static Sys_GridField GetDefaultGridField(Sys_Field sysField)
        {
            if (sysField != null && sysField.Sys_ModuleId.HasValue && sysField.Sys_ModuleId.Value != Guid.Empty)
            {
                return GetDefaultGridFields(sysField.Sys_ModuleId.Value, false).Where(x => x.Sys_FieldId == sysField.Id).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 获取视图字段
        /// </summary>
        /// <param name="grid">视图对象</param>
        /// <param name="addForeignNameField">是否自动添加外键Name网格字段</param>
        /// <param name="isNeedCopy">是否需要复制，字段提供重写时用到</param>
        /// <returns></returns>
        public static List<Sys_GridField> GetGridFields(Sys_Grid grid, bool addForeignNameField = true, bool isNeedCopy = false)
        {
            if (grid == null || !grid.Sys_ModuleId.HasValue)
                return new List<Sys_GridField>();
            bool isEnableMemeryCache = ModelConfigHelper.IsModelEnableMemeryCache(typeof(Sys_GridField)); //是否启动内存缓存
            //获取视图字段
            List<Sys_GridField> gridFields = GetAllGridFields(x => x.Sys_GridId == grid.Id && x.Sys_FieldId != null);
            List<Sys_GridField> list = new List<Sys_GridField>(); //装载最终视图字段
            foreach (Sys_GridField field in gridFields) //循环处理字段
            {
                if (!field.Sys_FieldId.HasValue) continue;
                Sys_Field sysField = field.TempSysField != null ? field.TempSysField : SystemOperate.GetFieldById(field.Sys_FieldId.Value);
                if (sysField == null) continue;
                field.Sys_FieldName = sysField.Name;
                if (field.TempSysField == null) field.TempSysField = sysField;
                bool isForegin = !string.IsNullOrEmpty(sysField.ForeignModuleName);
                if (isForegin && addForeignNameField) //外键字段并且需要添加name字段
                {
                    #region 复制字段
                    Sys_GridField tempGridField = new Sys_GridField();
                    if (isNeedCopy)
                        ObjectHelper.CopyValue<Sys_GridField>(field, tempGridField);
                    else if (isEnableMemeryCache)
                        ObjectHelper.CopyValue<Sys_GridField>(field, tempGridField);
                    else
                        tempGridField = field;
                    #endregion
                    #region 增加外键Name字段
                    //增加一个name字段
                    Sys_GridField tempNameGridField = new Sys_GridField();
                    ObjectHelper.CopyValue<Sys_GridField>(field, tempNameGridField);
                    //字段名称为原字段名称去掉最后的Id加上Name
                    tempNameGridField.Sys_FieldName = sysField.Name.EndsWith("Id") ? sysField.Name.Substring(0, sysField.Name.Length - 2) + "Name" : sysField.Name + "Name";
                    list.Add(tempNameGridField);
                    #endregion
                    tempGridField.IsVisible = false; //原字段不可见
                    tempGridField.IsAllowSearch = false; //不可搜索
                    tempGridField.IsGroupField = false; //不可分组
                    tempGridField.Width = 0;
                    list.Add(tempGridField);
                }
                else
                {
                    Sys_GridField tempGridField = new Sys_GridField();
                    if (isNeedCopy)
                        ObjectHelper.CopyValue<Sys_GridField>(field, tempGridField);
                    else
                        tempGridField = field;
                    list.Add(tempGridField);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取默认视图搜索字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="containId">是否包含Id列</param>
        /// <returns></returns>
        public static List<Sys_GridField> GetDefaultSearchGridFields(Guid moduleId, bool containId = false)
        {
            Sys_Grid grid = GetGrids(moduleId).Where(x => x.IsDefault).FirstOrDefault();
            if (grid == null) return new List<Sys_GridField>();
            return GetSearchGridFields(grid.Id);
        }

        /// <summary>
        /// 取搜索字段
        /// </summary>
        /// <param name="viewId">视图Id</param>
        /// <returns></returns>
        public static List<Sys_GridField> GetSearchGridFields(Guid viewId)
        {
            string errMsg = string.Empty;
            Sys_Grid grid = SystemOperate.GetGrid(viewId);
            if (grid == null || !grid.Sys_ModuleId.HasValue) return new List<Sys_GridField>();
            Type modelType = SystemOperate.GetModelType(grid.Sys_ModuleId.Value);
            if (modelType == null) return new List<Sys_GridField>();
            List<Sys_GridField> list = GetGridFields(viewId, false).Where(x => x.IsVisible && x.IsAllowSearch).ToList();
            if (list.Count == 0) return new List<Sys_GridField>();
            return list;
        }

        /// <summary>
        /// 获取默认列表分组字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static Sys_GridField GetDefaultGridGroupField(Guid moduleId)
        {
            Sys_Grid grid = GetGrids(moduleId).Where(x => x.IsDefault).FirstOrDefault();
            if (grid == null) return null;
            return GetGridGroupField(grid.Id);
        }

        /// <summary>
        /// 获取分组字段
        /// </summary>
        /// <param name="viewId">视图Id</param>
        /// <returns></returns>
        public static Sys_GridField GetGridGroupField(Guid viewId)
        {
            Sys_GridField field = GetGridFields(viewId).Where(x => x.IsGroupField).FirstOrDefault();
            return field;
        }

        /// <summary>
        /// 获取列表行过滤规则
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <param name="gridFields">模块字段</param>
        /// <param name="noFilterFields">不参与过滤的字段</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="permisstionBtnIds">权限过滤的按钮ID集合</param>
        /// <returns></returns>
        public static StringBuilder GetGridRowFilterRules(Guid moduleId, List<Sys_GridField> gridFields, out List<string> noFilterFields, UserInfo currUser, List<Guid> permisstionBtnIds = null)
        {
            StringBuilder ruleFilters = new StringBuilder();
            //不参与过滤的字段
            noFilterFields = new List<string>();
            List<Sys_GridButton> btns = SystemOperate.GetGridHeadButtons(moduleId);
            if (permisstionBtnIds != null && permisstionBtnIds.Count > 0)
            {
                btns = btns.Where(x => permisstionBtnIds.Contains(x.Id)).ToList();
            }
            else
            {
                List<Guid> btnIds = PermissionOperate.GetUserFunPermissions(currUser, FunctionTypeEnum.Button, moduleId);
                if (btnIds != null)
                    btns = btns.Where(x => btnIds.Contains(x.Id)).ToList();
            }
            if (btns.Count > 0)
            {
                noFilterFields.Add("RowOperateBtn");
            }
            foreach (Sys_GridField field in gridFields)
            {
                if (field.Sys_FieldName != "FlowStatus")
                {
                    if (!field.Sys_FieldId.HasValue) continue;
                    if (!field.IsAllowSearch)
                    {
                        noFilterFields.Add(field.Sys_FieldName);
                        continue;
                    }
                    Sys_Field sysField = field.TempSysField != null ? field.TempSysField : SystemOperate.GetFieldById(field.Sys_FieldId.Value);
                    if (sysField == null) continue;
                    if (field.TempSysField == null) field.TempSysField = sysField;
                    Type fieldType = SystemOperate.GetFieldType(moduleId, sysField.Name);
                    if (fieldType == null) continue;
                    if (!CommonDefine.BaseEntityFields.Contains(field.Sys_FieldName))
                    {
                        Sys_FormField formField = SystemOperate.GetNfDefaultFormSingleField(sysField);
                        if (formField != null)
                        {
                            switch (formField.ControlTypeOfEnum)
                            {
                                case ControlTypeEnum.TextBox:
                                case ControlTypeEnum.TextAreaBox:
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'textbox',op:['contains','equal','notequal','isnull','isnotnull']},");
                                    break;
                                case ControlTypeEnum.IntegerBox:
                                case ControlTypeEnum.NumberBox:
                                    if (fieldType.IsGenericType)
                                        ruleFilters.Append("{field:'" + sysField.Name + "',type:'numberbox',op:['equal','notequal','less','greater','isnull','isnotnull']},");
                                    else
                                        ruleFilters.Append("{field:'" + sysField.Name + "',type:'numberbox',op:['equal','notequal','less','greater']},");
                                    break;
                                case ControlTypeEnum.SingleCheckBox:
                                    if (fieldType.IsGenericType)
                                        ruleFilters.Append("{field:'" + sysField.Name + "',type:'combobox',options:{editable:false,data:[{ckId:'',ckName:'所有'},{ckId:'1',ckName:'是'},{ckId:'0',ckName:'否'}],valueField:'ckId',textField:'ckName'},op:['equal','notequal','isnull','isnotnull']},");
                                    else
                                        ruleFilters.Append("{field:'" + sysField.Name + "',type:'combobox',options:{editable:false,data:[{ckId:'',ckName:'所有'},{ckId:'1',ckName:'是'},{ckId:'0',ckName:'否'}],valueField:'ckId',textField:'ckName'},op:['equal','notequal']},");
                                    break;
                                case ControlTypeEnum.DialogGrid:
                                    {
                                        string fieldName = sysField.Name;
                                        fieldName = fieldName.Substring(0, fieldName.Length - 2) + "Name";
                                        ruleFilters.Append("{field:'" + fieldName + "',type:'textbox',op:['contains','isnull','isnotnull']},");
                                    }
                                    break;
                                case ControlTypeEnum.ComboBox:
                                case ControlTypeEnum.ComboGrid:
                                case ControlTypeEnum.ComboTree:
                                    {
                                        Sys_Module foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName);
                                        string valueField = formField.ValueField;
                                        string textField = formField.TextField;
                                        string fieldUrl = formField.UrlOrData;
                                        string type = "combobox";
                                        if (formField.ControlTypeOfEnum == ControlTypeEnum.ComboTree)
                                        {
                                            type = "combotree";
                                            if (string.IsNullOrEmpty(valueField)) valueField = "id";
                                            if (string.IsNullOrEmpty(textField)) textField = "text";
                                            if (fieldUrl == null && foreignModule != null)
                                            {
                                                fieldUrl = string.Format("/{0}/GetTreeByNode.html?moduleId={1}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, foreignModule.Id.ToString());
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(valueField)) valueField = "Id";
                                            if (string.IsNullOrEmpty(textField))
                                            {
                                                string foreignTitleKey = foreignModule == null ? null : SystemOperate.GetModuleTitleKey(foreignModule.Id);
                                                textField = string.IsNullOrWhiteSpace(foreignTitleKey) ? "Name" : foreignTitleKey;
                                            }
                                            if (string.IsNullOrEmpty(fieldUrl))
                                            {
                                                if (foreignModule != null) //外键字段
                                                    fieldUrl = string.Format("/{0}/BindForeignFieldComboData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), field.Sys_FieldName);
                                                else if (SystemOperate.IsEnumField(moduleId, field.Sys_FieldName)) //枚举字段
                                                    fieldUrl = string.Format("/{0}/BindEnumFieldData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), field.Sys_FieldName);
                                                else if (SystemOperate.IsDictionaryBindField(moduleId, field.Sys_FieldName)) //字典绑定字段
                                                    fieldUrl = string.Format("/{0}/BindDictionaryData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), field.Sys_FieldName);
                                            }
                                        }
                                        string fieldName = sysField.Name;
                                        if (foreignModule != null)
                                            fieldName = fieldName.Substring(0, fieldName.Length - 2) + "Name";
                                        string loadFilterElseStr = formField.ControlTypeOfEnum == ControlTypeEnum.ComboTree ? ",loadFilter:RuleFilterLoadComTreeFilter" : string.Empty;
                                        if (fieldType.IsGenericType)
                                            ruleFilters.Append("{field:'" + fieldName + "',type:'" + type + "',options:{panelWidth:'150',url:'" + fieldUrl + "',valueField:'" + valueField + "',textField:'" + textField + "'" + loadFilterElseStr + "},op:['equal','notequal','isnull','isnotnull']},");
                                        else
                                            ruleFilters.Append("{field:'" + fieldName + "',type:'" + type + "',options:{panelWidth:'150',url:'" + fieldUrl + "',valueField:'" + valueField + "',textField:'" + textField + "'" + loadFilterElseStr + "},op:['equal','notequal']},");
                                    }
                                    break;
                                case ControlTypeEnum.DateBox:
                                case ControlTypeEnum.DateTimeBox:
                                    {
                                        if (fieldType.IsGenericType)
                                            ruleFilters.Append("{field:'" + sysField.Name + "',type:'dateRange',op:['isnull','isnotnull']},");
                                        else
                                            ruleFilters.Append("{field:'" + sysField.Name + "',type:'dateRange',op:[]},");
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (fieldType == typeof(String))
                            {
                                ruleFilters.Append("{field:'" + sysField.Name + "',type:'textbox',op:['contains','equal','notequal','isnull','isnotnull']},");
                            }
                            else if (GetModuleByName(sysField.ForeignModuleName) != null) //外键字段
                            {
                                ruleFilters.Append("{field:'" + sysField.Name + "',type:'textbox',op:['contains','isnull','isnotnull']},");
                            }
                            else if (SystemOperate.IsEnumField(moduleId, sysField.Name)) //枚举字段
                            {
                                string fieldUrl = string.Format("/{0}/BindEnumFieldData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId, field.Sys_FieldName);
                                if (fieldType.IsGenericType)
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'combobox',options:{panelWidth:'120',url:'" + fieldUrl + "',valueField:'Id',textField:'Name'},op:['equal','notequal','isnull','isnotnull']},");
                                else
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'combobox',options:{panelWidth:'120',url:'" + fieldUrl + "',valueField:'Id',textField:'Name'},op:['equal','notequal']},");
                            }
                            else if (fieldType == typeof(Int16) || fieldType == typeof(Int32) || fieldType == typeof(Int64) ||
                                fieldType == typeof(Int16?) || fieldType == typeof(Int32?) || fieldType == typeof(Int64?) ||
                                fieldType == typeof(Double) || fieldType == typeof(float) || fieldType == typeof(Decimal) ||
                                fieldType == typeof(Double?) || fieldType == typeof(float?) || fieldType == typeof(Decimal?))
                            {
                                if (fieldType.IsGenericType)
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'numberbox',op:['equal','notequal','less','greater','isnull','isnotnull']},");
                                else
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'numberbox',op:['equal','notequal','less','greater']},");
                            }
                            else if (fieldType == typeof(Boolean) || fieldType == typeof(Boolean?))
                            {
                                if (fieldType.IsGenericType)
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'checkbox',op:['equal','notequal','isnull','isnotnull']},");
                                else
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'checkbox',op:['equal','notequal']},");
                            }
                            else if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
                            {
                                if (fieldType.IsGenericType)
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'dateRange',op:['isnull','isnotnull']},");
                                else
                                    ruleFilters.Append("{field:'" + sysField.Name + "',type:'dateRange',op:[]},");
                            }
                        }
                    }
                    else //基类字段
                    {
                        if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
                        {
                            ruleFilters.Append("{field:'" + sysField.Name + "',type:'dateRange',op:[]},");
                        }
                        else if (field.Sys_FieldName == "CreateUserName" || field.Sys_FieldName == "ModifyUserName")
                        {
                            ruleFilters.Append("{field:'" + field.Sys_FieldName + "',type:'textbox',op:['contains','isnull','isnotnull']},");
                        }
                    }
                }
                else //流程状态字段处理
                {
                    string fieldUrl = string.Format("/{0}/BindEnumFieldData.html?moduleId={1}&fieldName=FlowStatus", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString());
                    ruleFilters.Append("{field:'FlowStatus',type:'combobox',options:{panelWidth:'auto',url:'" + fieldUrl + "',valueField:'Id',textField:'Name'},op:['equal','notequal']},");
                }
            }
            return ruleFilters;
        }

        /// <summary>
        /// 获取视图的字段
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="viewId">视图ID</param>
        /// <returns></returns>
        public static List<string> GetFieldNamesOfView(Guid moduleId, Guid viewId)
        {
            List<Sys_GridField> gridFields = GetGridFields(viewId, false).Where(x => x.Sys_FieldId.HasValue).ToList();
            List<Guid> sysFieldIds = new List<Guid>();
            Type modelType = CommonOperate.GetModelType(moduleId);
            foreach (Sys_GridField field in gridFields)
            {
                PropertyInfo p = modelType.GetProperty(field.Sys_FieldName);
                if (p == null) continue;
                IgnoreAttribute ignoreAttr = (IgnoreAttribute)Attribute.GetCustomAttribute(p, typeof(IgnoreAttribute));
                if (ignoreAttr != null) continue;
                sysFieldIds.Add(field.Sys_FieldId.Value);
            }
            List<string> fieldNames = GetAllSysFields(x => sysFieldIds.Contains(x.Id)).Select(x => x.Name).ToList();
            return fieldNames;
        }

        #endregion

        #region 视图

        /// <summary>
        /// 获取所有视图集合
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_Grid> GetAllGrids(Expression<Func<Sys_Grid, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Sys_Grid, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => !x.IsDeleted && !x.IsDraft) : x => !x.IsDeleted && !x.IsDraft;
            List<Sys_Grid> grids = CommonOperate.GetEntities<Sys_Grid>(out errMsg, tempExp, null, false);
            if (grids == null) grids = new List<Sys_Grid>();
            return grids;
        }

        /// <summary>
        /// 获取所有用户视图集合
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_UserGrid> GetAllUserGrids(Expression<Func<Sys_UserGrid, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Sys_UserGrid, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => !x.IsDeleted && !x.IsDraft) : x => !x.IsDeleted && !x.IsDraft;
            List<Sys_UserGrid> userGrids = CommonOperate.GetEntities<Sys_UserGrid>(out errMsg, tempExp, null, false);
            if (userGrids == null) userGrids = new List<Sys_UserGrid>();
            return userGrids;
        }

        /// <summary>
        /// 获取模块所有视图
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Grid> GetGrids(Guid moduleId)
        {
            return GetAllGrids(x => x.Sys_ModuleId == moduleId);
        }

        /// <summary>
        /// 获取默认视图
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static Sys_Grid GetDefaultGrid(Guid moduleId)
        {
            return GetAllGrids(x => x.Sys_ModuleId == moduleId && x.IsDefault).FirstOrDefault();
        }

        /// <summary>
        /// 是否是系统默认视图
        /// </summary>
        /// <param name="viewId">视图ID</param>
        /// <returns></returns>
        public static bool IsSystemDefaultGrid(Guid viewId)
        {
            Sys_Grid grid = GetAllGrids(x => x.Id == viewId && x.IsDefault).FirstOrDefault();
            return grid != null;
        }

        /// <summary>
        /// 取用户默认列表视图
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static Sys_Grid GetUserDefaultGrid(Guid userId, Guid moduleId)
        {
            Sys_Grid grid = null;
            List<Sys_Grid> grids = GetAllGrids(x => x.Sys_ModuleId == moduleId);
            //取用户自定义默认视图
            List<Sys_UserGrid> userGrids = GetAllUserGrids(x => x.Sys_UserId == userId);
            if (userGrids.Count > 0)
            {
                List<Guid> gridIds = grids != null ? grids.Select(x => x.Id).ToList() : new List<Guid>();
                Sys_UserGrid userGrid = userGrids.Where(x => x.Sys_GridId != null && gridIds.Contains(x.Sys_GridId.Value) && x.IsDefault).FirstOrDefault();
                if (userGrid != null)
                {
                    Guid gridId = userGrid.Sys_GridId.Value;
                    grid = grids.Where(x => x.Id == gridId).FirstOrDefault();
                }
            }
            //取管理员admin默认视图
            if (grid == null)
            {
                grid = GetAdminDefaultConfigGrid(moduleId);
            }
            //取系统默认视图
            if (grid == null)
            {
                grid = grids.Where(x => x.IsDefault).FirstOrDefault();
            }
            return grid;
        }

        /// <summary>
        /// 获取管理员admin默认配置视图
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public static Sys_Grid GetAdminDefaultConfigGrid(Guid moduleId)
        {
            Guid adminUserId = UserOperate.GetSuperAdmin().UserId;
            List<Sys_UserGrid> adminUserGrids = GetAllUserGrids(x => x.Sys_UserId == adminUserId);
            if (adminUserGrids.Count == 0) return null;
            List<Sys_Grid> grids = GetAllGrids(x => x.Sys_ModuleId == moduleId);
            List<Guid> gridIds = grids != null ? grids.Select(x => x.Id).ToList() : new List<Guid>();
            Sys_UserGrid adminUserGrid = adminUserGrids.Where(x => x.Sys_GridId != null && gridIds.Contains(x.Sys_GridId.Value) && x.IsDefault).FirstOrDefault();
            if (adminUserGrid != null)
            {
                Guid gridId = adminUserGrid.Sys_GridId.Value;
                return grids.Where(x => x.Id == gridId).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 取用户自己创建的列表视图
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Grid> GetUserGrids(Guid userId, Guid moduleId)
        {
            List<Sys_UserGrid> userGrids = GetAllUserGrids(x => x.Sys_UserId == userId);
            if (userGrids != null)
            {
                List<Guid> gridIds = userGrids.Select(x => x.Sys_GridId.Value).ToList();
                List<Sys_Grid> grids = GetGrids(moduleId);
                if (grids != null)
                {
                    return grids.Where(x => gridIds.Contains(x.Id)).ToList();
                }
            }
            return new List<Sys_Grid>();
        }

        /// <summary>
        /// 获取系统列表视图
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Grid> GetSystemGrids(Guid moduleId)
        {
            List<Sys_Grid> grids = GetGrids(moduleId);
            List<Sys_UserGrid> userGrids = GetAllUserGrids();
            if (userGrids != null)
            {
                List<Guid> gridIds = userGrids.Select(x => x.Sys_GridId.Value).ToList();
                return grids.Where(x => !gridIds.Contains(x.Id)).ToList();
            }
            return grids;
        }

        /// <summary>
        /// 是否是用户视图
        /// </summary>
        /// <param name="viewId">视图Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public static bool IsUserGridView(Guid viewId, Guid userId)
        {
            List<Sys_UserGrid> userGrids = GetAllUserGrids(x => x.Sys_GridId == viewId && x.Sys_UserId == userId);
            return userGrids != null && userGrids.Count > 0;
        }

        /// <summary>
        /// 判断是否为默认视图
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="viewId">视图Id</param>
        /// <returns></returns>
        public static bool IsUserDefaultGridView(Guid userId, Guid viewId)
        {
            Sys_UserGrid userGrid = GetAllUserGrids(x => x.Sys_UserId == userId && x.Sys_GridId == viewId && x.IsDefault).FirstOrDefault();
            return userGrid != null;
        }

        /// <summary>
        /// 删除用户视图
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="viewId">视图Id</param>
        /// <returns>返回异常信息</returns>
        public static string DeleteUserGrid(Guid userId, Guid viewId)
        {
            string errMsg = string.Empty;
            Sys_UserGrid userGrid = GetAllUserGrids(x => x.Sys_UserId == userId && x.Sys_GridId == viewId && x.IsDefault).FirstOrDefault();
            //删除用户视图
            if (userGrid != null)
            {
                var ids = new List<Guid>() { userGrid.Id };
                bool rs = CommonOperate.DeleteRecords<Sys_UserGrid>(ids, out errMsg, false, false);
                if (!rs) return errMsg;
            }
            //删除视图
            bool delRs = CommonOperate.DeleteRecords<Sys_Grid>(new List<Guid>() { viewId }, out errMsg, false, false);
            //删除视图明细
            if (delRs)
            {
                CommonOperate.DeleteRecordsByExpression<Sys_GridField>(x => x.Sys_GridId == viewId, out errMsg);
            }
            return errMsg;
        }

        /// <summary>
        /// 获取视图
        /// </summary>
        /// <param name="id">视图Id</param>
        /// <returns></returns>
        public static Sys_Grid GetGrid(Guid id)
        {
            return GetAllGrids(x => x.Id == id).FirstOrDefault();
        }

        #endregion

        #region 视图按钮

        /// <summary>
        /// 获取所有视图按钮
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_GridButton> GetAllGridButtons(Expression<Func<Sys_GridButton, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Sys_GridButton, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => x.IsValid && !x.IsDeleted && !x.IsDraft) : x => x.IsValid && !x.IsDeleted && !x.IsDraft;
            List<Sys_GridButton> list = CommonOperate.GetEntities<Sys_GridButton>(out errMsg, tempExp, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (list == null) return new List<Sys_GridButton>();
            return list;
        }

        /// <summary>
        /// 获取视图按钮
        /// </summary>
        /// <param name="btnId">按钮Id</param>
        /// <returns></returns>
        public static Sys_GridButton GetGridButton(Guid btnId)
        {
            return GetAllGridButtons(x => x.Id == btnId).FirstOrDefault();
        }

        /// <summary>
        /// 获取视图按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="btnText">按钮显示文本</param>
        /// <returns></returns>
        public static Sys_GridButton GetGridButton(Guid moduleId, string btnText)
        {
            return GetGridButtons(moduleId).Where(x => x.ButtonText == btnText).FirstOrDefault();
        }

        /// <summary>
        /// 获取列表工具栏所有按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_GridButton> GetGridButtons(Guid moduleId)
        {
            return GetAllGridButtons(x => x.Sys_ModuleId == moduleId);
        }

        /// <summary>
        /// 获取顶级按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_GridButton> GetTopButtons(Guid moduleId)
        {
            List<Sys_GridButton> list = GetGridButtons(moduleId).Where(x => x.ParentId == null || x.ParentId == Guid.Empty).ToList();
            if (list == null) list = new List<Sys_GridButton>();
            return list;
        }

        /// <summary>
        /// 获取模块的文件菜单按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_GridButton> GetFileMenuButtons(Guid moduleId)
        {
            List<Sys_GridButton> list = GetGridButtons(moduleId);
            List<Sys_GridButton> tempList = new List<Sys_GridButton>();
            foreach (Sys_GridButton btn in list)
            {
                var childBtns = list.Where(x => x.ParentId == btn.Id).ToList();
                if (childBtns != null && childBtns.Count > 0 && btn.OperateButtonType == (int)OperateButtonTypeEnum.FileMenuButton)
                {
                    tempList.Add(btn);
                }
            }
            return tempList;
        }

        /// <summary>
        /// 是否是文件菜单按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="btnId">按钮Id</param>
        /// <returns></returns>
        public static bool IsFileMenuButton(Guid moduleId, Guid btnId)
        {
            Sys_GridButton btn = GetGridButton(btnId);
            if (btn == null) return false;
            return btn.OperateButtonType == (int)OperateButtonTypeEnum.FileMenuButton && HasChildButton(moduleId, btnId);
        }

        /// <summary>
        /// 获取按钮的所有子按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="btnId">按钮Id</param>
        /// <returns></returns>
        public static List<Sys_GridButton> GetChildButtons(Guid moduleId, Guid btnId)
        {
            List<Sys_GridButton> list = GetGridButtons(moduleId);
            if (list != null)
            {
                return list.Where(x => x.ParentId == btnId).ToList();
            }
            return new List<Sys_GridButton>();
        }

        /// <summary>
        /// 获取网格行头按钮集合，在网格行头显示的按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_GridButton> GetGridHeadButtons(Guid moduleId)
        {
            List<Sys_GridButton> list = GetGridButtons(moduleId);
            if (list != null)
            {
                return list.Where(x => x.GridButtonLocation == (int)GridButtonLocationEnum.RowHead).ToList();
            }
            return new List<Sys_GridButton>();
        }

        /// <summary>
        /// 是否有子按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="btnId">按钮Id</param>
        /// <returns></returns>
        public static bool HasChildButton(Guid moduleId, Guid btnId)
        {
            List<Sys_GridButton> list = GetChildButtons(moduleId, btnId);
            return list.Count > 0;
        }

        #endregion

        #region 格式化

        /// <summary>
        /// 视图字段格式化
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="sysField">字段</param>
        /// <param name="gridId">网格domId</param>
        /// <param name="isAllowFieldEdit">允许网格字段编辑</param>
        /// <param name="otherFormatParams">其他格式化参数，主要针对主键字段</param>
        /// <param name="foreignFormatParams">外键格式化参数，针对外键字段</param>
        /// <param name="gridFieldName">网格字段名称</param>
        /// <returns></returns>
        public static string GetGridFormatFunction(Guid moduleId, Sys_Field sysField, string gridId = null, bool isAllowFieldEdit = false, string otherFormatParams = null, string foreignFormatParams = null, string gridFieldName = null)
        {
            Sys_Module module = GetModuleById(moduleId);
            return GetGridFormatFunction(module, sysField, gridId, isAllowFieldEdit, otherFormatParams, foreignFormatParams, gridFieldName);
        }

        /// <summary>
        /// 视图字段格式化
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="sysField">字段</param>
        /// <param name="gridId">网格domId</param>
        /// <param name="isAllowFieldEdit">允许网格字段编辑</param>
        /// <param name="otherFormatParams">其他格式化参数，主要针对主键字段</param>
        /// <param name="foreignFormatParams">外键格式化参数，针对外键字段</param>
        /// <param name="gridFieldName">网格字段名称</param>
        /// <returns></returns>
        public static string GetGridFormatFunction(Sys_Module module, Sys_Field sysField, string gridId = null, bool isAllowFieldEdit = false, string otherFormatParams = null, string foreignFormatParams = null, string gridFieldName = null)
        {
            if (module == null || sysField == null) return string.Empty;
            Sys_FormField field = GetNfDefaultFormSingleField(sysField);
            StringBuilder sb = new StringBuilder();
            if (module == null || sysField == null) return string.Empty;
            Type modelType = CommonOperate.GetModelType(module.TableName); //实体类型
            bool isForeignKey = !string.IsNullOrEmpty(sysField.ForeignModuleName); //IsForeignField(sysField); //是否外键字段
            bool isEnum = IsEnumField(modelType, sysField.Name); //是否枚举字段
            bool isDic = IsDictionaryBindField(module.Id, sysField.Name); //是否字典字段
            Type fieldType = GetFieldType(modelType, sysField.Name); //字段类型
            string paramsObj = string.Empty; //字段参数对象，列表字段编辑时用到
            if (!ModelConfigHelper.ModelIsViewMode(modelType) && isAllowFieldEdit && field != null && field.IsAllowEdit.HasValue && field.IsAllowEdit.Value && field.IsAllowBatchEdit.HasValue && field.IsAllowBatchEdit.Value) //允许编辑
            {
                int w = 300;
                if (field.Sys_FormId.HasValue)
                {
                    Sys_Form form = GetForm(field.Sys_FormId.Value);
                    w = field.Width.HasValue && field.Width.Value > 0 ? field.Width.Value : 180;
                    w += form.LabelWidth > 0 ? form.LabelWidth : 100;
                    w += form.SpaceWidth > 0 ? form.SpaceWidth : 40;
                }
                paramsObj = "{moduleId:'" + module.Id + "',fieldName:'" + sysField.Name + "',fieldDisplay:'" + sysField.Display + "',fieldWidth:" + w + ",gridId:'" + gridId.ObjToStr() + "'}";
                paramsObj = HttpUtility.UrlEncode(paramsObj, Encoding.UTF8).Replace("+", "%20");
            }
            string formatFunction = string.Empty;
            if (module.TitleKey == sysField.Name) //是主键
            {
                string otherParams = otherFormatParams == null ? string.Empty : otherFormatParams;
                formatFunction = string.Format("return TitleKeyFormatter(value, row, index,'{0}','{1}', '{2}','{3}');", module.Name, sysField.Name, paramsObj, otherParams);
            }
            else if (isForeignKey) //外键
            {
                Sys_Module foreignModule = GetForeignModule(sysField);
                if (foreignModule != null)
                {
                    string otherParams = foreignFormatParams == null ? string.Empty : foreignFormatParams;
                    string tempFieldName = string.IsNullOrEmpty(gridFieldName) ? sysField.Name : gridFieldName;
                    formatFunction = string.Format("return ForeignKeyFormatter(value, row, index, '{0}', '{1}','{2}','{3}','{4}','{5}');", module.Name, tempFieldName, foreignModule.Name, paramsObj, otherParams, gridId.ObjToStr());
                }
            }
            else if (field != null && field.ControlTypeOfEnum == ControlTypeEnum.RadioList) //单选框组
            {
                formatFunction = string.Format("return RadioListFormatter(value, row, index,'{0}','{1}','{2}','{3}','{4}');", module.Name, sysField.Name, field.ValueField.ObjToStr(), field.TextField.ObjToStr(), paramsObj);
            }
            else if (isEnum) //枚举字段
            {
                object dic = EnumFieldDataJson(module.Id, sysField.Name);
                string json = HttpUtility.UrlEncode(JsonHelper.Serialize(dic), Encoding.UTF8).Replace("+", "%20"); ;
                formatFunction = string.Format("return EnumFieldFormatter(value, row, index, '{0}', '{1}','{2}','{3}');", module.Name, sysField.Name, json, paramsObj);
            }
            else if (isDic) //字典字段
            {
                object dic = DictionaryDataJson(module.Id, sysField.Name);
                string json = HttpUtility.UrlEncode(JsonHelper.Serialize(dic), Encoding.UTF8).Replace("+", "%20"); ;
                formatFunction = string.Format("return DicFieldFormatter(value, row, index, '{0}', '{1}','{2}','{3}');", module.Name, sysField.Name, json, paramsObj);
            }
            else if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?)) //日期类型
            {
                string format = sysField.Name == "CreateDate" || sysField.Name == "ModifyDate" || (field != null && field.ControlTypeOfEnum == ControlTypeEnum.DateTimeBox) ? "yyyy-MM-dd hh:mm:ss" : "yyyy-MM-dd";
                formatFunction = string.Format("return DateFormatter(value, row, index, '{0}', '{1}','{2}','{3}');", module.Name, sysField.Name, format, paramsObj);
            }
            else if (fieldType == typeof(Boolean) || fieldType == typeof(Boolean?)) //bool类型
            {
                formatFunction = string.Format("return BoolFormatter(value, row, index,'{0}','{1}','{2}');", module.Name, sysField.Name, paramsObj);
            }
            else if (field != null && field.ControlTypeOfEnum == ControlTypeEnum.MutiCheckBox) //多选CheckBox格式化
            {
                formatFunction = string.Format("return MutiCheckBoxFormatter(value, row, index,'{0}','{1}','{2}','{3}');", module.Name, sysField.Name, field.TextField.ObjToStr(), paramsObj);
            }
            else
            {
                formatFunction = string.Format("return GeneralFormatter(value, row, index,'{0}','{1}','{2}','{3}')", module.Name, sysField.Name, paramsObj, fieldType.ObjToStr());
            }
            if (string.IsNullOrEmpty(formatFunction)) return null;
            sb.Append("function(value, row, index)");
            sb.Append("{");
            sb.Append(formatFunction);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 视图字段格式化
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="gridId">网格domId</param>
        /// <param name="isAllowFieldEdit">允许网格字段编辑</param>
        /// <param name="otherFormatParams">其他格式化参数，主要针对主键字段</param>
        /// <param name="foreignFormatParams">外键格式化参数，针对外键字段</param>
        /// <param name="gridFieldName">网格字段名称</param>
        /// <returns></returns>
        public static string GetGridFormatFunction(Guid moduleId, string fieldName, string gridId = null, bool isAllowFieldEdit = false, string otherFormatParams = null, string foreignFormatParams = null, string gridFieldName = null)
        {
            Sys_Field sysField = GetFieldInfo(moduleId, fieldName);
            string formatString = GetGridFormatFunction(moduleId, sysField, gridId, isAllowFieldEdit, otherFormatParams, foreignFormatParams, gridFieldName);
            return formatString;
        }

        /// <summary>
        /// 网格字段格式化，用于自定义调用
        /// </summary>
        /// <param name="userId">当前用户</param>
        /// <param name="sysField">字段</param>
        /// <param name="gridFieldName">网格字段名</param>
        /// <param name="gridId">网格domid</param>
        /// <param name="isAllowEditField">字段是否允许编辑</param>
        /// <returns></returns>
        public static string GetComplexGridFormatFun(Guid userId, Sys_Field sysField, string gridFieldName, string gridId = "mainGrid", bool isAllowEditField = false)
        {
            if (sysField == null || !sysField.Sys_ModuleId.HasValue) return null;
            Sys_Module module = SystemOperate.GetModuleById(sysField.Sys_ModuleId.Value);
            string otherFormatParams = string.Empty;
            string foreignFormatParams = string.Empty; //外键格式化参数
            if (!CommonDefine.BaseEntityFields.Contains(sysField.Name) && !string.IsNullOrWhiteSpace(sysField.ForeignModuleName)) //外键模块处理
            {
                Sys_Module foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName);
                if (foreignModule != null)
                {
                    Sys_Form foreginForm = SystemOperate.GetUserForm(userId, foreignModule.Id); //表单对象
                    if (foreginForm.ModuleEditModeOfEnum == ModuleEditModeEnum.None ||
                        foreginForm.ModuleEditModeOfEnum == ModuleEditModeEnum.PopFormEdit ||
                        foreginForm.ModuleEditModeOfEnum == ModuleEditModeEnum.TabFormEdit)
                    {
                        int ew = foreginForm.Width.HasValue && foreginForm.Width.Value > 0 ? foreginForm.Width.Value : 500; //外键表单宽度
                        int eh = foreginForm.Height.HasValue && foreginForm.Height.Value > 0 ? foreginForm.Height.Value : 300; //外键表单高度
                        int em = (int)(foreginForm.ModuleEditModeOfEnum == ModuleEditModeEnum.None ? ModuleEditModeEnum.PopFormEdit : foreginForm.ModuleEditModeOfEnum);
                        string foreignTitleKey = string.IsNullOrEmpty(foreignModule.TitleKey) ? string.Empty : foreignModule.TitleKey;
                        string foreignTitleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(foreignModule);
                        foreignFormatParams = HttpUtility.UrlEncode("{moduleId:'" + foreignModule.Id + "',titleKey:'" + foreignTitleKey + "',titleKeyDisplay:'" + foreignTitleKeyDisplay + "',editMode:" + em + ",editWidth:" + ew + ",editHeight:" + eh + "}", Encoding.UTF8);
                    }
                }
            }
            string formatStr = SystemOperate.GetGridFormatFunction(module, sysField, gridId, isAllowEditField, otherFormatParams, foreignFormatParams, gridFieldName);
            return formatStr;
        }

        /// <summary>
        /// 获取字段编辑器
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string GetFieldEditor(Sys_Module module, string fieldName)
        {
            Sys_Field sysField = GetFieldInfo(module.Id, fieldName);
            return GetFieldEditor(module, sysField);
        }

        /// <summary>
        /// 获取字段编辑器
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="sysField">字段</param>
        /// <returns></returns>
        public static string GetFieldEditor(Sys_Module module, Sys_Field sysField)
        {
            if (module == null || sysField == null) return string.Empty;
            Sys_FormField formField = GetNfDefaultFormSingleField(sysField);
            if (formField == null || !formField.IsAllowEdit.HasValue || !formField.IsAllowEdit.Value)
                return string.Empty;
            return GetFieldEditor(module, sysField, formField);
        }

        /// <summary>
        /// 获取字段编辑器
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="sysField">字段</param>
        /// <param name="formField">表单字段</param>
        /// <returns></returns>
        public static string GetFieldEditor(Sys_Module module, Sys_Field sysField, Sys_FormField formField)
        {
            if (module == null || sysField == null) return string.Empty;
            if (formField == null || !formField.IsAllowEdit.HasValue || !formField.IsAllowEdit.Value)
                return string.Empty;
            object value = formField.DefaultValue;
            StringBuilder sb = new StringBuilder();
            string valueField = formField.ValueField;
            string textField = formField.TextField;
            string fieldUrl = formField.UrlOrData;
            #region 外键字段、枚举字段、字典字段处理
            if (formField.UrlOrData == null && formField.ControlTypeOfEnum == ControlTypeEnum.ComboBox)
            {
                //外键字段
                if (SystemOperate.IsForeignField(sysField))
                {
                    if (value == null) value = Guid.Empty;
                    valueField = "Id";
                    textField = "Name";
                    fieldUrl = string.Format("/{0}/BindForeignFieldComboData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, module.Id.ToString(), sysField.Name);
                }
                else if (SystemOperate.IsEnumField(module.Id, sysField.Name)) //枚举字段
                {
                    valueField = "Id";
                    textField = "Name";
                    fieldUrl = string.Format("/{0}/BindEnumFieldData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, module.Id.ToString(), sysField.Name);
                }
                else if (SystemOperate.IsDictionaryBindField(module.Id, sysField.Name)) //字典绑定字段
                {
                    valueField = "Id";
                    textField = "Name";
                    fieldUrl = string.Format("/{0}/BindDictionaryData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, module.Id.ToString(), sysField.Name);
                }
            }
            #endregion
            #region 验证处理
            string options = "onChange:function(newValue,oldValue){if(typeof(OnFieldValueChanged)=='function'){OnFieldValueChanged({moduleId:'" + module.Id + "',moduleName:'" + module.Name + "'},'" + sysField.Name + "',newValue,oldValue);}}";
            if (!string.IsNullOrEmpty(formField.NullTipText))
                options += string.Format(",prompt:'{0}'", formField.NullTipText);
            //必填性验证
            if (formField.IsRequired.HasValue && formField.IsRequired.Value)
            {
                options += ",required:true";
            }
            //字符长度验证
            string validTypeStr = string.Empty;
            if (formField.MinCharLen.HasValue && formField.MinCharLen.Value > 0 && formField.MaxCharLen.HasValue && formField.MaxCharLen.Value > 0)
            {
                validTypeStr = string.Format("'length[{0},{1}]'", formField.MinCharLen.Value.ToString(), formField.MaxCharLen.Value.ToString());
            }
            else if (formField.MinCharLen.HasValue && formField.MinCharLen.Value > 0)
            {
                validTypeStr = string.Format("'minLength[{0}]'", formField.MinCharLen.Value.ToString());
            }
            else if (formField.MaxCharLen.HasValue && formField.MaxCharLen.Value > 0)
            {
                validTypeStr = string.Format("'maxLength:[{0}]'", formField.MaxCharLen.Value.ToString());
            }
            //其他验证类型
            switch (formField.ValidateTypeOfEnum)
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
                options += string.Format(",validType:[{0}]", validTypeStr);
            }
            #endregion
            #region 控件处理
            switch (formField.ControlTypeOfEnum)
            {
                case ControlTypeEnum.RichTextBox:
                case ControlTypeEnum.TextBox:
                case ControlTypeEnum.TextAreaBox:
                case ControlTypeEnum.IconBox: //图标控件
                    {
                        sb.Append("{type:'textbox',options:{");
                        if (formField.ControlTypeOfEnum == ControlTypeEnum.TextAreaBox ||
                            formField.ControlTypeOfEnum == ControlTypeEnum.RichTextBox)
                            options += ",multiline:true";
                        options += string.Format(",value:'{0}'", value.ObjToStr());
                        sb.Append(options);
                        sb.Append("}}");
                    }
                    break;
                case ControlTypeEnum.DialogTree: //弹出选择树
                case ControlTypeEnum.DialogGrid: //外键弹出框
                    {

                        sb.Append("{type:'textbox',options:{");
                        options += string.Format(",value:'{0}'", value.ObjToStr());
                        string fieldAttr = string.Empty;
                        Sys_Module foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName); //外键模块
                        if (foreignModule != null)
                        {
                            bool isMutiSelect = formField.IsMultiSelect == true && SystemOperate.GetFieldType(module.Id, sysField.Name) == typeof(String);
                            if (string.IsNullOrEmpty(fieldUrl))
                            {
                                fieldUrl = formField.ControlTypeOfEnum == ControlTypeEnum.DialogGrid ? string.Format("/Page/Grid.html?page=fdGrid&moduleId={0}&initModule={1}&initField={2}", foreignModule.Id.ToString(), HttpUtility.UrlEncode(module.Name), sysField.Name) :
                                          string.Format("/Page/DialogTree.html?moduleName={0}", HttpUtility.UrlEncode(foreignModule.Name));
                                if (isMutiSelect) fieldUrl += "&ms=1";
                            }
                            if (string.IsNullOrEmpty(valueField)) valueField = "Id";
                            if (string.IsNullOrEmpty(textField)) textField = SystemOperate.GetModuleTitleKey(foreignModule.Id);
                            fieldAttr = HttpUtility.UrlEncode("{" + string.Format("url:'{0}',textField:'{1}',valueField:'{2}',foreignModuleId:'{3}',foreignModuleName:'{4}',foreignModuleDisplay:'{5}',ms:'{6}',isTree:'{7}'", fieldUrl, textField, valueField, foreignModule.Id.ToString(), HttpUtility.UrlEncode(foreignModule.Name), HttpUtility.UrlEncode(foreignModule.Display.ObjToStr()), isMutiSelect ? "1" : string.Empty, formField.ControlTypeOfEnum == ControlTypeEnum.DialogTree ? "1" : string.Empty) + "}", Encoding.UTF8).Replace("+", "%20"); ;
                        }
                        options += ",icons: [{iconCls:'eu-icon-search',handler: function(e){SelectDialogData($(e.data.target),null,'" + fieldAttr + "');}}]";
                        sb.Append(options);
                        sb.Append("}}");
                    }
                    break;
                case ControlTypeEnum.IntegerBox:
                case ControlTypeEnum.NumberBox:
                    {
                        sb.Append("{type:'numberbox',options:{");
                        int precision = sysField.Precision.HasValue ? sysField.Precision.Value : 2;
                        if (formField.ControlTypeOfEnum == ControlTypeEnum.IntegerBox) precision = 0;
                        options += string.Format(",precision:{0}", precision.ToString());
                        if (formField.MinValue.HasValue)
                            options += string.Format(",min:{0}", formField.MinValue.Value.ToString());
                        if (formField.MaxValue.HasValue)
                            options += string.Format(",max:{0}", formField.MaxValue.Value.ToString());
                        if (formField.DefaultValue.ObjToDouble() > 0)
                            options += string.Format(",value:'{0}'", value.ObjToStr());
                        sb.Append(options);
                        sb.Append("}}");
                    }
                    break;
                case ControlTypeEnum.SingleCheckBox:
                    {
                        sb.Append("{type:'checkbox',options:{on:'1',off:'0'}}");
                    }
                    break;
                case ControlTypeEnum.DateBox:
                case ControlTypeEnum.DateTimeBox:
                    {
                        string boxType = "datebox";
                        if (formField.ControlTypeOfEnum == ControlTypeEnum.DateTimeBox)
                            boxType = "datetimebox";
                        sb.Append("{type:'" + boxType + "',options:{");
                        options += string.Format(",value:'{0}'", value.ObjToStr());
                        sb.Append(options);
                        sb.Append("}}");
                    }
                    break;
                case ControlTypeEnum.ComboBox: //下拉框
                case ControlTypeEnum.ComboTree: //下拉树
                    {
                        string loadFilterElseStr = string.Empty;
                        if (value == null && !string.IsNullOrEmpty(sysField.ForeignModuleName))
                            value = Guid.Empty;
                        string isTreeStr = "false";
                        if (formField.ControlTypeOfEnum == ControlTypeEnum.ComboTree)
                        {
                            isTreeStr = "true";
                            if (string.IsNullOrEmpty(valueField)) valueField = "id";
                            if (string.IsNullOrEmpty(textField)) textField = "text";
                            Sys_Module foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName);
                            if (fieldUrl == null && foreignModule != null)
                            {
                                fieldUrl = string.Format("/{0}/GetTreeByNode.html?moduleId={1}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, foreignModule.Id.ToString());
                            }
                            loadFilterElseStr = " else {if(typeof (data)== 'string'){var tempData=eval('(' + data + ')');return tempData;} else{var arr=[];arr.push(data);return arr;}}";
                            options += ",delay:300,editable:true,keyHandler:{query:function(q){QueryComboTree(q, '" + sysField.Name + "');}}";
                        }
                        else
                        {
                            loadFilterElseStr = " else {if(typeof (data)== 'string'){var tempData=eval('(' + data + ')');return tempData;} else{return data;}}";
                            if (SystemOperate.IsEnumField(module.Id, sysField.Name))
                                options += ",editable:false";
                        }
                        if (fieldUrl != null && fieldUrl.StartsWith("[{") && fieldUrl.EndsWith("}]")) //下拉数据固定data
                        {
                            options += string.Format(",valueField:'{0}',textField:'{1}',data:{2}", valueField, textField, fieldUrl);
                        }
                        else //从URL取数据
                        {
                            options += string.Format(",valueField:'{0}',textField:'{1}',url:'{2}'", valueField, textField, fieldUrl.ObjToStr());
                        }
                        options += ",onLoadSuccess:function(){if(typeof(OnFieldLoadSuccess)=='function'){ OnFieldLoadSuccess('" + sysField.Name + "','" + valueField + "','" + textField + "'," + isTreeStr + ");}}";
                        options += ",loadFilter:function(data,parent){if(typeof(OnLoadFilter)=='function'){return OnLoadFilter('" + sysField.Name + "','" + valueField + "','" + textField + "',data,parent," + isTreeStr + ");}" + loadFilterElseStr + "}";
                        options += ",onLoadError:function(arguments){if(typeof(OnLoadError)=='function'){OnLoadError('" + sysField.Name + "','" + valueField + "','" + textField + "',arguments);}}";
                        if (formField.IsMultiSelect.HasValue && formField.IsMultiSelect.Value)
                        {
                            options += ",multiple:true";
                        }
                        string comboType = formField.ControlTypeOfEnum == ControlTypeEnum.ComboTree ? "combotree" : "combobox";
                        sb.Append("{type:'" + comboType + "',options:{");
                        options += string.Format(",valueField:'{0}',textField:'{1}',url:'{2}'", valueField, textField, fieldUrl);
                        options += string.Format(",value:'{0}'", value.ObjToStr());
                        options += ",panelMinWidth:200";
                        sb.Append(options);
                        sb.Append("}}");
                    }
                    break;
            }
            #endregion
            return sb.ToString();
        }

        #endregion

        #endregion

        #region 表单

        #region 表单字段

        /// <summary>
        /// 获取默认表单字段，不加载外键Name字段
        /// </summary>
        /// <param name="sysField"></param>
        /// <returns></returns>
        public static Sys_FormField GetNfDefaultFormSingleField(Sys_Field sysField)
        {
            if (sysField == null || !sysField.Sys_ModuleId.HasValue) return null;
            string errMsg = string.Empty;
            Sys_Form defaultForm = GetDefaultForm(sysField.Sys_ModuleId.Value);
            Sys_FormField formField = CommonOperate.GetEntity<Sys_FormField>(x => x.Sys_FieldId == sysField.Id && x.Sys_FormId == defaultForm.Id, null, out errMsg);
            return formField;
        }

        /// <summary>
        /// 获取默认表单单个字段，不加载外键Name字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static Sys_FormField GetNfDefaultFormSingleField(Guid moduleId, string fieldName)
        {
            Sys_Field sysField = GetFieldInfo(moduleId, fieldName);
            return GetNfDefaultFormSingleField(sysField);
        }

        /// <summary>
        /// 获取默认表单单个字段，加载外键Name字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static Sys_FormField GetDefaultFormSingleField(Guid moduleId, string fieldName)
        {
            List<Sys_FormField> fields = GetDefaultFormField(moduleId);
            if (fields != null && fields.Count > 0)
            {
                return fields.Where(x => x.Sys_FieldName == fieldName).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 获取默认表单单个字段
        /// </summary>
        /// <param name="sysField">字段信息</param>
        /// <returns></returns>
        public static Sys_FormField GetDefaultFormSingleField(Sys_Field sysField)
        {
            if (sysField == null || !sysField.Sys_ModuleId.HasValue) return null;
            return GetDefaultFormSingleField(sysField.Sys_ModuleId.Value, sysField.Name);
        }

        /// <summary>
        /// 获取模块默认表单单个字段
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static Sys_FormField GetDefaultFormSingleField(string moduleName, string fieldName)
        {
            Guid moduleId = GetModuleIdByName(moduleName);
            return GetDefaultFormSingleField(moduleId, fieldName);
        }

        /// <summary>
        /// 获取模块默认表单字段集合
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="reference">是否加载导航，默认为是</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetDefaultFormField(Guid moduleId, bool reference = true)
        {
            //获取表单
            Sys_Form form = GetDefaultForm(moduleId);
            if (form == null) return new List<Sys_FormField>();
            List<Sys_FormField> formFields = GetFormField(moduleId, form.Name, reference);
            return formFields;
        }

        /// <summary>
        /// 获取表单字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="formName">表单名称</param>
        /// <param name="reference">是否加载导航，默认为是</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetFormField(Guid moduleId, string formName, bool reference = true)
        {
            //获取表单
            Sys_Form form = GetForm(moduleId, formName);
            if (form == null) return new List<Sys_FormField>();
            //获表单表字段
            List<Sys_FormField> formFields = GetFormField(form.Id, reference);
            return formFields;
        }

        /// <summary>
        /// 获取表单字段
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="formName">表单名称</param>
        /// <param name="reference">是否加载导航</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetFormField(string moduleName, string formName, bool reference = true)
        {
            //获取表单
            Sys_Form form = GetForm(moduleName, formName);
            if (form == null) return new List<Sys_FormField>();
            if (form.FormFields != null && form.FormFields.Count > 0)
                return form.FormFields;
            //获表单表字段
            List<Sys_FormField> formFields = GetFormField(form.Id, reference);
            if (formFields.Count > 0)
            {
                form.FormFields = formFields;
            }
            return formFields;
        }

        /// <summary>
        /// 获取表单字段
        /// </summary>
        /// <param name="formId">表单id</param>
        /// <param name="reference">是否加载导航，默认为是</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetFormField(Guid formId, bool reference = true)
        {
            string errMsg = string.Empty;
            //获表单表字段
            List<Sys_FormField> formFields = CommonOperate.GetEntities<Sys_FormField>(out errMsg, x => x.Sys_FormId == formId && x.IsDeleted == false && x.IsDraft == false, null, false);
            if (formFields == null) formFields = new List<Sys_FormField>();
            if (reference)
            {
                formFields.ForEach(x =>
                {
                    if (x.Sys_FieldId.HasValue && string.IsNullOrEmpty(x.Sys_FieldName))
                    {
                        Sys_Field sysField = GetFieldById(x.Sys_FieldId.Value);
                        if (sysField != null)
                            x.Sys_FieldName = sysField.Name;
                    }
                });
            }
            formFields = formFields.OrderBy(x => x.ColNo).OrderBy(x => x.RowNo).ToList();
            return formFields;
        }

        /// <summary>
        /// 获取用户表单字段
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetUserFormFields(Guid userId, Guid moduleId)
        {
            Sys_Form form = GetUserForm(userId, moduleId);
            if (form != null)
            {
                if (form.FormFields != null && form.FormFields.Count > 0)
                    return form.FormFields;
                List<Sys_FormField> formFields = GetFormField(form.Id, false);
                if (formFields.Count > 0)
                {
                    form.FormFields = formFields;
                }
                return formFields;
            }
            return new List<Sys_FormField>();
        }

        /// <summary>
        /// 获取角色表单字段集合
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetRoleFormFields(Guid roleId, Guid moduleId)
        {
            Sys_Form form = GetRoleForm(roleId, moduleId);
            if (form != null)
            {
                if (form.FormFields != null && form.FormFields.Count > 0)
                    return form.FormFields;
                List<Sys_FormField> formFields = GetFormField(form.Id, false);
                if (formFields.Count > 0)
                {
                    form.FormFields = formFields;
                }
                return formFields;
            }
            return new List<Sys_FormField>();
        }

        /// <summary>
        /// 获取单个用户表单字段
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="sysField">字段信息</param>
        /// <returns></returns>
        public static Sys_FormField GetUserFormField(Guid userId, Guid moduleId, Sys_Field sysField)
        {
            if (sysField == null) return null;
            List<Sys_FormField> list = GetUserFormFields(userId, moduleId);
            return list.Where(x => x.Sys_FieldId == sysField.Id).FirstOrDefault();
        }

        /// <summary>
        /// 获取单个角色表单字段
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="sysField">字段信息</param>
        /// <returns></returns>
        public static Sys_FormField GetRoleFormField(Guid roleId, Guid moduleId, Sys_Field sysField)
        {
            if (sysField == null) return null;
            List<Sys_FormField> list = GetRoleFormFields(roleId, moduleId);
            return list.Where(x => x.Sys_FieldId == sysField.Id).FirstOrDefault();
        }

        /// <summary>
        /// 获取搜索表单字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetSearchFormField(Guid moduleId)
        {
            //先取搜索视图字段
            List<Sys_GridField> searchFields = GetDefaultSearchGridFields(moduleId);
            //找到对应的表单字段
            List<Guid> fieldIds = searchFields.Select(x => x.Sys_FieldId.Value).ToList();
            List<Sys_FormField> formFields = GetDefaultFormField(moduleId, false);
            List<Sys_FormField> list = new List<Sys_FormField>();
            //按搜索视图字段的顺序
            foreach (Guid fieldId in fieldIds)
            {
                Sys_FormField formField = formFields.Where(x => x.Sys_FieldId.Value == fieldId).FirstOrDefault();
                if (formField != null)
                {
                    list.Add(formField);
                }
            }
            return list;
        }

        /// <summary>
        /// 格式化表单字段
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>返回格式化后的字段</returns>
        public static Sys_FormField FormatFormField(Sys_Module module, string fieldName)
        {
            Sys_Field sysField = SystemOperate.GetFieldInfo(module.Id, fieldName);
            Sys_FormField field = SystemOperate.GetNfDefaultFormSingleField(sysField);
            Sys_Module foreignModule = SystemOperate.GetForeignModule(sysField);
            #region 外键字段、枚举字段、字典字段处理
            string valueField = string.IsNullOrWhiteSpace(field.ValueField) ? "Id" : field.ValueField;
            string textField = string.IsNullOrWhiteSpace(field.TextField) ? "Name" : field.TextField;
            string fieldUrl = field.UrlOrData;
            if (field.UrlOrData == null && field.ControlTypeOfEnum == ControlTypeEnum.ComboBox)
            {
                //外键字段
                if (SystemOperate.IsForeignField(sysField))
                {
                    valueField = "Id";
                    textField = "Name";
                    fieldUrl = string.Format("/{0}/BindForeignFieldComboData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, module.Id.ToString(), fieldName);
                }
                else if (SystemOperate.IsEnumField(module.Id, fieldName)) //枚举字段
                {
                    valueField = "Id";
                    textField = "Name";
                    fieldUrl = string.Format("/{0}/BindEnumFieldData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, module.Id.ToString(), fieldName);
                }
                else if (SystemOperate.IsDictionaryBindField(module.Id, fieldName)) //字典绑定字段
                {
                    valueField = "Id";
                    textField = "Name";
                    fieldUrl = string.Format("/{0}/BindDictionaryData.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, module.Id.ToString(), fieldName);
                }
            }
            else if (foreignModule != null)
            {
                if (field.ControlTypeOfEnum == ControlTypeEnum.ComboTree)
                {
                    if (string.IsNullOrEmpty(valueField)) valueField = "id";
                    if (string.IsNullOrEmpty(textField)) textField = "text";
                    if (fieldUrl == null && foreignModule != null)
                    {
                        fieldUrl = string.Format("/{0}/GetTreeByNode.html?moduleId={1}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, foreignModule.Id.ToString());
                    }
                }
                else
                {
                    fieldUrl = string.Format("/Page/Grid.html?page=fdGrid&moduleId={0}&initModule={1}&initField={2}", foreignModule.Id.ToString(), HttpUtility.UrlEncode(module.Name), fieldName);
                    if (string.IsNullOrEmpty(textField)) textField = SystemOperate.GetModuleTitleKey(foreignModule.Id);
                }
            }
            #endregion
            Sys_FormField tempField = new Sys_FormField();
            ObjectHelper.CopyValue(field, tempField);
            tempField.Sys_FieldName = fieldName;
            tempField.ValueField = valueField;
            tempField.TextField = textField;
            tempField.UrlOrData = fieldUrl;
            return tempField;
        }

        /// <summary>
        /// 获取默认表单批量编辑字段集合
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetDefaultBatchEditFields(Guid moduleId)
        {
            List<Sys_FormField> list = GetDefaultFormField(moduleId, false);
            if (list != null && list.Count > 0)
            {
                Sys_Module module = GetModuleById(moduleId);
                list = list.Where(x => x.Sys_FieldId.HasValue && x.IsAllowBatchEdit.HasValue && x.IsAllowBatchEdit.Value && x.Sys_FieldName != "Id" && x.Sys_FieldName != module.TitleKey).ToList();
                list = list.OrderBy(x => x.ColNo).OrderBy(x => x.RowNo).ToList();
            }
            if (list == null) list = new List<Sys_FormField>();
            return list;
        }

        /// <summary>
        /// 获取表单批量编辑字段集合
        /// </summary>
        /// <param name="formId">表单Id</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetBatchEditFields(Guid formId)
        {
            List<Sys_FormField> list = GetFormField(formId, false);
            if (list != null && list.Count > 0)
            {
                list = list.Where(x => x.IsAllowBatchEdit.HasValue && x.IsAllowBatchEdit.Value).ToList();
                list = list.OrderBy(x => x.ColNo).OrderBy(x => x.RowNo).ToList();
            }
            if (list == null) list = new List<Sys_FormField>();
            return list;
        }

        /// <summary>
        /// 获取唯一性验证字段集合
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetUniqueVerifyFields(Guid moduleId)
        {
            Sys_Form form = GetDefaultForm(moduleId);
            List<Sys_FormField> list = GetFormField(form.Id, false).Where(x => x.IsUnique == true).ToList();
            return list;
        }

        /// <summary>
        /// 获取默认图片上传表单字段
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetDefaultImageUploadFormFields(Guid moduleId)
        {
            Sys_Form form = GetDefaultForm(moduleId);
            if (form == null) return new List<Sys_FormField>();
            return GetImageUploadFormFields(form.Id);
        }

        /// <summary>
        /// 获取表单图片上传表单字段
        /// </summary>
        /// <param name="formId">表单ID</param>
        /// <returns></returns>
        public static List<Sys_FormField> GetImageUploadFormFields(Guid formId)
        {
            string errMsg = string.Empty;
            int imageUpload = (int)ControlTypeEnum.ImageUpload;
            return GetFormField(formId).Where(x => x.ControlType == imageUpload).ToList();
        }

        #endregion

        #region 表单

        /// <summary>
        /// 获取所有表单集合
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_Form> GetAllForms(Expression<Func<Sys_Form, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Sys_Form, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => !x.IsDeleted && !x.IsDraft) : x => !x.IsDeleted && !x.IsDraft;
            List<Sys_Form> list = CommonOperate.GetEntities<Sys_Form>(out errMsg, tempExp, null, false);
            if (list == null) list = new List<Sys_Form>();
            return list;
        }

        /// <summary>
        /// 获取所有角色表单集合
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        public static List<Sys_RoleForm> GetAllRoleForms(Expression<Func<Sys_RoleForm, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Sys_RoleForm, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => !x.IsDeleted && !x.IsDraft) : x => !x.IsDeleted && !x.IsDraft;
            List<Sys_RoleForm> list = CommonOperate.GetEntities<Sys_RoleForm>(out errMsg, tempExp, null, false);
            if (list == null) list = new List<Sys_RoleForm>();
            return list;
        }

        /// <summary>
        /// 获取默认表单
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static Sys_Form GetDefaultForm(Guid moduleId)
        {
            return GetAllForms(x => x.Sys_ModuleId == moduleId && x.IsDefault).FirstOrDefault();
        }

        /// <summary>
        /// 获取模块表单
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Sys_Form> GetModuleForms(Guid moduleId)
        {
            return GetAllForms(x => x.Sys_ModuleId == moduleId);
        }

        /// <summary>
        /// 获取表单
        /// </summary>
        /// <param name="formId">表单id</param>
        /// <returns></returns>
        public static Sys_Form GetForm(Guid formId)
        {
            return GetAllForms(x => x.Id == formId).FirstOrDefault();
        }

        /// <summary>
        /// 获取表单
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="formName">表单名称</param>
        /// <returns></returns>
        public static Sys_Form GetForm(Guid moduleId, string formName)
        {
            return GetAllForms(x => x.Sys_ModuleId == moduleId && x.Name == formName).FirstOrDefault();
        }

        /// <summary>
        /// 获取表单
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="formName">表单名称</param>
        /// <returns></returns>
        public static Sys_Form GetForm(string moduleName, string formName)
        {
            Guid moduleId = GetModuleIdByName(moduleName);
            return GetForm(moduleId, formName);
        }

        /// <summary>
        /// 获取角色表单
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static Sys_Form GetRoleForm(Guid roleId, Guid moduleId)
        {
            Sys_Form form = null;
            Sys_RoleForm roleForm = GetAllRoleForms(x => x.Sys_RoleId == roleId && x.Sys_ModuleId == moduleId).FirstOrDefault();
            if (roleForm != null && roleForm.Sys_FormId.HasValue)
            {
                form = GetForm(roleForm.Sys_FormId.Value);
            }
            if (form == null)
            {
                form = GetDefaultForm(moduleId);
            }
            return form;
        }

        /// <summary>
        /// 获取用户表单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static Sys_Form GetUserForm(Guid userId, Guid moduleId)
        {
            Sys_Form form = null;
            List<Sys_UserRole> userRoles = PermissionOperate.GetAllUserRoles(x => x.Sys_UserId == userId);
            if (userRoles != null && userRoles.Count > 0)
            {
                List<Guid?> roleIds = userRoles.Select(x => x.Sys_RoleId).ToList();
                Sys_RoleForm roleForm = GetAllRoleForms(x => roleIds.Contains(x.Sys_RoleId) && x.Sys_ModuleId == moduleId).FirstOrDefault();
                if (roleForm != null && roleForm.Sys_FormId.HasValue)
                {
                    form = GetForm(roleForm.Sys_FormId.Value);
                }
            }
            if (form == null)
            {
                form = GetDefaultForm(moduleId);
            }
            return form;
        }

        /// <summary>
        /// 获取用户最终表单
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <returns></returns>
        public static Sys_Form GetUserFinalForm(UserInfo currUser, Guid moduleId, Guid? todoTaskId = null)
        {
            if (currUser == null)
                return GetDefaultForm(moduleId);
            Sys_Form form = null; //表单对象
            bool isEnabledFlow = BpmOperate.IsEnabledWorkflow(moduleId); //是否启用流程
            Sys_Form tempForm = SystemOperate.GetUserForm(currUser.UserId, moduleId);
            if (isEnabledFlow && tempForm != null && tempForm.Id == SystemOperate.GetDefaultForm(moduleId).Id)
            {
                Sys_Form flowNodeForm = null;
                if (todoTaskId.HasValue && todoTaskId.Value != Guid.Empty)
                {
                    if (BpmOperate.IsCurrentToDoTaskHandler(todoTaskId.Value, currUser))
                    {
                        flowNodeForm = BpmOperate.GetWorkNodeForm(BpmOperate.GetWorkNodeIdByTodoId(todoTaskId.Value));
                    }
                }
                else
                {
                    if (BpmOperate.IsAllowLaunchFlow(moduleId, currUser, null))
                    {
                        flowNodeForm = BpmOperate.GetLaunchNodeForm(moduleId);
                    }
                }
                if (flowNodeForm != null && flowNodeForm.Id != tempForm.Id)
                    form = flowNodeForm;
                else
                    form = tempForm;
            }
            else
            {
                form = tempForm;
            }
            return form;
        }

        #endregion

        #region 表单按钮

        /// <summary>
        /// 获取表单按钮
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="formType">表单类型</param>
        /// <param name="isAdd">是否新增表单</param>
        /// <param name="isDraft">是否是草稿</param>
        /// <param name="recordId">表单记录ID</param>
        /// <param name="toDoTaskId">待办ID</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static List<FormButton> GetFormButtons(Guid moduleId, FormTypeEnum formType, bool isAdd = false, bool isDraft = false, Guid? recordId = null, Guid? toDoTaskId = null, UserInfo currUser = null)
        {
            Sys_Module module = GetModuleById(moduleId);
            return GetFormButtons(module, formType, isAdd, isDraft, recordId, toDoTaskId, currUser);
        }

        /// <summary>
        /// 获取表单按钮
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="formType">表单类型</param>
        /// <param name="isAdd">是否新增表单</param>
        /// <param name="isDraft">是否是草稿</param>
        /// <param name="recordId">表单记录ID</param>
        /// <param name="toDoTaskId">待办任务ID,针对流程表单</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static List<FormButton> GetFormButtons(Sys_Module module, FormTypeEnum formType, bool isAdd = false, bool isDraft = false, Guid? recordId = null, Guid? toDoTaskId = null, UserInfo currUser = null)
        {
            if (currUser == null) return new List<FormButton>();
            List<FormButton> btns = new List<FormButton>();
            bool isEnabledFlow = BpmOperate.IsEnabledWorkflow(module.Id); //是否启用流程
            switch (formType)
            {
                case FormTypeEnum.EditForm: //新增、编辑页面
                    {
                        if (!isEnabledFlow || (isEnabledFlow && (!toDoTaskId.HasValue || toDoTaskId.Value == Guid.Empty))) //未启用流程或启用流程并且是发起
                        {
                            bool canAdd = PermissionOperate.HasButtonPermission(currUser, module.Id, "新增");
                            bool canEdit = PermissionOperate.HasButtonPermission(currUser, module.Id, "编辑");
                            if (canAdd || canEdit) //有新增或编辑权限
                            {
                                btns.Add(new FormButton()
                                {
                                    TagId = "btnSave",
                                    DisplayText = "保存",
                                    IconType = ButtonIconType.Save,
                                    ClickMethod = "Save(this)",
                                    Icon = "eu-icon-save",
                                    Sort = 0
                                });
                            }
                            if (isDraft && (canAdd || canEdit)) //是否草稿，草稿时表单添加保存发布按钮
                            {
                                btns.Add(new FormButton()
                                {
                                    TagId = "btnSaveRelease",
                                    DisplayText = "保存并发布",
                                    IconType = ButtonIconType.DraftRelease,
                                    ClickMethod = "Save(this)",
                                    Icon = "eu-icon-ok",
                                    Sort = 1,
                                });
                            }
                            else if (isAdd && canAdd && !SystemOperate.IsDetailModule(module.Id) && module.IsEnabledDraft) //启用草稿
                            {
                                btns.Add(new FormButton()
                                {
                                    TagId = "btnSaveDraft",
                                    DisplayText = "保存为草稿",
                                    IconType = ButtonIconType.SaveDraft,
                                    ClickMethod = "Save(this,null,false,true)",
                                    Icon = "icon-draft",
                                    Sort = 2
                                });
                            }
                            if (module.IsAllowAdd && !isDraft && canAdd)
                            {
                                btns.Add(new FormButton()
                                {
                                    TagId = "btnSaveAndAdd",
                                    DisplayText = "保存并新增",
                                    IconType = ButtonIconType.SaveAndNew,
                                    ClickMethod = "Save(this,null,true)",
                                    Icon = "eu-icon-save",
                                    Sort = 3
                                });
                            }
                            if ((!module.ParentId.HasValue || module.ParentId.Value == Guid.Empty) && isEnabledFlow && BpmOperate.IsAllowLaunchFlow(module.Id, currUser, recordId)) //发起流程
                            {
                                btns.AddRange(BpmOperate.GetLaunchNodeFlowBtns());
                            }
                        }
                        else if (recordId.HasValue && recordId.Value != Guid.Empty)//审批流程
                        {
                            if ((!module.ParentId.HasValue || module.ParentId.Value == Guid.Empty))
                            {
                                if (BpmOperate.IsCurrentToDoTaskHandler(toDoTaskId.Value, currUser)) //为当前审批人时
                                {
                                    btns.AddRange(BpmOperate.GetNodeFlowBtns(toDoTaskId.Value, currUser));
                                }
                                else if (currUser != null && currUser.UserName == "admin") //管理员添加指派功能
                                {
                                    string errMsg = string.Empty;
                                    bool isParentTodo = false;
                                    Bpm_WorkToDoList todo = CommonOperate.GetEntityById<Bpm_WorkToDoList>(toDoTaskId.Value, out errMsg);
                                    if (todo == null)
                                    {
                                        Bpm_WorkToDoListHistory todoHistory = CommonOperate.GetEntityById<Bpm_WorkToDoListHistory>(toDoTaskId.Value, out errMsg);
                                        if (todoHistory != null)
                                            isParentTodo = todoHistory.IsParentTodo == true;
                                    }
                                    else
                                    {
                                        isParentTodo = todo.IsParentTodo == true;
                                    }
                                    FormButton tempBtn = new FormButton() { TagId = string.Format("flowBtn_{0}", Guid.NewGuid().ToString()), IconType = ButtonIconType.FlowDirect, Icon = "eu-icon-user", DisplayText = "指派", ClickMethod = "DirectFlow(this)", Sort = 7 };
                                    if (isParentTodo)
                                        tempBtn.ParentToDoId = toDoTaskId.Value.ToString();
                                    btns.Add(tempBtn);
                                }
                            }
                        }
                    }
                    break;
                case FormTypeEnum.ViewForm: //查看页面
                    {
                        if (!isEnabledFlow || (BpmOperate.GetRecordFlowStatus(module.Id, recordId) == WorkFlowStatusEnum.NoStatus && BpmOperate.GetRecordStatus(module.Id, recordId.Value) == WorkFlowStatusEnum.NoStatus))
                        {
                            bool canEdit = PermissionOperate.HasButtonPermission(currUser, module.Id, "编辑");
                            if (module.IsAllowEdit && canEdit)
                            {
                                btns.Add(new FormButton()
                                {
                                    TagId = "btnToEdit",
                                    DisplayText = "编辑",
                                    IconType = ButtonIconType.Edit,
                                    ClickMethod = "ToEdit(this)",
                                    Icon = "eu-icon-edit",
                                    Sort = 1
                                });
                            }
                        }
                    }
                    break;
            }
            btns.Add(new FormButton()
            {
                TagId = "btnClose",
                DisplayText = "关闭",
                IconType = ButtonIconType.Close,
                ClickMethod = "CloseTab()",
                Icon = "eu-icon-close",
                Sort = 20
            });
            //调用自定义表单按钮方法
            List<FormButton> newBtns = CommonOperate.GetFormButtons(module.Id, formType, btns, isAdd, isDraft, currUser);
            if (newBtns != null && newBtns.Count > 0)
            {
                return newBtns.OrderBy(x => x.Sort).ToList();
            }
            return btns;
        }

        /// <summary>
        /// 获取表单工具标签按钮集合
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="formType">表单类型</param>
        /// <param name="isAdd">是否新增页面</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static List<FormToolTag> GetFormToolTags(Sys_Module module, FormTypeEnum formType, bool isAdd = false, UserInfo currUser = null)
        {
            List<FormToolTag> tags = new List<FormToolTag>();
            if (currUser == null) return tags;
            if (formType == FormTypeEnum.ViewForm || (formType == FormTypeEnum.EditForm && !isAdd))
            {
                //tags.Add(new FormToolTag()
                //{
                //    TagId = "btnPreRecord",
                //    Title = "上一记录",
                //    ClickMethod = "PreRecord(this)",
                //    Icon = "eu-p2-icon-resultset_previous"
                //});
                //tags.Add(new FormToolTag()
                //{
                //    TagId = "btnNextRecord",
                //    Title = "下一记录",
                //    ClickMethod = "NextRecord(this)",
                //    Icon = "eu-p2-icon-resultset_next"
                //});
                if (module.IsEnabledPrint && PermissionOperate.HasButtonPermission(currUser, module.Id, "打印"))
                {
                    tags.Add(new FormToolTag()
                    {
                        TagId = "btnPrint",
                        Text = "打印",
                        ClickMethod = "PrintModel(this)",
                        Icon = "eu-icon-print"
                    });
                }
            }
            //调用自定义方法
            List<FormToolTag> newTags = CommonOperate.GetFormToolTags(module.Id, formType, tags, isAdd, currUser);
            if (newTags != null && newTags.Count > 0)
            {
                return newTags;
            }
            return tags;
        }

        #endregion

        #region 编码规则

        /// <summary>
        /// 获取编码规则字段
        /// </summary>
        /// <param name="module">模块</param>
        /// <returns></returns>
        public static string GetBillCodeFieldName(Sys_Module module)
        {
            if (module == null || !module.IsEnableCodeRule)
                return string.Empty;
            string errMsg = string.Empty;
            Sys_BillCodeRule billCodeRule = CommonOperate.GetEntity<Sys_BillCodeRule>(x => x.Sys_ModuleId == module.Id && !x.IsDeleted && !x.IsDraft, null, out errMsg);
            if (billCodeRule == null || string.IsNullOrWhiteSpace(billCodeRule.FieldName))
                return string.Empty;
            return billCodeRule.FieldName;
        }

        /// <summary>
        /// 获取编码规则字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static string GetBillCodeFieldName(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            return GetBillCodeFieldName(module);
        }

        /// <summary>
        /// 获取单据编码
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="currCode">当前编码</param>
        /// <returns></returns>
        public static string GetBillCode(Sys_Module module, string currCode = null)
        {
            if (module == null || !module.IsEnableCodeRule)
                return string.Empty;
            string errMsg = string.Empty;
            Sys_BillCodeRule billCodeRule = CommonOperate.GetEntity<Sys_BillCodeRule>(x => x.Sys_ModuleId == module.Id && !x.IsDeleted && !x.IsDraft, null, out errMsg);
            if (billCodeRule == null || string.IsNullOrWhiteSpace(billCodeRule.FieldName))
                return string.Empty;
            string nextCode = string.Empty;
            if (!string.IsNullOrEmpty(currCode)) //根据当前编码取下一编码
            {
                nextCode = GetNextBillCodeByCurrCode(billCodeRule, currCode);
                if (!string.IsNullOrEmpty(nextCode))
                    return nextCode;
            }
            nextCode = GetNextBillCode(billCodeRule); //获取下一编码
            if (!string.IsNullOrEmpty(nextCode))
                return nextCode;
            //调用存储过程取下一编码
            //输出参数
            Dictionary<string, object> outParams = new Dictionary<string, object>();
            outParams.Add("returnChar", "");
            //输出参数
            object inParams = new { ModuleId = module.Id };
            //执行存储过程
            CommonOperate.RunProcedureNoQuery(out errMsg, ref outParams, "GetBillCode", inParams);
            return outParams["returnChar"].ObjToStr();
        }

        /// <summary>
        /// 获取单据编码
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static string GetBillCode(Guid moduleId)
        {
            Sys_Module module = GetModuleById(moduleId);
            return GetBillCode(module);
        }

        /// <summary>
        /// 获取下一编码
        /// </summary>
        /// <param name="billCodeRule">编码规则</param>
        /// <returns></returns>
        private static string GetNextBillCode(Sys_BillCodeRule billCodeRule)
        {
            if (billCodeRule == null)
                return string.Empty;
            if (string.IsNullOrEmpty(billCodeRule.NextCode))
            {
                string rulePrex = string.Empty; //规则前缀
                rulePrex += billCodeRule.Prefix.ObjToStr();
                if (billCodeRule.IsEnableDate)
                {
                    string dateStr = DateTime.Now.ToString(EnumHelper.GetDescription(billCodeRule.DateFormatOfEnum));
                    rulePrex += dateStr; //日期前缀
                }
                string serierNum = string.Empty;
                for (int i = 0; i < billCodeRule.SNLength - 1; i++)
                {
                    serierNum += billCodeRule.PlaceHolder;
                }
                serierNum += billCodeRule.SerialNumber;
                return rulePrex + serierNum;
            }
            else
            {
                string nextCode = billCodeRule.NextCode;
                if (!string.IsNullOrEmpty(nextCode) && billCodeRule.IsEnableDate) //下一编码不为空并且启用了日期
                {
                    string rulePrex = string.Empty; //规则前缀
                    rulePrex += billCodeRule.Prefix.ObjToStr();
                    string dateStr = DateTime.Now.ToString(EnumHelper.GetDescription(billCodeRule.DateFormatOfEnum));
                    string currDateStr = nextCode.Substring(rulePrex.Length, dateStr.Length); //当前编码中日期字符串
                    rulePrex += dateStr; //日期前缀
                    //当前编码中日期与当前日期不一致时从1开始编号
                    int currCodeValue = currDateStr != dateStr ? 1 : nextCode.Substring(rulePrex.Length).ObjToInt();
                    string serierNum = string.Empty;
                    for (int i = 0; i < billCodeRule.SNLength - currCodeValue.ToString().Length; i++)
                    {
                        serierNum += billCodeRule.PlaceHolder;
                    }
                    nextCode = rulePrex + serierNum + currCodeValue;
                }
                return nextCode;
            }
        }

        /// <summary>
        /// 根据当前编码获取下一编码
        /// </summary>
        /// <param name="billCodeRule">编码规则对象</param>
        /// <param name="currCode">当前编码</param>
        /// <returns></returns>
        private static string GetNextBillCodeByCurrCode(Sys_BillCodeRule billCodeRule, string currCode)
        {
            if (billCodeRule == null || string.IsNullOrEmpty(currCode))
                return string.Empty;
            string rulePrex = string.Empty; //规则前缀
            rulePrex += billCodeRule.Prefix.ObjToStr();
            if (billCodeRule.IsEnableDate)
            {
                string dateStr = DateTime.Now.ToString(EnumHelper.GetDescription(billCodeRule.DateFormatOfEnum));
                rulePrex += dateStr; //日期前缀
            }
            int currCodeValue = rulePrex.Length > 0 ? currCode.Replace(rulePrex, string.Empty).ObjToInt() + 1 : currCode.ObjToInt() + 1;
            string serierNum = string.Empty;
            for (int i = 0; i < billCodeRule.SNLength - currCodeValue.ToString().Length; i++)
            {
                serierNum += billCodeRule.PlaceHolder;
            }
            string nextCode = rulePrex + serierNum + currCodeValue;
            return nextCode;
        }

        /// <summary>
        /// 更新当前模块编码
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="currCode">当前编码</param>
        public static void UpdateBillCode(Guid moduleId, string currCode)
        {
            if (string.IsNullOrEmpty(currCode))
                return;
            Sys_Module module = GetModuleById(moduleId);
            if (module == null || !module.IsEnableCodeRule)
                return;
            string errMsg = string.Empty;
            Sys_BillCodeRule billCodeRule = CommonOperate.GetEntity<Sys_BillCodeRule>(x => x.Sys_ModuleId == module.Id && !x.IsDeleted && !x.IsDraft, null, out errMsg);
            string nextCode = GetNextBillCodeByCurrCode(billCodeRule, currCode);
            if (!string.IsNullOrEmpty(nextCode))
            {
                billCodeRule.CurrCode = currCode;
                billCodeRule.NextCode = nextCode;
                CommonOperate.OperateRecord<Sys_BillCodeRule>(billCodeRule, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "CurrCode", "NextCode" }, false);
            }
        }

        #endregion

        #region 其他

        /// <summary>
        /// 取表单的过滤条件，过滤条件为SQL时whereSql为SQL语句条件，否则为Json或条件名称时返回条件表达式
        /// </summary>
        /// <param name="whereSql">SQL条件语句</param>
        /// <param name="moduleName">原始模块名称</param>
        /// <param name="fieldName">原始字段名称</param>
        /// <param name="foreignModuleId">外键模块Id</param>
        /// <param name="relyFieldsValue">依赖字段值</param>
        /// <returns></returns>
        public static object GetFormFieldFilterCondition(ref string whereSql, string moduleName, string fieldName, Guid foreignModuleId, string relyFieldsValue = null)
        {
            if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(fieldName))
                return null;
            Guid moduleId = GetModuleIdByName(moduleName);
            Sys_FormField formField = GetNfDefaultFormSingleField(moduleId, fieldName);
            if (formField == null || string.IsNullOrEmpty(formField.FilterCondition))
                return null;
            string condition = formField.FilterCondition;
            if (string.IsNullOrEmpty(condition)) return null;
            #region 条件表达式转化
            if ((condition.StartsWith("[") && condition.EndsWith("]")) ||
                (condition.StartsWith("{") && condition.EndsWith("}"))) //JSON条件，常量条件
            {
                if (condition.StartsWith("{")) //单个ConditionItem或Dictionary<string, string>对象或单个匿名对象
                {
                    //先反序列化为ConditionItem，如果失败则尝试反序列化为Dictionary<string, string>
                    try
                    {
                        ConditionItem conditionItem = JsonHelper.Deserialize<ConditionItem>(condition);
                        object exp = CommonOperate.GetQueryCondition(foreignModuleId, new List<ConditionItem>() { conditionItem });
                        return exp;
                    }
                    catch
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        try
                        {
                            dic = JsonHelper.Deserialize<Dictionary<string, string>>(condition);
                        }
                        catch
                        {
                            condition = condition.Replace("{", "").Replace("}", "");
                            var arr = condition.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (arr.Length > 0)
                            {
                                foreach (var item in arr)
                                {
                                    var val = item.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (val.Count() != 2) continue;
                                    if (!dic.ContainsKey(val[0]))
                                    {
                                        dic.Add(val[0], val[1]);
                                    }
                                }
                            }
                        }
                        if (dic != null && dic.Count > 0)
                        {
                            object exp = CommonOperate.GetQueryCondition(foreignModuleId, dic);
                            return exp;
                        }
                    }
                }
                else //List<ConditionItem>对象
                {
                    try
                    {
                        List<ConditionItem> conditionItemList = JsonHelper.Deserialize<List<ConditionItem>>(condition);
                        object exp = CommonOperate.GetQueryCondition(foreignModuleId, conditionItemList);
                        return exp;
                    }
                    catch
                    { }
                }
            }
            else if (condition.StartsWith("(") && condition.EndsWith(")")) //SQL条件语句
            {
                if (!string.IsNullOrWhiteSpace(whereSql)) whereSql += " AND ";
                whereSql += condition;
            }
            #endregion
            return null;
        }

        #endregion

        #endregion

        #region 字典

        /// <summary>
        /// 获取字段绑定的字典分类
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static string GetBindDictonaryClass(Guid moduleId, string fieldName)
        {
            string errMsg = string.Empty;
            Expression<Func<Sys_BindDictionary, bool>> expression = x => x.Sys_ModuleId == moduleId && x.FieldName == fieldName && x.IsValid == true && !x.IsDeleted && !x.IsDraft;
            Sys_BindDictionary bindDictionary = CommonOperate.GetEntity<Sys_BindDictionary>(expression, string.Empty, out errMsg);
            if (bindDictionary != null)
            {
                return bindDictionary.ClassName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取字典数据
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static List<Sys_Dictionary> GetDictionaryData(Guid moduleId, string fieldName)
        {
            string className = GetBindDictonaryClass(moduleId, fieldName);
            return GetDictionaryData(className);
        }

        /// <summary>
        /// 获取字典集合
        /// </summary>
        /// <param name="className">字典分类</param>
        /// <returns></returns>
        public static List<Sys_Dictionary> GetDictionaryData(string className)
        {
            if (string.IsNullOrEmpty(className)) return new List<Sys_Dictionary>();
            string errMsg = string.Empty;
            Expression<Func<Sys_Dictionary, bool>> expression = x => x.ClassName == className && x.IsValid == true && !x.IsDeleted && !x.IsDraft;
            List<Sys_Dictionary> list = CommonOperate.GetEntities<Sys_Dictionary>(out errMsg, expression, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (list == null) list = new List<Sys_Dictionary>();
            return list;
        }

        /// <summary>
        /// 获取字段的子字典
        /// </summary>
        /// <param name="moduleId">模块</param>
        /// <param name="fieldName">字段</param>
        /// <param name="dicValue">字段字典值</param>
        /// <returns></returns>
        public static List<Sys_Dictionary> GetChildDictionary(Guid moduleId, string fieldName, string dicValue)
        {
            string className = GetBindDictonaryClass(moduleId, fieldName);
            List<Sys_Dictionary> dic = GetDictionaryData(className);
            if (dic.Count == 0) return new List<Sys_Dictionary>();
            Sys_Dictionary parentDic = dic.Where(x => x.Value == dicValue).FirstOrDefault();
            if (parentDic == null) return new List<Sys_Dictionary>();
            string errMsg = string.Empty;
            List<Sys_Dictionary> childDics = CommonOperate.GetEntities<Sys_Dictionary>(out errMsg, x => x.ParentId == parentDic.Id && x.IsValid == true && !x.IsDeleted && !x.IsDraft, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            return childDics;
        }

        #endregion

        #region 图标

        /// <summary>
        /// 根据图标类型过滤分页图标
        /// </summary>
        /// <param name="iconType">图标类型</param>
        /// <param name="total">总记录数</param>
        /// <param name="pageInfo">分页信息</param>
        /// <returns></returns>
        public static List<Sys_IconManage> GetPageIcons(out long total, IconTypeEnum? iconType, PageInfo pageInfo = null)
        {
            total = 0;
            PageInfo tempPageInfo = pageInfo == null ? PageInfo.GetDefaultPageInfo() : pageInfo;
            int type = (int)iconType;
            string errMsg = string.Empty;
            Expression<Func<Sys_IconManage, bool>> expression = null;
            if (iconType.HasValue)
            {
                expression = x => x.IconType == type && !x.IsDeleted;
                if (iconType == IconTypeEnum.Piex16)
                    expression = x => (x.IconType == type || x.IconType == null) && !x.IsDeleted;
            }
            else
            {
                expression = x => x.IsDeleted == false;
            }
            List<Sys_IconManage> list = CommonOperate.GetPageEntities<Sys_IconManage>(out errMsg, tempPageInfo, true, expression);
            total = tempPageInfo.totalCount;
            if (list == null) list = new List<Sys_IconManage>();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="total">总记录数</param>
        /// <param name="pageSize">每页图标数</param>
        /// <param name="iconType">图标类型</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="w">图标面板宽，默认832</param>
        /// <param name="h">图标面板高，默认384</param>
        /// <param name="iconSize">图标所占大小，默认64</param>
        /// <returns></returns>
        public static string GetPageIconsHtml(out long total, out int pageSize, IconTypeEnum? iconType, int pageIndex = 1, int w = 832, int h = 384, int iconSize = 64)
        {
            total = 0;
            int size = iconSize < 32 ? 32 : iconSize; //图标大小最低点32个像素
            int width = w < size * 4 ? size * 4 : w; //最少4列
            int height = h < size * 2 ? size * 2 : h; //最少2行
            int col = (int)(width / size); //列数
            int row = (int)(height / size); //行数
            int page_Size = row * col; //每页多少个图标
            pageSize = page_Size;
            int page_index = pageIndex < 1 ? 1 : pageIndex;
            PageInfo pageInfo = new PageInfo(page_index, page_Size, null, null);
            List<Sys_IconManage> list = SystemOperate.GetPageIcons(out total, iconType, pageInfo);
            if (list.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            //图标内容
            sb.Append("<table>");
            for (int i = 0; i < list.Count; i++)
            {
                int r = i / col; //当前所在行
                int c = i % col; //当前所在列
                if (i == 0)
                {
                    sb.Append("<tr>");
                }
                else if (c == 0)
                {
                    sb.Append("</tr><tr>");
                }
                string imgUrl = string.Empty;
                if (list[i].IconClass == (int)IconClassTypeEnum.CustomerIcon) //自定义图标
                {
                    imgUrl = string.Format("/Css/{0}", list[i].IconAddr);
                }
                else if (list[i].IconClass == (int)IconClassTypeEnum.SystemIcon) //自定义图标
                {
                    imgUrl = string.Format("/Scripts/jquery-easyui/themes/{0}", list[i].IconAddr);
                }
                else
                {
                    imgUrl = list[i].IconAddr;
                }
                sb.AppendFormat("<td style=\"text-align:center;width:64px;height:64px;cursor:pointer;\"><img src=\"{0}\" styleName=\"{1}\" /></td>", imgUrl, list[i].StyleClassName);
            }
            sb.Append("</tr></table>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取图标url
        /// </summary>
        /// <param name="iconClassName">图标类名</param>
        /// <returns></returns>
        public static string GetIconUrl(string iconClassName)
        {
            string errMsg = string.Empty;
            Sys_IconManage icon = CommonOperate.GetEntity<Sys_IconManage>(x => x.StyleClassName == iconClassName && !x.IsDeleted, null, out errMsg);
            if (icon == null) return string.Empty;
            if (icon.IconClass == (int)IconClassTypeEnum.CustomerIcon) //自定义图标
                return string.Format("/Css/{0}", icon.IconAddr);
            else if (icon.IconClass == (int)IconClassTypeEnum.SystemIcon) //系统图标
                return string.Format("/Scripts/jquery-easyui/themes/{0}", icon.IconAddr);
            else //用户上传
                return icon.IconAddr;
        }

        #endregion

        #region 桌面

        /// <summary>
        /// 获取桌面项
        /// </summary>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static List<Desktop_Item> GetDesktopItems(UserInfo currUser)
        {
            string errMsg = string.Empty;
            List<Desktop_Item> desktopItems = CommonOperate.GetEntities<Desktop_Item>(out errMsg, x => !x.IsDeleted && !x.IsDraft, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (desktopItems == null) desktopItems = new List<Desktop_Item>();
            foreach (Desktop_Item deskItem in desktopItems)
            {
                deskItem.Desktop_ItemTabs = CommonOperate.GetEntities<Desktop_ItemTab>(out errMsg, x => x.Desktop_ItemId == deskItem.Id && !x.IsDeleted && !x.IsDraft, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            }
            return desktopItems;
        }

        /// <summary>
        /// 获取桌面配置字段
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public static List<Desktop_GridField> GetDesktopGridFields(Guid moduleId)
        {
            string errMsg = string.Empty;
            List<Desktop_GridField> list = CommonOperate.GetEntities<Desktop_GridField>(out errMsg, x => x.Sys_ModuleId == moduleId && !x.IsDeleted, null, false, new List<string>() { "Sort" }, new List<bool>() { false });
            if (list == null) list = new List<Desktop_GridField>();
            List<Desktop_GridField> deskGridFields = new List<Desktop_GridField>();
            //循环处理字段
            foreach (Desktop_GridField field in list)
            {
                if (!field.Sys_ModuleId.HasValue) continue;
                //对于外键字段需要添加name字段
                Sys_Field sysField = SystemOperate.GetFieldInfo(field.Sys_ModuleId.Value, field.FieidName);
                bool isForeign = !string.IsNullOrEmpty(sysField.ForeignModuleName); //IsForeignField(sysField);
                if (isForeign) //需要添加
                {
                    #region 复制字段
                    Desktop_GridField tempField = new Desktop_GridField();
                    tempField.Id = field.Id;
                    tempField.Sys_ModuleId = field.Sys_ModuleId;
                    tempField.FieidName = field.FieidName;
                    tempField.Sort = field.Sort;
                    tempField.Sys_ModuleName = field.Sys_ModuleName;
                    tempField.CreateDate = field.CreateDate;
                    tempField.CreateUserId = field.CreateUserId;
                    tempField.CreateUserName = field.CreateUserName;
                    tempField.ModifyDate = field.ModifyDate;
                    tempField.ModifyUserId = field.ModifyUserId;
                    tempField.ModifyUserName = field.ModifyUserName;
                    tempField.Width = 0;
                    #endregion
                    deskGridFields.Add(tempField);
                    //增加一个name字段
                    #region 增加外键Name字段
                    Desktop_GridField tempNameField = new Desktop_GridField();
                    tempNameField.Id = field.Id;
                    tempNameField.Sys_ModuleId = field.Sys_ModuleId;
                    tempNameField.Sort = field.Sort;
                    tempNameField.Sys_ModuleName = field.Sys_ModuleName;
                    tempNameField.Width = field.Width;
                    tempNameField.CreateDate = field.CreateDate;
                    tempNameField.CreateUserId = field.CreateUserId;
                    tempNameField.CreateUserName = field.CreateUserName;
                    tempNameField.ModifyDate = field.ModifyDate;
                    tempNameField.ModifyUserId = field.ModifyUserId;
                    tempNameField.ModifyUserName = field.ModifyUserName;
                    //字段名称为原字段名称去掉最后的Id加上Name
                    tempNameField.FieidName = isForeign ? field.FieidName.Substring(0, field.FieidName.Length - 2) + "Name" : field.FieidName + "Name";
                    #endregion
                    deskGridFields.Add(tempNameField);
                }
                else
                {
                    deskGridFields.Add(field);
                }
            }
            return deskGridFields;
        }

        #endregion

        #region 附件

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="tempAttachments">附件集合</param>
        public static string DeleteAttachment(List<Sys_Attachment> tempAttachments)
        {
            if (tempAttachments == null || tempAttachments.Count == 0)
                return string.Empty;
            string errMsg = string.Empty;
            List<Guid> delIds = tempAttachments.Select(x => x.Id).ToList(); //要删除的附件Id
            bool tempRs = CommonOperate.DeleteRecordsByExpression<Sys_Attachment>(x => delIds.Contains(x.Id), out errMsg, false);
            if (tempRs)
            {
                foreach (Sys_Attachment tempObj in tempAttachments)
                {
                    try
                    {
                        string tempFile = string.IsNullOrEmpty(tempObj.FileUrl) ? string.Empty : string.Format("{0}{1}", Globals.GetWebDir(), tempObj.FileUrl).Replace("/", "\\");
                        string tempPdfFile = string.IsNullOrEmpty(tempObj.PdfUrl) ? string.Empty : string.Format("{0}{1}", Globals.GetWebDir(), tempObj.PdfUrl).Replace("/", "\\");
                        string tempSwfFile = string.IsNullOrEmpty(tempObj.SwfUrl) ? string.Empty : string.Format("{0}{1}", Globals.GetWebDir(), tempObj.SwfUrl).Replace("/", "\\");
                        if (!string.IsNullOrEmpty(tempFile) && System.IO.File.Exists(tempFile))
                        {
                            System.IO.File.Delete(tempFile); //删除原文件
                        }
                        if (!string.IsNullOrEmpty(tempPdfFile) && System.IO.File.Exists(tempPdfFile))
                        {
                            System.IO.File.Delete(tempPdfFile); //删除ＰＤＦ文件
                        }
                        if (!string.IsNullOrEmpty(tempSwfFile) && System.IO.File.Exists(tempSwfFile))
                        {
                            System.IO.File.Delete(tempSwfFile);　//删除ＳＷＦ文件
                        }
                    }
                    catch (Exception ex)
                    {
                        errMsg = ex.Message;
                    }
                }
            }
            return errMsg;
        }

        #endregion

        #region UI处理

        #region 下拉框

        /// <summary>
        /// 获取枚举集合对象
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static object GetEnumTypeList(Type enumType)
        {
            List<object> list = new List<object>();
            Dictionary<string, string> dic = EnumHelper.GetEnumDescValue(enumType);
            foreach (string key in dic.Keys)
            {
                list.Add(new
                {
                    Id = dic[key],
                    Name = key
                });
            }
            return list;
        }

        /// <summary>
        /// 绑定枚举字段下拉框数据
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static object EnumFieldDataJson(Guid moduleId, string fieldName)
        {
            List<object> list = new List<object>();
            Dictionary<string, string> dic = CommonOperate.GetFieldEnumTypeList(moduleId, fieldName);
            if (dic == null) return null;
            foreach (string key in dic.Keys)
            {
                list.Add(new
                {
                    Id = dic[key],
                    Name = key
                });
            }
            list.Insert(0, new { Id = string.Empty, Name = "请选择" });
            return list;
        }

        /// <summary>
        /// 获取枚举字段显示值
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段值</param>
        /// <returns></returns>
        public static string GetEnumFieldDisplayText(Guid moduleId, string fieldName, string fieldValue)
        {
            Dictionary<string, string> dic = CommonOperate.GetFieldEnumTypeList(moduleId, fieldName);
            if (dic != null && dic.Count > 0)
            {
                return dic.Where(x => x.Value == fieldValue).Select(x => x.Key).FirstOrDefault();
            }
            return string.Empty;
        }

        /// <summary>
        /// 绑定外键字段下拉框数据
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public static object ForeignFieldComboDataJson(Guid moduleId, string fieldName, UserInfo currUser)
        {
            Type modelType = GetModelType(moduleId);
            //验证是否为外键字段
            bool isForeignKey = SystemOperate.IsForeignField(moduleId, fieldName);
            if (!isForeignKey) return string.Empty;
            //获取外键模块
            Sys_Module foreignModule = SystemOperate.GetForeignModule(moduleId, fieldName);
            if (foreignModule == null) return string.Empty;
            Type foreignModelType = GetModelType(foreignModule.Id);
            string errMsg = string.Empty; //异常信息
            string whereSql = string.Empty; //where语句
            string initModule = GetModuleNameById(moduleId); //原始模块
            //组装条件表达式
            object conditionExp = CommonOperate.GetGridFilterCondition(ref whereSql, foreignModule.Id, null, DataGridType.DialogGrid, null, initModule, fieldName, null, null, currUser);
            //联动条件表达式
            object formFieldConditionExp = SystemOperate.GetFormFieldFilterCondition(ref whereSql, initModule, fieldName, foreignModule.Id);
            //合并条件
            conditionExp = CommonOperate.ConditionMerge(foreignModule.Id, conditionExp, formFieldConditionExp);
            //取外键模块数据
            object data = CommonOperate.GetEntities(out errMsg, foreignModule.Id, conditionExp, whereSql, true, null, null, null, null, false, null, null, currUser);
            List<object> list = new List<object>();
            if (data != null)
            {
                foreach (object obj in (data as IEnumerable))
                {
                    list.Add(obj);
                }
            }
            Sys_FormField formField = SystemOperate.GetNfDefaultFormSingleField(moduleId, fieldName);
            if (formField != null)
            {
                string valueField = string.IsNullOrEmpty(formField.ValueField) ? "Id" : formField.ValueField;
                string textField = string.IsNullOrEmpty(formField.TextField) ? "Name" : formField.TextField;
                PropertyInfo pId = foreignModelType.GetProperty(valueField);
                PropertyInfo pName = foreignModelType.GetProperty(textField);
                if (pId != null && pName != null)
                {
                    object tempObj = Activator.CreateInstance(foreignModelType);
                    pId.SetValue2(tempObj, string.Empty.ObjToGuid(), null);
                    pName.SetValue2(tempObj, "请选择", null);
                    list.Insert(0, tempObj);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取字典分类数据
        /// </summary>
        /// <param name="className">字典分类名称</param>
        /// <returns></returns>
        public static object DictionaryDataJson(string className)
        {
            List<Sys_Dictionary> list = SystemOperate.GetDictionaryData(className);
            List<object> data = new List<object>();
            if (list != null && list.Count > 0)
            {
                foreach (Sys_Dictionary dictionay in list)
                {
                    data.Add(new
                    {
                        Id = dictionay.Value,
                        Name = dictionay.Name
                    });
                }
            }
            data.Insert(0, new { Id = string.Empty, Name = "请选择" });
            return data;
        }

        /// <summary>
        /// 绑定下拉框字典数据
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static object DictionaryDataJson(Guid moduleId, string fieldName)
        {
            List<Sys_Dictionary> list = SystemOperate.GetDictionaryData(moduleId, fieldName);
            List<object> data = new List<object>();
            if (list != null && list.Count > 0)
            {
                foreach (Sys_Dictionary dictionay in list)
                {
                    data.Add(new
                    {
                        Id = dictionay.Value,
                        Name = dictionay.Name
                    });
                }
            }
            data.Insert(0, new { Id = string.Empty, Name = "请选择" });
            return data;
        }

        /// <summary>
        /// 获取子字典数据JSON
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="dicValue">字段字典值</param>
        /// <returns></returns>
        public static object GetChildDictionaryJson(Guid moduleId, string fieldName, string dicValue)
        {
            List<Sys_Dictionary> list = SystemOperate.GetChildDictionary(moduleId, fieldName, dicValue);
            List<object> data = new List<object>();
            if (list != null && list.Count > 0)
            {
                foreach (Sys_Dictionary dictionay in list)
                {
                    data.Add(new
                    {
                        Id = dictionay.Value,
                        Name = dictionay.Name
                    });
                }
            }
            data.Insert(0, new { Id = string.Empty, Name = "请选择" });
            return data;
        }

        /// <summary>
        /// 获取字段字典显示值
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段值</param>
        /// <returns></returns>
        public static string GetDictionaryDisplayText(Guid moduleId, string fieldName, string fieldValue)
        {
            List<Sys_Dictionary> list = SystemOperate.GetDictionaryData(moduleId, fieldName);
            if (list != null && list.Count > 0)
            {
                return list.Where(x => x.Value == fieldValue).Select(x => x.Name).FirstOrDefault();
            }
            return string.Empty;
        }

        #endregion

        #region 页面缓存

        /// <summary>
        /// 获取页面缓存html
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="cachekey">缓存KEY</param>
        /// <param name="cachePageType">缓存页面类型</param>
        /// <returns></returns>
        public static string GetPageCacheHtml(Guid moduleId, string cachekey, CachePageTypeEnum cachePageType)
        {
            string errMsg = string.Empty;
            int pageType = (int)cachePageType;
            Sys_PageCache pageCache = CommonOperate.GetEntity<Sys_PageCache>(x => x.Sys_ModuleId == moduleId && x.CacheKey == cachekey && x.CachePageType == pageType && !x.IsDeleted, null, out errMsg);
            if (pageCache != null)
                return pageCache.PageHtml;
            return string.Empty;
        }

        /// <summary>
        /// 设置页面缓存html
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="cachekey">缓存KEY</param>
        /// <param name="cachePageType">缓存页面类型</param>
        /// <param name="html">页面html</param>
        public static void SetPageCacheHtml(Guid moduleId, string cachekey, CachePageTypeEnum cachePageType, string html)
        {
            try
            {
                string errMsg = string.Empty;
                int pageType = (int)cachePageType;
                Sys_PageCache pageCache = CommonOperate.GetEntity<Sys_PageCache>(x => x.Sys_ModuleId == moduleId && x.CacheKey == cachekey && x.CachePageType == pageType && !x.IsDeleted, null, out errMsg);
                List<string> updateFields = new List<string>() { "PageHtml", "ModifyDate", "ModifyUserId", "ModifyUserName" };
                ModelRecordOperateType opeateType = ModelRecordOperateType.Edit;
                if (pageCache == null)
                {
                    opeateType = ModelRecordOperateType.Add;
                    pageCache = new Sys_PageCache()
                    {
                        Sys_ModuleId = moduleId,
                        CacheKey = cachekey,
                        CachePageTypeOfEnum = cachePageType,
                        CreateDate = DateTime.Now
                    };
                }
                pageCache.PageHtml = html;
                pageCache.ModifyDate = DateTime.Now;
                CommonOperate.OperateRecord<Sys_PageCache>(pageCache, opeateType, out errMsg, updateFields, false);
            }
            catch { }
        }

        #endregion

        #endregion

        #region 其他

        #region 数据库操作

        /// <summary>
        /// 判断数据库是否存在
        /// </summary>
        /// <param name="dbLinkArgs">数据库连接参数</param>
        /// <returns></returns>
        public static bool DbIsExists(DbLinkArgs dbLinkArgs)
        {
            if (dbLinkArgs == null) return false;
            string errMsg = string.Empty;
            string sql = string.Empty;
            if (dbLinkArgs.DbType == DatabaseType.MsSqlServer)
            {
                sql = string.Format("SELECT 1 FROM master.sys.sysdatabases WHERE NAME=N'{0}'", dbLinkArgs.DbName);
                string connStr = string.Format("Data Source={0};Initial Catalog=master;User ID={1};Password={2};Pooling=true;MAX Pool Size=512;Min Pool Size=50;Connection Lifetime=30", dbLinkArgs.DataSource, dbLinkArgs.UserId, dbLinkArgs.Pwd);
                object obj = CommonOperate.ExecuteScale(out errMsg, sql, null, connStr, dbLinkArgs.DbType);
                return obj != null;
            }
            return false;
        }

        /// <summary>
        /// 注册其他链接数据服务器
        /// </summary>
        public static void RegCrossDbServer()
        {
            List<DbLinkArgs> list = ModelConfigHelper.GetCrossServerDbLinkArgs();
            string errMsg = string.Empty;
            foreach (DbLinkArgs linkArgs in list)
            {
                string sql = string.Empty;
                if (linkArgs.DbType == DatabaseType.MsSqlServer)
                    sql = string.Format("exec sp_dropserver '{0}','droplogins';exec sp_addlinkedserver  '{0}','','SQLOLEDB','{0}';exec sp_addlinkedsrvlogin '{0}','false',null,'{1}','{2}'", linkArgs.DataSource, linkArgs.UserId, linkArgs.Pwd);
                CommonOperate.ExecuteNonQuery(out errMsg, sql);
            }
        }

        /// <summary>
        /// 向数据库中注册存储过程
        /// </summary>
        public static void RegStoredProcedure()
        {
            //本地数据库连接对象
            DbLinkArgs dbLinkArgs = ModelConfigHelper.GetLocalDbLinkArgs();
            //获取跨服务的数据库连接对象
            List<DbLinkArgs> list = ModelConfigHelper.GetCrossServerDbLinkArgs(false);
            list.Add(dbLinkArgs);
            string errMsg = string.Empty;
            //开始注册存储过程
            foreach (DbLinkArgs linkArgs in list)
            {
                RegStoredProcedure(linkArgs);
            }
        }

        /// <summary>
        /// 注册存储过程
        /// </summary>
        /// <param name="dbLinkArgs">数据库链接对象</param>
        private static void RegStoredProcedure(DbLinkArgs dbLinkArgs)
        {
            if (dbLinkArgs == null) return;
            StringBuilder sb = new StringBuilder(); //要执行的存储过程
            string errMsg = string.Empty;
            if (dbLinkArgs.DbType == DatabaseType.MsSqlServer)
            {
                #region 创建数据库
                if (!string.IsNullOrWhiteSpace(dbLinkArgs.DbName))
                {
                    string appDataPath = string.Format("{0}App_Data", Globals.GetWebDir());
                    if (!Directory.Exists(appDataPath))
                    {
                        Directory.CreateDirectory(appDataPath);
                    }
                    StringBuilder createDbSb = new StringBuilder();
                    createDbSb.AppendLine("IF NOT EXISTS(SELECT 1 FROM master.sys.sysdatabases WHERE NAME=N'" + dbLinkArgs.DbName + "') --如果数据库不存在");
                    createDbSb.AppendLine("BEGIN");
                    createDbSb.AppendLine(" CREATE DATABASE [" + dbLinkArgs.DbName + "]");
                    createDbSb.AppendLine(" ON");
                    createDbSb.AppendLine(" PRIMARY  --创建主数据库文件");
                    createDbSb.AppendLine(" (");
                    createDbSb.AppendLine(" 	NAME='" + dbLinkArgs.DbName + "',");
                    createDbSb.AppendLine(" 	FILENAME='" + appDataPath + "\\" + dbLinkArgs.DbName + ".mdf',");
                    createDbSb.AppendLine(" 	FileGrowth=1MB");
                    createDbSb.AppendLine(" )");
                    createDbSb.AppendLine(" LOG ON --创建日志文件");
                    createDbSb.AppendLine(" (");
                    createDbSb.AppendLine(" 	NAME='HkTempLog',");
                    createDbSb.AppendLine("	    FileName='" + appDataPath + "\\" + dbLinkArgs.DbName + ".ldf',");
                    createDbSb.AppendLine("	    FileGrowth=1MB");
                    createDbSb.AppendLine(" )");
                    createDbSb.AppendLine(" END");
                    string connStr = string.Format("Data Source={0};Initial Catalog=master;User ID={1};Password={2};Pooling=true;MAX Pool Size=512;Min Pool Size=50;Connection Lifetime=30", dbLinkArgs.DataSource, dbLinkArgs.UserId, dbLinkArgs.Pwd);
                    CommonOperate.ExecuteNonQuery(out errMsg, createDbSb.ToString(), null, connStr);
                }
                #endregion
                #region 分页存储过程
                object count = CommonOperate.ExecuteScale(out errMsg, "select COUNT(1) from sysobjects where id=object_id(N'GetPageTableByRowNumber') and OBJECTPROPERTY(id,N'IsProcedure')=1", null, dbLinkArgs.ConnString, dbLinkArgs.DbType);
                if (count.ObjToInt() <= 0)
                {
                    #region 创建
                    sb.AppendLine("CREATE PROCEDURE GetPageTableByRowNumber");
                    sb.AppendLine(" @Field nvarchar(1000),");
                    sb.AppendLine(" @TableName  nvarchar(2000),");
                    sb.AppendLine(" @condition  nvarchar(4000),--格式为：and (查询条件)  如'and (key=value and key1=value1)'");
                    sb.AppendLine(" @OrderField nvarchar(100),");
                    sb.AppendLine(" @OrderType int,");
                    sb.AppendLine(" @pageindx int,");
                    sb.AppendLine(" @PageSize  int,");
                    sb.AppendLine(" @RecordCount int output     --记录的总数");
                    sb.AppendLine("as");
                    sb.AppendLine("BEGIN");
                    sb.AppendLine(" --判断是否有排序字段");
                    sb.AppendLine("    if(@OrderField is null or ltrim(rtrim(@OrderField))='')");
                    sb.AppendLine("     begin");
                    sb.AppendLine("       RAISERROR('排序字段不能为空',16,1)");
                    sb.AppendLine("       return");
                    sb.AppendLine("     end");
                    sb.AppendLine("    --组装order语句 ");
                    sb.AppendLine(" declare @temp nvarchar(200)");
                    sb.AppendLine(" set @temp=' order by '+@OrderField");
                    sb.AppendLine(" if(@OrderType=1)");
                    sb.AppendLine("  begin ");
                    sb.AppendLine("     set @temp=@temp+' asc '");
                    sb.AppendLine("        end");
                    sb.AppendLine("     else");
                    sb.AppendLine("  begin ");
                    sb.AppendLine("     set @temp=@temp+' desc '");
                    sb.AppendLine("        end");
                    sb.AppendLine("    --组装查询条件，如果没有查询条件，直接跳过");
                    sb.AppendLine("   if(@condition is not null and ltrim(rtrim(@condition))!='')");
                    sb.AppendLine("   begin");
                    sb.AppendLine("     set @condition='where 1=1'+@condition");
                    sb.AppendLine("   end");
                    sb.AppendLine(" else");
                    sb.AppendLine("   begin");
                    sb.AppendLine("     set @condition=''");
                    sb.AppendLine("   end");
                    sb.AppendLine(" --求记录的总数");
                    sb.AppendLine(" declare @Countsql nvarchar(max)");
                    sb.AppendLine(" set @Countsql='select @a= count(1) from '+@TableName +' '+@condition");
                    sb.AppendLine(" exec sp_executesql  @Countsql,N'@a int output',@RecordCount output  ");
                    sb.AppendLine(" print @RecordCount");
                    sb.AppendLine(" declare @sql nvarchar(max)");
                    sb.AppendLine(" --分页");
                    sb.AppendLine(" if(@pageindx=1)");
                    sb.AppendLine("  begin");
                    sb.AppendLine("    set @sql=' select top '+cast(@pagesize as nvarchar )+'  '+ @Field+' from '+@TableName +' '+@condition+' '+@temp");
                    sb.AppendLine("  end");
                    sb.AppendLine(" else");
                    sb.AppendLine("  begin");
                    sb.AppendLine("    declare @startNumber   int");
                    sb.AppendLine("    set @startNumber =(@pageindx-1)*@pagesize");
                    sb.AppendLine("    set @sql='select ROW_NUMBER() over('+@temp+') as _number, '+@Field+' from '+@TableName+'  '+@condition ");
                    sb.AppendLine("    set @sql='SET ROWCOUNT '+Convert(varchar(4),@PageSize)+'; WITH SP_TABLE AS('+@sql   +')  SELECT '+@Field+' from SP_TABLE   where  _number>'+CAST(@startNumber as nvarchar) ");
                    sb.AppendLine("  end");
                    sb.AppendLine(" print @sql");
                    sb.AppendLine(" exec(@sql)");
                    sb.AppendLine("END");
                    #endregion
                    #region 执行
                    try
                    {
                        //创建存储过程
                        CommonOperate.ExecuteNonQuery(out errMsg, sb.ToString(), null, dbLinkArgs.ConnString, dbLinkArgs.DbType);
                    }
                    catch { }
                }
                    #endregion
                #endregion
                #region 获取单据编码
                count = CommonOperate.ExecuteScale(out errMsg, "select COUNT(1) from sysobjects where id=object_id(N'GetBillCode') and OBJECTPROPERTY(id,N'IsProcedure')=1", null, dbLinkArgs.ConnString, dbLinkArgs.DbType);
                if (count.ObjToInt() <= 0)
                {
                    #region 创建
                    sb = new StringBuilder();
                    sb.AppendLine("CREATE PROCEDURE GetBillCode ");
                    sb.AppendLine("	@ModuleId NVARCHAR(36),");
                    sb.AppendLine("	@returnChar  NVARCHAR(1000)  output  ");
                    sb.AppendLine("AS");
                    sb.AppendLine("BEGIN");
                    sb.AppendLine("	DECLARE @TableName nvarchar(255)");
                    sb.AppendLine("	DECLARE @BillNo nvarchar(255)");
                    sb.AppendLine("	DECLARE @TempBillNo nvarchar(255)");
                    sb.AppendLine("	DECLARE @IdentifyExistNo nvarchar(255)");
                    sb.AppendLine("	DECLARE @IdentifyExistSN nvarchar(255)");
                    sb.AppendLine("	DECLARE @IdentifyTempSN nvarchar(255)");
                    sb.AppendLine("	DECLARE @Date DATETIME");
                    sb.AppendLine("	DECLARE @strSql nvarchar(max)");
                    sb.AppendLine("	DECLARE @Prefix nvarchar(255)");
                    sb.AppendLine("	DECLARE @FieldName nvarchar(255)");
                    sb.AppendLine("	DECLARE @IsEnableDate BIT");
                    sb.AppendLine("	DECLARE @DateFormat INT");
                    sb.AppendLine("	DECLARE @SerialNumber INT");
                    sb.AppendLine("	DECLARE @PlaceHolder INT");
                    sb.AppendLine("	DECLARE @SNLength INT");
                    sb.AppendLine("	DECLARE @RuleFormat nvarchar(255)");
                    sb.AppendLine("	SET @Date=GETDATE()");
                    sb.AppendLine(string.Format(" SELECT @TableName=TableName FROM {0} WHERE Id=@ModuleId", ModelConfigHelper.GetModuleTableName(typeof(Sys_Module))));
                    sb.AppendLine("	SELECT @Prefix=Prefix,@IsEnableDate=IsEnableDate,@DateFormat=DateFormat,@SerialNumber=SerialNumber,@PlaceHolder=PlaceHolder,@SNLength=SNLength,@RuleFormat=RuleFormat,@FieldName=FieldName ");
                    sb.AppendLine(string.Format(" FROM {0} WHERE Sys_ModuleId= @ModuleId ", ModelConfigHelper.GetModuleTableName(typeof(Sys_BillCodeRule))));
                    sb.AppendLine("	SET @BillNo=N''");
                    sb.AppendLine("	SET @BillNo=@BillNo+@Prefix");
                    sb.AppendLine("	IF @IsEnableDate=1		");
                    sb.AppendLine("	BEGIN	");
                    sb.AppendLine("		IF @DateFormat=0 SET @BillNo=@BillNo+CONVERT(CHAR(2),@Date,12)");
                    sb.AppendLine("		ELSE IF @DateFormat=1 SET @BillNo=@BillNo+CONVERT(CHAR(4),@Date,112)");
                    sb.AppendLine("		ELSE IF @DateFormat=2 SET @BillNo=@BillNo+CONVERT(CHAR(4),@Date,12)");
                    sb.AppendLine("		ELSE IF @DateFormat=3 SET @BillNo=@BillNo+CONVERT(CHAR(6),@Date,112)");
                    sb.AppendLine("		ELSE IF @DateFormat=4 SET @BillNo=@BillNo+CONVERT(CHAR(6),@Date,12)");
                    sb.AppendLine("		ELSE IF @DateFormat=5 SET @BillNo=@BillNo+CONVERT(CHAR(8),@Date,112)");
                    sb.AppendLine("		ELSE IF @DateFormat=6 SET @BillNo=@BillNo+CONVERT(CHAR(2),@Date,1)+CONVERT(CHAR(2),@Date,12)");
                    sb.AppendLine("		ELSE IF @DateFormat=7 SET @BillNo=@BillNo+CONVERT(CHAR(2),@Date,1)+CONVERT(CHAR(4),@Date,112)");
                    sb.AppendLine("		ELSE IF @DateFormat=8 SET @BillNo=@BillNo+CONVERT(CHAR(2),@Date,12)+'-'+CONVERT(CHAR(2),@Date,1)");
                    sb.AppendLine("		ELSE IF @DateFormat=9 SET @BillNo=@BillNo+CONVERT(CHAR(7),@Date,120)");
                    sb.AppendLine("		ELSE IF @DateFormat=10 SET @BillNo=@BillNo+REPLACE(CONVERT(CHAR(8),@Date,11),'/','-')");
                    sb.AppendLine("		ELSE IF @DateFormat=11 SET @BillNo=@BillNo+CONVERT(CHAR(10),@Date,120)");
                    sb.AppendLine("		ELSE IF @DateFormat=12 SET @BillNo=@BillNo+CONVERT(CHAR(2),@Date,1)+'-'+CONVERT(CHAR(2),@Date,12)");
                    sb.AppendLine("		ELSE IF @DateFormat=13 SET @BillNo=@BillNo+CONVERT(CHAR(2),@Date,1)+'-'+CONVERT(CHAR(4),@Date,112)");
                    sb.AppendLine("		ELSE IF @DateFormat=14 SET @BillNo=@BillNo+REPLACE(CONVERT(CHAR(8),@Date,1),'/','-')");
                    sb.AppendLine("		ELSE IF @DateFormat=15 SET @BillNo=@BillNo+REPLACE(CONVERT(CHAR(10),@Date,101),'/','-')");
                    sb.AppendLine("		ELSE IF @DateFormat=16 SET @BillNo=@BillNo+CONVERT(CHAR(8),@Date,1)");
                    sb.AppendLine("		ELSE IF @DateFormat=17 SET @BillNo=@BillNo+CONVERT(CHAR(10),@Date,101)");
                    sb.AppendLine("		ELSE IF @DateFormat=18 SET @BillNo=@BillNo+CONVERT(CHAR(8),@Date,11)");
                    sb.AppendLine("		ELSE IF @DateFormat=19 SET @BillNo=@BillNo+CONVERT(CHAR(10),@Date,111)");
                    sb.AppendLine("	END");
                    sb.AppendLine("	SET @TempBillNo=@BillNo");
                    sb.AppendLine("	SET @TempBillNo=@TempBillNo+REPLACE(right(cast(power(10,9) as varchar)+@SerialNumber,@SNLength),'0',@PlaceHolder)");
                    sb.AppendLine("	SET @strSql=N''");
                    sb.AppendLine("	SET @strSql =@strSql+ ' SELECT DISTINCT  @IdentifyExistNo='+@FieldName+',@IdentifyExistSN=SUBSTRING('+@FieldName+',LEN('+@FieldName+')-'+CONVERT(NVARCHAR,(@SNLength-1))+',LEN('+@FieldName+'))'");
                    sb.AppendLine("	SET @strSql =@strSql+ ' FROM  '+@TableName");
                    sb.AppendLine("	SET @strSql =@strSql+ ' WHERE LEN('+@FieldName+')='+CONVERT(NVARCHAR,LEN(@TempBillNo))+' AND '+@FieldName+' LIKE '''+@BillNo+'%'''  ");
                    sb.AppendLine("	SET @strSql =@strSql+ ' AND SUBSTRING('+@FieldName+',LEN('+@FieldName+')-'+CONVERT(NVARCHAR,(@SNLength-1))+',LEN('+@FieldName+'))=(SELECT MAX(SUBSTRING('+@FieldName+',LEN('+@FieldName+')-'+CONVERT(NVARCHAR,(@SNLength-1))+',LEN('+@FieldName+'))) FROM '+@TableName+' WHERE LEN('+@FieldName+')='+CONVERT(NVARCHAR,LEN(@TempBillNo))");
                    sb.AppendLine("	SET @strSql =@strSql+ ' AND SUBSTRING('+@FieldName+',0,LEN('+@FieldName+')-'+CONVERT(NVARCHAR,(@SNLength-1))+')='''+@BillNo+''''+')'");
                    sb.AppendLine("	EXEC sys.sp_executesql @strSql,N'@IdentifyExistNo nvarchar(255) output,@IdentifyExistSN nvarchar(255) output',@IdentifyExistNo output,@IdentifyExistSN output");
                    sb.AppendLine("	IF @IdentifyExistNo<>''");
                    sb.AppendLine("	BEGIN");
                    sb.AppendLine("	SET @IdentifyTempSN= CONVERT(NVARCHAR,CONVERT(DECIMAL,@IdentifyExistSN)+1)");
                    sb.AppendLine("	IF LEN(@IdentifyTempSN)<@IdentifyTempSN");
                    sb.AppendLine("	SET @IdentifyTempSN= REPLACE(right(cast(power(10,9) as varchar)+@IdentifyTempSN,@SNLength),'0',@PlaceHolder)");
                    sb.AppendLine("	SET @returnChar=@BillNo + @IdentifyTempSN");
                    sb.AppendLine("	END");
                    sb.AppendLine("	ELSE IF @IdentifyExistNo=''");
                    sb.AppendLine("	SET @returnChar=@BillNo+ REPLACE(right(cast(power(10,9) as varchar)+@SerialNumber,@SNLength),'0',@PlaceHolder)");
                    sb.AppendLine("	ELSE IF @IdentifyExistNo IS NULL");
                    sb.AppendLine("	SET @returnChar=@BillNo+ REPLACE(right(cast(power(10,9) as varchar)+@SerialNumber,@SNLength),'0',@PlaceHolder)");
                    sb.AppendLine("END");
                    #endregion
                    #region 执行
                    try
                    {
                        //创建存储过程
                        CommonOperate.ExecuteNonQuery(out errMsg, sb.ToString(), null, dbLinkArgs.ConnString, dbLinkArgs.DbType);
                    }
                    catch { }
                    #endregion
                }
                #endregion
            }
            else if (dbLinkArgs.DbType == DatabaseType.MySql)
            {
                #region 创建数据库
                StringBuilder createDbSb = new StringBuilder();
                createDbSb.AppendFormat("CREATE DATABASE IF NOT EXISTS {0};", dbLinkArgs.DbName);
                string connStr = string.Format("Data Source={0};Persist Security Info=yes;UserId={1}; PWD={2};", dbLinkArgs.DataSource, dbLinkArgs.UserId, dbLinkArgs.Pwd);
                CommonOperate.ExecuteNonQuery(out errMsg, createDbSb.ToString(), null, connStr, dbLinkArgs.DbType);
                #endregion
                #region 分页存储过程
                #region 创建
                sb.AppendLine("DELIMITER");
                sb.AppendLine("$$");
                sb.AppendLine("USE `" + dbLinkArgs.DbName + "`;");
                sb.AppendLine("DROP PROCEDURE IF EXISTS `GetPageTableByRowNumber`;");
                sb.AppendLine("CREATE DEFINER = `" + dbLinkArgs.UserId + "`@`" + dbLinkArgs.DataSource + "` PROCEDURE `GetPageTableByRowNumber`(IN `@Field` VARCHAR(1000),IN `@TableName` VARCHAR(2000),IN `@condition` VARCHAR(4000),IN `@OrderField` VARCHAR(100),IN `@OrderType` int,IN `@pageindx` int,IN `@PageSize` int,OUT `@RecordCount` int)");
                sb.AppendLine("BEGIN");
                sb.AppendLine("	   DECLARE m_begin_row INT DEFAULT 0;");
                sb.AppendLine("    DECLARE m_limit_string CHAR(64);");
                sb.AppendLine("    DECLARE m_orderType CHAR(10);");
                sb.AppendLine("    set m_orderType=' DESC ';");
                sb.AppendLine("    SET m_begin_row = (@pageindx - 1) * @PageSize;");
                sb.AppendLine("    SET m_limit_string = CONCAT(' LIMIT ', m_begin_row, ', ', @PageSize);");
                sb.AppendLine("    IF @OrderType=1 THEN");
                sb.AppendLine("       SET m_orderType=' ASC ';");
                sb.AppendLine("    END IF;");
                sb.AppendLine("    SET @COUNT_STRING = CONCAT('SELECT COUNT(*) INTO @ROWS_TOTAL FROM ', @TableName, ' ', @condition);");
                sb.AppendLine("    SET @MAIN_STRING = CONCAT('SELECT ', @Field, ' FROM ', @TableName, ' ', @condition, ' ORDER BY ', @OrderField,m_orderType, m_limit_string);");
                sb.AppendLine("    PREPARE count_stmt FROM @COUNT_STRING;");
                sb.AppendLine("    EXECUTE count_stmt;");
                sb.AppendLine("    DEALLOCATE PREPARE count_stmt;");
                sb.AppendLine("    SET @RecordCount = @ROWS_TOTAL;");
                sb.AppendLine("    PREPARE main_stmt FROM @MAIN_STRING;");
                sb.AppendLine("    EXECUTE main_stmt;");
                sb.AppendLine("    DEALLOCATE PREPARE main_stmt;");
                sb.AppendLine("END;");
                sb.AppendLine("$$");
                sb.AppendLine("DELIMITER;");
                #endregion
                #region 执行
                try
                {
                    //创建存储过程
                    CommonOperate.ExecuteNonQuery(out errMsg, sb.ToString(), null, dbLinkArgs.ConnString, dbLinkArgs.DbType);
                }
                catch { }
                #endregion
                #endregion
            }
        }

        /// <summary>
        /// 获取模块数据库类型，默认MsSqlServer
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public static DatabaseType GetModuleDbType(Sys_Module module, out string connString)
        {
            connString = string.Empty;
            DatabaseType dbTypeEnum = DatabaseType.MsSqlServer;
            if (module == null || module.DataSourceTypeOfEnum != ModuleDataSourceType.DbTable ||
                string.IsNullOrEmpty(module.TableName))
                return dbTypeEnum;
            Type modelType = GetModelType(module.Id);
            if (modelType == null) return dbTypeEnum;
            string errMsg = string.Empty;
            string dbType = string.Empty;
            connString = ModelConfigHelper.GetModelConnString(modelType, out dbType);
            if (string.IsNullOrWhiteSpace(dbType)) dbType = "0";
            try
            {
                dbTypeEnum = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbType);
            }
            catch { }
            return dbTypeEnum;
        }

        /// <summary>
        /// 获取模块索引信息
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        public static TbIndexInfo GetTableIndexInfo(string moduleName)
        {
            Sys_Module module = GetModuleByName(moduleName);
            if (module == null || module.DataSourceTypeOfEnum != ModuleDataSourceType.DbTable ||
               string.IsNullOrEmpty(module.TableName))
                return null;
            string errMsg = string.Empty;
            string connString = string.Empty;
            DatabaseType dbTypeEnum = GetModuleDbType(module, out connString);
            if (dbTypeEnum == DatabaseType.MsSqlServer)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT '[' + DB_NAME() + '].[' + OBJECT_SCHEMA_NAME(ddips.[object_id],");
                sb.Append(" DB_ID())+ '].['");
                sb.Append("+ OBJECT_NAME(ddips.[object_id], DB_ID()) + ']' AS [TableName] ,");
                sb.Append("i.[name] AS [IndexName] ,");
                sb.Append("ddips.[index_type_desc] AS [IndexTypeDes],");
                sb.Append("ddips.[partition_number] AS [PartitionNum],");
                sb.Append("ddips.[alloc_unit_type_desc],");
                sb.Append("ddips.[index_depth] ,");
                sb.Append("ddips.[index_level] ,");
                sb.Append("CAST(ddips.[avg_fragmentation_in_percent]AS SMALLINT) AS [FragmentationPercent] ,");
                sb.Append("CAST(ddips.[avg_fragment_size_in_pages]AS SMALLINT) AS [avg_frag_size_in_pages] ,");
                sb.Append("ddips.[fragment_count] ,");
                sb.Append("ddips.[page_count] ");
                sb.Append("FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'limited') ddips ");
                sb.Append("INNER JOIN sys.[indexes] i ON ddips.[object_id] = i.[object_id] ");
                sb.Append("AND ddips.[index_id] = i.[index_id] ");
                sb.AppendFormat("WHERE OBJECT_NAME(ddips.[object_id])='{0}' AND ddips.[alloc_unit_type_desc]='IN_ROW_DATA'", module.TableName);
                DataTable dt = CommonOperate.ExecuteQuery(out errMsg, sb.ToString(), null, connString, dbTypeEnum);
                TbIndexInfo tbIndex = ObjectHelper.FillModel<TbIndexInfo>(dt).FirstOrDefault();
                return tbIndex;
            }
            return null;
        }

        /// <summary>
        /// 重建数据表索引
        /// </summary>
        /// <param name="moduleName"></param>
        public static void RebuildTableIndex(string moduleName)
        {
            Sys_Module module = GetModuleByName(moduleName);
            if (module == null || module.DataSourceTypeOfEnum != ModuleDataSourceType.DbTable ||
                string.IsNullOrEmpty(module.TableName))
                return;
            Type modelType = CommonOperate.GetModelType(module.Id);
            if (ModelConfigHelper.ModelIsViewMode(modelType)) //视图模式退出
                return;
            string errMsg = string.Empty;
            string connString = string.Empty;
            DatabaseType dbTypeEnum = GetModuleDbType(module, out connString);
            if (dbTypeEnum == DatabaseType.MsSqlServer)
            {
                CommonOperate.ExecuteNonQuery(out errMsg, string.Format("DBCC DBREINDEX('{0}')", module.TableName), null, connString, dbTypeEnum);
            }
        }

        /// <summary>
        /// 重建所有模块索引
        /// </summary>
        public static void RebuildAllTableIndex()
        {
            string errMsg = string.Empty;
            List<Sys_DbConfig> list = CommonOperate.GetEntities<Sys_DbConfig>(out errMsg, null, null, false);
            if (list.Count > 0)
            {
                foreach (Sys_DbConfig dbConfig in list)
                {
                    if (!dbConfig.AutoReCreateIndex) continue;
                    TbIndexInfo indexInfo = GetTableIndexInfo(dbConfig.ModuleName);
                    if (indexInfo != null && indexInfo.FragmentationPercent > 0 &&
                        dbConfig.CreateIndexPageDensity > 0 &&
                        indexInfo.FragmentationPercent >= dbConfig.CreateIndexPageDensity)
                    {
                        RebuildTableIndex(dbConfig.ModuleName);
                    }
                }
            }
        }

        /// <summary>
        /// 获取模块主键索引名
        /// </summary>
        /// <param name="module">模块</param>
        public static string GetModulePrimarykeyIndexName(Sys_Module module)
        {
            if (module == null || module.DataSourceTypeOfEnum != ModuleDataSourceType.DbTable ||
                string.IsNullOrEmpty(module.TableName))
                return string.Empty;
            string errMsg = string.Empty;
            string connString = string.Empty;
            DatabaseType dbTypeEnum = GetModuleDbType(module, out connString);
            if (dbTypeEnum == DatabaseType.MsSqlServer)
            {
                string sql = string.Format("SELECT B.NAME FROM  SYSOBJECTS A JOIN SYSOBJECTS B ON A.ID=B.PARENT_OBJ AND A.XTYPE='U' AND B.XTYPE='PK' AND A.NAME = '{0}'", module.TableName);
                object obj = CommonOperate.ExecuteScale(out errMsg, sql, null, connString, dbTypeEnum);
                return obj.ObjToStr();
            }
            return string.Empty;
        }

        #endregion

        #region 加载模块缓存

        /// <summary>
        /// 加载所有模块缓存
        /// </summary>
        public static void LoadAllModuleCache()
        {
            string errMsg = string.Empty;
            List<Sys_CacheConfig> list = CommonOperate.GetEntities<Sys_CacheConfig>(out errMsg, x => x.IsEnableCache == true, null, false).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (Sys_CacheConfig cacheConfig in list)
                {
                    Guid moduleId = SystemOperate.GetModuleIdByName(cacheConfig.ModuleName);
                    if (moduleId != Guid.Empty)
                    {
                        CommonOperate.GetEntity(moduleId, null, null, out errMsg);
                    }
                }
            }
        }

        #endregion

        #region 事件通知

        #region 触发事件

        /// <summary>
        /// 触发事件通知
        /// </summary>
        /// <param name="eventNotifys">事件通知集合</param>
        /// <param name="recordIds">记录ID集合</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="otherDicTos">其他收件人</param>
        /// <param name="otherDicCcs">其他抄送人</param>
        /// <param name="otherDicBccs">其他密送人</param>
        /// <param name="sendFlag">发送标识，写入到发送日志中</param>
        private static void TriggerEventNotify(List<Msg_EventNotify> eventNotifys, List<Guid> recordIds, UserInfo currUser, Dictionary<string, string> otherDicTos = null, Dictionary<string, string> otherDicCcs = null, Dictionary<string, string> otherDicBccs = null, string sendFlag = null)
        {
            if (eventNotifys == null || eventNotifys.Count == 0 ||
                recordIds == null || recordIds.Count == 0)
                return;
            string errMsg = string.Empty;
            Guid moduleId = eventNotifys.FirstOrDefault().Sys_ModuleId.Value;
            List<Sys_FormField> formFields = SystemOperate.GetDefaultFormField(moduleId);
            foreach (Msg_EventNotify obj in eventNotifys)
            {
                if (!obj.Msg_TemplateId.HasValue)
                    continue;
                Msg_Template template = GetAllMsgTemplates(x => x.Id == obj.Msg_TemplateId.Value).FirstOrDefault();
                if (template == null || string.IsNullOrEmpty(template.Title) || string.IsNullOrEmpty(template.Content))
                    continue;
                List<Msg_To> msgTos = CommonOperate.GetEntities<Msg_To>(out errMsg, x => x.Msg_EventNotifyId == obj.Id && !x.IsDeleted, null, false);
                List<Msg_Cc> msgCcs = CommonOperate.GetEntities<Msg_Cc>(out errMsg, x => x.Msg_EventNotifyId == obj.Id && !x.IsDeleted, null, false);
                List<Msg_Bcc> msgBccs = CommonOperate.GetEntities<Msg_Bcc>(out errMsg, x => x.Msg_EventNotifyId == obj.Id && !x.IsDeleted, null, false);
                List<OrgM_Emp> empTos = new List<OrgM_Emp>(); //收件人
                List<string> otherTos = new List<string>(); //其他收件人
                List<OrgM_Emp> empCcs = new List<OrgM_Emp>(); //抄送人
                List<string> otherCcs = new List<string>(); //其他抄送人
                List<OrgM_Emp> empBccs = new List<OrgM_Emp>(); //密送人
                List<string> otherBccs = new List<string>(); //其他密送人
                #region 取收件人
                foreach (Msg_To to in msgTos)
                {
                    if (!string.IsNullOrEmpty(to.OtherReceiver))
                    {
                        otherTos = to.OtherReceiver.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    if (string.IsNullOrEmpty(to.ReceiverRange))
                        continue;
                    string[] token = to.ReceiverRange.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (token.Length == 0) continue;
                    List<Guid> rangIds = token.Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
                    if (rangIds.Count == 0) continue;
                    switch (to.ReceiverTypeOfEnum)
                    {
                        case ReceiverTypeEnum.Dept:
                            {
                                foreach (Guid deptId in rangIds)
                                {
                                    empTos.AddRange(OrgMOperate.GetDeptEmps(deptId));
                                }
                            }
                            break;
                        case ReceiverTypeEnum.Duty:
                            {
                                foreach (Guid dutyId in rangIds)
                                {
                                    empTos.AddRange(OrgMOperate.GetDutyEmps(dutyId));
                                }
                            }
                            break;
                        case ReceiverTypeEnum.Employee:
                            {
                                foreach (Guid empId in rangIds)
                                {
                                    empTos.Add(OrgMOperate.GetEmp(empId));
                                }
                            }
                            break;
                        case ReceiverTypeEnum.Position:
                            {
                                foreach (Guid positionId in rangIds)
                                {
                                    empTos.AddRange(OrgMOperate.GetPositionEmps(positionId));
                                }
                            }
                            break;
                        case ReceiverTypeEnum.Role:
                            {
                                foreach (Guid roleId in rangIds)
                                {
                                    List<Guid> userIds = PermissionOperate.GetUserIdsOfRole(roleId);
                                    foreach (Guid userId in userIds)
                                    {
                                        empTos.Add(OrgMOperate.GetEmpByUserId(userId));
                                    }
                                }
                            }
                            break;
                    }
                }
                #endregion
                if (obj.NotifyTypeOfEnum == EventNotifyTypeEnum.Email || obj.NotifyTypeOfEnum == EventNotifyTypeEnum.EmailSms || obj.NotifyTypeOfEnum == EventNotifyTypeEnum.EmailSys)
                {
                    #region 取抄送人
                    foreach (Msg_Cc cc in msgCcs)
                    {
                        if (!string.IsNullOrEmpty(cc.OtherReceiver))
                        {
                            otherCcs = cc.OtherReceiver.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        if (string.IsNullOrEmpty(cc.ReceiverRange))
                            continue;
                        string[] token = cc.ReceiverRange.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (token.Length == 0) continue;
                        List<Guid> rangIds = token.Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
                        if (rangIds.Count == 0) continue;
                        switch (cc.ReceiverTypeOfEnum)
                        {
                            case ReceiverTypeEnum.Dept:
                                {
                                    foreach (Guid deptId in rangIds)
                                    {
                                        empTos.AddRange(OrgMOperate.GetDeptEmps(deptId));
                                    }
                                }
                                break;
                            case ReceiverTypeEnum.Duty:
                                {
                                    foreach (Guid dutyId in rangIds)
                                    {
                                        empTos.AddRange(OrgMOperate.GetDutyEmps(dutyId));
                                    }
                                }
                                break;
                            case ReceiverTypeEnum.Employee:
                                {
                                    foreach (Guid empId in rangIds)
                                    {
                                        empTos.Add(OrgMOperate.GetEmp(empId));
                                    }
                                }
                                break;
                            case ReceiverTypeEnum.Position:
                                {
                                    foreach (Guid positionId in rangIds)
                                    {
                                        empTos.AddRange(OrgMOperate.GetPositionEmps(positionId));
                                    }
                                }
                                break;
                            case ReceiverTypeEnum.Role:
                                {
                                    foreach (Guid roleId in rangIds)
                                    {
                                        List<Guid> userIds = PermissionOperate.GetUserIdsOfRole(roleId);
                                        foreach (Guid userId in userIds)
                                        {
                                            empTos.Add(OrgMOperate.GetEmpByUserId(userId));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                    #region 取密送人
                    foreach (Msg_Bcc bcc in msgBccs)
                    {
                        if (!string.IsNullOrEmpty(bcc.OtherReceiver))
                        {
                            otherBccs = bcc.OtherReceiver.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        if (string.IsNullOrEmpty(bcc.ReceiverRange))
                            continue;
                        string[] token = bcc.ReceiverRange.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (token.Length == 0) continue;
                        List<Guid> rangIds = token.Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
                        if (rangIds.Count == 0) continue;
                        switch (bcc.ReceiverTypeOfEnum)
                        {
                            case ReceiverTypeEnum.Dept:
                                {
                                    foreach (Guid deptId in rangIds)
                                    {
                                        empTos.AddRange(OrgMOperate.GetDeptEmps(deptId));
                                    }
                                }
                                break;
                            case ReceiverTypeEnum.Duty:
                                {
                                    foreach (Guid dutyId in rangIds)
                                    {
                                        empTos.AddRange(OrgMOperate.GetDutyEmps(dutyId));
                                    }
                                }
                                break;
                            case ReceiverTypeEnum.Employee:
                                {
                                    foreach (Guid empId in rangIds)
                                    {
                                        empTos.Add(OrgMOperate.GetEmp(empId));
                                    }
                                }
                                break;
                            case ReceiverTypeEnum.Position:
                                {
                                    foreach (Guid positionId in rangIds)
                                    {
                                        empTos.AddRange(OrgMOperate.GetPositionEmps(positionId));
                                    }
                                }
                                break;
                            case ReceiverTypeEnum.Role:
                                {
                                    foreach (Guid roleId in rangIds)
                                    {
                                        List<Guid> userIds = PermissionOperate.GetUserIdsOfRole(roleId);
                                        foreach (Guid userId in userIds)
                                        {
                                            empTos.Add(OrgMOperate.GetEmpByUserId(userId));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                }
                bool isSendEmail = false; //是否需要发送邮件
                bool isSendSms = false; //是否需要发送短信
                bool isSendSys = false; //是否需要发送系统信息
                switch (obj.NotifyTypeOfEnum)
                {
                    case EventNotifyTypeEnum.Email:
                        isSendEmail = true;
                        break;
                    case EventNotifyTypeEnum.EmailSms:
                        isSendEmail = true;
                        isSendSms = true;
                        break;
                    case EventNotifyTypeEnum.EmailSys:
                        isSendEmail = true;
                        isSendSys = true;
                        break;
                    case EventNotifyTypeEnum.Sms:
                        isSendSms = true;
                        break;
                    case EventNotifyTypeEnum.Sys:
                        isSendSys = true;
                        break;
                    case EventNotifyTypeEnum.SysSms:
                        isSendSys = true;
                        isSendSms = true;
                        break;
                    case EventNotifyTypeEnum.SysSmsEmail:
                        isSendEmail = true;
                        isSendSms = true;
                        isSendSys = true;
                        break;
                }
                foreach (Guid recordId in recordIds)
                {
                    string title = template.Title;
                    string content = template.Content;
                    //异步处理
                    Task.Factory.StartNew(() =>
                    {
                        #region 替换模板字段
                        foreach (Sys_FormField formField in formFields)
                        {
                            string needReplace = "{" + formField.Sys_FieldName + "}";
                            if (title.Contains(needReplace) || content.Contains(needReplace))
                            {
                                string fieldValue = SystemOperate.GetFieldDisplayValue(moduleId, recordId, formField.Sys_FieldName);
                                if (title.Contains(needReplace))
                                    title = title.Replace(needReplace, fieldValue);
                                if (content.Contains(needReplace))
                                    content = content.Replace(needReplace, fieldValue);
                            }
                        }
                        #endregion
                        if (isSendEmail) //发送邮件
                        {
                            #region 组装收件人、抄送人、密送人
                            Dictionary<string, string> dicTo = empTos.Distinct(new DistinctComparer<OrgM_Emp>("Email")).ToDictionary(x => x.Email, y => y.Name);
                            Dictionary<string, string> dicCc = empCcs.Count > 0 ? empCcs.Distinct(new DistinctComparer<OrgM_Emp>("Email")).ToDictionary(x => x.Email, y => y.Name) : null;
                            Dictionary<string, string> dicBcc = empBccs.Count > 0 ? empBccs.Distinct(new DistinctComparer<OrgM_Emp>("Email")).ToDictionary(x => x.Email, y => y.Name) : null;
                            if (otherTos.Count > 0)
                            {
                                foreach (string tempTo in otherTos)
                                {
                                    if (dicTo.ContainsKey(tempTo))
                                        continue;
                                    dicTo.Add(tempTo, tempTo);
                                }
                            }
                            if (otherDicTos != null && otherDicTos.Count > 0)
                            {
                                foreach (string key in otherDicTos.Keys)
                                {
                                    if (dicTo.ContainsKey(key))
                                        continue;
                                    dicTo.Add(key, otherDicTos[key]);
                                }
                            }
                            if (otherCcs.Count > 0)
                            {
                                if (dicCc == null)
                                    dicCc = new Dictionary<string, string>();
                                foreach (string tempTo in otherCcs)
                                {
                                    if (dicCc.ContainsKey(tempTo))
                                        continue;
                                    dicCc.Add(tempTo, tempTo);
                                }
                            }
                            if (otherDicCcs != null && otherDicCcs.Count > 0)
                            {
                                if (dicCc == null)
                                    dicCc = new Dictionary<string, string>();
                                foreach (string key in otherDicCcs.Keys)
                                {
                                    if (dicCc.ContainsKey(key))
                                        continue;
                                    dicCc.Add(key, otherDicCcs[key]);
                                }
                            }
                            if (otherBccs.Count > 0)
                            {
                                if (dicBcc == null)
                                    dicBcc = new Dictionary<string, string>();
                                foreach (string tempTo in otherBccs)
                                {
                                    if (dicBcc.ContainsKey(tempTo))
                                        continue;
                                    dicBcc.Add(tempTo, tempTo);
                                }
                            }
                            if (otherDicBccs != null && otherDicBccs.Count > 0)
                            {
                                if (dicBcc == null)
                                    dicBcc = new Dictionary<string, string>();
                                foreach (string key in otherDicBccs.Keys)
                                {
                                    if (dicBcc.ContainsKey(key))
                                        continue;
                                    dicBcc.Add(key, otherDicBccs[key]);
                                }
                            }
                            #endregion
                            EmailSend(title, content, dicTo, null, dicCc, dicBcc, true, null, null, null, null, obj.Name, sendFlag);
                        }
                        if (isSendSys) //发送系统消息
                        {
                        }
                        if (isSendSms) //发送短信
                        {
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 触发事件通知，针对模块通用功能
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="operateType">操作类型</param>
        /// <param name="currUser">当前用户</param>
        public static void TriggerEventNotify(Guid moduleId, Guid recordId, ModelRecordOperateType operateType, UserInfo currUser)
        {
            //异步处理
            Task.Factory.StartNew(() =>
            {
                List<Msg_EventNotify> list = GetEventNotifys(moduleId, operateType);
                TriggerEventNotify(list, new List<Guid>() { recordId }, currUser);
            });
        }

        /// <summary>
        /// 触发事件通知
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordIds">记录ID集合</param>
        /// <param name="operateType">操作类型</param>
        /// <param name="currUser">当前用户</param>
        public static void TriggerEventNotify(Guid moduleId, List<Guid> recordIds, ModelRecordOperateType operateType, UserInfo currUser)
        {
            //异步处理
            Task.Factory.StartNew(() =>
            {
                List<Msg_EventNotify> list = GetEventNotifys(moduleId, operateType);
                TriggerEventNotify(list, recordIds, currUser);
            });
        }

        /// <summary>
        /// 触发事件通知，针对流程
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="workNodeId">流程结点ID</param>
        /// <param name="workAction">操作动作</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="starterEmpId">发起人ID</param>
        /// <param name="nextHandlers">下一审批人ID及待办集合</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="directHandler">被指派人ID</param>
        /// <param name="workTodo">当前待办</param>
        public static void TriggerEventNotify(Guid moduleId, Guid recordId, Guid workNodeId, WorkActionEnum workAction, UserInfo currUser, Guid? starterEmpId, Dictionary<Guid, Bpm_WorkToDoList> nextHandlers, WorkFlowStatusEnum flowStatus, Guid? directHandler, Bpm_WorkToDoList workTodo)
        {
            //异步处理
            Task.Factory.StartNew(() =>
            {
                Bpm_WorkNode workNode = BpmOperate.GetWorkNode(workNodeId);
                if (workNode == null || !workNode.Bpm_WorkFlowId.HasValue)
                    return;
                //如果当前为父流程并且发起结点绑定了子流程并且
                //只有两个结点并且第二个结点的处理者类型是发起者时不发邮件
                List<Bpm_WorkNode> workNodes = BpmOperate.GetWorkNodesOfFlow(workNode.Bpm_WorkFlowId.Value).Where(x => x.WorkNodeTypeOfEnum != WorkNodeTypeEnum.Start && x.WorkNodeTypeOfEnum != WorkNodeTypeEnum.End).ToList();
                if (workNodes.Count == 2) //有两个流程结点
                {
                    Bpm_WorkNode launchNode = BpmOperate.GetLaunchNode(workNode.Bpm_WorkFlowId.Value);
                    if (launchNode.Bpm_WorkFlowSubId.HasValue && launchNode.Bpm_WorkFlowSubId.Value != Guid.Empty) //发起结点绑定了子流程
                    {
                        if (workNodes.Where(x => x.Id != launchNode.Id).FirstOrDefault().HandlerTypeOfEnum == NodeHandlerTypeEnum.Starter) //第二个结点的处理者类型是发起者
                        {
                            return;
                        }
                    }
                }
                List<Msg_EventNotify> list = GetEventNotifys(moduleId, workNodeId, workAction);
                if (list.Count == 0) //当前节点未设置模板时走通用处理
                {
                    OrgM_Emp empStarter = starterEmpId.HasValue ? OrgMOperate.GetEmp(starterEmpId.Value) : null;
                    if (flowStatus != WorkFlowStatusEnum.Over && flowStatus != WorkFlowStatusEnum.Refused) //通知下一审批人
                    {
                        WorkTodoHandleCommonEmail(moduleId, recordId, nextHandlers, workNode, workAction, empStarter, currUser, directHandler, flowStatus, workTodo.Title);
                    }
                    else //流程结束知会流程发起人
                    {
                        WorkTodoNotifyCommonEmail(moduleId, recordId, workNode, workAction, empStarter, currUser, flowStatus, workTodo);
                    }
                }
                else if (list.Count > 0) //当前节点已设置了模板
                {
                    string sendFlag = null;
                    bool canEvent = true;
                    if (workTodo.ParentId.HasValue) //针对同一子流程同一父待办只发一次邮件
                    {
                        sendFlag = workTodo.ParentId.Value.ToString();
                        string errMsg = string.Empty;
                        canEvent = CommonOperate.Count<Msg_SendLog>(out errMsg, false, x => x.SendFlag == sendFlag) == 0;
                    }
                    if (canEvent)
                    {
                        TriggerEventNotify(list, new List<Guid>() { recordId }, currUser, null, null, null, sendFlag);
                    }
                }
            });
        }

        /// <summary>
        /// 待办处理通用邮件
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="nextHandlers">下一待办处理人集合</param>
        /// <param name="workNode">当前结点</param>
        /// <param name="workAction">操作</param>
        /// <param name="empStarter">发起者</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="directHandler">被指派人</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="todoTitle">当前待办标题</param>
        private static void WorkTodoHandleCommonEmail(Guid moduleId, Guid recordId, Dictionary<Guid, Bpm_WorkToDoList> nextHandlers, Bpm_WorkNode workNode, WorkActionEnum workAction, OrgM_Emp empStarter, UserInfo currUser, Guid? directHandler, WorkFlowStatusEnum flowStatus, string todoTitle)
        {
            if (nextHandlers == null || nextHandlers.Count == 0 || workNode == null || currUser == null)
                return;
            string webServer = WebConfigHelper.GetAppSettingValue("WebServer"); //跳转服务器
            string webIndex = WebConfigHelper.GetAppSettingValue("WebIndex"); //跳转首页地址
            string webHome = Globals.GetBaseUrl();
            //组装知会查看页面URL
            string notifyUrl = string.Format("/Page/ViewForm.html?page=view&mode=1&moduleId={0}&id={1}&title={2}", moduleId.ToString(), recordId.ToString(), todoTitle);
            if (!webServer.Contains(webHome))
            {
                notifyUrl += "&nfm=1&emialFlag=1";
                notifyUrl = notifyUrl.Replace("&", "__");
                notifyUrl = webHome + HttpUtility.UrlEncode(notifyUrl, Encoding.UTF8);
            }
            else
            {
                notifyUrl = HttpUtility.UrlEncode(notifyUrl, Encoding.UTF8);
            }
            notifyUrl = string.Format("{0}{1}?returnUrl={2}", webServer, webIndex, notifyUrl);
            //组装待办处理页面URL
            Dictionary<Guid, string> nextHandlerReturns = new Dictionary<Guid, string>();
            foreach (Guid empId in nextHandlers.Keys)
            {
                if (nextHandlerReturns.ContainsKey(empId))
                    continue;
                Bpm_WorkToDoList workTodo = nextHandlers[empId]; //下一处理人待办
                Bpm_WorkNode nextNode = BpmOperate.GetWorkNode(workTodo.Bpm_WorkNodeId.Value);
                if (nextNode == null) continue;
                string url = string.Empty;
                if ((nextNode.Sys_FormId == null || nextNode.Sys_FormId == Guid.Empty) && !string.IsNullOrEmpty(nextNode.FormUrl)) //自定义流程表单
                {
                    if (webServer.Contains(webHome))
                        url = nextNode.FormUrl;
                    else
                        url = webHome + nextNode.FormUrl;
                    if (!url.Contains("?"))
                        url += "?";
                    else if (url.Contains("&"))
                        url += "&";
                    url += string.Format("moduleId={0}&id={1}&todoId={2}&node={3}&title={4}", moduleId.ToString(), recordId.ToString(), workTodo.Id.ToString(), nextNode.Name, workTodo.Title);
                }
                else
                {
                    url = string.Format("/Page/EditForm.html?page=edit&mode=1&moduleId={0}&id={1}&todoId={2}&node={3}&title={4}", moduleId.ToString(), recordId.ToString(), workTodo.Id.ToString(), nextNode.Name, workTodo.Title);
                }
                if (!webServer.Contains(webHome))
                {
                    url += "&nfm=1&emialFlag=1";
                    url = url.Replace("&", "__");
                    url = webHome + HttpUtility.UrlEncode(url, Encoding.UTF8);
                }
                else
                {
                    url = HttpUtility.UrlEncode(url, Encoding.UTF8);
                }
                url = string.Format("{0}{1}?returnUrl={2}", webServer, webIndex, url);
                nextHandlerReturns.Add(empId, url);
            }
            List<string> subjectContens = new List<string>(); //主题内容,List[0]为主题，List[1]为内容
            object returnObj = CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, "WorkflowMsgNotify", new object[] { recordId, workNode.Name, workAction, flowStatus, currUser, empStarter, nextHandlerReturns, notifyUrl, true, null, subjectContens });
            if (returnObj == null) //继续走通用处理
            {
                string handleDisplay = string.Empty;
                int btnType = (int)FlowButtonTypeEnum.AgreeBtn;
                switch (workAction)
                {
                    case WorkActionEnum.Returning:
                        btnType = (int)FlowButtonTypeEnum.BackBtn;
                        break;
                    case WorkActionEnum.Refusing:
                        btnType = (int)FlowButtonTypeEnum.RejectBtn;
                        break;
                    case WorkActionEnum.Directing:
                        btnType = (int)FlowButtonTypeEnum.AssignBtn;
                        break;
                    case WorkActionEnum.Starting:
                    case WorkActionEnum.SubStarting:
                        handleDisplay = "发起";
                        break;
                    case WorkActionEnum.ReStarting:
                        handleDisplay = "重新发起";
                        break;
                }
                if (workAction != WorkActionEnum.Starting && workAction != WorkActionEnum.SubStarting && workAction != WorkActionEnum.ReStarting)
                {
                    Bpm_FlowBtn flowBtn = BpmOperate.GetAllWorkButtons(x => x.ButtonType == btnType).FirstOrDefault();
                    if (flowBtn != null && workNode.Bpm_WorkFlowId.HasValue)
                    {
                        Bpm_NodeBtnConfig btnConfig = BpmOperate.GetAllApprovalBtnConfigs(x => x.Bpm_FlowBtnId == flowBtn.Id && x.Bpm_WorkFlowId == workNode.Bpm_WorkFlowId.Value && x.Bpm_WorkNodeId == workNode.Id).FirstOrDefault();
                        if (btnConfig != null)
                            handleDisplay = btnConfig.BtnDisplay;
                        else
                            handleDisplay = flowBtn.ButtonText;
                    }
                }
                if (string.IsNullOrEmpty(handleDisplay))
                    return;
                string moduleDisplay = SystemOperate.GetModuleDiplay(moduleId);
                string titleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(moduleId);
                string titleKeyValue = string.IsNullOrEmpty(titleKeyDisplay) ? string.Empty : CommonOperate.GetModelTitleKeyValue(moduleId, recordId);
                string starterName = empStarter != null ? empStarter.Name : string.Empty;
                string tempStarterDes = string.IsNullOrEmpty(starterName) ||
                                        workAction == WorkActionEnum.Starting ||
                                        workAction == WorkActionEnum.ReStarting ||
                                        workAction == WorkActionEnum.SubStarting ?
                                        string.Empty : string.Format("{0}的", starterName);
                string title = string.Format("{0}{1}了{2}{3}流程", currUser.EmpName, handleDisplay, tempStarterDes, moduleDisplay);
                string formatStr = string.IsNullOrEmpty(titleKeyDisplay) || string.IsNullOrEmpty(titleKeyValue) ? string.Empty : string.Format("【{0}：{1}】", titleKeyDisplay, titleKeyValue);
                title += formatStr;
                string subject = string.Format("【流程处理】{0}", title);
                if (subjectContens != null && subjectContens.Count > 0 && !string.IsNullOrEmpty(subjectContens[0]))
                    subject = subjectContens[0]; //有自定义标题取自定义标题
                foreach (Guid empId in nextHandlers.Keys)
                {
                    if (!nextHandlerReturns.ContainsKey(empId))
                        continue;
                    Bpm_WorkToDoList workTodo = nextHandlers[empId]; //下一处理人待办
                    string url = nextHandlerReturns[empId];
                    string content = string.Format("{0}，转到流程处理页面请点击以下链接：<br />{1}", title, url);
                    Dictionary<string, string> otherDicTos = directHandler.HasValue ? OrgMOperate.GetEmployeeEmails(new List<Guid>() { directHandler.Value }) : OrgMOperate.GetEmployeeEmails(new List<Guid>() { empId });
                    
                    if (subjectContens != null && subjectContens.Count > 1 && !string.IsNullOrEmpty(subjectContens[1]))
                        content = subjectContens[1] + "<br />" + content;  //加上自定义内容

                    string eventName = string.Format("{0}流程{1}结点审批", moduleDisplay, workNode.Name);
                    string sendFlag = workTodo.ParentId.HasValue && workTodo.ParentId.Value != Guid.Empty ? workTodo.ParentId.Value.ObjToStr() : string.Empty;
                    EmailSend(subject, content, otherDicTos, null, null, null, true, null, null, null, null, eventName, sendFlag);
                }
            }
        }

        /// <summary>
        /// 待办知会通用邮件
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="workNode">当前结点</param>
        /// <param name="workAction">操作</param>
        /// <param name="empStarter">发起者</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="flowStatus">流程状态</param>
        /// <param name="workTodo">当前待办</param>
        private static void WorkTodoNotifyCommonEmail(Guid moduleId, Guid recordId, Bpm_WorkNode workNode, WorkActionEnum workAction, OrgM_Emp empStarter, UserInfo currUser, WorkFlowStatusEnum flowStatus, Bpm_WorkToDoList workTodo)
        {
            if (workNode == null || empStarter == null || workTodo == null)
                return;
            string webServer = WebConfigHelper.GetAppSettingValue("WebServer"); //跳转服务器
            string webIndex = WebConfigHelper.GetAppSettingValue("WebIndex"); //跳转首页地址
            string webHome = Globals.GetBaseUrl(); //当前站点地址
            //组装知会跳转URL
            string url = string.Format("/Page/ViewForm.html?page=view&mode=1&moduleId={0}&id={1}&title={2}", moduleId.ToString(), recordId.ToString(), workTodo.Title);
            if (!webServer.Contains(webHome))
            {
                url += "&nfm=1&emialFlag=1";
                url = url.Replace("&", "__");
                url = webHome + HttpUtility.UrlEncode(url, Encoding.UTF8);
            }
            else
            {
                url = HttpUtility.UrlEncode(url, Encoding.UTF8);
            }
            url = string.Format("{0}{1}?returnUrl={2}", webServer, webIndex, url);
            //调用自定义处理接口
            Dictionary<string, string> otherDicCcs = new Dictionary<string, string>(); //需要抄送人员
            List<string> subjectContens = new List<string>(); //主题内容,List[0]为主题，List[1]为内容
            object[] args = new object[] { recordId, workNode.Name, workAction, flowStatus, currUser, empStarter, null, url, true, otherDicCcs, subjectContens };
            object returnObj = CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, "WorkflowMsgNotify", args);
            if (returnObj == null) //继续走通用处理
            {
                string titleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(moduleId);
                string titleKeyValue = string.IsNullOrEmpty(titleKeyDisplay) ? string.Empty : CommonOperate.GetModelTitleKeyValue(moduleId, recordId);
                string moduleDisplay = SystemOperate.GetModuleDiplay(moduleId);

                Dictionary<string, string> otherDicTos = OrgMOperate.GetEmployeeEmails(new List<Guid>() { empStarter.Id });
                string subject = string.Empty; //邮件主题
                string content = string.Empty; //邮件内容
                if (subjectContens != null && subjectContens.Count > 0 && !string.IsNullOrEmpty(subjectContens[0]))
                {
                    subject = subjectContens[0]; //取自定义邮件主题
                }
                else //通用邮件主题
                {
                    string formatStr = string.IsNullOrEmpty(titleKeyDisplay) || string.IsNullOrEmpty(titleKeyValue) ? string.Empty : string.Format("【{0}：{1}】", titleKeyDisplay, titleKeyValue);
                    string title = string.Format("您发起的【{0}】流程{1}已{2}", moduleDisplay, formatStr, flowStatus == WorkFlowStatusEnum.Over ? "通过审批" : "被拒绝");
                    subject = string.Format("【流程知会】{0}", title);
                }
                if (subjectContens != null && subjectContens.Count > 1 && !string.IsNullOrEmpty(subjectContens[1]))
                {
                    content = subjectContens[1]; //取自定义邮件内容
                }
                else //通用邮件内容
                {
                    string formatStr = string.IsNullOrEmpty(titleKeyDisplay) || string.IsNullOrEmpty(titleKeyValue) ? string.Empty : string.Format("【{0}：{1}】", titleKeyDisplay, titleKeyValue);
                    string title = string.Format("您发起的【{0}】流程{1}已{2}", moduleDisplay, formatStr, flowStatus == WorkFlowStatusEnum.Over ? "通过审批" : "被拒绝");
                    content = string.Format("{0}，需要查看详情请点击以下链接：<br />{1}", title, url);
                }
                string eventName = string.Format("{0}流程{1}结点审批", moduleDisplay, workNode.Name);
                string sendFlag = workTodo.ParentId.HasValue && workTodo.ParentId.Value != Guid.Empty ? workTodo.ParentId.Value.ObjToStr() : string.Empty;
                EmailSend(subject, content, otherDicTos, null, otherDicCcs, null, true, null, null, null, null, eventName, sendFlag);
            }
        }

        /// <summary>
        /// 触发事件通知，针对自定义事件
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="flag">标识</param>
        /// <param name="currUser">当前用户</param>
        public static void TriggerEventNotify(Guid moduleId, Guid recordId, string flag, UserInfo currUser)
        {
            //异步处理
            Task.Factory.StartNew(() =>
            {
                List<Msg_EventNotify> list = GetEventNotifys(moduleId, flag);
                TriggerEventNotify(list, new List<Guid>() { recordId }, currUser);
            });
        }

        #endregion

        #region 事件通知操作

        /// <summary>
        /// 获取所有合法的事件通知集合
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        private static List<Msg_EventNotify> GetAllEventNotifys(Expression<Func<Msg_EventNotify, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Msg_EventNotify, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => !x.IsDeleted && !x.IsDraft) : x => !x.IsDeleted && !x.IsDraft;
            List<Msg_EventNotify> list = CommonOperate.GetEntities<Msg_EventNotify>(out errMsg, tempExp, null, false);
            list = list.Where(x => (x.ValidStartTime == null && x.ValidEndTime == null) ||
                (x.ValidStartTime != null && x.ValidEndTime == null && x.ValidStartTime.Value < DateTime.Now) ||
                (x.ValidEndTime != null && x.ValidStartTime == null && x.ValidEndTime.Value > DateTime.Now) ||
                (x.ValidEndTime != null && x.ValidStartTime != null && x.ValidStartTime.Value < DateTime.Now && x.ValidEndTime.Value > DateTime.Now)).ToList();
            return list;
        }

        /// <summary>
        /// 获取模块的事件通知集合，功能模块通用操作
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="operateType">操作类型</param>
        /// <returns></returns>
        private static List<Msg_EventNotify> GetEventNotifys(Guid moduleId, ModelRecordOperateType operateType)
        {
            List<Msg_EventNotify> eventList = new List<Msg_EventNotify>();
            if (operateType == ModelRecordOperateType.View) return eventList;
            List<Msg_EventNotify> list = GetAllEventNotifys(x => x.Sys_ModuleId == moduleId);
            foreach (Msg_EventNotify obj in list)
            {
                if (string.IsNullOrEmpty(obj.EventType))
                    continue;
                string[] token = obj.EventType.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (token.Length != 9) continue;
                if (operateType == ModelRecordOperateType.Add)
                {
                    if (token[0] == CommonDefine.MutiCheckboxTrueValue)
                        eventList.Add(obj);
                }
                else if (operateType == ModelRecordOperateType.Edit)
                {
                    if (token[1] == CommonDefine.MutiCheckboxTrueValue)
                        eventList.Add(obj);
                }
                else if (operateType == ModelRecordOperateType.Del)
                {
                    if (token[2] == CommonDefine.MutiCheckboxTrueValue)
                        eventList.Add(obj);
                }
            }
            return eventList;
        }

        /// <summary>
        /// 获取模块的事件通知集合，流程操作
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="workNodeId">流程结点ID</param>
        /// <param name="workAction">流程操作类型</param>
        /// <returns></returns>
        private static List<Msg_EventNotify> GetEventNotifys(Guid moduleId, Guid workNodeId, WorkActionEnum workAction)
        {
            List<Msg_EventNotify> eventList = new List<Msg_EventNotify>();
            if (workAction == WorkActionEnum.NoAction) return eventList;
            List<Msg_EventNotify> list = GetAllEventNotifys(x => x.Sys_ModuleId == moduleId && x.Bpm_WorkNodeId == workNodeId);
            foreach (Msg_EventNotify obj in list)
            {
                if (string.IsNullOrEmpty(obj.EventType))
                    continue;
                string[] token = obj.EventType.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (token.Length != 9) continue;
                if (workAction == WorkActionEnum.Starting || workAction == WorkActionEnum.ReStarting || workAction == WorkActionEnum.SubStarting)
                {
                    if (token[3] == CommonDefine.MutiCheckboxTrueValue)
                        eventList.Add(obj);
                }
                else if (workAction == WorkActionEnum.Approving)
                {
                    if (token[4] == CommonDefine.MutiCheckboxTrueValue)
                        eventList.Add(obj);
                }
                else if (workAction == WorkActionEnum.Refusing)
                {
                    if (token[5] == CommonDefine.MutiCheckboxTrueValue)
                        eventList.Add(obj);
                }
                else if (workAction == WorkActionEnum.Returning)
                {
                    if (token[6] == CommonDefine.MutiCheckboxTrueValue)
                        eventList.Add(obj);
                }
                else if (workAction == WorkActionEnum.Directing)
                {
                    if (token[7] == CommonDefine.MutiCheckboxTrueValue)
                        eventList.Add(obj);
                }
            }
            return eventList;
        }

        /// <summary>
        /// 获取模块的事件通知集合，自定义操作
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private static List<Msg_EventNotify> GetEventNotifys(Guid moduleId, string flag)
        {
            List<Msg_EventNotify> eventList = new List<Msg_EventNotify>();
            if (string.IsNullOrEmpty(flag)) return eventList;
            List<Msg_EventNotify> list = GetAllEventNotifys(x => x.Sys_ModuleId == moduleId && x.Flag == flag);
            foreach (Msg_EventNotify obj in list)
            {
                if (string.IsNullOrEmpty(obj.EventType))
                    continue;
                string[] token = obj.EventType.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (token.Length != 9) continue;
                if (token[8] == CommonDefine.MutiCheckboxTrueValue)
                    eventList.Add(obj);
            }
            return eventList;
        }

        #endregion

        #region 消息模板操作

        /// <summary>
        /// 获取所有合法的消息模板集合
        /// </summary>
        /// <param name="exp">条件表达式</param>
        /// <returns></returns>
        private static List<Msg_Template> GetAllMsgTemplates(Expression<Func<Msg_Template, bool>> exp = null)
        {
            string errMsg = string.Empty;
            Expression<Func<Msg_Template, bool>> tempExp = exp != null ? ExpressionExtension.And(exp, x => !x.IsDeleted && !x.IsDraft) : x => !x.IsDeleted && !x.IsDraft;
            List<Msg_Template> list = CommonOperate.GetEntities<Msg_Template>(out errMsg, tempExp, null, false);
            list = list.Where(x => (x.ValidStartTime == null && x.ValidEndTime == null) ||
                (x.ValidStartTime != null && x.ValidEndTime == null && x.ValidStartTime.Value < DateTime.Now) ||
                (x.ValidEndTime != null && x.ValidStartTime == null && x.ValidEndTime.Value > DateTime.Now) ||
                (x.ValidEndTime != null && x.ValidStartTime != null && x.ValidStartTime.Value < DateTime.Now && x.ValidEndTime.Value > DateTime.Now)).ToList();
            return list;
        }

        #endregion

        #endregion

        #region 邮件发送

        #region SMTP方式

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="attachFiles">附件信息</param>
        /// <param name="tomails">接收人，key-邮件地址，value-员工姓名</param>
        /// <param name="ccmails">抄送人，key-邮件地址，value-员工姓名</param>
        /// <param name="bccmails">密送人，key-邮件地址，value-员工姓名</param>
        /// <param name="isSaveLog">是否保存发送日志</param>
        /// <param name="fromAddress">发件人</param>
        /// <param name="fromDes">发件人描述</param>
        /// <param name="fromPwd">发件人密码</param>
        /// <param name="action">回调函数</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="sendFlag">发送标识，发送日志中用到</param>
        /// <returns>返回异常信息，为空表明成功</returns>
        public static string EmailSend(string subject, string body, Dictionary<string, string> tomails,
            List<string> attachFiles = null,
            Dictionary<string, string> ccmails = null,
            Dictionary<string, string> bccmails = null,
            bool isSaveLog = true,
            string fromAddress = null,
            string fromDes = null,
            string fromPwd = null,
            Action<string> action = null,
            string eventName = null,
            string sendFlag = null)
        {
            if (tomails != null && tomails.Count > 0)
            {
                if (string.IsNullOrEmpty(fromAddress))
                {
                    fromAddress = GlobalSet.SysEmail;
                    fromDes = GlobalSet.SysEmailDes;
                    fromPwd = GlobalSet.SysEmailPwd;
                }
                if (string.IsNullOrEmpty(fromDes))
                    fromDes = fromAddress;
                string testEmails = WebConfigHelper.GetAppSettingValue("TestEmail");
                if (!string.IsNullOrEmpty(testEmails))
                {
                    string[] token = testEmails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (token.Length == 0)
                        return "调试模式下测试邮件地址为空";
                    tomails = token.Distinct().ToDictionary(x => x, y => y);
                    ccmails = null;
                    bccmails = null;
                }
                try
                {
                    MailSendParams mailParams = new MailSendParams(tomails, fromAddress, fromAddress, fromPwd, subject, body, attachFiles, fromDes, ccmails, bccmails);
                    MailSend send = new MailSend(mailParams, true, false);
                    string errmsg = string.Empty;
                    bool success = send.Send(out errmsg, new MailAsyncHandle((isSuccess, message) =>
                    {
                        string err = message;
                    }));
                    if (isSaveLog)
                        SaveMsgLog(eventName, string.Format("{0}({1})", fromDes, fromAddress), tomails != null ? string.Join(",", tomails.Keys) : string.Empty, ccmails != null ? string.Join(",", ccmails.Keys) : string.Empty, bccmails != null ? string.Join(",", bccmails.Keys) : string.Empty, subject, body, success, errmsg, sendFlag);
                    if (action != null)
                        action(errmsg);
                    return errmsg;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "收件人信息为空";
        }

        #endregion

        #region WebMail方式

        /// <summary>
        /// WebMail发送邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="to">接收人</param>
        /// <param name="attachFiles">附件信息</param>
        /// <param name="cc">抄送人</param>
        /// <param name="bcc">密送人</param>
        /// <param name="isSaveLog">是否保存发送日志</param>
        /// <param name="fromAddress">发送人</param>
        /// <param name="fromPwd">发送邮箱密码</param>
        /// <param name="fromDes">发送人描述</param>
        /// <param name="action">回调函数</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="sendFlag">发送标识</param>
        /// <returns>发送成功返回空字符串，否则返回异常信息</returns>
        public static string EmailSendByWebMail(string subject, string body, string to, List<string> attachFiles = null,
            string cc = null, string bcc = null,
            bool isSaveLog = true,
            string fromAddress = null,
            string fromDes = null,
            string fromPwd = null,
            Action<string> action = null,
            string eventName = null,
            string sendFlag = null)
        {
            if (string.IsNullOrEmpty(fromAddress))
            {
                fromAddress = GlobalSet.SysEmail;
                fromDes = GlobalSet.SysEmailDes;
                fromPwd = GlobalSet.SysEmailPwd;
            }
            string testEmails = WebConfigHelper.GetAppSettingValue("TestEmail");
            if (!string.IsNullOrEmpty(testEmails))
            {
                to = testEmails;
                cc = null;
                bcc = null;
            }
            string errMsg = new WebEmailHelper().SendByWebMail(GlobalSet.SmtpServer, GlobalSet.SmtpPort, fromAddress, GlobalSet.SysEmailPwd, subject, body, to, attachFiles, fromDes);
            if (isSaveLog)
                SaveMsgLog(eventName, string.Format("{0}({1})", fromDes, fromAddress), to, null, null, subject, body, string.IsNullOrEmpty(errMsg), errMsg, sendFlag);
            if (action != null)
                action(errMsg);
            return errMsg;
        }

        #endregion

        #endregion

        #region 消息发送日志

        /// <summary>
        /// 保存邮件发送日志
        /// </summary>
        /// <param name="eventName">事件名称,自定义标识</param>
        /// <param name="sender">发送人</param>
        /// <param name="tos">收件人</param>
        /// <param name="ccs">抄送人</param>
        /// <param name="bccs">密送人</param>
        /// <param name="subject">主题</param>
        /// <param name="content">内容</param>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="sendFlag">发送标识</param>
        /// <returns></returns>
        public static void SaveMsgLog(string eventName, string sender, string tos, string ccs, string bccs, string subject, string content, bool isSuccess, string errMsg, string sendFlag = null)
        {
            //异步处理
            Task.Factory.StartNew(() =>
            {
                Msg_SendLog sendLog = new Msg_SendLog()
                {
                    EventName = eventName,
                    Sender = sender,
                    Tos = tos,
                    Ccs = ccs,
                    Bccs = bccs,
                    Subject = subject,
                    Content = content,
                    IsSuccess = isSuccess,
                    ErrMsg = errMsg,
                    SendFlag = sendFlag,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    CreateUserName = "admin",
                    ModifyUserName = "admin"
                };
                string msg = string.Empty;
                CommonOperate.OperateRecord<Msg_SendLog>(sendLog, ModelRecordOperateType.Add, out errMsg, null, false);
            });
        }

        #endregion

        #endregion
    }
}

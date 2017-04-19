/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rookey.Frame.EntityBase.Attr;
using System.Reflection;
using Rookey.Frame.Common;
using Rookey.Frame.EntityBase;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Common.Sys;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Bridge;
using Rookey.Frame.Common.PubDefine;
using Rookey.Frame.Office;
using System.Data;
using System.Collections;
using Rookey.Frame.Operate.Base.OperateHandle;
using ServiceStack.DataAnnotations;
using Rookey.Frame.Model;
using Rookey.Frame.Base;
using System.Configuration;
using System.Web.Configuration;
using Rookey.Frame.Common.Model;
using Rookey.Frame.Model.Bpm;
using Rookey.Frame.Base.Set;
using Rookey.Frame.Model.OrgM;
using Rookey.Frame.Model.Desktop;

namespace Rookey.Frame.Operate.Base
{
    /// <summary>
    /// 工具操作类
    /// </summary>
    public static class ToolOperate
    {
        #region 生成实体相关
        /// <summary>
        /// 创建临时模块
        /// </summary>
        /// <param name="tempModule">模块对象</param>
        /// <param name="tempField">字段信息</param>
        /// <returns>返回异常信息</returns>
        public static string CreateTempModule(Sys_Module tempModule, DetailFormObject tempField)
        {
            if (tempModule == null)
                return "模块对象不能为空！";
            if (string.IsNullOrWhiteSpace(tempModule.Name) && string.IsNullOrWhiteSpace(tempModule.TableName))
                return "模块名称不能为空！";
            string err = string.Empty;
            Sys_Module otherModule = tempModule.Id != Guid.Empty ? CommonOperate.GetEntity<Sys_Module>(x => x.Id != tempModule.Id && x.Name == tempModule.Name, null, out err) :
                                                         CommonOperate.GetEntity<Sys_Module>(x => x.Name == tempModule.Name, null, out err);
            if (otherModule != null)
                return string.Format("名称为【{0}】的模块已存在，不能添加重复模块！", tempModule.Name);
            otherModule = tempModule.Id != Guid.Empty ? CommonOperate.GetEntity<Sys_Module>(x => x.Id != tempModule.Id && x.TableName == tempModule.TableName, null, out err) :
                                              CommonOperate.GetEntity<Sys_Module>(x => x.TableName == tempModule.TableName, null, out err);
            if (otherModule != null)
                return string.Format("表名为【{0}】的模块已存在，不能添加重复模块！", tempModule.TableName);
            if (tempField == null || tempField.ModuleDatas == null || tempField.ModuleDatas.Count == 0)
                return "模块字段的配置数据不能为空！";
            //构建模块配置对象
            Sys_Module parentModule = tempModule.ParentId.HasValue ? SystemOperate.GetModuleById(tempModule.ParentId.Value) : null;
            ModuleConfigAttribute moduleConfig = new ModuleConfigAttribute()
            {
                Name = tempModule.Name,
                TableName = tempModule.TableName,
                PrimaryKeyFields = tempModule.PrimaryKeyFields,
                TitleKey = tempModule.TitleKey,
                IsAllowAdd = tempModule.IsAllowAdd,
                IsAllowCopy = tempModule.IsAllowCopy,
                IsAllowDelete = tempModule.IsAllowDelete,
                IsAllowEdit = tempModule.IsAllowEdit,
                IsAllowExport = tempModule.IsAllowExport,
                IsAllowImport = tempModule.IsAllowImport,
                IsEnableAttachment = tempModule.IsEnableAttachment,
                IsEnabledBatchEdit = tempModule.IsEnabledBatchEdit,
                IsEnabledRecycle = tempModule.IsEnabledRecycle,
                IsEnabledPrint = tempModule.IsEnabledPrint,
                Logo = tempModule.ModuleLogo,
                ParentName = parentModule != null ? parentModule.Name : string.Empty,
                Sort = tempModule.Sort,
                StandardJsFolder = tempModule.StandardJsFolder,
            };
            //构建字段配置对象
            List<FieldConfigAttribute> fieldConfigs = new List<FieldConfigAttribute>();
            foreach (string json in tempField.ModuleDatas)
            {
                List<NameValueObject> nvs = JsonHelper.Deserialize<List<NameValueObject>>(json);
                FieldConfigAttribute fieldConfig = new FieldConfigAttribute();
                Type fieldConfigType = typeof(FieldConfigAttribute);
                //对象属性赋值
                foreach (NameValueObject nv in nvs)
                {
                    PropertyInfo p = fieldConfigType.GetProperty(nv.name);
                    if (p == null) continue;
                    p.SetValue2(fieldConfig, TypeUtil.ChangeType(nv.value, p.PropertyType), null);
                }
                fieldConfigs.Add(fieldConfig);
            }
            if (tempModule.Id != Guid.Empty) //编辑时
            {
                //删除模块相关
                SystemOperate.DeleteModuleReferences(tempModule);
            }
            //生成临时模块实体类
            string filePath = Globals.GetWebDir() + "Config\\TempModel";
            //目录不存在则创建
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath += string.Format("\\{0}.code", moduleConfig.TableName);
            //开始生成
            string errMsg = GenerateModel(moduleConfig, fieldConfigs, filePath);
            //代码生成后开始编译
            if (string.IsNullOrEmpty(errMsg))
            {
                string basePath = Globals.GetBinPath();
                //要引用的dll
                string[] referenceAssemblyNames = new string[] 
                { 
                    "System.dll", 
                    "System.Core.dll", 
                    string.Format("{0}Rookey.Frame.EntityBase.dll",basePath), 
                    string.Format("{0}Rookey.Frame.Model.dll", basePath), 
                    string.Format("{0}ServiceStack.Interfaces.dll", basePath) 
                };
                //输出dll
                string outputDll = string.Format("{0}TempModel", basePath);
                if (!Directory.Exists(outputDll))
                {
                    Directory.CreateDirectory(outputDll);
                }
                outputDll += string.Format("\\{0}.dll", moduleConfig.TableName);
                errMsg = DynamicCompiler.CompileFromFile(new string[] { filePath }, referenceAssemblyNames, outputDll);
                //编译完成后生成数据表、模块数据、字段、表单、列表等数据
                if (string.IsNullOrEmpty(errMsg))
                {
                    //取当前临时模块类型
                    Assembly dllAssembly = Assembly.LoadFrom(outputDll);
                    List<Type> modelTypes = dllAssembly.GetTypes().ToList();
                    if (modelTypes.Count == 0)
                        return "临时实体类型不存在，无法初始化！";
                    //开始初始化模块数据
                    errMsg = InitNewModules(modelTypes, true);
                }
            }
            return errMsg;
        }

        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="moduleConfig">模块配置</param>
        /// <param name="fieldConfig">字段配置</param>
        /// <param name="file">生成文件全路径</param>
        /// <returns></returns>
        public static string GenerateModel(ModuleConfigAttribute moduleConfig, List<FieldConfigAttribute> fieldConfig, string file)
        {
            if (moduleConfig == null || fieldConfig == null || fieldConfig.Count == 0)
                return "模块配置或字段配置不存在！";
            string modelClassName = moduleConfig.TableName;
            if (string.IsNullOrWhiteSpace(modelClassName))
                return "实体类名不能为空！";
            try
            {
                FileStream fs = null;
                if (!File.Exists(file)) //文件不存在
                {
                    fs = File.Create(file);
                }
                else
                {
                    fs = new FileStream(file, FileMode.Create, FileAccess.Write);
                }
                #region 引入命名空间
                StringBuilder header = new StringBuilder();
                header.AppendLine("using System;");
                header.AppendLine("using Rookey.Frame.EntityBase;");
                header.AppendLine("using Rookey.Frame.EntityBase.Attr;");
                header.AppendLine("using Rookey.Frame.Model.EnumSpace;");
                header.AppendLine("using ServiceStack.DataAnnotations;");
                byte[] headBytes = Encoding.UTF8.GetBytes(header.ToString());
                fs.Write(headBytes, 0, headBytes.Length);
                #endregion
                StringBuilder code = new StringBuilder();
                code.AppendLine();
                code.AppendLine("namespace Rookey.Frame.TempModel"); //默认命名空间
                code.AppendLine("{");
                #region 开始生成类
                code.AppendLine("\t/// <summary>");
                code.AppendLine("\t/// " + moduleConfig.Name);
                code.AppendLine("\t/// </summary>");
                #region 模块配置代码
                StringBuilder moduleConfigSb = new StringBuilder();
                moduleConfigSb.AppendFormat("\t[ModuleConfig(Name = \"{0}\"", moduleConfig.Name);
                PropertyInfo[] moduleConfigProperties = typeof(ModuleConfigAttribute).GetProperties();
                foreach (PropertyInfo p in moduleConfigProperties)
                {
                    if (p.Name == "Name") continue;
                    if (p.PropertyType == typeof(String))
                    {
                        string value = p.GetValue2(moduleConfig, null).ObjToStr();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            moduleConfigSb.AppendFormat(",{0}=\"{1}\"", p.Name, value);
                        }
                    }
                    else if (p.PropertyType == typeof(Boolean))
                    {
                        string boolStr = p.GetValue2(moduleConfig, null).ObjToBool().ToString().ToLower();
                        moduleConfigSb.AppendFormat(",{0}={1}", p.Name, boolStr);
                    }
                    else if (p.PropertyType == typeof(Int32))
                    {
                        int value = p.GetValue2(moduleConfig, null).ObjToInt();
                        if (value > 0)
                        {
                            moduleConfigSb.AppendFormat(",{0}={1}", p.Name, value);
                        }
                    }
                    else if (p.PropertyType == typeof(ModuleEditModeEnum))
                    {
                        moduleConfigSb.AppendFormat(",{0}=ModuleEditModeEnum.None", p.Name);
                    }
                }
                if (string.IsNullOrWhiteSpace(moduleConfig.StandardJsFolder))
                {
                    moduleConfigSb.AppendFormat(",StandardJsFolder = \"TempModel\"");
                }
                moduleConfigSb.Append(")]");
                code.AppendLine(moduleConfigSb.ToString());
                #endregion
                #region 类代码
                code.AppendLine("\tpublic class " + modelClassName + " : BaseEntity");
                code.AppendLine("\t{");
                #region 字段代码
                foreach (FieldConfigAttribute fc in fieldConfig)
                {
                    if (CommonDefine.BaseEntityFieldsContainId.Contains(fc.FieldName))
                        continue;
                    string fieldName = fc.FieldName;
                    if (string.IsNullOrWhiteSpace(fieldName))
                        continue;
                    StringBuilder fieldSb = new StringBuilder();
                    fieldSb.AppendLine("\t\t/// <summary>");
                    fieldSb.AppendLine("\t\t/// " + fc.Display);
                    fieldSb.AppendLine("\t\t/// </summary>");
                    #region 字段配置代码
                    StringBuilder fieldConfigSb = new StringBuilder();
                    fieldConfigSb.AppendFormat("\t\t[FieldConfig(Display = \"{0}\"", fc.Display);
                    PropertyInfo[] fieldConfigProperties = typeof(FieldConfigAttribute).GetProperties();
                    foreach (PropertyInfo p in fieldConfigProperties)
                    {
                        if (p.Name == "Display") continue;
                        if (p.PropertyType == typeof(String))
                        {
                            string value = p.GetValue2(fc, null).ObjToStr();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                fieldConfigSb.AppendFormat(",{0}=\"{1}\"", p.Name, value);
                            }
                        }
                        else if (p.PropertyType == typeof(Boolean))
                        {
                            if (p.GetValue2(fc, null).ObjToBool())
                            {
                                fieldConfigSb.AppendFormat(",{0}=true", p.Name);
                            }
                        }
                        else if (p.PropertyType == typeof(Int32))
                        {
                            int value = p.GetValue2(fc, null).ObjToInt();
                            if (value > 0)
                            {
                                fieldConfigSb.AppendFormat(",{0}={1}", p.Name, value);
                            }
                        }
                    }
                    fieldConfigSb.Append(")]");
                    fieldSb.AppendLine(fieldConfigSb.ToString());
                    #endregion
                    #region 字段属性代码
                    string fieldTypeString = "string";
                    Sys_Module foreignModule = SystemOperate.GetModuleByName(fc.ForeignModuleName);
                    if (foreignModule != null || fc.ForeignModuleName == moduleConfig.Name) //外键字段
                    {
                        fieldTypeString = "Guid?";
                    }
                    else if (fc.ControlType == (int)ControlTypeEnum.DateBox || fc.ControlType == (int)ControlTypeEnum.DateTimeBox)
                    {
                        fieldTypeString = "DateTime?";
                    }
                    else if (fc.ControlType == (int)ControlTypeEnum.IntegerBox)
                    {
                        fieldTypeString = "int?";
                    }
                    else if (fc.ControlType == (int)ControlTypeEnum.NumberBox)
                    {
                        fieldTypeString = "decimal?";
                    }
                    else if (fc.ControlType == (int)ControlTypeEnum.SingleCheckBox)
                    {
                        fieldTypeString = "bool?";
                    }
                    else
                    {
                        fieldSb.AppendLine("\t\t[StringLength(" + (fc.FieldLen > 0 ? fc.FieldLen : 300) + ")]");
                    }
                    fieldSb.AppendLine("\t\tpublic " + fieldTypeString + " " + fieldName + " { get; set; }");
                    if ((foreignModule != null && !string.IsNullOrWhiteSpace(foreignModule.TitleKey)) ||
                        (fc.ForeignModuleName == moduleConfig.Name && !string.IsNullOrWhiteSpace(moduleConfig.TitleKey)))
                    {
                        //如果是外键字段并且titlekey字段存在
                        //添加外键titleKey字段
                        fieldSb.AppendLine();
                        fieldSb.AppendLine("\t\t/// <summary>");
                        fieldSb.AppendLine("\t\t/// ");
                        fieldSb.AppendLine("\t\t/// </summary>");
                        fieldSb.AppendLine("\t\t[Ignore]");
                        fieldSb.AppendLine("\t\tpublic string " + fieldName.Substring(0, fieldName.Length - 2) + "Name" + " { get; set; }");
                    }
                    #endregion
                    code.AppendLine(fieldSb.ToString());
                }
                #endregion
                code.AppendLine("\t}");
                #endregion
                #endregion
                code.AppendLine("}");
                byte[] codeBytes = Encoding.UTF8.GetBytes(code.ToString());
                fs.Write(codeBytes, 0, codeBytes.Length);
                fs.Close();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 把数据库下的所有数据表生成实体类
        /// </summary>
        /// <param name="dbName">数据库名</param>
        /// <param name="connectString">连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        public static string GenerateModel(string dbName, string connectString = null, DatabaseType? dbType = null)
        {

            return string.Empty;
        }
        #endregion

        #region 数据表、字段操作
        /// <summary>
        /// 修复数据表，参数为空时修复所有
        /// </summary>
        /// <param name="tableNames">表名集合</param>
        /// <param name="isRepairModuleData">是否修复模块相关数据</param>
        /// <returns></returns>
        public static string RepairTables(List<string> tableNames, bool isRepairModuleData = true)
        {
            List<Type> list = BridgeObject.GetAllModelTypes();
            if (tableNames != null && tableNames.Count > 0)
                list = list.Where(x => tableNames.Contains(x.Name)).ToList();
            if (isRepairModuleData)
                return InitNewModules(list, false);
            else
                return CreateOrRepairTable(list);
        }

        /// <summary>
        /// 修复表字段，针对已经存在的模块
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <param name="fieldNames">字段集合</param>
        /// <returns></returns>
        public static string RepairTableFields(Type modelType, List<string> fieldNames)
        {
            if (modelType == null || fieldNames == null || fieldNames.Count == 0)
                return "参数不能为空或字段集合长度为0";
            Sys_Module module = SystemOperate.GetModuleByTableName(modelType.Name);
            Sys_Grid grid = module == null ? null : SystemOperate.GetDefaultGrid(module.Id);
            Sys_Form form = module == null ? null : SystemOperate.GetDefaultForm(module.Id);
            Guid moduleId = module == null ? Guid.Empty : module.Id;
            Guid gridId = grid == null ? Guid.Empty : grid.Id;
            Guid formId = form == null ? Guid.Empty : form.Id;
            string errMsg = string.Empty;
            PropertyInfo[] properties = modelType.GetProperties().Where(x => fieldNames.Contains(x.Name)).ToArray();
            foreach (PropertyInfo p in properties)
            {
                IgnoreAttribute ignoreAttr = (IgnoreAttribute)Attribute.GetCustomAttribute(p, typeof(IgnoreAttribute));
                ReferenceAttribute referenceAttr = (ReferenceAttribute)Attribute.GetCustomAttribute(p, typeof(ReferenceAttribute));
                ReferencesAttribute referencesAttr = (ReferencesAttribute)Attribute.GetCustomAttribute(p, typeof(ReferencesAttribute));
                ForeignKeyAttribute foreignAttr = (ForeignKeyAttribute)Attribute.GetCustomAttribute(p, typeof(ForeignKeyAttribute));
                if (ignoreAttr == null && referenceAttr == null && referencesAttr == null && foreignAttr == null)
                {
                    bool isExists = CommonOperate.ColumnIsExists(modelType, p.Name);
                    if (!isExists) //不存在创建字段列
                    {
                        errMsg = CommonOperate.AddColumn(modelType, p.Name); //添加字段列
                        if (!string.IsNullOrEmpty(errMsg))
                            return errMsg;
                    }
                    if (module == null) continue;
                    FieldConfigAttribute fieldAttr = (FieldConfigAttribute)Attribute.GetCustomAttribute(p, typeof(FieldConfigAttribute));
                    if (ignoreAttr != null && fieldAttr == null) continue;
                    if (fieldAttr == null) fieldAttr = new FieldConfigAttribute();
                    if (string.IsNullOrEmpty(fieldAttr.Display))
                        fieldAttr.Display = p.Name;
                    string foreignModuleName = fieldAttr.ForeignModuleName;
                    //字段信息
                    //检查字段信息是否存在
                    Sys_Field field = CommonOperate.GetEntity<Sys_Field>(x => x.Name == p.Name && x.Sys_ModuleId == moduleId, null, out errMsg);
                    Guid fieldId = Guid.Empty;
                    if (field == null) //字段不存在
                    {
                        field = new Sys_Field();
                        field.CreateDate = DateTime.Now;
                        field.CreateUserId = Guid.Empty;
                        field.CreateUserName = "admin";
                        field.ModifyDate = DateTime.Now;
                        field.ModifyUserId = Guid.Empty;
                        field.ModifyUserName = "admin";
                    }
                    else
                    {
                        fieldId = field.Id;
                    }
                    field.Name = p.Name;
                    field.Display = fieldAttr.Display;
                    field.Sys_ModuleId = moduleId;
                    field.Sys_ModuleName = module.Name;
                    field.ForeignModuleName = foreignModuleName;
                    Guid rs = CommonOperate.OperateRecord<Sys_Field>(field, fieldId == Guid.Empty ? ModelRecordOperateType.Add : ModelRecordOperateType.Edit, out errMsg, null, false);
                    if (fieldId == Guid.Empty && rs != Guid.Empty) fieldId = rs;
                    //视图字段
                    if (grid != null)
                    {
                        if (fieldAttr.IsEnableGrid)
                        {
                            //检查视图字段是否存在
                            Sys_GridField gridField = CommonOperate.GetEntity<Sys_GridField>(x => x.Sys_GridId == gridId && x.Sys_FieldId == fieldId, null, out errMsg);
                            ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                            if (gridField == null) //不存在添加
                            {
                                gridField = new Sys_GridField();
                                gridField.CreateDate = DateTime.Now;
                                gridField.CreateUserId = Guid.Empty;
                                gridField.CreateUserName = "admin";
                                gridField.ModifyDate = DateTime.Now;
                                gridField.ModifyUserId = Guid.Empty;
                                gridField.ModifyUserName = "admin";
                                opType = ModelRecordOperateType.Add;
                            }
                            int w = p.PropertyType == typeof(Boolean) || p.PropertyType == typeof(Boolean?) ||
                                fieldAttr.ControlType == (int)ControlTypeEnum.IntegerBox ||
                                fieldAttr.ControlType == (int)ControlTypeEnum.NumberBox ? 80 : fieldAttr.HeadWidth;
                            if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                            {
                                w = fieldAttr.ControlType == (int)ControlTypeEnum.DateTimeBox ? 150 : 120;
                            }
                            gridField.Sys_GridId = gridId;
                            gridField.Sys_GridName = grid.Name;
                            gridField.Sys_FieldId = fieldId;
                            gridField.Sys_FieldName = field.Name;
                            gridField.Display = field.Display;
                            gridField.Width = w;
                            gridField.Sort = fieldAttr.HeadSort;
                            gridField.IsGroupField = fieldAttr.IsGroupField;
                            gridField.IsAllowSearch = fieldAttr.IsAllowGridSearch;
                            gridField.IsVisible = fieldAttr.IsGridVisible;
                            gridField.IsAllowHide = true;
                            gridField.IsFrozen = fieldAttr.IsFrozen;
                            CommonOperate.OperateRecord<Sys_GridField>(gridField, opType, out errMsg, null, false);
                        }
                        else
                        {
                            CommonOperate.DeleteRecordsByExpression<Sys_GridField>(x => x.Sys_GridId == gridId && x.Sys_FieldId == fieldId, out errMsg);
                        }
                    }
                    if (form != null)
                    {
                        if (fieldAttr.IsEnableForm)
                        {
                            //表单字段
                            //检查表单字段是否存在
                            Sys_FormField formField = CommonOperate.GetEntity<Sys_FormField>(x => x.Sys_FormId == formId && x.Sys_FieldId == fieldId, null, out errMsg);
                            ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                            if (formField == null) //表单字段不存在
                            {
                                formField = new Sys_FormField();
                                formField.CreateDate = DateTime.Now;
                                formField.CreateUserId = Guid.Empty;
                                formField.CreateUserName = "admin";
                                formField.ModifyDate = DateTime.Now;
                                formField.ModifyUserId = Guid.Empty;
                                formField.ModifyUserName = "admin";
                                opType = ModelRecordOperateType.Add;
                            }
                            formField.Sys_FormId = formId;
                            formField.Sys_FormName = form.Name;
                            formField.Sys_FieldId = fieldId;
                            formField.Sys_FieldName = field.Name;
                            formField.Display = field.Display;
                            formField.MinCharLen = fieldAttr.MinCharLen;
                            formField.MaxCharLen = fieldAttr.MaxCharLen;
                            formField.IsRequired = fieldAttr.IsRequired;
                            formField.IsUnique = fieldAttr.IsUnique;
                            formField.IsAllowAdd = fieldAttr.IsAllowAdd;
                            formField.IsAllowEdit = fieldAttr.IsAllowEdit;
                            formField.IsAllowBatchEdit = modelType.BaseType == typeof(BaseSysEntity) ? true : fieldAttr.IsAllowBatchEdit;
                            formField.IsAllowCopy = fieldAttr.IsAllowCopy;
                            formField.GroupName = fieldAttr.GroupName;
                            formField.GroupIcon = fieldAttr.GroupIcon;
                            formField.TabIcon = fieldAttr.TabIcon;
                            formField.TabName = fieldAttr.TabName;
                            formField.ControlType = fieldAttr.ControlType;
                            formField.Width = fieldAttr.ControlWidth;
                            formField.DefaultValue = fieldAttr.DefaultValue;
                            formField.RowNo = fieldAttr.RowNum;
                            formField.ColNo = fieldAttr.ColNum;
                            formField.NullTipText = fieldAttr.NullTipText;
                            formField.IsMultiSelect = fieldAttr.IsMultiSelect;
                            formField.ValueField = fieldAttr.ValueField;
                            formField.TextField = fieldAttr.TextField;
                            formField.UrlOrData = fieldAttr.Url;
                            CommonOperate.OperateRecord<Sys_FormField>(formField, opType, out errMsg, null, false);
                        }
                        else
                        {
                            CommonOperate.DeleteRecordsByExpression<Sys_FormField>(x => x.Sys_FormId == formId && x.Sys_FieldId == fieldId, out errMsg);
                        }
                    }
                }
            }
            return errMsg;
        }

        /// <summary>
        /// 创建或修复数据表
        /// </summary>
        /// <param name="modelType">实体类型对象</param>
        /// <returns></returns>
        public static string CreateOrRepairTable(Type modelType)
        {
            if (modelType == null) return "实体类型对象不能为空！";
            if (ModelConfigHelper.ModelIsViewMode(modelType))
            {
                return string.Empty;
            }
            ModuleConfigAttribute moduleConfig = ((ModuleConfigAttribute)(Attribute.GetCustomAttribute(modelType, typeof(ModuleConfigAttribute))));
            NoModuleAttribute noModuleAttr = ((NoModuleAttribute)(Attribute.GetCustomAttribute(modelType, typeof(NoModuleAttribute))));
            if ((moduleConfig == null && noModuleAttr != null) || (moduleConfig != null && moduleConfig.DataSourceType == (int)ModuleDataSourceType.DbTable)) //数据表模块
            {
                string errMsg = CommonOperate.CreateTable(modelType); //创建表
                if (!string.IsNullOrEmpty(errMsg)) return errMsg;
                PropertyInfo[] properties = modelType.GetProperties();
                foreach (PropertyInfo p in properties)
                {
                    IgnoreAttribute ignoreAttr = (IgnoreAttribute)Attribute.GetCustomAttribute(p, typeof(IgnoreAttribute));
                    ReferenceAttribute referenceAttr = (ReferenceAttribute)Attribute.GetCustomAttribute(p, typeof(ReferenceAttribute));
                    ReferencesAttribute referencesAttr = (ReferencesAttribute)Attribute.GetCustomAttribute(p, typeof(ReferencesAttribute));
                    ForeignKeyAttribute foreignAttr = (ForeignKeyAttribute)Attribute.GetCustomAttribute(p, typeof(ForeignKeyAttribute));
                    if (ignoreAttr == null && referenceAttr == null && referencesAttr == null && foreignAttr == null)
                    {
                        bool isExists = CommonOperate.ColumnIsExists(modelType, p.Name);
                        if (isExists) continue;
                        errMsg = CommonOperate.AddColumn(modelType, p.Name); //添加数据列
                        if (!string.IsNullOrEmpty(errMsg))
                            return errMsg;
                    }
                }
                return errMsg;
            }
            return string.Empty;
        }

        /// <summary>
        /// 创建或修复数据表
        /// </summary>
        /// <param name="modelTypes">实体类型对象集合</param>
        /// <returns></returns>
        public static string CreateOrRepairTable(List<Type> modelTypes)
        {
            if (modelTypes == null || modelTypes.Count == 0)
                return "实体类型集合不能为空！";
            string errMsg = string.Empty;
            foreach (Type type in modelTypes)
            {
                errMsg = CreateOrRepairTable(type);
                if (!string.IsNullOrEmpty(errMsg))
                    return errMsg;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取数据库字段信息
        /// </summary>
        /// <param name="tableName">数据表名</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDbColumnInfo(string tableName, string fieldName)
        {
            string errMsg = string.Empty;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Sys_Module module = SystemOperate.GetModuleByTableName(tableName);
            string connString = string.Empty;
            DatabaseType dbTypeEnum = SystemOperate.GetModuleDbType(module, out connString);
            if (dbTypeEnum == DatabaseType.MsSqlServer)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT  ");
                sb.AppendLine("ColumnType=t.name,");
                sb.AppendLine("Length=c.max_length");
                sb.AppendLine("FROM");
                sb.AppendLine("sys.columns c");
                sb.AppendLine("LEFT OUTER JOIN");
                sb.AppendLine("systypes t");
                sb.AppendLine("on c.system_type_id=t.xtype AND t.name<>'sysname'");
                sb.AppendLine("WHERE");
                sb.AppendLine("OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0");
                sb.AppendLine(string.Format("AND OBJECT_NAME(c.object_id) ='{0}' ", tableName));
                sb.AppendLine(string.Format("AND c.name='{0}'", fieldName));
                DataTable dt = CommonOperate.ExecuteQuery(out errMsg, sb.ToString(), null, connString, dbTypeEnum);
                if (dt != null && dt.Rows.Count > 0)
                {
                    dic.Add("ColumnType", dt.Rows[0]["ColumnType"].ObjToStr());
                    dic.Add("Length", dt.Rows[0]["Length"].ObjToStr());
                }
            }
            return dic;
        }

        #endregion

        #region 系统操作功能
        /// <summary>
        /// 生成导入实体模板
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="fileName">保存文件（绝对路径）</param>
        /// <returns></returns>
        public static string CreateImportModelTemplate(Sys_Module module, UserInfo currUser, string fileName)
        {
            if (module == null) return "指定模块为空";
            if (currUser == null) return "用户未登录，非法操作";
            Sys_Form form = SystemOperate.GetUserForm(currUser.UserId, module.Id);
            List<Sys_FormField> formFields = SystemOperate.GetFormField(form.Id);
            object returnObj = CommonOperate.ExecuteCustomeOperateHandleMethod(module.Id, "GetImportTemplateFields", new object[] { formFields.Select(x => x.Sys_FieldName).ToList(), currUser });
            if (returnObj != null)
            {
                List<string> overFields = returnObj as List<string>;
                if (overFields != null && overFields.Count > 0)
                    formFields = formFields.Where(x => overFields.Contains(x.Sys_FieldName)).ToList();
            }
            if (formFields.Count > 0)
            {
                Type modelType = CommonOperate.GetModelType(module.TableName);
                DataTable dt = new DataTable(module.Name);
                foreach (Sys_FormField field in formFields)
                {
                    if (field.Sys_FieldName == "Id" || field.IsDeleted || field.IsAllowAdd == false)
                        continue;
                    PropertyInfo p = modelType.GetProperty(field.Sys_FieldName);
                    if (p == null) continue;
                    Type tempType = p.PropertyType.IsGenericType ? p.PropertyType.GetGenericArguments()[0] : p.PropertyType;
                    DataColumn dc = new DataColumn(field.Display, tempType);
                    dt.Columns.Add(dc);
                }
                try
                {
                    string dir = System.IO.Path.GetDirectoryName(fileName);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    NPOI_ExcelHelper.RenderToExcel(dt, fileName);
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    return string.Format("模板生成失败，异常：{0}", ex.Message);
                }
            }
            return "无法生成模板，没有可用表单字段！";
        }

        /// <summary>
        /// 用DataTable填充实体集合并进行约束检查
        /// </summary>
        /// <param name="moduleId">模块</param>
        /// <param name="dt">dt</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="action">实体组装后动作</param>
        /// <returns></returns>
        public static IEnumerable FillModels(Guid moduleId, DataTable dt, out string errMsg, Func<object, string> action = null)
        {
            Type modelType = SystemOperate.GetModelType(moduleId);
            if (modelType == null || dt == null || dt.Rows.Count == 0)
            {
                errMsg = "数据表中没有记录！";
                return null;
            }
            Type listType = typeof(List<>).MakeGenericType(new Type[] { modelType });
            object modelList = Activator.CreateInstance(listType);
            StringBuilder sb = new StringBuilder();
            int rowNum = 1;
            foreach (DataRow dr in dt.Rows)
            {
                rowNum++;
                object model = Activator.CreateInstance(modelType);
                foreach (DataColumn dc in dt.Columns)
                {
                    //根据字段名获取字段
                    Sys_Field sysField = SystemOperate.GetFieldInfo(moduleId, dc.ColumnName.Trim());
                    if (sysField == null)
                    {
                        //根据字段显示名称获取字段
                        sysField = SystemOperate.GetFieldByDisplay(moduleId, dc.ColumnName.Trim());
                    }
                    if (sysField == null) continue;
                    PropertyInfo propertyInfo = modelType.GetProperty(sysField.Name);
                    if (propertyInfo == null) continue;
                    Sys_FormField field = SystemOperate.GetNfDefaultFormSingleField(sysField);
                    if (field == null) continue;
                    if (dr[dc] == DBNull.Value) continue;
                    object value = dr[dc];
                    #region 布尔字段处理
                    if (propertyInfo.PropertyType == typeof(Boolean) || propertyInfo.PropertyType == typeof(Boolean?))
                    {
                        if (value.ObjToStr() == "是")
                            value = true;
                        else if (value.ObjToStr() == "否")
                            value = false;
                    }
                    #endregion
                    #region 枚举字段处理
                    else if (SystemOperate.IsEnumField(modelType, sysField.Name)) //枚举字段
                    {
                        Dictionary<string, string> dic = CommonOperate.GetFieldEnumTypeList(moduleId, sysField.Name);
                        if (!dic.ContainsValue(value.ObjToStr())) //当前值不是枚举值
                        {
                            List<string> enumFields = new List<string>();
                            PropertyInfo tempProperty = modelType.GetProperty(string.Format("{0}OfEnum", sysField.Name));
                            if (tempProperty != null)
                            {
                                FieldInfo[] fieldInfos = tempProperty.PropertyType.GetFields();
                                if (fieldInfos != null && fieldInfos.Length > 0)
                                {
                                    enumFields = fieldInfos.Select(x => x.Name).ToList();
                                }
                            }
                            if (dic.ContainsKey(value.ObjToStr())) //枚举描述是否包含当前值
                            {
                                value = dic[value.ObjToStr()]; //取当前枚举描述对应的值
                            }
                            else if (enumFields.Contains(value.ObjToStr())) //枚举字段包含当前值
                            {
                                value = ((int)tempProperty.PropertyType.InvokeMember(value.ObjToStr(), BindingFlags.GetField, null, null, null)).ToString();
                            }
                            else
                            {
                                value = 0;
                                sb.AppendLine(string.Format("第【{0}】行对应字段【{1}】的值非法，请检查该枚举字段的值！", rowNum, dc.ColumnName.Trim()));
                            }
                        }
                    }
                    #endregion
                    #region 字典字段处理
                    else if (SystemOperate.IsDictionaryBindField(moduleId, sysField.Name)) //字典字段
                    {
                        List<Sys_Dictionary> dic = SystemOperate.GetDictionaryData(moduleId, sysField.Name);
                        if (!string.IsNullOrEmpty(value.ObjToStr()))
                        {
                            if (!dic.Select(x => x.Value).Contains(value.ObjToStr())) //当前值不是字典值
                            {
                                if (dic.Select(x => x.Name).Contains(value.ObjToStr())) //字典描述是否包含当前值
                                {
                                    value = dic.Where(x => x.Name == value.ObjToStr()).FirstOrDefault().Value; //取当前字典描述对应的值
                                }
                                else
                                {
                                    value = string.Empty;
                                    sb.AppendLine(string.Format("第【{0}】行对应字段【{1}】的值非法，请检查该字典字段的值！", rowNum, dc.ColumnName.Trim()));
                                }
                            }
                        }
                    }
                    #endregion
                    #region 外键字段处理
                    else if (SystemOperate.IsForeignField(sysField)) //外键字段
                    {
                        if (value.ObjToStr() != string.Empty) //外键字段值非空时
                        {
                            if (value.ObjToGuid() == Guid.Empty) //非正常值
                            {
                                Sys_Module foreignModule = SystemOperate.GetForeignModule(sysField);
                                if (!string.IsNullOrWhiteSpace(foreignModule.TitleKey))
                                {
                                    Type foreignModelType = CommonOperate.GetModelType(foreignModule.Id); //外键模块实体类型
                                    string tempMsg = string.Empty;
                                    string sql = string.Format("SELECT Id FROM {0} WHERE {1}='{2}'", ModelConfigHelper.GetModuleTableName(foreignModelType), foreignModule.TitleKey, value.ObjToStr());
                                    object idObj = CommonOperate.ExecuteScale(out tempMsg, sql);
                                    if (idObj.ObjToGuid() == Guid.Empty) //当前值在外键模块中不存在
                                    {
                                        value = 0;
                                        sb.AppendLine(string.Format("第【{0}】行对应字段【{1}】值在外键模块【{2}】中不存在，请重新检查该字段值！", rowNum, dc.ColumnName.Trim(), foreignModule.Name));
                                    }
                                    else
                                    {
                                        value = idObj;
                                    }
                                }
                                else
                                {
                                    sb.AppendLine(string.Format("第【{0}】行对应字段【{1}】的外键模块【{2}】的TitleKey值非法，无法导入该字段值！", rowNum, dc.ColumnName.Trim(), foreignModule.Name));
                                }
                            }
                        }
                        else //外键字段值为空时且必填时
                        {
                            if (field.IsRequired.HasValue && field.IsRequired.Value) //必填字段
                            {
                                sb.AppendLine(string.Format("第【{0}】行对应字段【{1}】为必填项！", rowNum, dc.ColumnName.Trim()));
                            }
                        }
                    }
                    #endregion
                    propertyInfo.SetValue2(model, TypeUtil.ChangeType(value, propertyInfo.PropertyType), null);
                }
                if (action != null)
                {
                    string msg = action(model);
                    if (!string.IsNullOrEmpty(msg))
                        sb.AppendLine(msg);
                }
                Globals.ExecuteReflectMethod(listType, "Add", new object[] { model }, ref modelList);
            }

            errMsg = sb.ToString();
            return modelList as IEnumerable;
        }

        /// <summary>
        /// 将数据导入到Excel中
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="data">数据集合</param>
        /// <param name="currUser">当前用户</param>
        /// <param name="errMsg">异常信息</param>
        /// <param name="viewId">视图ID</param>
        /// <returns>返回DataTable</returns>
        public static DataTable FillToDataTable(Guid moduleId, object data, UserInfo currUser, out string errMsg, Guid? viewId = null)
        {
            errMsg = string.Empty;
            if (data == null || currUser == null) return null;
            Type modelType = CommonOperate.GetModelType(moduleId);
            List<Sys_GridField> gridFieds = viewId.HasValue && viewId.Value != Guid.Empty ? SystemOperate.GetGridFields(viewId.Value, false) : SystemOperate.GetUserGridFields(currUser.UserId, moduleId);
            if (gridFieds.Count == 0) return null;
            if (BpmOperate.IsEnabledWorkflow(moduleId)) //启用流程
            {
                Sys_GridField flowStatusGf = new Sys_GridField() { Sys_FieldName = "FlowStatus", Display = "状态" };
                gridFieds.Insert(0, flowStatusGf);
            }
            DataTable dt = new DataTable();
            Dictionary<string, string> fieldNames = new Dictionary<string, string>();
            Dictionary<string, Guid> fieldModule = new Dictionary<string, Guid>();
            foreach (Sys_GridField gridField in gridFieds)
            {
                if (dt.Columns.Contains(gridField.Display))
                    continue;
                dt.Columns.Add(gridField.Display);
                string fieldName = gridField.Sys_FieldName;
                if (fieldName == "CreateUserId")
                    fieldName = "CreateUserName";
                else if (fieldName == "ModifyUserId")
                    fieldName = "ModifyUserName";
                fieldNames.Add(fieldName, gridField.Display);
                if (gridField.Sys_FieldId.HasValue)
                {
                    Sys_Field sysField = SystemOperate.GetFieldById(gridField.Sys_FieldId.Value);
                    if (sysField != null && sysField.Sys_ModuleId.HasValue)
                    {
                        fieldModule.Add(fieldName, sysField.Sys_ModuleId.Value);
                    }
                }
            }
            foreach (object obj in (data as IEnumerable))
            {
                DataRow dr = dt.NewRow();
                foreach (string fieldName in fieldNames.Keys)
                {
                    Guid tempModuleId = moduleId;
                    if (fieldModule.ContainsKey(fieldName))
                        tempModuleId = fieldModule[fieldName];
                    string value = SystemOperate.GetFieldDisplayValue(tempModuleId, obj, fieldName);
                    if (fieldName == "FlowStatus")
                    {
                        if (value == string.Empty) value = "0";
                        value = EnumHelper.GetDescription(typeof(WorkFlowStatusEnum), value);
                    }
                    dr[fieldNames[fieldName]] = value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion

        #region 系统初始化

        /// <summary>
        /// 初始化新建模块
        /// </summary>
        /// <param name="modelTypes">模块类型集合</param>
        /// <param name="isCustomer">是否自定义模块</param>
        /// <returns></returns>
        public static string InitNewModules(List<Type> modelTypes, bool isCustomer = true)
        {
            if (modelTypes == null || modelTypes.Count == 0)
                return "实体类型集合不能为空！";
            string errMsg = string.Empty;
            try
            {
                #region 表字段修复
                errMsg = CreateOrRepairTable(modelTypes);
                if (!string.IsNullOrEmpty(errMsg))
                    return errMsg;
                #endregion
                foreach (Type type in modelTypes)
                {
                    #region 添加模块信息
                    PropertyInfo[] properties = type.GetProperties(); //模块属性
                    NoModuleAttribute noModuleAttr = (NoModuleAttribute)(Attribute.GetCustomAttribute(type, typeof(NoModuleAttribute)));
                    //模块配置
                    ModuleConfigAttribute moduleConfig = (ModuleConfigAttribute)(Attribute.GetCustomAttribute(type, typeof(ModuleConfigAttribute)));
                    if (moduleConfig == null && noModuleAttr != null) continue;
                    bool isEnableForm = false; //是否允许有表单
                    bool isEnableGrid = false; //是否允许有网格
                    foreach (PropertyInfo p in properties)
                    {
                        IgnoreAttribute ignoreAttr = (IgnoreAttribute)Attribute.GetCustomAttribute(p, typeof(IgnoreAttribute));
                        ReferenceAttribute referenceAttr = (ReferenceAttribute)Attribute.GetCustomAttribute(p, typeof(ReferenceAttribute));
                        ReferencesAttribute referencesAttr = (ReferencesAttribute)Attribute.GetCustomAttribute(p, typeof(ReferencesAttribute));
                        ForeignKeyAttribute foreignAttr = (ForeignKeyAttribute)Attribute.GetCustomAttribute(p, typeof(ForeignKeyAttribute));
                        NoFieldAttribute nofieldAttr = (NoFieldAttribute)Attribute.GetCustomAttribute(p, typeof(NoFieldAttribute));
                        FieldConfigAttribute fieldAttr = (FieldConfigAttribute)Attribute.GetCustomAttribute(p, typeof(FieldConfigAttribute));
                        if (fieldAttr == null) fieldAttr = new FieldConfigAttribute();
                        if (ignoreAttr == null && referenceAttr == null && referencesAttr == null &&
                            foreignAttr == null && nofieldAttr == null)
                        {
                            if (fieldAttr.IsEnableForm)
                                isEnableForm = true;
                            if (fieldAttr.IsEnableGrid)
                                isEnableGrid = true;
                            if (isEnableForm && isEnableForm)
                                break;
                        }
                    }
                    if (moduleConfig == null) moduleConfig = new ModuleConfigAttribute();
                    if (string.IsNullOrEmpty(moduleConfig.Name)) moduleConfig.Name = type.Name;
                    //先检查该模块是否存在
                    Sys_Module module = SystemOperate.GetModuleByName(moduleConfig.Name);
                    Guid moduleId = Guid.Empty;
                    if (module == null) //模块不存在则添加
                    {
                        module = new Sys_Module();
                        module.CreateDate = DateTime.Now;
                        module.CreateUserId = Guid.Empty;
                        module.CreateUserName = "admin";
                        module.ModifyDate = DateTime.Now;
                        module.ModifyUserId = Guid.Empty;
                        module.ModifyUserName = "admin";
                    }
                    else //模块存在
                    {
                        moduleId = module.Id;
                    }
                    Sys_Module parentModule = SystemOperate.GetModuleByName(moduleConfig.ParentName);
                    module.Name = moduleConfig.Name;
                    module.Display = moduleConfig.Name;
                    module.TableName = type.Name;
                    module.PrimaryKeyFields = moduleConfig.PrimaryKeyFields;
                    module.TitleKey = moduleConfig.TitleKey;
                    module.DataSourceType = moduleConfig.DataSourceType;
                    module.ParentName = moduleConfig.ParentName;
                    module.ParentId = parentModule == null ? null : (Guid?)parentModule.Id;
                    module.Sort = moduleConfig.Sort;
                    module.IsCustomerModule = isCustomer;
                    module.IsAllowAdd = moduleConfig.IsAllowAdd;
                    module.IsAllowEdit = moduleConfig.IsAllowEdit;
                    module.IsAllowDelete = moduleConfig.IsAllowDelete;
                    module.IsAllowCopy = moduleConfig.IsAllowCopy;
                    module.IsAllowImport = moduleConfig.IsAllowImport;
                    module.IsAllowExport = moduleConfig.IsAllowExport;
                    module.IsEnableAttachment = moduleConfig.IsEnableAttachment;
                    module.IsEnabledBatchEdit = moduleConfig.IsEnabledBatchEdit;
                    module.IsEnabledRecycle = moduleConfig.IsEnabledRecycle;
                    module.IsEnabledPrint = moduleConfig.IsEnabledPrint;
                    module.StandardJsFolder = moduleConfig.StandardJsFolder;
                    Guid rs = CommonOperate.OperateRecord<Sys_Module>(module, moduleId == Guid.Empty ? ModelRecordOperateType.Add : ModelRecordOperateType.Edit, out errMsg, null, false);
                    if (moduleId == Guid.Empty && rs != Guid.Empty) moduleId = rs;
                    if (!string.IsNullOrEmpty(errMsg))
                        return errMsg;
                    #endregion
                    #region 添加列表信息
                    if (!isEnableGrid) continue;
                    //添加列表信息
                    //先检查默认列表是否存在
                    Sys_Grid grid = CommonOperate.GetEntity<Sys_Grid>(x => x.Sys_ModuleId == moduleId && x.Name == module.Name + "－默认视图", null, out errMsg);
                    Guid gridId = Guid.Empty;
                    if (grid == null) //默认列表不存在
                    {
                        grid = new Sys_Grid();
                        grid.CreateDate = DateTime.Now;
                        grid.CreateUserId = Guid.Empty;
                        grid.CreateUserName = "admin";
                        grid.ModifyDate = DateTime.Now;
                        grid.ModifyUserId = Guid.Empty;
                        grid.ModifyUserName = "admin";
                    }
                    else
                    {
                        gridId = grid.Id;
                    }
                    grid.Sys_ModuleId = moduleId;
                    grid.Name = module.Name + "－默认视图";
                    grid.GridType = (int)GridTypeEnum.System;
                    grid.AddFilterRow = false;
                    grid.ShowCheckBox = true;
                    grid.IsDefault = true;
                    grid.ButtonLocationOfEnum = ButtonLocationEnum.Top;
                    if (module.Name == "用户管理")
                    {
                        grid.TreeField = "Sys_OrganizationId";
                    }
                    else if (module.Name == "部门管理")
                    {
                        grid.TreeField = "ParentId";
                    }
                    else if (module.Name == "员工管理")
                    {
                        grid.TreeField = "OrgM_DeptId";
                    }
                    else if (module.Name == "岗位管理")
                    {
                        grid.TreeField = "ParentId";
                    }
                    else if (module.Name == "员工岗位")
                    {
                        grid.TreeField = "OrgM_DeptId";
                    }
                    rs = CommonOperate.OperateRecord<Sys_Grid>(grid, gridId == Guid.Empty ? ModelRecordOperateType.Add : ModelRecordOperateType.Edit, out errMsg, null, false);
                    if (gridId == Guid.Empty && rs != Guid.Empty) gridId = rs;
                    if (!string.IsNullOrEmpty(errMsg))
                        return errMsg;
                    #endregion
                    #region 添加列表按钮
                    #region 导入
                    //导入按钮
                    if (moduleConfig.IsAllowImport && !ModelConfigHelper.ModelIsViewMode(type))
                    {
                        //检查按钮是否存在
                        Sys_GridButton gridImportButton = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "导入", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (gridImportButton == null) //导入按钮不存在
                        {
                            gridImportButton = new Sys_GridButton();
                            gridImportButton.CreateDate = DateTime.Now;
                            gridImportButton.CreateUserId = Guid.Empty;
                            gridImportButton.CreateUserName = "admin";
                            gridImportButton.ModifyDate = DateTime.Now;
                            gridImportButton.ModifyUserId = Guid.Empty;
                            gridImportButton.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        gridImportButton.Sys_ModuleId = moduleId;
                        gridImportButton.Sys_ModuleName = module.Name;
                        gridImportButton.ButtonTagId = "btnImportModel";
                        gridImportButton.ButtonText = "导入";
                        gridImportButton.ButtonIcon = "eu-icon-export";
                        gridImportButton.ClickMethod = "ImportModel(this)";
                        gridImportButton.IsSystem = true;
                        gridImportButton.IsValid = true;
                        gridImportButton.Sort = 0;
                        gridImportButton.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        gridImportButton.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(gridImportButton, opType, out errMsg, null, false);
                    }
                    #endregion
                    #region 新增
                    //新增按钮
                    if (moduleConfig.IsAllowAdd && !ModelConfigHelper.ModelIsViewMode(type))
                    {
                        //检查按钮是否存在
                        Sys_GridButton gridAddButton = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "新增", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (gridAddButton == null) //新增按钮不存在
                        {
                            gridAddButton = new Sys_GridButton();
                            gridAddButton.CreateDate = DateTime.Now;
                            gridAddButton.CreateUserId = Guid.Empty;
                            gridAddButton.CreateUserName = "admin";
                            gridAddButton.ModifyDate = DateTime.Now;
                            gridAddButton.ModifyUserId = Guid.Empty;
                            gridAddButton.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        gridAddButton.Sys_ModuleId = moduleId;
                        gridAddButton.Sys_ModuleName = module.Name;
                        gridAddButton.ButtonTagId = "btnAdd";
                        gridAddButton.ButtonText = "新增";
                        gridAddButton.ButtonIcon = "eu-p2-icon-add_other";
                        gridAddButton.ClickMethod = "Add(this)";
                        gridAddButton.IsSystem = true;
                        gridAddButton.IsValid = true;
                        gridAddButton.Sort = 1;
                        gridAddButton.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        gridAddButton.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(gridAddButton, opType, out errMsg, null, false);
                    }
                    #endregion
                    #region 编辑
                    //编辑按钮
                    if (moduleConfig.IsAllowEdit && !ModelConfigHelper.ModelIsViewMode(type))
                    {
                        //检查编辑按钮是否存在
                        Sys_GridButton gridEditButton = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "编辑", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (gridEditButton == null)
                        {
                            gridEditButton = new Sys_GridButton();
                            gridEditButton.CreateDate = DateTime.Now;
                            gridEditButton.CreateUserId = Guid.Empty;
                            gridEditButton.CreateUserName = "admin";
                            gridEditButton.ModifyDate = DateTime.Now;
                            gridEditButton.ModifyUserId = Guid.Empty;
                            gridEditButton.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        gridEditButton.Sys_ModuleId = moduleId;
                        gridEditButton.Sys_ModuleName = module.Name;
                        gridEditButton.ButtonTagId = "btnEdit";
                        gridEditButton.ButtonText = "编辑";
                        gridEditButton.IsValid = true;
                        gridEditButton.ButtonIcon = "eu-icon-edit";
                        gridEditButton.ClickMethod = "Edit(this)";
                        gridEditButton.IsSystem = true;
                        gridEditButton.Sort = 2;
                        gridEditButton.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        gridEditButton.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(gridEditButton, opType, out errMsg, null, false);
                    }
                    #endregion
                    #region 删除
                    //删除按钮
                    if (moduleConfig.IsAllowDelete && !ModelConfigHelper.ModelIsViewMode(type))
                    {
                        //检查删除按钮是否存在
                        Sys_GridButton gridDeleteButton = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "删除", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (gridDeleteButton == null)
                        {
                            gridDeleteButton = new Sys_GridButton();
                            gridDeleteButton.CreateDate = DateTime.Now;
                            gridDeleteButton.CreateUserId = Guid.Empty;
                            gridDeleteButton.CreateUserName = "admin";
                            gridDeleteButton.ModifyDate = DateTime.Now;
                            gridDeleteButton.ModifyUserId = Guid.Empty;
                            gridDeleteButton.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        gridDeleteButton.Sys_ModuleId = moduleId;
                        gridDeleteButton.Sys_ModuleName = module.Name;
                        gridDeleteButton.ButtonTagId = "btnDelete";
                        gridDeleteButton.IsValid = true;
                        gridDeleteButton.ButtonText = "删除";
                        gridDeleteButton.ButtonIcon = "eu-p2-icon-delete2";
                        gridDeleteButton.ClickMethod = "Delete(this)";
                        gridDeleteButton.IsSystem = true;
                        gridDeleteButton.Sort = 3;
                        gridDeleteButton.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        gridDeleteButton.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(gridDeleteButton, opType, out errMsg, null, false);
                    }
                    #endregion
                    #region 查看
                    if (isEnableForm)
                    {
                        //查看按钮
                        //检查查看按钮是否存在
                        Sys_GridButton gridViewButton = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "查看", null, out errMsg);
                        ModelRecordOperateType opViewType = ModelRecordOperateType.Edit;
                        if (gridViewButton == null)
                        {
                            gridViewButton = new Sys_GridButton();
                            gridViewButton.CreateDate = DateTime.Now;
                            gridViewButton.CreateUserId = Guid.Empty;
                            gridViewButton.CreateUserName = "admin";
                            gridViewButton.ModifyDate = DateTime.Now;
                            gridViewButton.ModifyUserId = Guid.Empty;
                            gridViewButton.ModifyUserName = "admin";
                            opViewType = ModelRecordOperateType.Add;
                        }
                        gridViewButton.Sys_ModuleId = moduleId;
                        gridViewButton.Sys_ModuleName = module.Name;
                        gridViewButton.ButtonTagId = "btnViewRecord";
                        gridViewButton.IsValid = true;
                        gridViewButton.ButtonText = "查看";
                        gridViewButton.ButtonIcon = "eu-icon-search";
                        gridViewButton.ClickMethod = "ViewRecord(this)";
                        gridViewButton.IsSystem = true;
                        gridViewButton.Sort = 4;
                        gridViewButton.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        gridViewButton.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(gridViewButton, opViewType, out errMsg, null, false);
                    }
                    #endregion
                    #region 特殊
                    if (moduleConfig.Name == "模块管理")
                    {
                        #region 重新生成模块
                        Sys_GridButton reCreateBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "重新生成模块", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (reCreateBtn == null)
                        {
                            reCreateBtn = new Sys_GridButton();
                            reCreateBtn.CreateDate = DateTime.Now;
                            reCreateBtn.CreateUserId = Guid.Empty;
                            reCreateBtn.CreateUserName = "admin";
                            reCreateBtn.ModifyDate = DateTime.Now;
                            reCreateBtn.ModifyUserId = Guid.Empty;
                            reCreateBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        reCreateBtn.Sys_ModuleId = moduleId;
                        reCreateBtn.Sys_ModuleName = module.Name;
                        reCreateBtn.ButtonTagId = "btnReCreateModule";
                        reCreateBtn.IsValid = true;
                        reCreateBtn.ButtonText = "重新生成模块";
                        reCreateBtn.ButtonIcon = "eu-p2-icon-database_copy";
                        reCreateBtn.ClickMethod = "ReCreateModule()";
                        reCreateBtn.IsSystem = true;
                        reCreateBtn.Sort = 10;
                        reCreateBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        reCreateBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(reCreateBtn, opType, out errMsg, null, false);
                        #endregion
                    }
                    else if (moduleConfig.Name == "视图按钮")
                    {
                        #region 添加常用按钮
                        Sys_GridButton addCommonBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "添加常用按钮", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (addCommonBtn == null)
                        {
                            addCommonBtn = new Sys_GridButton();
                            addCommonBtn.CreateDate = DateTime.Now;
                            addCommonBtn.CreateUserId = Guid.Empty;
                            addCommonBtn.CreateUserName = "admin";
                            addCommonBtn.ModifyDate = DateTime.Now;
                            addCommonBtn.ModifyUserId = Guid.Empty;
                            addCommonBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        addCommonBtn.Sys_ModuleId = moduleId;
                        addCommonBtn.Sys_ModuleName = module.Name;
                        addCommonBtn.ButtonTagId = "btnAddCommonBtn";
                        addCommonBtn.IsValid = true;
                        addCommonBtn.ButtonText = "添加常用按钮";
                        addCommonBtn.ButtonIcon = "eu-p2-icon-add_other";
                        addCommonBtn.ClickMethod = "AddCommonBtn()";
                        addCommonBtn.IsSystem = true;
                        addCommonBtn.Sort = 20;
                        addCommonBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        addCommonBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(addCommonBtn, opType, out errMsg, null, false);
                        #endregion
                    }
                    else if (moduleConfig.Name == "用户管理")
                    {
                        #region 设置角色
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (!ModelConfigHelper.ModelIsViewMode(typeof(Sys_UserRole)))
                        {
                            Sys_GridButton setRoleBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "设置角色", null, out errMsg);
                            if (setRoleBtn == null)
                            {
                                setRoleBtn = new Sys_GridButton();
                                setRoleBtn.CreateDate = DateTime.Now;
                                setRoleBtn.CreateUserId = Guid.Empty;
                                setRoleBtn.CreateUserName = "admin";
                                setRoleBtn.ModifyDate = DateTime.Now;
                                setRoleBtn.ModifyUserId = Guid.Empty;
                                setRoleBtn.ModifyUserName = "admin";
                                opType = ModelRecordOperateType.Add;
                            }
                            setRoleBtn.Sys_ModuleId = moduleId;
                            setRoleBtn.Sys_ModuleName = module.Name;
                            setRoleBtn.ButtonTagId = "btnSetRole";
                            setRoleBtn.IsValid = true;
                            setRoleBtn.ButtonText = "设置角色";
                            setRoleBtn.ButtonIcon = "eu-icon-cog";
                            setRoleBtn.ClickMethod = "SetUserRole(this)";
                            setRoleBtn.IsSystem = true;
                            setRoleBtn.Sort = 4;
                            setRoleBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                            setRoleBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                            CommonOperate.OperateRecord<Sys_GridButton>(setRoleBtn, opType, out errMsg, null, false);
                        }
                        #endregion
                        #region 设置权限
                        Sys_GridButton setPowerBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "设置权限", null, out errMsg);
                        opType = ModelRecordOperateType.Edit;
                        if (setPowerBtn == null)
                        {
                            setPowerBtn = new Sys_GridButton();
                            setPowerBtn.CreateDate = DateTime.Now;
                            setPowerBtn.CreateUserId = Guid.Empty;
                            setPowerBtn.CreateUserName = "admin";
                            setPowerBtn.ModifyDate = DateTime.Now;
                            setPowerBtn.ModifyUserId = Guid.Empty;
                            setPowerBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        setPowerBtn.Sys_ModuleId = moduleId;
                        setPowerBtn.Sys_ModuleName = module.Name;
                        setPowerBtn.ButtonTagId = "btnSetPermission";
                        setPowerBtn.IsValid = true;
                        setPowerBtn.ButtonText = "设置权限";
                        setPowerBtn.ButtonIcon = "eu-icon-cog";
                        setPowerBtn.ClickMethod = "SetPermission(this)";
                        setPowerBtn.IsSystem = true;
                        setPowerBtn.Sort = 5;
                        setPowerBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        setPowerBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(setPowerBtn, opType, out errMsg, null, false);
                        #endregion
                    }
                    else if (moduleConfig.Name == "角色管理")
                    {
                        #region 关联表单
                        Sys_GridButton setFormBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "关联表单", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (setFormBtn == null)
                        {
                            setFormBtn = new Sys_GridButton();
                            setFormBtn.CreateDate = DateTime.Now;
                            setFormBtn.CreateUserId = Guid.Empty;
                            setFormBtn.CreateUserName = "admin";
                            setFormBtn.ModifyDate = DateTime.Now;
                            setFormBtn.ModifyUserId = Guid.Empty;
                            setFormBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        setFormBtn.Sys_ModuleId = moduleId;
                        setFormBtn.Sys_ModuleName = module.Name;
                        setFormBtn.ButtonTagId = "btnSetForm";
                        setFormBtn.IsValid = true;
                        setFormBtn.ButtonText = "关联表单";
                        setFormBtn.ButtonIcon = "eu-icon-cog";
                        setFormBtn.ClickMethod = "SetRoleForm(this)";
                        setFormBtn.IsSystem = true;
                        setFormBtn.Sort = 4;
                        setFormBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        setFormBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(setFormBtn, opType, out errMsg, null, false);
                        #endregion
                        #region 设置权限
                        Sys_GridButton setPowerBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "设置权限", null, out errMsg);
                        opType = ModelRecordOperateType.Edit;
                        if (setPowerBtn == null)
                        {
                            setPowerBtn = new Sys_GridButton();
                            setPowerBtn.CreateDate = DateTime.Now;
                            setPowerBtn.CreateUserId = Guid.Empty;
                            setPowerBtn.CreateUserName = "admin";
                            setPowerBtn.ModifyDate = DateTime.Now;
                            setPowerBtn.ModifyUserId = Guid.Empty;
                            setPowerBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        setPowerBtn.Sys_ModuleId = moduleId;
                        setPowerBtn.Sys_ModuleName = module.Name;
                        setPowerBtn.ButtonTagId = "btnSetPermission";
                        setPowerBtn.IsValid = true;
                        setPowerBtn.ButtonText = "设置权限";
                        setPowerBtn.ButtonIcon = "eu-icon-cog";
                        setPowerBtn.ClickMethod = "SetPermission(this)";
                        setPowerBtn.IsSystem = true;
                        setPowerBtn.Sort = 5;
                        setPowerBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        setPowerBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(setPowerBtn, opType, out errMsg, null, false);
                        #endregion
                    }
                    else if (moduleConfig.Name == "视图字段")
                    {
                        #region 刷新格式化
                        Sys_GridButton refreshformatBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "刷新格式化", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (refreshformatBtn == null)
                        {
                            refreshformatBtn = new Sys_GridButton();
                            refreshformatBtn.CreateDate = DateTime.Now;
                            refreshformatBtn.CreateUserId = Guid.Empty;
                            refreshformatBtn.CreateUserName = "admin";
                            refreshformatBtn.ModifyDate = DateTime.Now;
                            refreshformatBtn.ModifyUserId = Guid.Empty;
                            refreshformatBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        refreshformatBtn.Sys_ModuleId = moduleId;
                        refreshformatBtn.Sys_ModuleName = module.Name;
                        refreshformatBtn.ButtonTagId = "btnRefreshFormat";
                        refreshformatBtn.IsValid = true;
                        refreshformatBtn.ButtonText = "刷新格式化";
                        refreshformatBtn.ButtonIcon = "eu-icon-reload";
                        refreshformatBtn.ClickMethod = "RefreshFieldFormat(this)";
                        refreshformatBtn.IsSystem = false;
                        refreshformatBtn.Sort = 5;
                        refreshformatBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        refreshformatBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(refreshformatBtn, opType, out errMsg, null, false);
                        #endregion
                    }
                    else if (moduleConfig.Name == "缓存配置")
                    {
                        #region 刷新模块缓存
                        Sys_GridButton refreshModuleCacheBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "刷新模块缓存", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        if (refreshModuleCacheBtn == null)
                        {
                            refreshModuleCacheBtn = new Sys_GridButton();
                            refreshModuleCacheBtn.CreateDate = DateTime.Now;
                            refreshModuleCacheBtn.CreateUserId = Guid.Empty;
                            refreshModuleCacheBtn.CreateUserName = "admin";
                            refreshModuleCacheBtn.ModifyDate = DateTime.Now;
                            refreshModuleCacheBtn.ModifyUserId = Guid.Empty;
                            refreshModuleCacheBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        refreshModuleCacheBtn.Sys_ModuleId = moduleId;
                        refreshModuleCacheBtn.Sys_ModuleName = module.Name;
                        refreshModuleCacheBtn.ButtonTagId = "btnRefreshModuleCache";
                        refreshModuleCacheBtn.IsValid = true;
                        refreshModuleCacheBtn.ButtonText = "刷新模块缓存";
                        refreshModuleCacheBtn.ButtonIcon = "eu-icon-reload";
                        refreshModuleCacheBtn.ClickMethod = "RefreshModuleCache(this)";
                        refreshModuleCacheBtn.IsSystem = false;
                        refreshModuleCacheBtn.Sort = 5;
                        refreshModuleCacheBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        refreshModuleCacheBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(refreshModuleCacheBtn, opType, out errMsg, null, false);
                        #endregion
                        #region 刷新所有缓存
                        Sys_GridButton refreshAllCacheBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "刷新所有缓存", null, out errMsg);
                        opType = ModelRecordOperateType.Edit;
                        if (refreshAllCacheBtn == null)
                        {
                            refreshAllCacheBtn = new Sys_GridButton();
                            refreshAllCacheBtn.CreateDate = DateTime.Now;
                            refreshAllCacheBtn.CreateUserId = Guid.Empty;
                            refreshAllCacheBtn.CreateUserName = "admin";
                            refreshAllCacheBtn.ModifyDate = DateTime.Now;
                            refreshAllCacheBtn.ModifyUserId = Guid.Empty;
                            refreshAllCacheBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        refreshAllCacheBtn.Sys_ModuleId = moduleId;
                        refreshAllCacheBtn.Sys_ModuleName = module.Name;
                        refreshAllCacheBtn.ButtonTagId = "btnRefreshAllCache";
                        refreshAllCacheBtn.IsValid = true;
                        refreshAllCacheBtn.ButtonText = "刷新所有缓存";
                        refreshAllCacheBtn.ButtonIcon = "eu-icon-reload";
                        refreshAllCacheBtn.ClickMethod = "RefreshAllCache(this)";
                        refreshAllCacheBtn.IsSystem = false;
                        refreshAllCacheBtn.Sort = 6;
                        refreshAllCacheBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        refreshAllCacheBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(refreshAllCacheBtn, opType, out errMsg, null, false);
                        #endregion
                    }
                    #endregion
                    #region 辅助按钮
                    //包括复制、批量编辑、导出、打印等功能
                    if (moduleConfig.IsAllowCopy || moduleConfig.IsEnabledBatchEdit || moduleConfig.IsAllowExport || moduleConfig.IsEnabledPrint)
                    {
                        #region 辅助
                        Sys_GridButton auxiliaryBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "辅助", null, out errMsg);
                        ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                        Guid auxiliaryBtnId = Guid.Empty;
                        if (auxiliaryBtn == null)
                        {
                            auxiliaryBtn = new Sys_GridButton();
                            auxiliaryBtn.CreateDate = DateTime.Now;
                            auxiliaryBtn.CreateUserId = Guid.Empty;
                            auxiliaryBtn.CreateUserName = "admin";
                            auxiliaryBtn.ModifyDate = DateTime.Now;
                            auxiliaryBtn.ModifyUserId = Guid.Empty;
                            auxiliaryBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        else
                        {
                            auxiliaryBtnId = auxiliaryBtn.Id;
                        }
                        auxiliaryBtn.Sys_ModuleId = moduleId;
                        auxiliaryBtn.Sys_ModuleName = module.Name;
                        auxiliaryBtn.ButtonTagId = "btnAuxiliary";
                        auxiliaryBtn.IsValid = true;
                        auxiliaryBtn.ButtonText = "辅助";
                        auxiliaryBtn.ButtonIcon = "eu-icon-cog";
                        auxiliaryBtn.ClickMethod = "";
                        auxiliaryBtn.IsSystem = true;
                        auxiliaryBtn.Sort = 10;
                        auxiliaryBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.FileMenuButton;
                        auxiliaryBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        rs = CommonOperate.OperateRecord<Sys_GridButton>(auxiliaryBtn, opType, out errMsg, null, false);
                        if (auxiliaryBtnId == Guid.Empty && rs != Guid.Empty) auxiliaryBtnId = rs;
                        #endregion
                        #region 复制
                        if (!ModelConfigHelper.ModelIsViewMode(type))
                        {
                            Sys_GridButton copyBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "复制", null, out errMsg);
                            opType = ModelRecordOperateType.Edit;
                            if (copyBtn == null)
                            {
                                copyBtn = new Sys_GridButton();
                                copyBtn.CreateDate = DateTime.Now;
                                copyBtn.CreateUserId = Guid.Empty;
                                copyBtn.CreateUserName = "admin";
                                copyBtn.ModifyDate = DateTime.Now;
                                copyBtn.ModifyUserId = Guid.Empty;
                                copyBtn.ModifyUserName = "admin";
                                opType = ModelRecordOperateType.Add;
                            }
                            copyBtn.Sys_ModuleId = moduleId;
                            copyBtn.Sys_ModuleName = module.Name;
                            copyBtn.ButtonTagId = "btnCopy";
                            copyBtn.IsValid = true;
                            copyBtn.ButtonText = "复制";
                            copyBtn.ButtonIcon = "eu-icon-copy";
                            copyBtn.ClickMethod = "Copy(this)";
                            copyBtn.IsSystem = true;
                            copyBtn.ParentId = auxiliaryBtnId;
                            copyBtn.Sort = 11;
                            copyBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                            copyBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                            CommonOperate.OperateRecord<Sys_GridButton>(copyBtn, opType, out errMsg, null, false);
                        }
                        #endregion
                        #region 批量编辑
                        if (!ModelConfigHelper.ModelIsViewMode(type))
                        {
                            Sys_GridButton batchEditBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "批量编辑", null, out errMsg);
                            opType = ModelRecordOperateType.Edit;
                            if (batchEditBtn == null)
                            {
                                batchEditBtn = new Sys_GridButton();
                                batchEditBtn.CreateDate = DateTime.Now;
                                batchEditBtn.CreateUserId = Guid.Empty;
                                batchEditBtn.CreateUserName = "admin";
                                batchEditBtn.ModifyDate = DateTime.Now;
                                batchEditBtn.ModifyUserId = Guid.Empty;
                                batchEditBtn.ModifyUserName = "admin";
                                opType = ModelRecordOperateType.Add;
                            }
                            batchEditBtn.Sys_ModuleId = moduleId;
                            batchEditBtn.Sys_ModuleName = module.Name;
                            batchEditBtn.ButtonTagId = "btnBatchEdit";
                            batchEditBtn.IsValid = true;
                            batchEditBtn.ButtonText = "批量编辑";
                            batchEditBtn.ButtonIcon = "eu-icon-edit";
                            batchEditBtn.ClickMethod = "BatchEdit(this)";
                            batchEditBtn.IsSystem = true;
                            batchEditBtn.ParentId = auxiliaryBtnId;
                            batchEditBtn.Sort = 12;
                            batchEditBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                            batchEditBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                            CommonOperate.OperateRecord<Sys_GridButton>(batchEditBtn, opType, out errMsg, null, false);
                        }
                        #endregion
                        #region 导出
                        Sys_GridButton exportBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "导出", null, out errMsg);
                        opType = ModelRecordOperateType.Edit;
                        if (exportBtn == null)
                        {
                            exportBtn = new Sys_GridButton();
                            exportBtn.CreateDate = DateTime.Now;
                            exportBtn.CreateUserId = Guid.Empty;
                            exportBtn.CreateUserName = "admin";
                            exportBtn.ModifyDate = DateTime.Now;
                            exportBtn.ModifyUserId = Guid.Empty;
                            exportBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        exportBtn.Sys_ModuleId = moduleId;
                        exportBtn.Sys_ModuleName = module.Name;
                        exportBtn.ButtonTagId = "btnExportModel";
                        exportBtn.IsValid = true;
                        exportBtn.ButtonText = "导出";
                        exportBtn.ButtonIcon = "eu-icon-export";
                        exportBtn.ClickMethod = "ExportModel(this)";
                        exportBtn.IsSystem = true;
                        exportBtn.ParentId = auxiliaryBtnId;
                        exportBtn.Sort = 13;
                        exportBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        exportBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(exportBtn, opType, out errMsg, null, false);
                        #endregion
                        #region 打印
                        Sys_GridButton printBtn = CommonOperate.GetEntity<Sys_GridButton>(x => x.Sys_ModuleId == moduleId && x.ButtonText == "打印", null, out errMsg);
                        opType = ModelRecordOperateType.Edit;
                        if (printBtn == null)
                        {
                            printBtn = new Sys_GridButton();
                            printBtn.CreateDate = DateTime.Now;
                            printBtn.CreateUserId = Guid.Empty;
                            printBtn.CreateUserName = "admin";
                            printBtn.ModifyDate = DateTime.Now;
                            printBtn.ModifyUserId = Guid.Empty;
                            printBtn.ModifyUserName = "admin";
                            opType = ModelRecordOperateType.Add;
                        }
                        printBtn.Sys_ModuleId = moduleId;
                        printBtn.Sys_ModuleName = module.Name;
                        printBtn.ButtonTagId = "btnPrintModel";
                        printBtn.IsValid = true;
                        printBtn.ButtonText = "打印";
                        printBtn.ButtonIcon = "eu-icon-print";
                        printBtn.ClickMethod = "PrintModel(this)";
                        printBtn.IsSystem = true;
                        printBtn.ParentId = auxiliaryBtnId;
                        printBtn.Sort = 14;
                        printBtn.OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton;
                        printBtn.GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar;
                        CommonOperate.OperateRecord<Sys_GridButton>(printBtn, opType, out errMsg, null, false);
                        #endregion
                    }
                    #endregion
                    #endregion
                    #region 添加表单信息
                    //编辑表单
                    //检查编辑表单是否存在
                    Sys_Form form = CommonOperate.GetEntity<Sys_Form>(x => x.Name == module.Name + "默认表单" && x.Sys_ModuleId == moduleId, null, out errMsg);
                    Guid formId = Guid.Empty;
                    if (isEnableForm)
                    {
                        if (form == null) //默认表单不存在
                        {
                            form = new Sys_Form();
                            form.CreateDate = DateTime.Now;
                            form.CreateUserId = Guid.Empty;
                            form.CreateUserName = "admin";
                            form.ModifyDate = DateTime.Now;
                            form.ModifyUserId = Guid.Empty;
                            form.ModifyUserName = "admin";
                        }
                        else //默认表单存在
                        {
                            formId = form.Id;
                        }
                        form.Name = module.Name + "默认表单";
                        form.Sys_ModuleId = moduleId;
                        form.Sys_ModuleName = module.Name;
                        form.IsDefault = true;
                        form.ModuleEditMode = moduleConfig.ModuleEditMode;
                        rs = CommonOperate.OperateRecord<Sys_Form>(form, formId == Guid.Empty ? ModelRecordOperateType.Add : ModelRecordOperateType.Edit, out errMsg, null, false);
                        if (formId == Guid.Empty && rs != Guid.Empty) formId = rs;
                    }
                    #endregion
                    #region 添加字段信息
                    Dictionary<string, FieldConfigAttribute> dic = new Dictionary<string, FieldConfigAttribute>();
                    foreach (PropertyInfo p in properties)
                    {
                        IgnoreAttribute ignoreAttr = (IgnoreAttribute)Attribute.GetCustomAttribute(p, typeof(IgnoreAttribute));
                        ReferenceAttribute referenceAttr = (ReferenceAttribute)Attribute.GetCustomAttribute(p, typeof(ReferenceAttribute));
                        ReferencesAttribute referencesAttr = (ReferencesAttribute)Attribute.GetCustomAttribute(p, typeof(ReferencesAttribute));
                        ForeignKeyAttribute foreignAttr = (ForeignKeyAttribute)Attribute.GetCustomAttribute(p, typeof(ForeignKeyAttribute));
                        NoFieldAttribute nofieldAttr = (NoFieldAttribute)Attribute.GetCustomAttribute(p, typeof(NoFieldAttribute));
                        if (referenceAttr != null || referencesAttr != null || foreignAttr != null || nofieldAttr != null)
                            continue;
                        FieldConfigAttribute fieldAttr = (FieldConfigAttribute)Attribute.GetCustomAttribute(p, typeof(FieldConfigAttribute));
                        if (ignoreAttr != null && fieldAttr == null) continue;
                        if (fieldAttr == null) fieldAttr = new FieldConfigAttribute();
                        if (string.IsNullOrEmpty(fieldAttr.Display))
                            fieldAttr.Display = p.Name;
                        dic.Add(p.Name, fieldAttr);
                        string foreignModuleName = fieldAttr.ForeignModuleName;
                        //字段信息
                        //检查字段信息是否存在
                        Sys_Field field = CommonOperate.GetEntity<Sys_Field>(x => x.Name == p.Name && x.Sys_ModuleId == moduleId, null, out errMsg);
                        Guid fieldId = Guid.Empty;
                        if (field == null) //字段不存在
                        {
                            field = new Sys_Field();
                            field.CreateDate = DateTime.Now;
                            field.CreateUserId = Guid.Empty;
                            field.CreateUserName = "admin";
                            field.ModifyDate = DateTime.Now;
                            field.ModifyUserId = Guid.Empty;
                            field.ModifyUserName = "admin";
                        }
                        else
                        {
                            fieldId = field.Id;
                        }
                        field.Name = p.Name;
                        field.Display = fieldAttr.Display;
                        field.Sys_ModuleId = moduleId;
                        field.Sys_ModuleName = module.Name;
                        field.ForeignModuleName = foreignModuleName;
                        rs = CommonOperate.OperateRecord<Sys_Field>(field, fieldId == Guid.Empty ? ModelRecordOperateType.Add : ModelRecordOperateType.Edit, out errMsg, null, false);
                        if (fieldId == Guid.Empty && rs != Guid.Empty) fieldId = rs;
                        //视图字段
                        if (fieldAttr.IsEnableGrid)
                        {
                            //检查视图字段是否存在
                            Sys_GridField gridField = CommonOperate.GetEntity<Sys_GridField>(x => x.Sys_GridId == gridId && x.Sys_FieldId == fieldId, null, out errMsg);
                            ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                            if (gridField == null) //不存在添加
                            {
                                gridField = new Sys_GridField();
                                gridField.CreateDate = DateTime.Now;
                                gridField.CreateUserId = Guid.Empty;
                                gridField.CreateUserName = "admin";
                                gridField.ModifyDate = DateTime.Now;
                                gridField.ModifyUserId = Guid.Empty;
                                gridField.ModifyUserName = "admin";
                                opType = ModelRecordOperateType.Add;
                            }
                            int w = p.PropertyType == typeof(Boolean) || p.PropertyType == typeof(Boolean?) ||
                                fieldAttr.ControlType == (int)ControlTypeEnum.IntegerBox ||
                                fieldAttr.ControlType == (int)ControlTypeEnum.NumberBox ? 80 : fieldAttr.HeadWidth;
                            if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                            {
                                w = fieldAttr.ControlType == (int)ControlTypeEnum.DateTimeBox ? 150 : 120;
                            }
                            gridField.Sys_GridId = gridId;
                            gridField.Sys_GridName = grid.Name;
                            gridField.Sys_FieldId = fieldId;
                            gridField.Sys_FieldName = field.Name;
                            gridField.Display = field.Display;
                            gridField.Width = w;
                            gridField.Sort = fieldAttr.HeadSort;
                            gridField.IsGroupField = fieldAttr.IsGroupField;
                            gridField.IsAllowSearch = fieldAttr.IsAllowGridSearch;
                            gridField.IsVisible = fieldAttr.IsGridVisible;
                            gridField.IsAllowHide = true;
                            gridField.IsFrozen = fieldAttr.IsFrozen;
                            CommonOperate.OperateRecord<Sys_GridField>(gridField, opType, out errMsg, null, false);
                        }
                        else
                        {
                            CommonOperate.DeleteRecordsByExpression<Sys_GridField>(x => x.Sys_GridId == gridId && x.Sys_FieldId == fieldId, out errMsg);
                        }
                        if (fieldAttr.IsEnableForm)
                        {
                            //表单字段
                            //检查表单字段是否存在
                            Sys_FormField formField = CommonOperate.GetEntity<Sys_FormField>(x => x.Sys_FormId == formId && x.Sys_FieldId == fieldId, null, out errMsg);
                            ModelRecordOperateType opType = ModelRecordOperateType.Edit;
                            if (formField == null) //表单字段不存在
                            {
                                formField = new Sys_FormField();
                                formField.CreateDate = DateTime.Now;
                                formField.CreateUserId = Guid.Empty;
                                formField.CreateUserName = "admin";
                                formField.ModifyDate = DateTime.Now;
                                formField.ModifyUserId = Guid.Empty;
                                formField.ModifyUserName = "admin";
                                opType = ModelRecordOperateType.Add;
                            }
                            formField.Sys_FormId = formId;
                            formField.Sys_FormName = form.Name;
                            formField.Sys_FieldId = fieldId;
                            formField.Sys_FieldName = field.Name;
                            formField.Display = field.Display;
                            formField.MinCharLen = fieldAttr.MinCharLen;
                            formField.MaxCharLen = fieldAttr.MaxCharLen;
                            formField.IsRequired = fieldAttr.IsRequired;
                            formField.IsUnique = fieldAttr.IsUnique;
                            formField.IsAllowAdd = fieldAttr.IsAllowAdd;
                            formField.IsAllowEdit = fieldAttr.IsAllowEdit;
                            formField.IsAllowBatchEdit = type.BaseType == typeof(BaseSysEntity) ? true : fieldAttr.IsAllowBatchEdit;
                            formField.IsAllowCopy = fieldAttr.IsAllowCopy;
                            formField.GroupName = fieldAttr.GroupName;
                            formField.GroupIcon = fieldAttr.GroupIcon;
                            formField.TabIcon = fieldAttr.TabIcon;
                            formField.TabName = fieldAttr.TabName;
                            formField.ControlType = fieldAttr.ControlType;
                            formField.Width = fieldAttr.ControlWidth;
                            formField.DefaultValue = fieldAttr.DefaultValue;
                            formField.RowNo = fieldAttr.RowNum;
                            formField.ColNo = fieldAttr.ColNum;
                            formField.NullTipText = fieldAttr.NullTipText;
                            formField.IsMultiSelect = fieldAttr.IsMultiSelect;
                            formField.ValueField = fieldAttr.ValueField;
                            formField.TextField = fieldAttr.TextField;
                            formField.UrlOrData = fieldAttr.Url;
                            CommonOperate.OperateRecord<Sys_FormField>(formField, opType, out errMsg, null, false);
                        }
                        else
                        {
                            CommonOperate.DeleteRecordsByExpression<Sys_FormField>(x => x.Sys_FormId == formId && x.Sys_FieldId == fieldId, out errMsg);
                        }
                    }

                    #endregion
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 是否需要初始化
        /// </summary>
        /// <returns></returns>
        public static bool IsNeedInit()
        {
            try
            {
                if (WebConfigHelper.GetAppSettingValue("NeedInit") != "true")
                    return false;
                DbLinkArgs dbLinkArgs = ModelConfigHelper.GetLocalDbLinkArgs();
                if (dbLinkArgs == null) return true;
                if (!SystemOperate.DbIsExists(dbLinkArgs))
                {
                    //向各数据库注册存储过程
                    SystemOperate.RegStoredProcedure();
                    //在当前数据库中自动注册外部链接数据库服务器
                    SystemOperate.RegCrossDbServer();
                }
                string errMsg = string.Empty;
                bool rs = CommonOperate.ColumnIsExists(typeof(Sys_Module), "Name");
                return !rs;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 系统初始化时调用，初始化数据
        /// </summary>
        /// <returns></returns>
        public static string InitData()
        {
            string errMsg = string.Empty;
            if (IsNeedInit()) //需要初始化
            {
                if (ModelConfigHelper.GetLocalDbLinkArgs() == null)
                {
                    return "数据库连接字符串未设置！";
                }
                List<Type> types = BridgeObject.GetAllModelTypes();
                if (types == null || types.Count == 0)
                    return "不存在实体类型！";
                //是否只初始化系统模块
                bool isOnlyInitSys = WebConfigHelper.GetAppSettingValue("IsOnlyInitSys") == "true";
                if (isOnlyInitSys) //不包含组织架构模块和流程模块
                {
                    types = types.Where(x => x.BaseType != null && x.BaseType.Name != "BaseOrgMEntity" && x.BaseType.Name != "BaseBpmEntity").ToList();
                }
                #region 顺序调整
                Dictionary<Type, ModuleConfigAttribute> dic = new Dictionary<Type, ModuleConfigAttribute>();
                List<Type> modelTypes = new List<Type>();
                foreach (Type type in types)
                {
                    ModuleConfigAttribute moduleConfig = ((ModuleConfigAttribute)(Attribute.GetCustomAttribute(type, typeof(ModuleConfigAttribute))));
                    dic.Add(type, moduleConfig);
                }
                //将父模块的顺序放到前面
                foreach (Type type in dic.Keys)
                {
                    ModuleConfigAttribute moduleConfig = dic[type];
                    if (moduleConfig != null)
                    {
                        if (!string.IsNullOrEmpty(moduleConfig.ParentName))
                        {
                            KeyValuePair<Type, ModuleConfigAttribute> kv = dic.Where(x => x.Value != null && x.Value.Name == moduleConfig.ParentName).FirstOrDefault();
                            if (!string.IsNullOrEmpty(kv.Value.ParentName))
                            {
                                KeyValuePair<Type, ModuleConfigAttribute> tempKv = dic.Where(x => x.Value.Name == kv.Value.ParentName).FirstOrDefault();
                                if (modelTypes.Where(x => x.Name == tempKv.Key.Name).FirstOrDefault() == null)
                                {
                                    modelTypes.Add(tempKv.Key);
                                }
                            }
                            if (modelTypes.Where(x => x.Name == kv.Key.Name).FirstOrDefault() == null)
                            {
                                modelTypes.Add(kv.Key);
                            }
                        }
                    }
                    if (modelTypes.Where(x => x.Name == type.Name).FirstOrDefault() == null)
                    {
                        modelTypes.Add(type);
                    }
                }
                #endregion
                errMsg = InitNewModules(modelTypes, false);
                if (string.IsNullOrEmpty(errMsg))
                {
                    #region 图标初始化
                    long count = CommonOperate.Count<Sys_IconManage>(out errMsg);
                    if (count == 0)
                    {
                        List<Sys_IconManage> list = new List<Sys_IconManage>();
                        #region 系统图标
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-add",
                            IconAddr = "icons/edit_add.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-edit",
                            IconAddr = "icons/pencil.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-clear",
                            IconAddr = "icons/clear.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-remove",
                            IconAddr = "icons/edit_remove.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-save",
                            IconAddr = "icons/filesave.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-cut",
                            IconAddr = "icons/cut.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-ok",
                            IconAddr = "icons/ok.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-no",
                            IconAddr = "icons/no.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-cancel",
                            IconAddr = "icons/cancel.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-reload",
                            IconAddr = "icons/reload.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-search",
                            IconAddr = "icons/search.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-print",
                            IconAddr = "icons/print.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-help",
                            IconAddr = "icons/help.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-undo",
                            IconAddr = "icons/undo.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-redo",
                            IconAddr = "icons/redo.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-back",
                            IconAddr = "icons/back.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-sum",
                            IconAddr = "icons/sum.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-tip",
                            IconAddr = "icons/tip.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-filter",
                            IconAddr = "icons/filter.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-man",
                            IconAddr = "icons/man.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-lock",
                            IconAddr = "icons/lock.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-more",
                            IconAddr = "icons/more.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-large-picture",
                            IconAddr = "icons/large_picture.png",
                            IconType = (int)IconTypeEnum.Piex32,
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-large-shapes",
                            IconAddr = "icons/large_shapes.png",
                            IconType = (int)IconTypeEnum.Piex32,
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-large-smartart",
                            IconAddr = "icons/large_smartart.png",
                            IconType = (int)IconTypeEnum.Piex32,
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-large-chart",
                            IconAddr = "icons/large_chart.png",
                            IconType = (int)IconTypeEnum.Piex32,
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-close",
                            IconAddr = "icons/cancel.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-del",
                            IconAddr = "icons/cancel.png",
                            IconClass = (int)IconClassTypeEnum.SystemIcon
                        });
                        #endregion
                        #region 自定义图标
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-draft",
                            IconAddr = "icons/draft.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-recycle",
                            IconAddr = "icons/recycle.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-grid",
                            IconAddr = "icons/grid.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-user",
                            IconAddr = "icons/user.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-box",
                            IconAddr = "icons/box.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-group",
                            IconAddr = "icons/group.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-dept",
                            IconAddr = "icons/dept.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrow_in",
                            IconAddr = "icons/arrow_in.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrow_out",
                            IconAddr = "icons/arrow_out.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrow_down",
                            IconAddr = "icons/arrow_down.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrow_left",
                            IconAddr = "icons/arrow_left.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrow_right",
                            IconAddr = "icons/arrow_right.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrow_up",
                            IconAddr = "icons/arrow_up.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-switchcome",
                            IconAddr = "icons/switchcome.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-switchgo",
                            IconAddr = "icons/switchgo.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-batchresize",
                            IconAddr = "icons/batchresize.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-export",
                            IconAddr = "icons/excel.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-attachment",
                            IconAddr = "icons/attachment.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-copy",
                            IconAddr = "icons/copy.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-approvalok",
                            IconAddr = "icons/approvalok.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-tosubmit",
                            IconAddr = "icons/tosubmit.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-toapproval",
                            IconAddr = "icons/toapproval.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-inapproval",
                            IconAddr = "icons/inapproval.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-reject",
                            IconAddr = "icons/reject.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-toaudit",
                            IconAddr = "icons/toaudit.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-auditok",
                            IconAddr = "icons/auditok.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-tovoid",
                            IconAddr = "icons/tovoid.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-toreturn",
                            IconAddr = "icons/toreturn.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-more-customer",
                            IconAddr = "icons/more.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-password",
                            IconAddr = "icons/password.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-changePwd",
                            IconAddr = "icons/changePwd.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrowup",
                            IconAddr = "icons/arrowup.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrowdown",
                            IconAddr = "icons/arrowdown.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-email",
                            IconAddr = "icons/email.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-empty",
                            IconAddr = "icons/empty.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-print-customer",
                            IconAddr = "icons/print.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-cog",
                            IconAddr = "icons/cog.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-lock-customer",
                            IconAddr = "icons/lock.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-unlock",
                            IconAddr = "icons/unlock.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-calendar",
                            IconAddr = "icons/calendar.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-cross",
                            IconAddr = "icons/cross.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-arrow_refresh",
                            IconAddr = "icons/arrow_refresh.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-folder",
                            IconAddr = "icons/folder.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-advance_search",
                            IconAddr = "icons/advance_search.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-docEdit",
                            IconAddr = "icons/docEdit.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-excel_32",
                            IconAddr = "icons/excel_32.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        list.Add(new Sys_IconManage()
                        {
                            StyleClassName = "eu-icon-exit_16",
                            IconAddr = "icons/exit_16.png",
                            IconClass = (int)IconClassTypeEnum.CustomerIcon
                        });
                        #endregion
                        #region 第二批自定义图标
                        List<string> styleClassNames = new List<string>() { "eu-p2-icon-add_other", "eu-p2-icon-edit", "eu-p2-icon-remove", "eu-p2-icon-clear", "eu-p2-icon-save", "eu-p2-icon-delete", "eu-p2-icon-delete0", "eu-p2-icon-delete1", "eu-p2-icon-delete2", "eu-p2-icon-delete3", "eu-p2-icon-database", "eu-p2-icon-download", "eu-p2-icon-key", "eu-p2-icon-search", "eu-p2-icon-reload", "eu-p2-icon-ok", "eu-p2-icon-cancel", "eu-p2-icon-export", "eu-p2-icon-audit", "eu-p2-icon-expand", "eu-p2-icon-collapse", "eu-p2-icon-set1", "eu-p2-icon-set2", "eu-p2-icon-list", "eu-p2-icon-sysset", "eu-p2-icon-nav", "eu-p2-icon-users", "eu-p2-icon-undo", "eu-p2-icon-group", "eu-p2-icon-group32", "eu-p2-icon-group-add", "eu-p2-icon-group-delete", "eu-p2-icon-user", "eu-p2-icon-user32", "eu-p2-icon-user-edit32", "eu-p2-icon-user-accept", "eu-p2-icon-user-accept32", "eu-p2-icon-user-reject", "eu-p2-icon-user-reject32", "eu-p2-icon-boss", "eu-p2-icon-stop", "eu-p2-icon-error", "eu-p2-icon-org", "eu-p2-icon-org32", "eu-p2-icon-clock", "eu-p2-icon-rihtarrow", "eu-p2-icon-accept", "eu-p2-icon-anchor", "eu-p2-icon-application", "eu-p2-icon-application_add", "eu-p2-icon-application_cascade", "eu-p2-icon-application_delete", "eu-p2-icon-application_double", "eu-p2-icon-application_edit", "eu-p2-icon-application_error", "eu-p2-icon-application_form", "eu-p2-icon-application_form_add", "eu-p2-icon-application_form_delete", "eu-p2-icon-application_form_edit", "eu-p2-icon-application_form_magnify", "eu-p2-icon-application_get", "eu-p2-icon-application_go", "eu-p2-icon-application_home", "eu-p2-icon-application_key", "eu-p2-icon-application_lightning", "eu-p2-icon-application_link", "eu-p2-icon-application_osx", "eu-p2-icon-application_osx_add", "eu-p2-icon-application_osx_cascade", "eu-p2-icon-application_osx_delete", "eu-p2-icon-application_osx_double", "eu-p2-icon-application_osx_error", "eu-p2-icon-application_osx_get", "eu-p2-icon-application_osx_go", "eu-p2-icon-application_osx_home", "eu-p2-icon-application_osx_key", "eu-p2-icon-application_osx_lightning", "eu-p2-icon-application_osx_link", "eu-p2-icon-application_osx_split", "eu-p2-icon-application_osx_start", "eu-p2-icon-application_osx_stop", "eu-p2-icon-application_osx_terminal", "eu-p2-icon-application_put", "eu-p2-icon-application_side_boxes", "eu-p2-icon-application_side_contract", "eu-p2-icon-application_side_expand", "eu-p2-icon-application_side_list", "eu-p2-icon-application_side_tree", "eu-p2-icon-application_split", "eu-p2-icon-application_start", "eu-p2-icon-application_stop", "eu-p2-icon-application_tile_horizontal", "eu-p2-icon-application_tile_vertical", "eu-p2-icon-application_view_columns", "eu-p2-icon-application_view_detail", "eu-p2-icon-application_view_gallery", "eu-p2-icon-application_view_icons", "eu-p2-icon-application_view_list", "eu-p2-icon-application_view_tile", "eu-p2-icon-application_xp", "eu-p2-icon-application_xp_terminal", "eu-p2-icon-arrow_branch", "eu-p2-icon-arrow_divide", "eu-p2-icon-arrow_down", "eu-p2-icon-arrow_ew", "eu-p2-icon-arrow_in", "eu-p2-icon-arrow_inout", "eu-p2-icon-arrow_in_longer", "eu-p2-icon-arrow_join", "eu-p2-icon-arrow_left", "eu-p2-icon-arrow_merge", "eu-p2-icon-arrow_ne", "eu-p2-icon-arrow_ns", "eu-p2-icon-arrow_nsew", "eu-p2-icon-arrow_nw", "eu-p2-icon-arrow_nw_ne_sw_se", "eu-p2-icon-arrow_nw_se", "eu-p2-icon-arrow_out", "eu-p2-icon-arrow_out_longer", "eu-p2-icon-arrow_redo", "eu-p2-icon-arrow_refresh", "eu-p2-icon-arrow_refresh_small", "eu-p2-icon-arrow_right", "eu-p2-icon-arrow_right_16", "eu-p2-icon-arrow_rotate_anticlockwise", "eu-p2-icon-arrow_rotate_clockwise", "eu-p2-icon-arrow_se", "eu-p2-icon-arrow_sw", "eu-p2-icon-arrow_switch", "eu-p2-icon-arrow_switch_bluegreen", "eu-p2-icon-arrow_sw_ne", "eu-p2-icon-arrow_turn_left", "eu-p2-icon-arrow_turn_right", "eu-p2-icon-arrow_undo", "eu-p2-icon-arrow_up", "eu-p2-icon-asterisk_orange", "eu-p2-icon-asterisk_red", "eu-p2-icon-asterisk_yellow", "eu-p2-icon-attach", "eu-p2-icon-award_star_add", "eu-p2-icon-award_star_bronze_1", "eu-p2-icon-award_star_bronze_2", "eu-p2-icon-award_star_bronze_3", "eu-p2-icon-award_star_delete", "eu-p2-icon-award_star_gold_1", "eu-p2-icon-award_star_gold_2", "eu-p2-icon-award_star_gold_3", "eu-p2-icon-award_star_silver_1", "eu-p2-icon-award_star_silver_2", "eu-p2-icon-award_star_silver_3", "eu-p2-icon-basket", "eu-p2-icon-basket_add", "eu-p2-icon-basket_delete", "eu-p2-icon-basket_edit", "eu-p2-icon-basket_error", "eu-p2-icon-basket_go", "eu-p2-icon-basket_put", "eu-p2-icon-basket_remove", "eu-p2-icon-bell", "eu-p2-icon-bell_add", "eu-p2-icon-bell_delete", "eu-p2-icon-bell_error", "eu-p2-icon-bell_go", "eu-p2-icon-bell_link", "eu-p2-icon-bell_silver", "eu-p2-icon-bell_silver_start", "eu-p2-icon-bell_silver_stop", "eu-p2-icon-bell_start", "eu-p2-icon-bell_stop", "eu-p2-icon-bin", "eu-p2-icon-bin_closed", "eu-p2-icon-bin_empty", "eu-p2-icon-bomb", "eu-p2-icon-book", "eu-p2-icon-bookmark", "eu-p2-icon-bookmark_add", "eu-p2-icon-bookmark_delete", "eu-p2-icon-bookmark_edit", "eu-p2-icon-bookmark_error", "eu-p2-icon-bookmark_go", "eu-p2-icon-book_add", "eu-p2-icon-book_addresses", "eu-p2-icon-book_addresses_add", "eu-p2-icon-book_addresses_delete", "eu-p2-icon-book_addresses_edit", "eu-p2-icon-book_addresses_error", "eu-p2-icon-book_addresses_key", "eu-p2-icon-book_delete", "eu-p2-icon-book_edit", "eu-p2-icon-book_error", "eu-p2-icon-book_go", "eu-p2-icon-book_key", "eu-p2-icon-book_link", "eu-p2-icon-book_magnify", "eu-p2-icon-book_next", "eu-p2-icon-book_open", "eu-p2-icon-book_open_mark", "eu-p2-icon-book_previous", "eu-p2-icon-book_red", "eu-p2-icon-book_tabs", "eu-p2-icon-border_all", "eu-p2-icon-border_bottom", "eu-p2-icon-border_draw", "eu-p2-icon-border_inner", "eu-p2-icon-border_inner_horizontal", "eu-p2-icon-border_inner_vertical", "eu-p2-icon-border_left", "eu-p2-icon-border_none", "eu-p2-icon-border_outer", "eu-p2-icon-border_right", "eu-p2-icon-border_top", "eu-p2-icon-box", "eu-p2-icon-box_error", "eu-p2-icon-box_picture", "eu-p2-icon-box_world", "eu-p2-icon-brick", "eu-p2-icon-bricks", "eu-p2-icon-brick_add", "eu-p2-icon-brick_delete", "eu-p2-icon-brick_edit", "eu-p2-icon-brick_error", "eu-p2-icon-brick_go", "eu-p2-icon-brick_link", "eu-p2-icon-brick_magnify", "eu-p2-icon-briefcase", "eu-p2-icon-bug", "eu-p2-icon-bug_add", "eu-p2-icon-bug_delete", "eu-p2-icon-bug_edit", "eu-p2-icon-bug_error", "eu-p2-icon-bug_fix", "eu-p2-icon-bug_go", "eu-p2-icon-bug_link", "eu-p2-icon-bug_magnify", "eu-p2-icon-build", "eu-p2-icon-building", "eu-p2-icon-building_add", "eu-p2-icon-building_delete", "eu-p2-icon-building_edit", "eu-p2-icon-building_error", "eu-p2-icon-building_go", "eu-p2-icon-building_key", "eu-p2-icon-building_link", "eu-p2-icon-build_cancel", "eu-p2-icon-bullet_add", "eu-p2-icon-bullet_arrow_bottom", "eu-p2-icon-bullet_arrow_down", "eu-p2-icon-bullet_arrow_top", "eu-p2-icon-bullet_arrow_up", "eu-p2-icon-bullet_black", "eu-p2-icon-bullet_blue", "eu-p2-icon-bullet_connect", "eu-p2-icon-bullet_cross", "eu-p2-icon-bullet_database", "eu-p2-icon-bullet_database_yellow", "eu-p2-icon-bullet_delete", "eu-p2-icon-bullet_disk", "eu-p2-icon-bullet_earth", "eu-p2-icon-bullet_edit", "eu-p2-icon-bullet_eject", "eu-p2-icon-bullet_error", "eu-p2-icon-bullet_feed", "eu-p2-icon-bullet_get", "eu-p2-icon-bullet_go", 
                            "eu-p2-icon-bullet_green", "eu-p2-icon-bullet_home", "eu-p2-icon-bullet_key", "eu-p2-icon-bullet_left", "eu-p2-icon-bullet_lightning", "eu-p2-icon-bullet_magnify", "eu-p2-icon-bullet_minus", "eu-p2-icon-bullet_orange", "eu-p2-icon-bullet_page_white", "eu-p2-icon-bullet_picture", "eu-p2-icon-bullet_pink", "eu-p2-icon-bullet_plus", "eu-p2-icon-bullet_purple", "eu-p2-icon-bullet_red", "eu-p2-icon-bullet_right", "eu-p2-icon-bullet_shape", "eu-p2-icon-bullet_sparkle", "eu-p2-icon-bullet_star", "eu-p2-icon-bullet_start", "eu-p2-icon-bullet_stop", "eu-p2-icon-bullet_stop_alt", "eu-p2-icon-bullet_tick", "eu-p2-icon-bullet_toggle_minus", "eu-p2-icon-bullet_toggle_plus", "eu-p2-icon-bullet_white", "eu-p2-icon-bullet_wrench", "eu-p2-icon-bullet_wrench_red", "eu-p2-icon-bullet_yellow", "eu-p2-icon-button", "eu-p2-icon-cake", "eu-p2-icon-cake_out", "eu-p2-icon-cake_sliced", "eu-p2-icon-calculator", "eu-p2-icon-calculator_add", "eu-p2-icon-calculator_delete", "eu-p2-icon-calculator_edit", "eu-p2-icon-calculator_error", "eu-p2-icon-calculator_link", "eu-p2-icon-calendar", "eu-p2-icon-calendar_add", "eu-p2-icon-calendar_delete", "eu-p2-icon-calendar_edit", "eu-p2-icon-calendar_link", "eu-p2-icon-calendar_select_day", "eu-p2-icon-calendar_select_none", "eu-p2-icon-calendar_select_week", "eu-p2-icon-calendar_star", "eu-p2-icon-calendar_view_day", "eu-p2-icon-calendar_view_month", "eu-p2-icon-calendar_view_week", "eu-p2-icon-camera", "eu-p2-icon-camera_add", "eu-p2-icon-camera_connect", "eu-p2-icon-camera_delete", "eu-p2-icon-camera_edit", "eu-p2-icon-camera_error", "eu-p2-icon-camera_go", "eu-p2-icon-camera_link", "eu-p2-icon-camera_magnify", "eu-p2-icon-camera_picture", "eu-p2-icon-camera_small", "eu-p2-icon-camera_start", "eu-p2-icon-camera_stop", "eu-p2-icon-cancel", "eu-p2-icon-car", "eu-p2-icon-cart", "eu-p2-icon-cart_add", "eu-p2-icon-cart_delete", "eu-p2-icon-cart_edit", "eu-p2-icon-cart_error", "eu-p2-icon-cart_full", "eu-p2-icon-cart_go", "eu-p2-icon-cart_magnify", "eu-p2-icon-cart_put", "eu-p2-icon-cart_remove", "eu-p2-icon-car_add", "eu-p2-icon-car_delete", "eu-p2-icon-car_error", "eu-p2-icon-car_red", "eu-p2-icon-car_start", "eu-p2-icon-car_stop", "eu-p2-icon-cd", "eu-p2-icon-cdr", "eu-p2-icon-cdr_add", "eu-p2-icon-cdr_burn", "eu-p2-icon-cdr_cross", "eu-p2-icon-cdr_delete", "eu-p2-icon-cdr_edit", "eu-p2-icon-cdr_eject", "eu-p2-icon-cdr_error", "eu-p2-icon-cdr_go", "eu-p2-icon-cdr_magnify", "eu-p2-icon-cdr_play", "eu-p2-icon-cdr_start", "eu-p2-icon-cdr_stop", "eu-p2-icon-cdr_stop_alt", "eu-p2-icon-cdr_tick", "eu-p2-icon-cd_add", "eu-p2-icon-cd_burn", "eu-p2-icon-cd_delete", "eu-p2-icon-cd_edit", "eu-p2-icon-cd_eject", "eu-p2-icon-cd_go", "eu-p2-icon-cd_magnify", "eu-p2-icon-cd_play", "eu-p2-icon-cd_stop", "eu-p2-icon-cd_stop_alt", "eu-p2-icon-cd_tick", "eu-p2-icon-chart_bar", "eu-p2-icon-chart_bar_add", "eu-p2-icon-chart_bar_delete", "eu-p2-icon-chart_bar_edit", "eu-p2-icon-chart_bar_error", "eu-p2-icon-chart_bar_link", "eu-p2-icon-chart_curve", "eu-p2-icon-chart_curve_add", "eu-p2-icon-chart_curve_delete", "eu-p2-icon-chart_curve_edit", "eu-p2-icon-chart_curve_error", "eu-p2-icon-chart_curve_go", "eu-p2-icon-chart_curve_link", "eu-p2-icon-chart_line", "eu-p2-icon-chart_line_add", "eu-p2-icon-chart_line_delete", "eu-p2-icon-chart_line_edit", "eu-p2-icon-chart_line_error", "eu-p2-icon-chart_line_link", "eu-p2-icon-chart_organisation", "eu-p2-icon-chart_organisation_add", "eu-p2-icon-chart_organisation_delete", "eu-p2-icon-chart_org_inverted", "eu-p2-icon-chart_pie", "eu-p2-icon-chart_pie_add", "eu-p2-icon-chart_pie_delete", "eu-p2-icon-chart_pie_edit", "eu-p2-icon-chart_pie_error", "eu-p2-icon-chart_pie_lightning", "eu-p2-icon-chart_pie_link", "eu-p2-icon-check_error", "eu-p2-icon-clipboard", "eu-p2-icon-clock", "eu-p2-icon-clock_add", "eu-p2-icon-clock_delete", "eu-p2-icon-clock_edit", "eu-p2-icon-clock_error", "eu-p2-icon-clock_go", "eu-p2-icon-clock_link", "eu-p2-icon-clock_pause", "eu-p2-icon-clock_play", "eu-p2-icon-clock_red", "eu-p2-icon-clock_start", "eu-p2-icon-clock_stop", "eu-p2-icon-cmy", "eu-p2-icon-cog", "eu-p2-icon-cog_add", "eu-p2-icon-cog_delete", "eu-p2-icon-cog_edit", "eu-p2-icon-cog_error", "eu-p2-icon-cog_go", "eu-p2-icon-cog_start", "eu-p2-icon-cog_stop", "eu-p2-icon-coins", "eu-p2-icon-coins_add", "eu-p2-icon-coins_delete", "eu-p2-icon-color", "eu-p2-icon-color_swatch", "eu-p2-icon-color_wheel", "eu-p2-icon-comment", "eu-p2-icon-comments", "eu-p2-icon-comments_add", "eu-p2-icon-comments_delete", "eu-p2-icon-comment_add", "eu-p2-icon-comment_delete", "eu-p2-icon-comment_dull", "eu-p2-icon-comment_edit", "eu-p2-icon-comment_play", "eu-p2-icon-comment_record", "eu-p2-icon-compass", "eu-p2-icon-compress", "eu-p2-icon-computer", "eu-p2-icon-computer_add", "eu-p2-icon-computer_connect", "eu-p2-icon-computer_delete", "eu-p2-icon-computer_edit", "eu-p2-icon-computer_error", "eu-p2-icon-computer_go", "eu-p2-icon-computer_key", "eu-p2-icon-computer_link", "eu-p2-icon-computer_magnify", "eu-p2-icon-computer_off", "eu-p2-icon-computer_start", "eu-p2-icon-computer_stop", "eu-p2-icon-computer_wrench", "eu-p2-icon-connect", "eu-p2-icon-contrast", "eu-p2-icon-contrast_decrease", "eu-p2-icon-contrast_high", "eu-p2-icon-contrast_increase", "eu-p2-icon-contrast_low", "eu-p2-icon-controller", "eu-p2-icon-controller_add", "eu-p2-icon-controller_delete", "eu-p2-icon-controller_error", "eu-p2-icon-control_add", "eu-p2-icon-control_add_blue", "eu-p2-icon-control_blank", "eu-p2-icon-control_blank_blue", "eu-p2-icon-control_eject", "eu-p2-icon-control_eject_blue", "eu-p2-icon-control_end", "eu-p2-icon-control_end_blue", "eu-p2-icon-control_equalizer", "eu-p2-icon-control_equalizer_blue", "eu-p2-icon-control_fastforward", "eu-p2-icon-control_fastforward_blue", "eu-p2-icon-control_pause", "eu-p2-icon-control_pause_blue", "eu-p2-icon-control_play", "eu-p2-icon-control_play_blue", "eu-p2-icon-control_power", "eu-p2-icon-control_power_blue", "eu-p2-icon-control_record", "eu-p2-icon-control_record_blue", "eu-p2-icon-control_remove", "eu-p2-icon-control_remove_blue", "eu-p2-icon-control_repeat", "eu-p2-icon-control_repeat_blue", "eu-p2-icon-control_rewind", "eu-p2-icon-control_rewind_blue", "eu-p2-icon-control_start", "eu-p2-icon-control_start_blue", "eu-p2-icon-control_stop", "eu-p2-icon-control_stop_blue", "eu-p2-icon-creditcards", "eu-p2-icon-cross", "eu-p2-icon-css", "eu-p2-icon-css_add", "eu-p2-icon-css_delete", "eu-p2-icon-css_error", "eu-p2-icon-css_go", "eu-p2-icon-css_valid", "eu-p2-icon-cup", "eu-p2-icon-cup_add", "eu-p2-icon-cup_black", "eu-p2-icon-cup_delete", "eu-p2-icon-cup_edit", "eu-p2-icon-cup_error", "eu-p2-icon-cup_go", "eu-p2-icon-cup_green", "eu-p2-icon-cup_key", "eu-p2-icon-cup_link", "eu-p2-icon-cup_tea", "eu-p2-icon-cursor", "eu-p2-icon-cursor_small", "eu-p2-icon-cut", "eu-p2-icon-cut_red", "eu-p2-icon-database", "eu-p2-icon-database_add", "eu-p2-icon-database_connect", "eu-p2-icon-database_copy", "eu-p2-icon-database_delete", "eu-p2-icon-database_edit", "eu-p2-icon-database_error", "eu-p2-icon-database_gear", "eu-p2-icon-database_go", "eu-p2-icon-database_key", "eu-p2-icon-database_lightning", "eu-p2-icon-database_link", "eu-p2-icon-database_refresh", "eu-p2-icon-database_save", "eu-p2-icon-database_start", "eu-p2-icon-database_stop", "eu-p2-icon-database_table", 
                            "eu-p2-icon-database_wrench", "eu-p2-icon-database_yellow", "eu-p2-icon-database_yellow_start", "eu-p2-icon-database_yellow_stop", "eu-p2-icon-date", "eu-p2-icon-date_add", "eu-p2-icon-date_delete", "eu-p2-icon-date_edit", "eu-p2-icon-date_error", "eu-p2-icon-date_go", "eu-p2-icon-date_link", "eu-p2-icon-date_magnify", "eu-p2-icon-date_next", "eu-p2-icon-date_previous", "eu-p2-icon-decline", "eu-p2-icon-delete", "eu-p2-icon-device_stylus", "eu-p2-icon-disconnect", "eu-p2-icon-disk", "eu-p2-icon-disk_black", "eu-p2-icon-disk_black_error", "eu-p2-icon-disk_black_magnify", "eu-p2-icon-disk_download", "eu-p2-icon-disk_edit", "eu-p2-icon-disk_error", "eu-p2-icon-disk_magnify", "eu-p2-icon-disk_multiple", "eu-p2-icon-disk_upload", "eu-p2-icon-door", "eu-p2-icon-door_error", "eu-p2-icon-door_in", "eu-p2-icon-door_open", "eu-p2-icon-door_out", "eu-p2-icon-drink", "eu-p2-icon-drink_empty", "eu-p2-icon-drink_red", "eu-p2-icon-drive", "eu-p2-icon-drive_add", "eu-p2-icon-drive_burn", "eu-p2-icon-drive_cd", "eu-p2-icon-drive_cdr", "eu-p2-icon-drive_cd_empty", "eu-p2-icon-drive_delete", "eu-p2-icon-drive_disk", "eu-p2-icon-drive_edit", "eu-p2-icon-drive_error", "eu-p2-icon-drive_go", "eu-p2-icon-drive_key", "eu-p2-icon-drive_link", "eu-p2-icon-drive_magnify", "eu-p2-icon-drive_network", "eu-p2-icon-drive_network_error", "eu-p2-icon-drive_network_stop", "eu-p2-icon-drive_rename", "eu-p2-icon-drive_user", "eu-p2-icon-drive_web", "eu-p2-icon-dvd", "eu-p2-icon-dvd_add", "eu-p2-icon-dvd_delete", "eu-p2-icon-dvd_edit", "eu-p2-icon-dvd_error", "eu-p2-icon-dvd_go", "eu-p2-icon-dvd_key", "eu-p2-icon-dvd_link", "eu-p2-icon-dvd_start", "eu-p2-icon-dvd_stop", "eu-p2-icon-eject_blue", "eu-p2-icon-eject_green", "eu-p2-icon-email", "eu-p2-icon-email_add", "eu-p2-icon-email_attach", "eu-p2-icon-email_delete", "eu-p2-icon-email_edit", "eu-p2-icon-email_error", "eu-p2-icon-email_go", "eu-p2-icon-email_link", "eu-p2-icon-email_magnify", "eu-p2-icon-email_open", "eu-p2-icon-email_open_image", "eu-p2-icon-email_star", "eu-p2-icon-email_start", "eu-p2-icon-email_stop", "eu-p2-icon-email_transfer", "eu-p2-icon-emoticon_evilgrin", "eu-p2-icon-emoticon_grin", "eu-p2-icon-emoticon_happy", "eu-p2-icon-emoticon_smile", "eu-p2-icon-emoticon_surprised", "eu-p2-icon-emoticon_tongue", "eu-p2-icon-emoticon_unhappy", "eu-p2-icon-emoticon_waii", "eu-p2-icon-emoticon_wink", "eu-p2-icon-erase", "eu-p2-icon-error", "eu-p2-icon-error_add", "eu-p2-icon-error_delete", "eu-p2-icon-error_go", "eu-p2-icon-exclamation", "eu-p2-icon-eye", "eu-p2-icon-eyes", "eu-p2-icon-feed", "eu-p2-icon-feed_add", "eu-p2-icon-feed_delete", "eu-p2-icon-feed_disk", "eu-p2-icon-feed_edit", "eu-p2-icon-feed_error", "eu-p2-icon-feed_go", "eu-p2-icon-feed_key", "eu-p2-icon-feed_link", "eu-p2-icon-feed_magnify", "eu-p2-icon-feed_star", "eu-p2-icon-female", "eu-p2-icon-film", "eu-p2-icon-film_add", "eu-p2-icon-film_delete", "eu-p2-icon-film_edit", "eu-p2-icon-film_eject", "eu-p2-icon-film_error", "eu-p2-icon-film_go", "eu-p2-icon-film_key", "eu-p2-icon-film_link", "eu-p2-icon-film_magnify", "eu-p2-icon-film_save", "eu-p2-icon-film_star", "eu-p2-icon-film_start", "eu-p2-icon-film_stop", "eu-p2-icon-find", "eu-p2-icon-finger_point", "eu-p2-icon-flag_black", "eu-p2-icon-flag_blue", "eu-p2-icon-flag_checked", "eu-p2-icon-flag_france", "eu-p2-icon-flag_green", "eu-p2-icon-flag_grey", "eu-p2-icon-flag_orange", "eu-p2-icon-flag_pink", "eu-p2-icon-flag_purple", "eu-p2-icon-flag_red", "eu-p2-icon-flag_white", "eu-p2-icon-flag_yellow", "eu-p2-icon-flower_daisy", "eu-p2-icon-folder", "eu-p2-icon-folder_add", "eu-p2-icon-folder_bell", "eu-p2-icon-folder_bookmark", "eu-p2-icon-folder_brick", "eu-p2-icon-folder_bug", "eu-p2-icon-folder_camera", "eu-p2-icon-folder_connect", "eu-p2-icon-folder_database", "eu-p2-icon-folder_delete", "eu-p2-icon-folder_edit", "eu-p2-icon-folder_error", "eu-p2-icon-folder_explore", "eu-p2-icon-folder_feed", "eu-p2-icon-folder_film", "eu-p2-icon-folder_find", "eu-p2-icon-folder_font", "eu-p2-icon-folder_go", "eu-p2-icon-folder_heart", "eu-p2-icon-folder_home", "eu-p2-icon-folder_image", "eu-p2-icon-folder_key", "eu-p2-icon-folder_lightbulb", "eu-p2-icon-folder_link", "eu-p2-icon-folder_magnify", "eu-p2-icon-folder_page", "eu-p2-icon-folder_page_white", "eu-p2-icon-folder_palette", "eu-p2-icon-folder_picture", "eu-p2-icon-folder_star", "eu-p2-icon-folder_table", "eu-p2-icon-folder_up", "eu-p2-icon-folder_user", "eu-p2-icon-folder_wrench", "eu-p2-icon-font", "eu-p2-icon-font_add", "eu-p2-icon-font_color", "eu-p2-icon-font_delete", "eu-p2-icon-font_go", "eu-p2-icon-font_larger", "eu-p2-icon-font_smaller", "eu-p2-icon-forward_blue", "eu-p2-icon-forward_green", "eu-p2-icon-group", "eu-p2-icon-group_add", "eu-p2-icon-group_delete", "eu-p2-icon-group_edit", "eu-p2-icon-group_error", "eu-p2-icon-group_gear", "eu-p2-icon-group_go", "eu-p2-icon-group_key", "eu-p2-icon-group_link", "eu-p2-icon-heart", "eu-p2-icon-heart_add", "eu-p2-icon-heart_broken", "eu-p2-icon-heart_connect", "eu-p2-icon-heart_delete", "eu-p2-icon-help", "eu-p2-icon-hourglass", "eu-p2-icon-hourglass_add", "eu-p2-icon-hourglass_delete", "eu-p2-icon-hourglass_go", "eu-p2-icon-hourglass_link", "eu-p2-icon-house", "eu-p2-icon-house_connect", "eu-p2-icon-house_go", "eu-p2-icon-house_in", "eu-p2-icon-house_key", "eu-p2-icon-house_link", "eu-p2-icon-house_star", "eu-p2-icon-html", "eu-p2-icon-html_add", "eu-p2-icon-html_delete", "eu-p2-icon-html_error", "eu-p2-icon-html_go", "eu-p2-icon-html_valid", "eu-p2-icon-image", "eu-p2-icon-images", "eu-p2-icon-image_add", "eu-p2-icon-image_delete", "eu-p2-icon-image_edit", "eu-p2-icon-image_link", "eu-p2-icon-image_magnify", "eu-p2-icon-image_star", "eu-p2-icon-information", "eu-p2-icon-ipod", "eu-p2-icon-ipod_cast", "eu-p2-icon-ipod_cast_add", "eu-p2-icon-ipod_cast_delete", "eu-p2-icon-ipod_connect", "eu-p2-icon-ipod_nano", "eu-p2-icon-ipod_nano_connect", "eu-p2-icon-ipod_sound", "eu-p2-icon-joystick", "eu-p2-icon-joystick_add", "eu-p2-icon-joystick_connect", "eu-p2-icon-joystick_delete", "eu-p2-icon-joystick_error", "eu-p2-icon-key", "eu-p2-icon-keyboard", "eu-p2-icon-keyboard_add", "eu-p2-icon-keyboard_connect", "eu-p2-icon-keyboard_delete", "eu-p2-icon-keyboard_magnify", "eu-p2-icon-key_add", "eu-p2-icon-key_delete", "eu-p2-icon-key_go", "eu-p2-icon-key_start", "eu-p2-icon-key_stop", "eu-p2-icon-laptop", "eu-p2-icon-laptop_add", "eu-p2-icon-laptop_connect", "eu-p2-icon-laptop_delete", "eu-p2-icon-laptop_disk", "eu-p2-icon-laptop_edit", "eu-p2-icon-laptop_error", "eu-p2-icon-laptop_go", "eu-p2-icon-laptop_key", "eu-p2-icon-laptop_link", "eu-p2-icon-laptop_magnify", "eu-p2-icon-laptop_start", "eu-p2-icon-laptop_stop", "eu-p2-icon-laptop_wrench", "eu-p2-icon-layers", "eu-p2-icon-layout", "eu-p2-icon-layout_add", "eu-p2-icon-layout_content", "eu-p2-icon-layout_delete", "eu-p2-icon-layout_edit", "eu-p2-icon-layout_error", "eu-p2-icon-layout_header", "eu-p2-icon-layout_key", "eu-p2-icon-layout_lightning", "eu-p2-icon-layout_link", "eu-p2-icon-layout_sidebar", "eu-p2-icon-lightbulb", "eu-p2-icon-lightbulb_add", "eu-p2-icon-lightbulb_delete", "eu-p2-icon-lightbulb_off", "eu-p2-icon-lightning", "eu-p2-icon-lightning_add", "eu-p2-icon-lightning_delete", "eu-p2-icon-lightning_go", "eu-p2-icon-link", "eu-p2-icon-link_add", "eu-p2-icon-link_break", 
                            "eu-p2-icon-link_delete", "eu-p2-icon-link_edit", "eu-p2-icon-link_error", "eu-p2-icon-link_go", "eu-p2-icon-lock", "eu-p2-icon-lock_add", "eu-p2-icon-lock_break", "eu-p2-icon-lock_delete", "eu-p2-icon-lock_edit", "eu-p2-icon-lock_go", "eu-p2-icon-lock_key", "eu-p2-icon-lock_open", "eu-p2-icon-lock_start", "eu-p2-icon-lock_stop", "eu-p2-icon-lorry", "eu-p2-icon-lorry_add", "eu-p2-icon-lorry_delete", "eu-p2-icon-lorry_error", "eu-p2-icon-lorry_flatbed", "eu-p2-icon-lorry_go", "eu-p2-icon-lorry_link", "eu-p2-icon-lorry_start", "eu-p2-icon-lorry_stop", "eu-p2-icon-magifier_zoom_out", "eu-p2-icon-magnifier", "eu-p2-icon-magnifier_zoom_in", "eu-p2-icon-mail", "eu-p2-icon-male", "eu-p2-icon-map", "eu-p2-icon-map_add", "eu-p2-icon-map_clipboard", "eu-p2-icon-map_cursor", "eu-p2-icon-map_delete", "eu-p2-icon-map_edit", "eu-p2-icon-map_error", "eu-p2-icon-map_go", "eu-p2-icon-map_link", "eu-p2-icon-map_magnify", "eu-p2-icon-map_start", "eu-p2-icon-map_stop", "eu-p2-icon-medal_bronze_1", "eu-p2-icon-medal_bronze_2", "eu-p2-icon-medal_bronze_3", "eu-p2-icon-medal_bronze_add", "eu-p2-icon-medal_bronze_delete", "eu-p2-icon-medal_gold_1", "eu-p2-icon-medal_gold_2", "eu-p2-icon-medal_gold_3", "eu-p2-icon-medal_gold_add", "eu-p2-icon-medal_gold_delete", "eu-p2-icon-medal_silver_1", "eu-p2-icon-medal_silver_2", "eu-p2-icon-medal_silver_3", "eu-p2-icon-medal_silver_add", "eu-p2-icon-medal_silver_delete", "eu-p2-icon-money", "eu-p2-icon-money_add", "eu-p2-icon-money_delete", "eu-p2-icon-money_dollar", "eu-p2-icon-money_euro", "eu-p2-icon-money_pound", "eu-p2-icon-money_yen", "eu-p2-icon-monitor", "eu-p2-icon-monitor_add", "eu-p2-icon-monitor_delete", "eu-p2-icon-monitor_edit", "eu-p2-icon-monitor_error", "eu-p2-icon-monitor_go", "eu-p2-icon-monitor_key", "eu-p2-icon-monitor_lightning", "eu-p2-icon-monitor_link", "eu-p2-icon-moon_full", "eu-p2-icon-mouse", "eu-p2-icon-mouse_add", "eu-p2-icon-mouse_delete", "eu-p2-icon-mouse_error", "eu-p2-icon-music", "eu-p2-icon-music_note", "eu-p2-icon-neighbourhood", "eu-p2-icon-new", "eu-p2-icon-newspaper", "eu-p2-icon-newspaper_add", "eu-p2-icon-newspaper_delete", "eu-p2-icon-newspaper_go", "eu-p2-icon-newspaper_link", "eu-p2-icon-new_blue", "eu-p2-icon-new_red", "eu-p2-icon-next", "eu-p2-icon-next-green", "eu-p2-icon-next_blue", "eu-p2-icon-next_green", "eu-p2-icon-note", "eu-p2-icon-note_add", "eu-p2-icon-note_delete", "eu-p2-icon-note_edit", "eu-p2-icon-note_error", "eu-p2-icon-note_go", "eu-p2-icon-outline", "eu-p2-icon-overlays", "eu-p2-icon-package", "eu-p2-icon-package_add", "eu-p2-icon-package_delete", "eu-p2-icon-package_down", "eu-p2-icon-package_go", "eu-p2-icon-package_green", "eu-p2-icon-package_in", "eu-p2-icon-package_link", "eu-p2-icon-package_se", "eu-p2-icon-package_start", "eu-p2-icon-package_stop", "eu-p2-icon-package_white", "eu-p2-icon-page", "eu-p2-icon-page_add", "eu-p2-icon-page_attach", "eu-p2-icon-page_back", "eu-p2-icon-page_break", "eu-p2-icon-page_break_insert", "eu-p2-icon-page_cancel", "eu-p2-icon-page_code", "eu-p2-icon-page_copy", "eu-p2-icon-page_delete", "eu-p2-icon-page_edit", "eu-p2-icon-page_error", "eu-p2-icon-page_excel", "eu-p2-icon-page_find", "eu-p2-icon-page_forward", "eu-p2-icon-page_gear", "eu-p2-icon-page_go", "eu-p2-icon-page_green", "eu-p2-icon-page_header_footer", "eu-p2-icon-page_key", "eu-p2-icon-page_landscape", "eu-p2-icon-page_landscape_shot", "eu-p2-icon-page_lightning", "eu-p2-icon-page_link", "eu-p2-icon-page_magnify", "eu-p2-icon-page_paintbrush", "eu-p2-icon-page_paste", "eu-p2-icon-page_portrait", "eu-p2-icon-page_portrait_shot", "eu-p2-icon-page_red", "eu-p2-icon-page_refresh", "eu-p2-icon-page_save", "eu-p2-icon-page_white", "eu-p2-icon-page_white_acrobat", "eu-p2-icon-page_white_actionscript", "eu-p2-icon-page_white_add", "eu-p2-icon-page_white_break", "eu-p2-icon-page_white_c", "eu-p2-icon-page_white_camera", "eu-p2-icon-page_white_cd", "eu-p2-icon-page_white_cdr", "eu-p2-icon-page_white_code", "eu-p2-icon-page_white_code_red", "eu-p2-icon-page_white_coldfusion", "eu-p2-icon-page_white_compressed", "eu-p2-icon-page_white_connect", "eu-p2-icon-page_white_copy", "eu-p2-icon-page_white_cplusplus", "eu-p2-icon-page_white_csharp", "eu-p2-icon-page_white_cup", "eu-p2-icon-page_white_database", "eu-p2-icon-page_white_database_yellow", "eu-p2-icon-page_white_delete", "eu-p2-icon-page_white_dvd", "eu-p2-icon-page_white_edit", "eu-p2-icon-page_white_error", "eu-p2-icon-page_white_excel", "eu-p2-icon-page_white_find", "eu-p2-icon-page_white_flash", "eu-p2-icon-page_white_font", "eu-p2-icon-page_white_freehand", "eu-p2-icon-page_white_gear", "eu-p2-icon-page_white_get", "eu-p2-icon-page_white_go", "eu-p2-icon-page_white_h", "eu-p2-icon-page_white_horizontal", "eu-p2-icon-page_white_key", "eu-p2-icon-page_white_lightning", "eu-p2-icon-page_white_link", "eu-p2-icon-page_white_magnify", "eu-p2-icon-page_white_medal", "eu-p2-icon-page_white_office", "eu-p2-icon-page_white_paint", "eu-p2-icon-page_white_paintbrush", "eu-p2-icon-page_white_paste", "eu-p2-icon-page_white_paste_table", "eu-p2-icon-page_white_php", "eu-p2-icon-page_white_picture", "eu-p2-icon-page_white_powerpoint", "eu-p2-icon-page_white_put", "eu-p2-icon-page_white_refresh", "eu-p2-icon-page_white_ruby", "eu-p2-icon-page_white_side_by_side", "eu-p2-icon-page_white_stack", "eu-p2-icon-page_white_star", "eu-p2-icon-page_white_swoosh", "eu-p2-icon-page_white_text", "eu-p2-icon-page_white_text_width", "eu-p2-icon-page_white_tux", "eu-p2-icon-page_white_vector", "eu-p2-icon-page_white_visualstudio", "eu-p2-icon-page_white_width", "eu-p2-icon-page_white_word", "eu-p2-icon-page_white_world", "eu-p2-icon-page_white_wrench", "eu-p2-icon-page_white_zip", "eu-p2-icon-page_word", "eu-p2-icon-page_world", "eu-p2-icon-paint", "eu-p2-icon-paintbrush", "eu-p2-icon-paintbrush_color", "eu-p2-icon-paintcan", "eu-p2-icon-paintcan_red", "eu-p2-icon-paint_can_brush", "eu-p2-icon-palette", "eu-p2-icon-paste_plain", "eu-p2-icon-paste_word", "eu-p2-icon-pause_blue", "eu-p2-icon-pause_green", "eu-p2-icon-pause_record", "eu-p2-icon-pencil", "eu-p2-icon-pencil_add", "eu-p2-icon-pencil_delete", "eu-p2-icon-pencil_go", "eu-p2-icon-phone", "eu-p2-icon-phone_add", "eu-p2-icon-phone_delete", "eu-p2-icon-phone_edit", "eu-p2-icon-phone_error", "eu-p2-icon-phone_go", "eu-p2-icon-phone_key", "eu-p2-icon-phone_link", "eu-p2-icon-phone_sound", "eu-p2-icon-phone_start", "eu-p2-icon-phone_stop", "eu-p2-icon-photo", "eu-p2-icon-photos", "eu-p2-icon-photo_add", "eu-p2-icon-photo_delete", "eu-p2-icon-photo_edit", "eu-p2-icon-photo_link", "eu-p2-icon-photo_paint", "eu-p2-icon-picture", "eu-p2-icon-pictures", "eu-p2-icon-pictures_thumbs", "eu-p2-icon-picture_add", "eu-p2-icon-picture_clipboard", "eu-p2-icon-picture_delete", "eu-p2-icon-picture_edit", "eu-p2-icon-picture_empty", "eu-p2-icon-picture_error", "eu-p2-icon-picture_go", "eu-p2-icon-picture_key", "eu-p2-icon-picture_link", "eu-p2-icon-picture_save", "eu-p2-icon-pilcrow", "eu-p2-icon-pill", "eu-p2-icon-pill_add", "eu-p2-icon-pill_delete", "eu-p2-icon-pill_error", "eu-p2-icon-pill_go", "eu-p2-icon-play_blue", "eu-p2-icon-play_green", "eu-p2-icon-plugin", "eu-p2-icon-plugin_add", "eu-p2-icon-plugin_delete", "eu-p2-icon-plugin_disabled", "eu-p2-icon-plugin_edit", "eu-p2-icon-plugin_error", "eu-p2-icon-plugin_go", 
                            "eu-p2-icon-plugin_key", "eu-p2-icon-plugin_link", "eu-p2-icon-previous", "eu-p2-icon-previous-green", "eu-p2-icon-printer", "eu-p2-icon-printer_add", "eu-p2-icon-printer_cancel", "eu-p2-icon-printer_color", "eu-p2-icon-printer_connect", "eu-p2-icon-printer_delete", "eu-p2-icon-printer_empty", "eu-p2-icon-printer_error", "eu-p2-icon-printer_go", "eu-p2-icon-printer_key", "eu-p2-icon-printer_mono", "eu-p2-icon-printer_start", "eu-p2-icon-printer_stop", "eu-p2-icon-rainbow", "eu-p2-icon-rainbow_star", "eu-p2-icon-record_blue", "eu-p2-icon-record_green", "eu-p2-icon-record_red", "eu-p2-icon-reload", "eu-p2-icon-report", "eu-p2-icon-report_add", "eu-p2-icon-report_delete", "eu-p2-icon-report_disk", "eu-p2-icon-report_edit", "eu-p2-icon-report_go", "eu-p2-icon-report_key", "eu-p2-icon-report_link", "eu-p2-icon-report_magnify", "eu-p2-icon-report_picture", "eu-p2-icon-report_start", "eu-p2-icon-report_stop", "eu-p2-icon-report_user", "eu-p2-icon-report_word", "eu-p2-icon-resultset_first", "eu-p2-icon-resultset_last", "eu-p2-icon-resultset_next", "eu-p2-icon-resultset_previous", "eu-p2-icon-reverse_blue", "eu-p2-icon-reverse_green", "eu-p2-icon-rewind_blue", "eu-p2-icon-rewind_green", "eu-p2-icon-rgb", "eu-p2-icon-rosette", "eu-p2-icon-rosette_blue", "eu-p2-icon-rss", "eu-p2-icon-rss_add", "eu-p2-icon-rss_delete", "eu-p2-icon-rss_error", "eu-p2-icon-rss_go", "eu-p2-icon-rss_valid", "eu-p2-icon-ruby", "eu-p2-icon-ruby_add", "eu-p2-icon-ruby_delete", "eu-p2-icon-ruby_gear", "eu-p2-icon-ruby_get", "eu-p2-icon-ruby_go", "eu-p2-icon-ruby_key", "eu-p2-icon-ruby_link", "eu-p2-icon-ruby_put", "eu-p2-icon-script", "eu-p2-icon-script_add", "eu-p2-icon-script_code", "eu-p2-icon-script_code_red", "eu-p2-icon-script_delete", "eu-p2-icon-script_edit", "eu-p2-icon-script_error", "eu-p2-icon-script_gear", "eu-p2-icon-script_go", "eu-p2-icon-script_key", "eu-p2-icon-script_lightning", "eu-p2-icon-script_link", "eu-p2-icon-script_palette", "eu-p2-icon-script_save", "eu-p2-icon-script_start", "eu-p2-icon-script_stop", "eu-p2-icon-seasons", "eu-p2-icon-section_collapsed", "eu-p2-icon-section_expanded", "eu-p2-icon-server", "eu-p2-icon-server_add", "eu-p2-icon-server_chart", "eu-p2-icon-server_compressed", "eu-p2-icon-server_connect", "eu-p2-icon-server_database", "eu-p2-icon-server_delete", "eu-p2-icon-server_edit", "eu-p2-icon-server_error", "eu-p2-icon-server_go", "eu-p2-icon-server_key", "eu-p2-icon-server_lightning", "eu-p2-icon-server_link", "eu-p2-icon-server_start", "eu-p2-icon-server_stop", "eu-p2-icon-server_uncompressed", "eu-p2-icon-server_wrench", "eu-p2-icon-shading", "eu-p2-icon-shapes_many", "eu-p2-icon-shapes_many_select", "eu-p2-icon-shape_3d", "eu-p2-icon-shape_align_bottom", "eu-p2-icon-shape_align_center", "eu-p2-icon-shape_align_left", "eu-p2-icon-shape_align_middle", "eu-p2-icon-shape_align_right", "eu-p2-icon-shape_align_top", "eu-p2-icon-shape_flip_horizontal", "eu-p2-icon-shape_flip_vertical", "eu-p2-icon-shape_group", "eu-p2-icon-shape_handles", "eu-p2-icon-shape_move_back", "eu-p2-icon-shape_move_backwards", "eu-p2-icon-shape_move_forwards", "eu-p2-icon-shape_move_front", "eu-p2-icon-shape_rotate_anticlockwise", "eu-p2-icon-shape_rotate_clockwise", "eu-p2-icon-shape_shade_a", "eu-p2-icon-shape_shade_b", "eu-p2-icon-shape_shade_c", "eu-p2-icon-shape_shadow", "eu-p2-icon-shape_shadow_toggle", "eu-p2-icon-shape_square", "eu-p2-icon-shape_square_add", "eu-p2-icon-shape_square_delete", "eu-p2-icon-shape_square_edit", "eu-p2-icon-shape_square_error", "eu-p2-icon-shape_square_go", "eu-p2-icon-shape_square_key", "eu-p2-icon-shape_square_link", "eu-p2-icon-shape_square_select", "eu-p2-icon-shape_ungroup", "eu-p2-icon-share", "eu-p2-icon-shield", "eu-p2-icon-shield_add", "eu-p2-icon-shield_delete", "eu-p2-icon-shield_error", "eu-p2-icon-shield_go", "eu-p2-icon-shield_rainbow", "eu-p2-icon-shield_silver", "eu-p2-icon-shield_start", "eu-p2-icon-shield_stop", "eu-p2-icon-sitemap", "eu-p2-icon-sitemap_color", "eu-p2-icon-smartphone", "eu-p2-icon-smartphone_add", "eu-p2-icon-smartphone_connect", "eu-p2-icon-smartphone_delete", "eu-p2-icon-smartphone_disk", "eu-p2-icon-smartphone_edit", "eu-p2-icon-smartphone_error", "eu-p2-icon-smartphone_go", "eu-p2-icon-smartphone_key", "eu-p2-icon-smartphone_wrench", "eu-p2-icon-sort_ascending", "eu-p2-icon-sort_descending", "eu-p2-icon-sound", "eu-p2-icon-sound_add", "eu-p2-icon-sound_delete", "eu-p2-icon-sound_high", "eu-p2-icon-sound_in", "eu-p2-icon-sound_low", "eu-p2-icon-sound_mute", "eu-p2-icon-sound_none", "eu-p2-icon-sound_out", "eu-p2-icon-spellcheck", "eu-p2-icon-sport_8ball", "eu-p2-icon-sport_basketball", "eu-p2-icon-sport_football", "eu-p2-icon-sport_golf", "eu-p2-icon-sport_golf_practice", "eu-p2-icon-sport_raquet", "eu-p2-icon-sport_shuttlecock", "eu-p2-icon-sport_soccer", "eu-p2-icon-sport_tennis", "eu-p2-icon-star", "eu-p2-icon-star_bronze", "eu-p2-icon-star_bronze_half_grey", "eu-p2-icon-star_gold", "eu-p2-icon-star_gold_half_grey", "eu-p2-icon-star_gold_half_silver", "eu-p2-icon-star_grey", "eu-p2-icon-star_half_grey", "eu-p2-icon-star_silver", "eu-p2-icon-status_away", "eu-p2-icon-status_be_right_back", "eu-p2-icon-status_busy", "eu-p2-icon-status_invisible", "eu-p2-icon-status_offline", "eu-p2-icon-status_online", "eu-p2-icon-stop", "eu-p2-icon-stop_blue", "eu-p2-icon-stop_green", "eu-p2-icon-stop_red", "eu-p2-icon-style", "eu-p2-icon-style_add", "eu-p2-icon-style_delete", "eu-p2-icon-style_edit", "eu-p2-icon-style_go", "eu-p2-icon-sum", "eu-p2-icon-tab", "eu-p2-icon-table", "eu-p2-icon-table_add", "eu-p2-icon-table_cell", "eu-p2-icon-table_column", "eu-p2-icon-table_column_add", "eu-p2-icon-table_column_delete", "eu-p2-icon-table_connect", "eu-p2-icon-table_delete", "eu-p2-icon-table_edit", "eu-p2-icon-table_error", "eu-p2-icon-table_gear", "eu-p2-icon-table_go", "eu-p2-icon-table_key", "eu-p2-icon-table_lightning", "eu-p2-icon-table_link", "eu-p2-icon-table_multiple", "eu-p2-icon-table_refresh", "eu-p2-icon-table_relationship", "eu-p2-icon-table_row", "eu-p2-icon-table_row_delete", "eu-p2-icon-table_row_insert", "eu-p2-icon-table_save", "eu-p2-icon-table_sort", "eu-p2-icon-tab_add", "eu-p2-icon-tab_blue", "eu-p2-icon-tab_delete", "eu-p2-icon-tab_edit", "eu-p2-icon-tab_go", "eu-p2-icon-tab_green", "eu-p2-icon-tab_red", "eu-p2-icon-tag", "eu-p2-icon-tags_grey", "eu-p2-icon-tags_red", "eu-p2-icon-tag_blue", "eu-p2-icon-tag_blue_add", "eu-p2-icon-tag_blue_delete", "eu-p2-icon-tag_blue_edit", "eu-p2-icon-tag_green", "eu-p2-icon-tag_orange", "eu-p2-icon-tag_pink", "eu-p2-icon-tag_purple", "eu-p2-icon-tag_red", "eu-p2-icon-tag_yellow", "eu-p2-icon-telephone", "eu-p2-icon-telephone_add", "eu-p2-icon-telephone_delete", "eu-p2-icon-telephone_edit", "eu-p2-icon-telephone_error", "eu-p2-icon-telephone_go", "eu-p2-icon-telephone_key", "eu-p2-icon-telephone_link", "eu-p2-icon-telephone_red", "eu-p2-icon-television", "eu-p2-icon-television_add", "eu-p2-icon-television_delete", "eu-p2-icon-television_in", "eu-p2-icon-television_off", "eu-p2-icon-television_out", "eu-p2-icon-television_star", "eu-p2-icon-textfield", "eu-p2-icon-textfield_add", "eu-p2-icon-textfield_delete", "eu-p2-icon-textfield_key", "eu-p2-icon-textfield_rename", "eu-p2-icon-text_ab", "eu-p2-icon-text_align_center", "eu-p2-icon-text_align_justify", "eu-p2-icon-text_align_left", 
                            "eu-p2-icon-text_align_right", "eu-p2-icon-text_allcaps", "eu-p2-icon-text_bold", "eu-p2-icon-text_columns", "eu-p2-icon-text_complete", "eu-p2-icon-text_direction", "eu-p2-icon-text_double_underline", "eu-p2-icon-text_dropcaps", "eu-p2-icon-text_fit", "eu-p2-icon-text_flip", "eu-p2-icon-text_font_default", "eu-p2-icon-text_heading_1", "eu-p2-icon-text_heading_2", "eu-p2-icon-text_heading_3", "eu-p2-icon-text_heading_4", "eu-p2-icon-text_heading_5", "eu-p2-icon-text_heading_6", "eu-p2-icon-text_horizontalrule", "eu-p2-icon-text_indent", "eu-p2-icon-text_indent_remove", "eu-p2-icon-text_inverse", "eu-p2-icon-text_italic", "eu-p2-icon-text_kerning", "eu-p2-icon-text_left_to_right", "eu-p2-icon-text_letterspacing", "eu-p2-icon-text_letter_omega", "eu-p2-icon-text_linespacing", "eu-p2-icon-text_list_bullets", "eu-p2-icon-text_list_numbers", "eu-p2-icon-text_lowercase", "eu-p2-icon-text_lowercase_a", "eu-p2-icon-text_mirror", "eu-p2-icon-text_padding_bottom", "eu-p2-icon-text_padding_left", "eu-p2-icon-text_padding_right", "eu-p2-icon-text_padding_top", "eu-p2-icon-text_replace", "eu-p2-icon-text_right_to_left", "eu-p2-icon-text_rotate_0", "eu-p2-icon-text_rotate_180", "eu-p2-icon-text_rotate_270", "eu-p2-icon-text_rotate_90", "eu-p2-icon-text_ruler", "eu-p2-icon-text_shading", "eu-p2-icon-text_signature", "eu-p2-icon-text_smallcaps", "eu-p2-icon-text_spelling", "eu-p2-icon-text_strikethrough", "eu-p2-icon-text_subscript", "eu-p2-icon-text_superscript", "eu-p2-icon-text_tab", "eu-p2-icon-text_underline", "eu-p2-icon-text_uppercase", "eu-p2-icon-theme", "eu-p2-icon-thumb_down", "eu-p2-icon-thumb_up", "eu-p2-icon-tick", "eu-p2-icon-time", "eu-p2-icon-timeline_marker", "eu-p2-icon-time_add", "eu-p2-icon-time_delete", "eu-p2-icon-time_go", "eu-p2-icon-time_green", "eu-p2-icon-time_red", "eu-p2-icon-tree", "eu-p2-icon-tree32", "eu-p2-icon-node_tree", "eu-p2-icon-node_tree32", "eu-p2-icon-transmit", "eu-p2-icon-transmit_add", "eu-p2-icon-transmit_blue", "eu-p2-icon-transmit_delete", "eu-p2-icon-transmit_edit", "eu-p2-icon-transmit_error", "eu-p2-icon-transmit_go", "eu-p2-icon-transmit_red", "eu-p2-icon-tux", "eu-p2-icon-user", "eu-p2-icon-user_add", "eu-p2-icon-user_alert", "eu-p2-icon-user_b", "eu-p2-icon-user_brown", "eu-p2-icon-user_comment", "eu-p2-icon-user_cross", "eu-p2-icon-user_delete", "eu-p2-icon-user_earth", "eu-p2-icon-user_edit", "eu-p2-icon-user_female", "eu-p2-icon-user_go", "eu-p2-icon-user_gray", "eu-p2-icon-user_gray_cool", "eu-p2-icon-user_green", "eu-p2-icon-user_home", "eu-p2-icon-user_key", "eu-p2-icon-user_magnify", "eu-p2-icon-user_mature", "eu-p2-icon-user_orange", "eu-p2-icon-user_red", "eu-p2-icon-user_star", "eu-p2-icon-user_suit", "eu-p2-icon-user_suit_black", "eu-p2-icon-user_tick", "eu-p2-icon-vcard", "eu-p2-icon-vcard_add", "eu-p2-icon-vcard_delete", "eu-p2-icon-vcard_edit", "eu-p2-icon-vcard_key", "eu-p2-icon-vector", "eu-p2-icon-vector_add", "eu-p2-icon-vector_delete", "eu-p2-icon-vector_key", "eu-p2-icon-wand", "eu-p2-icon-weather_cloud", "eu-p2-icon-weather_clouds", "eu-p2-icon-weather_cloudy", "eu-p2-icon-weather_cloudy_rain", "eu-p2-icon-weather_lightning", "eu-p2-icon-weather_rain", "eu-p2-icon-weather_snow", "eu-p2-icon-weather_sun", "eu-p2-icon-webcam", "eu-p2-icon-webcam_add", "eu-p2-icon-webcam_connect", "eu-p2-icon-webcam_delete", "eu-p2-icon-webcam_error", "eu-p2-icon-webcam_start", "eu-p2-icon-webcam_stop", "eu-p2-icon-world", "eu-p2-icon-world_add", "eu-p2-icon-world_connect", "eu-p2-icon-world_dawn", "eu-p2-icon-world_delete", "eu-p2-icon-world_edit", "eu-p2-icon-world_go", "eu-p2-icon-world_key", "eu-p2-icon-world_link", "eu-p2-icon-world_night", "eu-p2-icon-world_orbit", "eu-p2-icon-wrench", "eu-p2-icon-wrench_orange", "eu-p2-icon-xhtml", "eu-p2-icon-xhtml_add", "eu-p2-icon-xhtml_delete", "eu-p2-icon-xhtml_error", "eu-p2-icon-xhtml_go", "eu-p2-icon-xhtml_valid", "eu-p2-icon-zoom", "eu-p2-icon-zoom_in", "eu-p2-icon-zoom_out", "eu-p2-icon-ext-chm", "eu-p2-icon-ext-doc", "eu-p2-icon-ext-exe", "eu-p2-icon-ext-jpg", "eu-p2-icon-ext-pdf", "eu-p2-icon-ext-ppt", "eu-p2-icon-ext-rar", "eu-p2-icon-ext-txt", "eu-p2-icon-ext-xls", "eu-p2-icon-ext-empty", "eu-p2-icon-upload", "eu-p2-icon-screen_full", "eu-p2-icon-screen_actual", "eu-p2-icon-writing32", "eu-p2-icon-settings32", "eu-p2-icon-chk_checked", "eu-p2-icon-chk_unchecked" };
                        List<string> iconUrls = new List<string>() { "icons/add_other.png", "icons/edit.gif", "icons/edit_remove.png", "icons/edit-clear.png", "icons/disk.png", "icons/deletered.png", "icons/delete.gif", "icons/delete.png", "icons/cross_octagon.png", "icons/delete3.png", "icons/database.png", "icons/download.png", "icons/key.png", "icons/search.png", "icons/refresh.png", "icons/ok.png", "icons/cancel.png", "icons/export.png", "icons/tick_shield.png", "icons/expand-all.gif", "icons/collapse-all.gif", "icons/advancedsettings.png", "icons/advancedsettings2.png", "icons/application_view_list.png", "icons/sys.png", "icons/application_side_tree.png", "icons/users.png", "icons/undo.png", "icons/user_group.png", "icons/group32.png", "icons/group_add.png", "icons/group_delete.png", "icons/user.png", "icons/personal.png", "icons/user_edit32.png", "icons/user_accept16.png", "icons/user_accept32.png", "icons/user_reject16.png", "icons/user_reject32.png", "icons/user_business_boss.png", "icons/stop.png", "icons/error.png", "icons/organization.png", "icons/org32.png", "icons/clock.png", "icons/menu_rightarrow.png", "icons/accept.png", "icons/anchor.png", "icons/application.png", "icons/application_add.png", "icons/application_cascade.png", "icons/application_delete.png", "icons/application_double.png", "icons/application_edit.png", "icons/application_error.png", "icons/application_form.png", "icons/application_form_add.png", "icons/application_form_delete.png", "icons/application_form_edit.png", "icons/application_form_magnify.png", "icons/application_get.png", "icons/application_go.png", "icons/application_home.png", "icons/application_key.png", "icons/application_lightning.png", "icons/application_link.png", "icons/application_osx.png", "icons/application_osx_add.png", "icons/application_osx_cascade.png", "icons/application_osx_delete.png", "icons/application_osx_double.png", "icons/application_osx_error.png", "icons/application_osx_get.png", "icons/application_osx_go.png", "icons/application_osx_home.png", "icons/application_osx_key.png", "icons/application_osx_lightning.png", "icons/application_osx_link.png", "icons/application_osx_split.png", "icons/application_osx_start.png", "icons/application_osx_stop.png", "icons/application_osx_terminal.png", "icons/application_put.png", "icons/application_side_boxes.png", "icons/application_side_contract.png", "icons/application_side_expand.png", "icons/application_side_list.png", "icons/application_side_tree.png", "icons/application_split.png", "icons/application_start.png", "icons/application_stop.png", "icons/application_tile_horizontal.png", "icons/application_tile_vertical.png", "icons/application_view_columns.png", "icons/application_view_detail.png", "icons/application_view_gallery.png", "icons/application_view_icons.png", "icons/application_view_list.png", "icons/application_view_tile.png", "icons/application_xp.png", "icons/application_xp_terminal.png", "icons/arrow_branch.png", "icons/arrow_divide.png", "icons/arrow_down.png", "icons/arrow_ew.png", "icons/arrow_in_2.png", "icons/arrow_inout.png", "icons/arrow_in_longer.png", "icons/arrow_join.png", "icons/arrow_left.png", "icons/arrow_merge.png", "icons/arrow_ne.png", "icons/arrow_ns.png", "icons/arrow_nsew.png", "icons/arrow_nw.png", "icons/arrow_nw_ne_sw_se.png", "icons/arrow_nw_se.png", "icons/arrow_out_2.png", "icons/arrow_out_longer.png", "icons/arrow_redo.png", "icons/arrow_refresh.png", "icons/arrow_refresh_small.png", "icons/arrow_right.png", "icons/arrow_right_16.png", "icons/arrow_rotate_anticlockwise.png", "icons/arrow_rotate_clockwise.png", "icons/arrow_se.png", "icons/arrow_sw.png", "icons/arrow_switch.png", "icons/arrow_switch_bluegreen.png", "icons/arrow_sw_ne.png", "icons/arrow_turn_left.png", "icons/arrow_turn_right.png", "icons/arrow_undo.png", "icons/arrow_up.png", "icons/asterisk_orange.png", "icons/asterisk_red.png", "icons/asterisk_yellow.png", "icons/attach.png", "icons/award_star_add.png", "icons/award_star_bronze_1.png", "icons/award_star_bronze_2.png", "icons/award_star_bronze_3.png", "icons/award_star_delete.png", "icons/award_star_gold_1.png", "icons/award_star_gold_2.png", "icons/award_star_gold_3.png", "icons/award_star_silver_1.png", "icons/award_star_silver_2.png", "icons/award_star_silver_3.png", "icons/basket.png", "icons/basket_add.png", "icons/basket_delete.png", "icons/basket_edit.png", "icons/basket_error.png", "icons/basket_go.png", "icons/basket_put.png", "icons/basket_remove.png", "icons/bell.png", "icons/bell_add.png", "icons/bell_delete.png", "icons/bell_error.png", "icons/bell_go.png", "icons/bell_link.png", "icons/bell_silver.png", "icons/bell_silver_start.png", "icons/bell_silver_stop.png", "icons/bell_start.png", "icons/bell_stop.png", "icons/bin.png", "icons/bin_closed.png", "icons/bin_empty.png", "icons/bomb.png", "icons/book.png", "icons/bookmark.png", "icons/bookmark_add.png", "icons/bookmark_delete.png", "icons/bookmark_edit.png", "icons/bookmark_error.png", "icons/bookmark_go.png", "icons/book_add.png", "icons/book_addresses.png", "icons/book_addresses_add.png", "icons/book_addresses_delete.png", "icons/book_addresses_edit.png", "icons/book_addresses_error.png", "icons/book_addresses_key.png", "icons/book_delete.png", "icons/book_edit.png", "icons/book_error.png", "icons/book_go.png", "icons/book_key.png", "icons/book_link.png", "icons/book_magnify.png", "icons/book_next.png", "icons/book_open.png", "icons/book_open_mark.png", "icons/book_previous.png", "icons/book_red.png", "icons/book_tabs.png", "icons/border_all.png", "icons/border_bottom.png", "icons/border_draw.png", "icons/border_inner.png", "icons/border_inner_horizontal.png", "icons/border_inner_vertical.png", "icons/border_left.png", "icons/border_none.png", "icons/border_outer.png", "icons/border_right.png", "icons/border_top.png", "icons/box.png", "icons/box_error.png", "icons/box_picture.png", "icons/box_world.png", "icons/brick.png", "icons/bricks.png", "icons/brick_add.png", "icons/brick_delete.png", "icons/brick_edit.png", "icons/brick_error.png", "icons/brick_go.png", "icons/brick_link.png", "icons/brick_magnify.png", "icons/briefcase.png", "icons/bug.png", "icons/bug_add.png", "icons/bug_delete.png", "icons/bug_edit.png", "icons/bug_error.png", "icons/bug_fix.png", "icons/bug_go.png", "icons/bug_link.png", "icons/bug_magnify.png", "icons/build.png", "icons/building.png", "icons/building_add.png", "icons/building_delete.png", "icons/building_edit.png", "icons/building_error.png", "icons/building_go.png", "icons/building_key.png", "icons/building_link.png", "icons/build_cancel.png", "icons/bullet_add.png", "icons/bullet_arrow_bottom.png", "icons/bullet_arrow_down.png", "icons/bullet_arrow_top.png", "icons/bullet_arrow_up.png", "icons/bullet_black.png", "icons/bullet_blue.png", "icons/bullet_connect.png", "icons/bullet_cross.png", "icons/bullet_database.png", "icons/bullet_database_yellow.png", "icons/bullet_delete.png", "icons/bullet_disk.png", "icons/bullet_earth.png", "icons/bullet_edit.png", "icons/bullet_eject.png", "icons/bullet_error.png", "icons/bullet_feed.png", "icons/bullet_get.png", "icons/bullet_go.png", "icons/bullet_green.png", "icons/bullet_home.png", "icons/bullet_key.png", "icons/bullet_left.png", "icons/bullet_lightning.png", "icons/bullet_magnify.png", "icons/bullet_minus.png", "icons/bullet_orange.png", "icons/bullet_page_white.png", "icons/bullet_picture.png", "icons/bullet_pink.png", "icons/bullet_plus.png", "icons/bullet_purple.png", "icons/bullet_red.png", "icons/bullet_right.png", "icons/bullet_shape.png", 
                            "icons/bullet_sparkle.png", "icons/bullet_star.png", "icons/bullet_start.png", "icons/bullet_stop.png", "icons/bullet_stop_alt.png", "icons/bullet_tick.png", "icons/bullet_toggle_minus.png", "icons/bullet_toggle_plus.png", "icons/bullet_white.png", "icons/bullet_wrench.png", "icons/bullet_wrench_red.png", "icons/bullet_yellow.png", "icons/button.png", "icons/cake.png", "icons/cake_out.png", "icons/cake_sliced.png", "icons/calculator.png", "icons/calculator_add.png", "icons/calculator_delete.png", "icons/calculator_edit.png", "icons/calculator_error.png", "icons/calculator_link.png", "icons/calendar_2.png", "icons/calendar_add.png", "icons/calendar_delete.png", "icons/calendar_edit.png", "icons/calendar_link.png", "icons/calendar_select_day.png", "icons/calendar_select_none.png", "icons/calendar_select_week.png", "icons/calendar_star.png", "icons/calendar_view_day.png", "icons/calendar_view_month.png", "icons/calendar_view_week.png", "icons/camera.png", "icons/camera_add.png", "icons/camera_connect.png", "icons/camera_delete.png", "icons/camera_edit.png", "icons/camera_error.png", "icons/camera_go.png", "icons/camera_link.png", "icons/camera_magnify.png", "icons/camera_picture.png", "icons/camera_small.png", "icons/camera_start.png", "icons/camera_stop.png", "icons/cancel.png", "icons/car.png", "icons/cart.png", "icons/cart_add.png", "icons/cart_delete.png", "icons/cart_edit.png", "icons/cart_error.png", "icons/cart_full.png", "icons/cart_go.png", "icons/cart_magnify.png", "icons/cart_put.png", "icons/cart_remove.png", "icons/car_add.png", "icons/car_delete.png", "icons/car_error.png", "icons/car_red.png", "icons/car_start.png", "icons/car_stop.png", "icons/cd.png", "icons/cdr.png", "icons/cdr_add.png", "icons/cdr_burn.png", "icons/cdr_cross.png", "icons/cdr_delete.png", "icons/cdr_edit.png", "icons/cdr_eject.png", "icons/cdr_error.png", "icons/cdr_go.png", "icons/cdr_magnify.png", "icons/cdr_play.png", "icons/cdr_start.png", "icons/cdr_stop.png", "icons/cdr_stop_alt.png", "icons/cdr_tick.png", "icons/cd_add.png", "icons/cd_burn.png", "icons/cd_delete.png", "icons/cd_edit.png", "icons/cd_eject.png", "icons/cd_go.png", "icons/cd_magnify.png", "icons/cd_play.png", "icons/cd_stop.png", "icons/cd_stop_alt.png", "icons/cd_tick.png", "icons/chart_bar.png", "icons/chart_bar_add.png", "icons/chart_bar_delete.png", "icons/chart_bar_edit.png", "icons/chart_bar_error.png", "icons/chart_bar_link.png", "icons/chart_curve.png", "icons/chart_curve_add.png", "icons/chart_curve_delete.png", "icons/chart_curve_edit.png", "icons/chart_curve_error.png", "icons/chart_curve_go.png", "icons/chart_curve_link.png", "icons/chart_line.png", "icons/chart_line_add.png", "icons/chart_line_delete.png", "icons/chart_line_edit.png", "icons/chart_line_error.png", "icons/chart_line_link.png", "icons/chart_organisation.png", "icons/chart_organisation_add.png", "icons/chart_organisation_delete.png", "icons/chart_org_inverted.png", "icons/chart_pie.png", "icons/chart_pie_add.png", "icons/chart_pie_delete.png", "icons/chart_pie_edit.png", "icons/chart_pie_error.png", "icons/chart_pie_lightning.png", "icons/chart_pie_link.png", "icons/check_error.png", "icons/clipboard.png", "icons/clock.png", "icons/clock_add.png", "icons/clock_delete.png", "icons/clock_edit.png", "icons/clock_error.png", "icons/clock_go.png", "icons/clock_link.png", "icons/clock_pause.png", "icons/clock_play.png", "icons/clock_red.png", "icons/clock_start.png", "icons/clock_stop.png", "icons/cmy.png", "icons/cog.png", "icons/cog_add.png", "icons/cog_delete.png", "icons/cog_edit.png", "icons/cog_error.png", "icons/cog_go.png", "icons/cog_start.png", "icons/cog_stop.png", "icons/coins.png", "icons/coins_add.png", "icons/coins_delete.png", "icons/color.png", "icons/color_swatch.png", "icons/color_wheel.png", "icons/comment.png", "icons/comments.png", "icons/comments_add.png", "icons/comments_delete.png", "icons/comment_add.png", "icons/comment_delete.png", "icons/comment_dull.png", "icons/comment_edit.png", "icons/comment_play.png", "icons/comment_record.png", "icons/compass.png", "icons/compress.png", "icons/computer.png", "icons/computer_add.png", "icons/computer_connect.png", "icons/computer_delete.png", "icons/computer_edit.png", "icons/computer_error.png", "icons/computer_go.png", "icons/computer_key.png", "icons/computer_link.png", "icons/computer_magnify.png", "icons/computer_off.png", "icons/computer_start.png", "icons/computer_stop.png", "icons/computer_wrench.png", "icons/connect.png", "icons/contrast.png", "icons/contrast_decrease.png", "icons/contrast_high.png", "icons/contrast_increase.png", "icons/contrast_low.png", "icons/controller.png", "icons/controller_add.png", "icons/controller_delete.png", "icons/controller_error.png", "icons/control_add.png", "icons/control_add_blue.png", "icons/control_blank.png", "icons/control_blank_blue.png", "icons/control_eject.png", "icons/control_eject_blue.png", "icons/control_end.png", "icons/control_end_blue.png", "icons/control_equalizer.png", "icons/control_equalizer_blue.png", "icons/control_fastforward.png", "icons/control_fastforward_blue.png", "icons/control_pause.png", "icons/control_pause_blue.png", "icons/control_play.png", "icons/control_play_blue.png", "icons/control_power.png", "icons/control_power_blue.png", "icons/control_record.png", "icons/control_record_blue.png", "icons/control_remove.png", "icons/control_remove_blue.png", "icons/control_repeat.png", "icons/control_repeat_blue.png", "icons/control_rewind.png", "icons/control_rewind_blue.png", "icons/control_start.png", "icons/control_start_blue.png", "icons/control_stop.png", "icons/control_stop_blue.png", "icons/creditcards.png", "icons/cross.png", "icons/css.png", "icons/css_add.png", "icons/css_delete.png", "icons/css_error.png", "icons/css_go.png", "icons/css_valid.png", "icons/cup.png", "icons/cup_add.png", "icons/cup_black.png", "icons/cup_delete.png", "icons/cup_edit.png", "icons/cup_error.png", "icons/cup_go.png", "icons/cup_green.png", "icons/cup_key.png", "icons/cup_link.png", "icons/cup_tea.png", "icons/cursor.png", "icons/cursor_small.png", "icons/cut.png", "icons/cut_red.png", "icons/database.png", "icons/database_add.png", "icons/database_connect.png", "icons/database_copy.png", "icons/database_delete.png", "icons/database_edit.png", "icons/database_error.png", "icons/database_gear.png", "icons/database_go.png", "icons/database_key.png", "icons/database_lightning.png", "icons/database_link.png", "icons/database_refresh.png", "icons/database_save.png", "icons/database_start.png", "icons/database_stop.png", "icons/database_table.png", "icons/database_wrench.png", "icons/database_yellow.png", "icons/database_yellow_start.png", "icons/database_yellow_stop.png", "icons/date.png", "icons/date_add.png", "icons/date_delete.png", "icons/date_edit.png", "icons/date_error.png", "icons/date_go.png", "icons/date_link.png", "icons/date_magnify.png", "icons/date_next.png", "icons/date_previous.png", "icons/decline.png", "icons/delete.png", "icons/device_stylus.png", "icons/disconnect.png", "icons/disk.png", "icons/disk_black.png", "icons/disk_black_error.png", "icons/disk_black_magnify.png", "icons/disk_download.png", "icons/disk_edit.png", "icons/disk_error.png", "icons/disk_magnify.png", "icons/disk_multiple.png", "icons/disk_upload.png", "icons/door.png", "icons/door_error.png", "icons/door_in.png", "icons/door_open.png", "icons/door_out.png", "icons/drink.png", "icons/drink_empty.png", "icons/drink_red.png", "icons/drive.png", "icons/drive_add.png", "icons/drive_burn.png", "icons/drive_cd.png", "icons/drive_cdr.png", 
                            "icons/drive_cd_empty.png", "icons/drive_delete.png", "icons/drive_disk.png", "icons/drive_edit.png", "icons/drive_error.png", "icons/drive_go.png", "icons/drive_key.png", "icons/drive_link.png", "icons/drive_magnify.png", "icons/drive_network.png", "icons/drive_network_error.png", "icons/drive_network_stop.png", "icons/drive_rename.png", "icons/drive_user.png", "icons/drive_web.png", "icons/dvd.png", "icons/dvd_add.png", "icons/dvd_delete.png", "icons/dvd_edit.png", "icons/dvd_error.png", "icons/dvd_go.png", "icons/dvd_key.png", "icons/dvd_link.png", "icons/dvd_start.png", "icons/dvd_stop.png", "icons/eject_blue.png", "icons/eject_green.png", "icons/email_2.png", "icons/email_add.png", "icons/email_attach.png", "icons/email_delete.png", "icons/email_edit.png", "icons/email_error.png", "icons/email_go.png", "icons/email_link.png", "icons/email_magnify.png", "icons/email_open.png", "icons/email_open_image.png", "icons/email_star.png", "icons/email_start.png", "icons/email_stop.png", "icons/email_transfer.png", "icons/emoticon_evilgrin.png", "icons/emoticon_grin.png", "icons/emoticon_happy.png", "icons/emoticon_smile.png", "icons/emoticon_surprised.png", "icons/emoticon_tongue.png", "icons/emoticon_unhappy.png", "icons/emoticon_waii.png", "icons/emoticon_wink.png", "icons/erase.png", "icons/error.png", "icons/error_add.png", "icons/error_delete.png", "icons/error_go.png", "icons/exclamation.png", "icons/eye.png", "icons/eyes.png", "icons/feed.png", "icons/feed_add.png", "icons/feed_delete.png", "icons/feed_disk.png", "icons/feed_edit.png", "icons/feed_error.png", "icons/feed_go.png", "icons/feed_key.png", "icons/feed_link.png", "icons/feed_magnify.png", "icons/feed_star.png", "icons/female.png", "icons/film.png", "icons/film_add.png", "icons/film_delete.png", "icons/film_edit.png", "icons/film_eject.png", "icons/film_error.png", "icons/film_go.png", "icons/film_key.png", "icons/film_link.png", "icons/film_magnify.png", "icons/film_save.png", "icons/film_star.png", "icons/film_start.png", "icons/film_stop.png", "icons/find.png", "icons/finger_point.png", "icons/flag_black.png", "icons/flag_blue.png", "icons/flag_checked.png", "icons/flag_france.png", "icons/flag_green.png", "icons/flag_grey.png", "icons/flag_orange.png", "icons/flag_pink.png", "icons/flag_purple.png", "icons/flag_red.png", "icons/flag_white.png", "icons/flag_yellow.png", "icons/flower_daisy.png", "icons/folder_2.png", "icons/folder_add.png", "icons/folder_bell.png", "icons/folder_bookmark.png", "icons/folder_brick.png", "icons/folder_bug.png", "icons/folder_camera.png", "icons/folder_connect.png", "icons/folder_database.png", "icons/folder_delete.png", "icons/folder_edit.png", "icons/folder_error.png", "icons/folder_explore.png", "icons/folder_feed.png", "icons/folder_film.png", "icons/folder_find.png", "icons/folder_font.png", "icons/folder_go.png", "icons/folder_heart.png", "icons/folder_home.png", "icons/folder_image.png", "icons/folder_key.png", "icons/folder_lightbulb.png", "icons/folder_link.png", "icons/folder_magnify.png", "icons/folder_page.png", "icons/folder_page_white.png", "icons/folder_palette.png", "icons/folder_picture.png", "icons/folder_star.png", "icons/folder_table.png", "icons/folder_up.png", "icons/folder_user.png", "icons/folder_wrench.png", "icons/font.png", "icons/font_add.png", "icons/font_color.png", "icons/font_delete.png", "icons/font_go.png", "icons/font_larger.png", "icons/font_smaller.png", "icons/forward_blue.png", "icons/forward_green.png", "icons/group.png", "icons/group_add.png", "icons/group_delete.png", "icons/group_edit.png", "icons/group_error.png", "icons/group_gear.png", "icons/group_go.png", "icons/group_key.png", "icons/group_link.png", "icons/heart.png", "icons/heart_add.png", "icons/heart_broken.png", "icons/heart_connect.png", "icons/heart_delete.png", "icons/help.png", "icons/hourglass.png", "icons/hourglass_add.png", "icons/hourglass_delete.png", "icons/hourglass_go.png", "icons/hourglass_link.png", "icons/house.png", "icons/house_connect.png", "icons/house_go.png", "icons/house_in.png", "icons/house_key.png", "icons/house_link.png", "icons/house_star.png", "icons/html.png", "icons/html_add.png", "icons/html_delete.png", "icons/html_error.png", "icons/html_go.png", "icons/html_valid.png", "icons/image.png", "icons/images.png", "icons/image_add.png", "icons/image_delete.png", "icons/image_edit.png", "icons/image_link.png", "icons/image_magnify.png", "icons/image_star.png", "icons/information.png", "icons/ipod.png", "icons/ipod_cast.png", "icons/ipod_cast_add.png", "icons/ipod_cast_delete.png", "icons/ipod_connect.png", "icons/ipod_nano.png", "icons/ipod_nano_connect.png", "icons/ipod_sound.png", "icons/joystick.png", "icons/joystick_add.png", "icons/joystick_connect.png", "icons/joystick_delete.png", "icons/joystick_error.png", "icons/key.png", "icons/keyboard.png", "icons/keyboard_add.png", "icons/keyboard_connect.png", "icons/keyboard_delete.png", "icons/keyboard_magnify.png", "icons/key_add.png", "icons/key_delete.png", "icons/key_go.png", "icons/key_start.png", "icons/key_stop.png", "icons/laptop.png", "icons/laptop_add.png", "icons/laptop_connect.png", "icons/laptop_delete.png", "icons/laptop_disk.png", "icons/laptop_edit.png", "icons/laptop_error.png", "icons/laptop_go.png", "icons/laptop_key.png", "icons/laptop_link.png", "icons/laptop_magnify.png", "icons/laptop_start.png", "icons/laptop_stop.png", "icons/laptop_wrench.png", "icons/layers.png", "icons/layout.png", "icons/layout_add.png", "icons/layout_content.png", "icons/layout_delete.png", "icons/layout_edit.png", "icons/layout_error.png", "icons/layout_header.png", "icons/layout_key.png", "icons/layout_lightning.png", "icons/layout_link.png", "icons/layout_sidebar.png", "icons/lightbulb.png", "icons/lightbulb_add.png", "icons/lightbulb_delete.png", "icons/lightbulb_off.png", "icons/lightning.png", "icons/lightning_add.png", "icons/lightning_delete.png", "icons/lightning_go.png", "icons/link.png", "icons/link_add.png", "icons/link_break.png", "icons/link_delete.png", "icons/link_edit.png", "icons/link_error.png", "icons/link_go.png", "icons/lock_2.png", "icons/lock_add.png", "icons/lock_break.png", "icons/lock_delete.png", "icons/lock_edit.png", "icons/lock_go.png", "icons/lock_key.png", "icons/lock_open.png", "icons/lock_start.png", "icons/lock_stop.png", "icons/lorry.png", "icons/lorry_add.png", "icons/lorry_delete.png", "icons/lorry_error.png", "icons/lorry_flatbed.png", "icons/lorry_go.png", "icons/lorry_link.png", "icons/lorry_start.png", "icons/lorry_stop.png", "icons/magifier_zoom_out.png", "icons/magnifier.png", "icons/magnifier_zoom_in.png", "icons/mail.png", "icons/male.png", "icons/map.png", "icons/map_add.png", "icons/map_clipboard.png", "icons/map_cursor.png", "icons/map_delete.png", "icons/map_edit.png", "icons/map_error.png", "icons/map_go.png", "icons/map_link.png", "icons/map_magnify.png", "icons/map_start.png", "icons/map_stop.png", "icons/medal_bronze_1.png", "icons/medal_bronze_2.png", "icons/medal_bronze_3.png", "icons/medal_bronze_add.png", "icons/medal_bronze_delete.png", "icons/medal_gold_1.png", "icons/medal_gold_2.png", "icons/medal_gold_3.png", "icons/medal_gold_add.png", "icons/medal_gold_delete.png", "icons/medal_silver_1.png", "icons/medal_silver_2.png", "icons/medal_silver_3.png", "icons/medal_silver_add.png", "icons/medal_silver_delete.png", "icons/money.png", "icons/money_add.png", "icons/money_delete.png", "icons/money_dollar.png", "icons/money_euro.png", "icons/money_pound.png", "icons/money_yen.png", "icons/monitor.png", "icons/monitor_add.png", "icons/monitor_delete.png", 
                            "icons/monitor_edit.png", "icons/monitor_error.png", "icons/monitor_go.png", "icons/monitor_key.png", "icons/monitor_lightning.png", "icons/monitor_link.png", "icons/moon_full.png", "icons/mouse.png", "icons/mouse_add.png", "icons/mouse_delete.png", "icons/mouse_error.png", "icons/music.png", "icons/music_note.png", "icons/neighbourhood.png", "icons/new.png", "icons/newspaper.png", "icons/newspaper_add.png", "icons/newspaper_delete.png", "icons/newspaper_go.png", "icons/newspaper_link.png", "icons/new_blue.png", "icons/new_red.png", "icons/next.png", "icons/next-green.png", "icons/next_blue.png", "icons/next_green.png", "icons/note.png", "icons/note_add.png", "icons/note_delete.png", "icons/note_edit.png", "icons/note_error.png", "icons/note_go.png", "icons/outline.png", "icons/overlays.png", "icons/package.png", "icons/package_add.png", "icons/package_delete.png", "icons/package_down.png", "icons/package_go.png", "icons/package_green.png", "icons/package_in.png", "icons/package_link.png", "icons/package_se.png", "icons/package_start.png", "icons/package_stop.png", "icons/package_white.png", "icons/page.png", "icons/page_add.png", "icons/page_attach.png", "icons/page_back.png", "icons/page_break.png", "icons/page_break_insert.png", "icons/page_cancel.png", "icons/page_code.png", "icons/page_copy.png", "icons/page_delete.png", "icons/page_edit.png", "icons/page_error.png", "icons/page_excel.png", "icons/page_find.png", "icons/page_forward.png", "icons/page_gear.png", "icons/page_go.png", "icons/page_green.png", "icons/page_header_footer.png", "icons/page_key.png", "icons/page_landscape.png", "icons/page_landscape_shot.png", "icons/page_lightning.png", "icons/page_link.png", "icons/page_magnify.png", "icons/page_paintbrush.png", "icons/page_paste.png", "icons/page_portrait.png", "icons/page_portrait_shot.png", "icons/page_red.png", "icons/page_refresh.png", "icons/page_save.png", "icons/page_white.png", "icons/page_white_acrobat.png", "icons/page_white_actionscript.png", "icons/page_white_add.png", "icons/page_white_break.png", "icons/page_white_c.png", "icons/page_white_camera.png", "icons/page_white_cd.png", "icons/page_white_cdr.png", "icons/page_white_code.png", "icons/page_white_code_red.png", "icons/page_white_coldfusion.png", "icons/page_white_compressed.png", "icons/page_white_connect.png", "icons/page_white_copy.png", "icons/page_white_cplusplus.png", "icons/page_white_csharp.png", "icons/page_white_cup.png", "icons/page_white_database.png", "icons/page_white_database_yellow.png", "icons/page_white_delete.png", "icons/page_white_dvd.png", "icons/page_white_edit.png", "icons/page_white_error.png", "icons/page_white_excel.png", "icons/page_white_find.png", "icons/page_white_flash.png", "icons/page_white_font.png", "icons/page_white_freehand.png", "icons/page_white_gear.png", "icons/page_white_get.png", "icons/page_white_go.png", "icons/page_white_h.png", "icons/page_white_horizontal.png", "icons/page_white_key.png", "icons/page_white_lightning.png", "icons/page_white_link.png", "icons/page_white_magnify.png", "icons/page_white_medal.png", "icons/page_white_office.png", "icons/page_white_paint.png", "icons/page_white_paintbrush.png", "icons/page_white_paste.png", "icons/page_white_paste_table.png", "icons/page_white_php.png", "icons/page_white_picture.png", "icons/page_white_powerpoint.png", "icons/page_white_put.png", "icons/page_white_refresh.png", "icons/page_white_ruby.png", "icons/page_white_side_by_side.png", "icons/page_white_stack.png", "icons/page_white_star.png", "icons/page_white_swoosh.png", "icons/page_white_text.png", "icons/page_white_text_width.png", "icons/page_white_tux.png", "icons/page_white_vector.png", "icons/page_white_visualstudio.png", "icons/page_white_width.png", "icons/page_white_word.png", "icons/page_white_world.png", "icons/page_white_wrench.png", "icons/page_white_zip.png", "icons/page_word.png", "icons/page_world.png", "icons/paint.png", "icons/paintbrush.png", "icons/paintbrush_color.png", "icons/paintcan.png", "icons/paintcan_red.png", "icons/paint_can_brush.png", "icons/palette.png", "icons/paste_plain.png", "icons/paste_word.png", "icons/pause_blue.png", "icons/pause_green.png", "icons/pause_record.png", "icons/pencil.png", "icons/pencil_add.png", "icons/pencil_delete.png", "icons/pencil_go.png", "icons/phone.png", "icons/phone_add.png", "icons/phone_delete.png", "icons/phone_edit.png", "icons/phone_error.png", "icons/phone_go.png", "icons/phone_key.png", "icons/phone_link.png", "icons/phone_sound.png", "icons/phone_start.png", "icons/phone_stop.png", "icons/photo.png", "icons/photos.png", "icons/photo_add.png", "icons/photo_delete.png", "icons/photo_edit.png", "icons/photo_link.png", "icons/photo_paint.png", "icons/picture.png", "icons/pictures.png", "icons/pictures_thumbs.png", "icons/picture_add.png", "icons/picture_clipboard.png", "icons/picture_delete.png", "icons/picture_edit.png", "icons/picture_empty.png", "icons/picture_error.png", "icons/picture_go.png", "icons/picture_key.png", "icons/picture_link.png", "icons/picture_save.png", "icons/pilcrow.png", "icons/pill.png", "icons/pill_add.png", "icons/pill_delete.png", "icons/pill_error.png", "icons/pill_go.png", "icons/play_blue.png", "icons/play_green.png", "icons/plugin.png", "icons/plugin_add.png", "icons/plugin_delete.png", "icons/plugin_disabled.png", "icons/plugin_edit.png", "icons/plugin_error.png", "icons/plugin_go.png", "icons/plugin_key.png", "icons/plugin_link.png", "icons/previous.png", "icons/previous-green.png", "icons/printer.png", "icons/printer_add.png", "icons/printer_cancel.png", "icons/printer_color.png", "icons/printer_connect.png", "icons/printer_delete.png", "icons/printer_empty.png", "icons/printer_error.png", "icons/printer_go.png", "icons/printer_key.png", "icons/printer_mono.png", "icons/printer_start.png", "icons/printer_stop.png", "icons/rainbow.png", "icons/rainbow_star.png", "icons/record_blue.png", "icons/record_green.png", "icons/record_red.png", "icons/reload.png", "icons/report.png", "icons/report_add.png", "icons/report_delete.png", "icons/report_disk.png", "icons/report_edit.png", "icons/report_go.png", "icons/report_key.png", "icons/report_link.png", "icons/report_magnify.png", "icons/report_picture.png", "icons/report_start.png", "icons/report_stop.png", "icons/report_user.png", "icons/report_word.png", "icons/resultset_first.png", "icons/resultset_last.png", "icons/resultset_next.png", "icons/resultset_previous.png", "icons/reverse_blue.png", "icons/reverse_green.png", "icons/rewind_blue.png", "icons/rewind_green.png", "icons/rgb.png", "icons/rosette.png", "icons/rosette_blue.png", "icons/rss.png", "icons/rss_add.png", "icons/rss_delete.png", "icons/rss_error.png", "icons/rss_go.png", "icons/rss_valid.png", "icons/ruby.png", "icons/ruby_add.png", "icons/ruby_delete.png", "icons/ruby_gear.png", "icons/ruby_get.png", "icons/ruby_go.png", "icons/ruby_key.png", "icons/ruby_link.png", "icons/ruby_put.png", "icons/script.png", "icons/script_add.png", "icons/script_code.png", "icons/script_code_red.png", "icons/script_delete.png", "icons/script_edit.png", "icons/script_error.png", "icons/script_gear.png", "icons/script_go.png", "icons/script_key.png", "icons/script_lightning.png", "icons/script_link.png", "icons/script_palette.png", "icons/script_save.png", "icons/script_start.png", "icons/script_stop.png", "icons/seasons.png", "icons/section_collapsed.png", "icons/section_expanded.png", "icons/server.png", "icons/server_add.png", "icons/server_chart.png", "icons/server_compressed.png", "icons/server_connect.png", "icons/server_database.png", 
                            "icons/server_delete.png", "icons/server_edit.png", "icons/server_error.png", "icons/server_go.png", "icons/server_key.png", "icons/server_lightning.png", "icons/server_link.png", "icons/server_start.png", "icons/server_stop.png", "icons/server_uncompressed.png", "icons/server_wrench.png", "icons/shading.png", "icons/shapes_many.png", "icons/shapes_many_select.png", "icons/shape_3d.png", "icons/shape_align_bottom.png", "icons/shape_align_center.png", "icons/shape_align_left.png", "icons/shape_align_middle.png", "icons/shape_align_right.png", "icons/shape_align_top.png", "icons/shape_flip_horizontal.png", "icons/shape_flip_vertical.png", "icons/shape_group.png", "icons/shape_handles.png", "icons/shape_move_back.png", "icons/shape_move_backwards.png", "icons/shape_move_forwards.png", "icons/shape_move_front.png", "icons/shape_rotate_anticlockwise.png", "icons/shape_rotate_clockwise.png", "icons/shape_shade_a.png", "icons/shape_shade_b.png", "icons/shape_shade_c.png", "icons/shape_shadow.png", "icons/shape_shadow_toggle.png", "icons/shape_square.png", "icons/shape_square_add.png", "icons/shape_square_delete.png", "icons/shape_square_edit.png", "icons/shape_square_error.png", "icons/shape_square_go.png", "icons/shape_square_key.png", "icons/shape_square_link.png", "icons/shape_square_select.png", "icons/shape_ungroup.png", "icons/share.png", "icons/shield.png", "icons/shield_add.png", "icons/shield_delete.png", "icons/shield_error.png", "icons/shield_go.png", "icons/shield_rainbow.png", "icons/shield_silver.png", "icons/shield_start.png", "icons/shield_stop.png", "icons/sitemap.png", "icons/sitemap_color.png", "icons/smartphone.png", "icons/smartphone_add.png", "icons/smartphone_connect.png", "icons/smartphone_delete.png", "icons/smartphone_disk.png", "icons/smartphone_edit.png", "icons/smartphone_error.png", "icons/smartphone_go.png", "icons/smartphone_key.png", "icons/smartphone_wrench.png", "icons/sort_ascending.png", "icons/sort_descending.png", "icons/sound.png", "icons/sound_add.png", "icons/sound_delete.png", "icons/sound_high.png", "icons/sound_in.png", "icons/sound_low.png", "icons/sound_mute.png", "icons/sound_none.png", "icons/sound_out.png", "icons/spellcheck.png", "icons/sport_8ball.png", "icons/sport_basketball.png", "icons/sport_football.png", "icons/sport_golf.png", "icons/sport_golf_practice.png", "icons/sport_raquet.png", "icons/sport_shuttlecock.png", "icons/sport_soccer.png", "icons/sport_tennis.png", "icons/star.png", "icons/star_bronze.png", "icons/star_bronze_half_grey.png", "icons/star_gold.png", "icons/star_gold_half_grey.png", "icons/star_gold_half_silver.png", "icons/star_grey.png", "icons/star_half_grey.png", "icons/star_silver.png", "icons/status_away.png", "icons/status_be_right_back.png", "icons/status_busy.png", "icons/status_invisible.png", "icons/status_offline.png", "icons/status_online.png", "icons/stop.png", "icons/stop_blue.png", "icons/stop_green.png", "icons/stop_red.png", "icons/style.png", "icons/style_add.png", "icons/style_delete.png", "icons/style_edit.png", "icons/style_go.png", "icons/sum.png", "icons/tab.png", "icons/table.png", "icons/table_add.png", "icons/table_cell.png", "icons/table_column.png", "icons/table_column_add.png", "icons/table_column_delete.png", "icons/table_connect.png", "icons/table_delete.png", "icons/table_edit.png", "icons/table_error.png", "icons/table_gear.png", "icons/table_go.png", "icons/table_key.png", "icons/table_lightning.png", "icons/table_link.png", "icons/table_multiple.png", "icons/table_refresh.png", "icons/table_relationship.png", "icons/table_row.png", "icons/table_row_delete.png", "icons/table_row_insert.png", "icons/table_save.png", "icons/table_sort.png", "icons/tab_add.png", "icons/tab_blue.png", "icons/tab_delete.png", "icons/tab_edit.png", "icons/tab_go.png", "icons/tab_green.png", "icons/tab_red.png", "icons/tag.png", "icons/tags_grey.png", "icons/tags_red.png", "icons/tag_blue.png", "icons/tag_blue_add.png", "icons/tag_blue_delete.png", "icons/tag_blue_edit.png", "icons/tag_green.png", "icons/tag_orange.png", "icons/tag_pink.png", "icons/tag_purple.png", "icons/tag_red.png", "icons/tag_yellow.png", "icons/telephone.png", "icons/telephone_add.png", "icons/telephone_delete.png", "icons/telephone_edit.png", "icons/telephone_error.png", "icons/telephone_go.png", "icons/telephone_key.png", "icons/telephone_link.png", "icons/telephone_red.png", "icons/television.png", "icons/television_add.png", "icons/television_delete.png", "icons/television_in.png", "icons/television_off.png", "icons/television_out.png", "icons/television_star.png", "icons/textfield.png", "icons/textfield_add.png", "icons/textfield_delete.png", "icons/textfield_key.png", "icons/textfield_rename.png", "icons/text_ab.png", "icons/text_align_center.png", "icons/text_align_justify.png", "icons/text_align_left.png", "icons/text_align_right.png", "icons/text_allcaps.png", "icons/text_bold.png", "icons/text_columns.png", "icons/text_complete.png", "icons/text_direction.png", "icons/text_double_underline.png", "icons/text_dropcaps.png", "icons/text_fit.png", "icons/text_flip.png", "icons/text_font_default.png", "icons/text_heading_1.png", "icons/text_heading_2.png", "icons/text_heading_3.png", "icons/text_heading_4.png", "icons/text_heading_5.png", "icons/text_heading_6.png", "icons/text_horizontalrule.png", "icons/text_indent.png", "icons/text_indent_remove.png", "icons/text_inverse.png", "icons/text_italic.png", "icons/text_kerning.png", "icons/text_left_to_right.png", "icons/text_letterspacing.png", "icons/text_letter_omega.png", "icons/text_linespacing.png", "icons/text_list_bullets.png", "icons/text_list_numbers.png", "icons/text_lowercase.png", "icons/text_lowercase_a.png", "icons/text_mirror.png", "icons/text_padding_bottom.png", "icons/text_padding_left.png", "icons/text_padding_right.png", "icons/text_padding_top.png", "icons/text_replace.png", "icons/text_right_to_left.png", "icons/text_rotate_0.png", "icons/text_rotate_180.png", "icons/text_rotate_270.png", "icons/text_rotate_90.png", "icons/text_ruler.png", "icons/text_shading.png", "icons/text_signature.png", "icons/text_smallcaps.png", "icons/text_spelling.png", "icons/text_strikethrough.png", "icons/text_subscript.png", "icons/text_superscript.png", "icons/text_tab.png", "icons/text_underline.png", "icons/text_uppercase.png", "icons/theme.png", "icons/thumb_down.png", "icons/thumb_up.png", "icons/tick.png", "icons/time.png", "icons/timeline_marker.png", "icons/time_add.png", "icons/time_delete.png", "icons/time_go.png", "icons/time_green.png", "icons/time_red.png", "icons/tree16.png", "icons/tree32.png", "icons/node_tree16.png", "icons/node_tree32.png", "icons/transmit.png", "icons/transmit_add.png", "icons/transmit_blue.png", "icons/transmit_delete.png", "icons/transmit_edit.png", "icons/transmit_error.png", "icons/transmit_go.png", "icons/transmit_red.png", "icons/tux.png", "icons/user_2.png", "icons/user_add.png", "icons/user_alert.png", "icons/user_b.png", "icons/user_brown.png", "icons/user_comment.png", "icons/user_cross.png", "icons/user_delete.png", "icons/user_earth.png", "icons/user_edit.png", "icons/user_female.png", "icons/user_go.png", "icons/user_gray.png", "icons/user_gray_cool.png", "icons/user_green.png", "icons/user_home.png", "icons/user_key.png", "icons/user_magnify.png", "icons/user_mature.png", "icons/user_orange.png", "icons/user_red.png", "icons/user_star.png", "icons/user_suit.png", "icons/user_suit_black.png", "icons/user_tick.png", "icons/vcard.png", "icons/vcard_add.png", "icons/vcard_delete.png", "icons/vcard_edit.png", 
                            "icons/vcard_key.png", "icons/vector.png", "icons/vector_add.png", "icons/vector_delete.png", "icons/vector_key.png", "icons/wand.png", "icons/weather_cloud.png", "icons/weather_clouds.png", "icons/weather_cloudy.png", "icons/weather_cloudy_rain.png", "icons/weather_lightning.png", "icons/weather_rain.png", "icons/weather_snow.png", "icons/weather_sun.png", "icons/webcam.png", "icons/webcam_add.png", "icons/webcam_connect.png", "icons/webcam_delete.png", "icons/webcam_error.png", "icons/webcam_start.png", "icons/webcam_stop.png", "icons/world.png", "icons/world_add.png", "icons/world_connect.png", "icons/world_dawn.png", "icons/world_delete.png", "icons/world_edit.png", "icons/world_go.png", "icons/world_key.png", "icons/world_link.png", "icons/world_night.png", "icons/world_orbit.png", "icons/wrench.png", "icons/wrench_orange.png", "icons/xhtml.png", "icons/xhtml_add.png", "icons/xhtml_delete.png", "icons/xhtml_error.png", "icons/xhtml_go.png", "icons/xhtml_valid.png", "icons/zoom.png", "icons/zoom_in.png", "icons/zoom_out.png", "icons/chm.gif", "icons/doc.gif", "icons/exe.gif", "icons/jpg.gif", "icons/pdf.gif", "icons/ppt.gif", "icons/rar.gif", "icons/txt.gif", "icons/xls.gif", "icons/empty.gif", "icons/upload.png", "icons/screen_full.gif", "icons/screen_actual.gif", "icons/writing32.png", "icons/settings32.png", "icons/chk_checked.gif", "icons/chk_unchecked.gif" };
                        if (styleClassNames.Count == iconUrls.Count)
                        {
                            for (int i = 0; i < styleClassNames.Count; i++)
                            {
                                list.Add(new Sys_IconManage()
                                {
                                    StyleClassName = styleClassNames[i],
                                    IconAddr = iconUrls[i],
                                    IconClass = (int)IconClassTypeEnum.CustomerIcon
                                });
                            }
                        }
                        #endregion
                        UserInfo admin = UserOperate.GetSuperAdmin();
                        list.ForEach(x =>
                        {
                            x.CreateDate = DateTime.Now;
                            x.ModifyDate = DateTime.Now;
                            x.CreateUserId = admin.UserId;
                            x.ModifyUserId = admin.UserId;
                            x.CreateUserName = string.IsNullOrEmpty(admin.AliasName) ? admin.UserName : admin.AliasName;
                            x.ModifyUserName = string.IsNullOrEmpty(admin.AliasName) ? admin.UserName : admin.AliasName;
                        });
                        CommonOperate.OperateRecords<Sys_IconManage>(list, OperateHandle.ModelRecordOperateType.Add, out errMsg, false);
                    }
                    #endregion
                    #region 添加管理员用户和管理员角色
                    Guid adminUserId = Guid.Empty;
                    //添加管理员组织
                    Sys_Organization org = new Sys_Organization() { Name = "管理员", Des = "管理员" };
                    Guid orgId = CommonOperate.OperateRecord<Sys_Organization>(org, ModelRecordOperateType.Add, out errMsg, null, false);
                    if (!ModelConfigHelper.ModelIsViewMode(typeof(Sys_User)))
                    {
                        //添加超级管理员
                        adminUserId = UserOperate.AddUser(out errMsg, "admin", "admin", orgId);
                        //添加管理员角色
                        PermissionOperate.AddAdminRole();
                    }
                    #endregion
                    #region 菜单初始化
                    if (!isOnlyInitSys)
                    {
                        #region 流程管理
                        Sys_Menu BpmMenu = new Sys_Menu()
                        {
                            Name = "流程管理",
                            Display = "流程管理",
                            IsValid = true,
                            IsLeaf = false,
                            Sort = 28
                        };
                        Guid BpmMenuId = CommonOperate.OperateRecord<Sys_Menu>(BpmMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu flowDesignMenu = new Sys_Menu()
                        {
                            Name = "流程设计",
                            Display = "流程设计",
                            Url = "/Bpm/FlowDesign.html",
                            ParentId = BpmMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 1
                        };
                        Guid flowDesignMenuId = CommonOperate.OperateRecord<Sys_Menu>(flowDesignMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu todoMenu = new Sys_Menu()
                        {
                            Name = "我的待办",
                            Display = "我的待办",
                            Url = "/Page/Grid.html?moduleName=待办任务&p_tp=0",
                            ParentId = BpmMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 2
                        };
                        Guid todoMenuId = CommonOperate.OperateRecord<Sys_Menu>(todoMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu applyMenu = new Sys_Menu()
                        {
                            Name = "我的申请",
                            Display = "我的申请",
                            Url = "/Page/Grid.html?moduleName=待办任务&p_tp=1",
                            ParentId = BpmMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 3
                        };
                        Guid applyMenuId = CommonOperate.OperateRecord<Sys_Menu>(applyMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu approvalMenu = new Sys_Menu()
                        {
                            Name = "我的审批",
                            Display = "我的审批",
                            Url = "/Page/Grid.html?moduleName=待办任务&p_tp=2",
                            ParentId = BpmMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 4
                        };
                        Guid approvalMenuId = CommonOperate.OperateRecord<Sys_Menu>(approvalMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        #endregion
                        #region 组织机构
                        Sys_Menu OrgMMenu = new Sys_Menu()
                        {
                            Name = "组织机构",
                            Display = "组织机构",
                            IsValid = true,
                            IsLeaf = false,
                            Sort = 29
                        };
                        Guid OrgMMenuId = CommonOperate.OperateRecord<Sys_Menu>(OrgMMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu deptMenu = new Sys_Menu()
                        {
                            Name = "部门管理",
                            Display = "部门管理",
                            Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "部门管理", null, out errMsg).Id,
                            Sys_ModuleName = "部门管理",
                            ParentId = OrgMMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 1
                        };
                        Guid deptMenuId = CommonOperate.OperateRecord<Sys_Menu>(deptMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu dutyMenu = new Sys_Menu()
                        {
                            Name = "职务管理",
                            Display = "职务管理",
                            Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "职务管理", null, out errMsg).Id,
                            Sys_ModuleName = "职务管理",
                            ParentId = OrgMMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 2
                        };
                        Guid dutyMenuId = CommonOperate.OperateRecord<Sys_Menu>(dutyMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu positionMenu = new Sys_Menu()
                        {
                            Name = "岗位管理",
                            Display = "岗位管理",
                            Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "岗位管理", null, out errMsg).Id,
                            Sys_ModuleName = "岗位管理",
                            ParentId = OrgMMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 3
                        };
                        Guid positionMenuId = CommonOperate.OperateRecord<Sys_Menu>(positionMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu empMenu = new Sys_Menu()
                        {
                            Name = "员工管理",
                            Display = "员工管理",
                            Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "员工管理", null, out errMsg).Id,
                            Sys_ModuleName = "员工管理",
                            ParentId = OrgMMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 4
                        };
                        Guid empMenuId = CommonOperate.OperateRecord<Sys_Menu>(empMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        Sys_Menu empPositionMenu = new Sys_Menu()
                        {
                            Name = "员工岗位",
                            Display = "员工岗位",
                            Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "员工岗位", null, out errMsg).Id,
                            Sys_ModuleName = "员工岗位",
                            ParentId = OrgMMenuId,
                            IsValid = true,
                            IsLeaf = true,
                            Sort = 5
                        };
                        Guid empPositionMenuId = CommonOperate.OperateRecord<Sys_Menu>(empPositionMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                        #endregion
                    }
                    #region 系统管理
                    Sys_Menu sysManageMenu = new Sys_Menu()
                    {
                        Name = "系统管理",
                        Display = "系统管理",
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 30
                    };
                    Guid sysManageMenuId = CommonOperate.OperateRecord<Sys_Menu>(sysManageMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    #region 系统设置
                    Sys_Menu sysSetMenu = new Sys_Menu()
                    {
                        Name = "系统设置",
                        Display = "系统设置",
                        ParentId = sysManageMenuId,
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 1
                    };
                    Guid sysSetMenuId = CommonOperate.OperateRecord<Sys_Menu>(sysSetMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg);

                    Sys_Menu orgMenu = new Sys_Menu()
                    {
                        Name = "组织管理",
                        Display = "组织管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "组织管理", null, out errMsg).Id,
                        Sys_ModuleName = "组织管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 0
                    };
                    Guid orgMenuId = CommonOperate.OperateRecord<Sys_Menu>(orgMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu userMenu = new Sys_Menu()
                    {
                        Name = "用户管理",
                        Display = "用户管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "用户管理", null, out errMsg).Id,
                        Sys_ModuleName = "用户管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 1
                    };
                    Guid userMenuId = CommonOperate.OperateRecord<Sys_Menu>(userMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu roleMenu = new Sys_Menu()
                    {
                        Name = "角色管理",
                        Display = "角色管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "角色管理", null, out errMsg).Id,
                        Sys_ModuleName = "角色管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 2
                    };
                    Guid roleMenuId = CommonOperate.OperateRecord<Sys_Menu>(roleMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu moduleMenu = new Sys_Menu()
                    {
                        Name = "模块管理",
                        Display = "模块管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "模块管理", null, out errMsg).Id,
                        Sys_ModuleName = "模块管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 3
                    };
                    Guid moduleMenuId = CommonOperate.OperateRecord<Sys_Menu>(moduleMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu menuMenu = new Sys_Menu()
                    {
                        Name = "菜单管理",
                        Display = "菜单管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "菜单管理", null, out errMsg).Id,
                        Sys_ModuleName = "菜单管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 4
                    };
                    Guid menuMenuId = CommonOperate.OperateRecord<Sys_Menu>(menuMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu sysFieldMenu = new Sys_Menu()
                    {
                        Name = "字段管理",
                        Display = "字段管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "字段管理", null, out errMsg).Id,
                        Sys_ModuleName = "字段管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 5
                    };
                    Guid sysFieldMenuId = CommonOperate.OperateRecord<Sys_Menu>(sysFieldMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu gridMenu = new Sys_Menu()
                    {
                        Name = "视图管理",
                        Display = "视图管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "视图管理", null, out errMsg).Id,
                        Sys_ModuleName = "视图管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 6
                    };
                    Guid gridMenuId = CommonOperate.OperateRecord<Sys_Menu>(gridMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu gridButtonMenu = new Sys_Menu()
                    {
                        Name = "视图按钮",
                        Display = "视图按钮",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "视图按钮", null, out errMsg).Id,
                        Sys_ModuleName = "视图按钮",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 7
                    };
                    Guid gridButtonMenuId = CommonOperate.OperateRecord<Sys_Menu>(gridButtonMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu formMenu = new Sys_Menu()
                    {
                        Name = "表单管理",
                        Display = "表单管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "表单管理", null, out errMsg).Id,
                        Sys_ModuleName = "表单管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 8
                    };
                    Guid formMenuId = CommonOperate.OperateRecord<Sys_Menu>(formMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu dictonaryMenu = new Sys_Menu()
                    {
                        Name = "数据字典",
                        Display = "数据字典",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "数据字典", null, out errMsg).Id,
                        Sys_ModuleName = "数据字典",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 9
                    };
                    Guid dictonaryMenuId = CommonOperate.OperateRecord<Sys_Menu>(dictonaryMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu bindDictonaryMenu = new Sys_Menu()
                    {
                        Name = "字典绑定",
                        Display = "字典绑定",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "字典绑定", null, out errMsg).Id,
                        Sys_ModuleName = "字典绑定",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 10
                    };
                    Guid bindDictonaryMenuId = CommonOperate.OperateRecord<Sys_Menu>(bindDictonaryMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu IconManageMenu = new Sys_Menu()
                    {
                        Name = "图标管理",
                        Display = "图标管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "图标管理", null, out errMsg).Id,
                        Sys_ModuleName = "图标管理",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 11
                    };
                    Guid IconManageMenuId = CommonOperate.OperateRecord<Sys_Menu>(IconManageMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu codeRuleMenu = new Sys_Menu()
                    {
                        Name = "编码规则",
                        Display = "编码规则",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "编码规则", null, out errMsg).Id,
                        Sys_ModuleName = "编码规则",
                        ParentId = sysSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 12
                    };
                    Guid codeRuleMenuId = CommonOperate.OperateRecord<Sys_Menu>(codeRuleMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    #endregion
                    #region 权限设置
                    Sys_Menu permissionSetMenu = new Sys_Menu()
                    {
                        Name = "权限设置",
                        Display = "权限设置",
                        ParentId = sysManageMenuId,
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 2
                    };
                    Guid permissionSetMenuId = CommonOperate.OperateRecord<Sys_Menu>(permissionSetMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu rolePermissionMenu = new Sys_Menu()
                    {
                        Name = "角色权限",
                        Display = "角色权限",
                        Url = "/Page/SetRolePermission.html",
                        ParentId = permissionSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 1
                    };
                    Guid rolePermissionMenuId = CommonOperate.OperateRecord<Sys_Menu>(rolePermissionMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu userPermissionMenu = new Sys_Menu()
                    {
                        Name = "用户权限",
                        Display = "用户权限",
                        Url = "/Page/SetUserPermission.html",
                        ParentId = permissionSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 2
                    };
                    Guid userPermissionMenuId = CommonOperate.OperateRecord<Sys_Menu>(userPermissionMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    #endregion
                    #region 桌面设置
                    Sys_Menu desktopSetMenu = new Sys_Menu()
                    {
                        Name = "桌面设置",
                        Display = "桌面设置",
                        ParentId = sysManageMenuId,
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 3
                    };
                    Guid desktopSetMenuId = CommonOperate.OperateRecord<Sys_Menu>(desktopSetMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    //桌面项管理
                    Sys_Menu deskItemMenu = new Sys_Menu()
                    {
                        Name = "桌面项管理",
                        Display = "桌面项管理",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "桌面项管理", null, out errMsg).Id,
                        Sys_ModuleName = "桌面项管理",
                        ParentId = desktopSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 1
                    };
                    Guid deskItemMenuId = CommonOperate.OperateRecord<Sys_Menu>(deskItemMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    //桌面列表配置
                    Sys_Menu deskGridMenu = new Sys_Menu()
                    {
                        Name = "桌面列表配置",
                        Display = "桌面列表配置",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "桌面列表配置", null, out errMsg).Id,
                        Sys_ModuleName = "桌面列表配置",
                        ParentId = desktopSetMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 2
                    };
                    Guid deskGridMenuId = CommonOperate.OperateRecord<Sys_Menu>(deskGridMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    #endregion
                    #region 参数配置
                    Sys_Menu paramsConfigMenu = new Sys_Menu()
                    {
                        Name = "参数配置",
                        Display = "参数配置",
                        ParentId = sysManageMenuId,
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 4
                    };
                    Guid paramsConfigMenuId = CommonOperate.OperateRecord<Sys_Menu>(paramsConfigMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu dbConfigMenu = new Sys_Menu()
                    {
                        Name = "数据库配置",
                        Display = "数据库配置",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "数据库配置", null, out errMsg).Id,
                        Sys_ModuleName = "数据库配置",
                        ParentId = paramsConfigMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 1
                    };
                    Guid dbConfigMenuId = CommonOperate.OperateRecord<Sys_Menu>(dbConfigMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu cacheConfigMenu = new Sys_Menu()
                    {
                        Name = "缓存配置",
                        Display = "缓存配置",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "缓存配置", null, out errMsg).Id,
                        Sys_ModuleName = "缓存配置",
                        ParentId = paramsConfigMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 2
                    };
                    Guid cacheConfigMenuId = CommonOperate.OperateRecord<Sys_Menu>(cacheConfigMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu pageCacheMenu = new Sys_Menu()
                    {
                        Name = "页面缓存",
                        Display = "页面缓存",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "页面缓存", null, out errMsg).Id,
                        Sys_ModuleName = "页面缓存",
                        ParentId = paramsConfigMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 3
                    };
                    Guid pageCacheMenuId = CommonOperate.OperateRecord<Sys_Menu>(pageCacheMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    Sys_Menu sysParamSetMenu = new Sys_Menu()
                    {
                        Name = "参数设定",
                        Display = "参数设定",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "参数设定", null, out errMsg).Id,
                        Sys_ModuleName = "参数设定",
                        ParentId = paramsConfigMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 10
                    };
                    Guid sysParamSetMenuId = CommonOperate.OperateRecord<Sys_Menu>(sysParamSetMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    #endregion
                    #region 日志管理
                    Sys_Menu logManageMenu = new Sys_Menu()
                    {
                        Name = "日志管理",
                        Display = "日志管理",
                        ParentId = sysManageMenuId,
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 5
                    };
                    Guid logManageMenuId = CommonOperate.OperateRecord<Sys_Menu>(logManageMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    //登录日志
                    Sys_Menu loginLogMenu = new Sys_Menu()
                    {
                        Name = "登录日志",
                        Display = "登录日志",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "登录日志", null, out errMsg).Id,
                        Sys_ModuleName = "登录日志",
                        ParentId = logManageMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 1
                    };
                    Guid loginLogMenuId = CommonOperate.OperateRecord<Sys_Menu>(loginLogMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    //操作日志
                    Sys_Menu operateLogMenu = new Sys_Menu()
                    {
                        Name = "操作日志",
                        Display = "操作日志",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "操作日志", null, out errMsg).Id,
                        Sys_ModuleName = "操作日志",
                        ParentId = logManageMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 2
                    };
                    Guid operateLogMenuId = CommonOperate.OperateRecord<Sys_Menu>(operateLogMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    //异常日志
                    Sys_Menu exceptionLogMenu = new Sys_Menu()
                    {
                        Name = "异常日志",
                        Display = "异常日志",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "异常日志", null, out errMsg).Id,
                        Sys_ModuleName = "异常日志",
                        ParentId = logManageMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 3
                    };
                    Guid exceptionLogMenuId = CommonOperate.OperateRecord<Sys_Menu>(exceptionLogMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);

                    #endregion
                    #region 系统监控
                    Sys_Menu monitorSysMenu = new Sys_Menu()
                    {
                        Name = "系统监控",
                        Display = "系统监控",
                        ParentId = sysManageMenuId,
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 6
                    };
                    Guid monitorSysMenuId = CommonOperate.OperateRecord<Sys_Menu>(monitorSysMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    Sys_Menu opTimeMonitorMenu = new Sys_Menu()
                    {
                        Name = "操作时间监控",
                        Display = "操作时间监控",
                        Sys_ModuleId = CommonOperate.GetEntity<Sys_Module>(x => x.Name == "操作时间监控", null, out errMsg).Id,
                        Sys_ModuleName = "操作时间监控",
                        ParentId = monitorSysMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 1
                    };
                    Guid opTimeMonitorMenuId = CommonOperate.OperateRecord<Sys_Menu>(opTimeMonitorMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    #endregion
                    #region 任务调度
                    Sys_Menu quartzMenu = new Sys_Menu()
                    {
                        Name = "任务调度",
                        Display = "任务调度",
                        ParentId = sysManageMenuId,
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 7
                    };
                    Guid quartzMenuId = CommonOperate.OperateRecord<Sys_Menu>(quartzMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    Sys_Menu quartzCenterMenu = new Sys_Menu()
                    {
                        Name = "调度中心",
                        Display = "调度中心",
                        Url = "/Quartz/QuartzCenter.html",
                        ParentId = quartzMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 1
                    };
                    Guid quartzCenterMenuId = CommonOperate.OperateRecord<Sys_Menu>(quartzCenterMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    Sys_Menu quartzTaskMenu = new Sys_Menu()
                    {
                        Name = "任务管理",
                        Display = "任务管理",
                        Url = "/Quartz/JobManage.html",
                        ParentId = quartzMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 2
                    };
                    Guid quartzTaskMenuId = CommonOperate.OperateRecord<Sys_Menu>(quartzTaskMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    #endregion
                    #region 消息通知
                    Sys_Menu msgNotifyMenu = new Sys_Menu()
                    {
                        Name = "消息通知",
                        Display = "消息通知",
                        ParentId = sysManageMenuId,
                        IsValid = true,
                        IsLeaf = false,
                        Sort = 8
                    };
                    Guid msgNotifyMenuId = CommonOperate.OperateRecord<Sys_Menu>(msgNotifyMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    Sys_Menu msgTemplateMenu = new Sys_Menu()
                    {
                        Name = "消息模板",
                        Display = "消息模板",
                        Sys_ModuleId = SystemOperate.GetModuleIdByName("消息模板"),
                        ParentId = msgNotifyMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 1
                    };
                    Guid msgTemplateMenuId = CommonOperate.OperateRecord<Sys_Menu>(msgTemplateMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    Sys_Menu eventNotifyMenu = new Sys_Menu()
                    {
                        Name = "事件通知",
                        Display = "事件通知",
                        Sys_ModuleId = SystemOperate.GetModuleIdByName("事件通知"),
                        ParentId = msgNotifyMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 2
                    };
                    Guid eventNotifyMenuId = CommonOperate.OperateRecord<Sys_Menu>(eventNotifyMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    Sys_Menu msgSendLogMenu = new Sys_Menu()
                    {
                        Name = "发送日志",
                        Display = "发送日志",
                        Sys_ModuleId = SystemOperate.GetModuleIdByName("消息发送日志"),
                        ParentId = msgNotifyMenuId,
                        IsValid = true,
                        IsLeaf = true,
                        Sort = 3
                    };
                    Guid msgSendLogMenuId = CommonOperate.OperateRecord<Sys_Menu>(msgSendLogMenu, OperateHandle.ModelRecordOperateType.Add, out errMsg, null, false);
                    #endregion
                    #endregion
                    CommonOperate.UpdateRecordsByExpression<Sys_Menu>(new
                                    {
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        CreateUserId = adminUserId,
                                        ModifyUserId = adminUserId,
                                        CreateUserName = "admin",
                                        ModifyUserName = "admin"
                                    }, null, out errMsg);
                    #endregion
                    #region 数据初始化
                    if (!isOnlyInitSys)
                    {
                        #region 流程按钮初始化
                        List<Bpm_FlowBtn> flowBtns = new List<Bpm_FlowBtn>()
                        {
                            new Bpm_FlowBtn(){ ButtonText="同意", ButtonIcon="eu-icon-approvalok", ClickMethod=string.Empty, ButtonTypeOfEnum=FlowButtonTypeEnum.AgreeBtn, Sort=1, Memo="同意按钮" },
                            new Bpm_FlowBtn(){ ButtonText="拒绝", ButtonIcon="eu-icon-reject", ClickMethod=string.Empty, ButtonTypeOfEnum=FlowButtonTypeEnum.RejectBtn, Sort=2, Memo="拒绝按钮" },
                            new Bpm_FlowBtn(){ ButtonText="回退", ButtonIcon="eu-icon-arrow_left", ClickMethod=string.Empty, ButtonTypeOfEnum=FlowButtonTypeEnum.BackBtn, Sort=3, Memo="回退按钮" },
                            new Bpm_FlowBtn(){ ButtonText="指派", ButtonIcon="eu-icon-user", ClickMethod=string.Empty, ButtonTypeOfEnum=FlowButtonTypeEnum.AssignBtn, Sort=4, Memo="指派按钮" }
                        };
                        flowBtns.ForEach(x =>
                        {
                            x.CreateDate = DateTime.Now;
                            x.CreateUserId = adminUserId;
                            x.CreateUserName = "admin";
                            x.ModifyDate = DateTime.Now;
                            x.ModifyUserId = adminUserId;
                            x.ModifyUserName = "admin";
                        });
                        //先删除所有
                        CommonOperate.DeleteRecordsByExpression<Bpm_FlowBtn>(null, out errMsg);
                        //再添加
                        CommonOperate.OperateRecords<Bpm_FlowBtn>(flowBtns, ModelRecordOperateType.Add, out errMsg, false);
                        #endregion
                        #region 桌面配置初始化
                        //先删除所有
                        CommonOperate.DeleteRecordsByExpression<Desktop_Item>(null, out errMsg);
                        CommonOperate.DeleteRecordsByExpression<Desktop_ItemTab>(null, out errMsg);
                        CommonOperate.DeleteRecordsByExpression<Desktop_GridField>(null, out errMsg);
                        //添加桌面各项配置
                        List<Desktop_Item> deskItems = new List<Desktop_Item>();
                        List<Desktop_ItemTab> deskTabs = new List<Desktop_ItemTab>();
                        List<Desktop_GridField> deskGridFields = new List<Desktop_GridField>();
                        #region 桌面项
                        Desktop_Item todoDeskItem = new Desktop_Item()
                        {
                            Id = Guid.NewGuid(),
                            Name = "我的待办",
                            Sort = 1,
                            IsCanClose = false,
                            Des = "我的待办"
                        };
                        Desktop_Item applicateDeskItem = new Desktop_Item()
                        {
                            Id = Guid.NewGuid(),
                            Name = "我的申请",
                            Sort = 2,
                            IsCanClose = false,
                            Des = "我的申请"
                        };
                        deskItems.Add(todoDeskItem);
                        deskItems.Add(applicateDeskItem);
                        deskItems.ForEach(x =>
                        {
                            x.CreateDate = DateTime.Now;
                            x.CreateUserId = adminUserId;
                            x.CreateUserName = "admin";
                            x.ModifyDate = DateTime.Now;
                            x.ModifyUserId = adminUserId;
                            x.ModifyUserName = "admin";
                        });
                        #endregion
                        #region 桌面项标签
                        Guid todoModuleId = SystemOperate.GetModuleIdByModelType(typeof(Bpm_WorkToDoList));
                        deskTabs.Add(new Desktop_ItemTab()
                        {
                            Desktop_ItemId = todoDeskItem.Id,
                            Title = "我的待办",
                            Url = string.Format("/Page/DesktopGrid.html?moduleId={0}&p_tp=0&top=8", todoModuleId.ToString()),
                            MoreUrl = string.Format("/Page/Grid.html?moduleId={0}&p_tp=0&nvm=1", todoModuleId.ToString()),
                            Sort = 1,
                            Des = "我的待办",
                            Flag = "taskTodo"
                        });
                        deskTabs.Add(new Desktop_ItemTab()
                        {
                            Desktop_ItemId = applicateDeskItem.Id,
                            Title = "我的申请",
                            Url = string.Format("/Page/DesktopGrid.html?moduleId={0}&p_tp=1&top=8", todoModuleId.ToString()),
                            MoreUrl = string.Format("/Page/Grid.html?moduleId={0}&p_tp=1&nvm=1", todoModuleId.ToString()),
                            Sort = 1,
                            Des = "我的申请",
                            Flag = "myApplicate"
                        });
                        deskTabs.Add(new Desktop_ItemTab()
                        {
                            Desktop_ItemId = applicateDeskItem.Id,
                            Title = "我的审批",
                            Url = string.Format("/Page/DesktopGrid.html?moduleId={0}&p_tp=2&top=8", todoModuleId.ToString()),
                            MoreUrl = string.Format("/Page/Grid.html?moduleId={0}&p_tp=2&nvm=1", todoModuleId.ToString()),
                            Sort = 2,
                            Des = "我的审批",
                            Flag = "myApproved"
                        });
                        deskTabs.ForEach(x =>
                        {
                            x.CreateDate = DateTime.Now;
                            x.CreateUserId = adminUserId;
                            x.CreateUserName = "admin";
                            x.ModifyDate = DateTime.Now;
                            x.ModifyUserId = adminUserId;
                            x.ModifyUserName = "admin";
                        });
                        #endregion
                        #region 桌面待办字段配置
                        deskGridFields.Add(new Desktop_GridField() { Sys_ModuleId = todoModuleId, FieidName = "Title", Width = 530, Sort = 1 });
                        deskGridFields.Add(new Desktop_GridField() { Sys_ModuleId = todoModuleId, FieidName = "Launcher", Width = 70, Sort = 2 });
                        deskGridFields.ForEach(x =>
                        {
                            x.CreateDate = DateTime.Now;
                            x.CreateUserId = adminUserId;
                            x.CreateUserName = "admin";
                            x.ModifyDate = DateTime.Now;
                            x.ModifyUserId = adminUserId;
                            x.ModifyUserName = "admin";
                        });
                        #endregion
                        bool rs = CommonOperate.OperateRecords<Desktop_Item>(deskItems, ModelRecordOperateType.Add, out errMsg, false);
                        if (rs)
                        {
                            CommonOperate.OperateRecords<Desktop_ItemTab>(deskTabs, ModelRecordOperateType.Add, out errMsg, false);
                            CommonOperate.OperateRecords<Desktop_GridField>(deskGridFields, ModelRecordOperateType.Add, out errMsg, false);
                        }
                        #endregion
                    }
                    #endregion
                    #region 自定义初始化
                    InitFactory factory = InitFactory.GetInstance();
                    if (factory != null)
                    {
                        try
                        {
                            factory.CustomerInit();
                        }
                        catch (Exception ex)
                        {
                            errMsg = ex.Message;
                        }
                    }
                    #endregion
                }
            }
            return errMsg;
        }

        #endregion
    }
}

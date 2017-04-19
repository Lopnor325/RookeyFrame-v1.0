using Rookey.Frame.Base;
using Rookey.Frame.Common;
using Rookey.Frame.EntityBase;
using Rookey.Frame.EntityBase.Attr;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base.EnumDef;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    /// <summary>
    /// 模块操作处理
    /// </summary>
    class Sys_ModuleOperateHandle : IModelOperateHandle<Sys_Module>, IGridOperateHandle<Sys_Module>, IUIDrawHandle<Sys_Module>
    {
        #region 模块操作接口
        /// <summary>
        /// 模块操作完成后
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandle(ModelRecordOperateType operateType, Sys_Module t, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (operateType == ModelRecordOperateType.Del && result)
            {
                if (t.IsCustomerModule)
                {
                    //自定义模块删除后要删除对应的字段、表单、表单字段、列表、列表字段、列表按钮、字典绑定等信息
                    SystemOperate.DeleteModuleReferences(t);
                }
            }
        }

        /// <summary>
        /// 模块操作前处理
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="t"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandle(ModelRecordOperateType operateType, Sys_Module t, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            if (operateType == ModelRecordOperateType.Del)
            {
                //非自定义模块不允许删除
                if (!t.IsCustomerModule)
                {
                    errMsg = string.Format("【{0}】为系统模块，不允许删除！", t.Name);
                    return false;
                }
                else //自定义模块如果有数据则不能删除
                {
                    long count = CommonOperate.Count(out errMsg, t.Id); //模块中记录数
                    if (count > 0)
                    {
                        errMsg = string.Format("模块【{0}】中存在【{0}】条记录，请先清空模块数据后再删除！", t.Name, count);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 模块集合操作完成后
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="result"></param>
        /// <param name="currUser"></param>
        /// <param name="otherParams"></param>
        public void OperateCompeletedHandles(ModelRecordOperateType operateType, List<Sys_Module> ts, bool result, UserInfo currUser, object[] otherParams = null)
        {
            if (operateType == ModelRecordOperateType.Del && result)
            {
                List<Sys_Module> tempModules = ts.Where(x => x.IsCustomerModule).ToList();
                if (tempModules.Count > 0)
                {
                    //自定义模块删除后要删除对应的字段、表单、表单字段、列表、列表字段、列表按钮、字典绑定等信息
                    foreach (Sys_Module t in tempModules)
                    {
                        SystemOperate.DeleteModuleReferences(t);
                    }
                }
            }
        }

        /// <summary>
        /// 模块集合操作完成前
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="ts"></param>
        /// <param name="errMsg"></param>
        /// <param name="otherParams"></param>
        /// <returns></returns>
        public bool BeforeOperateVerifyOrHandles(ModelRecordOperateType operateType, List<Sys_Module> ts, out string errMsg, object[] otherParams = null)
        {
            errMsg = string.Empty;
            if (operateType == ModelRecordOperateType.Del)
            {
                string otherErr = string.Empty;
                foreach (Sys_Module t in ts)
                {
                    if (!t.IsCustomerModule) //系统模块
                    {
                        //非自定义模块不允许删除
                        errMsg += errMsg == string.Empty ? string.Format("模块【{0}】", t.Name) : string.Format(",【{0}】", t.Name);
                    }
                    else //自定义模块，自定义模块如果有数据则不能删除
                    {
                        long count = CommonOperate.Count(out errMsg, t.Id);
                        if (count > 0)
                        {
                            otherErr += otherErr == string.Empty ? string.Format("模块【{0}】有记录【{1}】条", t.Name, count) : string.Format(",【{0}】有记录【{1}】条", t.Name, count);
                        }
                    }
                }
                if (errMsg != string.Empty)
                {
                    errMsg += "为系统模块，不允许删除！";
                }
                if (otherErr != string.Empty)
                {
                    otherErr += "，请清空各自定义模块的数据后再删除！";
                }
                errMsg = errMsg + otherErr;
                if (!string.IsNullOrEmpty(errMsg))
                    return false;
            }
            return true;
        }
        #endregion
        #region 网格接口
        /// <summary>
        /// 网格参数设置
        /// </summary>
        /// <param name="gridType"></param>
        /// <param name="gridParams"></param>
        /// <param name="request"></param>
        public void GridParamsSet(EnumDef.DataGridType gridType, TempModel.GridParams gridParams, HttpRequestBase request = null)
        {
        }

        /// <summary>
        /// 网格数据加载参数设置
        /// </summary>
        /// <param name="module"></param>
        /// <param name="gridDataParams"></param>
        /// <param name="request"></param>
        public void GridLoadDataParamsSet(Sys_Module module, TempModel.GridDataParmas gridDataParams, HttpRequestBase request = null)
        {
        }

        /// <summary>
        /// 网格数据处理
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        public void PageGridDataHandle(List<Sys_Module> data, object[] otherParams = null, UserInfo currUser = null)
        {
            if (data != null && data.Count > 0)
            {
                data.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.Display))
                        x.Display = x.Name;
                });
            }
        }

        /// <summary>
        /// 网格条件
        /// </summary>
        /// <returns></returns>
        public Expression<Func<Sys_Module, bool>> GetGridFilterCondition(out string where, EnumDef.DataGridType gridType, Dictionary<string, string> condition = null, string initModule = null, string initField = null, Dictionary<string, string> otherParams = null, UserInfo currUser = null)
        {
            where = string.Empty;
            return null;
        }

        /// <summary>
        /// 网格按钮操作验证
        /// </summary>
        /// <param name="buttonText">按钮显示名称</param>
        /// <param name="ids">操作记录id集合</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="currUser">当前用户</param>
        /// <returns></returns>
        public string GridButtonOperateVerify(string buttonText, List<Guid> ids, object[] otherParams = null, UserInfo currUser = null)
        {
            return string.Empty;
        }
        #endregion
        #region UI接口
        public string GetGridHTML(EnumDef.DataGridType gridType = DataGridType.MainGrid, string condition = null, string where = null, Guid? viewId = null, string initModule = null, string initField = null, Dictionary<string, object> otherParams = null, bool detailCopy = false, List<string> filterFields = null, Guid? menuId = null, bool isGridLeftTree = false, HttpRequestBase request = null)
        {
            return string.Empty;
        }

        public string GetSimpleSearchHTML(List<Sys_GridField> searchFields, EnumDef.DataGridType gridType = DataGridType.MainGrid, string condition = null, string where = null, Guid? viewId = null, string initModule = null, string initField = null, HttpRequestBase request = null)
        {
            return string.Empty;
        }

        public string GetEditFormHTML(Guid? id, string gridId = null, Guid? copyId = null, bool showTip = false, Guid? todoTaskId = null, Guid? formId = null, HttpRequestBase request = null)
        {
            return string.Empty;
        }

        public string GetEditDetailHTML(Guid? id, out bool detailTopDisplay, Guid? copyId = null, HttpRequestBase request = null)
        {
            detailTopDisplay = false;
            StringBuilder sb = new StringBuilder();
            Sys_Module module = id.HasValue ? SystemOperate.GetModuleById(id.Value) : null;
            bool overwriteFlag = module != null ? module.IsCustomerModule : true;
            if (overwriteFlag) //新增页面或者自定义模块
            {
                detailTopDisplay = true;
                sb.Append("<div title=\"模块字段\">");
                sb.Append("<div id=\"regon_\">");
                sb.Append("<table id=\"grid_\" class=\"easyui-datagrid\" data-options=\"border:false,toolbar:'#toolbar',singleSelect:true,onClickCell:OnCellClick\">");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th data-options=\"title:'ID',field:'Id',checkbox:true\">ID</th>");
                StringBuilder fieldTypeData = new StringBuilder("[");
                fieldTypeData.Append("{id:'varchar',text:'字符串型'},");
                fieldTypeData.Append("{id:'int',text:'整型'},");
                fieldTypeData.Append("{id:'guid',text:'Guid型'},");
                fieldTypeData.Append("{id:'bit',text:'布尔型'},");
                fieldTypeData.Append("{id:'date',text:'日期型'},");
                fieldTypeData.Append("{id:'datetime',text:'时间型'},");
                fieldTypeData.Append("{id:'decimal',text:'数值型'}");
                fieldTypeData.Append("]");
                sb.Append("<th data-options=\"title:'字段类型',field:'FieldType',width:100,align:'center',formatter:function(value,row,index){return FormatField(value,row,index,'FieldType'," + fieldTypeData + ");},editor:{type:'combobox',options:{valueField:'id',editable:false,textField:'text',data:" + fieldTypeData.ToString() + ",required:true,onSelect:OnFieldTypeSelected}}\">字段类型</th>");
                sb.Append("<th data-options=\"title:'外键模块',field:'ForeignModuleName',width:100,align:'center',editor:{type:'combobox',options:{valueField:'Name',textField:'Name',editable:true,onSelect:OnSelectForeignModule}}\">外键模块</th>");
                sb.Append("<th data-options=\"title:'字段名',field:'FieldName',width:100,align:'center',editor:{type:'textbox',options:{required:true}}\">字段名</th>");
                sb.Append("<th data-options=\"title:'字段长度',field:'FieldLen',width:100,align:'center',editor:{type:'numberbox',options:{precision:0}}\">字段长度</th>");
                sb.Append("<th data-options=\"title:'显示名称',field:'Display',width:100,align:'center',editor:{type:'textbox',options:{required:true}}\">显示名称</th>");
                sb.Append("<th data-options=\"title:'是否表单显示',field:'IsEnableForm',width:100,align:'center',formatter:function(value,row,index){return FormatField(value,row,index,'IsEnableForm');},editor:{type:'checkbox',options:{on:'1',off:'0'}}\">是否表单显示</th>");
                sb.Append("<th data-options=\"title:'控件类型',field:'ControlType',width:100,align:'center',formatter:function(value,row,index){return FormatField(value,row,index,'ControlType');},editor:{type:'combobox',options:{valueField:'id',editable:false,textField:'text',required:true}}\">控件类型</th>");
                sb.Append("<th data-options=\"title:'控件宽度',field:'ControlWidth',width:100,align:'center',editor:{type:'numberbox',options:{precision:0}}\">控件宽度</th>");
                sb.Append("<th data-options=\"title:'是否必填',field:'IsRequired',width:100,align:'center',formatter:function(value,row,index){return FormatField(value,row,index,'IsRequired');},editor:{type:'checkbox',options:{on:'1',off:'0'}}\">是否必填</th>");
                sb.Append("<th data-options=\"title:'所在行',field:'RowNum',width:100,align:'center',editor:{type:'numberbox',options:{precision:0}}\">所在行</th>");
                sb.Append("<th data-options=\"title:'所在列',field:'ColNum',width:100,align:'center',editor:{type:'numberbox',options:{precision:0}}\">所在列</th>");
                sb.Append("<th data-options=\"title:'是否列表显示',field:'IsEnableGrid',width:100,align:'center',formatter:function(value,row,index){return FormatField(value,row,index,'IsEnableGrid');},editor:{type:'checkbox',options:{on:'1',off:'0'}}\">是否列表显示</th>");
                sb.Append("<th data-options=\"title:'列头宽度',field:'HeadWidth',width:100,align:'center',editor:{type:'numberbox',options:{precision:0}}\">列头宽度</th>");
                sb.Append("<th data-options=\"title:'列排序编号',field:'HeadSort',width:100,align:'center',editor:{type:'numberbox',options:{precision:0}}\">列排序编号</th>");
                sb.Append("<th data-options=\"title:'允许搜索',field:'IsAllowGridSearch',width:100,align:'center',formatter:function(value,row,index){return FormatField(value,row,index,'IsAllowGridSearch');},editor:{type:'checkbox',options:{on:'1',off:'0'}}\">允许搜索</th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                if (module != null)
                {
                    string modelDll = string.Format("{0}TempModel\\{1}.dll", Globals.GetBinPath(), module.TableName);
                    Assembly dllAssembly = Assembly.LoadFrom(modelDll);
                    Type modelType = dllAssembly.GetTypes().Where(x => x.Name == module.TableName).FirstOrDefault();
                    List<PropertyInfo> ps = modelType.GetProperties().ToList();
                    sb.Append("<tbody>");
                    foreach (PropertyInfo p in ps)
                    {
                        if (CommonDefine.BaseEntityFieldsContainId.Contains(p.Name))
                            continue;
                        FieldConfigAttribute fieldAttr = (FieldConfigAttribute)Attribute.GetCustomAttribute(p, typeof(FieldConfigAttribute));
                        if (fieldAttr == null) continue;
                        Sys_Field sysField = SystemOperate.GetFieldInfo(module.Id, p.Name);
                        if (sysField == null) continue; int fieldLen = 0;
                        string fieldType = "varchar";
                        if (p.PropertyType == typeof(String))
                        {
                            StringLengthAttribute stringLenAttr = (StringLengthAttribute)Attribute.GetCustomAttribute(p, typeof(StringLengthAttribute));
                            if (stringLenAttr != null)
                                fieldLen = stringLenAttr.MaximumLength;
                            else
                                fieldLen = 8000;
                        }
                        else if (p.PropertyType == typeof(Boolean) || p.PropertyType == typeof(Boolean?))
                        {
                            fieldLen = 1;
                            fieldType = "bit";
                        }
                        else if (p.PropertyType == typeof(Int32) || p.PropertyType == typeof(Int32?))
                        {
                            fieldLen = 4;
                            fieldType = "int";
                        }
                        else if (p.PropertyType == typeof(Guid) || p.PropertyType == typeof(Guid?))
                        {
                            fieldLen = 36;
                            fieldType = "guid";
                        }
                        else if (p.PropertyType == typeof(Decimal) || p.PropertyType == typeof(Decimal?))
                        {
                            fieldLen = 20;
                            fieldType = "decimal";
                        }
                        else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                        {
                            if (fieldAttr.ControlType == (int)ControlTypeEnum.DateBox)
                                fieldType = "date";
                            else
                                fieldType = "datetime";
                            fieldLen = 20;
                        }
                        Sys_FormField field = SystemOperate.GetNfDefaultFormSingleField(sysField);
                        Sys_Grid grid = SystemOperate.GetDefaultGrid(module.Id);
                        Sys_GridField gridField = SystemOperate.GetGridFields(grid.Id, false).Where(x => x.Sys_FieldId == sysField.Id).FirstOrDefault();
                        sb.Append("<tr>");
                        sb.AppendFormat("<td>{0}</td>", sysField.Id); //字段Id
                        sb.AppendFormat("<td>{0}</td>", fieldType); //字段类型
                        sb.AppendFormat("<td>{0}</td>", sysField.ForeignModuleName.ObjToStr()); //外键模块
                        sb.AppendFormat("<td>{0}</td>", p.Name); //字段名
                        sb.AppendFormat("<td>{0}</td>", fieldLen); //字段长度
                        sb.AppendFormat("<td>{0}</td>", sysField.Display); //显示名称
                        sb.AppendFormat("<td>{0}</td>", field != null ? "1" : "0"); //是否表单显示
                        sb.AppendFormat("<td>{0}</td>", field != null ? field.ControlType.ToString() : string.Empty); //控件类型
                        sb.AppendFormat("<td>{0}</td>", field == null || field.Width == 0 ? 180 : field.Width); //控件宽度
                        sb.AppendFormat("<td>{0}</td>", field == null ? string.Empty : (field.IsRequired == true ? "1" : "0")); //是否必填
                        sb.AppendFormat("<td>{0}</td>", field != null ? field.RowNo.ToString() : string.Empty); //所在行
                        sb.AppendFormat("<td>{0}</td>", field != null ? field.ColNo.ToString() : string.Empty); //所在列
                        sb.AppendFormat("<td>{0}</td>", gridField != null ? "1" : "0"); //是否列表显示
                        sb.AppendFormat("<td>{0}</td>", gridField != null && gridField.Width.HasValue ? gridField.Width.Value.ToString() : "120"); //列头宽度
                        sb.AppendFormat("<td>{0}</td>", gridField != null ? gridField.Sort.ToString() : string.Empty); //列排序编号
                        sb.AppendFormat("<td>{0}</td>", gridField != null ? (gridField.IsAllowSearch ? "1" : "0") : string.Empty); //允许搜索
                        sb.Append("</tr>");
                    }
                    sb.Append("</tbody>");
                }
                sb.Append("</table>");
                //网格工具栏和搜索框
                sb.Append("<div id=\"toolbar\" class=\"toolbar datagrid-toolbar\" style=\"height:27px;\">");
                sb.Append("<a id=\"btnAdd\" href=\"#\" style=\"float:left;\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'eu-icon-add'\" onclick=\"AddField()\">新增</a>");
                sb.Append("<a id=\"btnDel\" href=\"#\" style=\"float:left;\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'eu-p2-icon-delete2'\" onclick=\"DelField()\">删除</a>");
                sb.Append("</div>");
                sb.Append("</div>");
                sb.Append("</div>");
                sb.Append("<script type=\"text/javascript\" src=\"/Scripts/common/Grid.js\"></script>");
            }
            return sb.ToString();
        }

        public string GetViewFormHTML(Guid id, string gridId = null, string fromEditPageFlag = null, bool isRecycle = false, bool showTip = false, Guid? formId = null, HttpRequestBase request = null)
        {
            return string.Empty;
        }

        public string GetViewDetailHTML(Guid? id, out bool detailTopDisplay, HttpRequestBase request = null)
        {
            detailTopDisplay = false;
            return string.Empty;
        }
        #endregion
    }
}

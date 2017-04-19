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
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Rookey.Frame.Common;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.EntityBase;

namespace Rookey.Frame.UIOperate.Control
{
    /// <summary>
    /// EasyUI扩展控件
    /// </summary>
    public static class MvcExtensions
    {
        #region 前端调用
        private static TagBuilder InitTagBuilder(this HtmlHelper htmlHelper, string name, bool isRequired = false, object value = null, object htmlAttributes = null)
        {
            //获取字段完整路径名
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            //生成元素
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), false);
            //如果未通过验证，则添加一个错误信息的css
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }
            //添加验证元属性
            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));
            tagBuilder.GenerateId(fullName);
            tagBuilder.MergeAttribute("name", fullName);
            if (isRequired)
            {
                tagBuilder.MergeAttribute("required", "required");
            }
            if (value != null)
            {
                tagBuilder.MergeAttribute("value", value.ToString());
            }
            return tagBuilder;
        }

        public static MvcHtmlString EasyuiValidatebox(this HtmlHelper htmlHelper, string name, string validType, bool isRequired = false, object value = null, string cls = null, object htmlAttributes = null)
        {
            StringBuilder options = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(validType))
            {
                options.AppendFormat("validType:'{0}'", validType);
            }
            if (htmlAttributes == null)
            {
                htmlAttributes = new { data_options = options.ToString() };
            }
            else
            {
                Dictionary<string, object> Dictionarys = htmlAttributes.ToDictionary();
                Dictionarys.Add("data_options", options.ToString());
            }

            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, htmlAttributes);
            tagBuilder.MergeAttribute("class", "easyui-validatebox " + (string.IsNullOrEmpty(cls) ? string.Empty : cls));

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiCombo(this HtmlHelper htmlHelper, string name, bool IsMultiple = false, bool isRequired = false, object value = null, string style = null, object htmlAttributes = null)
        {
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, htmlAttributes);
            tagBuilder.MergeAttribute("multiple", IsMultiple.ToString());
            tagBuilder.MergeAttribute("class", "easyui-combo");
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiComboBox(this HtmlHelper htmlHelper, string name, string valueField, string textField, string url = null, bool isRequired = false, bool multiple = false, object value = null, string cls = null, string style = null, bool isAllowEdit = true, bool isEditable = true)
        {
            StringBuilder builder = new StringBuilder();
            string editable = isEditable ? "true" : "false";
            builder.AppendFormat("editable:{0},", editable);
            builder.AppendFormat("url:'{0}',", url);
            string m = multiple ? "true" : "false";
            builder.AppendFormat("multiple:{0},", m);
            builder.AppendFormat("valueField: '{0}',", valueField);
            builder.AppendFormat("textField: '{0}',", textField);
            string required = isRequired ? "true" : "false";
            builder.AppendFormat("required: {0},", required);
            builder.AppendLine("filter: function (q, row) {");
            builder.AppendLine("var opts = $(this).combobox('options');");
            builder.AppendLine("return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) >= 0;");
            builder.AppendLine("}");
            builder.AppendLine(",onBeforeLoad:function(param){");
            builder.AppendLine("if(typeof(FunComboBoxDataBeforeLoad)=='function'){");
            builder.AppendFormat("FunComboBoxDataBeforeLoad(param,'{0}','{1}','{2}');", name, valueField, textField);
            builder.AppendLine("}");
            builder.AppendLine("},");
            builder.AppendLine("onLoadSuccess:function(){");
            builder.AppendLine("if(typeof(FunComboBoxDataLoadSuccess)=='function'){");
            builder.AppendFormat("FunComboBoxDataLoadSuccess('{0}','{1}');", name, value == null ? string.Empty : value.ToString());
            builder.AppendLine("}");
            if (!isAllowEdit)
            {
                builder.AppendLine("$('#" + name + "').combobox('disable');");
            }
            builder.AppendLine("},");
            builder.AppendLine("onSelect:function(record){");
            builder.AppendLine("if(typeof(FunComboBoxItemSelect)=='function'){");
            builder.AppendFormat("FunComboBoxItemSelect('{0}',record,'{1}','{2}');", name, valueField, textField);
            builder.AppendLine("}");
            builder.AppendLine("}");
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, new { data_options = builder.ToString() });
            tagBuilder.MergeAttribute("class", "easyui-combobox " + (string.IsNullOrEmpty(cls) ? string.Empty : cls));
            if (!string.IsNullOrEmpty(style))
            {
                tagBuilder.MergeAttribute("style", style);
            }
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiTextBox(this HtmlHelper htmlHelper, string name, bool isRequired = false, object value = null, object htmlAttributes = null)
        {
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, htmlAttributes);
            if (isRequired)
            {
                tagBuilder.MergeAttribute("class", "easyui-validatebox");
                tagBuilder.MergeAttribute("required", "true");
            }
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiComboGrid(this HtmlHelper htmlHelper, string name, string valueField, string textField, string url = null, bool isRequired = false, object value = null, object htmlAttributes = null)
        {
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, htmlAttributes);
            tagBuilder.MergeAttribute("idField", valueField);
            tagBuilder.MergeAttribute("textField", textField);
            tagBuilder.MergeAttribute("url", url);
            tagBuilder.MergeAttribute("class", "easyui-combogrid");
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiNumberBox(this HtmlHelper htmlHelper, string name, int min = 0, int max = 0, int precision = 2, bool isRequired = false, object value = null, string cls = null, object htmlAttributes = null)
        {
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, htmlAttributes);
            tagBuilder.MergeAttribute("min", min.ToString());
            tagBuilder.MergeAttribute("max", max.ToString());
            tagBuilder.MergeAttribute("precision", precision.ToString());
            tagBuilder.MergeAttribute("class", "easyui-numberbox " + (string.IsNullOrEmpty(cls) ? string.Empty : cls));
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiDateBox(this HtmlHelper htmlHelper, string name, bool isRequired = false, object value = null, string cls = null, string style = null, bool isAllowEdit = true)
        {
            DateTime time = DateTime.Now;
            if (value != null)
            {
                var flag = DateTime.TryParse(value.ToString(), out time);
                if (flag) value = Convert.ToDateTime(value).ToString("yyyy-MM-dd");
            }
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, null);
            tagBuilder.MergeAttribute("editable", "false");
            if (!isAllowEdit)
            {
                tagBuilder.MergeAttribute("disabled", "true");
            }
            if (!string.IsNullOrEmpty(style))
            {
                tagBuilder.MergeAttribute("style", style);
            }
            tagBuilder.MergeAttribute("class", "easyui-datebox " + (string.IsNullOrEmpty(cls) ? string.Empty : cls));
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiDateTimeBox(this HtmlHelper htmlHelper, string name, bool isRequired = false, object value = null, string cls = null, string style = null, bool isAllowEdit = true)
        {
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, null);
            tagBuilder.MergeAttribute("editable", "false");
            if (!isAllowEdit)
            {
                tagBuilder.MergeAttribute("disabled", "true");
            }
            if (!string.IsNullOrEmpty(style))
            {
                tagBuilder.MergeAttribute("style", style);
            }
            tagBuilder.MergeAttribute("class", "easyui-datetimebox " + (string.IsNullOrEmpty(cls) ? string.Empty : cls));
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiDialog(this HtmlHelper htmlHelper, string name, string url, string textField, string valueField, bool isRequired = false, object htmlAttributes = null, object value = null, bool isAllowEdit = true)
        {
            string options = "icons: [{iconCls:'eu-icon-search',handler: function(e){SelectDialogData($(e.data.target))}}]";
            if (htmlAttributes == null)
            {
                htmlAttributes = new { data_options = options };
            }
            else
            {
                Dictionary<string, object> Dictionarys = htmlAttributes.ToDictionary();
                Dictionarys.Add("data_options", options);
            }
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, htmlAttributes);
            if (isRequired)
            {
                tagBuilder.MergeAttribute("class", "easyui-validatebox");
                tagBuilder.MergeAttribute("required", "true");
            }
            if (!isAllowEdit)
            {
                tagBuilder.MergeAttribute("disabled", "true");
            }
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString EasyuiComboTree(this HtmlHelper htmlHelper, string name, string valueField, string textField, string url = null, bool isRequired = false, bool multiple = false, object value = null, string cls = null)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("url:'{0}',", url);
            builder.AppendLine("editable: false,");
            string m = multiple ? "true" : "false";
            builder.AppendFormat("multiple:{0},", m);
            string required = isRequired ? "true" : "false";
            builder.AppendFormat("required: {0},", required);
            StringBuilder function = new StringBuilder();
            function.AppendLine("function (data) {");
            function.AppendLine("if (typeof (data) == 'string') {");
            function.AppendLine(" var tempData = eval('(' + data + ')');");
            function.AppendLine(" return tempData;}");
            function.AppendLine(" else {arr = [];");
            function.AppendLine("arr.push(data);");
            function.AppendLine(" return arr;}}");
            builder.AppendFormat("loadFilter:{0},", function.ToString());
            StringBuilder funcSb = new StringBuilder();
            funcSb.AppendLine("if(typeof (OnSelectFunc) == 'function') {");
            funcSb.AppendLine("OnSelectFunc(node);}");
            builder.AppendFormat("onSelect:{0},", "function(node){ " + funcSb.ToString() + "}");
            StringBuilder successFn = new StringBuilder();
            successFn.AppendLine("if(typeof(OnLoadSuccessFunc)=='function'){");
            successFn.AppendLine("OnLoadSuccessFunc(node,data);}");
            builder.AppendFormat("onLoadSuccess:{0}", "function(node,data){" + successFn.ToString() + "}");
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, new { data_options = builder.ToString() });
            tagBuilder.MergeAttribute("class", "easyui-combotree " + (string.IsNullOrEmpty(cls) ? string.Empty : cls));
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString Easyui(this HtmlHelper htmlHelper, string name, ControlTypeEnum controlType, string valueField, string textField, string url, ValidateTypeEnum validateType = ValidateTypeEnum.No, bool isRequired = false, string style = null, object value = null, object htmlAttributes = null, string cls = null, bool multiple = false, bool isAllowEdit = true)
        {
            Dictionary<string, object> Dictionarys = new Dictionary<string, object>();
            if (htmlAttributes != null)
            {
                Dictionarys = htmlAttributes.ToDictionary();
            }
            if (!string.IsNullOrWhiteSpace(style))
            {
                if (Dictionarys.Keys.Contains("style"))
                {
                    Dictionarys["style"] = Dictionarys["style"] + style;
                }
                else
                {
                    Dictionarys.Add("style", style);
                }
            }
            bool isCanEdit = isAllowEdit; //是否允许编辑
            if (!isCanEdit) //不允许编辑
            {
                Dictionarys.Add("disabled", "disabled;");
                htmlAttributes = new { disabled = Dictionarys["disabled"], style = Dictionarys["style"] };
            }
            else
            {
                htmlAttributes = new { style = Dictionarys["style"] };
            }
            //文本框
            if (controlType == ControlTypeEnum.TextBox)
            {
                if (validateType != ValidateTypeEnum.No)
                {
                    return EasyuiValidatebox(htmlHelper, name, validateType.ToString(), isRequired, value, null, htmlAttributes);
                }
                else
                {
                    return EasyuiTextBox(htmlHelper, name, isRequired, value, htmlAttributes);
                }
            }
            //文本域
            if (controlType == ControlTypeEnum.TextAreaBox)
            {
                string defaultValue = value == null ? "" : value.ToString();
                return htmlHelper.TextArea(name, defaultValue, 6, 100, htmlAttributes);
            }
            //下拉框
            if (controlType == ControlTypeEnum.ComboBox)
            {
                return EasyuiComboBox(htmlHelper, name, valueField, textField, url, isRequired, false, value, null, Dictionarys["style"].ToString(), isCanEdit);
            }
            //弹出选择框
            if (controlType == ControlTypeEnum.DialogGrid)
            {
                return EasyuiDialog(htmlHelper, name, url, textField, valueField, isRequired, htmlAttributes, value, isCanEdit);
            }
            //日期
            if (controlType == ControlTypeEnum.DateBox)
            {
                string format = "yyyy-MM-dd";
                if (value.ObjToDateNull().HasValue)
                    value = value.ObjToDateNull().Value.ToString(format);
                return EasyuiDateBox(htmlHelper, name, isRequired, value, cls, Dictionarys["style"].ToString(), isCanEdit);
            }
            //时间
            if (controlType == ControlTypeEnum.DateTimeBox)
            {
                string format = "yyyy-MM-dd HH:mm:ss";
                if (value.ObjToDateNull().HasValue)
                    value = value.ObjToDateNull().Value.ToString(format);
                return EasyuiDateTimeBox(htmlHelper, name, isRequired, value, cls, Dictionarys["style"].ToString(), isCanEdit);
            }
            else if (controlType == ControlTypeEnum.ComboTree)
            {
                return EasyuiComboTree(htmlHelper, name, valueField, textField, url, isRequired, multiple, value, cls);
            }
            //其他
            TagBuilder tagBuilder = htmlHelper.InitTagBuilder(name, isRequired, value, htmlAttributes);
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
        #endregion

        #region 前后端通用

        public static string EasyuiTextBox(string name, object value = null, string cls = null, Dictionary<string, string> htmlAttr = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" type=\"text\" class=\"easyui-textbox", name);
            if (!string.IsNullOrWhiteSpace(cls))
            {
                sb.AppendFormat(" {0}\"", cls);
            }
            sb.AppendFormat(" value=\"{0}\"", value.ObjToStr());
            if (htmlAttr != null && htmlAttr.Count > 0)
            {
                foreach (string key in htmlAttr.Keys)
                {
                    if (string.IsNullOrWhiteSpace(key)) continue;
                    sb.AppendFormat(" {0}=\"{1}\"", key, htmlAttr[key]);
                }
            }
            sb.Append(" />");
            return sb.ToString();
        }

        public static string EasyuiComboBox2(string name, string valueField, string textField, string url = null, bool isRequired = false, object value = null)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<input name='" + name + "' id='" + name + "'></input>");
            builder.AppendLine("<script type=\"text/javascript\">");
            builder.AppendLine("$(function () {");
            builder.AppendLine("$('#" + name + "').combobox({");
            builder.AppendFormat("url:'{0}',", url);
            builder.AppendFormat("valueField: '{0}',", valueField);
            builder.AppendFormat("textField: '{0}',", textField);
            string required = isRequired ? "true" : "false";
            builder.AppendFormat("required: {0}", required + ",");
            builder.AppendLine("filter: function (q, row) {");
            builder.AppendLine("var opts = $(this).combobox('options');");
            builder.AppendLine("return row[opts.textField].toUpperCase().indexOf(q.toUpperCase()) >= 0;");
            builder.AppendLine("}");
            builder.AppendLine("})");
            builder.AppendLine("})");
            builder.AppendLine("</script>");
            return builder.ToString();
        }

        public static string CreateCheckbox(string name, string displayText, object value = null, int index = -1, bool isAllowEdit = true, string otherAttr = null)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            if (index > 0) //多选checkbox
            {
                htmlBuilder.Append("<span style=\"margin-right:5px;\">");
            }
            string idStr = index >= 0 ? name + index : name;
            string disabledStr = isAllowEdit ? string.Empty : "disabled=\"disabled\"";
            string checkedStr = value == null ? string.Empty : (value.ToString() == "1" ? "checked=\"checked\"" : string.Empty);
            htmlBuilder.Append("<input " + (otherAttr == null ? string.Empty : otherAttr) + " type=\"checkbox\" " + disabledStr + " name=\"" + name + "\" id=\"" + idStr + "\" value=\"" + (value == null ? string.Empty : value.ToString()) + "\" " + checkedStr + " />");
            if (index >= 0)
            {
                if (!string.IsNullOrWhiteSpace(displayText))
                {
                    htmlBuilder.Append("<label for=\"" + idStr + "\">" + displayText + "</label>");
                }
                htmlBuilder.Append("</span>");
            }
            return htmlBuilder.ToString();
        }

        public static string CreateMutiCheckbox(string name, string chkTexts, string chkValues, object value = null, bool isAllowEdit = true, string otherAttr = null)
        {
            bool isCanEdit = isAllowEdit;
            string[] values = string.IsNullOrWhiteSpace(chkValues) ? null : chkValues.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] texts = string.IsNullOrWhiteSpace(chkTexts) ? null : chkTexts.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (values != null && texts != null && values.Length > 0 && texts.Length > 0 && texts.Length == values.Length)
            {
                StringBuilder sb = new StringBuilder();
                string[] tempValues = values;
                if (value != null && !string.IsNullOrEmpty(value.ToString())) //编辑时赋值
                {
                    try
                    {
                        string[] token = value.ToString().Split(",".ToCharArray());
                        if (token.Length == values.Length)
                        {
                            tempValues = token;
                        }
                    }
                    catch
                    { }
                }
                for (int i = 0; i < tempValues.Length; i++)
                {
                    sb.Append(CreateCheckbox(name, texts[i], tempValues[i], i, isCanEdit, otherAttr));
                }
                return sb.ToString();
            }
            else
            {
                bool valueBool = value.ObjToBool();
                value = valueBool ? "1" : "0";
                string htm = CreateCheckbox(name, "", value, -1, isCanEdit, otherAttr);
                return htm;
            }
        }

        public static string CreateRadioButton(string name, string displayText, object value = null, int index = -1, bool isAllowEdit = true)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<span style=\"margin-right:5px;\">");
            string idStr = index >= 0 ? name + index : name;
            string disabledStr = isAllowEdit ? string.Empty : "disabled=\"disabled\"";
            htmlBuilder.Append("<input type=\"radio\" " + disabledStr + " name=\"" + name + "\" id=\"" + idStr + "\" value=\"" + value + "\" />");
            if (!string.IsNullOrWhiteSpace(displayText))
            {
                htmlBuilder.Append("<label for=\"" + idStr + "\">" + displayText + "</label>");
            }
            htmlBuilder.Append("</span>");

            return htmlBuilder.ToString();
        }

        public static string CreateMutiRadioButton(string name, string rdTexts, string rdValues, object value = null, bool isAllowEdit = true)
        {
            bool isCanEdit = isAllowEdit;
            string[] values = string.IsNullOrWhiteSpace(rdValues) ? null : rdValues.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] texts = string.IsNullOrWhiteSpace(rdTexts) ? null : rdTexts.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (values != null && texts != null && values.Length > 0 && texts.Length > 0 && texts.Length == values.Length)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < values.Length; i++)
                {
                    sb.Append(CreateRadioButton(name, texts[i], values[i], i, isCanEdit));
                }
                string js = SetRadioButtonDefaultValue(name, value);
                sb.Append(js);
                return sb.ToString();
            }
            else
            {
                string htm = CreateRadioButton(name, "", value, -1, isCanEdit);
                string js = SetRadioButtonDefaultValue(name, value);
                return htm + js;
            }
        }

        private static string SetRadioButtonDefaultValue(string name, object value)
        {
            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.Append("<script type=\"text/javascript\">");
            jsBuilder.Append("$(function () {");
            jsBuilder.Append("$('input[type=radio][name=" + name + "][value=" + value + "]').attr('checked',true);");
            jsBuilder.Append("})");
            jsBuilder.Append("</script>");
            return jsBuilder.ToString();
        }
        #endregion
    }
}

//编辑字段页面js
$(function () {
    $("input[type=checkbox]").click(function () {
        if ($(this).attr("checked")) {
            $(this).attr("value", "1");
        }
        else {
            $(this).attr("value", "0");
        }
    });
    //html标签上存在这个类会导致滚动条无法下拉到最底部，一直处于闪烁状态
    $("html").removeClass("panel-fit");
    //外键字段处理
    $("input[foreignField='1']").each(function () {
        var controlId = $(this).attr("Id");
        //设置外键字段的text值
        $("#" + controlId).textbox("setText", $(this).attr("textValue"));
    });
    //设置编辑字段智能提示
    try {
        var formField = eval("(" + decodeURIComponent($("#hd_formField").val()) + ")");
        if (formField && formField.ForeignModuleName && formField.ForeignTitleKey) { //针对外键智能提示
            var fieldDom = $('#' + formField.Sys_FieldName); //当前控件
            var tempDom = fieldDom.next('span').find('input.textbox-text'); //聚焦控件
            FieldBindAutoCompleted(tempDom, formField.ForeignModuleName, formField.ForeignTitleKey, null, function (dom, item, fieldName) {
                fieldDom.textbox("setValue", item["Id"]);
                fieldDom.textbox("setText", item["Name"]);
                tempDom.blur(function () {
                    var valueField = 'Id'; //值字段名
                    var textField = 'f_Name'; //文本字段名
                    fieldDom.textbox("setValue", item[valueField]);
                    fieldDom.textbox("setText", item[textField]);
                });
            });
        }
    } catch (err) { }
});

//获取更新的字段值
//backFun:回调函数
function GetUpdateFieldValue() {
    var form = $("#editFieldForm");
    var flag = form.form("validate");
    if (!flag) return;
    var data = form.fixedSerializeArrayFix();
    var fieldName = GetLocalQueryString("fieldName"); //字段名称
    var fieldValue = null; //字段值
    for (var i = 0; i < data.length; i++) {
        var obj = data[i];
        if (obj.name == fieldName) {
            fieldValue = obj.value;
            break;
        }
    }
    return fieldValue;
}

//下拉框、下拉列表、下拉树的下拉数据加载成功事件
//fieldName:字段名
//valueField:值字段
//textField:显示字段
function OnFieldLoadSuccess(fieldName, valueField, textField) {
    //调用模块自定义事件
    if (typeof (OverOnFieldLoadSuccess) == "function") {
        OverOnFieldLoadSuccess(fieldName, valueField, textField);
    }
}

//下拉框、下拉列表、下拉树的下拉数据加载失败事件
//fieldName:字段名
//valueField:值字段
//textField:显示字段
//arguments:在数据加载失败的时候触发，arguments参数和jQuery的$.ajax()函数里面的'error'回调函数的参数相同。
function OnLoadError(fieldName, valueField, textField, arguments) {
    //调用模块自定义事件
    if (typeof (OverOnLoadError) == "function") {
        OnLoadError(fieldName, valueField, textField, arguments);
    }
}

//下拉框、下拉列表、下拉树数据项选择事件
//record:选择的项
//fieldName:字段名
//valueField:值字段
//textField:显示字段
function OnFieldSelect(record, fieldName, valueField, textField) {
    //调用模块自定义事件
    if (typeof (OverOnFieldSelect) == "function") {
        OverOnFieldSelect(record, fieldName, valueField, textField);
    }
}

//字段值改变事件
//moduleInfo:模块信息
//fieldName:字段名
//newValue:改变后的值
//oldValue:改变前的值
function OnFieldValueChanged(moduleInfo, fieldName, newValue, oldValue) {
    var linkFields = $('#' + fieldName).attr('linkFields');
    if (linkFields && linkFields.length > 0) { //该字段存在值关联字段
        var token = linkFields.split(',');
        for (var i = 0; i < token.length; i++) {
            var tempDom = $('#' + token[i]);
            if (!tempDom.textbox('getText')) { //新增或编辑时，如果值关联字段为空
                tempDom.textbox('setText', newValue);
            }
        }
    }
    //调用模块自定义事件
    if (typeof (OverOnFieldValueChanged) == "function") {
        OverOnFieldValueChanged(moduleInfo, fieldName, newValue, oldValue);
    }
}

//下拉框、下拉列表、下拉树数据过滤
//fieldName:字段名
//valueField:值字段
//textField:显示字段
//data:当前数据
//parentDom:下拉树时父节点DOM对象
function LoadFilter(fieldName, valueField, textField, data, parentDom) {
    //调用模块自定义事件
    if (typeof (OverLoadFilter) == "function") {
        return OverLoadFilter(fieldName, valueField, textField, data, parentDom);
    }
    if (data && data.length > 0) {
        if (typeof (data) == "string") {
            var tempData = eval("(" + data + ")");
            arr = [];
            arr.push(tempData);
            return arr;
        }
        else {
            return data;
        }
    }
    return null;
}
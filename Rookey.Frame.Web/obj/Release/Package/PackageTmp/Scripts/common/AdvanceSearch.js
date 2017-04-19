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
    //搜索字段智能提示处理
    var formFields = GetFormFields();
    if (formFields && formFields.length > 0) {
        var moduleId = GetLocalQueryString("moduleId"); //模块Id
        $.each(formFields, function (i, obj) {
            var controlType = obj.ControlType; //字段控件类型
            if (controlType == 0 || controlType == 12 || controlType == 15 || controlType == 100 || obj.ForeignModuleName) { //字符串字段或外键字段
                var fieldDom = $('#' + obj.Sys_FieldName);
                var tempDom = fieldDom.next('span').find('input.textbox-text');
                FieldBindAutoCompleted(tempDom, moduleId, obj.Sys_FieldName, null, function (dom, item, fieldName, tempParamObj) {
                    tempParamObj.attr('v', item["Id"]);
                    tempParamObj.textbox("setValue", item["Id"]);
                    tempParamObj.textbox("setText", item["f_Name"]);
                }, fieldDom);
            }
        });
    }
    $('body').css('position', 'absolute');
});

//获取搜索参数
function GetSearchParam() {
    var form = $("#searchform");
    var data = form.fixedSerializeArrayFix();
    var o = {};
    $.each(data, function (index) {
        var name = this['name'];
        var value = this["value"];
        if (!o[name] && value) {
            o[name] = value;
        }
    });
    return o;
}

//清除搜索
function ClearSearchValue() {
    $("#searchform .easyui-textbox").each(function (i, item) {
        $(item).textbox('clear');
        $(item).attr('value', '').attr('v', '');
        $(item).next('span').find('input.textbox-value').attr('value', '');
    });
    $("#searchform .easyui-datetimebox").each(function (i, item) {
        $(item).combobox('clear');
    });
    $("#searchform .easyui-datebox").each(function (i, item) {
        $(item).combobox('clear');
    });
    $("#searchform .easyui-combobox").each(function (i, item) {
        $(item).combobox('clear');
    });
    $("#searchform .easyui-combotree").each(function (i, item) {
        $(item).combotree('clear');
    });
    $("#searchform .easyui-daterange").each(function (i, item) {
        $(item).val('');
    });
}
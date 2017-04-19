var id = GetLocalQueryString("id"); //记录Id
var page = GetLocalQueryString("page"); //页面类型标识

//通用查看表单js
$(function () {
    var mode = GetLocalQueryString("mode");
    if (mode == 2) { //弹出框时移除页面内部按钮
        $('#divFormBtn').remove();
    }
    //html标签上存在这个类会导致滚动条无法下拉到最底部，一直处于闪烁状态
    $("html").removeClass("panel-fit");
    $("a[id^='btnEditField_']").attr("title", "单击编辑字段值");
    $("#detailTab .datagrid-wrap").css('border-top-width', '0px')
                                  .css('border-left-width', '0px')
                                  .css('border-bottom-width', '0px');
    var ff = GetLocalQueryString("ff"); //主从编辑页面弹出明细查看标识
    if (ff == "1") { //是从主从编辑页面弹出明细查看页面
        //从编辑网格加载数据
        var detailModuleId = GetLocalQueryString("moduleId"); //明细模块Id
        var pmode = GetLocalQueryString("pmode"); //父页面编辑模式
        var gridId = "grid_" + detailModuleId;
        //取当前选中行字段文本显示值
        var getSelectRowDisplay = function () {
            if (pmode == 2) { //弹出框
                return topWin.GetParentDialogFrame()[0].contentWindow.GetSelectRowDisplayValue(gridId);
            }
            else { //tab方式
                return !isNfm ? top.getCurrentTabFrame()[0].contentWindow.GetSelectRowDisplayValue(gridId) : topWin.GetSelectRowDisplayValue(gridId);
            }
        };
        var row = getSelectRowDisplay();
        //绑定数据
        $.each(row, function (key, value) {
            var field = GetFormField(key);
            if (field && field.ForeignModuleName) {
                if (key.length > 4 && key.substr(key.length - 4) == "Name") {
                    var realName = key.substr(0, key.length - 4) + "Id";
                    $("#span_" + realName).text(value);
                }
            }
            else {
                $("#span_" + key).text(value);
            }
        });
        //加载对应主表的外键字段显示值
        var getTitleKeyValue = function () {
            if (pmode == 2) { //弹出框
                return topWin.GetParentDialogFrame()[0].contentWindow.GetTitleKeyValue(gridId);
            }
            else { //tab方式
                return !isNfm ? top.getCurrentTabFrame()[0].contentWindow.GetTitleKeyValue(gridId) : topWin.GetTitleKeyValue(gridId);
            }
        }
        var obj = getTitleKeyValue();
        if (obj) {
            $("#span_" + obj.foreignFieldName).text(obj.value);
        }
    }
    $("a[id^='btnEditField_']").parent().mouseover(function (e) {
        $("a[id^='btnEditField_']", $(this)).show();
    }).mouseout(function (e) {
        $("a[id^='btnEditField_']", $(this)).hide();
    });
    $("a[id^='btnEditField_']").hide();
    var formFields = GetFormFields();
    if (formFields && formFields.length > 0) {
        $.each(formFields, function (i, obj) {
            if (obj.ControlType == 6 || obj.ControlType == 7 || (obj.ControlType == 100 &&
                (obj.FieldType == "System.Int32" || obj.FieldType == "System.Int64" ||
                obj.FieldType == "System.Nullable`1[System.Int32]" || obj.FieldType == "System.Nullable`1[System.Int64]" ||
                obj.FieldType == "System.Decimal" || obj.FieldType == "System.Nullable`1[System.Decimal]" ||
                obj.FieldType == "System.Double" || obj.FieldType == "System.Nullable`1[System.Double]"))) {
                var fieldDom = $("#span_" + obj.Sys_FieldName);
                var oldValue = fieldDom.text();
                var value = NumThousandsFormat(oldValue);
                fieldDom.text(value);
            }
        });
    }
    $("div[id^='grid_toolbar_']").addClass('noprint'); //打印时隐藏明细列表操作按钮
    $('#divFormBtn').addClass('noprint'); //打印时隐藏表单操作按钮
    $('div.pagination').addClass('noprint'); //打印时隐藏分页工具栏
    $('.pagination table').addClass('noprint');
    if (typeof (OverFormLoadCompleted) == "function") {
        OverFormLoadCompleted();
    }
});

//编辑按钮事件
function ToEdit(obj) {
    var editMode = parseInt($(obj).attr("editMode"));
    if (editMode == 1) { //tab标签编辑模式
        var id = GetLocalQueryString("id");
        var tempModuleId = $(obj).attr("moduleId");
        var tempModuleName = $(obj).attr("moduleName");
        var title = "编辑" + tempModuleName;
        var titleKeyValue = $(obj).attr("titleKeyValue");
        if (titleKeyValue) {
            title = title + "－" + titleKeyValue;
        }
        var editUrl = "/Page/EditForm.html?page=edit&moduleId=" + tempModuleId + "&id=" + id;
        var fp = GetLocalQueryString("fp");
        if (fp) {
            editUrl += "&fp=" + fp;
        }
        if (isNfm) {
            editUrl += "&nfm=1";
        }
        editUrl += "&r=" + Math.random();
        //跳转到编辑页面
        UpdateSelectedTab(null, editUrl, title);
    }
    else if (editMode == 2) { //弹出框编辑模式
        topWin.CloseDialog();
        var dom = !isNfm ? top.getCurrentTabFrame().contents().find("#btnEdit") : parent.$('#btnEdit');
        if (dom.length == 0) { //编辑按钮不在网格工具栏上，从网格内行头找
            var tempDom = !isNfm ? top.getCurrentTabFrame().contents().find("div[id^='rowOperateDiv_'").eq(0) : parent.$("div[id^='rowOperateDiv_'").eq(0);
            var tag = tempDom.attr("Id").replace("rowOperateDiv_", "");
            dom = !isNfm ? top.getCurrentTabFrame().contents().find("#btnEdit_" + tag) : parent.$("#btnEdit_" + tag);
        }
        if (dom.length > 0) { //编辑按钮在工具栏上
            dom.click();
        }
    }
}

//获取表单的titlekey字段值
function GetTitleKeyValue() {
    var field = GetTitleKeyField();
    if (field && field.Sys_FieldName) {
        var name = field.Sys_FieldName;
        var value = $("#span_" + name).text();
        return { name: name, value: value, foreignFieldName: field.ForignFieldName, recordId: id };
    }
    return null;
}

//打印表单
function PrintForm(obj) {
    alert('打印表单');
}

//获取表单数据
function GetFormDataObj() {
    var dataJson = $('#formDataObj').val();
    if (dataJson && dataJson.length > 0) {
        var obj = decodeURIComponent(dataJson);
        return obj;
    }
    return null;
}
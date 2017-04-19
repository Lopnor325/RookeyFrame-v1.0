$(function () {
    var moduleName = $('#formCondition').attr('moduleName');
    var formCondition = null;
    var formConditionStr = $('#formCondition').attr('condition');
    if (formConditionStr != undefined && formConditionStr && formConditionStr.length > 0) {
        formCondition = JSON.parse(decodeURIComponent(formConditionStr));
    }
    var dutyCondition = null;
    var dutyConditionStr = $('#dutyCondition').attr('condition');
    if (dutyConditionStr != undefined && dutyConditionStr && dutyConditionStr.length > 0) {
        dutyCondition = JSON.parse(decodeURIComponent(dutyConditionStr));
    }
    var deptCondition = null;
    var deptConditionStr = $('#dutyCondition').attr('condition');
    if (deptConditionStr != undefined && deptConditionStr && deptConditionStr.length > 0) {
        deptCondition = JSON.parse(decodeURIComponent(deptConditionStr));
    }
    var iframe = topWin.GetCurrentDialogFrame()[0];
    var divDom = $(iframe).parent();
    var showOrHideOtherTabs = function (isShow) {
        if (isShow) {
            $('ul.tabs li').eq(1).hide();
            $('ul.tabs li').eq(2).hide();
            $('ul.tabs li').eq(3).hide();
            $('ul.tabs li').eq(4).hide();
        }
        else {
            $('ul.tabs li').eq(1).show();
            //$('ul.tabs li').eq(2).show();
            //$('ul.tabs li').eq(3).show();
            //$('ul.tabs li').eq(4).show();
            $('ul.tabs li').eq(2).hide();
            $('ul.tabs li').eq(3).hide();
            $('ul.tabs li').eq(4).hide();
        }
    };
    var paramsTemp = divDom.attr('params');
    if (paramsTemp != undefined && paramsTemp) {
        var params = JSON.parse(decodeURI(paramsTemp));
        if (params.Note != undefined && params.Note != null)
            $('#Note').textbox('setValue', params.Note);
        if (params.IsCustomerCondition != undefined && params.IsCustomerCondition != null && params.IsCustomerCondition) {
            $('#IsCustomerCondition').attr('checked', 'checked').val('1');
            showOrHideOtherTabs(true);
        }
        else {
            showOrHideOtherTabs(false);
        }
        if (params.FormCondition != undefined && params.FormCondition != null && params.FormCondition.length > 0) {
            try {
                formCondition = JSON.parse(params.FormCondition);
            } catch (e) { }
        }
        if (params.DutyCondition != undefined && params.DutyCondition != null && params.DutyCondition.length > 0) {
            try {
                dutyCondition = JSON.parse(params.DutyCondition);
            } catch (e) { }
        }
        if (params.DeptCondition != undefined && params.DeptCondition != null && params.DeptCondition.length > 0) {
            try {
                deptCondition = JSON.parse(params.DeptCondition);
            } catch (e) { }
        }
        if (params.SqlCondition != undefined && params.SqlCondition != null)
            $('#sql').textbox('setValue', params.SqlCondition);
    }
    else {
        showOrHideOtherTabs($('#IsCustomerCondition').attr('checked') == 'checked');
    }
    $('#formCondition').QueryConditionInit(moduleName, formCondition);
    if (false) {
        $('#dutyCondition').QueryConditionInit('职务管理', dutyCondition, function (data) {
            if (data && data.length > 0) {
                var tempData = [];
                for (var i = 0; i < data.length; i++) {
                    if (data[i].Sys_FieldName == 'Name' || data[i].Sys_FieldName == 'DutyLevel' || data[i].Sys_FieldName == 'DutyType') {
                        tempData.push(data[i]);
                    }
                }
                return tempData;
            }
            return data;
        });
        $('#deptCondition').QueryConditionInit('部门管理', deptCondition, function (data) {
            if (data && data.length > 0) {
                var tempData = [];
                for (var i = 0; i < data.length; i++) {
                    if (data[i].Sys_FieldName == 'Name' || data[i].Sys_FieldName == 'DeptType' || data[i].Sys_FieldName == 'DeptGrade') {
                        tempData.push(data[i]);
                    }
                }
                return tempData;
            }
            return data;
        });
    }
    $("input[type=checkbox]").click(function () {
        if ($(this).attr("checked")) {
            $(this).attr("value", "1");
            showOrHideOtherTabs(true);
        }
        else {
            $(this).attr("value", "0");
            showOrHideOtherTabs(false);
        }
    });
});

//获取连线参数
function GetLineParams() {
    var obj = {};
    obj.Note = $('#Note').textbox('getValue');
    var isCustomer = $('#IsCustomerCondition').val() == "1" ? true : false;
    var formConditions = $('#formCondition').GetQueryConditions();
    var dutyConditions = $('#dutyCondition').GetQueryConditions();
    var deptConditions = $('#deptCondition').GetQueryConditions();
    var sql = $('#sql').textbox('getValue');
    if (formConditions != null && formConditions.length > 0)
        obj.FormCondition = JSON.stringify(formConditions);
    if (dutyConditions != null && dutyConditions.length > 0)
        obj.DutyCondition = JSON.stringify(dutyConditions);
    if (deptConditions != null && deptConditions.length > 0)
        obj.DeptCondition = JSON.stringify(deptConditions);
    if (sql != null && sql.length > 0)
        obj.SqlCondition = sql;
    obj.IsCustomerCondition = isCustomer;
    return obj;
}
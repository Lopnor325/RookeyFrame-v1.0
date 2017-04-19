var page = GetLocalQueryString("page"); //页面类型标识
var moduleName = decodeURI(GetLocalQueryString("moduleName"));

//表单加载完成后
function OverFormLoadCompleted() {
    if (moduleName == "视图字段") {
        if (page == "add" || page == "edit") {
            //设置模块字段的下拉数据源
            var viewId = $('#Sys_GridId').textbox('getValue');
            if (viewId) { //根据当前表单视图绑定字段信息
                var flag = 4;
                if (page == "edit") flag = 5;
                var url = '/' + CommonController.Async_System_Controller + '/LoadFields.html?flag=' + flag + '&viewId=' + viewId;
                $('#Sys_FieldId').combobox('reload', url);
                if (page == "edit") {
                    var value = $('#Sys_FieldId').combobox('getValue');
                    $('#Sys_FieldId').combobox('setValue', value);
                }
            }
        }
    }
}

//重写字段选择事件
function OverOnFieldSelect(record, fieldName, valueField, textField) {
    if (moduleName == "视图字段") {
        if (page == "add") {
            if (fieldName == 'Sys_GridId') { //选择字段后加载依赖字段数据源
                var viewId = $('#Sys_GridId').textbox('getValue');
                var url = '/' + CommonController.Async_System_Controller + '/LoadFields.html?flag=4&viewId=' + viewId;
                $('#Sys_FieldId').combobox('clear').combobox('reload', url);
            }
        }
        if (page == "add" || page == "edit") {
            if (fieldName == 'Sys_FieldId') { //选择字段后，显示名称随着变
                $('#Display').textbox('setValue', $('#Sys_FieldId').combobox('getText'));
            }
        }
    }
}

//刷新字段格式化
function RefreshFieldFormat(obj) {
    var tempModuleId = $(obj).attr("moduleId");
    var gridId = $(obj).attr('gridId');
    var gridObj = $("#" + gridId);
    var rows = GetSelectRows(gridId); //获取选中行
    if (!rows || rows.length == 0) { //没有选中行，从当前按钮中找对应的记录Id来得到选择行
        var selectId = $(obj).attr("recordId");
        var tempRows = gridObj.datagrid("getRows");
        for (var i = 0; i < tempRows.length; i++) {
            var tempRow = tempRows[i];
            if (selectId == tempRow["Id"]) {
                rows.push(tempRow);
                break;
            }
        }
    }
    if (!rows || rows.length == 0) {
        topWin.ShowMsg("提示", "请至少选择一条记录！", null, null, 1);
        return;
    }
    var ids = "";
    for (var i = 0; i < rows.length; i++) {
        if (rows[i]["Id"] != null)
            ids += rows[i]["Id"] + ",";
    }
    if (!ids || ids.length == 0) {
        topWin.ShowMsg("提示", "ids为空！", null, null, 1);
        return;
    }
    ids = ids.substr(0, ids.length - 1);
    var url = '/' + CommonController.Async_System_Controller + '/RefreshFieldFormat.html';
    ExecuteCommonAjax(url, { ids: ids }, null, true, true);
}

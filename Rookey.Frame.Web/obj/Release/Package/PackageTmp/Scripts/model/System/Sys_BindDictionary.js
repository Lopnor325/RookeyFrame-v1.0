
//表单加载完成后
function OverFormLoadCompleted() {
    if (page == "add" || page == "edit") {
        //设置模块字段的下拉数据源
        var moduleId = $('#Sys_ModuleId').textbox('getValue');
        if (moduleId && moduleId != GuidEmpty) {
            var url = '/' + CommonController.Async_System_Controller + '/LoadFields.html?flag=9&moduleId=' + moduleId;
            $('#FieldName').combobox('reload', url);
            if (page == "edit") {
                var value = $('#FieldName').combobox('getValue');
                $('#FieldName').combobox('setValue', value);
            }
        }
    }
}

//重写字段选择事件
function OverOnFieldSelect(record, fieldName, valueField, textField) {
    if (page == "add" || page == "edit") {
        if (fieldName == "Sys_ModuleId") { //新增时选择模块后加载字段
            var url = '/' + CommonController.Async_System_Controller + '/LoadFields.html?flag=9&moduleId=' + record[valueField];
            $('#FieldName').combobox('clear').combobox('reload', url);
        }
    }
}
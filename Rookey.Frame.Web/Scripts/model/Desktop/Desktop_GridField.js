
//表单加载完成后
function OverFormLoadCompleted() {
    if (page == "add" || page == "edit") {
        var moduleId = $('#Sys_ModuleId').textbox('getValue');
        if (moduleId) {
            var url='/' + CommonController.Async_System_Controller + '/LoadViewFields.html?flag=2&moduleId=' + moduleId;
            $('#FieidName').combobox('reload', url);
            if (page == "edit") {
                var value = $('#FieidName').combobox('getValue');
                $('#FieidName').combobox('setValue', value);
            }
        }
    }
}

//重写字段选择事件
function OverOnFieldSelect(record, fieldName, valueField, textField) {
    if (page == "add" || page == "edit") {
        if (fieldName == 'Sys_ModuleId') { //选择模块后加载字段
            var moduleId = $('#Sys_ModuleId').textbox('getValue');
            if (moduleId) {
                var url = '/' + CommonController.Async_System_Controller + '/LoadViewFields.html?flag=2&moduleId=' + moduleId;
                $('#FieidName').combobox('clear').combobox('reload', url);
            }
        }
        else if (fieldName == 'FieidName') {
            $('#Width').numberbox('setValue', record.Width ? record.Width : 120);
            $('#Sort').numberbox('setValue', record.Sort ? record.Sort : 1);
        }
    }
}
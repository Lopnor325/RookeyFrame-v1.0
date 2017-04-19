//初始化
$(function () {
    if (page == "add" || page == "edit") {
        //设置模块字段的下拉数据源
        var moduleId = $('#Sys_ModuleId').combobox('getValue');
        if (moduleId) { //根据当前表单视图绑定字段信息
            var url = '/' + CommonController.Async_Bpm_Controller + '/GetModuleNodes.html?moduleId=' + moduleId;
            var workNodeDom = $('#Bpm_WorkNodeId');
            var value = workNodeDom.combobox('getValue');
            workNodeDom.combobox('clear').combobox('reload', url);
            if (page == "edit") {
                workNodeDom.combobox('setValue', value);
            }
        }
    }
});

//重写字段选择事件
function OverOnFieldSelect(record, fieldName, valueField, textField) {
    if (page == "add" || page == "edit") {
        if (fieldName == 'Sys_ModuleId') { //选择模块后加载模块流程结点
            var url = '/' + CommonController.Async_Bpm_Controller + '/GetModuleNodes.html?moduleId=' + record.Id;
            $('#Bpm_WorkNodeId').combobox('clear').combobox('reload', url);
        }
    }
}
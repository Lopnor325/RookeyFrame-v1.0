//字段值选择后事件
function OnFieldSelect(record, fieldName, valueField, textField) {
    if (fieldName == "OrgM_DeptId") {
        var deptName = record[textField];
        var dutyId = $('#OrgM_DutyId').combobox('getValue');
        if (dutyId) {
            var dutyName = $('#OrgM_DutyId').combobox('getText');
            $('#Name').textbox('setValue', deptName + '－' + dutyName);
        }
    }
    else if (fieldName == "OrgM_DutyId") {
        var dutyName = record[textField];
        var deptId = $('#OrgM_DeptId').textbox('getValue');
        if (deptId) {
            var deptName = $('#OrgM_DeptId').textbox('getText');
            $('#Name').textbox('setValue', deptName + '－' + dutyName);
        }
    }
}
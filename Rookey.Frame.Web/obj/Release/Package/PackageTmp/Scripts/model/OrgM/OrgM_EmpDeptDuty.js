//字段值选择后事件
function OnFieldSelect(record, fieldName, valueField, textField) {
    if (fieldName == "OrgM_DeptId") { //选择部门后过滤部门职务
        var deptId = record[valueField];
        var url = '/OrgMAsync/GetDeptDutys.html?deptId=' + deptId;
        $('#OrgM_DutyId').combobox('clear').combobox('reload', url);
    }
}
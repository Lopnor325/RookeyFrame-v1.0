
//入口
$(function () {
    if (page == "add" || page == "edit") {
        if ($("#IsEnableDate").attr("checked")) {
            $('#DateFormat').combobox("enable");
        }
        else {
            $('#DateFormat').combobox("disable");
        }
        $("#IsEnableDate").click(function () {
            if ($(this).attr("checked")) {
                $('#DateFormat').combobox("enable");
            }
            else {
                $('#DateFormat').combobox("disable");
            }
        });
        $('#RuleFormat').textbox('readonly', true); //启用只读
    }
});

//校验流水号
function CheckSerialNumberValue() {
    var serialNumber = $('#SerialNumber').numberbox('getValue');
    if (parseInt(serialNumber) <= 0 || parseInt(serialNumber) > 9) {
        $('#SerialNumber').numberbox('setValue', '1');
        $('#SerialNumber').focus();
        topWin.ShowAlertMsg("提示", "流水号应该在1-9之间！", "info");
        return false;
    }
    return true;
}

//校验流水号长度
function CheckSNLengthValue() {
    var sNLength = $('#SNLength').numberbox('getValue');
    if (parseInt(sNLength) <= 0 || parseInt(sNLength) > 9) {
        $('#SNLength').numberbox('setValue', '1');
        $('#SNLength').focus();
        topWin.ShowAlertMsg("提示", "流水号应该在1-9之间！", "info");
        return false;
    }
    return true;
}

//校验占位符
function CheckPlaceHolderValue() {
    var placeHolder = $('#PlaceHolder').numberbox('getValue');
    if (parseInt(placeHolder) < 0 || parseInt(placeHolder) > 9) {
        $('#PlaceHolder').numberbox('setValue', '0');
        $('#PlaceHolder').focus();
        topWin.ShowAlertMsg("提示", "序列号长度应该在0-9之间！", "info");
        return false;
    }
    return true;
}

///构造显示格式
function ConstructRuleFormat(newValue, fieldName) {
    var ruleFormat = "";
    var prefix = $("#Prefix").textbox("getText");
    var sNLength = "";
    if (fieldName == "SNLength") {
        sNLength = newValue;
    }
    else {
        sNLength = $('#SNLength').numberbox('getValue');
    }
    if (sNLength == null || sNLength == "") return;

    var serialNumber = "";
    if (fieldName == "SerialNumber") {
        serialNumber = newValue;
    }
    else {
        serialNumber = $('#SerialNumber').numberbox('getValue');
    }
    if (serialNumber == null || serialNumber == "") return;

    var placeHolder = "";
    if (fieldName == "PlaceHolder") {
        placeHolder = newValue;
    }
    else {
        placeHolder = $('#PlaceHolder').numberbox('getValue');
    }
    if (placeHolder == null || placeHolder == "") return;

    var dateFormatDb = "";
    if (fieldName == "DateFormat") {
        dateFormatDb = newValue;
    }
    else {
        dateFormatDb = $('#DateFormat').combobox('getText');
    }

    ruleFormat = ruleFormat + prefix;
    var isEnableDate = $("#IsEnableDate").attr("checked");
    if (isEnableDate) {
        if (dateFormatDb == null || dateFormatDb == "") return;
        ruleFormat = ruleFormat + dateFormatDb;
    }

    var serialNumberFormat = "";
    for (var ii = 0; ii < sNLength - 1; ii++)//这里不要等于
    {
        serialNumberFormat = serialNumberFormat + "" + placeHolder;
    }
    serialNumberFormat = serialNumberFormat + serialNumber;
    ruleFormat = ruleFormat + serialNumberFormat;
    $("#RuleFormat").textbox("setValue", ruleFormat);
}

//重写字段选择事件
function OverOnFieldSelect(record, fieldName, valueField, textField) {
    if (page == "add" || page == "edit") {
        if (fieldName == 'Sys_ModuleId') { //新增时选择模块后后自动带出该模块的所有字段
            var selectModuleId = $('#Sys_ModuleId').textbox('getValue');
            var url = '/' + CommonController.Async_System_Controller + '/LoadFields.html?flag=3&moduleId=' + selectModuleId;
            $('#FieldName').combobox('clear').combobox('loadData', [{ Name: '', Display: '' }]).combobox('reload', url);
        }
    }
}

//字段值改变事件重写
function OverOnFieldValueChanged(moduleInfo, fieldName, newValue, oldValue) {
    if (fieldName == "Prefix") {
        ConstructRuleFormat("", fieldName);
    }
    else if (fieldName == "SerialNumber") {
        if (CheckSerialNumberValue()) {
            ConstructRuleFormat(newValue, fieldName);
        }
    }
    else if (fieldName == "SNLength") {
        if (CheckSNLengthValue()) {
            ConstructRuleFormat(newValue, fieldName);
        }
    }
    else if (fieldName == "PlaceHolder") {
        if (CheckPlaceHolderValue()) {
            ConstructRuleFormat(newValue, fieldName);
        }
    }
    else if (fieldName == "DateFormat") {
        var tempValue = $("#DateFormat").combobox("getText");
        ConstructRuleFormat(tempValue, fieldName);
    }
}

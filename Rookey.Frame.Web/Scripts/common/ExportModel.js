var fieldInfos = null;
var methodData = null;
var currRowIndex = 0; //当前行

$(function () {
    //html标签上存在这个类会导致滚动条无法下拉到最底部，一直处于闪烁状态
    $("html").removeClass("panel-fit");
    AddRow(); //默认增加一行
    $('#div_condition').hide();
    $("input[name='rd_select']").click(function () {
        var val = $(this).val();
        if (val == "2") {
            $('#div_condition').show();
        }
        else {
            $('#div_condition').hide();
        }
    });
    //加载字段信息
    var fieldJson = $('#fieldInfos').val();
    if (fieldJson) {
        fieldInfos = eval("(" + decodeURIComponent(fieldJson) + ")");
    }
});

//增加一行
function AddRow() {
    var gridObj = $('#conditionGrid');
    var rows = gridObj.datagrid("getRows");
    if (rows.length == 4) {
        topWin.ShowMsg('提示', '最多只能添加4行！');
        return;
    }
    gridObj.datagrid('appendRow', { Method: 0, Group: 'And' });
    var rowIndex = 0;
    if (rows.length > 0) {
        var lastRow = rows[rows.length - 1];
        var rowIndex = gridObj.datagrid('getRowIndex', lastRow);
    }
    gridObj.datagrid('beginEdit', rowIndex);
    //添加监听事件
    $("div.datagrid-view2 div.datagrid-body td[field='Field']").find(".textbox-prompt,.textbox-addon-right").click(function () {
        var tr = $(this).parents("td[field='Field']").parent();
        var tempId = tr.attr("id");
        var startIndex = tempId.lastIndexOf("-");
        currRowIndex = tempId.substr(startIndex + 1, 1);
    });
}

//移除一行
function RemoveRow() {
    var gridObj = $('#conditionGrid');
    var rows = gridObj.datagrid("getRows");
    if (rows.length > 1) { //一行以上才能移除
        var lastRow = rows[rows.length - 1];
        var rowIndex = gridObj.datagrid('getRowIndex', lastRow);
        gridObj.datagrid('deleteRow', rowIndex);
    }
}

//格式化操作列
function FormatOp(value, row, index) {
    var btn = "<img title='增加条件' onclick='AddRow()' style='width:18px;cursor:pointer' src='/Scripts/jquery-easyui/themes/icons/edit_add.png' />";
    btn += "<img title='移除条件' onclick='RemoveRow()' style='margin-left:8px;width:18px;cursor:pointer' src='/Scripts/jquery-easyui/themes/icons/cancel.png' />";
    return btn;
}

//选择字段后，根据字段类型，过滤条件类型
function OnFieldSelected(record) {
    if (fieldInfos != null && fieldInfos.length > 0) {
        var fieldInfo = null;
        for (var i = 0; i < fieldInfos.length; i++) {
            if (record.Sys_FieldName == fieldInfos[i].FieldName) {
                fieldInfo = fieldInfos[i];
                break;
            }
        }
        if (fieldInfo != null) {
            var rowIndex = currRowIndex;
            var fieldObj = $($('#conditionGrid').datagrid('getEditor', { index: rowIndex, field: 'Field' }).target);
            var methodObj = $($('#conditionGrid').datagrid('getEditor', { index: rowIndex, field: 'Method' }).target);
            if (methodData == null) {
                methodData = methodObj.combobox('getData');
            }
            var rowIdPrefix = GetGridRowIdPrefix();
            var valueTdDom = $("#" + rowIdPrefix + rowIndex + " td[field='Value']");
            valueTdDom.html('<input style="width:100%;" id="value_' + rowIndex + '" type="text" />');
            var valueObj = $('#value_' + rowIndex);
            var tempMethodData = [];
            if (fieldInfo.DicData || fieldInfo.EnumData || fieldInfo.FieldType == 'System.Boolean' ||
                 fieldInfo.FieldType == 'System.Nullable`1[System.Boolean]') {
                for (var j = 0; j < methodData.length; j++) {
                    if (methodData[j].Id == 0 || methodData[j].Id == 9) {
                        tempMethodData.push(methodData[j]);
                    }
                }
                var tempData = [];
                if (fieldInfo.DicData) {
                    tempData = fieldInfo.DicData;
                }
                else if (fieldInfo.EnumData) {
                    tempData = fieldInfo.EnumData;
                }
                else if (fieldInfo.FieldType == 'System.Boolean') {
                    tempData = [{ Id: 1, Name: '是' }, { Id: 0, Name: '否' }];
                }
                else {
                    tempData = [{ Id: 1, Name: '是' }, { Id: 0, Name: '否' }];
                }
                if (fieldInfo.FieldType.indexOf("System.Nullable`1") > -1) {
                    tempData.push({ Id: null, Name: '空' });
                }
                valueObj.combobox({ valueField: 'Id', textField: 'Name', data: tempData });
            }
            else if (fieldInfo.FieldType == 'System.DateTime' || fieldInfo.FieldType == 'System.Nullable`1[System.DateTime]') {
                for (var j = 0; j < methodData.length; j++) {
                    if (methodData[j].Id == 1 || methodData[j].Id == 2 ||
                        methodData[j].Id == 3 || methodData[j].Id == 4) {
                        tempMethodData.push(methodData[j]);
                    }
                    if (fieldInfo.FieldType.indexOf("System.Nullable`1") > -1) {
                        if (methodData[j].Id == 0 || methodData[j].Id == 9) {
                            tempMethodData.push(methodData[j]);
                        }
                    }
                }
                if (fieldInfo.ControlType == 10) { //日期
                    valueObj.datebox({});
                }
                else if (fieldInfo.ControlType == 11) { //时间
                    valueObj.datetimebox({});
                }
            }
            else if (fieldInfo.ForeignModule || fieldInfo.ControlType == 0) {
                for (var j = 0; j < methodData.length; j++) {
                    if (methodData[j].Id != 1 && methodData[j].Id != 2 &&
                        methodData[j].Id != 3 && methodData[j].Id != 4) {
                        tempMethodData.push(methodData[j]);
                    }
                }
                var params = {};
                if (fieldInfo.ForeignModule) {
                    params.icons = [{
                        iconCls: 'eu-icon-search', handler: function (e) {
                            SelectModuleData(fieldInfo.ForeignModule, function (selectRow) {
                                valueObj.textbox('setText', selectRow[fieldInfo.ForeignTitleKey]);
                            });
                        }
                    }];
                }
                valueObj.textbox(params);
            }
            else if (fieldInfo.ControlType == 6 || fieldInfo.ControlType == 7) { //整型或浮点型字段
                for (var j = 0; j < methodData.length; j++) {
                    if (methodData[j].Id != 10 && methodData[j].Id != 11 && methodData[j].Id != 12) {
                        tempMethodData.push(methodData[j]);
                    }
                }
                valueObj.numberbox({});
            }
            methodObj.combobox('clear').combobox('loadData', tempMethodData);
            if (tempMethodData.length > 0) {
                methodObj.combobox('select', tempMethodData[0].Id);
            }
        }
    }
}

//获取行前缀
function GetGridRowIdPrefix() {
    var tr = $("div.datagrid-view2 div.datagrid-body tr.datagrid-row").eq(0);
    var tempId = tr.attr("id");
    var startIndex = tempId.lastIndexOf("-");
    return tempId.substr(0, startIndex + 1);
}

//获取条件
function GetCondition(backFun) {
    var type = $("input[name='rd_select']:checked").val();
    if (type == "2") { //按条件
        var gridObj = $('#conditionGrid');
        var rows = gridObj.datagrid("getRows");
        var data = [];
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            var rowIndex = gridObj.datagrid("getRowIndex", row); //行号
            //字段控件
            var fieldObj = $(gridObj.datagrid('getEditor', { index: rowIndex, field: 'Field' }).target);
            var fieldName = fieldObj.combobox('getValue'); //字段名
            var fieldInfo = null;
            for (var j = 0; j < fieldInfos.length; j++) {
                if (fieldName == fieldInfos[j].FieldName) {
                    fieldInfo = fieldInfos[j];
                    break;
                }
            }
            if (!fieldName || fieldInfo == null) {
                topWin.ShowMsg('提示', '第' + (rowIndex + 1) + '行字段未选择！');
                return;
            }
            //条件控件
            var methodObj = $(gridObj.datagrid('getEditor', { index: rowIndex, field: 'Method' }).target);
            //条件
            var method = methodObj.combobox('getValue');
            //值控件
            var valueObj = $('#value_' + rowIndex);
            var value = null;
            //条件控件
            var opObj = $(gridObj.datagrid('getEditor', { index: rowIndex, field: 'Group' }).target);
            var op = opObj.combobox('getValue');
            if (fieldInfo.DicData || fieldInfo.EnumData || fieldInfo.FieldType == 'System.Boolean' ||
                     fieldInfo.FieldType == 'System.Nullable`1[System.Boolean]') {
                value = valueObj.combobox('getValue');
            }
            else if (fieldInfo.FieldType == 'System.DateTime' || fieldInfo.FieldType == 'System.Nullable`1[System.DateTime]') {
                if (fieldInfo.ControlType == 10) { //日期
                    value = valueObj.datebox('getValue');
                }
                else if (fieldInfo.ControlType == 11) { //时间
                    value = valueObj.datetimebox('getValue');
                }
                if (fieldInfo.FieldType == 'System.DateTime' || (fieldInfo.FieldType == 'System.Nullable`1[System.DateTime]' && method != 0 && method != 9)) {
                    var regex = /^(\d{4})(\d{2})(\d{2})$/g
                    if (!regex.test(value)) { //判断日期格式符合YYYY-MM-DD标准
                        topWin.ShowMsg('提示', '第' + (rowIndex + 1) + '行日期字段值非法！');
                        return;
                    }
                }
            }
            else if (fieldInfo.ForeignModule || fieldInfo.ControlType == 0) {
                value = valueObj.textbox('getText');
            }
            else if (fieldInfo.ControlType == 6 || fieldInfo.ControlType == 7) { //整型或浮点型字段
                value = valueObj.numberbox('getValue');
            }
            data.push({ Field: fieldName, Method: method, Value: value, OrGroup: op });
        }
        if (typeof (backFun) == "function") {
            backFun(type, data);
            return;
        }
    }
    backFun(type);
}
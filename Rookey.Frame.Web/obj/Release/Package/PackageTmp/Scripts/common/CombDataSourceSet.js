//初始化
$(function () {
    //获取父页面的UrlOrData值
    var iframe = null;
    var pmode = GetLocalQueryString("pmode"); //父页面编辑模式
    if (pmode == 2) { //弹出框
        iframe = topWin.GetParentDialogFrame(); //取父弹出框iframe
    }
    else { //tab方式
        iframe = top.getCurrentTabFrame();
    }
    var formData = iframe[0].contentWindow.GetFormData(); //父Iframe的表单数据
    var urlOrdata = decodeURI(formData.UrlOrData);
    if (urlOrdata.indexOf('[{') == 0) { //有JSON数据
        var data = eval("(" + urlOrdata + ")");
        if (data && data.length > 0) {
            var gridObj = $('#valueGrid');
            for (var i = 0; i < data.length; i++) {
                AddNv(data[i].Name, data[i].Value);
            }
        }
    }
});

//向网格中添加name,value
function AddNameValue() {
    var name = $('#name').textbox('getText');
    if (!name) return;
    var value = $('#value').textbox('getText');
    AddNv(name, value);
    //添加完后清空
    $('#name').textbox('setText', '');
    $('#value').textbox('setText', '');
}

//添加name,value
function AddNv(name, value) {
    if (!name) return;
    var gridObj = $('#valueGrid');
    var rows = gridObj.datagrid("getRows");
    if (rows.length > 0) { //存在数据检查唯一性
        var row = null;
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].Name == name) {
                row = rows[i];
                break;
            }
        }
        if (row != null) {
            var rowIndex = gridObj.datagrid('getRowIndex', row);
            gridObj.datagrid('deleteRow', rowIndex);
            gridObj.datagrid('appendRow', { Op: '', Name: name, Value: value });
        }
        else {
            gridObj.datagrid('appendRow', { Op: '', Name: name, Value: value });
        }
    }
    else { //不存在数据直接添加
        gridObj.datagrid('appendRow', { Op: '', Name: name, Value: value });
    }
}

//删除一行
function DelNameValue(rowIndex) {
    $('#valueGrid').datagrid('deleteRow', rowIndex);
}

//操作列格式化
function OpFormatter(value, row, index) {
    return "<a href='#' onclick='DelNameValue(" + index + ")'>删除</a>";
}

//选择行数据后
function OnSelectDataRow(rowIndex, rowData) {
    $('#name').textbox('setText', rowData.Name);
    $('#value').textbox('setText', rowData.Value);
}

//获取所有name,value
function GetNameValues() {
    var gridObj = $('#valueGrid');
    var data = [];
    var rows = gridObj.datagrid("getRows");
    if (rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            data.push({ Name: rows[i].Name, Value: rows[i].Value });
        }
    }
    return data;
}
var page = GetLocalQueryString("page"); //页面类型标识
var moduleName = decodeURI(GetLocalQueryString("moduleName"));

$(function () {
    if (moduleName == "表单字段") {
        if (page == 'add' || page == 'edit') {
            $('#btn_addSource').attr('title', '点击添加下拉数据源');
        }
    }
});

//表单加载完成后
function OverFormLoadCompleted() {
    if (moduleName == "表单字段") {
        if (page == "add" || page == "edit") {
            //设置模块字段的下拉数据源
            var formId = $('#Sys_FormId').textbox('getValue');
            if (formId) { //根据当前表单视图绑定字段信息
                var flag = 6;
                if (page == "edit") flag = 7;
                var url = '/' + CommonController.Async_System_Controller + '/LoadFields.html?flag=' + flag + '&formId=' + formId;
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
    if (moduleName == "表单字段") {
        if (page == "add") {
            if (fieldName == 'Sys_FormId') { //选择字段后加载依赖字段数据源
                var formId = $('#Sys_FormId').textbox('getValue');
                var url = '/' + CommonController.Async_System_Controller + '/LoadFields.html?flag=6&formId=' + formId;
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

//为下拉框字段添加数据源
function AddDataSource(obj) {
    var title = '设置下拉数据源';
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var data = topWin.GetCurrentDialogFrame()[0].contentWindow.GetNameValues();
            if (data && data.length > 0) {
                var json = encodeURI(JSON.stringify(data).replace(/\"/g, "'"));
                $('#UrlOrData').textbox('setValue', json);
                $('#ValueField').textbox('setValue', 'Value');
                $('#TextField').textbox('setValue', 'Name');
            }
            topWin.CloseDialog();
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var pmode = $(obj).attr('pmode');
    var url = '/Page/CombDataSourceSet.html?pmode=' + pmode + '&r=' + Math.random();
    topWin.OpenDialog(title, url, toolbar, 500, 350, 'eu-icon-cog');
}
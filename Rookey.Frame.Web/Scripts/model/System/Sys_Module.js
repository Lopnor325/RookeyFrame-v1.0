//初始化
$(function () {
    $('#regon_main #btnDelete').attr('isHardDel', '1');
    if (page == 'add' || (page == 'edit' && $('#regon_').length > 0)) {
        var obj = $('#btnSave');
        //采用非快速保存方法
        $(obj).attr('noFast', '1');
        $(obj).attr('noTran', '1');
    }
});

//添加模块字段
function AddField() {
    var gridId = 'grid_';
    //先结束编辑所有行
    EndEditAllRows(gridId);
    //再增加一个编辑行
    AddRow(gridId, null, null, true);
    //获取当前行数
    var rows = GetCurrentRows(gridId);
    var rowNum = 1;
    if (rows != null && rows.length > 0) rowNum = rows.length;
    //初始化相关字段值
    var rowIndex = 0;// GetSelectRowIndex('grid_');
    //是否表单显示
    var isFormControl = GetGridEditorControl(rowIndex, 'IsEnableForm');
    isFormControl.attr("checked", "checked").val("1");
    //控件宽度
    var controlWidthControl = GetGridEditorControl(rowIndex, 'ControlWidth');
    controlWidthControl.numberbox('setValue', 180);
    //行号
    var rowNumControl = GetGridEditorControl(rowIndex, 'RowNum');
    rowNumControl.numberbox('setValue', rowNum);
    //列号
    var colNumControl = GetGridEditorControl(rowIndex, 'ColNum');
    colNumControl.numberbox('setValue', 1);
    //是否列表显示
    var isHeadDisplay = GetGridEditorControl(rowIndex, 'IsEnableGrid');
    isHeadDisplay.attr("checked", "checked").val("1");
    //列头宽度
    var headWidthControl = GetGridEditorControl(rowIndex, 'HeadWidth');
    headWidthControl.numberbox('setValue', 120);
    //列头排序编号
    var headSortControl = GetGridEditorControl(rowIndex, 'HeadSort');
    headSortControl.numberbox('setValue', rowNum);
}

//删除选中字段
function DelField() {
    var gridId = 'grid_';
    DelSelectRows(gridId);
}

//单击单元时触发
function OnCellClick(rowIndex, field, value) {
    var gridId = 'grid_';
    //先结束编辑所有行
    EndEditAllRows(gridId);
    //编辑当前选中行
    EditRow(gridId, rowIndex, null, null, true);
}

//选择字段类型触发事件
function OnFieldTypeSelected(record) {
    var trDom = $(this).parents('tr.datagrid-row');
    var rowIndex = trDom.attr('datagrid-row-index');
    //外键模块控件
    var foreignModuleControl = GetGridEditorControl(rowIndex, 'ForeignModuleName');
    foreignModuleControl.combobox('clear').combobox('loadData', [{ Name: '' }]).combobox('disable');
    //字段长度控件
    var fieldLenControl = GetGridEditorControl(rowIndex, 'FieldLen');
    //字段名控件
    var fieldNameControl = GetGridEditorControl(rowIndex, 'FieldName');
    fieldNameControl.textbox('enable');
    //列头宽度控件
    var headWidthControl = GetGridEditorControl(rowIndex, 'HeadWidth');
    var controlType = [];
    var defaultValue = 0;
    switch (record.id) {
        case 'varchar':
            {
                controlType.push({ id: 0, text: '文本框' });
                controlType.push({ id: 2, text: '多选CheckBox' });
                controlType.push({ id: 3, text: '下拉列表' });
                controlType.push({ id: 4, text: '下拉弹出列表' });
                controlType.push({ id: 5, text: '下拉树' });
                controlType.push({ id: 12, text: '文本域' });
                controlType.push({ id: 13, text: '富文本框' });
                controlType.push({ id: 15, text: '密码输入框' });
                controlType.push({ id: 16, text: '图标控件' });
                controlType.push({ id: 17, text: '文件上传' });
                controlType.push({ id: 26, text: '图片上传' });
                controlType.push({ id: 100, text: '标签控件' });
                fieldLenControl.textbox('setValue', '200').textbox('enable');
                //是否允许搜索
                var isAllowSearchControl = GetGridEditorControl(rowIndex, 'IsAllowGridSearch');
                isAllowSearchControl.attr("checked", "checked").val("1");
            }
            break;
        case 'int':
            {
                controlType.push({ id: 7, text: '整型数值' });
                defaultValue = 7;
                fieldLenControl.textbox('setValue', '4').textbox('disable');
                headWidthControl.numberbox('setValue', 80);
            }
            break;
        case 'guid':
            {
                controlType.push({ id: 8, text: '弹出列表框' });
                controlType.push({ id: 17, text: '弹出树' });
                fieldLenControl.textbox('setValue', '36').textbox('disable');
                headWidthControl.numberbox('setValue', 120);
                defaultValue = 8;
                var moduleName = $('#mainContent #Name').val();//当前模块名称
                var tableName = $('#mainContent #TableName').val();//当前模块表名
                var url = '/' + CommonController.Async_System_Controller + '/LoadModulesExp.html?moduleName=' + escape(moduleName) + '&tableName=' + tableName;
                foreignModuleControl.combobox('enable').combobox('reload', url);
            }
            break;
        case 'decimal':
            {
                controlType.push({ id: 6, text: '浮点数值' });
                headWidthControl.numberbox('setValue', 80);
                defaultValue = 6;
                fieldLenControl.textbox('setValue', '20').textbox('disable');
            }
            break;
        case 'bit':
            {
                controlType.push({ id: 1, text: '单选CheckBox' });
                controlType.push({ id: 9, text: '单选框组' });
                headWidthControl.numberbox('setValue', 60);
                defaultValue = 1;
                fieldLenControl.textbox('setValue', '1').textbox('disable');
            }
            break;
        case 'date':
            {
                controlType.push({ id: 10, text: '日期' });
                defaultValue = 10;
                fieldLenControl.textbox('setValue', '20').textbox('disable');
            }
            break;
        case 'datetime':
            {
                controlType.push({ id: 11, text: '日期时间' });
                headWidthControl.numberbox('setValue', 150);
                defaultValue = 11;
                fieldLenControl.textbox('setValue', '20').textbox('disable');
            }
            break;
    }
    controlType.push({ id: 30, text: '隐藏控件' });
    var controlTypeControl = GetGridEditorControl(rowIndex, 'ControlType');
    controlTypeControl.combobox('clear').combobox('loadData', controlType).combobox('setValue', defaultValue);
}

//选择外键模块后
function OnSelectForeignModule(record) {
    if (record && record.Name) {
        var trDom = $(this).parents('tr.datagrid-row');
        var rowIndex = trDom.attr('datagrid-row-index');
        //外键字段控件类型设置
        var controlType = [];
        controlType.push({ id: 8, text: '弹出列表框' });
        controlType.push({ id: 17, text: '弹出树' });
        controlType.push({ id: 3, text: '下拉列表' });
        controlType.push({ id: 4, text: '下拉弹出列表' });
        controlType.push({ id: 5, text: '下拉树' });
        var controlTypeControl = GetGridEditorControl(rowIndex, 'ControlType');
        controlTypeControl.combobox('clear').combobox('loadData', controlType).combobox('setValue', 8);
        //外键字段字段名固定
        var fieldNameControl = GetGridEditorControl(rowIndex, 'FieldName');
        var moduleName = $('#mainContent #Name').val();//当前模块名称
        if (moduleName && moduleName == record.Name) {
            fieldNameControl.textbox('setValue', 'ParentId').textbox('disable');
        }
        else {
            fieldNameControl.textbox('setValue', record.TableName + 'Id').textbox('disable');
        }
        //字段长度固定
        var fieldLenControl = GetGridEditorControl(rowIndex, 'FieldLen');
        fieldLenControl.textbox('setValue', '8').textbox('disable');
        //列头宽度控件
        var headWidthControl = GetGridEditorControl(rowIndex, 'HeadWidth');
        headWidthControl.numberbox('setValue', 120);
        //是否允许搜索
        var isAllowSearchControl = GetGridEditorControl(rowIndex, 'IsAllowGridSearch');
        isAllowSearchControl.attr("checked", "checked").val("1");
    }
}

//重写表单标签选择事件
function OverOnEditFormTabSelect(title, index) {
    if (title == '模块字段') {
        var moduleName = $('#mainContent #Name').textbox('getValue');//当前模块名称
        var tableName = $('#mainContent #TableName').textbox('getValue');//当前模块表名
        if (!moduleName || !tableName) {
            topWin.ShowAlertMsg("提示", "请先设置模块名称和模块表名的值！", "info", function () {
                $('#editFormTabs').tabs('select', 0);
            });
        }
    }
}

//格式化字段
function FormatField(value, row, index, field, data) {
    if (field == 'FieldType') {
        for (var i = 0; i < data.length; i++) {
            var dic = data[i];
            if (dic.id == value)
                return dic.text;
        }
    }
    else if (field == 'IsEnableForm' || field == 'AutoReCreateIndex' ||
        field == 'IsEnableGrid' || field == 'IsAllowGridSearch' || field == 'IsRequired') {
        if (value == 1)
            return "是";
        else
            return "否";
    }
    else if (field == 'ControlType') {
        var controlType = [];
        controlType.push({ id: 0, text: '文本框' });
        controlType.push({ id: 8, text: '弹出列表框' });
        controlType.push({ id: 3, text: '下拉列表' });
        controlType.push({ id: 4, text: '下拉弹出列表' });
        controlType.push({ id: 5, text: '下拉树' });
        controlType.push({ id: 12, text: '文本域' });
        controlType.push({ id: 13, text: '富文本框' });
        controlType.push({ id: 15, text: '密码输入框' });
        controlType.push({ id: 16, text: '图标控件' });
        controlType.push({ id: 17, text: '文件上传' });
        controlType.push({ id: 26, text: '图片上传' });
        controlType.push({ id: 11, text: '日期时间' });
        controlType.push({ id: 30, text: '隐藏控件' });
        controlType.push({ id: 10, text: '日期' });
        controlType.push({ id: 1, text: '单选CheckBox' });
        controlType.push({ id: 9, text: '单选框组' });
        controlType.push({ id: 6, text: '浮点数值' });
        controlType.push({ id: 7, text: '整型数值' });
        for (var i = 0; i < controlType.length; i++) {
            var dic = controlType[i];
            if (dic.id == value)
                return dic.text;
        }
    }
    return value ? value : "";
}

//重新生成模块
function ReCreateModule() {
    if (page != "grid") return;
    var row = GetSelectRow(); //获取选中行
    if (!row) {
        topWin.ShowAlertMsg("提示", "请选择一条记录！", "info"); //弹出提示信息
        return;
    }
    var msg = "确定要重新生成模块吗？重新生成模块后列表、表单、字段、字典绑定、附属模块设置等相关信息将重新生成！";
    topWin.ShowConfirmMsg('提示', msg, function (action) {
        if (action) {
            $.ajax({
                type: 'post',
                dataType: 'json',
                url: '/' + CommonController.Async_System_Controller + '/ReCreateModule.html',
                data: { moduleId: row["Id"] },
                beforeSend: function () {
                    topWin.OpenWaitDialog('模块正在重新生成...');
                },
                success: function (result) {
                    if (!result) return;
                    if (result.Success) {
                        topWin.ShowMsg('提示', '模块生成成功！', function () {
                            topWin.CloseWaitDialog();
                            topWin.CloseDialog();
                            topWin.location.reload();
                        });
                    }
                    else {
                        topWin.ShowAlertMsg('提示', result.Message, "info", function () {
                            topWin.CloseWaitDialog();
                        });
                    }
                },
                error: function () {
                    topWin.ShowAlertMsg('提示', '模块重新生成失败，服务器异常！', "error", function () {
                        topWin.CloseWaitDialog();
                    });
                }
            });
        }
    });
}

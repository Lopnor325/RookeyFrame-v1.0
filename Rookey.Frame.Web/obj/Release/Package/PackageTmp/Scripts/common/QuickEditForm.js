$(function () {
    $('#rightGrid').datagrid('enableCellEditing').datagrid('gotoCell', {
        index: 0,
        field: 'Id'
    });
});

//插入行
function InsertRow(row, index, select) {
    if (!row) return;
    var id = row["Id"];
    var gridObj = $('#rightGrid');
    var rows = gridObj.datagrid("getRows"); //当前的所有行
    var rowNum = rows.length + 1;
    if (index >= 0) {
        gridObj.datagrid('insertRow', {
            index: index,	// 索引从0开始
            row: {
                Id: row["Id"],
                FieldName: row["FieldName"],
                Display: row["Display"],
                ControlWidth: 180,
                RowNum: rowNum,
                ColNum: 1,
                CanEdit: '是'
            }
        });
    }
    else {
        gridObj.datagrid('appendRow', {
            Id: id,
            FieldName: row["FieldName"],
            Display: row["Display"],
            ControlWidth: 180,
            RowNum: rowNum,
            ColNum: 1,
            CanEdit: '是'
        });
    }
    if (select) {
        //清除所选
        gridObj.datagrid('clearSelections');
        //选中当前移动行
        gridObj.datagrid('selectRecord', id);
    }
}

//移动
function Move(rows) {
    if (rows && rows.length > 0) {
        //开始移入
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            var id = row["Id"];
            //先移出已经存在的
            var tempRows = $("#rightGrid").datagrid("getRows");
            if (tempRows && tempRows.length > 0) {
                var rmRow = null;
                for (var j = 0; j < tempRows.length; j++) {
                    if (id == tempRows[j].Id) {
                        rmRow = tempRows[j];
                        break;
                    }
                }
                if (rmRow != null) {
                    var index = $("#rightGrid").datagrid("getRowIndex", rmRow);
                    if (index >= 0) {
                        $("#rightGrid").datagrid("deleteRow", index);
                    }
                }
            }
            InsertRow(row, -1);
        }
    }
}

//移出
function MoveOut(rows) {
    if (rows && rows.length > 0) {
        //titlekey字段Id
        var titleKeyFieldId = $("#titleKeyFieldId").val();
        var copyRows = [];
        for (var j = 0; j < rows.length; j++) {
            if (rows[j].Id == titleKeyFieldId) continue;
            copyRows.push(rows[j]);
        }
        for (var i = 0; i < copyRows.length; i++) {
            var index = $('#rightGrid').datagrid('getRowIndex', copyRows[i]);
            $('#rightGrid').datagrid('deleteRow', index);
        }
    }
}

//移入选中字段
function RightMove() {
    var rows = $("#leftGrid").datagrid("getSelections");
    Move(rows);
}

//移入所有字段
function RightMoveAll() {
    var rows = $("#leftGrid").datagrid("getRows");
    Move(rows);
}

//移出选中字段
function LeftMove() {
    var rows = $("#rightGrid").datagrid("getSelections");
    MoveOut(rows);
}

//移出所有字段
function LeftMoveAll() {
    var rows = $("#rightGrid").datagrid("getRows");
    MoveOut(rows);
}

//保存表单
function SaveForm(backFun) {
    var moduleId = GetLocalQueryString("moduleId");
    var formId = GetLocalQueryString("formId");
    var formName = $("#FormName").val();
    var editMode = $("#EditMode").combobox("getValue");
    var rows = $("#rightGrid").datagrid("getRows");
    var fieldNames = "";
    var rowNums = "";
    var colNums = "";
    var canEdits = "";
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (fieldNames.length > 0) {
                fieldNames += ",";
                rowNums += ",";
                colNums += ",";
                canEdits += ",";
            }
            fieldNames += rows[i]["FieldName"];
            rowNums += rows[i]["RowNum"];
            colNums += rows[i]["ColNum"];
            canEdits += rows[i]["CanEdit"];
        }
    }
    if (fieldNames.length == 0) {
        if (typeof (backFun) == "function") {
            backFun({ Success: false, Message: '表单字段未设置！' });
        }
        return;
    }
    var data = { formName: formName, editMode: editMode, fieldNames: fieldNames, rowNums: rowNums, colNums: colNums, canEdits: canEdits, moduleId: moduleId, formId: formId };
    $.ajax({
        type: 'post',
        url: '/' + CommonController.Async_System_Controller + '/SaveForm.html',
        data: data,
        dataType: "json",
        success: function (result) {
            if (typeof (backFun) == "function") {
                backFun(result);
            }
        },
        error: function (err) {
            if (typeof (backFun) == "function") {
                backFun({ Success: false, Message: '表单保存失败，服务端异常！' });
            }
        }
    });
}


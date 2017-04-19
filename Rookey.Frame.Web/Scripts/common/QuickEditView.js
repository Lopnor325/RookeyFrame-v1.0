$(function () {
    //绑定操作事件
    BindOperateEvent();
});

//过滤分组字段
function FilterGroupFields(fields) {
    var groupFields = [];
    if (fields && fields.length > 0) {
        $.each(fields, function (i) {
            if (fields[i].Sys_FieldName != "Id") {
                groupFields.push(fields[i]);
            }
        });
    }
    return groupFields;
}

//绑定操作事件
function BindOperateEvent() {
    //绑定操作事件
    $("input[id^='btnUp_']").unbind("click").click(function (e) {
        StopBubble(e);
        var rowId = $(this).attr("rowId");
        if (rowId != undefined && rowId != null && rowId.length > 0) {
            Up(rowId);
        }
    });
    $("input[id^='btnTop_']").unbind("click").click(function (e) {
        StopBubble(e);
        var rowId = $(this).attr("rowId");
        if (rowId != undefined && rowId != null && rowId.length > 0) {
            Top(rowId);
        }
    });
    $("input[id^='btnDown_']").unbind("click").click(function (e) {
        StopBubble(e);
        var rowId = $(this).attr("rowId");
        if (rowId != undefined && rowId != null && rowId.length > 0) {
            Down(rowId);
        }
    });
    $("input[id^='btnBottom_']").unbind("click").click(function (e) {
        StopBubble(e);
        var rowId = $(this).attr("rowId");
        if (rowId != undefined && rowId != null && rowId.length > 0) {
            Bottom(rowId);
        }
    });
}

//插入行
function InsertRow(row, index, select) {
    if (!row) return;
    var id = row["Id"];
    var gridObj = $('#rightGrid');
    var operate = '<input id="btnUp_' + id + '" rowId="' + id + '" title="上移" type="button" style="width:30px;" value="↑" />';
    operate += '<input id="btnDown_' + id + '" rowId="' + id + '" title="下移" type="button" style="width:30px;" value="↓" />';
    operate += '<input id="btnTop_' + id + '" rowId="' + id + '" title="移至最顶部" type="button" style="width:30px;" value="↑↑" />';
    operate += '<input id="btnBottom_' + id + '" rowId="' + id + '" title="移至最底部" type="button" style="width:30px;" value="↓↓" />';
    if (index >= 0) {
        gridObj.datagrid('insertRow', {
            index: index,	// 索引从0开始
            row: {
                Id: row["Id"],
                FieldName: row["FieldName"],
                ModuleName: row["ModuleName"],
                Display: row["Display"],
                Operate: operate
            }
        });
    }
    else {
        gridObj.datagrid('appendRow', {
            Id: id,
            FieldName: row["FieldName"],
            ModuleName: row["ModuleName"],
            Display: row["Display"],
            Operate: operate
        });
    }
    if (select) {
        //清除所选
        gridObj.datagrid('clearSelections');
        //选中当前移动行
        gridObj.datagrid('selectRecord', id);
    }
    //绑定操作事件
    BindOperateEvent();
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

//向上移
function Up(id) {
    var rows = $("#rightGrid").datagrid("getRows");
    var row = null;
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (id == rows[i]["Id"]) {
                row = rows[i];
                break;
            }
        }
    }
    if (row != null) {
        var index = $("#rightGrid").datagrid("getRowIndex", row);
        if (index > 0) {
            //先移除当前行
            $("#rightGrid").datagrid("deleteRow", index);
            //再插入一行
            InsertRow(row, index - 1, true);
        }
    }
}

//向下移
function Down(id) {
    var rows = $("#rightGrid").datagrid("getRows");
    var row = null;
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (id == rows[i]["Id"]) {
                row = rows[i];
                break;
            }
        }
    }
    if (row != null) {
        var index = $("#rightGrid").datagrid("getRowIndex", row);
        if (index >= 0) {
            //先移除当前行
            $("#rightGrid").datagrid("deleteRow", index);
            //再插入一行
            InsertRow(row, index + 1, true);
        }
    }
}

//移动到顶部
function Top(id) {
    var rows = $("#rightGrid").datagrid("getRows");
    var row = null;
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (id == rows[i]["Id"]) {
                row = rows[i];
                break;
            }
        }
    }
    if (row != null) {
        var index = $("#rightGrid").datagrid("getRowIndex", row);
        if (index >= 0) {
            //先移除当前行
            $("#rightGrid").datagrid("deleteRow", index);
            //在第一行插入一行
            InsertRow(row, 0, true);
        }
    }
}

//移动到底部
function Bottom(id) {
    var rows = $("#rightGrid").datagrid("getRows");
    var row = null;
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (id == rows[i]["Id"]) {
                row = rows[i];
                break;
            }
        }
    }
    if (row != null) {
        var index = $("#rightGrid").datagrid("getRowIndex", row);
        if (index >= 0) {
            //先移除当前行
            $("#rightGrid").datagrid("deleteRow", index);
            //在最后再插入一行
            InsertRow(row, -1, true);
        }
    }
}

//保存视图
function SaveGridView(backFun) {
    var moduleId = GetLocalQueryString("moduleId");
    var viewId = GetLocalQueryString("viewId");
    var gridName = $("#GridName").val();
    var groupField = $("#GroupField").combobox("getValue");
    var treeField = $("#TreeField").combobox("getValue");
    var isDefault = $("#IsDefault").attr("checked") == "checked";
    var rows = $("#rightGrid").datagrid("getRows");
    var fieldNames = "";
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            if (fieldNames.length > 0)
                fieldNames += ",";
            fieldNames += rows[i]["FieldName"];
        }
    }
    if (fieldNames.length == 0) {
        if (typeof (backFun) == "function") {
            backFun({ Success: false, Message: '视图字段未设置！' });
        }
        return;
    }
    var data = { gridName: gridName, groupField: groupField, treeField: treeField, isDefault: isDefault, fieldNames: fieldNames, moduleId: moduleId, viewId: viewId };
    $.ajax({
        type: 'post',
        url: '/' + CommonController.Async_System_Controller + '/SaveGridView.html',
        data: data,
        dataType: "json",
        success: function (result) {
            if (typeof (backFun) == "function") {
                backFun(result);
            }
        },
        error: function (err) {
            if (typeof (backFun) == "function") {
                backFun({ Success: false, Message: '视图保存失败，服务端异常！' });
            }
        }
    });
}
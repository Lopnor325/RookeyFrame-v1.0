
//关联角色表单
function SetRoleForm(obj) {
    if (page == "grid") { //主网格页面
        var row = GetSelectRow();
        if (!row) {
            topWin.ShowMsg("提示", "请选择一条记录！", null, null, 1);
            //topWin.ShowAlertMsg("提示", "请选择一条记录！", "info"); //弹出提示信息
            return;
        }
        var roleId = row["Id"];
        var roleName = row["Name"];
        var toolbar = [{
            id: 'btnOk',
            text: "关 联",
            iconCls: "eu-icon-ok",
            handler: function (e) {
                topWin.OpenWaitDialog('数据保存中...');
                var formObj = topWin.GetCurrentDialogFrame()[0].contentWindow.GetSelectForm();
                if (formObj.FormId && formObj.FormId.length > 0) {
                    $.ajax({
                        type: 'post',
                        url: '/' + CommonController.Async_System_Controller + '/RelateRoleForm.html',
                        data: { roleId: roleId, formId: formObj.FormId, moduleId: formObj.ModuleId },
                        dataType: "json",
                        success: function (result) {
                            if (result.Success) {
                                topWin.ShowMsg('提示', '角色【' + roleName + '】表单关联成功！', function () {
                                    topWin.CloseDialog();
                                    topWin.CloseWaitDialog();
                                });
                            }
                            else {
                                topWin.ShowAlertMsg('提示', '角色关联表单失败，异常信息：' + result.Message, 'info', function () {
                                    topWin.CloseWaitDialog();
                                });
                            }
                        },
                        error: function (err) {
                            topWin.ShowAlertMsg('提示', '角色关联表单失败，服务器异常！', 'error', function () {
                                topWin.CloseWaitDialog();
                            });
                        }
                    });
                }
                else {
                    topWin.ShowAlertMsg('提示', '角色关联表单失败，未获取到正确的表单Id！', 'info', function () {
                        topWin.CloseWaitDialog();
                    });
                }
            }
        }, {
            id: 'btnClose',
            text: '关 闭',
            iconCls: "eu-icon-close",
            handler: function (e) {
                topWin.CloseDialog();
            }
        }];
        var url = '/Page/SetRoleForm.html?roleId=' + roleId + '&r=' + Math.random();
        topWin.OpenDialog('关联角色表单－' + roleName, url, toolbar, 400, 250, 'eu-icon-cog');
    }
}

//设置权限
function SetPermission(obj) {
    var toolbar = [{
        id: 'btnOk',
        text: "保 存",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            topWin.GetCurrentDialogFrame()[0].contentWindow.SaveRolePermission(function () {
                topWin.CloseDialog();
            });
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var row = GetSelectRow(); //获取选中行
    if (!row) { //没有选中行，从当前按钮中找对应的记录Id来得到选择行
        var selectId = $(obj).attr("recordId"); //要编辑的记录Id
        var rows = GetCurrentRows();
        for (var i = 0; i < rows.length; i++) {
            var tempRow = rows[i];
            if (selectId == tempRow["Id"]) {
                row = tempRow;
                break;
            }
        }
    }
    if (!row) {
        topWin.ShowMsg("提示", "请选择一条记录！", null, null, 1);
        //topWin.ShowAlertMsg("提示", "请选择一条记录！", "info"); //弹出提示信息
        return;
    }
    var roleId = row["Id"];
    var roleName = row["Name"];
    var url = '/Page/SetRolePermission.html?roleId=' + roleId + '&r=' + Math.random();
    topWin.OpenDialog('设置角色权限－' + roleName, url, toolbar, 900, 520, 'eu-icon-cog');
}


//设置用户角色
function SetUserRole(obj) {
    if (page == "grid") { //主网格页面
        var row = GetSelectRow();
        if (!row) {
            topWin.ShowMsg("提示", "请选择一条记录！", null, null, 1);
            //topWin.ShowAlertMsg("提示", "请选择一条记录！", "info"); //弹出提示信息
            return;
        }
        var userId = row["Id"];
        var userName = row["UserName"];
        var toolbar = [{
            id: 'btnOk',
            text: "确 定",
            iconCls: "eu-icon-ok",
            handler: function (e) {
                topWin.OpenWaitDialog('数据保存中...');
                var roleIds = topWin.GetCurrentDialogFrame()[0].contentWindow.GetSelectRoles();
                if (roleIds && roleIds.length > 0) {
                    $.ajax({
                        type: 'post',
                        url: '/' + CommonController.Async_System_Controller + '/SaveUserRole.html',
                        data: { userId: userId, roleIds: roleIds },
                        dataType: "json",
                        success: function (result) {
                            if (result.Success) {
                                topWin.ShowMsg('提示', '用户【' + userName + '】角色设置成功！', function () {
                                    topWin.CloseDialog();
                                    topWin.CloseWaitDialog();
                                });
                            }
                            else {
                                topWin.ShowAlertMsg('提示', '用户【' + userName + '】角色设置失败，异常信息：' + result.Message, 'info', function () {
                                    topWin.CloseWaitDialog();
                                });
                            }
                        },
                        error: function (err) {
                            topWin.ShowAlertMsg('提示', '用户【' + userName + '】角色设置失败，服务器异常！', 'error', function () {
                                topWin.CloseWaitDialog();
                            });
                        }
                    });
                }
                else {
                    topWin.ShowAlertMsg('提示', '角色关联表单失败，未获取到正确的角色Id！', 'info', function () {
                        topWin.CloseWaitDialog();
                    });
                }
            }
        }, {
            id: 'btnClose',
            text: '取 消',
            iconCls: "eu-icon-close",
            handler: function (e) {
                topWin.CloseDialog();
            }
        }];
        var url = '/Page/SetUserRole.html?userId=' + userId + '&r=' + Math.random();
        topWin.OpenDialog('设置用户【' + userName + '】角色', url, toolbar, 630, 382, 'eu-icon-cog');
    }
}

//设置权限
function SetPermission(obj) {
    var toolbar = [{
        id: 'btnOk',
        text: "保 存",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            topWin.GetCurrentDialogFrame()[0].contentWindow.SaveUserPermission(function () {
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
    var userId = row["Id"];
    var userName = row["UserName"];
    var url = '/Page/SetUserPermission.html?userId=' + userId + '&r=' + Math.random();
    topWin.OpenDialog('设置用户权限－' + userName, url, toolbar, 900, 520, 'eu-icon-cog');
}
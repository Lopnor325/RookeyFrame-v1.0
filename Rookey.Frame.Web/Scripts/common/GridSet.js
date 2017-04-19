
$(function () {
    $("#btnAddView").attr("title", "新增视图");
    $("#btnEditView").attr("title", "编辑当前视图");
    $("#btnDelView").attr("title", "删除当前视图");
});

//视图加载完成
function ViewLoadSuccess() {
    var obj = $("#txtView");
    var tempGridId = obj.attr("viewId");
    if (tempGridId) {
        obj.combobox("select", tempGridId);
        obj.removeAttr("viewId");
    }
    else {
        var viewId = GetLocalQueryString("viewId");
        obj.combobox("select", viewId);
    }
}

//选择视图后
function ViewSelected(record) {
    var viewId = record.Id;
    $.post('/' + CommonController.Async_System_Controller + '/IsGridViewCanOperate.html', { viewId: viewId },
        function (data) {
            if (data && data.Success) { //自己创建的视图可编辑删除
                $('#btnEditView').linkbutton('enable');
                $('#btnDelView').linkbutton('enable');
            }
            else { //系统视图编辑和删除按钮不可用
                $('#btnEditView').linkbutton('disable');
                $('#btnDelView').linkbutton('disable');
            }
            if (data) {
                var treeField = data.TreeField ? data.TreeField : "";
                $("#txtView").attr("treeField", treeField);
            }
        }, "json");
}

//添加视图
function AddView(obj) {
    var moduleId = $(obj).attr("moduleId");
    if (moduleId && moduleId != null && moduleId.length > 0) {
        var toolbar = [{
            text: "确 定",
            iconCls: "eu-icon-ok",
            handler: function (e) {
                topWin.OpenWaitDialog('视图保存中...');
                topWin.GetCurrentDialogFrame()[0].contentWindow.SaveGridView(function (result) {
                    topWin.CloseWaitDialog();
                    if (result && result.Success) {
                        var viewDom = $("#txtView");
                        viewDom.attr("viewId", result.ViewId);
                        viewDom.attr("treeField", result.TreeField && result.TreeField.length > 0 ? result.TreeField : "");
                        viewDom.combobox("reload");
                        topWin.CloseDialog();
                    }
                    else {
                        topWin.ShowAlertMsg("保存提示", result.Message, "info");
                    }
                });
            }
        }, {
            text: '关 闭',
            iconCls: "eu-icon-close",
            handler: function (e) {
                topWin.CloseDialog();
            }
        }];
        var url = '/Page/QuickEditView.html?moduleId=' + moduleId;
        if (isNfm) {
            url += '&nfm=1';
        }
        url += '&r=' + Math.random();
        var moduleName = $(obj).attr("moduleName");
        topWin.OpenDialog('新增视图－' + moduleName, url, toolbar, 630, 540);
    }
    else {
        topWin.ShowAlertMsg('提示', '找不到对应的模块！');
    }
}

//编辑视图
function EditView(obj) {
    var viewId = $("#txtView").combobox("getValue");
    if (!viewId || viewId.length <= 0) {
        topWin.ShowAlertMsg('提示', '请选择视图后再点击编辑视图按钮！');
        return;
    }
    var moduleId = $(obj).attr("moduleId");
    if (moduleId != undefined && moduleId != null && moduleId.length > 0) {
        $.ajax({
            type: 'post',
            url: '/' + CommonController.Async_System_Controller + '/IsGridViewCanOperate.html',
            data: { viewId: viewId },
            dataType: "json",
            success: function (data) {
                if (data && data.Success) {
                    var toolbar = [{
                        text: "确 定",
                        iconCls: "eu-icon-ok",
                        handler: function (e) {
                            topWin.OpenWaitDialog('视图保存中...');
                            topWin.GetCurrentDialogFrame()[0].contentWindow.SaveGridView(function (result) {
                                topWin.CloseWaitDialog();
                                if (result && result.Success) {
                                    var viewDom = $("#txtView");
                                    viewDom.attr("viewId", result.ViewId);
                                    viewDom.attr("treeField", result.TreeField && result.TreeField.length > 0 ? result.TreeField : "");
                                    viewDom.combobox("reload");
                                    topWin.CloseDialog();
                                }
                                else {
                                    topWin.ShowAlertMsg("保存提示", result.Message, "info");
                                }
                            });
                        }
                    }, {
                        text: '关 闭',
                        iconCls: "eu-icon-close",
                        handler: function (e) {
                            topWin.CloseDialog();
                        }
                    }];
                    var url = '/Page/QuickEditView.html?moduleId=' + moduleId + '&viewId=' + viewId;
                    if (isNfm) {
                        url += '&nfm=1';
                    }
                    url += '&r=' + Math.random();
                    var viewName = $("#txtView").combobox("getText");
                    topWin.OpenDialog('编辑视图－' + viewName, url, toolbar, 630, 540);
                }
                else {
                    topWin.ShowAlertMsg("提示", data.Message, "info");
                }
            },
            error: function (err) {
            }
        });
    }
    else {
        topWin.ShowAlertMsg('提示', '找不到对应的模块！');
    }
}

//删除用户视图
function DelView(obj) {
    var viewName = $("#txtView").combobox("getText");
    topWin.ShowConfirmMsg("删除视图", "确定要删除【" + viewName + "】视图吗？", function (ok) {
        if (ok) {
            var viewId = $("#txtView").combobox("getValue");
            if (!viewId || viewId.length <= 0) {
                topWin.ShowAlertMsg('提示', '请选择视图后再点击删除视图按钮！');
                return;
            }
            var moduleId = $(obj).attr("moduleId");
            if (moduleId != undefined && moduleId != null && moduleId.length > 0) {
                $.ajax({
                    type: 'post',
                    url: '/' + CommonController.Async_System_Controller + '/IsGridViewCanOperate.html',
                    data: { viewId: viewId },
                    dataType: "json",
                    beforeSend: function () {
                        topWin.OpenWaitDialog();
                    },
                    success: function (data) {
                        if (data && data.Success) {
                            var DelView = function () {
                                $.ajax({
                                    type: 'post',
                                    url: '/' + CommonController.Async_System_Controller + '/DelGridView.html',
                                    data: { viewId: viewId },
                                    dataType: "json",
                                    success: function (result) {
                                        topWin.CloseWaitDialog();
                                        if (result && result.Success) {
                                            topWin.ShowMsg('提示', '视图【' + viewName + '】删除成功！', function () {
                                                $("#txtView").removeAttr("viewId");
                                                $("#txtView").combobox("reload");
                                            });
                                        }
                                        else {
                                            topWin.ShowAlertMsg("提示", result.Message, "info");
                                        }
                                    },
                                    error: function (err) {
                                        topWin.CloseWaitDialog();
                                        topWin.ShowAlertMsg("提示", "删除失败，服务器异常！", "error");
                                    }
                                });
                            }
                            DelView();
                        }
                        else {
                            topWin.CloseWaitDialog();
                            topWin.ShowAlertMsg("提示", data.Message, "info");
                        }
                    },
                    error: function (err) {
                        topWin.CloseWaitDialog();
                        topWin.ShowAlertMsg("提示", "删除失败，服务器异常！", "error");
                    }
                });
            }
            else {
                topWin.ShowAlertMsg('提示', '找不到对应的模块！');
            }
        }
    });
}

//获取选中的视图
function GetSelectGridView() {
    var viewDom = $("#txtView");
    var viewId = viewDom.combobox("getValue");
    var treeField = viewDom.attr("treeField");
    return { ViewId: viewId, TreeField: treeField };
}
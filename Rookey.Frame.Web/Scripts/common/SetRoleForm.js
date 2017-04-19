$(function () {
    $("#btnAddForm").attr("title", "新增表单");
    $("#btnEditForm").attr("title", "编辑当前表单");
    $("#btnDelForm").attr("title", "删除当前表单");
});

//选择模块后
function ModuleSelected(record) {
    var roleId = GetLocalQueryString("roleId");
    $('#txtForm').combobox('clear').combobox('reload', '/' + CommonController.Async_System_Controller + '/LoadRoleForms.html?moduleId=' + record.Id + '&roleId=' + roleId);
}

//表单加载完成
function FormLoadSuccess() {
    var data = $('#txtForm').combobox('getData');
    if (data && data.length > 0) {
        $.each(data, function (i, item) {
            if (item.IsBind) {
                $('#txtForm').combobox('select', item.Id);
            }
        });
    }
}

//添加角色表单
function AddRoleForm(obj) {
    var moduleId = $("#txtModule").combobox("getValue");
    var moduleName = $("#txtModule").combobox("getText");
    if (!moduleId) {
        topWin.ShowAlertMsg('提示', '请先选择模块！');
        return;
    }
    var roleId = $(obj).attr("roleId");
    if (roleId != undefined && roleId != null && roleId.length > 0) {
        var toolbar = [{
            text: "确 定",
            iconCls: "eu-icon-ok",
            handler: function (e) {
                topWin.OpenWaitDialog('表单保存中...');
                topWin.GetCurrentDialogFrame()[0].contentWindow.SaveForm(function (result) {
                    topWin.CloseWaitDialog();
                    if (result && result.Success) {
                        var formDom = $("#txtForm");
                        formDom.combobox("reload").combobox("select", result.FormId);
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
        var url = '/Page/QuickEditForm.html?moduleId=' + moduleId + '&roleId=' + roleId + '&r=' + Math.random();
        var roleName = $(obj).attr("roleName");
        topWin.OpenDialog('角色【' + roleName + '】－新增【' + moduleName + '】表单', url, toolbar, 660, 540);
    }
    else {
        topWin.ShowAlertMsg('提示', '找不到对应的角色！');
    }
}

//编辑角色表单
function EditRoleForm(obj) {
    var formId = $("#txtForm").combobox("getValue");
    if (formId == undefined || formId == null || formId.length <= 0) {
        topWin.ShowAlertMsg('提示', '请选择表单后再点击编辑表单按钮！');
        return;
    }
    var roleId = $(obj).attr("roleId");
    if (roleId != undefined && roleId != null && roleId.length > 0) {
        var toolbar = [{
            text: "确 定",
            iconCls: "eu-icon-ok",
            handler: function (e) {
                topWin.OpenWaitDialog('表单保存中...');
                topWin.GetCurrentDialogFrame()[0].contentWindow.SaveForm(function (result) {
                    topWin.CloseWaitDialog();
                    if (result && result.Success) {
                        var formDom = $("#txtForm");
                        formDom.combobox("reload").combobox("select", result.FormId);
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
        var moduleId = $("#txtModule").combobox("getValue");
        var url = '/Page/QuickEditForm.html?moduleId=' + moduleId + '&roleId=' + roleId + '&formId=' + formId + '&r=' + Math.random();
        var moduleName = $("#txtModule").combobox("getText");
        var roleName = $(obj).attr("roleName");
        topWin.OpenDialog('角色【' + roleName + '】－编辑【' + moduleName + '】表单', url, toolbar, 630, 540);
    }
    else {
        topWin.ShowAlertMsg('提示', '找不到对应的角色！');
    }
}

//删除角色表单
function DelRoleForm(obj) {
    var formName = $("#txtForm").combobox("getText");
    var roleName = $(obj).attr("roleName");
    topWin.ShowConfirmMsg("删除角色表单", "确定要删除角色【" + roleName + "】的模块表单【" + formName + "】吗？", function (ok) {
        if (ok) {
            var formId = $("#txtForm").combobox("getValue");
            if (formId == undefined || formId == null || formId.length <= 0) {
                topWin.ShowAlertMsg('提示', '请选择表单后再点击删除表单按钮！');
                return;
            }
            var roleId = $(obj).attr("roleId");
            if (roleId != undefined && roleId != null && roleId.length > 0) {
                topWin.OpenWaitDialog('数据处理中...');
                $.ajax({
                    type: 'post',
                    url: '/' + CommonController.Async_System_Controller + '/DelRoleForm.html',
                    data: { roleId: roleId, formId: formId },
                    dataType: "json",
                    success: function (result) {
                        topWin.CloseWaitDialog();
                        if (result && result.Success) {
                            topWin.ShowMsg('提示', '角色【' + roleName + '】的模块表单【' + formName + '】删除成功！', function () {
                                $("#txtForm").combobox("reload");
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
            else {
                topWin.ShowAlertMsg('提示', '找不到对应的角色！');
            }
        }
    });
}

//获取选中的表单
function GetSelectForm() {
    var formDom = $("#txtForm");
    var formId = formDom.combobox("getValue");
    var moduleId = $("#txtModule").combobox("getValue");
    return { FormId: formId, ModuleId: moduleId };
}
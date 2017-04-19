//保存修改密码
function SaveModifyPwd(backFun) {
    var form = $("#changePwdForm");
    var flag = form.form("validate");
    if (!flag) return;
    var oldPwd = $('#oldPwd').textbox('getValue');
    var newPwd1 = $('#newPwd1').textbox('getValue');
    var newPwd2 = $('#newPwd2').textbox('getValue');
    if (newPwd1 != newPwd2) {
        top.ShowAlertMsg("提示", "两次输入的新密码不一致！");
        return;
    }
    $.ajax({
        type: 'post',
        url: '/UserAsync/ChangePwd.html',
        data: { newPwd: newPwd1, oldPwd: oldPwd },
        dataType: "json",
        beforeSend: function () {
            top.OpenWaitDialog('正在更新密码...');
        },
        success: function (result) {
            if (result && result.Success) {
                if (typeof (backFun) == "function") {
                    backFun();
                    top.CloseWaitDialog();
                }
            }
            else {
                top.ShowAlertMsg("提示", result.Message, "info", function () {
                    top.CloseWaitDialog();
                });
            }
        },
        error: function (err) {
            top.ShowAlertMsg("提示", "密码更新失败，服务端异常！", "error", function () {
                top.CloseWaitDialog();
            });
        }
    });
}

//下载导入模板
function DownImportTemp(obj) {
    var moduleId = $(obj).attr("moduleId");
    var downLoadUrl = '/' + CommonController.Async_Data_Controller + '/DownImportTemplate.html?moduleId=' + moduleId;
    if (typeof (OverDownImportTempUrl) == "function") { //提供下载模板重写方法
        var tempDownLoadUrl = OverDownImportTempUrl(downLoadUrl, obj);
        if (tempDownLoadUrl)
            downLoadUrl = tempDownLoadUrl;
    }
    window.open(downLoadUrl); //如果downLoadUrl中有nocreate=1标识，则在后台判断模板存在不重新生成模板，直接返回模板
}

//上传模板数据文件
//moduleId:模块Id
//backFun:上传成功后的回调函数
function UploadTempData(backFun) {
    var file = $('#file').val();
    if (!file) {
        topWin.ShowAlertMsg("提示", "请选择要上传的文件！", "info");
        return;
    }
    $("#form_Import").ajaxSubmit({
        type: "post",
        url: "/Annex/UploadTempImportFile.html",
        beforeSubmit: function () {
            topWin.OpenWaitDialog('处理中...');
        },
        success: function (result) {
            topWin.CloseWaitDialog();
            if (!result.Success) {
                topWin.ShowAlertMsg('提示', result.Message, 'info');
            }
            else {
                if (typeof (backFun) == "function") {
                    backFun(result.FilePath);
                }
            }
        },
        error: function (err) {
            topWin.CloseWaitDialog();
            topWin.ShowAlertMsg('提示', '数据上传失败，服务端异常！', 'error');
        },
        dataType: "json"
    });
}
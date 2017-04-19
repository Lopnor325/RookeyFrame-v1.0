var id = GetLocalQueryString("id"); //记录Id

//重写格式化函数
function OverGeneralFormatter(value, row, index, moduleName, fieldName, paramsObj) {
    if (fieldName == 'IconAddr') { //图标地址列显示图标
        var imgUrlPrefix = '';
        var iconClass = row["IconClass"]; //图标分类
        if (iconClass == 1) { //自定义图标
            imgUrlPrefix = "/Css/"
        }
        else if (iconClass == 2) { //系统图标
            imgUrlPrefix = "/Scripts/jquery-easyui/themes/"
        }
        else { //用户上传图标
            imgUrlPrefix = "";
        }
        var srcUrl = imgUrlPrefix + value;
        var s = "<img src='" + srcUrl + "' />";
        return value + s;
        //return "<table style><tr><td>" + value + "</td><td>&nbsp;" + s + "</td></tr></table>";
    }
    return value;
}

//图片上传验证
//fieldName:字段名
//backFun:回调函数
function ImgUploadControlVerify(fieldName, backFun) {
    if (fieldName == "IconAddr") {
        var dom = $("#StyleClassName");
        var styleClassName = dom.textbox('getText');
        if (!styleClassName || styleClassName.replace(' ', '').length == 0) {
            topWin.ShowAlertMsg('提示', '请先填写图标样式类名！', "info");
            return;
        }
        //判断系统中样式类名存不存在
        $.get("/" + CommonController.Async_System_Controller + "/IconStyleClassNameIsExists.html", { recordId: id, styleClassName: styleClassName }, function (result) {
            if (result && result.IsNotExists) { //不存在
                if (typeof (backFun) == "function") {
                    backFun(styleClassName);
                }
            }
            else {
                topWin.ShowAlertMsg('提示', '图标样式类名【' + styleClassName + '】已存在，不允许重复！', "info");
            }
        });
    }
}

//保存完成后将增加自定义图标样式
function OverAfterSaveCompleted(result) {
    $.ajax({
        type: "post",
        url: "/" + CommonController.Async_System_Controller + "/AddCustomerStyleClass.html",
        data: { recordId: result.RecordId },
        success: function (data) {
            if (data && !data.Success) {
                topWin.ShowAlertMsg('图标处理提示', '图标样式建立失败:' + data.Message + '！', "info");
            }
        },
        error: function (err) {
            topWin.ShowAlertMsg('图标处理提示', '图标样式建立失败:服务器异常！', "info");
        },
        dataType: "json"
    });
}

//保存前验证
function OverBeforeSaveVerify(backFun) {
    var styleClassName = $('#StyleClassName').textbox('getText');
    if (styleClassName.indexOf('icon-') == 0) {
        if (typeof (backFun) == "function") {
            backFun('样式类名不能以【icon-】开头，请重新设置样式类名！');
            return;
        }
    }
    backFun(null);
}

//获取选择的图标
function GetSelectedIcon() {
    var gridId = $("table[id^='grid_']").eq(0).attr('id');
    var row = GetSelectRow(gridId); //获取选中行
    if (!row) {
        topWin.ShowMsg('提示', '请选择图标！');
        return;
    }
    var imgUrl = row["IconAddr"]; //图标url
    var prex = "";
    if (row['IconClass'] == 1)
        prex = "/Css/";
    else if (row['IconClass'] == 2)
        prex = "/Scripts/jquery-easyui/themes/";
    imgUrl = prex + imgUrl;
    var styleClassName = row["StyleClassName"]; //图标样式类名
    return { ImgUrl: imgUrl, StyleClassName: styleClassName };
}

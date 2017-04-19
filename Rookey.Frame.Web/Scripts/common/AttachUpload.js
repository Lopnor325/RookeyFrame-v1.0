var page = GetLocalQueryString("page"); //源页面类型
var id = GetLocalQueryString("id"); //记录Id

$(function () {
    AddUpFile();
});

//添加上传控件
function AddUpFile() {
    var tbody = $("#form_Upload table:first tbody");
    var tr = document.createElement("tr");
    var fileTd = document.createElement("td");
    var addTd = document.createElement("td");
    var removeTd = document.createElement("td");
    var length = tbody.find("tr").size();
    //最多上传数量
    var count = 5;
    if (count < length) {
        mini.alert("已超出最大上传数【5】，无法添加！");
        return;
    }
    var fileHtml = "<input type='file' id='file" + (length + 1) + "' name='file' style='width:360px;margin-left:10px;' />";
    var addHtml = "<a id='btnAdd" + length + "' class='easyui-linkbutton' style='margin-right:5px;' iconCls='eu-p2-icon-add_other' name='a_Add'></a>";
    var removeHtml = "<a id='btnRemove" + length + "' class='easyui-linkbutton' style='margin-right:5px;' iconCls='eu-p2-icon-delete2' name='a_Remove'></a>";
    fileTd.innerHTML = fileHtml;
    addTd.innerHTML = addHtml;
    removeTd.innerHTML = removeHtml;
    tr.appendChild(fileTd);
    tr.appendChild(addTd);
    tr.appendChild(removeTd);
    tbody.append(tr);

    $.parser.parse();

    tbody.find("tr:last td a[name='a_Add']").parents("td:first").css("width", "20px");
    tbody.find("tr:last td a[name='a_Remove']").parents("td:first").css("width", "20px");
    tbody.find("tr:last td a[id^='btnAdd']").click(function () {
        AddUpFile();
    });
    tbody.find("tr:last td a[id^='btnRemove']").click(function () {
        RemoveUpFile($(this));
    });
}

//移除上传控件
function RemoveUpFile(t) {
    var length = $(t).parents("tbody").find("tr").size();
    if (length > 1) {
        $(t).parents("tr:first").remove();
        if (typeof (AttachDeleteFun) == "function") {
            AttachDeleteFun();
        }
    }
}

//清除所有上传控件
function ClearUpFile() {
    $("#form_Upload table:first tbody").html("");
    AddUpFile();
}

//文件上传方法
//moduleId:模块Id
//backFun:上传成功后回调函数
//attachType:附件类型
function UploadFile(moduleId, backFun, attachType) {
    var fileNames = []; //上传文件集合
    var sameFiles = ""; //相同文件集合
    $("input[id^='file']").each(function () {
        var isExists = false; //是否上传了相同的文件
        var fn = $(this).val();
        for (var i = 0; i < fileNames.length; i++) {
            if (fn == fileNames[i]) {
                isExists = true; //文件已经在上传列表中
            }
        }
        if (isExists) { //选择了相同的文件时提醒
            if (sameFiles) {
                sameFiles += "," + fn;
            }
            else {
                sameFiles = fn;
            }
        }
        else {
            fileNames.push(fn);
        }
    });
    if (sameFiles.length > 0) {
        topWin.ShowAlertMsg("提示", "文件【" + sameFiles + "】已经在上传列表中，请不要重复上传！", "error");
        return;
    }
    var data = { moduleId: moduleId };
    if (page == "view") {
        data["id"] = id;
    }
    if (attachType != undefined && attachType != null && attachType != '') {
        data["attachType"] = attachType;
    }
    $("#form_Upload").ajaxSubmit({
        type: "post",
        url: "/Annex/UploadAttachment.html",
        data: data,
        beforeSubmit: function () {
            topWin.OpenWaitDialog('附件上传中...');
        },
        success: function (result) {
            topWin.CloseWaitDialog();
            if (result.Success) {
                if (page == "add" || page == "edit") {
                    if (result.FileMsg) {
                        $("#attachFile").val(result.FileMsg);
                    }
                }
                if (typeof (backFun) == "function") {
                    backFun(result);
                }
            }
            else {
                topWin.ShowAlertMsg('提示', result.Message, 'info');
            }
        },
        error: function (err) {
            topWin.CloseWaitDialog();
            topWin.ShowAlertMsg('提示', '附件上传失败，服务端异常！', 'error');
        },
        dataType: "json"
    });
}

//获取表单上传的附件信息
function GetFormAttachFile() {
    var attachStr = $("#attachFile").val();
    return attachStr;
}
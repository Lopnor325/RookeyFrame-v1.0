var page = GetLocalQueryString("page"); //当前页面类型
var id = GetLocalQueryString("id"); //记录Id

$(function () {

});

//删除附件行记录
function DelAttachRow(attachId, fileName) {
    var attachGrid = $("#attachGrid");
    if (attachGrid && attachGrid.length > 0) {
        var msg = "确定删除文件名为【" + fileName + "】的记录？";
        topWin.ShowConfirmMsg('删除提示', msg, function (action) {
            if (action) {
                if (page == "add" || page == "edit") { //编辑页面
                    var rows = attachGrid.datagrid("getRows");
                    if (rows && rows.length > 0) {
                        var rowIndex = -1;
                        for (var i = 0; i < rows.length; i++) {
                            if (attachId == rows[i]["Id"]) {
                                rowIndex = attachGrid.datagrid("getRowIndex", rows[i]);
                            }
                        }
                        if (rowIndex > -1) {
                            attachGrid.datagrid("deleteRow", rowIndex); //删除列表行
                            //更新要保存的附件数据
                            var attachObj = [];
                            var oldAttachStr = decodeURIComponent($("#attachFile").val());
                            if (oldAttachStr && oldAttachStr.length > 0) {
                                var oldAttachObj = eval("(" + oldAttachStr + ")");
                                if (oldAttachObj && oldAttachObj.length > 0) {
                                    $.each(oldAttachObj, function (i, oldItem) {
                                        if (attachId != undefined && attachId != null && attachId.length > 0) { //记录有Id时，根据Id匹配
                                            if (oldItem["Id"] != attachId) {
                                                attachObj.push(oldItem);
                                            }
                                        }
                                        else { //没有Id根据文件名匹配
                                            if (oldItem["FileName"] != fileName) {
                                                attachObj.push(oldItem);
                                            }
                                        }
                                    });
                                }
                            }
                            $("#attachFile").val(JSON.stringify(attachObj));
                        }
                    }
                }
                else if (page == "view") { //查看页面
                    //直接删除对应的附件信息
                    DeleteAttach(attachId);
                    return;
                }
            }
        });
    }
}

//删除附件列表中选中的附件或者删除附件框中附件
function DelAttach(obj) {
    var removeAttach = function (rows) {
        var attachObj = [];
        var oldAttachStr = decodeURIComponent($("#attachFile").val());
        if (oldAttachStr && oldAttachStr.length > 0) {
            var oldAttachObj = eval("(" + oldAttachStr + ")");
            if (oldAttachObj && oldAttachObj.length > 0) {
                $.each(oldAttachObj, function (i, oldItem) {
                    var add = false; //是否添加
                    for (var i = 0; i < rows.length; i++) {
                        var rId = rows[i]["Id"];
                        if (rId != undefined && rId != null && rId.length > 0 && rId != '-1') { //记录有Id时，根据Id匹配
                            if (oldItem["Id"] != rId) {
                                add = true; //非移除项
                            }
                        }
                        else { //没有Id根据文件名匹配
                            if (oldItem["FileName"] != rows[i]["FileName"]) {
                                add = true;
                            }
                        }
                    }
                    if (add) {
                        attachObj.push(oldItem);
                    }
                });
            }
        }
        $("#attachFile").val(JSON.stringify(attachObj));
    };
    var attachGrid = $("#attachGrid");
    var rows = [];
    var msgTitle = "附件删除提示";
    if (attachGrid && attachGrid.length > 0) { //删除附件列表中选中的附件
        rows = attachGrid.datagrid("getSelections");
        if (!rows || rows.length == 0) {
            topWin.ShowAlertMsg("提示", "请至少选择一条记录！", "info");
            return;
        }
        var msg = "确定删除选中的记录？";
        topWin.ShowConfirmMsg(msgTitle, msg, function (action) {
            if (action) {
                for (var i = 0; i < rows.length; i++) {
                    var row = rows[i];
                    var rowIndex = attachGrid.datagrid("getRowIndex", row);
                    attachGrid.datagrid("deleteRow", rowIndex); //删除列表行
                    if (page == "view") { //查看页面
                        DeleteAttach(row["Id"]); //直接删除附件
                        continue;
                    }
                }
                if (page != 'view')
                    removeAttach(rows);
            }
            else {
                return;
            }
        });
    }
    else { //删除附件框中附件
        var attachId = $(obj).attr("AttachId");
        var fileName = $(obj).attr("FileName");
        rows.push({ Id: attachId, FileName: fileName });
        var msg = "确定删除附件【" + fileName + "】？";
        topWin.ShowConfirmMsg(msgTitle, msg, function (action) {
            if (action) {
                if (typeof (OverDeleteAttachment) == "function") {
                    OverDeleteAttachment(obj);
                }
                if (attachId != undefined && attachId != null && attachId.length > 0 && attachId != '-1') {
                    $("#btn_file_" + attachId).remove();
                    $("#btn_remove_" + attachId).remove();
                    if ($("#btn_view_" + attachId).length > 0 && $("#btn_view_" + attachId).parent().length > 0) {
                        $("#btn_view_" + attachId).parent().remove();
                    }
                    if (page == "view") { //查看页面
                        //直接删除对应的附件信息
                        DeleteAttach(attachId, function () {
                            $("#btn_download_" + attachId).remove();
                        });
                        return;
                    }
                }
                else {
                    var tempBtnId = $(obj).attr("BtnId");
                    $("#btn_file_" + tempBtnId).remove();
                    $("#btn_remove_" + tempBtnId).remove();
                }
                if (page != 'view')
                    removeAttach(rows);
            }
            else {
                return;
            }
        });
    }
}

//下载附件
//attachId:附件Id
function DownloadAttach(attachId) {
    var downLoadUrl = '/Annex/DownloadAttachment.html?attachId=' + attachId;
    window.open(downLoadUrl);
}

//删除附件，直接删除附件数据及附件文件，慎用
//attachIds，附件Id集合，多个以逗号分隔
//backFun:回调函数
function DeleteAttach(attachIds, backFun) {
    var msgTitle = '附件删除提示';
    $.ajax({
        type: "post",
        url: "/Annex/DeleteAttachment.html?r=" + Math.random(),
        data: { attachIds: attachIds },
        dataType: "json",
        beforeSend: function () {
            topWin.OpenWaitDialog('附件删除中...');
        },
        success: function (result) {
            topWin.CloseWaitDialog();
            if (result.Success) {
                topWin.ShowMsg(msgTitle, "附件删除成功！");
            }
            else {
                topWin.ShowAlertMsg(msgTitle, result.Message, 'info');
            }
            if (typeof (backFun) == "function") {
                backFun();
            }
        },
        error: function (err) {
            topWin.CloseWaitDialog();
            topWin.ShowAlertMsg(msgTitle, "附件删除失败，服务器异常！", 'error');
        }
    });
}

//上传文件
//obj:当前dom对象
//backFun:回调函数
function UploadFile(obj, backFun) {
    var moduleId = $(obj).attr('moduleId');
    var moduleName = $(obj).attr("moduleName"); //模块名称
    var attachType = $(obj).attr("attachType"); //附件类型
    var attachDisplayStyle = $(obj).attr("attachDisplayStyle"); //附件显示方式
    var toolbar = [{
        id: 'btnOk',
        text: '确 定',
        iconCls: 'eu-icon-ok',
        handler: function (e) {
            var iframe = topWin.GetCurrentDialogFrame();
            iframe[0].contentWindow.UploadFile(moduleId, function (result) {
                if (typeof (backFun) != "function") {
                    if (page == "add" || page == "edit") {
                        var attachStr = iframe[0].contentWindow.GetFormAttachFile();
                        if (!attachStr) return;
                        var attachObj = eval("(" + attachStr + ")");
                        if (!attachObj || attachObj.length == 0) return;
                        if (typeof (OverBeforeUploadFile) == "function") { //附件验证
                            var errMsg = OverBeforeUploadFile(obj, attachObj);
                            if (errMsg && errMsg.length > 0) //验证不通过
                                return;
                        }
                        var attachGrid = $("#attachGrid");
                        $.each(attachObj, function (i, item) {
                            if (attachDisplayStyle == 0) { //简单显示附件
                                var guid = Guid.NewGuid();
                                var tempId = guid.ToString();
                                $("#attachPanel").append("<a id='btn_file_" + tempId + "'>" + item.FileName + "</a>");
                                $("#attachPanel").append("<a id='btn_remove_" + tempId + "' style='margin-right:10px;' class='easyui-linkbutton' data-options=\"iconCls:'eu-p2-icon-delete2',plain:true\" onclick='DelAttach(this)' AttachId='-1' BtnId='" + tempId + "' FileName='" + item.FileName + "'></a>");
                            }
                            else { //列表方式时将附件信息添加到列表
                                attachGrid.datagrid('appendRow', item);
                            }
                        });
                        $.parser.parse("#attachPanel");
                        var oldAttachStr = decodeURIComponent($("#attachFile").val()); //旧的附件
                        if (oldAttachStr && oldAttachStr.length > 0) {
                            var oldAttachObj = eval("(" + oldAttachStr + ")");
                            if (oldAttachObj && oldAttachObj.length > 0) {
                                $.each(oldAttachObj, function (i, oldItem) {
                                    attachObj.push(oldItem);
                                });
                            }
                        }
                        $("#attachFile").val(JSON.stringify(attachObj));
                        if (typeof (OverAfterUploadFile) == "function") {
                            OverAfterUploadFile(obj, attachStr);
                        }
                    }
                    else if (page == "view" && result.AddAttachs) {
                        if (result.AddAttachs.length > 0) {
                            var attachObj = result.AddAttachs;
                            var attachGrid = $("#attachGrid");
                            $.each(attachObj, function (i, item) {
                                if (attachDisplayStyle == 0) { //简单显示附件
                                    var tempId = item.Id;
                                    $("#attachPanel").append("<a id='btn_file_" + tempId + "' target='_blank' href='" + item.AttachFile + "'>" + item.FileName + "</a>");
                                    $("#attachPanel").append("<a id='btn_remove_" + tempId + "' style='margin-right:10px;' class='easyui-linkbutton' data-options=\"iconCls:'eu-p2-icon-delete2',plain:true\" onclick='DelAttach(this)' AttachId='" + tempId + "' FileName='" + item.FileName + "'></a>");
                                    $("#attachPanel").append("<a id='btn_download_" + tempId + "' style='margin-right:10px;' class='easyui-linkbutton' data-options=\"iconCls:'eu-icon-arrow_down',plain:true\" onclick=\"window.open( '/Annex/DownloadAttachment.html?attachId=" + tempId + "')\"></a>");
                                }
                                else { //列表方式时将附件信息添加到列表
                                    attachGrid.datagrid('appendRow', item);
                                }
                            });
                            $.parser.parse("#attachPanel");
                        }
                    }
                    topWin.CloseDialog();
                }
                else { //有回调函数
                    backFun(result);
                }
            }, attachType);
        }
    }, {
        id: 'btnClear',
        text: '清除所有',
        iconCls: 'eu-p2-icon-delete2',
        handler: function (e) {
            topWin.GetCurrentDialogFrame()[0].contentWindow.ClearUpFile();
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: 'eu-icon-close',
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var url = "/Page/UploadForm.html?moduleId=" + moduleId + "&page=" + page + "&id=" + id + "&r=" + Math.random();
    if (isNfm) {
        url += '&nfm=1';
    }
    topWin.OpenDialog('上传附件', url, toolbar, 450, 300, 'eu-icon-calendar');
}

//保存表单附件
function SaveFormAttach(moduleId, id, backFun) {
    var fileMsg = $("#attachFile").val();
    if (!fileMsg || fileMsg.length == 0) {
        if (typeof (backFun) == "function") {
            backFun();
        }
        return;
    }
    $.ajax({
        type: "post",
        url: "/Annex/SaveFormAttach.html",
        data: { moduleId: moduleId, id: id, fileMsg: fileMsg },
        dataType: "json",
        beforeSend: function () {
            topWin.OpenWaitDialog('附件保存中...');
        },
        success: function (result) {
            topWin.CloseWaitDialog();
            if (result.Success) {
                if (typeof (backFun) == "function") {
                    backFun();
                }
            }
            else {
                topWin.ShowAlertMsg('提示', result.Message, 'info');
            }
        },
        error: function (err) {
            topWin.CloseWaitDialog();
            topWin.ShowAlertMsg('提示', "附件信息保存失败，服务器异常！", 'error');
        }
    });
}
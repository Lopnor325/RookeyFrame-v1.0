var id = GetLocalQueryString("id"); //记录Id
var page = GetLocalQueryString("page"); //页面类型标识
var toDoTaskId = GetLocalQueryString("todoId"); //待办任务ID
var moduleName = GetLocalQueryString("moduleName"); //模块名称

//初始化
$(function () {
    var mode = GetLocalQueryString("mode");
    if (mode == 2) { //弹出框时移除页面内部按钮
        $('#divFormBtn').remove();
    }
    //checkbox控件处理
    $("input[type=checkbox]").click(function () {
        if ($(this).attr("checked")) {
            $(this).attr("value", "1");
        }
        else {
            $(this).attr("value", "0");
        }
    });
    //html标签上存在这个类会导致滚动条无法下拉到最底部，一直处于闪烁状态
    $("html").removeClass("panel-fit");
    $("#detailTab .datagrid-wrap").css('border-top-width', '0px')
                                  .css('border-left-width', '0px')
                                  .css('border-bottom-width', '0px');
    //明细弹出编辑页面数据处理
    var ff = GetLocalQueryString("ff"); //主从编辑页面弹出明细编辑标识
    if (ff == 1) { //是从主从编辑页面弹出明细编辑页面
        var detailModuleId = GetLocalQueryString("moduleId"); //明细模块Id
        var pmode = GetLocalQueryString("pmode"); //父页面编辑模式
        var gridId = "grid_" + detailModuleId;
        if (page == "edit") { //编辑页面
            //从明细编辑网格加载数据
            var row = null;
            if (pmode == 2) { //弹出框
                row = topWin.GetParentDialogFrame()[0].contentWindow.GetSelectRow(gridId);
            }
            else { //tab方式
                row = !isNfm ? top.getCurrentTabFrame()[0].contentWindow.GetSelectRow(gridId) : topWin.GetSelectRow(gridId);
            }
            var form = $("#mainform");
            var formFields = GetFormFields();
            JsonToForm(row, form, formFields); //数据绑定到表单
        }
        var fg = GetLocalQueryString("fg"); //主网格页面的明细网格或附属模块网格标识
        var obj = null;
        if (fg == 1) { //加载明细模块或附属模块对应主模块选择行字段值
            if (pmode == 2) { //弹出框
                obj = topWin.GetParentDialogFrame()[0].contentWindow.GetSelectRowTitleKeyValue();
            }
            else { //tab方式
                obj = !isNfm ? top.getCurrentTabFrame()[0].contentWindow.GetSelectRowTitleKeyValue() : topWin.GetSelectRowTitleKeyValue();
            }
        }
        else { //加载对应主表的外键字段值
            if (pmode == 2) { //弹出框
                obj = topWin.GetParentDialogFrame()[0].contentWindow.GetTitleKeyValue();
            }
            else { //tab方式
                obj = !isNfm ? top.getCurrentTabFrame()[0].contentWindow.GetTitleKeyValue() : topWin.GetTitleKeyValue();
            }
        }
        if (obj) {
            var tempText = $("#" + obj.foreignFieldName).textbox("getText");
            if (obj.recordId) { //针对查看页面，明细新增
                $("#" + obj.foreignFieldName).attr('value', obj.recordId);
                $("#" + obj.foreignFieldName).attr('v', obj.recordId);
            }
            if (!tempText || tempText.length == 36) { //没有给对应的主表外键字段赋值时
                $("#" + obj.foreignFieldName).textbox("setText", obj.value);
            }
        }
    }
    else {
        //外键字段处理
        $("input[foreignField='1']").each(function () {
            var controlId = $(this).attr("Id");
            var control = $("#" + controlId);
            var v = control.textbox('getValue');
            if (v) {
                control.attr('v', v);
            }
            var textValue = $(this).attr("textValue");
            //设置外键字段的text值
            control.textbox("setText", textValue);
            //父模块外键字段标识
            var isParentForeignField = $(this).attr("isParentForeignField");
            if (isParentForeignField == "1") { //父模块外键字段没有初始化值是允许编辑
                var value = control.textbox("getValue"); //字段值
                if (!value) {
                    control.textbox("enable");
                }
            }
        });
    }
    //表单字段智能提示处理
    var formFields = GetFormFields();
    if (formFields && formFields.length > 0) {
        var rn = 100; //行号
        var cn = 100; //列号
        var focusFieldName = null; //需要光标定位的字段
        var moduleId = GetLocalQueryString("moduleId"); //模块Id
        var moduleName = decodeURIComponent(GetLocalQueryString("moduleName")); //模块名称
        var moduleIdOrModuleName = moduleId.length > 0 ? moduleId : moduleName;
        $.each(formFields, function (i, obj) {
            if (obj.ForeignModuleName && obj.ForeignTitleKey && (obj.ControlType == 8 || obj.ControlType == 17)) { //外键字段
                var fieldDom = $('#' + obj.Sys_FieldName);
                var tempDom = fieldDom.next('span').find('input.textbox-text');
                var displayTemplete = null;
                //自定义自动完成显示模板
                if (typeof (OverGetAutoCompletedDisplayTemplete) == "function") {
                    displayTemplete = OverGetAutoCompletedDisplayTemplete(obj.Sys_FieldName, obj.ForeignModuleName);
                }
                FieldBindAutoCompleted(tempDom, moduleIdOrModuleName, obj.Sys_FieldName, displayTemplete, function (dom, item, fieldName, paramObj) {
                    var oldValue = paramObj.textbox("getValue");
                    var valueField = 'Id'; //值字段名
                    var textField = 'f_Name'; //文本字段名
                    paramObj.attr('value', item[valueField]);
                    paramObj.attr('v', item[valueField]);
                    paramObj.textbox("setValue", item[valueField]);
                    paramObj.textbox("setText", item[textField]);
                    OnFieldSelect(item, fieldName, valueField, textField); //触发字段选择事件
                    if (oldValue != item[valueField]) { //触发字段值改变事件
                        OnFieldValueChanged({ moduleId: moduleId, moduleName: moduleName }, fieldName, item[valueField], oldValue);
                    }
                }, fieldDom, "autoFlag=1", obj.ForeignModuleName);
            }
            if (obj.CanEdit && (obj.ControlType == 0 || obj.ControlType == 6 ||
                obj.ControlType == 7 | obj.ControlType == 8 || obj.ControlType == 12) &&
                obj.RowNo <= rn && obj.ColNo <= cn) {
                rn = obj.RowNo;
                cn = obj.ColNo;
                focusFieldName = obj.Sys_FieldName;
            }
            if (obj.ControlType == 6 || obj.ControlType == 7 || (obj.ControlType == 100 &&
                (obj.FieldType == "System.Int32" || obj.FieldType == "System.Int64" ||
                obj.FieldType == "System.Nullable`1[System.Int32]" || obj.FieldType == "System.Nullable`1[System.Int64]" ||
                obj.FieldType == "System.Decimal" || obj.FieldType == "System.Nullable`1[System.Decimal]" ||
                obj.FieldType == "System.Double" || obj.FieldType == "System.Nullable`1[System.Double]"))) {
                var fieldDom = $("#" + obj.Sys_FieldName);
                if (fieldDom.is('span')) {
                    var oldValue = fieldDom.text();
                    var value = NumThousandsFormat(oldValue);
                    fieldDom.text(value);
                }
            }
        });
        if (focusFieldName) {
            try {
                $('#' + focusFieldName).parent().find("input.textbox-value[name='" + focusFieldName + "']").parent().find('input.textbox-text').focus();
            } catch (e) { }
        }
    }
    //自定义表单加载完成事件
    if (typeof (OverFormLoadCompleted) == "function") {
        OverFormLoadCompleted();
    }
    $('body').css('position', 'absolute');
});

//表单保存
//obj：按钮对象
//backFun:保存成功后的回调函数
//isAddNew:保存成功后是否继续新增
//isDraft:是否为草稿
function Save(obj, backFun, isAddNew, isDraft) {
    var form = $("#mainform");
    var nosave = $(obj).attr('nosave'); //等于1时流程操作不保存表单标识
    if (nosave != "1") { //需要验证保存
        //外键字段处理
        var $foreinInputs = form.find("#mainContent input[foreignField='1']");
        if ($foreinInputs.length > 0) {
            $foreinInputs.each(function () {
                var foreinName = $(this).attr("id");
                if (foreinName) {
                    var obj = form.find("input.textbox-value[name='" + foreinName + "']");
                    if (obj.length > 0) {
                        var v = $(this).attr('v');
                        if (v == undefined) v = '';
                        var textValue = $(this).next('span').find('input.textbox-text').val(); //外键名称为空时将值也清空
                        if (!textValue) {
                            $(this).textbox('clear');
                            v = '';
                        }
                        obj.attr('value', v);
                    }
                }
            });
        }
        //表单验证
        var flag = form.form("validate");
        if (!flag) return;
    }
    var msgTitle = '保存提示';
    var successMsg = '保存成功';
    var isFlowPage = false; //流程表单页面
    var isNoFastSave = $(obj).attr('noFast') == '1'; //采用非快速保存方法
    var isNoTran = $(obj).attr('noTran') == '1'; //采用非事务模式保存
    var detail = $(obj).attr("detail"); //是否明细编辑页面
    if ($(obj).attr('flowflag') && nosave == "1") {
        detail = false;
    }
    else {
        detail = detail && detail == "true" ? true : false;
    }
    //保存方法
    var ExecuteSave = function () {
        if (nosave != "1") {
            //有自定义保存方法则先调用自定义方法否则调用通用
            if (typeof (OverSave) == "function") {
                OverSave(obj, backFun, isAddNew, isDraft);
                return;
            }
        }
        var editMode = $(obj).attr("editMode"); //当前编辑模式
        if (editMode)
            editMode = parseInt(editMode);
        else
            editMode = 1; //默认标签编辑模式
        var tempModuleId = $(obj).attr("moduleId") || GetLocalQueryString("moduleId");
        var tempModuleName = $(obj).attr("moduleName") || decodeURIComponent(GetLocalQueryString("moduleName"));
        if (!detail) { //正常编辑保存
            var formObject = { ModuleId: tempModuleId, ModuleName: tempModuleName };
            if (nosave != "1") { //需要保存数据
                var data = form.fixedSerializeArrayFix();
                var updateFields = [];
                if (!isNoFastSave) { //快速保存方式
                    var tempData = {};
                    $.each(data, function (i, item) {
                        var name = item.name;
                        if (name != 'Id') {
                            var value = item.value;
                            if (value == '') {
                                var formField = GetFormField(name);
                                if (formField != null) {
                                    if (formField.ControlType == 6 || formField.ControlType == 7 || formField.IsEnum == true) {
                                        if (formField.IsCanNull)
                                            value = null;
                                        else
                                            value = 0;
                                    }
                                }
                            }
                            updateFields.push(name);
                            tempData[name] = value;
                        }
                    });
                    if (id)
                        tempData['Id'] = id;
                    data = tempData;
                }
                else {
                    if (id) {
                        data.push({ name: "Id", value: id });
                    }
                }
                //查看页面新增功能处理
                var ff = GetLocalQueryString("ff"); //主从编辑页面弹出明细编辑标识
                var fgFlag = GetLocalQueryString("fg"); //是否是主网格下方明细或附属模块新增标识
                if (ff == 1) { //是从主从查看页面弹出明细新增页面
                    //加载对应主表的外键字段值
                    var pmode = GetLocalQueryString("pmode"); //父页面编辑模式
                    var tempObj = null;
                    if (pmode == 2) { //弹出框
                        tempObj = topWin.GetParentDialogFrame()[0].contentWindow.GetTitleKeyValue()
                    }
                    else { //tab方式
                        tempObj = !isNfm ? top.getCurrentTabFrame()[0].contentWindow.GetTitleKeyValue() : topWin.GetTitleKeyValue();
                    }
                    if (tempObj && tempObj.recordId && tempObj.foreignFieldName) { //主表记录Id
                        if (!isNoFastSave) { //快速保存方式
                            data[tempObj.foreignFieldName] = tempObj.recordId;
                        }
                        else {
                            data.push({ name: tempObj.foreignFieldName, value: tempObj.recordId });
                        }
                    }
                    else {
                        topWin.ShowAlertMsg(msgTitle, "对应的主表外键字段值获取失败！", "info");
                        return;
                    }
                }
                //调用主模块自定义数据处理函数
                if (typeof (OverMainModuleDataHandleBeforeSaved) == "function") {
                    OverMainModuleDataHandleBeforeSaved(data);
                }
                if (!isNoFastSave) { //快速保存方式
                    data = JSON.stringify(data);
                }
                //组装表单数据对象
                var release = parseInt($(obj).attr("release")) == 1;
                formObject['ModuleData'] = data;
                formObject['IsDraft'] = isDraft ? true : false;
                formObject['IsReleaseDraft'] = release;
                if (!isNoFastSave) { //快速保存方式
                    formObject['NeedUpdateFields'] = updateFields;
                }
                var hasDetail = $("div[id^='regon_']").length > 0; //是否有明细模块
                if (hasDetail) { //有明细模块
                    //组装明细数据
                    var details = [];
                    $("div[id^='regon_']").each(function () {
                        var moduleDatas = [];
                        //明细模块Id
                        var detailModuleId = $(this).attr("moduleId");
                        //明细模块名称
                        var detailModuleName = $(this).attr("moduleName");
                        var detailUpdateFields = []; //明细更新字段
                        if (typeof (OverGetDeailData) != "function") { //没有重写方法
                            var detailGridId = 'grid_';
                            if (detailModuleId) detailGridId = 'grid_' + detailModuleId;
                            //明细结束编辑
                            EndEditAllRows(detailGridId);
                            //组装数据
                            var gridObj = $("#" + detailGridId);
                            var rows = gridObj.datagrid("getRows");
                            for (var i = 0; i < rows.length; i++) {
                                var row = rows[i]; //一条明细数据对象
                                //调用明细模块自定义数据处理函数
                                if (typeof (OverDetailModuleDataHandleBeforeSaved) == "function") {
                                    row = OverDetailModuleDataHandleBeforeSaved(row, detailModuleId, detailModuleName, detailUpdateFields);
                                }
                                if (isNoFastSave) {
                                    //数据组装
                                    var detailDatas = [];
                                    for (var p in row) {
                                        if (p != 'Id') {
                                            detailDatas.push({ name: p, value: row[p] });
                                        }
                                    }
                                    moduleDatas.push(JSON.stringify(detailDatas));
                                }
                                else {
                                    if (detailUpdateFields.length == 0) {
                                        for (var p in row) {
                                            detailUpdateFields.push(p);
                                        }
                                    }
                                    moduleDatas.push(JSON.stringify(row));
                                }
                            }
                        }
                        else { //有重写获取明细数据方法
                            moduleDatas = OverGetDeailData(detailModuleId, detailModuleName, detailUpdateFields);
                        }
                        var detailObj = { ModuleId: detailModuleId, ModuleName: detailModuleName };
                        if (moduleDatas && moduleDatas.length > 0) {
                            if (!isNoFastSave) { //快速保存方式
                                detailObj['NeedUpdateFields'] = detailUpdateFields;
                            }
                            detailObj.ModuleDatas = moduleDatas;
                        }
                        else {
                            detailObj.ModuleDatas = null;
                        }
                        details.push(detailObj);
                    });
                    if (details.length > 0) { //有明细数据
                        formObject["Details"] = details;
                    }
                }
            }
            //流程处理
            var btnTagId = $(obj).attr('id');
            if (btnTagId != null && btnTagId.length > 0 && btnTagId.indexOf('flowBtn_') > -1) {
                var flowBtnId = btnTagId.replace('flowBtn_', '');
                formObject.OpFlowBtnId = flowBtnId;
                isFlowPage = true;
                if (toDoTaskId != undefined && toDoTaskId != null && toDoTaskId.length > 0) {
                    formObject.ToDoTaskId = toDoTaskId;
                    if ($('#txt_approvalOpinions').length > 0)
                        formObject.ApprovalOpinions = $('#txt_approvalOpinions').textbox('getValue');
                    if ($(obj).attr('childTodoIds')) { //当前为子流程
                        formObject.ChildTodoIds = $(obj).attr('childTodoIds');
                    }
                    msgTitle = '审批流程提示';
                    successMsg = '流程操作成功！';
                }
                else {
                    msgTitle = '发起流程提示';
                    successMsg = '流程发起成功！';
                }
                var returnNodeId = $(obj).attr('returnNodeId'); //针对回退时回退结点
                var directHandler = $(obj).attr('directHandler'); //针对指派时指派人id
                if (returnNodeId != undefined && returnNodeId != null && returnNodeId.length > 0) {
                    formObject.ReturnNodeId = returnNodeId;
                }
                if (directHandler != undefined && directHandler != null && directHandler.length > 0) {
                    formObject.DirectHandler = directHandler;
                    msgTitle = '流程指派提示';
                    successMsg = '流程指派成功！';
                }
            }
            //开始保存
            var url = "/" + CommonController.Async_Data_Controller + "/SaveData.html";
            var params = { formData: $.base64.encode(escape(JSON.stringify(formObject).Replace("\\+", "%20"))), noFast: isNoFastSave ? '1' : '0', noTran: isNoTran ? '1' : '0' };
            if (nosave == "1") params['nosave'] = 1;
            $.ajax({
                type: "post",
                url: url,
                data: params,
                beforeSend: function () {
                    topWin.OpenWaitDialog('拼命处理中...');
                },
                success: function (result) {
                    if (result.Success) { //保存成功
                        if (editMode == 1) { //tab编辑模式
                            var recordId = page == 'add' ? result.RecordId : id;
                            var tempFun = function () {
                                topWin.ShowMsg(msgTitle, successMsg, function () {
                                    //保存成功后的回调函数
                                    topWin.CloseWaitDialog();
                                    //刷新网格
                                    var tb = GetLocalQueryString("tb"); //网格对应的tabindex
                                    if (tb && parseInt(tb) > 0) {
                                        if (!isNfm) {
                                            var tempIframe = top.getTabFrame(parseInt(tb));
                                            if (tempIframe.length > 0) {
                                                try {
                                                    tempIframe[0].contentWindow.RefreshGrid(gridId);
                                                } catch (er) { }
                                            }
                                        }
                                    }
                                    //自定义回调
                                    if (typeof (backFun) == "function") {
                                        backFun(result);
                                    }
                                    //自定义保存完成事件处理函数
                                    if (typeof (OverAfterSaveCompleted) == "function") {
                                        OverAfterSaveCompleted(result);
                                    }
                                    if (!isFlowPage) { //非流程表单页面
                                        if (!isNfm) { //非嵌入其他系统
                                            var fp = GetLocalQueryString("fp");
                                            if (fp == 'grid') { //来自网格页面
                                                var tab = GetSelectedTab();
                                                if (isAddNew) { //保存后新增
                                                    var addUrl = "/Page/EditForm.html?page=add&moduleId=" + tempModuleId + "&r=" + Math.random();
                                                    var title = "新增" + tempModuleName;
                                                    //跳转到新增页面
                                                    UpdateTab(null, tab, addUrl, title);
                                                }
                                                else {
                                                    CloseTab();
                                                    /*---------------------------
                                                    var viewUrl = "/Page/ViewForm.html?page=view&moduleId=" + tempModuleId + "&id=" + recordId;
                                                    viewUrl += "&mode=" + editMode + "&r=" + Math.random();
                                                    //跳转到查看页面
                                                    var title = tab.panel('options').title.replace("编辑", "查看").replace("新增", "查看");
                                                    UpdateTab(null, tab, viewUrl, title);
                                                    -----------------------------*/
                                                }
                                            }
                                        }
                                        else { //嵌入其他系统时
                                            CloseTab(null, false, true);
                                        }
                                    }
                                    else { //流程审批表单页面
                                        if (!formObject.ChildTodoIds) { //非子流程审批
                                            if (!isNfm) { //刷新桌面待办
                                                if (toDoTaskId) {
                                                    try {
                                                        var iframe = top.getTabFrame(0);
                                                        iframe[0].contentWindow.UpdateWorkTodo();
                                                    } catch (err) { }
                                                }
                                            }
                                            //关闭当前tab
                                            CloseTab(null, true);
                                        }
                                    }
                                });
                            }
                            if (recordId != undefined && recordId != null && typeof (SaveFormAttach) == "function") { //有附件需要保存
                                SaveFormAttach(tempModuleId, recordId, function () {
                                    tempFun();
                                });
                            }
                            else {
                                tempFun();
                            }
                        }
                        else if (editMode == 2 || editMode == 4) { //弹出框编辑模式或网格表单编辑模式
                            var recordId = page == 'add' ? result.RecordId : id;
                            var pmode = GetLocalQueryString("pmode"); //父页面编辑模式
                            var gridId = $(obj).attr("gridId");
                            var getGridWin = function () {
                                if (!isNfm) {
                                    if (pmode == 2) { //父页面为弹出框
                                        return topWin.GetCurrentDialogFrame()[0].contentWindow; //取当前弹出框window
                                    }
                                    else { //tab方式
                                        return top.getCurrentTabFrame()[0].contentWindow;
                                    }
                                }
                                else {
                                    return parent;
                                }
                            }
                            var gridWin = getGridWin();
                            var tempFun = function () {
                                if (editMode == 2 || (editMode == 4 && !id)) { //弹出框或网格表单编辑新增弹出框页面
                                    topWin.CloseDialog();
                                }
                                if (editMode == 2 || (editMode == 4 && !id)) {
                                    //刷新当前grid
                                    gridWin.RefreshGrid(gridId);
                                }
                                //关闭对话框
                                topWin.ShowMsg(msgTitle, successMsg, function () {
                                    //保存成功后的回调函数
                                    topWin.CloseWaitDialog();
                                    if (typeof (backFun) == "function") {
                                        backFun(result);
                                    }
                                    //自定义保存完成事件处理函数
                                    if (typeof (OverAfterSaveCompleted) == "function") {
                                        OverAfterSaveCompleted(result);
                                    }
                                    if (!isFlowPage) { //非流程表单页面
                                        var fp = GetLocalQueryString("fp");
                                        if (fp == 'grid') { //来自网格页面
                                            if (editMode == 4 && id) { //网格表单编辑时
                                                //刷新当前grid
                                                gridWin.RefreshGrid(gridId);
                                            }
                                            if (isAddNew) { //保存后新增，继续弹出新增对话框
                                                var addNewRecord = function () {
                                                    var iframe = null;
                                                    if (!isNfm) {
                                                        if (pmode == 2) { //父页面为弹出框
                                                            iframe = topWin.GetCurrentDialogFrame(); //取当前弹出框iframe
                                                        }
                                                        else { //tab方式
                                                            iframe = top.getCurrentTabFrame();
                                                        }
                                                    }
                                                    //找工具栏上的新增按钮
                                                    var dom = iframe != null ? iframe.contents().find("#btnAdd") : gridWin.$("#btnAdd");
                                                    if (dom.length == 0) { //新增按钮不在网格工具栏上，从网格内行头找
                                                        var tempDom = iframe != null ? iframe.contents().find("div[id^='rowOperateDiv_'").eq(0) : gridWin.$("div[id^='rowOperateDiv_'").eq(0);
                                                        var tag = tempDom.attr("Id").replace("rowOperateDiv_", "");
                                                        dom = iframe != null ? iframe.contents().find("#btnAdd_" + tag) : gridWin.$("#btnAdd_" + tag);
                                                    }
                                                    if (dom.length > 0) {
                                                        dom.click();
                                                    }
                                                }
                                                addNewRecord();
                                            }
                                        }
                                    }
                                });
                            }
                            if (recordId != undefined && recordId != null && typeof (SaveFormAttach) == "function") { //有附件需要保存
                                SaveFormAttach(tempModuleId, recordId, function () {
                                    tempFun();
                                });
                            }
                            else {
                                tempFun();
                            }
                        }
                    }
                    else {
                        if (result.RecordId != undefined && result.RecordId != null && result.RecordId != GuidEmpty)
                            id = result.RecordId;
                        topWin.CloseWaitDialog();
                        topWin.ShowAlertMsg(msgTitle, result.Message, "info");
                    }
                },
                error: function (err) {
                    topWin.CloseWaitDialog();
                    topWin.ShowAlertMsg(msgTitle, '数据保存失败，服务器异常！', "error");
                },
                dataType: "json"
            });
        }
        else { //明细编辑保存
            if (editMode == 2) { //弹出框编辑模式
                var gridId = "grid_" + tempModuleId;
                var row = GetFormData(true);
                var pmode = GetLocalQueryString("pmode"); //父页面编辑模式
                var iframe = null;
                var getEditFormWin = function () {
                    if (!isNfm) {
                        if (pmode == 2) { //父页面为弹出框
                            iframe = topWin.GetParentDialogFrame();
                            return iframe[0].contentWindow; //取当前弹出框window
                        }
                        else { //tab方式
                            iframe = top.getCurrentTabFrame();
                            return iframe[0].contentWindow;
                        }
                    }
                    else {
                        return parent;
                    }
                }
                var winObj = getEditFormWin();
                if (page == "add") {
                    winObj.AppendRow(gridId, row);
                }
                else if (page == "edit") {
                    var rowIndex = winObj.GetSelectRowIndex(gridId);
                    winObj.UpdateRow(gridId, rowIndex, row);
                }
                //明细弹出编辑保存完成后，当前页面为明细页面
                if (typeof (OverDialogDetailEditAfterSave) == "function") {
                    OverDialogDetailEditAfterSave(row, gridId, page, winObj);
                }
                topWin.CloseDialog();
                if (isAddNew) { //继续新增
                    var dom = iframe != null ? iframe.contents().find("div[id^='regon_'] a[id^='btnAdd']") : winObj.find("div[id^='regon_'] a[id^='btnAdd']");
                    dom.click();
                }
            }
        }
    }
    if (detail) { //明细模块
        //有自定义明细保存前验证方法
        if (typeof (OverDetailBeforeSaveVerify) == "function") {
            //调用后执行回调函数返回验证异常信息
            OverDetailBeforeSaveVerify(function (errMsg) {
                if (errMsg && errMsg.length > 0) { //验证不通过
                    topWin.ShowAlertMsg(msgTitle, errMsg, "info");
                    return;
                }
                else {
                    ExecuteSave();
                }
            }, obj);
        }
        else {
            ExecuteSave();
        }
    }
    else { //主模块
        if (nosave != "1") {
            //有自定义保存前验证方法
            if (typeof (OverBeforeSaveVerify) == "function") {
                //调用后执行回调函数返回验证异常信息
                OverBeforeSaveVerify(function (errMsg) {
                    if (errMsg && errMsg.length > 0) { //验证不通过
                        topWin.ShowAlertMsg(msgTitle, errMsg, "info");
                        return;
                    }
                    else {
                        ExecuteSave();
                    }
                }, obj);
            }
            else {
                ExecuteSave();
            }
        }
        else { //不需要保存时不验证
            ExecuteSave();
        }
    }
}

//提交流程
//obj：按钮对象
function SubmitFlow(obj) {
    topWin.ShowConfirmMsg('流程提交', '确定要提交流程吗？', function (action) {
        if (action) {
            Save(obj);
        }
    });
}

//审批流程，同意、拒绝
//obj：按钮对象
//confirmBackFun:确认回调
function ApprovalFlow(obj, confirmBackFun) {
    var btnText = $(obj).find('span.l-btn-text').text();
    var msg = '确定要' + btnText + '吗？';
    var backFun = null;
    if ($(obj).attr('parentToDoId')) { //子流程审批
        var selectTab = GetSelectedTab($('#detailTab'));
        var gridObj = selectTab.find("table[id^='grid_']");
        var rows = gridObj.datagrid('getSelections'); //获取所有选中的行
        if (rows && rows.length > 0) { //审批选中记录
            msg = '确定要对选中明细单据进行' + btnText + '操作处理吗？';
        }
        else { //审批所有行
            msg = '确定要对所有明细单据进行' + btnText + '操作处理吗？';
            rows = gridObj.datagrid('getRows');
        }
        if (!rows || rows.length == 0) {
            topWin.ShowMsg('提示', '获取明细记录失败！');
            return;
        }
        if (rows.length == 1)
            msg = '确定要对当前明细单据进行' + btnText + '操作处理吗？';
        if (rows.length == gridObj.datagrid('getRows').length) { //当前处理所有子流程
            $(obj).attr('handleAllFlow', '1');
        }
        else {
            $(obj).removeAttr('handleAllFlow');
        }
        var todoIds = "";
        for (var i = 0; i < rows.length; i++) {
            if (rows[i]["TaskToDoId"] != null)
                todoIds += rows[i]["TaskToDoId"] + ",";
        }
        if (todoIds.length > 0)
            todoIds = todoIds.substr(0, todoIds.length - 1);
        if (todoIds.length == 0) {
            topWin.ShowMsg('提示', '获取明细待办ID失败！');
            return;
        }
        $(obj).attr('childTodoIds', todoIds);
        backFun = function (result) {
            if (result.Success) {
                gridObj.datagrid('clearSelections');
                gridObj.datagrid("reload");
            }
            if (result.Message) {
                topWin.ShowMsg('提示', result.Message);
            }
            else if ($(obj).attr('handleAllFlow') == '1') { //处理了所有流程关闭当前tab
                CloseTab(null, true); //关闭tab
                if (!isNfm) { //刷新桌面待办
                    var iframe = top != null ? top.getTabFrame(0) : topWin.getTabFrame(0);
                    iframe[0].contentWindow.UpdateWorkTodo();
                }
            }
        };
    }
    topWin.ShowConfirmMsg('确认', msg, function (action) {
        if (typeof (confirmBackFun) == "function") {
            confirmBackFun(action);
        }
        if (action) {
            Save(obj, backFun);
        }
    });
}

//回退流程，针对流程回退
//obj：按钮对象
function BackFlow(obj) {
    var tempTodoId = toDoTaskId;
    if ($(obj).attr('parentToDoId')) { //子流程审批
        var selectTab = GetSelectedTab($('#detailTab'));
        var gridObj = selectTab.find("table[id^='grid_']");
        var rows = gridObj.datagrid('getRows'); //获取当前所有的行
        if (!rows || rows.length == 0) {
            topWin.ShowMsg('提示', '没有明细数据');
            return;
        }
        if (!rows[0]["TaskToDoId"]) {
            topWin.ShowMsg('提示', '明细数据待办ID丢失');
            return;
        }
        tempTodoId = rows[0]["TaskToDoId"];
    }
    var btnText = $(obj).find('span.l-btn-text').text();
    ExecuteCommonAjax('/' + CommonController.Async_Bpm_Controller + '/LoadBackNode.html', { toDoTaskId: tempTodoId }, function (result) {
        if (result != null && result.html != null) {
            topWin.OpenOkCancelDialog('选择回退结点', result.html, 270, 200, function (iframe, backFun) {
                var returnNode = topWin.$("input[name='backNodes']").val();
                $(obj).attr('returnNodeId', returnNode);
                ApprovalFlow(obj, function (action) {
                    if (action) {
                        if (typeof (backFun) == "function")
                            backFun(true);
                    }
                });
            });
        }
    }, false, true);
}

//指派流程，专门针对流程指派
//obj：按钮对象
function DirectFlow(obj) {
    var btnText = $(obj).find('span.l-btn-text').text();
    SelectModuleTree('员工管理', null, false, function (row) {
        var empId = row['Id'];
        $(obj).attr('directHandler', empId);
        ApprovalFlow(obj, function (action) {
            if (!action) {
                $(obj).removeAttr('directHandler');
            }
        });
    });
}

//获取表单数据
//handleForeignField:是否处理外键字段
function GetFormData(handleForeignField) {
    var form = $("#mainform");
    var data = form.fixedSerializeArrayFix();
    var formData = {};
    $.each(data, function (index) {
        var name = this['name'];
        var value = this["value"];
        var field = GetFormField(name);
        if (!formData[name]) {
            if (field != null && name != 'Id') {
                if (value == '') {
                    if (field.IsCanNull) { //可空类型
                        value = null;
                    }
                    else { //特殊处理
                        if (field.ControlType == 6 || field.ControlType == 7 || field.IsEnum == true) {
                            value = 0;
                        }
                    }
                }
            }
            formData[name] = value;
        }
        if (handleForeignField) {
            if (field != null && field.ForeignModuleName.length > 0 && field.ControlType != 100) { //是外键字段，并且不是标签字段
                var textName = name.substr(0, name.length - 2) + "Name";
                var textValue = $("#" + name, form).textbox("getText");
                formData[textName] = textValue;
            }
        }
    });
    $("#mainform span[fieldSpan='1'],#mainform span[fieldSpan='0']").each(function (i, item) {
        var fieldName = $(item).attr('id');
        if (fieldName) {
            var field = GetFormField(fieldName);
            if (field) {
                var value = $('#' + fieldName).attr('value');
                if (value == '') {
                    if (field.IsCanNull) { //可空类型
                        value = null;
                    }
                    else { //特殊处理
                        if (field.ControlType == 6 || field.ControlType == 7 || field.IsEnum == true) {
                            value = 0;
                        }
                    }
                }
                formData[fieldName] = value;
                if (handleForeignField && field.ForeignModuleName.length > 0) {
                    var textName = fieldName.substr(0, fieldName.length - 2) + "Name";
                    var textValue = $('#' + fieldName).text();
                    formData[textName] = textValue;
                }
            }
        }
    });
    return formData;
}

//获取表单的titlekey字段值
function GetTitleKeyValue() {
    var field = GetTitleKeyField();
    if (field && field.Sys_FieldName) {
        var name = field.Sys_FieldName;
        var value = null;
        try {
            value = $("#" + name).textbox('getValue');
        } catch (e) {
            value = $("#" + name).text();
        }
        return { name: name, value: value, foreignFieldName: field.ForignFieldName, recordId: id };
    }
    return null;
}

//下拉框、下拉列表、下拉树的下拉数据加载失败事件
//fieldName:字段名
//valueField:值字段
//textField:显示字段
//arguments:在数据加载失败的时候触发，arguments参数和jQuery的$.ajax()函数里面的'error'回调函数的参数相同。
function OnLoadError(fieldName, valueField, textField, arguments) {
    //调用模块自定义事件
    if (typeof (OverOnLoadError) == "function") {
        OnLoadError(fieldName, valueField, textField, arguments);
    }
}

//下拉框、下拉列表、下拉树数据项选择事件
//record:选择的项
//fieldName:字段名
//valueField:值字段
//textField:显示字段
function OnFieldSelect(record, fieldName, valueField, textField) {
    //调用模块自定义事件
    if (typeof (OverOnFieldSelect) == "function") {
        OverOnFieldSelect(record, fieldName, valueField, textField);
    }
}

//字段值改变事件
//moduleInfo:模块信息
//fieldName:字段名
//newValue:改变后的值
//oldValue:改变前的值
function OnFieldValueChanged(moduleInfo, fieldName, newValue, oldValue) {
    var linkFields = $('#' + fieldName).attr('linkFields');
    if (linkFields && linkFields.length > 0) { //该字段存在值关联字段
        var token = linkFields.split(',');
        for (var i = 0; i < token.length; i++) {
            var tempDom = $('#' + token[i]);
            if (!tempDom.textbox('getValue')) { //新增或编辑时，如果值关联字段为空
                tempDom.textbox('setValue', newValue);
            }
        }
    }
    //调用模块自定义事件
    if (typeof (OverOnFieldValueChanged) == "function") {
        OverOnFieldValueChanged(moduleInfo, fieldName, newValue, oldValue);
    }
}

//编辑表单的标签选择事件
//title:标签页面标题
//index://标签序号
function OnEditFormTabSelect(title, index) {
    if (typeof (OverOnEditFormTabSelect) == "function") {
        OverOnEditFormTabSelect(title, index);
    }
}

//获取表单数据
function GetFormDataObj() {
    var dataJson = $('#formDataObj').val();
    if (dataJson && dataJson.length > 0) {
        var obj = decodeURIComponent(dataJson);
        return obj;
    }
    return null;
}


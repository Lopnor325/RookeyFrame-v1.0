var workflowId = GetLocalQueryString("workflowId"); //当前流程ID
var tagId = GetLocalQueryString('tagId');

//初始化
$(function () {
    //checkbox控件处理
    $("input[type=checkbox]").click(function () {
        var domId = $(this).attr('id');
        if ($(this).attr("checked")) {
            $(this).attr("value", "1");
            if (domId == 'BackBtnEnabled') {
                $('#trBackType').show();
            }
        }
        else {
            $(this).attr("value", "0");
            if (domId == 'BackBtnEnabled') {
                $('#trBackType').hide();
            }
        }
    });
    $('.panel-header').css('border-top-width', '0px').
                       css('border-left-width', '0px');
    $("div[name='divPanel']").css('border-left-width', '0px');
    if ($('#Name').textbox('getValue') == '') {
        var name = decodeURI(GetLocalQueryString('name')); //节点名称
        $('#Name').textbox('setValue', name);
    }
    $('#Name').textbox({
        onChange: function (newValue, oldValue) {
            var tempDisplay = $('#DisplayName').textbox('getValue');
            if (!tempDisplay && newValue)
                $('#DisplayName').textbox('setValue', newValue);
        }
    });
    //节点表单
    $('#Sys_FormId').combobox({
        url: '/' + CommonController.Async_Bpm_Controller + '/GetNodeForms.html?workflowId=' + workflowId + '&tagId=' + tagId,
        valueField: 'Id',
        textField: 'Name',
        loadFilter: function (data) {
            var tempData = data;
            if (!data) {
                tempData = [];
            }
            tempData.push({ Id: GuidEmpty, Name: '自定义表单' });
            return tempData;
        },
        onLoadSuccess: function () {
            var v = $('#Sys_FormId').combobox('getValue');
            $('#Sys_FormId').combobox('select', v);
            if (v == GuidEmpty) {
                $('#FormUrl').textbox('enable');
                $('#FormFieldName').combobox('clear').combobox('disable');
            }
        },
        onSelect: function (record) {
            if (record.Id != GuidEmpty) {
                $('#FormUrl').textbox('clear').textbox('disable');
                if ($('#HandlerType').combobox('getValue') == 10) {
                    //加载处理者字段
                    $('#FormFieldName').combobox('enable').combobox({
                        url: '/' + CommonController.Async_Bpm_Controller + '/LoadHandlerFields.html?formId=' + record.Id,
                        valueField: 'Name',
                        textField: 'Display'
                    });
                }
            }
            else {
                $('#FormUrl').textbox('enable');
                $('#FormFieldName').combobox('clear').combobox('disable');
            }
        }
    });
    //处理者字段
    $('#FormFieldName').combobox('disable');
    $('#HandleRange').textbox('disable');
    $('#HandlerType').combobox({
        loadFilter: function (data) {
            if (data != null && data.length > 0) {
                var v = $('#Sys_FormId').combobox('getValue');
                if (v == GuidEmpty) {
                    var tmpDt = [];
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Id != 10)
                            tmpDt.push(data[i]);
                    }
                    return tmpDt;
                }
            }
            return data;
        },
        onLoadSuccess: function () {
            var loadRange = false;
            var v = $('#HandlerType').combobox('getValue');
            if (v == '10') { //处理者类型为选择字段值时
                var v = $('#Sys_FormId').combobox('getValue');
                if (v != GuidEmpty) { //非自定义表单
                    $('#FormFieldName').combobox('enable')
                    $('#HandleRange').textbox('disable');
                    loadRange = false;
                }
                else {
                    $('#FormFieldName').combobox('clear').combobox('disable');
                    $('#HandleRange').textbox('enable');
                    loadRange = true;
                }
            }
            else {
                $('#FormFieldName').combobox('clear').combobox('disable');
                $('#HandleRange').textbox('enable');
                loadRange = true;
            }
            if (loadRange) {
                //加载处理范围名称
                var ids = $('#HandleRange').textbox('getValue'); //处理范围
                if (ids != null && ids.length > 0) {
                    var handleType = $('#HandlerType').combobox('getText');
                    var moduleName = null;
                    switch (handleType) {
                        case '部门':
                            moduleName = "部门管理";
                            break;
                        case '职务':
                            moduleName = '职务管理';
                            break;
                        case '岗位':
                            moduleName = '岗位管理';
                            break;
                        case '人员':
                            moduleName = '员工管理';
                            break;
                        case '角色':
                            moduleName = '角色管理';
                            break;
                    }
                    if (moduleName != null) {
                        $.get('/' + CommonController.Async_Data_Controller + '/LoadModuleDatas.html', { moduleName: moduleName, ids: ids }, function (data) {
                            if (data && data.length > 0) {
                                var strName = '';
                                for (var i = 0; i < data.length; i++) {
                                    strName += data[i].Name + ',';
                                }
                                if (strName.length > 0) {
                                    strName = strName.substr(0, strName.length - 1);
                                    $('#HandleRange').textbox('setText', strName);
                                }
                            }
                        }, 'json');
                    }
                }
            }
        },
        onSelect: function (record) {
            if (record.Id == 0 || record.Id == 6 || record.Id == 7 || record.Id == 8 ||
                record.Id == 9 || record.Id == 11 || record.Id == 12 || record.Id == 13 ||
                record.Id == 14 || record.Id == 15 || record.Id == 16 || record.Id == 17 ||
                record.Id == 18 || record.Id == 19 || record.Id == 20 || record.Id == 21 ||
                record.Id == 22) {
                $('#HandleRange').textbox('disable');
                $('#FormFieldName').combobox('disable');
                return;
            }
            var flag = false; //加载表单字段标识
            var formId = $('#Sys_FormId').combobox('getValue');
            if (formId && record.Id == 10) { //处理者类型为选择字段值时
                var v = $('#Sys_FormId').combobox('getValue');
                if (v != GuidEmpty) { //非自定义表单
                    //加载处理者字段
                    $('#FormFieldName').combobox('enable').combobox({
                        url: '/' + CommonController.Async_Bpm_Controller + '/LoadHandlerFields.html?formId=' + formId,
                        valueField: 'Name',
                        textField: 'Display'
                    });
                    $('#HandleRange').textbox('disable');
                }
                else {
                    $('#FormFieldName').combobox('clear').combobox('disable');
                    $('#HandleRange').textbox('enable');
                }
            }
            else {
                $('#FormFieldName').combobox('clear').combobox('disable');
                $('#HandleRange').textbox('enable');
            }
        }
    });
    //处理范围
    $('#HandleRange').textbox({
        icons: [{
            iconCls: 'eu-icon-search',
            handler: function (e) {
                var handleType = $('#HandlerType').combobox('getText');
                var moduleName = null;
                var otherParams = null;
                switch (handleType) {
                    case '部门':
                        moduleName = "部门管理";
                        break;
                    case '职务':
                        moduleName = '职务管理';
                        otherParams = { fieldName: 'Name', hasId: 1 };
                        break;
                    case '岗位':
                        moduleName = '岗位管理';
                        break;
                    case '人员':
                        moduleName = '员工管理';
                        break;
                    case '角色':
                        moduleName = '角色管理';
                        otherParams = { fieldName: 'Name', hasId: 1 };
                        break;
                }
                if (moduleName != null) {
                    SelectModuleTree(moduleName, null, true, function (rows) {
                        var ids = '';
                        var names = '';
                        for (var i = 0; i < rows.length; i++) {
                            var row = rows[i];
                            ids += row.Id + ',';
                            names += row.Name + ',';
                        }
                        ids = ids.substr(0, ids.length - 1);
                        names = names.substr(0, names.length - 1);
                        $('#HandleRange').attr('v', ids).attr('value', ids);
                        $('#HandleRange').textbox('setValue', ids).textbox('setText', names);
                    }, true, otherParams);
                }
            }
        }]
    });
    var iframe = topWin.GetCurrentDialogFrame()[0];
    var divDom = $(iframe).parent();
    var paramsTemp = divDom.attr('params');
    if (paramsTemp != undefined && paramsTemp) {
        var params = JSON.parse(decodeURI(paramsTemp));
        SetNodeParams(params);
    }
});

//获取节点参数
function GetNodeParams() {
    //组装节点信息
    var name = $('#Name').textbox('getValue'); //节点名称
    var display = $('#DisplayName').textbox('getValue'); //节点显示名称
    var form = $('#Sys_FormId').combobox('getValue'); //节点表单
    var formUrl = $('#FormUrl').textbox('getValue'); //表单URL
    var handleType = $('#HandlerType').combobox('getValue'); //处理类型
    var handleRange = $('#HandleRange').attr('v'); //处理范围
    var handleStratege = $('#HandleStrategy').combobox('getValue'); //处理策略
    var formField = $('#FormFieldName').combobox('getValue'); //表单字段
    var backType = $('#BackType').length > 0 ? $('#BackType').combobox('getValue') : 2; //回退类型
    var subFlowId = $('#Bpm_WorkFlowSubId').combobox('getValue');
    var subFlowType = $('#SubFlowType').combobox('getValue'); //子流程类型
    if (subFlowType == '') subFlowType = 0;
    var autoJumpRule = '';
    for (var i = 1; i <= 5; i++) {
        if ($('#AutoJumpRule' + i).length > 0) {
            var v = $('#AutoJumpRule' + i).val();
            autoJumpRule += v + ',';
        }
    }
    if (autoJumpRule.length > 0)
        autoJumpRule = autoJumpRule.substr(0, autoJumpRule.length - 1);
    var workNode = {
        Name: name, DisplayName: display, FormUrl: formUrl, HandlerType: handleType, HandleRange: handleRange,
        HandleStrategy: handleStratege, FormFieldName: formField, BackType: backType, AutoJumpRule: autoJumpRule,
        Bpm_WorkFlowId: workflowId, TagId: tagId, SubFlowType: subFlowType
    };
    if (form != undefined && form != null && form != GuidEmpty && form.length > 0) {
        workNode.Sys_FormId = form;
    }
    if (subFlowId != undefined && subFlowId != null && subFlowId != GuidEmpty && subFlowId.length > 0) {
        workNode.Bpm_WorkFlowSubId = subFlowId;
    }
    //组装节点按钮
    var btns = [];
    $("input[tag='btn']").each(function (i, item) {
        var btnConfigId = $(this).attr('BtnConfigId');
        var flowBtnId = $(this).attr('FlowBtnId');
        var btnText = $(this).textbox('getValue');
        var isEnabled = $("input[tag='btnEnable'][flowBtnId='" + flowBtnId + "']").val();
        var obj = { Bpm_FlowBtnId: flowBtnId, BtnDisplay: btnText, IsEnabled: isEnabled == '1' };
        if (btnConfigId != undefined && btnConfigId != null && btnConfigId.length > 0) {
            obj.Id = btnConfigId;
        }
        btns.push(obj);
    });
    workNode.BtnConfigs = btns;
    return workNode;
}

//设置节点参数
function SetNodeParams(workNode) {
    $('#Name').textbox('setValue', workNode.Name);
    $('#DisplayName').textbox('setValue', workNode.DisplayName);
    $('#Sys_FormId').combobox('setValue', workNode.Sys_FormId);
    $('#FormUrl').textbox('setValue', workNode.FormUrl);
    $('#HandlerType').combobox('setValue', workNode.HandlerType);
    $('#HandleRange').attr('v', workNode.HandleRange).attr('value', workNode.HandleRange);
    $('#HandleRange').textbox('setValue', workNode.HandleRange).textbox('setText', workNode.HandleRangeName);
    $('#HandleStrategy').combobox('setValue', workNode.HandleStrategy);
    $('#FormFieldName').combobox('setValue', workNode.FormFieldName);
    $('#BackType').combobox('setValue', workNode.BackType);
    if (workNode.Bpm_WorkFlowSubId != undefined && workNode.Bpm_WorkFlowSubId != null && workNode.Bpm_WorkFlowSubId != GuidEmpty && workNode.Bpm_WorkFlowSubId.length > 0) {
        $('#Bpm_WorkFlowSubId').combobox('setValue', workNode.Bpm_WorkFlowSubId);
    }
    $('#SubFlowType').combobox('setValue', workNode.SubFlowType);
    if (workNode.AutoJumpRule) {
        var token = workNode.AutoJumpRule.split(',');
        if (token.length == 5) {
            for (var i = 1; i <= 5; i++) {
                var autoJumpDom = $('#AutoJumpRule' + i);
                var v = token[i - 1];
                autoJumpDom.val(v);
                if (v == "1")
                    autoJumpDom.attr('checked', 'checked');
                else
                    autoJumpDom.removeAttr('checked');
            }
        }
    }
    if (workNode.BtnConfigs != undefined && workNode.BtnConfigs && workNode.BtnConfigs.length > 0) {
        for (var i = 0; i < workNode.BtnConfigs.length; i++) {
            var btnConfig = workNode.BtnConfigs[i];
            var flowBtnId = btnConfig.Bpm_FlowBtnId;
            var btnText = btnConfig.BtnDisplay;
            if (btnText && btnText.length > 0) {
                $("input[tag='btn'][flowBtnId='" + flowBtnId + "']").textbox('setValue', btnText);
            }
            var btnEnableDom = $("input[tag='btnEnable'][flowBtnId='" + flowBtnId + "']");
            var btnEnableDomId = btnEnableDom.attr('id');
            if (btnConfig.IsEnabled) {
                btnEnableDom.val('1').attr('checked', 'checked');
                if (btnEnableDomId == 'BackBtnEnabled') {
                    $('#trBackType').show();
                }
            }
            else {
                btnEnableDom.val('0').removeAttr('checked');
                if (btnEnableDomId == 'BackBtnEnabled') {
                    $('#trBackType').hide();
                }
            }
        }
    }
}

var page = GetLocalQueryString("page");

//主键字段格式化
//value:字段值
//row:行对象
//index:行号 
//moduleName:当前模块信息
//fieldName:当前字段名称
//paramsObj:字段参数对象
//otherParams：模块其他参数
function TitleKeyFormatter(value, row, index, moduleName, fieldName, paramsObj, otherParams) {
    var v = value != undefined && value != null ? value : "";
    if (typeof (OverTitleKeyFormatter) == 'function') {
        v = OverTitleKeyFormatter(value, row, index, moduleName, fieldName, paramsObj, otherParams);
        if (v != undefined && v != null)
            return v;
    }
    if (page == 'fdGrid' || page == 'otGrid' || page == 'edit') return v;
    var recordId = row["Id"];
    var title = v;
    if (v.length > 0 && otherParams && otherParams.length > 0) {
        var obj = eval("(" + decodeURIComponent(otherParams) + ")");
        var clickMethod = "ViewRecord(this)"; //默认点击标识字段值链接查看记录
        var isDraft = GetLocalQueryString("draft"); //是否我的草稿列表页
        if (parseInt(isDraft) == 1) { //我的草稿列表时，点击标识字段链接进入编辑表单
            clickMethod = "Edit(this)";
        }
        v = "<a title='" + title + "' href='#' recycle='" + obj.recycle + "' recordId='" + recordId + "' moduleId='" + obj.moduleId + "' moduleName='" + moduleName + "' moduleDisplay='" + obj.moduleDisplay + "' titleKey='" + obj.titleKey + "' titleKeyDisplay='" + obj.titleKeyDisplay + "' editMode='" + obj.editMode + "' editWidth='" + obj.editWidth + "' editHeight='" + obj.editHeight + "' gridId='" + obj.gridId + "' onclick='" + clickMethod + "'>" + v + "</a>";
    }
    if (page != 'edit' && paramsObj && paramsObj.length > 0) {
        var obj = eval("(" + decodeURIComponent(paramsObj) + ")");
        var editFlag = "<img class='editFlag' gridId='" + obj.gridId + "' moduleId='" + obj.moduleId + "' fieldName='" + obj.fieldName + "' recordId='" + recordId + "' fieldDisplay='" + obj.fieldDisplay + "' fieldWidth='" + obj.fieldWidth + "' oldValue='" + value + "' onclick='EditField(this)' style='margin-left:5px;cursor:pointer;display:none;' src='/Css/icons/docEdit.png' />"
        return "<span title='" + title + "'>" + v + editFlag + "</span>";
    }
    return v;
}

//外键字段格式化
//value:字段值
//row:行对象
//index:行号 
//moduleName:当前模块名称
//fieldName:当前字段名称
//foreignModuleName:外键模块名称
//paramsObj:字段参数对象
//otherForeignParams:外键模块其他参数
//gridId:网格domId
function ForeignKeyFormatter(value, row, index, moduleName, fieldName, foreignModuleName, paramsObj, otherForeignParams, gridId) {
    var v = value != undefined && value != null && value != GuidEmpty ? value : "";
    if (typeof (OverForeignKeyFormatter) == 'function') {
        v = OverForeignKeyFormatter(value, row, index, moduleName, fieldName, foreignModuleName, paramsObj, otherForeignParams);
        if (v != undefined && v != null)
            return v;
    }
    if (page == 'fdGrid' || page == 'otGrid' || page == 'edit') return v;
    var title = v;
    if (v.length == 36 && otherForeignParams && otherForeignParams.length > 0) {
        var obj = eval("(" + decodeURIComponent(otherForeignParams) + ")");
        var foreignRecordId = "";
        var titleKeyValue = "";
        if (fieldName.length > 4 && fieldName.substr(fieldName.length - 4, 4) == "Name") {
            var foreignIdField = fieldName.substr(0, fieldName.length - 4) + "Id";
            foreignRecordId = row[foreignIdField];
            titleKeyValue = row[fieldName];
        }
        else {
            var foreignNameField = fieldName.substr(0, fieldName.length - 2) + "Name";
            foreignRecordId = row[fieldName];
            titleKeyValue = row[foreignNameField];
        }
        v = "<a title='" + title + "' href='#' recordId='" + foreignRecordId + "' titleKeyValue='" + titleKeyValue + "' moduleId='" + obj.moduleId + "' moduleName='" + foreignModuleName + "' moduleDisplay='" + obj.moduleDisplay + "' titleKey='" + obj.titleKey + "' titleKeyDisplay='" + obj.titleKeyDisplay + "' editMode='" + obj.editMode + "' editWidth='" + obj.editWidth + "' editHeight='" + obj.editHeight + "' gridId='" + obj.gridId + "' onclick='ViewRecord(this)'>" + v + "</a>";
    }
    if (page != 'edit' && paramsObj && paramsObj.length > 0) {
        var obj = eval("(" + decodeURIComponent(paramsObj) + ")");
        var editFlag = "<img class='editFlag' gridId='" + obj.gridId + "' moduleId='" + obj.moduleId + "' fieldName='" + obj.fieldName + "' recordId='" + row["Id"] + "' fieldDisplay='" + obj.fieldDisplay + "' fieldWidth='" + obj.fieldWidth + "' oldValue='" + value + "' onclick='EditField(this)' style='margin-left:5px;cursor:pointer;display:none;' src='/Css/icons/docEdit.png' />"
        return "<span title='" + title + "'>" + v + editFlag + "</span>";
    }
    return v;
}

//枚举字段格式化
//value:字段值
//row:行对象
//index:行号 
//moduleName:当前模块名称
//fieldName:当前字段名称
//enumData:枚举数据
//paramsObj:字段参数对象
function EnumFieldFormatter(value, row, index, moduleName, fieldName, enumData, paramsObj) {
    var v = value != undefined && value != null ? value : "";
    if (typeof (OverEnumFieldFormatter) == 'function') {
        v = OverEnumFieldFormatter(value, row, index, moduleName, fieldName, enumData, paramsObj);
        if (v != undefined && v != null)
            return v;
    }
    if ((value || value == 0) && enumData && enumData.length > 0) {
        var dic = eval("(" + decodeURIComponent(enumData) + ")");
        if (dic && dic.length > 0) {
            for (var i = 0; i < dic.length; i++) {
                var tempV = dic[i].Id;
                var n = dic[i].Name;
                if (value == tempV && tempV != "") {
                    v = n;
                    row[fieldName + '_Enum'] = n;
                    break;
                }
            }
        }
    }
    if (page != 'edit' && paramsObj && paramsObj.length > 0) {
        var obj = eval("(" + decodeURIComponent(paramsObj) + ")");
        var editFlag = "<img class='editFlag' gridId='" + obj.gridId + "' moduleId='" + obj.moduleId + "' fieldName='" + obj.fieldName + "' recordId='" + row["Id"] + "' fieldDisplay='" + obj.fieldDisplay + "' fieldWidth='" + obj.fieldWidth + "' oldValue='" + value + "' onclick='EditField(this)' style='margin-left:5px;cursor:pointer;display:none;' src='/Css/icons/docEdit.png' />"
        return "<span title='" + v + "'>" + v + editFlag + "</span>";
    }
    return v;
}

//字典绑定字段格式化
//value:字段值
//row:行对象
//index:行号 
//moduleName:当前模块名称
//fieldName:当前字段名称
//dicData:字典数据
//paramsObj:字段参数对象
function DicFieldFormatter(value, row, index, moduleName, fieldName, dicData, paramsObj) {
    var v = value != undefined && value != null ? value : "";
    if (typeof (OverDicFieldFormatter) == 'function') {
        v = OverDicFieldFormatter(value, row, index, moduleName, fieldName, dicData, paramsObj);
        if (v != undefined && v != null)
            return v;
    }
    if ((value || value == 0) && dicData && dicData.length > 0) {
        var dic = eval("(" + decodeURIComponent(dicData) + ")");
        if (dic && dic.length > 0) {
            for (var i = 0; i < dic.length; i++) {
                var tempV = dic[i].Id;
                var n = dic[i].Name;
                if (value == tempV && tempV != "") {
                    v = n;
                    row[fieldName + '_Dic'] = n;
                    break;
                }
            }
        }
    }
    if (page != 'edit' && paramsObj && paramsObj.length > 0) {
        var obj = eval("(" + decodeURIComponent(paramsObj) + ")");
        var editFlag = "<img class='editFlag' gridId='" + obj.gridId + "' moduleId='" + obj.moduleId + "' fieldName='" + obj.fieldName + "' recordId='" + row["Id"] + "' fieldDisplay='" + obj.fieldDisplay + "' fieldWidth='" + obj.fieldWidth + "' oldValue='" + value + "' onclick='EditField(this)' style='margin-left:5px;cursor:pointer;display:none;' src='/Css/icons/docEdit.png' />"
        return "<span title='" + v + "'>" + v + editFlag + "</span>";
    }
    return v;
}

//多选checkbox格式化
//value:字段值
//row:行对象
//index:行号 
//moduleName:当前模块名称
//fieldName:当前字段名称
//defaultTexts:checkbox的text显示名称
//paramsObj:字段参数对象
function MutiCheckBoxFormatter(value, row, index, moduleName, fieldName, defaultTexts, paramsObj) {
    var v = value != undefined && value != null ? value : "";
    if (typeof (OverMutiCheckBoxFormatter) == 'function') {
        v = OverMutiCheckBoxFormatter(value, row, index, moduleName, fieldName, defaultTexts, paramsObj);
        if (v != undefined && v != null)
            return v;
    }
    if (v.length > 0 && defaultTexts != undefined && defaultTexts != null && defaultTexts.length > 0) {
        var token1 = v.split(',');
        var token2 = defaultTexts.split(',');
        if (token1.length == token2.length) {
            var str = '';
            for (var i = 0; i < token1.length; i++) {
                if (token1[i] != "1") continue;
                str += token2[i] + ',';
            }
            if (str.length > 0) {
                str = str.substr(0, str.length - 1);
                return "<span title='" + str + "'>" + str + "</span>";
            }
        }
    }
    return v;
}

//日期字段格式化
//value:字段值
//row:行对象
//index:行号 
//moduleName:当前模块名称
//fieldName:当前字段名称
//format:格式化字符串
//paramsObj:字段参数对象
function DateFormatter(value, row, index, moduleName, fieldName, format, paramsObj) {
    var v = value != undefined && value != null ? value : "";
    if (typeof (OverDateFormatter) == 'function') {
        v = OverDateFormatter(value, row, index, moduleName, fieldName, format, paramsObj);
        if (v != undefined && v != null)
            return v;
    }
    if (typeof (v) == "string") {
        if (format == 'yyyy-MM-dd') {
            var token = v.split(' ');
            if (token.length > 0)
                v = token[0];
        }
    }
    else if (typeof (v) == "object") {
        v = v.Format(format);
    }
    if (page != 'edit' && paramsObj && paramsObj.length > 0) {
        var obj = eval("(" + decodeURIComponent(paramsObj) + ")");
        var editFlag = "<img class='editFlag' gridId='" + obj.gridId + "' moduleId='" + obj.moduleId + "' fieldName='" + obj.fieldName + "' recordId='" + row["Id"] + "' fieldDisplay='" + obj.fieldDisplay + "' fieldWidth='" + obj.fieldWidth + "' oldValue='" + value + "' onclick='EditField(this)' style='margin-left:5px;cursor:pointer;display:none;' src='/Css/icons/docEdit.png' />"
        return "<span title='" + v + "'>" + v + editFlag + "</span>";
    }
    return v;
}

//布尔型字段格式化
//value:字段值
//row:行对象
//index:行号 
//moduleName:当前模块名称
//fieldName:当前字段名称
//paramsObj:字段参数对象
function BoolFormatter(value, row, index, moduleName, fieldName, paramsObj) {
    var v = (value == true || value == "true" || value == "True") ? "是" : "否";;
    if (typeof (OverBoolFormatter) == 'function') {
        v = OverBoolFormatter(value, row, index, moduleName, fieldName, paramsObj);
        if (v != undefined && v != null)
            return v;
    }
    if (page != 'edit' && paramsObj && paramsObj.length > 0) {
        var obj = eval("(" + decodeURIComponent(paramsObj) + ")");
        var editFlag = "<img class='editFlag' gridId='" + obj.gridId + "' moduleId='" + obj.moduleId + "' fieldName='" + obj.fieldName + "' recordId='" + row["Id"] + "' fieldDisplay='" + obj.fieldDisplay + "' fieldWidth='" + obj.fieldWidth + "' oldValue='" + value + "' onclick='EditField(this)' style='margin-left:5px;margin-top:5px;cursor:pointer;display:none;' src='/Css/icons/docEdit.png' />"
        return "<span>" + v + editFlag + "</span>";
    }
    return v;
}

//通用字段格式化
//value:字段值
//row:行对象
//index:行号 
//moduleName:当前模块名称
//fieldName:当前字段名称
//paramsObj:字段参数对象
//fieldType:字段类型
function GeneralFormatter(value, row, index, moduleName, fieldName, paramsObj, fieldType) {
    var tempV = fieldType == "System.Int32" || fieldType == "System.Int64" ||
                fieldType == "System.Nullable`1[System.Int32]" || fieldType == "System.Nullable`1[System.Int64]" ||
                fieldType == "System.Decimal" || fieldType == "System.Nullable`1[System.Decimal]" ||
                fieldType == "System.Double" || fieldType == "System.Nullable`1[System.Double]" ?
                NumThousandsFormat(value) : value;
    var v = tempV != undefined && tempV != null ? "<span title='" + tempV + "'>" + tempV + "</span>" : "";
    if (typeof (OverGeneralFormatter) == 'function') {
        v = OverGeneralFormatter(value, row, index, moduleName, fieldName, paramsObj, fieldType);
        if (v != undefined && v != null)
            return v;
    }
    if (page != 'edit' && paramsObj && paramsObj.length > 0) {
        var obj = eval("(" + decodeURIComponent(paramsObj) + ")");
        var editFlag = "<img class='editFlag' gridId='" + obj.gridId + "' moduleId='" + obj.moduleId + "' fieldName='" + obj.fieldName + "' recordId='" + row["Id"] + "' fieldDisplay='" + obj.fieldDisplay + "' fieldWidth='" + obj.fieldWidth + "' oldValue='" + value + "' onclick='EditField(this)' style='margin-left:5px;cursor:pointer;display:none;' src='/Css/icons/docEdit.png' />"
        return v + editFlag;
    }
    return v;
}

//行操作按钮格式化
//value:字段值
//row:行对象
//index:行号 
//moduleId:当前模块ID
//moduleName:当前模块名称
function RowOperateBtnFormat(value, row, index, moduleId, moduleName) {
    var btnDom = $("#txtRowOperateBtn_" + moduleId);
    var btnJson = btnDom.val();
    if (btnJson && btnJson.length > 0) {
        btnJson = decodeURIComponent(btnJson);
        var btns = eval("(" + btnJson + ")");
        if (btns && btns.length > 0) {
            var titleKey = btnDom.attr("titleKey");
            var titleKeyDisplay = btnDom.attr("titleKeyDisplay");
            var editMode = parseInt(btnDom.attr("editMode"));
            var editWidth = btnDom.attr("editWidth");
            var editHeight = btnDom.attr("editHeight");
            var recordId = '';
            if (row["Id"]) {
                recordId = row["Id"];
            }
            var tag = moduleId + "_" + recordId;
            var a = "<div id=\"rowOperateDiv_" + tag + "\" style=\"display:" + (editMode == 3 && recordId == '' ? "none" : "block") + ";\">";
            var a_attr = "recordId=\"" + recordId + "\" moduleId=\"" + moduleId + "\" moduleName=\"" + moduleName + "\" titleKey=\"" + titleKey + "\" titleKeyDisplay=\"" + titleKeyDisplay + "\" editMode=\"" + editMode + "\" editWidth=\"" + editWidth + "\" editHeight=\"" + editHeight + "\"";
            var loadOkCancel = false;
            $(btns).each(function (i, btn) {
                if (btn.ClickMethod == null) btn.ClickMethod = '';
                if (btn.ButtonIcon == null) btn.ButtonIcon = '';
                if (btn.ButtonText == null) btn.ButtonText = '';
                if (btn.ButtonText == '新增' || btn.ButtonText == '编辑')
                    loadOkCancel = true;
                a += "<a style=\"height:24px\" rowOperateBtn=\"1\" id=\"" + btn.ButtonTagId + "_" + tag + "\" clickMethod=\"" + btn.ClickMethod + "\" icon=\"" + btn.ButtonIcon + "\" btnText=\"" + btn.ButtonText + "\" " + a_attr + "></a>";
            });
            a += "</div>";
            if (editMode == 3 && loadOkCancel) {
                a += "<div id=\"rowOkDiv_" + tag + "\" style=\"display:" + (recordId == '' ? "block" : "none") + ";\">";
                a += "<a style=\"height:24px\" id=\"rowOkBtn_" + tag + "\" " + a_attr + "></a>";
                a += "<a style=\"height:24px\" id=\"rowCancelBtn_" + tag + "\" " + a_attr + "></a>";
                a += "</div>";
            }
            return a;
        }
    }
    return value;
}

//流程图标列格式化
//value:字段值
//row:行对象
//index:行号 
//moduleId:当前模块ID
//moduleName:当前模块名称
function FlowIconFormatter(value, row, index, moduleId, moduleName) {
    if (typeof (OverFlowIconFormatter) == 'function') {
        var v = OverFlowIconFormatter(value, row, index, moduleId, moduleName);
        if (v != undefined && v != null)
            return v;
    }
    var icon = "/Css/icons/";
    if (isNfm) {
        icon = host + icon;
    }
    var flowStatus = row["FlowStatus"];
    if (!flowStatus) flowStatus = 0;
    switch (flowStatus) {
        case 0: //流程待提交
            icon += "tosubmit.png";
            break;
        case 1: //流程待审批
            icon += "toapproval.png";
            break;
        case 2: //流程审批中
            icon += "inapproval.png";
            break;
        case 3: //流程被回退
            icon += "toreturn.png";
            break;
        case 4: //流程被拒绝
            icon += "reject.png";
            break;
        case 5: //流程冻结中
            icon = "";
            break;
        case 10: //审批通过
            icon += "approvalok.png";
            break;
    }
    var recordId = row["Id"];
    if (row["TempDetailId"])
        recordId = row["TempDetailId"];
    return "<img class='flowImg' id='flowImg_" + recordId + "' flowStatus='" + flowStatus + "' recordId='" + recordId + "' moduleId='" + moduleId + "' src='" + icon + "' />";
}

//流程状态列格式化
//value:字段值
//row:行对象
//index:行号 
//moduleId:当前模块ID
//moduleName:当前模块名称
function FlowStatusFormatter(value, row, index, moduleId, moduleName) {
    if (typeof (OverFlowStatusFormatter) == 'function') {
        var v = OverFlowStatusFormatter(value, row, index, moduleId, moduleName);
        if (v != undefined && v != null)
            return v;
    }
    var flowStatus = row["FlowStatus"];
    if (!flowStatus) flowStatus = 0;
    var statusDes = "未提交";
    switch (flowStatus) {
        case 0: //未提交
            v = "<span>" + statusDes + "</span>";
            break;
        case 1: //已发起
            statusDes = "已发起";
            v = "<span style='color:#999933'>" + statusDes + "</span>";
            break;
        case 2: //审批中
            statusDes = "审批中";
            v = "<span style='color:#A52A2A'>" + statusDes + "</span>";
            break;
        case 3: //被退回
            statusDes = "被退回";
            v = "<span style='color:#808000'>" + statusDes + "</span>";
            break;
        case 4: //被拒绝
            statusDes = "被拒绝";
            v = "<span style='color:#FF0000'>" + statusDes + "</span>";
            break;
        case 5: //被冻结
            statusDes = "被冻结";
            v = "<span style='color:#808080'>" + statusDes + "</span>";
            break;
        case 10: //已通过
            statusDes = "已通过";
            v = "<span style='color:#008000'>" + statusDes + "</span>";
            break;
    }
    return v;
}
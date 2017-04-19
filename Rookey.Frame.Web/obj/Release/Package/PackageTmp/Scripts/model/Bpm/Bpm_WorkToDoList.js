
//重写主键格式化
function OverTitleKeyFormatter(value, row, index, moduleName, fieldName, paramsObj, otherParams) {
    var v = value != undefined && value != null ? value : "";
    if (fieldName == "Code" || fieldName == "Title") { //标题和Code格式化
        return "<a href='javascript:void(0);' title='" + v + "' node='" + row.Bpm_WorkNodeName + "' todoId='" + row.Id + "' moduleId='" + row.ModuleId + "' moduleName='" + row.ModuleName + "' recordId='" + row.RecordId + "' t_title='" + encodeURI(row.Title) + "' formUrl='" + row.FormUrl + "' onclick='ViewRecord(this)'>" + value + "</a>";
    }
    return v;
}

//重写通用格式化
function OverGeneralFormatter(value, row, index, moduleName, fieldName, paramsObj) {
    var v = value != undefined && value != null ? value : "";
    if (fieldName == "Code" || fieldName == "Title") {
        return OverTitleKeyFormatter(value, row, index, moduleName, fieldName, paramsObj);
    }
    return v;
}

//重写枚举字段格式化
function OverEnumFieldFormatter(value, row, index, moduleName, fieldName, enumData, paramsObj) {
    var v = value != undefined && value != null ? value : "";
    if (v && enumData && enumData.length > 0) {
        var dic = eval("(" + decodeURIComponent(enumData) + ")");
        if (dic && dic.length > 0) {
            for (var i = 0; i < dic.length; i++) {
                var tempV = dic[i].Id;
                var n = dic[i].Name;
                if (value == tempV && tempV != "") {
                    v = n;
                    break;
                }
            }
        }
    }
    if (fieldName == "Status") {
        switch (row.Status) {
            case 1: //已发起
                return "<font color='#999933'>" + v + "</font>";
            case 2: //审批中
                return "<font color='#A52A2A'>" + v + "</font>";
            case 3: //被退回
                return "<font color='#808000'>" + v + "</font>";
            case 4: //被拒绝
                return "<font color='#FF0000'>" + v + "</font>";
            case 5: //被冻结
                return "<font color='#808080'>" + v + "</font>";
            case 10: //已通过
                return "<font color='#008000'>" + v + "</font>";
        }
        return "<font>" + v + "</font>";
    }
    return v;
}

//重写外键格式化
function OverForeignKeyFormatter(value, row, index, moduleName, fieldName, foreignModuleName, paramsObj, otherForeignParams) {
    var v = value != undefined && value != null ? value : "";
    if (fieldName == "OrgM_EmpName") {
        return "<span>" + v + "</span>";
    }
    return v;
}

//查看记录
function ViewRecord(obj) {
    var todoId = $(obj).attr('todoId');
    if (todoId) {
        var moduleId = $(obj).attr('moduleId');
        var moduleName = $(obj).attr('moduleName');
        var recordId = $(obj).attr('recordId');
        var title = $(obj).attr('t_title');
        var node = $(obj).attr('node');
        var tp = GetLocalQueryString('p_tp');
        var formUrl = $(obj).attr('formUrl');
        var sepFlag = formUrl && formUrl.length > 0 && formUrl.indexOf('?') > -1 ? '&' : '?';
        var url = formUrl && formUrl.length > 0 ? formUrl + sepFlag + "moduleId=" + moduleId + "&id=" + recordId + "&todoId=" + todoId : "/Page/EditForm.html?page=edit&mode=1&moduleId=" + moduleId + "&id=" + recordId + "&todoId=" + todoId;
        if (tp != 0)
            url = "/Page/ViewForm.html?page=view&mode=1&moduleId=" + moduleId + "&id=" + recordId;
        if (tp == 0) {
            var currTabIndex = GetSelectTabIndex(); //当前grid网格页面的tabindex
            if (currTabIndex)
                url += "&tb=" + currTabIndex;
        }
        url += "&moduleName=" + moduleName;
        url += "&node=" + node;
        url += "&r=" + Math.random();
        AddTab(null, decodeURI(title), url);
    }
    else {
        var rows = GetFinalSelectRows(obj);
        if (rows) {
            var row = rows[0];
            var moduleId = row['ModuleId'];
            var recordId = row['RecordId'];
            var title = row['Title'];
            var node = row['Bpm_WorkNodeName'];
            var url = "/Page/ViewForm.html?page=view&mode=1&moduleId=" + moduleId + "&id=" + recordId + "&node=" + node;
            AddTab(null, decodeURI(title), url);
        }
    }
}

//桌面网格加载成功后事件
//data:数据
//gridId:桌面网格domId
function OverOnDeskGridLoadSuccess(data, gridId) {
    var gridObj = $('#' + gridId);
    if (data.total <= 0) {
        //无记录时显示处理
        $('body').html('<div style="width:100%;text-align:center;height:50px;line-height:50px;color:#BEBEBE">没有相关记录</div>');
    }
    var tp = GetLocalQueryString("p_tp");
    if (tp == '0') {
        try{
            //显示待办数量
            var panelTitleDom = $("div[id^='deskPanel_'][flag='taskTodo']", window.parent.document).prev().find('div.panel-title');
            var titleText = panelTitleDom.attr('titleText');
            var text = titleText ? titleText : panelTitleDom.text();
            if (!titleText)
                panelTitleDom.attr('titleText', text);
            if (data.total > 0) {
                text = text + "（<font color='red'>" + data.total + "</font>）";
            }
            panelTitleDom.html(text);
        } catch (err) { }
    }
}

$(function () {
    //保存
    $('#btnSave').bind('click', function () {
        SaveJob();
    });

    //取消退出
    $('#btnCancel').bind('click', function () {
        var sel = top.$("#tabs").tabs("getSelected");
        var index = top.$("#tabs").tabs("getTabIndex", sel);
        top.$("#tabs").tabs("close", index);
    });

    InitJobPlanGrid();
});

//初始化任务计划列表
function InitJobPlanGrid() {
    columns = [[{ field: 'grid_checkbox', checkbox: true },
                    { title: '计划名称', field: 'planName', align: 'left', width: 150 },
                    { title: '计划组', field: 'planGroupName', align: 'left', width: 150 },
                    { title: '计划类型', field: 'planType', width: 100 },
                    { title: '执行状态', field: 'planStatus', width: 100 },
                    { title: '开始时间', field: 'planStartTime', width: 150 },
                    { title: '结束时间', field: 'planEndTime', width: 150 },
                    { title: '表达式', field: 'planCron', width: 120 },
                    {
                        title: '计划说明', field: 'planDes', align: 'left', width: 300,
                        formatter: function (value, row, index) {
                            {
                                var planDes = value;
                                if (planDes.length > 30) planDes = planDes.substr(0, 30) + "...";
                                return "<div title='" + value + "'>" + planDes + "</div>";
                            }
                        }
                    }]];
    var toolbar = [{
        plain: true,
        text: '新增',
        iconCls: 'eu-icon-add',
        handler: function () {
            if (typeof (AddJobPlan) == 'function') {
                AddJobPlan();
            }
        }
    }, '-', {
        plain: true,
        text: '删除',
        iconCls: 'eu-icon-cancel',
        handler: function () {
            if (typeof (DelJobPlan) == 'function') {
                DelJobPlan();
            }
        }
    }];
    $('#tb_JobPlan').datagrid({
        title: "任务计划",
        url: '',
        queryParams: {},
        loadMsg: "正在加载数据，请稍候...",
        striped: true,
        fitColumns: false,
        pagination: false,
        nowrap: true,
        singleSelect: false,
        rownumbers: true,
        toolbar: toolbar,
        selectOnCheck: true,//选择时选中checkbox
        checkOnSelect: true,//选中时选择checkbox
        columns: columns
    });
}

//新增任务计划
function AddJobPlan() {
    var getUrl = '/Quartz/AddJobPlan.html';
    topWin.OpenOkCancelDialog("新增任务计划", getUrl, 600, 500, function (iframe, backFun) {
        var editwindow = iframe;
        var planName = editwindow.contentWindow.document.getElementById('PlanName').value;
        var planGroupName = editwindow.contentWindow.document.getElementById('PlanGroupH').value;
        var planType = editwindow.contentWindow.document.getElementById('IsRepeatH').value == "1" ? "重复执行" : "执行一次";
        var planStatus = editwindow.contentWindow.document.getElementById('PlanStatus').value == "1" ? "等待执行" : "立即执行";
        var planStartTime = editwindow.contentWindow.document.getElementById("PlanStartTimeH").value;
        var planEndTime = editwindow.contentWindow.document.getElementById("PlanEndTimeH").value;
        var planDes = editwindow.contentWindow.document.getElementById('PlanDes').value;
        var planCron = editwindow.contentWindow.document.getElementById('CronExp').value;
        if (planCron != '') {
            $('#tb_JobPlan').datagrid('appendRow', {
                planName: planName,
                planGroupName: planGroupName,
                planType: planType,
                planStatus: planStatus,
                planStartTime: planStartTime,
                planEndTime: planEndTime,
                planDes: planDes,
                planCron: planCron
            });
            if (typeof (backFun) == "function")
                backFun(true);
        }
        else {
            topWin.ShowMsg("保存提示", "计划没有生成！");
        }
    });
}

//删除任务计划
function DelJobPlan() {
    var rows = $("#tb_JobPlan").datagrid("getSelections");
    if (rows == null) {
        topWin.ShowMsg("操作提示", "请选择一条记录！");
        return;
    }
    if (rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            var index = $("#tb_JobPlan").datagrid("getRowIndex", rows[i]);
            $("#tb_JobPlan").datagrid("deleteRow", index);
        }
    }
}

//保存任务
function SaveJob() {
    //任务数据序列化
    var jobInfo = "";
    var jobGroupName = $("#JobGroup").combobox("getValue");
    var jobName = $("#JobName").val();
    var jobType = $("#JobType").combobox("getValue");
    var jobDes = $("#JobDes").val();
    var rows = $("#tb_JobPlan").datagrid("getRows");
    //数据验证
    if (jobName == '') {
        topWin.ShowMsg("保存提示", "任务名称不能为空！");
        return;
    }
    if (jobGroupName == '') {
        topWin.ShowMsg("保存提示", "任务组名称不能为空！");
        return;
    }
    if (jobType == '') {
        topWin.ShowMsg("保存提示", "任务类型不能为空！");
        return;
    }
    //计划验证
    if (rows == null || rows.length == 0) {
        topWin.ShowMsg("保存提示", "请设置任务计划！");
        return;
    }
    //数据组装
    jobInfo = "{\"JobName\":\"" + jobName + "\",\"JobGroupName\":\"" + jobGroupName + "\",\"JobType\":\"" + jobType + "\",\"JobDes\":\"" + jobDes + "\",";
    var triggerName = "\"TriggerName\":[";
    var triggerGroupName = "\"TriggerGroupName\":[";
    var triggerCron = "\"TriggerCron\":[";
    var triggerDes = "\"TriggerDes\":[";
    var startTime = "\"StartTime\":[";
    var endTime = "\"EndTime\":[";
    var isStartNow = "\"IsStartNow\":[";
    for (var i = 0; i < rows.length; i++) {
        triggerName += "\"" + rows[i].planName + "\",";
        triggerGroupName += "\"" + rows[i].planGroupName + "\",";
        triggerCron += "\"" + rows[i].planCron + "\",";
        triggerDes += "\"" + rows[i].planDes + "\",";
        startTime += "\"" + rows[i].planStartTime + "\",";
        endTime += "\"" + rows[i].planEndTime + "\",";
        isStartNow += rows[i].planStatus == "立即执行" ? "true" : "false" + ",";
    }
    triggerName = triggerName.substr(0, triggerName.length - 1);
    triggerName += "]";
    triggerGroupName = triggerGroupName.substr(0, triggerGroupName.length - 1);
    triggerGroupName += "]";
    triggerCron = triggerCron.substr(0, triggerCron.length - 1);
    triggerCron += "]";
    triggerDes = triggerDes.substr(0, triggerDes.length - 1);
    triggerDes += "]";
    startTime = startTime.substr(0, startTime.length - 1);
    startTime += "]";
    endTime = endTime.substr(0, endTime.length - 1);
    endTime += "]";
    isStartNow = isStartNow.substr(0, isStartNow.length - 1);
    isStartNow += "]";
    jobInfo += triggerName + "," + triggerGroupName + "," + triggerCron + "," + triggerDes + "," + startTime + "," + endTime + "," + isStartNow;
    jobInfo += "}";
    //保存数据
    var url = "/Quartz/SaveJob.html";
    var param = [{ name: "jobInfo", value: jobInfo },
                 { name: "r", value: Math.random() }];
    ExecuteCommonAjax(url, param, function (result) {
        if (result && result.Success) {
            //刷新网格
            var tb = GetLocalQueryString("tb"); //网格对应的tabindex
            if (tb && parseInt(tb) > 0) {
                var tempIframe = top.getTabFrame(parseInt(tb));
                if (tempIframe.length > 0) {
                    tempIframe[0].contentWindow.RefreshGrid();
                }
            }
            $('#btnCancel').click();
        }
    }, true);
}

var toolbar = [{
    id: "btnSetup", text: '安装服务', iconCls: 'eu-icon-add', handler: function () {
        ServiceOperate("SetupQuartzService");
    }
}, '-', {
    id: "btnUnSetup", text: '反安装服务', iconCls: 'eu-icon-cancel', handler: function () {
        ServiceOperate("UnSetupQuartzService");
    }
}, '-', {
    id: "btnStartService", text: '启动服务', iconCls: 'eu-icon-ok', handler: function () {
        ServiceOperate("StartQuartzService");
    }
}, '-', {
    id: "btnStopService", text: '停止服务', iconCls: 'eu-icon-no', handler: function () {
        ServiceOperate("StopQuartzService");
    }
}, '-', {
    id: "btnRestartService", text: '重启服务', iconCls: 'eu-icon-redo', handler: function () {
        ServiceOperate("RestartQuartzService");
    }
}];

$(document).ready(function () {
    OperationButtonSet("Init");
    var url = "/Quartz/GetQuartzServiceInfo.html?r=" + Math.random();
    //调度服务的
    try {
        ExecuteCommonAjax(url, null, function (obj) {
            if (obj == null) return;
            $("#serviceStatus").text(obj.Message);
            $("#remoteHostName").text(obj.RemoteHostName);
            $("#localHostName").text(obj.LocalHostName);
            $("#localIp").text(obj.LocalIp);
            $("#operateInfo").text("服务信息加载成功!");
            $("#exceptionMsg").text(obj.ErrMsg);
            if (obj.Message == "服务已启动!") {
                OperationButtonSet("StartQuartzService")
            }
            if (obj.Message == "服务已停止!") {
                OperationButtonSet("StopQuartzService")
            }
            if (obj.Message == "服务未安装!") {
                OperationButtonSet("UnSetupQuartzService")
            }
            if (obj.Message == "服务已安装!") {
                OperationButtonSet("SetupQuartzService")
            }
        }, false);
    } catch (e) { }
    setInterval(function () {
        RefreshSchedulerInfo();
    }, 1000);
});

//刷新作业
function RefreshSchedulerInfo() {
    //作业的
    try {
        var url = "/Quartz/GetSchedulerInfo.html?r=" + Math.random();
        ExecuteCommonAjax(url, null, function (obj) {
            var schedulerName = obj != null ? obj.SchedulerName : '';
            var schedulerInstanceID = obj != null ? obj.SchedulerInstanceID : '';
            var schedulerIsRemote = obj != null ? (obj.SchedulerIsRemote == 'true' ? "是" : "否") : '';
            var schedulerType = obj != null ? obj.SchedulerType : '';
            var schedulerStartTime = obj != null ? obj.SchedulerStartTime : '';
            var schedulerRunningStatus = obj != null ? obj.SchedulerRunningStatus : '';
            var jobTotal = obj != null ? obj.JobTotal : '';
            var jobExecuteTotal = obj != null ? obj.JobExecuteTotal : '';
            $("#SchedulerName").text(schedulerName);
            $("#SchedulerInstanceId").text(schedulerInstanceID);
            $("#IsRemote").text(schedulerIsRemote);
            $("#SchedulerType").text(schedulerType);
            $("#StartRunningTime").text(schedulerStartTime);
            $("#SchedulerStatus").text(schedulerRunningStatus);
            $("#TotalTaskNum").text(jobTotal);
            $("#ExcutedTaskNum").text(jobExecuteTotal);
        }, false);
    } catch (e) { }
}

//服务操作
function ServiceOperate(op) {
    $("#exceptionMsg").text("");
    $("#operateInfo").text("");
    url = "/Quartz/ServiceOperate.html";
    param = [
        { name: "op", value: op },
        { name: "r", value: Math.random() }];
    ExecuteCommonAjax(url, param, function (result) {
        if (result.Success) { //成功
            var msg = OperationButtonSet(op);
            $("#serviceStatus").text(msg);
            $("#operateInfo").text("操作成功!");
        }
        else {
            $("#operateInfo").text("操作失败");
        }
        $("#exceptionMsg").text(result.Message);
    }, false);
}

//操作按钮状态设置
function OperationButtonSet(op) {
    var msg = "";
    if (op == "SetupQuartzService") {
        msg = "服务已安装!";
        $('#btnSetup').linkbutton('disable');
        $("#btnStopService").linkbutton('disable'); //停止不可用
        $("#btnRestartService").linkbutton('disable'); //重启不可用
        $("#btnUnSetup").linkbutton('enable'); //反安装可用
        $("#btnStartService").linkbutton('enable'); //启动可用
    }
    if (op == "UnSetupQuartzService") {
        msg = "服务未安装!";
        $("#btnUnSetup").linkbutton('disable'); //反安装不可用
        $("#btnStartService").linkbutton('disable'); //启动不可用
        $("#btnStopService").linkbutton('disable'); //停止不可用
        $("#btnRestartService").linkbutton('disable'); //重启不可用
        $("#btnSetup").linkbutton('enable'); //安装可用
    }
    if (op == "StartQuartzService" || op == "RestartQuartzService") {
        msg = "服务已启动!";
        $("#btnSetup").linkbutton('disable'); //安装不可用
        $("#btnUnSetup").linkbutton('disable'); //反安装不可用
        $("#btnStartService").linkbutton('disable'); //启动不可用
        $("#btnStopService").linkbutton('enable'); //停止可用
        $("#btnRestartService").linkbutton('enable'); //重启可用
    }
    if (op == "StopQuartzService") {
        msg = "服务已停止!";
        $("#btnSetup").linkbutton('disable'); //安装不可用
        $("#btnUnSetup").linkbutton('enable'); //反安装可用
        $("#btnStopService").linkbutton('disable'); //停止不可用
        $("#btnRestartService").linkbutton('disable'); //重启不可用 
        $("#btnStartService").linkbutton('enable'); //启动可用 
    }
    if (op == "Init") {
        $("#btnSetup").linkbutton('disable'); //安装不可用
        $("#btnUnSetup").linkbutton('disable'); //反安装不可用
        $("#btnStopService").linkbutton('disable'); //停止不可用
        $("#btnRestartService").linkbutton('disable'); //重启不可用 
        $("#btnStartService").linkbutton('disable'); //启动不可用 
    }
    return msg;
}
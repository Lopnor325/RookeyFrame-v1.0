
$(document).ready(function () {
    var instance = null;
    var modelStr = $("#model").val();
    if (modelStr != undefined && modelStr != null && modelStr != "") {
        instance = eval("(" + modelStr + ")");
    }
    //初始化
    $('#IsRepeat').combobox({
        onSelect: function (record) {
            fnIsRepeat();
        }
    });
    $('#JobRepeatIntervalType').combobox({
        onSelect: function (record) {
            fnJobRepeatIntervalType();
        }
    });
    $('#IntervalMonthType').combobox({
        onSelect: function (record) {
            fnIntervalMonthType();
        }
    });
    $('#EverydayIntervalType').combobox({
        onSelect: function (record) {
            fnEverydayIntervalType();
        }
    });
    //加载是否重复执行
    fnIsRepeat();
    fnEverydayIntervalType();
});

//是否重复执行任务
var fnIsRepeat = function () {
    var IsRepeat = $("#IsRepeat").combobox("getValue");
    if (IsRepeat == "0") {
        $("#trJobRepeatIntervalType").hide();
        $("#trJobRepeatInterval").hide();
        $("#trIntervalWeek").hide();
        $("#trIntervalMonthType").hide();
        $("#trIntervalMonthDay").hide();
        $("#trIntervalMonthWeek").hide();
        $("#trEverydayIntervalType").hide();
        $("#trEverydayIntervalFrmTime").hide();
        $("#trEverydayIntervalEndTime").hide();
        $("#trEverydayInterval").hide();
        $("#trJobRepeatInterval").hide();
    }
    else {
        $("#trJobRepeatIntervalType").show();
        $("#trJobRepeatInterval").show();
        $("#trIntervalWeek").show();
        $("#trIntervalMonthType").show();
        $("#trIntervalMonthDay").show();
        $("#trIntervalMonthWeek").show();
        $("#trEverydayIntervalType").show();
        $("#trEverydayIntervalFrmTime").show();
        $("#trEverydayIntervalEndTime").show();
        $("#trEverydayInterval").show();
        $("#trJobRepeatInterval").show();
        fnJobRepeatIntervalType();
    }
}

//运行间隔类型
var fnJobRepeatIntervalType = function () {
    var JobRepeatIntervalType = $("#JobRepeatIntervalType").combobox("getValue");
    if (JobRepeatIntervalType == "EveryDay") {
        $("#trIntervalWeek").hide();
        $("#trIntervalMonthType").hide();
        $("#trIntervalMonthDay").hide();
        $("#trIntervalMonthWeek").hide();
        $("#spanRepeatInterval").text("天")
    }
    else if (JobRepeatIntervalType == "EveryWeek") {
        $("#trIntervalWeek").show();
        $("#trIntervalMonthType").hide();
        $("#trIntervalMonthDay").hide();
        $("#trIntervalMonthWeek").hide();
        $("#spanRepeatInterval").text("周")
    }
    else if (JobRepeatIntervalType == "EveryMonth") {
        $("#trIntervalWeek").hide();
        $("#trIntervalMonthType").show();
        $("#trIntervalMonthDay").show();
        $("#trIntervalMonthWeek").hide();
        $("#spanRepeatInterval").text("月")
        fnIntervalMonthType();
    } else {
        $("#trIntervalWeek").show();
        $("#trIntervalMonthType").show();
        $("#trIntervalMonthDay").show();
        $("#trIntervalMonthWeek").show();
    }
}

//每月执行频率类型
var fnIntervalMonthType = function () {
    var IntervalMonthType = $("#IntervalMonthType").combobox("getValue");
    if (IntervalMonthType == "1") {
        $("#trIntervalMonthDay").show();
        $("#trIntervalMonthWeek").hide();
    }
    else {
        $("#trIntervalMonthDay").hide();
        $("#trIntervalMonthWeek").show();
    }
}

//每天重复执行
var fnEverydayIntervalType = function () {
    var EverydayIntervalType = $("#EverydayIntervalType").combobox("getValue");
    if (EverydayIntervalType == "1") {
        $("#trEverydayInterval").show();
        $("#spanEverydayIntervalLabel").text("时间:");
        $("#spanEverydayIntervalSplit").show();
        $("#tdEverydayIntervalSplit").show();
        $("#tdEverydayIntervalEndTime").show();
    }
    else {
        $("#trEverydayInterval").hide();
        $("#spanEverydayIntervalLabel").text("执行时间:");
        $("#spanEverydayIntervalSplit").hide();
        $("#tdEverydayIntervalSplit").hide();
        $("#tdEverydayIntervalEndTime").hide();
    }
}

//设置周频率
var SetIntervalWeek = function () {
    var str = "";
    $("input:checkbox[name='checkbox']:checked").each(function () {
        str += $(this).val() + ",";
    });
    if (str != "") str = str.substr(0, str.length - 1);
    $("#IntervalWeek").val(str);
}

//数据验证
var fnCheckData = function () {
    try {
        if ($("#PlanName").val() == "") {
            return "任务名不能为空！";
        }

        var reg = /^\d+(\.\d+)?$/;
        if ($("#JobRepeatInterval").val() != "") {
            if (reg.test($("#JobRepeatInterval").val()) == false) {
                return "运行间隔需要输入非零数字!";
            }
        }
    }
    catch (e)
    { }
}

//生成任务计划
function CreateTaskPlan() {
    //验证
    var msg = fnCheckData();
    if (typeof (msg) != "undefined" && msg != "") {
        topWin.ShowMsg("提示", msg);
        return;
    }
    //初始值获取
    var isRepeat = $("#IsRepeat").combobox("getValue");
    var planStartTime = $("#PlanStartTime").combobox("getValue");
    var planEndTime = $("#PlanEndTime").combobox("getValue");
    var planStatus = $("#PlanStatus").combobox("getValue");
    var planGroup = $("#PlanGroup").combobox("getValue");
    $("#PlanGroupH").val(planGroup);
    $("#IsRepeatH").val(isRepeat);
    $("#PlanStartTimeH").val(planStartTime);
    $("#PlanEndTimeH").val(planEndTime);
    $("#PlanStatusH").val(planStatus);
    var taskPlan = "任务";
    var cronExp = "";
    var JobRepeatIntervalType = $("#JobRepeatIntervalType").combobox("getValue");
    var JobRepeatInterval = $("#JobRepeatInterval").val();
    var EverydayIntervalType = $("#EverydayIntervalType").combobox("getValue");
    var EverydayIntervalFrmTime = $("#EverydayIntervalFrmTime").val();
    var EverydayIntervalEndTime = $("#EverydayIntervalEndTime").val();
    var EverydayInterval = $("#EverydayInterval").val();
    var EverydayIntervalUnit = $("#EverydayIntervalUnit").combobox('getValue');
    var strEverydayIntervalUnit = "";
    switch (EverydayIntervalUnit) {
        case "H":
            strEverydayIntervalUnit = "小时";
            break;
        case "M":
            strEverydayIntervalUnit = "分";
            break;
        case "S":
            strEverydayIntervalUnit = "秒";
            break;
    }

    //周频率
    var IntervalWeek = $("#IntervalWeek").val().split(",");
    var strIntervalWeek = "";
    for (var i = 0; i < IntervalWeek.length; i++) {
        if (IntervalWeek[i] == "1") strIntervalWeek += "星期日,";
        if (IntervalWeek[i] == "2") strIntervalWeek += "星期一,";
        if (IntervalWeek[i] == "3") strIntervalWeek += "星期二,";
        if (IntervalWeek[i] == "4") strIntervalWeek += "星期三,";
        if (IntervalWeek[i] == "5") strIntervalWeek += "星期四,";
        if (IntervalWeek[i] == "6") strIntervalWeek += "星期五,";
        if (IntervalWeek[i] == "7") strIntervalWeek += "星期六,";
    }

    //月频率
    var IntervalMonthType = $("#IntervalMonthType").combobox("getValue");
    var IntervalMonthDay = $("#IntervalMonthDay").val();
    var NumWeekDay = $("#NumWeekDay").val();
    var strNumWeekDay = "";
    var IntervalMonthWeek = $("#IntervalMonthWeek").val();
    var strIntervalMonthWeek = "";
    switch (NumWeekDay) {
        case "1":
        case "2":
        case "3":
        case "4":
            strNumWeekDay = "第" + NumWeekDay + "个";
            break;
        case "5":
            strNumWeekDay = "最后一个";
            break;
    }
    switch (IntervalMonthWeek) {
        case "1":
            strIntervalMonthWeek = "周一";
            break;
        case "2":
            strIntervalMonthWeek = "周二";
            break;
        case "3":
            strIntervalMonthWeek = "周三";
            break;
        case "4":
            strIntervalMonthWeek = "周四";
            break;
        case "5":
            strIntervalMonthWeek = "周五";
            break;
        case "6":
            strIntervalMonthWeek = "周六";
            break;
        case "7":
            strIntervalMonthWeek = "周日";
            break;
        case "8":
            strIntervalMonthWeek = "工作日";
            break;
        case "9":
            strIntervalMonthWeek = "休息日";
            break;
    }

    //开始组装计划
    taskPlan += $("#PlanName").val() + "";
    taskPlan += "在" + planStartTime + "";
    taskPlan += "和" + planEndTime + "之间使用计划.";

    //格式化字符串
    var myDate = new Date();
    var JobStartDateDate = planStartTime == "" ? myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + " " + myDate.getHours() + ":" + myDate.getMinutes() + ":" + myDate.getSeconds() : planStartTime.split(" ")[0].split("-");
    var JobStartDateTime = planEndTime == "" ? (myDate.getFullYear() + 10) + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate() + " 00:00:00" : planEndTime.split(" ")[1].split(":");
    var arrEverydayIntervalFrmTime = EverydayIntervalFrmTime.split(":");
    var arrEverydayIntervalEndTime = EverydayIntervalEndTime.split(":");

    if (isRepeat == "1") {
        if (JobRepeatIntervalType == "EveryDay") {
            if (EverydayIntervalType == "1") {
                //每3天在07:04和19:10之间、每隔3分执行.
                taskPlan += "每" + JobRepeatInterval + "天在" + EverydayIntervalFrmTime + "和" + EverydayIntervalEndTime + "之间、每隔" +
                        EverydayInterval + strEverydayIntervalUnit + "执行.";
                switch (EverydayIntervalUnit) {
                    case "H":
                        cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + "/" + EverydayInterval + " "
                + "1/" + JobRepeatInterval + " " + "*" + " " + "?" + " " + "*";
                        break;
                    case "M":
                        cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + "/" + EverydayInterval + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + "1/" + JobRepeatInterval + " " + "*" + " " + "?" + " " + "*";
                        break;
                    case "S":
                        cronExp = "0" + "/" + EverydayInterval + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + "1/" + JobRepeatInterval + " " + "*" + " " + "?" + " " + "*";
                        break;
                }
            }
            else {
                //每3天在07:04执行.
                taskPlan += "每" + JobRepeatInterval + "天在" + EverydayIntervalFrmTime + "执行.";
                cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + " " + arrEverydayIntervalFrmTime[0] * 1 + " "
            + "1/" + JobRepeatInterval + " " + "*" + " " + "?" + " " + "*";
            }
        }
        else if (JobRepeatIntervalType == "EveryWeek") {
            if (EverydayIntervalType == "1") {
                //每3周的星期一,星期三,星期六,星期日,在07:04和19:10之间、每隔3分执行.
                taskPlan += "每" + JobRepeatInterval + "周的" + strIntervalWeek + "在" + EverydayIntervalFrmTime + "和" + EverydayIntervalEndTime + "之间、每隔" +
                        EverydayInterval + strEverydayIntervalUnit + "执行.";
                switch (EverydayIntervalUnit) {
                    case "H":
                        cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + "/" + EverydayInterval + " "
                + "?" + " " + "*" + " " + IntervalWeek + "/" + JobRepeatInterval + " " + "*";
                        break;
                    case "M":
                        cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + "/" + EverydayInterval + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + "?" + " " + "*" + " " + IntervalWeek + "/" + JobRepeatInterval + " " + "*";
                        break;
                    case "S":
                        cronExp = "0" + "/" + EverydayInterval + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + "?" + " " + "*" + " " + IntervalWeek + "/" + JobRepeatInterval + " " + "*";
                        break;
                }
            }
            else {
                //每3周的星期一,星期三,星期四,在07:04执行.
                taskPlan += "每" + JobRepeatInterval + "周的" + strIntervalWeek + "在" + EverydayIntervalFrmTime + "执行.";
                cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + " " + arrEverydayIntervalFrmTime[0] * 1 + " "
            + "?" + " " + "*" + " " + IntervalWeek + "/" + JobRepeatInterval + " " + "*";
            }
        }
        else if (JobRepeatIntervalType == "EveryMonth") {
            if (IntervalMonthType == "1") {
                if (EverydayIntervalType == "1") {
                    //每3个月,于当月第3天,在07:04和19:10之间、每隔3分执行.
                    taskPlan += "每" + JobRepeatInterval + "个月,于当月第" + IntervalMonthDay + "天,在" + EverydayIntervalFrmTime + "和" + EverydayIntervalEndTime + "之间、每隔" +
                        EverydayInterval + strEverydayIntervalUnit + "执行.";
                    switch (EverydayIntervalUnit) {
                        case "H":
                            cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + "/" + EverydayInterval + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + "?" + " " + "*";
                            break;
                        case "M":
                            cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + "/" + EverydayInterval + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + "?" + " " + "*";
                            break;
                        case "S":
                            cronExp = "0" + "/" + EverydayInterval + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + "?" + " " + "*";
                            break;
                    }
                }
                else {
                    //每3个月,于当月第3天,在07:04执行.
                    taskPlan += "每" + JobRepeatInterval + "个月,于当月第" + IntervalMonthDay + "天,在" + EverydayIntervalFrmTime + "执行.";
                    cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + " " + arrEverydayIntervalFrmTime[0] * 1 + " "
            + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + "?" + " " + "*";
                }
            }
            else {
                if (EverydayIntervalType == "1") {
                    //每3个月的第1个周三,在07:04和19:10之间、每隔3分执行.
                    taskPlan += "每" + JobRepeatInterval + "个月的" + strNumWeekDay + strIntervalMonthWeek + ",在" + EverydayIntervalFrmTime + "和" + EverydayIntervalEndTime + "之间、每隔" +
                        EverydayInterval + strEverydayIntervalUnit + "执行.";
                    if (NumWeekDay < 5) {
                        //每月的第几个周几
                        switch (EverydayIntervalUnit) {
                            case "H":
                                cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + "/" + EverydayInterval + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + IntervalMonthWeek + "#" + NumWeekDay + " " + "*";
                                break;
                            case "M":
                                cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + "/" + EverydayInterval + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + IntervalMonthWeek + "#" + NumWeekDay + " " + "*";
                                break;
                            case "S":
                                cronExp = "0" + "/" + EverydayInterval + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + IntervalMonthWeek + "#" + NumWeekDay + " " + "*";
                                break;
                        }
                    }
                    else {
                        //每月的最后周几
                        switch (EverydayIntervalUnit) {
                            case "H":
                                cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + "/" + EverydayInterval + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + IntervalMonthWeek + "L" + " " + "*";
                                break;
                            case "M":
                                cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + "/" + EverydayInterval + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + IntervalMonthWeek + "L" + " " + "*";
                                break;
                            case "S":
                                cronExp = "0" + "/" + EverydayInterval + " " + arrEverydayIntervalFrmTime[1] * 1 + "-" + arrEverydayIntervalEndTime[1] * 1 + " "
                + arrEverydayIntervalFrmTime[0] * 1 + "-" + arrEverydayIntervalEndTime[0] * 1 + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + IntervalMonthWeek + "L" + " " + "*";
                                break;
                        }
                    }
                }
                else {
                    //每3个月的第2个周三,在07:04执行.
                    taskPlan += "每" + JobRepeatInterval + "个月的" + strNumWeekDay + strIntervalMonthWeek + ",在" + EverydayIntervalFrmTime + "执行.";
                    if (NumWeekDay < 5) {
                        //每月的第几个周几
                        cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + " " + arrEverydayIntervalFrmTime[0] * 1 + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + IntervalMonthWeek + "#" + NumWeekDay + " " + "*";
                    }
                    else {
                        //每月的最后周几
                        cronExp = "0" + " " + arrEverydayIntervalFrmTime[1] * 1 + " " + arrEverydayIntervalFrmTime[0] * 1 + " "
                + IntervalMonthDay + " " + "1/" + JobRepeatInterval + " " + IntervalMonthWeek + "L" + " " + "*";
                    }
                }
            }
        }
        taskPlan += "";
    }
    else {
        taskPlan = "任务" + $("#PlanName").val() + "在" + planStartTime + " 运行一次";
        cronExp = "0" + " " + JobStartDateTime[1] * 1 + " " + JobStartDateTime[0] * 1 + " "
            + JobStartDateDate[2] * 1 + " " + JobStartDateDate[1] * 1 + " " + "?" + " " + JobStartDateDate[0] * 1;
    }
    $("#PlanDes").val(taskPlan);
    $("#CronExp").val(cronExp);

}
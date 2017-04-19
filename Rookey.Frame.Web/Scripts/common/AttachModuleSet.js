$(function () {

});

//获取启用的附属模块信息
function GetEnabledAttachModules() {
    var obj = [];
    $("span[id^='Sys_ModuleId_']").each(function () {
        var attachModuleId = $(this).attr("attachModuleId");
        if ($("#IsValid_" + attachModuleId).attr("checked") == "checked") { //启用
            var sort = $("#Sort_" + attachModuleId).numberbox("getValue"); //排序
            var inGrid = $("#InGrid_" + attachModuleId).attr("checked") == "checked";
            obj.push({ Sys_ModuleId: attachModuleId, Sort: sort, AttachModuleInGrid: inGrid });
        }
    });
    return obj;
}
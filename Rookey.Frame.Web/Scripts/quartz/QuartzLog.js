
$(function () {
    var columns = [[{ field: 'grid_checkbox', checkbox: true },
                   { title: '任务名称', field: 'jobName', align: 'left', width: 100 },
                   { title: '任务组', field: 'jobGroup', width: 100 },
                   { title: '计划名称', field: 'planName', align: 'left', width: 100 },
                   { title: '计划组', field: 'planGroupName', align: 'left', width: 100 },
                   { title: '执行时间', field: 'planExecTime', width: 150 }, ]];
    var jobName = GetLocalQueryString("jobName");
    var jobGroup = GetLocalQueryString("jobGroup");
    $('#dg').datagrid({
        url: '/Quartz/GetQuartzLog.html',
        queryParams: { jobName: jobName, jobGroup: jobGroup },
        loadMsg: "正在加载数据，请稍候...",
        striped: true,
        fitColumns: false,
        pagination: false,
        nowrap: true,
        rownumbers: true,
        singleSelect: true,
        toolbar: null,
        columns: columns
    });
});
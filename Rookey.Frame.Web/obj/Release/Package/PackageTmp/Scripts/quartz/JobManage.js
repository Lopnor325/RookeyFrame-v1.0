
$(function () {
    var toolbar = [{
        plain: true,
        text: '新增',
        iconCls: 'eu-icon-add',
        handler: function () {
            if (typeof (AddJob) == 'function') {
                AddJob();
            }
        }
    }, '-', {
        plain: true,
        text: '删除',
        iconCls: 'eu-icon-cancel',
        handler: function () {
            if (typeof (DelJob) == 'function') {
                DelJob();
            }
        }
    }, '-', {
        plain: true,
        text: '立即运行',
        iconCls: 'eu-icon-ok',
        handler: function () {
            if (typeof (RunJobNow) == 'function') {
                RunJobNow();
            }
        }
    }, '-', {
        plain: true,
        text: '暂停运行',
        iconCls: 'eu-icon-remove',
        handler: function () {
            if (typeof (SuspendJobRun) == 'function') {
                SuspendJobRun();
            }
        }
    }, '-', {
        plain: true,
        text: '恢复运行',
        iconCls: 'eu-icon-redo',
        handler: function () {
            if (typeof (ResumeJobRun) == 'function') {
                ResumeJobRun();
            }
        }
    }];
    var columns = [[{ field: 'grid_checkbox', checkbox: true },
                   { title: '任务名称', field: 'jobName', align: 'left', width: 200 },
                   { title: '任务组', field: 'jobGroup', width: 200 },
                   {
                       title: '任务类型', field: 'jobType', width: 200,
                       formatter: function (value, row, index) {
                           var jobtype = row.jobType;
                           if (jobtype.length > 20) jobtype = jobtype.substr(0, 20) + "...";
                           return "<div title='" + row.jobType + "'>" + jobtype + "</div>";
                       }
                   },
                   { title: '状态', field: 'jobStatus', width: 200 },
                   { title: '任务描述', field: 'jobDes', width: 200 },
                   {
                       title: '查看日志', field: 'viewLog', width: 200,
                       formatter: function (value, row, index) {
                           var jobName = row['jobName'];
                           var jobGroup = row['jobGroup'];
                           return "<a href='javascript:void(0)' onclick='ViewLog(\"" + jobName + "\",\"" + jobGroup + "\")'>" + value + "</a>";
                       }
                   }]];

    $('#dg').datagrid({
        url: '/Quartz/GetJobList.html',
        queryParams: {},
        loadMsg: "正在加载数据，请稍候...",
        striped: true,
        height: GetBodyHeight() - 4,
        fitColumns: false,
        pagination: false,
        nowrap: true,
        rownumbers: true,
        singleSelect: true,
        toolbar: toolbar,
        loadFilter: function (data) {
            return data;
        },
        selectOnCheck: true,//选择时选中checkbox
        checkOnSelect: true,//选中时选择checkbox
        columns: columns,
        view: detailview,
        detailFormatter: function (index, row) {
            return '<div style="padding:2px"><table class="ddv"></table></div>';
        },
        onExpandRow: function (index, row) {
            var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
            ddv.datagrid({
                url: '/Quartz/GetJobPlanList.html?jobName=' + row['jobName'] + '&jobGroupName=' + row['jobGroup'],
                fitColumns: true,
                singleSelect: true,
                rownumbers: true,
                loadMsg: '',
                height: 'auto',
                columns: [[
                    { title: '计划名称', field: 'planName', align: 'left', width: 60 },
                    { title: '计划组', field: 'planGroupName', align: 'left', width: 60 },
                    { title: '执行状态', field: 'planStatus', width: 50 },
                    { title: '开始时间', field: 'planStartTime', width: 100 },
                    { title: '结束时间', field: 'planEndTime', width: 100 },
                    { title: '上次执行时间', field: 'planPreFireTime', width: 100 },
                    { title: '下次执行时间', field: 'planNextFireTime', width: 100 },
                    {
                        title: '计划说明', field: 'planDes', align: 'left', width: 100,
                        formatter: function (value, row, index) {
                            {
                                var planDes = value;
                                if (planDes.length > 30) planDes = planDes.substr(0, 30) + "...";
                                return "<div title='" + value + "'>" + planDes + "</div>";
                            }
                        }
                    }
                ]],
                onResize: function () {
                    $('#dg').datagrid('fixDetailRowHeight', index);
                },
                onSelect: function (rowIndex, rowData) {
                    $(this).datagrid('unselectAll');
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg').datagrid('fixDetailRowHeight', index);
                    }, 10);
                }
            });
            $('#dg').datagrid('fixDetailRowHeight', index);
        }
    });
});

//操作任务的方法
function JobOperate(op) {
    var rows = $("#dg").datagrid("getSelections");
    if (!rows || rows.length == 0) {
        topWin.ShowMsg("提示", "请至少选择一条记录！", null, null, 1);
        return;
    }
    var obj = rows[0];
    var msgTitle = '操作提示';
    var msg = '确定要处理吗？';
    topWin.ShowConfirmMsg(msgTitle, msg, function (action) {
        if (action) {
            var url = "/Quartz/OperateJob.html";
            var param = [
                { name: "op", value: op },
                { name: "jobName", value: obj.jobName },
                { name: "jobGroupName", value: obj.jobGroup },
                { name: "r", value: Math.random() }];
            ExecuteCommonAjax(url, param, function (result) {
                RefreshGrid();
            }, true);
        }
    });
}

//新增任务
function AddJob() {
    var url = '/Quartz/AddJob.html';
    var currTabIndex = GetSelectTabIndex(); //当前grid网格页面的tabindex
    if (currTabIndex)
        url += "?tb=" + currTabIndex;
    AddTab(null, "新增任务", url, "eu-icon-add");
}

//删除任务
function DelJob() {
    JobOperate("DeleteJob");
}

//立即运行
function RunJobNow() {
    JobOperate("ExecuteJob");
}

//暂停运行
function SuspendJobRun() {
    JobOperate("PauseJob");
}

//恢复运行
function ResumeJobRun() {
    JobOperate("ResumeJob");
}

//刷新网格
function RefreshGrid() {
    $("#dg").datagrid("reload");
}

//查看日志
function ViewLog(jobName, jobGroup) {
    var toolbar = [{
        text: '关闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var title = '查看执行日志-' + jobName + '(' + jobGroup + ')';
    var url = '/Quartz/QuartzLog.html?jobName=' + jobName + '&jobGroup=' + jobGroup;
    topWin.OpenDialog(title, url, toolbar, 800, 500);
}
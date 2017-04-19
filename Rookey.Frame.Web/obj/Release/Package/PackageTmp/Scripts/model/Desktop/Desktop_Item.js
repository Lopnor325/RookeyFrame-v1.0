//保存前验证
function OverBeforeSaveVerify(backFun) {
    var gridObj = $("table[id^='grid_']").eq(0);
    var rows = gridObj.datagrid("getRows");
    if (!rows || rows.length == 0) {
        backFun('至少要添加一条【桌面项标签】记录！');
        return;
    }
    backFun(null);
}
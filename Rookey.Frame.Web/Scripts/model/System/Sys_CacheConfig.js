//刷新模块缓存
function RefreshModuleCache(obj) {
    var rows = GetFinalSelectRows(obj);
    if (rows && rows.length > 0) {
        if (!rows[0].IsEnableCache) {
            topWin.ShowMsg("提示", "当前模块未启动缓存，无需刷新！", null, null, 1);
            return null;
        }
        var msg = '确定要刷新' + rows[0].ModuleName + '缓存吗？';
        topWin.ShowConfirmMsg('确认', msg, function (action) {
            if (action) {
                var url = '/' + CommonController.Async_System_Controller + '/RefreshCache.html';
                var params = { moduleName: rows[0].ModuleName, type: 1 };
                ExecuteCommonAjax(url, params, null, true);
            }
        });
    }
}

//刷新所有缓存
function RefreshAllCache(obj) {
    topWin.ShowConfirmMsg('确认', '确定要刷新所有模块缓存吗？', function (action) {
        if (action) {
            var url = '/' + CommonController.Async_System_Controller + '/RefreshCache.html';
            var params = { type: 2 };
            ExecuteCommonAjax(url, params, null, true);
        }
    });
}
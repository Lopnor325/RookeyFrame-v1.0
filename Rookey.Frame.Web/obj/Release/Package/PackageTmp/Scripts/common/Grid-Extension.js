var page = GetLocalQueryString("page");

//初始化
$(function () {
    if (page == 'grid' && $('#btn_filterSearch').attr('isrf') == '1') {
        EnableRowFilter();
    }
});

//启用行过滤
function EnableRowFilter() {
    var ruleFilterStr = $('#ruleFilters').val();
    if (page == 'grid' && ruleFilterStr && ruleFilterStr.length > 0) {
        var gridObj = $('#mainGrid');
        //过滤字段处理
        var tempRules = decodeURIComponent($('#ruleFilters').val());
        var ruleFilters = eval("(" + tempRules + ")"); //过滤规则
        var noFilterFields = $('#ruleFilters').attr("noFilterFields"); //不过滤字段
        gridObj.datagrid('enableFilter', ruleFilters); // 启用过滤
        //移除不过滤的字段
        if (noFilterFields && noFilterFields.length > 0) {
            var token = noFilterFields.split(",");
            for (var i = 0; i < token.length; i++) {
                gridObj.datagrid('removeFilterRule', token[i]);
                $("#regon_main .datagrid-filter-c input.datagrid-filter[name='" + token[i] + "']").remove();
            }
        }
    }
}

//网格规则行过滤下拉树数据过滤
function RuleFilterLoadComTreeFilter(data, parent) {
    if (typeof (data) == "object") {
        arr = [];
        arr.push(data);
        return arr;
    }
    if (data && data.length > 0) {
        if (typeof (data) == "string") {
            var tempData = eval("(" + data + ")");
            arr = [];
            arr.push(tempData);
            return arr;
        }
        else {
            return data;
        }
    }
    return null;
}
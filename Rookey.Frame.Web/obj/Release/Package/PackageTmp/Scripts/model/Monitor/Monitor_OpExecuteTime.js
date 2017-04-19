//重写格式化函数
function OverGeneralFormatter(value, row, index, moduleName, fieldName, paramsObj) {
    if (fieldName == "ExecuteMiniSeconds") {
        var v = (value / 1000).toFixed(2);
        if (v > 0.5) {
            return '<span style="color:red">' + v + '秒</span>';
        }
        return v + '秒';
    }
    return value;
}
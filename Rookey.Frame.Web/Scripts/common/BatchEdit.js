//初始化
$(function () {
    $("#batchEditForm input[type=checkbox]").click(function () {
        if ($(this).attr("checked")) {
            $(this).attr("value", "1");
        }
        else {
            $(this).attr("value", "0");
        }
    });
    $("#batchEditForm input.textbox-text").removeClass("validatebox-text").removeClass("validatebox-invalid");
    $("input[id^=chk_]").click(function () {
        var val = $(this).val();
        var dom = $("#tr_" + val);
        var fieldDom = $('#' + val);
        var tempDom = fieldDom.next('span').find('input.textbox-text');
        if ($(this).attr("checked") == "checked") {
            dom.css("display", "block");
            if (!tempDom.hasClass("validatebox-text")) {
                tempDom.addClass("validatebox-text").addClass("validatebox-invalid");
            }
        }
        else {
            dom.css("display", "none");
            if (tempDom.hasClass("validatebox-text")) {
                tempDom.removeClass("validatebox-text").removeClass("validatebox-invalid");
            }
        }
    });
});

//下拉框、下拉列表、下拉树数据过滤
//fieldName:字段名
//valueField:值字段
//textField:显示字段
//data:当前数据
//parentDom:下拉树时父节点DOM对象
function LoadFilter(fieldName, valueField, textField, data, parentDom) {
    //调用模块自定义事件
    if (typeof (OverLoadFilter) == "function") {
        return OverLoadFilter(fieldName, valueField, textField, data, parentDom);
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

//获取批量编辑数据
//backFun:回调函数
function GetBatchEditData(backFun) {
    var form = $("#batchEditForm");
    var flag = form.form("validate");
    if (!flag) return;
    var data = form.fixedSerializeArrayFix();
    var o = [];
    $.each(data, function (index) {
        var name = this['name'];
        var dom = $("#tr_" + name);
        if (dom.css("display") == "block") {
            o.push(this);
        }
    });
    var updateRange = $("input[name='rdItem']:radio:checked").val();
    if (typeof (backFun) == "function") {
        backFun(updateRange, o);
    }
    return o;
}
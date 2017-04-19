
$(function () {
    $('#chk_selectAll').click(function () {
        if ($(this).attr("checked") == "checked") {
            $('#tb_fields input:enabled:checkbox').attr("checked", "checked");
        }
        else {
            $('#tb_fields input:enabled:checkbox').removeAttr("checked");
        }
    });
    $('#tb_fields input:enabled:checkbox').click(function () {
        if ($(this).attr("checked") == "checked") {
            if ($('#tb_fields input:checkbox:checked').length == $('#tb_fields input:checkbox').length) {
                $('#chk_selectAll').attr("checked", "checked");
            }
        }
        else {
            if ($('#chk_selectAll').attr("checked") == "checked") {
                $('#chk_selectAll').removeAttr("checked");
            }
        }
    });
});

//获取选中的字段
function GetSelectFields() {
    var selectAll = $('#chk_selectAll').attr("checked") == "checked";
    var appType = [];
    if ($('#chk_appToType1').attr('checked') == 'checked') {
        appType.push($('#chk_appToType1').val());
    }
    if ($('#chk_appToType2').attr('checked') == 'checked') {
        appType.push($('#chk_appToType2').val());
    }
    var fields = [];
    if (selectAll) { //选中全部
        fields.push({ FieldName: '-1', Display: '全部' });
        return { AppType: appType, Fields: fields };
    }
    $('#tb_fields input:enabled:checkbox').each(function (i, item) {
        if ($(item).attr("checked") == "checked") {
            fields.push({ FieldName: $(item).val(), Display: $(item).attr("display") });
        }
    });
    if (fields.length == 0) { //未选中
        fields.push({ FieldName: '-2', Display: '无' });
    }
    return { AppType: appType, Fields: fields };
}
//表单序列化扩展
//修改$("form:first").serializeArray(); 中某一名称的checkbox一个均未选择， 取不到name错误
$.fn.extend({
    "fixedSerializeArray": function () {
        var data = $(this).serializeArray();
        var $chks = $(this).find("#mainContent :checkbox:not(:checked)"); //取得所有未选中的checkbox
        if ($chks.length == 0) {
            return data;
        }
        $chks.each(function () {
            var chkName = $(this).attr("name");
            if (chkName) {
                data.push({ name: chkName, value: $(this).val() });
            }
        });
        return data;
    },
    "fixedSerializeArrayFix": function () {
        var $form = $(this);
        var formId = $form.attr('id');
        var $foreinInputs = $form.find("#mainContent input[foreignField='1']");
        if ($foreinInputs.length > 0) {
            $foreinInputs.each(function () {
                var foreinName = $(this).attr("id");
                if (foreinName) {
                    var obj = $form.find("input.textbox-value[name='" + foreinName + "']");
                    if (obj.length > 0) {
                        if (formId != 'searchform') { //普通表单
                            var v = $(this).attr('v');
                            var textValue = $(this).next('span').find('input.textbox-text').val();
                            if (!textValue) {
                                v = '';
                            }
                            obj.attr('value', v);
                        }
                        else { //搜索表单
                            var v = $(this).attr('v');
                            if (v == undefined || v == null || v == '') {
                                v = $(this).next('span').find('input.textbox-text').val();
                            }
                            obj.attr('value', v);
                        }
                    }
                }
            });
        }
        if (formId == 'searchform') { //搜索表单
            var $textbox = $form.find("#mainContent input[noforein='1']");
            $textbox.each(function () {
                var fn = $(this).attr("id");
                if (fn) {
                    var obj = $form.find("input.textbox-value[name='" + fn + "']");
                    if (obj.length > 0) {
                        var v = $(this).next('span').find('input.textbox-text').val();
                        obj.attr('value', v);
                    }
                }
            });
        }
        var data = $form.serializeArray();
        var tempData = [];
        //data去重
        for (var j = 0; j < data.length; j++) {
            if (data[j].name == 'Id')
                continue;
            var isContain = false;
            for (var i = 0; i < tempData.length; i++) {
                if (tempData[i].name == data[j].name) {
                    isContain = true;
                    break;
                }
            }
            if (!isContain) {
                tempData.push(data[j]);
            }
        }
        data = tempData;
        //单选、多选checkbox处理
        var $chks = $form.find("#mainContent :checkbox"); //取得所有checkbox
        if ($chks.length > 0) {
            var mutiChkObjs = [];
            var flag = 0;
            $chks.each(function () {
                var chkName = $(this).attr("name");
                if (chkName) {
                    var index = -1;
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].name == chkName) {
                            index = i;
                            break;
                        }
                    }
                    if ($(this).attr('singleChk') == '1') { //对单选checkbox处理
                        var obj = { name: chkName, value: $(this).val() == '1' };
                        if (index > -1)
                            data.splice(index, 1, obj);
                        else
                            data.push(obj);
                    }
                    else { //对多选checkbox处理
                        if (index > -1) {
                            if (flag == 0)
                                data[index].value = $(this).val();
                            else
                                data[index].value += "," + $(this).val();
                            flag++;
                        }
                    }
                }
            });
        }
        //处理label控件
        var $labels = $form.find("#mainContent span[fieldSpan='0']");
        $labels.each(function (i, item) {
            var fieldName = $(this).attr("id");
            if (fieldName) {
                var obj = { name: fieldName, value: $(item).attr('value') };
                data.push(obj);
            }
        });
        return data;
    }
});

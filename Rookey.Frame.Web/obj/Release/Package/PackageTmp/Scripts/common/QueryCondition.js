/*---------查询条件封装---------------------*/
//使用方法: $('#formCondition').QueryConditionInit('员工管理');
/*------------------------------------------*/
; (function ($) {
    $.fn.extend({
        //查询条件初始化
        //moduleName:模块名称
        //conditionItems:初始化条件，格式[{Field:'Code',Method:0,Value:'编码',OrGroup:'and'}]
        //filterFieldBackFun:过滤字段回调函数，格式 function filterFieldBackFun(data){return data;},其中data为当前加载的字段数据
        QueryConditionInit: function (moduleName, conditionItems, filterFieldBackFun) {
            var guid = Guid.NewGuid().ToString();
            var tb_conditionId = "tb_Condition" + guid;
            var tb_tempId = "tb_Temp" + guid;
            var fields = null; //条件字段
            var queryMethods = null; //查询方法
            //选择字段后
            var searchFieldSelected = function (conditionItem, field, conditionField, keyObj) {
                var tdObj = keyObj.parent();
                tdObj.html("<input style='width: 200px;' tt='key' />");
                keyObj = $("input", tdObj);
                var tempQueryMethod = queryMethods;
                var idField = "Id";
                var valueField = "Name";
                keyObj.validatebox({
                    required: true
                });
                if ((field.TempEnums != null && field.TempEnums != undefined) ||
                    (field.TempDics != null && field.TempDics != undefined)) //枚举、字典类型字段
                {
                    var executeMethod = field.TempEnums != null ? "BindEnumFieldData" : "BindDictionaryData";
                    var url = "/" + CommonController.Async_Data_Controller + "/" + executeMethod + ".html?moduleName=" + encodeURI(moduleName) + "&fieldName=" + field.Sys_FieldName;
                    keyObj.combobox({
                        url: url,
                        required: true,
                        valueField: idField,
                        textField: valueField
                    });
                    tempQueryMethod = [];
                    for (var i = 0; i < queryMethods.length; i++) {
                        if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "不等于") {
                            tempQueryMethod.push(queryMethods[i]);
                        }
                    }
                    if (conditionItem != undefined && conditionItem != null) {
                        keyObj.combobox("setValue", conditionItem.Value);
                    }
                }
                else {
                    switch (field.ControlType) {
                        case 0: //文本框
                        case 12: //文本域
                        case 13: //富文本框
                            tempQueryMethod = [];
                            for (var i = 0; i < queryMethods.length; i++) {
                                if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "不等于" ||
                                    queryMethods[i].Name == "包含" || queryMethods[i].Name == "开头为" ||
                                    queryMethods[i].Name == "结尾为" || queryMethods[i].Name == "包含于") {
                                    tempQueryMethod.push(queryMethods[i]);
                                }
                            }
                            keyObj.textbox({ required: true });
                            if (conditionItem != undefined && conditionItem != null) {
                                keyObj.textbox('setValue', conditionItem.Value);
                            }
                            break;
                        case 1: //checkbox
                            tempQueryMethod = [];
                            for (var i = 0; i < queryMethods.length; i++) {
                                if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "不等于") {
                                    tempQueryMethod.push(queryMethods[i]);
                                }
                            }
                            keyObj.combobox({
                                data: [{ Id: 1, Name: '是' }, { Id: 0, Name: '否' }],
                                required: true,
                                valueField: 'Id',
                                textField: 'Name',
                                editable: false
                            });
                            if (conditionItem != undefined && conditionItem != null) {
                                keyObj.combobox("setValue", conditionItem.Value);
                            }
                            break;
                        case 10: //日期输入框
                            tempQueryMethod = [];
                            for (var i = 0; i < queryMethods.length; i++) {
                                if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "大于" ||
                                    queryMethods[i].Name == "小于") {
                                    tempQueryMethod.push(queryMethods[i]);
                                }
                            }
                            keyObj.datebox({
                                required: true
                            });
                            if (conditionItem != undefined && conditionItem != null) {
                                keyObj.combobox("setValue", conditionItem.Value);
                            }
                            break;
                        case 6: //数值输入框
                            tempQueryMethod = [];
                            for (var i = 0; i < queryMethods.length; i++) {
                                if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "大于" ||
                                    queryMethods[i].Name == "小于" || queryMethods[i].Name == "大于等于" ||
                                     queryMethods[i].Name == "小于等于") {
                                    tempQueryMethod.push(queryMethods[i]);
                                }
                            }
                            keyObj.numberbox({
                                precision: field.TempSysField.Precision,
                                required: true
                            });
                            if (conditionItem != undefined && conditionItem != null) {
                                keyObj.numberbox('setValue', conditionItem.Value);
                            }
                            break;
                        case 7: //整形数值输入框
                            tempQueryMethod = [];
                            for (var i = 0; i < queryMethods.length; i++) {
                                if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "大于" ||
                                    queryMethods[i].Name == "小于" || queryMethods[i].Name == "大于等于" ||
                                     queryMethods[i].Name == "小于等于") {
                                    tempQueryMethod.push(queryMethods[i]);
                                }
                            }
                            keyObj.numberbox({
                                precision: 0,
                                required: true
                            });
                            if (conditionItem != undefined && conditionItem != null) {
                                keyObj.numberbox('setValue', conditionItem.Value);
                            }
                            break;
                        case 11: //日期时间输入框
                            tempQueryMethod = [];
                            for (var i = 0; i < queryMethods.length; i++) {
                                if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "大于" ||
                                    queryMethods[i].Name == "小于") {
                                    tempQueryMethod.push(queryMethods[i]);
                                }
                            }
                            keyObj.datetimebox({
                                required: true
                            });
                            if (conditionItem != undefined && conditionItem != null) {
                                keyObj.combobox("setValue", conditionItem.Value);
                            }
                            break;
                        case 3: //下拉列表框
                        case 5: //下拉树
                            tempQueryMethod = [];
                            for (var i = 0; i < queryMethods.length; i++) {
                                if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "不等于") {
                                    tempQueryMethod.push(queryMethods[i]);
                                }
                            }
                            if (field.ControlType == 5) {
                                keyObj.combotree({
                                    url: field.UrlOrData,
                                    required: true,
                                    valueField: field.ValueField,
                                    textField: field.TextField,
                                    delay: 300,
                                    editable: true,
                                    keyHandler: {
                                        query: function (q) {
                                            var tree = keyObj.combotree('tree');
                                            tree.tree("search", q);
                                        }
                                    },
                                    loadFilter: function (data) {
                                        if (typeof (data) == 'string') {
                                            var tempData = eval('(' + data + ')');
                                            return tempData;
                                        } else {
                                            var arr = []; arr.push(data);
                                            return arr;
                                        }
                                    }
                                });
                            }
                            else {
                                keyObj.combobox({
                                    url: field.UrlOrData,
                                    required: true,
                                    valueField: field.ValueField,
                                    textField: field.TextField
                                });
                            }
                            if (conditionItem != undefined && conditionItem != null) {
                                if (field.ControlType == 5)
                                    keyObj.combotree("setValue", conditionItem.Value);
                                else
                                    keyObj.combobox("setValue", conditionItem.Value);
                            }
                            break;
                        case 4: //弹出选择（外键）
                            tempQueryMethod = [];
                            for (var i = 0; i < queryMethods.length; i++) {
                                if (queryMethods[i].Name == "等于" || queryMethods[i].Name == "不等于") {
                                    tempQueryMethod.push(queryMethods[i]);
                                }
                            }
                            keyObj.textbox({
                                icons: [{
                                    editable: false,
                                    iconCls: 'eu-icon-search',
                                    handler: function (e) {
                                        SelectModuleData(field.TempSysField.ForeignModuleName, function (row) {
                                            keyObj.textbox('setValue', row[field.ValueField]);
                                            keyObj.textbox('setText', row[field.TextField]);
                                        });
                                    }
                                }]
                            });
                            if (conditionItem != undefined && conditionItem != null) {
                                keyObj.val(conditionItem.ValueText);
                            }
                            break;
                    }
                }
                //设置查询方式
                //条件
                conditionField.combobox({
                    data: tempQueryMethod,
                    required: true,
                    editable: false,
                    valueField: 'Id',
                    textField: 'Name',
                    multiple: false,
                    filter: function (q, row) {
                        var opts = $(this).combobox("options");
                        return row[opts.textField].indexOf(q) == 0;
                    },
                    onLoadSuccess: function () {
                        if (conditionItem == undefined || conditionItem == null) {
                            var rows = conditionField.combobox("getData");
                            if (rows.length > 0) {
                                var value = rows[0].Id;
                                conditionField.combobox("select", value);
                            }
                        }
                        else {
                            conditionField.combobox("select", conditionItem.Method);
                        }
                    }
                });
            };
            //添加条件
            var addCondition = function (conditionItem) {
                var tr = "<tr>" + $("#" + tb_tempId + " tr").html() + "</tr>";
                $("#" + tb_conditionId).append(tr);
                $.parser.parse('#' + tb_conditionId);
                $("#" + tb_conditionId + " a[name='a_add']").linkbutton({
                    iconCls: 'eu-icon-add',
                    onClick: function () {
                        addCondition();
                    }
                });
                $("#" + tb_conditionId + " a[name='a_delete']").linkbutton({
                    iconCls: 'eu-icon-del',
                    onClick: function () {
                        delCondition(this);
                    }
                });
                //查询字段
                var searchField = $("#" + tb_conditionId + " tr:last").find("input[tt='searchField']"); //字段控件
                var keyObj = $("#" + tb_conditionId + " tr:last").find("input[tt='key']"); //值控件
                var conditionField = $("#" + tb_conditionId + " tr:last").find("input[tt='condition']"); //条件控件
                var ralationShip = $("#" + tb_conditionId + " tr:last").find("select[tt='relationship']"); //关系控件
                if (conditionItem != undefined && conditionItem != null) {
                    ralationShip.combobox('setValue', conditionItem.OrGroup);
                }
                var loadField = function () {
                    var queryFieldUrl = fields != null && fields.length > 0 ? '' : '/' + CommonController.Async_System_Controller + '/GetFormFields.html?load_sysfield=1&load_dic=1&moduleName=' + encodeURI(moduleName);
                    searchField.combobox({
                        url: queryFieldUrl,
                        data: fields,
                        required: true,
                        editable: false,
                        valueField: 'Sys_FieldName',
                        textField: 'Display',
                        multiple: false,
                        filter: function (q, row) {
                            var opts = $(this).combobox("options");
                            return row[opts.textField].indexOf(q) == 0;
                        },
                        loadFilter: function (data) {
                            if (typeof (filterFieldBackFun) == "function") {
                                var tempData = filterFieldBackFun(data);
                                if (tempData && tempData.length > 0)
                                    return tempData;
                                return data;
                            }
                            return data;
                        },
                        onLoadSuccess: function () {
                            var data = searchField.combobox("getData");
                            if (conditionItem == undefined || conditionItem == null) {
                                if (data.length > 0) {
                                    var value = data[0].Sys_FieldName;
                                    searchField.combobox("select", value);
                                }
                            }
                            else {
                                searchField.combobox("select", conditionItem.Field);
                            }
                            if (data.length > 0) {
                                fields = data;
                            }
                        },
                        onSelect: function (data) {
                            var trObj = searchField.parent().parent();
                            keyObj = trObj.find("input[tt='key']");
                            var tdObj = keyObj.parent();
                            tdObj.html("<input style='width: 200px;' tt='key' />");
                            keyObj = $("input", tdObj);
                            //字段选择后
                            searchFieldSelected(conditionItem, data, conditionField, keyObj);
                            if (conditionItem != null)
                                $("select[tt='relationship']", trObj).combobox('setValue', conditionItem.OrGroup);
                        }
                    });
                };
                if (queryMethods == null) {
                    $.get('/' + CommonController.Async_System_Controller + '/LoadQueryMethodEnumList.html', function (result) {
                        queryMethods = result;
                        loadField();
                    }, 'json');
                }
                else {
                    loadField();
                }
            }
            var delCondition = function (t) {
                var length = $("#" + tb_conditionId + " tr").size();
                if (length > 1) {
                    var tr = $(t).parents("tr:first").eq(0);
                    tr.remove();
                }
            }
            var conditionHtml = '<div style="margin:5px;">';
            conditionHtml += '<table class="thinBorder padding5" style="width: 100%;">';
            conditionHtml += '<thead>';
            conditionHtml += '<tr>';
            conditionHtml += '<th>字段</th>';
            conditionHtml += '<th>条件</th>';
            conditionHtml += '<th>关键字</th>';
            conditionHtml += '<th>关系</th>';
            conditionHtml += '<th>添加</th>';
            conditionHtml += '<th>删除</th>';
            conditionHtml += '</tr>';
            conditionHtml += '</thead>';
            conditionHtml += '<tbody id="' + tb_conditionId + '">';
            conditionHtml += '</tbody>';
            conditionHtml += '</table>';
            conditionHtml += '<table id="' + tb_tempId + '" style="display: none;">';
            conditionHtml += '<tr>';
            conditionHtml += '<td>';
            conditionHtml += '<input tt="searchField" style="width: 120px;" />';
            conditionHtml += '</td>';
            conditionHtml += '<td>';
            conditionHtml += '<input tt="condition" class="easyui-combobox" data-options="editable:false" style="width: 100px;" />';
            conditionHtml += '</td>';
            conditionHtml += '<td>';
            conditionHtml += '<input type="text" tt="key" class="easyui-textbox" style="width: 200px;" />';
            conditionHtml += '</td>';
            conditionHtml += '<td>';
            conditionHtml += '<select tt="relationship" class="easyui-combobox" data-options="editable:false" style="width: 60px;">';
            conditionHtml += '<option value="and">并且</option>';
            conditionHtml += '<option value="or">或者</option>';
            conditionHtml += '</select>';
            conditionHtml += '</td>';
            conditionHtml += '<td>';
            conditionHtml += '<a name="a_add" class="easyui-linkbutton" plain="true"></a>';
            conditionHtml += '</td>';
            conditionHtml += '<td>';
            conditionHtml += '<a name="a_delete" class="easyui-linkbutton" plain="true"></a>';
            conditionHtml += '</td>';
            conditionHtml += '</tr>';
            conditionHtml += '</table>';
            conditionHtml += '</div>';
            this.html(conditionHtml);
            if (conditionItems && conditionItems != undefined && conditionItems.length > 0) {
                for (var i = 0; i < conditionItems.length; i++) {
                    addCondition(conditionItems[i]);
                }
            }
            else {
                addCondition();
            }
        },
        //获取查询条件
        GetQueryConditions: function () {
            var conditions = [];
            var tbody = this.find("tbody[id^='tb_Condition'] tr");
            var fields = tbody.length > 0 ? tbody.eq(0).find("input[tt='searchField']").combobox('getData') : null;
            if (fields != null && fields.length == 0) return conditions;
            this.find("tbody[id^='tb_Condition'] tr").each(function (i, item) {
                var fieldControl = $(item).find("input[tt='searchField']");
                var fieldName = fieldControl.combobox('getValue');
                var fieldObj = null;
                for (var i = 0; i < fields.length; i++) {
                    if (fields[i].Sys_FieldName == fieldName) {
                        fieldObj = fields[i];
                        break;
                    }
                }
                if (fieldObj != null) {
                    var valueControl = $(item).find("input[tt='key']");
                    var methodControl = $(item).find("input[tt='condition']");
                    var orGroupControl = $(item).find("select[tt='relationship']");
                    var value = null;
                    switch (fieldObj.ControlType) {
                        case 0: //文本框
                        case 12: //文本域
                        case 13: //富文本框
                        case 4: //弹出选择（外键）
                            value = valueControl.textbox('getValue');
                            break;
                        case 1: //checkbox
                            value = valueControl.hasClass('combobox-f') ? (valueControl.combobox('getValue') == '1' ? true : false) : (valueControl.val() == "1" || valueControl.attr('checked') == 'checked' ? true : false);
                            break;
                        case 10: //日期输入框
                            value = valueControl.datebox('getValue');
                            break;
                        case 6: //数值输入框
                        case 7: //整形数值输入框
                            value = valueControl.numberbox('getValue');
                            break;
                        case 11: //日期时间输入框
                            value = valueControl.datetimebox('getValue');
                            break;
                        case 3: //下拉列表框
                            value = valueControl.combobox('getValue');
                            break;
                        case 5: //下拉树
                            value = valueControl.combotree('getValue');
                            break;
                        case 4: //弹出选择（外键）
                            break;
                    }
                    if (fieldObj.ControlType != 1 && value == '')
                        value = null;
                    if (value != null) {
                        var method = methodControl.combobox('getValue');
                        var orgroup = orGroupControl.combobox('getValue');
                        conditions.push({ Field: fieldName, Method: method, Value: value, OrGroup: orgroup });
                    }
                }
            });
            return conditions;
        }
    });
})(jQuery);
var page = GetLocalQueryString("page");
var searchParams = null; //当前搜索条件

$(function () {
    //列表左边树的top、left、bottom边框去掉
    $("#region_west").css('border-left-width', '0px')
                     .css('border-bottom-width', '0px');
    $("#region_west").prev().css('border-top-width', '0px')
                     .css('border-left-width', '0px');
    var color = null;
    try {
        color = topWin.GetBorderColor();
    } catch (err) { }
    if (color != null) {
        $("#regon_main .datagrid").css('border-right-color', color)
                                  .css('border-right-style', 'solid')
                                  .css('border-right-width', '1px');
        if ($("div[id^='region_south']").length > 0) { //有明细或附属模块
            $("#regon_main .datagrid").css('border-bottom-color', color)
                                      .css('border-bottom-style', 'solid')
                                      .css('border-bottom-width', '1px')
                                      .css('border-left-color', color)
                                      .css('border-left-style', 'solid')
                                      .css('border-left-width', '1px');
            $('#detailTabs').css('border-right-color', color)
                            .css('border-right-style', 'solid')
                            .css('border-right-width', '1px')
                            .css('border-top-color', color)
                            .css('border-top-style', 'solid')
                            .css('border-top-width', '1px');
            //附属模块tab-head宽度设置
            var w = $('#detailTabs').width();
            $('#detailTabs .tabs-header').each(function (i, item) {
                $(item).width(w);
            });
        }
    }
    $('#div_sampleSearch span.textbox-addon').attr('title', '点击搜索');
    $('.pagination .pagination-page-list').parent().attr('title', '每页显示记录数');
    $('.pagination .pagination-first').parent().parent().attr('title', '第一页');
    $('.pagination .pagination-prev').parent().parent().attr('title', '上一页');
    $('.pagination .pagination-next').parent().parent().attr('title', '下一页');
    $('.pagination .pagination-last').parent().parent().attr('title', '最后一页');
    //搜索框智能提示绑定
    //绑定简单搜索框智能提示函数
    var bindSimpleSearchAutoComplete = function (txtSearchDom, tempFieldName) {
        try {
            var domId = $(txtSearchDom).attr("id");
            var tempModuleId = $(txtSearchDom).attr("moduleId");
            var fieldName = tempFieldName ? tempFieldName : $(txtSearchDom).searchbox("getName"); //搜索字段
            var tempDom = $(txtSearchDom).searchbox("textbox");
            tempDom.removeAttr('bindAuto');
            FieldBindAutoCompleted(tempDom, tempModuleId, fieldName, null, function (dom, item) {
                var text = item["f_Name"];
                var searchDom = $("#" + domId);
                searchDom.searchbox("setValue", text);
                SimpleSearch(searchDom, text, fieldName);
            });
        } catch (e) { }
    };
    //重新选中字段后重新绑定
    $("div[id^='search_mm']").each(function (i, item) {
        var txtSearchDomId = $(item).attr('searchTxtId');
        if (txtSearchDomId == 'txtSearch') {
            var txtSearchDom = $('#' + txtSearchDomId);
            $(item).unbind('afterOnClick').bind('afterOnClick', function (event, mm) {
                bindSimpleSearchAutoComplete(txtSearchDom, mm.name);
            });
        }
    });
    //延时绑定搜索框智能提示
    setTimeout(function () {
        if ($('#txtSearch').length > 0) {
            bindSimpleSearchAutoComplete($('#txtSearch')); //简单搜索绑定智能提示
        }
        var moduleId = GetLocalQueryString("moduleId"); //模块Id
        var moduleName = decodeURIComponent(GetLocalQueryString("moduleName")); //模块名称
        var moduleIdOrModuleName = moduleId.length > 0 ? moduleId : moduleName;
        $("#searchform .easyui-textbox[foreignField='1'],#searchform .easyui-textbox[noforein='1']").each(function (i, domItem) {
            var tempDom = $(domItem).next('span').find('input.textbox-text');
            var fieldName = $(domItem).attr('id'); //搜索字段
            var displayTemplete = null;
            //自定义自动完成显示模板
            var foreginModuleName = $(domItem).attr('foreignModuleName');
            if (typeof (OverGetAutoCompletedDisplayTemplete) == "function") {
                displayTemplete = OverGetAutoCompletedDisplayTemplete(fieldName, foreginModuleName);
            }
            FieldBindAutoCompleted(tempDom, moduleIdOrModuleName, fieldName, displayTemplete, function (dom, item, fieldName, paramObj) {
                var text = item["f_Name"];
                var value = item["Id"];
                if (foreginModuleName) {
                    paramObj.attr('v', value);
                    paramObj.attr('value', value);
                    paramObj.textbox("setValue", value);
                    paramObj.textbox("setText", text);
                }
                else {
                    paramObj.textbox("setValue", text);
                }
                ComplexSearch($('#btn_search')); //自动完成后自动搜索
            }, $(domItem), null, foreginModuleName);
        });
    }, 200);
    if (page == 'grid') {
        $('#region_south').addClass('noprint'); //主网格页面不打印明细部分
        $("div[id^='grid_toolbar_']").addClass('noprint'); //打印时隐藏列表操作按钮
        $('.pagination').addClass('noprint'); //打印时隐藏分页工具栏
        $('.pagination table').addClass('noprint');
    }
    if (IsNeedParsePage()) {
        ParserLayout();
    }
});

/*----------------------搜索相关----------------------------*/
//简单搜索
//obj:当前搜索dom对象
//value:输入值
//name:搜索字段名
function SimpleSearch(obj, value, name) {
    var treeDom = $("#gridTree"); //左侧树dom对象
    var o = {}; //搜索对象
    if ($(obj).attr('reget') == '1') { //需要重新获取值标识
        var textbox = $(obj).searchbox('textbox');
        value = textbox.val();
        if (value.indexOf('请输入搜索值') > -1)
            value = '';
    }
    if (value) {
        o[name] = value;
    }
    var s = JSON.stringify(o);
    var params = { q: s };
    if (treeDom && treeDom.length > 0) { //存在左侧树
        //树结点字段
        var treeField = treeDom.attr("treeField");
        var node = treeDom.tree("getSelected");
        if (node && node.id != -1 && node.id != GuidEmpty) {
            var oo = {};
            oo[treeField] = node.id;
            params["condition"] = JSON.stringify(oo);
        }
    }
    searchParams = params;
    var gridId = $(obj).attr("gridId");
    $("#" + gridId).datagrid("reload", params);
}

//复杂搜索
//obj:当前搜索dom对象
function ComplexSearch(obj) {
    var form = $("#searchform");
    var data = form.fixedSerializeArrayFix();
    var o = {};
    $.each(data, function (index) {
        var name = this['name'];
        var value = this["value"];
        if (!o[name] && value) {
            if ($('#searchform #' + name).hasClass('easyui-daterange')) {
                var tmpToken = value.split(' 至 ');
                o[name] = tmpToken[0];
                o[name + '_End'] = tmpToken[1];
            }
            else {
                o[name] = value;
            }
        }
    });
    var s = JSON.stringify(o);
    var params = { q: s };
    var treeDom = $("#gridTree"); //左侧树dom对象
    if (treeDom && treeDom.length > 0) { //存在左侧树
        //树结点字段
        var treeField = treeDom.attr("treeField");
        var node = treeDom.tree("getSelected");
        if (node && node.id != -1 && node.id != GuidEmpty) {
            var oo = {};
            oo[treeField] = node.id;
            params["condition"] = JSON.stringify(oo);
        }
    }
    searchParams = params;
    var gridId = $(obj).attr("gridId");
    $("#" + gridId).datagrid("reload", params);
}

//切换搜索方式，简单搜索与复杂搜索间切换
//obj:当前搜索dom对象
function ChangeSearchStyle(obj) {
    var sampleSearchObj = $('#div_sampleSearch');
    var complexSearchObj = $('#div_complexSearch');
    var btnSearchObj = $('#btn_search');
    var btnClear = $('#btn_clear');
    if (sampleSearchObj.css('display') == 'none') { //切换到简单搜索
        sampleSearchObj.show();
        complexSearchObj.hide();
        btnSearchObj.hide();
    }
    else { //切换到复杂搜索
        sampleSearchObj.hide();
        complexSearchObj.show();
        btnSearchObj.show();
    }
    CorrectGridHeight(false); //纠正网格高度
}

//高级搜索
function AdvanceSearch(obj) {
    var toolbar = [{
        text: "搜 索",
        iconCls: "eu-icon-search",
        handler: function (e) {
            var searchParam = topWin.GetCurrentDialogFrame()[0].contentWindow.GetSearchParam();
            var s = JSON.stringify(searchParam);
            var params = { q: s };
            searchParams = params;
            var gridId = $(obj).attr("gridId");
            $("#" + gridId).datagrid("reload", params);
            topWin.CloseDialog();
        }
    }, {
        text: "清 除",
        iconCls: "eu-icon-clear",
        handler: function (e) {
            topWin.GetCurrentDialogFrame()[0].contentWindow.ClearSearchValue();
        }
    }, {
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var moduleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var viewId = $(obj).attr("viewId"); //视图Id
    var url = '/Page/AdvanceSearch.html?moduleId=' + moduleId + '&viewId=' + viewId;
    if (isNfm) {
        url += "&nfm=1"
    }
    topWin.OpenDialog('高级搜索－' + moduleName, url, toolbar, 400, 450, 'eu-icon-search');
}

//开启或关闭行过滤搜索
function OpenOrCloseRowFilterSearch(obj) {
    var moduleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var viewId = $(obj).attr("viewId"); //视图Id
    var gridId = $(obj).attr("gridId"); //网格domId
    var isRf = $(obj).attr("isrf");
    if (isRf == undefined || isRf == null || isRf != 1) {
        isRf = 1;
    }
    else {
        isRf = 2;
    }
    ChangeGridView(moduleId, gridId, viewId, isRf, function () {
        var btnFilterSearchObj = $('#btn_filterSearch');
        if (isRf == 1) {
            btnFilterSearchObj.attr('title', '关闭行过滤搜索');
            btnFilterSearchObj.find('span.l-btn-icon').removeClass('eu-icon-filter').addClass('eu-icon-tip');
        }
        else {
            btnFilterSearchObj.attr('title', '启用行过滤搜索');
            btnFilterSearchObj.find('span.l-btn-icon').removeClass('eu-icon-tip').addClass('eu-icon-filter');
        }
        btnFilterSearchObj.attr('isrf', isRf);
    })
}

//清除搜索内容
function ClearSearch(obj) {
    $("#searchform .easyui-textbox").each(function (i, item) {
        $(item).textbox('clear');
        $(item).attr('value', '').attr('v', '');
        $(item).next('span').find('input.textbox-value').attr('value', '');
    });
    $("#searchform .easyui-combobox").each(function (i, item) {
        $(item).combobox('clear');
    });
    $("#searchform .easyui-combotree").each(function (i, item) {
        $(item).combotree('clear');
    });
    $("#searchform .easyui-daterange").each(function (i, item) {
        $(item).val('');
    });
    var searchObj = $(obj).parent().find("input[id^='txtSearch']").eq(0);
    searchObj.searchbox('clear');
    searchObj.searchbox('textbox').val('');
}

//当工具栏搜索框被挤到下方时处理，针对主网格，纠正网格高度
//isLoadSuccess:是否数据加载成功后
function CorrectGridHeight(isLoadSuccess) {
    //当工具栏搜索框被挤到下方时处理
    var moduleId = $('#regon_main').attr('moduleId');
    var gridToolbar = $('#grid_toolbar_' + moduleId);
    var btnsDiv = gridToolbar.find("div[name='btns']");
    var searchDiv = gridToolbar.find("div[id^='div_search']");
    var childBarWidth = 45;
    if (btnsDiv.length > 0)
        childBarWidth += btnsDiv.width();
    if (searchDiv.length > 0)
        childBarWidth += searchDiv.width();
    var divView = $('#regon_main div.datagrid-view').eq(0);
    var divBody = divView.find('div.datagrid-view2 div.datagrid-body');
    if (!isLoadSuccess) {
        var h = divView.attr('h');
        var hh = divBody.attr('h');
        if (childBarWidth > gridToolbar.width()) {
            var dvH = parseInt(h);
            var dbH = parseInt(hh);
            divView.height(dvH - 26);
            divBody.height(dbH - 26);
        }
        else {
            divView.height(parseInt(h));
            divBody.height(parseInt(hh));
        }
    }
    else { //网格数据加载完成后
        var dvH = parseInt(divView.height());
        var dbH = parseInt(divBody.height());
        if (dvH <= 25 || dbH <= 0) {
            var tbObj = divView.find('.easyui-datagrid').eq(0);
            var th = parseInt(tbObj.attr('h'));
            if (th > 25) {
                th -= 20;
                var gridId = tbObj.attr('id');
                ResizeGrid(gridId, null, th);
                dvH = parseInt(divView.height());
                dbH = parseInt(divBody.height());
            }
        }
        divView.attr('h', dvH);
        divBody.attr('h', dbH);
        if (childBarWidth > gridToolbar.width()) {
            divView.height(dvH - 26);
            divBody.height(dbH - 26);
        }
    }
}
/*-----------------------------------------------------------------*/

//列表视图设置
function GridSet(obj) {
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            topWin.CloseDialog();
            var moduleId = $(obj).attr("moduleId");
            var gridId = $(obj).attr("gridId");
            var viewObj = topWin.GetCurrentDialogFrame()[0].contentWindow.GetSelectGridView();
            var treeField = $("#btn_gridSet" + moduleId).attr("treeField");
            if (viewObj.TreeField == treeField) { //没有产生树显示字段变化时只切换列头
                ChangeGridView(moduleId, gridId, viewObj.ViewId); //切换视图
            }
            else { //树显示字段变化需要刷新整个页面
                var url = "/Page/Grid.html?moduleId=" + moduleId + "&viewId=" + viewObj.ViewId + "&r=" + Math.random();
                var iframe = $("iframe", GetSelectedTab());
                iframe.attr("src", url);
            }
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var moduleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var viewId = $(obj).attr("viewId");
    var url = '/Page/GridSet.html?moduleId=' + moduleId + '&viewId=' + viewId;
    if (isNfm) {
        url += "&nfm=1"
    }
    url += '&r=' + Math.random();
    topWin.OpenDialog('选择视图－' + moduleName, url, toolbar, 415, 250, 'eu-icon-grid');
}

//附属模块设置
//obj:按钮dom对象
function AttachModuleSet(obj) {
    var moduleId = $(obj).attr('moduleId');
    var toolbar = [{
        id: 'btnOk',
        text: '确 定',
        iconCls: 'eu-icon-ok',
        handler: function (e) {
            var attachModuleInfos = topWin.GetCurrentDialogFrame()[0].contentWindow.GetEnabledAttachModules();
            var json = escape(JSON.stringify(attachModuleInfos));
            $.ajax({
                type: 'post',
                url: '/' + CommonController.Async_System_Controller + '/SaveUserAttachModuleSet.html',
                data: { moduleId: moduleId, attachModuleInfo: json },
                dataType: 'json',
                beforeSend: function () {
                    topWin.OpenWaitDialog('数据保存中...');
                },
                success: function (result) {
                    topWin.CloseWaitDialog();
                    if (result && result.Success) {
                        topWin.CloseDialog();
                        //window.location.reload();
                    }
                    else {
                        topWin.ShowAlertMsg('提示', result.Message, 'info');
                    }
                },
                error: function (err) {
                    topWin.CloseWaitDialog();
                    topWin.ShowAlertMsg('提示', '附属模块显示设置保存失败，服务端异常！', 'error');
                }
            });
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: 'eu-icon-close',
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var url = '/Page/AttachModuleSet.html?moduleId=' + moduleId;
    if (isNfm) {
        url += "&nfm=1"
    }
    url += '&r=' + Math.random();
    topWin.OpenDialog('附属模块显示设置', url, toolbar, 600, 500, 'eu-icon-calendar');
}

//刷新网格
//gridId:网格Id
function RefreshGrid(gridId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    $("#" + tempGridId).datagrid("reload");
}

//重新加载数据
//gridId:网格domId
//url:网格数据加载url
function ReloadData(gridId, url) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridDom = $("#" + tempGridId)
    var options = gridDom.datagrid("options");
    options.url = url;
    gridDom.datagrid(options);
}

//获取行数据
//gridId:网格domId
//rowIndex:行号
function GetRow(gridId, rowIndex) {
    var gridObj = gridId ? $("#" + gridId) : $("#mainGrid");
    var tempRows = gridObj.datagrid("getRows");
    for (var i = 0; i < tempRows.length; i++) {
        var tempRow = tempRows[i];
        var tempIndex = gridObj.datagrid("getRowIndex", tempRow);
        if (tempIndex == rowIndex)
            return tempRow;
    }
    return null;
}

//获取选择的单条记录，针对主网格
//gridId:网格domId
function GetSelectRow(gridId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var row = $("#" + tempGridId).datagrid("getSelected");
    return row;
}

//获取选择的单条记录
//gridIdPrefix:网格domId前缀
function GetSelectRowByGridIdPrefix(gridIdPrefix) {
    var row = $("table[id^='" + gridIdPrefix + "']").datagrid("getSelected");
    return row;
}

//获取选择的多条记录
//gridIdPrefix:网格domId前缀
function GetSelectRowsByGridIdPrefix(gridIdPrefix) {
    var rows = $("table[id^='" + gridIdPrefix + "']").datagrid("getSelections");
    return rows;
}

//获取选中行的TitleKey的值
//gridId:网格domId
function GetSelectRowTitleKeyValue(gridId) {
    var regonObj = gridId && gridId != 'mainGrid' ? $("div[id^='regon_'][id!='regon_main']") : $("#regon_main");
    var titleKey = regonObj.attr("titleKey");
    if (titleKey) {
        var tableName = regonObj.attr("tableName");
        var row = GetSelectRow(gridId);
        if (row) {
            return { name: titleKey, value: row[titleKey], foreignFieldName: tableName + "Id", recordId: row["Id"] };
            return row[titleKey];
        }
    }
    return null;
}

//获取所有选择的记录，针对主网格
//gridId:网格domId
function GetSelectRows(gridId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var rows = $("#" + tempGridId).datagrid("getSelections");
    return rows;
}

//获取所有选择的记录
//gridIdPrefix:网格domId前缀
function GetSelectRowsByGridIdPrefix(gridIdPrefix) {
    var rows = $("table[id^='" + gridIdPrefix + "']").datagrid("getSelections");
    return rows;
}

//获取当前页所有行
//gridId:网格domId
function GetCurrentRows(gridId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var rows = $("#" + tempGridId).datagrid("getRows");
    return rows;
}

//获取网格的行id前缀
//如：datagrid-row-r2-1-rowIndex中的‘datagrid-row-r2-1-’
//parentDom:网格所属父dom对象
function GetGridRowIdPrefix(parentDom) {
    var tr = parentDom && parentDom.length > 0 ?
        $("div.datagrid-view2 div.datagrid-body tr.datagrid-row", parentDom).eq(0) :
        $("div.datagrid-view2 div.datagrid-body tr.datagrid-row").eq(0);
    var tempId = tr.attr("id");
    var startIndex = tempId.lastIndexOf("-");
    return tempId.substr(0, startIndex + 1);
}

//获取网格系统行前缀，系统行包含checkbox,展开图标等
//如：datagrid-row-r1-1-rowIndex中的‘datagrid-row-r1-1-’
//parentDom:网格所属父dom对象
function GetGridSysRowIdPrefix(parentDom) {
    var tr = parentDom && parentDom.length > 0 ?
        $("div.datagrid-view1 div.datagrid-body tr.datagrid-row", parentDom).eq(0) :
        $("div.datagrid-view1 div.datagrid-body tr.datagrid-row").eq(0);
    var tempId = tr.attr("id");
    var startIndex = tempId.lastIndexOf("-");
    return tempId.substr(0, startIndex + 1);
}

//获取网格选择行的字段文本显示值
function GetSelectRowDisplayValue(gridId) {
    var rowIndex = GetSelectRowIndex(gridId);
    var rowIdPrefix = GetGridRowIdPrefix();
    var tdDom = $("#" + rowIdPrefix + rowIndex + " td");
    var row = {};
    $.each(tdDom, function () {
        var fieldName = $(this).attr("field");
        var textValue = null;
        if (fieldName) {
            var span = $("span", $(this));
            if (span.length == 0) {
                var div = $("div", $(this));
                textValue = div.text();
            }
            else {
                textValue = span.text();
            }
            row[fieldName] = textValue;
        }
    });
    var rowIdSysPrefix = GetGridSysRowIdPrefix();
    var tdSysDom = $("#" + rowIdSysPrefix + rowIndex + " td");
    $.each(tdSysDom, function () {
        var fieldName = $(this).attr("field");
        var textValue = null;
        if (fieldName) {
            var span = $("span", $(this));
            if (span.length == 0) {
                var div = $("div", $(this));
                textValue = div.text();
            }
            else {
                textValue = span.text();
            }
            row[fieldName] = textValue;
        }
    });
    return row;
}

//获取选中行的行号
//gridId:网格domId
function GetSelectRowIndex(gridId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    var row = gridObj.datagrid("getSelected");
    var rowIndex = gridObj.datagrid("getRowIndex", row);
    return rowIndex;
}

//根据记录ID获取行对象
//gridId:网格domId
//recordId:记录Id
function GetRowByRecordId(gridId, recordId) {
    if (!recordId) return null;
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    var rows = gridObj.datagrid("getRows");
    for (var i = 0; i < rows.length; i++) {
        var tempRow = rows[i];
        if (recordId == tempRow["Id"]) {
            return tempRow;
        }
    }
    return null;
}

//根据记录Id取行号
//gridId:网格domId
//recordId:记录Id
function GetRowIndexByRecordId(gridId, recordId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    var rows = gridObj.datagrid("getRows");
    for (var i = 0; i < rows.length; i++) {
        var tempRow = rows[i];
        if (recordId == tempRow["Id"]) {
            var rowIndex = gridObj.datagrid("getRowIndex", tempRow);
            return rowIndex;
        }
    }
    return -1;
}

//获取行号
//gridId:网格domId
//row:行对象
function GetRowIndexByRow(gridId, row) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    var rowIndex = gridObj.datagrid("getRowIndex", row);
    return rowIndex;
}

//获取网格编辑器中字段的控件
//rowIndex:行号
//fieldName:字段名
//parentDom:网格所属父dom对象
function GetGridEditorControl(rowIndex, fieldName, parentDom) {
    var idPrefix = GetGridRowIdPrefix(parentDom);
    var dom = $("#" + idPrefix + rowIndex + " td[field='" + fieldName + "']");
    var control = dom.find('input.datagrid-editable-input');
    if (control.length == 0) {
        control = dom.find("td input");
    }
    return control;
}

//获取网格编辑器中字段的控件
//gridId:网格Id
//rowIndex:行号
//fieldName:字段名
function GetGridEditorControl2(gridId, rowIndex, fieldName) {
    try {
        var tmpGridId = gridId ? gridId : 'mainGrid';
        var obj = $($('#' + tmpGridId).datagrid('getEditor', { index: rowIndex, field: fieldName }).target);
        return obj;
    } catch (e) { }
    return null;
}

//获取网格编辑器中td
//rowIndex:行号
//fieldName:字段名
//parentDom:网格所属父dom对象
function GetGridEditorTdCell(rowIndex, fieldName, parentDom) {
    var idPrefix = GetGridRowIdPrefix(parentDom);
    var control = $("#" + idPrefix + rowIndex + " td[field='" + fieldName + "']");
    return control;
}

//获取网格系统编辑器中td，包含checkbox,展开图标等
//rowIndex:行号
//fieldName:字段名
//parentDom:网格所属父dom对象
function GetGridSysEditorTdCell(rowIndex, fieldName, parentDom) {
    var idPrefix = GetGridSysRowIdPrefix(parentDom);
    var control = $("#" + idPrefix + rowIndex + " td[field='" + fieldName + "']");
    return control;
}

//获取网格列头集合
//gridId:网格ID
function GetGridColumns(gridId) {
    var tmpGridId = gridId ? gridId : 'mainGrid';
    var options = $('#' + tmpGridId).datagrid('options');
    if (options && options.columns) {
        return options.columns[0];
    }
    return null;
}

//获取网格字段
//gridId:网格DomId
//fieldName:字段名称
function GetGridColumn(gridId, fieldName) {
    var columns = GetGridColumns(gridId);
    if (columns != null && columns.length > 0) {
        for (var i = 0; i < columns.length; i++) {
            var field = columns[i];
            if (field.field == fieldName)
                return field;
        }
    }
    return null;
}

//启用单元格字段编辑
//gridId:网格domId
function EnableCellEdit(gridId) {
    var tempGridId = gridId && gridId.length > 0 ? gridId : 'mainGrid';
    $("#" + tempGridId).datagrid('enableCellEditing').datagrid('gotoCell', {
        index: 0,
        field: 'Id'
    });
}

//获取当前主网格选中记录的titlekey字段值，主网格下方明细或附属模块新增时用到
function GetTitleKeyValue() {
    var row = GetSelectRow();
    var forignFieldName = $('#regon_main').attr('tableName') + "Id";
    return { foreignFieldName: forignFieldName, recordId: row["Id"] };
}

//新增，兼容明细新增
//copyId:复制时被复制记录Id
function Add(obj, copyId) {
    var tempModuleId = $(obj).attr("moduleId");
    var gridId = $(obj).attr('gridId');
    var moduleName = $(obj).attr("moduleName");
    var moduleDisplay = $(obj).attr("moduleDisplay");
    if (!moduleDisplay) moduleDisplay = moduleName;
    var title = "新增" + moduleDisplay;
    var formUrl = $(obj).attr('formUrl'); //自定义表单页面URL
    if (formUrl == undefined || formUrl == null || formUrl.length == 0) { //通用新增页面
        var editMode = parseInt($(obj).attr("editMode")); //编辑模式
        var editPageUrl = "/Page/EditForm.html?page=add&moduleId=" + tempModuleId;
        if (moduleName != undefined && moduleName != null && moduleName.length > 0)
            editPageUrl += "&moduleName=" + moduleName;
        var formId = $(obj).attr('formId'); //表单ID
        if (formId != undefined && formId != null && formId.length > 0)
            editPageUrl += "&formId=" + formId;
        editPageUrl += "&mode=" + editMode;
        editPageUrl += "&fp=grid";
        if (copyId) { //有复制ID
            editPageUrl += "&mode=" + editMode + "&copyId=" + copyId;
            if (gridId == 'mainGrid' && $(obj).attr('rsf') == '1') { //重新发起标识
                editPageUrl += "&rsf=1";
            }
        }
        if (editMode == 2 && (page == "add" || page == "edit" || page == "view" ||
            (page == "grid" && $(obj).attr('gt') == '5'))) { //主从编辑页面或附属模块添加标识
            var parentMode = GetLocalQueryString("mode"); //父页面编辑模式
            editPageUrl += "&pmode=" + parentMode;
            editPageUrl += "&ff=1"; //标识来自表单页面
            if (page == "grid" && $(obj).attr('gt') == '5') { //从附属模块网格中单击新增时
                var row = GetSelectRow(); //获取当前主网格选中记录
                if (row == null) {
                    var mainModuleName = $('#regon_main').attr('moduleName');
                    topWin.ShowAlertMsg("提示", "请在主模块【" + mainModuleName + "】列表中选择一条记录！", "info"); //弹出提示信息
                    return;
                }
                editPageUrl += "&fg=1"; //标识来自附属网格中
            }
        }
        var currTabIndex = GetSelectTabIndex(); //当前grid网格页面的tabindex
        if (currTabIndex)
            editPageUrl += "&tb=" + currTabIndex;
        if (isNfm) {
            editPageUrl += '&nfm=1';
        }
        editPageUrl += "&r=" + Math.random();
        //url重写
        if (typeof (OverGetFormUrl) == "function") {
            var tempUrl = OverGetFormUrl('add', editPageUrl, moduleName, tempModuleId, obj);
            if (tempUrl) {
                editPageUrl = tempUrl;
            }
        }
        var gridObj = $("#" + gridId);
        //执行新增方法
        var ExecuteMethod = function () {
            //有自定义新增方法则先调用自定义否则调用通用
            if (typeof (OverAdd) == "function") {
                OverAdd(obj);
                return;
            }
            if (editMode == 2 || editMode == 4) { //弹出框编辑模式或网格编辑表单
                var editWidth = parseInt($(obj).attr("editWidth"));
                var editHeight = parseInt($(obj).attr("editHeight"));
                editWidth += 40; //加上padding相关
                editHeight += 35 + 50; //标题栏高度和按钮栏高度
                //加载弹出框操作按钮
                var params = { moduleId: tempModuleId, formType: 0, editMode: editMode, page: page };
                if (isNfm) {
                    params.nfm = 1;
                }
                var ajaxUrl = '/' + CommonController.Async_System_Controller + '/LoadFormBtns.html';
                if (isNfm) {
                    ajaxUrl = host + ajaxUrl;
                }
                ExecuteCommonAjax(ajaxUrl, params, function (result) {
                    if (result && result.length > 0) {
                        var toolbar = result;
                        for (var i = 0; i < toolbar.length; i++) {
                            var tempHandler = toolbar[i].handler;
                            if (typeof (tempHandler) == 'string') {
                                toolbar[i].handler = eval('(' + tempHandler + ')');
                            }
                        }
                        topWin.OpenDialog(title, editPageUrl, toolbar, editWidth, editHeight, null, function (dialogDivId) {
                            setTimeout(function () {
                                var divBtnPar = topWin.$("#" + dialogDivId).parent();
                                var dgBtn = topWin.$("div.dialog-button a", divBtnPar);
                                if (page != "view") {
                                    dgBtn.attr("detail", toolbar[0].detail);
                                }
                                dgBtn.attr("editMode", editMode);
                                dgBtn.attr("moduleId", tempModuleId);
                                dgBtn.attr("moduleName", moduleName);
                                dgBtn.attr("gridId", gridId);
                            }, 50);
                        });
                    }
                }, false, true);
            }
            else if (editMode == 1) { //tab编辑模式
                AddTab(null, title, editPageUrl);
            }
            else if (editMode == 3) { //列表行编辑模式
                AddRow(gridId, tempModuleId, moduleName);
                InitRowOpBtns(gridId);
                if (page != 'add' && page != 'edit' && $("a[id^='rowOkBtn_']").length == 0) {
                    $(obj).hide();
                    $(obj).parent().append("<a id=\"btnRowSave\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-save',plain:true\">保存</a><a id=\"btnRowCancel\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-p2-icon-cancel',plain:true\">取消</a>");
                    $('#btnRowSave').click(function () {
                        RowEditSave(tempModuleId, moduleName, null, gridId, function (result) {
                            if (result && result.Success) {
                                $('#btnRowSave').remove();
                                $('#btnRowCancel').remove();
                                $(obj).show();
                            }
                        });
                    });
                    $('#btnRowCancel').click(function () {
                        RowEditCancel(null, gridId, function () {
                            $('#btnRowSave').remove();
                            $('#btnRowCancel').remove();
                            $(obj).show();
                        });
                    });
                    var gridToolbarId = $(obj).parent().parent().attr('id');
                    ParserLayout(gridToolbarId);
                }
            }
        }
        if (!copyId) { //新增
            //先进行客户端和服务端验证
            var otherParams = null;
            if ((page == 'grid' || page == 'view') && gridId != 'mainGrid') {
                var mainModuleId = GetLocalQueryString("moduleId"); //主模块moduleId
                var mainRecordId = page == 'grid' ? GetSelectRow('mainGrid').Id : GetLocalQueryString("id");
                otherParams = mainModuleId + ',' + mainRecordId;
            }
            GridOperateVerify(moduleName, "新增", null, function (errMsg) {
                if (errMsg && errMsg.length > 0) {
                    topWin.ShowAlertMsg("提示", errMsg, "info"); //弹出验证提示信息
                }
                else {
                    //调用通用方法
                    ExecuteMethod(); //执行新增
                }
            }, otherParams);
        }
        else { //复制
            ExecuteMethod(); //执行新增
        }
    }
    else { //自定义新增表单
        if (!copyId) { //新增
            //先进行客户端和服务端验证
            var otherParams = null;
            if ((page == 'grid' || page == 'view') && gridId != 'mainGrid') {
                var mainModuleId = GetLocalQueryString("moduleId"); //主模块moduleId
                var mainRecordId = page == 'grid' ? GetSelectRow('mainGrid').Id : GetLocalQueryString("id");
                otherParams = mainModuleId + ',' + mainRecordId;
            }
            GridOperateVerify(moduleName, "新增", null, function (errMsg) {
                if (errMsg && errMsg.length > 0) {
                    topWin.ShowAlertMsg("提示", errMsg, "info"); //弹出验证提示信息
                }
                else {
                    AddTab(null, title, formUrl); //执行新增
                }
            }, otherParams);
        }
        else { //复制
            if (formUrl.indexOf('?') > -1) {
                formUrl += '&copyId=' + copyId;
            }
            else {
                formUrl += '?copyId=' + copyId;
            }
            AddTab(null, title, formUrl); //执行新增
        }
    }
}

//编辑，兼容明细编辑
function Edit(obj) {
    var tempModuleId = $(obj).attr("moduleId");
    var gridId = $(obj).attr('gridId');
    var gridObj = $("#" + gridId);
    var editMode = parseInt($(obj).attr("editMode")); //编辑模式
    var selectId = $(obj).attr("recordId"); //要编辑的记录Id
    var row = GetSelectRow(gridId); //获取选中行
    if (!row) { //没有选中行，从当前按钮中找对应的记录Id来得到选择行
        if (selectId) {
            var rows = GetCurrentRows(gridId);
            for (var i = 0; i < rows.length; i++) {
                var tempRow = rows[i];
                if (selectId == tempRow["Id"]) {
                    row = tempRow;
                    break;
                }
            }
        }
    }
    if (!row) {
        topWin.ShowMsg("提示", "请选择一条记录！", null, null, 1);
        return;
    }
    if (selectId == undefined || !selectId)
        selectId = row["Id"];
    var moduleName = $(obj).attr("moduleName");
    var moduleDisplay = $(obj).attr("moduleDisplay");
    if (!moduleDisplay) moduleDisplay = moduleName;
    var titleKey = $(obj).attr("titleKey"); //标记字段名
    var title = "编辑" + moduleDisplay;
    if (titleKey && row[titleKey]) {
        title = title + "－" + row[titleKey];
    }
    var formUrl = $(obj).attr('formUrl'); //自定义表单页面URL
    if (formUrl == undefined || formUrl == null || formUrl.length == 0) { //通用新增页面
        var editPageUrl = "/Page/EditForm.html?page=edit&moduleId=" + tempModuleId;
        if (moduleName != undefined && moduleName != null && moduleName.length > 0)
            editPageUrl += "&moduleName=" + moduleName;
        var formId = $(obj).attr('formId'); //表单ID
        if (formId != undefined && formId != null && formId.length > 0)
            editPageUrl += "&formId=" + formId;
        editPageUrl += "&tip=0";
        editPageUrl += "&mode=" + editMode;
        editPageUrl += "&fp=grid";
        if (editMode == 2 && (page == "add" || page == "edit" || page == "view")) { //主从编辑页面添加标识
            var parentMode = GetLocalQueryString("mode"); //父页面编辑模式
            editPageUrl += "&pmode=" + parentMode;
            if (page == "add" || page == "edit") {
                editPageUrl += "&ff=1"; //标识来自表单页面
            }
            else {
                editPageUrl += "&id=" + selectId;
            }
        }
        else {
            editPageUrl += "&id=" + selectId;
        }
        var todoId = $(obj).attr('todoId');
        if (todoId && todoId.length > 0) {
            editPageUrl += "&todoId=" + todoId;
        }
        var currTabIndex = GetSelectTabIndex(); //当前grid网格页面的tabindex
        if (currTabIndex)
            editPageUrl += "&tb=" + currTabIndex;
        if (isNfm) {
            editPageUrl += '&nfm=1';
        }
        editPageUrl += "&r=" + Math.random();
        //url重写
        if (typeof (OverGetFormUrl) == "function") {
            var tempUrl = OverGetFormUrl('edit', editPageUrl, moduleName, tempModuleId, obj);
            if (tempUrl) {
                editPageUrl = tempUrl;
            }
        }
        //执行编辑方法
        var ExecuteMethod = function () {
            //有自定义编辑方法则先调用自定义否则调用通用
            if (typeof (OverEdit) == "function") {
                OverEdit(obj);
                return;
            }
            if (editMode == 2) { //弹出框编辑模式
                var editWidth = parseInt($(obj).attr("editWidth"));
                var editHeight = parseInt($(obj).attr("editHeight"));
                editWidth += 40; //加上padding相关
                editHeight += 35 + 50; //标题栏高度和按钮栏高度
                //加载弹出框操作按钮
                var params = { moduleId: tempModuleId, id: selectId, formType: 0, editMode: editMode, page: page };
                if (todoId && todoId.length > 0) {
                    params.todoId = todoId;
                }
                if (isNfm) {
                    params.nfm = 1;
                }
                var ajaxUrl = '/' + CommonController.Async_System_Controller + '/LoadFormBtns.html';
                if (isNfm) {
                    ajaxUrl = host + ajaxUrl;
                }
                ExecuteCommonAjax(ajaxUrl, params, function (result) {
                    if (result && result.length > 0) {
                        var toolbar = result;
                        for (var i = 0; i < toolbar.length; i++) {
                            var tempHandler = toolbar[i].handler;
                            if (typeof (tempHandler) == 'string') {
                                toolbar[i].handler = eval('(' + tempHandler + ')');
                            }
                        }
                        topWin.OpenDialog(title, editPageUrl, toolbar, editWidth, editHeight, null, function (dialogDivId) {
                            setTimeout(function () {
                                var divBtnPar = topWin.$("#" + dialogDivId).parent();
                                var dgBtn = topWin.$("div.dialog-button a", divBtnPar);
                                if (page != "view") {
                                    dgBtn.attr("detail", toolbar[0].detail);
                                }
                                dgBtn.attr("editMode", editMode);
                                dgBtn.attr("moduleId", tempModuleId);
                                dgBtn.attr("moduleName", moduleName);
                                dgBtn.attr("gridId", gridId);
                            }, 50);
                        });
                    }
                }, false, true);
            }
            else if (editMode == 1) { //tab编辑模式
                AddTab(null, title, editPageUrl);
            }
            else if (editMode == 3) { //列表行编辑模式
                var rowIndex = gridObj.datagrid("getRowIndex", row);
                EditRow(gridId, rowIndex);
                var tag = tempModuleId + "_" + selectId;
                $("#rowOperateDiv_" + tag).hide();
                $("#rowOkDiv_" + tag).show();
                InitRowOpBtns(gridId);
                if (page != 'add' && page != 'edit' && $("a[id^='rowOkBtn_']").length == 0) {
                    $(obj).hide();
                    $(obj).parent().append("<a id=\"btnRowSave\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-save',plain:true\">保存</a><a id=\"btnRowCancel\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-p2-icon-cancel',plain:true\">取消</a>");
                    $('#btnRowSave').click(function () {
                        RowEditSave(tempModuleId, moduleName, selectId, gridId, function (result) {
                            if (result && result.Success) {
                                $('#btnRowSave').remove();
                                $('#btnRowCancel').remove();
                                $(obj).show();
                            }
                        });
                    });
                    $('#btnRowCancel').click(function () {
                        RowEditCancel(selectId, gridId, function () {
                            $('#btnRowSave').remove();
                            $('#btnRowCancel').remove();
                            $(obj).show();
                        });
                    });
                    var gridToolbarId = $(obj).parent().parent().attr('id');
                    ParserLayout(gridToolbarId);
                }
            }
            else if (editMode == 4) { //网格编辑表单
                var rowIndex = gridObj.datagrid("getRowIndex", row);
                var expanderRowIdPrefix = GetExpanderRowIdPrefix();
                var rowExpanderIconDom = $("#" + expanderRowIdPrefix + rowIndex + " span.datagrid-row-expander");
                var formType = rowExpanderIconDom.attr("formType");
                if (rowExpanderIconDom.hasClass("datagrid-row-expand")) { //收缩状态
                    rowExpanderIconDom.attr("formType", "edit");
                    rowExpanderIconDom.click();
                }
                else { //已展开状态
                    if (formType != "edit") {
                        rowExpanderIconDom.attr("formType", "edit");
                        var editWidth = parseInt($(obj).attr("editWidth"));
                        var editHeight = parseInt($(obj).attr("editHeight"));
                        var option = { titleKey: titleKey, editWidth: editWidth, editHeight: editHeight };
                        ExpandGridRowForm(gridId, tempModuleId, moduleName, row, rowIndex, option);
                    }
                }
            }
        }
        var isDraft = GetLocalQueryString("draft"); //是否我的草稿列表页
        var btnVerifyText = $(obj).attr('btnVerifyText');
        if (btnVerifyText == undefined || btnVerifyText == '')
            btnVerifyText = "编辑";
        if (!isDraft) {
            //先进行客户端和服务端验证
            var tempTodoId = GetLocalQueryString("todoId"); //待办任务ID
            if (todoId)
                tempTodoId = todoId;
            if (tempTodoId == undefined || tempTodoId == '')
                tempTodoId = null;
            GridOperateVerify(moduleName, btnVerifyText, selectId, function (errMsg) {
                if (errMsg && errMsg.length > 0) {
                    topWin.ShowAlertMsg("提示", errMsg, "info"); //弹出验证提示信息
                }
                else {
                    //调用通用方法
                    ExecuteMethod(); //执行编辑
                }
            }, tempTodoId);
        }
        else {
            ExecuteMethod(); //执行编辑
        }
    }
    else { //自定义编辑表单
        if (formUrl.indexOf('?') > -1) {
            formUrl += '&id=' + selectId;
        }
        else {
            formUrl += '?id=' + selectId;
        }
        AddTab(null, title, formUrl); //执行编辑
    }
}

//删除，兼之明细删除
function Delete(obj) {
    var tempModuleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var gridId = $(obj).attr('gridId');
    var gridObj = $("#" + gridId);
    var rows = GetSelectRows(gridId); //获取选中行
    if (!rows || rows.length == 0) { //没有选中行，从当前按钮中找对应的记录Id来得到选择行
        var selectId = $(obj).attr("recordId");
        var tempRows = gridObj.datagrid("getRows");
        for (var i = 0; i < tempRows.length; i++) {
            var tempRow = tempRows[i];
            if (selectId == tempRow["Id"]) {
                rows.push(tempRow);
                break;
            }
        }
    }
    if (!rows || rows.length == 0) {
        topWin.ShowMsg("提示", "请至少选择一条记录！", null, null, 1);
        return;
    }
    var isRecycle = parseInt($(obj).attr("recycle")) == 1; //是来自回收站
    var isHardDel = parseInt($(obj).attr("isHardDel")) == 1; //是否硬删除
    var msgTitle = "删除提示";
    //有自定义删除方法则先调用自定义否则调用通用
    if (typeof (OverDelete) == "function") {
        OverDelete(obj);
        return;
    }
    var titleKey = $(obj).attr("titleKey");
    var titleKeyDisplay = $(obj).attr("titleKeyDisplay");
    var msg = "确定要删除";
    if (rows.length == 1 && titleKeyDisplay && rows[0][titleKey]) {
        msg += titleKeyDisplay + "为【" + rows[0][titleKey] + "】的记录吗？";
    }
    else {
        msg += "选中的记录吗？";
    }
    if (page == "grid" || page == "view") {
        if (isHardDel || isRecycle) {
            msg += "数据删除后不可恢复，请谨慎操作！";
        }
    }
    topWin.ShowConfirmMsg(msgTitle, msg, function (action) {
        if (action) {
            if (page == "grid" || page == "view") { //主网格页面或主网格下方明细网格或者主从查看页面
                var ids = "";
                var dr = [];
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i]["Id"] != null)
                        ids += rows[i]["Id"] + ",";
                    else
                        dr.push(rows[i]);
                }
                DelRows(gridId, dr); //删除新增的，还没保存到数据库的，主要针对编辑模式为行编辑模式下
                if (ids.length > 0) { //删除已存在的
                    ids = ids.substr(0, ids.length - 1);
                    //先进行客户端和服务端验证
                    GridOperateVerify(moduleName, "删除", ids, function (errMsg) {
                        //如果返回空或返回pass就表示验证通过
                        if (errMsg != "pass" && errMsg && errMsg.length > 0) {
                            topWin.ShowAlertMsg("提示", errMsg, "info"); //弹出验证提示信息
                        }
                        else {
                            ExecuteCommonDelete(tempModuleId, ids, isRecycle, isHardDel, function () {
                                //刷新列表
                                gridObj.datagrid("reload");
                            });
                        }
                    }, isRecycle ? "recycle" : "");
                }
            }
            else if (page == "add" || page == "edit") { //编辑页面
                DelRows(gridId, rows);
                if (page == 'edit') { //主从编辑页面
                    var editMode = $(obj).attr('editmode');
                    if (editMode == 3) { //列表行编辑模式
                        var fields = GetGridColumns(gridId);
                        if (fields != null) {
                            gridObj.datagrid('fixColumnSize', fields[0].field); //修正列宽
                        }
                    }
                }
            }
        }
    });
}

//查看，兼之明细查看
function ViewRecord(obj) {
    var tempModuleId = $(obj).attr("moduleId");
    var gridId = $(obj).attr('gridId');
    var gridObj = $("#" + gridId);
    var editMode = parseInt($(obj).attr("editMode"));
    var titleKey = $(obj).attr("titleKey"); //标记字段名
    var selectId = $(obj).attr("recordId"); //记录Id
    var titleKeyValue = $(obj).attr("titleKeyValue"); //标记字段值
    if (!titleKeyValue || titleKeyValue.length == 0) {
        var row = GetSelectRow(gridId); //获取选中行
        if (!row) { //没有选中行，从当前按钮中找对应的记录Id来得到选择行
            if (selectId) {
                var rows = gridObj.datagrid("getRows");
                for (var i = 0; i < rows.length; i++) {
                    var tempRow = rows[i];
                    if (selectId == tempRow["Id"]) {
                        row = tempRow;
                        break;
                    }
                }
            }
        }
        if (!row) {
            topWin.ShowMsg("提示", "请选择一条记录！", null, null, 1);
            return;
        }
        if (!selectId)
            selectId = row["Id"];
        titleKeyValue = row[titleKey];
    }
    var moduleName = $(obj).attr("moduleName");
    var moduleDisplay = $(obj).attr("moduleDisplay");
    if (!moduleDisplay) moduleDisplay = moduleName;
    var title = "查看" + moduleDisplay;
    if (titleKey && titleKeyValue) {
        title = title + "－" + titleKeyValue;
    }
    var formUrl = $(obj).attr('formUrl'); //自定义表单页面URL
    if (formUrl == undefined || formUrl == null || formUrl.length == 0) { //通用新增页面
        var viewPageUrl = "/Page/ViewForm.html?page=view&moduleId=" + tempModuleId;
        if (moduleName != undefined && moduleName != null && moduleName.length > 0)
            viewPageUrl += "&moduleName=" + encodeURI(moduleName);
        viewPageUrl += "&tip=1";
        viewPageUrl += "&mode=" + editMode;
        viewPageUrl += "&fp=grid";
        var isEditDetailView = false; //是否编辑明细查看
        if (page == "add" || page == "edit" || page == "view") { //主从编辑页面添加标识
            var parentMode = GetLocalQueryString("mode"); //父页面编辑模式
            viewPageUrl += "&pmode=" + parentMode;
            if ((editMode == 2 || editMode == 3) && (page == "add" || page == "edit")) {
                viewPageUrl += "&ff=1"; //标识来自表单页面
                isEditDetailView = true;
                if (selectId && selectId != GuidEmpty)
                    viewPageUrl += "&id=" + selectId;
            }
            else {
                viewPageUrl += "&id=" + selectId;
            }
        }
        else {
            viewPageUrl += "&id=" + selectId;
        }
        var recycle = $(obj).attr("recycle");
        if (parseInt(recycle) == 1) {
            viewPageUrl += "&recycle=1";
        }
        if (isNfm) {
            viewPageUrl += '&nfm=1';
        }
        viewPageUrl += "&r=" + Math.random();
        //url重写
        if (typeof (OverGetFormUrl) == "function") {
            var tempUrl = OverGetFormUrl('view', viewPageUrl, moduleName, tempModuleId, obj);
            if (tempUrl) {
                viewPageUrl = tempUrl;
            }
        }
        //执行新增方法
        var ExecuteMethod = function () {
            //有自定义查看方法则先调用自定义否则调用通用
            if (typeof (OverViewRecord) == "function") {
                OverViewRecord(obj);
                return;
            }
            if (editMode == 2 || editMode == 3) { //弹出框编辑模式
                var editWidth = parseInt($(obj).attr("editWidth"));
                var editHeight = parseInt($(obj).attr("editHeight"));
                editWidth += 40; //加上padding相关
                editHeight += 35 + 50; //标题栏高度和按钮栏高度
                //加载弹出框操作按钮
                var params = { moduleId: tempModuleId, id: selectId, formType: 1, editMode: editMode, page: page };
                if (parseInt(recycle) == 1) {
                    params.recycle = 1;
                }
                if (isNfm) {
                    params.nfm = 1;
                }
                var ajaxUrl = '/' + CommonController.Async_System_Controller + '/LoadFormBtns.html';
                if (isNfm) {
                    ajaxUrl = host + ajaxUrl;
                }
                ExecuteCommonAjax(ajaxUrl, params, function (result) {
                    if (result && result.length > 0) {
                        var toolbar = [];
                        for (var i = 0; i < result.length; i++) {
                            if (isEditDetailView && result[i].iconType == 1)
                                continue;
                            var tempHandler = result[i].handler;
                            if (typeof (tempHandler) == 'string') {
                                result[i].handler = eval('(' + tempHandler + ')');
                            }
                            toolbar.push(result[i]);
                        }
                        topWin.OpenDialog(title, viewPageUrl, toolbar, editWidth, editHeight, null, function (dialogDivId) {
                            setTimeout(function () {
                                var divBtnPar = topWin.$("#" + dialogDivId).parent();
                                var dgBtn = topWin.$("div.dialog-button a", divBtnPar);
                                if (page != "view") {
                                    dgBtn.attr("detail", toolbar[0].detail);
                                }
                                dgBtn.attr("editMode", editMode);
                                dgBtn.attr("moduleId", tempModuleId);
                                dgBtn.attr("moduleName", moduleName);
                                dgBtn.attr("gridId", gridId);
                            }, 50);
                        });
                    }
                }, false, true);
            }
            else if (editMode == 1 || editMode == 3) { //tab查看模式或列表行查看模式
                AddTab(null, title, viewPageUrl);
            }
            else if (editMode == 4) { //网格查看表单
                var rowIndex = gridObj.datagrid("getRowIndex", row);
                var expanderRowIdPrefix = GetExpanderRowIdPrefix();
                var rowExpanderIconDom = $("#" + expanderRowIdPrefix + rowIndex + " span.datagrid-row-expander");
                var formType = rowExpanderIconDom.attr("formType");
                if (rowExpanderIconDom.hasClass("datagrid-row-expand")) { //收缩状态
                    rowExpanderIconDom.attr("formType", "view");
                    rowExpanderIconDom.click();
                }
                else { //已展开状态
                    if (formType != "view") {
                        rowExpanderIconDom.attr("formType", "view");
                        var editWidth = parseInt($(obj).attr("editWidth"));
                        var editHeight = parseInt($(obj).attr("editHeight"));
                        var option = { titleKey: titleKey, editWidth: editWidth, editHeight: editHeight };
                        ExpandGridRowForm(gridId, tempModuleId, moduleName, row, rowIndex, option);
                    }
                }
            }
        }
        ExecuteMethod(); //执行查看
    }
    else { //自定义查看页面
        if (formUrl.indexOf('?') > -1) {
            formUrl += '&id=' + selectId;
        }
        else {
            formUrl += '?id=' + selectId;
        }
        AddTab(null, title, formUrl); //执行查看
    }
}

//批量编辑
function BatchEdit(obj) {
    //有自定义批量编辑方法则先调用自定义否则调用通用
    if (typeof (OverBatchEdit) == "function") {
        OverBatchEdit(obj);
        return;
    }
    var moduleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var moduleDisplay = $(obj).attr("moduleDisplay");
    if (!moduleDisplay) moduleDisplay = moduleDisplay;
    var gridId = $(obj).attr('gridId');
    var selectRows = GetSelectRows(gridId); //当前选择行
    var selectRecords = selectRows.length; //选择记录数
    var pageRows = GetCurrentRows(gridId); //当前页所有行
    var pageRecords = pageRows.length; //当前页记录数
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            topWin.GetCurrentDialogFrame()[0].contentWindow.GetBatchEditData(function (updateRange, data) {
                if (data && data.length > 0) {
                    var ids = ""; //更新所有记录
                    var rows = null;
                    if (updateRange == "1") { //更新当前选中记录
                        if (selectRows == null || selectRows.length == 0) {
                            topWin.ShowAlertMsg("提示", "当前选中记录数为0！", "info");
                            return;
                        }
                        rows = selectRows;
                    }
                    else if (updateRange == "2") { //更新当前页面记录
                        if (pageRows == null || pageRows.length == 0) {
                            topWin.ShowAlertMsg("提示", "当前页面记录数为0！", "info");
                            return;
                        }
                        rows = pageRows;
                    }
                    if (rows != null) {
                        for (var i = 0; i < rows.length; i++) {
                            var row = rows[i];
                            ids += row["Id"] + ",";
                        }
                        ids = ids.substr(0, ids.length - 1);
                    }
                    $.ajax({
                        type: 'post',
                        url: '/' + CommonController.Async_Data_Controller + '/BatchUpdate.html',
                        data: { moduleId: moduleId, ids: ids, data: $.base64.encode(escape(JSON.stringify(data).Replace("\\+", "%20"))) },
                        dataType: "json",
                        beforeSend: function () {
                            topWin.OpenWaitDialog('正在更新...');
                        },
                        success: function (result) {
                            topWin.CloseWaitDialog();
                            if (result && result.Success) {
                                topWin.CloseDialog();
                                RefreshGrid(gridId);
                            }
                            else {
                                topWin.ShowAlertMsg("提示", result.Message, "info");
                            }
                        },
                        error: function (err) {
                            topWin.CloseWaitDialog();
                            topWin.ShowAlertMsg("提示", "批量更新失败，服务端异常！", "error");
                        }
                    });
                }
                else {
                    topWin.ShowAlertMsg("提示", "请至少选择一个变更字段并设置字段值！", "info");
                }
            });
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var url = '/Page/BatchEdit.html?moduleId=' + moduleId + '&selectRecords=' + selectRecords + '&pageRecords=' + pageRecords;
    if (isNfm) {
        url += '&nfm=1';
    }
    url += '&r=' + Math.random();
    topWin.OpenDialog('批量编辑－' + moduleName, url, toolbar, 600, 450, 'eu-icon-edit');
}

//导入实体
function ImportModel(obj) {
    //有自定义导入实体方法则先调用自定义否则调用通用
    if (typeof (OverImportModel) == "function") {
        OverImportModel(obj);
        return;
    }
    var moduleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var moduleDisplay = $(obj).attr("moduleDisplay");
    if (!moduleDisplay) moduleDisplay = moduleDisplay;
    var gridId = $(obj).attr('gridId');
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var iframe = topWin.GetCurrentDialogFrame();
            iframe[0].contentWindow.UploadTempData(function (fileName) {
                //数据文件上传完成后开始导入数据
                topWin.DisableTopDialogBtn('btnOk'); //禁用确定按钮
                if (typeof (OverImportModelData) != "function") {
                    $.ajax({
                        type: "post",
                        url: "/" + CommonController.Async_Data_Controller + "/ImportModelData.html",
                        data: { moduleId: moduleId, fileName: escape(fileName) },
                        dataType: "json",
                        beforeSend: function () {
                        },
                        success: function (result) {
                            if (result.Success) {
                                topWin.ShowMsg('导入提示', "导入成功！", function () {
                                    RefreshGrid(gridId);
                                });
                            }
                            else {
                                topWin.ShowAlertMsg('导入提示', result.Message, 'info', function () {
                                });
                            }
                        },
                        error: function (err) {
                            topWin.CloseWaitDialog();
                            topWin.ShowAlertMsg('导入提示', "数据导入失败，服务器异常！", 'error');
                        }
                    });
                }
                else { //有自定义重写方法
                    OverImportModelData(fileName)
                }
                topWin.ShowMsg('导入提示', '系统正在处理中，可能需要等待较长时间，等待期间您可以处理其他工作，在弹出提示信息前请不要重复点击！', function () {
                    topWin.CloseDialog();
                });
            });
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var url = '/Page/ImportModel.html?moduleId=' + moduleId
    if (isNfm) {
        url += '&nfm=1';
    }
    url += '&r=' + Math.random();
    topWin.OpenDialog('数据导入－' + moduleDisplay, url, toolbar, 600, 300, 'eu-icon-export');
}

//导出数据
function ExportModel(obj) {
    //有自定义导出方法则先调用自定义否则调用通用
    if (typeof (OverExportModel) == "function") {
        OverExportModel(obj);
        return;
    }
    var moduleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var moduleDisplay = $(obj).attr("moduleDisplay");
    if (!moduleDisplay) moduleDisplay = moduleDisplay;
    var gridId = $(obj).attr('gridId');
    var toolbar = [{
        id: 'btnOk',
        text: "导 出",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var iframe = topWin.GetCurrentDialogFrame();
            iframe[0].contentWindow.GetCondition(function (type, conditions) {
                var url = '/' + CommonController.Async_Data_Controller + '/ExportModelData.html';
                var data = { moduleId: moduleId };
                if (type == 0) {
                    data = searchParams ? searchParams : {};
                    data.moduleId = moduleId;
                }
                else if (type == 2) {
                    data = { moduleId: moduleId, cdItems: JSON.stringify(conditions) };
                }
                var viewId = $("#btn_gridSet" + moduleId).attr("viewId");
                if (viewId != undefined && viewId != null) {
                    data.vId = viewId;
                    data.gvId = viewId;
                }
                //开始导出数据
                topWin.DisableTopDialogBtn('btnOk'); //禁用确定按钮
                $.ajax({
                    type: "post",
                    url: url,
                    data: data,
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (result) {
                        if (result.Success) {
                            var downUrl = result.DownUrl;
                            if (isNfm) {
                                downUrl = host + downUrl;
                            }
                            window.open(result.DownUrl);
                        }
                        else {
                            topWin.CloseWaitDialog();
                            topWin.ShowAlertMsg('导出提示', result.Message, 'info');
                        }
                    },
                    error: function (err) {
                        topWin.CloseWaitDialog();
                        topWin.ShowAlertMsg('导出提示', "【" + moduleName + "】数据导出失败，服务器异常！", 'error');
                    }
                });
                topWin.CloseDialog();
                topWin.ShowMsg('导出提示', '系统正在处理中，可能需要等待较长时间，等待期间您可以处理其他工作，在弹出提示信息前请不要重复点击！', null, 'center', 5);
            });
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var count = parseInt($("#" + gridId).attr('total'));
    var url = '/Page/ExportModel.html?moduleId=' + moduleId + '&cc=' + count;
    if (isNfm) {
        url += '&nfm=1';
    }
    url += '&r=' + Math.random();
    topWin.OpenDialog('数据导出－' + moduleDisplay, url, toolbar, 600, 300, 'eu-icon-export');
}

//复制
function Copy(obj) {
    var tempModuleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var gridId = $(obj).attr('gridId');
    var gridObj = $("#" + gridId);
    var editMode = parseInt($(obj).attr("editMode")); //编辑模式
    var row = GetSelectRow(gridId); //获取选中行
    if (!row) { //没有选中行，从当前按钮中找对应的记录Id来得到选择行
        var selectId = $(obj).attr("recordId"); //要编辑的记录Id
        var rows = gridObj.datagrid("getRows");
        for (var i = 0; i < rows.length; i++) {
            var tempRow = rows[i];
            if (selectId == tempRow["Id"]) {
                row = tempRow;
                break;
            }
        }
    }
    if (!row) {
        topWin.ShowMsg("提示", "请选择一条记录！", null, null, 1);
        return;
    }
    //先进行客户端和服务端验证
    var otherParams = null;
    if ((page == 'grid' || page == 'view') && gridId != 'mainGrid') {
        var mainModuleId = GetLocalQueryString("moduleId"); //主模块moduleId
        var mainRecordId = page == 'grid' ? GetSelectRow('mainGrid').Id : GetLocalQueryString("id");
        otherParams = mainModuleId + ',' + mainRecordId;
    }
    GridOperateVerify(moduleName, "复制", null, function (errMsg) {
        if (errMsg && errMsg.length > 0) {
            topWin.ShowAlertMsg("提示", errMsg, "info"); //弹出验证提示信息
        }
        else {
            //调用通用方法
            Add(obj, row["Id"]); //执行复制
        }
    }, otherParams);
}

//转到回收站
function GoToRecycle(obj) {
    var moduleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var moduleDisplay = $(obj).attr("moduleDisplay");
    if (!moduleDisplay)
        moduleDisplay = moduleName;
    var url = "/Page/Grid.html?page=grid&moduleId=" + moduleId + "&moduleName=" + moduleName + "&recycle=1";
    if (isNfm) {
        url += '&nfm=1';
    }
    var title = "回收站－" + moduleDisplay;
    AddTab(null, title, url);
}

//还原数据
function Restore(obj) {
    var tempModuleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var gridId = "mainGrid";
    var gridObj = $("#" + gridId);
    var rows = GetSelectRows(gridId); //获取选中行
    if (!rows || rows.length == 0) { //没有选中行，从当前按钮中找对应的记录Id来得到选择行
        var selectId = $(obj).attr("recordId");
        var tempRows = gridObj.datagrid("getRows");
        for (var i = 0; i < tempRows.length; i++) {
            var tempRow = tempRows[i];
            if (selectId == tempRow["Id"]) {
                rows.push(tempRow);
                break;
            }
        }
    }
    if (!rows || rows.length == 0) {
        topWin.ShowMsg("提示", "请至少选择一条记录！", null, null, 1);
        return;
    }
    var msgTitle = "还原提示";
    //有自定义删除方法则先调用自定义否则调用通用
    if (typeof (OverRestore) == "function") {
        OverRestore(obj);
        return;
    }
    var titleKey = $(obj).attr("titleKey");
    var titleKeyDisplay = $(obj).attr("titleKeyDisplay");
    var msg = "确定要还原";
    if (rows.length == 1 && titleKeyDisplay) {
        msg += titleKeyDisplay + "为【" + rows[0][titleKey] + "】的记录吗？";
    }
    else {
        msg += "选中的记录吗？";
    }
    topWin.ShowConfirmMsg(msgTitle, msg, function (action) {
        if (action) {
            if (page == "grid") { //主网格页面
                var ids = "";
                for (var i = 0; i < rows.length; i++) {
                    ids += rows[i]["Id"] + ",";
                }
                if (ids.length > 0) {
                    ids = ids.substr(0, ids.length - 1);
                }
                //先进行客户端和服务端验证
                GridOperateVerify(moduleName, "还原", ids, function (errMsg) {
                    if (errMsg && errMsg.length > 0) {
                        topWin.ShowAlertMsg("提示", errMsg, "info"); //弹出验证提示信息
                    }
                    else { //执行还原
                        ExecuteCommonRestore(tempModuleId, ids, function () {
                            //刷新列表
                            gridObj.datagrid("reload");
                        });
                    }
                });
            }
        }
    });
}

//转到草稿
function GoToDraft(obj) {
    var moduleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var url = "/Page/Grid.html?page=grid&moduleId=" + moduleId + "&draft=1";
    if (isNfm) {
        url += '&nfm=1';
    }
    var title = "我的草稿－" + moduleName;
    AddTab(null, title, url);
}

//草稿箱 发布
function Release(obj) {
}

//批量发起流程
function MutiStartFlow(obj) {
    if ($('img.flowImg').length == 0) return;
    var tempModuleId = $(obj).attr("moduleId");
    var moduleName = $(obj).attr("moduleName");
    var gridId = $(obj).attr('gridId');
    var gridObj = $("#" + gridId);
    var rows = GetSelectRows(gridId); //获取选中行
    if (!rows || rows.length == 0) {
        topWin.ShowMsg("提示", "请至少选择一条记录！", null, null, 1);
        return;
    }
    var ids = ""; //要发起的记录ID集合
    for (var i = 0; i < rows.length; i++) {
        var rowId = rows[i]["Id"];
        var flowStatus = rows[i]["FlowStatus"];
        if (rowId && (flowStatus == 0 || flowStatus == null))
            ids += rows[i]["Id"] + ",";
    }
    if (ids.length > 0) {
        ids = ids.substr(0, ids.length - 1);
    }
    if (ids.length == 0) {
        topWin.ShowMsg("提示", "所选记录中没有满足条件的记录！", null, null, 1);
        return;
    }
    var ExecuteStartFlow = function () {
        var msgTitle = '发起流程';
        var msg = '确定要提交流程吗？';
        topWin.ShowConfirmMsg(msgTitle, msg, function (action) {
            if (action) {
                var url = '/' + CommonController.Async_Bpm_Controller + '/MutiStartProcess.html';
                var paramas = { moduleId: tempModuleId, ids: ids };
                ExecuteCommonAjax(url, paramas, function (result) {
                    if (result.Success) { //成功刷新网格
                        RefreshGrid(gridId);
                        if (gridId == 'mainGrid') { //当前发起的是主模块流程
                            //如果下方明细模块启用流程则刷新明细模块数据
                            var tabObj = $('#detailTabs');
                            if (tabObj.length > 0 && tabObj.find('img.flowImg').length > 0) {
                                var selectTab = tabObj.tabs('getSelected');
                                LoadAttachModuleData(selectTab, rows[0], true);
                            }
                        }
                    }
                    if (result.Message) {
                        topWin.ShowAlertMsg(msgTitle, result.Message, 'info');
                    }
                }, true);
            }
        });
    }
    //先进行客户端和服务端验证
    GridOperateVerify(moduleName, "提交", ids, function (errMsg) {
        if (errMsg && errMsg.length > 0) {
            topWin.ShowAlertMsg("提示", errMsg, "info"); //弹出验证提示信息
        }
        else {
            ExecuteStartFlow(); //执行提交
        }
    }, null, true);
}

//重新发起流程
//针对拒绝的单据
function ReStartFlow(obj) {
    var rows = GetFinalSelectRows(obj);
    if (!rows) return;
    $(obj).attr('rsf', '1'); //重新发起标识
    //先进行客户端和服务端验证
    var moduleName = $(obj).attr("moduleName");
    var restartId = rows[0].Id;
    GridOperateVerify(moduleName, "重新发起", restartId, function (errMsg) {
        if (errMsg && errMsg.length > 0) {
            topWin.ShowAlertMsg("提示", errMsg, "info"); //弹出验证提示信息
        }
        else {
            //调用通用方法
            Add(obj, restartId);
        }
    }, null, true);
}

//添加新行
//gridId:网格domId
//moduleId:模块ID
//moduleName:模块名称
//notBindEvent:是否不绑定控件事件
function AddRow(gridId, moduleId, moduleName, notBindEvent) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    gridObj.datagrid('insertRow', { index: 0, row: {} });
    gridObj.datagrid('beginEdit', 0);
    if (!notBindEvent) {
        //绑定控件事件
        BindGridControlEditorEvent(tempGridId, 0, moduleId, moduleName, false);
    }
    //增加完成调用自定义事件
    if (typeof (OverAddRowCompeleted) == "function") {
        OverAddRowCompeleted(tempGridId, 0);
    }
}

//编辑行
//gridId:网格domId
//rowIndex:行号
//moduleId:当前模块Id
//moduleName:模块名称
//notBindEvent:是否不绑定控件事件
function EditRow(gridId, rowIndex, moduleId, moduleName, notBindEvent) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    try {
        gridObj.datagrid('beginEdit', rowIndex);
    } catch (e) { }
    if (!notBindEvent) {
        //绑定控件事件
        BindGridControlEditorEvent(tempGridId, rowIndex, moduleId, moduleName, true);
    }
}

//结束编辑行
//gridId:网格domId
//rowIndex:行号
function EndEditRow(gridId, rowIndex) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    gridObj.datagrid('endEdit', rowIndex);
    gridObj.datagrid('acceptChanges');
}

//结束编辑所有行
//gridId:网格domId
function EndEditAllRows(gridId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var rows = GetCurrentRows(tempGridId);
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            var rowIndex = GetRowIndexByRow(tempGridId, row);
            EndEditRow(tempGridId, rowIndex);
        }
    }
}

//取消编辑行
//gridId:网格domId
//rowIndex:行号
//isDelRow:是否删除行
function CancelEditRow(gridId, rowIndex, isDelRow) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    gridObj.datagrid('cancelEdit', rowIndex);
    if (isDelRow != undefined && isDelRow) {
        gridObj.datagrid('deleteRow', rowIndex);
    }
}

//插入行
//gridId:网格domId
//row:行记录
function AppendRow(gridId, row) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    gridObj.datagrid('appendRow', row);
}

//更新行
//gridId:网格domId
//rowIndex:行号
//row:行记录
function UpdateRow(gridId, rowIndex, row) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    gridObj.datagrid('updateRow', {
        index: rowIndex,
        row: row
    });
}

//更新行字段数据
//gridId:网格domId
//rowIndex:行号
//field:字段名
//value:字段值
function UpdateRowFieldValue(gridId, rowIndex, field, value) {
    try {
        var tempGridId = gridId ? gridId : 'mainGrid';
        var gridObj = $("#" + tempGridId);
        var updateJson = {};
        var td = GetGridEditorTdCell(rowIndex, field, $('#' + tempGridId).parent());
        if (td.find('input').length > 0) { //编辑网格
            var column = GetGridColumn(tempGridId, field);
            var editor = GetGridEditorControl2(tempGridId, rowIndex, field);
            switch (column.editor.type) {
                case 'checkbox':
                    editor.val(value);
                    break;
                case 'numberbox':
                    editor.numberbox('setValue', value);
                    break;
                case 'combobox':
                    editor.combobox('setValue', value);
                    break;
                case 'textarea':
                case 'textbox':
                    editor.textbox('setValue', value);
                    break;
                case 'datebox':
                    editor.datebox('setValue', value);
                    break;
                case 'datetimebox':
                    editor.datetimebox('setValue', value);
                    break;
                case 'combotree':
                    editor.combotree('setValue', value);
                    break;
            }
        }
        else { //非编辑状态时
            var row = {};
            row[field] = value;
            gridObj.datagrid('updateRow', {
                index: rowIndex,
                row: row
            });
            if (tempGridId.indexOf('grid_') > -1) {
                InitRowOpBtns(tempGridId);
            }
        }
    }
    catch (e) { }
}

//删除行
//gridId:网格domId
//rowIndex:行号
function DelRow(gridId, rowIndex) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var gridObj = $("#" + tempGridId);
    gridObj.datagrid("deleteRow", rowIndex);
}

//移除当前所有行
//gridId:网格domId
function DelCurrentRows(gridId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var rows = GetCurrentRows(tempGridId);
    var copyRows = [];
    for (var i = 0; i < rows.length; i++) {
        copyRows.push(rows[i]);
    }
    for (var i = 0; i < copyRows.length; i++) {
        var row = copyRows[i];
        var rowIndex = GetRowIndexByRow(tempGridId, row);
        DelRow(tempGridId, rowIndex);
    }
}

//移除当前所有选中行
//gridId:网格domId
function DelSelectRows(gridId) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    var rows = GetSelectRows(tempGridId);
    DelRows(tempGridId, rows);
}

//删除多行
//gridId:网格DomId
//rows:行记录
function DelRows(gridId, rows) {
    var tempGridId = gridId ? gridId : 'mainGrid';
    if (rows && rows.length > 0) {
        var copyRows = [];
        for (var i = 0; i < rows.length; i++) {
            copyRows.push(rows[i]);
        }
        for (var i = 0; i < copyRows.length; i++) {
            var row = copyRows[i];
            var rowIndex = GetRowIndexByRow(tempGridId, row);
            DelRow(tempGridId, rowIndex);
        }
    }
}

//表单数据保存完成事件
//result:保存成功后的结果对象
function FormDataSaveCompeleted(result) {
    var obj = $("#btnAdd");
    var linkAdd = obj.attr("linkAdd"); //外链
    if (linkAdd == "true") {
        var initField = obj.attr("initField"); //原始模块表单外链按钮左边的控件字段
        var linkField = obj.attr("linkField"); //外链模块的titleKey字段
        var initNameField = initField.replace("Id", "Name");
        var formData = topWin.$("#main_dialog").find("iframe")[0].contentWindow.GetFormData();
        $("#" + initField).val(result.RecordId); //值控件斌值
        $("#" + initNameField).val(formData[linkField]); //文本控件斌值
    }
}

//切换视图
//moduleId:模块Id
//gridId:网格domId
//viewId:视图Id
//isRf:是否启用过滤行
//backFun:切换成功回调函数
function ChangeGridView(moduleId, gridId, viewId, isRf, backFun) {
    if (isRf == undefined) {
        if ($('#btn_filterSearch').attr('isrf') == '1')
            isRf = 1;
    }
    var url = '/' + CommonController.Async_System_Controller + '/ChangeGridView.html';
    if (isNfm) {
        url += '?nfm=1';
    }
    //先加载视图字段
    $.ajax({
        type: 'post',
        url: url,
        data: { viewId: viewId, gridId: gridId, isRf: isRf },
        dataType: "json",
        beforeSend: function () {
            topWin.OpenWaitDialog('视图切换中...');
        },
        success: function (data) {
            topWin.CloseWaitDialog();
            if (!data) {
                topWin.ShowAlertMsg("提示", "视图切换失败，视图不存在！", "info");
                return;
            }
            var gridDom = $("#" + gridId);
            var gridView = data.GridView;
            $("#btn_gridSet" + moduleId).attr("title", "单击可切换列表视图--当前视图：" + gridView.Name);
            $("#btn_gridSet" + moduleId).attr("viewId", gridView.Id);
            $("#btn_advanceSearch" + moduleId).attr("viewId", gridView.Id);
            $('#btn_filterSearch').attr("viewId", gridView.Id);
            var isEnableFilter = isRf == 1 || (isRf != 2 && gridView.AddFilterRow);
            if (isEnableFilter) { //启用了行过滤
                //先移除所有行过滤规则
                gridDom.datagrid("removeFilterRule");
            }
            //处理搜索字段
            var searchFields = data.SearchFields;
            var menuObj = $("#search_mm" + moduleId);
            //先移除之前所有字段项 menuObj.html("");
            menuObj.find("div").each(function () {
                menuObj.menu('removeItem', $(this));
            });
            //重新加载字段项
            $('#txtSearch').searchbox('reset').searchbox('clear');
            if (searchFields && searchFields.length > 0) { //重置搜索字段
                $.each(searchFields, function (i, item) {
                    menuObj.menu('appendItem', {
                        id: item.FieldName,
                        name: item.FieldName,
                        text: item.Display,
                        iconCls: 'eu-p2-icon-table'
                    });
                });
                //默认选择第一个字段
                $('#txtSearch').searchbox('selectName', searchFields[0].FieldName);
            }
            //行过滤规则处理
            if (isEnableFilter) { //启用了行过滤
                if (data.RuleFilters) {
                    $('#ruleFilters').val(data.RuleFilters);
                }
                if (data.NoFilterFields) {
                    $('#ruleFilters').attr('noFilterFields', data.NoFilterFields);
                }
            }
            //字段处理
            var viewFields = data.ViewFields;
            if (viewFields && viewFields.length > 0) {
                //锁定字段
                var frozenList = [];
                var frozenArray = [];
                //显示字段
                var fieldList = [];
                var fieldArray = [];
                var groupField = null;
                for (var i = 0; i < viewFields.length; i++) {
                    var field = viewFields[i];
                    var formatter = null;
                    if (field.FieldFormatter && field.FieldFormatter.length > 0) {
                        try {
                            formatter = eval("(" + field.FieldFormatter + ")");
                        } catch (e) { }
                    }
                    if (field.IsGroupField) {
                        groupField = field.Sys_FieldName;
                    }
                    if (field.IsFrozen || field.Sys_FieldName == "Id") {
                        frozenArray.push({ field: field.Sys_FieldName, title: field.Display, width: field.Width, hidden: !field.IsVisible, formatter: formatter, checkbox: field.Sys_FieldName == "Id", sortable: field.IsAllowSort });
                    }
                    else {
                        fieldArray.push({ field: field.Sys_FieldName, title: field.Display, width: field.Width, hidden: !field.IsVisible, formatter: formatter, sortable: field.IsAllowSort });
                    }
                }
                frozenList.push(frozenArray);
                fieldList.push(fieldArray);
                //重新加载字段
                var options = gridDom.datagrid("options");
                options.columns = fieldList;
                options.frozenColumns = frozenList;
                var gridUrl = $("#btn_gridSet" + moduleId).attr("gridUrl");
                if (!gridUrl)
                    gridUrl = options.url;
                if (gridView.GridType == 3 || gridView.GridType == 4) { //综合视图或综合明细视图
                    gridUrl += "&viewId=" + gridView.Id;
                }
                else {
                    gridUrl += "&gvId=" + gridView.Id;
                }
                if (gridView.GridType == 4) { //综合明细视图
                    gridUrl += "&dv=1";
                    $("a[id^='btn_attach_set_']").hide();
                    $('#region_south').removeAttr("flag").attr("isDetailView", "true").hide(); //附属网格不显示
                    $("#regon_main .datagrid").css('border-bottom-width', '0px');
                }
                else {
                    $("a[id^='btn_attach_set_']").show();
                    $('#region_south').removeAttr("isDetailView").removeAttr("flag").show();
                }
                if (groupField) {
                    options.view = groupview;
                    options.groupField = groupField;
                    options.groupFormatter = function (value, rows) { return value + '(' + rows.length + ')'; };
                }
                options.url = gridUrl;
                options.loadFilter = function (data) { if (typeof (OverLoadGridFilter) == 'function') { return OverLoadGridFilter(data, gridId, moduleId, null); } else { return data; } }
                if (gridId == "mainGrid" && isEnableFilter) { //启用了行过滤功能
                    gridDom.attr("enableFilter", "true"); //启用过滤标识
                }
                gridDom.datagrid(options);
                if (typeof (backFun) == "function") {
                    backFun();
                }
            }
        },
        error: function (err) {
            topWin.CloseWaitDialog();
            topWin.ShowAlertMsg("提示", "视图加载失败，服务端异常！", "error");
        }
    });
}

//创建网格列头右键菜单
//e:系统参数
//targetField:鼠标右键点击的字段名称
//menuDomId:右键菜单容器domId
//gridId:网格domId
function CreateColumnContextMenu(e, targetField, menuDomId, gridId) {
    e.preventDefault();
    var colMenuDom = $("#" + menuDomId);
    var gridObj = $("#" + gridId);
    colMenuDom.menu({
        onClick: function (item) {
            if (item.iconCls == 'eu-icon-ok') {
                gridObj.datagrid('hideColumn', item.name);
                colMenuDom.menu('setIcon', {
                    target: item.target,
                    iconCls: 'eu-icon-empty'
                });
            } else {
                gridObj.datagrid('showColumn', item.name);
                colMenuDom.menu('setIcon', {
                    target: item.target,
                    iconCls: 'eu-icon-ok'
                });
            }
            colMenuDom.show();
        }
    });
    var fields = gridObj.datagrid('getColumnFields');
    var noHideFields = gridObj.attr('noHideFields');//不允许隐藏的字段
    var token = noHideFields && noHideFields.length > 0 ? noHideFields.split(',') : null;
    for (var i = 0; i < fields.length; i++) {
        var field = fields[i];
        if (field == "Id") continue;
        if (token != null) {
            var flag = false;
            for (var j = 0; j < token.length; j++) {
                if (token[j] == field) {
                    flag = true;
                    break;
                }
            }
            if (flag) continue;
        }
        var col = gridObj.datagrid('getColumnOption', field);
        if (!!col.title && !col.hidden) {
            var item = colMenuDom.menu("findItem", col.title);
            if (!item) {
                colMenuDom.menu('appendItem', {
                    text: col.title,
                    name: field,
                    iconCls: 'eu-icon-ok'
                });
            }
        }
    }
    colMenuDom.menu('show', { left: e.pageX, top: e.pageY });
}

//网格行展开事件，针对网格内明细模块展示
//gridId:当前网格domId
//moduleId:当前模块Id
//moduleName:模块名称
//row:行对象
//index:行索引编号,有值时为网格内展开，无值时为网格下方加载
function ExpandGridRow(gridId, moduleId, moduleName, row, index) {
    var id = row["Id"];
    var gridObj = $("#" + gridId);
    var mod = gridObj.datagrid('getRowDetail', index);
    var html = mod.html();
    if (html && html.length > 0) { //数据已加载
        return;
    }
    var tag = id;
    var data = { moduleId: moduleId, id: id };
    if (gridId == "mainGrid") {
        var treeDom = $("#gridTree"); //左侧树dom对象
        var hasTree = treeDom && treeDom.length > 0;
        if (hasTree) //存在左侧树
            data.hasTree = 1;
    }
    var url = '/' + CommonController.Async_System_Controller + '/LoadInnerDetailModuleGrid.html';
    if (isNfm) {
        url += '&nfm=1';
    }
    $.ajax({
        type: 'get',
        url: url,
        data: data,
        dataType: "html",
        success: function (html) {
            mod.html(html);
            $.parser.parse('#ddv_' + tag);
            var tt = $('#tt_' + tag);
            var mm = $('#tt_mm_' + tag);
            gridObj.datagrid('fixDetailRowHeight', index);
            //处理tab右键菜单和tab工具栏
            //为选项卡绑定右键
            $("#ddv_" + tag + " .tabs-inner").bind('contextmenu', function (e) {
                mm.menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
                var title = $(this).children(".tabs-closable").text();
                mm.data("currtab", title);
                tt.tabs('select', title);
                return false;
            });
            //绑定右键菜单事件，刷新
            $('#tt_mm_refresh_' + tag + ',#tt_a_refresh_' + tag).click(function () {
                var tab = tt.tabs('getSelected');
                var iframe = $('iframe', tab);
                var url = iframe.attr('src');
                if (url && url.length > 0) {
                    tt.tabs('update', {
                        tab: tab,
                        options: {
                            content: CreateIFrame(url)
                        }
                    });
                }
            });
        },
        error: function (err) {
            topWin.CloseWaitDialog();
        }
    });
}

//获取expander图标的行id前缀
//如：datagrid-row-r2-1-rowIndex
//parentDom:网格所属父dom对象
function GetExpanderRowIdPrefix(parentDom) {
    var td = parentDom && parentDom.length > 0 ?
        $("div.datagrid-body-inner td[field='_expander']", parentDom).eq(0) :
        $("div.datagrid-body-inner td[field='_expander']").eq(0);
    var tempId = td.parent().attr("id");
    var startIndex = tempId.lastIndexOf("-");
    return tempId.substr(0, startIndex + 1);
}

//网格行展开事件，针对网格行内展开表单编辑
//gridId:当前网格domId
//moduleId:当前模块Id
//moduleName:模块名称
//row:行对象
//index:行索引编号,有值时为网格内展开编辑表单，无值时展开新增表单
//option:关联数据，包括titleKey,titleKeyDisplay,formWidth,formHeight
function ExpandGridRowForm(gridId, moduleId, moduleName, row, index, option) {
    var prefixId = GetExpanderRowIdPrefix();
    var rowExpanderIconDom = $("#" + prefixId + index + " span.datagrid-row-expander");
    var formType = rowExpanderIconDom.attr("formType");
    var gridObj = $("#" + gridId);
    var ddv = gridObj.datagrid('getRowDetail', index).find('div.ddv');
    var ddvHtml = ddv.html();
    if (ddvHtml.length > 0 && ddv.attr("formType") == formType) { //非第一次加载
        return;
    }
    var title = "";
    var url = '';
    if (formType == "edit") {
        url = "/Page/EditForm.html?moduleId=" + moduleId + "&gridId=" + gridId;
        url += "&page=edit&id=" + row["Id"];
        title = "编辑" + moduleName;
    }
    else {
        url = "/Page/ViewForm.html?moduleId=" + moduleId + "&gridId=" + gridId;
        url += "&page=view&id=" + row["Id"];
        title = "查看" + moduleName;
    }
    if (option && option.titleKey) {
        title += "－" + row[option.titleKey];
    }
    if (isNfm) {
        url += '&nfm=1';
    }
    url += "&r=" + Math.random();
    var h = 250;
    var w = 550;
    var editH = option.editHeight ? option.editHeight + 35 + 50 : 0;
    var editW = option.editWidth ? option.editWidth : 0;
    if (option && editH > 250) {
        h = editH > 550 ? 550 : editH;
    }
    if (option && editW > 550) {
        w = editW > 900 ? 900 : editW;
    }
    ddv.panel({
        title: title,
        height: h,
        width: w,
        content: CreateIFrame(url),
        onLoad: function () {
            gridObj.datagrid('fixDetailRowHeight', index);
        }
    });
    ddv.attr("formType", formType);
    gridObj.datagrid('fixDetailRowHeight', index);
}

//网格行编辑表单格式化
//gridId:当前网格domId
//moduleId:当前模块Id
//moduleName:模块名称
//index:行索引编号
//row:行对象
function GridFormDeailFormatter(gridId, moduleId, moduleName, index, row) {
    return "<div class=\"ddv\"></div>";
}

//明细编辑或明细查看时网格行格式化
//gridId:当前网格domId
//moduleId:当前模块Id
//moduleName:模块名称
//index:行索引编号
//row:行对象
function DetailGridRowFormatter(gridId, moduleId, moduleName, index, row) {
    var divId = "div_" + moduleId + index;
    var div = "<div id=\"" + divId + "\" class=\"ddv\"></div>";
    if (typeof (OverDetailGridRowFormatter) == "function") {
        var tempDom = OverDetailGridRowFormatter(gridId, moduleId, moduleName, index, row);
        if (tempDom)
            return tempDom;
    }
    return div;
}

//绑定网格编辑器控件事件
//gridId:当前网格domId
//rowIndex:行索引编号
//moduleId:当前模块Id
//moduleName:模块名称
//isEditRow:是否编辑行，否则为新增行
function BindGridControlEditorEvent(gridId, rowIndex, moduleId, moduleName, isEditRow) {
    //绑定控件事件
    //获取当前所有列
    var columns = GetGridColumns(gridId);
    if (columns != null && columns.length > 0) {
        var foreignNameFields = $('#' + gridId).attr('foreignNameFields');
        var foreignNameFieldsToken = foreignNameFields ? foreignNameFields.split(',') : [];
        for (var i = 0; i < columns.length; i++) {
            var field = columns[i];
            if (field.editor == null) continue;
            var f = field.field;
            var editor = GetGridEditorControl2(gridId, rowIndex, f);
            if (editor == null) continue;
            editor.attr('fieldName', f);
            var moduleInfo = { moduleId: moduleId, moduleName: moduleName };
            try {
                switch (field.editor.type) {
                    case 'checkbox':
                        {
                            editor.change(function () {
                                var tempField = $(this).attr('fieldName');
                                var newValue = $(this).attr('checked') == 'checked' ? true : false;
                                var oldValue = !newValue;
                                $(this).val(newValue ? '1' : '0');
                                if (typeof (OverOnFieldValueChanged) == "function") {
                                    OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, rowIndex);
                                }
                            });
                            editor.click(function () {
                                var tempField = $(this).attr('fieldName');
                                var fv = $(this).attr('checked') == 'checked' ? true : false;
                                $(this).val(fv ? '1' : '0');
                            });
                        }
                        break;
                    case 'numberbox':
                        {
                            editor.numberbox({
                                onChange: function (newValue, oldValue) {
                                    var tempField = $(this).attr('fieldName');
                                    if (typeof (OverOnFieldValueChanged) == "function") {
                                        OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, rowIndex);
                                    }
                                }
                            });
                            if (isEditRow) {
                                var row = GetRow(gridId, rowIndex);
                                editor.numberbox('setValue', row[f]);
                            }
                        }
                        break;
                    case 'combobox':
                        {
                            var valueField = field.editor.options.valueField;
                            var textField = field.editor.options.textField;
                            editor.combobox({
                                onChange: function (newValue, oldValue) {
                                    var tempField = $(this).attr('fieldName');
                                    if (foreignNameFields && foreignNameFields.indexOf(tempField) > -1) {
                                        for (var j = 0; j < foreignNameFieldsToken.length; j++) {
                                            if (foreignNameFieldsToken[j] == tempField && tempField.lastIndexOf('Name') > -1) {
                                                var foreignIdField = tempField.substr(0, tempField.length - 4) + "Id";
                                                UpdateRowFieldValue(gridId, rowIndex, foreignIdField, newValue);
                                                break;
                                            }
                                        }
                                    }
                                    if (typeof (OverOnFieldValueChanged) == "function") {
                                        OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, rowIndex);
                                    }
                                },
                                onSelect: function (record) {
                                    var tempField = $(this).attr('fieldName');
                                    if (typeof (OverOnFieldSelect) == "function") {
                                        OverOnFieldSelect(record, tempField, valueField, textField, rowIndex);
                                    }
                                }
                            });
                            if (isEditRow) {
                                if (foreignNameFields && foreignNameFields.indexOf(f) > -1) { //外键字段处理
                                    for (var j = 0; j < foreignNameFieldsToken.length; j++) {
                                        if (foreignNameFieldsToken[j] == f && f.lastIndexOf('Name') > -1) {
                                            var row = GetRow(gridId, rowIndex);
                                            var foreignIdField = f.substr(0, f.length - 4) + "Id";
                                            editor.combobox('setValue', row[foreignIdField]);
                                            break;
                                        }
                                    }
                                }
                                else { //非外键字段处理
                                    var row = GetRow(gridId, rowIndex);
                                    editor.combobox('setValue', row[f]);
                                }
                            }
                        }
                        break;
                    case 'textarea':
                    case 'textbox':
                        {
                            editor.textbox({
                                onChange: function (newValue, oldValue) {
                                    var tempField = $(this).attr('fieldName');
                                    if (foreignNameFields && foreignNameFields.indexOf(tempField) > -1) {
                                        for (var j = 0; j < foreignNameFieldsToken.length; j++) {
                                            if (foreignNameFieldsToken[j] == tempField && tempField.lastIndexOf('Name') > -1) {
                                                var foreignIdField = tempField.substr(0, tempField.length - 4) + "Id";
                                                if (newValue && newValue.length == 36) {
                                                    UpdateRowFieldValue(gridId, rowIndex, foreignIdField, newValue);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    if (typeof (OverOnFieldValueChanged) == "function") {
                                        OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, rowIndex);
                                    }
                                }
                            });
                            if (isEditRow) {
                                if (foreignNameFields && foreignNameFields.indexOf(f) > -1) { //外键字段
                                    for (var j = 0; j < foreignNameFieldsToken.length; j++) {
                                        if (foreignNameFieldsToken[j] == f && f.lastIndexOf('Name') > -1) {
                                            var row = GetRow(gridId, rowIndex);
                                            var foreignIdField = f.substr(0, f.length - 4) + "Id";
                                            editor.textbox('setValue', row[foreignIdField]);
                                            editor.textbox('setText', row[f]);
                                            break;
                                        }
                                    }
                                }
                                else { //非外键字段
                                    var row = GetRow(gridId, rowIndex);
                                    editor.textbox('setValue', row[f]);
                                }
                            }
                        }
                        break;
                    case 'datebox':
                        {
                            editor.datebox({
                                onChange: function (newValue, oldValue) {
                                    var tempField = $(this).attr('fieldName');
                                    if (typeof (OverOnFieldValueChanged) == "function") {
                                        OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, rowIndex);
                                    }
                                }
                            });
                            if (isEditRow) {
                                var row = GetRow(gridId, rowIndex);
                                editor.datebox('setValue', row[f]);
                            }
                        }
                        break;
                    case 'datetimebox':
                        {
                            editor.datetimebox({
                                onChange: function (newValue, oldValue) {
                                    var tempField = $(this).attr('fieldName');
                                    if (typeof (OverOnFieldValueChanged) == "function") {
                                        OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, rowIndex);
                                    }
                                }
                            });
                            if (isEditRow) {
                                var row = GetRow(gridId, rowIndex);
                                editor.datetimebox('setValue', row[f]);
                            }
                        }
                        break;
                    case 'combotree':
                        {
                            var valueField = field.editor.options.valueField;
                            var textField = field.editor.options.textField;
                            editor.combotree({
                                onChange: function (newValue, oldValue) {
                                    var tempField = $(this).attr('fieldName');
                                    if (foreignNameFields && foreignNameFields.indexOf(tempField) > -1) {
                                        for (var j = 0; j < foreignNameFieldsToken.length; j++) {
                                            if (foreignNameFieldsToken[j] == tempField && tempField.lastIndexOf('Name') > -1) {
                                                var foreignIdField = tempField.substr(0, tempField.length - 4) + "Id";
                                                UpdateRowFieldValue(gridId, rowIndex, foreignIdField, newValue);
                                                break;
                                            }
                                        }
                                    }
                                    if (typeof (OverOnFieldValueChanged) == "function") {
                                        OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, rowIndex);
                                    }
                                },
                                onSelect: function (record) {
                                    if (typeof (OverOnFieldSelect) == "function") {
                                        var tempField = $(this).attr('fieldName');
                                        OverOnFieldSelect(record, tempField, valueField, textField, rowIndex);
                                    }
                                }
                            });
                            if (isEditRow) {
                                if (foreignNameFields && foreignNameFields.indexOf(f) > -1) { //外键字段处理
                                    for (var j = 0; j < foreignNameFieldsToken.length; j++) {
                                        if (foreignNameFieldsToken[j] == f && f.lastIndexOf('Name') > -1) {
                                            var row = GetRow(gridId, rowIndex);
                                            var foreignIdField = f.substr(0, f.length - 4) + "Id";
                                            editor.combotree('setValue', row[foreignIdField]);
                                            break;
                                        }
                                    }
                                }
                                else { //非外键字段处理
                                    var row = GetRow(gridId, rowIndex);
                                    editor.combotree('setValue', row[f]);
                                }
                            }
                        }
                        break;
                }
            } catch (e) { }
        }
    }
}

//明细编辑或明细查看时网格行展开事件
//gridId:当前网格domId
//moduleId:当前模块Id
//moduleName:模块名称
//row:行对象
//index:行索引编号
//dic:各编辑行字段
//minWidth:下拉展开网格最小宽
//maxWidth:下拉展开网格最大宽
function DetailExpandGridRow(gridId, moduleId, moduleName, row, index, dic, minWidth, maxWidth) {
    var obj = eval("(" + decodeURIComponent(dic) + ")");
    var div = $('#div_' + moduleId + index);
    if (typeof (OverDetailExpandGridRow) == "function") {
        OverDetailExpandGridRow(div, gridId, moduleId, moduleName, row, index, dic);
        return;
    }
    var moduleInfo = { moduleId: moduleId, moduleName: moduleName };
    if (obj) {
        var dgObj = $('#' + gridId);
        for (var i in obj) {
            var tbId = 'tb_' + moduleId + i + index;
            div.append('<table id="' + tbId + '" class="ddv" style="width:' + maxWidth + 'px"></table>');
        }
        setTimeout(function () {
            for (var i in obj) {
                var tbId = 'tb_' + moduleId + i + index;
                var tbObj = $('#' + tbId);
                var dc = [];
                for (var j = 0; j < obj[i].length; j++) {
                    if (obj[i][j].formatter) {
                        try {
                            obj[i][j].formatter = eval("(" + obj[i][j].formatter + ")");
                        } catch (e) { }
                    }
                    if (obj[i][j].editor) {
                        try {
                            obj[i][j].editor = eval("(" + obj[i][j].editor + ")");
                        } catch (e) { }
                    }
                }
                dc.push(obj[i]);
                var data = [];
                data.push(row);
                tbObj.datagrid({
                    data: data,
                    singleSelect: true,
                    rownumbers: false,
                    height: 'auto',
                    columns: dc,
                    onResize: function () {
                        dgObj.datagrid('fixDetailRowHeight', index);
                    },
                    onLoadSuccess: function () {
                        var tempTbObj = $(this);
                        var tbId = tempTbObj.attr('id');
                        setTimeout(function () {
                            dgObj.datagrid('fixDetailRowHeight', index);
                        }, 0);
                        if (page == 'edit') {
                            tempTbObj.datagrid('beginEdit', 0);
                            //绑定控件事件
                            //获取当前所有列
                            var columns = GetGridColumns(tbId);
                            if (columns != null && columns.length > 0) {
                                for (var i = 0; i < columns.length; i++) {
                                    var field = columns[i];
                                    if (field.editor == null) continue;
                                    var f = field.field;
                                    var editor = GetGridEditorControl2(tbId, 0, f);
                                    if (editor == null) continue;
                                    editor.attr('fieldName', f);
                                    switch (field.editor.type) {
                                        case 'checkbox':
                                            {
                                                editor.change(function () {
                                                    var value = $(this).attr('checked') == 'checked' ? '1' : '0';
                                                    var tempField = $(this).attr('fieldName');
                                                    UpdateRowFieldValue(gridId, index, tempField, value);
                                                    if (typeof (OverOnFieldValueChanged) == "function") {
                                                        var oldValue = $(this).attr('checked') == 'checked' ? '0' : '1';
                                                        OverOnFieldValueChanged(moduleInfo, tempField, value, oldValue, index);
                                                    }
                                                });
                                            }
                                            break;
                                        case 'numberbox':
                                            {
                                                editor.numberbox({
                                                    onChange: function (newValue, oldValue) {
                                                        var tempField = $(this).attr('fieldName');
                                                        UpdateRowFieldValue(gridId, index, tempField, newValue);
                                                        if (typeof (OverOnFieldValueChanged) == "function") {
                                                            OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, index);
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                        case 'combobox':
                                            {
                                                var valueField = field.editor.options.valueField;
                                                var textField = field.editor.options.textField;
                                                editor.combobox({
                                                    onChange: function (newValue, oldValue) {
                                                        var tempField = $(this).attr('fieldName');
                                                        UpdateRowFieldValue(gridId, index, tempField, newValue);
                                                        if (typeof (OverOnFieldValueChanged) == "function") {
                                                            OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, index);
                                                        }
                                                    },
                                                    onSelect: function (record) {
                                                        if (typeof (OverOnFieldSelect) == "function") {
                                                            var tempField = $(this).attr('fieldName');
                                                            OverOnFieldSelect(record, tempField, valueField, textField, index);
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                        case 'textarea':
                                        case 'textbox':
                                            {
                                                editor.textbox({
                                                    onChange: function (newValue, oldValue) {
                                                        var tempField = $(this).attr('fieldName');
                                                        UpdateRowFieldValue(gridId, index, tempField, newValue);
                                                        if (typeof (OverOnFieldValueChanged) == "function") {
                                                            OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, index);
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                            break;
                                        case 'datebox':
                                            {
                                                editor.datebox({
                                                    onChange: function (newValue, oldValue) {
                                                        var tempField = $(this).attr('fieldName');
                                                        UpdateRowFieldValue(gridId, index, tempField, newValue);
                                                        if (typeof (OverOnFieldValueChanged) == "function") {
                                                            OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, index);
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                        case 'datetimebox':
                                            {
                                                editor.datetimebox({
                                                    onChange: function (newValue, oldValue) {
                                                        var tempField = $(this).attr('fieldName');
                                                        UpdateRowFieldValue(gridId, index, tempField, newValue);
                                                        if (typeof (OverOnFieldValueChanged) == "function") {
                                                            OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, index);
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                        case 'combotree':
                                            {
                                                var valueField = field.editor.options.valueField;
                                                var textField = field.editor.options.textField;
                                                editor.combotree({
                                                    onChange: function (newValue, oldValue) {
                                                        var tempField = $(this).attr('fieldName');
                                                        UpdateRowFieldValue(gridId, index, tempField, newValue);
                                                        if (typeof (OverOnFieldValueChanged) == "function") {
                                                            OverOnFieldValueChanged(moduleInfo, tempField, newValue, oldValue, index);
                                                        }
                                                    },
                                                    onSelect: function (record) {
                                                        if (typeof (OverOnFieldSelect) == "function") {
                                                            var tempField = $(this).attr('fieldName');
                                                            OverOnFieldSelect(record, tempField, valueField, textField, index);
                                                        }
                                                    }
                                                });
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    },
                    onSelect: function (rowIndex, rowData) {
                        $(this).datagrid('unselectAll');
                    }
                });
            }
            var fields = GetGridColumns(gridId);
            if (fields != null) {
                dgObj.datagrid('autoSizeColumn', fields[0].field); //修正列宽
            }
        }, 20);
    }
}

//主网格页面行内明细或附属视图模块标签时触发事件
//title:tab标题
//index:tab序号
function ExpandRowTabSelected(title, index) {
    var target = this;
    var panelDom = $(target).tabs('getSelected');
    var iframe = $("iframe", panelDom);
    var src = iframe.attr("src");
    var url = iframe.attr("url");
    if (url && (!src || src.length == 0)) {
        iframe.attr("src", url);
    }
}

//主网格页面展开选择附属明细模块标签时触发事件
//title:tab标题
//index:tab序号
function AttachTabSelected(title, index) {
    var target = this;
    var row = GetSelectRow();
    var tab = $(target).tabs('getSelected');
    LoadAttachModuleData(tab, row);
}

//加载附属模块数据
//tab:tab
//row:行记录
//isForce:是否强制刷新
function LoadAttachModuleData(tab, row, isForce) {
    if (!tab || !row || !row["Id"]) return;
    var gridObj = tab.find("table[id^='grid_']");
    var currFlagId = gridObj.attr('flagId');
    if (!isForce) {
        if (currFlagId == row["Id"]) return;
    }
    var url = gridObj.attr('baseUrl');
    var condition = GetQueryStringByUrl(url, 'condition');
    if (condition && condition.length > 0) {
        var replaceCondition = decodeURIComponent(condition).replace('{Id}', row["Id"]);
        url = url.replace(condition, encodeURIComponent(replaceCondition));
        if (isNfm) {
            url += '&nfm=1';
        }
        ReloadData(gridObj.attr('id'), url);
        gridObj.attr('flagId', row["Id"]);
    }
}

//行选中事件
//girdId:网格domId
//moduleId:模块Id
//moduleName:模块名称
//rowIndex:行索引
//rowData:行数据
function OnSelect(rowIndex, rowData, gridId, moduleId, moduleName) {
    if (page == "grid" && gridId == "mainGrid") {
        //加载网格下方附属模块和明细模块
        var tabObj = $('#detailTabs');
        if (tabObj.length > 0) {
            var selectTab = tabObj.tabs('getSelected');
            LoadAttachModuleData(selectTab, rowData);
        }
        var flowStatus = rowData["FlowStatus"]; //流程状态
        var gridToolbar = $('#grid_toolbar_' + moduleId);
        if (flowStatus == undefined || flowStatus == null || flowStatus == 0 || flowStatus == -1) { //当前流程状态为未提交或自定义
            gridToolbar.find('#btnStartFlow').show(); //提交按钮可见
            if ($('#btnRowSave').length == 0) {
                gridToolbar.find('#btnEdit').show(); //编辑按钮可见
            }
            gridToolbar.find('#btnDelete').show(); //删除按钮可见
        }
        else { //流程已启动
            gridToolbar.find('#btnStartFlow').hide(); //提交按钮不可见
            if ($('#btnRowSave').length == 0) {
                gridToolbar.find('#btnEdit').hide(); //编辑按钮可见
            }
            gridToolbar.find('#btnDelete').hide(); //删除按钮可见
        }
        if (flowStatus == 4) { //流程拒绝
            gridToolbar.find('#btnReStartFlow').show(); //重新发起按钮可见
        }
        else {
            gridToolbar.find('#btnReStartFlow').hide(); //重新发起按钮不可见
        }
        //当工具栏搜索框被挤到下方时处理
        CorrectGridHeight(false);
    }
    else if ((page == 'view' || page == 'grid') && gridId.indexOf('grid') > -1) {
        var flowStatus = rowData["FlowStatus"]; //流程状态
        var gridToolbar = $('#grid_toolbar_' + moduleId);
        if (flowStatus == undefined || flowStatus == null || flowStatus == 0 || flowStatus == -1) { //当前流程状态为未提交或自定义
            if ($('#btnRowSave').length == 0) {
                gridToolbar.find('#btnEdit').show(); //编辑按钮可见
                gridToolbar.find('#btnAdd').show(); //新增按钮可见
            }
            gridToolbar.find('#btnDelete').show(); //删除按钮可见
        }
        else { //流程已启动
            if ($('#btnRowSave').length == 0) {
                gridToolbar.find('#btnEdit').hide(); //编辑按钮可见
                gridToolbar.find('#btnAdd').hide(); //新增按钮不可见
            }
            gridToolbar.find('#btnDelete').hide(); //删除按钮可见
        }
        if (page == 'view') {
            if ($('#div_ApprovalList').length > 0 && $('#div_ApprovalList').attr('childflow') == '1') {
                if ($('#regon_' + moduleId).length > 0 && $('#regon_' + moduleId).attr('isflow') == '1') {
                    $('#div_ApprovalList').show();
                    LoadFlowChart(moduleId, rowData['Id'], null, rowData); //加载子流程审批记录和流程图
                }
                else {
                    $('#div_ApprovalList').hide();
                }
            }
        }
    }
    else if (page == "edit" && rowData && rowData["TaskToDoId"]) {
        //子流程审批页面，选择明细记录后，加载当前明细的审批记录和流程图
        LoadFlowChart(moduleId, rowData['Id'], rowData['TaskToDoId'], rowData);
    }
    if (typeof (OverOnSelect) == "function") {
        OverOnSelect(rowIndex, rowData, gridId, moduleId, moduleName);
    }
}

//网格数据加载完成
//data:数据
//girdId:网格domId
//moduleId:模块Id
//moduleName:模块名称
function OnLoadSuccess(data, gridId, moduleId, moduleName) {
    //行操作按钮处理
    InitRowOpBtns(gridId);
    var gridObj = $("#" + gridId);
    //明细网格
    if (page == 'add' || page == 'edit' || page == 'view') {
        var tabsDom = $("#detailTab");
        if (tabsDom.length > 0) {
            var w = tabsDom.width();
            ResizeGrid("grid_" + moduleId, w);
            if (page == 'edit') { //主从编辑页面
                var detailEditMode = gridObj.attr('editMode');
                if (detailEditMode == 3) { //列表行编辑模式
                    //是否为行展开编辑模式
                    var isRowExpand = gridObj.parent().find("td[field='_expander']").length > 0;
                    //默认设置所有行为编辑状态
                    var detailRows = GetCurrentRows(gridId);
                    for (var i = 0; i < detailRows.length; i++) {
                        var r = detailRows[i];
                        var rowIndex = GetRowIndexByRow(gridId, r);
                        EditRow(gridId, rowIndex, moduleId, moduleName);
                    }
                    if (isRowExpand) { //为行展开编辑模式，默认展开
                        setTimeout(function () {
                            for (var i = 0; i < detailRows.length; i++) {
                                var r = detailRows[i];
                                var rowIndex = GetRowIndexByRow(gridId, r);
                                var td = GetGridSysEditorTdCell(rowIndex, '_expander', gridObj.parent());
                                if (td && td.length > 0) {
                                    td.find('span.datagrid-row-expander').click();
                                }
                            }
                        }, 100);
                    }
                }
                //明细子流程审批时加载明细流程图
                if ($("a[id^='flowBtn_']").length > 0 && $("a[id^='flowBtn_']").eq(0).attr('parentToDoId')) {
                    if (data && data.rows && data.rows.length > 0) {
                        var tempRow = data.rows[0];
                        var todoId = tempRow.TaskToDoId;
                        var recordId = tempRow.Id;
                        LoadFlowChart(moduleId, recordId, todoId, tempRow);
                    }
                }
            }
            else if (page == 'view') { //主从查看页面
                if ($('#div_ApprovalList').length > 0 && $('#div_ApprovalList').attr('childflow') == '1') {
                    var tempRow = null;
                    if (data && data.rows && data.rows.length > 0) {
                        tempRow = data.rows[0];
                    }
                    if (tempRow != null && $('#regon_' + moduleId).length > 0 && $('#regon_' + moduleId).attr('isflow') == '1') {
                        $('#div_ApprovalList').show();
                        LoadFlowChart(moduleId, tempRow['Id'], null, tempRow); //加载子流程审批记录和流程图
                    }
                    else {
                        $('#div_ApprovalList').hide();
                    }
                }
            }
        }
    }
    $("img.editFlag").attr("title", "单击编辑字段值");
    //鼠标移入单元格时显示字段编辑图标
    $(".datagrid-row td").mouseover(function (e) {
        $("img.editFlag", $(this)).show();
    }).mouseout(function (e) {
        $("img.editFlag", $(this)).hide();
    });
    if (page == 'grid' && gridId == 'mainGrid') {
        $("input.datagrid-filter[type='checkbox']").click(function () {
            if ($(this).attr("checked")) {
                $(this).attr("value", "1");
            }
            else {
                $(this).attr("value", "0");
            }
        });
        if (gridObj.attr("enableFilter") == "true") { //启用了行过滤功能
            gridObj.removeAttr("enableFilter");
            EnableRowFilter();
        }
        //有附属模块或明细
        if ($("div[id^='region_south']").length > 0 && $("div[id^='region_south']").css('display') != 'none') {
            var rows = GetCurrentRows(gridId);
            if (rows && rows.length > 0) {
                gridObj.datagrid("selectRow", 0);
            }
            if (data && data.rows && data.rows.length > 0) {
                var newh = 26 + 27 + data.rows.length * 26 + 25 + 33;
                var groupTitle = $('div.datagrid-group');
                if (groupTitle.length > 0)
                    newh += 28 * groupTitle.length;
                ResizeGrid(gridId, null, newh);
            }
        }
        //当工具栏搜索框被挤到下方时处理
        CorrectGridHeight(true);
        CorrectGridHeight(false); //这里解决加载tab后，切换tab后当前页面变形的处理
    }
    gridObj.attr('total', data.total);
    //流程图标tooltip处理
    $('img.flowImg').each(function (i, item) {
        BindFlowTips(item);
    });
    $('span.pagination-load').parent().parent().attr('title', '刷新');
    if (data.total <= 0) {
        var view2 = gridObj.data().datagrid.dc.view2;
        if (view2.find('tr.datagrid-filter-row').length == 0) { //未启用行过滤功能
            var dataBody = gridObj.data().datagrid.dc.body2;
            dataBody.html('<div style="width:100%;text-align:center;height:50px;line-height:50px;color:#BEBEBE">没有相关记录</div>');
        }
    }
    //调用重写函数
    if (typeof (OverOnLoadSuccess) == 'function') {
        OverOnLoadSuccess(data, gridId, moduleId, moduleName);
    }
}

//网格数据过滤事件
//data:数据
//girdId:网格domId
//moduleId:模块Id
//moduleName:模块名称
function OnGridLoadFilter(data, gridId, moduleId, moduleName) {
    if (typeof (OverOnGridLoadFilter) == "function") {
        return OverOnGridLoadFilter(data, gridId, moduleId, moduleName);
    }
    return data;
}

//初始化行操作按钮（行编辑时）
//gridId:网格domId
function InitRowOpBtns(gridId) {
    var loadOkCancel = false;
    //生成行操作按钮函数
    var createRowOpBtn = function () {
        $("div[id^='rowOperateDiv_'] a[rowOperateBtn='1']").each(function (i, item) {
            var moduleId = $(item).attr("moduleId");
            var recordId = $(item).attr("recordId"); //记录Id
            var editMode = parseInt($(item).attr("editMode"));
            var tag = moduleId + "_" + recordId;
            $(item).attr('gridId', gridId);
            var tempMeStr = $(item).attr('clickMethod');
            if (tempMeStr && tempMeStr.length > 0) {
                tempMeStr = tempMeStr.replace("(this)", "");
                if (tempMeStr == "Add" || tempMeStr == "Edit")
                    loadOkCancel = true;
            }
            $(item).linkbutton({
                iconCls: $(item).attr('icon'),
                onClick: function (e) {
                    var methodStr = $(item).attr('clickMethod');
                    if (methodStr != undefined && methodStr.length > 0) {
                        var tempMethodStr = methodStr.replace("(this)", "");
                        if (tempMethodStr == "Edit" && editMode == 3) {
                            $("#rowOperateDiv_" + tag).hide();
                            $("#rowOkDiv_" + tag).show();
                            createSaveBtn();
                            createCancelBtn();
                        }
                        else if (tempMethodStr == "Add" && editMode == 3) {
                            //一次只能添加一行，新增行保存后才能再增加
                            var hasNew = false;
                            var gridObj = $('#' + gridId);
                            var rows = gridObj.datagrid("getRows");
                            for (var i = 0; i < rows.length; i++) {
                                var row = rows[i];
                                if (!row["Id"]) {
                                    hasNew = true;
                                    break;
                                }
                            }
                            if (hasNew) { //存在新增行，返回
                                return;
                            }
                        }
                        eval('(' + methodStr + ')');
                        StopBubble(e);
                    }
                },
                plain: true
            });
            var btnText = $(item).attr('btnText');
            if (btnText != undefined && btnText) {
                //$(item).tooltip({ content: btnText });
                $(item).attr('title', btnText);
            }
        });
    }
    //生成保存按钮函数
    var createSaveBtn = function () {
        $("a[id^='rowOkBtn_']").each(function (i, item) {
            var moduleId = $(item).attr("moduleId");
            var moduleName = $(item).attr("moduleName");
            var recordId = $(item).attr("recordId"); //记录Id
            var tag = moduleId + "_" + recordId;
            $(item).attr('gridId', gridId);
            $(item).linkbutton({
                iconCls: 'eu-p2-icon-save',
                onClick: function (e) {
                    RowEditSave(moduleId, moduleName, recordId, gridId, function (result) {
                        if (recordId && recordId.length == 36) { //编辑状态
                            if (!result.Success) {
                                $("#rowOkDiv_" + tag).show();
                                $("#rowOperateDiv_" + tag).hide();
                            }
                        }
                    });
                    StopBubble(e);
                },
                plain: true
            });
            $(item).attr('title', '保存');
        });
    }
    //生成取消按钮函数
    var createCancelBtn = function () {
        $("a[id^='rowCancelBtn_']").each(function (i, item) {
            var moduleId = $(item).attr("moduleId");
            var recordId = $(item).attr("recordId"); //记录Id
            var tag = moduleId + "_" + recordId;
            $(item).attr('gridId', gridId);
            $(item).linkbutton({
                iconCls: 'eu-p2-icon-cancel',
                onClick: function (e) {
                    RowEditCancel(recordId, gridId, function () {
                        $("#rowOkDiv_" + tag).hide();
                        $("#rowOperateDiv_" + tag).show();
                        createRowOpBtn();
                    });
                    StopBubble(e);
                },
                plain: true
            });
            $(item).attr('title', '取消编辑');
        });
    }
    //调用函数
    createRowOpBtn();
    if (loadOkCancel) {
        createSaveBtn();
        createCancelBtn();
    }
}

//行编辑保存
//moduleId:模块ID
//moduleName:模块名称
//recordId:记录ID
//gridId:网格domId
//backFun:回调函数
function RowEditSave(moduleId, moduleName, recordId, gridId, backFun) {
    var rowIndex = 0; //行号
    var isEdit = false;
    if (recordId && recordId.length == 36) {
        rowIndex = GetRowIndexByRecordId(gridId, recordId);
        isEdit = true;
    }
    var gridObj = $('#' + gridId);
    var fs = GetGridColumns(gridId);
    var updateFields = []; //要更新的字段
    for (var i = 0; i < fs.length; i++) {
        var f = fs[i].field;
        var editor = GetGridEditorControl2(gridId, rowIndex, f);
        if (editor == null) continue;
        updateFields.push(f);
    }
    EndEditRow(gridId, rowIndex); //结束编辑当前行
    var row = null;
    var rows = gridObj.datagrid('getRows');
    for (var i = 0; i < rows.length; i++) {
        var tempRow = rows[i];
        if (!recordId && !tempRow["Id"]) { //新增行
            row = tempRow;
            break;
        }
        else { //编辑行
            if (tempRow["Id"] == recordId) {
                row = tempRow;
                break;
            }
        }
    }
    if (row != null) {
        var fieldIsOk = false;
        for (var f in row) {
            var column = GetGridColumn(gridId, f);
            if (!column || !column.editor || !column.editor.type)
                continue;
            if (column.editor.type == 'checkbox')
                row[f] = row[f] == '1';
            fieldIsOk = true;
        }
        if (!fieldIsOk) {
            topWin.ShowAlertMsg(msgTitle, '无可用字段！', "error");
            return;
        }
        var msgTitle = '保存提示';
        var formObject = { ModuleId: moduleId, ModuleName: moduleName, ModuleData: JSON.stringify(row) };
        formObject['NeedUpdateFields'] = updateFields;
        var url = "/" + CommonController.Async_Data_Controller + "/SaveData.html";
        $.ajax({
            type: "post",
            url: url,
            data: { formData: $.base64.encode(escape(JSON.stringify(formObject))) },
            beforeSend: function () {
                topWin.OpenWaitDialog('数据保存中...');
            },
            success: function (result) {
                RefreshGrid(gridId);
                topWin.CloseWaitDialog();
                if (result.Success) {
                    if (typeof (backFun) == "function") {
                        backFun(result);
                    }
                    topWin.ShowMsg(msgTitle, '数据保存成功！');
                }
                else {
                    topWin.ShowMsg(msgTitle, result.Message, function () {
                        if (isEdit) {
                            setTimeout(function () {
                                EditRow(gridId, rowIndex);
                                if (typeof (backFun) == "function") {
                                    backFun(result);
                                }
                            }, 20);
                        }
                        else {
                            if (typeof (backFun) == 'function') {
                                backFun(result);
                            }
                        }
                    });
                }
            },
            error: function (err) {
                RefreshGrid(gridId);
                topWin.CloseWaitDialog();
                topWin.ShowAlertMsg(msgTitle, '数据保存失败，服务器异常！', "error", function () {
                    if (isEdit) {
                        setTimeout(function () {
                            EditRow(gridId, rowIndex);
                            if (typeof (backFun) == "function") {
                                backFun({ Success: false, Message: '数据保存失败，服务器异常！' });
                            }
                        }, 20);
                    }
                    else {
                        if (typeof (backFun) == "function") {
                            backFun({ Success: false, Message: '数据保存失败，服务器异常！' });
                        }
                    }
                });
            },
            dataType: "json"
        });
    }
    else {
        RefreshGrid(gridId);
        if (isEdit) {
            setTimeout(function () {
                EditRow(gridId, rowIndex);
                if (typeof (backFun) == "function") {
                    backFun({ Success: false, Message: '数据获取失败！' });
                }
            }, 20);
        }
        else {
            if (typeof (backFun) == "function") {
                backFun({ Success: false, Message: '数据获取失败！' });
            }
        }
    }
}

//行编辑取消编辑
//recordId:记录ID
//gridId:网格domId
//backFun:操作后回调函数
function RowEditCancel(recordId, gridId, backFun) {
    var rowIndex = 0; //行号
    var del = true;
    if (recordId) {
        rowIndex = GetRowIndexByRecordId(gridId, recordId);
        del = false;
    }
    CancelEditRow(gridId, rowIndex, del);
    if (typeof (backFun) == "function") {
        backFun();
    }
}

//单击左侧树结点事件
//node:节点对象
function GridTreeNodeClick(node) {
    if ($('#div_sampleSearch').css('display') == 'none') { //当前为复杂搜索
        var searchBtn = $('#btn_search');
        ComplexSearch(searchBtn);
    }
    else { //当前为简单搜索
        var txtSearcher = $("#txtSearch");
        var name = txtSearcher.searchbox("getName"); //搜索字段
        var value = txtSearcher.searchbox("getValue"); //搜索值
        SimpleSearch(txtSearcher, value, name);
    }
}

//网格左侧树加载成功
function GridTreeLoadSuccess(node, data) {
    var treeDom = $("#gridTree");
    treeDom.tree("collapseAll");
    var roots = treeDom.tree("getRoots"); //展开所有根结点
    if (roots && roots.length > 0) {
        $.each(roots, function (i, root) {
            treeDom.tree("expand", root.target);
        });
    }
}

//网格左侧树加载过滤事件
//data:网格数据
function GridTreeLoadFilter(data) {
    if (typeof (data) == 'string') {
        var tempData = eval("(" + data + ")");
        arr = [];
        arr.push(tempData);
        return arr;
    }
    else if (data instanceof Array) { //是否为数组
        return data;
    }
    else { //为对象
        arr = [];
        arr.push(data);
        return arr;
    }
}

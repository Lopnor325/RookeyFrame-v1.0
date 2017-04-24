//通用控制器名
var CommonController = {
    Data_Controller: "Data",
    Async_Data_Controller: "DataAsync",
    System_Controller: "System",
    Async_System_Controller: "SystemAsync",
    Annex_Controller: "Annex",
    Async_Annex_Controller: "AnnexAsync",
    User_Controller: "User",
    Async_User_Controller: "UserAsync",
    Bpm_Controller: "Bpm",
    Async_Bpm_Controller: "BpmAsync"
};
var host = location.protocol + '//' + location.host;
var defaultBorderColor = '#95B8E7';
var isNfm = GetLocalQueryString("nfm") == '1'; //当前嵌入其他系统页面标识
var topWin = isNfm ? self : top;

$(function () {
    if (isNfm) {
        try {
            if (parent.location.host == location.host)
                topWin = parent;
            if (parent.parent.location.host == location.host)
                topWin = parent.parent;
        } catch (e) { }
    }
    try {
        var color = top.GetBorderColor();
        $("div[name='divImg']").css('border-color', color);
    } catch (err) { }
    try {
        $('.easyui-daterange').dateranges();
    } catch (e) { }
});

//是否需要重新解析页面
function IsNeedParsePage() {
    var browserName = GetBrowserName(); //浏览器名称
    var browserVer = GetVersion(); //浏览器版本
    if (browserName == "ie" && parseInt(browserVer) <= 10) {
        return true;
    }
    return false;
}

//获取当前用户信息
//用法：var user = topWin.GetUserInfo();
function GetUserInfo() {
    if (isNfm || top.location.href == location.href) {
        var json = decodeURIComponent($('#userInfo').val());
        if (json != null && json.length > 0) {
            var user = JSON.parse(json);
            return user;
        }
        return null;
    }
    else {
        return top.GetUserInfo();
    }
}

//获取用户扩展信息集合
//用法：var userExtends = topWin.GetUserExtends();
function GetUserExtends() {
    var user = GetUserInfo();
    if (user && user.ExtendUserObject && user.ExtendUserObject.EmpExtend && user.ExtendUserObject.EmpExtend.length > 0) {
        return user.ExtendUserObject.EmpExtend;
    }
    return null;
}

//获取一条用户扩展信息
//用法：var userExtend = topWin.GetUserExtend();
function GetUserExtend() {
    var userExtend = GetUserExtends();
    if (userExtend) {
        return userExtend[0];
    }
    return null;
}

//重置页面布局
function ParserLayout(domId) {
    if (domId != undefined && domId != null)
        $.parser.parse('#' + domId);
    else
        $.parser.parse();
}

//取边框颜色
function GetBorderColor() {
    try {
        var color = top.$("#tabs .tabs-header").css('border-bottom-color');
        return color;
    } catch (e) { }
    return defaultBorderColor;
}

/*----------------系统公共函数-----------------------*/
//dom: 如:$("#tt"),
//url: 获取tree数据路径
//params：其他参数
function LoadTree(dom, url, params) {
    if (!url) return;
    var isAsync = (url && url.indexOf('async=1') > -1) || (params && params.async == true);
    if (isAsync) {
        dom.attr('async', '1');
        if (url.indexOf('async=1') == -1) {
            if (url.indexOf('&') > -1)
                url += '&async=1';
            else
                url += '?async=1';
        }
    }
    var treeParams = {
        url: url,
        animate: true,
        loadFilter: function (data) {
            if (data) {
                if (typeof (TreeLoadFilter) == "function") {
                    return TreeLoadFilter(data, dom);
                }
                if (typeof (data) == 'string') {
                    var tempData = eval("(" + data + ")");
                    return tempData;
                }
                else {
                    arr = [];
                    arr.push(data);
                    return arr;
                }
            }
            return data;
        },
        onClick: function (node) {
            if (typeof (TreeNodeOnClick) == "function") {
                TreeNodeOnClick(node, dom);
                return;
            }
        },
        onExpand: function (node) {
            if (typeof (TreeNodeExpand) == "function") {
                TreeNodeExpand(node, dom);
                return;
            }
        },
        onCollapse: function (node) {
            if (typeof (TreeNodeCollapse) == "function") {
                TreeNodeCollapse(node, dom);
                return;
            }
        },
        onLoadSuccess: function (node, data) {
            isAsync = (url && url.indexOf('async=1') > -1) || (params && params.async == true);
            if (!isAsync && params && params.IsExpandRoots) {
                dom.tree("collapseAll");
                var roots = dom.tree("getRoots"); //展开所有根结点
                if (roots && roots.length > 0) {
                    $.each(roots, function (i, root) {
                        dom.tree("expand", root.target);
                    });
                }
            }
            if (typeof (TreeOnLoadSuccess) == "function") {
                TreeOnLoadSuccess(node, data, dom);
                return;
            }
        },
        onBeforeSelect: function (node) {
            if (typeof (TreeOnBeforeSelect) == "function") {
                return TreeOnBeforeSelect(node, dom);
            }
            return true;
        }
    };
    if (params) {
        if (params.loadFilter) {
            treeParams.loadFilter = params.loadFilter;
        }
        if (params.onClick) {
            treeParams.onClick = params.onClick;
        }
        if (params.onExpand) {
            treeParams.onExpand = params.onExpand;
        }
        if (params.onCollapse) {
            treeParams.onCollapse = params.onCollapse;
        }
        if (params.onLoadSuccess) {
            treeParams.onLoadSuccess = params.onLoadSuccess;
        }
        if (params.onBeforeSelect) {
            treeParams.onBeforeSelect = params.onBeforeSelect;
        }
        if (params.method) {
            treeParams.method = params.method;
        }
        if (params.animate) {
            treeParams.animate = params.animate;
        }
        if (params.checkbox) {
            treeParams.checkbox = params.checkbox;
        }
        if (params.cascadeCheck) {
            treeParams.cascadeCheck = params.cascadeCheck;
        }
        if (params.onlyLeafCheck) {
            treeParams.onlyLeafCheck = params.onlyLeafCheck;
        }
        if (params.lines) {
            treeParams.lines = params.lines;
        }
        if (params.dnd) {
            treeParams.dnd = params.dnd;
        }
        if (params.data) {
            treeParams.data = params.data;
        }
        if (params.queryParams) {
            treeParams.queryParams = params.queryParams;
        }
        if (params.formatter) {
            treeParams.formatter = params.formatter;
        }
        if (params.onDblClick) {
            treeParams.onDblClick = params.onDblClick;
        }
        if (params.onBeforeLoad) {
            treeParams.onBeforeLoad = params.onBeforeLoad;
        }
        if (params.onLoadError) {
            treeParams.onLoadError = params.onLoadError;
        }
        if (params.onBeforeExpand) {
            treeParams.onBeforeExpand = params.onBeforeExpand;
        }
        if (params.onBeforeCollapse) {
            treeParams.onBeforeCollapse = params.onBeforeCollapse;
        }
        if (params.onBeforeCheck) {
            treeParams.onBeforeCheck = params.onBeforeCheck;
        }
        if (params.onCheck) {
            treeParams.onCheck = params.onCheck;
        }
        if (params.onSelect) {
            treeParams.onSelect = params.onSelect;
        }
        if (params.onContextMenu) {
            treeParams.onContextMenu = params.onContextMenu;
        }
        if (params.onBeforeDrag) {
            treeParams.onBeforeDrag = params.onBeforeDrag;
        }
        if (params.onStartDrag) {
            treeParams.onStartDrag = params.onStartDrag;
        }
        if (params.onStopDrag) {
            treeParams.onStopDrag = params.onStopDrag;
        }
        if (params.onDragEnter) {
            treeParams.onDragEnter = params.onDragEnter;
        }
        if (params.onDragOver) {
            treeParams.onDragOver = params.onDragOver;
        }
        if (params.onDragLeave) {
            treeParams.onDragLeave = params.onDragLeave;
        }
        if (params.onBeforeDrop) {
            treeParams.onBeforeDrop = params.onBeforeDrop;
        }
        if (params.onDrop) {
            treeParams.onDrop = params.onDrop;
        }
        if (params.onBeforeEdit) {
            treeParams.onBeforeEdit = params.onBeforeEdit;
        }
        if (params.onAfterEdit) {
            treeParams.onAfterEdit = params.onAfterEdit;
        }
        if (params.onCancelEdit) {
            treeParams.onCancelEdit = params.onCancelEdit;
        }
        if (params.queryParams) {
            treeParams.queryParams = params.queryParams;
        }
    }
    dom.tree(treeParams);
}

//将常规JSON数据转换成树状数据
//treeData:json数据
//idField:id字段名
//pIdField:父id字段名
//childrenField:children字段名
function ToTreeData(treeData, idField, pIdField, childrenField) {
    var arr = [], hash = {}, len = (treeData || []).length;
    for (var i = 0; i < len; i++) {
        hash[treeData[i][idField]] = treeData[i];
    }
    for (var j = 0; j < len; j++) {
        var node = treeData[j], hashNodes = hash[node[pIdField]];
        if (hashNodes) {
            !hashNodes[childrenField] && (hashNodes[childrenField] = []);
            hashNodes[childrenField].push(node);
        } else {
            arr.push(node);
        }
    }
    return arr;
};

//网格按钮操作前验证
//moduleName:模块名称
//buttonText:操作按钮显示名称
//ids:操作记录集合,多个以逗号分隔
//backFun:验证回调函数
//otherParams:其他参数
//preventMutiClicks:是否防止多次点击
function GridOperateVerify(moduleName, buttonText, ids, backFun, otherParams, preventMutiClicks) {
    //客户端验证
    var errMsg = null;
    //调用自定义网格按钮操作客户端验证方法
    if (typeof (OverrideGridOperateVerify) == "function") {
        errMsg = OverrideGridOperateVerify(moduleName, buttonText, ids);
        if (errMsg && errMsg.length > 0) { //验证不通过
            if (typeof (backFun) == "function") {
                backFun(errMsg);
                return;
            }
        }
    }
    //服务端验证
    $.ajax({
        type: "post",
        url: '/' + CommonController.Async_Data_Controller + '/GridOperateVerify.html',
        data: { moduleName: moduleName, buttonText: escape(buttonText), ids: ids, otherParams: otherParams },
        beforeSend: function () {
            if (preventMutiClicks) {
                topWin.OpenWaitDialog('处理中...');
            }
        },
        success: function (result) {
            if (preventMutiClicks) {
                topWin.CloseWaitDialog();
            }
            if (typeof (backFun) == "function") {
                backFun(result.Message);
            }
        },
        error: function (err) {
            if (preventMutiClicks) {
                topWin.CloseWaitDialog();
            }
            if (typeof (backFun) == "function") {
                backFun("服务端调用异常！");
            }
        },
        dataType: "json"
    });
}

//弹出框、弹出树选择数据，外键选择数据
//obj:外键输入框对象
//backFun:回调函数
//fieldAttr:针对列表行编辑时外键字段属性
function SelectDialogData(obj, backFun, fieldAttr) {
    var url = $(obj).attr("url");
    var fieldName = $(obj).attr("id");
    var foreignModuleId = $(obj).attr("foreignModuleId"); //外键模块Id
    var foreignModuleName = $(obj).attr("foreignModuleName"); //外键模块
    var foreignModuleDisplay = $(obj).attr("foreignModuleDisplay"); //外键模块显示名称
    var isMutiSelect = $(obj).attr('ms') == '1'; //是否多选
    var isTree = $(obj).attr('isTree') == '1';
    var textField = $(obj).attr("textField"); //文本字段
    var valueField = $(obj).attr("valueField"); //值字段
    if (fieldAttr && $(obj).attr("fieldName")) {
        fieldName = $(obj).attr("fieldName");
        if (isMutiSelect)
            fieldName = fieldName.substr(0, fieldName.length - 4);
        else
            fieldName = fieldName.substr(0, fieldName.length - 4) + "Id";
        fieldAttr = eval("(" + unescape(fieldAttr) + ")");
        url = fieldAttr.url;
        foreignModuleId = fieldAttr.foreignModuleId;
        foreignModuleName = decodeURI(fieldAttr.foreignModuleName);
        foreignModuleDisplay = decodeURI(fieldAttr.foreignModuleDisplay);
        isMutiSelect = fieldAttr.ms == '1';
        isTree = fieldAttr.isTree == '1';
        textField = fieldAttr.textField;
        valueField = fieldAttr.valueField;
    }
    if (isTree) {
        textField = 'Name';
        valueField = 'Id';
    }
    if (!url) return;
    if (typeof (OverDialogValidate) == "function") { //外键字段弹出前先验证
        var errMsg = OverDialogValidate(fieldName, foreignModuleName); //验证返回错误消息
        if (errMsg && errMsg.length > 0) {
            topWin.ShowMsg('提示', errMsg);
            return;
        }
    }
    if (typeof (OverGetDialogCondition) == "function") { //条件重写方法
        var condition = OverGetDialogCondition(fieldName, foreignModuleName);
        if (condition && condition.length > 0)
            url += "&condition=" + escape(condition);
    }
    if (typeof (OverGetDialogWhere) == "function") { //where语句重写方法
        var where = OverGetDialogWhere(fieldName, foreignModuleName);
        if (where && where.length > 0)
            url += "&where=" + $.base64.encode(encodeURI(where));
    }
    if ($(obj).attr('nfp') == '1')
        url += "&nfp=1";
    url += "&r=" + Math.random(); //弹出框页面url
    if (textField == undefined || textField == '') textField = "Name";
    if (valueField == undefined || valueField == '') valueField = "Id";
    var title = "选择数据";
    if (foreignModuleDisplay) {
        title = "选择" + foreignModuleDisplay;
    }
    var toolbar = [{
        text: "选择",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var win = topWin.GetCurrentDialogFrame()[0].contentWindow;
            var rows = isTree ? win.GetSelectData() : (isMutiSelect ? win.GetSelectRows("grid_" + foreignModuleId) :
                      win.GetSelectRow("grid_" + foreignModuleId));
            if (!rows || (isMutiSelect && (!rows || rows.length == 0))) {
                topWin.ShowAlertMsg("提示", '请至少选择一条记录！', 'info');
                return;
            }
            var executeFun = function () {
                if (typeof (backFun) != "function") { //通用处理
                    var txtControl = $(obj);
                    var oldValue = txtControl.textbox("getValue");
                    var vf = '';
                    var tf = '';
                    if (isMutiSelect) { //多选
                        for (var i = 0; i < rows.length; i++) {
                            if (vf) {
                                vf += ',';
                                tf += ',';
                            }
                            vf += rows[i][valueField] ? rows[i][valueField] : '';
                            tf += rows[i][textField] ? rows[i][textField] : '';
                        }
                    }
                    else {
                        vf = rows[valueField];
                        tf = rows[textField];
                    }
                    txtControl.attr('value', vf);
                    txtControl.attr('v', vf);
                    txtControl.textbox("setValue", vf);
                    txtControl.textbox("setText", tf);
                    if (typeof (OnFieldSelect) == "function") { //触发字段选择事件
                        OnFieldSelect(rows, fieldName, valueField, textField);
                    }
                    if (typeof (OnFieldValueChanged) == "function" && oldValue != vf) { //触发字段值改变事件
                        var moduleId = GetLocalQueryString("moduleId");
                        OnFieldValueChanged({ moduleId: moduleId }, fieldName, vf, oldValue);
                    }
                }
                else { //有自定义函数时调用自定义函数
                    backFun(rows, valueField, textField);
                }
                topWin.CloseDialog();
            };
            if (typeof (OverDialogAfterSelectValidate) == "function") { //外键字段弹出选择后先验证
                OverDialogAfterSelectValidate(obj, rows, function (action) {
                    if (action) { //验证通过
                        executeFun();
                    }
                });
            }
            else {
                executeFun();
            }
        }
    }, {
        text: '关闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var w = isTree ? 500 : 800;
    var h = isTree ? 475 : 430;
    topWin.OpenDialog(title, url, toolbar, w, h, 'eu-icon-search');
}

//弹出框选择模块数据
//moduleIdOrName:模块Id或模块名称
//backFun:选择后的回调函数
//condition:过滤条件
//where:过滤where语句
//isMutiSelect:是否多选
//isNfp:是否过滤权限
function SelectModuleData(moduleIdOrName, backFun, condition, where, isMutiSelect, isNfp) {
    if (!moduleIdOrName) {
        topWin.ShowMsg('提示', "模块Id和模块名称至少要传递一个！");
        return;
    }
    var url = null;
    var title = "选择数据";
    if (typeof (moduleIdOrName) == "string" && moduleIdOrName.length < 36) {
        url = "/Page/Grid.html?page=fdGrid&moduleName=" + encodeURI(moduleIdOrName);
        title = "选择" + moduleIdOrName;
    }
    else {
        url = "/Page/Grid.html?page=fdGrid&moduleId=" + moduleIdOrName;
    }
    if (isMutiSelect)
        url += "&ms=1";
    if (condition && condition.length > 0)
        url += "&condition=" + escape(condition);
    if (where && where.length > 0)
        url += "&where=" + $.base64.encode(encodeURI(where));
    if (isNfp)
        url += "&nfp=1";
    url += "&r=" + Math.random(); //弹出框页面url
    var toolbar = [{
        text: "选择",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var win = topWin.GetCurrentDialogFrame()[0].contentWindow;
            var data = isMutiSelect ? win.GetSelectRowsByGridIdPrefix("grid_") : win.GetSelectRowByGridIdPrefix("grid_");
            if (typeof (backFun) == "function") { //有自定义函数时调用自定义函数
                backFun(data);
            }
            topWin.CloseDialog();
        }
    }, {
        text: '关闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    topWin.OpenDialog(title, url, toolbar, 800, 430, 'eu-icon-search');
}

//选择模块树
//moduleIdOrName:模块Id或模块名称
//url:自定义数据加载url
//isMutiSelect:是否多选
//backFun:回调函数
//isAsync:是否异步
//otherParams:其他参数
function SelectModuleTree(moduleIdOrName, url, isMutiSelect, backFun, isAsync, otherParams) {
    if (!moduleIdOrName) {
        topWin.ShowMsg('提示', "模块Id和模块名称至少要传递一个！");
        return;
    }
    var dataUrl = url;
    var title = "选择数据";
    if (typeof (moduleIdOrName) == "string" && moduleIdOrName.length < 36) {
        if (!dataUrl || dataUrl.length == 0) {
            dataUrl = "/Page/DialogTree.html?moduleName=" + encodeURI(moduleIdOrName);
        }
        title = "选择" + moduleIdOrName;
    }
    else {
        if (!dataUrl || dataUrl.length == 0) {
            dataUrl = "/Page/DialogTree.html?moduleId=" + moduleIdOrName;
        }
    }
    if (isMutiSelect) dataUrl += "&ms=1";
    if (isAsync && dataUrl.indexOf('async=1') == -1) {
        dataUrl += "&async=1";
    }
    if (otherParams) {
        if (otherParams.cascadeCheck)
            dataUrl += "&cccheck=1";
        if (otherParams.onlyCanSelectLeaf)
            dataUrl += "&onlyleaf=1";
        if (otherParams.fieldName)
            dataUrl += "&fieldName=" + otherParams.fieldName;
        if (otherParams.hasId == 1)
            dataUrl += "&hasId=1";
    }
    dataUrl += "&r=" + Math.random(); //弹出框页面url
    var toolbar = [{
        text: "选择",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var row = topWin.GetCurrentDialogFrame()[0].contentWindow.GetSelectData();
            if (isMutiSelect) {
                if (row.length == 0) {
                    topWin.ShowMsg('提示', '请至少选择一项！');
                    return;
                }
            }
            else {
                if (row == null) {
                    topWin.ShowMsg('提示', '请选择一项！');
                    return;
                }
            }
            if (typeof (backFun) == "function") { //有自定义函数时调用自定义函数
                backFun(row);
            }
            topWin.CloseDialog();
        }
    }, {
        text: '关闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    topWin.OpenDialog(title, dataUrl, toolbar, 500, 510, 'eu-icon-search');
}

//图标控件的选择图标事件
function SelectIcon(obj) {
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var iconInfo = topWin.GetCurrentDialogFrame()[0].contentWindow.GetSelectedIcon();
            if (iconInfo) { //获取到了图标信息
                var controlId = $(obj).attr("iconControlId");
                $("#" + controlId).val(iconInfo.StyleClassName);
                $("#img_" + controlId).attr("src", iconInfo.ImgUrl);
                topWin.CloseDialog();
            }
        }
    }, {
        id: 'btnChange',
        text: "切换列表",
        iconCls: "eu-icon-grid",
        handler: function (e) {
            var iframe = top.GetCurrentDialogFrame();
            var oldUrl = iframe.attr("src");
            if (oldUrl.indexOf('/Page/IconSelect.html') > -1) {
                topWin.$('#btnChange span.l-btn-text').text('切换图标');
                topWin.$('#btnChange span.l-btn-icon').removeClass('eu-icon-grid').addClass('eu-p2-icon-ext-jpg');
                iframe.attr("src", "/Page/Grid.html?page=otGrid&moduleName=图标管理");
            }
            else {
                topWin.$('#btnChange span.l-btn-text').text('切换列表');
                topWin.$('#btnChange span.l-btn-icon').removeClass('eu-p2-icon-ext-jpg').addClass('eu-icon-grid');
                iframe.attr("src", "/Page/IconSelect.html");
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
    var url = '/Page/IconSelect.html?&r=' + Math.random();
    topWin.OpenDialog('选择图标', url, toolbar, 900, 560, 'eu-icon-grid');
}

//图片上传控件的选择图片事件
function SelectImage(obj) {
    var controlId = $(obj).attr("imgControlId");
    var tempFun = function (imgName) {
        var iframe = $("#iframe_" + controlId); //当前图片上传控件的iframe
        //开始上传到临时目录
        iframe[0].contentWindow.UploadImage(function () { //上传前
            $("#a_" + controlId).linkbutton('disable');
        }, function (result) { //上传完成后
            if (result && result.Success) {
                var filePath = result.FilePath;
                $("#img_" + controlId).attr("src", filePath);
                $("#" + controlId).val(filePath);
            }
            $("#a_" + controlId).linkbutton('enable');
        }, imgName);
    }
    //自定义验证方法
    if (typeof (ImgUploadControlVerify) == "function") {
        ImgUploadControlVerify(controlId, function (imgName) { //验证通过后可返回自定义图片文件名执行上传
            tempFun(imgName);
        });
    }
    else {
        tempFun();
    }
}

//图片上传控件iframe加载完成
//fieldName:当前字段名
function ImgUploadIframeLoaded(fieldName) {
    $("#a_" + fieldName).linkbutton('enable');
}

//调整网格大小
//gridId:网格domId
//w:调整后宽
//h:调整后高
function ResizeGrid(gridId, w, h) {
    var width = w != null && w != undefined ? parseInt(w) : 0;
    var height = h != null && h != undefined ? parseInt(h) : 0;
    if (gridId && (width > 0 || height > 0)) {
        var gridObj = $("#" + gridId);
        var obj = null;
        if (width > 0 && height > 0) {
            obj = { width: width, height: height };
        }
        else if (width > 0) {
            obj = { width: width };
        }
        else {
            obj = { height: height };
        }
        gridObj.datagrid("resize", obj);
    }
}

//从网格中获取最终选择的记录（多条记录）
//obj:当前按钮对象
function GetFinalSelectRows(obj) {
    var gridId = $(obj).attr('gridId');
    if (!gridId) gridId = 'mainGrid';
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
        topWin.ShowMsg("提示", "请选择一条记录！", null, null, 1);
        return null;
    }
    return rows;
}

//从网格中获取最终选择的记录（单条记录）
//obj:当前按钮对象
function GetFinalSelectRow(obj) {
    var gridId = $(obj).attr('gridId');
    if (!gridId) gridId = 'mainGrid';
    var selectId = $(obj).attr("recordId");
    var row = GetRowByRecordId(gridId, selectId);
    if (row)
        return row;
    row = GetSelectRow(gridId);
    if (!row) {
        topWin.ShowMsg("提示", "请选择一条记录！", null, null, 1);
        return null;
    }
    return row;
}

//获取网格数
function GetGridCount() {
    return $('div.datagrid ').length;
}

//json对象绑定到form表单中
//json:JSON数据
//form:表单对象
//formFields:表单字段对象集合
function JsonToForm(json, form, formFields) {
    if (!json || !formFields || !form) return;
    $.each(json, function (key, value) {
        var field = null;
        $.each(formFields, function (i, obj) {
            if (obj.Sys_FieldName == key) {
                field = obj;
                return;
            }
        });
        if (field != null) {
            var controlType = field.ControlType; //字段控件类型
            if (field.CanEdit) { //可编辑字段
                if (controlType == 0 || controlType == 12 || controlType == 15 || controlType == 100) { //文本框或文本域或密码框或标签框
                    if (value == undefined || value == null)
                        value = '';
                    if (controlType == 100) { //标签框
                        if (field.ForeignModuleName && field.ForeignModuleName.length > 0) { //外键模块
                            if (key.substr(key.length - 2) == "Id") { //外键ID值
                                $("#" + key, form).attr('value', value);
                            }
                            else { //外键Name值
                                var controlName = key.substr(0, key.length - 4) + "Id";
                                $("#" + controlName, form).text(value);
                            }
                        }
                        else {
                            $("#" + key, form).attr('value', value).text(value);
                        }
                    }
                    else { //非标签框
                        $("#" + key, form).textbox('setValue', value);
                    }
                }
                else if (controlType == 1 || controlType == 2) { //单选checkbox或多选checkbox
                    $("#" + key, form).attr("checked", value);
                    $("#" + key, form).attr("value", value ? 1 : 0);
                }
                else if (controlType == 3) { //下拉列表
                    $("#" + key, form).combobox('setValue', value);
                }
                else if (controlType == 6 || controlType == 7) { //浮点数值或整形数值
                    $("#" + key, form).numberbox('setValue', value);
                }
                else if (controlType == 8 || controlType == 17) { //外键弹出列表框或外键弹出树
                    if (key.substr(key.length - 2) == "Id") {
                        $("#" + key, form).attr('v', value).attr('value', value);
                    }
                    else {
                        var controlName = key.substr(0, key.length - 4) + "Id";
                        $("#" + controlName, form).textbox('setText', value);
                    }
                }
                else if (controlType == 9) { //单选框组
                }
                else if (controlType == 10) { //日期框
                    $("#" + key, form).datebox('setValue', value);
                }
                else if (controlType == 11) { //日期时间框
                    $("#" + key, form).datetimebox('setValue', value);
                }
                else if (controlType == 13) { //富文本框
                }
                else if (controlType == 14) { //文本框列表
                }
            }
            else { //字段不可编辑
                if (value == undefined || value == null)
                    value = '';
                if (controlType == 8 || controlType == 17) { //外键弹出列表框或外键弹出树
                    if (key.substr(key.length - 2) == "Id") {
                        $("#" + key, form).attr('value', value);
                        $("#" + key, form).attr('v', value);
                    }
                    else {
                        $("#" + key, form).text(value);
                    }
                }
                else {
                    $("#" + key, form).attr('value', value).text(value);
                }
            }
        }
    });
}

//获取当前表单字段
function GetFormFields() {
    try {
        var formFields = eval("(" + decodeURIComponent($("#hd_formFields").val()) + ")");
        return formFields;
    } catch (err) { }
    return null;
}

//获取字段的表单字段信息
//fieldName:字段名
function GetFormField(fieldName) {
    var formFields = GetFormFields();
    if (formFields != null) {
        var field = null;
        $.each(formFields, function (i, obj) {
            if (obj.Sys_FieldName == fieldName) {
                field = obj;
                return;
            }
        });
        return field;
    }
    return null;
}

//获取titleKey字段
function GetTitleKeyField() {
    var formFields = GetFormFields();
    if (formFields != null) {
        var field = null;
        $.each(formFields, function (i, obj) {
            if (obj.IsTitleKey == true) {
                field = obj;
                return;
            }
        });
        return field;
    }
    return null;
}

//编辑字段值
//obj：控件对象
function EditField(obj) {
    //有自定义字段编辑方法则先调用自定义否则调用通用
    if (typeof (OverEditField) == "function") {
        OverEditField(obj);
        return;
    }
    var moduleId = $(obj).attr("moduleId"); //模块Id
    var fieldName = $(obj).attr("fieldName"); //字段名称
    var recordId = $(obj).attr("recordId"); //记录Id
    var fieldDisplay = $(obj).attr("fieldDisplay"); //字段显示名称
    var fieldWidth = parseInt($(obj).attr("fieldWidth")) + 100; //字段宽度
    var oldValue = $(obj).attr("oldValue"); //原始字段值
    if (fieldWidth < 400) fieldWidth = 400;
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var fieldValue = topWin.GetCurrentDialogFrame()[0].contentWindow.GetUpdateFieldValue();
            if (fieldValue == oldValue) {
                topWin.CloseDialog();
                return;
            }
            var msgTitle = "字段更新提示";
            $.ajax({
                type: 'post',
                url: '/' + CommonController.Async_Data_Controller + '/UpdateField.html',
                data: { moduleId: moduleId, recordId: recordId, fieldName: fieldName, fieldValue: typeof (fieldValue) == 'string' ? escape(fieldValue.Replace("\\+", "%20")) : fieldValue },
                dataType: "json",
                beforeSend: function () {
                    topWin.OpenWaitDialog('处理中...');
                },
                success: function (result) {
                    if (result.Success) {
                        topWin.CloseDialog();
                        topWin.ShowMsg(msgTitle, '字段更新成功！', function () {
                            topWin.CloseWaitDialog();
                            var gridId = $(obj).attr("gridId");
                            if (gridId) { //网格页面
                                RefreshGrid(gridId); //刷新网格
                            }
                            else { //查看页面
                                $("#span_" + fieldName).text(result.FieldDisplayValue);
                            }
                        });
                    }
                    else {
                        topWin.ShowAlertMsg(msgTitle, result.Message, "info", function () {
                            topWin.CloseWaitDialog();
                        });
                    }
                },
                error: function (err) {
                    topWin.ShowAlertMsg(msgTitle, "字段更新失败，服务端异常！", "error", function () {
                        top.CloseWaitDialog();
                    });
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
    var url = '/Page/EditField.html?page=editField&moduleId=' + moduleId + '&fieldName=' + fieldName + '&recordId=' + recordId + '&r=' + Math.random();
    topWin.OpenDialog('编辑字段－' + fieldDisplay, url, toolbar, fieldWidth, 280, 'eu-icon-docEdit');
}

//字段绑定自动完成功能
//dom:绑定自动完成功能的输入控件dom
//moduleIdOrModuleName:模块Id或模块名称
//fieldName:需要显示、提示的字段名
//displayTemplate:显示模板html
//backFun:选择值后的回调函数
//paramObj：参数对象，可选参数
//urlParams:额外的url参数
//foreignModuleName:外键模块名
/*
displayTemplate模板使用说明：
如：1.字段格式为{UserName},UserName字段为当前表字段
    <a href="#">{UserName}</a>
    2.字段格式为{用户管理.UserName},UserName字段为外键模块表字段，前面必须带上模块名称
    <a href="#">{用户管理.UserName}--{Des}</a>
*/
function FieldBindAutoCompleted(dom, moduleIdOrModuleName, fieldName, displayTemplate, backFun, paramObj, urlParams, foreignModuleName) {
    if (moduleIdOrModuleName == undefined || moduleIdOrModuleName == null || moduleIdOrModuleName.length == 0)
        return;
    if (dom.attr('bindAuto'))
        return;
    dom.focus(function () {
        if (typeof (OverDialogValidate) == "function") { //外键字段智能提示前先验证
            var errMsg = OverDialogValidate(fieldName, foreignModuleName); //验证返回错误消息
            if (errMsg && errMsg.length > 0) {
                return;
            }
        }
        var autoUrl = "/" + CommonController.Async_Data_Controller + "/AutoComplete.html?fieldName=" + fieldName;
        if (moduleIdOrModuleName.length == 36)
            autoUrl += "&moduleId=" + moduleIdOrModuleName;
        else
            autoUrl += "&moduleName=" + moduleIdOrModuleName;
        if (urlParams && urlParams.length > 0) { //额外参数
            if (urlParams.indexOf("&") != 0) {
                urlParams = "&" + urlParams;
            }
            autoUrl += urlParams;
        }
        if (displayTemplate && displayTemplate.length > 0) { //显示模板
            autoUrl += "&template=" + escape(displayTemplate);
        }
        if (typeof (OverGetDialogCondition) == "function") { //条件重写方法
            var condition = OverGetDialogCondition(fieldName, foreignModuleName);
            if (condition && condition.length > 0)
                autoUrl += "&condition=" + escape(condition);
        }
        if (typeof (OverGetDialogWhere) == "function") { //where语句重写方法
            var where = OverGetDialogWhere(fieldName, foreignModuleName);
            if (where && where.length > 0)
                autoUrl += "&where=" + $.base64.encode(encodeURI(where));
        }
        if (typeof (OverAutoCompleteLoadOtherFields) == "function") { //智能提示时需要加载的其他字段，多个字段以逗号分隔
            var of = OverAutoCompleteLoadOtherFields(fieldName, foreignModuleName);
            if (of && of.length > 0)
                autoUrl += "&of=" + escape(of);
        }
        if (typeof (OverAutoCompleteLoadOtherUrlParams) == "function") { //智能提示时需要加载的其他URL参数
            var otherUrlParams = OverAutoCompleteLoadOtherUrlParams(fieldName, foreignModuleName);
            if (otherUrlParams)
                autoUrl += "&" + otherUrlParams;
        }
        autoUrl += "&r=" + Math.random();
        AutoComplete(dom, autoUrl, function (item, dom, tempParamObj) {
            if (item == null) return;
            if (typeof (backFun) == "function") {
                backFun(dom, item, fieldName, tempParamObj);
            }
        }, false, paramObj);
    });
    dom.attr('bindAuto', '1');
}

//执行模块通用删除
//moduleIdOrName:模块id或模块名称
//ids:要删除的记录id,多个以逗号分隔,如，100,101,102
//isFromRecycle:是否来自回收站
//isHardDel:是否硬删除
//backFun:回调函数
function ExecuteCommonDelete(moduleIdOrName, ids, isFromRecycle, isHardDel, backFun) {
    var delUrl = "/" + CommonController.Async_Data_Controller + "/Delete.html?r=" + Math.random();
    if (isFromRecycle) { //来自回收站
        delUrl += "&recycle=1";
    }
    if (isHardDel) { //确认硬删除
        delUrl += "&isHardDel=1";
    }
    if (isNfm) {
        delUrl += '&nfm=1';
    }
    var msgTitle = "删除提示";
    var params = typeof (moduleIdOrName) == "string" && moduleIdOrName.length < 36 ? { moduleName: decodeURI(moduleIdOrName), ids: ids } :
                 { moduleId: moduleIdOrName, ids: ids };
    $.ajax({
        type: "post",
        url: delUrl,
        data: params,
        beforeSend: function () {
            topWin.OpenWaitDialog('正在删除...');
        },
        success: function (result) {
            if (result.Success) {
                topWin.ShowMsg(msgTitle, '数据删除成功！', function () {
                    topWin.CloseWaitDialog();
                    if (typeof (backFun) == "function") {
                        backFun();
                    }
                });
            }
            else {
                topWin.ShowAlertMsg(msgTitle, result.Message, "info", function () {
                    topWin.CloseWaitDialog();
                });
            }
        },
        error: function (err) {
            topWin.ShowAlertMsg(msgTitle, "操作失败，服务端异常！", "error", function () {
                topWin.CloseWaitDialog();
            });
        },
        dataType: "json"
    });
}

//执行模块通用还原
//moduleIdOrName:模块id或模块名称
//ids:要删除的记录id,多个以逗号分隔,如，100,101,102
//backFun:回调函数
function ExecuteCommonRestore(moduleIdOrName, ids, backFun) {
    var msgTitle = "还原提示";
    var params = typeof (moduleIdOrName) == "string" && moduleIdOrName.length < 36 ? { moduleName: decodeURI(moduleIdOrName), ids: ids } :
                 { moduleId: moduleIdOrName, ids: ids };
    var url = "/" + CommonController.Async_Data_Controller + "/Restore.html?r=" + Math.random();
    if (isNfm) {
        url += '&nfm=1';
    }
    $.ajax({
        type: "post",
        url: url,
        data: params,
        beforeSend: function () {
            topWin.OpenWaitDialog('正在还原...');
        },
        success: function (result) {
            if (result.Success) {
                topWin.ShowMsg(msgTitle, '数据还原成功！', function () {
                    topWin.CloseWaitDialog();
                    if (typeof (backFun) == "function") {
                        backFun();
                    }
                });
            }
            else {
                topWin.ShowAlertMsg(msgTitle, result.Message, "info", function () {
                    topWin.CloseWaitDialog();
                });
            }
        },
        error: function (err) {
            topWin.ShowAlertMsg(msgTitle, "操作失败，服务端异常！", "error", function () {
                topWin.CloseWaitDialog();
            });
        },
        dataType: "json"
    });
}

/*-----------tabs操作相关-----------------------------------------------*/
//增加Tab，并触发onLoad回调函数
//dom:tab对应的dom，为空是为top.$("#tabs");
//title:标题
//url:url
//icon:图标
//onloadFun:加载完成后的回调函数
//repeatTitle:是否可重复title
//backFun:回调函数
function AddTab(dom, title, url, icon, onloadFun, repeatTitle, backFun) {
    if (!isNfm) {
        var tt = dom || top.$("#tabs");
        var isExist = repeatTitle ? false : tt.tabs('exists', title);
        if (!isExist) {
            if (dom == null) {
                var cookieTheme = top.GetCookie('easyuitheme');
                if (cookieTheme == undefined || cookieTheme == null)
                    cookieTheme = top.defaultTheme;
                if (cookieTheme != undefined && cookieTheme) {
                    if (url.indexOf('/Page/Grid.html') == -1) {
                        if (url.indexOf("?") > -1)
                            url += "&";
                        else
                            url += "?";
                        url += "easyuitheme=" + cookieTheme;
                    }
                }
            }
            var optionParams = {
                title: title,
                border: false,
                content: CreateIFrame(url),
                closable: true,
                selected: true,
                icon: icon,
                loadingMessage: '正在加载中......'
            };
            if (dom == null && tt.attr('id') == 'tabs') { //main主页面
                optionParams.onResize = function (width, height) { //tab大小发生改变事件
                    try {
                        var tab = $(this);
                        var iframe = tab.find("iframe:first");
                        var win = iframe[0].contentWindow;
                        if (win.GetGridCount() == 1) {
                            win.ResizeGrid("mainGrid", null, height);
                            top.tabHeightInit();
                        }
                    } catch (ex) { }
                    if (typeof (OverTabResized) == "function") {
                        OverTabResized(tab, width, height);
                    }
                };
            }
            if (typeof (onloadFun) == "function") {
                optionParams.onLoad = onloadFun;
            }
            tt.tabs('add', optionParams);
            if (dom == null && tt.attr('id') == 'tabs') {
                top.tabHeightInit();
            }
            if (typeof (backFun) == "function") {
                backFun();
            }
        }
        else {
            tt.tabs('select', title);
        }
    }
    else if (!dom) { //针对嵌入其他系统的
        url = host + url;
        if (url.indexOf('nfm=1') == -1) {
            if (url.indexOf("?") > -1)
                url += "&";
            else
                url += "?";
            url += "nfm=1";
        }
        RegChildSysMessager(title);
        var msg = "addTab('" + title + "','" + url + "')";
        SendMsgToParent(msg);
    }
}

//关闭当前选中tab
//dom:tab对应的dom
//isFreshTodo:是否需要刷新待办（针对nfm=1)
//isNeedFg:是否需要刷新网格（针对nfm=1)
function CloseTab(dom, isFreshTodo, isNeedFg) {
    try {
        if (!isNfm) {
            var tt = dom || top.$("#tabs");
            var index = tt.tabs('getTabIndex', tt.tabs('getSelected'));
            CloseTabByIndex(dom, index);
        }
        else if (!dom) { //针对嵌入其他系统的
            var ftb = isNeedFg ? GetLocalQueryString("ftb") : ''; //来源tabIndex
            RegChildSysMessager('CloseThisTab');
            var msg = "CloseThisTab('" + ftb + "');";
            if (isFreshTodo)
                msg += "RefreshDeskTodoTab();"
            SendMsgToParent(msg);
        }
    } catch (e) { }
}

//关闭某个tab
//dom:tab对应的dom
function CloseTabItem(dom, tab) {
    try {
        var tt = dom || top.$("#tabs");
        var index = tt.tabs('getTabIndex', tab);
        CloseTabByIndex(dom, index);
    } catch (e) { }
}

//关闭tab
//dom:tab对应的dom
function CloseTabByIndex(dom, index) {
    try {
        var tt = dom || top.$("#tabs");
        if (index > 0) {
            tt.tabs('close', index);
            if (dom == null) {
                top.tabHeightInit();
            }
        }
    } catch (e) { }
}

//关闭tab
//dom:tab对应的dom
function CloseTabByTitle(dom, title) {
    try {
        var tt = dom || top.$("#tabs");
        var tab = tt.tabs('getTab', title);
        CloseTabItem(dom, tab);
    } catch (e) { }
}

//选中tab
//dom:tab对应的dom
//title:tab标题
function SelectTab(dom, title) {
    try {
        var tt = dom || top.$("#tabs");
        tt.tabs('select', title);
    } catch (e) { }
}

//刷新tab
//dom:tab对应的dom
//url:新的链接地址
function RefreshTab(dom, url) {
    UpdateSelectedTab(dom, url);
}

//获取选中的tab
//dom:tab对应的dom
function GetSelectedTab(dom) {
    try {
        var tt = dom || top.$("#tabs");
        var tab = tt.tabs('getSelected');
        return tab;
    } catch (e) { }
    return null;
}

//获取tabindex
//dom:tab对应的dom
//tab:标签对象
function GetTabIndex(dom, tab) {
    try {
        var tt = dom || top.$("#tabs");
        var tabIndex = tt.tabs('getTabIndex', tab);
        return tabIndex;
    } catch (e) { }
    return 0;
}

//dom:tab对应的dom
function GetSelectTabIndex(dom) {
    var tab = GetSelectedTab(dom);
    if (tab)
        return GetTabIndex(dom, tab);
    return 0;
}

//获取tab
//dom:tab对应的dom
//tabIndexOrTitle:tab索引或tab标题
function GetTab(dom, tabIndexOrTitle) {
    try {
        var tt = dom || top.$("#tabs");
        var tab = tt.tabs('getTab', tabIndexOrTitle);
        return tab;
    } catch (e) { }
    return null;
}

//更新tab
//dom:tab对应的dom
//tab:要更新的tab对象
//url:tab的url
//title:新的标题
function UpdateTab(dom, tab, url, title) {
    try {
        var tt = dom || top.$("#tabs");
        var params = {};
        if (url) params.content = CreateIFrame(url);
        if (title) params.title = title;
        tt.tabs('update', {
            tab: tab,
            options: params
        });
    } catch (e) { }
}

//更新当前选中tab
//dom:tab对应的dom
//url:tab的url
//title:新的标题
function UpdateSelectedTab(dom, url, title) {
    try {
        if (!isNfm) {
            var tt = dom || top.$("#tabs");
            var tab = tt.tabs('getSelected');
            var params = {};
            if (url != undefined && url != null && url.length > 0)
                params.content = CreateIFrame(url);
            if (title != undefined && title != null)
                params.title = title;
            tt.tabs('update', {
                tab: tab,
                options: params
            });
        }
        else if (!dom) { //对现有OA
            url = host + url;
            if (url.indexOf('nfm=1') == -1) {
                if (url.indexOf("?") > -1)
                    url += "&";
                else
                    url += "?";
                url += "nfm=1";
            }
            RegChildSysMessager(title);
            var msg = "CurrentTabRedirect('" + title + "','" + url + "')";
            SendMsgToParent(msg);
        }
    } catch (e) { }
}

//刷新页面皮肤样式
//tab:tab
function RefreshPageStyle() {
    try {
        var cookieTheme = top.GetCookie('easyuitheme');
        if (cookieTheme == undefined || cookieTheme == null)
            cookieTheme = top.defaultTheme;
        $('.easyuiTheme').attr('href', '/Scripts/jquery-easyui/themes/' + cookieTheme + '/easyui.css');
    } catch (e) { }
}

/*----------------------------------------------------------------------*/
//执行ajax方法操作
//url:url
//params:url对应的参数
//successBakFun:操作成功后的回调函数
//isShowTip:是否显示提示
//isGet:是否get方式获取
function ExecuteCommonAjax(url, params, successBakFun, isShowTip, isGet) {
    var msgTitle = '操作提示';
    var method = isGet ? "get" : "post";
    $.ajax({
        type: method,
        url: url,
        data: params,
        dataType: "json",
        beforeSend: function () {
            if (isShowTip) {
                topWin.OpenWaitDialog('拼命处理中...');
            }
        },
        success: function (result) {
            if (isShowTip) {
                topWin.CloseWaitDialog();
                if (result.Success) {
                    topWin.ShowMsg(msgTitle, '操作成功！', function () {
                        if (typeof (successBakFun) == "function")
                            successBakFun(result);
                    });
                }
                else {
                    topWin.ShowAlertMsg(msgTitle, result.Message, "info");
                }
            }
            else {
                if (typeof (successBakFun) == "function")
                    successBakFun(result);
            }
        },
        error: function (err) {
            if (isShowTip) {
                topWin.CloseWaitDialog();
                topWin.ShowAlertMsg(msgTitle, "操作失败，服务端异常！", "error");
            }
            if (typeof (successBakFun) == "function")
                successBakFun({ Success: false, Message: '操作失败，服务端异常！' });
        }
    });
}

/*-----------------弹出对话框-------------------------*/
//保存当前打开对话框编号Id
var dialogNum = [];
//打开对话框
//title:对话框标题
//urlOrContent:对话框内容页面或者html内容
//toolbar:对话框底部工具栏
/*---底部工具栏格式--------
 [{
        text: "保存",
        iconCls: "eu-icon-save",
        handler: function (e) {
        }
    }, {
        text: '关闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
  }];
-------------------------*/
//width:对话框宽度，默认500
//height:对话框高度，默认300
//icon:对话框图标,默认为搜索编辑图标
//openBackFun:打开后的回调函数
function OpenDialog(title, urlOrContent, toolbar, width, height, icon, openBackFun) {
    if (!toolbar || toolbar.length == 0) {
        $.messager.alert("错误", "弹框按钮未定义，至少要添加一个关闭按钮！", "error");
        return;
    }
    var maxNo = 0;
    for (var i = 0; i < dialogNum.length; i++) {
        if (dialogNum[i] > maxNo)
            maxNo = dialogNum[i];
    }
    if (maxNo >= 10) {
        $.messager.alert("错误", "弹框资源已耗尽，请先释放其他弹出框资源！", "error");
        return;
    }
    var no = maxNo + 1;
    dialogNum.push(no);
    var dialogTagId = 'page_dialog' + no;
    var div = $("#" + dialogTagId);
    var content = urlOrContent && urlOrContent.indexOf(".html") > -1 ? CreateIFrame(urlOrContent) : urlOrContent;
    var iframe = urlOrContent && urlOrContent.indexOf(".html") > -1 ? topWin.GetCurrentDialogFrame()[0] : null;
    div.dialog({
        minimizable: false,
        maximizable: true,
        closed: false,
        closable: true,
        modal: true,
        draggable: true,
        resizable: true,
        cache: false,
        maximized: false,
        title: title,
        iconCls: icon ? icon : "eu-icon-edit",
        content: content,
        width: parseInt(width) > 0 ? parseInt(width) : 500,
        height: parseInt(height) > 0 ? parseInt(height) : 300,
        buttons: toolbar,
        onOpen: function () {
            $('div.panel-tool a.panel-tool-close').click(function () {
                CloseDialog();
            });
            if (urlOrContent && urlOrContent.indexOf(".html") < 0) {
                $.parser.parse("#" + dialogTagId);
            }
            if (IsShowStyleBtn()) {
                setTimeout(function () {
                    var divBtnPar = topWin.$('#' + dialogTagId).parent();
                    var dgBtn = $("div.dialog-button a", divBtnPar);
                    dgBtn.each(function (i, item) {
                        var c = 'c' + (i + 1);
                        if (c != undefined && c != null && c.length > 0) {
                            $(item).addClass(c);
                        }
                    });
                    $.parser.parse('#' + dialogTagId);
                }, 50);
            }
            if (typeof (openBackFun) == "function") {
                openBackFun(dialogTagId, iframe); //回调函数
            }
        }
    });
}

//关闭对话框
function CloseDialog() {
    var no = dialogNum.pop();
    var div = $("#page_dialog" + no);
    div.dialog("close");
}

//打开确定关闭对话框
//title:对话框标题
//urlOrContent:对话框内容页面或者html内容
//width:对话框宽度，默认500
//height:对话框高度，默认300
//okHandleFun:点击确定后的事件
//cancelHandleFun:点击关闭后的事件
//openBackFun:打开后的回调函数
function OpenOkCancelDialog(title, urlOrContent, width, height, okHandleFun, cancelHandleFun, openBackFun) {
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var iframe = urlOrContent && urlOrContent.indexOf(".html") > -1 ? topWin.GetCurrentDialogFrame()[0] : null;
            if (typeof (okHandleFun) == "function") {
                okHandleFun(iframe, function (action) {
                    if (action) {
                        CloseDialog();
                    }
                });
            }
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            CloseDialog();
            if (typeof (cancelHandleFun) == "function") {
                cancelHandleFun();
            }
        }
    }];
    OpenDialog(title, urlOrContent, toolbar, width, height, 'eu-icon-cog', openBackFun);
}

//移除顶层弹出框按钮
//btnId:按钮ID
function RemoveTopDialogBtn(btnId) {
    var maxNo = 0;
    for (var i = 0; i < dialogNum.length; i++) {
        if (dialogNum[i] > maxNo)
            maxNo = dialogNum[i];
    }
    var div = $("#page_dialog" + maxNo);
    div.parent().find('#' + btnId).remove();
}

//移除顶层弹出框按钮
//btnId:id以些开头的按钮
function RemoveTopDialogBtnBySomeId(btnId) {
    var maxNo = 0;
    for (var i = 0; i < dialogNum.length; i++) {
        if (dialogNum[i] > maxNo)
            maxNo = dialogNum[i];
    }
    var div = $("#page_dialog" + maxNo);
    div.parent().find("a[id^='" + btnId + "']").remove();
}

//禁用顶层弹出框按钮
//btnId:按钮domID
function DisableTopDialogBtn(btnId) {
    var maxNo = 0;
    for (var i = 0; i < dialogNum.length; i++) {
        if (dialogNum[i] > maxNo)
            maxNo = dialogNum[i];
    }
    var div = $("#page_dialog" + maxNo);
    div.parent().find('#' + btnId).linkbutton('disable');
}

//启用顶层弹出框按钮
//btnId:按钮domID
function EnableTopDialogBtn(btnId) {
    var maxNo = 0;
    for (var i = 0; i < dialogNum.length; i++) {
        if (dialogNum[i] > maxNo)
            maxNo = dialogNum[i];
    }
    var div = $("#page_dialog" + maxNo);
    div.parent().find('#' + btnId).linkbutton('enable');
}

//获取当前对话框的iframe
//divId:指定div
function GetCurrentDialogFrame(divId) {
    if (divId != undefined && divId != null && divId.length > 0) {
        return $("#" + divId).find("iframe:first")
    }
    var no = 0;
    for (var i = 0; i < dialogNum.length; i++) {
        if (dialogNum[i] > no)
            no = dialogNum[i];
    }
    if (no == 0) no = 1;
    var iframe = $("#page_dialog" + no).find("iframe:first");
    return iframe;
}

//获取前一个对话框的iframe
function GetParentDialogFrame() {
    var maxNo = 0; //当前对话框编号
    for (var i = 0; i < dialogNum.length; i++) {
        if (dialogNum[i] > maxNo)
            maxNo = dialogNum[i];
    }
    var no = maxNo - 1; //前一个对话框编号
    if (no < 1) return null;
    var iframe = $("#page_dialog" + no).find("iframe:first");
    return iframe;
}

//显示消息
//title:标题
//msg:消息内容
//backFun:关闭后事件
//position:消息框位置,
//timeout:停留时间默认1s
/*position参数说明：
  'topLeft':左上角
  'topCenter':顶部中间
  'topRight':右上角
  'centerLeft':中间靠左
  'center':正中间
  'centerRight':中间靠右
  'bottomLeft':左下角
  'bottomCenter':底部中间
  'bottomRight':右下角
  为空或其他：正中间
*/
function ShowMsg(title, msg, backFun, position, timeout) {
    var params = {
        title: title,
        msg: msg,
        showType: 'fade'
    };
    if (typeof (backFun) == "function") {
        params.onClose = backFun;
    }
    if (timeout && parseInt(timeout) > 0) {
        params.timeout = parseInt(timeout) * 1000;
    }
    else {
        params.timeout = 1000;
    }
    if (position == 'topLeft') { //左上角
        params.style = {
            right: '',
            left: 0,
            top: document.body.scrollTop + document.documentElement.scrollTop,
            bottom: ''
        };
    }
    else if (position == 'topCenter') { //顶部中间
        params.style = {
            right: '',
            top: document.body.scrollTop + document.documentElement.scrollTop,
            bottom: ''
        };
    }
    else if (position == 'topRight') { //右上角
        params.style = {
            left: '',
            right: 0,
            top: document.body.scrollTop + document.documentElement.scrollTop,
            bottom: ''
        };
    }
    else if (position == 'centerLeft') { //中间靠左
        params.style = {
            left: 0,
            right: '',
            bottom: ''
        };
    }
    else if (position == 'center') { //正中间
        params.style = {
            right: '',
            bottom: ''
        };
    }
    else if (position == 'centerRight') { //中间靠右
        params.style = {
            left: '',
            right: 0,
            bottom: ''
        };
    }
    else if (position == 'bottomLeft') { //左下角
        params.style = {
            left: 0,
            right: '',
            top: '',
            bottom: -document.body.scrollTop - document.documentElement.scrollTop
        };
    }
    else if (position == 'bottomCenter') { //底部中间
        params.style = {
            right: '',
            top: '',
            bottom: -document.body.scrollTop - document.documentElement.scrollTop
        };
    }
    else if (position == 'bottomRight') { //右下角
        params.style = null;
    }
    else {
        params.style = {
            right: '',
            bottom: ''
        };
    }
    $.messager.show(params);
}

//显示一个包含“确定”和“取消”按钮的确认消息窗口
//title:在头部面板显示的标题文本
//msg:消息内容
//msgType:消息类型，有以下几种消息类型：
/*
info:普通消息
warning:警告消息
question:询问消息
error:错误消息
*/
//backFun:回调函数,当用户点击“确定”按钮的时侯将传递一个true值给回调函数，否则传递一个false值
function ShowAlertMsg(title, msg, msgType, backFun) {
    $.messager.alert(title, msg, msgType, backFun);
}

//显示一个用户可以输入文本的并且带“确定”和“取消”按钮的消息窗体。参数：
//title：在头部面板显示的标题文本。
//msg：显示的消息文本。
//backFun(ok): 在用户输入一个值参数的时候执行的回调函数。 
function ShowConfirmMsg(title, msg, backFun) {
    $.messager.confirm(title, msg, function (ok) {
        if (typeof (backFun) == "function") {
            backFun(ok);
        }
    });
}

//打开等待对话框
//title:标题
function OpenWaitDialog(title) {
    $.messager.progress({
        title: title && title.length > 0 ? title : '拼命处理中...',
        text: ''
    });
}

//关闭等待对话框
function CloseWaitDialog() {
    try {
        $.messager.progress('close');
    } catch (err) { }
}

/*---------------------------------------------------------------------*/
//是否显示样式按钮
function IsShowStyleBtn() {
    var v = $('#isShowStyleBtn').val();
    return parseInt(v) == 1;
}

//绑定流程tips
//dom:dom对象
function BindFlowTips(dom) {
    var moduleId = $(dom).attr('moduleId');
    var recordId = $(dom).attr('recordId');
    var tipUrl = '/Bpm/FlowTips.html?moduleId=' + moduleId + '&id=' + recordId;
    if (isNfm)
        tipUrl = host + tipUrl;
    $(dom).tooltip({
        content: $('<div></div>'),
        onShow: function () {
            $(this).tooltip('arrow').css('left', 20);
            $(this).tooltip('tip').css('left', $(this).offset().left);
        },
        onUpdate: function (cc) {
            cc.panel({
                width: 180,
                height: 'auto',
                border: false,
                href: tipUrl
            });
        }
    });
}

//初始化当前时间/时间，使用方法 InitCurrentDate($('#applicateTime'),true,true);
//dom:dom对象
//containTime:是否包含时间，时间控件
//isSpan:是否是lable控件
//disabled:是否禁用，针对时间控件
function InitCurrentDate(dom, containTime, isSpan, disable) {
    var curr_time = new Date();
    var strDate = curr_time.getFullYear() + "-";
    strDate += curr_time.getMonth() + 1 + "-";
    strDate += curr_time.getDate() + "-";
    if (containTime) {
        strDate += curr_time.getHours() + ":";
        strDate += curr_time.getMinutes() + ":";
        strDate += curr_time.getSeconds();
    }
    if (!isSpan) {
        if (containTime) {
            if (disable)
                $(dom).datetimebox('disable');
            $(dom).datetimebox("setValue", strDate);
        }
        else {
            if (disable)
                $(dom).datebox('disable');
            $(dom).datebox("setValue", strDate);
        }
    }
    else { //span控件
        $(dom).attr('value', strDate).text(strDate);
    }
}

//下拉树自动完成
//q:用户输入字符
//fieldName:字段名称
function QueryComboTree(q, fieldName) {
    if (typeof (OverQueryComboTree) == "function") {
        OverQueryComboTree(q, fieldName);
    }
    else {
        var tree = $('#' + fieldName).combotree('tree');
        tree.tree("search", q);
    }
}

//下拉框、下拉列表、下拉树数据过滤
//fieldName:字段名
//valueField:值字段
//textField:显示字段
//data:当前数据
//parentDom:下拉树时父节点DOM对象
//isComboTree:是否是下拉树
function OnLoadFilter(fieldName, valueField, textField, data, parentDom, isComboTree) {
    //调用模块自定义事件
    if (typeof (OverLoadFilter) == "function") {
        return OverLoadFilter(fieldName, valueField, textField, data, parentDom, isComboTree);
    }
    if (isComboTree) //下拉树
    {
        if (typeof (data) == 'string') {
            try {
                var tempData = eval('(' + data + ')');
                return tempData;
            } catch (e) { }
        } else {
            var arr = []; arr.push(data);
            return arr;
        }
    }
    else if (data && data.length > 0) { //下拉列表框
        if (typeof (data) == "string") {
            try {
                var tempData = eval("(" + data + ")");
                arr = [];
                arr.push(tempData);
                return arr;
            } catch (e) { }
        }
        else {
            return data;
        }
    }
    return null;
}

//下拉框、下拉列表、下拉树的下拉数据加载成功事件
//fieldName:字段名
//valueField:值字段
//textField:显示字段
//isTree:是否树字段
function OnFieldLoadSuccess(fieldName, valueField, textField, isTree) {
    if (isTree) {
        try {
            var treeDom = $('#' + fieldName).combotree('tree');
            treeDom.tree("collapseAll");
            var roots = treeDom.tree("getRoots"); //展开所有根结点
            if (roots && roots.length > 0) {
                $.each(roots, function (i, root) {
                    treeDom.tree("expand", root.target);
                });
            }
        } catch (e) { }
    }
    //调用模块自定义事件
    if (typeof (OverOnFieldLoadSuccess) == "function") {
        OverOnFieldLoadSuccess(fieldName, valueField, textField);
    }
}

//数字千分位格式化
//num:数字
function NumThousandsFormat(num) {
    if (num == undefined || num == null || num == '' || isNaN(num))
        return num;
    var source = String(num).split(".");//按小数点分成2部分
    source[0] = source[0].replace(new RegExp('(\\d)(?=(\\d{3})+$)', 'ig'), "$1,");//只将整数部分进行都好分割
    return source.join(".");//再将小数部分合并进来
}

//前一记录
function PreRecord(obj) {

}

//下一记录
function NextRecord(obj) {
}

//打印
function PrintModel(obj) {
    window.print();
}


var gooFlowObj = null; //流程对象
var workflowId = GetLocalQueryString("workflowId"); //当前流程ID
var workName = decodeURI(GetLocalQueryString("name")); //当前流程名称
var moduleId = GetLocalQueryString("moduleId");
var id = GetLocalQueryString("id");
var isRun = false; //当前流程是否正在运行

//初始化
$(document).ready(function () {
    var browserName = GetBrowserName();
    var broserVersion = GetVersion();
    if (browserName == 'ie' && parseInt(broserVersion) <= 8)
        return;
    //明细子流程审批时默认不加载流程图
    if ($("a[id^='flowBtn_']").length > 0 && $("a[id^='flowBtn_']").eq(0).attr('parentToDoId')) {
        moduleId = null;
    }
    //流程画面对象初始化
    if (workName) $("#gooFlowDom").attr('title', workName);
    var hasHead = workflowId ? true : false;
    var hasTool = hasHead;
    var w = workflowId ? GetBodyWidth() : $('#div_ApprovalList').width() - 10;
    var h = workflowId ? GetBodyHeight() : 200;
    var property = {
        width: w, height: h,
        toolBtns: ["start round", "end round", "task round"],
        haveHead: hasHead, headBtns: ["new", "import", "export", "save", "undo", "redo", "reload"],//如果haveHead=true，则定义HEAD区的按钮
        haveTool: hasTool, haveGroup: false, useOperStack: true
    };
    gooFlowObj = $.createGooFlow($("#gooFlowDom"), property);
    if (workflowId) {
        var remark = { cursor: "选择指针", direct: "连线", start: "开始结点", "end": "结束结点", "task": "任务结点" };
        gooFlowObj.setNodeRemarks(remark);
        //删除节点
        gooFlowObj.onItemDel = function (id, type) {
            topWin.ShowConfirmMsg('提示', '确定要删除该单元吗?', function (action) {
                if (action) {
                    if (type == 'node') {
                        gooFlowObj.delNode(id, true);
                    }
                    else if (type == 'line') {
                        gooFlowObj.delLine(id, true);
                    }
                }
            });
            return false;
        };
        //新建流程
        gooFlowObj.onBtnNewClick = function () {
            parent.AddNewFlow();
        };
        //导入流程
        gooFlowObj.onBtnImportClick = function () { };
        //导出流程
        gooFlowObj.onBtnExportClick = function () {

        };
        //保存流程
        gooFlowObj.onBtnSaveClick = function () {
            if (workflowId) {
                var startNodeNum = $("div[id^='gooFlowDom_start']").length;
                var endNodeNum = $("div[id^='gooFlowDom_end']").length;
                if (startNodeNum > 1 || endNodeNum > 1) {
                    topWin.ShowMsg('提示', '只能有一个开始结点和一个结束结点！');
                    return;
                }
                if (startNodeNum == 0 || endNodeNum == 0) {
                    topWin.ShowMsg('提示', '必须有一个开始结点和一个结束结点！');
                    return;
                }
                var url = '/' + CommonController.Async_Bpm_Controller + '/UpdateWorkflowChart.html';
                var workLines = [];
                var workNodes = [];
                $("div[id^='gooFlowDom_task'],g[id^='gooFlowDom_line'],div[id^='gooFlowDom_start'],div[id^='gooFlowDom_end']").each(function (i, item) {
                    var tagId = $(this).attr('id');
                    var params = $(this).attr('params');
                    var paramObj = null;
                    if (params != undefined && params != null && params.length > 0) {
                        paramObj = JSON.parse(decodeURI(params));
                    }
                    if (paramObj == null)
                        paramObj = { TagId: tagId };
                    else
                        paramObj.TagId = tagId;
                    if (tagId.indexOf('gooFlowDom_line') > -1) { //连接线
                        var line = gooFlowObj.getItemInfo(tagId, 'line');
                        paramObj.FromTagId = line['from'];
                        paramObj.ToTagId = line['to'];
                        paramObj.M = line['M'];
                        paramObj.Note = line['name'];
                        paramObj.LineType = line['type'];
                        workLines.push(paramObj);
                    }
                    else { //普通结点
                        var node = gooFlowObj.getItemInfo(tagId, 'node');
                        if (!paramObj.Name) {
                            paramObj.Name = node['name'];
                        }
                        paramObj.Top = node['top'];
                        paramObj.Left = node['left'];
                        paramObj.Width = node['width'];
                        paramObj.Height = node['height'];
                        if (tagId.indexOf('gooFlowDom_start') > -1)
                            paramObj.WorkNodeType = 0;
                        else if (tagId.indexOf('gooFlowDom_end') > -1)
                            paramObj.WorkNodeType = 1;
                        else
                            paramObj.WorkNodeType = 2;
                        workNodes.push(paramObj);
                    }
                });
                if (workNodes.length == 0) {
                    topWin.ShowMsg('提示', '请至少添加一个流程节点！');
                    return;
                }
                var workflow = { Id: workflowId, WorkNodes: workNodes, WorkLines: workLines };
                var params = { workflowJson: $.base64.encode(escape(JSON.stringify(workflow))) };
                ExecuteCommonAjax(url, params, null, true);
            }
        };
        //重新加载流程
        gooFlowObj.onFreshClick = function () {
            window.location.reload(true);
        };
        //结点参数设置
        var NodeParamsSet = function (tagId) {
            var nodeDom = $('#' + tagId);
            var nodeName = nodeDom.find('.span').text();
            var url = "/" + CommonController.Bpm_Controller + "/NodeParamSet.html?workflowId=" + workflowId + "&tagId=" + tagId + "&name=" + encodeURI(nodeName);
            topWin.OpenOkCancelDialog('结点参数设置－【' + nodeName + '】', url, 650, 480, function (iframe, backFun) {
                //获取节点参数
                var nodeParams = iframe.contentWindow.GetNodeParams();
                nodeDom.attr('params', encodeURI(JSON.stringify(nodeParams)));
                var display = nodeParams.DisplayName ? nodeParams.DisplayName : nodeParams.Name;
                gooFlowObj.setName(tagId, display, 'node');
                if (isRun && workflowId) { //当前流程正在运行，保存节点参数
                    var upUrl = "/" + CommonController.Bpm_Controller + "/UpdateNodeParams.html";
                    var upParams = { workflowId: workflowId, tagId: tagId, nodeData: encodeURI(JSON.stringify(nodeParams)) };
                    ExecuteCommonAjax(upUrl, upParams, function (result) {
                        var err = result ? result.Message : '服务端发生异常';
                        if (err && err.length > 0) {
                            topWin.ShowMsg('提示', err);
                        }
                        else {
                            topWin.ShowMsg('提示', '结点参数保存成功');
                            if (typeof (backFun) == "function")
                                backFun(true);
                        }
                    }, false, false);
                }
                else {
                    if (typeof (backFun) == "function")
                        backFun(true);
                }
            }, null, function (divDomId, iframe) {
                var divDom = topWin.$('#' + divDomId);
                var paramsTemp = nodeDom.attr('params');
                if (paramsTemp != undefined && paramsTemp) {
                    divDom.attr('params', paramsTemp);
                }
                else {
                    divDom.removeAttr('params');
                }
            });
        };
        //连线参数设置
        var LineParamsSet = function (tagId) {
            var lineDom = $('#' + tagId);
            var url = "/" + CommonController.Bpm_Controller + "/LineParamSet.html?workflowId=" + workflowId + "&tagId=" + tagId;
            topWin.OpenOkCancelDialog('连线参数设置', url, 650, 450, function (iframe, backFun) {
                //获取节点参数
                var lineParams = iframe.contentWindow.GetLineParams();
                lineDom.attr('params', encodeURI(JSON.stringify(lineParams)));
                gooFlowObj.setName(tagId, lineParams.Note, 'line');
                if (typeof (backFun) == "function")
                    backFun(true);
            }, null, function (divDomId, iframe) {
                var divDom = topWin.$('#' + divDomId);
                var paramsTemp = lineDom.attr('params');
                if (paramsTemp != undefined && paramsTemp) {
                    divDom.attr('params', paramsTemp);
                }
                else {
                    divDom.removeAttr('params');
                }
            });
        };
        //结点双击事件
        gooFlowObj.onItemNodeDbClick = function (id) {
            NodeParamsSet(id);
        };
        //连线双击事件
        gooFlowObj.onItemLineDbClick = function (id) {
            LineParamsSet(id);
        };
        //绑定结点、连线右键菜单
        $("div[id^='gooFlowDom_task'],g[id^='gooFlowDom_line']").live('contextmenu', function (e) {
            var id = $(this).attr('id');
            if (id.indexOf('gooFlowDom_line') > -1) {
                var line = gooFlowObj.getItemInfo(id, 'line');
                if (line['from'].indexOf('gooFlowDom_start') > -1 ||
                   line['to'].indexOf('gooFlowDom_end') > -1)
                    return; //连接开始结点和结束结点的连线不显示右键菜单
            }
            $('#mm-chart').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
            $('#mm-chart').data('tagId', id);
            return false;
        });
        //参数设置
        $('#mm-set').click(function (e) {
            var tagId = $('#mm-chart').data('tagId');
            if (tagId.indexOf('gooFlowDom_task') > -1) {
                NodeParamsSet(tagId);
            }
            else {
                LineParamsSet(tagId);
            }
        });
    }
    //加载流程图
    if (workflowId || moduleId) {
        var flowParams = workflowId ? { workflowId: workflowId } : { moduleId: moduleId, id: id };
        $.get('/' + CommonController.Async_Bpm_Controller + '/LoadWorkflowChart.html', flowParams, function (data) {
            if (data && data.FlowData) {
                gooFlowObj.loadData(data.FlowData);
                if (data.NodeParams) {
                    for (var tagId in data.NodeParams) {
                        var nodeDom = $('#' + tagId);
                        nodeDom.attr('params', encodeURI(JSON.stringify(data.NodeParams[tagId])));
                    }
                }
                if (data.LineParams) {
                    for (var tagId in data.LineParams) {
                        var lineDom = $('#' + tagId);
                        lineDom.attr('params', encodeURI(JSON.stringify(data.LineParams[tagId])));
                    }
                }
                if (data.CurrNodeTagObj) {
                    var currNodeDom = $('#' + data.CurrNodeTagObj.TagId);
                    currNodeDom.css('border', 'red solid 3px');
                    var svg = $('#draw_gooFlowDom').length > 0 ? $('#draw_gooFlowDom')[0] : null;
                    if (svg) {
                        var h = data.CurrNodeTagObj.MaxTop + 30;
                        var w = data.CurrNodeTagObj.MaxLeft + 30;
                        svg.setAttribute("width", w);
                        svg.setAttribute("height", h);
                        currNodeDom.parent().width(w);
                        currNodeDom.parent().height(h);
                    }
                    if (data.CurrNodeTagObj.Display)
                        $('#handleNode').text(data.CurrNodeTagObj.Display);
                }
                if (data.IsRun == true) { //当前流程正在运行
                    isRun = true;
                }
            }
        }, 'json');
    }
});

//加载流程审批进度图和审批记录
//mId:模块ID
//recoredId:记录ID
//todoId:待办ID
//row:当前行数据
function LoadFlowChart(mId, recoredId, todoId, row) {
    if ($('#div_ApprovalList').attr('recordId') == recoredId) //当前已加载
        return;
    //加载流程图
    var flowParams = { moduleId: mId, id: recoredId, todoId: todoId };
    $.get('/' + CommonController.Async_Bpm_Controller + '/LoadWorkflowChart.html', flowParams, function (data) {
        if (data && data.FlowData) {
            if (gooFlowObj != null) {
                gooFlowObj.clearData();
                gooFlowObj.loadData(data.FlowData);
                if (data.CurrNodeTagObj) { //流程图
                    var currNodeDom = $('#' + data.CurrNodeTagObj.TagId);
                    currNodeDom.css('border', 'red solid 3px');
                    var svg = $('#draw_gooFlowDom').length > 0 ? $('#draw_gooFlowDom')[0] : null;
                    if (svg) {
                        var h = data.CurrNodeTagObj.MaxTop + 30;
                        var w = data.CurrNodeTagObj.MaxLeft + 30;
                        svg.setAttribute("width", w);
                        svg.setAttribute("height", h);
                        currNodeDom.parent().width(w);
                        currNodeDom.parent().height(h);
                    }
                    if (data.CurrNodeTagObj.Display)
                        $('#handleNode').text(data.CurrNodeTagObj.Display);
                }
            }
            if (data.AppInfos && data.AppInfos.length) { //审批信息
                $('#tb_approvalList').datagrid('loadData', { total: data.AppInfos.length, rows: data.AppInfos });
                $('#div_ApprovalList').attr('recordId', recoredId);
                AutoMergeAppInfoCell(); //合并审批信息单元格
            }
            if (data.TitleKey && row) {
                var tabsDom = $("#detailTab");
                $('#div_ApprovalList').find('span.tabs-title').each(function (i, item) {
                    var initText = $(item).attr('value');
                    if (!initText) {
                        initText = $(item).text();
                        $(item).attr('value', initText);
                    }
                    var text = initText + '【' + row[data.TitleKey] + '】';
                    $(item).text(text);
                });
            }
        }
    }, 'json');
}

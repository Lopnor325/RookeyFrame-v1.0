
//初始化
$(document).ready(function () {
    /*------解决IE8下兼容性问题，加上会导致高版本兼容性问题---------------
    if (document.documentMode && document.documentMode >= 8) {
        document.writeln('<?import namespace="v" implementation="#default#VML" ?>');
    }
    ---------------------------------------------------------------------*/
    //初始化tree panel
    InitTreePanel();
    //加载流程分类树
    LoadFlowClassTree();
    //初始化tab
    InitTab();
});

//初始化tree panel
function InitTreePanel() {
    //tree panel添加右键菜单
    $('#treePanel').live('contextmenu', function (e) {
        $('#mm-pannel').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
        return false;
    });
    $('#mm-addclass').click(function (e) {
        AddFolder();
    });
}

//加载流程分类树
function LoadFlowClassTree() {
    var treeDom = $("#categoryTree");
    var treeUrl = "/" + CommonController.Async_Bpm_Controller + "/GetFlowClassTree.html?noroot=1";
    var treeParam = {
        onContextMenu: function (e, node) { //绑定右键菜单
            e.preventDefault();
            // 查找节点
            treeDom.tree('select', node.target);
            //在右键菜单显示之前就隐藏相应的菜单
            var selectedTreeNode = treeDom.tree("getSelected");
            //有子节点不显示删除
            if (selectedTreeNode.children && selectedTreeNode.children.length > 0) {
                $("#del").hide();
            }
            else {
                $("#del").show();
            }
            var workflowId = node.attribute.obj ? node.attribute.obj.workflowId : null;
            if (workflowId) {
                $("#open").show();
                $("#new").hide();
                $("#add").hide();
                $("#property").show();
                $("#sep").show();
            }
            else {
                $("#open").hide();
                $("#new").show();
                $("#add").show();
                $("#property").hide();
                $("#sep").hide();
            }
            // 显示快捷菜单
            $('#rightClkMenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
            StopBubble(e);
        },
        onClick: function (node) {
            var workflowId = node.attribute.obj ? node.attribute.obj.workflowId : null;
            if (workflowId) {
                var url = '/' + CommonController.Bpm_Controller + '/FlowCanvas.html?workflowId=' + workflowId;
                AddTab($('#flowTabs'), node.text, url, 'eu-icon-cog');
            }
        }
    };
    LoadTree(treeDom, treeUrl, treeParam);
}

//初始化tabs
function InitTab() {
    //初始化tab
    var tabs = $('#flowTabs');
    tabs.tabs({
        onSelect: function (title, index) {
            if (index > 0) {
                var tab = tabs.tabs('getTab', index);
                if (tab.attr('needfresh') == 'true') {
                    tab.removeAttr('needfresh');
                    UpdateTab(tabs, tab);
                }
            }
        }
    });
    var mm = $('#mm-flow');
    //双击关闭TAB选项卡
    $(".tabs li", tabs).live('dblclick', function (e) {
        var title = $(this).find(".tabs-closable").text();
        CloseTabByTitle(tabs, title);
    });
    //为选项卡绑定右键
    $(".tabs li", tabs).live('contextmenu', function (e) {
        mm.menu('show', {
            left: e.pageX,
            top: e.pageY
        });
        var title = $(this).find(".tabs-closable").text();
        mm.data("currtab", title);
        tabs.tabs('select', title);
        return false;
    });
    //刷新
    $('#mm-tabupdate', mm).click(function () {
        UpdateSelectedTab(tabs);
    });
    //关闭当前
    $('#mm-tabclose', mm).click(function () {
        var t = mm.data("currtab");
        CloseTabByTitle(tabs, t);
    });
    //关闭除当前之外的TAB
    $('#mm-tabcloseother', mm).click(function () {
        var nextall = $('.tabs-selected').nextAll();
        if (nextall.length > 0) {
            nextall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                CloseTabByTitle(tabs, t);
            });
        }
        var prevall = $('.tabs-selected').prevAll();
        if (prevall.length > 0) {
            prevall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                CloseTabByTitle(tabs, t);
            });
        }
    });
    //去除边框
    $('div.layout-panel-west .panel-header').css('border-top-width', '0px').
                                             css('border-left-width', '0px');
    $('div.layout-panel-center .panel-header').css('border-top-width', '0px').
                                             css('border-right-width', '0px');
    $('div.layout-panel-west .panel-body').css('border-left-width', '0px').
                                             css('border-bottom-width', '0px');
    $('div.layout-panel-center .panel-body').css('border-right-width', '0px').
                                             css('border-bottom-width', '0px');
    //$.parser.parse(); //重新解析页面
}

//分类树面板收缩展开事件
function TreePanelChange() {
    setTimeout(function () {
        //刷新tab的流程画布
        var tt = $('#flowTabs');
        var tabs = tt.tabs('tabs');
        if (tabs && tabs.length > 0) {
            var selectTab = GetSelectedTab(tt);
            var selectIndex = tt.tabs('getTabIndex', selectTab);
            if (selectIndex > 0) {
                UpdateTab(tt, selectTab); //刷新当前tab
            }
            //将其他tab添加标记以便在切换tab时将重新刷新页面
            for (var i = 0; i < tabs.length; i++) {
                var tab = tabs[i];
                var tabIndex = tt.tabs('getTabIndex', tab);
                if (tabIndex > 0 && tabIndex != selectIndex) {
                    tab.attr('needfresh', 'true');
                }
            }
        }
    }, 100);
}

//打开流程
function OpenFlow() {
    var node = $("#categoryTree").tree("getSelected");
    if (node && node.id) {
        var workflowId = node.attribute.obj ? node.attribute.obj.workflowId : null;
        if (workflowId) {
            var url = '/' + CommonController.Bpm_Controller + '/FlowCanvas.html?workflowId=' + workflowId;
            AddTab($('#flowTabs'), node.text, url, 'eu-icon-cog');
        }
    }
}

//新建流程
function AddNewFlow() {
    var node = $("#categoryTree").tree("getSelected");
    if (node && node.id) {
        var toolbar = [{
            id: 'btnOk',
            text: "确 定",
            iconCls: "eu-icon-ok",
            handler: function (e) {
                var win = topWin.GetCurrentDialogFrame()[0].contentWindow;
                win.Save(null, function (result) {
                    $("#categoryTree").tree('reload');
                    var flowInfo = win.GetFormData();
                    var url = '/' + CommonController.Bpm_Controller + '/FlowCanvas.html?workflowId=' + result.RecordId + '&name=' + encodeURI(flowInfo.Name);
                    AddTab($('#flowTabs'), flowInfo.Name, url, 'eu-icon-cog');
                    topWin.CloseDialog();
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
        var url = "/Page/EditForm.html?page=add&moduleName=流程信息&flowClassId=" + node.id;
        topWin.OpenDialog('新建流程', url, toolbar, 680, 280, 'eu-p2-icon-add_other');
    }
}

//新建分类
function AddFolder() {
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            topWin.GetCurrentDialogFrame()[0].contentWindow.Save(null, function (result) {
                if ($('#categoryTree li').length == 0)
                    location.reload();
                else
                    $("#categoryTree").tree('reload');
                topWin.CloseDialog();
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
    var url = "/Page/EditForm.html?page=add&moduleName=流程分类";
    topWin.OpenDialog('新建目录', url, toolbar, 680, 200, 'eu-icon-edit');
}

//删除选中分类节点
function DeleteNode() {
    var node = $("#categoryTree").tree("getSelected");
    if (node && node.id) {
        var workflowId = node.attribute.obj ? node.attribute.obj.workflowId : null;
        var moduleName = workflowId ? "流程信息" : "流程分类";
        var recordId = workflowId ? workflowId : node.id;
        topWin.ShowConfirmMsg('提示', '确定要删除' + moduleName + '【' + node.text + '】吗?', function (action) {
            if (action) {
                ExecuteCommonDelete(moduleName, recordId, false, true, function () {
                    $("#categoryTree").tree('reload');
                    //关闭当前打开的该流程标签
                    CloseTabByTitle($('#flowTabs'), node.text);
                });
            }
        });
    }
}

//加载选中流程属性
function LoadFlow() {
    var node = $("#categoryTree").tree("getSelected");
    if (node && node.id) {
        var workflowId = node.attribute.obj ? node.attribute.obj.workflowId : null;
        if (workflowId) {
            var toolbar = [{
                id: 'btnOk',
                text: "确 定",
                iconCls: "eu-icon-ok",
                handler: function (e) {
                    var win = topWin.GetCurrentDialogFrame()[0].contentWindow;
                    win.Save(null, function (result) {
                        //修改后的流程信息
                        var flowInfo = win.GetFormData();
                        var treeDom = $("#categoryTree");
                        //刷新流程分类树
                        treeDom.tree('reload');
                        var displayName = flowInfo.DisplayName ? flowInfo.DisplayName : flowInfo.Name;
                        if (displayName != node.text) { //当前流程显示名称已变更
                            //关闭当前打开的该流程标签
                            CloseTabByTitle($('#flowTabs'), node.text);
                            //找到修改属性后的流程结点并选中它
                            var tempNode = treeDom.tree('find', workflowId + 10000);
                            treeDom.tree('select', tempNode.target);
                            //重新打开tab
                            var url = '/' + CommonController.Bpm_Controller + '/FlowCanvas.html?workflowId=' + workflowId;
                            AddTab($('#flowTabs'), displayName, url, 'eu-icon-cog');
                        }
                        topWin.CloseDialog();
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
            var url = "/Page/EditForm.html?page=edit&id=" + workflowId + "&moduleName=流程信息";
            topWin.OpenDialog('编辑流程', url, toolbar, 660, 280, 'eu-icon-edit');
        }
    }
}

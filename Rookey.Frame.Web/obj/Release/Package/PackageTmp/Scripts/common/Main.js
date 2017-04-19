var defaultTheme = 'default'; //默认皮肤样式

//初始化
$(function () {
    //初始化tab
    initTab();
    //初始化菜单
    initMenu();
    //初始化设置
    initSet();
    //初始化其他
    initOther();
});

/*-------------------------私有方法-----------------------------*/
//初始化Tab
function initTab() {
    //双击关闭TAB选项卡
    $(".tabs-inner").live('dblclick', function () {
        var title = $(this).children(".tabs-closable").text();
        CloseTabByTitle(null, title);
    });
    //为选项卡绑定右键
    $(".tabs-inner").live('contextmenu', function (e) {
        var title = $(this).children(".tabs-closable").text();
        if (title) {
            $('#mm').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
            $('#mm').data("currtab", title);
            $('#tabs').tabs('select', title);
        }
        return false;
    });
    //绑定右键菜单事件
    //刷新
    $('#mm-tabupdate').click(function () {
        UpdateSelectedTab(null);
    });
    //关闭当前
    $('#mm-tabclose').click(function () {
        var t = $('#mm').data("currtab");
        CloseTabByTitle(null, t);
    });
    //全部关闭
    $('#mm-tabcloseall').click(function () {
        $('.tabs-inner span').each(function (i, n) {
            var t = $(n).text();
            CloseTabByTitle(null, t);
        });
    });
    //关闭除当前之外的TAB
    $('#mm-tabcloseother').click(function () {
        $('#mm-tabcloseright').click();
        $('#mm-tabcloseleft').click();
    });
    //关闭当前右侧的TAB
    $('#mm-tabcloseright').click(function () {
        var nextall = $('.tabs-selected').nextAll();
        if (nextall.length == 0) {
            return false;
        }
        nextall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            CloseTabByTitle(null, t);
        });
        return false;
    });
    //关闭当前左侧的TAB
    $('#mm-tabcloseleft').click(function () {
        var prevall = $('.tabs-selected').prevAll();
        if (prevall.length == 0) {
            return false;
        }
        prevall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            CloseTabByTitle(null, t);
        });
        return false;
    });
    tabHeightInit();
    $('#deskIframe').attr('src', $('#deskIframe').attr('url'));
}

//菜单初始化
function initMenu() {
    var menuDom = $("#leftMenu");
    if (menuDom.length > 0) {
        var loadPanel = function (panel) {
            var ul = panel.find("ul:first");
            var cls = ul.attr("class");
            if (!!cls == false || cls.indexOf("tree") < 0) {
                var menuId = ul.attr("menuId");
                var url = '/DataAsync/GetTreeByNode.html?moduleName=' + encodeURI("菜单管理") + '&noRoot=1&parentId=' + menuId;
                var attrUrl = ul.attr("url");
                if (attrUrl != null && attrUrl != undefined && attrUrl != "" && attrUrl != "null") {
                    url = attrUrl + "?menuId=" + menuId;
                }
                LoadTree(ul, url);
            }
        }
        //加载菜单
        menuDom.accordion({
            onSelect: function (title, index) {
                var panel = $(this).accordion("getPanel", title);
                loadPanel(panel);
            }
        });
        //加载第一个菜单面板树
        var panel = menuDom.accordion("getSelected");
        if (panel) {
            loadPanel(panel);
        }
    }
}

//初始化其他设置
function initSet() {
    $('span.head-left').css('cursor', 'pointer').click(function () {
        window.location.reload();
    });
    //退出系统
    $('#btnLogout').click(function () {
        top.ShowConfirmMsg("系统提示", "您确定要退出本次登录吗", function (ok) {
            if (ok) top.location.href = '/User/Logout.html';
        });
    });
    //个人设置
    $('#btnPersonalSet').click(function () {
        var toolbar = [{
            text: "保 存",
            iconCls: "eu-icon-save",
            handler: function (e) {
                var theme = top.GetCurrentDialogFrame()[0].contentWindow.GetTheme();
                if (theme != null) {
                    top.SetCookie('easyuitheme', theme);
                    $('.easyuiTheme').attr('href', '/Scripts/jquery-easyui/themes/' + theme + '/easyui.css');
                    $('.easyuiExtTheme').attr('href', '/Scripts/jquery-easyui/themes/' + theme + '/easyui-ext.css');
                    $('.mainTheme').attr('href', '/Css/common/main_ext/main_' + theme + '.css');
                    //刷新当前打开页面样式
                    var tabs = $('#tabs').tabs('tabs');
                    if (tabs.length > 0) {
                        for (var i = 0; i < tabs.length; i++) {
                            try {
                                var tab = tabs[i];
                                var iframe = tab.find("iframe:first");
                                iframe[0].contentWindow.RefreshPageStyle();
                            } catch (ex) { }
                        }
                    }
                }
                top.CloseDialog();
            }
        }, {
            text: '关 闭',
            iconCls: "eu-icon-close",
            handler: function (e) {
                top.CloseDialog();
            }
        }];
        top.OpenDialog("个人设置", '/Page/PersonalSet.html', toolbar);
    });
    //修改密码
    $('#btnChangePwd').click(function () {
        var toolbar = [{
            text: "确 定",
            iconCls: "eu-icon-ok",
            handler: function (e) {
                top.GetCurrentDialogFrame()[0].contentWindow.SaveModifyPwd(function () {
                    top.ShowMsg("提示", "密码修改成功！", function () {
                        top.CloseDialog();
                    });
                });
            }
        }, {
            text: '取 消',
            iconCls: "eu-icon-close",
            handler: function (e) {
                top.CloseDialog();
            }
        }];
        top.OpenDialog("修改密码", '/Page/ChangePwd.html', toolbar, 480, 250, 'eu-icon-changePwd');
    });
    //切换账号
    $('#btnChangeUser').click(function () {
        var toolbar = [{
            text: "确 定",
            iconCls: "eu-icon-ok",
            handler: function (e) {
                var url = '/' + CommonController.User_Controller + '/ChangeUser.html';
                var params = { username: $('#username').val() };
                ExecuteCommonAjax(url, params, function (result) {
                    if (result.Success)
                        window.location.href = "/Page/Main.html";
                    else
                        top.ShowMsg('提示', result.Message);
                }, false);
            }
        }, {
            text: '取 消',
            iconCls: "eu-icon-close",
            handler: function (e) {
                top.CloseDialog();
            }
        }];
        var content = "<table style='margin-top:15px;margin-left:15px;'><tr><td>用户名：</td><td><input type='text' id='username' name='username' style='width:200px;'/></td></tr></table>";
        top.OpenDialog("切换账号", content, toolbar, 330, 160, 'eu-icon-user');
    });
    //系统配置
    $('#btnWebConfig').click(function () {
        var toolbar = [{
            text: "保 存",
            iconCls: "eu-icon-save",
            handler: function (e) {
            }
        }, {
            text: '关 闭',
            iconCls: "eu-icon-close",
            handler: function (e) {
                top.CloseDialog();
            }
        }];
        top.OpenDialog("系统配置", '/Page/WebConfig.html', toolbar);
    });
    //邮件
    $('#btnEmail').click(function () {
        window.open('/Page/EmailIndex.html');
    });
}

//初始化其他
function initOther() {
    defaultTheme = $('#regionNorth').attr('defaultTheme');
    $.parser.parse('#wnav');
    //打开returnUrl
    var returnUrl = GetLocalQueryString("returnUrl");
    if (returnUrl) {
        returnUrl = decodeURIComponent(returnUrl);
        var title = GetQueryStringByUrl(returnUrl, 'title');
        AddTab(null, title, returnUrl);
    }
}

//设置快捷菜单
function SetQuckMenu() {
    var toolbar = [{
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            top.OpenWaitDialog('快捷菜单设置保存中...');
            top.GetCurrentDialogFrame()[0].contentWindow.SaveUserQuckMenus(function (result) {
                if (result && result.Success) {
                    top.ShowMsg("提示", "快捷菜单设置成功！", function () {
                        top.CloseWaitDialog();
                        top.CloseDialog();
                        //重新加载用户快捷菜单
                        $.post('/' + CommonController.Async_System_Controller + '/ReloadUserQuckMenus.html', function (html) {
                            $('#quickOpToolbar').html(html);
                            $.parser.parse('#quickOpToolbar');
                        }, "html");
                    });
                }
                else {
                    top.ShowAlertMsg("提示", result.Message, "info", function () {
                        top.CloseWaitDialog();
                    });
                }
            });
        }
    }, {
        text: '取 消',
        iconCls: "eu-icon-close",
        handler: function (e) {
            top.CloseDialog();
        }
    }];
    top.OpenDialog("快捷菜单设置", '/Page/AddQuckMenu.html', toolbar, 630, 500, 'eu-p2-icon-tag_blue_add');
}

//单击菜单
function TreeNodeOnClick(node, dom) {
    var title = node.text;
    if (!node.children) { //子菜单
        if (!node.attribute) return;
        var url = node.attribute.url;
        if (url) { //自定义url菜单
            if (url.indexOf("?") > -1)
                url += "&";
            else
                url += "?";
            url += 'mId=' + node.id;
            if (node.attribute.obj && node.attribute.obj.isNewWinOpen) { //新窗口中打开
                window.open(url);
            }
            else { //在框架的标签页中打开
                AddTab(null, title, url);
            }
        }
        else { //通用模块菜单
            var moduleId = node.attribute.obj.moduleId;
            var moduleName = node.attribute.obj.moduleName;
            if (node.attribute.obj && (moduleId || moduleName)) {
                var gridUrl = "/Page/Grid.html?page=grid";
                if (moduleId) {
                    gridUrl += "&moduleId=" + moduleId;
                }
                else if (moduleName) {
                    gridUrl += "&moduleName=" + moduleName;
                }
                gridUrl += "&r=" + Math.random();
                AddTab(null, title, gridUrl);
            }
        }
    }
    else { //文件夹菜单
        $(dom).tree('toggle', node.target);
    }
}

//菜单加载成功
function TreeOnLoadSuccess(node, data, dom) {
    $(dom).tree("collapseAll"); //全部收缩
}

//主页面tab选中事件
function OverOnSelect(title, index) {
    if (index > 0) {
        var iframe = getTabFrame(index);
        if (iframe.length > 0) {
            try {
                iframe[0].contentWindow.CorrectGridHeight(false);
            } catch (ex) { }
        }
    }
}
/*------------------------------------------------------------------*/

/*----------------------公有方法------------------------------------*/
//tabs工具栏初始化
function tabHeightInit() {
    var th = $('#tabs').attr('th');
    if (th != undefined && th) {
        $('#tabs_toolbar').height(th);
    }
}

//最大
function maximizeTab() {
    $("body").layout("collapse", "north");
    try {
        $("body").layout("collapse", "west");
    } catch (ex) { }
    $("#ttb_max").hide();
    $("#ttb_min").show();
}

//还原
function restoreTab() {
    $("body").layout("expand", "north");
    try {
        $("body").layout("expand", "west");
    } catch (ex) { }
    $("#ttb_min").hide();
    $("#ttb_max").show();
}

//调整页面布局
function parseLayout() {
    var panel = top.$("body").layout('panel', 'center');
    var w = panel.panel('options').width;
    panel.panel('resize', { width: w - 1 });
    panel.panel('resize', { width: w });
}

//获取当前tab的iframe
function getCurrentTabFrame() {
    var tab = GetSelectedTab();
    var iframe = tab.find("iframe:first");
    return iframe;
}

//获取tab中的第一个iframe
//tabIndexOrTitle:tab索引或标题
function getTabFrame(tabIndexOrTitle) {
    var tab = GetTab(null, tabIndexOrTitle);
    var iframe = tab.find("iframe:first");
    return iframe;
}

//获取当前tab的frame内的dom对象
//domId:domId
function getCurrentTabFrameDom(domId) {
    var tab = GetSelectedTab();
    var iframe = tab.find("iframe:first");
    return iframe.contents().find("#" + domId);
}

//获取当前tab的frmae内的dom对象
//selector:选择器
function getCurrentTabFrameDomBySelector(selector) {
    var tab = GetSelectedTab();
    var iframe = tab.find("iframe:first");
    return iframe.contents().find(selector);
}

//获取当前tab的frame内的某个标签内的dom对象
//tag:标签
//startDomId:Id以startDomId开始的
function getCurrentTabFrameSomeDom(tag, startDomId) {
    var tab = GetSelectedTab();
    var iframe = tab.find("iframe:first");
    return iframe.contents().find(tag + "[id^='" + startDomId + "']");
}

//获取中间内容区宽度
function getContentAreaWidth() {
    return $("#tabs").tabs("options").width;
}

//获取中间区域高度
function getContentAreaHeight() {
    return $("#tabs").tabs("options").height;
}

//弹出登录对话框
function openLoginDialog() {
    var toolbar = [{
        id: 'btnOk',
        text: "登录",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var iframe = $('#' + $(e.data.target).attr('divId')).find("iframe:first")[0];
            iframe.contentWindow.DoLogin();
        }
    }, {
        id: 'btnClose',
        text: '关闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            top.CloseDialog();
        }
    }];
    top.OpenDialog('登录窗口', '/User/DialogLogin.html?noself=1', toolbar, 470, 300, 'eu-icon-password', function (divId) {
        $('#btnOk').attr('divId', divId);
    });
}

/*-----------------------------------------------------------------*/

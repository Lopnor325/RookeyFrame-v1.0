var tempUserId = GetLocalQueryString("userId");
var treeUrl = '/' + CommonController.Async_Data_Controller + '/GetTreeByNode.html?icon=eu-icon-user&moduleName=' + encodeURI("用户管理");
var isAsync = true;
var treeParams = { async: isAsync };
var treeDom = $('#userTree');

//初始化
$(function () {
    if (!tempUserId) { //从菜单进入
        //加载用户树
        LoadTree(treeDom, treeUrl, treeParams);
        //设置右边tab左边框
        $("#region_center").css('border-left-width', '1px').css('border-left-style', 'solid').css('border-left-color', topWin.GetBorderColor());
    }
    else { //从用户列表进入
        LoadUserPermission(tempUserId);
    }
});

//大模块值发生改变时
function BigModuleChanged(newValue, oldValue) {
    var userId = tempUserId;
    if (!tempUserId) {
        var node = treeDom.tree('getSelected');
        if (node && node.id)
            userId = node.id;
    }
    if (userId)
        LoadUserPermission(userId, newValue);
}

//用户树加载完成
function TreeOnLoadSuccess(node, data, dom) {
    if (!isAsync) {
        if (data && data.length > 0) {
            var userId = data[0].id;
            var treeDomId = "_easyui_tree_1";
            if (userId == GuidEmpty && data[0].children && data[0].children.length > 0) { //第一个节点为非正常根节点
                userId = data[0].children[0].id;
                treeDomId = "_easyui_tree_2";
            }
            LoadUserPermission(userId);
            treeDom.tree("select", $("#" + treeDomId));
        }
    }
}

//选择用户结点前事件
function TreeOnBeforeSelect(node) {
    if (node.id == GuidEmpty || node.children != null) {
        return false;
    }
    return true;
}

//单击用户树节点事件
function TreeNodeOnClick(node) {
    if (node.id != GuidEmpty && node.children == null) {
        LoadUserPermission(node.id);
    }
}

//加载用户权限
function LoadUserPermission(userId, tempTopMenuId) {
    if (userId == GuidEmpty) return;
    var topMenuId = tempTopMenuId ? tempTopMenuId : $('#bigModuleItem').combobox('getValue');
    var gridDom = $('#tb_permission');
    gridDom.treegrid({
        height: gridDom.attr('h'),
        url: '/' + CommonController.Async_System_Controller + '/LoadUserPermission.html?userId=' + userId + '&topMenuId=' + topMenuId + '&r=' + Math.random(),
        loadFilter: function (data) {
            var arr = ToTreeData(data, "MenuId", "ParentId", "children");
            return arr;
        },
        onLoadSuccess: function (data) {
            $("input[funType='0'],input[funType='1']").click(function (e) {
                StopBubble(e);
            });
            $("img[name='setPower']").click(function (e) {
                StopBubble(e);
            });
        },
        onCheckAll: function (rows) {
            $("input:enabled[funType='0'],input:enabled[funType='1']").attr("checked", "checked");
        },
        onUncheckAll: function (rows) {
            $("input:enabled[funType='0'],input:enabled[funType='1']").removeAttr("checked");
        },
        onCheck: function (rowData) {
            var rowIndex = rowData.MenuId;
            var tr = $("div.datagrid-view2 div.datagrid-body tr.datagrid-row").eq(0);
            var tempId = tr.attr("id");
            var startIndex = tempId.lastIndexOf("-");
            var prefix = tempId.substr(0, startIndex + 1);
            $("#" + prefix + rowIndex + " input:enabled[funType='0']").attr("checked", "checked");
            $("#" + prefix + rowIndex + " input:enabled[funType='1']").attr("checked", "checked");
        },
        onUncheck: function (rowData) {
            var rowIndex = rowData.MenuId;
            var tr = $("div.datagrid-view2 div.datagrid-body tr.datagrid-row").eq(0);
            var tempId = tr.attr("id");
            var startIndex = tempId.lastIndexOf("-");
            var prefix = tempId.substr(0, startIndex + 1);
            $("#" + prefix + rowIndex + " input:enabled[funType='0']").removeAttr("checked");
            $("#" + prefix + rowIndex + " input:enabled[funType='1']").removeAttr("checked");
        }
    });
}

//设置数据权限范围
//obj:当前dom对象
//type:数据权限类型，0-数据浏览，1-数据编辑，2-数据删除
function SetDataPermissionRange(obj, type) {
    if (type < 0 || type > 2) return;
    var moduleName = $(obj).attr("moduleName");
    var moduleId = $(obj).attr("moduleId");
    var userName = $(obj).attr("userName");
    var userId = $(obj).attr("userId");
    var rowId = $(obj).attr("menuId");
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var obj = topWin.GetCurrentDialogFrame()[0].contentWindow.GetSelectedOrgs();
            //获取网格tr的id前缀
            var getPermissionGridPrefix = function () {
                var tr = $("div.datagrid-view2 div.datagrid-body tr.datagrid-row").eq(0);
                var tempId = tr.attr("id");
                var nodeId = tr.attr('node-id');
                return tempId.replace(nodeId, '');
            }
            //填充数据
            var setHtml = function (tempType) {
                var html = '';
                var spanName = tempType == 0 ? 'viewData' : (tempType == 1 ? 'editData' : 'delData');
                var field = tempType == 0 ? 'CanViewData' : (tempType == 1 ? 'CanEditData' : 'CanDelData');
                if (obj && obj.Org && obj.Org.length > 0) {
                    for (var i = 0; i < obj.Org.length; i++) {
                        var org = obj.Org[i];
                        html += '<span name="' + spanName + '" value="' + org.OrgId + '" menuId="' + rowId + '" moduleId="' + moduleId + '">【' + org.OrgName + '】</span>'
                    }
                }
                var typeName = tempType == 0 ? '浏览' : (tempType == 1 ? '编辑' : '删除');
                var imgHtml = '&nbsp;&nbsp;<img name="setPower" style="cursor:pointer;" title="设置【' + moduleName + '】数据' + typeName + '权限范围" src="/Css/icons/docEdit.png" moduleName="' + moduleName + '" moduleId="' + moduleId + '" userId="' + userId + '" userName="' + userName + '" menuId="' + rowId + '" onclick="SetDataPermissionRange(this,' + tempType + ')" />';
                html += imgHtml;
                var prefix = getPermissionGridPrefix();
                var tdObj = $("#" + prefix + rowId + " td[field='" + field + "']");
                tdObj.find("span[name='" + spanName + "']").remove();
                tdObj.find("img").remove();
                tdObj.css('text-align', 'center').append(html);
            }
            setHtml(type); //填充当前
            //应用到其他类型数据权限
            if (obj.AppType && obj.AppType.length > 0) {
                for (var i = 0; i < obj.AppType.length; i++) {
                    var appType = obj.AppType[i];
                    setHtml(appType);
                }
            }
            topWin.CloseDialog();
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var url = '/Page/UserDataPermissionSet.html?moduleName=' + encodeURI(moduleName) + '&userId=' + userId + '&type=' + type + '&r=' + Math.random();
    var title = "【" + moduleName + "】模块【" + userName + "】用户－";
    title += type == 0 ? "数据浏览" : (type == 1 ? "数据编辑" : "数据删除");
    title += "权限范围设置";
    topWin.OpenDialog(title, url, toolbar, 500, 520, 'eu-icon-crog');
}

//设置字段权限
//obj:当前dom对象
//type:字段权限类型，0-字段查看，1-字段新增，2-字段编辑
function SetFieldPermission(obj, type) {
    if (type < 0 || type > 2) return;
    var moduleName = $(obj).attr("moduleName");
    var moduleId = $(obj).attr("moduleId");
    var userName = $(obj).attr("userName");
    var userId = $(obj).attr("userId");
    var rowId = $(obj).attr("menuId");
    var toolbar = [{
        id: 'btnOk',
        text: "确 定",
        iconCls: "eu-icon-ok",
        handler: function (e) {
            var obj = topWin.GetCurrentDialogFrame()[0].contentWindow.GetSelectFields();
            //获取网格tr的id前缀
            var getPermissionGridPrefix = function () {
                var tr = $("div.datagrid-view2 div.datagrid-body tr.datagrid-row").eq(0);
                var tempId = tr.attr("id");
                var nodeId = tr.attr('node-id');
                return tempId.replace(nodeId, '');
            }
            //填充数据
            var setHtml = function (tempType) {
                var html = '';
                var spanName = tempType == 0 ? 'viewField' : (tempType == 1 ? 'addField' : 'editField');
                var field = tempType == 0 ? 'CanViewFields' : (tempType == 1 ? 'CanAddFields' : 'CanEditFields');
                if (obj && obj.Fields && obj.Fields.length > 0) {
                    for (var i = 0; i < obj.Fields.length; i++) {
                        var objField = obj.Fields[i];
                        html += '<span name="' + spanName + '" value="' + objField.FieldName + '" menuId="' + rowId + '" moduleId="' + moduleId + '">【' + objField.Display + '】</span>'
                    }
                }
                var typeName = tempType == 0 ? '查看' : (tempType == 1 ? '新增' : '编辑');
                var imgHtml = '&nbsp;&nbsp;<img name="setPower" style="cursor:pointer;" title="设置【' + moduleName + '】字段' + typeName + '权限" src="/Css/icons/docEdit.png" moduleName="' + moduleName + '" moduleId="' + moduleId + '" userId="' + userId + '" userName="' + userName + '" menuId="' + rowId + '" onclick="SetFieldPermission(this,' + tempType + ')" />';
                html += imgHtml;
                var prefix = getPermissionGridPrefix();
                var tdObj = $("#" + prefix + rowId + " td[field='" + field + "']");
                tdObj.find("span[name='" + spanName + "']").remove();
                tdObj.find("img").remove();
                $("#" + prefix + rowId + " td[field='" + field + "']").css('text-align', 'center').append(html);
            }
            setHtml(type); //填充当前
            //应用到其他类型数据权限
            if (obj.AppType && obj.AppType.length > 0) {
                for (var i = 0; i < obj.AppType.length; i++) {
                    var appType = obj.AppType[i];
                    setHtml(appType);
                }
            }
            topWin.CloseDialog();
        }
    }, {
        id: 'btnClose',
        text: '关 闭',
        iconCls: "eu-icon-close",
        handler: function (e) {
            topWin.CloseDialog();
        }
    }];
    var url = '/Page/UserFieldPermissionSet.html?moduleName=' + encodeURI(moduleName) + '&userId=' + userId + '&type=' + type + '&r=' + Math.random();
    var title = "【" + moduleName + "】模块【" + userName + "】用户－";
    title += type == 0 ? "字段查看" : (type == 1 ? "字段新增" : "字段编辑");
    title += "权限设置";
    topWin.OpenDialog(title, url, toolbar, 800, 400, 'eu-icon-crog');
}

//保存角色权限
//backFun:保存成功后的回调函数
function SaveUserPermission(backFun) {
    var tempFun = function () {
        var topMenuId = $('#bigModuleItem').combobox('getValue');
        var userId = tempUserId;
        if (!tempUserId) {
            var node = treeDom.tree('getSelected');
            userId = node.id;
        }
        var gridDom = $('#tb_permission');
        //获取所有行的menuId
        var menuIds = [];
        var getLeafMenuIds = function (node) {
            var menuIds = [];
            if (node.children && node.children.length > 0) {
                for (var i = 0; i < node.children.length; i++) {
                    var tempNode = node.children[i];
                    var tempMenuIds = getLeafMenuIds(tempNode);
                    if (tempMenuIds && tempMenuIds.length > 0) {
                        for (var j = 0; j < tempMenuIds.length; j++) {
                            menuIds.push(tempMenuIds[j]);
                        }
                    }
                }
            }
            else {
                menuIds.push(node.MenuId);
            }
            return menuIds;
        }
        var roots = gridDom.treegrid('getRoots');
        if (roots && roots.length > 0) {
            for (var i = 0; i < roots.length; i++) {
                var arr = getLeafMenuIds(roots[i]);
                for (var j = 0; j < arr.length; j++) {
                    menuIds.push(arr[j]);
                }
            }
        }
        if (!menuIds || menuIds.length == 0) {
            topWin.ShowAlertMsg('提示', '没有权限数据！', 'info');
            return;
        }
        var powerData = [];
        $.each(menuIds, function (i, menuId) {
            var permissonModel = null;
            //菜单
            var menuObj = $("input:enabled[funType='0'][menuId='" + menuId + "']");
            var tempMenuId = menuObj.val();
            if (menuId == tempMenuId && menuObj.attr("checked") == "checked") {
                permissonModel = { CanOpMeuId: menuId };
            }
            //网格按钮
            var btnIds = [];
            $("input:enabled[funType='1'][menuId='" + menuId + "']").each(function (i, btn) {
                if ($(btn).attr("checked") == "checked") {
                    btnIds.push($(btn).val());
                }
            });
            if (btnIds.length > 0) {
                if (permissonModel == null) permissonModel = {};
                permissonModel["CanOpBtnIds"] = btnIds;
            }
            //数据权限
            //数据查看权限
            var viewOrgIds = [];
            $("span[name='viewData'][menuId='" + menuId + "']").each(function (i, item) {
                var orgId = $(item).attr("value");
                if (orgId && orgId.length > 0) {
                    viewOrgIds.push(orgId);
                }
            });
            if (viewOrgIds.length > 0) {
                if (permissonModel == null) permissonModel = {};
                permissonModel["CanViewDataOrgIds"] = viewOrgIds;
            }
            //数据编辑权限
            var editOrgIds = [];
            $("span[name='editData'][menuId='" + menuId + "']").each(function (i, item) {
                var orgId = $(item).attr("value");
                if (orgId && orgId.length > 0) {
                    editOrgIds.push(orgId);
                }
            });
            if (editOrgIds.length > 0) {
                if (permissonModel == null) permissonModel = {};
                permissonModel["CanEditDataOrgIds"] = editOrgIds;
            }
            //数据删除权限
            var delOrgIds = [];
            $("span[name='delData'][menuId='" + menuId + "']").each(function (i, item) {
                var orgId = $(item).attr("value");
                if (orgId && orgId.length > 0) {
                    delOrgIds.push(orgId);
                }
            });
            if (delOrgIds.length > 0) {
                if (permissonModel == null) permissonModel = {};
                permissonModel["CanDelDataOrgIds"] = delOrgIds;
            }
            //字段权限
            //字段查看权限
            var viewFields = [];
            $("span[name='viewField'][menuId='" + menuId + "']").each(function (i, item) {
                var field = $(item).attr("value");
                if (field && field.length > 0) {
                    viewFields.push(field);
                }
            });
            if (viewFields.length > 0) {
                if (permissonModel == null) permissonModel = {};
                permissonModel["CanViewFields"] = viewFields;
            }
            //字段新增权限
            var addFields = [];
            $("span[name='addField'][menuId='" + menuId + "']").each(function (i, item) {
                var field = $(item).attr("value");
                if (field && field.length > 0) {
                    addFields.push(field);
                }
            });
            if (addFields.length > 0) {
                if (permissonModel == null) permissonModel = {};
                permissonModel["CanAddFields"] = addFields;
            }
            //字段编辑权限
            var editFields = [];
            $("span[name='editField'][menuId='" + menuId + "']").each(function (i, item) {
                var field = $(item).attr("value");
                if (field && field.length > 0) {
                    editFields.push(field);
                }
            });
            if (editFields.length > 0) {
                if (permissonModel == null) permissonModel = {};
                permissonModel["CanEditFields"] = editFields;
            }
            if (permissonModel != null) {
                var tempModuleId = menuObj.attr("moduleId");
                if (!tempModuleId) tempModuleId = $('#hd_module_' + menuId).val();
                permissonModel["ModuleId"] = menuObj.attr("moduleId");
                powerData.push(permissonModel);
            }
        });
        var data = { userId: userId, topMenuId: topMenuId, powerData: $.base64.encode(escape(JSON.stringify(powerData))) }
        $.ajax({
            type: "post",
            url: '/' + CommonController.Async_System_Controller + '/SaveUserPermission.html',
            data: data,
            beforeSend: function () {
                topWin.OpenWaitDialog('权限保存中...');
            },
            success: function (result) {
                if (result.Success) { //保存成功
                    topWin.ShowMsg('提示', '用户权限保存成功！', function () {
                        topWin.CloseWaitDialog();
                        if (typeof (backFun) == "function") {
                            backFun();
                        }
                    });
                }
                else {
                    topWin.ShowAlertMsg('提示', '用户权限保存失败，' + result.Message, "error", function () {
                        topWin.CloseWaitDialog();
                    });
                }
            },
            error: function (err) {
                topWin.ShowAlertMsg('提示', '用户权限保存失败，服务器异常！', "error", function () {
                    topWin.CloseWaitDialog();
                });
            },
            dataType: "json"
        });
    };
    var msgTitle = '保存提示';
    var msg = '保存用户权限后，该用户的角色权限将失效，用户权限将做为最终权限配置，确定要保存吗？'
    topWin.ShowConfirmMsg(msgTitle, msg, function (action) {
        if (action) {
            tempFun();
        }
    });
}

//搜索角色
function SearchUser() {
    var value = $('#userTxt').val();
    if (!isAsync) {
        if (!value || value.length == 0) {
            treeDom.find("div[id^='_easyui_tree_']").show();
            return;
        }
        var target = $("#userTree li span.eu-icon-user~ span.tree-title:contains('" + value + "')").parent();
        if (target.length <= 0) {
            return;
        }
        treeDom.find("div[id^='_easyui_tree_']").hide();
        $(target).show();
    }
    else {
        if (!value) {
            treeParams.queryParams = { q: '' };
            treeParams.async = true;
        }
        else {
            treeParams.queryParams = { q: value };
            treeParams.async = false;
        }
        LoadTree(treeDom, treeUrl, treeParams);
    }
}

//清除用户权限
function ClearUserPermission() {
    var userId = tempUserId;
    if (!tempUserId) {
        var node = treeDom.tree('getSelected');
        if (node && node.id)
            userId = node.id;
    }
    if (userId) {
        var msgTitle = '清除提示';
        var msg = '确认后该用户权限配置将会清除，确定要清除吗？'
        topWin.ShowConfirmMsg(msgTitle, msg, function (action) {
            if (action) {
                var url = '/' + CommonController.System_Controller + '/ClearUserPermission.html';
                ExecuteCommonAjax(url, { userId: userId }, function (result) {
                    if (result && result.Success) {
                        LoadUserPermission(userId)
                    }
                }, true);
            }
        });
    }
}
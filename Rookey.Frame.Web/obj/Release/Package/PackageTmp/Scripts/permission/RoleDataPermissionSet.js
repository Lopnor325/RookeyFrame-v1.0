
$(function () {
    //加载组织树
    $("#tree").tree({
        checkbox: true, //显示复选框
        cascadeCheck: true,//定义是否层叠选中状态
        url: '/User/GetDataPermissionOrgTree.html',
        loadFilter: function (data) {
            if (typeof (data) == 'string') {
                var tempData = eval("(" + data + ")");
                return tempData;
            }
            else {
                arr = [];
                arr.push(data);
                return arr;
            }
        },
        onLoadSuccess: function () {
            $("#tree").tree("collapseAll");
            var roots = $("#tree").tree("getRoots"); //展开所有根结点
            if (roots && roots.length > 0) {
                $.each(roots, function (i, root) {
                    $("#tree").tree("expand", root.target);
                });
            }
            //添加已存在的权限
            var dicStr = $('#selectedNodeList').attr('dic');
            if (dicStr && dicStr.length > 0) {
                dicStr = decodeURIComponent(dicStr);
                var dic = JSON.parse(dicStr);
                for (var orgId in dic) {
                    var node = $('#tree').tree('find', orgId);
                    if (!node) continue;
                    if (dic[orgId]) {
                        $(node.target).attr('fromParentRole', 'true').attr('title', '继承自父角色权限，不可去选');
                        $(node.target).find('span.tree-checkbox').unbind().click(function () {
                            return false;
                        });
                    }
                    $('#tree').tree('check', node.target);
                }
            }
        },
        onBeforeExpand: function (node) {
            var selectAllNode = false;
            var nodes = $('#tree').tree('getChecked');
            if (nodes && nodes.length > 0) {
                for (var i = 0; i < nodes.length; i++) {
                    if (nodes[i].id == -1) {
                        selectAllNode = true;
                        break;
                    }
                }
            }
            if (selectAllNode) { //'全部'节点已经被选中
                return false;
            }
        },
        onBeforeCheck: function (node, checked) {
            if (node.id != -1 && checked) {
                var selectAllNode = false;
                var nodes = $('#tree').tree('getChecked');
                if (nodes && nodes.length > 0) {
                    for (var i = 0; i < nodes.length; i++) {
                        if (nodes[i].id == -1) {
                            selectAllNode = true;
                            break;
                        }
                    }
                }
                if (selectAllNode) { //'全部'节点已经被选中
                    return false;
                }
            }
        },
        onCheck: function (node, checked) {
            var dom = $("#tree");
            if (node.id == -1) { //选择'全部'节点
                $("#selectedNodeList a").each(function (i, item) {
                    if ($(item).attr('orgId')) {
                        $(item).parent().remove();
                    }
                });
                var roots = dom.tree("getRoots"); //展开所有根结点
                if (checked) { //选中'全部'节点
                    //取消选中其他节点
                    if (roots && roots.length > 0) {
                        $.each(roots, function (i, root) {
                            var isFromParentRole = $(root.target).attr('fromParentRole');
                            if (root.id != -1 && !isFromParentRole) {
                                dom.tree("uncheck", root.target);
                            }
                        });
                    }
                    //添加'全部'节点
                    AddSelectedNode(node);
                    dom.tree("collapseAll");
                }
                else { //取消选中'全部'节点时展开其他根结点
                    if (roots && roots.length > 0) {
                        $.each(roots, function (i, root) {
                            dom.tree("expand", root.target);
                        });
                    }
                }
                return;
            }
            var item = $("#selectedNodeList").find("span[targetid='" + node.target.id + "']");
            //选择叶节点
            var flag = dom.tree("isLeaf", node.target);
            if (flag || node.state == 'closed') { //叶子节点或非叶子节点收缩状态时
                if (checked && item.length <= 0) {
                    AddSelectedNode(node);
                }
                else if (!checked && item.length > 0) {
                    item.parent().remove();
                }
            }//选择非叶节点
            else {
                var children = dom.tree("getChildren", node.target);
                $(children).each(function () {
                    var cnode = $(this)[0];
                    var it = $("#selectedNodeList").find("span[targetid='" + cnode.target.id + "']");
                    if (cnode.checked && it.length <= 0) {
                        AddSelectedNode(cnode);
                    }
                    else if (!cnode.checked && it.length > 0) {
                        it.parent().remove();
                    }
                });
            }
        }
    });
});

//搜索组织节点
function SearchNode(value) {
    var dom = $("#tree");
    var target = $("#tree li span.eu-icon-dept~ span.tree-title:contains('" + value + "')").parent();
    if (target.length <= 0) {
        top.ShowMsg("提示", "未找到任何相关节点");
        return;
    }
    $(target).each(function () {
        var tt = $(this);
        var parentNode = dom.tree("getParent", tt);
        while (parentNode != null) {
            if (parentNode != null) {
                dom.tree("expand", parentNode.target);
                parentNode = dom.tree("getParent", parentNode.target);
            }
        }
    });
    dom.tree("select", target);
}

//已设置或选择添加记录
//node:组织节点
function AddSelectedNode(node) {
    if (!node) return;
    var nodeList = $("#selectedNodeList");
    var dom = document.createDocumentFragment();
    var span = document.createElement("span");
    var fromParentPower = $(node.target).attr('fromParentRole') == 'true'; //是否继承父角色权限
    var title = "删除";
    var clickMethod = "UnSelect(this)";
    var spClass = "attaDelete";
    var spText = "删除";
    $(span).attr("class", "attaItem");
    var a = document.createElement("a");
    $(a).attr("href", "javascript:void(0);");
    if (fromParentPower) {
        $(a).attr("fromParentRole", "true");
        title = "权限继承父角色，不可删除";
        clickMethod = "";
        spClass = "attaNoDelete";
        spText = "";
    }
    else {
        $(a).attr("orgId", node.id);
    }
    $(a).text(node.text);
    var sp = document.createElement("span");
    $(sp).attr("class", spClass);
    if (!fromParentPower) {
        $(sp).attr("targetId", node.target.id);
    }
    $(sp).attr("title", title);
    $(sp).attr("onclick", clickMethod);
    $(sp).text(spText);
    span.appendChild(a);
    span.appendChild(sp);
    dom.appendChild(span);
    nodeList[0].appendChild(dom);
}

//取消选择
function UnSelect(obj) {
    try{
        var targetId = $(obj).attr("targetId");
        var target = $("#" + targetId);
        var dom = $("#tree");
        dom.tree("uncheck", target);
    } catch (e) { }
    $(obj).parent("span.attaItem").remove();
}

//获取已选择的组织
function GetSelectedOrgs() {
    var org = [];
    $("#selectedNodeList a").each(function (i, item) {
        var orgId = $(this).attr("orgId");
        if (orgId) {
            var orgName = $(this).text();
            org.push({ OrgId: orgId, OrgName: orgName });
        }
    });
    var appType = [];
    if ($('#chk_appToType1').attr('checked') == 'checked') {
        appType.push($('#chk_appToType1').attr('appType'));
    }
    if ($('#chk_appToType2').attr('checked') == 'checked') {
        appType.push($('#chk_appToType2').attr('appType'));
    }
    return { Org: org, AppType: appType };
}
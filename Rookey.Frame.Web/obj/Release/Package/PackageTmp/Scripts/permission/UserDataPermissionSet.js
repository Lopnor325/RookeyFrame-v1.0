var baseUrl = '/' + CommonController.Async_User_Controller + '/GetDataPermissionOrgTree.html';
var isAsync = true; //是否异步树
var treeUrl = isAsync ? baseUrl + '?async=1' : baseUrl;
var treeDom = $('#tree');
var treeParams = {
    checkbox: true, //显示复选框
    cascadeCheck: true,//定义是否层叠选中状态
    url: treeUrl,
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
        if (!isAsync) {
            treeDom.tree("collapseAll");
            var roots = treeDom.tree("getRoots"); //展开所有根结点
            if (roots && roots.length > 0) {
                $.each(roots, function (i, root) {
                    treeDom.tree("expand", root.target);
                });
            }
        }
        //添加已存在的权限
        var dicStr = $('#selectedNodeList').attr('dic');
        if (dicStr && dicStr.length > 0) {
            dicStr = decodeURIComponent(dicStr);
            var dic = JSON.parse(dicStr);
            for (var orgId in dic) {
                var node = treeDom.tree('find', orgId);
                if (!node) continue;
                treeDom.tree('check', node.target);
            }
        }
    },
    onBeforeExpand: function (node) {
        var selectAllNode = false;
        var nodes = treeDom.tree('getChecked');
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
            var nodes = treeDom.tree('getChecked');
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
        if (node.id == -1) { //选择'全部'节点
            $("#selectedNodeList a").each(function (i, item) {
                if ($(item).attr('orgId')) {
                    $(item).parent().remove();
                }
            });
            var roots = treeDom.tree("getRoots"); //展开所有根结点
            if (checked) { //选中'全部'节点
                //取消选中其他节点
                if (roots && roots.length > 0) {
                    $.each(roots, function (i, root) {
                        if (root.id != -1) {
                            treeDom.tree("uncheck", root.target);
                        }
                    });
                }
                //添加'全部'节点
                AddSelectedNode(node);
                treeDom.tree("collapseAll");
            }
            else { //取消选中'全部'节点时展开其他根结点
                if (roots && roots.length > 0) {
                    $.each(roots, function (i, root) {
                        treeDom.tree("expand", root.target);
                    });
                }
            }
            return;
        }
        var item = $("#selectedNodeList").find("span[targetid='" + node.target.id + "']");
        //选择叶节点
        var flag = treeDom.tree("isLeaf", node.target);
        if (flag || node.state == 'closed') { //叶子节点或非叶子节点收缩状态时
            if (checked && item.length <= 0) {
                AddSelectedNode(node);
            }
            else if (!checked && item.length > 0) {
                item.parent().remove();
            }
        }//选择非叶节点
        else {
            var children = treeDom.tree("getChildren", node.target);
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
};

$(function () {
    //加载组织树
    treeDom.tree(treeParams);
});

//搜索组织节点
function SearchNode(value) {
    if (!isAsync) {
        if (!value || value.length == 0)
            return;
        var target = $("#tree li span.eu-icon-dept~ span.tree-title:contains('" + value + "')").parent();
        if (target.length <= 0) {
            top.ShowMsg("提示", "未找到任何相关节点");
            return;
        }
        $(target).each(function () {
            var tt = $(this);
            var parentNode = treeDom.tree("getParent", tt);
            while (parentNode != null) {
                if (parentNode != null) {
                    treeDom.tree("expand", parentNode.target);
                    parentNode = treeDom.tree("getParent", parentNode.target);
                }
            }
        });
        treeDom.tree("select", target);
    }
    else {
        if (!value) {
            treeParams.queryParams = { q: '' };
            treeParams.url = treeUrl;
        }
        else {
            treeParams.queryParams = { q: value };
            treeParams.url = baseUrl;
        }
        treeDom.tree(treeParams);
    }
}

//已设置或选择添加记录
//node:组织节点
function AddSelectedNode(node) {
    if (!node) return;
    var nodeList = $("#selectedNodeList");
    var dom = document.createDocumentFragment();
    var span = document.createElement("span");
    var title = "删除"
    var clickMethod = "UnSelect(this)";
    var spClass = "attaDelete";
    var spText = "删除";
    $(span).attr("class", "attaItem");
    var a = document.createElement("a");
    $(a).attr("href", "javascript:void(0);");
    $(a).attr("orgId", node.id);
    $(a).text(node.text);
    var sp = document.createElement("span");
    $(sp).attr("class", spClass);
    $(sp).attr("targetId", node.target.id);
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
        treeDom.tree("uncheck", target);
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
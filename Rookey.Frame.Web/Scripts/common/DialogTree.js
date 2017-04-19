var moduleId = GetLocalQueryString("moduleId");
var moduleName = GetLocalQueryString("moduleName");
var fieldName = GetLocalQueryString("fieldName"); //字段名
var hasId = GetLocalQueryString("hasId") == '1'; //当传了fieldName后是否包含Id字段
var isMutiSelect = GetLocalQueryString("ms") == '1'; //是不多选
var isAsync = GetLocalQueryString("async") == '1'; //是否异步加载
var cascadeCheck = GetLocalQueryString("cccheck") == '1'; //是否层叠选中状态
var onlyLeaf = GetLocalQueryString("onlyleaf") == '1'; //是否只能选择叶子节点
var baseUrl = '/' + CommonController.Async_Data_Controller + '/GetTreeByNode.html?moduleId=' + moduleId + '&moduleName=' + moduleName;
var treeUrl = baseUrl;
if (fieldName)
    treeUrl += "&fieldName=" + fieldName;
if (hasId)
    treeUrl += "&hasId=1";
if (isAsync)
    treeUrl += "&async=1";
var treeDom = $("#tree");
var treeParams = {
    checkbox: isMutiSelect, //显示复选框
    cascadeCheck: cascadeCheck,//定义是否层叠选中状态
    onlyLeafCheck: onlyLeaf, //是否只能选择叶子节点
    animate: true,
    url: treeUrl,
    loadFilter: function (data) {
        if (data == null) return data;
        var lastData = null;
        if (typeof (data) == 'string') {
            var tempData = eval("(" + data + ")");
            lastData = tempData;
        }
        else if (data instanceof Array) { //是否为数组
            return data;
        }
        else {
            arr = [];
            arr.push(data);
            lastData = arr;
        }
        if (typeof (OverDialogTreeLoadFilter) == "function") {
            lastData = OverDialogTreeLoadFilter(lastData, moduleName, moduleId);
        }
        return lastData;
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
    },
    onSelect: function (node) {
        if (node.id == GuidEmpty) return;
        var item = $("#selectedNodeList").find("span[targetid='" + node.target.id + "']");
        if (item.length <= 0) {
            AddSelectedNode(node);
        }
    },
    onCheck: function (node, checked) {
        if (node.id == GuidEmpty) return;
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
        }
        else { //选择非叶节点
            if (cascadeCheck) {
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
    }
};

$(function () {
    //加载组织树
    treeDom.tree(treeParams);
});

//搜索节点
function SearchNode(value) {
    if (!isAsync) { //同步
        if (!value) return;
        var target = $("#tree li span.tree-title:contains('" + value + "')").parent();
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
    else { //异步
        if (!value) {
            treeParams.url = treeUrl;
            treeParams.queryParams = { q: '' };
        }
        else {
            treeParams.url = baseUrl;
            treeParams.queryParams = { q: value };
        }
        treeDom.tree(treeParams);
    }
}

//已设置或选择添加记录
//node:节点
function AddSelectedNode(node) {
    if (!node) return;
    var nodeList = $("#selectedNodeList");
    if (!isMutiSelect) nodeList.html('');
    var dom = document.createDocumentFragment();
    var span = document.createElement("span");
    var title = "删除"
    var clickMethod = "UnSelect(this)";
    var spClass = "attaDelete";
    var spText = "删除";
    $(span).attr("class", "attaItem");
    var a = document.createElement("a");
    $(a).attr("href", "javascript:void(0);");
    $(a).text(node.text);
    $(a).attr("dataId", node.id);
    var sp = document.createElement("span");
    $(sp).attr("class", spClass);
    $(sp).attr("title", title);
    $(sp).attr("onclick", clickMethod);
    $(sp).attr("targetId", node.target.id);
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

//获取已选数据
//isMutiSelect:是否多选
function GetSelectData() {
    var data = [];
    $("#selectedNodeList a").each(function (i, item) {
        var dataId = $(this).attr("dataId");
        if (dataId) {
            var dataText = $(this).text();
            var obj = { Id: dataId, Name: dataText };
            data.push(obj);
        }
    });
    return isMutiSelect ? data : data[0];
}
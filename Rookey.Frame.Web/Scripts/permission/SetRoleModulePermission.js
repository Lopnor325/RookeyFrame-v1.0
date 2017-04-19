//初始化
$(function () {
    //加载角色树
    LoadTree($('#roleTree'), '/' + CommonController.Async_Data_Controller + '/GetTreeByNode.html?moduleName=' + encodeURI("角色管理"));
    //加载菜单树
    LoadTree($('#menuTree'), '/' + CommonController.Async_Data_Controller + '/GetTreeByNode.html?moduleName=' + encodeURI("菜单管理"));
    //设置右边tab左边框
    $("#tabs_permission").parent().css('border-left-width', '1px').css('border-left-style', 'solid').css('border-left-color', topWin.GetBorderColor());
    //权限设置panel左边框
    $("#permisson_center").parent().css('border-left-width', '1px').css('border-left-style', 'solid').css('border-left-color', topWin.GetBorderColor());
});

//单击角色树节点事件
function TreeNodeOnClick(node) {

}
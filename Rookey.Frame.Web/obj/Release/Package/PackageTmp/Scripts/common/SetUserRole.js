
//获取选中的角色Id集
function GetSelectRoles() {
    var roleIds = '';
    $("input[id^='chk_']").each(function () {
        if ($(this).attr("checked") == "checked") {
            roleIds += $(this).val() + ',';
        }
    });
    if (roleIds.length > 0) {
        roleIds = roleIds.substr(0, roleIds.length - 1);
    }
    return roleIds;
}
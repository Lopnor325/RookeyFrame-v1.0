
$(function () {
    IconBindEvent();
});

//绑定图标事件
function IconBindEvent() {
    $("div[id^='div_icon_'] td").mouseover(function (e) {
        if (!$(this).hasClass("selected")) {
            $(this).css("background", "#ECF5FF");
        }
    }).mouseout(function (e) {
        if (!$(this).hasClass("selected")) {
            $(this).css("background", "");
        }
    }).mousedown(function (e) {
        $("div[id^='div_icon_'] td").css("background", "").removeClass("selected");
        $(this).css("background", "#FFE66F").addClass("selected");
    });
}

//翻页事件
//pageIndex:页码
//pageSize：每页记录数
//iconType：图标类型
function IconPageSelected(pageIndex, pageSize,iconType) {
    $.get("/" + CommonController.Async_System_Controller + "/GetIconPageHtml.html", { pageIndex: pageIndex, pageSize: pageSize, iconType: iconType },
        function (html) {
        if (html) {
            var div = $("#icon_tab").tabs('getSelected').find("div[id^='div_icon_']");
            div.html(html);
            IconBindEvent(); //重新绑定图标事件
        }
    });
}

//获取选择的图标
function GetSelectedIcon() {
    var dom = $("div[id^='div_icon_'] td.selected img");
    if (dom.length == 0) {
        topWin.ShowMsg('提示', '请选择图标！');
        return null;
    }
    var imgUrl = dom.attr("src"); //图标url
    var styleClassName = dom.attr("styleName"); //图标样式类名
    return { ImgUrl: imgUrl, StyleClassName: styleClassName };
}
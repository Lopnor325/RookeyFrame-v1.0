//初始化
$(function () {
    document.onkeydown = ForbidBackSpace;
    //禁止后退键 作用于Firefox、Opera  
    document.onkeypress = ForbidBackSpace;
});

//-----系统常用函数---------------------------------------------------------------
//禁止后退键、退格键
function ForbidBackSpace(e) {
    //按ESC键退出系统
    var ev = arguments[0] || window.event;
    if (ev.keyCode == 27) {

    }
    //退格键 
    if (ev.keyCode == 8) {
        var obj = ev.target || ev.srcElement; //获取事件源   
        var t = obj.type || obj.getAttribute('type'); //获取事件源类型
        if (!!t) {
            var vReadOnly = obj.readOnly;
            var vDisabled = obj.disabled;
            var contentEditable = obj.getAttribute('contentEditable');
            contentEditable = (contentEditable == undefined) ? false : contentEditable;

            vReadOnly = (vReadOnly == undefined) ? false : vReadOnly;
            vDisabled = (vDisabled == undefined) ? true : vDisabled;
            if (((t == "password" || t == "text" || t == "textarea") && (!vReadOnly && !vDisabled)) || contentEditable) {
                return true;
            }
            else
                return false;
        }
        else {
            return false;
        }
    }
}

//浏览器检测相关方法
window["MzBrowser"] = {}; (function () {
    if (MzBrowser.platform) return;
    var ua = window.navigator.userAgent;
    MzBrowser.platform = window.navigator.platform;
    MzBrowser.firefox = ua.indexOf("Firefox") > 0;
    MzBrowser.opera = typeof (window.opera) == "object";
    MzBrowser.ie = !MzBrowser.opera && ua.indexOf("MSIE") > 0;
    MzBrowser.mozilla = window.navigator.product == "Gecko";
    MzBrowser.netscape = window.navigator.vendor == "Netscape";
    MzBrowser.safari = ua.indexOf("Safari") > -1;
    if (MzBrowser.firefox) var re = /Firefox(\s|\/)(\d+(\.\d+)?)/;
    else if (MzBrowser.ie) var re = /MSIE( )(\d+(\.\d+)?)/;
    else if (MzBrowser.opera) var re = /Opera(\s|\/)(\d+(\.\d+)?)/;
    else if (MzBrowser.netscape) var re = /Netscape(\s|\/)(\d+(\.\d+)?)/;
    else if (MzBrowser.safari) var re = /Version(\/)(\d+(\.\d+)?)/;
    else if (MzBrowser.mozilla) var re = /rv(\:)(\d+(\.\d+)?)/;
    if ("undefined" != typeof (re) && re.test(ua))
        MzBrowser.version = parseFloat(RegExp.$2);
})();

//浏览器名称
function GetBrowserName() {
    var name = "undefined";
    if (MzBrowser.ie) { name = "ie"; }
    else if (MzBrowser.firefox) { name = "firefox"; }
    else if (MzBrowser.safari) { name = "safari"; }
    return name;
}

//浏览器版本
function GetVersion() {
    return MzBrowser.version;
}

//是否IE
function IsIE(versionValue) {
    var name = GetBrowserName();
    var version = GetVersion();
    if (name == 'ie' && parseInt(version) == versionValue)
        return true;
    else
        return false;
}

//是否是IE8及以下
function IsLowIE8() {
    var name = GetBrowserName();
    var version = GetVersion();
    if (name == "ie" && parseInt(version) < 9) {
        return true;
    }
    return false;
}

//窗口是否是最小化
function IsMinStatus() {
    var isMin = false;
    if (window.outerWidth != undefined) {
        isMin = window.outerWidth <= 160 && window.outerHeight <= 27;
    }
    else {
        isMin = window.screenTop < -30000 && window.screenLeft < -30000;
    }
    return isMin;
}

//最大化窗口
function MaxWinToAll() {
    try {
        window.moveTo(0, 0);
        if (document.all) { window.resizeTo(screen.availWidth, screen.availHeight); }
        else if (document.layers || document.getElementById) {
            if (window.outerHeight < screen.availHeight || window.outerWidth < screen.availWidth) {
                window.outerHeight = screen.availHeight;
                window.outerWidth = screen.availWidth;
            }
        }
    }
    catch (ex) {

    }
}

//出现滚动条
function IsHasScrollBar() {
    return document.body.scrollTop > 0;
}

//阻止事件冒泡的通用函数  
function StopBubble(e) {
    // 如果传入了事件对象，那么就是非ie浏览器  
    if (e && e.stopPropagation) {
        //因此它支持W3C的stopPropagation()方法  
        e.stopPropagation();
    } else {
        //否则我们使用ie的方法来取消事件冒泡  
        window.event.cancelBubble = true;
    }
}

//禁止右键菜单
function DisableContextMenu() {
    $(document).bind("contextmenu", function (e) {
        return false;
    });
}
//-----end-----------------------------------------------------

//获取url里的参数值 name:参数名称
function GetQueryStringByUrl(source, name) {
    var reg = new RegExp("(^|\\?|&)" + name + "=([^&]*)(\\s|&|$)", "i");
    if (reg.test(source))
        return RegExp.$2;
    return "";
};

//获取本地url里的参数值
function GetLocalQueryString(name) {
    var _url = "http://" + document.location;
    return GetQueryStringByUrl(_url, name);
}

//取cookies函数    
function GetCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null)
        return unescape(arr[2]);
    return null;
}

//两个参数，一个是cookie的名子，一个是值  
function SetCookie(name, value, time) {
    var cookie = name + "=" + escape(value);
    if (time) {
        var exp = new Date();
        exp.setDate(exp.getDate() + time);
        cookie += ";expires=" + exp.toGMTString();
    }
    document.cookie = cookie;
    document.cookie = cookie;
}

//删除cookie  
function DelCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = GetCookie(name);
    if (cval != null) document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

//取网页可见区域高
function GetBodyHeight() {
    var winHeight = 0;
    //获取窗口高度
    if (window.innerHeight)
        winHeight = window.innerHeight;
    else if ((document.body) && (document.body.clientHeight))
        winHeight = document.body.clientHeight;
    //通过深入Document内部对body进行检测，获取窗口大小
    if (document.documentElement && document.documentElement.clientHeight) {
        winHeight = document.documentElement.clientHeight;
    }
    return winHeight;
}

//取网页可见区域宽
function GetBodyWidth() {
    var winWidth = 0;
    //获取窗口宽度
    if (window.innerWidth)
        winWidth = window.innerWidth;
    else if ((document.body) && (document.body.clientWidth))
        winWidth = document.body.clientWidth;
    //通过深入Document内部对body进行检测，获取窗口大小
    if (document.documentElement && document.documentElement.clientWidth) {
        winWidth = document.documentElement.clientWidth;
    }
    return winWidth;
}

//格式化
function FormatUnits(size) {
    if (isNaN(size) || size == null) {
        size = 0;
    }
    if (size <= 0) return size + "bytes";

    var t1 = (size / 1024).toFixed(2);
    if (t1 < 0) {
        return "0KB";
    }
    if (t1 > 0 && t1 < 1024) {
        return t1 + "KB";
    }
    var t2 = (t1 / 1024).toFixed(2);
    if (t2 < 1024)
        return t2 + "MB";
    return (t2 / 1024).toFixed(2) + "GB";
}

//动态调用方法、并传递参数
//fn:函数
//args:参数数组
//backFun:回调函数
//举例:doCallback(eval("callback"),['a','b']);    
//doCallback(callback, ['a', 'b', 'c']);
function DoCallback(fn, args, backFun) {
    fn.apply(this, args);
    if (typeof (backFun) == "function") {
        backFun();
    }
}

//---------智能提示，自动完成功能--------------------------------------
//dom-要实现自动完成的jquery输入对象
//urlOrData-json数据或者URL
//backFun-选择完成回调函数
//matchCase-是否非法字符过滤
//paramObj:传递的参数
function AutoComplete(dom, urlOrData, backFun, matchCase, paramObj) {
    dom.unautocomplete();
    dom.autocomplete(urlOrData, {
        matchCase: matchCase, //如果设置为true，autoComplete只会允许匹配的结果出现在输入框,所有当用户输入的是非法字符时将会得不到下拉框，Default:false
        matchSubset: false,//缓存问题，默认为true，在缓存中读取
        minChars: 0, //表示在自动完成激活之前填入的最小字符
        max: 10, //表示列表里的条目数
        autoFill: false, //表示自动填充
        mustMatch: false, //表示必须匹配条目,文本框里输入的内容,必须是data参数里的数据,如果不匹配,文本框就被清空
        matchContains: true, //表示包含匹配,相当于模糊匹配
        width: 200,
        scrollHeight: 200, //表示列表显示高度,默认高度为180
        dataType: "json", //json类型
        //需要把data转换成json数据格式
        parse: function (data) {
            if (data == null || data == "") return null;
            return $.map(data, function (row) {
                return {
                    data: row,
                    value: row["Name"],
                    result: row["f_Name"]
                }
            });
        },
        formatItem: function (row, i, max) {
            if (row != null) {
                return row["Name"];
            }
        },
        formatResult: function (row) {
            if (row != null) {
                return row["Name"];
            }
        }
    }).result(function (event, item, formatted) {
        if (typeof (backFun) == "function") { //完成选择后
            backFun(item, dom, paramObj);
        }
    });
}
//---------------------------------------------------------------------

//创建一个iframe
function CreateIFrame(url) {
    var s = '<iframe scrolling="auto" frameborder="0" class="ifr"  src="' + url + '" style="width: 100%;height: 100%;"></iframe>';
    return s;
}

/*--------------------全局消息处理，跨域处理----------------------------*/
//全局消息对象
var GlobalMessagerObject = null;
//注册子系统消息信使
//name:信使名称
//projectName:项目名称
//msgReceivedCallbackMethod:消息接收回调函数
function RegChildSysMessager(name, msgReceivedCallbackMethod) {
    if (GlobalMessagerObject == null) {
        GlobalMessagerObject = new Messenger(name);
        GlobalMessagerObject.listen(function (msg) {
            if (typeof (msgReceivedCallbackMethod) == "function") {
                msgReceivedCallbackMethod(name, msg);
            }
        });
        GlobalMessagerObject.addTarget(window.parent, 'parent');
    }
}

//向父系统发送消息
function SendMsgToParent(msg) {
    if (GlobalMessagerObject) {
        GlobalMessagerObject.targets['parent'].send(msg);
    }
}
/*--------------------------------------------------------------------*/
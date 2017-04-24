//初始化
$(function () {
    var noself = GetLocalQueryString("noself");
    if (noself != "1") {
        if (top.location != self.location) top.location = self.location;
    }
    if (GetCookie("UserName") && GetCookie("UserName") != null)
        $("#txtUserName").val(GetCookie("UserName"));
    $(document).keydown(function (e) {
        if (e.keyCode == 13) {
            DoLogin();
        }
    });
});

//获取本地url里的参数值
function GetLocalQueryString(name) {
    var _url = "http://" + document.location;
    var reg = new RegExp("(^|\\?|&)" + name + "=([^&]*)(\\s|&|$)", "i");
    if (reg.test(_url))
        return RegExp.$2;
    return "";
}

//取cookies函数    
function GetCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null)
        return unescape(arr[2]);
    return null;
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

//用户登录
function DoLogin() {
    var username = $("#txtUserName").val();
    var password = $("#txtPwd").val();
    if (username == "") {
        window.alert("账号不能为空！");
        $("#txtUserName").focus();
        return;
    }
    if (password == "") {
        window.alert("密码不能为空！");
        $("#txtPwd").focus();
        return;
    }
    $.ajax({
        type: 'post',
        dataType: 'json',
        url: '/UserAsync/UserLogin.html',
        data: { username: username, userpwd: password, valcode: '', isNoCode: true },
        success: function (result) {
            if (!result.Success) {
                $('#login').removeAttr('disabled');
                window.alert(result.Message);
                $("#txtUserName").focus();
            } else {
                var fromUrl = decodeURIComponent(GetLocalQueryString("fromUrl"));
                if (!fromUrl) {
                    fromUrl = "/Page/Main.html";
                }
                location.href = fromUrl;
            }
        },
        error: function () {
            $('#login').removeAttr('disabled');
            window.alert('发生系统错误，请与系统管理员联系！');
        },
        beforeSend: function () {
            $('#login').attr('disabled', 'disabled');
        }
    });
}
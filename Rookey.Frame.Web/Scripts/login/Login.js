var isShowCode = false; //是否需要输入验证码

//初始化
$(function () {
    var noself = GetLocalQueryString("noself");
    if (noself != "1") {
        if (top.location != self.location) top.location = self.location;
    }
    if (GetCookie("UserName") && GetCookie("UserName") != null)
        $("#txtUserName").textbox('setValue',GetCookie("UserName"))
    $(document).keydown(function (e) {
        if (e.keyCode == 13) {
            DoLogin();
        }
    });
    isShowCode = $("#txtShowCode").val() == "true"; 
    if (isShowCode)
        $('#tr_validcode').show();
    else
        $('#tr_validcode').hide();
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
    var showMsg = function (msg) {
        $.messager.show({
            title: '登录提示',
            msg: msg,
            timeout: 500,
            showType: 'slide',
            style: {
                right: '',
                bottom: ''
            }
        });
    };
    var username = $("#txtUserName").textbox('getValue');
    var password = $("#txtPwd").textbox('getValue');
    var validate = $("#txtValidate").textbox('getValue');
    if (username == "") {
        showMsg("账号不能为空！");
        $("#txtUserName").focus();
        return;
    }
    if (password == "") {
        showMsg("密码不能为空！");
        $("#txtPwd").focus();
        return;
    }
    if (isShowCode) {
        if (validate == "") {
            showMsg("验证码不能为空！");
            $("#txtValidate").focus();
            return;
        }
        else if (validate.length != 4) {
            showMsg("验证码错误！");
            $("#txtValidate").focus();
            return;
        }
    }
    $.ajax({
        type: 'post',
        dataType: 'json',
        url: '/UserAsync/UserLogin.html',
        data: [
        { name: 'username', value: username },
        { name: 'userpwd', value: password },
        { name: 'valcode', value: validate },
        { name: 'w', value: GetBodyWidth() },
        { name: 'h', value: GetBodyHeight() },
        { name: 'r', value: Math.random() }
        ],
        success: function (result) {
            if (!result) {
                showMsg('登陆失败，服务器无响应！');
                return;
            }
            if (!result.Success) {
                showMsg(result.Message);
                isShowCode = result.IsShowCode;
                if (isShowCode) {
                    $('#tr_validcode').show();
                    $("#validate").click();
                }
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
            $("#validate").click();
            showMsg('发生系统错误，请与系统管理员联系！');
        },
        beforeSend: function () {
            $('#btnLogin').linkbutton('disable');
        },
        complete: function () {
            $("#btnLogin").linkbutton("enable");
        }
    });
}

//关闭窗口
function Close() {
    $.messager.confirm('关闭窗口', '您确定要放弃登录吗？', function (r) {
        if (r) {
            top.window.close();
        }
    });
}
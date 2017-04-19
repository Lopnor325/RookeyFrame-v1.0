$(function () {
    var themes = [
			{ value: 'default', text: '天空蓝', group: '基本' },
			{ value: 'gray', text: '亮白银', group: '基本' },
			{ value: 'metro', text: '清新白', group: '基本' },
			{ value: 'bootstrap', text: '响应灰', group: '基本' },
			{ value: 'black', text: '炫酷黑', group: '基本' },
			{ value: 'metro-blue', text: '扁平蓝', group: '扁平' },
			{ value: 'metro-gray', text: '扁平灰', group: '扁平' },
			{ value: 'metro-green', text: '扁平绿', group: '扁平' },
			{ value: 'metro-orange', text: '扁平黄', group: '扁平' },
			{ value: 'metro-red', text: '扁平红', group: '扁平' },
			{ value: 'ui-cupertino', text: '清爽蓝', group: '其他' },
			{ value: 'ui-dark-hive', text: '护眼黑', group: '其他' },
			{ value: 'ui-pepper-grinder', text: '胡椒灰', group: '其他' },
			{ value: 'ui-sunny', text: '金太阳', group: '其他' }
    ];
    $('#cb-theme').combobox({
        groupField: 'group',
        data: themes,
        editable: false,
        panelHeight: 'auto',
        onLoadSuccess: function () {
            var cookieTheme = GetCookie('easyuitheme');
            if (cookieTheme == undefined || cookieTheme == null)
                cookieTheme = top.defaultTheme;
            $(this).combobox('setValue', cookieTheme);
        }
    });
});

//获取设置样式
function GetTheme() {
    var cookieTheme = GetCookie('easyuitheme');
    var theme = $('#cb-theme').combobox('getValue');
    if (theme != cookieTheme) {
        SetCookie('easyuitheme', theme);
        return theme;
    }
    return null;
}
/**
 * 设置文本框输入限制的默认配置
 *  type: 文本框只能输入的类型，合法的值有:
 *          num: 只允许是数字，ch: 只能是字母，numch: 数字或字母，zto: 0-1值浮点数，fnum: 浮点数
 *  len: 允许输入的最大个数
 *  intLen: 当为浮点数时允许的整数的最大位数, 默认为：0 不限制，如果为zto是此属性无作用
 *  flatLen: 当为浮点数时允许的小数部分的最大位数。
 */

; (function ($) {
    $.fn.onlyAllowEnter = function (settings) {
        options = $.extend({}, $.fn.onlyAllowEnter.defaults, settings);
        if (this.length == 0) {
            debug('Selector invalid or missing!');
            return;
        } else if (this.length > 1) {
            return this.each(function () {
                $.fn.onlyAllowEnter.apply($(this), [settings]);
            });
        }

        switch (options['type']) {
            case 'num':
                $(this).unbind('keydown').keydown(allowEnterNumber);
                break;
            case 'ch':
                $(this).unbind('keydown').keydown(allowEnterChar);
                break;
            case 'numch':
                $(this).unbind('keydown').keydown(allowEnterNumberOrChar);
                break;
            case 'zto':
                $(this).unbind('keydown').keydown(allowEnterNumberZero2One);
                break;
            case 'fnum':
                $(this).unbind('keydown').keydown(allowEnterFloat);
                break;
            default:
                return;
        }

    };

    function debug(message) {
        if (!window.console) {
            window.console = {};
            window.console.log = function () {
                return;
            }
        }
        window.console.log(message + ' ');
    };

    /**
     * 设置文本框输入限制的默认配置
     *  type: 文本框只能输入的类型，合法的值有:
     *          num: 只允许是数字，ch: 只能是字母，numch: 数字或字母，zto: 0-1值浮点数，fnum: 浮点数
     *  len: 允许输入的最大个数
     *  intLen: 当为浮点数时允许的整数的最大位数, 默认为：0 不限制，如果为zto是此属性无作用
     *  flatLen: 当为浮点数时允许的小数部分的最大位数。
     */
    $.fn.onlyAllowEnter.defaults = {
        'type': '',
        'len': 0,
        'intLen': 0,
        'flatLen': 0
    };

    /**
     * 方法说明
     *      只允许某个文本框中输入0~1之间的数值。当用户输入数值时，
     *    会自动在输入的数值前面添加'0.';
     * 参数说明：
     *     @param element input文本框对象
     * 使用方式：
     *     为指定的文本框绑定onKeyDown事件，
     */
    function allowEnterNumberZero2One() {
        var keyCode = event.keyCode;
        var _val = $(this).val();
        if (_val == '' || !(/^0\./).test(_val)) {
            $(this).val('0.');
        }
        if (!onlyNumber(keyCode)) {
            return false;
        }
        var len = options['flatLen'];
        _val = $(this).val().substring(2);
        debug(_val);
        if (len != undefined && len != 0 && _val.length >= len && isBackspace(keyCode)) {
            return false;
        }
        return true;
    };

    /** 
     * 方法说明：
     *     判断输入的数值是否为数字、删除、退格、左移或右移键
     * 参数说明：
     *  @param keyCode 输入的键盘的键值
     */
    function onlyNumber(keyCode) {
        if (isBackspace(keyCode) && (keyCode > 57 || keyCode < 48)) {
            return false;
        }
        return true;
    };

    function isBackspace(keyCode) {
        return keyCode != 8 && keyCode != 46 && keyCode != 37 && keyCode != 39;
    }

    /**
     * 方法说明：
     *   设置一个文本框中可以输入数值或浮点数。
     * 参数说明：
     *   @param element input文本框对象
     * 使用方式
     *   为指定的文本框绑定onKeyDown事件
     */
    function allowEnterFloat() {
        var keyCode = event.keyCode;
        var _val = $(this).val();
        if (keyCode == 190) {
            if (_val == '') {
                $(this).val('0');
            } else if (_val.indexOf('.') != -1) {
                return false;
            }
        }
        var ilen = options['intLen'];
        var flen = options['flatLen'];
        return onlyNumber(keyCode) || keyCode == 190;
    };

    /**
     * 方法说明：
     *   设置一个文本框中只能输入字母
     * 参数说明：
     *   @param {HTMLObject} element input元素对象
     *   @param {Number} len 文本框中允许输入的字母个数，如果为空则为不限制
     */
    function allowEnterChar() {
        var keyCode = event.keyCode;
        if (isBackspace(keyCode) && (keyCode < 65 || keyCode > 90)) {
            return false;
        }

        var len = options['len'];
        var val = $(this).val();
        if (len != undefined && len != 0 && (val.length >= len) && isBackspace(keyCode)) {
            return false;
        }
        return true;
    };

    /**
	 * 方法说明：
	 *    设置某个文本框中只能输入数字
	 * 方法说明：
	 *    @param {HTMLObject} element input元素对象
	 *    @param {Number} len 允许输入的数字的个数，当没指定时为无限制 
	 */
    function allowEnterNumber() {
        var keyCode = event.keyCode;
        if (!onlyNumber(keyCode)) {
            return false;
        }
        var _val = $(this).val();
        var len = options['len'];
        if (len != undefined && len != 0 && (_val.length >= len) && isBackspace(keyCode)) {
            return false;
        }
        return true;
    };

    /**
	 * 方法说明：
	 *    设置一个文本框只能输入数字或字母
	 * 参数说明：
	 *    @param {HTMLObject} element  input文本框对象
	 *    @param {Number} len 允许输入的字母或字母的最大数量
	 */
    function allowEnterNumberOrChar() {
        return allowEnterChar() || allowEnterNumber();
    }
})(jQuery);

////限制文本控件输入的长度
////使用方法：
////        $("#Code").restrictFieldLength({
////            maxTextLength: 4,
////             restoreTime: 2000
////              });
////表示限制长度为4，超过长度将自动截取，限制输入
//;(function ($) {
//    $.fn.RestrictFieldLength = function (settings) {
//        var opts = $.extend({}, $.fn.restrictFieldLength.defaultSettings, settings || {});

//        return this.each(function () {
//            var elem = $(this);

//            elem.on("keyup", function () {
//                var s = elem.val();
//                if (s.length >= opts.maxTextLength) {
//                    elem.val(s.slice(0, opts.maxTextLength));
//                    elem.prop("class", opts.exceptionCss);
//                    if (opts.exceptionCallback) {
//                        opts.exceptionCallback(elem[0].id);
//                    }
//                    var normalIt = function () {
//                        elem.prop("class", opts.defaultCss);
//                    }
//                    setTimeout(normalIt, opts.restoreTime);
//                }
//            });
//        });
//    }
//    $.fn.restrictFieldLength.defaultSettings = {
//        maxTextLength: 100,
//        defaultCss: "restrictFieldLength_defaultCss",
//        exceptionCss: "restrictFieldLength_exceptionCss",
//        restoreTime: 1000,
//        exceptionCallback: null
//    }
//}
//)(jQuery);

////function ProcessException(id) {
////    $("#msg").html(id + "&nbsp;exception occurred.<br />" + $("#msg").html());
////}
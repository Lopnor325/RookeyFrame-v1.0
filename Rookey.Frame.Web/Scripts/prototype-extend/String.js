//实现对字符串 头和尾 空格的过滤
String.prototype.Trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
};

//实现对字符串 头（左侧Left） 空格的过滤
String.prototype.LTrim = function () {
    return this.replace(/(^\s*)/g, "");
};

//实现对字符串 尾（右侧Right） 空格的过滤
String.prototype.RTrim = function () {
    return this.replace(/(\s*$)/g, "");
};

//实现Contains方法（核心是用Index方法的返回值进行判断），
String.prototype.Contains = function (subStr) {
    var currentIndex = this.indexOf(subStr);
    if (currentIndex != -1) {
        return true;
    }
    else {
        return false;
    }
};

//字符串转化为数组
String.prototype.ToArray = function (delimiter) {
    var array = this.split(delimiter);
    return array;
};

//字符串替换
String.prototype.Replace = function (oldValue, newValue) {
    var reg = new RegExp(oldValue, "g");
    return this.replace(reg, newValue);
};


// 判断字符串是否以指定的字符串结束
String.prototype.EndsWith = function (str) {
    return this.substr(this.length - str.length) == str;
};

// 判断字符串是否以指定的字符串开始
String.prototype.StartsWith = function (str) {
    return this.substr(0, str.length) == str;
};

//在字符串末尾追加字符串
String.prototype.Append = function (aStr) {
    return this.concat(aStr);
};

//删除指定索引位置的字符，索引无效将不删除任何字符
String.prototype.DeleteCharAt = function (sIndex) {
    if (sIndex < 0 || sIndex >= this.length) {
        return this.valueOf();
    } else if (sIndex == 0) {
        return this.substring(1, this.length);
    } else if (sIndex == this.length - 1) {
        return this.substring(0, this.length - 1);
    } else {
        return this.substring(0, sIndex) + this.substring(sIndex + 1);
    }
};

//删除指定索引间的字符串.sIndex和eIndex所在的字符不被删除！
String.prototype.DeleteString = function (sIndex, eIndex) {
    if (sIndex == eIndex) {
        return this.deleteCharAt(sIndex);
    } else {
        if (sIndex > eIndex) {
            var tIndex = eIndex;
            eIndex = sIndex;
            sIndex = tIndex;
        }
        if (sIndex < 0) sIndex = 0;
        if (eIndex > this.length - 1) eIndex = this.length - 1;
        return this.substring(0, sIndex + 1) + this.substring(eIndex, this.length);
    }
};


//比较两个字符串是否相等。其实也可以直接使用==进行比较
String.prototype.Equals = function (aStr) {
    if (this.length != aStr.length) {
        return false;
    } else {
        for (var i = 0; i < this.length; i++) {
            if (this.charAt(i) != aStr.charAt(i)) {
                return false;
            }
        }
        return true;
    }
};

//比较两个字符串是否相等，不区分大小写!
String.prototype.EqualsIgnoreCase = function (aStr) {
    if (this.length != aStr.length) {
        return false;
    } else {
        var tmp1 = this.toLowerCase();
        var tmp2 = aStr.toLowerCase();
        return tmp1.equals(tmp2);
    }
};

//将指定的字符串插入到指定的位置后面!索引无效将直接追加到字符串的末尾
String.prototype.Insert = function (ofset, aStr) {
    if (ofset < 0 || ofset >= this.length - 1) {
        return this.append(aStr);
    }
    return this.substring(0, ofset + 1) + aStr + this.substring(ofset + 1);
};

//查看该字符串是否是数字串
String.prototype.IsAllNumber = function () {
    for (var i = 0; i < this.length; i++) {
        if (this.charAt(i) < '0' || this.charAt(i) > '9') {
            return false;
        }
    }
    return true;
};

//将该字符串反序排列
String.prototype.Reverse = function () {
    var aStr = "";
    for (var i = this.length - 1; i >= 0; i--) {
        aStr = aStr.concat(this.charAt(i));
    }
    return aStr;
};

//将指定的位置的字符设置为另外指定的字符或字符串.索引无效将直接返回不做任何处理！
String.prototype.SetCharAt = function (sIndex, aStr) {
    if (sIndex < 0 || sIndex > this.length - 1) {
        return this.valueOf();
    }
    return this.substring(0, sIndex) + aStr + this.substring(sIndex + 1);
};

//计算长度，每个汉字占两个长度，英文字符每个占一个长度
String.prototype.UCLength = function () {
    var len = 0;
    for (var i = 0; i < this.length; i++) {
        if (this.charCodeAt(i) > 255)
            len += 2;
        else
            len++;
    }
    return len;
};



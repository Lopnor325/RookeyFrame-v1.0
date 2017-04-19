
//数组转化为字符串
Array.prototype.ToString = function (delimiter) {
    var str = this.join(delimiter)
    return str;
}

var oldArrayIndexOf = Array.indexOf;//判断是否原始浏览器是否存在indexOf方法
Array.prototype.IndexOf = function (obj) {
    if (!oldArrayIndexOf) {
        for (var i = 0, imax = this.length; i < imax; i++) {
            if (this[i] === obj) {
                return i;
            }
        }
        return -1;
    } else {
        return oldArrayIndexOf(obj);
    }
}

//判断数据项是否在该数组中
Array.prototype.Contains = function (obj) {
    return this.indexOf(obj) !== -1;
}

//把数据项添加到指定的位置
Array.prototype.InsertAt = function (index, obj) {
    if (index < 0) index = 0;
    if (index > this.length) {
        index = this.length;
    }
    this.length++;
    for (var i = this.length - 1; i > index; i--) {
        this[i] = this[i - 1];
    }
    this[index] = obj;
}

//返回最有一项数据
Array.prototype.Last = function () {
    returnthis[this.length - 1];
}

//移除数组指定索引的值
Array.prototype.RemoveAt = function (index) {
    if (index < 0 || index >= this.length) return;
    var item = this[index];
    for (var i = index, imax = this.length - 2; i < imax; i++) {
        this[i] = this[i + 1];
    }
    this.length--;
    return item;
}

//移除数据项实体
Array.prototype.Remove = function (obj) {
    var index = this.indexOf(obj);
    if (index >= 0) {
        this.RemoveAt(index);
    }
}
//清除数据
Array.prototype.Clear = function () {
    this.length = 0;
}

//用于查询对象数组中对象的某些值，同时支持对已查询属性进行重命名，
//若查询的属性不在改数组中，则该属性返回为undefined
Array.prototype.Select = function (args) {
    var newItems = [];
    if (typeof (args) === "object" && arguments.length === 1) {//传入查询的参数为对象时的处理方式  
        for (vari = 0, imax = this.length; i < imax; i++) {
            var item = {};
            for (var key in args) {
                if (args[key] !== undefined) {
                    item[key] = this[i][key] === undefined ? "undefined" : this[i][key];
                }
            }
            newItems.push(item);
        }
    } else if (typeof (args) === "string" && arguments.length === 1) {//传入参数为字符串，且只有一个参数的处理方式  
        for (var i = 0, imax = this.length; i < imax; i++) {
            var item = {};
            var keys = args.split(',');
            for (var k = 0, kmax = keys.length; k < kmax; k++) {
                var iKey = keys[k].split("as");
                if (iKey.length === 1) {
                    item[iKey[0].trim()] = this[i][iKey[0].trim()] === undefined ? "undefined" : this[i][iKey[0].trim()];
                } else {
                    item[iKey[1].trim()] = this[i][iKey[0].trim()] === undefined ? "undefined" : this[i][iKey[0].trim()];
                }
            }
            newItems.push(item);
        }
    } else {//传入的参数是多个字符串的处理方式  
        for (var i = 0, imax = this.length; i < imax; i++) {
            var item = {};
            for (var j = 0, jmax = arguments.length; j < jmax; j++) {
                if (arguments[j] !== undefined) {
                    var iKey = arguments[j].split("as");
                    if (iKey.length === 1) {
                        item[iKey[0].trim()] = this[i][iKey[0].trim()] === undefined ? "undefined" : this[i][iKey[0].trim()];
                    } else {
                        item[iKey[1].trim()] = this[i][iKey[0].trim()] === undefined ? "undefined" : this[i][iKey[0].trim()];
                    }
                }
            }
            newItems.push(item);
        }
    }
    return newItems;
}

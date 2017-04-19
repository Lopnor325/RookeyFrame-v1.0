//查找标签 var btn=$("btn_ok")
function $(divID) {
    return document.getElementById(divID);
}
//创建图像标签
var createImg = function () {
    return document.createElement("img");
}
//创建按钮标签
var createBtn = function () {
    var btn = document.createElement("input");
    btn.type = "button"; //改变他的类型 
    return btn;
}
//创建一个DIV
var createDiv = function () {
    return document.createElement("div");
}
//创建文本标签
var createSpan = function () { 
 return document.createElement("span");
}
//创建字符过滤
String.prototype.trim = function () {
    return this.replace("","");//?????????????????
}
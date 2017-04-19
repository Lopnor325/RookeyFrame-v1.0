///身份证号码验证
function CheckIDCard(idcard) {
    var Errors = new Array(
                 "0",
                 "身份证号码位数不对!",
                 "身份证号码出生日期超出范围或含有非法字符!",
                 "身份证号码校验错误!",
                  "身份证地区非法!");
    var area = { 11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江", 31: "上海", 32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南", 42: "湖北", 43: "湖南", 44: "广东", 45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州", 53: "云南", 54: "西藏", 61: "陕西", 62: "甘肃", 63: "青海", 64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外" }
    var idcard, Y, JYM;
    var S, M;
    var idcard_array = new Array();
    idcard_array = idcard.split("");
    //地区检验 
    if (area[parseInt(idcard.substr(0, 2))] == null) return Errors[4];
    //身份号码位数及格式检验 
    switch (idcard.length) {
        case 15:
            if ((parseInt(idcard.substr(6, 2)) + 1900) % 4 == 0 || ((parseInt(idcard.substr(6, 2)) + 1900) % 100 == 0 && (parseInt(idcard.substr(6, 2)) + 1900) % 4 == 0)) {
                ereg = /^[1-9][0-9]{5}[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}$/; //测试出生日期的合法性 
            } else {
                ereg = /^[1-9][0-9]{5}[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}$/; //测试出生日期的合法性 
            }
            if (ereg.test(idcard)) return Errors[0];
            else return Errors[2];
            break;
        case 18:
            //18位身份号码检测 
            //出生日期的合法性检查  
            //闰年月日:((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9])) 
            //平年月日:((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8])) 
            if (parseInt(idcard.substr(6, 4)) % 4 == 0 || (parseInt(idcard.substr(6, 4)) % 100 == 0 && parseInt(idcard.substr(6, 4)) % 4 == 0)) {
                ereg = /^[1-9][0-9]{5}19[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}[0-9Xx]$/; //闰年出生日期的合法性正则表达式 
            } else {
                ereg = /^[1-9][0-9]{5}19[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}[0-9Xx]$/; //平年出生日期的合法性正则表达式 
            }
            if (ereg.test(idcard)) {//测试出生日期的合法性 
                //计算校验位 
                S = (parseInt(idcard_array[0]) + parseInt(idcard_array[10])) * 7
                            + (parseInt(idcard_array[1]) + parseInt(idcard_array[11])) * 9
                            + (parseInt(idcard_array[2]) + parseInt(idcard_array[12])) * 10
                            + (parseInt(idcard_array[3]) + parseInt(idcard_array[13])) * 5
                            + (parseInt(idcard_array[4]) + parseInt(idcard_array[14])) * 8
                            + (parseInt(idcard_array[5]) + parseInt(idcard_array[15])) * 4
                            + (parseInt(idcard_array[6]) + parseInt(idcard_array[16])) * 2
                            + parseInt(idcard_array[7]) * 1
                            + parseInt(idcard_array[8]) * 6
                            + parseInt(idcard_array[9]) * 3;
                Y = S % 11;
                M = "F";
                JYM = "10X98765432";
                M = JYM.substr(Y, 1); //判断校验位 
                if (M == idcard_array[17]) return Errors[0]; //检测ID的校验位 
                else return Errors[3];
            }
            else return Errors[2];
            break;
        default:
            return Errors[1];
            break;
    }
}

//验证台湾身份证
function CheckTaiWanIdCard(idnum) {
    num = idnum;
    num = num.toLowerCase();
    patten = /^[a-z][12][0-9]{8}$/;
    if (patten.test(num)) {
        h = "abcdefghjklmnpqrstuvxywzio ";
        x = 10 + h.indexOf(num.substring(0, 1));
        chksum = (x - (x % 10)) / 10 + (x % 10) * 9;
        for (i = 1; i < 9; i++) {
            chksum += num.substring(i, i + 1) * (9 - i);
        }
        chksum = (10 - chksum % 10) % 10;
        if (chksum == num.substring(9, 10)) {
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}

//香港身份證信息，沒有性別信息，沒有地域信息。不過，還是有一個完整的驗證規則。
//　　身份證字號的樣式為XYabcdef(z)，其中X可以是英文數字或是空白不填，而Y一定是英文字母，z是檢核碼為0~10的數字(當z為10的時候，則以A表示)，要驗算是否為正確之身份證號碼時，第一個英文字若為空白則以58代替，若是英文字則依A=10，B=11，C=12，D=13…的順序繼續下去，第二個英文字作法亦同，然後依照以下規則算出數字後，再加上檢核碼一定會是11的倍數~
//　　例如：A123456(3)→58 10 123456
//               ＝58×9+10×8+1×7+2×6+3×5+4×4+5×3+6×2
//   　　　　＝522+80+7+12+15+16+15+12
//               ＝679
//　　679+3＝682＝11×62 是11的倍數，因此它是一個正確的身份證字號。
// 
//　　又例如：AB654321(□)　□是多少呢？
//　　　　　AB654321(□)→10 11 654321
//               ＝10×9+11×8+6×7+5×6+4×5+3×4+2×3+1×2
//   　　　　＝90+88+42+30+20+12+6+2
//               ＝290
//　　∵290÷11=26…3  
//　　∴□=3
//验证香港身份证函数
function CheckHongKongIdCard(cardNum) {
    if (!cardNum.match(/^[a-zA-Z][0-9]{6}[0-9aA]$/)) return false;
    var mul = 8, sum = (cardNum.toUpperCase().charCodeAt() - 64) * mul--;
    while (mul > 1) sum += parseInt(cardNum.substr(8 - mul, 1)) * mul--;
    var chksum = (11 - (sum % 11)).toString(16);
    if (chksum == "b") chksum = "0";
    return chksum == cardNum.substr(7, 1).toLowerCase();
}


//根据身份证号获取出生日期
function GetBirthdatByIdNo(iIdNo) {
    var tmpStr = "";
    var idDate = "";
    var tmpInt = 0;
    var strReturn = "";
    var no = iIdNo.replace(/^\s+|\s+$/g, "");
    if ((no.length != 15) && (no.length != 18)) {
        return strReturn;
    }
    if (no.length == 15) {
        tmpStr = no.substring(6, 12);
        tmpStr = "19" + tmpStr;
        tmpStr = tmpStr.substring(0, 4) + "-" + tmpStr.substring(4, 6) + "-" + tmpStr.substring(6)
        return tmpStr;
    }
    else {
        tmpStr = iIdNo.substring(6, 14);
        tmpStr = tmpStr.substring(0, 4) + "-" + tmpStr.substring(4, 6) + "-" + tmpStr.substring(6)
        return tmpStr;
    }
}
//根据身份证获取性别
function GetSexByIdNo(iIdNo) {
    var tmpStr = "";
    var idDate = "";
    var tmpInt = 0;
    var strReturn = "";
    var no = iIdNo.replace(/^\s+|\s+$/g, "");;
    if ((no.length != 15) && (no.length != 18)) {
        return strReturn;
    }
    if (no.length == 15) {
        if (parseInt(no.charAt(14) / 2) * 2 != no.charAt(14)) {
            return "男";
        }
        else {
            return "女";
        }
    }
    else {
        if (parseInt(no.charAt(16) / 2) * 2 != no.charAt(16)) {
            return "男";
        }
        else {
            return "女";
        }
    }
}

//根据出生日期获取年龄
function GetAgeByBirthday(birthdayValue) {
    var dt1 = new Date(birthdayValue.replace("-", "/"));
    var dt2 = new Date();
    var age = dt2.getFullYear() - dt1.getFullYear();
    var m = dt2.getMonth() - dt1.getMonth();
    if (m < 0)
        age--;
    return age;
}
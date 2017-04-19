
//默认URL
var defaultURL = "/FileManage/WebExplorer.ashx";

// ajax helper method
function getHttpRequest() {
    var httpRequest = null;

    if (window.XMLHttpRequest) {
        httpRequest = new XMLHttpRequest();

        if (httpRequest.overrideMimeType) {
            httpRequest.overrideMimeType("text/xml");
        }
    }
    else if (window.ActiveXObject) {
        try {
            httpRequest = new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            try {
                httpRequest = new ActiveXObject("Microsoft.SMLHTTP");
            }
            catch (e)
            { }
        }
    }

    if (!httpRequest) {
        alert("Can't create XMLHttpRequest object!");
    }

    return httpRequest;
}

function executeHttpRequest(method, url, data) {
    var xmlHttp = getHttpRequest();
    xmlHttp.open(method, url, false);
    xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xmlHttp.send(data);

    return xmlHttp.responseText;
}
function objCB(id) { this.id = id; this.element = this.DomElement = document.getElementById(id); return this; };
objCB.prototype.Callback = function (param) {
    var scriptFiles = [], cssElement, request, elem = this, callback = document.getElementById(this.id), scriptElement;
    scriptElement = document.createElement('script');
    scriptElement.setAttribute("type", "text/javascript");
    if (this.CallbackInProgress) {
        return false;
    } else {
        this.CallbackInProgress = true;
    };

    handleResponse = function () {
        if (request.readyState && request.readyState != 4 && request.readyState != 'complete') {
            return;
        };
        elem.CallbackInProgress = false;
        var responseText = request.responseText;
        if (elem.Debug) {
            if (responseText) {
                alert('Data received: ' + request.responseText);
            };
        };
        request = request.responseXML;
        if (request && request.documentElement) {
            if (request.documentElement.nodeName == "CallbackError") {
                if (elem.Debug) {
                    alert(request.documentElement.firstChild.nodeValue);
                } else {
                    alert("An error has occured");
                    window.location.href = window.location.href;
                };
                return;
            };
            var data = request.documentElement.firstChild.nodeValue;
            if (elem.Target != '') {
                var t = window.document.getElementById(elem.Target);
                if (t !== null) {
                    //t.innerHTML = '';
                    callback = t;
                }
            }
            var pdata = recieveServerData(data);
            if (pdata != '') {
                callback.innerHTML = '';
                callback.innerHTML = pdata;
            };
            
            if (scriptFiles.length > 0) {
                for (sci = 0; sci < scriptFiles.length; sci++) {
                    var scriptFileElement = document.createElement("script");
                    scriptFileElement.setAttribute("type", "text/javascript");
                    scriptFileElement.setAttribute("src", scriptFiles[sci]);
                    document.getElementsByTagName("head").item(0).appendChild(scriptFileElement);
                };
            };
            cssElement = document.createElement('style');
            cssElement.setAttribute("type", "text/css");
            var css = groupCSS(data);
            if (css != "") {
                if (cssElement.styleSheet) {
                    cssElement.styleSheet.cssText = css;
                } else {
                    var cssText = doc.createTextNode(css);
                    cssElement.appendChild(cssText);
                };
                document.body.appendChild(cssElement);
            };
            if (scriptElement.innerHTML != "") {
                document.body.appendChild(scriptElement);
            };
            elem.Target = '';
            if (elem['CBC_' + elem.id]) {
                elem['CBC_' + elem.id]();
            };
        } else {
            var formTag = responseText.substring(responseText.indexOf('<form'), responseText.indexOf('>', responseText.indexOf('<form'))), errorMsg = formTag.substring(formTag.indexOf('error=') + 6, formTag.indexOf('&', formTag.indexOf('error=')));
            if (elem.debug) {
                alert(errorMsg);
            };
        };
    };
    recieveServerData = function (value) {
        //elem.innerHTML = '';
        var scripts = groupScripts(value);
        scriptElement.text = scripts;
        if (elem.Debug) {
            if (scripts != "") {
                alert('Registered scripts: ' + getOuterHTML(scriptElement));
            };
        };
        return value;
    };
    groupCSS = function (string) {
        var css = "", data = string;
        while (true) {
            var start = data.indexOf('<style');
            if (start < 0) { break; };
            var startCssIndex = data.indexOf(">", start) + 1, endCssIndex = data.indexOf("</sty" + "le>", start);
            if (endCssIndex >= startCssIndex) {
                var end = endCssIndex + 8;
                if (endCssIndex > startCssIndex) {
                    var cssBlock = data.substring(startCssIndex, endCssIndex);
                    css += cssBlock;
                };
                data = data.substring(0, start) + data.substring(end);
            };
        };
        return css;
    };
    groupScripts = function (string) {
        var scripts = "", data = string;
        while (true) {
            var start = data.indexOf('<script');
            if (start < 0) { break; };
            var startScriptIndex = data.indexOf(">", start) + 1, endScriptIndex = data.indexOf("</scr" + "ipt>", start);
            if (endScriptIndex >= startScriptIndex) {
                if (endScriptIndex == startScriptIndex) {
                    var iStartFileNameIndex = data.indexOf('src="', start);
                    if (iStartFileNameIndex > 0) {
                        iStartFileNameIndex += 5;
                        var iEndFileNameIndex = data.indexOf('"', iStartFileNameIndex), sFileName = data.substring(iStartFileNameIndex, iEndFileNameIndex);
                        if (sFileName.length > 0) {
                            var headElement = document.getElementsByTagName("head").item(0);
                            if (headElement) {
                                var arHeadScripts = headElement.getElementsByTagName('script'), bPreLoaded = false;
                                for (var qzAde = 0; qzAde < arHeadScripts.length; qzAde++) {
                                    if (arHeadScripts[qzAde].src && arHeadScripts[qzAde].src == sFileName) {
                                        bPreLoaded = true;
                                        break;
                                    };
                                };
                                if (!bPreLoaded) {
                                    scriptFiles[scriptFiles.length] = sFileName;
                                };
                            };
                        };
                    };
                };
                var end = endScriptIndex + 9;
                if (endScriptIndex > startScriptIndex) {
                    var sScriptBlock = data.substring(startScriptIndex, endScriptIndex);
                    scripts += sScriptBlock;
                };
                data = data.substring(0, start) + data.substring(end);
            };
        };
        return scripts;
    };
    httpRequest = function (reqType, url, asynch, data) {
        initReq = function (reqType, url, bool, data) {
            if (elem.Debug) {
                alert('Data sent: ' + data);
            };
            request.onreadystatechange = handleResponse;
            request.open(reqType, url, bool);
            request.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
            request.send(data);
        };
        if (window.XMLHttpRequest) {
            request = new XMLHttpRequest();
        } else if (window.ActiveXObject("Microsoft.XMLHTTP")) {
            if (!request) {
                request = new ActiveXObject("Microsoft.XMLHTTP");
            };
        };
        if (request) {
            if (elem['LT_' + elem.id]) {
                elem.element.innerHTML = elem['LT_' + elem.id];
            };
            initReq(reqType, url, asynch, data);
        };
    };
    var data = "";
    if (arguments.length == 0) {
        data = "amCB_" + this.id + "=0";
    };
    encode = function (str) {
        if (str == undefined) {
            return undefined;
        } else {
            return str.toString().replace(/&/g, "!AM#").replace(/=/g, "#AM!").replace(/\+/g, "#MA!");
        };
    };
    for (cbi = 0; cbi < arguments.length; cbi++) {
        if (data == "") {
            data += "amCB_" + this.id + "=" + encode(arguments[cbi]);
        } else {
            data += "&amCB_" + this.id + "=" + encode(arguments[cbi]);
        };

    };
    var hdval = window.document.getElementById('amcbid');
    if (hdval !== null) {
        hdval = hdval.value;
    };
    data += "&hidreq=" + encode(hdval);
    if (typeof (elem.Location) == 'undefined') {
        if (window.location.toString().indexOf('#') >= 0) {
            httpRequest("POST", window.location.href.substring(0, window.location.href.toString().indexOf('#')), true, data);
        } else {
            httpRequest("POST", window.location, true, data);
        };

    } else {
        httpRequest("POST", elem.Location, true, data);
    }

};
function getOuterHTML(object) {
    if (document.all) {
        return object.outerHTML;
    } else {
        var element;
        if (!object) { return null; };
        element = document.createElement("div");
        element.appendChild(object.cloneNode(true));
        return element.innerHTML;
    };
};
                                                                   
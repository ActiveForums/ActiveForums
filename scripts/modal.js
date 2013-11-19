var asCurrentModal = null;
var asModalTemp = null;
function getModalBody() {
    if (asCurrentModal != null) {
        var modbod = window.document.getElementById(asCurrentModal + 'ammodalbody');
        if (modbod != null) {
            return modbod;
        };
    };
    return null;
};
function asModal(id) {
    this.id = id;
    this.element = this.DomElement = document.getElementById(id);
    this.title = '';
    this.height = 250;
    this.width = 350;
    this.params = '';
    this.view = '';
    this.showMask = true;
    this.closeImage = '';
    this.position = 'middle';
    this.listeners = {};
    this.scriptSrc = '';
    this.element.style.display = 'none';
    return this;
};
var timetoclose;
asModal.prototype.closeTimer = function() {
    timetoclose = setTimeout('asModalTemp.Close()', 2000);
};
function getDimensions(){
    var w = 0, h = 0;
    if (typeof (window.innerWidth) == 'number') {
        w = window.innerWidth;
        h = window.innerHeight;
    } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
        w = document.documentElement.clientWidth;
        h = document.documentElement.clientHeight;
    } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
       w = document.body.clientWidth;
        h = document.body.clientHeight;
    };

    return { h: h, w: w };
};
asModal.prototype.Show = function(params) {
    asCurrentModal = this.id;
    asModalTemp = this;
    var dimensions = getDimensions();
    var viewport = asui_GetViewPort();
    var winWidth = dimensions.w;
    var divTop = 0;
    if (this.position == 'middle') {
        divTop = (viewport.h / 2) - (this.height / 2);
        if (document.all) {
            divTop += top.document.documentElement.scrollTop;
        } else {
            divTop += top.pageYOffset;
        };
    } else {
        divTop = 200;
    };
    var divLeft = (winWidth / 2) - (this.width / 2);
    var shell = this.LoadShell();
    var modalBody = document.getElementById(this.id + 'ammodalbody');
    var div = document.getElementById(this.id);
    shell.style.position = 'absolute';
    shell.style.top = divTop + 'px';
    shell.style.left = divLeft + 'px';
    div.style.width = this.width + 'px';
    div.style.height = this.height + 'px';
    div.style.zIndex = 500;
    div.style.display = 'block';
    div.style.backgroundColor = '#fff';
    div.style.visibility = 'visible';
    modalBody.appendChild(div);
    var tmp = document.getElementById(asCurrentModal + 'ammodalshell');
    tmp.style.visibility = 'hidden';
    tmp.style.display = 'block';
    var mytimeout;
    asui_appear(asCurrentModal + 'ammodalshell', 10, 10);
    var cb = eval(this.id + '_CB');
    if (cb != null) {
        cb['CBC_' + cb.id] = asModalTemp.LoadComplete;
        cb.Callback(this.view, this.params);
    } else {
        this.LoadComplete();
    };
};
asModal.prototype.LoadComplete = function() {
    if (asModalTemp != null) {
        var evt = {
            Modal: asModalTemp,
            args: arguments
        };
        if (asModalTemp.RaiseEvent("OnLoadComplete", evt) == false) {
            return;
        };
        if (asModalTemp != null) {
            asModalTemp.DetachEvent("OnLoadComplete");
        };

    };

};
asModal.prototype.UpdateTitle = function(title) {
    var titlebar = window.document.getElementById(this.id + 'ammodaltitle');
    if (titlebar !== null) {
        titlebar.innerHTML = title;
    };
};
asModal.prototype.Close = function(e) {
    var evt = {
        Modal: this,
        DomEvent: e
    };
    if (this.RaiseEvent("OnClick", evt) == false) {
        return;
    };
    var mm = document.getElementById('ammodalmask');
    if (mm != null || mm != undefined) {
        var tmp = mm.parentNode;
        tmp.removeChild(mm);
    };
    var mTable = document.getElementById(this.id + 'ammodalshell');
    asui_fade(mTable.id, 10, 10);

    var bb = document.getElementById('ammodalbuttonbar');
    var div = document.getElementById(this.id);
    var modalBody = document.getElementById(this.id + 'ammodalbody');
    if (bb != null || bb != undefined) {
        modalBody.removeChild(bb);
    };
    if (div !== null || div != undefined) {
        var cb = eval(this.id + '_CB');
        if (cb !== null) {
            var d = document.getElementById(this.id + '_CB');
            if (d !== null) { d.innerHTML = ''; };

        };

    };

    clearTimeout(timetoclose);
    asCurrentModal = null;
    asModalTemp = null;

};
asModal.prototype.LoadShell = function() {
    var mTable;
    mTable = document.getElementById(this.id + 'ammodalshell');
    var id = this.id;
    if (mTable == undefined) {
        mTable = document.createElement('table');
        mTable.setAttribute('style', 'display:none;');
        mTable.setAttribute('cellpadding', '0');
        mTable.setAttribute('cellspacing', '0');
        var mDiv = document.createElement('div');
        mDiv.setAttribute('id', this.id + 'ammodalbody');
        mDiv.setAttribute('class', 'ammodalbody');
        mTable.setAttribute('id', this.id + 'ammodalshell');
        mTable.className = 'ammodalshell';
        var oTR = mTable.insertRow(0);
        var oTD = oTR.insertCell(0);
        oTD.className = 'ammodalContent';
        var tDiv = document.createElement('div');
        tDiv.setAttribute('id', this.id + 'ammodaltitle');
        tDiv.innerHTML = this.title;
        tDiv.className = 'ammodaltitle';
        if (this.closeImage != '') {
            var cImg = new Image();
            cImg.src = this.closeImage;
            cImg.setAttribute('style', 'float:right;margin-top:3px;margin-right:3px;cursor:pointer;');
            cImg.style.cursor = 'pointer';
            cImg.onclick = function() { window[id].Close(); };
            cImg.setAttribute('align', 'right');
            oTD.appendChild(cImg);
        };

        oTD.appendChild(tDiv);
        oTD.appendChild(mDiv);
        document.body.appendChild(mTable);

    } else {
        var tDiv = document.getElementById(this.id + 'ammodaltitle');
        tDiv.innerHTML = this.title;
        var mDiv = document.getElementById(this.id + 'ammodalbody');
        mTable.style.display = 'none';

    };
    return mTable;

};

asModal.prototype.RaiseEvent = function(eventName, eventArgs) {

    if (this.listeners == null) {
        return false;
    };
    if (this.listeners.length <= 0) {
        return false;
    };
    var outcome = true;

    if (this[eventName] != undefined) {
        var eventResult = resolveFunction(this[eventName][0])(this, eventArgs);
        if (typeof (eventResult) == "undefined") {
            eventResult = true;
        };
        outcome = outcome && eventResult;
    };
    if (!this.listeners[eventName]) { return outcome; }
    for (var i = 0; i < this.listeners[eventName].length; i++) {
        var handler = this.listeners[eventName][i];
        if (handler !== null) {
            var eventResult = handler(this, eventArgs);
            if (typeof (eventResult) == "undefined") {
                eventResult = true;
            };
            outcome = outcome && eventResult;
        };

    };

    return outcome;


};
asModal.prototype.AttachEvent = function(eventName, handler) {
    if (!this.listeners[eventName]) {
        this.listeners[eventName] = [];
    };
    this.listeners[eventName][this.listeners[eventName].length] = (resolveFunction(handler));

};
asModal.prototype.DetachEvent = function(eventName, handler) {
    var listeners = this.listeners[eventName];
    if (!listeners) {
        return false;
    };

    if (handler != undefined) {
        var funcHandler = resolveFunction(handler);
        for (var i = 0; i < listeners.length; i++) {
            if (funcHandler == listeners[i]) {
                listeners.splice(i, 1);
                return true;
            };
        };
    } else {

        for (var i = 0; i < listeners.length; i++) {
            listeners[i] = null;
        };
    };

    return false;
};
function resolveFunction(func) {
    if (typeof (func) == "function") {
        return func;

    } else if (typeof (window[func]) == "function") {
        return window[func];
    } else {
        return new Function("var Sender = arguments[0]; var Arguments = arguments[1];" + func);
    };
};
function closeOnTimer() {
    timetoclose = setTimeout('window.asModalTemp.Close()', 2000);
};
function asgetTipPosition(e) {
    var left = 0;
    var top = 0;
    while (e.offsetParent) {
        left += e.offsetLeft;
        top += e.offsetTop;
        e = e.offsetParent;

    };
    left += e.offsetLeft;
    top += e.offsetTop;
    return { x: left, y: top };
};

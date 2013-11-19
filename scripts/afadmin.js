afHistoryChange = function (newLocation, historyData) {
    if (newLocation.indexOf("cpview") >= 0) {
        var tmp = newLocation.split("&");

        var view = tmp[0].split("=")[1];
        var param = tmp[1];
        if (param.indexOf('!') >= 0) {
            param = param.replace('params=', '');
        } else {
            param = param.split("=")[1];
        };
        var currview = getQueryString()["cpview"];
        var currparms = getQueryString()["params"];
        if (currview != view || currparms != param) {
            LoadView(view, param);
        }
        
    };
};


function trim(str, chars) {
    return ltrim(rtrim(str, chars), chars);
};
function ltrim(str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
};
function rtrim(str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
};
function onlyNumbers(evt){
    var charCode = (evt.which != undefined) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode != 46) && (charCode != 44)) {
        return false;
    } else {
        return true;
    };
};
function af_showLoad() {
    amcp.UI.Elastic('amcploader',48,10,20,undefined);
   
};
function af_clearLoad(delay) {
    if (typeof (delay) == 'undefined') {
        delay = 500;
    };
    if (delay > 0) {
        setTimeout(function () { amcp.UI.Elastic('amcploader', 0, -10, 15); }, delay);
    } else {
        amcp.UI.Elastic('amcploader', 0, -10, 10);
    };
};
function closeAllProp(){
    displaySelectBoxes();
    var propdivs = document.getElementsByTagName("div"); 
    for (var i = 0; i < propdivs.length; i++) { 
        if (propdivs[i].className == 'ammodalpop'){
            propdivs[i].style.display='none';
        };
    };
};
function hideSelectBoxes() {
    var amshell = document.getElementById("amcpcontainer");
	var input = amshell.getElementsByTagName("SELECT");
	for(var hsby = 0; hsby < input.length; hsby++) {
        var o = input.item(hsby);
	    o.style.visibility='hidden';
	 };
};
function displaySelectBoxes() {
    
    var amshell = document.getElementById("amcpcontainer");
	var input = amshell.getElementsByTagName("SELECT");
    for(var hsby = 0; hsby < input.length; hsby++) {
        var o = input.item(hsby);
	    o.style.visibility='visible';
	 };
};
function amShowTip(obj,message){
    //position = getTipPosition(obj);
    //console.log(position);
    var elem = $(obj);
    position = elem.position();
    var tt =document.getElementById("amtip");
    var tttxt = document.getElementById("amtiptext");
    tt.style.left= position.left + 'px';
    tt.style.top= (position.top +18 ) + 'px';
    tt.style.visibility="visible";
    tt.style.display = 'block';
    tttxt.innerHTML = message;
};
function amHideTip(){
    var tt =document.getElementById("amtip");
    tt.style.visibility = "hidden";
    tt.style.display = 'none';
};
function getTipPosition(e){
    var left = 0;
    var top = 0;
    while (e.offsetParent){
        left += e.offsetLeft;
        top += e.offsetTop;
        e = e.offsetParent;
            
    };
    left += e.offsetLeft;
    top += e.offsetTop;
    return {x:left, y:top};
};
var timetoclose;

function toggleTab(tab) {
   
    closeAllProp();

    if (typeof (tab) == 'undefined') {
        alert('tab is undefined');
    }

    if (selectedTab != tab.id) {
        
        selectedTab = tab.id;
        var obj = document.getElementsByTagName("div");
            for (var i=0;i < obj.length;i++){
                var el = obj[i];
                if (el.id.indexOf('afcontent') > 1){
                    el.style.display = 'none';
                };
                if (el.id.indexOf('_text') > 1){
                    el.className = 'amtabtext';
                };
                if (el.className == 'amtabsel'){
                    el.className = 'amtab';
                };
            };
        var tabContent = document.getElementById(tab.id + '_afcontent');
        var tabtext = document.getElementById(tab.id + '_text');
        var tab = document.getElementById(tab.id);
        tab.className = 'amtabsel';
        tabtext.className = 'amtabseltext';
        tabContent.style.display = 'block';
        var tmptb = document.getElementById('amtoolbar');

        if (tab.id == 'divClean' && tmptb != null) {
            tmptb.style.visibility = 'hidden';
        } else if (tmptb != null) {
            tmptb.style.visibility = 'visible';
        };
        var view = getQueryString()["cpview"];
        
       
        var parms = getQueryString()["params"];
        History.pushState(null, null, '?cpview=' + view + '&params=' + parms + '&tab=' + tab.id);                  
    };
};
function amOpenModal(modalDiv, title, height, width, saveFunction) {
    amLoadMask();
    var shell = amLoadShell(title);
    var modalBody = document.getElementById('ammodalbody');
    var div = document.getElementById(modalDiv);
    var winHeight = document.body.clientHeight;
    var winWidth = document.body.clientWidth;
    var divTop = (winHeight / 2) - (height / 2);
    var divLeft = (winWidth / 2) - (width / 2);
    shell.style.top = divTop;
    shell.style.left = divLeft;
    shell.style.position = 'absolute';
    shell.style.zIndex = '2000';
    shell.style.display = '';
    div.style.display = '';
    div.style.width = width + 'px';
    div.style.height = height + 'px';
    modalBody.appendChild(div);
    var btnDiv = document.createElement('div');
    btnDiv.setAttribute('id', 'ammodalbuttonbar');
    btnDiv.className = 'ammodalbuttonbar';
    var btn = document.createElement('input');
    btn.setAttribute('type', 'button');
    btn.setAttribute('value', 'Save');
    btn.className = 'ambutton';
    if (saveFunction != undefined) {
        btn.setAttribute('onclick', saveFunction);
        btnDiv.appendChild(btn);
    };



    btn = document.createElement('input');
    btn.setAttribute('type', 'button');
    btn.setAttribute('value', 'Cancel');
    btn.className = 'ambutton';
    btn.setAttribute('onclick', 'amCloseModal(' + modalDiv + ');');
    btnDiv.appendChild(btn);
    modalBody.appendChild(btnDiv);
};
function amCloseModal(modalDiv) {
    var mm = document.getElementById('ammodalmask');
    document.body.removeChild(mm);
    var mTable = document.getElementById('ammodalshell');
    mTable.style.display = 'none';
    var bb = document.getElementById('ammodalbuttonbar');
    var modalBody = document.getElementById('ammodalbody');
    modalBody.removeChild(bb);


};
function amLoadMask() {
    var mDiv = document.createElement('div');
    mDiv.setAttribute('id', 'ammodalmask');
    mDiv.style.position = 'absolute';
    mDiv.style.top = '0px';
    mDiv.style.left = '0px';
    mDiv.style.width = getScrollWidth();
    mDiv.style.height = getScrollHeight();
    mDiv.style.display = '';
    mDiv.style.zIndex = '0';
    document.body.appendChild(mDiv);
};
function amLoadShell(title) {
    var mTable;
    mTable = document.getElementById('ammodalshell');
    if (mTable == undefined) {
        mTable = document.createElement('table');
        mTable.setAttribute('cellpadding', '0');
        mTable.setAttribute('cellspacing', '0');
        var mDiv = document.createElement('div');
        mDiv.setAttribute('id', 'ammodalbody');
        mDiv.setAttribute('class', 'ammodalbody');
        mTable.setAttribute('id', 'ammodalshell');
        mTable.className = 'ammodalshell';
        var oTR = mTable.insertRow(0);
        var oTD = oTR.insertCell(0);
        oTD.className = 'ammodalUL';
        oTD = oTR.insertCell(1);
        oTD.className = 'ammodalUB';
        oTD = oTR.insertCell(2);
        oTD.className = 'ammodalUR';
        oTR = mTable.insertRow(1);
        oTD = oTR.insertCell(0);
        oTD.className = 'ammodalLB';
        oTD = oTR.insertCell(1);
        var tDiv = document.createElement('div');
        tDiv.setAttribute('id', 'ammodaltitle');
        tDiv.innerHTML = title;
        tDiv.className = 'ammodaltitle';
        oTD.appendChild(tDiv);
        oTD.appendChild(mDiv);
        oTD = oTR.insertCell(2);
        oTD.className = 'ammodalRB';
        oTR = mTable.insertRow(2);
        oTD = oTR.insertCell(0);
        oTD.className = 'ammodalBL';
        oTD = oTR.insertCell(1);
        oTD.className = 'ammodalBB';
        oTD = oTR.insertCell(2);
        oTD.className = 'ammodalBR';
        document.body.appendChild(mTable);

    } else {
        var tDiv = document.getElementById('ammodaltitle');
        tDiv.innerHTML = title;
        var mDiv = document.getElementById('ammodalbody');
        mDiv.r;
    };
    return mTable;

};
function getScrollWidth() {
    var w = document.body.scrollWidth ||
        document.body.scrollLeft ||
        document.documentElement.scrollLeft;

    return w ? w : 0;
};

function getScrollHeight() {
    var h = document.body.scrollHeight ||
        document.body.scrollTop ||
        document.documentElement.scrollTop;

    return h ? h : 0;
};

function removeProps(node) {
    var popShell = document.getElementById("amProp");
    if (popShell.hasChildNodes()) {
        while (popShell.childNodes.length >= 1) {
            popShell.removeChild(popShell.firstChild);
        };
    };
    
};
var specchar = false;
function filterVanity(obj, evt, shadow) {
    var dest = null;
    if (typeof (shadow) != 'undefined') {
        dest = document.getElementById(shadow);
    };
    var charCode = (evt.which != undefined) ? evt.which : evt.keyCode;
    if (obj.value.length > 50 && charCode != 8) {
        return false;
    };
    if (dest !== null) {
        dest.value += String.fromCharCode(charCode);
    };
    var k = '';
    if (
		(charCode == 32) || 
        (charCode == 45) ||
		(charCode > 46 && charCode <58) ||
        (charCode > 64 && charCode < 91) ||
		(charCode > 96 && charCode < 123)  ||
		(charCode == 8) ||
		(charCode == 0)
		) {
        if (charCode == 32) {
            k = '-';
            if (specchar) {
                return false;
            };
            specchar = true;
            obj.value = obj.value.toString().trim() + k;
            return false;

        } else if (charCode > 8 && charCode != 32) {
            k = String.fromCharCode(charCode);
            specchar = false;
        };

    } else {
        return false;
    };
};
var amcp = {};
amcp.Utils = {
    GetByClassName: function (cname, p) {
        var es;
        if (typeof (p) == 'undefined') {
            es = document.childNodes;
        } else {
            es = p.childNodes;
        };
        var iCount = es.length;
        for (var i = 0; i < iCount; i++) {
            var el = es[i];
            if (typeof (el.className) != 'undefined') {
                if (el.className.indexOf(cname) >= 0) {
                    return el;
                };
            };
        };
    },
    SetSelected: function (drp, value) {
        var drp = document.getElementById(drp);
        for (var x = 0; x < drp.options.length; x++) {
            if (drp.options[x].value == value) {
                drp.options[x].setAttribute('selected', 'selected');
                drp.selectedIndex = x;
            } else {
                drp.options[x].removeAttribute('selected');
            };
        };
    },
    GetElementsByClassName: function(clsName){
        var retVal = new Array();
        var elements = document.getElementsByTagName("*");
        for (var i = 0; i < elements.length; i++) {
            if (elements[i].className.indexOf(" ") >= 0) {
                var classes = elements[i].className.split(" ");
                for (var j = 0; j < classes.length; j++) {
                    if (classes[j] == clsName) {
                        retVal.push(elements[i]);
                    };
                };
            } else if (elements[i].className == clsName) {
                retVal.push(elements[i]);
            };
        };
        return retVal;
    },
    addOption: function (select, text, value, obj) {
        var dd;
        if (select == '') {
            dd = obj;
        } else {
            dd = afV(select);
        };
        if (document.all) {
            dd.options.add(new Option(text, value));
        } else {
            var opt = document.createElement('option');
            opt.text = text;
            opt.value = value;
            dd.appendChild(opt);
        };
    },
    GetParentByTagName: function (obj, tag) {
        var cmt = obj;
        var i = 0;
        do {
            cmt = cmt.parentNode;
            i++
        } while (cmt.tagName != tag || i == 100);
        return cmt;
    }
};
amcp.UI = {
    currentModalDiv: null,
    LoadDiv: function (id, header) {
        if (amcp.UI.currentModalDiv !== null) {
            amcp.UI.ClearMask();
            document.getElementById(amcp.UI.currentModalDiv).style.display = 'none';
            amcp.UI.currentModalDiv = null;
        };
        document.getElementById(id + '_header').innerHTML = header;
        amcp.UI.LoadMask();
        amcp.UI.currentModalDiv = id;
        amcp.UI.DivToModal(id);
    },
    CloseDiv: function (id) {
        amcp.UI.Fade(id, 10, 10);
        amcp.UI.ClearMask();
        amcp.UI.currentModalDiv = null;
    },
    GetScroll: function () {
        var w = document.body.scrollWidth || document.body.scrollLeft || document.documentElement.scrollLeft;
        w = w ? w : 0;
        var h = document.body.scrollHeight || document.body.scrollTop || document.documentElement.scrollTop;
        h = h ? h : 0;
        return { "h": h, "w": w };
    },
    GetViewPort: function () {
        var w = 0;
        var h = 0;
        if (window.innerHeight != window.undefined) {
            h = window.innerHeight;
        } else if (document.compatMode == 'CSS1Compat') {
            h = document.documentElement.clientHeight;
            w = document.documentElement.clientWidth;
        } else if (document.body) {
            h = document.body.clientHeight;
            w = document.body.clientWidth;
        };
        if (window.innerWidth != window.undefined && w == 0) {
            w = window.innerWidth;
        };
        return { "h": h, "w": w };
    },
    GetStyle: function (id, propName) {
        var x = document.getElementById(id);
        var y = '';
        if (x.currentStyle) {
            y = x.currentStyle[propName];
        } else if (window.getComputedStyle) {
            y = document.defaultView.getComputedStyle(x, null).getPropertyValue(propName);
        };
        return y;
    },
    Elastic: function (id, h, step, speed, timeToClose) {
        var obj = document.getElementById(id);
        if (obj === null) return false;
        if (typeof (step) == 'undefined' || step == null) {
            step = 10;
        };
        var divH = amcp.UI.GetStyle(id, 'height').replace('px', '');
        if (divH == '') {
            divH = 1;
        };
        obj.style.overflow = 'hidden';
        var remove = false;
        if (h == -1) {
            remove = true;
            h = 1;
        };
        if (divH >= h && step > 0) {
            obj.style.height = '1px';
            divH = 1;
        };
        obj.style.display = 'block';
        function exp() {
            if ((divH >= h && step > 0) || (step < 0 && divH <= h)) {
                clearTimeout(timer);
                obj.style.overflow = 'auto';
                obj.style.height = h + 'px';
                if (h == '0') {
                    obj.style.display = 'none';
                };
                if (remove == true) {
                    var p = obj.parentNode;
                    p.removeChild(obj);
                };
                timer = null;
                if (typeof (timeToClose) != 'undefined') {
                    setTimeout(function () { amcp.UI.Elastic(id, 0, -step, speed); }, timeToClose);
                };
                return true;
            } else {
                divH = (parseInt(divH) + parseInt(step));
                try {
                    obj.style.height = divH + 'px';
                } catch (err) {
                };


                clearTimeout(timer);
                timer = setTimeout(exp, speed);
            };
        };
        var timer = setTimeout(exp, speed);
    },
    Transition: function (id1, id2, mode) {
        if (mode == 'fade') {
            var obj1 = document.getElementById(id1);
            obj1.style.display = amcp.UI.GetStyle(id1, 'display');
            amClient.UI.Fade(id1, 10, 10);
            var max = 0;
            function check() {
                max += 1;
                if (obj1.style.display == 'none' || max >= 500) {
                    clearTimeout(tcheck);
                    amcp.UI.Appear(id2, 10, 10);
                    tcheck = null;
                } else {
                    clearTimeout(tcheck);
                    tcheck = setTimeout(check, 5);
                };
            };
            var tcheck = setTimeout(check, 5);
        } else {

        };
    },
    Fade: function (id, step, speed) {
        var obj = window.document.getElementById(id);
        if (obj === null) return false;
        if (amcp.UI.GetStyle(id, 'display') == 'none') {
            obj.style.display = '';
        };
        if (amcp.UI.GetStyle(id, 'visibility') == 'hidden') {
            return false;
        };
        var alpha = 10;
        function f() {
            alpha--;
            amcp.UI.SetOpacity(obj, alpha);
            if (alpha > -1) {
                clearTimeout(timer);
                timer = setTimeout(f, speed);
            } else {
                clearTimeout(timer);
                obj.style.display = 'none';
                obj.style.visibility = 'hidden';
                timer = null;
            };
        };
        var timer = setTimeout(f, speed);
    },
    Appear: function (id, step, speed) {
        var obj = window.document.getElementById(id);
        if (obj === null) return false;
        var alpha = 0;
        amcp.UI.SetOpacity(obj, alpha);
        obj.style.visibility = 'visible';
        obj.style.display = 'block';
        function f() {
            alpha++;
            amcp.UI.SetOpacity(obj, alpha);
            if (alpha < 11) {
                clearTimeout(timer);
                timer = setTimeout(f, speed);
            } else {
                obj.style.display = 'block';
                obj.style.visibility = 'visible';
                amcp.UI.SetOpacity(obj, 10);
                clearTimeout(timer);
                timer = null;
            };
        };
        var timer = setTimeout(f, speed);
    },
    SetOpacity: function (obj, val) {
        if (val > 10) {
            val = 10;
        };
        obj.style.opacity = val / 10;
        obj.style.filter = 'alpha(opacity=' + val * 10 + ')';
    },
    LoadMask: function () {
      
    },
    ClearMask: function () {
        var mDiv = document.getElementById('ammodalmask');
        if (typeof (mDiv) != 'undefined' && mDiv !== null) {
            var p = mDiv.parentNode;
            if (p !== null) {
                p.removeChild(mDiv);
            };
        };
    },
    DivToModal: function (id) {
        var div = document.getElementById(id);
        var w = div.style.width.replace('px', '');
        var h = div.style.height.replace('px', '');
        var winWidth = amcp.UI.GetScroll().w;
        var divTop = 0;
        divTop = (amcp.UI.GetViewPort().h / 2) - (h / 2);
        if (document.all) {
            divTop += top.document.documentElement.scrollTop;
        } else {
            divTop += top.pageYOffset;
        };
        var divLeft = (winWidth / 2) - (w / 2);
        div.style.position = 'absolute';
        div.style.top = divTop + 'px';
        div.style.left = divLeft + 'px';
        div.style.zIndex = 1000;
        div.style.zoom = 1;
        var mytime;
        amcp.UI.Appear(id, 10, 10);
    },
    ShowSuccess: function (msg) {
        var msgSpan = document.getElementById('amcpnotify-message');
        msgSpan.innerHTML = msg;
        var notify = document.getElementById('amcpnotify');
        notify.className = 'amcpnotify amcpsuccess';
        amcp.UI.Elastic('amcpnotify', 48, 10, 20, 1500);
    },
    ShowWarn: function (msg) {
        var msgSpan = document.getElementById('amcpnotify-message');
        msgSpan.innerHTML = msg;
        var notify = document.getElementById('amcpnotify');
        notify.className = 'amcpnotify amcpwarn';
        amcp.UI.Elastic('amcpnotify', 48, 10, 20, 2000);
    }

};
function afadmin_callback(data, cb) {
    var http = new XMLHttpRequest();
    var url = afAdminHandlerURL;
    http.open('POST', url, true);
    http.setRequestHeader('content-type', 'application/x-www-form-urlencoded');
    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {

            try {
                var result = '';
                if (http.responseText != '') {
                    if (http.responseText.indexOf('[') == 0 || http.responseText.indexOf('{') == 0) {
                        result = JSON.parse(http.responseText);
                    } else {
                        result = http.responseText;
                    };

                };
                if (cb != null) {
                    cb(result);
                };

            } catch (err) {
                alert(err.message + '\n' + http.responseText);
            };
        };
    };
    http.send(data);
};
function afadmin_viewLoader(view, params, cb) {
    var v = {};
    v.action = 7;
    v.view = view;
    afadmin_callback(JSON.stringify(v), afadmin_viewLoaded);
};
function afadmin_viewLoaded(result) {
    var d = document.getElementById('amcp_Dispatch');
    d.innerHTML = result;
};
function afV(eid) {
    return document.getElementById(eid);
};
function afGet(id) {
    var e = afV(id);
    if (e == null) { return null; };
    var t = e.tagName;
    var required = e.getAttribute('required');
    var val = '';
    switch (t) {
        case 'INPUT':
            if (e.type == 'checkbox') {
                if (e.checked) {
                    val = true;
                } else {
                    val = false;
                };
            } else if (e.type == 'radio') {
                val = { "value": e.value, "checked": e.checked };
            } else {
                val = e.value;
            };

            break;
        case 'SELECT':
            val = { "value": e.options[e.selectedIndex].value, "text": e.options[e.selectedIndex].text };
            break;
        case 'TEXTAREA':
            val = e.value;


        default:
            break;
    };
   
    return val;
};
var specchar = false;
function filterInput(obj, evt, shadow) {
    var dest = null;
    if (typeof (shadow) != 'undefined') {
        dest = document.getElementById(shadow);
    };
    var charCode = (evt.which != undefined) ? evt.which : evt.keyCode;
    if (obj.value.length > 50 && charCode != 8) {
        return false;
    };
    if (dest !== null && charCode != 8 && charCode != 46) {
        dest.value += String.fromCharCode(charCode);
    };
    var k = '';
    if (
		(charCode == 32) ||
		(charCode > 47 && charCode < 91) ||
		(charCode > 96 && charCode < 123) ||
		(charCode == 8) ||
		(charCode == 0)
		) {
        if (charCode == 32) {
            k = '-';
            if (specchar) {
                return false;
            };
            specchar = true;
            obj.value = obj.value.toString().trim() + k;
            return false;

        } else if (charCode > 8 && charCode != 32) {
            k = String.fromCharCode(charCode);
            specchar = false;
        };
        
    } else {
        return false;
    };
};
function updateShadow(obj, evt, shadow) {
    var dest = null;
    if (typeof (shadow) != 'undefined') {
        dest = document.getElementById(shadow);
    };
    var charCode = (evt.which != undefined) ? evt.which : evt.keyCode;
    if (charCode == 8 || charCode == 46) {
        dest.value = obj.value.replace('-',' ');
    };
};
function getQueryString() {
    var result = {}, queryString = location.search.substring(1),
          re = /([^&=]+)=([^&]*)/g, m;
    if (queryString == '' && location.href != '') {
        queryString = location.href.substring(location.href.indexOf('?') + 1);
    
    }
     
    while (m = re.exec(queryString)) {
        result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
    }
  
    return result;
}
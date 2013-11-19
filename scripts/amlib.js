var actmod = {};
actmod.Utils = {
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
    GetElementsByClassName: function (clsName) {
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
actmod.UI = {
    currentModalDiv: null,
    LoadDiv: function (id, header) {
        if (actmod.UI.currentModalDiv !== null) {
            actmod.UI.ClearMask();
            document.getElementById(actmod.UI.currentModalDiv).style.display = 'none';
            actmod.UI.currentModalDiv = null;
        };
        document.getElementById(id + '_header').innerHTML = header;
        actmod.UI.LoadMask();
        actmod.UI.currentModalDiv = id;
        actmod.UI.DivToModal(id);
    },
    CloseDiv: function (id) {
        actmod.UI.Fade(id, 10, 10);
        actmod.UI.ClearMask();
        actmod.UI.currentModalDiv = null;
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
        var divH = actmod.UI.GetStyle(id, 'height').replace('px', '');
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
                    setTimeout(function () { actmod.UI.Elastic(id, 0, -step, speed); }, timeToClose);
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
            obj1.style.display = actmod.UI.GetStyle(id1, 'display');
            actmod.UI.Fade(id1, 10, 10);
            var max = 0;
            function check() {
                max += 1;
                if (obj1.style.display == 'none' || max >= 500) {
                    clearTimeout(tcheck);
                    actmod.UI.Appear(id2, 10, 10);
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
        if (actmod.UI.GetStyle(id, 'display') == 'none') {
            obj.style.display = '';
        };
        if (actmod.UI.GetStyle(id, 'visibility') == 'hidden') {
            return false;
        };
        var alpha = 10;
        function f() {
            alpha--;
            actmod.UI.SetOpacity(obj, alpha);
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
        actmod.UI.SetOpacity(obj, alpha);
        obj.style.visibility = 'visible';
        obj.style.display = 'block';
        function f() {
            alpha++;
            actmod.UI.SetOpacity(obj, alpha);
            if (alpha < 11) {
                clearTimeout(timer);
                timer = setTimeout(f, speed);
            } else {
                obj.style.display = 'block';
                obj.style.visibility = 'visible';
                actmod.UI.SetOpacity(obj, 10);
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
        var mDiv = document.createElement('div');
        mDiv.setAttribute('id', 'ammodalmask');
        mDiv.style.position = 'absolute';
        mDiv.style.top = '0px';
        mDiv.style.left = '0px';
        mDiv.style.backgroundColor = '#666666';
        mDiv.style.filter = 'alpha(opacity=50)';
        mDiv.style.opacity = '.5';
        amcp.UI.SetOpacity(mDiv, 5);
        mDiv.style.display = '';
        mDiv.style.zIndex = '100';
        var scroll = amcp.UI.GetScroll();
        mDiv.style.width = scroll.w + 'px';

        var vp = amcp.UI.GetViewPort();
        if (scroll.h > vp.h) {
            mDiv.style.height = scroll.h + 'px';
        } else {
            mDiv.style.height = vp.h + 'px';
        };
        document.body.appendChild(mDiv);
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
        divTop = (actmod.UI.GetViewPort().h / 2) - (h / 2);
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
        actmod.UI.Appear(id, 10, 10);
    },
    ElementWidth: function (id) {
        var el = document.getElementById(id);
        if (el == null) {
            return 0;
        } else {
            return el.offsetWidth;
        };
    },
    ShowSuccess: function (msg) {
        var w = actmod.UI.ElementWidth('afcontainer');
        if (w == 0) {
            w = 500;
        };
        var msgSpan = document.getElementById('amnotify-message');
        msgSpan.innerHTML = msg;
        var notify = document.getElementById('amnotify');
        notify.style.width = w +'px';
        notify.className = 'amnotify amsuccess';
        actmod.UI.Elastic('amnotify', 48, 10, 20, 1500);
    },
    ShowWarn: function (msg) {
        var w = actmod.UI.ElementWidth('afcontainer');
        if (w == 0) {
            w = 500;
        };
        var msgSpan = document.getElementById('amnotify-message');
        msgSpan.innerHTML = msg;
        var notify = document.getElementById('amnotify');
        notify.style.width = w + 'px';
        notify.className = 'amnotify amwarn';
        actmod.UI.Elastic('amnotify', 48, 10, 20, 2000);
    }

};
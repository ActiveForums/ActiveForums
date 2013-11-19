var am = {};
am.Utils = {
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
    SetSelected: function (obj, value) {
        var drp;
        if (typeof (obj) == 'object') {
            drp = obj;
        } else {
            drp = document.getElementById(obj);
        };
        for (var x = 0; x < drp.options.length; x++) {
            if (drp.options[x].value == value) {
                drp.options[x].setAttribute('selected', 'selected');
                drp.selectedIndex = x;
            } else {
                drp.options[x].removeAttribute('selected');
            };
        };
    },
    GetElementsByClassName: function (clsName, parent) {
        var retVal = new Array();
        var elements;
        if (typeof (parent) == 'undefined') {
            elements = document.getElementsByTagName("*");
        } else {
            parent = document.getElementById(parent);
            elements = parent.getElementsByTagName("*");
        };
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
            dd = document.getElementById(select);
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
    },
    Trim: function (str, chars) {
        return this.LTrim(this.RTrim(str, chars), chars);
    },
    LTrim: function (str, chars) {
        chars = chars || "\\s";
        return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
    },
    RTrim: function (str, chars) {
        chars = chars || "\\s";
        return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
    },
    RemoveChildNodes: function (id) {
        var p = document.getElementById(id);
        if (p.hasChildNodes()) {
            while (p.childNodes.length >= 1) {
                p.removeChild(p.firstChild);
            };
        };
    },
    OnlyNumbers: function (evt) {
        var charCode = (evt.which != undefined) ? evt.which : evt.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode != 46 && charCode != 44 && charCode != 45)) {
            return false;
        };
        return true;
    },
    CheckMinMax: function (obj, min, max) {
        if (obj.value > max) {
            obj.value = max;
        } else if (obj.value < min) {
            obj.value = '';
        };
    },
    CheckDefault: function (obj, val) {
        if (obj.value < val) {
            obj.value = val;
        };
    },
    CreateObject: function (obj) {
        var func = function () { };
        func.prototype = obj;
        return new func();
    },
    FillSelect: function (data, id) {
        if (data != '') {
            var drp;

            if (typeof (id) == 'object') {
                drp = id;
            } else {
                drp = document.getElementById(drp);
            };
            if (drp.options.length > 0) {
                am.Utils.RemoveChildNodes(id);
            };

            for (var i = 0; i < data.length; i++) {
                p = data[i];
                am.Utils.addOption('', p.itemId, p.itemName, drp);
            };
        };

    }
};
am.UI = {
    currentModalDiv: null,
    LoadDiv: function (id, options) {

        var $modal = $('#' + id);
        var width = $modal.width();
        var height = $modal.height();
        var modtitle = document.getElementById(id).getAttribute('title');
        if (typeof (options) != 'undefined') {
            width = options.width;
            height = options.height;
            modtitle = options.title;
        }
        $modal.dialog({
            modal: true,
            autoOpen: true,
            dialogClass: "dnnFormPopup",
            position: "center",
            minWidth: width,
            minHeight: height,
            maxWidth: width,
            maxHeight: height,
            resizable: false,
            closeOnEscape: true,
            title: modtitle,
            close: function (event, ui) {
                $(this).dialog("destroy");
            }
        });

        if (typeof (options) != 'undefined') {
            if (typeof (options.loadEvent) != 'undefined') {
                eval(options.loadEvent);
            }
        }
        

    },
    CloseDiv: function (id) {
        $("#" + id).dialog("close");
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
        var divH = am.UI.GetStyle(id, 'height').replace('px', '');
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
                    setTimeout(function () { am.UI.Elastic(id, 0, -step, speed); }, timeToClose);
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
            obj1.style.display = am.UI.GetStyle(id1, 'display');
            am.UI.Fade(id1, 10, 15);
            var max = 0;
            function check() {
                max += 1;
                if (obj1.style.display == 'none' || max >= 500) {
                    clearTimeout(tcheck);
                    am.UI.Appear(id2, 10, 15);
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
        if (am.UI.GetStyle(id, 'display') == 'none') {
            obj.style.display = '';
        };
        if (am.UI.GetStyle(id, 'visibility') == 'hidden') {
            return false;
        };
        var alpha = 10;
        function f() {
            alpha--;
            am.UI.SetOpacity(obj, alpha);
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
        am.UI.SetOpacity(obj, alpha);
        obj.style.visibility = 'visible';
        obj.style.display = 'block';
        function f() {
            alpha++;
            am.UI.SetOpacity(obj, alpha);
            if (alpha < 11) {
                clearTimeout(timer);
                timer = setTimeout(f, speed);
            } else {
                obj.style.display = 'block';
                obj.style.visibility = 'visible';
                am.UI.SetOpacity(obj, 10);
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
        am.UI.SetOpacity(mDiv, 5);
        mDiv.style.display = '';
        mDiv.style.zIndex = '100';
        var scroll = am.UI.GetScroll();
        mDiv.style.width = scroll.w + 'px';

        var vp = am.UI.GetViewPort();
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
        if (jQuery) {
            var winH = $(window).height();
            var winW = $(window).width();
            $('#' + id).css('top', (winH / 2) - ($('#' + id).height() / 2));
            $('#' + id).css('left', (winW / 2) - ($('#' + id).width() / 2));
            $('#' + id).fadeIn();
        } else {
            var div = document.getElementById(id);
            var w = am.UI.GetStyle(id, 'width').replace('px', '');
            var h = am.UI.GetStyle(id, 'height').replace('px', '');
            var winWidth = am.UI.GetScroll().w;
            var divTop = 0;
            divTop = (am.UI.GetViewPort().h / 2) - (h / 2);
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
            am.UI.Appear(id, 10, 10);
        }

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
        var w = am.UI.ElementWidth('afcontainer');
        if (w == 0) {
            w = 500;
        };
        var msgSpan = document.getElementById('amnotify-message');
        msgSpan.innerHTML = msg;
        var notify = document.getElementById('amnotify');
        notify.style.width = w + 'px';
        notify.className = 'amnotify amsuccess';
        am.UI.Elastic('amnotify', 48, 10, 20, 1500);
    },
    ShowWarn: function (msg) {
        var w = am.UI.ElementWidth('afcontainer');
        if (w == 0) {
            w = 500;
        };
        var msgSpan = document.getElementById('amnotify-message');
        msgSpan.innerHTML = msg;
        var notify = document.getElementById('amnotify');
        notify.style.width = w + 'px';
        notify.className = 'amnotify amwarn';
        am.UI.Elastic('amnotify', 48, 10, 20, 2000);
    },
    ToggleTab: function (obj, p) {
        var selClass = 'am-ui-tab-selected';
        var cnt = document.getElementById(obj.id + '-content');
        var sels = am.Utils.GetElementsByClassName(selClass, p);
        var currDiv = '';
        for (var i = 0; i < sels.length; i++) {
            var el = sels[i];
            var curClass = el.className;
            curClass = curClass.replace(selClass, '');
            el.className = am.Utils.Trim(curClass, ' ');
            if (el.tagName == 'DIV') {
                currDiv = el.id;
            } else {
                currDiv = el.id + '-content';
            };
        };

        if (obj.className != '') {
            obj.className += ' ';
        };
        obj.className += selClass;

        //cnt.className += selClass;
        if (currDiv != '') {
            am.UI.Transition(currDiv, cnt.id, 'fade');
        } else {
            am.UI.Appear(cnt.id, 10, 20);
        };
    },
    ElementWidth: function (id) {
        var el;
        if (typeof (id) == 'Object') {
            el = id;
        } else {
            el = document.getElementById(id);
        };
        if (el == null) {
            return 0;
        } else {
            return el.offsetWidth;
        };
    },
    ShowMessage: function (msg, css, id, height, duration) {
        var d = document.getElementById(id);
        var w = am.UI.ElementWidth(d.parentNode);
        if (w == 0) {
            w = 500;
            d.style.marginLeft = 'auto';
            d.style.marginRight = 'auto';
        };
        d.style.width = w + 'px';
        am.Utils.RemoveChildNodes(id);
        var div = document.createElement('div');
        div.className = css;
        var i = document.createElement('i');
        div.appendChild(i);
        var span = document.createElement('span');
        span.appendChild(document.createTextNode(msg));
        div.appendChild(span);
        d.appendChild(div);
        am.UI.Elastic(id, height, 10, 20, duration);
    },
    HideMessage: function (id) {
        am.Utils.RemoveChildNodes(id);
        am.UI.Elastic(id, 0, -10, 20);

    }


};
am.Tabs = {
    init: function (tabStripId) {
        var ul = document.getElementById(tabStripId);
        var items = ul.getElementsByTagName('LI');
        for (var i = 0; i < items.length; i++) {
            var li = items[i];
            li.onclick = new Function('am.UI.ToggleTab(this,\'' + tabStripId + '\')');

            if (i == 0) {
                am.UI.ToggleTab(li, ul.id);
                //li.className = 'am-ui-tab-selected';
                //var cnt = document.getElementById(li.id + '-content');
                //cnt.style.display = 'block';
            };
        };
    }
};
am.Form = {
    BindForm: function (data) {
        for (var p in data) {
            var el = document.getElementById(p);
            if (el != null) {
                switch (el.tagName) {
                    case 'INPUT':
                        switch (el.type) {
                            case 'text':
                                el.value = data[p];
                                break;
                            case 'checkbox':
                                el.checked = data[p];
                                break;
                        };
                        break;
                    case 'SELECT':
                        am.Utils.SetSelected(el.id, data[p]);
                        break;
                    case 'TEXTAREA':
                        el.value = data[p];
                        break;

                };

            };
        };
    },
    BuildForm: function (id, data) {
        var ul = document.createElement('ul');
        ul.className = 'am-ui-form';
        for (var p in data) {
            var li = document.createElement('li');
            var label = document.createElement('label');
            label.setAttribute('for', p);
            label.appendChild(document.createTextNode(p));
            li.appendChild(label);
            var tp = typeof (data[p]);
            var inp = document.createElement('input');
            inp.setAttribute('id', p);
            if (tp == 'string' || tp == 'number') {
                inp.setAttribute('type', 'text');
                inp.value = data[p];
            } else if (tp == 'boolean') {
                inp.setAttribute('type', 'checkbox');
                if (data[p] == true) {
                    inp.checked = true;
                };
            };
            li.appendChild(inp);
            ul.appendChild(li);
        };
        var li = document.createElement('li');
        var btnSave = document.createElement('span');
        btnSave.appendChild(document.createTextNode('Save'));
        btnSave.onclick = function () { lm_saveLicenseType(); };
        li.appendChild(btnSave);
        var btnReset = document.createElement('input');
        btnReset.setAttribute('type', 'reset');
        btnReset.value = 'Reset';
        li.appendChild(btnReset);
        ul.appendChild(li);
        document.getElementById(id).appendChild(ul);
    },
    PackageForm: function (obj) {
        for (var p in obj) {
            var el = document.getElementById(p);
            if (el !== null) {
                if (el.type == 'text' || el.tagName == 'TEXTAREA' || el.type == 'hidden') {
                    obj[p] = el.value;
                } else if (el.tagName == 'SELECT') {
                    obj[p] = el.options[el.selectedIndex].value;
                } else {
                    obj[p] = el.checked;
                };
            };


        };
        return obj;
    },
    ShowRequired: function (id) {
        document.getElementById(id).focus();
    }
};
am.RolePicker = {
    id: null,
    roles: [],
    currentRoles: null,
    Bind: function () {
        am.Utils.addOption('drp' + this.id, '', '');
        for (var i = 0; i < this.roles.length; i++) {
            am.Utils.addOption('drp' + this.id, this.roles[i].itemName, this.roles[i].itemId);
        };
        if (this.currentRoles != ''){
            var cr = this.currentRoles.split(';');
            for (var i = 0; i<cr.length;i++){
                if (cr[i] != ''){
                    this.Add(this.GetRole(cr[i]));
                };
            };
        };
    },
    Add: function (item) {
        var drp = document.getElementById('drp' + this.id);
        if (typeof (item) == 'undefined') {
            rid = drp.options[drp.selectedIndex].value;
            rname = drp.options[drp.selectedIndex].text;
        }else{
            rid = item.itemId;
            rname = item.itemName;
        };
        var ul = document.getElementById(this.id);
        var itemId = this.id + '-' + rid;
        if (document.getElementById(itemId) == null) {
            ul.appendChild(this.CreateTokenElement(itemId, rname));
        };

    },
    Remove: function (id) {
        var li = document.getElementById(id);
        if (li != null) {
            var p = document.getElementById(this.id);
            p.removeChild(li);
        };
    },
    Selected: function () {
        var roleIds = '';
        var p = document.getElementById(this.id);
        var items = p.getElementsByTagName('LI');
        for (var i = 0; i < items.length; i++) {
            roleIds += items[i].id.replace(this.id + '-', '') + ';'
        };
        return roleIds;
    },
    CreateTokenElement: function (id, text) {
        var li = document.createElement('li');
        li.className = 'am-ui-token';
        var b = document.createElement('b');
        b.onclick = new Function(this.id + '.Remove(\'' + id + '\')');
        li.appendChild(b);
        var sp = document.createElement('span');
        sp.appendChild(document.createTextNode(text));
        li.appendChild(sp);
        li.setAttribute('id', id);
        return li;
    },
    GetRole: function (rid){
        for (var i = 0; i < this.roles.length; i++) {
            if (this.roles[i].itemId == rid){
                return this.roles[i];
            };
        };
    }
};
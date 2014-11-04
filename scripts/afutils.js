var amaf_selectedTab;


function amaf_getSelectedTab() {
    return amaf_selectedTab;
};
function amaf_toggleTab(tab) {
    if (amaf_selectedTab != tab.id) {
        amaf_selectedTab = tab.id;
        var obj = document.getElementsByTagName("div");
        for (var i = 0; i < obj.length; i++) {
            var el = obj[i];
            if (el.id.indexOf('amafcontent') > 1) {
                el.style.display = 'none';
            };
            if (el.id.indexOf('_text') > 1) {
                el.className = 'amtabtext';
            };
            if (el.className == 'amtabsel') {
                el.className = 'amtab';
            };
        };
        var tabContent = document.getElementById(tab.id + '_amafcontent');
        var tabtext = document.getElementById(tab.id + '_text');
        var tab = document.getElementById(tab.id);
        tab.className = 'amtabsel';
        tabtext.className = 'amtabseltext';
        tabContent.style.display = 'block';
    };
};

function toggleGroup(whichgroup, cssOn, cssOff) {
    var myDate = new Date();
    myDate.setDate(myDate.getDate() + 30);
    var oGroup = eval(document.getElementById('group' + whichgroup));
    var oImage = eval(document.getElementById("imgGroup" + whichgroup));
    if (oGroup.style.display == 'none') {
        oGroup.style.display = '';
        oImage.src = af_imgPath + '/images/arrows_down.png';
        oImage.className = cssOn;
        document.cookie = whichgroup + 'S=T; expires=' + myDate.toGMTString() + '; path=/;';
    } else {
        oGroup.style.display = 'none';
        oImage.src = af_imgPath + '/images/arrows_left.png';
        oImage.className = cssOff;
        document.cookie = whichgroup + 'S=F; expires=' + myDate.toGMTString() + '; path=/;';

    };
};
function aftoggleSection(whichgroup) {
    var oGroup = eval(document.getElementById('section' + whichgroup));
    var oImage = eval(document.getElementById('imgSection' + whichgroup));
    if (oGroup.style.display == 'none') {
        oGroup.style.display = '';
        oImage.src = af_imgPath + '/images/arrows_down.png';
    } else {
        oGroup.style.display = 'none';
        oImage.src = af_imgPath + '/images/arrows_left.png';
    };
};
function af_showLoad() {
    var afdiv = document.getElementById('afgrid');
    if (afdiv.style.position == '' || afdiv.style.position == 'static') {
        afdiv.style.position = 'relative';
    };
    var afloaddiv = document.createElement('div');
    afloaddiv.setAttribute('id', 'afLoadDiv');
    afloaddiv.style.position = 'absolute';
    afloaddiv.style.float = 'left';
    afloaddiv.style.top = '0px';
    afloaddiv.style.left = '0px';
    afloaddiv.style.width = afdiv.offsetWidth + 'px';
    afloaddiv.style.height = afdiv.offsetHeight + 'px';
    afloaddiv.style.zIndex = '1000';
    afloaddiv.className = 'afloader';
    afSpinLg.style.position = 'absolute';
    afSpinLg.style.top = ((afdiv.offsetHeight / 2) - 16) + 'px';
    afSpinLg.style.left = ((afdiv.offsetWidth / 2) - 16) + 'px';
    afloaddiv.appendChild(afSpinLg);
    afdiv.appendChild(afloaddiv);
};
function af_clearLoad() {
    var afdiv = document.getElementById('afgrid');
    if (afdiv.style.position == 'relative' || afdiv.style.position == 'static') {
        afdiv.style.position = '';
    };
    var ld = document.getElementById('afLoadDiv');
    if (ld != null) {
        afdiv.removeChild(ld);
    };

};
function af_OnTreeClick(evt) {
    var src = window.event != window.undefined ? window.event.srcElement : evt.target;
    var isChkBoxClick = (src.tagName.toLowerCase() == 'input' && src.type == "checkbox");
    if (isChkBoxClick) {
        var parentTable = af_GetParentByTagName('table', src);
        var nxtSibling = parentTable.nextSibling;
        if (nxtSibling && nxtSibling.nodeType == 1) {
            if (nxtSibling.tagName.toLowerCase() == 'div') {
                af_CheckUncheckChildren(parentTable.nextSibling, src.checked);
            };
        };
        af_CheckUncheckParents(src, src.checked);
    };
};
function af_CheckUncheckChildren(childContainer, check) {
    var childChkBoxes = childContainer.getElementsByTagName('input');
    var childChkBoxCount = childChkBoxes.length;
    for (var i = 0; i < childChkBoxCount; i++) {
        childChkBoxes[i].checked = check;
    };
};
function af_CheckUncheckParents(srcChild, check) {
    var parentDiv = af_GetParentByTagName('div', srcChild);
    var parentNodeTable = parentDiv.previousSibling;
    if (parentNodeTable) {
        var checkUncheckSwitch;
        if (check) {
            var isAllSiblingsChecked = af_AreAllSiblingsChecked(srcChild);
            if (isAllSiblingsChecked) {
                checkUncheckSwitch = true;
            } else {
                return;
            };
        } else {
            checkUncheckSwitch = false;
        };
        var inpElemsInParentTable = parentNodeTable.getElementsByTagName('input');
        if (inpElemsInParentTable.length > 0) {
            var parentNodeChkBox = inpElemsInParentTable[0];
            parentNodeChkBox.checked = checkUncheckSwitch;
            af_CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
        };
    };
};
function af_AreAllSiblingsChecked(chkBox) {
    var parentDiv = af_GetParentByTagName('div', chkBox);
    var childCount = parentDiv.childNodes.length;
    for (var i = 0; i < childCount; i++) {
        if (parentDiv.childNodes[i].nodeType == 1) {
            if (parentDiv.childNodes[i].tagName.toLowerCase() == 'table') {
                var prevChkBox = parentDiv.childNodes[i].getElementsByTagName('input')[0];
                if (!prevChkBox.checked) {
                    return false;
                };
            };
        };
    };
    return true;
};
function af_GetParentByTagName(parentTagName, childElementObj) {
    var parent = childElementObj.parentNode;
    while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
        parent = parent.parentNode;
    };
    return parent;
};
function amaf_ShowTip(obj, message) {
    position = amaf_getTipPosition(obj);
    var tt = document.getElementById("amtip");
    var tttxt = document.getElementById("amtiptext");
    tt.style.left = position.x + 'px';
    tt.style.top = (position.y + 18) + 'px';
    tt.style.visibility = "visible";
    tt.style.display = '';
    tttxt.innerHTML = message;
};
function amaf_HideTip() {
    var tt = document.getElementById("amtip");
    tt.style.visibility = "hidden";
    tt.style.display = 'none';
};
function amaf_getTipPosition(e) {
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
var amaf_timetoclose;
function amaf_MsgBox(title, text, height, width, type) {
    var scroll = actmod.UI.GetScroll();
    var view = actmod.UI.GetViewPort();
    var top = ((scroll.h / 2) - (height / 2)) + 'px';
    var left  = ((view.w / 2) - (width / 2)) + 'px';
    var amMsgBox = document.getElementById("amMsgBox");
    amMsgBox.style.height = height + "px";
    amMsgBox.style.width = width + "px";
    amMsgBox.style.zIndex = 300000;
    amMsgBox.style.top = top;
    amMsgBox.style.left = left;
    amMsgBox.style.display = 'block';
    var amMsgBoxHeaderText = document.getElementById("amMsgBoxHeaderText");
    amMsgBoxHeaderText.innerHTML = title;
    var amMsgBoxText = document.getElementById("amMsgBoxText");
    amMsgBoxText.innerHTML = text;
   
    if (type == '') {
        amaf_timetoclose = setTimeout('amaf_MsgBoxClose()', 3000);
    } else {
        amaf_timetoclose = null;
    };
};
function amaf_MsgBoxClose() {
    var dlg = document.getElementById("amMsgBox");
    dlg.style.display = 'none';
    if (amaf_timetoclose != null) {
        clearTimeout(amaf_timetoclose);
    };

};
function onlyNumbers(evt) {
    var charCode = (evt.which != undefined) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode != 46) && (charCode != 44) && (charCode != 45)) {
        return false;
    } else {
        return true;
    };
};
function amaf_setParam(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    } else {
        var expires = "";
    };
    document.cookie = name + "=" + value + expires + "; path=/";
};
function amaf_getParam(name) {
    var value = "; " + document.cookie;
    var parts = value.split("; " + name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
    return "";
};
function afAddBookmark(title, url) {
    if (window.external) {
        window.external.AddFavorite(url, title);
    } else if (window.sidebar) {
        window.sidebar.addPanel(title, url, '');
    } else if (window.opera && window.print) {
        return true;
    };
};
var nmode = '';
var tmpimgsrc = '';
var tmpimgid = '';
function af_notifyComplete() {
    
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
    mDiv.style.width = amGetScrollWidth();
    mDiv.style.height = amGetScrollHeight();
    mDiv.style.display = '';
    //mDiv.style.zIndex = '1000';
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
function amGetScrollWidth() {
    var w = document.body.scrollWidth ||
           document.body.scrollLeft ||
           document.documentElement.scrollLeft;

    return w ? w : 0;
};

function amGetScrollHeight() {
    var h = document.body.scrollHeight ||
           document.body.scrollTop ||
           document.documentElement.scrollTop;

    return h ? h : 0;
};
function insertCode(sText) {
    var newMessage;
    var strMessage = document.getElementById('txtBody').value;
    newMessage = strMessage + sText;
    document.getElementById('txtBody').value = newMessage;
    document.getElementById('txtBody').focus();
    return;
};
function nothing() {
    return;
};


function insertQuote() {
    var txt = "";
    if (document.getSelection) {
        txt = document.getSelection();
    } else if (document.selection && document.selection.createRange) {
        txt = document.selection.createRange().text;
    } else {
        return;
    };
    if (txt != "") {
        var s = new String();
        s += "[quote]";
        s += txt + "[/quote]";
        insertCode(s);
    } else {
        insertCode('[quote] [/quote]');
    };
};
function afQuickSubmit() {
    var hid = document.getElementById('hidReply1');
    hid.value = 'true';
    document.forms[0].submit();
};
function amaf_catSelect(obj) {
    var chkv = document.getElementById('amaf-catselect');
    chkv.value = '';
    var cks = document.getElementsByTagName('input');
    for (var i = 0; i < cks.length; i++) {
        var myElement = cks[i];
        if (myElement.type == "checkbox") {
            var id = myElement.id;
            if (myElement.checked == true && id.indexOf('amaf_cat_') >= 0) {
                chkv.value += myElement.value + ';';
            };
        };
    };
};

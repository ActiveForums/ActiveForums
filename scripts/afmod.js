function amaf_modDel(tid) {
    if (confirm(amaf.resx.DeleteConfirm)) {
        var d = {};
        d.action = 6;
        d.topicid = tid;
        amaf.callback(d, amaf_modDelComplete);
    };
};
function amaf_modDelComplete(result) {
    if (result[0].success == true) {
        afreload();
    };
};
function amaf_openMove(tid) {
    var d = {};
    d.action = 15;
    $('#aftopicmove-topicid').val(tid);
    amaf.callback(d, amaf_loadMoveTopic);
};
function amaf_loadMoveTopic(result) {
    am.UI.LoadDiv('aftopicmove');
    $('#drpForums').append($(result));
    var d = {};
    d.action = 13;
    d.topicid = $('#aftopicmove-topicid').val();
    amaf.callback(d, amaf_bindLoadMoveTopic);
   
};
function amaf_bindLoadMoveTopic(result) {
    if (result[0].success == true) {
        var t = result[0].result;
        $('#aftopicmove-topicid').val(t.topicid);
        $('#aftopicmove-subject').val(t.subject);
        $('#aftopicmove-currentforum').val(t.forumname);
    };
}
function amaf_modMove(tid, fid) {
    var d = {};
    d.action = 7;
    d.topicid = tid;
    d.forumid = fid;
    amaf.callback(d, amaf_modMoveComplete);
};
function amaf_modMoveComplete(result) {
    if (result[0].success == true) {
        afreload();
    };
};
function amaf_modPin(tid) {
    var d = {};
    d.action = 8;
    d.topicid = tid;
    amaf.callback(d, amaf_modPinComplete);
};
function amaf_modPinComplete(result) {
    if (result[0].success == true) {
        afreload();
    };
};
function amaf_modLock(tid) {
    var d = {};
    d.action = 9;
    d.topicid = tid;
    amaf.callback(d, amaf_modLockComplete);
};
function amaf_modLockComplete(result) {
    if (result[0].success == true) {
        afreload();
    };
};
function amaf_quickEdit(tid) {
    amaf_resetQuickEdit();
    am.UI.LoadDiv('aftopicedit');
    var d = {};
    d.action = 13;
    d.topicid = tid;
    amaf.callback(d,amaf_loadTopicComplete);
};
function amaf_resetQuickEdit() {
    document.getElementById('aftopicedit-topicid').value = '';
    document.getElementById('aftopicedit-subject').value = '';
    document.getElementById('aftopicedit-tags').value = '';
    document.getElementById('aftopicedit-priority').value = 0;
    am.Utils.SetSelected('aftopicedit-status', '-1');
    document.getElementById('aftopicedit-locked').checked = false;
    document.getElementById('aftopicedit-pinned').checked = false;
    am.Utils.RemoveChildNodes('catlist');
    am.Utils.RemoveChildNodes('proplist');

};
function amaf_loadTopicComplete(result) {
    if (result[0].success == true) {
        var t = result[0].result;
        document.getElementById('aftopicedit-topicid').value = t.topicid;
        document.getElementById('aftopicedit-subject').value = t.subject;
        document.getElementById('aftopicedit-tags').value = t.tags;
        document.getElementById('aftopicedit-priority').value = t.priority;
        am.Utils.SetSelected('aftopicedit-status', t.status);
        document.getElementById('aftopicedit-locked').checked = t.locked;
        document.getElementById('aftopicedit-pinned').checked = t.pinned;
        amaf_loadCatList(t.categories);
        amaf_loadProperties(t.properties);
    };
};
function amaf_loadCatList(cats) {
    var iCount = cats.length;
    var ul = document.getElementById('catlist');
    am.Utils.RemoveChildNodes('catlist');
    for (var i = 0; i < iCount; i++) {
        var c = cats[i];
        var li = document.createElement('li');
        li.setAttribute('id', c.id);
        var chk = document.createElement('input');
        chk.setAttribute('id', 'cat-' + c.id);
        chk.setAttribute('type', 'checkbox');
        chk.value = c.id;
        chk.checked = c.selected;
        li.appendChild(chk);
        li.appendChild(document.createTextNode(c.name));
        ul.appendChild(li);
    };
};
function amaf_loadProperties(props) {
    var iCount = props.length;
    var ul = document.getElementById('proplist');
    am.Utils.RemoveChildNodes('proplist');
    for (var i = 0; i < iCount; i++) {
        var p = props[i];
        var li = document.createElement('li');
        li.setAttribute('id', p.propertyid);
        var lbl = document.createElement('label');
        lbl.setAttribute('for', 'prop-' + p.propertyid);
        lbl.appendChild(document.createTextNode(p.propertyname));
        li.appendChild(lbl);
        switch (p.datatype) {
            case 'text':
                var txt = document.createElement('input');
                txt.setAttribute('id', 'prop-' + p.propertyid);
                txt.setAttribute('type', 'text');
                txt.value = p.propertyvalue;
                li.appendChild(txt);
                ul.appendChild(li);
                break;
            case 'yesno':
                var txt = document.createElement('input');
                txt.setAttribute('id', 'prop-' + p.propertyid);
                txt.setAttribute('type', 'checkbox');
                txt.value = p.propertyvalue;
                txt.checked = p.propertyvalue;
                li.appendChild(txt);
                ul.appendChild(li);
                break;
            default:
                var sel = document.createElement('select');
                sel.setAttribute('id', 'prop-' + p.propertyid);
                li.appendChild(sel);
                ul.appendChild(li);
                am.Utils.FillSelect(p.listdata, sel);
                am.Utils.SetSelected(sel, p.propertyvalue);
            
        };
        
        
    };
};
function amaf_saveTopic() {
    var d = {};
    d.action = 14;
    d.topicid = document.getElementById('aftopicedit-topicid').value;
    d.subject = document.getElementById('aftopicedit-subject').value;
    d.tags = document.getElementById('aftopicedit-tags').value;
    d.priority = document.getElementById('aftopicedit-priority').value;
    var stat = document.getElementById('aftopicedit-status');
    d.status = stat.options[stat.selectedIndex].value;
    d.locked = document.getElementById('aftopicedit-locked').checked;
    d.pinned = document.getElementById('aftopicedit-pinned').checked;
    var ul = document.getElementById('proplist');
    var props = ul.getElementsByTagName('li');
    for (var i = 0; i < props.length; i++) {
        var l = props[i];
        var pname = 'prop-' + l.id;
        var el = document.getElementById(pname);
        if (el.tagName == 'INPUT') {
            if (el.type == 'text') {
                d[pname] = el.value;
            } else {
                d[pname] = el.checked;
            };
        } else {
            d[pname] = el.options[el.selectedIndex].value;
        };

    };
    var ul = document.getElementById('catlist');
    var cats = ul.getElementsByTagName('li');
    d.categories = '';
    for (var i = 0; i < cats.length; i++) {
        var li = cats[i];
        var chk = document.getElementById('cat-' + li.id);
        if (chk.checked){
            d.categories += chk.value + ';';
        };
    };
    amaf.callback(d, amaf_saveTopicComplete);   
};
function amaf_saveTopicComplete(result) {
    am.UI.CloseDiv('aftopicedit');
    window.location.href = window.location.href;

};
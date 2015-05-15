function af_loadComplete() {
    window.scrollTo(0, 0);
};
function afreload() {
    af_showLoad();
    document.location.reload(true);
};
var amaf_timerID = null;
var amaf_timerRunning = false;
var amaf_runtime = 0;
function amaf_startTimer() {
    amaf_runtime = amaf_runtime + 1;
    amaf_timerRunning = true;
    amaf_timerID = setTimeout('amaf_startTimer()', 1);

};
function amaf_stopTimer() {
    if (amaf_timerRunning) {
        clearTimeout(amaf_timerID);
    };
    amaf_timerRunning = false;
};
function amaf_trackTime() {
    amaf_runtime = 0;
    amaf_stopTimer();
    amaf_startTimer();
};
var amaf = {
    callback: function (data, cb) {
        amaf_trackTime();
        var http = new XMLHttpRequest();
        var url = afHandlerURL;
        http.open('POST', url, true);
        http.setRequestHeader('content-type', 'application/x-www-form-urlencoded');
        http.onreadystatechange = function () {
            if (http.readyState == 4 && http.status == 200) {
                try {
                    var result = http.responseText;
                    amaf_stopTimer();
                    if (result.indexOf('[') == 0 || result.indexOf('{') == 0) {
                        result = JSON.parse(result);
                        if (typeof (result[1]) != 'undefined') {
                            amaf_handleDebug(result[1]);
                        };
                    }


                    if (cb != null) {
                        cb(result);
                    };
                } catch (err) {
                    amaf_stopTimer();
                    alert(err.message + '\n' + http.responseText);
                };
            };
        };
        data = JSON.stringify(data);
        http.send(data);
    }
};
function amaf_pinger() {
    var d = {};
    d.action = 1;
    amaf.callback(d, null);
};
function amaf_uo() {
    var d = {};
    d.action = 2;
    amaf.callback(d, amaf_uocomplete);
};
function amaf_uocomplete(result) {
    try {
        var u = document.getElementById('af-usersonline');
        u.innerHTML = result[0].result;
    } catch (err) {
        alert(err.message);
    };
};

function amaf_topicSubscribe(fid, tid) {
    var d = {};
    d.action = 3;
    d.forumid = fid;
    d.topicid = tid;
    amaf.callback(d, amaf_topicSubscribeComplete);
};
function amaf_topicSubscribeComplete(result) {
    var r = result[0].result;
    if (!r) return;
    $('input[type=checkbox]#amaf-chk-subs')
        .prop('checked', r.subscribed)
        .siblings('label[for=amaf-chk-subs]').html(r.text);
};
function amaf_forumSubscribe(fid, uid) {
    var d = {};
    d.action = 4;
    d.forumid = fid;
    if (typeof (uid) != 'undefined') {
        d.userid = uid;
    };
    amaf.callback(d, amaf_forumSubscribeComplete);
};
function amaf_forumSubscribeComplete(result) {
    var r = result[0].result;
    if (!r) return;

    // Checkbox
    $('input[type=checkbox]#amaf-chk-subs')
        .prop('checked', r.subscribed)
        .siblings('label[for=amaf-chk-subs]').html(r.text);

    $('img#amaf-sub-' + r.forumid).each(function () {
        var imgSrc = $(this).attr('src');
        if (r.subscribed)
            $(this).attr('src', imgSrc.replace(/email_unchecked/, 'email_checked'));
        else
            $(this).attr('src', imgSrc.replace(/email_checked/, 'email_unchecked'));
    });
};
function amaf_changeRate(r, t) {
    var d = {};
    d.action = 5;
    d.rate = r;
    d.topicid = t;
    amaf.callback(d, amaf_rateComplete);
};
function amaf_rateComplete(result) {
    var r = document.getElementById('af-rater');
    var rate = result[0].result;
    var rv = document.getElementById('af-rate-value');
    rv.value = rate;
    if (typeof (r) != 'undefined') {
        r.className = 'fa-rater fa-rate' + rate;
    };
};
function amaf_hoverRate(obj, r) {
    var p = obj.parentNode;
    var rv = document.getElementById('af-rate-value');
    if (typeof (r) == 'undefined') {
        r = rv.value;
    };
    p.className = 'fa-rater fa-rate' + r;
};
function amaf_markAnswer(tid, rid) {
    var d = {};
    d.action = 10;
    d.topicid = tid;
    d.replyid = rid;
    amaf.callback(d, amaf_markAnswerComplete);
};
function amaf_markAnswerComplete() {
    afreload();
};
function amaf_loadSuggest(field, prepop, type) {
    if (typeof (type) == 'undefined') {
        type = -1;
    };
    if (prepop !== null) {
        prepop = [prepop];
    };
    var url = afHandlerURL + '&action=11';
    jQuery("#" + field).tokenInput(url, {
        tokenLimit: 100, prePopulate: prepop,
        classes: {
            tokenList: "token-input-list-facebook",
            token: "token-input-token-facebook",
            tokenDelete: "token-input-delete-token-facebook",
            selectedToken: "token-input-selected-token-facebook",
            highlightedToken: "token-input-highlighted-token-facebook",
            dropdown: "token-input-dropdown-facebook",
            dropdownItem: "token-input-dropdown-item-facebook",
            dropdownItem2: "token-input-dropdown-item2-facebook",
            selectedDropdownItem: "token-input-selected-dropdown-item-facebook",
            inputToken: "token-input-input-token-facebook"
        }
    });
};
function amaf_postDel(tid, rid) {
    if (confirm(amaf.resx.DeleteConfirm)) {
        var d = {};
        d.action = 12;
        d.topicid = tid;
        d.replyid = rid;
        amaf.callback(d, amaf_postDelComplete);
    };

};
function amaf_postDelComplete(result) {
    if (result[0].success == true) {
        if (typeof (result[0].result) != 'undefined') {
            var rid = result[0].result.split('|')[1];
            if (rid > 0) {
                afreload();
            } else {
                window.history.go(-1);
            };
        }

    };
};

function amaf_splitRestore() {
    var split_topicid = amaf_getParam('splitId');
    if (typeof (split_topicid) != 'undefined') {
        if (split_topicid == current_topicid) {
            var sv = amaf_getParam('splitValue');
            if (sv != '') splitposts = sv.split('|');
            amaf_splitButtons(true);
            return;
        }
    }
    amaf_splitButtons(false);
}


function amaf_splitCheck(el) {
    if (el.checked) {
        if (splitposts.indexOf(el.value) < 0) splitposts.push(el.value);
        var saved_split = splitposts.join('|');
        amaf_setParam('splitValue', saved_split, 0);
    }
    else {
        var index = splitposts.indexOf(el.value);
        splitposts.splice(index, 1);
        var saved_split = splitposts.join('|');
        amaf_setParam('splitValue', saved_split, 0);
    }
};
function amaf_splitCreate(el, tid) {
    amaf_setParam('splitId', tid, 0);
    amaf_setParam('splitValue', '', 0);
    splitposts = new Array();
    amaf_splitButtons(true);
};

function amaf_splitButtons(opt) {
    var btns = document.getElementById('splitbuttons');
    if (typeof (btns) == 'undefined') return;
    if (opt) {
        btns.childNodes[0].style.display = 'none';
        btns.childNodes[1].style.display = 'block';
        var objs = am.Utils.GetElementsByClassName('split-checkbox', 'afgrid');
        for (var i = 0; i < objs.length; i++) {
            objs[i].style.display = 'block';
            if (splitposts.indexOf(objs[i].firstChild.value) > -1) objs[i].firstChild.checked = true;
        };
    }
    else {
        btns.childNodes[0].style.display = 'block';
        btns.childNodes[1].style.display = 'none';
        var objs = am.Utils.GetElementsByClassName('split-checkbox', 'afgrid');
        for (var i = 0; i < objs.length; i++) {
            objs[i].style.display = 'none';
            objs[i].firstChild.checked = false;
        };
    }


};

function amaf_splitCancel() {
    amaf_setParam('splitId', '', 0);
    amaf_setParam('splitValue', '', 0);
    splitposts = new Array();
    amaf_splitButtons(false);
};

function amaf_likePost(userId, contentId) {
    var d = {};
    d.action = 16;
    d.userId = userId;
    d.contentId = contentId;
    amaf.callback(d, amaf_likePostComplete);
};
function amaf_likePostComplete(result) {
    if (result[0].success == true) {
        if (typeof (result[0].result) != 'undefined') {
            var rid = result[0].result.split('|')[1];
            if (rid > 0) {
                afreload();
            } else {
                window.history.go(-1);
            };
        }

    };
};
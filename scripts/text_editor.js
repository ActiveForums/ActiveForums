window.isTextEditor = true;

function amaf_insertHTML(html) {
    document.getElementById(afeditor).value += html;
};

function amaf_getBody() {
    return document.getElementById(afeditor).value;
};

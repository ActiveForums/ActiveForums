function amaf_insertHTML(html) {
    if (document.getElementById(afeditor).createTextRange) {
        document.getElementById(afeditor).focus(document.getElementById(afeditor).caretPos);
        document.getElementById(afeditor).caretPos = document.selection.createRange().duplicate();
        if (document.getElementById(afeditor).caretPos.text.length > 0) {
            document.getElementById(afeditor).caretPos.text = document.getElementById(afeditor).caretPos.text;
        } else {
            document.getElementById(afeditor).caretPos.text = html;
        };
    } else {
    document.getElementById(afeditor).value += html;
    };

};
function amaf_getBody() {
    return document.getElementById(afeditor).value;
};
function amaf_insertHTML(html) {
    var editor = $find(afeditor + '_txtBody');
    if (editor != null) {
        editor.pasteHtml(html);
    };
};
function amaf_getBody() {
    var editor = $find(afeditor + '_txtBody');
    return editor.get_html();
};
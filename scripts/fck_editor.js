function amaf_insertHTML(html) {
    if (window.FCKeditorAPI && afeditor != null) {
        var editor = FCKeditorAPI.GetInstance(afeditor + '_txtBody');
        if (editor != null) {
            editor.InsertHtml(html);
        };
    };
};
function amaf_getBody() {
    if (window.FCKeditorAPI && afeditor != null) {
        var editor = FCKeditorAPI.GetInstance(afeditor + '_txtBody');
        if (editor != null) {
            return editor.GetHTML();
        };
    };
};
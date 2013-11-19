﻿function amaf_insertHTML(html) {
    if (window.CKEDITOR && afeditor != null) {
        var editor = CKEDITOR.instances[afeditor + '_txtBody'];
        if (editor != null && editor.mode == 'wysiwyg') {
            editor.insertHtml(html);
        };
     };
};

function amaf_getBody() {
    if (window.CKEDITOR && afeditor != null) {
        var editor = CKEDITOR.instances[afeditor + '_txtBody'];
        if (editor != null && editor.mode == 'wysiwyg') {
            return editor.getData();
        };
    };
};
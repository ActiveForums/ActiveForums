<script type="text/javascript">
    jQuery(document).ready(function () {
        afadmin_getProperties();
        jQuery(function () {
            $.extend($.fn.disableTextSelect = function () {
                return this.each(function () {
                    if ($.browser.mozilla) {//Firefox
                        $(this).css('MozUserSelect', 'none');
                    } else if ($.browser.msie) {//IE
                        $(this).bind('selectstart', function () { return false; });
                    } else {//Opera, etc.
                        $(this).mousedown(function () { return false; });
                    }
                });
            });
            $('.noSelect').disableTextSelect(); //No text selection on elements with a class of 'noSelect'
        });
    });
</script>
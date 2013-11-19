(function ($) {
    $.fn.afSearchPopup = function () {

        return this.each(function () {

            var obj = $(this);

            obj.data('visible', false);

            var popup = obj.find('.aftb-search-popup');

            var button = obj.find('button');
            button.button({ icons: { primary: "ui-icon-search" } });

            popup.css('left', (obj.width() - popup.width()) + "px");
            popup.css('top', obj.height() + "px");

            $('span', obj).first().click(function () {

                obj.data('visible', !obj.data('visible'));

                $('.ui-icon', this).toggleClass('ui-icon-triangle-1-s ui-icon-triangle-1-n');
                $('.aftb-search-popup', obj).toggle();

            });

            $(':text', obj).keypress(function (event) {
                if (event.keyCode == 13) {
                    $('button', obj).click();
                }
            });

            $('button', obj).click(function () {

                var resultType = obj.find('[type=radio]:checked').val();

                var query = obj.find('[type=text]').val();
                query = $.trim(query);
                if (!query)
                    return false;

                var searchUrl = obj.attr('data-searchUrl');

                document.location.href = searchUrl + "?q=" + query + "&rt=" + resultType;
                
                return false;
            });

            $(document).mouseup(function (e) {

                if (obj.has(e.target).length === 0 && obj.data('visible')) {
                    obj.data('visible', false);
                    $('.ui-icon', obj).toggleClass('ui-icon-triangle-1-s ui-icon-triangle-1-n');
                    $('.aftb-search-popup', obj).toggle();
                }
                    
            });

        });
    };
})(jQuery);

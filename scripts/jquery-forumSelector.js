(function ($) {
    $.fn.afForumSelector = function () {

        return this.each(function () {

            var obj = $(this);

            obj.find('option[value^="G"]').replaceWith(function () {
                var groupOption = $(this);
                return "<optgroup data-id='" + groupOption.val() + "' label='" + groupOption.text() + "' />";
            });

            var optionsGroups = obj.find('optgroup');

            optionsGroups.each(function () {
                var optionGroup = $(this);
                obj.find('option[value$="' + optionGroup.attr('data-id') + '"]').appendTo(optionGroup);
            });

            optionsGroups.click(function(e) {

                // Don't process clicks for children
                if (e.target == this) {

                    var groupOption = $(this);

                    var options = groupOption.find('option');
                    var selectedOptions = groupOption.find('option:selected');

                    if (selectedOptions.length < options.length)
                        options.attr('selected', 'selected');
                    else
                        options.removeAttr('selected');

                }

            });

        });
    };
    
})(jQuery);

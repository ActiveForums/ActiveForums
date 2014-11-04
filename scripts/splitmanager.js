(function ($) {

    $.fn.afSplitManager = function (options) {

        var opts = $.extend({}, $.fn.afSplitManager.defaultOptions, options);

        var $wrap = $(opts.openTriggerScope);
        var topicId = -1;
        var forumId = -1;
        var oTopicId;
        var forumsList;
        var topicList;
        var subject;
        var splitManagerDialog;
        var existed = false;

        opts.serviceurlbase = opts.servicesFramework.getServiceRoot('ActiveForums') + 'ForumService/';

        function openSplitManagerDialog() {

            // Construct the file
            if (!splitManagerDialog) {

                // Construct the form that will be displayed in the standard DNN Dialog
                var html = "<fieldset style='margin-top:20px;'>";
                html += "<div class='dnnFormItem' id='topic_selector'><div class='dnnLabel'>" + opts.typeTopicText + "</div>";
                html += "<label for='anexitopic'>" + opts.exTopicText + "</label><input type='radio' id='anexitopic' name='typetopic' value='1' />";
                html += "<label for='anewtopic'>" + opts.newTopicText + "</label><input type='radio' id='anewtopic' name='typetopic' checked='true' value='2' /></div>";
                html += "<div class='dnnFormItem' id='subjectNewItem'><div class='dnnLabel'><label for='subjectNew'>" + opts.subjectNewText + "</label></div><input type='text' id='subjectNew' maxlength='255' /></div>";
                html += "<div class='dnnFormItem' id='subjectExistingItem'><div class='dnnLabel'><label for='subjectExisting'>" + opts.subjectExistingText + "</label></div><select id='subjectExisting'>";
                html += "<option value='-1'>" + opts.selectTopicText + "</option>";
                html += "</select></div>";
                html += "<div class='dnnFormItem'><div class='dnnLabel'><label for='locationExisting'>" + opts.locationExistingText + "</label></div><select id='locationExisting'>";
                html += "<option value='-1'>" + opts.selectForumText + "</option>";
                html += "</select></div>";
                html += "</fieldset>";

                splitManagerDialog = $("<div id='afSplitManagerDialog' class='dnnForm dnnClear'/>").html(html).dialog(opts);

                splitManagerDialog.find("#subjectExistingItem").hide();


                splitManagerDialog.find('#topic_selector input:radio').click(
                    function () {
                        existed = $(this).val() == 1;
                        if (existed) {
                            splitManagerDialog.find("#subjectNewItem").hide();
                            splitManagerDialog.find("#subjectExistingItem").show();
                            splitManagerDialog.find("#locationExisting").change();
                        }
                        else {
                            splitManagerDialog.find("#subjectNewItem").show();
                            splitManagerDialog.find("#subjectExistingItem").hide();
                        }
                    }
                );

                splitManagerDialog.find("#locationExisting").change(function () {
                    forumId = $(this).val();
                    if (!existed) return;
                    if (forumId > 0) {
                        loadTopics();
                    }
                    /*else {
                    topicId = -1;
                    fillListFromJsonArray('subjectExisting', null);
                    }*/
                });
            }

            splitManagerDialog.dialog({
                minWidth: 650,
                modal: true,
                resizable: false,
                title: opts.titleText,
                buttons: [
                    { text: opts.splitText, click: splitStart },
                    { text: opts.cancelText, click: function () { $(this).dialog("close"); } }
                ],
                close: function () { }
            });

            splitManagerDialog.dialog('open');

        };

        function loadTopics() {
            $.ajax({
                url: opts.serviceurlbase + "GetTopicList?ForumId=" + forumId,
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                beforeSend: opts.servicesFramework.setModuleHeaders
            }).done(function (data) {
                fillListFromJsonArray('subjectExisting', data);
            }).fail(function (xhr, status) {
                alert('Topics Not Found');
            });
        }

        function splitStart() {
            var errorChk = '';
            if (forumId == -1) errorChk = '"Location" is not selected';
            else {
                if (existed) {
                    topicId = splitManagerDialog.find("#subjectExisting").val();
                    if (topicId == -1) errorChk = '"Topic" is not selected';
                }
                else {
                    topicId = -1;
                    subject = splitManagerDialog.find("#subjectNew").val();
                    if (subject == '') errorChk = '"Subject" is empty';
                }
            }
            if (oTopicId == topicId) return;

            if (errorChk != '') {
                alert(errorChk);
                return;
            }

            var params = {
                OldTopicId: oTopicId,
                NewTopicId: topicId,
                NewForumId: forumId,
                Subject: subject,
                Replies: amaf_getParam('splitValue')
            };

            $.ajax({
                url: opts.serviceurlbase + "CreateSplit",
                type: "POST",
                data: JSON.stringify(params),
                contentType: "application/json",
                dataType: "json",
                beforeSend: opts.servicesFramework.setModuleHeaders
            }).done(function (data) {
                splitManagerDialog.dialog('close');
                amaf_splitCancel()
                afreload();
            }).fail(function (xhr, status) {
                alert('Splitting Error');
                splitManagerDialog.dialog('close');
                amaf_splitCancel()
                afreload();
            });
        }

        function fillListFromJsonArray(list, json) {
            var $list = $('#' + list);
            $list.find('option').remove();
            if (json == null) return;
            var arr = $.parseJSON(json);
            if (arr != null) {
                $.each(arr, function (key, value) {
                    $list.append('<option value=' + key + '>' + value + '</option>');
                });
            }
        }

        $wrap.on('click', opts.openTriggerSelector, function (e) {

            e.preventDefault();
            e.stopPropagation();

            oTopicId = $(e.currentTarget).attr('data-id');

            $.ajax({
                url: opts.serviceurlbase + "GetForumsList",
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                beforeSend: opts.servicesFramework.setModuleHeaders
            }).done(function (data) {
                openSplitManagerDialog();
                fillListFromJsonArray('locationExisting', data);
                splitManagerDialog.find("#locationExisting").change();
            }).fail(function (xhr, status) {
                alert('Error Retriving User Profile');
            });
        });
    };

    $.fn.afSplitManager.defaultOptions = {
        openTriggerScope: 'body', // defines parent scope for openTriggerSelector, allows for event delegation
        openTriggerSelector: '.af-button-split', // opens dialog
        titleText: 'Add selected posts to',
        typeTopicText: "Add selected posts to:",
        exTopicText: "Existing Topic",
        newTopicText: "New Topic",
        subjectNewText: "Subject:",
        subjectExistingText: "Topic:",
        locationExistingText: "Location:",
        splitText: "Split",
        cancelText: "Cancel",
        selectForumText: "Select forum",
        selectTopicText: "Select topic",
        dialogClass: 'dnnFormPopup dnnClear',
        autoOpen: false
    };

} (jQuery));
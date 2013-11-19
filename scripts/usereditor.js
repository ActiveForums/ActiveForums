(function ($) {
    
    $.fn.afUserEditor = function (options) {

        var opts = $.extend({}, $.fn.afUserEditor.defaultOptions, options);

        var $wrap = $(opts.openTriggerScope);
        var userId;
        var userName;
        var editUserDialog;

        opts.serviceurlbase = opts.servicesFramework.getServiceRoot('ActiveForums') + 'AdminService/';
  
        function openEditUserDialog(userData) {

            // Construct the file
            if (!editUserDialog) {

                // Construct the form that will be displayed in the standard DNN Dialog
                var html = "<fieldset style='margin-top:20px;'>";
                html += "<div class='dnnFormItem'><div class='dnnLabel'><label for='trustLevel'>" + opts.trustLevelText + "</label></div><select id='trustLevel' name='trustLevel'>";
                html += "<option value='0'>" + opts.neutralText + "</option>";
                html += "<option value='1'>" + opts.trustedText + "</option>";
                html += "<option value='-1'>" + opts.untrustedText + "</option>";
                html += "</select></div>";
                html += "<div class='dnnFormItem'><div class='dnnLabel'><label for='userCaption'>" + opts.userCaptionText + "</label></div><input type='text' id='userCaption' name='userCaption' maxlength='255'/></div>";
                html += "<div class='dnnFormItem'><div class='dnnLabel'><label for='rewardPoints'>" + opts.rewardPointsText + "</label></div><input type='text' id='rewardPoints' name='rewardPoints'/></div>";
                html += "<div class='dnnFormItem'><div class='dnnLabel'><label for='signature'>" + opts.signatureText + "</label></div><textarea rows='2' cols='20' id='signature' name='signature'/></div>";
                html += "</fieldset>";
                                
                editUserDialog = $("<div id='afEditUserDialog' class='dnnForm dnnClear'/>").html(html).dialog(opts);
                
                // Numeric Input Only
                editUserDialog.find("#rewardPoints").keydown(function (event) {
                    // Allow: backspace, delete, tab, escape, and enter
                    if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 || (event.keyCode == 65 && event.ctrlKey === true) ||  (event.keyCode >= 35 && event.keyCode <= 39)) {
                        return;
                    }
                    else {
                        // Ensure that it is a number and stop the keypress
                        if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                            event.preventDefault();
                        }
                    }
                });
            }

            // Populate the form
            editUserDialog.find('#trustLevel').val(userData.TrustLevel);
            editUserDialog.find('#userCaption').val(userData.UserCaption);
            editUserDialog.find('#rewardPoints').val(userData.RewardPoints);
            editUserDialog.find('#signature').val(userData.Signature);
       
            editUserDialog.dialog({
                minWidth: 650,
                modal: true,
                resizable: false,
                title: opts.titleText + ' ' + userName,
                open: function () { editUserDialog.find("#trustLevel").focus(); },
                buttons: [
                    { text: opts.updateText, click: updateUser },
                    { text: opts.cancelText, click: function () { $(this).dialog("close"); } }
                ],
                close: function () { }
            });

            editUserDialog.dialog('open');

        };

        function updateUser() {

            var params = {
                UserID: userId,
                TrustLevel: parseInt(editUserDialog.find("#trustLevel").val()),
                UserCaption: editUserDialog.find("#userCaption").val(),
                RewardPoints: parseInt(editUserDialog.find("#rewardPoints").val()),
                Signature: editUserDialog.find("#signature").val()
            };
            $.ajax(
                {
                    url: opts.serviceurlbase + "UpdateUserProfile",
                    type: "POST",
                    data: JSON.stringify(params),
                    contentType: "application/json",
                    dataType: "json",
                    beforeSend: opts.servicesFramework.setModuleHeaders
                }).done(function (data) {
                    editUserDialog.dialog('close');
                }).fail(function (xhr, status) {
                    alert('Error Updating User Profile');
                });   
        }

        $(opts.openTriggerSelector).button({ icons: { primary:'ui-icon-person' } });

        $wrap.on('click', opts.openTriggerSelector, function (e) {

            e.preventDefault();
            e.stopPropagation();

            userId = $(e.currentTarget).attr('data-id');
            userName = $(e.currentTarget).attr('data-name');

            $.ajax(
            {
                url: opts.serviceurlbase + "GetUserProfile?UserID=" + userId,
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                beforeSend: opts.servicesFramework.setModuleHeaders
            }).done(function (data) {
                openEditUserDialog(data);
            }).fail(function (xhr, status) {
                alert('Error Retriving User Profile');
            });
        });
    };

    $.fn.afUserEditor.defaultOptions = {
        openTriggerScope: 'body', // defines parent scope for openTriggerSelector, allows for event delegation
        openTriggerSelector: '.af-button-edituser', // opens dialog
        titleText: 'Edit User:',
        trustLevelText: "Trust Level:",
        untrustedText: "Not Trusted",
        neutralText: "Neutral - Default",
        trustedText: "Trusted",
        userCaptionText: "User Caption:",
        signatureText: "Signature:",
        rewardPointsText: "Reward Points:",
        updateText: "Update User",
        cancelText: "Cancel",
        dialogClass: 'dnnFormPopup dnnClear',
        autoOpen: false
    };

}(jQuery));
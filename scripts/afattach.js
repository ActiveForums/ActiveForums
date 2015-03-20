

function AFAttachmentManager($, ko, options) {

    var $element, $attachments;
    var attachmentsViewModel;
    var sf = $.ServicesFramework(options.moduleId);

    // Handles the UI for the attachments list
    function AttachmentsViewModel() {

        var self = this;

        this.allowBrowseSite = options.allowBrowseSite;

        this.attachments = ko.observableArray($.parseJSON($attachments.val()) || []);

        this.attachments.subscribe(function (changes) {
            $attachments.val(JSON.stringify(self.attachments()));

            self.setAttachButtonVisibility();
        });

        this.hasAttachments = ko.computed(function () { return self.attachments().length ? true : false; });

        this.addAttachment = function (attachment) {

            if (!attachment || (options.maxAttachmentCount > 0 && self.attachments().length >= options.maxAttachmentCount))
                return;

            self.attachments.push(attachment);
        };

        this.setAttachButtonVisibility = function () {

            console.log(self.attachments().length);
            console.log(options.maxAttachmentCount);

            if (options.maxAttachmentCount > 0 && self.attachments().length >= options.maxAttachmentCount) {
                $element.find(".af-attach-upload").hide();
                console.log('hide');
            }
            else {
                $element.find(".af-attach-upload").show();
                $element.find(".fileupload-error").hide();
                $element.find(".fileupload-progress").hide();
            }
        };

        this.addUserFileAttachment = function (file) {

            if (!file || !file.id)
                return;

            // Make sure we don't already have the file in the list
            for (var i = 0; i < self.attachments().length; i++) {
                var item = self.attachments()[i];
                if (item.userFileId === file.id) {
                    return;
                }
            }

            var attachment = {
                fileName: file.name,
                fileUrl: file.thumb_url,
                contentType: file.type,
                fileSize: 0,
                fileSizeText: file.size,
                fileId: file.id
            };

            self.attachments.push(attachment);

            self.insertImage(attachment);
        };

        this.removeAttachment = function (item) {

            if (confirm(options.confirmRemoveText)) {
                self.attachments.remove(item);
            }
        };

        this.insertImage = function (item) {
            var fileid = item.fileId;

            var filetype = item.type || item.contentType || '';

            if (filetype) {
                var imageTypes = "gif,jpg,jpeg,png";

                filetype = filetype.substr(filetype.lastIndexOf("/") + 1);

                if ($.inArray(filetype, imageTypes.split(',')) > -1) {
                    $.ajax({
                        type: "GET",
                        url: '/DesktopModules/ActiveForums/API/ForumService/GetUserFileUrl?FileId=' + fileid,
                        beforeSend: sf.setModuleHeaders
                    })
                    .done(function (data) { self.getImageUrl(data); })
                    .fail(function (xhr, status) {alert('Topics Not Found')});
                    
                    return;
                }
            }
            $.ajax({
                type: "GET",
                url: '/DesktopModules/ActiveForums/API/ForumService/GetUserFileUrl?FileId=' + fileid,
                beforeSend: sf.setModuleHeaders,
            })
            .done(function (data) { self.getImageUrl(data); })
            .fail(function (xhr, status) {alert('Topics Not Found')});
        };

        this.getFileUrl = function (url) {
            var isTE = window.isTextEditor;
            if ((!isTE) && (options.attachInsertAllowed < 1)) amaf_insertHTML('<a href="' + url + '" target="_blank" >' + url.substr(url.lastIndexOf("/") + 1) + '</a>');
            //else amaf_insertHTML(url);
        }

        this.getImageUrl = function (url) {
            var isTE = window.isTextEditor;
            if ((!isTE) && (options.attachInsertAllowed < 1)) amaf_insertHTML('<img src="' + url + '" border="0" class="afpostimg" />');
            else amaf_insertHTML('[img]' + url + '[/img]');
        }

        this.getIconClass = function (item) {

            var fileName = item.fileName || "";

            var ext = fileName.substr((~ -fileName.lastIndexOf(".") >>> 0) + 2);

            return "af-fileicon af-fileicon-" + ext;
        };
    }

    this.initialize = function () {

        $element = $('#' + options.elementId);
        $attachments = $('#' + options.attachmentsClientId);


        //Make DNN 7.2 consistent with 7.0
        var $fileInput = $element.find("input[type=file]");
        if (!$fileInput.parent().hasClass('dnnInputFileWrapper')) {
            var wrapper = $("<span class='dnnInputFileWrapper'></span>");
            $fileInput.wrap(wrapper);
            var btn = $("<span class='dnnSecondaryAction'>Choose File</span>");
            btn.insertBefore($fileInput);
        }

        // bind view model

        attachmentsViewModel = new AttachmentsViewModel();

        ko.bindingHandlers.stopBinding = { init: function () { return { controlsDescendantBindings: true }; } };
        ko.virtualElements.allowedBindings.stopBinding = true;
        ko.applyBindings(attachmentsViewModel, $element[0]);

        // Init jQuery plugins

        $element.find('.af-attach-upload').afFileUpload({
            maxFileSize: options.maxUploadSize,
            serverErrorMessage: options.serverErrorMessage,
            fileTypeNotAllowedMsg: options.fileTypeNotAllowedMsg,
            allowedFileTypes: options.allowedFileTypes,
            maxFileSizeExceededMsg: options.maxFileSizeExceededMsg,
            uploadButtonText: options.uploadButtonText,
            beforeSend: sf.setModuleHeaders,
            callback: attachmentsViewModel.addAttachment,
            formData: { "forumId": options.forumId }
        });

        if (!options.allowBrowseSite) {

            $element.find('#photoFromSite').remove();
        }
        else {

            $element.find('.af-userFileManager').userFileManager({
                title: options.titleText,
                cancelText: options.cancelText,
                attachText: options.attachText,
                getItemsServiceUrl: sf.getServiceRoot('InternalServices') + 'UserFile/GetItems',
                nameHeaderText: options.nameHeaderText,
                typeHeaderText: options.typeHeaderText,
                lastModifiedHeaderText: options.lastModifiedHeaderText,
                fileSizeText: options.fileSizeText,
                templatePath: options.templatePath,
                templateName: 'Default',
                templateExtension: '.html',
                attachCallback: attachmentsViewModel.addUserFileAttachment
            });
        }


        // Have to call this after initializing the upload controls
        attachmentsViewModel.setAttachButtonVisibility();






    };
};
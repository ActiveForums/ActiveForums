

function AFAttachmentManager($, ko, options) {

        var $element, $attachments, $thumbnailer, $thumbnailerDialog;
        var attachmentsViewModel, thumbnailerViewModel;
        var sf = $.ServicesFramework(options.moduleId);
                
        function getFileTicket(url) {

            var fileData = { Url : url };
            
            //sf.getAntiForgeryProperty(data);
            
            $.ajax({
                type: "POST",
                url: sf.getServiceRoot('ActiveForums') + "ForumService/EncryptTicket",
                beforeSend: sf.setModuleHeaders,
                data: fileData,
                async: false,
                success: function (data) { amaf_insertHTML('<img src="' + options.fileLinkPrefix + data + '" />'); },
                error: function (xhr, status, error) { alert(error); }
            });  
        };
    
        function AttachmentFileViewModel(attachment) {

            var self = this;
            var isResizinhg = false;

            this.id = attachment.id;
            this.fileName = attachment.fileName;
            this.fileUrl = attachment.fileUrl;
            this.fileSize = attachment.fileSize;
            this.contentType = attachment.contentType;
            this.isImage = /(jpg|jpeg|png|gif)/i.test(attachment.contentType);
            this.displayLink = ko.observable(attachment.displayLink);

            this.displayLinkChecked = ko.computed(function () { return self.displayLink() || !self.isImage; });

            this.toggleDisplayLink = function() {
                if (self.isImage) {  self.displayLink(!self.displayLink()); }
            };
            
            this.insertImage = function () {
                if (options.editorType == 0) { amaf_insertHTML('[IMAGE:' + self.id + ']'); }
                else {  getFileTicket('FileId=' + self.id); }
            };

            this.createThumb = function () {
                thumbnailerViewModel.displayDialog(self);
            };
            
          
        }

        // Handles the UI for the attachments list
        function AttachmentsViewModel() {

            var self = this;

            this.attachments = ko.observableArray([]);

            this.attachments.subscribe(function(changes) { });

            this.hasAttachments = ko.computed(function() { return self.attachments().length ? true : false; });

            this.addAttachment = function(file) {

                // file object properties vary depending on the caller
                // So we convert it to a standard attachment which we pass to the viewmodel
                // constructor

                if (!file || (!file.id && !file.file_id))
                    return;

                var attachment = {
                    id : file.id || file.file_id,
                    fileName : file.name,
                    fileUrl : file.url || file.thumb_url,
                    displayLink : file.isImage,
                    contentType : file.type,
                    fileSize: (/\d+/.test(file.size) && file.size > 1024) ? Math.round(file.size / 1024) + "k" : file.size.toString()
                };
           
                self.attachments.push(new AttachmentFileViewModel(attachment));
            };

            this.removeAttachment = function(item) {
                self.attachments.remove(item);
            };
    
        }

        // Handles the UI for the resize dialog
        function ThumbnailerViewModel() {

            var self = this;
            var isResizing = false;

            // Extender to force integer input

            ko.extenders.thumbInput = function (target) {
                var result = ko.computed({
                    read: target,
                    write: function (newValue) {
                        var current = target();
                        newValue = isNaN(newValue) ? 0 : parseInt(newValue);
                        if (newValue !== current && newValue > 0) {
                            target(newValue);
                        } else {
                            target(current);
                            target.notifySubscribers(current);
                        }
                    }
                }).extend({ notify: 'always' });

                result(target());

                return result;
            };

            this.fileId = ko.observable();
            this.originalWidth = ko.observable(0);
            this.originalHeight = ko.observable(0);
            this.width = ko.observable(0).extend({ thumbInput: 1 });
            this.height = ko.observable(0).extend({ thumbInput: 1 });
            this.maintainAspectRatio = ko.observable(true);
            this.linkOriginal = ko.observable(true);

            this.width.subscribe(function (newWidth) {

                if (isResizing || !self.maintainAspectRatio() || isNaN(newWidth) || !self.originalWidth()) return;

                isResizing = true;
                self.height(Math.round((parseInt(newWidth) / self.originalWidth()) * self.originalHeight()));
                isResizing = false;
            });
            
            this.height.subscribe(function (newHeight) {

                if (isResizing || !self.maintainAspectRatio() || isNaN(newHeight) || !self.originalHeight()) return;

                isResizing = true;
                self.width(Math.round((parseInt(newHeight) / self.originalHeight()) * self.originalWidth()));
                isResizing = false;
            });

            this.maintainAspectRatio.subscribe(function(isEnabled) {

                if (!isEnabled) return;

                // This will force the height to be recalculated for the correct aspect ratio
                self.width.notifySubscribers(self.width());
            });

            this.displayDialog = function(attachment) {

                this.fileId(attachment.id);

                // Get the original image size
                var img = new Image();

                img.onload = function() {

                    self.originalWidth(img.naturalWidth || img.width);
                    self.originalHeight(img.naturalHeight || img.height);

                    if (!self.originalHeight() || !self.originalHeight()) {
                        img.onerror();
                        return;
                    }

                    if (self.originalWidth() <= 128 && self.originalHeight() <= 128) {
                        self.width(self.originalWidth());
                        self.height(self.originalHeight());
                    } else if (self.originalWidth() >= self.originalHeight()) {
                        self.width(128);
                        self.height(Math.round((128 / self.originalWidth()) * self.originalHeight()));
                    } else {
                        self.width(Math.round((128 / self.originalHeight()) * self.originalWidth()));
                        self.height(128);
                    }

                    $thumbnailerDialog.dialog('open');
                };

                img.onerror = function() { alert('could not load image'); };

                img.src = attachment.fileUrl;

            };

            this.createThumbnail = function(e) {

                if (!self.fileId() || !self.height() || !self.width())
                    return;

                var fileData = {
                    FileId: self.fileId(),
                    Height: self.height(),
                    Width: self.width()
                };

                //sf.getAntiForgeryProperty(data);

                $.ajax({
                    type: "POST",
                    url: sf.getServiceRoot('ActiveForums') + "ForumService/CreateThumbnail",
                    beforeSend: sf.setModuleHeaders,
                    data: fileData,
                    success: function (data) {

                        data = $.parseJSON(data);
                        
                        console.log(data);

                        if (options.editorType == 0) {
                            amaf_insertHTML('[THUMBNAIL:' + data.FileId + ':' + fileData.fileId + ']');
                        } else {
                            getFileTicket('FileId=' + data.FileId);
                        }
                        $thumbnailerDialog.dialog('close');
                    },
                    error: function(xhr, status, error) { alert(error); }
                });
            };
        }

        this.initialize = function() {

            $element = $('#' + options.elementId);
            $attachments = $('#' + options.attachmentsClientId);
            $thumbnailer = $('#' + options.elementId + "_thumbnailer");
            $thumbnailerDialog = $('#' + options.elementId + "_thumbnailer_dialog");

            // Need to do this before binding
            $thumbnailerDialog.dialog({ autoOpen: false, minWidth: 450, title: options.createThumbnailTitle, dialogClass: 'dnnFormPopup' });

            // bind view model

            attachmentsViewModel = new AttachmentsViewModel();
            thumbnailerViewModel = new ThumbnailerViewModel();
            
            ko.bindingHandlers.stopBinding = { init: function () { return { controlsDescendantBindings: true }; } };
            ko.virtualElements.allowedBindings.stopBinding = true;

            ko.applyBindings(attachmentsViewModel, $element[0]);
            ko.applyBindings(thumbnailerViewModel, $thumbnailer[0]);

            // Init jQuery plugins
            
            $element.find('.af_attachmentUpload').afFileUpload({                
                maxFileSize: options.maxUploadSize,
                serverErrorMessage: 'Some Error Message',
                beforeSend: sf.setModuleHeaders,
                callback: attachmentsViewModel.addAttachment,
                formData: { "forumId" : 1 }
            });

            $element.find('.af_userFileManager').userFileManager({
                title: options.titleText,
                cancelText: 'Cancel',
                attachText: 'Attach',
                getItemsServiceUrl: sf.getServiceRoot('InternalServices') + 'UserFile/GetItems',
                nameHeaderText: options.nameHeaderText,
                typeHeaderText: options.typeHeaderText,
                lastModifiedHeaderText: options.lastModifiedHeaderText,
                fileSizeText: options.fileSizeText,
                templatePath: options.templatePath,
                templateName: 'Default',
                templateExtension: '.html',
                attachCallback: attachmentsViewModel.addAttachment
            });

        };
    };
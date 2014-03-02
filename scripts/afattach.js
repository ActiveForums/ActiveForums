

function AFAttachmentManager($, ko, options) {

        var $element, $attachments;
        var attachmentsViewModel;
        var sf = $.ServicesFramework(options.moduleId);
                   
        // Handles the UI for the attachments list
        function AttachmentsViewModel() {

            var self = this;

            this.allowBrowseSite = options.allowBrowseSite;

            this.attachments = ko.observableArray($.parseJSON($attachments.val()) || []);

            this.attachments.subscribe(function(changes) {
                $attachments.val(JSON.stringify(self.attachments()));
            });

            this.hasAttachments = ko.computed(function () { return self.attachments().length ? true : false; });

            this.addAttachment = function(attachment) {

                if (!attachment)
                    return;

                self.attachments.push(attachment);
            };

            this.addUserFileAttachment = function(file) {

                if (!file || !file.id)
                    return;

                // Make sure we don't already have the file in the list
                for (var i = 0; i < self.attachments().length; i++) {
                    var item = self.attachments()[i];
                    if(item.userFileId === file.id) {
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
            };

            this.removeAttachment = function (item) {
                
                if(confirm(options.confirmRemoveText)) {
                    self.attachments.remove(item);
                }
            };

            this.getIconClass = function(item) {

                var fileName = item.fileName || "";

                var ext = fileName.substr((~-fileName.lastIndexOf(".") >>> 0) + 2);

                return "af-fileicon af-fileicon-" + ext;
            };
    
        }

        this.initialize = function() {

            $element = $('#' + options.elementId);
            $attachments = $('#' + options.attachmentsClientId);

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
                beforeSend: sf.setModuleHeaders,
                callback: attachmentsViewModel.addAttachment,
                formData: { "forumId" : options.forumId }
            });
            
            if(!options.allowBrowseSite) {

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



        };
    };
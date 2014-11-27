/*globals jQuery */
(function ($) {
    "use strict";

    var supportAjaxUpload = function () {
        var xhr = new XMLHttpRequest;
        return !!(xhr && ('upload' in xhr) && ('onprogress' in xhr.upload));
    };

    $.fn.afFileUpload = function (options) {

        var opts = $.extend({ }, $.fn.afFileUpload.defaultOptions, options),
            $wrap = $(this),
            $fileUploadWrapperSelector = $(opts.fileUploadWrapperSelector),
            $picker = $fileUploadWrapperSelector.find(opts.pickerContextSelector),
            $errorMsg = $fileUploadWrapperSelector.find(opts.errorMessageSelector),
            $progressContext = $fileUploadWrapperSelector.find(opts.progressContextSelector),
            $progressBar = $fileUploadWrapperSelector.find(opts.progressBarSelector).progressbar({ value: 0, max: 100 }),
            buttonText = $fileUploadWrapperSelector.find(".dnnInputFileWrapper span").text();

        function resetButtonText() {
            $fileUploadWrapperSelector.find(".dnnInputFileWrapper span").text(options.uploadButtonText);
        }

        // error response 
        $fileUploadWrapperSelector.bind('fileuploadfail', function (e, data) {
            opts.complete(data);
            $progressContext.hide();
            $errorMsg.text(opts.serverErrorMessage).show();
            setTimeout((function () { $errorMsg.hide('fade', function () { $picker.show('fade'); }); }), 4000);
        });

        // success response
        $fileUploadWrapperSelector.bind('fileuploaddone', function (e, data) {

            opts.complete(data);

            var result;

            if (data.result.body) {
                result = $.parseJSON(data.result.body.innerText);
            }
            else {
                result = data.result;
            }

            opts.callback(result);
        });
        
        // Reset the button text
        $fileUploadWrapperSelector.bind('fileuploadalways', function (e, data) {
            resetButtonText();
        });
        

        var url = opts.uploadServiceUrl;
        if (!supportAjaxUpload()) {
            var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
            url += '?__RequestVerificationToken=' + antiForgeryToken;
        }

        var $fileupload = $fileUploadWrapperSelector;
        if (!$fileupload.data('loaded')) {

            $fileupload.fileupload({
                dataType: 'json',
                url: url,
                formData: opts.formData,
                beforeSend: opts.beforeSend,
                add: function (e, data) {

                    var uploadErrors = [];
                    
                    if(opts.allowedFileTypes) {
                        var fileName = data.originalFiles[0].name;
                        var fileExtension = fileName.substr((~-fileName.lastIndexOf(".") >>> 0) + 2).toLowerCase();

                        var acceptableFileExtensions = opts.allowedFileTypes.toLowerCase().replace(' ', '').replace(/\./g, '').split(',');

                        if ($.inArray(fileExtension, acceptableFileExtensions) === -1) {
                            uploadErrors.push(opts.fileTypeNotAllowedMsg);
                        }
                    }

                    if (opts.maxFileSize && data.originalFiles[0]['size'] && data.originalFiles[0]['size'] > opts.maxFileSize) {
                        uploadErrors.push(opts.maxFileSizeExceededMsg);
                    }
                    
                    if (uploadErrors.length > 0) {
                        resetButtonText();
                        alert(uploadErrors.join("\n"));
                        return;
                    }

                    $picker.hide();

                    data.context = $progressContext;
                    data.context.find($(opts.progressFileNameSelector)).html(data.files[0].name);
                    data.context.show('fade');
                    data.submit();
                },
                progress: function (e, data) {
                    if (data.context) {
                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        data.context.find(opts.progressPercentageSelector).html(progress + '%');
                        $progressBar.progressbar("value", progress);
                    }
                },
                done: function (e, data) {

                    if (data.context) {
                        data.context.find(opts.progressPercentageSelector).html('100%');
                        $progressBar.progressbar("value", 100);
                        setTimeout((function () { data.context.hide('fade', function () { $picker.show('fade'); }); }), 2000);
                    }
                }
            }).data('loaded', true);

        }

        $wrap.show();

        resetButtonText();

    };

    $.fn.afFileUpload.defaultOptions = {
        fileUploadWrapperSelector: '.af-fileupload', // wrapper element for the main file upload content area
        uploadServiceUrl: '/DesktopModules/ActiveForums/API/ForumService/UploadFile', // post files here
        pickerContextSelector: '.fileupload-picker',
        progressContextSelector: '.fileupload-progress', // wrapper element for the progress area
        progressFileNameSelector: '.fileupload-filename', // element to update file name text w/ during upload
        progressPercentageSelector: '.fileupload-progress-percent',
        progressBarSelector: '.fileupload-progress-bar', // the actual progress bar element itself, its width will be expanded dynamically
        errorMessageSelector: '.fileupload-error', // the element to display the error message, its text will be updated dynamically
        serverErrorMessage: 'Unexpected error. This generally happens when the file is too large.', // error message when server returns a 500, 404 or the like
        fileTypeNotAllowedMsg: 'Not an accepted file type',
        allowedFileTypes: "gif,jpg,jpeg,png,txt,pdf,zip",
        maxFileSizeExceededMsg: 'Filesize is too big',
        maxFileSize: 1024000,
        formData: {},
        beforeSend: null, //method to set the request headers should be an instance on servicesFramework.setModuleHeaders
        callback: function (result) {
            // function called after the upload is successful. Supplied with result object representing the file.
            // key properties: name, extension, type, size, url, message, file_id
            // e.g. console.log('file id: ' + result.file_id + ' path: ' + result.url);
        },
        complete: function (data) {
            // A function to be called when the request finishes (after success and error callbacks are executed)
        }
    };
} (jQuery));

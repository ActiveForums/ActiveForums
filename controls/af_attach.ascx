<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_attach.ascx.cs"
	Inherits="DotNetNuke.Modules.ActiveForums.Controls.af_attach" %>
<%@ Import Namespace="DotNetNuke.Common.Utilities" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnJsInclude runat="server" PathNameAlias="SharedScripts" FilePath="knockout.js" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Components/UserFileManager/jquery.dnnUserFileUpload.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Components/UserFileManager/UserFileManager.js" Priority="105"></dnn:DnnJsInclude>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/Components/UserFileManager/UserFileManager.css"></dnn:DnnCssInclude>

<div id="userFileManager">
</div>
<div class="fileUploadArea">
	<div class="jpa" id="tbar-attach-Area">
		<span id="tbar-photoText">attach</span> <a href="#" id="photoFromSite">Browse From Site</a>
		<span>|</span> <span class="browser-upload-btn">Upload <span style="position:relative;"></span><input type="file" name="files[]" /></span>
	</div>
	<div id="itemUpload">
		<div class="fileupload-error dnnFormMessage dnnFormValidationSummary" style="display: none;">
		</div>
		<div class="progress_bar_wrapper">
			<div class="progress_context" style="margin: 10px 0px; display: none;">
				<div class="upload_file_name" style="margin-top: 5px; margin-bottom: -5px;">
				</div>
				<div class="progress-bar green">
					<div style="width: 0px;">
						<span></span>
					</div>
				</div>
			</div>
		</div>
		<div class="filePreviewArea">
		</div>
	</div>
</div>
<div id="attachments" style="clear:left;">
	<table>
		<thead>
			<tr>
				<td>
					 <%=GetSharedResource("[RESX:FileName]")%>
				</td>
				<td>
					 <%=GetSharedResource("[RESX:FileSize]")%>
				</td>
				<td>
					<%=GetSharedResource("[RESX:CreateThumbnail]")%>
				</td>
				<td>
					 <%=GetSharedResource("[RESX:InsertImage]")%>
				</td>
				<td>
					 <%=GetSharedResource("[RESX:DisplayLink]")%>
				</td>
				<td>
					 <%=GetSharedResource("[RESX:Delete]")%>
				</td>
			</tr>
		</thead>
		<tbody>
		</tbody>
	</table>
</div>
<div id="divThumbnail" class="afDialog" style="display: none;">
	<h2 class="dnnFormSectionHead">
		<%=GetSharedResource("[RESX:CreateThumbnail]")%></h2>
	<div class="options">
		<table>
			<tbody>
				<tr>
					<td>
						Width:
					</td>
					<td>
						<input type="text" value="175" class="aftextbox" style="width: 30px" onkeyup="amaf_recalc(2)"
							name="txtWidth" id="txtWidth">
					</td>
				</tr>
				<tr>
					<td>
						Height:
					</td>
					<td>
						<input type="text" value="175" class="aftextbox" style="width: 30px" onkeyup="amaf_recalc(1)"
							name="txtHeight" id="txtHeight">
					</td>
				</tr>
				<tr>
					<td colspan="4">
						Constrain proportions:<input type="checkbox" checked="" id="chkProp">
					</td>
				</tr>
				<tr>
					<td colspan="4">
						Link to original image:<input type="checkbox" checked="" id="chkOpenOrig">
					</td>
				</tr>
			</tbody>
		</table>
	</div>
	<ul class="dnnActions dnnClear">
		<li><a href="javascript:void(0);" class="dnnPrimaryAction" onclick="javascript:dnnaf_createThumbnail(100,2,150,150);">
			<%=GetSharedResource("[RESX:Create]")%></a></li>
	</ul>
	<input type="hidden" id="thumb-id" value="" />
</div>
<script type="text/javascript">
    var editorType = <%=(int)EditorType%>;
    function dnnaf_createThumbnail() {
        var sf = $.ServicesFramework(<%=ModuleId%>);
	    var data = {};
	    data.FileId = $('#thumb-id').val();
	    data.Height = $('#txtHeight').val();
	    data.Width = $('#txtWidth').val();
	    //sf.getAntiForgeryProperty(data);

	    $.ajax({
	        type: "POST",
	        url: sf.getServiceRoot('ActiveForums') + "ForumService/CreateThumbnail",
	        beforeSend: sf.setModuleHeaders,
	        data: data,
	        success: function (data) {

	            data = $.parseJSON(data);

	            if (editorType == 0) {
	                amaf_insertHTML('[THUMBNAIL:' + data.FileId + ':' + $('#thumb-id').val() + ']');
	            }else{
	                getFileTicket('FileId=' +  data.FileId);
	            }
	            $('#thumb-id').val('');
	            $('#divThumbnail').dialog('close');
	            if (typeof (callback) != "undefined") {
	                //callback();

	            }
	        },
	        error: function (xhr, status, error) {
	            alert(error);
	        }
	    });
	};
	$(document).ready(function () {

	    $('#divThumbnail').dialog({ autoOpen: false, minWidth: 450, title: '<%=GetSharedResource("[RESX:CreateThumbnail]")%>' });
	    var createThumbnail = "<img src='<%=Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/image_thumb.png")%>' />"
	    var insertImage = "<img src='<%=Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/image_insert.png")%>' />"
	    var deleteImage = "<img src='<%=Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/delete12.png")%>' />"
	    var uncheckedImage = "<img src='<%=Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/checkbox_unchecked.png")%>' />"
	    var checkedImage = "<img src='<%=Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/checkbox.png")%>' />"
	    var sf = $.ServicesFramework(<%=ModuleId%>);
	    var maxUploadSize = <%=Config.GetMaxUploadSize()%>;
	    $('.fileUploadArea').dnnUserFileUpload({
	        maxFileSize: maxUploadSize,
	        serverErrorMessage: 'Some Error Message',
	        addImageServiceUrl: sf.getServiceRoot('Journal') + 'FileUpload/UploadFile',
	        beforeSend: sf.setModuleHeaders,
	        callback: function (file) {
	            var $previewArea = $('.filePreviewArea');
	            var fileId = -1;
	            if (typeof(file.id) == 'undefined') {
	                fileId = file.file_id;
	            }else{
	                fileId = file.id;
	            }
	            $('#attachments table tbody').append('<tr id="' + fileId + '"><td>' + file.name + '</td><td>' + file.size + '</td><td class="createThumb">' + createThumbnail + '</td><td class="insertImage">' + insertImage + '</td><td>' + checkedImage + '</td><td>' + deleteImage + '</td></tr>');
	            $('#' + fileId + ' .insertImage').click(function(){
	                if (editorType == 0) {
	                    amaf_insertHTML('[IMAGE:' + this.parentNode.id + ']');
	                }else{
	                    getFileTicket('FileId=' + this.parentNode.id);

	                }
	            }); 
	            $('#' + fileId + ' .createThumb').click(function(){
	                $('#thumb-id').val(this.parentNode.id);
	                $('#divThumbnail').dialog('open');
	            });

	        }
	    });
	    $('#userFileManager').userFileManager({
	        title: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Title.Text"))%>',
			    cancelText: 'Cancel',
			    attachText: 'Attach',
			    getItemsServiceUrl: sf.getServiceRoot('InternalServices') + 'UserFile/GetItems',
			    nameHeaderText: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Name.Header"))%>',
				typeHeaderText: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("Type.Header"))%>',
			    lastModifiedHeaderText: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("LastModified.Header"))%>',
			    fileSizeText: '<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("FileSize.Header"))%>',
			    templatePath: '<%=Page.ResolveUrl("~/Resources/Shared/Components/UserFileManager/Templates/")%>',
			    templateName: 'Default',
			    templateExtension: '.html',
			    attachCallback: function (file) {
			        $('#attachments table tbody').append('<tr id="' + file.id + '"><td>' + file.name + '</td><td>' + file.size + '</td><td class="createThumb">' + createThumbnail + '</td><td class="insertImage">' + insertImage + '</td><td>' + checkedImage + '</td><td>' + deleteImage + '</td></tr>');
			        $('#' + file.id + ' .insertImage').click(function(){
			            if (editorType == 0) {
			                amaf_insertHTML('[IMAGE:' + this.parentNode.id + ']');
			            }else{
			                getFileTicket('FileId=' + this.parentNode.id);

			            }
			        });
			        $('#' + file.id + ' .createThumb').click(function(){
			            $('#thumb-id').val(this.parentNode.id);
			            $('#divThumbnail').dialog('open');
			        });
			    }
			});
	});

        function getFileTicket(url){
            var sf = $.ServicesFramework(<%=ModuleId%>);
    var data = {};
    data.Url = url;
    //sf.getAntiForgeryProperty(data);
    $.ajax({
        type: "POST",
        url: sf.getServiceRoot('ActiveForums') + "ForumService/EncryptTicket",
        beforeSend: sf.setModuleHeaders,
        data: data,
        async: false,
        success: function (data) {
            amaf_insertHTML('<img src="<%=Page.ResolveUrl("~/LinkClick.aspx")%>?fileticket=' + data + '" />');


			},
			error: function (xhr, status, error) {
			    alert(error);
			}
	});  
        };
        var amaf_imgH;
        var amaf_imgW;
        function amaf_recalc(src){
            var chk = document.getElementById("chkProp");
            var h = document.getElementById("txtHeight");
            var w = document.getElementById("txtWidth");
            if (chk.checked && h.value != '' && w.value != ''){
                var tmp;
                var tmpH = parseInt(h.value);
                var tmpW = parseInt(w.value)
                if (tmpH < amaf_imgH || tmpW < amaf_imgW){
                    if (src == 1){
                        tmp = (tmpH / amaf_imgH);
                        w.value = Math.round(amaf_imgW * tmp);
                    };
                    if (src == 2){
                        var dif = (tmpW / amaf_imgW);
                        h.value = Math.round(amaf_imgH * dif);
                    };
                };
            };
        };
</script>
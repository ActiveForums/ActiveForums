<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_attach.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Controls.af_attach" %>
<%@ Import Namespace="DotNetNuke.Common.Utilities" %>
<%@ Import Namespace="DotNetNuke.Modules.ActiveForums.Extensions" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnJsInclude runat="server" FilePath="knockout.js" PathNameAlias="SharedScripts" Priority="100"></dnn:DnnJsInclude>
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Components/UserFileManager/UserFileManager.js" Priority="105"></dnn:DnnJsInclude>
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/Components/UserFileManager/UserFileManager.css"></dnn:DnnCssInclude>
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/ActiveForums/Scripts/afattach.js" Priority="106"></dnn:DnnJsInclude>
<!-- ko stopBinding: true -->  
<div id="<%=ClientID%>">
    <!-- ko stopBinding: true -->
    <div class="af-userFileManager"></div>
    <div class="af-attach-upload af-fileupload">
	    <div class="fileupload-picker">
            <span>
                <input type="file" name="files[]" />
                <a href="#" id="photoFromSite" class="dnnSecondaryAction"><%=GetSharedResource("[RESX:BrowseFromSite]")%></a>
            </span>	
	    </div>
	    <div class="fileupload-status">
		    <div class="fileupload-error dnnFormMessage dnnFormValidationSummary" style="display: none;"></div>
            <div class="fileupload-progress" style="display:none;">
		        <span class="fileupload-progress-percent">0%</span>
                <span><%=GetSharedResource("[RESX:Uploading]")%> </span><span class="fileupload-filename"></span>
                <div class="fileupload-progress-bar"></div>
            </div>
	    </div>
    </div>
    <!-- /ko -->
    <div class="af-attach-editor-list" style="clear:left;">
        <!-- ko if:hasAttachments -->
	    <table>
		    <thead>
			    <tr>
				    <td><%=GetSharedResource("[RESX:FileName]")%></td>
				    <td><%=GetSharedResource("[RESX:FileSize]")%></td>
                    <td><%=GetSharedResource("[RESX:InsertImage]")%></td>
				    <td><%=GetSharedResource("[RESX:Delete]")%></td>
			    </tr>
		    </thead>
		    <tbody data-bind="foreach: attachments">
		        <tr>
                    <td><i data-bind="attr: { 'class' : $root.getIconClass($data) }"></i> <span data-bind="text:fileName"></span></td>
                    <td data-bind="text:(fileSize || fileSizeText)"></td>
                    <td><span class="ui-icon ui-icon-plus" data-bind="click:$root.insertImage" /></td>
                    <td><span class="ui-icon ui-icon-trash" data-bind="click:$root.removeAttachment" /></td>
		        </tr>
		    </tbody>
	    </table>
        <!-- /ko -->
    </div>
</div>
<!-- /ko -->


<script type="text/javascript">

    $(document).ready(function () {

        var options = {
            elementId: "<%= ClientID %>",
            editorType: <%=(int)ForumInfo.EditorType%>,
            moduleId: <%=ModuleId%>,
            forumId: <%=ForumId%>,
            attachmentsClientId: "<%=AttachmentsClientId %>",
            titleText: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:Attachments:BrowseSite:Title]"))%>",
            nameHeaderText: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:FileName]"))%>",
            typeHeaderText: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:FileType]"))%>",
            maxUploadSize: <%=ForumInfo.AttachMaxSize * 1024%>,
            lastModifiedHeaderText: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:LastModified]"))%>",
            fileSizeText: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:FileSize]"))%>",
            templatePath: "<%=Page.ResolveUrl("~/Resources/Shared/Components/UserFileManager/Templates/")%>",
            confirmRemoveText : "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:Attachments:Confirm:Remove]"))%>",
            cancelText : "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:Cancel]"))%>",
            attachText : "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:Attach]"))%>",
            serverErrorMessage: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:Attachments:Error]"))%>",
            fileTypeNotAllowedMsg: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:Error:BlockedFile]"))%>",      
            allowedFileTypes: "<%= ForumInfo.AttachTypeAllowed %>",
            allowBrowseSite: <%= ForumInfo.AttachAllowBrowseSite ? 1 : 0 %>,
            attachInsertAllowed: <%= ForumInfo.AttachInsertAllowed ? 1 : 0 %>,
            maxFileSizeExceededMsg: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(string.Format(LocalizeString("[RESX:Error:FileTooLarge]").TextOrEmpty(), ForumInfo.AttachMaxSize))%>",
            maxAttachmentCount: <%= ForumInfo.AttachCount %>,
            uploadButtonText: "<%=DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("[RESX:UploadButton]"))%>"
        };

        var attachmentMgr = new AFAttachmentManager(jQuery, ko, options);

        attachmentMgr.initialize();

    });

    
</script>
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_topicscripts.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Controls.af_topicscripts" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Components/ComposeMessage/ComposeMessage.js" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/Components/ComposeMessage/ComposeMessage.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Scripts/jquery/jquery.fileupload.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Components/UserFileManager/jquery.dnnUserFileUpload.js" Priority="102" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Components/Tokeninput/jquery.tokeninput.js" Priority="103" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/Components/Tokeninput/Themes/token-input-facebook.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/DesktopModules/activeforums/scripts/splitmanager.js" Priority="104" />


<script type="text/javascript">

    var postdivs, afgrid;
    function mozhackformaxwidth()
    {
        var shift = 0;
        if ($(window).width() > 768) shift = 120;
        var nwidth = afgrid.width() - shift;
        if (nwidth > 300) {
            postdivs.css("width", nwidth + "px");
        }
    }

    $(document).ready(function() {

        // Wire up core private message buttons
        $.fn.dnnComposeMessage({
            openTriggerSelector: ".afcontainer .ComposeMessage",
            onPrePopulate: function (target) {    
                var recipientJSON = $(target).attr('data-recipient');              
                var recipient = $.parseJSON(recipientJSON);
                var prePopulatedRecipients = [recipient];
                return prePopulatedRecipients;
            },
            servicesFramework: $.ServicesFramework(<%=ModuleId%>)
        });
        
        // Create the user editor
        $.fn.afUserEditor({
            openTriggerSelector: ".af-button-edituser",
            servicesFramework: $.ServicesFramework(<%=ModuleId%>)
        });

        // Split selected posts
        $.fn.afSplitManager({
            openTriggerSelector: ".af-button-split",
            servicesFramework: $.ServicesFramework(<%=ModuleId%>)
        });
        if ($.browser.mozilla) {
            postdivs = $(".afpostbody");
            afgrid = $("#afgrid");
            if (postdivs.length > 0) {
                mozhackformaxwidth();
                $(window).resize(function () {
                    mozhackformaxwidth();
                });
            }
        }
    });

</script>
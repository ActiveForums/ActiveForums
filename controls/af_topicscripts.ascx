<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_topicscripts.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Controls.af_topicscripts" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server" FilePath="~/Resources/Shared/Components/ComposeMessage/ComposeMessage.js" Priority="101" />
<dnn:DnnCssInclude ID="DnnCssInclude1" runat="server" FilePath="~/Resources/Shared/Components/ComposeMessage/ComposeMessage.css" />
<dnn:DnnJsInclude ID="DnnJsInclude3" runat="server" FilePath="~/Resources/Shared/Components/Tokeninput/jquery.tokeninput.js" Priority="103" />
<dnn:DnnCssInclude ID="DnnCssInclude4" runat="server" FilePath="~/Resources/Shared/Components/Tokeninput/Themes/token-input-facebook.css" />
<script type="text/javascript">

    // Wire up core private message buttons
    $(document).ready(function() {

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
    });

</script>
﻿<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_topicscripts.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Controls.af_topicscripts" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Components/ComposeMessage/ComposeMessage.js" Priority="101" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/Components/ComposeMessage/ComposeMessage.css" />
<dnn:DnnJsInclude runat="server" FilePath="~/Resources/Shared/Components/Tokeninput/jquery.tokeninput.js" Priority="103" />
<dnn:DnnCssInclude runat="server" FilePath="~/Resources/Shared/Components/Tokeninput/Themes/token-input-facebook.css" />

<script type="text/javascript">

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
    });

</script>
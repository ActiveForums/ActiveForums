<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_subscribe.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_subscribe" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<am:Callback ID="cbSubscribe" runat="server">
	<Content>
	<asp:checkbox id="chkSubscribe" cssclass="afnormal" runat="server" />
	</Content>
</am:Callback>
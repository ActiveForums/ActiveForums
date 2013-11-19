<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_topicrating.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_topicrating" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<am:Callback ID="cbRating" runat="server">
	<Content>
		<asp:placeholder ID="plhRating" runat="server" />
	</Content>
</am:Callback>
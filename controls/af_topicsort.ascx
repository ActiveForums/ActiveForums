<%@ Control Language="C#" AutoEventWireup="false" Codebehind="af_topicsort.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_topicsorter" %>
<asp:dropdownlist CssClass="afdropdown" id="drpSort" runat="server" AutoPostBack="true">
	<asp:ListItem Value="ASC">[RESX:TopicSortOldest]</asp:ListItem>
	<asp:ListItem Value="DESC">[RESX:TopicSortNewest]</asp:ListItem>
</asp:dropdownlist>
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ActiveForumViewerSettings.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.ActiveForumViewerSettings" %>
<table>
	<tr>
		<td class="Normal">Select Forum Instance:</td>
		<td><asp:DropDownList id="drpForumInstance" runat="server" AutoPostBack="True"></asp:DropDownList></td>
	</tr>
	<tr>
		<td class="Normal">Select Forum:</td>
		<td><asp:DropDownList id="drpForum" runat="server"></asp:DropDownList></td>
	</tr>
	<tr>
		<td class="Normal">Select Forum Group Template:</td>
		<td><asp:DropDownList id="drpForumViewTemplate" runat="server"></asp:DropDownList></td>
	</tr>
	<tr>
		<td class="Normal">Select Topics Template:</td>
		<td>
			<asp:DropDownList id="drpTopicsTemplate" runat="server"></asp:DropDownList></td>
	</tr>
	<tr>
		<td class="Normal">Select Topic Template:</td>
		<td>
			<asp:DropDownList id="drpTopicTemplate" runat="server"></asp:DropDownList></td>
	</tr>
</table>
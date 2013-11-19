<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_home.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_home_new" %>
<div class="amcpsubnav"><div class="amcplnkbtn">&nbsp;</div></div>
<div class="amcpbrdnav">[RESX:Dashboard]</div>
<div class="amcpcontrols">

<table>
	<tr>
		<td valign="top"></td>
		<td valign="top"  style="width:100%">
			<table cellpadding="0" cellspacing="0" class="dashOuter"  style="width:100%">
				<tr>
					<td class="dashOuterHeader">[RESX:RecentTopics]</td>
				</tr>
				<tr>
					<td><asp:Literal ID="litRecentTopics" runat="server" /></td>
				</tr>
			</table>
			<table cellpadding="0" cellspacing="0" class="dashOuter" style="width:100%">
				<tr>
					<td class="dashOuterHeader">[RESX:RecentMembers]</td>
				</tr>
				<tr>
					<td><asp:Literal ID="litRecentMembers" runat="server" /></td>
				</tr>
			</table>
			<table cellpadding="0" cellspacing="0" class="dashOuter" style="width:100%">
				<tr>
					<td class="dashOuterHeader">[RESX:TopForums]</td>
				</tr>
				<tr>
					<td><asp:Literal ID="litTopForums" runat="server" /></td>
				</tr>
			</table>

		</td>
		<td valign="top">
			<asp:Literal ID="litQuickLinks" runat="server" />
			<table cellpadding="0" cellspacing="0" class="dashOuter" style="width:200px">
				<tr>
					<td class="dashOuterHeader">[RESX:QuickStats]</td>
				</tr>
				<tr>
					<td><asp:Literal ID="litQuickStats" runat="server" /></td>
				</tr>
			</table>
			<table cellpadding="0" cellspacing="0" class="dashOuter" style="width:200px">
				<tr>
					<td class="dashOuterHeader">[RESX:TopMembers]</td>
				</tr>
				<tr>
					<td><asp:Literal ID="litTopMembers" runat="server" /></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</div>
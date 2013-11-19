<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_pollvote.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_pollvote" %>
	<table>
		<tr>
			<td class="afbold">[RESX:Poll:Question]:</td>
		</tr>
		<tr>
			<td class="afnormal">
				<asp:Label id="lblQuestion" runat="server" /></td>
		</tr>
		<tr>
			<td class="afbold">[RESX:Poll:Options]:</td>
		</tr>
		<tr>
			<td class="afnormal">
				<asp:RadioButtonList id="rdbtnOptions" runat="server" CssClass="afnormal" />
				<asp:CheckBoxList id="cblstOptions" runat="server" CssClass="afnormal" />
			</td>
		</tr>
		<tr>
			<td align="center"><asp:Button ID="btnVote" runat="server" Text="[RESX:SubmitVote]" CssClass="CommandButton" /></td>
		</tr>
	</table>
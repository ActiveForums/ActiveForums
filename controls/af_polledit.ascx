<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_polledit.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_polledit" %>
<table width="100%">
	<tr>
		<td>[RESX:Poll:Question]:</td>
		<td><asp:TextBox ID="txtPollQuestion" runat="server" CssClass="aftextbox" /></td>
		<td></td>
	</tr>
	<tr>
		<td>[RESX:Poll:Type]:</td>
		<td><asp:RadioButtonList id="rdPollType" CssClass="afnormal" runat="server" RepeatDirection="Horizontal"
				CellPadding="0" CellSpacing="0" Height="20px">
				<asp:ListItem Value="S" Selected="True">[RESX:Poll:SingleSelect]</asp:ListItem>
				<asp:ListItem Value="M">[RESX:Poll:MultipleSelect]</asp:ListItem>
			</asp:RadioButtonList></td>
		<td></td>
	</tr>
	<tr>
		<td valign="top">[RESX:Poll:Options]:</td>
		<td>
			<asp:TextBox ID="txtPollOptions" TextMode="MultiLine" Rows="4" Width="90%" Wrap="false" CssClass="aftextbox" runat="server"></asp:TextBox>
		</td>
	</tr>

</table>
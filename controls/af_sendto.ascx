<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_sendto.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_sendto" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<div class="afcrumb">[AF:LINK:FORUMMAIN] > [AF:LINK:FORUMGROUP] > [AF:LINK:FORUMNAME]</div>
<div style="text-align:center;padding-top:10px;">
<div style="width:350px;margin-left:auto;margin-right:auto;padding-top:5px;">
	<div class="afeditor">
		<table>
			<tr>
					<td class="afbold" style="text-align:left;">[RESX:RecipientName]:</td><td></td>
				</tr>
				<tr>
					<td><asp:TextBox ID="txtRecipName" runat="server" CssClass="aftextbox" /></td>
					<td><asp:RequiredFieldValidator ID="reqName" runat="server" ControlToValidate="txtRecipName" ValidationGroup="AFSEND" /></td>
				</tr>
				<tr>
					<td class="afbold" style="text-align:left;">[RESX:RecipientEmail]:</td><td></td>
				</tr>
				<tr>
					<td><asp:TextBox ID="txtRecipEmail" runat="server" CssClass="aftextbox" /></td>
					<td><asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtRecipEmail" ValidationGroup="AFSEND" /><asp:RegularExpressionValidator ID="regEmail" runat="server"  ControlToValidate="txtRecipEmail"  ValidationGroup="AFSEND" /></td>
				</tr>
				<tr>
					<td class="afbold" style="text-align:left;">[RESX:EmailSubject]:</td><td></td>
				</tr>
				<tr>
					<td><asp:TextBox ID="txtRecipSubject" runat="server" CssClass="aftextbox" /></td>
					<td><asp:RequiredFieldValidator ID="reqSubject" runat="server" ControlToValidate="txtRecipSubject" ValidationGroup="AFSEND" /></td>
				</tr>
				<tr>
					<td class="afbold" style="text-align:left;">[RESX:EmailMessage]:</td><td><asp:RequiredFieldValidator ID="reqMessage" runat="server" ControlToValidate="txtMessage" ValidationGroup="AFSEND" /></td>
				</tr>
				<tr>
					<td><asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Height="200" Width="300" CssClass="aftextbox" /></td><td></td>
				</tr>
				<tr>
					<td align="center">
					 <ul class="dnnActions dnnClear">
									<li><asp:LinkButton ID="btnSend" CssClass="dnnPrimaryAction" runat="server" Text="[RESX:Send]"  ValidationGroup="AFSEND" /></li>
									<li><asp:LinkButton ID="btnCancel" CssClass="dnnSecondaryAction" runat="server" Text="[RESX:Cancel]" CausesValidation="false" /></li>
								</ul>

					</td>
					<td></td>
				</tr>
			</table>
			</div>
		</div>
	</div>
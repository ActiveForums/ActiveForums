<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="profile_adminsettings.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.profile_adminsettings" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
	function amaf_saveAdmin(){
		var rp = document.getElementById('<%=txtRewardPoints.ClientID%>').value;
		var uc = document.getElementById('<%=txtUserCaption.ClientID%>').value;
		var ds = document.getElementById('<%=chkDisableSignature.ClientID%>').checked;
		var da = document.getElementById('<%=chkDisableAvatar.ClientID%>').checked;
		var tu = document.getElementById('<%=drpDefaultTrust.ClientID%>');
		tu = tu.options[tu.selectedIndex].value;
		var ma = document.getElementById('<%=chkMonitor.ClientID%>').checked;
		var du = document.getElementById('<%=chkDisableAttachments.ClientID%>').checked;
		<%=cbAdmin.ClientID%>.Callback('saveadmin',rp,uc,ds,da,tu,ma,du);

	};
	function amaf_cancelAdmin(){
		window.location.href = window.location.href;
	};
	function amaf_cbAdminComplete(){
		actmod.UI.ShowSuccess('[RESX:Actions:ProfileUpdated]');
		window.location.href = window.location.href;
	};
</script>
<table width="100%" cellspacing="0" cellpadding="1" border="0">
					 <tr id="adminrow1" runat="server">
						<td class="afbold" style="white-space:nowrap;">[RESX:RewardPoints]:</td>
						<td>
							<asp:TextBox id="txtRewardPoints" runat="server" CssClass="aftextbox" /></td>
						<td style="width:100%"></td>
					</tr>
					<tr>
						<td class="afbold" style="white-space:nowrap;">
							[RESX:UserCaption]:</td>
						<td>
							<asp:TextBox id="txtUserCaption" runat="server" CssClass="aftextbox" Width="150" /></td>
						<td></td>
					</tr>
					<tr>
						<td class="afbold" style="white-space:nowrap;">[RESX:DisableSignature]:</td>
						<td>
							<asp:CheckBox id="chkDisableSignature" runat="server" /></td>
						<td></td>
					</tr>
					<tr>
						<td class="afbold" style="white-space:nowrap;">[RESX:DisableAvatar]:</td>
						<td>
							<asp:CheckBox id="chkDisableAvatar" runat="server" /></td>
						<td></td>
					</tr>
					<tr>
						<td class="afbold" style="white-space:nowrap;">[RESX:TrustedUser]:</td>
						<td>
						   <asp:DropDownList ID="drpDefaultTrust" Width="100" runat="server"><asp:ListItem Value="0" Text="" /><asp:ListItem Value="-1" Text="[RESX:NotTrusted]" /><asp:ListItem Value="1" Text="[RESX:Trusted]" Selected="True" /></asp:DropDownList></td>
						<td></td>
					</tr>
					<tr>
						<td class="afbold">[RESX:MonitorActivity]:</td>
						<td>
							<asp:CheckBox id="chkMonitor" runat="server" /></td>
						<td></td>
					</tr>
					<tr>
						<td class="afbold" style="white-space:nowrap;">
							[RESX:DisableAttachments]:</td>
						<td>
							<asp:CheckBox id="chkDisableAttachments" runat="server" /></td>
						<td></td>
					</tr>
				 </table>
			   <div class="amtbwrapper" style="text-align:center;">
					<div style="margin-left:0 auto;margin-right:0 auto;min-width:50px;max-width:110px;">
					<am:imagebutton id="btnAdminSave" CssClass="amimagebutton" Height="50" Width="50" runat="server" PostBack="False" ClientSideScript="amaf_saveAdmin();" ImageLocation="TOP" text="[RESX:Button:Save]" ImageUrl="~/DesktopModules/ActiveForums/images/save32.png" />
					<am:ImageButton ID="btnAdminCancel"  CssClass="amimagebutton" Height="50" Width="50" runat="server" PostBack="false" ImageLocation="TOP" ClientSideScript="amaf_cancelAdmin();" Text="[RESX:Button:Cancel]" ImageUrl="~/DesktopModules/ActiveForums/images/cancel32.png" />
					</div>
				</div>
			<am:Callback ID="cbAdmin" runat="server" OnCallbackComplete="amaf_cbAdminComplete">
				<Content></Content>
			</am:Callback>
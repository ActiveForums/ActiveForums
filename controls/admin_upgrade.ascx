<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_upgrade.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_upgrade" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
	function upgradeStart() {
		document.getElementById('upStep1').style.display = 'none';
		document.getElementById('upStep2').style.display = '';
		<%=cbUpgrade.ClientID%>.Callback();
	};
</script>
<div class="amcpsubnav"><div class="amcplnkbtn">&nbsp;</div></div>
<div class="amcpbrdnav">&nbsp;</div>
<div class="amcpcontrols">
<am:Callback ID="cbUpgrade" runat="server">
	<Content><div id="upStep1" style="width:200px;margin-right:auto;margin-left:auto;border:solid 1px #ff0000;padding:15px;text-align:center;font-weight:bold;">Please click the button below to complete the upgrade process.<br /><input type="button" id="btnUpgrade" value="Click to Upgrade" onclick="upgradeStart();" /></div>
	<div id="upStep2" style="display:none;width:200px;margin-right:auto;margin-left:auto;text-align:center;font-weight:bold;">This may take several minutes.<br /><img src="~/desktopmodules/activeforums/images/spinner-lg.gif" runat="server" /></div>
	</Content>

</am:Callback>


</div>
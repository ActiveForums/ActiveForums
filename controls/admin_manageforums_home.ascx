<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_manageforums_home.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_manageforums_home" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
	function groupMove(groupId, dir) {
		af_showLoad();
		<%=cbGrid.ClientID%>.Callback('g',groupId, dir);
	};
	function forumMove(forumId, dir){
		af_showLoad();
		<%=cbGrid.ClientID%>.Callback('f',forumId, dir);
	};
	function moveComplete(){
		af_clearLoad();
	};
</script>
<div style="padding:0px;margin:0px;overflow:auto;">
	<div style="padding:10px;">
		<am:Callback ID="cbGrid" runat="server" OnCallbackComplete="moveComplete">
			<Content>
				<asp:Literal ID="litForums" runat="server" />           
			</Content>
		</am:Callback>

	</div>
</div>
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_manageforums.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_manageforums" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
	var currCtl;
	var currObj;
	var currGroup = 0;

	function af_refreshView(group,forum){
		LoadView('manageforums_forumeditor',currObj + '|' + currCtl)
	};
	function af_setCurrObj(obj,ctl){
		currObj = obj;
		currCtl = ctl;
	};
	function editorLoadComplete(){
		af_clearLoad();
	};

</script>
<div class="amcpsubnav"><asp:Literal ID="litButtons" runat="server" /></div>
<table cellpadding="0" cellspacing="0" width="100%">
	<tr>
		<td valign="top" style="border-left:solid 1px #ccc;">
		<div style="min-height:450px;">
			<am:Callback ID="cbForumEditor" runat="server" OnCallbackComplete="editorLoadComplete">
				<Content>
					<asp:PlaceHolder ID="plhForumEditor" runat="server" />
				</Content>
			</am:Callback>
		</div>
		</td>
   </tr>
</table>
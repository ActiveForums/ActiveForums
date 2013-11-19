<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ForumEditor.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Controls.ForumEditor" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnnweb" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<table>
	<tr>
		<td>
		<dnnweb:DnnTreeView ID="ctlForums" cssclass="dnnTreePages" runat="server" AllowNodeEditing="true"
		 EnableDragAndDropBetweenNodes="true" OnClientNodeClicked="OnClientNodeClicked">
			<ContextMenus>                
				<dnnweb:DnnTreeViewContextMenu ID="ctlContext" runat="server">
					<Items>                                            
						<dnnweb:DnnMenuItem Text="View" Value="view" />
						<dnnweb:DnnMenuItem Text="Edit" Value="edit" />
						<dnnweb:DnnMenuItem Text="Delete" Value="delete" />
						<dnnweb:DnnMenuItem Text="Add" Value="add" />
						<dnnweb:DnnMenuItem Text="Hide Page in Menu" Value="hide" />
						<dnnweb:DnnMenuItem Text="Show Page in Menu" Value="show" />
						<dnnweb:DnnMenuItem Text="Enable Page" Value="enable" />
						<dnnweb:DnnMenuItem Text="Disable Page" Value="disable" />
						<dnnweb:DnnMenuItem Text="Make Homepage" Value="makehome" />
					</Items>
				</dnnweb:DnnTreeViewContextMenu>               
			</ContextMenus>
		</dnnweb:DnnTreeView>
		</td>
		<td>
			<div class="dnnForm" id="forumTabs">
				<ul class="dnnAdminTabNav">
					<li><a href="#Main"><%=LocalizeString("Main")%></a></li>
					<li><a href="#Security"><%=LocalizeString("Security")%></a></li>
					<li><a href="#Features"><%=LocalizeString("Features")%></a></li>
				</ul>
				<div id="Main" class="dnnClear">
					<div class="dnnFormItem">
						<dnn:label resourcekey="ForumName" Text="ForumName" Suffix=":" runat="server" />
						<asp:TextBox ID="txtForumName" runat="server" />
					</div>
				</div>
				<div id="Security" class="dnnClear"></div>
				<div id="Features" class="dnnClear"></div>
			</div>
		</td>
	</tr>
</table>

<div class="dnnForm">
	<div class="dnnFormItem">

		<asp:dropdownlist ID="drpForums" runat="server" />
	</div>

</div>

<script type="text/javascript">
	jQuery(function ($) {
		$('#forumTabs').dnnTabs();
	});
	function OnClientNodeClicked(sender, eventArgs) {
		var nodeValue = eventArgs.get_node().get_value();
		console.log(nodeValue);
		//location.hash = "#" + $("input[type=radio][name$=rblMode]:checked").val() + "&" + nodeValue;
	}
</script>
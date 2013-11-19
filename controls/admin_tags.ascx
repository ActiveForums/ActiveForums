<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_tags.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_tags" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
var itemid = 0;
function renderDG(){
	<%=agTags.ClientID%>.Callback();
};
function amaf_openDialog(row){
	itemid = 0;
	var itemname;
	if (row != undefined){
		itemid = row.cells[0].firstChild.nodeValue;
		itemname = row.cells[1].firstChild.nodeValue;
		var txt = document.getElementById('<%=txtTag.ClientID%>');
		txt.value = itemname;
	};
};
function amaf_saveTag(){
	var objectName = document.getElementById('<%=txtTag.ClientID%>').value;
	if (objectName != ''){
		<%=agTags.ClientID%>.Params = 'save:' + objectName + ':' + itemid;
		itemid = 0;
		document.getElementById('<%=txtTag.ClientID%>').value = '';
		<%=agTags.ClientID%>.Callback();
	};
};
function amaf_deleteTag(row){
	if (confirm('[RESX:Actions:DeleteConfirm]')){
		var itemid;
		if (row != undefined){
			itemid = row.cells[0].firstChild.nodeValue;
		};

		<%=agTags.ClientID%>.Params = 'delete:' + itemid;
		<%=agTags.ClientID%>.Callback();
	};
 };
</script>
<div class="amcpsubnav"><div class="amcplnkbtn">&nbsp;</div></div>
<div class="amcpbrdnav">[RESX:Tags]</div>
<div class="amcpcontrols">
	<table>
		<tr>
			<td></td><td class="amcpbold">[RESX:TagName]:</td><td><asp:TextBox ID="txtTag" runat="server" CssClass="amcptxtbx" Font-Size="14px" Height="18" Width="125"/></td>
			<td><am:ImageButton ID="imgSave" runat="server" PostBack="false" ClientSideScript="amaf_saveTag();" CssClass="amsmallbtn" Text="[RESX:Save]" Height="18" Width="50" ImageLocation="LEFT" ImageUrl="~/DesktopModules/ActiveForums/images/save16.png" /></td>
			<td></td>
		</tr>
	</table>
	<div style="height:400px;overflow:auto;">
	<am:ActiveGrid ID="agTags" runat="server" DefaultColumn="TagName" PageSize="20000" ImagePath="~/DesktopModules/activeforums/images/">
		<HeaderTemplate><table cellpadding="2" cellspacing="0" border="0" class="amGrid" style="width:100%;">
					<tr><td ColumnName="TagId" style="display:none;width:0px;"></td><td class="amcptblhdr" ColumnName="TagName" style="height:16px;"><div class="amheadingcelltext">[RESX:TagName]</div></td><td class="amcptblhdr" ColumnName="Clicks" style="width:50px;height:16px;"><div class="amheadingcelltext">[RESX:TagClicks]</div></td><td class="amcptblhdr" ColumnName="Items" style="height:16px;white-space:nowrap;width:50px;"><div class="amheadingcelltext">[RESX:TagItems]</div></td><td class="amcptblhdr" style="height:16px;white-space:nowrap;width:30px;"><div class="amheadingcelltext">&nbsp;</div></td></tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr style="display:none;" class="amdatarow">
				<td style="display:none;">##DataItem('Tagid')##</td>
				<td class="amcpnormal" resize="true" onclick="amaf_openDialog(this.parentNode);">##DataItem('TagName')##</td>
				<td class="amcpnormal" onclick="amaf_openDialog(this.parentNode);" style="text-align:center;">##DataItem('Clicks')##</td>
				<td class="amcpnormal" onclick="openDialog(this.parentNode);" style="white-space:nowrap;text-align:center;">##DataItem('Items')##</td>
				<td class="amcpnormal" onclick="amaf_deleteTag(this.parentNode);">##DataItem('Tagid')##</td>
		   </tr>
	   </ItemTemplate>
		<FooterTemplate></table></FooterTemplate>
	</am:ActiveGrid>
	</div>
</div>
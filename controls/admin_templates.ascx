<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_templates.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_templates_new" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
function renderDG(){
}
function openDialog(row){
		var templateid;
		if (row != undefined){
			templateid = row.cells[0].firstChild.nodeValue;
		};
		LoadView("templates_edit",templateid);

};
</script>
<div class="amcpsubnav"><div onclick="openDialog();" class="amcplnkbtn">[RESX:AddTemplate]</div></div>
<div class="amcpbrdnav">[RESX:Templates]</div>
<div class="amcpcontrols">
	<am:ActiveGrid ID="agTemplates" runat="server" ImagePath="~/DesktopModules/ActiveForums/images/" PageSize="15">
		<HeaderTemplate><table cellpadding="2" cellspacing="0" border="0" class="amGrid" style="width:100%;">
					<tr><td ColumnName="TemplateId" style="display:none;width:0px;"></td><td class="amcptblhdr" ColumnName="Title" style="width:100px;height:16px;"><div class="amheadingcelltext">[RESX:TemplateType]</div></td><td class="amcptblhdr" ColumnName="Title" style="height:16px;"><div class="amheadingcelltext">[RESX:Title]</div></td><td class="amcptblhdr" ColumnName="DateCreated" style="height:16px;white-space:nowrap;width:120px;"><div class="amheadingcelltext">[RESX:DateCreated]</div></td></tr>
		</HeaderTemplate>
		<ItemTemplate><tr style="display:none;" class="amdatarow"><td style="display:none;">##DataItem('TemplateId')##</td><td class="amcpnormal" onclick="openDialog(this.parentNode);">##DataItem('TemplateType')##</td><td class="amcpnormal" resize="true" onclick="openDialog(this.parentNode);">##DataItem('Title')##</td><td class="amcpnormal" onclick="openDialog(this.parentNode);" style="white-space:nowrap;">##DataItem('DateCreated')##</td></ItemTemplate>
		<FooterTemplate></table></FooterTemplate>
	</am:ActiveGrid>

</div>
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_filters.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_filters_new" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
function renderDG(){
	<%=agFilters.ClientID%>.Callback();
};
var filterOptions = {};
filterOptions.width = "500";
filterOptions.height = "350";
filterOptions.modtitle = "[RESX:Filters]";

function amaf_openDialog(row){
	var filterId;
	if (row != undefined){
		filterId = row.cells[0].firstChild.nodeValue;
		var data = {};
		data.action = 11;
		data.FilterId = filterId;
		afadmin_callback(JSON.stringify(data), loadEdit);
	} else {
		filter.FilterId = -1;
		$('#txtFind').val('');
		$('#txtReplace').val('');
		$('#drpFilterType').val('-1');
		am.UI.LoadDiv('afFilterEdit', filterOptions);
	};
};
function amaf_deleteFilter(row){

		var filterid;
		if (row != undefined){
			filterid = row.cells[0].firstChild.nodeValue;
		};
		deleteFilter(filterid);


 };
function deleteFilter(id) {
    if (typeof(id) == 'undefined'){
        if (filter.FilterId == -1) {
            return;
        }else{
            id = filter.FilterId;
        }
    }
    if (confirm('[RESX:Actions:DeleteConfirm]')){
        var data = {};
        data.action = 13;
        data.FilterId = id;
        afadmin_callback(JSON.stringify(data));
        renderDG();
    }
};
 function amaf_loadDefaults(){
	if (confirm('[RESX:Actions:RestoreConfirm]')){
		<%=agFilters.ClientID%>.Params = 'defaults:0';
		<%=agFilters.ClientID%>.Callback();
	};
 };
 function loadEdit(data) {
	filter = data;

	$('#txtFind').val(decodeURIComponent(unescape(data.Find)).replace(/(-\|-)/gi,' '));
	$('#txtReplace').val(decodeURIComponent(unescape(data.Replacement)).replace(/(-\|-)/gi,' '));
	$('#drpFilterType').val(data.FilterType);
	am.UI.LoadDiv('afFilterEdit', filterOptions);
}
 var filter = {};
 filter.FilterId = -1;
 filter.Find = '';
 filter.Replacement = '';
 filter.FilterType = '';

function saveFilter() {
	var isvalid = true;
	filter.action = 12;
	filter.Find = $('#txtFind').val();
	if (filter.Find == '') {
		isvalid = false;
	}
	filter.Find = encodeURIComponent(filter.Find);
	filter.Replacement = $('#txtReplace').val();
	if (filter.Replacement == '') {
		isvalid = false;
	}
	filter.Replacement = encodeURIComponent(filter.Replacement);
	filter.FilterType = $('#drpFilterType').val();

	if (isvalid) {
		am.UI.CloseDiv('afFilterEdit');
		afadmin_callback(JSON.stringify(filter), renderDG);
	}
}
</script>
<div class="amcpsubnav"><div onclick="amaf_openDialog();" class="amcplnkbtn">[RESX:AddFilter]</div><div onclick="amaf_loadDefaults();" class="amcplnkbtn">[RESX:RestoreDefaults]</div></div>
<div class="amcpbrdnav">[RESX:Filters]</div>
<div class="amcpcontrols">
	<am:ActiveGrid ID="agFilters" runat="server" DefaultColumn="FilterType" PageSize="15" ImagePath="~/DesktopModules/activeforums/images/">
		<HeaderTemplate><table cellpadding="2" cellspacing="0" border="0" class="amGrid" style="width:100%;">
					<tr><td ColumnName="FilterId" style="display:none;width:0px;"></td><td class="amcptblhdr" ColumnName="Find" style="width:100px;height:16px;"><div class="amheadingcelltext">[RESX:Find]</div></td><td class="amcptblhdr" ColumnName="Replace" style="height:16px;"><div class="amheadingcelltext">[RESX:Replace]</div></td><td class="amcptblhdr" ColumnName="FilterType" style="height:16px;white-space:nowrap;width:120px;"><div class="amheadingcelltext">[RESX:FilterType]</div></td><td class="amcptblhdr" style="height:16px;white-space:nowrap;width:30px;"><div class="amheadingcelltext">&nbsp;</div></td></tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr style="display:none;" class="amdatarow">
				<td style="display:none;">##DataItem('FilterId')##</td>
				<td class="amcpnormal" onclick="amaf_openDialog(this.parentNode);">##DataItem('Find')##</td>
				<td class="amcpnormal" resize="true" onclick="amaf_openDialog(this.parentNode);">##DataItem('Replace')##</td>
				<td class="amcpnormal" onclick="openDialog(this.parentNode);" style="white-space:nowrap;">##DataItem('FilterType')##</td>
				<td class="amcpnormal" onclick="amaf_deleteFilter(this.parentNode);">##DataItem('FilterId')##</td>
		   </tr>
	   </ItemTemplate>
		<FooterTemplate></table></FooterTemplate>
	</am:ActiveGrid>
</div>

<div id="afFilterEdit" style="width:500px;height:350px;display:none;" title="[RESX:Filters]">
	<div class="dnnForm">
		<div class="dnnFormItem">
			<label>
				[RESX:Find]:</label>
				<input type="text" id="txtFind" class="dnnFormRequired" />

		</div>
		<div class="dnnFormItem">
			<label>
				[RESX:Replace]:</label>
				<input type="text" id="txtReplace" class="dnnFormRequired" />

		</div>
		<div class="dnnFormItem">
			<label>
				[RESX:FilterType]:</label>
			<select id="drpFilterType">
				<option value="MARKUP">[RESX:MarkUp]</option>
				<option value="EMOTICON">[RESX:Emoticon]</option>
				<option value="REGEX">[RESX:RegEx]</option>
			</select>
		</div>
		<ul class="dnnActions dnnClear">
			<li><a href="#" onclick="saveFilter(); return false;" class="dnnPrimaryAction">[RESX:Button:Save]</a></li>
			<li><a href="#" onclick="deleteFilter(); return false;" class="confirm dnnSecondaryAction">[RESX:Button:Delete]</a></li>
			<li><a href="#" onclick="am.UI.CloseDiv('afFilterEdit'); return false;" class="dnnSecondaryAction">
				[RESX:Button:Cancel]</a></li>
		</ul>
	</div>
</div>
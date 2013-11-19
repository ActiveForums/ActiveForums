<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_ranks.ascx.cs"
	Inherits="DotNetNuke.Modules.ActiveForums.admin_ranks_new" %>
<%@ Register Assembly="DotNetNuke.Modules.ActiveForums" Namespace="DotNetNuke.Modules.ActiveForums.Controls"
	TagPrefix="am" %>
<script type="text/javascript">
function renderDG(){
	<%=agRanks.ClientID%>.Callback();
};
var rankOptions = {};
rankOptions.width = "500";
rankOptions.height = "350";
rankOptions.modtitle = "[RESX:RanksRecognition]";
function openDialog(row){
	var rankid;
	if (row != undefined){
		rankid = row.cells[0].firstChild.nodeValue;
		var data = {};
		data.action = 8;
		data.RankId = rankid;
		afadmin_callback(JSON.stringify(data), loadEdit);
	} else {
		rank.RankId = -1;
		$('#txtRankName').val('');
		$('#txtMinPoints').val('');
		$('#txtMaxPoints').val('');
		$('#<%=drpRankImages.ClientID%>').val('-1');
		am.UI.LoadDiv('afRankEdit', rankOptions);
	};
};

function loadEdit(data) {
	rank = data;
	$('#txtRankName').val(data.RankName);
	$('#txtMinPoints').val(data.MinPosts);
	$('#txtMaxPoints').val(data.MaxPosts);
	$('#<%=drpRankImages.ClientID%>').val(data.Display);
	am.UI.LoadDiv('afRankEdit', rankOptions);
}
var rank = {};
rank.RankId = -1;
rank.RankName = '';
rank.Display = '';
rank.MinPosts = 0;
rank.MaxPosts = 0;
function saveRank() {
	var isvalid = true;
	rank.action = 9;
	rank.RankName = $('#txtRankName').val();
	if (rank.RankName == '') {
		isvalid = false;
	}
	rank.MinPosts = $('#txtMinPoints').val();
	rank.MaxPosts = $('#txtMaxPoints').val();
	if (rank.MinPosts == '') {
		isvalid = false;
	}
	if (rank.MaxPosts == '') {
		isvalid = false;
	}
	if (isvalid) {
		rank.MaxPosts = parseInt(rank.MaxPosts);
		rank.MinPosts = parseInt(rank.MinPosts);
	}
	if (rank.MinPosts < 0 || rank.MaxPosts < 0) {
		isvalid = false;
	}


	if (rank.MinPosts >= rank.MaxPosts) {
		isvalid = false;
	}
	if (rank.MaxPosts <= rank.MinPosts) {
		isvalid = false;
	}


	rank.Display = $('#<%=drpRankImages.ClientID%>').val();
	if (rank.Display == -1) {
		isvalid = false;
	}
	if (isvalid) {
		am.UI.CloseDiv('afRankEdit');
		afadmin_callback(JSON.stringify(rank), renderDG);
	}
}



</script>
<div class="amcpsubnav">
	<div onclick="openDialog();" class="amcplnkbtn">
		[RESX:RanksNew]</div>
</div>
<div class="amcpbrdnav">
	[RESX:RanksRecognition]</div>
<div class="amcpcontrols">
	<am:ActiveGrid ID="agRanks" runat="server" ImagePath="~/DesktopModules/activeforums/images/">
		<headertemplate><table cellpadding="2" cellspacing="0" border="0" class="amGrid" style="width:100%;">
								<tr><td ColumnName="RankId" style="display:none;width:0px;"></td><td class="amcptblhdr" ColumnName="RankName" style="height:16px;"><div class="amheadingcelltext">[RESX:RankName]</div></td><td class="amcptblhdr" ColumnName="MinPosts" style="width:120px;height:16px;"><div class="amheadingcelltext">[RESX:MinPoints]</div></td><td class="amcptblhdr" ColumnName="MaxPosts" style="height:16px;white-space:nowrap;width:120px;"><div class="amheadingcelltext">[RESX:MaxPoints]</div></td><td class="amcptblhdr" ColumnName="Display" style="height:16px;white-space:nowrap;width:120px;"><div class="amheadingcelltext">[RESX:Display]</div></td></tr>
					</headertemplate>
		<itemtemplate><tr style="display:none;" class="amdatarow"><td style="display:none;">##DataItem('RankId')##</td><td class="amcpnormal" resize="true" onclick="openDialog(this.parentNode);">##DataItem('RankName')##</td><td class="amcpnormal" onclick="openDialog(this.parentNode);">##DataItem('MinPosts')##</td><td class="amcpnormal" onclick="openDialog(this.parentNode);" style="white-space:nowrap;">##DataItem('MaxPosts')##</td><td class="amcpnormal" onclick="openDialog(this.parentNode);" style="white-space:nowrap;">##DataItem('Display')##</td></tr></itemtemplate>
		<footertemplate></table></footertemplate>
	</am:ActiveGrid>
</div>
<div id="afRankEdit" style="width:500px;height:350px;display:none;" title="[RESX:RanksRecognition]">
	<div class="dnnForm">
		<div class="dnnFormItem">
			<label>
				[RESX:RankName]:</label>
				<input type="text" id="txtRankName" class="dnnFormRequired" />

		</div>
		<div class="dnnFormItem">
			<label>
				[RESX:MinPoints]:</label>
				<input type="text" id="txtMinPoints" class="dnnFormRequired" onkeypress="return onlyNumbers(event);" width="50" />

		</div>
		<div class="dnnFormItem">
			<label>
				[RESX:MaxPoints]:</label>
		   <input type="text" id="txtMaxPoints" class="dnnFormRequired" onkeypress="return onlyNumbers(event);" width="50" />

		</div>
		<div class="dnnFormItem">
			<label>
				[RESX:Display]:</label>
			<asp:DropDownList ID="drpRankImages" runat="server" Width="150" />
		</div>
		<ul class="dnnActions dnnClear">
			<li><a href="#" onclick="saveRank(); return false;" class="dnnPrimaryAction">[RESX:Button:Save]</a></li>
			<li><a href="#" class="confirm dnnSecondaryAction">[RESX:Button:Delete]</a></li>
			<li><a href="#" onclick="am.UI.CloseDiv('afRankEdit'); return false;" class="dnnSecondaryAction">
				[RESX:Button:Cancel]</a></li>
		</ul>
	</div>
</div>
<script type="text/javascript">
	jQuery(function ($) {
		var opts = {};
		opts.callbackTrue = function () {
			$(this).dialog("close");
			var data = {};
			data.action = 10;
			data.RankId = rank.RankId;
			am.UI.CloseDiv('afRankEdit');
			afadmin_callback(JSON.stringify(data), renderDG);
		};
		$('#afRankEdit .confirm').dnnConfirm(opts);
	});
</script>
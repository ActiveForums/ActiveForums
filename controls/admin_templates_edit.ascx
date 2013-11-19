<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_templates_edit.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_templates_edit" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
var currAction;
function saveTemplate(){
	currAction = 'save';
	var templateid = document.getElementById("<%=hidTemplateId.ClientID%>").value;
	var title = document.getElementById("<%=txtTitle.ClientID%>").value;
	var subject = document.getElementById("<%=txtSubject.ClientID%>").value;
	var plaintext = document.getElementById("<%=txtPlainText.ClientID%>").value;
	var htmltext = document.getElementById("<%=txtEditor.ClientID%>").value;
	var templatetype = document.getElementById("<%=drpTemplateType.ClientID%>");
	templatetype = templatetype.options[templatetype.selectedIndex].value;
	if (trim(title) == '' || trim(subject) == ''){
		amcp.UI.ShowWarn('[RESX:Actions:RequiredFields]');
		return false;
	};
	<%=cbAction.ClientID%>.Callback('save',templateid,title,subject,escape(htmltext),plaintext,templatetype);
};
function deleteTemplate(){
	currAction = 'delete';
	var templateid = document.getElementById("<%=hidTemplateId.ClientID%>").value;
	if (templateid != ''){
		<%=cbAction.ClientID%>.Callback('delete', templateid);
	};
};
function toggleTextTab(){
	var templatetype = document.getElementById("<%=drpTemplateType.ClientID%>");
	templatetype = templatetype.options[templatetype.selectedIndex].text;
	if (templatetype == 'Email' || templatetype == 'ModEmail' || templatetype=='Mail Connector'){
		document.getElementById('divTEXT').style.display = '';
	}else{
	   document.getElementById('divTEXT').style.display = 'none';
	};
};
function actionComplete(){
	var cbActionMessage = document.getElementById("<%=cbActionMessage.ClientID%>");
	switch(currAction){
		case 'save':
			if (cbActionMessage.innerHTML != ''){
				amcp.UI.ShowSuccess(cbActionMessage.innerHTML);
			};
			currAction = '';
			var templateid = document.getElementById("<%=hidTemplateId.ClientID%>").value;
			if (templateid == ''){
				LoadView('templates');
			};
			break;
		case 'delete':
			if (cbActionMessage.innerHTML != ''){
				amcp.UI.ShowSuccess(cbActionMessage.innerHTML);
			};
			currAction = '';
			LoadView('templates');
			break;
		default:
	};

};

</script>
<div class="amcpsubnav"><div class="amcplnkbtn">&nbsp;</div></div>
<div class="amcpbrdnav"><span class="amcpbrdnavitem" onclick="LoadView('templates');">[RESX:Templates]</span> > [RESX:Details]</div>
<div class="amcpcontrolstab" style="background-color:#fff;border-top:solid 1px #666;">

<table width="98%" style="background-color:#fff;">
	<tr>
		<td class="amcpbold">[RESX:Title]:<img id="reqTitle" align="absmiddle" hspace="5" src="~/DesktopModules/activeforums/images/error.gif" runat="server"  onmouseover="amShowTip(this,'Title is required');" onmouseout="amHideTip();" height="16" width="16" /></td><td class="amcpbold">[RESX:Subject]:<img id="reqSubject"  align="absmiddle" hspace="5" src="~/DesktopModules/activeforums/images/error.gif" runat="server"  onmouseover="amShowTip(this,'Subject is required');" onmouseout="amHideTip();" height="16" width="16" /></td><td class="amcpbold">[RESX:TemplateType]:<img id="Img1" src="~/DesktopModules/activeforums/images/error.gif" runat="server"  onmouseover="amShowTip(this,'Template Type is Required');" onmouseout="amHideTip();" height="16" width="16" align="absmiddle" hspace="5"  /></td>
	</tr>
	<tr>
		<td><asp:TextBox ID="txtTitle" runat="server" CssClass="amcptxtbx" /></td><td><asp:TextBox ID="txtSubject" runat="server" CssClass="amcptxtbx" /></td><td><asp:DropDownList ID="drpTemplateType" runat="server" CssClass="amcptxtbx" /></td>
	</tr>
</table>
<div id="divHTML" onclick="toggleTab(this);" class="amtabsel" style="margin-left:10px;"><div id="divHTML_text" class="amtabseltext">[RESX:HTML]</div></div><div id="divTEXT" onclick="toggleTab(this);" class="amtab" style="display:none;"><div id="divTEXT_text" class="amtabtext">[RESX:PlainText]</div></div>
	<div class="amtabcontent" id="amTabContent" style="height:auto;">
		<div id="divHTML_afcontent" style="display:block;width:98%">
			<asp:TextBox ID="txtEditor" runat="server" CssClass="amcptxtbx" Width="99%" Height="340" TextMode="MultiLine" />
		</div>
		<div id="divTEXT_afcontent" style="display:none;width:98%">
			<asp:TextBox ID="txtPlainText" runat="server" CssClass="amcptxtbx" Width="99%" Height="340" TextMode="MultiLine" />
		</div>
	</div>
	<asp:HiddenField ID="hidTemplateId" runat="server" />
<am:callback ID="cbAction" runat="server" OnCallbackComplete="actionComplete">
	<Content><div id="cbActionMessage" runat="server" style="display:none;"></div></Content>
</am:callback>
<div class="amtbwrapper">
	<div class="amcpmdtoolbarbtm" style="width:175px;">
			<am:imagebutton id="btnSave" runat="server" Height="50" width="50" PostBack="False" ClientSideScript="saveTemplate();" ImageLocation="TOP" text="[RESX:Button:Save]" ImageUrl="~/DesktopModules/ActiveForums/images/save32.png" />
			<am:ImageButton ID="btnDelete" Height="50" width="50"  Confirm="true" ConfirmMessage="[RESX:Actions:DeleteConfirm]" ImageLocation="TOP" runat="server" PostBack="false" ClientSideScript="deleteTemplate();" Text="[RESX:Button:Delete]" ImageUrl="~/DesktopModules/ActiveForums/images/delete32.png" />
			<am:ImageButton ID="btnClose" Height="50" width="50"  runat="server" PostBack="false" ImageLocation="TOP" ClientSideScript="LoadView('templates');" Text="[RESX:Button:Cancel]" ImageUrl="~/DesktopModules/ActiveForums/images/cancel32.png" />

		</div>
	</div>
</div>

<script>toggleTextTab();</script>
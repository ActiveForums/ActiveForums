<%@ Control Language="C#" AutoEventWireup="false" Codebehind="af_quickreply.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_quickreplyform" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<script type="text/javascript">
<!--
	var isSubmitted = false;
function insertCode(sText) {
		var newMessage;
		var strMessage = document.getElementById('txtBody').value;
		newMessage = strMessage+sText;
		document.getElementById('txtBody').value=newMessage;
		document.getElementById('txtBody').focus();
		return;
	};
function nothing () {
		return;
	};	


function insertQuote() {
	var txt = "";
	if (document.getSelection) txt = document.getSelection();
	else if (document.selection && document.selection.createRange) txt = document.selection.createRange().text;
	else return;
	if (txt != "")	{
		var s = new String();
		s += "[quote]";
		s += txt + "[/quote]";
		insertCode(s);	
	}else
		insertCode('[quote] [/quote]');
};
function afQuickSubmit() {
	if (isSubmitted == false) {
		isSubmitted = true;
		var hid = document.getElementById('hidReply1');
		hid.value = 'true';
		document.forms[0].submit();
	}
};
//-->
</script>

<table class="afgrid">
	<tr>
		<td class="afgrouprow"><div class="afcontrolheader">[RESX:QuickReply]</div></td>
		<td class="afgrouprow" align="right" style="text-align:right;padding-right:10px;"><img align="absmiddle" class="afarrow" id="imgGroupQR" alt="toggle" onclick="toggleGroup('QR');" src="<%=ImagePath + "/images/arrows_down.png"%>" /></td>
	</tr>
	<tr>
		<td colspan="2" class="afborder">
		<div id="groupQR" <%=DisplayMode%>><asp:PlaceHolder ID="plhMessage" runat="server" />
			<table width="100%"  cellspacing="0" cellpadding="4">
				<tr id="trUsername" runat="server">
					<td><asp:RequiredFieldValidator ID="reqUserName" runat="server" ControlToValidate="txtUserName" Enabled="false" /></td>
					<td class="NormalBold" style="white-space:nowrap">[RESX:Username]:</td>
					<td>
						<input type="text" id="txtUserName" class="aftextbox" maxlength="25" runat="server" width="200" /></td>
					<td></td>
				</tr>
				<tr>
					<td></td>
					<td class="NormalBold">[RESX:Subject]:</td>
					<td><input type="text" id="txtSubject" class="aftextbox" readonly="readonly" value="<%=Subject%>" /></td>
					<td></td>
				</tr>
				<tr>
					<td valign="top"><asp:Label ID="reqBody" runat="server" Visible="false" /></td>
					<td valign="top" class="NormalBold">[RESX:Body]:</td>
					<td width="100%"><div id="btnToolBar" runat="server">
						<input type="button" class="afButton" accesskey="b" name="afBold" value="<%=BoldText%>" style="font-weight:bold;" onclick="insertCode('[b] [/b]');" onmouseover="window.status='<%=BoldDesc%>';  return true;" onmouseout="window.status=''; return true;" />
						<input type="button" class="afButton" accesskey="i" name="afBold" value="<%=ItalicsText%>"  style="font-weight:bold;" onclick="insertCode('[i] [/i]');" onmouseover="window.status='<%=ItalicsDesc%>';  return true;" onmouseout="window.status=''; return true;" />
						<input type="button" class="afButton" accesskey="u" name="afBold" value="<%=UnderlineText%>" style="font-weight:bold;" onclick="insertCode('[u] [/u]');" onmouseover="window.status='<%=UnderlineDesc%>';  return true;" onmouseout="window.status=''; return true;" />
						<input type="button" class="afButton" accesskey="q" name="afBold" value="<%=QuoteText%>" style="font-weight:bold;" onclick="insertQuote();" onmouseover="window.status='<%=QuoteDesc%>';  return true;" onmouseout="window.status=''; return true;" />
						<input type="button" class="afButton" accesskey="m" name="afBold" value="<%=ImageText%>" style="font-weight:bold;" onclick="insertCode('[img] [/img]');" onmouseover="window.status='<%=ImageDesc%>';  return true;" onmouseout="window.status=''; return true;" />
						<input type="button" class="afButton" accesskey="c" name="afBold" value="<%=CodeText%>" style="font-weight:bold;" onclick="insertCode('[code] [/code]');" onmouseover="window.status='<%=CodeDesc%>';  return true;" title='<%=CodeDesc%>' onmouseout="window.status=''; return true;" />
						</div>
						<textarea id="txtBody" name="txtBody" class="aftextbox" style="height:120px" rows="5" cols="250"></textarea></td>
					<td valign="top">
					</td>
				</tr>
				<tr id="trCaptcha" runat="server">
					<td valign="top"><asp:Label ID="reqSecurityCode" runat="server" Visible="false" /></td>
					<td class="NormalBold" valign="top" style="white-space:nowrap">[RESX:SecurityCode]:</td>
					<td>
					<dnn:captchacontrol  id="ctlCaptcha" captchawidth="130" captchaheight="40" cssclass="Normal" runat="server" errorstyle-cssclass="NormalRed"  />
					</td>
					<td></td>
				</tr>
				 <tr id="trSubscribe" runat="server">
					<td></td>
					<td class="NormalBold">[RESX:Subscribe]:</td>
					<td class="afcheckbox"><input type="checkbox" id="chkSubscribe" name="chkSubscribe" value="1" <%=SubscribedChecked%> />[RESX:Subscribe:Note]</td>
					<td></td>
				</tr>
				<tr>
					<td align="center" colspan="3">
					<div class="amtbwrapper" style="text-align:center;">
						<div style="margin:0px auto;min-width:50px;max-width:60px;">
							<asp:linkbutton ID="btnSubmitLink" runat="server" CssClass="dnnPrimaryAction">[RESX:Submit]</asp:linkbutton>
						</div>   
					</div>

					</td>
				</tr>
			</table></div>
		</td>
	</tr>

</table>
<input type="hidden" name="hidReply1" id="hidReply1" value="" />
<input type="hidden" name="hidReply2" id="hidReply2" value="" />
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_usersonline.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_usersonline" %>
<table class="afgrid" id="tblWhosOnline" cellspacing="0" cellpadding="4" width="100%">
	<tr>
		<td class="afgrouprow"><div class="afcontrolheader">[RESX:WhosOnline]</div></td>
		<td align="right" class="afgrouprow" style="text-align:right;padding-right:10px;"><img class="afarrow" id="imgGroupWHOS"  onclick="toggleGroup('WHOS');" src="<%=ImagePath + "/images/arrows_down.png"%>" alt="-" /></td>
	</tr>
	<tr>
		<td colspan="2" class="afborder">
		<div class="afnormal" id="groupWHOS" <%=DisplayMode%>>

				<asp:Literal ID="litGuestsOnline" runat="server" /><br />
				<div id="af-usersonline">
				<asp:literal id="litUsersOnline" runat="server">There are [USERCOUNT] of [TOTALMEMBERCOUNT] member(s) online:</asp:literal>&nbsp;
				</div>

					<br />


		</div>
		</td>
	</tr>
</table>
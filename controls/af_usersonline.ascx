<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_usersonline.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_usersonline" %>
<div class="afgrid">
<div class="afgrid-inner">

<div class="afgroupsection afgroupsectiononline" id="tblWhosOnline">
	
	<tr>
		<td class="afgrouprow af-groupname">
			<div class="afgroupsectiontitle">Who's Online</div>
			<img class="afarrow" id="imgGroupWHOS" onclick="toggleGroup('WHOS');" src="<%=ImagePath + "/images/arrows_down.png"%>" alt="-" />
		</td>
	</tr>
	
	<tr>
		<td colspan="2">
		
			<div class="afgroup afgrouponline" id="groupWHOS" <%=DisplayMode%>>
				
				<asp:Literal ID="litGuestsOnline" runat="server" /><br />
				<div id="af-usersonline">
				<asp:literal id="litUsersOnline" runat="server">There are [USERCOUNT] of [TOTALMEMBERCOUNT] member(s) online:</asp:literal>&nbsp;
				</div>
	
				<br />
			
			</div>
			
		</td>
	</tr>
</div>

</div>
</div>
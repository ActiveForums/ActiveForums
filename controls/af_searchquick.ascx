<%@ Control Language="C#" AutoEventWireup="false" Codebehind="af_searchquick.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_searchquick" %>
<table id="tblSearch" cellspacing="0" cellpadding="0" border="0" runat="server">
	<tr>
		<td><asp:Label id="lblSearch" runat="server" resourcekey="SearchCaption" CssClass="afnormal">Search this forum:</asp:Label></td>
		<td><asp:TextBox id="txtSearch" runat="server" CssClass="afminisearchbox" /></td>
		<td><div class="afsearchgo"><asp:LinkButton id="lnkSearch" runat="server"><img src="<%=ImagePath%>/images/search.gif" border="0" alt="[RESX:Search]" /></asp:LinkButton></div></td>
	</tr>
</table>
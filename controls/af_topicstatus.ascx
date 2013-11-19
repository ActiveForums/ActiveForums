<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_topicstatus.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_topicstatus" %>
<asp:DropDownList id="drpStatus" runat="server" class="afsmalltext">
	<asp:ListItem Value="-1">[RESX:NoStatus]</asp:ListItem>
	<asp:ListItem Value="0">[RESX:Informative]</asp:ListItem>
	<asp:ListItem Value="1">[RESX:NotResolved]</asp:ListItem>
	<asp:ListItem Value="3">[RESX:Resolved]</asp:ListItem>
</asp:DropDownList>
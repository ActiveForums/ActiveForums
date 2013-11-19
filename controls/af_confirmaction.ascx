<%@ Control Language="C#" AutoEventWireup="false" Codebehind="af_confirmaction.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_confirmaction_new" %>
<center>
<DIV align="center" style="width:350px;">
	<TABLE width="350" align="center" class="AFGrid">
		<tr>
			<td colspan="2" class="afgrouprow">
				<asp:Label id="lblTitle" runat="server" resourcekey="Title"></asp:Label></td>
		</tr>
		<TR>
			<TD class="Normal" colSpan="2"><br>
				<asp:Label id="lblMessage" Runat="server"></asp:Label><br><br></TD>
		</TR>
		<TR>
			<TD class="Normal" align="center">
				<asp:HyperLink id="hypForums" runat="server" resourcekey="Forum" CssClass="CommandButton"></asp:HyperLink></TD>
			<TD class="Normal" align="center">
				<asp:HyperLink id="hypPost" runat="server" resourcekey="Topic" CssClass="CommandButton"></asp:HyperLink></TD>
		</TR>
		<TR>
			<TD class="Normal" align="center" colSpan="2">
				<asp:HyperLink id="hypHome" runat="server" resourcekey="Home" CssClass="CommandButton"></asp:HyperLink></TD>
		</TR>
	</TABLE>
</DIV>
</center>
<%@ Control Language="C#" AutoEventWireup="false" Codebehind="af_modreport.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_modreport" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<div class="afcrumb">[AF:LINK:FORUMMAIN] > [AF:LINK:FORUMGROUP] > [AF:LINK:FORUMNAME]</div>
<div class="aftitlelg">[RESX:ReportContent]</div>
<div style="text-align:center;padding-top:10px;">
	<div style="width:450px;margin-left:auto;margin-right:auto;padding-top:5px;">
		 <div class="afeditor">
				<table>
					<tr>
						<td class="afbold" style="text-align:left;">[RESX:Reason]:</td>
						<td style="text-align:left;">
							<asp:DropDownList id="drpReasons" CssClass="afdropdown" runat="server"></asp:DropDownList></td>
						<td></td>
					</tr>
					<tr>
						<td valign="top" class="afbold" style="text-align:left;">[RESX:Comments]:</td>
						<td>
							<asp:TextBox id="txtComments" runat="server" CssClass="aftextbox" Width="344px" Height="160px"
								TextMode="MultiLine"></asp:TextBox></td>
						<td></td>
					</tr>
					<tr>
					<td align="center" colspan="2">
								<ul class="dnnActions dnnClear">
									<li><asp:LinkButton ID="btnSend" CssClass="dnnPrimaryAction" runat="server" Text="[RESX:Send]" /></li>
									<li><asp:LinkButton ID="btnCancel" CssClass="dnnSecondaryAction" runat="server" Text="[RESX:Cancel]" /></li>
								</ul>






						</div>
					</td>
					<td></td>
					</tr>
				</table>
		   </div>
	  </div>			
</div>
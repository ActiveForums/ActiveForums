<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_search.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_search_new" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>

<table cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td><asp:Label ID="lblHeader" CssClass="aftitlelg" runat="server" Text="[RESX:Search]" /></td>
		<td style="width:200px;text-align:right;"><asp:TextBox id="txtSearch" runat="server" CssClass="afsearchbox" Width="200" /></td>
		<td><span class="ambtnwrap" style="display:block;width:65px;"><div class="ambuttonlg"><asp:LinkButton ID="lnkSearch" runat="server" /></div></span></td>
	</tr>
</table>
<div id="divSearch" style="position:static;">
<table cellspacing="0" cellpadding="4" width="100%">
	<tr>
		<td valign="top" width="200">
		<asp:Label ID="lblOptions" runat="server" CssClass="afsmallbold" />
		<fieldset class="affieldset">
			<legend class="aflegend"><asp:Literal ID="litSearchColumns" runat="server" /></legend>
			<asp:DropDownList CssClass="aftextbox" ID="drpSearchColumns" runat="server">
				<asp:ListItem Text="Subject & Topic" Value="0" />
				<asp:ListItem Text="Subject Only" Value="1" />
				<asp:ListItem Text="Topic Only" Value="2" />
			 </asp:DropDownList>
		</fieldset>
		<fieldset class="affieldset">
			<legend class="aflegend"><asp:Literal ID="litSearchType" runat="server" /></legend>
			<asp:DropDownList CssClass="aftextbox" ID="drpSearchType" runat="server">
				<asp:ListItem Text="ANY Keyword" Value="0" />
				<asp:ListItem Text="ALL Keywords" Value="1" />
				<asp:ListItem Text="Exact Match" Value="2" />
			 </asp:DropDownList>
		</fieldset>
		<fieldset class="affieldset">
			<legend class="aflegend"><asp:literal ID="litSearchTimeFrame" runat="server" /></legend>
			<asp:literal ID="litSearchFromText" runat="server" />
				<asp:DropDownList CssClass="aftextbox" ID="drpSearchDays" runat="server" />
		</fieldset>
		<fieldset class="affieldset">
			<legend class="aflegend"><asp:literal ID="litSearchUserText" runat="server" /></legend>
			<asp:TextBox ID="txtUserName" runat="server" CssClass="aftextbox" />
		</fieldset>
		<fieldset class="affieldset">
			<legend class="aflegend"><asp:Literal ID="litSearchForums" runat="server" /></legend>
			<am:MenuButton ID="mnShowForums" runat="server" Text="[RESX:SearchForumsAll]" CssClass="afmenuclick" MenuCss="afmenu" MenuHeight="300" MenuWidth="180" MenuOverflow="auto">
				<menu><asp:TreeView ID="trForums" runat="server" onclick="af_OnTreeClick(event);" ShowCheckBoxes="All" /></menu>
			</am:menubutton>

		 </fieldset>

		</td>
		<td valign="top">
<div id="afgrid" style="position:relative;">
<asp:Panel ID="pnlMessage" runat="server" Visible="true">
	<div align="center" class="afnormal"><asp:Literal ID="litMessage" runat="server" /></div>
</asp:Panel>
<asp:Panel ID="pnlResults" runat="server" Visible="false">
<table class="afgrid" cellspacing="0" cellpadding="4" width="100%">
	<tr>
		<td class="afgrouprow" width="100%" colspan="2" style="padding-left:10px;">[RESX:Topic]</td>


	</tr>
	<asp:Repeater id="rptPosts" runat="server" EnableViewState="False">
		<ItemTemplate>
			<tr>
				<td width="20" class='aftopicrow'>
					<asp:Image id="Image1" runat="server" ImageUrl='<%#GetIcon(DataBinder.Eval(Container.DataItem, "TopicIcon"),DataBinder.Eval(Container.DataItem, "IsPinned"),DataBinder.Eval(Container.DataItem, "IsLocked"))%>'>
					</asp:Image></td>
				<td  width="100%" class='aftopicrow'>
				<%#GetTopic(DataBinder.Eval(Container.DataItem, "ForumID"), DataBinder.Eval(Container.DataItem, "TopicId"), DataBinder.Eval(Container.DataItem, "ContentId"), DataBinder.Eval(Container.DataItem, "Body"), DataBinder.Eval(Container.DataItem, "DateUpdated"), DataBinder.Eval(Container.DataItem, "Subject"), DataBinder.Eval(Container.DataItem, "AuthorId"), DataBinder.Eval(Container.DataItem, "ReplyCount"), DataBinder.Eval(Container.DataItem, "ForumName"), DataBinder.Eval(Container.DataItem, "UserName"), DataBinder.Eval(Container.DataItem, "FirstName"), DataBinder.Eval(Container.DataItem, "LastName"), DataBinder.Eval(Container.DataItem, "TopicType"), DataBinder.Eval(Container.DataItem, "DisplayName"))%>
				</td>


			</tr>
		</ItemTemplate>
		<AlternatingItemTemplate>
		<tr>
				<td width="20" class='aftopicrowalt'>
					<asp:Image id="Image1" runat="server" ImageUrl='<%#GetIcon(DataBinder.Eval(Container.DataItem, "TopicIcon"),DataBinder.Eval(Container.DataItem, "IsPinned"),DataBinder.Eval(Container.DataItem, "IsLocked"))%>'>
					</asp:Image></td>
				<td  width="100%" class='aftopicrowalt'><%#GetTopic(DataBinder.Eval(Container.DataItem, "ForumID"), DataBinder.Eval(Container.DataItem, "TopicId"), DataBinder.Eval(Container.DataItem, "ContentId"), DataBinder.Eval(Container.DataItem, "Body"), DataBinder.Eval(Container.DataItem, "DateUpdated"), DataBinder.Eval(Container.DataItem, "Subject"), DataBinder.Eval(Container.DataItem, "AuthorId"), DataBinder.Eval(Container.DataItem, "ReplyCount"), DataBinder.Eval(Container.DataItem, "ForumName"), DataBinder.Eval(Container.DataItem, "UserName"), DataBinder.Eval(Container.DataItem, "FirstName"), DataBinder.Eval(Container.DataItem, "LastName"), DataBinder.Eval(Container.DataItem, "TopicType"), DataBinder.Eval(Container.DataItem, "DisplayName"))%>
				</td>

			</tr>
		</AlternatingItemTemplate>
	</asp:Repeater>
</table>
</asp:Panel>
</div>
<div align="right"><am:pagernav id="Pager1" runat="server" /></div>
</td></tr></table></div>
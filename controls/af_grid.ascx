<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_grid.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_grid" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" Assembly="DotNetNuke.Modules.ActiveForums" %>

<table cellspacing="0" cellpadding="4" width="100%">
	<tr>
		<td><asp:Label ID="lblHeader" CssClass="aftitlelg" runat="server" /></td>
		<td align="right">
<asp:DropDownList ID="drpTimeFrame" runat="server" AutoPostBack="true" CssClass="aftextbox" Visible="false" Width="200">
			<asp:ListItem Value=15 resourcekey="ActiveTopics.15min">Last 15 Minutes</asp:ListItem>
			<asp:ListItem Value=30 resourcekey="ActiveTopics.30min">Last 30 Minutes</asp:ListItem>
			<asp:ListItem Value=45 resourcekey="ActiveTopics.45min">Last 45 Minutes</asp:ListItem>
			<asp:ListItem Value=60 resourcekey="ActiveTopics.60min">Last Hour</asp:ListItem>
			<asp:ListItem Value=120 resourcekey="ActiveTopics.120min">Last 2 Hours</asp:ListItem>
			<asp:ListItem Value="360" resourcekey="ActiveTopics.360min">Last 6 Hours</asp:ListItem>
			<asp:ListItem Value="720" resourcekey="ActiveTopics.720min">Last 12 Hours</asp:ListItem>
			<asp:ListItem Value="1440" resourcekey="ActiveTopics.1440min">Yesterday</asp:ListItem>
			<asp:ListItem Value="2880" resourcekey="ActiveTopics.2880min">Last 2 Days</asp:ListItem>
			<asp:ListItem Value="10080" resourcekey="ActiveTopics.10080min">Last Week</asp:ListItem>
			<asp:ListItem Value="20160" resourcekey="ActiveTopics.20160min">Last 2 Weeks</asp:ListItem>
			<asp:ListItem Value="40320" resourcekey="ActiveTopics.40320min">Last Month</asp:ListItem>
			<asp:ListItem Value="80640" resourcekey="ActiveTopics.80640min">Last 2 Months</asp:ListItem>
		</asp:DropDownList>        

		</td>
	</tr>
</table>
<div id="afgrid" style="position:relative;">
<table class="afgrid" cellspacing="0" cellpadding="4" width="100%">
	<tr>
		<td class="afgrouprow" width="100%" colspan="2" style="padding-left:10px;">[RESX:Topic]</td>
		<td class="afgrouprow" style="white-space:nowrap;padding-left:5px;padding-right:5px;" align="center">[RESX:REPLIES]</td>
		<td class="afgrouprow" style="white-space:nowrap;padding-left:5px;padding-right:5px;" align="center">[RESX:Views]</td>
		<td class="afgrouprow" style="white-space:nowrap" align="center">[RESX:LastPost]</td>
	</tr>
	<asp:Repeater id="rptPosts" runat="server" EnableViewState="False">
		<ItemTemplate>
			<tr>
				<td width="20" class='<%#GetRowCSS(DataBinder.Eval(Container.DataItem, "IsRead").ToString())%>'>
					<asp:Image id="Image1" runat="server" ImageUrl='<%#GetIcon(Eval("IsRead"),Eval("TopicIcon"),Eval("IsPinned"),Eval("IsLocked"))%>'>
					</asp:Image></td>
				<td  width="100%" class='<%#GetRowCSS(DataBinder.Eval(Container.DataItem, "IsRead").ToString())%>' >
					<%#GetTopic(DataBinder.Eval(Container.DataItem, "TopicURL"), DataBinder.Eval(Container.DataItem, "ForumUrl"), DataBinder.Eval(Container.DataItem, "GroupUrl"), DataBinder.Eval(Container.DataItem, "ForumGroupId"), DataBinder.Eval(Container.DataItem, "ForumID"), DataBinder.Eval(Container.DataItem, "TopicId"), DataBinder.Eval(Container.DataItem, "Subject"), DataBinder.Eval(Container.DataItem, "AuthorId"), DataBinder.Eval(Container.DataItem, "ReplyCount"), DataBinder.Eval(Container.DataItem, "ForumName"), DataBinder.Eval(Container.DataItem, "AuthorUserName"), DataBinder.Eval(Container.DataItem, "AuthorFirstName"), DataBinder.Eval(Container.DataItem, "AuthorLastName"), DataBinder.Eval(Container.DataItem, "TopicType"), DataBinder.Eval(Container.DataItem, "AuthorDisplayName"), DataBinder.Eval(Container.DataItem, "LastReplyRead"))%>

				</td>
				<td  style='white-space:nowrap' align='center' class='<%#GetRowCSS(DataBinder.Eval(Container.DataItem, "IsRead"))%>' ><%#DataBinder.Eval(Container.DataItem, "ReplyCount")%></td>
				<td  style='white-space:nowrap' align='center' class='<%#GetRowCSS(DataBinder.Eval(Container.DataItem, "IsRead"))%>' ><%#DataBinder.Eval(Container.DataItem, "ViewCount")%></td>
				<td  style='white-space:nowrap' align='center' class='<%#GetRowCSS(DataBinder.Eval(Container.DataItem, "IsRead"))%>' ><div class="af_lastpost">
						<asp:literal id="Literal4" runat="server" Text='<%#GetLastPost(DataBinder.Eval(Container, "DataItem.LastReplyAuthorId"), DataBinder.Eval(Container, "DataItem.LastReplyUserName"), DataBinder.Eval(Container, "DataItem.LastReplyDate"), DataBinder.Eval(Container, "DataItem.LastReplyID"), DataBinder.Eval(Container, "DataItem.TopicId"), DataBinder.Eval(Container, "DataItem.Subject"), DataBinder.Eval(Container, "DataItem.ForumID"), DataBinder.Eval(Container, "DataItem.ReplyCount"), DataBinder.Eval(Container, "DataItem.LastReplyFirstName"), DataBinder.Eval(Container, "DataItem.LastReplyLastName"), DataBinder.Eval(Container, "DataItem.LastReplyDisplayName"))%>'>
						</asp:literal></div>
				</td>
			</tr>
		</ItemTemplate>
		<AlternatingItemTemplate>
		<tr>
				<td width="20" class='<%#GetRowAltCSS(DataBinder.Eval(Container.DataItem, "IsRead"))%>'>
					<asp:Image id="Image1" runat="server" ImageUrl='<%#GetIcon(DataBinder.Eval(Container.DataItem, "IsRead").ToString(),DataBinder.Eval(Container.DataItem, "TopicIcon"),DataBinder.Eval(Container.DataItem, "IsPinned").ToString(),DataBinder.Eval(Container.DataItem, "IsLocked").ToString())%>'>
					</asp:Image></td>
				<td  width="100%" class='<%#GetRowAltCSS(DataBinder.Eval(Container.DataItem, "IsRead"))%>' ><%#GetTopic(DataBinder.Eval(Container.DataItem, "TopicURL"), DataBinder.Eval(Container.DataItem, "ForumUrl"), DataBinder.Eval(Container.DataItem, "GroupUrl"), DataBinder.Eval(Container.DataItem, "ForumGroupId"), DataBinder.Eval(Container.DataItem, "ForumID"), DataBinder.Eval(Container.DataItem, "TopicId"), DataBinder.Eval(Container.DataItem, "Subject"), DataBinder.Eval(Container.DataItem, "AuthorId"), DataBinder.Eval(Container.DataItem, "ReplyCount"), DataBinder.Eval(Container.DataItem, "ForumName"), DataBinder.Eval(Container.DataItem, "AuthorUserName"), DataBinder.Eval(Container.DataItem, "AuthorFirstName"), DataBinder.Eval(Container.DataItem, "AuthorLastName"), DataBinder.Eval(Container.DataItem, "TopicType"), DataBinder.Eval(Container.DataItem, "AuthorDisplayName"), DataBinder.Eval(Container.DataItem, "LastReplyRead"))%>
				</td>
				<td  style='white-space:nowrap' align='center' class='<%#GetRowAltCSS(DataBinder.Eval(Container.DataItem, "IsRead"))%>' ><%#DataBinder.Eval(Container.DataItem, "ReplyCount")%></td>
				<td  style='white-space:nowrap' align='center' class='<%#GetRowAltCSS(DataBinder.Eval(Container.DataItem, "IsRead"))%>' ><%#DataBinder.Eval(Container.DataItem, "ViewCount")%></td>
				<td  style='white-space:nowrap' align='center' class='<%#GetRowAltCSS(DataBinder.Eval(Container.DataItem, "IsRead"))%>' ><div class="af_lastpost">
						<asp:literal id="Literal4" runat="server" Text='<%#GetLastPost(DataBinder.Eval(Container, "DataItem.LastReplyAuthorId"), DataBinder.Eval(Container, "DataItem.LastReplyUserName"), DataBinder.Eval(Container, "DataItem.LastReplyDate"), DataBinder.Eval(Container, "DataItem.LastReplyID"), DataBinder.Eval(Container, "DataItem.TopicId"), DataBinder.Eval(Container, "DataItem.Subject"), DataBinder.Eval(Container, "DataItem.ForumID"), DataBinder.Eval(Container, "DataItem.ReplyCount"), DataBinder.Eval(Container, "DataItem.LastReplyFirstName"), DataBinder.Eval(Container, "DataItem.LastReplyLastName"), DataBinder.Eval(Container, "DataItem.LastReplyDisplayName"))%>'>
						</asp:literal></div>
				</td>
			</tr>
		</AlternatingItemTemplate>
	</asp:Repeater>
</table>
</div>
<div>
	<div style="width:50%;"><asp:PlaceHolder ID="plhMarkRead" runat="server" /></div>
	<div align="right"><am:pagernav id="Pager1" runat="server"></am:pagernav></div>   
 </div>
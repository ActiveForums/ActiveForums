<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_grid.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_grid" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" Assembly="DotNetNuke.Modules.ActiveForums" %>
<div class="af-search-header">
    <am:pagernav id="PagerTop" runat="server" />
    <h1 class="af-search-title"><asp:Label ID="lblHeader" runat="server" /></h1>
    <button type="submit" runat="server" ID="btnMarkRead" class="af-markread" Visible="false" onclick="if(!af_confirmMarkAllRead()) return false;" />
    <asp:DropDownList ID="drpTimeFrame" runat="server" AutoPostBack="true" Visible="false" >
		<asp:ListItem Value=15 resourcekey="ActiveTopics.15min">Last 15 Minutes</asp:ListItem>
		<asp:ListItem Value=30 resourcekey="ActiveTopics.30min">Last 30 Minutes</asp:ListItem>
		<asp:ListItem Value=45 resourcekey="ActiveTopics.45min">Last 45 Minutes</asp:ListItem>
		<asp:ListItem Value=60 resourcekey="ActiveTopics.60min">Last Hour</asp:ListItem>
		<asp:ListItem Value=120 resourcekey="ActiveTopics.120min">Last 2 Hours</asp:ListItem>
		<asp:ListItem Value="360" resourcekey="ActiveTopics.360min">Last 6 Hours</asp:ListItem>
		<asp:ListItem Value="720" resourcekey="ActiveTopics.720min">Last 12 Hours</asp:ListItem>
		<asp:ListItem Value="1440" resourcekey="ActiveTopics.1440min">Last Day</asp:ListItem>
		<asp:ListItem Value="2880" resourcekey="ActiveTopics.2880min">Last 2 Days</asp:ListItem>
		<asp:ListItem Value="10080" resourcekey="ActiveTopics.10080min">Last Week</asp:ListItem>
		<asp:ListItem Value="20160" resourcekey="ActiveTopics.20160min">Last 2 Weeks</asp:ListItem>
		<asp:ListItem Value="40320" resourcekey="ActiveTopics.40320min">Last Month</asp:ListItem>
		<asp:ListItem Value="80640" resourcekey="ActiveTopics.80640min">Last 2 Months</asp:ListItem>
	</asp:DropDownList>  
</div>
<div class="af-search-bar">
    <span class="af-search-title"><%= GetSharedResource("[RESX:SearchByTopics]") %></span>
</div>
<asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="af-search-noresults">
    <asp:Literal ID="litMessage" runat="server" />
</asp:Panel>
<div class="af-search-results" style="position: relative;">
    <asp:repeater id="rptTopics" runat="server">
        <HeaderTemplate><table class="af-topics"></HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="af-icon">
                    <a href="<%# GetThreadUrl() %>"><asp:Image runat="server" ImageUrl='<%#GetIcon()%>' /></a>
                </td>
                <td class="af-thread">
                    <a class="af-thread-link" href="<%# GetThreadUrl() %>"><%# Eval("Subject") %></a><br/>
                    <%= GetSharedResource("Started") %> <%# GetPostTime() %> <%= GetSharedResource("By") %> <%# GetAuthor() %>
                    <span class="af-mini-pager"><%# GetMiniPager() %></span> 
                </td>
                <td class="af-stats">
                    <label><%= GetSharedResource("SearchReplies") %></label><span> <%# Eval("ReplyCount") %></span><br/>
                    <label><%= GetSharedResource("SearchViews") %></label><span> <%# Eval("ViewCount") %></span>
                </td>
                <td class="af-lastpost">
                    <label><%= GetSharedResource("SearchLastPost") %></label> <%# GetLastPostTime() %> <br/>
                    <label><%= GetSharedResource("SearchBy") %></label> <%# GetLastPostAuthor() %>
                </td>
                <td class="af-forum">
                   <label><%= GetSharedResource("SearchForum") %></label><br/>
                   <a class="af-forum-url" href="<%# GetForumUrl() %>"><%# Eval("ForumName") %></a> 
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:Repeater>
</div>
<div class="af-search-footer">
    <am:pagernav id="PagerBottom" runat="server" />
    <span class="af-search-recordCount"><asp:Literal runat="server" ID="litRecordCount" /></span>  
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('.af-markread').button({ "icons": { "primary": "ui-icon-check" } });
    });
    
    function af_confirmMarkAllRead() {
        return confirm('<%= GetSharedResource("MarkAllReadConfirm") %>');
    }
</script>

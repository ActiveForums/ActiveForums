<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_search.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_search" EnableViewState="false" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<div class="af-search-header">
    <am:pagernav id="PagerTop" runat="server" />
    <h1 class="af-search-title"><%= GetSharedResource("Search") %> </h1><button class="af-search-modify"><%= GetSharedResource("SearchModify") %></button><br/>  
    <asp:PlaceHolder runat="server" ID="phKeywords" Visible="False">
        <span class="af-search-criteria">
            <%= GetSharedResource("[RESX:SearchKeywords]") %> 
            <asp:Repeater runat="server" ID="rptKeywords">
                <ItemTemplate> <b><%# Eval("Value") %></b></ItemTemplate>
                <SeparatorTemplate>, </SeparatorTemplate>
            </asp:Repeater>
        </span>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phUsername" Visible="False">
        <span class="af-search-criteria">
            <%= GetSharedResource("[RESX:SearchByUser]") %> <b><asp:Literal runat="server" ID="litUserName"></asp:Literal></b>
        </span>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phTag" Visible="False">
        <span class="af-search-criteria">
           <%= GetSharedResource("[RESX:SearchByTag]") %> <b><asp:Literal runat="server" ID="litTag"></asp:Literal></b>
        </span>
    </asp:PlaceHolder>
</div>
<div class="af-search-bar">
    <span class="af-search-duration"><asp:Literal runat="server" ID="litSearchDuration" /> <asp:Literal runat="server" ID="litSearchAge" /></span>
    <span class="af-search-title"><asp:Literal runat="server" ID="litSearchTitle" /></span>
</div>
<asp:Panel ID="pnlMessage" runat="server" Visible="true" CssClass="af-search-noresults">
    <asp:Literal ID="litMessage" runat="server" />
</asp:Panel>
<div class="af-search-results" style="position: relative;">
    
    <!-- Post View -->
    <asp:Repeater runat="server" ID="rptPosts" Visible="False">
        <ItemTemplate>
            <div class="af-post">
                <div class="af-post-header">
                    <div class="af-stats">
                        <label><%= GetSharedResource("SearchReplies") %></label><span><%# Eval("ReplyCount") %></span><br/>
                        <label><%= GetSharedResource("SearchViews") %></label><span><%# Eval("ViewCount") %></span>
                    </div>
                    <div class="af-forum"><label><%= GetSharedResource("SearchForum") %></label> <a class="af-forum-url" href="<%# GetForumUrl() %>"><%# Eval("ForumName") %></a></div>
                    <div class="af-thread"><label><%= GetSharedResource("SearchTopic") %></label> <a class="af-thread-url" href="<%# GetThreadUrl() %>"><%# Eval("Subject") %></a></div>
                    <div class="af-postinfo"><label><%= GetSharedResource("SearchPosted") %></label><%# GetPostTime() %> <%= GetSharedResource("By") %> <a class="af-profile-url" href="<%# GetAuthorProfileUrl() %>"><%# GetAuthorName() %></a></div>   
                </div>
                <div class="af-post-content">
                    <a class="af-post-url" href="<%# GetPostUrl() %>"><%# Eval("PostSubject") %></a>
                    <div><%# GetPostSnippet() %></div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <!-- Topic View -->
    <asp:repeater id="rptTopics" runat="server" Visible="False">
        <HeaderTemplate><table class="af-topics"></HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="af-icon">
                    <a href="<%# GetThreadUrl() %>"><asp:Image runat="server" ImageUrl='<%#GetIcon()%>' /></a>
                </td>
                <td class="af-thread">
                    <a class="af-thread-link" href="<%# GetThreadUrl() %>"><%# Eval("Subject") %></a><br/>
                    <%= GetSharedResource("Started") %> <%# GetPostTime() %> <%= GetSharedResource("By") %> <a class="af-profile-link" href="<%# GetAuthorProfileUrl() %>"><%# GetAuthorName() %></a>
                    <span class="af-mini-pager"><%# GetMiniPager() %></span> 
                </td>
                <td class="af-stats">
                    <label><%= GetSharedResource("SearchReplies") %></label><span> <%# Eval("ReplyCount") %></span><br/>
                    <label><%= GetSharedResource("SearchViews") %></label><span> <%# Eval("ViewCount") %></span>
                </td>
                <td class="af-reply">
                    <label><%= GetSharedResource("SearchLastReply") %></label> <%# GetLastReplyTime() %> <br/>
                    <label><%= GetSharedResource("SearchBy") %></label> <a class="af-profile-link" href="<%# GetLastReplyAuthorProfileUrl() %>"><%# GetLastReplyAuthorName() %></a>
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

    $(document).ready(function() {

        $('.af-search-modify').button({ "icons": { "primary": "ui-icon-wrench" } }).click(function () {
            document.location.href = '<%=GetSearchUrl()%>';
            return false;
        });
    });

</script>

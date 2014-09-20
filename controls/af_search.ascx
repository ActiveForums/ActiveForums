<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_search.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_search" EnableViewState="false" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<div class="af-search-header">

    <am:pagernav id="PagerTop" runat="server" />
    <h1 class="af-search-title"><%= GetSharedResource("Search") %> </h1>
    <br/>
    <button class="af-search-modify"><%= GetSharedResource("SearchModify") %></button><br/>  
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
<div class="af-search-bar afgrouprow afgrouprow-f">
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
                    <div class="af-forum"><label><%= GetSharedResource("SearchForum") %></label> <a class="af-forum-url" href='<%# GetForumUrl() %>'><%# Eval("ForumName") %></a></div>
                    <div class="af-thread"><label><%= GetSharedResource("SearchTopic") %></label> <a class="af-thread-url" href='<%# GetThreadUrl() %>'><%# Eval("Subject") %></a></div>
                    <div class="af-postinfo"><label><%= GetSharedResource("SearchPosted") %></label><%# GetPostTime() %> <%= GetSharedResource("By") %> <%# GetAuthor() %></div>   
                </div>
                <div class="af-post-content">
                    <a class="af-post-url" href='<%# GetPostUrl() %>'><%# Eval("PostSubject") %></a>
                    <div><%# GetPostSnippet() %></div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <!-- Topic View -->
    <asp:repeater id="rptTopics" runat="server" Visible="False">
        <HeaderTemplate>
        	<table class="afgrid" cellspacing="0" cellpadding="0" width="100%">
        </HeaderTemplate>
        <ItemTemplate>
					<tr>
						<td colspan="0">
							 <table class="afgrid" cellspacing="0" cellpadding="0" width="100%">
								<tr>
									<td class="aftopicrow _hide" width="20"><a href='<%# GetThreadUrl() %>'><asp:Image runat="server" ImageUrl='<%#GetIcon()%>' /></a></td>
									<td class="aftopicrow af-content">
										<table>
											<tr>
												<td rowspan="2" class="afsubject">
												
												<span class="afhiddenstats"><%# Eval("ReplyCount") %> replies and <%# Eval("ViewCount") %> views</span>
												<span class="aftopictitle"><a class="af-thread-link" href='<%# GetThreadUrl() %>'><%# Eval("Subject") %></a></span> 
												<span class="aftopicsubtitle"><%= GetSharedResource("Started") %> <%# GetPostTime() %> <%= GetSharedResource("By") %> <%# GetAuthor() %></span>
												
											</tr>
										</table>
									</td>
									<td class="aftopicrow af-colstats af-colstats-replies"><%# Eval("ReplyCount") %></td>
									<td class="aftopicrow af-colstats af-colstats-views"><%# Eval("ViewCount") %></td>
									<td class="aftopicrow af-lastpost"><div class="af_lastpost" style="white-space:nowrap;">In: <a class="af-forum-url" href='<%# GetForumUrl() %>'><%# Eval("ForumName") %></a> <br /><%= GetSharedResource("SearchBy") %> <%# GetLastPostAuthor() %><br /><%# GetLastPostTime() %></div></td>
								</tr>
							</table>	
									</td>
								</tr>
							</table>
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

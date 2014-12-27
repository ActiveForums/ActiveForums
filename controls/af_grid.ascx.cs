//
// Active Forums - http://www.dnnsoftware.com
// Copyright (c) 2013
// by DNN Corp.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.ActiveForums.Controls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_grid : ForumBase
    {
        #region Private Members
        
        private int _rowCount;
        private DataTable _dtResults;
        private int _pageSize = 20;
        private int _rowIndex;
        private DataRow _currentRow;
        private string _currentTheme = "_default";
        
        #endregion

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            _currentTheme = MainSettings.Theme;

            drpTimeFrame.SelectedIndexChanged += DrpTimeFrameSelectedIndexChanged;
            btnMarkRead.ServerClick += BtnMarkReadClick;

            rptTopics.ItemCreated += RepeaterOnItemCreated;

            var sortDirection = Request.Params["afsort"] ?? "DESC";

            BindPosts(sortDirection);
        }

        private void DrpTimeFrameSelectedIndexChanged(object sender, EventArgs e)
        {
            var timeframe = Utilities.SafeConvertInt(drpTimeFrame.SelectedItem.Value, 1440);
            Response.Redirect(NavigateUrl(TabId, string.Empty, new[] { ParamKeys.ViewType + "=grid", "afgt=activetopics", "ts=" + timeframe }));
        }

        private void BtnMarkReadClick(object sender, EventArgs e)
        {
            if(UserId >= 0)
                DataProvider.Instance().Utility_MarkAllRead(ModuleId, UserId, 0);

            Response.Redirect(NavigateUrl(TabId, string.Empty, new[] { ParamKeys.ViewType + "=grid", "afgt=notread" }));
        }

        private void RepeaterOnItemCreated(object sender, RepeaterItemEventArgs repeaterItemEventArgs)
        {
            var dataRowView = repeaterItemEventArgs.Item.DataItem as DataRowView;

            if (dataRowView == null)
                return;

            _currentRow = dataRowView.Row;
        }

        #endregion
        
        #region Private Methods
        
        private void BindPosts(string sort = "ASC")
        {
            _pageSize = MainSettings.PageSize;
            
            if (UserId > 0)
                _pageSize = UserDefaultPageSize;
            
            if (_pageSize < 5)
                _pageSize = 10;

            _rowIndex = (PageId == 1) ? 0 : ((PageId * _pageSize) - _pageSize);

            var db = new Data.Common();
            var fc = new ForumController();
            var forumIds = fc.GetForumsForUser(ForumUser.UserRoles, PortalId, ModuleId, "CanRead");
            
            var sCrumb = "<a href=\"" + Utilities.NavigateUrl(TabId, "", new[] { ParamKeys.ViewType + "=grid", "afgt=xxx" }) + "\">yyyy</a>";
            sCrumb = sCrumb.Replace("xxx", "{0}").Replace("yyyy", "{1}");
            
            if (Request.Params["afgt"] != null)
            {
                var gview = Utilities.XSSFilter(Request.Params["afgt"]).ToLowerInvariant();
                switch (gview)
                {
                    case "notread":

                        if (UserId != -1)
                        {
                            lblHeader.Text = GetSharedResource("[RESX:NotRead]");
                            _dtResults = db.UI_NotReadView(PortalId, ModuleId, UserId, _rowIndex, _pageSize, sort, forumIds).Tables[0];
                            if (_dtResults.Rows.Count > 0)
                            {
                                _rowCount = _dtResults.Rows[0].GetInt("RecordCount");
                                btnMarkRead.Visible = true;
                                btnMarkRead.InnerText = GetSharedResource("[RESX:MarkAllRead]");
                            }
                        }
                        else
                            Response.Redirect(NavigateUrl(TabId), true);
                        break;

                    case "unanswered":

                        lblHeader.Text = GetSharedResource("[RESX:Unanswered]");
                        _dtResults = db.UI_UnansweredView(PortalId, ModuleId, UserId, _rowIndex, _pageSize, sort, forumIds).Tables[0];
                        if (_dtResults.Rows.Count > 0)
                            _rowCount = _dtResults.Rows[0].GetInt("RecordCount");

                        break;

                    case "tags":

                        var tagId = -1;
                        if (Request.QueryString["aftg"] != null && SimulateIsNumeric.IsNumeric(Request.QueryString["aftg"]))
                            tagId = int.Parse(Request.QueryString["aftg"]);

                        lblHeader.Text = GetSharedResource("[RESX:Tags]");
                        _dtResults = db.UI_TagsView(PortalId, ModuleId, UserId, _rowIndex, _pageSize, sort, forumIds, tagId).Tables[0];
                        if (_dtResults.Rows.Count > 0)
                            _rowCount = _dtResults.Rows[0].GetInt("RecordCount");

                        break;

                    case "mytopics":

                        if (UserId != -1)
                        {
                            lblHeader.Text = GetSharedResource("[RESX:MyTopics]");
                            _dtResults = db.UI_MyTopicsView(PortalId, ModuleId, UserId, _rowIndex, _pageSize, sort, forumIds).Tables[0];
                            if (_dtResults.Rows.Count > 0)
                                _rowCount = _dtResults.Rows[0].GetInt("RecordCount");
                        }
                        else
                            Response.Redirect(NavigateUrl(TabId), true);

                        break;

                    case "activetopics":

                        lblHeader.Text = GetSharedResource("[RESX:ActiveTopics]");

                        /*
                        if (UserLastAccess != Utilities.NullDate())
                        {
                            timeFrame = Convert.ToInt32(SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Minute, UserLastAccess, DateTime.Now));
                            drpTimeFrame.Items.Insert(0, new ListItem(GetDate(UserLastAccess), "~" + timeFrame.ToString()));
                        }
                         */

                        var timeFrame = Utilities.SafeConvertInt(Request.Params["ts"], 1440);

                        if (timeFrame < 15 | timeFrame > 80640)
                            timeFrame = 1440;

                        drpTimeFrame.Visible = true;
                        drpTimeFrame.SelectedIndex = drpTimeFrame.Items.IndexOf(drpTimeFrame.Items.FindByValue(timeFrame.ToString()));
                        _dtResults = db.UI_ActiveView(PortalId, ModuleId, UserId, _rowIndex, _pageSize, sort, timeFrame, forumIds).Tables[0];
                        if (_dtResults.Rows.Count > 0)
                            _rowCount = Convert.ToInt32(_dtResults.Rows[0]["RecordCount"]);

                        break;

                    default:
                        Response.Redirect(NavigateUrl(TabId), true);
                        break;
                }

                sCrumb = string.Format(sCrumb, gview, lblHeader.Text);
               
                if (MainSettings.UseSkinBreadCrumb)
                    Environment.UpdateBreadCrumb(Page.Controls, sCrumb);

                var tempVar = BasePage;
                Environment.UpdateMeta(ref tempVar, "[VALUE] - " + lblHeader.Text, "[VALUE]", "[VALUE]");
            }


            if (_dtResults != null && _dtResults.Rows.Count > 0)
            {
                litRecordCount.Text = string.Format(GetSharedResource("[RESX:SearchRecords]"), _rowIndex + 1, _rowIndex + _dtResults.Rows.Count, _rowCount);

                pnlMessage.Visible = false;

                try
                {
                    rptTopics.Visible = true;
                    rptTopics.DataSource = _dtResults;
                    rptTopics.DataBind();
                    BuildPager(PagerTop);
                    BuildPager(PagerBottom);
                }
                catch (Exception ex)
                {
                    litMessage.Text = ex.Message;
                    pnlMessage.Visible = true;
                    rptTopics.Visible = false;
                }
            }
            else
            {
                litMessage.Text = GetSharedResource("[RESX:SearchNoResults]");
                pnlMessage.Visible = true;
            }

        }

        private void BuildPager(PagerNav pager)
        {
            if (pager == null)
                return;

            var intPages = Convert.ToInt32(Math.Ceiling(_rowCount/(double) _pageSize));

            string[] @params;
            if (Request.Params["afsort"] != null)
                @params = new[]
                              {
                                  "afgt=" + Request.Params["afgt"], "afsort=" + Request.Params["afsort"],
                                  "afcol=" + Request.Params["afcol"]
                              };
            else if (Request.Params["ts"] != null)
                @params = new[] {"afgt=" + Request.Params["afgt"], "ts=" + Request.Params["ts"]};
            else
                @params = new[] {"afgt=" + Request.Params["afgt"]};


            pager.PageCount = intPages;
            pager.CurrentPage = PageId;
            pager.TabID = TabId;
            pager.ForumID = ForumId;
            pager.PageText = Utilities.GetSharedResource("[RESX:Page]");
            pager.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
            pager.View = "grid";

            pager.PageMode = Modules.ActiveForums.Controls.PagerNav.Mode.Links;

            if (MainSettings.URLRewriteEnabled)
            {
                if (!(string.IsNullOrEmpty(MainSettings.PrefixURLBase)))
                    pager.BaseURL = "/" + MainSettings.PrefixURLBase;

                if (!(string.IsNullOrEmpty(MainSettings.PrefixURLOther)))
                    pager.BaseURL += "/" + MainSettings.PrefixURLOther;

                pager.BaseURL += "/" + Request.Params["afgt"] + "/";
            }

            pager.Params = @params;
        }

        #endregion
        
        #region Public Methods

        public string GetForumUrl()
        {
            if (_currentRow == null)
                return null;

            var forumId = _currentRow["ForumID"].ToString();

            var @params = new[] { ParamKeys.ForumId + "=" + forumId, ParamKeys.ViewType + "=" + Views.Topics };

            return NavigateUrl(TabId, string.Empty, @params);
        }

        public string GetThreadUrl()
        {
            if (_currentRow == null)
                return null;

            var forumId = _currentRow["ForumID"].ToString();
            var topicId = _currentRow["TopicId"].ToString();

            var @params = new[] { ParamKeys.ForumId + "=" + forumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + topicId };

            return NavigateUrl(TabId, string.Empty, @params);
        }

        public string GetLastRead()
        {
            if (_currentRow == null) return null;

            var forumId = _currentRow["ForumID"].ToString();
            var topicId = _currentRow["TopicId"].ToString();
            var userLastRead = _currentRow["UserLastTopicRead"].ToString();

            var @params = new [] {ParamKeys.ForumId + "=" + forumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + topicId, ParamKeys.FirstNewPost + "=" + userLastRead };
            return NavigateUrl(TabId, string.Empty, @params);
        }

        public string GetArrowPath()
        {
            string theme = Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + _currentTheme  + "/images/miniarrow_down.png");
            return theme;
        }

        // Todo: Localize today and yesterday
        public string GetPostTime()
        {
            if (_currentRow == null)
                return null;

            var date = GetUserDate(Convert.ToDateTime(_currentRow["DateCreated"]));
            var currentDate = GetUserDate(DateTime.Now);

            var datePart = date.ToString(MainSettings.DateFormatString);

            if (currentDate.Date == date.Date)
                datePart = GetSharedResource("Today");
            else if (currentDate.AddDays(-1).Date == date.Date)
                datePart = GetSharedResource("Yesterday");

            return string.Format(GetSharedResource("SearchPostTime"), datePart, date.ToString(MainSettings.TimeFormatString));
        }

        public string GetAuthor()
        {
            if (_currentRow == null)
                return null;

            var userId = Convert.ToInt32(_currentRow["AuthorId"]);
            var userName = _currentRow["AuthorUserName"].ToString();
            var firstName = _currentRow["AuthorFirstName"].ToString();
            var lastName = _currentRow["AuthorLastName"].ToString();
            var displayName = _currentRow["AuthorDisplayName"].ToString();

            return UserProfiles.GetDisplayName(ModuleId, true, false, ForumUser.IsAdmin, userId, userName, firstName, lastName, displayName);
        }

        public string GetLastPostAuthor()
        {
            if (_currentRow == null)
                return null;

            var userId = Convert.ToInt32(_currentRow["LastReplyAuthorId"]);
            var userName = _currentRow["LastReplyUserName"].ToString();
            var firstName = _currentRow["LastReplyFirstName"].ToString();
            var lastName = _currentRow["LastReplyLastName"].ToString();
            var displayName = _currentRow["LastReplyDisplayName"].ToString();

            return UserProfiles.GetDisplayName(ModuleId, true, false, ForumUser.IsAdmin, userId, userName, firstName, lastName, displayName);
        }

        // Todo: Localize today and yesterday
        public string GetLastPostTime()
        {
            if (_currentRow == null)
                return null;

            var date = GetUserDate(Convert.ToDateTime(_currentRow["LastReplyDate"]));
            var currentDate = GetUserDate(DateTime.Now);

            var datePart = date.ToString(MainSettings.DateFormatString);

            if (currentDate.Date == date.Date)
                datePart = "Today";
            else if (currentDate.AddDays(-1).Date == date.Date)
                datePart = "Yesterday";

            return datePart + " @ " + date.ToString(MainSettings.TimeFormatString);
        }

        public string GetIcon()
        {
            if (_currentRow == null)
                return null;

            var theme = MainSettings.Theme;

            // If we have a post icon, use it
            var icon = _currentRow["TopicIcon"].ToString();
            if (!string.IsNullOrWhiteSpace(icon))
                return "~/DesktopModules/ActiveForums/themes/" + theme + "/emoticons/" + icon;

            // Otherwise, chose the icons based on the post stats

            var pinned = Convert.ToBoolean(_currentRow["IsPinned"]);
            var locked = Convert.ToBoolean(_currentRow["IsLocked"]);

            if(pinned && locked)
                return "~/DesktopModules/ActiveForums/themes/" + theme + "/images/topic_pinlocked.png";

            if (pinned)
                return "~/DesktopModules/ActiveForums/themes/" + theme + "/images/topic_pin.png";

            
            if (locked)
                return "~/DesktopModules/ActiveForums/themes/" + theme + "/images/topic_lock.png";

            // Unread has to be calculated based on a few fields
            //var topicId = Convert.ToInt32(_currentRow["TopicId"]);
            //var replyCount = Convert.ToInt32(_currentRow["replyCount"]);
            //var lastReplyId = Convert.ToInt32(_currentRow["LastReplyId"]);
            //var userLastTopicRead = Convert.ToInt32(_currentRow["UserLastTopicRead"]);
            //var userLastReplyRead = Convert.ToInt32(_currentRow["UserLastReplyRead"]);
            //var unread = (replyCount <= 0 && topicId > userLastTopicRead) || (lastReplyId > userLastReplyRead);

            var isRead = _currentRow.GetBoolean("IsRead");

            if (isRead)
                return "~/DesktopModules/ActiveForums/themes/" + theme + "/images/topic.png";

            return "~/DesktopModules/ActiveForums/themes/" + theme + "/images/topic_new.png";
        }

        public string GetMiniPager()
        {
            if (_currentRow == null)
                return null;

            var replyCount = Convert.ToInt32(_currentRow["ReplyCount"]);
            var pageCount = Convert.ToInt32(Math.Ceiling((double)(replyCount + 1) / _pageSize));
            var forumId = _currentRow["ForumId"].ToString();
            var topicId = _currentRow["TopicId"].ToString();

            // No pager if there is only one page.
            if (pageCount <= 1)
                return null;

            string[] @params;

            var result = string.Empty;

            if (pageCount <= 5)
            {
                for (var i = 1; i <= pageCount; i++)
                {
                    @params = new[] { ParamKeys.ForumId + "=" + forumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + i };
                    result += "<a href=\"" + NavigateUrl(TabId, string.Empty, @params) + "\">" + i + "</a>";
                }

                return result;
            }

            // 1 2 3 ... N

            for (var i = 1; i <= 3; i++)
            {
                @params = new[] { ParamKeys.ForumId + "=" + forumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + i };
                result += "<a href=\"" + NavigateUrl(TabId, string.Empty, @params) + "\">" + i + "</a>";
            }

            result += "<span>...</span>";

            @params = new[] { ParamKeys.ForumId + "=" + forumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + pageCount };
            result += "<a href=\"" + NavigateUrl(TabId, string.Empty, @params) + "\">" + pageCount + "</a>";

            return result;
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_grid : ForumBase
    {
        #region Private Members
        private int RowCount = 0;
        private DataTable dtResults;
        private int PageSize = 20;
        private int RowIndex = 0;
        #endregion
        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            drpTimeFrame.SelectedIndexChanged += new System.EventHandler(drpTimeFrame_SelectedIndexChanged);

            string SortDirection = null;
            if (Request.Params["afsort"] != null)
            {
                SortDirection = Request.Params["afsort"];
            }
            else
            {
                SortDirection = "DESC";
            }
            BindPosts(0, SortDirection);
        }
        private void drpTimeFrame_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Response.Redirect(NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=grid", "afgt=activetopics", "ts=" + Convert.ToInt32(drpTimeFrame.SelectedItem.Value) }));
        }
        #endregion
        #region Private Methods
        private void BindPosts(int Column = 0, string Sort = "ASC")
        {
            PageSize = MainSettings.PageSize;
            if (UserId > 0)
            {
                PageSize = UserDefaultPageSize;
            }
            if (PageSize < 5)
            {
                PageSize = 10;
            }

            if (PageId == 1)
            {
                RowIndex = 0;
            }
            else
            {
                RowIndex = ((PageId * PageSize) - PageSize);
            }
            Data.Common db = new Data.Common();
            string ForumIds = string.Empty;
            ForumController fc = new ForumController();
            ForumIds = fc.GetForumsForUser(ForumUser.UserRoles, PortalId, ModuleId, "CanRead");
            string sCrumb = "<a href=\"" + Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=grid", "afgt=xxx" }) + "\">yyyy</a>";
            sCrumb = sCrumb.Replace("xxx", "{0}").Replace("yyyy", "{1}");
            if (Request.Params["afgt"] != null)
            {
                string gview = Utilities.XSSFilter(Request.Params["afgt"]).ToLowerInvariant();
                switch (gview)
                {
                    case "notread":
                        if (!(this.UserId == -1))
                        {
                            lblHeader.Text = GetSharedResource("[RESX:NotRead]");
                            dtResults = db.UI_NotReadView(PortalId, ModuleId, UserId, RowIndex, PageSize, Sort, ForumIds).Tables[0];
                            if (dtResults.Rows.Count > 0)
                            {
                                RowCount = Convert.ToInt32(dtResults.Rows[0]["RecordCount"]);
                            }
                            ForumBase ctl = (ForumBase)(LoadControl("~/DesktopModules/ActiveForums/controls/af_markallread.ascx"));
                            ctl.ModuleConfiguration = this.ModuleConfiguration;
                            plhMarkRead.Controls.Add(ctl);
                        }
                        else
                        {
                            Response.Redirect(NavigateUrl(TabId), true);
                        }
                        break;
                    case "unanswered":
                        lblHeader.Text = GetSharedResource("[RESX:Unanswered]");
                        dtResults = db.UI_UnansweredView(PortalId, ModuleId, UserId, RowIndex, PageSize, Sort, ForumIds).Tables[0];
                        if (dtResults.Rows.Count > 0)
                        {
                            RowCount = Convert.ToInt32(dtResults.Rows[0]["RecordCount"]);
                        }
                        break;
                    case "tags":
                        int tagId = -1;
                        if (Request.QueryString["aftg"] != null && SimulateIsNumeric.IsNumeric(Request.QueryString["aftg"]))
                        {
                            tagId = int.Parse(Request.QueryString["aftg"]);
                        }
                        lblHeader.Text = GetSharedResource("[RESX:Tags]");
                        dtResults = db.UI_TagsView(PortalId, ModuleId, UserId, RowIndex, PageSize, Sort, ForumIds, tagId).Tables[0];
                        if (dtResults.Rows.Count > 0)
                        {
                            RowCount = Convert.ToInt32(dtResults.Rows[0]["RecordCount"]);
                        }
                        break;
                    case "mytopics":
                        if (!(this.UserId == -1))
                        {
                            lblHeader.Text = GetSharedResource("[RESX:MyTopics]");
                            dtResults = DataProvider.Instance().UI_MyTopicsView(PortalId, ModuleId, UserId, RowIndex, PageSize, Sort, UserInfo.IsSuperUser).Tables[0];
                            if (dtResults.Rows.Count > 0)
                            {
                                RowCount = Convert.ToInt32(dtResults.Rows[0]["RecordCount"]);
                            }
                        }
                        else
                        {
                            Response.Redirect(NavigateUrl(TabId), true);
                        }
                        break;
                    case "activetopics":
                        lblHeader.Text = GetSharedResource("[RESX:ActiveTopics]");
                        int timeFrame = 1440; //24hours
                        if (!(UserLastAccess == Utilities.NullDate()))
                        {
                            timeFrame = Convert.ToInt32(SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Minute, UserLastAccess, DateTime.Now));
                            drpTimeFrame.Items.Insert(0, new ListItem(GetDate(UserLastAccess), timeFrame.ToString()));
                        }

                        if (Request.Params["ts"] != null)
                        {
                            if (SimulateIsNumeric.IsNumeric(Request.Params["ts"]))
                            {
                                timeFrame = Convert.ToInt32(Request.Params["ts"]);
                                if (timeFrame < 15 | timeFrame > 80640)
                                {
                                    timeFrame = 1440;
                                }
                            }
                        }
                        drpTimeFrame.Visible = true;
                        drpTimeFrame.SelectedIndex = drpTimeFrame.Items.IndexOf(drpTimeFrame.Items.FindByValue(timeFrame.ToString()));
                        dtResults = db.UI_ActiveView(PortalId, ModuleId, UserId, RowIndex, PageSize, Sort, UserInfo.IsSuperUser, timeFrame, ForumIds).Tables[0];
                        if (dtResults.Rows.Count > 0)
                        {
                            RowCount = Convert.ToInt32(dtResults.Rows[0]["RecordCount"]);
                        }

                        break;
                    default:
                        Response.Redirect(NavigateUrl(TabId), true);
                        break;
                }
                sCrumb = string.Format(sCrumb, gview, lblHeader.Text);
                if (MainSettings.UseSkinBreadCrumb)
                {
                    Environment.UpdateBreadCrumb(Page.Controls, sCrumb);
                }
                DotNetNuke.Framework.CDefault tempVar = this.BasePage;
                Environment.UpdateMeta(ref tempVar, "[VALUE] - " + lblHeader.Text, "[VALUE]", "[VALUE]");
            }
            try
            {
                rptPosts.DataSource = dtResults;
                rptPosts.DataBind();
                BuildPager();
            }
            catch (Exception ex)
            {

            }
        }
        private void BuildPager()
        {
            int intPages = 0;
            intPages = Convert.ToInt32(System.Math.Ceiling(RowCount / (double)PageSize));
            Pager1.PageCount = intPages;
            Pager1.CurrentPage = PageId;
            Pager1.TabID = TabId;
            Pager1.ForumID = ForumId;
            Pager1.PageText = Utilities.GetSharedResource("[RESX:Page]");
            Pager1.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
            Pager1.View = "grid";
            // If UseAjax Then
            //Pager1.PageMode = Forums.Controls.PagerNav.Mode.CallBack
            // Else
            Pager1.PageMode = Modules.ActiveForums.Controls.PagerNav.Mode.Links;
            // End If
            if (MainSettings.URLRewriteEnabled)
            {
                if (!(string.IsNullOrEmpty(MainSettings.PrefixURLBase)))
                {
                    Pager1.BaseURL = "/" + MainSettings.PrefixURLBase;
                }
                if (!(string.IsNullOrEmpty(MainSettings.PrefixURLOther)))
                {
                    Pager1.BaseURL += "/" + MainSettings.PrefixURLOther;
                }
                Pager1.BaseURL += "/" + Request.Params["afgt"] + "/";
                Pager1.PageMode = Modules.ActiveForums.Controls.PagerNav.Mode.Links;
            }
            if (Request.Params["afsort"] != null)
            {
                string[] Params = { "afgt=" + Request.Params["afgt"], "afsort=" + Request.Params["afsort"], "afcol=" + Request.Params["afcol"] };
                Pager1.Params = Params;
            }
            else if (Request.Params["ts"] != null)
            {
                string[] Params = { "afgt=" + Request.Params["afgt"], "ts=" + Request.Params["ts"] };
                Pager1.Params = Params;
            }
            else
            {
                string[] Params = { "afgt=" + Request.Params["afgt"] };
                Pager1.Params = Params;
            }
        }
        #endregion
        #region Public Methods
        public string GetIcon(object isRead, object icon, object pinned, object locked)
        {
            string myTheme = MainSettings.Theme;
            string Icon = (string)icon;
            bool Pinned = (bool)pinned;
            bool Locked = (bool)locked;
            bool IsRead = false;
            if (isRead.ToString() == "0") {
                IsRead = false;
            } else {
                IsRead = true;
            }
           
            if (Icon == string.Empty)
            {

                if (Pinned == true)
                {
                    return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/topic_pin.png";
                }
                else if (Locked == true)
                {
                    return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/lock.gif";
                }
                else
                {
                    if (IsRead == true)
                    {
                        return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/document.gif";
                    }
                    else
                    {
                        return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/document_new.gif";
                    }

                }
            }
            else
            {
                return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/" + Icon;
            }
        }
        public string GetLastPost(object userID, object userName, object dateAdded, object lastPostID, object parentPostID, 
            object subject, object forumID, object replyCount, object firstName, object lastName, object displayName)
        {
            int UserID = (int)userID;
            string UserName = (string)userName;
            DateTime DateAdded = (DateTime)dateAdded;
            int LastPostID = (int)lastPostID;
            int ParentPostID = (int)parentPostID;
            string Subject = subject.ToString();
            int ForumID = (int)forumID;
            int ReplyCount = (int)replyCount;
            string FirstName = firstName.ToString();
            string LastName = lastName.ToString();
            string DisplayName = displayName.ToString();




            try
            {
                int PostId = LastPostID;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (!(UserName == ""))
                {
                    sb.Append("<nobr> [RESX:BY] ");
                    if (UserID == 0 || UserID == -1)
                    {
                        sb.Append(UserName);
                    }
                    else
                    {
                        bool isMod = false;
                        if (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.ForumMod || CurrentUserType == CurrentUserTypes.SuperUser)
                        {
                            isMod = true;
                        }
                        sb.Append(UserProfiles.GetDisplayName(ModuleId, MainSettings.ProfileVisibility, isMod, UserID, MainSettings.UserNameDisplay, UserName, FirstName, LastName, DisplayName));
                    }
                    sb.Append("</nobr><br />");
                }

                sb.Append("<nobr>" + GetDate(DateAdded) + "</nobr><br />");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "&nbsp;";
            }
        }
        public string GetTopic(object topicUrl, object forumUrl, object groupUrl, object forumGroupId, object forumID, object topicId, object subject, object userID, object replies, 
            object forumName, object userName, object firstName, object lastName, object postType, object displayName, object lastReplyRead)
        {
            string TopicUrl = (string)topicUrl;
            string ForumUrl = (string)forumUrl;
            string GroupUrl = (string)groupUrl;
            int ForumGroupId = (int)forumGroupId;
            int ForumID = (int)forumID;
            int TopicId = (int)topicId;
            string Subject = (string)subject;
            int UserID = (int)userID;
            int Replies = (int)replies;
            string ForumName = (string)forumName;
            string UserName = (string)userName;
            string FirstName = String.Empty;
            if (!String.IsNullOrEmpty(firstName.ToString())) {
                FirstName = firstName.ToString();
            }
            string LastName = String.Empty;
            if (!String.IsNullOrEmpty(lastName.ToString())) {
                LastName = lastName.ToString();
            }
            string PostType = String.Empty;
            if (!String.IsNullOrEmpty(postType.ToString())) {
                PostType = postType.ToString();
            }
            string DisplayName = String.Empty;
            if (!String.IsNullOrEmpty(displayName.ToString())) {
                DisplayName = displayName.ToString();
            }
            int LastReplyRead = (int)lastReplyRead;
            string sPollImage = "";
            if (PostType == "POLL")
            {
                sPollImage = "<img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MainSettings.Theme + "/poll.gif") + "\" align=\"absmiddle\" border=\"0\" alt=\"[RESX:Poll]\" />";
            }
            ControlUtils ctlUtils = new ControlUtils();
            string sTopicURL = string.Empty;
            sTopicURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, GroupUrl, ForumUrl, ForumGroupId, ForumID, TopicId, TopicUrl, -1, -1, string.Empty, 1, SocialGroupId);

            string sOut = string.Empty;
            //Dim Params As String() = {ParamKeys.ForumId & "=" & ForumID, ParamKeys.TopicId & "=" & TopicId, ParamKeys.ViewType & "=" & Views.Topic}
            sOut = "<a href=\"" + sTopicURL + "\">" + sPollImage + Subject + "</a>";
            if (LastReplyRead > 0 & LastReplyRead > TopicId)
            {
                if (sTopicURL.EndsWith("/"))
                {
                    sOut += "<a href=\"" + sTopicURL + "?" + ParamKeys.FirstNewPost + "=" + LastReplyRead + "\" rel=\"nofollow\"><img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MainSettings.Theme + "/miniarrow_down.png") + "\" style=\"vertical-align:middle;\" alt=\"[RESX:JumpToLastRead]\" border=\"0\" class=\"afminiarrow\" /></a>";
                }
                else
                {
                    sOut += "<a href=\"" + NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.FirstNewPost + "=" + LastReplyRead }) + "\" rel=\"nofollow\"><img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MainSettings.Theme + "/miniarrow_down.png") + "\" style=\"vertical-align:middle;\" alt=\"[RESX:JumpToLastRead]\" border=\"0\" class=\"afminiarrow\" /></a>";
                }

            }
            sOut += "<br />";
            string sForumURL = string.Empty;
            sForumURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, GroupUrl, ForumUrl, ForumGroupId, ForumID, -1, -1, string.Empty, 0, SocialGroupId);
            sOut += "<span class=\"afsmalltext\">[RESX:BY] " + GetDisplayName(UserName, UserID, FirstName, LastName, DisplayName) + " " + GetSharedResource("IN.Text") + " <a href=\"" + sForumURL + "\">" + ForumName + "</a></span>";
            sOut += GetSubPages(TabId, ModuleId, Replies, ForumID, TopicId);
            return sOut;
        }
        public string GetDisplayName(string Username, int UserID, string FirstName = "", string LastName = "", string DisplayName = "")
        {
            return UserProfiles.GetDisplayName(ModuleId, MainSettings.ProfileVisibility, false, UserID, MainSettings.UserNameDisplay, Username, FirstName, LastName, DisplayName);
        }
        public string GetRowCSS(object isRead)
        {
            bool IsRead = false;
            if (isRead.ToString() == "0") {
                IsRead = false;
            } else {
                IsRead = true;
            }
            
           
            if (IsRead == true)
            {
                return "aftopicrow";
            }
            else
            {
                return "aftopicrownew";
            }
        }
        public string GetRowAltCSS(object isRead)
        {
            bool IsRead = false;
            if (isRead.ToString() == "0") {
                IsRead = false;
            } else {
                IsRead = true;
            }
       
            if (IsRead == true)
            {
                return "aftopicrowalt";
            }
            else
            {
                return "aftopicrownewalt";
            }
        }
        private string GetSubPages(int TabID, int ModuleID, int Replies, int ForumID, int PostID)
        {
            int i = 0;
            string sOut = "";

            if (Replies + 1 > PageSize)
            {
                sOut = "&nbsp;<div class=\"afpagermini\">[RESX:SubPages]&nbsp;";
                //Jump to pages
                int intPostPages = 0;
                intPostPages = Convert.ToInt32(System.Math.Ceiling((double)(Replies + 1) / PageSize));
                if (intPostPages > 5)
                {
                    for (i = 1; i <= 5; i++)
                    {
                        if (UseAjax)
                        {
                            //afPageJump
                            //sOut &= "<span class=""afpagerminiitem"" onclick=""javascript:afPageJump(" & i & ");"">" & i & "</span>&nbsp;"
                            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageJumpId + "=" + i };
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", Params) + "\">" + i + "</a>&nbsp;";
                        }
                        else
                        {
                            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + i };
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", Params) + "\">" + i + "</a>&nbsp;";
                        }

                    }
                    if (intPostPages > 6)
                    {
                        sOut += "...&nbsp;";
                    }
                    if (UseAjax)
                    {
                        //sOut &= "<span class=""afpagerminiitem"" onclick=""javascript:afPageJump(" & intPostPages & ");"">" & intPostPages & "</span>&nbsp;"
                        string[] Params2 = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageJumpId + "=" + intPostPages };
                        sOut += "<a href=\"" + NavigateUrl(TabID, "", Params2) + "\">" + intPostPages + "</a>&nbsp;";
                    }
                    else
                    {
                        string[] Params2 = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + intPostPages };
                        sOut += "<a href=\"" + NavigateUrl(TabID, "", Params2) + "\">" + intPostPages + "</a>&nbsp;";
                    }


                }
                else
                {
                    for (i = 1; i <= intPostPages; i++)
                    {
                        if (UseAjax)
                        {
                            //sOut &= "<span class=""afpagerminiitem"" onclick=""javascript:afPageJump(" & i & ");"">" & i & "</span>&nbsp;"
                            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageJumpId + "=" + i };
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", Params) + "\">" + i + "</a>&nbsp;";
                        }
                        else
                        {
                            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + i };
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", Params) + "\">" + i + "</a>&nbsp;";
                        }

                    }
                }

                sOut += "</div>";
            }
            return sOut;
        }
        #endregion
    }
}

//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
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
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [ToolboxData("<{0}:WhatsNewControl runat=server></{0}:WhatsNewControl>")]
    public class WhatsNewControl : Entities.Modules.PortalModuleBase
    {

        #region Private Member Variables

        private string _forumIds = string.Empty;
        private bool _topicsOnly = true;
        private int _rows = 10;
        private string _tags = string.Empty;
        private int _filterByUserId = -1;
        private int _tabId = -1;
        private string _additionalParams = string.Empty;

        #endregion

        public string AdditionalParams
        {
            get
            {
                return _additionalParams;
            }
            set
            {
                _additionalParams = value;
            }
        }

        public int TabId
        {
            get
            {
                return _tabId;
            }
            set
            {
                _tabId = value;
            }
        }

        public string ForumIds
        {
            get
            {
                return _forumIds;
            }
            set
            {
                _forumIds = value;
            }
        }

        public bool TopicsOnly
        {
            get
            {
                return _topicsOnly;
            }
            set
            {
                _topicsOnly = value;
            }
        }

        public bool RandomOrder { get; set; }

        public int Rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
            }
        }

        public string Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                _tags = value;
            }
        }

        public int FilterByUserId
        {
            get
            {
                return _filterByUserId;
            }
            set
            {
                _filterByUserId = value;
            }
        }

        [Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
        public DisplayTemplate Template { get; set; }

        [Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
        public DisplayTemplate HeaderTemplate { get; set; }

        [Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
        public DisplayTemplate FooterTemplate { get; set; }

        public bool EnableRSS { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int timeOffset = Convert.ToInt32(PortalSettings.TimeZone.BaseUtcOffset.TotalMinutes);
            if (UserId > 0)
            {
                var uc = new Entities.Users.UserController();
                
                var dnnUser = uc.GetUser(PortalId, UserId);
                string propValue = UserInfo.Profile.GetPropertyValue(Entities.Users.UserProfile.USERPROFILE_TimeZone);
                if (!string.IsNullOrEmpty(propValue))
                {
                    timeOffset = int.Parse(propValue);
                }

                if (timeOffset == 0)
                    timeOffset = Convert.ToInt32(PortalSettings.TimeZone.BaseUtcOffset.TotalMinutes);
            }

            var sHeaderTemplate = "<div style=\"padding:10px;padding-top:5px;\">";
            var sFooterTemplate = "</div>";

            if (HeaderTemplate != null)
                sHeaderTemplate = HeaderTemplate.Text;

            if (FooterTemplate != null)
                sFooterTemplate = FooterTemplate.Text;

            var sTemplate = "<div style=\"padding-bottom:2px;\" class=\"normal\">[SUBJECTLINK]</div><div style=\"padding-bottom:2px;border-bottom:solid 1px #AAA;\">by [AUTHORDISPLAYNAME]</div>";
            if (Template != null)
                sTemplate = Template.Text;

            if (ForumIds == string.Empty && FilterByUserId <= 0) 
                return;

            if (ForumIds.Contains(";"))
                ForumIds = ForumIds.Replace(";", ":");

            var sb = new StringBuilder(1024);
            sb.Append(sHeaderTemplate);

            var bodyLength = -1;
            var bodyTrim = string.Empty;
            if (sTemplate.Contains("[BODY:"))
            {
                var inStart = (sTemplate.IndexOf("[BODY:", StringComparison.Ordinal) + 1) + 5;
                var inEnd = (sTemplate.IndexOf("]", inStart - 1, StringComparison.Ordinal) + 1) - 1;
                var sLength = sTemplate.Substring(inStart, inEnd - inStart);
                bodyLength = Convert.ToInt32(sLength);
                bodyTrim = "[BODY:" + bodyLength + "]";
            }

            IDataReader dr;
            if (ForumIds == string.Empty && FilterByUserId > 0)
            {
                var fc = new ForumController();
                var uc = new UserController();
                var u = uc.DNNGetCurrentUser(PortalId, -1);
                ForumIds = fc.GetForumsForUser(u.UserRoles, PortalId, -1);
                ForumIds = ForumIds.Replace(";", ":");
                dr = DataProvider.Instance().GetPostsByUser(PortalId, Rows, UserInfo.IsSuperUser, UserInfo.UserID, FilterByUserId, TopicsOnly, ForumIds);
            }
            else
            {
                dr = DataProvider.Instance().GetPosts(PortalId, ForumIds, TopicsOnly, RandomOrder, Rows, Tags, FilterByUserId);
            }

            var useFriendly = Utilities.IsRewriteLoaded();
            var sHost = Utilities.GetHost();
            try
            {
                var sTempTemplate = sTemplate;
                string lastPostDate;
                while (dr.Read())
                {
                    var groupName = Convert.ToString(dr["GroupName"]);
                    var groupId = Convert.ToString(dr["ForumGroupId"]);
                    var topicTabId = Convert.ToString(dr["TabId"]);
                    var topicModuleId = Convert.ToString(dr["ModuleId"]);
                    var forumName = Convert.ToString(dr["ForumName"]);
                    var forumId = Convert.ToString(dr["ForumId"]);
                    var subject = Convert.ToString(dr["Subject"]);
                    var userName = Convert.ToString(dr["AuthorUserName"]);
                    var firstName = Convert.ToString(dr["AuthorFirstName"]);
                    var lastName = Convert.ToString(dr["AuthorLastName"]);
                    var authorId = Convert.ToString(dr["AuthorId"]);
                    var displayName = Convert.ToString(dr["AuthorDisplayName"]);
                    var postDate = Convert.ToString(dr["DateCreated"]);
                    var body = Utilities.StripHTMLTag(Convert.ToString(dr["Body"]));
                    var topicId = Convert.ToString(dr["TopicId"]);
                    var replyId = Convert.ToString(dr["ReplyId"]);
                    var bodyHTML = Convert.ToString(dr["Body"]);
                    var replyCount = Convert.ToString(dr["ReplyCount"]);
                    var sForumUrl = dr["PrefixURL"].ToString();
                    var sTopicURL = dr["TopicURL"].ToString();
                    var sGroupPrefixURL = dr["GroupPrefixURL"].ToString();
                    sTempTemplate = sTempTemplate.Replace("[FORUMGROUPNAME]", groupName);
                    sTempTemplate = sTempTemplate.Replace("[FORUMGROUPID]", groupId);
                    sTempTemplate = sTempTemplate.Replace("[TOPICTABID]", topicTabId);
                    sTempTemplate = sTempTemplate.Replace("[TOPICMODULEID]", topicModuleId);
                    sTempTemplate = sTempTemplate.Replace("[FORUMNAME]", forumName);
                    sTempTemplate = sTempTemplate.Replace("[FORUMID]", forumId);
                    sTempTemplate = sTempTemplate.Replace("[SUBJECT]", subject);
                    sTempTemplate = sTempTemplate.Replace("[AUTHORUSERNAME]", userName);
                    sTempTemplate = sTempTemplate.Replace("[AUTHORFIRSTNAME]", firstName);
                    sTempTemplate = sTempTemplate.Replace("[AUTHORLASTNAME]", lastName);
                    sTempTemplate = sTempTemplate.Replace("[AUTHORID]", authorId);
                    sTempTemplate = sTempTemplate.Replace("[AUTHORDISPLAYNAME]", displayName);
                    sTempTemplate = sTempTemplate.Replace("[DATE]", Utilities.GetDate(Convert.ToDateTime(postDate), Convert.ToInt32(topicModuleId), timeOffset));
                    sTempTemplate = sTempTemplate.Replace("[BODY]", body);
                    sTempTemplate = sTempTemplate.Replace("[BODYHTML]", bodyHTML);
                    sTempTemplate = sTempTemplate.Replace("[BODYTEXT]", Utilities.StripHTMLTag(bodyHTML));
                    
                    if (bodyTrim != string.Empty)
                    {
                        if (bodyLength > 0 & body.Length > bodyLength)
                            sTempTemplate = sTempTemplate.Replace(bodyTrim, body.Substring(0, bodyLength) + "...");
                        else
                            sTempTemplate = sTempTemplate.Replace(bodyTrim, body);
                    }

                    sTempTemplate = sTempTemplate.Replace("[TOPICID]", topicId);
                    sTempTemplate = sTempTemplate.Replace("[REPLYID]", replyId);
                    sTempTemplate = sTempTemplate.Replace("[REPLYCOUNT]", replyCount);

                    if (TabId == -1)
                        TabId = Convert.ToInt32(topicTabId);

                    if (useFriendly && !(string.IsNullOrEmpty(sForumUrl)) && !(string.IsNullOrEmpty(sTopicURL)))
                    {
                        var ctlUtils = new ControlUtils();
                        sTopicURL = ctlUtils.BuildUrl(Convert.ToInt32(topicTabId), Convert.ToInt32(topicModuleId), sGroupPrefixURL, sForumUrl, Convert.ToInt32(groupId), Convert.ToInt32(forumId), Convert.ToInt32(topicId), sTopicURL, -1, -1, string.Empty, 1, Convert.ToInt32(replyId), -1);

                        if (Convert.ToInt32(replyId) == 0)
                        {
                            sTempTemplate = sTempTemplate.Replace("[POSTURL]", sTopicURL);
                            sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + sTopicURL + "\">" + subject + "</a>");
                        }
                        else
                        {
                            if (!(sTopicURL.EndsWith("/")))
                                sTopicURL += "/";

                            sTopicURL += "?afc=" + replyId;
                            sTempTemplate = sTempTemplate.Replace("[POSTURL]", sTopicURL);
                            if (Request.IsAuthenticated)
                                sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + sTopicURL + "\">" + subject + "</a>");
                            else
                                sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + sTopicURL + "\" rel=\"nofollow\">" + subject + "</a>");
                        }
                        sTempTemplate = sTempTemplate.Replace("[TOPICSURL]", sForumUrl);

                    }
                    else
                    {
                        List<string> @params;
                        if (Convert.ToInt32(replyId) == 0)
                        {
                            @params = new List<string>{ ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + forumId, ParamKeys.TopicId + "=" + topicId };
                            if (AdditionalParams != string.Empty)
                                @params.Add(AdditionalParams);

                            sTempTemplate = sTempTemplate.Replace("[POSTURL]", Utilities.NavigateUrl(TabId, string.Empty, @params.ToArray()));
                           
                            @params = new List<string> { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + forumId, ParamKeys.TopicId + "=" + topicId };
                            if (AdditionalParams != string.Empty)
                                @params.Add(AdditionalParams);

                            sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", @params.ToArray()) + "\">" + subject + "</a>");
                        }
                        else
                        {
                            @params = new List<string> { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + forumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.ContentJumpId + "=" + replyId };
                            if (AdditionalParams != string.Empty)
                                @params.Add(AdditionalParams);

                            sTempTemplate = sTempTemplate.Replace("[POSTURL]", Utilities.NavigateUrl(TabId, "", @params.ToArray()));

                            @params = new List<string> { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + forumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.ContentJumpId + "=" + replyId };
                            if (AdditionalParams != string.Empty)
                                @params.Add(AdditionalParams);

                            sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, string.Empty, @params.ToArray()) + "\">" + subject + "</a>");
                        }
                        @params = new List<string> { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + forumId };
                        if (AdditionalParams != string.Empty)
                            @params.Add(AdditionalParams);

                        sTempTemplate = sTempTemplate.Replace("[TOPICSURL]", Utilities.NavigateUrl(TabId, string.Empty, @params.ToArray()));
                    }
                    sTempTemplate = sTempTemplate.Replace("[FORUMURL]", Utilities.NavigateUrl(TabId));
                    sb.Append(sTempTemplate);
                }

                dr.Close();

                var sRSSImage = "<img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/images/feedicon.gif") + "\" border=\"0\" />";
                var sRSSURL = Page.ResolveUrl("~/desktopmodules/activeforumswhatsnew/feeds.aspx") + "?portalId=" + PortalId + "&tabid=" + TabId.ToString() + "&moduleid=" + ModuleId.ToString();
                var sRSSIconLink = "<a href=\"" + sRSSURL + "\">" + sRSSImage + "</a>";
                sFooterTemplate = sFooterTemplate.Replace("[RSSICON]", sRSSImage);
                sFooterTemplate = sFooterTemplate.Replace("[RSSURL]", sRSSURL);
                sFooterTemplate = sFooterTemplate.Replace("[RSSICONLINK]", sRSSIconLink);

                sb.Append(sFooterTemplate);
                var lit = new Literal {Text = sb.ToString()};
                Controls.Add(lit);

            }
            catch (Exception ex)
            {
                if (!dr.IsClosed)
                {
                    dr.Close();
                }
                sb.Append(ex.StackTrace);
                var lit = new Literal {Text = ex.Message};
                Controls.Add(lit);
            }
        }
    }
}

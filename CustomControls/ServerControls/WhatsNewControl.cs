using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [ToolboxData("<{0}:WhatsNewControl runat=server></{0}:WhatsNewControl>")]
    public class WhatsNewControl : DotNetNuke.Entities.Modules.PortalModuleBase
    {

        private string _forumIds = string.Empty;
        private bool _topicsOnly = true;
        private bool _randomOrder = false;
        private int _rows = 10;
        private string _tags = string.Empty;
        private int _filterByUserId = -1;
        private DisplayTemplate _whatsNewTemplate;
        private DisplayTemplate _headerTemplate;
        private DisplayTemplate _footerTemplate;
        private bool _allowRSS;
        private int _tabId = -1;
        private string _additionalParams = string.Empty;
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
        public bool RandomOrder
        {
            get
            {
                return _randomOrder;
            }
            set
            {
                _randomOrder = value;
            }
        }
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
        public DisplayTemplate Template
        {
            get
            {
                return _whatsNewTemplate;
            }
            set
            {
                _whatsNewTemplate = value;
            }
        }
        [Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
        public DisplayTemplate HeaderTemplate
        {
            get
            {
                return _headerTemplate;
            }
            set
            {
                _headerTemplate = value;
            }
        }
        [Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
        public DisplayTemplate FooterTemplate
        {
            get
            {
                return _footerTemplate;
            }
            set
            {
                _footerTemplate = value;
            }
        }
        public bool EnableRSS
        {
            get
            {
                return _allowRSS;
            }
            set
            {
                _allowRSS = false;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //If TabId = -1 Then
            //    TabId = CInt(Request.QueryString["TabId"])
            //End If
            int timeOffset = 0;
            timeOffset = PortalSettings.TimeZoneOffset;
            if (UserId > 0)
            {
                DotNetNuke.Entities.Users.UserController uc = new DotNetNuke.Entities.Users.UserController();
                DotNetNuke.Entities.Users.UserInfo dnnUser = uc.GetUser(PortalId, UserId);
                timeOffset = dnnUser.Profile.TimeZone;
                if (timeOffset == 0)
                {
                    timeOffset = PortalSettings.TimeZoneOffset;
                }
            }

            string sHeaderTemplate = "<div style=\"padding:10px;padding-top:5px;\">";
            string sFooterTemplate = "</div>";
            if (HeaderTemplate != null)
            {
                sHeaderTemplate = HeaderTemplate.Text;
            }
            if (FooterTemplate != null)
            {
                sFooterTemplate = FooterTemplate.Text;
            }
            string sTemplate = "<div style=\"padding-bottom:2px;\" class=\"normal\">[SUBJECTLINK]</div><div style=\"padding-bottom:2px;border-bottom:solid 1px #AAA;\">by [AUTHORDISPLAYNAME]</div>";
            if (Template != null)
            {
                sTemplate = Template.Text;
            }
            if (!(ForumIds == string.Empty) || FilterByUserId > 0)
            {
                if (ForumIds.Contains(";"))
                {
                    ForumIds = ForumIds.Replace(";", ":");
                }
                StringBuilder sb = new StringBuilder(1024);
                sb.Append(sHeaderTemplate);
                int BodyLength = -1;
                string BodyTrim = "";
                if (sTemplate.Contains("[BODY:"))
                {
                    int inStart = (sTemplate.IndexOf("[BODY:", 0) + 1) + 5;
                    int inEnd = (sTemplate.IndexOf("]", inStart - 1) + 1) - 1;
                    string sLength = sTemplate.Substring(inStart, inEnd - inStart);
                    BodyLength = Convert.ToInt32(sLength);
                    BodyTrim = "[BODY:" + BodyLength.ToString() + "]";
                }
                IDataReader dr = null;
                if (ForumIds == string.Empty && FilterByUserId > 0)
                {
                    ForumController fc = new ForumController();
                    UserController uc = new UserController();
                    User u = uc.DNNGetCurrentUser(PortalId, -1);
                    ForumIds = fc.GetForumsForUser(u.UserRoles, PortalId, -1);
                    ForumIds = ForumIds.Replace(";", ":");
                    dr = DataProvider.Instance().GetPostsByUser(PortalId, Rows, UserInfo.IsSuperUser, UserInfo.UserID, FilterByUserId, TopicsOnly, ForumIds);
                }
                else
                {
                    dr = DataProvider.Instance().GetPosts(PortalId, ForumIds, TopicsOnly, RandomOrder, Rows, Tags, FilterByUserId);
                }
                bool useFriendly = Utilities.IsRewriteLoaded();
                string sHost = Utilities.GetHost();
                try
                {
                    while (dr.Read())
                    {
                        string sTempTemplate = sTemplate;
                        string GroupName = string.Empty;
                        string GroupId = string.Empty;
                        string TopicTabId = string.Empty;
                        string TopicModuleId = string.Empty;
                        string ForumName = string.Empty;
                        string ForumId = string.Empty;
                        string Subject = string.Empty;
                        string UserName = string.Empty;
                        string PostDate = string.Empty;
                        string Body = string.Empty;
                        string BodyHTML = string.Empty;
                        string DisplayName = string.Empty;
                        string ReplyCount = string.Empty;
                        string LastPostDate = string.Empty;
                        string TopicId = string.Empty;
                        string ReplyId = string.Empty;
                        string FirstName = string.Empty;
                        string LastName = string.Empty;
                        string AuthorId = string.Empty;
                        string sForumUrl = string.Empty;
                        string sTopicURL = string.Empty;
                        string sGroupPrefixURL = string.Empty;

                        GroupName = Convert.ToString(dr["GroupName"]);
                        GroupId = Convert.ToString(dr["ForumGroupId"]);
                        TopicTabId = Convert.ToString(dr["TabId"]);
                        TopicModuleId = Convert.ToString(dr["ModuleId"]);
                        ForumName = Convert.ToString(dr["ForumName"]);
                        ForumId = Convert.ToString(dr["ForumId"]);
                        Subject = Convert.ToString(dr["Subject"]);
                        UserName = Convert.ToString(dr["AuthorUserName"]);
                        FirstName = Convert.ToString(dr["AuthorFirstName"]);
                        LastName = Convert.ToString(dr["AuthorLastName"]);
                        AuthorId = Convert.ToString(dr["AuthorId"]);
                        DisplayName = Convert.ToString(dr["AuthorDisplayName"]);
                        PostDate = Convert.ToString(dr["DateCreated"]);
                        Body = Utilities.StripHTMLTag(Convert.ToString(dr["Body"]));
                        TopicId = Convert.ToString(dr["TopicId"]);
                        ReplyId = Convert.ToString(dr["ReplyId"]);
                        BodyHTML = Convert.ToString(dr["Body"]);
                        ReplyCount = Convert.ToString(dr["ReplyCount"]);
                        sForumUrl = dr["PrefixURL"].ToString();
                        sTopicURL = dr["TopicURL"].ToString();
                        sGroupPrefixURL = dr["GroupPrefixURL"].ToString();
                        sTempTemplate = sTempTemplate.Replace("[FORUMGROUPNAME]", GroupName);
                        sTempTemplate = sTempTemplate.Replace("[FORUMGROUPID]", GroupId);
                        sTempTemplate = sTempTemplate.Replace("[TOPICTABID]", TopicTabId);
                        sTempTemplate = sTempTemplate.Replace("[TOPICMODULEID]", TopicModuleId);
                        sTempTemplate = sTempTemplate.Replace("[FORUMNAME]", ForumName);
                        sTempTemplate = sTempTemplate.Replace("[FORUMID]", ForumId);
                        sTempTemplate = sTempTemplate.Replace("[SUBJECT]", Subject);
                        sTempTemplate = sTempTemplate.Replace("[AUTHORUSERNAME]", UserName);
                        sTempTemplate = sTempTemplate.Replace("[AUTHORFIRSTNAME]", FirstName);
                        sTempTemplate = sTempTemplate.Replace("[AUTHORLASTNAME]", LastName);
                        sTempTemplate = sTempTemplate.Replace("[AUTHORID]", AuthorId);
                        sTempTemplate = sTempTemplate.Replace("[AUTHORDISPLAYNAME]", DisplayName);
                        sTempTemplate = sTempTemplate.Replace("[DATE]", Utilities.GetDate(Convert.ToDateTime(PostDate), Convert.ToInt32(TopicModuleId), timeOffset));
                        sTempTemplate = sTempTemplate.Replace("[BODY]", Body);
                        sTempTemplate = sTempTemplate.Replace("[BODYHTML]", BodyHTML);
                        sTempTemplate = sTempTemplate.Replace("[BODYTEXT]", Utilities.StripHTMLTag(BodyHTML));
                        if (!(BodyTrim == string.Empty))
                        {
                            if (BodyLength > 0 & Body.Length > BodyLength)
                            {
                                sTempTemplate = sTempTemplate.Replace(BodyTrim, Body.Substring(0, BodyLength) + "...");
                            }
                            else
                            {
                                sTempTemplate = sTempTemplate.Replace(BodyTrim, Body);
                            }
                        }

                        sTempTemplate = sTempTemplate.Replace("[TOPICID]", TopicId);
                        sTempTemplate = sTempTemplate.Replace("[REPLYID]", ReplyId);
                        sTempTemplate = sTempTemplate.Replace("[REPLYCOUNT]", ReplyCount);

                        if (TabId == -1)
                        {
                            TabId = Convert.ToInt32(TopicTabId);
                        }
                        if (useFriendly && !(string.IsNullOrEmpty(sForumUrl)) && !(string.IsNullOrEmpty(sTopicURL)))
                        {
                            ControlUtils ctlUtils = new ControlUtils();
                            sTopicURL = ctlUtils.BuildUrl(Convert.ToInt32(TopicTabId), Convert.ToInt32(TopicModuleId), sGroupPrefixURL, sForumUrl, Convert.ToInt32(GroupId), Convert.ToInt32(ForumId), Convert.ToInt32(TopicId), sTopicURL, -1, -1, string.Empty, 1, -1);

                            //If sHost.EndsWith("/") And sForumUrl.StartsWith("/") Then
                            //    sForumUrl = sForumUrl.Substring(1)
                            //End If
                            //sForumUrl = sHost & sForumUrl
                            //If sHost.EndsWith("/") And sTopicURL.StartsWith("/") Then
                            //    sTopicURL = sTopicURL.Substring(1)
                            //End If
                            //sTopicURL = sHost & sTopicURL
                            if (Convert.ToInt32(ReplyId) == 0)
                            {
                                sTempTemplate = sTempTemplate.Replace("[POSTURL]", sTopicURL);
                                sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + sTopicURL + "\">" + Subject + "</a>");
                            }
                            else
                            {
                                if (!(sTopicURL.EndsWith("/")))
                                {
                                    sTopicURL += "/";
                                }
                                sTopicURL += "?afc=" + ReplyId;
                                sTempTemplate = sTempTemplate.Replace("[POSTURL]", sTopicURL);
                                if (Request.IsAuthenticated)
                                {
                                    sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + sTopicURL + "\">" + Subject + "</a>");
                                }
                                else
                                {
                                    sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + sTopicURL + "\" rel=\"nofollow\">" + Subject + "</a>");
                                }
                            }
                            sTempTemplate = sTempTemplate.Replace("[TOPICSURL]", sForumUrl);

                        }
                        else
                        {
                            string[] @params = null;
                            if (Convert.ToInt32(ReplyId) == 0)
                            {
                                @params = new string[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId };
                                if (!(AdditionalParams == string.Empty))
                                {
                                    @params = Utilities.AddParams(AdditionalParams, @params);
                                }
                                sTempTemplate = sTempTemplate.Replace("[POSTURL]", Utilities.NavigateUrl(TabId, "", @params));
                                @params = new string[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId };
                                if (!(AdditionalParams == string.Empty))
                                {
                                    @params = Utilities.AddParams(AdditionalParams, @params);
                                }
                                sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", @params) + "\">" + Subject + "</a>");
                            }
                            else
                            {
                                @params = new string[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId };
                                if (!(AdditionalParams == string.Empty))
                                {
                                    @params = Utilities.AddParams(AdditionalParams, @params);
                                }
                                sTempTemplate = sTempTemplate.Replace("[POSTURL]", Utilities.NavigateUrl(TabId, "", @params));
                                @params = new string[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId };
                                if (!(AdditionalParams == string.Empty))
                                {
                                    @params = Utilities.AddParams(AdditionalParams, @params);
                                }
                                sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", @params) + "\">" + Subject + "</a>");
                            }
                            @params = new string[] { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumId };
                            if (!(AdditionalParams == string.Empty))
                            {
                                @params = Utilities.AddParams(AdditionalParams, @params);
                            }
                            sTempTemplate = sTempTemplate.Replace("[TOPICSURL]", Utilities.NavigateUrl(TabId, "", @params));

                        }
                        sTempTemplate = sTempTemplate.Replace("[FORUMURL]", Utilities.NavigateUrl(TabId));
                        sb.Append(sTempTemplate);
                        //sb.Append("<div class=""astopicsubject""><a href=""" & Utilities.NavigateUrl(CInt(TopicTabId), "", New String() {ParamKeys.ViewType & "=" & Views.Topic, ParamKeys.ForumId & "=" & ForumId, ParamKeys.TopicId & "=" & TopicId}) & """>" & Subject & "</a> in " & ForumName & "</div>")
                        //If Body.Length > 150 Then
                        //    Body = Body.Substring(0, 150)
                        //End If
                        //sb.Append("<div class=""astopicbody"">" & Body & "</div>")
                    }
                    dr.Close();
                    string sRSSImage = string.Empty;
                    string sRSSURL = string.Empty;
                    string sRSSIconLink = string.Empty;

                    sRSSImage = "<img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/images/feedicon.gif") + "\" border=\"0\" />";
                    sRSSURL = Page.ResolveUrl("~/desktopmodules/activeforumswhatsnew/feeds.aspx") + "?portalId=" + PortalId + "&tabid=" + TabId.ToString() + "&moduleid=" + ModuleId.ToString();
                    sRSSIconLink = "<a href=\"" + sRSSURL + "\">" + sRSSImage + "</a>";
                    sFooterTemplate = sFooterTemplate.Replace("[RSSICON]", sRSSImage);
                    sFooterTemplate = sFooterTemplate.Replace("[RSSURL]", sRSSURL);
                    sFooterTemplate = sFooterTemplate.Replace("[RSSICONLINK]", sRSSIconLink);

                    sb.Append(sFooterTemplate);
                    Literal lit = new Literal();
                    lit.Text = sb.ToString();
                    this.Controls.Add(lit);

                }
                catch (Exception ex)
                {
                    if (!dr.IsClosed)
                    {
                        dr.Close();
                    }
                    sb.Append(ex.StackTrace);
                    Literal lit = new Literal();
                    lit.Text = ex.Message;
                    this.Controls.Add(lit);
                }

            }

        }
    }
}

using System;
using System.Collections;
using System.Data;

using System.Web.UI.WebControls;
using System.Web.UI;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class WhatsNew : PortalModuleBase, IActionable
    {
        private Forum fi;

        #region DNN Actions
        public Entities.Modules.Actions.ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new Entities.Modules.Actions.ModuleActionCollection
                                  {
                                      {
                                          GetNextActionID(), Utilities.GetSharedResource("Configure"),
                                          Entities.Modules.Actions.ModuleActionType.AddContent, "", "",
                                          EditUrl(), false, Security.SecurityAccessLevel.Edit, true, false
                                      }
                                  };
                return Actions;
            }
        }
        #endregion

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            int timeOffset;
            timeOffset = PortalSettings.TimeZoneOffset;
            if (UserId > 0)
            {
                var dnnUser = Entities.Users.UserController.GetCurrentUserInfo();
                timeOffset = dnnUser.Profile.TimeZone;
                if (timeOffset == 0)
                {
                    timeOffset = PortalSettings.TimeZoneOffset;
                }
            }
            var lit = new Literal();
            var sb = new System.Text.StringBuilder();
            string forumids = "";
            bool TopicsOnly = false;
            bool RandomOrder = false;
            string Header = "";
            string Template = "";
            string Footer = "";
            int Rows = 10;
            bool bRSS = false;
            bool bRSSSecurity = false;
            bool bRSSIncludBody = false;
            string Tags = "";
            object obj = DataCache.CacheRetrieve("aftp" + ModuleId);
            Hashtable settings;
            if (obj == null)
            {
                settings = Entities.Portals.PortalSettings.GetModuleSettings(ModuleId);
                DataCache.CacheStore("aftp" + ModuleId, settings);
            }
            else
            {
                settings = (Hashtable)obj;
            }

            if (Convert.ToString(settings["AFTopPostsForums"]) != null)
            {
                forumids = Convert.ToString(settings["AFTopPostsForums"]);
            }
            if (Convert.ToString(settings["AFTopPostsNumber"]) != null)
            {
                Rows = Convert.ToInt32(settings["AFTopPostsNumber"]);
            }
            if (Convert.ToString(settings["AFTopPostsFormat"])!= null)
            {
                Template = Convert.ToString(settings["AFTopPostsFormat"]);
            }
            if (Convert.ToString(settings["AFTopPostsHeader"]) != null)
            {
                Header = Convert.ToString(settings["AFTopPostsHeader"]);
            }
            if (Convert.ToString(settings["AFTopPostsFooter"]) != null)
            {
                Footer = Convert.ToString(settings["AFTopPostsFooter"]);
            }
            if (Convert.ToString(settings["AFTopPostsTags"]) != null)
            {
                Tags = Convert.ToString(settings["AFTopPostsTags"]);
            }
            if (Convert.ToString(settings["AFTopPostsRSS"]) != null)
            {
                bRSS = Convert.ToBoolean(settings["AFTopPostsRSS"]);
            }
            if (Convert.ToString(settings["AFTopPostsTopicsOnly"]) != null)
            {
                TopicsOnly = Convert.ToBoolean(settings["AFTopPostsTopicsOnly"]);
            }
            if (Convert.ToString(settings["AFTopPostsRandomOrder"]) != null)
            {
                RandomOrder = Convert.ToBoolean(settings["AFTopPostsRandomOrder"]);
            }
            if (Convert.ToString(settings["AFTopPostsSecurity"]) != null)
            {
                bRSSSecurity = Convert.ToBoolean(settings["AFTopPostsSecurity"]);
            }
            if (Convert.ToString(settings["AFTopPostsBody"]) != null)
            {
                bRSSIncludBody = Convert.ToBoolean(settings["AFTopPostsBody"]);
            }
            //If Not CType(Settings["AFTopPostsCache"], String) Is Nothing Then
            //    txtCache.Text = CType(Settings["AFTopPostsCache"], String)
            //End If
            var uc = new UserController();
            User u = uc.GetUser(PortalId, -1);
            var db = new Data.Common();
            forumids = db.CheckForumIdsForView(forumids, u.UserRoles);

            IDataReader dr = DataProvider.Instance().GetPosts(PortalId, forumids, TopicsOnly, RandomOrder, Rows, UserId, false, UserInfo.IsSuperUser, Tags);
            sb.Append(Header);
            int BodyLength = -1;
            string BodyTrim = "";
            if (Template.Contains("[BODY:"))
            {
                int inStart = (Template.IndexOf("[BODY:", 0) + 1) + 5;
                int inEnd = (Template.IndexOf("]", inStart - 1) + 1) - 1;
                string sLength = Template.Substring(inStart, inEnd - inStart);
                BodyLength = Convert.ToInt32(sLength);
                BodyTrim = "[BODY:" + BodyLength.ToString() + "]";
            }
            bool useFriendly = Utilities.IsRewriteLoaded();
            string sHost = Utilities.GetHost();
            try
            {
                while (dr.Read())
                {
                    string sTempTemplate = Template;
                    string GroupName;
                    string GroupId;
                    string TopicTabId;
                    string TopicModuleId;
                    string ForumName;
                    string ForumId;
                    string Subject;
                    string UserName;
                    string PostDate;
                    string Body;
                    string BodyHTML;
                    string DisplayName;
                    string ReplyCount;
                    string LastPostDate;
                    string TopicId;
                    string ReplyId;
                    string FirstName;
                    string LastName;
                    string AuthorId;

                    string sForumUrl;
                    string sTopicURL;
                    string sGroupPrefixURL;

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

                    SettingsInfo ts = DataCache.MainSettings(Convert.ToInt32(TopicModuleId));
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
                    if (BodyTrim != string.Empty)
                    {
                        if (BodyLength > 0 & Body.Length > BodyLength)
                        {
                            sTempTemplate = sTempTemplate.Replace(BodyTrim, Body.Substring(0, BodyLength));
                        }
                        else
                        {
                            sTempTemplate = sTempTemplate.Replace(BodyTrim, Body);
                        }
                    }

                    sTempTemplate = sTempTemplate.Replace("[TOPICID]", TopicId);
                    sTempTemplate = sTempTemplate.Replace("[REPLYID]", ReplyId);
                    sTempTemplate = sTempTemplate.Replace("[REPLYCOUNT]", ReplyCount);

                    //Dim Expression As New Text.RegularExpressions.Regex("[{|\(]?[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}[\)|}]?$")
                    //If Expression.IsMatch(Subject) Then
                    //    Subject = Subject.Replace(Expression.Match(Subject).Value, String.Empty)
                    //End If



                    //UserProfiles.GetDisplayName(CInt(AuthorId), ts.UserNameDisplay, False, UserName, FirstName, LastName, DisplayName)
                    if (useFriendly && !(string.IsNullOrEmpty(sForumUrl) && string.IsNullOrEmpty(sTopicURL)))
                    {
                        var ctlUtils = new ControlUtils();
                        sTopicURL = ctlUtils.BuildUrl(Convert.ToInt32(TopicTabId), Convert.ToInt32(TopicModuleId), sGroupPrefixURL, sForumUrl, Convert.ToInt32(GroupId), Convert.ToInt32(ForumId), Convert.ToInt32(TopicId), sTopicURL, -1, -1, string.Empty, 1, -1);
                        sForumUrl = ctlUtils.BuildUrl(Convert.ToInt32(TopicTabId), Convert.ToInt32(TopicModuleId), sGroupPrefixURL, sForumUrl, Convert.ToInt32(GroupId), Convert.ToInt32(ForumId), -1, string.Empty, -1, -1, string.Empty, 1, -1);
                        if (sHost.EndsWith("/") && sForumUrl.StartsWith("/"))
                        {
                            sForumUrl = sForumUrl.Substring(1);
                        }
                        if (!(sForumUrl.StartsWith(sHost)))
                        {
                            sForumUrl = sHost + sForumUrl;
                        }

                        if (sHost.EndsWith("/") && sTopicURL.StartsWith("/"))
                        {
                            sTopicURL = sTopicURL.Substring(1);
                        }
                        if (!(sTopicURL.StartsWith(sHost)))
                        {
                            sTopicURL = sHost + sTopicURL;
                        }

                        if (Convert.ToInt32(ReplyId) == 0)
                        {
                            sTempTemplate = sTempTemplate.Replace("[POSTURL]", sTopicURL);
                            sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + sTopicURL + "\">" + Subject + "</a>");
                        }
                        else
                        {
                            if (!(sTopicURL.EndsWith("/")) && !(sTopicURL.EndsWith("aspx")))
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
                        if (Convert.ToInt32(ReplyId) == 0)
                        {
                            sTempTemplate = sTempTemplate.Replace("[POSTURL]", Common.Globals.NavigateURL(Convert.ToInt32(TopicTabId), "", new[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId }));
                            sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + Common.Globals.NavigateURL(Convert.ToInt32(TopicTabId), "", new[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId }) + "\">" + Subject + "</a>");
                        }
                        else
                        {
                            sTempTemplate = sTempTemplate.Replace("[POSTURL]", Common.Globals.NavigateURL(Convert.ToInt32(TopicTabId), "", new[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId }));
                            sTempTemplate = sTempTemplate.Replace("[SUBJECTLINK]", "<a href=\"" + Common.Globals.NavigateURL(Convert.ToInt32(TopicTabId), "", new[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId }) + "\">" + Subject + "</a>");
                        }
                        sTempTemplate = sTempTemplate.Replace("[TOPICSURL]", Common.Globals.NavigateURL(Convert.ToInt32(TopicTabId), "", new[] { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumId }));
                    }


                    sTempTemplate = sTempTemplate.Replace("[FORUMURL]", Common.Globals.NavigateURL(Convert.ToInt32(TopicTabId)));
                    sb.Append(sTempTemplate);


                }
                dr.Close();
            }
            catch (Exception ex)
            {
                if (!dr.IsClosed)
                {
                    dr.Close();
                }
                sb.Append(ex.StackTrace);
            }
            string sRSSImage = string.Empty;
            string sRSSURL = string.Empty;
            string sRSSIconLink = string.Empty;
            if (bRSS)
            {
                sRSSImage = "<img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/images/feedicon.gif") + "\" border=\"0\" />";
                sRSSURL = Page.ResolveUrl("~/desktopmodules/activeforumswhatsnew/feeds.aspx") + "?portalId=" + PortalId + "&tabid=" + TabId.ToString() + "&moduleid=" + ModuleId.ToString();
                sRSSIconLink = "<a href=\"" + sRSSURL + "\">" + sRSSImage + "</a>";
            }
            Footer = Footer.Replace("[RSSICON]", sRSSImage);
            Footer = Footer.Replace("[RSSURL]", sRSSURL);
            Footer = Footer.Replace("[RSSICONLINK]", sRSSIconLink);

            sb.Append(Footer);
            lit.Text = sb.ToString();
            Controls.Add(lit);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            var stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            base.Render(htmlWriter);
            string html = stringWriter.ToString();
            html = Utilities.LocalizeControl(html, "WhatsNew");
            writer.Write(html);
        }
    }
}

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

using DotNetNuke.Modules.ActiveForums.Constants;
using DotNetNuke.Modules.ActiveForums.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:TopicView runat=server></{0}:TopicView>")]
    public class TopicView : ForumBase
    {
        #region Private Members

        private string _metaTemplate = "[META][TITLE][TOPICSUBJECT] - [PORTALNAME] - [PAGENAME] - [GROUPNAME] - [FORUMNAME][/TITLE][DESCRIPTION][BODY:255][/DESCRIPTION][KEYWORDS][TAGS][VALUE][/KEYWORDS][/META]";
        private string _metaTitle = string.Empty;
        private string _metaDescription = string.Empty;
        private string _metaKeywords = string.Empty;
        private string _forumName;
        private string _groupName;
        private int _topicTemplateId;
        private DataRow _drForum;
        private DataRow _drSecurity;
        private DataTable _dtTopic;
        private DataTable _dtAttach;
        private bool _bRead;
        private bool _bAttach;
        private bool _bTrust;
        private bool _bDelete;
        private bool _bEdit;
        private bool _bLock;
        private bool _bPin;
        private bool _bSubscribe;
        private bool _bModApprove;
        private bool _bModSplit;
        private bool _bModDelete;
        private bool _bModEdit;
        private bool _bModMove;
        private bool _bAllowRSS;
        private int _rowIndex;
        private int _pageSize = 20;
        private string _myTheme = "_default";
        private string _myThemePath = string.Empty;
        private bool _bLocked;
        private int _topicType;
        private string _topicSubject = string.Empty;
        private string _topicDescription = string.Empty;
        private int _viewCount;
        private int _replyCount;
        private int _rowCount;
        private int _statusId;
        private int _topicAuthorId;
        private string _topicAuthorDisplayName = string.Empty;
        private string _topicDateCreated = string.Empty;
        private string _memberListMode = string.Empty;
        private string _profileVisibility = string.Empty;
        private bool _enablePoints;
        private bool _trustDefault;
        private bool _isTrusted;
        private int _topicRating;
        private bool _allowHTML;
        private bool _allowLikes;
        private bool _allowScript;
        private bool _allowSubscribe;
        private int _nextTopic;
        private int _prevTopic;
        private bool _isSubscribedTopic;
        private string _lastPostDate;
        private readonly Author _lastPostAuthor = new Author();
        private string _defaultSort;
        private int _editInterval;
        private string _tags = string.Empty;
        private string _topicURL = string.Empty;
        private string _template = string.Empty;
        private string _topicData = string.Empty;
        private bool _useListActions;

        #endregion

        #region Public Properties

        public string TopicTemplate
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
            }
        }

        public int OptPageSize { get; set; }
        public string OptDefaultSort { get; set; }
        public string MetaTemplate
        {
            get
            {
                return _metaTemplate;
            }
            set
            {
                _metaTemplate = value;
            }
        }
        public string MetaTitle
        {
            get
            {
                return _metaTitle;
            }
            set
            {
                _metaTitle = value;
            }
        }
        public string MetaDescription
        {
            get
            {
                return _metaDescription;
            }
            set
            {
                _metaDescription = value;
            }
        }
        public string MetaKeywords
        {
            get
            {
                return _metaKeywords;
            }
            set
            {
                _metaKeywords = value;
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (ForumInfo == null)
                ForumInfo = new ForumController().Forums_Get(PortalId, ForumModuleId, ForumId, UserId, true, false, TopicId);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (!Page.IsPostBack)
                {
                    // Handle an old URL form if needed
                    var sURL = Request.RawUrl;
                    if (!string.IsNullOrWhiteSpace(sURL) && sURL.Contains("view") && sURL.Contains("postid") && sURL.Contains("forumid"))
                    {
                        sURL = sURL.Replace("view", ParamKeys.ViewType);
                        sURL = sURL.Replace("postid", ParamKeys.TopicId);
                        sURL = sURL.Replace("forumid", ParamKeys.ForumId);
                        Response.Status = "301 Moved Permanently";
                        Response.AddHeader("Location", sURL);
                    }

                    // We must have a forum id
                    if (ForumId < 1)
                    {
                        Response.Redirect(Utilities.NavigateUrl(TabId));
                    }
                }

                // Redirect to the first new post if the first new post param is found in the url
                // Note that this should probably be the default behavior unless a page is specified.
                var lastPostRead = Utilities.SafeConvertInt(Request.Params[ParamKeys.FirstNewPost]);
                if (lastPostRead > 0)
                {
                    var firstUnreadPost = DataProvider.Instance().Utility_GetFirstUnRead(TopicId, Convert.ToInt32(Request.Params[ParamKeys.FirstNewPost]));
                    if (firstUnreadPost > lastPostRead)
                    {
                        var tURL = Utilities.NavigateUrl(TabId, "", new[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=topic", ParamKeys.ContentJumpId + "=" + firstUnreadPost });
                        if (MainSettings.UseShortUrls)
                            tURL = Utilities.NavigateUrl(TabId, "", new[] { ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + firstUnreadPost });

                        Response.Redirect(tURL);
                    }
                }


                AppRelativeVirtualPath = "~/";

                _myTheme = MainSettings.Theme;
                _myThemePath = Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + _myTheme);
                //_allowAvatars = !UserPrefHideAvatars;
                _enablePoints = MainSettings.EnablePoints;
                _editInterval = MainSettings.EditInterval;

                // Get our default sort
                // Try the OptDefaultSort Value First
                _defaultSort = OptDefaultSort;
                if (string.IsNullOrWhiteSpace(_defaultSort))
                {
                    // Next, try getting the sort from the query string
                    _defaultSort = (Request.Params[ParamKeys.Sort] + string.Empty).Trim().ToUpperInvariant();
                    if (string.IsNullOrWhiteSpace(_defaultSort) || (_defaultSort != "ASC" && _defaultSort != "DESC"))
                    {
                        // If we still don't have a valid sort, try and use the value from the users profile
                        _defaultSort = (ForumUser.Profile.PrefDefaultSort + string.Empty).Trim().ToUpper();
                        if (string.IsNullOrWhiteSpace(_defaultSort) || (_defaultSort != "ASC" && _defaultSort != "DESC"))
                        {
                            // No other option than to just use ASC
                            _defaultSort = "ASC";
                        }
                    }
                }

                LoadData(PageId);

                BindTopic();

                _allowSubscribe = Request.IsAuthenticated && _bSubscribe;

                var tempVar = BasePage;
                Environment.UpdateMeta(ref tempVar, MetaTitle, MetaDescription, MetaKeywords);
            }
            catch (Exception ex)
            {
                RenderMessage("[RESX:Error:LoadingTopic]", ex.Message, ex);
            }
        }

        #endregion

        #region Private Methods

        private void LoadData(int pageId)
        {
            // Get our page size
            _pageSize = OptPageSize;
            if (_pageSize <= 0)
            {
                _pageSize = UserId > 0 ? UserDefaultPageSize : MainSettings.PageSize;
                if (_pageSize < 5)
                    _pageSize = 10;
            }

            // If we are in print mode, set the page size to maxValue
            if (!string.IsNullOrWhiteSpace(Request.QueryString["dnnprintmode"]))
                _pageSize = int.MaxValue;

            // If our pagesize is maxvalue, we can only have one page
            if (_pageSize == int.MaxValue)
                pageId = 1;

            // Get our Row Index
            _rowIndex = (pageId - 1) * _pageSize;

            var ds = DataProvider.Instance().UI_TopicView(PortalId, ModuleId, ForumId, TopicId, UserId, _rowIndex, _pageSize, UserInfo.IsSuperUser, _defaultSort);

            // Test for a proper dataset
            if (ds.Tables.Count < 4 || ds.Tables[0].Rows.Count == 0 || ds.Tables[1].Rows.Count == 0)
                Response.Redirect(Utilities.NavigateUrl(TabId));

            // Load our values
            _drForum = ds.Tables[0].Rows[0];
            _drSecurity = ds.Tables[1].Rows[0];
            _dtTopic = ds.Tables[2];
            _dtAttach = ds.Tables[3];

            // If we don't have any rows to display, redirect
            if (_dtTopic.Rows.Count == 0)
            {
                if (pageId > 1)
                {
                    if (MainSettings.UseShortUrls)
                        Response.Redirect(Utilities.NavigateUrl(TabId, string.Empty, new[] { ParamKeys.TopicId + "=" + TopicId }), true);
                    else
                        Response.Redirect(Utilities.NavigateUrl(TabId, string.Empty, new[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId }), true);
                }
                else
                {
                    if (MainSettings.UseShortUrls)
                        Response.Redirect(Utilities.NavigateUrl(TabId, string.Empty, new[] { ParamKeys.ForumId + "=" + ForumId }), true);
                    else
                        Response.Redirect(Utilities.NavigateUrl(TabId, string.Empty, new[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topics }), true);
                }
            }

            // first make sure we have read permissions, otherwise we need to redirect
            _bRead = Permissions.HasPerm(_drSecurity["CanRead"].ToString(), ForumUser.UserRoles);

            if (!_bRead)
            {
                var settings = Entities.Portals.PortalController.GetCurrentPortalSettings();
                if (settings.LoginTabId > 0)
                    Response.Redirect(Common.Globals.NavigateURL(settings.LoginTabId, "", "returnUrl=" + Request.RawUrl), true);
                else
                    Response.Redirect(Utilities.NavigateUrl(TabId, "", "ctl=login&returnUrl=" + Request.RawUrl), true);
            }


            //bCreate = Permissions.HasPerm(drSecurity["CanCreate"].ToString(), ForumUser.UserRoles);
            _bEdit = Permissions.HasPerm(_drSecurity["CanEdit"].ToString(), ForumUser.UserRoles);
            _bDelete = Permissions.HasPerm(_drSecurity["CanDelete"].ToString(), ForumUser.UserRoles);
            //bReply = Permissions.HasPerm(drSecurity["CanReply"].ToString(), ForumUser.UserRoles);
            //bPoll = Permissions.HasPerm(_drSecurity["CanPoll"].ToString(), ForumUser.UserRoles);
            _bAttach = Permissions.HasPerm(_drSecurity["CanAttach"].ToString(), ForumUser.UserRoles);
            _bSubscribe = Permissions.HasPerm(_drSecurity["CanSubscribe"].ToString(), ForumUser.UserRoles);
            // bModMove = Permissions.HasPerm(_drSecurity["CanModMove"].ToString(), ForumUser.UserRoles);
            _bModSplit = Permissions.HasPerm(_drSecurity["CanModSplit"].ToString(), ForumUser.UserRoles);
            _bModDelete = Permissions.HasPerm(_drSecurity["CanModDelete"].ToString(), ForumUser.UserRoles);
            _bModApprove = Permissions.HasPerm(_drSecurity["CanModApprove"].ToString(), ForumUser.UserRoles);
            _bTrust = Permissions.HasPerm(_drSecurity["CanTrust"].ToString(), ForumUser.UserRoles);
            _bModEdit = Permissions.HasPerm(_drSecurity["CanModEdit"].ToString(), ForumUser.UserRoles);
            _bModMove = Permissions.HasPerm(_drSecurity["CanModMove"].ToString(), ForumUser.UserRoles);

            _isTrusted = Utilities.IsTrusted((int)ForumInfo.DefaultTrustValue, ForumUser.TrustLevel, Permissions.HasPerm(ForumInfo.Security.Trust, ForumUser.UserRoles));

            _forumName = _drForum["ForumName"].ToString();
            _groupName = _drForum["GroupName"].ToString();
            ForumGroupId = Utilities.SafeConvertInt(_drForum["ForumGroupId"]);
            _topicTemplateId = Utilities.SafeConvertInt(_drForum["TopicTemplateId"]);
            _bAllowRSS = Utilities.SafeConvertBool(_drForum["AllowRSS"]);
            _bLocked = Utilities.SafeConvertBool(_drForum["IsLocked"]);
            _topicType = Utilities.SafeConvertInt(_drForum["TopicType"]);
            _statusId = Utilities.SafeConvertInt(_drForum["StatusId"]);
            _topicSubject = _drForum["Subject"].ToString();
            _topicDescription = Utilities.StripHTMLTag(_drForum["Body"].ToString());
            _tags = _drForum["Tags"].ToString();
            _viewCount = Utilities.SafeConvertInt(_drForum["ViewCount"]);
            _replyCount = Utilities.SafeConvertInt(_drForum["ReplyCount"]);
            _topicAuthorId = Utilities.SafeConvertInt(_drForum["AuthorId"]);
            _topicAuthorDisplayName = _drForum["TopicAuthor"].ToString();
            _topicRating = Utilities.SafeConvertInt(_drForum["TopicRating"]);
            _allowHTML = Utilities.SafeConvertBool(_drForum["AllowHTML"]);
            _allowLikes = Utilities.SafeConvertBool(_drForum["AllowLikes"]);
            _allowScript = Utilities.SafeConvertBool(_drForum["AllowScript"]);
            _rowCount = Utilities.SafeConvertInt(_drForum["ReplyCount"]) + 1;
            _nextTopic = Utilities.SafeConvertInt(_drForum["NextTopic"]);
            _prevTopic = Utilities.SafeConvertInt(_drForum["PrevTopic"]);
            _lastPostDate = GetDate(Utilities.SafeConvertDateTime(_drForum["LastPostDate"]));
            _lastPostAuthor.AuthorId = Utilities.SafeConvertInt(_drForum["LastPostAuthorId"]);
            _lastPostAuthor.DisplayName = _drForum["LastPostDisplayName"].ToString();
            _lastPostAuthor.FirstName = _drForum["LastPostFirstName"].ToString();
            _lastPostAuthor.LastName = _drForum["LastPostLastName"].ToString();
            _lastPostAuthor.Username = _drForum["LastPostUserName"].ToString();
            _topicURL = _drForum["URL"].ToString();
            _topicDateCreated = Utilities.GetDate(Utilities.SafeConvertDateTime(_drForum["DateCreated"]), ForumModuleId, TimeZoneOffset);
            _topicData = _drForum["TopicData"].ToString();
            _isSubscribedTopic = UserId > 0 && Utilities.SafeConvertInt(_drForum["IsSubscribedTopic"]) > 0;

            if (Page.IsPostBack)
                return;

            // If a content jump id was passed it, we need to calulate a page and then jump to it with an ancor
            // otherwise, we don't need to do anything 

            var contentJumpId = Utilities.SafeConvertInt(Request.Params[ParamKeys.ContentJumpId], -1);
            if (contentJumpId < 0)
                return;

            var sTarget = "#" + contentJumpId;

            var sURL = string.Empty;

            if (MainSettings.URLRewriteEnabled && Request.QueryString["asg"] == null && !string.IsNullOrEmpty(_topicURL))
            {
                var db = new Data.Common();
                sURL = db.GetUrl(ModuleId, ForumGroupId, ForumId, TopicId, UserId, contentJumpId);

                if (!(sURL.StartsWith("/")))
                    sURL = "/" + sURL;

                if (!(sURL.EndsWith("/")))
                    sURL += "/";

                sURL += sTarget;
            }

            if (string.IsNullOrEmpty(sURL))
            {

                var @params = new List<string> { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=" + Views.Topic };
                if (MainSettings.UseShortUrls)
                    @params = new List<string> { ParamKeys.TopicId + "=" + TopicId };

                var intPages = Convert.ToInt32(Math.Ceiling(_rowCount / (double)_pageSize));
                if (intPages > 1)
                    @params.Add(ParamKeys.PageJumpId + "=" + intPages);

                if (SocialGroupId > 0)
                    @params.Add("GroupId=" + SocialGroupId.ToString());

                sURL = Utilities.NavigateUrl(TabId, "", @params.ToArray()) + sTarget;
            }

            if (Request.IsAuthenticated)
                Response.Redirect(sURL, true);

            // Not sure why we're doing this.  I assume it may have something to do with search engines - JB
            Response.Status = "301 Moved Permanently";
            Response.AddHeader("Location", sURL);
        }

        private void BindTopic()
        {
            string sOutput;

            var bFullTopic = true;

            // Load the proper template into out output variable
            if (!string.IsNullOrWhiteSpace(TopicTemplate))
            {
                // Note:  The template may be set in the topic review section of the Post form.
                bFullTopic = false;
                sOutput = TopicTemplate;
                sOutput = Utilities.ParseSpacer(sOutput);
            }
            else if (UseTemplatePath && TemplatePath != string.Empty)
            {
                sOutput = Utilities.GetFileContent(TemplatePath + "TopicView.htm");
                sOutput = Utilities.ParseSpacer(sOutput);
            }
            else
            {
                sOutput = DataCache.GetCachedTemplate(MainSettings.TemplateCache, ModuleId, "TopicView", _topicTemplateId);
            }

            // Handle the postinfo token if present
            if (sOutput.Contains("[POSTINFO]") && ForumInfo.ProfileTemplateId > 0)
                sOutput = sOutput.Replace("[POSTINFO]", DataCache.GetCachedTemplate(MainSettings.TemplateCache, ModuleId, "ProfileInfo", ForumInfo.ProfileTemplateId));

            // Run some basic rpleacements
            sOutput = sOutput.Replace("[PORTALID]", PortalId.ToString());
            sOutput = sOutput.Replace("[MODULEID]", ForumModuleId.ToString());
            sOutput = sOutput.Replace("[TABID]", ForumTabId.ToString());
            sOutput = sOutput.Replace("[TOPICID]", TopicId.ToString());
            sOutput = sOutput.Replace("[AF:CONTROL:FORUMID]", ForumId.ToString());
            sOutput = sOutput.Replace("[AF:CONTROL:FORUMGROUPID]", ForumGroupId.ToString());
            sOutput = sOutput.Replace("[AF:CONTROL:PARENTFORUMID]", ParentForumId.ToString());

            // Add Topic Scripts
            var ctlTopicScripts = (af_topicscripts)(LoadControl("~/DesktopModules/ActiveForums/controls/af_topicscripts.ascx"));
            ctlTopicScripts.ModuleConfiguration = ModuleConfiguration;
            Controls.Add(ctlTopicScripts);

            // Pretty sure this is no longer used
            /*
            if (sOutput.Contains("<am:TopicsNavigator"))
            {
                var ctl = ParseControl(sOutput);
                
                if(ctl != null)
                    Controls.Add(ctl);

                LinkControls(Controls);

                return;
            }
            */

            #region Build Topic Properties

            if (sOutput.Contains("[AF:PROPERTIES"))
            {
                var sProps = string.Empty;

                if (!string.IsNullOrWhiteSpace(_topicData))
                {
                    var sPropTemplate = TemplateUtils.GetTemplateSection(sOutput, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");

                    try
                    {
                        var xDoc = new XmlDocument();
                        xDoc.LoadXml(_topicData);
                        var xRoot = xDoc.DocumentElement;
                        var xNodeList = (xRoot != null) ? xRoot.SelectNodes("//properties/property") : null;
                        if (xNodeList != null && xNodeList.Count > 0)
                        {
                            for (var i = 0; i < xNodeList.Count; i++)
                            {
                                var pName = Utilities.HTMLDecode(xNodeList[i].ChildNodes[0].InnerText);
                                var pValue = Utilities.HTMLDecode(xNodeList[i].ChildNodes[1].InnerText);

                                // This builds the replacement text for the properties template
                                var tmp = sPropTemplate.Replace("[AF:PROPERTY:LABEL]", "[RESX:" + pName + "]");
                                tmp = tmp.Replace("[AF:PROPERTY:VALUE]", pValue);
                                sProps += tmp;

                                // This deals with any specific property tokens that may be present outside of the normal properties template
                                sOutput = sOutput.Replace("[AF:PROPERTY:" + pName + ":LABEL]", Utilities.GetSharedResource("[RESX:" + pName + "]"));
                                sOutput = sOutput.Replace("[AF:PROPERTY:" + pName + ":VALUE]", pValue);
                                var pValueKey = string.IsNullOrWhiteSpace(pValue) ? string.Empty : Utilities.CleanName(pValue).ToLowerInvariant();
                                sOutput = sOutput.Replace("[AF:PROPERTY:" + pName + ":VALUEKEY]", pValueKey);
                            }
                        }
                    }
                    catch (XmlException)
                    {
                        // Property XML is invalid
                        // Nothing to do in this case but ignore the issue.
                    }
                }

                sOutput = TemplateUtils.ReplaceSubSection(sOutput, sProps, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");
            }

            #endregion

            #region Populate Metadata

            // If the template contains a meta template, grab it then remove the token
            if (sOutput.Contains("[META]"))
            {
                MetaTemplate = TemplateUtils.GetTemplateSection(sOutput, "[META]", "[/META]");
                sOutput = TemplateUtils.ReplaceSubSection(sOutput, string.Empty, "[META]", "[/META]");
            }

            //Parse Meta Template
            if (!string.IsNullOrEmpty(MetaTemplate))
            {
                MetaTemplate = MetaTemplate.Replace("[FORUMNAME]", _forumName);
                MetaTemplate = MetaTemplate.Replace("[GROUPNAME]", _groupName);

                var settings = Entities.Portals.PortalController.GetCurrentPortalSettings();
                var pageName = (settings.ActiveTab.Title.Length == 0)
                                   ? Server.HtmlEncode(settings.ActiveTab.TabName)
                                   : Server.HtmlEncode(settings.ActiveTab.Title);

                MetaTemplate = MetaTemplate.Replace("[PAGENAME]", pageName);
                MetaTemplate = MetaTemplate.Replace("[PORTALNAME]", settings.PortalName);
                MetaTemplate = MetaTemplate.Replace("[TAGS]", _tags);

                // Subject
                if (MetaTemplate.Contains("[TOPICSUBJECT:"))
                {
                    const string pattern = "(\\[TOPICSUBJECT:(.+?)\\])";
                    foreach (Match m in Regex.Matches(MetaTemplate, pattern))
                    {
                        var maxLength = Utilities.SafeConvertInt(m.Groups[2].Value, 255);
                        if (_topicSubject.Length > maxLength)
                            MetaTemplate = MetaTemplate.Replace(m.Value, _topicSubject.Substring(0, maxLength) + "...");
                        else
                            MetaTemplate = MetaTemplate.Replace(m.Value, Utilities.StripHTMLTag(_topicSubject));
                    }
                }
                MetaTemplate = MetaTemplate.Replace("[TOPICSUBJECT]", Utilities.StripHTMLTag(_topicSubject));

                // Body
                if (MetaTemplate.Contains("[BODY:"))
                {
                    const string pattern = "(\\[BODY:(.+?)\\])";
                    foreach (Match m in Regex.Matches(MetaTemplate, pattern))
                    {
                        var maxLength = Utilities.SafeConvertInt(m.Groups[2].Value, 512);
                        if (_topicDescription.Length > maxLength)
                            MetaTemplate = MetaTemplate.Replace(m.Value, _topicDescription.Substring(0, maxLength) + "...");
                        else
                            MetaTemplate = MetaTemplate.Replace(m.Value, _topicDescription);
                    }
                }
                MetaTemplate = MetaTemplate.Replace("[BODY]", _topicDescription);

                MetaTitle = TemplateUtils.GetTemplateSection(MetaTemplate, "[TITLE]", "[/TITLE]").Replace("[TITLE]", string.Empty).Replace("[/TITLE]", string.Empty);
                MetaTitle = MetaTitle.TruncateAtWord(SEOConstants.MaxMetaTitleLength);
                MetaDescription = TemplateUtils.GetTemplateSection(MetaTemplate, "[DESCRIPTION]", "[/DESCRIPTION]").Replace("[DESCRIPTION]", string.Empty).Replace("[/DESCRIPTION]", string.Empty);
                MetaDescription = MetaDescription.TruncateAtWord(SEOConstants.MaxMetaDescriptionLength);
                MetaKeywords = TemplateUtils.GetTemplateSection(MetaTemplate, "[KEYWORDS]", "[/KEYWORDS]").Replace("[KEYWORDS]", string.Empty).Replace("[/KEYWORDS]", string.Empty);
            }


            #endregion

            #region Setup Breadcrumbs

            var breadCrumb = TemplateUtils.GetTemplateSection(sOutput, "[BREADCRUMB]", "[/BREADCRUMB]").Replace("[BREADCRUMB]", string.Empty).Replace("[/BREADCRUMB]", string.Empty);

            if (MainSettings.UseSkinBreadCrumb)
            {
                var ctlUtils = new ControlUtils();

                var groupUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, string.Empty, ForumInfo.ForumGroupId, -1, -1, -1, string.Empty, 1, -1, SocialGroupId);
                var forumUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, -1, -1, string.Empty, 1, -1, SocialGroupId);
                var topicUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, _topicURL, -1, -1, string.Empty, 1, -1, SocialGroupId);

                var sCrumb = "<a href=\"" + groupUrl + "\">" + _groupName + "</a>|";
                sCrumb += "<a href=\"" + forumUrl + "\">" + _forumName + "</a>";
                sCrumb += "|<a href=\"" + topicUrl + "\">" + _topicSubject + "</a>";

                if (Environment.UpdateBreadCrumb(Page.Controls, sCrumb))
                    breadCrumb = string.Empty;
            }

            sOutput = TemplateUtils.ReplaceSubSection(sOutput, breadCrumb, "[BREADCRUMB]", "[/BREADCRUMB]");

            #endregion

            // Parse Common Controls First
            sOutput = ParseControls(sOutput);

            // Note: If the containing element is not found, GetTemplateSection returns the entire template
            // This is desired behavior in this case as it's possible that the entire template is our topics container.
            var topic = TemplateUtils.GetTemplateSection(sOutput, "[AF:CONTROL:CALLBACK]", "[/AF:CONTROL:CALLBACK]");

            topic = ParseTopic(topic);

            if (!topic.Contains(Globals.ControlRegisterTag))
                topic = Globals.ControlRegisterTag + topic;

            topic = Utilities.LocalizeControl(topic);

            // If a template was passed in, we don't need to do this.
            if (bFullTopic)
            {
                sOutput = TemplateUtils.ReplaceSubSection(sOutput, "<asp:placeholder id=\"plhTopic\" runat=\"server\" />", "[AF:CONTROL:CALLBACK]", "[/AF:CONTROL:CALLBACK]");

                sOutput = Utilities.LocalizeControl(sOutput);
                sOutput = Utilities.StripTokens(sOutput);

                // If we added a banner, make sure we register than banner tag
                if (sOutput.Contains("<dnn:BANNER") && !sOutput.Contains(Globals.BannerRegisterTag))
                    sOutput = Globals.BannerRegisterTag + sOutput;

                var ctl = ParseControl(sOutput);
                if (ctl != null)
                    Controls.Add(ctl);
            }

            // Create a topic placeholder if we don't have one.
            var plhTopic = FindControl("plhTopic") as PlaceHolder;
            if (plhTopic == null)
            {
                plhTopic = new PlaceHolder { ID = "plhTopic" };
                Controls.Add(plhTopic);
            }

            // Parse and add out topic control

            // If we added a banner, make sure we register than banner tag
            // This has to be done separately from sOutput because they are usually different.
            if (topic.Contains("<dnn:BANNER") && !topic.Contains(Globals.BannerRegisterTag))
                topic = Globals.BannerRegisterTag + topic;

            var ctlTopic = ParseControl(topic);
            if (ctlTopic != null)
                plhTopic.Controls.Add(ctlTopic);

            //Add helper controls
            //Quick Jump DropDownList
            var plh = FindControl("plhQuickJump") as PlaceHolder;
            if (plh != null)
            {
                plh.Controls.Clear();
                var ctlForumJump = new af_quickjump
                {
                    ModuleConfiguration = ModuleConfiguration,
                    MOID = ModuleId,
                    dtForums = null,
                    ForumId = ForumId,
                    EnableViewState = false,
                    ForumInfo = ForumId > 0 ? ForumInfo : null
                };

                if (!(plh.Controls.Contains(ctlForumJump)))
                {
                    plh.Controls.Add(ctlForumJump);
                }

            }

            //Poll Container
            plh = FindControl("plhPoll") as PlaceHolder;
            if (plh != null)
            {
                plh.Controls.Clear();
                plh.Controls.Add(new af_polls
                {
                    ModuleConfiguration = ModuleConfiguration,
                    TopicId = TopicId,
                    ForumId = ForumId
                });
            }

            //Quick Reply
            if (CanRead && _bLocked == false)
            {
                plh = FindControl("plhQuickReply") as PlaceHolder;
                if (plh != null)
                {
                    plh.Controls.Clear();
                    var ctlQuickReply = (af_quickreplyform)(LoadControl("~/DesktopModules/ActiveForums/controls/af_quickreply.ascx"));
                    ctlQuickReply.ModuleConfiguration = ModuleConfiguration;
                    ctlQuickReply.CanTrust = _bTrust;
                    ctlQuickReply.ModApprove = _bModApprove;
                    ctlQuickReply.IsTrusted = _isTrusted;
                    ctlQuickReply.Subject = _topicSubject;
                    ctlQuickReply.AllowSubscribe = _allowSubscribe;
                    ctlQuickReply.AllowHTML = _allowHTML;
                    ctlQuickReply.AllowScripts = _allowScript;
                    ctlQuickReply.ForumId = ForumId;
                    ctlQuickReply.SocialGroupId = SocialGroupId;
                    ctlQuickReply.ForumModuleId = ForumModuleId;
                    ctlQuickReply.ForumTabId = TabId;

                    if (ForumId > 0)
                        ctlQuickReply.ForumInfo = ForumInfo;

                    plh.Controls.Add(ctlQuickReply);
                }
            }

            // Topic Sort
            plh = FindControl("plhTopicSort") as PlaceHolder;
            if (plh != null)
            {
                plh.Controls.Clear();
                var ctlTopicSort = (af_topicsorter)(LoadControl("~/DesktopModules/ActiveForums/controls/af_topicsort.ascx"));
                ctlTopicSort.ModuleConfiguration = ModuleConfiguration;
                ctlTopicSort.ForumId = ForumId;
                ctlTopicSort.DefaultSort = _defaultSort;
                if (ForumId > 0)
                    ctlTopicSort.ForumInfo = ForumInfo;

                plh.Controls.Add(ctlTopicSort);
            }

            // Topic Status
            plh = FindControl("plhStatus") as PlaceHolder;
            if (plh != null)
            {
                plh.Controls.Clear();
                var ctlTopicStatus = (af_topicstatus)(LoadControl("~/DesktopModules/ActiveForums/controls/af_topicstatus.ascx"));
                ctlTopicStatus.ModuleConfiguration = ModuleConfiguration;
                ctlTopicStatus.Status = _statusId;
                ctlTopicStatus.ForumId = ForumId;
                if (ForumId > 0)
                    ctlTopicStatus.ForumInfo = ForumInfo;

                plh.Controls.Add(ctlTopicStatus);
            }

            BuildPager();
        }

        /*
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                if ((ctrl) is ForumBase)
                {
                    ((ForumBase)ctrl).ModuleConfiguration = this.ModuleConfiguration;
                    ((ForumBase)ctrl).TopicId = TopicId;
                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }*/

        private string ParseControls(string sOutput)
        {
            // Do a few things before we switch to a string builder

            // Add This
            if (sOutput.Contains("[AF:CONTROL:ADDTHIS"))
            {
                var strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(Request));
                sOutput = TemplateUtils.ParseSpecial(sOutput, SpecialTokenTypes.AddThis, strHost + Request.RawUrl, _topicSubject, _bRead, MainSettings.AddThisAccount);
            }

            // Banners
            if (sOutput.Contains("[BANNER"))
            {
                sOutput = sOutput.Replace("[BANNER]", "<dnn:BANNER runat=\"server\" GroupName=\"FORUMS\" BannerCount=\"1\" EnableViewState=\"False\" />");

                const string pattern = @"(\[BANNER:(.+?)\])";
                const string sBanner = "<dnn:BANNER runat=\"server\" BannerCount=\"1\" GroupName=\"$1\" EnableViewState=\"False\" />";

                sOutput = Regex.Replace(sOutput, pattern, sBanner);
            }

            // Hide Toolbar

            if (sOutput.Contains("[NOTOOLBAR]"))
            {
                if (HttpContext.Current.Items.Contains("ShowToolbar"))
                {
                    HttpContext.Current.Items["ShowToolbar"] = false;
                }
                else
                {
                    HttpContext.Current.Items.Add("ShowToolbar", false);
                }
            }

            // Now use the string builder to do all replacements
            var sbOutput = new StringBuilder(sOutput);

            if (Request.QueryString["dnnprintmode"] != null)
            {
                sbOutput.Replace("[ADDREPLY]", string.Empty);
                sbOutput.Replace("[QUICKREPLY]", string.Empty);
                sbOutput.Replace("[TOPICSUBSCRIBE]", string.Empty);
                sbOutput.Replace("[AF:CONTROL:PRINTER]", string.Empty);
                sbOutput.Replace("[AF:CONTROL:EMAIL]", string.Empty);
                sbOutput.Replace("[PAGER1]", string.Empty);
                sbOutput.Replace("[PAGER2]", string.Empty);
                sbOutput.Replace("[SORTDROPDOWN]", string.Empty);
                sbOutput.Replace("[POSTRATINGBUTTON]", string.Empty);
                sbOutput.Replace("[JUMPTO]", string.Empty);
                sbOutput.Replace("[NEXTTOPIC]", string.Empty);
                sbOutput.Replace("[PREVTOPIC]", string.Empty);
                sbOutput.Replace("[AF:CONTROL:STATUS]", string.Empty);
                sbOutput.Replace("[ACTIONS:DELETE]", string.Empty);
                sbOutput.Replace("[ACTIONS:EDIT]", string.Empty);
                sbOutput.Replace("[ACTIONS:QUOTE]", string.Empty);
                sbOutput.Replace("[ACTIONS:REPLY]", string.Empty);
                sbOutput.Replace("[ACTIONS:ANSWER]", string.Empty);
                sbOutput.Replace("[ACTIONS:ALERT]", string.Empty);
                sbOutput.Replace("[ACTIONS:MOVE]", string.Empty);
                sbOutput.Replace("[RESX:SortPosts]:", string.Empty);
                sbOutput.Append("<img src=\"~/desktopmodules/activeforums/images/spacer.gif\" width=\"800\" height=\"1\" runat=\"server\" alt=\"---\" />");
            }


            sbOutput.Replace("[NOPAGING]", "<script type=\"text/javascript\">afpagesize=" + int.MaxValue + ";</script>");
            sbOutput.Replace("[NOTOOLBAR]", string.Empty);

            // Subscribe Option
            if (_bSubscribe)
            {
                var subControl = new ToggleSubscribe(1, ForumId, TopicId);
                subControl.Checked = _isSubscribedTopic;
                subControl.Text = "[RESX:TopicSubscribe:" + _isSubscribedTopic.ToString().ToUpper() + "]";
                sbOutput.Replace("[TOPICSUBSCRIBE]", subControl.Render());
            }
            else
            {
                sbOutput.Replace("[TOPICSUBSCRIBE]", string.Empty);
            }

            // Topic and post actions
            var tc = new TokensController();
            var topicActions = tc.TokenGet("topic", "[AF:CONTROL:TOPICACTIONS]");
            var postActions = tc.TokenGet("topic", "[AF:CONTROL:POSTACTIONS]");
            if (sOutput.Contains("[AF:CONTROL:TOPICACTIONS]"))
            {
                _useListActions = true;
                sbOutput.Replace("[AF:CONTROL:TOPICACTIONS]", topicActions);
                sbOutput.Replace("[AF:CONTROL:POSTACTIONS]", postActions);
            }


            // Quick Reply
            if (_bLocked)
            {
                sbOutput.Replace("[ADDREPLY]", "<span class=\"afnormal\">[RESX:TopicLocked]</span>");
                sbOutput.Replace("[QUICKREPLY]", string.Empty);
            }
            else
            {
                //TODO: Check for owner
                if (CanReply)
                {
                    var @params = new List<string> { ParamKeys.ViewType + "=post", ParamKeys.TopicId + "=" + TopicId, ParamKeys.ForumId + "=" + ForumId };
                    if (SocialGroupId > 0)
                        @params.Add("GroupId=" + SocialGroupId);

                    sbOutput.Replace("[ADDREPLY]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", @params.ToArray()) + "\" class=\"dnnPrimaryAction\">[RESX:AddReply]</a>");
                    sbOutput.Replace("[QUICKREPLY]", "<asp:placeholder id=\"plhQuickReply\" runat=\"server\" />");
                }
                else
                {
                    sbOutput.Replace("[ADDREPLY]", "<span class=\"afnormal\">[RESX:NotAuthorizedReply]</span>");
                    sbOutput.Replace("[QUICKREPLY]", string.Empty);
                }

                //TODO: Check for owner
            }

            if (_bModSplit && (_replyCount > 0))
            {
                /*var @params = new List<string> { ParamKeys.ViewType + "=post", ParamKeys.TopicId + "=" + TopicId, ParamKeys.ForumId + "=" + ForumId };*/
                sbOutput.Replace("[SPLITBUTTONS]", "<div id=\"splitbuttons\"><div><a href=\"javascript:void(0);\" onclick=\"amaf_splitCreate(this," + TopicId + ");\" title=\"[RESX:SplitCreate]\" class=\"dnnPrimaryAction\">[RESX:SplitCreate]</a></div><div><span class=\"NormalBold\">[RESX:SplitHeader]</span> <a href=\"javascript:void(0);\" title=\"[RESX:SplitSave]\" class=\"dnnPrimaryAction af-button-split\" data-id='" + TopicId + "'>[RESX:SplitSave]</a>  <a href=\"javascript:void(0);\" onclick=\"amaf_splitCancel();\" title=\"[RESX:SplitCancel]\" class=\"dnnPrimaryAction\">[RESX:SplitCancel]</a></div></div><script type=\"text/javascript\">var splitposts=new Array();var current_topicid = " + TopicId + ";</script>");

                //sbOutput.Replace("[QUICKREPLY]", "<asp:placeholder id=\"plhQuickReply\" runat=\"server\" />");
            }
            else
            {
                //sbOutput.Replace("[SPLIT]", "<span class=\"afnormal\">[RESX:NotAuthorizedSplit]</span>");
                sbOutput.Replace("[SPLITBUTTONS]", string.Empty);
            }

            // Parent Forum Link
            if (sOutput.Contains("[PARENTFORUMLINK]"))
            {
                if (ForumInfo.ParentForumId > 0)
                {
                    if (MainSettings.UseShortUrls)
                        sbOutput.Replace("[PARENTFORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", new[] { ParamKeys.ForumId + "=" + ForumInfo.ParentForumId }) + "\">" + ForumInfo.ParentForumName + "</a>");
                    else
                        sbOutput.Replace("[PARENTFORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", new[] { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumInfo.ParentForumId }) + "\">" + ForumInfo.ParentForumName + "</a>");
                }
                else if (ForumInfo.ForumGroupId > 0)
                    sbOutput.Replace("[PARENTFORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId) + "\">" + ForumInfo.GroupName + "</a>");
            }

            // Parent Forum Name
            if (string.IsNullOrEmpty(ForumInfo.ParentForumName))
                sbOutput.Replace("[PARENTFORUMNAME]", ForumInfo.ParentForumName);

            // ForumLinks

            var ctlUtils = new ControlUtils();
            var groupUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, string.Empty, ForumInfo.ForumGroupId, -1, -1, -1, string.Empty, 1, -1, SocialGroupId);
            var forumUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, -1, -1, string.Empty, 1, -1, SocialGroupId);
            var topicUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, _topicURL, -1, -1, string.Empty, 1, -1, SocialGroupId);

            sbOutput.Replace("[FORUMMAINLINK]", "<a href=\"" + NavigateUrl(TabId) + "\">[RESX:ForumMain]</a>");
            sbOutput.Replace("[FORUMGROUPLINK]", "<a href=\"" + groupUrl + "\">" + _groupName + "</a>");
            if (MainSettings.UseShortUrls)
                sbOutput.Replace("[FORUMLINK]", "<a href=\"" + forumUrl + "\">" + _forumName + "</a>");
            else
                sbOutput.Replace("[FORUMLINK]", "<a href=\"" + forumUrl + "\">" + _forumName + "</a>");

            // Names and Ids
            sbOutput.Replace("[FORUMID]", ForumId.ToString());
            sbOutput.Replace("[FORUMNAME]", _forumName);
            sbOutput.Replace("[GROUPNAME]", _groupName);

            // Printer Friendly Link
            var sURL = "<a rel=\"nofollow\" href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId, "mid=" + ModuleId, "dnnprintmode=true") + "?skinsrc=" + HttpUtility.UrlEncode("[G]" + UI.Skins.SkinInfo.RootSkin + "/" + Common.Globals.glbHostSkinFolder + "/" + "No Skin") + "&amp;containersrc=" + HttpUtility.UrlEncode("[G]" + UI.Skins.SkinInfo.RootContainer + "/" + Common.Globals.glbHostSkinFolder + "/" + "No Container") + "\" target=\"_blank\">";
            //sURL += "<img src=\"" + _myThemePath + "/images/print16.png\" border=\"0\" alt=\"[RESX:PrinterFriendly]\" /></a>";
            sURL += "<i class=\"fa fa-print fa-fw fa-blue\"></i></a>";
            sbOutput.Replace("[AF:CONTROL:PRINTER]", sURL);

            // Email Link
            if (Request.IsAuthenticated)
            {
                sURL = Utilities.NavigateUrl(TabId, "", new[] { ParamKeys.ViewType + "=sendto", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId });
                //sbOutput.Replace("[AF:CONTROL:EMAIL]", "<a href=\"" + sURL + "\" rel=\"nofollow\"><img src=\"" + _myThemePath + "/images/email16.png\" border=\"0\" alt=\"[RESX:EmailThis]\" /></a>");
                sbOutput.Replace("[AF:CONTROL:EMAIL]", "<a href=\"" + sURL + "\" rel=\"nofollow\"><i class=\"fa fa-envelope-o fa-fw fa-blue\"></i></a>");
            }
            else
                sbOutput.Replace("[AF:CONTROL:EMAIL]", string.Empty);

            // RSS Link
            if (_bAllowRSS)
            {
                var url = Common.Globals.AddHTTP(Common.Globals.GetDomainName(Request)) + "/DesktopModules/ActiveForums/feeds.aspx?portalid=" + PortalId + "&forumid=" + ForumId + "&tabid=" + TabId + "&moduleid=" + ModuleId;
                sbOutput.Replace("[RSSLINK]", "<a href=\"" + url + "\"><img src=\"~/DesktopModules/ActiveForums/themes/" + _myTheme + "/images/rss.png\" runat=server border=\"0\" alt=\"[RESX:RSS]\" /></a>");
            }
            else
                sbOutput.Replace("[RSSLINK]", string.Empty);

            // Subject
            _topicSubject = _topicSubject.Replace("[", "&#91");
            _topicSubject = _topicSubject.Replace("]", "&#93");
            sbOutput.Replace("[SUBJECT]", Utilities.StripHTMLTag(_topicSubject));

            // Reply Count
            sbOutput.Replace("[REPLYCOUNT]", _replyCount.ToString());
            sbOutput.Replace("[AF:LABEL:ReplyCount]", _replyCount.ToString());

            // View Count
            sbOutput.Replace("[VIEWCOUNT]", _viewCount.ToString());

            // Last Post
            sbOutput.Replace("[AF:LABEL:LastPostDate]", _lastPostDate);
            sbOutput.Replace("[AF:LABEL:LastPostAuthor]", UserProfiles.GetDisplayName(ModuleId, true, _bModApprove, ForumUser.IsAdmin || ForumUser.IsSuperUser, _lastPostAuthor.AuthorId, _lastPostAuthor.Username, _lastPostAuthor.FirstName, _lastPostAuthor.LastName, _lastPostAuthor.DisplayName));

            // Topic Info
            sbOutput.Replace("[AF:LABEL:TopicAuthor]", UserProfiles.GetDisplayName(ModuleId, _topicAuthorId, _topicAuthorDisplayName, string.Empty, string.Empty, _topicAuthorDisplayName));
            sbOutput.Replace("[AF:LABEL:TopicDateCreated]", _topicDateCreated);

            if (_bModSplit && (_replyCount > 0))
            {
                /*var @params = new List<string> { ParamKeys.ViewType + "=post", ParamKeys.TopicId + "=" + TopicId, ParamKeys.ForumId + "=" + ForumId };*/

                sbOutput.Replace("[SPLITBUTTONS2]", "<script type=\"text/javascript\">amaf_splitRestore();</script>");
            }
            else
            {
                sbOutput.Replace("[SPLITBUTTONS2]", string.Empty);
            }

            // Pagers
            if (_pageSize == int.MaxValue)
            {
                sbOutput.Replace("[PAGER1]", string.Empty);
                sbOutput.Replace("[PAGER2]", string.Empty);
            }
            else
            {
                sbOutput.Replace("[PAGER1]", "<am:pagernav id=\"Pager1\" runat=\"server\" EnableViewState=\"False\" />");
                sbOutput.Replace("[PAGER2]", "<am:pagernav id=\"Pager2\" runat=\"server\" EnableViewState=\"False\" />");
            }

            // Sort
            sbOutput.Replace("[SORTDROPDOWN]", "<asp:placeholder id=\"plhTopicSort\" runat=\"server\" />");
            var rateControl = new Ratings(TopicId, true, _topicRating);
            sbOutput.Replace("[POSTRATINGBUTTON]", rateControl.Render());

            // Jump To
            sbOutput.Replace("[JUMPTO]", "<asp:placeholder id=\"plhQuickJump\" runat=\"server\" />");

            // Next Topic
            if (_nextTopic == 0)
                sbOutput.Replace("[NEXTTOPIC]", string.Empty);
            else
            {
                string nextTopic;
                if (MainSettings.UseShortUrls)
                {
                    if (SocialGroupId > 0) nextTopic = Utilities.NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + _nextTopic + "&" + ParamKeys.GroupIdName + "=" + SocialGroupId);
                    else nextTopic = Utilities.NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + _nextTopic);
                }
                else
                {
                    if (SocialGroupId > 0) nextTopic = Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId + "&" + ParamKeys.TopicId + "=" + _nextTopic + "&" + ParamKeys.ViewType + "=" + Views.Topic + "&" + ParamKeys.GroupId + SocialGroupId);
                    else nextTopic = Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId + "&" + ParamKeys.TopicId + "=" + _nextTopic + "&" + ParamKeys.ViewType + "=" + Views.Topic);
                }
                sbOutput.Replace("[NEXTTOPIC]", "<a href=\"" + nextTopic + "\" rel=\"nofollow\"><span>[RESX:NextTopic]</span><img src=\"~/DesktopModules/ActiveForums/themes/" + _myTheme + "/images/arrow_right_blue.gif\" runat=server style=\"vertical-align:middle;\" border=\"0\" alt=\"[RESX:NextTopic]\" /></a>");
            }

            // Previous Topic
            if (_prevTopic == 0)
                sbOutput.Replace("[PREVTOPIC]", string.Empty);
            else
            {
                string prevTopic;
                if (MainSettings.UseShortUrls)
                {
                    if (SocialGroupId > 0) prevTopic = Utilities.NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + _prevTopic + "&" + ParamKeys.GroupIdName + "=" + SocialGroupId);
                    else prevTopic = Utilities.NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + _prevTopic);
                }
                else
                {
                    if (SocialGroupId > 0) prevTopic = Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId + "&" + ParamKeys.TopicId + "=" + _prevTopic + "&" + ParamKeys.ViewType + "=" + Views.Topic + "&" + ParamKeys.GroupIdName + "=" + SocialGroupId);
                    else prevTopic = Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId + "&" + ParamKeys.TopicId + "=" + _prevTopic + "&" + ParamKeys.ViewType + "=" + Views.Topic);
                }
                sbOutput.Replace("[PREVTOPIC]", "<a href=\"" + prevTopic + "\" rel=\"nofollow\"><img src=\"~/DesktopModules/ActiveForums/themes/" + _myTheme + "/images/arrow_left_blue.gif\" runat=server style=\"vertical-align:middle;\" border=\"0\" alt=\"[RESX:PrevTopic]\" /><span>[RESX:PrevTopic]</span></a>");
            }

            // Topic Status
            if (((_bRead && _topicAuthorId == UserId) || _bModEdit) & _statusId >= 0)
            {
                sbOutput.Replace("[AF:CONTROL:STATUS]", "<asp:placeholder id=\"plhStatus\" runat=\"server\" />");
                //sbOutput.Replace("[AF:CONTROL:STATUSICON]", "<img alt=\"[RESX:PostStatus" + _statusId.ToString() + "]\" src=\"" + _myThemePath + "/images/status" + _statusId.ToString() + ".png\" />");
                sbOutput.Replace("[AF:CONTROL:STATUSICON]", "<div><i class=\"fa fa-status" + _statusId.ToString() + " fa-red fa-2x\"></i></div>");
            }
            else if (_statusId >= 0)
            {
                sbOutput.Replace("[AF:CONTROL:STATUS]", string.Empty);
                //sbOutput.Replace("[AF:CONTROL:STATUSICON]", "<img alt=\"[RESX:PostStatus" + _statusId.ToString() + "]\" src=\"" + _myThemePath + "/images/status" + _statusId.ToString() + ".png\" />");
                sbOutput.Replace("[AF:CONTROL:STATUSICON]", "<div><i class=\"fa fa-status" + _statusId.ToString() + " fa-red fa-2x\"></i></div>");
            }
            else
            {
                sbOutput.Replace("[AF:CONTROL:STATUS]", string.Empty);
                sbOutput.Replace("[AF:CONTROL:STATUSICON]", string.Empty);
            }

            // Poll
            if (_topicType == (int)TopicTypes.Poll)
                sbOutput.Replace("[AF:CONTROL:POLL]", "<asp:placeholder id=\"plhPoll\" runat=\"server\" />");
            else
                sbOutput.Replace("[AF:CONTROL:POLL]", string.Empty);

            return sbOutput.ToString();
        }

        private string ParseTopic(string sOutput)
        {
            // Process our separators which are injected between rows.
            // Minimum index is 1.  If zero is specified, it will be treated as the default.

            const string pattern = @"\[REPLYSEPARATOR:(\d+?)\]";
            var separators = new Dictionary<int, string>();
            if (sOutput.Contains("[REPLYSEPARATOR"))
            {
                var defaultSeparator = TemplateUtils.GetTemplateSection(sOutput, "[REPLYSEPARATOR]", "[/REPLYSEPARATOR]", false);
                if (!string.IsNullOrWhiteSpace(defaultSeparator))
                {
                    separators.Add(0, defaultSeparator.Replace("[REPLYSEPARATOR]", string.Empty).Replace("[/REPLYSEPARATOR]", string.Empty));
                    sOutput = TemplateUtils.ReplaceSubSection(sOutput, string.Empty, "[REPLYSEPARATOR]", "[/REPLYSEPARATOR]");
                }

                foreach (Match match in Regex.Matches(sOutput, pattern))
                {
                    var rowIndex = int.Parse(match.Groups[1].Value);
                    var startTag = string.Format("[REPLYSEPARATOR:{0}]", rowIndex);
                    var endTag = string.Format("[/REPLYSEPARATOR:{0}]", rowIndex);

                    var separator = TemplateUtils.GetTemplateSection(sOutput, startTag, endTag, false);
                    if (string.IsNullOrWhiteSpace(separator))
                        continue;

                    separators[rowIndex] = separator.Replace(startTag, string.Empty).Replace(endTag, string.Empty);
                    sOutput = TemplateUtils.ReplaceSubSection(sOutput, string.Empty, startTag, endTag);
                }
            }


            // Prorcess topic and reply templates.

            var sTopicTemplate = TemplateUtils.GetTemplateSection(sOutput, "[TOPIC]", "[/TOPIC]");
            var sReplyTemplate = TemplateUtils.GetTemplateSection(sOutput, "[REPLIES]", "[/REPLIES]");
            var sTemp = string.Empty;
            var i = 0;

            if (_dtTopic.Rows.Count > 0)
            {
                foreach (DataRow dr in _dtTopic.Rows)
                {
                    // deal with our separator first
                    if (i > 0 && separators.Count > 0) // No separator before the first row
                    {
                        if (separators.ContainsKey(i)) // Specific row
                            sTemp += separators[i];
                        else if (separators.ContainsKey(0)) // Default
                            sTemp += separators[0];
                    }

                    var replyId = Convert.ToInt32(dr["ReplyId"]);

                    if (replyId == 0)
                        sTopicTemplate = ParseContent(dr, sTopicTemplate, i);
                    else
                        sTemp += ParseContent(dr, sReplyTemplate, i);

                    i++;
                }

                if (_defaultSort == "ASC")
                {
                    sOutput = TemplateUtils.ReplaceSubSection(sOutput, sTemp, "[REPLIES]", "[/REPLIES]");
                    sOutput = TemplateUtils.ReplaceSubSection(sOutput, sTopicTemplate, "[TOPIC]", "[/TOPIC]");
                }
                else
                {
                    sOutput = TemplateUtils.ReplaceSubSection(sOutput, sTemp + sTopicTemplate, "[REPLIES]", "[/REPLIES]");
                    sOutput = TemplateUtils.ReplaceSubSection(sOutput, string.Empty, "[TOPIC]", "[/TOPIC]");
                }
                if (sTopicTemplate.Contains("[BODY]"))
                {
                    sOutput = sOutput.Replace(sTopicTemplate, string.Empty);
                }

            }

            return sOutput;
        }

        private string ParseContent(DataRow dr, string tempate, int rowcount)
        {
            var sOutput = tempate;

            var replyId = dr.GetInt("ReplyId");
            var topicId = dr.GetInt("TopicId");
            var postId = replyId == 0 ? topicId : replyId;
            var contentId = dr.GetInt("ContentId");
            var dateCreated = dr.GetDateTime("DateCreated");
            var dateUpdated = dr.GetDateTime("DateUpdated");
            var authorId = dr.GetInt("AuthorId");
            var username = dr.GetString("Username");
            var firstName = dr.GetString("FirstName");
            var lastName = dr.GetString("LastName");
            var displayName = dr.GetString("DisplayName");
            var userTopicCount = dr.GetInt("TopicCount");
            var userReplyCount = dr.GetInt("ReplyCount");
            var postCount = userTopicCount + userReplyCount;
            var userCaption = dr.GetString("UserCaption");
            var body = dr.GetString("Body");
            var subject = dr.GetString("Subject");
            var tags = dr.GetString("Tags");
            var signature = dr.GetString("Signature");
            var ipAddress = dr.GetString("IPAddress");
            var memberSince = dr.GetDateTime("MemberSince");
            var avatarDisabled = dr.GetBoolean("AvatarDisabled");
            var userRoles = dr.GetString("UserRoles");
            var isUserOnline = dr.GetBoolean("IsUserOnline");
            var replyStatusId = dr.GetInt("StatusId");
            var totalPoints = _enablePoints ? dr.GetInt("UserTotalPoints") : 0;
            var answerCount = dr.GetInt("AnswerCount");
            var rewardPoints = dr.GetInt("RewardPoints");
            var dateLastActivity = dr.GetDateTime("DateLastActivity");
            var signatureDisabled = dr.GetBoolean("SignatureDisabled");

            DotNetNuke.Entities.Users.UserController uc = new DotNetNuke.Entities.Users.UserController();
            DotNetNuke.Entities.Users.UserInfo author = uc.GetUser(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId, authorId);

            // Populate the user object with the post author info.  
            var up = new User
            {
                UserId = authorId,
                UserName = username,
                FirstName = firstName.Replace("&amp;#", "&#"),
                LastName = lastName.Replace("&amp;#", "&#"),
                DisplayName = displayName.Replace("&amp;#", "&#"),
                Profile =
                {
                    UserCaption = userCaption,
                    Signature = signature,
                    DateCreated = memberSince,
                    AvatarDisabled = avatarDisabled,
                    Roles = userRoles,
                    ReplyCount = userReplyCount,
                    TopicCount = userTopicCount,
                    AnswerCount = answerCount,
                    RewardPoints = rewardPoints,
                    DateLastActivity = dateLastActivity,
                    PrefBlockAvatars = UserPrefHideAvatars,
                    PrefBlockSignatures = UserPrefHideSigs,
                    IsUserOnline = isUserOnline,
                    SignatureDisabled = signatureDisabled
                }
            };
            if (author != null) up.Email = author.Email;

            //Perform Profile Related replacements
            sOutput = TemplateUtils.ParseProfileTemplate(sOutput, up, PortalId, ModuleId, ImagePath, CurrentUserType, true, UserPrefHideAvatars, UserPrefHideSigs, ipAddress, UserId, TimeZoneOffset);

            // Replace Tags Control
            if (string.IsNullOrWhiteSpace(tags))
                sOutput = TemplateUtils.ReplaceSubSection(sOutput, string.Empty, "[AF:CONTROL:TAGS]", "[/AF:CONTROL:TAGS]");
            else
            {
                sOutput = sOutput.Replace("[AF:CONTROL:TAGS]", string.Empty);
                sOutput = sOutput.Replace("[/AF:CONTROL:TAGS]", string.Empty);
                var tagList = string.Empty;
                foreach (var tag in tags.Split(','))
                {
                    if (tagList != string.Empty)
                        tagList += ", ";

                    tagList += "<a href=\"" + Utilities.NavigateUrl(TabId, string.Empty, new[] { ParamKeys.ViewType + "=search", ParamKeys.Tags + "=" + HttpUtility.UrlEncode(tag) }) + "\">" + tag + "</a>";
                }
                sOutput = sOutput.Replace("[AF:LABEL:TAGS]", tagList);
            }

            // Use a string builder from here on out.

            var sbOutput = new StringBuilder(sOutput);

            if (_bModSplit)
            {
                sbOutput = sbOutput.Replace("[SPLITCHECKBOX]", "<div class=\"split-checkbox\" style=\"display:none;\"><input type=\"checkbox\" onChange=\"amaf_splitCheck(this);\" value=\"" + replyId + "\" /></div>");
            }
            else
            {
                sbOutput = sbOutput.Replace("[SPLITCHECKBOX]", string.Empty);
            }

            // Row CSS Classes
            if (rowcount % 2 == 0)
            {
                sbOutput.Replace("[POSTINFOCSS]", "afpostinfo afpostinfo1");
                sbOutput.Replace("[POSTTOPICCSS]", "afposttopic afpostreply1");
                sbOutput.Replace("[POSTREPLYCSS]", "afpostreply afpostreply1");
            }
            else
            {
                sbOutput.Replace("[POSTTOPICCSS]", "afposttopic afpostreply2");
                sbOutput.Replace("[POSTINFOCSS]", "afpostinfo afpostinfo2");
                sbOutput.Replace("[POSTREPLYCSS]", "afpostreply afpostreply2");
            }

            // Description
            if (replyId == 0)
            {
                sbOutput.Replace("[POSTID]", topicId.ToString());
                _topicDescription = Utilities.StripHTMLTag(body).Trim();
                _topicDescription = _topicDescription.Replace(System.Environment.NewLine, string.Empty);
                if (_topicDescription.Length > 255)
                    _topicDescription = _topicDescription.Substring(0, 255);
            }
            else
            {
                sbOutput.Replace("[POSTID]", replyId.ToString());
            }

            sbOutput.Replace("[FORUMID]", ForumId.ToString());
            sbOutput.Replace("[REPLYID]", replyId.ToString());
            sbOutput.Replace("[TOPICID]", topicId.ToString());
            sbOutput.Replace("[POSTDATE]", GetDate(dateCreated));
            sbOutput.Replace("[DATECREATED]", GetDate(dateCreated));




            // Parse Roles -- This should actually be taken care of in ParseProfileTemplate
            //if (sOutput.Contains("[ROLES:"))
            //    sOutput = TemplateUtils.ParseRoles(sOutput, (up.UserId == -1) ? string.Empty : up.Profile.Roles);

            // Delete Action
            if (_bModDelete || ((_bDelete && authorId == UserId && !_bLocked) && ((replyId == 0 && _replyCount == 0) || replyId > 0)))
            {
                if (_useListActions)
                    sbOutput.Replace("[ACTIONS:DELETE]", "<li onclick=\"amaf_postDel(" + topicId + "," + replyId + ");\" title=\"[RESX:Delete]\"><i class=\"fa fa-trash-o fa-fw fa-blue\"></i>&nbsp;[RESX:Delete]</li>");
                else
                    sbOutput.Replace("[ACTIONS:DELETE]", "<a href=\"javascript:void(0);\" class=\"af-actions\" onclick=\"amaf_postDel(" + topicId + "," + replyId + ");\" title=\"[RESX:Delete]\"><i class=\"fa fa-trash-o fa-fw fa-blue\"></i>&nbsp;[RESX:Delete]</a>");
            }
            else
            {
                sbOutput.Replace("[ACTIONS:DELETE]", string.Empty);
            }

            // Edit Action
            if (_bModEdit || (_bEdit && authorId == UserId && (_editInterval == 0 || SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Minute, dateCreated, DateTime.Now) < _editInterval)))
            {
                var sAction = "re";
                if (replyId == 0)
                    sAction = "te";

                //var editParams = new[] { ParamKeys.ViewType + "=post", "action=" + sAction, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + topicId, "postid=" + postId };
                var editParams = new List<string>() { ParamKeys.ViewType + "=post", "action=" + sAction, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + topicId, "postid=" + postId };
                if (SocialGroupId > 0) editParams.Add(ParamKeys.GroupIdName + "=" + SocialGroupId);
                if (_useListActions)
                    sbOutput.Replace("[ACTIONS:EDIT]", "<li onclick=\"window.location.href='" + Utilities.NavigateUrl(TabId, "", editParams) + "';\" title=\"[RESX:Edit]\"><i class=\"fa fa-pencil fa-fw fa-blue\"></i>&nbsp;[RESX:Edit]</li>");
                else
                    sbOutput.Replace("[ACTIONS:EDIT]", "<a class=\"af-actions\" href=\"" + Utilities.NavigateUrl(TabId, "", editParams) + "\" title=\"[RESX:Edit]\"><i class=\"fa fa-pencil fa-fw fa-blue\"></i>&nbsp;[RESX:Edit]</a>");
            }
            else
            {
                sbOutput.Replace("[ACTIONS:EDIT]", string.Empty);
            }

            // Reply and Quote Actions
            if (!_bLocked && CanReply)
            {
                var quoteParams = new List<string> { ParamKeys.ViewType + "=post", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.QuoteId + "=" + postId };
                var replyParams = new List<string> { ParamKeys.ViewType + "=post", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.ReplyId + "=" + postId };
                if (SocialGroupId > 0)
                {
                    quoteParams.Add(ParamKeys.GroupIdName + "=" + SocialGroupId);
                    replyParams.Add(ParamKeys.GroupIdName + "=" + SocialGroupId);
                }
                if (_useListActions)
                {
                    sbOutput.Replace("[ACTIONS:QUOTE]", "<li onclick=\"window.location.href='" + Utilities.NavigateUrl(TabId, "", quoteParams) + "';\" title=\"[RESX:Quote]\"><i class=\"fa fa-quote-left fa-fw fa-blue\"></i>&nbsp;[RESX:Quote]</li>");
                    sbOutput.Replace("[ACTIONS:REPLY]", "<li onclick=\"window.location.href='" + Utilities.NavigateUrl(TabId, "", replyParams) + "';\" title=\"[RESX:Reply]\"><i class=\"fa fa-reply fa-fw fa-blue\"></i>&nbsp;[RESX:Reply]</li>");
                }
                else
                {
                    sbOutput.Replace("[ACTIONS:QUOTE]", "<a class=\"af-actions\" href=\"" + Utilities.NavigateUrl(TabId, "", quoteParams) + "\" title=\"[RESX:Quote]\"><i class=\"fa fa-quote-left fa-fw fa-blue\"></i>&nbsp;[RESX:Quote]</a>");
                    sbOutput.Replace("[ACTIONS:REPLY]", "<a class=\"af-actions\" href=\"" + Utilities.NavigateUrl(TabId, "", replyParams) + "\" title=\"[RESX:Reply]\"><i class=\"fa fa-reply fa-fw fa-blue\"></i>&nbsp;[RESX:Reply]</a>");
                }
            }
            else
            {
                sbOutput.Replace("[ACTIONS:QUOTE]", string.Empty);
                sbOutput.Replace("[ACTIONS:REPLY]", string.Empty);
            }

            if (_bModMove)
            {
                sbOutput.Replace("[ACTIONS:MOVE]", "<li onclick=\"javascript:amaf_openMove([TOPICID])\"';\" title=\"[RESX:Move]\"><i class=\"fa fa-exchange fa-rotate-90 fa-blue\"></i>&nbsp;[RESX:Move]</li>");
            }
            else
            {
                sbOutput = sbOutput.Replace("[ACTIONS:MOVE]", string.Empty);
            }

            sbOutput = sbOutput.Replace("[TOPICID]", TopicId.ToString());

            // Status
            if (_statusId <= 0 || (_statusId == 3 && replyStatusId == 0))
            {
                sbOutput.Replace("[ACTIONS:ANSWER]", string.Empty);
            }
            else if (replyStatusId == 1)
            {
                // Answered
                if (_useListActions)
                    sbOutput.Replace("[ACTIONS:ANSWER]", "<li class=\"af-answered\" title=\"[RESX:Status:Answer]\"><em></em>[RESX:Status:Answer]</li>");
                else
                    sbOutput.Replace("[ACTIONS:ANSWER]", "<span class=\"af-actions af-answered\" title=\"[RESX:Status:Answer]\"><em></em>[RESX:Status:Answer]</span>");
            }
            else
            {
                // Not Answered
                if ((UserId == _topicAuthorId && !_bLocked) || _bModEdit)
                {
                    // Can mark answer
                    if (_useListActions)
                        sbOutput.Replace("[ACTIONS:ANSWER]", "<li class=\"af-markanswer\" onclick=\"amaf_markAnswer(" + topicId.ToString() + "," + replyId.ToString() + ");\" title=\"[RESX:Status:SelectAnswer]\"><em></em>[RESX:Status:SelectAnswer]</li>");
                    else
                        sbOutput.Replace("[ACTIONS:ANSWER]", "<a class=\"af-actions af-markanswer\" href=\"#\" onclick=\"amaf_markAnswer(" + topicId.ToString() + "," + replyId.ToString() + "); return false;\" title=\"[RESX:Status:SelectAnswer]\"><em></em>[RESX:Status:SelectAnswer]</a>");
                }
                else
                {
                    // Display Nothing
                    sbOutput.Replace("[ACTIONS:ANSWER]", string.Empty);
                }
            }

            // User Edit Date
            if (dateUpdated == dateCreated || dateUpdated == DateTime.MinValue || dateUpdated == Utilities.NullDate())
            {
                sbOutput.Replace("[EDITDATE]", string.Empty);
            }
            else
            {
                sbOutput.Replace("[EDITDATE]", Utilities.GetDate(dateUpdated, ModuleId, TimeZoneOffset));
            }

            // Mod Edit Date
            if (_bModEdit)
            {
                if (dateCreated == dateUpdated || dateUpdated == DateTime.MinValue || dateUpdated == Utilities.NullDate())
                    sbOutput.Replace("[MODEDITDATE]", string.Empty);
                else
                    sbOutput.Replace("[MODEDITDATE]", Utilities.GetDate(dateUpdated, ModuleId, TimeZoneOffset));
            }
            else
            {
                sbOutput.Replace("[MODEDITDATE]", string.Empty);
            }

            if (_allowLikes)
            {
                Image likeImage = new Image();
                var likesController = new LikesController();
                var likes = likesController.GetForPost(contentId);

                bool youLike = likes.Where(o => o.UserId == UserId)
                    .Select(o => o.Checked)
                    .FirstOrDefault();
                string image = string.Empty;
                if (youLike)
                    image = "fa-thumbs-o-up";
                else
                    image = "fa-thumbs-up";

                likeImage.ImageUrl = image;
                if (CanReply)
                {
                    sbOutput = sbOutput.Replace("[LIKES]", "<i class=\"fa " + image + "\" onclick=\"amaf_likePost(" + UserId + "," + contentId + ")\" > " + likes.Count.ToString() + "</i>");
                    sbOutput = sbOutput.Replace("[LIKESx2]", "<i class=\"fa " + image + " fa-2x\" onclick=\"amaf_likePost(" + UserId + "," + contentId + ")\" > " + likes.Count.ToString() + "</i>");
                    sbOutput = sbOutput.Replace("[LIKESx3]", "<i class=\"fa " + image + " fa-3x\" onclick=\"amaf_likePost(" + UserId + "," + contentId + ")\" > " + likes.Count.ToString() + "</i>");
                }
                else
                {
                    sbOutput = sbOutput.Replace("[LIKES]", "<i class=\"fa " + image + "\" /> " + likes.Count.ToString());
                    sbOutput = sbOutput.Replace("[LIKESx2]", "<i class=\"fa " + image + " fa-2x\" /> " + likes.Count.ToString());
                    sbOutput = sbOutput.Replace("[LIKESx3]", "<i class=\"fa " + image + " fa-3x\" /> " + likes.Count.ToString());
                    //sbOutput = sbOutput.Replace("[LIKES]", "<img src=\"" + image + "\" onclick=\"amaf_likePost(" + UserId + "," + contentId + ")\" /> " + likes.Count.ToString());
                }
            }
            else
            {
                sbOutput = sbOutput.Replace("[LIKES]", string.Empty);
                sbOutput = sbOutput.Replace("[LIKESx2]", string.Empty);
                sbOutput = sbOutput.Replace("[LIKESx3]", string.Empty);
            }

            // Poll Results
            if (sOutput.Contains("[POLLRESULTS]"))
            {
                if (_topicType == 1)
                {
                    var polls = new Polls();
                    sbOutput.Replace("[POLLRESULTS]", polls.PollResults(topicId, ImagePath));
                }
                else
                {
                    sbOutput.Replace("[POLLRESULTS]", string.Empty);
                }
            }

            // Mod Alert
            //var alertParams = new[] { ParamKeys.ViewType + "=modreport", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.ReplyId + "=" + postId };
            var alertParams = new List<string> { ParamKeys.ViewType + "=modreport", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + topicId, ParamKeys.ReplyId + "=" + postId };
            if (SocialGroupId > 0) alertParams.Add(String.Format("{0}={1}", ParamKeys.GroupIdName, SocialGroupId));
            if (Request.IsAuthenticated)
            {
                if (_useListActions)
                    sbOutput.Replace("[ACTIONS:ALERT]", "<li onclick=\"window.location.href='" + Utilities.NavigateUrl(TabId, "", alertParams) + "';\" title=\"[RESX:Alert]\"><i class=\"fa fa-bell-o fa-fw fa-blue\"></i>&nbsp;[RESX:Alert]</li>");
                else
                    sbOutput.Replace("[ACTIONS:ALERT]", "<a class=\"af-actions\" href=\"" + Utilities.NavigateUrl(TabId, "", alertParams) + "\" title=\"[RESX:Alert]\"><i class=\"fa fa-bell-o fa-fw fa-blue\"></i>&nbsp;[RESX:Alert]</a>");
            }
            else
            {
                sbOutput.Replace("[ACTIONS:ALERT]", string.Empty);
            }

            // Process Body
            if (string.IsNullOrEmpty(body))
                body = " <br />";

            var sBody = Utilities.ManageImagePath(body);

            sBody = sBody.Replace("[", "&#91;");
            sBody = sBody.Replace("]", "&#93;");
            if (sBody.ToUpper().Contains("&#91;CODE&#93;"))
            {
                sBody = Regex.Replace(sBody, "(&#91;CODE&#93;)", "[CODE]", RegexOptions.IgnoreCase);
                sBody = Regex.Replace(sBody, "(&#91;\\/CODE&#93;)", "[/CODE]", RegexOptions.IgnoreCase);

            }
            //sBody = sBody.Replace("&lt;CODE&gt;", "<CODE>")
            if (Regex.IsMatch(sBody, "\\[CODE([^>]*)\\]", RegexOptions.IgnoreCase))
            {
                var objCode = new CodeParser();
                sBody = CodeParser.ParseCode(Utilities.HTMLDecode(sBody));
            }
            sBody = Utilities.StripExecCode(sBody);
            if (MainSettings.AutoLinkEnabled)
                sBody = Utilities.AutoLinks(sBody, Request.Url.Host);

            if (sBody.Contains("<%@"))
                sBody = sBody.Replace("<%@ ", "&lt;&#37;@ ");

            if (body.ToLowerInvariant().Contains("runat"))
                sBody = Regex.Replace(sBody, "runat", "&#114;&#117;nat", RegexOptions.IgnoreCase);

            sbOutput.Replace("[BODY]", sBody);

            // Subject
            sbOutput.Replace("[SUBJECT]", subject);

            // Attachments
            sbOutput.Replace("[ATTACHMENTS]", GetAttachments(contentId, true, PortalId, ModuleId));

            // Switch back from the string builder to a normal string before we perform the image/thumbnail replacements.

            sOutput = sbOutput.ToString();

            // Legacy attachment functionality, uses "attachid"
            // &#91;IMAGE:38&#93;
            if (sOutput.Contains("&#91;IMAGE:"))
            {
                var strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(Request)) + "/";
                const string pattern = "(&#91;IMAGE:(.+?)&#93;)";
                sOutput = Regex.Replace(sOutput, pattern, match => "<img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + match.Groups[2].Value + "\" border=\"0\" class=\"afimg\" />");
            }

            // Legacy attachment functionality, uses "attachid"
            // &#91;THUMBNAIL:38&#93;
            if (sOutput.Contains("&#91;THUMBNAIL:"))
            {
                var strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(Request)) + "/";
                const string pattern = "(&#91;THUMBNAIL:(.+?)&#93;)";
                sOutput = Regex.Replace(sOutput, pattern, match =>
                {
                    var thumbId = match.Groups[2].Value.Split(':')[0];
                    var parentId = match.Groups[2].Value.Split(':')[1];
                    return "<a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + parentId + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + thumbId + "\" border=\"0\" class=\"afimg\" /></a>";
                });
            }

            return sOutput;
        }


        // Renders the [ATTACHMENTS] block
        private string GetAttachments(int contentId, bool allowAttach, int portalId, int moduleId)
        {
            if (!allowAttach || _dtAttach.Rows.Count == 0)
                return string.Empty;

            const string itemTemplate = "<li><a href='/DesktopModules/ActiveForums/viewer.aspx?portalid={0}&moduleid={1}&attachmentid={2}' target='_blank'><i class='af-fileicon af-fileicon-{3}'></i><span>{4}</span></a></li>";

            _dtAttach.DefaultView.RowFilter = "ContentId = " + contentId;

            var attachmentRows = _dtAttach.DefaultView.ToTable().Rows;

            if (attachmentRows.Count == 0)
                return string.Empty;

            var sb = new StringBuilder(1024);

            sb.Append("<div class='af-attach-post-list'><span>");
            sb.Append(Utilities.GetSharedResource("[RESX:Attachments]"));
            sb.Append("</span><ul>");

            foreach (DataRow dr in _dtAttach.DefaultView.ToTable().Rows)
            {
                //AttachId, ContentId, UserID, FileName, ContentType, FileSize, FileID

                var attachId = dr.GetInt("AttachId");
                var filename = dr.GetString("Filename").TextOrEmpty();

                var fileExtension = System.IO.Path.GetExtension(filename).TextOrEmpty().Replace(".", string.Empty);

                filename = HttpUtility.HtmlEncode(Regex.Replace(filename, @"^__\d+__\d+__", string.Empty));

                sb.AppendFormat(itemTemplate, portalId, moduleId, attachId, fileExtension, filename);
            }

            sb.Append("</ul></div>");

            return sb.ToString();
        }

        private void BuildPager()
        {
            var pager1 = FindControl("Pager1") as PagerNav;
            var pager2 = FindControl("Pager2") as PagerNav;

            if (pager1 == null && pager2 == null)
                return;

            var intPages = Convert.ToInt32(Math.Ceiling(_rowCount / (double)_pageSize));

            if (!(_topicURL.Contains(ForumInfo.PrefixURL)))
                _topicURL = "/" + ForumInfo.PrefixURL + "/" + _topicURL;

            var @params = new List<string>();
            if (!string.IsNullOrWhiteSpace(Request.Params[ParamKeys.Sort]))
                @params.Add(ParamKeys.Sort + "=" + Request.Params[ParamKeys.Sort]);

            if (pager1 != null)
            {
                pager1.PageCount = intPages;
                pager1.CurrentPage = PageId;
                pager1.TabID = Utilities.SafeConvertInt(Request.Params["TabId"], -1);
                pager1.ForumID = ForumId;
                pager1.UseShortUrls = MainSettings.UseShortUrls;
                pager1.PageText = Utilities.GetSharedResource("[RESX:Page]");
                pager1.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
                pager1.View = Views.Topic;
                pager1.TopicId = TopicId;
                pager1.PageMode = PagerNav.Mode.Links;

                if (MainSettings.URLRewriteEnabled && !string.IsNullOrWhiteSpace(_topicURL))
                    pager1.BaseURL = _topicURL;

                pager1.Params = @params.ToArray();
            }

            if (pager2 != null)
            {
                pager2.PageCount = intPages;
                pager2.CurrentPage = PageId;
                pager2.TabID = Utilities.SafeConvertInt(Request.Params["TabId"], -1);
                pager2.ForumID = ForumId;
                pager2.UseShortUrls = MainSettings.UseShortUrls;
                pager2.PageText = Utilities.GetSharedResource("[RESX:Page]");
                pager2.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
                pager2.View = Views.Topic;
                pager2.TopicId = TopicId;
                pager2.PageMode = PagerNav.Mode.Links;

                if (MainSettings.URLRewriteEnabled && !string.IsNullOrWhiteSpace(_topicURL))
                    pager2.BaseURL = _topicURL;

                pager2.Params = @params.ToArray();
            }

        }

        #endregion
    }
}
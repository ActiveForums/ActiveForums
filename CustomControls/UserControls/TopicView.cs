//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
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
        private string ForumName;
        private string GroupName;
        //Private ForumGroupId As Integer = 0
        private int TopicTemplateId;
        private DataRow drForum;
        private DataRow drSecurity;
        private DataTable dtTopic;
        private DataTable dtAttach;
        private bool bView = false;
        private bool bRead = false;
        private bool bCreate = false;
        private bool bPoll = false;
        private bool bAttach = false;
        private bool bTrust = false;
        private bool bReply = false;
        private bool bDelete = false;
        private bool bEdit = false;
        private bool bLock = false;
        private bool bPin = false;
        private bool bSubscribe = false;
        private bool bModApprove = false;
        private bool bModMove = false;
        private bool bModSplit = false;
        private bool bModDelete = false;
        private bool bModEdit = false;
        private bool bAllowRSS = false;
        private int RowIndex = 0;
        private int PageSize = 20;
        private string MyTheme = "_default";
        private string MyThemePath = string.Empty;
        private bool bLocked = false;
        private int TopicType = 0;
        private string TopicSubject = string.Empty;
        private string TopicDescription = string.Empty;
        private int ViewCount = 0;
        private int ReplyCount = 0;
        private int RowCount = 0;
        private int StatusId = 0;
        private int TopicAuthorId = 0;
        private string TopicAuthorDisplayName = string.Empty;
        private string TopicDateCreated = string.Empty;
        private string MemberListMode = string.Empty;
        private string ProfileVisibility = string.Empty;
        private string UserNameDisplay = string.Empty;
        private bool DisableUserProfiles = false;
        private bool AllowAvatars = false;
        private bool bAllowSignatures = true;
        private bool EnablePoints = false;
        private bool TrustDefault = false;
        private bool IsTrusted = false;
        private int TopicRating = 0;
        private bool AllowHTML = false;
        private bool AllowScript = false;
        private bool AllowSubscribe = false;
        private int NextTopic = 0;
        private int PrevTopic = 0;
        private bool IsSubscribedTopic = false;
        private bool IsSubscribedForum = false;
        private string LastPostDate;
        private Author LastPostAuthor = new Author();
        private string DefaultSort = "ASC";
        private bool AllowTags = false;
        private int EditInterval = 0;
        private string Tags = string.Empty;
        private string TopicURL = string.Empty;
        private string _template = string.Empty;
        private int _pageSize = -1;
        private string _defaultSort = string.Empty;
        private string TopicData = string.Empty;
        private bool SkipBindTopic = false;
        private string sGroupURL = string.Empty;
        private string sForumURL = string.Empty;
        private string sTopicURL = string.Empty;
        private bool UseListActions = false;
        private bool NoPaging = false;
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
        public int OptPageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }
        public string OptDefaultSort
        {
            set
            {
                _defaultSort = value;
            }
        }
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
        #region Protected Controls
        protected af_quickjump ctlForumJump = new af_quickjump();
        protected af_polls ctlPoll = new af_polls();
        protected af_quickreplyform ctlQuickReply = new af_quickreplyform();
        protected af_topicsorter ctlTopicSort = new af_topicsorter();
        protected af_topicstatus ctlTopicStatus = new af_topicstatus();
        protected Callback cbTopicActions = new Callback();
        protected PlaceHolder plhTopic = new PlaceHolder();
        #endregion
        #region Event Handlers
        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            if (ForumInfo == null)
            {
                ForumController fc = new ForumController();
                ForumInfo = fc.Forums_Get(PortalId, ForumModuleId, ForumId, UserId, true, false, TopicId);
            }

            //Me.EnableViewState = False

        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {

                string sURL = Request.RawUrl;
                if (!Page.IsPostBack)
                {
                    if (sURL.Contains("view") && sURL.Contains("postid") && sURL.Contains("forumid"))
                    {
                        sURL = sURL.Replace("view", ParamKeys.ViewType);
                        sURL = sURL.Replace("postid", ParamKeys.TopicId);
                        sURL = sURL.Replace("forumid", ParamKeys.ForumId);
                        Response.Status = "301 Moved Permanently";
                        Response.AddHeader("Location", sURL);
                    }
                    if (ForumId < 1)
                    {
                        Response.Redirect(Utilities.NavigateUrl(TabId));
                    }
                }
                if (Request.Params[ParamKeys.FirstNewPost] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.FirstNewPost]))
                    {
                        int cjid = -1;
                        cjid = DataProvider.Instance().Utility_GetFirstUnRead(TopicId, Convert.ToInt32(Request.Params[ParamKeys.FirstNewPost]));
                        if (cjid > 0 & cjid > Convert.ToInt32(Request.Params[ParamKeys.FirstNewPost]))
                        {
                            string tURL = Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=topic", ParamKeys.ContentJumpId + "=" + cjid });
                            if (MainSettings.UseShortUrls)
                            {
                                tURL = Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + cjid });
                            }
                            Response.Redirect(tURL);
                        }
                    }


                }
                this.AppRelativeVirtualPath = "~/";
                MyTheme = MainSettings.Theme;
                MyThemePath = Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MyTheme);
                UserNameDisplay = MainSettings.UserNameDisplay;
                DisableUserProfiles = false;
                AllowAvatars = true;
                EnablePoints = MainSettings.EnablePoints;


                EditInterval = MainSettings.EditInterval;

                if (MainSettings.AllowSignatures == 1 || MainSettings.AllowSignatures == 2)
                {
                    if (UserId > 0)
                    {
                        bAllowSignatures = Convert.ToBoolean((UserPrefHideSigs ? false : true));
                    }
                    else
                    {
                        bAllowSignatures = true;
                    }
                }
                if (AllowAvatars == true)
                {
                    if (UserId > 0)
                    {
                        AllowAvatars = Convert.ToBoolean((UserPrefHideAvatars ? false : true));
                    }
                }
                string sSort = "ASC";

                if (ForumUser.Profile.PrefDefaultSort.Trim() == "ASC" || ForumUser.Profile.PrefDefaultSort.Trim() == "DESC")
                {
                    sSort = ForumUser.Profile.PrefDefaultSort.Trim();
                    DefaultSort = sSort;
                }
                if (Request.Params[ParamKeys.Sort] != null)
                {
                    sSort = Request.Params[ParamKeys.Sort];
                    if (!(sSort.ToUpper() == "ASC") && !(sSort.ToUpper() == "DESC"))
                    {
                        sSort = "ASC";
                    }
                }
                if (_defaultSort == string.Empty)
                {
                    DefaultSort = sSort;
                }
                else
                {
                    DefaultSort = _defaultSort;
                }
                //If Not IsCallback() Then
                LoadData(PageId);
                if (!SkipBindTopic)
                {
                    BindTopic();
                }

                if (Request.IsAuthenticated)
                {
                    AllowSubscribe = bSubscribe;
                }
                else
                {
                    AllowSubscribe = false;
                }
                //End If
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //Dim cbscript As String = "function afcbtopic(params){" & cbTopicActions.ClientID & ".Callback(params);};function afcbtopiccomplete(){window.scrollTo(0,0);window.top.afreload();};"
                //If bModDelete Or bDelete Then
                //    cbscript &= "function afcbdelpost(fid,tid,pid){if (confirm('" & GetSharedResource("[RESX:Confirm:Delete]") & "')){af_showLoad();" & cbTopicLoader.ClientID & ".Callback('delpost',pid,tid,fid);};};"
                //End If

                //Page.ClientScript.RegisterClientScriptBlock(Page.GetType, "cbscript", cbscript, True)
                try
                {
                    DotNetNuke.Framework.CDefault tempVar = this.BasePage;
                    Environment.UpdateMeta(ref tempVar, MetaTitle, MetaDescription, MetaKeywords);

                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {
                RenderMessage("[RESX:Error:LoadingTopic]", ex.Message, ex);

            }
        }

        #endregion
        #region Private Methods

        private void LoadData(int PageId)
        {
            if (OptPageSize == -1)
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
            }
            else
            {
                PageId = 1;
                PageSize = int.MaxValue;
            }


            if (PageId == 1)
            {
                RowIndex = 0;
            }
            else
            {
                RowIndex = ((PageId * PageSize) - PageSize);
            }
            string sURL = Request.RawUrl;
            if (Request.QueryString["dnnprintmode"] != null | OptPageSize > 0)
            {
                RowIndex = 0;
                PageSize = int.MaxValue;
            }
            int defaultTemplateId = ForumInfo.TopicTemplateId;
            if (DefaultTopicViewTemplateId >= 0)
            {
                defaultTemplateId = DefaultTopicViewTemplateId;
            }
            TopicTemplateId = defaultTemplateId;
            string stmp = DataCache.GetCachedTemplate(MainSettings.TemplateCache, ModuleId, "TopicView", defaultTemplateId);
            if (stmp.Contains("[NOPAGING]"))
            {
                PageId = 1;
                RowIndex = 0;
                PageSize = int.MaxValue;
                NoPaging = true;
            }
            if (stmp.Contains("[NOTOOLBAR]"))
            {
                if (HttpContext.Current.Items.Contains("ShowToolbar"))
                {
                    HttpContext.Current.Items["ShowToolbar"] = false;
                }
                else
                {
                    HttpContext.Current.Items.Add("ShowToolbar", false);
                }
                stmp = stmp.Replace("[NOTOOLBAR]", string.Empty);
            }
            if (stmp.ToLowerInvariant().Contains("am:topicnavigator"))
            {
                stmp = stmp.Replace("[PORTALID]", PortalId.ToString());
                stmp = stmp.Replace("[MODULEID]", ForumModuleId.ToString());
                stmp = stmp.Replace("[TABID]", ForumTabId.ToString());
                this.Controls.Add(this.ParseControl(stmp));
                LinkControls(this.Controls);
                SkipBindTopic = true;
                return;
            }
            DataSet ds = DataProvider.Instance().UI_TopicView(PortalId, ModuleId, ForumId, TopicId, UserId, RowIndex, PageSize, UserInfo.IsSuperUser, DefaultSort);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count == 0)
                {
                    Response.Redirect(Utilities.NavigateUrl(TabId));
                }

                drForum = ds.Tables[0].Rows[0];
                drSecurity = ds.Tables[1].Rows[0];
                dtTopic = ds.Tables[2];
                if (dtTopic.Rows.Count == 0 && PageId > 1)
                {
                    if (MainSettings.UseShortUrls)
                    {
                        Response.Redirect(Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.TopicId + "=" + TopicId }));
                    }
                    else
                    {
                        Response.Redirect(Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId }));
                    }
                }
                else if (dtTopic.Rows.Count == 0 && PageId == 1)
                {
                    if (MainSettings.UseShortUrls)
                    {
                        Response.Redirect(Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId }));
                    }
                    else
                    {
                        Response.Redirect(Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topics }));
                    }
                }

                dtAttach = ds.Tables[3];
                bView = Permissions.HasPerm(drSecurity["CanView"].ToString(), ForumUser.UserRoles);
                bRead = Permissions.HasPerm(drSecurity["CanRead"].ToString(), ForumUser.UserRoles);
                bCreate = Permissions.HasPerm(drSecurity["CanCreate"].ToString(), ForumUser.UserRoles);
                bEdit = Permissions.HasPerm(drSecurity["CanEdit"].ToString(), ForumUser.UserRoles);
                bDelete = Permissions.HasPerm(drSecurity["CanDelete"].ToString(), ForumUser.UserRoles);
                bReply = Permissions.HasPerm(drSecurity["CanReply"].ToString(), ForumUser.UserRoles);
                bPoll = Permissions.HasPerm(drSecurity["CanPoll"].ToString(), ForumUser.UserRoles);
                bAttach = Permissions.HasPerm(drSecurity["CanAttach"].ToString(), ForumUser.UserRoles);
                bSubscribe = Permissions.HasPerm(drSecurity["CanSubscribe"].ToString(), ForumUser.UserRoles);
                bModMove = Permissions.HasPerm(drSecurity["CanModMove"].ToString(), ForumUser.UserRoles);
                bModSplit = Permissions.HasPerm(drSecurity["CanModSplit"].ToString(), ForumUser.UserRoles);
                bModDelete = Permissions.HasPerm(drSecurity["CanModDelete"].ToString(), ForumUser.UserRoles);
                bModApprove = Permissions.HasPerm(drSecurity["CanModApprove"].ToString(), ForumUser.UserRoles);
                bTrust = Permissions.HasPerm(drSecurity["CanTrust"].ToString(), ForumUser.UserRoles);
                bModEdit = Permissions.HasPerm(drSecurity["CanModEdit"].ToString(), ForumUser.UserRoles);
                IsTrusted = Utilities.IsTrusted((int)ForumInfo.DefaultTrustValue, ForumUser.TrustLevel, Permissions.HasPerm(ForumInfo.Security.Trust, ForumUser.UserRoles));


                if (bRead)
                {
                    ForumName = drForum["ForumName"].ToString();
                    GroupName = drForum["GroupName"].ToString();
                    ForumGroupId = Convert.ToInt32(drForum["ForumGroupId"]);
                    TopicTemplateId = Convert.ToInt32(drForum["TopicTemplateId"]);
                    bAllowRSS = Convert.ToBoolean(drForum["AllowRSS"]);
                    bLocked = Convert.ToBoolean(drForum["IsLocked"]);
                    TopicType = Convert.ToInt32(drForum["TopicType"]);
                    StatusId = Convert.ToInt32(drForum["StatusId"]);
                    TopicSubject = Convert.ToString(drForum["Subject"]);
                    TopicDescription = Utilities.StripHTMLTag(drForum["Body"].ToString());
                    Tags = drForum["Tags"].ToString();
                    ViewCount = Convert.ToInt32(drForum["ViewCount"]);
                    ReplyCount = Convert.ToInt32(drForum["ReplyCount"]);
                    TopicAuthorId = Convert.ToInt32(drForum["AuthorId"]);
                    TopicAuthorDisplayName = drForum["TopicAuthor"].ToString();
                    TopicRating = Convert.ToInt32(drForum["TopicRating"]);
                    AllowHTML = Convert.ToBoolean(drForum["AllowHTML"]);
                    AllowScript = Convert.ToBoolean(drForum["AllowScript"]);
                    AllowTags = Convert.ToInt32(drForum["AllowTags"]) == 1;
                    RowCount = Convert.ToInt32(drForum["ReplyCount"]) + 1;
                    NextTopic = Convert.ToInt32(drForum["NextTopic"]);
                    PrevTopic = Convert.ToInt32(drForum["PrevTopic"]);
                    LastPostDate = GetDate(Convert.ToDateTime(drForum["LastPostDate"]));
                    LastPostAuthor.AuthorId = Convert.ToInt32(drForum["LastPostAuthorId"]);
                    LastPostAuthor.DisplayName = drForum["LastPostDisplayName"].ToString();
                    LastPostAuthor.FirstName = drForum["LastPostFirstName"].ToString();
                    LastPostAuthor.LastName = drForum["LastPostLastName"].ToString();
                    LastPostAuthor.Username = drForum["LastPostUserName"].ToString();
                    TopicURL = drForum["URL"].ToString();
                    TopicDateCreated = Utilities.GetDate(Convert.ToDateTime(drForum["DateCreated"].ToString()), ForumModuleId, TimeZoneOffset);
                    TopicData = drForum["TopicData"].ToString();
                    if (UserId > 0)
                    {
                        IsSubscribedTopic = Convert.ToInt32(drForum["IsSubscribedTopic"]) > 0;
                        IsSubscribedForum = Convert.ToInt32(drForum["IsSubscribedForum"]) > 0;
                    }
                    if (!Page.IsPostBack)
                    {
                        if (Request.Params["ptarget"] != null | Request.Params[ParamKeys.ContentJumpId] != null)
                        {
                            int intPages = 0;
                            intPages = Convert.ToInt32(System.Math.Ceiling(RowCount / (double)PageSize));
                            string sTarget = "";
                            if (Request.Params["ptarget"] != null)
                            {
                                sTarget = "#" + Request.Params["ptarget"];
                            }
                            else if (Request.Params[ParamKeys.ContentJumpId] != null)
                            {
                                sTarget = "#" + Request.Params[ParamKeys.ContentJumpId];

                            }
                            if (Request.IsAuthenticated)
                            {
                                if (MainSettings.URLRewriteEnabled && Request.QueryString["asg"] == null & !(string.IsNullOrEmpty(TopicURL)))
                                {
                                    Data.Common db = new Data.Common();
                                    sURL = db.GetUrl(ModuleId, ForumGroupId, ForumId, TopicId, this.UserId, Convert.ToInt32(Request.Params[ParamKeys.ContentJumpId]));
                                    if (!(sURL.StartsWith("/")))
                                    {
                                        sURL = "/" + sURL;
                                    }
                                    if (!(sURL.EndsWith("/")))
                                    {
                                        sURL += "/";
                                    }
                                    sURL += sTarget;
                                }
                                else
                                {
                                    sURL = string.Empty;
                                }
                                if (string.IsNullOrEmpty(sURL))
                                {
                                    string[] Params = { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=" + Views.Topic };
                                    if (MainSettings.UseShortUrls)
                                    {
                                        Params = new string[] { ParamKeys.TopicId + "=" + TopicId };
                                    }
                                    if (intPages > 1 && NoPaging == false)
                                    {
                                        Params = Utilities.AddParams(ParamKeys.PageJumpId + "=" + intPages, Params);
                                    }
                                    sURL = Utilities.NavigateUrl(TabId, "", Params) + sTarget;
                                }

                                Response.Redirect(sURL);
                            }
                            else
                            {
                                if (MainSettings.URLRewriteEnabled && !(string.IsNullOrEmpty(TopicURL)))
                                {
                                    Data.Common db = new Data.Common();
                                    sURL = db.GetUrl(ModuleId, ForumGroupId, ForumId, TopicId, -1, Convert.ToInt32(Request.Params[ParamKeys.ContentJumpId]));
                                    if (!(sURL.StartsWith("/")))
                                    {
                                        sURL = "/" + sURL;
                                    }
                                    if (!(sURL.EndsWith("/")))
                                    {
                                        sURL += "/";
                                    }
                                    sURL += sTarget;
                                }
                                else
                                {
                                    sURL = string.Empty;
                                }
                                if (string.IsNullOrEmpty(sURL))
                                {
                                    string[] Params = { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=" + Views.Topic };
                                    if (MainSettings.UseShortUrls)
                                    {
                                        Params = new string[] { ParamKeys.TopicId + "=" + TopicId };
                                    }
                                    if (intPages > 1 && NoPaging == false)
                                    {
                                        Params = Utilities.AddParams(ParamKeys.PageId + "=" + intPages, Params);
                                    }
                                    sURL = Utilities.NavigateUrl(TabId, "", Params) + sTarget;
                                }

                                Response.Status = "301 Moved Permanently";
                                Response.AddHeader("Location", sURL);
                            }
                        }
                    }
                }
                else
                {
                    DotNetNuke.Entities.Portals.PortalSettings settings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
                    if (settings.LoginTabId > 0)
                    {
                        //Response.Redirect(Utilities.NavigateUrl(settings.LoginTabId, "", "returnUrl=" & Request.RawUrl.ToString), True)
                        Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(settings.LoginTabId, "", "returnUrl=" + Request.RawUrl.ToString()), true);
                    }
                    else
                    {
                        Response.Redirect(Utilities.NavigateUrl(TabId, "", "ctl=login&returnUrl=" + Request.RawUrl.ToString()), true);
                        //Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", "returnUrl=" & Request.RawUrl.ToString), True)
                    }

                }


            }
            else
            {
                Response.Redirect(NavigateUrl(TabId), true);
            }
        }
        private void BindTopic()
        {
            string sOutput = "";
            bool bFullTopic = true;
            if (UseTemplatePath && !(TemplatePath == string.Empty))
            {
                sOutput = Utilities.GetFileContent(TemplatePath + "TopicView.htm");
                sOutput = Utilities.ParseSpacer(sOutput);
            }
            else if (TopicTemplate == string.Empty)
            {
                sOutput = DataCache.GetCachedTemplate(MainSettings.TemplateCache, ModuleId, "TopicView", TopicTemplateId);
            }
            else
            {
                bFullTopic = false;
                sOutput = TopicTemplate;
                sOutput = Utilities.ParseSpacer(sOutput);
            }
            if (sOutput.Contains("[POSTINFO]") && ForumInfo.ProfileTemplateId > 0)
            {
                sOutput = sOutput.Replace("[POSTINFO]", DataCache.GetCachedTemplate(MainSettings.TemplateCache, ModuleId, "ProfileInfo", ForumInfo.ProfileTemplateId));
            }
            if (sOutput.Contains("[META]"))
            {
                MetaTemplate = TemplateUtils.GetTemplateSection(sOutput, "[META]", "[/META]");
                sOutput = TemplateUtils.ReplaceSubSection(sOutput, string.Empty, "[META]", "[/META]");
            }
            sOutput = sOutput.Replace("[PORTALID]", PortalId.ToString());
            sOutput = sOutput.Replace("[MODULEID]", ForumModuleId.ToString());
            sOutput = sOutput.Replace("[TABID]", ForumTabId.ToString());
            sOutput = sOutput.Replace("[TOPICID]", TopicId.ToString());
            sOutput = sOutput.Replace("[AF:CONTROL:FORUMID]", ForumId.ToString());
            sOutput = sOutput.Replace("[AF:CONTROL:FORUMGROUPID]", ForumGroupId.ToString());
            sOutput = sOutput.Replace("[AF:CONTROL:PARENTFORUMID]", ParentForumId.ToString());


            if (sOutput.Contains("<am:TopicsNavigator"))
            {
                this.Controls.Add(this.ParseControl(sOutput));
                LinkControls(this.Controls);
            }
            else
            {
                if (sOutput.Contains("[AF:PROPERTIES"))
                {

                    if (string.IsNullOrEmpty(TopicData))
                    {
                        sOutput = TemplateUtils.ReplaceSubSection(sOutput, string.Empty, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");
                    }
                    else
                    {
                        string sPropTemplate = TemplateUtils.GetTemplateSection(sOutput, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");
                        string sProps = string.Empty;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(TopicData);
                        if (xDoc != null)
                        {
                            System.Xml.XmlNode xRoot = xDoc.DocumentElement;
                            System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes("//properties/property");
                            if (xNodeList.Count > 0)
                            {
                                int i = 0;
                                for (i = 0; i < xNodeList.Count; i++)
                                {
                                    string tmp = sPropTemplate;
                                    string pName = Utilities.HTMLDecode(xNodeList[i].ChildNodes[0].InnerText);
                                    string pValue = Utilities.HTMLDecode(xNodeList[i].ChildNodes[1].InnerText);
                                    tmp = tmp.Replace("[AF:PROPERTY:LABEL]", "[RESX:" + pName + "]");
                                    tmp = tmp.Replace("[AF:PROPERTY:VALUE]", pValue);
                                    sOutput = sOutput.Replace("[AF:PROPERTY:" + pName + ":LABEL]", Utilities.GetSharedResource("[RESX:" + pName + "]"));
                                    sOutput = sOutput.Replace("[AF:PROPERTY:" + pName + ":VALUE]", pValue);
                                    string pValueKey = string.Empty;
                                    if (!(string.IsNullOrEmpty(pValue)))
                                    {
                                        pValueKey = Utilities.CleanName(pValue).ToLowerInvariant();
                                    }
                                    sOutput = sOutput.Replace("[AF:PROPERTY:" + pName + ":VALUEKEY]", pValueKey);
                                    sProps += tmp;
                                }
                            }
                        }
                        sOutput = TemplateUtils.ReplaceSubSection(sOutput, sProps, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");

                    }


                }
                //Parse Meta Template
                if (!(string.IsNullOrEmpty(MetaTemplate)))
                {
                    MetaTemplate = MetaTemplate.Replace("[FORUMNAME]", ForumName);
                    MetaTemplate = MetaTemplate.Replace("[GROUPNAME]", GroupName);
                    string pageName = string.Empty;
                    DotNetNuke.Entities.Portals.PortalSettings settings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
                    if (settings.ActiveTab.Title.Length == 0)
                    {
                        pageName = Server.HtmlEncode(settings.ActiveTab.TabName);
                    }
                    else
                    {
                        pageName = Server.HtmlEncode(settings.ActiveTab.Title);
                    }
                    MetaTemplate = MetaTemplate.Replace("[PAGENAME]", pageName);
                    MetaTemplate = MetaTemplate.Replace("[PORTALNAME]", settings.PortalName);
                    MetaTemplate = MetaTemplate.Replace("[TAGS]", Tags);
                    if (MetaTemplate.Contains("[TOPICSUBJECT:"))
                    {
                        string pattern = "(\\[TOPICSUBJECT:(.+?)\\])";
                        Regex regExp = new Regex(pattern);
                        MatchCollection matches = null;
                        matches = regExp.Matches(MetaTemplate);
                        foreach (Match m in matches)
                        {
                            int iLen = Convert.ToInt32(m.Groups[2].Value);
                            if (TopicSubject.Length > iLen)
                            {
                                MetaTemplate = MetaTemplate.Replace(m.Value, TopicSubject.Substring(0, iLen) + "...");
                            }
                            else
                            {
                                MetaTemplate = MetaTemplate.Replace(m.Value, Utilities.StripHTMLTag(TopicSubject));
                            }
                        }
                    }
                    MetaTemplate = MetaTemplate.Replace("[TOPICSUBJECT]", Utilities.StripHTMLTag(TopicSubject));
                    if (MetaTemplate.Contains("[BODY:"))
                    {
                        string pattern = "(\\[BODY:(.+?)\\])";
                        Regex regExp = new Regex(pattern);
                        MatchCollection matches = null;
                        matches = regExp.Matches(MetaTemplate);
                        foreach (Match m in matches)
                        {
                            int iLen = Convert.ToInt32(m.Groups[2].Value);
                            if (TopicDescription.Length > iLen)
                            {
                                MetaTemplate = MetaTemplate.Replace(m.Value, TopicDescription.Substring(0, iLen) + "...");
                            }
                            else
                            {
                                MetaTemplate = MetaTemplate.Replace(m.Value, TopicDescription);
                            }
                        }
                    }
                    MetaTemplate = MetaTemplate.Replace("[BODY]", TopicDescription);

                    MetaTitle = TemplateUtils.GetTemplateSection(MetaTemplate, "[TITLE]", "[/TITLE]").Replace("[TITLE]", string.Empty).Replace("[/TITLE]", string.Empty);
                    MetaDescription = TemplateUtils.GetTemplateSection(MetaTemplate, "[DESCRIPTION]", "[/DESCRIPTION]").Replace("[DESCRIPTION]", string.Empty).Replace("[/DESCRIPTION]", string.Empty);
                    MetaKeywords = TemplateUtils.GetTemplateSection(MetaTemplate, "[KEYWORDS]", "[/KEYWORDS]").Replace("[KEYWORDS]", string.Empty).Replace("[/KEYWORDS]", string.Empty);
                }
                ControlUtils ctlUtils = new ControlUtils();
                sGroupURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, string.Empty, ForumInfo.ForumGroupId, -1, -1, -1, string.Empty, 1, SocialGroupId);
                sForumURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, -1, -1, string.Empty, 1, SocialGroupId);
                sTopicURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, TopicURL, -1, -1, string.Empty, 1, SocialGroupId);

                if (MainSettings.UseSkinBreadCrumb)
                {
                    string sCrumb = "<a href=\"" + sGroupURL + "\">" + GroupName + "</a>|";
                    sCrumb += "<a href=\"" + sForumURL + "\">" + ForumName + "</a>";
                    sCrumb += "|<a href=\"" + sTopicURL + "\">" + TopicSubject + "</a>";


                    if (Environment.UpdateBreadCrumb(Page.Controls, sCrumb))
                    {
                        sOutput = sOutput.Replace("<div  class=\"afcrumb\">[FORUMMAINLINK] > [FORUMGROUPLINK] > [FORUMLINK]</div>", string.Empty);
                    }

                }
                //Parse Common Controls First
                sOutput = ParseControls(sOutput);
                //Parse Topics and Add to controls
                string topics = TemplateUtils.GetTemplateSection(sOutput, "[AF:CONTROL:CALLBACK]", "[/AF:CONTROL:CALLBACK]");
                topics = ParseTopic(topics);

                topics = "<%@ Register TagPrefix=\"am\" Namespace=\"DotNetNuke.Modules.ActiveForums.Controls\" Assembly=\"DotNetNuke.Modules.ActiveForums\" %>" + topics;

                if (MainSettings.ProfileType == ProfileTypes.Social)
                {
                    if (!(topics.Contains(Globals.SocialRegisterTag)))
                    {
                        topics = Globals.SocialRegisterTag + topics;
                    }
                }

                topics = Utilities.LocalizeControl(topics);
                if (bFullTopic)
                {
                    sOutput = TemplateUtils.ReplaceSubSection(sOutput, "<asp:placeholder id=\"plhTopicCallback\" runat=\"server\" />", "[AF:CONTROL:CALLBACK]", "[/AF:CONTROL:CALLBACK]");
                    //sOutput = ParseTopic(sOutput)
                    sOutput = Utilities.LocalizeControl(sOutput);
                    sOutput = Utilities.StripTokens(sOutput);
                    this.Controls.Add(this.ParseControl(sOutput));
                }



                PlaceHolder plhCB = (PlaceHolder)(this.FindControl("plhTopicCallback"));
                if (plhCB != null)
                {
                    plhTopic.Controls.Clear();
                    plhTopic.ID = "plhTopic";

                    plhTopic.Controls.Add(this.ParseControl(topics));
                    plhCB.Controls.Add(plhTopic);
                    //cbTopicLoader = New DotNetNuke.Modules.ActiveForums.Controls.Callback

                    //cbTopicLoader.OnCallbackComplete = "af_clearLoad"
                    //Dim ct As New Modules.ActiveForums.Controls.CallBackContent
                    //ct.Controls.Add(plhTopic)
                    //cbTopicLoader.Content = ct
                    //plhCB.Controls.Add(cbTopicLoader)
                }
                else
                {
                    plhTopic.Controls.Clear();
                    plhTopic.ID = "plhTopic";
                    plhTopic.Controls.Add(this.ParseControl(topics));
                    this.Controls.Add(plhTopic);

                }
                //Add helper controls
                //Quick Jump DropDownList
                PlaceHolder plh = (PlaceHolder)(this.FindControl("plhQuickJump"));
                if (plh != null)
                {
                    plh.Controls.Clear();
                    ctlForumJump = new af_quickjump();
                    ctlForumJump.ModuleConfiguration = this.ModuleConfiguration;
                    ctlForumJump.MOID = ModuleId;
                    ctlForumJump.dtForums = null;
                    ctlForumJump.ForumId = ForumId;
                    ctlForumJump.EnableViewState = false;
                    if (ForumId > 0)
                    {
                        ctlForumJump.ForumInfo = ForumInfo;
                    }
                    if (!(plh.Controls.Contains(ctlForumJump)))
                    {
                        plh.Controls.Add(ctlForumJump);
                    }

                }
                plh = null;
                //'Poll Container
                plh = (PlaceHolder)(this.FindControl("plhPoll"));
                if (plh != null)
                {
                    ctlPoll = new af_polls();
                    ctlPoll.ModuleConfiguration = this.ModuleConfiguration;
                    ctlPoll.TopicId = TopicId;
                    ctlPoll.ForumId = ForumId;
                    plh.Controls.Add(ctlPoll);
                }
                //Quick Reply
                if (bReply && bLocked == false)
                {
                    plh = (PlaceHolder)(this.FindControl("plhQuickReply"));
                    if (plh != null)
                    {
                        ctlQuickReply = (af_quickreplyform)(LoadControl("~/DesktopModules/ActiveForums/controls/af_quickreply.ascx"));
                        ctlQuickReply.ModuleConfiguration = this.ModuleConfiguration;
                        //ctlQuickReply.CanReply = bReply
                        ctlQuickReply.CanTrust = bTrust;
                        ctlQuickReply.ModApprove = bModApprove;
                        ctlQuickReply.IsTrusted = IsTrusted;
                        //ctlQuickReply.ThemePath = MyThemePath
                        ctlQuickReply.Subject = TopicSubject;
                        ctlQuickReply.AllowSubscribe = AllowSubscribe;
                        ctlQuickReply.AllowHTML = AllowHTML;
                        ctlQuickReply.AllowScripts = AllowScript;
                        ctlQuickReply.ForumId = ForumId;
                        ctlQuickReply.SocialGroupId = SocialGroupId;
                        ctlQuickReply.ForumModuleId = ForumModuleId;
                        ctlQuickReply.ForumTabId = TabId;
                        if (ForumId > 0)
                        {
                            ctlQuickReply.ForumInfo = ForumInfo;
                        }
                        if (!(plh.Controls.Contains(ctlQuickReply)))
                        {
                            plh.Controls.Add(ctlQuickReply);
                        }

                    }
                }
                plh = (PlaceHolder)(this.FindControl("plhTopicSort"));
                if (plh != null)
                {
                    plh.Controls.Clear();
                    ctlTopicSort = (af_topicsorter)(LoadControl("~/DesktopModules/ActiveForums/controls/af_topicsort.ascx"));
                    ctlTopicSort.ModuleConfiguration = this.ModuleConfiguration;
                    ctlTopicSort.ForumId = ForumId;
                    ctlTopicSort.DefaultSort = DefaultSort;
                    if (ForumId > 0)
                    {
                        ctlTopicSort.ForumInfo = ForumInfo;
                    }
                    if (!(plh.Controls.Contains(ctlTopicSort)))
                    {
                        plh.Controls.Add(ctlTopicSort);
                    }

                }
                plh = (PlaceHolder)(this.FindControl("plhStatus"));
                if (plh != null)
                {
                    plh.Controls.Clear();
                    ctlTopicStatus = (af_topicstatus)(LoadControl("~/DesktopModules/ActiveForums/controls/af_topicstatus.ascx"));
                    ctlTopicStatus.ModuleConfiguration = this.ModuleConfiguration;
                    ctlTopicStatus.Status = StatusId;
                    ctlTopicStatus.ForumId = ForumId;
                    if (ForumId > 0)
                    {
                        ctlTopicStatus.ForumInfo = ForumInfo;
                    }
                    if (!(plh.Controls.Contains(ctlTopicStatus)))
                    {
                        plh.Controls.Add(ctlTopicStatus);
                    }

                }
                //Dim sPage As String = "function afPageTopic(page){"
                //sPage &= "af_showLoad();window.scrollTo(0,0);"
                //sPage &= cbTopicLoader.ClientID & ".Callback(page);"
                //sPage &= "};"
                //Page.ClientScript.RegisterClientScriptBlock(Page.GetType, "aftopicpg", sPage, True)
                BuildPager();
            }










        }
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
        }
        private string ParseControls(string sOutput)
        {
            if (Request.QueryString["dnnprintmode"] != null)
            {
                sOutput = sOutput.Replace("[ADDREPLY]", string.Empty);
                sOutput = sOutput.Replace("[QUICKREPLY]", string.Empty);
                sOutput = sOutput.Replace("[TOPICSUBSCRIBE]", string.Empty);
                sOutput = sOutput.Replace("[AF:CONTROL:PRINTER]", string.Empty);
                sOutput = sOutput.Replace("[AF:CONTROL:EMAIL]", string.Empty);
                sOutput = sOutput.Replace("[PAGER1]", string.Empty);
                sOutput = sOutput.Replace("[PAGER2]", string.Empty);
                sOutput = sOutput.Replace("[SORTDROPDOWN]", string.Empty);
                sOutput = sOutput.Replace("[POSTRATINGBUTTON]", string.Empty);
                sOutput = sOutput.Replace("[JUMPTO]", string.Empty);
                sOutput = sOutput.Replace("[NEXTTOPIC]", string.Empty);
                sOutput = sOutput.Replace("[PREVTOPIC]", string.Empty);
                sOutput = sOutput.Replace("[AF:CONTROL:STATUS]", string.Empty);
                sOutput = sOutput.Replace("[ACTIONS:DELETE]", string.Empty);
                sOutput = sOutput.Replace("[ACTIONS:EDIT]", string.Empty);
                sOutput = sOutput.Replace("[ACTIONS:QUOTE]", string.Empty);
                sOutput = sOutput.Replace("[ACTIONS:REPLY]", string.Empty);
                sOutput = sOutput.Replace("[ACTIONS:ANSWER]", string.Empty);
                sOutput = sOutput.Replace("[ACTIONS:ALERT]", string.Empty);
                sOutput = sOutput.Replace("[RESX:SortPosts]:", string.Empty);
                sOutput = sOutput + "<img src=\"~/desktopmodules/activeforums/images/spacer.gif\" width=\"800\" height=\"1\" runat=\"server\" alt=\"---\" />";
            }
            if (sOutput.Contains("[AF:CONTROL:ADDTHIS"))
            {
                string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request));
                sOutput = TemplateUtils.ParseSpecial(sOutput, SpecialTokenTypes.AddThis, strHost + Request.RawUrl, TopicSubject, bRead, MainSettings.AddThisAccount);
            }
            sOutput = sOutput.Replace("[NOPAGING]", "<script type=\"text/javascript\">afpagesize=" + int.MaxValue + ";</script>");
            sOutput = sOutput.Replace("[NOTOOLBAR]", string.Empty);
            if (bSubscribe == true)
            {
                Controls.ToggleSubscribe subControl = new Controls.ToggleSubscribe(1, ForumId, TopicId);
                subControl.Checked = IsSubscribedTopic;
                subControl.Text = "[RESX:TopicSubscribe:" + IsSubscribedTopic.ToString().ToUpper() + "]";
                sOutput = sOutput.Replace("[TOPICSUBSCRIBE]", subControl.Render());
            }
            else
            {
                sOutput = sOutput.Replace("[TOPICSUBSCRIBE]", string.Empty);
            }
            TokensController tc = new TokensController();
            string topicActions = tc.TokenGet("topic", "[AF:CONTROL:TOPICACTIONS]");
            string postActions = tc.TokenGet("topic", "[AF:CONTROL:POSTACTIONS]");
            if (sOutput.Contains("[AF:CONTROL:TOPICACTIONS]"))
            {
                UseListActions = true;
                sOutput = sOutput.Replace("[AF:CONTROL:TOPICACTIONS]", topicActions);
                sOutput = sOutput.Replace("[AF:CONTROL:POSTACTIONS]", postActions);
            }


            if (sOutput.IndexOf("[BANNER") > 0)
            {
                int bannerCount = 1;
                sOutput = "<%@ Register TagPrefix=\"dnn\" TagName=\"BANNER\" Src=\"~/Admin/Skins/Banner.ascx\" %>" + sOutput;
                sOutput = sOutput.Replace("[BANNER]", "<dnn:BANNER runat=\"server\" id=\"dnnBANNER" + bannerCount + "\" BannerTypeId=\"-1\" GroupName=\"FORUMS\" EnableViewState=\"False\" />");
                string pattern = "(\\[BANNER:(.+?)\\])";
                System.Text.RegularExpressions.Regex regExp = new System.Text.RegularExpressions.Regex(pattern);
                System.Text.RegularExpressions.MatchCollection matches = null;
                matches = regExp.Matches(sOutput);
                string sBanner = "<dnn:BANNER runat=\"server\" id=\"dnnBANNER{0}\" BannerTypeId=\"-1\" GroupName=\"{1}\" EnableViewState=\"False\" />";
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    string sReplace = null;
                    bannerCount += 1;
                    sReplace = string.Format(sBanner, bannerCount, match.Groups[2].Value);
                    sOutput = regExp.Replace(sOutput, sReplace, 1);
                }
            }
            if (bLocked == true)
            {
                sOutput = sOutput.Replace("[ADDREPLY]", "<span class=\"afnormal\">[RESX:TopicLocked]</span>");
                sOutput = sOutput.Replace("[QUICKREPLY]", string.Empty);
            }
            else
            {
                //TODO: Check for owner
                if (bReply)
                {
                    string[] Params = { ParamKeys.ViewType + "=post", ParamKeys.TopicId + "=" + TopicId, ParamKeys.ForumId + "=" + ForumId };
                    if (SocialGroupId > 0)
                    {
                        Params = Utilities.AddParams("GroupId=" + SocialGroupId, Params);
                    }
                    sOutput = sOutput.Replace("[ADDREPLY]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", Params) + "\" class=\"dnnPrimaryAction\">[RESX:AddReply]</a>");
                    sOutput = sOutput.Replace("[QUICKREPLY]", "<asp:placeholder id=\"plhQuickReply\" runat=\"server\" />");
                }
                else
                {
                    sOutput = sOutput.Replace("[ADDREPLY]", "<span class=\"afnormal\">[RESX:NotAuthorizedReply]</span>");
                    sOutput = sOutput.Replace("[QUICKREPLY]", string.Empty);
                }
            }
            if (sOutput.Contains("[PARENTFORUMLINK]"))
            {
                if (ForumInfo.ParentForumId > 0)
                {
                    if (MainSettings.UseShortUrls)
                    {
                        sOutput = sOutput.Replace("[PARENTFORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumInfo.ParentForumId }) + "\">" + ForumInfo.ParentForumName + "</a>");
                    }
                    else
                    {
                        sOutput = sOutput.Replace("[PARENTFORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumInfo.ParentForumId }) + "\">" + ForumInfo.ParentForumName + "</a>");
                    }

                }
                else if (ForumInfo.ForumGroupId > 0)
                {
                    sOutput = sOutput.Replace("[PARENTFORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId) + "\">" + ForumInfo.GroupName + "</a>");
                }
            }
            if (string.IsNullOrEmpty(ForumInfo.ParentForumName))
            {
                sOutput = sOutput.Replace("[PARENTFORUMNAME]", ForumInfo.ParentForumName);
            }
            sOutput = sOutput.Replace("[FORUMMAINLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId) + "\">[RESX:ForumMain]</a>");
            sOutput = sOutput.Replace("[FORUMGROUPLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + ForumGroupId) + "\">" + GroupName + "</a>");
            if (MainSettings.UseShortUrls)
            {
                sOutput = sOutput.Replace("[FORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId) + "\">" + ForumName + "</a>");
            }
            else
            {
                sOutput = sOutput.Replace("[FORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.ViewType + "=" + Views.Topics + "&" + ParamKeys.ForumId + "=" + ForumId) + "\">" + ForumName + "</a>");
            }

            sOutput = sOutput.Replace("[FORUMID]", ForumId.ToString());
            sOutput = sOutput.Replace("[FORUMNAME]", ForumName);
            sOutput = sOutput.Replace("[GROUPNAME]", GroupName);
            string sURL = "<a rel=\"nofollow\" href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId, "mid=" + ModuleId.ToString(), "dnnprintmode=true") + "?skinsrc=" + HttpUtility.UrlEncode("[G]" + DotNetNuke.UI.Skins.SkinInfo.RootSkin + "/" + DotNetNuke.Common.Globals.glbHostSkinFolder + "/" + "No Skin") + "&amp;containersrc=" + HttpUtility.UrlEncode("[G]" + DotNetNuke.UI.Skins.SkinInfo.RootContainer + "/" + DotNetNuke.Common.Globals.glbHostSkinFolder + "/" + "No Container") + "\" target=\"_blank\">";
            sURL += "<img src=\"" + MyThemePath + "/images/print16.png\" border=\"0\" alt=\"[RESX:PrinterFriendly]\" /></a>";
            sOutput = sOutput.Replace("[AF:CONTROL:PRINTER]", sURL);
            if (Request.IsAuthenticated)
            {
                sURL = Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=sendto", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId });
                sOutput = sOutput.Replace("[AF:CONTROL:EMAIL]", "<a href=\"" + sURL + "\" rel=\"nofollow\"><img src=\"" + MyThemePath + "/images/email16.png\" border=\"0\" alt=\"[RESX:EmailThis]\" /></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[AF:CONTROL:EMAIL]", string.Empty);
            }
            if (bAllowRSS)
            {
                string Url = null;
                Url = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/DesktopModules/ActiveForums/feeds.aspx?portalid=" + PortalId + "&forumid=" + ForumId + "&tabid=" + TabId + "&moduleid=" + ModuleId;
                sOutput = sOutput.Replace("[RSSLINK]", "<a href=\"" + Url + "\"><img src=\"~/DesktopModules/ActiveForums/themes/" + MyTheme + "/images/rss.png\" runat=server border=\"0\" alt=\"[RESX:RSS]\" /></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[RSSLINK]", string.Empty);
            }
            TopicSubject = TopicSubject.Replace("[", "&#91");
            TopicSubject = TopicSubject.Replace("]", "&#93");
            sOutput = sOutput.Replace("[SUBJECT]", Utilities.StripHTMLTag(TopicSubject));
            sOutput = sOutput.Replace("[REPLYCOUNT]", ReplyCount.ToString());
            sOutput = sOutput.Replace("[AF:LABEL:ReplyCount]", ReplyCount.ToString());
            sOutput = sOutput.Replace("[VIEWCOUNT]", ViewCount.ToString());
            sOutput = sOutput.Replace("[AF:LABEL:LastPostDate]", LastPostDate);
            sOutput = sOutput.Replace("[AF:LABEL:LastPostAuthor]", UserProfiles.GetDisplayName(ModuleId, MainSettings.MemberListMode, bModApprove, LastPostAuthor.AuthorId, MainSettings.UserNameDisplay, LastPostAuthor));
            sOutput = sOutput.Replace("[AF:LABEL:TopicAuthor]", UserProfiles.GetDisplayName(ModuleId, TopicAuthorId, MainSettings.MemberListMode, TopicAuthorDisplayName, string.Empty, string.Empty, TopicAuthorDisplayName));
            sOutput = sOutput.Replace("[AF:LABEL:TopicDateCreated]", TopicDateCreated);
            if (PageSize == int.MaxValue)
            {
                sOutput = sOutput.Replace("[PAGER1]", string.Empty);
                sOutput = sOutput.Replace("[PAGER2]", string.Empty);
            }
            else
            {
                sOutput = sOutput.Replace("[PAGER1]", "<am:pagernav id=\"Pager1\" runat=\"server\" EnableViewState=\"False\" />");
                sOutput = sOutput.Replace("[PAGER2]", "<am:pagernav id=\"Pager2\" runat=\"server\" EnableViewState=\"False\" />");
            }

            sOutput = sOutput.Replace("[SORTDROPDOWN]", "<asp:placeholder id=\"plhTopicSort\" runat=\"server\" />");
            Ratings rateControl = new Ratings(TopicId, true, TopicRating);
            sOutput = sOutput.Replace("[POSTRATINGBUTTON]", rateControl.Render());
            sOutput = sOutput.Replace("[JUMPTO]", "<asp:placeholder id=\"plhQuickJump\" runat=\"server\" />");
            if (NextTopic == 0)
            {
                sOutput = sOutput.Replace("[NEXTTOPIC]", string.Empty);
            }
            else
            {
                if (MainSettings.UseShortUrls)
                {
                    sOutput = sOutput.Replace("[NEXTTOPIC]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + NextTopic) + "\" rel=\"nofollow\"><span>[RESX:NextTopic]</span><img src=\"~/DesktopModules/ActiveForums/themes/" + MyTheme + "/images/arrow_right_blue.gif\" runat=server style=\"vertical-align:middle;\" border=\"0\" alt=\"[RESX:NextTopic]\" /></a>");
                }
                else
                {
                    sOutput = sOutput.Replace("[NEXTTOPIC]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId + "&" + ParamKeys.TopicId + "=" + NextTopic + "&" + ParamKeys.ViewType + "=" + Views.Topic) + "\" rel=\"nofollow\"><span>[RESX:NextTopic]</span><img src=\"~/DesktopModules/ActiveForums/themes/" + MyTheme + "/images/arrow_right_blue.gif\" runat=server style=\"vertical-align:middle;\" border=\"0\" alt=\"[RESX:NextTopic]\" /></a>");
                }

            }
            if (PrevTopic == 0)
            {
                sOutput = sOutput.Replace("[PREVTOPIC]", string.Empty);
            }
            else
            {
                if (MainSettings.UseShortUrls)
                {
                    sOutput = sOutput.Replace("[PREVTOPIC]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + PrevTopic) + "\" rel=\"nofollow\"><img src=\"~/DesktopModules/ActiveForums/themes/" + MyTheme + "/images/arrow_left_blue.gif\" runat=server style=\"vertical-align:middle;\" border=\"0\" alt=\"[RESX:PrevTopic]\" /><span>[RESX:PrevTopic]</span></a>");
                }
                else
                {
                    sOutput = sOutput.Replace("[PREVTOPIC]", "<a href=\"" + Utilities.NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId + "&" + ParamKeys.TopicId + "=" + PrevTopic + "&" + ParamKeys.ViewType + "=" + Views.Topic) + "\" rel=\"nofollow\"><img src=\"~/DesktopModules/ActiveForums/themes/" + MyTheme + "/images/arrow_left_blue.gif\" runat=server style=\"vertical-align:middle;\" border=\"0\" alt=\"[RESX:PrevTopic]\" /><span>[RESX:PrevTopic]</span></a>");
                }

            }


            if (((bRead && TopicAuthorId == this.UserId) || bModEdit) & StatusId >= 0)
            {
                sOutput = sOutput.Replace("[AF:CONTROL:STATUS]", "<asp:placeholder id=\"plhStatus\" runat=\"server\" />");
                sOutput = sOutput.Replace("[AF:CONTROL:STATUSICON]", "<img alt=\"[RESX:PostStatus" + StatusId.ToString() + "]\" src=\"" + MyThemePath + "/images/status" + StatusId.ToString() + ".png\" />");
            }
            else if (StatusId >= 0)
            {
                sOutput = sOutput.Replace("[AF:CONTROL:STATUS]", string.Empty);
                sOutput = sOutput.Replace("[AF:CONTROL:STATUSICON]", "<img alt=\"[RESX:PostStatus" + StatusId.ToString() + "]\" src=\"" + MyThemePath + "/images/status" + StatusId.ToString() + ".png\" />");
            }
            else
            {
                sOutput = sOutput.Replace("[AF:CONTROL:STATUS]", string.Empty);
                sOutput = sOutput.Replace("[AF:CONTROL:STATUSICON]", string.Empty);
            }

            if (TopicType == (int)TopicTypes.Poll)
            {
                sOutput = sOutput.Replace("[AF:CONTROL:POLL]", "<asp:placeholder id=\"plhPoll\" runat=\"server\" />");
            }
            else
            {
                sOutput = sOutput.Replace("[AF:CONTROL:POLL]", string.Empty);
            }
            return sOutput;
        }
        private string ParseTopic(string sOutput)
        {
            string sTopicTemplate = TemplateUtils.GetTemplateSection(sOutput, "[TOPIC]", "[/TOPIC]");
            string sReplyTemplate = TemplateUtils.GetTemplateSection(sOutput, "[REPLIES]", "[/REPLIES]");
            string sTemp = string.Empty;
            int i = 0;
            if (dtTopic.Rows.Count > 0)
            {
                foreach (DataRow dr in dtTopic.Rows)
                {
                    int ReplyId = Convert.ToInt32(dr["ReplyId"]);

                    if (ReplyId == 0)
                    {
                        sTopicTemplate = ParseContent(dr, sTopicTemplate, i);
                        //sOutput = TemplateUtils.ReplaceSubSection(sOutput, sTopicTemplate, "[TOPIC]", "[/TOPIC]")
                    }
                    else
                    {
                        sTemp += ParseContent(dr, sReplyTemplate, i);
                    }
                    i += 1;
                }
                if (DefaultSort == "ASC")
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
            string sOutput = tempate;
            if (rowcount % 2 == 0)
            {
                sOutput = sOutput.Replace("[POSTINFOCSS]", "afpostinfo1");
                sOutput = sOutput.Replace("[POSTTOPICCSS]", "afpostreply1");
                sOutput = sOutput.Replace("[POSTREPLYCSS]", "afpostreply1");
            }
            else
            {
                sOutput = sOutput.Replace("[POSTTOPICCSS]", "afpostreply2");
                sOutput = sOutput.Replace("[POSTINFOCSS]", "afpostinfo2");
                sOutput = sOutput.Replace("[POSTREPLYCSS]", "afpostreply2");
            }
            int ReplyId = Convert.ToInt32(dr["ReplyId"]);
            int TopicId = Convert.ToInt32(dr["TopicId"]);
            int ContentId = Convert.ToInt32(dr["ContentId"]);
            int PostId = 0;
            DateTime DateCreated = Convert.ToDateTime(dr["DateCreated"]);
            DateTime DateUpdated = Convert.ToDateTime(dr["DateUpdated"]);
            int AuthorId = Convert.ToInt32(dr["AuthorId"]);
            string Username = Convert.ToString(dr["Username"]);
            string FirstName = Convert.ToString(dr["FirstName"]);
            string LastName = Convert.ToString(dr["LastName"]);
            string DisplayName = Convert.ToString(dr["DisplayName"]);
            int UserTopicCount = Convert.ToInt32(dr["TopicCount"]);
            int UserReplyCount = Convert.ToInt32(dr["ReplyCount"]);
            string UserCaption = Convert.ToString(dr["UserCaption"]);
            string Body = Convert.ToString(dr["Body"]);
            string Subject = Convert.ToString(dr["Subject"]);
            string Tags = dr["Tags"].ToString();
            string Signature = Convert.ToString(dr["Signature"]);
            string IPAddress = Convert.ToString(dr["IPAddress"]);
            string Avatar = Convert.ToString(dr["Avatar"]);
            int AvatarType = Convert.ToInt32(dr["AvatarType"]);
            string Yahoo = Convert.ToString(dr["Yahoo"]);
            string MSN = Convert.ToString(dr["MSN"]);
            string ICQ = Convert.ToString(dr["ICQ"]);
            string AOL = Convert.ToString(dr["AOL"]);
            string Occupation = Convert.ToString(dr["Occupation"]);
            string Location = Convert.ToString(dr["Location"]);
            string Interests = Convert.ToString(dr["Interests"]);
            string WebSite = Convert.ToString(dr["WebSite"]);
            DateTime MemberSince = Convert.ToDateTime(dr["MemberSince"]);
            bool AvatarDisabled = Convert.ToBoolean(dr["AvatarDisabled"]);
            string UserRoles = Convert.ToString(dr["UserRoles"]);
            bool IsUserOnline = Convert.ToBoolean(dr["IsUserOnline"]);
            int ReplyStatusId = Convert.ToInt32(dr["StatusId"]);
            int TotalPoints = 0;
            int AnswerCount = Convert.ToInt32(dr["AnswerCount"]);
            int RewardPoints = Convert.ToInt32(dr["RewardPoints"]);
            DateTime DateLastActivity = Convert.ToDateTime(dr["DateLastActivity"]);
            bool SignatureDisabled = Convert.ToBoolean(dr["SignatureDisabled"]);
            //TODO:  Need to finish adding points
            int PostCount = UserTopicCount + UserReplyCount;
            if (EnablePoints)
            {
                TotalPoints = Convert.ToInt32(dr["UserTotalPoints"]);
            }
            User up = new User();
            up.UserId = AuthorId;
            up.UserName = Username;
            up.FirstName = FirstName.Replace("&amp;#", "&#");
            up.LastName = LastName.Replace("&amp;#", "&#");
            up.DisplayName = DisplayName.Replace("&amp;#", "&#");

            up.Profile.UserCaption = UserCaption;
            up.Profile.Signature = Signature;
            up.Profile.Avatar = Avatar;
            up.Profile.AvatarType = (AvatarTypes)AvatarType;
            up.Profile.Yahoo = Yahoo;
            up.Profile.MSN = MSN;
            up.Profile.ICQ = ICQ;
            up.Profile.AOL = AOL;
            up.Profile.Occupation = Occupation;
            up.Profile.Location = Location;
            up.Profile.Interests = Interests;
            up.Profile.WebSite = WebSite;
            up.Profile.DateCreated = MemberSince;
            up.Profile.AvatarDisabled = AvatarDisabled;
            up.Profile.Roles = UserRoles;
            up.Profile.ReplyCount = UserReplyCount;
            up.Profile.TopicCount = UserTopicCount;
            up.Profile.AnswerCount = AnswerCount;
            up.Profile.RewardPoints = RewardPoints;
            up.Profile.DateLastActivity = DateLastActivity;
            up.Profile.PrefBlockAvatars = UserPrefHideAvatars;
            up.Profile.PrefBlockSignatures = UserPrefHideSigs;
            up.Profile.IsUserOnline = IsUserOnline;
            up.Profile.SignatureDisabled = SignatureDisabled;



            if (ReplyId == 0)
            {
                sOutput = sOutput.Replace("[POSTID]", TopicId.ToString());
                PostId = TopicId;
                TopicDescription = Utilities.StripHTMLTag(Body).Trim();
                TopicDescription = TopicDescription.Replace(System.Environment.NewLine, string.Empty);
                if (TopicDescription.Length > 255)
                {
                    TopicDescription = TopicDescription.Substring(0, 255);
                }


            }
            else
            {
                sOutput = sOutput.Replace("[POSTID]", ReplyId.ToString());
                PostId = ReplyId;
            }
            sOutput = sOutput.Replace("[FORUMID]", ForumId.ToString());
            sOutput = sOutput.Replace("[REPLYID]", ReplyId.ToString());
            sOutput = sOutput.Replace("[TOPICID]", TopicId.ToString());
            sOutput = sOutput.Replace("[POSTDATE]", GetDate(DateCreated));
            sOutput = sOutput.Replace("[DATECREATED]", GetDate(DateCreated));
            if (Tags == string.Empty)
            {
                sOutput = TemplateUtils.ReplaceSubSection(sOutput, string.Empty, "[AF:CONTROL:TAGS]", "[/AF:CONTROL:TAGS]");
            }
            else
            {
                sOutput = sOutput.Replace("[AF:CONTROL:TAGS]", string.Empty);
                sOutput = sOutput.Replace("[/AF:CONTROL:TAGS]", string.Empty);
                string tagList = string.Empty;
                int i = 0;
                foreach (string tag in Tags.Split(','))
                {
                    tagList += "<a href=\"" + Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=search", ParamKeys.Tags + "=" + HttpUtility.UrlEncode(tag) }) + "\">" + tag + "</a>";
                    i += 1;
                    if (i <= Tags.Split(',').GetUpperBound(0))
                    {
                        tagList += ", ";
                    }

                }
                sOutput = sOutput.Replace("[AF:LABEL:TAGS]", tagList);
            }
            //Perform Profile Related replacements
            sOutput = TemplateUtils.ParseProfileTemplate(sOutput, up, PortalId, ModuleId, ImagePath, CurrentUserType, true, UserPrefHideAvatars, UserPrefHideSigs, IPAddress, TimeZoneOffset);

            if (bModDelete || ((bDelete && AuthorId == UserId && bLocked == false) & ((ReplyId == 0 && ReplyCount == 0) | ReplyId > 0)))
            {
                if (UseListActions)
                {
                    sOutput = sOutput.Replace("[ACTIONS:DELETE]", "<li class=\"af-delete\" onclick=\"amaf_postDel(" + TopicId + "," + ReplyId + ");\" title=\"[RESX:Delete]\"><em></em>[RESX:Delete]</li>");
                }
                else
                {
                    sOutput = sOutput.Replace("[ACTIONS:DELETE]", "<a href=\"javascript:void(0);\" class=\"af-actions af-delete\" onclick=\"amaf_postDel(" + TopicId + "," + ReplyId + ");\" title=\"[RESX:Delete]\"><em></em>[RESX:Delete]</a>");
                }

            }
            else
            {
                sOutput = sOutput.Replace("[ACTIONS:DELETE]", string.Empty);
            }
            if (bModEdit || (bEdit && AuthorId == UserId && (EditInterval == 0 || SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Minute, DateCreated, DateTime.Now) < EditInterval)))
            {
                string sAction = "re";
                if (ReplyId == 0)
                {
                    sAction = "te";
                }
                string[] EditParams = { ParamKeys.ViewType + "=post", "action=" + sAction, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, "postid=" + PostId };
                if (UseListActions)
                {
                    sOutput = sOutput.Replace("[ACTIONS:EDIT]", "<li class=\"af-edit\" onclick=\"window.location.href='" + Utilities.NavigateUrl(TabId, "", EditParams) + "';\" title=\"[RESX:Edit]\"><em></em>[RESX:Edit]</li>");
                }
                else
                {
                    sOutput = sOutput.Replace("[ACTIONS:EDIT]", "<a class=\"af-actions af-edit\" href=\"" + Utilities.NavigateUrl(TabId, "", EditParams) + "\" title=\"[RESX:Edit]\"><em></em>[RESX:Edit]</a>");
                }

            }
            else
            {
                sOutput = sOutput.Replace("[ACTIONS:EDIT]", string.Empty);
            }
            if (bLocked == false)
            {
                if (bReply || bReply && AuthorId == UserId)
                {
                    string[] QuoteParams = { ParamKeys.ViewType + "=post", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.QuoteId + "=" + PostId };
                    string[] ReplyParams = { ParamKeys.ViewType + "=post", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ReplyId + "=" + PostId };
                    if (UseListActions)
                    {
                        sOutput = sOutput.Replace("[ACTIONS:QUOTE]", "<li class=\"af-quote\" onclick=\"window.location.href='" + Utilities.NavigateUrl(TabId, "", QuoteParams) + "';\" title=\"[RESX:Quote]\"><em></em>[RESX:Quote]</li>");
                        sOutput = sOutput.Replace("[ACTIONS:REPLY]", "<li class=\"af-reply\" onclick=\"window.location.href='" + Utilities.NavigateUrl(TabId, "", ReplyParams) + "';\" title=\"[RESX:Reply]\"><em></em>[RESX:Reply]</li>");
                    }
                    else
                    {
                        sOutput = sOutput.Replace("[ACTIONS:QUOTE]", "<a class=\"af-actions af-quote\" href=\"" + Utilities.NavigateUrl(TabId, "", QuoteParams) + "\" title=\"[RESX:Quote]\"><em></em>[RESX:Quote]</a>");
                        sOutput = sOutput.Replace("[ACTIONS:REPLY]", "<a class=\"af-actions af-reply\" href=\"" + Utilities.NavigateUrl(TabId, "", ReplyParams) + "\" title=\"[RESX:Reply]\"><em></em>[RESX:Reply]</a>");
                    }



                }
                else
                {
                    sOutput = sOutput.Replace("[ACTIONS:QUOTE]", string.Empty);
                    sOutput = sOutput.Replace("[ACTIONS:REPLY]", string.Empty);
                }
            }
            else
            {
                sOutput = sOutput.Replace("[ACTIONS:QUOTE]", string.Empty);
                sOutput = sOutput.Replace("[ACTIONS:REPLY]", string.Empty);
            }

            if (StatusId <= 0 || (StatusId == 3 && ReplyStatusId == 0))
            {
                sOutput = sOutput.Replace("[ACTIONS:ANSWER]", string.Empty);
            }
            else if (ReplyStatusId == 1)
            {
                //Answered
                if (UseListActions)
                {
                    sOutput = sOutput.Replace("[ACTIONS:ANSWER]", "<li class=\"af-answered\" title=\"[RESX:Status:Answer]\"><em></em>[RESX:Status:Answer]</li>");
                }
                else
                {
                    sOutput = sOutput.Replace("[ACTIONS:ANSWER]", "<span class=\"af-actions af-answered\" title=\"[RESX:Status:Answer]\"><em></em>[RESX:Status:Answer]</span>");
                }

            }
            else
            {
                //Not Answered
                if ((this.UserId == TopicAuthorId && !bLocked) || bModEdit)
                {
                    //Can mark answer
                    string sLink = string.Empty;
                    if (UseListActions)
                    {
                        sLink = "<li class=\"af-markanswer\" onclick=\"amaf_markAnswer(" + TopicId.ToString() + "," + ReplyId.ToString() + ");\" title=\"[RESX:Status:SelectAnswer]\"><em></em>[RESX:Status:SelectAnswer]</li>";
                    }
                    else
                    {
                        sLink = "<a class=\"af-actions af-markanswer\" href=\"#\" onclick=\"amaf_markAnswer(" + TopicId.ToString() + "," + ReplyId.ToString() + "); return false;\" title=\"[RESX:Status:SelectAnswer]\"><em></em>[RESX:Status:SelectAnswer]</a>";
                    }

                    sOutput = sOutput.Replace("[ACTIONS:ANSWER]", sLink);
                }
                else
                {
                    //Display Nothing
                    sOutput = sOutput.Replace("[ACTIONS:ANSWER]", string.Empty);

                }
            }

            if (bModEdit)
            {
                if (DateCreated == DateUpdated || DateUpdated == DateTime.MinValue || DateUpdated == Utilities.NullDate())
                {
                    sOutput = sOutput.Replace("[MODEDITDATE]", string.Empty);
                }
                else
                {
                    sOutput = sOutput.Replace("[MODEDITDATE]", Utilities.GetDate(DateUpdated, ModuleId, TimeZoneOffset));
                }
            }
            else
            {
                sOutput = sOutput.Replace("[MODEDITDATE]", string.Empty);
            }
            if ((sOutput.IndexOf("[POLLRESULTS]", 0) + 1) > 0)
            {
                if (TopicType == 1)
                {
                    Polls Polls = new Polls();
                    sOutput = sOutput.Replace("[POLLRESULTS]", Polls.PollResults(TopicId, ImagePath));
                }
                else
                {
                    sOutput = sOutput.Replace("[POLLRESULTS]", string.Empty);
                }
            }

            if (DateUpdated == DateCreated || DateUpdated == DateTime.MinValue || DateUpdated == Utilities.NullDate())
            {
                sOutput = sOutput.Replace("[EDITDATE]", string.Empty);
            }
            else
            {
                sOutput = sOutput.Replace("[EDITDATE]", Utilities.GetDate(DateUpdated, ModuleId, TimeZoneOffset));
            }


            string[] AlertParams = { ParamKeys.ViewType + "=modreport", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ReplyId + "=" + PostId };
            if (Request.IsAuthenticated)
            {
                if (UseListActions)
                {
                    sOutput = sOutput.Replace("[ACTIONS:ALERT]", "<li class=\"af-alert\" onclick=\"window.location.href='" + Utilities.NavigateUrl(TabId, "", AlertParams) + "';\" title=\"[RESX:Alert]\"><em></em>[RESX:Alert]</li>");
                }
                else
                {
                    sOutput = sOutput.Replace("[ACTIONS:ALERT]", "<a class=\"af-actions af-alert\" href=\"" + Utilities.NavigateUrl(TabId, "", AlertParams) + "\" title=\"[RESX:Alert]\"><em></em>[RESX:Alert]</a>");
                }

            }
            else
            {
                sOutput = sOutput.Replace("[ACTIONS:ALERT]", string.Empty);
            }
            if (string.IsNullOrEmpty(Body))
            {
                Body = " <br />";
            }
            string sBody = Utilities.ManageImagePath(Body);



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
                CodeParser objCode = new CodeParser();
                sBody = CodeParser.ParseCode(Utilities.HTMLDecode(sBody));
            }
            sBody = Utilities.StripExecCode(sBody);
            if (MainSettings.AutoLinkEnabled)
            {
                sBody = Utilities.AutoLinks(sBody, Request.Url.Host);
            }

            if (sBody.Contains("<%@"))
            {
                sBody = sBody.Replace("<%@ ", "&lt;&#37;@ ");
            }

            //sBody = Replace(sBody, " #", " &#35;")
            //sBody = Replace(sBody, "-#", "-&#35;")
            //sBody = Replace(sBody, "<%", "&lt;&#37;")
            //sBody = Replace(sBody, "%>", "&#37;&gt;")
            if (Body.ToLowerInvariant().Contains("runat"))
            {
                sBody = Regex.Replace(sBody, "runat", "&#114;&#117;nat", RegexOptions.IgnoreCase);
            }
            //Dim sysEx As New System.Web.RegularExpressions.RunatServerRegex
            //sBody = sysEx.Replace(sBody, "&#114;&#117;nat")
            //'sBody = Replace(sBody, "runat", "&#114;&#117;nat")
            //sBody = Replace(sBody, "<!--", "&lt;&#33;&#45;&#45;")
            //sBody = Replace(sBody, "-->", "&#45;&#45;&gt;")

            sOutput = sOutput.Replace("[BODY]", sBody);
            sOutput = sOutput.Replace("[SUBJECT]", Subject);
            string sAttach = string.Empty;
            if (dtAttach.Rows.Count > 0)
            {
                sAttach = GetAttachments(ContentId, bAttach, PortalId, ModuleId);
            }
            sOutput = sOutput.Replace("[ATTACHMENTS]", sAttach);

            //&#91;IMAGE:38&#93;
            if (sOutput.Contains("&#91;IMAGE:"))
            {
                string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/";
                string pattern = "(&#91;IMAGE:(.+?)&#93;)";
                Regex regExp = new Regex(pattern);
                MatchCollection matches = null;
                matches = regExp.Matches(sOutput);
                foreach (Match match in matches)
                {
                    string sImage = "";
                    sImage = "<img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + match.Groups[2].Value + "\" border=\"0\" class=\"afimg\" />";
                    sOutput = sOutput.Replace(match.Value, sImage);
                }
            }
            if (sOutput.Contains("&#91;THUMBNAIL:"))
            {
                string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/";
                string pattern = "(&#91;THUMBNAIL:(.+?)&#93;)";
                Regex regExp = new Regex(pattern);
                MatchCollection matches = null;
                matches = regExp.Matches(sOutput);
                foreach (Match match in matches)
                {
                    string sImage = "";
                    string thumbId = match.Groups[2].Value.Split(':')[0];
                    string parentId = match.Groups[2].Value.Split(':')[1];
                    sImage = "<a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + parentId + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + thumbId + "\" border=\"0\" class=\"afimg\" /></a>";
                    sOutput = sOutput.Replace(match.Value, sImage);
                }
            }


            return sOutput;
        }
        private string GetAttachments(int ContentId, bool AllowAttach, int PortalID, int ModuleID)
        {
            AllowAttach = true;
            string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/";
            if (Request.IsSecureConnection)
            {
                strHost = strHost.Replace("http://", "https://");
            }
            //TODO: Add option for folder storage
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
            if (AllowAttach == true)
            {
                string vpath = null;
                vpath = PortalSettings.HomeDirectory + "activeforums_Attach/";
                string fpath = null;
                fpath = Server.MapPath(PortalSettings.HomeDirectory + "activeforums_Attach/");
                dtAttach.DefaultView.RowFilter = "ContentId = " + ContentId;
                sb.Append("<br />");
                foreach (DataRow dr in dtAttach.DefaultView.ToTable().Rows)
                {
                    string tmpPath = null;
                    int attachId = Convert.ToInt32(dr["AttachId"]);
                    string Filename = dr["Filename"].ToString();
                    string contentType = dr["ContentType"].ToString();
                    string fileURL = string.Empty;
                    if (!(string.IsNullOrEmpty(dr["FileURL"].ToString())))
                    {
                        fileURL = Page.ResolveUrl(dr["FileURL"].ToString());
                    }
                    if (dr.IsNull("FileData") && string.IsNullOrEmpty(fileURL))
                    {
                        tmpPath = fpath + dr["Filename"].ToString();
                        if (!(System.IO.File.Exists(tmpPath)))
                        {
                            tmpPath = tmpPath.Replace("activeforums_Attach", "ntforums_attach");
                        }
                        string strExt = System.IO.Path.GetExtension(tmpPath);
                        string sPath = strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalID + "&moduleid=" + ModuleID + "&attachid=" + attachId;
                        //sb.Append("<a href=""" & vpath & Filename & """ target=""_blank"">")
                        sb.Append("<a href=\"" + sPath + "\" target=\"_blank\">");
                        sb.Append("<img src=\"" + strHost + "DesktopModules/ActiveForums/images/attach.gif\" border=\"0\" align=\"absmiddle\" />" + Filename + "</a><br />");
                    }
                    else if (!(string.IsNullOrEmpty(fileURL)))
                    {
                        //If contentType.Contains("jpg") Or contentType.Contains("jpeg") Or contentType.Contains("png") Or contentType.Contains("gif") Or contentType.Contains("bmp") Then
                        //    sb.Append("<img src=""" & fileURL & """ />")
                        //Else
                        sb.Append("<a href=\"" + fileURL + "\" target=\"_blank\">" + Filename + "</a>");
                        //End If

                    }
                    else
                    {
                        string strExt = System.IO.Path.GetExtension(Filename);
                        sb.Append("<span class=\"afattachlink\"><a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalID + "&moduleid=" + ModuleID + "&attachid=" + attachId + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/images/attach.gif\" border=\"0\" align=\"absmiddle\" /><span>" + Filename + "</span></a></span><br />");
                    }
                }
                sb.Append("<br />");

                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        private void BuildPager()
        {
            object obj = null;
            DotNetNuke.Modules.ActiveForums.Controls.PagerNav Pager1 = null;
            obj = this.FindControl("Pager1");
            if (obj != null)
            {
                Pager1 = (DotNetNuke.Modules.ActiveForums.Controls.PagerNav)(this.FindControl("Pager1"));
            }
            obj = null;
            DotNetNuke.Modules.ActiveForums.Controls.PagerNav Pager2 = null;
            obj = this.FindControl("Pager2");
            if (obj != null)
            {
                Pager2 = (DotNetNuke.Modules.ActiveForums.Controls.PagerNav)obj;
            }
            if (Pager1 == null & Pager2 == null)
            {
                return;
            }

            int intPages = 0;
            intPages = Convert.ToInt32(System.Math.Ceiling(RowCount / (double)PageSize));
            Pager1.PageCount = intPages;
            Pager1.CurrentPage = PageId;
            Pager1.TabID = Convert.ToInt32(Request.Params["TabId"]);
            Pager1.ForumID = ForumId;
            Pager1.UseShortUrls = MainSettings.UseShortUrls;
            Pager1.PageText = Utilities.GetSharedResource("[RESX:Page]");
            Pager1.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
            Pager1.View = Views.Topic;
            Pager1.TopicId = TopicId;
            //Pager1.ClientScript = "afPageTopic({0});"
            //If UseAjax Then
            //Pager1.PageMode = PagerNav.Mode.CallBack
            //Else
            Pager1.PageMode = PagerNav.Mode.Links;
            //End If
            if (!(TopicURL.Contains(ForumInfo.PrefixURL)))
            {
                TopicURL = "/" + ForumInfo.PrefixURL + "/" + TopicURL;
            }
            if (MainSettings.URLRewriteEnabled)
            {
                if (!(string.IsNullOrEmpty(TopicURL)))
                {
                    if (!(string.IsNullOrEmpty(MainSettings.PrefixURLBase)))
                    {
                        Pager1.BaseURL = "/" + MainSettings.PrefixURLBase;
                    }
                    if (ForumInfo.ForumGroup.PrefixURL != null && !(string.IsNullOrEmpty(ForumInfo.ForumGroup.PrefixURL)))
                    {
                        Pager1.BaseURL += "/" + ForumInfo.ForumGroup.PrefixURL;
                    }
                    Pager1.BaseURL += TopicURL;
                    Pager1.PageMode = PagerNav.Mode.Links;
                }
            }
            if (Request.Params[ParamKeys.Sort] != null)
            {
                string[] Params = { ParamKeys.Sort + "=" + Request.Params[ParamKeys.Sort] };
                Pager1.Params = Params;
                if (Pager2 != null)
                {
                    Pager2.Params = Params;
                }

            }
            if (Pager2 != null)
            {
                //If UseAjax Then
                //Pager2.PageMode = PagerNav.Mode.CallBack
                // Else
                Pager2.PageMode = Modules.ActiveForums.Controls.PagerNav.Mode.Links; // DotNetNuke.Modules.ActiveForums.Controls.Pager.Mode.CallBack
                // End If
                Pager2.PageCount = intPages;
                Pager2.UseShortUrls = MainSettings.UseShortUrls;
                Pager2.CurrentPage = PageId;
                Pager2.TabID = Convert.ToInt32(Request.Params["TabId"]);
                Pager2.ForumID = ForumId;
                Pager2.TopicId = TopicId;
                // Pager2.ClientScript = "afPageTopic({0});"
                Pager2.PageText = Utilities.GetSharedResource("[RESX:Page]");
                Pager2.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
                Pager2.View = Views.Topic;
                Pager2.PageMode = PagerNav.Mode.Links;
                if (MainSettings.URLRewriteEnabled)
                {
                    if (!(string.IsNullOrEmpty(TopicURL)))
                    {
                        if (!(string.IsNullOrEmpty(MainSettings.PrefixURLBase)))
                        {
                            Pager2.BaseURL = "/" + MainSettings.PrefixURLBase;
                        }
                        if (ForumInfo.ForumGroup.PrefixURL != null && !(string.IsNullOrEmpty(ForumInfo.ForumGroup.PrefixURL)))
                        {
                            Pager2.BaseURL += "/" + ForumInfo.ForumGroup.PrefixURL;
                        }
                        Pager2.BaseURL += TopicURL;

                    }
                }

            }

        }
        #endregion
    }
}
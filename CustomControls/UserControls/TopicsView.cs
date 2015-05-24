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
using DotNetNuke.Modules.ActiveForums.Constants;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:TopicsView runat=server></{0}:TopicsView>")]
    public class TopicsView : ForumBase
    {
        #region Private Members
        private string _metaTemplate = "[META][TITLE][PORTALNAME] - [PAGENAME] - [GROUPNAME] - [FORUMNAME][/TITLE][DESCRIPTION][BODY][/DESCRIPTION][KEYWORDS][VALUE][/KEYWORDS][/META]";
        private string _metaTitle = string.Empty;
        private string _metaDescription = string.Empty;
        private string _metaKeywords = string.Empty;
        private string ForumName;
        private string GroupName;
        //Private ForumGroupId As Integer = 0
        //Private TopicsTemplateId As Integer
        private DataRow drForum;
        private DataRow drSecurity;
        private DataTable dtTopics;
        private DataTable dtAnnounce;
        private DataTable dtSubForums;
        private bool bView = false;
        private bool bRead = false;
        //private bool bReply = false;
        //private bool bCreate = false;
        private bool bPoll = false;
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
        private bool bModLock = false;
        private bool bModPin = false;
        private bool bAllowRSS = false;
        private int RowIndex = 1;
        private int PageSize = 20;
        private int TopicRowCount = 0;
        private string MyTheme = "_default";
        private string MyThemePath = string.Empty;
        private string LastReplySubjectReplaceTag = string.Empty;
        private bool IsSubscribedForum = false;
        private string sGroupURL = string.Empty;
        private string sForumURL = string.Empty;
        #endregion
        #region Public Properties
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
        private string _ForumUrl = string.Empty;
        public string ForumUrl
        {
            get
            {
                return _ForumUrl;
            }
            set
            {
                _ForumUrl = value;
            }
        }
        #endregion
        #region Controls
        protected af_quickjump ctlForumJump = new af_quickjump();
        //Protected WithEvents cbActions As New DotNetNuke.Modules.ActiveForums.Controls.Callback
        //protected Modal ctlModal = new Modal();
        protected ForumView ctlForumSubs = new ForumView();

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //ctlModal.Callback += ctlModal_Callback;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (ForumId < 1)
                {
                    Response.Redirect(NavigateUrl(TabId));
                }
                if (ForumInfo == null)
                {
                    Response.Redirect(NavigateUrl(TabId));
                }
                if (ForumInfo.Active == false)
                {
                    Response.Redirect(NavigateUrl(TabId));
                }
                this.AppRelativeVirtualPath = "~/";
                MyTheme = MainSettings.Theme;
                MyThemePath = Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MyTheme);
                int defaultTemplateId = ForumInfo.TopicsTemplateId;
                if (DefaultTopicsViewTemplateId >= 0)
                {
                    defaultTemplateId = DefaultTopicsViewTemplateId;
                }
                string TopicsTemplate = string.Empty;
                if (UseTemplatePath && !(TemplatePath == string.Empty))
                {
                    TopicsTemplate = Utilities.GetFileContent(TemplatePath + "TopicsView.htm");
                    TopicsTemplate = Utilities.ParseSpacer(TopicsTemplate);
                }
                else
                {
                    TopicsTemplate = DataCache.GetCachedTemplate(MainSettings.TemplateCache, ModuleId, "TopicsView", defaultTemplateId);
                }
                bool loadComplete = false;
                if (TopicsTemplate.Contains("[NOTOOLBAR]"))
                {
                    if (HttpContext.Current.Items.Contains("ShowToolbar"))
                    {
                        HttpContext.Current.Items["ShowToolbar"] = false;
                    }
                    else
                    {
                        HttpContext.Current.Items.Add("ShowToolbar", false);
                    }
                    TopicsTemplate = TopicsTemplate.Replace("[NOTOOLBAR]", string.Empty);
                }
                TopicsTemplate = TopicsTemplate.Replace("[PORTALID]", PortalId.ToString());
                TopicsTemplate = TopicsTemplate.Replace("[MODULEID]", ForumModuleId.ToString());
                TopicsTemplate = TopicsTemplate.Replace("[TABID]", ForumTabId.ToString());
                TopicsTemplate = TopicsTemplate.Replace("[AF:CONTROL:FORUMID]", ForumId.ToString());
                TopicsTemplate = TopicsTemplate.Replace("[AF:CONTROL:FORUMGROUPID]", ForumGroupId.ToString());
                TopicsTemplate = TopicsTemplate.Replace("[AF:CONTROL:PARENTFORUMID]", ParentForumId.ToString());

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
                string sort = SortColumns.ReplyCreated;
                if (TopicsTemplate.Contains("[AF:SORT:TOPICCREATED]"))
                {
                    sort = SortColumns.TopicCreated;
                    TopicsTemplate = TopicsTemplate.Replace("[AF:SORT:TOPICCREATED]", string.Empty);
                }
                TopicsTemplate = CheckControls(TopicsTemplate);

                TopicsTemplate = TopicsTemplate.Replace("[AF:SORT:REPLYCREATED]", string.Empty);
                if (TopicsTemplate.Contains("[TOPICS]"))
                {
                    DataSet ds = DataProvider.Instance().UI_TopicsView(PortalId, ModuleId, ForumId, UserId, RowIndex, PageSize, UserInfo.IsSuperUser, sort);
                    if (ds.Tables.Count > 0)
                    {
                        drForum = ds.Tables[0].Rows[0];
                        drSecurity = ds.Tables[1].Rows[0];
                        dtSubForums = ds.Tables[2];
                        dtTopics = ds.Tables[3];
                        if (PageId == 1)
                        {
                            dtAnnounce = ds.Tables[4];
                        }

                        bView = Permissions.HasPerm(drSecurity["CanView"].ToString(), ForumUser.UserRoles);
                        bRead = Permissions.HasPerm(drSecurity["CanRead"].ToString(), ForumUser.UserRoles);
                        //bCreate = Permissions.HasPerm(drSecurity["CanCreate"].ToString(), ForumUser.UserRoles);
                        bEdit = Permissions.HasPerm(drSecurity["CanEdit"].ToString(), ForumUser.UserRoles);
                        bDelete = Permissions.HasPerm(drSecurity["CanDelete"].ToString(), ForumUser.UserRoles);
                        //bReply = Permissions.HasPerm(drSecurity["CanReply"].ToString(), ForumUser.UserRoles);
                        bPoll = Permissions.HasPerm(drSecurity["CanPoll"].ToString(), ForumUser.UserRoles);

                        bSubscribe = Permissions.HasPerm(drSecurity["CanSubscribe"].ToString(), ForumUser.UserRoles);
                        bModMove = Permissions.HasPerm(drSecurity["CanModMove"].ToString(), ForumUser.UserRoles);
                        bModSplit = Permissions.HasPerm(drSecurity["CanModSplit"].ToString(), ForumUser.UserRoles);
                        bModDelete = Permissions.HasPerm(drSecurity["CanModDelete"].ToString(), ForumUser.UserRoles);
                        bModApprove = Permissions.HasPerm(drSecurity["CanModApprove"].ToString(), ForumUser.UserRoles);
                        bModEdit = Permissions.HasPerm(drSecurity["CanModEdit"].ToString(), ForumUser.UserRoles);
                        bModPin = Permissions.HasPerm(drSecurity["CanModPin"].ToString(), ForumUser.UserRoles);
                        bModLock = Permissions.HasPerm(drSecurity["CanModLock"].ToString(), ForumUser.UserRoles);
                        bModApprove = Permissions.HasPerm(drSecurity["CanModApprove"].ToString(), ForumUser.UserRoles);

                        ControlUtils ctlUtils = new ControlUtils();
                        sGroupURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, string.Empty, ForumInfo.ForumGroupId, -1, -1, -1, string.Empty, 1, -1, SocialGroupId);
                        sForumURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, -1, -1, string.Empty, 1, -1, SocialGroupId);
                        if (bView)
                        {
                            ForumName = drForum["ForumName"].ToString();
                            GroupName = drForum["GroupName"].ToString();
                            ForumGroupId = Convert.ToInt32(drForum["ForumGroupId"]);
                            //TopicsTemplateId = CInt(drForum("TopicsTemplateId"))
                            try
                            {
                                bAllowRSS = Convert.ToBoolean(drForum["AllowRSS"]);
                            }
                            catch
                            {
                                bAllowRSS = false;
                            }

                            if (bRead == false)
                            {
                                bAllowRSS = false;
                            }
                            TopicRowCount = Convert.ToInt32(drForum["TopicRowCount"]);
                            if (UserId > 0)
                            {
                                IsSubscribedForum = Convert.ToBoolean(((Convert.ToInt32(drForum["IsSubscribedForum"]) > 0) ? true : false));
                            }
                            if (MainSettings.UseSkinBreadCrumb)
                            {
                                Environment.UpdateBreadCrumb(Page.Controls, "<a href=\"" + sGroupURL + "\">" + GroupName + "</a>");
                                TopicsTemplate = TopicsTemplate.Replace("<div class=\"afcrumb\">[FORUMMAINLINK] > [FORUMGROUPLINK]</div>", string.Empty);
                            }
                            if (TopicsTemplate.Contains("[META]"))
                            {
                                MetaTemplate = TemplateUtils.GetTemplateSection(TopicsTemplate, "[META]", "[/META]");
                                TopicsTemplate = TemplateUtils.ReplaceSubSection(TopicsTemplate, string.Empty, "[META]", "[/META]");
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
                                MetaTemplate = MetaTemplate.Replace("[TAGS]", string.Empty);
                                if (MetaTemplate.Contains("[TOPICSUBJECT:"))
                                {
                                    string pattern = "(\\[TOPICSUBJECT:(.+?)\\])";
                                    Regex regExp = new Regex(pattern);
                                    MatchCollection matches = null;
                                    matches = regExp.Matches(MetaTemplate);
                                    foreach (Match m in matches)
                                    {
                                        MetaTemplate = MetaTemplate.Replace(m.Value, string.Empty);
                                    }
                                }
                                MetaTemplate = MetaTemplate.Replace("[TOPICSUBJECT]", string.Empty);
                                if (MetaTemplate.Contains("[BODY:"))
                                {
                                    string pattern = "(\\[BODY:(.+?)\\])";
                                    Regex regExp = new Regex(pattern);
                                    MatchCollection matches = null;
                                    matches = regExp.Matches(MetaTemplate);
                                    foreach (Match m in matches)
                                    {
                                        int iLen = Convert.ToInt32(m.Groups[2].Value);
                                        if (ForumInfo.ForumDesc.Length > iLen)
                                        {
                                            MetaTemplate = MetaTemplate.Replace(m.Value, ForumInfo.ForumDesc.Substring(0, iLen) + "...");
                                        }
                                        else
                                        {
                                            MetaTemplate = MetaTemplate.Replace(m.Value, ForumInfo.ForumDesc);
                                        }
                                    }
                                }
                                MetaTemplate = MetaTemplate.Replace("[BODY]", Utilities.StripHTMLTag(ForumInfo.ForumDesc));

                                MetaTitle = TemplateUtils.GetTemplateSection(MetaTemplate, "[TITLE]", "[/TITLE]").Replace("[TITLE]", string.Empty).Replace("[/TITLE]", string.Empty);
                                MetaTitle = MetaTitle.TruncateAtWord(SEOConstants.MaxMetaTitleLength);
                                MetaDescription = TemplateUtils.GetTemplateSection(MetaTemplate, "[DESCRIPTION]", "[/DESCRIPTION]").Replace("[DESCRIPTION]", string.Empty).Replace("[/DESCRIPTION]", string.Empty);
                                MetaDescription = MetaDescription.TruncateAtWord(SEOConstants.MaxMetaDescriptionLength);
                                MetaKeywords = TemplateUtils.GetTemplateSection(MetaTemplate, "[KEYWORDS]", "[/KEYWORDS]").Replace("[KEYWORDS]", string.Empty).Replace("[/KEYWORDS]", string.Empty);
                            }
                            BindTopics(TopicsTemplate);
                        }
                        else
                        {
                            Response.Redirect(NavigateUrl(TabId), true);
                        }

                        try
                        {
                            DotNetNuke.Framework.CDefault tempVar = this.BasePage;
                            Environment.UpdateMeta(ref tempVar, MetaTitle, MetaDescription, MetaKeywords);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        Response.Redirect(NavigateUrl(TabId), true);
                    }
                    if (Session["modal_View"] != null)
                    {
                        //LoadModal(Session["modal_View"].ToString(), Session["modal_options"].ToString());
                    }
                }
                else
                {
                    ForumController fc = new ForumController();
                    string fs = fc.GetForumsForUser(ForumUser.UserRoles, PortalId, ModuleId, "CanEdit");
                    if (!(string.IsNullOrEmpty(fs)))
                    {
                        bModEdit = true;
                    }
                    TopicsTemplate = ParseControls(TopicsTemplate);
                    TopicsTemplate = Utilities.LocalizeControl(TopicsTemplate);
                    this.Controls.Add(this.ParseControl(TopicsTemplate));
                    LinkControls(this.Controls);
                }




            }
            catch (Exception exc)
            {
                RenderMessage("[RESX:Error:LoadingTopics]", exc.Message, exc);
            }
        }

        private void BindTopics(string TopicsTemplate)
        {
            string sOutput = TopicsTemplate;
            string subTemplate = string.Empty;

            //Subforum Template

            if (sOutput.Contains("[SUBFORUMS]"))
            {
                if (dtSubForums.Rows.Count > 0)
                {
                    subTemplate = TemplateUtils.GetTemplateSection(sOutput, "[SUBFORUMS]", "[/SUBFORUMS]");
                }
                sOutput = TemplateUtils.ReplaceSubSection(sOutput, "<asp:placeholder id=\"plhSubForums\" runat=\"server\" />", "[SUBFORUMS]", "[/SUBFORUMS]");
            }

            //Parse Common Controls
            sOutput = ParseControls(sOutput);
            //Parse Topics
            sOutput = ParseTopics(sOutput, dtTopics, "TOPICS");
            //Parse Announce
            string sAnnounce = TemplateUtils.GetTemplateSection(sOutput, "[ANNOUNCEMENTS]", "[/ANNOUNCEMENTS]");
            if (dtAnnounce != null)
            {
                if (dtAnnounce.Rows.Count > 0)
                {
                    sAnnounce = ParseTopics(sAnnounce, dtAnnounce, "ANNOUNCEMENT");
                }
                else
                {
                    sAnnounce = string.Empty;
                }
            }
            else
            {
                sAnnounce = string.Empty;
            }
            sOutput = TemplateUtils.ReplaceSubSection(sOutput, sAnnounce, "[ANNOUNCEMENTS]", "[/ANNOUNCEMENTS]");

            sOutput = Utilities.LocalizeControl(sOutput);
            this.Controls.Add(this.ParseControl(sOutput));
            BuildPager();

            PlaceHolder plh = (PlaceHolder)(this.FindControl("plhQuickJump"));
            if (plh != null)
            {
                ctlForumJump = new af_quickjump();
                ctlForumJump.MOID = ModuleId;
                ctlForumJump.dtForums = null;
                ctlForumJump.ModuleConfiguration = this.ModuleConfiguration;
                ctlForumJump.ForumId = ForumId;
                ctlForumJump.ModuleId = ModuleId;
                if (ForumId > 0)
                {
                    ctlForumJump.ForumInfo = ForumInfo;
                }
                plh.Controls.Add(ctlForumJump);
            }

            plh = (PlaceHolder)(this.FindControl("plhSubForums"));
            if (plh != null)
            {
                ctlForumSubs = (ForumView)(LoadControl(typeof(ForumView), null));
                ctlForumSubs.ModuleConfiguration = this.ModuleConfiguration;
                ctlForumSubs.ForumId = ForumId;
                ctlForumSubs.ForumTable = dtSubForums;
                ctlForumSubs.ForumTabId = ForumTabId;
                ctlForumSubs.ForumModuleId = ForumModuleId;
                ctlForumSubs.SubsOnly = true;
                ctlForumSubs.DisplayTemplate = subTemplate;
                if (ForumId > 0)
                {
                    ctlForumSubs.ForumInfo = ForumInfo;
                }
                plh.Controls.Add(ctlForumSubs);
            }
            //Me.Controls.Add(cbActions)
            //this.Controls.Add(ctlModal);
            // LoadCallBackScripts()
        }
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                if ((ctrl) is ForumBase)
                {
                    ((ForumBase)ctrl).ModuleConfiguration = this.ModuleConfiguration;

                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }
        private string ParseControls(string Template)
        {
            string MyTheme = MainSettings.Theme;
            string sOutput = Template;
            sOutput = "<%@ Register TagPrefix=\"ac\" Namespace=\"DotNetNuke.Modules.ActiveForums.Controls\" Assembly=\"DotNetNuke.Modules.ActiveForums\" %>" + sOutput;

            //Forum Drop Downlist
            sOutput = sOutput.Replace("[JUMPTO]", "<asp:placeholder id=\"plhQuickJump\" runat=\"server\" />");
            //Tag Cloud
            sOutput = sOutput.Replace("[AF:CONTROLS:TAGCLOUD]", "<ac:tagcloud instanceid=\"" + ModuleId + "\" siteid=\"" + PortalId + "\" tabid=\"" + TabId + "\" runat=\"server\" />");
            //Forum Subscription Control
            if (bSubscribe)
            {
                Controls.ToggleSubscribe subControl = new Controls.ToggleSubscribe(0, ForumId, -1);
                subControl.Checked = IsSubscribedForum;
                subControl.Text = "[RESX:ForumSubscribe:" + IsSubscribedForum.ToString().ToUpper() + "]";
                sOutput = sOutput.Replace("[FORUMSUBSCRIBE]", subControl.Render());
            }
            else
            {
                sOutput = sOutput.Replace("[FORUMSUBSCRIBE]", string.Empty);
            }
            if (Request.IsAuthenticated)
            {
                sOutput = sOutput.Replace("[MARKFORUMREAD]", "<am:MarkForumRead EnableViewState=\"False\" id=\"amMarkForumRead\" MID=\"" + ForumModuleId + "\" runat=\"server\" />");
            }
            else
            {
                sOutput = sOutput.Replace("[MARKFORUMREAD]", string.Empty);
            }

            if (CanCreate)
            {
                string[] Params = { };
                if (SocialGroupId <= 0)
                {
                    Params = new string[] { ParamKeys.ViewType + "=post", ParamKeys.ForumId + "=" + ForumId };
                }
                else
                {
                    Params = new string[] { ParamKeys.ViewType + "=post", ParamKeys.ForumId + "=" + ForumId, "GroupId=" + SocialGroupId, };
                }
                sOutput = sOutput.Replace("[ADDTOPIC]", "<a href=\"" + NavigateUrl(TabId, "", Params) + "\" class=\"dnnPrimaryAction\">[RESX:AddTopic]</a>");
            }
            else
            {
                sOutput = sOutput.Replace("[ADDTOPIC]", "<div class=\"amnormal\">[RESX:NotAuthorizedTopic]</div>");
            }
            sOutput = sOutput.Replace("[ADDPOLL]", string.Empty);
            string Url = null;
            if (bAllowRSS)
            {

                Url = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/DesktopModules/ActiveForums/feeds.aspx?portalid=" + PortalId + "&forumid=" + ForumId + "&tabid=" + TabId + "&moduleid=" + ForumModuleId;
                if (Request.QueryString["asg"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.QueryString["asg"]))
                    {
                        Url += "&asg=" + Request.QueryString["asg"];
                    }
                }
                if (SocialGroupId > 0)
                {
                    Url += "&GroupId=" + SocialGroupId;
                }
                sOutput = sOutput.Replace("[RSSLINK]", "<a href=\"" + Url + "\"><img src=\"" + MyThemePath + "/images/rss.png\" border=\"0\" alt=\"[RESX:RSS]\" /></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[RSSLINK]", string.Empty);
            }
            if (Request.IsAuthenticated)
            {
                Url = NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=sendto", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId });
                sOutput = sOutput.Replace("[AF:CONTROL:EMAIL]", "<a href=\"" + Url + "\" rel=\"nofollow\"><img src=\"" + MyThemePath + "/images/email16.png\" border=\"0\" alt=\"[RESX:EmailThis]\" /></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[AF:CONTROL:EMAIL]", string.Empty);
            }
            if (sOutput.Contains("[AF:CONTROL:ADDTHIS"))
            {
                string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request));
                sOutput = TemplateUtils.ParseSpecial(sOutput, SpecialTokenTypes.AddThis, strHost + Request.RawUrl, ForumName, bRead, MainSettings.AddThisAccount);
            }
            sOutput = sOutput.Replace("[MINISEARCH]", "<am:MiniSearch  EnableViewState=\"False\" id=\"amMiniSearch\" MID=\"" + ModuleId + "\" FID=\"" + ForumId + "\" runat=\"server\" />");
            sOutput = sOutput.Replace("[PAGER1]", "<am:pagernav id=\"Pager1\"  EnableViewState=\"False\" runat=\"server\" />");
            sOutput = sOutput.Replace("[PAGER2]", "<am:pagernav id=\"Pager2\" runat=\"server\" EnableViewState=\"False\" />");
            if (sOutput.Contains("[PARENTFORUMLINK]"))
            {
                if (ForumInfo.ParentForumId > 0)
                {
                    sOutput = sOutput.Replace("[PARENTFORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId) + "\">" + ForumInfo.ParentForumName + "</a>");
                }
                else if (ForumInfo.ForumGroupId > 0)
                {
                    sOutput = sOutput.Replace("[PARENTFORUMLINK]", "<a href=\"" + Utilities.NavigateUrl(TabId) + "\">" + ForumInfo.GroupName + "</a>");
                }
            }
            // If String.IsNullOrEmpty(ForumInfo.ParentForumName) Then
            sOutput = sOutput.Replace("[PARENTFORUMNAME]", ForumInfo.ParentForumName);
            //End If

            sOutput = sOutput.Replace("[FORUMMAINLINK]", "<a href=\"" + NavigateUrl(TabId) + "\">[RESX:ForumMain]</a>");
            sOutput = sOutput.Replace("[FORUMGROUPLINK]", "<a href=\"" + sGroupURL + "\">" + GroupName + "</a>");

            sOutput = sOutput.Replace("[FORUMNAME]", ForumName);
            sOutput = sOutput.Replace("[FORUMID]", ForumId.ToString());
            sOutput = sOutput.Replace("[GROUPNAME]", GroupName);
            if (bModDelete)
            {
                sOutput = sOutput.Replace("[ACTIONS:DELETE]", "<a href=\"javascript:void(0)\" onclick=\"amaf_modDel([TOPICID]);\" style=\"vertical-align:middle;\" title=\"[RESX:DeleteTopic]\" /><i class=\"fa fa-trash-o fa-fw fa-blue\"></i></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[ACTIONS:DELETE]", string.Empty);
            }
            if (bModEdit)
            {
                string[] EditParams = { ParamKeys.ViewType + "=post", "action=te", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=0-0" };
                sOutput = sOutput.Replace("[ACTIONS:EDIT]", "<a title=\"[RESX:EditTopic]\" href=\"" + NavigateUrl(TabId, "", EditParams) + "\"><i class=\"fa fa-pencil-square-o fa-fw fa-blue\"></i></a>");
                sOutput = sOutput.Replace("0-0", "[TOPICID]");
                sOutput = sOutput.Replace("[AF:QUICKEDITLINK]", "<a href=\"javascript:void(0)\" title=\"[RESX:TopicQuickEdit]\" onclick=\"amaf_quickEdit([TOPICID]);\"><i class=\"fa fa-cog fa-fw fa-blue\"></i></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[AF:QUICKEDITLINK]", string.Empty);
                sOutput = sOutput.Replace("[ACTIONS:EDIT]", string.Empty);
            }
            if (bModMove)
            {
                sOutput = sOutput.Replace("[ACTIONS:MOVE]", "<a href=\"javascript:void(0)\" onclick=\"javascript:amaf_openMove([TOPICID]);\" title=\"[RESX:MoveTopic]\" style=\"vertical-align:middle;\" /><i class=\"fa fa-exchange fa-rotate-90 fa-blue\"></i></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[ACTIONS:MOVE]", string.Empty);
            }
            if (bModLock)
            {
                sOutput = sOutput.Replace("[ACTIONS:LOCK]", "<a href=\"javascript:void(0)\" onclick=\"javascript:if(confirm('[RESX:Confirm:Lock]')){amaf_modLock([TOPICID]);};\" title=\"[RESX:LockTopic]\" style=\"vertical-align:middle;\"><i class=\"fa fa-lock fa-fw fa-blue\"></i></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[ACTIONS:LOCK]", string.Empty);
            }
            if (bModPin)
            {
                sOutput = sOutput.Replace("[ACTIONS:PIN]", "<a href=\"javascript:void(0)\" onclick=\"javascript:if(confirm('[RESX:Confirm:Pin]')){amaf_modPin([TOPICID]);};\" title=\"[RESX:Pin]\" style=\"vertical-align:middle;\"><i class=\"fa fa-thumb-tack fa-fw fa-blue\"></i></a>");
            }
            else
            {
                sOutput = sOutput.Replace("[ACTIONS:PIN]", string.Empty);
            }
            sOutput = sOutput.Replace("[FORUMLINK]", "<a href=\"" + sForumURL + "\">" + ForumName + "</a>");


            return sOutput;
        }
        private string ParseTopics(string Template, DataTable Topics, string Section)
        {
            string sOutput = Template;
            sOutput = TemplateUtils.GetTemplateSection(Template, "[" + Section + "]", "[/" + Section + "]");
            string sTopics = string.Empty;
            string MemberListMode = MainSettings.MemberListMode;
            var ProfileVisibility = MainSettings.ProfileVisibility;
            string UserNameDisplay = MainSettings.UserNameDisplay;
            bool DisableUserProfiles = false;
            string sLastReply = TemplateUtils.GetTemplateSection(sOutput, "[LASTPOST]", "[/LASTPOST]");
            int iLength = 0;
            if (sLastReply.Contains("[LASTPOSTSUBJECT:"))
            {
                int inStart = (sLastReply.IndexOf("[LASTPOSTSUBJECT:", 0) + 1) + 17;
                int inEnd = (sLastReply.IndexOf("]", inStart - 1) + 1);
                string sLength = sLastReply.Substring(inStart - 1, inEnd - inStart);
                iLength = Convert.ToInt32(sLength);
            }
            int rowcount = 0;
            LastReplySubjectReplaceTag = "[LASTPOSTSUBJECT:" + iLength.ToString() + "]";
            if (Topics == null)
            {
                sOutput = TemplateUtils.ReplaceSubSection(Template, string.Empty, "[" + Section + "]", "[/" + Section + "]");
                return sOutput;
            }
            if (Topics.Rows.Count > 0)
            {
                foreach (DataRow drTopic in Topics.Rows)
                {
                    string sTopicsTemplate = sOutput;
                    int TopicId = Convert.ToInt32(drTopic["TopicId"]);
                    string Subject = Convert.ToString(drTopic["Subject"]);
                    string Summary = Convert.ToString(drTopic["Summary"]);
                    string Body = Convert.ToString(drTopic["Body"]);
                    //Strip comments

                    int AuthorId = Convert.ToInt32(drTopic["AuthorId"]);
                    string AuthorName = Convert.ToString(drTopic["AuthorName"]).ToString().Replace("&amp;#", "&#");
                    string AuthorFirstName = Convert.ToString(drTopic["AuthorFirstName"]).ToString().Replace("&amp;#", "&#");
                    string AuthorLastName = Convert.ToString(drTopic["AuthorLastName"]).ToString().Replace("&amp;#", "&#");
                    string AuthorUserName = Convert.ToString(drTopic["AuthorUserName"]);
                    if (AuthorUserName == string.Empty)
                    {
                        AuthorUserName = AuthorName;
                    }
                    string AuthorDisplayName = Convert.ToString(drTopic["AuthorDisplayName"]).ToString().Replace("&amp;#", "&#");
                    int ReplyCount = Convert.ToInt32(drTopic["ReplyCount"]);
                    int ViewCount = Convert.ToInt32(drTopic["ViewCount"]);
                    DateTime DateCreated = Convert.ToDateTime(drTopic["DateCreated"]);
                    int StatusId = Convert.ToInt32(drTopic["StatusId"]);
                    //LastReply info
                    int LastReplyId = Convert.ToInt32(drTopic["LastReplyId"]);
                    string LastReplySubject = Convert.ToString(drTopic["LastReplySubject"]);
                    if (LastReplySubject == "")
                    {
                        LastReplySubject = "RE: " + Subject;
                    }
                    string LastReplySummary = Convert.ToString(drTopic["LastReplySummary"]);
                    if (LastReplySummary == "")
                    {
                        LastReplySummary = Summary;
                    }
                    int LastReplyAuthorId = Convert.ToInt32(drTopic["LastReplyAuthorId"]);
                    string LastReplyAuthorName = Convert.ToString(drTopic["LastReplyAuthorName"]).ToString().Replace("&amp;#", "&#");
                    string LastReplyFirstName = Convert.ToString(drTopic["LastReplyFirstName"]).ToString().Replace("&amp;#", "&#");
                    string LastReplyLastName = Convert.ToString(drTopic["LastReplyLastName"]).ToString().Replace("&amp;#", "&#");
                    string LastReplyUserName = Convert.ToString(drTopic["LastReplyUserName"]);
                    string LastReplyDisplayName = Convert.ToString(drTopic["LastReplyDisplayName"]).ToString().Replace("&amp;#", "&#");
                    DateTime LastReplyDate = Convert.ToDateTime(drTopic["LastReplyDate"]);
                    int UserLastTopicRead = Convert.ToInt32(drTopic["UserLastTopicRead"]);
                    int UserLastReplyRead = Convert.ToInt32(drTopic["UserLastReplyRead"]);
                    bool isLocked = Convert.ToBoolean(drTopic["IsLocked"]);
                    bool isPinned = Convert.ToBoolean(drTopic["IsPinned"]);
                    string TopicURL = drTopic["TopicURL"].ToString();
                    string topicData = drTopic["TopicData"].ToString();
                    if (isLocked)
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[RESX:LockTopic]", "[RESX:UnLockTopic]");
                        sTopicsTemplate = sTopicsTemplate.Replace("[RESX:Confirm:Lock]", "[RESX:Confirm:UnLock]");
                        sTopicsTemplate = sTopicsTemplate.Replace("[ICONLOCK]", "&nbsp;&nbsp;<i class=\"fa fa-lock fa-fw fa-red\"></i>");
                    }
                    else
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[ICONLOCK]", "");
                    }

                    if (isPinned)
                    {
                        //sTopicsTemplate = sTopicsTemplate.Replace("[RESX:PinTopic]", "[RESX:UnPinTopic]");
                        sTopicsTemplate = sTopicsTemplate.Replace("[RESX:Confirm:Pin]", "[RESX:Confirm:UnPin]");
                        sTopicsTemplate = sTopicsTemplate.Replace("[ICONPIN]", "&nbsp;&nbsp;<i class=\"fa fa-thumb-tack fa-fw fa-red\"></i>");
                    }
                    else
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[ICONPIN]", "");
                    }

                    if (string.IsNullOrEmpty(topicData))
                    {
                        sTopicsTemplate = TemplateUtils.ReplaceSubSection(sTopicsTemplate, string.Empty, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");
                    }
                    else
                    {
                        string sPropTemplate = TemplateUtils.GetTemplateSection(sTopicsTemplate, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");
                        string sProps = string.Empty;
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(topicData);
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
                                    tmp = tmp.Replace("[AF:PROPERTY:LABEL]", Utilities.GetSharedResource("[RESX:" + pName + "]"));
                                    tmp = tmp.Replace("[AF:PROPERTY:VALUE]", pValue);
                                    sTopicsTemplate = sTopicsTemplate.Replace("[AF:PROPERTY:" + pName + ":LABEL]", Utilities.GetSharedResource("[RESX:" + pName + "]"));
                                    sTopicsTemplate = sTopicsTemplate.Replace("[AF:PROPERTY:" + pName + ":VALUE]", pValue);
                                    string pValueKey = string.Empty;
                                    if (!(string.IsNullOrEmpty(pValue)))
                                    {
                                        pValueKey = Utilities.CleanName(pValue).ToLowerInvariant();
                                    }
                                    sTopicsTemplate = sTopicsTemplate.Replace("[AF:PROPERTY:" + pName + ":VALUEKEY]", pValueKey);
                                    sProps += tmp;
                                }
                            }
                            if (sTopicsTemplate.Contains("[AF:PROPERTY:"))
                            {

                            }
                        }
                        sTopicsTemplate = TemplateUtils.ReplaceSubSection(sTopicsTemplate, sProps, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");

                    }


                    sTopicsTemplate = sTopicsTemplate.Replace("[TOPICID]", TopicId.ToString());
                    sTopicsTemplate = sTopicsTemplate.Replace("[AUTHORID]", AuthorId.ToString());
                    sTopicsTemplate = sTopicsTemplate.Replace("[FORUMID]", ForumId.ToString());
                    sTopicsTemplate = sTopicsTemplate.Replace("[USERID]", UserId.ToString());
                    //sTopicsTemplate = sTopicsTemplate.Replace("[POSTICON]", GetIcon(UserLastTopicRead, UserLastReplyRead, TopicId, LastReplyId, drTopic["TopicIcon"].ToString(), isPinned, isLocked));

                    if (UserLastTopicRead == 0 || (UserLastTopicRead > 0 & UserLastReplyRead < ReplyId))
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[POSTICON]", "<div><i class=\"fa fa-file-o fa-2x fa-red\"></i></div>");
                    }
                    else
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[POSTICON]", "<div><i class=\"fa fa-file-o fa-2x fa-grey\"></i></div>");
                    }

                    if (!(string.IsNullOrEmpty(Summary)))
                    {
                        if (!(Utilities.HasHTML(Summary)))
                        {
                            Summary = Summary.Replace(System.Environment.NewLine, "<br />");
                        }
                    }

                    string sBodyTitle = string.Empty;
                    if (bRead)
                    {
                        sBodyTitle = GetTitle(Body, AuthorId);
                    }
                    sTopicsTemplate = sTopicsTemplate.Replace("[BODYTITLE]", sBodyTitle);
                    sTopicsTemplate = sTopicsTemplate.Replace("[BODY]", GetBody(Body, AuthorId));
                    int BodyLength = -1;
                    string BodyTrim = string.Empty;

                    if (Template.Contains("[BODY:"))
                    {
                        int inStart = (Template.IndexOf("[BODY:", 0) + 1) + 5;
                        int inEnd = (Template.IndexOf("]", inStart - 1) + 1) - 1;
                        string sLength = Template.Substring(inStart, inEnd - inStart);
                        BodyLength = Convert.ToInt32(sLength);
                        BodyTrim = "[BODY:" + BodyLength.ToString() + "]";
                    }
                    string BodyPlain = string.Empty;
                    if (string.IsNullOrEmpty(Summary) && sTopicsTemplate.Contains("[SUMMARY]") && string.IsNullOrEmpty(BodyTrim))
                    {
                        BodyTrim = "[BODY:250]";
                        BodyLength = 250;
                    }
                    if (!(BodyTrim == string.Empty))
                    {
                        BodyPlain = Body.Replace("<br>", System.Environment.NewLine);
                        BodyPlain = BodyPlain.Replace("<br />", System.Environment.NewLine);
                        BodyPlain = Utilities.StripHTMLTag(BodyPlain);
                        if (BodyLength > 0 & BodyPlain.Length > BodyLength)
                        {
                            BodyPlain = BodyPlain.Substring(0, BodyLength);
                        }
                        BodyPlain = BodyPlain.Replace(System.Environment.NewLine, "<br />");
                        sTopicsTemplate = sTopicsTemplate.Replace(BodyTrim, BodyPlain);
                    }
                    if (string.IsNullOrEmpty(Summary))
                    {
                        Summary = BodyPlain;
                        Summary = Summary.Replace("<br />", "  ");
                    }
                    sTopicsTemplate = sTopicsTemplate.Replace("[SUMMARY]", Summary);
                    string sPollImage = "";
                    if (Convert.ToInt32(drTopic["TopicType"]) == 1)
                    {
                        //sPollImage = "<img src=\"" + MyThemePath + "/images/poll.png\" style=\"vertical-align:middle;\" alt=\"[RESX:Poll]\" />";

                        sPollImage = "&nbsp;<i class=\"fa fa-signal fa-fw fa-red\"></i>";
                    }
                    string sTopicURL = string.Empty;
                    ControlUtils ctlUtils = new ControlUtils();
                    sTopicURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumGroupId, ForumId, TopicId, TopicURL, -1, -1, string.Empty, 1, -1, SocialGroupId);

                    var @params = new List<string> { ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + LastReplyId };

                    if (SocialGroupId > 0)
                        @params.Add("GroupId=" + SocialGroupId.ToString());

                    string sLastReplyURL = NavigateUrl(TabId, "", @params.ToArray());

                    if (!(string.IsNullOrEmpty(sTopicURL)))
                    {
                        if (sTopicURL.EndsWith("/"))
                        {
                            sLastReplyURL = sTopicURL + "?" + ParamKeys.ContentJumpId + "=" + LastReplyId;
                        }
                    }
                    string sLastReadURL = string.Empty;
                    string sUserJumpUrl = string.Empty;
                    if (UserLastReplyRead > 0)
                    {
                        @params = new List<string> { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=topic", ParamKeys.FirstNewPost + "=" + UserLastReplyRead };
                        if (SocialGroupId > 0)
                            @params.Add("GroupId=" + SocialGroupId.ToString());

                        sLastReadURL = NavigateUrl(TabId, "", @params.ToArray());

                        if (MainSettings.UseShortUrls)
                        {
                            @params = new List<string> { ParamKeys.TopicId + "=" + TopicId, ParamKeys.FirstNewPost + "=" + UserLastReplyRead };
                            if (SocialGroupId > 0)
                                @params.Add("GroupId=" + SocialGroupId.ToString());

                            sLastReadURL = NavigateUrl(TabId, "", @params.ToArray());

                        }
                        if (sTopicURL.EndsWith("/"))
                        {
                            sLastReadURL = sTopicURL + "?" + ParamKeys.FirstNewPost + "=" + UserLastReplyRead;
                        }
                    }

                    if (UserPrefJumpLastPost && sLastReadURL != string.Empty)
                    {
                        sTopicURL = sLastReadURL;
                        sUserJumpUrl = sLastReadURL;
                    }
                    if (UserId == -1 || LastReplyId == 0)
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[AF:ICONLINK:LASTREAD]", string.Empty);
                        sTopicsTemplate = sTopicsTemplate.Replace("[AF:URL:LASTREAD]", string.Empty);
                    }
                    else
                    {
                        if ((UserLastTopicRead >= TopicId || UserLastTopicRead == 0) & (UserLastReplyRead >= LastReplyId || UserLastReplyRead == 0))
                        {
                            sTopicsTemplate = sTopicsTemplate.Replace("[AF:ICONLINK:LASTREAD]", string.Empty);
                            sTopicsTemplate = sTopicsTemplate.Replace("[AF:URL:LASTREAD]", string.Empty);
                        }
                        else
                        {
                            sTopicsTemplate = sTopicsTemplate.Replace("[AF:ICONLINK:LASTREAD]", "<a href=\"" + sLastReadURL + "\" rel=\"nofollow\"><img src=\"" + MyThemePath + "/images/miniarrow_down.png\" style=\"vertical-align:middle;\" alt=\"[RESX:JumpToLastRead]\" border=\"0\" class=\"afminiarrow\" /></a>");
                            sTopicsTemplate = sTopicsTemplate.Replace("[AF:URL:LASTREAD]", sLastReadURL);
                        }

                    }
                    if (LastReplyId > 0 && bRead)
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[AF:ICONLINK:LASTREPLY]", "<a href=\"" + sLastReplyURL + "\" rel=\"nofollow\"><img src=\"" + MyThemePath + "/images/miniarrow_right.png\" style=\"vertical-align:middle;\" alt=\"[RESX:JumpToLastReply]\" border=\"0\" class=\"afminiarrow\" /></a>");
                        sTopicsTemplate = sTopicsTemplate.Replace("[AF:URL:LASTREPLY]", sLastReplyURL);
                        sTopicsTemplate = sTopicsTemplate.Replace("[AF:UI:MINIPAGER]", GetSubPages(TabId, ModuleId, ReplyCount, ForumId, TopicId));
                    }
                    else
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[AF:ICONLINK:LASTREPLY]", string.Empty);
                        sTopicsTemplate = sTopicsTemplate.Replace("[AF:URL:LASTREPLY]", string.Empty);
                        sTopicsTemplate = sTopicsTemplate.Replace("[AF:UI:MINIPAGER]", string.Empty);
                    }

                    sTopicsTemplate = sTopicsTemplate.Replace("[TOPICURL]", sTopicURL);
                    Subject = Utilities.StripHTMLTag(Subject);
                    Subject = Subject.Replace("&#91;", "[");
                    Subject = Subject.Replace("&#93;", "]");
                    sTopicsTemplate = sTopicsTemplate.Replace("[SUBJECT]", Subject + sPollImage);
                    //sTopicsTemplate = sTopicsTemplate.Replace("[SUBJECTLINK]", sPollImage & GetTopic(ModuleId, TabId, ForumId, TopicId, Subject, sBodyTitle, UserId, AuthorId, ReplyCount, -1, sUserJumpUrl))
                    sTopicsTemplate = sTopicsTemplate.Replace("[SUBJECTLINK]", GetTopic(ModuleId, TabId, ForumId, TopicId, Subject, sBodyTitle, UserId, AuthorId, ReplyCount, -1, sTopicURL) + sPollImage);


                    sTopicsTemplate = sTopicsTemplate.Replace("[STARTEDBY]", UserProfiles.GetDisplayName(ModuleId, true, bModApprove, ForumUser.IsAdmin || ForumUser.IsSuperUser, AuthorId, AuthorUserName, AuthorFirstName, AuthorLastName, AuthorDisplayName).ToString().Replace("&amp;#", "&#"));
                    sTopicsTemplate = sTopicsTemplate.Replace("[DATECREATED]", GetDate(DateCreated));
                    sTopicsTemplate = sTopicsTemplate.Replace("[REPLIES]", ReplyCount.ToString());
                    sTopicsTemplate = sTopicsTemplate.Replace("[VIEWS]", ViewCount.ToString());
                    sTopicsTemplate = sTopicsTemplate.Replace("[ROWCSS]", GetRowCSS(UserLastTopicRead, UserLastReplyRead, TopicId, LastReplyId, rowcount));

                    if (Convert.ToInt32(drTopic["TopicRating"]) == 0)
                    {
                        sTopicsTemplate = sTopicsTemplate.Replace("[POSTRATINGDISPLAY]", string.Empty);
                    }
                    else
                    {
                        string sRatingImage = null;
                        //sRatingImage = "<img src=""" & MyThemePath & "/yellow_star_0" & drTopic("TopicRating").ToString & ".gif"" alt=""" & drTopic("TopicRating").ToString & """ />"
                        //sRatingImage = "<span class=\"af-rater rate" + drTopic["TopicRating"].ToString() + "\">&nbsp;</span>";

                        sRatingImage = "<span class=\"fa-rater fa-rate" + drTopic["TopicRating"].ToString() + "\"><i class=\"fa fa-star1\"></i><i class=\"fa fa-star2\"></i><i class=\"fa fa-star3\"></i><i class=\"fa fa-star4\"></i><i class=\"fa fa-star5\"></i></span>";
                        sTopicsTemplate = sTopicsTemplate.Replace("[POSTRATINGDISPLAY]", sRatingImage);
                    }

                    if (sTopicsTemplate.Contains("[STATUS]"))
                    {
                        string sImg = string.Empty;
                        if (StatusId == -1)
                        {
                            sTopicsTemplate = sTopicsTemplate.Replace("[STATUS]", string.Empty);
                        }
                        else
                        {
                            //sImg = "<img alt=\"[RESX:PostStatus" + StatusId.ToString() + "]\" src=\"" + MyThemePath + "/images/status" + StatusId.ToString() + ".png\" />";

                            sImg = "<div><i class=\"fa fa-status" + StatusId.ToString() + " fa-red fa-2x\"></i></div>";
                        }
                        sTopicsTemplate = sTopicsTemplate.Replace("[STATUS]", sImg);
                    }

                    if (sTopicsTemplate.Contains("[LASTPOST]"))
                    {
                        if (LastReplyId == 0)
                        {
                            sTopicsTemplate = TemplateUtils.ReplaceSubSection(sTopicsTemplate, GetDate(LastReplyDate), "[LASTPOST]", "[/LASTPOST]");
                        }
                        else
                        {
                            string sLastReplyTemp = sLastReply;
                            if (bRead)
                            {
                                sLastReplyTemp = sLastReplyTemp.Replace("[AF:ICONLINK:LASTREPLY]", "<a href=\"" + sLastReplyURL + "\" rel=\"nofollow\"><img src=\"" + MyThemePath + "/images/miniarrow_right.png\" style=\"vertical-align:middle;\" alt=\"[RESX:JumpToLastReply]\" border=\"0\" class=\"afminiarrow\" /></a>");
                                sLastReplyTemp = sLastReplyTemp.Replace("[AF:URL:LASTREPLY]", sLastReplyURL);
                            }
                            else
                            {
                                sLastReplyTemp = sLastReplyTemp.Replace("[AF:ICONLINK:LASTREPLY]", string.Empty);
                                sLastReplyTemp = sLastReplyTemp.Replace("[AF:URL:LASTREPLY]", string.Empty);
                            }

                            sLastReplyTemp = sLastReplyTemp.Replace(LastReplySubjectReplaceTag, Utilities.GetLastPostSubject(LastReplyId, TopicId, ForumId, TabId, LastReplySubject, iLength, PageSize, ReplyCount, bRead));
                            //sLastReplyTemp = sLastReplyTemp.Replace("[RESX:BY]", Utilities.GetSharedResource("By.Text"))
                            if (LastReplyAuthorId > 0)
                            {
                                sLastReplyTemp = sLastReplyTemp.Replace("[LASTPOSTDISPLAYNAME]", UserProfiles.GetDisplayName(ModuleId, true, bModApprove, ForumUser.IsAdmin || ForumUser.IsSuperUser, LastReplyAuthorId, LastReplyUserName, LastReplyFirstName, LastReplyLastName, LastReplyDisplayName).ToString().Replace("&amp;#", "&#"));
                            }
                            else
                            {
                                sLastReplyTemp = sLastReplyTemp.Replace("[LASTPOSTDISPLAYNAME]", LastReplyAuthorName);
                            }

                            sLastReplyTemp = sLastReplyTemp.Replace("[LASTPOSTDATE]", GetDate(LastReplyDate));
                            sTopicsTemplate = TemplateUtils.ReplaceSubSection(sTopicsTemplate, sLastReplyTemp, "[LASTPOST]", "[/LASTPOST]");
                        }
                    }

                    sTopics += sTopicsTemplate;
                    rowcount += 1;
                }
                sOutput = TemplateUtils.ReplaceSubSection(Template, sTopics, "[" + Section + "]", "[/" + Section + "]");
            }
            else
            {
                sOutput = TemplateUtils.ReplaceSubSection(Template, string.Empty, "[" + Section + "]", "[/" + Section + "]");
            }

            return sOutput;
        }
        private void BuildPager()
        {
            if (TopicRowCount > 0)
            {
                DotNetNuke.Modules.ActiveForums.Controls.PagerNav Pager1 = null;
                Pager1 = (DotNetNuke.Modules.ActiveForums.Controls.PagerNav)(this.FindControl("Pager1"));

                DotNetNuke.Modules.ActiveForums.Controls.PagerNav Pager2 = null;
                object obj = this.FindControl("Pager2");
                if (obj != null)
                {
                    Pager2 = (DotNetNuke.Modules.ActiveForums.Controls.PagerNav)obj;
                }

                int intPages = 0;
                if (Pager1 != null)
                {
                    intPages = Convert.ToInt32(System.Math.Ceiling(TopicRowCount / (double)PageSize));
                    Pager1.PageCount = intPages;
                    Pager1.PageMode = PagerNav.Mode.Links;
                    if (!(string.IsNullOrEmpty(ForumInfo.PrefixURL)) && MainSettings.URLRewriteEnabled)
                    {
                        if (!(string.IsNullOrEmpty(MainSettings.PrefixURLBase)))
                        {
                            Pager1.BaseURL = "/" + MainSettings.PrefixURLBase;
                        }
                        Pager1.BaseURL += "/" + ForumInfo.ForumGroup.PrefixURL + "/" + ForumInfo.PrefixURL + "/";
                        Pager1.PageMode = PagerNav.Mode.Links;
                    }
                    Pager1.CurrentPage = PageId;
                    Pager1.TabID = Convert.ToInt32(Request.Params["TabId"]);
                    Pager1.ForumID = ForumId;
                    Pager1.UseShortUrls = MainSettings.UseShortUrls;
                    Pager1.PageText = Utilities.GetSharedResource("[RESX:Page]");
                    Pager1.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
                    Pager1.View = Views.Topics;
                    if (Request.Params["afsort"] != null)
                    {
                        string[] Params = { "afsort=" + Request.Params["afsort"], "afcol=" + Request.Params["afcol"] };
                        Pager1.Params = Params;
                        if (Pager2 != null)
                        {
                            Pager2.Params = Params;
                        }

                    }
                }


                if (Pager2 != null)
                {
                    Pager2.PageMode = Modules.ActiveForums.Controls.PagerNav.Mode.Links; // DotNetNuke.Modules.ActiveForums.Controls.Pager.Mode.CallBack
                    if (!(string.IsNullOrEmpty(ForumInfo.PrefixURL)) && MainSettings.URLRewriteEnabled)
                    {
                        if (!(string.IsNullOrEmpty(MainSettings.PrefixURLBase)))
                        {
                            Pager2.BaseURL = "/" + MainSettings.PrefixURLBase;
                        }
                        Pager2.BaseURL += "/" + ForumInfo.ForumGroup.PrefixURL + "/" + ForumInfo.PrefixURL + "/";
                        Pager2.PageMode = PagerNav.Mode.Links;
                    }
                    Pager2.UseShortUrls = MainSettings.UseShortUrls;
                    Pager2.PageCount = intPages;
                    Pager2.CurrentPage = PageId;
                    Pager2.TabID = Convert.ToInt32(Request.Params["TabId"]);
                    Pager2.ForumID = ForumId;
                    Pager2.PageText = Utilities.GetSharedResource("[RESX:Page]");
                    Pager2.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
                    Pager2.View = Views.Topics;
                }
            }


        }
        private string GetIcon(int LastTopicRead, int LastReplyRead, int TopicId, int ReplyId, string Icon, bool Pinned = false, bool Locked = false)
        {
            if (Icon != string.Empty)
            {
                return "<img src=\"" + MyThemePath + "/emoticons/" + Icon + "\" alt=\"" + Icon + "\" />";
            }

            if (Pinned && Locked)
            {
                return "<img src=\"" + MyThemePath + "/images/topic_pinlocked.png\" alt=\"[RESX:PinnedLocked]\" />";
            }
            if (Pinned)
            {
                return "<img src=\"" + MyThemePath + "/images/topic_pin.png\" alt=\"[RESX:Pinned]\" />";
            }
            if (Locked)
            {
                return "<img src=\"" + MyThemePath + "/images/topic_lock.png\" alt=\"[RESX:Locked]\" />";
            }
            if (!Request.IsAuthenticated)
            {
                return "<img src=\"" + MyThemePath + "/images/topic.png\" alt=\"[RESX:TopicRead]\" />";
            }
            if (LastTopicRead == 0)
            {
                return "<img src=\"" + MyThemePath + "/images/topic_new.png\" alt=\"[RESX:TopicNew]\" />";
            }
            if (LastTopicRead > 0 & LastReplyRead < ReplyId)
            {
                return "<img src=\"" + MyThemePath + "/images/topic_new.png\" alt=\"[RESX:TopicNew]\" />";
            }
            return "<img src=\"" + MyThemePath + "/images/topic.png\" alt=\"[RESX:TopicRead]\" />";
        }

        private string GetTitle(string Body, int AuthorId)
        {
            if (bRead || AuthorId == this.UserId)
            {
                Body = Body.Replace("<br>", System.Environment.NewLine);
                Body = Body.Replace("[", "&#91");
                Body = Body.Replace("]", "&#93");
                Body = Utilities.StripHTMLTag(Body);
                Body = Body.Length > 500 ? Body.Substring(0, 500) + "..." : Body;
                Body = Body.Replace("\"", "'");
                Body = Body.Replace("?", string.Empty);
                Body = Body.Replace("+", string.Empty);
                Body = Body.Replace("%", string.Empty);
                Body = Body.Replace("#", string.Empty);
                Body = Body.Replace("@", string.Empty);
                return Body.Trim();
            }
            else
            {
                return string.Empty;
            }

        }
        private string GetBody(string Body, int AuthorId)
        {
            if (bRead || AuthorId == this.UserId)
            {
                Body = Body.Replace("[", "&#91");
                Body = Body.Replace("]", "&#93");
                return Body;
            }
            else
            {
                return string.Empty;
            }

        }
        private string GetTopic(int ModuleID, int TabID, int ForumID, int PostID, string Subject, string BodyTitle, int UserID, int PostUserID, int Replies, int ForumOwnerID, string sLink)
        {
            string sOut = null;
            //Subject = Replace(Subject, "[", "&#91")
            //Subject = Replace(Subject, "]", "&#93")
            Subject = Utilities.StripHTMLTag(Subject);
            Subject = Subject.Replace("\"", string.Empty);
            //Subject = Subject.Replace("'", string.Empty);
            Subject = Subject.Replace("#", string.Empty);
            Subject = Subject.Replace("%", string.Empty);
            // Subject = Subject.Replace("?", String.Empty)
            Subject = Subject.Replace("+", string.Empty);

            if (bRead)
            {
                if (sLink == string.Empty)
                {
                    string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic };
                    if (MainSettings.UseShortUrls)
                    {
                        Params = new string[] { ParamKeys.TopicId + "=" + PostID };
                    }
                    sOut = "<a title=\"" + BodyTitle + "\" href=\"" + NavigateUrl(TabID, "", Params) + "\">" + Subject + "</a>";
                }
                else
                {
                    sOut = "<a title=\"" + BodyTitle + "\" href=\"" + sLink + "\">" + Subject + "</a>";
                }

                //sOut = sOut & GetSubPages(TabID, ModuleID, Replies, ForumID, PostID)
            }
            else
            {
                sOut = Subject;
            }
            return sOut;
        }
        private string GetSubPages(int TabID, int ModuleID, int Replies, int ForumID, int PostID)
        {
            int i = 0;
            string sOut = "";

            if (Replies + 1 > PageSize)
            {
                //sOut = "<div class=""afpagermini"">" & GetSharedResource("SubPages.Text") & "&nbsp;"
                sOut = "<div class=\"afpagermini\">(<img src=\"" + MyThemePath + "/images/icon_multipage.png\" alt=\"[RESX:MultiPageTopic]\" style=\"vertical-align:middle;\" />";
                //Jump to pages
                int intPostPages = 0;
                intPostPages = Convert.ToInt32(System.Math.Ceiling((double)(Replies + 1) / PageSize));
                if (intPostPages > 3)
                {
                    for (i = 1; i <= 3; i++)
                    {
                        if (UseAjax)
                        {
                            var @params = new List<string> { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic };
                            if (MainSettings.UseShortUrls)
                            {
                                @params = new List<string> { ParamKeys.TopicId + "=" + PostID };
                            }
                            if (i > 1)
                            {
                                @params.Add(ParamKeys.PageJumpId + "=" + i);
                            }
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", @params.ToArray()) + "\">" + i + "</a>&nbsp;";
                        }
                        else
                        {
                            var @params = new List<string> { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic };
                            if (MainSettings.UseShortUrls)
                            {
                                @params = new List<string> { ParamKeys.TopicId + "=" + PostID };
                            }
                            if (i > 1)
                            {
                                @params.Add(ParamKeys.PageId + "=" + i);
                            }
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", @params.ToArray()) + "\">" + i + "</a>&nbsp;";
                        }

                    }
                    if (intPostPages > 4)
                    {
                        sOut += "...&nbsp;";
                    }
                    if (UseAjax)
                    {
                        var @params = new List<string> { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic };
                        if (MainSettings.UseShortUrls)
                        {
                            @params = new List<string> { ParamKeys.TopicId + "=" + PostID };
                        }
                        if (i > 1)
                        {
                            @params.Add(ParamKeys.PageJumpId + "=" + i);
                        }
                        sOut += "<a href=\"" + NavigateUrl(TabID, "", @params.ToArray()) + "\">" + i + "</a>&nbsp;";

                    }
                    else
                    {
                        var @params = new List<string> { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic };
                        if (MainSettings.UseShortUrls)
                        {
                            @params = new List<string> { ParamKeys.TopicId + "=" + PostID };
                        }
                        if (i > 1)
                        {
                            @params.Add(ParamKeys.PageId + "=" + i);
                        }
                        sOut += "<a href=\"" + NavigateUrl(TabID, "", @params.ToArray()) + "\">" + i + "</a>&nbsp;";
                    }

                }
                else
                {
                    for (i = 1; i <= intPostPages; i++)
                    {
                        if (UseAjax)
                        {
                            //sOut &= "<span class=""afpagerminiitem"" onclick=""javascript:afPageJump(" & i & ");"">" & i & "</span>&nbsp;"
                            var @params = new List<string> { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic };
                            if (MainSettings.UseShortUrls)
                            {
                                @params = new List<string> { ParamKeys.TopicId + "=" + PostID };
                            }
                            if (i > 1)
                            {
                                @params.Add(ParamKeys.PageJumpId + "=" + i);
                            }
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", @params.ToArray()) + "\">" + i + "</a>&nbsp;";
                        }
                        else
                        {
                            var @params = new List<string> { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic };
                            if (MainSettings.UseShortUrls)
                            {
                                @params = new List<string> { ParamKeys.TopicId + "=" + PostID };
                            }
                            if (i > 1)
                            {
                                @params.Add(ParamKeys.PageId + "=" + i);
                            }
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", @params.ToArray()) + "\">" + i + "</a>&nbsp;";
                        }

                    }
                }

                sOut += ")</div>";
            }
            return sOut;
        }
        private string GetRowCSS(int LastTopicRead, int LastReplyRead, int TopicId, int ReplyId, int RowCount)
        {
            bool isRead = false;
            if (LastTopicRead >= TopicId && LastReplyRead >= ReplyId)
            {
                isRead = true;
            }
            if (!Request.IsAuthenticated)
            {
                isRead = true;
            }
            if (isRead == true)
            {
                if (RowCount % 2 == 0)
                {
                    return "aftopicrow";
                }
                else
                {
                    return "aftopicrowalt";
                }
            }
            else
            {
                if (RowCount % 2 == 0)
                {
                    return "aftopicrownew";
                }
                else
                {
                    return "aftopicrownewalt";
                }
            }
        }

        /*
        private void ctlModal_Callback(object sender, CallBackEventArgs e)
        {
            switch (e.Parameters[0].ToLowerInvariant())
            {
                case "load":
                    ctlModal.ModalContent.Controls.Clear();
                    string ctlPath = string.Empty;
                    string ctrl = e.Parameters[1].ToLowerInvariant();
                    string ctlParams = e.Parameters[2].ToLowerInvariant();
                    Session["modal_View"] = ctrl;
                    Session["modal_options"] = ctlParams;
                    LoadModal(ctrl, ctlParams);
                    break;
                case "clear":
                    ctlModal.ModalContent.Controls.Clear();
                    Session["modal_View"] = null;
                    Session["modal_options"] = null;
                    break;
            }
            ctlModal.ModalContent.RenderControl(e.Output);
        }
        private void LoadModal(string ctrl, string @params = "")
        {
            ctlModal.ModalContent.Controls.Clear();
            string ctlPath = string.Empty;
            Session["modal_View"] = ctrl;
            Session["modal_options"] = @params;
            ctlPath = "~/DesktopModules/activeforums/controls/af_" + ctrl + ".ascx";
            ForumBase ctl = (ForumBase)(LoadControl(ctlPath));
            ctl.ID = ctrl;
            ctl.ModuleConfiguration = this.ModuleConfiguration;

            if (!(@params == string.Empty))
            {
                ctl.Params = @params;
            }
            if (!(ctlModal.ModalContent.Controls.Contains(ctl)))
            {
                ctlModal.ModalContent.Controls.Add(ctl);
            }
        }
         */

        private static string CheckControls(string template)
        {
            const string tagRegistration = "<%@ Register TagPrefix=\"ac\" Namespace=\"DotNetNuke.Modules.ActiveForums.Controls\" Assembly=\"DotNetNuke.Modules.ActiveForums\" %>";

            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;

            if (!(template.Contains(tagRegistration)))
                template = tagRegistration + template;

            return template;
        }
    }
}


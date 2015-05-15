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

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:ForumView runat=server></{0}:ForumView>")]
    public class ForumView : ForumBase
    {
        private DataTable dtForums;
        private string ForumURL = string.Empty;
        private string ForumPageTitle = string.Empty;
        private string _DisplayTemplate = "";
        private int _currentUserId = -1;
        public bool SubsOnly { get; set; }

        public string DisplayTemplate
        {
            get
            {
                return _DisplayTemplate;
            }
            set
            {
                _DisplayTemplate = value;
            }
        }
        public DataTable ForumTable
        {
            get
            {
                return dtForums;
            }
            set
            {
                dtForums = value;
            }
        }
        public int CurrentUserId
        {
            get
            {
                return _currentUserId;
            }
            set
            {
                _currentUserId = value;
            }
        }
        protected af_quickjump ctlForumJump = new af_quickjump();
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AppRelativeVirtualPath = "~/";

            try
            {

                if (CurrentUserId == -1)
                {
                    CurrentUserId = UserId;
                }
                string sOutput = string.Empty;
                try
                {
                    int defaultTemplateId = MainSettings.ForumTemplateID;
                    if (DefaultForumViewTemplateId >= 0)
                    {
                        defaultTemplateId = DefaultForumViewTemplateId;
                    }
                    sOutput = BuildForumView(defaultTemplateId, CurrentUserId, Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MainSettings.Theme + "/"));
                }
                catch (Exception ex)
                {
                    //sOutput = ex.Message
                    //DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, ex)
                }

                if (sOutput != string.Empty)
                {
                    try
                    {
                        if (sOutput.Contains("[TOOLBAR"))
                        {
                            var lit = new LiteralControl();
                            object sToolbar = null; //DataCache.CacheRetrieve("aftb" & ModuleId)
                            sToolbar = Utilities.GetFileContent(SettingKeys.TemplatePath + "ToolBar.txt");
                            DataCache.CacheStore("aftb" + ModuleId, sToolbar);
                            sToolbar = Utilities.ParseToolBar(sToolbar.ToString(), TabId, ModuleId, UserId, CurrentUserType);
                            lit.Text = sToolbar.ToString();
                            sOutput = sOutput.Replace("[TOOLBAR]", sToolbar.ToString());
                        }
                        Control tmpCtl = null;
                        try
                        {

                            tmpCtl = ParseControl(sOutput);

                        }
                        catch (Exception ex)
                        {

                        }
                        if (tmpCtl != null)
                        {
                            try
                            {
                                Controls.Add(tmpCtl);
                                LinkControls(Controls);
                                if (!SubsOnly)
                                {
                                    var plh = (PlaceHolder)(tmpCtl.FindControl("plhQuickJump"));
                                    if (plh != null)
                                    {
                                        ctlForumJump = new af_quickjump { MOID = ModuleId, dtForums = ForumTable, ModuleId = ModuleId };
                                        plh.Controls.Add(ctlForumJump);
                                    }
                                    plh = (PlaceHolder)(tmpCtl.FindControl("plhUsersOnline"));
                                    if (plh != null)
                                    {
                                        ForumBase ctlWhosOnline;
                                        ctlWhosOnline = (ForumBase)(LoadControl("~/Desktopmodules/ActiveForums/controls/af_usersonline.ascx"));
                                        ctlWhosOnline.ModuleConfiguration = ModuleConfiguration;
                                        plh.Controls.Add(ctlWhosOnline);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #endregion
        #region Public Methods
        public string BuildForumView(int ForumTemplateId, int CurrentUserId, string ThemePath)
        {
            try
            {
                SettingsInfo MainSettings = DataCache.MainSettings(ModuleId);
                string sOutput = string.Empty;
                string sTemplate;
                int TemplateCache = MainSettings.TemplateCache;
                if (UseTemplatePath && TemplatePath != string.Empty)
                {
                    DisplayTemplate = Utilities.GetFileContent(TemplatePath + "ForumView.htm");
                    DisplayTemplate = Utilities.ParseSpacer(DisplayTemplate);
                }
                else if (DisplayTemplate == string.Empty)
                {
                    DisplayTemplate = Utilities.GetFileContent(Server.MapPath(ThemePath) + "templates/ForumView.ascx");
                    DisplayTemplate = Utilities.ParseSpacer(DisplayTemplate);
                }

                sTemplate = DisplayTemplate == string.Empty ? DataCache.GetCachedTemplate(TemplateCache, ModuleId, "ForumView", ForumTemplateId) : DisplayTemplate;
                try
                {
                    sTemplate = ParseControls(sTemplate);
                }
                catch (Exception ex)
                {
                    Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
                    sTemplate = ex.Message; //ParseControls(sTemplate)
                }
                if (sTemplate.Contains("[NOTOOLBAR]"))
                {
                    if (HttpContext.Current.Items.Contains("ShowToolbar"))
                    {
                        HttpContext.Current.Items["ShowToolbar"] = false;
                    }
                    else
                    {
                        HttpContext.Current.Items.Add("ShowToolbar", false);
                    }
                    sTemplate = sTemplate.Replace("[NOTOOLBAR]", string.Empty);
                }
                if (sTemplate.Contains("[FORUMS]"))
                {
                    string sGroupSection = string.Empty;
                    string sGroupSectionTemp = TemplateUtils.GetTemplateSection(sTemplate, "[GROUPSECTION]", "[/GROUPSECTION]");
                    string sGroup = TemplateUtils.GetTemplateSection(sTemplate, "[GROUP]", "[/GROUP]");
                    string sForums = string.Empty;
                    string sForumTemp = TemplateUtils.GetTemplateSection(sTemplate, "[FORUMS]", "[/FORUMS]");
                    string tmpGroup = string.Empty;
                    //Dim reader As IDataReader = Nothing
                    if (ForumTable == null)
                    {
                        var ds = new DataSet();
                        dtForums = new DataTable();
                        var fc = new ForumController();
                        ForumTable = fc.GetForumView(PortalId, ModuleId, CurrentUserId, UserInfo.IsSuperUser, ForumIds); // KR - added cache retreival
                        //ds = DataProvider.Instance.UI_ForumView(PortalId, ModuleId, CurrentUserId, UserInfo.IsSuperUser, ForumIds)
                        //ForumTable = ds.Tables(0)
                    }
                    string sCrumb = string.Empty;
                    string sGroupName = string.Empty;
                    //Dim ForumGroupId As Integer = 0
                    if (ForumGroupId != -1)
                    {
                        DataRow tmpDR = null;
                        ForumTable.DefaultView.RowFilter = "ForumGroupId = " + ForumGroupId;
                        if (ForumTable.DefaultView.ToTable().Rows.Count > 0)
                        {
                            tmpDR = ForumTable.DefaultView.ToTable().Rows[0];
                        }
                        if (tmpDR != null)
                        {
                            sGroupName = tmpDR["GroupName"].ToString();
                            sCrumb = "<div class=\"afcrumb\"><i class=\"fa fa-comments-o fa-grey\"></i>  <a href=\"" + Utilities.NavigateUrl(TabId) + "\">[RESX:ForumMain]</a>  <i class=\"fa fa-long-arrow-right fa-grey\"></i>  " + tmpDR["GroupName"] + "</div>";
                        }


                        //dtForums.DefaultView.RowFilter = ""

                    }
                    if (ParentForumId != -1)
                    {
                        //SubsOnly = True
                        DataRow tmpDR = null;
                        string sFilter = "ForumId = " + ParentForumId + " ";

                        ForumTable.DefaultView.RowFilter = sFilter;
                        if (ForumTable.DefaultView.ToTable().Rows.Count > 0)
                        {
                            tmpDR = ForumTable.DefaultView.ToTable().Rows[0];
                        }
                        ForumTable.DefaultView.RowFilter = "";
                        sGroupName = tmpDR["GroupName"].ToString();

                    }
                    if (MainSettings.UseSkinBreadCrumb && ForumTable.Rows.Count > 0 && SubsOnly == false && ForumGroupId != -1)
                    {
                        Environment.UpdateBreadCrumb(Page.Controls, "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + ForumGroupId) + "\">" + sGroupName + "</a>");
                        sTemplate = sTemplate.Replace("<div class=\"afcrumb\">[FORUMMAINLINK] > [FORUMGROUPLINK]</div>", string.Empty);
                        sTemplate = sTemplate.Replace("[BREADCRUMB]", string.Empty);
                    }
                    else
                    {
                        sTemplate = sTemplate.Replace("[BREADCRUMB]", sCrumb);
                    }

                    int iForum = 1;
                    int ForumCount = 0;
                    bool hasForums = false;
                    DataTable rsForums = ForumTable.DefaultView.ToTable();
                    Forum fi;
                    int tmpGroupCount = 0;
                    int asForumGroupId;
                    var social = new Social();
                    asForumGroupId = social.GetMasterForumGroup(PortalId, TabId);
                    int groupForumsCount = 0;
                    foreach (DataRow dr in rsForums.Rows)
                    {

                        //If CBool(dr("CanView")) Or (Not CBool(dr("GroupHidden"))) Then ' And Not CBool(dr("CanView"))) Or UserInfo.IsSuperUser Then
                        if (!(asForumGroupId == Convert.ToInt32(dr["ForumGroupId"]) && Convert.ToBoolean(dr["GroupHidden"])))
                        {
                            if (tmpGroup != dr["GroupName"].ToString())
                            {
                                if (tmpGroupCount < Globals.GroupCount)
                                {
                                    ForumTable.DefaultView.RowFilter = "ForumGroupId = " + Convert.ToInt32(dr["ForumGroupId"]) + " AND ParentForumId = 0";

                                    ForumCount = ForumTable.DefaultView.Count;
                                    ForumTable.DefaultView.RowFilter = "";
                                    if (sForums != string.Empty)
                                    {
                                        sGroupSection = TemplateUtils.ReplaceSubSection(sGroupSection, sForums, "[FORUMS]", "[/FORUMS]");
                                        sForums = string.Empty;
                                    }
                                    int GroupId = Convert.ToInt32(dr["ForumGroupId"]);
                                    sGroupSectionTemp = TemplateUtils.GetTemplateSection(sTemplate, "[GROUPSECTION]", "[/GROUPSECTION]");
                                    sGroupSectionTemp = sGroupSectionTemp.Replace("[GROUPNAME]", dr["GroupName"].ToString());
                                    sGroupSectionTemp = sGroupSectionTemp.Replace("[FORUMGROUPID]", dr["ForumGroupId"].ToString());
                                    sGroupSectionTemp = sGroupSectionTemp.Replace("[GROUPCOLLAPSE]", "<img class=\"afarrow\" id=\"imgGroup" + GroupId.ToString() + "\" onclick=\"toggleGroup(" + GroupId.ToString() + ");\" src=\"" + ThemePath + GetImage(GroupId) + "\" alt=\"[RESX:ToggleGroup]\" />");

                                    //any replacements on the group
                                    string sNewGroup = "<div id=\"group" + GroupId + "\" " + GetDisplay(GroupId) + " class=\"afgroup\">" + sGroup + "</div>";
                                    sGroupSectionTemp = TemplateUtils.ReplaceSubSection(sGroupSectionTemp, sNewGroup, "[GROUP]", "[/GROUP]");
                                    sGroupSection += sGroupSectionTemp;
                                    tmpGroup = dr["GroupName"].ToString();
                                    tmpGroupCount += 1;
                                    iForum = 1;
                                }

                            }
                            if (iForum <= Globals.ForumCount)
                            {
                                fi = FillForumRow(dr);
                                bool canView = Permissions.HasPerm(fi.Security.View, ForumUser.UserRoles);
                                if (canView || (!fi.Hidden))
                                {
                                    sForumTemp = TemplateUtils.GetTemplateSection(sTemplate, "[FORUMS]", "[/FORUMS]");
                                    hasForums = true;
                                    if (fi.ParentForumId == 0 || SubsOnly || (SubsOnly == false && fi.ParentForumId > 0 && rsForums.Rows.Count == 1))
                                    {
                                        sForumTemp = ParseForumRow(sForumTemp, fi, iForum, ThemePath, ForumCount);
                                        iForum += 1;
                                        sForums += sForumTemp;
                                    }
                                }
                            }
                        }




                        //End If
                    }

                    if (hasForums == false && SubsOnly)
                    {
                        return string.Empty;
                    }
                    if (sForums != string.Empty)
                    {
                        sGroupSection = TemplateUtils.ReplaceSubSection(sGroupSection, sForums, "[FORUMS]", "[/FORUMS]");
                    }
                    sTemplate = sTemplate.Contains("[GROUPSECTION]") ? TemplateUtils.ReplaceSubSection(sTemplate, sGroupSection, "[GROUPSECTION]", "[/GROUPSECTION]") : sGroupSection;
                    sTemplate = TemplateUtils.ReplaceSubSection(sTemplate, string.Empty, "[FORUMS]", "[/FORUMS]");

                }


                return sTemplate;
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
                return string.Empty;
            }
        }
        private Forum FillForumRow(DataRow dr)
        {
            var fi = new Forum();
            try
            {
                fi.ForumID = Convert.ToInt32(dr["ForumId"]);
                fi.ForumName = dr["ForumName"].ToString();
                fi.ForumDesc = dr["ForumDesc"].ToString();
                fi.Security.Read = dr["CanRead"].ToString();
                fi.Security.View = dr["CanView"].ToString();
                fi.Security.Subscribe = dr["CanSubscribe"].ToString();
                fi.ParentForumId = Convert.ToInt32(dr["ParentForumId"]);


                fi.LastPostSubject = dr["LastPostSubject"].ToString();
                fi.LastPostDisplayName = dr["LastPostAuthorName"].ToString().Replace("&amp;#", "&#");
                fi.LastPostUserID = Convert.ToInt32(dr["LastPostAuthorId"]);
                fi.LastPostDateTime = Convert.ToDateTime(dr["LastPostDate"]);

                fi.LastPostUserName = fi.LastPostDisplayName;


                if (dr["LastRead"].ToString() != string.Empty)
                {
                    fi.LastRead = Convert.ToDateTime(dr["LastRead"]);
                }

                fi.ForumGroup = new ForumGroupInfo
                {
                    ForumGroupId = int.Parse(dr["ForumGroupId"].ToString()),
                    PrefixURL = dr["GroupPrefixURL"].ToString()
                };
                fi.ForumGroupId = int.Parse(dr["ForumGroupId"].ToString());
                fi.TopicUrl = dr["TopicURL"].ToString();

                fi.TopicSubject = dr["LastPostSubject"].ToString();
                fi.TopicId = Convert.ToInt32(dr["LastTopicId"]);

                fi.TotalTopics = Convert.ToInt32(dr["TotalTopics"]);
                fi.TotalReplies = Convert.ToInt32(dr["TotalReplies"]);
                fi.Hidden = Convert.ToBoolean(dr["ForumHidden"]);
                fi.LastReplyId = Convert.ToInt32(dr["LastReplyId"]);
                fi.LastTopicId = Convert.ToInt32(dr["LastTopicId"]);
                fi.PrefixURL = dr["PrefixURL"].ToString();
                fi.LastPostID = fi.LastReplyId == 0 ? fi.LastTopicId : fi.LastReplyId;

                //.Active = CBool(dr("Active"))
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
            }

            return fi;
        }
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                if ((ctrl) is ForumBase)
                {
                    ((ForumBase)ctrl).ModuleConfiguration = ModuleConfiguration;

                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }
        private string ParseControls(string Template)
        {
            string sOutput = Template;
            sOutput = sOutput.Replace("[JUMPTO]", "<asp:placeholder id=\"plhQuickJump\" runat=\"server\" />");
            if (sOutput.Contains("[STATISTICS]"))
            {
                sOutput = sOutput.Replace("[STATISTICS]", "<am:Stats id=\"amStats\" MID=\"" + ModuleId + "\" PID=\"" + PortalId.ToString() + "\" runat=\"server\" />");
            }
            if (sOutput.Contains("[WHOSONLINE]"))
            {
                sOutput = sOutput.Replace("[WHOSONLINE]", MainSettings.UsersOnlineEnabled ? "<asp:placeholder id=\"plhUsersOnline\" runat=\"server\" />" : string.Empty);
            }
            sOutput = sOutput.Replace("[PORTALID]", PortalId.ToString());
            sOutput = sOutput.Replace("[MODULEID]", ModuleId.ToString());
            sOutput = sOutput.Replace("[TABID]", TabId.ToString());
            sOutput = sOutput.Replace("[USERID]", CurrentUserId.ToString());
            return sOutput;
        }
        private string ParseForumRow(string Template, Forum fi, int currForumIndex, string ThemePath, int totalForums)
        {


            if (Template.Contains("[SUBFORUMS]") && Template.Contains("[/SUBFORUMS]"))
            {
                Template = GetSubForums(Template, fi.ForumID, ModuleId, TabId, ThemePath);
            }
            else
            {
                Template = Template.Replace("[SUBFORUMS]", GetSubForums(fi.ForumID, ModuleId, TabId));
            }
            string[] css = null;
            string cssmatch = string.Empty;
            if (Template.Contains("[CSS:"))
            {
                string pattern = "(\\[CSS:.+?\\])";
                if (Regex.IsMatch(Template, pattern))
                {
                    cssmatch = Regex.Match(Template, pattern).Value;
                    css = cssmatch.Split(':'); //0=CSS,1=TopRow, 2=mid rows, 3=lastRow
                }
            }
            if (cssmatch != string.Empty)
            {
                if (currForumIndex == 1)
                {
                    Template = Template.Replace(cssmatch, css[1]);
                }
                else if (currForumIndex > 1 & currForumIndex < totalForums)
                {
                    Template = Template.Replace(cssmatch, css[2]);
                }
                else
                {
                    Template = Template.Replace(cssmatch, css[3].Replace("]", string.Empty));
                }
            }

            bool canView = Permissions.HasPerm(fi.Security.View, ForumUser.UserRoles);
            bool canSubscribe = Permissions.HasPerm(fi.Security.Subscribe, ForumUser.UserRoles);
            bool canRead = Permissions.HasPerm(fi.Security.Read, ForumUser.UserRoles);
            string sIcon = TemplateUtils.ShowIcon(canView, fi.ForumID, CurrentUserId, fi.LastPostDateTime, fi.LastRead, fi.LastPostID);
            string sIconImage = "<img alt=\"" + fi.ForumName + "\" src=\"" + ThemePath + "images/" + sIcon + "\" />";

            if (Template.Contains("[FORUMICON]"))
            {
                Template = Template.Replace("[FORUMICON]", sIconImage);
            }
            else if (Template.Contains("[FORUMICONCSS]"))
            {
                string sFolderCSS = "fa-folder fa-blue";
                switch (sIcon.ToLower())
                {
                    case "folder.png":
                        sFolderCSS = "fa-folder fa-blue";
                        break;
                    case "folder_new.png":
                        sFolderCSS = "fa-folder fa-red";
                        break;
                    case "folder_forbidden.png":
                        sFolderCSS = "fa-folder fa-grey";
                        break;
                    case "folder_closed.png":
                        sFolderCSS = "fa-folder-o fa-grey";
                        break;
                }
                Template = Template.Replace("[FORUMICONCSS]", "<div style=\"height:30px;margin-right:10px;\"><i class=\"fa " + sFolderCSS + " fa-2x\"></i></div>");
            }

            var ctlUtils = new ControlUtils();
            ForumURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, fi.ForumGroup.PrefixURL, fi.PrefixURL, fi.ForumGroupId, fi.ForumID, -1, string.Empty, -1, -1, string.Empty, 1, -1, SocialGroupId);

            //ForumURL = GetForumLink(.ForumID, TabId, canView, MainSettings.UseShortUrls, .PrefixURL)
            Template = Template.Replace("[FORUMNAME]", GetForumLink(fi.ForumName, fi.ForumID, TabId, canView, ForumURL));
            Template = Template.Replace("[FORUMNAMENOLINK]", fi.ForumName);
            Template = Template.Replace("[FORUMID]", fi.ForumID.ToString());
            if (Template.Contains("[RSSLINK]"))
            {
                if (fi.AllowRSS && canRead)
                {
                    string Url;
                    Url = Common.Globals.AddHTTP(Common.Globals.GetDomainName(Request)) + "/DesktopModules/ActiveForums/feeds.aspx?portalid=" + PortalId + "&forumid=" + fi.ForumID + "&tabid=" + TabId + "&moduleid=" + ModuleId;
                    Template = Template.Replace("[RSSLINK]", "<a href=\"" + Url + "\" target=\"_blank\"><img src=\"" + ThemePath + "images/rss.png\" border=\"0\" alt=\"[RESX:RSS]\" /></a>");
                }
                else
                {
                    Template = Template.Replace("[RSSLINK]", "<img src=\"" + ThemePath + "images/rss_disabled.png\" border=\"0\" alt=\"[RESX:RSSDisabled]\" />");
                }
            }

            if (Template.Contains("[AF:CONTROL:TOGGLESUBSCRIBE]"))
            {
                if (canSubscribe)
                {
                    bool IsSubscribed = Subscriptions.IsSubscribed(PortalId, ModuleId, fi.ForumID, 0, SubscriptionTypes.Instant, CurrentUserId);
                    string sAlt = "[RESX:ForumSubscribe:" + IsSubscribed.ToString().ToUpper() + "]";
                    string sImg = ThemePath + "images/email_unchecked.png";
                    if (IsSubscribed)
                    {
                        sImg = ThemePath + "images/email_checked.png";
                    }
                    var subControl = new ToggleSubscribe(0, fi.ForumID, -1);
                    subControl.Checked = IsSubscribed;
                    subControl.DisplayMode = 1;
                    subControl.UserId = CurrentUserId;
                    subControl.ImageURL = sImg;
                    subControl.Text = "[RESX:ForumSubscribe:" + IsSubscribed.ToString().ToUpper() + "]";
                    string subOption = subControl.Render();

                    //Template = Template.Replace("[AF:CONTROL:TOGGLESUBSCRIBE]", "<a href=""javascript:af_toggleSubscribe(" & .ForumID & ",'" & PortalId & "|" & ModuleId & "|" & .ForumID & "|" & CurrentUserId & "');""><img id=""toggleSub" & .ForumID & """ src=""" & sImg & """ border=""0"" alt=""" & sAlt & """ /></a>")
                    Template = Template.Replace("[AF:CONTROL:TOGGLESUBSCRIBE]", subOption);
                }
                else
                {
                    Template = Template.Replace("[AF:CONTROL:TOGGLESUBSCRIBE]", "<img src=\"" + ThemePath + "email_disabled.png\" border=\"0\" alt=\"[RESX:ForumSubscribe:Disabled]\" />");
                }
            }

            Template = canRead ? Template.Replace("[AF:CONTROL:ADDFAVORITE]", "<a href=\"javascript:afAddBookmark('" + fi.ForumName + "','" + ForumURL + "');\"><img src=\"" + ThemePath + "images/favorites16_add.png\" border=\"0\" alt=\"[RESX:AddToFavorites]\" /></a>") : Template.Replace("[AF:CONTROL:ADDFAVORITE]", string.Empty);
            if (Template.Contains("[AF:CONTROL:ADDTHIS"))
            {
                Template = TemplateUtils.ParseSpecial(Template, SpecialTokenTypes.AddThis, ForumURL, fi.ForumName, canRead, MainSettings.AddThisAccount);
            }

            if (fi.ForumDesc != "")
            {
                Template = Template.Replace("[FORUMDESCRIPTION]", "<i class=\"fa fa-file-o fa-grey\"></i>&nbsp;" + fi.ForumDesc);
            }
            else
            {
                Template = Template.Replace("[FORUMDESCRIPTION]", "");
            }

            Template = Template.Replace("[TOTALTOPICS]", fi.TotalTopics.ToString());
            Template = Template.Replace("[TOTALREPLIES]", fi.TotalReplies.ToString());
            //Last Post Section
            int intLength = 0;
            if ((Template.IndexOf("[LASTPOSTSUBJECT:", 0) + 1) > 0)
            {
                int inStart = (Template.IndexOf("[LASTPOSTSUBJECT:", 0) + 1) + 17;
                int inEnd = (Template.IndexOf("]", inStart - 1) + 1);
                string sLength = Template.Substring(inStart - 1, inEnd - inStart);
                intLength = Convert.ToInt32(sLength);
            }
            string ReplaceTag = "[LASTPOSTSUBJECT:" + intLength.ToString() + "]";
            if (fi.LastPostID == 0)
            {
                Template = Template.Replace("[RESX:BY]", string.Empty);
                Template = Template.Replace("[DISPLAYNAME]", string.Empty);
                Template = Template.Replace("[LASTPOSTDATE]", string.Empty);
                Template = Template.Replace(ReplaceTag, string.Empty);
                //Template = TemplateUtils.ParseUserDetails(PortalId, -1, Template, String.Empty)
            }
            else
            {
                if (canView)
                {
                    if (fi.LastPostUserID <= 0)
                    {
                        //Template = Template.Replace("[RESX:BY]", String.Empty)
                        Template = Template.Replace("[DISPLAYNAME]", "<i class=\"fa fa-user fa-fw fa-blue\"></i>&nbsp;" + fi.LastPostDisplayName);
                        //Template = TemplateUtils.ParseUserDetails(PortalId, -1, Template, "FG")
                    }
                    else
                    {
                        bool isMod = CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.ForumMod || CurrentUserType == CurrentUserTypes.SuperUser;
                        bool isAdmin = CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser;
                        Template = Template.Replace("[DISPLAYNAME]", "<i class=\"fa fa-user fa-fw fa-blue\"></i>&nbsp;" + UserProfiles.GetDisplayName(ModuleId, true, isMod, isAdmin, fi.LastPostUserID, fi.LastPostUserName, fi.LastPostFirstName, fi.LastPostLastName, fi.LastPostDisplayName));
                        //Template = TemplateUtils.ParseUserDetails(PortalId, .LastPostUserID, Template, "FG")
                    }
                    DateTime dtLastPostDate = fi.LastPostDateTime;
                    Template = Template.Replace("[LASTPOSTDATE]", Utilities.GetDate(dtLastPostDate, ModuleId, TimeZoneOffset));
                    string Subject = fi.LastPostSubject;
                    if (Subject == "")
                    {
                        Subject = GetSharedResource("[RESX:SubjectPrefix]") + " " + fi.TopicSubject;
                    }
                    if (Subject != string.Empty)
                    {
                        string sDots = "";
                        if (Subject.Length > intLength)
                        {
                            sDots = "...";
                        }

                        Template = Template.Replace(ReplaceTag, GetLastPostSubject(fi.LastPostID, fi.TopicId, fi.ForumID, TabId, Subject, intLength, MainSettings.PageSize, fi));
                    }
                    else
                    {
                        Template = Template.Replace("[RESX:BY]", string.Empty);
                        Template = Template.Replace(ReplaceTag, string.Empty);
                    }
                }
                else
                {
                    Template = Template.Replace("[DISPLAYNAME]", string.Empty);
                    Template = Template.Replace("[LASTPOSTDATE]", string.Empty);
                    Template = Template.Replace("[RESX:BY]", string.Empty);
                    Template = Template.Replace(ReplaceTag, string.Empty);
                }

            }

            return Template;


        }

        #endregion
        #region Private Methods - Helpers
        private string GetForumLink(int ForumId, int TabId, bool CanView, string Url)
        {
            return GetForumLink(string.Empty, ForumId, TabId, CanView, Url);
        }
        private string GetForumLink(string Name, int ForumId, int TabId, bool CanView, string Url)
        {
            string sOut;
            sOut = Name;
            //Dim sTopicURL As String = String.Empty

            //Dim Params() As String = {ParamKeys.ForumId & "=" & ForumId}
            //Dim sURL As String = String.Empty
            //If Not MainSettings.URLRewriteEnabled Or String.IsNullOrEmpty(PrefixURL) Then
            //    sURL = Utilities.NavigateUrl(TabId, "", Params)
            //Else
            //    sURL = "/"
            //    If Not String.IsNullOrEmpty(MainSettings.PrefixURLBase) Then
            //        sURL &= MainSettings.PrefixURLBase & "/"
            //    End If
            //    sURL &= PrefixURL & "/"
            //End If

            if (CanView && Name != string.Empty)
            {
                sOut = "<a href=\"" + Url + "\">" + Name + "</a>";

            }
            else if (CanView && Name == string.Empty)
            {
                return Url;
            }
            else
            {
                sOut = Name;
            }
            return sOut;
        }
        private string GetImage(int GroupID)
        {
            if (Request.Cookies[GroupID + "Show"] != null)
            {
                if (Convert.ToBoolean(Request.Cookies[GroupID + "Show"].Value))
                {
                    return "images/arrows_down.png";
                }
                return "images/arrows_left.png";
            }
            return "images/arrows_down.png";
        }

        private string GetDisplay(int GroupID)
        {
            bool bolShow = true;
            if (Request.Cookies[GroupID + "S"] != null)
            {
                string sShow = Convert.ToString(Request.Cookies[GroupID + "S"].Value);
                bolShow = sShow != "F";
            }
            if (bolShow)
            {
                return " style=\"clear:both;\"";
            }
            return " style=\"clear:both;display:none;\"";
        }
        private string GetLastPostSubject(int LastPostID, int ParentPostID, int ForumID, int TabID, string Subject, int Length, int PageSize, Forum fi)
        {
            //TODO: Verify that this will still jump to topics on page 2
            var sb = new StringBuilder();
            int PostId = LastPostID;
            Subject = Utilities.StripHTMLTag(Subject);
            Subject = Subject.Replace("[", "&#91");
            Subject = Subject.Replace("]", "&#93");
            if (Subject.Length > Length & Length > 0)
            {
                Subject = Subject.Substring(0, Length) + "...";
            }
            if (LastPostID != 0)
            {
                string sTopicURL;
                var ctlUtils = new ControlUtils();
                sTopicURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, fi.ForumGroup.PrefixURL, fi.PrefixURL, fi.ForumGroupId, ForumID, ParentPostID, fi.TopicUrl, -1, -1, string.Empty, 1, -1, SocialGroupId);

                string sURL;
                if (ParentPostID == 0 || LastPostID == ParentPostID)
                {
                    sURL = sTopicURL;
                    //If UseShortUrls Then
                    //    sURL = Utilities.NavigateUrl(TabID, "", New String() {ParamKeys.TopicId & "=" & PostId})
                    //Else
                    //    sURL = Utilities.NavigateUrl(TabID, "", New String() {ParamKeys.ForumId & "=" & ForumID, ParamKeys.ViewType & "=" & Views.Topic, ParamKeys.TopicId & "=" & PostId})
                    //End If
                }
                else
                {
                    if (sTopicURL.EndsWith("/"))
                    {
                        sURL = sTopicURL + "?" + ParamKeys.ContentJumpId + "=" + PostId;
                    }
                    else
                    {
                        var @params = new List<string> { ParamKeys.TopicId + "=" + ParentPostID, ParamKeys.ContentJumpId + "=" + PostId };

                        if (SocialGroupId > 0)
                            @params.Add("GroupId=" + SocialGroupId.ToString());

                        sURL = Utilities.NavigateUrl(TabID, "", @params.ToArray());
                    }



                }
                sb.Append("<a href=\"" + sURL + "\">" + Utilities.HTMLEncode(Subject) + "</a>");
            }
            return sb.ToString();
        }
        private string GetSubForums(int ForumId, int ModuleId, int TabId)
        {
            return GetSubForums(string.Empty, ForumId, ModuleId, TabId, string.Empty);
        }
        private string GetSubForums(string Template, int ForumId, int ModuleId, int TabId, string ThemePath)
        {
            int i = 0;
            string sFilter = "ParentForumId = " + ForumId;
            //If Not ForumIds = String.Empty Then
            //    sFilter = String.Empty
            //    For Each f As String In ForumIds.Split(CChar(";"))
            //        If Not f = String.Empty Then
            //            If Not sFilter = String.Empty Then
            //                sFilter = sFilter & " OR "
            //            End If
            //            sFilter &= " (ParentForumId = " & ForumId & " AND ForumId = " & f & ") "
            //        End If
            //    Next
            //End If
            ForumTable.DefaultView.RowFilter = sFilter;
            int rows = ForumTable.DefaultView.Count;
            if (Template == string.Empty)
            {
                var sb = new StringBuilder();
                string SubForum;
                foreach (DataRow dr in ForumTable.DefaultView.ToTable().Rows)
                {
                    if (Convert.ToInt32(dr["ParentForumId"]) == ForumId)
                    {
                        bool canView = Permissions.HasPerm(dr["CanView"].ToString(), ForumUser.UserRoles);
                        SubForum = GetForumName(ModuleId, canView, Convert.ToBoolean(dr["ForumHidden"]), TabId, Convert.ToInt32(dr["ForumGroupId"]), Convert.ToInt32(dr["ForumId"]), dr["ForumName"].ToString(), MainSettings.UseShortUrls);
                        if (SubForum != string.Empty)
                        {
                            sb.Append(SubForum);
                            if (i < rows - 1)
                            {
                                sb.Append(", ");
                            }
                            i += 1;
                        }

                    }
                }
                string subs = string.Empty;
                if (sb.Length > 0)
                {
                    sb.Insert(0, "<br />[RESX:SubForums] ");
                    subs = sb.ToString();
                    subs = subs.Trim();
                    if (subs.IndexOf(",", subs.Length - 1) > 0)
                    {
                        subs = subs.Substring(0, subs.LastIndexOf(","));
                    }
                }
                ForumTable.DefaultView.RowFilter = "";
                return subs;
            }
            else
            {
                string subs = string.Empty;
                foreach (DataRow dr in ForumTable.DefaultView.ToTable().Rows)
                {
                    i += 1;
                    string tmpSubs = TemplateUtils.GetTemplateSection(Template, "[SUBFORUMS]", "[/SUBFORUMS]");
                    Forum fi = FillForumRow(dr);
                    bool canView = Permissions.HasPerm(dr["CanView"].ToString(), ForumUser.UserRoles);
                    if (canView || (!fi.Hidden) | UserInfo.IsSuperUser)
                    {
                        string sIcon = TemplateUtils.ShowIcon(canView, fi.ForumID, CurrentUserId, fi.LastPostDateTime, fi.LastRead, fi.LastPostID);
                        //string sIconImage = "<img alt=\"" + fi.ForumName + "\" src=\"" + ThemePath + "images/" + sIcon + "\" />";
                        //tmpSubs = tmpSubs.Replace("[FORUMICONSM]", sIconImage.Replace("folder", "folder16"));


                        string sFolderCSS = "fa-folder fa-blue";
                        switch (sIcon.ToLower())
                        {
                            case "folder.png":
                                sFolderCSS = "fa-folder fa-blue";
                                break;
                            case "folder_new.png":
                                sFolderCSS = "fa-folder fa-red";
                                break;
                            case "folder_forbidden.png":
                                sFolderCSS = "fa-folder fa-grey";
                                break;
                            case "folder_closed.png":
                                sFolderCSS = "fa-folder-o fa-grey";
                                break;
                        }
                        tmpSubs = tmpSubs.Replace("[FORUMICONSM]", "<i class=\"fa " + sFolderCSS + " fa-fw\"></i>&nbsp;&nbsp;");

                        //tmpSubs = tmpSubs.Replace("[FORUMICONSM]", "<i class=\"fa fa-folder fa-fw fa-blue\"></i>&nbsp;&nbsp;");
                        tmpSubs = ParseForumRow(tmpSubs, fi, i, ThemePath, ForumTable.DefaultView.ToTable().Rows.Count);
                    }
                    else
                    {
                        tmpSubs = string.Empty;
                    }
                    subs += tmpSubs;
                }
                ForumTable.DefaultView.RowFilter = "";
                Template = TemplateUtils.ReplaceSubSection(Template, subs, "[SUBFORUMS]", "[/SUBFORUMS]");
                return Template;
            }

        }
        private string GetForumName(int ModuleID, bool CanView, bool Hidden, int TabID, int ForumGroupID, int ForumID, string Name, bool UseShortUrls)
        {
            string sOut;
            string[] Params = { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumID };
            if (UseShortUrls)
            {
                Params = new[] { ParamKeys.ForumId + "=" + ForumID };
            }
            if (CanView)
            {
                sOut = "<a href=\"" + Utilities.NavigateUrl(TabID, "", Params) + "\">" + Name + "</a>";
            }
            else if (Hidden)
            {
                sOut = string.Empty;
            }
            else
            {
                sOut = Name;
            }
            return sOut;
        }
        #endregion
    }

}
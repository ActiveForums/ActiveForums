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
using System.Text;
using System.Web.UI;
using System.Xml;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [ToolboxData("<{0}:forumdisplay runat=server></{0}:forumdisplay>")]
    public class ForumDisplay : ControlsBase
    {
        #region Private Members
        //Dim pt As Forums.Utils.TimeCalcItem
        private int _forumGroupId = -1;

        #endregion
        #region Public Properties

        public int ToggleBehavior { get; set; }

        #endregion
        #region Event Handlers
        //Dim sb As New StringBuilder
        //Dim fl As New ForumCollection
        //Dim db As New Data.Forums
        //    fl = db.Forums_List(PortalId, ModuleId, UserId, String.Empty)

        //    For Each f As Forum In fl
        //        sb.Append(f.ForumName)
        //        sb.Append("<br />")
        //    Next
        //    writer.Write(sb.ToString)
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string sTemp;
            //pt = New Forums.Utils.TimeCalcItem("ForumDisplay")

            object obj = DataCache.CacheRetrieve(ModuleId + ForumGroupId + "fvt");
            sTemp = obj == null ? ParseTemplate() : Convert.ToString(obj);
            sTemp = Utilities.LocalizeControl(sTemp);
            if (!(sTemp.Contains(Globals.ControlRegisterAFTag)))
            {
                sTemp = Globals.ControlRegisterAFTag + sTemp;
            }
            Control ctl = Page.ParseControl(sTemp);
            LinkControls(ctl.Controls);
            Controls.Add(ctl);



        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // pt.ProcEnd("ForumDisplay")
        }
        #endregion
        #region Private Methods
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                if (ctrl is ForumRow)
                {
                    ((ForumRow)ctrl).UserRoles = ForumUser.UserRoles;
                }
                if (ctrl is ControlsBase)
                {
                    ((ControlsBase)ctrl).ControlConfig = ControlConfig;
                    ((ControlsBase)ctrl).ModuleConfiguration = ModuleConfiguration;
                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }
        private string ParseTemplate()
        {
            var sb = new StringBuilder();
            string groupTemplate = DisplayTemplate;
            if (groupTemplate.Contains("[GROUPSECTION]"))
            {
                groupTemplate = TemplateUtils.GetTemplateSection(DisplayTemplate, "[GROUPSECTION]", "[/GROUPSECTION]");
            }
            var db = new Data.ForumsDB();
            ForumData = db.ForumListXML(ControlConfig.SiteId, ControlConfig.InstanceId);
            if (ForumData != null)
            {
                XmlNodeList xGroups;
                xGroups = ForumGroupId == -1 ? ForumData.SelectNodes("//groups/group[@active='true']") : ForumData.SelectNodes("//groups/group[@groupid='" + ForumGroupId + "' and @active='true']");

                foreach (XmlNode xNode in xGroups)
                {
                    string sGroup = groupTemplate;
                    int groupId = int.Parse(xNode.Attributes["groupid"].Value);
                    string groupName = xNode.Attributes["name"].Value;
                    sGroup = sGroup.Replace("[GROUPNAME]", groupName);
                    bool isVisible = ToggleBehavior != 1;
                    sGroup = sGroup.Replace("[GROUPCOLLAPSE]", "<af:toggle IsVisible=\"" + isVisible + "\"  id=\"tgGroup" + groupId + "\" key=\"" + groupId + "\" cssclass=\"afarrow\" CssClassOn=\"aficon afarrowdown\" CssClassOff=\"aficon afarrowleft\" runat=\"server\" ImagePath=\"" + ThemePath + "\" />");
                    var xNodeList = ForumData.SelectNodes("//forums/forum[@active='true' and @groupid='" + groupId + "' and @parentforumid='0']");
                    var forums = new StringBuilder();
                    int i = 0;
                    foreach (XmlNode fNode in xNodeList)
                    {
                        i += 1;
                        string sCSSClass = "afforumrowmid";
                        if (i == 1)
                        {
                            sCSSClass = "afforumrowtop";
                        }
                        else if (i == xNodeList.Count)
                        {
                            sCSSClass = "afforumrowbottom";
                        }
                        int fid = int.Parse(fNode.Attributes["forumid"].Value);
                        //Dim viewRoles As String = fNode.Attributes["canview"].Value.ToString
                        //Dim readRoles As String = fNode.Attributes["canread"].Value.ToString
                        string sForum = TemplateUtils.GetTemplateSection(sGroup, "[FORUMS]", "[/FORUMS]");
                        sForum = sForum.Replace("[CSS:ROWCLASS]", sCSSClass);
                        sForum = ParseForumRow(fNode, sForum, groupName);

                        forums.Append(sForum);
                    }

                    sGroup = TemplateUtils.ReplaceSubSection(sGroup, forums.ToString(), "[FORUMS]", "[/FORUMS]");
                    sGroup = sGroup.Replace("[GROUP]", "<af:toggledisplay IsVisible=\"" + isVisible + "\" id=\"tgdGroup" + groupId + "\" key=\"" + groupId + "\" runat=\"server\"><content>");
                    sGroup = sGroup.Replace("[/GROUP]", "</content></af:toggledisplay>");
                    sb.Append(sGroup);
                }
            }
            string sOut = sb.ToString();
            if (sOut.Contains("[GROUPSECTION]"))
            {
                sOut = TemplateUtils.ReplaceSubSection(DisplayTemplate, sb.ToString(), "[GROUPSECTION]", "[/GROUPSECTION]");
            }

            //sOut = sOut.Replace("[BREADCRUMB]", String.Empty)
            //sOut = sOut.Replace("[WHOSONLINE]", "<af:usersonline id=""ctlUsersOnline"" templatefile=""usersonline.htm"" runat=""server"" />")
            //sOut = sOut.Replace("[JUMPTO]", String.Empty)
            //sOut = sOut.Replace("[TEMPLATE:TOOLBAR]", "<af:toolbar id=""ctlToolbar"" templatefile=""toolbar.htm"" runat=""server"" />")

            //DataCache.CacheStore(ModuleId & "fvt", sOut)
            return sOut;
        }
        private string ParseForumRow(XmlNode fNode, string Template, string GroupName)
        {
            string sForum = Template;

            int lasttopicid;
            int lastreplyid;
            int fid;
            string lastpostdate = fNode.Attributes["lastpostdate"].Value;
            DateTime lastReadDate = DateTime.Parse(fNode.Attributes["lastread"].Value);
            string viewRoles = fNode.Attributes["viewroles"].Value;
            string readRoles = fNode.Attributes["readroles"].Value;
            string forumname = fNode.Attributes["name"].Value;
            string hidden = fNode.Attributes["hidden"].Value;
            lasttopicid = int.Parse(fNode.Attributes["lasttopicid"].Value);
            lastreplyid = int.Parse(fNode.Attributes["lastreplyid"].Value);



            fid = int.Parse(fNode.Attributes["forumid"].Value);
            //TODO: Validate can view
            //sForum = sForum.Replace("[FORUMNAME]", "<af:link id=""hypForumName" & fid & """ navigateurl=""" & Utilities.NavigateUrl(PageId, "", New String() {ParamKeys.ViewType & "=" & Views.Topics, ParamKeys.ForumId & "=" & fid}) & """ text=""" & forumname & """ runat=""server"" />") 'GetForumLink(forumname, PageId, True))
            sForum = sForum.Replace("[FORUMNAME]", "<af:link id=\"hypForumName" + fid + "\" navigateurl=\"" + URL.ForForum(PageId, fid, string.Empty, forumname) + "\" text=\"" + forumname + "\" runat=\"server\" />"); //GetForumLink(forumname, PageId, True))
            sForum = sForum.Replace("[FORUMNAMENOLINK]", forumname);
            sForum = sForum.Replace("[FORUMDESCRIPTION]", fNode.Attributes["desc"].Value);
            sForum = sForum.Replace("[TOTALTOPICS]", fNode.Attributes["totaltopics"].Value);
            sForum = sForum.Replace("[TOTALREPLIES]", fNode.Attributes["totalreplies"].Value);
            sForum = sForum.Replace("[DISPLAYNAME]", "<i class=\"fa fa-user fa-fw fa-blue\"></i>&nbsp;" + fNode.Attributes["lastpostauthorname"].Value);
            sForum = sForum.Replace("[LASTPOST]", "<asp:placeholder id=\"plhLastPost" + fid + "\" runat=\"server\">");
            sForum = sForum.Replace("[/LASTPOST]", "</asp:placeholder>");


            int intLength = 0;
            if ((sForum.IndexOf("[LASTPOSTSUBJECT:", 0) + 1) > 0)
            {
                int inStart = (sForum.IndexOf("[LASTPOSTSUBJECT:", 0) + 1) + 17;
                int inEnd = (sForum.IndexOf("]", inStart - 1) + 1);
                string sLength = sForum.Substring(inStart - 1, inEnd - inStart);
                intLength = Convert.ToInt32(sLength);
            }
            string ReplaceTag = "[LASTPOSTSUBJECT:" + intLength.ToString() + "]";
            string sSubject = fNode.Attributes["lastpostsubject"].Value;
            if (lastreplyid == 0)
            {
                lastreplyid = lasttopicid;
            }
            sSubject = GetLastPostSubject(lastreplyid, lasttopicid, fid, sSubject, intLength);

            sForum = sForum.Replace(ReplaceTag, sSubject);
            if (sSubject == string.Empty)
            {
                sForum = sForum.Replace("[RESX:BY]", string.Empty);
                sForum = sForum.Replace("[LASTPOSTDATE]", string.Empty);
            }
            else
            {
                sForum = sForum.Replace("[LASTPOSTDATE]", lastpostdate);
            }
            //TODO: Properly check "canview"
            string sIcon = TemplateUtils.ShowIcon(true, fid, UserId, DateTime.Parse(lastpostdate), lastReadDate, lastreplyid);
            string sIconImage = "<img alt=\"" + forumname + "\" src=\"" + ThemePath + "images/" + sIcon + "\" />";
            //sForum = sForum.Replace("[FORUMICON]", sIconImage);
            sForum = sForum.Replace("[FORUMICON]", "<div style=\"height:30px;margin=right:10px;\"><i class=\"fa fa-folder fa-2x fa-blue\"></i></div>");
            sForum = sForum.Replace("[CSS:FORUMICON]", "affoldernorm");
            sForum = sForum.Replace("[CSS:FORUMICONSM]", "affoldersmnorm");
            //sForum = sForum.Replace("[FORUMICONSM]", sIconImage.Replace("folder", "folder16"));
            sForum = sForum.Replace("[FORUMICONSM]", "");
            var xNodeList = ForumData.SelectNodes("//forums/forum[@active='true' and @parentforumid='" + fid + "']");
            string sSubs = string.Empty;
            if (xNodeList.Count > 0)
            {
                string sTemplate = TemplateUtils.GetTemplateSection(Template, "[SUBFORUMS]", "[/SUBFORUMS]");
                foreach (XmlNode n in xNodeList)
                {
                    sSubs += ParseForumRow(n, sTemplate, GroupName);
                }
            }
            sForum = TemplateUtils.ReplaceSubSection(sForum, sSubs, "[SUBFORUMS]", "[/SUBFORUMS]");
            string fc = "<af:forumrow id=\"ctlFR" + fid + "\" Hidden=\"" + hidden + "\" ForumId=\"" + fid + "\" ReadRoles=\"" + readRoles + "\" ViewRoles=\"" + viewRoles + "\" runat=\"server\">";
            fc += "<content>" + sForum + "</content>";
            fc += "</af:forumrow>";
            return fc;
        }
        private string GetForumLink(int ForumId, bool CanView)
        {
            return GetForumLink(string.Empty, ForumId, CanView);
        }
        private string GetForumLink(string Name, int ForumId, bool CanView)
        {
            string sOut;
            string[] Params = { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumId };
            if (CanView && Name != string.Empty)
            {
                sOut = "<a href=\"" + Utilities.NavigateUrl(PageId, "", new[] { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumId }) + "\">" + Name + "</a>";
            }
            else if (CanView && Name == string.Empty)
            {
                return Utilities.NavigateUrl(PageId, "", Params);
            }
            else
            {
                sOut = Name;
            }
            return sOut;
        }
        #endregion
        #region Public Methods
        private string GetLastPostSubject(int LastPostID, int ParentPostID, int fid, string Subject, int Length)
        {
            string sOut = string.Empty;
            int PostId = LastPostID;
            Subject = Utilities.StripHTMLTag(Subject);
            Subject = Subject.Replace("[", "&#91");
            Subject = Subject.Replace("]", "&#93");
            if (LastPostID != 0)
            {
                if (Subject.Length > Length)
                {
                    Subject = Subject.Substring(0, Length) + "...";
                }
                string sURL;
                if (ParentPostID == 0 || LastPostID == ParentPostID)
                {
                    sURL = Utilities.NavigateUrl(PageId, "", new[] { ParamKeys.ForumId + "=" + fid, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + PostId });
                }
                else
                {
                    sURL = Utilities.NavigateUrl(PageId, "", new[] { ParamKeys.ForumId + "=" + fid, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + ParentPostID, ParamKeys.ContentJumpId + "=" + PostId });

                }
                sOut = "<af:link id=\"hypLastPostSubject" + fid + "\" NavigateUrl=\"" + sURL + "\" Text=\"" + Utilities.HTMLEncode(Subject) + "\" runat=\"server\" />";
            }
            return sOut;
        }
        #endregion
    }
}


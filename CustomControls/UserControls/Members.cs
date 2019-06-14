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
    [DefaultProperty("Text"), ToolboxData("<{0}:Members runat=server></{0}:Members>")]
    public class Members : SettingsBase
    {
        #region Private Members
        private int _memberCount = 0;

        private int PageSize = 20;
        private int RowIndex = 0;
        private string Filter = "";
        #endregion
        #region Public Properties
        #endregion
        #region Protected Controls
        protected PlaceHolder plhContent = new PlaceHolder();
        protected global::DotNetNuke.Modules.ActiveForums.Controls.PagerNav Pager1;
        #endregion
        #region Event Handlers

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            this.AppRelativeVirtualPath = "~/";
            if (Request.Params["affilter"] != null)
            {
                Filter = Convert.ToString(Request.Params["affilter"]).Substring(0, 1);
            }
            else
            {
                Filter = string.Empty;
            }
        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                BuildControl();
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
            try
            {
                LinkControls(Controls);
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
            try
            {
                BuildPager();
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }

        }
        #endregion
        #region Private Methods
        private void BuildControl()
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
            string sTemplate = string.Empty;
            if (System.IO.File.Exists(Server.MapPath(ImagePath + "/_memberlist.txt")))
            {
                sTemplate = Utilities.GetFileContent(Server.MapPath(ImagePath + "/_memberlist.txt"));
            }
            else
            {
                sTemplate = Utilities.GetFileContent(Server.MapPath("~/DesktopModules/ActiveForums/config/templates/_memberlist.txt"));
            }


            if (!(sTemplate == string.Empty))
            {
                string sGrid = TemplateUtils.GetTemplateSection(sTemplate, "[AF:CONTROL:LIST]", "[/AF:CONTROL:LIST]");
                string sHeader = TemplateUtils.GetTemplateSection(sTemplate, "[AF:CONTROL:LIST:HEADER]", "[/AF:CONTROL:LIST:HEADER]");
                string sNormRow = TemplateUtils.GetTemplateSection(sTemplate, "[AF:CONTROL:LIST:ITEM]", "[/AF:CONTROL:LIST:ITEM]");
                string sAltRow = TemplateUtils.GetTemplateSection(sTemplate, "[AF:CONTROL:LIST:ALTITEM]", "[/AF:CONTROL:LIST:ALTITEM]");
                string sFooter = TemplateUtils.GetTemplateSection(sTemplate, "[AF:CONTROL:LIST:FOOTER]", "[/AF:CONTROL:LIST:FOOTER]");
                sGrid = TemplateUtils.ReplaceSubSection(sGrid, sHeader, "[AF:CONTROL:LIST:HEADER]", "[/AF:CONTROL:LIST:HEADER]");
                sGrid = TemplateUtils.ReplaceSubSection(sGrid, sFooter, "[AF:CONTROL:LIST:FOOTER]", "[/AF:CONTROL:LIST:FOOTER]");

                List<User> upl = GetMemberList();
                if (upl != null)
                {
                    int i = 0;
                    foreach (User up in upl)
                    {
                        string sRow = string.Empty;
                        if (i % 2 == 0)
                        {
                            sRow = sNormRow;
                        }
                        else
                        {
                            sRow = sAltRow;
                        }
                        sRow = TemplateUtils.ParseProfileTemplate(sRow, up, PortalId, ModuleId, ImagePath, CurrentUserType, TimeZoneOffset);
                        sb.Append(sRow);
                        i += 1;
                    }
                    sGrid = TemplateUtils.ReplaceSubSection(sGrid, sb.ToString(), "[AF:CONTROL:LIST:ITEM]", "[/AF:CONTROL:LIST:ALTITEM]");
                }
                sTemplate = TemplateUtils.ReplaceSubSection(sTemplate, sGrid, "[AF:CONTROL:LIST]", "[/AF:CONTROL:LIST]");
            }
            sTemplate = Globals.ControlRegisterTag + sTemplate;
            sTemplate = sTemplate.Replace("[AF:CONTROL:PAGER]", "<am:pagernav id=\"Pager1\" runat=\"server\" />");
            sTemplate = sTemplate.Replace("[AF:CONTROL:ALPHABAR]", BuildAlphaList());
            Control ctl = this.ParseControl(sTemplate);

            this.Controls.Add(ctl);
        }
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                switch (ctrl.ID)
                {
                    case "Pager1":
                        Pager1 = (DotNetNuke.Modules.ActiveForums.Controls.PagerNav)ctrl;
                        break;
                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }
        private void BuildPager()
        {
            int intPages = 0;
            intPages = Convert.ToInt32(System.Math.Ceiling(_memberCount / (double)PageSize));
            Pager1.PageCount = intPages;
            Pager1.CurrentPage = PageId;
            Pager1.TabID = TabId;
            Pager1.ForumID = -1;
            Pager1.PageText = Utilities.GetSharedResource("[RESX:Page]");
            Pager1.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
            Pager1.View = "members";
            if (UseAjax)
            {
                Pager1.PageMode = PagerNav.Mode.CallBack;
            }
            else
            {
                Pager1.PageMode = PagerNav.Mode.Links;
            }
            if (Request.Params["affilter"] != null)
            {
                string[] Params = { "affilter=" + Request.Params["affilter"] };
                Pager1.Params = Params;
            }
        }
        private string BuildAlphaList()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            LiteralControl litString = new LiteralControl();
            sb.Append("<div align=\"center\"><table width=\"90%\" cellpadding=\"0\" cellspacing=\"5\"><tr>");
            int i = 65;
            HyperLink hypAlpha = new HyperLink();
            for (i = 65; i <= 90; i++)
            {
                litString = new LiteralControl();
                sb.Append("<td align=\"center\">");
                char strChar = (char)(i);
                string[] Params = { ParamKeys.ViewType + "=members", "affilter=" + strChar };
                sb.Append("<a href=\"" + NavigateUrl(TabId, "", Params) + "\" class=\"CommandButton\">");
                sb.Append(strChar);
                sb.Append("</a></td>");
            }
            sb.Append("<td align=center>");
            sb.Append("<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ViewType + "=members") + "\" class=\"CommandButton\">");
            sb.Append(Utilities.GetSharedResource("[RESX:All]"));
            sb.Append("</a></td></tr></table></div>");
            return sb.ToString();
        }
        private List<User> GetMemberList()
        {
            List<User> upl = new List<User>();
            User upi = null;
            PageSize = MainSettings.PageSize;
            if (PageId == 1)
            {
                RowIndex = 0;
            }
            else
            {
                RowIndex = ((PageId * PageSize) - PageSize);
            }
            IDataReader dr = DataProvider.Instance().Profiles_MemberList(PortalId, ModuleId, PageSize, RowIndex, Filter);
            try
            {
                dr.Read();
                _memberCount = Convert.ToInt32(dr[0]);
                dr.NextResult();
                while (dr.Read())
                {
                    upi = new User();
                    upi.UserId = Convert.ToInt32(dr["UserId"].ToString());
                    upi.Profile.PortalId = Convert.ToInt32(dr["PortalId"].ToString());
                    upi.Profile.TopicCount = Convert.ToInt32(dr["TopicCount"].ToString());
                    upi.Profile.ReplyCount = Convert.ToInt32(dr["ReplyCount"].ToString());
                    upi.Profile.ViewCount = Convert.ToInt32(dr["ViewCount"].ToString());
                    upi.Profile.AnswerCount = Convert.ToInt32(dr["AnswerCount"].ToString());
                    upi.Profile.RewardPoints = Convert.ToInt32(dr["RewardPoints"].ToString());
                    upi.Profile.UserCaption = Convert.ToString(dr["UserCaption"].ToString());
                    upi.Profile.IsUserOnline = Convert.ToBoolean(dr["IsUserOnline"]);
                    if (!(dr["DateCreated"].ToString() == string.Empty))
                    {
                        upi.Profile.DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString());
                    }
                    if (!(dr["DateUpdated"].ToString() == string.Empty))
                    {
                        upi.Profile.DateUpdated = Convert.ToDateTime(dr["DateUpdated"].ToString());
                    }
                    if (!(dr["DateLastActivity"].ToString() == string.Empty))
                    {
                        upi.Profile.DateLastActivity = Convert.ToDateTime(dr["DateLastActivity"].ToString());
                    }
                    upi.Profile.Signature = Convert.ToString(dr["Signature"].ToString());
                    if (!(dr["SignatureDisabled"].ToString() == string.Empty))
                    {
                        upi.Profile.SignatureDisabled = Convert.ToBoolean(dr["SignatureDisabled"]);
                    }
                    if (!(dr["TrustLevel"].ToString() == string.Empty))
                    {
                        upi.Profile.TrustLevel = Convert.ToInt32(dr["TrustLevel"].ToString());
                    }
                    if (!(dr["AdminWatch"].ToString() == string.Empty))
                    {
                        upi.Profile.AdminWatch = Convert.ToBoolean(dr["AdminWatch"]);
                    }
                    if (!(dr["AttachDisabled"].ToString() == string.Empty))
                    {
                        upi.Profile.AttachDisabled = Convert.ToBoolean(dr["AttachDisabled"]);
                    }
                    upi.Profile.Avatar = Convert.ToString(dr["Avatar"].ToString());
                    if (!(dr["AvatarType"].ToString() == string.Empty))
                    {
                        upi.Profile.AvatarType = (AvatarTypes)Convert.ToInt32(dr["AvatarType"].ToString());
                    }
                    if (!(dr["AvatarDisabled"].ToString() == string.Empty))
                    {
                        upi.Profile.AvatarDisabled = Convert.ToBoolean(dr["AvatarDisabled"]);
                    }
                    upi.Profile.Yahoo = Convert.ToString(dr["Yahoo"].ToString());
                    upi.Profile.MSN = Convert.ToString(dr["MSN"].ToString());
                    upi.Profile.ICQ = Convert.ToString(dr["ICQ"].ToString());
                    upi.Profile.AOL = Convert.ToString(dr["AOL"].ToString());
                    upi.Profile.Occupation = Convert.ToString(dr["Occupation"].ToString());
                    upi.Profile.Location = Convert.ToString(dr["Location"].ToString());
                    upi.Profile.Interests = Convert.ToString(dr["Interests"].ToString());
                    upi.Profile.WebSite = Convert.ToString(dr["WebSite"].ToString());
                    upi.Profile.Badges = Convert.ToString(dr["Badges"].ToString());
                    upi.Profile.Bio = Convert.ToString(dr["Bio"].ToString());
                    if (!(dr["PrefBlockAvatars"].ToString() == string.Empty))
                    {
                        upi.Profile.PrefBlockAvatars = Convert.ToBoolean(dr["PrefBlockAvatars"]);
                    }
                    else
                    {
                        upi.Profile.PrefBlockAvatars = false;
                    }

                    if (!(dr["DateLastPost"].ToString() == string.Empty))
                    {
                        upi.Profile.DateLastPost = Convert.ToDateTime(dr["DateLastPost"].ToString());
                    }


                    upi.UserName = Convert.ToString(dr["Username"].ToString());
                    upi.FirstName = Convert.ToString(dr["FirstName"].ToString());
                    upi.LastName = Convert.ToString(dr["LastName"].ToString());
                    upi.Email = Convert.ToString(dr["Email"].ToString());
                    upi.DisplayName = Convert.ToString(dr["DisplayName"].ToString());




                    upl.Add(upi);
                }
            }
            catch (Exception ex)
            {
                dr.Close();
                dr = null;
            }
            finally
            {
                if (!dr.IsClosed)
                {
                    dr.Close();
                    dr = null;
                }
            }
            return upl;
        }

        #endregion
    }
}
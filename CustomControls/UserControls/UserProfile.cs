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
using System.Text.RegularExpressions;


namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:UserProfile runat=server></{0}:UserProfile>")]
    public class UserProfile : ProfileBase
    {
        #region Enum
        public enum ProfileModes : int
        {
            View,
            Edit
        }
        #endregion
        #region Private Members
        private ProfileModes _profileMode = ProfileModes.View;
        #endregion
        #region Public Properties
        public ProfileModes ProfileMode
        {
            get
            {
                return _profileMode;
            }
            set
            {
                _profileMode = value;
            }
        }
        #endregion
        #region Protected Members
        protected PlaceHolder plh = new PlaceHolder();
        protected PlaceHolder plhTabs = new PlaceHolder();
        protected PlaceHolder plhProfileAdminSettings = new PlaceHolder();
        protected PlaceHolder plhProfilePrefs = new PlaceHolder();
        protected PlaceHolder plhProfileUserAccount = new PlaceHolder();
        protected PlaceHolder plhTracker = new PlaceHolder();
        protected PlaceHolder plhProfileEditButton = new PlaceHolder();
        protected PlaceHolder plhProfileSaveButton = new PlaceHolder();
        protected PlaceHolder plhProfileCancelButton = new PlaceHolder();
        protected DotNetNuke.Modules.ActiveForums.Controls.ActiveTabs amTabs = new DotNetNuke.Modules.ActiveForums.Controls.ActiveTabs();
        //Edit Mode
        protected global::System.Web.UI.WebControls.TextBox txtWebSite;
        protected global::System.Web.UI.WebControls.TextBox txtOccupation;
        protected global::System.Web.UI.WebControls.TextBox txtLocation;
        protected global::System.Web.UI.WebControls.TextBox txtInterests;

        protected global::System.Web.UI.WebControls.TextBox txtYahoo;
        protected global::System.Web.UI.WebControls.TextBox txtMSN;
        protected global::System.Web.UI.WebControls.TextBox txtICQ;
        protected global::System.Web.UI.WebControls.TextBox txtAOL;
        protected global::System.Web.UI.WebControls.TextBox txtSignature;
        protected global::DotNetNuke.Modules.ActiveForums.Controls.ImageButton btnProfileEdit;
        protected global::DotNetNuke.Modules.ActiveForums.Controls.ImageButton btnProfileSave;
        protected global::DotNetNuke.Modules.ActiveForums.Controls.ImageButton btnProfileCancel;
        protected global::System.Web.UI.HtmlControls.HtmlTableRow trAvatarLinks;
        protected global::System.Web.UI.HtmlControls.HtmlTable tblAvatars;
        protected global::System.Web.UI.WebControls.Label lblAvatarError;

        #endregion
        #region Event Handlers

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            btnProfileEdit.Click += new System.EventHandler(btnProfileEdit_Click);
            btnProfileCancel.Click += new System.EventHandler(btnProfileCancel_Click);
            btnProfileSave.Click += new System.EventHandler(btnProfileSave_Click);

            this.AppRelativeVirtualPath = "~/";
            try
            {
                if (Request.QueryString["mode"] != null)
                {
                    if (Request.QueryString["mode"].ToLowerInvariant() == "edit" && CanEditMode())
                    {
                        ProfileMode = ProfileModes.Edit;
                    }
                    else
                    {
                        GoViewURL();
                    }
                }
                else
                {
                    ProfileMode = ProfileModes.View;
                }
            }
            catch (Exception ex)
            {
                ProfileMode = ProfileModes.View;
            }

        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            string sTemplate = string.Empty;
            if (System.IO.File.Exists(Server.MapPath(ImagePath + "/_userprofile.txt")))
            {
                sTemplate = Utilities.GetFileContent(Server.MapPath(ImagePath + "/_userprofile.txt"));
            }
            else
            {
                sTemplate = Utilities.GetFileContent(Server.MapPath("~/DesktopModules/ActiveForums/config/templates/_userprofile.txt"));
            }
            sTemplate = Utilities.ParseSpacer(sTemplate);


            if (ProfileMode == ProfileModes.Edit)
            {
                sTemplate = "<%@ Register TagPrefix=\"dnn\" Assembly=\"DotNetNuke\" Namespace=\"DotNetNuke.UI.WebControls\"%>" + sTemplate;
            }
            Literal lit = new Literal();
            UserController upc = new UserController();
            User up = upc.GetUser(PortalId, ModuleId, UID);
            ForumController fc = new ForumController();
            up.UserForums = fc.GetForumsForUser(up.UserRoles, PortalId, ForumModuleId, "CanRead");
            sTemplate = TemplateUtils.ParseProfileTemplate(sTemplate, up, PortalId, ModuleId, ImagePath, CurrentUserType, UserInfo.UserID, TimeZoneOffset);
            sTemplate = RenderModals(sTemplate);

            sTemplate = sTemplate.Replace("[AM:CONTROLS:AdminProfileSettings]", "<asp:placeholder id=\"plhProfileAdminSettings\" runat=\"server\" />");
            sTemplate = sTemplate.Replace("[AM:CONTROLS:ProfileMyPreferences]", "<asp:placeholder id=\"plhProfilePrefs\" runat=\"server\" />");
            sTemplate = sTemplate.Replace("[AM:CONTROLS:ProfileUserAccount]", "<asp:placeholder id=\"plhProfileUserAccount\" runat=\"server\" />");
            sTemplate = sTemplate.Replace("[AM:CONTROLS:ProfileForumTracker]", "<asp:placeholder id=\"plhTracker\" runat=\"server\" />");
            sTemplate = sTemplate.Replace("[AF:PROFILE:VIEWUSERPOSTS]", "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ViewType + "=search&uid=" + UID.ToString()) + "\">[RESX:ViewPostsByUser]</a>");



            sTemplate = GetTabsSection(sTemplate);
            Control ctl = this.ParseControl(sTemplate);
            this.Controls.Add(ctl);
            while (!(ctl.Controls.Count == 0))
            {
                this.Controls.Add(ctl.Controls[0]);
            }
            //Begin Load Tab Control
            plhTabs = (PlaceHolder)(this.FindControl("plhTabs"));
            if (plhTabs != null & amTabs != null)
            {
                plhTabs.Controls.Add(amTabs);
            }
            LinkControls(Controls);
            if (plhProfileEditButton != null)
            {
                btnProfileEdit = new ImageButton();
                btnProfileEdit.ID = "btnProfileEdit";
                btnProfileEdit.CssClass = "amimagebutton";
                btnProfileEdit.PostBack = true;
                btnProfileEdit.Height = 50;
                btnProfileEdit.Width = 50;
                btnProfileEdit.ImageLocation = "TOP";
                btnProfileEdit.Text = "[RESX:Button:Edit]";
                btnProfileEdit.ImageUrl = "~/DesktopModules/ActiveForums/images/edit32.png";
                btnProfileEdit.Visible = false;
                plhProfileEditButton.Controls.Add(btnProfileEdit);
            }
            if (plhProfileCancelButton != null)
            {
                btnProfileCancel = new ImageButton();
                btnProfileCancel.ID = "btnProfileCancel";
                btnProfileCancel.CssClass = "amimagebutton";
                btnProfileCancel.PostBack = true;
                btnProfileCancel.Height = 50;
                btnProfileCancel.Width = 50;
                btnProfileCancel.ImageLocation = "TOP";
                btnProfileCancel.Text = "[RESX:Button:Cancel]";
                btnProfileCancel.ImageUrl = "~/DesktopModules/ActiveForums/images/cancel32.png";
                btnProfileCancel.Visible = false;
                plhProfileCancelButton.Controls.Add(btnProfileCancel);
            }
            if (plhProfileSaveButton != null)
            {
                btnProfileSave = new ImageButton();
                btnProfileSave.ID = "btnProfileSave";
                btnProfileSave.CssClass = "amimagebutton";
                btnProfileSave.PostBack = true;
                btnProfileSave.Height = 50;
                btnProfileSave.Width = 50;
                btnProfileSave.ImageLocation = "TOP";
                btnProfileSave.Text = "[RESX:Button:Save]";
                btnProfileSave.ImageUrl = "~/DesktopModules/ActiveForums/images/save32.png";
                btnProfileSave.Visible = false;
                plhProfileSaveButton.Controls.Add(btnProfileSave);
            }

            if (plhProfileAdminSettings != null)
            {
                ProfileBase tmpCtl = (ProfileBase)(this.LoadControl("~/DesktopModules/ActiveForums/controls/profile_adminsettings.ascx"));
                tmpCtl.ModuleConfiguration = this.ModuleConfiguration;
                tmpCtl.UserProfile = up.Profile;
                plhProfileAdminSettings.Controls.Add(tmpCtl);
            }
            if (plhProfilePrefs != null)
            {
                ProfileBase tmpCtl = (ProfileBase)(this.LoadControl("~/DesktopModules/ActiveForums/controls/profile_mypreferences.ascx"));
                tmpCtl.ModuleConfiguration = this.ModuleConfiguration;
                tmpCtl.UserProfile = up.Profile;
                plhProfilePrefs.Controls.Add(tmpCtl);
            }
            if (plhProfileUserAccount != null)
            {
                ProfileBase tmpCtl = (ProfileBase)(this.LoadControl("~/DesktopModules/ActiveForums/controls/profile_useraccount.ascx"));
                tmpCtl.ModuleConfiguration = this.ModuleConfiguration;
                tmpCtl.UserProfile = up.Profile;
                plhProfileUserAccount.Controls.Add(tmpCtl);
            }
            if (plhTracker != null)
            {
                ForumView ctlForums = new ForumView();
                ctlForums.ModuleConfiguration = this.ModuleConfiguration;
                ctlForums.DisplayTemplate = DataCache.GetTemplate("ForumTracking.txt");
                ctlForums.CurrentUserId = UID;
                ctlForums.ForumIds = up.UserForums;
                plhTracker.Controls.Add(ctlForums);
            }
            if (btnProfileEdit != null)
            {
                if (!(CurrentUserType == CurrentUserTypes.Anon) && (UID == this.UserId || (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser)))
                {
                    if (ProfileMode == ProfileModes.View)
                    {
                        btnProfileEdit.Visible = true;
                        btnProfileCancel.Visible = false;
                        btnProfileSave.Visible = false;
                    }
                    else
                    {
                        btnProfileEdit.Visible = false;
                        btnProfileCancel.Visible = true;
                        btnProfileSave.Visible = true;
                    }
                }
            }

        }

        private void btnProfileEdit_Click(object sender, System.EventArgs e)
        {
            if (!(CurrentUserType == CurrentUserTypes.Anon) && (UID == this.UserId || (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser)))
            {
                Response.Redirect(NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=profile", "uid=" + UID, "mode=edit" }));
            }
        }
        private void btnProfileCancel_Click(object sender, System.EventArgs e)
        {
            GoViewURL();
        }
        private void btnProfileSave_Click(object sender, System.EventArgs e)
        {
            if (SaveProfile())
            {
                GoViewURL();
            }
        }
        #endregion
        #region Private Methods
        private void GoViewURL()
        {
            Response.Redirect(NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=profile", "uid=" + UID }));
        }
        private bool CanEditMode()
        {
            if (!(CurrentUserType == CurrentUserTypes.Anon) && (UID == this.UserId || (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                switch (ctrl.ID)
                {
                    case "plhProfileAdminSettings":
                        plhProfileAdminSettings = (PlaceHolder)ctrl;
                        break;
                    case "plhProfilePrefs":
                        plhProfilePrefs = (PlaceHolder)ctrl;
                        break;
                    case "plhProfileUserAccount":
                        plhProfileUserAccount = (PlaceHolder)ctrl;
                        break;
                    case "plhTracker":
                        plhTracker = (PlaceHolder)ctrl;
                        break;
                    case "plhProfileEditButton":
                        plhProfileEditButton = (PlaceHolder)ctrl;
                        break;
                    case "plhProfileSaveButton":
                        plhProfileSaveButton = (PlaceHolder)ctrl;
                        break;
                    case "plhProfileCancelButton":
                        plhProfileCancelButton = (PlaceHolder)ctrl;
                        break;
                    case "txtWebSite":
                        txtWebSite = (TextBox)ctrl;
                        break;
                    case "txtOccupation":
                        txtOccupation = (TextBox)ctrl;
                        break;
                    case "txtLocation":
                        txtLocation = (TextBox)ctrl;
                        break;
                    case "txtInterests":
                        txtInterests = (TextBox)ctrl;

                        break;
                    case "trAvatarLinks":
                        trAvatarLinks = (System.Web.UI.HtmlControls.HtmlTableRow)ctrl;
                        break;
                    case "tblAvatars":
                        tblAvatars = (System.Web.UI.HtmlControls.HtmlTable)ctrl;
                        break;
                    case "txtYahoo":
                        txtYahoo = (TextBox)ctrl;
                        break;
                    case "txtMSN":
                        txtMSN = (TextBox)ctrl;
                        break;
                    case "txtICQ":
                        txtICQ = (TextBox)ctrl;
                        break;
                    case "txtAOL":
                        txtAOL = (TextBox)ctrl;
                        break;
                    case "txtSignature":
                        txtSignature = (TextBox)ctrl;
                        break;
                    case "btnProfileEdit":
                        btnProfileEdit = (ImageButton)ctrl;
                        break;
                    case "btnProfileCancel":
                        btnProfileCancel = (ImageButton)ctrl;
                        break;
                    case "btnProfileSave":
                        btnProfileSave = (ImageButton)ctrl;
                        break;
                    case "lblAvatarError":
                        lblAvatarError = (Label)ctrl;
                        break;
                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }
        private bool SaveProfile()
        {
            if (CanEditMode())
            {
                DotNetNuke.Entities.Users.UserInfo objuser = DotNetNuke.Entities.Users.UserController.GetUserById(PortalId, UID);
                UserProfileController upc = new UserProfileController();
                UserController uc = new UserController();
                UserProfileInfo upi = uc.GetUser(PortalId, ModuleId, UID).Profile;
                if (upi != null)
                {
                    upi.WebSite = Utilities.XSSFilter(txtWebSite.Text, true);
                    upi.Occupation = Utilities.XSSFilter(txtOccupation.Text, true);
                    upi.Location = Utilities.XSSFilter(txtLocation.Text, true);
                    upi.Interests = Utilities.XSSFilter(txtInterests.Text, true);
                    upi.Yahoo = Utilities.XSSFilter(txtYahoo.Text, true);
                    upi.MSN = Utilities.XSSFilter(txtMSN.Text, true);
                    upi.ICQ = Utilities.XSSFilter(txtICQ.Text, true);
                    upi.AOL = Utilities.XSSFilter(txtAOL.Text, true);
                    if (MainSettings.AllowSignatures == 1)
                    {
                        upi.Signature = Utilities.XSSFilter(txtSignature.Text, true);
                        upi.Signature = Utilities.StripHTMLTag(upi.Signature);
                        upi.Signature = Utilities.HTMLEncode(upi.Signature);
                    }
                    else if (MainSettings.AllowSignatures == 2)
                    {
                        upi.Signature = Utilities.XSSFilter(txtSignature.Text, false);
                    }



                    upc.Profiles_Save(upi);
                    bool blnSaveProfile = false;

                    DotNetNuke.Entities.Profile.ProfilePropertyDefinitionCollection profproperties = new DotNetNuke.Entities.Profile.ProfilePropertyDefinitionCollection();
                    profproperties = objuser.Profile.ProfileProperties;
                    foreach (DotNetNuke.Entities.Profile.ProfilePropertyDefinition profprop in profproperties)
                    {
                        Control ctl = RecursiveFind(this, "dnnctl" + profprop.PropertyName);
                        if (ctl != null)
                        {
                            if (ctl is TextBox)
                            {
                                TextBox txt = (TextBox)ctl;
                                if (txt.ID.Contains("dnnctl"))
                                {
                                    blnSaveProfile = true;
                                    string propName = txt.ID.Replace("dnnctl", string.Empty);
                                    objuser.Profile.GetProperty(propName).PropertyValue = txt.Text;
                                }

                            }
                        }
                    }

                    if (blnSaveProfile)
                    {
                        DotNetNuke.Entities.Users.UserController.UpdateUser(PortalId, objuser);
                    }
                }
            }
            return true;
        }
        private Control RecursiveFind(Control ctl, string controlId)
        {
            if (ctl.ID == controlId)
            {
                return ctl;
            }
            foreach (Control tmpctl in ctl.Controls)
            {
                Control t = RecursiveFind(tmpctl, controlId);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }


        private string RenderModals(string Template)
        {
            string sOut = Template;
            //[AM:CONTROLS:MODAL:MyPreferences:Private]
            string pattern = "(\\[AM:CONTROLS:MODAL:(.+?)\\])";
            Regex regExp = new Regex(pattern);
            MatchCollection matches = null;
            matches = regExp.Matches(sOut);
            bool bAddModal = false;
            foreach (Match match in matches)
            {
                string matchValue = match.Groups[2].Value;
                if (matchValue.Contains(":"))
                {
                    string sec = matchValue.Split(':')[1].ToLowerInvariant();
                    switch (sec.ToLowerInvariant())
                    {
                        case "anon":
                            bAddModal = true;
                            break;
                        case "private":
                            if (!(CurrentUserType == CurrentUserTypes.Anon))
                            {
                                if (UserId == UID || (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser))
                                {
                                    bAddModal = true;
                                }
                            }
                            break;
                        case "auth":
                            if (!(CurrentUserType == CurrentUserTypes.Anon))
                            {
                                bAddModal = true;
                            }
                            break;
                        case "forummod":
                            if (!(CurrentUserType == CurrentUserTypes.Anon) && !(CurrentUserType == CurrentUserTypes.Auth))
                            {
                                bAddModal = true;
                            }
                            break;
                        case "admin":
                            if (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser)
                            {
                                bAddModal = true;
                            }
                            break;
                        case "superuser":
                            if (CurrentUserType == CurrentUserTypes.SuperUser)
                            {
                                bAddModal = true;
                            }
                            break;
                        default:
                            bAddModal = UserInfo.IsInRole(sec);
                            break;
                    }
                    matchValue = matchValue.Split(':')[0];
                }
                else
                {
                    bAddModal = true;
                }
                if (bAddModal == true)
                {
                    string sModalDivId = "afmodal" + matchValue;
                    string sModalText = "[RESX:Label:" + matchValue + "]";
                    string sModalContent = string.Empty;
                    Template = Template.Replace(match.Value, "<a href=\"javascript:void(0);\" onclick=\"amOpenModal('" + sModalDivId + "','" + sModalText + "',350, 300);\">" + sModalText + "</a>" + match.Value);
                    if (sOut.Contains(match.Value.Replace("[AM", "[/AM")))
                    {
                        string tmp = TemplateUtils.GetTemplateSection(sOut, match.Value, match.Value.Replace("[AM", "[/AM"));
                        sModalContent = "<div id=\"" + sModalDivId + "\" style=\"display:none;\">" + tmp + "</div>";
                    }
                    Template = TemplateUtils.ReplaceSubSection(Template, string.Empty, match.Value, match.Value.Replace("[AM", "[/AM"));
                    Template = Template + sModalContent;
                }

            }
            return Template;
        }

        private string GetTabsSection(string Template)
        {
            string sOut = string.Empty;
            sOut = TemplateUtils.GetTemplateSection(Template, "[AM:CONTROLS:TABS]", "[/AM:CONTROLS:TABS]");
            string pattern = "(\\[AM:CONTROLS:TAB:(.+?)\\])";
            Regex regExp = new Regex(pattern);
            MatchCollection matches = null;
            matches = regExp.Matches(sOut);
            amTabs = new DotNetNuke.Modules.ActiveForums.Controls.ActiveTabs();
            foreach (Match match in matches)
            {
                bool bAddTab = false;
                string matchValue = match.Groups[2].Value;
                CurrentUserTypes access = CurrentUserTypes.Anon;
                if (matchValue.Contains(":"))
                {
                    string sec = matchValue.Split(':')[1].ToLowerInvariant();
                    if (sec == "private")
                    {
                        if (!(CurrentUserType == CurrentUserTypes.Anon))
                        {
                            if (UserId == UID || (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser))
                            {
                                bAddTab = true;
                                access = CurrentUserTypes.Admin;
                            }
                        }
                    }
                    else
                    {
                        switch (sec)
                        {
                            case "auth":
                                access = CurrentUserTypes.Auth;
                                break;
                            case "forummod":
                                access = CurrentUserTypes.ForumMod;
                                break;
                            case "admin":
                                access = CurrentUserTypes.Admin;
                                break;
                            case "superuser":
                                access = CurrentUserTypes.SuperUser;
                                break;
                        }
                        switch (access)
                        {
                            case CurrentUserTypes.Anon:
                                bAddTab = true;
                                break;
                            case CurrentUserTypes.Auth:
                                if (!(CurrentUserType == CurrentUserTypes.Anon))
                                {
                                    bAddTab = true;
                                }
                                break;
                            case CurrentUserTypes.ForumMod:
                                if (!(CurrentUserType == CurrentUserTypes.Anon) && !(CurrentUserType == CurrentUserTypes.Auth))
                                {
                                    bAddTab = true;
                                }
                                break;
                            case CurrentUserTypes.Admin:
                                if (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser)
                                {
                                    bAddTab = true;
                                }
                                break;
                            case CurrentUserTypes.SuperUser:
                                if (CurrentUserType == CurrentUserTypes.SuperUser)
                                {
                                    bAddTab = true;
                                }
                                break;
                        }
                    }
                    matchValue = matchValue.Split(':')[0];
                }
                else
                {
                    bAddTab = true;
                }


                if (bAddTab)
                {
                    DotNetNuke.Modules.ActiveForums.Controls.Tab tb = new DotNetNuke.Modules.ActiveForums.Controls.Tab();
                    tb.ControlKey = matchValue;
                    tb.Text = "[RESX:Label:" + matchValue + "]";
                    if (sOut.Contains(match.Value.Replace("[AM", "[/AM")))
                    {
                        DotNetNuke.Modules.ActiveForums.Controls.TabContent tbc = new DotNetNuke.Modules.ActiveForums.Controls.TabContent();
                        string tmp = TemplateUtils.GetTemplateSection(sOut, match.Value, match.Value.Replace("[AM", "[/AM"));
                        if (tmp.Contains("<dnn:"))
                        {
                            tmp = "<%@ Register TagPrefix=\"dnn\" Assembly=\"DotNetNuke\" Namespace=\"DotNetNuke.UI.WebControls\"%>" + tmp;
                        }
                        if (tmp.Contains("<social:"))
                        {
                            tmp = Globals.SocialRegisterTag + tmp;
                        }
                        Control ctl = this.ParseControl(tmp);
                        tbc.Controls.Add(ctl);
                        tb.Content = tbc;
                    }
                    amTabs.Tabs.Add(tb);
                }

            }
            Template = TemplateUtils.ReplaceSubSection(Template, "<asp:placeholder id=\"plhTabs\" runat=\"server\" />", "[AM:CONTROLS:TABS]", "[/AM:CONTROLS:TABS]");
            return Template;
        }
        #endregion
    }
}
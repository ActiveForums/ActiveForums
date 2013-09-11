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

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:QuickReply runat=server></{0}:QuickReply>")]
    public class QuickReply : ControlsBase
    {
        #region Protected Members
        protected Controls.Toggle tgQR;
        protected Controls.ToggleDisplay tgdQR;
        protected PlaceHolder plhMessage;
        protected System.Web.UI.WebControls.RequiredFieldValidator reqUserName;
        protected System.Web.UI.HtmlControls.HtmlInputText txtUserName;
        protected Label reqBody;
        protected Label reqSecurityCode;
        protected CheckBox chkSubscribe;
        protected Controls.ImageButton ambtnSubmit;
        protected System.Web.UI.HtmlControls.HtmlInputButton btnSubmit;
        protected System.Web.UI.HtmlControls.HtmlTableRow trUsername;
        protected System.Web.UI.HtmlControls.HtmlTableRow trCaptcha;
        protected System.Web.UI.HtmlControls.HtmlTableRow trSubscribe;

        #endregion
        #region Private Members
        private int InstanceId = -1;
        private int SiteId = -1;
        private string _Subject = string.Empty;
        #endregion
        #region Public Properties
        public string Subject
        {
            get
            {
                return _Subject;
            }
            set
            {
                _Subject = value;
            }
        }
        #endregion
        #region Event Handlers
        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            ambtnSubmit.Click += new System.EventHandler(ambtnSubmit_Click);

            InstanceId = ControlConfig.InstanceId;
            SiteId = ControlConfig.SiteId;
            ForumController fc = new ForumController();
            if (ForumInfo == null)
            {
                ForumInfo = fc.Forums_Get(SiteId, InstanceId, ForumId, this.UserId, true, false, TopicId);
            }

            SettingsInfo MainSettings = DataCache.MainSettings(ControlConfig.InstanceId);
            string sTemp = string.Empty;
            if (ControlConfig != null)
            {
                object obj = DataCache.CacheRetrieve(InstanceId + "qr");
                if (obj == null)
                {
                    sTemp = ParseTemplate();
                }
                else
                {
                    sTemp = Convert.ToString(obj);
                }
                sTemp = Utilities.LocalizeControl(sTemp);
                string SubscribedChecked = string.Empty;
                if (ControlConfig.User.PrefTopicSubscribe || Subscriptions.IsSubscribed(SiteId, InstanceId, ForumId, TopicId, SubscriptionTypes.Instant, this.UserId))
                {
                    SubscribedChecked = " checked=true";
                }
                sTemp = sTemp.Replace("[AF:CONTROL:SUBSCRIBECHECK]", "<input type=\"checkbox\" id=\"chkSubscribe\" name=\"chkSubscribe\" value=\"1\" " + SubscribedChecked + "\" />");
                //Security
                sTemp = sTemp.Replace("[CREATEROLES]", "1;");
                sTemp = sTemp.Replace("[USERROLES]", ForumUser.UserRoles);
                sTemp = sTemp.Replace("[THEMEPATH]", ThemePath);
                sTemp = sTemp.Replace("[SUBJECT]", Subject);
                sTemp = sTemp.Replace("[REPLYROLES]", ForumInfo.Security.Reply);
                if (!HttpContext.Current.Request.IsAuthenticated)
                {
                    sTemp = "<%@ Register TagPrefix=\"dnn\" Assembly=\"DotNetNuke\" Namespace=\"DotNetNuke.UI.WebControls\"%>" + sTemp;
                }
                if (!(sTemp.Contains(Globals.ControlRegisterAFTag)))
                {
                    sTemp = Globals.ControlRegisterAFTag + sTemp;
                }
                Control ctl = Page.ParseControl(sTemp);
                LinkControls(ctl.Controls);
                this.Controls.Add(ctl);
            }
        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                bool bolShow = true;
                if (HttpContext.Current.Request.Cookies["QRShow"] != null)
                {
                    bolShow = Convert.ToBoolean(HttpContext.Current.Request.Cookies["QRShow"].Value);
                }
                tgdQR.Visible = bolShow;
                tgQR.Visible = bolShow;
                if (!HttpContext.Current.Request.Browser.IsMobileDevice)
                {
                    ambtnSubmit.ImageUrl = ThemePath + "save32.png";
                    ambtnSubmit.ImageLocation = "TOP";
                    btnSubmit.Visible = false;
                    ambtnSubmit.Visible = true;
                    if (!HttpContext.Current.Request.IsAuthenticated)
                    {
                        trSubscribe.Visible = false;
                        ambtnSubmit.PostBack = true;
                        ambtnSubmit.ClientSideScript = "";
                        reqUserName.Enabled = true;
                        reqUserName.Text = "<img src=\"" + ThemePath + "warning.png\" />";
                        reqBody.Text = "<img src=\"" + ThemePath + "warning.png\" />";
                        reqSecurityCode.Text = "<img src=\"" + ThemePath + "warning.png\" />";
                        ambtnSubmit.Click += ambtnSubmit_Click;
                    }
                    else
                    {
                        trUsername.Visible = false;
                        reqUserName.Visible = false;
                        reqUserName.Enabled = false;
                        txtUserName.Visible = false;
                        trCaptcha.Visible = false;


                    }
                }
                else
                {
                    btnSubmit.Visible = true;
                    ambtnSubmit.Visible = false;
                }
                if (!(string.IsNullOrEmpty(HttpContext.Current.Request.Form["txtBody"])) && HttpContext.Current.Request.IsAuthenticated & ((!(string.IsNullOrEmpty(HttpContext.Current.Request.Form["hidReply1"])) && string.IsNullOrEmpty(HttpContext.Current.Request.Form["hidReply2"])) | HttpContext.Current.Request.Browser.IsMobileDevice))
                {
                    SaveQuickReply();
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void ambtnSubmit_Click(object sender, System.EventArgs e)
        {

        }
        #endregion
        #region Private Methods
        private void SaveQuickReply()
        {
            int iFloodInterval = MainSettings.FloodInterval;
            if (iFloodInterval > 0)
            {
                UserProfileController upc = new UserProfileController();
                UserProfileInfo upi = upc.Profiles_Get(SiteId, InstanceId, this.UserId);
                if (upi != null)
                {
                    if (SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Second, upi.DateLastPost, DateTime.Now) < iFloodInterval)
                    {
                        Controls.InfoMessage im = new Controls.InfoMessage();
                        im.Message = "<div class=\"afmessage\">" + string.Format(Utilities.GetSharedResource("[RESX:Error:FloodControl]"), iFloodInterval) + "</div>";
                        plhMessage.Controls.Add(im);
                        return;
                    }
                }
            }
            //TODO: Fix for anon
            //If Not Current.Request.IsAuthenticated Then
            //    If Not ctlCaptcha.IsValid Or txtUserName.Value = "" Then
            //        Exit Sub
            //    End If
            //End If
            //Dim ui As New UserProfileInfo
            //If UserId > 0 Then
            //    Dim upc As New UserProfileController
            //    ui = upc.Profiles_Get(PortalId, ForumModuleId, UserId)
            //Else
            //    ui.TopicCount = 0
            //    ui.ReplyCount = 0
            //    ui.RewardPoints = 0
            //    ui.IsMod = False
            //    ui.TrustLevel = -1

            //End If
            ForumController fc = new ForumController();
            Forum forumInfo = fc.Forums_Get(SiteId, InstanceId, ForumId, this.UserId, true, false, TopicId);
            bool UserIsTrusted = false;
            UserIsTrusted = Utilities.IsTrusted((int)forumInfo.DefaultTrustValue, ControlConfig.User.TrustLevel, Permissions.HasPerm(forumInfo.Security.Trust, ForumUser.UserRoles), forumInfo.AutoTrustLevel, ControlConfig.User.PostCount);
            bool isApproved = false;
            isApproved = Convert.ToBoolean(((forumInfo.IsModerated == true) ? false : true));
            if (UserIsTrusted || Permissions.HasPerm(forumInfo.Security.ModApprove, ForumUser.UserRoles))
            {
                isApproved = true;
            }
            ReplyInfo ri = new ReplyInfo();
            Data.Topics db = new Data.Topics();
            //im rc As New ReplyController
            int ReplyId = -1;
            string sUsername = string.Empty;
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                sUsername = ControlConfig.User.DisplayName;
            }
            else
            {
                sUsername = Utilities.CleanString(SiteId, txtUserName.Value, false, EditorTypes.TEXTBOX, true, false, InstanceId, ThemePath, false);
            }

            string sBody = string.Empty;
            //TODO: Check for allowhtml
            bool allowHtml = false;
            //If forumInfo.AllowHTML Then
            //    allowHtml = isHTMLPermitted(forumInfo.EditorPermittedUsers, IsTrusted, forumInfo.Security.ModEdit)
            //End If
            sBody = Utilities.CleanString(SiteId, HttpContext.Current.Request.Form["txtBody"], allowHtml, EditorTypes.TEXTBOX, forumInfo.UseFilter, forumInfo.AllowScript, InstanceId, ThemePath, forumInfo.AllowEmoticons);
            DateTime createDate = DateTime.Now;
            ri.TopicId = TopicId;
            ri.ReplyToId = TopicId;
            ri.Content.AuthorId = UserId;
            ri.Content.AuthorName = sUsername;
            ri.Content.Body = sBody;
            ri.Content.DateCreated = createDate;
            ri.Content.DateUpdated = createDate;
            ri.Content.IsDeleted = false;
            ri.Content.Subject = Subject;
            ri.Content.Summary = string.Empty;
            ri.IsApproved = isApproved;
            ri.IsDeleted = false;
            ri.Content.IPAddress = HttpContext.Current.Request.UserHostAddress;
            ReplyId = db.Reply_Save(ri);
            //Check if is subscribed
            if (HttpContext.Current.Request.Params["chkSubscribe"] != null)
            {
                if (HttpContext.Current.Request.Params["chkSubscribe"] == "1" && UserId > 0)
                {
                    if (!(Subscriptions.IsSubscribed(SiteId, InstanceId, ForumId, TopicId, SubscriptionTypes.Instant, UserId)))
                    {
                        //TODO: Fix Subscriptions
                        //Dim sc As New Data.Tracking
                        //sc.Subscription_Update(SiteId, InstanceId, ForumId, TopicId, 1, UserId)
                    }
                }
            }
            if (isApproved)
            {
                //Send Subscriptions
                try
                {
                    string sURL = Utilities.NavigateUrl(PageId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId });
                    Subscriptions.SendSubscriptions(SiteId, InstanceId, PageId, ForumId, TopicId, ReplyId, UserId);

#if !SKU_LITE
                    try
                    {
                        Social oSocial = new Social();
                        oSocial.AddForumItemToJournal(SiteId, InstanceId, UserId, "forumreply", sURL, Subject, sBody);
                    }
                    catch (Exception ex)
                    {
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                    }

#endif
                }
                catch (Exception ex)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
                }
                //Redirect to show post
                string fullURL = Utilities.NavigateUrl(PageId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId });
                HttpContext.Current.Response.Redirect(fullURL, false);
            }
            else if (isApproved == false)
            {
                Email oEmail = new Email();
                oEmail.SendEmailToModerators(forumInfo.ModNotifyTemplateId, SiteId, ForumId, ri.TopicId, ReplyId, InstanceId, PageId, string.Empty);
                string[] Params = { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=confirmaction", "afmsg=pendingmod", ParamKeys.TopicId + "=" + TopicId };
                HttpContext.Current.Response.Redirect(Utilities.NavigateUrl(PageId, "", Params), false);
            }
            else
            {
                string fullURL = Utilities.NavigateUrl(PageId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId });

#if !SKU_LITE
                try
                {
                    Modules.ActiveForums.Social oSocial = new Modules.ActiveForums.Social();
                    oSocial.AddForumItemToJournal(SiteId, InstanceId, UserId, "forumreply", fullURL, Subject, sBody);
                }
                catch (Exception ex)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                }

#endif
                HttpContext.Current.Response.Redirect(fullURL, false);
            }

            //End If


        }
        private string ParseTemplate()
        {
            string sOut = DisplayTemplate;

            DataCache.CacheStore(InstanceId + "qr", sOut);
            return sOut;
        }
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                switch (ctrl.ID)
                {
                    case "tgQR":
                        tgQR = (Toggle)ctrl;
                        tgQR.ImagePath = ThemePath;
                        break;
                    case "tgdQR":
                        tgdQR = (ToggleDisplay)ctrl;
                        break;
                    case "plhMessage":
                        plhMessage = (PlaceHolder)ctrl;
                        break;
                    case "reqUserName":
                        reqUserName = (System.Web.UI.WebControls.RequiredFieldValidator)ctrl;
                        break;
                    case "txtUserName":
                        txtUserName = (System.Web.UI.HtmlControls.HtmlInputText)ctrl;
                        break;
                    case "reqBody":
                        reqBody = (Label)ctrl;
                        break;
                    case "reqSecurityCode":
                        reqSecurityCode = (Label)ctrl;
                        break;
                    case "chkSubscribe":
                        chkSubscribe = (CheckBox)ctrl;
                        break;
                    case "ambtnSubmit":
                        ambtnSubmit = (ImageButton)ctrl;
                        break;
                    case "btnSubmit":
                        btnSubmit = (System.Web.UI.HtmlControls.HtmlInputButton)ctrl;
                        break;
                    case "trUsername":
                        trUsername = (System.Web.UI.HtmlControls.HtmlTableRow)ctrl;
                        break;
                    case "trCaptcha":
                        trCaptcha = (System.Web.UI.HtmlControls.HtmlTableRow)ctrl;
                        break;
                    case "trSubscribe":
                        trSubscribe = (System.Web.UI.HtmlControls.HtmlTableRow)ctrl;
                        break;
                }
                if (ctrl is Controls.ControlsBase)
                {
                    ((Controls.ControlsBase)ctrl).ControlConfig = this.ControlConfig;
                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }
        #endregion
    }

}

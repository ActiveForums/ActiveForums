using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DotNetNuke;
using System.IO;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Services.Localization;
using DotNetNuke.Framework;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_post : ForumBase
    {
        protected Controls.SubmitForm ctlForm = new Controls.SubmitForm();
        private bool isApproved = false;
        public string spinner;
        public string EditorClientId;
        public string PreviewText = "";
        private bool isEdit = false;
        private Forum fi;
        private UserProfileInfo ui = new UserProfileInfo();
        private string MyThemePath = string.Empty;
        private string sPostBack = string.Empty;
        private bool UserIsTrusted = false;
        private int _contentId = -1;
        private int _authorId = -1;
        private bool _allowHTML = false;
        private EditorTypes _editorType = EditorTypes.TEXTBOX;
        private bool canModEdit = false;
        private bool canModApprove = false;
        private bool canEdit = false;
        private bool canAttach = false;
        private bool canTrust = false;
        private bool canLock = false;
        private bool canPin = false;
        private bool canAnnounce = false;



        #region Event Handlers

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            System.Web.UI.HtmlControls.HtmlGenericControl oLink = new System.Web.UI.HtmlControls.HtmlGenericControl("link");
            oLink.Attributes["rel"] = "stylesheet";
            oLink.Attributes["type"] = "text/css";
            oLink.Attributes["href"] = Page.ResolveUrl("~/DesktopModules/ActiveForums/scripts/calendar.css");
            System.Web.UI.Control oCSS = this.Page.FindControl("CSS");
            if (oCSS != null)
            {
                int iControlIndex = 0;
                iControlIndex = oCSS.Controls.Count;
                oCSS.Controls.Add(oLink);
            }

            fi = this.ForumInfo;
            _authorId = UserId;
            canModEdit = Permissions.HasPerm(fi.Security.ModEdit, ForumUser.UserRoles);
            canModApprove = Permissions.HasPerm(fi.Security.ModApprove, ForumUser.UserRoles);
            canEdit = Permissions.HasPerm(fi.Security.Edit, ForumUser.UserRoles);
            //CanReply = Permissions.HasPerm(fi.Security.Reply, ForumUser.UserRoles)
            //CanCreate = Permissions.HasPerm(fi.Security.Create, ForumUser.UserRoles)
            canAttach = Permissions.HasPerm(fi.Security.Attach, ForumUser.UserRoles);
            canTrust = Permissions.HasPerm(fi.Security.Trust, ForumUser.UserRoles);
            canLock = Permissions.HasPerm(fi.Security.Lock, ForumUser.UserRoles);
            canPin = Permissions.HasPerm(fi.Security.Pin, ForumUser.UserRoles);
            canAnnounce = Permissions.HasPerm(fi.Security.Announce, ForumUser.UserRoles);
            if (fi == null)
            {
                Response.Redirect(NavigateUrl(ForumTabId));
            }
            else if (Request.Params["action"] != null)
            {
                if (!canEdit && (Request.Params["action"].ToLowerInvariant() == "te" || Request.Params["action"].ToLowerInvariant() == "re"))
                {
                    Response.Redirect(NavigateUrl(ForumTabId));
                }
            }
            if (CanCreate == false && CanReply == false)
            {
                Response.Redirect(NavigateUrl(ForumTabId, "", "ctl=login") + "?returnurl=" + Server.UrlEncode(Request.RawUrl));
            }

            if (UserId > 0)
            {
                ui = ForumUser.Profile;
            }
            else
            {
                ui.TopicCount = 0;
                ui.ReplyCount = 0;
                ui.RewardPoints = 0;
                ui.IsMod = false;
                ui.TrustLevel = -1;

            }
            UserIsTrusted = Utilities.IsTrusted((int)fi.DefaultTrustValue, ui.TrustLevel, canTrust, fi.AutoTrustLevel, ui.PostCount);
            spinner = Page.ResolveUrl("~/DesktopModules/activeforums/themes/" + MainSettings.Theme + "/images/loading.gif");
            isApproved = Convert.ToBoolean(((fi.IsModerated == true) ? false : true));
            if (UserIsTrusted || canModApprove)
            {
                isApproved = true;
            }
            string MyTheme = MainSettings.Theme;
            MyThemePath = Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MyTheme);
            ctlForm.ID = "ctlForm";
            ctlForm.PostButton.ImageUrl = MyThemePath + "/images/save32.png";
            ctlForm.PostButton.ImageLocation = "TOP";
            ctlForm.PostButton.Height = Unit.Pixel(50);
            ctlForm.PostButton.Width = Unit.Pixel(50);
            if (canAttach && fi.AllowAttach)
            {
                ctlForm.PostButton.ClientSideScript = "af_checkupload();";
                sPostBack = "GET"; //Page.ClientScript.GetPostBackEventReference(ctlForm.SubmitButton, String.Empty)
                ctlForm.PostButton.PostBack = false;
            }
            else
            {
                ctlForm.PostButton.ClientSideScript = "af_checkupload();";
                //ctlForm.PostButton.ClientSideScript = Page.ClientScript.GetPostBackEventReference(Me, String.Empty)
                ctlForm.PostButton.PostBack = false;
            }
            ctlForm.CancelButton.ImageUrl = MyThemePath + "/images/cancel32.png";
            ctlForm.CancelButton.ImageLocation = "TOP";
            ctlForm.CancelButton.PostBack = false;
            ctlForm.CancelButton.ClientSideScript = "javascript:history.go(-1);";
            ctlForm.CancelButton.Confirm = true;
            ctlForm.CancelButton.Height = Unit.Pixel(50);
            ctlForm.CancelButton.Width = Unit.Pixel(50);
            ctlForm.CancelButton.ConfirmMessage = GetSharedResource("[RESX:ConfirmCancel]");
            ctlForm.ModuleConfiguration = this.ModuleConfiguration;
            ctlForm.Subscribe = UserPrefTopicSubscribe;
            if (fi.AllowHTML)
            {
                _allowHTML = IsHtmlPermitted(fi.EditorPermittedUsers, UserIsTrusted, canModEdit);
            }
            ctlForm.AllowHTML = _allowHTML;
            if (_allowHTML)
            {
                _editorType = fi.EditorType;
            }
            else
            {
                _editorType = EditorTypes.TEXTBOX;
            }
            if (Request.Browser.IsMobileDevice)
            {
                _editorType = EditorTypes.TEXTBOX;
                _allowHTML = false;
            }
            ctlForm.EditorType = _editorType;
            ctlForm.ForumInfo = fi;
            ctlForm.RequireCaptcha = true;
            if (_editorType == EditorTypes.TEXTBOX)
            {
                Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/text_editor.js"));
            }
            else if (_editorType == EditorTypes.ACTIVEEDITOR)
            {
                Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/active_editor.js"));
            }
            else
            {
                Framework.Providers.ProviderConfiguration _prov = new Framework.Providers.ProviderConfiguration();
                _prov = Framework.Providers.ProviderConfiguration.GetProviderConfiguration("htmlEditor");

                if (_prov.DefaultProvider.ToLowerInvariant().Contains("telerik") | _prov.DefaultProvider.ToLowerInvariant().Contains("radeditor"))
                {
                    Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/telerik_editor.js"));
                }
                else if (_prov.DefaultProvider.Contains("CKHtmlEditorProvider"))
                {
                    Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/ck_editor.js"));
                }
                else if (_prov.DefaultProvider.Contains("FckHtmlEditorProvider"))
                {
                    Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/fck_editor.js"));
                }
                else
                {
                    Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/other_editor.js"));
                }
            }
            if (Request.Params["action"] != null)
            {
                switch (Request.Params["action"].ToLowerInvariant())
                {
                    case "te": //Topic Edit
                        if (canModEdit || (canEdit && Request.IsAuthenticated))
                        {
                            isEdit = true;
                            PrepareTopic();
                            LoadTopic();
                        }
                        break;
                    case "re": //Reply Edit
                        if (canModEdit || (canEdit && Request.IsAuthenticated))
                        {
                            isEdit = true;
                            PrepareReply();
                            LoadReply();
                        }
                        break;
                    case "reply":
                        if (CanReply)
                        {
                            PrepareReply();
                        }
                        break;
                    case "new":
                        if (CanCreate)
                        {
                            PrepareTopic();

                        }
                        break;
                    default:
                        if (CanCreate)
                        {
                            PrepareTopic();
                        }
                        break;
                }
            }
            else
            {
                if (QuoteId == 0 && ReplyId == 0 && TopicId == -1 && CanCreate)
                {
                    PrepareTopic();
                }
                else if ((QuoteId > 0 | ReplyId > 0 | TopicId > 0) && CanReply)
                {
                    PrepareReply();
                }
            }
            if (isEdit && !Request.IsAuthenticated)
            {
                Response.Redirect(NavigateUrl(ForumTabId));
            }
            ctlForm.ContentId = _contentId;
            ctlForm.AuthorId = _authorId;
            plhContent.Controls.Add(ctlForm);

            EditorClientId = ctlForm.ClientID;

            ctlForm.BubbleClick += new System.EventHandler(ctlForm_Click);
            cbPreview.CallbackEvent += cbPreview_Callback;

            //Page.ClientScript.RegisterClientScriptInclude("aftags", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/jquery.tokeninput.js"))
        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
        }
        private void ctlForm_Click(object sender, System.EventArgs e)
        {
            Page.Validate();
            int iFloodInterval = MainSettings.FloodInterval;
            if (iFloodInterval > 0)
            {
                if (ForumUser != null)
                {
                    if (SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Second, ForumUser.Profile.DateLastPost, DateTime.Now) < iFloodInterval)
                    {
                        Controls.InfoMessage im = new Controls.InfoMessage();
                        im.Message = "<div class=\"afmessage\">" + string.Format(GetSharedResource("[RESX:Error:FloodControl]"), iFloodInterval) + "</div>";
                        plhMessage.Controls.Add(im);
                        return;
                    }
                }
            }
            if (Page.IsValid & Utilities.InputIsValid(ctlForm.Body.Trim()) && Utilities.InputIsValid(ctlForm.Subject))
            {
                if (TopicId == -1 || (TopicId > 0 && Request.Params["action"] == "te"))
                {
                    if (ValidateProperties())
                    {
                        SaveTopic();
                    }
                }
                else
                {
                    SaveReply();
                }
            }
        }
        private bool ValidateProperties()
        {
            if (ForumInfo.Properties != null && ForumInfo.Properties.Count > 0)
            {
                foreach (PropertiesInfo p in ForumInfo.Properties)
                {
                    if (p.IsRequired)
                    {
                        if (Request.Form["afprop-" + p.PropertyId] == null)
                        {
                            return false;
                        }
                        if ((Request.Form["afprop-" + p.PropertyId] != null) && string.IsNullOrEmpty(Request.Form["afprop-" + p.PropertyId].ToString().Trim()))
                        {
                            return false;
                        }
                    }
                    if (!(string.IsNullOrEmpty(p.ValidationExpression)) && !(string.IsNullOrEmpty(Request.Form["afprop-" + p.PropertyId].ToString().Trim())))
                    {
                        bool isMatch = Regex.IsMatch(Request.Form["afprop-" + p.PropertyId].ToString().Trim(), p.ValidationExpression, RegexOptions.IgnoreCase);
                        if (!isMatch)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return true;
        }
        private void cbPreview_Callback(object sender, DotNetNuke.Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            switch (e.Parameters[0].ToLower())
            {
                case "preview":
                    string message = e.Parameters[1].ToString();
                    int TopicTemplateID = 0;

                    TopicTemplateID = ForumInfo.TopicTemplateId;
                    message = Utilities.CleanString(PortalId, message, _allowHTML, _editorType, ForumInfo.UseFilter, ForumInfo.AllowScript, ForumModuleId, ImagePath, ForumInfo.AllowEmoticons);
                    message = Utilities.ManageImagePath(message);
                    var uc = new UserController();
                    var up = uc.GetUser(PortalId, ForumModuleId, UserId);
                    if (up == null)
                    {
                        up = new User();
                        up.UserId = -1;
                        up.UserName = "guest";
                        up.Profile.TopicCount = 0;
                        up.Profile.ReplyCount = 0;
                        up.DateCreated = DateTime.Now;

                    }
                    message = TemplateUtils.PreviewTopic(TopicTemplateID, PortalId, ForumModuleId, ForumTabId, ForumInfo, UserId, message, ImagePath, up, DateTime.Now, CurrentUserType, TimeZoneOffset);
                    hidPreviewText.Value = message;
                    break;
            }
            hidPreviewText.RenderControl(e.Output);
        }
        #endregion
        #region Private Methods
        private void LoadTopic()
        {
            ctlForm.EditorMode = DotNetNuke.Modules.ActiveForums.Controls.SubmitForm.EditorModes.EditTopic;
            TopicInfo ti = null;
            TopicsController tc = new TopicsController();
            ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, UserId, true);
            if (ti == null)
            {
                Response.Redirect(NavigateUrl(ForumTabId));
            }
            else if ((ti.Content.AuthorId != this.UserId && canModEdit == false) | (ti.Content.AuthorId == this.UserId && canEdit == false) | (canEdit == false && canModEdit))
            {
                Response.Redirect(NavigateUrl(ForumTabId));
            }
            else if (!canModEdit && (ti.Content.AuthorId == this.UserId && canEdit && MainSettings.EditInterval > 0 & SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Minute, ti.Content.DateCreated, DateTime.Now) > MainSettings.EditInterval))
            {
                Controls.InfoMessage im = new Controls.InfoMessage();
                im.Message = "<div class=\"afmessage\">" + string.Format(GetSharedResource("[RESX:Message:EditIntervalReached]"), MainSettings.EditInterval.ToString()) + "</div>";
                plhMessage.Controls.Add(im);
                plhContent.Controls.Clear();
            }
            else
            {
                //User has acccess
                string sBody = ti.Content.Body;
                string sSubject = ti.Content.Subject;
                sBody = Utilities.PrepareForEdit(PortalId, ForumModuleId, ImagePath, sBody, _allowHTML, _editorType);
                sSubject = Utilities.PrepareForEdit(PortalId, ForumModuleId, ImagePath, sSubject, false, EditorTypes.TEXTBOX);
                ctlForm.Subject = sSubject;
                ctlForm.Summary = ti.Content.Summary;
                ctlForm.Body = sBody;
                ctlForm.AnnounceEnd = ti.AnnounceEnd;
                ctlForm.AnnounceStart = ti.AnnounceStart;
                ctlForm.Locked = ti.IsLocked;
                ctlForm.Pinned = ti.IsPinned;
                ctlForm.TopicIcon = ti.TopicIcon;
                ctlForm.Tags = ti.Tags;
                ctlForm.Categories = ti.Categories;
                ctlForm.IsApproved = ti.IsApproved;
                ctlForm.StatusId = ti.StatusId;
                ctlForm.TopicPriority = ti.Priority;
                if (ti.Author.AuthorId > 0)
                {
                    ctlForm.Subscribe = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, ti.Author.AuthorId);
                }
                _contentId = ti.ContentId;
                _authorId = ti.Author.AuthorId;
                if (!(string.IsNullOrEmpty(ti.TopicData)))
                {
                    List<PropertiesInfo> pl = new List<PropertiesInfo>();
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(ti.TopicData);
                    if (xDoc != null)
                    {
                        System.Xml.XmlNode xRoot = xDoc.DocumentElement;
                        System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes("//properties/property");
                        if (xNodeList.Count > 0)
                        {
                            int i = 0;
                            for (i = 0; i < xNodeList.Count; i++)
                            {
                                string pName = Utilities.HTMLDecode(xNodeList[i].ChildNodes[0].InnerText);
                                string pValue = Utilities.HTMLDecode(xNodeList[i].ChildNodes[1].InnerText);
                                int pId = Convert.ToInt32(xNodeList[i].Attributes["id"].Value);
                                PropertiesInfo p = new PropertiesInfo();
                                p.Name = pName;
                                p.DefaultValue = pValue;
                                p.PropertyId = pId;
                                pl.Add(p);
                            }
                        }
                    }
                    ctlForm.TopicProperties = pl;
                }
                if (ti.TopicType == TopicTypes.Poll)
                {
                    //Get Poll
                    DataSet ds = DataProvider.Instance().Poll_Get(ti.TopicId);
                    if (ds.Tables.Count > 0)
                    {
                        DataRow pollRow = ds.Tables[0].Rows[0];
                        ctlForm.PollQuestion = pollRow["Question"].ToString();
                        ctlForm.PollType = pollRow["PollType"].ToString();

                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            ctlForm.PollOptions += dr["OptionName"].ToString() + System.Environment.NewLine;
                        }
                    }
                }
                if (!(ti.Content.AuthorId == this.UserId) && canModApprove)
                {
                    ctlForm.ShowModOptions = true;
                }
            }
        }
        private void LoadReply()
        {
            //Edit a Reply
            ctlForm.EditorMode = DotNetNuke.Modules.ActiveForums.Controls.SubmitForm.EditorModes.EditReply;
            ReplyInfo ri = null;
            ReplyController rc = new ReplyController();
            ri = rc.Reply_Get(PortalId, ForumModuleId, TopicId, PostId);
            if (ri == null)
            {
                //Load Reply Not Found
                //Dim im As New Controls.InfoMessage
                //im.Message = "<div class=""afmessage"">" & GetSharedResource("[RESX:Message:TopicNotFound]") & "</div>"
                //plhMessage.Controls.Add(im)
                //plhContent.Controls.Clear()
                Response.Redirect(NavigateUrl(ForumTabId));
            }
            else if ((ri.Content.AuthorId != this.UserId && canModEdit == false) | (ri.Content.AuthorId == this.UserId && canEdit == false) | (canEdit == false && canModEdit))
            {
                //Load Access Denied
                //Dim im As New Controls.InfoMessage
                //im.Message = "<div class=""afmessage"">" & GetSharedResource("[RESX:Message:AccessDenied]") & "</div>"
                //plhMessage.Controls.Add(im)
                //plhContent.Controls.Clear()
                Response.Redirect(NavigateUrl(ForumTabId));
            }
            else if (!canModEdit && (ri.Content.AuthorId == this.UserId && canEdit && MainSettings.EditInterval > 0 & SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Minute, ri.Content.DateCreated, DateTime.Now) > MainSettings.EditInterval))
            {
                Controls.InfoMessage im = new Controls.InfoMessage();
                im.Message = "<div class=\"afmessage\">" + string.Format(GetSharedResource("[RESX:Message:EditIntervalReached]"), MainSettings.EditInterval.ToString()) + "</div>";
                plhMessage.Controls.Add(im);
                plhContent.Controls.Clear();
            }
            else
            {
                string sBody = ri.Content.Body;
                string sSubject = ri.Content.Subject;
                sBody = Utilities.PrepareForEdit(PortalId, ForumModuleId, ImagePath, sBody, _allowHTML, _editorType);
                sSubject = Utilities.PrepareForEdit(PortalId, ForumModuleId, ImagePath, sSubject, false, EditorTypes.TEXTBOX);
                ctlForm.Subject = sSubject;
                ctlForm.Body = sBody;
                ctlForm.IsApproved = ri.IsApproved;
                _contentId = ri.ContentId;
                _authorId = ri.Author.AuthorId;
                if (ri.Author.AuthorId > 0)
                {
                    ctlForm.Subscribe = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, ri.Author.AuthorId);
                }
                if (!(ri.Content.AuthorId == this.UserId) && canModApprove)
                {
                    ctlForm.ShowModOptions = true;
                }
            }

        }
        private void PrepareTopic()
        {
            string Template = string.Empty;
            if (fi.TopicFormId == 0)
            {
                string myFile = string.Empty;
                myFile = Request.MapPath(DotNetNuke.Common.Globals.ApplicationPath) + "\\DesktopModules\\ActiveForums\\config\\templates\\TopicEditor.txt";
                Template = File.ReadAllText(myFile);
            }
            else
            {
                TemplateController tc = new TemplateController();
                TemplateInfo ti = tc.Template_Get(fi.TopicFormId, PortalId, ForumModuleId);
                Template = ti.TemplateHTML;
            }

            if (MainSettings.UseSkinBreadCrumb)
            {
                string sCrumb = "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + ForumInfo.ForumGroupId.ToString()) + "\">" + ForumInfo.GroupName + "</a>|";
                sCrumb += "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumInfo.ForumID.ToString()) + "\">" + ForumInfo.ForumName + "</a>";
                if (Environment.UpdateBreadCrumb(Page.Controls, sCrumb))
                {
                    Template = Template.Replace("<div class=\"afcrumb\">[AF:LINK:FORUMMAIN] > [AF:LINK:FORUMGROUP] > [AF:LINK:FORUMNAME]</div>", string.Empty);
                }
            }


            ctlForm.EditorMode = DotNetNuke.Modules.ActiveForums.Controls.SubmitForm.EditorModes.NewTopic;

            if (Permissions.HasPerm(fi.Security.ModApprove, ForumUser.UserRoles))
            {
                ctlForm.ShowModOptions = true;
            }
            ctlForm.Template = Template;
            ctlForm.IsApproved = isApproved;

        }
        /// <summary>
        /// Prepares the post form for creating a reply.
        /// </summary>
        private void PrepareReply()
        {
            ctlForm.EditorMode = DotNetNuke.Modules.ActiveForums.Controls.SubmitForm.EditorModes.Reply;

            string Template = string.Empty;
            if (fi.ReplyFormId == 0)
            {
                string myFile = string.Empty;
                myFile = Request.MapPath(DotNetNuke.Common.Globals.ApplicationPath) + "\\DesktopModules\\ActiveForums\\config\\templates\\ReplyEditor.txt";
                Template = File.ReadAllText(myFile);
            }
            else
            {
                TemplateController tc = new TemplateController();
                TemplateInfo ti = tc.Template_Get(fi.ReplyFormId, PortalId, ForumModuleId);
                Template = ti.TemplateHTML;
            }
            if (MainSettings.UseSkinBreadCrumb)
            {
                Template = Template.Replace("<div class=\"afcrumb\">[AF:LINK:FORUMMAIN] > [AF:LINK:FORUMGROUP] > [AF:LINK:FORUMNAME]</div>", string.Empty);

            }
            ctlForm.Template = Template;
            if (!(TopicId > 0))
            {
                //Can't Find Topic
                Controls.InfoMessage im = new Controls.InfoMessage();
                im.Message = GetSharedResource("[RESX:Message:LoadTopicFailed]");
                plhContent.Controls.Add(im);
            }
            else if (!CanReply)
            {
                //No permission to reply
                Controls.InfoMessage im = new Controls.InfoMessage();
                im.Message = GetSharedResource("[RESX:Message:AccessDenied]");
                plhContent.Controls.Add(im);
            }
            else
            {
                TopicInfo ti = null;
                TopicsController tc = new TopicsController();
                ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, UserId, true);
                ctlForm.Subject = Utilities.GetSharedResource("[RESX:SubjectPrefix]") + " " + ti.Content.Subject;
                ctlForm.TopicSubject = ti.Content.Subject;
                string body = string.Empty;
                if (ti.IsLocked && (CurrentUserType == CurrentUserTypes.Anon || CurrentUserType == CurrentUserTypes.Auth))
                {
                    Response.Redirect(NavigateUrl(ForumTabId));
                }
                if (Request.Params[ParamKeys.QuoteId] != null | Request.Params[ParamKeys.ReplyId] != null | Request.Params[ParamKeys.PostId] != null)
                {
                    //Setup form for Quote or Reply with body display
                    bool IsQuote = false;
                    int PostId = 0;
                    string sPostedBy = Utilities.GetSharedResource("[RESX:PostedBy]") + " {0} {1} {2}";
                    if (Request.Params[ParamKeys.QuoteId] != null)
                    {
                        IsQuote = true;
                        if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.QuoteId]))
                        {
                            PostId = Convert.ToInt32(Request.Params[ParamKeys.QuoteId]);
                        }
                    }
                    else if (Request.Params[ParamKeys.ReplyId] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.ReplyId]))
                        {
                            PostId = Convert.ToInt32(Request.Params[ParamKeys.ReplyId]);
                        }
                    }
                    else if (Request.Params[ParamKeys.PostId] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.PostId]))
                        {
                            PostId = Convert.ToInt32(Request.Params[ParamKeys.PostId]);
                        }

                    }
                    if (!(PostId == 0))
                    {
                        string userDisplay = MainSettings.UserNameDisplay;
                        if (_editorType == EditorTypes.TEXTBOX)
                        {
                            userDisplay = "none";
                        }
                        Content ci = null;
                        if (PostId == TopicId)
                        {
                            ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId);
                            ci = ti.Content;
                            sPostedBy = string.Format(sPostedBy, UserProfiles.GetDisplayName(ForumModuleId, MainSettings.MemberListMode, false, ti.Content.AuthorId, userDisplay, ti.Author), Utilities.GetSharedResource("On.Text"), GetServerDateTime(ti.Content.DateCreated));
                        }
                        else
                        {
                            ReplyInfo ri = null;
                            ReplyController rc = new ReplyController();
                            ri = rc.Reply_Get(PortalId, ForumModuleId, TopicId, PostId);
                            ci = ri.Content;
                            sPostedBy = string.Format(sPostedBy, UserProfiles.GetDisplayName(ForumModuleId, MainSettings.MemberListMode, false, ri.Content.AuthorId, userDisplay, ri.Author), Utilities.GetSharedResource("On.Text"), GetServerDateTime(ri.Content.DateCreated));
                        }
                        if (ci != null)
                        {
                            body = ci.Body;
                        }
                    }
                    if (_allowHTML && _editorType != EditorTypes.TEXTBOX)
                    {
                        if (body.ToUpper().Contains("<CODE") | body.ToUpper().Contains("[CODE]"))
                        {
                            CodeParser objCode = new CodeParser();
                            body = CodeParser.ParseCode(Utilities.HTMLDecode(body));
                        }
                    }
                    else
                    {
                        body = Utilities.PrepareForEdit(PortalId, ForumModuleId, ImagePath, body, _allowHTML, _editorType);
                    }
                    if (IsQuote)
                    {
                        ctlForm.EditorMode = DotNetNuke.Modules.ActiveForums.Controls.SubmitForm.EditorModes.Quote;
                        if (_allowHTML && _editorType != EditorTypes.TEXTBOX)
                        {
                            body = "<blockquote>" + System.Environment.NewLine + sPostedBy + System.Environment.NewLine + "<br />" + System.Environment.NewLine + body + System.Environment.NewLine + "</blockquote><br /><br />";
                        }
                        else
                        {
                            body = "[quote]" + System.Environment.NewLine + sPostedBy + System.Environment.NewLine + body + System.Environment.NewLine + "[/quote]" + System.Environment.NewLine;
                        }
                    }
                    else
                    {
                        ctlForm.EditorMode = DotNetNuke.Modules.ActiveForums.Controls.SubmitForm.EditorModes.ReplyWithBody;
                        body = sPostedBy + "<br />" + body;
                    }
                    ctlForm.Body = body;


                }
            }
            if (!(ctlForm.EditorMode == DotNetNuke.Modules.ActiveForums.Controls.SubmitForm.EditorModes.EditReply) && canModApprove)
            {
                ctlForm.ShowModOptions = false;
            }
        }
        private void SaveTopic()
        {
            bool bSend = true;

            string Subject = string.Empty;
            string Body = string.Empty;
            string Summary = string.Empty;
            Subject = ctlForm.Subject;
            Body = ctlForm.Body;
            Subject = Utilities.CleanString(PortalId, Subject, false, EditorTypes.TEXTBOX, ForumInfo.UseFilter, false, ForumModuleId, MyThemePath, false);
            Body = Utilities.CleanString(PortalId, Body, _allowHTML, _editorType, ForumInfo.UseFilter, ForumInfo.AllowScript, ForumModuleId, MyThemePath, ForumInfo.AllowEmoticons);
            Summary = ctlForm.Summary;
            int AuthorId = -1;
            string AuthorName = string.Empty;
            if (Request.IsAuthenticated)
            {
                AuthorId = UserInfo.UserID;
                switch (MainSettings.UserNameDisplay.ToUpperInvariant())
                {
                    case "USERNAME":
                        AuthorName = UserInfo.Username.Trim(' ');
                        break;
                    case "FULLNAME":
                        AuthorName = Convert.ToString(UserInfo.FirstName + " " + UserInfo.LastName).Trim(' ');
                        break;
                    case "FIRSTNAME":
                        AuthorName = UserInfo.FirstName.Trim(' ');
                        break;
                    case "LASTNAME":
                        AuthorName = UserInfo.LastName.Trim(' ');
                        break;
                    case "DISPLAYNAME":
                        AuthorName = UserInfo.DisplayName.Trim(' ');
                        break;
                    default:
                        AuthorName = UserInfo.DisplayName;
                        break;
                }
                //AuthorName = UserInfo.DisplayName
            }
            else
            {
                AuthorId = -1;
                AuthorName = Utilities.CleanString(PortalId, ctlForm.AuthorName, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, MyThemePath, false);
                if (AuthorName.Trim() == string.Empty)
                {
                    return;
                }
            }
            TopicsController tc = new TopicsController();
            TopicInfo ti = null;
            if (TopicId > 0)
            {
                ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId);
                ti.Content.DateUpdated = DateTime.Now;
                AuthorId = ti.Author.AuthorId;
                bSend = false;
            }
            else
            {
                ti = new TopicInfo();
                DateTime dt = DateTime.Now;
                ti.Content.DateCreated = dt;
                ti.Content.DateUpdated = dt;
            }
            ti.AnnounceEnd = ctlForm.AnnounceEnd;
            ti.AnnounceStart = ctlForm.AnnounceStart;
            ti.Priority = ctlForm.TopicPriority;
            if (!isEdit)
            {
                ti.Content.AuthorId = AuthorId;
                ti.Content.AuthorName = AuthorName;
                ti.Content.IPAddress = Request.UserHostAddress;
            }
            if (Regex.IsMatch(Body, "<CODE([^>]*)>", RegexOptions.IgnoreCase))
            {
                foreach (Match m in Regex.Matches(Body, "<CODE([^>]*)>(.*?)</CODE>", RegexOptions.IgnoreCase))
                {
                    Body = Body.Replace(m.Value, m.Value.Replace("<br>", System.Environment.NewLine));
                }
            }
            if (!(string.IsNullOrEmpty(ForumInfo.PrefixURL)))
            {
                string cleanSubject = Utilities.CleanName(Subject).ToLowerInvariant();
                if (SimulateIsNumeric.IsNumeric(cleanSubject))
                {
                    cleanSubject = "Topic-" + cleanSubject;
                }
                string topicUrl = cleanSubject;
                string urlPrefix = "/";
                if (!(string.IsNullOrEmpty(ForumInfo.ForumGroup.PrefixURL)))
                {
                    urlPrefix += ForumInfo.ForumGroup.PrefixURL + "/";
                }
                if (!(string.IsNullOrEmpty(ForumInfo.PrefixURL)))
                {
                    urlPrefix += ForumInfo.PrefixURL + "/";
                }
                string urlToCheck = urlPrefix + cleanSubject;
                Data.Topics topicsDb = new Data.Topics();
                for (int u = 0; u <= 200; u++)
                {
                    int tid = topicsDb.TopicIdByUrl(PortalId, ModuleId, urlToCheck);
                    if (tid > 0 && tid == TopicId)
                    {
                        break;
                    }
                    else if (tid > 0)
                    {
                        topicUrl = (u + 1) + "-" + cleanSubject;
                        urlToCheck = urlPrefix + topicUrl;
                    }
                    else
                    {
                        break;
                    }
                }
                if (topicUrl.Length > 150)
                {
                    topicUrl = topicUrl.Substring(0, 149);
                    topicUrl = topicUrl.Substring(0, topicUrl.LastIndexOf("-"));
                }
                ti.TopicUrl = topicUrl;
                //.URL = topicUrl
            }
            else
            {
                //.URL = String.Empty
                ti.TopicUrl = string.Empty;
            }

            ti.Content.Body = Body; //Utilities.CleanString(PortalId, Body, fi.AllowHTML, fi.EditorType, fi.UseFilter, fi.AllowScript, ForumModuleId, String.Empty)
            ti.Content.Subject = Subject;
            ti.Content.Summary = Summary;
            ti.IsAnnounce = Convert.ToBoolean(((ti.AnnounceEnd != Utilities.NullDate() & ti.AnnounceStart != Utilities.NullDate()) ? true : false));
            if (canModApprove && fi.IsModerated)
            {
                ti.IsApproved = ctlForm.IsApproved;
            }
            else
            {
                ti.IsApproved = isApproved;
            }
            bSend = ti.IsApproved;
            ti.IsArchived = false;
            ti.IsDeleted = false;
            if (canLock)
            {
                ti.IsLocked = ctlForm.Locked;
            }
            else
            {
                ti.IsLocked = false;
            }
            if (canPin)
            {
                ti.IsPinned = ctlForm.Pinned;
            }
            else
            {
                ti.IsPinned = false;
            }
            ti.StatusId = ctlForm.StatusId;
            ti.TopicIcon = ctlForm.TopicIcon;
            ti.TopicType = 0;
            if (ForumInfo.Properties != null)
            {
                StringBuilder tData = new StringBuilder();
                tData.Append("<topicdata>");
                tData.Append("<properties>");
                foreach (PropertiesInfo p in ForumInfo.Properties)
                {
                    string pkey = "afprop-" + p.PropertyId.ToString();

                    tData.Append("<property id=\"" + p.PropertyId.ToString() + "\">");
                    tData.Append("<name><![CDATA[");
                    tData.Append(p.Name);
                    tData.Append("]]></name>");
                    if (Request.Form[pkey] != null)
                    {
                        tData.Append("<value><![CDATA[");
                        tData.Append(Utilities.XSSFilter(Request.Form[pkey]));
                        tData.Append("]]></value>");
                    }
                    else
                    {
                        tData.Append("<value></value>");
                    }
                    tData.Append("</property>");
                }
                tData.Append("</properties>");
                tData.Append("</topicdata>");
                ti.TopicData = tData.ToString();
            }

            TopicId = tc.TopicSave(PortalId, ti);
            ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, -1, false);
            if (ti != null)
            {
                tc.Topics_SaveToForum(ForumId, TopicId, PortalId, ModuleId);
                SaveAttach(ti.ContentId);
                if (ti.IsApproved && ti.Author.AuthorId > 0)
                {
                    Data.Profiles uc = new Data.Profiles();
                    uc.Profile_UpdateTopicCount(PortalId, ti.Author.AuthorId);
                }
            }
            if (Permissions.HasPerm(ForumInfo.Security.Tag, ForumUser.UserRoles))
            {
                DataProvider.Instance().Tags_DeleteByTopicId(PortalId, ForumModuleId, TopicId);
                string tagForm = string.Empty;
                if (Request.Form["txtTags"] != null)
                {
                    tagForm = Request.Form["txtTags"];
                }
                if (!(tagForm == string.Empty))
                {
                    string[] Tags = tagForm.Split(',');
                    foreach (string tag in Tags)
                    {
                        string sTag = Utilities.CleanString(PortalId, tag.Trim(), false, EditorTypes.TEXTBOX, false, false, ForumModuleId, string.Empty, false);
                        DataProvider.Instance().Tags_Save(PortalId, ForumModuleId, -1, sTag, 0, 1, 0, TopicId, false, -1, -1);
                    }
                }
            }
            if (Permissions.HasPerm(ForumInfo.Security.Categorize, ForumUser.UserRoles))
            {
                if (Request.Form["amaf-catselect"] != null)
                {
                    string[] cats = Request.Form["amaf-catselect"].Split(';');
                    DataProvider.Instance().Tags_DeleteTopicToCategory(PortalId, ForumModuleId, -1, TopicId);
                    foreach (string c in cats)
                    {
                        int cid = -1;
                        if (!(string.IsNullOrEmpty(c)) && SimulateIsNumeric.IsNumeric(c))
                        {
                            cid = Convert.ToInt32(c);
                            if (cid > 0)
                            {
                                DataProvider.Instance().Tags_AddTopicToCategory(PortalId, ForumModuleId, cid, TopicId);
                            }
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(ctlForm.PollQuestion) && !String.IsNullOrEmpty(ctlForm.PollOptions))
            {
                string sPollQ = ctlForm.PollQuestion.Trim();
                sPollQ = Utilities.CleanString(PortalId, sPollQ, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, string.Empty, false);
                int PollId = 0;
                PollId = DataProvider.Instance().Poll_Save(-1, TopicId, UserId, ctlForm.PollQuestion.Trim(), ctlForm.PollType);
                if (PollId > 0)
                {
                    string[] Options = ctlForm.PollOptions.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);
               

                    foreach (string opt in Options)
                    {
                        if (opt.Trim() != "")
                        {
                            var value = Utilities.CleanString(PortalId, opt, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, string.Empty, false);
                            DataProvider.Instance().Poll_Option_Save(-1, PollId, value.Trim(), TopicId);
                        }

                    }
                }
                ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, -1, false);
                ti.TopicType = TopicTypes.Poll;
                tc.TopicSave(PortalId, ti);
            }
            try
            {
                string cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                DataCache.CacheClearPrefix(cachekey);
                if (ctlForm.Subscribe && AuthorId == UserId)
                {
                    if (!(Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, AuthorId)))
                    {
                        SubscriptionController sc = new SubscriptionController();
                        sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, AuthorId, ForumUser.UserRoles);
                    }
                }
                else if (isEdit)
                {
                    bool isSub = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, AuthorId);
                    if (isSub && !ctlForm.Subscribe)
                    {
                        SubscriptionController sc = new SubscriptionController();
                        sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, AuthorId, ForumUser.UserRoles);
                    }
                }
                if (bSend && !isEdit)
                {
                    Subscriptions.SendSubscriptions(PortalId, ForumModuleId, ForumTabId, fi, TopicId, 0, ti.Content.AuthorId);
                }
                if (ti.IsApproved == false)
                {
                    List<Entities.Users.UserInfo> mods = Utilities.GetListOfModerators(PortalId, ForumId);
                    NotificationType notificationType = NotificationsController.Instance.GetNotificationType("AF-ForumModeration");
                    string notifySubject = Utilities.GetSharedResource("NotificationSubjectTopic");
                    notifySubject = notifySubject.Replace("[DisplayName]", UserInfo.DisplayName);
                    notifySubject = notifySubject.Replace("[TopicSubject]", ti.Content.Subject);
                    string notifyBody = Utilities.GetSharedResource("NotificationBodyTopic");
                    notifyBody = notifyBody.Replace("[Post]", ti.Content.Body);
                    string notificationKey = string.Format("{0}:{1}:{2}:{3}:{4}", TabId, ForumModuleId, ForumId, TopicId, ReplyId);

                    Notification notification = new Notification();
                    notification.NotificationTypeID = notificationType.NotificationTypeId;
                    notification.Subject = notifySubject;
                    notification.Body = notifyBody;
                    notification.IncludeDismissAction = false;
                    notification.SenderUserID = UserInfo.UserID;
                    notification.Context = notificationKey;


                    NotificationsController.Instance.SendNotification(notification, PortalId, null, mods);



                    string[] Params = { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=confirmaction", ParamKeys.ConfirmActionId + "=" + ConfirmActions.MessagePending };
                    Response.Redirect(NavigateUrl(ForumTabId, "", Params), false);
                }
                else
                {
                    if (ti != null)
                    {
                        ti.TopicId = TopicId;
                    }
                    ControlUtils ctlUtils = new ControlUtils();

                    string sUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, ti.TopicUrl, -1, -1, string.Empty, 1, SocialGroupId);
                    if (sUrl.Contains("~/") || Request.QueryString["asg"] != null)
                    {
                        sUrl = Utilities.NavigateUrl(ForumTabId, "", ParamKeys.TopicId + "=" + TopicId);
                    }

                    if (!isEdit)
                    {
                        try
                        {
                            Social amas = new Social();
                            amas.AddTopicToJournal(PortalId, ForumModuleId, ForumId, TopicId, UserId, sUrl, Subject, Summary, Body, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read, SocialGroupId);
                            if (Request.QueryString["asg"] == null & !(string.IsNullOrEmpty(MainSettings.ActiveSocialTopicsKey)) && ForumInfo.ActiveSocialEnabled)
                            {
                                // amas.AddTopicToJournal(PortalId, ForumModuleId, ForumId, UserId, sUrl, Subject, Summary, Body, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read)
                            }
                            else
                            {
                                amas.AddForumItemToJournal(PortalId, ForumModuleId, UserId, "forumtopic", sUrl, Subject, Body);
                            }


                        }
                        catch (Exception ex)
                        {
                            DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                        }
                    }



                    Response.Redirect(sUrl, false);
                }
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }


        }
        private void SaveReply()
        {
            string Subject = string.Empty;
            string Body = string.Empty;
            bool bSend = true;
            Subject = ctlForm.Subject;
            Body = ctlForm.Body;
            Subject = Utilities.CleanString(PortalId, Subject, false, EditorTypes.TEXTBOX, fi.UseFilter, false, ForumModuleId, MyThemePath, false);
            Body = Utilities.CleanString(PortalId, Body, _allowHTML, _editorType, fi.UseFilter, fi.AllowScript, ForumModuleId, MyThemePath, fi.AllowEmoticons);
            int AuthorId = -1;
            string AuthorName = string.Empty;
            if (Request.IsAuthenticated)
            {
                AuthorId = UserInfo.UserID;
                switch (MainSettings.UserNameDisplay.ToUpperInvariant())
                {
                    case "USERNAME":
                        AuthorName = UserInfo.Username.Trim(' ');
                        break;
                    case "FULLNAME":
                        AuthorName = Convert.ToString(UserInfo.FirstName + " " + UserInfo.LastName).Trim(' ');
                        break;
                    case "FIRSTNAME":
                        AuthorName = UserInfo.FirstName.Trim(' ');
                        break;
                    case "LASTNAME":
                        AuthorName = UserInfo.LastName.Trim(' ');
                        break;
                    case "DISPLAYNAME":
                        AuthorName = UserInfo.DisplayName.Trim(' ');
                        break;
                    default:
                        AuthorName = UserInfo.DisplayName;
                        break;
                }
            }
            else
            {
                AuthorId = -1;
                AuthorName = Utilities.CleanString(PortalId, ctlForm.AuthorName, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, MyThemePath, false);
                if (AuthorName.Trim() == string.Empty)
                {
                    return;
                }
            }

            TopicsController tc = new TopicsController();
            ReplyController rc = new ReplyController();
            ReplyInfo ri = null;
            if (PostId > 0)
            {
                ri = rc.Reply_Get(PortalId, ForumModuleId, TopicId, PostId);
                ri.Content.DateUpdated = DateTime.Now;
                bSend = false;
            }
            else
            {
                ri = new ReplyInfo();
                DateTime dt = DateTime.Now;
                ri.Content.DateCreated = dt;
                ri.Content.DateUpdated = dt;
                //AuthorId = ri.Author.AuthorId
            }
            if (!isEdit)
            {
                ri.Content.AuthorId = AuthorId;
                ri.Content.AuthorName = AuthorName;
                ri.Content.IPAddress = Request.UserHostAddress;
            }
            if (Regex.IsMatch(Body, "<CODE([^>]*)>", RegexOptions.IgnoreCase))
            {
                foreach (Match m in Regex.Matches(Body, "<CODE([^>]*)>(.*?)</CODE>", RegexOptions.IgnoreCase))
                {
                    Body = Body.Replace(m.Value, m.Value.Replace("<br>", System.Environment.NewLine));
                }


            }
            ri.Content.Body = Body;
            ri.Content.Subject = Subject;
            ri.Content.Summary = string.Empty;
            if (ctlForm.IsApproved != null && canModApprove && ri.Content.AuthorId != this.UserId)
            {
                ri.IsApproved = ctlForm.IsApproved;
            }
            else
            {
                ri.IsApproved = isApproved;
            }
            bSend = ri.IsApproved;
            ri.IsDeleted = false;
            ri.StatusId = ctlForm.StatusId;
            ri.TopicId = TopicId;
            int tmpReplyId = -1;
            tmpReplyId = rc.Reply_Save(PortalId, ri);
            ri = rc.Reply_Get(PortalId, ForumModuleId, TopicId, tmpReplyId);
            SaveAttach(ri.ContentId);
            //tc.ForumTopicSave(ForumID, TopicId, ReplyId)
            string cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
            DataCache.CacheClearPrefix(cachekey);
            try
            {
                if (ctlForm.Subscribe && AuthorId == UserId)
                {
                    if (!(Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, AuthorId)))
                    {
                        SubscriptionController sc = new SubscriptionController();
                        sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, AuthorId, ForumUser.UserRoles);
                    }
                }
                else if (isEdit)
                {
                    bool isSub = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, AuthorId);
                    if (isSub && !ctlForm.Subscribe)
                    {
                        SubscriptionController sc = new SubscriptionController();
                        sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, AuthorId, ForumUser.UserRoles);
                    }
                }
                if (bSend && !isEdit)
                {
                    Subscriptions.SendSubscriptions(PortalId, ForumModuleId, ForumTabId, fi, TopicId, tmpReplyId, ri.Content.AuthorId);
                }
                if (ri.IsApproved == false)
                {
                    Email oEmail = new Email();
                    oEmail.SendEmailToModerators(ForumInfo.ModNotifyTemplateId, PortalId, ForumId, ri.TopicId, tmpReplyId, ForumModuleId, ForumTabId, string.Empty);
                    string[] Params = { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=confirmaction", ParamKeys.ConfirmActionId + "=" + ConfirmActions.MessagePending };
                    Response.Redirect(Utilities.NavigateUrl(ForumTabId, "", Params), false);
                }
                else
                {
                    ControlUtils ctlUtils = new ControlUtils();
                    TopicInfo ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, -1, false);
                    string fullURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, ti.TopicUrl, -1, -1, string.Empty, 1, SocialGroupId);
                    if (fullURL.Contains("~/") || Request.QueryString["asg"] != null)
                    {
                        fullURL = Utilities.NavigateUrl(ForumTabId, "", new string[] { ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + tmpReplyId });
                    }
                    if (fullURL.EndsWith("/"))
                    {
                        fullURL += "?" + ParamKeys.ContentJumpId + "=" + tmpReplyId;
                    }

                    if (!isEdit)
                    {
                        try
                        {
                            Social amas = new Social();
                            amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, TopicId, ReplyId, UserId, fullURL, Subject, string.Empty, Body, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read, SocialGroupId);
                            //If Request.QueryString["asg"] Is Nothing And Not String.IsNullOrEmpty(MainSettings.ActiveSocialTopicsKey) And ForumInfo.ActiveSocialEnabled And Not ForumInfo.ActiveSocialTopicsOnly Then
                            //    amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, TopicId, ReplyId, UserId, fullURL, Subject, String.Empty, Body, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read)
                            //Else
                            //    amas.AddForumItemToJournal(PortalId, ForumModuleId, UserId, "forumreply", fullURL, Subject, Body)
                            //End If


                        }
                        catch (Exception ex)
                        {
                            DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                        }

                    }
                    Response.Redirect(fullURL);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void SaveAttach(int ContentId)
        {
            string AttachIds = hidAttachIds.Value;
            IFileManager _fileManager = FileManager.Instance;



            if (!(AttachIds == string.Empty))
            {
                foreach (string attachid in AttachIds.Split(';'))
                {
                    if (!(attachid.Trim() == string.Empty))
                    {
                        int tmpAttachId = Convert.ToInt32(attachid);
                        IFileInfo _file = _fileManager.GetFile(tmpAttachId);
                        Data.AttachController adb = new Data.AttachController();
                        if (_file == null)
                        {
                            adb.SaveToContent(ContentId, tmpAttachId, null, null, false, null);
                        }
                        else
                        {
                            string fileUrl = "~/LinkClick.aspx?fileticket={0}";
                            string url = Page.ResolveUrl("~/LinkClick.aspx?fileid=" + _file.FileId);
                            fileUrl = string.Format(fileUrl, UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(url)));
                            adb.SaveToContent(ContentId, tmpAttachId, fileUrl, _file.FileName, true, _file.ContentType);
                        }

                    }
                }
            }



        }

        #endregion


    }
}

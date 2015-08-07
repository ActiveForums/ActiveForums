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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Modules.ActiveForums.Controls;
using DotNetNuke.Modules.ActiveForums.Extensions;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Framework;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_post : ForumBase
    {
        protected SubmitForm ctlForm = new SubmitForm();
        private bool _isApproved;
        public string PreviewText = string.Empty;
        private bool _isEdit;
        private Forum _fi;
        private UserProfileInfo _ui = new UserProfileInfo();
        private string _themePath = string.Empty;
        private bool _userIsTrusted;
        private int _contentId = -1;
        private int _authorId = -1;
        private bool _allowHTML;
        private EditorTypes _editorType = EditorTypes.TEXTBOX;
        private bool _canModEdit;
        private bool _canModApprove;
        private bool _canEdit;
        private bool _canAttach;
        private bool _canTrust;
        private bool _canLock;
        private bool _canPin;
        private bool _canAnnounce;

        public string Spinner { get; set; }
        public string EditorClientId { get; set; }

        #region Event Handlers

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            
            var oLink = new System.Web.UI.HtmlControls.HtmlGenericControl("link");
            oLink.Attributes["rel"] = "stylesheet";
            oLink.Attributes["type"] = "text/css";
            oLink.Attributes["href"] = Page.ResolveUrl("~/DesktopModules/ActiveForums/scripts/calendar.css");
            
            var oCSS = Page.FindControl("CSS");
            if (oCSS != null)
                oCSS.Controls.Add(oLink);

            _fi = ForumInfo;
            _authorId = UserId;
            _canModEdit = Permissions.HasPerm(_fi.Security.ModEdit, ForumUser.UserRoles);
            _canModApprove = Permissions.HasPerm(_fi.Security.ModApprove, ForumUser.UserRoles);
            _canEdit = Permissions.HasPerm(_fi.Security.Edit, ForumUser.UserRoles);
            _canAttach = Permissions.HasPerm(_fi.Security.Attach, ForumUser.UserRoles);
            _canTrust = Permissions.HasPerm(_fi.Security.Trust, ForumUser.UserRoles);
            _canLock = Permissions.HasPerm(_fi.Security.Lock, ForumUser.UserRoles);
            _canPin = Permissions.HasPerm(_fi.Security.Pin, ForumUser.UserRoles);
            _canAnnounce = Permissions.HasPerm(_fi.Security.Announce, ForumUser.UserRoles);

            if (_fi == null)
                Response.Redirect(NavigateUrl(ForumTabId));
            else if (Request.Params["action"] != null)
            {
                if (!_canEdit && (Request.Params["action"].ToLowerInvariant() == "te" || Request.Params["action"].ToLowerInvariant() == "re"))
                    Response.Redirect(NavigateUrl(ForumTabId));
            }

            if (CanCreate == false && CanReply == false)
                Response.Redirect(NavigateUrl(ForumTabId, "", "ctl=login") + "?returnurl=" + Server.UrlEncode(Request.RawUrl));

            if (UserId > 0)
                _ui = ForumUser.Profile;
            else
            {
                _ui.TopicCount = 0;
                _ui.ReplyCount = 0;
                _ui.RewardPoints = 0;
                _ui.IsMod = false;
                _ui.TrustLevel = -1;
            }

            _userIsTrusted = Utilities.IsTrusted((int)_fi.DefaultTrustValue, _ui.TrustLevel, _canTrust, _fi.AutoTrustLevel, _ui.PostCount);
            Spinner = Page.ResolveUrl("~/DesktopModules/activeforums/themes/" + MainSettings.Theme + "/images/loading.gif");
            _isApproved = !_fi.IsModerated || _userIsTrusted || _canModApprove;

            var myTheme = MainSettings.Theme;
            _themePath = Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + myTheme);
            ctlForm.ID = "ctlForm";
            ctlForm.PostButton.ImageUrl = _themePath + "/images/save32.png";
            ctlForm.PostButton.ImageLocation = "TOP";
            ctlForm.PostButton.Height = Unit.Pixel(50);
            ctlForm.PostButton.Width = Unit.Pixel(50);

            ctlForm.PostButton.ClientSideScript = "amPostback();";
            ctlForm.PostButton.PostBack = false;

            ctlForm.AttachmentsClientId = hidAttachments.ClientID;


            // TODO: Make sure this check happens on submit
            //if (_canAttach && _fi.AllowAttach) {}

            ctlForm.CancelButton.ImageUrl = _themePath + "/images/cancel32.png";
            ctlForm.CancelButton.ImageLocation = "TOP";
            ctlForm.CancelButton.PostBack = false;
            ctlForm.CancelButton.ClientSideScript = "javascript:history.go(-1);";
            ctlForm.CancelButton.Confirm = true;
            ctlForm.CancelButton.Height = Unit.Pixel(50);
            ctlForm.CancelButton.Width = Unit.Pixel(50);
            ctlForm.CancelButton.ConfirmMessage = GetSharedResource("[RESX:ConfirmCancel]");
            ctlForm.ModuleConfiguration = ModuleConfiguration;
            ctlForm.Subscribe = UserPrefTopicSubscribe;
            if (_fi.AllowHTML)
            {
                _allowHTML = IsHtmlPermitted(_fi.EditorPermittedUsers, _userIsTrusted, _canModEdit);
            }
            ctlForm.AllowHTML = _allowHTML;
            if (_allowHTML)
            {
                if (Request.Browser.IsMobileDevice) _editorType = (EditorTypes)_fi.EditorMobile;
                else _editorType = _fi.EditorType;
            }
            else
            {
                _editorType = EditorTypes.TEXTBOX;
            }
            ctlForm.EditorType = _editorType;
            ctlForm.ForumInfo = _fi;
            ctlForm.RequireCaptcha = true;
            switch (_editorType)
            {
                case EditorTypes.TEXTBOX:
                    Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/text_editor.js"));
                    break;
                case EditorTypes.ACTIVEEDITOR:
                    Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/active_editor.js"));
                    break;
                default:
                    {
                        var prov = ProviderConfiguration.GetProviderConfiguration("htmlEditor");

                        if (prov.DefaultProvider.ToLowerInvariant().Contains("telerik") | prov.DefaultProvider.ToLowerInvariant().Contains("radeditor"))
                        {
                            Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/telerik_editor.js"));
                        }
                        else if (prov.DefaultProvider.Contains("CKHtmlEditorProvider"))
                        {
                            Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/ck_editor.js"));
                        }
                        else if (prov.DefaultProvider.Contains("FckHtmlEditorProvider"))
                        {
                            Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/fck_editor.js"));
                        }
                        else
                        {
                            Page.ClientScript.RegisterClientScriptInclude("afeditor", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/other_editor.js"));
                        }
                    }
                    break;
            }
            if (Request.Params["action"] != null)
            {
                switch (Request.Params["action"].ToLowerInvariant())
                {
                    case "te": //Topic Edit
                        if (_canModEdit || (_canEdit && Request.IsAuthenticated))
                        {
                            _isEdit = true;
                            PrepareTopic();
                            LoadTopic();
                        }
                        break;
                    case "re": //Reply Edit
                        if (_canModEdit || (_canEdit && Request.IsAuthenticated))
                        {
                            _isEdit = true;
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
            if (_isEdit && !Request.IsAuthenticated)
            {
                Response.Redirect(NavigateUrl(ForumTabId));
            }

            PrepareAttachments(_contentId);

            ctlForm.ContentId = _contentId;
            ctlForm.AuthorId = _authorId;
            plhContent.Controls.Add(ctlForm);

            EditorClientId = ctlForm.ClientID;

            ctlForm.BubbleClick += ctlForm_Click;
            cbPreview.CallbackEvent += cbPreview_Callback;

            //Page.ClientScript.RegisterClientScriptInclude("aftags", Page.ResolveUrl("~/desktopmodules/activeforums/scripts/jquery.tokeninput.js"))
        }

        private void ctlForm_Click(object sender, EventArgs e)
        {
            Page.Validate();
            var iFloodInterval = MainSettings.FloodInterval;
            if (iFloodInterval > 0)
            {
                if (ForumUser != null)
                {
                    if (SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Second, ForumUser.Profile.DateLastPost, DateTime.Now) < iFloodInterval)
                    {
                        var im = new InfoMessage { Message = "<div class=\"afmessage\">" + string.Format(GetSharedResource("[RESX:Error:FloodControl]"), iFloodInterval) + "</div>" };
                        plhMessage.Controls.Add(im);
                        return;
                    }
                }
            }
            if (!Page.IsValid || !Utilities.InputIsValid(ctlForm.Body.Trim()) || !Utilities.InputIsValid(ctlForm.Subject)) 
                return;

            if (TopicId == -1 || (TopicId > 0 && Request.Params["action"] == "te"))
            {
                if (ValidateProperties())
                    SaveTopic();
            }
            else
            {
                SaveReply();
            }
        }

        private bool ValidateProperties()
        {
            if (ForumInfo.Properties != null && ForumInfo.Properties.Count > 0)
            {
                foreach (var p in ForumInfo.Properties)
                {
                    if (p.IsRequired)
                    {
                        if (Request.Form["afprop-" + p.PropertyId] == null)
                            return false;

                        if ((Request.Form["afprop-" + p.PropertyId] != null) && string.IsNullOrEmpty(Request.Form["afprop-" + p.PropertyId].Trim()))
                            return false;
                    }

                    if (!(string.IsNullOrEmpty(p.ValidationExpression)) && !(string.IsNullOrEmpty(Request.Form["afprop-" + p.PropertyId].Trim())))
                    {
                        var isMatch = Regex.IsMatch(Request.Form["afprop-" + p.PropertyId].Trim(), p.ValidationExpression, RegexOptions.IgnoreCase);
                        if (!isMatch)
                            return false;
                    }
                }
                return true;
            }
            return true;
        }

        private void cbPreview_Callback(object sender, CallBackEventArgs e)
        {
            switch (e.Parameters[0].ToLower())
            {
                case "preview":
                    var message = e.Parameters[1];

                    var topicTemplateID = ForumInfo.TopicTemplateId;
                    message = Utilities.CleanString(PortalId, message, _allowHTML, _editorType, ForumInfo.UseFilter, ForumInfo.AllowScript, ForumModuleId, ImagePath, ForumInfo.AllowEmoticons);
                    message = Utilities.ManageImagePath(message);
                    var uc = new UserController();
                    var up = uc.GetUser(PortalId, ForumModuleId, UserId) ?? new User
                                                                                {
                                                                                    UserId = -1,
                                                                                    UserName = "guest",
                                                                                    Profile = {TopicCount = 0, ReplyCount = 0},
                                                                                    DateCreated = DateTime.Now
                                                                                };
                    message = TemplateUtils.PreviewTopic(topicTemplateID, PortalId, ForumModuleId, ForumTabId, ForumInfo, UserId, message, ImagePath, up, DateTime.Now, CurrentUserType, UserId, TimeZoneOffset);
                    hidPreviewText.Value = message;
                    break;
            }
            hidPreviewText.RenderControl(e.Output);
        }

        #endregion

        #region Private Methods

        private void LoadTopic()
        {
            ctlForm.EditorMode = Modules.ActiveForums.Controls.SubmitForm.EditorModes.EditTopic;
            var tc = new TopicsController();
            var ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, UserId, true);
            if (ti == null)
            {
                Response.Redirect(NavigateUrl(ForumTabId));
            }
            else if ((ti.Content.AuthorId != UserId && _canModEdit == false) | (ti.Content.AuthorId == UserId && _canEdit == false) | (_canEdit == false && _canModEdit))
            {
                Response.Redirect(NavigateUrl(ForumTabId));
            }
            else if (!_canModEdit && (ti.Content.AuthorId == UserId && _canEdit && MainSettings.EditInterval > 0 & SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Minute, ti.Content.DateCreated, DateTime.Now) > MainSettings.EditInterval))
            {
                var im = new InfoMessage
                                        {
                                            Message =  "<div class=\"afmessage\">" +  string.Format(GetSharedResource("[RESX:Message:EditIntervalReached]"),  MainSettings.EditInterval) + "</div>"
                                        };
                plhMessage.Controls.Add(im);
                plhContent.Controls.Clear();
            }
            else
            {
                //User has acccess
                var sBody = ti.Content.Body;
                var sSubject = ti.Content.Subject;
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
                    ctlForm.Subscribe = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, ti.Author.AuthorId);

                _contentId = ti.ContentId;
                _authorId = ti.Author.AuthorId;

                if (!(string.IsNullOrEmpty(ti.TopicData)))
                {
                    var pl = new List<PropertiesInfo>();
                    var xDoc = new XmlDocument();
                    xDoc.LoadXml(ti.TopicData);
 
                    XmlNode xRoot = xDoc.DocumentElement;
                    if (xRoot != null)
                    {
                        var xNodeList = xRoot.SelectNodes("//properties/property");
                        if (xNodeList != null && xNodeList.Count > 0)
                        {
                            for (var i = 0; i < xNodeList.Count; i++)
                            {
                                var pName = Utilities.HTMLDecode(xNodeList[i].ChildNodes[0].InnerText);
                                var pValue = Utilities.HTMLDecode(xNodeList[i].ChildNodes[1].InnerText);
                                var xmlAttributeCollection = xNodeList[i].Attributes;
                                if (xmlAttributeCollection == null) 
                                    continue;
                                var pId = Convert.ToInt32(xmlAttributeCollection["id"].Value);
                                var p = new PropertiesInfo {Name = pName, DefaultValue = pValue, PropertyId = pId};
                                pl.Add(p);
                            }
                        }
                    }

                    ctlForm.TopicProperties = pl;
                }

                if (ti.TopicType == TopicTypes.Poll)
                {
                    //Get Poll
                    var ds = DataProvider.Instance().Poll_Get(ti.TopicId);
                    if (ds.Tables.Count > 0)
                    {
                        var pollRow = ds.Tables[0].Rows[0];
                        ctlForm.PollQuestion = pollRow["Question"].ToString();
                        ctlForm.PollType = pollRow["PollType"].ToString();

                        foreach (DataRow dr in ds.Tables[1].Rows)
                            ctlForm.PollOptions += dr["OptionName"] + System.Environment.NewLine;
                    }
                }

                if (ti.Content.AuthorId != UserId && _canModApprove)
                    ctlForm.ShowModOptions = true;
            }
        }

        private void LoadReply()
        {
            //Edit a Reply
            ctlForm.EditorMode = Modules.ActiveForums.Controls.SubmitForm.EditorModes.EditReply;
            
            var rc = new ReplyController();
            var ri = rc.Reply_Get(PortalId, ForumModuleId, TopicId, PostId);
            
            if (ri == null)
                Response.Redirect(NavigateUrl(ForumTabId));
            else if ((ri.Content.AuthorId != UserId && _canModEdit == false) | (ri.Content.AuthorId == UserId && _canEdit == false) | (_canEdit == false && _canModEdit))
                Response.Redirect(NavigateUrl(ForumTabId));

            else if (!_canModEdit && (ri.Content.AuthorId == UserId && _canEdit && MainSettings.EditInterval > 0 & SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Minute, ri.Content.DateCreated, DateTime.Now) > MainSettings.EditInterval))
            {
                var im = new Controls.InfoMessage
                             {
                                 Message = "<div class=\"afmessage\">" +  string.Format(GetSharedResource("[RESX:Message:EditIntervalReached]"), MainSettings.EditInterval.ToString()) + "</div>"
                             };
                plhMessage.Controls.Add(im);
                plhContent.Controls.Clear();
            }
            else
            {
                var sBody = ri.Content.Body;
                var sSubject = ri.Content.Subject;
                sBody = Utilities.PrepareForEdit(PortalId, ForumModuleId, ImagePath, sBody, _allowHTML, _editorType);
                sSubject = Utilities.PrepareForEdit(PortalId, ForumModuleId, ImagePath, sSubject, false, EditorTypes.TEXTBOX);
                ctlForm.Subject = sSubject;
                ctlForm.Body = sBody;
                ctlForm.IsApproved = ri.IsApproved;
                _contentId = ri.ContentId;
                _authorId = ri.Author.AuthorId;

               

                if (ri.Author.AuthorId > 0)
                    ctlForm.Subscribe = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, ri.Author.AuthorId);

                if (ri.Content.AuthorId != UserId && _canModApprove)
                    ctlForm.ShowModOptions = true;
            }
        }

        private void PrepareTopic()
        {
            string template;
            if (_fi.TopicFormId == 0)
            {
                var myFile = Request.MapPath(Common.Globals.ApplicationPath) + "\\DesktopModules\\ActiveForums\\config\\templates\\TopicEditor.txt";
                template = File.ReadAllText(myFile);
            }
            else
            {
                var tc = new TemplateController();
                var ti = tc.Template_Get(_fi.TopicFormId, PortalId, ForumModuleId);
                template = ti.TemplateHTML;
            }

            if (MainSettings.UseSkinBreadCrumb)
            {
                var sCrumb = "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + ForumInfo.ForumGroupId.ToString()) + "\">" + ForumInfo.GroupName + "</a>|";
                sCrumb += "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumInfo.ForumID.ToString()) + "\">" + ForumInfo.ForumName + "</a>";
                if (Environment.UpdateBreadCrumb(Page.Controls, sCrumb))
                    template = template.Replace("<div class=\"afcrumb\">[AF:LINK:FORUMMAIN] > [AF:LINK:FORUMGROUP] > [AF:LINK:FORUMNAME]</div>", string.Empty);
            }

            ctlForm.EditorMode = Modules.ActiveForums.Controls.SubmitForm.EditorModes.NewTopic;

            if (Permissions.HasPerm(_fi.Security.ModApprove, ForumUser.UserRoles))
            {
                ctlForm.ShowModOptions = true;
            }

            ctlForm.Template = template;
            ctlForm.IsApproved = _isApproved;

        }

        /// <summary>
        /// Prepares the post form for creating a reply.
        /// </summary>
        private void PrepareReply()
        {
            ctlForm.EditorMode = Modules.ActiveForums.Controls.SubmitForm.EditorModes.Reply;

            string template;
            if (_fi.ReplyFormId == 0)
            {
                var myFile = Request.MapPath(Common.Globals.ApplicationPath) + "\\DesktopModules\\ActiveForums\\config\\templates\\ReplyEditor.txt";
                template = File.ReadAllText(myFile);
            }
            else
            {
                var tc = new TemplateController();
                var ti = tc.Template_Get(_fi.ReplyFormId, PortalId, ForumModuleId);
                template = ti.TemplateHTML;
            }

            if (MainSettings.UseSkinBreadCrumb)
                template = template.Replace("<div class=\"afcrumb\">[AF:LINK:FORUMMAIN] > [AF:LINK:FORUMGROUP] > [AF:LINK:FORUMNAME]</div>", string.Empty);

            ctlForm.Template = template;
            if (!(TopicId > 0))
            {
                //Can't Find Topic
                var im = new InfoMessage { Message = GetSharedResource("[RESX:Message:LoadTopicFailed]") };
                plhContent.Controls.Add(im);
            }
            else if (!CanReply)
            {
                //No permission to reply
                var im = new InfoMessage { Message = GetSharedResource("[RESX:Message:AccessDenied]") };
                plhContent.Controls.Add(im);
            }
            else
            {
                var tc = new TopicsController();
                var ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, UserId, true);

                if(ti == null)
                    Response.Redirect(NavigateUrl(ForumTabId));

                ctlForm.Subject = Utilities.GetSharedResource("[RESX:SubjectPrefix]") + " " + ti.Content.Subject;
                ctlForm.TopicSubject = ti.Content.Subject;
                var body = string.Empty;

                if (ti.IsLocked && (CurrentUserType == CurrentUserTypes.Anon || CurrentUserType == CurrentUserTypes.Auth))
                    Response.Redirect(NavigateUrl(ForumTabId));

                if (Request.Params[ParamKeys.QuoteId] != null | Request.Params[ParamKeys.ReplyId] != null | Request.Params[ParamKeys.PostId] != null)
                {
                    //Setup form for Quote or Reply with body display
                    var isQuote = false;
                    var postId = 0;
                    var sPostedBy = Utilities.GetSharedResource("[RESX:PostedBy]") + " {0} {1} {2}";
                    if (Request.Params[ParamKeys.QuoteId] != null)
                    {
                        isQuote = true;
                        if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.QuoteId]))
                            postId = Convert.ToInt32(Request.Params[ParamKeys.QuoteId]);

                    }
                    else if (Request.Params[ParamKeys.ReplyId] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.ReplyId]))
                            postId = Convert.ToInt32(Request.Params[ParamKeys.ReplyId]);
                    }
                    else if (Request.Params[ParamKeys.PostId] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.PostId]))
                            postId = Convert.ToInt32(Request.Params[ParamKeys.PostId]);
                    }

                    if (postId != 0)
                    {
                        var userDisplay = MainSettings.UserNameDisplay;
                        if (_editorType == EditorTypes.TEXTBOX)
                            userDisplay = "none";

                        Content ci;
                        if (postId == TopicId)
                        {
                            ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId);
                            ci = ti.Content;
                            sPostedBy = string.Format(sPostedBy, UserProfiles.GetDisplayName(ForumModuleId, true, false, false, ti.Content.AuthorId, ti.Author.Username, ti.Author.FirstName, ti.Author.LastName, ti.Author.DisplayName), Utilities.GetSharedResource("On.Text"), GetServerDateTime(ti.Content.DateCreated));
                        }
                        else
                        {
                            var rc = new ReplyController();
                            var ri = rc.Reply_Get(PortalId, ForumModuleId, TopicId, postId);
                            ci = ri.Content;
                            sPostedBy = string.Format(sPostedBy, UserProfiles.GetDisplayName(ForumModuleId, true, false, false, ri.Content.AuthorId, ri.Author.Username, ri.Author.FirstName, ri.Author.LastName, ri.Author.DisplayName), Utilities.GetSharedResource("On.Text"), GetServerDateTime(ri.Content.DateCreated));
                        }

                        if (ci != null)
                            body = ci.Body;

                    }

                    if (_allowHTML && _editorType != EditorTypes.TEXTBOX)
                    {
                        if (body.ToUpper().Contains("<CODE") | body.ToUpper().Contains("[CODE]"))
                        {
                            var objCode = new CodeParser();
                            body = CodeParser.ParseCode(Utilities.HTMLDecode(body));
                        }
                    }
                    else
                    {
                        body = Utilities.PrepareForEdit(PortalId, ForumModuleId, ImagePath, body, _allowHTML, _editorType);
                    }

                    if (isQuote)
                    {
                        ctlForm.EditorMode = SubmitForm.EditorModes.Quote;
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
                        ctlForm.EditorMode = SubmitForm.EditorModes.ReplyWithBody;
                        body = sPostedBy + "<br />" + body;
                    }
                    ctlForm.Body = body;


                }
            }
            if (ctlForm.EditorMode != SubmitForm.EditorModes.EditReply && _canModApprove)
            {
                ctlForm.ShowModOptions = false;
            }
        }

        private void SaveTopic()
        {
            var subject = ctlForm.Subject;
            var body = ctlForm.Body;
            subject = Utilities.CleanString(PortalId, subject, false, EditorTypes.TEXTBOX, ForumInfo.UseFilter, false, ForumModuleId, _themePath, false);
            body = Utilities.CleanString(PortalId, body, _allowHTML, _editorType, ForumInfo.UseFilter, ForumInfo.AllowScript, ForumModuleId, _themePath, ForumInfo.AllowEmoticons);
            var summary = ctlForm.Summary;
            int authorId;
            string authorName;
            if (Request.IsAuthenticated)
            {
                authorId = UserInfo.UserID;
                switch (MainSettings.UserNameDisplay.ToUpperInvariant())
                {
                    case "USERNAME":
                        authorName = UserInfo.Username.Trim(' ');
                        break;
                    case "FULLNAME":
                        authorName = Convert.ToString(UserInfo.FirstName + " " + UserInfo.LastName).Trim(' ');
                        break;
                    case "FIRSTNAME":
                        authorName = UserInfo.FirstName.Trim(' ');
                        break;
                    case "LASTNAME":
                        authorName = UserInfo.LastName.Trim(' ');
                        break;
                    case "DISPLAYNAME":
                        authorName = UserInfo.DisplayName.Trim(' ');
                        break;
                    default:
                        authorName = UserInfo.DisplayName;
                        break;
                }
            }
            else
            {
                authorId = -1;
                authorName = Utilities.CleanString(PortalId, ctlForm.AuthorName, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, _themePath, false);
                if (authorName.Trim() == string.Empty)
                    return;
            }

            var tc = new TopicsController();
            TopicInfo ti;

            if (TopicId > 0)
            {
                ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId);
                ti.Content.DateUpdated = DateTime.Now;
                authorId = ti.Author.AuthorId;
            }
            else
            {
                ti = new TopicInfo();
                var dt = DateTime.Now;
                ti.Content.DateCreated = dt;
                ti.Content.DateUpdated = dt;
            }

            ti.AnnounceEnd = ctlForm.AnnounceEnd;
            ti.AnnounceStart = ctlForm.AnnounceStart;
            ti.Priority = ctlForm.TopicPriority;

            if (!_isEdit)
            {
                ti.Content.AuthorId = authorId;
                ti.Content.AuthorName = authorName;
                ti.Content.IPAddress = Request.UserHostAddress;
            }

            if (Regex.IsMatch(body, "<CODE([^>]*)>", RegexOptions.IgnoreCase))
            {
                foreach (Match m in Regex.Matches(body, "<CODE([^>]*)>(.*?)</CODE>", RegexOptions.IgnoreCase))
                    body = body.Replace(m.Value, m.Value.Replace("<br>", System.Environment.NewLine));
            }

            if (!(string.IsNullOrEmpty(ForumInfo.PrefixURL)))
            {
                var cleanSubject = Utilities.CleanName(subject).ToLowerInvariant();
                if (SimulateIsNumeric.IsNumeric(cleanSubject))
                    cleanSubject = "Topic-" + cleanSubject;

                var topicUrl = cleanSubject;
                var urlPrefix = "/";

                if (!(string.IsNullOrEmpty(ForumInfo.ForumGroup.PrefixURL)))
                    urlPrefix += ForumInfo.ForumGroup.PrefixURL + "/";

                if (!(string.IsNullOrEmpty(ForumInfo.PrefixURL)))
                    urlPrefix += ForumInfo.PrefixURL + "/";

                var urlToCheck = urlPrefix + cleanSubject;

                var topicsDb = new Data.Topics();
                for (var u = 0; u <= 200; u++)
                {
                    var tid = topicsDb.TopicIdByUrl(PortalId, ModuleId, urlToCheck);
                    if (tid > 0 && tid == TopicId)
                        break;

                    if (tid <= 0)
                        break;

                    topicUrl = (u + 1) + "-" + cleanSubject;
                    urlToCheck = urlPrefix + topicUrl;
                }
                if (topicUrl.Length > 150)
                {
                    topicUrl = topicUrl.Substring(0, 149);
                    topicUrl = topicUrl.Substring(0, topicUrl.LastIndexOf("-", StringComparison.Ordinal));
                }

                ti.TopicUrl = topicUrl;
            }
            else
            {
                //.URL = String.Empty
                ti.TopicUrl = string.Empty;
            }

            ti.Content.Body = body; //Utilities.CleanString(PortalId, Body, fi.AllowHTML, fi.EditorType, fi.UseFilter, fi.AllowScript, ForumModuleId, String.Empty)
            ti.Content.Subject = subject;
            ti.Content.Summary = summary;
            ti.IsAnnounce = ti.AnnounceEnd != Utilities.NullDate() && ti.AnnounceStart != Utilities.NullDate();
            
            if (_canModApprove && _fi.IsModerated)
                ti.IsApproved = ctlForm.IsApproved;
            else
                ti.IsApproved = _isApproved;

            bool bSend = ti.IsApproved;

            ti.IsArchived = false;
            ti.IsDeleted = false;
            ti.IsLocked = _canLock && ctlForm.Locked;
            ti.IsPinned = _canPin && ctlForm.Pinned;
            ti.StatusId = ctlForm.StatusId;
            ti.TopicIcon = ctlForm.TopicIcon;
            ti.TopicType = 0;
            if (ForumInfo.Properties != null)
            {
                var tData = new StringBuilder();
                tData.Append("<topicdata>");
                tData.Append("<properties>");
                foreach (var p in ForumInfo.Properties)
                {
                    var pkey = "afprop-" + p.PropertyId.ToString();

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
                SaveAttachments(ti.ContentId);
                if (ti.IsApproved && ti.Author.AuthorId > 0)
                {
                    var uc = new Data.Profiles();
                    uc.Profile_UpdateTopicCount(PortalId, ti.Author.AuthorId);
                }
            }

            if (Permissions.HasPerm(ForumInfo.Security.Tag, ForumUser.UserRoles))
            {
                DataProvider.Instance().Tags_DeleteByTopicId(PortalId, ForumModuleId, TopicId);
                var tagForm = string.Empty;
                if (Request.Form["txtTags"] != null)
                    tagForm = Request.Form["txtTags"];

                if (tagForm != string.Empty)
                {
                    var tags = tagForm.Split(',');
                    foreach (var tag in tags)
                    {
                        var sTag = Utilities.CleanString(PortalId, tag.Trim(), false, EditorTypes.TEXTBOX, false, false, ForumModuleId, string.Empty, false);
                        DataProvider.Instance().Tags_Save(PortalId, ForumModuleId, -1, sTag, 0, 1, 0, TopicId, false, -1, -1);
                    }
                }
            }

            if (Permissions.HasPerm(ForumInfo.Security.Categorize, ForumUser.UserRoles))
            {
                if (Request.Form["amaf-catselect"] != null)
                {
                    var cats = Request.Form["amaf-catselect"].Split(';');
                    DataProvider.Instance().Tags_DeleteTopicToCategory(PortalId, ForumModuleId, -1, TopicId);
                    foreach (var c in cats)
                    {
                        if (string.IsNullOrEmpty(c) || !SimulateIsNumeric.IsNumeric(c)) 
                            continue;

                        var cid = Convert.ToInt32(c);
                        if (cid > 0)
                            DataProvider.Instance().Tags_AddTopicToCategory(PortalId, ForumModuleId, cid, TopicId);
                    }
                }
            }

            if (!String.IsNullOrEmpty(ctlForm.PollQuestion) && !String.IsNullOrEmpty(ctlForm.PollOptions))
            {
                //var sPollQ = ctlForm.PollQuestion.Trim();
                //sPollQ = Utilities.CleanString(PortalId, sPollQ, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, string.Empty, false);
                var pollId = DataProvider.Instance().Poll_Save(-1, TopicId, UserId, ctlForm.PollQuestion.Trim(), ctlForm.PollType);
                if (pollId > 0)
                {
                    var options = ctlForm.PollOptions.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None);
               
                    foreach (string opt in options)
                    {
                        if (opt.Trim() != string.Empty)
                        {
                            var value = Utilities.CleanString(PortalId, opt, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, string.Empty, false);
                            DataProvider.Instance().Poll_Option_Save(-1, pollId, value.Trim(), TopicId);
                        }
                    }
                }

                ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, -1, false);
                ti.TopicType = TopicTypes.Poll;
                tc.TopicSave(PortalId, ti);
            }

            try
            {
                var cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                DataCache.CacheClearPrefix(cachekey);
                if (ctlForm.Subscribe && authorId == UserId)
                {
                    if (!(Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, authorId)))
                    {
                        var sc = new SubscriptionController();
                        sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, authorId, ForumUser.UserRoles);
                    }
                }
                else if (_isEdit)
                {
                    bool isSub = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, authorId);
                    if (isSub && !ctlForm.Subscribe)
                    {
                        var sc = new SubscriptionController();
                        sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, authorId, ForumUser.UserRoles);
                    }
                }

                if (bSend && !_isEdit)
                    Subscriptions.SendSubscriptions(PortalId, ForumModuleId, ForumTabId, _fi, TopicId, 0, ti.Content.AuthorId);

                if (ti.IsApproved == false)
                {
                    var mods = Utilities.GetListOfModerators(PortalId, ForumId);
                    var notificationType = NotificationsController.Instance.GetNotificationType("AF-ForumModeration");
                   
                    var notifySubject = Utilities.GetSharedResource("NotificationSubjectTopic");
                    notifySubject = notifySubject.Replace("[DisplayName]", UserInfo.DisplayName);
                    notifySubject = notifySubject.Replace("[TopicSubject]", ti.Content.Subject);
                   
                    var notifyBody = Utilities.GetSharedResource("NotificationBodyTopic");
                    notifyBody = notifyBody.Replace("[Post]", ti.Content.Body);
                    
                    var notificationKey = string.Format("{0}:{1}:{2}:{3}:{4}", TabId, ForumModuleId, ForumId, TopicId, ReplyId);

                    var notification = new Notification
                                           {
                                               NotificationTypeID = notificationType.NotificationTypeId,
                                               Subject = notifySubject,
                                               Body = notifyBody,
                                               IncludeDismissAction = false,
                                               SenderUserID = UserInfo.UserID,
                                               Context = notificationKey
                                           };

                    NotificationsController.Instance.SendNotification(notification, PortalId, null, mods);

                    string[] @params = { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=confirmaction", ParamKeys.ConfirmActionId + "=" + ConfirmActions.MessagePending };
                    Response.Redirect(NavigateUrl(ForumTabId, "", @params), false);
                }
                else
                {
                    if (ti != null)
                        ti.TopicId = TopicId;

                    var ctlUtils = new ControlUtils();

                    var sUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, ti.TopicUrl, -1, -1, string.Empty, 1, -1, SocialGroupId);
                    
                    if (sUrl.Contains("~/") || Request.QueryString["asg"] != null)
                        sUrl = Utilities.NavigateUrl(ForumTabId, "", ParamKeys.TopicId + "=" + TopicId);

                    if (!_isEdit)
                    {
                        try
                        {
                            var amas = new Social();
                            amas.AddTopicToJournal(PortalId, ForumModuleId, ForumId, TopicId, UserId, sUrl, subject, summary, body, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read, SocialGroupId);
                            if (Request.QueryString["asg"] == null && !(string.IsNullOrEmpty(MainSettings.ActiveSocialTopicsKey)) && ForumInfo.ActiveSocialEnabled)
                            {
                                // amas.AddTopicToJournal(PortalId, ForumModuleId, ForumId, UserId, sUrl, Subject, Summary, Body, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read)
                            }
                            else
                            {
                                amas.AddForumItemToJournal(PortalId, ForumModuleId, UserId, "forumtopic", sUrl, subject, body);
                            }
                        }
                        catch (Exception ex)
                        {
                            Services.Exceptions.Exceptions.LogException(ex);
                        }
                    }

                    Response.Redirect(sUrl, false);
                }
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
            }
        }

        private void SaveReply()
        {
            var subject = ctlForm.Subject;
            var body = ctlForm.Body;
            subject = Utilities.CleanString(PortalId, subject, false, EditorTypes.TEXTBOX, _fi.UseFilter, false, ForumModuleId, _themePath, false);
            body = Utilities.CleanString(PortalId, body, _allowHTML, _editorType, _fi.UseFilter, _fi.AllowScript, ForumModuleId, _themePath, _fi.AllowEmoticons);
			// This HTML decode is used to make Quote functionality work properly even when it appears in Text Box instead of Editor
            if (Request.Params[ParamKeys.QuoteId] != null)
            {
                body = Utilities.HTMLDecode(body);
            }
            int authorId;
            string authorName;
            if (Request.IsAuthenticated)
            {
                authorId = UserInfo.UserID;
                switch (MainSettings.UserNameDisplay.ToUpperInvariant())
                {
                    case "USERNAME":
                        authorName = UserInfo.Username.Trim(' ');
                        break;
                    case "FULLNAME":
                        authorName = Convert.ToString(UserInfo.FirstName + " " + UserInfo.LastName).Trim(' ');
                        break;
                    case "FIRSTNAME":
                        authorName = UserInfo.FirstName.Trim(' ');
                        break;
                    case "LASTNAME":
                        authorName = UserInfo.LastName.Trim(' ');
                        break;
                    case "DISPLAYNAME":
                        authorName = UserInfo.DisplayName.Trim(' ');
                        break;
                    default:
                        authorName = UserInfo.DisplayName;
                        break;
                }
            }
            else
            {
                authorId = -1;
                authorName = Utilities.CleanString(PortalId, ctlForm.AuthorName, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, _themePath, false);
                if (authorName.Trim() == string.Empty)
                    return;
            }

            var tc = new TopicsController();
            var rc = new ReplyController();
            ReplyInfo ri;
            
            if (PostId > 0)
            {
                ri = rc.Reply_Get(PortalId, ForumModuleId, TopicId, PostId);
                ri.Content.DateUpdated = DateTime.Now;
            }
            else
            {
                ri = new ReplyInfo();
                var dt = DateTime.Now;
                ri.Content.DateCreated = dt;
                ri.Content.DateUpdated = dt;
            }

            if (!_isEdit)
            {
                ri.Content.AuthorId = authorId;
                ri.Content.AuthorName = authorName;
                ri.Content.IPAddress = Request.UserHostAddress;
            }

            if (Regex.IsMatch(body, "<CODE([^>]*)>", RegexOptions.IgnoreCase))
            {
                foreach (Match m in Regex.Matches(body, "<CODE([^>]*)>(.*?)</CODE>", RegexOptions.IgnoreCase))
                    body = body.Replace(m.Value, m.Value.Replace("<br>", System.Environment.NewLine));
            }

            ri.Content.Body = body;
            ri.Content.Subject = subject;
            ri.Content.Summary = string.Empty;

            if (_canModApprove && ri.Content.AuthorId != UserId)
                ri.IsApproved = ctlForm.IsApproved;
            else
                ri.IsApproved = _isApproved;

            var bSend = ri.IsApproved;
            ri.IsDeleted = false;
            ri.StatusId = ctlForm.StatusId;
            ri.TopicId = TopicId;
            var tmpReplyId = rc.Reply_Save(PortalId, ri);
            ri = rc.Reply_Get(PortalId, ForumModuleId, TopicId, tmpReplyId);
            SaveAttachments(ri.ContentId);
            //tc.ForumTopicSave(ForumID, TopicId, ReplyId)
            var cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
            DataCache.CacheClearPrefix(cachekey);
            try
            {
                if (ctlForm.Subscribe && authorId == UserId)
                {
                    if (!(Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, authorId)))
                    {
                        var sc = new SubscriptionController();
                        sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, authorId, ForumUser.UserRoles);
                    }
                }
                else if (_isEdit)
                {
                    var isSub = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, authorId);
                    if (isSub && !ctlForm.Subscribe)
                    {
                        var sc = new SubscriptionController();
                        sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, authorId, ForumUser.UserRoles);
                    }
                }
                if (bSend && !_isEdit)
                {
                    Subscriptions.SendSubscriptions(PortalId, ForumModuleId, ForumTabId, _fi, TopicId, tmpReplyId, ri.Content.AuthorId);
                }
                if (ri.IsApproved == false)
                {
                    var ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId);

                    var mods = Utilities.GetListOfModerators(PortalId, ForumId);
                    var notificationType = NotificationsController.Instance.GetNotificationType("AF-ForumModeration");
                    var notifySubject = Utilities.GetSharedResource("NotificationSubjectReply");
                    notifySubject = notifySubject.Replace("[DisplayName]", UserInfo.DisplayName);
                    notifySubject = notifySubject.Replace("[TopicSubject]", ti.Content.Subject);
                    var notifyBody = Utilities.GetSharedResource("NotificationBodyReply");
                    notifyBody = notifyBody.Replace("[Post]", ri.Content.Body);
                    var notificationKey = string.Format("{0}:{1}:{2}:{3}:{4}", TabId, ForumModuleId, ForumId, TopicId, ri.ReplyId);

                    var notification = new Notification
                                           {
                                               NotificationTypeID = notificationType.NotificationTypeId,
                                               Subject = notifySubject,
                                               Body = notifyBody,
                                               IncludeDismissAction = false,
                                               SenderUserID = UserInfo.UserID,
                                               Context = notificationKey
                                           };

                    NotificationsController.Instance.SendNotification(notification, PortalId, null, mods);

                    string[] @params = { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=confirmaction", ParamKeys.ConfirmActionId + "=" + ConfirmActions.MessagePending };
                    Response.Redirect(Utilities.NavigateUrl(ForumTabId, "", @params), false);
                }
                else
                {
                    var ctlUtils = new ControlUtils();
                    var ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, -1, false);
                    var fullURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, ti.TopicUrl, -1, -1, string.Empty, 1, tmpReplyId, SocialGroupId);
                    
                    if (fullURL.Contains("~/") || Request.QueryString["asg"] != null)
                        fullURL = Utilities.NavigateUrl(ForumTabId, "", new[] { ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + tmpReplyId });
                    
                    if (fullURL.EndsWith("/"))
                        fullURL += "?" + ParamKeys.ContentJumpId + "=" + tmpReplyId;

                    if (!_isEdit)
                    {
                        try
                        {
                            var amas = new Social();
                            amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, TopicId, ReplyId, UserId, fullURL, subject, string.Empty, body, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read, SocialGroupId);
                            //If Request.QueryString["asg"] Is Nothing And Not String.IsNullOrEmpty(MainSettings.ActiveSocialTopicsKey) And ForumInfo.ActiveSocialEnabled And Not ForumInfo.ActiveSocialTopicsOnly Then
                            //    amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, TopicId, ReplyId, UserId, fullURL, Subject, String.Empty, Body, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read)
                            //Else
                            //    amas.AddForumItemToJournal(PortalId, ForumModuleId, UserId, "forumreply", fullURL, Subject, Body)
                            //End If


                        }
                        catch (Exception ex)
                        {
                            Services.Exceptions.Exceptions.LogException(ex);
                        }

                    }
                    Response.Redirect(fullURL);
                }
            }
            catch (Exception)
            {

            }
        }

        // Note attachments are currently saved into the authors file directory

        private void SaveAttachments(int contentId)
        {
            var fileManager = FileManager.Instance;
            var folderManager = FolderManager.Instance;
            var adb = new Data.AttachController();

            var userFolder = folderManager.GetUserFolder(UserInfo);

            const string uploadFolderName = "activeforums_Upload";
            const string attachmentFolderName = "activeforums_Attach";
            const string fileNameTemplate = "__{0}__{1}__{2}";

            var attachmentFolder = folderManager.GetFolder(PortalId, attachmentFolderName) ?? folderManager.AddFolder(PortalId, attachmentFolderName);

            // Read the attachment list sent in the hidden field as json
            var attachmentsJson = hidAttachments.Value;
            var serializer = new DataContractJsonSerializer(typeof (List<ClientAttachment>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(attachmentsJson));
            var attachmentsNew = (List<ClientAttachment>)serializer.ReadObject(ms);
            ms.Close();

            // Read the list of existing attachments for the content.  Must do this before saving any of the new attachments!
            // Ignore any legacy inline attachments
            var attachmentsOld = adb.ListForContent(contentId).Where(o => !o.AllowDownload.HasValue || o.AllowDownload.Value);

            // Save all of the new attachments
            foreach(var attachment in attachmentsNew)
            {
                // Don't need to do anything with existing attachments
                if(attachment.AttachmentId.HasValue && attachment.AttachmentId.Value > 0)
                    continue;

                IFileInfo file = null;
                
                var fileId = attachment.FileId.GetValueOrDefault();
                if(fileId > 0 && userFolder != null)
                {
                    // Make sure that the file exists and it actually belongs to the user who is trying to attach it
                    file = fileManager.GetFile(fileId);
                    if(file == null || file.FolderId != userFolder.FolderID) continue;
                }
                else if(!string.IsNullOrWhiteSpace(attachment.UploadId) && !string.IsNullOrWhiteSpace(attachment.FileName))
                {
                    if (!Regex.IsMatch(attachment.UploadId, @"^[\w\-. ]+$")) // Check for shenanigans.
                        continue;

                    var uploadFilePath = PathUtils.Instance.GetPhysicalPath(PortalId, uploadFolderName + "/" + attachment.UploadId);

                    if (!File.Exists(uploadFilePath))
                        continue;

                    // Store the files with a filename format that prevents overwrites.
                    var index = 0;
                    var fileName = string.Format(fileNameTemplate, contentId, index, Regex.Replace(attachment.FileName, @"[^\w\-. ]+", string.Empty));
                    while(fileManager.FileExists(attachmentFolder, fileName))
                    {
                        index++;
                        fileName = string.Format(fileNameTemplate, contentId, index, Regex.Replace(attachment.FileName, @"[^\w\-. ]+", string.Empty));
                    }

                    // Copy the file into the attachment folder with the correct name.
                    using (var fileStream = new FileStream(uploadFilePath, FileMode.Open, FileAccess.Read))
                    {
                        file = fileManager.AddFile(attachmentFolder, fileName, fileStream);
                    }
                    
                    File.Delete(uploadFilePath);
                }
                
                if(file == null)
                    continue;

                adb.Save(contentId, UserId, file.FileName, file.ContentType, file.Size, file.FileId);
            }

            // Remove any attachments that are no longer in the list of attachments
            var attachmentsToRemove = attachmentsOld.Where(a1 => attachmentsNew.All(a2 => a2.AttachmentId != a1.AttachmentId));
            foreach(var attachment in attachmentsToRemove)
            {
                adb.Delete(attachment.AttachmentId);

                var file = attachment.FileId.HasValue ? fileManager.GetFile(attachment.FileId.Value) : fileManager.GetFile(attachmentFolder, attachment.FileName);

                // Only delete the file if it exists in the attachment folder
                if(file != null && file.FolderId == attachmentFolder.FolderID)
                    fileManager.DeleteFile(file);
            }
        }

        private void PrepareAttachments(int? contentId = null)
        {
            // Handle the case where we don't yet have a topic id (new posts)
            if(!contentId.HasValue || contentId.Value <= 0)
            {
                hidAttachments.Value = "[]"; // JSON for an empty array
                return;
            }

            var adb = new Data.AttachController();
            var attachments = adb.ListForContent(contentId.Value);

            var clientAttachments = attachments.Select(attachment => new ClientAttachment
            {
                AttachmentId = attachment.AttachmentId, 
                ContentType = attachment.ContentType, 
                FileId = attachment.FileId, 
                FileName = Regex.Replace(attachment.FileName.TextOrEmpty(), @"^__\d+__\d+__", string.Empty), // Remove our unique file prefix before sending to the client.
                FileSize = attachment.FileSize
            }).ToList();

            var serializer = new DataContractJsonSerializer(typeof(List<ClientAttachment>)); 
            
            using(var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, clientAttachments);
                ms.Seek(0, 0);
                using(var sr = new StreamReader(ms, Encoding.UTF8))
                {
                    hidAttachments.Value = sr.ReadToEnd();
                }
            } 
        }

        #endregion
    }
}

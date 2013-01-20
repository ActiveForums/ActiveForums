//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke;
using System.Text.RegularExpressions;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.ActiveForums
{

    public partial class af_quickreplyform : ForumBase
    {
        #region Private Members
        private bool _CanReply = false;
        private string _ThemePath = string.Empty;
        private bool _UseFilter = true;
        private string _Subject = string.Empty;
        private bool _ModAppove = false;
        private bool _CanTrust = false;
        private bool _IsTrusted = false;
        private bool _TrustDefault = false;
        private bool _AllowHTML = false;
        private bool _AllowScripts = false;
        private bool _AllowSubscribe = false;
        private int _ForumId = -1;
        private bool _CanSubscribe = false;

        #endregion
        #region Public Members
        public string SubscribedChecked = string.Empty;
        public IClientCapability device = ClientCapabilityProvider.CurrentClientCapability;
        #endregion
        #region Public Properties
        public bool UseFilter
        {
            get
            {
                return _UseFilter;
            }
            set
            {
                _UseFilter = value;
            }
        }
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
        public bool ModApprove
        {
            get
            {
                return _ModAppove;
            }
            set
            {
                _ModAppove = value;
            }
        }
        public bool CanTrust
        {
            get
            {
                return _CanTrust;
            }
            set
            {
                _CanTrust = value;
            }
        }
        public bool IsTrusted
        {
            get
            {
                return _IsTrusted;
            }
            set
            {
                _IsTrusted = value;
            }
        }
        public bool TrustDefault
        {
            get
            {
                return _TrustDefault;
            }
            set
            {
                _TrustDefault = value;
            }
        }
        public bool AllowHTML
        {
            get
            {
                return _AllowHTML;
            }
            set
            {
                _AllowHTML = value;
            }
        }
        public bool AllowScripts
        {
            get
            {
                return _AllowScripts;
            }
            set
            {
                _AllowScripts = value;
            }
        }
        public bool AllowSubscribe
        {
            get
            {
                return _AllowSubscribe;
            }
            set
            {
                _AllowSubscribe = value;
            }
        }
        public bool CanSubscribe
        {
            get
            {
                return _CanSubscribe;
            }
            set
            {
                _CanSubscribe = value;
            }
        }
        #endregion
        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                //Me.AFModID = MID


                if (Request.IsAuthenticated)
                {
                    btnSubmitLink.OnClientClick = "afQuickSubmit(); return false;";

                    AllowSubscribe = Permissions.HasPerm(ForumInfo.Security.Subscribe, ForumUser.UserRoles);
                }
                else
                {
                    reqUserName.Enabled = true;
                    reqUserName.Text = "<img src=\"" + ImagePath + "/images/warning.png\" />";
                    reqBody.Text = "<img src=\"" + ImagePath + "/images/warning.png\" />";
                    reqSecurityCode.Text = "<img src=\"" + ImagePath + "/images/warning.png\" />";
                    btnSubmitLink.Click += ambtnSubmit_Click;

                    AllowSubscribe = false;
                }



                BoldText = Utilities.GetSharedResource("[RESX:Bold]");
                ItalicsText = Utilities.GetSharedResource("[RESX:Italics]");
                UnderlineText = Utilities.GetSharedResource("[RESX:Underline]");
                QuoteText = Utilities.GetSharedResource("[RESX:Quote]");
                BoldDesc = Utilities.GetSharedResource("[RESX:BoldDesc]");
                ItalicsDesc = Utilities.GetSharedResource("[RESX:ItalicsDesc]");
                UnderlineDesc = Utilities.GetSharedResource("[RESX:UnderlineDesc]");
                QuoteDesc = Utilities.GetSharedResource("[RESX:QuoteDesc]");
                CodeText = Utilities.GetSharedResource("[RESX:Code]");
                CodeDesc = Utilities.GetSharedResource("[RESX:CodeDesc]");
                ImageText = Utilities.GetSharedResource("[RESX:Image]");
                ImageDesc = Utilities.GetSharedResource("[RESX:ImageDesc]");

                if (UseFilter)
                {
                    btnToolBar.Visible = true;
                }
                else
                {
                    btnToolBar.Visible = false;
                }
                Subject = Utilities.GetSharedResource("[RESX:SubjectPrefix]") + " " + Subject;
                trSubscribe.Visible = AllowSubscribe;
                if (!Request.IsAuthenticated && CanReply)
                {
                    trUsername.Visible = true;
                    bolIsAnon = true;
                    trCaptcha.Visible = true;
                }
                else
                {
                    trUsername.Visible = false;
                    trCaptcha.Visible = false;
                    if (UserPrefTopicSubscribe || Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, this.UserId))
                    {
                        SubscribedChecked = " checked=true";
                    }
                }

                if (Utilities.InputIsValid(Request.Form["txtBody"]) && Request.IsAuthenticated & ((!(string.IsNullOrEmpty(Request.Form["hidReply1"])) && string.IsNullOrEmpty(Request.Form["hidReply2"])) | Request.Browser.IsMobileDevice))
                {
                    SaveQuickReply();
                }
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {

        }

        private bool bolIsAnon = false;
        public string txtBodyID;
        public string DisplayMode;
        public string BoldText;
        public string ItalicsText;
        public string UnderlineText;
        public string QuoteText;
        public string BoldDesc;
        public string ItalicsDesc;
        public string UnderlineDesc;
        public string QuoteDesc;
        public string CodeText;
        public string CodeDesc;
        public string ImageText;
        public string ImageDesc;
        public string SubmitText = Utilities.GetSharedResource("Submit.Text");


        //NOTE: The following placeholder declaration is required by the Web Form Designer.
        //Do not delete or move it.
        private object designerPlaceholderDeclaration;

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        #endregion
        private void SaveQuickReply()
        {
            SettingsInfo ms = DataCache.MainSettings(ForumModuleId);
            int iFloodInterval = MainSettings.FloodInterval;
            if (iFloodInterval > 0)
            {

                UserProfileInfo upi = ForumUser.Profile;
                if (upi != null)
                {
                    if (SimulateDateDiff.DateDiff(SimulateDateDiff.DateInterval.Second, upi.DateLastPost, DateTime.Now) < iFloodInterval)
                    {
                        Controls.InfoMessage im = new Controls.InfoMessage();
                        im.Message = "<div class=\"afmessage\">" + string.Format(GetSharedResource("[RESX:Error:FloodControl]"), iFloodInterval) + "</div>";
                        plhMessage.Controls.Add(im);
                        return;
                    }
                }
            }
            if (!Request.IsAuthenticated)
            {
                if ((!ctlCaptcha.IsValid) || txtUserName.Value == "")
                {
                    return;
                }
            }
            UserProfileInfo ui = new UserProfileInfo();
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
            bool UserIsTrusted = false;
            UserIsTrusted = Utilities.IsTrusted((int)ForumInfo.DefaultTrustValue, ui.TrustLevel, Permissions.HasPerm(ForumInfo.Security.Trust, ForumUser.UserRoles), ForumInfo.AutoTrustLevel, ui.PostCount);
            bool isApproved = false;
            isApproved = Convert.ToBoolean(((ForumInfo.IsModerated == true) ? false : true));
            if (UserIsTrusted || Permissions.HasPerm(ForumInfo.Security.ModApprove, ForumUser.UserRoles))
            {
                isApproved = true;
            }
            ReplyInfo ri = new ReplyInfo();
            ReplyController rc = new ReplyController();
            int ReplyId = -1;
            string sUsername = string.Empty;
            if (Request.IsAuthenticated)
            {
                switch (MainSettings.UserNameDisplay.ToUpperInvariant())
                {
                    case "USERNAME":
                        sUsername = UserInfo.Username.Trim(' ');
                        break;
                    case "FULLNAME":
                        sUsername = Convert.ToString(UserInfo.FirstName + " " + UserInfo.LastName).Trim(' ');
                        break;
                    case "FIRSTNAME":
                        sUsername = UserInfo.FirstName.Trim(' ');
                        break;
                    case "LASTNAME":
                        sUsername = UserInfo.LastName.Trim(' ');
                        break;
                    case "DISPLAYNAME":
                        sUsername = UserInfo.DisplayName.Trim(' ');
                        break;
                    default:
                        sUsername = UserInfo.DisplayName;
                        break;
                }

            }
            else
            {
                sUsername = Utilities.CleanString(PortalId, txtUserName.Value, false, EditorTypes.TEXTBOX, true, false, ForumModuleId, ThemePath, false);
            }

            //Dim sSubject As String = Server.HtmlEncode(Request.Form("txtSubject"))
            //If (UseFilter) Then
            //    sSubject = Utilities.FilterWords(PortalId,  ForumModuleId, ThemePath, sSubject)
            //End If
            string sBody = string.Empty;
            if (AllowHTML)
            {
                AllowHTML = isHTMLPermitted(ForumInfo.EditorPermittedUsers, IsTrusted, Permissions.HasPerm(ForumInfo.Security.ModEdit, ForumUser.UserRoles));
            }
            sBody = Utilities.CleanString(PortalId, Request.Form["txtBody"], AllowHTML, EditorTypes.TEXTBOX, UseFilter, AllowScripts, ForumModuleId, ThemePath, ForumInfo.AllowEmoticons);
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
            ri.Content.IPAddress = Request.UserHostAddress;
            ReplyId = rc.Reply_Save(PortalId, ri);
            //Check if is subscribed
            string cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
            DataCache.CacheClearPrefix(cachekey);


            // Subscribe or unsubscribe if needed
            if (AllowSubscribe && UserId > 0)
            {
                var subscribe = Request.Params["chkSubscribe"] == "1";
                var currentlySubscribed = Subscriptions.IsSubscribed(PortalId, ForumModuleId, ForumId, TopicId, SubscriptionTypes.Instant, UserId);

                if (subscribe != currentlySubscribed)
                {
                    // Will need to update this to support multiple subscrition types later
                    // Subscription_Update works as a toggle, so you only call it if you want to change the value.
                    var sc = new SubscriptionController();
                    sc.Subscription_Update(PortalId, ForumModuleId, ForumId, TopicId, 1, UserId, ForumUser.UserRoles);
                }
            }



            ControlUtils ctlUtils = new ControlUtils();
            TopicsController tc = new TopicsController();
            TopicInfo ti = tc.Topics_Get(PortalId, ForumModuleId, TopicId, ForumId, -1, false);
            string fullURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, ti.TopicUrl, -1, -1, string.Empty, 1, SocialGroupId);

            if (fullURL.Contains("~/") || Request.QueryString["asg"] != null)
            {
                fullURL = Utilities.NavigateUrl(TabId, "", new string[] { ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId });
            }
            if (fullURL.EndsWith("/"))
            {
                fullURL += "?" + ParamKeys.ContentJumpId + "=" + ReplyId;
            }
            if (isApproved)
            {

                //Send Subscriptions

                try
                {
                    //Dim sURL As String = Utilities.NavigateUrl(TabId, "", New String() {ParamKeys.ForumId & "=" & ForumId, ParamKeys.ViewType & "=" & Views.Topic, ParamKeys.TopicId & "=" & TopicId, ParamKeys.ContentJumpId & "=" & ReplyId})
                    Subscriptions.SendSubscriptions(PortalId, ForumModuleId, TabId, ForumId, TopicId, ReplyId, UserId);
                    try
                    {
                        Social amas = new Social();
                        amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, TopicId, ReplyId, UserId, fullURL, Subject, string.Empty, sBody, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read, SocialGroupId);
                        //If Request.QueryString["asg"] Is Nothing And Not String.IsNullOrEmpty(MainSettings.ActiveSocialTopicsKey) And ForumInfo.ActiveSocialEnabled And Not ForumInfo.ActiveSocialTopicsOnly Then
                        //    amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, TopicId, ReplyId, UserId, fullURL, Subject, String.Empty, sBody, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read)
                        //Else
                        //    amas.AddForumItemToJournal(PortalId, ForumModuleId, UserId, "forumreply", fullURL, Subject, sBody)
                        //End If

                    }
                    catch (Exception ex)
                    {
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                    }
                }
                catch (Exception ex)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
                }
                //Redirect to show post

                Response.Redirect(fullURL, false);
            }
            else if (isApproved == false)
            {
                Email oEmail = new Email();
                List<Entities.Users.UserInfo> mods = Utilities.GetListOfModerators(PortalId, ForumId);
                NotificationType notificationType = NotificationsController.Instance.GetNotificationType("AF-ForumModeration");
                string subject = Utilities.GetSharedResource("NotificationSubjectReply");
                subject = subject.Replace("[DisplayName]", UserInfo.DisplayName);
                subject = subject.Replace("[TopicSubject]", ti.Content.Subject);
                string body = Utilities.GetSharedResource("NotificationBodyReply");
                body = body.Replace("[Post]", sBody);
                string notificationKey = string.Format("{0}:{1}:{2}:{3}:{4}", TabId, ForumModuleId, ForumId, TopicId, ReplyId);

                Notification notification = new Notification();
                notification.NotificationTypeID = notificationType.NotificationTypeId;
                notification.Subject = subject;
                notification.Body = body;
                notification.IncludeDismissAction = false;
                notification.SenderUserID = UserInfo.UserID;
                notification.Context = notificationKey;


                NotificationsController.Instance.SendNotification(notification, PortalId, null, mods);


                //oEmail.SendEmailToModerators(ForumInfo.ModNotifyTemplateId, PortalId, ForumId, ri.TopicId, ReplyId, ForumModuleId, TabId, String.Empty)
                string[] Params = { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=confirmaction", "afmsg=pendingmod", ParamKeys.TopicId + "=" + TopicId };
                if (SocialGroupId > 0)
                {
                    Params = Utilities.AddParams("GroupId=" + SocialGroupId, Params);
                }
                Response.Redirect(Utilities.NavigateUrl(TabId, "", Params), false);
            }
            else
            {
                //Dim fullURL As String = Utilities.NavigateUrl(TabId, "", New String() {ParamKeys.ForumId & "=" & ForumId, ParamKeys.ViewType & "=" & Views.Topic, ParamKeys.TopicId & "=" & TopicId, ParamKeys.ContentJumpId & "=" & ReplyId})
                //If MainSettings.UseShortUrls Then
                //    fullURL = Utilities.NavigateUrl(TabId, "", New String() {ParamKeys.TopicId & "=" & TopicId, ParamKeys.ContentJumpId & "=" & ReplyId})
                //End If

                try
                {
                    Social amas = new Social();
                    amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, TopicId, ReplyId, UserId, fullURL, Subject, string.Empty, sBody, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read, SocialGroupId);
                    //If Request.QueryString["asg"] Is Nothing And Not String.IsNullOrEmpty(MainSettings.ActiveSocialTopicsKey) And ForumInfo.ActiveSocialEnabled Then
                    //    amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, TopicId, ReplyId, UserId, fullURL, Subject, String.Empty, sBody, ForumInfo.ActiveSocialSecurityOption, ForumInfo.Security.Read)
                    //Else
                    //    amas.AddForumItemToJournal(PortalId, ForumModuleId, UserId, "forumreply", fullURL, Subject, sBody)
                    //End If

                }
                catch (Exception ex)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                }
                Response.Redirect(fullURL, false);
            }

            //End If


        }


        private void ambtnSubmit_Click(object sender, System.EventArgs e)
        {

            Page.Validate();
            bool tmpVal = true;
            if (Utilities.InputIsValid(Request.Form["txtBody"].Trim()) == false)
            {
                reqBody.Visible = true;
                tmpVal = false;
            }
            if (!Request.IsAuthenticated && Utilities.InputIsValid(txtUserName.Value.Trim()) == false)
            {
                reqUserName.Visible = true;
                tmpVal = false;
            }
            if (!ctlCaptcha.IsValid)
            {
                reqSecurityCode.Visible = true;
            }
            if (Page.IsValid && tmpVal)
            {
                SaveQuickReply();
            }

        }
    }

}

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

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.Social.Notifications;


namespace DotNetNuke.Modules.ActiveForums
{

    public partial class af_modreport : ForumBase
    {


        #region Controls
        #endregion

        #region Event Handlers
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {

                if (Request.IsAuthenticated)
                {

                    string strReasons = GetSharedResource("[RESX:ReasonOptions]"); //.GetString("ReasonOptions", "~/DesktopModules/ActiveForums/App_LocalResources/af_modalert")
                    int i = 0;
                    foreach (string strReason in strReasons.Split(new char[] { ';' }))
                    {
                        if (!(strReason == ""))
                        {
                            drpReasons.Items.Insert(i, new ListItem(strReason, strReason));
                            i += 1;
                        }
                    }

                }
                else
                {
                    Response.Redirect(NavigateUrl(TabId));
                }


            }
            catch (Exception exc)
            {
                //DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(Me, exc, False)
            }
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            base.Render(htmlWriter);
            string html = stringWriter.ToString();
            html = html.Replace("[AF:LINK:FORUMMAIN]", "<a href=\"" + NavigateUrl(TabId) + "\">[RESX:FORUMS]</a>");
            html = html.Replace("[AF:LINK:FORUMGROUP]", "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + ForumInfo.ForumGroupId) + "\">" + ForumInfo.GroupName + "</a>");
            html = html.Replace("[AF:LINK:FORUMNAME]", "<a href=\"" + NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topics }) + "\">" + ForumInfo.ForumName + "</a>");

            html = Utilities.LocalizeControl(html);

            writer.Write(html);
        }
        #endregion

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }
        protected Panel pnlMessage;

        //NOTE: The following placeholder declaration is required by the Web Form Designer.
        //Do not delete or move it.
        private object designerPlaceholderDeclaration;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            this.LocalResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/af_modalert.ascx.resx";
            InitializeComponent();

            btnSend.Click += new System.EventHandler(btnSend_Click);
            btnCancel.Click += new System.EventHandler(btnCancel_Click);

        }

        #endregion


        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId }));
        }

        private void btnSend_Click(object sender, System.EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                Email objEmail = new Email();
                string Comments = null;
                Comments = drpReasons.SelectedItem.Value + "<br>";
                Comments += Utilities.CleanString(PortalId, txtComments.Text, false, EditorTypes.TEXTBOX, false, false, ModuleId, string.Empty, false);
                int templateId = 0;
                TemplateController tc = new TemplateController();
                TemplateInfo ti = tc.Template_Get("ModAlert", PortalId, ModuleId);
                string sUrl;
                if (SocialGroupId > 0) sUrl = NavigateUrl(Convert.ToInt32(Request.QueryString["TabId"]), "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=confirmaction", ParamKeys.ConfirmActionId + "=" + ConfirmActions.AlertSent + "&" + ParamKeys.GroupIdName + "=" + SocialGroupId });
                else sUrl = NavigateUrl(Convert.ToInt32(Request.QueryString["TabId"]), "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=confirmaction", ParamKeys.ConfirmActionId + "=" + ConfirmActions.AlertSent });

                NotificationType notificationType = NotificationsController.Instance.GetNotificationType("AF-ContentAlert");
                TopicsController topicController = new TopicsController();
                TopicInfo topic = topicController.Topics_Get(PortalId, ModuleId, TopicId, ForumId, -1, false);
                string sBody = string.Empty;
                string authorName = string.Empty;
                string sSubject = string.Empty;
                string sTopicURL = string.Empty;
                sTopicURL = topic.TopicUrl;
                if (ReplyId > 0 & TopicId != ReplyId)
                {
                    ReplyController rc = new ReplyController();
                    ReplyInfo reply = rc.Reply_Get(PortalId, ModuleId, TopicId, ReplyId);
                    sBody = reply.Content.Body;
                    sSubject = reply.Content.Subject;
                    authorName = reply.Author.DisplayName;

                }
                else
                {
                    sBody = topic.Content.Body;
                    sSubject = topic.Content.Subject;
                    authorName = topic.Author.DisplayName;

                }
                ControlUtils ctlUtils = new ControlUtils();
                string fullURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, ForumInfo.ForumGroup.PrefixURL, ForumInfo.PrefixURL, ForumInfo.ForumGroupId, ForumInfo.ForumID, TopicId, sTopicURL, -1, -1, string.Empty, 1, ReplyId, SocialGroupId);
                string subject = Utilities.GetSharedResource("AlertSubject");
                subject = subject.Replace("[DisplayName]", authorName);
                subject = subject.Replace("[Subject]", sSubject);
                subject = subject.Replace("[FlaggedBy]", UserInfo.DisplayName);
                string body = Utilities.GetSharedResource("AlertBody");
                body = body.Replace("[Post]", sBody);
                body = body.Replace("[Comment]", Comments);
                body = body.Replace("[URL]", fullURL);
                body = body.Replace("[Reason]", drpReasons.SelectedItem.Value);
                List<Entities.Users.UserInfo> mods = Utilities.GetListOfModerators(PortalId, ForumId);


                string notificationKey = string.Format("{0}:{1}:{2}:{3}:{4}", TabId, ForumModuleId, ForumId, TopicId, ReplyId);

                Notification notification = new Notification();
                notification.NotificationTypeID = notificationType.NotificationTypeId;
                notification.Subject = subject;
                notification.Body = body;
                notification.IncludeDismissAction = false;
                notification.SenderUserID = UserInfo.UserID;
                notification.Context = notificationKey;


                NotificationsController.Instance.SendNotification(notification, PortalId, null, mods);



                Response.Redirect(sUrl);
            }
        }
    }

}

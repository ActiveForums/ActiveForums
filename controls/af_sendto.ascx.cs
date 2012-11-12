using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_sendto : ForumBase
    {
        private bool bcUpdated = false;
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            btnCancel.Click += new System.EventHandler(btnCancel_Click);
            btnSend.Click += new System.EventHandler(btnSend_Click);
            string warnImg = "<img src=\"" + ImagePath + "/images/warning.png\" />";
            reqEmail.Text = warnImg;
            reqMessage.Text = warnImg;
            reqName.Text = warnImg;
            reqSubject.Text = warnImg;
            regEmail.Text = warnImg;
            regEmail.ValidationExpression = "\\b[a-zA-Z0-9._%\\-+']+@[a-zA-Z0-9.\\-]+\\.[a-zA-Z]{2,4}\\b";
            string TopicSubject = string.Empty;
            if (TopicId > 0)
            {
                TopicsController tc = new TopicsController();
                TopicInfo ti = tc.Topics_Get(PortalId, ModuleId, TopicId, ForumId, UserId, true);
                if (ti != null)
                {
                    if (Permissions.HasPerm(ForumInfo.Security.Read, ForumUser.UserRoles))
                    {
                        if (!Page.IsPostBack)
                        {
                            string SubjectDefault = GetSharedResource("[RESX:EmailSubjectDefault]");
                            TopicSubject = ti.Content.Subject;
                            SubjectDefault = SubjectDefault.Replace("[SUBJECT]", ti.Content.Subject);

                            txtRecipSubject.Text = SubjectDefault;
                            string MessageDefault = GetSharedResource("[RESX:EmailMessageDefault]");
                            string sURL = NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId });
                            if (MainSettings.UseShortUrls)
                            {
                                sURL = NavigateUrl(TabId, "", new string[] { ParamKeys.TopicId + "=" + TopicId });
                            }
                            MessageDefault = MessageDefault.Replace("[TOPICLINK]", sURL);
                            MessageDefault = MessageDefault.Replace("[DISPLAYNAME]", UserProfiles.GetDisplayName(ModuleId, UserId, MainSettings.UserNameDisplay, UserInfo.Username, UserInfo.FirstName, UserInfo.LastName, UserInfo.DisplayName));
                            txtMessage.Text = MessageDefault;
                        }

                    }
                    if (MainSettings.UseSkinBreadCrumb)
                    {
                        string sCrumb = "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + ForumGroupId) + "\">" + ForumInfo.GroupName + "</a>|";
                        if (MainSettings.UseShortUrls)
                        {
                            sCrumb += "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ForumId + "=" + ForumId) + "\">" + ForumInfo.ForumName + "</a>";
                            sCrumb += "|<a href=\"" + NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + TopicId) + "\">" + TopicSubject + "</a>";
                        }
                        else
                        {
                            sCrumb += "<a href=\"" + NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topics }) + "\">" + ForumInfo.ForumName + "</a>";
                            sCrumb += "|<a href=\"" + NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId }) + "\">" + TopicSubject + "</a>";
                        }
                        if (Environment.UpdateBreadCrumb(Page.Controls, sCrumb))
                        {
                            bcUpdated = true;
                        }

                    }
                }
                else
                {
                    Response.Redirect(NavigateUrl(TabId));
                }
            }
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            base.Render(htmlWriter);
            string html = stringWriter.ToString();
            if (bcUpdated)
            {
                html = html.Replace("<div class=\"afcrumb\">[AF:LINK:FORUMMAIN] > [AF:LINK:FORUMGROUP] > [AF:LINK:FORUMNAME]</div>", string.Empty);
            }
            else
            {
                html = html.Replace("[AF:LINK:FORUMMAIN]", "<a href=\"" + NavigateUrl(TabId) + "\">[RESX:FORUMS]</a>");
                html = html.Replace("[AF:LINK:FORUMGROUP]", "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + ForumInfo.ForumGroupId) + "\">" + ForumInfo.GroupName + "</a>");
                html = html.Replace("[AF:LINK:FORUMNAME]", "<a href=\"" + NavigateUrl(TabId, "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topics }) + "\">" + ForumInfo.ForumName + "</a>");
            }


            html = Utilities.LocalizeControl(html);

            writer.Write(html);
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            Response.Redirect(NavigateUrl(Convert.ToInt32(Request.QueryString["TabId"]), "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId }));
        }

        private void btnSend_Click(object sender, System.EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                string sSubject = txtRecipSubject.Text;
                string sEmail = txtRecipEmail.Text;
                string sEmailName = txtRecipName.Text;
                string sMessage = txtMessage.Text;
                sSubject = Utilities.CleanString(PortalId, sSubject.Trim(), false, EditorTypes.TEXTBOX, false, false, ModuleId, string.Empty, false);
                sMessage = Utilities.CleanString(PortalId, sMessage.Trim(), false, EditorTypes.TEXTBOX, false, false, ModuleId, string.Empty, false);
                string sUrl = NavigateUrl(Convert.ToInt32(Request.QueryString["TabId"]), "", new string[] { ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ViewType + "=confirmaction", ParamKeys.ConfirmActionId + "=" + ConfirmActions.SendToComplete });
                try
                {
                    Email oEmail = new Email();
                    if (!(sMessage == string.Empty) && !(sSubject == string.Empty))
                    {
                        oEmail.SendNotification(UserInfo.Email, sEmail, sSubject, sMessage, sMessage.Replace(System.Environment.NewLine, "<br />"));
                    }


                }
                catch (Exception ex)
                {
                    //Response.Redirect(NavigateUrl(CInt(Request.QueryString["TabId"]), "", New String() {ParamKeys.ForumId & "=" & ForumId, ParamKeys.TopicId & "=" & TopicId, ParamKeys.ViewType & "=confirmaction", ParamKeys.ConfirmActionId & "=" & ConfirmActions.SendToFailed}))
                }
                Response.Redirect(sUrl);


            }
        }
    }
}

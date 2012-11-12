using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_modtopics_new : ForumBase
    {
        #region Private Members
        private bool bModDelete = false;
        private bool bModEdit = false;
        private bool bModApprove = false;
        private bool bModMove = false;
        private bool bCanMod = false;

        #endregion
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cbMod.CallbackEvent += cbMod_Callback;

        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (Request.IsAuthenticated && ForumUser.Profile.IsMod)
            {
                if (ForumId > 0)
                {
                    SetPermissions(ForumId);
                }


                if (!cbMod.IsCallback)
                {
                    lblHeader.Text = Utilities.GetSharedResource("[RESX:PendingPosts]");
                    BuildModList();
                }

            }
            else
            {
                Response.Redirect(NavigateUrl(ForumTabId));
            }

        }
        private void cbMod_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            SettingsInfo ms = DataCache.MainSettings(ForumModuleId);
            Forum fi = null;
            if (e.Parameters.Length > 0)
            {
                if (ForumId < 1)
                {
                    SetPermissions(Convert.ToInt32(e.Parameters[1]));
                    ForumController fc = new ForumController();
                    fi = fc.Forums_Get(Convert.ToInt32(e.Parameters[1]), -1, false, true);
                }
                else
                {
                    fi = ForumInfo;
                }
                switch (e.Parameters[0].ToLowerInvariant())
                {
                    case "moddel":
                        {
                            if (bModDelete)
                            {
                                int delAction = ms.DeleteBehavior;
                                int tmpForumId = -1;
                                int tmpTopicId = -1;
                                int tmpReplyId = -1;
                                tmpForumId = Convert.ToInt32(e.Parameters[1]);
                                tmpTopicId = Convert.ToInt32(e.Parameters[2]);
                                tmpReplyId = Convert.ToInt32(e.Parameters[3]);
                                Author auth = null;
                                if (fi.ModDeleteTemplateId > 0)
                                {
                                    try
                                    {
                                        Email oEmail = new Email();
                                        oEmail.SendEmail(fi.ModDeleteTemplateId, PortalId, ForumModuleId, ForumTabId, tmpForumId, tmpTopicId, tmpReplyId, string.Empty, auth);
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                                if (tmpForumId > 0 & tmpTopicId > 0 && tmpReplyId == 0)
                                {
                                    TopicsController tc = new TopicsController();
                                    TopicInfo ti = tc.Topics_Get(PortalId, ForumModuleId, tmpTopicId);
                                    if (ti != null)
                                    {
                                        auth = ti.Author;
                                    }
                                    tc.Topics_Delete(PortalId, ModuleId, tmpForumId, tmpTopicId, delAction);
                                }
                                else if (tmpForumId > 0 & tmpTopicId > 0 & tmpReplyId > 0)
                                {
                                    ReplyController rc = new ReplyController();
                                    ReplyInfo ri = rc.Reply_Get(PortalId, ForumModuleId, tmpTopicId, tmpReplyId);
                                    if (ri != null)
                                    {
                                        auth = ri.Author;
                                    }
                                    rc.Reply_Delete(PortalId, tmpForumId, tmpTopicId, tmpReplyId, delAction);
                                }

                            }
                            break;
                        }
                    case "modreject":
                        {
                            int tmpForumId = 0;
                            int tmpTopicId = 0;
                            int tmpReplyId = 0;
                            int tmpAuthorId = 0;
                            tmpForumId = Convert.ToInt32(e.Parameters[1]);
                            tmpTopicId = Convert.ToInt32(e.Parameters[2]);
                            tmpReplyId = Convert.ToInt32(e.Parameters[3]);
                            tmpAuthorId = Convert.ToInt32(e.Parameters[4]);
                            ModController mc = new ModController();
                            mc.Mod_Reject(PortalId, ForumModuleId, UserId, tmpForumId, tmpTopicId, tmpReplyId);
                            if (fi.ModRejectTemplateId > 0 & tmpAuthorId > 0)
                            {
                                DotNetNuke.Entities.Users.UserController uc = new DotNetNuke.Entities.Users.UserController();
                                DotNetNuke.Entities.Users.UserInfo ui = uc.GetUser(PortalId, tmpAuthorId);
                                if (ui != null)
                                {
                                    Author au = new Author();
                                    au.AuthorId = tmpAuthorId;
                                    au.DisplayName = ui.DisplayName;
                                    au.Email = ui.Email;
                                    au.FirstName = ui.FirstName;
                                    au.LastName = ui.LastName;
                                    au.Username = ui.Username;
                                    Email oEmail = new Email();
                                    oEmail.SendEmail(fi.ModRejectTemplateId, PortalId, ForumModuleId, ForumTabId, tmpForumId, tmpTopicId, tmpReplyId, string.Empty, au);
                                }

                            }

                            break;
                        }
                    case "modappr":
                        {
                            int tmpForumId = -1;
                            int tmpTopicId = -1;
                            int tmpReplyId = -1;
                            tmpForumId = Convert.ToInt32(e.Parameters[1]);
                            tmpTopicId = Convert.ToInt32(e.Parameters[2]);
                            tmpReplyId = Convert.ToInt32(e.Parameters[3]);
                            string sSubject = string.Empty;
                            string sBody = string.Empty;
                            if (tmpForumId > 0 & tmpTopicId > 0 && tmpReplyId == 0)
                            {
                                TopicsController tc = new TopicsController();
                                TopicInfo ti = tc.Topics_Get(PortalId, ForumModuleId, tmpTopicId, tmpForumId, -1, false);
                                if (ti != null)
                                {
                                    sSubject = ti.Content.Subject;
                                    sBody = ti.Content.Body;
                                    ti.IsApproved = true;
                                    tc.TopicSave(PortalId, ti);
                                    tc.Topics_SaveToForum(tmpForumId, tmpTopicId, PortalId, ModuleId);
                                    //TODO: Add Audit log for who approved topic
                                    if (fi.ModApproveTemplateId > 0 & ti.Author.AuthorId > 0)
                                    {
                                        Email oEmail = new Email();
                                        oEmail.SendEmail(fi.ModApproveTemplateId, PortalId, ForumModuleId, ForumTabId, tmpForumId, tmpTopicId, tmpReplyId, string.Empty, ti.Author);
                                    }

                                    Subscriptions.SendSubscriptions(PortalId, ForumModuleId, ForumTabId, tmpForumId, tmpTopicId, 0, ti.Content.AuthorId);

                                    try
                                    {
                                        ControlUtils ctlUtils = new ControlUtils();
                                        string sUrl = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, fi.ForumGroup.PrefixURL, fi.PrefixURL, fi.ForumGroupId, fi.ForumID, TopicId, ti.TopicUrl, -1, -1, string.Empty, 1, fi.SocialGroupId); // Utilities.NavigateUrl(ForumTabId, "", ParamKeys.ViewType & "=" & Views.Topic & "&" & ParamKeys.ForumId & "=" & ForumId, ParamKeys.TopicId & "=" & TopicId)
                                        if (sUrl.Contains("~/") || Request.QueryString["asg"] != null)
                                        {
                                            sUrl = Utilities.NavigateUrl(ForumTabId, "", ParamKeys.TopicId + "=" + TopicId);
                                        }
                                        Social amas = new Social();
                                        if (Request.QueryString["asg"] == null & !(string.IsNullOrEmpty(MainSettings.ActiveSocialTopicsKey)) && fi.ActiveSocialEnabled)
                                        {
                                            amas.AddTopicToJournal(PortalId, ForumModuleId, ForumId, ti.TopicId, ti.Author.AuthorId, sUrl, sSubject, ti.Content.Summary, sBody, fi.ActiveSocialSecurityOption, fi.Security.Read, SocialGroupId);
                                        }
                                        else
                                        {
                                            amas.AddForumItemToJournal(PortalId, ForumModuleId, ti.Author.AuthorId, "forumtopic", sUrl, sSubject, sBody);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                                    }
                                }
                            }
                            else if (tmpForumId > 0 & tmpTopicId > 0 & tmpReplyId > 0)
                            {
                                ReplyController rc = new ReplyController();
                                ReplyInfo ri = rc.Reply_Get(PortalId, ForumModuleId, tmpTopicId, tmpReplyId);
                                if (ri != null)
                                {
                                    ri.IsApproved = true;
                                    sSubject = ri.Content.Subject;
                                    sBody = ri.Content.Body;
                                    rc.Reply_Save(PortalId, ri);
                                    TopicsController tc = new TopicsController();
                                    tc.Topics_SaveToForum(tmpForumId, tmpTopicId, PortalId, ModuleId, tmpReplyId);
                                    TopicInfo ti = tc.Topics_Get(PortalId, ForumModuleId, tmpTopicId, tmpForumId, -1, false);
                                    //TODO: Add Audit log for who approved topic
                                    if (fi.ModApproveTemplateId > 0 & ri.Author.AuthorId > 0)
                                    {
                                        Email oEmail = new Email();
                                        oEmail.SendEmail(fi.ModApproveTemplateId, PortalId, ForumModuleId, ForumTabId, tmpForumId, tmpTopicId, tmpReplyId, string.Empty, ri.Author);
                                    }

                                    Subscriptions.SendSubscriptions(PortalId, ForumModuleId, ForumTabId, tmpForumId, tmpTopicId, tmpReplyId, ri.Content.AuthorId);

                                    try
                                    {
                                        ControlUtils ctlUtils = new ControlUtils();
                                        string fullURL = ctlUtils.BuildUrl(ForumTabId, ForumModuleId, fi.ForumGroup.PrefixURL, fi.PrefixURL, fi.ForumGroupId, fi.ForumID, TopicId, ti.TopicUrl, -1, -1, string.Empty, 1, fi.SocialGroupId);
                                        if (fullURL.Contains("~/") || Request.QueryString["asg"] != null)
                                        {
                                            fullURL = Utilities.NavigateUrl(ForumTabId, "", new string[] { ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + tmpReplyId });
                                        }
                                        Social amas = new Social();
                                        if (Request.QueryString["asg"] == null & !(string.IsNullOrEmpty(MainSettings.ActiveSocialTopicsKey)) && fi.ActiveSocialEnabled && !fi.ActiveSocialTopicsOnly)
                                        {
                                            amas.AddReplyToJournal(PortalId, ForumModuleId, ForumId, ri.TopicId, ri.ReplyId, ri.Author.AuthorId, fullURL, ri.Content.Subject, string.Empty, sBody, fi.ActiveSocialSecurityOption, fi.Security.Read, fi.SocialGroupId);
                                        }
                                        else
                                        {
                                            amas.AddForumItemToJournal(PortalId, ForumModuleId, ri.Author.AuthorId, "forumreply", fullURL, sSubject, sBody);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                                    }

                                }

                            }


                            break;
                        }
                }
                string cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                DataCache.CacheClearPrefix(cachekey);
            }
            BuildModList();
            litTopics.RenderControl(e.Output);
        }
        #endregion
        #region Private Members
        private void BuildModList()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            DataSet ds = DataProvider.Instance().Mod_Pending(PortalId, ModuleId, ForumId, UserId);
            DataTable dtContent = ds.Tables[0];
            DataTable dtAttach = ds.Tables[1];
            string tmpForum = string.Empty;

            sb.Append("<div id=\"afgrid\" style=\"position:relative;\"><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\">");
            foreach (DataRow dr in dtContent.Rows)
            {
                string forumKey = dr["ForumId"].ToString() + dr["ForumName"].ToString();
                if (forumKey != tmpForum)
                {
                    if (ForumId == -1)
                    {
                        SetPermissions(Convert.ToInt32(dr["ForumId"]));
                    }
                    if (bCanMod)
                    {
                        if (!(tmpForum == string.Empty))
                        {
                            sb.Append("</td></tr>");
                        }
                        int pendingCount = 0;
                        dtContent.DefaultView.RowFilter = "ForumId = " + Convert.ToInt32(dr["ForumId"]);
                        pendingCount = dtContent.DefaultView.ToTable().Rows.Count;
                        dtContent.DefaultView.RowFilter = "";
                        sb.Append("<tr><td class=\"afgrouprow\" style=\"padding-left:10px;\">" + dr["GroupName"].ToString() + " > " + dr["ForumName"].ToString() + " [RESX:Pending]: (" + pendingCount + ")</td><td class=\"afgrouprow\" align=\"right\" style=\"padding-right:5px;\">");
                        if (ForumId == -1)
                        {
                            sb.Append("<img align=\"absmiddle\" class=\"afarrow\" id=\"imgSection" + dr["ForumId"].ToString() + "\" onclick=\"aftoggleSection(" + dr["ForumId"].ToString() + ");\" src=\"" + ImagePath + "/arrows_down.png\" /></td></tr>");
                            sb.Append("<tr id=\"section" + dr["ForumId"].ToString() + "\" style=\"display:none;\"><td colspan=\"2\">");
                        }
                        else
                        {
                            sb.Append("<img align=\"absmiddle\" class=\"afarrow\" id=\"imgSection" + dr["ForumId"].ToString() + "\" onclick=\"aftoggleSection(" + dr["ForumId"].ToString() + ");\" src=\"" + ImagePath + "/arrows_up.png\" /></td></tr>");
                            sb.Append("<tr id=\"section" + dr["ForumId"].ToString() + "\"><td colspan=\"2\">");
                        }
                    }
                    tmpForum = forumKey;
                }
                if (bCanMod)
                {
                    sb.Append("<div class=\"afmodrow\">");
                    sb.Append("<table width=\"99%\">");
                    sb.Append("<tr><td style=\"white-space:nowrap;\">" + GetDate(Convert.ToDateTime(dr["DateCreated"])) + "</td>");
                    sb.Append("<td align=\"right\">");
                    if (bModApprove)
                    {
                        sb.Append("<span class=\"afminibtn\" onclick=\"afmodApprove(" + dr["ForumId"].ToString() + "," + dr["TopicId"].ToString() + "," + dr["ReplyId"].ToString() + ");\" onmouseover=\"this.className='afminibtn_over';\" onmouseout=\"this.className='afminibtn';\">[RESX:Approve]</span>");
                    }
                    //If bModApprove And bModMove And CInt(dr("ReplyId")) = 0 Then
                    //    sb.Append("<span class=""afminibtn"" onmouseover=""this.className='afminibtn_over';"" onmouseout=""this.className='afminibtn';"">[RESX:MoveApprove]</span>")
                    //End If
                    if (bModApprove || bModEdit)
                    {
                        sb.Append("<span class=\"afminibtn\" onclick=\"javascript:if(confirm('[RESX:Confirm:Reject]')){afmodReject(" + dr["ForumId"].ToString() + "," + dr["TopicId"].ToString() + "," + dr["ReplyId"].ToString() + "," + dr["AuthorId"].ToString() + ");};\" onmouseover=\"this.className='afminibtn_over';\" onmouseout=\"this.className='afminibtn';\">[RESX:Reject]</span>");
                    }
                    if (bModDelete)
                    {
                        sb.Append("<span class=\"afminibtn\" onclick=\"javascript:if(confirm('[RESX:Confirm:Delete]')){afmodDelete(" + dr["ForumId"].ToString() + "," + dr["TopicId"].ToString() + "," + dr["ReplyId"].ToString() + ");};\" onmouseover=\"this.className='afminibtn_over';\" onmouseout=\"this.className='afminibtn';\">[RESX:Delete]</span>");
                    }
                    if (bModEdit)
                    {
                        sb.Append("<span class=\"afminibtn\" onclick=\"afmodEdit('" + TopicEditUrl(Convert.ToInt32(dr["ForumId"]), Convert.ToInt32(dr["TopicId"]), Convert.ToInt32(dr["ReplyId"])) + "');\" onmouseover=\"this.className='afminibtn_over';\" onmouseout=\"this.className='afminibtn';\">[RESX:Edit]</span>");
                    }


                    sb.Append("</td></tr>");
                    sb.Append("<tr><td style=\"width:90px\" valign=\"top\">" + dr["AuthorName"].ToString() + "</td>");
                    sb.Append("<td><div class=\"afrowsub\">[RESX:Subject]: " + dr["Subject"].ToString() + "</div><div class=\"afrowbod\">" + dr["Body"].ToString() + "</div>");
                    sb.Append(GetAttachments(Convert.ToInt32(dr["ContentId"]), true, PortalId, ModuleId, dtAttach) + "</td></tr>");
                    sb.Append("</table></div>");
                }

            }
            sb.Append("</table></div>");

            litTopics.Text = Utilities.LocalizeControl(sb.ToString());
        }
        private string TopicEditUrl(int ForumId, int TopicId, int ReplyId)
        {
            if (ReplyId == 0)
            {
                return NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=post", "action=te", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId });
            }
            else
            {
                return NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=post", "action=re", ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, "postid=" + ReplyId });
            }
        }
        private void SetPermissions(int fId)
        {
            ForumController fc = new ForumController();
            Forum f = fc.GetForum(PortalId, ModuleId, fId);
            bModDelete = false;
            bModApprove = false;
            bModEdit = false;
            bModMove = false;
            bCanMod = false;
            if (f != null)
            {
                bModDelete = Permissions.HasPerm(f.Security.ModDelete, ForumUser.UserRoles);
                bModApprove = Permissions.HasPerm(f.Security.ModApprove, ForumUser.UserRoles);
                bModMove = Permissions.HasPerm(f.Security.ModMove, ForumUser.UserRoles);
                bModEdit = Permissions.HasPerm(f.Security.ModEdit, ForumUser.UserRoles);
                if (bModDelete || bModApprove || bModMove || bModEdit)
                {
                    bCanMod = true;
                }

            }

        }
        private string GetAttachments(int ContentId, bool AllowAttach, int PortalID, int ModuleID, DataTable dtAttach)
        {
            string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/";
            if (Request.IsSecureConnection)
            {
                strHost = strHost.Replace("http://", "https://");
            }
            //TODO: Add option for folder storage
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (AllowAttach == true)
            {
                string vpath = null;
                vpath = PortalSettings.HomeDirectory + "activeforums_Attach/";
                string fpath = null;
                fpath = Server.MapPath(PortalSettings.HomeDirectory + "activeforums_Attach/");
                dtAttach.DefaultView.RowFilter = "ContentId = " + ContentId;
                foreach (DataRow dr in dtAttach.DefaultView.ToTable().Rows)
                {
                    sb.Append("<br />");
                    string tmpPath = null;
                    int attachId = Convert.ToInt32(dr["AttachId"]);
                    string Filename = dr["Filename"].ToString();
                    string contentType = dr["ContentType"].ToString();
                    if (dr.IsNull("FileData"))
                    {
                        tmpPath = fpath + dr["Filename"].ToString();
                        if (!(System.IO.File.Exists(tmpPath)))
                        {
                            tmpPath = tmpPath.Replace("activeforums_Attach", "ntforums_attach");
                        }
                        string strExt = System.IO.Path.GetExtension(tmpPath);
                        switch (strExt.ToUpper())
                        {
                            case ".JPG":
                            case ".JPEG":
                            case ".GIF":
                            case ".PNG":
                                sb.Append("<br><span class=\"afimage\"><img src=\"" + vpath + Filename + "\" border=\"0\" align=\"center\" /></span><br /><br />");
                                break;
                            default:
                                sb.Append("<a href=\"" + vpath + Filename + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/images/attach.gif\" border=\"0\" align=\"absmiddle\">Attachment: " + Filename + "</a><br>");
                                break;
                        }
                    }
                    else
                    {
                        switch (contentType.ToLower())
                        {
                            case "image/jpeg":
                            case "image/pjpeg":
                            case "image/gif":
                            case "image/png":
                                sb.Append("<br /><span class=\"afimage\"><img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalID + "&moduleid=" + ModuleID + "&attachid=" + attachId + "\" border=0 align=center></span><br><br>");
                                break;
                            default:
                                sb.Append("<span class=\"afattachlink\"><a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalID + "&moduleid=" + ModuleID + "&attachid=" + attachId + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/images/attach.gif\" border=\"0\" align=\"absmiddle\">Attachment: " + Filename + "</a></span><br />");
                                break;
                        }
                    }
                }
                sb.Append("<br />");

                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

    }
}

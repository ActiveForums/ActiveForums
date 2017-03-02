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
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Modules.ActiveForums
{
	public class Email
	{
		public string Subject;
		public string From;
		public string BodyText;
		public string BodyHTML;

		public List<SubscriptionInfo> Recipients;
	
        public bool UseQueue = false;

		public static void SendEmail(int templateId, int portalId, int moduleId, int tabId, int forumId, int topicId, int replyId, string comments, Author author)
		{
			var portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
			var mainSettings = DataCache.MainSettings(moduleId);
		    var sTemplate = string.Empty;
			var tc = new TemplateController();
			var ti = tc.Template_Get(templateId, portalId, moduleId);
			var subject = TemplateUtils.ParseEmailTemplate(ti.Subject, string.Empty, portalId, moduleId, tabId, forumId, topicId, replyId, string.Empty, author.AuthorId, portalSettings.TimeZoneOffset);
			var bodyText = TemplateUtils.ParseEmailTemplate(ti.TemplateText, string.Empty, portalId, moduleId, tabId, forumId, topicId, replyId, string.Empty, author.AuthorId, portalSettings.TimeZoneOffset);
			var bodyHTML = TemplateUtils.ParseEmailTemplate(ti.TemplateHTML, string.Empty, portalId, moduleId, tabId, forumId, topicId, replyId, string.Empty, author.AuthorId, portalSettings.TimeZoneOffset);
			bodyText = bodyText.Replace("[REASON]", comments);
			bodyHTML = bodyHTML.Replace("[REASON]", comments);
		    var fc = new ForumController();
			var fi = fc.Forums_Get(forumId, -1, false, true);
			var sFrom = fi.EmailAddress != string.Empty ? fi.EmailAddress : portalSettings.Email;
			
            //Send now
			
            var oEmail = new Email();
			var subs = new List<SubscriptionInfo>();
			var si = new SubscriptionInfo
			             {
			                 DisplayName = author.DisplayName,
			                 Email = author.Email,
			                 FirstName = author.FirstName,
			                 LastName = author.LastName,
			                 UserId = author.AuthorId,
			                 Username = author.Username
			             };

		    subs.Add(si);

			oEmail.UseQueue = mainSettings.MailQueue;
			oEmail.Recipients = subs;
			oEmail.Subject = subject;
			oEmail.From = sFrom;
			oEmail.BodyText = bodyText;
			oEmail.BodyHTML = bodyHTML;

			new Thread(oEmail.Send).Start();
		}

		public static void SendEmailToModerators(int templateId, int portalId, int forumId, int topicId, int replyId, int moduleID, int tabID, string comments)
		{
			SendEmailToModerators(templateId, portalId, forumId, topicId, replyId, moduleID, tabID, comments, null);
		}

		public static void SendEmailToModerators(int templateId, int portalId, int forumId, int topicId, int replyId, int moduleID, int tabID, string comments, UserInfo user)
		{
			var portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
			var mainSettings = DataCache.MainSettings(moduleID);
			var fc = new ForumController();
			var fi = fc.Forums_Get(forumId, -1, false, true);
			if (fi == null)
				return;

			var subs = new List<SubscriptionInfo>();
			var rc = new Security.Roles.RoleController();
			var uc = new Entities.Users.UserController();
		    var modApprove = fi.Security.ModApprove;
			var modRoles = modApprove.Split('|')[0].Split(';');
		    foreach (var r in modRoles)
		    {
		        if (string.IsNullOrEmpty(r)) continue;
		        var rid = Convert.ToInt32(r);
		        var rName = rc.GetRole(rid, portalId).RoleName;
		        foreach (UserRoleInfo usr in rc.GetUserRolesByRoleName(portalId, rName))
		        {
		            var ui = uc.GetUser(portalId, usr.UserID);
		            var si = new SubscriptionInfo
		                         {
		                             UserId = ui.UserID,
		                             DisplayName = ui.DisplayName,
		                             Email = ui.Email,
		                             FirstName = ui.FirstName,
		                             LastName = ui.LastName
		                         };
		            if (! (subs.Contains(si)))
		            {
		                subs.Add(si);
		            }
		        }
		    }

		    if (subs.Count <= 0)
				return;

		    var sTemplate = string.Empty;
			var tc = new TemplateController();
			var ti = tc.Template_Get(templateId, portalId, moduleID);
			var subject = TemplateUtils.ParseEmailTemplate(ti.Subject, string.Empty, portalId, moduleID, tabID, forumId, topicId, replyId, portalSettings.TimeZoneOffset);
			var bodyText = TemplateUtils.ParseEmailTemplate(ti.TemplateText, string.Empty, portalId, moduleID, tabID, forumId, topicId, replyId, comments, user, -1, portalSettings.TimeZoneOffset);
			var bodyHTML = TemplateUtils.ParseEmailTemplate(ti.TemplateHTML, string.Empty, portalId, moduleID, tabID, forumId, topicId, replyId, comments, user, -1, portalSettings.TimeZoneOffset);
		    var sFrom = fi.EmailAddress != string.Empty ? fi.EmailAddress : portalSettings.Email;

			var oEmail = new Email
			                 {
			                     Recipients = subs,
			                     Subject = subject,
			                     From = sFrom,
			                     BodyText = bodyText,
			                     BodyHTML = bodyHTML,
                                 UseQueue = mainSettings.MailQueue
			                 };


			new Thread(oEmail.Send).Start();
		}

		public static void SendAdminWatchEmail(int postID, int userID)
		{
			//TODO: Come back to fix and mod list
			// Try
			//    Dim _portalSettings As Entities.Portals.PortalSettings = CType(Current.Items("PortalSettings"), Entities.Portals.PortalSettings)
			//    Dim objsubs As New SubscriptionController
			//    Dim intPost As Integer
			//    Dim objPosts As New PostController
			//    Dim objPost As PostInfo = objPosts.NTForums_GetPostByID(PostID)
			//    Dim strSubject As String = _portalSettings.PortalName & " Forums:" & objPost.Subject
			//    If objPost.ParentPostID = 0 Then
			//        intPost = objPost.PostID
			//    Else
			//        intPost = objPost.ParentPostID
			//    End If
			//    Dim ForumID As Integer = objPost.ForumID
			//    Dim objForums As New ForumController
			//    Dim objForum As ForumInfo = objForums.Forums_Get(objPost.ForumID)

			//    Dim myTemplate As String
			//    myTemplate = CType(Current.Cache("AdminWatchEmail" & objForum.ModuleId), String)
			//    If myTemplate Is Nothing Then
			//        TemplateUtils.LoadTemplateCache(objForum.ModuleId)
			//        myTemplate = CType(Current.Cache("AdminWatchEmail" & objForum.ModuleId), String)
			//    End If
			//    Dim arrMods As String()
			//    'TODO: Come back and properly get list of moderators
			//    'arrMods = Split(objForum.CanModerate, ";")
			//    Dim i As Integer = 0
			//    Dim sLink As String
			//    sLink = Common.Globals.GetPortalDomainName(_portalSettings.PortalAlias.HTTPAlias, Current.Request) & "/default.aspx?tabid=" & _portalSettings.ActiveTab.TabID & "&view=topic&forumid=" & objPost.ForumID & "&postid=" & intPost
			//    Dim PortalId As Integer = _portalSettings.PortalId
			//    Dim FromEmail As String = _portalSettings.Email
			//    Try
			//        If Not String.IsNullOrEmpty(objForum.EmailAddress) Then
			//            FromEmail = objForum.EmailAddress
			//        End If
			//    Catch ex As Exception

			//    End Try
			//    For i = 0 To UBound(arrMods) - 1
			//        Dim objUserController As New Entities.Users.UserController
			//        Dim objRoleController As New Security.Roles.RoleController
			//        Dim RoleName As String = objRoleController.GetRole(CInt(arrMods(i)), PortalId).RoleName
			//        Dim Arr As ArrayList = Roles.GetUsersByRoleName(PortalId, RoleName)
			//        Dim objUser As Entities.Users.UserRoleInfo
			//        For Each objUser In Arr
			//            Dim sBody As String = myTemplate
			//            sBody = Replace(sBody, "[FULLNAME]", objUser.FullName)
			//            sBody = Replace(sBody, "[PORTALNAME]", _portalSettings.PortalName)
			//            sBody = Replace(sBody, "[USERNAME]", objPost.UserName)
			//            sBody = Replace(sBody, "[POSTDATE]", objPost.DateAdded.ToString)
			//            sBody = Replace(sBody, "[SUBJECT]", objPost.Subject)
			//            sBody = Replace(sBody, "[BODY]", objPost.Body)
			//            sBody = Replace(sBody, "[LINK]", "<a href=""" & sLink & """>" & sLink & "</a>")
			//            Dim objUserInfo As Entities.Users.UserInfo = objUserController.GetUser(PortalId, objUser.UserID)
			//            SendNotification(FromEmail, objUserInfo.Membership.Email, strSubject, sBody, ForumID, intPost)
			//        Next
			//    Next
			//Catch ex As Exception
			//    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
			//End Try

		}

        /* 
         * Note: This is the method that actual sends the email.  The mail queue  
         */
		public static void SendNotification(string fromEmail, string toEmail, string subject, string bodyText, string bodyHTML)
		{
            //USE DNN API for this to ensure proper delivery & adherence to portal settings
		    Services.Mail.Mail.SendEmail(fromEmail, fromEmail, toEmail, subject, bodyHTML);
		}

		public void Send()
		{
			try
			{
				var intRecipients = 0;
				var intMessages = 0;
				var strDistributionList = string.Empty;
				Subject = Subject.Replace("&#91;", "[");
				Subject = Subject.Replace("&#93;", "]");


				foreach (var si in Recipients.Where(si => si.Email != string.Empty))
				{
					intRecipients += 1;

                    if(UseQueue)
					    Queue.Controller.Add(From, si.Email, Subject, BodyHTML, BodyText, string.Empty, string.Empty);
                    else
                        SendNotification(From, si.Email, Subject, BodyText, BodyHTML);  

					intMessages += 1;
				}

			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
			}

		}

	}
}
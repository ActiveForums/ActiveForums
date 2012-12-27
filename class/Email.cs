//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections.Generic;
using System.Data;
//ORIGINAL LINE: Imports System.Web.HttpContext

using System.Web;
namespace DotNetNuke.Modules.ActiveForums
{
	public class Email
	{
		public string Subject;
		public string From;
		public string BodyText;
		public string BodyHTML;
		//Public Recipients As ArrayList
		public List<SubscriptionInfo> Recipients;
		private string _SmtpServer = string.Empty;
		private string _SmtpUserName = string.Empty;
		private string _SmtpPassword = string.Empty;
		private string _SmtpAuthentication = string.Empty;
		private string _SmtpSSL = string.Empty;
		public bool UseQueue = false;

		public string SmtpServer
		{
			get
			{
				return _SmtpServer;
			}
			set
			{
				_SmtpServer = value;
			}
		}
		public string SmtpUserName
		{
			get
			{
				return _SmtpUserName;
			}
			set
			{
				_SmtpUserName = value;
			}
		}
		public string SmtpPassword
		{
			get
			{
				return _SmtpPassword;
			}
			set
			{
				_SmtpPassword = value;
			}
		}
		public string SmtpAuthentication
		{
			get
			{
				return _SmtpAuthentication;
			}
			set
			{
				_SmtpAuthentication = value;
			}
		}
		public string SmtpSSL
		{
			get
			{
				return _SmtpSSL;
			}
			set
			{
				_SmtpSSL = value;
			}
		}

		public void SendEmail(int TemplateId, int PortalId, int ModuleId, int TabId, int ForumId, int TopicId, int ReplyId, string Comments, Author author)
		{
			var _portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
			SettingsInfo MainSettings = DataCache.MainSettings(ModuleId);
			string Subject;
			string BodyText;
			string BodyHTML;
			string sTemplate = string.Empty;
			var tc = new TemplateController();
			TemplateInfo ti = tc.Template_Get(TemplateId, PortalId, ModuleId);
			Subject = TemplateUtils.ParseEmailTemplate(ti.Subject, string.Empty, PortalId, ModuleId, TabId, ForumId, TopicId, ReplyId, string.Empty, author.AuthorId, _portalSettings.TimeZoneOffset);
			BodyText = TemplateUtils.ParseEmailTemplate(ti.TemplateText, string.Empty, PortalId, ModuleId, TabId, ForumId, TopicId, ReplyId, string.Empty, author.AuthorId, _portalSettings.TimeZoneOffset);
			BodyHTML = TemplateUtils.ParseEmailTemplate(ti.TemplateHTML, string.Empty, PortalId, ModuleId, TabId, ForumId, TopicId, ReplyId, string.Empty, author.AuthorId, _portalSettings.TimeZoneOffset);
			BodyText = BodyText.Replace("[REASON]", Comments);
			BodyHTML = BodyHTML.Replace("[REASON]", Comments);
			string sFrom;
			var fc = new ForumController();
			Forum fi = fc.Forums_Get(ForumId, -1, false, true);
			sFrom = fi.EmailAddress != string.Empty ? fi.EmailAddress : _portalSettings.Email;
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
			oEmail.UseQueue = MainSettings.MailQueue;
			oEmail.Recipients = subs;
			oEmail.Subject = Subject;
			oEmail.From = sFrom;
			oEmail.BodyText = BodyText;
			oEmail.BodyHTML = BodyHTML;
			oEmail.SmtpServer = Convert.ToString(_portalSettings.HostSettings["SMTPServer"]);
			oEmail.SmtpUserName = Convert.ToString(_portalSettings.HostSettings["SMTPUsername"]);
			oEmail.SmtpPassword = Convert.ToString(_portalSettings.HostSettings["SMTPPassword"]);
			oEmail.SmtpAuthentication = Convert.ToString(_portalSettings.HostSettings["SMTPAuthentication"]);
			var objThread = new System.Threading.Thread(oEmail.Send);
			objThread.Start();


		}
		public void SendEmailToModerators(int TemplateId, int PortalId, int ForumId, int TopicId, int ReplyId, int ModuleID, int TabID, string Comments)
		{
			SendEmailToModerators(TemplateId, PortalId, ForumId, TopicId, ReplyId, ModuleID, TabID, Comments, null);
		}
		public void SendEmailToModerators(int TemplateId, int PortalId, int ForumId, int TopicId, int ReplyId, int ModuleID, int TabID, string Comments, DotNetNuke.Entities.Users.UserInfo User)
		{
			var _portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
			SettingsInfo MainSettings = DataCache.MainSettings(ModuleID);
			var fc = new ForumController();
			Forum fi = fc.Forums_Get(ForumId, -1, false, true);
			if (fi == null)
			{
				return;
			}
			var subs = new List<SubscriptionInfo>();
			var rc = new Security.Roles.RoleController();
			var uc = new Entities.Users.UserController();
			SubscriptionInfo si;
			string modApprove = fi.Security.ModApprove;
			string[] modRoles = modApprove.Split('|')[0].Split(';');
			if (modRoles != null)
			{
				foreach (string r in modRoles)
				{
					if (! (string.IsNullOrEmpty(r)))
					{
						int rid = Convert.ToInt32(r);
						string rName = rc.GetRole(rid, PortalId).RoleName;
						foreach (Entities.Users.UserRoleInfo usr in rc.GetUserRolesByRoleName(PortalId, rName))
						{
							var ui = uc.GetUser(PortalId, usr.UserID);
							si = new SubscriptionInfo
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
				}
			}

			if (subs.Count <= 0)
			{
				return;
			}
			string Subject;
			string BodyText;
			string BodyHTML;
			string sTemplate = string.Empty;
			var tc = new TemplateController();
			TemplateInfo ti = tc.Template_Get(TemplateId, PortalId, ModuleID);
			Subject = TemplateUtils.ParseEmailTemplate(ti.Subject, string.Empty, PortalId, ModuleID, TabID, ForumId, TopicId, ReplyId, _portalSettings.TimeZoneOffset);
			BodyText = TemplateUtils.ParseEmailTemplate(ti.TemplateText, string.Empty, PortalId, ModuleID, TabID, ForumId, TopicId, ReplyId, Comments, User, -1, _portalSettings.TimeZoneOffset);
			BodyHTML = TemplateUtils.ParseEmailTemplate(ti.TemplateHTML, string.Empty, PortalId, ModuleID, TabID, ForumId, TopicId, ReplyId, Comments, User, -1, _portalSettings.TimeZoneOffset);
			string sFrom;

			sFrom = fi.EmailAddress != string.Empty ? fi.EmailAddress : _portalSettings.Email;

			var oEmail = new Email
			                 {
			                     Recipients = subs,
			                     Subject = Subject,
			                     From = sFrom,
			                     BodyText = BodyText,
			                     BodyHTML = BodyHTML,
			                     SmtpServer = Convert.ToString(_portalSettings.HostSettings["SMTPServer"]),
			                     SmtpUserName = Convert.ToString(_portalSettings.HostSettings["SMTPUsername"]),
			                     SmtpPassword = Convert.ToString(_portalSettings.HostSettings["SMTPPassword"]),
			                     SmtpAuthentication = Convert.ToString(_portalSettings.HostSettings["SMTPAuthentication"])
			                 };

#if SKU_ENTERPRISE
			oEmail.UseQueue = MainSettings.MailQueue;
#endif
			var objThread = new System.Threading.Thread(oEmail.Send);
			objThread.Start();

		}



		public void SendAdminWatchEmail(int PostID, int UserID)
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

		public void SendNotification(string FromEmail, string ToEmail, string Subject, string BodyText, string BodyHTML, int ForumID = 0, int TopicId = 0, int ReplyId = 0)
		{
			try
			{
				Subject = Subject.Replace("&#91;", "[");
				Subject = Subject.Replace("&#93;", "]");
				if (SmtpServer == string.Empty && SmtpUserName == string.Empty && SmtpPassword == string.Empty && SmtpAuthentication == string.Empty && SmtpSSL == string.Empty)
				{
					var objHost = new Entities.Host.HostSettingsController();
					IDataReader drHost = objHost.GetHostSettings();

					while (drHost.Read())
					{
						if (Convert.ToString(drHost["SettingName"]) == "SMTPServer")
						{
							SmtpServer = Convert.ToString(drHost["SettingValue"]);
						}
						if (Convert.ToString(drHost["SettingName"]) == "SMTPUsername")
						{
							SmtpUserName = Convert.ToString(drHost["SettingValue"]);
						}
						if (Convert.ToString(drHost["SettingName"]) == "SMTPPassword")
						{
							SmtpPassword = Convert.ToString(drHost["SettingValue"]);
						}
						if (Convert.ToString(drHost["SettingName"]) == "SMTPEnableSSL")
						{
							SmtpSSL = Convert.ToString(drHost["SettingName"]);
						}

						if (Convert.ToString(drHost["SettingName"]) == "SMTPAuthentication")
						{
							SmtpAuthentication = Convert.ToString(drHost["SettingValue"]);
						}
					}
					drHost.Close();
					drHost.Dispose();
				}

				var Email = new System.Net.Mail.MailMessage();
				try
				{

				}
				catch (Exception ex)
				{

				}
				Email.From = new System.Net.Mail.MailAddress(FromEmail);
				Email.To.Add(new System.Net.Mail.MailAddress(ToEmail));

				string sGuid = "";
				try
				{

					//sGuid = DataProvider.Instance.ActiveForums_MC_GetPostGUID(TopicId).ToString
				}
				catch (Exception ex)
				{
				}
				if (sGuid == "00000000-0000-0000-0000-000000000000" || sGuid == "")
				{
					sGuid = "";
				}
				else
				{
					sGuid = "       (" + sGuid + ")";

				}
				if (TopicId == 0)
				{
					Email.Subject = Subject;
				}
				else
				{
					Email.Subject = Subject + sGuid;
				}
				if (BodyHTML == string.Empty)
				{
					Email.Body = BodyText;
					Email.IsBodyHtml = false;
				}
				else if (BodyText == string.Empty)
				{
					Email.Body = BodyHTML;
					Email.IsBodyHtml = true;
				}
				else
				{
					System.Net.Mail.AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(BodyText, null, "text/plain");

					System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(BodyHTML, null, "text/html");

					Email.AlternateViews.Add(plainView);
					Email.AlternateViews.Add(htmlView);

				}

				var client = new System.Net.Mail.SmtpClient();

				if (SmtpServer != "")
				{

					int portPos = SmtpServer.IndexOf(":");
					if (portPos > -1)
					{
						client.Port = Convert.ToInt32(SmtpServer.Substring(portPos + 1, SmtpServer.Length - portPos - 1));
						SmtpServer = SmtpServer.Substring(0, portPos);
					}
					client.Host = SmtpServer;
					// with authentication
					//If SmtpUserName <> "" And SmtpPassword <> "" Then
					//    client.UseDefaultCredentials = False
					//    client.Credentials = New Net.NetworkCredential(SmtpUserName, SmtpPassword)
					//End If
				}
				switch (SmtpAuthentication)
				{
					case "":
					case "0": // anonymous
					break;
					case "1": // basic
						if (SmtpUserName != "" & SmtpPassword != "")
						{
							client.UseDefaultCredentials = false;
							client.Credentials = new System.Net.NetworkCredential(SmtpUserName, SmtpPassword);
						}
						break;
					case "2": // NTLM
						client.UseDefaultCredentials = true;
						break;
				}
				if (SmtpSSL == "Y" || SmtpServer.Contains("gmail"))
				{
					client.EnableSsl = true;
				}
				//Logger.Log("Email.vb line 256")
				try
				{
					client.Send(Email);
				}
				catch (Exception ex)
				{
					//Logger.Log("Email.vb line 260" & ex.ToString)
					Services.Exceptions.Exceptions.LogException(ex);
				}
			}
			catch (Exception ex)
			{
				//Logger.Log("Email.vb line 264" & ex.ToString)
				Services.Exceptions.Exceptions.LogException(ex);
			}
		}
		public void Send()
		{
			try
			{
				int intRecipients = 0;
				int intMessages = 0;
				string strDistributionList = "";
				Subject = Subject.Replace("&#91;", "[");
				Subject = Subject.Replace("&#93;", "]");
#if SKU_ENTERPRISE
				if (UseQueue)
				{
					foreach (SubscriptionInfo si in Recipients)
					{
						if (si.Email != "")
						{
							intRecipients += 1;
							Queue.Controller.Add(From, si.Email, Subject, BodyHTML, BodyText, string.Empty, string.Empty);
							//SendNotification(From, si.Email, Subject, BodyText, BodyHTML)
							intMessages += 1;
						}
					}
				}
				else
				{
					foreach (SubscriptionInfo si in Recipients)
					{
						if (si.Email != "")
						{
							intRecipients += 1;
							SendNotification(From, si.Email, Subject, BodyText, BodyHTML);
							intMessages += 1;
						}
					}
				}
#else
                foreach (SubscriptionInfo si in Recipients)
				{
					if (si.Email != "")
					{
						intRecipients += 1;
						SendNotification(From, si.Email, Subject, BodyText, BodyHTML);
						intMessages += 1;
					}
				}
#endif


			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
			}

		}

	}
}
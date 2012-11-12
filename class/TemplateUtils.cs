//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
using System.IO;
//ORIGINAL LINE: Imports System.Web.HttpContext

using System.Web;
using System.Text.RegularExpressions;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Profile;

namespace DotNetNuke.Modules.ActiveForums
{
	public class TemplateUtils
	{
		internal static string ShowIcon(bool CanView, int ForumID, int UserId, DateTime DateAdded, DateTime LastRead, int LastPostId)
		{
			string sOutput;
			if (LastRead == new DateTime())
			{
				LastRead = Utilities.NullDate();
			}
			try
			{
				if (CanView)
				{
					if (LastPostId == 0 || UserId == -1)
					{
						sOutput = "folder_closed.png";
					}
					else
					{
						if (! (DateAdded == Utilities.NullDate()))
						{
							sOutput = DateAdded > LastRead ? "folder_new.png" : "folder.png";
						}
						else
						{
							sOutput = "folder.png";
						}

					}
				}
				else
				{
					sOutput = "folder_forbidden.png";

				}
			}
			catch
			{
				sOutput = "folder.png";
			}
			return sOutput;
		}
		public static void LoadTemplateCache(int ModuleID)
		{
			var tc = new TemplateController();
			foreach (TemplateInfo ti in tc.Template_List(-1, ModuleID))
			{
				DataCache.CacheStore(ti.Title + ModuleID, ti.Template);
				DataCache.CacheStore(ti.Subject + "_Subject_" + ModuleID, ti.Subject);
			}
		}
		private static TemplateInfo GetTemplateByName(string TemplateName, int ModuleId, int PortalId)
		{
			var tc = new TemplateController();
			TemplateInfo ti;
			try
			{
				ti = tc.Template_Get(TemplateName, PortalId, ModuleId);
				if (ti != null)
				{
					return ti;
				}
			    return null;
			}
			catch (Exception ex)
			{
				ti = new TemplateInfo {TemplateHTML = "Error loading " + TemplateName + " template."};
			    ti.TemplateText = ti.TemplateHTML;
			}
			return ti;
		}
		public static string ParseEmailTemplate(string Template, string TemplateName, int PortalID, int ModuleID, int TabID, int ForumID, int TopicId, int ReplyId, int TimeZoneOffset)
		{
			return ParseEmailTemplate(Template, TemplateName, PortalID, ModuleID, TabID, ForumID, TopicId, ReplyId, string.Empty, 0, TimeZoneOffset);
		}
		public static string ParseEmailTemplate(string TemplateName, int PortalID, int ModuleID, int TabID, int ForumID, int TopicId, int ReplyId, int TimeZoneOffset)
		{
			return ParseEmailTemplate(string.Empty, TemplateName, PortalID, ModuleID, TabID, ForumID, TopicId, ReplyId, string.Empty, null, -1, TimeZoneOffset);
		}
		public static string ParseEmailTemplate(string TemplateName, int PortalID, int ModuleID, int TabID, int ForumID, int TopicId, int ReplyId, string Comments, int TimeZoneOffset)
		{
			return ParseEmailTemplate(string.Empty, TemplateName, PortalID, ModuleID, TabID, ForumID, TopicId, ReplyId, Comments, null, -1, TimeZoneOffset);
		}
		public static string ParseEmailTemplate(string TemplateName, int PortalID, int ModuleID, int TabID, int ForumID, int TopicId, int ReplyId, DotNetNuke.Entities.Users.UserInfo User, int TimeZoneOffset)
		{
			return ParseEmailTemplate(string.Empty, TemplateName, PortalID, ModuleID, TabID, ForumID, TopicId, ReplyId, string.Empty, User, -1, TimeZoneOffset);
		}
		public static string ParseEmailTemplate(string Template, string TemplateName, int PortalID, int ModuleID, int TabID, int ForumID, int TopicId, int ReplyId, string Comments, int UserId, int TimeZoneOffset)
		{
			var uc = new Entities.Users.UserController();
			var usr = uc.GetUser(PortalID, UserId);
			return ParseEmailTemplate(Template, TemplateName, PortalID, ModuleID, TabID, ForumID, TopicId, ReplyId, Comments, usr, UserId, TimeZoneOffset);
		}
		public static string ParseEmailTemplate(string Template, string TemplateName, int PortalID, int ModuleID, int TabID, int ForumID, int TopicId, int ReplyId, string Comments, DotNetNuke.Entities.Users.UserInfo User, int UserId, int TimeZoneOffset)
		{
			var _portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
			SettingsInfo ms = DataCache.MainSettings(ModuleID);
			string sOut;
			if (TemplateName != string.Empty)
			{
			    object obj = null;
			    //TemplateUtils.LoadTemplateCache(ModuleID)
			    if (TemplateName.Contains("_Subject_"))
			    {
			        TemplateName = TemplateName.Replace("_Subject_" + ModuleID, string.Empty);
			        TemplateInfo objTemplate = GetTemplateByName(TemplateName, ModuleID, PortalID);
			        sOut = objTemplate.Subject; //CType(Current.Cache(TemplateName & ModuleID), String)
			    }
			    else
			    {
			        TemplateInfo objTemplate = GetTemplateByName(TemplateName, ModuleID, PortalID);
			        sOut = objTemplate.TemplateHTML; //CType(Current.Cache(TemplateName & ModuleID), String)
			    }
			}
			else
			{
				sOut = Template;
			}


			string Subject = string.Empty;
			string Body = string.Empty;
			DateTime DateCreated = Utilities.NullDate();
			string AuthorName = string.Empty;
			if (TopicId > 0 & ReplyId > 0)
			{
				var rc = new ReplyController();
				ReplyInfo ri = rc.Reply_Get(PortalID, ModuleID, TopicId, ReplyId);
				if (ri != null)
				{
					Subject = ri.Content.Subject;
					Body = ri.Content.Body;
					DateCreated = ri.Content.DateCreated;
					AuthorName = ri.Content.AuthorName;
				}
			}
			else
			{
				var tc = new TopicsController();
				TopicInfo ti = tc.Topics_Get(PortalID, ModuleID, TopicId);
				if (ti != null)
				{
					Subject = ti.Content.Subject;
					Body = ti.Content.Body;
					DateCreated = ti.Content.DateCreated;
					AuthorName = ti.Content.AuthorName;
				}

			}
			var fc = new ForumController();
			Forum fi = fc.Forums_Get(ForumID, -1, false);
			if (User == null)
			{
				var objUsers = new Entities.Users.UserController();
				Entities.Users.UserInfo objUser = objUsers.GetUser(PortalID, UserId);
				User = objUser;
			}

			string sFirstName;
			string sLastName;
			string sDisplayName;
			string sUsername = string.Empty;
			int iUserId = -1;
			if (User != null)
			{
				iUserId = User.UserID;
				sFirstName = User.FirstName;
				sLastName = User.LastName;
				sDisplayName = User.DisplayName;
				sUsername = User.Username;
			}
			else
			{
				sFirstName = string.Empty;
				sLastName = string.Empty;
				sDisplayName = string.Empty;
			}

			string Link;

			string sURL;
			if (string.IsNullOrEmpty(fi.PrefixURL) || ! Utilities.IsRewriteLoaded())
			{
				if (ReplyId == 0)
				{
					sURL = Common.Globals.NavigateURL(TabID, "", new[] {ParamKeys.ForumId + "=" + ForumID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId});
					if (ms.UseShortUrls)
					{
						sURL = Common.Globals.NavigateURL(TabID, "", new[] {ParamKeys.TopicId + "=" + TopicId});
					}
				}
				else
				{
					sURL = Common.Globals.NavigateURL(TabID, "", new[] {ParamKeys.ForumId + "=" + ForumID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId});
					if (ms.UseShortUrls)
					{
						sURL = Common.Globals.NavigateURL(TabID, "", new[] {ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId});
					}
				}
			}
			else
			{
				var db = new Data.Common();
				int contentId = -1;
				if (ReplyId > 0)
				{
					contentId = ReplyId;
				}
				sURL = db.GetUrl(ModuleID, -1, ForumID, TopicId, -1, contentId);
			}

			//Dim LinkPostId As Integer = CInt(IIf(ReplyId = 0, TopicId, ReplyId))
			Link = sURL; //DotNetNuke.Common.Globals.NavigateURL(TabID, "", ParamKeys.ForumId & "=" & fi.ForumID, ParamKeys.ViewType & "=topic&" & ParamKeys.TopicId & "=" & TopicId & "&ptarget=" & LinkPostId)

			if (! (Link.StartsWith("/")) && ! (Link.StartsWith("http")))
			{
				Link = "/" + Link;
			}
			string ForumURL;
			ForumURL = Common.Globals.NavigateURL(TabID, "", new[] {ParamKeys.ForumId + "=" + ForumID, ParamKeys.ViewType + "=" + Views.Topics});
			if (ms.UseShortUrls)
			{
				ForumURL = Common.Globals.NavigateURL(TabID, "", new[] {ParamKeys.ForumId + "=" + ForumID});
			}
			if (! (Link.StartsWith("http")))
			{
				if (Link.IndexOf(HttpContext.Current.Request.Url.Host, StringComparison.Ordinal) == -1)
				{
					Link = Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + Link;
				}
			}

			string ModLink;
			ModLink = Common.Globals.NavigateURL(TabID, "", new[] {ParamKeys.ViewType + "=modtopics", ParamKeys.ForumId + "=" + ForumID});
			if (ModLink.IndexOf(HttpContext.Current.Request.Url.Host, StringComparison.Ordinal) == -1)
			{
				ModLink = Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + ModLink;
			}
			bool isMod = false;

			sOut = sOut.Replace("[DISPLAYNAME]", UserProfiles.GetDisplayName(ModuleID, "DISABLED", false, UserId, ms.UserNameDisplay, AuthorName, sFirstName, sLastName, sDisplayName));
			sOut = sOut.Replace("[USERNAME]", sUsername);
			sOut = sOut.Replace("[USERID]", UserId.ToString());
			sOut = sOut.Replace("[FORUMNAME]", fi.ForumName);
			sOut = sOut.Replace("[PORTALID]", PortalID.ToString());
			sOut = sOut.Replace("[FIRSTNAME]", sFirstName);
			sOut = sOut.Replace("[LASTNAME]", sLastName);
			sOut = sOut.Replace("[FULLNAME]", sFirstName + " " + sLastName);
			//sOut = Replace(sOut, "[DISPLAYNAME]", DisplayName)
			sOut = sOut.Replace("[GROUPNAME]", fi.GroupName);
			sOut = sOut.Replace("[POSTDATE]", Utilities.GetDate(DateCreated, ModuleID, TimeZoneOffset));
			sOut = sOut.Replace("[COMMENTS]", Comments);
			sOut = sOut.Replace("[PORTALNAME]", _portalSettings.PortalName);
			sOut = sOut.Replace("[MODLINK]", "<a href=\"" + ModLink + "\">" + ModLink + "</a>");
			sOut = sOut.Replace("[LINK]", "<a href=\"" + Link + "\">" + Link + "</a>");
			sOut = sOut.Replace("[HYPERLINK]", "<a href=\"" + Link + "\">" + Link + "</a>");
			sOut = sOut.Replace("[LINKURL]", Link);
			sOut = sOut.Replace("[FORUMURL]", ForumURL);
			sOut = sOut.Replace("[FORUMLINK]", "<a href=\"" + ForumURL + "\">" + ForumURL + "</a>");

			if (User != null)
			{
				sOut = sOut.Replace("[SENDERUSERNAME]", User.UserID.ToString());
				sOut = sOut.Replace("[SENDERFIRSTNAME]", User.FirstName);
				sOut = sOut.Replace("[SENDERLASTNAME]", User.LastName);
				sOut = sOut.Replace("[SENDERDISPLAYNAME]", User.DisplayName);
			}
			else
			{
				sOut = sOut.Replace("[SENDERUSERNAME]", string.Empty);
				sOut = sOut.Replace("[SENDERFIRSTNAME]", string.Empty);
				sOut = sOut.Replace("[SENDERLASTNAME]", string.Empty);
				sOut = sOut.Replace("[SENDERDISPLAYNAME]", string.Empty);
			}
			sOut = sOut.Replace("[SUBJECT]", Subject);
			sOut = sOut.Replace("[BODY]", Utilities.ManageImagePath(Body, Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request))));
			sOut = sOut.Replace("[Body]", Utilities.ManageImagePath(Body, Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request))));
			return sOut;
		}
		public static string ParseTemplate(int PortalID, string Template)
		{
		    string sOut = ParsePortalInfo(PortalID, Template);
		    return sOut;
		}

	    public static string ParseTemplate(int PortalID, string Template, int UserID)
		{
	        string sOut = Template;
			sOut = ParseUserInfo(PortalID, UserID, sOut);
			sOut = ParsePortalInfo(PortalID, sOut);
			return sOut;
		}
		private static string ParseUserInfo(int PortalID, int UserID, string Template)
		{
		    string sOut = Template;
			var objUsers = new Entities.Users.UserController();
			Entities.Users.UserInfo objUser = objUsers.GetUser(PortalID, UserID);
			if (objUser != null)
			{
				sOut = sOut.Replace("[FULLNAME]", objUser.DisplayName);
				sOut = sOut.Replace("[USERNAME]", objUser.Username);
				sOut = sOut.Replace("[FIRSTNAME]", objUser.FirstName);
				sOut = sOut.Replace("[LASTNAME]", objUser.LastName);
				sOut = sOut.Replace("[DISPLAYNAME]", objUser.DisplayName);
				sOut = sOut.Replace("[PASSWORD]", objUser.Membership.Password);
				sOut = sOut.Replace("[EMAIL]", objUser.Membership.Email);
				sOut = sOut.Replace("[USERID]", objUser.UserID.ToString());
			}
			return sOut;
		}

		//Public Shared Function ParseUserDetails(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal Template As String, ByVal TokenPrefix As String) As String
		//    Return ParseUserDetails(PortalId, UserId, Template, TokenPrefix, False, True)
		//End Function
		//Public Shared Function ParseUserDetails(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal Template As String, ByVal TokenPrefix As String, ByVal ParseUserProfile As Boolean) As String
		//    Return ParseUserDetails(PortalId, UserId, Template, TokenPrefix, ParseUserProfile, True)
		//End Function
		//Public Shared Function ParseUserDetails(ByVal PortalId As Integer, ByVal UserId As Integer, ByVal Template As String, ByVal TokenPrefix As String, ByVal ParseUserProfile As Boolean, ByVal OmitPass As Boolean, ByVal CurrentUserType As CurrentUserTypes) As String
		//    Dim sOut As String
		//    sOut = Template
		//    Dim objUsers As New Entities.Users.UserController
		//    Dim objUser As Entities.Users.UserInfo = objUsers.GetUser(PortalId, UserId)
		//    If Not TokenPrefix = "" Then
		//        TokenPrefix &= ":"
		//    End If
		//    If Not objUser Is Nothing Then
		//        sOut = Replace(sOut, "[" & TokenPrefix & "FULLNAME]", objUser.DisplayName)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "USERNAME]", objUser.Username)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "FIRSTNAME]", objUser.FirstName)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "LASTNAME]", objUser.LastName)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "DISPLAYNAME]", objUser.DisplayName)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "EMAIL]", objUser.Membership.Email)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "USERID]", objUser.UserID.ToString)
		//        If OmitPass = True Then
		//            sOut = Replace(sOut, "[" & TokenPrefix & "PASSWORD]", String.Empty)
		//        Else
		//            sOut = Replace(sOut, "[" & TokenPrefix & "PASSWORD]", objUser.Membership.Password)
		//        End If
		//        If ParseUserProfile = True Then
		//            If InStr(sOut, "[DNN:PROFILE:") > 0 Then
		//                sOut = ParseProfile(PortalId, UserId, sOut, CurrentUserType)
		//            End If
		//        End If
		//    Else
		//        sOut = Replace(sOut, "[" & TokenPrefix & "FULLNAME]", String.Empty)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "USERNAME]", String.Empty)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "FIRSTNAME]", String.Empty)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "LASTNAME]", String.Empty)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "DISPLAYNAME]", String.Empty)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "EMAIL]", String.Empty)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "USERID]", String.Empty)
		//        sOut = Replace(sOut, "[" & TokenPrefix & "PASSWORD]", String.Empty)
		//    End If
		//    Return sOut
		//End Function
		private static string ParsePortalInfo(int PortalID, string Template)
		{
			string sOut = "";
			var objPortals = new Entities.Portals.PortalController();
			Entities.Portals.PortalInfo objPortal = objPortals.GetPortal(PortalID);
			var objPortalAlias = new Entities.Portals.PortalAliasController();
			if (objPortal != null)
			{
				sOut = Template.Replace("[PORTALNAME]", objPortal.PortalName);
			}
			return sOut;
		}
		public static string GetPostInfo(int PortalId, int ModuleId, int UserId, string Username, User up, string ImagePath, bool IsMod, string IPAddress, bool IsUserOnline, CurrentUserTypes CurrentUserType, bool UserPrefHideAvatar, int TimeZoneOffset)
		{
			string sPostInfo = ParseProfileInfo(CacheKeys.PostInfo, PortalId, ModuleId, UserId, Username, up, ImagePath, IsMod, IPAddress, CurrentUserType, UserPrefHideAvatar, TimeZoneOffset);
			if (sPostInfo.ToLower().Contains("<br"))
			{
				return sPostInfo;
			}
		    var sr = new StringReader(sPostInfo);
		    string sTrim = string.Empty;
		    while (sr.Peek() != -1)
		    {
		        string tmp = sr.ReadLine();
		        if (tmp != null && tmp.Trim() != string.Empty)
		        {
		            sTrim += tmp.Trim() + "<br />";
		        }
		    }
		    return sTrim;
		}

		public static string ParseProfileInfo(string CacheKey, int PortalId, int ModuleId, int UserId, string Username, User up, string ImagePath, bool IsMod, string IPAddress, CurrentUserTypes CurrentUserType, bool UserPrefHideAvatar, int TimeZoneOffset)
		{
			SettingsInfo mainSettings = DataCache.MainSettings(ModuleId);
		    CacheKey = string.Format(CacheKeys.PostInfo, ModuleId.ToString());
			string myTemplate = Convert.ToString(DataCache.CacheRetrieve(CacheKey));
			if (string.IsNullOrEmpty(myTemplate))
			{
				TemplateInfo objTemplateInfo = GetTemplateByName("ProfileInfo", ModuleId, PortalId);
				myTemplate = objTemplateInfo.TemplateHTML;
				if (CacheKey != string.Empty)
				{
					DataCache.CacheStore(CacheKey, myTemplate);
				}
			}
			myTemplate = ParseProfileTemplate(myTemplate, up, PortalId, ModuleId, ImagePath, CurrentUserType, true, UserPrefHideAvatar, false, IPAddress, Controls.UserProfile.ProfileModes.View, -1, TimeZoneOffset);
			return myTemplate;
		}
		public static string ParseProfileTemplate(string ProfileTemplate, int UserId, int PortalId, int ModuleId, int CurrentUserId, int TimeZoneOffset)
		{
			var uc = new UserController();
			User up = uc.GetUser(PortalId, ModuleId, UserId);
			return ParseProfileTemplate(ProfileTemplate, up, PortalId, ModuleId, string.Empty, CurrentUserTypes.Anon, false, false, false, string.Empty, Controls.UserProfile.ProfileModes.View, CurrentUserId, TimeZoneOffset);
		}
		public static string ParseProfileTemplate(string ProfileTemplate, User up, int PortalId, int ModuleId, string ImagePath, CurrentUserTypes CurrentUserType, Controls.UserProfile.ProfileModes ProfileMode, int CurrentUserId, int TimeZoneOffset)
		{
			return ParseProfileTemplate(ProfileTemplate, up, PortalId, ModuleId, ImagePath, CurrentUserType, false, false, false, string.Empty, ProfileMode, CurrentUserId, TimeZoneOffset);
		}
		public static string ParseProfileTemplate(string ProfileTemplate, User up, int PortalId, int ModuleId, string ImagePath, CurrentUserTypes CurrentUserType, int TimeZoneOffset)
		{
			return ParseProfileTemplate(ProfileTemplate, up, PortalId, ModuleId, ImagePath, CurrentUserType, false, false, false, string.Empty, Controls.UserProfile.ProfileModes.View, -1, TimeZoneOffset);
		}
		public static string ParseProfileTemplate(string ProfileTemplate, User up, int PortalId, int ModuleId, string ImagePath, CurrentUserTypes CurrentUserType, bool LegacyTemplate, int TimeZoneOffset)
		{
			return ParseProfileTemplate(ProfileTemplate, up, PortalId, ModuleId, ImagePath, CurrentUserType, false, false, false, string.Empty, Controls.UserProfile.ProfileModes.View, -1, TimeZoneOffset);
		}
		public static string ParseProfileTemplate(string ProfileTemplate, User up, int PortalId, int ModuleId, string ImagePath, CurrentUserTypes CurrentUserType, bool LegacyTemplate, bool UserPrefHideAvatar, bool UserPrefHideSignature, string IPAddress, int TimeZoneOffset)
		{
			return ParseProfileTemplate(ProfileTemplate, up, PortalId, ModuleId, ImagePath, CurrentUserType, LegacyTemplate, UserPrefHideAvatar, UserPrefHideSignature, IPAddress, Controls.UserProfile.ProfileModes.View, -1, TimeZoneOffset);
		}
		public static string ParseProfileTemplate(string ProfileTemplate, User up, int PortalId, int ModuleId, string ImagePath, CurrentUserTypes CurrentUserType, bool LegacyTemplate, bool UserPrefHideAvatar, bool UserPrefHideSignature, string IPAddress, Controls.UserProfile.ProfileModes Mode, int CurrentUserId, int TimeZoneOffset)
		{
			try
			{
				if (LegacyTemplate)
				{
					ProfileTemplate = CleanTemplate(ProfileTemplate);
				}
				if (up.Profile == null)
				{
					var uc = new UserController();
					up = uc.FillProfile(PortalId, -1, up);
				}
				if (ProfileTemplate.Contains("[POSTINFO]"))
				{
					string sPostInfo = GetPostInfo(PortalId, ModuleId, up.UserId, up.UserName, up, ImagePath, false, IPAddress, up.Profile.IsUserOnline, CurrentUserType, UserPrefHideAvatar, TimeZoneOffset);
					ProfileTemplate = ProfileTemplate.Replace("[POSTINFO]", sPostInfo);
				}
				SettingsInfo MainSettings = DataCache.MainSettings(ModuleId);
				string pt = ProfileTemplate;
				if ((pt.IndexOf("[DNN:PROFILE:", 0, StringComparison.Ordinal) + 1) > 0)
				{
					pt = ParseProfile(PortalId, up.UserId, pt, CurrentUserType, Mode, CurrentUserId);
				}
				if (CurrentUserType == CurrentUserTypes.Anon || CurrentUserType == CurrentUserTypes.Auth)
				{
					IPAddress = string.Empty;
				}
				int totalPoints = up.PostCount;
				if (MainSettings.EnablePoints && up.UserId > 0)
				{
					try
					{
						totalPoints = (up.Profile.TopicCount * MainSettings.TopicPointValue) + (up.Profile.ReplyCount * MainSettings.ReplyPointValue) + (up.Profile.AnswerCount * MainSettings.AnswerPointValue) + up.Profile.RewardPoints;
						pt = pt.Replace("[AF:PROFILE:TOTALPOINTS]", totalPoints.ToString());
					}
					catch (Exception ex)
					{
						pt = pt.Replace("[AF:PROFILE:TOTALPOINTS]", string.Empty);
					}
					pt = pt.Replace("[AF:POINTS:VIEWCOUNT]", up.Profile.ViewCount.ToString());
					pt = pt.Replace("[AF:POINTS:ANSWERCOUNT]", up.Profile.AnswerCount.ToString());
					pt = pt.Replace("[AF:POINTS:REWARDPOINTS]", up.Profile.RewardPoints.ToString());
				}
				else
				{
					pt = pt.Replace("[AF:PROFILE:TOTALPOINTS]", string.Empty);
					pt = pt.Replace("[AF:POINTS:VIEWCOUNT]", string.Empty);
					pt = pt.Replace("[AF:POINTS:ANSWERCOUNT]", string.Empty);
					pt = pt.Replace("[AF:POINTS:REWARDPOINTS]", string.Empty);
				}
				string sUserStatus = string.Empty;
				if (MainSettings.UsersOnlineEnabled && up.UserId > 0)
				{
					sUserStatus = UserProfiles.UserStatus(ImagePath, up.Profile.IsUserOnline, up.UserId, ModuleId, "[RESX:UserOnline]", "[RESX:UserOffline]");
				}
				string RankDisplay = string.Empty;
				string RankName = string.Empty;
				if (up.UserId > 0)
				{
					RankDisplay = UserProfiles.GetUserRank(PortalId, ModuleId, up.UserId, totalPoints, 0);
					RankName = UserProfiles.GetUserRank(PortalId, ModuleId, up.UserId, totalPoints, 1);
				}

				string PMUrl = string.Empty;
				string PMLink = string.Empty;
				if (up.UserId > 0)
				{
					switch (MainSettings.PMType)
					{
						case PMTypes.AM:
							PMUrl = Common.Globals.NavigateURL(MainSettings.PMTabId, "", new[] {"anon=y", "sendto=" + up.UserId});
							PMLink = "<a href=\"" + PMUrl + "\"><img src=\"" + ImagePath + "/icon_pm.png\" alt=\"[RESX:SendPM]\" border=\"0\" /></a>";
							break;
						case PMTypes.Ventrian:
							PMUrl = Common.Globals.NavigateURL(MainSettings.PMTabId, "", new[] {"type=compose", "sendto=" + up.UserId});
							PMLink = "<a href=\"" + PMUrl + "\"><img src=\"" + ImagePath + "/icon_pm.png\" alt=\"[RESX:SendPM]\" border=\"0\" /></a>";
							break;
						case PMTypes.Social:
							PMUrl = Common.Globals.NavigateURL(MainSettings.PMTabId, "", new[] {"senduid=" + up.UserId});
							PMLink = "<a href=\"" + PMUrl + "\"><img src=\"" + ImagePath + "/icon_pm.png\" alt=\"[RESX:SendPM]\" border=\"0\" /></a>";
							break;
					}
				}

				string sSignature;
				if (MainSettings.AllowSignatures != 0 && ! UserPrefHideSignature && ! up.Profile.SignatureDisabled && Mode == Controls.UserProfile.ProfileModes.View)
				{
					sSignature = up.Profile.Signature;
					if (sSignature != string.Empty)
					{
						sSignature = Utilities.ManageImagePath(sSignature);
					}
					if (MainSettings.AllowSignatures == 1)
					{
						sSignature = Utilities.HTMLEncode(sSignature);
						sSignature = sSignature.Replace(System.Environment.NewLine, "<br />");
					}
					else if (MainSettings.AllowSignatures == 2)
					{
						sSignature = Utilities.HTMLDecode(sSignature);
					}
				}
				else if (Mode == Controls.UserProfile.ProfileModes.Edit && MainSettings.AllowSignatures > 0)
				{
					sSignature = up.Profile.Signature;
					sSignature = Utilities.HTMLEncode(sSignature);
				}
				else if (up.Profile.SignatureDisabled)
				{
					sSignature = string.Empty;
				}
				else
				{
					sSignature = string.Empty;
				}

				string sAvatar = string.Empty;
				if (! UserPrefHideAvatar && ! up.Profile.AvatarDisabled)
				{
					if (MainSettings.ProfileType == ProfileTypes.Social)
					{
						sAvatar = "<social:ProfilePicture ProfileUserId='" + up.UserId + "' PicSizes='sm' imagealt='" + UserProfiles.GetDisplayName(ModuleId, up.UserId, MainSettings.UserNameDisplay, up.UserName, up.FirstName, up.LastName, up.DisplayName).Replace("'", string.Empty).Replace("\"", string.Empty) + "'  runat='server'/>";
					}
					else
					{
                        sAvatar = UserProfiles.GetAvatar(up.UserId, PortalId, ImagePath, MainSettings.Theme, up.Profile.Avatar, (int)up.Profile.AvatarType, MainSettings.AvatarWidth, MainSettings.ProfileType, true);
					}

				}

				string sUserPosts = string.Empty;
				bool isMod = (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.ForumMod || CurrentUserType == CurrentUserTypes.SuperUser);
			    pt = pt.Replace("[AF:PROFILE:DISPLAYNAME]", UserProfiles.GetDisplayName(ModuleId, MainSettings.ProfileVisibility, isMod, up.UserId, MainSettings.UserNameDisplay, up.UserName, up.FirstName, up.LastName, up.DisplayName));
				if (Mode == Controls.UserProfile.ProfileModes.View)
				{
					pt = pt.Replace("[AF:PROFILE:LOCATION]", up.Profile.Location);
					pt = pt.Replace("[AF:PROFILE:WEBSITE]", up.Profile.WebSite);
					pt = pt.Replace("[AF:PROFILE:YAHOO]", up.Profile.Yahoo);
					pt = pt.Replace("[AF:PROFILE:MSN]", up.Profile.MSN);
					pt = pt.Replace("[AF:PROFILE:ICQ]", up.Profile.ICQ);
					pt = pt.Replace("[AF:PROFILE:AOL]", up.Profile.AOL);
					pt = pt.Replace("[AF:PROFILE:OCCUPATION]", up.Profile.Occupation);
					pt = pt.Replace("[AF:PROFILE:INTERESTS]", up.Profile.Interests);
					pt = pt.Replace("[AF:PROFILE:SIGNATURE]", sSignature);
					pt = pt.Replace("[AF:CONTROL:AVATAREDIT]", string.Empty);
				}
				else if (Mode == Controls.UserProfile.ProfileModes.Edit)
				{
					pt = pt.Replace("[AF:PROFILE:LOCATION]", "<asp:textbox id=\"txtLocation\" text=\"" + up.Profile.Location + "\" runat=\"server\" CssClass=\"aftextbox\" Width=\"150\" />");
					pt = pt.Replace("[AF:PROFILE:WEBSITE]", "<asp:textbox id=\"txtWebSite\" text=\"" + up.Profile.WebSite + "\" runat=\"server\" CssClass=\"aftextbox\" Width=\"150\" />");
					pt = pt.Replace("[AF:PROFILE:YAHOO]", "<asp:textbox id=\"txtYahoo\" text=\"" + up.Profile.Yahoo + "\" runat=\"server\" CssClass=\"aftextbox\" Width=\"150\" />");
					pt = pt.Replace("[AF:PROFILE:MSN]", "<asp:textbox id=\"txtMSN\" text=\"" + up.Profile.MSN + "\" runat=\"server\" CssClass=\"aftextbox\" Width=\"150\" />");
					pt = pt.Replace("[AF:PROFILE:ICQ]", "<asp:textbox id=\"txtICQ\" text=\"" + up.Profile.ICQ + "\" runat=\"server\" CssClass=\"aftextbox\" Width=\"150\" />");
					pt = pt.Replace("[AF:PROFILE:AOL]", "<asp:textbox id=\"txtAOL\" text=\"" + up.Profile.AOL + "\" runat=\"server\" CssClass=\"aftextbox\" Width=\"150\" />");
					pt = pt.Replace("[AF:PROFILE:OCCUPATION]", "<asp:textbox id=\"txtOccupation\" text=\"" + up.Profile.Occupation + "\" runat=\"server\" CssClass=\"aftextbox\" Width=\"150\" />");
					pt = pt.Replace("[AF:PROFILE:INTERESTS]", "<asp:textbox id=\"txtInterests\" text=\"" + up.Profile.Interests + "\" runat=\"server\" CssClass=\"aftextbox\" Width=\"150\" />");
					if (sSignature != string.Empty)
					{
						sSignature = sSignature.Replace("\"", "&#34;");
						sSignature = Utilities.HTMLEncode(sSignature);
					}

					pt = pt.Replace("[AF:PROFILE:SIGNATURE]", "<asp:textbox id=\"txtSignature\" text=\"" + sSignature + "\" TextMode=\"MultiLine\" runat=\"server\" CssClass=\"aftextbox\"  Width=\"300\" Rows=\"5\" />");
					pt = pt.Replace("[AF:CONTROL:AVATAREDIT]", "<table id=\"tblAvatars\" runat=\"server\"><tr><td class=\"afbold\" colspan=\"2\"><asp:Label ID=\"lblAvatarError\" runat=\"server\" Visible=\"false\" /></td></tr><tr><td class=\"afbold\">[RESX:Avatar:Upload]:</td><td><asp:FileUpload ID=\"inpAvatar\" runat=\"server\" cssclass=\"aftextbox\" Visible=\"false\" Width=\"200\" /></td></tr><tr id=\"trAvatarLinks\" runat=\"server\"><td class=\"afbold\">[RESX:Avatar:Link]:</td><td><asp:TextBox ID=\"txtAvatarLink\" runat=\"server\" CssClass=\"aftextbox\" /></td></tr></table>");
				}
				pt = pt.Replace("[AF:BUTTON:PROFILEEDIT]", "<asp:placeholder id=\"plhProfileEditButton\" runat=\"server\" />");
				pt = pt.Replace("[AF:BUTTON:PROFILESAVE]", "<asp:placeholder id=\"plhProfileSaveButton\" runat=\"server\" />");
				pt = pt.Replace("[AF:BUTTON:PROFILECANCEL]", "<asp:placeholder id=\"plhProfileCancelButton\" runat=\"server\" />");
				string sDateCreated = string.Empty;
				string sDateCreatedReplacement = "[AF:PROFILE:DATECREATED]";
				if (up.Profile.DateCreated != null && up.UserId > 0)
				{
					if (pt.Contains("[AF:PROFILE:DATECREATED:"))
					{
					    string sFormat = pt.Substring(pt.IndexOf("[AF:PROFILE:DATECREATED:", StringComparison.Ordinal) + (sDateCreatedReplacement.Length), 1);
						sDateCreated = up.Profile.DateCreated.ToString(sFormat);
						sDateCreatedReplacement = "[AF:PROFILE:DATECREATED:" + sFormat + "]";
					}
					else
					{
						sDateCreated = Utilities.GetDate(up.Profile.DateCreated, ModuleId, TimeZoneOffset);
					}
				}
				pt = pt.Replace(sDateCreatedReplacement, sDateCreated);


				string sDateLastActivity = string.Empty;
				string sDateLastActivityReplacement = "[AF:PROFILE:DATELASTACTIVITY]";

				if (up.Profile.DateLastActivity != null && up.UserId > 0)
				{
					if (pt.Contains("[AF:PROFILE:DATELASTACTIVITY:"))
					{
					    string sFormat = pt.Substring(pt.IndexOf("[AF:PROFILE:DATELASTACTIVITY:", StringComparison.Ordinal) + (sDateLastActivityReplacement.Length), 1);
						sDateLastActivity = up.Profile.DateLastActivity.ToString(sFormat);
						sDateLastActivityReplacement = "[AF:PROFILE:DATELASTACTIVITY:" + sFormat + "]";
					}
					else
					{
						sDateLastActivity = Utilities.GetDate(up.Profile.DateLastActivity, ModuleId, TimeZoneOffset);
					}
				}
				pt = pt.Replace(sDateLastActivityReplacement, sDateLastActivity);
				if (up.PostCount == 0)
				{
					pt = pt.Replace("[AF:PROFILE:POSTCOUNT]", string.Empty);
					pt = pt.Replace("Posts:", string.Empty);
				}
				else
				{
					pt = pt.Replace("[AF:PROFILE:POSTCOUNT]", up.PostCount.ToString());
				}

				pt = pt.Replace("[AF:PROFILE:AVATAR]", sAvatar);
				pt = pt.Replace("[AF:PROFILE:USERCAPTION]", up.Profile.UserCaption);
				pt = pt.Replace("[AF:PROFILE:USERID]", up.UserId.ToString());
				pt = pt.Replace("[AF:PROFILE:USERNAME]", Utilities.HTMLEncode(up.UserName).Replace("&amp;#", "&#"));
				pt = pt.Replace("[AF:PROFILE:FIRSTNAME]", Utilities.HTMLEncode(up.FirstName).Replace("&amp;#", "&#"));
				pt = pt.Replace("[AF:PROFILE:LASTNAME]", Utilities.HTMLEncode(up.LastName).Replace("&amp;#", "&#"));
				pt = pt.Replace("[AF:PROFILE:USERSTATUS]", sUserStatus);
			    pt = pt.Replace("[AF:PROFILE:USERSTATUS:CSS]", sUserStatus.Contains("online") ? "af-status-online" : "af-status-offline");
			    pt = pt.Replace("[AF:PROFILE:DATELASTPOST]", ((up.Profile.DateLastPost == new DateTime()) ? string.Empty : Utilities.GetDate(up.Profile.DateLastPost, ModuleId, TimeZoneOffset)));
				pt = pt.Replace("[AF:PROFILE:TOPICCOUNT]", up.Profile.TopicCount.ToString());
				pt = pt.Replace("[AF:PROFILE:REPLYCOUNT]", up.Profile.ReplyCount.ToString());
				pt = pt.Replace("[AF:PROFILE:ANSWERCOUNT]", up.Profile.AnswerCount.ToString());
				pt = pt.Replace("[AF:PROFILE:REWARDPOINTS]", up.Profile.RewardPoints.ToString());
				pt = pt.Replace("[AF:PROFILE:BIO]", up.Profile.Bio);
				pt = pt.Replace("[AF:PROFILE:RANKDISPLAY]", RankDisplay);
				pt = pt.Replace("[AF:PROFILE:RANKNAME]", RankName);
				pt = pt.Replace("[AF:PROFILE:PMLINK]", PMLink);
				pt = pt.Replace("[AF:PROFILE:PMURL]", PMUrl);

				pt = pt.Replace("[MODIPADDRESS]", IPAddress);
				string sSettingsURL = string.Empty;
				if (up.UserId > 0 & (CurrentUserType == CurrentUserTypes.ForumMod || CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser))
				{
					sSettingsURL = "<a href=\"" + Common.Globals.NavigateURL(Convert.ToInt32(HttpContext.Current.Request.QueryString["TabId"]), "", ParamKeys.ViewType + "=usersettings&uid=" + up.UserId) + "\">[RESX:UserSettings]</a>";
				}
				pt = pt.Replace("[MODUSERSETTINGS]", sSettingsURL);
				if (up.UserId == -1)
				{
					up.Profile.Roles = string.Empty;
				}
				if (pt.Contains("[ROLES:"))
				{
					pt = ParseRoles(pt, up.Profile.Roles);
				}
				//pt = Globals.ControlRegisterTag & pt
				return pt;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

		}
		private static string ParseRoles(string Template, string UserRoles)
		{
			int intLength = 0;
			int inStart = (Template.IndexOf("[ROLES:", 0) + 1) + 7;
			int inEnd = (Template.IndexOf("]", inStart - 1) + 1);
			string sRoles = Template.Substring(inStart - 1, inEnd - inStart);
			string RoleDisplay = string.Empty;
			string ReplaceTag = "[ROLES:" + sRoles + "]";
			if (UserRoles == string.Empty)
			{
				string pattern = "(\\[ROLES:(.+?)\\])";
				var regExp = new Regex(pattern);
				MatchCollection matches;
				matches = regExp.Matches(Template);
				string sResource = string.Empty;
				foreach (Match match in matches)
				{
					Template = Template.Replace(match.Groups[0].Value, string.Empty);
				}

				return Template;
			}
		    foreach (string role in sRoles.Split(new[] {';'}))
		    {
		        if (!string.IsNullOrEmpty(role) && role != "-10")
		        {
		            foreach (string userRole in UserRoles.Split(new[] {';'}))
		            {
		                if (role.Trim() == userRole.Trim())
		                {
		                    RoleDisplay = role.Trim();
		                    break;
		                }
		            }
		            if (RoleDisplay != string.Empty)
		            {
		                break;
		            }
		        }
		    }
		    Template = Template.Replace(ReplaceTag, RoleDisplay);
		    return Template;

		}
		private static string CleanTemplate(string Template)
		{
			string s = Template;
			string sReplace;
			string pattern = "(\\[.+?\\])";
			var regExp = new Regex(pattern);
		    MatchCollection matches = regExp.Matches(s);
			foreach (Match match in matches)
			{
				sReplace = string.Empty;
				switch (match.Value)
				{
					case "[RANKNAME]":
						sReplace = "[AF:PROFILE:RANKNAME]";
						break;
					case "[RANKDISPLAY]":
						sReplace = "[AF:PROFILE:RANKDISPLAY]";
						break;
					case "[AF:PROFILE:LASTACTIVE]":
						sReplace = "[AF:PROFILE:DATELASTACTIVITY]";
						break;
					case "[MEMBERSINCE]":
						sReplace = "[AF:PROFILE:DATECREATED]";
						break;
					case "[AF:PROFILE:MEMBERSINCE]":
						sReplace = "[AF:PROFILE:DATECREATED]";
						break;
					case "[USERSTATUS]":
						sReplace = "[AF:PROFILE:USERSTATUS]";
						break;
					case "[USERCAPTION]":
						sReplace = "[AF:PROFILE:USERCAPTION]";
						break;
					case "[USERNAME]":
						sReplace = "[AF:PROFILE:USERNAME]";
						break;
					case "[USERID]":
						sReplace = "[AF:PROFILE:USERID]";
						break;
					case "[DISPLAYNAME]":
						sReplace = "[AF:PROFILE:DISPLAYNAME]";
						break;
					case "[POSTS]":
						sReplace = "[AF:PROFILE:POSTCOUNT]";
						break;
					case "[AVATAR]":
						sReplace = "[AF:PROFILE:AVATAR]";
						break;
					case "[LOCATION]":
						sReplace = "[AF:PROFILE:LOCATION]";
						break;
					case "[WEBSITE]":
						sReplace = "[AF:PROFILE:WEBSITE]";
						break;
					case "[AF:POINTS:TOPICCOUNT]":
						sReplace = "[AF:PROFILE:TOPICCOUNT]";
						break;
					case "[AF:POINTS:REPLYCOUNT]":
						sReplace = "[AF:PROFILE:REPLYCOUNT]";
						break;
					case "[SIGNATURE]":
						sReplace = "[AF:PROFILE:SIGNATURE]";

						break;
				}
				if (sReplace != string.Empty)
				{
					s = s.Replace(match.Value, sReplace);
				}
			}

			return s;
		}
		public static string GetTemplateSection(string Template, string StartTag, string EndTag)
		{
			int intStartTag = Template.IndexOf(StartTag, StringComparison.Ordinal);
			int intEndTag = Template.IndexOf(EndTag, StringComparison.Ordinal);
			if (intStartTag >= 0 && intEndTag > intStartTag)
			{
				int intSubTempStart = intStartTag + StartTag.Length;
				int intSubTempEnd = intEndTag;
				int intSubTempLength = intSubTempEnd - intSubTempStart;
				string sSubTemp = Template.Substring(intSubTempStart, intSubTempLength);
				return sSubTemp;
			}
		    return Template;
		}
		public static string ReplaceSubSection(string Template, string SubTemplate, string StartTag, string EndTag)
		{
			int intStartTag = Template.IndexOf(StartTag, StringComparison.Ordinal);
			int intEndTag = Template.IndexOf(EndTag, StringComparison.Ordinal);
			if (intStartTag >= 0 && intEndTag > intStartTag)
			{
				int intSubTempStart = intStartTag + StartTag.Length;
				int intSubTempEnd = intEndTag - 1;
				int intSubTempLength = intSubTempEnd - intSubTempStart;
				Template = Template.Substring(0, intStartTag) + SubTemplate + Template.Substring(intEndTag + EndTag.Length);
			}
			return Template;
		}
		public static string ParseProfile(int PortalId, int UserId, string Template, CurrentUserTypes CurrentUserType, Controls.UserProfile.ProfileModes Mode, int CurrentUserId)
		{
			var objuser = Entities.Users.UserController.GetUser(PortalId, UserId, true);

			string s = Template;
			string sReplace;
			s = s.Replace("[DNN:PROFILE:Email]", objuser.Email);
			string pattern = "(\\[DNN:PROFILE:(.+?)\\])";
			var regExp = new Regex(pattern);
		    MatchCollection matches = regExp.Matches(s);
			string sResource;
			foreach (Match match in matches)
			{
				sReplace = string.Empty;
				sResource = string.Empty;
				if (objuser != null)
				{
				    ProfilePropertyDefinitionCollection profproperties = objuser.Profile.ProfileProperties;
					ProfilePropertyDefinition profprop = profproperties.GetByName(match.Groups[2].Value);
					sResource = "ProfileProperties_{0}";
					if (profprop != null)
					{
						sResource = string.Format(sResource, match.Groups[2].Value);
						if (Mode == Controls.UserProfile.ProfileModes.View)
						{
							if (profprop.Visibility == Entities.Users.UserVisibilityMode.AdminOnly && (CurrentUserType != CurrentUserTypes.Anon || CurrentUserType != CurrentUserTypes.Auth))
							{
								sReplace = profprop.PropertyValue;
							}
							else if (profprop.Visibility == Entities.Users.UserVisibilityMode.MembersOnly && CurrentUserType != CurrentUserTypes.Anon)
							{
								sReplace = profprop.PropertyValue;
							}
							else
							{
								sReplace = "[RESX:Private]";
							}
						}
						else if (Mode == Controls.UserProfile.ProfileModes.Edit)
						{
							if (CurrentUserType != CurrentUserTypes.Anon)
							{
								if (CurrentUserId == UserId)
								{
									var lc = new ListController();
								    ListEntryInfoCollection list = lc.GetListEntryInfoCollection("DataType");
									foreach (ListEntryInfo li in list)
									{
										if (li.EntryID == profprop.DataType)
										{

											string listText = li.Text.Replace("DotNetNuke.UI.WebControls.", string.Empty);
											listText = listText.Replace(", DotNetNuke", string.Empty);
											switch (listText.ToUpperInvariant())
											{
												case "TEXTEDITCONTROL":
													sReplace = "<asp:textbox id=\"dnnctl" + profprop.PropertyName + "\" runat=\"server\" text=\"" + profprop.PropertyValue + "\" CssClass=\"aftextbox\" Width=\"150\" />";
													break;
											}

										}


									}

								}
							}
						}

						sResource = Services.Localization.Localization.GetString(sResource, "~/admin/users/app_localresources/profile.ascx.resx");

					}
				}
				s = s.Replace(match.Value, sReplace);
				s = s.Replace("[RESX:DNNProfile:" + match.Groups[2].Value + "]", sResource);
			}
			return s;
		}
		private static string GetTopicTemplate(int TopicTemplateId, int ModuleId)
		{
			SettingsInfo MainSettings = DataCache.MainSettings(ModuleId);
			//Dim myFile As String
			string strTemplate = "";
		    string sOutput = DataCache.GetCachedTemplate(MainSettings.TemplateCache, ModuleId, "TopicView", TopicTemplateId);

			return sOutput;
		}
		public static string PreviewTopic(int TopicTemplateID, int PortalId, int ModuleId, int TabId, Forum ForumInfo, int UserId, string Body, string ImagePath, User up, DateTime PostDate, CurrentUserTypes CurrentUserType, int timeZoneOffset)
		{
			string sTemplate = GetTopicTemplate(TopicTemplateID, ModuleId);
			try
			{
				SettingsInfo MainSettings = DataCache.MainSettings(ModuleId);
			    string sTopic = GetTemplateSection(sTemplate, "[TOPIC]", "[/TOPIC]");
				sTopic = sTopic.Replace("[ACTIONS:ALERT]", string.Empty);
				sTopic = sTopic.Replace("[ACTIONS:EDIT]", string.Empty);
				sTopic = sTopic.Replace("[ACTIONS:DELETE]", string.Empty);
				sTopic = sTopic.Replace("[ACTIONS:QUOTE]", string.Empty);
				sTopic = sTopic.Replace("[ACTIONS:REPLY]", string.Empty);
				sTopic = sTopic.Replace("[POSTDATE]", Utilities.GetDate(PostDate, ModuleId, timeZoneOffset));
				sTopic = sTopic.Replace("[POSTINFO]", GetPostInfo(PortalId, ModuleId, UserId, up.UserName, up, ImagePath, false, HttpContext.Current.Request.UserHostAddress, true, CurrentUserType, false, timeZoneOffset));
				sTemplate = ParsePreview(PortalId, UserId, sTopic, Body, ModuleId, TabId, ForumInfo);
				sTemplate = "<table class=\"afgrid\" width=\"100%\" cellspacing=\"0\" cellpadding=\"4\" border=\"1\">" + sTemplate;
				sTemplate = sTemplate + "</table>";
				sTemplate = Utilities.LocalizeControl(sTemplate);
				sTemplate = Utilities.StripTokens(sTemplate);
			}
			catch (Exception ex)
			{
				sTemplate = ex.ToString();
			}

			return sTemplate;
		}
		private static string ParsePreview(int PortalId, int UserId, string Template, string Message, int ModuleId, int TabId, Forum ForumInfo)
		{
			if (Message.Contains("&#91;IMAGE:"))
			{
				string strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
				string pattern = "(&#91;IMAGE:(.+?)&#93;)";
				var regExp = new Regex(pattern);
			    MatchCollection matches = regExp.Matches(Message);
				foreach (Match match in matches)
				{
				    string sImage = "<img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + match.Groups[2].Value + "\" border=\"0\" />";
				    Message = Message.Replace(match.Value, sImage);
				}
			}
			if (Message.Contains("&#91;THUMBNAIL:"))
			{
				string strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
				string pattern = "(&#91;THUMBNAIL:(.+?)&#93;)";
				var regExp = new Regex(pattern);
			    MatchCollection matches = regExp.Matches(Message);
				foreach (Match match in matches)
				{
				    string thumbId = match.Groups[2].Value.Split(':')[0];
					string parentId = match.Groups[2].Value.Split(':')[1];
					string sImage = "<a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + parentId + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + thumbId + "\" border=\"0\" /></a>";
					Message = Message.Replace(match.Value, sImage);
				}
			}
			Template = Template.Replace("[BODY]", Message);
			if (Regex.IsMatch(Template, "<CODE([^>]*)>", RegexOptions.IgnoreCase))
			{
				if (Regex.IsMatch(Message, "<CODE([^>]*)>", RegexOptions.IgnoreCase))
				{
					foreach (Match m in Regex.Matches(Message, "<CODE([^>]*)>(.*?)</CODE>", RegexOptions.IgnoreCase))
					{
						Message = Message.Replace(m.Value, m.Value.Replace("<br>", System.Environment.NewLine));
					}


				}
				var objCode = new CodeParser();
				Template = CodeParser.ParseCode(Utilities.HTMLDecode(Template));
			}
			return Template;
		}

		internal static string ParseSpecial(string template, SpecialTokenTypes tokenType, string Url, string Title, bool CanRead, string Options = "")
		{
			switch (tokenType)
			{
				case SpecialTokenTypes.AddThis:
					return ParseAddThis(template, Url, Title, CanRead, Options);
			}
			return template;
		}
		private static string ParseAddThis(string template, string Url, string Title, bool CanRead, string Options)
		{
			if (Options == string.Empty)
			{
				template = template.Replace("[AF:CONTROL:ADDTHIS:0]", string.Empty);
			}
			string pattern = "(\\[AF:CONTROL:ADDTHIS:(.*?)\\])";
			var regExp = new Regex(pattern);
		    string sFind = string.Empty;
			string sUserName = string.Empty;
			MatchCollection matches = regExp.Matches(template);
			foreach (Match match in matches)
			{
				sFind = match.Groups[0].Value;
				sUserName = match.Groups[2].Value;
			}
			if (sFind != string.Empty)
			{
				string replace = string.Empty;
				if ((sUserName == "0" && Options != string.Empty) | sUserName != "0")
				{
					if (sFind != string.Empty && CanRead)
					{
						if (sUserName == "0" && Options != string.Empty)
						{
							sUserName = Options;
						}
						replace = DataCache.GetTemplate("AddThis.txt");
						replace = replace.Replace("[USERNAME]", sUserName.Replace("'", "\\'"));
						replace = replace.Replace("[URL]", Url);
						replace = replace.Replace("[TITLE]", Title.Replace("'", "\\'"));
					}
				}

				template = template.Replace(sFind, replace);
				if (sFind != string.Empty)
				{
					template = template.Replace(sFind, string.Empty);
				}
			}


			return template;
		}

	}
}

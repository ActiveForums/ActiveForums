//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
//ORIGINAL LINE: Imports System.Web.HttpContext

using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class UserProfiles
	{
		public static string GetAvatar(int UserID, int PortalID, string ImagePath, string ThemeName, string AvatarFileName, int AvatarType, int AvatarWidth, ProfileTypes ProfileType, bool DefaultAvatarEnabled)
		{
			string strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
			return "<img src=\"" + strHost + "profilepic.ashx?userid=" + UserID + "&w=" + AvatarWidth + "&h=" + AvatarWidth + "\" />";
			//If ProfileType = ProfileTypes.Basic Or ProfileType = ProfileTypes.Disabled Then
			//    Dim _portalSettings As Entities.Portals.PortalSettings = CType(Current.Items("PortalSettings"), Entities.Portals.PortalSettings)

			//    If Not AvatarFileName = "" Then
			//        If AvatarType = 1 Then
			//            Return "<img src=""" & AvatarFileName & """ border=""0"" alt=""[RESX:Avatar]"" />"
			//        ElseIf AvatarType = 0 Then
			//            Dim Avatar As String
			//            Avatar = _portalSettings.HomeDirectory & "ActiveForums_Avatars/" & AvatarFileName
			//            If IO.File.Exists(Current.Server.MapPath(Avatar)) Then
			//                Return "<img src=""" & Avatar & """ border=""0"" alt=""[RESX:Avatar]"" />"
			//            Else
			//                Return String.Empty
			//            End If

			//        End If
			//    ElseIf File.Exists(Current.Server.MapPath("~/DesktopModules/ActiveForums/themes/" & ThemeName & "/images/default_avatar.png")) Then
			//        Return "<img src=""" & ImagePath & "/images/default_avatar.png"" border=""0"" alt=""[RESX:Avatar]"" />"

			//    End If
			//    Return String.Empty
			//End If

		}
		public static string GetWebSite(string WebSite)
		{
		    if (WebSite.Trim() != "")
			{
				if (! ((WebSite.IndexOf("http://", 0) + 1) > 0))
				{
					WebSite = "http://" + WebSite;
				}
				return "<a href=\"" + WebSite + "\" target=\"_blank\">" + WebSite + "</a>";
			}
		    return string.Empty;
		}

	    public static string GetDisplayName(int ModuleId, int UserID, string DisplayMode, string Username, string FirstName = "", string LastName = "", string DisplayName = "")
		{
			return GetDisplayName(ModuleId, "DISABLED", false, UserID, DisplayMode, Username, FirstName, LastName, DisplayName);
		}
		public static string GetDisplayName(int ModuleId, string SecurityMode, bool IsMod, int UserId, string DisplayMode, Author ai)
		{
			return GetDisplayName(ModuleId, SecurityMode, IsMod, UserId, DisplayMode, ai.Username, ai.FirstName, ai.LastName, ai.DisplayName);
		}
		public static string GetDisplayName(int ModuleId, string SecurityMode, bool IsMod, int UserID, string DisplayMode, string Username, string FirstName = "", string LastName = "", string DisplayName = "")
		{
            var _portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);

			SettingsInfo MainSettings = DataCache.MainSettings(ModuleId);
			int iTabId = _portalSettings.ActiveTab.TabID;
			string sURL;
            if (DisplayMode == "none")
            {
                sURL = "{0}";
                DisplayMode = MainSettings.UserNameDisplay;
            }
            else
            {
                if (MainSettings.ProfileType == ProfileTypes.Basic)
                {
                    //Dim Params() As String = {ParamKeys.ViewType & "=profile", "uid=" & UserID}
                    //sURL = "<a href=""" & DotNetNuke.Common.Globals.NavigateURL(iTabId, "", Params) & """>{0}</a>"
                    string[] Params = { "userid=" + UserID };
                    sURL = "<a href=\"" + Common.Globals.NavigateURL(_portalSettings.UserTabId, "", Params) + "\">{0}</a>";
                }
                else if (MainSettings.ProfileType == ProfileTypes.Advanced)
                {
                    string[] Params = { "uid=" + UserID };
                    sURL = "<a href=\"" + Common.Globals.NavigateURL(_portalSettings.UserTabId, "", Params) + "\">{0}</a>";
                }
                else if (MainSettings.ProfileType == ProfileTypes.Social)
                {
                    string[] Params = { "asuid=" + UserID };
                    sURL = "<a href=\"" + Common.Globals.NavigateURL(_portalSettings.UserTabId, "", Params) + "\">{0}</a>";
                }
                else
                {
                    sURL = "{0}";
                }
            }
			

			bool LinkProfile = false;
			switch (SecurityMode.ToUpperInvariant())
			{
				case "DISABLED":
					LinkProfile = false;
					break;
				case "ENABLED":
					LinkProfile = true;
					break;
				case "ENABLEDREG":
					LinkProfile = HttpContext.Current.Request.IsAuthenticated;
					break;
				case "ENABLEDMOD":
					if (HttpContext.Current.Request.IsAuthenticated && IsMod)
					{
						LinkProfile = true;
					}
					else
					{
						LinkProfile = false;
					}
					break;
			}
			if (FirstName == "" && LastName == "" && (DisplayMode.ToLowerInvariant() == "FULLNAME" || DisplayMode.ToLowerInvariant() == "FIRSTNAME" || DisplayMode.ToLowerInvariant() == "LASTNAME") & UserID > 0)
			{
				var objUsers = new Entities.Users.UserController();
				Entities.Users.UserInfo objUser;
				objUser = objUsers.GetUser(_portalSettings.PortalId, UserID);
				if (objUser != null)
				{
					Username = objUser.Username;
					FirstName = objUser.FirstName;
					LastName = objUser.LastName;
					DisplayName = objUser.DisplayName;
				}
			}
			string sOutput;
			DisplayMode = DisplayMode.ToUpperInvariant();
			if (DisplayMode == "USERNAME")
			{
				if (UserID != -1 && UserID != 0 && LinkProfile)
				{
					sOutput = string.Format(sURL, Username);
				}
				else
				{
					sOutput = DisplayName;
				}
			}
			else
			{
				sOutput = DisplayName;
				switch (DisplayMode.ToUpperInvariant())
				{
					case "USERNAME":
						sOutput = Username.Trim(' ');
						break;
					case "FULLNAME":
						sOutput = Convert.ToString(FirstName + " " + LastName).Trim(' ');
						break;
					case "FIRSTNAME":
						sOutput = FirstName.Trim(' ');
						break;
					case "LASTNAME":
						sOutput = LastName.Trim(' ');
						break;
					case "DISPLAYNAME":
						sOutput = DisplayName.Trim(' ');
						break;
				}
				if (sOutput == "")
				{
					sOutput = Username;
				}
				sOutput = HttpUtility.HtmlEncode(sOutput);
				if (UserID != -1 && UserID != 0 && LinkProfile)
				{
					sOutput = string.Format(sURL, sOutput);
				}
			}
			return sOutput.Replace("&amp;#", "&#");
		}

		public static string UserStatus(string ThemePath, bool IsUserOnline, int UserID, int ModuleID, string AltOnlineText = "User is Online", string AltOfflineText = "User is Offline")
		{
		    if (IsUserOnline)
			{
				return "<img src=\"" + ThemePath + "/images/online.png\" alt=\"" + AltOnlineText + "\" style=\"vertical-align:middle;\" vspace=\"2\" hspace=\"2\" />";
			}
		    return "<img src=\"" + ThemePath + "/images/offline.png\" alt=\"" + AltOfflineText + "\" style=\"vertical-align:middle;\" vspace=\"2\" hspace=\"2\" />";
		}

	    /// <summary>
		/// Returns the Rank for the user
		/// </summary>
		/// <returns>ReturnType 0 Returns RankDisplay ReturnType 1 Returns RankName</returns>
		public static string GetUserRank(int PortalId, int ModuleID, int UserID, int Posts, int ReturnType)
		{
			//ReturnType 0 for RankDisplay
			//ReturnType 1 for RankName
			try
			{
				string strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
				var rc = new RewardController();
				int i = 0;
				string sRank = string.Empty;
				foreach (RewardInfo ri in rc.Reward_List(PortalId, ModuleID, true))
				{
					if (ri.MinPosts <= Posts && ri.MaxPosts > Posts)
					{
						if (ReturnType == 0)
						{
							sRank = "<img src=\"" + strHost + ri.Display.Replace("activeforums/Ranks", "activeforums/images/Ranks") + "\" border=0 alt=\"" + ri.RankName + "\" />";
							break;
						}
					    sRank = ri.RankName;
					    break;
					}
				}
				return sRank;
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}
	}
}

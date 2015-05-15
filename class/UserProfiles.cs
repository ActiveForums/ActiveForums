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
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Modules.ActiveForums
{
    public class UserProfiles
    {
        public static string GetAvatar(int userID, int avatarWidth, int avatarHeight)
        {
            var portalSettings = HttpContext.Current.Items["PortalSettings"] as PortalSettings;
            if (portalSettings == null)
                return string.Empty;

            //GIF files when reduced using DNN class losses its animation, so for gifs send them as is
            var user = new Entities.Users.UserController().GetUser(portalSettings.PortalId, userID);
            string imgUrl = string.Empty;
            if (user != null) imgUrl = user.Profile.PhotoURL;
            if (!string.IsNullOrWhiteSpace(imgUrl) && imgUrl.ToLower().EndsWith("gif"))
            {
                return "<img class='af-avatar' src='" + imgUrl + "' height='" + avatarHeight + "px' width='" + avatarWidth + "px' />";
            }
            else
            {
                return "<img class='af-avatar' src='" + string.Format(Common.Globals.UserProfilePicFormattedUrl(), userID, avatarWidth, avatarHeight) + "' />";
            }
        }

        public static string GetDisplayName(int moduleId, int userID, string username, string firstName = "", string lastName = "", string displayName = "", string profileNameClass = "af-profile-name")
        {
            return GetDisplayName(moduleId, false, false, false, userID, username, firstName, lastName, displayName, null, profileNameClass);
        }

        public static string GetDisplayName(int moduleId, bool linkProfile, bool isMod, bool isAdmin, int userId, string username, string firstName = "", string lastName = "", string displayName = "", string profileLinkClass = "af-profile-link", string profileNameClass = "af-profile-name")
        {
            var portalSettings = HttpContext.Current.Items["PortalSettings"] as PortalSettings;
            if (portalSettings == null)
                return null;

            var mainSettings = DataCache.MainSettings(moduleId);

            var outputTemplate = string.IsNullOrWhiteSpace(profileLinkClass) ? "{0}" : "<span class='" + profileNameClass + "'>{0}</span>";

            if (linkProfile && userId > 0)
            {
                var profileVisibility = mainSettings.ProfileVisibility;

                switch (profileVisibility)
                {
                    case ProfileVisibilities.Disabled:
                        linkProfile = false;
                        break;

                    case ProfileVisibilities.Everyone: // Nothing to do in this case
                        break;

                    case ProfileVisibilities.RegisteredUsers:
                        linkProfile = HttpContext.Current.Request.IsAuthenticated;
                        break;

                    case ProfileVisibilities.Moderators:
                        linkProfile = isMod || isAdmin;
                        break;

                    case ProfileVisibilities.Admins:
                        linkProfile = isAdmin;
                        break;
                }

                if (linkProfile)
                    outputTemplate = "<a href='" + Common.Globals.NavigateURL(portalSettings.UserTabId, string.Empty, new[] { "userid=" + userId }) + "' class='" + profileLinkClass + "'>{0}</a>";
            }

            var displayMode = mainSettings.UserNameDisplay + string.Empty;

            string outputName = null;
            UserInfo user;

            switch (displayMode.ToUpperInvariant())
            {
                case "DISPLAYNAME":

                    if (string.IsNullOrWhiteSpace(username) && userId > 0)
                    {
                        user = new Entities.Users.UserController().GetUser(portalSettings.PortalId, userId);
                        displayName = (user != null) ? user.DisplayName : null;
                    }

                    outputName = displayName;
                    break;

                case "USERNAME":

                    if (string.IsNullOrWhiteSpace(username) && userId > 0)
                    {
                        user = new Entities.Users.UserController().GetUser(portalSettings.PortalId, userId);
                        username = (user != null) ? user.Username : null;
                    }

                    outputName = username;
                    break;

                case "FIRSTNAME":

                    if (string.IsNullOrWhiteSpace(firstName) && userId > 0)
                    {
                        user = new Entities.Users.UserController().GetUser(portalSettings.PortalId, userId);
                        firstName = (user != null) ? user.FirstName : null;
                    }

                    outputName = firstName;
                    break;

                case "LASTNAME":

                    if (string.IsNullOrWhiteSpace(lastName) && userId > 0)
                    {
                        user = new Entities.Users.UserController().GetUser(portalSettings.PortalId, userId);
                        lastName = (user != null) ? user.LastName : null;
                    }

                    outputName = lastName;
                    break;

                case "FULLNAME":
                    if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName) && userId > 0)
                    {
                        user = new Entities.Users.UserController().GetUser(portalSettings.PortalId, userId);
                        firstName = (user != null) ? Utilities.SafeTrim(user.FirstName) : null;
                        lastName = (user != null) ? Utilities.SafeTrim(user.LastName) : null;
                    }

                    outputName = firstName + " " + lastName;
                    break;
            }


            outputName = Utilities.SafeTrim(outputName);

            if (string.IsNullOrWhiteSpace(outputName))
                outputName = userId > 0 ? "Deleted User" : "Anonymous";

            outputName = HttpUtility.HtmlEncode(outputName);

            return string.Format(outputTemplate, outputName);
        }

        public static string UserStatus(string themePath, bool isUserOnline, int userID, int moduleID, string altOnlineText = "User is Online", string altOfflineText = "User is Offline")
        {
            if (isUserOnline)
            {
                //return "<img class='af-user-status af-user-status-online' src='" + themePath + "/images/online.png' alt='" + altOnlineText + "' style='vertical-align:middle;' vspace='2' hspace='2' />";
                return "<span class=\"af-user-status\"><i class=\"fa fa-circle fa-blue\"></i></span>";
            }
            //return "<img class='af-user-status af-user-status-offline' src='" + themePath + "/images/offline.png' alt='" + altOfflineText + "' style='vertical-align:middle;' vspace='2' hspace='2' />";
            return "<span class=\"af-user-status\"><i class=\"fa fa-circle fa-red\"></i></span>";
        }

        /// <summary>
        /// Returns the Rank for the user
        /// </summary>
        /// <returns>ReturnType 0 Returns RankDisplay ReturnType 1 Returns RankName</returns>
        public static string GetUserRank(int portalId, int moduleID, int userID, int posts, int returnType)
        {
            //ReturnType 0 for RankDisplay
            //ReturnType 1 for RankName
            try
            {
                var strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
                var rc = new RewardController();
                var i = 0;
                var sRank = string.Empty;
                foreach (var ri in rc.Reward_List(portalId, moduleID, true).Where(ri => ri.MinPosts <= posts && ri.MaxPosts > posts))
                {
                    if (returnType == 0)
                    {
                        sRank = "<img src='" + strHost + ri.Display.Replace("activeforums/Ranks", "activeforums/images/Ranks") + "' border='0' alt='" + ri.RankName + "' />";
                        break;
                    }
                    sRank = ri.RankName;
                    break;
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

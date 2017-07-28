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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Collections.Generic;

namespace DotNetNuke.Modules.ActiveForums
{
    public class TemplateUtils
    {
        public static List<SubscriptionInfo> lstSubscriptionInfo { get; set; }

        public static string ShowIcon(bool canView, int forumID, int userId, DateTime dateAdded, DateTime lastRead, int lastPostId)
        {
            if (!canView)
                return "folder_forbidden.png";

            if (lastPostId == 0 || userId == -1)
                return "folder_closed.png";

            if (lastRead == DateTime.MinValue)
                lastRead = Utilities.NullDate();

            if (dateAdded != Utilities.NullDate())
                return dateAdded > lastRead ? "folder_new.png" : "folder.png";

            return "folder.png";
        }

        public static void LoadTemplateCache(int moduleID)
        {
            var tc = new TemplateController();
            foreach (var ti in tc.Template_List(-1, moduleID))
            {
                DataCache.CacheStore(ti.Title + moduleID, ti.Template);
                DataCache.CacheStore(ti.Subject + "_Subject_" + moduleID, ti.Subject);
            }
        }

        private static TemplateInfo GetTemplateByName(string templateName, int moduleId, int portalId)
        {
            var tc = new TemplateController();
            TemplateInfo ti;
            try
            {
                ti = tc.Template_Get(templateName, portalId, moduleId);
            }
            catch (Exception ex)
            {
                ti = new TemplateInfo { TemplateHTML = "Error loading " + templateName + " template." };
                ti.TemplateText = ti.TemplateHTML;
            }
            return ti;
        }

        #region Email

        public static string ParseEmailTemplate(string template, string templateName, int portalID, int moduleID, int tabID, int forumID, int topicId, int replyId, int timeZoneOffset)
        {
            return ParseEmailTemplate(template, templateName, portalID, moduleID, tabID, forumID, topicId, replyId, string.Empty, 0, timeZoneOffset);
        }

        public static string ParseEmailTemplate(string templateName, int portalID, int moduleID, int tabID, int forumID, int topicId, int replyId, int timeZoneOffset)
        {
            return ParseEmailTemplate(string.Empty, templateName, portalID, moduleID, tabID, forumID, topicId, replyId, string.Empty, null, -1, timeZoneOffset);
        }

        public static string ParseEmailTemplate(string templateName, int portalID, int moduleID, int tabID, int forumID, int topicId, int replyId, string comments, int timeZoneOffset)
        {
            return ParseEmailTemplate(string.Empty, templateName, portalID, moduleID, tabID, forumID, topicId, replyId, comments, null, -1, timeZoneOffset);
        }

        public static string ParseEmailTemplate(string templateName, int portalID, int moduleID, int tabID, int forumID, int topicId, int replyId, Entities.Users.UserInfo user, int timeZoneOffset)
        {
            return ParseEmailTemplate(string.Empty, templateName, portalID, moduleID, tabID, forumID, topicId, replyId, string.Empty, user, -1, timeZoneOffset);
        }

        public static string ParseEmailTemplate(string template, string templateName, int portalID, int moduleID, int tabID, int forumID, int topicId, int replyId, string comments, int userId, int timeZoneOffset)
        {
            var uc = new Entities.Users.UserController();
            var usr = uc.GetUser(portalID, userId);
            return ParseEmailTemplate(template, templateName, portalID, moduleID, tabID, forumID, topicId, replyId, comments, usr, userId, timeZoneOffset);
        }

        public static string ParseEmailTemplate(string template, string templateName, int portalID, int moduleID, int tabID, int forumID, int topicId, int replyId, string comments, Entities.Users.UserInfo user, int userId, int timeZoneOffset)
        {
            var portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
            var ms = DataCache.MainSettings(moduleID);
            var sOut = template;

            // If we have a template name, load the template into sOut
            if (templateName != string.Empty)
            {
                if (templateName.Contains("_Subject_"))
                {
                    templateName = templateName.Replace("_Subject_" + moduleID, string.Empty);
                    var objTemplate = GetTemplateByName(templateName, moduleID, portalID);
                    sOut = objTemplate.Subject;
                }
                else
                {
                    var objTemplate = GetTemplateByName(templateName, moduleID, portalID);
                    sOut = objTemplate.TemplateHTML;
                }
            }

            // Load Subject and body from topic or reply

            var subject = string.Empty;
            var body = string.Empty;
            var dateCreated = Utilities.NullDate();
            var authorName = string.Empty;

            if (topicId > 0 && replyId > 0)
            {
                var ri = new ReplyController().Reply_Get(portalID, moduleID, topicId, replyId);
                if (ri != null)
                {
                    subject = ri.Content.Subject;
                    body = ri.Content.Body;
                    dateCreated = ri.Content.DateCreated;
                    authorName = ri.Content.AuthorName;
                }
            }
            else
            {
                var ti = new TopicsController().Topics_Get(portalID, moduleID, topicId);
                if (ti != null)
                {
                    subject = ti.Content.Subject;
                    body = ti.Content.Body;
                    dateCreated = ti.Content.DateCreated;
                    authorName = ti.Content.AuthorName;
                }
            }

            body = Utilities.ManageImagePath(body, Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)));

            // load the forum information
            var fi = new ForumController().Forums_Get(forumID, -1, false);

            // Load the user if needed
            if (user == null)
            {
                var objUsers = new Entities.Users.UserController();
                var objUser = objUsers.GetUser(portalID, userId);
                user = objUser;
            }

            // Load the user properties

            string sFirstName;
            string sLastName;
            string sDisplayName;
            string sUsername;

            if (user != null)
            {
                sFirstName = user.FirstName;
                sLastName = user.LastName;
                sDisplayName = user.DisplayName;
                sUsername = user.Username;
            }
            else
            {
                sFirstName = string.Empty;
                sLastName = string.Empty;
                sDisplayName = string.Empty;
                sUsername = string.Empty;
            }

            // Build the link

            string link;
            if (string.IsNullOrEmpty(fi.PrefixURL) || !Utilities.IsRewriteLoaded())
            {
                if (replyId == 0)
                    link = ms.UseShortUrls ? Common.Globals.NavigateURL(tabID, string.Empty, new[] { ParamKeys.TopicId + "=" + topicId })
                        : Common.Globals.NavigateURL(tabID, string.Empty, new[] { ParamKeys.ForumId + "=" + forumID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + topicId });
                else
                    link = ms.UseShortUrls ? Common.Globals.NavigateURL(tabID, string.Empty, new[] { ParamKeys.TopicId + "=" + topicId, ParamKeys.ContentJumpId + "=" + replyId })
                        : Common.Globals.NavigateURL(tabID, string.Empty, new[] { ParamKeys.ForumId + "=" + forumID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + topicId, ParamKeys.ContentJumpId + "=" + replyId });
            }
            else
            {
                var contentId = (replyId > 0) ? replyId : -1;
                link = new Data.Common().GetUrl(moduleID, -1, forumID, topicId, -1, contentId);
            }

            if (!(link.StartsWith("http")))
            {
                if (!link.StartsWith("/"))
                    link = "/" + link;

                if (link.IndexOf(HttpContext.Current.Request.Url.Host, StringComparison.Ordinal) == -1)
                    link = Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + link;
            }


            // Build the forum Url
            var forumURL = ms.UseShortUrls ? Common.Globals.NavigateURL(tabID, string.Empty, new[] { ParamKeys.ForumId + "=" + forumID })
                : Common.Globals.NavigateURL(tabID, string.Empty, new[] { ParamKeys.ForumId + "=" + forumID, ParamKeys.ViewType + "=" + Views.Topics });

            // Build Moderation url
            var modLink = Common.Globals.NavigateURL(tabID, string.Empty, new[] { ParamKeys.ViewType + "=modtopics", ParamKeys.ForumId + "=" + forumID });
            if (modLink.IndexOf(HttpContext.Current.Request.Url.Host, StringComparison.Ordinal) == -1)
                modLink = Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + modLink;

            var result = new StringBuilder(sOut);

            result.Replace("[DISPLAYNAME]", UserProfiles.GetDisplayName(moduleID, userId, authorName, sFirstName, sLastName, sDisplayName));
            result.Replace("[USERNAME]", sUsername);
            result.Replace("[USERID]", userId.ToString());
            result.Replace("[FORUMNAME]", fi.ForumName);
            result.Replace("[PORTALID]", portalID.ToString());
            result.Replace("[FIRSTNAME]", sFirstName);
            result.Replace("[LASTNAME]", sLastName);
            result.Replace("[FULLNAME]", sFirstName + " " + sLastName);
            result.Replace("[GROUPNAME]", fi.GroupName);
            result.Replace("[POSTDATE]", Utilities.GetDate(dateCreated, moduleID, timeZoneOffset));
            result.Replace("[COMMENTS]", comments);
            result.Replace("[PORTALNAME]", portalSettings.PortalName);
            result.Replace("[MODLINK]", "<a href=\"" + modLink + "\">" + modLink + "</a>");
            result.Replace("[LINK]", "<a href=\"" + link + "\">" + link + "</a>");
            result.Replace("[HYPERLINK]", "<a href=\"" + link + "\">" + link + "</a>");
            result.Replace("[LINKURL]", link);
            result.Replace("[FORUMURL]", forumURL);
            result.Replace("[FORUMLINK]", "<a href=\"" + forumURL + "\">" + forumURL + "</a>");

            // Introduced for Active Forum Email Connector plug-in Starts
            if (result.ToString().Contains("[EMAILCONNECTORITEMID]"))
            {
                // This Try with empty catch is introduced here because this code section is for Email Connector functionality only and this section should not 
                // cause any issue to Active Forums functionality in case it does not run successfully.
                try
                {
                    long itemID = GetEmailInfo(portalID, moduleID, forumID, topicId, HttpContext.Current.Request.UserHostAddress);
                    result.Replace("[EMAILCONNECTORITEMID]", itemID.ToString());
                }
                catch
                { }
            }
            // Introduced for Active Forum Email Connector plug-in Ends


            if (user != null)
            {
                result.Replace("[SENDERUSERNAME]", user.UserID.ToString());
                result.Replace("[SENDERFIRSTNAME]", user.FirstName);
                result.Replace("[SENDERLASTNAME]", user.LastName);
                result.Replace("[SENDERDISPLAYNAME]", user.DisplayName);
            }
            else
            {
                result.Replace("[SENDERUSERNAME]", string.Empty);
                result.Replace("[SENDERFIRSTNAME]", string.Empty);
                result.Replace("[SENDERLASTNAME]", string.Empty);
                result.Replace("[SENDERDISPLAYNAME]", string.Empty);
            }

            result.Replace("[SUBJECT]", subject);
            result.Replace("[BODY]", body);
            result.Replace("[Body]", body);

            return result.ToString();
        }

        private static long GetEmailInfo(int siteID, int instanceID, int forumID, int topicID, string ipAddress)
        {
            long ItemID = -1;

            DotNetNuke.Framework.Providers.ProviderConfiguration _providerConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data");
            string connectionString;
            string objectQualifier;
            string databaseOwner;
            connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            var objProvider = (DotNetNuke.Framework.Providers.Provider)(_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);

            objectQualifier = objProvider.Attributes["objectQualifier"];
            if (objectQualifier != "" && objectQualifier.EndsWith("_") == false)
            {
                objectQualifier += "_";
            }

            databaseOwner = objProvider.Attributes["databaseOwner"];
            if (databaseOwner != "" && databaseOwner.EndsWith(".") == false)
            {
                databaseOwner += ".";
            }

            StringBuilder userIds = new StringBuilder();
            userIds.Append("(");

            SubscriptionInfo[] arrSubscriptionInfo = lstSubscriptionInfo.ToArray();
            for (int i = 0; i < arrSubscriptionInfo.Length; i++)
            {
                userIds.Append(arrSubscriptionInfo[i].UserId);
                if (i < arrSubscriptionInfo.Length - 1)
                {
                    userIds.Append(",");
                }
                else
                {
                    userIds.Append(")");
                }
            }

            //dbPrefix = databaseOwner + objectQualifier + databaseObjectPrefix;
            IDataReader dataReader = (IDataReader)(SqlHelper.ExecuteReader(connectionString, databaseOwner + objectQualifier + "ActiveForumsEmailConnector_GetEmailInfo", siteID, instanceID, forumID, topicID, ipAddress, userIds.ToString()));
            if (dataReader.Read())
            {
                ItemID = Convert.ToInt32(dataReader["RecordID"]);
            }

            return ItemID;
        }
        #endregion

        #region Profile

        public static string GetPostInfo(int portalId, int moduleId, int userId, string username, User up, string imagePath, bool isMod, string ipAddress, bool isUserOnline, CurrentUserTypes currentUserType, int currentUserId, bool userPrefHideAvatar, int timeZoneOffset)
        {
            var sPostInfo = ParseProfileInfo(portalId, moduleId, userId, username, up, imagePath, isMod, ipAddress, currentUserType, currentUserId, userPrefHideAvatar, timeZoneOffset);
            if (sPostInfo.ToLower().Contains("<br"))
                return sPostInfo;

            var sr = new StringReader(sPostInfo);
            var sTrim = string.Empty;
            while (sr.Peek() != -1)
            {
                var tmp = sr.ReadLine();
                if (tmp != null && tmp.Trim() != string.Empty)
                {
                    sTrim += tmp.Trim() + "<br />";
                }
            }
            return sTrim;
        }

        public static string ParseProfileInfo(int portalId, int moduleId, int userId, string username, User up, string imagePath, bool isMod, string ipAddress, CurrentUserTypes currentUserType, int currentUserId, bool userPrefHideAvatar, int timeZoneOffset)
        {
            var mainSettings = DataCache.MainSettings(moduleId);

            var cacheKey = string.Format(CacheKeys.PostInfo, moduleId);
            var myTemplate = Convert.ToString(DataCache.CacheRetrieve(cacheKey));
            if (string.IsNullOrEmpty(myTemplate))
            {
                var objTemplateInfo = GetTemplateByName("ProfileInfo", moduleId, portalId);
                myTemplate = objTemplateInfo.TemplateHTML;
                if (cacheKey != string.Empty)
                    DataCache.CacheStore(cacheKey, myTemplate);
            }

            myTemplate = ParseProfileTemplate(myTemplate, up, portalId, moduleId, imagePath, currentUserType, true, userPrefHideAvatar, false, ipAddress, currentUserId, timeZoneOffset);

            return myTemplate;
        }

        public static string ParseProfileTemplate(string profileTemplate, int userId, int portalId, int moduleId, int currentUserId, int timeZoneOffset)
        {
            var uc = new UserController();
            var up = uc.GetUser(portalId, moduleId, userId);

            return ParseProfileTemplate(profileTemplate, up, portalId, moduleId, string.Empty, CurrentUserTypes.Anon, false, false, false, string.Empty, currentUserId, timeZoneOffset);
        }

        public static string ParseProfileTemplate(string profileTemplate, User up, int portalId, int moduleId, string imagePath, CurrentUserTypes currentUserType, int currentUserId, int timeZoneOffset)
        {
            return ParseProfileTemplate(profileTemplate, up, portalId, moduleId, imagePath, currentUserType, false, false, false, string.Empty, currentUserId, timeZoneOffset);
        }

        public static string ParseProfileTemplate(string profileTemplate, User up, int portalId, int moduleId, string imagePath, CurrentUserTypes currentUserType, int timeZoneOffset)
        {
            return ParseProfileTemplate(profileTemplate, up, portalId, moduleId, imagePath, currentUserType, false, false, false, string.Empty, -1, timeZoneOffset);
        }

        public static string ParseProfileTemplate(string profileTemplate, User up, int portalId, int moduleId, string imagePath, CurrentUserTypes currentUserType, bool legacyTemplate, int timeZoneOffset)
        {
            return ParseProfileTemplate(profileTemplate, up, portalId, moduleId, imagePath, currentUserType, false, false, false, string.Empty, -1, timeZoneOffset);
        }

        public static string ParseProfileTemplate(string profileTemplate, User up, int portalId, int moduleId, string imagePath, CurrentUserTypes currentUserType, bool legacyTemplate, bool userPrefHideAvatar, bool userPrefHideSignature, string ipAddress, int timeZoneOffset)
        {
            return ParseProfileTemplate(profileTemplate, up, portalId, moduleId, imagePath, currentUserType, legacyTemplate, userPrefHideAvatar, userPrefHideSignature, ipAddress, -1, timeZoneOffset);
        }


        public static string ParseProfileTemplate(string profileTemplate, User up, int portalId, int moduleId, string imagePath, CurrentUserTypes currentUserType, bool legacyTemplate, bool userPrefHideAvatar, bool userPrefHideSignature, string ipAddress, int currentUserId, int timeZoneOffset)
        {
            try
            {
                if (legacyTemplate)
                    profileTemplate = CleanTemplate(profileTemplate);

                if (up.Profile == null)
                    up = new UserController().FillProfile(portalId, -1, up);

                // TODO figure out why/if this recurion is possible.  Seems a bit scary as it could create a loop.
                if (profileTemplate.Contains("[POSTINFO]"))
                {
                    var sPostInfo = GetPostInfo(portalId, moduleId, up.UserId, up.UserName, up, imagePath, false, ipAddress, up.Profile.IsUserOnline, currentUserType, currentUserId, userPrefHideAvatar, timeZoneOffset);
                    profileTemplate = profileTemplate.Replace("[POSTINFO]", sPostInfo);
                }

                var mainSettings = DataCache.MainSettings(moduleId);

                // Parse DNN profile fields if needed
                var pt = profileTemplate;
                if (pt.IndexOf("[DNN:PROFILE:", StringComparison.Ordinal) >= 0)
                    pt = ParseProfile(portalId, up.UserId, pt, currentUserType, currentUserId);

                // Parse Roles
                if (pt.Contains("[ROLES:"))
                    pt = ParseRoles(pt, (up.UserId == -1) ? string.Empty : up.Profile.Roles);


                var result = new StringBuilder(pt);

                // Used in a few places to determine if info should be shown or removed.
                var isMod = (currentUserType == CurrentUserTypes.Admin || currentUserType == CurrentUserTypes.ForumMod || currentUserType == CurrentUserTypes.SuperUser);

                // Used in a few places to determine if info should be shown or removed.
                var isAdmin = (currentUserType == CurrentUserTypes.Admin || currentUserType == CurrentUserTypes.SuperUser);

                var isAuthethenticated = currentUserType != CurrentUserTypes.Anon;

                // IP Address
                result.Replace("[MODIPADDRESS]", isMod ? ipAddress : string.Empty);


                // User Edit
                result.Replace("[AF:BUTTON:EDITUSER]", isAdmin && up.UserId > 0 ? string.Format("<button class='af-button af-button-edituser' data-id='{0}' data-name='{1}'>[RESX:Edit]</button>", up.UserId, Utilities.JSON.EscapeJsonString(up.DisplayName)) : string.Empty);

                // Points
                var totalPoints = up.PostCount;
                if (mainSettings.EnablePoints && up.UserId > 0 && up.Profile != null)
                {
                    totalPoints = (up.Profile.TopicCount * mainSettings.TopicPointValue) + (up.Profile.ReplyCount * mainSettings.ReplyPointValue) + (up.Profile.AnswerCount * mainSettings.AnswerPointValue) + up.Profile.RewardPoints;
                    result.Replace("[AF:PROFILE:TOTALPOINTS]", totalPoints.ToString());
                    result.Replace("[AF:POINTS:VIEWCOUNT]", up.Profile.ViewCount.ToString());
                    result.Replace("[AF:POINTS:ANSWERCOUNT]", up.Profile.AnswerCount.ToString());
                    result.Replace("[AF:POINTS:REWARDPOINTS]", up.Profile.RewardPoints.ToString());
                }
                else
                {
                    result.Replace("[AF:PROFILE:TOTALPOINTS]", string.Empty);
                    result.Replace("[AF:POINTS:VIEWCOUNT]", string.Empty);
                    result.Replace("[AF:POINTS:ANSWERCOUNT]", string.Empty);
                    result.Replace("[AF:POINTS:REWARDPOINTS]", string.Empty);
                }

                // User Status
                var sUserStatus = string.Empty;
                if (mainSettings.UsersOnlineEnabled && up.UserId > 0 && up.Profile != null)
                    sUserStatus = UserProfiles.UserStatus(imagePath, up.Profile.IsUserOnline, up.UserId, moduleId, "[RESX:UserOnline]", "[RESX:UserOffline]");

                result.Replace("[AF:PROFILE:USERSTATUS]", sUserStatus);
                result.Replace("[AF:PROFILE:USERSTATUS:CSS]", sUserStatus.Contains("online") ? "af-status-online" : "af-status-offline");

                // Rank
                result.Replace("[AF:PROFILE:RANKDISPLAY]", (up.UserId > 0) ? UserProfiles.GetUserRank(portalId, moduleId, up.UserId, totalPoints, 0) : string.Empty);
                result.Replace("[AF:PROFILE:RANKNAME]", (up.UserId > 0) ? UserProfiles.GetUserRank(portalId, moduleId, up.UserId, totalPoints, 1) : string.Empty);

                // PM Image/link
                var pmUrl = string.Empty;
                var pmLink = string.Empty;
                if (up.UserId > 0 && currentUserId >= 0 && up.UserId != currentUserId)
                {
                    switch (mainSettings.PMType)
                    {
                        case PMTypes.Core:
                            pmLink = "<img class='ComposeMessage' data-recipient='{ \"id\": \"user-" + up.UserId + "\", \"name\": \"" + HttpUtility.JavaScriptStringEncode(up.DisplayName) + "\"}' src='" + imagePath + "/images/icon_pm.png' alt=\"[RESX:SendPM]\" title=\"[RESX:SendPM]\" border=\"0\" /></a>";
                            break;

                        case PMTypes.Ventrian:
                            pmUrl = Common.Globals.NavigateURL(mainSettings.PMTabId, string.Empty, new[] { "type=compose", "sendto=" + up.UserId });
                            pmLink = "<a href=\"" + pmUrl + "\"><img src=\"" + imagePath + "/images/icon_pm.png\" alt=\"[RESX:SendPM]\" border=\"0\" /></a>";
                            break;
                    }
                }

                result.Replace("[AF:PROFILE:PMLINK]", pmLink);
                result.Replace("[AF:PROFILE:PMURL]", pmUrl);

                // Signature
                var sSignature = string.Empty;
                if (mainSettings.AllowSignatures != 0 && !userPrefHideSignature && up.Profile != null && !up.Profile.SignatureDisabled)
                {
                    sSignature = up.Profile.Signature;

                    if (sSignature != string.Empty)
                        sSignature = Utilities.ManageImagePath(sSignature);

                    switch (mainSettings.AllowSignatures)
                    {
                        case 1:
                            sSignature = Utilities.HTMLEncode(sSignature);
                            sSignature = sSignature.Replace(System.Environment.NewLine, "<br />");
                            break;
                        case 2:
                            sSignature = Utilities.HTMLDecode(sSignature);
                            break;
                    }
                }

                result.Replace("[AF:PROFILE:SIGNATURE]", sSignature);

                // Avatar
                var sAvatar = string.Empty;
                if (!userPrefHideAvatar && !up.Profile.AvatarDisabled)
                    sAvatar = UserProfiles.GetAvatar(up.UserId, mainSettings.AvatarWidth, mainSettings.AvatarHeight);

                result.Replace("[AF:PROFILE:AVATAR]", sAvatar);

                // Display Name
                result.Replace("[AF:PROFILE:DISPLAYNAME]", UserProfiles.GetDisplayName(moduleId, true, isMod, isAdmin, up.UserId, up.UserName, up.FirstName, up.LastName, up.DisplayName));


                // These fields are no longer used
                result.Replace("[AF:PROFILE:LOCATION]", string.Empty);
                result.Replace("[AF:PROFILE:WEBSITE]", string.Empty);
                result.Replace("[AF:PROFILE:YAHOO]", string.Empty);
                result.Replace("[AF:PROFILE:MSN]", string.Empty);
                result.Replace("[AF:PROFILE:ICQ]", string.Empty);
                result.Replace("[AF:PROFILE:AOL]", string.Empty);
                result.Replace("[AF:PROFILE:OCCUPATION]", string.Empty);
                result.Replace("[AF:PROFILE:INTERESTS]", string.Empty);
                result.Replace("[AF:CONTROL:AVATAREDIT]", string.Empty);
                result.Replace("[AF:BUTTON:PROFILEEDIT]", string.Empty);
                result.Replace("[AF:BUTTON:PROFILESAVE]", string.Empty);
                result.Replace("[AF:BUTTON:PROFILECANCEL]", string.Empty);
                result.Replace("[AF:PROFILE:BIO]", string.Empty);
                result.Replace("[MODUSERSETTINGS]", string.Empty);

                // Date Created
                var sDateCreated = string.Empty;
                var sDateCreatedReplacement = "[AF:PROFILE:DATECREATED]";
                if (up.UserId > 0 && up.Profile != null && up.Profile.DateCreated != null)
                {
                    if (pt.Contains("[AF:PROFILE:DATECREATED:"))
                    {
                        var sFormat = pt.Substring(pt.IndexOf("[AF:PROFILE:DATECREATED:", StringComparison.Ordinal) + (sDateCreatedReplacement.Length), 1);
                        sDateCreated = up.Profile.DateCreated.ToString(sFormat);
                        sDateCreatedReplacement = "[AF:PROFILE:DATECREATED:" + sFormat + "]";
                    }
                    else
                        sDateCreated = Utilities.GetDate(up.Profile.DateCreated, moduleId, timeZoneOffset);
                }
                result.Replace(sDateCreatedReplacement, sDateCreated);

                // Last Activity
                var sDateLastActivity = string.Empty;
                var sDateLastActivityReplacement = "[AF:PROFILE:DATELASTACTIVITY]";

                if (up.Profile.DateLastActivity != null && up.UserId > 0)
                {
                    if (pt.Contains("[AF:PROFILE:DATELASTACTIVITY:"))
                    {
                        string sFormat = pt.Substring(pt.IndexOf("[AF:PROFILE:DATELASTACTIVITY:", StringComparison.Ordinal) + (sDateLastActivityReplacement.Length), 1);
                        sDateLastActivity = up.Profile.DateLastActivity.ToString(sFormat);
                        sDateLastActivityReplacement = "[AF:PROFILE:DATELASTACTIVITY:" + sFormat + "]";
                    }
                    else
                        sDateLastActivity = Utilities.GetDate(up.Profile.DateLastActivity, moduleId, timeZoneOffset);
                }
                result.Replace(sDateLastActivityReplacement, sDateLastActivity);


                // Post Count
                result.Replace("[AF:PROFILE:POSTCOUNT]", (up.PostCount == 0) ? string.Empty : up.PostCount.ToString());
                result.Replace("[AF:PROFILE:USERCAPTION]", up.Profile.UserCaption);
                result.Replace("[AF:PROFILE:USERID]", up.UserId.ToString());
                result.Replace("[AF:PROFILE:USERNAME]", Utilities.HTMLEncode(up.UserName).Replace("&amp;#", "&#"));
                result.Replace("[AF:PROFILE:FIRSTNAME]", Utilities.HTMLEncode(up.FirstName).Replace("&amp;#", "&#"));
                result.Replace("[AF:PROFILE:LASTNAME]", Utilities.HTMLEncode(up.LastName).Replace("&amp;#", "&#"));
                result.Replace("[AF:PROFILE:DATELASTPOST]", (up.Profile.DateLastPost == DateTime.MinValue) ? string.Empty : Utilities.GetDate(up.Profile.DateLastPost, moduleId, timeZoneOffset));
                result.Replace("[AF:PROFILE:TOPICCOUNT]", up.Profile.TopicCount.ToString());
                result.Replace("[AF:PROFILE:REPLYCOUNT]", up.Profile.ReplyCount.ToString());
                result.Replace("[AF:PROFILE:ANSWERCOUNT]", up.Profile.AnswerCount.ToString());
                result.Replace("[AF:PROFILE:REWARDPOINTS]", up.Profile.RewardPoints.ToString());
                result.Replace("[AF:PROFILE:EMAIL]", up.Email);


                return result.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        #endregion

        public static string ParseRoles(string template, string userRoles)
        {
            if (string.IsNullOrWhiteSpace(template))
                return template;

            var userRoleArray = string.IsNullOrWhiteSpace(userRoles) ? null : userRoles.Split(';').Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Trim()).ToList();

            const string pattern = @"\[ROLES:(.+?)\]";

            template = Regex.Replace(template, pattern, delegate(Match match)
            {
                if (userRoleArray == null || userRoleArray.Count == 0)
                    return string.Empty;

                var roles = match.Groups[1].Value.Split(';').Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Trim());

                var replacement = roles.FirstOrDefault(role => role != "-10" && userRoleArray.Contains(role));

                return replacement ?? string.Empty;
            });

            return template;
        }

        private static string CleanTemplate(string template)
        {
            const string pattern = "(\\[.+?\\])";

            var sb = new StringBuilder(template);

            foreach (Match match in Regex.Matches(template, pattern))
            {
                var sReplace = string.Empty;

                switch (match.Value)
                {
                    case "[RANKNAME]":
                        sb.Replace(match.Value, "[AF:PROFILE:RANKNAME]");
                        break;
                    case "[RANKDISPLAY]":
                        sb.Replace(match.Value, "[AF:PROFILE:RANKDISPLAY]");
                        break;
                    case "[AF:PROFILE:LASTACTIVE]":
                        sb.Replace(match.Value, "[AF:PROFILE:DATELASTACTIVITY]");
                        break;
                    case "[MEMBERSINCE]":
                        sb.Replace(match.Value, "[AF:PROFILE:DATECREATED]");
                        break;
                    case "[AF:PROFILE:MEMBERSINCE]":
                        sb.Replace(match.Value, "[AF:PROFILE:DATECREATED]");
                        break;
                    case "[USERSTATUS]":
                        sb.Replace(match.Value, "[AF:PROFILE:USERSTATUS]");
                        break;
                    case "[USERCAPTION]":
                        sb.Replace(match.Value, "[AF:PROFILE:USERCAPTION]");
                        break;
                    case "[USERNAME]":
                        sb.Replace(match.Value, "[AF:PROFILE:USERNAME]");
                        break;
                    case "[USERID]":
                        sb.Replace(match.Value, "[AF:PROFILE:USERID]");
                        break;
                    case "[DISPLAYNAME]":
                        sb.Replace(match.Value, "[AF:PROFILE:DISPLAYNAME]");
                        break;
                    case "[POSTS]":
                        sb.Replace(match.Value, "[AF:PROFILE:POSTCOUNT]");
                        break;
                    case "[AVATAR]":
                        sb.Replace(match.Value, "[AF:PROFILE:AVATAR]");
                        break;
                    case "[LOCATION]":
                        sb.Replace(match.Value, "[AF:PROFILE:LOCATION]");
                        break;
                    case "[WEBSITE]":
                        sb.Replace(match.Value, "[AF:PROFILE:WEBSITE]");
                        break;
                    case "[AF:POINTS:TOPICCOUNT]":
                        sb.Replace(match.Value, "[AF:PROFILE:TOPICCOUNT]");
                        break;
                    case "[AF:POINTS:REPLYCOUNT]":
                        sb.Replace(match.Value, "[AF:PROFILE:REPLYCOUNT]");
                        break;
                    case "[SIGNATURE]":
                        sb.Replace(match.Value, "[AF:PROFILE:SIGNATURE]");

                        break;
                }
            }

            return sb.ToString();
        }

        public static string GetTemplateSection(string template, string startTag, string endTag, bool returnTemplateIfTagNotFound = true)
        {
            var intStartTag = template.IndexOf(startTag, StringComparison.Ordinal);
            var intEndTag = template.IndexOf(endTag, StringComparison.Ordinal);
            if (intStartTag >= 0 && intEndTag > intStartTag)
            {
                var intSubTempStart = intStartTag + startTag.Length;
                var intSubTempEnd = intEndTag;
                var intSubTempLength = intSubTempEnd - intSubTempStart;
                var sSubTemp = template.Substring(intSubTempStart, intSubTempLength);
                return sSubTemp;
            }

            return returnTemplateIfTagNotFound ? template : string.Empty;
        }

        public static string ReplaceSubSection(string template, string subTemplate, string startTag, string endTag)
        {
            var intStartTag = template.IndexOf(startTag, StringComparison.Ordinal);
            var intEndTag = template.IndexOf(endTag, StringComparison.Ordinal);
            if (intStartTag >= 0 && intEndTag > intStartTag)
            {
                var intSubTempStart = intStartTag + startTag.Length;
                var intSubTempEnd = intEndTag - 1;
                var intSubTempLength = intSubTempEnd - intSubTempStart;
                template = template.Substring(0, intStartTag) + subTemplate + template.Substring(intEndTag + endTag.Length);
            }
            return template;
        }

        public static string ParseProfile(int portalId, int userId, string template, CurrentUserTypes currentUserType, int currentUserId)
        {
            var objuser = Entities.Users.UserController.GetUser(portalId, userId, true);

            var s = template ?? string.Empty;

            const string pattern = "(\\[DNN:PROFILE:(.+?)\\])";

            foreach (Match match in Regex.Matches(s, pattern))
            {
                var sReplace = string.Empty;
                var sResource = string.Empty;
                if (objuser != null)
                {
                    var profproperties = objuser.Profile.ProfileProperties;
                    var profprop = profproperties.GetByName(match.Groups[2].Value);
                    sResource = "ProfileProperties_{0}";
                    if (profprop != null)
                    {
                        sResource = string.Format(sResource, match.Groups[2].Value);

                        if (profprop.Visibility == Entities.Users.UserVisibilityMode.AdminOnly && (currentUserType != CurrentUserTypes.Anon || currentUserType != CurrentUserTypes.Auth))
                            sReplace = profprop.PropertyValue;
                        else if (profprop.Visibility == Entities.Users.UserVisibilityMode.MembersOnly && currentUserType != CurrentUserTypes.Anon)
                            sReplace = profprop.PropertyValue;
                        else if (profprop.Visibility == Entities.Users.UserVisibilityMode.AllUsers)
                            sReplace = profprop.PropertyValue;
                        else
                            sReplace = "[RESX:Private]";

                        sResource = Services.Localization.Localization.GetString(sResource, "~/admin/users/app_localresources/profile.ascx.resx");
                    }
                }
                s = s.Replace(match.Value, sReplace);
                s = s.Replace("[RESX:DNNProfile:" + match.Groups[2].Value + "]", sResource);
            }
            return s;
        }

        private static string GetTopicTemplate(int topicTemplateId, int moduleId)
        {
            var mainSettings = DataCache.MainSettings(moduleId);

            return DataCache.GetCachedTemplate(mainSettings.TemplateCache, moduleId, "TopicView", topicTemplateId);
        }

        public static string PreviewTopic(int topicTemplateID, int portalId, int moduleId, int tabId, Forum forumInfo, int userId, string body, string imagePath, User up, DateTime postDate, CurrentUserTypes currentUserType, int currentUserId, int timeZoneOffset)
        {
            var sTemplate = GetTopicTemplate(topicTemplateID, moduleId);
            try
            {
                var mainSettings = DataCache.MainSettings(moduleId);
                var sTopic = GetTemplateSection(sTemplate, "[TOPIC]", "[/TOPIC]");
                sTopic = sTopic.Replace("[ACTIONS:ALERT]", string.Empty);
                sTopic = sTopic.Replace("[ACTIONS:EDIT]", string.Empty);
                sTopic = sTopic.Replace("[ACTIONS:DELETE]", string.Empty);
                sTopic = sTopic.Replace("[ACTIONS:QUOTE]", string.Empty);
                sTopic = sTopic.Replace("[ACTIONS:REPLY]", string.Empty);
                sTopic = sTopic.Replace("[POSTDATE]", Utilities.GetDate(postDate, moduleId, timeZoneOffset));
                sTopic = sTopic.Replace("[POSTINFO]", GetPostInfo(portalId, moduleId, userId, up.UserName, up, imagePath, false, HttpContext.Current.Request.UserHostAddress, true, currentUserType, currentUserId, false, timeZoneOffset));
                sTemplate = ParsePreview(portalId, sTopic, body, moduleId);
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

        private static string ParsePreview(int portalId, string template, string message, int moduleId)
        {
            //TODO: Legacy Attachments Functionality - Probably can remove.
            if (message.Contains("&#91;IMAGE:"))
            {
                var strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
                const string pattern = "(&#91;IMAGE:(.+?)&#93;)";
                foreach (Match match in Regex.Matches(message, pattern))
                {
                    var sImage = "<img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + portalId + "&moduleid=" + moduleId + "&attachid=" + match.Groups[2].Value + "\" border=\"0\" />";
                    message = message.Replace(match.Value, sImage);
                }
            }

            //TODO: Legacy Attachments Functionality - Probably can remove.
            if (message.Contains("&#91;THUMBNAIL:"))
            {
                var strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
                const string pattern = "(&#91;THUMBNAIL:(.+?)&#93;)";
                foreach (Match match in Regex.Matches(message, pattern))
                {
                    var thumbId = match.Groups[2].Value.Split(':')[0];
                    var parentId = match.Groups[2].Value.Split(':')[1];
                    var sImage = "<a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + portalId + "&moduleid=" + moduleId + "&attachid=" + parentId + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + portalId + "&moduleid=" + moduleId + "&attachid=" + thumbId + "\" border=\"0\" /></a>";
                    message = message.Replace(match.Value, sImage);
                }
            }

            template = template.Replace("[BODY]", message);
            if (Regex.IsMatch(template, "<CODE([^>]*)>", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(message, "<CODE([^>]*)>", RegexOptions.IgnoreCase))
                {
                    foreach (Match m in Regex.Matches(message, "<CODE([^>]*)>(.*?)</CODE>", RegexOptions.IgnoreCase))
                        message = message.Replace(m.Value, m.Value.Replace("<br>", System.Environment.NewLine));
                }
                var objCode = new CodeParser();
                template = CodeParser.ParseCode(Utilities.HTMLDecode(template));
            }
            return template;
        }

        internal static string ParseSpecial(string template, SpecialTokenTypes tokenType, string url, string title, bool canRead, string options = "")
        {
            switch (tokenType)
            {
                case SpecialTokenTypes.AddThis:
                    return ParseAddThis(template, url, title, canRead, options);
            }

            return template;
        }

        private static string ParseAddThis(string template, string url, string title, bool canRead, string options)
        {
            if (options == string.Empty)
                template = template.Replace("[AF:CONTROL:ADDTHIS:0]", string.Empty);

            const string pattern = "(\\[AF:CONTROL:ADDTHIS:(.*?)\\])";

            var sFind = string.Empty;
            var sUserName = string.Empty;

            foreach (Match match in Regex.Matches(template, pattern))
            {
                sFind = match.Groups[0].Value;
                sUserName = match.Groups[2].Value;
            }

            if (sFind != string.Empty)
            {
                var replace = string.Empty;
                if ((sUserName == "0" && options != string.Empty) || sUserName != "0")
                {
                    if (canRead)
                    {
                        if (sUserName == "0" && options != string.Empty)
                            sUserName = options;

                        replace = DataCache.GetTemplate("AddThis.txt");
                        replace = replace.Replace("[USERNAME]", sUserName.Replace("'", "\\'"));
                        replace = replace.Replace("[URL]", url);
                        replace = replace.Replace("[TITLE]", title.Replace("'", "\\'"));
                    }
                }

                template = template.Replace(sFind, replace);
                if (sFind != string.Empty)
                    template = template.Replace(sFind, string.Empty);
            }


            return template;
        }

    }
}
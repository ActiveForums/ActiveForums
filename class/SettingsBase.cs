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
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.ActiveForums.Data;

namespace DotNetNuke.Modules.ActiveForums
{
    public class SettingsBase : PortalModuleBase
    {
        #region Private Members
        private int _forumModuleId = -1;
        private string _LoadView = "";
        private int _LoadGroupForumID = 0;
        private int _LoadPostID = 0;
        private string _imagePath = string.Empty;
        private string _Params = string.Empty;
        private int _forumTabId = -1;
        #endregion

        #region Public Properties
        internal User ForumUser
        {
            get
            {
                var uc = new UserController();
                return uc.GetUser(PortalId, ForumModuleId);
            }
        }
        internal string UserForumsList
        {
            get
            {
                string forums;
                if (string.IsNullOrEmpty(ForumUser.UserForums))
                {
                    var fc = new ForumController();
                    forums = fc.GetForumsForUser(ForumUser.UserRoles, PortalId, ForumModuleId);
                    ForumUser.UserForums = forums;
                }
                else
                {
                    forums = ForumUser.UserForums;
                }
                return forums;
            }
        }
        public int ForumModuleId
        {
            get
            {
                if (_forumModuleId > 0)
                {
                    return _forumModuleId;
                }
                return ModuleId;
            }
            set
            {
                _forumModuleId = value;
            }
        }
        public int ForumTabId
        {
            get
            {
                return _forumTabId;
            }
            set
            {
                _forumTabId = value;
            }
        }
        public string Params
        {
            get
            {
                return _Params;
            }
            set
            {
                _Params = value;
            }
        }
        public bool UseAjax
        {
            get
            {
                bool tempUseAjax = Request.IsAuthenticated && UserPrefUseAjax;

                return tempUseAjax;
            }
        }
        public int PageId
        {
            get
            {
                int tempPageId = 0;
                if (Request.QueryString[ParamKeys.PageId] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.QueryString[ParamKeys.PageId]))
                    {
                        tempPageId = Convert.ToInt32(Request.QueryString[ParamKeys.PageId]);
                    }
                }
                else if (Request.QueryString["page"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.QueryString["page"]))
                    {
                        tempPageId = Convert.ToInt32(Request.QueryString["page"]);
                    }
                }
                else if (Params != string.Empty && Params.Contains("PageId"))
                {
                    tempPageId = Convert.ToInt32(Params.Split('=')[1]);
                }
                else
                {
                    tempPageId = 1;
                }
                return tempPageId;
            }
        }
        private bool _ShowToolbar = true;
        public bool ShowToolbar
        {
            get
            {
                return _ShowToolbar;
            }
            set
            {
                _ShowToolbar = value;
            }
        }
        #endregion

        public UserController UserController
        {
            get
            {
                const string userControllerContextKey = "AF|UserController";
                var userController = HttpContext.Current.Items[userControllerContextKey] as UserController;
                if (userController == null)
                {
                    userController = new UserController();
                    HttpContext.Current.Items[userControllerContextKey] = userController;
                }
                return userController;
            }
        }

        public ForumController ForumController
        {
            get
            {
                const string forumControllerContextKey = "AF|ForumController";
                var forumController = HttpContext.Current.Items[forumControllerContextKey] as ForumController;
                if (forumController == null)
                {
                    forumController = new ForumController();
                    HttpContext.Current.Items[forumControllerContextKey] = forumController;
                }
                return forumController;
            }
        }

        public ForumsDB ForumsDB
        {
            get
            {
                const string forumsDBContextKey = "AF|ForumsDB";
                var forumsDB = HttpContext.Current.Items[forumsDBContextKey] as ForumsDB;
                if (forumsDB == null)
                {
                    forumsDB = new ForumsDB();
                    HttpContext.Current.Items[forumsDBContextKey] = forumsDB;
                }
                return forumsDB;
            }
        }


        #region Public Properties - User Preferences
        public CurrentUserTypes CurrentUserType
        {
            get
            {
                if (Request.IsAuthenticated)
                {
                    if (UserInfo.IsSuperUser)
                    {
                        return CurrentUserTypes.SuperUser;
                    }
                    if (HasModulePermission("EDIT"))
                    {
                        return CurrentUserTypes.Admin;
                    }
                    if (ForumUser.Profile.IsMod)
                    {
                        return CurrentUserTypes.ForumMod;
                    }
                    return CurrentUserTypes.Auth;
                }
                return CurrentUserTypes.Anon;
            }
        }
        public bool UserIsMod
        {
            get
            {
                if (UserId == -1)
                {
                    return false;
                }
                if (ForumUser != null)
                {
                    return ForumUser.Profile.IsMod;
                }
                return false;
            }
        }
        public string UserDefaultSort
        {
            get
            {
                if (UserId != -1)
                {
                    return ForumUser.Profile.PrefDefaultSort;
                }
                return "ASC";
            }
        }
        public int UserDefaultPageSize
        {
            get
            {
                if (UserId != -1)
                {
                    return ForumUser.Profile.PrefPageSize;
                }
                return MainSettings.PageSize;
            }
        }
        public bool UserPrefHideSigs
        {
            get
            {
                if (UserId != -1)
                {
                    try
                    {
                        return ForumUser.Profile.PrefBlockSignatures;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                return false;
            }
        }
        public bool UserPrefHideAvatars
        {
            get
            {
                if (UserId != -1)
                {
                    return ForumUser.Profile.PrefBlockAvatars;
                }
                return false;
            }
        }
        public bool UserPrefJumpLastPost
        {
            get
            {
                if (UserId != -1)
                {
                    return ForumUser.Profile.PrefJumpLastPost;
                }
                return false;
            }
        }
        public bool UserPrefUseAjax
        {
            get
            {
                if (UserId != -1)
                {
                    return ForumUser.Profile.PrefUseAjax;
                }
                return false;
            }
        }
        public bool UserPrefShowReplies
        {
            get
            {
                if (UserId != -1)
                {
                    return ForumUser.Profile.PrefDefaultShowReplies;
                }
                return false;
            }
        }
        public bool UserPrefTopicSubscribe
        {
            get
            {
                if (UserId != -1)
                {
                    return ForumUser.Profile.PrefTopicSubscribe;
                }
                return false;
            }
        }
        #endregion
        #region Public ReadOnly Properties
        public Framework.CDefault BasePage
        {
            get
            {
                return (Framework.CDefault)Page;
            }
        }
        public SettingsInfo MainSettings
        {
            get
            {
                ForumModuleId = _forumModuleId <= 0 ? ForumModuleId : _forumModuleId;

                var _portalSettings = (PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
                var objModules = new Entities.Modules.ModuleController();
                var objSettings = new SettingsInfo {MainSettings = objModules.GetModuleSettings(ForumModuleId)};

                return objSettings;
            }
        }
        public string ImagePath
        {
            get
            {
                return Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MainSettings.Theme);
            }
        }
        public string GetViewType
        {
            get
            {
                if (Request.Params[ParamKeys.ViewType] != null)
                {
                    return Request.Params[ParamKeys.ViewType].ToUpperInvariant();
                }
                if (Request.Params["view"] != null)
                {
                    return Request.Params["view"].ToUpperInvariant();
                }
                return null;
            }
        }
        public int TimeZoneOffset
        {
            /* the code that uses this expects minutes to be returned
             * this method now compares the user's timezone to the portaltimezone and returns the offset for that difference.
             * it appears the AF module doesn't store dates as UTC, so you can't offset from UTC             * 
             */
            get
            {
                int timeOffset = 0;
                if (UserId > 0)
                {
                    try
                    {
                        if (UserInfo.Profile.PreferredTimeZone != null)
                        {
                            /* get the portal timezone offset so we can calculate differences for users */
                            int portalTimeZone = Convert.ToInt32(PortalController.GetCurrentPortalSettings().TimeZone.BaseUtcOffset.TotalMinutes);
                            int userTimeZone = Convert.ToInt32(UserInfo.Profile.PreferredTimeZone.BaseUtcOffset.TotalMinutes);
                            if (userTimeZone != portalTimeZone)
                            {
                                timeOffset = Math.Abs(portalTimeZone - userTimeZone);
                                if (portalTimeZone > userTimeZone)
                                    timeOffset = -timeOffset; 
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        timeOffset = 0;
                    }
                }
                return timeOffset;
            }
        }

        #endregion
        #region Protected Methods
        protected string GetDate(DateTime DisplayDate)
        {
            return Utilities.GetDate(DisplayDate, ModuleId, TimeZoneOffset);
        }

        protected DateTime GetUserDate(DateTime displayDate)
        {
            return Utilities.GetUserDate(displayDate, ModuleId, TimeZoneOffset);
        }

        protected string GetServerDateTime(DateTime DisplayDate)
        {
            //Dim newDate As Date 
            string dateString;
            var ps = (PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
            try
            {
                int mServerOffSet = 0;
                mServerOffSet = ps.TimeZoneOffset;
                dateString = DisplayDate.ToString(MainSettings.DateFormatString + " " + MainSettings.TimeFormatString);
                return dateString;
            }
            catch (Exception ex)
            {
                dateString = DisplayDate.ToString();
                return dateString;
            }
        }
        #endregion

        #region Public Methods
        public string NavigateUrl(int TabId)
        {
            return Utilities.NavigateUrl(TabId);
        }
        public string NavigateUrl(int TabId, string ControlKey, params string[] AdditionalParameters)
        {
            return Utilities.NavigateUrl(TabId, ControlKey, AdditionalParameters);
        }
        private string[] AddParams(string param, string[] currParams)
        {
            var tmpParams = new[] { param };
            int intLength = tmpParams.Length;
            Array.Resize(ref tmpParams, (intLength + currParams.Length));
            currParams.CopyTo(tmpParams, intLength);
            return tmpParams;
        }

        public void RenderMessage(string Title, string Message)
        {
            RenderMessage(Utilities.GetSharedResource(Title), Message, string.Empty, null);
        }
        public void RenderMessage(string Message, string ErrorMsg, Exception ex)
        {
            RenderMessage(Utilities.GetSharedResource("[RESX:Error]"), Message, ErrorMsg, ex);
        }
        public void RenderMessage(string Title, string Message, string ErrorMsg, Exception ex)
        {
            var im = new Controls.InfoMessage {Message = Utilities.GetSharedResource(Message) + "<br />"};
            if (ex != null)
            {
                im.Message = im.Message + ex.Message;
            }
            if (ex != null)
            {
                Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
            }

        }


        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (Request.Params["view"] != null)
            {
                string sUrl;
                string sParams = string.Empty;
                if (Request.Params["forumid"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params["ForumId"]))
                    {
                        sParams = ParamKeys.ForumId + "=" + Request.Params["ForumId"];
                    }
                }
                if (Request.Params["postid"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params["postid"]))
                    {
                        sParams += "|" + ParamKeys.TopicId + "=" + Request.Params["postid"];
                    }
                }
                sParams += "|" + ParamKeys.ViewType + "=" + Request.Params["view"];
                sUrl = NavigateUrl(TabId, "", sParams.Split('|'));
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", sUrl);
            }

            Framework.jQuery.RequestRegistration();

        }
        #endregion



    }
}


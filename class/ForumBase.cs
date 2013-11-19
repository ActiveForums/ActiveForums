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
using System.Xml;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.ActiveForums
{
    public class ForumBase : SettingsBase
    {
        #region Private Member Variables

        private int? _forumId;
        private string _forumIds = string.Empty;
        private int? _forumGroupId;
        private int _parentForumId = -1;
        private int? _postId;
        private int? _topicId; // = -1;
        private int? _replyId;
        private int? _quoteId;
        private bool? _jumpToLastPost;
        private string _defaultView = Views.ForumView;
        private int _defaultForumViewTemplateId = -1;
        private int _defaultTopicsViewTemplateId = -1;
        private int _defaultTopicViewTemplateId = -1;
        private string _templatePath = string.Empty;
        private string _templateFile = string.Empty;
        private Forum _foruminfo;
        private XmlDocument _forumData;

        private bool? _canRead;
        private bool? _canView;
        private bool? _canCreate;
        private bool? _canReply;

        #endregion

        #region Public Properties

        public XmlDocument ForumData
        {
            get
            {
                if(_forumData == null)
                    return ControlConfig != null ? ForumsDB.ForumListXML(ControlConfig.SiteId, ControlConfig.InstanceId) : ForumsDB.ForumListXML(PortalId, ModuleId); 

                return _forumData;
            }

            set { _forumData = value; }
        }

        public ControlsConfig ControlConfig { get; set; }

        public string ThemePath
        {
            get
            {
                return Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MainSettings.Theme + "/");
            }
        }

        public string ForumIds
        {
            get
            {
                return _forumIds;
            }
            set
            {
                _forumIds = value;
            }
        }

        public int DefaultForumViewTemplateId
        {
            get
            {
                return _defaultForumViewTemplateId;
            }
            set
            {
                _defaultForumViewTemplateId = value;
            }
        }

        public string TemplatePath
        {
            get
            {
                return _templatePath;
            }
            set
            {
                _templatePath = value;
            }
        }

        public bool UseTemplatePath { get; set; }

        public int DefaultTopicsViewTemplateId
        {
            get
            {
                return _defaultTopicsViewTemplateId;
            }
            set
            {
                _defaultTopicsViewTemplateId = value;
            }
        }

        public int DefaultTopicViewTemplateId
        {
            get
            {
                return _defaultTopicViewTemplateId;
            }
            set
            {
                _defaultTopicViewTemplateId = value;
            }
        }

        public string DefaultView
        {
            get
            {

                return _defaultView;
            }
            set
            {

                _defaultView = value;
            }
        }

        public bool InheritModuleCSS { get; set; }

        public bool JumpToLastPost
        {
            get
            {
                if (!Request.IsAuthenticated)
                {
                    return false;
                }

                if(!_jumpToLastPost.HasValue)
                {
                    _jumpToLastPost = UserController.GetUser(PortalId, UserId).Profile.PrefJumpLastPost;
                }

                return _jumpToLastPost.Value;
            }
        }

        public DateTime UserLastAccess
        {
            get
            {
                if (!Request.IsAuthenticated)
                    return Utilities.NullDate();

                var lastAccess = Session[UserId.ToString() + ModuleId + "LastAccess"];
                return  lastAccess == null ? Utilities.NullDate() : Convert.ToDateTime(lastAccess);
            }
            set
            {
                Session[UserId.ToString() + ModuleId + "LastAccess"] = value;
            }
        }

        public int PostId
        {
            get
            {
                // If the id has already been set, return it.
                if(_postId.HasValue)
                    return _postId.Value;

                // If there is no id in the query string, set it to the default value and return it.
                var queryPostId = Request.QueryString[ParamKeys.PostId];
                if(string.IsNullOrWhiteSpace(queryPostId))
                {
                    _postId = 0;
                    return _postId.Value;
                }

                // If there is a hash tag in the query value, remove it and anything after it before parsing.
                var hashIndex = queryPostId.IndexOf("#", 0, StringComparison.Ordinal);
                if (hashIndex >= 0)
                    queryPostId = queryPostId.Substring(0, hashIndex);

                // Try to parse the id, if it doesn't work, return the default value.
                int parsedPostId;
                _postId = int.TryParse(queryPostId, out parsedPostId) ? parsedPostId : 0;

                return _postId.Value;
            }
        }

        public int TopicId
        {
            get
            {
                // If the id has already been set, return it.
                if (_topicId.HasValue)
                    return _topicId.Value;

                // If there is no id in the query string, set it to the default value and return it.
                var queryTopicId = Request.QueryString[ParamKeys.TopicId];
                if(string.IsNullOrWhiteSpace(queryTopicId))
                {
                    _topicId = PostId > 0 ? PostId : -1; // If we have no topic id, but we do have a post id, use that instead.  Need to track down where this is used.
                    return _topicId.Value;
                }

                // If there is a hash tag in the query value, remove it and anything after it before parsing.
                var hashIndex = queryTopicId.IndexOf("#", 0, StringComparison.Ordinal);
                if (hashIndex >= 0)
                    queryTopicId = queryTopicId.Substring(0, hashIndex);

                // Try to parse the id, if it doesn't work, return the default value.
                int parsedTopicId;
                _topicId = int.TryParse(queryTopicId, out parsedTopicId) ? parsedTopicId : -1;

                return _topicId.Value;
            }
            set
            {
                _topicId = value;
            }
        }

        public int ReplyId
        {
            get
            {
                // If the id has already been set, return it.
                if (_replyId.HasValue)
                    return _replyId.Value;

                // If there is no id in the query string, set it to the default value and return it.
                var queryReplyId = Request.QueryString[ParamKeys.ReplyId];
                if (string.IsNullOrWhiteSpace(queryReplyId))
                {
                    _replyId = 0;
                    return _replyId.Value;
                }

                // If there is a hash tag in the query value, remove it and anything after it before parsing.
                var hashIndex = queryReplyId.IndexOf("#", 0, StringComparison.Ordinal);
                if (hashIndex >= 0)
                    queryReplyId = queryReplyId.Substring(0, hashIndex);

                // Try to parse the id, if it doesn't work, return the default value.
                int parsedReplyId;
                _replyId = int.TryParse(queryReplyId, out parsedReplyId) ? parsedReplyId : 0;

                return _replyId.Value;
            }
            set
            {
                _replyId = value;
            }
        }

        public int QuoteId
        {
            get
            {
                // If the id has already been set, return it.
                if (_quoteId.HasValue)
                    return _quoteId.Value;

                // If there is no id in the query string, set it to the default value and return it.
                var queryQuoteId = Request.QueryString[ParamKeys.QuoteId];
                if (string.IsNullOrWhiteSpace(queryQuoteId))
                {
                    _quoteId = 0;
                    return _quoteId.Value;
                }

                // If there is a hash tag in the query value, remove it and anything after it before parsing.
                var hashIndex = queryQuoteId.IndexOf("#", 0, StringComparison.Ordinal);
                if (hashIndex >= 0)
                    queryQuoteId = queryQuoteId.Substring(0, hashIndex);

                // Try to parse the id, if it doesn't work, return the default value.
                int parsedQuoteId;
                _quoteId = int.TryParse(queryQuoteId, out parsedQuoteId) ? parsedQuoteId : 0;

                return _quoteId.Value;
            }
            set
            {
                _quoteId = value;
            }
        }

        public int ForumId
        {
            get
            {
                // If the id has already been set, return it.
                if (_forumId.HasValue)
                    return _forumId.Value;

                // Set out default value
                _forumId = -1;

                // If there is an id in the query string, parse it
                var queryForumId = Request.QueryString[ParamKeys.ForumId];
                if (!string.IsNullOrWhiteSpace(queryForumId))
                {
                    // Try to parse the id, if it doesn't work, return the default value.
                    int parsedForumId;
                    _forumId = int.TryParse(queryForumId, out parsedForumId) ? parsedForumId : 0;
                }

                // If we don't have a forum id at this point, try and pull it from "forumid" in the query string
                if (_forumId < 1)
                {
                    queryForumId = Request.QueryString["forumid"];
                    if (!string.IsNullOrWhiteSpace(queryForumId))
                    {
                        // Try to parse the id, if it doesn't work, return the default value.
                        int parsedForumId;
                        _forumId = int.TryParse(queryForumId, out parsedForumId) ? parsedForumId : 0;
                    }
                }

                // If we still don't have a forum id, but we have a topic id, look up the forum id
                if (_forumId < 1 & TopicId > 0)
                {
                   _forumId = ForumsDB.Forum_GetByTopicId(TopicId);
                }

                return _forumId.Value;
            }
            set
            {
                _forumId = value;
            }
        }

        public int ForumGroupId
        {
            get
            {
                // If the id has already been set, return it.
                if (_forumGroupId.HasValue)
                    return _forumGroupId.Value;

                // If there is no id in the query string, set it to the default value and return it.
                var queryForumGroupId = Request.QueryString[ParamKeys.GroupId];
                if (string.IsNullOrWhiteSpace(queryForumGroupId))
                {
                    _forumGroupId = -1;
                    return _forumGroupId.Value;
                }

                // If there is a hash tag in the query value, remove it and anything after it before parsing.
                var hashIndex = queryForumGroupId.IndexOf("#", 0, StringComparison.Ordinal);
                if (hashIndex >= 0)
                    queryForumGroupId = queryForumGroupId.Substring(0, hashIndex);

                // Try to parse the id, if it doesn't work, return the default value.
                int parsedForumGroupId;
                _forumGroupId = int.TryParse(queryForumGroupId, out parsedForumGroupId) ? parsedForumGroupId : 0;

                return _forumGroupId.Value;
            }
            set
            {
                _forumGroupId = value;
            }
        }

        public int ParentForumId
        {
            get
            {
                return _parentForumId;
            }
            set
            {
                _parentForumId = value;
            }
        }

        public string TemplateFile
        {
            get
            {
                return _templateFile;
            }
            set
            {
                _templateFile = value;
            }
        }

        public Forum ForumInfo
        {
            get 
            {
                return _foruminfo ?? (_foruminfo = ForumController.Forums_Get(PortalId, ForumModuleId, ForumId, UserId, true, true, TopicId));
            }
            set
            {
                _foruminfo = value;
            }
        }

        public int SocialGroupId { get; set; }

        public bool CanRead
        {
            get
            {
                if(!_canRead.HasValue)
                    _canRead =  SecurityCheck("read");

                return _canRead.Value;
            }
        }

        public bool CanView
        {
            get
            {
                if(!_canView.HasValue)
                    _canView = SecurityCheck("view");

                return _canView.Value;
            }
        }

        public bool CanCreate
        {
            get
            {
                if(!_canCreate.HasValue)
                {
                    // The basic security check trumps everything.
                    if (!SecurityCheck("create"))
                        _canCreate = false;

                    // Admins and trusted users shall pass!
                    else if (ForumUser.IsAdmin || ForumUser.IsSuperUser || Permissions.HasPerm(ForumInfo.Security.Trust, ForumUser.UserRoles))
                        _canCreate = true;

                    // If CreatePostCount is not set, no need to go further
                    else if (ForumInfo.CreatePostCount <= 0)
                        _canCreate = true;

                    // If we don't have a valid user, there is no way they could meed the minumum post count requirement
                    else if (ForumUser.UserId <= 0)
                        _canCreate = false;

                    else
                        _canCreate = ForumUser.PostCount >= ForumInfo.CreatePostCount; 
                }

                return _canCreate.Value;
            }
        }

        public bool CanReply
        {
            get
            {
                if(!_canReply.HasValue)
                {
                    // The basic security check trumps everything.
                    if (!SecurityCheck("reply"))
                        _canReply = false;

                    // Admins and trusted users shall pass!
                    else if (ForumUser.IsAdmin || ForumUser.IsSuperUser || Permissions.HasPerm(ForumInfo.Security.Trust, ForumUser.UserRoles))
                        _canReply = true;

                    // If ReplyPostCount is not set, no need to go further
                    else if (ForumInfo.ReplyPostCount <= 0)
                        _canReply = true;

                    // If we don't have a valid user, there is no way they could meed the minumum post count requirement
                    else if (ForumUser.UserId <= 0)
                        _canReply = false;

                    else
                        _canReply = ForumUser.PostCount >= ForumInfo.ReplyPostCount;   
                }

                return _canReply.Value;
            }
        }


        #endregion

        #region Helper Methods

        private bool SecurityCheck(string secType)
        {
            if (ForumUser == null)
            {
                return false;
            }

            var xNode = ForumData.SelectSingleNode("//forums/forum[@forumid='" + ForumId + "']/security/" + secType);

            if (xNode == null)
            {
                return false;
            }

            var secRoles = xNode.InnerText;

            return Permissions.HasPerm(secRoles, ForumUser.UserRoles);
        }

        protected string GetSharedResource(string key)
        {
            return Localization.GetString(key, "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx");
        }

        internal bool IsHtmlPermitted(HTMLPermittedUsers permittedMode, bool userIsTrusted, bool userIsModerator)
        {
            if (permittedMode == HTMLPermittedUsers.AllUsers)
                return true;

            if (permittedMode == HTMLPermittedUsers.AuthenticatedUsers && Request.IsAuthenticated)
                return true;

            if (permittedMode == HTMLPermittedUsers.TrustedUsers && userIsTrusted)
                return true;

            if (permittedMode == HTMLPermittedUsers.Moderators && userIsModerator)
                return true;

            if (permittedMode == HTMLPermittedUsers.Administrators && HasModulePermission("EDIT"))
                return true;

            return false;
        }

        #endregion

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            // If printmode, simply exit.
            if (Request.QueryString["dnnprintmode"] != null)
                return;

            // Do a conversion if needed 
            // TODO: Figure out what's happening here
            if (ModuleId > 0)
            {
                if (MainSettings.NeedsConversion)
                {
                    if (!(string.IsNullOrEmpty(UserForumsList)))
                    {
                        var move = new Helpers.SettingConversion();
                        move.MoveSettings(ForumModuleId, ModuleId);

                    }
                }
            }

            var p = new List<string>();

            var viewType = Request.Params[ParamKeys.ViewType];

            // Topic View
            if (TopicId > 0 && (viewType == Views.Topic))
            {
                p.Add(ParamKeys.TopicId + "=" + TopicId);

                var firstNewPost = Request.Params[ParamKeys.FirstNewPost];
                if(!string.IsNullOrWhiteSpace(firstNewPost))
                    p.Add(ParamKeys.FirstNewPost + "=" + firstNewPost);

                var contentJumpId = Request.Params[ParamKeys.ContentJumpId];
                if (!string.IsNullOrWhiteSpace(contentJumpId))
                    p.Add(ParamKeys.ContentJumpId + "=" + contentJumpId);

                var pageId = Request.Params[ParamKeys.PageId];
                if (!string.IsNullOrWhiteSpace(pageId))
                {
                    int parsedPageId;
                    if(int.TryParse(pageId, out parsedPageId) && parsedPageId > 1)
                        p.Add(ParamKeys.PageId + "=" + pageId);
                }

                var pageJumpId = Request.Params[ParamKeys.PageJumpId];
                if (!string.IsNullOrWhiteSpace(pageJumpId))
                {
                    int parsedPageJumpId;
                    if (int.TryParse(pageJumpId, out parsedPageJumpId) && parsedPageJumpId > 1)
                        p.Add(ParamKeys.PageJumpId + "=" + pageJumpId);
                }

                var sort = Request.QueryString[ParamKeys.Sort];
                if(!string.IsNullOrWhiteSpace(sort) && Request.IsAuthenticated)
                {
                    sort = sort.ToUpperInvariant();
                    if(sort == "ASC" || sort == "DESC")
                        p.Add(ParamKeys.Sort + "=" + sort);
                }
            }

            // Topics View
            else if (ForumId > 0 && viewType == Views.Topics)
            {
                p.Add(ParamKeys.ForumId + "=" + ForumId);

                var pageId = Request.Params[ParamKeys.PageId];
                if (!string.IsNullOrWhiteSpace(pageId))
                {
                    int parsedPageId;
                    if (int.TryParse(pageId, out parsedPageId) && parsedPageId > 1)
                        p.Add(ParamKeys.PageId + "=" + pageId);
                }

                var pageJumpId = Request.Params[ParamKeys.PageJumpId];
                if (!string.IsNullOrWhiteSpace(pageJumpId))
                {
                    int parsedPageJumpId;
                    if (int.TryParse(pageJumpId, out parsedPageJumpId) && parsedPageJumpId > 1)
                        p.Add(ParamKeys.PageJumpId + "=" + pageJumpId);
                }
            }


            if (p.Count <= 0) 
                return;
            
            var sURL = Utilities.NavigateUrl(TabId, string.Empty, p.ToArray());
            if (string.IsNullOrEmpty(sURL))
                return;

            Response.Clear();
            Response.Status = "301 Moved Permanently";
            Response.AddHeader("Location", sURL);
            Response.End();
		}
    }
}


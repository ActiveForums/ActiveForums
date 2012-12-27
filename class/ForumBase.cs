//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
//ORIGINAL LINE: Imports System.Web.HttpContext

using System;
using System.Web;
using System.Xml;

namespace DotNetNuke.Modules.ActiveForums
{
    public class ForumBase : SettingsBase
    {

        private int _ForumId = -1;
        private string _ForumIds = string.Empty;
        private int _ForumGroupId = -1;
        private int _parentForumId = -1;
        private int _PostId;
        private int _TopicId = -1;
        private int _ReplyId;
        private int _QuoteId;
        private bool _bolJumpToLastPost = false;
        private string _defaultView = Views.ForumView;
        private int _defaultForumViewTemplateId = -1;
        private int _defaultTopicsViewTemplateId = -1;
        private int _defaultTopicViewTemplateId = -1;
        private string _templatePath = string.Empty;
        private string _templateFile = string.Empty;
        private Forum _foruminfo;
        private XmlDocument _forumData;


        public XmlDocument ForumData
        {
            get
            {
                var db = new Data.ForumsDB();
                if (ControlConfig != null)
                {
                    return db.ForumListXML(ControlConfig.SiteId, ControlConfig.InstanceId);
                }
                return db.ForumListXML(PortalId, ModuleId);
            }
            set
            {
                _forumData = value;
            }

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
                return _ForumIds;
            }
            set
            {
                _ForumIds = value;
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

        public bool bolJumpToLastPost
        {
            get
            {
                if (!Request.IsAuthenticated)
                {
                    return false;
                }
                if (Session["JumpToLastPost"] != null)
                {
                    return Convert.ToBoolean(Session["JumpToLastPost"]);
                }
                //TODO:Fix this
                //Dim objUserDetails As New UserDetailsController
                //Dim objUser As UserDetailsInfo = objUserDetails.ActiveForums_GetUserDetails(UserId, PortalId)
                //If Not objUser Is Nothing Then
                //    Return objUser.JumpLastPost
                //Else
                return false;
                //End If
            }
        }
        public DateTime UserLastAccess
        {
            get
            {
                if (Request.IsAuthenticated)
                {
                    if (Session[UserId.ToString() + ModuleId.ToString() + "LastAccess"] == null)
                    {
                        return Utilities.NullDate();
                    }
                    return Convert.ToDateTime(Session[UserId.ToString() + ModuleId.ToString() + "LastAccess"]);
                }
                return Utilities.NullDate();
            }
            set
            {
                Session[UserId.ToString() + ModuleId.ToString() + "LastAccess"] = value;
            }
        }
        public int PostId
        {
            get
            {
                int tempPostId = 0;
                if (tempPostId < 1)
                {
                    if (Request.QueryString[ParamKeys.PostId] != null)
                    {
                        if ((Request.Params[ParamKeys.PostId].IndexOf("#", 0) + 1) > 0)
                        {
                            _PostId = Convert.ToInt32(Request.Params[ParamKeys.PostId].Substring(0, (Request.Params[ParamKeys.PostId].IndexOf("#", 0) + 1) - 1));
                        }
                        else
                        {
                            _PostId = Convert.ToInt32(Request.QueryString[ParamKeys.PostId]);
                        }
                    }
                }
                return _PostId;
            }
        }
        public int TopicId
        {
            get
            {
                int tempTopicId = 0;
                if (tempTopicId < 1)
                {
                    if (Request.QueryString[ParamKeys.TopicId] != null)
                    {
                        if ((Request.Params[ParamKeys.TopicId].IndexOf("#", 0) + 1) > 0)
                        {
                            _TopicId = Convert.ToInt32(Request.Params[ParamKeys.TopicId].Substring(0, (Request.Params[ParamKeys.TopicId].IndexOf("#", 0) + 1) - 1));
                        }
                        else if (SimulateIsNumeric.IsNumeric(Request.QueryString[ParamKeys.TopicId]))
                        {
                            _TopicId = Convert.ToInt32(Request.QueryString[ParamKeys.TopicId]);
                        }
                    }
                    else if (Request.Params["postid"] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params["postid"]))
                        {
                            _TopicId = Convert.ToInt32(Request.Params["postid"]);
                        }

                    }
                }
                return _TopicId;
            }
            set
            {
                _TopicId = value;
            }
        }
        public int ReplyId
        {
            get
            {
                int tempReplyId = 0;
                if (tempReplyId < 1)
                {
                    if (Request.QueryString[ParamKeys.ReplyId] != null)
                    {
                        if ((Request.Params[ParamKeys.ReplyId].IndexOf("#", 0) + 1) > 0)
                        {
                            _ReplyId = Convert.ToInt32(Request.Params[ParamKeys.ReplyId].Substring(0, (Request.Params[ParamKeys.ReplyId].IndexOf("#", 0) + 1) - 1));
                        }
                        else
                        {
                            _ReplyId = Convert.ToInt32(Request.QueryString[ParamKeys.ReplyId]);
                        }
                    }
                }
                return _ReplyId;
            }
            set
            {
                _ReplyId = value;
            }
        }
        public int QuoteId
        {
            get
            {
                int tempQuoteId = 0;
                if (tempQuoteId < 1)
                {
                    if (Request.QueryString[ParamKeys.QuoteId] != null)
                    {
                        if ((Request.Params[ParamKeys.QuoteId].IndexOf("#", 0) + 1) > 0)
                        {
                            _QuoteId = Convert.ToInt32(Request.Params[ParamKeys.QuoteId].Substring(0, (Request.Params[ParamKeys.QuoteId].IndexOf("#", 0) + 1) - 1));
                        }
                        else
                        {
                            _QuoteId = Convert.ToInt32(Request.QueryString[ParamKeys.QuoteId]);
                        }
                    }
                }
                return _QuoteId;
            }
            set
            {
                _QuoteId = value;
            }
        }
        public int ForumId
        {
            get
            {
                if (_ForumId < 1)
                {
                    if (Request.Params[ParamKeys.ForumId] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.ForumId]))
                        {
                            _ForumId = Convert.ToInt32(Request.Params[ParamKeys.ForumId]);
                        }

                    }
                    else if (Request.Params["forumid"] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params["forumid"]))
                        {
                            _ForumId = Convert.ToInt32(Request.Params["forumid"]);
                        }
                    }
                }
                if (_ForumId < 1 & TopicId > 0)
                {
                    if (HttpContext.Current.Items["AFID" + TopicId.ToString()] == null)
                    {
                        var fdb = new Data.ForumsDB();
                        _ForumId = fdb.Forum_GetByTopicId(TopicId);
                        if (_ForumId > 0)
                        {
                            HttpContext.Current.Items.Add("AFID" + TopicId.ToString(), _ForumId);
                        }
                    }
                    else
                    {
                        _ForumId = Convert.ToInt32(HttpContext.Current.Items["AFID" + TopicId.ToString()]);
                    }
                }
                return _ForumId;
            }
            set
            {
                _ForumId = value;
            }
        }
        public int ForumGroupId
        {
            get
            {
                if (_ForumGroupId < 1)
                {
                    if (Request.Params[ParamKeys.GroupId] != null)
                    {
                        _ForumGroupId = Convert.ToInt32(Request.Params[ParamKeys.GroupId]);
                    }
                }
                if (_ForumGroupId < 1 & ForumId > 0 && ForumInfo != null)
                {
                    _ForumGroupId = ForumInfo.ForumGroupId;
                }
                return _ForumGroupId;
            }
            set
            {
                _ForumGroupId = value;
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
                var fc = new ForumController();
                _foruminfo = fc.Forums_Get(PortalId, ForumModuleId, ForumId, UserId, true, true, TopicId);
                return _foruminfo;
            }
            set
            {
                _foruminfo = value;
            }
        }
        public int SocialGroupId { get; set; }

        protected string GetSharedResource(string key)
        {
            return Services.Localization.Localization.GetString(key, "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx");
        }
        internal bool isHTMLPermitted(HTMLPermittedUsers permittedMode, bool UserIsTrusted, bool UserIsModerator)
        {
            if (permittedMode == HTMLPermittedUsers.AllUsers)
            {
                return true;
            }
            if (permittedMode == HTMLPermittedUsers.AuthenticatedUsers && Request.IsAuthenticated)
            {
                return true;
            }
            if (permittedMode == HTMLPermittedUsers.TrustedUsers && UserIsTrusted)
            {
                return true;
            }
            if (permittedMode == HTMLPermittedUsers.Moderators && UserIsModerator)
            {
                return true;
            }
            if (permittedMode == HTMLPermittedUsers.Administrators && HasModulePermission("EDIT"))
            {
                return true;
            }
            return false;
        }

        public bool CanRead
        {
            get
            {
                return SecurityCheck("read");
            }
        }
        public bool CanView
        {
            get
            {
                return SecurityCheck("view");
            }
        }
        public bool CanCreate
        {
            get
            {
                return SecurityCheck("create");
            }
        }
        public bool CanReply
        {
            get
            {
                return SecurityCheck("reply");
            }
        }
        //Public Sub New()
        //    Me.LicensePath = "~/DesktopModules/ActiveForums"
        //    Me.ProductKey = "AFKey"
        //End Sub
        private bool SecurityCheck(string secType)
        {
            if (ForumUser == null)
            {
                return false;
            }
            //Logger.Log(secType)
            var xNode = ForumData.SelectSingleNode("//forums/forum[@forumid='" + ForumId + "']/security/" + secType);

            if (xNode == null)
            {
                return false;
            }
            string secRoles = xNode.InnerText;
            return Permissions.HasPerm(secRoles, ForumUser.UserRoles);
        }



        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (Request.QueryString["dnnprintmode"] != null)
            {
                return;
            }
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

            string sURL = string.Empty;
            string[] p = new string[0];
            if (TopicId > 0 & (Request.Params[ParamKeys.ViewType] == Views.Topic))
            {
                p = new[] { ParamKeys.TopicId + "=" + TopicId };
                if (Request.Params[ParamKeys.FirstNewPost] != null)
                {
                    p = Utilities.AddParams(ParamKeys.FirstNewPost + "=" + Request.Params[ParamKeys.FirstNewPost], p);
                }
                if (Request.Params[ParamKeys.ContentJumpId] != null)
                {
                    p = Utilities.AddParams(ParamKeys.ContentJumpId + "=" + Request.Params[ParamKeys.ContentJumpId], p);
                }
                if (Request.QueryString[ParamKeys.PageId] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.QueryString[ParamKeys.PageId]))
                    {
                        if (Convert.ToInt32(Request.QueryString[ParamKeys.PageId]) > 1)
                        {
                            p = Utilities.AddParams(ParamKeys.PageId + "=" + Request.QueryString[ParamKeys.PageId], p);
                        }
                    }
                }
                if (Request.QueryString[ParamKeys.PageJumpId] != null & Request.IsAuthenticated)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.QueryString[ParamKeys.PageJumpId]))
                    {
                        if (Convert.ToInt32(Request.QueryString[ParamKeys.PageJumpId]) > 1)
                        {
                            p = Utilities.AddParams(ParamKeys.PageId + "=" + Request.QueryString[ParamKeys.PageJumpId], p);
                        }
                    }
                }
                if (Request.QueryString[ParamKeys.Sort] != null & Request.IsAuthenticated)
                {
                    string sSort = Request.QueryString[ParamKeys.Sort].ToUpperInvariant();
                    if (sSort == "ASC" || sSort == "DESC")
                    {
                        p = Utilities.AddParams(ParamKeys.Sort + "=" + sSort, p);
                    }
                }
            }
            else if (ForumId > 0 && Request.Params[ParamKeys.ViewType] == Views.Topics)
            {
                p = new[] { ParamKeys.ForumId + "=" + ForumId };
                if (Request.QueryString[ParamKeys.PageId] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.QueryString[ParamKeys.PageId]))
                    {
                        if (Convert.ToInt32(Request.QueryString[ParamKeys.PageId]) > 1)
                        {
                            p = Utilities.AddParams(ParamKeys.PageId + "=" + Request.QueryString[ParamKeys.PageId], p);
                        }
                    }
                }
                if (Request.QueryString[ParamKeys.PageJumpId] != null & Request.IsAuthenticated)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.QueryString[ParamKeys.PageJumpId]))
                    {
                        if (Convert.ToInt32(Request.QueryString[ParamKeys.PageJumpId]) > 1)
                        {
                            p = Utilities.AddParams(ParamKeys.PageJumpId + "=" + Request.QueryString[ParamKeys.PageJumpId], p);
                        }
                    }
                }
            }
            if (p.Length > 0)
            {
                sURL = Utilities.NavigateUrl(TabId, "", p);
            }
            if (!(string.IsNullOrEmpty(sURL)))
            {
                Response.Clear();
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", sURL);
                Response.End();
            }
        }
    }
}


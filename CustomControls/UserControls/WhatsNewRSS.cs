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
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:WhatsNewRSS runat=server></{0}:WhatsNewRSS>")]
    public class WhatsNewRSS : Control
    {

        #region Constants

        private const string ModuleIDRequestKey = "moduleid";
        private const string PortalIDRequestKey = "portalid";
        private const string TabIDRequestKey = "tabid";

        private const int DefaultModuleID = -1;
        private const int DefaultPortalID = 0;
        private const int DefaultTabID = -1;

        private const string CacheKeyTemplate = "aftprss_{0}_{1}";

        private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        private const string RSSHeader = "<rss version=\"2.0\" xmlns:atom=\"http://www.w3.org/2005/Atom\" xmlns:cf=\"http://www.microsoft.com/schemas/rss/core/2005\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:slash=\"http://purl.org/rss/1.0/modules/slash/\">";

        #endregion

        #region Private Variables

        private int? _requestTabID;
        private int? _requestModuleID;
        private int? _requestPortalID;
        private WhatsNewModuleSettings _settings;
        private string _authorizedForums;
        private User _currentUser;
        private string _cacheKey;

        #endregion

        #region Properties

        private int RequestTabID
        {
            get
            {
                if (!_requestTabID.HasValue)
                {
                    int parsedTabId;
                    _requestTabID = int.TryParse(HttpContext.Current.Request.QueryString[TabIDRequestKey], out parsedTabId) ? parsedTabId : DefaultTabID;
                }

                return _requestTabID.Value;
            }
        }
        private int RequestModuleID
        {
            get
            {
                if (!_requestModuleID.HasValue)
                {
                    int parsedModuleID;
                    _requestModuleID = int.TryParse(HttpContext.Current.Request.QueryString[ModuleIDRequestKey], out parsedModuleID) ? parsedModuleID : DefaultModuleID;
                }

                return _requestModuleID.Value;
            }
        }
        private int RequestPortalID
        {
            get
            {
                if (!_requestPortalID.HasValue)
                {
                    int parsedPortalID;
                    _requestPortalID = int.TryParse(HttpContext.Current.Request.QueryString[PortalIDRequestKey], out parsedPortalID) ? parsedPortalID : DefaultPortalID;
                }

                return _requestPortalID.Value;
            }
        }

        private WhatsNewModuleSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    var settingsCacheKey = "aftp_" + RequestModuleID;

                    var moduleSettings = DataCache.CacheRetrieve(settingsCacheKey) as Hashtable;
                    if (moduleSettings == null)
                    {
                        moduleSettings = new ModuleController().GetModuleSettings(RequestModuleID);
                        DataCache.CacheStore(settingsCacheKey, moduleSettings);
                    }

                    _settings = WhatsNewModuleSettings.CreateFromModuleSettings(moduleSettings);
                }
                return _settings;
            }
        }

        private User CurrentUser
        {
            get { return _currentUser ?? (_currentUser = new UserController().GetUser(RequestPortalID, -1)); }
        }

        private string AuthorizedForums
        {
            get
            {
                return _authorizedForums ??
                       (_authorizedForums =
                        new Data.Common().CheckForumIdsForView(Settings.Forums, CurrentUser.UserRoles));
            }
        }

        private string CacheKey
        {
            get
            {
                return _cacheKey ??
                       (_cacheKey =
                        string.Format(CacheKeyTemplate, RequestModuleID,
                                      Settings.RSSIgnoreSecurity ? Settings.Forums : AuthorizedForums));
            }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            HttpContext.Current.Response.ContentType = "text/xml";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (RequestTabID == DefaultTabID || RequestModuleID == DefaultModuleID || !Settings.RSSEnabled)
            {
                return;
            }

            // Attempt to load from cache if it's enabled
            var rss = (Settings.RSSCacheTimeout > 0) ? DataCache.CacheRetrieve(CacheKey) as string : null;

            // Build the RSS if needed
            rss = rss ?? BuildRSS();

            // Save the rss to cache if it's enabled
            if (Settings.RSSCacheTimeout > 0)
                DataCache.CacheStore(CacheKey, rss);

            // Render the output
            writer.Write(rss);
            base.Render(writer);
        }

        #region XML Helpers

        private static string WriteElement(string element, int indent)
        {
            var inputLength = element.Trim().Length + 20;
            var sb = new StringBuilder(inputLength);
            sb.Append(System.Environment.NewLine.PadRight(indent + 2, '\t'));
            sb.Append("<").Append(element).Append(">");
            return sb.ToString();
        }

        private static string WriteElement(string element, string elementValue, int indent)
        {
            var inputLength = element.Trim().Length + elementValue.Trim().Length + 20;
            var sb = new StringBuilder(inputLength);
            sb.Append(System.Environment.NewLine.PadRight(indent + 2, '\t'));
            sb.Append("<").Append(element).Append(">");
            sb.Append(CleanXmlString(elementValue));
            sb.Append("</").Append(element).Append(">");
            return sb.ToString();
        }

        private static string CleanXmlString(string xmlString)
        {
            xmlString = HttpContext.Current.Server.HtmlEncode(xmlString);
            return xmlString;
        }

        #endregion

        private string BuildRSS()
        {
            const int indent = 2;

            var counter = 0;

            var sb = new StringBuilder(1024);

            // build header
            sb.Append(XmlHeader + System.Environment.NewLine);
            sb.Append(RSSHeader + System.Environment.NewLine);

            // build channel
            var pc = new PortalController();
            var ps = PortalController.GetCurrentPortalSettings();

            var offSet = Convert.ToInt32(PortalSettings.Current.TimeZone.BaseUtcOffset.TotalMinutes);

            sb.Append(WriteElement("channel", indent));
            sb.Append(WriteElement("title", ps.PortalName, indent));
            sb.Append(WriteElement("link", "http://" + HttpContext.Current.Request.Url.Host, indent));
            sb.Append(WriteElement("description", ps.PortalName, indent));
            sb.Append(WriteElement("generator", "ActiveForums  5.0", indent));
            sb.Append(WriteElement("language", ps.DefaultLanguage, indent));

            if (ps.LogoFile != string.Empty)
            {
                var sLogo = "<image><url>http://" + HttpContext.Current.Request.Url.Host + ps.HomeDirectory + ps.LogoFile + "</url>";
                sLogo += "<title>" + ps.PortalName + "</title>";
                sLogo += "<link>http://" + HttpContext.Current.Request.Url.Host + "</link></image>";
                sb.Append(sLogo);
            }

            sb.Append(WriteElement("copyright", ps.FooterText.Replace("[year]",DateTime.Now.Year.ToString()), 2));
            sb.Append(WriteElement("lastBuildDate", "[LASTBUILDDATE]", 2));

            var lastBuildDate = DateTime.MinValue;

            // build items

            var forumids = Settings.RSSIgnoreSecurity ? Settings.Forums : AuthorizedForums;

            var useFriendly = Utilities.IsRewriteLoaded();
            var dr = DataProvider.Instance().GetPosts(RequestPortalID, forumids, true, false, Settings.Rows, Settings.Tags);
            var sHost = Utilities.GetHost();

            try
            {
                while (dr.Read())
                {
                    var groupName = Convert.ToString(dr["GroupName"]);
                    var groupId = Convert.ToInt32(dr["ForumGroupId"]);
                    var topicTabId = Convert.ToInt32(dr["TabId"]);
                    var topicModuleId = Convert.ToInt32(dr["ModuleId"]);
                    var forumName = Convert.ToString(dr["ForumName"]);
                    var forumId = Convert.ToInt32(dr["ForumId"]);
                    var subject = Convert.ToString(dr["Subject"]);
                    var userName = Convert.ToString(dr["AuthorUserName"]);
                    var postDate = Convert.ToString(dr["DateCreated"]);
                    var body = Utilities.StripHTMLTag(Convert.ToString(dr["Body"]));
                    var bodyHtml = Convert.ToString(dr["Body"]);
                    var displayName = Convert.ToString(dr["AuthorDisplayName"]);
                    var replyCount = Convert.ToString(dr["ReplyCount"]);
                    var lastPostDate = string.Empty;
                    var topicId = Convert.ToInt32(dr["TopicId"]);
                    var replyId = Convert.ToInt32(dr["ReplyId"]);
                    var firstName = Convert.ToString(dr["AuthorFirstName"]);
                    var lastName = Convert.ToString(dr["AuthorLastName"]);
                    var authorId = Convert.ToInt32(dr["AuthorId"]);
                    var sForumUrl = Convert.ToString(dr["PrefixURL"]);
                    var sTopicUrl = Convert.ToString(dr["TopicURL"]);
                    var sGroupPrefixUrl = Convert.ToString(dr["GroupPrefixURL"]);

                    var dateCreated = Convert.ToDateTime(dr["DateCreated"]).AddMinutes(offSet);

                    if (lastBuildDate == DateTime.MinValue || dateCreated > lastBuildDate)
                    {
                        lastBuildDate = dateCreated;
                    }

                    var ts = DataCache.MainSettings(topicModuleId);

                    string url;
                    if (string.IsNullOrEmpty(sTopicUrl) || !useFriendly)
                    {
                        string[] Params = { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + forumId, ParamKeys.TopicId + "=" + topicId };
                        url = Common.Globals.NavigateURL(topicTabId, "", Params);
                        if (url.IndexOf(HttpContext.Current.Request.Url.Host, StringComparison.CurrentCulture) == -1)
                        {
                            url = Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + url;
                        }
                    }
                    else
                    {
                        var ctlUtils = new ControlUtils();
                        sTopicUrl = ctlUtils.BuildUrl(topicTabId, topicModuleId, sGroupPrefixUrl, sForumUrl, groupId, forumId, topicId, sTopicUrl, -1, -1, string.Empty, 1, replyId, -1);
                        if (sHost.EndsWith("/") && sTopicUrl.StartsWith("/"))
                        {
                            sHost = sHost.Substring(0, sHost.Length - 1);
                        }
                        url = sHost + sTopicUrl;
                    }

                    sb.Append(WriteElement("item", indent));

                    sb.Append(WriteElement("title", subject, indent + 1));
                    if (Settings.RSSIncludeBody)
                    {
                        if (bodyHtml.IndexOf("<body>", StringComparison.CurrentCulture) > 0)
                        {
                            bodyHtml = TemplateUtils.GetTemplateSection(bodyHtml, "<body>", "</body>");
                        }
                        // Legacy Attachment functionality uses "attachid"
                        if (bodyHtml.Contains("&#91;IMAGE:"))
                        {
                            var strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
                            const string pattern = "(&#91;IMAGE:(.+?)&#93;)";
                            var regExp = new Regex(pattern);
                            var matches = regExp.Matches(bodyHtml);
                            foreach (Match match in matches)
                            {
                                var sImage = "<img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + RequestPortalID + "&moduleid=" + RequestModuleID + "&attachid=" + match.Groups[2].Value + "\" border=\"0\" />";
                                bodyHtml = bodyHtml.Replace(match.Value, sImage);
                            }
                        }
                        // Legacy Attachment functionality uses "attachid"
                        if (bodyHtml.Contains("&#91;THUMBNAIL:"))
                        {
                            var strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
                            const string pattern = "(&#91;THUMBNAIL:(.+?)&#93;)";
                            var regExp = new Regex(pattern);
                            var matches = regExp.Matches(bodyHtml);
                            foreach (Match match in matches)
                            {
                                var thumbId = match.Groups[2].Value.Split(':')[0];
                                var parentId = match.Groups[2].Value.Split(':')[1];
                                var sImage = "<a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + RequestPortalID + "&moduleid=" + RequestModuleID + "&attachid=" + parentId + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + RequestPortalID + "&moduleid=" + RequestModuleID + "&attachid=" + thumbId + "\" border=\"0\" /></a>";
                                bodyHtml = bodyHtml.Replace(match.Value, sImage);
                            }
                        }
                        bodyHtml = bodyHtml.Replace("src=\"/Portals", "src=\"" + Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + "/Portals");
                        bodyHtml = Utilities.ManageImagePath(bodyHtml, Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host));

                        sb.Append(WriteElement("description", bodyHtml, indent + 1));
                    }
                    else
                    {
                        sb.Append(WriteElement("description", string.Empty, indent + 1));
                    }
                    sb.Append(WriteElement("link", url, indent + 1));
                    sb.Append(WriteElement("comments", url, indent + 1));
                    sb.Append(WriteElement("pubDate", Convert.ToDateTime(postDate).AddMinutes(offSet).ToString("r"), indent + 1));
                    sb.Append(WriteElement("dc:creator", displayName, indent + 1));
                    sb.Append(WriteElement("guid", url, indent + 1));
                    sb.Append(WriteElement("slash:comments", replyCount, indent + 1));
                    sb.Append(WriteElement("/item", indent));



                }
                dr.Close();
                sb.Append("<atom:link href=\"http://" + HttpContext.Current.Request.Url.Host + HttpUtility.HtmlEncode(HttpContext.Current.Request.RawUrl) + "\" rel=\"self\" type=\"application/rss+xml\" />");
                sb.Append(WriteElement("/channel", 1));
                sb.Append("</rss>");
                sb.Replace("[LASTBUILDDATE]", lastBuildDate.ToString("r"));
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }


        }
    }

}

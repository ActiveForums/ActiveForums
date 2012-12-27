using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:WhatsNewRSS runat=server></{0}:WhatsNewRSS>")]
    public class WhatsNewRSS : Control
    {

        private double dblCacheTimeOut;
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            int intTabId = -1;
            int intModuleId = -1;
            int intPortalId = 0;
            //Response.Write(intPortalId)
            if (HttpContext.Current.Request.QueryString["tabid"] != null)
            {
                if (SimulateIsNumeric.IsNumeric(HttpContext.Current.Request.QueryString["tabid"]))
                {
                    intTabId = Convert.ToInt32(HttpContext.Current.Request.QueryString["tabid"]);
                }
            }
            if (HttpContext.Current.Request.QueryString["moduleid"] != null)
            {
                if (SimulateIsNumeric.IsNumeric(HttpContext.Current.Request.QueryString["moduleid"]))
                {
                    intModuleId = Convert.ToInt32(HttpContext.Current.Request.QueryString["moduleid"]);
                }
            }
            if (HttpContext.Current.Request.QueryString["portalid"] != null)
            {
                if (SimulateIsNumeric.IsNumeric(HttpContext.Current.Request.QueryString["portalid"]))
                {
                    intPortalId = Convert.ToInt32(HttpContext.Current.Request.QueryString["portalid"]);
                }
            }
            if (intTabId == -1 || intModuleId == -1)
            {
                return;
            }
            int intPosts = 0;
            DotNetNuke.Entities.Modules.ModuleController mc = new DotNetNuke.Entities.Modules.ModuleController();
            Hashtable settings = mc.GetModuleSettings(intModuleId);
            if (!(Convert.ToString(settings["AFTopPostsNumber"]) == null))
            {
                intPosts = Convert.ToInt32(settings["AFTopPostsNumber"]);
            }
            else
            {
                intPosts = 5;
            }
            string sForums = string.Empty;
            if (!(Convert.ToString(settings["AFTopPostsForums"]) == null))
            {
                sForums = Convert.ToString(settings["AFTopPostsForums"]);
            }
            bool bolSecurity = false;
            bool bolBody = false;
            if (!(Convert.ToString(settings["AFTopPostsSecurity"]) == null))
            {
                bolSecurity = Convert.ToBoolean(settings["AFTopPostsSecurity"]);
            }
            if (!(Convert.ToString(settings["AFTopPostsBody"]) == null))
            {
                bolBody = Convert.ToBoolean(settings["AFTopPostsBody"]);
            }
            if (!(Convert.ToString(settings["AFTopPostsCache"]) == null))
            {
                dblCacheTimeOut = Convert.ToInt32(settings["AFTopPostsCache"]);
            }
            else
            {
                dblCacheTimeOut = 30;
            }
            //Response.Write(BuildRSS(intPortalId, intTabId, intModuleId, intPosts, sForums, bolSecurity, bolBody))
            string sOut = string.Empty;
            if (!(Convert.ToString(settings["AFTopPostsRSS"]) == null))
            {
                if (Convert.ToBoolean(settings["AFTopPostsRSS"]) == true)
                {
                    object obj = DataCache.CacheRetrieve("aftprss" + intModuleId);
                    if (obj != null)
                    {
                        sOut = Convert.ToString(obj);
                    }
                    else
                    {
                        sOut = BuildRSS(intModuleId, intPortalId);
                    }
                }

            }


            writer.Write(sOut);
            base.Render(writer);
        }
        //Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)

        //End Sub
        private string WriteElement(string Element, int Indent)
        {
            int InputLength = Element.Trim().Length + 20;
            StringBuilder sb = new StringBuilder(InputLength);
            sb.Append(System.Environment.NewLine.PadRight(Indent + 2, '\t'));
            sb.Append("<").Append(Element).Append(">");
            return sb.ToString();
        }

        private string WriteElement(string Element, string ElementValue, int Indent)
        {
            int InputLength = Element.Trim().Length + ElementValue.Trim().Length + 20;
            StringBuilder sb = new StringBuilder(InputLength);
            sb.Append(System.Environment.NewLine.PadRight(Indent + 2, '\t'));
            sb.Append("<").Append(Element).Append(">");
            sb.Append(CleanXmlString(ElementValue));
            sb.Append("</").Append(Element).Append(">");
            return sb.ToString();
        }

        private string CleanXmlString(string XmlString)
        {
            XmlString = HttpContext.Current.Server.HtmlEncode(XmlString);
            return XmlString;
        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            //'Put user code to initialize the page here
            HttpContext.Current.Response.ContentType = "text/xml";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;


            //HttpContext.Current.Response.Write(sOut)

        }
        private string BuildRSS(int ModuleId, int PortalId)
        {

            Hashtable settings = DotNetNuke.Entities.Portals.PortalSettings.GetModuleSettings(ModuleId);
            string forumids = string.Empty;
            bool bolTopicsOnly = false;
            bool bolRandom = false;
            int Rows = 10;
            bool IgnoreSecurity = false;
            bool IncludeBody = false;
            bool bRSS = false;
            string tags = string.Empty;
            if (!(Convert.ToString(settings["AFTopPostsTopicsOnly"]) == null))
            {
                bolTopicsOnly = Convert.ToBoolean(settings["AFTopPostsTopicsOnly"]);
            }
            if (!(Convert.ToString(settings["AFTopPostsRandomOrder"]) == null))
            {
                bolRandom = Convert.ToBoolean(settings["AFTopPostsRandomOrder"]);
            }
            if (!(Convert.ToString(settings["AFTopPostsForums"]) == null))
            {
                forumids = Convert.ToString(settings["AFTopPostsForums"]);
            }
            if (!(Convert.ToString(settings["AFTopPostsNumber"]) == null))
            {
                Rows = Convert.ToInt32(settings["AFTopPostsNumber"]);
            }
            if (!(Convert.ToString(settings["AFTopPostsRSS"]) == null))
            {
                bRSS = Convert.ToBoolean(settings["AFTopPostsRSS"]);
            }

            if (!(Convert.ToString(settings["AFTopPostsSecurity"]) == null))
            {
                IgnoreSecurity = Convert.ToBoolean(settings["AFTopPostsSecurity"]);
            }
            if (!(Convert.ToString(settings["AFTopPostsBody"]) == null))
            {
                IncludeBody = Convert.ToBoolean(settings["AFTopPostsBody"]);
            }
            if (!(Convert.ToString(settings["AFTopPostsTags"]) == null))
            {
                tags = Convert.ToString(settings["AFTopPostsTags"]);
            }
            int counter = 0;
            StringBuilder sb = new StringBuilder(1024);
            // build header
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + System.Environment.NewLine);
            sb.Append("<rss version=\"2.0\" xmlns:atom=\"http://www.w3.org/2005/Atom\" xmlns:cf=\"http://www.microsoft.com/schemas/rss/core/2005\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:slash=\"http://purl.org/rss/1.0/modules/slash/\">" + System.Environment.NewLine);



            // build channel
            DotNetNuke.Entities.Portals.PortalController pc = new DotNetNuke.Entities.Portals.PortalController();
            DotNetNuke.Entities.Portals.PortalSettings ps = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
            int offSet = ps.TimeZoneOffset;
            sb.Append(WriteElement("channel", 1));
            sb.Append(WriteElement("title", ps.PortalName, 2));
            sb.Append(WriteElement("link", "http://" + HttpContext.Current.Request.Url.Host, 2));
            sb.Append(WriteElement("description", ps.PortalName, 2));
            sb.Append(WriteElement("generator", "ActiveForums  4.1", 2));
            sb.Append(WriteElement("language", ps.DefaultLanguage, 2));
            //sb.Append(WriteElement("copyright", ps.FooterText, 2))
            if (!(ps.LogoFile == string.Empty))
            {
                string sLogo = "<image><url>http://" + HttpContext.Current.Request.Url.Host + ps.HomeDirectory + ps.LogoFile + "</url>";
                sLogo += "<title>" + ps.PortalName + "</title>";
                sLogo += "<link>http://" + HttpContext.Current.Request.Url.Host + "</link></image>";
                sb.Append(sLogo);
            }
            sb.Append(WriteElement("copyright", ps.FooterText, 2));
            //sb.Append(WriteElement("webMaster", ps.Email, 2))
            sb.Append(WriteElement("lastBuildDate", "[LASTBUILDDATE]", 2));

            DateTime LastBuildDate = DateTime.MinValue;
            // build items
            DotNetNuke.Entities.Users.UserInfo ui = null;
            ui = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
            bool useFriendly = Utilities.IsRewriteLoaded();
            IDataReader dr = DataProvider.Instance().GetPosts(PortalId, forumids, true, false, Rows, ui.UserID, IgnoreSecurity, false, tags);
            string sHost = Utilities.GetHost();
            try
            {
                while (dr.Read())
                {
                    int Indent = 2;
                    string GroupName = string.Empty;
                    string GroupId = string.Empty;
                    string TopicTabId = string.Empty;
                    string TopicModuleId = string.Empty;
                    string ForumName = string.Empty;
                    string ForumId = string.Empty;
                    string Subject = string.Empty;
                    string UserName = string.Empty;
                    string PostDate = string.Empty;
                    string Body = string.Empty;
                    string BodyHTML = string.Empty;
                    string DisplayName = string.Empty;
                    string ReplyCount = string.Empty;
                    string LastPostDate = string.Empty;
                    string TopicId = string.Empty;
                    string ReplyId = string.Empty;
                    string FirstName = string.Empty;
                    string LastName = string.Empty;
                    string AuthorId = string.Empty;


                    string sForumUrl = string.Empty;
                    string sTopicURL = string.Empty;
                    string sGroupPrefixURL = string.Empty;

                    GroupName = Convert.ToString(dr["GroupName"]);
                    GroupId = Convert.ToString(dr["ForumGroupId"]);
                    TopicTabId = Convert.ToString(dr["TabId"]);
                    TopicModuleId = Convert.ToString(dr["ModuleId"]);
                    ForumName = Convert.ToString(dr["ForumName"]);
                    ForumId = Convert.ToString(dr["ForumId"]);
                    Subject = Convert.ToString(dr["Subject"]);
                    UserName = Convert.ToString(dr["AuthorUserName"]);
                    FirstName = Convert.ToString(dr["AuthorFirstName"]);
                    LastName = Convert.ToString(dr["AuthorLastName"]);
                    AuthorId = Convert.ToString(dr["AuthorId"]);
                    DisplayName = Convert.ToString(dr["AuthorDisplayName"]);
                    PostDate = Convert.ToString(dr["DateCreated"]);
                    Body = Utilities.StripHTMLTag(Convert.ToString(dr["Body"]));
                    TopicId = Convert.ToString(dr["TopicId"]);
                    ReplyId = Convert.ToString(dr["ReplyId"]);
                    BodyHTML = Convert.ToString(dr["Body"]);
                    ReplyCount = Convert.ToString(dr["ReplyCount"]);
                    //  topicUrl = dr("URL").ToString
                    sForumUrl = dr["PrefixURL"].ToString();
                    sTopicURL = dr["TopicURL"].ToString();
                    sGroupPrefixURL = dr["GroupPrefixURL"].ToString();
                    if (LastBuildDate == new DateTime())
                    {
                        LastBuildDate = Convert.ToDateTime(dr["DateCreated"]).AddMinutes(offSet);
                    }
                    else
                    {
                        if (Convert.ToDateTime(dr["DateCreated"]).AddMinutes(offSet) > LastBuildDate)
                        {
                            LastBuildDate = Convert.ToDateTime(dr["DateCreated"]).AddMinutes(offSet);
                        }
                    }
                    SettingsInfo ts = DataCache.MainSettings(Convert.ToInt32(TopicModuleId));

                    //UserProfiles.GetDisplayName(CInt(AuthorId), ts.UserNameDisplay, False, UserName, FirstName, LastName, DisplayName)
                    string URL = string.Empty;
                    if (string.IsNullOrEmpty(sTopicURL) || !useFriendly)
                    {
                        string[] Params = { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId };
                        URL = DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(TopicTabId), "", Params);
                        if (URL.IndexOf(HttpContext.Current.Request.Url.Host) == -1)
                        {
                            URL = DotNetNuke.Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + URL;
                        }
                    }
                    else
                    {
                        ControlUtils ctlUtils = new ControlUtils();
                        sTopicURL = ctlUtils.BuildUrl(Convert.ToInt32(TopicTabId), Convert.ToInt32(TopicModuleId), sGroupPrefixURL, sForumUrl, Convert.ToInt32(GroupId), Convert.ToInt32(ForumId), Convert.ToInt32(TopicId), sTopicURL, -1, -1, string.Empty, 1, -1);
                        if (sHost.EndsWith("/") && sTopicURL.StartsWith("/"))
                        {
                            sHost = sHost.Substring(0, sHost.Length - 1);
                        }
                        URL = sHost + sTopicURL;
                    }

                    //URL = URL.Replace("&", "&amp;")
                    sb.Append(WriteElement("item", Indent));

                    sb.Append(WriteElement("title", Subject, Indent + 1));
                    if (IncludeBody == true)
                    {
                        if (BodyHTML.IndexOf("<body>") > 0)
                        {
                            BodyHTML = TemplateUtils.GetTemplateSection(BodyHTML, "<body>", "</body>");
                        }
                        if (BodyHTML.Contains("&#91;IMAGE:"))
                        {
                            string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
                            string pattern = "(&#91;IMAGE:(.+?)&#93;)";
                            Regex regExp = new Regex(pattern);
                            MatchCollection matches = null;
                            matches = regExp.Matches(BodyHTML);
                            foreach (Match match in matches)
                            {
                                string sImage = "";
                                sImage = "<img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + match.Groups[2].Value + "\" border=\"0\" />";
                                BodyHTML = BodyHTML.Replace(match.Value, sImage);
                            }
                        }
                        if (BodyHTML.Contains("&#91;THUMBNAIL:"))
                        {
                            string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
                            string pattern = "(&#91;THUMBNAIL:(.+?)&#93;)";
                            Regex regExp = new Regex(pattern);
                            MatchCollection matches = null;
                            matches = regExp.Matches(BodyHTML);
                            foreach (Match match in matches)
                            {
                                string sImage = "";
                                string thumbId = match.Groups[2].Value.Split(':')[0];
                                string parentId = match.Groups[2].Value.Split(':')[1];
                                sImage = "<a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + parentId + "\" target=\"_blank\"><img src=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + thumbId + "\" border=\"0\" /></a>";
                                BodyHTML = BodyHTML.Replace(match.Value, sImage);
                            }
                        }
                        BodyHTML = BodyHTML.Replace("src=\"/Portals", "src=\"" + DotNetNuke.Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + "/Portals");
                        BodyHTML = Utilities.ManageImagePath(BodyHTML, DotNetNuke.Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host));

                        sb.Append(WriteElement("description", BodyHTML, Indent + 1));
                    }
                    else
                    {
                        sb.Append(WriteElement("description", string.Empty, Indent + 1));
                    }
                    sb.Append(WriteElement("link", URL, Indent + 1));
                    sb.Append(WriteElement("comments", URL, Indent + 1));
                    sb.Append(WriteElement("pubDate", Convert.ToDateTime(PostDate).AddMinutes(offSet).ToString("r"), Indent + 1));
                    sb.Append(WriteElement("dc:creator", DisplayName, Indent + 1));
                    sb.Append(WriteElement("guid", URL, Indent + 1));
                    sb.Append(WriteElement("slash:comments", ReplyCount, Indent + 1));
                    sb.Append(WriteElement("/item", Indent));



                }
                dr.Close();
                sb.Append("<atom:link href=\"http://" + HttpContext.Current.Request.Url.Host + HttpUtility.HtmlEncode(HttpContext.Current.Request.RawUrl) + "\" rel=\"self\" type=\"application/rss+xml\" />");
                sb.Append(WriteElement("/channel", 1));
                sb.Append("</rss>");
                sb.Replace("[LASTBUILDDATE]", LastBuildDate.ToString("r"));
                DataCache.CacheStore("aftprss" + ModuleId, sb.ToString(), DateTime.Now.AddMinutes(dblCacheTimeOut));
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }


        }
    }

}

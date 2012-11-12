//© 2004 - 2010 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
//ORIGINAL LINE: Imports System.Web.HttpContext

using System.Web;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Threading;

namespace DotNetNuke.Modules.ActiveForums
{
    public abstract partial class Utilities
    {
        internal static string AppPath
        {
            get
            {
                return "~/DesktopModules/ActiveForums/";
            }
        }
        /// <summary>
        /// Calculates a friendly display string based on an input timespan
        /// </summary>
        public static string HumanFriendlyDate(DateTime DisplayDate, int InstanceId, int TimeZoneOffset)
        {
            DateTime newDate = DateTime.Parse(GetDate(DisplayDate, InstanceId, TimeZoneOffset));
            var ts = new TimeSpan(DateTime.Now.Ticks - newDate.Ticks);
            double delta = ts.TotalSeconds;
            if (delta <= 1)
            {
                return GetSharedResource("[RESX:TimeSpan:SecondAgo]");
            }
            if (delta < 60)
            {
                return string.Format(GetSharedResource("[RESX:TimeSpan:SecondsAgo]"), ts.Seconds);
            }
            if (delta < 120)
            {
                return GetSharedResource("[RESX:TimeSpan:MinuteAgo]");
            }
            if (delta < (45 * 60))
            {
                return string.Format(GetSharedResource("[RESX:TimeSpan:MinutesAgo]"), ts.Minutes);
            }
            if (delta < (90 * 60))
            {
                return GetSharedResource("[RESX:TimeSpan:HourAgo]");
            }
            if (delta < (24 * 60 * 60))
            {
                return string.Format(GetSharedResource("[RESX:TimeSpan:HoursAgo]"), ts.Hours);
            }
            if (delta < (48 * 60 * 60))
            {
                return GetSharedResource("[RESX:TimeSpan:DayAgo]");
            }
            if (delta < (72 * 60 * 60))
            {
                return string.Format(GetSharedResource("[RESX:TimeSpan:DaysAgo]"), ts.Days);
            }
            if (delta < Convert.ToDouble(new TimeSpan(24 * 32, 0, 0).TotalSeconds))
            {
                return GetSharedResource("[RESX:TimeSpan:MonthAgo]");
            }
            if (delta < Convert.ToDouble(new TimeSpan(((24 * 30) * 11), 0, 0).TotalSeconds))
            {
                return string.Format(GetSharedResource("[RESX:TimeSpan:MonthsAgo]"), Math.Ceiling(ts.Days / 30.0).ToString());
            }
            if (delta < Convert.ToDouble(new TimeSpan(((24 * 30) * 18), 0, 0).TotalSeconds))
            {
                return GetSharedResource("[RESX:TimeSpan:YearAgo]");
            }
            return string.Format(GetSharedResource("[RESX:TimeSpan:YearsAgo]"), Math.Ceiling(ts.Days / 365.0).ToString());
            //Else
            //   Return ts.TotalDays.ToString
        }
        internal static string ParseTokenConfig(string Template, string group, ControlsConfig config)
        {
            if (string.IsNullOrEmpty(Template))
            {
                return string.Empty;
            }
            if (!(Template.Contains(Globals.ControlRegisterTag)))
            {
                Template = Globals.ControlRegisterTag + Template;
            }
            Template = ParseSpacer(Template);
            var li = new List<Token>();
            var tc = new TokensController();
            li = tc.TokensList(group);
            if (li != null)
            {
                foreach (Token tk in li)
                {
                    Template = Template.Replace(tk.TokenTag, tk.TokenReplace);
                }
            }
            Template = Template.Replace("[PARAMKEYS:GROUPID]", ParamKeys.GroupId);
            Template = Template.Replace("[PARAMKEYS:FORUMID]", ParamKeys.ForumId);
            Template = Template.Replace("[PARAMKEYS:TOPICID]", ParamKeys.TopicId);
            Template = Template.Replace("[PARAMKEYS:VIEWTYPE]", ParamKeys.ViewType);
            Template = Template.Replace("[PARAMKEYS:QUOTEID]", ParamKeys.QuoteId);
            Template = Template.Replace("[PARAMKEYS:REPLYID]", ParamKeys.ReplyId);
            Template = Template.Replace("[VIEWS:TOPICS]", Views.Topics);
            Template = Template.Replace("[VIEWS:TOPIC]", Views.Topic);
            Template = Template.Replace("[PAGEID]", config.PageId.ToString());
            Template = Template.Replace("[SITEID]", config.SiteId.ToString());
            Template = Template.Replace("[INSTANCEID]", config.InstanceId.ToString());
            return Template;
        }
        internal static string ParseSecurityTokens(string Template, string userRoles)
        {
            string sKey = "";
            string sReplace = "";
            string pattern = "(\\[AF:SECURITY:(.+?):(.+?)\\])(.|\\n)*?(\\[/AF:SECURITY:(.+?):(.+?)\\])";
            var regExp = new Regex(pattern);
            MatchCollection matches;
            matches = regExp.Matches(Template);
            foreach (Match match in matches)
            {
                string sRoles = match.Groups[3].Value;
                if (Permissions.HasAccess(sRoles, userRoles))
                {
                    Template = Template.Replace(match.Groups[1].Value, string.Empty);
                    Template = Template.Replace(match.Groups[5].Value, string.Empty);
                }
                else
                {
                    Template = Template.Replace(match.Value, string.Empty);
                    //Template = Template.Replace(match.Value.Replace("[AF:SECURITY:", "[/AF:SECURITY:"), String.Empty)
                }
            }
            return Template;
        }
        internal static string GetTemplate(string FilePath)
        {
            string sPath = FilePath;
            if (!(sPath.Contains("\\\\")) && !(sPath.Contains(":\\")))
            {
                sPath = HttpContext.Current.Server.MapPath(FilePath);
            }
            //If sPath.Contains("~/") Then

            //End If
            string sContents = string.Empty;
            StreamReader objStreamReader;
            if (File.Exists(sPath))
            {
                try
                {
                    objStreamReader = File.OpenText(sPath);
                    sContents = objStreamReader.ReadToEnd();
                    objStreamReader.Close();
                }
                catch (Exception exc)
                {
                    sContents = exc.Message;
                }
            }
            //sContents = "<%@ Register TagPrefix=""af"" Assembly=""Active.Modules.Forums"" Namespace=""DotNetNuke.Modules.ActiveForums.Controls""%>" & sContents
            //sContents = Utilities.ParseSpacer(sContents)
            return sContents;
        }
        internal static string ParseToolBar(string template, int TabId, int ModuleId, int UserId, CurrentUserTypes CurrentUserType)
        {
            SettingsInfo _mainSettings = DataCache.MainSettings(ModuleId);
            string sMemUrl = "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ViewType + "=members") + "\">[RESX:Members]</a>";
            string sProfileURL = string.Empty;
            string sSettingsURL = string.Empty;
            //If Current.Request.IsAuthenticated Then
            //    sProfileURL = "<a href=""" & NavigateUrl(TabId, "", ParamKeys.ViewType & "=profile&uid=" & UserId.ToString) & """>[RESX:MyProfile]</a>"

            //End If

            //If (_mainSettings.ProfileType = ProfileTypes.Advanced Or _mainSettings.ProfileType = ProfileTypes.Social) And _mainSettings.ProfileTabId > 0 Then
            //    If _mainSettings.ProfileType <> ProfileTypes.Social Then
            //        sMemUrl = "<a href=""" & NavigateUrl(_mainSettings.ProfileTabId) & """>[RESX:Members]</a>"
            //    End If
            //    If Not sProfileURL = String.Empty Then
            //        If _mainSettings.ProfileType = ProfileTypes.Social Then
            //            Dim Params() As String = {"asuid=" & UserId}
            //            sProfileURL = "<a href=""" & NavigateUrl(_mainSettings.ProfileTabId, "", Params) & """>[RESX:MyProfile]</a>"
            //            sSettingsURL = "<a href=""" & NavigateUrl(TabId, "", ParamKeys.ViewType & "=usersettings&uid=" & UserId.ToString) & """>[RESX:MySettings]</a>"
            //        Else
            //            Dim Params() As String = {"uid=" & UserId}
            //            sProfileURL = "<a href=""" & NavigateUrl(_mainSettings.ProfileTabId, "", Params) & """>[RESX:MyProfile]</a>"
            //            sSettingsURL = "<a href=""" & NavigateUrl(TabId, "", ParamKeys.ViewType & "=usersettings&uid=" & UserId.ToString) & """>[RESX:MySettings]</a>"
            //        End If

            //    End If
            //End If

            var ctlUtils = new ControlUtils();
            if (HttpContext.Current.Request.IsAuthenticated)
            {

                template = template.Replace("[AF:TB:NotRead]", "<a href=\"" + ctlUtils.BuildUrl(TabId, ModuleId, string.Empty, string.Empty, -1, -1, -1, -1, "notread", 1, -1) + "\">[RESX:NotRead]</a>");
                template = template.Replace("[AF:TB:MyTopics]", "<a href=\"" + ctlUtils.BuildUrl(TabId, ModuleId, string.Empty, string.Empty, -1, -1, -1, -1, "mytopics", 1, -1) + "\">[RESX:MyTopics]</a>");
                template = template.Replace("[AF:TB:MyProfile]", sProfileURL);
                template = template.Replace("[AF:TB:MySettings]", sSettingsURL);
                if (_mainSettings.MemberListMode == "ENABLEDREG")
                {
                    template = template.Replace("[AF:TB:MemberList]", sMemUrl);
                }

                if (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser)
                {
                    template = template.Replace("[AF:TB:ControlPanel]", "<a href=\"" + NavigateUrl(TabId, "EDIT", "mid=" + ModuleId) + "\">[RESX:ControlPanel]</a>");
                }
                else
                {
                    template = template.Replace("[AF:TB:ControlPanel]", string.Empty);
                }
                if (CurrentUserType == CurrentUserTypes.ForumMod || CurrentUserType == CurrentUserTypes.SuperUser || CurrentUserType == CurrentUserTypes.Admin)
                {
                    template = template.Replace("[AF:TB:ModList]", "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ViewType + "=modtopics") + "\">[RESX:Moderate]</a>");
                    if (_mainSettings.MemberListMode == "ENABLEDMOD")
                    {
                        template = template.Replace("[AF:TB:MemberList]", sMemUrl);
                    }
                }
                else
                {
                    template = template.Replace("[AF:TB:ModList]", string.Empty);
                }

            }
            else
            {
                template = template.Replace("[AF:TB:NotRead]", string.Empty);
                template = template.Replace("[AF:TB:MyTopics]", string.Empty);
                template = template.Replace("[AF:TB:ModList]", string.Empty);
                template = template.Replace("[AF:TB:MyProfile]", string.Empty);
                template = template.Replace("[AF:TB:MySettings]", string.Empty);
                template = template.Replace("[AF:TB:ControlPanel]", string.Empty);
                if (_mainSettings.MemberListMode == "DISABLED")
                {
                    template = template.Replace("[AF:TB:MemberList]", string.Empty);
                }
            }
            template = template.Replace("[AF:TB:Unanswered]", "<a href=\"" + ctlUtils.BuildUrl(TabId, ModuleId, string.Empty, string.Empty, -1, -1, -1, -1, "unanswered", 1, -1) + "\">[RESX:Unanswered]</a>");
            template = template.Replace("[AF:TB:ActiveTopics]", "<a href=\"" + ctlUtils.BuildUrl(TabId, ModuleId, string.Empty, string.Empty, -1, -1, -1, -1, "activetopics", 1, -1) + "\">[RESX:ActiveTopics]</a>");
            template = template.Replace("[AF:TB:Search]", "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ViewType + "=search") + "\">[RESX:Search]</a>");
            template = template.Replace("[AF:TB:Forums]", "<a href=\"" + NavigateUrl(TabId) + "\">[RESX:FORUMS]</a>");
            template = template.Replace("[AF:TB:MySettings]", string.Empty);
            if (_mainSettings.MemberListMode == "ENABLED")
            {
                template = template.Replace("[AF:TB:MemberList]", sMemUrl);
            }
            template = template.Replace("[AF:TB:MemberList]", string.Empty);
            return template;
        }
        public static string CleanStringForUrl(string Text)
        {
            Text = Text.Trim();
            Text = Text.Replace(":", string.Empty);
            Text = Regex.Replace(Text, "[^\\w]", "-");
            Text = Regex.Replace(Text, "([-]+)", "-");
            if (Text.EndsWith("-"))
            {
                Text = Text.Substring(0, Text.Length - 1);
            }
            return Text;
        }
        public static bool IsTrusted(int ForumTrustLevel, int UserTrustLevel, bool IsTrustedRole)
        {
            return IsTrusted(ForumTrustLevel, UserTrustLevel, IsTrustedRole, 0, 0);
        }
        public static bool IsTrusted(int ForumTrustLevel, int UserTrustLevel, bool IsTrustedRole, int AutoTrustLevel, int UserPostCount)
        {
            int trustSum = (ForumTrustLevel + UserTrustLevel);
            if (ForumTrustLevel == 0 && AutoTrustLevel == 0 && !IsTrustedRole && UserTrustLevel != 1)
            {
                return false;
            }
            if (UserTrustLevel == 1)
            {
                return true;
            }
            if (IsTrustedRole && UserTrustLevel != -1)
            {
                return true;
            }
            if (UserPostCount >= AutoTrustLevel && trustSum > 0 & UserTrustLevel != -1)
            {
                return true;
            }
            if (UserTrustLevel == -1)
            {
                return false;
            }
            if (trustSum > 0)
            {
                return true;
            }
            if (trustSum == 0 && UserTrustLevel == 0 && IsTrustedRole)
            {
                return true;
            }
            if (trustSum == 0 && UserTrustLevel == 0 && UserPostCount >= AutoTrustLevel)
            {
                return true;
            }
            return false;
        }
        public static DateTime NullDate()
        {
            System.Globalization.DateTimeFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
            return DateTime.Parse("1/1/1900", nfi);
        }
        public static string GetHost()
        {
            string strHost;
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                strHost = (Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/").Replace("http://", "https://");
            }
            else
            {
                strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
            }
            return strHost.ToLowerInvariant();

        }
        public static string NavigateUrl(int TabId)
        {
            var currParams = new string[0];
            if (HttpContext.Current.Request.Params["asgv"] != null)
            {
                currParams = AddParams("asgv=" + HttpContext.Current.Request.Params["asgv"], currParams);
            }
            if (HttpContext.Current.Request.Params["asg"] != null)
            {
                currParams = AddParams("asg=" + HttpContext.Current.Request.Params["asg"], currParams);
            }
            if (currParams.Length > 0)
            {
                return Common.Globals.NavigateURL(TabId, string.Empty, currParams);
            }
            return Common.Globals.NavigateURL(TabId);
        }
        public static string NavigateUrl(int TabId, string ControlKey, params string[] AdditionalParameters)
        {
            return NavigateUrl(TabId, ControlKey, string.Empty, -1, AdditionalParameters);
        }
        public static string NavigateUrl(int TabId, string ControlKey, string PageName, int portalId, params string[] AdditionalParameters)
        {
            string[] currParams = AdditionalParameters;
            if (HttpContext.Current.Request.Params["asgv"] != null)
            {
                currParams = AddParams("asgv=" + HttpContext.Current.Request.Params["asgv"], currParams);
            }
            if (HttpContext.Current.Request.Params["asg"] != null)
            {
                currParams = AddParams("asg=" + HttpContext.Current.Request.Params["asg"], currParams);
            }
            string s = Common.Globals.NavigateURL(TabId, ControlKey, currParams);
            if (portalId == -1 || string.IsNullOrEmpty(PageName))
            {
                return s;
            }
            var tc = new Entities.Tabs.TabController();
            var ti = tc.GetTab(TabId, portalId, false);
            string sURL = Common.Globals.ApplicationURL(TabId);
            foreach (string p in currParams)
            {
                sURL += "&" + p;
            }
            var _portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
            PageName = CleanStringForUrl(PageName);
            s = Common.Globals.FriendlyUrl(ti, sURL, PageName, _portalSettings);
            return s;
        }
        public static string[] AddParams(string param, string[] currParams)
        {
            var tmpParams = new[] { param };
            //Dim intLength As Integer = tmpParams.Length
            int currLength = currParams.Length;
            Array.Resize(ref currParams, (currLength + tmpParams.Length));
            tmpParams.CopyTo(currParams, currLength);
            return currParams;
        }

        public static void BindEnum(DropDownList pDDL, Type enumType, string pColValue, bool addEmptyValue)
        {
            BindEnum(pDDL, enumType, pColValue, addEmptyValue, false, -1);
        }
        public static void BindEnum(DropDownList pDDL, Type enumType, string pColValue, bool addEmptyValue, bool Localize, int ExcludeIndex)
        {
            pDDL.Items.Clear();

            Array values = Enum.GetValues(enumType);

            if (addEmptyValue)
            {
                pDDL.Items.Add(new ListItem("", "-1"));
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (i != ExcludeIndex)
                {
                    int key = Convert.ToInt32(Enum.Parse(enumType, values.GetValue(i).ToString()));
                    string text = Convert.ToString(Enum.Parse(enumType, values.GetValue(i).ToString()));
                    if (Localize)
                    {
                        text = "[RESX:" + text + "]";
                    }
                    pDDL.Items.Add(new ListItem(text, key.ToString()));
                }
            }
            if (pColValue != "")
            {
                pDDL.SelectedValue = Enum.Parse(enumType, pColValue).ToString();
            }
        }
        internal static string PrepareForEdit(int PortalId, int ModuleId, string ThemePath, string Text, bool AllowHTML, EditorTypes EditorType)
        {
            if (!AllowHTML)
            {
                Text = Text.Replace("&#91;", "[");
                Text = Text.Replace("&#93;", "]");
                Text = Text.Replace("<br>", System.Environment.NewLine);
                Text = Text.Replace("<br />", System.Environment.NewLine);
                Text = Text.Replace("<BR>", System.Environment.NewLine);
                Text = RemoveFilterWords(PortalId, ModuleId, ThemePath, Text);
                //Text = Utilities.StripHTMLTag(Text)
                return Text;
            }
            if (AllowHTML && EditorType == EditorTypes.TEXTBOX)
            {
                Text = Text.Replace("&#91;", "[");
                Text = Text.Replace("&#93;", "]");
                Text = Text.Replace("<br>", System.Environment.NewLine);
                Text = Text.Replace("<br />", System.Environment.NewLine);
                Text = Text.Replace("<BR>", System.Environment.NewLine);
                Text = RemoveFilterWords(PortalId, ModuleId, ThemePath, Text);
                return Text;
            }
            return Text;
        }

        public static string AutoLinks(string text, string currentSite)
        {
            string original = text;
            if (!(string.IsNullOrEmpty(text)))
            {

                string regHref = "<a.*?href=[\"'](?<url>.*?)[\"'].*?>(?<http>http.*?)</a>";
                foreach (Match m in Regex.Matches(text, "&lt;a.*?href=[\"'](?<url>.*?)[\"'].*?&gt;(http.*?)&lt;/a&gt;", RegexOptions.IgnoreCase))
                {
                    text = text.Replace(m.Value, HttpUtility.HtmlDecode(m.Value));
                }
                foreach (Match m in Regex.Matches(text, regHref, RegexOptions.IgnoreCase))
                {
                    text = text.Replace(m.Value, m.Groups["http"].Value.Contains("...") ? m.Groups["url"].Value : m.Groups["http"].Value);
                }
                if (string.IsNullOrEmpty(text))
                {
                    return original;
                }
                //text = Regex.Replace(text, regHref, "$1")
                string outSite = "<a href=\"{0}\" target=\"_blank\" rel=\"nofollow\">{1}</a>";
                string inSite = "<a href=\"{0}\">{1}</a>";
                string url;
                string urlText;
                foreach (Match m in Regex.Matches(text, "http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase))
                {
                    url = m.Value;
                    if (url.ToLowerInvariant().Contains("jpg") | url.ToLowerInvariant().Contains("gif") | url.ToLowerInvariant().Contains("png") | url.ToLowerInvariant().Contains("jpeg"))
                    {
                        break;
                    }

                    int xStart = 0;
                    if ((m.Index - 10) > 0)
                    {
                        xStart = m.Index - 10;
                    }
                    if (text.Substring(xStart, 10).ToLowerInvariant().Contains("href"))
                    {
                        break;
                    }
                    if (text.Substring(xStart, 10).ToLowerInvariant().Contains("src"))
                    {
                        break;
                    }
                    if (text.Substring(xStart, 10).ToLowerInvariant().Contains("="))
                    {
                        break;
                    }
                    urlText = m.Value;
                    if (urlText.Length > 47)
                    {
                        urlText = m.Value.Substring(0, 35) + "..." + m.Value.Substring(m.Value.Length - 10);

                    }
                    text = text.Replace(m.Value, url.ToLowerInvariant().Contains(currentSite.ToLowerInvariant()) ? string.Format(inSite, url, urlText) : string.Format(outSite, url, urlText));
                }
                if (string.IsNullOrEmpty(text))
                {
                    return original;
                }
            }
            return text;
        }

        public static string CleanString(int PortalId, string Text, bool AllowHTML, EditorTypes EditorType, bool UseFilter, bool AllowScript, int ModuleId, string ThemePath, bool ProcessEmoticons)
        {
            string sClean = Text;
            sClean = HttpContext.Current.Server.HtmlDecode(sClean);
            if (sClean != string.Empty)
            {

                if (AllowHTML)
                {
                    //sClean = Regex.Replace(sClean, Utilities.GetCaseInsensitiveSearch("<a href="), "<a href=")
                }
                else
                {
                    sClean = HTMLEncode(sClean);
                }
                sClean = EditorType == EditorTypes.TEXTBOX ? CleanTextBox(PortalId, sClean, AllowHTML, UseFilter, ModuleId, ThemePath, ProcessEmoticons) : CleanEditor(PortalId, sClean, UseFilter, ModuleId, ThemePath, ProcessEmoticons);

                var regExp = new Regex("(<a [^>]*>)(?'url'(\\S*?))(</a>)", RegexOptions.IgnoreCase);
                MatchCollection matches;
                matches = regExp.Matches(sClean);
                foreach (Match match in matches)
                {
                    string sNewURL = match.Groups[0].Value;
                    string sStart = match.Groups[1].Value;
                    string sText = match.Groups[2].Value;
                    string sEnd = match.Groups[3].Value;
                    if (sText.Length > 55)
                    {
                        sClean = sClean.Replace(sNewURL, sStart + sText.Substring(0, 35) + "..." + sText.Substring(sText.Length - 10) + sEnd);
                        //sClean = sClean.Replace(match.Groups(1).Value, sNewText.Substring(0, 35) & "..." & sNewText.Substring(sNewText.Length - 10))
                    }
                }
                if (AllowScript == false)
                {
                    sClean = sClean.Replace("&#91;", "[");
                    sClean = sClean.Replace("&#93;", "]");
                    sClean = XSSFilter(sClean);
                }
                sClean = sClean.Replace("[", "&#91;");
                sClean = sClean.Replace("]", "&#93;");
            }

            return sClean;
        }
        private static string CleanTextBox(int PortalId, string Text, bool AllowHTML, bool UseFilter, int ModuleId, string ThemePath, bool ProcessEmoticons)
        {
            string strMessage = Text;
            strMessage = HTMLDecode(strMessage);
            if (strMessage != string.Empty)
            {
                if (strMessage.ToUpper().Contains("[CODE]") | strMessage.ToUpper().Contains("<CODE"))
                {
                    //Dim intStart As Integer
                    //Dim intEnd As Integer
                    //Dim sCode As String
                    var codes = new List<string>();
                    int i = 0;
                    Regex objRegEx;
                    objRegEx = new Regex("(\\[CODE\\](.*?)\\[\\/CODE\\])", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    MatchCollection Matches;
                    Matches = objRegEx.Matches(strMessage);
                    foreach (Match m in Matches)
                    {
                        strMessage = strMessage.Replace(m.Value, "[CODEHOLDER" + i + "]");
                        codes.Add(m.Value);
                        i += 1;
                    }

                    //intStart = InStr(strMessage.ToUpper, "<CODE")
                    //intEnd = InStr(strMessage.ToUpper, "</CODE>") + 7
                    //sCode = strMessage.Substring(intStart - 1, intEnd - intStart)
                    //strMessage = Replace(strMessage, sCode, "[CODEHOLDER]")
                    strMessage = Regex.Replace(strMessage, GetCaseInsensitiveSearch("<form"), "&lt;form&gt;");
                    strMessage = Regex.Replace(strMessage, GetCaseInsensitiveSearch("</form>"), "&lt;/form&gt;");
                    if (UseFilter)
                    {
                        strMessage = FilterWords(PortalId, ModuleId, ThemePath, strMessage, ProcessEmoticons);
                    }
                    strMessage = HTMLEncode(strMessage);
                    strMessage = Regex.Replace(strMessage, System.Environment.NewLine, " <br /> ");
                    //strMessage = Replace(strMessage, "[CODEHOLDER]", sCode)
                    i = 0;
                    foreach (string s in codes)
                    {
                        strMessage = strMessage.Replace("[CODEHOLDER" + i + "]", HttpUtility.HtmlEncode(s));
                        i += 1;
                    }
                }
                else
                {
                    strMessage = Regex.Replace(strMessage, GetCaseInsensitiveSearch("<form"), "&lt;form&gt;");
                    strMessage = Regex.Replace(strMessage, GetCaseInsensitiveSearch("</form>"), "&lt;/form&gt;");
                    if (AllowHTML == false)
                    {
                        strMessage = HTMLEncode(strMessage);
                    }
                    if (UseFilter)
                    {
                        strMessage = FilterWords(PortalId, ModuleId, ThemePath, strMessage, ProcessEmoticons);
                    }

                    strMessage = Regex.Replace(strMessage, System.Environment.NewLine, " <br /> ");
                }
                strMessage = strMessage.Replace("[", "&#91;");
                strMessage = strMessage.Replace("]", "&#93;");
            }

            return strMessage;
        }
        private static string CleanEditor(int PortalId, string Text, bool UseFilter, int ModuleId, string ThemePath, bool ProcessEmoticons)
        {
            string strMessage = Text;
            strMessage = HTMLDecode(strMessage);
            if ((strMessage.ToUpper().IndexOf("<CODE", 0) + 1) > 0)
            {
                int intStart;
                int intEnd;
                string sCode;
                intStart = (strMessage.ToUpper().IndexOf("<CODE", 0) + 1);
                intEnd = (strMessage.ToUpper().IndexOf("</CODE>", 0) + 1) + 7;
                sCode = strMessage.Substring(intStart - 1, intEnd - intStart);
                strMessage = strMessage.Replace(sCode, "[CODEHOLDER]");
                if (UseFilter)
                {
                    strMessage = FilterWords(PortalId, ModuleId, ThemePath, strMessage, ProcessEmoticons);
                }
                strMessage = Regex.Replace(strMessage, GetCaseInsensitiveSearch("<form"), "&lt;form&gt;");
                strMessage = Regex.Replace(strMessage, GetCaseInsensitiveSearch("</form>"), "&lt;/form&gt;");
                //strMessage = HTMLEncode(strMessage)
                strMessage = strMessage.Replace("[CODEHOLDER]", sCode);
            }
            else
            {
                if (UseFilter)
                {
                    strMessage = FilterWords(PortalId, ModuleId, ThemePath, strMessage, ProcessEmoticons);
                }

                strMessage = Regex.Replace(strMessage, GetCaseInsensitiveSearch("<form"), "&lt;form&gt;");
                strMessage = Regex.Replace(strMessage, GetCaseInsensitiveSearch("</form>"), "&lt;/form&gt;");
            }
            return strMessage;
        }


        public static string GetCaseInsensitiveSearch(string strSearch)
        {
            string strReturn = "";
            char chrCurrent;
            char chrLower;
            char chrUpper;
            int intCounter;
            for (intCounter = 0; intCounter < strSearch.Length; intCounter++)
            {
                chrCurrent = strSearch[intCounter];
                chrLower = char.ToLower(chrCurrent);
                chrUpper = char.ToUpper(chrCurrent);
                if (chrUpper == chrLower)
                {
                    strReturn = strReturn + chrCurrent.ToString();
                }
                else
                {
                    strReturn = strReturn + "[" + chrLower.ToString() + chrUpper.ToString() + "]";
                }
            }
            return strReturn;
        }
        public static string FilterWords(int PortalId, int ModuleId, string ThemePath, string strMessage, bool ProcessEmoticons, bool RemoveHTML = false)
        {
            if (RemoveHTML)
            {
                string newSubject;
                newSubject = StripHTMLTag(strMessage);
                if (newSubject == string.Empty)
                {
                    newSubject = strMessage.Replace("<", "");
                    newSubject = newSubject.Replace(">", "");
                }
                strMessage = newSubject;
            }
            //Dim objFilter As FilterInfo
            IDataReader dr = DataProvider.Instance().Filters_List(PortalId, ModuleId, 0, 100000, "ASC", "FilterId");
            dr.NextResult();

            while (dr.Read())
            {
                string sReplace = dr["Replace"].ToString();
                string sFind = dr["Find"].ToString();
                switch (dr["FilterType"].ToString().ToUpper())
                {
                    case "MARKUP":
                        strMessage = strMessage.Replace(sFind, sReplace.Trim());
                        break;
                    case "EMOTICON":
                        if (ProcessEmoticons)
                        {
                            if ((sReplace.IndexOf("/emoticons", 0) + 1) > 0)
                            {
                                sReplace = "<img src='" + ThemePath + sReplace + "' align=\"absmiddle\"  border=\"0\" />";
                            }
                            strMessage = strMessage.Replace(sFind, sReplace);
                        }

                        break;
                    case "REGEX":
                        strMessage = Regex.Replace(strMessage, sFind.Trim(), sReplace, RegexOptions.IgnoreCase);

                        //Dim regExp As New Regex(sFind.Trim, RegexOptions.IgnoreCase)
                        //Dim matches As MatchCollection
                        //matches = regExp.Matches(strMessage.Trim)
                        //For Each match As Match In matches
                        //    Dim i As Integer = 0
                        //    For Each grp As Group In match.Groups
                        //        If Not grp.Value = String.Empty Then
                        //            If sReplace.Contains("{" & i & "}") Then
                        //                strMessage = strMessage.Replace(match.Groups(0).Value, sReplace.Replace("{" & i & "}", grp.Value))
                        //            End If
                        //        End If

                        //        i += 1
                        //    Next
                        //Next

                        break;
                }

            }
            dr.Close();
            return strMessage;
        }
        public static string RemoveFilterWords(int PortalId, int ModuleId, string ThemePath, string strMessage)
        {
            IDataReader dr = DataProvider.Instance().Filters_List(PortalId, ModuleId, 0, 100000, "ASC", "FilterId");
            dr.NextResult();

            while (dr.Read())
            {
                string sReplace = dr["Replace"].ToString();
                string sFind = dr["Find"].ToString();
                switch (dr["FilterType"].ToString().ToUpper())
                {
                    case "MARKUP":
                        strMessage = strMessage.Replace(sReplace, sFind.Trim());
                        break;
                    case "EMOTICON":
                        if ((sReplace.IndexOf("/emoticons", 0) + 1) > 0)
                        {
                            sReplace = "<img src='" + ThemePath + sReplace + "' align=\"absmiddle\"  border=\"0\" />";
                            strMessage = strMessage.Replace(sReplace, sFind);
                        }
                        break;
                }

            }
            dr.Close();
            strMessage = ManageImagePath(strMessage);
            return strMessage;
        }


        public static string ImportFilter(int PortalID, int ModuleID)
        {
            string @out;
            string myFile;
            try
            {
                myFile = HttpContext.Current.Server.MapPath("~/DesktopModules/ActiveForums/config/templates/Filters.txt");
                if (File.Exists(myFile))
                {
                    StreamReader objStreamReader;
                    string sFind;
                    string sReplace;
                    string sType;
                    try
                    {
                        objStreamReader = File.OpenText(myFile);
                    }
                    catch (Exception exc)
                    {
                        @out = exc.Message;
                        return @out;
                    }
                    string strFilter;
                    strFilter = objStreamReader.ReadLine();
                    while (strFilter != null)
                    {
                        string[] row = Regex.Split(strFilter, ",,");
                        sFind = row[0].Substring(1, row[0].Length - 2);
                        sReplace = row[1].Trim(' ');
                        sReplace = sReplace.Substring(1, sReplace.Length - 2);
                        sType = row[2].Substring(1, row[2].Length - 2);
                        DataProvider.Instance().Filters_Save(PortalID, ModuleID, -1, sFind, sReplace, sType);
                        strFilter = objStreamReader.ReadLine();
                    }
                    objStreamReader.Close();
                    @out = "Success";
                }
                else
                {
                    @out = "File Not Found<br />Path:" + myFile;
                }
            }
            catch (Exception exc)
            {
                @out = exc.Message;
            }

            return @out;
        }
        public static bool InputIsValid(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }
            body = body.Trim();
            if (string.IsNullOrEmpty(StripHTMLTag(body)) && !(body.ToUpper().Contains("<CODE")))
            {
                return false;
            }
            if (string.IsNullOrEmpty(body.Replace("&nbsp;", string.Empty)))
            {
                return false;
            }
            return true;
        }
        public static string HTMLEncode(string strMessage = "")
        {
            if (strMessage != "")
            {
                strMessage = strMessage.Replace(">", "&gt;");
                strMessage = strMessage.Replace("<", "&lt;");
            }

            return strMessage;
        }
        public static string ParsePre(string strMessage)
        {
            Regex objRegEx;
            objRegEx = new Regex("<pre>(.*?)</pre>");
            strMessage = "<code>" + HTMLDecode(objRegEx.Replace(strMessage, "$1")) + "</code>";
            return strMessage;
        }
        public static string HTMLDecode(string strMessage)
        {
            strMessage = strMessage.Replace("&gt;", ">");
            strMessage = strMessage.Replace("&lt;", "<");
            return strMessage;
        }
        public static string StripHTMLTag(string sText)
        {
            if (string.IsNullOrEmpty(sText))
            {
                return string.Empty;
            }
            string pattern = "<(.|\\n)*?>";
            return Regex.Replace(sText, pattern, string.Empty).Trim();
        }
        public static bool HasHTML(string sText)
        {
            if (string.IsNullOrEmpty(sText))
            {
                return false;
            }
            string pattern = "<(.|\\n)*?>";
            return Regex.IsMatch(sText, pattern);
        }
        public static string StripTokens(string sText)
        {
            sText = sText.Replace("AF:DIR:", "AFHOLD:");
            string pattern = "(\\[AF:.+?\\])";
            sText = Regex.Replace(sText, pattern, string.Empty);
            sText = Regex.Replace(sText, "(\\[/AF:.+?\\])", string.Empty);
            sText = Regex.Replace(sText, "(\\[RESX:.+?\\])", string.Empty);
            sText = sText.Replace("[ATTACHMENTS]", string.Empty);
            sText = sText.Replace("[MODEDITDATE]", string.Empty);
            sText = sText.Replace("[SIGNATURE]", string.Empty);
            sText = sText.Replace("AFHOLD:", "AF:DIR:");
            return sText;
        }
        public static string XSSFilter(string sText = "", bool RemoveHTML = false)
        {
            //If sText.Contains("&#") Then
            //    sText = StrongDecode(sText)
            //End If
            //If Regex.Match(sText, "<.*&#[a-zA-Z0-9].*>").Success Then
            //    sText = Regex.Replace(sText, "<.*&#[a-zA-Z0-9].*>", String.Empty)
            //End If
            string pattern = "<style.*/*>|</style>|<script.*/*>|</script>|<[a-zA-Z][^>]*=[`'\"]+javascript:\\w+.*[`'\"]+>|<\\w+[^>]*\\son\\w+.*[ /]*>|<[a-zA-Z][^>].*=javascript:.*>|<\\w+[^>]*[\\x00-\\x20]*=[\\x00-\\x20]*[`'\"]*[\\x00-\\x20]*j[\\x00-\\x20]*a[\\x00-\\x20]*v[\\x00-\\x20]*a[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*(.|\\n)*?";
            foreach (Match m in Regex.Matches(sText, pattern, RegexOptions.IgnoreCase))
            {
                sText = sText.Replace(m.Value, StrongEncode(m.Value));
            }
            //sText = Regex.Replace(sText, pattern, StrongEncode, RegexOptions.IgnoreCase)
            if (RemoveHTML)
            {
                sText = StripHTMLTag(sText);
            }
            return sText;
        }
        public static string StripExecCode(string sText)
        {
            int i = 0;
            while (i < sText.Length)
            {
                Match m;
                var aspTag = new System.Web.RegularExpressions.TagRegex();
                m = aspTag.Match(sText, i);
                if (m.Success)
                {
                    i = m.Index + m.Length;
                    string tag = m.Value;
                    var aspRunAt = new System.Web.RegularExpressions.RunatServerRegex();
                    Match mInner = aspRunAt.Match(m.Value, 0);
                    if (mInner.Success)
                    {
                        sText = sText.Replace(tag, HttpUtility.HtmlEncode(m.Value));
                        var endTag = new System.Web.RegularExpressions.EndTagRegex();
                        m = endTag.Match(sText, i);
                        if (m.Success)
                        {
                            i = m.Index + m.Length;
                            sText = sText.Replace(m.Value, HttpUtility.HtmlEncode(m.Value));

                        }
                    }

                    continue;
                }

                i += 1;
            }

            //RUNAT SERVER
            //Dim ServerReg As New System.Web.RegularExpressions.ServerTagsRegex

            //For Each m As Match In ServerReg.Matches(sText)
            //    sText = sText.Replace(m.Value, HttpUtility.HtmlEncode(m.Value))
            //Next

            //Dim AspControl As New System.Web.RegularExpressions.TagRegex

            //For Each m As Match In AspControl.Matches(sText)
            //    sText = sText.Replace(m.Value, HttpUtility.HtmlEncode(m.Value))
            //Next


            //For Each m As Match In Regex.Matches(sText, "runat\W*server.*?>", RegexOptions.IgnoreCase)
            //    sText = sText.Replace(m.ToString, HttpUtility.HtmlEncode(m.ToString))
            //    ' sText = sText.Replace(m.ToString, StrongEncode(m.ToString))
            //Next
            //CODE
            foreach (Match m in Regex.Matches(sText, "<%(?!@)(?<code>.*?)%>", RegexOptions.IgnoreCase))
            {
                sText = sText.Replace(m.ToString(), HttpUtility.HtmlEncode(m.ToString().Replace("<br />", System.Environment.NewLine)));
            }

            //SERVER Tags
            //For Each m As Match In Regex.Matches(sText, "(<(?<tagname>[\w:\.]+)(\s+(?<attrname>\w[-\w:]*)(\s*=\s*""(?<attrval>[^""]*)""|\s*=\s*'(?<attrval>[^']*)'|\s*=\s*(?<attrval><%#.*?%>)|\s*=\s*(?!'|"")(?<attrval>[^\s=/>]*)(?!'|"")|(?<attrval>\s*?)))*\s*(?<empty>/)?>)", RegexOptions.IgnoreCase)
            //    sText = sText.Replace(m.ToString, HttpUtility.HtmlEncode(m.ToString.Replace("<br />", vbCrLf)))
            //Next
            //INCLUDES
            foreach (Match m in Regex.Matches(sText, "<!--\\s*#(?i:include)\\s*(?<pathtype>[\\w]+)\\s*=\\s*[\"']?(?<filename>[^\\\"']*?)[\"']?\\s*-->", RegexOptions.IgnoreCase))
            {
                sText = sText.Replace(m.ToString(), StrongEncode(m.ToString()));
            }
            sText = sText.Replace("<!--", "&lt;&#33;&#45;&#45;");
            sText = sText.Replace("-->", "&#45;&#45;&gt;");
            return sText;
        }
        public static string StrongEncode(string text)
        {
            string @out = string.Empty;
            foreach (var s in text.ToCharArray())
            {
                @out += "&#" + Convert.ToInt32(s) + ";";
            }
            return @out;
        }
        public static string StrongDecode(string text)
        {
            string @out = string.Empty;
            foreach (Match m in Regex.Matches(text, "&#[a-zA-Z0-9];"))
            {
                string scode = m.Value.Replace("&#", string.Empty).Replace(";", string.Empty);
                text = text.Replace(m.Value, scode);
            }
            return text;
        }
        public static string CheckSqlString(string input)
        {
            input = input.Replace("\\", "");
            input = input.Replace("[", "");
            input = input.Replace("]", "");
            input = input.Replace("(", "");
            input = input.Replace(")", "");
            input = input.Replace("{", "");
            input = input.Replace("}", "");
            input = input.Replace("'", "''");
            input = input.Replace("UNION", "");
            input = input.Replace("TABLE", "");
            input = input.Replace("WHERE", "");
            input = input.Replace("DROP", "");
            input = input.Replace("EXECUTE", "");
            input = input.Replace("EXEC ", "");
            input = input.Replace("FROM ", "");
            input = input.Replace("CMD ", "");
            input = input.Replace(";", "");
            input = input.Replace("--", "");


            //input = input.Replace("""", """""")

            return input;
        }
        internal static string CleanName(string name)
        {
            string currentName = name;
            if (name == "-1")
            {
                return "-1";
            }
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            name = name.Trim();
            char[] chars = "_$%#@!*?;:~`+=()[]{}|\\'<>,/^&\".".ToCharArray();
            name = name.Replace(". ", ".");
            name = name.Replace(" .", ".");
            name = name.Replace("- ", "-");
            name = name.Replace(" -", "-");
            name = name.Replace(".", "-");
            name = name.Replace(" ", "-");
            for (int i = 0; i < chars.Length; i++)
            {
                string strChar = chars.GetValue(i).ToString();
                if (name.Contains(strChar))
                {
                    name = name.Replace(strChar, string.Empty);
                }
            }
            name = name.Replace("--", "-");
            name = name.Replace("---", "-");
            name = name.Replace("----", "-");
            name = name.Replace("-----", "-");
            name = name.Replace("----", "-");
            name = name.Replace("---", "-");
            name = name.Replace("--", "-");
            name = name.Trim('-');
            if (name.Length > 0)
            {
                return name;
            }
            return currentName;
        }
        internal static bool IsRewriteLoaded()
        {
            try
            {
                string sConfig = GetFile(HttpContext.Current.Server.MapPath("~/web.config"));
                if (sConfig.Contains("DotNetNuke.Modules.ActiveForums.ForumsReWriter"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Get the template as a string from the specified path
        /// </summary>
        /// <param name="FilePath">Physical path to file</param>
        /// <returns>String</returns>
        /// <remarks></remarks>
        internal static string GetFile(string FilePath)
        {
            string sContents = string.Empty;
            if (File.Exists(FilePath))
            {
                try
                {
                    using (var sr = new StreamReader(FilePath))
                    {
                        sContents = sr.ReadToEnd();
                        sr.Close();
                    }
                }
                catch (Exception exc)
                {
                    sContents = exc.Message;
                }
            }
            return sContents;
        }
        public static string ManageImagePath(string sHTML)
        {
            // Dim strHost As String = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Current.Request)) & "/" 'DotNetNuke.Common.Globals.AddHTTP(Current.Request.Url.Host)
            string strHost = Common.Globals.AddHTTP(HttpContext.Current.Request.Url.Host);
            return ManageImagePath(sHTML, strHost);
        }
        public static string ManageImagePath(string sHTML, string HostURL)
        {
            string strHost = HostURL.ToLower(); //DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Current.Request)) & "/"

            int iStart = sHTML.IndexOf("src='/");
            while (iStart != -1)
            {
                sHTML = sHTML.Insert(iStart + 5, strHost);
                iStart = sHTML.IndexOf("src='/");
            }
            iStart = sHTML.IndexOf("src=\"/");
            while (iStart != -1)
            {
                sHTML = sHTML.Insert(iStart + 5, strHost);
                iStart = sHTML.IndexOf("src=\"/");
            }
            return sHTML;

        }
        internal static string GetFileContent(string FilePath)
        {
            string sPath = FilePath;
            if (!(sPath.Contains(":\\")) && !(sPath.Contains("\\\\")))
            {
                sPath = HttpContext.Current.Server.MapPath(sPath);
            }
            //If sPath.Contains("~/") Then

            //End If
            string sContents = string.Empty;
            StreamReader objStreamReader;
            if (File.Exists(sPath))
            {
                try
                {
                    objStreamReader = File.OpenText(sPath);
                    sContents = objStreamReader.ReadToEnd();
                    objStreamReader.Close();
                }
                catch (Exception exc)
                {
                    sContents = exc.Message;
                }
            }
            return sContents;
        }
        public static string GetDate(DateTime DisplayDate, int MID, int offset)
        {
            string dateStr;
            System.Globalization.DateTimeFormatInfo dtFI = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
            dtFI = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            try
            {
                DateTime newDate;

                int mServerOffSet;
                int mUserOffSet = 0;
                SettingsInfo _mainSettings = DataCache.MainSettings(MID);
                mServerOffSet = _mainSettings.TimeZoneOffset;
                newDate = DisplayDate.AddMinutes(-mServerOffSet);

                newDate = newDate.AddMinutes(offset);
                //If Not Current.Session["UserOffSet"] Is Nothing Then
                //    mUserOffSet = CType(Current.Session["UserOffSet"), Integer]
                //End If
                //If mServerOffSet > 0 Then
                //    newDate = DisplayDate.AddMinutes(-mServerOffSet)
                //Else
                //    newDate = DisplayDate.AddMinutes(Math.Abs(mServerOffSet))
                //End If
                //newDate = newDate.AddMinutes(mUserOffSet)
                string dateFormat = _mainSettings.DateFormatString;
                string timeFormat = _mainSettings.TimeFormatString;
                string formatString;
                formatString = dateFormat + " " + timeFormat;
                try
                {
                    dateStr = newDate.ToString(formatString);
                }
                catch
                {
                    dateStr = DisplayDate.ToString();
                }
                return dateStr;
            }
            catch (Exception ex)
            {
                dateStr = DisplayDate.ToString();
                return dateStr;
            }
        }
        public static string GetLastPostSubject(int LastPostID, int ParentPostID, int ForumID, int TabID, string Subject, int Length, int PageSize, int ReplyCount, bool CanRead)
        {
            var sb = new System.Text.StringBuilder();
            int PostId = LastPostID;
            Subject = StripHTMLTag(Subject);
            Subject = Subject.Replace("[", "&#91");
            Subject = Subject.Replace("]", "&#93");
            if (LastPostID != 0)
            {
                if (Subject.Length > Length & Length > 0)
                {
                    Subject = Subject.Substring(0, Length) + "...";
                }
                if (ParentPostID != 0)
                {
                    LastPostID = ParentPostID;
                }
                if (ReplyCount > 1)
                {
                    int intPages;
                    intPages = Convert.ToInt32(Math.Ceiling(ReplyCount / (double)PageSize));
                    if (CanRead)
                    {
                        string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + LastPostID, ParamKeys.PageJumpId + "=" + intPages };
                        sb.Append("<a href=\"" + Common.Globals.NavigateURL(TabID, "", Params) + "#" + PostId + "\" rel=\"nofollow\">" + HTMLEncode(Subject) + "</a>");
                    }
                    else
                    {
                        sb.Append(HTMLEncode(Subject));
                    }

                }
                else
                {
                    if (CanRead)
                    {
                        string[] Params = { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + LastPostID };
                        sb.Append("<a href=\"" + Common.Globals.NavigateURL(TabID, "", Params) + "#" + PostId + "\" rel=\"nofollow\">" + HTMLEncode(Subject) + "</a>");
                    }
                    else
                    {
                        sb.Append(HTMLEncode(Subject));
                    }

                }
            }
            return sb.ToString();
        }

        public static string ParseSpacer(string Template)
        {
            string sOutput;
            string strHost = Common.Globals.AddHTTP(Common.Globals.GetDomainName(HttpContext.Current.Request)) + "/";
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                strHost = strHost.Replace("http://", "https://");
            }
            if ((Template.IndexOf("[SPACER", 0) + 1) > 0)
            {
                sOutput = Template;
                int intStart;
                int intEnd;
                int intHeight;
                int intWidth;
                string sReplace = "[SPACER:";
                string sTemp = null;
                string sImg;
                intStart = (sOutput.IndexOf("[SPACER:", 0) + 1) + 8;
                intEnd = (sOutput.IndexOf(":", intStart) + 1);
                sTemp = sOutput.Substring(intStart - 1, intEnd - intStart);
                intHeight = Convert.ToInt32(sOutput.Substring(intStart - 1, intEnd - intStart));
                sTemp = sOutput.Substring(intEnd, ((sOutput.IndexOf("]", intEnd - 1) + 1) - 1) - intEnd);
                intWidth = Convert.ToInt32(sOutput.Substring(intEnd, ((sOutput.IndexOf("]", intEnd - 1) + 1) - 1) - intEnd));
                sReplace += intHeight.ToString() + ":" + intWidth.ToString() + "]";
                sImg = "<img src=\"" + strHost + "DesktopModules/ActiveForums/images/spacer.gif\" alt=\"--\" width=\"" + intWidth + "\" height=\"" + intHeight + "\" />";
                sOutput = sOutput.Replace(sReplace, sImg);
                Template = ParseSpacer(sOutput);
            }
            return Template;
        }
        internal static string GetSqlString(string SqlFile)
        {
            string resourceLocation = SqlFile;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation);
            var sr = new StreamReader(stream);
            string contents = sr.ReadToEnd();
            sr.Close();
            return contents;
        }
        public static string LocalizeControl(string controlText)
        {
            return LocalizeControl(controlText, string.Empty, false, false);
        }

        public static string LocalizeControl(string controlText, string ResourceFile)
        {
            return LocalizeControl(controlText, ResourceFile, false, false);
        }
        public static string LocalizeControl(string controlText, bool IsAdmin)
        {
            return LocalizeControl(controlText, string.Empty, IsAdmin, false);
        }
        public static string LocalizeControl(string controlText, bool IsAdmin, bool scriptSafe)
        {
            return LocalizeControl(controlText, string.Empty, IsAdmin, scriptSafe);
        }
        private static string LocalizeControl(string controlText, string resourceFile, bool IsAdmin, bool scriptSafe)
        {
            //controlText = controlText.Replace(" & ", " &amp; ")
            //controlText = controlText.Replace("<BR>", "<br />")
            //controlText = controlText.Replace("<br>", "<br />")
            //controlText = controlText.Replace("<P>", "<p>")
            //controlText = controlText.Replace("</P>", "</p>")
            //controlText = controlText.Replace("</A>", "</a>")
            //controlText = controlText.Replace("<P ", "<p ")
            //controlText = controlText.Replace("</DIV>", "</div>")
            //controlText = controlText.Replace("<DIV", "<div")
            //controlText = controlText.Replace(" target=_blank", " target=""_blank""")
            controlText = controlText.Replace(" class=afquote", " class=\"afquote\"");

            int i = 0;
            int intStart = 0;
            int intEnd = 0;
            string sKey;
            string sReplace;
            string pattern = "(\\[RESX:.+?\\])";
            Regex regExp = new Regex(pattern);
            MatchCollection matches;
            matches = regExp.Matches(controlText);
            foreach (Match match in matches)
            {
                sKey = match.Value;
                if (IsAdmin)
                {
                    sReplace = GetSharedResource(match.Value, true);
                }
                else if (resourceFile != string.Empty)
                {
                    sReplace = GetSharedResource(match.Value, resourceFile);
                }
                else
                {
                    sReplace = GetSharedResource(match.Value);
                }
                string newValue = match.Value;
                if (!(string.IsNullOrEmpty(sReplace)))
                {
                    newValue = sReplace;
                }
                if (scriptSafe)
                {
                    newValue = HttpUtility.HtmlEncode(newValue).Replace("'", "\\'");
                    newValue = newValue.Replace("[", "\\[").Replace("]", "\\]");
                    newValue = JSON.EscapeJsonString(newValue);
                }
                controlText = controlText.Replace(sKey, newValue);
                //If Not String.IsNullOrEmpty(sReplace) Then
                //    controlText = controlText.Replace(match.Value, sReplace)
                //Else
                //    controlText = controlText.Replace(match.Value, match.Value)
                //End If

            }
            return controlText;
        }
        public static string GetSharedResource(string key, string ResourceFile)
        {
            return Services.Localization.Localization.GetString(key, "~/DesktopModules/ActiveForums/App_LocalResources/" + ResourceFile + ".resx");
        }
        public static string GetSharedResource(string key, bool IsAdmin = false)
        {
            string sValue;
            if (IsAdmin)
            {
                sValue = Services.Localization.Localization.GetString(key, "~/DesktopModules/ActiveForums/App_LocalResources/ControlPanel.ascx.resx");
                if (sValue == string.Empty)
                {
                    return key;
                }
                return sValue;
            }
            sValue = Services.Localization.Localization.GetString(key, "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx");
            if (sValue == string.Empty)
            {
                return key;
            }
            return sValue;
        }
        public static string FormatFileSize(int FileSize)
        {
            try
            {
                if (FileSize >= 1073741824)
                {
                    return (FileSize / 1024.0 / 1024.0 / 1024.0).ToString("#0.00") + " GB";
                }
                if (FileSize >= 1048576)
                {
                    return (FileSize / 1024.0 / 1024.0).ToString("#0.00") + " MB";
                }
                if (FileSize >= 1024)
                {
                    return (FileSize / 1024.0).ToString("#0.00") + " KB";
                }
                if (FileSize < 1024)
                {
                    return FileSize + " Bytes";
                }
            }
            catch (Exception ex)
            {
                return "0 Bytes";
            }
            return "0 Bytes";
        }
        public static object ConvertFromHashTableToObject(Hashtable ht, object InfoObject)
        {
            Type myType = InfoObject.GetType();
            var myProperties = myType.GetProperties((BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
            string sKey;
            string sValue;
            foreach (PropertyInfo pItem in myProperties)
            {
                sValue = string.Empty;
                sKey = pItem.Name.ToLower();
                foreach (string k in ht.Keys)
                {
                    if (k.ToLowerInvariant() == sKey.ToLowerInvariant())
                    {
                        sValue = ht[k].ToString();
                        break;
                    }
                }

                if (!(string.IsNullOrEmpty(sValue)))
                {
                    object obj = null;
                    switch (pItem.PropertyType.ToString())
                    {
                        case "System.Int16":
                            obj = Convert.ToInt32(sValue);
                            break;
                        case "System.Int32":
                            obj = Convert.ToInt32(sValue);
                            break;
                        case "System.Int64":
                            obj = Convert.ToInt64(sValue);
                            break;
                        case "System.Single":
                            obj = Convert.ToSingle(sValue);
                            break;
                        case "System.Double":
                            obj = Convert.ToDouble(sValue);
                            break;
                        case "System.Decimal":
                            obj = Convert.ToDecimal(sValue);
                            break;
                        case "System.DateTime":
                            obj = Convert.ToDateTime(sValue);
                            break;
                        case "System.String":
                        case "System.Char":
                            obj = Convert.ToString(sValue);
                            break;
                        case "System.Boolean":
                            obj = Convert.ToBoolean(sValue);
                            break;
                        case "System.Guid":
                            obj = new Guid(sValue);

                            break;
                    }
                    if (obj != null)
                    {
                        InfoObject.GetType().GetProperty(pItem.Name).SetValue(InfoObject, obj, BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
                    }

                }
            }

            return InfoObject;
        }
        internal static string GetFileResource(string resourceName)
        {
            string contents;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var sr = new StreamReader(s))
                {
                    contents = sr.ReadToEnd();
                    sr.Close();
                }
                s.Close();
            }
            return contents;
        }
        public static List<Entities.Users.UserInfo> GetListOfModerators(int PortalId, int ForumId)
        {
            var rc = new Security.Roles.RoleController();
            var uc = new Entities.Users.UserController();
            var fc = new ForumController();
            Forum fi = fc.Forums_Get(ForumId, -1, false, true);
            if (fi == null)
            {
                return null;
            }
            var mods = new List<Entities.Users.UserInfo>();
            SubscriptionInfo si = null;
            string modApprove = fi.Security.ModApprove;
            string[] modRoles = modApprove.Split('|')[0].Split(';');
            if (modRoles != null)
            {
                foreach (string r in modRoles)
                {
                    if (!(string.IsNullOrEmpty(r)))
                    {
                        int rid = Convert.ToInt32(r);
                        string rName = rc.GetRole(rid, PortalId).RoleName;
                        foreach (Entities.Users.UserRoleInfo usr in rc.GetUserRolesByRoleName(PortalId, rName))
                        {
                            var ui = uc.GetUser(PortalId, usr.UserID);

                            if (!(mods.Contains(ui)))
                            {
                                mods.Add(ui);
                            }
                        }
                    }
                }
            }
            return mods;
        }
    }

}

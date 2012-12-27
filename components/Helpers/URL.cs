using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
	public class URL
	{
		public static string ForumLink(int TabId, Forum fi)
		{
			SettingsInfo _mainSettings = DataCache.MainSettings(fi.ModuleId);
			string sURL = string.Empty;
			if (string.IsNullOrEmpty(fi.PrefixURL) || ! _mainSettings.URLRewriteEnabled)
			{
				sURL = Utilities.NavigateUrl(TabId, string.Empty, ParamKeys.ForumId + "=" + fi.ForumID.ToString());
			}
			else
			{
				if (! (string.IsNullOrEmpty(_mainSettings.PrefixURLBase)))
				{
					sURL = "/" + _mainSettings.PrefixURLBase;
				}
				if (! (string.IsNullOrEmpty(fi.ForumGroup.PrefixURL)))
				{
					sURL += "/" + fi.ForumGroup.PrefixURL;
				}
				sURL += "/" + fi.PrefixURL + "/";
			}
			string sHost = Utilities.GetHost();
			if (! (sURL.StartsWith(sHost)))
			{
				if (sHost.EndsWith("/"))
				{
					sHost = sHost.Substring(0, sHost.Length - 1);
				}
				sURL = sHost + sURL;
			}
			return sURL;
		}
		public static string TopicLink(int TabId, int ModuleId, TopicInfo ti)
		{
			Data.Common db = new Data.Common();
			string sURL = string.Empty;
			SettingsInfo _mainSettings = DataCache.MainSettings(ModuleId);
			if (string.IsNullOrEmpty(ti.URL) || ! _mainSettings.URLRewriteEnabled)
			{
				sURL = Utilities.NavigateUrl(TabId, string.Empty, ParamKeys.TopicId + "=" + ti.TopicId);
			}
			else
			{
				sURL = "/" + db.GetUrl(ModuleId, -1, -1, ti.TopicId, -1, -1);
			}
			string sHost = Utilities.GetHost();
			if (! (sURL.StartsWith(sHost)))
			{
				if (sHost.EndsWith("/"))
				{
					sHost = sHost.Substring(0, sHost.Length - 1);
				}
				sURL = sHost + sURL;
			}
			return sURL;
		}
		public static string ReplyLink(int TabId, TopicInfo ti, int UserId, int ReplyId)
		{
			Data.Common db = new Data.Common();
			string sURL = Utilities.NavigateUrl(TabId, string.Empty, new string[] {ParamKeys.TopicId + "=" + ti.TopicId, ParamKeys.ContentJumpId + "=" + ReplyId});
			if (string.IsNullOrEmpty(ti.URL) || ! Utilities.IsRewriteLoaded())
			{
				return sURL;
			}
			else
			{
				sURL = db.GetUrl(-1, -1, -1, ti.TopicId, UserId, ReplyId);
				if (! (string.IsNullOrEmpty(sURL)))
				{
					string sHost = Utilities.GetHost();
					if (sURL.StartsWith("/"))
					{
						sURL = sURL.Substring(1);
					}
					if (! (sHost.EndsWith("/")))
					{
						sHost += "/";
					}
					sURL = sHost + sURL;
					if (! (sURL.EndsWith("/")))
					{
						sURL += "/";
					}
					if (ReplyId > 0)
					{
						sURL += "#" + ReplyId.ToString();
					}
				}
			}
			return sURL;


		}
		public static string ForForum(int PageId, int ForumId, string GroupName, string ForumName)
		{
			string sURL = Utilities.NavigateUrl(PageId, "", new string[] {ParamKeys.ForumId + "=" + ForumId});
			sURL = sURL.ToLowerInvariant();
			string sNewPage = string.Empty;
			if (! (string.IsNullOrEmpty(GroupName)))
			{
				sNewPage = Utilities.CleanStringForUrl(GroupName) + "/";
			}
			if (! (string.IsNullOrEmpty(ForumName)))
			{
				sNewPage += Utilities.CleanStringForUrl(ForumName);
			}
			if (! (string.IsNullOrEmpty(sNewPage)))
			{
				sURL = sURL.Replace("default.aspx", sNewPage + ".aspx");
			}
			return sURL.ToLowerInvariant();
		}

		public static string ForTopic(int PageId, int PortalId, int ForumId, int TopicId)
		{
			return ForTopic(PageId, PortalId, ForumId, TopicId, string.Empty, string.Empty, string.Empty, 1);
		}
		public static string ForTopic(int PageId, int PortalId, int ForumId, int TopicId, string GroupName, string ForumName, string Subject, int PageNumber)
		{
			string sURL = string.Empty;
			if (PageNumber > 1)
			{
				//        sURL = Utilities.NavigateUrl(PageId, "", New String() {ParamKeys.ForumId & "=" & ForumId, ParamKeys.TopicId & "=" & TopicId, ParamKeys.PageId & "=" & PageNumber})
				sURL = Utilities.NavigateUrl(PageId, "", Subject, PortalId, new string[] {ParamKeys.TopicId + "=" + TopicId, ParamKeys.PageId + "=" + PageNumber});
			}
			else
			{
				//sURL = Utilities.NavigateUrl(PageId, "", New String() {ParamKeys.ForumId & "=" & ForumId, ParamKeys.TopicId & "=" & TopicId})
				sURL = Utilities.NavigateUrl(PageId, "", Subject, PortalId, new string[] {ParamKeys.TopicId + "=" + TopicId});
			}
			sURL = sURL.ToLowerInvariant();
			if (! (sURL.EndsWith(".aspx")))
			{
				sURL += ".aspx";
			}
			return sURL;
			string sNewPage = string.Empty;
			if (! (string.IsNullOrEmpty(GroupName)))
			{
				sNewPage = Utilities.CleanStringForUrl(GroupName) + "/";
			}
			if (! (string.IsNullOrEmpty(ForumName)))
			{
				sNewPage += Utilities.CleanStringForUrl(ForumName) + "/";
			}
			if (! (string.IsNullOrEmpty(Subject)))
			{
				sNewPage += Utilities.CleanStringForUrl(Subject);
			}
			if (! (string.IsNullOrEmpty(sNewPage)))
			{
				sURL = sURL.Replace("default.aspx", sNewPage + ".aspx");
			}
			return sURL.ToLowerInvariant();
		}

	}
}
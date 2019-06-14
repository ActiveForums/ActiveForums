//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
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

namespace DotNetNuke.Modules.ActiveForums
{
	public class URL
	{
		public static string ForumLink(int tabId, Forum fi)
		{
			var mainSettings = DataCache.MainSettings(fi.ModuleId);
			
            var sURL = string.Empty;

            if (string.IsNullOrWhiteSpace(fi.PrefixURL) || !mainSettings.URLRewriteEnabled)
			{
				sURL = Utilities.NavigateUrl(tabId, string.Empty, ParamKeys.ForumId + "=" + fi.ForumID);
			}
			else
			{
				if (!(string.IsNullOrWhiteSpace(mainSettings.PrefixURLBase)))
				{
					sURL = "/" + mainSettings.PrefixURLBase;
				}

                if (!(string.IsNullOrWhiteSpace(fi.ForumGroup.PrefixURL)))
				{
					sURL += "/" + fi.ForumGroup.PrefixURL;
				}

				sURL += "/" + fi.PrefixURL + "/";
			}

			var sHost = Utilities.GetHost();
			if (! (sURL.StartsWith(sHost)))
			{
				if (sHost.EndsWith("/"))
					sHost = sHost.Substring(0, sHost.Length - 1);

				sURL = sHost + sURL;
			}

			return sURL;
		}

		public static string TopicLink(int tabId, int moduleId, TopicInfo ti)
		{	
			string sURL;
			var mainSettings = DataCache.MainSettings(moduleId);

			if (string.IsNullOrEmpty(ti.URL) || !mainSettings.URLRewriteEnabled)
			{
				sURL = Utilities.NavigateUrl(tabId, string.Empty, ParamKeys.TopicId + "=" + ti.TopicId);
			}
			else
			{
                var db = new Data.Common();
				sURL = "/" + db.GetUrl(moduleId, -1, -1, ti.TopicId, -1, -1);
			}

			var sHost = Utilities.GetHost();
			if (! (sURL.StartsWith(sHost)))
			{
				if (sHost.EndsWith("/"))
					sHost = sHost.Substring(0, sHost.Length - 1);

				sURL = sHost + sURL;
			}

			return sURL;
		}

		public static string ReplyLink(int tabId, TopicInfo ti, int userId, int replyId)
		{
			var sURL = Utilities.NavigateUrl(tabId, string.Empty, new [] { ParamKeys.TopicId + "=" + ti.TopicId, ParamKeys.ContentJumpId + "=" + replyId });

			if (string.IsNullOrEmpty(ti.URL) || ! Utilities.IsRewriteLoaded())
				return sURL;

            var db = new Data.Common();
			sURL = db.GetUrl(-1, -1, -1, ti.TopicId, userId, replyId);
			if (! (string.IsNullOrEmpty(sURL)))
			{
				var sHost = Utilities.GetHost();
				if (sURL.StartsWith("/"))
					sURL = sURL.Substring(1);

				if (!(sHost.EndsWith("/")))
					sHost += "/";

				sURL = sHost + sURL;
				if (! (sURL.EndsWith("/")))
					sURL += "/";

				if (replyId > 0)
					sURL += "#" + replyId.ToString();
			}

			return sURL;
		}

		public static string ForForum(int pageId, int forumId, string groupName, string forumName)
		{
			var sURL = Utilities.NavigateUrl(pageId, string.Empty, ParamKeys.ForumId + "=" + forumId);

			var sNewPage = string.Empty;
			if (! (string.IsNullOrEmpty(groupName)))
				sNewPage = Utilities.CleanStringForUrl(groupName) + "/";

			if (! (string.IsNullOrEmpty(forumName)))
				sNewPage += Utilities.CleanStringForUrl(forumName);

			if (! (string.IsNullOrEmpty(sNewPage)))
				sURL = sURL.Replace("default.aspx", sNewPage + ".aspx");

			return sURL.ToLowerInvariant();
		}

		public static string ForTopic(int pageId, int portalId, int forumId, int topicId)
		{
			return ForTopic(pageId, portalId, forumId, topicId, string.Empty, string.Empty, string.Empty, 1);
		}

		public static string ForTopic(int pageId, int portalId, int forumId, int topicId, string groupName, string forumName, string subject, int pageNumber)
		{
			string sURL;

			if (pageNumber > 1)
				sURL = Utilities.NavigateUrl(pageId, "", subject, portalId, new [] {ParamKeys.TopicId + "=" + topicId, ParamKeys.PageId + "=" + pageNumber});
			else
				sURL = Utilities.NavigateUrl(pageId, "", subject, portalId, ParamKeys.TopicId + "=" + topicId);

			sURL = sURL.ToLowerInvariant();
			if (!(sURL.EndsWith(".aspx")))
				sURL += ".aspx";

			return sURL;
		}

	}
}
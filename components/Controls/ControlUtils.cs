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
using System.Data;
using System.Text;


namespace DotNetNuke.Modules.ActiveForums
{
	public class ControlUtils
	{
		public string BuildPager(int tabId, int moduleId, string groupPrefix, string forumPrefix, int forumGroupId, int forumID, int tagId, int categoryId, string otherPrefix, int pageId, int pageCount)
		{
			if (pageCount == 1)
				return string.Empty;

			int iMaxPage;
			int iStart;

			if (pageId <= 3)
			{
				iStart = 1;
				iMaxPage = 5;
			}
			else
			{
				iStart = pageId - 2;
				iMaxPage = pageId + 2;
			}

			if (iMaxPage > pageCount)
				iMaxPage = pageCount;

			if (iMaxPage == pageCount)
				iStart = iMaxPage - 4;

			if (iStart <= 0)
				iStart = 1;

			var sb = new StringBuilder();
			sb.Append("<div class=\"af-pager\"><table><tr>");

			for (var i = iStart; i <= iMaxPage; i++)
			{
				if (i == pageId)
				    sb.AppendFormat("<td class=\"afpg-current\"><span>{0}</span></td>", i);
				else
                    sb.AppendFormat("<td class=\"afpg-page\"><a href=\"{0}\"><span>{1}</span></a></td>", BuildUrl(tabId, moduleId, groupPrefix, forumPrefix, forumGroupId, forumID, tagId, categoryId, otherPrefix, i, -1), i);

				if (i == pageCount)
					break;
			}

            sb.Append("</tr></table></div>");

			return sb.ToString();
		}

		public string BuildUrl(int tabId, int moduleId, string groupPrefix, string forumPrefix, int forumGroupId, int forumID, int tagId, int categoryId, string otherPrefix, int pageId, int socialGroupId)
		{
			return BuildUrl(tabId, moduleId, groupPrefix, forumPrefix, forumGroupId, forumID, -1, string.Empty, tagId, categoryId, otherPrefix, pageId, socialGroupId);
		}

		public string BuildUrl(int tabId, int moduleId, string groupPrefix, string forumPrefix, int forumGroupId, int forumID, int topicId, string topicURL, int tagId, int categoryId, string otherPrefix, int pageId, int socialGroupId)
		{
			var mainSettings = DataCache.MainSettings(moduleId);

		    var @params = new List<string>();

			if (! mainSettings.URLRewriteEnabled || (((string.IsNullOrEmpty(forumPrefix) && forumID > 0 && string.IsNullOrEmpty(groupPrefix)) || (string.IsNullOrEmpty(forumPrefix) && string.IsNullOrEmpty(groupPrefix) && forumGroupId > 0)) && string.IsNullOrEmpty(otherPrefix)))
			{
				if (forumID > 0 && topicId == -1)
					@params.Add(ParamKeys.ForumId + "=" + forumID);

				else if (forumGroupId > 0 && topicId == -1)
                    @params.Add(ParamKeys.GroupId + "=" + forumGroupId);

				else if (tagId > 0)
				{
					@params.Add("afv=grid");
					@params.Add("afgt=tags");
					@params.Add("aftg=" + tagId);
				}

				else if (categoryId > 0)
					@params.Add("act=" + categoryId);

				else if (! (string.IsNullOrEmpty(otherPrefix)))
				{
					@params.Add("afv=grid");
					@params.Add("afgt=" + otherPrefix);
				}

				else if (topicId > 0)
					@params.Add(ParamKeys.TopicId + "=" + topicId);

				if (pageId > 1)
					@params.Add(ParamKeys.PageId + "=" + pageId);

				if (socialGroupId > 0)

					@params.Add("GroupId=" + socialGroupId);

				return Utilities.NavigateUrl(tabId, string.Empty, @params.ToArray());
			}


		    var sURL = string.Empty;
		    if (! (string.IsNullOrEmpty(mainSettings.PrefixURLBase)))
		        sURL += "/" + mainSettings.PrefixURLBase;

		    if (! (string.IsNullOrEmpty(groupPrefix)))
		        sURL += "/" + groupPrefix;

		    if (! (string.IsNullOrEmpty(forumPrefix)))
		        sURL += "/" + forumPrefix;

		    if (! (string.IsNullOrEmpty(topicURL)))
		        sURL += "/" + topicURL;

		    if (tagId > 0)
		        sURL += "/" + mainSettings.PrefixURLTag + "/" + otherPrefix;

		    else if (categoryId > 0)
		        sURL += "/" + mainSettings.PrefixURLCategory + "/" + otherPrefix;

		    else if (! (string.IsNullOrEmpty(otherPrefix)) && (tagId == -1 || categoryId == -1))
		        sURL += "/" + mainSettings.PrefixURLOther + "/" + otherPrefix;

		    if (topicId > 0 && string.IsNullOrEmpty(topicURL))
		        return Utilities.NavigateUrl(tabId, string.Empty, ParamKeys.TopicId + "=" + topicId);

		    if (pageId > 1)
		    {
		        if (string.IsNullOrEmpty(sURL))
		            return Utilities.NavigateUrl(tabId, string.Empty, ParamKeys.PageId + "=" + pageId);

		        sURL += "/" + pageId.ToString();
		    }
		    if (string.IsNullOrEmpty(sURL))

		        return Utilities.NavigateUrl(tabId);

		    return sURL + "/";
		}

		public string TopicURL(IDataRecord row, int tabId, int moduleId, int pageId = 1)
		{
			var mainSettings = DataCache.MainSettings(moduleId);

			var sURL = string.Empty;

			if (!(string.IsNullOrEmpty(row["PrefixURL"].ToString())) && !(string.IsNullOrEmpty(row["URL"].ToString())) && mainSettings.URLRewriteEnabled)
			{
				if (! (string.IsNullOrWhiteSpace(mainSettings.PrefixURLBase)))
					sURL += "/" + mainSettings.PrefixURLBase;

				if (! (string.IsNullOrWhiteSpace(row["GroupPrefixURL"].ToString())))
					sURL += "/" + row["GroupPrefixURL"];

				sURL += "/" + row["PrefixURL"] + "/" + row["URL"] + "/";
				if (pageId > 1)
					sURL += "/" + pageId.ToString() + "/";
			}
			else
			{
				if (pageId == 1)
					sURL = Utilities.NavigateUrl(tabId, "", ParamKeys.TopicId + "=" + row["TopicId"]);

				else
					sURL = Utilities.NavigateUrl(tabId, "", new [] { ParamKeys.TopicId + "=" + row["TopicId"], ParamKeys.PageId + "=" + pageId });
			}
			return sURL;
		}

		public string ForumURL(IDataRecord row, int tabId, int moduleId, int pageId = 1)
		{
			return ForumURL(row["GroupPrefixURL"].ToString(), row["PrefixURL"].ToString(), int.Parse(row["ForumID"].ToString()), tabId, moduleId, pageId);
		}

		public string ForumURL(string groupPrefix, string forumPrefix, int forumId, int tabId, int moduleId, int pageId = 1)
		{
			var mainSettings = DataCache.MainSettings(moduleId);

			var sURL = string.Empty;

			if (!(string.IsNullOrWhiteSpace(forumPrefix)) && mainSettings.URLRewriteEnabled)
			{
                if (!(string.IsNullOrWhiteSpace(mainSettings.PrefixURLBase)))
					sURL += "/" + mainSettings.PrefixURLBase;

                if (!(string.IsNullOrWhiteSpace(groupPrefix)))
					sURL += "/" + groupPrefix;

				sURL += "/" + forumPrefix + "/";
				if (pageId > 1)
					sURL += "/" + pageId + "/";
			}
			else
			{
				if (pageId == 1)
					sURL = Utilities.NavigateUrl(tabId, string.Empty, ParamKeys.ForumId + "=" + forumId);
				else
                    sURL = Utilities.NavigateUrl(tabId, string.Empty, new[] { ParamKeys.ForumId + "=" + forumId, ParamKeys.PageId + "=" + pageId });
			}
			return sURL;
		}

		public string TopicState(IDataRecord row)
		{
			var states = string.Empty;

			if (Convert.ToBoolean(row["IsLocked"]))
				states += "<span class=\"af-locked\"></span>";

			if (Convert.ToBoolean(row["IsPinned"]))
				states += "<span class=\"af-pinned\"></span>";

			switch (int.Parse(row["StatusId"].ToString()))
			{
				case 0:
					states += "<span class=\"af-status0\"></span>";
					break;
				case 1:
					states += "<span class=\"af-status1\"></span>";
					break;
				case 3:
					states += "<span class=\"af-status3\"></span>";
					break;
			}

			return states;
		}

		public string Pager(int recordCount, int pageSize, int currentPage, int tabId)
		{
			return string.Empty;
		}
	}
}

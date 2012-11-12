using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ControlUtils
	{
		internal string BuildPager(int TabId, int ModuleId, string GroupPrefix, string ForumPrefix, int ForumGroupId, int ForumID, int TagId, int CategoryId, string OtherPrefix, int PageId, int PageCount)
		{
			if (PageCount == 1)
			{
				return string.Empty;
			}
			int iMaxPage = PageId + 2;
			if (iMaxPage > PageCount)
			{
				iMaxPage = PageCount;
			}
			int i = 1;
			int iStart = 1;
			if (PageId <= 3)
			{
				iStart = 1;
				iMaxPage = 5;
			}
			else
			{
				iStart = PageId - 2;
				iMaxPage = PageId + 2;
			}

			if (iMaxPage > PageCount)
			{
				iMaxPage = PageCount;
			}
			if (iMaxPage == PageCount)
			{
				iStart = iMaxPage - 4;
			}
			if (iStart <= 0)
			{
				iStart = 1;
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<div class=\"af-pager\"><table>");
			sb.Append("<tr>");
			//For x As Integer = 1 To PageCount
			for (i = iStart; i <= iMaxPage; i++)
			{

				if (i == PageId)
				{
					sb.Append("<td class=\"afpg-current\">");
					sb.Append("<span>");
					sb.Append(i);
					sb.Append("</span>");
					sb.Append("</td>");
				}
				else
				{
					sb.Append("<td class=\"afpg-page\">");
					sb.Append("<a href=\"" + BuildUrl(TabId, ModuleId, GroupPrefix, ForumPrefix, ForumGroupId, ForumID, TagId, CategoryId, OtherPrefix, i, -1) + "\"><span>");
					sb.Append(i);
					sb.Append("</span></a>");
					sb.Append("</td>");
				}

				if (i == PageCount)
				{
					break;
				}
			}
			sb.Append("</tr>");
			sb.Append("</table></div>");
			return sb.ToString();
		}
		internal string BuildUrl(int TabId, int ModuleId, string GroupPrefix, string ForumPrefix, int ForumGroupId, int ForumID, int TagId, int CategoryId, string OtherPrefix, int PageId, int SocialGroupId)
		{
			return BuildUrl(TabId, ModuleId, GroupPrefix, ForumPrefix, ForumGroupId, ForumID, -1, string.Empty, TagId, CategoryId, OtherPrefix, PageId, SocialGroupId);
		}
		internal string BuildUrl(int TabId, int ModuleId, string GroupPrefix, string ForumPrefix, int ForumGroupId, int ForumID, int TopicId, string TopicURL, int TagId, int CategoryId, string OtherPrefix, int PageId, int SocialGroupId)
		{
			SettingsInfo _mainSettings = DataCache.MainSettings(ModuleId);
			string[] @params = {};
			if (! _mainSettings.URLRewriteEnabled || (((string.IsNullOrEmpty(ForumPrefix) && ForumID > 0 && string.IsNullOrEmpty(GroupPrefix)) | (string.IsNullOrEmpty(ForumPrefix) && string.IsNullOrEmpty(GroupPrefix) && ForumGroupId > 0)) && string.IsNullOrEmpty(OtherPrefix)))
			{
				if (ForumID > 0 && TopicId == -1)
				{
					@params = Utilities.AddParams(ParamKeys.ForumId + "=" + ForumID.ToString(), @params);
				}
				else if (ForumGroupId > 0 && TopicId == -1)
				{
					@params = Utilities.AddParams(ParamKeys.GroupId + "=" + ForumGroupId.ToString(), @params);
				}
				else if (TagId > 0)
				{
					//afv=grid&afgt=tags&aftg=
					@params = Utilities.AddParams("afv=grid", @params);
					@params = Utilities.AddParams("afgt=tags", @params);
					@params = Utilities.AddParams("aftg=" + TagId.ToString(), @params);


				}
				else if (CategoryId > 0)
				{
					@params = Utilities.AddParams("act=" + CategoryId.ToString(), @params);
				}
				else if (! (string.IsNullOrEmpty(OtherPrefix)))
				{
					@params = Utilities.AddParams("afv=grid", @params);
					@params = Utilities.AddParams("afgt=" + OtherPrefix, @params);
				}
				else if (TopicId > 0)
				{
					@params = Utilities.AddParams(ParamKeys.TopicId + "=" + TopicId.ToString(), @params);
				}
				if (PageId > 1)
				{
					@params = Utilities.AddParams(ParamKeys.PageId + "=" + PageId, @params);
				}
				if (SocialGroupId > 0)
				{
					@params = Utilities.AddParams("GroupId=" + SocialGroupId, @params);
				}
				return Utilities.NavigateUrl(TabId, "", @params);
			}
			else
			{
				string sURL = string.Empty;
				if (! (string.IsNullOrEmpty(_mainSettings.PrefixURLBase)))
				{
					sURL += "/" + _mainSettings.PrefixURLBase;
				}
				if (! (string.IsNullOrEmpty(GroupPrefix)))
				{
					sURL += "/" + GroupPrefix;
				}
				if (! (string.IsNullOrEmpty(ForumPrefix)))
				{
					sURL += "/" + ForumPrefix;
				}
				if (! (string.IsNullOrEmpty(TopicURL)))
				{
					sURL += "/" + TopicURL;
				}
				if (TagId > 0)
				{
					sURL += "/" + _mainSettings.PrefixURLTag + "/" + OtherPrefix;
				}
				else if (CategoryId > 0)
				{
					sURL += "/" + _mainSettings.PrefixURLCategory + "/" + OtherPrefix;
				}
				else if (! (string.IsNullOrEmpty(OtherPrefix)) && (TagId == -1 || CategoryId == -1))
				{
					sURL += "/" + _mainSettings.PrefixURLOther + "/" + OtherPrefix;
				}
				if (TopicId > 0 && string.IsNullOrEmpty(TopicURL))
				{
					return Utilities.NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + TopicId.ToString());
				}
				if (PageId > 1)
				{
					if (string.IsNullOrEmpty(sURL))
					{
						return Utilities.NavigateUrl(TabId, "", ParamKeys.PageId + "=" + PageId);
					}
					sURL += "/" + PageId.ToString();
				}
				if (string.IsNullOrEmpty(sURL))
				{
					return Utilities.NavigateUrl(TabId);
				}
				return sURL + "/";
			}
		}
		internal string TopicURL(IDataRecord row, int TabId, int ModuleId, int PageId = 1)
		{
			SettingsInfo _mainSettings = DataCache.MainSettings(ModuleId);
			string sURL = string.Empty;
			if (! (string.IsNullOrEmpty(row["PrefixURL"].ToString())) && ! (string.IsNullOrEmpty(row["URL"].ToString())) && _mainSettings.URLRewriteEnabled)
			{
				if (! (string.IsNullOrEmpty(_mainSettings.PrefixURLBase)))
				{
					sURL += "/" + _mainSettings.PrefixURLBase;
				}
				if (! (string.IsNullOrEmpty(row["GroupPrefixURL"].ToString())))
				{
					sURL += "/" + row["GroupPrefixURL"].ToString();
				}
				sURL += "/" + row["PrefixURL"].ToString() + "/" + row["URL"].ToString() + "/";
				if (PageId > 1)
				{
					sURL += "/" + PageId.ToString() + "/";
				}
			}
			else
			{
				if (PageId == 1)
				{
					sURL = Utilities.NavigateUrl(TabId, "", new string[] {ParamKeys.TopicId + "=" + row["TopicId"].ToString()});
				}
				else
				{
					sURL = Utilities.NavigateUrl(TabId, "", new string[] {ParamKeys.TopicId + "=" + row["TopicId"].ToString(), ParamKeys.PageId + "=" + PageId.ToString()});
				}

			}
			return sURL;
		}
		internal string ForumURL(IDataRecord row, int TabId, int ModuleId, int PageId = 1)
		{
			return ForumURL(row["GroupPrefixURL"].ToString(), row["PrefixURL"].ToString(), int.Parse(row["ForumID"].ToString()), TabId, ModuleId, PageId);
		}
		internal string ForumURL(string GroupPrefix, string ForumPrefix, int ForumId, int TabId, int ModuleId, int PageId = 1)
		{
			SettingsInfo _mainSettings = DataCache.MainSettings(ModuleId);
			string sURL = string.Empty;

			if (! (string.IsNullOrEmpty(ForumPrefix)) && _mainSettings.URLRewriteEnabled)
			{
				if (! (string.IsNullOrEmpty(_mainSettings.PrefixURLBase)))
				{
					sURL += "/" + _mainSettings.PrefixURLBase;
				}
				if (! (string.IsNullOrEmpty(GroupPrefix)))
				{
					sURL += "/" + GroupPrefix;
				}
				sURL += "/" + ForumPrefix + "/";
				if (PageId > 1)
				{
					sURL += "/" + PageId.ToString() + "/";
				}
			}
			else
			{
				if (PageId == 1)
				{
					sURL = Utilities.NavigateUrl(TabId, "", new string[] {ParamKeys.ForumId + "=" + ForumId.ToString()});
				}
				else
				{
					sURL = Utilities.NavigateUrl(TabId, "", new string[] {ParamKeys.ForumId + "=" + ForumId.ToString(), ParamKeys.PageId + "=" + PageId.ToString()});
				}

			}
			return sURL;
		}
		internal string TopicState(IDataRecord row)
		{
			string states = string.Empty;
			if (Convert.ToBoolean(row["IsLocked"]))
			{
				states += "<span class=\"af-locked\"></span>";
			}
			if (Convert.ToBoolean(row["IsPinned"]))
			{
				states += "<span class=\"af-pinned\"></span>";
			}
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
		internal string Pager(int RecordCount, int PageSize, int CurrentPage, int TabId)
		{
			return string.Empty;
		}
	}
}

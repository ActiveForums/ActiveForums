//© 2004 - 2010 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Microsoft.ApplicationBlocks.Data;
namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class Common : DataConfig
	{
#region Security

		public void SavePermissionSet(int PermissionSetId, string PermissionSet)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, dbPrefix + "Permissions_Save", PermissionSetId, PermissionSet);
		}
		public IDataReader GetRoles(int SiteId)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "Permissions_GetRoles", SiteId);
		}
		public string GetPermSet(int PermissionsId, string Key)
		{
			string sSQL = "SELECT IsNULL(Can" + Key + ",'||||') from " + dbPrefix + "Permissions WHERE PermissionsId = " + PermissionsId;
			return Convert.ToString(SqlHelper.ExecuteScalar(_connectionString, CommandType.Text, sSQL));
		}
		public string SavePermSet(int PermissionsId, string Key, string PermSet)
		{
			string sSQL = "UPDATE " + dbPrefix + "Permissions SET Can" + Key + " = '" + PermSet + "' WHERE PermissionsId = " + PermissionsId;
			SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, sSQL);
			return GetPermSet(PermissionsId, Key);
		}
		public int CreatePermSet(string AdminRoleId)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "Permission_Create", AdminRoleId));
		}
		// KR - added caching
		public string CheckForumIdsForView(string ForumIds, string UserRoles)
		{
			string cacheKey = string.Format("AF-Perm-{0}", ForumIds);
			string sSQL = "SELECT f.ForumId, ISNULL(CanView,'') as CanView from " + dbPrefix + "Permissions as P INNER JOIN " + dbPrefix + "Forums as f on f.PermissionsID = P.PermissionsId  INNER JOIN " + dbPrefix + "Functions_Split('" + ForumIds + "',':') as ids on ids.id = f.ForumId";
			string sForums = string.Empty;
			DataTable dt = null;

			object data = DataCache.CacheRetrieve(cacheKey);

			if (data != null)
			{
				dt = (DataTable)data;
			}
			else
			{
				dt = DotNetNuke.Common.Globals.ConvertDataReaderToDataTable(SqlHelper.ExecuteReader(_connectionString, CommandType.Text, sSQL));
				DataCache.CacheStore(cacheKey, dt);
			}

			foreach (DataRow row in dt.Rows)
			{
				string canView = row["CanView"].ToString();
				if (Permissions.HasPerm(canView, UserRoles))
				{
					sForums += row["ForumId"].ToString() + ":";
				}
			}

			return sForums;
		}
		public bool SecurityUpgraded()
		{
			string sSQL = "SELECT Count(PermissionsId) FROM " + dbPrefix + "Permissions ";
			int secCount = Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, CommandType.Text, sSQL));
			if (secCount > 0)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

#endregion
#region Views
		public DataSet UI_ActiveView(int PortalId, int ModuleId, int UserId, int RowIndex, int MaxRows, string Sort, bool IsSuper, int TimeFrame, string ForumIds)
		{
			return (DataSet)(SqlHelper.ExecuteDataset(_connectionString, dbPrefix + "UI_ActiveView", PortalId, ModuleId, UserId, RowIndex, MaxRows, Sort, IsSuper, TimeFrame, ForumIds));
		}
		public DataSet UI_NotReadView(int PortalId, int ModuleId, int UserId, int RowIndex, int MaxRows, string Sort, string ForumIds)
		{
			return (DataSet)(SqlHelper.ExecuteDataset(_connectionString, dbPrefix + "UI_NotRead", PortalId, ModuleId, UserId, RowIndex, MaxRows, Sort, ForumIds));
		}
		public DataSet UI_UnansweredView(int PortalId, int ModuleId, int UserId, int RowIndex, int MaxRows, string Sort, string ForumIds)
		{
			return (DataSet)(SqlHelper.ExecuteDataset(_connectionString, dbPrefix + "UI_UnansweredView", PortalId, ModuleId, UserId, RowIndex, MaxRows, Sort, ForumIds));
		}
		public DataSet UI_TagsView(int PortalId, int ModuleId, int UserId, int RowIndex, int MaxRows, string Sort, string ForumIds, int TagId)
		{
			return (DataSet)(SqlHelper.ExecuteDataset(_connectionString, dbPrefix + "UI_TagsView", PortalId, ModuleId, UserId, RowIndex, MaxRows, Sort, ForumIds, TagId));
		}
#endregion
#region TagCloud
		public IDataReader TagCloud_Get(int PortalId, int ModuleId, string ForumIds, int Rows)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "UI_TagCloud", PortalId, ModuleId, ForumIds, Rows);
		}
#endregion
#region Tags
		public int Tag_GetIdByName(int PortalId, int ModuleId, string TagName, bool IsCategory)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "Tags_GetByName", PortalId, ModuleId, TagName.Replace("-", " ").ToLowerInvariant(), IsCategory));
		}
#endregion
#region TopMembers
		public IDataReader TopMembers_Get(int PortalId, int Rows)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "UI_TopMembers", PortalId, Rows);
		}
#endregion
#region CustomURLS
		public Dictionary<string, string> GetPrefixes(int PortalId)
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			using (IDataReader dr = SqlHelper.ExecuteReader(_connectionString, dbPrefix + "Forums_GetPrefixes", PortalId))
			{
				while (dr.Read())
				{
					string prefix = dr["PrefixURL"].ToString();
					string tabid = dr["TabId"].ToString();
					string forumid = dr["ForumId"].ToString();
					string moduleId = dr["ModuleId"].ToString();
					string archived = dr["Archived"].ToString();
					string forumgroupId = dr["ForumGroupId"].ToString();
					string groupPrefix = dr["GroupPrefixURL"].ToString();
					if (! (string.IsNullOrEmpty(groupPrefix)))
					{
						prefix = groupPrefix + "/" + prefix;
					}
					dict.Add(prefix, tabid + "|" + forumid + "|" + moduleId + "|" + archived + "|" + forumgroupId + "|" + groupPrefix);
				}
				dr.Close();
			}
			return dict;
		}
		public string GetUrl(int ModuleId, int ForumGroupId, int ForumId, int TopicId, int UserId, int ContentId)
		{
			try
			{
				return Convert.ToString(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "Util_GetUrl", ModuleId, ForumGroupId, ForumId, TopicId, UserId, ContentId));
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}
		public IDataReader FindByURL(int PortalId, string URL)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "FindByURL", PortalId, URL);
		}
		public IDataReader URLSearch(int PortalId, string URL)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "URL_Search", PortalId, URL);
		}
		public void ArchiveURL(int PortalId, int ForumGroupId, int ForumId, int TopicId, string URL)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, dbPrefix + "URL_Archive", PortalId, ForumGroupId, ForumId, TopicId, URL);
		}
		public bool CheckForumURL(int PortalId, int ModuleId, string VanityName, int ForumId, int ForumGroupId)
		{
			try
			{
				SettingsInfo _mainSettings = DataCache.MainSettings(ModuleId);
				ForumGroupController fgc = new ForumGroupController();
				ForumGroupInfo fg = fgc.GetForumGroup(ModuleId, ForumGroupId);
				if (! (string.IsNullOrEmpty(fg.PrefixURL)))
				{
					VanityName = fg.PrefixURL + "/" + VanityName;
				}
				if (! (string.IsNullOrEmpty(_mainSettings.PrefixURLBase)))
				{
					VanityName = _mainSettings.PrefixURLBase + "/" + VanityName;
				}
				int tmpForumId = -1;
				tmpForumId = Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "URL_CheckForumVanity", PortalId, VanityName));
				if (tmpForumId > 0 && ForumId == -1)
				{
					return false;
				}
				else if (tmpForumId == ForumId && ForumId > 0)
				{
					return true;
				}
				else if (tmpForumId <= 0)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;

			}

            return false;
		}
		public bool CheckGroupURL(int PortalId, int ModuleId, string VanityName, int ForumGroupId)
		{
			try
			{
				SettingsInfo _mainSettings = DataCache.MainSettings(ModuleId);
				if (! (string.IsNullOrEmpty(_mainSettings.PrefixURLBase)))
				{
					VanityName = _mainSettings.PrefixURLBase + "/" + VanityName;
				}
				int tmpForumGroupId = -1;
				tmpForumGroupId = Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "URL_CheckGroupVanity", PortalId, VanityName));
				if (tmpForumGroupId > 0 && ForumGroupId == -1)
				{
					return false;
				}
				else if (tmpForumGroupId == ForumGroupId && ForumGroupId > 0)
				{
					return true;
				}
				else if (tmpForumGroupId <= 0)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;

			}

            return false;
		}

#endregion
	}
}

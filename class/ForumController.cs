//© 2004 - 2010 ActiveModules, Inc. All Rights Reserved
using System;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Modules.ActiveForums
{

	public class ForumController
	{
		internal string GetForumsForUser(string UserRoles, int PortalId, int ModuleId, string PermissionType = "CanView")
		{
			var db = new Data.ForumsDB();
			string forumIds = string.Empty;
			ForumCollection fc = db.Forums_List(PortalId, ModuleId);
			foreach (Forum f in fc)
			{
				string roles;
				switch (PermissionType)
				{
					case "CanView":
						roles = f.Security.View;
						break;
					case "CanRead":
						roles = f.Security.Read;
						break;
					case "CanApprove":
						roles = f.Security.ModApprove;
						break;
					case "CanEdit":
						roles = f.Security.ModEdit;
						break;
					default:
						roles = f.Security.View;
						break;
				}
				bool canView = Permissions.HasPerm(roles, UserRoles);
				if ((canView || (f.Hidden == false && (PermissionType == "CanView" || PermissionType == "CanRead"))) && f.Active)
				{
					forumIds += f.ForumID + ";";
				}
			}
			return forumIds;
		}
		public Forum GetForum(int PortalId, int ModuleId, int ForumId)
		{
			return GetForum(PortalId, ModuleId, ForumId, false);
		}
		public Forum GetForum(int PortalId, int ModuleId, int ForumId, bool IgnoreCache)
		{
			string cachekey = string.Format("AF-FI-{0}-{1}-{2}", PortalId, ModuleId, ForumId);
			var data = DataCache.CacheRetrieve(cachekey);
			if (data == null || IgnoreCache)
			{
				var db = new Data.ForumsDB();
				Forum fi = null;
				using (IDataReader dr = db.Forums_Get(PortalId, ModuleId, ForumId))
				{
					while (dr.Read())
					{
						fi = FillForum(dr);
					}
					dr.Close();
				}
				if (fi != null)
				{
					if (fi.HasProperties)
					{
						var propC = new PropertiesController();
						fi.Properties = propC.ListProperties(PortalId, 1, ForumId);
					}

				}
				DataCache.CacheStore(cachekey, fi);
				return fi;
			}
		    return (Forum)data;
		}
		private Forum FillForum(IDataRecord dr)
		{
			var fi = new Forum
			             {
			                 ForumGroup = new ForumGroupInfo(),
			                 ForumID = Convert.ToInt32(dr["ForumId"].ToString()),
			                 Active = Convert.ToBoolean(dr["Active"]),
			                 ModuleId = Convert.ToInt32(dr["ModuleId"].ToString()),
			                 ForumGroupId = Convert.ToInt32(dr["ForumGroupId"].ToString()),
			                 ParentForumId = Convert.ToInt32(dr["ParentForumId"].ToString()),
			                 ForumName = dr["ForumName"].ToString(),
			                 ForumDesc = dr["ForumDesc"].ToString(),
			                 SortOrder = Convert.ToInt32(dr["SortOrder"].ToString()),
			                 Hidden = Convert.ToBoolean(dr["Hidden"]),
			                 TotalTopics = Convert.ToInt32(dr["TotalTopics"].ToString()),
			                 TotalReplies = Convert.ToInt32(dr["TotalReplies"].ToString()),
			                 LastTopicId = Convert.ToInt32(dr["LastTopicId"].ToString()),
			                 LastReplyId = Convert.ToInt32(dr["LastReplyId"].ToString()),
			                 GroupName = dr["GroupName"].ToString(),
			                 PermissionsId = Convert.ToInt32(dr["PermissionsId"].ToString()),
			                 ForumSettingsKey = dr["ForumSettingsKey"].ToString(),
			                 InheritSecurity = Convert.ToBoolean(dr["InheritSecurity"]),
			                 PrefixURL = dr["PrefixURL"].ToString()
			             };

		    fi.ForumGroup.ForumGroupId = fi.ForumGroupId;
			fi.ForumGroup.GroupName = fi.GroupName;
			fi.ForumGroup.PrefixURL = dr["GroupPrefixURL"].ToString();
			fi.SocialGroupId = Convert.ToInt32(dr["SocialGroupId"].ToString());
			fi.HasProperties = Convert.ToBoolean(dr["HasProperties"]);
			try
			{
				fi.AllowRSS = Convert.ToBoolean(dr["AllowRSS"]);
			}
			catch (Exception ex)
			{
				fi.AllowRSS = false;
			}
			fi.Security.Announce = dr["CanAnnounce"].ToString();
			fi.Security.Attach = dr["CanAttach"].ToString();
			fi.Security.Create = dr["CanCreate"].ToString();
			fi.Security.Delete = dr["CanDelete"].ToString();
			fi.Security.Edit = dr["CanEdit"].ToString();
			fi.Security.Lock = dr["CanLock"].ToString();
			fi.Security.ModApprove = dr["CanModApprove"].ToString();
			fi.Security.ModDelete = dr["CanModDelete"].ToString();
			fi.Security.ModEdit = dr["CanModEdit"].ToString();
			fi.Security.ModLock = dr["CanModLock"].ToString();
			fi.Security.ModMove = dr["CanModMove"].ToString();
			fi.Security.ModPin = dr["CanModPin"].ToString();
			fi.Security.ModSplit = dr["CanModSplit"].ToString();
			fi.Security.ModUser = dr["CanModUser"].ToString();
			fi.Security.Pin = dr["CanPin"].ToString();
			fi.Security.Poll = dr["CanPoll"].ToString();
			fi.Security.Block = dr["CanBlock"].ToString();
			fi.Security.Read = dr["CanRead"].ToString();
			fi.Security.Reply = dr["CanReply"].ToString();
			fi.Security.Subscribe = dr["CanSubscribe"].ToString();
			fi.Security.Trust = dr["CanTrust"].ToString();
			fi.Security.View = dr["CanView"].ToString();
			fi.Security.Tag = dr["CanTag"].ToString();
			fi.Security.Prioritize = dr["CanPrioritize"].ToString();
			fi.Security.Categorize = dr["CanCategorize"].ToString();

			return fi;
		}
		public string GetForumIdsBySocialGroup(int PortalId, int SocialGroupId)
		{
			var db = new Data.ForumsDB();
			string forumIds = string.Empty;
			using (IDataReader dr = db.Forums_GetForSocialGroup(PortalId, SocialGroupId))
			{
				while (dr.Read())
				{
					forumIds += dr["ForumId"] + ";";
				}
				dr.Close();
			}
			return forumIds;
		}
		internal int Forums_GetGroupId(int ForumId)
		{
			Forum fi = Forums_Get(ForumId, -1, false);
			return fi.ForumGroupId;
		}
		internal Forum Forums_Get(int ForumId, int UserId, bool WithSecurity, bool UseCache)
		{
			return Forums_Get(-1, -1, ForumId, UserId, WithSecurity, UseCache, -1);
		}
		internal Forum Forums_Get(int ForumId, int UserId, bool WithSecurity)
		{
			return Forums_Get(-1, -1, ForumId, UserId, WithSecurity, false, -1);
		}
		internal Forum Forums_Get(int PortalId, int ModuleId, int ForumID, int UserId, bool WithSecurity, bool UseCache, int TopicId)
		{
			Forum fi = null;
			if (ForumID <= 0 && TopicId <= 0)
			{
				return null;
			}
		    if (TopicId > 0 & ForumID <= 0)
		    {
		        var fdb = new Data.ForumsDB();
		        ForumID = fdb.Forum_GetByTopicId(TopicId);
		    }
		    if (ForumID <= 0)
			{
				return null;
			}
			// KR - I reworked this, the cache wasn't ever being used and I added caching for a specific user as well
			if (UserId == -1 && UseCache)
			{
				//Try Cache First
				object obj = DataCache.CacheRetrieve(string.Format(CacheKeys.ForumInfo, ForumID));
				if (obj != null)
				{
					fi = (Forum)obj;
				}
			}
			else if (UserId > -1 && UseCache)
			{
				//Try Cache First
				object obj = DataCache.CacheRetrieve(string.Format(CacheKeys.ForumInfoWithUser, ForumID, UserId));
				if (obj != null)
				{
					fi = (Forum)obj;
				}
			}
			if (fi == null)
			{
			    fi = GetForum(PortalId, ModuleId, ForumID, !UseCache);
			}
		    if (fi != null)
			{
				fi.ForumSettings = DataCache.GetSettings(fi.ModuleId, fi.ForumSettingsKey, string.Format(CacheKeys.ForumInfo, ForumID), UseCache);
			}
			if (UserId == -1 && fi != null)
			{
				DataCache.CacheStore(string.Format(CacheKeys.ForumInfo, ForumID), fi);
			}
			return fi;
		}
		// KR - added All settings cache
		private DataTable GetAllSettings(int ModuleId)
		{
			DataTable tempGetAllSettings;

			object data = DataCache.CacheRetrieve(string.Format(CacheKeys.AllSettings, ModuleId));

			if (data != null)
			{
				tempGetAllSettings = (DataTable)data;
			}
			else
			{
                tempGetAllSettings = Common.Globals.ConvertDataReaderToDataTable(DataProvider.Instance().Settings_ListAll(ModuleId));
				DataCache.CacheStore(string.Format(CacheKeys.AllSettings, ModuleId), tempGetAllSettings);
			}

			return tempGetAllSettings;
		}
		public int Forums_Save(int PortalId, Forum fi, bool isNew, bool UseGroup)
		{
			var rc = new RoleController();
			RoleInfo ri;
			var db = new Data.Common();
			int permissionsId = -1;
			if (UseGroup && (string.IsNullOrEmpty(fi.ForumSettingsKey) || fi.PermissionsId == -1))
			{
				var fc = new ForumGroupController();
				ForumGroupInfo fg = fc.Groups_Get(fi.ModuleId, fi.ForumGroupId);
				if (fg != null)
				{
					fi.ForumSettingsKey = fg.GroupSettingsKey;
					//fi.ModuleId = fg.ModuleId
					fi.PermissionsId = fg.PermissionsId;
				}
			}
			else if (fi.PermissionsId <= 0 && UseGroup == false)
			{
				ri = rc.GetRoleByName(PortalId, "Administrators");
				if (ri != null)
				{
					fi.PermissionsId = db.CreatePermSet(ri.RoleID.ToString());
					permissionsId = fi.PermissionsId;
					isNew = true;
				}
				if (fi.ForumID > 0 & ! (string.IsNullOrEmpty(fi.ForumSettingsKey)))
				{
					if (fi.ForumSettingsKey.Contains("G:"))
					{
						fi.ForumSettingsKey = string.Empty;
					}
				}
				if (fi.ForumSettingsKey == "" && fi.ForumID > 0)
				{
					fi.ForumSettingsKey = "F:" + fi.ForumID;
				}
			}
			else if (UseGroup == false && string.IsNullOrEmpty(fi.ForumSettingsKey) && fi.ForumID > 0)
			{
				fi.ForumSettingsKey = "F:" + fi.ForumID;

			}

			int forumId = Convert.ToInt32(DataProvider.Instance().Forum_Save(PortalId, fi.ForumID, fi.ModuleId, fi.ForumGroupId, fi.ParentForumId, fi.ForumName, fi.ForumDesc, fi.SortOrder, fi.Active, fi.Hidden, fi.ForumSettingsKey, fi.PermissionsId, fi.PrefixURL, fi.SocialGroupId, fi.HasProperties));
			if (String.IsNullOrEmpty(fi.ForumSettingsKey))
			{
				fi.ForumSettingsKey = "F:" + forumId;
			}
			if (fi.ForumSettingsKey.Contains("G:"))
			{
				DataProvider.Instance().Forum_ConfigCleanUp(fi.ModuleId, "F:" + fi.ForumID, "F:" + fi.ForumID);
			}
			if (isNew && UseGroup == false)
			{
				int moduleId = fi.ModuleId;
				Permissions.CreateDefaultSets(PortalId, permissionsId);

				string sKey = "F:" + forumId.ToString();
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.TopicsTemplateId, "0");
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.TopicTemplateId, "0");
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.TopicFormId, "0");
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.ReplyFormId, "0");
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.AllowRSS, "false");
			}
			DataCache.CacheClear(string.Format(CacheKeys.ForumList, fi.ModuleId));
			string cachekey = string.Format("AF-FI-{0}-{1}-{2}", PortalId, fi.ModuleId, forumId);
			object data = DataCache.CacheClear(cachekey);
			return forumId;
		}

		public string GetForumsHtmlOption(int PortalId, int ModuleId, User currentUser)
		{
			string userForums = GetForumsForUser(currentUser.UserRoles, PortalId, ModuleId, "CanView");
			DataTable dt = DataProvider.Instance().UI_ForumView(PortalId, ModuleId, currentUser.UserId, currentUser.IsSuperUser, userForums).Tables[0];
			int i = 0;
			int n = 1;
			int tmpGroupCount = 0;
			int tmpForumCount = 0;
			string tmpGroupKey = string.Empty;
			string tmpForumKey = string.Empty;
			var sb = new StringBuilder();
			foreach (DataRow dr in dt.Rows)
			{
				bool bView = Permissions.HasPerm(dr["CanView"].ToString(), currentUser.UserRoles);
				string GroupName = Convert.ToString(dr["GroupName"]);
				int GroupId = Convert.ToInt32(dr["ForumGroupId"]);
				string GroupKey = GroupName + GroupId.ToString();
				string ForumName = Convert.ToString(dr["ForumName"]);
				int ForumId = Convert.ToInt32(dr["ForumId"]);
				string ForumKey = ForumName + ForumId.ToString();
				int ParentForumId = Convert.ToInt32(dr["ParentForumId"]);
				//TODO - Need to add support for Group Permissions and GroupHidden

				if (tmpGroupKey != GroupKey)
				{
					sb.AppendFormat("<option value=\"{0}\">{1}</option>", "-1", GroupName);
					n += 1;
					tmpGroupKey = GroupKey;
				}
				if (bView)
				{
					if (ParentForumId == 0)
					{
						sb.AppendFormat("<option value=\"{0}\">{1}</option>", dr["ForumID"], "--" + dr["ForumName"]);
						n += 1;
						sb.Append(GetSubForums(n, Convert.ToInt32(dr["ForumId"]), dt, ref n));
					}

				}
			}

			return sb.ToString();
		}
		private string GetSubForums(int ItemCount, int ParentForumId, DataTable dtForums, ref int n)
		{
			var sb = new StringBuilder();
			dtForums.DefaultView.RowFilter = "ParentForumId = " + ParentForumId;
			if (dtForums.DefaultView.Count > 0)
			{
				foreach (DataRow dr in dtForums.DefaultView.ToTable().Rows)
				{
					sb.AppendFormat("<option value=\"{0}\">----{1}</option>", dr["ForumID"], dr["ForumName"]);
					ItemCount += 1;
				}
			}
			n = ItemCount;
			return sb.ToString();
		}

		public DataTable GetForumView(int PortalId, int ModuleId, int CurrentUserId, bool IsSuperUser, string ForumIds)
		{
			DataSet ds;
			DataTable dt;
			string cachekey = string.Format("AF-FV-{0}-{1}-{2}-{3}", PortalId, ModuleId, CurrentUserId, ForumIds);

			object data = DataCache.CacheRetrieve(cachekey);

			// cached datatable is held as an XML string (because data vanishes if just caching the DT in this instance)
			if (data != null)
			{
				var sr = new StringReader(data.ToString());
				ds = new DataSet();
				ds.ReadXml(sr);
				dt = ds.Tables[0];
			}
			else
			{
				ds = DataProvider.Instance().UI_ForumView(PortalId, ModuleId, CurrentUserId, IsSuperUser, ForumIds);
				dt = ds.Tables[0];

				string result;
				var sw = new StringWriter();

				dt.WriteXml(sw);
				result = sw.ToString();

				sw.Close();
				sw.Dispose();

				DataCache.CacheStore(cachekey, result);
			}

			return dt;
		}
		public int CreateGroupForum(int PortalId, int ModuleId, int SocialGroupId, int ForumGroupId, string ForumName, string ForumDescription, bool IsPrivate, string ForumConfig)
		{
			int forumId = -1;

			try
			{
				var rc = new RoleController();
				var forumsDb = new Data.Common();
				ForumGroupInfo gi;
				var fgc = new ForumGroupController();
				gi = fgc.Groups_Get(ModuleId, ForumGroupId);
				RoleInfo socialGroup = rc.GetRole(SocialGroupId, PortalId);
				string groupAdmin = SocialGroupId.ToString() + ":0";
				string groupMember = SocialGroupId.ToString();

				int permissionsId;
                
				RoleInfo ri;
				ri = rc.GetRoleByName(PortalId, "Administrators");
				permissionsId = forumsDb.CreatePermSet(ri.RoleID.ToString());

				ModuleId = gi.ModuleId;

				var fi = new Forum
				             {
				                 ForumDesc = ForumDescription,
				                 Active = true,
				                 ForumGroupId = ForumGroupId,
				                 ForumID = -1,
				                 ForumName = ForumName,
				                 Hidden = IsPrivate,
				                 ModuleId = gi.ModuleId,
				                 ParentForumId = 0,
				                 PortalId = PortalId,
				                 PermissionsId = gi.PermissionsId,
				                 SortOrder = 0,
				                 SocialGroupId = SocialGroupId
				             };
			    forumId = Forums_Save(PortalId, fi, true, true);
				fi = GetForum(PortalId, ModuleId, forumId);
				fi.PermissionsId = permissionsId;
				fi.AllowRSS = true;
				Forums_Save(PortalId, fi, false, false);

				string permSet;
				string secKey;
				var xDoc = new XmlDocument();
				xDoc.LoadXml(ForumConfig);
				if (xDoc != null)
				{
					XmlNode xRoot = xDoc.DocumentElement;
					XmlNode xSecList = xRoot.SelectSingleNode("//security[@type='groupadmin']");
					if (xSecList != null)
					{
						foreach (XmlNode n in xSecList.ChildNodes)
						{
							secKey = n.Name;
							if (n.Attributes["value"].Value == "true")
							{
								permSet = forumsDb.GetPermSet(permissionsId, secKey);
								permSet = Permissions.AddPermToSet(groupAdmin, 2, permSet);
								forumsDb.SavePermSet(permissionsId, secKey, permSet);
								permSet = string.Empty;
							}

						}
					}
					xSecList = xRoot.SelectSingleNode("//security[@type='groupmember']");
					if (xSecList != null)
					{
						foreach (XmlNode n in xSecList.ChildNodes)
						{
							secKey = n.Name;
							if (n.Attributes["value"].Value == "true")
							{
								permSet = forumsDb.GetPermSet(permissionsId, secKey);
								permSet = Permissions.AddPermToSet(groupMember, 0, permSet);
								forumsDb.SavePermSet(permissionsId, secKey, permSet);
								permSet = string.Empty;
							}

						}
					}
					if (socialGroup.IsPublic)
					{
						xSecList = xRoot.SelectSingleNode("//security[@type='registereduser']");
						ri = rc.GetRoleByName(PortalId, "Registered Users");
						if (xSecList != null)
						{
							foreach (XmlNode n in xSecList.ChildNodes)
							{
								secKey = n.Name;
								if (n.Attributes["value"].Value == "true")
								{
									permSet = forumsDb.GetPermSet(permissionsId, secKey);
									permSet = Permissions.AddPermToSet(ri.RoleID.ToString(), 0, permSet);
									forumsDb.SavePermSet(permissionsId, secKey, permSet);
									permSet = string.Empty;
								}

							}
						}
						xSecList = xRoot.SelectSingleNode("//security[@type='anon']");
						if (xSecList != null)
						{
							foreach (XmlNode n in xSecList.ChildNodes)
							{
								secKey = n.Name;
								if (n.Attributes["value"].Value == "true")
								{
									permSet = forumsDb.GetPermSet(permissionsId, secKey);
									permSet = Permissions.AddPermToSet("-1", 0, permSet);
									forumsDb.SavePermSet(permissionsId, secKey, permSet);
									permSet = string.Empty;
								}

							}
						}
					}
				}

			}
			catch (Exception ex)
			{

			}
			DataCache.CacheClear(ModuleId + "fv");

			return forumId;
		}
	}
}
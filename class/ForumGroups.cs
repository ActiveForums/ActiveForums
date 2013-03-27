//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Data;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Modules.ActiveForums
{

	public class ForumGroupInfo
	{
		public ForumGroupInfo()
		{
			Security = new PermissionInfo();
		    PermissionsId = -1;
		    PrefixURL = string.Empty;
            GroupSettings = new Hashtable();
		}

	    public int ForumGroupId { get; set; }

	    public int ModuleId { get; set; }

	    public string GroupName { get; set; }

	    public int UserID { get; set; }

	    public int SortOrder { get; set; }

	    public int PortalID { get; set; }

	    public bool Hidden { get; set; }

	    public bool Active { get; set; }

	    public string GroupSettingsKey { get; set; }

        public int PermissionsId { get; set; }

        public string PrefixURL { get; set; }

        #region Settings & Security

        public PermissionInfo Security { get; set; }

        public Hashtable GroupSettings { get; set; }

		public bool AllowAttach
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AllowAttach]); }
		}

		public bool AllowEmoticons
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AllowEmoticons]); }
		}

		public bool AllowHTML
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AllowHTML]); }
		}

		public bool AllowPostIcon
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AllowPostIcon]); }
		}

		public bool AllowRSS
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AllowRSS]); }
		}

        // TODO: Eliminate this
		public bool ActiveSocialEnabled
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.ActiveSocialEnabled]); }
		}

		public bool ActiveSocialTopicsOnly
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.ActiveSocialTopicsOnly]); }
		}

		public int ActiveSocialSecurityOption
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ActiveSocialSecurityOption], 1); }
		}

		public bool AllowScript
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AllowScript]); }
		}

		public bool AllowSubscribe
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AllowSubscribe]); }
		}

		public int AttachCount
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.AttachCount], 3); }
		}

		public int AttachMaxHeight
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.AttachMaxHeight], 500); }
		}

		public int AttachMaxSize
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.AttachMaxSize], 1000); }
		}

		public int AttachMaxWidth
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.AttachMaxWidth], 500); }
		}

		public AttachStores AttachStore
		{
			get
			{
			    AttachStores parsedValue;
			    return Enum.TryParse(Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.AttachStore], "0"), true,
			                         out parsedValue)
			               ? parsedValue
                           : AttachStores.FILESYSTEM;
			}
		}

		public string AttachTypeAllowed
		{
			get { return Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.AttachTypeAllowed], ".jpg,.gif,.png"); }
		}

		public bool AttachUniqueFileNames
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AttachUniqueFileNames]); }
		}

		public string EditorHeight
		{
			get { return Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EditorHeight], "400"); }
		}

		public int EditorStyle
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.EditorStyle], 1); }
		}

		public HTMLPermittedUsers EditorPermittedUsers
		{
			get
			{
			    HTMLPermittedUsers parseValue;
			    return Enum.TryParse(Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EditorPermittedUsers], "1"), true, out parseValue)
			               ? parseValue
			               : HTMLPermittedUsers.AuthenticatedUsers;
			}
		}

		public string EditorToolBar
		{
			get { return Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EditorToolbar], "bold,italic,underline"); }
		}

		public EditorTypes EditorType
		{
			get
			{
                EditorTypes parseValue;
                return Enum.TryParse(Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EditorPermittedUsers], "0"), true, out parseValue)
                           ? parseValue
                           : EditorTypes.TEXTBOX;
			}
		}

		public string EditorWidth
		{
			get { return Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EditorWidth], "100%"); }
		}

		public string EmailAddress
		{
			get { return Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EmailAddress], string.Empty); }
		}

		public bool IndexContent
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.IndexContent]); }
		}

		public bool AutoSubscribeEnabled
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AutoSubscribeEnabled]); }
		}

		public string AutoSubscribeRoles
		{
			get { return Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.AutoSubscribeRoles], string.Empty); }
		}

		public bool AutoSubscribeNewTopicsOnly
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AutoSubscribeNewTopicsOnly]); }
		}

		public bool IsModerated
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.IsModerated]); }
		}

		public int TopicsTemplateId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.TopicsTemplateId]); }
		}

		public int TopicTemplateId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.TopicTemplateId]); }
		}

		public int TopicFormId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.TopicFormId]); }
		}

		public int ReplyFormId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ReplyFormId]); }
		}

		public int QuickReplyFormId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.QuickReplyFormId]); }
		}

		public int ProfileTemplateId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ProfileTemplateId]); }
		}

		public bool UseFilter
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.UseFilter]); }
		}

		public int AutoTrustLevel
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.AutoTrustLevel]); }
		}

        public TrustTypes DefaultTrustValue
        {
            get
            {
                TrustTypes parseValue;
                return Enum.TryParse(Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.DefaultTrustValue], "0"), true, out parseValue)
                           ? parseValue
                           : TrustTypes.NotTrusted;
            }
        }

		public int ModApproveTemplateId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ModApproveTemplateId]); }
		}

		public int ModRejectTemplateId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ModRejectTemplateId]); }
		}

		public int ModMoveTemplateId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ModMoveTemplateId]); }
		}

		public int ModDeleteTemplateId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ModDeleteTemplateId]); }
		}

		public int ModNotifyTemplateId
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ModNotifyTemplateId]); }
		}

        public int CreatePostCount // Minimum posts required to create a topic in this forum if the user is not trusted
        {
            get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.CreatePostCount]); }
        }

        public int ReplyPostCount // Minimum posts required to reply to a topic in this forum if the user is not trusted
        {
            get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.ReplyPostCount]); }
        }

        #endregion

	}

	public class ForumGroupController
	{
		public void Group_Delete(int ModuleId, int ForumGroupId)
		{
			DataProvider.Instance().Groups_Delete(ModuleId, ForumGroupId);
		}
		public ForumGroupInfo GetForumGroup(int ModuleId, int ForumGroupid)
		{
			var db = new Data.Groups();
			ForumGroupInfo gi = null;
			using (IDataReader dr = db.Groups_Get(ModuleId, ForumGroupid))
			{
				while (dr.Read())
				{
					gi = FillForumGroup(dr);
				}
				dr.Close();
			}
			return gi;
		}
		private ForumGroupInfo FillForumGroup(IDataRecord dr)
		{
			var g = new ForumGroupInfo
			            {
			                Active = Convert.ToBoolean(dr["Active"]),
			                ForumGroupId = Convert.ToInt32(dr["ForumGroupId"].ToString()),
			                Hidden = Convert.ToBoolean(dr["Hidden"]),
			                GroupName = dr["GroupName"].ToString(),
			                SortOrder = Convert.ToInt32(dr["SortOrder"].ToString()),
			                GroupSettingsKey = dr["GroupSettingsKey"].ToString(),
			                PermissionsId = Convert.ToInt32(dr["PermissionsId"].ToString()),
			                PrefixURL = dr["PrefixURL"].ToString(),
			                ModuleId = Convert.ToInt32(dr["ModuleId"].ToString()),
			                Security =
			                    {
			                        Announce = dr["CanAnnounce"].ToString(),
			                        Attach = dr["CanAttach"].ToString(),
			                        Create = dr["CanCreate"].ToString(),
			                        Delete = dr["CanDelete"].ToString(),
			                        Edit = dr["CanEdit"].ToString(),
			                        Lock = dr["CanLock"].ToString(),
			                        ModApprove = dr["CanModApprove"].ToString(),
			                        ModDelete = dr["CanModDelete"].ToString(),
			                        ModEdit = dr["CanModEdit"].ToString(),
			                        ModLock = dr["CanModLock"].ToString(),
			                        ModMove = dr["CanModMove"].ToString(),
			                        ModPin = dr["CanModPin"].ToString(),
			                        ModSplit = dr["CanModSplit"].ToString(),
			                        ModUser = dr["CanModUser"].ToString(),
			                        Pin = dr["CanPin"].ToString(),
			                        Poll = dr["CanPoll"].ToString(),
			                        Block = dr["CanBlock"].ToString(),
			                        Read = dr["CanRead"].ToString(),
			                        Reply = dr["CanReply"].ToString(),
			                        Subscribe = dr["CanSubscribe"].ToString(),
			                        Trust = dr["CanTrust"].ToString(),
			                        View = dr["CanView"].ToString(),
			                        Tag = dr["CanTag"].ToString(),
			                        Prioritize = dr["CanPrioritize"].ToString(),
			                        Categorize = dr["CanCategorize"].ToString()
			                    }
			            };

		    return g;
		}
		public ForumGroupInfo Groups_Get(int ModuleID, int ForumGroupID)
		{
			ForumGroupInfo gi = GetForumGroup(ModuleID, ForumGroupID);
			//gi.GroupSecurity = DataCache.GetSecurity(ModuleID, gi.ForumGroupId, SecureType.ForumGroup)
			gi.GroupSettings = DataCache.GetSettings(ModuleID, gi.GroupSettingsKey, string.Format(CacheKeys.GroupInfo, ForumGroupID), false);
			return gi;
		}
		public ArrayList Groups_List(int ModuleId, bool FillSettings = false, bool FillSecurity = false)
		{
			ArrayList groupArr = CBO.FillCollection(DataProvider.Instance().Groups_List(ModuleId), typeof(ForumGroupInfo));
			if (FillSettings == false || FillSecurity == false)
			{
				return groupArr;
			}
			ForumGroupInfo gi;
			int i;
			for (i = 0; i < groupArr.Count; i++)
			{
				gi = (ForumGroupInfo)(groupArr[i]);
				//If FillSecurity Then
				//    gi.GroupSecurity = DataCache.GetSecurity(ModuleId, gi.ForumGroupId, SecureType.ForumGroup)
				//End If
				if (FillSettings)
				{
					gi.GroupSettings = DataCache.GetSettings(ModuleId, gi.GroupSettingsKey, string.Format(CacheKeys.GroupInfo, gi.ForumGroupId), false);
				}
				groupArr[i] = gi;
			}
			return groupArr;
		}
		internal int Groups_Save(int PortalId, ForumGroupInfo fg)
		{
			return Groups_Save(PortalId, fg, false);
		}
		public int Groups_Save(int PortalId, ForumGroupInfo fg, bool IsNew)
		{
			var rc = new Security.Roles.RoleController();
			Security.Roles.RoleInfo ri;
			var db = new Data.Common();
			int permissionsId = -1;
			if (fg.PermissionsId == -1)
			{
				ri = rc.GetRoleByName(PortalId, "Administrators");
				if (ri != null)
				{
					fg.PermissionsId = db.CreatePermSet(ri.RoleID.ToString());
					permissionsId = fg.PermissionsId;
				}
			}
			int groupId = Convert.ToInt32(DataProvider.Instance().Groups_Save(PortalId, fg.ModuleId, fg.ForumGroupId, fg.GroupName, fg.SortOrder, fg.Active, fg.Hidden, fg.PermissionsId, fg.PrefixURL));
			if (IsNew)
			{
				Permissions.CreateDefaultSets(PortalId, permissionsId);
				int moduleId = fg.ModuleId;
				string sKey = "G:" + groupId.ToString();
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.TopicsTemplateId, "0");
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.TopicTemplateId, "0");
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.TopicFormId, "0");
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.ReplyFormId, "0");
				Settings.SaveSetting(moduleId, sKey, ForumSettingKeys.AllowRSS, "false");
			}
			DataCache.CacheClear(string.Format(CacheKeys.ForumList, fg.ModuleId));
			return groupId;
		}
	}

}


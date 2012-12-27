//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Data;
using DotNetNuke.Common.Utilities;
namespace DotNetNuke.Modules.ActiveForums
{
#region Forum Groups
	public class ForumGroupInfo
	{
	    private string _Move;
	    private string _GroupCanView;
	    private int _permissionsId = -1;
		public ForumGroupInfo()
		{
			Security = new PermissionInfo();
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

	    public int PermissionsId
		{
			get
			{
				return _permissionsId;
			}
			set
			{
				_permissionsId = value;
			}
		}
		private string _PrefixURL = string.Empty;
		public string PrefixURL
		{
			get
			{
				return _PrefixURL;
			}
			set
			{
				_PrefixURL = value;
			}
		}
		//Public ReadOnly Property GroupCanView() As String
		//    Get
		//        Try
		//            Return _GroupSecurity.Item(SecureActions.View.ToString).ToString
		//        Catch ex As Exception
		//            Return String.Empty
		//        End Try

		//    End Get
		//End Property
#region Settings & Security

		private Hashtable _GroupSettings = new Hashtable();
		private PermissionInfo _PermissionInfo;
		public PermissionInfo Security
		{
			get
			{
				return _PermissionInfo;
			}
			set
			{
				_PermissionInfo = value;
			}
		}
		public Hashtable GroupSettings
		{
			get
			{
				return _GroupSettings;
			}
			set
			{
				_GroupSettings = value;
			}
		}
		public bool AllowAttach
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AllowAttach]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool AllowEmoticons
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AllowEmoticons]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool AllowHTML
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AllowHTML]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool AllowPostIcon
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AllowPostIcon]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool AllowRSS
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AllowRSS]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool ActiveSocialEnabled
		{
			get
			{
#if SKU_LITE
				return false;
#else
				if (_GroupSettings.ContainsKey(ForumSettingKeys.ActiveSocialEnabled))
				{
					try
					{
						return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.ActiveSocialEnabled]);
					}
					catch (Exception ex)
					{
						return false;
					}
				}
			    return false;
#endif
			}
		}
		public bool ActiveSocialTopicsOnly
		{
			get
			{
			    if (_GroupSettings.ContainsKey(ForumSettingKeys.ActiveSocialTopicsOnly))
				{
					try
					{
						return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.ActiveSocialTopicsOnly]);
					}
					catch (Exception ex)
					{
						return false;
					}
				}
			    return false;
			}
		}
		public int ActiveSocialSecurityOption
		{
			get
			{
			    if (_GroupSettings.ContainsKey(ForumSettingKeys.ActiveSocialSecurityOption))
				{
					try
					{
						return Convert.ToInt32(_GroupSettings[ForumSettingKeys.ActiveSocialSecurityOption]);
					}
					catch (Exception ex)
					{
						return 1;
					}
				}
			    return 1;
			}
		}
		public bool AllowScript
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AllowScript]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool AllowSubscribe
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AllowSubscribe]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public int AttachCount
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.AttachCount]);
				}
				catch (Exception ex)
				{
					return 3;
				}
			}
		}
		public int AttachMaxHeight
		{
			get
			{
				try
				{
				    if (_GroupSettings[ForumSettingKeys.AttachMaxHeight].ToString() == string.Empty)
					{
						return 500;
					}
				    return Convert.ToInt32(_GroupSettings[ForumSettingKeys.AttachMaxHeight]);
				}
				catch (Exception ex)
				{
					return 500;
				}
			}
		}
		public int AttachMaxSize
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.AttachMaxSize]);
				}
				catch (Exception ex)
				{
					return 1000;
				}
			}
		}
		public int AttachMaxWidth
		{
			get
			{
				try
				{
				    if (_GroupSettings[ForumSettingKeys.AttachMaxWidth].ToString() == string.Empty)
					{
						return 500;
					}
				    return Convert.ToInt32(_GroupSettings[ForumSettingKeys.AttachMaxWidth]);
				}
				catch (Exception ex)
				{
					return 500;
				}
			}
		}
		public AttachStores AttachStore
		{
			get
			{
				try
				{
					return (AttachStores)(Convert.ToInt32(Enum.Parse(typeof(AttachStores), Convert.ToString(_GroupSettings[ForumSettingKeys.AttachStore].ToString().ToUpper()))));
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public string AttachTypeAllowed
		{
			get
			{
				try
				{
					return Convert.ToString(_GroupSettings[ForumSettingKeys.AttachTypeAllowed]);
				}
				catch (Exception ex)
				{
					return ".jpg,.gif,.png";
				}
			}
		}
		public bool AttachUniqueFileNames
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AttachUniqueFileNames]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public string EditorHeight
		{
			get
			{
				try
				{
					return Convert.ToString(_GroupSettings[ForumSettingKeys.EditorHeight]);
				}
				catch (Exception ex)
				{
					return "400";
				}
			}
		}
		public int EditorStyle
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.EditorStyle]);
				}
				catch (Exception ex)
				{
					return 1;
				}
			}
		}
		public HTMLPermittedUsers EditorPermittedUsers
		{
			get
			{
				try
				{
					return (HTMLPermittedUsers)(Convert.ToInt32(_GroupSettings[ForumSettingKeys.EditorPermittedUsers]));
				}
				catch (Exception ex)
				{
					return HTMLPermittedUsers.AuthenticatedUsers;
				}
			}
		}
		public string EditorToolBar
		{
			get
			{
				try
				{
					return Convert.ToString(_GroupSettings[ForumSettingKeys.EditorToolbar]);
				}
				catch (Exception ex)
				{
					return "bold,italic,underline";
				}
			}
		}
		public EditorTypes EditorType
		{
			get
			{
				try
				{
					return (EditorTypes)(Convert.ToInt32(_GroupSettings[ForumSettingKeys.EditorType]));
				}
				catch (Exception ex)
				{
					return EditorTypes.TEXTBOX;
				}
			}
		}
		public string EditorWidth
		{
			get
			{
				try
				{
					return Convert.ToString(_GroupSettings[ForumSettingKeys.EditorWidth]);
				}
				catch (Exception ex)
				{
					return "100%";
				}
			}
		}
		public string EmailAddress
		{
			get
			{
				try
				{
					return Convert.ToString(_GroupSettings[ForumSettingKeys.EmailAddress]);
				}
				catch (Exception ex)
				{
					return string.Empty;
				}
			}
		}
		public bool IndexContent
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.IndexContent]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool AutoSubscribeEnabled
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AutoSubscribeEnabled]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public string AutoSubscribeRoles
		{
			get
			{
				try
				{
					return Convert.ToString(_GroupSettings[ForumSettingKeys.AutoSubscribeRoles]);
				}
				catch (Exception ex)
				{
					return string.Empty;
				}
			}
		}
		public bool AutoSubscribeNewTopicsOnly
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.AutoSubscribeNewTopicsOnly]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool IsModerated
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.IsModerated]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public int TopicsTemplateId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.TopicsTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int TopicTemplateId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.TopicTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int TopicFormId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.TopicFormId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int ReplyFormId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.ReplyFormId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int QuickReplyFormId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.QuickReplyFormId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int ProfileTemplateId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.ProfileTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public bool UseFilter
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_GroupSettings[ForumSettingKeys.UseFilter]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public int AutoTrustLevel
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.AutoTrustLevel]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int DefaultTrustValue
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.DefaultTrustValue]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int ModApproveTemplateId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.ModApproveTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int ModRejectTemplateId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.ModRejectTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int ModMoveTemplateId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.ModMoveTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int ModDeleteTemplateId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.ModDeleteTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int ModNotifyTemplateId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_GroupSettings[ForumSettingKeys.ModNotifyTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
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
#endregion
}


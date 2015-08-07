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

        public bool AllowLikes
        {
            get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AllowLikes]); }
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

		public string AttachTypeAllowed
		{
			get { return Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.AttachTypeAllowed], ".jpg,.gif,.png"); }
		}

		public bool AttachAllowBrowseSite
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AttachAllowBrowseSite]); }
		}

		public int MaxAttachWidth
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.MaxAttachWidth], 800); }
		}

		public int MaxAttachHeight
		{
			get { return Utilities.SafeConvertInt(GroupSettings[ForumSettingKeys.MaxAttachHeight], 800); }
		}

		public bool AttachInsertAllowed
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.AttachInsertAllowed]); }
		}

		public bool ConvertingToJpegAllowed
		{
			get { return Utilities.SafeConvertBool(GroupSettings[ForumSettingKeys.ConvertingToJpegAllowed]); }
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
				var val = Enum.TryParse(Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EditorType], "0"), true, out parseValue)
						   ? parseValue
						   : EditorTypes.TEXTBOX;

				return val;
				
			}
		}

		public string EditorWidth
		{
			get { return Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EditorWidth], "100%"); }
		}

        public EditorTypes EditorMobile
        {
            get
            {
                EditorTypes parseValue;
                var val = Enum.TryParse(Utilities.SafeConvertString(GroupSettings[ForumSettingKeys.EditorMobile], "0"), true, out parseValue)
                           ? parseValue
                           : EditorTypes.TEXTBOX;

                return val;

            }
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
		public void Group_Delete(int moduleId, int forumGroupId)
		{
			DataProvider.Instance().Groups_Delete(moduleId, forumGroupId);
		}

		public ForumGroupInfo GetForumGroup(int moduleId, int forumGroupId)
		{
			var db = new Data.Groups();
			ForumGroupInfo gi = null;
			using (var dr = db.Groups_Get(moduleId, forumGroupId))
			{
				while (dr.Read())
				{
					gi = FillForumGroup(dr);
				}
				dr.Close();
			}
			return gi;
		}

		private static ForumGroupInfo FillForumGroup(IDataRecord dr)
		{
			var g = new ForumGroupInfo
						{
							Active = dr.GetBoolean("Active"),
							ForumGroupId = dr.GetInt("ForumGroupId"),
							Hidden = dr.GetBoolean("Hidden"),
							GroupName = dr.GetString("GroupName"),
							SortOrder = dr.GetInt("SortOrder"),
							GroupSettingsKey = dr.GetString("GroupSettingsKey"),
							PermissionsId = dr.GetInt("PermissionsId"),
							PrefixURL = dr.GetString("PrefixURL"),
							ModuleId = dr.GetInt("ModuleId"),
							Security =
								{
									Announce =  dr.GetString("CanAnnounce"),
									Attach = dr.GetString("CanAttach"),
									Create = dr.GetString("CanCreate"),
									Delete = dr.GetString("CanDelete"),
									Edit = dr.GetString("CanEdit"),
									Lock = dr.GetString("CanLock"),
									ModApprove = dr.GetString("CanModApprove"),
									ModDelete = dr.GetString("CanModDelete"),
									ModEdit = dr.GetString("CanModEdit"),
									ModLock = dr.GetString("CanModLock"),
									ModMove = dr.GetString("CanModMove"),
									ModPin = dr.GetString("CanModPin"),
									ModSplit = dr.GetString("CanModSplit"),
									ModUser = dr.GetString("CanModUser"),
									Pin = dr.GetString("CanPin"),
									Poll = dr.GetString("CanPoll"),
									Block = dr.GetString("CanBlock"),
									Read = dr.GetString("CanRead"),
									Reply = dr.GetString("CanReply"),
									Subscribe = dr.GetString("CanSubscribe"),
									Trust = dr.GetString("CanTrust"),
									View = dr.GetString("CanView"),
									Tag = dr.GetString("CanTag"),
									Prioritize = dr.GetString("CanPrioritize"),
									Categorize = dr.GetString("CanCategorize")
								}
						};

			return g;
		}

		public ForumGroupInfo Groups_Get(int moduleID, int forumGroupID)
		{
			var gi = GetForumGroup(moduleID, forumGroupID);
			gi.GroupSettings = DataCache.GetSettings(moduleID, gi.GroupSettingsKey, string.Format(CacheKeys.GroupInfo, forumGroupID), false);
			return gi;
		}

		public ArrayList Groups_List(int moduleId, bool fillSettings = false)
		{
			var groupArr = CBO.FillCollection(DataProvider.Instance().Groups_List(moduleId), typeof(ForumGroupInfo));
			
			if (fillSettings == false)
				return groupArr;

			int i;
			for (i = 0; i < groupArr.Count; i++)
			{
				var gi = groupArr[i] as ForumGroupInfo;
				if(gi == null)
					continue;

				gi.GroupSettings = DataCache.GetSettings(moduleId, gi.GroupSettingsKey, string.Format(CacheKeys.GroupInfo, gi.ForumGroupId), false);

				groupArr[i] = gi;
			}
			return groupArr;
		}

		internal int Groups_Save(int portalId, ForumGroupInfo fg)
		{
			return Groups_Save(portalId, fg, false);
		}

		public int Groups_Save(int portalId, ForumGroupInfo fg, bool isNew)
		{
			var rc = new Security.Roles.RoleController();
			var db = new Data.Common();
			
			var permissionsId = -1;
			if (fg.PermissionsId == -1)
			{
				var ri = rc.GetRoleByName(portalId, "Administrators");
				if (ri != null)
				{
					fg.PermissionsId = db.CreatePermSet(ri.RoleID.ToString());
					permissionsId = fg.PermissionsId;
				}
			}
		   
			var groupId = DataProvider.Instance().Groups_Save(portalId, fg.ModuleId, fg.ForumGroupId, fg.GroupName, fg.SortOrder, fg.Active, fg.Hidden, fg.PermissionsId, fg.PrefixURL);
			if (isNew)
			{
				Permissions.CreateDefaultSets(portalId, permissionsId);
				var moduleId = fg.ModuleId;
				var sKey = "G:" + groupId.ToString();
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


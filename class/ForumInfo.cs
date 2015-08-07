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
using System.Collections.Generic;

namespace DotNetNuke.Modules.ActiveForums
{
	public class Forum
	{
		#region AF Forums DB Fields

		public int ForumID { get; set; }

		public int ModuleId { get; set; }

		public int PortalId { get; set; }

		public int TabId { get; set; }

		public int ForumGroupId { get; set; }

		public int ParentForumId { get; set; }

		public string ForumName { get; set; }

		public string ForumDesc { get; set; }

		public int SortOrder { get; set; }

		public bool Active { get; set; }

		public bool Hidden { get; set; }

		public int TotalTopics { get; set; }

		public int TotalReplies { get; set; }

		public int LastPostID { get; set; }

		public string GroupName { get; set; }

		public string ParentForumName { get; set; }

		public string ForumSettingsKey { get; set; }

		public string ForumSecurityKey { get; set; }

		public int LastTopicId { get; set; }

		public int LastReplyId { get; set; }

		public int PermissionsId { get; set; }

		public bool InheritSecurity { get; set; }

		public ForumGroupInfo ForumGroup { get; set; }

		public string PrefixURL { get; set; }

		public string ForumURL
		{
			get
			{
				return URL.ForumLink(TabId, this);
			}
		}

		public string TopicUrl { get; set; }

		public ForumCollection SubForums { get; set; }

		public List<PropertiesInfo> Properties { get; set; }

		public int SocialGroupId { get; set; }

		public bool HasProperties { get; set; }

		#endregion

		#region Settings & Security

		public PermissionInfo Security { get; set; }

		public Hashtable ForumSettings { get; set; }

		#endregion

		#region Last Post

		public DateTime LastRead { get; set; }

		public DateTime LastPostDateTime { get; set; }

		public int LastPostUserID { get; set; }

		public string LastPostUserName { get; set; }

		public string LastPostFirstName { get; set; }

		public string LastPostLastName { get; set; }

		public string LastPostSubject { get; set; }

		public int LastPostLastPostID { get; set; }

		public string LastPostDisplayName { get; set; }

		public int LastPostParentPostID { get; set; }

		#endregion
		
		// initialization
		public Forum()
		{
			PortalId = -1;
			TabId = -1;
			PermissionsId = -1;
			PrefixURL = string.Empty;
			ForumSettings = new Hashtable();

			Security = new PermissionInfo();
		}

		public string TopicSubject { get; set; }

		public int TopicId { get; set; }

		public int CustomFieldType { get; set; }

		public bool AllowAttach
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowAttach]); }
		}

		public bool AllowEmoticons
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowEmoticons]); }
		}

		public bool AllowHTML
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowHTML]); }
		}

		public bool AllowLikes
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowLikes]); }
		}

		public bool AllowPostIcon
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowPostIcon]); }
		}

		public bool AllowRSS
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowRSS]); }
		}

		public bool AllowScript
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowScript]); }
		}

		public bool AllowSubscribe
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowSubscribe]); }
		}

		public int AttachCount
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.AttachCount], 3); }
		}

		public int AttachMaxHeight
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.AttachMaxHeight], 500); }
		}

		public int AttachMaxSize
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.AttachMaxSize], 1000); }
		}

		public int AttachMaxWidth
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.AttachMaxWidth], 500); }
		}

		public string AttachTypeAllowed
		{
			get { return Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.AttachTypeAllowed], ".jpg,.gif,.png"); }
		}

		public bool AttachAllowBrowseSite
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AttachAllowBrowseSite]); }
		}

		public int MaxAttachWidth
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.MaxAttachWidth], 800); }
		}

		public int MaxAttachHeight
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.MaxAttachHeight], 800); }
		}

		public bool AttachInsertAllowed
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AttachInsertAllowed]); }
		}

		public bool ConvertingToJpegAllowed
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.ConvertingToJpegAllowed]); }
		}

		public string EditorHeight
		{
			get { return Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.EditorHeight], "400"); }
		}

		public int EditorStyle
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.EditorStyle], 1); }
		}

        public int EditorMobile
        {
            get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.EditorMobile], 0); }
        }

		public string EditorToolBar
		{
			get { return Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.EditorToolbar], "bold,italic,underline"); }
		}

		public EditorTypes EditorType
		{
			get
			{
				EditorTypes parseValue;
				return Enum.TryParse(Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.EditorType], "0"), true, out parseValue)
						   ? parseValue
						   : EditorTypes.TEXTBOX;
			}
		}

		public HTMLPermittedUsers EditorPermittedUsers
		{
			get
			{
				HTMLPermittedUsers parseValue;
				return Enum.TryParse(Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.EditorPermittedUsers], "1"), true, out parseValue)
						   ? parseValue
						   : HTMLPermittedUsers.AuthenticatedUsers;
			}
		}

		public string EditorWidth
		{
			get { return Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.EditorWidth], "100%"); }
		}

		public string EmailAddress
		{
			get { return Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.EmailAddress], string.Empty); }
		}

		public bool IndexContent
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.IndexContent]); }
		}

		public bool IsModerated
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.IsModerated]); }
		}

		public int TopicsTemplateId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.TopicsTemplateId]); }
		}

		public int TopicTemplateId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.TopicTemplateId]); }
		}

		public int TopicFormId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.TopicFormId]); }
		}

		public int ReplyFormId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ReplyFormId]); }
		}

		public int QuickReplyFormId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.QuickReplyFormId]); }
		}

		public int ProfileTemplateId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ProfileTemplateId]); }
		}

		public bool UseFilter
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.UseFilter]); }
		}

		public int AutoTrustLevel
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.AutoTrustLevel]); }
		}

		public TrustTypes DefaultTrustValue
		{
			get
			{
				TrustTypes parseValue;
				return Enum.TryParse(Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.DefaultTrustValue], "0"), true, out parseValue)
						   ? parseValue
						   : TrustTypes.NotTrusted;
			}
		}

		public int ModApproveTemplateId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ModApproveTemplateId]); }
		}

		public int ModRejectTemplateId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ModRejectTemplateId]); }
		}

		public int ModMoveTemplateId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ModMoveTemplateId]); }
		}

		public int ModDeleteTemplateId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ModDeleteTemplateId]); }
		}

		public int ModNotifyTemplateId
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ModNotifyTemplateId]); }
		}

		public bool AllowTags
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AllowTags]); }
		}

		public bool ActiveSocialEnabled
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.ActiveSocialEnabled]); }
		}

		public bool ActiveSocialTopicsOnly
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.ActiveSocialTopicsOnly]); }
		}

		public int ActiveSocialSecurityOption
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ActiveSocialSecurityOption], 1); }
		}

		public bool AutoSubscribeEnabled
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AutoSubscribeEnabled]); }
		}

		public string AutoSubscribeRoles
		{
			get { return Utilities.SafeConvertString(ForumSettings[ForumSettingKeys.AutoSubscribeRoles], string.Empty); }
		}

		public bool AutoSubscribeNewTopicsOnly
		{
			get { return Utilities.SafeConvertBool(ForumSettings[ForumSettingKeys.AutoSubscribeNewTopicsOnly]); }
		}

		public int CreatePostCount // Minimum posts required to create a topic in this forum if the user is not trusted
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.CreatePostCount]); }
		}

		public int ReplyPostCount // Minimum posts required to reply to a topic in this forum if the user is not trusted
		{
			get { return Utilities.SafeConvertInt(ForumSettings[ForumSettingKeys.ReplyPostCount]); }
		}

	}
}
//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
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
        
		private bool _isModerated;
		private bool _allowRSS;


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

	    public bool AllowAttach
		{
			get
            {
                bool value;
		        var obj = ForumSettings[ForumSettingKeys.AllowAttach];
                return (obj != null && bool.TryParse(obj.ToString(), out value)) && value;
			}
		}

		public bool AllowEmoticons
		{
			get
			{
				try
				{
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AllowEmoticons]);
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
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AllowHTML]);
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
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AllowPostIcon]);
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
				    if (_allowRSS == false)
					{
						return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AllowRSS]);
					}
				    return _allowRSS;
				}
				catch (Exception ex)
				{
					return false;
				}
			}
			set
			{
				_allowRSS = value;
			}
		}

		public bool AllowScript
		{
			get
			{
				try
				{
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AllowScript]);
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
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AllowSubscribe]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.AttachCount]);
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
				    if (ForumSettings[ForumSettingKeys.AttachMaxHeight].ToString() == string.Empty)
					{
						return 500;
					}
				    return Convert.ToInt32(ForumSettings[ForumSettingKeys.AttachMaxHeight]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.AttachMaxSize]);
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
				    if (ForumSettings[ForumSettingKeys.AttachMaxWidth].ToString() == string.Empty)
					{
						return 500;
					}
				    return Convert.ToInt32(ForumSettings[ForumSettingKeys.AttachMaxWidth]);
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
					return (AttachStores)(Convert.ToInt32(Enum.Parse(typeof(AttachStores), Convert.ToString(ForumSettings[ForumSettingKeys.AttachStore].ToString().ToUpper()))));
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
					return Convert.ToString(ForumSettings[ForumSettingKeys.AttachTypeAllowed]);
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
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AttachUniqueFileNames]);
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
					return Convert.ToString(ForumSettings[ForumSettingKeys.EditorHeight]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.EditorStyle]);
				}
				catch (Exception ex)
				{
					return 1;
				}
			}
		}

		public string EditorToolBar
		{
			get
			{
				try
				{
					return Convert.ToString(ForumSettings[ForumSettingKeys.EditorToolbar]);
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
					return (EditorTypes)(Convert.ToInt32(ForumSettings[ForumSettingKeys.EditorType]));
				}
				catch (Exception ex)
				{
					return EditorTypes.TEXTBOX;
				}
			}
		}

		public HTMLPermittedUsers EditorPermittedUsers
		{
			get
			{
				try
				{
					return (HTMLPermittedUsers)(Convert.ToInt32(ForumSettings[ForumSettingKeys.EditorPermittedUsers]));
				}
				catch (Exception ex)
				{
					return HTMLPermittedUsers.AuthenticatedUsers;
				}
			}
		}

		public string EditorWidth
		{
			get
			{
				try
				{
					return Convert.ToString(ForumSettings[ForumSettingKeys.EditorWidth]);
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
					return Convert.ToString(ForumSettings[ForumSettingKeys.EmailAddress]);
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
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.IndexContent]);
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
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.IsModerated]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.TopicsTemplateId]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.TopicTemplateId]);
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

					return Convert.ToInt32(ForumSettings[ForumSettingKeys.TopicFormId]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.ReplyFormId]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.QuickReplyFormId]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.ProfileTemplateId]);
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
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.UseFilter]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.AutoTrustLevel]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}

		public TrustTypes DefaultTrustValue
		{
			get
			{
				try
				{
					return (TrustTypes)(Convert.ToInt32(ForumSettings[ForumSettingKeys.DefaultTrustValue]));
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.ModApproveTemplateId]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.ModRejectTemplateId]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.ModMoveTemplateId]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.ModDeleteTemplateId]);
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
					return Convert.ToInt32(ForumSettings[ForumSettingKeys.ModNotifyTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}

		public bool AllowTags
		{
			get
			{
				try
				{
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AllowTags]);
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
			    if (ForumSettings.ContainsKey(ForumSettingKeys.ActiveSocialEnabled))
				{
					try
					{
						return Convert.ToBoolean(ForumSettings[ForumSettingKeys.ActiveSocialEnabled]);
					}
					catch (Exception ex)
					{
						return false;
					}
				}
			    return false;
			}
		}

		public bool ActiveSocialTopicsOnly
		{
			get
			{
			    if (ForumSettings.ContainsKey(ForumSettingKeys.ActiveSocialTopicsOnly))
				{
					try
					{
						return Convert.ToBoolean(ForumSettings[ForumSettingKeys.ActiveSocialTopicsOnly]);
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
			    if (ForumSettings.ContainsKey(ForumSettingKeys.ActiveSocialSecurityOption))
			    {
			        int value;
                    return int.TryParse(ForumSettings[ForumSettingKeys.ActiveSocialSecurityOption].ToString(), out value) ? value : 1;
			    }
			    return 1;
			}
		}

	    public int CustomFieldType { get; set; }

	    public bool AutoSubscribeEnabled
		{
			get
			{
				try
				{
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AutoSubscribeEnabled]);
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
					return Convert.ToString(ForumSettings[ForumSettingKeys.AutoSubscribeRoles]);
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
					return Convert.ToBoolean(ForumSettings[ForumSettingKeys.AutoSubscribeNewTopicsOnly]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}

	}
}
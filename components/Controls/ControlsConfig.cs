using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ControlsConfig
	{
#region Private Members
		private int _siteId;
		private int _pageId = -1;
		private int _instanceId;
		private int _forumId;
		private int _topicId;
		private int _contentId;
		private string _appPath;
		private string _themePath;
		private User _user;
		private string _templatePath;
		private string _defaultViewRoles;
		private string _profileLink;
		private string _membersLink;
		private string _adminRoles;
#endregion
#region Public Properties
		public string AdminRoles
		{
			get
			{
				return _adminRoles;
			}
			set
			{
				_adminRoles = value;
			}
		}

		public string AppPath
		{
			get
			{
				return _appPath;
			}
			set
			{
				_appPath = value;
			}
		}

		public int ContentId
		{
			get
			{
				return _contentId;
			}
			set
			{
				_contentId = value;
			}
		}

		public string DefaultViewRoles
		{
			get
			{
				return _defaultViewRoles;
			}
			set
			{
				_defaultViewRoles = value;
			}
		}

		public int ForumId
		{
			get
			{
				return _forumId;
			}
			set
			{
				_forumId = value;
			}
		}

		public int InstanceId
		{
			get
			{
				return _instanceId;
			}
			set
			{
				_instanceId = value;
			}
		}

		public string MembersLink
		{
			get
			{
				return _membersLink;
			}
			set
			{
				_membersLink = value;
			}
		}

		public int PageId
		{
			get
			{
				return _pageId;
			}
			set
			{
				_pageId = value;
			}
		}

		public string ProfileLink
		{
			get
			{
				return _profileLink;
			}
			set
			{
				_profileLink = value;
			}
		}

		public int SiteId
		{
			get
			{
				return _siteId;
			}
			set
			{
				_siteId = value;
			}
		}

		public string TemplatePath
		{
			get
			{
				return _templatePath;
			}
			set
			{
				_templatePath = value;
			}
		}

		public string ThemePath
		{
			get
			{
				return _themePath;
			}
			set
			{
				_themePath = value;
			}
		}

		public int TopicId
		{
			get
			{
				return _topicId;
			}
			set
			{
				_topicId = value;
			}
		}

		public User User
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value;
			}
		}

#endregion


	}
}


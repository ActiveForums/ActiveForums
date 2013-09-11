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


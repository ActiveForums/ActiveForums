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
	public class User
	{
		private bool _isAdmin = false;
		private bool _isSuperUser = false;
		private UserProfileInfo _profile = null;
		private Hashtable _properties = null;
		private int _userId = -1;
		private string _userName = string.Empty;
		private string _userRoles;
		private string _lastName = string.Empty;
		private string _firstName = string.Empty;
		private string _displayName = string.Empty;
		private string _email = string.Empty;
		private DateTime _dateUpdated;
		private DateTime _dateCreated;
		private string _userForums = string.Empty;
		public string FirstName
		{
			get
			{
				return _firstName;
			}
			set
			{
				_firstName = value;
			}
		}
		public string LastName
		{
			get
			{
				return _lastName;
			}
			set
			{
				_lastName = value;
			}
		}
		public string DisplayName
		{
			get
			{
				return _displayName;
			}
			set
			{
				_displayName = value;
			}
		}
		public string Email
		{
			get
			{
				return _email;
			}
			set
			{
				_email = value;
			}
		}
		public DateTime DateCreated
		{
			get
			{
				return _dateCreated;
			}
			set
			{
				_dateCreated = value;
			}
		}
		public DateTime DateUpdated
		{
			get
			{
				return _dateUpdated;
			}
			set
			{
				_dateUpdated = value;
			}
		}
		public bool IsAdmin
		{
			get
			{
				return _isAdmin;
			}
			set
			{
				_isAdmin = value;
			}
		}
		public bool IsSuperUser
		{
			get
			{
				return _isSuperUser;
			}
			set
			{
				_isSuperUser = value;
			}
		}

		public UserProfileInfo Profile
		{
			get
			{
				return _profile;
			}
			set
			{
				_profile = value;
			}
		}

		public System.Collections.Hashtable Properties
		{
			get
			{
				return _properties;
			}
			set
			{
				_properties = value;
			}
		}

		public int UserId
		{
			get
			{
				return _userId;
			}
			set
			{
				_userId = value;
			}
		}
		public string UserName
		{
			get
			{
				return _userName;
			}
			set
			{
				_userName = value;
			}
		}

		public string UserRoles
		{
			get
			{
				return _userRoles;
			}
			set
			{
				_userRoles = value;
			}
		}
		public bool PrefBlockSignatures
		{
			get
			{
				return Profile.PrefBlockSignatures;
			}
		}
		public bool PrefBlockAvatars
		{
			get
			{
				return Profile.PrefBlockAvatars;
			}
		}
		public int PostCount
		{
			get
			{
				return Profile.PostCount;
			}
		}
		public int TrustLevel
		{
			get
			{
				return Profile.TrustLevel;
			}
		}
		public bool PrefTopicSubscribe
		{
			get
			{
				return Profile.PrefTopicSubscribe;
			}
		}
		public CurrentUserTypes CurrentUserType
		{
			get
			{
				return Profile.CurrentUserType;
			}
		}
		public string UserForums
		{
			get
			{
				return _userForums;
			}
			set
			{
				_userForums = value;
			}
		}
		public User()
		{
			_userId = -1;
			_isSuperUser = false;
			_isAdmin = false;
			_profile = new UserProfileInfo();
			_userRoles = Globals.DefaultAnonRoles + "|-1;||";
		}
	}
}


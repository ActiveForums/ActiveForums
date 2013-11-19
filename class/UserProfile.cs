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
#region UserProfileInfo
	public class UserProfileInfo
	{
#region Private Members
		private int _ProfileId = -1;
		private int _UserId = -1;
	    private string _PrefDefaultSort = "ASC";
	    private bool _PrefUseAjax = true;
		private EmailFormats _PrefEmailFormat;
	    private int _PrefPageSize = 20;
	    private string _Bio = string.Empty;

	    #endregion
#region Constructors
		public UserProfileInfo()
		{
		    IsUserOnline = false;
		    IsMod = false;

		    _PrefDefaultSort = "ASC";
			_PrefPageSize = 20;
			PrefBlockSignatures = false;
			PrefBlockAvatars = false;
			PrefJumpLastPost = false;
			PrefDefaultShowReplies = false;
			_PrefUseAjax = false;
			PrefTopicSubscribe = false;
		}
		public UserProfileInfo(int UserId, int PortalId)
		{
		    IsUserOnline = false;
		    IsMod = false;
		    PrefBlockSignatures = false;
		    PrefBlockAvatars = false;
		    PrefTopicSubscribe = false;
		    PrefJumpLastPost = false;
		    PrefDefaultShowReplies = false;
		}

	    #endregion
#region Public Properties
		public int ProfileId
		{
			get
			{
				return _ProfileId;
			}
			set
			{
				_ProfileId = value;
			}
		}
		public int UserID
		{
			get
			{
				return _UserId;
			}
			set
			{
				_UserId = value;
			}
		}

	    public int PortalId { get; set; }

	    public int ModuleId { get; set; }

	    public int TopicCount { get; set; }

	    public int ReplyCount { get; set; }

	    public int ViewCount { get; set; }

	    public int AnswerCount { get; set; }

	    public int RewardPoints { get; set; }

	    public string UserCaption { get; set; }

	    public DateTime DateCreated { get; set; }

	    public DateTime DateUpdated { get; set; }

	    public DateTime DateLastActivity { get; set; }

	    public DateTime DateLastPost { get; set; }

	    public string Signature { get; set; }

	    public bool SignatureDisabled { get; set; }

	    public int TrustLevel { get; set; }

	    public bool AdminWatch { get; set; }

	    public bool AttachDisabled { get; set; }

	    public string Avatar { get; set; }

	    public AvatarTypes AvatarType { get; set; }

	    public bool AvatarDisabled { get; set; }

	    public string PrefDefaultSort
		{
			get
			{
				return _PrefDefaultSort;
			}
			set
			{
				_PrefDefaultSort = value;
			}
		}

	    public bool PrefDefaultShowReplies { get; set; }

	    public bool PrefJumpLastPost { get; set; }

	    public bool PrefTopicSubscribe { get; set; }

	    public SubscriptionTypes PrefSubscriptionType { get; set; }

	    public bool PrefUseAjax
		{
			get
			{
				return _PrefUseAjax;
			}
			set
			{
				_PrefUseAjax = value;
			}
		}

	    public bool PrefBlockAvatars { get; set; }

	    public bool PrefBlockSignatures { get; set; }

	    public int PrefPageSize
		{
			get
			{
				return _PrefPageSize;
			}
			set
			{
				_PrefPageSize = value;
			}
		}

	    public string Yahoo { get; set; }

	    public string MSN { get; set; }

	    public string ICQ { get; set; }

	    public string AOL { get; set; }

	    public string Occupation { get; set; }

	    public string Location { get; set; }

	    public string Interests { get; set; }

	    public string WebSite { get; set; }

	    public string Badges { get; set; }

	    public string Roles { get; set; }

	    public string FirstName { get; set; }

	    public string LastName { get; set; }

	    public string DisplayName { get; set; }

	    public string Username { get; set; }

	    public string Email { get; set; }

	    public bool IsMod { get; set; }

	    public string Bio
		{
			get
			{
				return _Bio;
			}
			set
			{
				_Bio = value;
			}
		}

	    public bool IsUserOnline { get; set; }

	    public Hashtable ProfileProperties { get; set; }

	    public CurrentUserTypes CurrentUserType { get; set; }

	    public string ForumsAllowed { get; set; }

	    public string UserForums { get; set; }

	    #endregion
#region Public ReadOnly Properties
		public int PostCount
		{
			get
			{
				return TopicCount + ReplyCount;
			}
		}
#endregion
	}
#endregion
#region UserProfileController
	public class UserProfileController
	{
		public UserProfileInfo Profiles_Get(int PortalId, int ModuleId, int UserId)
		{
            
			UserProfileInfo upi = null;
			DataSet ds = DataProvider.Instance().Profiles_Get(PortalId, ModuleId, UserId);
			if (ds.Tables.Count == 0)
			{
                //todo: UPI is always null?
				return upi;
			}
			if (ds.Tables[0].Rows.Count > 0)
			{
				IDataReader dr;
				dr = ds.CreateDataReader();
				upi = (UserProfileInfo)(CBO.FillObject(dr, typeof(UserProfileInfo)));
			}

			return upi;
		}


		public void Profiles_Save(UserProfileInfo ui)
		{
            DataProvider.Instance().Profiles_Save(ui.PortalId, ui.ModuleId, ui.UserID, ui.TopicCount, ui.ReplyCount, ui.ViewCount, ui.AnswerCount, ui.RewardPoints, ui.UserCaption, ui.Signature, ui.SignatureDisabled, ui.TrustLevel, ui.AdminWatch, ui.AttachDisabled, ui.Avatar, (int)ui.AvatarType, ui.AvatarDisabled, ui.PrefDefaultSort, ui.PrefDefaultShowReplies, ui.PrefJumpLastPost, ui.PrefTopicSubscribe, (int)ui.PrefSubscriptionType, ui.PrefUseAjax, ui.PrefBlockAvatars, ui.PrefBlockSignatures, ui.PrefPageSize, ui.Yahoo, ui.MSN, ui.ICQ, ui.AOL, ui.Occupation, ui.Location, ui.Interests, ui.WebSite, ui.Badges);
			// KR - clear cache when updated
			DataCache.CacheClearPrefix(string.Format("AF-prof-{0}", ui.UserID));
		}

	}
#endregion

}
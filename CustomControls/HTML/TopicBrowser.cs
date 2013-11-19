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

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class TopicBrowser
	{
		private int _PortalId = -1;
		public int PortalId
		{
			get
			{
				return _PortalId;
			}
			set
			{
				_PortalId = value;
			}
		}
		private int _ModuleId = -1;
		public int ModuleId
		{
			get
			{
				return _ModuleId;
			}
			set
			{
				_ModuleId = value;
			}
		}
		private int _TabId = -1;
		public int TabId
		{
			get
			{
				return _TabId;
			}
			set
			{
				_TabId = value;
			}
		}
		private string _ForumIds = string.Empty;
		public string ForumIds
		{
			get
			{
				return _ForumIds;
			}
			set
			{
				_ForumIds = value;
			}
		}
		private int _ForumId = -1;
		public int ForumId
		{
			get
			{
				return _ForumId;
			}
			set
			{
				_ForumId = value;
			}
		}
		private int _ForumGroupId = -1;
		public int ForumGroupId
		{
			get
			{
				return _ForumGroupId;
			}
			set
			{
				_ForumGroupId = value;
			}
		}
		private int _ParentForumId = -1;
		public int ParentForumId
		{
			get
			{
				return _ParentForumId;
			}
			set
			{
				_ParentForumId = value;
			}
		}
		private int _TopicId = -1;
		public int TopicId
		{
			get
			{
				return _TopicId;
			}
			set
			{
				_TopicId = value;
			}
		}
		private string _Topic = string.Empty;
		public string Topic
		{
			get
			{
				return _Topic;
			}
			set
			{
				_Topic = value;
			}
		}
		private string _Template = string.Empty;
		public string Template
		{
			get
			{
				return _Template;
			}
			set
			{
				_Template = value;
			}
		}
		private string _HeaderTemplate = string.Empty;
		public string HeaderTemplate
		{
			get
			{
				return _HeaderTemplate;
			}
			set
			{
				_HeaderTemplate = value;
			}
		}
		private string _FooterTemplate = string.Empty;
		public string FooterTemplate
		{
			get
			{
				return _FooterTemplate;
			}
			set
			{
				_FooterTemplate = value;
			}
		}
		public User ForumUser {get; set;}
		private int _CategoryId = -1;
		public int CategoryId
		{
			get
			{
				return _CategoryId;
			}
			set
			{
				_CategoryId = value;
			}
		}
		private int _TagId = -1;
		public int TagId
		{
			get
			{
				return _TagId;
			}
			set
			{
				_TagId = value;
			}
		}
		private int _PageIndex = 1;
		public int PageIndex
		{
			get
			{
				return _PageIndex;
			}
			set
			{
				_PageIndex = value;
			}
		}
		private int _PageSize = 20;
		public int PageSize
		{
			get
			{
				return _PageSize;
			}
			set
			{
				_PageSize = value;
			}
		}
		private string _ItemCss = "aftb-topic";
		public string ItemCss
		{
			get
			{
				return _ItemCss;
			}
			set
			{
				_ItemCss = value;
			}
		}
		private string _AltItemCSS = "aftb-topic-alt";
		public string AltItemCSS
		{
			get
			{
				return _AltItemCSS;
			}
			set
			{
				_AltItemCSS = value;
			}
		}
		private bool _UseAjax = false;
		public bool UseAjax
		{
			get
			{
				return _UseAjax;
			}
			set
			{
				_UseAjax = value;
			}
		}
		private string _ImagePath = string.Empty;
		public string ImagePath
		{
			get
			{
				return _ImagePath;
			}
			set
			{
				_ImagePath = value;
			}
		}
		private bool _MaintainPage = false;
		public bool MaintainPage
		{
			get
			{
				return _MaintainPage;
			}
			set
			{
				_MaintainPage = value;
			}
		}
		private SettingsInfo _mainSettings = null;
		private bool _canEdit = false;
		private int _TimeZoneOffset = 0;
		public int TimeZoneOffset
		{
			get
			{
				return _TimeZoneOffset;
			}
			set
			{
				_TimeZoneOffset = value;
			}
		}
		public string Render()
		{
			ForumController fc = new ForumController();
			string fs = fc.GetForumsForUser(ForumUser.UserRoles, PortalId, ModuleId, "CanEdit");
			if (! (string.IsNullOrEmpty(fs)))
			{
				_canEdit = true;
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string forumPrefix = string.Empty;
			string groupPrefix = string.Empty;
			_mainSettings = DataCache.MainSettings(ModuleId);
			if (_mainSettings.URLRewriteEnabled)
			{
				if (ForumId > 0)
				{
					Forum f = fc.GetForum(PortalId, ModuleId, ForumId);
					if (f != null)
					{
						forumPrefix = f.PrefixURL;
						groupPrefix = f.ForumGroup.PrefixURL;
					}
				}
				else if (ForumGroupId > 0)
				{
					ForumGroupController grp = new ForumGroupController();
					ForumGroupInfo g = grp.Groups_Get(ModuleId, ForumGroupId);
					if (g != null)
					{
						groupPrefix = g.PrefixURL;
					}
				}
			}

			string tmp = string.Empty;
			Data.Topics db = new Data.Topics();
			int recordCount = 0;
			int i = 0;
			sb.Append(HeaderTemplate);
			using (IDataReader dr = db.TopicsList(PortalId, PageIndex, PageSize, ForumIds, CategoryId, TagId))
			{
				while (dr.Read())
				{
					if (recordCount == 0)
					{
						recordCount = int.Parse(dr["RecordCount"].ToString());
					}
					tmp = ParseDataRow(dr, Template);
					if (i % 2 == 0)
					{
						tmp = tmp.Replace("[ROWCSS]", ItemCss);
					}
					else
					{
						tmp = tmp.Replace("[ROWCSS]", AltItemCSS);
					}
					i += 1;
					sb.Append(tmp);
				}
				dr.Close();
			}
			sb.Append(FooterTemplate);
			int pageCount = 1;
			pageCount = Convert.ToInt32(System.Math.Ceiling((double)recordCount / PageSize));
			ControlUtils cUtils = new ControlUtils();
			string otherPrefix = string.Empty;
			if (TagId > 0 | CategoryId > 0)
			{
				int id = -1;
				if (TagId > 0)
				{
					id = TagId;
				}
				else
				{
					id = CategoryId;
				}
				using (IDataReader dr = DataProvider.Instance().Tags_Get(PortalId, ModuleId, id))
				{
					while (dr.Read())
					{
						otherPrefix = Utilities.CleanName(dr["TagName"].ToString());
					}
					dr.Close();
				}
			}
			sb.Append(cUtils.BuildPager(TabId, ModuleId, groupPrefix, forumPrefix, ForumGroupId, ForumId, TagId, CategoryId, otherPrefix, PageIndex, pageCount));
			return sb.ToString();
		}
		private string ParseDataRow(IDataRecord row, string tmp)
		{
			try
			{
				tmp = tmp.Replace("[AVATAR]", "[AF:AVATAR]");
				for (int i = 0; i < row.FieldCount; i++)
				{
					string name = row.GetName(i);
					string k = "[" + name.ToUpperInvariant() + "]";
					string value = row[i].ToString();
					switch (row[i].GetType().ToString())
					{
						case "System.DateTime":
							value = Utilities.GetDate(Convert.ToDateTime(row[i].ToString()), ModuleId, TimeZoneOffset);
							break;
					}
					tmp = tmp.Replace(k, value);
				}

				ControlUtils cUtils = new ControlUtils();
				Author auth = new Author();
				string columnPrefix = "Topic";
				if (Convert.ToInt32(row["ReplyId"].ToString()) > 0)
				{
					columnPrefix = "Reply";
					auth.DisplayName = row[columnPrefix + "AuthorDisplayName"].ToString();
				}
				else
				{
					auth.DisplayName = row["TopicAuthorName"].ToString();
				}
				auth.AuthorId = int.Parse(row[columnPrefix + "AuthorId"].ToString());

				auth.LastName = row[columnPrefix + "AuthorLastName"].ToString();
				auth.FirstName = row[columnPrefix + "AuthorFirstName"].ToString();
				auth.Username = row[columnPrefix + "AuthorUsername"].ToString();

				tmp = tmp.Replace("[TOPICURL]", cUtils.TopicURL(row, TabId, ModuleId));
				tmp = tmp.Replace("[FORUMURL]", cUtils.ForumURL(row, TabId, ModuleId));
				if (int.Parse(row["LastAuthorId"].ToString()) == -1)
				{
					try
					{
						tmp = tmp.Replace("[LASTAUTHOR]", UserProfiles.GetDisplayName(ModuleId, true, ForumUser.Profile.IsMod, ForumUser.IsAdmin || ForumUser.IsSuperUser, -1, auth.Username, auth.FirstName, auth.LastName, auth.DisplayName));
					}
					catch (Exception ex)
					{
						tmp = tmp.Replace("[LASTAUTHOR]", "anon");
					}

				}
				else
				{
                    tmp = tmp.Replace("[LASTAUTHOR]", UserProfiles.GetDisplayName(ModuleId, true, ForumUser.Profile.IsMod, ForumUser.IsAdmin || ForumUser.IsSuperUser, int.Parse(row["LastAuthorId"].ToString()), auth.Username, auth.FirstName, auth.LastName, auth.DisplayName));
				}

				if (_canEdit)
				{
					tmp = tmp.Replace("[AF:QUICKEDITLINK]", "<span class=\"af-icon16 af-icon16-gear\" onclick=\"amaf_quickEdit(" + row["TopicId"].ToString() + ");\"></span>");
				}
				else
				{
					tmp = tmp.Replace("[AF:QUICKEDITLINK]", string.Empty);
				}
				//

				tmp = tmp.Replace("[TOPICSTATE]", cUtils.TopicState(row));
				var sAvatar = UserProfiles.GetAvatar(auth.AuthorId, _mainSettings.AvatarWidth, _mainSettings.AvatarHeight);

				tmp = tmp.Replace("[AF:AVATAR]", sAvatar);
				return tmp;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

		}

	}
}


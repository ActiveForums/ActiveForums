//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
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

using System.Text;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class ForumContent
	{
		public enum GroupingOptions: int
		{
			None,
			Categories,
			Forums
		}
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
		private GroupingOptions _GroupBy = GroupingOptions.None;
		public GroupingOptions GroupBy
		{
			get
			{
				return _GroupBy;
			}
			set
			{
				_GroupBy = value;
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
		private string _ItemTemplate = string.Empty;
		public string ItemTemplate
		{
			get
			{
				return _ItemTemplate;
			}
			set
			{
				_ItemTemplate = value;
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
		private bool _IncludeClasses = true;
		public bool IncludeClasses
		{
			get
			{
				return _IncludeClasses;
			}
			set
			{
				_IncludeClasses = value;
			}
		}
		public string Render()
		{
			StringBuilder sb = new StringBuilder();
			Data.CommonDB db = new Data.CommonDB();
			string sHost = Utilities.GetHost();
			if (sHost.EndsWith("/"))
			{
				sHost = sHost.Substring(0, sHost.Length - 1);
			}
			ControlUtils ctlUtils = new ControlUtils();
			string forumPrefix = string.Empty;
			string groupPrefix = string.Empty;
			//Dim _forumGroupId As Integer = -1
			if (ParentForumId == -1)
			{
				ParentForumId = ForumId;
			}
			string groupTemplate = TemplateUtils.GetTemplateSection(ItemTemplate, "[AF:DIR:FORUMGROUP]", "[/AF:DIR:FORUMGROUP]");
			string forumTemplate = TemplateUtils.GetTemplateSection(ItemTemplate, "[AF:DIR:FORUM]", "[/AF:DIR:FORUM]");
			string subForumTemplate = TemplateUtils.GetTemplateSection(ItemTemplate, "[AF:DIR:SUBFORUM]", "[/AF:DIR:SUBFORUM]");
			bool useFriendlyUrl = Utilities.IsRewriteLoaded();
			int currGroup = -1;
			string gtmp = string.Empty;
			string ftmp = string.Empty;
			string subtmp = string.Empty;
			using (IDataReader dr = db.ForumContent_List(PortalId, ModuleId, ForumGroupId, ForumId, ParentForumId))
			{
				//ParentForum Section
				while (dr.Read())
				{
					string sURL = ctlUtils.BuildUrl(TabId, ModuleId, dr["GroupPrefixURL"].ToString(), dr["PrefixURL"].ToString(), int.Parse(dr["ForumGroupId"].ToString()), int.Parse(dr["ForumId"].ToString()), -1, -1, string.Empty, 1, -1, -1);
					if (IncludeClasses)
					{
						sb.Append("<div class=\"fcv-header\"><a href=\"" + sURL + "\"><span>" + dr["ForumName"].ToString() + "</span></a></div>");
					}
					else
					{
						sb.Append("<div><a href=\"" + sURL + "\"><span>" + dr["ForumName"].ToString() + "</span></a></div>");
					}

					forumPrefix = dr["PrefixURL"].ToString();
					groupPrefix = dr["GroupPrefixURL"].ToString();
					//  _forumGroupId = Integer.Parse(dr("ForumGroupId").ToString)
				}
				//SubForums
				dr.NextResult();
				int subForumCount = 0;
				string sSubforums = string.Empty;
				while (dr.Read())
				{
					if (Permissions.HasPerm(dr["CanRead"].ToString(), ForumUser.UserRoles))
					{
						string sURL = ctlUtils.BuildUrl(TabId, ModuleId, dr["GroupPrefixURL"].ToString(), dr["PrefixURL"].ToString(), int.Parse(dr["ForumGroupId"].ToString()), int.Parse(dr["ForumId"].ToString()), -1, -1, string.Empty, 1, -1, -1);
						if (ForumId == int.Parse(dr["ForumId"].ToString()))
						{
							sSubforums += "<li class=\"fcv-selected\">";
						}
						else
						{
							sSubforums += "<li>";
						}

						sSubforums += "<a href=\"" + sURL + "\"><em></em><span>" + dr["ForumName"].ToString() + "</span></a></li>";
						if (IncludeClasses)
						{
							sSubforums += "<li class=\"fcv-desc\">" + dr["ForumDesc"].ToString() + "</li>";
						}
						else
						{
							sSubforums += "<li>" + dr["ForumDesc"].ToString() + "</li>";
						}

					}

				}
				if (! (string.IsNullOrEmpty(sSubforums)))
				{
					if (IncludeClasses)
					{
						sb.Append("<ul class=\"fcv-subforums\">");
					}
					else
					{
						sb.Append("<ul>");
					}

					sb.Append(sSubforums);
					sb.Append("</ul>");
				}
				//Topics in ParentForum
				dr.NextResult();
				string catKey = string.Empty;
				int count = 0;
				int catCount = 0;
				while (dr.Read())
				{
					if (catKey != dr["CategoryName"].ToString() + dr["CategoryId"].ToString())
					{
						if (count > 0)
						{
							sb.Replace("[CATCOUNT]", catCount.ToString());
							sb.Append("</ul></div>");
							count = 0;
							catCount = 0;
						}
						if (IncludeClasses)
						{
							sb.Append("<div class=\"fcv-categorysection\"><div class=\"fcv-categoryname\"><span class=\"fcv-catcount\">[CATCOUNT]</span>" + dr["CategoryName"].ToString() + " </div>");
							sb.Append("<ul class=\"fcv-topicslist\">");
						}
						else
						{
							sb.Append("<div><div><span>[CATCOUNT]</span>" + dr["CategoryName"].ToString() + " </div>");
							sb.Append("<ul>");
						}

						catKey = dr["CategoryName"].ToString() + dr["CategoryId"].ToString();
					}
					//Dim Params As String() = {"aff=" & ForumId, "fcc=" & dr("TopicId").ToString}
					if (TopicId == Convert.ToInt32(dr["TopicId"].ToString()))
					{
						sb.Append("<li class=\"fcv-selected\">");
					}
					else
					{
						sb.Append("<li>");
					}
					catCount += 1;
					//Dim Params As String() = {ParamKeys.ForumId & "=" & ForumId, ParamKeys.TopicId & "=" & TopicId, ParamKeys.ViewType & "=topic"}
					string[] Params = {ParamKeys.TopicId + "=" + dr["TopicId"].ToString()};
					string sTopicURL = ctlUtils.BuildUrl(TabId, ModuleId, groupPrefix, forumPrefix, ForumGroupId, ForumId, int.Parse(dr["TopicId"].ToString()), dr["URL"].ToString(), -1, -1, string.Empty, 1, -1, -1);
					sb.Append("<a href=\"" + sTopicURL + "\"><span>" + dr["Subject"].ToString() + "</span></a></li>");
					if (TopicId > 0)
					{
						if (Convert.ToInt32(dr["TopicId"].ToString()) == TopicId)
						{
							//  RenderTopic(dr)
						}
					}

					count += 1;
				}
				sb.Replace("[CATCOUNT]", catCount.ToString());
				if (count > 0)
				{
					sb.Append("</ul></div>");
				}
				dr.Close();
			}
			return sb.ToString();
		}
		//Private Sub RenderTopic(ByVal dr As IDataRecord)
		//    Dim topicsTemplate As String = TemplateUtils.GetTemplateSection(TopicTemplate, "[TOPIC]", "[/TOPIC]")
		//    Dim replyTemplate As String = TemplateUtils.GetTemplateSection(TopicTemplate, "[REPLIES]", "[/REPLIES]")
		//    TopicTemplate = TemplateUtils.ReplaceSubSection(TopicTemplate, String.Empty, "[REPLIES]", "[/REPLIES]")
		//    topicsTemplate = topicsTemplate.Replace("[BODY]", dr("Body").ToString)
		//    topicsTemplate = topicsTemplate.Replace("[ACTIONS:DELETE]", String.Empty)
		//    topicsTemplate = topicsTemplate.Replace("[ACTIONS:EDIT]", String.Empty)
		//    topicsTemplate = topicsTemplate.Replace("[ACTIONS:ALERT]", String.Empty)
		//    topicsTemplate = topicsTemplate.Replace("[ATTACHMENTS]", String.Empty)
		//    topicsTemplate = topicsTemplate.Replace("[SIGNATURE]", String.Empty)
		//    TopicTemplate = TemplateUtils.ReplaceSubSection(TopicTemplate, topicsTemplate, "[TOPIC]", "[/TOPIC]")


		//    TopicTemplate = TopicTemplate.Replace("[ADDREPLY]", String.Empty)
		//    TopicTemplate = TopicTemplate.Replace("[POSTRATINGBUTTON]", String.Empty)
		//    TopicTemplate = TopicTemplate.Replace("[TOPICSUBSCRIBE]", String.Empty)
		//    Topic = TopicTemplate

		//End Sub
	}
}


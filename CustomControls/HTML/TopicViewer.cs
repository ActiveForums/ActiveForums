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
	public class TopicViewer
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
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			Data.Topics db = new Data.Topics();
			int i = 0;

			using (IDataReader dr = db.TopicWithReplies(PortalId, TopicId, PageIndex, PageSize))
			{
				while (dr.Read())
				{
					Template = ParseTopic(dr, Template);
				}
				dr.NextResult();
				string rtemplate = TemplateUtils.GetTemplateSection(Template, "[REPLIES]", "[/REPLIES]");
				while (dr.Read())
				{
					sb.Append(ParseReply(dr, rtemplate));
				}
				dr.Close();
			}
			Template = TemplateUtils.ReplaceSubSection(Template, sb.ToString(), "[REPLIES]", "[/REPLIES]");
			return Template;
		}
		private string ParseTopic(IDataRecord row, string tmp)
		{
			tmp = ParseDataRow(row, tmp);
			ControlUtils cUtils = new ControlUtils();
			tmp = tmp.Replace("[TOPICURL]", cUtils.TopicURL(row, TabId, ModuleId));
			tmp = tmp.Replace("[FORUMURL]", cUtils.ForumURL(row, TabId, ModuleId));
			tmp = tmp.Replace("[TOPICSTATE]", cUtils.TopicState(row));
			return tmp;
		}
		private string ParseReply(IDataRecord row, string tmp)
		{
			return ParseDataRow(row, tmp);
		}
		private string ParseDataRow(IDataRecord row, string tmp)
		{
			try
			{
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

				//tmp = tmp.Replace("[AVATAR]", "<span style=""background-image:url('/desktopmodules/activesocial/profilepic.ashx?PortalId=" & PortalId.ToString & "&uid=" & row("LastAuthorId").ToString & "&h=26&w=26');""></span>")
				return tmp;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

		}
	}
}
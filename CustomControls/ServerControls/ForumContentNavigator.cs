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

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	[DefaultProperty("Text"), ToolboxData("<{0}:ForumContentNavigator runat=server></{0}:ForumContentNavigator>")]
	public class ForumContentNavigator : WebControl
	{

		private DisplayTemplate _itemTemplate;

		public DisplayTemplate ItemTemplate
		{
			get
			{
				return _itemTemplate;
			}
			set
			{
				_itemTemplate = value;
			}
		}
		private DisplayTemplate _headerTemplate;
		public DisplayTemplate HeaderTemplate
		{
			get
			{
				return _headerTemplate;
			}
			set
			{
				_headerTemplate = value;
			}
		}
		private DisplayTemplate _footerTemplate;
		public DisplayTemplate FooterTemplate
		{
			get
			{
				return _footerTemplate;
			}
			set
			{
				_footerTemplate = value;
			}
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
		private User forumUser = null;
		protected override void Render(HtmlTextWriter writer)
		{
			UserController uc = new UserController();
			forumUser = uc.GetUser(PortalId, ModuleId);
			Controls.ForumContent fd = new Controls.ForumContent();
			fd.ModuleId = ModuleId;
			fd.TabId = TabId;
			fd.PortalId = PortalId;
			fd.ForumUser = forumUser;
			fd.ForumId = ForumId;
			fd.ForumGroupId = ForumGroupId;
			fd.ParentForumId = ParentForumId;
			fd.IncludeClasses = IncludeClasses;
			if (HttpContext.Current.Request.QueryString[ParamKeys.TopicId] != null)
			{
				fd.TopicId = int.Parse(HttpContext.Current.Request.QueryString[ParamKeys.TopicId]);
			}
			if (ItemTemplate != null)
			{
				fd.ItemTemplate = ItemTemplate.Text;
			}
			if (HeaderTemplate != null)
			{
				fd.HeaderTemplate = HeaderTemplate.Text;
			}
			if (FooterTemplate != null)
			{
				fd.FooterTemplate = FooterTemplate.Text;
			}
			writer.Write(fd.Render());
		}

	}

}

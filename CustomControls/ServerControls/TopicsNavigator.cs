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

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	[DefaultProperty("Text"), ToolboxData("<{0}:TopicsNavigator runat=server></{0}:TopicsNavigator>")]
	public class TopicsNavigator : ForumBase
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
		protected override void Render(HtmlTextWriter writer)
		{
			Controls.TopicBrowser tb = new Controls.TopicBrowser();

			tb.PortalId = PortalId;
			tb.ModuleId = ForumModuleId;
			tb.TabId = ForumTabId;
			if (tb.TabId <= 0)
			{
				tb.TabId = int.Parse(Request.QueryString["TabID"]);
			}
			tb.ForumGroupId = ForumGroupId;
			tb.ForumId = ForumId;

			if (ForumId > 0)
			{
				if (Permissions.HasAccess(ForumInfo.Security.View, ForumUser.UserRoles))
				{
					tb.ForumIds = ForumId.ToString();
				}
				else
				{
					writer.Write(string.Empty);
					return;
				}
			}
			else
			{
				tb.ForumIds = UserForumsList;
			}
			if (Request.QueryString["atg"] != null && SimulateIsNumeric.IsNumeric(Request.QueryString["atg"]))
			{
				tb.TagId = int.Parse(Request.QueryString["atg"]);

			}
			if (Request.QueryString["act"] != null && SimulateIsNumeric.IsNumeric(Request.QueryString["act"]))
			{
				tb.CategoryId = int.Parse(Request.QueryString["act"]);
			}
			tb.ForumUser = ForumUser;
			tb.PageIndex = PageId;
			tb.PageSize = MainSettings.PageSize;
			tb.Template = ItemTemplate.Text;
			tb.HeaderTemplate = HeaderTemplate.Text;
			tb.FooterTemplate = FooterTemplate.Text;
			tb.ImagePath = Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MainSettings.Theme);
			tb.TopicId = TopicId;
			tb.TimeZoneOffset = TimeZoneOffset;
			writer.Write(tb.Render());
		}

	}

}

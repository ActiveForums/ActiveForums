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

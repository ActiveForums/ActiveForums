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
	[DefaultProperty("Text"), ToolboxData("<{0}:TopicViewer runat=server></{0}:TopicViewer>")]
	public class TopicNavigator : ForumBase
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
		protected override void Render(HtmlTextWriter writer)
		{
			Controls.TopicViewer tb = new Controls.TopicViewer();

			tb.PortalId = PortalId;
			tb.ModuleId = ForumModuleId;
			tb.TabId = ForumTabId;
			tb.PageIndex = PageId;
			tb.PageSize = MainSettings.PageSize;
			tb.Template = ItemTemplate.Text;
			//tb.HeaderTemplate = HeaderTemplate.Text
			//tb.FooterTemplate = FooterTemplate.Text
			tb.TopicId = TopicId;
			tb.TimeZoneOffset = TimeZoneOffset;
			writer.Write(tb.Render());
		}

	}
}


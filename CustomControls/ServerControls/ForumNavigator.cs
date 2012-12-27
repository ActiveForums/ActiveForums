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
	[DefaultProperty("Text"), ToolboxData("<{0}:ForumNavigator runat=server></{0}:ForumNavigator>")]
	public class ForumNavigator : WebControl
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
		private User forumUser = null;
		protected override void Render(HtmlTextWriter writer)
		{
			UserController uc = new UserController();
			forumUser = uc.GetUser(PortalId, ModuleId);
			Controls.ForumDirectory fd = new Controls.ForumDirectory();
			fd.ModuleId = ModuleId;
			fd.TabId = TabId;
			fd.PortalId = PortalId;
			fd.ForumUser = forumUser;
			if (ItemTemplate != null)
			{
				fd.Template = ItemTemplate.Text;
			}
			writer.Write(fd.Render());
		}

	}

}

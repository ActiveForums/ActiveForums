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
	[ToolboxData("<{0}:TopMembers runat=server></{0}:TopMembers>")]
	public class TopMembers : WebControl
	{
		private int _siteId = -1;
		private int _rows = 10;
		private DisplayTemplate _itemTemplate;
		private DisplayTemplate _headerTemplate;
		private DisplayTemplate _footerTemplate;
		public int SiteId
		{
			get
			{
				return _siteId;
			}
			set
			{
				_siteId = value;
			}
		}
		public int Rows
		{
			get
			{
				return _rows;
			}
			set
			{
				_rows = value;
			}
		}
		[Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
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
		[Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
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
		[Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
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
			string sHeaderTemplate = string.Empty;
			string sFooterTemplate = string.Empty;
			if (HeaderTemplate != null)
			{
				sHeaderTemplate = HeaderTemplate.Text;
			}
			if (FooterTemplate != null)
			{
				sFooterTemplate = FooterTemplate.Text;
			}
			string sTemplate = "[DISPLAYNAME]";
			Data.Common db = new Data.Common();
			IDataReader dr = db.TopMembers_Get(SiteId, Rows);
			if (ItemTemplate != null)
			{
				sTemplate = ItemTemplate.Text;
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (string.IsNullOrEmpty(CssClass))
			{
				CssClass = "aflist2";
			}
			while (dr.Read())
			{
				string sOut = sTemplate;
				sOut = sOut.Replace("[DISPLAYNAME]", dr["DisplayName"].ToString());
				sb.Append(sOut);
			}
			dr.Close();
			dr.Dispose();
			if (! (string.IsNullOrEmpty(sb.ToString())))
			{
				writer.Write(sHeaderTemplate);
				writer.Write(sb.ToString());
				writer.Write(sFooterTemplate);
			}
		}

	}

}

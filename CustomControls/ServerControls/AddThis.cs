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
	[ToolboxData("<{0}:AddThis runat=server></{0}:AddThis>")]
	public class AddThis : WebControl
	{
		private string _addThisId;
		private string _title;
		public string AddThisId
		{
			get
			{
				return _addThisId;
			}
			set
			{
				_addThisId = value;
			}
		}
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}
		protected override void Render(HtmlTextWriter writer)
		{
			string sURL = HttpContext.Current.Request.RawUrl;
			string tmp = DataCache.GetTemplate("AddThis.txt");
			if (! (string.IsNullOrEmpty(AddThisId)))
			{
				tmp = tmp.Replace("[USERNAME]", AddThisId.Replace("'", "\\'"));
				tmp = tmp.Replace("[URL]", sURL);
				tmp = tmp.Replace("[TITLE]", Title.Replace("'", "\\'"));
				writer.Write(tmp);
			}

		}

	}

}

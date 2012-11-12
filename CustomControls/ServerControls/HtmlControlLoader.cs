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
	[DefaultProperty("Text"), ToolboxData("<{0}:HtmlControlLoader runat=server></{0}:HtmlControlLoader>")]
	public class HtmlControlLoader : Control
	{
		public string ControlId {get; set;}
		public string Height {get; set;}
		public string Width {get; set;}
		public string Name {get; set;}
		public string FilePath {get; set;}
		protected override void Render(HtmlTextWriter writer)
		{
			this.EnableViewState = false;
			FilePath = HttpContext.Current.Server.MapPath(FilePath);
			string sControl = Utilities.GetFile(FilePath);
			sControl = sControl.Replace("{id}", ControlId);
			sControl = sControl.Replace("{height}", Height);
			sControl = sControl.Replace("{width}", Width);
			sControl = sControl.Replace("{name}", Name);
			writer.Write(sControl);
		}

	}
}


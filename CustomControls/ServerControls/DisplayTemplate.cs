using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Web.UI;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class DisplayTemplate : System.Web.UI.Control
	{
		[DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text
		{
			get
			{
				if (this.Controls.Count > 0 && this.Controls[0] is LiteralControl)
				{
					return ((LiteralControl)(this.Controls[0])).Text;
				}
				return "";
			}
			set
			{
				this.Controls.Clear();
				this.Controls.Add(new LiteralControl(value));
			}
		}
	}
}


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
	[DefaultProperty("message"), ToolboxData("<{0}:InfoMessage runat=server></{0}:InfoMessage>")]
	public class InfoMessage : CompositeControl
	{
#region Private Members
		private string _message;
		private string _header;
		private string _returnUrl;
#endregion
#region Public Properties
		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}
		public string Header
		{
			get
			{
				return _header;
			}
			set
			{
				_header = value;
			}
		}
		public string ReturnUrl
		{
			get
			{
				return _returnUrl;
			}
			set
			{
				_returnUrl = value;
			}
		}
#endregion
#region Protected Methods
		protected override void RenderContents(HtmlTextWriter writer)
		{
			writer.Write(Message);
		}
#endregion
	}
}

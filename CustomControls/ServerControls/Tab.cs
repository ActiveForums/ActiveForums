//© 2004 - 2007 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class Tab
	{
		private string _text;
		private string _CSSClass;
		private string _onClick = string.Empty;
		private string _controlKey = string.Empty;
		private TabContent _tabContent;

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}
		public string CSSClass
		{
			get
			{
				return _CSSClass;
			}
			set
			{
				_CSSClass = value;
			}
		}
		public string OnClientClick
		{
			get
			{
				return _onClick;
			}
			set
			{
				_onClick = value;
			}
		}
		public string ControlKey
		{
			get
			{
				return _controlKey;
			}
			set
			{
				_controlKey = value;
			}
		}

		public TabContent Content
		{
			get
			{
				return _tabContent;
			}
			set
			{
				_tabContent = value;
			}
		}

	}
	public class TabContent : System.Web.UI.Control
	{
	}
}

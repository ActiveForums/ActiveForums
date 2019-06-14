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

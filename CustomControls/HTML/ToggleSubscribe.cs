//
// Active Forums - http://www.dnnsoftware.com
// Copyright (c) 2013
// by DNN Corp.
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

using System.Text;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class ToggleSubscribe
	{
		private int _ToggleMode = 0;
		public int ToggleMode
		{
			get
			{
				return _ToggleMode;
			}
			set
			{
				_ToggleMode = value;
			}
		}
		private int _DisplayMode = 0;
		public int DisplayMode
		{
			get
			{
				return _DisplayMode;
			}
			set
			{
				_DisplayMode = value;
			}
		}
		private int _ForumId = -1;
		public int ForumId
		{
			get
			{
				return _ForumId;
			}
			set
			{
				_ForumId = value;
			}
		}
		private int _TopicId = -1;
		public int TopicId
		{
			get
			{
				return _TopicId;
			}
			set
			{
				_TopicId = value;
			}
		}
		private bool _Checked = false;
		public bool Checked
		{
			get
			{
				return _Checked;
			}
			set
			{
				_Checked = value;
			}
		}
		private string _Text = string.Empty;
		public string Text
		{
			get
			{
				return _Text;
			}
			set
			{
				_Text = value;
			}
		}
		private int _UserId = -1;
		public int UserId
		{
			get
			{
				return _UserId;
			}
			set
			{
				_UserId = value;
			}
		}
		private string _ImageURL = string.Empty;
		public string ImageURL
		{
			get
			{
				return _ImageURL;
			}
			set
			{
				_ImageURL = value;
			}
		}

		public ToggleSubscribe(int m, int f, int t)
		{
			ToggleMode = m;
			ForumId = f;
			TopicId = t;
		}
		//amaf_topicSubscribe
		public string Render()
		{
			StringBuilder sb = new StringBuilder();
			if (DisplayMode == 0)
			{
				sb.Append("<span class=\"afnormal\">");
				sb.Append("<input id=\"amaf-chk-subs\" type=\"checkbox\" ");
				if (Checked)
				{
					sb.Append("checked=\"checked\" ");
				}
				if (ToggleMode == 0)
				{
					sb.Append(" onclick=\"amaf_forumSubscribe(" + ForumId + ");\" />");
				}
				else
				{
					sb.Append(" onclick=\"amaf_topicSubscribe(" + ForumId + "," + TopicId + ");\" />");
				}

				sb.Append("<label for=\"amaf-chk-subs\">" + Text + "</label>");
				sb.Append("</span>");
			}
			else
			{
				sb.Append("<img src=\"" + ImageURL + "\" border=\"0\" alt=\"" + Text + "\" onclick=\"amaf_forumSubscribe(" + ForumId + ", " + UserId + ");\" id=\"amaf-sub-" + ForumId + "\" />");
			}

			return sb.ToString();
		}

	}
}


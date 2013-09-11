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

using System.ComponentModel;
using System.Web.UI;
using System.Web;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class ControlsBase : ForumBase
	{
		private string _template;
		private string _templateFile;
		private string _currentView = "forumview";
		private bool _parseTemplate = false;
		[Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
		public string DisplayTemplate
		{
			get
			{
				if (string.IsNullOrEmpty(_template) && ! (string.IsNullOrEmpty(TemplateFile)))
				{
					if (! (string.IsNullOrEmpty(ControlConfig.TemplatePath)))
					{
						_template = ControlConfig.TemplatePath + TemplateFile;
					}
					else
					{
						_template = TemplateFile;
					}
					_template = Utilities.GetTemplate(Page.ResolveUrl(_template));
					_template = Utilities.ParseTokenConfig(_template, "default", ControlConfig);
				}
				return _template;
			}
			set
			{
				_template = value;
			}
		}
		public string CurrentView
		{
			get
			{
				return _currentView;
			}
			set
			{
				_currentView = value;
			}
		}
		public bool ParseTemplateFile
		{
			get
			{
				return _parseTemplate;
			}
			set
			{
				_parseTemplate = value;
			}
		}
		public int DataPageId
		{
			get
			{
				if (HttpContext.Current.Request.QueryString[ParamKeys.PageId] == null)
				{
					return 1;
				}
				else
				{
					return int.Parse(HttpContext.Current.Request.QueryString[ParamKeys.PageId].ToString());
				}
			}
		}
        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (ParseTemplateFile)
			{
				if (! (string.IsNullOrEmpty(DisplayTemplate)))
				{
					Control ctl = Page.ParseControl(DisplayTemplate);
					LinkControls(ctl.Controls);
					this.Controls.Add(ctl);
				}
			}
		}
		private void LinkControls(ControlCollection ctrls)
		{
			foreach (Control ctrl in ctrls)
			{
				if (ctrl is Controls.ForumRow)
				{
					((Controls.ForumRow)ctrl).UserRoles = ForumUser.UserRoles;
				}
				if (ctrl is Controls.ControlsBase)
				{
					((Controls.ControlsBase)ctrl).ControlConfig = this.ControlConfig;
					((Controls.ControlsBase)ctrl).ForumData = ForumData;
					((Controls.ControlsBase)ctrl).ForumInfo = ForumInfo;
				}
				if (ctrl.Controls.Count > 0)
				{
					LinkControls(ctrl.Controls);
				}
			}
		}
	}
}
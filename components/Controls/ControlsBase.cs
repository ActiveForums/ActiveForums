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
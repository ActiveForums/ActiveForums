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
	[ToolboxData("<{0}:ForumGroupRepeater runat=server></{0}:ForumGroupRepeater>")]
	public class ForumGroupRepeater : ControlsBase
	{
		public enum RepeatDirections: int
		{
			Vertical,
			Horizontal
		}
		private RepeatDirections _repeatDirection;
		private int _repeatColumns = 1;
		private string _headerTemplate;
		private string _footerTemplate;
		private string _noResults;
		private int _toggleBehavior = 0;
		public RepeatDirections RepeatDirection
		{
			get
			{
				return _repeatDirection;
			}
			set
			{
				_repeatDirection = value;
			}
		}
		public int RepeatColumns
		{
			get
			{
				return _repeatColumns;
			}
			set
			{
				_repeatColumns = value;
			}
		}
		[Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
		public string HeaderTemplate
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
		public string FooterTemplate
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
		[Description("Template for display"), PersistenceMode(PersistenceMode.InnerProperty)]
		public string NoResultsTemplate
		{
			get
			{
				return _noResults;
			}
			set
			{
				_noResults = value;
			}
		}
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.EnableViewState = false;
		}
		public int ToggleBehavior
		{
			get
			{
				return _toggleBehavior;
			}
			set
			{
				_toggleBehavior = value;
			}
		}
		protected override void Render(HtmlTextWriter writer)
		{
			//writer.Write(Text)
			writer.Write(HeaderTemplate);
			int i = 0;
			if (ForumData != null)
			{
				string tmp = DisplayTemplate;
				System.Xml.XmlNodeList xGroups = ForumData.SelectNodes("//groups/group");
				ForumDisplay fd = null;
				foreach (System.Xml.XmlNode xNode in xGroups)
				{
					int groupId = int.Parse(xNode.Attributes["groupid"].Value.ToString());
					fd = new ForumDisplay();
					fd.DisplayTemplate = this.DisplayTemplate;
					fd.ForumGroupId = groupId;
					fd.ControlConfig = this.ControlConfig;
					fd.ModuleConfiguration = this.ModuleConfiguration;
					//fd.ForumData = ForumData
					if (i == 0 && ToggleBehavior == 1)
					{
						fd.ToggleBehavior = 0;
					}
					else if (i > 0 && ToggleBehavior == 1)
					{
						fd.ToggleBehavior = 1;
					}
					this.Controls.Add(fd);
					fd.RenderControl(writer);
					i += 1;
				}
			}
			else
			{
				writer.Write(NoResultsTemplate);
			}
			writer.Write(FooterTemplate);
		}


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);


		}
	}

}


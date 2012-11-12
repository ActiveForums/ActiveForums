//© 2004 - 2007 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class ActiveTabsDesigner : System.Web.UI.Design.ControlDesigner
	{
		private ActiveTabs m_ControlInstance;

		public override void Initialize(System.ComponentModel.IComponent component)
		{
			m_ControlInstance = (ActiveTabs)component;

			base.Initialize(component);
		}
		public override string GetDesignTimeHtml()
		{
			//Return MyBase.GetDesignTimeHtml()
			string template = "...";
			string message = "test";
			string markup = string.Format(template, "name", base.Component.Site, base.GetDesignTimeHtml(), message);
			return markup;
			//Return CreatePlaceHolderDesignTimeHtml()
		}

		protected override string GetErrorDesignTimeHtml(System.Exception e)
		{
			//Return MyBase.GetErrorDesignTimeHtml(e)
			return CreatePlaceHolderDesignTimeHtml("Error!");
		}

		public override System.Web.UI.Design.TemplateGroupCollection TemplateGroups
		{
			get
			{
				return base.TemplateGroups;
			}
		}


		public override string GetDesignTimeHtml(System.Web.UI.Design.DesignerRegionCollection regions)
		{
			string message = "This <b>TabularMultiView</b> control represents the look and feel of the Multiview control, but with a tabular like interface. Each tab corresponds to a View. Use the properties on each tab (TabularView) to modify each Tab. <hr/> <small> Control Library - Tiger</small>";
			return CreatePlaceHolderDesignTimeHtml(message);
		}


	}
}
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
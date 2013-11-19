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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DotNetNuke.Modules.ActiveForums.Controls
{
	[DefaultProperty("Text"), ToolboxData("<{0}:UserStats runat=server></{0}:UserStats>")]
	public class UserStats : WebControl
	{

		private DisplayTemplate _template;
		private int _userId = -1;
		private int _moduleId = -1;
		public DisplayTemplate Template
		{
			get
			{
				return _template;
			}
			set
			{
				_template = value;
			}
		}
		public int UserId
		{
			get
			{
				return _userId;
			}
			set
			{
				try
				{
					_userId = value;
				}
				catch (Exception ex)
				{
					_userId = -1;
				}

			}
		}
		public int ModuleId
		{
			get
			{
				return _moduleId;
			}
			set
			{
				_moduleId = value;
			}
		}
		protected override void RenderContents(HtmlTextWriter writer)
		{
			if (UserId == -1)
			{
				return;
			}
			try
			{
				string output = string.Empty;
				DotNetNuke.Entities.Portals.PortalSettings ps = (DotNetNuke.Entities.Portals.PortalSettings)(Context.Items["PortalSettings"]);
				DotNetNuke.Entities.Users.UserInfo cu = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
				string imagePath = string.Empty;
				int portalId = ps.PortalId;
				string tmp = string.Empty;
				if (Template == null)
				{
					tmp = "<span class=\"aslabelsmbold\">[RESX:Posts]:</span> [AF:PROFILE:POSTCOUNT]<br />" + "<span class=\"aslabelsmbold\">[RESX:RankName]:</span> [AF:PROFILE:RANKNAME]<br />" + "<span class=\"aslabelsmbold\">[RESX:RankDisplay]:</span> [AF:PROFILE:RANKDISPLAY] <br />" + "<span class=\"aslabelsmbold\">[RESX:LastUpdate]:</span> [AF:PROFILE:DATELASTACTIVITY:d] <br />" + "<span class=\"aslabelsmbold\">[RESX:MemberSince]:</span> [AF:PROFILE:DATECREATED:d]";
				}
				else
				{
					tmp = Template.Text;
				}
				if (ModuleId == -1)
				{
					DotNetNuke.Entities.Modules.ModuleController mc = new DotNetNuke.Entities.Modules.ModuleController();
					DotNetNuke.Entities.Tabs.TabController tc = new DotNetNuke.Entities.Tabs.TabController();
					DotNetNuke.Entities.Tabs.TabInfo ti = null;
					foreach (DotNetNuke.Entities.Modules.ModuleInfo mi in mc.GetModules(portalId))
					{
						if (mi.DesktopModule.ModuleName.ToUpperInvariant() == "Active Forums".ToUpperInvariant())
						{
							ModuleId = mi.ModuleID;
							break;
						}
					}
				}
				output = TemplateUtils.ParseProfileTemplate(tmp, UserId, portalId, ModuleId, cu.UserID, ps.TimeZoneOffset);
				output = Utilities.LocalizeControl(output);
				writer.Write(output);
			}
			catch (Exception ex)
			{
				writer.Write(ex.Message);
			}

		}

	}

}


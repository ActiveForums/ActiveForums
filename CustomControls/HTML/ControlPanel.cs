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
	public class ControlPanel
	{
		public int PortalId {get; set;}
		public int ModuleId {get; set;}

		public ControlPanel(int _portalId, int _moduleId)
		{
			PortalId = _portalId;
			ModuleId = _moduleId;
		}

		public string TemplatesOptions(Templates.TemplateTypes templateType)
		{
			StringBuilder sb = new StringBuilder();
			TemplateController tc = new TemplateController();
			sb.Append("<option value=\"0\">[RESX:Default]</option>");
			List<TemplateInfo> lc = tc.Template_List(PortalId, ModuleId, templateType);
			foreach (TemplateInfo l in lc)
			{
				sb.Append("<option value=\"" + l.TemplateId + "\">" + l.Subject + "</option>");
			}
			return sb.ToString();
		}
		public string ForumGroupOptions()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<option value=\"-1\">" + Utilities.GetSharedResource("DropDownSelect", true) + "</option>");
			using (IDataReader dr = DataProvider.Instance().Forums_List(PortalId, ModuleId, -1, -1, false))
			{
				int tmpGroupId = -1;
				while (dr.Read())
				{
					if (! (tmpGroupId == Convert.ToInt32(dr["ForumGroupId"])))
					{
						sb.Append("<option value=\"GROUP" + dr["ForumGroupId"].ToString() + "\">" + dr["GroupName"].ToString() + "</option>");
						tmpGroupId = Convert.ToInt32(dr["ForumGroupId"]);
					}
					if (! (Convert.ToInt32(dr["ForumId"]) == 0))
					{
						if (Convert.ToInt32(dr["ParentForumID"]) == 0)
						{
							sb.Append("<option value=\"FORUM" + dr["ForumId"].ToString() + "\"> - " + dr["ForumName"].ToString() + "</option>");
						}
					}
				}
				dr.Close();
			}


			return sb.ToString();
		}
		public string BindRolesForSecurityGrid(string rootPath)
		{
			StringBuilder sb = new StringBuilder();

			DotNetNuke.Security.Roles.RoleController rc = new DotNetNuke.Security.Roles.RoleController();
			foreach (DotNetNuke.Security.Roles.RoleInfo ri in rc.GetPortalRoles(PortalId))
			{
				sb.Append("<option value=\"" + ri.RoleID + "\">" + ri.RoleName + "</option>");
            }

#if !SKU_LITE
            if (System.IO.File.Exists(rootPath + "bin\\active.modules.social.dll"))
			{
				Modules.ActiveForums.Social social = new Modules.ActiveForums.Social();
				using (IDataReader dr = social.ActiveSocialListGroups(PortalId))
				{
					while (dr.Read())
					{
						sb.Append("<optgroup label=\"" + dr["GroupName"].ToString() + "\">");
						sb.Append("<option value=\"" + dr["GroupId"].ToString() + ":0\">Group Admin</option>");
						sb.Append("<option value=\"" + dr["GroupId"].ToString() + ":1\">Group Member</option>");
						sb.Append("</optgroup>");

					}
					dr.Close();
				}
			}
#endif

			return sb.ToString();
		}
	}
}

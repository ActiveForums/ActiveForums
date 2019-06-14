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

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_profile : ForumBase
    {

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            string sDisplayName = string.Empty;
            int tUid = -1;
            if (Request.Params["UID"] != null)
            {
                if (SimulateIsNumeric.IsNumeric(Request.Params["UID"]))
                {
                    tUid = Convert.ToInt32(Request.Params["UID"]);
                    DotNetNuke.Entities.Users.UserController uc = new DotNetNuke.Entities.Users.UserController();
                    DotNetNuke.Entities.Users.UserInfo ui = uc.GetUser(PortalId, tUid);
                    if (ui != null)
                    {
                        sDisplayName = UserProfiles.GetDisplayName(ModuleId, ui.UserID, ui.Username, ui.FirstName, ui.LastName, ui.DisplayName);
                    }

                }

            }
            else
            {
                tUid = UserId;
                sDisplayName = UserProfiles.GetDisplayName(ModuleId, UserId, UserInfo.Username, UserInfo.FirstName, UserInfo.LastName, UserInfo.DisplayName);
            }
            lblHeader.Text = string.Format(Utilities.GetSharedResource("[RESX:ProfileForUser]"), sDisplayName);
            if (MainSettings.UseSkinBreadCrumb)
            {
                Environment.UpdateBreadCrumb(Page.Controls, "<a href=\"" + Utilities.NavigateUrl(TabId, "", new string[] { "afv=profile", "uid=" + tUid.ToString() }) + "\">" + lblHeader.Text + "</a>");
            }
            DotNetNuke.Framework.CDefault tempVar = this.BasePage;
            Environment.UpdateMeta(ref tempVar, "[VALUE] - " + lblHeader.Text, "[VALUE]", "[VALUE]");
            SettingsBase ctl = null;
            ctl = (SettingsBase)(new DotNetNuke.Modules.ActiveForums.Controls.UserProfile());
            ctl.ModuleConfiguration = this.ModuleConfiguration;
            if (!(this.Params == string.Empty))
            {
                ctl.Params = this.Params;
            }
            plhProfile.Controls.Add(ctl);
        }
    }
}

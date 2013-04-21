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

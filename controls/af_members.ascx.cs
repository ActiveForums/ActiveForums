using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_members : ForumBase
    {

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

/*
#if !SKU_LITE
            try
            {
                Social social = new Social();
                if (MainSettings.ProfileType == ProfileTypes.Social)
                {
                    int memberTabId = social.GetMemberTabId(PortalId);
                    if (memberTabId > -1)
                    {
                        Response.Redirect(Utilities.NavigateUrl(memberTabId));
                    }
                }
            }
            catch (Exception ex)
            {

            }

#endif
*/


            lblHeader.Text = Utilities.GetSharedResource("[RESX:MemberDirectory]");
            bool bCanLoad = false;
            string sMode = MainSettings.MemberListMode;
            if (!UserInfo.IsSuperUser)
            {
                if (sMode == "DISABLED")
                {
                    Response.Redirect(NavigateUrl(TabId));
                }
                if (!Request.IsAuthenticated & (sMode == "ENABLEDREG" || sMode == "ENABLEDMOD"))
                {
                    Response.Redirect(NavigateUrl(TabId));
                }
                else if (Request.IsAuthenticated && sMode == "ENABLEDMOD" && !UserIsMod)
                {
                    Response.Redirect(NavigateUrl(TabId));
                }
            }

            SettingsBase ctl = null;
            ctl = (SettingsBase)(new DotNetNuke.Modules.ActiveForums.Controls.Members());
            ctl.ModuleConfiguration = this.ModuleConfiguration;
            if (!(this.Params == string.Empty))
            {
                ctl.Params = this.Params;
            }
            DotNetNuke.Framework.CDefault tempVar = this.BasePage;
            Environment.UpdateMeta(ref tempVar, "[VALUE] - " + lblHeader.Text, "[VALUE]", "[VALUE]");
            plhMembers.Controls.Add(ctl);


        }
    }
}

using System;
using DotNetNuke.Modules.ActiveForums.Controls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class ActiveForums : ForumBase, Entities.Modules.IActionable
    {


        #region DNN Actions
        public Entities.Modules.Actions.ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new Entities.Modules.Actions.ModuleActionCollection
                                  {
                                      {
                                          GetNextActionID(), Utilities.GetSharedResource("[RESX:ControlPanel]"),
                                          Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl(), false,
                                          Security.SecurityAccessLevel.Edit, true, false
                                      }
                                  };
                //Actions.Add(GetNextActionID, Utilities.GetSharedResource("ForumEditor"), Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl("ForumEditor"), False, Security.SecurityAccessLevel.Edit, True, False)
                return Actions;
            }
        }
        #endregion

        #region Event Handlers

        protected override void  OnLoad(EventArgs e)
        {
 	         base.OnLoad(e);

            string defaultView = "classic";
            if (Settings["amafDefaultView"] != null)
            {
                if (!(string.IsNullOrEmpty(Settings["amafDefaultView"].ToString())))
                {
                    defaultView = Settings["amafDefaultView"].ToString();
                }
            }
            if (!Page.IsPostBack)
            {
                plhAF.Controls.Clear();
            }

            if (defaultView.ToLowerInvariant() == "advanced")
            {
                var ctl2 = (ControlsBase)(LoadControl("~/desktopmodules/activeforums/advanced.ascx"));
                ctl2.ModuleConfiguration = ModuleConfiguration;
                plhAF.Controls.Add(ctl2);
            }
            else if (PortalSettings.UserTabId == PortalSettings.ActiveTab.ParentId)
            {
                int userId = Request.QueryString["UserId"] == null ? UserInfo.UserID : Convert.ToInt32(Request.QueryString["UserId"]);
                if (userId == UserInfo.UserID || UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    var ctl2 = (SettingsBase)(LoadControl("~/desktopmodules/activeforums/controls/profile_mypreferences.ascx"));
                    ctl2.ModuleConfiguration = ModuleConfiguration;
                    ctl2.LocalResourceFile = "~/desktopmodules/activeforums/app_localresources/sharedresources.resx";
                    plhAF.Controls.Add(ctl2);
                }
            }
            else
            {
                var ctl = (ForumBase)(LoadControl("~/desktopmodules/activeforums/classic.ascx"));
                ctl.ModuleConfiguration = ModuleConfiguration;
                plhAF.Controls.Add(ctl);
            }
        }


        #endregion
    }
}
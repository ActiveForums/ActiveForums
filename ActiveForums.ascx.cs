using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class ActiveForums : ForumBase, IActionable
    {
        #region DNN Actions

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                {
                    {
                        GetNextActionID(), Utilities.GetSharedResource("[RESX:ControlPanel]"),
                        ModuleActionType.AddContent, string.Empty, string.Empty, EditUrl(), false,
                        Security.SecurityAccessLevel.Edit, true, false
                    }
                };

                return actions;
            }
        }

        #endregion

        #region Event Handlers

        protected override void  OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // If the forum instance is on the user profile page, load the users prefs control
            if (PortalSettings.UserTabId == PortalSettings.ActiveTab.ParentId)
            {
                int userId;

                userId = int.TryParse(Request.QueryString["UserId"], out userId) ? userId : UserInfo.UserID;

                // Users can only view thier own settings unless they are admin.
                if (userId == UserInfo.UserID || UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    var userPrefsCtl = (SettingsBase)(LoadControl("~/desktopmodules/activeforums/controls/profile_mypreferences.ascx"));
                    userPrefsCtl.ModuleConfiguration = ModuleConfiguration;
                    userPrefsCtl.LocalResourceFile = "~/desktopmodules/activeforums/app_localresources/sharedresources.resx";
                    plhAF.Controls.Add(userPrefsCtl);
                }

                return;
            }

            // Otherwise, just load the normal forum control
            var ctl = (ForumBase)(LoadControl("~/desktopmodules/activeforums/classic.ascx"));
            ctl.ModuleConfiguration = ModuleConfiguration;
            plhAF.Controls.Add(ctl);

        }

        #endregion
    }
}
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

            if (Request.QueryString["afgt"] == "afprofile" || PortalSettings.UserTabId == PortalSettings.ActiveTab.ParentId)
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
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

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class profile_adminsettings : ProfileBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cbAdmin.CallbackEvent += cbAdmin_Callback;
        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            UserProfileInfo ui = UserProfile;
            if (ui == null & UID > 0)
            {
                UserController up = new UserController();
                ui = up.GetUser(PortalId, ForumModuleId, UID).Profile;
            }

            if (ui != null)
            {
                txtRewardPoints.Text = ui.RewardPoints.ToString();
                txtUserCaption.Text = ui.UserCaption;
                chkDisableSignature.Checked = ui.SignatureDisabled;
                chkDisableAttachments.Checked = ui.AttachDisabled;
                chkDisableAvatar.Checked = ui.AvatarDisabled;
                chkMonitor.Checked = ui.AdminWatch;
                drpDefaultTrust.SelectedIndex = drpDefaultTrust.Items.IndexOf(drpDefaultTrust.Items.FindByValue(ui.TrustLevel.ToString()));
                txtRewardPoints.Attributes.Add("onkeypress", "return onlyNumbers(event);");
            }

        }

        private void cbAdmin_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            if (!(CurrentUserType == CurrentUserTypes.Anon) && !(CurrentUserType == CurrentUserTypes.Auth))
            {
                UserProfileController upc = new UserProfileController();
                UserController uc = new UserController();
                UserProfileInfo upi = uc.GetUser(PortalId, ForumModuleId, UID).Profile;
                if (upi != null)
                {
                    upi.RewardPoints = Convert.ToInt32(e.Parameters[1]);
                    upi.UserCaption = e.Parameters[2].ToString();
                    upi.SignatureDisabled = Convert.ToBoolean(e.Parameters[3]);
                    upi.AvatarDisabled = Convert.ToBoolean(e.Parameters[4]);
                    upi.TrustLevel = Convert.ToInt32(e.Parameters[5]);
                    upi.AdminWatch = Convert.ToBoolean(e.Parameters[6]);
                    upi.AttachDisabled = Convert.ToBoolean(e.Parameters[7]);
                    upc.Profiles_Save(upi);
                }
            }
        }
    }
}

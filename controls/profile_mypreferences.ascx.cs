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

using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class profile_mypreferences : SettingsBase
    {
        public int UID { get; set; }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            btnSave.Click += new System.EventHandler(btnSave_Click);


            if (Request.QueryString["UserId"] == null)
            {
                UID = UserInfo.UserID;
            }
            else
            {
                UID = Convert.ToInt32(Request.QueryString["UserId"]);
            }
            //If UID <> UserInfo.UserID And Not UserInfo.IsInRole(PortalSettings.AdministratorRoleName) Then

            //End If
            UserProfileInfo ui = null;
            if (ui == null & UID > 0)
            {
                UserController up = new UserController();
                ui = up.GetUser(PortalId, ForumModuleId, UID).Profile;
            }
            if (ui != null && !Page.IsPostBack )
            {

                drpPrefDefaultSort.SelectedIndex = drpPrefDefaultSort.Items.IndexOf(drpPrefDefaultSort.Items.FindByValue(ui.PrefDefaultSort.Trim()));
                drpPrefPageSize.SelectedIndex = drpPrefPageSize.Items.IndexOf(drpPrefPageSize.Items.FindByValue(ui.PrefPageSize.ToString()));

                chkPrefJumpToLastPost.Checked = ui.PrefJumpLastPost;
                chkPrefTopicSubscribe.Checked = ui.PrefTopicSubscribe;
                //chkPrefUseAjax.Checked = .PrefUseAjax
                chkPrefBlockAvatars.Checked = ui.PrefBlockAvatars;
                chkPrefBlockSignatures.Checked = ui.PrefBlockSignatures;
                txtSignature.Text = ui.Signature;
            }
        }

        public string GetString(string key)
        {
            return Utilities.GetSharedResource(key);
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            if (UserId == UID || (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.SuperUser))
            {
                UserProfileController upc = new UserProfileController();
                UserController uc = new UserController();
                UserProfileInfo upi = uc.GetUser(PortalId, ForumModuleId, UID).Profile;
                if (upi != null)
                {
                    upi.PrefDefaultSort = Utilities.XSSFilter(drpPrefDefaultSort.SelectedItem.Value, true);
                    upi.PrefPageSize = Convert.ToInt32(((Convert.ToInt32(drpPrefPageSize.SelectedValue) < 5) ? 5 : Convert.ToInt32(drpPrefPageSize.SelectedValue)));
                    upi.PrefDefaultShowReplies = false;
                    upi.PrefJumpLastPost = chkPrefJumpToLastPost.Checked;
                    upi.PrefTopicSubscribe = chkPrefTopicSubscribe.Checked;
                    upi.PrefSubscriptionType = SubscriptionTypes.Instant;
                    upi.PrefUseAjax = false;
                    upi.PrefBlockAvatars = chkPrefBlockAvatars.Checked;
                    upi.PrefBlockSignatures = chkPrefBlockSignatures.Checked;
                    if (MainSettings.AllowSignatures == 1 || MainSettings.AllowSignatures == 0)
                    {
                        upi.Signature = Utilities.XSSFilter(txtSignature.Text, true);
                        upi.Signature = Utilities.StripHTMLTag(upi.Signature);
                        upi.Signature = Utilities.HTMLEncode(upi.Signature);
                    }
                    else if (MainSettings.AllowSignatures == 2)
                    {
                        upi.Signature = Utilities.XSSFilter(txtSignature.Text, false);
                    }
                    upc.Profiles_Save(upi);

                    Response.Redirect(NavigateUrl(TabId));

                }

            }
        }
    }
}
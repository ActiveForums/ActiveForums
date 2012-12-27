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

            nsPrefPageSize.Style.Add("float", "none");
            nsPrefPageSize.EmptyMessageStyle.CssClass += "dnnformHint";
            nsPrefPageSize.NumberFormat.DecimalDigits = 0;
            nsPrefPageSize.IncrementSettings.Step = 5;
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
            if (ui != null)
            {

                drpPrefDefaultSort.SelectedIndex = drpPrefDefaultSort.Items.IndexOf(drpPrefDefaultSort.Items.FindByValue(ui.PrefDefaultSort.Trim()));
                nsPrefPageSize.Value = ui.PrefPageSize;
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
                    upi.PrefPageSize = Convert.ToInt32(((Convert.ToInt32(nsPrefPageSize.Text) < 5) ? 5 : Convert.ToInt32(nsPrefPageSize.Text)));
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



                }

            }
        }
    }
}
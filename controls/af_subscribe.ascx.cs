//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_subscribe : ForumBase
    {
        #region Public Members
        public int mode = 1; //0 = Forum 1 = Topic
        public bool IsSubscribed = false;
        #endregion
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cbSubscribe.CallbackEvent += cbSubscribe_Callback;
            chkSubscribe.CheckedChanged += new System.EventHandler(chkSubscribe_CheckedChanged);

        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            string SubscribeText = null;
            if (mode == 0)
            {
                SubscribeText = GetSharedResource("[RESX:ForumSubscribe:" + IsSubscribed.ToString().ToUpper() + "]");
            }
            else
            {
                SubscribeText = GetSharedResource("[RESX:TopicSubscribe:" + IsSubscribed.ToString().ToUpper() + "]");
            }
            chkSubscribe.Text = SubscribeText;
            chkSubscribe.Checked = IsSubscribed;
            if (UseAjax)
            {
                chkSubscribe.Attributes.Add("onclick", "af_toggleSub();");
                AddToggleScript();
            }
            else
            {
                chkSubscribe.AutoPostBack = true;
            }
        }
        private void cbSubscribe_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            ToggleSubscribe();
            chkSubscribe.RenderControl(e.Output);
        }
        private void chkSubscribe_CheckedChanged(object sender, System.EventArgs e)
        {
            ToggleSubscribe();
        }
        #endregion
        #region Private Methods
        private void AddToggleScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("function af_toggleSub(){" + cbSubscribe.ClientID + ".Callback();};");
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "afsubscribe", sb.ToString(), true);
        }
        private void ToggleSubscribe()
        {
            int iStatus = 0;
            SubscriptionController sc = new SubscriptionController();
            iStatus = sc.Subscription_Update(PortalId, ModuleId, ForumId, TopicId, 1, this.UserId, ForumUser.UserRoles);
            if (iStatus == 1)
            {
                IsSubscribed = true;
            }
            else
            {
                IsSubscribed = false;
            }
            chkSubscribe.Checked = IsSubscribed;
            if (mode == 0)
            {
                chkSubscribe.Text = GetSharedResource("[RESX:ForumSubscribe:" + IsSubscribed.ToString().ToUpper() + "]");
            }
            else
            {
                chkSubscribe.Text = GetSharedResource("[RESX:TopicSubscribe:" + IsSubscribed.ToString().ToUpper() + "]");
            }


        }
        #endregion
    }
}

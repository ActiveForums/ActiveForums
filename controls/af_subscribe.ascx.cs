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

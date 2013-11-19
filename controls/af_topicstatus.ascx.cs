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
    public partial class af_topicstatus : ForumBase
    {
        private int _status = -1;
        private bool _autoPostBack = true;
        public int Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        public bool AutoPostBack
        {
            get
            {
                return _autoPostBack;
            }
            set
            {
                _autoPostBack = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            drpStatus.SelectedIndexChanged += new System.EventHandler(drpStatus_SelectedIndexChanged);
        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);


            drpStatus.AutoPostBack = AutoPostBack;
            foreach (ListItem li in drpStatus.Items)
            {
                li.Text = Utilities.GetSharedResource(li.Text);
            }
            if (!Page.IsPostBack)
            {
                drpStatus.SelectedIndex = drpStatus.Items.IndexOf(drpStatus.Items.FindByValue(Status.ToString()));
            }
        }

        private void drpStatus_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (AutoPostBack == true)
            {
                int intStatus = 0;
                intStatus = Convert.ToInt32(drpStatus.SelectedItem.Value);
                if (intStatus >= -1 && intStatus <= 3)
                {
                    DataProvider.Instance().Topics_UpdateStatus(PortalId, ModuleId, TopicId, -1, intStatus, -1, this.UserId);
                }
                Response.Redirect(Request.RawUrl);
            }

        }
    }
}

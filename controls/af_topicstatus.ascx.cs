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

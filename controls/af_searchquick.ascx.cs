//© 2004 - 2007 ActiveModules, Inc. All Rights Reserved

// Orphan Control??  Not Currently used - JB

using System;
using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{

    public partial class af_searchquick : ForumBase
    {
        public int MID;
        public int FID;


        #region Event Handlers

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                ForumModuleId = MID;
                if (ForumId < 1)
                {
                    ForumId = FID;
                }
                //Put user code to initialize the page here
                txtSearch.Attributes.Add("onkeydown", "if(event.keyCode == 13){document.getElementById('" + lnkSearch.ClientID + "').click();}");
            }
            catch (Exception exc)
            {
                Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion


        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            LocalResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx";

            lnkSearch.Click += lnkSearch_Click;
        }


        private void lnkSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text.Trim() != "")
            {
                string[] Params = { ParamKeys.ViewType + "=search", ParamKeys.ForumId + "=" + ForumId, "q=" + HttpUtility.UrlEncode(txtSearch.Text.Trim()) };
                Response.Redirect(NavigateUrl(TabId, "", Params));
            }

        }
    }

}

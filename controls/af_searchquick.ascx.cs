//© 2004 - 2007 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke;

namespace DotNetNuke.Modules.ActiveForums
{

    public partial class af_searchquick : ForumBase
    {
        public int MID;
        public int FID;

        #region Controls
        #endregion

        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                this.ForumModuleId = MID;
                if (ForumId < 1)
                {
                    ForumId = FID;
                }
                //Put user code to initialize the page here
                txtSearch.Attributes.Add("onkeydown", "if(event.keyCode == 13){document.getElementById('" + lnkSearch.ClientID + "').click();}");
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        //NOTE: The following placeholder declaration is required by the Web Form Designer.
        //Do not delete or move it.
        private object designerPlaceholderDeclaration;

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            this.LocalResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx";
            InitializeComponent();

            lnkSearch.Click += new System.EventHandler(lnkSearch_Click);

        }

        #endregion

        private void lnkSearch_Click(object sender, System.EventArgs e)
        {
            if (!(txtSearch.Text.Trim() == ""))
            {
                string[] Params = { ParamKeys.ViewType + "=search", ParamKeys.ForumId + "=" + ForumId, "q=" + HttpUtility.UrlEncode(txtSearch.Text.Trim()) };
                Response.Redirect(NavigateUrl(TabId, "", Params));
            }

        }
    }

}

//© 2004 - 2005 ActiveModules, Inc. All Rights Reserved
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

    public partial class af_markread : ForumBase
    {
        public int MID;
        #region Controls
        #endregion

        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                //Dim myMainSettings As SettingsInfo = DataCache.MainSettings(MID)
                this.ForumModuleId = MID;

                if (this.UserId == -1)
                {
                    btnMarkAllRead.Visible = false;
                }
                else
                {
                    btnMarkAllRead.Visible = true;
                }
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

            btnMarkAllRead.Click += new System.EventHandler(btnMarkAllRead_Click);
        }

        #endregion

        private void btnMarkAllRead_Click(object sender, System.EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (ForumId > 0)
                {
                    DataProvider.Instance().Utility_MarkAllRead(ModuleId, UserId, ForumId);
                }
                else
                {
                    DataProvider.Instance().Utility_MarkAllRead(ModuleId, UserId, 0);
                }
                Response.Redirect(Request.RawUrl);
            }
        }
    }

}

//© 2004 - 2005 ActiveModules, Inc. All Rights Reserved
using System;
namespace DotNetNuke.Modules.ActiveForums
{

    public partial class af_markread : ForumBase
    {
        public string CSSClass { get; set; }

        #region Event Handlers
        
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (btnMarkAllRead == null)
                return;

            btnMarkAllRead.Visible = UserId != -1;

            if (!string.IsNullOrWhiteSpace(CSSClass))
                btnMarkAllRead.CssClass = CSSClass;
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
            LocalResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx";
            InitializeComponent();

            btnMarkAllRead.Click += BtnMarkAllReadClick;
        }

        #endregion

        private void BtnMarkAllReadClick(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated) 
                return;

            DataProvider.Instance().Utility_MarkAllRead(ModuleId, UserId, ForumId > 0 ? ForumId : 0);
            
            Response.Redirect(Request.RawUrl);
        }
    }

}

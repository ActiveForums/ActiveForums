//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
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

    public partial class af_topicsorter : ForumBase
    {
        private string _defaultSort = "ASC";
        public string DefaultSort
        {
            get
            {
                return _defaultSort;
            }
            set
            {
                _defaultSort = value;
            }
        }
        #region Controls
        #endregion

        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                //Put user code to initialize the page here
                if (!Page.IsPostBack)
                {
                    string Sort = DefaultSort;
                    if (Request.Params[ParamKeys.Sort] != null)
                    {
                        Sort = Request.Params[ParamKeys.Sort];
                    }
                    drpSort.SelectedIndex = drpSort.Items.IndexOf(drpSort.Items.FindByValue(Sort));
                    drpSort.Items[0].Text = GetSharedResource(drpSort.Items[0].Text);
                    drpSort.Items[1].Text = GetSharedResource(drpSort.Items[1].Text);
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

            drpSort.SelectedIndexChanged += new System.EventHandler(drpSort_SelectedIndexChanged);

        }

        #endregion

        private void drpSort_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string Sort = drpSort.SelectedItem.Value;
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId, "", new string[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId, ParamKeys.Sort + "=" + Sort }));
        }
    }

}

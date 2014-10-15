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
            var sort = drpSort.SelectedItem.Value;
            var dest = DotNetNuke.Common.Globals.NavigateURL(TabId, "",
                                                             new[]
                                                                 {
                                                                     ParamKeys.ViewType + "=" + Views.Topic,
                                                                     ParamKeys.ForumId + "=" + ForumId,
                                                                     ParamKeys.TopicId + "=" + TopicId,
                                                                     ParamKeys.Sort + "=" + sort
                                                                 });
            Response.Redirect(dest);
        }
    }

}

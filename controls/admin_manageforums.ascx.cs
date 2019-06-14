//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
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
using System.Text;
using System.Web.UI;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class admin_manageforums : ActiveAdminBase
    {
        public string imgOn = string.Empty;
        public string imgOff = string.Empty;
        public string ctlView = string.Empty;

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cbForumEditor.CallbackEvent += cbForumEditor_Callback;

        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            imgOn = Page.ResolveUrl("~/DesktopModules/ActiveForums/images/admin_check.png");
            imgOff = Page.ResolveUrl("~/DesktopModules/ActiveForums/images/admin_stop.png");

            litButtons.Text = "<div class=\"amcplnkbtn\" onclick=\"LoadView('manageforums_forumeditor','0|G');\">[RESX:NewForumGroup]</div><div class=\"amcplnkbtn\" onclick=\"LoadView('manageforums_forumeditor','0|F');\">[RESX:NewForum]</div>";


            GetControl("admin_manageforums_home", string.Empty);


        }

        private void cbForumEditor_Callback(object sender, Controls.CallBackEventArgs e)
        {
            try
            {
                string sOptions = string.Empty;
                if (e.Parameters[1] != null)
                {
                    sOptions = e.Parameters[1];
                }
                GetControl(e.Parameters[0], sOptions);
                System.IO.StringWriter stringWriter = new System.IO.StringWriter();
                HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
                plhForumEditor.RenderControl(e.Output);
            }
            catch (Exception ex)
            {

            }
        }
        
        #endregion

        #region Private Methods
        private void GetControl(string view, string options)
        {
            try
            {
                plhForumEditor.Controls.Clear();
                string ctlPath = string.Empty;
                string ctlId = string.Empty;
                if (view == "admin_manageforums_home")
                {
                    ctlPath = "~/DesktopModules/ActiveForums/controls/admin_manageforums_home.ascx";
                    ctlId = "admin_manageforums_home";
                }
                else
                {
                    ctlPath = "~/DesktopModules/ActiveForums/controls/admin_manageforums_forumeditor.ascx";
                    ctlId = "admin_manageforums_forumeditor";
                }

                ActiveAdminBase ctl = (ActiveAdminBase)(LoadControl(ctlPath));
                ctl.ID = ctlId;
                ctl.ModuleConfiguration = this.ModuleConfiguration;

                if (!(options == string.Empty))
                {
                    ctl.Params = options;
                }
                if (!(plhForumEditor.Controls.Contains(ctl)))
                {
                    plhForumEditor.Controls.Add(ctl);
                }
            }
            catch (Exception ex)
            {
                LiteralControl lit = new LiteralControl();
                lit.Text = ex.Message;
                plhForumEditor.Controls.Add(lit);
            }


        }

        #endregion

    }
}

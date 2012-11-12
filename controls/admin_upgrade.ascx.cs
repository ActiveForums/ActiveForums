using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI;

namespace DotNetNuke.Modules.ActiveForums
{

    public partial class admin_upgrade : ActiveAdminBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cbUpgrade.CallbackEvent += cbUpgrade_Callback;

        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            Server.ScriptTimeout = int.MaxValue;
        }

        private void cbUpgrade_Callback(object sender, Controls.CallBackEventArgs e)
        {
            string upFilePath = Server.MapPath("~/desktopmodules/activeforums/upgrade4x.txt");
            string err = "Success";
            try
            {
                if (System.IO.File.Exists(upFilePath))
                {
                    string s = Utilities.GetFileContent(upFilePath);
                    err = DotNetNuke.Entities.Portals.PortalSettings.ExecuteScript(s);
                    System.IO.File.Delete(upFilePath);
                }
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException && string.IsNullOrEmpty(err))
                {
                    err = "<span style=\"font-size:14px;font-weight:bold;\">The forum data was upgraded successfully, but you must manually delete the following file:<br /><br />" + upFilePath + "<br /><br />The upgrade is not complete until you delete the file indicated above.</span>";
                }
                else
                {
                    err = "<span style=\"font-size:14px;font-weight:bold;\">Upgrade Failed - Please go to the <a href=\"http://www.activemodules.com/community/helpdesk.aspx\">Active Modules Help Desk</a> to report the error indicated below:<br /><br />" + ex.Message + "</span>";
                }

            }
            LiteralControl lit = new LiteralControl();
            if (string.IsNullOrEmpty(err))
            {
                err = "<script type=\"text/javascript\">LoadView('home');</script>";
            }
            lit.Text = err;
            lit.RenderControl(e.Output);
        }
    }
}
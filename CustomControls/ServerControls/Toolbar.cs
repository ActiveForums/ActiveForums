using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [ToolboxData("<{0}:Toolbar runat=server></{0}:Toolbar>")]
    public class Toolbar : ControlsBase
    {



        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            string sTemp = string.Empty;
            //pt = New Forums.Utils.TimeCalcItem("ForumDisplay")
            if (ControlConfig != null)
            {
                object obj = DataCache.CacheRetrieve(ControlConfig.InstanceId + "aftb");
                if (obj == null)
                {
                    sTemp = ParseTemplate();
                }
                else
                {
                    sTemp = Convert.ToString(obj);
                }
                sTemp = Utilities.LocalizeControl(sTemp);
                if (!(sTemp.Contains(Globals.ControlRegisterAFTag)))
                {
                    sTemp = Globals.ControlRegisterAFTag + sTemp;
                }
                Control ctl = Page.ParseControl(sTemp);
                LinkControls(ctl.Controls);
                this.Controls.Add(ctl);
            }
        }
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                if (ctrl is Controls.ControlsBase)
                {
                    ((Controls.ControlsBase)ctrl).ControlConfig = this.ControlConfig;
                }
                if (ctrl is Controls.Link)
                {
                    ((Controls.Link)ctrl).UserRoles = ForumUser.UserRoles;
                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }
        private string ParseTemplate()
        {
            string tb = DisplayTemplate; //Utilities.ParseToolBar(DisplayTemplate, PageId, InstanceId, UserId, CurrentUserTypes.Admin)
            //tb = tb.Replace
            tb = tb.Replace("[AF:TB:Unanswered]", "<af:link id=\"lnkUnanswered\" NavigateUrl=\"" + Utilities.NavigateUrl(PageId, "", new string[] { ParamKeys.ViewType + "=grid", "afgt=unanswered" }) + "\" text=\"[RESX:Unanswered]\" runat=\"server\" />");
            tb = tb.Replace("[AF:TB:ActiveTopics]", "<af:link id=\"lnkActive\" NavigateURL=\"" + Utilities.NavigateUrl(PageId, "", new string[] { ParamKeys.ViewType + "=grid", "afgt=activetopics" }) + "\" text=\"[RESX:ActiveTopics]\" runat=\"server\" />");
            tb = tb.Replace("[AF:TB:Search]", "<af:link id=\"lnkSearch\" NavigateUrl=\"" + Utilities.NavigateUrl(PageId, "", ParamKeys.ViewType + "=search") + "\" text=\"[RESX:Search]\" runat=\"server\" />");
            tb = tb.Replace("[AF:TB:Forums]", "<af:link id=\"lnkForums\" navigateUrl=\"" + Utilities.NavigateUrl(PageId) + "\" text=\"[RESX:FORUMS]\" runat=\"server\" />");
            tb = tb.Replace("[AF:TB:NotRead]", "<af:link id=\"lnkNotRead\" NavigateUrl=\"" + Utilities.NavigateUrl(PageId, "", new string[] { ParamKeys.ViewType + "=grid", "afgt=notread" }) + "\" text=\"[RESX:NotRead]\" AuthRequired=\"True\" runat=\"server\" />");
            tb = tb.Replace("[AF:TB:MyTopics]", "<af:link id=\"lnkMyTopics\" NavigateUrl=\"" + Utilities.NavigateUrl(PageId, "", new string[] { ParamKeys.ViewType + "=grid", "afgt=mytopics" }) + "\" text=\"[RESX:MyTopics]\" AuthRequired=\"True\" runat=\"server\" />");
            tb = tb.Replace("[AF:TB:MyProfile]", string.Empty);
            tb = tb.Replace("[AF:TB:MemberList]", string.Empty);
            tb = tb.Replace("[AF:TB:MySettings]", string.Empty);
            tb = tb.Replace("[AF:TB:ControlPanel]", "<af:link id=\"lnkControlPanel\" NavigateUrl=\"" + Utilities.NavigateUrl(PageId, "EDIT", "mid=" + ControlConfig.InstanceId) + "\" EnabledRoles=\"" + ControlConfig.AdminRoles + "\" Text=\"[RESX:ControlPanel]\" runat=\"server\" />");
            //TODO: Check for moderator
            tb = tb.Replace("[AF:TB:ModList]", string.Empty);
            DataCache.CacheStore(ControlConfig.InstanceId + "aftb", tb);
            return tb;
        }
    }

}

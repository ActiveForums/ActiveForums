//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Text;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class admin_home_new : ActiveAdminBase
    {
        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            BindDashboard();
        }
        #endregion
        #region Private Methods
        private void BindDashboard()
        {
            DataSet ds = DataProvider.Instance().Dashboard_Get(PortalId, ModuleId);
            if (ds != null)
            {
                BindRecentTopics(ds.Tables[0]);
                BindRecentMembers(ds.Tables[1]);
                BindTopForums(ds.Tables[2]);
                BindTopMembers(ds.Tables[3]);
                BindQuickStats(ds.Tables[4]);
            }
#if !SKU_LITE
            LoadQuickLinks();
#endif
        }

#if !SKU_LITE
        private void LoadQuickLinks()
        {
            StringBuilder sb = new StringBuilder(1024);
            sb.Append("<table cellpadding=\"0\" cellspacing=\"0\" class=\"dashOuter\" style=\"width:200px\">");
            sb.Append("<tr><td class=\"dashOuterHeader\">[RESX:QuickLinks]</td></tr><tr><td>");
            sb.Append("<table class=\"dashInner\" cellpadding=\"2\" style=\"width:100%\">");
            sb.Append("<tr><td class=\"dashRow\"><a href=\"javascript:void(0);\" onclick=\"LoadView('manageforums_forumeditor','0|G');\">[RESX:NewForumGroup]</a><br /></td>");
            sb.Append("</tr><tr><td class=\"dashRow\"><a href=\"javascript:void(0);\" onclick=\"LoadView('manageforums_forumeditor','0|F');\">[RESX:NewForum]</a><br /></td>");
            sb.Append("</tr></table></td></tr></table>");
            litQuickLinks.Text = sb.ToString();
        }
#endif

        private void BindRecentTopics(DataTable dt)
        {
            StringBuilder sb = new StringBuilder(1024);
            int rows = 0;
            sb.Append("<table class=\"dashInner\" cellpadding=\"2\" width=\"100%\">");
            sb.Append("<tr><td class=\"dashHD\">[RESX:Topic]</td><td class=\"dashHD\">[RESX:Author]</td></tr>");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class=\"dashRow\">" + dr["Subject"].ToString() + "</td>");
                    sb.Append("<td class=\"dashRow\">" + dr["DisplayName"].ToString() + "</td>");
                    sb.Append("</tr>");
                }
            }
            else
            {
                sb.Append("<tr><td colspan=\"2\" class=\"dashRow\">[RESX:NoTopics]</td></tr>");
            }
            sb.Append("</table>");
            litRecentTopics.Text = sb.ToString();
        }
        private void BindRecentMembers(DataTable dt)
        {
            StringBuilder sb = new StringBuilder(1024);
            int rows = 0;
            sb.Append("<table class=\"dashInner\" cellpadding=\"2\" width=\"100%\">");
            sb.Append("<tr><td class=\"dashHD\">[RESX:Date]</td><td class=\"dashHD\">[RESX:Name]</td><td class=\"dashHD\">[RESX:UserName]</td></tr>");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr>");
                    //TODO - Fix date time format
                    sb.Append("<td class=\"dashRow\">" + Convert.ToDateTime(dr["CreatedDate"]).ToShortDateString() + "</td>");
                    sb.Append("<td class=\"dashRow\">" + dr["FirstName"].ToString() + " " + dr["LastName"].ToString() + "</td>");
                    sb.Append("<td class=\"dashRow\">" + dr["Username"].ToString() + "</td>");
                    sb.Append("</tr>");
                }
            }
            else
            {
                sb.Append("<tr><td colspan=\"3\" class=\"dashRow\">[RESX:NoMembers]</td></tr>");
            }
            sb.Append("</table>");
            litRecentMembers.Text = sb.ToString();
        }
        private void BindTopForums(DataTable dt)
        {
            StringBuilder sb = new StringBuilder(1024);
            int rows = 0;
            sb.Append("<table class=\"dashInner\" cellpadding=\"2\" width=\"100%\">");
            sb.Append("<tr><td class=\"dashHD\">[RESX:ForumName]</td><td class=\"dashHD\" align=\"center\" style=\"text-align:center;\">[RESX:Topics]</td><td class=\"dashHD\" align=\"center\" style=\"text-align:center;\">[RESX:Replies]</td></tr>");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class=\"dashRow\">" + dr["ForumName"].ToString() + "</td>");
                    sb.Append("<td class=\"dashRow\" align=\"center\" style=\"text-align:center;\">" + dr["TotalTopics"].ToString() + "</td>");
                    sb.Append("<td class=\"dashRow\" align=\"center\" style=\"text-align:center;\">" + dr["TotalReplies"].ToString() + "</td>");
                    sb.Append("</tr>");
                }
            }
            else
            {
                sb.Append("<tr><td colspan=\"3\" class=\"dashRow\">[RESX:NoForums]</td></tr>");
            }
            sb.Append("</table>");
            litTopForums.Text = sb.ToString();
        }
        private void BindTopMembers(DataTable dt)
        {
            StringBuilder sb = new StringBuilder(1024);
            int rows = 0;
            sb.Append("<table class=\"dashInner\" cellpadding=\"2\" width=\"100%\">");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr>");
                    //TODO - Fix display name
                    sb.Append("<td class=\"dashRow\">" + dr["DisplayName"].ToString() + "</td>");
                    sb.Append("</tr>");
                }
            }
            else
            {
                sb.Append("<tr><td class=\"dashRow\">[RESX:NoMembers]</td></tr>");
            }
            sb.Append("</table>");
            litTopMembers.Text = sb.ToString();
        }
        private void BindQuickStats(DataTable dt)
        {
            StringBuilder sb = new StringBuilder(1024);
            int rows = 0;
            sb.Append("<table class=\"dashInner\" cellpadding=\"2\" width=\"100%\">");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr><td class=\"dashHD\">[RESX:TotalForums]</td><td class=\"dashRow\" style=\"text-align:right\">" + dr["TotalForums"].ToString() + "</td></tr>");
                    sb.Append("<tr><td class=\"dashHD\">[RESX:TotalTopics]</td><td class=\"dashRow\" style=\"text-align:right\">" + dr["TotalTopics"].ToString() + "</td></tr>");
                    sb.Append("<tr><td class=\"dashHD\">[RESX:TotalReplies]</td><td class=\"dashRow\" style=\"text-align:right\">" + dr["TotalReplies"].ToString() + "</td></tr>");
                    sb.Append("<tr><td class=\"dashHD\">[RESX:TotalMembers]</td><td class=\"dashRow\" style=\"text-align:right\">" + dr["TotalMembers"].ToString() + "</td></tr>");
                    sb.Append("<tr><td class=\"dashHD\">[RESX:TotalActiveMembers]</td><td class=\"dashRow\" style=\"text-align:right\">" + dr["TotalActiveMembers"].ToString() + "</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><td class=\"dashRow\">[RESX:NoData]</td></tr>");
            }
            sb.Append("</table>");
            litQuickStats.Text = sb.ToString();
        }
        #endregion
    }
}

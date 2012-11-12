using System;
using System.Data;

using DotNetNuke.Entities.Modules;
using System.Web.UI.WebControls;
using DotNetNuke.Framework;
using Telerik.Web.UI;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    public partial class ForumEditor : PortalModuleBase
    {

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            jQuery.RequestDnnPluginsRegistration();

            BindForums();
        }
        private void BindForums()
        {
            using (IDataReader dr = DataProvider.Instance().Forums_List(PortalId, ModuleId, -1, -1, false))
            {
                ctlForums.Nodes.Clear();
                var dt = new DataTable("Forums");
                dt.Load(dr);
                dr.Close();

                var groupNode = new RadTreeNode();
                string strParent = "-1";



                int totalGroupForum = 0;
                string tmpGroup = string.Empty;
                int i = 0;
                int groupCount = 0;
                int forumCount = 0;
                bool hasChildren = false;
                foreach (DataRow row in dt.Rows)
                {
                    if (tmpGroup != row["ForumGroupId"].ToString())
                    {
                        if (hasChildren)
                        {
                            ctlForums.Nodes.Add(groupNode);
                        }
                        drpForums.Items.Add(new ListItem(row["GroupName"].ToString(), row["ForumGroupId"].ToString()));
                        groupNode = new RadTreeNode
                                        {Text = row["GroupName"].ToString(), Value = row["ForumGroupId"].ToString()};
                        tmpGroup = row["ForumGroupId"].ToString();
                    }
                    drpForums.Items.Add(new ListItem("-----" + row["ForumName"], row["ForumId"].ToString()));
                    var node = new RadTreeNode {Text = row["ForumName"].ToString(), Value = row["ForumId"].ToString()};
                    groupNode.Nodes.Add(node);
                    hasChildren = true;
                }
                ctlForums.Nodes.Add(groupNode);
            }

        }
    }
}

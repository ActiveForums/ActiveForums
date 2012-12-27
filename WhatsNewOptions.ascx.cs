using System;
using System.Collections;
using System.Data;

using System.Web.UI.WebControls;
using System.Web.UI;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class WhatsNewOptions : PortalModuleBase
    {
        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            lnkUpdate.Click += lnkUpdate_Click;

            Page.ClientScript.RegisterClientScriptInclude("afutils", Page.ResolveUrl("~/DesktopModules/ActiveForums/scripts/afutils.js"));
            if (!Page.IsPostBack)
            {
                LoadForm();
            }
        }
        //Private Sub tsTags_Callback(ByVal sender As Object, ByVal e As Modules.ActiveForums.Controls.CallBackEventArgs) Handles tsTags.Callback
        //    tsTags.Datasource = DataProvider.Instance.Tags_Search(PortalId, ModuleId, e.Parameter.ToString + "%")
        //    tsTags.Refresh(e.Output)
        //End Sub
        private void lnkUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var objModules = new ModuleController();

                // Update Settings

                // Construct Forums 
                string f = string.Empty;
                if (GetNodeCount() != trForums.CheckedNodes.Count)
                {
                    if (trForums.CheckedNodes.Count > 1)
                    {
                        f = "";
                        foreach (TreeNode tr in trForums.CheckedNodes)
                        {
                            if (tr.Value.Contains("F:"))
                            {
                                f += tr.Value.Split(':')[1] + ":";
                            }
                        }
                    }
                    else
                    {
                        string sv = trForums.CheckedNodes[0].Value;
                        if (sv.Contains("F:"))
                        {
                            f = sv.Split(':')[1];
                        }
                    }
                }
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsForums", f);
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsNumber", txtNumItems.Text);
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsFormat", txtTemplate.Text);
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsHeader", txtHeader.Text);
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsFooter", txtFooter.Text);
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsRSS", chkRSS.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsTopicsOnly", chkTopicsOnly.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsRandomOrder", chkRandomOrder.Checked.ToString());
                objModules.UpdateModuleSetting(ModuleId, "AFTopPostsTags", txtTags.Text);

                if (chkRSS.Checked)
                {
                    objModules.UpdateModuleSetting(ModuleId, "AFTopPostsSecurity", chkIgnoreSecurity.Checked.ToString());
                    objModules.UpdateModuleSetting(ModuleId, "AFTopPostsBody", chkIncludeBody.Checked.ToString());
                    objModules.UpdateModuleSetting(ModuleId, "AFTopPostsCache", txtCache.Text);
                }


                // Redirect back to the portal home page
                DataCache.CacheClear("aftp" + ModuleId);
                Response.Redirect(Common.Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion
        #region Private Methods
        private int GetNodeCount()
        {
            int nc = 0;
            nc += trForums.Nodes.Count;
            foreach (TreeNode node in trForums.Nodes)
            {
                nc += 1;
                if (node.ChildNodes.Count > 0)
                {
                    nc += node.ChildNodes.Count;
                    foreach (TreeNode cnode in node.ChildNodes)
                    {
                        nc += cnode.ChildNodes.Count;
                        if (cnode.ChildNodes.Count > 0)
                        {
                            foreach (TreeNode subnode in cnode.ChildNodes)
                            {
                                nc += cnode.ChildNodes.Count;
                            }
                        }
                    }
                }
            }

            return nc;
        }
        private void LoadForm()
        {
            Hashtable settings = Entities.Portals.PortalSettings.GetModuleSettings(ModuleId);
            string forumids = "";
            if (settings == null)
            {
                txtHeader.Text = "<div style=\"padding:25px;padding-top:35px;\">";
                txtTemplate.Text = "<div style=\"padding-bottom:5px;\" class=\"normal\">[SUBJECTLINK]</div>";
                txtFooter.Text = "[RSSICONLINK]</div>";
                txtCache.Text = "10";
            }
            else if (settings.Count == 0)
            {
                txtHeader.Text = "<div style=\"padding:25px;padding-top:35px;\">";
                txtTemplate.Text = "<div style=\"padding-bottom:5px;\" class=\"normal\">[SUBJECTLINK]</div>";
                txtFooter.Text = "[RSSICONLINK]</div>";
                txtCache.Text = "10";
            }
            else
            {
                if (!(Convert.ToString(settings["AFTopPostsForums"]) == null))
                {
                    forumids = Convert.ToString(settings["AFTopPostsForums"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsNumber"]) == null))
                {
                    txtNumItems.Text = Convert.ToString(settings["AFTopPostsNumber"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsFormat"]) == null))
                {
                    txtTemplate.Text = Convert.ToString(settings["AFTopPostsFormat"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsHeader"]) == null))
                {
                    txtHeader.Text = Convert.ToString(settings["AFTopPostsHeader"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsFooter"]) == null))
                {
                    txtFooter.Text = Convert.ToString(settings["AFTopPostsFooter"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsRSS"]) == null))
                {
                    chkRSS.Checked = Convert.ToBoolean(settings["AFTopPostsRSS"]);
                    trRSS.Visible = Convert.ToBoolean(settings["AFTopPostsRSS"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsTopicsOnly"]) == null))
                {
                    chkTopicsOnly.Checked = Convert.ToBoolean(settings["AFTopPostsTopicsOnly"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsTags"]) == null))
                {
                    txtTags.Text = Convert.ToString(settings["AFTopPostsTags"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsRandomOrder"]) == null))
                {
                    chkRandomOrder.Checked = Convert.ToBoolean(settings["AFTopPostsRandomOrder"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsSecurity"]) == null))
                {
                    chkIgnoreSecurity.Checked = Convert.ToBoolean(settings["AFTopPostsSecurity"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsBody"]) == null))
                {
                    chkIncludeBody.Checked = Convert.ToBoolean(settings["AFTopPostsBody"]);
                }
                if (!(Convert.ToString(settings["AFTopPostsCache"]) == null))
                {
                    txtCache.Text = Convert.ToString(settings["AFTopPostsCache"]);
                }
            }

            BindForumsTree();
            if (forumids != string.Empty)
            {
                CheckNodes(forumids);
            }





        }
        private void CheckNodes(string ForumList)
        {
            string[] Forums;
            Forums = ForumList.Split(':');
            if (Forums != null)
            {
                //Clear all Nodes
                ManageCheck(false);
                if (Forums != null)
                {
                    foreach (string f in Forums)
                    {
                        if (f.Trim() != "")
                        {
                            ManageCheck(false, "F:" + f);
                        }
                    }
                }
            }

        }
        private void ManageCheck(bool state, string value = "")
        {
            foreach (TreeNode node in trForums.Nodes)
            {
                if (!node.Checked)
                {
                    node.Checked = Convert.ToBoolean(((node.Value == value) ? true : state));
                    if (node.Checked)
                    {
                        node.Parent.Expanded = true;
                    }
                }
                if (node.ChildNodes.Count > 0)
                {
                    foreach (TreeNode cnode in node.ChildNodes)
                    {
                        if (!cnode.Checked)
                        {
                            cnode.Checked = Convert.ToBoolean(((cnode.Value == value) ? true : state));
                            if (cnode.Checked)
                            {
                                cnode.Parent.Expanded = true;
                            }
                        }
                        if (cnode.ChildNodes.Count > 0)
                        {
                            foreach (TreeNode subnode in cnode.ChildNodes)
                            {
                                if (!subnode.Checked)
                                {
                                    subnode.Checked = Convert.ToBoolean(((subnode.Value == value) ? true : state));
                                    if (subnode.Checked)
                                    {
                                        subnode.Parent.Expanded = true;
                                    }
                                }

                            }
                        }
                    }
                }
            }

        }

        private void BindForumsTree()
        {

            var trNodes = new TreeNodeCollection();
            TreeNode trGroupNode = null;
            TreeNode trParentNode = null;
            TreeNode trNode;
            IDataReader reader = null;
            var dt = new DataTable("Forums");
            dt = new DataTable();
            dt.Load(DataProvider.Instance().PortalForums(PortalId));


            string tmpGroup = string.Empty;
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (tmpGroup != row["ForumGroupId"].ToString())
                {
                    trGroupNode = new TreeNode
                                      {
                                          Text = row["GroupName"].ToString(),
                                          ImageUrl = "~/DesktopModules/ActiveForums/images/tree/tree_group.png",
                                          ShowCheckBox = true,
                                          SelectAction = TreeNodeSelectAction.None,
                                          Value = "G:" + row["ForumGroupId"]
                                      };
                    trGroupNode.Expanded = i == 0;
                    i += 1;
                    tmpGroup = row["ForumGroupId"].ToString();
                    trNodes.Add(trGroupNode);
                }
                if (Convert.ToInt32(row["ParentForumId"]) == 0)
                {
                    trNode = new TreeNode
                                 {
                                     Text = row["ForumName"].ToString(),
                                     ImageUrl = "~/DesktopModules/ActiveForums/images/tree/tree_forum.png",
                                     ShowCheckBox = true,
                                     Expanded = false,
                                     SelectAction = TreeNodeSelectAction.None,
                                     Value = "F:" + row["ForumId"]
                                 };
                    if (HasSubForums(Convert.ToInt32(row["ForumId"]), dt))
                    {
                        AddChildNodes(trNode, dt, row);
                    }
                    //If trNode.ChildNodes.Count > 0 Then
                    trGroupNode.ChildNodes.Add(trNode);
                    //End If

                }

            }
            foreach (TreeNode tr in trNodes)
            {
                if (tr.ChildNodes.Count > 0)
                {
                    trForums.Nodes.Add(tr);
                }
            }


        }
        private void AddChildNodes(TreeNode ParentNode, DataTable dt, DataRow dr)
        {
            TreeNode tNode;
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(dr["ForumId"]) == Convert.ToInt32(row["ParentForumId"]))
                {
                    tNode = new TreeNode
                                {
                                    Text = row["ForumName"].ToString(),
                                    ImageUrl = "~/DesktopModules/ActiveForums/images/tree/tree_forum.png",
                                    ShowCheckBox = true,
                                    Value = "F:" + row["ForumId"],
                                    Checked = false,
                                    SelectAction = TreeNodeSelectAction.None
                                };
                    ParentNode.ChildNodes.Add(tNode);
                    AddChildNodes(tNode, dt, row);
                }
            }
        }
        private bool HasSubForums(int ForumId, DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(row["ParentForumId"]) == ForumId)
                {
                    return true;
                }
            }
            return false;
        }
        protected override void Render(HtmlTextWriter writer)
        {
            var stringWriter = new System.IO.StringWriter();
            var htmlWriter = new HtmlTextWriter(stringWriter);
            base.Render(htmlWriter);
            string html = stringWriter.ToString();
            html = Utilities.LocalizeControl(html, "WhatsNew.ascx");
            writer.Write(html);
        }

        #endregion

    }
}

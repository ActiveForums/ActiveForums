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
            lnkCancel.Click += lnkCancel_Click;

            chkRSS.CheckedChanged += chkRSS_Change;

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
                // Construct Forums 
                var forums = string.Empty;
                if (GetNodeCount() != trForums.CheckedNodes.Count)
                {
                    if (trForums.CheckedNodes.Count > 1)
                    {
                        forums = string.Empty;
                        foreach (TreeNode tr in trForums.CheckedNodes)
                        {
                            if (tr.Value.Contains("F:"))
                            {
                                forums += tr.Value.Split(':')[1] + ":";
                            }
                        }
                    }
                    else
                    {
                        var sv = trForums.CheckedNodes[0].Value;
                        if (sv.Contains("F:"))
                        {
                            forums = sv.Split(':')[1];
                        }
                    }
                }

                var mc = new ModuleController();

                // Load the current settings
                var settings = WhatsNewModuleSettings.CreateFromModuleSettings(mc.GetModuleSettings(ModuleId));

                // Update Settings Values
                settings.Forums = forums;

                int rows;
                if (int.TryParse(txtNumItems.Text, out rows))
                    settings.Rows = int.Parse(txtNumItems.Text);

                settings.Format = txtTemplate.Text;
                settings.Header = txtHeader.Text;
                settings.Footer = txtFooter.Text;
                settings.RSSEnabled = chkRSS.Checked;
                settings.TopicsOnly = chkTopicsOnly.Checked;
                settings.RandomOrder = chkRandomOrder.Checked;
                settings.Tags = txtTags.Text;

                if (chkRSS.Checked)
                {
                    settings.RSSIgnoreSecurity = chkIgnoreSecurity.Checked;
                    settings.RSSIncludeBody = chkIncludeBody.Checked;

                    int rssCacheTimeout;
                    if (int.TryParse(txtCache.Text, out rssCacheTimeout))
                        settings.RSSCacheTimeout = rssCacheTimeout;
                }

                // Save Settings
                settings.Save(mc, ModuleId);

                // Redirect back to the portal home page
                Response.Redirect(Common.Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void lnkCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Common.Globals.NavigateURL(), true);
        }

        private void chkRSS_Change(object sender, EventArgs e)
        {
            pnlRSS.Visible = chkRSS.Checked;
            rqvTxtCache.Enabled = chkRSS.Checked;
        }

        #endregion

        #region Private Methods

        private int GetNodeCount()
        {
            var nc = trForums.Nodes.Count;
            foreach (TreeNode node in trForums.Nodes)
            {
                nc += 1;
                if (node.ChildNodes.Count <= 0) continue;

                nc += node.ChildNodes.Count;
                foreach (TreeNode cnode in node.ChildNodes)
                {
                    nc += cnode.ChildNodes.Count;

                    if (cnode.ChildNodes.Count <= 0) continue;

                    foreach (TreeNode subnode in cnode.ChildNodes)
                    {
                        nc += cnode.ChildNodes.Count;
                    }
                }
            }

            return nc;
        }

        private void LoadForm()
        {
            var moduleSettings = Entities.Portals.PortalSettings.GetModuleSettings(ModuleId);
            var settings = WhatsNewModuleSettings.CreateFromModuleSettings(moduleSettings);

            txtNumItems.Text = settings.Rows.ToString();
            txtTemplate.Text = settings.Format;
            txtHeader.Text = settings.Header;
            txtFooter.Text = settings.Footer;
            chkRSS.Checked = settings.RSSEnabled;
            chkTopicsOnly.Checked = settings.TopicsOnly;
            chkRandomOrder.Checked = settings.RandomOrder;
            chkIgnoreSecurity.Checked = settings.RSSIgnoreSecurity;
            chkIncludeBody.Checked = settings.RSSIncludeBody;
            txtCache.Text = settings.RSSCacheTimeout.ToString();
            txtTags.Text = settings.Tags;

            BindForumsTree();

            if (settings.Forums != string.Empty)
                CheckNodes(settings.Forums);

            pnlRSS.Visible = chkRSS.Checked;
            rqvTxtCache.Enabled = chkRSS.Checked;
        }

        private void CheckNodes(string forumList)
        {
            var forums = forumList.Split(':');

            //Clear all Nodes
            ManageCheck(false);

            foreach (var f in forums)
            {
                if (f.Trim() != "")
                {
                    ManageCheck(false, "F:" + f);
                }
            }

        }

        private void ManageCheck(bool state, string value = "")
        {
            foreach (TreeNode node in trForums.Nodes)
            {
                if (!node.Checked)
                {
                    node.Checked = (node.Value == value || state);
                    if (node.Checked && node.Parent != null)
                    {
                        node.Parent.Expanded = true;
                    }
                }

                if (node.ChildNodes.Count <= 0) continue;

                foreach (TreeNode cnode in node.ChildNodes)
                {
                    if (!cnode.Checked)
                    {
                        cnode.Checked = (cnode.Value == value || state);
                        if (cnode.Checked & cnode.Parent != null)
                        {
                            cnode.Parent.Expanded = true;
                        }
                    }

                    if (cnode.ChildNodes.Count <= 0) continue;

                    foreach (TreeNode subnode in cnode.ChildNodes)
                    {
                        if (subnode.Checked) continue;
                        subnode.Checked = (subnode.Value == value || state);
                        if (subnode.Checked && subnode.Parent != null)
                        {
                            subnode.Parent.Expanded = true;
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
            IDataReader reader = null;
            var dt = new DataTable();
            dt.Load(DataProvider.Instance().PortalForums(PortalId));


            var tmpGroup = string.Empty;
            var i = 0;
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
                    var trNode = new TreeNode
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
        private void AddChildNodes(TreeNode parentNode, DataTable dt, DataRow dr)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(dr["ForumId"]) != Convert.ToInt32(row["ParentForumId"])) continue;
                var tNode = new TreeNode
                {
                    Text = row["ForumName"].ToString(),
                    ImageUrl = "~/DesktopModules/ActiveForums/images/tree/tree_forum.png",
                    ShowCheckBox = true,
                    Value = "F:" + row["ForumId"],
                    Checked = false,
                    SelectAction = TreeNodeSelectAction.None
                };
                parentNode.ChildNodes.Add(tNode);
                AddChildNodes(tNode, dt, row);
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

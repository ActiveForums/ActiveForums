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

using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_quickjump : ForumBase
    {
        public int MOID;
        private DataTable _dtForums = null;
        protected DropDownList drpForums = new DropDownList();
        public DataTable dtForums
        {
            get
            {
                return _dtForums;
            }
            set
            {
                _dtForums = value;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            drpForums.SelectedIndexChanged += new System.EventHandler(drpForums_SelectedIndexChanged);

            try
            {
                if (drpForums == null)
                {
                    drpForums = new DropDownList();
                }
                drpForums.AutoPostBack = true;
                drpForums.CssClass = "afdropdown";
                BindForums();
                Controls.Add(drpForums);
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void BindForums()
        {
            ForumController fc = new ForumController();
            dtForums = fc.GetForumView(PortalId, MOID, UserId, UserInfo.IsSuperUser, UserForumsList);

            drpForums.Items.Clear();
            drpForums.Items.Insert(0, new ListItem(string.Empty, string.Empty));
            int i = 0;
            int n = 1;
            int tmpGroupCount = 0;
            int tmpForumCount = 0;
            string tmpGroupKey = string.Empty;
            string tmpForumKey = string.Empty;
            foreach (DataRow dr in dtForums.Rows)
            {
                bool bView = Permissions.HasPerm(dr["CanView"].ToString(), ForumUser.UserRoles);
                string GroupName = Convert.ToString(dr["GroupName"]);
                int GroupId = Convert.ToInt32(dr["ForumGroupId"]);
                string GroupKey = GroupName + GroupId.ToString();
                string ForumName = Convert.ToString(dr["ForumName"]);
                if (ForumName.Length > 30) ForumName = ForumName.Substring(0, 30) + "...";
                int ForumId = Convert.ToInt32(dr["ForumId"]);
                string ForumKey = ForumName + ForumId.ToString();
                int ParentForumId = Convert.ToInt32(dr["ParentForumId"]);
                //TODO - Need to add support for Group Permissions and GroupHidden

                if (tmpGroupKey != GroupKey)
                {
                    drpForums.Items.Insert(n, new ListItem(GroupName, "GROUPJUMP:" + GroupId));
                    n += 1;
                    tmpGroupKey = GroupKey;
                }
                if (bView)
                {
                    if (ParentForumId == 0)
                    {
                        drpForums.Items.Insert(n, new ListItem("--" + ForumName, "FORUMJUMP:" + dr["ForumID"].ToString()));
                        n += 1;
                        n = GetSubForums(n, Convert.ToInt32(dr["ForumId"]));
                    }

                }






            }

            if (GetViewType != null)
            {
                string sView = GetViewType;
                if (sView == "TOPICS" || sView == "TOPIC")
                {
                    string sForum = "FORUMJUMP:" + ForumId;
                    drpForums.SelectedIndex = drpForums.Items.IndexOf(drpForums.Items.FindByValue(sForum));
                }
            }
        }
        private int GetSubForums(int ItemCount, int ParentForumId)
        {
            dtForums.DefaultView.RowFilter = "ParentForumId = " + ParentForumId;
            if (dtForums.DefaultView.Count > 0)
            {
                foreach (DataRow dr in dtForums.DefaultView.ToTable().Rows)
                {
                    if (Permissions.HasPerm(dr["CanView"].ToString(), ForumUser.UserRoles))
                    {
                        string ForumName = dr["ForumName"].ToString();

                        if (ForumName.Length > 30) ForumName = ForumName.Substring(0, 30) + "...";

                        drpForums.Items.Insert(ItemCount, new ListItem("----" + ForumName, "FORUMJUMP:" + dr["ForumID"].ToString()));
                        ItemCount += 1;
                    }

                }

            }
            return ItemCount;
        }
        private void drpForums_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Try
            string sJumpValue = drpForums.SelectedItem.Value;
            if (!(sJumpValue == string.Empty) && !(sJumpValue == ""))
            {
                string sJumpType = sJumpValue.Substring(0, (sJumpValue.IndexOf(":", 0) + 1) - 1);
                string sJumpID = sJumpValue.Substring((sJumpValue.IndexOf(":", 0) + 1));
                switch (sJumpType)
                {
                    case "GROUPJUMP":
                        Response.Redirect(NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + sJumpID));
                        break;
                    case "FORUMJUMP":
                        string[] Params = { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + sJumpID };
                        Response.Redirect(NavigateUrl(TabId, "", Params));
                        break;
                }
            }

            //Catch ex As Exception

            //End Try

        }
    }
}

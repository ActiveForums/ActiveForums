using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI.WebControls;
using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_search_new : ForumBase
    {
        #region Private Members
        private int RowCount = 0;
        private DataTable dtResults;
        private int PageSize = 20;
        private int RowIndex = 0;
        #endregion
        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            lnkSearch.Click += new System.EventHandler(btnSearch_Click);

            try
            {

                txtSearch.Attributes.Add("onkeydown", "if(event.keyCode == 13){document.getElementById('" + lnkSearch.ClientID + "').click();}");
                BindSearchRange();


                if (!Page.IsPostBack)
                {
                    BindForumsTree();
                    CheckNodes();

                    litSearchColumns.Text = GetSharedResource("[RESX:SearchArea]");
                    litSearchType.Text = GetSharedResource("[RESX:SearchMatch]");
                    litSearchTimeFrame.Text = GetSharedResource("[RESX:SearchTimeFrame]");
                    litSearchUserText.Text = GetSharedResource("[RESX:SearchByUser]");
                    litSearchForums.Text = GetSharedResource("[RESX:SearchInForums]");


                    lnkSearch.Text = "<img src=\"" + ImagePath + "/images/search.png\" style=\"vertical-align:middle;border:none;\" alt=\"" + GetSharedResource("[RESX:Search]") + "\" />" + GetSharedResource("[RESX:Search]");
                    string SortDirection = null;
                    if (Request.Params["afsort"] != null)
                    {
                        SortDirection = Request.Params["afsort"];
                    }
                    else
                    {
                        SortDirection = "DESC";
                    }
                    //Setup Tree
                    string sq = string.Empty;
                    if (!(string.IsNullOrEmpty(Request.QueryString["q"])))
                    {
                        sq = Server.UrlDecode(Request.QueryString["q"]);
                        sq = Utilities.XSSFilter(sq);
                    }
                    DotNetNuke.Framework.CDefault tempVar = this.BasePage;
                    Environment.UpdateMeta(ref tempVar, "[VALUE] - " + GetSharedResource("[RESX:Search]") + " - " + sq, "[VALUE]", "[VALUE]");
                    BindPosts(0, SortDirection);
                }
            }
            catch (Exception ex)
            {
                this.Controls.Clear();
                RenderMessage("[RESX:ERROR]", "[RESX:ERROR:Search]", ex.Message, ex);
            }


        }
        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            if (!(txtSearch.Text.Trim() == string.Empty))
            {
                string s = txtSearch.Text.Trim();
                s = Utilities.XSSFilter(s);
                s = Server.UrlEncode(s);
                int mt = 0;
                mt = Convert.ToInt32(drpSearchType.SelectedItem.Value);
                int sf = 0;
                sf = Convert.ToInt32(drpSearchColumns.SelectedItem.Value);
                int ts = 0;
                ts = Convert.ToInt32(drpSearchDays.SelectedItem.Value);
                int au = 0;
                if (Request.Params["uid"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params["uid"]))
                    {
                        au = Convert.ToInt32(Request.Params["uid"]);
                    }
                }
                string an = "0";
                if (!(txtUserName.Text.Trim() == ""))
                {
                    an = txtUserName.Text;
                    an = Utilities.CheckSqlString(an);
                    an = Utilities.XSSFilter(an);
                    an = HttpUtility.UrlEncode(an);
                }
                string f = "0";
                if (!(GetNodeCount() == trForums.CheckedNodes.Count))
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
                Response.Redirect(NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=search", "q=" + s, "k=" + mt.ToString(), "c=" + sf.ToString(), "ts=" + ts.ToString(), "uid=" + au.ToString(), "author=" + an, "f=" + f }));
            }

        }
        #endregion

        #region Private Methods
        private void BindPosts(int Column = 0, string Sort = "ASC")
        {
            PageSize = MainSettings.PageSize;
            if (UserId > 0)
            {
                PageSize = UserDefaultPageSize;
            }
            if (PageSize < 5)
            {
                PageSize = 10;
            }
            if (PageId == 1)
            {
                RowIndex = 0;
            }
            else
            {
                RowIndex = ((PageId * PageSize) - PageSize);
            }
            lblHeader.Text = GetSharedResource("[RESX:Search]");
            int MatchType = 0;
            string SearchString = "";
            if (Request.Params["q"] != null)
            {
                SearchString = Request.Params["q"];
                SearchString = Server.UrlDecode(SearchString);
                SearchString = Utilities.XSSFilter(SearchString);
                SearchString = Utilities.StripHTMLTag(SearchString);
                SearchString = Utilities.CheckSqlString(SearchString);
                SearchString = SearchString.Trim();
                txtSearch.Text = SearchString;
            }
            string tg = string.Empty;
            if (Request.Params[ParamKeys.Tags] != null)
            {
                tg = Request.Params[ParamKeys.Tags];
                tg = HttpUtility.UrlDecode(tg);
                tg = Utilities.XSSFilter(tg);
                tg = Utilities.StripHTMLTag(tg);
                tg = Utilities.CheckSqlString(tg);
                tg = tg.Trim();
            }
            int au = 0;
            if (Request.Params["uid"] != null)
            {
                if (SimulateIsNumeric.IsNumeric(Request.Params["uid"]))
                {
                    au = Convert.ToInt32(Request.Params["uid"]);
                }
            }
            if (!(SearchString == string.Empty) || !(tg == string.Empty) || au > 0)
            {
                int mt = 0;
                int sf = 0;
                int ts = 0;

                string an = "";

                if (Request.Params["k"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params["k"]))
                    {
                        mt = Convert.ToInt32(Request.Params["k"]);
                        drpSearchType.SelectedIndex = drpSearchType.Items.IndexOf(drpSearchType.Items.FindByValue(mt.ToString()));
                    }

                }
                if (Request.Params["c"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params["c"]))
                    {
                        sf = Convert.ToInt32(Request.Params["c"]);
                        drpSearchColumns.SelectedIndex = drpSearchColumns.Items.IndexOf(drpSearchColumns.Items.FindByValue(sf.ToString()));
                    }
                }
                if (Request.Params["ts"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params["ts"]))
                    {
                        ts = Convert.ToInt32(Request.Params["ts"]);
                        drpSearchDays.SelectedIndex = drpSearchDays.Items.IndexOf(drpSearchDays.Items.FindByValue(ts.ToString()));
                    }

                }
                if (Request.Params["author"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params["author"]))
                    {
                        au = Convert.ToInt32(Request.Params["author"]);
                    }
                    else
                    {
                        an = Request.Params["author"];
                        an = Utilities.XSSFilter(an);
                        an = Utilities.StripHTMLTag(an);
                        an = Utilities.CheckSqlString(an);
                        an = an.Trim();
                    }
                    txtUserName.Text = Utilities.XSSFilter(an);
                }

                string forums = string.Empty;
                int tmpForumId = ForumId;
                if (Request.Params["f"] != null)
                {
                    if (SimulateIsNumeric.IsNumeric(Request.Params["f"]))
                    {
                        tmpForumId = Convert.ToInt32(Request.Params["f"]);
                    }
                    else
                    {
                        forums = Request.Params["f"];
                        forums = Utilities.CheckSqlString(forums);
                    }
                }
                DataSet ds = null;
                ForumController fc = new ForumController();
                string sForumsAllowed = fc.GetForumsForUser(ForumUser.UserRoles, PortalId, ModuleId, "CanRead");

#if SKU_ENTERPRISE

                if (MainSettings.FullText)
                {
                    if (tg == string.Empty && !(SearchString == string.Empty))
                    {
                        ds = DataProvider.Instance().Search_FullText(PortalId, ModuleId, UserId, tmpForumId, UserInfo.IsSuperUser, RowIndex, PageSize, SearchString, mt, sf, ts, au, an, forums, tg, sForumsAllowed);
                    }
                    else if (!(tg == string.Empty))
                    {
                        ds = DataProvider.Instance().Search_Standard(PortalId, ModuleId, UserId, tmpForumId, UserInfo.IsSuperUser, RowIndex, PageSize, SearchString, mt, sf, ts, au, an, forums, tg, sForumsAllowed);
                    }
                    else if (!(SearchString == string.Empty))
                    {
                        ds = DataProvider.Instance().Search_FullText(PortalId, ModuleId, UserId, tmpForumId, UserInfo.IsSuperUser, RowIndex, PageSize, SearchString, mt, sf, ts, au, an, forums, tg, sForumsAllowed);
                    }
                    else if (au > 0 && SearchString == string.Empty && tg == string.Empty)
                    {
                        ds = DataProvider.Instance().Search_Standard(PortalId, ModuleId, UserId, tmpForumId, UserInfo.IsSuperUser, RowIndex, PageSize, SearchString, mt, sf, ts, au, an, forums, tg, sForumsAllowed);
                    }

                }
                else
                {
                    ds = DataProvider.Instance().Search_Standard(PortalId, ModuleId, UserId, tmpForumId, UserInfo.IsSuperUser, RowIndex, PageSize, SearchString, mt, sf, ts, au, an, forums, tg, sForumsAllowed);
                }

#else
				ds = DataProvider.Instance().Search_Standard(PortalId, ModuleId, UserId, tmpForumId, UserInfo.IsSuperUser, RowIndex, PageSize, SearchString, mt, sf, ts, au, an, forums, tg, sForumsAllowed);
#endif

                dtResults = ds.Tables[1];
                if (dtResults.Rows.Count > 0)
                {
                    RowCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                    pnlMessage.Visible = false;
                    pnlResults.Visible = true;
                    try
                    {
                        rptPosts.DataSource = dtResults;
                        rptPosts.DataBind();
                        BuildPager();
                    }
                    catch (Exception ex)
                    {
                        litMessage.Text = ex.Message.ToString();
                        pnlMessage.Visible = true;
                        pnlResults.Visible = false;
                    }
                }
                else
                {
                    litMessage.Text = GetSharedResource("[RESX:SearchNoResults]");
                    pnlMessage.Visible = true;
                    pnlResults.Visible = false;
                }
            }



        }
        private void BuildPager()
        {
            int intPages = 0;
            intPages = Convert.ToInt32(System.Math.Ceiling(RowCount / (double)PageSize));
            Pager1.PageCount = intPages;
            Pager1.CurrentPage = PageId;
            Pager1.TabID = TabId;
            Pager1.ForumID = ForumId;
            Pager1.PageText = Utilities.GetSharedResource("[RESX:Page]");
            Pager1.OfText = Utilities.GetSharedResource("[RESX:PageOf]");
            Pager1.View = "search";
            //If UseAjax Then
            //Pager1.PageMode = Forums.Controls.PagerNav.Mode.CallBack
            //Else
            Pager1.PageMode = Modules.ActiveForums.Controls.PagerNav.Mode.Links;
            //End If
            if (Request.Params["q"] != null)
            {
                string[] Params = { "q=" + Request.Params["q"] };
                Pager1.Params = Params;
            }
        }
        private void BindSearchRange()
        {
            string sHours = GetSharedResource("SearchRangeHours.Text");
            string sDays = GetSharedResource("SearchRangeDays.Text");
            string sAll = GetSharedResource("SearchRangeAll.Text");
            drpSearchDays.Items.Clear();
            drpSearchDays.Items.Add(new ListItem(sAll, "0"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sHours, "12"), "12"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sHours, "24"), "24"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "3"), "72"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "5"), "124"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "10"), "240"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "15"), "360"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "30"), "720"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "45"), "1080"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "60"), "1440"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "90"), "2160"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "120"), "2880"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "240"), "5760"));
            drpSearchDays.Items.Add(new ListItem(string.Format(sDays, "365"), "8640"));


        }
        private void CheckNodes()
        {
            string[] Forums = null;
            string Forum = string.Empty;
            if (Request.Params["f"] != null)
            {
                if (Request.Params["f"].Contains(":"))
                {
                    Forums = Request.Params["f"].Split(':');
                }
                else
                {
                    Forum = Request.Params["f"];
                }
            }
            if (!(Forum == string.Empty) || Forums != null | ForumId > 0)
            {
                //Clear all Nodes
                ManageCheck(false);
                if (Forums != null)
                {
                    foreach (string f in Forums)
                    {
                        if (!(f.Trim() == ""))
                        {
                            ManageCheck(false, "F:" + f);
                        }
                    }
                }
                if (!(Forum == string.Empty))
                {
                    ManageCheck(false, "F:" + Forum);
                }
                if (ForumId > 0)
                {
                    ManageCheck(false, "F:" + ForumId.ToString());
                }
            }
            else
            {
                mnShowForums.Text = GetSharedResource("[RESX:SearchForumsAll]");
                ManageCheck(true);
            }
            //            If ForumId = CInt(row("ForumId")) Or (Not Forums Is Nothing AndAlso Array.IndexOf(Forums, row("ForumId").ToString) > 0) Or Forum = row("ForumId").ToString Or (ForumId <= 0 And Forums Is Nothing And Forum = String.Empty) Then
        }
        private void ManageCheck(bool state, string value = "")
        {
            foreach (TreeNode node in trForums.Nodes)
            {
                if (!node.Checked)
                {
                    node.Checked = Convert.ToBoolean(((node.Value == value) ? true : state));
                }
                if (node.ChildNodes.Count > 0)
                {
                    foreach (TreeNode cnode in node.ChildNodes)
                    {
                        if (!cnode.Checked)
                        {
                            cnode.Checked = Convert.ToBoolean(((cnode.Value == value) ? true : state));
                        }
                        if (cnode.ChildNodes.Count > 0)
                        {
                            foreach (TreeNode subnode in cnode.ChildNodes)
                            {
                                if (!subnode.Checked)
                                {
                                    subnode.Checked = Convert.ToBoolean(((subnode.Value == value) ? true : state));
                                }

                            }
                        }
                    }
                }
            }

        }
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

        private void BindForumsTree()
        {

            TreeNodeCollection trNodes = new TreeNodeCollection();
            TreeNode trGroupNode = null;
            TreeNode trParentNode = null;
            TreeNode trNode = null;
            IDataReader reader = null;
            DataTable dt = new DataTable("Forums");
            dt = new DataTable();
            dt = DataProvider.Instance().UI_ForumView(PortalId, ModuleId, this.UserId, UserInfo.IsSuperUser, ForumIds).Tables[0];


            string tmpGroup = string.Empty;
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (tmpGroup != row["ForumGroupId"].ToString())
                {
                    trGroupNode = new TreeNode();
                    trGroupNode.Text = row["GroupName"].ToString();
                    trGroupNode.ImageUrl = "~/DesktopModules/ActiveForums/images/tree/tree_group.png";
                    trGroupNode.ShowCheckBox = true;
                    trGroupNode.Value = "G:" + row["ForumGroupId"].ToString();
                    trGroupNode.SelectAction = TreeNodeSelectAction.None;
                    tmpGroup = row["ForumGroupId"].ToString();
                    trNodes.Add(trGroupNode);
                }
                if (Convert.ToInt32(row["ParentForumId"]) == 0)
                {
                    trNode = new TreeNode();
                    trNode.Text = row["ForumName"].ToString();
                    trNode.ImageUrl = "~/DesktopModules/ActiveForums/images/tree/tree_forum.png";
                    trNode.ShowCheckBox = true;
                    trNode.SelectAction = TreeNodeSelectAction.None;
                    trNode.Value = "F:" + row["ForumId"].ToString();
                    if (HasSubForums(Convert.ToInt32(row["ForumId"]), dt))
                    {
                        AddChildNodes(trNode, dt, row);
                    }
                    if (trNode.ChildNodes.Count > 0 || Permissions.HasPerm(row["CanRead"].ToString(), ForumUser.UserRoles))
                    {
                        trGroupNode.ChildNodes.Add(trNode);
                    }

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
            TreeNode tNode = null;
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(dr["ForumId"]) == Convert.ToInt32(row["ParentForumId"]))
                {
                    tNode = new TreeNode();
                    tNode.Text = row["ForumName"].ToString();
                    tNode.ImageUrl = "~/DesktopModules/ActiveForums/images/tree/tree_forum.png";
                    tNode.ShowCheckBox = true;
                    tNode.Value = "F:" + row["ForumId"].ToString();
                    tNode.Checked = false;
                    tNode.SelectAction = TreeNodeSelectAction.None;
                    if (Permissions.HasPerm(row["CanRead"].ToString(), ForumUser.UserRoles))
                    {
                        ParentNode.ChildNodes.Add(tNode);
                    }
                    AddChildNodes(tNode, dt, row);
                }
            }
        }
        private bool HasSubForums(int ForumId, DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(row["ParentForumId"]) == ForumId && Permissions.HasPerm(row["CanRead"].ToString(), ForumUser.UserRoles))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
        #region Public Methods
        //public string GetIcon(string Icon, bool Pinned = false, bool Locked = false)
        //{
        //    bool IsRead = true;
        //    string myTheme = MainSettings.Theme;
        //    if (Icon == string.Empty)
        //    {

        //        if (Pinned == true)
        //        {
        //            return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/pinned.gif";
        //        }
        //        else if (Locked == true)
        //        {
        //            return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/lock.gif";
        //        }
        //        else
        //        {
        //            if (IsRead == true)
        //            {
        //                return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/document.gif";
        //            }
        //            else
        //            {
        //                return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/document_new.gif";
        //            }

        //        }
        //    }
        //    else
        //    {
        //        return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/" + Icon;
        //    }
        //}
        public string GetIcon(object icon, object pinned, object locked) {
            string myTheme = MainSettings.Theme;
            string Icon = (string)icon;
            bool Pinned = (bool)pinned;
            bool Locked = (bool)locked;
            bool IsRead = true;
           

            if (Icon == string.Empty) {

                if (Pinned == true) {
                    return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/topic_pin.png";
                } else if (Locked == true) {
                    return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/lock.gif";
                } else {
                    if (IsRead == true) {
                        return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/document.gif";
                    } else {
                        return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/document_new.gif";
                    }

                }
            } else {
                return "~/DesktopModules/ActiveForums/themes/" + myTheme + "/emoticons/" + Icon;
            }
        }
        //public string GetLastPost(int UserID, string UserName, DateTime DateAdded, int LastPostID, int ParentPostID = 0, string Subject = "", int ForumID = 0, int ReplyCount = 1, string FirstName = "", string LastName = "", string DisplayName = "")
        //{
        //    try
        //    {
        //        int PostId = LastPostID;
        //        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //        if (!(UserName == ""))
        //        {
        //            sb.Append("<nobr> " + GetSharedResource("By.Text") + " ");
        //            if (UserID == 0 || UserID == -1)
        //            {
        //                sb.Append(UserName);
        //            }
        //            else
        //            {
        //                bool isMod = false;
        //                if (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.ForumMod || CurrentUserType == CurrentUserTypes.SuperUser)
        //                {
        //                    isMod = true;
        //                }
        //                sb.Append(UserProfiles.GetDisplayName(ModuleId, MainSettings.ProfileVisibility, isMod, UserID, MainSettings.UserNameDisplay, UserName, FirstName, LastName, DisplayName));
        //            }
        //            sb.Append("</nobr><br />");
        //        }

        //        sb.Append("<nobr>" + GetDate(DateAdded) + "</nobr><br />");
        //        return sb.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        return "&nbsp;";
        //    }
        //}
        public string GetLastPost(object userID, object userName, object dateAdded, object lastPostID, object parentPostID,
           object subject, object forumID, object replyCount, object firstName, object lastName, object displayName) {
            int UserID = (int)userID;
            string UserName = (string)userName;
            DateTime DateAdded = (DateTime)dateAdded;
            int LastPostID = (int)lastPostID;
            int ParentPostID = (int)parentPostID;
            string Subject = subject.ToString();
            int ForumID = (int)forumID;
            int ReplyCount = (int)replyCount;
            string FirstName = firstName.ToString();
            string LastName = lastName.ToString();
            string DisplayName = displayName.ToString();




            try {
                int PostId = LastPostID;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (!(UserName == "")) {
                    sb.Append("<nobr> [RESX:BY] ");
                    if (UserID == 0 || UserID == -1) {
                        sb.Append(UserName);
                    } else {
                        bool isMod = false;
                        if (CurrentUserType == CurrentUserTypes.Admin || CurrentUserType == CurrentUserTypes.ForumMod || CurrentUserType == CurrentUserTypes.SuperUser) {
                            isMod = true;
                        }
                        sb.Append(UserProfiles.GetDisplayName(ModuleId, MainSettings.ProfileVisibility, isMod, UserID, MainSettings.UserNameDisplay, UserName, FirstName, LastName, DisplayName));
                    }
                    sb.Append("</nobr><br />");
                }

                sb.Append("<nobr>" + GetDate(DateAdded) + "</nobr><br />");
                return sb.ToString();
            } catch (Exception ex) {
                return "&nbsp;";
            }
        }
        public string GetTopic(object forumID, object topicId, object contentId, object body,
            object dateUpdated, object subject, object userID, object replies, object forumName,
            object userName, object firstName, object lastName, object postType, object displayName) {

            int ForumID = (int)forumID;
            int ContentId = (int)contentId;
            int TopicId = (int)topicId;
            string Subject = (string)subject;
            string Body = (string)body;
            int UserID = (int)userID;
            int Replies = (int)replies;
            DateTime DateUpdated = (DateTime)dateUpdated;
            string ForumName = (string)forumName;
            string UserName = (string)userName;
            string FirstName = String.Empty;
            if (!String.IsNullOrEmpty(firstName.ToString())) {
                FirstName = firstName.ToString();
            }
            string LastName = String.Empty;
            if (!String.IsNullOrEmpty(lastName.ToString())) {
                LastName = lastName.ToString();
            }
            string PostType = String.Empty;
            if (!String.IsNullOrEmpty(postType.ToString())) {
                PostType = postType.ToString();
            }
            string DisplayName = String.Empty;
            if (!String.IsNullOrEmpty(displayName.ToString())) {
                DisplayName = displayName.ToString();
            }
         
            string sPollImage = "";
            if (PostType == "POLL") {
                sPollImage = "<img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MainSettings.Theme + "/poll.gif") + "\" align=absmiddle border=0 />";
            }
            string sOut = null;
            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ContentId };
            if (MainSettings.UseShortUrls) {
                Params = new string[] { ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ContentId };
            }
            sOut = "<a href=\"" + NavigateUrl(TabId, "", Params) + "\">" + sPollImage + Subject + "</a><br />";
            Body = Utilities.StripHTMLTag(Body);
            Body = Body.Replace(System.Environment.NewLine, " ");
            if (Body.Length > 250) {
                Body = Body.Substring(0, 249) + " ...";
            }
            sOut += "<span style=\"font-size:90%;\">" + Body + "</span><br />";
            sOut += "<span class=\"afsmalltext\">" + GetSharedResource("By.Text") + " " + GetDisplayName(UserName, UserID, FirstName, LastName, DisplayName) + " " + GetSharedResource("IN.Text") + " <a href=\"" + NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumID }) + "\">" + ForumName + "</a>";
            sOut += " " + GetSharedResource("On.Text") + " " + GetDate(DateUpdated) + "</span>";
            return sOut;
        }
      
        public string GetDisplayName(string Username, int UserID, string FirstName = "", string LastName = "", string DisplayName = "")
        {
            return UserProfiles.GetDisplayName(ModuleId, MainSettings.ProfileVisibility, false, UserID, MainSettings.UserNameDisplay, Username, FirstName, LastName, DisplayName);
        }
        public string GetRowCSS(object isRead) {
            bool IsRead = false;
            if (isRead.ToString() == "0") {
                IsRead = false;
            } else {
                IsRead = true;
            }


            if (IsRead == true) {
                return "aftopicrow";
            } else {
                return "aftopicrownew";
            }
        }
        public string GetRowAltCSS(object isRead) {
            bool IsRead = false;
            if (isRead.ToString() == "0") {
                IsRead = false;
            } else {
                IsRead = true;
            }

            if (IsRead == true) {
                return "aftopicrowalt";
            } else {
                return "aftopicrownewalt";
            }
        }
        //public string GetRowCSS(bool IsRead)
        //{
        //    if (IsRead == true)
        //    {
        //        return "aftopicrow";
        //    }
        //    else
        //    {
        //        return "aftopicrownew";
        //    }
        //}
        //public string GetRowAltCSS(bool IsRead)
        //{
        //    if (IsRead == true)
        //    {
        //        return "aftopicrowalt";
        //    }
        //    else
        //    {
        //        return "aftopicrownewalt";
        //    }
        //}
        private string GetSubPages(int TabID, int ModuleID, int Replies, int ForumID, int PostID) {
            int i = 0;
            string sOut = "";

            if (Replies + 1 > PageSize) {
                sOut = "&nbsp;<div class=\"afpagermini\">[RESX:SubPages]&nbsp;";
                //Jump to pages
                int intPostPages = 0;
                intPostPages = Convert.ToInt32(System.Math.Ceiling((double)(Replies + 1) / PageSize));
                if (intPostPages > 5) {
                    for (i = 1; i <= 5; i++) {
                        if (UseAjax) {
                            //afPageJump
                            //sOut &= "<span class=""afpagerminiitem"" onclick=""javascript:afPageJump(" & i & ");"">" & i & "</span>&nbsp;"
                            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageJumpId + "=" + i };
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", Params) + "\">" + i + "</a>&nbsp;";
                        } else {
                            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + i };
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", Params) + "\">" + i + "</a>&nbsp;";
                        }

                    }
                    if (intPostPages > 6) {
                        sOut += "...&nbsp;";
                    }
                    if (UseAjax) {
                        //sOut &= "<span class=""afpagerminiitem"" onclick=""javascript:afPageJump(" & intPostPages & ");"">" & intPostPages & "</span>&nbsp;"
                        string[] Params2 = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageJumpId + "=" + intPostPages };
                        sOut += "<a href=\"" + NavigateUrl(TabID, "", Params2) + "\">" + intPostPages + "</a>&nbsp;";
                    } else {
                        string[] Params2 = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + intPostPages };
                        sOut += "<a href=\"" + NavigateUrl(TabID, "", Params2) + "\">" + intPostPages + "</a>&nbsp;";
                    }


                } else {
                    for (i = 1; i <= intPostPages; i++) {
                        if (UseAjax) {
                            //sOut &= "<span class=""afpagerminiitem"" onclick=""javascript:afPageJump(" & i & ");"">" & i & "</span>&nbsp;"
                            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageJumpId + "=" + i };
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", Params) + "\">" + i + "</a>&nbsp;";
                        } else {
                            string[] Params = { ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.PageId + "=" + i };
                            sOut += "<a href=\"" + NavigateUrl(TabID, "", Params) + "\">" + i + "</a>&nbsp;";
                        }

                    }
                }

                sOut += "</div>";
            }
            return sOut;
        }
        #endregion

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class admin_categories : ActiveAdminBase
	{
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            agCategories.Callback += agCategories_Callback;
            agCategories.ItemBound += agCategories_ItemBound;
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			BindGroups();
		}

		private void agCategories_Callback(object sender, Controls.CallBackEventArgs e)
		{
			try
			{
				if (! (e.Parameters[4] == ""))
				{
					string sAction = e.Parameters[4].Split(':')[0];

					switch (sAction.ToUpper())
					{
						case "DELETE":
						{
							int TagId = Convert.ToInt32(e.Parameters[4].Split(':')[1]);
							if (SimulateIsNumeric.IsNumeric(TagId))
							{
								DataProvider.Instance().Tags_Delete(PortalId, ModuleId, TagId);
							}
							break;
						}
						case "SAVE":
						{
							string[] sParams = e.Parameters[4].Split(':');
							string TagName = sParams[1].Trim();
							int TagId = 0;
							int ForumId = -1;
							int ForumGroupId = -1;
							if (sParams.Length > 2)
							{
								TagId = Convert.ToInt32(sParams[2]);
							}
							if (sParams[3].Contains("FORUM"))
							{
								ForumId = Convert.ToInt32(sParams[3].Replace("FORUM", string.Empty));
							}
							if (sParams[3].Contains("GROUP"))
							{
								ForumGroupId = Convert.ToInt32(sParams[3].Replace("GROUP", string.Empty));
							}

							if (! (TagName == string.Empty))
							{
								DataProvider.Instance().Tags_Save(PortalId, ModuleId, TagId, TagName, 0, 0, 0, -1, true, ForumId, ForumGroupId);
							}



							break;
						}
					}

				}
				agCategories.DefaultParams = string.Empty;
				int PageIndex = Convert.ToInt32(e.Parameters[0]);
				int PageSize = Convert.ToInt32(e.Parameters[1]);
				string SortColumn = e.Parameters[2].ToString();
				string Sort = e.Parameters[3].ToString();
				agCategories.Datasource = DataProvider.Instance().Tags_List(PortalId, ModuleId, true, PageIndex, PageSize, Sort, SortColumn, -1, -1);
				agCategories.Refresh(e.Output);
			}
			catch (Exception ex)
			{

			}
		}
		private void BindGroups()
		{
			drpForums.Items.Add(new ListItem(Utilities.GetSharedResource("DropDownSelect"), "-1"));
			Data.ForumsDB fdb = new Data.ForumsDB();
			ForumCollection allForums = fdb.Forums_List(PortalId, ModuleId);
			ForumCollection filteredForums = new ForumCollection();
			foreach (Forum f in allForums)
			{
				if (f.ForumGroup.Active && f.Active && f.ParentForumId == 0)
				{
					f.TabId = TabId;
					f.SubForums = GetSubForums(allForums, f.ForumID);
					filteredForums.Add(f);
				}
			}
			int tmpGroupId = -1;
			foreach (Forum f in filteredForums)
			{
				if (! (tmpGroupId == f.ForumGroupId))
				{
					drpForums.Items.Add(new ListItem(f.GroupName, "GROUP" + f.ForumGroupId.ToString()));
					tmpGroupId = f.ForumGroupId;
				}
				drpForums.Items.Add(new ListItem(" - " + f.ForumName, "FORUM" + f.ForumID.ToString()));
				if (f.SubForums != null && f.SubForums.Count > 0)
				{
					foreach (Forum ff in f.SubForums)
					{
						drpForums.Items.Add(new ListItem(" ---- " + ff.ForumName, "FORUM" + ff.ForumID.ToString()));
					}
				}
			}
			//Dim dr As IDataReader = DataProvider.Instance.Forums_List(PortalId, ModuleId, -1, -1, False)


			//While dr.Read
			//    If Not tmpGroupId = CInt(dr("ForumGroupId")) Then
			//        drpForums.Items.Add(New ListItem(dr("GroupName").ToString, "GROUP" & dr("ForumGroupId").ToString))
			//        tmpGroupId = CInt(dr("ForumGroupId"))
			//    End If
			//    If Not CInt(dr("ForumId")) = 0 Then
			//        If CInt(dr("ParentForumID")) = 0 Then
			//            drpForums.Items.Add(New ListItem(" - " & dr("ForumName").ToString, "FORUM" & dr("ForumId").ToString))
			//        End If
			//        'If CInt(dr("ParentForumID")) > 0 Then
			//        '    drpForums.Items.Add(New ListItem(" ---- " & dr("ForumName").ToString, "FORUM" & dr("ForumId").ToString))
			//        'End If
			//    End If
			//End While
			//dr.Close()
		}
		private void agCategories_ItemBound(object sender, Modules.ActiveForums.Controls.ItemBoundEventArgs e)
		{
			//e.Item(1) = Server.HtmlEncode(e.Item(1).ToString)
			//e.Item(2) = Server.HtmlEncode(e.Item(2).ToString)
			e.Item[6] = "<img src=\"" + Page.ResolveUrl("~/desktopmodules/activeforums/images/delete16.png") + "\" alt=\"" + GetSharedResource("[RESX:Delete]") + "\" height=\"16\" width=\"16\" />";
		}
		private ForumCollection GetSubForums(ForumCollection forums, int forumId)
		{
			ForumCollection subforums = null;
			foreach (Forum s in forums)
			{
				if (s.ParentForumId == forumId)
				{
					if (subforums == null)
					{
						subforums = new ForumCollection();
					}
					s.TabId = TabId;
					subforums.Add(s);
				}
			}
			return subforums;
		}
	}
}

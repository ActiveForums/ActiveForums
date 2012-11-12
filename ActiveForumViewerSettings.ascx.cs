using System;
using System.Data;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class ActiveForumViewerSettings : Entities.Modules.ModuleSettingsBase
	{

		public override void LoadSettings()
		{
			try
			{
				if (! Page.IsPostBack)
				{
					// Load settings from TabModuleSettings: specific to this instance
					//Dim setting1 As String = CType(TabModuleSettings["settingname1"], String)
					LoadForums();
					// Load settings from ModuleSettings: general for all instances
					if (! (Convert.ToString(Settings["AFForumModuleID"]) == null))
					{
						drpForumInstance.SelectedIndex = drpForumInstance.Items.IndexOf(drpForumInstance.Items.FindByValue(Convert.ToString(Settings["AFForumModuleID"])));
						LoadForumGroups(Convert.ToInt32(Settings["AFForumModuleID"]));
					}
					if (! (Convert.ToString(Settings["AFTopicsTemplate"]) == null))
					{
						BindTemplates(Convert.ToInt32(Settings["AFForumModuleID"]));
						drpTopicsTemplate.SelectedIndex = drpTopicsTemplate.Items.IndexOf(drpTopicsTemplate.Items.FindByValue(Convert.ToString(Settings["AFTopicsTemplate"])));
					}
					if (! (Convert.ToString(Settings["AFForumViewTemplate"]) == null))
					{
						drpForumViewTemplate.SelectedIndex = drpForumViewTemplate.Items.IndexOf(drpForumViewTemplate.Items.FindByValue(Convert.ToString(Settings["AFForumViewTemplate"])));
					}
					if (! (Convert.ToString(Settings["AFTopicTemplate"]) == null))
					{
						drpTopicTemplate.SelectedIndex = drpTopicTemplate.Items.IndexOf(drpTopicTemplate.Items.FindByValue(Convert.ToString(Settings["AFTopicTemplate"])));
					}
					if (! (Convert.ToString(Settings["AFForumGroup"]) == null))
					{
						drpForum.SelectedIndex = drpForum.Items.IndexOf(drpForum.Items.FindByValue(Convert.ToString(Settings["AFForumGroup"])));
					}
					//If Not CType(Settings["AFEnableToolbar"], String) Is Nothing Then
					//    chkEnableToolbar.Checked = CType(Settings["AFEnableToolbar"], Boolean)
					//End If
				}
			}
			catch (Exception exc)
			{
				Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
			}
		}
		public override void UpdateSettings()
		{
			try
			{
				var objModules = new Entities.Modules.ModuleController();
				// Update ModuleSettings
				objModules.UpdateModuleSetting(ModuleId, "AFTopicsTemplate", drpTopicsTemplate.SelectedItem.Value);
				objModules.UpdateModuleSetting(ModuleId, "AFTopicTemplate", drpTopicTemplate.SelectedItem.Value);
				objModules.UpdateModuleSetting(ModuleId, "AFForumViewTemplate", drpForumViewTemplate.SelectedItem.Value);
				objModules.UpdateModuleSetting(ModuleId, "AFForumModuleID", drpForumInstance.SelectedItem.Value);
				objModules.UpdateModuleSetting(ModuleId, "AFForumGroup", drpForum.SelectedItem.Value);
				//objModules.UpdateModuleSetting(ModuleId, "AFEnableToolbar", CType(chkEnableToolbar.Checked, String))
				string ForumGroup;
				int ForumGroupID = 0;
				ForumGroup = drpForum.SelectedItem.Value;
				if ((ForumGroup.IndexOf("GROUPID:", 0) + 1) > 0)
				{
					objModules.UpdateModuleSetting(ModuleId, "AFViewType", "AFGROUP");
				}
				else
				{
					objModules.UpdateModuleSetting(ModuleId, "AFViewType", "TOPICS");
				}
				int @int = ForumGroup.IndexOf(":") + 1;
				string sID = ForumGroup.Substring(@int);
				//ForumGroupID = CType(ForumGroup.Substring(ForumGroup.IndexOf(":")), Integer)
				objModules.UpdateModuleSetting(ModuleId, "AFForumGroupID", sID);
				DataCache.CacheClear(drpForumInstance.SelectedItem.Value + TabId + sID + "TopicTemplate");
				DataCache.CacheClear(drpForumInstance.SelectedItem.Value + TabId + sID + "TopicsTemplate");
				DataCache.CacheClear(drpForumInstance.SelectedItem.Value + TabId + "ForumTemplate");
				// Redirect back to the portal home page
				Response.Redirect(Utilities.NavigateUrl(TabId), true);
			}
			catch (Exception exc)
			{
				Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
			}
		}
		public void LoadForums()
		{
			//Dim dr As IDataReader
			//dr = DataProvider.Instance.PortalForums(PortalId)
			//Dim i As Integer = 0
			//While dr.Read
			//    drpForumInstance.Items.Insert(i, New Web.UI.WebControls.ListItem(CType(dr("TabName"), String), CType(dr("ModuleID"), String)))
			//End While
			//dr.Close()
			int i = 0;
			var mc = new Entities.Modules.ModuleController();
			var tc = new Entities.Tabs.TabController();
			Entities.Tabs.TabInfo ti;
			foreach (Entities.Modules.ModuleInfo mi in mc.GetModules(PortalId))
			{
                if (mi.DesktopModule.ModuleName.Trim().ToLowerInvariant() == "Active Forums".ToLowerInvariant() && mi.IsDeleted == false)
				{
					ti = tc.GetTab(mi.TabID, PortalId, false);
					if (ti != null)
					{
						if (ti.IsDeleted == false)
						{
							drpForumInstance.Items.Insert(i, new ListItem(ti.TabName + " - " + mi.DesktopModule.ModuleName, Convert.ToString(mi.ModuleID)));
							i += 1;
						}
					}

				}
			}

			drpForumInstance.Items.Insert(0, new ListItem("-- Select a Forum Instance --", "-1"));
		}
		public void LoadForumGroups(int ForumModuleID)
		{
			drpForum.Items.Insert(0, new ListItem("-- Select a Group or Forum --", "-1"));
			IDataReader dr = DataProvider.Instance().Forums_List(PortalId, ForumModuleID, -1, -1, false);
			int i = 1;
			string GroupName = string.Empty;
			string ForumName = string.Empty;
			int ForumID = -1;
			while (dr.Read())
			{
				if (GroupName != Convert.ToString(dr["GroupName"]))
				{
					drpForum.Items.Insert(i, new ListItem(Convert.ToString(dr["GroupName"]), "GROUPID:" + Convert.ToString(dr["ForumGroupID"])));
					i += 1;
					GroupName = Convert.ToString(dr["GroupName"]);
				}


				if (ForumID != Convert.ToInt32(dr["ForumID"]))
				{
					drpForum.Items.Insert(i, new ListItem("|---" + Convert.ToString(dr["ForumName"]), "FORUMID:" + Convert.ToString(dr["ForumID"])));
					i += 1;
					ForumID = Convert.ToInt32(dr["ForumID"]);
				}


			}
			dr.Close();

		}
		private void BindTemplates(int ForumModuleID)
		{
			string sDefault = Utilities.GetSharedResource("[RESX:Default]", true);
			BindTemplateDropDown(ForumModuleID, drpTopicsTemplate, Templates.TemplateTypes.TopicsView, sDefault, "0");
			BindTemplateDropDown(ForumModuleID, drpTopicTemplate, Templates.TemplateTypes.TopicView, sDefault, "0");
			BindTemplateDropDown(ForumModuleID, drpForumViewTemplate, Templates.TemplateTypes.ForumView, sDefault, "0");
		}
		private void drpForumInstance_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindTemplates(Convert.ToInt32(drpForumInstance.SelectedItem.Value));
			LoadForumGroups(Convert.ToInt32(drpForumInstance.SelectedItem.Value));
		}
		public void BindTemplateDropDown(int ForumModuleId, DropDownList drp, Templates.TemplateTypes TemplateType, string DefaultText, string DefaultValue)
		{
			var tc = new TemplateController();
			drp.DataTextField = "Title";
			drp.DataValueField = "TemplateID";
			drp.DataSource = tc.Template_List(PortalId, ForumModuleId, TemplateType);
			drp.DataBind();
			drp.Items.Insert(0, new ListItem(DefaultText, DefaultValue));
		}

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            drpForumInstance.SelectedIndexChanged += drpForumInstance_SelectedIndexChanged;
        }
	}
}
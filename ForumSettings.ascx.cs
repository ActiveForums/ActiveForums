using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using DotNetNuke.Framework;
using System.Text;
using System.Xml;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public partial class ForumSettings : ForumSettingsBase
	{

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

			txtPageSize.Style.Add("float", "none");
			txtPageSize.EmptyMessageStyle.CssClass += "dnnformHint";
			txtPageSize.NumberFormat.DecimalDigits = 0;
			txtPageSize.IncrementSettings.Step = 5;


			txtFloodInterval.Style.Add("float", "none");
			txtFloodInterval.EmptyMessageStyle.CssClass += "dnnformHint";
			txtFloodInterval.NumberFormat.DecimalDigits = 0;
			txtFloodInterval.IncrementSettings.Step = 30;


			txtEditInterval.Style.Add("float", "none");
			txtEditInterval.EmptyMessageStyle.CssClass += "dnnformHint";
			txtEditInterval.NumberFormat.DecimalDigits = 0;
			txtEditInterval.IncrementSettings.Step = 1;


			if (!(Utilities.IsRewriteLoaded()) || PortalSettings.PortalAlias.HTTPAlias.Contains("/"))
			{
				rdEnableURLRewriter.SelectedIndex = 1;
				rdEnableURLRewriter.Enabled = false;
				rdEnableURLRewriter.Enabled = false;
			}
			var u = Entities.Users.UserController.GetCurrentUserInfo();

			if (u.IsSuperUser & (Request.ServerVariables["SERVER_SOFTWARE"].Contains("7") || Request.ServerVariables["SERVER_SOFTWARE"].Contains("8")) & !(PortalSettings.PortalAlias.HTTPAlias.Contains("/")))
			{
				if (Utilities.IsRewriteLoaded())
				{
					litToggleConfig.Text = "<a href=\"javascript:void(0);\" onclick=\"amaf_toggleConfig('configdisable',this); return false;\">Uninstall Active Forums URL Handler</a>";
				}
				else
				{
					litToggleConfig.Text = "<a href=\"javascript:void(0);\" onclick=\"amaf_toggleConfig('configenable',this); return false;\">Install Active Forums URL Handler</a>";
				}

			}


		}
		#region Base Method Implementations

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// LoadSettings loads the settings from the Database and displays them
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// </history>
		/// -----------------------------------------------------------------------------
		public override void LoadSettings()
		{
			try
			{
				if (Page.IsPostBack == false)
				{
					BindThemes();
					BindTemplates();
					BindPrivateMessaging();
					BindForumGroups();
					BindForumSecurity();


					rdFullTextSearch.SelectedIndex = 1;
					rdMailQueue.SelectedIndex = 1;
					rdAutoLinks.SelectedIndex = 0;
					rdPoints.SelectedIndex = 1;

					txtPageSize.Value = 20;
					txtPageSize.Text = "20";
					txtFloodInterval.Value = 0;
					txtFloodInterval.Text = "0";
					txtEditInterval.Value = 0;
					txtEditInterval.Text = "0";

					drpMode.SelectedIndex = drpMode.Items.IndexOf(drpMode.Items.FindByValue(Mode));
					drpThemes.SelectedIndex = drpThemes.Items.IndexOf(drpThemes.Items.FindByValue(Theme));
					drpTemplates.SelectedIndex = drpTemplates.Items.IndexOf(drpTemplates.Items.FindByValue(TemplateId.ToString()));
					txtPageSize.Text = PageSize.ToString();
					txtFloodInterval.Text = FloodInterval.ToString();
					txtEditInterval.Text = EditInterval.ToString();
					rdAutoLinks.SelectedIndex = rdAutoLinks.Items.IndexOf(rdAutoLinks.Items.FindByValue(AutoLink.ToString()));
					drpDeleteBehavior.SelectedIndex = drpDeleteBehavior.Items.IndexOf(drpDeleteBehavior.Items.FindByValue(DeleteBehavior.ToString()));
					txtAddThis.Text = AddThis;
					drpProfileType.SelectedIndex = drpProfileType.Items.IndexOf(drpProfileType.Items.FindByValue(ProfileType.ToString()));
					drpSignatures.SelectedIndex = drpSignatures.Items.IndexOf(drpSignatures.Items.FindByValue(Signatures.ToString()));
					drpUserDisplayMode.SelectedIndex = drpUserDisplayMode.Items.IndexOf(drpUserDisplayMode.Items.FindByValue(UserNameDisplay));
					rdEnableURLRewriter.SelectedIndex = rdEnableURLRewriter.Items.IndexOf(rdEnableURLRewriter.Items.FindByValue(FriendlyURLs.ToString()));
					rdFullTextSearch.SelectedIndex = rdFullTextSearch.Items.IndexOf(rdFullTextSearch.Items.FindByValue(FullTextSearch.ToString()));
					rdMailQueue.SelectedIndex = rdMailQueue.Items.IndexOf(rdMailQueue.Items.FindByValue(MailQueue.ToString()));

					rdPoints.SelectedIndex = rdPoints.Items.IndexOf(rdPoints.Items.FindByValue(EnablePoints.ToString().ToLower()));
					txtAnswerPointValue.Text = AnswerPointValue.ToString();
					txtTopicPointValue.Text = TopicPointValue.ToString();
					txtReplyPointValue.Text = ReplyPointValue.ToString();
					txtMarkAnswerPointValue.Text = MarkAsAnswerPointValue.ToString();
					txtModPointValue.Text = ModPointValue.ToString();

					txtURLPrefixBase.Text = PrefixURLBase;
					txtURLPrefixCategory.Text = PrefixURLCategory;
					txtURLPrefixOther.Text = PrefixURLOther;
					txtURLPrefixTags.Text = PrefixURLTag;

					txtAvatarHeight.Text = AvatarHeight.ToString();
					txtAvatarWidth.Text = AvatarWidth.ToString();

					drpForumGroupTemplate.SelectedIndex = drpForumGroupTemplate.Items.IndexOf(drpForumGroupTemplate.Items.FindByValue(ForumGroupTemplate.ToString()));


				}
			}
			catch (Exception exc) //Module failed to load
			{
				Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// UpdateSettings saves the modified settings to the Database
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// </history>
		/// -----------------------------------------------------------------------------
		public override void UpdateSettings()
		{
			try
			{
				bool fullTextCurrent = FullTextSearch;


				Theme = drpThemes.SelectedItem.Value;
				Mode = drpMode.SelectedItem.Value;
				TemplateId = Convert.ToInt32(drpTemplates.SelectedItem.Value);
				PageSize = Convert.ToInt32(txtPageSize.Text);
				FloodInterval = Convert.ToInt32(txtFloodInterval.Text);
				EditInterval = Convert.ToInt32(txtEditInterval.Text);
				AutoLink = Convert.ToBoolean(rdAutoLinks.SelectedItem.Value);
				DeleteBehavior = Convert.ToInt32(drpDeleteBehavior.SelectedItem.Value);
				AddThis = txtAddThis.Text;
				ProfileType = Convert.ToInt32(drpProfileType.SelectedItem.Value);
				Signatures = Convert.ToInt32(drpSignatures.SelectedItem.Value);
				UserNameDisplay = drpUserDisplayMode.SelectedItem.Value;
				FriendlyURLs = Convert.ToBoolean(rdEnableURLRewriter.SelectedItem.Value);
				FullTextSearch = Convert.ToBoolean(rdFullTextSearch.SelectedItem.Value);
				MailQueue = Convert.ToBoolean(rdMailQueue.SelectedItem.Value);

				PrefixURLBase = txtURLPrefixBase.Text;
				PrefixURLCategory = txtURLPrefixCategory.Text;
				PrefixURLOther = txtURLPrefixOther.Text;
				PrefixURLTag = txtURLPrefixTags.Text;


				EnablePoints = Convert.ToBoolean(rdPoints.SelectedItem.Value);
				AnswerPointValue = Convert.ToInt32(txtAnswerPointValue.Text);
				ReplyPointValue = Convert.ToInt32(txtReplyPointValue.Text);
				MarkAsAnswerPointValue = Convert.ToInt32(txtMarkAnswerPointValue.Text);
				TopicPointValue = Convert.ToInt32(txtTopicPointValue.Text);
				ModPointValue = Convert.ToInt32(txtModPointValue.Text);

				AvatarHeight = Convert.ToInt32(txtAvatarHeight.Text);
				AvatarWidth = Convert.ToInt32(txtAvatarWidth.Text);


				ForumGroupTemplate = Convert.ToInt32(drpForumGroupTemplate.SelectedItem.Value);
				string[] adminSec = txtGroupModSec.Value.Split(',');
				SaveForumSecurity("groupadmin", adminSec);
				string[] memSec = txtGroupMemSec.Value.Split(',');
				SaveForumSecurity("groupmember", memSec);
				string[] regSec = txtGroupRegSec.Value.Split(',');
				SaveForumSecurity("registereduser", regSec);
				string[] anonSec = txtGroupAnonSec.Value.Split(',');
				SaveForumSecurity("anon", anonSec);


				try
				{
					if (FullTextSearch && fullTextCurrent == false)
					{
						DataProvider.Instance().Search_ManageFullText(FullTextSearch);
						string err = string.Empty;
                        string s = Utilities.GetSqlString("DotNetNuke.Modules.ActiveForums.FullText.sql");
						err = DotNetNuke.Data.DataProvider.Instance().ExecuteScript(s);
					}
					else if (FullTextSearch == false && fullTextCurrent)
					{
						DataProvider.Instance().Search_ManageFullText(FullTextSearch);
					}
				}
				catch (Exception ex)
				{
					FullTextSearch = false;
					Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
				}

			}
			catch (Exception exc) //Module failed to load
			{
				Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		#endregion
		#region Private Methods
		private void BindTemplates()
		{
			var tc = new TemplateController();
		    List<TemplateInfo> tl = tc.Template_List(PortalId, ModuleId, Templates.TemplateTypes.ForumView);
			drpTemplates.DataTextField = "Title";
			drpTemplates.DataValueField = "TemplateId";
			drpTemplates.DataSource = tl;
			drpTemplates.DataBind();
			drpTemplates.Items.Insert(0, new ListItem(LocalizeString("Default"), "0"));
		}
		private void BindThemes()
		{
			var di = new System.IO.DirectoryInfo(Server.MapPath("~/DesktopModules/ActiveForums/themes"));
			drpThemes.DataSource = di.GetDirectories();
			drpThemes.DataBind();
		}
		private void BindPrivateMessaging()
		{

			var mc = new Entities.Modules.ModuleController();
			var tc = new Entities.Tabs.TabController();
			Entities.Tabs.TabInfo ti;
			foreach (Entities.Modules.ModuleInfo mi in mc.GetModules(PortalId))
			{
				if (mi.DesktopModule.ModuleName.Contains("Active Social") && mi.IsDeleted == false)
				{
					ti = tc.GetTab(mi.TabID, PortalId, false);
					if (ti != null)
					{
						if (ti.IsDeleted == false)
						{
							drpProfileType.Items.Add(new ListItem(ti.TabName + " - Active Social", "3"));

						}
					}
				}
			}
		}
		private void BindForumGroups()
		{
			using (IDataReader dr = DataProvider.Instance().Forums_List(PortalId, ModuleId, -1, -1, false))
			{
				var dt = new DataTable("Forums");
				dt.Load(dr);
				dr.Close();

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
						drpForumGroupTemplate.Items.Add(new ListItem(row["GroupName"].ToString(), row["ForumGroupId"].ToString()));
						tmpGroup = row["ForumGroupId"].ToString();
					}

				}

			}
		}
		private void BindForumSecurity()
		{
			var xDoc = new XmlDocument();
			if (string.IsNullOrEmpty(ForumConfig))
			{
				xDoc.Load(Server.MapPath(Globals.ModulePath + "config/defaultgroupforums.config"));
			}
			else
			{
				xDoc.LoadXml(ForumConfig);
			}
            
			if (xDoc != null)
			{
				XmlNode xRoot = xDoc.DocumentElement;
				var xNodeList = xRoot.SelectSingleNode("//defaultforums/forum/security[@type='groupadmin']").ChildNodes;
				var sb = new StringBuilder();
				sb.Append("<table cellpadding=\"0\" cellspacing=\"0\">");
				var rows = new string[16, 5];
				int i = 0;
				foreach (XmlNode x in xNodeList)
				{
					rows[i, 0] = x.Name;
					rows[i, 1] = x.Attributes["value"].Value;
					i += 1;
				}
				i = 0;
				xNodeList = xRoot.SelectSingleNode("//defaultforums/forum/security[@type='groupmember']").ChildNodes;
				foreach (XmlNode x in xNodeList)
				{
					rows[i, 2] = x.Attributes["value"].Value;
					i += 1;
				}
				i = 0;
				xNodeList = xRoot.SelectSingleNode("//defaultforums/forum/security[@type='registereduser']").ChildNodes;
				foreach (XmlNode x in xNodeList)
				{
					rows[i, 3] = x.Attributes["value"].Value;
					i += 1;
				}
				i = 0;
				xNodeList = xRoot.SelectSingleNode("//defaultforums/forum/security[@type='anon']").ChildNodes;
				foreach (XmlNode x in xNodeList)
				{
					rows[i, 4] = x.Attributes["value"].Value;
					i += 1;
				}
				i = 0;
				sb.Append("<tr id=\"hd1\"><td></td><td colspan=\"10\" class=\"afgridhd sec1\">" + LocalizeString("UserPermissions") + "</td><td colspan=\"6\" class=\"afgridhd sec2\">" + LocalizeString("ModeratorPermissions") + "</td></tr>");
				sb.Append("<tr id=\"hd2\"><td></td>");
				string sClass;
				for (i = 0; i <= 15; i++)
				{
					sClass = "afgridhdsub";
					if (i == 0)
					{
						sClass += " colstart";
					}
					else if (i == 15)
					{
						sClass += " colend";
					}
					else if (i == 9)
					{
						sClass += " gridsep";

					}
					sb.Append("<td class=\"" + sClass + "\">");
					sb.Append(LocalizeString("SecGrid:" + rows[i, 0]));
					sb.Append("</td>");
				}
				sb.Append("</tr><tr id=\"row1\"><td class=\"rowhd\">" + LocalizeString("GroupAdmin") + "</td>");
				i = 0;

				for (i = 0; i <= 15; i++)
				{
					sClass = "gridcheck";
					if (i <= 9)
					{
						sClass += " sec1";
					}
					else
					{
						sClass += " sec2";
					}
					if (i == 15)
					{
						sClass += " colend";
					}
					if (i == 9)
					{
						//sClass &= " gridsep"
					}
					sb.Append("<td align=\"center\" class=\"" + sClass + "\">");

					if (rows[i, 1] == "true")
					{
						sb.Append("<input type=\"checkbox\" id=\"ga" + rows[i, 0] + "\" checked=\"checked\" />");
					}
					else
					{
						sb.Append("<input type=\"checkbox\" id=\"ga" + rows[i, 0] + "\" />");
					}
					sb.Append("</td>");
				}
				sb.Append("</tr>");
				i = 0;
				sb.Append("<tr id=\"row2\"><td class=\"rowhd\">" + LocalizeString("GroupMember") + "</td>");
				for (i = 0; i <= 15; i++)
				{
					sClass = "gridcheck";
					if (i <= 9)
					{
						sClass += " sec1";
					}
					else
					{
						sClass += " sec2";
					}
					if (i == 15)
					{
						sClass += " colend";
					}
					if (i == 9)
					{
						//sClass &= " gridsep"
					}
					sb.Append("<td align=\"center\" class=\"" + sClass + "\">");
					if (rows[i, 2] == "true")
					{
						sb.Append("<input type=\"checkbox\" id=\"gm" + rows[i, 0] + "\" checked=\"checked\" />");
					}
					else
					{
						sb.Append("<input type=\"checkbox\" id=\"gm" + rows[i, 0] + "\" />");
					}

					sb.Append("</td>");
				}
				sb.Append("</tr>");

				i = 0;
				sb.Append("<tr id=\"row3\"><td class=\"rowhd\">" + LocalizeString("RegisteredUser") + "</td>");
				for (i = 0; i <= 15; i++)
				{
					sClass = "gridcheck";
					if (i <= 9)
					{
						sClass += " sec1";
					}
					else
					{
						sClass += " sec2";
					}
					if (i == 15)
					{
						sClass += " colend";
					}
					if (i == 9)
					{
						//sClass &= " gridsep"
					}
					sb.Append("<td align=\"center\" class=\"" + sClass + "\">");
					if (rows[i, 3] == "true")
					{
						sb.Append("<input type=\"checkbox\" id=\"gr" + rows[i, 0] + "\" checked=\"checked\" />");
					}
					else
					{
						sb.Append("<input type=\"checkbox\" id=\"gr" + rows[i, 0] + "\" />");
					}

					sb.Append("</td>");
				}
				sb.Append("</tr>");
				i = 0;
				sb.Append("<tr id=\"row4\"><td class=\"rowhd\">" + LocalizeString("Anon") + "</td>");
				for (i = 0; i <= 15; i++)
				{
					sClass = "gridcheck";
					if (i <= 9)
					{
						sClass += " sec1";
					}
					else
					{
						sClass += " sec2";
					}
					if (i == 15)
					{
						sClass += " colend";
					}
					if (i == 9)
					{
						//sClass &= " gridsep"
					}
					sb.Append("<td align=\"center\" class=\"" + sClass + "\">");
					if (rows[i, 4] == "true")
					{
						sb.Append("<input type=\"checkbox\" id=\"gn" + rows[i, 0] + "\" checked=\"checked\" />");
					}
					else
					{
						sb.Append("<input type=\"checkbox\" id=\"gn" + rows[i, 0] + "\" />");
					}

					sb.Append("</td>");
				}
				sb.Append("</tr>");

				sb.Append("</table>");
				litForumSecurity.Text = sb.ToString();
			}
		}
		private void SaveForumSecurity(string sectype, string[] security)
		{
			var xDoc = new XmlDocument();
			if (string.IsNullOrEmpty(ForumConfig))
			{
				xDoc.Load(Server.MapPath(Globals.ModulePath + "config/defaultgroupforums.config"));
			}
			else
			{
				xDoc.LoadXml(ForumConfig);
			}
			XmlNode xRoot = xDoc.DocumentElement;
			var xNode = xRoot.SelectSingleNode("//defaultforums/forum/security[@type='" + sectype + "']");
			foreach (string s in security)
			{
				if (!(string.IsNullOrEmpty(s)))
				{
					string nodeName = s.Split('=')[0];
					string nodeValue = s.Split('=')[1];
					xNode[nodeName].Attributes["value"].Value = nodeValue;
				}
			}
			ForumConfig = xDoc.OuterXml;

		}
		#endregion

	}
}
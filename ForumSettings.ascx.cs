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
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using System.Text;
using System.Xml;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public partial class ForumSettings : ForumSettingsBase
	{

	    private int? _fullTextStatus;
        
	    private int FullTextStatus
	    {
	        get
	        {
	            if(!_fullTextStatus.HasValue)
	            {
                    _fullTextStatus = DataProvider.Instance().Search_GetFullTextStatus();
	            }

	            return _fullTextStatus.HasValue ? _fullTextStatus.Value : -5;
	        }
	    }

	    private bool IsFullTextAvailable
	    {
	        get
	        {
	            return FullTextStatus != -5 && FullTextStatus != -4 && FullTextStatus != 0;
	        }
	    }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

			drpPageSize.Style.Add("float", "none");



			drpFloodInterval.Style.Add("float", "none");


			drpEditInterval.Style.Add("float", "none");



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

            // Full Text
		    rdFullTextSearch.Enabled = IsFullTextAvailable;
            switch (FullTextStatus)
            {
                case -4:
                    ltrFullTextMessage.Text = LocalizeString("FullTextAzure");
                    ltrFullTextMessage.Visible = true;
                    break;
                case 0:
                    ltrFullTextMessage.Text = LocalizeString("FullTextNotInstalled");
                    ltrFullTextMessage.Visible = true;
                    break;
                default:
                    ltrFullTextMessage.Visible = false;
                    break;
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
            // Note, this is called before OnLoad

			try
			{
				//if (Page.IsPostBack == false)
				//{
					BindThemes();
					BindTemplates();
					BindPrivateMessaging();
					BindForumGroups();
					BindForumSecurity();

                    Utilities.SelectListItemByValue(drpPageSize, 20);
                    Utilities.SelectListItemByValue(drpFloodInterval, 0);
                    Utilities.SelectListItemByValue(drpEditInterval, 0);


                    Utilities.SelectListItemByValue(drpMode, Mode);
                    Utilities.SelectListItemByValue(drpThemes, Theme);
                    Utilities.SelectListItemByValue(drpTemplates, TemplateId);

					Utilities.SelectListItemByValue(rdAutoLinks, AutoLink);
                    Utilities.SelectListItemByValue(drpDeleteBehavior, DeleteBehavior);
					txtAddThis.Text = AddThis;
                    Utilities.SelectListItemByValue(drpProfileVisibility, ProfileVisibility);
                    Utilities.SelectListItemByValue(drpSignatures, Signatures);
                    Utilities.SelectListItemByValue(drpUserDisplayMode, UserNameDisplay);
                    Utilities.SelectListItemByValue(rdEnableURLRewriter, FriendlyURLs);

                    Utilities.SelectListItemByValue(rdFullTextSearch, FullTextSearch && FullTextStatus == 1); // 1 = Enabled Status

                    Utilities.SelectListItemByValue(rdMailQueue, MailQueue);
                    Utilities.SelectListItemByValue(rdPoints, EnablePoints);
                    Utilities.SelectListItemByValue(rdUsersOnline, EnableUsersOnline);
                    Utilities.SelectListItemByValue(rdUseSkinBreadCrumb, UseSkinBreadCrumb);

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

				    txtTimeFormat.Text = TimeFormatString;
				    txtDateFormat.Text = DateFormatString;

                    Utilities.SelectListItemByValue(drpForumGroupTemplate, ForumGroupTemplate);
				//}
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
				Theme = drpThemes.SelectedValue;
				Mode = drpMode.SelectedValue;
				TemplateId = Utilities.SafeConvertInt(drpTemplates.SelectedValue);
				PageSize = Utilities.SafeConvertInt(drpPageSize.SelectedValue, 10);
                FloodInterval = Utilities.SafeConvertInt(drpFloodInterval.SelectedValue,0);
                EditInterval = Utilities.SafeConvertInt(drpEditInterval.SelectedValue,0);
                AutoLink = Utilities.SafeConvertBool(rdAutoLinks.SelectedValue);
                DeleteBehavior = Utilities.SafeConvertInt(drpDeleteBehavior.SelectedValue);
				AddThis = txtAddThis.Text;
                ProfileVisibility = Utilities.SafeConvertInt(drpProfileVisibility.SelectedValue);
                Signatures = Utilities.SafeConvertInt(drpSignatures.SelectedValue);
                UserNameDisplay = drpUserDisplayMode.SelectedValue;
                FriendlyURLs = Utilities.SafeConvertBool(rdEnableURLRewriter.SelectedValue);
                FullTextSearch = Utilities.SafeConvertBool(rdFullTextSearch.SelectedValue);
                MailQueue = Utilities.SafeConvertBool(rdMailQueue.SelectedValue);

                MessagingType = Utilities.SafeConvertInt(drpMessagingType.SelectedValue);

			    EnableUsersOnline = Utilities.SafeConvertBool(rdUsersOnline.SelectedValue);
			    UseSkinBreadCrumb = Utilities.SafeConvertBool(rdUseSkinBreadCrumb.SelectedValue);

                if(drpMessagingTab.SelectedItem != null)
                    MessagingTabId = Utilities.SafeConvertInt(drpMessagingTab.SelectedValue);              

				PrefixURLBase = txtURLPrefixBase.Text;
				PrefixURLCategory = txtURLPrefixCategory.Text;
				PrefixURLOther = txtURLPrefixOther.Text;
				PrefixURLTag = txtURLPrefixTags.Text;

                EnablePoints = Utilities.SafeConvertBool(rdPoints.SelectedValue);
                AnswerPointValue = Utilities.SafeConvertInt(txtAnswerPointValue.Text, 1);
                ReplyPointValue = Utilities.SafeConvertInt(txtReplyPointValue.Text, 1);
                MarkAsAnswerPointValue = Utilities.SafeConvertInt(txtMarkAnswerPointValue.Text, 1);
                TopicPointValue = Utilities.SafeConvertInt(txtTopicPointValue.Text, 1);
                ModPointValue = Utilities.SafeConvertInt(txtModPointValue.Text, 1);

                AvatarHeight = Utilities.SafeConvertInt(txtAvatarHeight.Text, 48);
                AvatarWidth = Utilities.SafeConvertInt(txtAvatarWidth.Text, 48);

			    TimeFormatString = !string.IsNullOrWhiteSpace(txtTimeFormat.Text) ? txtTimeFormat.Text : "h:mm tt";
                DateFormatString = !string.IsNullOrWhiteSpace(txtDateFormat.Text) ? txtDateFormat.Text : "M/d/yyyy";

                ForumGroupTemplate = Utilities.SafeConvertInt(drpForumGroupTemplate.SelectedValue);
				var adminSec = txtGroupModSec.Value.Split(',');
				SaveForumSecurity("groupadmin", adminSec);
				var memSec = txtGroupMemSec.Value.Split(',');
				SaveForumSecurity("groupmember", memSec);
				var regSec = txtGroupRegSec.Value.Split(',');
				SaveForumSecurity("registereduser", regSec);
				var anonSec = txtGroupAnonSec.Value.Split(',');
				SaveForumSecurity("anon", anonSec);

				try
				{
					if (IsFullTextAvailable && FullTextSearch && FullTextStatus != 1) // Available, selected and not currently installed
					{
                        // Note: We have to jump through some hoops here to maintain Azure compatibility and prevent a race condition in the procs.

                        // Create the full text manager proc
					    var fullTextInstallScript = Utilities.GetSqlString("DotNetNuke.Modules.ActiveForums.sql.FullTextInstallPart1.sql");
                        var result = DotNetNuke.Data.DataProvider.Instance().ExecuteScript(fullTextInstallScript);

                        // Exectute the full text manager proc to setup the search indexes
                        DataProvider.Instance().Search_ManageFullText(true);

                        // Create the full text search proc (can't be reliably created until the indexes are in place)
                        fullTextInstallScript = Utilities.GetSqlString("DotNetNuke.Modules.ActiveForums.sql.FullTextInstallPart2.sql");
                        DotNetNuke.Data.DataProvider.Instance().ExecuteScript(fullTextInstallScript);
					}
					else if (IsFullTextAvailable && !FullTextSearch) // Available, but not selected
					{
                        // Remove the search indexes if they exist
						DataProvider.Instance().Search_ManageFullText(false);
					}
				}
				catch (Exception ex)
				{
					FullTextSearch = false;
					Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
				}

                // Clear out the cache
                DataCache.ClearSettingsCache(ModuleId);

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
		    var tl = tc.Template_List(PortalId, ModuleId, Templates.TemplateTypes.ForumView);
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
            var selectedMessagingType = drpMessagingType.Items.FindByValue(MessagingType.ToString());
            if (selectedMessagingType != null)
                selectedMessagingType.Selected = true;

            BindPrivateMessagingTab();
        }

        private void BindPrivateMessagingTab()
        {
            drpMessagingTab.Items.Clear();
            drpMessagingTab.ClearSelection();

            var mc = new Entities.Modules.ModuleController();
            var tc = new TabController();

            foreach (Entities.Modules.ModuleInfo mi in mc.GetModules(PortalId))
            {
                if (!mi.DesktopModule.ModuleName.Contains("DnnForge - PrivateMessages") || mi.IsDeleted)
                    continue;

                var ti = tc.GetTab(mi.TabID, PortalId, false);
                if (ti != null && !ti.IsDeleted)
                {
                    drpMessagingTab.Items.Add(new ListItem
                    {
                        Text = ti.TabName + " - Ventrian Messages",
                        Value = ti.TabID.ToString(),
                        Selected = ti.TabID == MessagingTabId
                    });
                }
            }

            if (drpMessagingTab.Items.Count == 0)
            {
                drpMessagingTab.Items.Add(new ListItem("No Messaging Tabs Found", "-1"));
                drpMessagingTab.Enabled = false;
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
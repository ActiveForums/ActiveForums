using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace Active.Modules.Forums.Controls
{
	[ToolboxData("<{0}:TopicDisplay runat=server></{0}:TopicDisplay>")]
	public class TopicDisplay : ControlsBase
	{
		#region "Private Members"
		private bool bRead = true;
		private bool bReply = true;
		private bool bLocked = false;
		private int PageSize = 20;

		private int RowIndex = 0;
		#endregion
		#region "Protected Members"
		protected Active.Modules.Forums.Controls.Callback cbTopicActions = new Active.Modules.Forums.Controls.Callback();
			#endregion
		protected Active.Modules.Forums.Controls.Callback cbTopicLoader = new Active.Modules.Forums.Controls.Callback();
		#region "Event Handlers"
		private void TopicDisplay_Init(object sender, System.EventArgs e)
		{
			SettingsInfo MainSettings = DataCache.MainSettings(ControlConfig.InstanceId);
			PageSize = MainSettings.PageSize;
			string sTemp = string.Empty;
			if ((ControlConfig != null)) {
				object obj = DataCache.CacheRetrieve(ControlConfig.InstanceId + CurrentView);
				if (obj == null) {
					sTemp = ParseTemplate();
				} else {
					sTemp = Convert.ToString(obj);
				}
				if (sTemp.Contains("[NOPAGING]")) {
					RowIndex = 0;
					PageSize = int.MaxValue;
					sTemp = sTemp.Replace("[NOPAGING]", string.Empty);
				}
				sTemp = Utilities.ParseTokenConfig(sTemp, "topic", ControlConfig);

				string Subject = string.Empty;
				string Body = string.Empty;
				System.DateTime CreateDate = null;
				System.DateTime EditDate = null;
				string Tags = string.Empty;
				string AuthorRoles = string.Empty;
				string IPAddress = string.Empty;
				System.DateTime LastPostDate = null;
				string LastPostData = string.Empty;
				UserProfileInfo tAuthor = null;
				int ReplyCount = 0;
				Data.Topics tc = new Data.Topics();
				int rowCount = 0;
				int nextTopic = 0;
				int prevTopic = 0;
				int statusId = -1;
				double topicRating = 0;
				IDataReader dr = tc.TopicForDisplay(ControlConfig.SiteId, ControlConfig.InstanceId, -1, TopicId, UserId, RowIndex, PageSize, "ASC");
				while (dr.Read()) {
					ForumId = int.Parse(dr["ForumId"].ToString());
					Subject = dr["Subject"].ToString();
					Body = dr["Body"].ToString();
					CreateDate = Convert.ToDateTime(dr["DateCreated"].ToString());
					EditDate = Convert.ToDateTime(dr["DateUpdated"].ToString());
					Tags = dr["Tags"].ToString();
					IPAddress = dr["IPAddress"].ToString();
					LastPostDate = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["LastPostDate"].ToString()), ControlConfig.InstanceId));
					LastPostData = dr["LastPostData"].ToString();
					ReplyCount = int.Parse(dr["ReplyCount"].ToString());
					nextTopic = int.Parse(dr["NextTopic"].ToString());
					prevTopic = int.Parse(dr["PrevTopic"].ToString());
					topicRating = double.Parse(dr["TopicRating"].ToString());
					UserProfileInfo profile = new UserProfileInfo();
					var _with1 = profile;
					_with1.UserID = int.Parse(dr["AuthorId"].ToString());
					if (string.IsNullOrEmpty(dr["DisplayName"].ToString())) {
						_with1.DisplayName = dr["AuthorName"].ToString();
					} else {
						_with1.DisplayName = dr["DisplayName"].ToString();
					}
					_with1.FirstName = dr["FirstName"].ToString();
					_with1.LastName = dr["LastName"].ToString();
					_with1.Username = dr["Username"].ToString();
					_with1.UserCaption = dr["UserCaption"].ToString();
					_with1.AnswerCount = int.Parse(dr["AnswerCount"].ToString());
					_with1.AOL = dr["AOL"].ToString();
					_with1.Avatar = dr["Avatar"].ToString();
					_with1.AvatarType = (AvatarTypes)int.Parse(dr["AvatarType"].ToString());
					_with1.DateCreated = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["UserDateCreated"].ToString()), ControlConfig.InstanceId));
					_with1.DateLastActivity = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["DateLastActivity"].ToString()), ControlConfig.InstanceId));
					_with1.DateLastPost = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["DateLastPost"].ToString()), ControlConfig.InstanceId));
					if (!string.IsNullOrEmpty(dr["UserDateUpdated"].ToString())) {
						_with1.DateUpdated = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["UserDateUpdated"].ToString()), ControlConfig.InstanceId));
					}
					_with1.Interests = dr["Interests"].ToString();
					_with1.IsUserOnline = bool.Parse(dr["IsUserOnline"].ToString());
					_with1.Location = dr["Location"].ToString();
					_with1.MSN = dr["MSN"].ToString();
					_with1.Occupation = dr["Occupation"].ToString();
					_with1.TopicCount = int.Parse(dr["UserTopicCount"].ToString());
					_with1.ReplyCount = int.Parse(dr["UserReplyCount"].ToString());
					_with1.RewardPoints = int.Parse(dr["RewardPoints"].ToString());
					_with1.Roles = dr["UserRoles"].ToString();
					_with1.Signature = dr["Signature"].ToString();
					_with1.TrustLevel = int.Parse(dr["TrustLevel"].ToString());
					_with1.WebSite = dr["WebSite"].ToString();
					_with1.Yahoo = dr["Yahoo"].ToString();
					tAuthor = profile;

					if (DataPageId == 1) {
						sTemp = ParseTopic(sTemp, Subject, CreateDate, Body, Tags, Convert.ToString(EditDate), IPAddress, ForumUser, rowCount);
						rowCount += 1;
					} else {
						sTemp = TemplateUtils.ReplaceSubSection(sTemp, string.Empty, "[TOPIC]", "[/TOPIC]");
					}
				}
				if (ForumInfo == null) {
					ForumController fc = new ForumController();
					Forum fi = null;
					fi = fc.Forums_Get(ForumId, UserId, true, true);
					ForumInfo = fi;
				}
				sTemp = sTemp.Replace("[FORUMID]", ForumId.ToString);
				sTemp = sTemp.Replace("[FORUMNAME]", ForumInfo.ForumName);
				sTemp = sTemp.Replace("[TOPICID]", TopicId.ToString);
				sTemp = sTemp.Replace("[CREATEROLES]", ForumInfo.Security.Create);
				sTemp = sTemp.Replace("[USERROLES]", ForumUser.UserRoles);
				sTemp = sTemp.Replace("[THEMEPATH]", ThemePath);
				sTemp = sTemp.Replace("[SUBJECT]", Subject);
				sTemp = sTemp.Replace("[PAGEID]", PageId.ToString);
				sTemp = sTemp.Replace("[REPLYROLES]", ForumInfo.Security.Reply);
				sTemp = sTemp.Replace("AF:SECURITY:MODROLES]", "AF:SECURITY:MODROLES:" + ForumInfo.Security.ModApprove + "]");
				sTemp = sTemp.Replace("AF:SECURITY:MODAPPROVE]", "AF:SECURITY:MODAPPROVE:" + ForumInfo.Security.ModApprove + "]");
				sTemp = sTemp.Replace("AF:SECURITY:DELETE]", "AF:SECURITY:DELETE:" + ForumInfo.Security.Delete + ForumInfo.Security.ModDelete + "]");
				sTemp = sTemp.Replace("AF:SECURITY:EDIT]", "AF:SECURITY:EDIT:" + ForumInfo.Security.Edit + ForumInfo.Security.ModEdit + "]");
				sTemp = sTemp.Replace("AF:SECURITY:LOCK]", "AF:SECURITY:LOCK:" + ForumInfo.Security.Lock + ForumInfo.Security.ModLock + "]");
				sTemp = sTemp.Replace("AF:SECURITY:MOVE]", "AF:SECURITY:MOVE:" + ForumInfo.Security.ModMove + "]");
				sTemp = sTemp.Replace("AF:SECURITY:PIN]", "AF:SECURITY:PIN:" + ForumInfo.Security.Pin + ForumInfo.Security.ModPin + "]");
				sTemp = sTemp.Replace("AF:SECURITY:SPLIT]", "AF:SECURITY:SPLIT:" + ForumInfo.Security.ModSplit + "]");
				sTemp = sTemp.Replace("AF:SECURITY:REPLY]", "AF:SECURITY:REPLY:" + ForumInfo.Security.Reply + "]");
				if (LastPostDate == null) {
					LastPostDate = CreateDate;
				}
				string LastPostAuthor = string.Empty;
				if (((bRead & tAuthor.UserID == this.UserId)) & statusId >= 0) {
					sTemp = sTemp.Replace("[AF:CONTROL:STATUS]", "<asp:placeholder id=\"plhStatus\" runat=\"server\" />");
					sTemp = sTemp.Replace("[AF:CONTROL:STATUSICON]", "<img alt=\"[RESX:PostStatus" + statusId.ToString() + "]\" src=\"" + ThemePath + "status" + statusId.ToString() + ".png\" />");
				} else if (statusId >= 0) {
					sTemp = sTemp.Replace("[AF:CONTROL:STATUS]", string.Empty);
					sTemp = sTemp.Replace("[AF:CONTROL:STATUSICON]", "<img alt=\"[RESX:PostStatus" + statusId.ToString() + "]\" src=\"" + ThemePath + "status" + statusId.ToString() + ".png\" />");
				} else {
					sTemp = sTemp.Replace("[AF:CONTROL:STATUS]", string.Empty);
					sTemp = sTemp.Replace("[AF:CONTROL:STATUSICON]", string.Empty);
					sTemp = sTemp.Replace("[ACTIONS:ANSWER]", string.Empty);
				}
				if (string.IsNullOrEmpty(LastPostData)) {
					LastPostAuthor = UserProfiles.GetDisplayName(ControlConfig.InstanceId, MainSettings.ProfileVisibility, false, tAuthor.UserID, MainSettings.UserNameDisplay, MainSettings.DisableUserProfiles, tAuthor.Username, tAuthor.FirstName, tAuthor.LastName, tAuthor.DisplayName);
				} else {
					Author la = new Author();
					System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
					xDoc.LoadXml(LastPostData);
					System.Xml.XmlNode xNode = xDoc.SelectSingleNode("//root/lastpost");
					if ((xNode != null)) {
						var _with2 = la;
						_with2.AuthorId = int.Parse(xNode["authorid"].InnerText.ToString());
						_with2.DisplayName = xNode["displayname"].InnerText;
						_with2.FirstName = xNode["firstname"].InnerText;
						_with2.LastName = xNode["lastname"].InnerText;
						_with2.Username = xNode["username"].InnerText;
					}
					LastPostAuthor = UserProfiles.GetDisplayName(ControlConfig.InstanceId, MainSettings.ProfileVisibility, false, la.AuthorId, MainSettings.UserNameDisplay, MainSettings.DisableUserProfiles, la.Username, la.FirstName, la.LastName, la.DisplayName);
				}
				//TODO:Fix LastPostDate Format
				sTemp = sTemp.Replace("[AF:LABEL:LastPostDate]", LastPostDate.ToString());
				sTemp = sTemp.Replace("[AF:LABEL:LastPostAuthor]", LastPostAuthor);
				sTemp = sTemp.Replace("[AF:LABEL:ReplyCount]", ReplyCount.ToString());
				string sURL = "<a rel=\"nofollow\" href=\"" + Utilities.NavigateUrl(PageId, "", ParamKeys.ForumId + "=" + ForumId, ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.TopicId + "=" + TopicId, "mid=" + ControlConfig.InstanceId.ToString, "dnnprintmode=true") + "?skinsrc=" + HttpUtility.UrlEncode("[G]" + DotNetNuke.UI.Skins.SkinInfo.RootSkin + "/" + DotNetNuke.Common.glbHostSkinFolder + "/" + "No Skin") + "&amp;containersrc=" + HttpUtility.UrlEncode("[G]" + DotNetNuke.UI.Skins.SkinInfo.RootContainer + "/" + DotNetNuke.Common.glbHostSkinFolder + "/" + "No Container") + "\" target=\"_blank\">";
				sURL += "<img src=\"" + ThemePath + "images/spacer.gif\" alt=\"[RESX:PrinterFriendly]\" class=\"aficon aficonprint\" /></a>";
				sTemp = sTemp.Replace("[AF:CONTROL:PRINTER]", sURL);
				if (HttpContext.Current.Request.IsAuthenticated) {
					sURL = Utilities.NavigateUrl(PageId, "", new string[] {
						ParamKeys.ViewType + "=sendto",
						ParamKeys.ForumId + "=" + ForumId,
						ParamKeys.TopicId + "=" + TopicId
					});
					sTemp = sTemp.Replace("[AF:CONTROL:EMAIL]", "<a href=\"" + sURL + "\" rel=\"nofollow\"><img src=\"" + ThemePath + "images/spacer.gif\" class=\"aficon aficonemail\" alt=\"[RESX:EmailThis]\" /></a>");
				} else {
					sTemp = sTemp.Replace("[AF:CONTROL:EMAIL]", string.Empty);
				}
				if (ForumInfo.AllowRSS) {
					string Url = null;
					Url = DotNetNuke.Common.AddHTTP(DotNetNuke.Common.GetDomainName(HttpContext.Current.Request)) + "/DesktopModules/ActiveForums/feeds.aspx?portalid=" + ControlConfig.SiteId + "&forumid=" + ForumId + "&tabid=" + PageId + "&moduleid=" + ControlConfig.InstanceId;
					sTemp = sTemp.Replace("[RSSLINK]", "<a href=\"" + Url + "\"><img src=\"" + ThemePath + "images/rss.png\" runat=server border=\"0\" alt=\"[RESX:RSS]\" /></a>");
				} else {
					sTemp = sTemp.Replace("[RSSLINK]", string.Empty);
				}
				if (nextTopic == 0) {
					sTemp = sTemp.Replace("[NEXTTOPICID]", string.Empty);
					sTemp = sTemp.Replace("[HASNEXTTOPIC]", "False");
				} else {
					sTemp = sTemp.Replace("[NEXTTOPICID]", nextTopic.ToString());
					sTemp = sTemp.Replace("[HASNEXTTOPIC]", "True");
				}
				if (prevTopic == 0) {
					sTemp = sTemp.Replace("[PREVTOPICID]", string.Empty);
					sTemp = sTemp.Replace("[HASPREVTOPIC]", "False");
				} else {
					sTemp = sTemp.Replace("[PREVTOPICID]", prevTopic.ToString());
					sTemp = sTemp.Replace("[HASPREVTOPIC]", "True");
				}
				dr.NextResult();
				//Process Replies
				StringBuilder sb = new StringBuilder();
				sb.Append(string.Empty);
				int replyId = 0;
				while (dr.Read()) {
					Subject = dr["Subject"].ToString();
					Body = dr["Body"].ToString();
					CreateDate = Convert.ToDateTime(dr["DateCreated"].ToString());
					EditDate = Convert.ToDateTime(dr["DateUpdated"].ToString());
					IPAddress = dr["IPAddress"].ToString();
					replyId = int.Parse(dr["ReplyId"].ToString());
					UserProfileInfo profile = new UserProfileInfo();
					var _with3 = profile;
					_with3.UserID = int.Parse(dr["AuthorId"].ToString());
					if (string.IsNullOrEmpty(dr["DisplayName"].ToString())) {
						_with3.DisplayName = dr["AuthorName"].ToString();
					} else {
						_with3.DisplayName = dr["DisplayName"].ToString();
					}
					_with3.FirstName = dr["FirstName"].ToString();
					_with3.LastName = dr["LastName"].ToString();
					_with3.Username = dr["Username"].ToString();
					_with3.UserCaption = dr["UserCaption"].ToString();
					_with3.AnswerCount = int.Parse(dr["AnswerCount"].ToString());
					_with3.AOL = dr["AOL"].ToString();
					_with3.Avatar = dr["Avatar"].ToString();
					_with3.AvatarType = (AvatarTypes)int.Parse(dr["AvatarType"].ToString());
					_with3.DateCreated = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["UserDateCreated"].ToString()), ControlConfig.InstanceId));
					_with3.DateLastActivity = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["DateLastActivity"].ToString()), ControlConfig.InstanceId));
					if (!string.IsNullOrEmpty(dr["DateLastPost"].ToString())) {
						_with3.DateLastPost = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["DateLastPost"].ToString()), ControlConfig.InstanceId));
					}

					if (!string.IsNullOrEmpty(dr["UserDateUpdated"].ToString())) {
						_with3.DateUpdated = System.DateTime.Parse(Utilities.GetDate(System.DateTime.Parse(dr["UserDateUpdated"].ToString()), ControlConfig.InstanceId));
					}
					_with3.Interests = dr["Interests"].ToString();
					_with3.IsUserOnline = bool.Parse(dr["IsUserOnline"].ToString());
					_with3.Location = dr["Location"].ToString();
					_with3.MSN = dr["MSN"].ToString();
					_with3.Occupation = dr["Occupation"].ToString();
					_with3.TopicCount = int.Parse(dr["UserTopicCount"].ToString());
					_with3.ReplyCount = int.Parse(dr["UserReplyCount"].ToString());
					_with3.RewardPoints = int.Parse(dr["RewardPoints"].ToString());
					_with3.Roles = dr["UserRoles"].ToString();
					_with3.Signature = dr["Signature"].ToString();
					_with3.TrustLevel = int.Parse(dr["TrustLevel"].ToString());
					_with3.WebSite = dr["WebSite"].ToString();
					_with3.Yahoo = dr["Yahoo"].ToString();
					sb.Append(ParseReply(sTemp, replyId, Subject, CreateDate, Body, Convert.ToString(EditDate), IPAddress, ForumUser, rowCount));
					rowCount += 1;
				}
				dr.Close();
				sTemp = TemplateUtils.ReplaceSubSection(sTemp, sb.ToString(), "[REPLIES]", "[/REPLIES]");
				sTemp = Utilities.LocalizeControl(sTemp);
				sTemp = sTemp.Replace("[TOPICID]", TopicId.ToString);
				sTemp = sTemp.Replace("[GROUPNAME]", ForumInfo.GroupName);
				sTemp = sTemp.Replace("[FORUMNAME]", ForumInfo.ForumName);
				sTemp = sTemp.Replace("[TOPICRATING]", topicRating.ToString());
				sTemp = sTemp.Replace("[CURRENTUSERID]", UserId.ToString);

				sTemp = Utilities.ParseSecurityTokens(sTemp, ForumUser.UserRoles);
				if (!sTemp.Contains(Globals.ControlRegisterAFTag)) {
					sTemp = Globals.ControlRegisterAFTag + sTemp;
				}
				Control ctl = Page.ParseControl(sTemp);
				LinkControls(ctl.Controls);
				this.Controls.Add(ctl);
			}
		}
		#endregion
		#region "Private Methods"
		private string ParseTemplate()
		{
			string sOut = DisplayTemplate;
			//sOut = ParseForumData(sOut)
			sOut = ParseTopicControls(sOut);
			sOut = sOut.Replace("[AF:CONTROL:CALLBACK]", string.Empty);
			sOut = sOut.Replace("[/AF:CONTROL:CALLBACK]", string.Empty);

			DataCache.CacheStore(ControlConfig.InstanceId + CurrentView, sOut);
			return sOut;
		}
		private string ParseTopic(string template, string Subject, System.DateTime PostDate, string Body, string Tags, string EditDate, string IPAddress, User profile, int RowCount)
		{

			string t = TemplateUtils.GetTemplateSection(template, "[TOPIC]", "[/TOPIC]");
			t = Utilities.ParseTokenConfig(t, "topic", ControlConfig);
			t = t.Replace("[SUBJECT]", Subject);
			t = t.Replace("[BODY]", Body);
			t = t.Replace("[POSTDATE]", Utilities.GetDate(PostDate, ControlConfig.InstanceId));
			t = t.Replace("[DATECREATED]", Utilities.GetDate(PostDate, ControlConfig.InstanceId));
			t = t.Replace("[POSTID]", TopicId.ToString);
			t = t.Replace("[FORUMID]", ForumId.ToString);

			if (PostDate == Convert.ToDateTime(EditDate) | Convert.ToDateTime(EditDate) == System.DateTime.MinValue | Convert.ToDateTime(EditDate) == Utilities.NullDate) {
				t = t.Replace("[MODEDITDATE]", string.Empty);
			} else {
				t = t.Replace("[MODEDITDATE]", Utilities.GetDate(Convert.ToDateTime(EditDate), ControlConfig.InstanceId));
			}
			if (string.IsNullOrEmpty(Tags)) {
				t = TemplateUtils.ReplaceSubSection(t, string.Empty, "[AF:CONTROL:TAGS]", "[/AF:CONTROL:TAGS]");
			} else {
				t = t.Replace("[AF:LABEL:TAGS]", Tags);
			}
			t = TemplateUtils.ParseProfileTemplate(t, ForumUser, ControlConfig.SiteId, ControlConfig.InstanceId, ThemePath, CurrentUserTypes.Auth, true, ControlConfig.User.PrefBlockAvatars, ControlConfig.User.PrefBlockSignatures, IPAddress);
			t = t.Replace("[ATTACHMENTS]", string.Empty);
			if (RowCount % 2 == 0) {
				t = t.Replace("[POSTINFOCSS]", "afpostinfo1");
				t = t.Replace("[POSTTOPICCSS]", "afpostreply1");
				t = t.Replace("[POSTREPLYCSS]", "afpostreply1");
			} else {
				t = t.Replace("[POSTTOPICCSS]", "afpostreply2");
				t = t.Replace("[POSTINFOCSS]", "afpostinfo2");
				t = t.Replace("[POSTREPLYCSS]", "afpostreply2");
			}
			template = TemplateUtils.ReplaceSubSection(template, t, "[TOPIC]", "[/TOPIC]");
			return template;
		}
		private string ParseReply(string template, int PostId, string Subject, System.DateTime PostDate, string Body, string EditDate, string IPAddress, User profile, int RowCount)
		{
			string t = TemplateUtils.GetTemplateSection(template, "[REPLIES]", "[/REPLIES]");
			t = Utilities.ParseTokenConfig(t, "topic", ControlConfig);
			t = t.Replace("[SUBJECT]", Subject);
			t = t.Replace("[BODY]", Body);
			t = t.Replace("[POSTDATE]", Utilities.GetDate(PostDate, ControlConfig.InstanceId));
			t = t.Replace("[DATECREATED]", Utilities.GetDate(PostDate, ControlConfig.InstanceId));
			t = t.Replace("[POSTID]", PostId.ToString());
			t = t.Replace("[FORUMID]", ForumId.ToString);
			t = t.Replace("[TOPICID]", TopicId.ToString);

			if (PostDate == Convert.ToDateTime(EditDate) | Convert.ToDateTime(EditDate) == System.DateTime.MinValue | Convert.ToDateTime(EditDate) == Utilities.NullDate) {
				t = t.Replace("[MODEDITDATE]", string.Empty);
			} else {
				t = t.Replace("[MODEDITDATE]", Utilities.GetDate(Convert.ToDateTime(EditDate), ControlConfig.InstanceId));
			}
			t = t.Replace("[ATTACHMENTS]", string.Empty);
			t = TemplateUtils.ParseProfileTemplate(t, profile, ControlConfig.SiteId, ControlConfig.InstanceId, ThemePath, CurrentUserTypes.Auth, true, ControlConfig.User.PrefBlockAvatars, ControlConfig.User.PrefBlockSignatures, IPAddress);
			if (RowCount % 2 == 0) {
				t = t.Replace("[POSTINFOCSS]", "afpostinfo1");
				t = t.Replace("[POSTTOPICCSS]", "afpostreply1");
				t = t.Replace("[POSTREPLYCSS]", "afpostreply1");
			} else {
				t = t.Replace("[POSTTOPICCSS]", "afpostreply2");
				t = t.Replace("[POSTINFOCSS]", "afpostinfo2");
				t = t.Replace("[POSTREPLYCSS]", "afpostreply2");
			}
			return t;
		}
		private string ParseTopicControls(string template)
		{
			template = template.Replace("[AF:CONTROL:ADDTHIS]", "<af:addthis runat=\"server\" AddThisId=\"" + MainSettings.AddThisAccount + "\" Title=\"[SUBJECT]\" />");

			template = template.Replace("[NOPAGING]", "[NOPAGING]<script type=\"text/javascript\">afpagesize=" + int.MaxValue + ";</script>");
			template = ParseBanner(template);
			if (bLocked) {
				template = template.Replace("[ADDREPLY]", "<span class=\"afnormal\">[RESX:TopicLocked]</span>");
				template = template.Replace("[QUICKREPLY]", string.Empty);
			}
			//template = template.Replace("[ADDREPLY]", "<af:imagebutton runat='server' imagelocation='LEFT' cssclass='ambutton' PostBack='false' PageId='[PAGEID]' Params='[PARAMKEYS:VIEWTYPE]=topicreply,[PARAMKEYS:FORUMID]=[FORUMID],[PARAMKEYS:TOPICID]=[TOPICID]' text='[RESX:AddReply]' imageurl='[THEMEPATH]add16.png' NotAuthText='[RESX:NotAuthorizedReply]' AuthRoles='[REPLYROLES]' NotAuthCSS='amnormal' UserRoles='[USERROLES]' />")
			//template = template.Replace("[QUICKREPLY]", "<af:quickreply runat='server' Subject=""[SUBJECT]"" templatefile='quickreply.ascx' AuthRoles='[REPLYROLES]' UserRoles='[USERROLES]' />")

			return template;
		}
		private void LinkControls(ControlCollection ctrls)
		{
			foreach (Control ctrl in ctrls) {
				if (ctrl is Controls.ControlsBase) {
					((Controls.ControlsBase)ctrl).ControlConfig = this.ControlConfig;
					((Controls.ControlsBase)ctrl).ForumData = this.ForumData;
					((Controls.ControlsBase)ctrl).ForumId = this.ForumId;
				}
				if (ctrl.Controls.Count > 0) {
					LinkControls(ctrl.Controls);
				}
			}
		}
		private string ParseBanner(string template)
		{
			#if SKU = "PROFESSIONAL" Or SKU = "ENTERPRISE"
			if (template.Contains("[BANNER")) {
				int bannerCount = 1;
				template = "<%@ Register TagPrefix=\"dnn\" TagName=\"BANNER\" Src=\"~/Admin/Skins/Banner.ascx\" %>" + template;
				template = template.Replace("[BANNER]", "<dnn:BANNER runat=\"server\" id=\"dnnBANNER" + bannerCount + "\" BannerTypeId=\"-1\" GroupName=\"FORUMS\" EnableViewState=\"False\" />");
				string pattern = "(\\[BANNER:(.+?)\\])";
				Regex regExp = new Regex(pattern);
				MatchCollection matches = null;
				matches = regExp.Matches(template);
				string sBanner = "<dnn:BANNER runat=\"server\" id=\"dnnBANNER{0}\" BannerTypeId=\"-1\" GroupName=\"{1}\" EnableViewState=\"False\" />";
				foreach (Match match in matches) {
					string sReplace = null;
					bannerCount += 1;
					sReplace = string.Format(sBanner, bannerCount, match.Groups[2].Value);
					template = regExp.Replace(template, sReplace, 1);
				}
			}
			#elif
			template = template.Replace("[BANNER]", string.Empty);
			template = template.Replace("<%@ Register TagPrefix=\"dnn\" TagName=\"BANNER\" Src=\"~/Admin/Skins/Banner.ascx\" %>", string.Empty);
			#endif
			return template;
		}
		public TopicDisplay()
		{
			Init += TopicDisplay_Init;
		}
		#endregion

	}

}

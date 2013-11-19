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
using System.Xml;

namespace Active.Modules.Forums.Controls
{
	[ToolboxData("<{0}:TopicsDisplay runat=server></{0}:TopicsDisplay>")]
	public class TopicsDisplay : ControlsBase
	{
		#region "Private Members"
		private string MemberListMode;
		private string ProfileVisibility;
		private string UserNameDisplay;
		private bool DisableUserProfiles;
		private bool bRead = true;
		private bool bView = true;
			#endregion
		XmlDocument topicsData;
		#region "Event Handlers"
		private void TopicsDisplay_Init(object sender, System.EventArgs e)
		{
			SettingsInfo MainSettings = DataCache.MainSettings(ControlConfig.InstanceId);
			MemberListMode = MainSettings.MemberListMode;
			ProfileVisibility = MainSettings.ProfileVisibility;
			UserNameDisplay = MainSettings.UserNameDisplay;
			DisableUserProfiles = MainSettings.DisableUserProfiles;
			if (topicsData == null) {
				Data.Topics db = new Data.Topics();
				topicsData = db.TopicsForDisplayXML(ControlConfig.SiteId, ControlConfig.InstanceId, -1, ForumUser.UserId, PageId, MainSettings.PageSize, ForumUser.IsSuperUser, "ReplyCreated", UserForumsList);

			}
			string sTemp = string.Empty;
			//pt = New Forums.Utils.TimeCalcItem("ForumDisplay")

			//TODO: Turn cache back on
			//Dim obj As Object = DataCache.CacheRetrieve(InstanceId & CurrentView)
			//If obj Is Nothing Then
			DisplayTemplate = Utilities.ParseTokenConfig(DisplayTemplate, "topic", ControlConfig);
			sTemp = ParseTemplate();
			//Else
			//   sTemp = CType(obj, String)
			//End If
			sTemp = Utilities.LocalizeControl(sTemp);
			//Security
			if (!sTemp.Contains(Globals.ControlRegisterAFTag)) {
				sTemp = Globals.ControlRegisterAFTag + sTemp;
			}
			sTemp = sTemp.Replace("[CREATEROLES]", "1;");
			sTemp = sTemp.Replace("[USERROLES]", ForumUser.UserRoles);
			sTemp = sTemp.Replace("[THEMEPATH]", ThemePath);
			sTemp = Utilities.ParseSecurityTokens(sTemp, ForumUser.UserRoles);
			Control ctl = Page.ParseControl(sTemp);
			LinkControls(ctl.Controls);
			this.Controls.Add(ctl);


		}
		#endregion
		#region "Private Methods"
		private string ParseTemplate()
		{
			SettingsInfo MainSettings = DataCache.MainSettings(ControlConfig.InstanceId);
			string sOut = DisplayTemplate;
			//sOut = ParseForumData(sOut)
			string sort = SortColumns.ReplyCreated;
			if (sOut.Contains("[AF:SORT:TOPICCREATED]")) {
				sort = SortColumns.TopicCreated;
				sOut = sOut.Replace("[AF:SORT:TOPICCREATED]", string.Empty);
			}
			int Replies = 0;
			int Views = 0;
			string Subject = string.Empty;
			string summary = string.Empty;
			string body = string.Empty;
			int replyCount = 0;
			int viewCount = 0;
			int topicid = 0;
			string topicIcon = string.Empty;
			int UserLastTopicRead = 0;
			int UserLastReplyRead = 0;
			int topicRating = 0;
			int LastReplyId = 0;
			bool isLocked = false;
			bool isPinned = false;
			int AuthorId = -1;
			string AuthorName = string.Empty;
			string AuthorFirstName = string.Empty;
			string AuthorLastName = string.Empty;
			string AuthorUserName = string.Empty;
			string AuthorDisplayName = string.Empty;
			string LastPostData = string.Empty;
			System.DateTime LastPostDate = default(System.DateTime);
			System.DateTime DateCreated = default(System.DateTime);
			string sTopicURL = string.Empty;
			string sBodyTitle = string.Empty;
			int statusId = -1;
			bool isAnnounce = false;
			System.DateTime AnnounceStart = default(System.DateTime);
			System.DateTime AnnounceEnd = default(System.DateTime);
			int topicType = 0;
			Author la = null;
			string ModApprove = string.Empty;
			string ModDelete = string.Empty;
			string ModEdit = string.Empty;
			string ModPin = string.Empty;
			string ModLock = string.Empty;
			string ModMove = string.Empty;
			string ModSplit = string.Empty;
			string Reply = string.Empty;
			string Lock = string.Empty;
			string Pin = string.Empty;
			string Delete = string.Empty;
			string Edit = string.Empty;
			string groupName = string.Empty;
			//Dim t As New Data.Topics
			string topics = TemplateUtils.GetTemplateSection(sOut, "[TOPICS]", "[/TOPICS]");
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Xml.XmlNodeList xTopics = null;

			xTopics = topicsData.SelectNodes("//topics/topic");
			int rowCount = 0;
			foreach (System.Xml.XmlNode xNode in xTopics) {
				string ForumName = string.Empty;

				ForumId = int.Parse(xNode.Attributes["forumid"].Value.ToString());
				ForumName = xNode["forumname"].InnerText;
				groupName = xNode["groupname"].InnerText;

				topicid = int.Parse(xNode.Attributes["topicid"].Value.ToString());
				LastReplyId = int.Parse(xNode.Attributes["lastreplyid"].Value.ToString());
				viewCount = int.Parse(xNode.Attributes["viewcount"].Value.ToString());
				replyCount = int.Parse(xNode.Attributes["replycount"].Value.ToString());
				isLocked = bool.Parse(xNode.Attributes["islocked"].Value.ToString());
				isPinned = bool.Parse(xNode.Attributes["ispinned"].Value.ToString());
				topicIcon = xNode.Attributes["topicicon"].Value.ToString();
				statusId = int.Parse(xNode.Attributes["statusid"].Value.ToString());
				isAnnounce = bool.Parse(xNode.Attributes["isannounce"].Value.ToString());
				AnnounceStart = Convert.ToDateTime(xNode.Attributes["announcestart"].Value.ToString());
				AnnounceEnd = Convert.ToDateTime(xNode.Attributes["announceend"].Value.ToString());
				topicType = int.Parse(xNode.Attributes["topictype"].Value.ToString());
				AuthorId = int.Parse(xNode.Attributes["authorid"].Value.ToString());
				DateCreated = Convert.ToDateTime(xNode.Attributes["datecreated"].Value.ToString());
				if (!string.IsNullOrEmpty(xNode.Attributes["lastpostdate"].Value.ToString())) {
					LastPostDate = Convert.ToDateTime(xNode.Attributes["lastpostdate"].Value.ToString());
				} else {
					LastPostDate = DateCreated;
				}
				UserLastReplyRead = int.Parse(xNode.Attributes["userlastreplyread"].Value.ToString());
				UserLastTopicRead = int.Parse(xNode.Attributes["userlasttopicread"].Value.ToString());
				topicRating = int.Parse(xNode.Attributes["topicrating"].Value.ToString());
				Subject = xNode["subject"].InnerText;
				Subject = Utilities.StripHTMLTag(Subject);
				Subject = Subject.Replace("&#91;", "[");
				Subject = Subject.Replace("&#93;", "]");
				summary = xNode["summary"].InnerText;
				body = xNode["body"].InnerText;
				AuthorName = xNode["authorname"].InnerText;
				AuthorUserName = xNode["username"].InnerText;
				AuthorFirstName = xNode["firstname"].InnerText;
				AuthorLastName = xNode["lastname"].InnerText;
				AuthorDisplayName = xNode["displayname"].InnerText;
				XmlNode secNode = xNode["security"];
				ModApprove = secNode["modapprove"].InnerText;
				ModDelete = secNode["moddelete"].InnerText;
				ModEdit = secNode["modedit"].InnerText;
				ModPin = secNode["modpin"].InnerText;
				ModLock = secNode["modlock"].InnerText;
				ModMove = secNode["modmove"].InnerText;
				Reply = secNode["reply"].InnerText;
				Lock = secNode["lock"].InnerText;
				Pin = secNode["pin"].InnerText;
				Delete = secNode["delete"].InnerText;
				Edit = secNode["edit"].InnerText;
				if ((xNode["lastpost"] != null)) {
					LastPostData = xNode["lastpost"].OuterXml;
				} else {
					LastPostData = string.Empty;
				}

				if (!string.IsNullOrEmpty(LastPostData)) {
					la = new Author();
					System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
					xDoc.LoadXml(LastPostData);
					System.Xml.XmlNode xLast = xDoc.SelectSingleNode("//lastpost");
					if ((xLast != null)) {
						var _with1 = la;
						_with1.AuthorId = int.Parse(xLast["authorid"].InnerText.ToString());
						_with1.DisplayName = xLast["displayname"].InnerText;
						_with1.FirstName = xLast["firstname"].InnerText;
						_with1.LastName = xLast["lastname"].InnerText;
						_with1.Username = xLast["username"].InnerText;
					}
				}
				int BodyLength = -1;
				string BodyTrim = "";
				sTopicURL = URL.ForTopic(TabId, PortalId, ForumId, topicid, groupName, ForumName, Subject, 1);
				string tmp = topics;
				string sLastPost = TemplateUtils.GetTemplateSection(tmp, "[LASTPOST]", "[/LASTPOST]");
				if ((la != null)) {
					sLastPost = sLastPost.Replace("[LASTPOSTDISPLAYNAME]", UserProfiles.GetDisplayName(ModuleId, ProfileVisibility, false, la.AuthorId, UserNameDisplay, DisableUserProfiles, la));
					sLastPost = sLastPost.Replace("[AF:PROFILE:AVATAR]", UserProfiles.GetAvatar(la.AuthorId, PortalId, ThemePath, MainSettings.Theme, string.Empty, 0, MainSettings.AvatarWidth, MainSettings.ProfileType, MainSettings.AvatarDefault));

					sLastPost = sLastPost.Replace("[LASTPOSTDATE]", Utilities.GetDate(LastPostDate, ModuleId));
					if (bRead) {
						//TODO: Replace with link
						sLastPost = sLastPost.Replace("[AF:ICONLINK:LASTREPLY]", string.Empty);
					} else {
						sLastPost = sLastPost.Replace("[AF:ICONLINK:LASTREPLY]", string.Empty);
					}
					tmp = TemplateUtils.ReplaceSubSection(tmp, sLastPost, "[LASTPOST]", "[/LASTPOST]");
				} else {
					tmp = TemplateUtils.ReplaceSubSection(tmp, string.Empty, "[LASTPOST]", "[/LASTPOST]");
				}

				//Dim sLastReplyURL As String = NavigateUrl(TabId, "", New String() {ParamKeys.ForumId & "=" & ForumId, ParamKeys.TopicId & "=" & topicid, ParamKeys.ViewType & "=topic", ParamKeys.ContentJumpId & "=" & LastReplyId})
				string sLastReadURL = string.Empty;
				string sUserJumpUrl = string.Empty;
				//If UserLastReplyRead > 0 Then
				//sLastReadURL = NavigateUrl(TabId, "", New String() {ParamKeys.ForumId & "=" & ForumId, ParamKeys.TopicId & "=" & topicid, ParamKeys.ViewType & "=topic", ParamKeys.FirstNewPost & "=" & UserLastReplyRead})
				//End If

				//If UserPrefJumpLastPost And sLastReadURL <> String.Empty Then
				//    '   sTopicURL = sLastReadURL
				//    sUserJumpUrl = sLastReadURL
				//End If
				if (tmp.Contains("[BODY:")) {
					int inStart = Strings.InStr(tmp, "[BODY:") + 5;
					int inEnd = Strings.InStr(inStart, tmp, "]") - 1;
					string sLength = tmp.Substring(inStart, inEnd - inStart);
					BodyLength = Convert.ToInt32(sLength);
					BodyTrim = "[BODY:" + BodyLength.ToString() + "]";
				}
				if (!(BodyTrim == string.Empty)) {
					string BodyPlain = body.Replace("<br>", Constants.vbCrLf);
					BodyPlain = BodyPlain.Replace("<br />", Constants.vbCrLf);
					BodyPlain = Utilities.StripHTMLTag(BodyPlain);
					if (BodyLength > 0 & BodyPlain.Length > BodyLength) {
						BodyPlain = BodyPlain.Substring(0, BodyLength);
					}
					BodyPlain = BodyPlain.Replace(Constants.vbCrLf, "<br />");
					tmp = tmp.Replace(BodyTrim, BodyPlain);
				}
				if (isLocked) {
					tmp = tmp.Replace("[RESX:LockTopic]", "[RESX:UnLockTopic]");
					tmp = tmp.Replace("[RESX:Confirm:Lock]", "[RESX:Confirm:UnLock]");
				}
				if (isPinned) {
					tmp = tmp.Replace("[RESX:PinTopic]", "[RESX:UnPinTopic]");
					tmp = tmp.Replace("[RESX:Confirm:Pin]", "[RESX:Confirm:UnPin]");
				}
				tmp = tmp.Replace("[BODY]", GetBody(body, AuthorId));
				tmp = tmp.Replace("[BODYTITLE]", GetTitle(body, AuthorId));
				tmp = tmp.Replace("[AUTHORID]", AuthorId.ToString());
				tmp = tmp.Replace("[USERID]", UserId.ToString);
				if (string.IsNullOrEmpty(Subject)) {
					Subject = "(no subject)";
				}
				string LastSubject = Subject;
				if (LastReplyId > 0) {
					LastSubject = "[RESX:RE]" + LastSubject;
				}

				tmp = tmp.Replace("[SUBJECT]", Subject);
				string sSubjectLink = GetTopic(topicid, Subject, sBodyTitle, UserId, AuthorId, replyCount, -1, sUserJumpUrl, groupName, ForumName);
				string lastSubjectLink = GetTopic(topicid, Subject, string.Empty, UserId, AuthorId, replyCount, -1, sUserJumpUrl, groupName, ForumName);
				tmp = tmp.Replace("[SUBJECTLINK]", sSubjectLink);
				tmp = tmp.Replace("[THEMEPATH]", ThemePath);
				tmp = tmp.Replace("[TOPICID]", topicid.ToString());
				tmp = tmp.Replace("[MODEDIT]", string.Empty);
				tmp = tmp.Replace("[MODDELETE]", string.Empty);
				tmp = tmp.Replace("[MODMOVE]", string.Empty);
				tmp = tmp.Replace("[MODLOCK]", string.Empty);
				tmp = tmp.Replace("[MODPIN]", string.Empty);
				tmp = tmp.Replace("[REPLIES]", replyCount.ToString());
				tmp = tmp.Replace("[VIEWS]", viewCount.ToString());
				tmp = tmp.Replace("[POSTICONIMAGE]", GetIcon(UserLastTopicRead, UserLastReplyRead, topicid, LastReplyId, topicIcon, isPinned, isLocked));

				tmp = tmp.Replace("[POSTICONCSS]", GetIconCSS(UserLastTopicRead, UserLastReplyRead, topicid, LastReplyId, topicIcon, isPinned, isLocked));
				if (tmp.Contains("[STATUSCSS]")) {
					string sImg = string.Empty;
					if (statusId == -1) {
						tmp = tmp.Replace("[STATUSCSS]", string.Empty);
					} else {
						sImg = "<span title=\"[RESX:PostStatus" + statusId.ToString() + "]\" class=\"afstatus afstatus" + statusId.ToString() + "\"></span>";
					}
					tmp = tmp.Replace("[STATUSCSS]", sImg);
				}
				tmp = tmp.Replace("[STARTEDBY]", UserProfiles.GetDisplayName(ModuleId, ProfileVisibility, false, AuthorId, UserNameDisplay, DisableUserProfiles, AuthorUserName, AuthorFirstName, AuthorLastName, AuthorDisplayName));
				tmp = tmp.Replace("[DATECREATED]", Utilities.GetDate(DateCreated, ModuleId));
				tmp = tmp.Replace("[TOPICURL]", sTopicURL);
				//TODO: Still need to process
				tmp = tmp.Replace("[AF:ICONLINK:LASTREAD]", string.Empty);
				tmp = tmp.Replace("[AF:URL:LASTREAD]", string.Empty);
				if (tmp.Contains("[STATUS]")) {
					string sImg = string.Empty;
					if (statusId == -1) {
						tmp = tmp.Replace("[STATUS]", string.Empty);
					} else {
						sImg = "<img alt=\"[RESX:PostStatus" + statusId.ToString() + "]\" src=\"" + Utilities.AppPath + "themes/images/status" + statusId.ToString() + ".png\" />";
					}
					tmp = tmp.Replace("[STATUS]", sImg);
				}


				//sTopicsTemplate = sTopicsTemplate.Replace("[AF:UI:MINIPAGER]", GetSubPages(TabId, ModuleId, ReplyCount, ForumId, TopicId))
				tmp = tmp.Replace("[AF:UI:MINIPAGER]", string.Empty);
				tmp = tmp.Replace("[POSTRATINGDISPLAY]", string.Empty);
				tmp = tmp.Replace("[TOPICRATING]", topicRating.ToString());
				tmp = tmp.Replace("[ROWCSS]", GetRowCSS(UserLastTopicRead, UserLastReplyRead, topicid, LastReplyId, rowCount));

				tmp = tmp.Replace("[AF:TOPIC:FRIENDLYDATE]", Utilities.HumanFriendlyDate(DateCreated, ModuleId));
				tmp = tmp.Replace("[AF:REPLY:FRIENDLYDATE]", Utilities.HumanFriendlyDate(LastPostDate, ModuleId));
				tmp = tmp.Replace("[AF:TOPIC:FORUMNAME]", ForumName);
				tmp = tmp.Replace("[AF:TOPIC:LASTSUBJECT:LINK]", lastSubjectLink);
				tmp = tmp.Replace("[AF:TOPIC:LASTSUBJECT]", LastSubject);
				tmp = tmp.Replace("[AF:TOPIC:FORUMNAME:LINK]", "<a href=\"" + URL.ForForum(PageId, ForumId, groupName[ForumId], ForumName) + "\" title=\"" + ForumName + "\">" + ForumName + "</a>");
				tmp = tmp.Replace("AF:SECURITY:MODROLES]", "AF:SECURITY:MODROLES:" + ModApprove + "]");
				tmp = tmp.Replace("AF:SECURITY:MODAPPROVE]", "AF:SECURITY:MODAPPROVE:" + ModApprove + "]");
				tmp = tmp.Replace("AF:SECURITY:DELETE]", "AF:SECURITY:DELETE:" + Delete + ModDelete + "]");
				tmp = tmp.Replace("AF:SECURITY:EDIT]", "AF:SECURITY:EDIT:" + Edit + ModEdit + "]");
				tmp = tmp.Replace("AF:SECURITY:LOCK]", "AF:SECURITY:LOCK:" + Lock + ModLock + "]");
				tmp = tmp.Replace("AF:SECURITY:MOVE]", "AF:SECURITY:MOVE:" + ModMove + "]");
				tmp = tmp.Replace("AF:SECURITY:PIN]", "AF:SECURITY:PIN:" + Pin + ModPin + "]");
				tmp = tmp.Replace("AF:SECURITY:SPLIT]", "AF:SECURITY:SPLIT:" + ModSplit + "]");
				tmp = tmp.Replace("AF:SECURITY:REPLY]", "AF:SECURITY:REPLY:" + Reply + "]");

				sb.Append(tmp);
				rowCount += 1;
			}

			sOut = TemplateUtils.ReplaceSubSection(sOut, sb.ToString(), "[TOPICS]", "[/TOPICS]");
			sOut = sOut.Replace("[TOPICS]", string.Empty);
			sOut = sOut.Replace("[/TOPICS]", string.Empty);

			DataCache.CacheStore(ModuleId + "topicsview", sOut);
			return sOut;
		}
		private string GetTitle(string Body, int AuthorId)
		{
			if (bRead | AuthorId == this.UserId) {
				Body = Strings.Replace(Body, "<br>", Constants.vbCrLf);
				Body = Strings.Replace(Body, "[", "&#91");
				Body = Strings.Replace(Body, "]", "&#93");
				Body = Utilities.StripHTMLTag(Body);
				Body = Strings.Left(Body, 500) + "...";
				Body = Strings.Replace(Body, "\"", "'");
				Body = Strings.Replace(Body, "?", string.Empty);
				Body = Strings.Replace(Body, "+", string.Empty);
				Body = Strings.Replace(Body, "%", string.Empty);
				Body = Strings.Replace(Body, "#", string.Empty);
				Body = Strings.Replace(Body, "@", string.Empty);
				return Body.Trim();
			} else {
				return string.Empty;
			}

		}
		private string GetBody(string Body, int AuthorId)
		{
			if (bRead | AuthorId == this.UserId) {
				Body = Strings.Replace(Body, "[", "&#91");
				Body = Strings.Replace(Body, "]", "&#93");
				return Body;
			} else {
				return string.Empty;
			}

		}
		private string GetTopic(int TopicId, string Subject, string BodyTitle, int UserID, int PostUserID, int Replies, int ForumOwnerID, string sLink, string GroupName, string Forumname)
		{
			string sOut = null;
			Subject = Utilities.StripHTMLTag(Subject);
			Subject = Subject.Replace("\"", string.Empty);
			Subject = Subject.Replace("'", string.Empty);
			Subject = Subject.Replace("#", string.Empty);
			Subject = Subject.Replace("%", string.Empty);
			// Subject = Subject.Replace("?", String.Empty)
			Subject = Subject.Replace("+", string.Empty);

			if (bRead) {
				if (sLink == string.Empty) {
					sOut = "<a title=\"" + BodyTitle + "\" href=\"" + URL.ForTopic(TabId, PortalId, ForumId, TopicId, GroupName, Forumname, Subject, 1) + "\">" + Subject + "</a>";
				} else {
					sOut = "<a title=\"" + BodyTitle + "\" href=\"" + sLink + "\">" + Subject + "</a>";
				}

			//sOut = sOut & GetSubPages(TabID, ModuleID, Replies, ForumID, PostID)
			} else {
				sOut = Subject;
			}
			return sOut;
		}
		private void LinkControls(ControlCollection ctrls)
		{
			foreach (Control ctrl in ctrls) {
				if (ctrl is Controls.ControlsBase) {
					((Controls.ControlsBase)ctrl).ControlConfig = this.ControlConfig;
					((Controls.ControlsBase)ctrl).ModuleConfiguration = this.ModuleConfiguration;
				}
				if (ctrl.Controls.Count > 0) {
					LinkControls(ctrl.Controls);
				}
			}
		}

		private string GetIcon(int LastTopicRead, int LastReplyRead, int TopicId, int ReplyId, string Icon, bool Pinned = false, bool Locked = false)
		{
			if (Icon == string.Empty) {
				if (Pinned & Locked) {
					return "topic_pinlocked.png";

				} else if (Pinned) {
					return "topic_pin.png";

				} else if (Locked) {
					return "lock.gif";

				} else {
					if (!HttpContext.Current.Request.IsAuthenticated) {
						return "document.gif";

					} else {
						if (LastTopicRead == 0) {
							return "document_new.gif";

						} else if (LastTopicRead > 0 & LastReplyRead < ReplyId) {
							return "document_new.gif";

						} else {
							return "document.gif";
						}
					}

				}
			} else {
				return Icon;
			}
		}
		private string GetIconCSS(int LastTopicRead, int LastReplyRead, int TopicId, int ReplyId, string Icon, bool Pinned = false, bool Locked = false)
		{
			StringBuilder sb = new StringBuilder();
			if (string.IsNullOrEmpty(Icon)) {
				sb.Append("<div class=\"docicon\">");
			} else {
				sb.Append("<div class=\"docicon\" style=\"background-image:none;\"><img src=\"[EMOTICONPATH]/" + Icon + "\" border=\"0\" />");
			}

			if (Pinned) {
				sb.Append("<div class=\"aficonover dociconpin\"></div>");
			}
			if (Locked) {
				sb.Append("<div class=\"aficonover dociconlock\"></div>");
			}
			if (HttpContext.Current.Request.IsAuthenticated) {
				if (LastTopicRead == 0) {
					sb.Append("<div class=\"aficonover dociconnew\"></div>");
				} else if (LastTopicRead > 0 & LastReplyRead < ReplyId) {
					sb.Append("<div class=\"aficonover dociconnew\"></div>");
					//Else
					//    'Return "document.gif"
					//    Return "<div class=""docicon"">[STATUSCSS]<div class=""aficonover dociconnorm""></div></div>"
				}
			}
			//If Pinned And Locked Then
			//    'Return "topic_pinlocked.png"
			//        sb.Append("<div class=""aficonover dociconpin""></div><div class=""aficonover dociconlock""></div></div>"
			//ElseIf Pinned Then
			//    'Return "topic_pin.png"
			//    Return "<div class=""docicon"">[STATUSCSS]</div>"
			//ElseIf Locked Then
			//    'Return "lock.gif"
			//    Return "<div class=""docicon"">[STATUSCSS]</div>"
			//Else
			//    If Not HttpContext.Current.Request.IsAuthenticated Then
			//        'Return "document.gif"
			//        Return "<div class=""docicon"">[STATUSCSS]<div class=""aficonover dociconnorm""></div></div>"
			//    Else
			//        If LastTopicRead = 0 Then
			//            'Return "document_new.gif"
			//            Return "<div class=""docicon"">[STATUSCSS]<div class=""aficonover dociconnew""></div></div>"
			//        ElseIf LastTopicRead > 0 And LastReplyRead < ReplyId Then
			//            'Return "document_new.gif"
			//            Return "<div class=""docicon"">[STATUSCSS]<div class=""aficonover dociconnew""></div></div>"
			//        Else
			//            'Return "document.gif"
			//            Return "<div class=""docicon"">[STATUSCSS]<div class=""aficonover dociconnorm""></div></div>"
			//        End If
			//    End If

			//End If

			sb.Append("[STATUSCSS]</div>");
			return sb.ToString();
		}
		private string GetRowCSS(int LastTopicRead, int LastReplyRead, int TopicId, int ReplyId, int RowCount)
		{
			bool isRead = false;
			if (LastTopicRead >= TopicId & LastReplyRead >= ReplyId) {
				isRead = true;
			}
			if (!HttpContext.Current.Request.IsAuthenticated) {
				isRead = true;
			}
			if (isRead == true) {
				if (RowCount % 2 == 0) {
					return "aftopicrow";
				} else {
					return "aftopicrowalt";
				}
			} else {
				if (RowCount % 2 == 0) {
					return "aftopicrownew";
				} else {
					return "aftopicrownewalt";
				}
			}
		}
		public TopicsDisplay()
		{
			Init += TopicsDisplay_Init;
		}
		#endregion


	}

}

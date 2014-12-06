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

using System.Web;
using System.Web.Services;
using System.Text;
using System.Xml;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Journal;

namespace DotNetNuke.Modules.ActiveForums.Handlers
{
	public class forumhelper : HandlerBase
	{
		public enum Actions: int
		{
			None,
			UserPing,
			GetUsersOnline,
			TopicSubscribe,
			ForumSubscribe,
			RateTopic,
			DeleteTopic,
			MoveTopic,
			PinTopic,
			LockTopic,
			MarkAnswer,
			TagsAutoComplete,
			DeletePost,
			LoadTopic,
			SaveTopic,
			ForumList,
            LikePost

		}
		public override void ProcessRequest(HttpContext context)
		{
			AdminRequired = false;
			base.AdminRequired = false;
			base.ProcessRequest(context);
			string sOut = "{\"result\":\"success\"}";
			Actions action = Actions.None;
			if (Params != null && Params.Count > 0)
			{
				if (Params["action"] != null && SimulateIsNumeric.IsNumeric(Params["action"]))
				{
					action = (Actions)(Convert.ToInt32(Params["action"].ToString()));
				}
			}
			else if (HttpContext.Current.Request.QueryString["action"] != null && SimulateIsNumeric.IsNumeric(HttpContext.Current.Request.QueryString["action"]))
			{
				if (int.Parse(HttpContext.Current.Request.QueryString["action"]) == 11)
				{
					action = Actions.TagsAutoComplete;
				}
			}
			switch (action)
			{
				case Actions.UserPing:
					sOut = UserOnline();
					break;
				case Actions.GetUsersOnline:
					sOut = GetUserOnlineList();
					break;
				case Actions.TopicSubscribe:
					sOut = SubscribeTopic();
					break;
				case Actions.ForumSubscribe:
					sOut = SubscribeForum();
					break;
				case Actions.RateTopic:
					sOut = RateTopic();
					break;
				case Actions.DeleteTopic:
					sOut = DeleteTopic();
					break;
				case Actions.MoveTopic:
					sOut = MoveTopic();
					break;
				case Actions.PinTopic:
					sOut = PinTopic();
					break;
				case Actions.LockTopic:
					sOut = LockTopic();
					break;
				case Actions.MarkAnswer:
					sOut = MarkAnswer();
					break;
				case Actions.TagsAutoComplete:
					sOut = TagsAutoComplete();
					break;
				case Actions.DeletePost:
					sOut = DeletePost();
					break;
				case Actions.LoadTopic:
					sOut = LoadTopic();
					break;
				case Actions.SaveTopic:
					sOut = SaveTopic();
					break;
				case Actions.ForumList:
					sOut = ForumList();
					break;
                case Actions.LikePost:
                    sOut = LikePost();
                    break;
			}
			context.Response.ContentType = "text/plain";
			context.Response.Write(sOut);
		}

        private string LikePost()
        {
            int userId = 0;
            int contentId = 0;
            if (Params.ContainsKey("userId") && SimulateIsNumeric.IsNumeric(Params["userId"]))
            {
                userId = int.Parse(Params["userId"].ToString());
            }
            if (Params.ContainsKey("contentId") && SimulateIsNumeric.IsNumeric(Params["contentId"]))
            {
                contentId = int.Parse(Params["contentId"].ToString());
            }
            var likeController = new LikesController();
            likeController.Like(contentId, UserId);
            return BuildOutput(userId + "|" + contentId, OutputCodes.Success, true);
        }

		private string ForumList()
		{
			ForumController fc = new ForumController();
			return fc.GetForumsHtmlOption(PortalId, ModuleId, ForumUser);
		}
		private string UserOnline()
		{
			try
			{
				if (UserId > 0)
				{
					DataProvider.Instance().Profiles_UpdateActivity(PortalId, ModuleId, UserId);
					return BuildOutput(UserId.ToString(), OutputCodes.Success, true, false);
				}
				else
				{
					return BuildOutput(UserId.ToString(), OutputCodes.AccessDenied, true, false);
				}

			}
			catch (Exception ex)
			{
				return BuildOutput(ex.Message, OutputCodes.AccessDenied, false, false);
			}
		}
		private string GetUserOnlineList()
		{
			UsersOnline uo = new UsersOnline();
			string sOnlineList = uo.GetUsersOnline(PortalId, ModuleId, ForumUser);
			IDataReader dr = DataProvider.Instance().Profiles_GetStats(PortalId, ModuleId, 2);
			int anonCount = 0;
			int memCount = 0;
			int memTotal = 0;
			while (dr.Read())
			{
				anonCount = Convert.ToInt32(dr["Guests"]);
				memCount = Convert.ToInt32(dr["Members"]);
				memTotal = Convert.ToInt32(dr["MembersTotal"]);
			}
			dr.Close();
			string sUsersOnline = null;
			sUsersOnline = Utilities.GetSharedResource("[RESX:UsersOnline]");
			sUsersOnline = sUsersOnline.Replace("[USERCOUNT]", memCount.ToString());
			sUsersOnline = sUsersOnline.Replace("[TOTALMEMBERCOUNT]", memTotal.ToString());
			return BuildOutput(sUsersOnline + " " + sOnlineList, OutputCodes.Success, true, false);
		}
		private string SubscribeTopic()
		{
			if (UserId <= 0)
			{
				return BuildOutput(string.Empty, OutputCodes.AuthenticationFailed, true);
			}
			int iStatus = 0;
			SubscriptionController sc = new SubscriptionController();
			int forumId = -1;
			int topicId = -1;
			if (Params.ContainsKey("forumid") && SimulateIsNumeric.IsNumeric(Params["forumid"]))
			{
				forumId = int.Parse(Params["forumid"].ToString());
			}
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			iStatus = sc.Subscription_Update(PortalId, ModuleId, forumId, topicId, 1, this.UserId, ForumUser.UserRoles);
			if (iStatus == 1)
			{
				return BuildOutput("{\"subscribed\":true,\"text\":\"" + Utilities.JSON.EscapeJsonString(Utilities.GetSharedResource("[RESX:TopicSubscribe:TRUE]")) + "\"}", OutputCodes.Success, true, true);
			}
			else
			{
				return BuildOutput("{\"subscribed\":false,\"text\":\"" + Utilities.JSON.EscapeJsonString(Utilities.GetSharedResource("[RESX:TopicSubscribe:FALSE]")) + "\"}", OutputCodes.Success, true, true);
			}
		}
		private string SubscribeForum()
		{
			if (UserId <= 0)
			{
				return BuildOutput(string.Empty, OutputCodes.AuthenticationFailed, true);
			}
			int rUserId = -1;

			if (Params.ContainsKey("userid") && SimulateIsNumeric.IsNumeric(Params["userid"]))
			{
				rUserId = int.Parse(Params["userid"].ToString());

			}
			if (rUserId > 0 & rUserId != ForumUser.UserId & ! ForumUser.IsAdmin)
			{
				return BuildOutput(string.Empty, OutputCodes.AuthenticationFailed, true);
			}
			rUserId = UserId;
			int iStatus = 0;
			SubscriptionController sc = new SubscriptionController();
			int forumId = -1;
			if (Params.ContainsKey("forumid") && SimulateIsNumeric.IsNumeric(Params["forumid"]))
			{
				forumId = int.Parse(Params["forumid"].ToString());
			}
			iStatus = sc.Subscription_Update(PortalId, ModuleId, forumId, -1, 1, rUserId, ForumUser.UserRoles);

			if (iStatus == 1)
			{
				return BuildOutput("{\"subscribed\":true,\"text\":\"" + Utilities.JSON.EscapeJsonString(Utilities.GetSharedResource("[RESX:ForumSubscribe:TRUE]")) + "\",\"forumid\":" + forumId.ToString() + "}", OutputCodes.Success, true, true);
			}
			else
			{
				return BuildOutput("{\"subscribed\":false,\"text\":\"" + Utilities.JSON.EscapeJsonString(Utilities.GetSharedResource("[RESX:ForumSubscribe:FALSE]")) + "\",\"forumid\":" + forumId.ToString() + "}", OutputCodes.Success, true, true);
			}


		}
		private string RateTopic()
		{
			int r = 0;
			int topicId = -1;
			if (Params.ContainsKey("rate") && SimulateIsNumeric.IsNumeric(Params["rate"]))
			{
				r = int.Parse(Params["rate"].ToString());
			}
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			if (r >= 1 && r <= 5 && topicId > 0)
			{
				DataProvider.Instance().Topics_AddRating(topicId, UserId, r, string.Empty, HttpContext.Current.Request.UserHostAddress.ToString());
			}
			r = DataProvider.Instance().Topics_GetRating(topicId);
			return BuildOutput(r.ToString(), OutputCodes.Success, true, false);
		}
		private string DeleteTopic()
		{
			int topicId = -1;
			int forumId = -1;
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			if (topicId > 0)
			{
				TopicsController tc = new TopicsController();
				TopicInfo t = tc.Topics_Get(PortalId, ModuleId, topicId);
				Data.ForumsDB db = new Data.ForumsDB();
				forumId = db.Forum_GetByTopicId(topicId);
				ForumController fc = new ForumController();
				Forum f = fc.Forums_Get(forumId, this.UserId, true);
				if (f != null)
				{
					if (Permissions.HasPerm(f.Security.ModDelete, ForumUser.UserRoles) || (t.Author.AuthorId == this.UserId && Permissions.HasAccess(f.Security.Delete, ForumUser.UserRoles)))
					{
						tc.Topics_Delete(PortalId, ModuleId, forumId, topicId, MainSettings.DeleteBehavior);
						return BuildOutput(string.Empty, OutputCodes.Success, true);
					}
				}
			}
			return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
		}
		private string MoveTopic()
		{
			int topicId = -1;
			int forumId = -1;
			int targetForumId = -1;
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			if (Params.ContainsKey("forumid") && SimulateIsNumeric.IsNumeric(Params["forumid"]))
			{
				targetForumId = int.Parse(Params["forumid"].ToString());
			}
			if (topicId > 0)
			{
				TopicsController tc = new TopicsController();
				TopicInfo t = tc.Topics_Get(PortalId, ModuleId, topicId);
				Data.ForumsDB db = new Data.ForumsDB();
				forumId = db.Forum_GetByTopicId(topicId);
				ForumController fc = new ForumController();
				Forum f = fc.Forums_Get(forumId, this.UserId, true);
				if (f != null)
				{
					if (Permissions.HasPerm(f.Security.ModMove, ForumUser.UserRoles))
					{
						tc.Topics_Move(PortalId, ModuleId, targetForumId, topicId);
						DataCache.ClearAllCache(ModuleId, TabId);
						return BuildOutput(string.Empty, OutputCodes.Success, true);
					}
				}

			}
			return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
		}
		private string PinTopic()
		{
			int topicId = -1;
			int forumId = -1;
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			if (topicId > 0)
			{
				TopicsController tc = new TopicsController();
				TopicInfo t = tc.Topics_Get(PortalId, ModuleId, topicId);
				Data.ForumsDB db = new Data.ForumsDB();
				forumId = db.Forum_GetByTopicId(topicId);
				ForumController fc = new ForumController();
				Forum f = fc.Forums_Get(forumId, this.UserId, true);
				if (f != null)
				{
					if (Permissions.HasPerm(f.Security.ModPin, ForumUser.UserRoles) || (t.Author.AuthorId == this.UserId && Permissions.HasAccess(f.Security.Pin, ForumUser.UserRoles)))
					{
						if (t.IsPinned)
						{
							t.IsPinned = false;
						}
						else
						{
							t.IsPinned = true;
						}
						tc.TopicSave(PortalId, t);
						return BuildOutput(string.Empty, OutputCodes.Success, true);
					}
				}
			}
			return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
		}
		private string LockTopic()
		{
			int topicId = -1;
			int forumId = -1;
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			if (topicId > 0)
			{
				TopicsController tc = new TopicsController();
				TopicInfo t = tc.Topics_Get(PortalId, ModuleId, topicId);
				Data.ForumsDB db = new Data.ForumsDB();
				forumId = db.Forum_GetByTopicId(topicId);
				ForumController fc = new ForumController();
				Forum f = fc.Forums_Get(forumId, this.UserId, true);
				if (f != null)
				{
					if (Permissions.HasPerm(f.Security.ModLock, ForumUser.UserRoles) || (t.Author.AuthorId == this.UserId && Permissions.HasAccess(f.Security.Lock, ForumUser.UserRoles)))
					{
						if (t.IsLocked)
						{
							t.IsLocked = false;
						}
						else
						{
							t.IsLocked = true;
						}
						tc.TopicSave(PortalId, t);
						return BuildOutput(string.Empty, OutputCodes.Success, true);
					}
				}
			}
			return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
		}
		private string MarkAnswer()
		{
			int topicId = -1;
			int forumId = -1;
			int replyId = -1;
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			if (Params.ContainsKey("replyid") && SimulateIsNumeric.IsNumeric(Params["replyid"]))
			{
				replyId = int.Parse(Params["replyid"].ToString());
			}
			if (topicId > 0 & UserId > 0)
			{
				TopicsController tc = new TopicsController();
				TopicInfo t = tc.Topics_Get(PortalId, ModuleId, topicId);
				Data.ForumsDB db = new Data.ForumsDB();
				forumId = db.Forum_GetByTopicId(topicId);
				ForumController fc = new ForumController();
				Forum f = fc.Forums_Get(forumId, this.UserId, true);
				if ((this.UserId == t.Author.AuthorId && ! t.IsLocked) || Permissions.HasAccess(f.Security.ModEdit, ForumUser.UserRoles))
				{
					DataProvider.Instance().Reply_UpdateStatus(PortalId, ModuleId, topicId, replyId, UserId, 1, Permissions.HasAccess(f.Security.ModEdit, ForumUser.UserRoles));

				}
				return BuildOutput(string.Empty, OutputCodes.Success, true);
			}
			else
			{
				return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
			}
		}
		private string TagsAutoComplete()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string q = string.Empty;
			if (! (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["q"])))
			{
				q = HttpContext.Current.Request.QueryString["q"].Trim();
				q = Utilities.Text.RemoveHTML(q);
				q = Utilities.Text.CheckSqlString(q);
				if (! (string.IsNullOrEmpty(q)))
				{
					if (q.Length > 20)
					{
						q = q.Substring(0, 20);
					}
				}
			}
			int i = 0;
			if (! (string.IsNullOrEmpty(q)))
			{
				using (IDataReader dr = DataProvider.Instance().Tags_Search(PortalId, ModuleId, q))
				{
					while (dr.Read())
					{
						sb.AppendLine("{\"id\":\"" + dr["TagId"].ToString() + "\",\"name\":\"" + dr["TagName"].ToString() + "\",\"type\":\"0\"},");
						i += 1;
					}
					dr.Close();
				}
			}
			string @out = "[";
			if (i > 0)
			{
				@out += sb.ToString().Trim();
				@out = @out.Substring(0, @out.Length - 1);
			}
			@out += "]";
			return @out;
		}
		private string DeletePost()
		{
			int replyId = -1;
			int TopicId = -1;
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				TopicId = int.Parse(Params["topicid"].ToString());
			}
			if (Params.ContainsKey("replyid") && SimulateIsNumeric.IsNumeric(Params["replyid"]))
			{
				replyId = int.Parse(Params["replyid"].ToString());
			}
			int forumId = -1;
			Data.ForumsDB db = new Data.ForumsDB();
			forumId = db.Forum_GetByTopicId(TopicId);
			ForumController fc = new ForumController();
			Forum f = fc.Forums_Get(forumId, this.UserId, true);

			// Need to get the list of attachments BEFORE we remove the post recods
			var attachmentController = new Data.AttachController();
			var attachmentList = (MainSettings.DeleteBehavior == 0)
									 ? attachmentController.ListForPost(TopicId, replyId)
									 : null;


			if (TopicId > 0 & replyId < 1)
			{
				TopicsController tc = new TopicsController();
				TopicInfo ti = tc.Topics_Get(PortalId, ModuleId, TopicId);

				if (Permissions.HasAccess(f.Security.ModDelete, ForumUser.UserRoles) || (Permissions.HasAccess(f.Security.Delete, ForumUser.UserRoles) && ti.Content.AuthorId == UserId && ti.IsLocked == false))
				{
					DataProvider.Instance().Topics_Delete(forumId, TopicId, MainSettings.DeleteBehavior);
					string journalKey = string.Format("{0}:{1}", forumId.ToString(), TopicId.ToString());
					JournalController.Instance.DeleteJournalItemByKey(PortalId, journalKey);
				}
				else
				{
					return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
				}
			}
			else
			{
				ReplyController rc = new ReplyController();
				ReplyInfo ri = rc.Reply_Get(PortalId, ModuleId, TopicId, replyId);
				if (Permissions.HasAccess(f.Security.ModDelete, ForumUser.UserRoles) || (Permissions.HasAccess(f.Security.Delete, ForumUser.UserRoles) && ri.Content.AuthorId == UserId))
				{
					DataProvider.Instance().Reply_Delete(forumId, TopicId, replyId, MainSettings.DeleteBehavior);
					string journalKey = string.Format("{0}:{1}:{2}", forumId.ToString(), TopicId.ToString(), replyId.ToString());
					JournalController.Instance.DeleteJournalItemByKey(PortalId, journalKey);

				}
				else
				{
					return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
				}

			}

			// If it's a hard delete, delete associated attachments
			// attachmentList will only be populated if the DeleteBehavior is 0
			if (attachmentList != null)
			{      
				var fileManager = FileManager.Instance;
				var folderManager = FolderManager.Instance;
				var attachmentFolder = folderManager.GetFolder(PortalId, "activeforums_Attach");

				foreach (var attachment in attachmentList)
				{
					attachmentController.Delete(attachment.AttachmentId);

					var file = attachment.FileId.HasValue
								   ? fileManager.GetFile(attachment.FileId.Value)
								   : fileManager.GetFile(attachmentFolder, attachment.FileName);

					// Only delete the file if it exists in the attachment folder
					if (file != null && file.FolderId == attachmentFolder.FolderID)
						fileManager.DeleteFile(file);
				}
			}

			// Return the result
			string cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
			DataCache.CacheClearPrefix(cachekey);
			return BuildOutput(TopicId + "|" + replyId, OutputCodes.Success, true);
		}
		private string LoadTopic()
		{
			int topicId = -1;
			int forumId = -1;
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			if (topicId > 0)
			{
				TopicsController tc = new TopicsController();
				TopicInfo t = tc.Topics_Get(PortalId, ModuleId, topicId);
				Data.ForumsDB db = new Data.ForumsDB();
				forumId = db.Forum_GetByTopicId(topicId);
				ForumController fc = new ForumController();
				Forum f = fc.Forums_Get(PortalId, -1, forumId, this.UserId, true, false, -1);
				if (f != null)
				{
					if (Permissions.HasPerm(f.Security.ModEdit, ForumUser.UserRoles))
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("{");
						sb.Append(Utilities.JSON.Pair("topicid", t.TopicId.ToString()));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("subject", t.Content.Subject));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("authorid", t.Content.AuthorId.ToString()));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("locked", t.IsLocked.ToString()));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("pinned", t.IsPinned.ToString()));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("priority", t.Priority.ToString()));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("status", t.StatusId.ToString()));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("forumid", forumId.ToString()));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("forumname", f.ForumName));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("tags", t.Tags));
						sb.Append(",");
						sb.Append(Utilities.JSON.Pair("categories", t.Categories));
						sb.Append(",");
						sb.Append("\"properties\":[");
						string sCats = string.Empty;
						if (f.Properties != null)
						{
							int i = 0;
							foreach (PropertiesInfo p in f.Properties)
							{
								sb.Append("{");
								sb.Append(Utilities.JSON.Pair("propertyid", p.PropertyId.ToString()));
								sb.Append(",");
								sb.Append(Utilities.JSON.Pair("datatype", p.DataType));
								sb.Append(",");
								sb.Append(Utilities.JSON.Pair("propertyname", p.Name));
								sb.Append(",");
								string pvalue = p.DefaultValue;
								foreach (PropertiesInfo tp in t.TopicProperties)
								{
									if (tp.PropertyId == p.PropertyId)
									{
										pvalue = tp.DefaultValue;
									}
								}

								sb.Append(Utilities.JSON.Pair("propertyvalue", pvalue));
								if (p.DataType.Contains("list"))
								{
									sb.Append(",\"listdata\":[");
									if (p.DataType.Contains("list|categories"))
									{
										using (IDataReader dr = DataProvider.Instance().Tags_List(PortalId, f.ModuleId, true, 0, 200, "ASC", "TagName", forumId, f.ForumGroupId))
										{
											dr.NextResult();
											while (dr.Read())
											{
												sCats += "{";
												sCats += Utilities.JSON.Pair("id", dr["TagId"].ToString());
												sCats += ",";
												sCats += Utilities.JSON.Pair("name", dr["TagName"].ToString());
												sCats += ",";
												sCats += Utilities.JSON.Pair("selected", IsSelected(dr["TagName"].ToString(), t.Categories).ToString());
												sCats += "},";
											}
											dr.Close();
										}
										if (! (string.IsNullOrEmpty(sCats)))
										{
											sCats = sCats.Substring(0, sCats.Length - 1);
										}
										sb.Append(sCats);
									}
									else
									{
										DotNetNuke.Common.Lists.ListController lists = new DotNetNuke.Common.Lists.ListController();
										string lName = p.DataType.Substring(p.DataType.IndexOf("|") + 1);
										DotNetNuke.Common.Lists.ListEntryInfoCollection lc = lists.GetListEntryInfoCollection(lName, string.Empty);
										int il = 0;
										foreach (DotNetNuke.Common.Lists.ListEntryInfo l in lc)
										{
											sb.Append("{");
											sb.Append(Utilities.JSON.Pair("itemId", l.Value));
											sb.Append(",");
											sb.Append(Utilities.JSON.Pair("itemName", l.Text));
											sb.Append("}");
											il += 1;
											if (il < lc.Count)
											{
												sb.Append(",");
											}
										}
									}
									sb.Append("]");
								}
								sb.Append("}");
								i += 1;
								if (i < f.Properties.Count)
								{
									sb.Append(",");
								}

							}
						}




						sb.Append("],\"categories\":[");
						sCats = string.Empty;
						using (IDataReader dr = DataProvider.Instance().Tags_List(PortalId, f.ModuleId, true, 0, 200, "ASC", "TagName", forumId, f.ForumGroupId))
						{
							dr.NextResult();
							while (dr.Read())
							{
								sCats += "{";
								sCats += Utilities.JSON.Pair("id", dr["TagId"].ToString());
								sCats += ",";
								sCats += Utilities.JSON.Pair("name", dr["TagName"].ToString());
								sCats += ",";
								sCats += Utilities.JSON.Pair("selected", IsSelected(dr["TagName"].ToString(), t.Categories).ToString());
								sCats += "},";
							}
							dr.Close();
						}
						if (! (string.IsNullOrEmpty(sCats)))
						{
							sCats = sCats.Substring(0, sCats.Length - 1);
						}
						sb.Append(sCats);
						sb.Append("]");
						sb.Append("}");
						return BuildOutput(sb.ToString(), OutputCodes.Success, true, true);
					}
				}

			}
			return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
		}
		private string SaveTopic()
		{
			int topicId = -1;
			int forumId = -1;
			if (Params.ContainsKey("topicid") && SimulateIsNumeric.IsNumeric(Params["topicid"]))
			{
				topicId = int.Parse(Params["topicid"].ToString());
			}
			if (topicId > 0)
			{
				TopicsController tc = new TopicsController();
				TopicInfo t = tc.Topics_Get(PortalId, ModuleId, topicId);
				Data.ForumsDB db = new Data.ForumsDB();
				forumId = db.Forum_GetByTopicId(topicId);
				ForumController fc = new ForumController();
				Forum f = fc.Forums_Get(PortalId, -1, forumId, this.UserId, true, false, -1);
				if (Permissions.HasPerm(f.Security.ModEdit, ForumUser.UserRoles))
				{
					string subject = Params["subject"].ToString();
					subject = Utilities.XSSFilter(subject, true);
					if (! (string.IsNullOrEmpty(f.PrefixURL)))
					{
						string cleanSubject = Utilities.CleanName(subject).ToLowerInvariant();
						if (SimulateIsNumeric.IsNumeric(cleanSubject))
						{
							cleanSubject = "Topic-" + cleanSubject;
						}
						string topicUrl = cleanSubject;
						string urlPrefix = "/";
						if (! (string.IsNullOrEmpty(f.ForumGroup.PrefixURL)))
						{
							urlPrefix += f.ForumGroup.PrefixURL + "/";
						}
						if (! (string.IsNullOrEmpty(f.PrefixURL)))
						{
							urlPrefix += f.PrefixURL + "/";
						}
						string urlToCheck = urlPrefix + cleanSubject;
						Data.Topics topicsDb = new Data.Topics();
						for (int u = 0; u <= 200; u++)
						{
							int tid = topicsDb.TopicIdByUrl(PortalId, f.ModuleId, urlToCheck);
							if (tid > 0 && tid == topicId)
							{
								break;
							}
							else if (tid > 0)
							{
								topicUrl = (u + 1) + "-" + cleanSubject;
								urlToCheck = urlPrefix + topicUrl;
							}
							else
							{
								break;
							}
						}
						if (topicUrl.Length > 150)
						{
							topicUrl = topicUrl.Substring(0, 149);
							topicUrl = topicUrl.Substring(0, topicUrl.LastIndexOf("-"));
						}
						t.TopicUrl = topicUrl;
						//.URL = topicUrl
					}
					else
					{
						//.URL = String.Empty
						t.TopicUrl = string.Empty;
					}
					t.Content.Subject = subject;
					t.IsPinned = bool.Parse(Params["pinned"].ToString());
					t.IsLocked = bool.Parse(Params["locked"].ToString());
					t.Priority = int.Parse(Params["priority"].ToString());
					t.StatusId = int.Parse(Params["status"].ToString());
					if (f.Properties != null)
					{
						StringBuilder tData = new StringBuilder();
						tData.Append("<topicdata>");
						tData.Append("<properties>");
						foreach (PropertiesInfo p in f.Properties)
						{
							string pkey = "prop-" + p.PropertyId.ToString();

							tData.Append("<property id=\"" + p.PropertyId.ToString() + "\">");
							tData.Append("<name><![CDATA[");
							tData.Append(p.Name);
							tData.Append("]]></name>");
							if (Params[pkey] != null)
							{
								tData.Append("<value><![CDATA[");
								tData.Append(Utilities.XSSFilter(Params[pkey].ToString()));
								tData.Append("]]></value>");
							}
							else
							{
								tData.Append("<value></value>");
							}
							tData.Append("</property>");
						}
						tData.Append("</properties>");
						tData.Append("</topicdata>");
						t.TopicData = tData.ToString();
					}
				}
				tc.TopicSave(PortalId, t);
				if (Params["tags"] != null)
				{
					DataProvider.Instance().Tags_DeleteByTopicId(PortalId, f.ModuleId, topicId);
					string tagForm = string.Empty;
					if (Params["tags"] != null)
					{
						tagForm = Params["tags"].ToString();
					}
					if (! (tagForm == string.Empty))
					{
						string[] Tags = tagForm.Split(',');
						foreach (string tag in Tags)
						{
							string sTag = Utilities.CleanString(PortalId, tag.Trim(), false, EditorTypes.TEXTBOX, false, false, f.ModuleId, string.Empty, false);
							DataProvider.Instance().Tags_Save(PortalId, f.ModuleId, -1, sTag, 0, 1, 0, topicId, false, -1, -1);
						}
					}
				}

				if (Params["categories"] != null)
				{
					string[] cats = Params["categories"].ToString().Split(';');
					DataProvider.Instance().Tags_DeleteTopicToCategory(PortalId, f.ModuleId, -1, topicId);
					foreach (string c in cats)
					{
						int cid = -1;
						if (! (string.IsNullOrEmpty(c)) && SimulateIsNumeric.IsNumeric(c))
						{
							cid = Convert.ToInt32(c);
							if (cid > 0)
							{
								DataProvider.Instance().Tags_AddTopicToCategory(PortalId, f.ModuleId, cid, topicId);
							}
						}
					}
				}
			}


			return BuildOutput(string.Empty, OutputCodes.UnsupportedRequest, false);
		}
		private bool IsSelected(string TagName, string selectedValues)
		{
			if (string.IsNullOrEmpty(selectedValues))
			{
				return false;
			}
			else
			{
				foreach (string s in selectedValues.Split('|'))
				{
					if (! (string.IsNullOrEmpty(s)))
					{
						if (s.ToLowerInvariant().Trim() == TagName.ToLowerInvariant().Trim())
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}
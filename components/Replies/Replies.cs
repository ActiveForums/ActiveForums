//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
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
using System.IO;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Journal;

namespace DotNetNuke.Modules.ActiveForums
{
#region ReplyInfo
	public class ReplyInfo
	{
#region Private Members
		private int _ReplyId;
		private int _TopicId;
		private int _ReplyToId;
		private int _ContentId;
		private int _StatusId;
		private bool _IsApproved;
		private bool _IsDeleted;
		private Content _Content;
		private Author _Author;
#endregion
#region Public Properties
		public int ReplyId
		{
			get
			{
				return _ReplyId;
			}
			set
			{
				_ReplyId = value;
			}
		}
		public int ReplyToId
		{
			get
			{
				return _ReplyToId;
			}
			set
			{
				_ReplyToId = value;
			}
		}
		public int TopicId
		{
			get
			{
				return _TopicId;
			}
			set
			{
				_TopicId = value;
			}
		}
		public int ContentId
		{
			get
			{
				return _ContentId;
			}
			set
			{
				_ContentId = value;
			}
		}
		public int StatusId
		{
			get
			{
				return _StatusId;
			}
			set
			{
				_StatusId = value;
			}
		}
		public bool IsApproved
		{
			get
			{
				return _IsApproved;
			}
			set
			{
				_IsApproved = value;
			}
		}
		public bool IsDeleted
		{
			get
			{
				return _IsDeleted;
			}
			set
			{
				_IsDeleted = value;
			}
		}
		public Content Content
		{
			get
			{
				return _Content;
			}
			set
			{
				_Content = value;
			}
		}
		public Author Author
		{
			get
			{
				return _Author;
			}
			set
			{
				_Author = value;
			}
		}
#endregion
		public ReplyInfo()
		{
			Content = new Content();
			Author = new Author();
		}
	}
#endregion
#region Reply Controller
	public class ReplyController
	{
#region Public Methods
		public void Reply_Delete(int PortalId, int ForumId, int TopicId, int ReplyId, int DelBehavior)
		{
            DataProvider.Instance().Reply_Delete(ForumId, TopicId, ReplyId, DelBehavior);
			var objectKey = string.Format("{0}:{1}:{2}", ForumId, TopicId, ReplyId);
			JournalController.Instance.DeleteJournalItemByKey(PortalId, objectKey);

		    if (DelBehavior != 0) 
                return;

            // If it's a hard delete, delete associated attachments
		    var attachmentController = new Data.AttachController();
		    var fileManager = FileManager.Instance;
            var folderManager = FolderManager.Instance;
		    var attachmentFolder = folderManager.GetFolder(PortalId, "activeforums_Attach");

		    foreach(var attachment in attachmentController.ListForPost(TopicId, ReplyId))
		    {
                attachmentController.Delete(attachment.AttachmentId);

                var file = attachment.FileId.HasValue ? fileManager.GetFile(attachment.FileId.Value) : fileManager.GetFile(attachmentFolder, attachment.FileName);

                // Only delete the file if it exists in the attachment folder
                if (file != null && file.FolderId == attachmentFolder.FolderID)
                    fileManager.DeleteFile(file); 
		    }
		}
		public int Reply_QuickCreate(int PortalId, int ModuleId, int ForumId, int TopicId, int ReplyToId, string Subject, string Body, int UserId, string DisplayName, bool IsApproved, string IPAddress)
		{
			int replyId = -1;
			ReplyInfo ri = new ReplyInfo();
			DateTime dt = DateTime.Now;
			ri.Content.DateUpdated = dt;
			ri.Content.DateCreated = dt;
			ri.Content.AuthorId = UserId;
			ri.Content.AuthorName = DisplayName;
			ri.Content.Subject = Subject;
			ri.Content.Body = Body;
			ri.Content.IPAddress = IPAddress;
			ri.Content.Summary = string.Empty;
			ri.IsApproved = IsApproved;
			ri.IsDeleted = false;
			ri.ReplyToId = ReplyToId;
			ri.StatusId = -1;
			ri.TopicId = TopicId;
			replyId = Reply_Save(PortalId, ri);
			return replyId;
		}
		public int Reply_Save(int PortalId, ReplyInfo ri)
		{
			return Convert.ToInt32(DataProvider.Instance().Reply_Save(PortalId, ri.TopicId, ri.ReplyId, ri.ReplyToId, ri.StatusId, ri.IsApproved, ri.IsDeleted, ri.Content.Subject.Trim(), ri.Content.Body.Trim(), ri.Content.DateCreated, ri.Content.DateUpdated, ri.Content.AuthorId, ri.Content.AuthorName, ri.Content.IPAddress));
		}
		public ReplyInfo Reply_Get(int PortalId, int ModuleId, int TopicId, int ReplyId)
		{
			IDataReader dr = DataProvider.Instance().Reply_Get(PortalId, ModuleId, TopicId, ReplyId);
			ReplyInfo ri = null;
			while (dr.Read())
			{
				ri = new ReplyInfo();
				ri.ReplyId = Convert.ToInt32(dr["ReplyId"]);
				ri.ReplyToId = Convert.ToInt32(dr["ReplyToId"]);
				ri.Content.AuthorId = Convert.ToInt32(dr["AuthorId"]);
				ri.Content.AuthorName = dr["AuthorName"].ToString();
				ri.Content.Body = dr["Body"].ToString();
				ri.Content.ContentId = Convert.ToInt32(dr["ContentId"]);
				ri.Content.DateCreated = Convert.ToDateTime(dr["DateCreated"]);
				ri.Content.DateUpdated = Convert.ToDateTime(dr["DateUpdated"]);
				ri.Content.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
				ri.Content.Subject = dr["Subject"].ToString();
				ri.Content.Summary = dr["Summary"].ToString();
				ri.Content.IPAddress = dr["IPAddress"].ToString();
				ri.Author.AuthorId = ri.Content.AuthorId;
				ri.Author.DisplayName = dr["DisplayName"].ToString();
				ri.Author.Email = dr["Email"].ToString();
				ri.Author.FirstName = dr["FirstName"].ToString();
				ri.Author.LastName = dr["LastName"].ToString();
				ri.Author.Username = dr["Username"].ToString();
				ri.ContentId = Convert.ToInt32(dr["ContentId"]);
				ri.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
				ri.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
				ri.StatusId = Convert.ToInt32(dr["StatusId"]);
				ri.TopicId = Convert.ToInt32(dr["TopicId"]);
				//tl.Add(ti)
			}
			dr.Close();
			return ri;
		}
		public ReplyInfo ApproveReply(int PortalId, int TabId, int ModuleId, int ForumId, int TopicId, int ReplyId)
		{
			SettingsInfo ms = DataCache.MainSettings(ModuleId);
			ForumController fc = new ForumController();
			Forum fi = fc.Forums_Get(ForumId, -1, false, true);

			ReplyController rc = new ReplyController();
			ReplyInfo reply = rc.Reply_Get(PortalId, ModuleId, TopicId, ReplyId);
			if (reply == null)
			{
				return null;
			}
			reply.IsApproved = true;
			rc.Reply_Save(PortalId, reply);
			TopicsController tc = new TopicsController();
			tc.Topics_SaveToForum(ForumId, TopicId, PortalId, ModuleId, ReplyId);
			TopicInfo topic = tc.Topics_Get(PortalId, ModuleId, TopicId, ForumId, -1, false);

			if (fi.ModApproveTemplateId > 0 & reply.Author.AuthorId > 0)
			{
				Email.SendEmail(fi.ModApproveTemplateId, PortalId, ModuleId, TabId, ForumId, TopicId, ReplyId, string.Empty, reply.Author);
			}

			Subscriptions.SendSubscriptions(PortalId, ModuleId, TabId, ForumId, TopicId, ReplyId, reply.Content.AuthorId);

			try
			{
				ControlUtils ctlUtils = new ControlUtils();
                string fullURL = ctlUtils.BuildUrl(TabId, ModuleId, fi.ForumGroup.PrefixURL, fi.PrefixURL, fi.ForumGroupId, ForumId, TopicId, topic.TopicUrl, -1, -1, string.Empty, 1, ReplyId, fi.SocialGroupId);

				if (fullURL.Contains("~/"))
				{
					fullURL = Utilities.NavigateUrl(TabId, "", new string[] {ParamKeys.TopicId + "=" + TopicId, ParamKeys.ContentJumpId + "=" + ReplyId});
				}
				if (fullURL.EndsWith("/"))
				{
					fullURL += "?" + ParamKeys.ContentJumpId + "=" + ReplyId;
				}
				Social amas = new Social();
				amas.AddReplyToJournal(PortalId, ModuleId, ForumId, TopicId, ReplyId, reply.Author.AuthorId, fullURL, reply.Content.Subject, string.Empty, reply.Content.Body, fi.ActiveSocialSecurityOption, fi.Security.Read, fi.SocialGroupId);
			}
			catch (Exception ex)
			{
				DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
			}
			return reply;
		}
#endregion
	}
#endregion

}


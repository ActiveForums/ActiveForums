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
using System.Xml;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Journal;
using System.Text.RegularExpressions;

namespace DotNetNuke.Modules.ActiveForums
{
#region TopicInfo Class
	public class TopicInfo
	{
		public TopicInfo()
		{
			Content = new Content();
			Security = new PermissionInfo();
			Author = new Author();
		}
#region Private Members
		private int _TopicId;
		private int _ContentId;
		private int _ViewCount;
		private int _ReplyCount;
		private bool _IsLocked;
		private bool _IsPinned;
		private string _TopicIcon;
		private int _StatusId;
		private bool _IsApproved;
		private bool _IsDeleted;
		private bool _IsAnnounce;
		private bool _IsArchived;
		private TopicTypes _TopicType;
		private DateTime _AnnounceStart;
		private DateTime _AnnounceEnd;
		private Content _Content;
		private PermissionInfo _PermissionInfo;
		private Author _author;
		private string _tags;
#endregion
#region Public Properties
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
		public int ViewCount
		{
			get
			{
				return _ViewCount;
			}
			set
			{
				_ViewCount = value;
			}
		}
		public int ReplyCount
		{
			get
			{
				return _ReplyCount;
			}
			set
			{
				_ReplyCount = value;
			}
		}
		public bool IsLocked
		{
			get
			{
				return _IsLocked;
			}
			set
			{
				_IsLocked = value;
			}
		}
		public bool IsPinned
		{
			get
			{
				return _IsPinned;
			}
			set
			{
				_IsPinned = value;
			}
		}
		public string TopicIcon
		{
			get
			{
				return _TopicIcon;
			}
			set
			{
				_TopicIcon = value;
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
		public bool IsAnnounce
		{
			get
			{
				return _IsAnnounce;
			}
			set
			{
				_IsAnnounce = value;
			}
		}
		public bool IsArchived
		{
			get
			{
				return _IsArchived;
			}
			set
			{
				_IsArchived = value;
			}
		}
		public TopicTypes TopicType
		{
			get
			{
				return _TopicType;
			}
			set
			{
				_TopicType = value;
			}
		}
		public DateTime AnnounceStart
		{
			get
			{
				return _AnnounceStart;
			}
			set
			{
				_AnnounceStart = value;
			}
		}
		public DateTime AnnounceEnd
		{
			get
			{
				return _AnnounceEnd;
			}
			set
			{
				_AnnounceEnd = value;
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
		public PermissionInfo Security
		{
			get
			{
				return _PermissionInfo;
			}
			set
			{
				_PermissionInfo = value;
			}
		}
		public Author Author
		{
			get
			{
				return _author;
			}
			set
			{
				_author = value;
			}
		}
		public string Tags
		{
			get
			{
				return _tags;
			}
			set
			{
				_tags = value;
			}
		}
		private int _Priority = 0;
		public int Priority
		{
			get
			{
				return _Priority;
			}
			set
			{
				_Priority = value;
			}
		}
		private string _Categories = string.Empty;
		public string Categories
		{
			get
			{
				return _Categories;
			}
			set
			{
				_Categories = value;
			}
		}
		private string _TopicUrl = string.Empty;
		public string TopicUrl
		{
			get
			{
				return _TopicUrl;
			}
			set
			{
				_TopicUrl = value;
			}
		}
		private string _ForumURL = string.Empty;
		public string ForumURL
		{
			get
			{
				return _ForumURL;
			}
			set
			{
				_ForumURL = value;
			}
		}
		private string _TopicData = string.Empty;
		public string TopicData
		{
			get
			{
				return _TopicData;
			}
			set
			{
				_TopicData = value;
			}
		}
		public string URL
		{
			get
			{
				if (! (string.IsNullOrEmpty(TopicUrl)) && ! (string.IsNullOrEmpty(ForumURL)))
				{
					if (TopicUrl.StartsWith("/") & ForumURL.EndsWith("/"))
					{
						ForumURL = ForumURL.Substring(0, ForumURL.Length - 1);
						if (! (ForumURL.StartsWith("/")))
						{
							ForumURL = "/" + ForumURL;
						}
					}
					return ForumURL + TopicUrl;
				}
				else
				{
					return string.Empty;
				}
			}
		}
		public List<PropertiesInfo> TopicProperties
		{
			get
			{
				if (TopicData == string.Empty)
				{
					return null;
				}
				else
				{
					List<PropertiesInfo> pl = new List<PropertiesInfo>();
					XmlDocument xDoc = new XmlDocument();
					xDoc.LoadXml(TopicData);
					if (xDoc != null)
					{
						System.Xml.XmlNode xRoot = xDoc.DocumentElement;
						System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes("//properties/property");
						if (xNodeList.Count > 0)
						{
							int i = 0;
							for (i = 0; i < xNodeList.Count; i++)
							{
								string pName = Utilities.HTMLDecode(xNodeList[i].ChildNodes[0].InnerText);
								string pValue = Utilities.HTMLDecode(xNodeList[i].ChildNodes[1].InnerText);
								int pId = Convert.ToInt32(xNodeList[i].Attributes["id"].Value);
								PropertiesInfo p = new PropertiesInfo();
								p.Name = pName;
								p.DefaultValue = pValue;
								p.PropertyId = pId;
								pl.Add(p);
							}
						}
					}
					return pl;
				}
			}
		}

#endregion
	}
#endregion
#region Topics Controller
	public class TopicsController : DotNetNuke.Entities.Modules.ISearchable
	{
		public int Topic_QuickCreate(int PortalId, int ModuleId, int ForumId, string Subject, string Body, int UserId, string DisplayName, bool IsApproved, string IPAddress)
		{
			int topicId = -1;
			TopicInfo ti = new TopicInfo();
			ti.AnnounceEnd = Utilities.NullDate();
			ti.AnnounceStart = Utilities.NullDate();
			ti.Content.AuthorId = UserId;
			ti.Content.AuthorName = DisplayName;
			ti.Content.Subject = Subject;
			ti.Content.Body = Body;
			ti.Content.Summary = string.Empty;

			ti.Content.IPAddress = IPAddress;
			DateTime dt = DateTime.Now;
			ti.Content.DateCreated = dt;
			ti.Content.DateUpdated = dt;
			ti.IsAnnounce = false;
			ti.IsApproved = IsApproved;
			ti.IsArchived = false;
			ti.IsDeleted = false;
			ti.IsLocked = false;
			ti.IsPinned = false;
			ti.ReplyCount = 0;
			ti.StatusId = -1;
			ti.TopicIcon = string.Empty;
			ti.TopicType = TopicTypes.Topic;
			ti.ViewCount = 0;
			topicId = TopicSave(PortalId, ti);
			if (topicId > 0)
			{
				Topics_SaveToForum(ForumId, topicId, PortalId, ModuleId);
				if (UserId > 0)
				{
					Data.Profiles uc = new Data.Profiles();
					uc.Profile_UpdateTopicCount(PortalId, UserId);
				}
			}
			return topicId;
		}
        public void Replies_Split(int OldTopicId, int NewTopicId, string listreplies, bool isNew)
        {
            Regex rgx = new Regex(@"^\d+(\|\d+)*$");
            if (OldTopicId > 0 && NewTopicId > 0 && rgx.IsMatch(listreplies))
            {
                if (isNew)
                {
                    string[] slistreplies = listreplies.Split("|".ToCharArray(), 2);
                    string str = "";
                    if (slistreplies.Length > 1) str = slistreplies[1];
                    DataProvider.Instance().Replies_Split(OldTopicId, NewTopicId, str, DateTime.Now, Convert.ToInt32(slistreplies[0]));
                }
                else
                {
                    DataProvider.Instance().Replies_Split(OldTopicId, NewTopicId, listreplies, DateTime.Now, 0);
                }
            }
        }
		public int TopicSave(int PortalId, TopicInfo ti)
		{
            return Convert.ToInt32(DataProvider.Instance().Topics_Save(PortalId, ti.TopicId, ti.ViewCount, ti.ReplyCount, ti.IsLocked, ti.IsPinned, ti.TopicIcon, ti.StatusId, ti.IsApproved, ti.IsDeleted, ti.IsAnnounce, ti.IsArchived, ti.AnnounceStart, ti.AnnounceEnd, ti.Content.Subject.Trim(), ti.Content.Body.Trim(), ti.Content.Summary.Trim(), ti.Content.DateCreated, ti.Content.DateUpdated, ti.Content.AuthorId, ti.Content.AuthorName, ti.Content.IPAddress, (int)ti.TopicType, ti.Priority, ti.TopicUrl, ti.TopicData));
		}
		public int Topics_SaveToForum(int ForumId, int TopicId, int PortalId, int ModuleId)
		{
			int id = Topics_SaveToForum(ForumId, TopicId, PortalId, ModuleId, -1);
			return id;
		}
		public int Topics_SaveToForum(int ForumId, int TopicId, int PortalId, int ModuleId, int LastReplyId)
		{
			int id = Convert.ToInt32(DataProvider.Instance().Topics_SaveToForum(ForumId, TopicId, LastReplyId));

			return id;
		}
		public List<TopicInfo> TopicsList(int ForumId, int ModuleId, int PortalId)
		{
			IDataReader dr = DataProvider.Instance().Topics_List(ForumId, ModuleId, PortalId);
			List<TopicInfo> tl = new List<TopicInfo>();
			while (dr.Read())
			{
				TopicInfo ti = new TopicInfo();
				if (! (dr["AnnounceEnd"] == DBNull.Value))
				{
					ti.AnnounceEnd = Convert.ToDateTime(dr["AnnounceEnd"]);
				}

				if (! (dr["AnnounceStart"] == DBNull.Value))
				{
					ti.AnnounceStart = Convert.ToDateTime(dr["AnnounceStart"]);
				}
				ti.Content.AuthorId = Convert.ToInt32(dr["AuthorId"]);
				ti.Content.AuthorName = dr["AuthorName"].ToString();
				ti.Content.Body = dr["Body"].ToString();
				ti.Content.ContentId = Convert.ToInt32(dr["ContentId"]);
				ti.Content.DateCreated = Convert.ToDateTime(dr["DateCreated"]);
				ti.Content.DateUpdated = Convert.ToDateTime(dr["DateUpdated"]);
				ti.Content.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
				ti.Content.Subject = dr["Subject"].ToString();
				ti.Content.Summary = dr["Summary"].ToString();
				ti.Author.AuthorId = ti.Content.AuthorId;
				ti.Author.DisplayName = dr["DisplayName"].ToString();
				ti.Author.Email = dr["Email"].ToString();
				ti.Author.FirstName = dr["FirstName"].ToString();
				ti.Author.LastName = dr["LastName"].ToString();
				ti.Author.Username = dr["Username"].ToString();
				ti.ContentId = Convert.ToInt32(dr["ContentId"]);
				ti.IsAnnounce = Convert.ToBoolean(dr["IsAnnounce"]);
				ti.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
				ti.IsArchived = Convert.ToBoolean(dr["IsArchived"]);
				ti.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
				ti.IsLocked = Convert.ToBoolean(dr["IsLocked"]);
				ti.IsPinned = Convert.ToBoolean(dr["IsPinned"]);
				ti.ReplyCount = Convert.ToInt32(dr["ReplyCount"]);
				ti.StatusId = Convert.ToInt32(dr["StatusId"]);
				ti.TopicIcon = Convert.ToString(dr["TopicIcon"]);
				ti.TopicId = Convert.ToInt32(dr["TopicId"]);
				ti.TopicType = (TopicTypes)(dr["TopicType"]);
				ti.ViewCount = Convert.ToInt32(dr["ViewCount"]);
				ti.Tags = dr["Tags"].ToString();
				tl.Add(ti);
			}
			dr.Close();
			return tl;
		}
		public TopicInfo Topics_Get(int PortalId, int ModuleId, int TopicId)
		{
			return Topics_Get(PortalId, ModuleId, TopicId, -1, -1, false);
		}
		public TopicInfo Topics_Get(int PortalId, int ModuleId, int TopicId, int ForumId, int UserId, bool WithSecurity)
		{
			IDataReader dr = DataProvider.Instance().Topics_Get(PortalId, ModuleId, TopicId, ForumId, UserId, WithSecurity);
			TopicInfo ti = null;
			while (dr.Read())
			{
				ti = new TopicInfo();
				if (! (dr["AnnounceEnd"] == DBNull.Value))
				{
					ti.AnnounceEnd = Convert.ToDateTime(dr["AnnounceEnd"]);
				}

				if (! (dr["AnnounceStart"] == DBNull.Value))
				{
					ti.AnnounceStart = Convert.ToDateTime(dr["AnnounceStart"]);
				}
				ti.Content.AuthorId = Convert.ToInt32(dr["AuthorId"]);
				ti.Content.AuthorName = dr["AuthorName"].ToString();
				ti.Content.Body = dr["Body"].ToString();
				ti.Content.ContentId = Convert.ToInt32(dr["ContentId"]);
				ti.Content.DateCreated = Convert.ToDateTime(dr["DateCreated"]);
				ti.Content.DateUpdated = Convert.ToDateTime(dr["DateUpdated"]);
				ti.Content.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
				ti.Content.Subject = dr["Subject"].ToString();
				ti.Content.Summary = dr["Summary"].ToString();
				ti.ContentId = Convert.ToInt32(dr["ContentId"]);
				ti.Author.AuthorId = ti.Content.AuthorId;
				ti.Author.DisplayName = dr["DisplayName"].ToString();
				ti.Author.Email = dr["Email"].ToString();
				ti.Author.FirstName = dr["FirstName"].ToString();
				ti.Author.LastName = dr["LastName"].ToString();
				ti.Author.Username = dr["Username"].ToString();
				ti.IsAnnounce = Convert.ToBoolean(dr["IsAnnounce"]);
				ti.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
				ti.IsArchived = Convert.ToBoolean(dr["IsArchived"]);
				ti.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);
				ti.IsLocked = Convert.ToBoolean(dr["IsLocked"]);
				ti.IsPinned = Convert.ToBoolean(dr["IsPinned"]);
				ti.ReplyCount = Convert.ToInt32(dr["ReplyCount"]);
				ti.StatusId = Convert.ToInt32(dr["StatusId"]);
				ti.TopicIcon = Convert.ToString(dr["TopicIcon"]);
				ti.TopicId = Convert.ToInt32(dr["TopicId"]);
				ti.TopicType = (TopicTypes)(dr["TopicType"]);
				ti.ViewCount = Convert.ToInt32(dr["ViewCount"]);
				ti.Tags = dr["Tags"].ToString();
				ti.Priority = Convert.ToInt32(dr["Priority"].ToString());
				ti.Categories = dr["Categories"].ToString();
				ti.ForumURL = dr["ForumURL"].ToString();
				ti.TopicUrl = dr["TopicURL"].ToString();
					//.URL = dr("URL").ToString
				try
				{
					ti.TopicData = dr["TopicData"].ToString();
				}
				catch (Exception ex)
				{
					ti.TopicData = string.Empty;
				}

			}
			//If WithSecurity Then
			//    dr.NextResult()
			//    'Dim tmpDr As IDataReader = dr
			//    ti.Security = CType(DotNetNuke.Common.Utilities.CBO.FillObject(dr, GetType(PermissionInfo)), PermissionInfo)
			//End If
			if (! dr.IsClosed)
			{
				dr.Close();
			}

			return ti;
		}
		public void Topics_Delete(int PortalId, int ModuleId, int ForumId, int TopicId, int DelBehavior)
		{
            DataProvider.Instance().Topics_Delete(ForumId, TopicId, DelBehavior);
			var cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
			DataCache.CacheClearPrefix(cachekey);
			try
			{
				var objectKey = string.Format("{0}:{1}", ForumId, TopicId);
				JournalController.Instance.DeleteJournalItemByKey(PortalId, objectKey);
			}
			catch (Exception ex)
			{

			}

            if (DelBehavior != 0)
                return;

            // If it's a hard delete, delete associated attachments
            var attachmentController = new Data.AttachController();
            var fileManager = FileManager.Instance;
            var folderManager = FolderManager.Instance;
            var attachmentFolder = folderManager.GetFolder(PortalId, "activeforums_Attach");

            foreach (var attachment in attachmentController.ListForPost(TopicId, null))
            {
                attachmentController.Delete(attachment.AttachmentId);

                var file = attachment.FileId.HasValue ? fileManager.GetFile(attachment.FileId.Value) : fileManager.GetFile(attachmentFolder, attachment.FileName);

                // Only delete the file if it exists in the attachment folder
                if (file != null && file.FolderId == attachmentFolder.FolderID)
                    fileManager.DeleteFile(file);
            }


		}
		public void Topics_Move(int PortalId, int ModuleId, int ForumId, int TopicId)
		{
			SettingsInfo settings = DataCache.MainSettings(ModuleId);
			if (settings.URLRewriteEnabled)
			{
				try
				{
					Data.ForumsDB db = new Data.ForumsDB();
					int oldForumId = -1;
					oldForumId = db.Forum_GetByTopicId(TopicId);
					ForumController fc = new ForumController();
					Forum fi = fc.Forums_Get(oldForumId, -1, false);
					if (! (string.IsNullOrEmpty(fi.PrefixURL)))
					{
						Data.Common dbC = new Data.Common();
						string sURL = dbC.GetUrl(ModuleId, fi.ForumGroupId, oldForumId, TopicId, -1, -1);
						if (! (string.IsNullOrEmpty(sURL)))
						{
							dbC.ArchiveURL(PortalId, fi.ForumGroupId, ForumId, TopicId, sURL);
						}
					}
				}
				catch (Exception ex)
				{

				}
			}
			DataProvider.Instance().Topics_Move(PortalId, ModuleId, ForumId, TopicId);
		}
		public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(DotNetNuke.Entities.Modules.ModuleInfo ModInfo)
		{
			DotNetNuke.Services.Search.SearchItemInfoCollection SearchItemCollection = new DotNetNuke.Services.Search.SearchItemInfoCollection();
			IDataReader dr = null;
			try
			{
				dr = DataProvider.Instance().Search_DotNetNuke(ModInfo.ModuleID);
				DotNetNuke.Services.Search.SearchItemInfo SearchItem = null;
				while (dr.Read())
				{
					string subject = dr["Subject"].ToString();
					string description = string.Empty;
					string body = dr["Body"].ToString();
					int authorid = Convert.ToInt32(dr["AuthorId"]);
					DateTime datecreated = Convert.ToDateTime(dr["DateCreated"]);
					DateTime dateupdated = Convert.ToDateTime(dr["DateUpdated"]);
					int contentid = Convert.ToInt32(dr["ContentId"]);
					int forumid = Convert.ToInt32(dr["ForumId"]);
					int topicid = Convert.ToInt32(dr["TopicId"]);
					int replyId = Convert.ToInt32(dr["ReplyId"]);
					int jumpid = topicid;
					if (replyId > 0)
					{
						jumpid = replyId;
					}
					body = System.Web.HttpUtility.HtmlDecode(body);
					body = Utilities.StripHTMLTag(body);
					if (! (string.IsNullOrEmpty(body)))
					{
						if (body.Length > 100)
						{
							description = body.Substring(0, 100) + "...";
						}
						else
						{
							description = body;
						}
					}
					SearchItem = new DotNetNuke.Services.Search.SearchItemInfo(subject, description, authorid, datecreated, ModInfo.ModuleID, contentid.ToString() + "-" + forumid.ToString(), body, ParamKeys.ForumId + "=" + forumid + "&" + ParamKeys.ViewType + "=" + Views.Topic + "&" + ParamKeys.TopicId + "=" + topicid + "&" + ParamKeys.ContentJumpId + "=" + jumpid);
					SearchItemCollection.Add(SearchItem);
				}
				dr.Close();
				return SearchItemCollection;
			}
			catch (Exception ex)
			{
				return null;
			}
			finally
			{
				if (dr != null)
				{
					if (! dr.IsClosed)
					{
						dr.Close();
					}
				}
			}
		}
		public TopicInfo ApproveTopic(int PortalId, int TabId, int ModuleId, int ForumId, int TopicId)
		{
			SettingsInfo ms = DataCache.MainSettings(ModuleId);
			ForumController fc = new ForumController();
			Forum fi = fc.Forums_Get(ForumId, -1, false, true);

			TopicsController tc = new TopicsController();
			TopicInfo topic = tc.Topics_Get(PortalId, ModuleId, TopicId, ForumId, -1, false);
			if (topic == null)
			{
				return null;
			}
			topic.IsApproved = true;
			tc.TopicSave(PortalId, topic);
			tc.Topics_SaveToForum(ForumId, TopicId, PortalId, ModuleId);

			if (fi.ModApproveTemplateId > 0 & topic.Author.AuthorId > 0)
			{
				Email.SendEmail(fi.ModApproveTemplateId, PortalId, ModuleId, TabId, ForumId, TopicId, 0, string.Empty, topic.Author);
			}

			Subscriptions.SendSubscriptions(PortalId, ModuleId, TabId, ForumId, TopicId, 0, topic.Content.AuthorId);

			try
			{
				ControlUtils ctlUtils = new ControlUtils();
				string sUrl = ctlUtils.BuildUrl(TabId, ModuleId, fi.ForumGroup.PrefixURL, fi.PrefixURL, fi.ForumGroupId, fi.ForumID, TopicId, topic.TopicUrl, -1, -1, string.Empty, 1, -1, fi.SocialGroupId);
				Social amas = new Social();
				amas.AddTopicToJournal(PortalId, ModuleId, ForumId, TopicId, topic.Author.AuthorId, sUrl, topic.Content.Subject, string.Empty, topic.Content.Body, fi.ActiveSocialSecurityOption, fi.Security.Read, fi.SocialGroupId);
			}
			catch (Exception ex)
			{
				DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
			}
			return topic;
		}
		//Public Function ActiveForums_GetPostsForSearch(ByVal ModuleID As Integer) As ArrayList
		//    Return CBO.FillCollection(DataProvider.Instance().ActiveForums_GetPostsForSearch(ModuleID), GetType(PostInfo))
		//End Function
	}

#endregion
}


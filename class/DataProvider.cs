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
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
    public abstract class DataProvider
    {
        private static DataProvider objProvider;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.ActiveForums", ""));
        }

        // return the provider
        public static new DataProvider Instance()
        {
            return objProvider;
        }

        //        Public MustOverride Function Utility_ExecuteSQL(ByVal SQL As String) As IDataReader

        #region Filters
        public abstract int Filters_Save(int PortalId, int ModuleId, int FilterId, string Find, string Replace, string FilterType);
        public abstract IDataReader Filters_Get(int PortalId, int ModuleId, int FilterId);
        public abstract IDataReader Filters_GetEmoticons(int ModuleId);
        public abstract IDataReader Filters_List(int PortalId, int ModuleId, int PageIndex, int PageSize, string Sort, string SortColumn);
        public abstract IDataReader Filters_ListByType(int PortalId, int ModuleId, string FilterType);
        public abstract void Filters_Delete(int PortalId, int ModuleId, int FilterId);
        public abstract void Filters_DeleteByModuleId(int PortalId, int ModuleId);
        #endregion
        #region Forums
        public abstract void Forums_Delete(int PortalId, int ModuleId, int ForumId);
        public abstract IDataReader Forums_Get(int PortalId, int ModuleId, int ForumID, int UserId, bool WithSecurity);
        public abstract IDataReader Forums_List(int PortalId, int ModuleId, int ForumGroupId, int ParentForumId, bool FillLastPost);
        public abstract void Forums_Move(int ModuleId, int ForumId, int SortDirection);
        public abstract int Forum_Save(int PortalId, int ForumId, int ModuleId, int ForumGroupId, int ParentForumId, string ForumName, string ForumDesc, int SortOrder, bool Active, bool Hidden, string ForumSettingsKey, int PermissionsId, string PrefixURL, int SocialGroupId, bool HasProperties);
        public abstract void Forum_ConfigCleanUp(int ModuleId, string ForumSettingsKey, string ForumSecurityKey);
        #endregion
        #region Groups
        public abstract void Groups_Delete(int ModuleID, int ForumGroupID);
        public abstract IDataReader Groups_Get(int ModuleId, int ForumGroupID);
        public abstract IDataReader Groups_List(int ModuleId);
        public abstract void Groups_Move(int ModuleId, int ForumGroupId, int SortDirection);
        public abstract int Groups_Save(int PortalId, int ModuleId, int ForumGroupId, string GroupName, int SortOrder, bool Active, bool Hidden, int PermissionsId, string PrefixURL);
        #endregion
        #region Polls
        public abstract DataSet Poll_Get(int TopicId);
        public abstract IDataReader Poll_GetResults(int TopicId);
        public abstract void Poll_Option_Save(int PollOptionsId, int PollId, string OptionName, int TopicId);
        public abstract int Poll_Save(int PollId, int TopicId, int UserId, string Question, string PollType);
        public abstract void Poll_Vote(int PollId, int PollOptionId, string Response, string IPAddress, int UserId);
        public abstract int Poll_HasVoted(int TopicId, int UserId);
        #endregion
        #region Profiles
        public abstract void Profiles_Create(int PortalId, int ModuleId, int UserId);
        public abstract void Profiles_UpdateActivity(int PortalId, int ModuleId, int UserId);
        public abstract IDataReader Profiles_GetUsersOnline(int PortalId, int ModuleId, int Interval);
        public abstract DataSet Profiles_Get(int PortalId, int ModuleId, int UserId);
        public abstract IDataReader Profiles_MemberList(int PortalId, int ModuleId, int MaxRows, int RowIndex, string Filter);
        public abstract void Profiles_Save(int PortalId, int ModuleId, int UserId, int TopicCount, int ReplyCount, int ViewCount, int AnswerCount, int RewardPoints, string UserCaption, string Signature, bool SignatureDisabled, int TrustLevel, bool AdminWatch, bool AttachDisabled, string Avatar, int AvatarType, bool AvatarDisabled, string PrefDefaultSort, bool PrefDefaultShowReplies, bool PrefJumpLastPost, bool PrefTopicSubscribe, int PrefSubscriptionType, bool PrefUseAjax, bool PrefBlockAvatars, bool PrefBlockSignatures, int PrefPageSize, string Yahoo, string MSN, string ICQ, string AOL, string Occupation, string Location, string Interests, string WebSite, string Badges);
        public abstract IDataReader Profiles_GetStats(int PortalId, int ModuleId, int Interval);
        #endregion
        #region Moderation
        public abstract DataSet Mod_Pending(int PortalId, int ModuleId, int ForumId, int UserId);
        public abstract void Mod_Reject(int PortalId, int ModuleId, int UserId, int ForumId, int TopicId, int ReplyId, int Reason, string Comment);

        #endregion
        #region Ranks
        public abstract int Ranks_Save(int PortalId, int ModuleId, int RankId, string RankName, int MinPosts, int MaxPosts, string Display);
        public abstract IDataReader Ranks_Get(int PortalId, int ModuleId, int RankId);
        public abstract IDataReader Ranks_List(int PortalId, int ModuleId);
        public abstract void Ranks_Delete(int PortalId, int ModuleId, int RankId);
        #endregion
        #region Replies/Comments
        public abstract int Reply_Save(int PortalId, int TopicId, int ReplyId, int ReplyToId, int StatusId, bool IsApproved, bool IsDeleted, string Subject, string Body, DateTime DateCreated, DateTime DateUpdated, int AuthorId, string AuthorName, string IPAddress);
        public abstract IDataReader Reply_Get(int PortalId, int ModuleId, int TopicId, int ReplyId);
        public abstract void Reply_UpdateStatus(int PortalId, int ModuleId, int TopicId, int ReplyId, int UserId, int StatusId, bool IsMod);
        public abstract void Reply_Delete(int ForumId, int TopicId, int ReplyId, int DelBehavior);
        #endregion
        
        #region Search
        
        public abstract DataSet Search(int portalId, int moduleId, int userId, int searchId, int rowIndex, int maxRows, string searchString, int matchType, int searchField, int timespan, int authorId, string author, string forums, string tags, int resultType, int sort, int maxCacheHours, bool fullText);
        public abstract int Search_ManageFullText(bool enabled);
        public abstract int Search_GetFullTextStatus();

        public abstract IDataReader Search_DotNetNuke(int moduleId);

        #endregion
        
        #region Security
        public abstract void Security_Delete(int SecuredId, int ObjectId, int SecureAction, int SecureType, int ObjectType);
        public abstract IDataReader Security_Get(int SecuredId, int ObjectId, int SecureType);
        public abstract IDataReader Security_GetByKey(string SecurityKey);
        public abstract IDataReader Security_GetByUser(int PortalId, int ForumId, int UserId, bool IsSuperUser);
        public abstract void Security_Save(int SecuredId, int ObjectId, string SecureAction, bool SecureActionValue, int SecureType, string ObjectName, int ObjectType, string SecurityKey);
        public abstract IDataReader Security_SearchObjects(int PortalId, string Search);
        #endregion
        #region Settings
        public abstract IDataReader Settings_List(int ModuleId, string GroupKey);
        public abstract IDataReader Settings_ListAll(int ModuleId);
        public abstract string Settings_Get(int ModuleId, string GroupKey, string SettingName);
        public abstract void Settings_Delete(int ModuleId, string GroupKey, string SettingName);
        public abstract void Settings_Save(int ModuleId, string GroupKey, string SettingName, string SettingValue);
        #endregion
        #region Subscriptions
        public abstract IDataReader Subscriptions_GetDigest(string SubscriptionType, DateTime StartDate);
        public abstract IDataReader Subscriptions_GetSubscribers(int PortalId, int ForumId, int TopicId, int Mode);
        public abstract int Subscription_Update(int PortalId, int ModuleId, int ForumId, int TopicId, int Mode, int UserId);
        public abstract int Subscriptions_IsSubscribed(int PortalId, int ModuleId, int ForumId, int TopicId, int Mode, int UserId);
        #endregion
        #region Tags
        public abstract void Tags_Delete(int PortalId, int ModuleId, int TagId);
        public abstract void Tags_DeleteByTopicId(int PortalId, int ModuleId, int TopicId);
        public abstract IDataReader Tags_Get(int PortalId, int ModuleId, int TagId);
        public abstract IDataReader Tags_List(int PortalId, int ModuleId, bool IsCategory, int PageIndex, int PageSize, string Sort, string SortColumn, int ForumId, int ForumGroupId);
        public abstract int Tags_Save(int PortalId, int ModuleId, int TagId, string TagName, int Clicks, int Items, int Priority, int TopicId, bool IsCategory, int ForumId, int ForumGroupId);
        public abstract IDataReader Tags_Search(int PortalId, int ModuleId, string Search);
        public abstract void Tags_AddTopicToCategory(int PortalId, int ModuleId, int TagId, int TopicId);
        public abstract void Tags_DeleteTopicToCategory(int PortalId, int ModuleId, int TagId, int TopicId);
        #endregion
        #region Templates
        public abstract void Templates_Delete(int TemplateId, int PortalId, int ModuleId);
        public abstract IDataReader Templates_Get(int TemplateId, int PortalId, int ModuleId);
        public abstract IDataReader Templates_List(int PortalId, int ModuleId, int TemplateType);
        public abstract IDataReader Templates_List(int PortalId, int ModuleId, int TemplateType, int RowIndex, int PageSize);
        public abstract int Templates_Save(int TemplateId, int PortalId, int ModuleId, int TemplateType, bool IsSystem, string Title, string Subject, string Template);
        #endregion
        #region Topics
        public abstract int Topics_AddRating(int TopicId, int UserID, int Rating, string Comments, string IPAddress);
        public abstract void Topics_Delete(int ForumId, int TopicId, int DelBehavior);
        public abstract IDataReader Topics_Get(int PortalId, int ModuleId, int TopicId, int ForumId, int UserId, bool WithSecurity);
        public abstract int Topics_GetRating(int TopicId);
        public abstract IDataReader Topics_List(int ForumId, int PortalId, int ModuleId);
        public abstract void Topics_Move(int PortalId, int ModuleId, int ForumId, int TopicId);
        public abstract IDataReader Topics_Replies(int TopicId);
        public abstract int Topics_Save(int PortalId, int TopicId, int ViewCount, int ReplyCount, bool IsLocked, bool IsPinned, string TopicIcon, int StatusId, bool IsApproved, bool IsDeleted, bool IsAnnounce, bool IsArchived, DateTime AnnounceStart, DateTime AnnounceEnd, string Subject, string Body, string Summary, DateTime DateCreated, DateTime DateUpdated, int AuthorId, string AuthorName, string IPAddress, int TopicType, int TopicPriority, string URL, string TopicData);
        public abstract int Topics_SaveToForum(int ForumId, int TopicId, int LastReplyId);
        public abstract void Replies_Split(int OldTopicId, int NewTopicId, string listreplies, DateTime DateUpdated, int FirstReplyId);
        public abstract void Topics_UpdateStatus(int PortalId, int ModuleId, int TopicId, int ReplyId, int TopicStatusId, int ReplyStatusId, int UserId);
        #endregion

        #region Content

        public abstract int Content_GetID(int topicId, int? replyId);

        #endregion



        #region MailQueue

        public abstract IDataReader Queue_List();
        public abstract void Queue_Delete(int EmailId);
        public abstract void Queue_Add(string EmailFrom, string EmailTo, string EmailSubject, string EmailBody, string EmailBodyPlainText, string EmailCC, string EmailBCC);


        #endregion
        #region Maintenance
        public abstract int Forum_Maintenance(int ForumId, int OlderThanTimeFrame, int LastActivityTimeFrame, int ByUserId, bool WithoutReplies, bool TestRun, int DelBehavior);
        #endregion
        #region Dashboard
        public abstract DataSet Dashboard_Get(int PortalId, int ModuleId);
        #endregion

        #region UI
        public abstract DataSet UI_ForumView(int PortalId, int ModuleId, int UserId, bool IsSuper, string ForumIds);
        public abstract DataSet UI_TopicsView(int PortalId, int ModuleId, int ForumId, int UserId, int PageIndex, int PageSize, bool IsSuper, string SortColumn);
        public abstract DataSet UI_TopicView(int PortalId, int ModuleId, int ForumId, int TopicId, int UserId, int PageIndex, int PageSize, bool IsSuper, string Sort);
        public abstract DataSet UI_NotReadView(int PortalId, int ModuleId, int UserId, int RowIndex, int MaxRows, string Sort, bool IsSuper);
        public abstract DataSet UI_UnansweredView(int PortalId, int ModuleId, int UserId, int RowIndex, int MaxRows, string Sort, bool IsSuper);
        public abstract DataSet UI_MyTopicsView(int PortalId, int ModuleId, int UserId, int RowIndex, int MaxRows, string Sort, bool IsSuper);
        public abstract DataSet UI_ActiveView(int PortalId, int ModuleId, int UserId, int RowIndex, int MaxRows, string Sort, bool IsSuper, int TimeFrame);
        #endregion
        #region Utility Items
        public abstract void Utility_MarkAllRead(int ModuleId, int UserId, int ForumId);
        public abstract int Utility_GetFirstUnRead(int TopicId, int LastReadId);
        #endregion
        #region Top Posts
        public abstract IDataReader PortalForums(int PortalId);
        public abstract IDataReader GetPosts(int PortalId, string Forums, bool TopicsOnly, bool RandomOrder, int Rows, string Tags, int FilterByUserId = -1);
        public abstract IDataReader GetPostsByUser(int PortalId, int Rows, bool IsSuperUser, int currentUserId, int FilteredUserid, bool TopicsOnly, string ForumIds);
        #endregion

    }

}

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

using Microsoft.ApplicationBlocks.Data;
namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class Profiles : DataConfig
	{
		public void Profiles_Create(int PortalId, int ModuleId, int UserId)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, dbPrefix + "UserProfiles_Create", PortalId, -1, UserId);
		}
		public void Profiles_UpdateActivity(int PortalId, int ModuleId, int UserId)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, dbPrefix + "UserProfiles_UpdateActivity", PortalId, ModuleId, UserId);
		}
		public IDataReader Profiles_GetUsersOnline(int PortalId, int ModuleId, int Interval)
		{
			return (IDataReader)(SqlHelper.ExecuteReader(_connectionString, dbPrefix + "UserProfiles_GetUsersOnline", PortalId, ModuleId, Interval));
		}
		public IDataReader Profiles_Get(int PortalId, int ModuleId, int UserId)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "UserProfiles_Get", PortalId, -1, UserId);
		}
		public void Profiles_Save(int PortalId, int ModuleId, int UserId, int TopicCount, int ReplyCount, int ViewCount, int AnswerCount, int RewardPoints, string UserCaption, string Signature, bool SignatureDisabled, int TrustLevel, bool AdminWatch, bool AttachDisabled, string Avatar, int AvatarType, bool AvatarDisabled, string PrefDefaultSort, bool PrefDefaultShowReplies, bool PrefJumpLastPost, bool PrefTopicSubscribe, int PrefSubscriptionType, bool PrefUseAjax, bool PrefBlockAvatars, bool PrefBlockSignatures, int PrefPageSize, string Yahoo, string MSN, string ICQ, string AOL, string Occupation, string Location, string Interests, string WebSite, string Badges)
		{
			SqlHelper.ExecuteNonQuery(_connectionString, dbPrefix + "UserProfiles_Save", PortalId, -1, UserId, TopicCount, ReplyCount, ViewCount, AnswerCount, RewardPoints, UserCaption, Signature, SignatureDisabled, TrustLevel, AdminWatch, AttachDisabled, Avatar, AvatarType, AvatarDisabled, PrefDefaultSort, PrefDefaultShowReplies, PrefJumpLastPost, PrefTopicSubscribe, PrefSubscriptionType, PrefUseAjax, PrefBlockAvatars, PrefBlockSignatures, PrefPageSize, Yahoo, MSN, ICQ, AOL, Occupation, Location, Interests, WebSite, Badges);
		}
		public IDataReader Profiles_GetStats(int PortalId, int ModuleId, int Interval)
		{
			return (IDataReader)(SqlHelper.ExecuteReader(_connectionString, dbPrefix + "UserProfiles_Stats", PortalId, ModuleId, Interval));
		}
		public IDataReader Profiles_MemberList(int PortalId, int ModuleId, int MaxRows, int RowIndex, string Filter)
		{
			//Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "activeforums_UserProfiles_Members", PortalId, MaxRows, RowIndex, Filter), IDataReader)
			return (IDataReader)(SqlHelper.ExecuteReader(_connectionString, dbPrefix + "UserProfiles_List", PortalId, ModuleId, MaxRows, RowIndex, Filter));
		}
		public void Profile_UpdateTopicCount(int PortalId, int UserId)
		{
			string sSql = "UPDATE " + dbPrefix + "UserProfiles SET TopicCount = ISNULL((Select Count(t.TopicId) from ";
			sSql += dbPrefix + "Topics as t INNER JOIN ";
			sSql += dbPrefix + "Content as c ON t.ContentId = c.ContentId AND c.AuthorId = @AuthorId INNER JOIN ";
			sSql += dbPrefix + "ForumTopics as ft ON ft.TopicId = t.TopicId INNER JOIN ";
			sSql += dbPrefix + "Forums as f ON ft.ForumId = f.ForumId ";
			sSql += "WHERE c.AuthorId = @AuthorId AND t.IsApproved = 1 AND t.IsDeleted=0 AND f.PortalId=@PortalId),0) ";
			sSql += "WHERE UserId = @AuthorId AND PortalId = @PortalId";
			sSql = sSql.Replace("@AuthorId", UserId.ToString());
			sSql = sSql.Replace("@PortalId", PortalId.ToString());
			SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, sSql);


		}

	}
}
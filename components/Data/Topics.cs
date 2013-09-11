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
using System.Xml;
namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class Topics : DataConfig
	{
		public int Reply_Save(ReplyInfo ri)
		{
			return Convert.ToInt32(Reply_Save(ri.TopicId, ri.ReplyId, ri.ReplyToId, ri.StatusId, ri.IsApproved, ri.IsDeleted, ri.Content.Subject.Trim(), ri.Content.Body.Trim(), ri.Content.DateCreated, ri.Content.DateUpdated, ri.Content.AuthorId, ri.Content.AuthorName, ri.Content.IPAddress));
		}
		public int Reply_Save(int TopicId, int ReplyId, int ReplyToId, int StatusId, bool IsApproved, bool IsDeleted, string Subject, string Body, DateTime DateCreated, DateTime DateUpdated, int AuthorId, string AuthorName, string IPAddress)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "Reply_Save", TopicId, ReplyId, ReplyToId, StatusId, IsApproved, IsDeleted, Subject, Body, DateCreated, DateUpdated, AuthorId, AuthorName, IPAddress));
		}
		public int TopicIdByUrl(int PortalId, int ModuleId, string URL)
		{
			if (URL.EndsWith("/"))
			{
				URL = URL.Substring(0, URL.Length - 1);
			}
			return Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "TopicIdByURL", PortalId, ModuleId, URL));
		}
		public int Topics_AddRating(int TopicId, int UserID, int Rating, string Comments, string IPAddress)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "Topics_AddRating", TopicId, UserID, Rating, Comments, IPAddress));
		}
		public IDataReader TopicForDisplay(int SiteId, int InstanceId, int ForumId, int TopicId, int UserId, int RowIndex, int MaxRows, string Sort)
		{
			IDataReader dr = SqlHelper.ExecuteReader(_connectionString, dbPrefix + "UI_TopicDisplay", SiteId, InstanceId, ForumId, TopicId, UserId, RowIndex, MaxRows, false, Sort);
			return dr;
		}
		public IDataReader TopicsList(int PortalId, int PageIndex, int PageSize, string ForumIds, int CategoryId, int TagId)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "TopicsList", PortalId, PageIndex, PageSize, ForumIds, CategoryId, TagId);
		}
		public IDataReader TopicWithReplies(int PortalId, int TopicId, int PageIndex, int PageSize)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "TopicWithReplies", PortalId, TopicId, PageIndex, PageSize);
		}
		//Public Function TopicsForDisplayXML(ByVal SiteId As Integer, ByVal InstanceId As Integer, ByVal ForumId As Integer, ByVal UserId As Integer, ByVal PageIndex As Integer, ByVal PageSize As Integer, ByVal IsSuper As Boolean, ByVal SortColumn As String, ByVal ForumIds As String) As XmlDocument
		//    Dim dr As IDataReader = SqlHelper.ExecuteReader(_connectionString, dbPrefix & "UI_TopicsDisplay", SiteId, InstanceId, ForumId, UserId, PageIndex, PageSize, IsSuper, SortColumn, ForumIds)
		//    Dim sb As New Text.StringBuilder
		//    sb.Append("<?xml version=""1.0"" encoding=""utf-8"" ?>")
		//    sb.Append("<root><topics>")
		//    While dr.Read()
		//        sb.Append("<topic forumid=""" & dr("ForumId").ToString & """ ")
		//        sb.Append("topicid=""" & dr("TopicId").ToString & """ ")
		//        sb.Append("lastreplyid=""" & dr("lastreplyid").ToString & """ ")
		//        sb.Append("viewcount=""" & dr("viewcount").ToString & """ ")
		//        sb.Append("replycount=""" & dr("replycount").ToString & """ ")
		//        sb.Append("islocked=""" & dr("islocked").ToString & """ ")
		//        sb.Append("ispinned=""" & dr("ispinned").ToString & """ ")
		//        sb.Append("topicicon=""" & dr("topicicon").ToString & """ ")
		//        sb.Append("statusid=""" & dr("statusid").ToString & """ ")
		//        sb.Append("isannounce=""" & dr("isannounce").ToString & """ ")
		//        sb.Append("announcestart=""" & dr("announcestart").ToString & """ ")
		//        sb.Append("announceend=""" & dr("announceend").ToString & """ ")
		//        sb.Append("topictype=""" & dr("TopicType").ToString & """ ")
		//        sb.Append("authorid=""" & dr("authorid").ToString & """ ")
		//        sb.Append("datecreated=""" & dr("datecreated").ToString & """ ")
		//        sb.Append("lastpostdate=""" & dr("lastpostdate").ToString & """ ")
		//        sb.Append("userlastreplyread=""" & dr("userlastreplyread").ToString & """ ")
		//        sb.Append("userlasttopicread=""" & dr("userlasttopicread").ToString & """ ")
		//        sb.Append("topicrating=""" & dr("topicrating").ToString & """>")
		//        sb.Append("<subject><![CDATA[" & dr("subject").ToString & "]]></subject>")
		//        sb.Append("<summary><![CDATA[" & dr("summary").ToString & "]]></summary>")
		//        sb.Append("<body><![CDATA[" & dr("body").ToString & "]]></body>")
		//        sb.Append("<authorname><![CDATA[" & dr("authorname").ToString & "]]></authorname>")
		//        sb.Append("<username><![CDATA[" & dr("username").ToString & "]]></username>")
		//        sb.Append("<firstname><![CDATA[" & dr("firstname").ToString & "]]></firstname>")
		//        sb.Append("<lastname><![CDATA[" & dr("lastname").ToString & "]]></lastname>")
		//        sb.Append("<displayname><![CDATA[" & dr("displayname").ToString & "]]></displayname>")
		//        sb.Append("<forumname><![CDATA[" & dr("forumname").ToString & "]]></forumname>")
		//        sb.Append("<groupname><![CDATA[" & dr("groupname").ToString & "]]></groupname>")
		//        sb.Append("<security>")
		//        sb.Append("<view>" & dr("CanView").ToString & "</view>")
		//        sb.Append("<read>" & dr("CanRead").ToString & "</read>")
		//        sb.Append("<create>" & dr("CanCreate").ToString & "</create>")
		//        sb.Append("<reply>" & dr("CanEdit").ToString & "</reply>")
		//        sb.Append("<edit>" & dr("CanEdit").ToString & "</edit>")
		//        sb.Append("<delete>" & dr("CanDelete").ToString & "</delete>")
		//        sb.Append("<lock>" & dr("CanLock").ToString & "</lock>")
		//        sb.Append("<pin>" & dr("CanPin").ToString & "</pin>")
		//        sb.Append("<modapprove>" & dr("Canmodapprove").ToString & "</modapprove>")
		//        sb.Append("<modedit>" & dr("canmodedit").ToString & "</modedit>")
		//        sb.Append("<moddelete>" & dr("canmoddelete").ToString & "</moddelete>")
		//        sb.Append("<modlock>" & dr("canmodlock").ToString & "</modlock>")
		//        sb.Append("<modpin>" & dr("canmodpin").ToString & "</modpin>")
		//        sb.Append("<modmove>" & dr("canmodmove").ToString & "</modmove>")
		//        sb.Append("</security>")
		//        sb.Append(dr("lastpostdata").ToString)
		//        sb.Append("</topic>")


		//    End While
		//    sb.Append("</topics></root>")
		//    dr.Close()
		//    Dim xDoc As New XmlDocument
		//    xDoc.LoadXml(sb.ToString)
		//    Return xDoc
		//End Function
	}
}

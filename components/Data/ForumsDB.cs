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
using System.Web;

namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class ForumsDB : DataConfig
	{
		public int Forum_GetByTopicId(int TopicId)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, dbPrefix + "ForumGetByTopicId", TopicId));
		}
		public IDataReader Forums_GetForSocialGroup(int PortalId, int SocialGroupId)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "GetForumsForSocialGroup", SocialGroupId);
		}
		//Public Function Forums_List(ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader
		//    Return SqlHelper.ExecuteReader(_connectionString, dbPrefix & "Forums_GetPermissions", PortalId, ModuleId)
		//End Function
		public IDataReader Forums_Get(int PortalId, int ModuleId, int ForumId)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "ForumGet", PortalId, ModuleId, ForumId);
		}
		public IDataReader Forums_List(int PortalId, int ModuleId, int ForumGroupId, int ParentForumId, bool FillLastPost)
		{
			return (IDataReader)(SqlHelper.ExecuteReader(_connectionString, dbPrefix + "Forums_List", ModuleId, ForumGroupId, ParentForumId, FillLastPost));
		}
		public ForumCollection Forums_List(int PortalId, int ModuleId)
		{
			ForumCollection f = new ForumCollection();
			object obj = DataCache.CacheRetrieve(string.Format(CacheKeys.ForumList, ModuleId));
			if (obj != null)
			{
				f = (ForumCollection)obj;
			}
			else
			{
				using (IDataReader dr = SqlHelper.ExecuteReader(_connectionString, dbPrefix + "ForumsList", PortalId, ModuleId))
				{

					Forum fi = null;
					ForumGroupInfo gi = null;
					while (dr.Read())
					{
						fi = new Forum();
						gi = new ForumGroupInfo();
						fi.ModuleId = int.Parse(dr["ModuleId"].ToString());
						fi.ForumID = Convert.ToInt32(dr["ForumId"].ToString());
						fi.Active = bool.Parse(dr["Active"].ToString());
						fi.ForumDesc = dr["ForumDesc"].ToString();
						fi.ForumGroupId = int.Parse(dr["ForumGroupId"].ToString());
						fi.ForumID = int.Parse(dr["ForumId"].ToString());
						fi.ForumName = dr["ForumName"].ToString();
						fi.GroupName = dr["GroupName"].ToString();
						fi.Hidden = bool.Parse(dr["Hidden"].ToString());
						fi.ParentForumId = Convert.ToInt32(dr["ParentForumId"].ToString());
					    DateTime postTime;
						if (! (DateTime.TryParse(dr["LastPostDate"].ToString(), out postTime)))
						{
							fi.LastPostDateTime = new DateTime();
						}
						else
						{
						    fi.LastPostDateTime = postTime;
						}

						fi.LastTopicId = int.Parse(dr["LastTopicId"].ToString());
						fi.LastReplyId = int.Parse(dr["LastReplyId"].ToString());
						fi.LastPostSubject = dr["LastPostSubject"].ToString();
						fi.LastPostDisplayName = dr["LastPostAuthorName"].ToString();
						fi.LastPostUserID = int.Parse(dr["LastPostAuthorId"].ToString());
						fi.LastPostUserName = fi.LastPostDisplayName;
						fi.LastRead = DateTime.Parse(dr["LastRead"].ToString());
						gi.Active = bool.Parse(dr["GroupActive"].ToString());
						gi.Hidden = bool.Parse(dr["GroupHidden"].ToString());
						gi.GroupName = fi.GroupName;
						gi.ForumGroupId = fi.ForumGroupId;
						gi.PrefixURL = dr["GroupPrefixURL"].ToString();
							//gi.SEO = dr("GroupSEO").ToString
						fi.ForumGroup = gi;
						fi.Security.Announce = dr["CanAnnounce"].ToString();
						fi.Security.Attach = dr["CanAttach"].ToString();
						fi.Security.Create = dr["CanCreate"].ToString();
						fi.Security.Delete = dr["CanDelete"].ToString();
						fi.Security.Edit = dr["CanEdit"].ToString();
						fi.Security.Lock = dr["CanLock"].ToString();
						fi.Security.ModApprove = dr["CanModApprove"].ToString();
						fi.Security.ModDelete = dr["CanModDelete"].ToString();
						fi.Security.ModEdit = dr["CanModEdit"].ToString();
						fi.Security.ModLock = dr["CanModLock"].ToString();
						fi.Security.ModMove = dr["CanModMove"].ToString();
						fi.Security.ModPin = dr["CanModPin"].ToString();
						fi.Security.ModSplit = dr["CanModSplit"].ToString();
						fi.Security.ModUser = dr["CanModUser"].ToString();
						fi.Security.Pin = dr["CanPin"].ToString();
						fi.Security.Poll = dr["CanPoll"].ToString();
						fi.Security.Block = dr["CanBlock"].ToString();
						fi.Security.Read = dr["CanRead"].ToString();
						fi.Security.Reply = dr["CanReply"].ToString();
						fi.Security.Subscribe = dr["CanSubscribe"].ToString();
						fi.Security.Trust = dr["CanTrust"].ToString();
						fi.Security.View = dr["CanView"].ToString();
						fi.ForumSettings = LoadSettings(dr);
						fi.PrefixURL = dr["PrefixURL"].ToString();
							//.SEO = dr("ForumSEO").ToString
						fi.TotalTopics = int.Parse(dr["TotalTopics"].ToString());
						fi.TotalReplies = int.Parse(dr["TotalReplies"].ToString());
						f.Add(fi);
					}
					dr.Close();
				}

				DataCache.CacheStore(string.Format(CacheKeys.ForumList, ModuleId), f);
			}
			return f;

		}
		private Hashtable LoadSettings(IDataRecord dr)
		{
			Hashtable ht = new Hashtable();
            string snames = "ALLOWATTACH,ALLOWEMOTICONS,ALLOWHTML,ALLOWPOSTICON,ALLOWRSS,ALLOWSCRIPT,ALLOWTAGS,ATTACHCOUNT,ATTACHMAXHEIGHT,ATTACHMAXSIZE,ATTACHMAXWIDTH,ATTACHSTORE,ATTACHTYPEALLOWED,ATTACHUNIQUEFILENAMES,AUTOSUBSCRIBEENABLED,AUTOSUBSCRIBENEWTOPICSONLY,AUTOSUBSCRIBEROLES,ATTACHINSERTALLOWED,MAXATTACHWIDTH,MAXATTACHHEIGHT,CONVERTINGTOJPEGALLOWED,AUTOTRUSTLEVEL,DEFAULTTRUSTLEVEL,EDITORHEIGHT,EDITORPERMITTEDUSERS,EDITORSTYLE,EDITORTOOLBAR,EDITORTYPE,EDITORWIDTH,EMAILADDRESS,INDEXCONTENT,ISMODERATED,MCADDRESS,MCADMINNOTIFY,MCAUTOCREATEUSERS,MCAUTORESPONSE,MCENABLED,MCEOMTAG,MCEOMTAGREQ,MCMODTYPE,MCPOPPASSWORD,MCPOPSERVER,MCPOPUSERNAME,MCREJECTNOTIFY,MCRESTRICTALIAS,MCSTRIPHTML,MCSUBNOTIFY,MCURL,MODAPPROVETEMPLATEID,MODDELETETEMPLATEID,MODMOVETEMPLATEID,MODNOTIFYTEMPLATEID,MODREJECTTEMPLATEID,PROFILETEMPLATEID,QUICKREPLYFORMID,REPLYFORMID,TOPICFORMID,TOPICSTEMPLATEID,TOPICTEMPLATEID,USEFILTER";
			string[] SettingKeys = snames.Split(',');
			foreach (string s in SettingKeys)
			{
				if (dr.GetOrdinal(s) != null)
				{
					ht.Add(s, dr[s]);
				}
			}
			return ht;
		}
		public XmlDocument ForumListXML(int SiteId, int InstanceId)
		{
			XmlDocument xDoc = new XmlDocument();
			object obj = DataCache.CacheRetrieve(InstanceId + "fv");
			if (obj == null)
			{
				Data.ForumsDB db = new Data.ForumsDB();
				ForumCollection fc = db.Forums_List(SiteId, InstanceId);
				//Dim ds As DataSet = SqlHelper.ExecuteDataset(connectionString, databaseOwner & objectQualifier & "activeforums_UI_ForumDisplay", SiteId, InstanceId, UserId, -1, ForumIds)
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
				sb.AppendLine();
				sb.Append("<root>");
				sb.AppendLine();
				int groupId = -1;
				System.Text.StringBuilder groups = new System.Text.StringBuilder();
				System.Text.StringBuilder forums = new System.Text.StringBuilder();
				foreach (Forum f in fc)
				{
					if (groupId != f.ForumGroupId)
					{
						groups.Append("<group groupid=\"" + f.ForumGroupId.ToString() + "\" active=\"" + f.ForumGroup.Active.ToString().ToLowerInvariant() + "\" hidden=\"" + f.ForumGroup.Hidden.ToString().ToLowerInvariant() + "\">");
						groups.Append("<name><![CDATA[" + f.GroupName.ToString() + "]]></name>");
						//If Not String.IsNullOrEmpty(f.ForumGroup.SEO) Then
						//    groups.Append(f.ForumGroup.SEO)
						//End If
						groups.Append("</group>");
						sb.AppendLine();
						groupId = f.ForumGroupId;
					}
				}
				sb.Append("<groups>");
				sb.AppendLine();
				sb.Append(groups.ToString());
				sb.Append("</groups>");
				sb.AppendLine();
				foreach (Forum f in fc)
				{
					forums.Append("<forum groupid=\"" + f.ForumGroupId.ToString() + "\" forumid=\"" + f.ForumID.ToString() + "\"");
					//forums.Append(" name=""" & HttpUtility.UrlEncode(f.ForumName) & """")
					//forums.Append(" desc=""" & HttpUtility.UrlEncode(Utilities.HTMLEncode(f.ForumDesc.ToString)) & """")
					forums.Append(" active=\"" + f.Active.ToString().ToLowerInvariant() + "\"");
					forums.Append(" hidden=\"" + f.Hidden.ToString().ToLowerInvariant() + "\"");
					forums.Append(" totaltopics=\"" + f.TotalTopics.ToString() + "\"");
					forums.Append(" totalreplies=\"" + f.TotalReplies.ToString() + "\"");
					forums.Append(" lasttopicid=\"" + f.LastTopicId.ToString() + "\"");
					forums.Append(" lastreplyid=\"" + f.LastReplyId.ToString() + "\"");
					//forums.Append(" lastpostsubject=""" & f.LastPostSubject & """")
					//forums.Append(" lastpostauthorname=""" & f.LastPostDisplayName & """")
					forums.Append(" lastpostauthorid=\"" + f.LastPostUserID + "\"");
					forums.Append(" lastpostdate=\"" + f.LastPostDateTime.ToString() + "\"");
					forums.Append(" lastread=\"" + f.LastRead.ToString() + "\"");
					forums.Append(" allowrss=\"" + f.ForumSettings["ALLOWRSS"].ToString() + "\"");
					forums.Append(" parentforumid=\"" + f.ParentForumId.ToString() + "\"");
					forums.Append(" viewroles=\"" + f.Security.View.ToString() + "\"");
					forums.Append(" readroles=\"" + f.Security.Read.ToString() + "\"");
					forums.Append(" replyroles=\"" + f.Security.Reply.ToString() + "\"");
					forums.Append(" modroles=\"" + f.Security.ModApprove.ToString() + "\"");
					forums.Append(" modmove=\"" + f.Security.ModMove.ToString() + "\"");
					forums.Append(">");
					forums.Append("<name><![CDATA[" + f.ForumName + "]]></name>");
					forums.Append("<description><![CDATA[" + f.ForumDesc + "]]></description>");
					forums.Append("<security>");
					forums.Append("<view>" + f.Security.View + "</view>");
					forums.Append("<read>" + f.Security.Read + "</read>");
					forums.Append("<create>" + f.Security.Create + "</create>");
					forums.Append("<reply>" + f.Security.Reply + "</reply>");
					forums.Append("<edit>" + f.Security.Edit + "</edit>");
					forums.Append("<delete>" + f.Security.Delete + "</delete>");
					forums.Append("<lock>" + f.Security.Lock + "</lock>");
					forums.Append("<pin>" + f.Security.Pin + "</pin>");
					forums.Append("<modapprove>" + f.Security.ModApprove + "</modapprove>");
					forums.Append("<modedit>" + f.Security.ModEdit + "</modedit>");
					forums.Append("<moddelete>" + f.Security.ModDelete + "</moddelete>");
					forums.Append("<modlock>" + f.Security.ModLock + "</modlock>");
					forums.Append("<modpin>" + f.Security.ModPin + "</modpin>");
					forums.Append("<modmove>" + f.Security.ModMove + "</modmove>");
					forums.Append("</security>");
					//If Not String.IsNullOrEmpty(f.SEO) Then
					//    forums.Append(f.SEO)
					//End If

					forums.Append("</forum>");
					sb.AppendLine();
				}
				sb.Append("<forums>");
				sb.AppendLine();
				sb.Append(forums.ToString());
				sb.Append("</forums>");
				sb.AppendLine();
				sb.Append("<topics />");
				sb.AppendLine();
				sb.Append("<replies />");
				sb.AppendLine();
				sb.Append("</root>");
				sb.AppendLine();


				//Dim sXML As String = ds.GetXml()
				xDoc.LoadXml(sb.ToString());
				DataCache.CacheStore(InstanceId + "fv", xDoc);
			}
			else
			{
				xDoc = (XmlDocument)obj;
			}
			//Logger.Log(xDoc.OuterXml)
			return xDoc;
		}
	}
}

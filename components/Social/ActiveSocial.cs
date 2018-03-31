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

using System.Web;
using DotNetNuke.Modules.ActiveForums.Data;
using Microsoft.ApplicationBlocks.Data;
using DotNetNuke.Services.Journal;
namespace DotNetNuke.Modules.ActiveForums
{
    public class Social : DataConfig
    {
        public int GetActiveSocialStatus(int PortalId)
        {
#if SKU_LITE
			//Not Available
			return -2;
#else
            //TODO: the following can probably be removed? Not sure on AS support anymore
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
            {
                var mc = new Entities.Modules.ModuleController();
                var tc = new Entities.Tabs.TabController();
                Entities.Tabs.TabInfo ti;
                foreach (Entities.Modules.ModuleInfo mi in mc.GetModules(PortalId))
                {
                    if (mi.DesktopModule.ModuleName.Contains("Active Social") && mi.IsDeleted == false)
                    {
                        ti = tc.GetTab(mi.TabID, PortalId, false);
                        if (ti != null)
                        {
                            if (ti.IsDeleted == false)
                            {
                                //Installed and ready
                                return 1;
                            }
                        }
                    }
                }
                //Not installed on portal
                return 0;
            }
            //Not Installed
            return -1;
#endif
        }
        public void AddTopicToJournal(int PortalId, int ModuleId, int ForumId, int TopicId, int UserId, string URL, string Subject, string Summary, string Body, int SecurityOption, string ReadRoles, int SocialGroupId)
        {

            var ji = new JournalItem
                         {
                             PortalId = PortalId,
                             ProfileId = UserId,
                             UserId = UserId,
                             Title = Subject,
                             ItemData = new ItemData { Url = URL }
                         };
            if (string.IsNullOrEmpty(Summary))
            {
                Summary = Utilities.StripHTMLTag(Body);
                if (Summary.Length > 150)
                {
                    Summary = Summary.Substring(0, 150) + "...";
                }
            }
            ji.Summary = Summary;
            ji.Body = Utilities.StripHTMLTag(Body);
            ji.JournalTypeId = 5;
            ji.ObjectKey = string.Format("{0}:{1}", ForumId.ToString(), TopicId.ToString());
            if (JournalController.Instance.GetJournalItemByKey(PortalId, ji.ObjectKey) != null)
            {
                JournalController.Instance.DeleteJournalItemByKey(PortalId, ji.ObjectKey);
            }
            string roles = string.Empty;
            if (!(string.IsNullOrEmpty(ReadRoles)))
            {
                if (ReadRoles.Contains("|"))
                {
                    roles = ReadRoles.Substring(0, ReadRoles.IndexOf("|", StringComparison.Ordinal) - 1);
                }
            }

            foreach (string s in roles.Split(';'))
            {
                if ((s == "-1") | (s == "-3"))
                {
                    /* cjh - securityset was null and throwing an error, thus journal items weren't added */
                    if ((ji.SecuritySet != null) && !(ji.SecuritySet.Contains("E,")))
                    {
                        ji.SecuritySet += "E,";
                    }
                }
                else
                {
                    ji.SecuritySet += "R" + s + ",";
                }

            }
            if (SocialGroupId > 0)
            {

                ji.SocialGroupId = SocialGroupId;

            }
            JournalController.Instance.SaveJournalItem(ji, -1);


           
        }
        public void AddReplyToJournal(int PortalId, int ModuleId, int ForumId, int TopicId, int ReplyId, int UserId, string URL, string Subject, string Summary, string Body, int SecurityOption, string ReadRoles, int SocialGroupId)
        {
            //make sure that this is a User before trying to create a journal item, you can't post a JI without
            if (UserId > 0)
            {
                var ji = new JournalItem
                             {
                                 PortalId = PortalId,
                                 ProfileId = UserId,
                                 UserId = UserId,
                                 Title = Subject,
                                 ItemData = new ItemData { Url = URL }
                             };
                if (string.IsNullOrEmpty(Summary))
                {
                    Summary = Utilities.StripHTMLTag(Body);
                    if (Summary.Length > 150)
                    {
                        Summary = Summary.Substring(0, 150) + "...";
                    }
                }
                ji.Summary = Summary;
                ji.Body = Utilities.StripHTMLTag(Body);
                ji.JournalTypeId = 6;
                ji.ObjectKey = string.Format("{0}:{1}:{2}", ForumId.ToString(), TopicId.ToString(), ReplyId.ToString());
                if (JournalController.Instance.GetJournalItemByKey(PortalId, ji.ObjectKey) != null)
                {
                    JournalController.Instance.DeleteJournalItemByKey(PortalId, ji.ObjectKey);
                }
                string roles = string.Empty;
                if (!(string.IsNullOrEmpty(ReadRoles)))
                {
                    if (ReadRoles.Contains("|"))
                    {
                        roles = ReadRoles.Substring(0, ReadRoles.IndexOf("|", StringComparison.Ordinal) - 1);
                    }
                }

                foreach (string s in roles.Split(';'))
                {
                    if ((s == "-1") | (s == "-3"))
                    {
                        /* cjh - securityset was null and throwing an error, thus journal items weren't added */
                        if ((ji.SecuritySet != null) && (!(ji.SecuritySet.Contains("E,"))))
                        {
                            ji.SecuritySet += "E,";
                        }
                    }
                    else
                    {
                        ji.SecuritySet += "R" + s + ",";
                    }
                }
                if (SocialGroupId > 0)
                {
                    ji.SocialGroupId = SocialGroupId;
                }
                JournalController.Instance.SaveJournalItem(ji, -1);
            }
        }
        public void AddForumItemToJournal(int PortalId, int ModuleId, int UserId, string entryType, string sURL, string sSubject, string sBody)
        {
            try
            {

                //todo: we should probably have something here

            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
            }
        }
        internal IDataReader ActiveSocialListGroups(int PortalId)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
            {
                string sSQL = "Select g.GroupId, g.GroupName from " + _databaseOwner + _objectQualifier + "activesocial_Groups as g WHERE PortalId = " + PortalId.ToString() + " AND StatusId = 1";
                return SqlHelper.ExecuteReader(_connectionString, CommandType.Text, sSQL);
            }
            return null;
        }

        internal string ActiveSocialGroups(int userId, int PortalId)
        {

            string sGroups = string.Empty;
            try
            {
                //Dim MainSettings As SettingsInfo = DataCache.MainSettings(ModuleId)
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
                {
                    DataSet ds = SqlHelper.ExecuteDataset(_connectionString, _databaseOwner + _objectQualifier + "activesocial_Groups_ListMyGroups", PortalId, 0, 1000, userId, 1, userId);
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                sGroups += dr["GroupId"] + ":" + dr["GroupMemberTypeId"] + ";";
                            }
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
            }
            return sGroups;

        }
        internal string ActiveSocialGetGroup(int groupId)
        {

            string sGroups = string.Empty;
            try
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
                {
                    sGroups = Convert.ToString(SqlHelper.ExecuteScalar(_connectionString, _databaseOwner + _objectQualifier + "activesocial_Groups_GetName", groupId));
                    return sGroups;
                }
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
            }
            return sGroups;
        }
        internal string ActiveSocialGetGroupType(int groupTypeId, int PortalId)
        {

            string sGroups = string.Empty;
            try
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
                {
                    using (IDataReader dr = SqlHelper.ExecuteReader(_connectionString, _databaseOwner + _objectQualifier + "activesocial_Groups_GetMemberTypes", PortalId))
                    {
                        while (dr.Read())
                        {
                            if (Convert.ToInt32(dr["GroupMemberTypeId"].ToString()) == groupTypeId)
                            {
                                sGroups = dr["MemberTypeName"].ToString();

                            }
                        }
                        dr.Close();
                    }
                    return sGroups;
                }
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
                //Return ex.Message
            }
            return sGroups;
        }
        internal int GetMemberTabId(int PortalId)
        {

            try
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
                {
                    string sSQL = "SELECT SettingValue FROM " + _databaseOwner + _objectQualifier + "activesocial_Settings WHERE SettingName='MEMBERSTABID' AND PortalId = " + PortalId;
                    return Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, CommandType.Text, sSQL));
                }
                return -1;
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
            }
            return -1;

        }
        internal int GetMasterForumGroup(int PortalId, int CurrentTabId)
        {
            int ForumGroupId = -1;
            int GroupViewTabId = -1;

            try
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
                {
                    string sSQL = "SELECT SettingValue FROM " + _databaseOwner + _objectQualifier + "activesocial_Settings WHERE SettingName='GLOBALGROUPFORUMTEMPLATEGROUP' AND PortalId = " + PortalId;
                    ForumGroupId = Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, CommandType.Text, sSQL));
                    sSQL = "SELECT SettingValue FROM " + _databaseOwner + _objectQualifier + "activesocial_Settings WHERE SettingName='GROUPSVIEWTABID' AND PortalId = " + PortalId;
                    GroupViewTabId = Convert.ToInt32(SqlHelper.ExecuteScalar(_connectionString, CommandType.Text, sSQL));
                    if (GroupViewTabId == CurrentTabId)
                    {
                        ForumGroupId = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
            }
            return ForumGroupId;
        }
    }
}

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
using System.Data;

using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class DataCache
	{
		public static bool _disableCache = false;
		public static bool disableCache
		{
			get
			{
				return _disableCache;
			}
			set
			{
				_disableCache = value;
			}
		}
		public static bool CacheStore(string cacheKey, object cacheObj)
		{
			return CacheStore(cacheKey, cacheObj, DateTime.Now.AddMinutes(10));
		}
		public static bool CacheStore(string cacheKey, object cacheObj, DateTime Expiration)
		{
			try
			{
				Common.Utilities.DataCache.SetCache(cacheKey, cacheObj, Expiration);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		public static object CacheRetrieve(string cacheKey)
		{
			object obj = Common.Utilities.DataCache.GetCache(cacheKey);
			return obj;
		}
		public static bool CacheClear(string cacheKey)
		{
			try
			{
				Common.Utilities.DataCache.RemoveCache(cacheKey);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		// KR - remove all cache starting with the given string
		public static bool CacheClearPrefix(string cacheKeyPrefix)
		{
			try
			{
				Common.Utilities.DataCache.ClearCache(cacheKeyPrefix);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public static SettingsInfo MainSettings(int MID)
		{
			object obj = CacheRetrieve(string.Format(CacheKeys.MainSettings, MID));
			if (obj == null || disableCache)
			{
				var objSettings = new SettingsInfo();
				var sb = new SettingsBase {ForumModuleId = MID};
			    obj = sb.MainSettings;
				if (disableCache == false)
				{
					CacheStore(string.Format(CacheKeys.MainSettings, MID), obj);
				}
			}

			return (SettingsInfo)obj;
		}
		public static void ClearAllCache(int ModuleId, int TabId)
		{
			try
			{
				ClearAllForumSettingsCache(ModuleId);
				ClearSettingsCache(ModuleId);
				ClearTemplateCache(ModuleId);
				CacheClear(ModuleId + "fv");
				CacheClear(ModuleId + "ForumStatTable");
				CacheClear(ModuleId + "ForumStatsOutput");
				CacheClear(ModuleId + TabId + "ForumTemplate");
			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
			}

		}
		public static void ClearSettingsCache(int ModuleID)
		{
			try
			{
				object obj = CacheRetrieve(string.Format(CacheKeys.MainSettings, ModuleID));
				if (obj != null)
				{
					CacheClear(string.Format(CacheKeys.MainSettings, ModuleID));
				}
			}
			catch (Exception ex)
			{

			}

		}
		public static void ClearForumsByGroupCache(int ModuleID, int GroupID)
		{
			object obj = CacheRetrieve(ModuleID + GroupID + "ForumsByGroup");
			if (obj != null)
			{
				CacheClear(ModuleID + GroupID + "ForumsByGroup");
			}
		}
		public static void ClearForumGroupsCache(int ModuleID)
		{
			CacheClear(ModuleID + "ForumGroups");
			IDataReader rd;
			rd = DataProvider.Instance().Groups_List(ModuleID);
			while (rd.Read())
			{
				ClearForumsByGroupCache(ModuleID, Convert.ToInt32(rd["ForumGroupID"]));
			}
			rd.Close();
		}
		public static void ClearForumSettingsCache(int ForumID)
		{
			CacheClear(ForumID + "ForumSettings");
			CacheClear(string.Format(CacheKeys.ForumInfo, ForumID));
			CacheClear(string.Format(CacheKeys.ForumInfo, ForumID) + "st");

		}
		public static void ClearAllForumSettingsCache(int ModuleID)
		{
			try
			{
				IDataReader rd;
				rd = DataProvider.Instance().Forums_List(-1, ModuleID, -1, -1, false);
				while (rd.Read())
				{
					int intForumID;
					intForumID = Convert.ToInt32(rd["ForumID"]);
					int TopicsTemplateId;
					int TopicTemplateId;
					TopicsTemplateId = Convert.ToInt32(rd["TopicsTemplateId"]);
					TopicTemplateId = Convert.ToInt32(rd["TopicTemplateId"]);
					CacheClear(intForumID + "ForumSettings");
					CacheClear(ModuleID + TopicsTemplateId + "TopicsTemplate");
					CacheClear(ModuleID + TopicTemplateId + "TopicTemplate");
					CacheClear(string.Format(CacheKeys.ForumInfo, intForumID));
					CacheClear(string.Format(CacheKeys.ForumInfo, intForumID) + "st");
				}
				rd.Close();
			}
			catch (Exception ex)
			{

			}

		}
		public static void ClearFilterCache(int ModuleID)
		{
			object obj = CacheRetrieve(ModuleID + "FilterList");
			if (obj != null)
			{
				//Current.Cache.Remove(ModuleID & "FilterList")
				CacheClear(ModuleID + "FilterList");
			}
		}
		public static void ClearTemplateCache(int ModuleId)
		{
			try
			{
				if (System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/DesktopModules/ActiveForums/cache")))
				{
					var di = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("~/DesktopModules/ActiveForums/cache"));
					foreach (System.IO.FileInfo fi in di.GetFiles())
					{
						if ((fi.FullName.IndexOf(ModuleId + "_", 0) + 1) > 0)
						{
							fi.Delete();
						}

					}
				}
			}
			catch (Exception ex)
			{
				//DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
			}
		}
		public static Hashtable GetSettings(int ModuleId, string SettingsKey, string CacheKey, bool UseCache)
		{
			var ht = new Hashtable();
			if (UseCache)
			{
				object obj = CacheRetrieve(CacheKey + "st");
				if (obj == null)
				{
					IDataReader dr = DataProvider.Instance().Settings_List(ModuleId, SettingsKey);
					while (dr.Read())
					{
						if (! (ht.ContainsKey(dr["SettingName"].ToString())))
						{
							ht.Add(dr["SettingName"].ToString(), string.Empty);
						}
						ht[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
					}
					dr.Close();
					CacheStore(CacheKey + "st", ht);
					//Current.Cache.Insert(ModuleId & SettingsKey & "Settings", ht, Nothing, DateTime.Now.AddMinutes(10), Web.Caching.Cache.NoSlidingExpiration)
				}
				else
				{
					ht = (Hashtable)obj;
				}
			}
			else
			{
				IDataReader dr = DataProvider.Instance().Settings_List(ModuleId, SettingsKey);
				while (dr.Read())
				{
					if (! (ht.ContainsKey(dr["SettingName"].ToString())))
					{
						ht.Add(dr["SettingName"].ToString(), string.Empty);
					}
					ht[dr["SettingName"].ToString()] = dr["SettingValue"].ToString();
				}
				dr.Close();
			}

			return ht;
		}

#region Cache Storage
		private static void CacheTemplateToDisk(int ModuleId, int TemplateId, string TemplateType, string Template)
		{
			string myFile;
			string FileName = ModuleId + "_" + TemplateId + TemplateType + ".resources";
			string strPath;
			strPath = HttpContext.Current.Request.MapPath(Common.Globals.ApplicationPath) + "\\DesktopModules\\ActiveForums\\cache\\";
			if (! (System.IO.Directory.Exists(strPath)))
			{
				try
				{
					System.IO.Directory.CreateDirectory(strPath);
				}
				catch (Exception ex)
				{
					//   DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
					return;
				}

			}
			try
			{
				myFile = HttpContext.Current.Request.MapPath(Common.Globals.ApplicationPath) + "\\DesktopModules\\ActiveForums\\cache\\" + FileName;
				if (System.IO.File.Exists(myFile))
				{
					try
					{
						System.IO.File.Delete(myFile);
					}
					catch (Exception ex)
					{
						Services.Exceptions.Exceptions.LogException(ex);
					}
				}
				try
				{
					System.IO.File.AppendAllText(myFile, Template);
				}
				catch (Exception ex)
				{
					// DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
				}
			}
			catch (Exception ex)
			{
				//DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
			}
		}
#endregion

#region Cache Retrieval
		public static string GetCachedTemplate(int TemplateStore, int ModuleId, string TemplateType, int TemplateId)
		{
			string sTemplate;
			switch (TemplateStore)
			{
				case 0:
					//Get From Memory
					sTemplate = GetTemplateFromMemory(ModuleId, TemplateType, TemplateId);
					break;
				case 1:
					//Get From Disk
					sTemplate = GetTemplateFromDisk(ModuleId, TemplateType, TemplateId);
					break;
				default:
					//Get From DB
					sTemplate = GetTemplate(TemplateId, TemplateType);
					break;
			}
			sTemplate = sTemplate.Replace("[TOOLBAR]", string.Empty);
			sTemplate = sTemplate.Replace("[TEMPLATE:TOOLBAR]", string.Empty);

			return sTemplate;
		}
		private static string GetTemplateFromMemory(int ModuleId, string TemplateType, int TemplateId)
		{
			string sTemplate = string.Empty;

			string myFile;
			object obj = CacheRetrieve(ModuleId + TemplateId + TemplateType);
			if (disableCache)
			{
				obj = null;
			}
			if (obj == null)
			{
				if (TemplateId == 0)
				{
					try
					{
						myFile = HttpContext.Current.Server.MapPath("~/DesktopModules/ActiveForums/config/templates/" + TemplateType + ".txt");
						if (System.IO.File.Exists(myFile))
						{
							System.IO.StreamReader objStreamReader = null;
							try
							{
								objStreamReader = System.IO.File.OpenText(myFile);
							}
							catch (Exception ex)
							{
								Services.Exceptions.Exceptions.LogException(ex);
							}
							sTemplate = objStreamReader.ReadToEnd();
							objStreamReader.Close();
							sTemplate = Utilities.ParseSpacer(sTemplate);
							CacheStore(ModuleId + TemplateId + TemplateType, sTemplate);
							//Current.Cache.Insert(ModuleId & TemplateId & TemplateType, sTemplate, New System.Web.Caching.CacheDependency(Current.Server.MapPath("~/DesktopModules/ActiveForums/config/Templates/" & TemplateType & ".txt")))
						}
					}
					catch (Exception ex)
					{
						Services.Exceptions.Exceptions.LogException(ex);
					}
				}
				else
				{
					sTemplate = GetTemplate(TemplateId, TemplateType);
					CacheStore(ModuleId + TemplateId + TemplateType, sTemplate);
				}
			}
			else
			{
				sTemplate = Convert.ToString(obj);
			}
			return sTemplate;
		}
		private static string GetTemplateFromDisk(int ModuleId, string TemplateType, int TemplateId)
		{
			string sTemplate;
			string myFile;
			string FileName = ModuleId + "_" + TemplateId + TemplateType + ".resources";
			System.IO.StreamReader objStreamReader;
			if (_disableCache)
			{
				sTemplate = GetTemplate(TemplateId, TemplateType);
			}
			else
			{
				try
				{
					myFile = HttpContext.Current.Request.MapPath(Common.Globals.ApplicationPath) + "\\DesktopModules\\ActiveForums\\cache\\" + FileName;
					if (System.IO.File.Exists(myFile))
					{
						try
						{
							objStreamReader = System.IO.File.OpenText(myFile);

							sTemplate = objStreamReader.ReadToEnd();
							objStreamReader.Close();
						}
						catch (Exception exc)
						{
							sTemplate = GetTemplate(TemplateId, TemplateType);
						}
					}
					else
					{
						sTemplate = GetTemplate(TemplateId, TemplateType);
						CacheTemplateToDisk(ModuleId, TemplateId, TemplateType, sTemplate);
					}
				}
				catch (Exception ex)
				{
					Services.Exceptions.Exceptions.LogException(ex);
					sTemplate = "ERROR: Loading template failed";
				}
			}


			return sTemplate;
		}
		private static string GetTemplate(int TemplateId, string TemplateType)
		{
			string sOut = string.Empty;
		    try
			{
				if (TemplateId == 0)
				{
					try
					{
					    string myFile = HttpContext.Current.Server.MapPath("~/DesktopModules/ActiveForums/config/templates/" + TemplateType + ".txt");
					    if (System.IO.File.Exists(myFile))
						{
							System.IO.StreamReader objStreamReader = null;
							try
							{
								objStreamReader = System.IO.File.OpenText(myFile);
							}
							catch (Exception ex)
							{
								Services.Exceptions.Exceptions.LogException(ex);
							}
							sOut = objStreamReader.ReadToEnd();
							objStreamReader.Close();
							sOut = Utilities.ParseSpacer(sOut);
						}
					}
					catch (Exception ex)
					{
						Services.Exceptions.Exceptions.LogException(ex);
					}
				}
				else
				{
					var objTemplates = new TemplateController();
					TemplateInfo objTempInfo = objTemplates.Template_Get(TemplateId);
					if (objTempInfo != null)
					{
						sOut = objTempInfo.TemplateHTML;
						sOut = Utilities.ParseSpacer(sOut);
					}
				}

			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
				sOut = "ERROR: Loading template failed";
			}
			return sOut;
		}
		internal static string GetTemplate(string TemplateFileName)
		{
			string sOut = string.Empty;
			string myFile;
			try
			{
				try
				{
					myFile = HttpContext.Current.Server.MapPath("~/DesktopModules/ActiveForums/config/templates/" + TemplateFileName);
					if (System.IO.File.Exists(myFile))
					{
						System.IO.StreamReader objStreamReader = null;
						try
						{
							objStreamReader = System.IO.File.OpenText(myFile);
						}
						catch (Exception ex)
						{
							Services.Exceptions.Exceptions.LogException(ex);
						}
						sOut = objStreamReader.ReadToEnd();
						objStreamReader.Close();
						sOut = Utilities.ParseSpacer(sOut);
					}
				}
				catch (Exception ex)
				{
					Services.Exceptions.Exceptions.LogException(ex);
				}

			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
				sOut = "ERROR: Loading template failed";
			}
			return sOut;
		}
#endregion


	}
}

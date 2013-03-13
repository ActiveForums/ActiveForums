//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
//ORIGINAL LINE: Imports System.Web.HttpContext

using System;
using System.Collections;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
#region SettingsInfo
	public class SettingsInfo
	{
		private Hashtable _MainSettings = new Hashtable();
		public Hashtable MainSettings
		{
			get
			{
				return _MainSettings;
			}
			set
			{
				_MainSettings = value;
			}
		}
		public int PageSize
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.PageSize].ToString());
				}
				catch (Exception ex)
				{
					return 20;
				}

			}
		}



		public string UserNameDisplay
		{
			get
			{
				try
				{
					return _MainSettings[SettingKeys.UserNameDisplay].ToString();
				}
				catch (Exception ex)
				{
					return "USERNAME";
				}
			}
		}

		public ProfileTypes ProfileType
		{
			get
			{
				try
				{
					return (ProfileTypes)(Convert.ToInt32(_MainSettings[SettingKeys.ProfileType]));
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}


		public bool EnablePoints
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_MainSettings[SettingKeys.EnablePoints]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public int TopicPointValue
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.TopicPointValue]);
				}
				catch (Exception ex)
				{
					return 1;
				}
			}
		}
		public int ReplyPointValue
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.ReplyPointValue]);
				}
				catch (Exception ex)
				{
					return 1;
				}
			}
		}
		public int AnswerPointValue
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.AnswerPointValue]);
				}
				catch (Exception ex)
				{
					return 1;
				}
			}
		}
		public int MarkAsAnswerPointValue
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.MarkAnswerPointValue]);
				}
				catch (Exception ex)
				{
					return 1;
				}
			}
		}
		public int ModPointValue
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.ModPointValue]);
				}
				catch (Exception ex)
				{
					return 1;
				}
			}
		}

		public int AvatarHeight
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.AvatarHeight]);
				}
				catch (Exception ex)
				{
					return 80;
				}
			}
		}
		public int AvatarWidth
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.AvatarWidth]);
				}
				catch (Exception ex)
				{
					return 80;
				}
			}
		}
		public int AllowSignatures
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.AllowSignatures]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public string DateFormatString
		{
			get
			{
				try
				{
					return Convert.ToString(_MainSettings[SettingKeys.DateFormatString]);
				}
				catch (Exception ex)
				{
					return "MM/dd/yyyy";
				}
			}
		}
		public string TimeFormatString
		{
			get
			{
				try
				{
					return Convert.ToString(_MainSettings[SettingKeys.TimeFormatString]);
				}
				catch (Exception ex)
				{
					return "h:mm tt";
				}
			}
		}
		public int TimeZoneOffset
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.TimeZoneOffset]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public bool UsersOnlineEnabled
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_MainSettings[SettingKeys.UsersOnlineEnabled]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public string MemberListMode
		{
			get
			{
				return "Enabled";
			}
		}
		public int ForumTemplateID
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.ForumTemplateId]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public DateTime InstallDate
		{
			get
			{
				try
				{
					if (_MainSettings[SettingKeys.InstallDate] == null)
					{
						return Utilities.NullDate();
					}
				    System.Globalization.DateTimeFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
				    return DateTime.Parse(_MainSettings[SettingKeys.InstallDate].ToString(), nfi);
				}
				catch (Exception ex)
				{
					return Utilities.NullDate();
				}
			}
		}
		public bool IsInstalled
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_MainSettings[SettingKeys.IsInstalled]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public bool NeedsConversion
		{
			get
			{
			    if (_MainSettings.ContainsKey("NeedsConvert"))
				{
					return Convert.ToBoolean(_MainSettings["NeedsConvert"]);
				}
			    return true;
			}
		}
		public PMTypes PMType
		{
			get
			{
				try
				{
					return (PMTypes)(Convert.ToInt32(_MainSettings[SettingKeys.PMType]));
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public int PMTabId
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.PMTabId]);
				}
				catch (Exception ex)
				{
					return -1;
				}
			}
		}
		public bool DisableAccountTab
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_MainSettings[SettingKeys.DisableAccountTab]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public string Theme
		{
			get
			{
				try
				{
					string _theme = "_default";
					if (MainSettings[SettingKeys.Theme] == null)
					{
						return _theme;
					}
				    return _MainSettings[SettingKeys.Theme].ToString();
				}
				catch (Exception ex)
				{
					return "_default";
				}
			}
		}
		public bool FullText
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_MainSettings[SettingKeys.FullText]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public string AllowSubTypes
		{
			get
			{
				try
				{
					return _MainSettings[SettingKeys.AllowSubTypes].ToString();
				}
				catch (Exception ex)
				{
					return string.Empty;
				}
			}
		}
		public int TemplateCache
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.TemplateCache]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public bool MailQueue
		{
			get
			{
				try
				{
					return Convert.ToBoolean(_MainSettings[SettingKeys.MailQueue]);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		public int FloodInterval
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.FloodInterval]);
				}
				catch (Exception ex)
				{
					return 5;
				}
			}
		}
		public int EditInterval
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.EditInterval]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}

		public int DeleteBehavior
		{
			get
			{
				try
				{
					return Convert.ToInt32(_MainSettings[SettingKeys.DeleteBehavior]);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}
		public string ProfileVisibility
		{
			get
			{
				try
				{
					return _MainSettings[SettingKeys.ProfileVisibility].ToString();
				}
				catch (Exception ex)
				{
					return "ENABLEDREG";
				}
			}
		}
		public string AddThisAccount
		{
			get
			{
				try
				{
					return _MainSettings[SettingKeys.AddThisAccount].ToString();
				}
				catch (Exception ex)
				{
					return string.Empty;
				}
			}
		}
		public bool UseShortUrls
		{
			get
			{
			    if (! (_MainSettings.ContainsKey(SettingKeys.UseShortUrls)))
				{
					return false;
				}
			    return bool.Parse(_MainSettings[SettingKeys.UseShortUrls].ToString());
			}
		}

		public bool UseSkinBreadCrumb
		{
			get
			{
			    if (! (_MainSettings.ContainsKey(SettingKeys.UseSkinBreadCrumb)))
				{
					return false;
				}
			    return bool.Parse(_MainSettings[SettingKeys.UseSkinBreadCrumb].ToString());
			}
		}
		public bool AutoLinkEnabled
		{
			get
			{
			    if (! (_MainSettings.ContainsKey(SettingKeys.EnableAutoLink)))
				{
					return true;
				}
			    return bool.Parse(_MainSettings[SettingKeys.EnableAutoLink].ToString());
			}
		}
		public string ActiveSocialTopicsKey
		{
			get
			{
			    if (! (_MainSettings.ContainsKey(SettingKeys.ActiveSocialTopicKey)))
				{
					return string.Empty;
				}
			    return _MainSettings[SettingKeys.ActiveSocialTopicKey].ToString();
			}
		}
		public string ActiveSocialReplyKey
		{
			get
			{
			    if (! (_MainSettings.ContainsKey(SettingKeys.ActiveSocialRepliesKey)))
				{
					return string.Empty;
				}
			    return _MainSettings[SettingKeys.ActiveSocialRepliesKey].ToString();
			}
		}
		public bool URLRewriteEnabled
		{
			get
			{
			    if (Utilities.IsRewriteLoaded())
				{
				    if (! (_MainSettings.ContainsKey(SettingKeys.EnableURLRewriter)))
					{
						return false;
					}
				    return bool.Parse(_MainSettings[SettingKeys.EnableURLRewriter].ToString());
				}
			    return false;
			}
		}
		public string PrefixURLBase
		{
			get
			{
			    if (! (_MainSettings.ContainsKey(SettingKeys.PrefixURLBase)))
				{
					return string.Empty;
				}
			    return _MainSettings[SettingKeys.PrefixURLBase].ToString();
			}
		}
		public string PrefixURLOther
		{
			get
			{
				string s;
				s = ! (_MainSettings.ContainsKey(SettingKeys.PrefixURLOther)) ? "other" : _MainSettings[SettingKeys.PrefixURLOther].ToString();
				if (URLRewriteEnabled)
				{
					return s;
				}
			    return string.Empty;
			}
		}
		public string PrefixURLTag
		{
			get
			{
				string s;
				s = ! (_MainSettings.ContainsKey(SettingKeys.PrefixURLTags)) ? "tag" : _MainSettings[SettingKeys.PrefixURLTags].ToString();
				if (URLRewriteEnabled)
				{
					return s;
				}
			    return string.Empty;
			}
		}
		public string PrefixURLCategory
		{
			get
			{
				string s;
				s = ! (_MainSettings.ContainsKey(SettingKeys.PrefixURLCategories)) ? "category" : _MainSettings[SettingKeys.PrefixURLCategories].ToString();
				if (URLRewriteEnabled)
				{
					return s;
				}
			    return string.Empty;
			}
		}

	}
#endregion

	public class Settings
	{
		public static Hashtable GeneralSettings(int ModuleId, string GroupKey)
		{
			var ht = new Hashtable();
			IDataReader dr;
			dr = DataProvider.Instance().Settings_List(ModuleId, GroupKey);
			while (dr.Read())
			{
				ht.Add(dr["SettingName"].ToString(), dr["SettingValue"].ToString());
			}
			dr.Close();
			return ht;
		}
		public static string GetSetting(int ModuleId, string GroupKey, string SettingName)
		{
			string sOut = string.Empty;
			try
			{
				sOut = DataProvider.Instance().Settings_Get(ModuleId, GroupKey, SettingName);
			}
			catch (Exception ex)
			{

			}

			return sOut;
		}
		public static void SaveSetting(int ModuleId, string GroupKey, string SettingName, string SettingValue)
		{
			try
			{
				DataProvider.Instance().Settings_Save(ModuleId, GroupKey, SettingName, SettingValue);
			}
			catch (Exception ex)
			{

			}
		}
	}
}


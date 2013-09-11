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
using DotNetNuke.Entities.Profile;

namespace DotNetNuke.Modules.ActiveForums
{
    #region SettingsInfo
	
    public class SettingsInfo
	{
        public Hashtable MainSettings { get; set; }

        public int PageSize
        {
            get { return MainSettings.GetInt(SettingKeys.PageSize, 20); }
        }

        public string UserNameDisplay
        {
            get { return MainSettings.GetString(SettingKeys.UserNameDisplay, "USERNAME"); }
        }

        public bool EnablePoints
        {
            get { return MainSettings.GetBoolean(SettingKeys.EnablePoints); }
        }

        public int TopicPointValue
        {
            get { return MainSettings.GetInt(SettingKeys.TopicPointValue, 1); }
        }

        public int ReplyPointValue
        {
            get { return MainSettings.GetInt(SettingKeys.ReplyPointValue, 1); }
        }

        public int AnswerPointValue
        {
            get { return MainSettings.GetInt(SettingKeys.AnswerPointValue, 1); }
        }

        public int MarkAsAnswerPointValue
        {
            get { return MainSettings.GetInt(SettingKeys.MarkAnswerPointValue, 1); }
        }

        public int ModPointValue
        {
            get { return MainSettings.GetInt(SettingKeys.ModPointValue, 1); }
        }

        public int AvatarHeight
        {
            get { return MainSettings.GetInt(SettingKeys.AvatarHeight, 80); }
        }

        public int AvatarWidth
        {
            get { return MainSettings.GetInt(SettingKeys.AvatarWidth, 80); }
        }

        public int AllowSignatures
        {
            get { return MainSettings.GetInt(SettingKeys.AllowSignatures); }
        }

        public string DateFormatString
        {
            get { return MainSettings.GetString(SettingKeys.DateFormatString, "M/d/yyyy"); }
        }

        public string TimeFormatString
		{
			get { return MainSettings.GetString(SettingKeys.TimeFormatString, "h:mm tt"); }
		}

        public int TimeZoneOffset
        {
            get { return MainSettings.GetInt(SettingKeys.TimeZoneOffset); }
        }

        public bool UsersOnlineEnabled
        {
            get { return MainSettings.GetBoolean(SettingKeys.UsersOnlineEnabled); }
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
            get { return MainSettings.GetInt(SettingKeys.ForumTemplateId); }
        }

        public DateTime InstallDate
		{
			get { return Utilities.SafeConvertDateTime(MainSettings[SettingKeys.InstallDate], Utilities.NullDate()); }
		}

        public bool IsInstalled
        {
            get { return MainSettings.GetBoolean(SettingKeys.IsInstalled); }
        }

        public bool NeedsConversion
        {
            get { return MainSettings.GetBoolean("NeedsConvert", true); }
        }

        public PMTypes PMType
        {
            get
            {
                PMTypes parsedValue;
                return Enum.TryParse(MainSettings.GetString(SettingKeys.PMType), true, out parsedValue)
                           ? parsedValue
                           : PMTypes.Disabled;
            }
        }

        public int PMTabId
        {
            get { return MainSettings.GetInt(SettingKeys.PMTabId, -1); }
        }

        public bool DisableAccountTab
        {
            get { return MainSettings.GetBoolean(SettingKeys.DisableAccountTab); }
        }

        public string Theme
		{
			get
			{
			    var result = MainSettings.GetString(SettingKeys.Theme);
			    return string.IsNullOrWhiteSpace(result) ? "_default" : result; 
			}
		}

        public bool FullText
        {
            get { return MainSettings.GetBoolean(SettingKeys.FullText); }
        }

        public string AllowSubTypes
        {
            get { return MainSettings.GetString(SettingKeys.AllowSubTypes, string.Empty); }
        }

        public int TemplateCache
        {
            get { return MainSettings.GetInt(SettingKeys.TemplateCache); }
        }

        public bool MailQueue
        {
            get { return MainSettings.GetBoolean(SettingKeys.MailQueue); }
        }

        public int FloodInterval
        {
            get { return MainSettings.GetInt(SettingKeys.FloodInterval, 5); }
        }

        public int EditInterval
        {
            get { return MainSettings.GetInt(SettingKeys.EditInterval); }
        }

        public int DeleteBehavior
        {
            get { return MainSettings.GetInt(SettingKeys.DeleteBehavior); }
        }

        public ProfileVisibilities ProfileVisibility
        {
            get
            {
                ProfileVisibilities parsedValue;

                return Enum.TryParse(MainSettings.GetString(SettingKeys.ProfileVisibility), true,
                                     out parsedValue)
                           ? parsedValue
                           : ProfileVisibilities.Disabled;
            }
        }

        public string AddThisAccount
        {
            get { return MainSettings.GetString(SettingKeys.AddThisAccount, string.Empty); }
        }

        public bool UseShortUrls
        {
            get { return MainSettings.GetBoolean(SettingKeys.UseShortUrls); }
        }

        public bool UseSkinBreadCrumb
        {
            get { return MainSettings.GetBoolean(SettingKeys.UseSkinBreadCrumb); }
        }

        public bool AutoLinkEnabled
        {
            get { return MainSettings.GetBoolean(SettingKeys.EnableAutoLink, true); }
        }

        public string ActiveSocialTopicsKey
        {
            get { return Utilities.SafeConvertString(MainSettings.ContainsKey(SettingKeys.ActiveSocialTopicKey), string.Empty); }
        }

        public string ActiveSocialReplyKey
        {
            get { return MainSettings.GetString(SettingKeys.ActiveSocialRepliesKey, string.Empty); }
        }

        public bool URLRewriteEnabled
        {
            get { return MainSettings.GetBoolean(SettingKeys.EnableURLRewriter); }
        }

        public string PrefixURLBase
        {
            get { return MainSettings.GetString(SettingKeys.PrefixURLBase, string.Empty); }
        }

        public string PrefixURLOther
        {
            get
            {
                return !URLRewriteEnabled
                           ? string.Empty
                           : MainSettings.GetString(SettingKeys.PrefixURLOther, "other");
            }
        }

        public string PrefixURLTag
        {
            get
            {
                return !URLRewriteEnabled
                           ? string.Empty
                           : MainSettings.GetString(SettingKeys.PrefixURLTags, "tag");
            }
        }

        public string PrefixURLCategory
        {
            get
            {
                return !URLRewriteEnabled
                           ? string.Empty
                           : MainSettings.GetString(SettingKeys.PrefixURLCategories, "category");
            }
        }

        public SettingsInfo()
        {
            MainSettings = new Hashtable();
        }

	}

    #endregion

	public class Settings
	{
		public static Hashtable GeneralSettings(int moduleId, string groupKey)
		{
			var ht = new Hashtable();
		    var dr = DataProvider.Instance().Settings_List(moduleId, groupKey);
			while (dr.Read())
			{
				ht.Add(dr.GetString("SettingName"), dr.GetString("SettingValue"));
			}
			dr.Close();
			return ht;
		}

		public static string GetSetting(int moduleId, string groupKey, string settingName)
		{
			try
			{
				return DataProvider.Instance().Settings_Get(moduleId, groupKey, settingName);
			}
			catch (Exception)
			{
			    return string.Empty;
			}
		}

		public static bool SaveSetting(int moduleId, string groupKey, string settingName, string settingValue)
		{
			try
			{
				DataProvider.Instance().Settings_Save(moduleId, groupKey, settingName, settingValue);
			    return true;
			}
			catch (Exception ex)
			{
			    return false;
			}
		}
	}
}


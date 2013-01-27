using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DotNetNuke;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ForumSettingsBase : Entities.Modules.ModuleSettingsBase
	{
		private Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();

		public string Mode
		{
			get
			{
				return GetStringSetting(SettingKeys.Mode, "Standard");
			}
			set
			{
				Settings[SettingKeys.Mode] = value;
				objModules.UpdateTabModuleSetting(TabModuleId, SettingKeys.Mode, value);
			}
		}

		public string Theme
		{
			get
			{
				return GetStringSetting(SettingKeys.Theme, "_default");
			}
			set
			{
				Settings[SettingKeys.Theme] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.Theme, value);
			}
		}

		public int TemplateId
		{
			get
			{
				return GetIntegerSetting(SettingKeys.ForumTemplateId, 0);
			}
			set
			{
				Settings[SettingKeys.ForumTemplateId] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.ForumTemplateId, value.ToString());
			}
		}

		public int PageSize
		{
			get
			{
				return GetIntegerSetting(SettingKeys.PageSize, 20);
			}
			set
			{
				Settings[SettingKeys.PageSize] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.PageSize, value.ToString());
			}
		}

		public int FloodInterval
		{
			get
			{
				return GetIntegerSetting(SettingKeys.FloodInterval, 0);
			}
			set
			{
				Settings[SettingKeys.FloodInterval] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.FloodInterval, value.ToString());
			}
		}

		public int EditInterval
		{
			get
			{
				return GetIntegerSetting(SettingKeys.EditInterval, 0);
			}
			set
			{
				Settings[SettingKeys.EditInterval] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.EditInterval, value.ToString());
			}
		}

		public bool AutoLink
		{
			get
			{
				return GetBooleanSetting(SettingKeys.EnableAutoLink, true);
			}
			set
			{
				Settings[SettingKeys.EnableAutoLink] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.EnableAutoLink, value.ToString());
			}
		}

		public int DeleteBehavior
		{
			get
			{
				return GetIntegerSetting(SettingKeys.DeleteBehavior, 0);
			}
			set
			{
				Settings[SettingKeys.DeleteBehavior] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.DeleteBehavior, value.ToString());
			}
		}

		public string AddThis
		{
			get
			{
				return GetStringSetting(SettingKeys.AddThisAccount, string.Empty);
			}
			set
			{
				Settings[SettingKeys.AddThisAccount] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.AddThisAccount, value);
			}
		}

		public int ProfileType
		{
			get
			{
				return GetIntegerSetting(SettingKeys.ProfileType, 1);
			}
			set
			{
				Settings[SettingKeys.ProfileType] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.ProfileType, value.ToString());
			}
		}

        public int MessagingType
        {
            get
            {
                return GetIntegerSetting(SettingKeys.PMType, 0);
            }
            set
            {
                Settings[SettingKeys.PMType] = value;
                objModules.UpdateModuleSetting(ModuleId, SettingKeys.PMType, value.ToString());
            }
        }

        public int MessagingTabId
        {
            get
            {
                return GetIntegerSetting(SettingKeys.PMTabId, 0);
            }
            set
            {
                Settings[SettingKeys.PMTabId] = value;
                objModules.UpdateModuleSetting(ModuleId, SettingKeys.PMTabId, value.ToString());
            }
        }

		public int Signatures
		{
			get
			{
				return GetIntegerSetting(SettingKeys.AllowSignatures, 1);
			}
			set
			{
				Settings[SettingKeys.AllowSignatures] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.AllowSignatures, value.ToString());
			}
		}

		public string UserNameDisplay
		{
			get
			{
				return GetStringSetting(SettingKeys.UserNameDisplay, "Username");
			}
			set
			{
				Settings[SettingKeys.UserNameDisplay] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.UserNameDisplay, value);
			}
		}

		public bool FriendlyURLs
		{
			get
			{
				return GetBooleanSetting(SettingKeys.EnableURLRewriter, false);
			}
			set
			{
				Settings[SettingKeys.EnableURLRewriter] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.EnableURLRewriter, value.ToString());
			}
		}

		public string PrefixURLBase
		{
			get
			{
				return GetStringSetting(SettingKeys.PrefixURLBase, "forums");
			}
			set
			{
				Settings[SettingKeys.PrefixURLBase] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.PrefixURLBase, value);
			}
		}
		public string PrefixURLTag
		{
			get
			{
				return GetStringSetting(SettingKeys.PrefixURLTags, "tag");
			}
			set
			{
				Settings[SettingKeys.PrefixURLTags] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.PrefixURLTags, value);
			}
		}

		public string PrefixURLCategory
		{
			get
			{
				return GetStringSetting(SettingKeys.PrefixURLCategories, "category");
			}
			set
			{
				Settings[SettingKeys.PrefixURLCategories] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.PrefixURLCategories, value);
			}
		}

		public string PrefixURLOther
		{
			get
			{
				return GetStringSetting(SettingKeys.PrefixURLOther, "views");
			}
			set
			{
				Settings[SettingKeys.PrefixURLOther] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.PrefixURLOther, value);
			}
		}

		public bool FullTextSearch
		{
			get
			{
				return GetBooleanSetting(SettingKeys.FullText, false);
			}
			set
			{
				Settings[SettingKeys.FullText] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.FullText, value.ToString());
			}
		}

		public bool MailQueue
		{
			get
			{
				return GetBooleanSetting(SettingKeys.MailQueue, false);
			}
			set
			{
				Settings[SettingKeys.MailQueue] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.MailQueue, value.ToString());
			}
		}

		public bool EnablePoints
		{
			get
			{
				return GetBooleanSetting(SettingKeys.EnablePoints, false);
			}
			set
			{
				Settings[SettingKeys.EnablePoints] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.EnablePoints, value.ToString());
			}
		}

		public int TopicPointValue
		{
			get
			{
				return GetIntegerSetting(SettingKeys.TopicPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.TopicPointValue] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.TopicPointValue, value.ToString());
			}
		}

		public int ReplyPointValue
		{
			get
			{
				return GetIntegerSetting(SettingKeys.ReplyPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.ReplyPointValue] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.ReplyPointValue, value.ToString());
			}
		}

		public int AnswerPointValue
		{
			get
			{
				return GetIntegerSetting(SettingKeys.AnswerPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.AnswerPointValue] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.AnswerPointValue, value.ToString());
			}
		}

		public int MarkAsAnswerPointValue
		{
			get
			{
				return GetIntegerSetting(SettingKeys.MarkAnswerPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.MarkAnswerPointValue] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.MarkAnswerPointValue, value.ToString());
			}
		}

		public int ModPointValue
		{
			get
			{
				return GetIntegerSetting(SettingKeys.ModPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.ModPointValue] = value;
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.ModPointValue, value.ToString());
			}
		}

		public int ForumGroupTemplate
		{
			get
			{
				return GetIntegerSetting("ForumGroupTemplate", -1);
			}
			set
			{
				Settings["ForumGroupTemplate"] = value;
				//Use Tab Module Setting
				objModules.UpdateTabModuleSetting(TabModuleId, "ForumGroupTemplate", value.ToString());
			}
		}

		public string ForumConfig
		{
			get
			{
				return GetStringSetting("ForumConfig", string.Empty);
			}
			set
			{
				Settings["ForumConfig"] = value;
				objModules.UpdateTabModuleSetting(TabModuleId, "ForumConfig", value);
			}
		}
		public int AvatarHeight
		{
			get
			{
				return GetIntegerSetting(SettingKeys.AvatarHeight, 48);
			}
			set
			{
				Settings[SettingKeys.AvatarHeight] = value.ToString();
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.AvatarHeight, value.ToString());
			}
		}
		public int AvatarWidth
		{
			get
			{
				return GetIntegerSetting(SettingKeys.AvatarWidth, 48);
			}
			set
			{
				Settings[SettingKeys.AvatarWidth] = value.ToString();
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.AvatarWidth, value.ToString());
			}
		}
		public int GetIntegerSetting(string key, int defaultValue)
		{
			if (Settings.ContainsKey(key))
			{
				if (SimulateIsNumeric.IsNumeric(Settings[key]))
				{
					return Convert.ToInt32(Settings[key].ToString());
				}
				else
				{
					return defaultValue;
				}
			}
			else
			{
				return defaultValue;
			}
		}
		public bool GetBooleanSetting(string key, bool defaultValue)
		{
			if (Settings.ContainsKey(key))
			{
				return Convert.ToBoolean(Settings[key]);
			}
			else
			{
				return defaultValue;
			}
		}
		public string GetStringSetting(string key, string defaultValue)
		{
			if (Settings.ContainsKey(key))
			{
				return Settings[key].ToString();
			}
			else
			{
				return defaultValue;
			}
		}
	}
}


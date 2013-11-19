﻿//
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
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ForumSettingsBase : ModuleSettingsBase
	{
		private readonly ModuleController _objModules = new ModuleController();

		public string Mode
		{
			get
			{
                return Settings.GetString(SettingKeys.Mode, "Standard");
			}
			set
			{
				Settings[SettingKeys.Mode] = value;
				_objModules.UpdateTabModuleSetting(TabModuleId, SettingKeys.Mode, value);
			}
		}

		public string Theme
		{
			get
			{
                return Settings.GetString(SettingKeys.Theme, "_default");
			}
			set
			{
				Settings[SettingKeys.Theme] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.Theme, value);
			}
		}

        public string TimeFormatString
        {
            get
            {
                return Settings.GetString(SettingKeys.TimeFormatString, "h:mm tt");
            }
            set
            {
                Settings[SettingKeys.TimeFormatString] = value;
                _objModules.UpdateModuleSetting(ModuleId, SettingKeys.TimeFormatString, value);
            }
        }

        public string DateFormatString
        {
            get
            {
                return Settings.GetString(SettingKeys.DateFormatString, "MM/dd/yyyy");
            }
            set
            {
                Settings[SettingKeys.DateFormatString] = value;
                _objModules.UpdateModuleSetting(ModuleId, SettingKeys.DateFormatString, value);
            }
        }

		public int TemplateId
		{
			get
			{
				return Settings.GetInt(SettingKeys.ForumTemplateId);
			}
			set
			{
				Settings[SettingKeys.ForumTemplateId] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.ForumTemplateId, value.ToString());
			}
		}

		public int PageSize
		{
			get
			{
				return Settings.GetInt(SettingKeys.PageSize, 20);
			}
			set
			{
				Settings[SettingKeys.PageSize] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.PageSize, value.ToString());
			}
		}

		public int FloodInterval
		{
			get
			{
                return Settings.GetInt(SettingKeys.FloodInterval);
			}
			set
			{
				Settings[SettingKeys.FloodInterval] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.FloodInterval, value.ToString());
			}
		}

		public int EditInterval
		{
			get
			{
                return Settings.GetInt(SettingKeys.EditInterval);
			}
			set
			{
				Settings[SettingKeys.EditInterval] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.EditInterval, value.ToString());
			}
		}

		public bool AutoLink
		{
			get
			{
                return Settings.GetBoolean(SettingKeys.EnableAutoLink, true);
			}
			set
			{
				Settings[SettingKeys.EnableAutoLink] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.EnableAutoLink, value.ToString());
			}
		}

		public int DeleteBehavior
		{
			get
			{
                return Settings.GetInt(SettingKeys.DeleteBehavior);
			}
			set
			{
				Settings[SettingKeys.DeleteBehavior] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.DeleteBehavior, value.ToString());
			}
		}

		public string AddThis
		{
			get
			{
				return Settings.GetString(SettingKeys.AddThisAccount, string.Empty);
			}
			set
			{
				Settings[SettingKeys.AddThisAccount] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.AddThisAccount, value);
			}
		}

		public int ProfileVisibility
		{
			get
			{
				return Settings.GetInt(SettingKeys.ProfileVisibility, (int)ProfileVisibilities.Everyone);
			}
			set
			{
				Settings[SettingKeys.ProfileVisibility] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.ProfileVisibility, value.ToString());
			}
		}

        public int MessagingType
        {
            get
            {
                return Settings.GetInt(SettingKeys.PMType);
            }
            set
            {
                Settings[SettingKeys.PMType] = value;
                _objModules.UpdateModuleSetting(ModuleId, SettingKeys.PMType, value.ToString());
            }
        }

        public int MessagingTabId
        {
            get
            {
                return Settings.GetInt(SettingKeys.PMTabId);
            }
            set
            {
                Settings[SettingKeys.PMTabId] = value;
                _objModules.UpdateModuleSetting(ModuleId, SettingKeys.PMTabId, value.ToString());
            }
        }

		public int Signatures
		{
			get
			{
                return Settings.GetInt(SettingKeys.AllowSignatures, 1);
			}
			set
			{
				Settings[SettingKeys.AllowSignatures] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.AllowSignatures, value.ToString());
			}
		}

		public string UserNameDisplay
		{
			get
			{
				return Settings.GetString(SettingKeys.UserNameDisplay, "Username");
			}
			set
			{
				Settings[SettingKeys.UserNameDisplay] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.UserNameDisplay, value);
			}
		}

		public bool FriendlyURLs
		{
			get
			{
				return Settings.GetBoolean(SettingKeys.EnableURLRewriter);
			}
			set
			{
				Settings[SettingKeys.EnableURLRewriter] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.EnableURLRewriter, value.ToString());
			}
		}

		public string PrefixURLBase
		{
			get
			{
				return Settings.GetString(SettingKeys.PrefixURLBase, "forums");
			}
			set
			{
				Settings[SettingKeys.PrefixURLBase] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.PrefixURLBase, value);
			}
		}

		public string PrefixURLTag
		{
			get
			{
				return Settings.GetString(SettingKeys.PrefixURLTags, "tag");
			}
			set
			{
				Settings[SettingKeys.PrefixURLTags] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.PrefixURLTags, value);
			}
		}

		public string PrefixURLCategory
		{
			get
			{
                return Settings.GetString(SettingKeys.PrefixURLCategories, "category");
			}
			set
			{
				Settings[SettingKeys.PrefixURLCategories] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.PrefixURLCategories, value);
			}
		}

		public string PrefixURLOther
		{
			get
			{
                return Settings.GetString(SettingKeys.PrefixURLOther, "views");
			}
			set
			{
				Settings[SettingKeys.PrefixURLOther] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.PrefixURLOther, value);
			}
		}

		public bool FullTextSearch
		{
			get
			{
                return Settings.GetBoolean(SettingKeys.FullText);
			}
			set
			{
				Settings[SettingKeys.FullText] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.FullText, value.ToString());
			}
		}

		public bool MailQueue
		{
			get
			{
				return Settings.GetBoolean(SettingKeys.MailQueue);
			}
			set
			{
				Settings[SettingKeys.MailQueue] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.MailQueue, value.ToString());
			}
		}

		public bool EnablePoints
		{
			get
			{
				return Settings.GetBoolean(SettingKeys.EnablePoints);
			}
			set
			{
				Settings[SettingKeys.EnablePoints] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.EnablePoints, value.ToString());
			}
		}

		public int TopicPointValue
		{
			get
			{
				return Settings.GetInt(SettingKeys.TopicPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.TopicPointValue] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.TopicPointValue, value.ToString());
			}
		}

		public int ReplyPointValue
		{
			get
			{
				return Settings.GetInt(SettingKeys.ReplyPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.ReplyPointValue] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.ReplyPointValue, value.ToString());
			}
		}

		public int AnswerPointValue
		{
			get
			{
                return Settings.GetInt(SettingKeys.AnswerPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.AnswerPointValue] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.AnswerPointValue, value.ToString());
			}
		}

		public int MarkAsAnswerPointValue
		{
			get
			{
                return Settings.GetInt(SettingKeys.MarkAnswerPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.MarkAnswerPointValue] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.MarkAnswerPointValue, value.ToString());
			}
		}

		public int ModPointValue
		{
			get
			{
                return Settings.GetInt(SettingKeys.ModPointValue, 1);
			}
			set
			{
				Settings[SettingKeys.ModPointValue] = value;
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.ModPointValue, value.ToString());
			}
		}

		public int ForumGroupTemplate
		{
			get
			{
                return Settings.GetInt("ForumGroupTemplate", -1);
			}
			set
			{
				Settings["ForumGroupTemplate"] = value;
				//Use Tab Module Setting
				_objModules.UpdateTabModuleSetting(TabModuleId, "ForumGroupTemplate", value.ToString());
			}
		}

		public string ForumConfig
		{
			get
			{
				return Settings.GetString("ForumConfig", string.Empty);
			}
			set
			{
				Settings["ForumConfig"] = value;
				_objModules.UpdateTabModuleSetting(TabModuleId, "ForumConfig", value);
			}
		}

		public int AvatarHeight
		{
			get
			{
                return Settings.GetInt(SettingKeys.AvatarHeight, 48);
			}
			set
			{
				Settings[SettingKeys.AvatarHeight] = value.ToString();
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.AvatarHeight, value.ToString());
			}
		}

		public int AvatarWidth
		{
			get
			{
                return Settings.GetInt(SettingKeys.AvatarWidth, 48);
			}
			set
			{
				Settings[SettingKeys.AvatarWidth] = value.ToString();
				_objModules.UpdateModuleSetting(ModuleId, SettingKeys.AvatarWidth, value.ToString());
			}
		}

        public bool EnableUsersOnline
        {
            get
            {
                return Settings.GetBoolean(SettingKeys.UsersOnlineEnabled);
            }
            set
            {
                Settings[SettingKeys.UsersOnlineEnabled] = value;
                _objModules.UpdateModuleSetting(ModuleId, SettingKeys.UsersOnlineEnabled, value.ToString());
            }
        }

        public bool UseSkinBreadCrumb
        {
            get
            {
                return Settings.GetBoolean(SettingKeys.UseSkinBreadCrumb);
            }
            set
            {
                Settings[SettingKeys.UseSkinBreadCrumb] = value;
                _objModules.UpdateModuleSetting(ModuleId, SettingKeys.UseSkinBreadCrumb, value.ToString());
            }
        }
	}
}


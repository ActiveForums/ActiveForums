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

namespace DotNetNuke.Modules.ActiveForums.Helpers
{
	public class SettingConversion
	{
		public bool MoveSettings(int forumModuleId, int tabModuleId)
		{
			var objModules = new Entities.Modules.ModuleController();
			var currSettings = new SettingsInfo {MainSettings = Settings.GeneralSettings(forumModuleId, "GEN")};

		    objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PageSize, currSettings.PageSize.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.UserNameDisplay, currSettings.UserNameDisplay);
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.ProfileVisibility, ((int)currSettings.ProfileVisibility).ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.EnablePoints, currSettings.EnablePoints.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.TopicPointValue, currSettings.TopicPointValue.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.ReplyPointValue, currSettings.ReplyPointValue.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.AnswerPointValue, currSettings.AnswerPointValue.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.MarkAnswerPointValue, currSettings.MarkAsAnswerPointValue.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.ModPointValue, currSettings.ModPointValue.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.AvatarHeight, currSettings.AvatarHeight.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.AvatarWidth, currSettings.AvatarWidth.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.AllowSignatures, currSettings.AllowSignatures.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.ForumTemplateId, currSettings.ForumTemplateID.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.InstallDate, currSettings.InstallDate.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.IsInstalled, currSettings.IsInstalled.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.Theme, currSettings.Theme);
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.FullText, currSettings.FullText.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.MailQueue, currSettings.MailQueue.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.FloodInterval, currSettings.FloodInterval.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.EditInterval, currSettings.EditInterval.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.DeleteBehavior, currSettings.DeleteBehavior.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.AddThisAccount, currSettings.AddThisAccount);
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.EnableAutoLink, currSettings.AutoLinkEnabled.ToString());
			objModules.UpdateModuleSetting(tabModuleId, SettingKeys.EnableURLRewriter, currSettings.URLRewriteEnabled.ToString());
			if (string.IsNullOrEmpty(currSettings.PrefixURLBase))
			{
				objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PrefixURLBase, "forums");
			}
			else
			{
				objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PrefixURLBase, currSettings.PrefixURLBase);
			}
			if (string.IsNullOrEmpty(currSettings.PrefixURLOther))
			{
				objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PrefixURLOther, "views");
			}
			else
			{
				objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PrefixURLOther, currSettings.PrefixURLOther);
			}
			if (string.IsNullOrEmpty(currSettings.PrefixURLTag))
			{
				objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PrefixURLTags, "tag");
			}
			else
			{
				objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PrefixURLTags, currSettings.PrefixURLTag);
			}
			if (string.IsNullOrEmpty(currSettings.PrefixURLCategory))
			{
				objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PrefixURLCategories, "category");
			}
			else
			{
				objModules.UpdateModuleSetting(tabModuleId, SettingKeys.PrefixURLCategories, currSettings.PrefixURLCategory);
			}

			objModules.UpdateModuleSetting(tabModuleId, "NeedsConvert", "False");
			objModules.UpdateModuleSetting(tabModuleId, "AFINSTALLED", "True");
			DataCache.CacheClear(string.Format(CacheKeys.MainSettings, forumModuleId));

            return false;
		}

	}
}


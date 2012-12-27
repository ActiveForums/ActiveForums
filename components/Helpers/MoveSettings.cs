using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums.Helpers
{
	public class SettingConversion
	{
		public bool MoveSettings(int ForumModuleId, int TabModuleId)
		{
			Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
			SettingsInfo currSettings = new SettingsInfo();
			currSettings.MainSettings = Settings.GeneralSettings(ForumModuleId, "GEN");

			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PageSize, currSettings.PageSize.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.UserNameDisplay, currSettings.UserNameDisplay);
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.ProfileType, Convert.ToInt32(currSettings.ProfileType).ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.EnablePoints, currSettings.EnablePoints.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.TopicPointValue, currSettings.TopicPointValue.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.ReplyPointValue, currSettings.ReplyPointValue.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.AnswerPointValue, currSettings.AnswerPointValue.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.MarkAnswerPointValue, currSettings.MarkAsAnswerPointValue.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.ModPointValue, currSettings.ModPointValue.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.AvatarHeight, currSettings.AvatarHeight.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.AvatarWidth, currSettings.AvatarWidth.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.AllowSignatures, currSettings.AllowSignatures.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.ForumTemplateId, currSettings.ForumTemplateID.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.InstallDate, currSettings.InstallDate.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.IsInstalled, currSettings.IsInstalled.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.Theme, currSettings.Theme.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.FullText, currSettings.FullText.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.MailQueue, currSettings.MailQueue.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.FloodInterval, currSettings.FloodInterval.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.EditInterval, currSettings.EditInterval.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.DeleteBehavior, currSettings.DeleteBehavior.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.AddThisAccount, currSettings.AddThisAccount.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.EnableAutoLink, currSettings.AutoLinkEnabled.ToString());
			objModules.UpdateModuleSetting(TabModuleId, SettingKeys.EnableURLRewriter, currSettings.URLRewriteEnabled.ToString());
			if (string.IsNullOrEmpty(currSettings.PrefixURLBase))
			{
				objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PrefixURLBase, "forums");
			}
			else
			{
				objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PrefixURLBase, currSettings.PrefixURLBase.ToString());
			}
			if (string.IsNullOrEmpty(currSettings.PrefixURLOther))
			{
				objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PrefixURLOther, "views");
			}
			else
			{
				objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PrefixURLOther, currSettings.PrefixURLOther.ToString());
			}
			if (string.IsNullOrEmpty(currSettings.PrefixURLTag))
			{
				objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PrefixURLTags, "tag");
			}
			else
			{
				objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PrefixURLTags, currSettings.PrefixURLTag.ToString());
			}
			if (string.IsNullOrEmpty(currSettings.PrefixURLCategory))
			{
				objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PrefixURLCategories, "category");
			}
			else
			{
				objModules.UpdateModuleSetting(TabModuleId, SettingKeys.PrefixURLCategories, currSettings.PrefixURLCategory.ToString());
			}

			objModules.UpdateModuleSetting(TabModuleId, "NeedsConvert", "False");
			objModules.UpdateModuleSetting(TabModuleId, "AFINSTALLED", "True");
			DataCache.CacheClear(string.Format(CacheKeys.MainSettings, ForumModuleId));

            return false;
		}

	}
}


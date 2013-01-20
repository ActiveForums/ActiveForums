using System;
using System.Collections;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.ActiveForums
{
    public class WhatsNewModuleSettings
    {
        public const string RowsSettingsKey = "AFTopPostsNumber";
        public const string ForumsSettingsKey = "AFTopPostsForums";
        public const string RSSEnabledSettingsKey = "AFTopPostsRSS";
        public const string RSSIgnoreSecuritySettingsKey = "AFTopPostsSecurity";
        public const string RSSIncludeBodySettingsKey = "AFTopPostsBody";
        public const string RSSCacheTimeoutSettingsKey = "AFTopPostsCache";
        public const string TopicsOnlySettingsKey = "AFTopPostsTopicsOnly";
        public const string RandomOrderSettingsKey = "AFTopPostsRandomOrder";
        public const string TagsSettingsKey = "AFTopPostsTags";
        public const string HeaderSettingsKey = "AFTopPostsHeader";
        public const string FooterSettingsKey = "AFTopPostsFooter";
        public const string FormatSettingsKey = "AFTopPostsFormat";

        public const int DefaultRows = 5;
        public const string DefaultForums = "";
        public const bool DefaultRSSEnabled = true;
        public const bool DefaultRSSIgnoreSecurity = false;
        public const bool DefaultRSSIncludeBody = false;
        public const int DefaultRSSCacheTimeout = 30;
        public const bool DefaultTopicsOnly = true;
        public const bool DefaultRandomOrder = false;
        public const string DefaultTags = "";
        public const string DefaultHeader = "<div style=\"padding:25px;padding-top:35px;\">";
        public const string DefaultFooter = "[RSSICONLINK]</div>";
        public const string DefaultFormat = "<div style=\"padding-bottom:5px;\" class=\"normal\">[SUBJECTLINK]</div>";

        public int Rows { get; set; }
        public string Forums { get; set; }
        public bool RSSEnabled { get; set; }
        public bool RSSIgnoreSecurity { get; set; }
        public bool RSSIncludeBody { get; set; }
        public int RSSCacheTimeout { get; set; }
        public bool TopicsOnly { get; set; }
        public bool RandomOrder { get; set; }
        public string Tags { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string Format { get; set; }

        public bool Save(ModuleController moduleController, int moduleId)
        {
            try
            {
                if (moduleController == null || moduleId < 0)
                    return false;

                moduleController.UpdateModuleSetting(moduleId, ForumsSettingsKey, Forums);
                moduleController.UpdateModuleSetting(moduleId, RowsSettingsKey, Rows.ToString());
                moduleController.UpdateModuleSetting(moduleId, FormatSettingsKey, Format);
                moduleController.UpdateModuleSetting(moduleId, HeaderSettingsKey, Header);
                moduleController.UpdateModuleSetting(moduleId, FooterSettingsKey, Footer);
                moduleController.UpdateModuleSetting(moduleId, RSSEnabledSettingsKey, RSSEnabled.ToString());
                moduleController.UpdateModuleSetting(moduleId, TopicsOnlySettingsKey, TopicsOnly.ToString());
                moduleController.UpdateModuleSetting(moduleId, RandomOrderSettingsKey, RandomOrder.ToString());
                moduleController.UpdateModuleSetting(moduleId, TagsSettingsKey, Tags);
                moduleController.UpdateModuleSetting(moduleId, RSSIgnoreSecuritySettingsKey, RSSIgnoreSecurity.ToString());
                moduleController.UpdateModuleSetting(moduleId, RSSIncludeBodySettingsKey, RSSIncludeBody.ToString());
                moduleController.UpdateModuleSetting(moduleId, RSSCacheTimeoutSettingsKey, RSSCacheTimeout.ToString());

                // Clear the cache
                DataCache.CacheClear("aftp_" + moduleId);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static WhatsNewModuleSettings CreateFromModuleSettings(Hashtable moduleSettings)
        {
            if (moduleSettings == null)
            {
                return new WhatsNewModuleSettings
                {
                    Rows = DefaultRows,
                    Forums = DefaultForums,
                    RSSEnabled = DefaultRSSEnabled,
                    RSSIgnoreSecurity = DefaultRSSIgnoreSecurity,
                    RSSIncludeBody = DefaultRSSIncludeBody,
                    RSSCacheTimeout = DefaultRSSCacheTimeout,
                    TopicsOnly = DefaultTopicsOnly,
                    RandomOrder = DefaultRandomOrder,
                    Tags = DefaultTags,
                    Header = DefaultHeader,
                    Footer = DefaultFooter,
                    Format = DefaultFormat
                };
            }

            return new WhatsNewModuleSettings
            {
                Rows = SimulateIsNumeric.IsNumeric(moduleSettings[RowsSettingsKey]) ? Convert.ToInt32(moduleSettings[RowsSettingsKey]) : DefaultRows,
                Forums = (moduleSettings[ForumsSettingsKey] != null) ? Convert.ToString(moduleSettings[ForumsSettingsKey]) : DefaultForums,
                RSSEnabled = SimulateIsNumeric.IsNumeric(moduleSettings[RSSEnabledSettingsKey]) ? Convert.ToBoolean(moduleSettings[RSSEnabledSettingsKey]) : DefaultRSSEnabled,
                RSSIgnoreSecurity = SimulateIsNumeric.IsNumeric(moduleSettings[RSSIgnoreSecuritySettingsKey]) ? Convert.ToBoolean(moduleSettings[RSSIgnoreSecuritySettingsKey]) : DefaultRSSIgnoreSecurity,
                RSSIncludeBody = SimulateIsNumeric.IsNumeric(moduleSettings[RSSIncludeBodySettingsKey]) ? Convert.ToBoolean(moduleSettings[RSSIncludeBodySettingsKey]) : DefaultRSSIncludeBody,
                RSSCacheTimeout = SimulateIsNumeric.IsNumeric(moduleSettings[RSSCacheTimeoutSettingsKey]) ? Convert.ToInt32(moduleSettings[RSSCacheTimeoutSettingsKey]) : DefaultRSSCacheTimeout,
                TopicsOnly = SimulateIsNumeric.IsNumeric(moduleSettings[TopicsOnlySettingsKey]) ? Convert.ToBoolean(moduleSettings[TopicsOnlySettingsKey]) : DefaultTopicsOnly,
                RandomOrder = SimulateIsNumeric.IsNumeric(moduleSettings[RandomOrderSettingsKey]) ? Convert.ToBoolean(moduleSettings[RandomOrderSettingsKey]) : DefaultRandomOrder,
                Tags = (moduleSettings[TagsSettingsKey] != null) ? Convert.ToString(moduleSettings[TagsSettingsKey]) : DefaultTags,
                Header = (moduleSettings[HeaderSettingsKey] != null) ? Convert.ToString(moduleSettings[HeaderSettingsKey]) : DefaultHeader,
                Footer = (moduleSettings[FooterSettingsKey] != null) ? Convert.ToString(moduleSettings[FooterSettingsKey]) : DefaultFooter,
                Format = (moduleSettings[FormatSettingsKey] != null) ? Convert.ToString(moduleSettings[FormatSettingsKey]) : DefaultFormat
            };
        }

    }
}

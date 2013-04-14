using System;
using System.Collections;

namespace DotNetNuke.Modules.ActiveForums
{
    public static class HashtableExtensions
    {
        public static string GetString(this Hashtable ht, string key, string defaultValue = null)
        {
            return string.IsNullOrWhiteSpace(key) || !ht.ContainsKey(key) ? defaultValue : Utilities.SafeConvertString(ht[key], defaultValue);
        }

        public static int GetInt(this Hashtable ht, string key, int defaultValue = 0)
        {
            return string.IsNullOrWhiteSpace(key) || !ht.ContainsKey(key) ? defaultValue : Utilities.SafeConvertInt(ht[key], defaultValue);
        }

        public static double GetDouble(this Hashtable ht, string key, double defaultValue = 0)
        {
            return string.IsNullOrWhiteSpace(key) || !ht.ContainsKey(key) ? defaultValue : Utilities.SafeConvertDouble(ht[key], defaultValue);
        }

        public static bool GetBoolean(this Hashtable ht, string key, bool defaultValue = false)
        {
            return string.IsNullOrWhiteSpace(key) || !ht.ContainsKey(key) ? defaultValue : Utilities.SafeConvertBool(ht[key], defaultValue);
        }

        public static DateTime GetDateTime(this Hashtable ht, string key, DateTime? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(key) || !ht.ContainsKey(key))
                return defaultValue.HasValue ? defaultValue.Value : Utilities.NullDate();

            return Utilities.SafeConvertDateTime(ht[key], defaultValue);
        }
    }
}

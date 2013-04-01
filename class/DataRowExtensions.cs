using System;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
    public static class DataRowExtensions
    {
        public static bool HasColumn(this DataRow dr, string columnName)
        {
            if (dr == null || string.IsNullOrWhiteSpace(columnName))
                return false;

            return dr.Table.Columns.Contains(columnName);
        }

        public static string GetString(this DataRow dr, string columnName, string defaultValue = null)
        {
            return !dr.HasColumn(columnName) ? defaultValue : Utilities.SafeConvertString(dr[columnName], defaultValue);
        }

        public static int GetInt(this DataRow dr, string columnName, int defaultValue = 0)
        {
            return !dr.HasColumn(columnName) ? defaultValue : Utilities.SafeConvertInt(dr[columnName], defaultValue);
        }

        public static double GetDouble(this DataRow dr, string columnName, double defaultValue = 0)
        {
            return !dr.HasColumn(columnName) ? defaultValue : Utilities.SafeConvertDouble(dr[columnName], defaultValue);
        }

        public static bool GetBoolean(this DataRow dr, string columnName, bool defaultValue = false)
        {
            return !dr.HasColumn(columnName) ? defaultValue : Utilities.SafeConvertBool(dr[columnName], defaultValue);
        }

        public static DateTime GetDateTime(this DataRow dr, string columnName, DateTime? defaultValue = null)
        {
            if (!dr.HasColumn(columnName))
                return defaultValue.HasValue ? defaultValue.Value : Utilities.NullDate();

            return Utilities.SafeConvertDateTime(dr[columnName], defaultValue);
        }
    }
}

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

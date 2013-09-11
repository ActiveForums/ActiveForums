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

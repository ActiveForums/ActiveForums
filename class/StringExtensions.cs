using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Modules.ActiveForums
{
	public static class StringExtensions
	{
        public static string TruncateAtWord(this string value, int length)
        {
            if (String.IsNullOrEmpty(value) || value.Length < length)
                return value;
            int iNextSpace = value.LastIndexOf(" ", length);
            return string.Format("{0}", value.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
        }
	}
}

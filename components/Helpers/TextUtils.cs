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

using DotNetNuke.Security;
using System.Text.RegularExpressions;
using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class Utilities
	{
		public class Text
		{
			public static string CheckSqlString(string input)
			{
				input = input.ToUpperInvariant();
				input = input.Replace("\\", "");
				input = input.Replace("[", "");
				input = input.Replace("]", "");
				input = input.Replace("(", "");
				input = input.Replace(")", "");
				input = input.Replace("{", "");
				input = input.Replace("}", "");
				input = input.Replace("'", "''");
				input = input.Replace("UNION", "");
				input = input.Replace("TABLE", "");
				input = input.Replace("WHERE", "");
				input = input.Replace("DROP", "");
				input = input.Replace("EXECUTE", "");
				input = input.Replace("EXEC ", "");
				input = input.Replace("FROM ", "");
				input = input.Replace("CMD ", "");
				input = input.Replace(";", "");
				input = input.Replace("--", "");
				//input = input.Replace("""", """""")
				return input;
			}
			public static string FilterScripts(string text)
			{
				if (string.IsNullOrEmpty(text))
				{
					return string.Empty;
				}
				PortalSecurity objPortalSecurity = new PortalSecurity();
				try
				{
					text = objPortalSecurity.InputFilter(text, PortalSecurity.FilterFlag.NoScripting);
				}
				catch (Exception ex)
				{

				}

				string pattern = "<script.*/*>|</script>|<[a-zA-Z][^>]*=['\"]+javascript:\\w+.*['\"]+>|<\\w+[^>]*\\son\\w+=.*[ /]*>";
				text = Regex.Replace(text, pattern, string.Empty, RegexOptions.IgnoreCase);
				string strip = "/*,*/,alert,document.,window.,eval(,eval[,@import,vbscript,javascript,jscript,msgbox";
				foreach (string s in strip.Split(','))
				{
					if (text.ToUpper().Contains(s.ToUpper()))
					{
						text = text.Replace(s.ToUpper(), string.Empty);
						text = text.Replace(s, string.Empty);
					}
				}
				return text;
			}
			public static string RemoveHTML(string sText)
			{
				if (string.IsNullOrEmpty(sText))
				{
					return string.Empty;
				}
				sText = HttpUtility.HtmlDecode(sText);
				sText = HttpUtility.UrlDecode(sText);
				sText = sText.Trim();
				if (string.IsNullOrEmpty(sText))
				{
					return string.Empty;
				}
				PortalSecurity objPortalSecurity = new PortalSecurity();
				sText = objPortalSecurity.InputFilter(sText, PortalSecurity.FilterFlag.NoScripting);
				sText = FilterScripts(sText);
				string strip = "/*,*/,alert,document.,window.,eval(,eval[,src=,rel=,href=,@import,vbscript,javascript,jscript,msgbox,<style";
				foreach (string s in strip.Split(','))
				{
					if (sText.ToUpper().Contains(s.ToUpper()))
					{
						sText = sText.Replace(s.ToUpper(), string.Empty);
						sText = sText.Replace(s, string.Empty);
					}
				}
				string pattern = "<(.|\\n)*?>";
				sText = Regex.Replace(sText, pattern, string.Empty, RegexOptions.IgnoreCase);
				sText = HttpUtility.HtmlEncode(sText);
				//sText = HttpUtility.UrlEncode(sText)
				return sText;
			}
		}
	}

}


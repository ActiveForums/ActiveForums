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

namespace DotNetNuke.Modules.ActiveForums
{
	public class UserRolesDictionary
	{
		internal static string GetRoles(string key)
		{
			try
			{
				object obj = DataCache.CacheRetrieve("afuserroles");
				if (obj != null)
				{
					Dictionary<string, string> dict = (Dictionary<string, string>)obj;
					if (dict.ContainsKey(key))
					{
						return dict[key];
					}
					else
					{
						return string.Empty;
					}
				}
				else
				{
					return string.Empty;
				}
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}
		internal static bool AddRoles(string key, string v)
		{
			try
			{
				object obj = DataCache.CacheRetrieve("afuserroles");
				Dictionary<string, string> dict = null;
				if (obj == null)
				{
					dict = new Dictionary<string, string>();
				}
				else
				{
					dict = (Dictionary<string, string>)obj;
				}
				if (dict.ContainsKey(key))
				{
					dict[key] = v;
				}
				else
				{
					dict.Add(key, v);
				}
				DataCache.CacheStore("afuserroles", dict, DateTime.Now.AddMinutes(3));
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}

		}
	}
}


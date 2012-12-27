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


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
using System.Text;

namespace DotNetNuke.Modules.ActiveForums
{
	public class UsersOnline
	{
		public string GetUsersOnline(int portalId, int moduleId, User user)
		{
			var sb = new StringBuilder();
			var dr = DataProvider.Instance().Profiles_GetUsersOnline(portalId, moduleId, 2);
			try
			{
				var mainSettings = DataCache.MainSettings(moduleId);
				
                while (dr.Read())
				{
                    if(sb.Length > 0)
                        sb.Append(", ");

				    sb.Append(UserProfiles.GetDisplayName(moduleId, true, false, user.IsAdmin || user.IsSuperUser, dr.GetInt("UserId"), dr.GetString("Username"), dr.GetString("FirstName"), dr.GetString("LastName"), dr.GetString("DisplayName")));
				}

				dr.Close();
				return sb.ToString();
			}
			catch (Exception ex)
			{
				if (! dr.IsClosed)
				{
					dr.Close();
				}
				return string.Empty;
			}


		}

	}
}

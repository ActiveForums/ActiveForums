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

using System;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
	public class UsersOnline
	{
		public string GetUsersOnline(int PortalId, int ModuleId, int UserId)
		{

			var sb = new System.Text.StringBuilder();
			IDataReader dr = DataProvider.Instance().Profiles_GetUsersOnline(PortalId, ModuleId, 2);
			try
			{
				SettingsInfo MainSettings = DataCache.MainSettings(ModuleId);
				while (dr.Read())
				{
					var ai = new Author
					             {
					                 AuthorId = Convert.ToInt32(dr["UserId"]),
					                 DisplayName = dr["DisplayName"].ToString(),
					                 Email = dr["Email"].ToString(),
					                 FirstName = dr["FirstName"].ToString(),
					                 LastName = dr["LastName"].ToString(),
					                 Username = dr["Username"].ToString()
					             };
				    sb.Append(UserProfiles.GetDisplayName(ModuleId, MainSettings.MemberListMode, false, ai.AuthorId, MainSettings.UserNameDisplay, ai));
					sb.Append(", ");

				}
				if (sb.Length > 3)
				{
					sb.Remove(sb.Length - 2, 2);
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

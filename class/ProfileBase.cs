using System;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ProfileBase : SettingsBase
	{
		private int _UID = -1;
	    public UserProfileInfo UserProfile { get; set; }

	    public int UID
		{
			get
			{
				if (Request.Params["UID"] != null)
				{
					if (SimulateIsNumeric.IsNumeric(Request.Params["UID"]))
					{
						_UID = Convert.ToInt32(Request.Params["UID"]);
					}
				}
				else
				{
					_UID = UserId;
				}
				return _UID;
			}
		}

	}
}

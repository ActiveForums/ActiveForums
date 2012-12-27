//ORIGINAL LINE: Imports System.Web.HttpContext

using System;
using System.Web;
namespace DotNetNuke.Modules.ActiveForums
{
	public class AMPageBase : Framework.PageBase
	{
		public static int _AFModId;
		public static int AFModID
		{
			get
			{
				return _AFModId;
			}
			set
			{
				_AFModId = value;
			}
		}
		//Public ReadOnly Property PortalId() As Integer
		//    Get
		//        Return CInt(Request.QueryString["pid"])
		//    End Get
		//End Property
		public static SettingsInfo MainSettings
		{
			get
			{
				var sb = new SettingsBase {ForumModuleId = AFModID};
			    return sb.MainSettings;
			}
		}
		public static int TabId
		{
			get
			{
				return Convert.ToInt32(HttpContext.Current.Request.QueryString["tabid"]);
			}
		}


	}
}

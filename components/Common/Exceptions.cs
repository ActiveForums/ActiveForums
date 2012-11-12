using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI;

namespace DotNetNuke.Modules.ActiveForums
{
	public class Exceptions
	{
		public static void LogException(Exception ex)
		{
			if (! ((ex) is System.Threading.ThreadAbortException))
			{
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
			}

		}
		public static void ModuleException(ref Control ctl, Exception ex)
		{
			if (! ((ex) is System.Threading.ThreadAbortException))
			{
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(ctl, ex);
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class FilterInfo
	{
		public int FilterId {get; set;}
		public string Find {get; set;}
		public string Replace {get; set;}
		public string FilterType {get; set;}
		public int PortalId {get; set;}
		public int ModuleId {get; set;}
	}
}


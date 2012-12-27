using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class FilterController
	{
		public FilterInfo Filter_Save(FilterInfo filter)
		{
			int filterId = DataProvider.Instance().Filters_Save(filter.PortalId, filter.ModuleId, filter.FilterId, filter.Find, filter.Replace, filter.FilterType);
			return Filter_Get(filter.PortalId, filter.ModuleId, filterId);
		}
		public void Filter_Delete(int PortalId, int ModuleId, int FilterId)
		{
			DataProvider.Instance().Filters_Delete(PortalId, ModuleId, FilterId);
		}
		public FilterInfo Filter_Get(int PortalId, int ModuleID, int FilterId)
		{
			FilterInfo fi = new FilterInfo();
			IDataReader dr = DataProvider.Instance().Filters_Get(PortalId, ModuleID, FilterId);
			while (dr.Read())
			{
				fi.FilterId = Convert.ToInt32(dr["FilterId"].ToString());
				fi.Find = dr["Find"].ToString();
				fi.Replace = dr["Replace"].ToString();
				fi.FilterType = dr["FilterType"].ToString();
				fi.ModuleId = ModuleID;
				fi.PortalId = PortalId;

			}
			dr.Close();
			return fi;
		}
	}
}


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


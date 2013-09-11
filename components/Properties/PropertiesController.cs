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
using System.Text;

namespace DotNetNuke.Modules.ActiveForums
{
	public class PropertiesController
	{
		internal PropertiesInfo SaveProperty(PropertiesInfo pi)
		{
			Data.PropertiesDB db = new Data.PropertiesDB();
			pi.PropertyId = db.SaveProperty(pi.PropertyId, pi.PortalId, pi.ObjectType, pi.ObjectOwnerId, pi.Name, pi.DataType, pi.DefaultAccessControl, pi.IsHidden, pi.IsRequired, pi.IsReadOnly, pi.ValidationExpression, pi.EditTemplate, pi.ViewTemplate, pi.SortOrder, pi.DefaultValue);
			return GetProperty(pi.PropertyId, pi.PortalId);
		}
		internal List<PropertiesInfo> ListProperties(int PortalId, int ObjectType, int ObjectOwnerId)
		{
			List<PropertiesInfo> list = new List<PropertiesInfo>();
			Data.PropertiesDB db = new Data.PropertiesDB();
			using (IDataReader dr = db.ListProperties(PortalId, ObjectType, ObjectOwnerId))
			{
				while (dr.Read())
				{
					PropertiesInfo p = FillObject(dr);
					if (p != null)
					{
						list.Add(p);
					}
				}
				dr.Close();
			}
			return list;
		}
		internal void DeleteProperty(int PortalId, int PropertyId)
		{
			Data.PropertiesDB db = new Data.PropertiesDB();
			db.DeleteProperty(PortalId, PropertyId);
		}
		internal string ListPropertiesJSON(int PortalId, int ObjectType, int ObjectOwnerId)
		{
			List<PropertiesInfo> list = new List<PropertiesInfo>();
			list = this.ListProperties(PortalId, ObjectType, ObjectOwnerId);
			StringBuilder sb = new StringBuilder();
			foreach (PropertiesInfo p in list)
			{
				sb.Append("{");
				sb.Append(Utilities.JSON.Pair("PropertyId", p.PropertyId.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("PortalId", p.PortalId.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("ObjectType", p.ObjectType.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("ObjectOwnerId", p.ObjectOwnerId.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("Name", p.Name.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("DataType", p.DataType.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("DefaultAccessControl", Convert.ToInt32(p.DefaultAccessControl).ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("IsHidden", p.IsHidden.ToString().ToLowerInvariant()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("IsReadOnly", p.IsReadOnly.ToString().ToLowerInvariant()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("IsRequired", p.IsRequired.ToString().ToLowerInvariant()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("ValidationExpression", HttpUtility.UrlEncode(HttpUtility.HtmlEncode(p.ValidationExpression.ToString()))));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("ViewTemplate", p.ViewTemplate.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("EditTemplate", p.EditTemplate.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("SortOrder", p.SortOrder.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("DefaultValue", p.DefaultValue.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("Label", HttpUtility.HtmlEncode("[RESX:" + p.Name + "]")));
				sb.Append("},");

			}
			if (sb.Length > 2)
			{
				sb.Remove(sb.Length - 1, 1);
			}
			return sb.ToString();
		}
		internal PropertiesInfo GetProperty(int PropertyId, int PortalId)
		{
			Data.PropertiesDB db = new Data.PropertiesDB();
			PropertiesInfo pi = null;
			using (IDataReader dr = db.GetProperties(PropertyId, PortalId))
			{
				while (dr.Read())
				{
					pi = FillObject(dr);
				}
				dr.Close();
			}
			return pi;
		}
		private PropertiesInfo FillObject(IDataRecord dr)
		{
			PropertiesInfo pi = null;
			if (dr != null)
			{
				pi = new PropertiesInfo();
				pi.PropertyId = Convert.ToInt32(dr["PropertyId"].ToString());
				pi.PortalId = Convert.ToInt32(dr["PortalId"].ToString());
				pi.ObjectType = Convert.ToInt32(dr["ObjectType"].ToString());
				pi.ObjectOwnerId = Convert.ToInt32(dr["ObjectOwnerId"].ToString());
				pi.Name = dr["Name"].ToString();
				pi.DataType = dr["DataType"].ToString();
				pi.DefaultAccessControl = Convert.ToInt32(dr["DefaultAccessControl"].ToString());
				pi.IsHidden = bool.Parse(dr["IsHidden"].ToString());
				pi.IsReadOnly = bool.Parse(dr["IsReadOnly"].ToString());
				pi.IsRequired = bool.Parse(dr["IsRequired"].ToString());
				pi.ValidationExpression = dr["ValidationExpression"].ToString();
				pi.ViewTemplate = dr["ViewTemplate"].ToString();
				pi.EditTemplate = dr["EditTemplate"].ToString();
				pi.SortOrder = int.Parse(dr["SortOrder"].ToString());
				pi.DefaultValue = dr["DefaultValue"].ToString();
			}
			return pi;
		}
	}
}
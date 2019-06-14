//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
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

using Microsoft.ApplicationBlocks.Data;
namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class PropertiesDB : Connection
	{
		internal IDataReader ListProperties(int PortalId, int ObjectType, int ObjectOwnerId)
		{
			return (IDataReader)(SqlHelper.ExecuteReader(connectionString, dbPrefix + "Properties_List", PortalId, ObjectType, ObjectOwnerId));
		}
		internal IDataReader GetProperties(int PropertyId, int PortalId)
		{
			return (IDataReader)(SqlHelper.ExecuteReader(connectionString, dbPrefix + "Properties_Get", PropertyId, PortalId));
		}
		internal int SaveProperty(int PropertyId, int PortalId, int ObjectType, int ObjectOwnerId, string Name, string DataType, int DefaultAccessControl, bool IsHidden, bool IsRequired, bool IsReadOnly, string ValidationExpression, string EditTemplate, string ViewTemplate, int SortOrder, string DefaultValue)
		{
			return Convert.ToInt32(SqlHelper.ExecuteScalar(connectionString, dbPrefix + "Properties_Save", PropertyId, PortalId, ObjectType, ObjectOwnerId, Name, DataType, DefaultAccessControl, IsHidden, IsRequired, ValidationExpression, EditTemplate, ViewTemplate, IsReadOnly, SortOrder, DefaultValue));
		}
		internal void SortRebuild(int PortalId, int ObjectType, int ObjectOwnerId)
		{
			SqlHelper.ExecuteNonQuery(connectionString, dbPrefix + "Properties_RebuildSort", PortalId, ObjectType, ObjectOwnerId);
		}
		internal void DeleteProperty(int PortalId, int PropertyId)
		{
			SqlHelper.ExecuteNonQuery(connectionString, dbPrefix + "Properties_DeleteDefTopicProp", PortalId, PropertyId);
		}
	}
}

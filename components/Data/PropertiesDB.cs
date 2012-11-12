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

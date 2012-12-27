//© 2004 - 2010 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Microsoft.ApplicationBlocks.Data;
namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class Groups : DataConfig
	{
		//Public Function Forums_List(ByVal PortalId As Integer, ByVal ModuleId As Integer) As IDataReader
		//    Return SqlHelper.ExecuteReader(_connectionString, dbPrefix & "Forums_GetPermissions", PortalId, ModuleId)
		//End Function
		public IDataReader Groups_Get(int ModuleId, int ForumGroupId)
		{
			return SqlHelper.ExecuteReader(_connectionString, dbPrefix + "Groups_Get", ModuleId, ForumGroupId);
		}
	}
}
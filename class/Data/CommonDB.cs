using System.Data;

using Microsoft.ApplicationBlocks.Data;

namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class CommonDB : Connection
	{
		public IDataReader ForumContent_List(int PortalId, int ModuleId, int ForumGroupId, int ForumId, int ParentForumId)
		{
			return SqlHelper.ExecuteReader(connectionString, dbPrefix + "ForumContent_List", PortalId, ModuleId, ForumGroupId, ForumId, ParentForumId);
		}
	}
}


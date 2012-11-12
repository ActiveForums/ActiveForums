namespace DotNetNuke.Modules.ActiveForums
{
	public class ModController
	{
		internal void Mod_Reject(int PortalId, int ModuleId, int UserId, int ForumId, int TopicId, int ReplyId)
		{
			DataProvider.Instance().Mod_Reject(PortalId, ModuleId, UserId, ForumId, TopicId, ReplyId, 0, string.Empty);
		}
	}
}


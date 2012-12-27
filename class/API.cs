using System;

namespace DotNetNuke.Modules.ActiveForums.API
{
	public class Content
	{
		public int Topic_QuickCreate(int PortalId, int ModuleId, int ForumId, string Subject, string Body, int UserId, string DisplayName, bool IsApproved, string IPAddress)
		{
			try
			{
				var tc = new TopicsController();
				return tc.Topic_QuickCreate(PortalId, ModuleId, ForumId, Subject, Body, UserId, DisplayName, IsApproved, IPAddress);
			}
			catch (Exception ex)
			{
				return -1;
			}
		}
		public int Reply_QuickCreate(int PortalId, int ModuleId, int ForumId, int TopicId, int ReplyToId, string Subject, string Body, int UserId, string DisplayName, bool IsApproved, string IPAddress)
		{
			try
			{
				var rc = new ReplyController();
				return rc.Reply_QuickCreate(PortalId, ModuleId, ForumId, TopicId, ReplyToId, Subject, Body, UserId, DisplayName, IsApproved, IPAddress);
			}
			catch (Exception ex)
			{
				return -1;
			}
		}
	}
	public class ForumGroups
	{

	}
	public class Forums
	{
		public int Forums_Save(int PortalId, Forum fi, bool isNew, bool UseGroup)
		{
			try
			{
				var fc = new ForumController();
				return fc.Forums_Save(PortalId, fi, isNew, UseGroup);
			}
			catch (Exception ex)
			{
				return -1;
			}
		}
	}
	public class Rewards
	{

	}
}

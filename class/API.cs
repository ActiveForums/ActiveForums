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

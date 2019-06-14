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

namespace DotNetNuke.Modules.ActiveForums
{
	public class DailyDigest
//#if SKU_ENTERPRISE
		 : DotNetNuke.Services.Scheduling.SchedulerClient
{
		public DailyDigest(DotNetNuke.Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem) : base()
		{
			this.ScheduleHistoryItem = objScheduleHistoryItem;
		}
		public override void DoWork()
		{
			try
			{


				Subscriptions.SendSubscriptions(SubscriptionTypes.DailyDigest, DateTime.Now);
				ScheduleHistoryItem.Succeeded = true;
				ScheduleHistoryItem.TimeLapse = GetElapsedTimeTillNextStart();
				ScheduleHistoryItem.AddLogNote("Daily Digest Complete");

			}
			catch (Exception ex)
			{
				ScheduleHistoryItem.Succeeded = false;
				ScheduleHistoryItem.AddLogNote("Daily Digest Failed: " + ex.ToString());
				Errored(ref ex);
				DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
			}
		}

		private static int GetElapsedTimeTillNextStart()
		{
			DateTime NextRun = DateTime.Now.AddDays(1);
			DateTime nextStart = new DateTime(NextRun.Year, NextRun.Month, NextRun.Day, 18, 0, 0);
			int elapseMinutes = Convert.ToInt32((nextStart.Ticks - DateTime.Now.Ticks) / TimeSpan.TicksPerDay);
			return elapseMinutes;
		}

//#else
//    {
//#endif


	}
}
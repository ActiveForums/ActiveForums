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
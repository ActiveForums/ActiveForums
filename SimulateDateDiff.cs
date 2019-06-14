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

// original copyright
//----------------------------------------------------------------------------------------
//	Copyright © 2003 - 2011 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class simulates the behavior of the classic VB 'DateDiff' function.
//----------------------------------------------------------------------------------------
public static class SimulateDateDiff
{
	public enum DateInterval
	{
		Day,
		DayOfYear,
		Hour,
		Minute,
		Month,
		Quarter,
		Second,
		Weekday,
		WeekOfYear,
		Year
	}

	public static long DateDiff(DateInterval intervalType, System.DateTime dateOne, System.DateTime dateTwo)
	{
		switch (intervalType)
		{
			case DateInterval.Day:
			case DateInterval.DayOfYear:
				System.TimeSpan spanForDays = dateTwo - dateOne;
				return (long)spanForDays.TotalDays;
			case DateInterval.Hour:
				System.TimeSpan spanForHours = dateTwo - dateOne;
				return (long)spanForHours.TotalHours;
			case DateInterval.Minute:
				System.TimeSpan spanForMinutes = dateTwo - dateOne;
				return (long)spanForMinutes.TotalMinutes;
			case DateInterval.Month:
				return ((dateTwo.Year - dateOne.Year) * 12) + (dateTwo.Month - dateOne.Month);
			case DateInterval.Quarter:
				long dateOneQuarter = (long)System.Math.Ceiling(dateOne.Month / 3.0);
				long dateTwoQuarter = (long)System.Math.Ceiling(dateTwo.Month / 3.0);
				return (4 * (dateTwo.Year - dateOne.Year)) + dateTwoQuarter - dateOneQuarter;
			case DateInterval.Second:
				System.TimeSpan spanForSeconds = dateTwo - dateOne;
				return (long)spanForSeconds.TotalSeconds;
			case DateInterval.Weekday:
				System.TimeSpan spanForWeekdays = dateTwo - dateOne;
				return (long)(spanForWeekdays.TotalDays / 7.0);
			case DateInterval.WeekOfYear:
				System.DateTime dateOneModified = dateOne;
				System.DateTime dateTwoModified = dateTwo;
				while (dateTwoModified.DayOfWeek != System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek)
				{
					dateTwoModified = dateTwoModified.AddDays(-1);
				}
				while (dateOneModified.DayOfWeek != System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek)
				{
					dateOneModified = dateOneModified.AddDays(-1);
				}
				System.TimeSpan spanForWeekOfYear = dateTwoModified - dateOneModified;
				return (long)(spanForWeekOfYear.TotalDays / 7.0);
			case DateInterval.Year:
				return dateTwo.Year - dateOne.Year;
			default:
				return 0;
		}
	}
}
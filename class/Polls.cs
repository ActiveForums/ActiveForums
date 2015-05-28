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
using System.Data;

using System.Text;
namespace DotNetNuke.Modules.ActiveForums
{
	public class Polls
	{
		public string PollResults(int TopicId, string ImagePath)
		{
			int BarWidth = 275;
			var sb = new StringBuilder();
			sb.Append("<table width=\"80%\" align=\"center\" cellpadding=\"4\" cellspacing=\"0\" class=\"afpollresults\">");
			IDataReader dr;
			dr = DataProvider.Instance().Poll_GetResults(TopicId);
			dr.Read();
			sb.Append("<tr><td colspan=\"2\" class=\"afnormal\"><b>");
			sb.Append(Convert.ToString(dr["Question"]));
			sb.Append("</b></td></tr>");
			dr.NextResult();
			dr.Read();
			double VoteCount;
			VoteCount = Convert.ToDouble(dr[0]);
			dr.NextResult();
			while (dr.Read())
			{
				double dblPercent = 0;
				if (VoteCount != 0)
				{
					dblPercent = Convert.ToDouble(Convert.ToDouble(dr["ResultCount"]) / VoteCount);
				}
				sb.Append("<tr><td class=\"afnormal\"><b>");
				sb.Append(Convert.ToString(dr["OptionName"]) + "</b> (" + Convert.ToString(dr["ResultCount"]) + ")");
				sb.Append("</td></tr><tr><td class=\"afnormal\">");
				sb.Append("<span class=\"afpollbar\">");
				sb.Append("<img src=\"" + ImagePath + "/spacer.gif\" style=\"height:10px !important;max-height:10px !important;\" height=\"10\" width=\"" + Convert.ToInt32((BarWidth * dblPercent)) + "\" />");
				sb.Append("</span>&nbsp;" + Convert.ToInt32(dblPercent * 100).ToString() + "%");
				sb.Append("</td></tr>");
			}
			sb.Append("</table>");
			return sb.ToString();
		}

		protected internal bool HasVoted(int TopicId, int UserId)
		{
			try
			{
				int iVote = DataProvider.Instance().Poll_HasVoted(TopicId, UserId);
				if (iVote > 0)
				{
					return true;
				}
			    return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

	}

}


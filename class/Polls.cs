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
				sb.Append("<img src=\"" + ImagePath + "/spacer.gif\" height=\"10\" width=\"" + Convert.ToInt32((BarWidth * dblPercent)) + "\" />");
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


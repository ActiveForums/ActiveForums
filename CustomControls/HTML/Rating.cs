using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Text;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class Ratings
	{
		private int _TopicId = -1;
		public int TopicId
		{
			get
			{
				return _TopicId;
			}
			set
			{
				_TopicId = value;
			}
		}
		private bool _Enabled = false;
		public bool Enabled
		{
			get
			{
				return _Enabled;
			}
			set
			{
				_Enabled = value;
			}
		}
		private int _Rating = -1;
		public int Rating
		{
			get
			{
				return _Rating;
			}
			set
			{
				_Rating = value;
			}
		}
		public Ratings(int t, bool enable, int r)
		{
			TopicId = t;
			Enabled = enable;
			Rating = r;
		}
		public string Render()
		{
			StringBuilder sb = new StringBuilder();
			if (Rating == -1)
			{
				Rating = DataProvider.Instance().Topics_GetRating(TopicId);
			}
			if (Enabled)
			{
				sb.Append("<ul id=\"af-rater\" class=\"af-rater ");
			}
			else
			{
				sb.Append("<ul class=\"af-rater ");
			}

			if (Rating > 0)
			{
				sb.Append(" rate" + Rating.ToString());
			}
			sb.Append("\">");
			if (Enabled)
			{
				sb.Append("<li onmouseover=\"amaf_hoverRate(this,1);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(1," + TopicId.ToString() + ");\">&nbsp;</li>");
				sb.Append("<li onmouseover=\"amaf_hoverRate(this,2);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(2," + TopicId.ToString() + ");\">&nbsp;</li>");
				sb.Append("<li onmouseover=\"amaf_hoverRate(this,3);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(3," + TopicId.ToString() + ");\">&nbsp;</li>");
				sb.Append("<li onmouseover=\"amaf_hoverRate(this,4);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(4," + TopicId.ToString() + ");\">&nbsp;</li>");
				sb.Append("<li onmouseover=\"amaf_hoverRate(this,5);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(5," + TopicId.ToString() + ");\">&nbsp;</li>");
			}
			else
			{
				sb.Append("<li>&nbsp;</li>");
				sb.Append("<li>&nbsp;</li>");
				sb.Append("<li>&nbsp;</li>");
				sb.Append("<li>&nbsp;</li>");
				sb.Append("<li>&nbsp;</li>");
			}

			sb.Append("</ul>");
			if (Enabled)
			{
				sb.Append("<input type=\"hidden\" value=\"" + Rating.ToString() + "\" id=\"af-rate-value\" />");
			}

			return sb.ToString();
		}
	}
}


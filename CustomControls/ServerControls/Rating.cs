using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	[ToolboxData("<{0}:Rating runat=server></{0}:Rating>")]
	public class Rating : WebControl
	{
		private double _rating = 0;
		private string _ratingCSS = "afrate";
		private int _topicId = -1;
		private int _userId = -1;
		protected Callback cb = new Callback();
		
        public double RatingValue
		{
			get
			{
				return _rating;
			}
			set
			{
				_rating = value;
			}
		}
		public string RatingCSS
		{
			get
			{
				return _ratingCSS;
			}
			set
			{
				_ratingCSS = value;
			}
		}
		public int TopicId
		{
			get
			{
				return _topicId;
			}
			set
			{
				_topicId = value;
			}
		}
		public int UserId
		{
			get
			{
				return _userId;
			}
			set
			{
				_userId = value;
			}
		}
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            cb.CallbackEvent += new Callback.CallbackEventHandler(cb_Callback);

			if (Enabled)
			{
				cb.ID = "cb";
				this.Controls.Add(cb);
			}

		}
		protected override void Render(HtmlTextWriter writer)
		{
			Literal lit = new Literal();
			lit.Text = RenderRating();
			if (Enabled)
			{
				CallBackContent cbContent = new CallBackContent();
				cbContent.Controls.Add(lit);
				cb.Content = cbContent;

				cb.RenderControl(writer);
			}
			else
			{
				lit.RenderControl(writer);
			}


		}

		private string RenderRating()
		{
			if (RatingValue > 0)
			{
				if ((Math.Round(RatingValue, 0)) == 1)
				{
						RatingCSS += " onepos";
				}
				else if ((Math.Round(RatingValue, 0)) == 2)
				{
						RatingCSS += " twopos";
				}
				else if ((Math.Round(RatingValue, 0)) == 3)
				{
						RatingCSS += " threepos";
				}
				else if ((Math.Round(RatingValue, 0)) == 4)
				{
						RatingCSS += " fourpos";
				}
				else if ((Math.Round(RatingValue, 0)) == 5)
				{
						RatingCSS += " fivepos";
				}
			}
			if (RatingValue == 0 && Enabled == false)
			{
				return string.Empty;
			}
			StringBuilder sb = new StringBuilder();
			sb.Append("<div class=\"" + CssClass + "\">");
			sb.Append("<ul class=\"" + RatingCSS + "\">");
			if (Enabled)
			{
				sb.Append("<li class=\"one\"><a title=\"[RESX:Rate:1Star]\" href=\"#\" onclick=\"" + cb.ClientID + ".Callback(1); return false;\">1</a></li>");
				sb.Append("<li class=\"two\"><a title=\"[RESX:Rate:2Star]\" href=\"#\" onclick=\"" + cb.ClientID + ".Callback(2); return false;\">2</a></li>");
				sb.Append("<li class=\"three\"><a title=\"[RESX:Rate:3Star]\" href=\"#\" onclick=\"" + cb.ClientID + ".Callback(3); return false;\">3</a></li>");
				sb.Append("<li class=\"four\"><a title=\"[RESX:Rate:4Star]\" href=\"#\" onclick=\"" + cb.ClientID + ".Callback(4); return false;\">4</a></li>");
				sb.Append("<li class=\"five\"><a title=\"[RESX:Rate:5Star]\" href=\"#\" onclick=\"" + cb.ClientID + ".Callback(5); return false;\">5</a></li>");
			}
			else
			{
				sb.Append("<li class=\"one\">1</li>");
				sb.Append("<li class=\"two\">2</li>");
				sb.Append("<li class=\"three\">3</li>");
				sb.Append("<li class=\"four\">4</li>");
				sb.Append("<li class=\"five\">5</li>");
			}

			sb.Append("</ul>");
			sb.Append("</div>");
			return sb.ToString();


		}


		private void cb_Callback(object sender, CallBackEventArgs e)
		{
			Data.Topics db = new Data.Topics();
			if (e.Parameters.Length > 0)
			{
				int rate = Convert.ToInt32(e.Parameter);
				if (rate >= 1 && rate <= 5)
				{
					RatingValue = db.Topics_AddRating(TopicId, UserId, rate, string.Empty, HttpContext.Current.Request.UserHostAddress.ToString());
				}
			}

			CallBackContent cbContent = new CallBackContent();
			cbContent.Controls.Add(new LiteralControl(RenderRating()));
			cb.Content = cbContent;
			cb.Content.RenderControl(e.Output);
		}
	}
}


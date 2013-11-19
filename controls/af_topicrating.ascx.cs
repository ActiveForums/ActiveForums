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
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_topicrating : ForumBase
    {
        public string RatingClass = "rating0";
        #region Private Members
        private int _Rating = -1;
        private bool _Enabled = false;
        #endregion
        #region Controls
        protected ImageButton Rate1 = new ImageButton();
        protected ImageButton Rate2 = new ImageButton();
        protected ImageButton Rate3 = new ImageButton();
        protected ImageButton Rate4 = new ImageButton();
        protected ImageButton Rate5 = new ImageButton();

        #endregion
        #region Public Properties
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
        #endregion
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Rate1.Click += Rate1_Click;
            Rate2.Click += Rate2_Click;
            Rate3.Click += Rate3_Click;
            Rate4.Click += Rate4_Click;
            Rate5.Click += Rate5_Click;
            cbRating.CallbackEvent += cbRating_Callback;

        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            RenderRating();
            string sRating = "function afchangerate(rate){var rd = document.getElementById('ratingdiv');rd.className=rate;};";
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "afratescript", sRating, true);
        }

        private void Rate1_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DataProvider.Instance().Topics_AddRating(TopicId, UserId, 1, string.Empty, Request.UserHostAddress.ToString());
        }

        private void Rate2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DataProvider.Instance().Topics_AddRating(TopicId, UserId, 2, string.Empty, Request.UserHostAddress.ToString());
        }

        private void Rate3_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DataProvider.Instance().Topics_AddRating(TopicId, UserId, 3, string.Empty, Request.UserHostAddress.ToString());
        }

        private void Rate4_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DataProvider.Instance().Topics_AddRating(TopicId, UserId, 4, string.Empty, Request.UserHostAddress.ToString());
        }

        private void Rate5_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DataProvider.Instance().Topics_AddRating(TopicId, UserId, 5, string.Empty, Request.UserHostAddress.ToString());
        }
        private void cbRating_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            if (e.Parameters.Length > 0)
            {
                int rate = Convert.ToInt32(e.Parameter);
                if (rate >= 1 && rate <= 5)
                {
                    DataProvider.Instance().Topics_AddRating(TopicId, UserId, rate, string.Empty, Request.UserHostAddress.ToString());
                }
            }
            Rating = -1;
            RenderRating();
            plhRating.RenderControl(e.Output);
        }
        #endregion
        #region Private Methods
        private void RenderRating()
        {
            if (Rating == -1)
            {
                Rating = DataProvider.Instance().Topics_GetRating(TopicId);
            }
            RatingClass = "rating" + Rating.ToString();
            plhRating.Controls.Clear();
            Rate1.Attributes.Add("onmouseover", "afchangerate('rating1');");
            Rate1.Enabled = Enabled;
            Rate2.Attributes.Add("onmouseover", "afchangerate('rating2');");
            Rate2.Enabled = Enabled;
            Rate3.Attributes.Add("onmouseover", "afchangerate('rating3');");
            Rate3.Enabled = Enabled;
            Rate4.Attributes.Add("onmouseover", "afchangerate('rating4');");
            Rate4.Enabled = Enabled;
            Rate5.Attributes.Add("onmouseover", "afchangerate('rating5');");
            Rate5.Enabled = Enabled;

            Literal lit = new Literal();
            lit.Text = "<div class=\"" + RatingClass + "\" id=\"ratingdiv\" onmouseout=\"this.className='" + RatingClass + "'\">";
            plhRating.Controls.Add(lit);
            Rate1.ID = "Rate1";
            Rate1.CausesValidation = false;
            Rate1.Width = 13;
            Rate1.Height = 14;
            Rate1.ImageUrl = "~/DesktopModules/ActiveForums/images/spacer.gif";
            plhRating.Controls.Add(Rate1);
            Rate2.ID = "Rate2";
            Rate2.CausesValidation = false;
            Rate2.Width = 14;
            Rate2.Height = 14;
            Rate2.ImageUrl = "~/DesktopModules/ActiveForums/images/spacer.gif";
            plhRating.Controls.Add(Rate2);
            Rate3.ID = "Rate3";
            Rate3.CausesValidation = false;
            Rate3.Width = 14;
            Rate3.Height = 14;
            Rate3.ImageUrl = "~/DesktopModules/ActiveForums/images/spacer.gif";
            plhRating.Controls.Add(Rate3);
            Rate4.ID = "Rate4";
            Rate4.CausesValidation = false;
            Rate4.Width = 14;
            Rate4.Height = 14;
            Rate4.ImageUrl = "~/DesktopModules/ActiveForums/images/spacer.gif";
            plhRating.Controls.Add(Rate4);
            Rate5.ID = "Rate5";
            Rate5.CausesValidation = false;
            Rate5.Width = 14;
            Rate5.Height = 14;
            Rate5.ImageUrl = "~/DesktopModules/ActiveForums/images/spacer.gif";

            plhRating.Controls.Add(Rate5);
            lit = new Literal();
            lit.Text = "</div>";
            plhRating.Controls.Add(lit);
            if (UseAjax)
            {
                Rate1.OnClientClick = "af_rateTopic(1);return false;";
                Rate2.OnClientClick = "af_rateTopic(2);return false;";
                Rate3.OnClientClick = "af_rateTopic(3);return false;";
                Rate4.OnClientClick = "af_rateTopic(4);return false;";
                Rate5.OnClientClick = "af_rateTopic(5);return false;";
                AddRatingScript();
            }
        }
        private void AddRatingScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("function af_rateTopic(rate){" + cbRating.ClientID + ".Callback(rate);};");
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "afrate", sb.ToString(), true);
        }
        #endregion

    }
}

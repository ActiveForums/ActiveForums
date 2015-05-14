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
                sb.Append("<ul id=\"af-rater\" class=\"fa-rater ");
            }
            else
            {
                sb.Append("<ul class=\"fa-rater ");
            }

            if (Rating > 0)
            {
                sb.Append(" fa-rate" + Rating.ToString());
            }
            sb.Append("\">");
            if (Enabled)
            {
                /*sb.Append("<li onmouseover=\"amaf_hoverRate(this,1);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(1," + TopicId.ToString() + ");\">&nbsp;</li>");
                sb.Append("<li onmouseover=\"amaf_hoverRate(this,2);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(2," + TopicId.ToString() + ");\">&nbsp;</li>");
                sb.Append("<li onmouseover=\"amaf_hoverRate(this,3);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(3," + TopicId.ToString() + ");\">&nbsp;</li>");
                sb.Append("<li onmouseover=\"amaf_hoverRate(this,4);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(4," + TopicId.ToString() + ");\">&nbsp;</li>");
                sb.Append("<li onmouseover=\"amaf_hoverRate(this,5);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(5," + TopicId.ToString() + ");\">&nbsp;</li>");*/

                sb.Append("<li onmouseover=\"amaf_hoverRate(this,1);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(1," + TopicId.ToString() + ");\"><i class=\"fa fa-star1\"></i></li>");
                sb.Append("<li onmouseover=\"amaf_hoverRate(this,2);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(2," + TopicId.ToString() + ");\"><i class=\"fa fa-star2\"></i></li>");
                sb.Append("<li onmouseover=\"amaf_hoverRate(this,3);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(3," + TopicId.ToString() + ");\"><i class=\"fa fa-star3\"></i></li>");
                sb.Append("<li onmouseover=\"amaf_hoverRate(this,4);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(4," + TopicId.ToString() + ");\"><i class=\"fa fa-star4\"></i></li>");
                sb.Append("<li onmouseover=\"amaf_hoverRate(this,5);\" onmouseout=\"amaf_hoverRate(this);\" onclick=\"amaf_changeRate(5," + TopicId.ToString() + ");\"><i class=\"fa fa-star5\"></i></li>");
            }
            else
            {
                sb.Append("<li><i class=\"fa fa-star1\"></i></li>");
                sb.Append("<li><i class=\"fa fa-star2\"></i></li>");
                sb.Append("<li><i class=\"fa fa-star3\"></i></li>");
                sb.Append("<li><i class=\"fa fa-star4\"></i></li>");
                sb.Append("<li><i class=\"fa fa-star5\"></i></li>");
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


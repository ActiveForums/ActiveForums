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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:pagernav runat=server></{0}:pagernav>")]
    public class PagerNav : WebControl
    {
        public enum Mode
        {
            Links,
            CallBack
        }

        public bool UseShortUrls { get; set; }

        public Mode PageMode { get; set; }

        public string BaseURL { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public int PageCount { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public int ForumID { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("0")]
        public int TopicId { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public int TabID { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string Text { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string View { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("Page:")]
        public string PageText { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("of")]
        public string OfText { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public int CurrentPage { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string[] Params { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string Optional2 { get; set; }

        public string ClientScript
        {
            get
            {
                return "afPage({0});";
            }
        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnableViewState = false;
        }

        protected override void Render(HtmlTextWriter output)
        {
            Text = RenderPager();
            output.Write(Text);
        }

        public string RenderPager()
        {
            var sb = new StringBuilder();

            PageMode = Mode.Links;

            if (!(string.IsNullOrEmpty(BaseURL)))
            {
                if (!(BaseURL.EndsWith("/")))
                {
                    BaseURL += "/";
                }
                if (!(BaseURL.StartsWith("/")))
                {
                    BaseURL = "/" + BaseURL;
                }
            }

            var qs = string.Empty;
            if (HttpContext.Current.Request.QueryString["ts"] != null)
            {
                qs = "?ts=" + HttpContext.Current.Request.QueryString["ts"];
            }

            if (PageCount > 1)
            {
                sb.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"afpager\"><tr>");
                sb.Append("<td class=\"af_pager\">" + PageText + " " + CurrentPage + " " + OfText + " " + PageCount + "</td>");
                if (CurrentPage != 1)
                {
                    if (PageMode == Mode.Links)
                    {
                        if (string.IsNullOrEmpty(BaseURL))
                        {
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + Utilities.NavigateUrl(TabID, "", BuildParams(View, ForumID, 1, TopicId)) + "\" title=\"First Page\"> &lt;&lt; </a></td>");
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + Utilities.NavigateUrl(TabID, "", BuildParams(View, ForumID, CurrentPage - 1, TopicId)) + "\" title=\"Previous Page\"> &lt; </a></td>");
                        }
                        else
                        {
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + BaseURL + qs + "\" title=\"First Page\"> &lt;&lt; </a></td>");
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + BaseURL + (CurrentPage - 1) + "/" + qs + "\" title=\"Previous Page\"> &lt; </a></td>");
                        }
                    }
                    else
                    {
                        sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, "1") + "\" title=\"First Page\"> &lt;&lt; </a></td>");
                        sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, CurrentPage - 1) + "\" title=\"Previous Page\"> &lt; </a></td>");
                    }
                }

                int iStart;
                int iMaxPage;

                if (CurrentPage <= 3)
                {
                    iStart = 1;
                    iMaxPage = 5;
                }
                else
                {
                    iStart = CurrentPage - 2;
                    iMaxPage = CurrentPage + 2;
                }

                if (iMaxPage > PageCount)
                {
                    iMaxPage = PageCount;
                }

                if (iMaxPage == PageCount)
                {
                    iStart = iMaxPage - 4;
                }

                if (iStart <= 0)
                {
                    iStart = 1;
                }

                int i;
                for (i = iStart; i <= iMaxPage; i++)
                {
                    if (i == CurrentPage)
                    {
                        sb.Append("<td class=\"af_currentpage\" style=\"text-align:center;\">" + i + "</td>");
                    }
                    else
                    {
                        if (PageMode == Mode.Links)
                        {
                            if (string.IsNullOrEmpty(BaseURL))
                            {
                                sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + Utilities.NavigateUrl(TabID, "", BuildParams(View, ForumID, i, TopicId)) + "\">" + i + "</a></td>");
                            }
                            else
                            {
                                if (i > 1)
                                    sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + BaseURL + i + "/" + qs + "\">" + i + "</a></td>");
                                else
                                    sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + BaseURL + qs + "\">" + i + "</a></td>");
                            }
                        }
                        else
                        {
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, i) + "\">" + i + "</a></td>");
                        }

                    }

                    if (i == PageCount)
                        break;

                }

                if (CurrentPage != PageCount)
                {
                    if (PageMode == Mode.Links)
                    {
                        if (string.IsNullOrEmpty(BaseURL))
                        {
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + Utilities.NavigateUrl(TabID, "", BuildParams(View, ForumID, CurrentPage + 1, TopicId)) + "\" title=\"Next Page\"> &gt;</a></td>");
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + Utilities.NavigateUrl(TabID, "", BuildParams(View, ForumID, PageCount, TopicId)) + "\" title=\"Last Page\"> &gt;&gt;</a></td>");
                        }
                        else
                        {
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + BaseURL + (CurrentPage + 1) + "/" + qs + "\" title=\"Next Page\"> &gt;</a></td>");
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + BaseURL + PageCount + "/" + qs + "\" title=\"Last Page\"> &gt;&gt;</a></td>");
                        }
                    }
                    else
                    {
                        sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, CurrentPage + 1) + "\" title=\"Next Page\"> &gt;</a></td>");
                        sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, PageCount) + "\"> &gt;&gt;</a></td>");
                    }
                }

                sb.Append("</tr></table>");
            }

            return sb.ToString();
        }

        private string[] BuildParams(string view, int forumID, int page, int postID = 0)
        {
            string[] params2;
            if (view.ToLowerInvariant() == Views.Topics.ToLowerInvariant())
            {
                if (page > 1)
                {
                    params2 = new[] { ParamKeys.ViewType + "=" + view, ParamKeys.ForumId + "=" + forumID, ParamKeys.PageId + "=" + page };
                }
                else
                {
                    params2 = new[] { ParamKeys.ViewType + "=" + view, ParamKeys.ForumId + "=" + forumID };
                }

                if (UseShortUrls && page > 1)
                {
                    params2 = new[] { ParamKeys.ForumId + "=" + forumID, ParamKeys.PageId + "=" + page };
                }
                else if (UseShortUrls && page == 1)
                {
                    params2 = new[] { ParamKeys.ForumId + "=" + forumID };
                }
                if (Params != null)
                {
                    var intLength = params2.Length;
                    Array.Resize(ref params2, (intLength + Params.Length));
                    Params.CopyTo(params2, intLength);
                }

            }
            else if (view.ToLowerInvariant() == Views.Topic.ToLowerInvariant())
            {
                if (page > 1)
                {
                    params2 = new[] { ParamKeys.ViewType + "=" + view, ParamKeys.ForumId + "=" + forumID, ParamKeys.TopicId + "=" + postID, ParamKeys.PageId + "=" + page };
                }
                else
                {
                    params2 = new[] { ParamKeys.ViewType + "=" + view, ParamKeys.ForumId + "=" + forumID, ParamKeys.TopicId + "=" + postID };
                }

                if (UseShortUrls && page > 1)
                {
                    params2 = new[] { ParamKeys.TopicId + "=" + postID, ParamKeys.PageId + "=" + page };
                }
                else if (UseShortUrls && page == 1)
                {
                    params2 = new[] { ParamKeys.TopicId + "=" + postID };
                }
                if (Params != null)
                {
                    var intLength = params2.Length;
                    Array.Resize(ref params2, (intLength + Params.Length));
                    Params.CopyTo(params2, intLength);
                }
            }
            else
            {
                if (Params != null)
                {
                    params2 = new[] { ParamKeys.ViewType + "=" + view, ParamKeys.PageId + "=" + page };
                    var intLength = params2.Length;
                    Array.Resize(ref params2, (intLength + Params.Length));
                    Params.CopyTo(params2, intLength);
                }
                else
                {
                    params2 = new[] { ParamKeys.ViewType + "=" + view, ParamKeys.PageId + "=" + page };
                }
            }
            return params2;
        }


    }
}
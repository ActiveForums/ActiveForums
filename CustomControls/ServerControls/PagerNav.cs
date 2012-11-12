using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Web.UI;
using System.Web;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:pagernav runat=server></{0}:pagernav>")]
    public class PagerNav : System.Web.UI.WebControls.WebControl
    {
        public enum Mode : int
        {
            Links,
            CallBack
        }
        private int _PageCount;
        private int _CurrentPage;
        private string _Text;
        private int _ForumID = 0;
        private int _TopicId;
        private int _TabID;
        private string _PageText;
        private string _OfText;
        private string _View;
        private string[] _Params;
        private string _Optional2 = "";
        private string _ClientScript = "afPage({0});";
        private Mode _Mode;
        private bool _useShortUrls = false;

        public bool UseShortUrls
        {
            get
            {
                return _useShortUrls;
            }
            set
            {
                _useShortUrls = value;
            }
        }
        public Mode PageMode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
            }
        }
        private string _BaseURL = string.Empty;
        public string BaseURL
        {
            get
            {
                return _BaseURL;
            }
            set
            {
                _BaseURL = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public int PageCount
        {
            get
            {
                return _PageCount;
            }

            set
            {
                _PageCount = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public int ForumID
        {
            get
            {
                return _ForumID;
            }

            set
            {
                _ForumID = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("0")]
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
        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public int TabID
        {
            get
            {
                return _TabID;
            }

            set
            {
                _TabID = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string Text
        {
            get
            {
                return _Text;
            }

            set
            {
                _Text = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string View
        {
            get
            {
                return _View;
            }

            set
            {
                _View = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("Page:")]
        public string PageText
        {
            get
            {
                return _PageText;
            }

            set
            {
                _PageText = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("of")]
        public string OfText
        {
            get
            {
                return _OfText;
            }

            set
            {
                _OfText = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public int CurrentPage
        {
            get
            {
                return _CurrentPage;
            }

            set
            {
                _CurrentPage = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string[] Params
        {
            get
            {
                return _Params;
            }

            set
            {
                _Params = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue("")]
        public string Optional2
        {
            get
            {
                return _Optional2;
            }

            set
            {
                _Optional2 = value;
            }
        }
        public string ClientScript
        {
            get
            {
                return _ClientScript;
            }
            set
            {
                _ClientScript = value;
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter output)
        {
            _Text = RenderPager();
            output.Write(_Text);
        }
        public string RenderPager()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int iMaxPage = CurrentPage + 2;
            if (iMaxPage > PageCount)
            {
                iMaxPage = PageCount;
            }
            int i = 1;
            int iStart = 1;
            if (HttpContext.Current.Request.Browser.IsMobileDevice)
            {
                PageMode = Mode.Links;
            }
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
            string qs = string.Empty;
            if (HttpContext.Current.Request.QueryString["ts"] != null)
            {
                qs = "?ts=" + HttpContext.Current.Request.QueryString["ts"];
            }
            //Dim iEnd As Integer
            if (PageCount > 1)
            {

                sb.Append("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"afpager\"><tr>");
                sb.Append("<td class=\"af_pager\">" + PageText + " " + CurrentPage + " " + OfText + " " + PageCount + "</td>");
                if (!(CurrentPage == 1))
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
                        sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, "1").ToString() + "\" title=\"First Page\"> &lt;&lt; </a></td>");
                        sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, CurrentPage - 1).ToString() + "\" title=\"Previous Page\"> &lt; </a></td>");

                    }


                    //Else
                    //    iMaxPage = iMaxPage + 1
                }
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
                                {
                                    sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + BaseURL + i + "/" + qs + "\">" + i + "</a></td>");
                                }
                                else
                                {
                                    sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"" + BaseURL + qs + "\">" + i + "</a></td>");
                                }

                            }

                        }
                        else
                        {
                            sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, i).ToString() + "\">" + i + "</a></td>");
                        }

                    }
                    if (i == PageCount)
                    {
                        break;
                    }
                }
                if (!(CurrentPage == PageCount))
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
                        sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, CurrentPage + 1).ToString() + "\" title=\"Next Page\"> &gt;</a></td>");
                        sb.Append("<td class=\"af_pagernumber\" style=\"text-align:center;\"><a href=\"javascript:" + string.Format(ClientScript, PageCount).ToString() + "\"> &gt;&gt;</a></td>");
                    }

                }
                sb.Append("</tr></table>");
            }
            return sb.ToString();
        }
        private string[] BuildParams(string View, int ForumID, int Page, int PostID = 0)
        {
            string[] Params2 = new string[0];
            if (View.ToLowerInvariant() == Views.Topics.ToLowerInvariant())
            {
                if (Page > 1)
                {
                    Params2 = new string[] { ParamKeys.ViewType + "=" + View, ParamKeys.ForumId + "=" + ForumID, ParamKeys.PageId + "=" + Page };
                }
                else
                {
                    Params2 = new string[] { ParamKeys.ViewType + "=" + View, ParamKeys.ForumId + "=" + ForumID };
                }

                if (UseShortUrls && Page > 1)
                {
                    Params2 = new string[] { ParamKeys.ForumId + "=" + ForumID, ParamKeys.PageId + "=" + Page };
                }
                else if (UseShortUrls && Page == 1)
                {
                    Params2 = new string[] { ParamKeys.ForumId + "=" + ForumID };
                }
                if (Params != null)
                {
                    int intLength = Params2.Length;
                    Array.Resize(ref Params2, (intLength + Params.Length));
                    Params.CopyTo(Params2, intLength);
                }

            }
            else if (View.ToLowerInvariant() == Views.Topic.ToLowerInvariant())
            {
                if (Page > 1)
                {
                    Params2 = new string[] { ParamKeys.ViewType + "=" + View, ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID, ParamKeys.PageId + "=" + Page };
                }
                else
                {
                    Params2 = new string[] { ParamKeys.ViewType + "=" + View, ParamKeys.ForumId + "=" + ForumID, ParamKeys.TopicId + "=" + PostID };
                }

                if (UseShortUrls && Page > 1)
                {
                    Params2 = new string[] { ParamKeys.TopicId + "=" + PostID, ParamKeys.PageId + "=" + Page };
                }
                else if (UseShortUrls && Page == 1)
                {
                    Params2 = new string[] { ParamKeys.TopicId + "=" + PostID };
                }
                if (Params != null)
                {
                    int intLength = Params2.Length;
                    Array.Resize(ref Params2, (intLength + Params.Length));
                    Params.CopyTo(Params2, intLength);
                }
            }
            else
            {
                if (Params != null)
                {
                    Params2 = new string[] { ParamKeys.ViewType + "=" + View, ParamKeys.PageId + "=" + Page };
                    int intLength = Params2.Length;
                    Array.Resize(ref Params2, (intLength + Params.Length));
                    Params.CopyTo(Params2, intLength);
                }
                else
                {
                    Params2 = new string[] { ParamKeys.ViewType + "=" + View, ParamKeys.PageId + "=" + Page };
                }
            }
            return Params2;
        }

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            this.EnableViewState = false;
        }
    }
}
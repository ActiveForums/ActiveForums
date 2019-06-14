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
    [DefaultProperty("Text"), ToolboxData("<{0}:Link runat=server></{0}:Link>")]
    public class Link : WebControl
    {
        private string _text;
        private bool _visible;
        private string _navigateUrl;
        private string _enabledRoles;
        private string _userRoles;
        private bool _authRequired;
        private int _pageId;
        private string _params;
        private string _title;
        public string NavigateURL
        {
            get
            {
                return _navigateUrl;
            }
            set
            {
                _navigateUrl = value;
            }
        }
        public string EnabledRoles
        {
            get
            {
                return _enabledRoles;
            }
            set
            {
                _enabledRoles = value;
            }
        }
        public string UserRoles
        {
            get
            {
                return _userRoles;
            }
            set
            {
                _userRoles = value;
            }
        }
        public bool AuthRequired
        {
            get
            {
                return _authRequired;
            }
            set
            {
                _authRequired = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }
        public int PageId
        {
            get
            {
                return _pageId;
            }
            set
            {
                _pageId = value;
            }
        }
        public string Params
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!(string.IsNullOrEmpty(EnabledRoles)))
            {
                if (string.IsNullOrEmpty(UserRoles))
                {
                    Visible = false;
                }
                else
                {
                    Visible = Permissions.HasAccess(EnabledRoles, UserRoles);
                }
            }
            if (AuthRequired && !HttpContext.Current.Request.IsAuthenticated)
            {
                Visible = false;
            }
            if (string.IsNullOrEmpty(NavigateURL) && string.IsNullOrEmpty(Params))
            {
                NavigateURL = Utilities.NavigateUrl(PageId);
            }
            else if (string.IsNullOrEmpty(NavigateURL) && !(string.IsNullOrEmpty(Params)))
            {
                NavigateURL = Utilities.NavigateUrl(PageId, "", Params.Split(','));
            }
            string sTitle = " title=";
            if (!(string.IsNullOrEmpty(Title)))
            {
                sTitle += "\"" + Title + "\"";
            }
            else
            {
                sTitle = string.Empty;
            }
            string sClass = string.Empty;
            if (!(string.IsNullOrEmpty(CssClass)))
            {
                sClass = " class=\"" + CssClass + "\"";
            }
            if (Visible)
            {
                if (Enabled)
                {
                    writer.Write("<a href=\"" + NavigateURL + "\"" + sTitle + sClass + ">");
                }
                writer.Write(Text);
                if (Enabled)
                {
                    writer.Write("</a>");
                }
            }

        }

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            EnableViewState = false;
        }
    }

}

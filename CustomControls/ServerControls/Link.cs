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

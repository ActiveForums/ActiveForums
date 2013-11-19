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

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [ToolboxData("<{0}:TagCloud runat=server></{0}:TagCloud>")]
    public class TagCloud : WebControl
    {
        private int _siteId = -1;
        private int _instanceId = -1;
        private string _cssOne = "tagcssone";
        private string _cssTwo = "tagcsstwo";
        private string _cssThree = "tagcssthree";
        private int _TabId = -1;
        public int TabId
        {
            get
            {
                return _TabId;
            }
            set
            {
                _TabId = value;
            }
        }
        public int PortalId
        {
            get
            {
                return _siteId;
            }
            set
            {
                _siteId = value;
            }
        }
        public int ModuleId
        {
            get
            {
                return _instanceId;
            }
            set
            {
                _instanceId = value;
            }
        }
        public string CSSOne
        {
            get
            {
                return _cssOne;
            }
            set
            {
                _cssOne = value;
            }
        }
        public string CSSTwo
        {
            get
            {
                return _cssTwo;
            }
            set
            {
                _cssTwo = value;
            }
        }
        public string CSSThree
        {
            get
            {
                return _cssThree;
            }
            set
            {
                _cssThree = value;
            }
        }
        private string _ForumIds = string.Empty;
        public string ForumIds
        {
            get
            {
                return _ForumIds;
            }
            set
            {
                _ForumIds = value;
            }
        }
        private int _TagCount = 15;
        public int TagCount
        {
            get
            {
                return _TagCount;
            }
            set
            {
                _TagCount = value;
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            User forumUser = null;
            if (string.IsNullOrEmpty(ForumIds))
            {
                UserController uc = new UserController();
                forumUser = uc.GetUser(PortalId, ModuleId);
                if (string.IsNullOrEmpty(forumUser.UserForums))
                {
                    ForumController fc = new ForumController();
                    ForumIds = fc.GetForumsForUser(forumUser.UserRoles, PortalId, ModuleId);
                }
                else
                {
                    ForumIds = forumUser.UserForums;
                }
            }
            SettingsInfo _mainSettings = DataCache.MainSettings(ModuleId);
            Data.Common db = new Data.Common();
            IDataReader dr = db.TagCloud_Get(PortalId, ModuleId, ForumIds, TagCount);
            ControlUtils ctlUtils = new ControlUtils();
            string sURL = string.Empty;
            while (dr.Read())
            {
                int priority = 1;
                string tagName = string.Empty;
                string css = string.Empty;
                priority = int.Parse(dr["Priority"].ToString());
                tagName = dr["TagName"].ToString();
                switch (priority)
                {
                    case 1:
                        css = CSSOne;
                        break;
                    case 2:
                        css = CSSTwo;
                        break;
                    case 3:
                        css = CSSThree;
                        break;
                }
                writer.Write("<span class=\"" + css + "\">");
                writer.Write("<a href=\"");
                sURL = ctlUtils.BuildUrl(TabId, ModuleId, string.Empty, string.Empty, -1, -1, int.Parse(dr["TagID"].ToString()), -1, Utilities.CleanName(tagName), 1, -1);
                writer.Write(sURL);
                writer.Write("\" title=\"" + HttpUtility.HtmlAttributeEncode(tagName) + "\">" + tagName + "</a></span> ");
            }
            dr.Close();
            dr.Dispose();
        }

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            EnableViewState = false;
        }
    }
}
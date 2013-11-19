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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace DotNetNuke.Modules.ActiveForums
{
    public class ActiveAdminBase : Entities.Modules.PortalModuleBase
    {
        private string _Params = string.Empty;
        private string _currentView = string.Empty;
        private DateTime _CacheUpdatedTime;
        public const string RequiredImage = "~/DesktopModules/ActiveForums/Images/error.gif";
        
        #region Constants
        internal const string ViewKey = "afcpView";
        internal const string ParamKey = "afcpParams";
        internal const string DefaultView = "home";
        #endregion

        public string Params
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

        public bool IsCallBack { get; set; }

        public string HostURL
        {
            get
            {
                object obj = DataCache.CacheRetrieve(ModuleId + "HostURL");
                if (obj == null)
                {
                    string sURL;
                    if (Request.IsSecureConnection)
                    {
                        sURL = "https://" + Common.Globals.GetDomainName(Request) + "/";
                    }
                    else
                    {
                        sURL = "http://" + Common.Globals.GetDomainName(Request) + "/";
                    }
                    DataCache.CacheStore(ModuleId + "HostURL", sURL, DateTime.Now.AddMinutes(30));
                    return sURL;
                }
                return Convert.ToString(obj);
            }
        }
        public string GetWarningImage(string ImageId, string WarningMessage)
        {
            return "<img id=\"" + ImageId + "\" onmouseover=\"showTip(this,'" + WarningMessage + "');\" onmouseout=\"hideTip();\" alt=\"" + WarningMessage + "\" height=\"16\" width=\"16\" src=\"" + Page.ResolveUrl("~/DesktopModules/ActivePurchase/images/warning.gif") + "\" />";
        }
        protected string GetSharedResource(string key)
        {
            return Utilities.GetSharedResource(key, true);
        }
        public Hashtable ActiveSettings
        {
            get
            {
                return MainSettings.MainSettings;
            }
        }
        public SettingsInfo MainSettings
        {
            get
            {
                var _portalSettings = (Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
                var objModules = new Entities.Modules.ModuleController();
                var objSettings = new SettingsInfo {MainSettings = objModules.GetModuleSettings(ModuleId)};
                return objSettings;
            }
        }
        public DateTime CacheUpdatedTime
        {
            get
            {
                object obj = DataCache.CacheRetrieve(ModuleId + "CacheUpdate");
                if (obj != null)
                {
                    return Convert.ToDateTime(obj);
                }
                return DateTime.Now;
            }
            set
            {
                DataCache.CacheStore(ModuleId + "CacheUpdate", value);
                _CacheUpdatedTime = value;
            }
        }
        protected override void OnInit(EventArgs e)
        {
 	         base.OnInit(e);

            LocalResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/ControlPanel.ascx.resx";
        }

        internal string ScriptEscape(string escape)
        {
            escape = escape.Replace("'", "\\'");
            escape = escape.Replace("\"", "\\\"");
            return escape;
        }
        public string LocalizeControl(string controlText)
        {
            return Utilities.LocalizeControl(controlText, true);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            var stringWriter = new System.IO.StringWriter();
            var htmlWriter = new HtmlTextWriter(stringWriter);
            base.Render(htmlWriter);
            string html = stringWriter.ToString();
            html = LocalizeControl(html);
            writer.Write(html);
        }
        public Controls.ClientTemplate GetLoadingTemplate()
        {
            var template = new Controls.ClientTemplate {ID = "LoadingTemplate"};
            template.Controls.Add(new LiteralControl("<div class=\"amloading\"><div class=\"amload\"><img src=\"" + Page.ResolveUrl("~/desktopmodules/activeforums/images/spinner.gif") + "\" align=\"absmiddle\" alt=\"Loading\" />Loading...</div></div>"));
            return template;
        }
        public Controls.ClientTemplate GetLoadingTemplateSmall()
        {
            var template = new Controls.ClientTemplate {ID = "LoadingTemplate"};
            template.Controls.Add(new LiteralControl("<div style=\"text-align:center;font-family:Tahoma;font-size:10px;\"><img src=\"" + Page.ResolveUrl("~/desktopmodules/activeforums/images/spinner.gif") + "\" align=\"absmiddle\" alt=\"Loading\" />Loading...</div>"));
            return template;
        }
        public void BindTemplateDropDown(DropDownList drp, Templates.TemplateTypes TemplateType, string DefaultText, string DefaultValue)
        {
            var tc = new TemplateController();
            drp.DataTextField = "Title";
            drp.DataValueField = "TemplateID";
            drp.DataSource = tc.Template_List(PortalId, ModuleId, TemplateType);
            drp.DataBind();
            drp.Items.Insert(0, new ListItem(DefaultText, DefaultValue));
        }
        public string CurrentView
        {
            get
            {
                if (Session[ViewKey] != null)
                {
                    return Session[ViewKey].ToString();
                }
                if (_currentView != string.Empty)
                {
                    return _currentView;
                }
                return DefaultView;
            }
            set
            {
                Session[ViewKey] = value;
                _currentView = value;
            }
        }
        public string ProductEditon
        {
            get
            {
                return string.Empty;
            }

        }
    }
}
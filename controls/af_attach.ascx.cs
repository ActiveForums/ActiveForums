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
using DotNetNuke.Framework;
using DotNetNuke.Web.Client.ClientResourceManagement;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    public partial class af_attach : ForumBase
    {
        public EditorTypes EditorType { get; set; }

        public string AttachmentsClientId { get; set;  }

        protected override void OnInit(EventArgs e)
		{
            base.OnInit(e);

            LocalResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx";

            jQuery.RegisterJQuery(Page);
            jQuery.RegisterJQueryUI(Page);
            jQuery.RequestDnnPluginsRegistration();
            jQuery.RegisterFileUpload(Page);

            ClientResourceManager.RegisterScript(Page, "~/DesktopModules/ActiveForums/scripts/jquery.afFileUpload.js", 102);

            /*
            var version = System.Reflection.Assembly.GetAssembly(typeof(Common.Globals)).GetName().Version;
            if (version != null && version.Major == 7 && version.Minor < 2)
            {
                // v7.0, 7.1
                ClientResourceManager.RegisterScript(Page, "~/DesktopModules/Journal/scripts/jquery.iframe-transport.js", 101);
                ClientResourceManager.RegisterScript(Page, "~/DesktopModules/Journal/scripts/jquery.dnnUserFileUpload.js", 102);
            }
            else
            {
                // v7.2+
                ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Components/UserFileManager/jquery.dnnUserFileUpload.js", 102);
            }
            */

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
        }
    }
}

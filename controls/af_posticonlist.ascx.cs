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
    public partial class af_posticonlist : System.Web.UI.UserControl
    {
        private string _Theme;
        private string _PostIcon;
        public string Theme
        {
            get
            {
                return _Theme;
            }
            set
            {
                _Theme = value;
            }
        }
        public string PostIcon
        {
            get
            {
                string tempPostIcon = null;
                return _PostIcon; //PostIcon = rblMessageIcons1.SelectedItem.Value
            }
            set
            {
                _PostIcon = value;
            }
        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                //If Not Page.IsPostBack Then
                LoadPostIcons();
                if (!(string.IsNullOrEmpty(PostIcon)))
                {
                    rblMessageIcons1.SelectedIndex = rblMessageIcons1.Items.IndexOf(rblMessageIcons1.Items.FindByValue(PostIcon));
                }
                //End If


            }
            catch (Exception ex)
            {

            }
        }
        private void LoadPostIcons()
        {
            rblMessageIcons1.Items.Clear();
            string MyTheme = Theme;
            string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/";
            rblMessageIcons1.Items.Insert(0, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/biggrin.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "biggrin.gif"));
            rblMessageIcons1.Items.Insert(1, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/crazy.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "crazy.gif"));
            rblMessageIcons1.Items.Insert(2, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/cry.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "cry.gif"));
            rblMessageIcons1.Items.Insert(3, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/arrow.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "arrow.gif"));
            rblMessageIcons1.Items.Insert(4, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/hazard.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "hazard.gif"));
            rblMessageIcons1.Items.Insert(5, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/explanationmark.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "explanationmark.gif"));
            rblMessageIcons1.Items.Insert(6, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/w00t.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "w00t.gif"));
            rblMessageIcons1.Items.Insert(7, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/pinch.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "pinch.gif"));
            rblMessageIcons1.Items.Insert(8, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/whistling.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "whistling.gif"));
            rblMessageIcons1.Items.Insert(9, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/sad.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "sad.gif"));
            rblMessageIcons1.Items.Insert(10, new ListItem("<img src=\"" + strHost + "DesktopModules/ActiveForums/themes/" + MyTheme + "/emoticons/questionmark.gif\" width=\"20\" height=\"20\" align=\"absmiddle\">&nbsp;&nbsp;&nbsp;&nbsp;", "questionmark.gif"));
            rblMessageIcons1.Items.Insert(11, new ListItem(Utilities.GetSharedResource("[RESX:PostIconNone]"), string.Empty));
            rblMessageIcons1.SelectedIndex = 11;
        }
    }
}

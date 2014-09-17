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

// TODO: Orphan Control??  Not Currently used - JB

using System;
using System.Web;
using System.Collections.Generic;

namespace DotNetNuke.Modules.ActiveForums
{

    public partial class af_searchquick : ForumBase
    {
        public int MID;
        public int FID;


        #region Event Handlers

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                ForumModuleId = MID;
                if (ForumId < 1)
                {
                    ForumId = FID;
                }

                if (Request.QueryString["GroupId"] != null && SimulateIsNumeric.IsNumeric(Request.QueryString["GroupId"]))
                {
                    SocialGroupId = Convert.ToInt32(Request.QueryString["GroupId"]);
                }

                //Put user code to initialize the page here
                txtSearch.Attributes.Add("onkeydown", "if(event.keyCode == 13){document.getElementById('" + lnkSearch.ClientID + "').click();}");
            }
            catch (Exception exc)
            {
                Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion


        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            LocalResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx";

            lnkSearch.Click += lnkSearch_Click;
        }


        private void lnkSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text.Trim() != "")
            {
                var @params = new List<string> { ParamKeys.ViewType + "=search", ParamKeys.ForumId + "=" + ForumId, "q=" + HttpUtility.UrlEncode(txtSearch.Text.Trim()) };

                if (SocialGroupId > 0)
                    @params.Add("GroupId=" + SocialGroupId.ToString());

                Response.Redirect(NavigateUrl(TabId, "", @params.ToArray()));
            }

        }
    }

}

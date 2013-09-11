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

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class admin_tags : ActiveAdminBase
    {

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            agTags.Callback += agTags_Callback;
            agTags.ItemBound += agTags_ItemBound;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void agTags_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            try
            {
                if (!(e.Parameters[4] == ""))
                {
                    string sAction = e.Parameters[4].Split(':')[0];

                    switch (sAction.ToUpper())
                    {
                        case "DELETE":
                            {
                                int TagId = Convert.ToInt32(e.Parameters[4].Split(':')[1]);
                                if (SimulateIsNumeric.IsNumeric(TagId))
                                {
                                    DataProvider.Instance().Tags_Delete(PortalId, ModuleId, TagId);
                                }
                                break;
                            }
                        case "SAVE":
                            {
                                string[] sParams = e.Parameters[4].Split(':');
                                string TagName = sParams[1].Trim();
                                int TagId = 0;
                                if (sParams.Length > 2)
                                {
                                    TagId = Convert.ToInt32(sParams[2]);
                                }
                                if (!(TagName == string.Empty))
                                {
                                    DataProvider.Instance().Tags_Save(PortalId, ModuleId, TagId, TagName, 0, 0, 0, -1, false, -1, -1);
                                }



                                break;
                            }
                    }

                }
                agTags.DefaultParams = string.Empty;
                int PageIndex = Convert.ToInt32(e.Parameters[0]);
                int PageSize = Convert.ToInt32(e.Parameters[1]);
                string SortColumn = e.Parameters[2].ToString();
                string Sort = e.Parameters[3].ToString();
                agTags.Datasource = DataProvider.Instance().Tags_List(PortalId, ModuleId, false, PageIndex, PageSize, Sort, SortColumn, -1, -1);
                agTags.Refresh(e.Output);
            }
            catch (Exception ex)
            {

            }

        }

        private void agTags_ItemBound(object sender, Modules.ActiveForums.Controls.ItemBoundEventArgs e)
        {
            //e.Item(1) = Server.HtmlEncode(e.Item(1).ToString)
            //e.Item(2) = Server.HtmlEncode(e.Item(2).ToString)
            e.Item[4] = "<img src=\"" + Page.ResolveUrl("~/desktopmodules/activeforums/images/delete16.png") + "\" alt=\"" + GetSharedResource("[RESX:Delete]") + "\" height=\"16\" width=\"16\" />";
        }
        #endregion

    }
}

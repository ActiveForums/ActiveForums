//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class admin_filters_new : ActiveAdminBase
    {
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            agFilters.Callback += agFilters_Callback;
            agFilters.ItemBound += agFilters_ItemBound;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            agFilters.ColDelimiter = "||";
        }

        private void agFilters_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            try
            {
                if (!(e.Parameters[4] == ""))
                {
                    string sAction = e.Parameters[4].Split(':')[0];
                    int FilterId = Convert.ToInt32(e.Parameters[4].Split(':')[1]);
                    switch (sAction.ToUpper())
                    {
                        case "DELETE":
                            if (SimulateIsNumeric.IsNumeric(FilterId))
                            {
                                DataProvider.Instance().Filters_Delete(PortalId, ModuleId, FilterId);
                            }
                            break;
                        case "DEFAULTS":
                            DataProvider.Instance().Filters_DeleteByModuleId(PortalId, ModuleId);
                            Utilities.ImportFilter(PortalId, ModuleId);
                            break;
                    }

                }
                int PageIndex = Convert.ToInt32(e.Parameters[0]);
                int PageSize = Convert.ToInt32(e.Parameters[1]);
                string SortColumn = e.Parameters[2].ToString();
                string Sort = e.Parameters[3].ToString();
                agFilters.Datasource = DataProvider.Instance().Filters_List(PortalId, ModuleId, PageIndex, PageSize, Sort, SortColumn);
                agFilters.Refresh(e.Output);
            }
            catch (Exception ex)
            {

            }

        }

        private void agFilters_ItemBound(object sender, Modules.ActiveForums.Controls.ItemBoundEventArgs e)
        {
            e.Item[1] = Server.HtmlEncode(e.Item[1].ToString());
            e.Item[2] = Server.HtmlEncode(e.Item[2].ToString());
            e.Item[4] = "<img src=\"" + Page.ResolveUrl("~/desktopmodules/activeforums/images/delete16.png") + "\" alt=\"" + GetSharedResource("[RESX:Delete]") + "\" height=\"16\" width=\"16\" />";
        }
        #endregion
    }
}

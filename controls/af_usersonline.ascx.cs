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

using DotNetNuke;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_usersonline : ForumBase
    {
        #region Public Members
        public string DisplayMode;
        public int pid = 0;
        #endregion
        #region Private Members
        private int intGuestCount = 0;
        private int intMemberCount = 0;
        #endregion
        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                //cbUsersOnline.PostURL = Page.ResolveUrl("~/DesktopModules/activeforums/cb.aspx")
                //cbUsersOnline.Parameter = "uo|" & PortalId.ToString & "|" & ModuleId.ToString & "|" & Me.UserId.ToString
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "amaf_uo", "setInterval('amaf_uo()',50000);", true);
                bool bolShow = true;
                if (Request.Cookies["WHOSShow"] != null)
                {
                    bolShow = Convert.ToBoolean(Request.Cookies["WHOSShow"].Value);
                }

                if (bolShow)
                {
                    DisplayMode = " style=\"display:block;\"";
                }
                else
                {
                    DisplayMode = " style=\"display:none;\"";
                }
                BindUsersOnline();
                //hidUserId.Value = CStr(Me.UserId)
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
            }

        }
        #endregion

        #region Private Methods
        private void BindUsersOnline()
        {
            UsersOnline uo = new UsersOnline();
            string sOnlineList = uo.GetUsersOnline(PortalId, ModuleId, ForumUser);
            IDataReader dr = DataProvider.Instance().Profiles_GetStats(PortalId, -1, 2);
            int anonCount = 0;
            int memCount = 0;
            int memTotal = 0;
            while (dr.Read())
            {
                anonCount = Convert.ToInt32(dr["Guests"]);
                memCount = Convert.ToInt32(dr["Members"]);
                memTotal = Convert.ToInt32(dr["MembersTotal"]);
            }
            dr.Close();
            string sGuestsOnline = null;
            string sUsersOnline = null;
            sGuestsOnline = Utilities.GetSharedResource("[RESX:GuestsOnline]");
            sUsersOnline = Utilities.GetSharedResource("[RESX:UsersOnline]");
            litGuestsOnline.Text = sGuestsOnline.Replace("[GUESTCOUNT]", anonCount.ToString());
            sUsersOnline = sUsersOnline.Replace("[USERCOUNT]", memCount.ToString());
            sUsersOnline = sUsersOnline.Replace("[TOTALMEMBERCOUNT]", memTotal.ToString());
            litUsersOnline.Text = sUsersOnline + " " + sOnlineList;
        }

        #endregion

    }
}

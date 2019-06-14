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

using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class admin_ranks_new : ActiveAdminBase
	{
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);


            agRanks.Callback += agRanks_Callback;
            agRanks.ItemBound += agRanks_ItemBound;

        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			BindRankImages();

		}

		private void agRanks_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
		{
			agRanks.Datasource = DataProvider.Instance().Ranks_List(PortalId, ModuleId);
			agRanks.Refresh(e.Output);
		}

		private void agRanks_ItemBound(object sender, Modules.ActiveForums.Controls.ItemBoundEventArgs e)
		{
			e.Item[4] = GetDisplay(e.Item[4].ToString(), e.Item[1].ToString());
		}
		public string GetDisplay(string Display, string RankName)
		{
			return "<img src=\"" + HostURL + Display.Replace("activeforums/Ranks", "activeforums/images/Ranks") + "\" border=\"0\" alt=\"" + RankName + "\" />";
		}
		private void BindRankImages()
		{
			string[] FileCollection = null;
			System.IO.FileInfo myFileInfo = null;
			int i = 0;

			FileCollection = System.IO.Directory.GetFiles(Server.MapPath("~/DesktopModules/ActiveForums/Images/ranks"));
			for (i = 0; i < FileCollection.Length; i++)
			{
				string path = null;
				myFileInfo = new System.IO.FileInfo(FileCollection[i]);
				path = "DesktopModules/activeforums/Images/Ranks/" + myFileInfo.Name;
				drpRankImages.Items.Insert(i, new ListItem(myFileInfo.Name, path.ToLowerInvariant()));

			}
			drpRankImages.Items.Insert(0, new ListItem("[RESX:DropDownDefault]", "-1"));
			// drpRankImages.Items.Insert(1, New ListItem(Utilities.GetSharedResource("RankCustom.Text"), "0"))
		}
	}
}

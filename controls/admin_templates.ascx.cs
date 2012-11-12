using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class admin_templates_new : ActiveAdminBase
	{
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            agTemplates.Callback += agTemplates_Callback;
            agTemplates.ItemBound += agTemplates_ItemBound;

        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		private void agTemplates_Callback(object sender, Controls.CallBackEventArgs e)
		{
			int PageIndex = Convert.ToInt32(e.Parameters[0]);
			int PageSize = Convert.ToInt32(e.Parameters[1]);
			int RowIndex = 0;
			if (PageIndex == 0)
			{
				RowIndex = 0;
			}
			else
			{
				RowIndex = (((PageIndex + 1) * PageSize) - PageSize);
			}
			agTemplates.Datasource = DataProvider.Instance().Templates_List(PortalId, ModuleId, 0, RowIndex, PageSize);
			agTemplates.Refresh(e.Output);
		}

		private void agTemplates_ItemBound(object sender, Modules.ActiveForums.Controls.ItemBoundEventArgs e)
		{
			string sValue = string.Empty;
			try
			{
				sValue = GetSharedResource("[RESX:" + Convert.ToString(Enum.Parse(typeof(Templates.TemplateTypes), e.Item[1].ToString())) + "]");
			}
			catch (Exception ex)
			{
				sValue = e.Item[1].ToString();
			}
			e.Item[1] = sValue;
		}
	}
}
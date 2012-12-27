using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class _default : ForumBase
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			btnContinue.Visible = this.HasModulePermission("EDIT");
            btnContinue.Click += new System.EventHandler(btnContinue_Click);
		}

		private void btnContinue_Click(object sender, System.EventArgs e)
		{
			ForumsConfig fc = new ForumsConfig();
			bool init = false;
			init = fc.ForumsInit(PortalId, ModuleId);
			if (init == true)
			{
				DotNetNuke.Entities.Modules.ModuleController objModules = new DotNetNuke.Entities.Modules.ModuleController();
				objModules.UpdateModuleSetting(ModuleId, "AFINSTALLED", init.ToString());
				DataCache.ClearAllCache(ModuleId, TabId);
				Response.Redirect(EditUrl());
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DotNetNuke.Services.Localization;
using DotNetNuke.Framework;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    public partial class af_attach : ForumBase
    {
        public EditorTypes EditorType { get; set; }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

        }
    }
}

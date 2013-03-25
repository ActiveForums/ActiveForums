using System;
using DotNetNuke.Framework;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    public partial class af_topicscripts : ForumBase
    {
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            ServicesFramework.Instance.RequestAjaxScriptSupport();
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AFUserEditor")))
                Page.ClientScript.RegisterClientScriptInclude("AFUserEditor", Page.ResolveUrl("~/DesktopModules/activeforums/scripts/usereditor.js"));
        }
    }
}

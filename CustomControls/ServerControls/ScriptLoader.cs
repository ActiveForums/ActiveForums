using System;
using System.ComponentModel;
using System.Web.UI;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:ScriptLoader runat=server></{0}:ScriptLoader>")]
    public class ScriptLoader : Control
    {
        #region Properties

        public bool TextSuggest { get; set; }

        public bool ActiveGrid { get; set; }

        public bool Callback { get; set; }

        public bool DatePicker { get; set; }

        public bool RequiredFieldValidator { get; set; }

        #endregion

        public ScriptLoader()
        {
            RequiredFieldValidator = false;
            DatePicker = false;
            Callback = false;
            ActiveGrid = false;
            TextSuggest = false;
        }

        #region Subs/Functions

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            /*if (TextSuggest && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMTextSuggest")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMTextSuggest", Page.ClientScript.GetWebResourceUrl(Me.GetType, "TextSuggest.js"))
            }*/
            if (ActiveGrid && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMActiveGrid")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMActiveGrid", Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.ActiveGrid.js"));
            }
            if (Callback && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMCallback")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMCallback", Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.cb.js"));
            }
            if (DatePicker && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMDatePicker")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMDatePicker", Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.DatePicker.js"));
            }
            if (RequiredFieldValidator && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMValidation")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMValidation", Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.Validation.js"));
            }
            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMMenu")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMMenu", Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.MenuButton.js"));
            }
        }

        #endregion

    }
}
